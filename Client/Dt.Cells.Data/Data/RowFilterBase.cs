#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a row filter base that supports row filters for filtering rows in a sheet.
    /// </summary>
    public abstract class RowFilterBase : IRangeSupport
    {
        /// <summary>
        /// Indicates a blank item in the filter.
        /// </summary>
        public static readonly object BlankItem = new object();
        CellRange range;
        bool showFilterButton = true;
        /// <summary>
        /// the sheet.
        /// </summary>
        Worksheet worksheet;

        internal event EventHandler<FilterEventArgs> Changed;

        /// <summary>
        /// Creates a new row filter.
        /// </summary>
        protected RowFilterBase(CellRange range)
        {
            this.range = range;
        }

        /// <summary>
        /// Adds the average filter for the row filter.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="compareType">The comparison type.</param>
        public void AddAverageFilter(int column, AverageConditionType compareType)
        {
            AverageCondition filterItem = new AverageCondition(compareType, null);
            this.AddFilterItem(column, filterItem);
        }

        /// <summary>
        /// Adds the background filter for the row filter. 
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="color">The background color for comparison.</param>
        public void AddBackgroundFilter(int column, Windows.UI.Color color)
        {
            ColorCondition filterItem = new ColorCondition(ColorCompareType.BackgroundColor, color);
            this.AddFilterItem(column, filterItem);
        }

        /// <summary>
        /// Adds the date filter for the row filter. 
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="compareType">The date comparison type.</param>
        /// <param name="date">The date for comparison.</param>
        public void AddDateFilter(int column, DateCompareType compareType, DateTime date)
        {
            DateCondition filterItem = new DateCondition(compareType, date, null);
            this.AddFilterItem(column, filterItem);
        }

        /// <summary>
        /// Adds a specified filter to the row filter.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="filterItem">The filter.</param>
        public abstract void AddFilterItem(int column, ConditionBase filterItem);
        /// <summary>
        /// Adds the foreground filter for the row filter. 
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="color">The foreground color for comparison.</param>
        public void AddForegroundFilter(int column, Windows.UI.Color color)
        {
            ColorCondition filterItem = new ColorCondition(ColorCompareType.ForegroundColor, color);
            this.AddFilterItem(column, filterItem);
        }

        /// <summary>
        /// Adds the number filter for the row filter.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="compareType">The comparison type.</param>
        /// <param name="number">The number for comparison.</param>
        public void AddNumberFilter(int column, GeneralCompareType compareType, double number)
        {
            NumberCondition filterItem = new NumberCondition(compareType, (double) number, null);
            this.AddFilterItem(column, filterItem);
        }

        /// <summary>
        /// Adds the text filter for the row filter. 
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="compareType">The text comparison type.</param>
        /// <param name="text">The text for comparison.</param>
        public void AddTextFilter(int column, TextCompareType compareType, string text)
        {
            TextCondition filterItem = new TextCondition(compareType, text, null);
            this.AddFilterItem(column, filterItem);
        }

        /// <summary>
        /// Adds the top 10 filter for the row filter.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="compareType">The comparison type.</param>
        /// <param name="rank">The item count of the top 10 condition.</param>
        public void AddTop10Filter(int column, Top10ConditionType compareType, int rank)
        {
            Top10Condition filterItem = new Top10Condition(compareType, rank, null);
            this.AddFilterItem(column, filterItem);
        }

        /// <summary>
        /// Filters all columns.
        /// </summary>
        public abstract void Filter();
        /// <summary>
        /// Filters the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        public abstract void Filter(int column);
        /// <summary>
        /// Gets the data that can be filtered and is to be displayed in the drop-down list for the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>Returns a collection that contains the data that can be filtered.</returns>
        public abstract ReadOnlyCollection<object> GetFilterableDataItems(int column);
        /// <summary>
        /// Gets all the filtered conditions.
        /// </summary>
        /// <returns>Returns a <see cref="T:Dt.Cells.Data.ConditionBase" /> collection that contains all the filtered conditions.</returns>
        public abstract ReadOnlyCollection<ConditionBase> GetFilteredItems();
        /// <summary>
        /// Gets the filters for a specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>Returns a <see cref="T:Dt.Cells.Data.ConditionBase" /> collection that contains filters that belong to a specified column.</returns>
        public abstract ReadOnlyCollection<ConditionBase> GetFilterItems(int column);
        void IRangeSupport.AddColumns(int column, int count)
        {
            this.OnAddColumns(column, count);
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            this.OnAddRows(row, count);
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            this.OnClear(row, column, rowCount, columnCount);
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.OnCopy(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.OnMove(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            this.OnRemoveColumns(column, count);
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            this.OnRemoveRows(row, count);
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.OnSwap(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        /// <summary>
        /// Determines whether a specified column is filtered.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>
        /// <c>true</c> if the column is filtered; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsColumnFiltered(int column);
        /// <summary>
        /// Determines whether the specified row is filtered out.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <returns>
        /// <c>true</c> if the row is filtered out; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsRowFilteredOut(int row);
        /// <summary>
        /// Adds columns in the filter after the specified column.
        /// </summary>
        /// <param name="column">The column index of the column after which to add columns.</param>
        /// <param name="count">The number of columns to add.</param>
        protected abstract void OnAddColumns(int column, int count);
        /// <summary>
        /// Adds rows in the filter after the specified row.
        /// </summary>
        /// <param name="row">The row index of the row after which to add rows.</param>
        /// <param name="count">The number of rows to add.</param>
        protected abstract void OnAddRows(int row, int count);
        /// <summary>
        /// Removes filters in the specified range 
        /// </summary>
        /// <param name="row">The row index of the first row in the selected range.</param>
        /// <param name="column">The column index of the first column in the selected range.</param>
        /// <param name="rowCount">The number of rows in the selected range.</param>
        /// <param name="columnCount">The number of columns in the selected range.</param>
        protected abstract void OnClear(int row, int column, int rowCount, int columnCount);
        /// <summary>
        /// Copies filters from the start range and pastes into a range of cells at the end location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin copying.</param>
        /// <param name="fromColumn">The column index from which to begin copying.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to copy.</param>
        /// <param name="columnCount">The number of columns to copy.</param>
        protected abstract void OnCopy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
        /// <summary>
        /// Moves filters from the start range to a range of cells at the end location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the move.</param>
        /// <param name="fromColumn">The column index from which to begin the move.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to move.</param>
        /// <param name="columnCount">The number of columns to move.</param>
        protected abstract void OnMove(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
        /// <summary>
        /// Removes columns in the filter from the specified column.
        /// </summary>
        /// <param name="column">The column index at which to start removing columns.</param>
        /// <param name="count">The number of columns to remove.</param>
        protected abstract void OnRemoveColumns(int column, int count);
        /// <summary>
        /// Removes rows in the filter from the specified row.
        /// </summary>
        /// <param name="row">The row index at which to start removing rows.</param>
        /// <param name="count">The number of rows to remove.</param>
        protected abstract void OnRemoveRows(int row, int count);
        /// <summary>
        /// Swaps filters of the specified range from one location to another.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the swap.</param>
        /// <param name="fromColumn">The column index from which to begin the swap.</param>
        /// <param name="toRow">The row index of the destination range.</param>
        /// <param name="toColumn">The column index of the destination range.</param>
        /// <param name="rowCount">The number of rows to swap.</param>
        /// <param name="columnCount">The number of columns to swap.</param>
        protected abstract void OnSwap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
        internal void RaiseFilterChangeddEvent(FilterAction filterAction)
        {
            if (this.Changed != null)
            {
                this.Changed(this, new FilterEventArgs(filterAction));
            }
        }

        /// <summary>
        /// Refreshes all the filters.
        /// </summary>
        public abstract void ReFilter();
        /// <summary>
        /// Removes the specified filter.
        /// </summary>
        /// <param name="column">The column index.</param>
        public abstract void RemoveFilterItems(int column);
        /// <summary>
        /// Clears all filters.
        /// </summary>
        public abstract void Reset();
        /// <summary>
        /// Removes the filter for all columns.
        /// </summary>
        public abstract void Unfilter();
        /// <summary>
        /// Removes the filter from the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        public abstract void Unfilter(int column);

        /// <summary>
        /// Gets a value that indicates whether any row is filtered.
        /// </summary>
        /// <value>
        /// <c>true</c> if some rows are filtered; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public abstract bool IsFiltered { get; }

        /// <summary>
        /// Gets or sets the cell range for the row filter.
        /// </summary>
        public virtual CellRange Range
        {
            get { return  this.range; }
            set { this.range = value; }
        }

        /// <summary>
        /// Gets or sets the sheet.
        /// </summary>
        /// <value>The sheet.</value>
        public Worksheet Sheet
        {
            get { return  this.worksheet; }
            internal set { this.worksheet = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show a filter button.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowFilterButton
        {
            get { return  this.showFilterButton; }
            set { this.showFilterButton = value; }
        }
    }
}

