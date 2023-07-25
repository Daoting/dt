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
    public partial class WfiItemX
    {
        const string Sql后续活动工作项 = @"
select
	a.Is_Accept,
	a.Status,
	b.id atvi_id 
from
	cm_wfi_item a,
	cm_wfi_atv b 
where
	a.atvi_id = b.id 
	and b.atvd_id in ( select Tgt_Atv_ID from cm_wfd_trs d where d.Src_Atv_ID = {0} and d.Is_Rollback = 0 ) 
	and b.prci_id = {1}
";

        const string Sql最后工作项 = @"
select
	wi.id itemid,
	pi.prcd_id prcid 
from
	cm_wfi_item wi,
	cm_wfi_atv wa,
	cm_wfi_prc pi 
where
	wi.atvi_id = wa.id 
	and wa.prci_id = pi.id 
	and pi.id = {0}
order by
	wi.mtime desc
";

        const string Sql活动实例的工作项 = @"
select
	status,
	assign_kind,
	sender,
	usr.name recv,
	is_accept,
	wi.mtime 
from
	cm_wfi_item wi
	left join cm_user usr on wi.user_id = usr.id 
where
	atvi_id = {0}
order by
	dispidx
";

        const string Sql生成日志列表 = @"
select b.prci_id,
       b.id atvi_id,
       c.status prcistatus,
       d.name atvdname,
       a.assign_kind,
       a.is_accept,
       a.accept_time,
       a.status itemstatus,
       ( CASE user_id WHEN NULL THEN (select name from cm_role t where t.id = a.role_id) ELSE (select name from cm_user t where t.id = a.user_id) END ) username,
       a.note,
       a.ctime,
       a.mtime,
       c.mtime prcitime,
       a.sender
from cm_wfi_item a, cm_wfi_atv b, cm_wfi_prc c, cm_wfd_atv d
where a.atvi_id = b.id
      and b.prci_id = c.id
      and b.atvd_id = d.id
      and b.prci_id = {0}
      and ({1} = 0 or b.atvd_id = {1})
order by a.dispidx
";

        const string Sql日志目标项 = @"
select ( CASE username WHEN NULL THEN rolename ELSE username END ) accpname,
       atvdname,
       atvdtype,
       join_kind,
       atvi_id
from (select a.atvi_id,
             (select group_concat(name order by a.dispidx separator '、') from cm_user where id = a.user_id) as username,
             (select group_concat(name order by a.dispidx separator '、') from cm_role where id = a.role_id) as rolename,
             max(a.dispidx) dispidx,
             c.name as atvdname,
             c.type as atvdtype,
             c.join_kind
      from cm_wfi_item a,
           (select ti.Tgt_Atvi_ID id
               from cm_wfi_atv ai, cm_wfi_trs ti
               where ai.id = ti.Src_Atvi_ID
                 	and ai.prci_id = {0}
                 	and ti.Src_Atvi_ID = {1}) b,
           cm_wfd_atv c,
           cm_wfi_atv d
      where a.atvi_id = b.id
      	and b.id = d.id
      	and d.atvd_id = c.id
      group by a.atvi_id, c.name, c.type, c.join_kind) t
 order by dispidx
";
    }
}