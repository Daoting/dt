#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

using Windows.Storage;

namespace Dt.Base.Tools
{
    class SqlServerTools : IDbTools
    {
        DbInitInfo _info;
        const string _sqlDrop = "IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN ('U')) DROP TABLE [dbo].[{0}]";
        SqlServerAccess _da;
        string _host;

        public SqlServerTools(DbInitInfo p_info)
        {
            _info = p_info;
            _host = $"Data Source={_info.Host},{_info.Port}";
            var connStr = $"{_host};Initial Catalog={_info.DefDb};User ID={_info.DefUser};Password={_info.Pwd};Encrypt=True;TrustServerCertificate=True;";
            _da = new SqlServerAccess(new DbAccessInfo("sqlserver", connStr, DatabaseType.SqlServer));
        }

        public async Task<bool> ExistsDb()
        {
            return await _da.GetScalar<int>($"select count(*) from sys.sysdatabases where name='{_info.NewDb}'") > 0;
        }

        public async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from sys.server_principals where type_desc='SQL_LOGIN' and name='{_info.NewUser}'") > 0;
        }

        public async Task<bool> IsPwdCorrect()
        {
            var connStr = $"{_host};Initial Catalog={_info.NewDb};User ID={_info.NewUser};Password={_info.NewPwd};Encrypt=True;TrustServerCertificate=True;";
            var da = new SqlServerAccess(new DbAccessInfo("sqlserver", connStr, DatabaseType.SqlServer));
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

        public Task CreateDb()
        {
            return Task.Run(async () =>
            {
                _da.AutoClose = false;
                await DropExists();

                await DoCreateDb();
                await CreateUser();
                await _da.Close(true);
                _info.Log("创建空库成功");
            });
        }

        public Task DeleteDb()
        {
            return Task.Run(async () =>
            {
                _da.AutoClose = false;
                await DropExists();
                await _da.Close(true);
            });
        }

        public Task ImportInit()
        {
            return Task.Run(async () =>
            {
                await Import(DbKit.GetFileStream("sqlserver-init.sql"));
            });
        }

        public Task ImportFromFile(StorageFile p_file)
        {
            return Task.Run(async () =>
            {
                if (p_file != null)
                {
                    var fs = await p_file.OpenStreamForReadAsync();
                    await Import(fs);
                }
            });
        }

        async Task Import(Stream p_fs)
        {
            var connStr = $"{_host};Initial Catalog={_info.NewDb};User ID={_info.NewUser};Password={_info.NewPwd};Encrypt=True;TrustServerCertificate=True;";
            var da = new SqlServerAccess(new DbAccessInfo("sqlserver", connStr, DatabaseType.SqlServer));
            da.AutoClose = false;

            using (var sr = new StreamReader(p_fs))
            {
                var sql = sr.ReadToEnd();
                await ImportSql(sql, da);
            }

            int cntTbl = await da.GetScalar<int>("select count(*) from sysobjects where xtype='U'");
            int cntView = await da.GetScalar<int>("select count(*) from sysobjects where xtype='V'");
            int cntSp = await da.GetScalar<int>("select count(*) from sysobjects where xtype='P'");
            int cntSeq = await da.GetScalar<int>("select count(*) from sys.sequences");

            _info.Log($"导入成功，当前库：\r\n{cntTbl}个表\r\n{cntSeq}个序列\r\n{cntSp}个存储过程\r\n{cntView}个视图\r\n");

            await da.Close(true);

            _info.Log("连接串：\r\n" + connStr);
        }

        async Task ImportSql(string p_sql, SqlServerAccess p_da)
        {
            var ls = p_sql.Split("GO");
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
                        _info.Log(ex.Message);
                    }
                }
            }
        }
        
        async Task DoCreateDb()
        {
            _info.Log($"创建数据库【{_info.NewDb}】...");

            // 数据文件和默认库放在同一路径
            var path = await _da.GetScalar<string>($"select filename from sys.sysdatabases where name='{_info.DefDb}'");
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
    FILEGROWTH = 5 MB)", _info.NewDb, path));

            _info.Log("创建成功！");
        }

        async Task CreateUser()
        {
            _info.Log($"创建用户【{_info.NewUser}】...");
            await _da.Exec($"create login {_info.NewUser} with password='{_info.NewPwd}',default_database={_info.NewDb}");
            await _da.Exec("use " + _info.NewDb);
            await _da.Exec(string.Format("create user {0} for login {0} with default_schema=dbo", _info.NewUser));
            _info.Log($"创建成功！密码：{_info.NewPwd}");

            _info.Log($"数据库【{_info.NewDb}】的所有权限授予给用户【{_info.NewUser}】...");
            await _da.Exec("sp_addrolemember", new { rolename = "db_owner", membername = _info.NewUser });
            _info.Log("授权成功！");
        }

        async Task DropExists()
        {
            if (await ExistsDb())
            {
                _info.Log($"数据库【{_info.NewDb}】已存在，正在删除...");

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
)", _info.NewDb));
                foreach (var id in ls)
                {
                    await _da.Exec("kill " + id);
                }

                // 删库后用户自动删除
                await _da.Exec($"drop database {_info.NewDb}");
                if (!await ExistsDb())
                {
                    _info.Log($"【{_info.NewDb}】删除成功！");
                }
                else
                {
                    throw new Exception("数据库删除失败！");
                }
            }

            if (await ExistsUser())
            {
                _info.Log($"登录名【{_info.NewUser}】已存在，正在删除...");
                await _da.Exec($"drop login {_info.NewUser}");
                _info.Log($"【{_info.NewUser}】删除成功！");
            }
        }
    }
}
