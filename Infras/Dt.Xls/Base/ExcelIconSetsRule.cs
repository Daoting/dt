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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Describes an icon set conditional formatting rule.
    /// </summary>
    public class ExcelIconSetsRule : IExcelIconSetsRule, IExcelConditionalFormatRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.IExcelIconSetsRule" /> instance.
        /// </summary>
        public ExcelIconSetsRule()
        {
            this.Thresholds = new List<IExcelConditionalFormatValueObject>();
            this.NotPassTheThresholdsWhenEquals = new List<bool>();
            this.Type = ExcelConditionalFormatType.IconSet;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show icon only
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show icon only; otherwise, <see langword="false" />.
        /// </value>
        public bool IconOnly { get; set; }

        /// <summary>
        /// Gets or sets the icon set.
        /// </summary>
        /// <value>The icon set.</value>
        public ExcelIconSetType IconSet { get; set; }

        /// <summary>
        /// Gets or sets the not pass the thresholds when equals.
        /// </summary>
        /// <value>The not pass the thresholds when equals.</value>
        public List<bool> NotPassTheThresholdsWhenEquals { get; set; }

        /// <summary>
        /// The priority of the conditional formatting rule. The value is used to determine which format
        /// should be evaluated and rendered. Lower numeric values are higher priority than higher numeric values. where '1' is the highest priority.
        /// </summary>
        /// <value>The priority of the rule</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reverse the default order of the icons
        /// </summary>
        /// <value>
        /// <see langword="true" /> if reverse the default order of the icons; otherwise, <see langword="false" />.
        /// </value>
        public bool ReversedOrder { get; set; }

        /// <summary>
        /// Flag indicates whether apply other lower priority rules when this rule evaluates to true.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if don't apply lower priority rules when this rule evaluates to true; otherwise, <see langword="false" />.
        /// </value>
        public bool StopIfTrue { get; set; }

        /// <summary>
        /// Gets or sets the thresholds.
        /// </summary>
        /// <value>The thresholds.</value>
        public List<IExcelConditionalFormatValueObject> Thresholds { get; set; }

        /// <summary>
        /// Type of conditional formatting rule.
        /// </summary>
        /// <value>Type of conditional formatting rule.</value>
        public ExcelConditionalFormatType Type { get; private set; }
    }
}

