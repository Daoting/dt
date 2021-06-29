#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 本地事件处理接口
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface ILocalHandler<TEvent> : IEventHandler
        where TEvent : IEvent
    {
        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <returns></returns>
        Task Handle(TEvent p_event);
    }
}