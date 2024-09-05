#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Oracle.ManagedDataAccess.Client;
using Windows.Storage;
#endregion

namespace Dt.Base.Tools
{
    class OracleTools : IDbTools
    {
        DbInitInfo _info;
        OracleAccess _da;
        string _host;
        string _newDb => _info.NewDb.ToUpper();
        string _newUser => _info.NewUser.ToUpper();

        public OracleTools(DbInitInfo p_info)
        {
            _info = p_info;
            _host = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={_info.Host})(PORT={_info.Port}))(CONNECT_DATA=(SERVICE_NAME={_info.DefDb})(SERVER=dedicated)))";
            var connStr = $"User Id={_info.DefUser};Password={_info.Pwd};{_host}";
            _da = new OracleAccess(new DbAccessInfo("orcl", connStr, DatabaseType.Oracle));
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
            var connStr = $"User Id={_newUser};Password={_info.NewPwd};{_host}";
            var da = new OracleAccess(new DbAccessInfo("orcl", connStr, DatabaseType.Oracle));
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

                await CreateTablespace();
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
                await DropExistsDB();
                await _da.Close(true);
            });
        }

        public Task ImportInit()
        {
            return Task.Run(async () =>
            {
                await Import(DbKit.GetFileStream("oracle-init.sql"));
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
            var connStr = $"User Id={_newUser};Password={_info.NewPwd};{_host}";
            var da = new OracleAccess(new DbAccessInfo("orcl", connStr, DatabaseType.Oracle));
            da.AutoClose = false;

            await ImportSql(p_fs, da);
            
            int cntTbl = await da.GetScalar<int>("select count(*) from user_tables");
            int cntSeq = await da.GetScalar<int>("select count(*) from user_objects where object_type='SEQUENCE'");
            int cntSp = await da.GetScalar<int>("select count(*) from user_objects where object_type='PROCEDURE'");
            int cntView = await da.GetScalar<int>("select count(*) from user_objects where object_type='VIEW'");

            _info.Log($"导入成功，当前库：\r\n{cntTbl}个表\r\n{cntSeq}个序列\r\n{cntSp}个存储过程\r\n{cntView}个视图\r\n");

            await da.Close(true);

            _info.Log("连接串：\r\n" + connStr);

            // 不清理连接池，再次创建同样表空间时无法删除文件
            OracleConnection.ClearAllPools();
        }

        async Task ImportSql(Stream p_fs, OracleAccess p_da)
        {
            string sql = "";
            using (var sr = new StreamReader(p_fs))
            {
                bool isComment = false;
                while (!sr.EndOfStream)
                {
                    var temp = sr.ReadLine().Trim();
                    if (temp.StartsWith("--")
                        || temp.StartsWith("//")
                        || temp.StartsWith("prompt ", StringComparison.OrdinalIgnoreCase)
                        || temp.StartsWith("set ", StringComparison.OrdinalIgnoreCase))
                    {
                        sql = "";
                        continue;
                    }

                    if (isComment)
                    {
                        if (temp.EndsWith("*/"))
                            isComment = false;
                        sql = "";
                        continue;
                    }
                    if (temp.StartsWith("/*"))
                    {
                        isComment = true;
                        sql = "";
                        continue;
                    }

                    if (sql != "")
                        sql += " ";
                    sql += temp;
                    if (temp.EndsWith(';'))
                    {
                        try
                        {
                            await p_da.Exec(sql.TrimEnd(';'));
                        }
                        catch (Exception ex)
                        {
                            if (!sql.StartsWith("DROP ", StringComparison.OrdinalIgnoreCase))
                                Log.Warning(ex.Message);
                        }
                        finally
                        {
                            sql = "";
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

            _info.Log($"正在创建表空间 {_newDb}...");
            string temp = $"create tablespace \"{_newDb}\" logging datafile '{path}.ora' size 50m autoextend on next 50m maxsize unlimited extent management local segment space management auto";
            await _da.Exec(temp);

            _info.Log($"正在创建临时表空间 {_newDb}TEMP...");
            temp = $"create temporary tablespace \"{_newDb}TEMP\" tempfile '{path}TEMP.ora' size 20m autoextend on next 20m maxsize unlimited extent management local";
            await _da.Exec(temp);
            _info.Log("创建表空间成功");
        }

        async Task CreateUser()
        {
            _info.Log($"正在创建用户 {_newUser} ...");
            await _da.Exec($"create user {_newUser} identified by {_info.NewPwd} default tablespace \"{_newDb}\" temporary tablespace \"{_newDb}TEMP\"");
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

            _info.Log($"创建用户并赋予权限成功！密码：{_info.NewPwd}");
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
                _info.Log($"正在断开{tbl.Count}个会话...");
                foreach (var row in tbl)
                {
                    await _da.Exec(row[0].ToString());
                }
                _info.Log("已全部断开");
            }
        }

        async Task DeleteUser()
        {
            _info.Log($"正在删除用户【{_newUser}】...");
            await _da.Exec($"drop user {_newUser} cascade");
            _info.Log($"【{_newUser}】删除成功！");
        }

        async Task DeleteTablespace()
        {
            _info.Log($"正在删除表空间 {_newDb}...");
            await _da.Exec($"drop tablespace {_newDb} including contents and datafiles cascade constraints");

            _info.Log($"正在删除临时表空间 {_newDb}TEMP...");
            await _da.Exec($"drop tablespace {_newDb}TEMP including contents and datafiles");
            _info.Log("删除成功！");
        }
        #endregion
    }
}
