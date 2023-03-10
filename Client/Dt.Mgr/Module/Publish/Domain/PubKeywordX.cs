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

namespace Dt.Mgr.Module
{
    public partial class PubKeywordX
    {
        public static PubKeywordX New(string ID = "新关键字")
        {
            return new PubKeywordX(
                ID: ID,
                Creator: Kit.UserName,
                Ctime: Kit.Now);
        }
    }
}