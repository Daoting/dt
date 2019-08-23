#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Bs.Mgr
{
    /// <summary>
    /// 所有服务的公共Api代理类（自动生成）
    /// </summary>
    /// <typeparam name="TSrv">服务类型</typeparam>
    public partial class SrvAgent<TSrv>
    {
        #region DataAccess
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public static Task<Table> Query(string p_keyOrSql, Object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "DataAccess.Query",
                p_keyOrSql,
                p_params
            ).Call<Table>();
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public static Task<Table> GetPage(int p_starRow, int p_pageSize, string p_keyOrSql, Object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "DataAccess.GetPage",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            ).Call<Table>();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static async Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            object result = await new UnaryRpc(
                typeof(TSrv).Name,
                "DataAccess.GetScalar",
                p_keyOrSql,
                p_params
            ).Call<Object>();
            return AtKit.ConvertType<T>(result);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public static Task<Row> GetRow(string p_keyOrSql, Object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "DataAccess.GetRow",
                p_keyOrSql,
                p_params
            ).Call<Row>();
        }

        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> Exec(string p_keyOrSql, Object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "DataAccess.Exec",
                p_keyOrSql,
                p_params
            ).Call<int>();
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params</param>
        /// <returns>true 成功</returns>
        public static Task<bool> BatchExec(List<Dict> p_dts)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "DataAccess.BatchExec",
                p_dts
            ).Call<bool>();
        }

        /// <summary>
        /// 获取序列的下一值
        /// </summary>
        /// <param name="p_seqName">序列名称</param>
        /// <returns></returns>
        public static Task<int> GetSeqVal(string p_seqName)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "DataAccess.GetSeqVal",
                p_seqName
            ).Call<int>();
        }
        #endregion
    }
}
