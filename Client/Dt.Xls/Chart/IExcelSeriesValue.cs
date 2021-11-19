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
    /// Specifies the data values which shall be used to define the location of data markers on a charts.
    /// </summary>
    public interface IExcelSeriesValue
    {
        /// <summary>
        /// Represents the number format code used for the referenced value
        /// </summary>
        string FormatCode { get; set; }

        /// <summary>
        /// Specifies a set of numbers.
        /// </summary>
        NumericDataLiterals NumericLiterals { get; set; }

        /// <summary>
        /// Specifies a reference formula to numeric data.
        /// </summary>
        string ReferenceFormula { get; set; }
    }
}

