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
	and b.atvd_id in ( select Tgt_Atv_ID from cm_wfd_trs d where d.Src_Atv_ID = {0} and d.Is_Rollback = '0' ) 
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
	coalesce(usr.name, usr.acc, usr.phone) as recv,
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
       ( case when user_id is null then (select name from cm_role t where t.id = a.role_id) else (select coalesce(name, acc, phone) from cm_user t where t.id = a.user_id) end ) username,
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
select ( case username when null then rolename else username end ) accpname,
       atvdname,
       atvdtype,
       join_kind,
       atvi_id
from 
	(
	select  a.atvi_id,
			max(d.name) as atvdname,
			max(d.type) as atvdtype,
			max(d.join_kind) as join_kind,
			group_concat(coalesce(u.name, u.acc, u.phone) separator '、') as username,
			group_concat(r.name separator '、') as rolename,
			max(a.dispidx) as dispidx
	from
		cm_wfi_item a
		join
		    (
		    select
		    	ti.tgt_atvi_id id 
		    from
		    	cm_wfi_atv ai,
		    	cm_wfi_trs ti 
		    where
		    	ai.id = ti.src_atvi_id 
		    	and ai.prci_id = {0}
		    	and ti.Src_Atvi_ID = {1}
		    ) b on a.atvi_id = b.id
		join cm_wfi_atv c on b.id=c.id
		join cm_wfd_atv d on c.atvd_id=d.id
		left join cm_user u on a.user_id = u.id
		left join cm_role r on a.role_id = r.id
	group by a.atvi_id
	) t
order by dispidx
";

        const string Sql日志目标项_pg = @"
select ( case username when null then rolename else username end ) accpname,
       atvdname,
       atvdtype,
       join_kind,
       atvi_id
from 
	(
	select  a.atvi_id,
			max(d.name) as atvdname,
			max(d.type) as atvdtype,
			max(d.join_kind) as join_kind,
			array_to_string(array_agg(coalesce(u.name, u.acc, u.phone)), '、') as username,
			array_to_string(array_agg(r.name), '、') as rolename,
			max(a.dispidx) as dispidx
	from
		cm_wfi_item a
		join
		    (
		    select
		    	ti.tgt_atvi_id id 
		    from
		    	cm_wfi_atv ai,
		    	cm_wfi_trs ti 
		    where
		    	ai.id = ti.src_atvi_id 
		    	and ai.prci_id = {0}
		    	and ti.Src_Atvi_ID = {1}
		    ) b on a.atvi_id = b.id
		join cm_wfi_atv c on b.id=c.id
		join cm_wfd_atv d on c.atvd_id=d.id
		left join cm_user u on a.user_id = u.id
		left join cm_role r on a.role_id = r.id
	group by a.atvi_id
	) t
order by dispidx
";
    }
}