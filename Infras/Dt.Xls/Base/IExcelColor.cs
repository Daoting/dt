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
    /// Defines color used for excel
    /// </summary>
    public interface IExcelColor : IEquatable<IExcelColor>
    {
        /// <summary>
        /// Gets the type of the color.
        /// </summary>
        /// <value>The type of the color.</value>
        ExcelColorType ColorType { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is auto color; otherwise, <see langword="false" />.
        /// </value>
        bool IsAutoColor { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is indexed color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is indexed color; otherwise, <see langword="false" />.
        /// </value>
        bool IsIndexedColor { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is RGB color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is RGB color; otherwise, <see langword="false" />.
        /// </value>
        bool IsRGBColor { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is theme color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is theme color; otherwise, <see langword="false" />.
        /// </value>
        bool IsThemeColor { get; }

        /// <summary>
        /// Gets the tint applied to the color.
        /// </summary>
        /// <value>The tint applied to the color</value>
        /// <remarks>
        /// If tint is supplied, then it is applied to the RGB value of the color to determine the final
        /// color applied. The tint value is stored as a double from -1.0 .. 1.0, where -1.0 means 100% darken and
        /// 1.0 means 100% lighten. Also, 0.0 means no change.
        /// </remarks>
        double Tint { get; }

        /// <summary>
        /// Gets the value of the color
        /// </summary>
        /// <value>The value of the color</value>
        uint Value { get; }
    }
}

