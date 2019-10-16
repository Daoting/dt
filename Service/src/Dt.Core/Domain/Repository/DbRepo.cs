#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    public class DbRepo<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// 业务线上下文
        /// </summary>
        protected readonly LobContext _c = LobContext.Current;

        public Task<TEntity> Insert(TEntity p_entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(TEntity p_entity)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Update(TEntity p_entity)
        {
            throw new NotImplementedException();
        }
    }

    public class DbRepo<TEntity, TKey> : DbRepo<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        
        public Task<bool> Delete(TKey p_id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Get(TKey p_id, bool p_includeChildren = true)
        {
            throw new NotImplementedException();
        }

    }
}
