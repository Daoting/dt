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
    public partial class FileDs : DomainSvc<FileDs, AtCm.Info>
    {
        public static async Task<Table> SearchFiles(string p_name, string p_ext)
        {
            var dbType = await _da.GetDbType();

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
                return await _da.Query(sql);
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
            return await _da.Query(sql);
        }
    }
}