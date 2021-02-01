#if !WASM
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using Dt.Core.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 本地存储管理类
    /// </summary>
    public static class AtLocal
    {
        #region 成员变量
        /// <summary>
        /// 状态库文件名称
        /// </summary>
        public const string StateDbName = "State.db";
        static SqliteConnectionEx _modelDb;
        static SqliteConnectionEx _stateDb;
        #endregion

        #region 构造方法
        static AtLocal()
        {
            // 初始化不同平台的包绑定！V2支持类型和属性的绑定
            SQLitePCL.Batteries_V2.Init();

            // 创建本地文件存放目录
            if (!Directory.Exists(CachePath))
                Directory.CreateDirectory(CachePath);
        }
        #endregion

        #region 状态库

        #region 查询
        /// <summary>
        /// 查询状态库返回Table
        /// </summary>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>返回Table</returns>
        public static Table Query(string p_sql, Dict p_params = null)
        {
            return _stateDb.Query(p_sql, p_params);
        }

        /// <summary>
        /// 查询状态库，返回对象列表
        /// </summary>
        /// <typeparam name="T">ORM的映射类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>返回对象列表</returns>
        public static List<T> Query<T>(string p_sql, Dict p_params = null) where T : class
        {
            return _stateDb.Query<T>(p_sql, p_params);
        }

        /// <summary>
        /// 查询状态库，返回枚举列表
        /// </summary>
        /// <typeparam name="T">ORM的映射类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>返回可枚举列表</returns>
        public static IEnumerable<T> Each<T>(string p_sql, Dict p_params = null) where T : class
        {
            return _stateDb.DeferredQuery<T>(p_sql, p_params);
        }

        /// <summary>
        /// SQL查询，只返回第一行数据
        /// </summary>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns></returns>
        public static Row First(string p_sql, Dict p_params = null)
        {
            return _stateDb.GetRow(p_sql, p_params);
        }

        /// <summary>
        /// 查询状态库，返回符合条件的第一行数据
        /// </summary>
        /// <typeparam name="T">ORM的映射类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>返回可枚举列表</returns>
        public static T First<T>(string p_sql, Dict p_params = null) where T : class
        {
            return _stateDb.DeferredQuery<T>(p_sql, p_params).FirstOrDefault();
        }

        /// <summary>
        /// 查询状态库，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>返回可枚举列表</returns>
        public static List<T> FirstCol<T>(string p_sql, Dict p_params = null)
        {
            return _stateDb.GetFirstCol<T>(p_sql, p_params);
        }

        /// <summary>
        /// 查询状态库，返回第一个单元格数据
        /// </summary>
        /// <typeparam name="T">ORM的映射类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>返回第一个单元格数据</returns>
        public static T GetScalar<T>(string p_sql, Dict p_params = null)
        {
            return _stateDb.ExecuteScalar<T>(p_sql, p_params);
        }
        #endregion

        #region 增删改
        /// <summary>
        /// 将给定的对象插入到ORM对应的表，确保存在主键
        /// <para>1. 若存在相同主键的行则更新原行，相当于Update</para>
        /// <para>2. 若无相同主键则执行Insert</para>
        /// <para>3. 基本可以替代Insert和Update</para>
        /// <para>*. 若有自增主键插入数据时请使用Insert</para>
        /// </summary>
        /// <param name="p_obj">待保存对象</param>
        /// <returns>返回影响的行数</returns>
        public static int Save(object p_obj)
        {
            return _stateDb.Insert(p_obj, true);
        }

        /// <summary>
        /// 保存Row到对应的表
        /// </summary>
        /// <param name="p_row">数据</param>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static int Save(Row p_row, string p_tblName)
        {
            return _stateDb.InsertRow(p_row, p_tblName);
        }

        /// <summary>
        /// 事务内批量保存对象，确保存在主键
        /// <para>1. 若存在相同主键的行则更新原行，相当于Update</para>
        /// <para>2. 若无相同主键则执行Insert</para>
        /// <para>3. 基本可以替代Insert和Update</para>
        /// <para>*. 若有自增主键插入数据时请使用Insert</para>
        /// </summary>
        /// <param name="p_list">对象列表</param>
        /// <returns>返回影响的行数</returns>
        public static int BatchSave<T>(IEnumerable<T> p_list)
        {
            return _stateDb.BatchInsert(p_list, true);
        }

        /// <summary>
        /// 保存表格数据到对应的表
        /// </summary>
        /// <param name="p_tbl">数据</param>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static int BatchSave(Table p_tbl, string p_tblName)
        {
            return _stateDb.InsertTable(p_tbl, p_tblName);
        }

        /// <summary>
        /// 将给定的对象插入到ORM对应的表
        /// </summary>
        /// <param name="p_obj">待插入的对象</param>
        /// <returns>返回影响的行数</returns>
        public static int Insert(object p_obj)
        {
            return _stateDb.Insert(p_obj, false);
        }

        /// <summary>
        /// 事务内批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_list">待插入的对象列表</param>
        /// <returns>返回影响的行数</returns>
        public static int BatchInsert<T>(IEnumerable<T> p_list)
        {
            return _stateDb.BatchInsert(p_list, false);
        }

        /// <summary>
        /// 删除行数据
        /// </summary>
        /// <param name="p_rows">数据</param>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static bool Delete(IEnumerable<Row> p_rows, string p_tblName)
        {
            if (p_rows == null || string.IsNullOrEmpty(p_tblName))
                throw new Exception("待删除的数据和表名不可为空！");

            bool suc = true;
            TableMapping map = _stateDb.GetTblMapping(p_tblName);
            var pk = map.PK;
            if (pk == null || !p_rows.First().Contains(pk.Name))
                throw new Exception("无法删除无主键列的数据！");

            _stateDb.RunInTransaction(() =>
            {
                var sql = $"delete from {map.TableName} where {pk.Name}=:pkVal";
                Dict par = new Dict();
                foreach (Row dr in p_rows)
                {
                    par["pkVal"] = dr.Str(pk.Name);
                    Exec(sql, par);
                }
            });
            return suc;
        }

        /// <summary>
        /// 在状态库执行SQL语句
        /// </summary>
        /// <param name="p_sql">SQL语句</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回影响的行数</returns>
        public static int Exec(string p_sql, Dict p_params = null)
        {
            return _stateDb.Execute(p_sql, p_params);
        }

        /// <summary>
        /// 事务内批量执行SQL语句
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_list"></param>
        /// <returns></returns>
        public static int BatchExec(string p_sql, List<Dict> p_list)
        {
            return _stateDb.BatchExecute(p_sql, p_list);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 查询本地存储的Cookie值
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public static string GetCookie(string p_key)
        {
            string val = _stateDb.ExecuteScalar<string>($"select val from ClientCookie where key='{p_key}'");
            return val == null ? string.Empty : val;
        }

        /// <summary>
        /// 保存Cookie的键值
        /// </summary>
        /// <param name="p_key"></param>
        /// <param name="p_val"></param>
        public static void SaveCookie(string p_key, string p_val)
        {
            _stateDb.Insert(new ClientCookie() { Key = p_key, Val = p_val }, true);
        }

        /// <summary>
        /// 删除指定Cookie
        /// </summary>
        /// <param name="p_key"></param>
        public static void DeleteCookie(string p_key)
        {
            _stateDb.Execute($"delete from ClientCookie where key='{p_key}'");
        }

        /// <summary>
        /// 查询自启动信息
        /// </summary>
        /// <returns></returns>
        internal static AutoStartInfo GetAutoStart()
        {
            try
            {
                string json = _stateDb.ExecuteScalar<string>("select val from ClientCookie where key='AutoStart'");
                return JsonSerializer.Deserialize<AutoStartInfo>(json);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 保存自启动信息
        /// </summary>
        /// <param name="p_info"></param>
        internal static void SaveAutoStart(AutoStartInfo p_info)
        {
            if (p_info != null)
            {
                string json = JsonSerializer.Serialize(p_info, JsonOptions.UnsafeSerializer);
                _stateDb.Insert(new ClientCookie() { Key = "AutoStart", Val = json }, true);
            }
        }

        /// <summary>
        /// 删除自启动信息
        /// </summary>
        internal static void DelAutoStart()
        {
            _stateDb.Execute("delete from ClientCookie where key='AutoStart'");
        }

        /// <summary>
        /// 查询状态库中所有表名
        /// </summary>
        /// <returns></returns>
        public static Table QueryStateTblsName()
        {
            if (_stateDb == null)
                return null;
            return _stateDb.QueryTblsName();
        }

        /// <summary>
        /// 根据状态库表名获取列结构
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static TableMapping GetMapping(string p_tblName)
        {
            return _stateDb.GetTblMapping(p_tblName);
        }
        #endregion

        #endregion

        #region 模型库
        /// <summary>
        /// 查询状态库
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public static Table ModelQuery(string p_sql, Dict p_params = null)
        {
            return _modelDb.Query(p_sql, p_params);
        }

        /// <summary>
        /// 查询模型库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public static List<T> ModelQuery<T>(string p_sql, Dict p_params = null) where T : class
        {
            return _modelDb.Query<T>(p_sql, p_params);
        }

        /// <summary>
        /// 查询模型库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public static IEnumerable<T> ModelEach<T>(string p_sql, Dict p_params = null) where T : class
        {
            return _modelDb.DeferredQuery<T>(p_sql, p_params);
        }

        /// <summary>
        /// 查询模型库，返回符合条件的第一行数据
        /// </summary>
        /// <typeparam name="T">ORM的映射类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>返回可枚举列表</returns>
        public static T ModelFirst<T>(string p_sql, Dict p_params = null) where T : class
        {
            return _modelDb.DeferredQuery<T>(p_sql, p_params).FirstOrDefault();
        }

        /// <summary>
        /// 查询模型库，返回第一个单元格数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public static T ModelGetScalar<T>(string p_sql, Dict p_params = null)
        {
            return _modelDb.ExecuteScalar<T>(p_sql, p_params);
        }

        /// <summary>
        /// 在模型库执行SQL语句
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public static int ModelExec(string p_sql, Dict p_params = null)
        {
            return _modelDb.Execute(p_sql, p_params);
        }

        /// <summary>
        /// 查询指定表的所有列
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static IEnumerable<OmColumn> QueryColumns(string p_tblName)
        {
            return _modelDb.DeferredQuery<OmColumn>($"select * from OmColumn where tabname='{p_tblName}'");
        }

        /// <summary>
        /// 查询指定表的主键列
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static IEnumerable<OmColumn> QueryPrimaryColumns(string p_tblName)
        {
            return _modelDb.DeferredQuery<OmColumn>($"select * from OmColumn where tabname='{p_tblName}' and isprimary=1");
        }

        /// <summary>
        /// 获取模型库中的所有表名
        /// </summary>
        /// <returns></returns>
        public static Table QueryModelTblsName()
        {
            if (_modelDb == null)
                return null;
            return _modelDb.QueryTblsName();
        }
        #endregion

        #region 本地文件
        /* 在uno保存文件时只支持以下路径，形如：
         * LocalFolder
         * uwp：C:\Users\hdt\AppData\Local\Packages\4e169f82-ed49-494f-8c23-7dab11228222_dm57000t4aqw0\LocalState
         * android：/data/user/0/App.Droid/files
         * ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data
         * wasm：/local
         * 
         * RoamingFolder
         * android：/data/user/0/App.Droid/files/.config
         * 
         * SharedLocalFolder
         * android：/data/user/0/App.Droid/files/.local/share
         * 
         * TemporaryFolder 缓存路径，app关闭时删除，不可用于保存文件！
         */

        /// <summary>
        /// 本地文件的根路径
        /// uwp：C:\Users\...\LocalState
        /// android：/data/user/0/App.Droid/files
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data
        /// wasm：/local
        /// </summary>
        public static string RootPath
        {
            get { return ApplicationData.Current.LocalFolder.Path; }
        }

        /// <summary>
        /// 本地缓存文件的存放路径
        /// uwp：C:\Users\...\LocalState\.doc
        /// android：/data/user/0/App.Droid/files/.doc
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data/.doc
        /// wasm：/local/.doc
        /// </summary>
        public static string CachePath
        {
            get { return Path.Combine(ApplicationData.Current.LocalFolder.Path, ".doc"); }
        }

        /// <summary>
        /// 清空所有存放在.doc路径的缓存文件
        /// </summary>
        public static void ClearAllFiles()
        {
            try
            {
                if (Directory.Exists(CachePath))
                    Directory.Delete(CachePath, true);
                Directory.CreateDirectory(CachePath);
            }
            catch { }
        }

        /// <summary>
        /// 删除存放在.doc路径的本地文件
        /// </summary>
        /// <param name="p_fileName">文件名</param>
        public static void DeleteFile(string p_fileName)
        {
            try
            {
                File.Delete(Path.Combine(CachePath, p_fileName));
            }
            catch { }
        }

        /// <summary>
        /// 获取存放在.doc路径的本地文件(仅uwp可用)
        /// </summary>
        /// <param name="p_fileName"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetStorageFile(string p_fileName)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(CachePath);
            return await folder.TryGetItemAsync(p_fileName) as StorageFile;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 打开状态库 
        /// </summary>
        internal static void OpenStateDb()
        {
            try
            {
                _stateDb = new SqliteConnectionEx("Data Source=" + Path.Combine(RootPath, StateDbName));
                _stateDb.Open();

                // 初次运行、状态库版本变化或文件被删除时创建库表结构
                string id;
                if (_stateDb.ExecuteScalar<int>("select count(1) from sqlite_master where type='table'") == 0
                    || string.IsNullOrEmpty(id = GetCookie("StateDbVer"))
                    || id != AtSys.Stub.StateDbVer)
                {
                    _stateDb.InitStateDb();
                    SaveCookie("StateDbVer", AtSys.Stub.StateDbVer);
                }
            }
            catch (Exception ex)
            {
                _stateDb = null;
                throw new Exception("打开状态库异常，请重新启动应用！" + ex.Message);
            }
        }

        /// <summary>
        /// 重新打开状态库，表结构可能有变化，打开前请自行关闭原有
        /// </summary>
        public static void ReopenStateDb()
        {
            _stateDb = new SqliteConnectionEx("Data Source=" + Path.Combine(RootPath, StateDbName));
            _stateDb.Open();
            _stateDb.InitStateDb();
        }

        /// <summary>
        /// 关闭状态库
        /// </summary>
        public static void CloseStateDb()
        {
            if (_stateDb != null)
            {
                _stateDb.Close();
                _stateDb = null;
            }
        }

        /// <summary>
        /// 状态库是否已打开
        /// </summary>
        public static bool IsStateDbOpened
        {
            get { return _stateDb != null; }
        }

        /// <summary>
        /// 打开模型库
        /// </summary>
        /// <param name="p_dbName"></param>
        internal static void OpenModelDb(string p_dbName)
        {
            if (_modelDb != null)
                _modelDb.Close();
            _modelDb = new SqliteConnectionEx("Data Source=" + Path.Combine(RootPath, p_dbName));
            _modelDb.Open();
        }

        /// <summary>
        /// 关闭模型库
        /// </summary>
        internal static void CloseModelDb()
        {
            if (_modelDb != null)
            {
                _modelDb.Close();
                _modelDb = null;
            }
        }

        /// <summary>
        /// 获取本地库描述信息
        /// </summary>
        /// <param name="p_sb"></param>
        /// <returns></returns>
        internal static void GetDbInfo(StringBuilder p_sb)
        {
            if (_stateDb != null)
            {
                p_sb.Append("状态库　　----");
                int cnt = _stateDb.ExecuteScalar<int>("select count(*) from sqlite_master where type='table'");
                p_sb.Append(cnt);
                p_sb.Append("表");
                try
                {
                    FileInfo fi = new FileInfo(Path.Combine(RootPath, StateDbName));
                    p_sb.Append("，");
                    p_sb.Append(AtKit.GetFileSizeDesc((ulong)fi.Length));
                }
                catch { }
                p_sb.AppendLine();
            }

            if (_modelDb != null)
            {
                p_sb.Append("模型库　　----");
                int cnt = _modelDb.ExecuteScalar<int>("select count(*) from sqlite_master where type='table'");
                p_sb.Append(cnt);
                p_sb.Append("表");
                try
                {
                    FileInfo fi = new FileInfo(Path.Combine(RootPath, _modelDb.DataSource));
                    p_sb.Append("，");
                    p_sb.Append(AtKit.GetFileSizeDesc((ulong)fi.Length));
                }
                catch { }
                p_sb.AppendLine();
            }
        }
        #endregion
    }
}
#endif