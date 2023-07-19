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
    class PostgreSqlTools : IDbTools
    {
        PostgreSqlAccess _da;
        string _host;
        string _newDb;
        string _newUser;
        string _pwdPostgres;

        public PostgreSqlTools(List<string> p_list)
        {
            _host = $"Host={p_list[1]};Port={p_list[2]}";
            var connStr = $"{_host};Database={p_list[3]};Username=postgres;Password={p_list[4]};";
            _da = new PostgreSqlAccess(new DbInfo("pg", connStr, DatabaseType.PostgreSql));

            _pwdPostgres = p_list[4];
            _newDb = p_list[5].Trim().ToLower();
            _newUser = p_list[6].Trim().ToLower();
            Kit.TraceSql = false;
        }

        public async Task<string> IsExists()
        {
            string msg = null;
            if (await ExistsDb())
            {
                msg = $"表空间【{_newDb}】";
            }
            if (await ExistsUser())
            {
                if (msg == null)
                    msg = $"用户【{_newUser}】";
                else
                    msg += $"、用户【{_newUser}】";
            }

            if (msg != null)
            {
                msg += "已存在，\r\n点击【确定】将删除重建！\r\n需要【确定】多次避免误操作！";
            }
            return msg;
        }

        async Task<bool> ExistsDb()
        {
            return await _da.GetScalar<int>($"select count(*) from pg_database where datname='{_newDb}'") > 0;
        }

        async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from pg_user where usename='{_newUser}'") > 0;
        }

        public async Task<bool> InitDb(int p_initType)
        {
            _da.AutoClose = false;
            await DropExistsDB();

            Log.Information($"正在创建用户 {_newUser} ...");
            await _da.Exec($"create user {_newUser} with password '{_newDb}'");
            Log.Information($"创建用户成功！默认密码：{_newDb}");

            Log.Information($"创建数据库【{_newDb}】...");
            await _da.Exec($"create database {_newDb} owner {_newUser}");
            Log.Information("创建成功！");
            await _da.Close(true);

            // 切换库，授权
            var connStr = $"{_host};Database={_newDb};Username=postgres;Password={_pwdPostgres};";
            var da = new PostgreSqlAccess(new DbInfo("pg", connStr, DatabaseType.PostgreSql));
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

            connStr = $"{_host};Database={_newDb};Username={_newUser};Password={_newDb};";

            if (p_initType != 1)
            {
                da = new PostgreSqlAccess(new DbInfo("pg", connStr, DatabaseType.PostgreSql));
                da.AutoClose = false;

                string sql;
                using (var sr = MySqlTools.GetSqlStream(p_initType == 0 ? "postgresql-init.sql" : "postgresql-demo.sql"))
                {
                    sql = sr.ReadToEnd();
                }

                await da.Exec(sql);
                int cntTbl = await da.GetScalar<int>($"select count(*) from pg_tables where schemaname='public'");
                int cntSp = await da.GetScalar<int>($"select count(*) from pg_proc p join pg_namespace n on p.pronamespace = n.oid where n.nspname='public' and p.prokind = 'p'");
                int cntSeq = await da.GetScalar<int>($"select count(*) from pg_sequence");
                Log.Information($"新库初始化成功，共{cntTbl}个表，{cntSp}个存储过程, {cntSeq}个序列");

                await da.Close(true);
            }

            Log.Information("新库连接串：\r\n" + connStr);
            return true;
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
