#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class WfdAtvX
    {
        const string Sql后续活动 = @"
select
	atv.* 
from
	cm_wfd_atv atv,
	( select trs.Tgt_Atv_ID atvid from cm_wfd_trs trs where trs.Src_Atv_ID = {0} and Is_Rollback = 0 ) trs 
where
	atv.id = trs.atvid
";
    }
}