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
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;
#if SERVER
using Microsoft.Extensions.Caching.Memory;
#endif
#endregion

namespace Dt.Core;

/// <summary>
/// 数据库访问抽象基类，全部采用异步操作
/// 基于开源项目 Dapper
/// Dapper涉及dynamic的速度非常慢！已废弃
/// </summary>
abstract class DbAccess : IDataAccess
{
    #region 成员变量
    protected DbConnection _conn;
    protected DbTransaction _tran;
    // 只Npgsql为KeyInfo，否则不返回列结构！其余 Default
    protected CommandBehavior _cmdBehavior;
    internal DbAccessInfo _dbInfo;
    #endregion

    #region 构造方法
    public DbAccess(DbAccessInfo p_info)
    {
        _dbInfo = p_info;
    }
    #endregion

    #region 属性
    /// <summary>
    /// 调用每个公共方法后是否自动关闭连接，默认true，false时切记最后手动关闭！
    /// </summary>
    public bool AutoClose { get; set; } = true;
    #endregion

    #region Table查询
    /// <summary>
    /// 以参数值方式执行Sql语句，返回结果集
    /// </summary>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Table数据集</returns>
    public async Task<Table> Query(string p_sqlOrKey, object p_params = null)
    {
        var tbl = new Table();
        await QueryInternal<Row>(tbl, p_sqlOrKey, p_params);
        return tbl;
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回结果集
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Table数据集</returns>
    public async Task<Table<TEntity>> Query<TEntity>(string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
        var tbl = new Table<TEntity>();
        await QueryInternal<TEntity>(tbl, p_sqlOrKey, p_params);
        return tbl;
    }

    /// <summary>
    /// 按页查询数据
    /// </summary>
    /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
    /// <param name="p_pageSize">每页显示行数</param>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Table数据</returns>
    public Task<Table> Page(int p_starRow, int p_pageSize, string p_sqlOrKey, object p_params = null)
    {
#if SERVER
        // 无空格先查询Sql字典
        if (p_sqlOrKey.IndexOf(' ') == -1)
            p_sqlOrKey = Kit.Sql(p_sqlOrKey);
#endif
        return Query(GetPageSql(p_starRow, p_pageSize, p_sqlOrKey), p_params);
    }

    /// <summary>
    /// 按页查询数据
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
    /// <param name="p_pageSize">每页显示行数</param>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Table数据集</returns>
    public Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
#if SERVER
        // 无空格先查询Sql字典
        if (p_sqlOrKey.IndexOf(' ') == -1)
            p_sqlOrKey = Kit.Sql(p_sqlOrKey);
#endif
        return Query<TEntity>(GetPageSql(p_starRow, p_pageSize, p_sqlOrKey), p_params);
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
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Row枚举</returns>
    public Task<IEnumerable<Row>> Each(string p_sqlOrKey, object p_params = null)
    {
        return ForEachRow<Row>(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回Row枚举，高性能
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Row枚举</returns>
    public Task<IEnumerable<TEntity>> Each<TEntity>(string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
        return ForEachRow<TEntity>(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，只返回第一行数据
    /// </summary>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一行Row或null</returns>
    public async Task<Row> First(string p_sqlOrKey, object p_params = null)
    {
        return (await ForEachRow<Row>(p_sqlOrKey, p_params)).FirstOrDefault();
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，只返回第一行数据
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一行Row或null</returns>
    public async Task<TEntity> First<TEntity>(string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
        return (await ForEachRow<TEntity>(p_sqlOrKey, p_params)).FirstOrDefault();
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，只返回第一个单元格数据
    /// </summary>
    /// <typeparam name="T">单元格数据类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一个单元格数据</returns>
    public async Task<T> GetScalar<T>(string p_sqlOrKey, object p_params = null)
    {
        var cmd = CreateCommand(p_sqlOrKey, p_params, false);
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
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一列数据的泛型列表</returns>
    public async Task<List<T>> FirstCol<T>(string p_sqlOrKey, object p_params = null)
    {
        return (List<T>)await FirstCol(typeof(T), p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
    /// </summary>
    /// <param name="p_type">第一列数据类型</param>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一列数据的泛型列表</returns>
    [UnconditionalSuppressMessage("AOT", "IL3050")]
    public async Task<object> FirstCol(Type p_type, string p_sqlOrKey, object p_params = null)
    {
        Throw.IfNull(p_type);
        var cmd = CreateCommand(p_sqlOrKey, p_params, false);
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
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回泛型枚举</returns>
    public async Task<IEnumerable<T>> EachFirstCol<T>(string p_sqlOrKey, object p_params = null)
    {
        var cmd = CreateCommand(p_sqlOrKey, p_params, true);
        await OpenConnection();
        var reader = (IWrappedDataReader)await _conn.ExecuteReaderAsync(cmd, _cmdBehavior);
        return ForEachFirstCol<T>(reader);
    }

    /// <summary>
    /// 获取新ID
    /// </summary>
    /// <returns></returns>
    public Task<long> NewID()
    {
        return Task.FromResult(Kit.NewID);
    }

    /// <summary>
    /// 获取新序列值
    /// </summary>
    /// <param name="p_seqName">序列名称，不可为空</param>
    /// <returns>新序列值</returns>
    public abstract Task<long> NewSeq(string p_seqName);

    /// <summary>
    /// 根据表名创建行数据，自动填写ID和列的默认值
    /// </summary>
    /// <param name="p_tblName">表名</param>
    /// <param name="p_count">批量创建的行数，默认1</param>
    /// <returns></returns>
    public async Task<Table> NewRow(string p_tblName, int p_count)
    {
        var model = await TableSchema.GetSchema(p_tblName);
        var tbl = new Table();
        int cnt = p_count > 0 ? p_count : 1;
        // 批量生成时重复
        long id = Kit.NewID;
        for (int i = 0; i < cnt; i++)
        {
            Row row = new Row();

            // 主键
            foreach (var col in model.PrimaryKey)
            {
                if (i == 0)
                    tbl.Add(col.Name, col.Type);
                new Cell(row, col.Name, col.Type, col.Type == typeof(long) ? id++ : null);
            }

            foreach (var col in model.Columns)
            {
                if (i == 0)
                    tbl.Add(col.Name, col.Type);
                new Cell(row, col.Name, col.Type, string.IsNullOrEmpty(col.Default) ? null : col.Default);
            }
            tbl.Add(row);
        }
        return tbl;
    }

    async Task QueryInternal<TRow>(Table p_tbl, string p_sqlOrKey, object p_params = null)
        where TRow : Row
    {
        var cmd = CreateCommand(p_sqlOrKey, p_params, false);
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

    [UnconditionalSuppressMessage("AOT", "IL3050")]
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

    async Task<IEnumerable<TRow>> ForEachRow<TRow>(string p_sqlOrKey, object p_params = null)
        where TRow : Row
    {
        var cmd = CreateCommand(p_sqlOrKey, p_params, true);
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
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象</param>
    /// <returns>执行后影响的行数</returns>
    public async Task<int> Exec(string p_sqlOrKey, object p_params = null)
    {
        try
        {
            await OpenConnection();
            var cmd = CreateCommand(p_sqlOrKey, p_params, false);
            var result = await _conn.ExecuteAsync(cmd);
            return result;
        }
        catch (Exception ex)
        {
            throw GetSqlException(CreateCommand(p_sqlOrKey, p_params, false), ex);
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
                if (dt.TryGetValue("params", out var pars))
                {
                    if (pars is List<Dict> ls)
                    {
                        // 一个sql，多行数据
                        cnt += await Exec(sql, ls);
                    }
                    else if (pars is Dict par)
                    {
                        // 一个sql，一行数据
                        cnt += await Exec(sql, par);
                    }
                }
                else
                {
                    // 无参数
                    cnt += await Exec(sql);
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

    /// <summary>
    /// 一个事务内批量保存列表中所有需要新增、修改、删除的数据
    /// <para>1. SaveItem项中的Data只支持：有表名的Table、有表名的Row、Entity、Table{TEntity}</para>
    /// <para>2. Entity、Table{TEntity}支持实体的各种回调和领域事件</para>
    /// <para>3. 反序列化时根据表名确定是否有对应Entity，没有时按Row、Table处理</para>
    /// </summary>
    /// <param name="p_datas">保存项列表</param>
    /// <returns>是否成功</returns>
    public async Task<bool> Save(List<SaveItem> p_datas)
    {
        var w = new EntityWriter(this);
        await w.SaveItems(p_datas);
        return await w.Commit();
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
        return Task.FromResult(_dbInfo.DbType);
    }

    public Task<string> GetDbKey()
    {
        return Task.FromResult(_dbInfo.Name);
    }

#if !SERVER
    public IAccessInfo AccessInfo => _dbInfo;
#endif
    #endregion

    #region 实体写入器
    public IEntityWriter NewWriter() => new EntityWriter(this);
    #endregion

    #region 内部方法
    const string _sqlFormatArgs = "f";

    /// <summary>
    /// 创建Dapper的命令定义
    /// </summary>
    /// <param name="p_sqlOrKey"></param>
    /// <param name="p_params"></param>
    /// <param name="p_deferred"></param>
    /// <returns></returns>
    protected CommandDefinition CreateCommand(string p_sqlOrKey, object p_params, bool p_deferred)
    {
        var sql = p_sqlOrKey;
        if (string.IsNullOrWhiteSpace(sql))
            throw new Exception("Sql语句或键名不能为空！");

#if SERVER
        // Sql键名以#开头
        if (sql[0] == '#')
        {
            if (sql.Length > 1)
            {
                // 获取sql模板
                sql = Kit.Sql(p_sqlOrKey.AsSpan(1));
                if (sql == null)
                    throw new Exception($"Sql键名 {p_sqlOrKey} 不存在！");

                // sql模板可能需要格式化
                if (p_params is Dict dt
                    && dt.TryGetValue(_sqlFormatArgs, out object formatArgs))
                {
                    sql = string.Format(sql, (formatArgs is object[] args) ? args : formatArgs);
                    // 移除格式化键值，提供到Command时异常
                    dt.Remove(_sqlFormatArgs);
                    if (dt.Count == 0)
                        p_params = null;
                }
            }
            else
            {
                // 仅有#，通用查询，动态生成sql模板
                if (p_params is Dict dt
                    && dt.TryGetValue(_sqlFormatArgs, out object args))
                {
                    if (args is string tbl)
                    {
                        // 单一个表名
                        sql = "SELECT * FROM " + tbl;
                    }
                    else if (args is IList arr && arr.Count > 0)
                    {
                        // 数组顺序：表名、条件、字段
                        sql = "SELECT";
                        var table = arr[0] as string;

                        if (arr.Count > 2
                            && arr[2] is string field
                            && !string.IsNullOrWhiteSpace(field))
                            sql += $" {field} FROM {table}";
                        else
                            sql += " * FROM " + table;

                        if (arr.Count > 1
                            && arr[1] is string where
                            && !string.IsNullOrWhiteSpace(where))
                            sql += " WHERE " + where;
                    }
                    else
                    {
                        throw new Exception("通用查询方式需提供单个表名或按顺序提供：表名、条件、字段！");
                    }
                    dt.Remove(_sqlFormatArgs);
                    if (dt.Count == 0)
                        p_params = null;
                }
                else
                {
                    throw new Exception("通用查询方式至少提供表名！");
                }
            }

            if (Kit.TraceSql)
                Kit.ContextLog.Information(p_sqlOrKey + ": " + sql);
        }
        else if (Kit.TraceSql)
        {
            Kit.ContextLog.Information(sql);
        }
#else
        Log.Debug(sql);
#endif

        return new CommandDefinition(
            sql,
            p_params,
            _tran,
            null,
            CommandType.Text,
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

    #region 缓存
    public async Task<T> GetCache<T>(object p_key, bool p_persist = false)
    {
#if SERVER
        string key = p_key == null ? null : p_key.ToString();
        if (string.IsNullOrEmpty(key))
            return default;

        if (Kit.Cache.TryGetValue(key, out T val))
            return val;

        if (p_persist)
        {
            var v = await GetScalar<string>($"select val from cm_cache where id='{key}'");
            if (!string.IsNullOrEmpty(v))
            {
                var result = Kit.Deserialize<T>(v);
                Kit.Cache.Set(key, result, new MemoryCacheEntryOptions { Size = 1 });
                return result;
            }
        }
        return default;
#else
        throw new Exception("直连数据库不支持缓存操作！");
#endif
    }

    public async Task<bool> SetCache(object p_key, object p_value, bool p_persist = false)
    {
#if SERVER
        string key = p_key == null ? null : p_key.ToString();
        if (string.IsNullOrEmpty(key))
            return false;

        Kit.Cache.Set(key, p_value, new MemoryCacheEntryOptions { Size = 1 });
        if (p_persist)
        {
            string val = Kit.Serialize(p_value);
            List<Dict> ls = new List<Dict>
            {
                new Dict { { "text", $"delete from cm_cache where id='{key}'" } },
                new Dict { {"text", $"insert into cm_cache (id,val) values ('{key}','{val}')"}},
            };
            return await BatchExec(ls) > 0;
        }
        return true;
#else
        throw new Exception("直连数据库不支持缓存操作！");
#endif
    }

    public Task<bool> SetExpireCache(object p_key, object p_value, double p_seconds)
    {
#if SERVER
        string key = p_key == null ? null : p_key.ToString();
        if (string.IsNullOrEmpty(key) || p_value == null || p_seconds < 0)
            return Task.FromResult(false);

        var option = new MemoryCacheEntryOptions { Size = 1 };
        if (p_seconds > 0)
        {
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(p_seconds);
        }
        Kit.Cache.Set(key, p_value, option);
        return Task.FromResult(true);
#else
        throw new Exception("直连数据库不支持缓存操作！");
#endif
    }

    public async Task<bool> DeleteCache(object p_key, bool p_persist = false)
    {
#if SERVER
        string key = p_key == null ? null : p_key.ToString();
        if (string.IsNullOrEmpty(key))
            return false;

        Kit.Cache.Remove(key);
        if (p_persist)
        {
            return await Exec($"delete from cm_cache where id='{key}'") > 0;
        }
        return true;
#else
        throw new Exception("直连数据库不支持缓存操作！");
#endif
    }

    public async Task<bool> BatchDeleteCache(IList p_keys, bool p_persist = false)
    {
#if SERVER
        if (p_keys == null || p_keys.Count == 0)
            return false;

        List<Dict> ls = new List<Dict>();
        foreach (var key in p_keys)
        {
            string k = key == null ? null : key.ToString();
            if (string.IsNullOrEmpty(k))
                continue;

            Kit.Cache.Remove(k);
            ls.Add(new Dict { { "id", k } });
        }

        if (p_persist)
        {
            string prefix = _dbInfo.DbType == DatabaseType.Oracle ? ":" : "@";
            return await Exec($"delete from cm_cache where id={prefix}id", ls) > 0;
        }
        return true;
#else
        throw new Exception("直连数据库不支持缓存操作！");
#endif
    }

    public async Task<bool> DelCacheByPattern(string p_keyStr, int p_pattern, bool p_persist = false)
    {
#if SERVER
        if (string.IsNullOrEmpty(p_keyStr) || p_pattern < 0 || p_pattern > 2)
            return false;

        var cache = Kit.Cache as MemoryCache;
        foreach (var key in cache.Keys)
        {
            string k = key == null ? null : key.ToString();
            if (string.IsNullOrEmpty(k))
                continue;

            bool suc = p_pattern switch
            {
                0 => k.StartsWith(p_keyStr),
                1 => k.EndsWith(p_keyStr),
                2 => k.Contains(p_keyStr),
                _ => false,
            };
            if (suc)
            {
                Kit.Cache.Remove(k);
            }
        }

        if (p_persist)
        {
            string prefix = p_pattern switch
            {
                0 => "'" + p_keyStr + "%'",
                1 => "'%" + p_keyStr + "'",
                2 => "'%" + p_keyStr + "%'",
                _ => null,
            };
            return await Exec($"delete from cm_cache where id like {prefix}") > 0;
        }
        return true;
#else
        throw new Exception("直连数据库不支持缓存操作！");
#endif
    }

    public Task<List<string>> GetCacheKeys(string p_keyStr, int p_pattern)
    {
#if SERVER
        var cache = Kit.Cache as MemoryCache;
        if (string.IsNullOrEmpty(p_keyStr))
            return Task.FromResult(cache.Keys.Cast<string>().ToList());

        List<string> ls = new();
        foreach (var key in cache.Keys)
        {
            string k = key == null ? "" : key.ToString();
            bool suc = p_pattern switch
            {
                0 => k.StartsWith(p_keyStr),
                1 => k.EndsWith(p_keyStr),
                2 => k.Contains(p_keyStr),
                _ => false,
            };
            if (suc)
            {
                ls.Add(k);
            }
        }
        return Task.FromResult(ls);
#else
        throw new Exception("直连数据库不支持缓存操作！");
#endif
    }
    #endregion
}