#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    partial class RbacDs
    {
        const string Sql分组列表的用户 = @"select distinct(user_id) from cm_user_group where group_id in ({0})";

        const string Sql角色列表的用户 = @"select distinct(user_id) from cm_user_role where role_id in ({0})";
    }
}