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
using StackExchange.Redis;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 获取用户会话信息的事件
    /// </summary>
    public class ClientInfoEvent : ExcludeEvent
    {
        public string CacheKey { get; set; }

        public long UserID { get; set; }
    }

    public class ClientInfoHandler : IRemoteHandler<ClientInfoEvent>
    {
        public Task Handle(ClientInfoEvent p_event)
        {
            ClientInfo ci = Online.GetClient(p_event.UserID);
            if (ci != null)
            {
                var db = Redis.Db;
                HashEntry[] hash = new HashEntry[4];
                hash[0] = new HashEntry("userid", ci.UserID);
                hash[1] = new HashEntry("svcid", Glb.ID);
                hash[2] = new HashEntry("starttime", ci.StartTime.ToString());
                hash[3] = new HashEntry("system", (int)ci.System);
                return db.HashSetAsync(p_event.CacheKey, hash);
            }
            return Task.CompletedTask;
        }
    }
}
