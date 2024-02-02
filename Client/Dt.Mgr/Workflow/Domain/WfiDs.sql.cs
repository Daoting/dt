#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-11 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    partial class WfiDs
    {
        const string Sql前一活动的执行者 = @"
select distinct user_id from cm_wfi_item
where atvi_id in (
  select id from cm_wfi_atv
	where prci_id = {0}
		and atvd_id in (select Src_Atv_ID from cm_wfd_trs where Tgt_Atv_ID={1}))
";

        const string Sql前一活动的同部门执行者 = @"

";

        const string Sql已完成活动的执行者 = @"
select distinct user_id from cm_wfi_item
where
	atvi_id in ( select id from cm_wfi_atv where prci_id = {0} and atvd_id = {1} )
";

        const string Sql已完成活动同部门执行者 = @"

";
    }
}