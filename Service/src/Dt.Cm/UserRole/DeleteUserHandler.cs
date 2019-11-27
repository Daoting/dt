#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    public class DeleteUserHandler : DeleteEventHandler<User>
    {
        public override Task Handle(DeleteEvent<User> p_event)
        {
            // 删除用户时同步删除userrole缓存
            return new UserRoleCache().Remove(p_event.Entity.ID);
        }
    }
}
