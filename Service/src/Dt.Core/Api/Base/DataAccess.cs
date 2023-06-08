#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 默认库的数据访问Api，全称DataAccess
    /// </summary>
    [Api(AgentMode = AgentMode.Generic)]
    public class Da : DomainSvc
    {
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return _da.Query(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public Task<Table> Page(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return _da.Page(p_starRow, p_pageSize, p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public Task<Row> First(string p_keyOrSql, object p_params = null)
        {
            return _da.First(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <param name="p_type">第一列数据类型</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public Task<object> FirstCol(string p_type, string p_keyOrSql, object p_params = null)
        {
            return _da.FirstCol(Type.GetType(p_type), p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        [CustomAgent(
@"public static Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
{
    return new UnaryRpc(
        ###,
        ""Da.GetScalar"",
        p_keyOrSql,
        p_params
    ).Call<T>();
}
")]
        public Task<object> GetScalar(string p_keyOrSql, object p_params = null)
        {
            return _da.GetScalar<object>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回影响的行数
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象</param>
        /// <returns>返回执行后影响的行数</returns>
        public Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return _da.Exec(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public Task<int> BatchExec(List<Dict> p_dts)
        {
            return _da.BatchExec(p_dts);
        }

        /// <summary>
        /// 获取新ID，默认无标志位
        /// </summary>
        /// <returns></returns>
        public long NewID()
        {
            return Kit.NewID;
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_seqName">序列名称，不可为空</param>
        /// <returns>新序列值</returns>
        public Task<int> NewSeq(string p_seqName)
        {
            return _da.NewSequence(p_seqName);
        }
    }
}
