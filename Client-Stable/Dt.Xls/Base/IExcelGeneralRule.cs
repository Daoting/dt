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
    /// Represent a general conditional formatting rule.
    /// </summary>
    public interface IExcelGeneralRule : IExcelConditionalFormatRule
    {
        /// <summary>
        /// Indicates whether the rule is an "above average" rule. '1' indicates 'above average'. This
        /// attribute is ignored if type is not equal to aboveAverage.
        /// </summary>
        /// <value>The above average.</value>
        bool? AboveAverage { get; set; }

        /// <summary>
        /// Indicates whether the "top/bottom n" rule is a "bottom n" rule. '1' indicates 'bottom'. This
        /// attribute is ignored if type is not equal to top10.
        /// </summary>
        /// <value>True if the "top/bottom n" rule is a "bottom n" rule.</value>
        bool? Bottom { get; set; }

        /// <summary>
        /// Gets or sets the id of the differential formatting which is  used to locate the <see cref="T:Dt.Xls.IDifferentialFormatting" /> instance from the workbook.
        /// </summary>
        /// <value>The index of the differential formatting</value>
        int DifferentialFormattingId { get; set; }

        /// <summary>
        /// Flag indicating whether the 'aboveAverage' and 'belowAverage' criteria is inclusive of the
        /// average itself, or exclusive of that value. '1' indicates to include the average value in the
        /// criteria. This attribute is ignored if type is not equal to aboveAverage.
        /// </summary>
        /// <value>The equal average.</value>
        bool? EqualAverage { get; set; }

        /// <summary>
        /// Gets or sets the formulas used to evaluates whether this rule should be applied.
        /// </summary>
        /// <value>The formulas used to evaluate this rule</value>
        List<string> Formulas { get; set; }

        /// <summary>
        /// The operator in the "cell value is" conditional formatting rule. This attribute is ignored if
        /// type is not equal to cellIs
        /// </summary>
        /// <value>The operator.</value>
        ExcelConditionalFormattingOperator? Operator { get; set; }

        /// <summary>
        /// Indicates whether the "top/bottom n" rule is a "top/bottom n percent" rule. This attribute
        /// is ignored if type is not equal to top10.
        /// </summary>
        /// <value>True is the "top/bottom n" rule is a "top/bottom n percent" rule</value>
        bool? Percent { get; set; }

        /// <summary>
        /// The value of "n" in the "top/bottom n" conditional formatting rule. This attribute is ignored
        /// if type is not equal to top10.
        /// </summary>
        /// <value>The value of "n" in the "top/bottom n" conditional formatting rule.</value>
        int? Rank { get; set; }

        /// <summary>
        /// The number of standard deviations to include above or below the average in the
        /// conditional formatting rule. This attribute is ignored if type is not equal to
        /// aboveAverage. If a value is present for stdDev and the rule type = aboveAverage, then
        /// this rule is automatically an "above or below N standard deviations" rule.
        /// </summary>
        /// <value>The value of the standard deviations</value>
        int? StdDev { get; set; }

        /// <summary>
        /// The text value in a "text contains" conditional formatting rule. This attribute is ignored if
        /// type is not equal to containsText.
        /// </summary>
        /// <value>The text value.</value>
        string Text { get; set; }

        /// <summary>
        /// Type of conditional formatting rule.
        /// </summary>
        /// <value>Type of conditional formatting rule.</value>
        new ExcelConditionalFormatType Type { get; set; }
    }
}

