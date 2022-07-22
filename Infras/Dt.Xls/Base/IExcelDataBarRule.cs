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
    /// Describes a data bar conditional formatting rule.
    /// </summary>
    public interface IExcelDataBarRule : IExcelConditionalFormatRule
    {
        /// <summary>
        /// Gets or sets the color of the data bar.
        /// </summary>
        /// <value>The color of the data bar.</value>
        IExcelColor Color { get; set; }

        /// <summary>
        /// The values of the maximum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The values of the maximum of the interpolation points in a gradient scale.
        /// </value>
        IExcelConditionalFormatValueObject Maximum { get; set; }

        /// <summary>
        /// The maximum length of the data bar, as a percentage of the cell width.
        /// </summary>
        /// <value>The maximum length of the data bar, as a percentage of the cell width.</value>
        byte MaximumDataBarLength { get; set; }

        /// <summary>
        /// The values of the minimum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The values of the minimum of the interpolation points in a gradient scale.
        /// </value>
        IExcelConditionalFormatValueObject Minimum { get; set; }

        /// <summary>
        /// The minimum length of the data bar, as a percentage of the cell width.
        /// </summary>
        /// <value>The minimum length of the data bar, as a percentage of the cell width.</value>
        byte MinimumDataBarLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data bar grows from right to left.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the data bar grows from right to left; otherwise, <see langword="false" />.
        /// </value>
        bool RightToLeft { get; set; }

        /// <summary>
        /// Indicates whether to show the values of the cells on which this data bar is applied.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show the values of the cells on which this data bar is applied; otherwise, <see langword="false" />.
        /// </value>
        bool ShowValue { get; set; }
    }
}

