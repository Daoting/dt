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
    /// Specifies formatting for one area of a table or PivotTable.Together the sequence of these elements  makes up one entire Table style or PivotTable style definition.
    /// </summary>
    public interface IExcelTableStyleElement
    {
        /// <summary>
        /// Zero-based index to a different formatting records, specifying differential formatting to use with this the table or PivotTable style element.
        /// </summary>
        int DifferentFormattingIndex { get; set; }

        /// <summary>
        /// Number of rows or columns in a single band of striping.
        /// </summary>
        /// <remarks>
        /// Applies only when type is firstRowStripe, secondRowStripe, firstColumnStripe, or secondColumnStripe
        /// </remarks>
        int Size { get; set; }

        /// <summary>
        /// Identifies this table style element's type.
        /// </summary>
        ExcelTableElementType Type { get; set; }
    }
}

