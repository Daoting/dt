#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Caches;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 针对Entity的缓存管理
    /// </summary>
    class CacheHandler<TEntity>
        where TEntity : Entity
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

        readonly string _prefix;
        readonly string _primaryKey;
        readonly string[] _otherKeys;
        readonly Lazy<IDatabase> _db;
        readonly TimeSpan? _expiry;

        public CacheHandler(EntitySchema p_model, CacheAttribute p_cfg)
        {
            // 只支持单主键的缓存
            if (p_model.Schema.PrimaryKey.Count == 0 || p_model.Schema.PrimaryKey.Count > 1)
                throw new Exception($"表{p_model.Schema.Name}不是单个主键！");

            // 过期时间
            if (p_cfg.ExpiryHour > 0)
                _expiry = TimeSpan.FromHours(p_cfg.ExpiryHour);

            // 缓存键前缀
            _prefix = p_cfg.PrefixKey.ToLower();

            // 主键缓存项
            _primaryKey = p_model.Schema.PrimaryKey[0].Name;

            // 其它唯一键缓存项
            if (!string.IsNullOrEmpty(p_cfg.OtherKey))
                _otherKeys = p_cfg.OtherKey.Split(',');

            _db = new Lazy<IDatabase>(() => Redis.Db);
        }

        public Task Cache(TEntity p_entity)
        {
            Check.NotNull(p_entity);
            string val = Serialize(p_entity);
            string id = p_entity.Str(_primaryKey);

            // 只主键
            if (_otherKeys == null)
                return _db.Value.StringSetAsync($"{_prefix}:{_primaryKey}:{id}", val, _expiry);

            // 多个缓存键
            var batch = _db.Value.CreateBatch();
            var tasks = new List<Task>();
            tasks.Add(batch.StringSetAsync($"{_prefix}:{_primaryKey}:{id}", val, _expiry));
            foreach (var item in _otherKeys)
            {
                // 缓存的值为主键值，不是实体的json！
                var itemVal = p_entity.Str(item);
                if (itemVal != string.Empty)
                    tasks.Add(batch.StringSetAsync($"{_prefix}:{item}:{itemVal}", id, _expiry));
            }
            batch.Execute();
            return Task.WhenAll(tasks);
        }

        public async Task<TEntity> Get(string p_keyName, string p_keyVal)
        {
            if (_primaryKey.Equals(p_keyName, StringComparison.OrdinalIgnoreCase))
            {
                var val = await _db.Value.StringGetAsync($"{_prefix}:{_primaryKey}:{p_keyVal}");
                if (val.HasValue)
                    return Deserialize(val);
            }
            else if (_otherKeys != null)
            {
                foreach (var item in _otherKeys)
                {
                    if (!item.Equals(p_keyName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // lua脚本：先查到主键值，再用主键值查询json
                    var result = await _db.Value.ScriptEvaluateAsync(_sha1LuaGet, new RedisKey[] { $"{_prefix}:{item}:{p_keyVal}" }, new RedisValue[] { $"{_prefix}:{_primaryKey}" });
                    if (!result.IsNull)
                        return Deserialize((string)result);
                    break;
                }
            }
            return default;
        }

        public Task Remove(TEntity p_entity)
        {
            Check.NotNull(p_entity);
            // 实体信息可能不全，根据缓存实体执行删除！
            string id = p_entity.Str(_primaryKey);
            return RemoveByID(id);
        }

        public async Task RemoveByID(string p_id)
        {
            // 只删除主键
            string priKey = $"{_prefix}:{_primaryKey}:{p_id}";
            if (_otherKeys == null)
            {
                await _db.Value.KeyDeleteAsync(priKey);
                return;
            }

            // 删除多键
            var val = await _db.Value.StringGetAsync(priKey);
            if (!val.HasValue)
                return;

            var entity = Deserialize(val);
            List<RedisKey> ls = new List<RedisKey>();
            ls.Add(priKey);
            foreach (var item in _otherKeys)
            {
                var itemVal = entity.Str(item);
                if (itemVal != string.Empty)
                    ls.Add($"{_prefix}:{item}:{itemVal}");
            }

            // lua脚本：批量删除
            await _db.Value.ScriptEvaluateAsync(_sha1LuaDel, ls.ToArray());
        }

        string Serialize(TEntity p_entity)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sr))
            {
                JsonRpcSerializer.Serialize(p_entity, writer);
                writer.Flush();
            }
            return sb.ToString();
        }

        TEntity Deserialize(string p_val)
        {
            using (StringReader sr = new StringReader(p_val))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                reader.Read();
                return JsonRpcSerializer.Deserialize<TEntity>(reader);
            }
        }
    }
}
