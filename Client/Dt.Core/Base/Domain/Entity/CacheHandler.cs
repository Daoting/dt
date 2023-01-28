#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-26 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

using Dt.Core.Rpc;

namespace Dt.Core
{
    /// <summary>
    /// 针对Entity的缓存管理
    /// </summary>
    class CacheHandler
    {
        
        public CacheHandler(EntitySchema p_model, CacheAttribute p_cfg)
        {
            
        }

        public Task Cache<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.IfNull(p_entity);
            string val = RpcKit.GetObjectString(p_entity);
            string id = p_entity.Str(_primaryKey);

            return Task.CompletedTask;
        }

        public async Task<TEntity> Get<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            return null;
        }

        public Task Remove<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            return Task.CompletedTask;
        }

        public async Task RemoveByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            
        }
    }
}
