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
    public abstract class EntityAccess<TAccessInfo>
        where TAccessInfo : AccessInfo, new()
    {
        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return _ea.Query(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Query<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return _ea.Query<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Task<Table> Page(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return _ea.Page(
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            );
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public static Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return _ea.Page<TEntity>(
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            );
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public static Task<Row> First(string p_keyOrSql, object p_params = null)
        {
            return _ea.First(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> First<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return _ea.First<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public static Task<List<T>> FirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            return _ea.FirstCol<T>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            return _ea.GetScalar<T>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能，客户端代理不支持！
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public static Task<IEnumerable<Row>> Each(string p_keyOrSql, object p_params = null)
        {
            return _ea.Each(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体枚举，高性能，客户端代理不支持！
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体枚举</returns>
        public static Task<IEnumerable<TEntity>> Each<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return _ea.Each<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一列枚举，高性能，客户端代理不支持！
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型枚举</returns>
        public static Task<IEnumerable<T>> EachFirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            return _ea.EachFirstCol<T>(p_keyOrSql, p_params);
        }
        #endregion

        #region 增删改
        /// <summary>
        /// 以参数值方式执行Sql语句，返回影响的行数，底层方法万不得已慎用！
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象</param>
        /// <returns>执行后影响的行数</returns>
        public static Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return _ea.Exec(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 一个事务内执行多个Sql，底层方法万不得已慎用！
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> BatchExec(List<Dict> p_dts)
        {
            return _ea.BatchExec(p_dts);
        }
        #endregion

        /// <summary>
        /// 数据访问对象
        /// </summary>
        public static IEntityAccess Ea => _ea;

        static IEntityAccess _ea = new TAccessInfo().GetEntityAccess();
    }
}
