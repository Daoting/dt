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
    /// Express a single set of cell border formats. (left, right, top, bottom). Color is optional, when missing, 'automatic' is implied.
    /// </summary>
    public interface IExcelBorder : IEquatable<IExcelBorder>
    {
        /// <summary>
        /// Clones this instance and create a new <see cref="T:Dt.Xls.IExcelBorder" /> instance
        /// </summary>
        /// <returns></returns>
        IExcelBorder Clone();

        /// <summary>
        /// Gets or sets the bottom border line
        /// </summary>
        /// <value>The bottom border line</value>
        IExcelBorderSide Bottom { get; set; }

        /// <summary>
        /// Gets or sets the left border line
        /// </summary>
        /// <value>The left border line</value>
        IExcelBorderSide Left { get; set; }

        /// <summary>
        /// Gets or sets the right border line
        /// </summary>
        /// <value>The right border line</value>
        IExcelBorderSide Right { get; set; }

        /// <summary>
        /// Gets or sets the top border line
        /// </summary>
        /// <value>The top border line</value>
        IExcelBorderSide Top { get; set; }
    }
}

