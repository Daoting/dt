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
    partial class WfdDs
    {
        const string Sql参与的流程 = @"
select distinct(p.id),p.name,p.dispidx
from
	cm_wfd_prc p,
	cm_wfd_atv a,
	cm_wfd_atv_role r,
	cm_user_role u 
where
	p.id = a.prc_id 
	and a.id = r.atv_id 
	and ( r.role_id = u.role_id or r.role_id = 1 ) 
	and u.user_id = {0}
order by
	p.dispidx
";

        const string Sql可启动流程 = @"
select
	pd.id,
	name 
from
	cm_wfd_prc pd,
	(
select distinct
	p.id 
from
	cm_wfd_prc p,
	cm_wfd_atv a,
	cm_wfd_atv_role r,
	cm_user_role u 
where
	p.id = a.prc_id 
	and a.id = r.atv_id 
	and ( r.role_id = u.role_id or r.role_id = 1 ) 
	and u.user_id = {0}
	and p.is_locked = '0'
	and a.type = 1 
	) pa 
where
	pd.id = pa.id 
order by
	dispidx
";

        // 用户只能看到一个流程实例的最后完成的任务
        const string Sql历史任务 = @"
select * from
(select wi.id item_id,
       pi.id prci_id,
       pd.id prcd_id,
       ad.id atvd_id,
       ai.id atvi_id,
       pd.name prcname,
       ad.name atvname,
       pi.status,
       pi.name formname,
       wi.sender,
       wi.stime,
       wi.mtime mtime,
       wi.reCount,
       rank() over ( partition by pi.id order by wi.stime desc ) no
from cm_wfi_atv ai,
     cm_wfi_prc pi,
     cm_wfd_atv ad,
     cm_wfd_prc pd,
     (select id,
             atvi_id,
             mtime,
             sender,
             stime,
             (select count(1)
              from cm_wfi_item
              where atvi_id = t.atvi_id
                    and Assign_Kind = 4
                    and id <> t.id) as reCount
     	from cm_wfi_item t
     	where status = 1
     	      and user_id = @p_userid
     	      and (@p_start < '1900-01-01' or mtime >= @p_start)
     	      and (@p_end < '1900-01-01' or mtime <= @p_end)
     	      order by mtime desc) wi
 where wi.atvi_id = ai.id
       and ai.prci_id = pi.id
       and pi.prcd_id = pd.id
       and ai.atvd_id = ad.id
       and wi.reCount = 0
       and (@p_status > 2 or pi.status = @p_status)) t
 where t.no=1 {0}
";

        // 用户在一个流程实例中参与的所有任务
        const string Sql所有经办历史任务 = @"
select wi.id item_id,
       pi.id prci_id,
       pd.id prcd_id,
       ad.id atvd_id,
       ai.id atvi_id,
       pd.name prcname,
       ad.name atvname,
       pi.status,
       pi.name formname,
       wi.sender,
       wi.stime,
       wi.mtime,
       wi.reCount
from cm_wfi_atv ai,
     cm_wfi_prc pi,
     cm_wfd_atv ad,
     cm_wfd_prc pd,
     (select id,
             atvi_id,
             mtime,
             sender,
             stime,
             (select count(1)
              from cm_wfi_item
              where atvi_id = t.atvi_id
                    and Assign_Kind = 4
                    and id <> t.id) as reCount
     	from cm_wfi_item t
     	where status = 1
     	      and user_id = @p_userid
     	      and (@p_start < '1900-01-01' or mtime >= @p_start)
     	      and (@p_end < '1900-01-01' or mtime <= @p_end)) wi
 where wi.atvi_id = ai.id
       and ai.prci_id = pi.id
       and pi.prcd_id = pd.id
       and ai.atvd_id = ad.id
       and (@p_status > 2 or pi.status = @p_status) {0}
 order by wi.stime desc
";

        const string Sql待办任务 = @"
select wi.id   item_id,
       pi.id   prci_id,
       pd.id   prcd_id,
       pd.name prcname,
       ad.name atvname,
       pi.name formname,
       wi.assign_kind,
       wi.sender,
       wi.stime,
       wi.is_accept
from cm_wfi_atv ai,
     cm_wfd_atv ad,
     cm_wfi_prc pi,
     cm_wfd_prc pd,
     (select id,
             atvi_id,
             sender,
             stime,
             is_accept,
             assign_kind
      from cm_wfi_item wi
      where status = 0
      	and (user_id = {0} or
      		(user_id is null and
      		(exists (select 1
      				 from cm_user_role
      				 where wi.role_id = role_id
      					   and user_id = {0})) or
      	    role_id = 1))) wi
where ai.id = wi.atvi_id
 and ai.atvd_id = ad.id
 and ai.prci_id = pi.id
 and pi.prcd_id = pd.id
order by wi.stime desc
";

        const string Sql待办任务总数 = @"
select
	sum( 1 ) allTask 
from
	cm_wfi_prc a,
	cm_wfi_atv b,
	cm_wfi_item c 
where
	a.id = b.prci_id 
	and b.id = c.atvi_id 
	and c.status = 0 
	and 
	(
		c.user_id = {0} 
		or ( user_id is null and exists ( select 1 from cm_user_role where c.role_id = role_id and user_id = {0} ) ) 
	)
";
    }
}