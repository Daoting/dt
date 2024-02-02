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
    /// Defines Excel formula used in Excel
    /// </summary>
    public interface IExcelFormula
    {
        /// <summary>
        /// Gets or sets the array formula range.
        /// </summary>
        /// <value>The array formula range.</value>
        IRange ArrayFormulaRange { get; set; }

        /// <summary>
        /// Gets or sets the formula in A1 reference style 
        /// </summary>
        /// <value>The A1 reference style formula.</value>
        string Formula { get; set; }

        /// <summary>
        /// Gets or sets the formula in R1C1 reference style
        /// </summary>
        /// <value>The R1C1 reference style formula</value>
        string FormulaR1C1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the formula is array formula.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this formula is array formula; otherwise, <see langword="false" />.
        /// </value>
        bool IsArrayFormula { get; set; }
    }
}

