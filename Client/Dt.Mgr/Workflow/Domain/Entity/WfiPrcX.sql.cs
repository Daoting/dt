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
    public partial class WfiPrcX
    {
        const string Sql查询实例 = @"
select * from cm_wfi_prc
where
	prcd_id = @prcdid 
	and ( @status > 2 or status = @status )
	and ( @title = '' or name = @title )
	and ( @start < '1900-01-01' or Mtime >= @start )
	and ( @end < '1900-01-01' or Mtime <= @end )
order by
	dispidx
";
    }
}