﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Oracle.ManagedDataAccess.Client;
#endregion

namespace Dt.Core
{
    class OracleTools : IDbTools
    {
        OracleAccess _da;
        string _host;
        string _newDb;
        string _newUser;
        string _newPwd;

        public OracleTools(List<string> p_list)
        {
            _host = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={p_list[1]})(PORT={p_list[2]}))(CONNECT_DATA=(SERVICE_NAME={p_list[3]})(SERVER=dedicated)))";
            var connStr = $"User Id={p_list[4]};Password={p_list[5]};{_host}";
            _da = new OracleAccess(new DbInfo("orcl", connStr, DatabaseType.Oracle));

            _newDb = p_list[6].Trim().ToUpper();
            _newUser = p_list[7].Trim().ToUpper();
            _newPwd = p_list[8].Trim();
            Kit.TraceSql = false;
        }

        public static async Task<bool> TestConnect(List<string> p_list)
        {
            var da = new OracleAccess(new DbInfo(
                "orcl",
                $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={p_list[1]})(PORT={p_list[2]}))(CONNECT_DATA=(SERVICE_NAME={p_list[3]})(SERVER=dedicated)));User Id={p_list[4]};Password={p_list[5]};",
                DatabaseType.Oracle));
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
            return await _da.GetScalar<int>($"select count(*) from sys.dba_tablespaces where tablespace_name='{_newDb}'") > 0;
        }

        public async Task<bool> ExistsUser()
        {
            return await _da.GetScalar<int>($"select count(*) from all_users where username='{_newUser}'") > 0;
        }

        public async Task<bool> IsPwdCorrect()
        {
            var connStr = $"User Id={_newUser};Password={_newPwd};{_host}";
            var da = new OracleAccess(new DbInfo("orcl", connStr, DatabaseType.Oracle));
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
            await DropExistsDB();

            await CreateTablespace();
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
            var connStr = $"User Id={_newUser};Password={_newPwd};{_host}";
            var da = new OracleAccess(new DbInfo("orcl", connStr, DatabaseType.Oracle));
            da.AutoClose = false;

            if (p_dropExists)
            {
                await DropTbl("drop-init.txt", da);
                if (p_initType == 1)
                    await DropTbl("drop-demo.txt", da);
                Log.Information($"删除旧表");
            }

            Log.Information($"初始化数据库...");
            await ImportSql("oracle-init.sql", da);
            if (p_initType == 1)
                await ImportSql("oracle-demo.sql", da);

            int cntTbl = await da.GetScalar<int>("select count(*) from user_tables");
            int cntSeq = await da.GetScalar<int>("select count(*) from user_objects where object_type='SEQUENCE'");
            int cntSp = await da.GetScalar<int>("select count(*) from user_objects where object_type='PROCEDURE'");
            int cntView = await da.GetScalar<int>("select count(*) from user_objects where object_type='VIEW'");

            var tp = p_initType == 0 ? "标准库" : "样例库";
            Log.Information($"{tp}初始化成功：\r\n{cntTbl}个表\r\n{cntSeq}个序列\r\n{cntSp}个存储过程\r\n{cntView}个视图\r\n");

            await da.Close(true);

            Log.Information("连接串：\r\n" + connStr);

            // 不清理连接池，再次创建同样表空间时无法删除文件
            OracleConnection.ClearAllPools();
            return true;
        }

        async Task ImportSql(string p_file, OracleAccess p_da)
        {
            string sql;
            using (var sr = MySqlTools.GetSqlStream(p_file))
            {
                sql = sr.ReadToEnd();
            }
                        
            var ls = sql.Split(';');
            foreach (var item in ls)
            {
                if (!string.IsNullOrWhiteSpace(item) && item != "\r\nCOMMIT")
                {
                    var str = item;
                    // 存储过程
                    if (str.EndsWith("END"))
                    {
                        str = str.Substring(0, str.Length - 3).TrimEnd() + ";\r\n\r\nEND;";
                    }
                    try
                    {
                        await p_da.Exec(str);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex.Message);
                    }
                }
            }
        }

        async Task DropTbl(string p_file, OracleAccess p_da)
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
                            await p_da.Exec($"DROP TABLE {tbl}");
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex.Message);
                        }
                    }
                }
            }
        }

        #region 创建表空间和用户
        async Task CreateTablespace()
        {
            var path = await _da.GetScalar<string>("select file_name from dba_data_files where tablespace_name='SYSTEM'");
            if (path.Contains('/'))
            {
                int index = path.LastIndexOf('/');
                path = path.Substring(0, index) + "/" + _newDb;
            }
            else
            {
                int index = path.LastIndexOf('\\');
                path = path.Substring(0, index) + "\\" + _newDb;
            }

            Log.Information($"正在创建表空间 {_newDb}...");
            string temp = $"create tablespace \"{_newDb}\" logging datafile '{path}.ora' size 50m autoextend on next 50m maxsize unlimited extent management local segment space management auto";
            await _da.Exec(temp);

            Log.Information($"正在创建临时表空间 {_newDb}TEMP...");
            temp = $"create temporary tablespace \"{_newDb}TEMP\" tempfile '{path}TEMP.ora' size 20m autoextend on next 20m maxsize unlimited extent management local";
            await _da.Exec(temp);
            Log.Information("创建表空间成功");
        }

        async Task CreateUser()
        {
            Log.Information($"正在创建用户 {_newUser} ...");
            await _da.Exec($"create user {_newUser} identified by {_newPwd} default tablespace \"{_newDb}\" temporary tablespace \"{_newDb}TEMP\"");
            await _da.Exec($"grant DBA to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY CLUSTER to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY DIMENSION to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY INDEX to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY INDEXTYPE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY LIBRARY to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY OUTLINE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY PROCEDURE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY ROLE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY SEQUENCE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY SNAPSHOT to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY TRIGGER to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ANY TYPE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER DATABASE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER PROFILE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER RESOURCE COST to {_newUser} with admin option");
            await _da.Exec($"grant ALTER ROLLBACK SEGMENT to {_newUser} with admin option");
            await _da.Exec($"grant ALTER SESSION to {_newUser} with admin option");
            await _da.Exec($"grant ALTER SYSTEM to {_newUser} with admin option");
            await _da.Exec($"grant ALTER TABLESPACE to {_newUser} with admin option");
            await _da.Exec($"grant ALTER USER to {_newUser} with admin option");
            await _da.Exec($"grant ANALYZE ANY to {_newUser} with admin option");
            await _da.Exec($"grant AUDIT ANY to {_newUser} with admin option");
            await _da.Exec($"grant AUDIT SYSTEM to {_newUser} with admin option");
            await _da.Exec($"grant BACKUP ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant BECOME USER to {_newUser} with admin option");
            await _da.Exec($"grant COMMENT ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY CLUSTER to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY CONTEXT to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY DIMENSION to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY DIRECTORY to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY INDEX to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY INDEXTYPE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY LIBRARY to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY OUTLINE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY PROCEDURE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY SEQUENCE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY SNAPSHOT to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY SYNONYM to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY TRIGGER to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY TYPE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ANY VIEW to {_newUser} with admin option");
            await _da.Exec($"grant CREATE CLUSTER to {_newUser} with admin option");
            await _da.Exec($"grant CREATE DATABASE LINK to {_newUser} with admin option");
            await _da.Exec($"grant CREATE DIMENSION to {_newUser} with admin option");
            await _da.Exec($"grant CREATE INDEXTYPE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE LIBRARY to {_newUser} with admin option");
            await _da.Exec($"grant CREATE PROCEDURE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE PROFILE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE PUBLIC DATABASE LINK to {_newUser} with admin option");
            await _da.Exec($"grant CREATE PUBLIC SYNONYM to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ROLE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE ROLLBACK SEGMENT to {_newUser} with admin option");
            await _da.Exec($"grant CREATE SEQUENCE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE SESSION to {_newUser} with admin option");
            await _da.Exec($"grant CREATE SNAPSHOT to {_newUser} with admin option");
            await _da.Exec($"grant CREATE SYNONYM to {_newUser} with admin option");
            await _da.Exec($"grant CREATE TABLE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE TABLESPACE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE TRIGGER to {_newUser} with admin option");
            await _da.Exec($"grant CREATE TYPE to {_newUser} with admin option");
            await _da.Exec($"grant CREATE USER to {_newUser} with admin option");
            await _da.Exec($"grant CREATE VIEW to {_newUser} with admin option");
            await _da.Exec($"grant DEBUG ANY PROCEDURE to {_newUser} with admin option");
            await _da.Exec($"grant DEBUG CONNECT SESSION to {_newUser} with admin option");
            await _da.Exec($"grant DELETE ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY CLUSTER to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY CONTEXT to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY DIMENSION to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY DIRECTORY to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY INDEXTYPE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY LIBRARY to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY OUTLINE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY PROCEDURE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY ROLE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY SEQUENCE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY SNAPSHOT to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY SYNONYM to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY TRIGGER to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY TYPE to {_newUser} with admin option");
            await _da.Exec($"grant DROP ANY VIEW to {_newUser} with admin option");
            await _da.Exec($"grant DROP PROFILE to {_newUser} with admin option");
            await _da.Exec($"grant DROP PUBLIC DATABASE LINK to {_newUser} with admin option");
            await _da.Exec($"grant DROP PUBLIC SYNONYM to {_newUser} with admin option");
            await _da.Exec($"grant DROP ROLLBACK SEGMENT to {_newUser} with admin option");
            await _da.Exec($"grant DROP TABLESPACE to {_newUser} with admin option");
            await _da.Exec($"grant DROP USER to {_newUser} with admin option");
            await _da.Exec($"grant EXECUTE ANY INDEXTYPE to {_newUser} with admin option");
            await _da.Exec($"grant EXECUTE ANY LIBRARY to {_newUser} with admin option");
            await _da.Exec($"grant EXECUTE ANY PROCEDURE to {_newUser} with admin option");
            await _da.Exec($"grant EXECUTE ANY TYPE to {_newUser} with admin option");
            await _da.Exec($"grant FORCE ANY TRANSACTION to {_newUser} with admin option");
            await _da.Exec($"grant FORCE TRANSACTION to {_newUser} with admin option");
            await _da.Exec($"grant GLOBAL QUERY REWRITE to {_newUser} with admin option");
            await _da.Exec($"grant grant ANY OBJECT PRIVILEGE to {_newUser} with admin option");
            await _da.Exec($"grant grant ANY PRIVILEGE to {_newUser} with admin option");
            await _da.Exec($"grant grant ANY ROLE to {_newUser} with admin option");
            await _da.Exec($"grant INSERT ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant LOCK ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant MANAGE TABLESPACE to {_newUser} with admin option");
            await _da.Exec($"grant ON COMMIT REFRESH to {_newUser} with admin option");
            await _da.Exec($"grant QUERY REWRITE to {_newUser} with admin option");
            await _da.Exec($"grant RESTRICTED SESSION to {_newUser} with admin option");
            await _da.Exec($"grant SELECT ANY DICTIONARY to {_newUser} with admin option");
            await _da.Exec($"grant SELECT ANY SEQUENCE to {_newUser} with admin option");
            await _da.Exec($"grant SELECT ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant UNDER ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"grant UNDER ANY TYPE to {_newUser} with admin option");
            await _da.Exec($"grant UNDER ANY VIEW to {_newUser} with admin option");
            await _da.Exec($"grant UNLIMITED TABLESPACE to {_newUser} with admin option");
            await _da.Exec($"grant UPDATE ANY TABLE to {_newUser} with admin option");
            await _da.Exec($"revoke unlimited tablespace from {_newUser}");
            await _da.Exec($"alter user {_newUser} quota 0 on users");
            await _da.Exec($"alter user {_newUser} quota unlimited on {_newDb}");

            Log.Information($"创建用户并赋予权限成功！密码：{_newPwd}");
        }
        #endregion

        #region 删除旧库和用户
        async Task DropExistsDB()
        {
            if (await ExistsUser())
            {
                await KillSessions();
                await DeleteUser();
            }
            if (await ExistsDb())
                await DeleteTablespace();
        }

        async Task KillSessions()
        {
            var tbl = await _da.Query($"select 'alter system kill session ' || '''' ||t.sid ||','||t.serial#|| '''' sql from v$session t where t.username='{_newUser}'");
            if (tbl.Count > 0)
            {
                Log.Information($"正在断开{tbl.Count}个会话...");
                foreach (var row in tbl)
                {
                    await _da.Exec(row[0].ToString());
                }
                Log.Information("已全部断开");
            }
        }

        async Task DeleteUser()
        {
            Log.Warning($"正在删除用户【{_newUser}】...");
            await _da.Exec($"drop user {_newUser} cascade");
            Log.Warning($"【{_newUser}】删除成功！");
        }

        async Task DeleteTablespace()
        {
            Log.Warning($"正在删除表空间 {_newDb}...");
            await _da.Exec($"drop tablespace {_newDb} including contents and datafiles cascade constraints");

            Log.Warning($"正在删除临时表空间 {_newDb}TEMP...");
            await _da.Exec($"drop tablespace {_newDb}TEMP including contents and datafiles");
            Log.Warning("删除成功！");
        }
        #endregion
    }
}
