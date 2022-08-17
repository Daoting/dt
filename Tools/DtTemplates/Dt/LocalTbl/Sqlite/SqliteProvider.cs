#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            return _db.Query(p_sql, p_params);
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
            return _db.Query<TEntity>(p_sql, p_params);
        }

        /// <summary>
        /// 返回所有实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns></returns>
        public static Table<TEntity> GetAll<TEntity>()
            where TEntity : Entity
        {
            return _db.Query<TEntity>($"select * from `{typeof(TEntity).Name}`");
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public static IEnumerable<Row> Each(string p_sql, object p_params = null)
        {
            return _db.ForEach<Row>(p_sql, p_params);
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
            return _db.ForEach<TEntity>(p_sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static T GetScalar<T>(string p_sql, object p_params = null)
        {
            return _db.GetScalar<T>(p_sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public static Row First(string p_sql, object p_params = null)
        {
            return _db.ForEach<Row>(p_sql, p_params).FirstOrDefault();
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
            return _db.ForEach<TEntity>(p_sql, p_params).FirstOrDefault();
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
            return _db.GetFirstCol<T>(p_sql, p_params);
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
            return _db.EachFirstCol<T>(p_sql, p_params);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键id，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static TEntity GetByID<TEntity>(object p_id)
            where TEntity : Entity
        {
            var pkName = SqliteConnectionEx.GetMapping(typeof(TEntity)).PK.Name;
            return _db.ForEach<TEntity>(
                $"select * from `{typeof(TEntity).Name}` where {pkName}=@id", new { id = p_id })
                .FirstOrDefault();
        }

        /// <summary>
        /// 获取表的主键名称
        /// </summary>
        /// <param name="p_type">实体类型</param>
        /// <returns></returns>
        public static string GetPrimaryKey(Type p_type)
        {
            return SqliteConnectionEx.GetMapping(p_type).PK.Name;
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
            return _db.Execute(p_sql, p_params);
        }

        /// <summary>
        /// 一个事务内批量执行SQL语句，如单表的批量插入
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_list">参数值列表</param>
        /// <returns>返回执行后影响的行数</returns>
        public static int BatchExec(string p_sql, List<Dict> p_list)
        {
            return _db.BatchExecute(p_sql, p_list);
        }

        /// <summary>
        /// 获取库中的所有表名
        /// </summary>
        /// <returns></returns>
        public static Table GetAllTables()
        {
            return _db.QueryTblsName();
        }
        #endregion

        #region 库管理
        /// <summary>
        /// 打开Sqlite库，自动创建、同步库表结构
        /// </summary>
        public static bool OpenDb(string p_dbFile)
        {
            if (_db != null)
                return false;

            try
            {
                SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                _db = new SqliteConnectionEx("Data Source=" + p_dbFile);
                _db.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开sqlite库 [{p_dbFile}] 异常：\r\n{ex.Message}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭Sqlite库
        /// </summary>
        public static void CloseDb()
        {
            if (_db != null)
            {
                _db.Close();
                _db = null;
            }
        }

        /// <summary>
        /// 是否已打开当前Sqlite库
        /// </summary>
        public static bool IsOpened => _db != null;
        #endregion

        #region 成员变量
        const string _unchangedMsg = "没有需要保存的数据！";
        const string _saveError = "数据源不可为空！";
        protected static SqliteConnectionEx _db;
        #endregion
    }
}
