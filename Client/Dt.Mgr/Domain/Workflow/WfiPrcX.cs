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
    public partial class WfiPrcX
    {
        public static async Task<WfiPrcX> New(
            long PrcdID = default,
            string Name = default,
            WfiPrcStatus Status = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            long id = await NewID();
            return new WfiPrcX(id, PrcdID, Name, Status, Dispidx, Ctime, Mtime);
        }

        protected override void InitHook()
        {
        }
    }
}