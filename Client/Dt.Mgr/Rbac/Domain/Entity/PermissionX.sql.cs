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

        const string Sql角色权限 = @"
select
	a.*,
	b.name funcname,
	c.name modname 
from
	cm_permission a,
	cm_permission_func b,
	cm_permission_module c 
where
	a.func_id = b.id 
	and b.module_id = c.id 
	and exists (
	select
		per_id 
	from
		cm_role_per d 
	where
		a.id = d.per_id 
	and role_id = {0} 
	)
";

        const string Sql用户具有的权限及所属 = @"
select
	per.*,
	func.name funcname,
	module.name modname
from
(
select id, name, func_id
from
	(
    select distinct (b.id),b.name,b.func_id
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
) per,
cm_permission_func func,
cm_permission_module module 
where
	per.func_id = func.id 
	and func.module_id = module.id
";
    }
}