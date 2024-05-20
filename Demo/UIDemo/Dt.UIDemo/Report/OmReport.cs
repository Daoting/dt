#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.UIDemo
{
    public partial class OmReportX
    {
        public static async Task<OmReportX> New(string Name, string Define)
        {
            return new OmReportX(
                ID: await NewID(),
                Name: Name,
                Define: Define);
        }
    }
}