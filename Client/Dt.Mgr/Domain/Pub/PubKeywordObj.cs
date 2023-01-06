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
    public partial class PubKeywordObj
    {
        public static PubKeywordObj New(string ID = "新关键字")
        {
            return new PubKeywordObj(
                ID: ID,
                Creator: Kit.UserName,
                Ctime: Kit.Now);
        }
    }
}