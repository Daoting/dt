#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the partial (abstract) implementation of the ISheetSelectionModel interface 
    /// for a selection model for a sheet.  
    /// </summary>
    internal abstract class SheetSelectionModelBase
    {
        /// <summary>
        /// Occurs when the user makes a change to the model 
        /// that affects the selection of cells in the worksheet.
        /// </summary>
        public event EventHandler<SheetSelectionChangedEventArgs> Changed;

        protected SheetSelectionModelBase()
        {
        }

        /// <summary>
        /// Adds a cell or cells to the selection.
        /// </summary>
        /// <param name="row">The row index of the first cell to add.</param>
        /// <param name="column">The column index of the first cell to add.</param>
        /// <param name="rowCount">The number of rows to add.</param>
        /// <param name="columnCount">The number of columns to add.</param>
        public virtual void AddSelection(int row, int column, int rowCount, int columnCount)
        {
        }

        /// <summary>
        /// Adds a cell or cells to the selection.
        /// </summary>
        /// <param name="row">The row index of the first cell to add.</param>
        /// <param name="column">The column index of the first cell to add.</param>
        /// <param name="rowCount">The number of rows to add.</param>
        /// <param name="columnCount">The number of columns to add.</param>
        /// <param name="moveAnchorCell">Whether to reset the anchor cell to the top-left corner of the selection area.</param>
        public virtual void AddSelection(int row, int column, int rowCount, int columnCount, bool moveAnchorCell)
        {
        }

        /// <summary>
        /// Removes all the selections from the worksheet so that cells are no longer selected.
        /// </summary>
        public virtual void ClearSelection()
        {
        }

        /// <summary>
        /// Extends the selection.
        /// </summary>
        /// <param name="row">The row index to which to extend the selection.</param>
        /// <param name="column">The column index to which to extend the selection.</param>
        /// <param name="rowCount">The number of rows to which to extend the selection.</param>
        /// <param name="columnCount">The number of columns to which to extend the selection.</param>
        public virtual void ExtendSelection(int row, int column, int rowCount, int columnCount)
        {
        }

        /// <summary>
        /// Raises the selection changed event.
        /// </summary>
        /// <param name="row">The row index of the start of the selection.</param>
        /// <param name="column">The column index of the start of the selection.</param>
        /// <param name="rowCount">The number of rows in the selection.</param>
        /// <param name="columnCount">The number of columns in the selection.</param>
        protected void FireChanged(int row, int column, int rowCount, int columnCount)
        {
            this.OnChanged(new SheetSelectionChangedEventArgs(row, column, rowCount, columnCount));
        }

        /// <summary>
        /// Gets an enumerator that can iterate through the selections.
        /// </summary>
        /// <returns>Returns an enumerator to enumerate all selections.</returns>
        public virtual IEnumerator GetEnumerator()
        {
            return null;
        }

        /// <summary>
        /// Determines whether any cell in the specified column is in the selection.
        /// </summary>
        /// <param name="column">The column index of the column to check.</param>
        /// <returns>
        /// <c>true</c> if any cell in a specified column is in the selection; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsAnyCellInColumnSelected(int column)
        {
            return false;
        }

        /// <summary>
        /// Determines whether any cell in the specified row is in the selection.
        /// </summary>
        /// <param name="row">The row index of the row to check.</param>
        /// <returns>
        /// <c>true</c> if any cell in the specified row is in the selection; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsAnyCellInRowSelected(int row)
        {
            return false;
        }

        /// <summary>
        /// Determines whether the model has no selections.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the model has no selections; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsEmpty()
        {
            return false;
        }

        /// <summary>
        /// Determines whether the specified cell is in the selection.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>
        /// <c>true</c> if the specified row is selected; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsSelected(int row, int column)
        {
            return false;
        }

        /// <summary>
        /// For handling the Changed event in subclasses.
        /// </summary>
        /// <param name="e">SheetSelectionModelEventArgs</param>
        protected virtual void OnChanged(SheetSelectionChangedEventArgs e)
        {
            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }

        /// <summary>
        /// Removes the specified selection range from the selection list, if one exists. 
        /// </summary>
        /// <param name="row">The row index of the cell at which to start.</param>
        /// <param name="column">The column index of the cell at which to start.</param>
        /// <param name="rowCount">The number of rows to deselect.</param>
        /// <param name="columnCount">The number of columns to deselect.</param>
        public virtual void RemoveSelection(int row, int column, int rowCount, int columnCount)
        {
        }

        /// <summary>
        /// Specifies the anchor (first cell) in the cell selection.
        /// </summary>
        /// <param name="row">The row index of the first cell in the selection.</param>
        /// <param name="column">The column index of the first cell in the selection.</param>
        public virtual void SetAnchor(int row, int column)
        {
        }

        /// <summary>
        /// Selects the specified cells.
        /// </summary>
        /// <param name="row">The row index of the first cell.</param>
        /// <param name="column">The column index of the first cell.</param>
        /// <param name="rowCount">The number of rows in the selection.</param>
        /// <param name="columnCount">The number of columns in the selection.</param>
        public virtual void SetSelection(int row, int column, int rowCount, int columnCount)
        {
        }

        /// <summary>
        /// Gets the column index of the first cell selected in the range.
        /// </summary>
        /// <value>The column index of the first cell selected in the range.</value>
        public virtual int AnchorColumn
        {
            get { return  0; }
        }

        /// <summary>
        /// Gets the row index of the first cell selected in the range.
        /// </summary>
        /// <value>The row index of the first cell selected in the range.</value>
        public virtual int AnchorRow
        {
            get { return  0; }
        }

        /// <summary>
        /// Gets the number of selections.
        /// </summary>
        /// <value>The number of selections.</value>
        public virtual int Count
        {
            get { return  0; }
        }

        /// <summary>
        /// Gets a selection in the model.
        /// </summary>
        /// <param name="index">The index of the selection.</param>
        /// <value>A cell range that represents a selection in the model.</value>
        public virtual CellRange this[int index]
        {
            get { return  null; }
        }

        public abstract ReadOnlyCollection<CellRange> Items { get; }

        /// <summary>
        /// Gets or sets whether users can select ranges of items.
        /// </summary>
        /// <value>The <see cref="P:Dt.Cells.Data.SheetSelectionModelBase.SelectionPolicy" /> enumeration that indicates how users can select ranges of items.</value>
        public virtual Dt.Cells.Data.SelectionPolicy SelectionPolicy
        {
            get { return  Dt.Cells.Data.SelectionPolicy.Single; }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets whether users can select cells, rows, or columns.
        /// </summary>
        /// <value>The <see cref="P:Dt.Cells.Data.SheetSelectionModelBase.SelectionUnit" /> enumeration that specifies the selection type.</value>
        public virtual Dt.Cells.Data.SelectionUnit SelectionUnit
        {
            get { return  Dt.Cells.Data.SelectionUnit.Cell; }
            set
            {
            }
        }
    }
}

