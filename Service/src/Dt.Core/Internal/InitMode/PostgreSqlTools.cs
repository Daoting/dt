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

namespace Dt.Core
{
    class PostgreSqlTools : IDbTools
    {
        PostgreSqlAccess _da;
        string _host;
        string _newDb;
        string _newUser;
        string _newPwd;
        string _sysUser;

        public PostgreSqlTools(List<string> p_list)
        {
            _host = $"Host={p_list[1]};Port={p_list[2]}";
            var connStr = $"{_host};Database={p_list[3]};Username={p_list[4]};Password={p_list[5]};";
            _da = new PostgreSqlAccess(new DbAccessInfo("pg", connStr, DatabaseType.PostgreSql));

            _sysUser = $"Username={p_list[4]};Password={p_list[5]};";
            _newDb = p_list[6].Trim().ToLower();
            _newUser = p_list[7].Trim().ToLower();
            _newPwd = p_list[8].Trim();
            Kit.TraceSql = false;
        }

        public static async Task<bool> TestConnect(List<string> p_list)
        {
            var da = new PostgreSqlAccess(new DbAccessInfo(
                "pg",
                $"Host={p_list[1]};Port={p_list[2]};Database={p_list[3]};Username={p_list[4]};Password={p_list[5]};",
                DatabaseType.PostgreSql));
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
            return await _da.GetScalar<int>($"select count(*) from pg_database where datname='{_newDb}'") > 0;
        }

        public async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from pg_user where usename='{_newUser}'") > 0;
        }

        public async Task<bool> IsPwdCorrect()
        {
            var connStr = $"{_host};Database={_newDb};Username={_newUser};Password={_newPwd};";
            var da = new PostgreSqlAccess(new DbAccessInfo("pg", connStr, DatabaseType.PostgreSql));
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

        public async Task InitDb()
        {
            _da.AutoClose = false;
            await DropExistsDB();

            Log.Information($"正在创建用户 {_newUser} ...");
            await _da.Exec($"create user {_newUser} with password '{_newPwd}'");
            Log.Information($"创建用户成功！密码：{_newPwd}");

            Log.Information($"创建数据库【{_newDb}】...");
            await _da.Exec($"create database {_newDb} owner {_newUser}");
            Log.Information("创建成功！");
            await _da.Close(true);
            NpgsqlConnection.ClearAllPools();

            // 切换库，授权
            var connStr = $"{_host};Database={_newDb};{_sysUser}";
            var da = new PostgreSqlAccess(new DbAccessInfo("pg", connStr, DatabaseType.PostgreSql));
            da.AutoClose = false;
            Log.Information($"数据库【{_newDb}】的所有权限授予给用户【{_newUser}】...");
            await da.Exec($"grant all privileges on database {_newDb} to {_newUser}");
            await da.Exec($"grant usage on schema public to {_newUser}");
            await da.Exec($"grant all privileges on all tables in schema public to {_newUser}");
            await da.Exec($"grant all privileges on all sequences in schema public to {_newUser}");
            await da.Exec($"grant select,insert,update,delete on all tables in schema public to {_newUser}");
            await da.Exec($"grant all on schema public to {_newUser}");
            Log.Information("授权成功！");

            await da.Close(true);
            Log.Information("创建空库成功");

            await Import();
        }
        
        public async Task Import()
        {
            var connStr = $"{_host};Database={_newDb};Username={_newUser};Password={_newPwd};";
            var da = new PostgreSqlAccess(new DbAccessInfo("pg", connStr, DatabaseType.PostgreSql));
            da.AutoClose = false;

            string sql;
            using (var sr = MySqlTools.GetSqlStream("postgresql-init.sql"))
            {
                sql = sr.ReadToEnd();
            }
            await da.Exec(sql);
            
            int cntTbl = await da.GetScalar<int>($"select count(*) from pg_tables where schemaname='public'");
            int cntSp = await da.GetScalar<int>($"select count(*) from pg_proc p join pg_namespace n on p.pronamespace = n.oid where n.nspname='public' and p.prokind = 'p'");
            int cntSeq = await da.GetScalar<int>($"select count(*) from pg_sequence");
            int cntView = await da.GetScalar<int>($"select count(*) from pg_views where schemaname='public'");

            Log.Information($"初始化成功：\r\n{cntTbl}个表\r\n{cntSeq}个序列\r\n{cntSp}个存储过程\r\n{cntView}个视图\r\n");

            await da.Close(true);

            Log.Information("连接串：\r\n" + connStr);

            // 不清理连接池，再次创建同样表空间时无法删除文件
            NpgsqlConnection.ClearAllPools();
        }
        
        #region 删除旧库和用户
        async Task DropExistsDB()
        {
            if (await ExistsDb())
            {
                Log.Warning($"正在删除数据库 {_newDb}...");

                // 强制关闭连接
                var tbl = await _da.Query($"select pg_terminate_backend(pid) from pg_stat_activity where datname='{_newDb}'");

                await _da.Exec($"drop database {_newDb}");
                Log.Warning("删除成功！");
            }

            if (await ExistsUser())
            {
                Log.Warning($"正在删除用户【{_newUser}】...");
                await _da.Exec($"drop user {_newUser}");
                Log.Warning($"【{_newUser}】删除成功！");
            }
        }

        #endregion
    }
}
