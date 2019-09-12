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
    public class OnlinePushEvent : ExcludeEvent
    {
        public string PrefixKey { get; set; }

        public List<long> Receivers { get; set; }

        public string Msg { get; set; }
    }

    public class OnlinePushHandler : IRemoteHandler<OnlinePushEvent>
    {
        public async Task Handle(OnlinePushEvent p_event)
        {
            if (p_event.Receivers != null && p_event.Receivers.Count > 0)
            {
                StringCache cache = new StringCache(p_event.PrefixKey);
                foreach (var id in p_event.Receivers)
                {
                    if (Online.All.TryGetValue(id, out var ci))
                    {
                        ci.AddMsg(p_event.Msg);
                        await cache.Set(id.ToString(), true);
                    }
                }
            }
        }
    }
}
