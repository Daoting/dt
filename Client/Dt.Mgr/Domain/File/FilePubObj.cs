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
    public partial class FilePubObj
    {
        public static async Task<FilePubObj> New(
           long? ParentID = default,
           string Name = default,
           bool IsFolder = default,
           string ExtName = default,
           string Info = default,
           DateTime Ctime = default)
        {
            return new FilePubObj(
                ID: await NewID(),
                ParentID: ParentID,
                Name: Name,
                IsFolder: IsFolder,
                ExtName: ExtName,
                Info: Info,
                Ctime: Ctime);
        }

        protected override void InitHook()
        {
        }
    }
}