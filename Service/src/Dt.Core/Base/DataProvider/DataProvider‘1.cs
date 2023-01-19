#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Agent
{
    /// <summary>
    /// 当前微服务作为客户端，调用其他微服务的 DataAccess Api，和客户端DataProvider'TSvc'不同，单体时直接本地调用
    /// </summary>
    /// <typeparam name="TSvc">类型名称当服务名用</typeparam>
    public abstract class DataProvider<TSvc>
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
            return Kit.Rpc<Table>(
                typeof(TSvc).Name,
                "Da.Query",
                p_keyOrSql,
                p_params
            );
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
            return Kit.Rpc<Table<TEntity>>(
                typeof(TSvc).Name,
                "Da.Query",
                p_keyOrSql,
                p_params
            );
        }

        /// <summary>
        /// 返回所有实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns></returns>
        public static Task<Table<TEntity>> GetAll<TEntity>()
            where TEntity : Entity
        {
            return Kit.Rpc<Table<TEntity>>(
                typeof(TSvc).Name,
                "Da.Query",
                EntitySchema.Get(typeof(TEntity)).Schema.GetSelectAllSql(),
                null
            );
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Task<Table> GetPage(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return Kit.Rpc<Table>(
                typeof(TSvc).Name,
                "Da.GetPage",
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
        public static Task<Table<TEntity>> GetPage<TEntity>(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return Kit.Rpc<Table<TEntity>>(
                typeof(TSvc).Name,
                "Da.GetPage",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            );
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            return Kit.Rpc<T>(
                typeof(TSvc).Name,
                "Da.GetScalar",
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
            return Kit.Rpc<Row>(
                typeof(TSvc).Name,
                "Da.First",
                p_keyOrSql,
                p_params
            );
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
            return Kit.Rpc<TEntity>(
                typeof(TSvc).Name,
                "Da.First",
                p_keyOrSql,
                p_params
            );
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
            return Kit.Rpc<List<T>>(
                typeof(TSvc).Name,
                "Da.FirstCol",
                typeof(T).FullName,
                p_keyOrSql,
                p_params
            );
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键id，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            return First<TEntity>(EntitySchema.Get(typeof(TEntity)).Schema.GetSelectByIDSql(), new { id = p_id });
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键id，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(long p_id)
            where TEntity : Entity
        {
            return First<TEntity>(EntitySchema.Get(typeof(TEntity)).Schema.GetSelectByIDSql(), new { id = p_id });
        }
        #endregion

        #region Exec
        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return Kit.Rpc<int>(
                typeof(TSvc).Name,
                "Da.Exec",
                p_keyOrSql,
                p_params
            );
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> BatchExec(List<Dict> p_dts)
        {
            return Kit.Rpc<int>(
                typeof(TSvc).Name,
                "Da.BatchExec",
                p_dts
            );
        }
        #endregion
    }
}
