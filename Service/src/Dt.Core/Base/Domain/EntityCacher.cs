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
        #endregion

        public EntityCacher(EntitySchema p_model)
        {
            _model = p_model;
        }

        public async Task Cache<TEntity>(TEntity p_entity, string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            Throw.IfNull(p_entity);
            Throw.If(string.IsNullOrEmpty(p_keyName) || string.IsNullOrEmpty(p_keyVal), "实体缓存时键值不可为空！");

            string val = RpcKit.GetObjectString(p_entity);
            var idKey = $"{_prefix}:{p_keyName.ToLower()}:{p_keyVal}";

#if SERVER
            await Redis.Db.StringSetAsync(idKey, val);
#else
            await _model.AccessInfo.GetDa().StringSet(idKey, val);
#endif
        }

        public async Task<TEntity> Get<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            if (string.IsNullOrEmpty(p_keyName) || string.IsNullOrEmpty(p_keyVal))
                return default;

            var key = $"{_prefix}:{p_keyName.ToLower()}:{p_keyVal}";
#if SERVER
            var val = await Redis.Db.StringGetAsync(key);
            if (val.HasValue)
                return RpcKit.ParseString<TEntity>(val);
#else
            var val = await _model.AccessInfo.GetDa().StringGet(key);
            if (!string.IsNullOrEmpty(val))
                return RpcKit.ParseString<TEntity>(val);
#endif
            return default;
        }

        public async Task Remove(string p_keyName, string p_keyVal)
        {
            if (string.IsNullOrEmpty(p_keyName) || string.IsNullOrEmpty(p_keyVal))
                return;

            var key = $"{_prefix}:{p_keyName.ToLower()}:{p_keyVal}";
#if SERVER
            await Redis.Db.KeyDeleteAsync(key);
#else
            await _model.AccessInfo.GetDa().KeyDelete(key);
#endif
        }

        /// <summary>
        /// 表名作为缓存键前缀
        /// </summary>
        string _prefix => _model.Schema.Name.ToLower();
    }
}
