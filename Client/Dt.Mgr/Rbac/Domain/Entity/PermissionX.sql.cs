#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class PermissionX
    {
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
    }
}