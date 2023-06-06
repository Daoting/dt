#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dapper;
using System.Collections;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据库访问抽象基类，全部采用异步操作
    /// 基于开源项目 Dapper
    /// Dapper涉及dynamic的速度非常慢！已废弃
    /// </summary>
    abstract class DataAccess : IDataAccess
    {
        #region 成员变量
        protected DbConnection _conn;
        protected DbTransaction _tran;
        #endregion

        #region 构造方法
        public DataAccess(DbInfo p_info)
        {
            DbInfo = p_info;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 调用每个公共方法后是否自动关闭连接，默认true，false时切记最后手动关闭！
        /// </summary>
        public bool AutoClose { get; set; } = true;

        /// <summary>
        /// 数据库描述信息
        /// </summary>
        public DbInfo DbInfo { get; }
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
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public Task<Table> Page(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return Query(GetPageSql(p_starRow, p_pageSize, p_keyOrSql), p_params);
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return Query<TEntity>(GetPageSql(p_starRow, p_pageSize, p_keyOrSql), p_params);
        }

        /// <summary>
        /// 构造按页查询sql
        /// </summary>
        /// <param name="p_starRow"></param>
        /// <param name="p_pageSize"></param>
        /// <param name="p_keyOrSql"></param>
        /// <returns></returns>
        protected abstract string GetPageSql(int p_starRow, int p_pageSize, string p_keyOrSql);

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
                    var reader = (DbDataReader)wrappedReader.Reader;

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
                    var reader = (DbDataReader)wrappedReader.Reader;

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
                    var reader = (DbDataReader)p_wrappedReader.Reader;
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
                    var reader = (DbDataReader)p_wrappedReader.Reader;
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

        #region 增删改
        /// <summary>
        /// 以参数值方式执行Sql语句，返回影响的行数
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象</param>
        /// <returns>执行后影响的行数</returns>
        public async Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            try
            {
                await OpenConnection();
                var cmd = CreateCommand(p_keyOrSql, p_params, false);
                var result = await _conn.ExecuteAsync(cmd);
                return result;
            }
            catch (Exception ex)
            {
                throw GetSqlException(CreateCommand(p_keyOrSql, p_params, false), ex);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public async Task<int> BatchExec(List<Dict> p_dts)
        {
            if (p_dts == null || p_dts.Count == 0)
                return 0;

            try
            {
                await BeginTrans();
                int cnt = 0;
                foreach (Dict dt in p_dts)
                {
                    string sql = (string)dt["text"];
                    if (dt["params"] is List<Dict> ls)
                    {
                        foreach (var par in ls)
                        {
                            cnt += await Exec(sql, par);
                        }
                    }
                    else if (dt["params"] is Dict par)
                    {
                        cnt += await Exec(sql, par);
                    }
                }
                await CommitTrans();
                return cnt;
            }
            catch
            {
                await RollbackTrans();
                throw;
            }
            finally
            {
                ReleaseConnection();
            }
        }
        #endregion

        #region 手动关闭
        /// <summary>
        /// AutoClose为false时需要手动关闭连接，
        /// </summary>
        /// <param name="p_commitTrans">若有事务，true表提交，false表回滚</param>
        /// <returns></returns>
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
                _tran = _conn.BeginTransaction();
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
        protected async Task OpenConnection()
        {
            if (_conn == null)
            {
                _conn = CreateConnection();
                await _conn.OpenAsync();
            }
        }

        /// <summary>
        /// 关闭数据库连接（未启动事务的前提下）
        /// </summary>
        protected void ReleaseConnection()
        {
            if (AutoClose && _tran == null && _conn != null)
            {
                _conn.Close();
                _conn = null;
            }
        }

        protected abstract DbConnection CreateConnection();
        #endregion

        #region 表结构
        /// <summary>
        /// 获取数据库所有表结构信息（已调整到最优）
        /// </summary>
        /// <returns>返回加载结果信息</returns>
        public abstract IReadOnlyDictionary<string, TableSchema> GetDbSchema();

        /// <summary>
        /// 获取数据库的所有表名
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetAllTableNames();

        /// <summary>
        /// 获取单个表结构信息
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public abstract TableSchema GetTableSchema(string p_tblName);

        /// <summary>
        /// 数据库中是否存在指定的表
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public abstract Task<bool> ExistTable(string p_tblName);
        #endregion

        #region 内部方法
        /// <summary>
        /// 创建Dapper的命令定义
        /// </summary>
        /// <param name="p_keyOrSql"></param>
        /// <param name="p_params"></param>
        /// <param name="p_deferred"></param>
        /// <returns></returns>
        protected CommandDefinition CreateCommand(string p_keyOrSql, object p_params, bool p_deferred)
        {
            string sql = GetSql(p_keyOrSql);
            if (Kit.TraceSql)
                Log.Information(BuildSql(sql, p_params));

            return new CommandDefinition(
                sql,
                p_params,
                _tran,
                null,
                null,
                p_deferred ? CommandFlags.Pipelined : CommandFlags.Buffered,
                default(CancellationToken));
        }

        /// <summary>
        /// 输出异常信息
        /// </summary>
        /// <param name="p_cmd"></param>
        /// <param name="p_ex"></param>
        /// <returns></returns>
        protected Exception GetSqlException(CommandDefinition p_cmd, Exception p_ex)
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

        /// <summary>
        /// 生成执行时候的sql语句。
        /// 注意：对于二进制编码数组的情况在实际应用时可能需要调整。
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        static string BuildSql(string p_sql, object p_params)
        {
            if (p_params == null)
                return p_sql;

            string str = p_sql;
            try
            {
                string prefix = str.Contains('@') ? "@" : ":";
                if (p_params is IDictionary<string, object> dt)
                {
                    // 键/值型对象
                    foreach (var item in dt)
                    {
                        str = ReplaceSql(str, prefix + item.Key, item.Value);
                    }
                }
                else
                {
                    // 普通对象
                    Type type = p_params.GetType();
                    foreach (var prop in type.GetProperties())
                    {
                        str = ReplaceSql(str, prefix + prop.Name, prop.GetValue(p_params));
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
        static string ReplaceSql(string p_sql, string p_key, object p_value)
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

        static readonly Regex _sqlPattern = new Regex("[0-9a-zA-Z_$]");
        #endregion

        #region Sql字典
        /// <summary>
        /// 获取查询Sql语句，默认从缓存字典中查询，service.json中CacheSql为false时直接从表xxx_sql查询！
        /// </summary>
        /// <param name="p_keyOrSql">输入参数为键名(无空格) 或 Sql语句，含空格时不需查询，直接返回Sql语句</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected string GetSql(string p_keyOrSql)
        {
            if (string.IsNullOrEmpty(p_keyOrSql))
                throw new Exception("Sql键名不可为空！");

            // Sql语句中包含空格
            if (p_keyOrSql.IndexOf(' ') != -1)
                return p_keyOrSql;

            // 键名不包含空格！！！

            // 已缓存
            if (_sqlDict != null)
            {
                if (_sqlDict.TryGetValue(p_keyOrSql, out var sql))
                    return sql;
                throw new Exception($"不存在键名为[{p_keyOrSql}]的Sql语句！");
            }

            // 未缓存，直接查询
            var task = GetScalar<string>(DbInfo.DebugSqlStr, new { id = p_keyOrSql });
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// 缓存Sql串
        /// </summary>
        internal static void CacheSql()
        {
            if (Kit.GetCfg("CacheSql", true))
            {
                LoadCacheSql().Wait();
            }
            else
            {
                // 生成查询sql
                if (Kit.SingletonSvcDbs != null)
                {
                    Dictionary<DbInfo, IDataAccess> das = new Dictionary<DbInfo, IDataAccess>();
                    foreach (var item in Kit.SingletonSvcDbs)
                    {
                        var di = item.Value;
                        if (!das.TryGetValue(di, out var da))
                        {
                            da = di.NewDataAccess();
                            da.AutoClose = false;
                            das.Add(di, da);
                        }

                        var task = da.ExistTable(item.Key + "_sql");
                        task.Wait();
                        if (task.Result)
                        {
                            var sql = GetSelectStr(item.Key, di.Type);
                            if (di.DebugSqlStr != null && di.DebugSqlStr.Length > 0)
                            {
                                di.DebugSqlStr += " union ";
                                di.DebugSqlStr += sql;
                            }
                            else
                            {
                                di.DebugSqlStr = sql;
                            }
                        }
                    }

                    foreach (var da in das.Values)
                    {
                        da.Close(true).Wait();
                    }
                }
                else
                {
                    // 只默认库
                    StringBuilder sb = new StringBuilder();
                    foreach (var name in Kit.SvcNames)
                    {
                        if (DbSchema.Schema.ContainsKey(name + "_sql"))
                        {
                            if (sb.Length > 0)
                                sb.Append(" union ");
                            sb.Append(GetSelectStr(name, Kit.DefaultDbInfo.Type));
                        }
                    }
                    Kit.DefaultDbInfo.DebugSqlStr = sb.ToString();
                }
                Log.Information("未缓存Sql, 调试状态");
            }

            string GetSelectStr(string p_svc, DatabaseType p_type)
            {
                switch (p_type)
                {
                    case DatabaseType.MySql:
                        return $"select `sql` from {p_svc}_sql where id=@id";

                    case DatabaseType.Oracle:
                        return $"select \"sql\" from {p_svc}_sql where id=:id";

                    default:
                        return $"select [sql] from {p_svc}_sql where id=@id";
                }
            }
        }

        /// <summary>
        /// 系统配置(json文件)修改事件
        /// </summary>
        internal static void OnConfigChanged()
        {
            bool refresh = false;
            if (Kit.GetCfg("CacheSql", true) && _sqlDict == null)
            {
                Log.Information("切换到Sql缓存模式");
                refresh = true;
            }
            else if (!Kit.GetCfg("CacheSql", true) && _sqlDict != null)
            {
                Log.Information("切换到Sql调试模式");
                _sqlDict = null;
                refresh = true;
            }

            if (refresh)
                CacheSql();
        }

        /// <summary>
        /// 缓存当前所有服务的所有Sql语句，表名xxx_sql
        /// </summary>
        internal static async Task LoadCacheSql()
        {
            try
            {
                _sqlDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (Kit.SingletonSvcDbs == null)
                {
                    // 同一库
                    var da = Kit.DefaultDbInfo.NewDataAccess();
                    foreach (var name in Kit.SvcNames)
                    {
                        await LoadSvcCacheSql(da, name);
                    }
                }
                else
                {
                    // 多个库
                    foreach (var item in Kit.SingletonSvcDbs)
                    {
                        await LoadSvcCacheSql(item.Value.NewDataAccess(), item.Key);
                    }
                }
                Log.Information("缓存Sql成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "缓存Sql失败！");
                throw;
            }
        }

        static async Task LoadSvcCacheSql(IDataAccess p_da, string p_svcName)
        {
            string sql;
            if (p_da.DbInfo.Type == DatabaseType.MySql)
                sql = $"select id,`sql` from {p_svcName}_sql";
            else if (p_da.DbInfo.Type == DatabaseType.Oracle)
                sql = $"select id,\"sql\" from {p_svcName}_sql";
            else
                sql = $"select id,[sql] from {p_svcName}_sql";

            var ls = await p_da.Each(sql);
            foreach (Row item in ls)
            {
                _sqlDict[item.Str("id")] = item.Str("sql");
            }
        }

        static Dictionary<string, string> _sqlDict;
        #endregion
    }
}