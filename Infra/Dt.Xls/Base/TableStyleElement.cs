#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// An implementation of <see cref="T:Dt.Xls.IExcelTableStyleElement" />
    /// </summary>
    public class TableStyleElement : IExcelTableStyleElement
    {
        /// <summary>
        /// Zero-based index to a different formatting records, specifying differential formatting to use with this the table or PivotTable style element.
        /// </summary>
        /// <value></value>
        public int DifferentFormattingIndex { get; set; }

        /// <summary>
        /// Number of rows or columns in a single band of striping.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Applies only when type is firstRowStripe, secondRowStripe, firstColumnStripe, or secondColumnStripe
        /// </remarks>
        public int Size { get; set; }

        /// <summary>
        /// Identifies this table style element's type.
        /// </summary>
        /// <value></value>
        public ExcelTableElementType Type { get; set; }
    }
}

