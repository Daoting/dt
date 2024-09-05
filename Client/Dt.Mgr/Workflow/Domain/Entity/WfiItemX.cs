#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class WfiItemX
    {
        public static async Task<WfiItemX> New(
            long AtviID,
            WfiItemStatus Status = default,
            WfiItemAssignKind AssignKind = default,
            long? SenderID = default,
            string Sender = default,
            DateTime Stime = default,
            bool IsAccept = default,
            DateTime? AcceptTime = default,
            long? RoleID = default,
            long? UserID = default,
            string Note = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            return new WfiItemX(
                ID: await NewID(),
                AtviID: AtviID,
                Status: Status,
                AssignKind: AssignKind,
                SenderID: SenderID,
                Sender: Sender,
                Stime: Stime,
                IsAccept: IsAccept,
                AcceptTime: AcceptTime,
                RoleID: RoleID,
                UserID: UserID,
                Note: Note,
                Dispidx: await NewSeq("Dispidx"),
                Ctime: Ctime,
                Mtime: Mtime);
        }

        public static Task<Table<WfiItemX>> GetNextItems(long p_atvdid, long p_prciid)
        {
            return Query(string.Format(Sql后续活动工作项, p_atvdid, p_prciid));
        }

        public static async Task<WfiItemX> GetLastItem(long p_prciid)
        {
            return await First(string.Format(Sql最后工作项, p_prciid));
        }

        public static Task<Table<WfiItemX>> GetItemsOfAtvi(long p_atviid)
        {
            return Query(string.Format(Sql活动实例的工作项, p_atviid));
        }

        public static async Task<Table> GetLogList(long p_prciID, long p_atvdID = 0)
        {
            return await Query(string.Format(Sql生成日志列表, p_prciID, p_atvdID));
        }

        public static async Task<Table> GetAtviLog(long p_prciID, long p_atviID = 0)
        {
            var sql = await At.GetDbType() == DatabaseType.PostgreSql ? Sql日志目标项_pg : Sql日志目标项;
            return await Query(string.Format(sql, p_prciID, p_atviID));
        }

        public void Finished()
        {
            Status = WfiItemStatus.结束;
            Mtime = Kit.Now;
            UserID = Kit.UserID;
        }

        #region Sql
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
	wi.*
from
	cm_wfi_item wi,
	cm_wfi_atv wa,
	cm_wfi_prc pi 
where
	wi.atvi_id = wa.id
	and wa.prci_id = pi.id 
	and pi.id = {0}
order by
	wi.dispidx desc
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
        #endregion
    }
}