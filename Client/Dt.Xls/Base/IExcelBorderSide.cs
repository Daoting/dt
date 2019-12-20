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
    /// Represents border side used in excel border
    /// </summary>
    public interface IExcelBorderSide : IEquatable<IExcelBorderSide>
    {
        /// <summary>
        /// Gets or sets the color of the border line
        /// </summary>
        /// <value>The color of the border line</value>
        IExcelColor Color { get; set; }

        /// <summary>
        /// Gets or sets the border line style.
        /// </summary>
        /// <value>The border line style.</value>
        ExcelBorderStyle LineStyle { get; set; }
    }
}

