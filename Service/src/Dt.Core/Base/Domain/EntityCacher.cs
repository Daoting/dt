#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#if SERVER
using Dt.Core.Caches;
using StackExchange.Redis;
#endif
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 针对Entity的缓存管理
    /// </summary>
    class EntityCacher
    {
        #region 成员变量
        readonly EntitySchema _model;
        readonly List<string> _otherKeys;
        #endregion

        public EntityCacher(EntitySchema p_model, CacheAttribute p_cfg)
        {
            // 只支持单主键的缓存
            if (p_model.Schema.PrimaryKey.Count == 0 || p_model.Schema.PrimaryKey.Count > 1)
                throw new Exception($"表{p_model.Schema.Name}不是单个主键，不支持缓存！");

            _model = p_model;

            // 其它唯一键缓存项
            if (p_cfg.Keys != null && p_cfg.Keys.Length > 0)
            {
                var ls = new List<string>();
                foreach (var key in p_cfg.Keys)
                {
                    if (!string.IsNullOrEmpty(key))
                        ls.Add(key.ToLower());
                }

                if (ls.Count > 0)
                    _otherKeys = ls;
            }
        }

        public async Task Cache<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.IfNull(p_entity);
            string val = RpcKit.GetObjectString(p_entity);
            string id = p_entity.Str(_primaryKey);
            var idKey = $"{_prefix}:{_primaryKey}:{id}";

#if SERVER
            if (_otherKeys == null)
            {
                // 只主键
                await Redis.Db.StringSetAsync(idKey, val);
            }
            else
            {
                // 多个缓存键
                var batch = Redis.Db.CreateBatch();
                var tasks = new List<Task>();
                tasks.Add(batch.StringSetAsync(idKey, val));
                foreach (var item in _otherKeys)
                {
                    // 缓存的值为主键值，不是实体的json！
                    var itemVal = p_entity.Str(item);
                    if (itemVal != string.Empty)
                        tasks.Add(batch.StringSetAsync($"{_prefix}:{item}:{itemVal}", id));
                }
                batch.Execute();
                await Task.WhenAll(tasks);
            }

#else
            var ca = _model.AccessInfo.GetDataAccess();
            await ca.StringSet(idKey, val);
            if (_otherKeys != null)
            {
                foreach (var item in _otherKeys)
                {
                    // 缓存的值为主键值，不是实体的json！
                    var itemVal = p_entity.Str(item);
                    if (itemVal != string.Empty)
                        await ca.StringSet($"{_prefix}:{item}:{itemVal}", id);
                }
            }
#endif
        }

        public async Task<TEntity> Get<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            if (string.IsNullOrEmpty(p_keyName) || string.IsNullOrEmpty(p_keyVal))
                return default;

            var key = $"{_prefix}:{p_keyName.ToLower()}:{p_keyVal}";
            if (_primaryKey.Equals(p_keyName, StringComparison.OrdinalIgnoreCase))
            {
#if SERVER
                var val = await Redis.Db.StringGetAsync(key);
                if (val.HasValue)
                    return RpcKit.ParseString<TEntity>(val);
#else
                var val = await _model.AccessInfo.GetDataAccess().StringGet(key);
                if (!string.IsNullOrEmpty(val))
                    return RpcKit.ParseString<TEntity>(val);
#endif
            }
            else if (_otherKeys != null && _otherKeys.Contains(p_keyName.ToLower()))
            {
                string val;
                var priKeyPrefix = $"{_prefix}:{_primaryKey}";
#if SERVER
                val = await Redis.GetEntityJson(key, priKeyPrefix);
#else
                val = await _model.AccessInfo.GetDataAccess().GetEntityJson(key, priKeyPrefix);
#endif
                if (!string.IsNullOrEmpty(val))
                    return RpcKit.ParseString<TEntity>(val);
            }
            return default;
        }

        public Task Remove(Row p_entity)
        {
            Throw.IfNull(p_entity);
            // 实体信息可能不全，多键时根据缓存实体执行删除！
            string id = p_entity.Str(_primaryKey);
            return RemoveByID(id);
        }

        public async Task RemoveByID(string p_id)
        {
            // 只删除主键
            string priKey = $"{_prefix}:{_primaryKey}:{p_id}";
            if (_otherKeys == null)
            {
#if SERVER
                await Redis.Db.KeyDeleteAsync(priKey);
#else
                await _model.AccessInfo.GetDataAccess().KeyDelete(priKey);
#endif
                return;
            }

#if SERVER
            // 删除多键
            var val = await Redis.Db.StringGetAsync(priKey);
            if (!val.HasValue)
                return;

            var entity = RpcKit.ParseString<Row>(val);
            var ls = new List<string> { priKey };
            foreach (var item in _otherKeys)
            {
                var itemVal = entity.Str(item);
                if (itemVal != string.Empty)
                    ls.Add($"{_prefix}:{item}:{itemVal}");
            }

            // lua脚本：批量删除
            await Redis.BatchKeyDelete(ls);
#else
            var ca = _model.AccessInfo.GetDataAccess();
            var val = await ca.StringGet(priKey);
            if (string.IsNullOrEmpty(val))
                return;

            var entity = RpcKit.ParseString<Row>(val);
            var ls = new List<string> { priKey };
            foreach (var item in _otherKeys)
            {
                var itemVal = entity.Str(item);
                if (itemVal != string.Empty)
                    ls.Add($"{_prefix}:{item}:{itemVal}");
            }
            await ca.BatchKeyDelete(ls);
#endif
        }

        /// <summary>
        /// 表名作为缓存键前缀
        /// </summary>
        string _prefix => _model.Schema.Name.ToLower();

        /// <summary>
        /// 主键缓存项
        /// </summary>
        string _primaryKey => _model.Schema.PrimaryKey[0].Name.ToLower();
    }
}
