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
    public partial class WfdTrsObj
    {
        public static async Task<WfdTrsObj> New(
            long PrcID = default,
            long SrcAtvID = default,
            long TgtAtvID = default,
            bool IsRollback = default,
            long? TrsID = default)
        {
            return new WfdTrsObj(
                ID: await NewID(),
                PrcID: PrcID,
                SrcAtvID: SrcAtvID,
                TgtAtvID: TgtAtvID,
                IsRollback: IsRollback,
                TrsID: TrsID);
        }
    }
}