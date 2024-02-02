#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Npgsql;
using Windows.Storage;
#endregion

namespace Dt.Base.Tools
{
    class PostgreSqlTools : IDbTools
    {
        DbInitInfo _info;
        PostgreSqlAccess _da;
        string _host;
        string _sysUser;

        public PostgreSqlTools(DbInitInfo p_info)
        {
            _info = p_info;
            _host = $"Host={_info.Host};Port={_info.Port}";
            var connStr = $"{_host};Database={_info.DefDb};Username={_info.DefUser};Password={_info.Pwd};";
            _da = new PostgreSqlAccess(new DbAccessInfo("pg", connStr, DatabaseType.PostgreSql));
            _sysUser = $"Username={_info.DefUser};Password={_info.Pwd};";
        }

        public async Task<bool> ExistsDb()
        {
            return await _da.GetScalar<int>($"select count(*) from pg_database where datname='{_info.NewDb}'") > 0;
        }

        public async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from pg_user where usename='{_info.NewUser}'") > 0;
        }

        public async Task<bool> IsPwdCorrect()
        {
            var connStr = $"{_host};Database={_info.NewDb};Username={_info.NewUser};Password={_info.NewPwd};";
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

        public Task CreateDb()
        {
            return Task.Run(async () =>
            {
                _da.AutoClose = false;
                await DropExistsDB();

                _info.Log($"正在创建用户 {_info.NewUser} ...");
                await _da.Exec($"create user {_info.NewUser} with password '{_info.NewPwd}'");
                _info.Log($"创建用户成功！密码：{_info.NewPwd}");

                _info.Log($"创建数据库【{_info.NewDb}】...");
                await _da.Exec($"create database {_info.NewDb} owner {_info.NewUser}");
                _info.Log("创建成功！");
                await _da.Close(true);
                NpgsqlConnection.ClearAllPools();

                // 切换库，授权
                var connStr = $"{_host};Database={_info.NewDb};{_sysUser}";
                var da = new PostgreSqlAccess(new DbAccessInfo("pg", connStr, DatabaseType.PostgreSql));
                da.AutoClose = false;
                _info.Log($"数据库【{_info.NewDb}】的所有权限授予给用户【{_info.NewUser}】...");
                await da.Exec($"grant all privileges on database {_info.NewDb} to {_info.NewUser}");
                await da.Exec($"grant usage on schema public to {_info.NewUser}");
                await da.Exec($"grant all privileges on all tables in schema public to {_info.NewUser}");
                await da.Exec($"grant all privileges on all sequences in schema public to {_info.NewUser}");
                await da.Exec($"grant select,insert,update,delete on all tables in schema public to {_info.NewUser}");
                await da.Exec($"grant all on schema public to {_info.NewUser}");
                _info.Log("授权成功！");

                await da.Close(true);
                _info.Log("创建空库成功");
            });
        }

        public Task DeleteDb()
        {
            return Task.Run(async () =>
            {
                _da.AutoClose = false;
                await DropExistsDB();
                await _da.Close(true);
            });
        }

        public Task ImportInit()
        {
            return Task.Run(async () =>
            {
                await Import(DbKit.GetFileStream("postgresql-init.sql"));
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
            var connStr = $"{_host};Database={_info.NewDb};Username={_info.NewUser};Password={_info.NewPwd};";
            var da = new PostgreSqlAccess(new DbAccessInfo("pg", connStr, DatabaseType.PostgreSql));
            da.AutoClose = false;

            using (var sr = new StreamReader(p_fs))
            {
                var sql = sr.ReadToEnd();
                await da.Exec(sql);
            }

            int cntTbl = await da.GetScalar<int>($"select count(*) from pg_tables where schemaname='public'");
            int cntSp = await da.GetScalar<int>($"select count(*) from pg_proc p join pg_namespace n on p.pronamespace = n.oid where n.nspname='public' and p.prokind = 'p'");
            int cntSeq = await da.GetScalar<int>($"select count(*) from pg_sequence");
            int cntView = await da.GetScalar<int>($"select count(*) from pg_views where schemaname='public'");

            _info.Log($"导入成功，当前库：\r{cntTbl}个表\r{cntSeq}个序列\r{cntSp}个存储过程\r{cntView}个视图\r");

            await da.Close(true);

            _info.Log("连接串：\r\n" + connStr);

            // 不清理连接池，再次创建同样表空间时无法删除文件
            NpgsqlConnection.ClearAllPools();
        }

        #region 删除旧库和用户
        async Task DropExistsDB()
        {
            if (await ExistsDb())
            {
                _info.Log($"正在删除数据库 {_info.NewDb}...");

                // 强制关闭连接
                var tbl = await _da.Query($"select pg_terminate_backend(pid) from pg_stat_activity where datname='{_info.NewDb}'");

                await _da.Exec($"drop database {_info.NewDb}");
                _info.Log("删除成功！");
            }

            if (await ExistsUser())
            {
                _info.Log($"正在删除用户【{_info.NewUser}】...");
                await _da.Exec($"drop user {_info.NewUser}");
                _info.Log($"【{_info.NewUser}】删除成功！");
            }
        }

        #endregion
    }
}
