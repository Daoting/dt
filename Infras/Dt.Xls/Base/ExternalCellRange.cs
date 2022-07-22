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
    /// A class implements <see cref="T:Dt.Xls.IExternalRange" /> used to represents an external range.
    /// </summary>
    public class ExternalCellRange : IExternalRange, IRange
    {
        /// <summary>
        /// Gets the zero-based index of the start column of the range.
        /// </summary>
        /// <value>The start column index.</value>
        public int Column { get; set; }

        /// <summary>
        /// Gets the column span of the range.
        /// </summary>
        /// <value>The column span.</value>
        public int ColumnSpan { get; set; }

        /// <summary>
        /// Gets the zero-based index of the start row of the range.
        /// </summary>
        /// <value>The start row index.</value>
        public int Row { get; set; }

        /// <summary>
        /// Gets the row span of the range.
        /// </summary>
        /// <value>The row span.</value>
        public int RowSpan { get; set; }

        /// <summary>
        /// External References workbook name.
        /// </summary>
        /// <value>Workbook name, null means the workbook itself.</value>
        public string WorkbookName { get; set; }

        /// <summary>
        /// External References workbook name
        /// </summary>
        /// <value></value>
        public string WorksheetName { get; set; }
    }
}

