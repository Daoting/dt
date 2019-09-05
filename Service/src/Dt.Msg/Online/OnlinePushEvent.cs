#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 在线推送事件
    /// </summary>
    public class OnlinePushEvent : IEvent
    {
        public long UserID { get; set; }

        public string Msg { get; set; }
    }

    public class OnlinePushHandler : IRemoteHandler<OnlinePushEvent>
    {
        public Task Handle(OnlinePushEvent p_event)
        {
            if (Online.All.TryGetValue(p_event.UserID, out var ci))
                ci.AddMsg(p_event.Msg);
            return Task.CompletedTask;
        }
    }
}
