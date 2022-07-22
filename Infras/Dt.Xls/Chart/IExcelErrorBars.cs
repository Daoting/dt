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

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies error bars. 
    /// </summary>
    public interface IExcelErrorBars
    {
        /// <summary>
        /// Gets or sets the error bar direciton.
        /// </summary>
        /// <value>
        /// The error bar direciton.
        /// </value>
        ExcelErrorBarDireciton ErrorBarDireciton { get; set; }

        /// <summary>
        /// Represents the error bar format.
        /// </summary>
        IExcelChartFormat ErrorBarsFormat { get; set; }

        /// <summary>
        /// Gets or sets the type of the error bar.
        /// </summary>
        /// <value>
        /// The type of the error bar.
        /// </value>
        ExcelErrorBarType ErrorBarType { get; set; }

        /// <summary>
        /// Gets or sets the type of the error bar value.
        /// </summary>
        /// <value>
        /// The type of the error bar value.
        /// </value>
        ExcelErrorBarValueType ErrorBarValueType { get; set; }

        /// <summary>
        /// Specifies the error bar value in the negtive direction.
        /// </summary>
        NumericDataLiterals Minus { get; set; }

        /// <summary>
        /// Specifies the error bar formula in the negtive direction.
        /// </summary>
        string MinusReferenceFormula { get; set; }

        /// <summary>
        /// Specifies an end cap is not drawn on the error bars.
        /// </summary>
        bool NoEndCap { get; set; }

        /// <summary>
        /// Specifies the error bar value in the positive direction.
        /// </summary>
        NumericDataLiterals Plus { get; set; }

        /// <summary>
        /// Specifies the error bar formula in the positive direction.
        /// </summary>
        string PlusReferenceFormula { get; set; }

        /// <summary>
        /// Specifies a value which is used with the errorBar element to determine the length of the error bars.
        /// </summary>
        double Value { get; set; }
    }
}

