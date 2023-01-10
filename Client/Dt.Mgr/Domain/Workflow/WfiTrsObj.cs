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
    public partial class WfiTrsObj
    {
        public static async Task<WfiTrsObj> New(
            long TrsdID = default,
            long SrcAtviID = default,
            long TgtAtviID = default,
            bool IsRollback = default,
            DateTime Ctime = default)
        {
            return new WfiTrsObj(
                ID: await NewID(),
                TrsdID: TrsdID,
                SrcAtviID: SrcAtviID,
                TgtAtviID: TgtAtviID,
                IsRollback: IsRollback,
                Ctime: Ctime);
        }
    }
}