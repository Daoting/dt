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

namespace Dt.Cm
{
    public class InsertUserHandler : InsertEventHandler<UserObj>
    {
        public override Task Handle(InsertEvent<UserObj> p_event)
        {
            return Task.CompletedTask;
        }
    }

    public class UpdateUserHandler : UpdateEventHandler<UserObj>
    {
        public override Task Handle(UpdateEvent<UserObj> p_event)
        {
            return Task.CompletedTask;
        }
    }

    public class DeleteUserHandler : DeleteEventHandler<UserObj>
    {
        public override Task Handle(DeleteEvent<UserObj> p_event)
        {
            // 删除用户时同步删除缓存
            return new HashCache("ver").Delete(p_event.Entity.ID);
        }
    }

    public class UserPhoneChangedHandler : ILocalHandler<UserPhoneChangedEvent>
    {
        public Task Handle(UserPhoneChangedEvent p_event)
        {
            // 换手机号事件
            return Task.CompletedTask;
        }
    }
}
