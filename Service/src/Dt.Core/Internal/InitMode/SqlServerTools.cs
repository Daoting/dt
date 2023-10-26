#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    class SqlServerTools : IDbTools
    {
        const string _sqlDrop = "IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN ('U')) DROP TABLE [dbo].[{0}]";
        SqlServerAccess _da;
        string _host;
        string _newDb;
        string _newUser;
        string _newPwd;
        string _defDb;

        public SqlServerTools(List<string> p_list)
        {
            _host = $"Data Source={p_list[1]},{p_list[2]}";
            var connStr = $"{_host};Initial Catalog={p_list[3]};User ID={p_list[4]};Password={p_list[5]};Encrypt=True;TrustServerCertificate=True;";
            _da = new SqlServerAccess(new DbInfo("sqlserver", connStr, DatabaseType.SqlServer));

            _newDb = p_list[6].Trim();
            _newUser = p_list[7].Trim();
            _newPwd = p_list[8].Trim();
            _defDb = p_list[3];
            Kit.TraceSql = false;
        }

        public static async Task<bool> TestConnect(List<string> p_list)
        {
            var da = new SqlServerAccess(new DbInfo(
                "sqlserver",
                $"Data Source={p_list[1]},{p_list[2]};Initial Catalog={p_list[3]};User ID={p_list[4]};Password={p_list[5]};Encrypt=True;TrustServerCertificate=True;",
                DatabaseType.SqlServer));
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

        public async Task<bool> ExistsDb()
        {
            return await _da.GetScalar<int>($"select count(*) from sys.sysdatabases where name='{_newDb}'") > 0;
        }

        public async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from sys.server_principals where type_desc='SQL_LOGIN' and name='{_newUser}'") > 0;
        }

        public async Task<bool> IsPwdCorrect()
        {
            var connStr = $"{_host};Initial Catalog={_newDb};User ID={_newUser};Password={_newPwd};Encrypt=True;TrustServerCertificate=True;";
            var da = new SqlServerAccess(new DbInfo("sqlserver", connStr, DatabaseType.SqlServer));
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

        public async Task<bool> InitDb(int p_initType)
        {
            _da.AutoClose = false;
            await DropExists();

            await CreateDb();
            await CreateUser();
            await _da.Close(true);
            Log.Information("创建空库成功");

            return await Import(p_initType, false);
        }

        public Task<bool> ImportToDb(int p_initType)
        {
            return Import(p_initType, true);
        }

        async Task<bool> Import(int p_initType, bool p_dropExists)
        {
            var connStr = $"{_host};Initial Catalog={_newDb};User ID={_newUser};Password={_newPwd};Encrypt=True;TrustServerCertificate=True;";
            var da = new SqlServerAccess(new DbInfo("sqlserver", connStr, DatabaseType.SqlServer));
            da.AutoClose = false;

            if (p_dropExists)
            {
                await DropTbl("drop-init.txt", da);
                if (p_initType == 1)
                    await DropTbl("drop-demo.txt", da);
                Log.Information($"删除旧表");
            }

            await ImportSql("sqlserver-init.sql", da);
            if (p_initType == 1)
                await ImportSql("sqlserver-demo.sql", da);

            int cntTbl = await da.GetScalar<int>("select count(*) from sysobjects where xtype='U'");
            int cntView = await da.GetScalar<int>("select count(*) from sysobjects where xtype='V'");
            int cntSp = await da.GetScalar<int>("select count(*) from sysobjects where xtype='P'");
            int cntSeq = await da.GetScalar<int>("select count(*) from sys.sequences");

            var tp = p_initType == 0 ? "标准库" : "样例库";
            Log.Information($"{tp}初始化成功：\r\n{cntTbl}个表\r\n{cntSeq}个序列\r\n{cntSp}个存储过程\r\n{cntView}个视图\r\n");

            await da.Close(true);

            Log.Information("连接串：\r\n" + connStr);
            return true;
        }

        async Task ImportSql(string p_file, SqlServerAccess p_da)
        {
            string sql;
            using (var sr = MySqlTools.GetSqlStream(p_file))
            {
                sql = sr.ReadToEnd();
            }

            var ls = sql.Split("GO");
            foreach (var item in ls)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    try
                    {
                        await p_da.Exec(item);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex.Message);
                    }
                }
            }
        }

        async Task DropTbl(string p_file, SqlServerAccess p_da)
        {
            using (var sr = MySqlTools.GetSqlStream(p_file))
            {
                while (true)
                {
                    var tbl = sr.ReadLine();
                    if (tbl == null)
                        break;

                    if (!string.IsNullOrEmpty(tbl))
                    {
                        try
                        {
                            await p_da.Exec(string.Format(_sqlDrop, tbl));
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex.Message);
                        }
                    }
                }
            }
        }

        async Task CreateDb()
        {
            Log.Information($"创建数据库【{_newDb}】...");

            // 数据文件和默认库放在同一路径
            var path = await _da.GetScalar<string>($"select filename from sys.sysdatabases where name='{_defDb}'");
            path = path.Substring(0, path.LastIndexOf('\\'));

            await _da.Exec(string.Format(
@"CREATE DATABASE {0} ON
(NAME = {0}_dat,
    FILENAME = '{1}\{0}dat.mdf',
    SIZE = 10,
    MAXSIZE = 50,
    FILEGROWTH = 5)
LOG ON
(NAME = {0}_log,
    FILENAME = '{1}\{0}log.ldf',
    SIZE = 5 MB,
    MAXSIZE = 25 MB,
    FILEGROWTH = 5 MB)", _newDb, path));

            Log.Information("创建成功！");
        }

        async Task CreateUser()
        {
            Log.Information($"创建用户【{_newUser}】...");
            await _da.Exec($"create login {_newUser} with password='{_newPwd}',default_database={_newDb}");
            await _da.Exec("use " + _newDb);
            await _da.Exec(string.Format("create user {0} for login {0} with default_schema=dbo", _newUser));
            Log.Information($"创建成功！密码：{_newPwd}");

            Log.Information($"数据库【{_newDb}】的所有权限授予给用户【{_newUser}】...");
            await _da.Exec("sp_addrolemember", new { rolename = "db_owner", membername = _newUser });
            Log.Information("授权成功！");
        }

        async Task DropExists()
        {
            if (await ExistsDb())
            {
                Log.Warning($"数据库【{_newDb}】已存在，正在删除...");

                // 强制关闭连接
                var ls = await _da.FirstCol<string>(string.Format(@"
SELECT * FROM 
[Master].[dbo].[SYSPROCESSES] WHERE [DBID] 
IN 
(
  SELECT 
   [DBID]
  FROM 
   [Master].[dbo].[SYSDATABASES] 
  WHERE 
   NAME='{0}'  
)", _newDb));
                foreach (var id in ls)
                {
                    await _da.Exec("kill " + id);
                }

                // 删库后用户自动删除
                await _da.Exec($"drop database {_newDb}");
                if (!await ExistsDb())
                {
                    Log.Warning($"【{_newDb}】删除成功！");
                }
                else
                {
                    throw new Exception("数据库删除失败！");
                }
            }

            if (await ExistsUser())
            {
                Log.Warning($"登录名【{_newUser}】已存在，正在删除...");
                await _da.Exec($"drop login {_newUser}");
                Log.Warning($"【{_newUser}】删除成功！");
            }
        }
    }
}
