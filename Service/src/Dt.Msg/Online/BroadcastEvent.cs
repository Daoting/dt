#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.EventBus;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 向所有会话广播消息的事件
    /// </summary>
    public class BroadcastEvent : IEvent
    {
        public string Msg { get; set; }
    }

    public class BroadcastHandler : IRemoteHandler<BroadcastEvent>
    {
        public Task Handle(BroadcastEvent p_event)
        {
            foreach (var ls in Online.All.Values)
            {
                foreach (var ci in ls)
                {
                    ci.AddMsg(p_event.Msg);
                }
            }
            return Task.CompletedTask;
        }
    }
}
