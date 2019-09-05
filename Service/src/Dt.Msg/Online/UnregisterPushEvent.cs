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
    /// 注销推送事件
    /// </summary>
    public class UnregisterPushEvent : IEvent
    {
        public long UserID { get; set; }
    }

    public class UnregisterPushHandler : IRemoteHandler<UnregisterPushEvent>
    {
        public Task Handle(UnregisterPushEvent p_event)
        {
            // 会话位置已删除
            if (Online.All.TryRemove(p_event.UserID, out var ci))
                ci.Close();
            return Task.CompletedTask;
        }
    }
}
