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
    /// Defines the common properties for every conditional formatting rule
    /// </summary>
    public interface IExcelConditionalFormatRule
    {
        /// <summary>
        /// The priority of the conditional formatting rule. The value is used to determine which format
        /// should be evaluated and rendered. Lower numeric values are higher priority than higher numeric values. where '1' is the highest priority.
        /// </summary>
        /// <value>The priority of the rule</value>
        int Priority { get; set; }

        /// <summary>
        /// Flag indicates whether apply other lower priority rules when this rule evaluates to true.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if don't apply lower priority rules when this rule evaluates to true; otherwise, <see langword="false" />.
        /// </value>
        bool StopIfTrue { get; set; }

        /// <summary>
        /// Type of conditional formatting rule.
        /// </summary>
        /// <value>Type of conditional formatting rule.</value>
        ExcelConditionalFormatType Type { get; }
    }
}

