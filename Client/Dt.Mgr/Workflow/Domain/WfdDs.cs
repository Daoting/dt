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
    class WfdDs : DomainSvc<WfdDs, AtCm>
    {
        WfdDs() { }

        public async Task<bool> SavePrc(WfdPrcX p_prc)
        {
            await Save(p_prc);
            await Save(p_prc.Atvs);
            await Save(p_prc.Trss);
            await Save(p_prc.AtvRoles);
            return await Commit();
        }

    }
}
