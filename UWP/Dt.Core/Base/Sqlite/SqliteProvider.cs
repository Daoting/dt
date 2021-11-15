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

        #region 保存
        /// <summary>
        /// 保存实体数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entity == null
                || (!p_entity.IsAdded && !p_entity.IsChanged))
            {
                if (p_isNotify)
                    Kit.Warn(_unchangedMsg);
                return false;
            }

            var model = SqliteEntitySchema.Get(typeof(TEntity));
            if (model.OnSaving != null)
            {
                // 保存前外部校验，不合格在外部抛出异常
                if (!await OnSaving(model, p_entity))
                    return false;
            }

            if (_db.Save(p_entity))
            {
                if (p_isNotify)
                    Kit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                Kit.Warn("保存失败！");
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
        public static async Task<bool> BatchSave(IList p_list, bool p_isNotify = true)
        {
            if (p_list == null || p_list.Count == 0)
            {
                if (p_isNotify)
                    Kit.Warn(_unchangedMsg);
                return false;
            }

            // 触发外部保存前处理
            Type tp = p_list.GetType();
            if (tp.IsGenericType
                && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
            {
                // 单表增删改，列表中的实体类型相同

                // 不判断列表项数0，因可能Table<Entity>只包含删除列表的情况！
                // IList<Entity> 或 Table<Entity>
                var model = SqliteEntitySchema.Get(tp.GetGenericArguments()[0]);
                if (model.OnSaving != null)
                {
                    foreach (var ci in p_list)
                    {
                        if (!await OnSaving(model, ci))
                            return false;
                    }
                }
            }
            else
            {
                // 多表增删改
                foreach (var item in p_list)
                {
                    if (item is Entity entity)
                    {
                        if (entity.IsAdded || entity.IsChanged)
                        {
                            var model = SqliteEntitySchema.Get(item.GetType());
                            if (model.OnSaving != null)
                            {
                                if (!await OnSaving(model, entity))
                                    return false;
                            }
                        }
                    }
                    else if (item is IList clist
                        && (tp = item.GetType()) != null
                        && tp.IsGenericType
                        && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                    {
                        // 不判断列表项数0，因可能Table<Entity>只包含删除列表的情况！
                        // IList<Entity> 或 Table<Entity>
                        var model = SqliteEntitySchema.Get(tp.GetGenericArguments()[0]);
                        if (model.OnSaving != null)
                        {
                            foreach (var ci in clist)
                            {
                                if (!await OnSaving(model, ci))
                                    return false;
                            }
                        }
                    }
                    //else
                    //{
                    //    throw new Exception($"批量保存不支持[{item.GetType().Name}]类型！");
                    //}
                }
            }

            if (_db.BatchSave(p_list))
            {
                if (p_isNotify)
                    Kit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                Kit.Warn("保存失败！");
            return false;
        }

        /// <summary>
        /// 保存前外部校验，不合格在外部抛出异常
        /// </summary>
        /// <param name="p_model"></param>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        static async Task<bool> OnSaving(SqliteEntitySchema p_model, object p_entity)
        {
            try
            {
                if (p_model.OnSaving.ReturnType == typeof(Task))
                    await (Task)p_model.OnSaving.Invoke(p_entity, null);
                else
                    p_model.OnSaving.Invoke(p_entity, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is KnownException kex)
                    Kit.Warn(kex.Message);
                else
                    Kit.Warn(ex.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的行</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entity == null || p_entity.IsAdded)
            {
                if (p_isNotify)
                    Kit.Warn(_saveError);
                return false;
            }

            var model = SqliteEntitySchema.Get(typeof(TEntity));
            if (model.OnDeleting != null)
            {
                if (!await OnDeleting(model, p_entity))
                    return false;
            }

            bool suc = _db.Delete(p_entity);
            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
            return suc;
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
        public static async Task<bool> BatchDelete(IList p_list, bool p_isNotify = true)
        {
            if (p_list == null || p_list.Count == 0)
            {
                if (p_isNotify)
                    Kit.Warn(_saveError);
                return false;
            }

            // 触发外部删除前处理
            Type tp = p_list.GetType();
            if (tp.IsGenericType
                && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
            {
                // 列表中的实体类型相同
                var model = SqliteEntitySchema.Get(tp.GetGenericArguments()[0]);
                if (model.OnDeleting != null)
                {
                    foreach (var ci in p_list)
                    {
                        if (!await OnDeleting(model, ci))
                            return false;
                    }
                }
            }
            else
            {
                // 多类型
                foreach (var item in p_list)
                {
                    if (item is Entity entity)
                    {
                        var model = SqliteEntitySchema.Get(item.GetType());
                        if (model.OnDeleting != null)
                        {
                            if (!await OnDeleting(model, entity))
                                return false;
                        }
                    }
                    else if (item is IList clist
                        && clist.Count > 0
                        && (tp = item.GetType()) != null
                        && tp.IsGenericType
                        && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                    {
                        // IList<Entity> 或 Table<Entity>
                        var model = SqliteEntitySchema.Get(tp.GetGenericArguments()[0]);
                        if (model.OnDeleting != null)
                        {
                            foreach (var ci in clist)
                            {
                                if (!await OnDeleting(model, ci))
                                    return false;
                            }
                        }
                    }
                }
            }

            if (_db.BatchDelete(p_list))
            {
                if (p_isNotify)
                    Kit.Msg("删除成功！");
                return true;
            }

            if (p_isNotify)
                Kit.Warn("删除失败！");
            return false;
        }

        /// <summary>
        /// 根据主键值删除实体对象，仅支持单主键，主键列名内部确定
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键值</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static bool DelByID<TEntity>(object p_id, bool p_isNotify = true)
            where TEntity : Entity
        {
            bool suc = _db.DelByPK<TEntity>(p_id);
            if (p_isNotify)
                Kit.Msg(suc ? "删除成功！" : "删除失败！");
            return suc;
        }

        /// <summary>
        /// 删除前外部校验，不合格在外部抛出异常
        /// </summary>
        /// <param name="p_model"></param>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        static async Task<bool> OnDeleting(SqliteEntitySchema p_model, object p_entity)
        {
            try
            {
                if (p_model.OnDeleting.ReturnType == typeof(Task))
                    await (Task)p_model.OnDeleting.Invoke(p_entity, null);
                else
                    p_model.OnDeleting.Invoke(p_entity, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is KnownException kex)
                    Kit.Warn(kex.Message);
                else
                    Kit.Warn(ex.Message);
                return false;
            }
            return true;
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
        public static void OpenDb()
        {
            if (_db != null)
                return;

            var dbName = GetDbName();
            try
            {
                var path = Path.Combine(Kit.DataPath, dbName + ".db");
                bool exists = File.Exists(path);
                _db = new SqliteConnectionEx("Data Source=" + path);
                _db.Open();

                // 初次运行、库表结构版本变化或文件被删除时创建库表结构
                if (Kit.Stub.SqliteDb.TryGetValue(dbName, out var dbInfo))
                {
                    path = Path.Combine(Kit.DataPath, $"{dbName}-{dbInfo.Version}.ver");
                    if (!exists || !File.Exists(path))
                    {
                        _db.InitDb(dbInfo.Tables);

                        // 删除旧版本号文件
                        foreach (var file in new DirectoryInfo(Kit.DataPath).GetFiles($"{dbName}-*.ver"))
                        {
                            try { file.Delete(); } catch { }
                        }

                        // 创建空文件，文件名是库表结构的版本号
                        File.Create(path);
                    }
                }

                SqliteDbs.All[dbName] = _db;
            }
            catch (Exception ex)
            {
                throw new Exception($"打开sqlite库[{dbName}]异常，请重新启动应用！{ex.Message}");
            }
        }

        /// <summary>
        /// 打开Sqlite库，提供给后台任务使用，不自动创建、同步库表结构
        /// </summary>
        public static void OpenDbBackground()
        {
            if (_db != null)
                return;

            var dbName = GetDbName();
            try
            {
                var path = Path.Combine(Kit.DataPath, dbName + ".db");
                bool exists = File.Exists(path);
                _db = new SqliteConnectionEx("Data Source=" + path);
                _db.Open();

                SqliteDbs.All[dbName] = _db;
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
            if (_db != null)
            {
                _db.Close();
                _db = null;
                SqliteDbs.All.Remove(GetDbName());
            }
        }

        /// <summary>
        /// 是否已打开当前Sqlite库
        /// </summary>
        public static bool IsOpened => _db != null;

        protected static string GetDbName()
        {
            return typeof(Sqlite_name).Name.Substring(7);
        }
        #endregion

        #region 成员变量
        const string _unchangedMsg = "没有需要保存的数据！";
        const string _saveError = "数据源不可为空！";
        protected static SqliteConnectionEx _db;
        #endregion
    }

    /// <summary>
    /// 管理所有已打开的Sqlite库，不可合并到SqliteProvider！
    /// </summary>
    public static class SqliteDbs
    {
        /// <summary>
        /// 所有已打开的Sqlite库
        /// </summary>
        internal static readonly Dictionary<string, SqliteConnectionEx> All = new Dictionary<string, SqliteConnectionEx>();

        /// <summary>
        /// 获取已打开的Sqlite库
        /// </summary>
        /// <param name="p_dbName">库名</param>
        /// <returns></returns>
        public static SqliteConnectionEx GetDb(string p_dbName)
        {
            if (All.TryGetValue(p_dbName, out var db))
                return db;
            return null;
        }

        /// <summary>
        /// 获取所有已打开库的描述信息
        /// </summary>
        /// <returns></returns>
        public static Table GetAllDbInfo()
        {
            var tbl = new Table { { "name" }, { "info" } };
            foreach (var item in All)
            {
                int cnt = item.Value.GetScalar<int>("select count(*) from sqlite_master where type='table'");
                FileInfo fi = new FileInfo(Path.Combine(Kit.DataPath, item.Key + ".db"));
                tbl.AddRow(new { name = item.Key, info = $"{cnt - 1}张表，{Kit.GetFileSizeDesc((ulong)fi.Length)}" });
            }
            return tbl;
        }
    }
}
