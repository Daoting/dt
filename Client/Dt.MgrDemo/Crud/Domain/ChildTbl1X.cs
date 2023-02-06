#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Crud
{
    public partial class ChildTbl1X
    {
        public static async Task<ChildTbl1X> New(
            long ParentID = default,
            string ItemName = default)
        {
            return new ChildTbl1X(
                ID: await NewID(),
                ParentID: ParentID,
                ItemName: ItemName);
        }

        protected override void InitHook()
        {
        }
    }
}