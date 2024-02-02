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
    public partial class WfdTrsX
    {
        public static async Task<WfdTrsX> New(
            long PrcID = default,
            long SrcAtvID = default,
            long TgtAtvID = default,
            bool IsRollback = default,
            long? TrsID = default)
        {
            return new WfdTrsX(
                ID: await NewID(),
                PrcID: PrcID,
                SrcAtvID: SrcAtvID,
                TgtAtvID: TgtAtvID,
                IsRollback: IsRollback,
                TrsID: TrsID);
        }
    }
}