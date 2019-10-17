#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 更新事件
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class UpdateEventData<TEntity> : IEvent
        where TEntity : class, IEntity
    {
        public UpdateEventData(TEntity p_entity)
        {
            Entity = p_entity;
        }

        /// <summary>
        /// 实体对象
        /// </summary>
        public TEntity Entity { get; }
    }
}
