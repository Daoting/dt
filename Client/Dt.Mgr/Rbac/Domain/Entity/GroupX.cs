#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class GroupX
    {
        public static async Task<GroupX> New(
            string Name = default,
            string Note = default)
        {
            return new GroupX(
                ID: await NewID(),
                Name: "新分组",
                Note: Note);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                Throw.IfEmpty(Name, "分组名称不可为空！");
                return Task.CompletedTask;
            });

            OnDeleting(async () =>
            {
                Throw.If(ID < 1000, "系统分组无法删除！");

                // 清除关联用户的数据版本号，没放在 OnDeleted 处理因为cm_user_group有级联删除
                var users = await AtCm.FirstCol<long>("cm_分组_关联用户", new { p_groupid = ID });
                RbacDs.DelUserDataVer(users);
            });
        }
    }
}