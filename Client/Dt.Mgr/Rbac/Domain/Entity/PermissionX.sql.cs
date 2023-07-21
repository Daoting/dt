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
select id, name
from
	(
    select distinct (b.id),b.name
	from
		cm_role_per a
		left join cm_permission b on a.per_id = b.id 
	where
		exists (
            select role_id from cm_user_role c
		    where
			    a.role_id = c.role_id 
			    and user_id = {0}
		    union
            select role_id from cm_group_role d
		    where
			    a.role_id = d.role_id 
			    and exists ( select group_id from cm_user_group e where d.group_id = e.group_id and e.user_id = {0} ) 
		    ) 
		    or a.role_id = 1 
	    ) t 
order by
	id
";
    }
}