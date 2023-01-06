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
            long p_atviID,
            DateTime p_date,
            bool p_isRole,
            long p_receiver,
            string p_note,
            bool p_isBack)
        {
            WfiItemObj item = new WfiItemObj(
                ID: await AtCm.NewID(),
                AtviID: p_atviID,
                AssignKind: (p_isBack ? WfiItemAssignKind.回退 : WfiItemAssignKind.普通指派),
                Status: WfiItemStatus.活动,
                IsAccept: false,
                Sender: Kit.UserName,
                Stime: p_date,
                Ctime: p_date,
                Mtime: p_date,
                Note: p_note,
                Dispidx: await AtCm.NewSeq("sq_wfi_item"));

            if (p_isRole)
                item.RoleID = p_receiver;
            else
                item.UserID = p_receiver;
            return item;
        }

        public void Finished()
        {
            Status = WfiItemStatus.结束;
            Mtime = Kit.Now;
            UserID = Kit.UserID;
        }
    }
}