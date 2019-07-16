#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dapper;
using MySql.Data.MySqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// MySql数据库访问类，全部采用异步操作
    /// 基于开源项目 MySqlConnector 和 Dapper
    /// </summary>
    public class Db
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
        public Db() : this(null, true)
        { }

        /// <summary>
        /// 默认连接串 + 设置是否自动关闭连接
        /// </summary>
        /// <param name="p_autoClose">调用每个公共方法后是否自动关闭连接，false时切记最后手动关闭！</param>
        public Db(bool p_autoClose) : this(null, p_autoClose)
        { }

        /// <summary>
        /// 其他数据源连接串 + 设置是否自动关闭连接
        /// </summary>
        /// <param name="p_dbKey">数据源键名，在json配置DbList节</param>
        /// <param name="p_autoClose">调用每个公共方法后是否自动关闭连接，false时切记最后手动关闭！</param>
        public Db(string p_dbKey, bool p_autoClose = true)
        {
            _dbKey = p_dbKey;
            _autoClose = p_autoClose;
        }
        #endregion

        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public async Task<Table> Table(string p_keyOrSql, object p_params = null)
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, false);
            try
            {
                await OpenConnection();
                Table tbl = new Table();
                using (MySqlDataReader reader = (MySqlDataReader)await _conn.ExecuteReaderAsync(cmd))
                {
                    // 参见github上的MySqlDataReader.cs

                    // 获取列定义
                    var cols = reader.GetColumnSchema();
                    foreach (var col in cols)
                    {
                        tbl.Add(col.ColumnName, col.DataType);
                    }

                    while (await reader.ReadAsync())
                    {
                        // 整行已读到内存，官方推荐使用同步方法获取值，比异步性能更好！
                        Row row = new Row();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var col = cols[i];
                            if (reader.IsDBNull(i))
                                new Cell(row, col.ColumnName, col.DataType);
                            else
                                new Cell(col.ColumnName, reader.GetValue(i), row);
                        }
                        tbl.Add(row);
                    }
                }
                return tbl;
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
        /// 以参数值方式执行Sql语句，返回泛型列表
        /// </summary>
        /// <typeparam name="T">ORM类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回泛型列表</returns>
        public async Task<List<T>> List<T>(string p_keyOrSql, object p_params = null)
        {
            return (List<T>)await Query<T>(p_keyOrSql, p_params, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回泛型枚举，高性能
        /// </summary>
        /// <typeparam name="T">ORM类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回泛型枚举</returns>
        public Task<IEnumerable<T>> ForEach<T>(string p_keyOrSql, object p_params = null)
        {
            return Query<T>(p_keyOrSql, p_params, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回dynamic对象列表
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回dynamic对象列表</returns>
        public async Task<List<dynamic>> List(string p_keyOrSql, object p_params = null)
        {
            return (List<dynamic>)await Query(p_keyOrSql, p_params, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回dynamic对象枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回dynamic对象枚举</returns>
        public Task<IEnumerable<dynamic>> ForEach(string p_keyOrSql, object p_params = null)
        {
            return Query(p_keyOrSql, p_params, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回指定类型的对象列表
        /// </summary>
        /// <param name="p_type">ORM类型</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回指定类型的对象列表</returns>
        public async Task<List<object>> List(Type p_type, string p_keyOrSql, object p_params = null)
        {
            return (List<object>)await Query(p_type, p_keyOrSql, p_params, false);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回指定类型对象的枚举，高性能
        /// </summary>
        /// <param name="p_type">ORM类型</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回指定类型对象的枚举</returns>
        public Task<IEnumerable<object>> ForEach(Type p_type, string p_keyOrSql, object p_params = null)
        {
            return Query(p_type, p_keyOrSql, p_params, true);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <typeparam name="T">单元格数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public async Task<T> Scalar<T>(string p_keyOrSql, object p_params = null)
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
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <typeparam name="T">ORM类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行数据对象，无数据时返回空</returns>
        public Task<T> First<T>(string p_keyOrSql, object p_params = null)
        {
            return QueryFirstRow<T>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行数据的dynamic对象</returns>
        public Task<dynamic> First(string p_keyOrSql, object p_params = null)
        {
            return QueryFirstRow<dynamic>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public async Task<Row> FirstRow(string p_keyOrSql, object p_params = null)
        {
            string sql = $"select * from ({Glb.Sql(p_keyOrSql)}) a limit 1";
            var tbl = await Table(sql, p_params);
            return tbl.FirstOrDefault();
        }

        async Task<IEnumerable<T>> Query<T>(string p_keyOrSql, object p_params, bool p_deferred)
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
            try
            {
                await OpenConnection();
                var result = await _conn.QueryAsync<T>(cmd);
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

        async Task<IEnumerable<dynamic>> Query(string p_keyOrSql, object p_params, bool p_deferred)
        {
            var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
            try
            {
                await OpenConnection();
                var result = await _conn.QueryAsync(cmd);
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

        async Task<IEnumerable<object>> Query(Type p_type, string p_keyOrSql, object p_params, bool p_deferred)
        {
            if (p_type == null)
                throw new ArgumentNullException(nameof(p_type));

            var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
            try
            {
                await OpenConnection();
                var result = await _conn.QueryAsync(p_type, cmd);
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

        async Task<T> QueryFirstRow<T>(string p_keyOrSql, object p_params)
        {
            string sql = $"select * from ({Glb.Sql(p_keyOrSql)}) a limit 1";
            var cmd = CreateCommand(sql, p_params, false);
            try
            {
                await OpenConnection();
                var result = await _conn.QueryFirstOrDefaultAsync<T>(cmd);
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
            string sql = Glb.Sql(p_keyOrSql);

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
                _conn = new MySqlConnection(string.IsNullOrEmpty(_dbKey) ? DefaultConnStr : Glb.Config["DbList:" + _dbKey]);
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
            string sql = Glb.Sql(p_keyOrSql);
            if (TraceSql)
                Log.Information(BuildSql(sql, p_params));

            return new CommandDefinition(
                Glb.Sql(p_keyOrSql),
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
            DbType dbtype = DbTypeConverter.GetDbTypeByValue(p_value);
            bool isBinary = dbtype == DbType.Binary;
            // 时间类型和字符串类型同样处理
            bool isStr = dbtype == DbType.DateTime || dbtype == DbType.String;
            int posStart = str.IndexOf(p_key, StringComparison.CurrentCultureIgnoreCase);
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
                    posStart = str.IndexOf(p_key, posEnd, StringComparison.CurrentCultureIgnoreCase);
                    continue;
                }

                if (isBinary)
                    trueVal = "长度为" + (p_value as byte[]).Length + "的二进制字符数组";
                else
                    trueVal = isStr ? "'" + p_value as string + "'" : p_value.ToString();

                str = str.Substring(0, posStart) + trueVal + str.Substring(posStart + p_key.Length);
                posStart = str.IndexOf(p_key, posStart + trueVal.Length - 1, StringComparison.CurrentCultureIgnoreCase);
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