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
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 在线推送事件
    /// </summary>
    public class OnlinePushEvent : IEvent
    {
        public string PrefixKey { get; set; }

        public List<long> Receivers { get; set; }

        public string Msg { get; set; }

        /// <summary>
        /// 同一账号只发送给第一个session
        /// </summary>
        public bool PushFirstSession { get; set; }
    }

    public class OnlinePushHandler : IRemoteHandler<OnlinePushEvent>
    {
        public async Task Handle(OnlinePushEvent p_event)
        {
            StringCache sc = new StringCache(p_event.PrefixKey);
            if (p_event.Receivers != null && p_event.Receivers.Count > 0)
            {
                foreach (var id in p_event.Receivers)
                {
                    if (Online.All.TryGetValue(id, out var ls)
                        && ls != null
                        && ls.Count > 0)
                    {
                        // 在线推送，直接放入推送队列
                        if (p_event.PushFirstSession)
                        {
                            ls[0].AddMsg(p_event.Msg);
                            await sc.Set(null, true);
                        }
                        else
                        {
                            foreach (var ci in ls)
                            {
                                ci.AddMsg(p_event.Msg);
                            }

                            // 设置处理标志： msg:Push:6位id前缀:userid = true
                            await sc.Set(id, true);
                        }
                    }
                }
            }
            // 统计总数+1
            await sc.Increment("cnt");
        }
    }
}
