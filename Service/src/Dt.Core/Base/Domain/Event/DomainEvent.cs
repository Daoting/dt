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

namespace Dt.Core
{
    /// <summary>
    /// 领域事件
    /// </summary>
    internal class DomainEvent
    {
        public DomainEvent(bool p_isRemote, IEvent p_event)
        {
            IsRemoteEvent = p_isRemote;
            Event = p_event;
        }

        /// <summary>
        /// 是否为远程事件
        /// </summary>
        public bool IsRemoteEvent { get; }

        /// <summary>
        /// 事件内容
        /// </summary>
        public IEvent Event { get; }
    }
}
