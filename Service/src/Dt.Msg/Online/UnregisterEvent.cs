#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Caches;
using Dt.Core.EventBus;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 注销客户端的事件
    /// </summary>
    public class UnregisterEvent : ExcludeEvent
    {
        public string CacheKey { get; set; }

        public long UserID { get; set; }

        public string SessionID { get; set; }
    }

    public class UnregisterHandler : IRemoteHandler<UnregisterEvent>
    {
        public async Task Handle(UnregisterEvent p_event)
        {
            var sc = new StringCache(p_event.CacheKey);
            var ci = Online.GetSession(p_event.UserID, p_event.SessionID);
            if (ci != null)
            {
                await sc.Set(null, "true");
                await ci.Close();
            }
            // 统计总数
            await sc.Increment("cnt");
        }
    }
}
