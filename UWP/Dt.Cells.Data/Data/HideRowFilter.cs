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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a default row filter.
    /// </summary>
    public class HideRowFilter : RowFilterBase, IXmlSerializable
    {
        /// <summary>
        /// Map: Column and FilterItems
        /// key: Column Model Index.
        /// value: The Filter items of The Column.
        /// </summary>
        Dictionary<int, List<ConditionBase>> filterConditionMap;
        List<int> filteredColumns;
        /// <summary>
        /// Map: Filtered in Row and Column.
        /// Key: filtered in row model index.
        /// Value: Indicate which column filter this row in.
        /// </summary>
        Dictionary<int, List<int>> filteredInRowModelIndex;
        /// <summary>
        /// All filtered conditions.
        /// </summary>
        List<ConditionBase> filteredItems;
        SortState sortState;
        int sorttedColumn;

        /// <summary>
        /// Creates a new default row filter.
        /// </summary>
        public HideRowFilter() : this(null)
        {
            this.Init();
        }

        /// <summary>
        /// Creates a new default row filter.
        /// </summary>
        public HideRowFilter(CellRange range) : base(range)
        {
            this.filterConditionMap = new Dictionary<int, List<ConditionBase>>();
            this.sorttedColumn = -1;
            this.filteredColumns = new List<int>();
            this.filteredInRowModelIndex = new Dictionary<int, List<int>>();
            this.filteredItems = new List<ConditionBase>();
            this.Init();
        }

        /// <summary>
        /// Adds a specified filter to the row filter.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="filterItem">The filter.</param>
        public override void AddFilterItem(int column, ConditionBase filterItem)
        {
            this.CheckSheet();
            if (filterItem == null)
            {
                throw new ArgumentNullException("filterItem");
            }
            if ((column < -1) || (column >= base.Sheet.ColumnCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidColumnIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) column), ((int) (base.Sheet.ColumnCount - 1)) }));
            }
            if (this.Range != null)
            {
                int modelColumnFromViewColumn = base.Sheet.GetModelColumnFromViewColumn(column);
                if ((this.Range.Column <= -1) || ((modelColumnFromViewColumn >= this.Range.Column) && (modelColumnFromViewColumn < (this.Range.Column + this.Range.ColumnCount))))
                {
                    List<ConditionBase> list = null;
                    if (this.filterConditionMap.ContainsKey(modelColumnFromViewColumn))
                    {
                        list = this.filterConditionMap[modelColumnFromViewColumn];
                    }
                    if (list == null)
                    {
                        list = new List<ConditionBase>();
                    }
                    list.Add(filterItem);
                    this.filterConditionMap[modelColumnFromViewColumn] = list;
                }
            }
        }

        /// <summary>
        /// Filters the specified row.
        /// </summary>
        /// <param name="row">The index of the row view.</param>
        public void AddRowFilteredIn(int row)
        {
            if ((row < 0) || (row >= base.Sheet.RowCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            if (this.IsFiltered)
            {
                int modelRowFromViewRow = base.Sheet.GetModelRowFromViewRow(row);
                if ((this.Range != null) && ((this.Range.Row <= -1) || ((modelRowFromViewRow >= this.Range.Row) && (modelRowFromViewRow < (this.Range.Row + this.Range.RowCount)))))
                {
                    this.FilteredInRowModelIndex.Remove(modelRowFromViewRow);
                    List<int> list = new List<int>((IEnumerable<int>) this.GetFilteredColumns());
                    if (list != null)
                    {
                        this.FilteredInRowModelIndex.Add(modelRowFromViewRow, list);
                    }
                    base.Sheet.SetRowVisible(modelRowFromViewRow, SheetArea.Cells, true);
                }
            }
        }

        /// <summary>
        /// Checks the sheet.
        /// </summary>
        void CheckSheet()
        {
            if (base.Sheet == null)
            {
                throw new NotSupportedException(ResourceStrings.FilterSheetNullError);
            }
        }

        /// <summary>
        /// Removes all filters for every column.
        /// </summary>
        public void ClearFilterItems()
        {
            this.CheckSheet();
            if ((this.filterConditionMap != null) && (this.filterConditionMap.Count > 0))
            {
                List<int> list = new List<int>((IEnumerable<int>) this.filterConditionMap.Keys);
                foreach (int num in list)
                {
                    this.RemoveFilterItems(base.Sheet.GetViewColumnFromModelColumn(num));
                }
            }
        }

        /// <summary>
        /// Clears the row filtered statues.
        /// </summary>
        void ClearRowFilteredStatues()
        {
            this.filteredColumns.Clear();
            this.filteredInRowModelIndex.Clear();
            this.filteredItems.Clear();
        }

        /// <summary>
        /// Filters all columns.
        /// </summary>
        public override void Filter()
        {
            this.CheckSheet();
            this.ClearRowFilteredStatues();
            foreach (int num in this.filterConditionMap.Keys)
            {
                this.FilterColumn(num);
            }
            base.RaiseFilterChangeddEvent(FilterAction.Filter);
        }

        /// <summary>
        /// Filters the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        public override void Filter(int column)
        {
            this.CheckSheet();
            if ((column < 0) || (column >= base.Sheet.ColumnCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidColumnIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) column), ((int) (base.Sheet.ColumnCount - 1)) }));
            }
            int modelColumnFromViewColumn = base.Sheet.GetModelColumnFromViewColumn(column);
            if (this.filterConditionMap.ContainsKey(modelColumnFromViewColumn))
            {
                this.FilterColumn(modelColumnFromViewColumn);
            }
            base.RaiseFilterChangeddEvent(FilterAction.Filter);
        }

        /// <summary>
        /// Filters the specified column view index.
        /// </summary>
        /// <param name="columnModelIndex">Index of the column model</param>
        void FilterColumn(int columnModelIndex)
        {
            this.UnfilterColumn(columnModelIndex);
            if ((this.filterConditionMap.ContainsKey(columnModelIndex) && (this.Range != null)) && (((this.Range == null) || (this.Range.Column == -1)) || ((columnModelIndex >= this.Range.Column) && (columnModelIndex <= ((this.Range.Column + this.Range.ColumnCount) - 1)))))
            {
                int num = (this.Range.Row == -1) ? 0 : this.Range.Row;
                int num2 = (this.Range.Row == -1) ? base.Sheet.RowCount : (this.Range.Row + this.Range.RowCount);
                int rowModelIndex = num;
                int num4 = rowModelIndex;
                while ((rowModelIndex >= 0) && (rowModelIndex < num2))
                {
                    this.FilterRowByCell(rowModelIndex, columnModelIndex);
                    num4 = base.Sheet.NextNonEmptyRow(rowModelIndex);
                    if (num4 < 0)
                    {
                        num4 = num2;
                    }
                    for (int j = rowModelIndex + 1; j < num4; j++)
                    {
                        this.FilterRowByCell(j, columnModelIndex, true);
                    }
                    rowModelIndex = num4;
                }
                this.SetColumnFilteredState(columnModelIndex, true);
                this.SuspendEvent();
                for (int i = num; i < num2; i++)
                {
                    bool flag = !this.IsRowFilteredOut(i);
                    base.Sheet.SetRowVisible(i, SheetArea.Cells, flag);
                }
                this.ResumeEvent();
            }
        }

        void FilterRowByCell(int rowModelIndex, int columnModelIndex)
        {
            this.FilterRowByCell(rowModelIndex, columnModelIndex, false);
        }

        /// <summary>
        /// Filters the row by cell.
        /// </summary>
        /// <param name="rowModelIndex">Index of the row model.</param>
        /// <param name="columnModelIndex">Index of the column model.</param>
        /// <param name="nullValue">if set to <c>true</c> [null value].</param>
        void FilterRowByCell(int rowModelIndex, int columnModelIndex, bool nullValue)
        {
            if ((this.Range != null) && (((this.Range == null) || (this.Range.Row == -1)) || ((rowModelIndex >= this.Range.Row) && (rowModelIndex <= ((this.Range.Row + this.Range.RowCount) - 1)))))
            {
                Worksheet sheet = base.Sheet;
                int viewColumnFromModelColumn = sheet.GetViewColumnFromModelColumn(columnModelIndex);
                List<ConditionBase> list = null;
                this.filterConditionMap.TryGetValue(columnModelIndex, out list);
                if (list != null)
                {
                    ActualValue value2 = null;
                    ActualValue value3 = null;
                    ActualValue value4 = null;
                    ActualValue value5 = null;
                    foreach (ConditionBase base2 in list)
                    {
                        IRangesDependent dependent = base2 as IRangesDependent;
                        if (dependent != null)
                        {
                            dependent.Ranges = new CellRange[] { new CellRange(((this.Range == null) || (this.Range.Row < 0)) ? 0 : this.Range.Row, columnModelIndex, ((this.Range == null) || (this.Range.RowCount < 0)) ? sheet.RowCount : this.Range.RowCount, 1) };
                        }
                        ActualValue actual = value5;
                        if (!nullValue)
                        {
                            if (base2 is TextCondition)
                            {
                                if (value2 == null)
                                {
                                    value2 = new ActualValue(sheet.GetText(sheet.GetViewRowFromModelRow(rowModelIndex), viewColumnFromModelColumn)) {
                                        Sheet = sheet,
                                        Row = rowModelIndex,
                                        Column = columnModelIndex
                                    };
                                }
                                actual = value2;
                            }
                            else if (base2 is ColorCondition)
                            {
                                if (value3 == null)
                                {
                                    value3 = new ActualValue(sheet.GetActualStyleInfo(sheet.GetViewRowFromModelRow(rowModelIndex), viewColumnFromModelColumn, SheetArea.Cells, true)) {
                                        Sheet = sheet,
                                        Row = rowModelIndex,
                                        Column = columnModelIndex
                                    };
                                }
                                actual = value3;
                            }
                            else
                            {
                                if (value4 == null)
                                {
                                    value4 = new ActualValue(sheet.GetValue(rowModelIndex, columnModelIndex, SheetArea.Cells)) {
                                        Sheet = sheet,
                                        Row = rowModelIndex,
                                        Column = columnModelIndex
                                    };
                                }
                                actual = value4;
                            }
                        }
                        if (actual == null)
                        {
                            if (value5 == null)
                            {
                                value5 = new ActualValue(null) {
                                    Sheet = sheet,
                                    Row = rowModelIndex,
                                    Column = columnModelIndex
                                };
                            }
                            actual = value5;
                        }
                        if (base2.Evaluate(sheet, rowModelIndex, columnModelIndex, actual))
                        {
                            if (this.filteredInRowModelIndex.ContainsKey(rowModelIndex))
                            {
                                this.filteredInRowModelIndex[rowModelIndex].Add(columnModelIndex);
                            }
                            else
                            {
                                List<int> list3 = new List<int> {
                                    columnModelIndex
                                };
                                this.filteredInRowModelIndex.Add(rowModelIndex, list3);
                            }
                            if (!this.filteredItems.Contains(base2))
                            {
                                this.filteredItems.Add(base2);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the candidate data items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnViewIndex">Index of the column view.</param>
        /// <param name="order">The order.</param>
        /// <param name="hasBlankItem">if set to <c>true</c> [has blank item].</param>
        /// <returns></returns>
        List<T> GetCandindateDataItems<T>(int columnViewIndex, string order, out bool hasBlankItem)
        {
            if ((columnViewIndex < 0) || (columnViewIndex >= base.Sheet.ColumnCount))
            {
                throw new ArgumentOutOfRangeException("columnViewIndex");
            }
            base.Sheet.GetModelColumnFromViewColumn(columnViewIndex);
            IDictionary<T, string> dictionary = (IDictionary<T, string>) new Dictionary<T, string>();
            hasBlankItem = false;
            if (this.Range != null)
            {
                int num = (this.Range.Row == -1) ? 0 : this.Range.Row;
                int num2 = (this.Range.Row == -1) ? base.Sheet.RowCount : (this.Range.Row + this.Range.RowCount);
                bool flag = order == "Text";
                int row = num;
                int num4 = row;
                bool flag2 = num4 != 0;
                while ((row >= 0) && (row < num2))
                {
                    object obj2 = this.GetCellInfo(row, columnViewIndex, order);
                    if ((obj2 != null) && !dictionary.Keys.Contains((T) obj2))
                    {
                        if (!this.IsRowFilteredOut(row))
                        {
                            if (base.Sheet.GetActualRowHeight(row, SheetArea.Cells) > 0.0)
                            {
                                dictionary.Add((T) obj2, string.Empty);
                            }
                        }
                        else if ((this.filteredColumns.Count == 0) || (this.IsLastFilteredColumn(columnViewIndex) && this.IsRowfilteredOutByColumn(row, columnViewIndex)))
                        {
                            dictionary.Add((T) obj2, string.Empty);
                        }
                        else if (base.Sheet.GetActualRowHeight(row, SheetArea.Cells) > 0.0)
                        {
                            dictionary.Add((T) obj2, string.Empty);
                        }
                    }
                    if (flag)
                    {
                        if (obj2 == null)
                        {
                            hasBlankItem = true;
                        }
                        row++;
                    }
                    else
                    {
                        num4 = base.Sheet.NextNonEmptyRow(row);
                        if (num4 > (row + 1))
                        {
                            flag2 = true;
                            hasBlankItem = true;
                        }
                        row = num4;
                    }
                }
                if (!flag)
                {
                    if (flag2)
                    {
                        object obj3 = this.GetCellInfo(-1, columnViewIndex, order);
                        if (obj3 != null)
                        {
                            dictionary.Add((T) obj3, string.Empty);
                        }
                    }
                    if ((dictionary.Count == 0) && (base.Sheet.RowCount > 0))
                    {
                        object obj4 = this.GetCellInfo(0, columnViewIndex, order);
                        if (obj4 != null)
                        {
                            dictionary.Add((T) obj4, string.Empty);
                        }
                    }
                }
            }
            T[] localArray = new T[dictionary.Count];
            dictionary.Keys.CopyTo(localArray, 0);
            List<T> list = new List<T>(localArray);
            list.Sort(new Comparison<T>(this.ItemComparison<T>));
            return list;
        }

        /// <summary>
        /// Gets the cell info.
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <param name="order">Order string</param>
        /// <returns>Returns the cell info.</returns>
        protected internal virtual object GetCellInfo(int row, int column, string order)
        {
            this.CheckSheet();
            object obj2 = null;
            if (order == "Text")
            {
                object obj3 = base.Sheet.GetValue(row, column, SheetArea.Cells);
                if ((obj3 is DateTime) || (obj3 is TimeSpan))
                {
                    return obj3;
                }
                return base.Sheet.GetText(row, column);
            }
            if (order == "ForegroundColor")
            {
                SolidColorBrush foreground = base.Sheet.GetActualStyleInfo(row, column, SheetArea.Cells).Foreground as SolidColorBrush;
                if ((foreground != null) && (foreground.Color != Colors.Transparent))
                {
                    obj2 = foreground.Color;
                }
                return obj2;
            }
            if (order == "BackgroundColor")
            {
                SolidColorBrush background = base.Sheet.GetActualStyleInfo(row, column, SheetArea.Cells).Background as SolidColorBrush;
                if ((background != null) && (background.Color != Colors.Transparent))
                {
                    obj2 = background.Color;
                }
            }
            return obj2;
        }

        /// <summary>
        /// Gets the current sort state.
        /// </summary>
        /// <returns>
        /// The sort state <see cref="T:Dt.Cells.Data.SortState" /> of the current filter.
        /// </returns>
        public SortState GetColumnSortState()
        {
            return this.sortState;
        }

        /// <summary>
        /// Gets the background colors for filtered data to be displayed in the drop-down list for the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>Returns a collection that contains the color list.</returns>
        public ReadOnlyCollection<Windows.UI.Color> GetFilterableBackgroundColorItems(int column)
        {
            this.CheckSheet();
            bool hasBlankItem = false;
            return new ReadOnlyCollection<Windows.UI.Color>((IList<Windows.UI.Color>) this.GetCandindateDataItems<Windows.UI.Color>(column, "BackgroundColor", out hasBlankItem));
        }

        /// <summary>
        /// Gets the data that can be filtered and is to be displayed in the drop-down list for the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>Returns a collection that contains the data that can be filtered.</returns>
        public override ReadOnlyCollection<object> GetFilterableDataItems(int column)
        {
            this.CheckSheet();
            bool hasBlankItem = false;
            List<object> list = this.GetCandindateDataItems<object>(column, "Text", out hasBlankItem);
            if (hasBlankItem)
            {
                list.Add(RowFilterBase.BlankItem);
            }
            return new ReadOnlyCollection<object>((IList<object>) list);
        }

        /// <summary>
        /// Gets the foreground colors for filtered data to be displayed in the drop-down list for the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>Returns a collection that contains the color list.</returns>
        public ReadOnlyCollection<Windows.UI.Color> GetFilterableForeColorItems(int column)
        {
            this.CheckSheet();
            bool hasBlankItem = false;
            return new ReadOnlyCollection<Windows.UI.Color>((IList<Windows.UI.Color>) this.GetCandindateDataItems<Windows.UI.Color>(column, "ForegroundColor", out hasBlankItem));
        }

        /// <summary>
        /// Gets the indexes of columns that are filtered.
        /// </summary>
        /// <returns>Returns a collection that contains the filtered column indexes.</returns>
        public ReadOnlyCollection<int> GetFilteredColumns()
        {
            this.CheckSheet();
            return new ReadOnlyCollection<int>((IList<int>) this.filteredColumns);
        }

        /// <summary>
        /// Gets all the filtered conditions.
        /// </summary>
        /// <returns>Returns a <see cref="T:Dt.Cells.Data.ConditionBase" /> collection that contains all the filtered conditions.</returns>
        public override ReadOnlyCollection<ConditionBase> GetFilteredItems()
        {
            return new ReadOnlyCollection<ConditionBase>((IList<ConditionBase>) this.filteredItems);
        }

        /// <summary>
        /// Gets the filters for the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>
        /// Returns a <see cref="T:Dt.Cells.Data.ConditionBase" /> collection that contains filters that belong to the specified column.
        /// </returns>
        public override ReadOnlyCollection<ConditionBase> GetFilterItems(int column)
        {
            this.CheckSheet();
            if ((column < -1) || (column >= base.Sheet.ColumnCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidColumnIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) column), ((int) (base.Sheet.ColumnCount - 1)) }));
            }
            int modelColumnFromViewColumn = base.Sheet.GetModelColumnFromViewColumn(column);
            if (this.filterConditionMap.ContainsKey(modelColumnFromViewColumn))
            {
                List<ConditionBase> list = this.filterConditionMap[modelColumnFromViewColumn];
                if (list != null)
                {
                    return new ReadOnlyCollection<ConditionBase>((IList<ConditionBase>) list);
                }
            }
            return new ReadOnlyCollection<ConditionBase>(new ConditionBase[0]);
        }

        /// <summary>
        /// Gets the sorted column index.
        /// </summary>
        /// <returns>The sorted column index.</returns>
        public int GetSorttedColumnIndex()
        {
            return this.sorttedColumn;
        }

        void Init()
        {
            this.filterConditionMap = new Dictionary<int, List<ConditionBase>>();
            this.sorttedColumn = -1;
            this.sortState = SortState.None;
            this.filteredColumns = new List<int>();
            this.filteredInRowModelIndex = new Dictionary<int, List<int>>();
            this.filteredItems = new List<ConditionBase>();
            base.ShowFilterButton = true;
        }

        /// <summary>
        /// Determines whether the specified column is filtered.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns>
        /// <c>true</c> if the column is filtered; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsColumnFiltered(int column)
        {
            this.CheckSheet();
            return this.filteredColumns.Contains(column);
        }

        bool IsLastFilteredColumn(int column)
        {
            int num = this.filteredColumns.Count;
            return ((num > 0) && (this.filteredColumns[num - 1] == column));
        }

        /// <summary>
        /// Determines whether the specified row is filtered out.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <returns>
        /// <c>true</c> if the row is filtered out; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsRowFilteredOut(int row)
        {
            this.CheckSheet();
            int modelRowFromViewRow = base.Sheet.GetModelRowFromViewRow(row);
            if (!this.IsFiltered)
            {
                return false;
            }
            if (((this.Range != null) && (this.Range.Row != -1)) && ((modelRowFromViewRow < this.Range.Row) || (modelRowFromViewRow > ((this.Range.Row + this.Range.RowCount) - 1))))
            {
                return false;
            }
            if (((this.filteredInRowModelIndex.Count > 0) && this.filteredInRowModelIndex.ContainsKey(modelRowFromViewRow)) && (this.filteredInRowModelIndex[modelRowFromViewRow].Count >= this.filteredColumns.Count))
            {
                return false;
            }
            return true;
        }

        bool IsRowfilteredOutByColumn(int row, int byColumnIndex)
        {
            if (this.filteredColumns.Count == 0)
            {
                return false;
            }
            if (this.filteredInRowModelIndex == null)
            {
                return false;
            }
            int num = -1;
            int index = this.filteredColumns.IndexOf(byColumnIndex);
            if (index > 0)
            {
                num = this.filteredColumns[index - 1];
            }
            List<int> list = null;
            this.filteredInRowModelIndex.TryGetValue(row, out list);
            if (num > -1)
            {
                return (((list != null) && (list.Count != 0)) && (num == list[list.Count - 1]));
            }
            if (this.filteredColumns.Count != 1)
            {
                return false;
            }
            if (list != null)
            {
                return (list.Count == 0);
            }
            return true;
        }

        /// <summary>
        /// Gets whether the filter is sorted.
        /// </summary>
        /// <returns>
        /// <c>True</c> if the filter is sorted; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSortted()
        {
            return ((this.sorttedColumn > -1) && (this.sortState != SortState.None));
        }

        /// <summary>
        /// Items the comparison.
        /// </summary>
        /// <typeparam name="T">Any object.</typeparam>
        /// <param name="x">The object x</param>
        /// <param name="y">The object y</param>
        /// <returns>Returns the result of comparison.</returns>
        protected internal virtual int ItemComparison<T>(T x, T y)
        {
            return StringComparer.CurrentCultureIgnoreCase.Compare(Convert.ToString(x), Convert.ToString(y));
        }

        /// <summary>
        /// Adds columns to the filter after the specified column.
        /// </summary>
        /// <param name="column">The column index of the column after which to add columns.</param>
        /// <param name="count">The number of columns to add.</param>
        protected override void OnAddColumns(int column, int count)
        {
            if (this.Range != null)
            {
                if ((column >= 0) && this.IsSortted())
                {
                    int num = 0;
                    for (int j = 0; j < count; j++)
                    {
                        int num3 = j + column;
                        if (num3 <= this.sorttedColumn)
                        {
                            num++;
                        }
                    }
                    this.sorttedColumn += num;
                }
                int num4 = -1;
                int columnCount = 0;
                CellRange range = this.Range;
                if (range.Column > -1)
                {
                    if (range.Column >= column)
                    {
                        num4 = this.Range.Column;
                        this.SetRangeInternal(new CellRange(range.Row, range.Column + count, range.RowCount, range.ColumnCount));
                        columnCount = this.Range.ColumnCount;
                    }
                    else if ((range.Column < column) && (column < (range.Column + range.ColumnCount)))
                    {
                        num4 = column;
                        columnCount = range.ColumnCount - (column - range.Column);
                        this.SetRangeInternal(new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount + count));
                    }
                }
                if (num4 < 0)
                {
                    num4 = 0;
                    columnCount = base.Sheet.ColumnCount - count;
                }
                for (int i = (num4 + columnCount) - 1; i >= num4; i--)
                {
                    if (i >= column)
                    {
                        int num7 = i + count;
                        int index = this.filteredColumns.IndexOf(i);
                        if (index >= 0)
                        {
                            this.filteredColumns[index] = num7;
                        }
                        List<ConditionBase> list = null;
                        this.filterConditionMap.TryGetValue(i, out list);
                        if ((list != null) && (list.Count > 0))
                        {
                            this.filterConditionMap.Remove(i);
                            this.filterConditionMap[num7] = list;
                        }
                    }
                }
                this.ReFilter();
            }
        }

        /// <summary>
        /// Adds rows to the filter after the specified row.
        /// </summary>
        /// <param name="row">The row index of the row after which to add rows.</param>
        /// <param name="count">The number of rows to add.</param>
        protected override void OnAddRows(int row, int count)
        {
            if (this.Range != null)
            {
                int num = (base.Sheet.RowCount - count) - 1;
                if (this.Range.Row > -1)
                {
                    CellRange range = this.Range;
                    num = (range.Row + range.RowCount) - 1;
                    if (range.Row >= row)
                    {
                        this.SetRangeInternal(new CellRange(range.Row + count, range.Column, range.RowCount, range.ColumnCount));
                    }
                    else if ((range.Row < row) && (row < (range.Row + range.RowCount)))
                    {
                        this.SetRangeInternal(new CellRange(range.Row, range.Column, range.RowCount + count, range.ColumnCount));
                    }
                }
                if (this.IsFiltered && (this.filteredInRowModelIndex != null))
                {
                    List<int> list = new List<int>((IEnumerable<int>) this.filteredInRowModelIndex.Keys);
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        int num3 = list[i];
                        if ((num3 >= row) && (num3 <= num))
                        {
                            List<int> list2 = this.filteredInRowModelIndex[num3];
                            int num4 = num3 + count;
                            this.filteredInRowModelIndex[num4] = list2;
                            this.filteredInRowModelIndex.Remove(num3);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes filters from the specified range 
        /// </summary>
        /// <param name="row">The row index of the first row in the selected range.</param>
        /// <param name="column">The column index of the first column in the selected range.</param>
        /// <param name="rowCount">The number of rows in the selected range.</param>
        /// <param name="columnCount">The number of columns in the selected range.</param>
        protected override void OnClear(int row, int column, int rowCount, int columnCount)
        {
            if (this.Range != null)
            {
                CellRange range = new CellRange(row, column, rowCount, columnCount);
                if (base.ShowFilterButton)
                {
                    int num = this.Range.Row - 1;
                    int num2 = this.Range.RowCount + 1;
                    if (num < 0)
                    {
                        num = -1;
                        num2 = -1;
                    }
                    if (range.Contains(num, this.Range.Column, num2, this.Range.ColumnCount))
                    {
                        this.Unfilter();
                    }
                }
                else if (range.Contains(this.Range))
                {
                    this.Unfilter();
                }
            }
        }

        /// <summary>
        /// Copies filters from the start range and pastes into a range of cells at the end location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin copying.</param>
        /// <param name="fromColumn">The column index from which to begin copying.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to copy.</param>
        /// <param name="columnCount">The number of columns to copy.</param>
        protected override void OnCopy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
        }

        /// <summary>
        /// Moves filters from the start range to a range of cells at the end location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the move.</param>
        /// <param name="fromColumn">The column index from which to begin the move.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to move.</param>
        /// <param name="columnCount">The number of columns to move.</param>
        protected override void OnMove(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
        }

        /// <summary>
        /// Removes columns from the filter starting with the specified column.
        /// </summary>
        /// <param name="column">The column index at which to start removing columns.</param>
        /// <param name="count">The number of columns to remove.</param>
        protected override void OnRemoveColumns(int column, int count)
        {
            if (this.Range != null)
            {
                if ((column >= 0) && this.IsSortted())
                {
                    if ((column <= this.sorttedColumn) && (this.sorttedColumn < (column + count)))
                    {
                        this.sortState = SortState.None;
                        this.sorttedColumn = -1;
                    }
                    else if (this.sorttedColumn >= column)
                    {
                        this.sorttedColumn -= count;
                    }
                }
                CellRange range = this.Range;
                int num = range.Column;
                int columnCount = range.ColumnCount;
                if (num < 0)
                {
                    num = 0;
                    columnCount = base.Sheet.ColumnCount + count;
                }
                else
                {
                    columnCount = this.Range.ColumnCount;
                }
                int num3 = column + count;
                for (int i = num; i < (num + columnCount); i++)
                {
                    if (i >= column)
                    {
                        if (i < num3)
                        {
                            this.RemoveFilterItems(i);
                        }
                        else
                        {
                            int num5 = i - count;
                            int index = this.filteredColumns.IndexOf(i);
                            if (index >= 0)
                            {
                                this.filteredColumns[index] = num5;
                            }
                            List<ConditionBase> list = null;
                            this.filterConditionMap.TryGetValue(i, out list);
                            if ((list != null) && (list.Count > 0))
                            {
                                this.filterConditionMap.Remove(i);
                                this.filterConditionMap[num5] = list;
                            }
                        }
                    }
                }
                if (range.Column >= 0)
                {
                    if (column < range.Column)
                    {
                        if (num3 <= range.Column)
                        {
                            this.SetRangeInternal(new CellRange(range.Row, range.Column - count, range.RowCount, range.ColumnCount));
                        }
                        else if (num3 <= (range.Column + range.ColumnCount))
                        {
                            this.SetRangeInternal(new CellRange(range.Row, column, range.RowCount, (range.Column + range.ColumnCount) - num3));
                        }
                        else
                        {
                            this.SetRangeInternal(null);
                        }
                    }
                    else if (column < (range.Column + range.ColumnCount))
                    {
                        if (num3 <= (range.Column + range.ColumnCount))
                        {
                            this.SetRangeInternal(new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount - count));
                        }
                        else
                        {
                            this.SetRangeInternal(new CellRange(range.Row, range.Column, range.RowCount, column - range.Column));
                        }
                    }
                }
                this.ReFilter();
            }
        }

        /// <summary>
        /// Removes rows in the filter from the specified row.
        /// </summary>
        /// <param name="row">The row index at which to start removing rows.</param>
        /// <param name="count">The number of rows to remove.</param>
        protected override void OnRemoveRows(int row, int count)
        {
            if (this.Range != null)
            {
                int num = 0;
                int num2 = (base.Sheet.RowCount + count) - 1;
                if (this.Range.Row > -1)
                {
                    CellRange range = this.Range;
                    num = range.Row;
                    num2 = (range.Row + range.RowCount) - 1;
                    if (range.Row >= row)
                    {
                        if (range.Row == (row + 1))
                        {
                            this.SetRangeInternal(null);
                        }
                        else if ((range.Row + range.RowCount) <= (row + count))
                        {
                            this.SetRangeInternal(null);
                        }
                        else if (range.Row < (row + count))
                        {
                            this.SetRangeInternal(new CellRange(row, range.Column, (range.Row + range.RowCount) - (row + count), range.ColumnCount));
                        }
                        else
                        {
                            this.SetRangeInternal(new CellRange(range.Row - count, range.Column, range.RowCount, range.ColumnCount));
                        }
                    }
                    else if ((range.Row < row) && (row < (range.Row + range.RowCount)))
                    {
                        this.SetRangeInternal(new CellRange(range.Row, range.Column, range.RowCount - Math.Min((range.Row + range.RowCount) - row, count), range.ColumnCount));
                    }
                }
                if (this.IsFiltered && (this.filteredInRowModelIndex != null))
                {
                    new List<int>((IEnumerable<int>) this.filteredInRowModelIndex.Keys);
                    for (int i = num; i <= num2; i++)
                    {
                        if (i >= row)
                        {
                            if (i < (row + count))
                            {
                                this.filteredInRowModelIndex.Remove(i);
                            }
                            else
                            {
                                int num4 = i;
                                List<int> list = null;
                                if (this.filteredInRowModelIndex.TryGetValue(num4, out list))
                                {
                                    int num5 = num4 - count;
                                    this.filteredInRowModelIndex[num5] = list;
                                    this.filteredInRowModelIndex.Remove(num4);
                                }
                            }
                        }
                    }
                }
                this.ReFilter();
            }
        }

        /// <summary>
        /// Swaps filters in the specified range from one location to another.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the swap.</param>
        /// <param name="fromColumn">The column index from which to begin the swap.</param>
        /// <param name="toRow">The row index of the destination range.</param>
        /// <param name="toColumn">The column index of the destination range.</param>
        /// <param name="rowCount">The number of rows to swap.</param>
        /// <param name="columnCount">The number of columns to swap.</param>
        protected override void OnSwap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
        }

        internal virtual void ReadXml(XmlReader reader)
        {
            string name = reader.Name;
            if (name != null)
            {
                if (name == "Conditions")
                {
                    XmlReader reader2 = Serializer.ExtractNode(reader);
                    Serializer.InitReader(reader2);
                    while (reader2.Read())
                    {
                        if ((reader2.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && "Column".Equals(reader2.Name))
                        {
                            int num = Serializer.ReadAttributeInt("index", -1, reader);
                            List<ConditionBase> list = Serializer.DeserializeObj(typeof(List<ConditionBase>), reader2) as List<ConditionBase>;
                            if (num > -1)
                            {
                                this.filterConditionMap.Add(num, list);
                            }
                        }
                    }
                }
                else if (name == "ElementFilteredColumns")
                {
                    XmlReader reader3 = Serializer.ExtractNode(reader);
                    Serializer.InitReader(reader3);
                    List<int> list2 = new List<int>();
                    Serializer.DeserializeList((IList) list2, reader3);
                    if (list2.Count > 0)
                    {
                        this.filteredColumns = list2;
                    }
                }
                else if (name == "Range")
                {
                    this.SetRangeInternal(Serializer.DeserializeObj(typeof(CellRange), reader) as CellRange);
                }
            }
        }

        /// <summary>
        /// Refreshes all the filters.
        /// </summary>
        public override void ReFilter()
        {
            if (this.filteredInRowModelIndex != null)
            {
                this.filteredInRowModelIndex.Clear();
            }
            if (this.filteredItems != null)
            {
                this.filteredItems.Clear();
            }
            try
            {
                if ((this.filteredColumns != null) && (this.filteredColumns.Count > 0))
                {
                    int[] numArray = this.filteredColumns.ToArray();
                    this.filteredColumns.Clear();
                    foreach (int num in numArray)
                    {
                        this.FilterColumn(num);
                    }
                }
            }
            catch
            {
            }
        }

        void RemoveFilteredColumn(int column)
        {
            this.filteredColumns.Remove(column);
        }

        /// <summary>
        /// Removes the filtered items.
        /// </summary>
        /// <param name="columnModelIndex">Index of the column model.</param>
        void RemoveFilteredItems(int columnModelIndex)
        {
            if (this.filterConditionMap.ContainsKey(columnModelIndex))
            {
                List<ConditionBase> list = new List<ConditionBase>();
                List<ConditionBase> list2 = new List<ConditionBase>();
                foreach (int num in this.filterConditionMap.Keys)
                {
                    if (num == columnModelIndex)
                    {
                        list.AddRange(this.filterConditionMap[num]);
                    }
                    else
                    {
                        list2.AddRange(this.filterConditionMap[num]);
                    }
                }
                foreach (ConditionBase base2 in list)
                {
                    if (!list2.Contains(base2))
                    {
                        this.filteredItems.Remove(base2);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the filter for the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        public override void RemoveFilterItems(int column)
        {
            this.CheckSheet();
            this.RemoveFilteredItems(column);
            this.filterConditionMap.Remove(column);
            this.Unfilter(column);
        }

        /// <summary>
        /// Clears all filters.
        /// </summary>
        public override void Reset()
        {
            if (base.Sheet != null)
            {
                this.Unfilter();
            }
            this.ClearRowFilteredStatues();
            this.filterConditionMap.Clear();
        }

        void ResumeEvent()
        {
            if ((base.Sheet != null) && (base.Sheet.Workbook != null))
            {
                base.Sheet.Workbook.ResumeEvent();
            }
        }

        internal virtual void SaveXml(XmlWriter writer)
        {
            if (!base.ShowFilterButton)
            {
                Serializer.WriteAttribute("ShowFilterButton", base.ShowFilterButton, writer);
            }
            if (this.IsSortted())
            {
                Serializer.WriteAttribute("SorttedColumnIndex", this.sorttedColumn, writer);
                Serializer.WriteAttribute("SorttedColumnState", this.sortState.ToString(), writer);
            }
            if ((this.filterConditionMap != null) && (this.filterConditionMap.Count > 0))
            {
                writer.WriteStartElement("Conditions");
                foreach (int num in this.filterConditionMap.Keys)
                {
                    List<ConditionBase> list = this.filterConditionMap[num];
                    Serializer.WriteStartObj("Column", writer);
                    Serializer.WriteAttr("index", (int) num, writer);
                    Serializer.SerializeObj(list, null, writer);
                    Serializer.WriteEndObj(writer);
                }
                writer.WriteEndElement();
            }
            if ((this.filteredColumns != null) && (this.filteredColumns.Count > 0))
            {
                Serializer.WriteStartObj("ElementFilteredColumns", writer);
                Serializer.SerializeList((IList) this.filteredColumns, writer);
                Serializer.WriteEndObj(writer);
            }
            if (this.Range != null)
            {
                Serializer.SerializeObj(this.Range, "Range", writer);
            }
        }

        /// <summary>
        /// Sets the filtered state of the column.
        /// </summary>
        /// <param name="columnModelIndex">Index of the column model</param>
        /// <param name="isFiltered">Set to <c>true</c> to filter the column</param>
        void SetColumnFilteredState(int columnModelIndex, bool isFiltered)
        {
            if (isFiltered)
            {
                this.TryPushFilteredColumn(columnModelIndex);
            }
            else
            {
                this.RemoveFilteredColumn(columnModelIndex);
            }
        }

        internal void SetRangeInternal(CellRange newRange)
        {
            base.Range = newRange;
        }

        /// <summary>
        /// Sorts the specified column in the specified order.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="ascending">
        /// <c>true</c> to sort as ascending; otherwise, <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true </c> if sorted successfully; otherwise, <c>false</c>.
        /// </returns>
        public bool SortColumn(int column, bool ascending)
        {
            if (base.Sheet == null)
            {
                return false;
            }
            CellRange range = this.Range;
            if (range == null)
            {
                return false;
            }
            int row = range.Row;
            int num2 = range.Column;
            int rowCount = range.RowCount;
            int columnCount = range.ColumnCount;
            if ((((row == -1) && (num2 == -1)) && ((rowCount == -1) && (columnCount == -1))) || ((((row == 0) && (num2 == 0)) && (rowCount == base.Sheet.RowCount)) && (columnCount == base.Sheet.ColumnCount)))
            {
                row = 0;
                num2 = 0;
                rowCount = base.Sheet.GetLastDirtyRow(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data) + 1;
                columnCount = base.Sheet.GetLastDirtyColumn(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data) + 1;
            }
            bool flag2 = base.Sheet.SortRange(row, num2, rowCount, columnCount, true, new SortInfo[] { new SortInfo(column, ascending) });
            if (flag2)
            {
                this.sorttedColumn = column;
                this.sortState = ascending ? SortState.Ascending : SortState.Descending;
                this.ReFilter();
            }
            return flag2;
        }

        void SuspendEvent()
        {
            if ((base.Sheet != null) && (base.Sheet.Workbook != null))
            {
                base.Sheet.Workbook.SuspendEvent();
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            this.Init();
            base.ShowFilterButton = Serializer.ReadAttributeBoolean("ShowFilterButton", true, reader);
            string s = Serializer.ReadAttribute("SorttedColumnIndex", reader);
            if (s != null)
            {
                this.sorttedColumn = int.Parse(s);
                string str2 = Serializer.ReadAttribute("SorttedColumnState", reader);
                if (str2 != null)
                {
                    this.sortState = (SortState) Enum.Parse((Type) typeof(SortState), str2, true);
                }
            }
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    this.ReadXml(reader);
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            Serializer.WriteTypeAttr(this, writer);
            this.SaveXml(writer);
        }

        void TryPushFilteredColumn(int column)
        {
            if (this.filteredColumns.Count > 0)
            {
                if (this.filteredColumns[this.filteredColumns.Count - 1] == column)
                {
                    return;
                }
                this.RemoveFilteredColumn(column);
            }
            this.filteredColumns.Add(column);
        }

        /// <summary>
        /// Removes the filter for all columns.
        /// </summary>
        public override void Unfilter()
        {
            this.CheckSheet();
            foreach (int num in this.filterConditionMap.Keys)
            {
                this.UnfilterColumn(num);
            }
            this.ClearRowFilteredStatues();
            base.RaiseFilterChangeddEvent(FilterAction.Ufilter);
        }

        /// <summary>
        /// Removes the filter from the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        public override void Unfilter(int column)
        {
            this.CheckSheet();
            int modelColumnFromViewColumn = base.Sheet.GetModelColumnFromViewColumn(column);
            this.UnfilterColumn(modelColumnFromViewColumn);
            base.RaiseFilterChangeddEvent(FilterAction.Ufilter);
        }

        /// <summary>
        /// Removes the filter from the specified column.
        /// </summary>
        /// <param name="columnModelIndex">Index of the column model</param>
        void UnfilterColumn(int columnModelIndex)
        {
            if (this.Range != null)
            {
                this.SuspendEvent();
                int num = (this.Range.Row == -1) ? 0 : this.Range.Row;
                int num2 = (this.Range.Row == -1) ? base.Sheet.RowCount : (this.Range.Row + this.Range.RowCount);
                if (this.IsColumnFiltered(columnModelIndex))
                {
                    if ((this.filteredColumns.Count <= 1) || (columnModelIndex != this.filteredColumns[0]))
                    {
                        for (int i = num; i < num2; i++)
                        {
                            if (this.IsRowfilteredOutByColumn(i, columnModelIndex))
                            {
                                base.Sheet.SetRowVisible(i, SheetArea.Cells, true);
                            }
                        }
                    }
                    List<int> list = new List<int>();
                    foreach (KeyValuePair<int, List<int>> pair in this.filteredInRowModelIndex)
                    {
                        List<int> list2 = pair.Value;
                        if (list2.Remove(columnModelIndex) && (list2.Count == 0))
                        {
                            list.Add(pair.Key);
                        }
                    }
                    if (list.Count > 0)
                    {
                        foreach (int num4 in list)
                        {
                            this.filteredInRowModelIndex.Remove(num4);
                        }
                    }
                    this.SetColumnFilteredState(columnModelIndex, false);
                    this.RemoveFilteredItems(columnModelIndex);
                    this.ReFilter();
                }
                else
                {
                    for (int j = num; j < num2; j++)
                    {
                        if (!this.IsRowFilteredOut(j))
                        {
                            base.Sheet.SetRowVisible(j, SheetArea.Cells, true);
                        }
                    }
                }
                this.ResumeEvent();
            }
        }

        internal virtual void UpdateRange(CellRange newRange)
        {
            CellRange range = this.Range;
            if (range != null)
            {
                if ((newRange != null) && (range != newRange))
                {
                    int column = range.Column;
                    int columnCount = range.ColumnCount;
                    base.Range = newRange;
                    int num3 = newRange.Column;
                    int num4 = newRange.ColumnCount;
                    if (((column >= 0) || (num3 >= 0)) && this.IsFiltered)
                    {
                        if (column < 0)
                        {
                            column = 0;
                            columnCount = base.Sheet.RowCount;
                        }
                        if (num3 < 0)
                        {
                            num3 = 0;
                            num4 = base.Sheet.RowCount;
                        }
                        for (int i = 0; i < columnCount; i++)
                        {
                            int num6 = column + i;
                            if ((num6 < num3) || (num6 >= (num3 + num4)))
                            {
                                this.RemoveFilterItems(num6);
                            }
                        }
                    }
                    int row = range.Row;
                    int rowCount = range.RowCount;
                    int num9 = newRange.Row;
                    int num10 = newRange.RowCount;
                    if (((row >= 0) || (num9 >= 0)) && (this.IsFiltered && (this.filteredInRowModelIndex != null)))
                    {
                        for (int j = 0; j < rowCount; j++)
                        {
                            int num12 = row + j;
                            if ((num12 < num9) || (num12 >= (num9 + num10)))
                            {
                                this.filteredInRowModelIndex.Remove(num12);
                            }
                        }
                    }
                }
                else if (newRange == null)
                {
                    this.Reset();
                }
            }
            if (this.IsSortted() && ((newRange == null) || ((range.Row >= 0) && !newRange.Contains(range.Row, this.sorttedColumn))))
            {
                this.sorttedColumn = -1;
                this.sortState = SortState.None;
            }
            this.SetRangeInternal(newRange);
        }

        /// <summary>
        /// Gets the filtered columns in queue.
        /// </summary>
        protected internal List<int> FilteredColumns
        {
            get { return  this.filteredColumns; }
        }

        /// <summary>
        /// Gets the row model index.
        /// </summary>
        /// <value>The row model index.</value>
        protected internal Dictionary<int, List<int>> FilteredInRowModelIndex
        {
            get { return  this.filteredInRowModelIndex; }
        }

        /// <summary>
        /// Gets the filter item map.
        /// </summary>
        /// <value>The filter item map.</value>
        protected internal Dictionary<int, List<ConditionBase>> FilterItemMap
        {
            get { return  this.filterConditionMap; }
        }

        /// <summary>
        /// Gets whether any of the columns are filtered.
        /// </summary>
        /// <value>
        /// <c>true</c> if some columns are filtered; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public override bool IsFiltered
        {
            get { return  (this.filteredColumns.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the cell range for the hide row filter.
        /// </summary>
        public override CellRange Range
        {
            get { return  base.Range; }
            set
            {
                if (value != base.Range)
                {
                    this.UpdateRange(value);
                }
            }
        }
    }
}

