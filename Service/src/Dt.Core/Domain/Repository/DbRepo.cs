#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// mysql仓库类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DbRepo<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// 业务线上下文
        /// </summary>
        protected readonly LobContext _ = LobContext.Current;

        /// <summary>
        /// 插入实体对象
        /// </summary>
        /// <param name="p_entity">待插入的实体</param>
        /// <returns>true 成功</returns>
        public async Task<bool> Insert(TEntity p_entity)
        {
            Check.NotNull(p_entity);
            OnInserting(p_entity);
            bool suc = await _.Db.Insert(p_entity);
            if (suc)
            {
                OnInserted(p_entity);

                if (InsertEvent != DomainEvent.None)
                {
                    var e = new InsertEventData<TEntity>(p_entity);
                    if (InsertEvent == DomainEvent.Remote)
                        _.RemoteEB.Multicast(e);
                    else
                        _.LocalEB.Publish(e);
                }
            }
            return suc;
        }

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="p_entity">实体</param>
        /// <returns>true 成功</returns>
        public async Task<bool> Update(TEntity p_entity)
        {
            Check.NotNull(p_entity);
            OnUpdating(p_entity);
            bool suc = await _.Db.Update(p_entity);
            if (suc)
            {
                OnUpdated(p_entity);

                if (UpdateEvent != DomainEvent.None)
                {
                    var e = new UpdateEventData<TEntity>(p_entity);
                    if (UpdateEvent == DomainEvent.Remote)
                        _.RemoteEB.Multicast(e);
                    else
                        _.LocalEB.Publish(e);
                }
            }
            return suc;
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns>true 删除成功</returns>
        public async Task<bool> Delete(TEntity p_entity)
        {
            Check.NotNull(p_entity);
            OnDeleting(p_entity);
            bool suc = await _.Db.Delete(p_entity);
            if (suc)
            {
                OnDeleted(p_entity);
                if (DeleteEvent != DomainEvent.None)
                {
                    var e = new DeleteEventData<TEntity>(p_entity);
                    if (DeleteEvent == DomainEvent.Remote)
                        _.RemoteEB.Multicast(e);
                    else
                        _.LocalEB.Publish(e);
                }
            }
            return suc;
        }

        /// <summary>
        /// 插入实体后触发的领域事件种类
        /// </summary>
        public DomainEvent InsertEvent { get; set; }

        /// <summary>
        /// 更新实体后触发的领域事件种类
        /// </summary>
        public DomainEvent UpdateEvent { get; set; }

        /// <summary>
        /// 删除实体后触发的领域事件种类
        /// </summary>
        public DomainEvent DeleteEvent { get; set; }

        /// <summary>
        /// 插入前
        /// </summary>
        /// <param name="p_entity"></param>
        protected virtual void OnInserting(TEntity p_entity)
        { }

        /// <summary>
        /// 插入后
        /// </summary>
        /// <param name="p_entity"></param>
        protected virtual void OnInserted(TEntity p_entity)
        { }

        /// <summary>
        /// 更新前
        /// </summary>
        /// <param name="p_entity"></param>
        protected virtual void OnUpdating(TEntity p_entity)
        { }

        /// <summary>
        /// 更新后
        /// </summary>
        /// <param name="p_entity"></param>
        protected virtual void OnUpdated(TEntity p_entity)
        { }

        /// <summary>
        /// 删除前
        /// </summary>
        /// <param name="p_entity"></param>
        protected virtual void OnDeleting(TEntity p_entity)
        { }

        /// <summary>
        /// 删除后
        /// </summary>
        /// <param name="p_entity"></param>
        protected virtual void OnDeleted(TEntity p_entity)
        { }
    }

    /// <summary>
    /// 包含"ID"主键的mysql仓库
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class DbRepo<TEntity, TKey> : DbRepo<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 根据主键获得实体对象，不存在时返回null
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_includeChildren">是否包含所有的子实体</param>
        /// <returns>返回实体对象或null</returns>
        public async Task<TEntity> Get(TKey p_id, bool p_includeChildren = true)
        {
            TEntity entity = await _.Db.FirstByKey<TEntity, TKey>(p_id);
            if (entity != null && p_includeChildren)
                await LoadChildren(entity);
            return entity;
        }

        /// <summary>
        /// 加载所有子实体
        /// </summary>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        protected virtual Task LoadChildren(TEntity p_entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据主键删除实体对象
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>true 删除成功</returns>
        public async Task<bool> Delete(TKey p_id)
        {
            TEntity entity = await Get(p_id);
            if (entity != null)
                return await Delete(entity);
            return false;
        }
    }
}
