#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// An interface used to represents custom or built-in function used in Excel formula
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// The custom or built-in function name
        /// </summary>
        string Name { get; }
    }
}

