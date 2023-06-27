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
    public partial class RoleX
    {
        public static async Task<RoleX> New()
        {
            long id = await NewID();
            return new RoleX(id, "新角色");
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(Name, "角色名称不可为空！");

                if ((IsAdded || Cells["name"].IsChanged)
                    && await GetCount($"where name='{Name}'") > 0)
                {
                    Throw.Msg("角色名称重复！");
                }
            });

            OnDeleting(async () =>
            {
                Throw.If(ID < 1000, "系统角色无法删除！");

                // 清除关联用户的数据版本号，没放在 OnDeleted 处理因为cm_user_role有级联删除
                var ls = await AtCm.FirstCol<long>($"select id from cm_user a where exists (select userid from cm_user_role b where a.id=b.userid and roleid={ID})");
                RbacDs.DelUserDataVer(ls);
            });
        }
    }
}