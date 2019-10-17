#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 默认删除事件的处理内容
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DeleteEventHandler<TEntity> : IRemoteHandler<InsertEventData<TEntity>>, ILocalHandler<InsertEventData<TEntity>>
        where TEntity : class, IEntity
    {
        public virtual Task Handle(InsertEventData<TEntity> p_event)
        {
            return Task.CompletedTask;
        }
    }
}
