#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Caches;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 针对具体Entity的缓存管理
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

        readonly CacheItem _primaryItem;
        readonly Dictionary<string, CacheItem> _cacheItems;
        readonly Lazy<IDatabase> _db;

        public CacheHandler(Type p_type, TagAttribute p_tag)
        {
            // 只支持单主键的缓存
            string tblName = !string.IsNullOrEmpty(p_tag.TblName) ? p_tag.TblName : p_type.Name;
            var schema = DbSchema.GetTableSchema(tblName);
            if (schema.PrimaryKey.Count == 0 || schema.PrimaryKey.Count > 1)
                return;

            string priKey = schema.PrimaryKey[0].Name;
            if (!string.IsNullOrEmpty(p_tag.CacheKey))
            {
                _cacheItems = new Dictionary<string, CacheItem>(StringComparer.OrdinalIgnoreCase);
                foreach (string name in p_tag.CacheKey.Split(','))
                {
                    string keyName = name.Trim().ToLower();
                    if (keyName != "")
                    {
                        var prop = p_type.GetProperty(keyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (prop != null)
                        {
                            var item = new CacheItem($"{p_type.Name.ToLower()}:{keyName}", prop);
                            if (keyName == priKey)
                                _primaryItem = item;
                            else
                                _cacheItems[keyName] = item;
                        }
                    }
                }
            }
            else
            {
                var prop = p_type.GetProperty(priKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                    _primaryItem = new CacheItem($"{p_type.Name.ToLower()}:{priKey}", prop);
            }
            _db = new Lazy<IDatabase>(() => Redis.Db);
        }

        public bool IsCached
        {
            get { return _primaryItem != null || (_cacheItems != null && _cacheItems.Count > 0); }
        }

        bool OnlyPrimary
        {
            get { return _primaryItem != null && (_cacheItems == null || _cacheItems.Count == 0); }
        }

        bool PrimaryAndCommon
        {
            get { return _primaryItem != null && _cacheItems != null && _cacheItems.Count > 0; }
        }

        bool OnlyCommon
        {
            get { return _primaryItem == null && _cacheItems != null && _cacheItems.Count > 0; }
        }

        public Task Cache(object p_entity)
        {
            Check.NotNull(p_entity);
            string val = JsonConvert.SerializeObject(p_entity);
            if (OnlyPrimary)
            {
                var id = _primaryItem.PropInfo.GetValue(p_entity);
                return _db.Value.StringSetAsync($"{_primaryItem.KeyPrefix}:{id}", val);
            }

            if (OnlyCommon && _cacheItems.Count == 1)
            {
                var item = _cacheItems.Values.First();
                var propVal = item.PropInfo.GetValue(p_entity);
                return _db.Value.StringSetAsync($"{item.KeyPrefix}:{propVal}", val);
            }

            // 批量
            var batch = _db.Value.CreateBatch();
            var tasks = new List<Task>();
            if (PrimaryAndCommon)
            {
                var id = _primaryItem.PropInfo.GetValue(p_entity).ToString();
                tasks.Add(batch.StringSetAsync($"{_primaryItem.KeyPrefix}:{id}", val));

                foreach (var item in _cacheItems.Values)
                {
                    // 缓存的值为主键值，不是实体的json！
                    var propVal = item.PropInfo.GetValue(p_entity);
                    if (propVal != null)
                        tasks.Add(batch.StringSetAsync($"{item.KeyPrefix}:{propVal}", id));
                }
            }
            else
            {
                foreach (var item in _cacheItems.Values)
                {
                    // 缓存json！
                    var propVal = item.PropInfo.GetValue(p_entity);
                    if (propVal != null)
                        tasks.Add(batch.StringSetAsync($"{item.KeyPrefix}:{propVal}", val));
                }
            }
            batch.Execute();
            return Task.WhenAll(tasks);
        }

        public async Task<TEntity> Get<TEntity>(string p_keyName, string p_keyVal)
        {
            if (_primaryItem != null && _primaryItem.PropInfo.Name.Equals(p_keyName, StringComparison.OrdinalIgnoreCase))
            {
                var val = await _db.Value.StringGetAsync($"{_primaryItem.KeyPrefix}:{p_keyVal}");
                if (val.HasValue)
                    return JsonConvert.DeserializeObject<TEntity>(val);
                return default;
            }

            if (_cacheItems != null && _cacheItems.TryGetValue(p_keyName, out var item))
            {
                // 未以主键值为键缓存
                if (OnlyCommon)
                {
                    var val = await _db.Value.StringGetAsync($"{item.KeyPrefix}:{p_keyVal}");
                    if (val.HasValue)
                        return JsonConvert.DeserializeObject<TEntity>(val);
                    return default;
                }

                // lua脚本
                var result = await _db.Value.ScriptEvaluateAsync(_sha1LuaGet, new RedisKey[] { $"{item.KeyPrefix}:{p_keyVal}" }, new RedisValue[] { _primaryItem.KeyPrefix });
                if (!result.IsNull)
                    return JsonConvert.DeserializeObject<TEntity>((string)result);
            }
            return default;
        }

        public Task Remove(object p_entity)
        {
            if (OnlyPrimary)
            {
                var val = _primaryItem.PropInfo.GetValue(p_entity);
                return _db.Value.KeyDeleteAsync($"{_primaryItem.KeyPrefix}:{val}");
            }

            if (OnlyCommon && _cacheItems.Count == 1)
            {
                var item = _cacheItems.Values.First();
                var propVal = item.PropInfo.GetValue(p_entity);
                if (propVal != null)
                    return _db.Value.KeyDeleteAsync($"{item.KeyPrefix}:{propVal}");
                return Task.CompletedTask;
            }

            List<RedisKey> ls = new List<RedisKey>();
            if (_primaryItem != null)
            {
                var val = _primaryItem.PropInfo.GetValue(p_entity);
                ls.Add($"{_primaryItem.KeyPrefix}:{val}");
            }

            if (_cacheItems != null)
            {
                foreach (var item in _cacheItems.Values)
                {
                    var propVal = item.PropInfo.GetValue(p_entity);
                    if (propVal != null)
                        ls.Add($"{item.KeyPrefix}:{propVal}");
                }
            }
            return _db.Value.ScriptEvaluateAsync(_sha1LuaDel, ls.ToArray());
        }

        class CacheItem
        {
            public readonly string KeyPrefix;
            public readonly PropertyInfo PropInfo;

            public CacheItem(string p_keyPrefix, PropertyInfo p_propInfo)
            {
                KeyPrefix = p_keyPrefix;
                PropInfo = p_propInfo;
            }
        }
    }
}
