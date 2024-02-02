#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class ErrorBars : SpreadChartElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ErrorBars" /> class.
        /// </summary>
        internal ErrorBars()
        {
            this.ErrorBarValueType = Dt.Cells.Data.ErrorBarValueType.FixedValue;
            this.ErrorBarDireciton = Dt.Cells.Data.ErrorBarDireciton.None;
            this.ErrorBarType = Dt.Cells.Data.ErrorBarType.Both;
            this.NoEndCap = true;
        }

        /// <summary>
        /// Gets or sets the error bar direciton.
        /// </summary>
        /// <value>
        /// The error bar direciton.
        /// </value>
        internal Dt.Cells.Data.ErrorBarDireciton ErrorBarDireciton { get; set; }

        /// <summary>
        /// Gets or sets the type of the error bar.
        /// </summary>
        /// <value>
        /// The type of the error bar.
        /// </value>
        internal Dt.Cells.Data.ErrorBarType ErrorBarType { get; set; }

        /// <summary>
        /// Gets or sets the type of the error bar value.
        /// </summary>
        /// <value>
        /// The type of the error bar value.
        /// </value>
        internal Dt.Cells.Data.ErrorBarValueType ErrorBarValueType { get; set; }

        /// <summary>
        /// Specifies the error bar value in the negtive direction.
        /// </summary>
        internal DoubleSeriesCollection Minus { get; set; }

        internal IFormatter MinusFormatter { get; set; }

        internal string MinusReferenceFormula { get; set; }

        /// <summary>
        /// Specifies an end cap is not drawn on the error bars.
        /// </summary>
        internal bool NoEndCap { get; set; }

        /// <summary>
        /// Specifies the error bar value in the positive direction.
        /// </summary>
        internal DoubleSeriesCollection Plus { get; set; }

        internal IFormatter PlusFormatter { get; set; }

        internal string PlusReferenceFormula { get; set; }

        /// <summary>
        /// Specifies a value which is used with the errorBar element to determine the length of the error bars.
        /// </summary>
        internal double Value { get; set; }
    }
}

