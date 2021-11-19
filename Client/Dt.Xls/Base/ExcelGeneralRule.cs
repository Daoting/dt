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
    /// A class implements <see cref="T:Dt.Xls.IExcelGeneralRule" /> represents a general conditional formatting rule.
    /// </summary>
    public class ExcelGeneralRule : IExcelGeneralRule, IExcelConditionalFormatRule
    {
        private List<string> _formulas;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelGeneralRule" /> class.
        /// </summary>
        /// <param name="cfType">The type of this conditional formatting rule</param>
        public ExcelGeneralRule(ExcelConditionalFormatType cfType)
        {
            this.Type = cfType;
        }

        /// <summary>
        /// Indicates whether the rule is an "above average" rule. '1' indicates 'above average'. This
        /// attribute is ignored if type is not equal to aboveAverage.
        /// </summary>
        /// <value>The above average.</value>
        public bool? AboveAverage { get; set; }

        /// <summary>
        /// Indicates whether the "top/bottom n" rule is a "bottom n" rule. '1' indicates 'bottom'. This
        /// attribute is ignored if type is not equal to top10.
        /// </summary>
        /// <value>True if the "top/bottom n" rule is a "bottom n" rule.</value>
        public bool? Bottom { get; set; }

        /// <summary>
        /// Gets or sets the DXF id used to locate the <see cref="T:Dt.Xls.IDifferentialFormatting" /> from the workbook.
        /// </summary>
        /// <value>The id of the DXF</value>
        public int DifferentialFormattingId { get; set; }

        /// <summary>
        /// Flag indicating whether the 'aboveAverage' and 'belowAverage' criteria is inclusive of the
        /// average itself, or exclusive of that value. '1' indicates to include the average value in the
        /// criteria. This attribute is ignored if type is not equal to aboveAverage.
        /// </summary>
        /// <value>The equal average.</value>
        public bool? EqualAverage { get; set; }

        /// <summary>
        /// Gets or sets the formulas used to evaluates whether this rule should be applied.
        /// </summary>
        /// <value>The formulas used to evaluate this rule</value>
        public List<string> Formulas
        {
            get
            {
                if (this._formulas == null)
                {
                    this._formulas = new List<string>();
                }
                return this._formulas;
            }
            set { this._formulas = value; }
        }

        /// <summary>
        /// The operator in the "cell value is" conditional formatting rule. This attribute is ignored if
        /// type is not equal to cellIs
        /// </summary>
        /// <value>The operator.</value>
        public ExcelConditionalFormattingOperator? Operator { get; set; }

        /// <summary>
        /// Indicates whether the "top/bottom n" rule is a "top/bottom n percent" rule. This attribute
        /// is ignored if type is not equal to top10.
        /// </summary>
        /// <value>True is the "top/bottom n" rule is a "top/bottom n percent" rule</value>
        public bool? Percent { get; set; }

        /// <summary>
        /// The priority of the conditional formatting rule. The value is used to determine which format
        /// should be evaluated and rendered. Lower numeric values are higher priority than higher numeric values. where '1' is the highest priority.
        /// </summary>
        /// <value>The priority of the rule</value>
        public int Priority { get; set; }

        /// <summary>
        /// The value of "n" in the "top/bottom n" conditional formatting rule. This attribute is ignored
        /// if type is not equal to top10.
        /// </summary>
        /// <value>
        /// The value of "n" in the "top/bottom n" conditional formatting rule.
        /// </value>
        public int? Rank { get; set; }

        /// <summary>
        /// The number of standard deviations to include above or below the average in the
        /// conditional formatting rule. This attribute is ignored if type is not equal to
        /// aboveAverage. If a value is present for stdDev and the rule type = aboveAverage, then
        /// this rule is automatically an "above or below N standard deviations" rule.
        /// </summary>
        /// <value>The value of the standard deviations</value>
        public int? StdDev { get; set; }

        /// <summary>
        /// Flag indicates whether apply other lower priority rules when this rule evaluates to true.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if don't apply lower priority rules when this rule evaluates to true; otherwise, <see langword="false" />.
        /// </value>
        public bool StopIfTrue { get; set; }

        /// <summary>
        /// The text value in a "text contains" conditional formatting rule. This attribute is ignored if
        /// type is not equal to containsText.
        /// </summary>
        /// <value>The text value.</value>
        public string Text { get; set; }

        /// <summary>
        /// Type of conditional formatting rule.
        /// </summary>
        /// <value>Type of conditional formatting rule.</value>
        public ExcelConditionalFormatType Type { get; set; }
    }
}

