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
    /// 数据访问基类，为方便使用所有方法为静态，通过 TAccessInfo 提供实际的数据查询对象
    /// </summary>
    /// <typeparam name="TAccessInfo">数据访问的描述信息</typeparam>
    public abstract class DataAccess<TAccessInfo>
        where TAccessInfo : AccessInfo, new()
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
            return _da.Query(p_sqlOrSp, p_params);
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
            return _da.Query<TEntity>(p_sqlOrSp, p_params);
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
            return _da.Page(
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
            return _da.Page<TEntity>(
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
            return _da.First(p_sqlOrSp, p_params);
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
            return _da.First<TEntity>(p_sqlOrSp, p_params);
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
            return _da.FirstCol<T>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static Task<T> GetScalar<T>(string p_sqlOrSp, object p_params = null)
        {
            return _da.GetScalar<T>(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能，客户端代理不支持！
        /// </summary>
        /// <param name="p_sqlOrSp">Sql语句 或 存储过程名</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public static Task<IEnumerable<Row>> Each(string p_sqlOrSp, object p_params = null)
        {
            return _da.Each(p_sqlOrSp, p_params);
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
            return _da.Each<TEntity>(p_sqlOrSp, p_params);
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
            return _da.EachFirstCol<T>(p_sqlOrSp, p_params);
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
            return _da.Exec(p_sqlOrSp, p_params);
        }

        /// <summary>
        /// 一个事务内执行多个Sql，底层方法万不得已慎用！
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> BatchExec(List<Dict> p_dts)
        {
            return _da.BatchExec(p_dts);
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
            return _da.StringGet(p_key);
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
            return _da.StringSet(p_key, p_value, p_seconds);
        }

        /// <summary>
        /// 先根据 p_key 查到实体的主键值，再根据主键前缀 p_priKeyPrefix 和主键值查询实体json
        /// </summary>
        /// <param name="p_key">实体非主键列键名：“表名:列名:列值”</param>
        /// <param name="p_priKeyPrefix">查询实体json的键名：“表名:主键列名”</param>
        /// <returns></returns>
        public static Task<string> GetEntityJson(string p_key, string p_priKeyPrefix)
        {
            return _da.GetEntityJson(p_key, p_priKeyPrefix);
        }

        /// <summary>
        /// 根据键查询所有field-value数组
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns></returns>
        public static Task<Dict> HashGetAll(string p_key)
        {
            return _da.HashGetAll(p_key);
        }

        /// <summary>
        /// 获取指定键名的hash中field对应的value
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <param name="p_field">hash中的field，大小写敏感</param>
        /// <returns>field对应的value</returns>
        public static Task<string> HashGet(string p_key, string p_field)
        {
            return _da.HashGet(p_key, p_field);
        }

        /// <summary>
        /// 根据键名删除缓存内容
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns></returns>
        public static Task<bool> KeyDelete(string p_key)
        {
            return _da.KeyDelete(p_key);
        }

        /// <summary>
        /// 根据键名列表批量删除缓存
        /// </summary>
        /// <param name="p_keys">键名列表</param>
        /// <returns></returns>
        public static Task BatchKeyDelete(List<string> p_keys)
        {
            return _da.BatchKeyDelete(p_keys);
        }
        #endregion

        #region 库信息
        /// <summary>
        /// 获取当前服务默认数据库的类型
        /// </summary>
        /// <returns></returns>
        public static Task<DatabaseType> GetDbType()
        {
            return _da.GetDbType();
        }

        /// <summary>
        /// 获取当前服务的默认数据源键名
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetDbKey()
        {
            return _da.GetDbKey();
        }
        #endregion

        /// <summary>
        /// 数据访问对象
        /// </summary>
        public static IDataAccess Da => _da;

        static IDataAccess _da = new TAccessInfo().GetDataAccess();
    }
}
