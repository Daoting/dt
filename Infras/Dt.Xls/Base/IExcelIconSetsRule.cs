#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines an icon set conditional formatting rule.
    /// </summary>
    public interface IExcelIconSetsRule : IExcelConditionalFormatRule
    {
        /// <summary>
        /// Gets or sets a value indicating whether show icon only
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show icon only; otherwise, <see langword="false" />.
        /// </value>
        bool IconOnly { get; set; }

        /// <summary>
        /// Gets or sets the icon set.
        /// </summary>
        /// <value>The icon set.</value>
        ExcelIconSetType IconSet { get; set; }

        /// <summary>
        /// Gets or sets the not pass the thresholds when equals.
        /// </summary>
        /// <value>The not pass the thresholds when equals.</value>
        List<bool> NotPassTheThresholdsWhenEquals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reverse the default order of the icons
        /// </summary>
        /// <value>
        /// <see langword="true" /> if reverse the default order of the icons; otherwise, <see langword="false" />.
        /// </value>
        bool ReversedOrder { get; set; }

        /// <summary>
        /// Gets or sets the thresholds.
        /// </summary>
        /// <value>The thresholds.</value>
        List<IExcelConditionalFormatValueObject> Thresholds { get; set; }
    }
}

