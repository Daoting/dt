#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-31 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 针对ICacheItem的缓存管理
    /// </summary>
    class CacheHandler
    {
        #region lua脚本
        // 先查出主键值，再查询实体json
        const string _luaGet =
            "local id = redis.call('GET', KEYS[1])\n" +
            "if not id then\n" +
            "  return nil\n" +
            "end\n" +
            "local key = ARGV[1] .. ':' .. id\n" +
            "return redis.call('GET', key)\n";
        // 批量删除
        const string _luaDel =
            "for i = 1,#(KEYS) do\n" +
            "  redis.call('del', KEYS[i])\n" +
            "end\n" +
            "return true";
        static byte[] _sha1LuaGet;
        static byte[] _sha1LuaDel;

        static CacheHandler()
        {
            var server = Redis.Server;

            // 服务器上预加载脚本
            _sha1LuaGet = CalcLuaSha1(_luaGet);
            if (!server.ScriptExists(_sha1LuaGet))
                _sha1LuaGet = server.ScriptLoad(_luaGet);

            _sha1LuaDel = CalcLuaSha1(_luaDel);
            if (!server.ScriptExists(_sha1LuaDel))
                _sha1LuaDel = server.ScriptLoad(_luaDel);
        }

        static byte[] CalcLuaSha1(string p_lua)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var bytesSha1In = Encoding.UTF8.GetBytes(p_lua);
            return sha1.ComputeHash(bytesSha1In);
        }
        #endregion

        readonly CacheDesc _primaryItem;
        readonly Dictionary<string, CacheDesc> _cacheItems;
        readonly Lazy<IDatabase> _db;
        readonly Type _type;
        readonly TimeSpan? _expiry;
        readonly string _sqlGet;

        public CacheHandler(Type p_type)
        {
            var cfg = p_type.GetCustomAttribute<CacheCfgAttribute>(false);
            if (cfg == null || !cfg.Validate())
                throw new Exception($"类型{p_type.Name}缺少缓存项配置！");

            // 只支持单主键的缓存
            var schema = DbSchema.GetTableSchema(cfg.TblName);
            if (schema.PrimaryKey.Count == 0 || schema.PrimaryKey.Count > 1)
                throw new Exception($"表{cfg.TblName}不是单个主键！");

            // 过期时间
            if (cfg.ExpiryHour > 0)
                _expiry = TimeSpan.FromHours(cfg.ExpiryHour);

            // 主键缓存项
            string priKey = schema.PrimaryKey[0].Name;
            var prop = p_type.GetProperty(priKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null)
                _primaryItem = new CacheDesc($"{cfg.PrefixKey.ToLower()}:{priKey}", prop);

            // 其它唯一键缓存项
            if (!string.IsNullOrEmpty(cfg.OtherKey))
            {
                _cacheItems = new Dictionary<string, CacheDesc>(StringComparer.OrdinalIgnoreCase);
                foreach (string name in cfg.OtherKey.Split(','))
                {
                    string keyName = name.Trim().ToLower();
                    if (keyName == priKey || keyName == "")
                        continue;

                    prop = p_type.GetProperty(keyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop != null)
                        _cacheItems[keyName] = new CacheDesc($"{cfg.PrefixKey.ToLower()}:{keyName}", prop);
                }
                if (_cacheItems.Count == 0)
                    _cacheItems = null;
            }

            _type = p_type;
            _sqlGet = $"select * from {cfg.TblName} where " + "{0}=@{0}";
            _db = new Lazy<IDatabase>(() => Redis.Db);
        }

        public Task Cache(object p_entity)
        {
            Check.NotNull(p_entity);
            string val = JsonConvert.SerializeObject(p_entity);
            string id = _primaryItem.PropInfo.GetValue(p_entity).ToString();

            // 只主键
            if (_cacheItems == null)
                return _db.Value.StringSetAsync($"{_primaryItem.KeyPrefix}:{id}", val, _expiry);

            // 多个缓存键
            var batch = _db.Value.CreateBatch();
            var tasks = new List<Task>();
            tasks.Add(batch.StringSetAsync($"{_primaryItem.KeyPrefix}:{id}", val, _expiry));
            foreach (var item in _cacheItems.Values)
            {
                // 缓存的值为主键值，不是实体的json！
                var propVal = item.PropInfo.GetValue(p_entity);
                if (propVal != null)
                    tasks.Add(batch.StringSetAsync($"{item.KeyPrefix}:{propVal}", id, _expiry));
            }
            batch.Execute();
            return Task.WhenAll(tasks);
        }

        public async Task<TEntity> Get<TEntity>(string p_keyName, string p_keyVal)
        {
            if (_primaryItem.PropInfo.Name.Equals(p_keyName, StringComparison.OrdinalIgnoreCase))
            {
                var val = await _db.Value.StringGetAsync($"{_primaryItem.KeyPrefix}:{p_keyVal}");
                if (val.HasValue)
                    return JsonConvert.DeserializeObject<TEntity>(val);
            }
            else if (_cacheItems != null && _cacheItems.TryGetValue(p_keyName, out var item))
            {
                // lua脚本：先查到主键值，再用主键值查询json
                var result = await _db.Value.ScriptEvaluateAsync(_sha1LuaGet, new RedisKey[] { $"{item.KeyPrefix}:{p_keyVal}" }, new RedisValue[] { _primaryItem.KeyPrefix });
                if (!result.IsNull)
                    return JsonConvert.DeserializeObject<TEntity>((string)result);
            }

            // 缓存没有，查询db
            Db db = new Db(false);
            var tgt = await db.First(_type, string.Format(_sqlGet, p_keyName), new Dict { { p_keyName, p_keyVal } });
            if (tgt != null)
            {
                // 缓存结果
                await ((ICacheItem)tgt).Init(db);
                await Cache(tgt);
            }
            await db.Close(true);
            return (TEntity)tgt;
        }

        public Task Remove(object p_entity)
        {
            string id = _primaryItem.PropInfo.GetValue(p_entity).ToString();

            // 只删除主键
            if (_cacheItems == null)
                return _db.Value.KeyDeleteAsync($"{_primaryItem.KeyPrefix}:{id}");

            // 删除多键
            List<RedisKey> ls = new List<RedisKey>();
            ls.Add($"{_primaryItem.KeyPrefix}:{id}");
            foreach (var item in _cacheItems.Values)
            {
                var propVal = item.PropInfo.GetValue(p_entity);
                if (propVal != null)
                    ls.Add($"{item.KeyPrefix}:{propVal}");
            }

            // lua脚本：批量删除
            return _db.Value.ScriptEvaluateAsync(_sha1LuaDel, ls.ToArray());
        }

        public async Task RemoveByID(string p_id)
        {
            // 只删除主键
            string priKey = $"{_primaryItem.KeyPrefix}:{p_id}";
            if (_cacheItems == null)
            {
                await _db.Value.KeyDeleteAsync(priKey);
                return;
            }

            // 删除多键
            var val = await _db.Value.StringGetAsync(priKey);
            if (!val.HasValue)
                return;

            object entity = JsonConvert.DeserializeObject(val, _type);
            List<RedisKey> ls = new List<RedisKey>();
            ls.Add(priKey);
            foreach (var item in _cacheItems.Values)
            {
                var propVal = item.PropInfo.GetValue(entity);
                if (propVal != null)
                    ls.Add($"{item.KeyPrefix}:{propVal}");
            }

            // lua脚本：批量删除
            await _db.Value.ScriptEvaluateAsync(_sha1LuaDel, ls.ToArray());
        }

        class CacheDesc
        {
            public readonly string KeyPrefix;
            public readonly PropertyInfo PropInfo;

            public CacheDesc(string p_keyPrefix, PropertyInfo p_propInfo)
            {
                KeyPrefix = p_keyPrefix;
                PropInfo = p_propInfo;
            }
        }
    }
}
