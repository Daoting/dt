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
    public partial class FileDs : DomainSvc<FileDs>
    {
        public static async Task<Table> SearchFiles(string p_name, string p_ext)
        {
            var dbType = await At.GetDbType();

            string sql;
            if (string.IsNullOrEmpty(p_ext))
            {
                switch (dbType)
                {
                    case DatabaseType.Oracle:
                        sql = string.Format(Sql搜索所有文件, $"%{p_name}%", Kit.UserID, "", "where rownum<50");
                        break;

                    case DatabaseType.SqlServer:
                        sql = string.Format(Sql搜索所有文件, $"%{p_name}%", Kit.UserID, "top 50", "");
                        break;

                    default:
                        sql = string.Format(Sql搜索所有文件, $"%{p_name}%", Kit.UserID, "", "limit 50");
                        break;
                }
                return await At.Query(sql);
            }

            switch (dbType)
            {
                case DatabaseType.Oracle:
                    sql = string.Format(Sql搜索扩展名文件, $"%{p_name}%", p_ext, Kit.UserID, "instr", "", "where rownum<50");
                    break;

                case DatabaseType.SqlServer:
                    sql = string.Format(Sql搜索扩展名文件, $"%{p_name}%", p_ext, Kit.UserID, "charindex", "top 50", "");
                    break;

                case DatabaseType.PostgreSql:
                    sql = string.Format(Sql搜索扩展名文件, $"%{p_name}%", p_ext, Kit.UserID, "strpos", "", "limit 50");
                    break;

                default:
                    sql = string.Format(Sql搜索扩展名文件, $"%{p_name}%", p_ext, Kit.UserID, "locate", "", "limit 50");
                    break;
            }
            return await At.Query(sql);
        }

        #region Sql
        const string Sql搜索所有文件 = @"
select {2} * from
(
select info from cm_file_pub
where
	is_folder = '0' 
	and name like '{0}'
union
select info from cm_file_my
where
	is_folder = '0' 
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
	is_folder = '0' 
	and {3}(ext_name, '{1}') > 0
	and name like '{0}'
union
select info from cm_file_my
where
	is_folder = '0' 
	and {3}(ext_name, '{1}') > 0
	and user_id = {2} 
	and name like '{0}'
) a
{5}
";


        #endregion
    }
}