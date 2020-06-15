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

namespace Dt.Base
{
    /// <summary>
    /// 所有服务的公共Api代理类（自动生成）
    /// </summary>
    /// <typeparam name="TSrv">服务类型</typeparam>
    public partial class SrvAgent<TSrv>
    {
        #region Da
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public static Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
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
        public static Task<Table> GetPage(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
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
        public static Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Da.GetScalar",
                p_keyOrSql,
                p_params
            ).Call<T>();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public static Task<Row> GetRow(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
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
        public static Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
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
        public static Task<bool> BatchExec(List<Dict> p_dts)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Da.BatchExec",
                p_dts
            ).Call<bool>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Da.NewID"
            ).Call<long>();
        }

        /// <summary>
        /// 获取新ID和新序列值
        /// </summary>
        /// <param name="p_seqName">序列名称，不可为空</param>
        /// <returns>返回新ID和新序列值列表</returns>
        public static Task<List<long>> NewIDAndSeq(string p_seqName)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Da.NewIDAndSeq",
                p_seqName
            ).Call<List<long>>();
        }

        /// <summary>
        /// 产生含3位标志位的新ID
        /// </summary>
        /// <param name="p_flag">ID标志，取值范围0-7</param>
        /// <returns></returns>
        public static Task<long> NewFlagID(int p_flag)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Da.NewFlagID",
                p_flag
            ).Call<long>();
        }
        #endregion

        #region Ea
        /// <summary>
        /// 调用服务端Repo保存实体
        /// </summary>
        /// <param name="p_row">实体</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>true 成功</returns>
        public static Task<bool> SaveRow(Row p_row, string p_tblName)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Ea.SaveRow",
                p_row,
                p_tblName
            ).Call<bool>();
        }

        /// <summary>
        /// 调用服务端Repo批量保存实体
        /// </summary>
        /// <param name="p_entities">实体列表</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>true 成功</returns>
        public static Task<bool> SaveRows(Table p_entities, string p_tblName)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Ea.SaveRows",
                p_entities,
                p_tblName
            ).Call<bool>();
        }

        /// <summary>
        /// 调用服务端Repo删除实体
        /// </summary>
        /// <param name="p_row">实体</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>删除行数</returns>
        public static Task<int> DelRow(Row p_row, string p_tblName)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Ea.DelRow",
                p_row,
                p_tblName
            ).Call<int>();
        }

        /// <summary>
        /// 调用服务端Repo批量删除实体
        /// </summary>
        /// <param name="p_entities">实体列表</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>删除行数</returns>
        public static Task<int> DelRows(Table p_entities, string p_tblName)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Ea.DelRows",
                p_entities,
                p_tblName
            ).Call<int>();
        }

        /// <summary>
        /// 调用服务端Repo删除实体
        /// </summary>
        /// <param name="p_id">实体主键</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>删除行数</returns>
        public static Task<int> DelRowByKey(string p_id, string p_tblName)
        {
            return new UnaryRpc(
                typeof(TSrv).Name,
                "Ea.DelRowByKey",
                p_id,
                p_tblName
            ).Call<int>();
        }
        #endregion
    }
}
