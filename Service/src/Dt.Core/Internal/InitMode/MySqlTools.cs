﻿#region 文件描述
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

        public MySqlTools(List<string> p_list)
        {
            _host = $"Server={p_list[1]};Port={p_list[2]}";
            var connStr = $"{_host};Database={p_list[3]};Uid=root;Pwd={p_list[4]};";
            _da = new MySqlAccess(new DbInfo("mysql", connStr, DatabaseType.MySql, false));

            _newDb = p_list[5];
            _newUser = p_list[6];
            Kit.TraceSql = false;
        }

        public async Task<string> IsExists()
        {
            string msg = null;
            if (await ExistsDb())
            {
                msg = $"数据库【{_newDb}】";
            }
            if (await ExistsUser())
            {
                if (msg == null)
                    msg = $"用户名【{_newUser}】";
                else
                    msg += $"、用户名【{_newUser}】";
            }

            if (msg != null)
            {
                msg += "已存在，\r\n点击【确定】将删除重建！\r\n需要【确定】多次避免误操作！";
            }
            return msg;
        }

        async Task<bool> ExistsDb()
        {
            return await _da.GetScalar<int>($"select count(*) from information_schema.schemata where schema_name = '{_newDb}'") == 1;
        }

        async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from mysql.user where user='{_newUser}'") == 1;
        }

        public async Task<bool> InitDb()
        {
            _da.AutoClose = false;
            if (await ExistsDb())
            {
                Log.Warning($"数据库【{_newDb}】已存在，正在删除...");
                await _da.Exec($"drop database {_newDb}");
                if (!await ExistsDb())
                {
                    Log.Warning($"【{_newDb}】删除成功！");
                }
                else
                {
                    Log.Error("删除失败！");
                    return false;
                }
            }

            if (await ExistsUser())
            {
                Log.Warning($"用户【{_newUser}】已存在，正在删除...");
                await _da.Exec($"drop user '{_newUser}'@'%'");
                if (!await ExistsUser())
                {
                    Log.Warning($"【{_newUser}】删除成功！");
                }
                else
                {
                    Log.Error("删除失败！");
                    return false;
                }
            }

            Log.Information($"创建数据库【{_newDb}】...");
            await _da.Exec($"create database {_newDb}");
            if (await ExistsDb())
            {
                Log.Information("创建成功！");
            }
            else
            {
                Log.Error("创建数据库失败！");
                return false;
            }

            Log.Information($"创建用户【{_newUser}】...");
            await _da.Exec($"create user '{_newUser}'@'%' identified by '{_newDb}'");
            if (await ExistsUser())
            {
                Log.Information($"创建成功！默认密码：{_newDb}");
            }
            else
            {
                Log.Error("创建用户失败！");
                return false;
            }

            Log.Information($"数据库【{_newDb}】的所有权限授予给用户【{_newUser}】...");
            await _da.Exec($"grant all privileges on {_newDb}.* to '{_newUser}'@'%'");
            Log.Information("授权成功！");

            await _da.Close(true);
            Log.Information($"关闭root连接，打开新库【{_newDb}】的连接");

            var connStr = $"{_host};Database={_newDb};Uid={_newUser};Pwd={_newDb};";
            var da = new MySqlAccess(new DbInfo("mysql", connStr, DatabaseType.MySql, false));

            string sql;
            using (var sr = new StreamReader(typeof(DtMiddleware).Assembly.GetManifestResourceStream("Dt.Core.Res.mysql-init.sql")))
            {
                sql = sr.ReadToEnd();
            }

            Log.Information($"初始化数据库...");
            int cnt = await da.Exec(sql);
            Log.Information($"初始化成功，运行脚本{cnt}个");

            Log.Information("新库连接串：\r\n" + connStr);
            return true;
        }
    }
}