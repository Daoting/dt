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
    /// A class implement <see cref="T:Dt.Xls.ISelectionRange" /> used to represents a selection range
    /// </summary>
    public class SelectionRange : ISelectionRange, IRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.SelectionRange" /> class.
        /// </summary>
        /// <param name="row">The start row index</param>
        /// <param name="column">The start column index</param>
        /// <param name="rowCount">The total row count.</param>
        /// <param name="columnCount">The total column count.</param>
        public SelectionRange(int row, int column, int rowCount, int columnCount)
        {
            if (row == -1)
            {
                rowCount = -1;
            }
            if (column == -1)
            {
                columnCount = -1;
            }
            this.Row = row;
            this.Column = column;
            this.RowSpan = rowCount;
            this.ColumnSpan = columnCount;
        }

        /// <summary>
        /// Gets or sets the type of the active pane.
        /// </summary>
        /// <value>The type of the active pane.</value>
        public PaneType activePaneType { get; set; }

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
    }
}

