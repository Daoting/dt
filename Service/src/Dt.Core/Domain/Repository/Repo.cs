#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 主键ID为long类型的mysql仓库
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repo<TEntity> : DbRepo<TEntity, long>
        where TEntity : Root<long>
    {
        protected override void AddInsertEvent(TEntity p_entity, CudEvent p_cudEvent)
        {
            if (p_cudEvent != CudEvent.None)
            {
                var ev = new InsertEvent<TEntity> { ID = p_entity.ID };
                _.AddDomainEvent(new DomainEvent(p_cudEvent == CudEvent.Remote, ev));
            }
        }

        protected override void AddUpdateEvent(TEntity p_entity, CudEvent p_cudEvent)
        {
            if (p_cudEvent != CudEvent.None)
            {
                var ev = new UpdateEvent<TEntity> { ID = p_entity.ID };
                _.AddDomainEvent(new DomainEvent(p_cudEvent == CudEvent.Remote, ev));
            }
        }

        protected override void AddDeleteEvent(long p_id, CudEvent p_cudEvent)
        {
            if (p_cudEvent != CudEvent.None)
            {
                var ev = new DeleteEvent<TEntity> { ID = p_id };
                _.AddDomainEvent(new DomainEvent(p_cudEvent == CudEvent.Remote, ev));
            }
        }
    }
}
