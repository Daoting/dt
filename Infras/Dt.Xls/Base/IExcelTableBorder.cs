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
    /// Express a single set of table border formats. (left, right, top, bottom). Color is optional, when missing, 'automatic' is implied.
    /// </summary>
    public interface IExcelTableBorder : IExcelBorder, IEquatable<IExcelBorder>
    {
        /// <summary>
        /// Gets or sets the horizontal border line
        /// </summary>
        /// <value>The horizontal border line</value>
        IExcelBorderSide Horizontal { get; set; }

        /// <summary>
        /// Gets or sets the vertical border line
        /// </summary>
        /// <value>The vertical border line</value>
        IExcelBorderSide Vertical { get; set; }
    }
}

