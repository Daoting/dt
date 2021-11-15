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
    /// Describes an icon set conditional formatting rule.
    /// </summary>
    public interface IExcelColorScaleRule : IExcelConditionalFormatRule
    {
        /// <summary>
        /// Flag indicate whether it's a three points gradient scale or two point gradient scale.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this it's a three points gradient scale; otherwise, <see langword="false" />.
        /// </value>
        bool HasMiddleNode { get; set; }

        /// <summary>
        /// The values of the maximum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value> The values of the maximum of the interpolation points in a gradient scale.</value>
        IExcelConditionalFormatValueObject Maximum { get; set; }

        /// <summary>
        /// The color of the maximum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>The color of the maximum of the interpolation points in a gradient scale.</value>
        IExcelColor MaximumColor { get; set; }

        /// <summary>
        /// The values of the middle of the interpolation points in a gradient scale.
        /// </summary>
        /// <value> The values of the middle of the interpolation points in a gradient scale.</value>
        IExcelConditionalFormatValueObject Middle { get; set; }

        /// <summary>
        /// The color of the middle of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>The color of the middle of the interpolation points in a gradient scale.</value>
        IExcelColor MiddleColor { get; set; }

        /// <summary>
        /// The values of the minimum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value> The values of the minimum of the interpolation points in a gradient scale.</value>
        IExcelConditionalFormatValueObject Minimum { get; set; }

        /// <summary>
        /// The color of the minimum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>The color of the minimum of the interpolation points in a gradient scale.</value>
        IExcelColor MinimumColor { get; set; }
    }
}

