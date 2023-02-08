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

namespace Dt.MgrDemo.CrudSqlite
{
    public partial class ChildTbl2X
    {
        public static async Task<ChildTbl2X> New(
            long GroupID = default,
            string ItemName = default)
        {
            return new ChildTbl2X(
                ID: await NewID(),
                GroupID: GroupID,
                ItemName: ItemName);
        }

        protected override void InitHook()
        {
        }
    }
}