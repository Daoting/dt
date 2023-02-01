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
    public partial class WfiTrsX
    {
        public static async Task<WfiTrsX> New(
            long TrsdID = default,
            long SrcAtviID = default,
            long TgtAtviID = default,
            bool IsRollback = default,
            DateTime Ctime = default)
        {
            return new WfiTrsX(
                ID: await NewID(),
                TrsdID: TrsdID,
                SrcAtviID: SrcAtviID,
                TgtAtviID: TgtAtviID,
                IsRollback: IsRollback,
                Ctime: Ctime);
        }
    }
}