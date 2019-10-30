#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 公共Api（自动生成）
    /// </summary>
    public partial class DataAccess
    {
        #region Da
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.Query",
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
        public Task<Table> GetPage(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.GetPage",
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
        public async Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            object result = await new UnaryRpc(
                _svc,
                "Da.GetScalar",
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
        public Task<Row> GetRow(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.GetRow",
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
        public Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _svc,
                "Da.Exec",
                p_keyOrSql,
                p_params
            ).Call<int>();
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>true 成功</returns>
        public Task<bool> BatchExec(List<Dict> p_dts)
        {
            return new UnaryRpc(
                _svc,
                "Da.BatchExec",
                p_dts
            ).Call<bool>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public Task<long> NewID()
        {
            return new UnaryRpc(
                _svc,
                "Da.NewID"
            ).Call<long>();
        }

        /// <summary>
        /// 获取新ID和新序列值
        /// </summary>
        /// <param name="p_seqName">序列名称，不可为空</param>
        /// <returns>返回新ID和新序列值列表</returns>
        public Task<List<long>> NewIDAndSeq(string p_seqName)
        {
            return new UnaryRpc(
                _svc,
                "Da.NewIDAndSeq",
                p_seqName
            ).Call<List<long>>();
        }
        #endregion
    }
}