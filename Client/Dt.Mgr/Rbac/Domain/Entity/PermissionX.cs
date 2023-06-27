﻿#region 文件描述
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
    public partial class PermissionX
    {
        public static async Task<PermissionX> New(
            string Name = default,
            string Note = default)
        {
            return new PermissionX(
                ID: await NewID(),
                Name: "新权限",
                Note: Note);
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(Name, "权限名称不可为空！");

                if (!IsAdded && Cells["Name"].IsChanged)
                {
                    if (!await Kit.Confirm("权限名称会被硬编码在程序中，确认要修改吗？"))
                        Throw.Msg("已取消保存");
                }

                if ((IsAdded || Cells["Name"].IsChanged)
                    && await GetCount($"where name='{Name}'") > 0)
                {
                    Throw.Msg("权限名称重复！");
                }
            });

            OnDeleting(async () =>
            {
                Throw.If(ID < 1000, "系统权限无法删除！");

                // 清除关联用户的数据版本号，没放在 OnDeleted 处理因为cm_role_per有级联删除
                var ls = await AtCm.FirstCol<long>($"select id from cm_role a where exists (select roleid from cm_role_per b where a.id=b.roleid and perid={ID})");
                RbacDs.DelRoleDataVer(ls, RbacDs.PrefixPer);
            });
        }
    }
}