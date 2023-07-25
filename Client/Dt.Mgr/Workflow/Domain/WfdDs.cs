#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Mgr.Workflow
{
    partial class WfdDs : DomainSvc<WfdDs, AtCm.Info>
    {
        public static async Task<bool> SavePrc(WfdPrcX p_prc)
        {
            var w = _da.NewWriter();
            await w.Save(p_prc);
            await w.Save(p_prc.Atvs);
            await w.Save(p_prc.Trss);
            await w.Save(p_prc.AtvRoles);
            return await w.Commit();
        }

        public static Task<long> GetWfdTrsID(long p_prcid, long p_srcAtvID, long p_tgtAtvID, bool p_isRollback)
        {
            return _da.GetScalar<long>($"select ID from cm_wfd_trs where prc_id={p_prcid} and src_atv_id={p_srcAtvID} and tgt_atv_id={p_tgtAtvID} and is_rollback={(p_isRollback ? 1 : 0)}");
        }

        public static Task<Table> GetAllMyPrc()
        {
            return _da.Query(string.Format(Sql参与的流程, Kit.UserID));
        }

        public static Task<Table> GetMyStartablePrc()
        {
            return _da.Query(string.Format(Sql可启动流程, Kit.UserID));
        }

        public static Task<Table> GetMyTodoTasks()
        {
            return _da.Query(string.Format(Sql待办任务, Kit.UserID));
        }

        public static Task<int> GetMyTotalTodoTasks()
        {
            return _da.GetScalar<int>(string.Format(Sql待办任务总数, Kit.UserID));
        }

        public static async Task<Table> GetMyHistoryPrcs(bool p_allItems, DateTime p_start, DateTime p_end, int p_status)
        {
            var dt = new { p_userid = Kit.UserID, p_start = p_start, p_end = p_end, p_status = p_status };

            string sql = p_allItems? Sql所有经办历史任务 : Sql历史任务;
            var db = await _da.GetDbType();
            if (db == DatabaseType.Oracle)
                sql = sql.Replace("@", ":");
                        
            return await _da.Query(sql, dt);
        }
    }
}
