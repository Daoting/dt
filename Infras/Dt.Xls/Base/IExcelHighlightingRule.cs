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
    /// Defines a highlighting rule
    /// </summary>
    public interface IExcelHighlightingRule : IExcelConditionalFormatRule
    {
        /// <summary>
        /// Gets or sets the comparison operator of the conditional formatting rule.
        /// </summary>
        /// <value>The comparison operator of the conditional formatting rule.</value>
        ExcelConditionalFormattingOperator ComparisonOperator { get; set; }

        /// <summary>
        /// Gets or sets the id of the differential formatting which is  used to locate the <see cref="T:Dt.Xls.IDifferentialFormatting" /> instance from the workbook.
        /// </summary>
        /// <value>The index of the differential formatting</value>
        int DifferentialFormattingId { get; set; }

        /// <summary>
        /// Gets or sets the formulas used to evaluates whether this rule should be applied.
        /// </summary>
        /// <value>The formulas used to evaluate this rule</value>
        List<string> Formulas { get; set; }

        /// <summary>
        /// Gets or sets the type of the conditional formatting rule.
        /// </summary>
        /// <value>The type of the conditional formatting rule.</value>
        new ExcelConditionalFormatType Type { get; set; }
    }
}

