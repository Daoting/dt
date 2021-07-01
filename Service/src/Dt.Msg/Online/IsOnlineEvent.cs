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
    /// 获取用户是否在线的事件
    /// </summary>
    public class IsOnlineEvent : ExcludeEvent
    {
        public string CacheKey { get; set; }

        public long UserID { get; set; }
    }

    public class IsOnlineHandler : IRemoteHandler<IsOnlineEvent>
    {
        public async Task Handle(IsOnlineEvent p_event)
        {
            var sc = new StringCache(p_event.CacheKey);
            var ls = Online.GetSessions(p_event.UserID);
            if (ls != null && ls.Count > 0)
            {
                await sc.Set(null, "true");
            }
            // 统计总数+1
            await sc.Increment("cnt");
        }
    }
}
