#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Npgsql;
#endregion

namespace Dt.Base.Tools
{
    class DbKit
    {
        public static async Task<bool> Test(DbInitInfo p_info)
        {
            IDataAccess da = null;
            if (p_info.DbType == DatabaseType.PostgreSql)
                da = NewPostgreSql(p_info);
            else if (p_info.DbType == DatabaseType.MySql)
                da = NewMySql(p_info);
            else if (p_info.DbType == DatabaseType.Oracle)
                da = NewOracle(p_info);
            else if (p_info.DbType == DatabaseType.SqlServer)
                da = NewSqlServer(p_info);
            else
                Throw.Msg("不支持该数据库类型！");
            
            try
            {
                await da.SyncDbTime();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static Stream GetFileStream(string p_sqlFile)
        {
            return typeof(DbKit).Assembly.GetManifestResourceStream("Dt.Base.SysTools.DbInit.Sql." + p_sqlFile);
        }
        
        static IDataAccess NewPostgreSql(DbInitInfo p_info)
        {
            return new PostgreSqlAccess(new DbAccessInfo(
                "pg",
                $"Host={p_info.Host};Port={p_info.Port};Database={p_info.DefDb};Username={p_info.DefUser};Password={p_info.Pwd};",
                DatabaseType.PostgreSql));
        }

        static IDataAccess NewMySql(DbInitInfo p_info)
        {
            return new MySqlAccess(new DbAccessInfo(
                "mysql",
                $"Server={p_info.Host};Port={p_info.Port};Uid={p_info.DefUser};Pwd={p_info.Pwd};",
                DatabaseType.MySql));
        }

        static IDataAccess NewOracle(DbInitInfo p_info)
        {
            return new OracleAccess(new DbAccessInfo(
                "orcl",
                $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={p_info.Host})(PORT={p_info.Port}))(CONNECT_DATA=(SERVICE_NAME={p_info.DefDb})(SERVER=dedicated)));User Id={p_info.DefUser};Password={p_info.Pwd};",
                DatabaseType.Oracle));
        }

        static IDataAccess NewSqlServer(DbInitInfo p_info)
        {
            return new SqlServerAccess(new DbAccessInfo(
                "sqlserver",
                $"Data Source={p_info.Host},{p_info.Port};Initial Catalog={p_info.DefDb};User ID={p_info.DefUser};Password={p_info.Pwd};Encrypt=True;TrustServerCertificate=True;",
                DatabaseType.SqlServer));
        }
    }
}
