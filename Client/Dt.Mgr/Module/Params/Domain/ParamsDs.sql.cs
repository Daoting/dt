#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    partial class ParamsDs
    {
        const string Sql用户参数值byid = @"
select value from cm_user_params
where
	user_id = {0}
	and param_id = {1}
union
select value from cm_params
where
	id = {1}
";

        const string Sql用户参数值byname = @"
select a.value
from
	cm_user_params a,
	cm_params b 
where
	a.param_id = b.id 
	and a.user_id = {0} 
	and b.name = '{1}'
union
select value from cm_params
where
	name = '{1}'
";

        const string Sql用户参数列表 = @"
select param_id,value from cm_user_params
where user_id={0}
union
select id,value from cm_params a
where
    not exists (
    select param_id from cm_user_params b
    where
	    a.id = b.param_id 
	    and user_id = {0} )
";
    }
}