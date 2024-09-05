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
    public partial class PermissionX
    {
        public static async Task<PermissionX> New(
            long FuncID = default,
            string Name = default,
            string Note = default)
        {
            return new PermissionX(
                ID: await NewID(),
                FuncID: FuncID,
                Name: "",
                Note: Note);
        }

        public static Task<Table<PermissionX>> GetUserPermission(long p_userID)
        {
            return Query(string.Format(Sql用户具有的权限, p_userID));
        }

        public static Task<Table<PermissionX>> GetRolePermission(long p_roleID)
        {
            return Query(string.Format(Sql角色权限, p_roleID));
        }

        public static Task<Table<PermissionX>> GetUserPersAndModule(long p_userID)
        {
            return Query(string.Format(Sql用户具有的权限及所属, p_userID));
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

                if (IsAdded || Cells["Name"].IsChanged)
                {
                    if (Decimal.TryParse(Name, out _))
                        Throw.Msg("权限名称不可全部为数字！", cName);
                    if (await GetCount($"where name='{Name}' and func_id={FuncID}") > 0)
                        Throw.Msg("相同功能内的权限名称不可重复！", cName);
                }
            });

            OnDeleting(async () =>
            {
                Throw.If(ID < 1000, "系统权限无法删除！");

                // 清除关联用户的数据版本号，没放在 OnDeleted 处理因为cm_role_per有级联删除
                var ls = await At.FirstCol<long>($"select id from cm_role a where exists (select role_id from cm_role_per b where a.id=b.role_id and per_id={ID})");
                RbacDs.DelRoleDataVer(ls, RbacDs.PrefixPer);
            });
        }

        #region Sql
        const string Sql用户具有的权限 = @"
SELECT
    id,
    name
FROM
    (
        SELECT
            DISTINCT (b.id),
            b.name
        FROM
            cm_role_per a
            LEFT JOIN cm_permission b ON a.per_id = b.id
        WHERE
            EXISTS (
                SELECT
                    role_id
                FROM
                    cm_user_role c
                WHERE
                    a.role_id = c.role_id
                    AND user_id = {0}
                UNION
                SELECT
                    role_id
                FROM
                    cm_group_role d
                WHERE
                    a.role_id = d.role_id
                    AND EXISTS (
                        SELECT
                            group_id
                        FROM
                            cm_user_group e
                        WHERE
                            d.group_id = e.group_id
                            AND e.user_id = {0}
                    )
            )
            OR a.role_id = 1
    ) t
ORDER BY
    id
";

        const string Sql角色权限 = @"
SELECT
    a.*,
    b.name funcname,
    c.name modname
FROM
    cm_permission a,
    cm_permission_func b,
    cm_permission_module c
WHERE
    a.func_id = b.id
    AND b.module_id = c.id
    AND EXISTS (
        SELECT
            per_id
        FROM
            cm_role_per d
        WHERE
            a.id = d.per_id
            AND role_id = {0}
    )
";

        const string Sql用户具有的权限及所属 = @"
SELECT
    per.*,
    func.name funcname,
    MODULE.name modname
FROM
    (
        SELECT
            id,
            name,
            func_id
        FROM
            (
                SELECT
                    DISTINCT (b.id),
                    b.name,
                    b.func_id
                FROM
                    cm_role_per a
                    LEFT JOIN cm_permission b ON a.per_id = b.id
                WHERE
                    EXISTS (
                        SELECT
                            role_id
                        FROM
                            cm_user_role c
                        WHERE
                            a.role_id = c.role_id
                            AND user_id = {0}
                        UNION
                        SELECT
                            role_id
                        FROM
                            cm_group_role d
                        WHERE
                            a.role_id = d.role_id
                            AND EXISTS (
                                SELECT
                                    group_id
                                FROM
                                    cm_user_group e
                                WHERE
                                    d.group_id = e.group_id
                                    AND e.user_id = {0}
                            )
                    )
                    OR a.role_id = 1
            ) t
        ORDER BY
            id
    ) per,
    cm_permission_func func,
    cm_permission_module MODULE
WHERE
    per.func_id = func.id
    AND func.module_id = MODULE.id
";
        #endregion
    }
}