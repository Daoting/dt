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
    class MySqlTools : IDbTools
    {
        DbInitInfo _info;
        MySqlAccess _da;
        string _host;

        public MySqlTools(DbInitInfo p_info)
        {
            _info = p_info;
            _host = $"Host={_info.Host};Port={_info.Port}";
            var connStr = $"{_host};Uid={_info.DefUser};Pwd={_info.Pwd};";
            _da = new MySqlAccess(new DbAccessInfo("mysql", connStr, DatabaseType.MySql));
        }

        public async Task<bool> ExistsDb()
        {
            return await _da.GetScalar<int>($"select count(*) from information_schema.schemata where schema_name='{_info.NewDb}'") > 0;
        }

        public async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from mysql.user where user='{_info.NewUser}'") > 0;
        }

        public async Task<bool> IsPwdCorrect()
        {
            var connStr = $"{_host};Database={_info.NewDb};Uid={_info.NewUser};Pwd={_info.NewPwd};";
            var da = new MySqlAccess(new DbAccessInfo("mysql", connStr, DatabaseType.MySql));
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

                _info.Log($"创建数据库【{_info.NewDb}】...");
                await _da.Exec($"create database {_info.NewDb}");
                _info.Log("创建成功！");

                _info.Log($"创建用户【{_info.NewUser}】...");
                await _da.Exec($"create user '{_info.NewUser}'@'%' identified by '{_info.NewPwd}'");
                _info.Log($"创建成功！密码：{_info.NewPwd}");

                _info.Log($"数据库【{_info.NewDb}】的所有权限授予给用户【{_info.NewUser}】...");
                await _da.Exec($"grant all privileges on {_info.NewDb}.* to '{_info.NewUser}'@'%'");
                _info.Log("授权成功！");

                await _da.Close(true);
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
                await Import(DbKit.GetFileStream("mysql-init.sql"));
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
            var connStr = $"{_host};Database={_info.NewDb};Uid={_info.NewUser};Pwd={_info.NewPwd};";
            var da = new MySqlAccess(new DbAccessInfo("mysql", connStr, DatabaseType.MySql));
            da.AutoClose = false;

            using (var sr = new StreamReader(p_fs))
            {
                var sql = sr.ReadToEnd();
                await da.Exec(sql);
            }

            int cntTbl = await da.GetScalar<int>($"select count(*) from information_schema.tables where table_schema='{_info.NewDb}'");
            int cntFun = await da.GetScalar<int>($"select count(*) from information_schema.routines where routine_schema='{_info.NewDb}' and routine_type='function'");
            int cntSp = await da.GetScalar<int>($"select count(*) from information_schema.routines where routine_schema='{_info.NewDb}' and routine_type='procedure'");
            int cntView = await da.GetScalar<int>($"select count(*) from information_schema.views where table_schema='{_info.NewDb}'");

            _info.Log($"导入成功，当前库：\r\n{cntTbl}个表\r\n{cntFun}个函数\r\n{cntSp}个存储过程\r\n{cntView}个视图\r\n");

            await da.Close(true);

            _info.Log("连接串：\r\n" + connStr);
        }

        #region 删除旧库和用户
        async Task DropExistsDB()
        {
            if (await ExistsDb())
            {
                _info.Log($"数据库【{_info.NewDb}】已存在，正在删除...");
                await _da.Exec($"drop database {_info.NewDb}");
                _info.Log($"【{_info.NewDb}】删除成功！");
            }

            if (await ExistsUser())
            {
                _info.Log($"用户【{_info.NewUser}】已存在，正在删除...");
                await _da.Exec($"drop user '{_info.NewUser}'@'%'");
                _info.Log($"【{_info.NewUser}】删除成功！");
            }
        }

        #endregion
    }
}
