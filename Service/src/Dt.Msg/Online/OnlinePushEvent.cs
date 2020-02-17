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
            if (p_event.Receivers == null || p_event.Receivers.Count == 0)
                return;

            StringCache cache = new StringCache(p_event.PrefixKey);
            foreach (var id in p_event.Receivers)
            {
                var ci = Online.GetClient(id);
                // 在线推送成功
                if (ci != null && ci.AddMsg(p_event.Msg))
                {
                    // 设置处理标志： 6位id前缀:userid = true
                    await cache.Set(id.ToString(), true);
                }
            }
        }
    }
}
