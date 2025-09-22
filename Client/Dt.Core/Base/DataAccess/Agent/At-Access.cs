#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前数据访问对象，静态类方便使用
    /// </summary>
    public static partial class At
    {
        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Task<Table> Query(string p_sqlOrSp, object p_params = null)
        {
            return _currentAI.GetDa().Query(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Query<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return _currentAI.GetDa().Query<TEntity>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Task<Table> Page(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
        {
            return _currentAI.GetDa().Page(
                p_starRow,
                p_pageSize,
                p_sql,
                p_params
            );
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
        public static Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_sql, object p_params = null)
            where TEntity : Entity
        {
            return _currentAI.GetDa().Page<TEntity>(
                p_starRow,
                p_pageSize,
                p_sql,
                p_params
            );
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public static Task<Row> First(string p_sqlOrSp, object p_params = null)
        {
            return _currentAI.GetDa().First(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> First<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return _currentAI.GetDa().First<TEntity>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public static Task<List<T>> FirstCol<T>(string p_sqlOrSp, object p_params = null)
        {
            return _currentAI.GetDa().FirstCol<T>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static Task<T> GetScalar<T>(string p_sqlOrSp, object p_params = null)
        {
            return _currentAI.GetDa().GetScalar<T>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能，客户端代理不支持！
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public static Task<IEnumerable<Row>> Each(string p_sqlOrSp, object p_params = null)
        {
            return _currentAI.GetDa().Each(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体枚举，高性能，客户端代理不支持！
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体枚举</returns>
        public static Task<IEnumerable<TEntity>> Each<TEntity>(string p_sqlOrSp, object p_params = null)
            where TEntity : Entity
        {
            return _currentAI.GetDa().Each<TEntity>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一列枚举，高性能，客户端代理不支持！
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型枚举</returns>
        public static Task<IEnumerable<T>> EachFirstCol<T>(string p_sqlOrSp, object p_params = null)
        {
            return _currentAI.GetDa().EachFirstCol<T>(p_sqlOrSp, p_params);
        }
        #endregion

        #region 增删改
        /// <summary>
        /// 以参数值方式执行Sql语句，返回影响的行数，底层方法万不得已慎用！
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象</param>
        /// <returns>执行后影响的行数</returns>
        public static Task<int> Exec(string p_sqlOrSp, object p_params = null)
        {
            return _currentAI.GetDa().Exec(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 一个事务内执行多个Sql，底层方法万不得已慎用！
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> BatchExec(List<Dict> p_dts)
        {
            return _currentAI.GetDa().BatchExec(p_dts);
        }
        #endregion

        #region 新ID和序列
        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return _currentAI.GetDa().NewID();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_seqName">序列名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_seqName)
        {
            return _currentAI.GetDa().NewSeq(p_seqName);
        }
        #endregion
        
        #region 缓存
        /// <summary>
        /// 根据键查询字符串类型的缓存值
        /// </summary>
        /// <param name="p_key">完整缓存键，形如"prefix:key"</param>
        /// <returns>缓存字符串</returns>
        public static Task<string> StringGet(string p_key)
        {
            return _currentAI.GetDa().StringGet(p_key);
        }

        /// <summary>
        /// 将键值添加到缓存
        /// </summary>
        /// <param name="p_key">完整缓存键，形如"prefix:key"，非空</param>
        /// <param name="p_value">待缓存内容</param>
        /// <param name="p_seconds">过期秒数，0表始终不过期</param>
        /// <returns></returns>
        public static Task<bool> StringSet(string p_key, string p_value, double p_seconds = 0)
        {
            return _currentAI.GetDa().StringSet(p_key, p_value, p_seconds);
        }

        /// <summary>
        /// 根据键名删除缓存内容
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns></returns>
        public static Task<bool> KeyDelete(string p_key)
        {
            return _currentAI.GetDa().KeyDelete(p_key);
        }

        /// <summary>
        /// 根据键名列表批量删除缓存
        /// </summary>
        /// <param name="p_keys">键名列表</param>
        /// <returns></returns>
        public static Task BatchKeyDelete(List<string> p_keys)
        {
            return _currentAI.GetDa().BatchKeyDelete(p_keys);
        }
        #endregion

        #region 库信息
        /// <summary>
        /// 获取当前服务默认数据库的类型
        /// </summary>
        /// <returns></returns>
        public static Task<DatabaseType> GetDbType()
        {
            return _currentAI.GetDa().GetDbType();
        }

        /// <summary>
        /// 获取当前服务的默认数据源键名
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetDbKey()
        {
            return _currentAI.GetDa().GetDbKey();
        }
        #endregion

        #region 表结构
        /// <summary>
        /// 获取数据库所有表结构信息
        /// </summary>
        /// <returns></returns>
        public static Task<IReadOnlyDictionary<string, TableSchema>> GetDbSchema()
        {
            return _currentAI.GetDa().GetDbSchema();
        }

        /// <summary>
        /// 获取数据库的所有表名
        /// </summary>
        /// <returns></returns>
        public static Task<List<string>> GetAllTableNames()
        {
            return _currentAI.GetDa().GetAllTableNames();
        }

        /// <summary>
        /// 获取单个表结构信息
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static Task<TableSchema> GetTableSchema(string p_tblName)
        {
            return _currentAI.GetDa().GetTableSchema(p_tblName);
        }

        /// <summary>
        /// 同步数据库时间
        /// </summary>
        /// <returns></returns>
        public static Task SyncDbTime()
        {
            return _currentAI.GetDa().SyncDbTime();
        }
        #endregion
    }
}
