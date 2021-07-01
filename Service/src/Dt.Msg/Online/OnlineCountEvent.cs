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
    /// 统计各副本会话总数的事件
    /// </summary>
    public class OnlineCountEvent : IEvent
    {
        public string CacheKey { get; set; }
    }

    public class OnlineCountHandler : IRemoteHandler<OnlineCountEvent>
    {
        public async Task Handle(OnlineCountEvent p_event)
        {
            await Kit.HashSetField(p_event.CacheKey, null, Kit.SvcID, Online.TotalCount);
            await Kit.StringIncrement(p_event.CacheKey, "cnt");
        }
    }
}
