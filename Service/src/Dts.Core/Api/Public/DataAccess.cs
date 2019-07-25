#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// MySql默认库的数据访问Api
    /// </summary>
    [Api(false, null, AgentMode.Generic)]
    public class DataAccess : BaseApi
    {
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return new Db().Table(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table> GetPage(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            string sql = $"select * from ({Glb.Sql(p_keyOrSql)}) a limit {p_starRow},{p_pageSize} ";
            return new Db().Table(sql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        [CustomAgent(
@"public static async Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
{
    object result = await Call<Object>(
        ###,
        ""DataAccess.GetScalar"",
        p_keyOrSql,
        p_params
    );
    return AtKit.ConvertType<T>(result);
}
")]
        public Task<object> GetScalar(string p_keyOrSql, object p_params = null)
        {
            return new Db().Scalar<object>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public Task<Row> GetRow(string p_keyOrSql, object p_params = null)
        {
            return new Db().FirstRow(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        public async Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return await new Db().Exec(p_keyOrSql, p_params, true);
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params</param>
        /// <returns>true 成功</returns>
        public async Task<bool> BatchExec(List<Dict> p_dts)
        {
            if (p_dts == null || p_dts.Count == 0)
                return false;

            Db db = new Db();
            try
            {
                await db.BeginTrans();
                foreach (Dict dt in p_dts)
                {
                    await db.Exec((string)dt["text"], dt["params"]);
                }
                await db.CommitTrans();
                return true;
            }
            catch
            {
                await db.RollbackTrans();
            }
            return false;
        }

        /// <summary>
        /// 获取序列的下一值
        /// </summary>
        /// <param name="p_seqName">序列名称</param>
        /// <returns></returns>
        public Task<int> GetSeqVal(string p_seqName)
        {
            if (!string.IsNullOrEmpty(p_seqName))
                return new Db().Scalar<int>($"select nextval('{p_seqName}')");
            return Task.FromResult(0);
        }
    }
}
