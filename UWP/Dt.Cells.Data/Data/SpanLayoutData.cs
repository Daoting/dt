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
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// SpanLayoutData
    /// </summary>
    internal class SpanLayoutData
    {
        readonly SheetSpanModel pureSpans = new SheetSpanModel();
        readonly SheetSpanModel spans = new SheetSpanModel();

        /// <summary>
        /// Adds the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <returns></returns>
        public bool Add(int row, int column, int rowCount, int columnCount)
        {
            return this.Add(row, column, rowCount, columnCount, false);
        }

        /// <summary>
        /// Adds the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="isPureSpan">If set to <c>true</c>, [is pure span].</param>
        /// <returns></returns>
        public bool Add(int row, int column, int rowCount, int columnCount, bool isPureSpan)
        {
            if (isPureSpan)
            {
                this.pureSpans.Add(row, column, rowCount, columnCount);
            }
            return this.spans.Add(row, column, rowCount, columnCount);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.spans.Clear();
        }

        /// <summary>
        /// Finds the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public CellRange Find(int row, int column)
        {
            return this.spans.Find(row, column);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <returns></returns>
        public IEnumerator GetEnumerator(int row, int column, int rowCount, int columnCount)
        {
            return this.spans.GetEnumerator(row, column, rowCount, columnCount);
        }

        /// <summary>
        /// Gets the outside cell range.
        /// </summary>
        /// <returns></returns>
        public CellRange GetOutsideCellRage()
        {
            Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
            IEnumerator enumerator = this.GetEnumerator(-1, -1, -1, -1);
            while (enumerator.MoveNext())
            {
                CellRange current = (CellRange) enumerator.Current;
                empty = Utilities.Union(empty, new Windows.Foundation.Rect((double) current.Column, (double) current.Row, (double) current.ColumnCount, (double) current.RowCount));
            }
            if (!empty.IsEmpty)
            {
                return new CellRange((int) empty.Y, (int) empty.X, (int) empty.Height, (int) empty.Width);
            }
            return null;
        }

        /// <summary>
        /// Gets the value column. Internal only. Find value cell for right overflow.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="area">The area</param>
        /// <param name="row">The row</param>
        /// <param name="column">The column</param>
        /// <param name="spans">The spans</param>
        /// <returns></returns>
        internal static int GetValueColumn(Worksheet worksheet, SheetArea area, int row, int column, SheetSpanModelBase spans)
        {
            if (string.IsNullOrEmpty(worksheet.GetText(row, column, area)) && (spans.Find(row, column) == null))
            {
                for (int i = column + 1; i < worksheet.ColumnCount; i++)
                {
                    if (!string.IsNullOrEmpty(worksheet.GetText(row, i, area)))
                    {
                        return i;
                    }
                }
            }
            return column;
        }

        internal static int GetValueRow(Worksheet worksheet, SheetArea area, int row, int column, SheetSpanModelBase spans)
        {
            if (string.IsNullOrEmpty(worksheet.GetText(row, column, area)) && (spans.Find(row, column) == null))
            {
                for (int i = row + 1; i < worksheet.RowCount; i++)
                {
                    if (!string.IsNullOrEmpty(worksheet.GetText(i, column, area)))
                    {
                        return i;
                    }
                }
            }
            return row;
        }

        /// <summary>
        /// Gets the pure spans.
        /// </summary>
        /// <value>The pure spans.</value>
        public SheetSpanModel PureSpans
        {
            get { return  this.pureSpans; }
        }
    }
}

