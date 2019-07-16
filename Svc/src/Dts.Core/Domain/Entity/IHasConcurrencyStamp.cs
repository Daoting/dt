#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dts.Core
{
    public interface IHasConcurrencyStamp
    {
        string ConcurrencyStamp { get; set; }
    }
}
