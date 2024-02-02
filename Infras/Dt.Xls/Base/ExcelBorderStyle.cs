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
    /// Represent the border line style.
    /// </summary>
    public enum ExcelBorderStyle : byte
    {
        /// <summary>
        /// Dash Dot.
        /// </summary>
        DashDot = 9,
        /// <summary>
        /// Dash Dot Dot.
        /// </summary>
        DashDotDot = 11,
        /// <summary>
        /// Dashed.
        /// </summary>
        Dashed = 3,
        /// <summary>
        /// Dotted.
        /// </summary>
        Dotted = 4,
        /// <summary>
        /// Double Line.
        /// </summary>
        Double = 6,
        /// <summary>
        /// Hairline Border.
        /// </summary>
        Hair = 7,
        /// <summary>
        /// Medium Border.
        /// </summary>
        Medium = 2,
        /// <summary>
        /// Medium Dash Dot.
        /// </summary>
        MediumDashDot = 10,
        /// <summary>
        /// Medium Dash Dot Dot.
        /// </summary>
        MediumDashDotDot = 12,
        /// <summary>
        /// Medium Dashed.
        /// </summary>
        MediumDashed = 8,
        /// <summary>
        /// No border. 
        /// </summary>
        None = 0,
        /// <summary>
        /// Slant Dash Dot.
        /// </summary>
        SlantDashDot = 13,
        /// <summary>
        /// Thick Line Border.
        /// </summary>
        Thick = 5,
        /// <summary>
        /// Thin Border. 
        /// </summary>
        Thin = 1
    }
}

