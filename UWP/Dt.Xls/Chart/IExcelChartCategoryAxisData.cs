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
    /// Specifies the  data used for the category axis.
    /// </summary>
    public interface IExcelChartCategoryAxisData
    {
        /// <summary>
        /// Represents the number format code used for the NumberReferencesFormula 
        /// </summary>
        string FormatCode { get; set; }

        /// <summary>
        /// Specifies a reference to data for the category axis
        /// </summary>
        string MultiLevelStringReferenceFormula { get; set; }

        /// <summary>
        /// Specifies a set of numbers used for the parent element.
        /// </summary>
        string NumberReferencesFormula { get; set; }

        /// <summary>
        /// Specifies a reference to numeric data with a cache of the last valuse used.
        /// </summary>
        NumericDataLiterals NumericLiterals { get; set; }

        /// <summary>
        /// Specifies a set of string s used for a chart.
        /// </summary>
        StringLiteralData StringLiterals { get; set; }

        /// <summary>
        /// Specifies a reference to data for a single data label or title with a cache of the last values used.
        /// </summary>
        string StringReferencedFormula { get; set; }
    }
}

