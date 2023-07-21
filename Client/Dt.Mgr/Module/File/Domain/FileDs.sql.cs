#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-11 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    partial class FileDs
    {
        const string Sql搜索所有文件 = @"
select {2} * from
(
select info from cm_file_pub
where
	is_folder = 0 
	and name like '{0}'
union
select info from cm_file_my
where
	is_folder = 0 
	and user_id = {1} 
	and name like '{0}'
) a
{3}
";

        const string Sql搜索扩展名文件 = @"
select {4} * from
(
select info from cm_file_pub
where
	is_folder = 0 
	and {3}(ext_name, '{1}') > 0
	and name like '{0}'
union
select info from cm_file_my
where
	is_folder = 0 
	and {3}(ext_name, '{1}') > 0
	and user_id = {2} 
	and name like '{0}'
) a
{5}
";


    }
}