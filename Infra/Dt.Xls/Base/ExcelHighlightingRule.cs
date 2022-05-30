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
    /// Represents a highlighting rule
    /// </summary>
    public class ExcelHighlightingRule : IExcelHighlightingRule, IExcelConditionalFormatRule
    {
        private List<string> _formulas;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelHighlightingRule" /> class.
        /// </summary>
        public ExcelHighlightingRule()
        {
            this.Type = ExcelConditionalFormatType.CellIs;
        }

        /// <summary>
        /// Gets or sets the comparison operator of the conditional formatting rule.
        /// </summary>
        /// <value>The comparison operator of the conditional formatting rule.</value>
        public ExcelConditionalFormattingOperator ComparisonOperator { get; set; }

        /// <summary>
        /// Gets or sets the id of the differential formatting which is  used to locate the <see cref="T:Dt.Xls.IDifferentialFormatting" /> instance from the workbook.
        /// </summary>
        /// <value>The index of the differential formatting</value>
        public int DifferentialFormattingId { get; set; }

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
        /// The priority of the conditional formatting rule. The value is used to determine which format
        /// should be evaluated and rendered. Lower numeric values are higher priority than higher numeric values. where '1' is the highest priority.
        /// </summary>
        /// <value>The priority of the rule</value>
        public int Priority { get; set; }

        /// <summary>
        /// Flag indicates whether apply other lower priority rules when this rule evaluates to true.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if don't apply lower priority rules when this rule evaluates to true; otherwise, <see langword="false" />.
        /// </value>
        public bool StopIfTrue { get; set; }

        /// <summary>
        /// Gets or sets the type of the conditional formatting rule.
        /// </summary>
        /// <value>The type of the conditional formatting rule.</value>
        public ExcelConditionalFormatType Type { get; set; }
    }
}

