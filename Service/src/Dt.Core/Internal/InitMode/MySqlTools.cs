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
    class MySqlTools : IDbTools
    {
        MySqlAccess _da;
        string _host;
        string _newDb;
        string _newUser;
        string _newPwd;

        public MySqlTools(List<string> p_list)
        {
            _host = $"Server={p_list[1]};Port={p_list[2]}";
            var connStr = $"{_host};Uid={p_list[4]};Pwd={p_list[5]};";
            _da = new MySqlAccess(new DbInfo("mysql", connStr, DatabaseType.MySql));

            _newDb = p_list[6].Trim();
            _newUser = p_list[7].Trim();
            _newPwd = p_list[8].Trim();
            Kit.TraceSql = false;
        }

        public static async Task<bool> TestConnect(List<string> p_list)
        {
            var da = new MySqlAccess(new DbInfo(
                "mysql",
                $"Server={p_list[1]};Port={p_list[2]};Uid={p_list[4]};Pwd={p_list[5]};",
                DatabaseType.MySql));
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
            return await _da.GetScalar<int>($"select count(*) from information_schema.schemata where schema_name='{_newDb}'") > 0;
        }

        public async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from mysql.user where user='{_newUser}'") > 0;
        }

        public async Task<bool> IsPwdCorrect()
        {
            var connStr = $"{_host};Database={_newDb};Uid={_newUser};Pwd={_newPwd};";
            var da = new MySqlAccess(new DbInfo("mysql", connStr, DatabaseType.MySql));
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
            if (await ExistsDb())
            {
                Log.Warning($"数据库【{_newDb}】已存在，正在删除...");
                await _da.Exec($"drop database {_newDb}");
                Log.Warning($"【{_newDb}】删除成功！");
            }

            if (await ExistsUser())
            {
                Log.Warning($"用户【{_newUser}】已存在，正在删除...");
                await _da.Exec($"drop user '{_newUser}'@'%'");
                Log.Warning($"【{_newUser}】删除成功！");
            }

            Log.Information($"创建数据库【{_newDb}】...");
            await _da.Exec($"create database {_newDb}");
            Log.Information("创建成功！");

            Log.Information($"创建用户【{_newUser}】...");
            await _da.Exec($"create user '{_newUser}'@'%' identified by '{_newPwd}'");
            Log.Information($"创建成功！密码：{_newPwd}");

            Log.Information($"数据库【{_newDb}】的所有权限授予给用户【{_newUser}】...");
            await _da.Exec($"grant all privileges on {_newDb}.* to '{_newUser}'@'%'");
            Log.Information("授权成功！");

            await _da.Close(true);
            Log.Information("创建空库成功");

            return await Import(p_initType);
        }

        public Task<bool> ImportToDb(int p_initType)
        {
            return Import(p_initType);
        }

        async Task<bool> Import(int p_initType)
        {
            string sql;
            var connStr = $"{_host};Database={_newDb};Uid={_newUser};Pwd={_newPwd};";
            var da = new MySqlAccess(new DbInfo("mysql", connStr, DatabaseType.MySql));
            da.AutoClose = false;

            using (var sr = GetSqlStream(p_initType == 0 ? "mysql-init.sql" : "mysql-demo.sql"))
            {
                sql = sr.ReadToEnd();
            }
            await da.Exec(sql);

            int cntTbl = await da.GetScalar<int>($"select count(*) from information_schema.tables where table_schema='{_newDb}'");
            int cntFun = await da.GetScalar<int>($"select count(*) from information_schema.routines where routine_schema='{_newDb}' and routine_type='function'");
            int cntSp = await da.GetScalar<int>($"select count(*) from information_schema.routines where routine_schema='{_newDb}' and routine_type='procedure'");
            int cntView = await da.GetScalar<int>($"select count(*) from information_schema.views where table_schema='{_newDb}'");

            var tp = p_initType == 0 ? "标准库" : "样例库";
            Log.Information($"{tp}初始化成功：\r\n{cntTbl}个表\r\n{cntFun}个函数\r\n{cntSp}个存储过程\r\n{cntView}个视图\r\n");

            await da.Close(true);

            Log.Information("连接串：\r\n" + connStr);
            return true;
        }

        internal static StreamReader GetSqlStream(string p_sqlFile)
        {
            return new StreamReader(typeof(MySqlTools).Assembly.GetManifestResourceStream("Dt.Core.Internal.InitMode.InitSql." + p_sqlFile));
        }
    }
}
