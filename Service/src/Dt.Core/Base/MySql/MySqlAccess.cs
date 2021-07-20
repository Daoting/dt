#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dapper;
using MySqlConnector;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// MySql数据库访问类，全部采用异步操作
    /// 基于开源项目 MySqlConnector 和 Dapper
    /// Dapper涉及dynamic的速度非常慢！
    /// </summary>
    public class MySqlAccess
    {
        #region 成员变量
        // 默认连接串
        internal static string DefaultConnStr;
        // 是否输出所有运行中的Sql语句
        public static bool TraceSql;
        static readonly Regex _sqlPattern = new Regex("[0-9a-zA-Z_$]");

        string _dbKey;
        bool _autoClose;
        MySqlConnection _conn;
        MySqlTransaction _tran;
        #endregion

        #region 构造方法
        /// <summary>
        /// 默认连接串 + 自动关闭连接
        /// </summary>
        public MySqlAccess() : this(null, true)
        { }

        /// <summary>
        /// 默认连接串 + 设置是否自动关闭连接
        /// </summary>
        /// <param name="p_autoClose">调用每个公共方法后是否自动关闭连接，false时切记最后手动关闭！</param>
        public MySqlAccess(bool p_autoClose) : this(null, p_autoClose)
        { }

        /// <summary>
        /// 其他数据源连接串 + 设置是否自动关闭连接
        /// </summary>
        /// <param name="p_dbKey">数据源键名，在global.json的MySql节</param>
        /// <param name="p_autoClose">调用每个公共方法后是否自动关闭连接，false时切记最后手动关闭！</param>
        public MySqlAccess(string p_dbKey, bool p_autoClose = true)
        {
            _dbKey = p_dbKey;
            _autoClose = p_autoClose;
        }
        #endregion

        #region Table查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public async Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            var tbl = new Table();
            await QueryInternal<Row>(tbl, p_keyOrSql, p_params);
            return tbl;
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public async Task<Table<TEntity>> Query<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            var tbl = new Table<TEntity>();
            await QueryInternal<TEntity>(tbl, p_keyOrSql, p_params);
            return tbl;
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public Task<IEnumerable<Row>> Each(string p_keyOrSql, object p_params = null)
        {
            return ForEachRow<Row>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public Task<IEnumerable<TEntity>> Each<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return ForEachRow<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public async Task<Row> First(string p_keyOrSql, object p_params = null)
        {
            return (await ForEachRow<Row>(p_keyOrSql, p_params)).FirstOrDefault();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public async Task<TEntity> First<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return (await ForEachRow<TEntity>(p_keyOrSql, p_params)).FirstOrDefault();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <typeparam name="T">单元格数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public async Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, false);
            try
            {
                await OpenConnection();
                var result = await _conn.ExecuteScalarAsync<T>(cmd);
                return result;
            }
            catch (Exception ex)
            {
                throw GetSqlException(cmd, ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public async Task<List<T>> FirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            return (List<T>)await FirstCol(typeof(T), p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <param name="p_type">第一列数据类型</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public async Task<object> FirstCol(Type p_type, string p_keyOrSql, object p_params = null)
        {
            Throw.IfNull(p_type);
            var cmd = CreateCommand(p_keyOrSql, p_params, false);
            try
            {
                await OpenConnection();
                using (var wrappedReader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd))
                {
                    // Dapper2.0 改版
                    MySqlDataReader reader = (MySqlDataReader)wrappedReader.Reader;

                    Type tp = typeof(List<>).MakeGenericType(p_type);
                    var ls = Activator.CreateInstance(tp) as IList;
                    var cols = reader.GetColumnSchema();
                    if (cols[0].DataType == p_type)
                    {
                        while (await reader.ReadAsync())
                        {
                            ls.Add(reader.GetValue(0));
                        }
                    }
                    else
                    {
                        while (await reader.ReadAsync())
                        {
                            ls.Add(Convert.ChangeType(reader.GetValue(0), p_type));
                        }
                    }
                    return ls;
                }
            }
            catch (Exception ex)
            {
                throw GetSqlException(cmd, ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一列枚举，高性能
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回泛型枚举</returns>
        public async Task<IEnumerable<T>> EachFirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, true);
            await OpenConnection();
            var reader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd);
            return ForEachFirstCol<T>(reader);
        }

        async Task QueryInternal<TRow>(Table p_tbl, string p_keyOrSql, object p_params = null)
            where TRow : Row
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, false);
            try
            {
                await OpenConnection();
                using (var wrappedReader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd))
                {
                    // Dapper2.0 改版
                    MySqlDataReader reader = (MySqlDataReader)wrappedReader.Reader;

                    // Entity类型
                    Type tpEntity = null;
                    if (typeof(TRow).IsSubclassOf(typeof(Entity)))
                    {
                        tpEntity = typeof(TRow);
                    }

                    // 参见github上的MySqlDataReader.cs
                    // 获取列定义
                    var cols = reader.GetColumnSchema();
                    foreach (var col in cols)
                    {
                        if (col.AllowDBNull.HasValue && col.AllowDBNull.Value && col.DataType.IsValueType)
                        {
                            // 可为null的值类型
                            p_tbl.Add(col.ColumnName, typeof(Nullable<>).MakeGenericType(col.DataType));
                        }
                        else if (col.DataType == typeof(byte) && tpEntity != null)
                        {
                            // Entity 时根据属性类型将 byte 自动转为 enum 类型
                            var prop = tpEntity.GetProperty(col.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                            p_tbl.Add(col.ColumnName, prop != null ? prop.PropertyType : col.DataType);
                        }
                        else
                        {
                            p_tbl.Add(col.ColumnName, col.DataType);
                        }
                    }

                    while (await reader.ReadAsync())
                    {
                        // 整行已读到内存，官方推荐使用同步方法获取值，比异步性能更好！
                        // 无参数构造方法可能为private，如实体类型
                        var row = (TRow)Activator.CreateInstance(typeof(TRow), true);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var col = p_tbl.Columns[i];
                            if (reader.IsDBNull(i))
                                new Cell(row, col.ID, col.Type);
                            else
                                new Cell(row, col.ID, col.Type, reader.GetValue(i));
                        }
                        p_tbl.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                throw GetSqlException(cmd, ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        async Task<IEnumerable<TRow>> ForEachRow<TRow>(string p_keyOrSql, object p_params = null)
            where TRow : Row
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, true);
            await OpenConnection();
            var reader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd);
            return ForEachRow<TRow>(reader);
        }

        IEnumerable<TRow> ForEachRow<TRow>(IWrappedDataReader p_wrappedReader)
            where TRow : Row
        {
            // yield无法使用await，无法在含catch的try内
            // 一定要使用using 或 finally方式释放资源，不然foreach内部break时资源无法释放！！！
            try
            {
                using (p_wrappedReader)
                {
                    MySqlDataReader reader = (MySqlDataReader)p_wrappedReader.Reader;
                    var cols = reader.GetColumnSchema();

                    // Entity类型
                    Type tpEntity = null;
                    if (typeof(TRow).IsSubclassOf(typeof(Entity)))
                    {
                        tpEntity = typeof(TRow);
                    }

                    while (reader.Read())
                    {
                        // 无参数构造方法可能为private，如实体类型
                        TRow row = (TRow)Activator.CreateInstance(typeof(TRow), true);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var col = cols[i];

                            Type colType = col.DataType;
                            if (col.AllowDBNull.HasValue && col.AllowDBNull.Value && col.DataType.IsValueType)
                            {
                                // 可为null的值类型
                                colType = typeof(Nullable<>).MakeGenericType(col.DataType);
                            }
                            else if (colType == typeof(byte) && tpEntity != null)
                            {
                                // Entity 时根据属性类型将 byte 自动转为 enum 类型
                                var prop = tpEntity.GetProperty(col.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                                if (prop != null)
                                    colType = prop.PropertyType;
                            }

                            if (reader.IsDBNull(i))
                                new Cell(row, col.ColumnName, colType);
                            else
                                new Cell(row, col.ColumnName, colType, reader.GetValue(i));
                        }
                        yield return row;
                    }
                }
            }
            finally
            {
                ReleaseConnection();
            }
        }

        IEnumerable<T> ForEachFirstCol<T>(IWrappedDataReader p_wrappedReader)
        {
            // yield无法使用await，无法在含catch的try内
            // 一定要使用using 或 finally方式释放资源，不然foreach内部break时资源无法释放！！！
            try
            {
                using (p_wrappedReader)
                {
                    MySqlDataReader reader = (MySqlDataReader)p_wrappedReader.Reader;
                    var cols = reader.GetColumnSchema();
                    if (cols[0].DataType == typeof(T))
                    {
                        while (reader.Read())
                        {
                            yield return reader.GetFieldValue<T>(0);
                        }
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            yield return (T)Convert.ChangeType(reader.GetValue(0), typeof(T));
                        }
                    }
                }
            }
            finally
            {
                ReleaseConnection();
            }
        }
        #endregion

        #region 强类型查询
        /* 
         * Dapper涉及dynamic的速度非常慢！已移除
         * 
         * Dapper的ORM映射优先级：
         * 1. 存在和字段名称相同的属性，且属性含有setter，setter可以为private
         * 2. 存在和字段名称相同的变量(field)
         * 3. 存在和字段名称相同的属性，但无setter，直接设置到自动变量(Backing Field)
         * 4. 删除字段名称中的_ ，按上述方法比较是否有映射属性
         */

        /// <summary>
        /// 以参数值方式执行Sql语句，返回泛型列表
        /// </summary>
        /// <typeparam name="T">ORM类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回泛型列表</returns>
        public async Task<List<T>> GetList<T>(string p_keyOrSql, object p_params = null)
        {
            return (List<T>)await GetListInternal<T>(p_keyOrSql, p_params, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回泛型枚举，高性能
        /// </summary>
        /// <typeparam name="T">ORM类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回泛型枚举</returns>
        public Task<IEnumerable<T>> EachItem<T>(string p_keyOrSql, object p_params = null)
        {
            return GetListInternal<T>(p_keyOrSql, p_params, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回指定类型的对象列表，可直接转型为List`T
        /// </summary>
        /// <param name="p_type">ORM类型</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回指定类型的对象列表</returns>
        public Task<System.Collections.IEnumerable> GetList(Type p_type, string p_keyOrSql, object p_params = null)
        {
            return GetListInternal(p_type, p_keyOrSql, p_params, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回指定类型对象的枚举，高性能
        /// </summary>
        /// <param name="p_type">ORM类型</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回指定类型对象的枚举</returns>
        public Task<System.Collections.IEnumerable> EachItem(Type p_type, string p_keyOrSql, object p_params = null)
        {
            return GetListInternal(p_type, p_keyOrSql, p_params, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <typeparam name="T">ORM类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行数据对象，无数据时返回空</returns>
        public async Task<T> GetItem<T>(string p_keyOrSql, object p_params = null)
        {
            return (T)await GetItem(typeof(T), p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_tgtType">ORM类型</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行数据对象，无数据时返回空</returns>
        public async Task<object> GetItem(Type p_tgtType, string p_keyOrSql, object p_params = null)
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, false);
            try
            {
                await OpenConnection();
                return await _conn.QueryFirstOrDefaultAsync(p_tgtType, cmd);
            }
            catch (Exception ex)
            {
                throw GetSqlException(cmd, ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        async Task<IEnumerable<T>> GetListInternal<T>(string p_keyOrSql, object p_params, bool p_deferred)
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
            try
            {
                await OpenConnection();
                return await _conn.QueryAsync<T>(cmd);
            }
            catch (Exception ex)
            {
                throw GetSqlException(cmd, ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        // 泛型方法 QueryAsync<T>
        static MethodInfo _queryMethod = typeof(SqlMapper).GetMethod("QueryAsync", 1, new Type[] { typeof(IDbConnection), typeof(CommandDefinition) });
        async Task<System.Collections.IEnumerable> GetListInternal(Type p_type, string p_keyOrSql, object p_params, bool p_deferred)
        {
            if (p_type == null)
                throw new ArgumentNullException(nameof(p_type));

            var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
            try
            {
                await OpenConnection();
                // 实例化泛型方法 QueryAsync<T>，若调用 QueryAsync 返回 IEnumerable<object>，外部还需要类型转换！
                Task task = _queryMethod.MakeGenericMethod(p_type).Invoke(null, new object[] { _conn, cmd }) as Task;
                await task;
                return task.GetType().GetProperty("Result").GetValue(task) as System.Collections.IEnumerable;
                //return await _conn.QueryAsync(p_type, cmd);
            }
            catch (Exception ex)
            {
                throw GetSqlException(cmd, ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }
        #endregion

        #region 多映射查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<TReturn>> Map<TFirst, TSecond, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return (List<TReturn>)await QueryMap<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象枚举</returns>
        public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return QueryMap<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象枚举</returns>
        public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return QueryMap<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象枚举</returns>
        public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return QueryMap<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象枚举</returns>
        public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象枚举</returns>
        public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象枚举</returns>
        public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_types">返回的多个类型</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<TReturn>> Map<TReturn>(string p_keyOrSql, Type[] p_types, Func<object[], TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return (List<TReturn>)await QueryMap(p_keyOrSql, p_types, p_map, p_params, false, p_splitOn);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_types">返回的多个类型</param>
        /// <param name="p_map">合并结果的回调方法</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
        /// <returns>返回指定类型的对象枚举</returns>
        public Task<IEnumerable<TReturn>> ForEach<TReturn>(string p_keyOrSql, Type[] p_types, Func<object[], TReturn> p_map, object p_params = null, string p_splitOn = "id")
        {
            return QueryMap(p_keyOrSql, p_types, p_map, p_params, true, p_splitOn);
        }

        async Task<IEnumerable<TReturn>> QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            string p_keyOrSql,
            Delegate p_map,
            object p_params,
            string p_splitOn,
            bool p_deferred)
        {
            if (p_map == null)
                throw new ArgumentNullException(nameof(p_map));

            var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
            try
            {
                await OpenConnection();
                IEnumerable<TReturn> result = null;
                if (typeof(TThird) == typeof(DontMap))
                    result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TReturn>)p_map, p_splitOn);
                else if (typeof(TFourth) == typeof(DontMap))
                    result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TReturn>)p_map, p_splitOn);
                else if (typeof(TFifth) == typeof(DontMap))
                    result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TReturn>)p_map, p_splitOn);
                else if (typeof(TSixth) == typeof(DontMap))
                    result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>)p_map, p_splitOn);
                else if (typeof(TSeventh) == typeof(DontMap))
                    result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>)p_map, p_splitOn);
                else
                    result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>)p_map, p_splitOn);
                return result;
            }
            catch (Exception ex)
            {
                throw GetSqlException(cmd, ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        async Task<IEnumerable<TReturn>> QueryMap<TReturn>(
            string p_keyOrSql,
            Type[] p_types,
            Func<object[], TReturn> p_map,
            object p_params,
            bool p_deferred,
            string p_splitOn)
        {
            string sql = Kit.Sql(p_keyOrSql);

            try
            {
                await OpenConnection();
                var result = await _conn.QueryAsync(sql, p_types, p_map, p_params, _tran, !p_deferred, p_splitOn);
                return result;
            }
            catch (Exception ex)
            {
                throw GetSqlException(CreateCommand(p_keyOrSql, p_params, p_deferred), ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        class DontMap { }
        #endregion

        #region 增删改
        /// <summary>
        /// 以参数值方式执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <param name="p_beginTrans">是否启动事务，默认false</param>
        /// <returns>执行后影响的行数</returns>
        public async Task<int> Exec(string p_keyOrSql, object p_params = null, bool p_beginTrans = false)
        {
            try
            {
                await OpenConnection();
                if (p_beginTrans)
                    await BeginTrans();

                var cmd = CreateCommand(p_keyOrSql, p_params, false);
                var result = await _conn.ExecuteAsync(cmd);

                if (p_beginTrans)
                    await CommitTrans();
                return result;
            }
            catch (Exception ex)
            {
                await RollbackTrans();
                throw GetSqlException(CreateCommand(p_keyOrSql, p_params, false), ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }
        #endregion

        #region 手动关闭
        /// <summary>
        /// _autoClose为false时需要手动关闭连接，
        /// </summary>
        /// <param name="p_commitTrans">若有事务，true表提交，false表回滚</param>
        public async Task Close(bool p_commitTrans)
        {
            if (_tran != null)
            {
                if (p_commitTrans)
                    await _tran.CommitAsync();
                else
                    await _tran.RollbackAsync();
                _tran = null;
            }

            if (_conn != null)
            {
                _conn.Close();
                _conn = null;
            }
        }
        #endregion

        #region 事务
        /// <summary>
        /// 开始事务，打开连接
        /// </summary>
        public async Task BeginTrans()
        {
            if (_tran == null)
            {
                await OpenConnection();
                _tran = await _conn.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// 提交事务，释放连接
        /// </summary>
        public async Task CommitTrans()
        {
            if (_tran != null)
            {
                await _tran.CommitAsync();
                _tran = null;
                ReleaseConnection();
            }
        }

        /// <summary>
        /// 回滚事务，释放连接
        /// </summary>
        public async Task RollbackTrans()
        {
            if (_tran != null)
            {
                await _tran.RollbackAsync();
                _tran = null;
                ReleaseConnection();
            }
        }
        #endregion

        #region 打开/关闭连接
        /// <summary>
        /// 打开数据库连接，若已打开(嵌套在事务)无操作
        /// </summary>
        /// <returns></returns>
        async Task OpenConnection()
        {
            if (_conn == null)
            {
                // 数据源键名在json配置中
                _conn = new MySqlConnection(string.IsNullOrEmpty(_dbKey) ? DefaultConnStr : Kit.Config["MySql:" + _dbKey]);
                await _conn.OpenAsync();
            }
        }

        /// <summary>
        /// 关闭数据库连接（未启动事务的前提下）
        /// </summary>
        void ReleaseConnection()
        {
            if (_autoClose && _tran == null && _conn != null)
            {
                _conn.Close();
                _conn = null;
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 创建Dapper的命令定义
        /// </summary>
        /// <param name="p_keyOrSql"></param>
        /// <param name="p_params"></param>
        /// <param name="p_deferred"></param>
        /// <returns></returns>
        CommandDefinition CreateCommand(string p_keyOrSql, object p_params, bool p_deferred)
        {
            string sql = Kit.Sql(p_keyOrSql);
            if (TraceSql)
                Log.Information(BuildSql(sql, p_params));

            return new CommandDefinition(
                Kit.Sql(p_keyOrSql),
                p_params,
                _tran,
                null,
                null,
                p_deferred ? CommandFlags.Pipelined : CommandFlags.Buffered,
                default(CancellationToken));
        }

        /// <summary>
        /// 生成执行时候的sql语句。
        /// 注意：对于二进制编码数组的情况在实际应用时可能需要调整。
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        string BuildSql(string p_sql, object p_params)
        {
            if (p_params == null)
                return p_sql;

            string str = p_sql;
            try
            {
                if (p_params is IDictionary<string, object> dt)
                {
                    // 键/值型对象
                    foreach (var item in dt)
                    {
                        str = ReplaceSql(str, "@" + item.Key, item.Value);
                    }
                }
                else
                {
                    // 普通对象
                    Type type = p_params.GetType();
                    foreach (var prop in type.GetProperties())
                    {
                        str = ReplaceSql(str, "@" + prop.Name, prop.GetValue(p_params));
                    }
                }
            }
            catch { }
            return str;
        }

        /// <summary>
        /// 替换sql中的占位符
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_key">占位符</param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        string ReplaceSql(string p_sql, string p_key, object p_value)
        {
            string str = p_sql;
            int posStart = str.IndexOf(p_key, StringComparison.OrdinalIgnoreCase);
            while (posStart > -1)
            {
                string next, trueVal;
                int posEnd = posStart + p_key.Length;
                if (posEnd >= str.Length)
                    next = "";
                else
                    next = str.Substring(posEnd, 1);

                // 判断参数名的匹配是不是只匹配了一部分
                if (!string.IsNullOrEmpty(next) && _sqlPattern.IsMatch(next))
                {
                    // 匹配了一部分则继续向后查找
                    posStart = str.IndexOf(p_key, posEnd, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                // mysql中非string类型外面加''也可正常运行！
                if (p_value == null)
                    trueVal = "null";
                else
                    trueVal = $"'{p_value}'";

                str = str.Substring(0, posStart) + trueVal + str.Substring(posStart + p_key.Length);
                posStart = str.IndexOf(p_key, posStart + trueVal.Length - 1, StringComparison.OrdinalIgnoreCase);
            }
            return str;
        }

        /// <summary>
        /// 输出异常信息
        /// </summary>
        /// <param name="p_cmd"></param>
        /// <param name="p_ex"></param>
        /// <returns></returns>
        Exception GetSqlException(CommandDefinition p_cmd, Exception p_ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("【Sql命令】");
            sb.AppendLine(p_cmd.CommandText);
            if (p_cmd.Parameters == null)
            {
                sb.AppendLine("【无参数】");
            }
            else
            {
                sb.AppendLine("【参数】");
                try
                {
                    if (p_cmd.Parameters is IDictionary<string, object> dt)
                    {
                        foreach (var val in dt)
                        {
                            sb.Append(val.Key);
                            sb.Append("：");
                            sb.AppendLine(val.Value == null ? "null" : val.Value.ToString());
                        }
                    }
                    else
                    {
                        // 普通对象
                        Type type = p_cmd.Parameters.GetType();
                        foreach (var prop in type.GetProperties())
                        {
                            sb.Append(prop.Name);
                            sb.Append("：");
                            object val = prop.GetValue(p_cmd.Parameters);
                            sb.AppendLine(val == null ? "null" : val.ToString());
                        }
                    }
                }
                catch { }
            }
            sb.AppendLine("【异常内容】");
            sb.AppendLine(p_ex.Message);
            return new Exception(sb.ToString());
        }
        #endregion
    }
}