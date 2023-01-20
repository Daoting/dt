#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.EventBus;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm.Domain
{
    public class UserPhoneChangedHandler : IEventHandler<UserPhoneChangedEvent>
    {
        public Task Handle(UserPhoneChangedEvent p_event)
        {
            // 换手机号事件
            return Task.CompletedTask;
        }
    }
}
