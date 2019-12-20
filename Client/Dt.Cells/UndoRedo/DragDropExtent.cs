#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a drag drop action extent that supports drag and drop on the sheet.
    /// </summary>
    public class DragDropExtent
    {
        private int _columnCount;
        private int _fromColumn;
        private int _fromRow;
        private int _rowCount;
        private int _toColumn;
        private int _toRow;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.DragDropExtent" /> class.
        /// </summary>
        /// <param name="fromRow">The from row index.</param>
        /// <param name="fromColumn">The from column index.</param>
        /// <param name="toRow">The to row index.</param>
        /// <param name="toColumn">The to column index.</param>
        /// <param name="rowCount">The drag drop row count.</param>
        /// <param name="columnCount">The drag drop column count.</param>
        public DragDropExtent(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this._fromRow = fromRow;
            this._fromColumn = fromColumn;
            this._toRow = toRow;
            this._toColumn = toColumn;
            this._rowCount = rowCount;
            this._columnCount = columnCount;
        }

        /// <summary>
        /// Gets the column count for the drag drop.
        /// </summary>
        public int ColumnCount
        {
            get { return  this._columnCount; }
        }

        /// <summary>
        /// Gets the from column index for the drag drop.
        /// </summary>
        public int FromColumn
        {
            get { return  this._fromColumn; }
        }

        /// <summary>
        /// Gets the from row index for the drag drop.
        /// </summary>
        public int FromRow
        {
            get { return  this._fromRow; }
        }

        /// <summary>
        /// Gets the row count for the drag drop.
        /// </summary>
        public int RowCount
        {
            get { return  this._rowCount; }
        }

        /// <summary>
        /// Gets the to column index for the drag drop.
        /// </summary>
        public int ToColumn
        {
            get { return  this._toColumn; }
        }

        /// <summary>
        /// Gets the to row index for the drag drop.
        /// </summary>
        public int ToRow
        {
            get { return  this._toRow; }
        }
    }
}

