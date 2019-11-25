#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 插入后事件处理的默认基类
    /// </summary>
    public abstract class InsertEventHandler<TEntity> : IRemoteHandler<InsertEvent<TEntity>>, ILocalHandler<InsertEvent<TEntity>>
        where TEntity : Entity
    {
        public virtual Task Handle(InsertEvent<TEntity> p_event)
        {
            return Task.CompletedTask;
        }
    }
}
