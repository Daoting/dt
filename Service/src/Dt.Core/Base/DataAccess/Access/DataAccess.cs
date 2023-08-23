#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dapper;
using Dapper.Oracle;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;
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
        // 只Npgsql为KeyInfo，否则不返回列结构！其余 Default
        protected CommandBehavior _cmdBehavior;
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
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public async Task<Table> Query(string p_sqlOrSp, object p_params = null)
        {
            var tbl = new Table();
            await QueryInternal<Row>(tbl, p_sqlOrSp, p_params);
            return tbl;
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public async Task<Table<TEntity>> Query<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            var tbl = new Table<TEntity>();
            await QueryInternal<TEntity>(tbl, p_sqlOrSp, p_params);
            return tbl;
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public Task<Table> Page(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
        {
            return Query(GetPageSql(p_starRow, p_pageSize, p_sql), p_params);
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
            where TEntity : Entity
        {
            return Query<TEntity>(GetPageSql(p_starRow, p_pageSize, p_sql), p_params);
        }

        /// <summary>
        /// 构造按页查询sql
        /// </summary>
        /// <param name="p_starRow"></param>
        /// <param name="p_pageSize"></param>
        /// <param name="p_sql">Sql语句</param>
        /// <returns></returns>
        protected abstract string GetPageSql(int p_starRow, int p_pageSize, string p_sql);

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public Task<IEnumerable<Row>> Each(string p_sqlOrSp, object p_params = null)
        {
            return ForEachRow<Row>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public Task<IEnumerable<TEntity>> Each<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return ForEachRow<TEntity>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public async Task<Row> First(string p_sqlOrSp, object p_params = null)
        {
            return (await ForEachRow<Row>(p_sqlOrSp, p_params)).FirstOrDefault();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public async Task<TEntity> First<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return (await ForEachRow<TEntity>(p_sqlOrSp, p_params)).FirstOrDefault();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <typeparam name="T">单元格数据类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public async Task<T> GetScalar<T>(string p_sqlOrSp, object p_params = null)
        {
            var cmd = CreateCommand(p_sqlOrSp, p_params, false, true);
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
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public async Task<List<T>> FirstCol<T>(string p_sqlOrSp, object p_params = null)
        {
            return (List<T>)await FirstCol(typeof(T), p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <param name="p_type">第一列数据类型</param>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public async Task<object> FirstCol(Type p_type, string p_sqlOrSp, object p_params = null)
        {
            Throw.IfNull(p_type);
            var cmd = CreateCommand(p_sqlOrSp, p_params, false, true);
            try
            {
                await OpenConnection();
                using (var wrappedReader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd, _cmdBehavior))
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
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回泛型枚举</returns>
        public async Task<IEnumerable<T>> EachFirstCol<T>(string p_sqlOrSp, object p_params = null)
        {
            var cmd = CreateCommand(p_sqlOrSp, p_params, true, true);
            await OpenConnection();
            var reader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd, _cmdBehavior);
            return ForEachFirstCol<T>(reader);
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_seqName">序列名称，不可为空</param>
        /// <returns>新序列值</returns>
        public abstract Task<int> NewSequence(string p_seqName);

        async Task QueryInternal<TRow>(Table p_tbl, string p_sqlOrSp, object p_params = null)
            where TRow : Row
        {
            var cmd = CreateCommand(p_sqlOrSp, p_params, false, true);
            try
            {
                await OpenConnection();
                using (var wrappedReader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd, _cmdBehavior))
                {
                    // Dapper2.0 改版
                    var reader = (DbDataReader)wrappedReader.Reader;

                    // Entity类型
                    Dictionary<string, Type> specialCols = null;
                    if (typeof(TRow).IsSubclassOf(typeof(Entity)))
                    {
                        // 提取Entity中特殊类型的属性：enum bool，enum都需要处理，只有oracle需要特殊处理bool，只为统一写法
                        // bool：mysql tinyint(1)，sqlserver bit，pg bool 默认自动映射，只有 oracle char(1) 需特殊处理
                        // enum：mysql sqlserver枚举为byte，oracle pg枚举为short
                        specialCols = Entity.GetSpecialCols(typeof(TRow));
                    }

                    // 加载列定义
                    var cols = reader.GetColumnSchema();
                    if (specialCols != null && specialCols.Count > 0)
                    {
                        foreach (var col in cols)
                        {
                            Type colType;

                            // 自动生成属性名时无下划线，字典有说明为enum 或 bool
                            if (!specialCols.TryGetValue(col.ColumnName.Replace("_", ""), out colType))
                                colType = GetColumnType(col);

                            p_tbl.Add(col.ColumnName, colType);
                        }
                    }
                    else
                    {
                        foreach (var col in cols)
                        {
                            p_tbl.Add(col.ColumnName, GetColumnType(col));
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

        protected virtual Type GetColumnType(DbColumn p_col)
        {
            if (p_col.AllowDBNull.HasValue
                && p_col.AllowDBNull.Value
                && p_col.DataType.IsValueType)
            {
                // 可为null的值类型
                return typeof(Nullable<>).MakeGenericType(p_col.DataType);
            }

            return p_col.DataType;
        }

        async Task<IEnumerable<TRow>> ForEachRow<TRow>(string p_sqlOrSp, object p_params = null)
            where TRow : Row
        {
            var cmd = CreateCommand(p_sqlOrSp, p_params, true, true);
            await OpenConnection();
            var reader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd, _cmdBehavior);
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
                    Dictionary<string, Type> specialCols = null;
                    if (typeof(TRow).IsSubclassOf(typeof(Entity)))
                    {
                        // 提取Entity中特殊类型的属性：enum bool，enum都需要处理，只有oracle需要特殊处理bool，只为统一写法
                        // bool：mysql tinyint(1)，sqlserver bit，pg bool 默认自动映射，只有 oracle char(1) 需特殊处理
                        // enum：mysql sqlserver枚举为byte，oracle pg枚举为short
                        specialCols = Entity.GetSpecialCols(typeof(TRow));
                    }

                    while (reader.Read())
                    {
                        // 无参数构造方法可能为private，如实体类型
                        TRow row = (TRow)Activator.CreateInstance(typeof(TRow), true);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var col = cols[i];

                            if (specialCols == null || !specialCols.TryGetValue(col.ColumnName.Replace("_", ""), out var colType))
                                colType = GetColumnType(col);

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
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象</param>
        /// <returns>执行后影响的行数</returns>
        public async Task<int> Exec(string p_sqlOrSp, object p_params = null)
        {
            try
            {
                await OpenConnection();
                var cmd = CreateCommand(p_sqlOrSp, p_params, false, false);
                var result = await _conn.ExecuteAsync(cmd);
                return result;
            }
            catch (Exception ex)
            {
                throw GetSqlException(CreateCommand(p_sqlOrSp, p_params, false, false), ex);
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
        public abstract Task<IReadOnlyDictionary<string, TableSchema>> GetDbSchema();

        /// <summary>
        /// 获取数据库的所有表名
        /// </summary>
        /// <returns></returns>
        public abstract Task<List<string>> GetAllTableNames();

        /// <summary>
        /// 获取单个表结构信息
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public abstract Task<TableSchema> GetTableSchema(string p_tblName);

        /// <summary>
        /// 同步数据库时间
        /// </summary>
        /// <returns></returns>
        public abstract Task SyncDbTime();
        #endregion

        #region 库信息
        public Task<DatabaseType> GetDbType()
        {
            return Task.FromResult(DbInfo.Type);
        }

        public Task<string> GetDbKey()
        {
            return Task.FromResult(DbInfo.Key);
        }
        #endregion

        #region 实体写入器
        public IEntityWriter NewWriter() => new EntityWriter();
        #endregion

        #region 内部方法
        /// <summary>
        /// 创建Dapper的命令定义
        /// </summary>
        /// <param name="p_sqlOrSp"></param>
        /// <param name="p_params"></param>
        /// <param name="p_deferred"></param>
        /// <param name="p_isQuery">是否为查询，oracle使用存储过程查询时自动添加Cursor参数</param>
        /// <returns></returns>
        protected CommandDefinition CreateCommand(string p_sqlOrSp, object p_params, bool p_deferred, bool p_isQuery)
        {
            if (Kit.TraceSql)
                Log.Information(p_sqlOrSp);

            // Sql语句中包含空格，存储过程名无空格！
            bool isSql = p_sqlOrSp.IndexOf(' ') != -1;

            // oracle使用存储过程查询时添加游标输出参数
            if (!isSql
                && p_isQuery
                && DbInfo.Type == DatabaseType.Oracle)
            {
                var pars = new OracleDynamicParameters();
                if (p_params != null)
                    pars.AddDynamicParams(p_params);

                // 添加游标输出参数
                pars.Add("p_cur", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                return new CommandDefinition(
                    p_sqlOrSp,
                    pars,
                    _tran,
                    null,
                    CommandType.StoredProcedure,
                    p_deferred ? CommandFlags.Pipelined : CommandFlags.Buffered,
                    default);
            }

            return new CommandDefinition(
                p_sqlOrSp,
                p_params,
                _tran,
                null,
                isSql ? CommandType.Text : CommandType.StoredProcedure,
                p_deferred ? CommandFlags.Pipelined : CommandFlags.Buffered,
                default);
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
        #endregion
    }
}