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
            var sql = await AtCm.GetDbType() == DatabaseType.PostgreSql ? Sql日志目标项_pg : Sql日志目标项;
            return await Query(string.Format(sql, p_prciID, p_atviID));
        }

        public void Finished()
        {
            Status = WfiItemStatus.结束;
            Mtime = Kit.Now;
            UserID = Kit.UserID;
        }
    }
}