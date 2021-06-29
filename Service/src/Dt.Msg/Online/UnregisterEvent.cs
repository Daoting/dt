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
            var ci = Online.GetSession(p_event.UserID, p_event.SessionID);
            if (ci != null)
            {
                await Redis.Db.StringSetAsync(p_event.CacheKey, "true");
                await ci.Close();
            }
        }
    }
}
