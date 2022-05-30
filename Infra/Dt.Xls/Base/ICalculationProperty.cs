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
    /// Defines workbook formula setting properties
    /// </summary>
    public interface ICalculationProperty
    {
        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelCalculationMode" />
        /// </summary>
        /// <value>The calculation mode.</value>
        ExcelCalculationMode CalculationMode { get; set; }

        /// <summary>
        /// Get or set a value indicating whether this instance is full precision.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is full precision; otherwise, <see langword="false" />.
        /// </value>
        bool IsFullPrecision { get; set; }

        /// <summary>
        /// Get or set a value indicating whether this instance is iterate calculation on.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is iterate calculation on; otherwise, <see langword="false" />.
        /// </value>
        bool IsIterateCalculate { get; set; }

        /// <summary>
        /// Get or set the maximum change.
        /// </summary>
        /// <value>The maximum change.</value>
        double MaximunChange { get; set; }

        /// <summary>
        /// Get or set the max iteration count.
        /// </summary>
        /// <value>The max iteration count.</value>
        int MaxIterationCount { get; set; }

        /// <summary>
        /// Get or set a value indicating whether the formula need recalculated before save.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the formula need to be recalculated; otherwise, <see langword="false" />.
        /// </value>
        bool ReCalculationBeforeSave { get; set; }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelReferenceStyle" />
        /// </summary>
        /// <value>The reference mode</value>
        ExcelReferenceStyle RefMode { get; set; }
    }
}

