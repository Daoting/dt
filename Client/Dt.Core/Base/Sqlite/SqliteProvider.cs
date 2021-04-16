#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Dt.Core.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Sqlite本地数据提供者，为保证使用习惯一致，和DataProvider方法基本相同
    /// 为方便使用都采用静态方法，巧妙使用泛型传递库名
    /// </summary>
    /// <typeparam name="Sqlite_name">类型名称：Sqlite_xxx，xxx为库文件名称(无扩展名)，Sqlite_为前缀</typeparam>
    public abstract class SqliteProvider<Sqlite_name>
    {
        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Table Query(string p_sql, object p_params = null)
        {
            return GetDb().Query(p_sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Table<TEntity> Query<TEntity>(string p_sql, object p_params = null)
            where TEntity : Entity
        {
            return GetDb().Query<TEntity>(p_sql, p_params);
        }

        /// <summary>
        /// 返回所有实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns></returns>
        public static Table<TEntity> GetAll<TEntity>()
            where TEntity : Entity
        {
            return GetDb().Query<TEntity>($"select * from `{typeof(TEntity).Name}`");
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public static IEnumerable<Row> Each(string p_sql, object p_params = null)
        {
            return GetDb().ForEach<Row>(p_sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体枚举，高性能
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体枚举</returns>
        public static IEnumerable<TEntity> Each<TEntity>(string p_sql, object p_params = null)
            where TEntity : Entity
        {
            return GetDb().ForEach<TEntity>(p_sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static T GetScalar<T>(string p_sql, object p_params = null)
        {
            return GetDb().ExecuteScalar<T>(p_sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public static Row First(string p_sql, object p_params = null)
        {
            return GetDb().ForEach<Row>(p_sql, p_params).FirstOrDefault();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static TEntity First<TEntity>(string p_sql, object p_params = null)
            where TEntity : Entity
        {
            return GetDb().ForEach<TEntity>(p_sql, p_params).FirstOrDefault();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public static List<T> FirstCol<T>(string p_sql, object p_params = null)
        {
            return GetDb().GetFirstCol<T>(p_sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一列枚举，高性能
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型枚举</returns>
        public static IEnumerable<T> EachFirstCol<T>(string p_sql, object p_params = null)
        {
            return GetDb().EachFirstCol<T>(p_sql, p_params);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键id，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static TEntity GetByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            return GetByKey<TEntity>("id", p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键id，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static TEntity GetByID<TEntity>(long p_id)
            where TEntity : Entity
        {
            return GetByKey<TEntity>("id", p_id.ToString());
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public static TEntity GetByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            return GetDb().ForEach<TEntity>(
                $"select * from `{typeof(TEntity).Name}` where {p_keyName}='{p_keyVal}'")
                .FirstOrDefault();
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存实体数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public static bool Save<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            return false;
        }

        /// <summary>
        /// 一个事务内批量保存实体数据，根据实体状态执行增改，Table&lt;Entity&gt;支持删除，列表类型支持：
        /// <para>Table&lt;Entity&gt;，单表增删改</para>
        /// <para>List&lt;Entity&gt;，单表增改</para>
        /// <para>IList，多表增删改，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待保存列表</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>true 保存成功</returns>
        public static bool BatchSave(IList p_list, bool p_isNotify = true)
        {
            return false;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的行</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static bool Delete<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            return false;
        }

        /// <summary>
        /// 批量删除实体，单表或多表，列表类型支持：
        /// <para>Table&lt;Entity&gt;，单表删除</para>
        /// <para>List&lt;Entity&gt;，单表删除</para>
        /// <para>IList，多表删除，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待删除实体列表</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static bool BatchDelete(IList p_list, bool p_isNotify = true)
        {
            return false;
        }

        /// <summary>
        /// 批量删除行数据
        /// </summary>
        /// <param name="p_rows">数据</param>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static bool BatchDelete(string p_tblName, IEnumerable<Row> p_rows, bool p_isNotify = true)
        {
            return true;
            //if (p_rows == null || string.IsNullOrEmpty(p_tblName))
            //    throw new Exception("待删除的数据和表名不可为空！");

            //bool suc = true;
            //TableMapping map = _stateDb.GetTblMapping(p_tblName);
            //var pk = map.PK;
            //if (pk == null || !p_rows.First().Contains(pk.Name))
            //    throw new Exception("无法删除无主键列的数据！");

            //_stateDb.RunInTransaction(() =>
            //{
            //    var sql = $"delete from {map.TableName} where {pk.Name}=:pkVal";
            //    Dict par = new Dict();
            //    foreach (Row dr in p_rows)
            //    {
            //        par["pkVal"] = dr.Str(pk.Name);
            //        Exec(sql, par);
            //    }
            //});
            //return suc;
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键id，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static bool DelByID<TEntity>(string p_id, bool p_isNotify = true)
        {
            return false;
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键id，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static bool DelByID<TEntity>(long p_id, bool p_isNotify = true)
        {
            return false;
        }

        /// <summary>
        /// 根据单主键或唯一索引列删除实体，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <returns>实际删除行数</returns>
        public static bool DelByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            return false;
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        public static int Exec(string p_sql, object p_params = null)
        {
            return GetDb().Execute(p_sql, p_params);
        }

        /// <summary>
        /// 一个事务内批量执行SQL语句，如单表的批量插入
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_list">参数值列表</param>
        /// <returns>返回执行后影响的行数</returns>
        public static int BatchExec(string p_sql, List<Dict> p_list)
        {
            return GetDb().BatchExecute(p_sql, p_list);
        }

        /// <summary>
        /// 获取库中的所有表名
        /// </summary>
        /// <returns></returns>
        public static Table GetAllTables()
        {
            return GetDb().QueryTblsName();
        }
        #endregion

        #region 库管理
        static readonly Dictionary<string, SqliteConnectionEx> _dbs = new Dictionary<string, SqliteConnectionEx>();

        /// <summary>
        /// 打开Sqlite库，自动创建、同步库表结构
        /// </summary>
        public static void OpenDb()
        {
            var dbName = GetDbName();
            if (_dbs.ContainsKey(dbName))
                throw new Exception($"sqlite库[{dbName}]重复打开！");

            try
            {
                var path = Path.Combine(AtSys.DataPath, dbName + ".db");
                bool exists = File.Exists(path);
                var db = new SqliteConnectionEx("Data Source=" + path);
                db.Open();

                // 初次运行、库表结构版本变化或文件被删除时创建库表结构
                if (AtSys.Stub.SqliteDb.TryGetValue(dbName, out var dbInfo))
                {
                    path = Path.Combine(AtSys.DataPath, $"{dbName}-{dbInfo.Version}.ver");
                    if (!exists || !File.Exists(path))
                    {
                        db.InitDb(dbInfo.Tables);

                        // 删除旧版本号文件
                        foreach (var file in new DirectoryInfo(AtSys.DataPath).GetFiles($"{dbName}-*.ver"))
                        {
                            try { file.Delete(); } catch { }
                        }

                        // 创建空文件，文件名是库表结构的版本号
                        File.Create(path);
                    }
                }

                _dbs[dbName] = db;
            }
            catch (Exception ex)
            {
                throw new Exception($"打开sqlite库[{dbName}]异常，请重新启动应用！{ex.Message}");
            }
        }

        /// <summary>
        /// 关闭Sqlite库
        /// </summary>
        public static void CloseDb()
        {
            var db = GetDb();
            if (db != null)
            {
                db.Close();
                _dbs.Remove(GetDbName());
            }
        }

        protected static SqliteConnectionEx GetDb()
        {
            if (_dbs.TryGetValue(typeof(Sqlite_name).Name.Substring(7), out var db))
                return db;
            throw new Exception($"sqlite库[{typeof(Sqlite_name).Name.Substring(7)}]未打开！");
        }

        protected static string GetDbName()
        {
            return typeof(Sqlite_name).Name.Substring(7);
        }

        /// <summary>
        /// 获取所有已打开库的描述信息
        /// </summary>
        /// <returns></returns>
        public static string GetAllDbInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in _dbs)
            {
                sb.Append(item.Key);
                sb.Append("　　----");
                int cnt = item.Value.ExecuteScalar<int>("select count(*) from sqlite_master where type='table'");
                sb.Append(cnt);
                sb.Append("表");
                try
                {
                    FileInfo fi = new FileInfo(Path.Combine(AtSys.DataPath, item.Key + ".db"));
                    sb.Append("，");
                    sb.Append(AtKit.GetFileSizeDesc((ulong)fi.Length));
                }
                catch { }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        #endregion
    }
}
