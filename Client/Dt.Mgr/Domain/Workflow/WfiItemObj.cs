#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    public partial class WfiItemObj
    {
        public static async Task<WfiItemObj> New(
            long AtviID,
            WfiItemStatus Status = default,
            WfiItemAssignKind AssignKind = default,
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
            return new WfiItemObj(
                ID: await NewID(),
                AtviID: AtviID,
                Status: Status,
                AssignKind: AssignKind,
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

        public void Finished()
        {
            Status = WfiItemStatus.结束;
            Mtime = Kit.Now;
            UserID = Kit.UserID;
        }
    }
}