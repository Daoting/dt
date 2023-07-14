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
    /// <summary>
    /// 
    /// </summary>
    class WfdDs : DomainSvc<WfdDs, AtCm.Info>
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
            return _da.GetScalar<long>($"select ID from cm_wfd_trs where prcid={p_prcid} and SrcAtvID={p_srcAtvID} and TgtAtvID={p_tgtAtvID} and IsRollback={(p_isRollback ? 1 : 0)}");
        }
    }
}
