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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the full (concrete) implementation of the ISheetSpanModel interface for a worksheet span model, 
    /// which represents cell spans.
    /// </summary>
    public sealed class SheetSpanModel : SheetSpanModelBase, IEnumerable, IXmlSerializable, IRangeSupport, IDataEmptySupport
    {
        /// <summary>
        /// The merged cell range collection.
        /// </summary>
        List<CellRange> items = new List<CellRange>();

        /// <summary>
        /// Adds a cell span to the collection.
        /// </summary>
        /// <param name="row">The row index at which to start the span.</param>
        /// <param name="column">The column index at which to start the span.</param>
        /// <param name="rowCount">The number of rows in the span.</param>
        /// <param name="columnCount">The number of columns in the span.</param>
        /// <returns>
        /// <c>true</c> if the span is added successfully; otherwise, <c>false</c>.
        /// </returns>
        public override bool Add(int row, int column, int rowCount, int columnCount)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException("row");
            }
            if (column < 0)
            {
                throw new ArgumentOutOfRangeException("column");
            }
            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException("rowCount");
            }
            if (columnCount < 1)
            {
                throw new ArgumentOutOfRangeException("columnCount");
            }
            CellRange range = new CellRange(row, column, rowCount, columnCount);
            int num = 0;
            int num2 = -1;
            for (num = 0; num < this.items.Count; num++)
            {
                CellRange range2 = this.items[num];
                if (((range2.Row == range.Row) && (range2.Column == range.Column)) && ((range2.RowCount != range.RowCount) || (range2.ColumnCount != range.ColumnCount)))
                {
                    num2 = num;
                    this.items[num] = range;
                    this.RaiseChanged(range.Row, range.Column, range.RowCount, range.ColumnCount, SheetSpanModelChangedEventAction.SpanUpdated);
                    break;
                }
                if (range2.Intersects(row, column, rowCount, columnCount) && !range.Contains(range2.Row, range2.Column))
                {
                    return false;
                }
            }
            num = 0;
            while (num < this.items.Count)
            {
                CellRange range3 = this.items[num];
                if (range.Contains(range3.Row, range3.Column) && (range3 != range))
                {
                    this.items.RemoveAt(num);
                    this.RaiseChanged(range3.Row, range3.Column, range3.RowCount, range3.ColumnCount, SheetSpanModelChangedEventAction.SpanRemoved);
                }
                else
                {
                    num++;
                }
            }
            if (num2 == -1)
            {
                this.items.Add(new CellRange(row, column, rowCount, columnCount));
                this.RaiseChanged(row, column, rowCount, columnCount, SheetSpanModelChangedEventAction.SpanAdded);
            }
            return true;
        }

        /// <summary>
        /// Removes all cell spans from the collection.
        /// </summary>
        public override void Clear()
        {
            this.items.Clear();
            this.RaiseChanged(0, 0, 0, 0, SheetSpanModelChangedEventAction.ModelUpdated);
        }

        /// <summary>
        /// Removes cell spans from the collection within the specified area. 
        /// </summary>
        /// <param name="row">The row index from which to start clearing cell spans.</param>
        /// <param name="column">The column index from which to start clearing cell spans.</param>
        /// <param name="rowCount">The number of rows to clear.</param>
        /// <param name="columnCount">The number of columns to clear.</param>
        public void Clear(int row, int column, int rowCount, int columnCount)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                CellRange range = this.items[i];
                if (((row == -1) || ((row <= range.Row) && (range.Row < (row + rowCount)))) && ((column == -1) || ((column <= range.Column) && (range.Column < (column + columnCount)))))
                {
                    this.items.RemoveAt(i--);
                    this.RaiseChanged(range.Row, range.Column, range.RowCount, range.ColumnCount, SheetSpanModelChangedEventAction.SpanRemoved);
                }
            }
        }

        /// <summary>
        /// Clones the specified items.
        /// </summary>
        /// <param name="items">The items</param>
        /// <returns>The cloned items</returns>
        static List<CellRange> Clone(List<CellRange> items)
        {
            if (items == null)
            {
                return null;
            }
            return new List<CellRange>((IEnumerable<CellRange>) items);
        }

        /// <summary>
        /// Copies a cell span and pastes it at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin copying.</param>
        /// <param name="fromColumn">The column index from which to begin copying.</param>
        /// <param name="toRow">The row index at which to paste the cell span.</param>
        /// <param name="toColumn">The column index at which to paste the cell span.</param>
        /// <param name="rowCount">The number of rows to copy.</param>
        /// <param name="columnCount">The number of columns to copy.</param>
        public void Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            CellRange range;
            bool flag = false;
            int num = this.items.Count;
            if (fromRow == -1)
            {
                List<CellRange> list = new List<CellRange>();
                for (int i = 0; i < num; i++)
                {
                    range = this.items[i];
                    if ((fromColumn <= range.Column) && (range.Column < (fromColumn + columnCount)))
                    {
                        list.Add(new CellRange(range.Row, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount));
                        flag = true;
                    }
                    else if ((toColumn <= range.Column) && (range.Column < (toColumn + columnCount)))
                    {
                        this.items.RemoveAt(i);
                        i--;
                        num--;
                        flag = true;
                    }
                }
                foreach (CellRange range2 in list)
                {
                    if (!this.IsValid(this.items, 0, this.items.Count, range2))
                    {
                        throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                    }
                    this.items.Add(range2);
                }
            }
            else if (fromColumn == -1)
            {
                List<CellRange> list2 = new List<CellRange>();
                for (int j = 0; j < num; j++)
                {
                    range = this.items[j];
                    if ((fromRow <= range.Row) && (range.Row < (fromRow + rowCount)))
                    {
                        list2.Add(new CellRange((toRow + range.Row) - fromRow, range.Column, range.RowCount, range.ColumnCount));
                        flag = true;
                    }
                    else if ((toRow <= range.Row) && (range.Row < (toRow + rowCount)))
                    {
                        this.items.RemoveAt(j);
                        j--;
                        num--;
                        flag = true;
                    }
                }
                foreach (CellRange range3 in list2)
                {
                    if (!this.IsValid(this.items, 0, this.items.Count, range3))
                    {
                        throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                    }
                    this.items.Add(range3);
                }
            }
            else
            {
                List<CellRange> list3 = new List<CellRange>();
                for (int k = 0; k < num; k++)
                {
                    range = this.items[k];
                    if (((fromRow <= range.Row) && (range.Row < (fromRow + rowCount))) && ((fromColumn <= range.Column) && (range.Column < (fromColumn + columnCount))))
                    {
                        list3.Add(new CellRange((toRow + range.Row) - fromRow, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount));
                        flag = true;
                    }
                    else if (((toRow <= range.Row) && (range.Row < (toRow + rowCount))) && ((toColumn <= range.Column) && (range.Column < (toColumn + columnCount))))
                    {
                        this.items.RemoveAt(k);
                        k--;
                        num--;
                        flag = true;
                    }
                }
                foreach (CellRange range4 in list3)
                {
                    if (!this.IsValid(this.items, 0, this.items.Count, range4))
                    {
                        throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                    }
                    this.items.Add(range4);
                }
            }
            if (flag)
            {
                this.RaiseChanged(toRow, toColumn, rowCount, columnCount, SheetSpanModelChangedEventAction.SpanUpdated);
            }
        }

        /// <summary>
        /// Copies columns and pastes them at the specified location.
        /// </summary>
        /// <param name="fromColumn">The column index at which to start copying.</param>
        /// <param name="toColumn">The column index at which to paste columns.</param>
        /// <param name="count">The number of columns to copy.</param>
        public void CopyColumns(int fromColumn, int toColumn, int count)
        {
            List<CellRange> list = Clone(this.items);
            bool flag = false;
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if ((fromColumn <= range.Column) && ((range.Column + range.ColumnCount) <= (fromColumn + count)))
                {
                    CellRange cellRange = new CellRange(range.Row, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount);
                    if (!this.IsValid(list, 0, list.Count, cellRange))
                    {
                        throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                    }
                    list.Add(cellRange);
                    flag = true;
                }
            }
            if (flag)
            {
                this.items = list;
            }
            if (flag)
            {
                this.RaiseChanged(-1, toColumn, -1, count, SheetSpanModelChangedEventAction.SpanUpdated);
            }
        }

        /// <summary>
        /// Copies rows and pastes them at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index at which to start copying.</param>
        /// <param name="toRow">The row index at which to paste columns.</param>
        /// <param name="count">The number of rows to copy.</param>
        public void CopyRows(int fromRow, int toRow, int count)
        {
            List<CellRange> list = Clone(this.items);
            bool flag = false;
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if ((fromRow <= range.Row) && ((range.Row + range.RowCount) <= (fromRow + count)))
                {
                    CellRange cellRange = new CellRange((toRow + range.Row) - fromRow, range.Column, range.RowCount, range.ColumnCount);
                    if (!this.IsValid(list, 0, list.Count, cellRange))
                    {
                        throw new Exception(ResourceStrings.SpanModelOverlappingError);
                    }
                    list.Add(cellRange);
                    flag = true;
                }
            }
            if (flag)
            {
                this.items = list;
            }
            if (flag)
            {
                this.RaiseChanged(toRow, -1, count, -1, SheetSpanModelChangedEventAction.SpanUpdated);
            }
        }

        /// <summary>
        /// Finds the cell span with the specified anchor cell in the collection.
        /// </summary>
        /// <param name="row">The row index of the starting cell for the span.</param>
        /// <param name="column">The column index of the starting cell for the span.</param>
        /// <returns>Returns the cell range of the span.</returns>
        public override CellRange Find(int row, int column)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].Contains(row, column))
                {
                    return this.items[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new SpanEnumerator(this, -1, -1, -1, -1);
        }

        /// <summary>
        /// Gets an enumerator for iterating to the next cell span in the collection after the specified span.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="rowCount">The number of rows in the cell span.</param>
        /// <param name="columnCount">The number of columns in the cell span.</param>
        /// <returns>
        /// Returns an enumerator to enumerate the span information for this model.
        /// </returns>
        public override IEnumerator GetEnumerator(int row, int column, int rowCount, int columnCount)
        {
            return new SpanEnumerator(this, row, column, rowCount, columnCount);
        }

        /// <summary>
        /// This IRangeSupport method is used internally by Spread to
        /// keep the row and column counts synchronized between the models
        /// and is not intended to be called directly from your code.
        /// Instead, use the IRangeSupport methods in the data model
        /// or the wrapper methods in the sheet view.
        /// </summary>
        /// <param name="column">The column index at which to start adding columns.</param>
        /// <param name="count">The number of columns to add to the span.</param>
        void IRangeSupport.AddColumns(int column, int count)
        {
            bool flag = false;
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Column >= column)
                {
                    this.items[i] = new CellRange(range.Row, range.Column + count, range.RowCount, range.ColumnCount);
                    flag = true;
                }
                else if ((range.Column < column) && (column < (range.Column + range.ColumnCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount + count);
                    flag = true;
                }
            }
            if (flag)
            {
                this.RaiseChanged(-1, column, 0, count, SheetSpanModelChangedEventAction.ModelUpdated);
            }
        }

        /// <summary>
        /// This IRangeSupport method is used internally by Spread to
        /// keep the row and column counts synchronized between the models
        /// and is not intended to be called directly from your code.
        /// Instead, use the IRangeSupport methods in the data model 
        /// or the wrapper methods in the sheet view.
        /// </summary>
        /// <param name="row">The row index at which to start adding rows.</param>
        /// <param name="count">The number of rows to add to the span.</param>
        void IRangeSupport.AddRows(int row, int count)
        {
            bool flag = false;
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Row >= row)
                {
                    this.items[i] = new CellRange(range.Row + count, range.Column, range.RowCount, range.ColumnCount);
                    flag = true;
                }
                else if ((range.Row < row) && (row < (range.Row + range.RowCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount + count, range.ColumnCount);
                    flag = true;
                }
            }
            if (flag)
            {
                this.RaiseChanged(row, -1, count, 0, SheetSpanModelChangedEventAction.ModelUpdated);
            }
        }

        /// <summary>
        /// This IRangeSupport method is used internally by Spread to
        /// keep the row and column counts synchronized between the models
        /// and is not intended to be called directly from your code.
        /// Instead, use the IRangeSupport methods in the data model
        /// or the wrapper methods in the SheetView.
        /// </summary>
        /// <param name="column">The column index at which to start removing columns.</param>
        /// <param name="count">The number of columns to remove from the cell span.</param>
        void IRangeSupport.RemoveColumns(int column, int count)
        {
            bool flag = false;
            List<CellRange> list = new List<CellRange>();
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Column >= column)
                {
                    if (range.Column < (column + count))
                    {
                        list.Add(range);
                        flag = true;
                    }
                    else
                    {
                        this.items[i] = new CellRange(range.Row, range.Column - count, range.RowCount, range.ColumnCount);
                        flag = true;
                    }
                }
                else if ((range.Column < column) && (column < (range.Column + range.ColumnCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount - Math.Min((range.Column + range.ColumnCount) - column, count));
                    flag = true;
                }
            }
            foreach (CellRange range2 in list)
            {
                this.items.Remove(range2);
            }
            if (flag)
            {
                this.RaiseChanged(-1, column, 0, count, SheetSpanModelChangedEventAction.ModelUpdated);
            }
        }

        /// <summary>
        /// This IRangeSupport method is used internally by Spread to
        /// keep the row and column counts synchronized between the models
        /// and is not intended to be called directly from your code.
        /// Instead, use the IRangeSupport methods in the data model
        /// or the wrapper methods in the sheet view.
        /// </summary>
        /// <param name="row">The row index at which to start removing rows.</param>
        /// <param name="count">The number of rows to remove from the cell span.</param>
        void IRangeSupport.RemoveRows(int row, int count)
        {
            bool flag = false;
            List<CellRange> list = new List<CellRange>();
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Row >= row)
                {
                    if (range.Row < (row + count))
                    {
                        list.Add(range);
                        flag = true;
                    }
                    else
                    {
                        this.items[i] = new CellRange(range.Row - count, range.Column, range.RowCount, range.ColumnCount);
                        flag = true;
                    }
                }
                else if ((range.Row < row) && (row < (range.Row + range.RowCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount - Math.Min((range.Row + range.RowCount) - row, count), range.ColumnCount);
                    flag = true;
                }
            }
            foreach (CellRange range2 in list)
            {
                this.items.Remove(range2);
            }
            if (flag)
            {
                this.RaiseChanged(row, -1, count, 0, SheetSpanModelChangedEventAction.ModelUpdated);
            }
        }

        /// <summary>
        /// Determines whether the model is empty of cell spans.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this model is empty of cell spans; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsEmpty()
        {
            return (this.items.Count == 0);
        }

        /// <summary>
        /// Determines whether the specified list is valid.
        /// </summary>
        /// <param name="list">The cell range list</param>
        /// <param name="start">The start index</param>
        /// <param name="end">The end index</param>
        /// <param name="cellRange">The cell range</param>
        /// <returns>
        /// <c>true</c> if the specified list is valid; otherwise, <c>false</c>
        /// </returns>
        bool IsValid(List<CellRange> list, int start, int end, CellRange cellRange)
        {
            for (int i = start; (i < end) && (i < list.Count); i++)
            {
                if (list[i].Intersects(cellRange.Row, cellRange.Column, cellRange.RowCount, cellRange.ColumnCount))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Moves a cell span and pastes it at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the move.</param>
        /// <param name="fromColumn">The column index from which to begin the move.</param>
        /// <param name="toRow">The row index at which to paste the cell span.</param>
        /// <param name="toColumn">The column index at which to paste the cell span.</param>
        /// <param name="rowCount">The number of rows to move.</param>
        /// <param name="columnCount">The number of columns to move.</param>
        public void Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            CellRange range;
            bool flag = false;
            List<CellRange> list = new List<CellRange>();
            List<CellRange> list2 = new List<CellRange>();
            int num = this.items.Count;
            if (fromRow == -1)
            {
                for (int i = 0; i < num; i++)
                {
                    range = this.items[i];
                    if ((fromColumn <= range.Column) && (range.Column < (fromColumn + columnCount)))
                    {
                        CellRange range2 = new CellRange(range.Row, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount);
                        list2.Add(range2);
                        flag = true;
                    }
                    else if ((toColumn < range.Column) && (range.Column < (toColumn + columnCount)))
                    {
                        flag = true;
                    }
                    else
                    {
                        list.Add(range);
                    }
                }
            }
            else if (fromColumn == -1)
            {
                for (int j = 0; j < num; j++)
                {
                    range = this.items[j];
                    if ((fromRow <= range.Row) && (range.Row < (fromRow + rowCount)))
                    {
                        CellRange range3 = new CellRange((toRow + range.Row) - fromRow, range.Column, range.RowCount, range.ColumnCount);
                        list2.Add(range3);
                        flag = true;
                    }
                    else if ((toRow <= range.Row) && (range.Row < (toRow + rowCount)))
                    {
                        flag = true;
                    }
                    else
                    {
                        list.Add(range);
                    }
                }
            }
            else
            {
                for (int k = 0; k < num; k++)
                {
                    range = this.items[k];
                    if (((fromRow <= range.Row) && (range.Row < (fromRow + rowCount))) && ((fromColumn <= range.Column) && (range.Column < (fromColumn + columnCount))))
                    {
                        CellRange range4 = new CellRange((toRow + range.Row) - fromRow, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount);
                        list2.Add(range4);
                        flag = true;
                    }
                    else if (((toRow <= range.Row) && (range.Row < (toRow + rowCount))) && ((toColumn <= range.Column) && (range.Column < (toColumn + columnCount))))
                    {
                        flag = true;
                    }
                    else
                    {
                        list.Add(range);
                    }
                }
            }
            if (flag)
            {
                if (list2.Count > 0)
                {
                    foreach (CellRange range5 in list2)
                    {
                        if (!this.IsValid(list, 0, list.Count, range5))
                        {
                            throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                        }
                        list.Add(range5);
                    }
                }
                this.items = list;
            }
            if (flag)
            {
                this.RaiseChanged(fromRow, fromColumn, rowCount, columnCount, SheetSpanModelChangedEventAction.SpanUpdated);
                this.RaiseChanged(toRow, toColumn, rowCount, columnCount, SheetSpanModelChangedEventAction.SpanUpdated);
            }
        }

        /// <summary>
        /// Moves columns and pastes them at the specified location. 
        /// </summary>
        /// <param name="fromColumn">The column index at which to start the move.</param>
        /// <param name="toColumn">The column index at which to paste columns.</param>
        /// <param name="count">The number of columns to move.</param>
        public void MoveColumns(int fromColumn, int toColumn, int count)
        {
            if (fromColumn != toColumn)
            {
                List<CellRange> list = new List<CellRange>();
                List<CellRange> list2 = new List<CellRange>();
                bool flag = false;
                if (toColumn < fromColumn)
                {
                    int num = this.items.Count;
                    for (int i = 0; i < num; i++)
                    {
                        CellRange range = this.items[i];
                        if ((range.Column >= fromColumn) && (range.Column < (fromColumn + count)))
                        {
                            CellRange range2 = new CellRange(range.Row, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount);
                            list2.Add(range2);
                            flag = true;
                        }
                        else if ((range.Column >= toColumn) && (range.Column < fromColumn))
                        {
                            CellRange range3 = new CellRange(range.Row, range.Column + count, range.RowCount, range.ColumnCount);
                            list2.Add(range3);
                            flag = true;
                        }
                        else
                        {
                            list.Add(range);
                        }
                    }
                }
                else
                {
                    int num3 = this.items.Count;
                    for (int j = 0; j < num3; j++)
                    {
                        CellRange range4 = this.items[j];
                        if ((range4.Column >= fromColumn) && (range4.Column < (fromColumn + count)))
                        {
                            CellRange range5 = new CellRange(range4.Row, (toColumn + range4.Column) - fromColumn, range4.RowCount, range4.ColumnCount);
                            list2.Add(range5);
                            flag = true;
                        }
                        else if ((range4.Column >= (fromColumn + count)) && (range4.Column < toColumn))
                        {
                            CellRange range6 = new CellRange(range4.Row, range4.Column - count, range4.RowCount, range4.ColumnCount);
                            list2.Add(range6);
                            flag = true;
                        }
                        else
                        {
                            list.Add(range4);
                        }
                    }
                }
                if (list2.Count > 0)
                {
                    foreach (CellRange range7 in list2)
                    {
                        if (!this.IsValid(list, 0, list.Count, range7))
                        {
                            throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                        }
                        list.Add(range7);
                    }
                    this.items = list;
                }
                if (flag)
                {
                    this.RaiseChanged(-1, fromColumn, 0, count, SheetSpanModelChangedEventAction.ModelUpdated);
                    this.RaiseChanged(-1, toColumn, 0, count, SheetSpanModelChangedEventAction.ModelUpdated);
                }
            }
        }

        /// <summary>
        /// Moves rows and pastes them at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index at which to start the move.</param>
        /// <param name="toRow">The row index at which to paste rows.</param>
        /// <param name="count">The number of rows to move.</param>
        public void MoveRows(int fromRow, int toRow, int count)
        {
            if (fromRow != toRow)
            {
                List<CellRange> list = new List<CellRange>();
                List<CellRange> list2 = new List<CellRange>();
                bool flag = false;
                if (toRow < fromRow)
                {
                    int num = this.items.Count;
                    for (int i = 0; i < num; i++)
                    {
                        CellRange range = this.items[i];
                        if ((range.Row >= fromRow) && (range.Row < (fromRow + count)))
                        {
                            CellRange range2 = new CellRange((toRow + range.Row) - fromRow, range.Column, range.RowCount, range.ColumnCount);
                            list2.Add(range2);
                            flag = true;
                        }
                        else if ((range.Row >= toRow) && (range.Row < fromRow))
                        {
                            CellRange range3 = new CellRange(range.Row + count, range.Column, range.RowCount, range.ColumnCount);
                            list2.Add(range3);
                            flag = true;
                        }
                        else
                        {
                            list.Add(range);
                        }
                    }
                }
                else
                {
                    int num3 = this.items.Count;
                    for (int j = 0; j < num3; j++)
                    {
                        CellRange range4 = this.items[j];
                        if ((range4.Row >= fromRow) && (range4.Row < (fromRow + count)))
                        {
                            CellRange range5 = new CellRange((toRow + range4.Row) - fromRow, range4.Column, range4.RowCount, range4.ColumnCount);
                            list2.Add(range5);
                            flag = true;
                        }
                        else if ((range4.Row >= (fromRow + count)) && (range4.Row < toRow))
                        {
                            CellRange range6 = new CellRange(range4.Row - count, range4.Column, range4.RowCount, range4.ColumnCount);
                            list2.Add(range6);
                            flag = true;
                        }
                        else
                        {
                            list.Add(range4);
                        }
                    }
                }
                if (list2.Count > 0)
                {
                    foreach (CellRange range7 in list2)
                    {
                        if (!this.IsValid(list, 0, list.Count, range7))
                        {
                            throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                        }
                        list.Add(range7);
                    }
                    this.items = list;
                }
                if (flag)
                {
                    this.RaiseChanged(fromRow, -1, count, 0, SheetSpanModelChangedEventAction.ModelUpdated);
                    this.RaiseChanged(toRow, -1, count, 0, SheetSpanModelChangedEventAction.ModelUpdated);
                }
            }
        }

        /// <summary>
        /// Returns information about the event that was raised. 
        /// </summary>
        /// <param name="e"><see cref="T:Dt.Cells.Data.SheetSpanModelChangedEventArgs" /> object that contains event data.</param>
        void OnChanged(SheetSpanModelChangedEventArgs e)
        {
            base.RaiseChanged(e);
        }

        /// <summary>
        /// Returns information about the cell span that has changed. 
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="rowCount">The number of rows in the span.</param>
        /// <param name="columnCount">The number of columns in the span.</param>
        /// <param name="type">Returns the type of event that was raised.</param>
        void RaiseChanged(int row, int column, int rowCount, int columnCount, SheetSpanModelChangedEventAction type)
        {
            this.OnChanged(new SheetSpanModelChangedEventArgs(row, column, rowCount, columnCount, type));
        }

        /// <summary>
        /// Removes the cell span with the specified anchor cell from the collection.
        /// </summary>
        /// <param name="row">The row index of the starting cell.</param>
        /// <param name="column">The column index of the starting cell.</param>
        public override void Remove(int row, int column)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException("row");
            }
            if (column < 0)
            {
                throw new ArgumentOutOfRangeException("column");
            }
            for (int i = 0; i < this.items.Count; i++)
            {
                CellRange range = this.items[i];
                if ((range.Row == row) && (range.Column == column))
                {
                    this.items.RemoveAt(i--);
                    this.RaiseChanged(row, column, range.RowCount, range.ColumnCount, SheetSpanModelChangedEventAction.SpanRemoved);
                }
            }
        }

        /// <summary>
        /// Swaps cell spans. 
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the swap.</param>
        /// <param name="fromColumn">The column index from which to begin the swap.</param>
        /// <param name="toRow">The row index at which to get the cell span to swap.</param>
        /// <param name="toColumn">The column index at which to get the cell span to swap.</param>
        /// <param name="rowCount">The number of rows to swap.</param>
        /// <param name="columnCount">The number of columns to swap.</param>
        public void Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            CellRange range;
            bool flag = false;
            List<CellRange> list = new List<CellRange>();
            List<CellRange> list2 = new List<CellRange>();
            int num = this.items.Count;
            if (fromRow == -1)
            {
                for (int i = 0; i < num; i++)
                {
                    range = this.items[i];
                    if ((fromColumn <= range.Column) && (range.Column < (fromColumn + columnCount)))
                    {
                        CellRange range2 = new CellRange(range.Row, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount);
                        list2.Add(range2);
                        flag = true;
                    }
                    else if ((toColumn <= range.Column) && (range.Column < (toColumn + columnCount)))
                    {
                        CellRange range3 = new CellRange(range.Row, (fromColumn + range.Column) - toColumn, range.RowCount, range.ColumnCount);
                        list2.Add(range3);
                        flag = true;
                    }
                    else
                    {
                        list.Add(range);
                    }
                }
            }
            else if (fromColumn == -1)
            {
                for (int j = 0; j < num; j++)
                {
                    range = this.items[j];
                    if ((fromRow <= range.Row) && (range.Row < (fromRow + rowCount)))
                    {
                        CellRange range4 = new CellRange((toRow + range.Row) - fromRow, range.Column, range.RowCount, range.ColumnCount);
                        list2.Add(range4);
                        flag = true;
                    }
                    else if ((toRow <= range.Row) && (range.Row < (toRow + rowCount)))
                    {
                        CellRange range5 = new CellRange((fromRow + range.Row) - toRow, range.Column, range.RowCount, range.ColumnCount);
                        list2.Add(range5);
                        flag = true;
                    }
                    else
                    {
                        list.Add(range);
                    }
                }
            }
            else
            {
                for (int k = 0; k < num; k++)
                {
                    range = this.items[k];
                    if (((fromRow <= range.Row) && (range.Row < (fromRow + rowCount))) && ((fromColumn <= range.Column) && (range.Column < (fromColumn + columnCount))))
                    {
                        CellRange range6 = new CellRange((toRow + range.Row) - fromRow, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount);
                        list2.Add(range6);
                        flag = true;
                    }
                    else if (((toRow <= range.Row) && (range.Row < (toRow + rowCount))) && ((toColumn <= range.Column) && (range.Column < (toColumn + columnCount))))
                    {
                        CellRange range7 = new CellRange((fromRow + range.Row) - toRow, (fromColumn + range.Column) - toColumn, range.RowCount, range.ColumnCount);
                        list2.Add(range7);
                        flag = true;
                    }
                    else
                    {
                        list.Add(range);
                    }
                }
            }
            if (flag)
            {
                if (list2.Count > 0)
                {
                    foreach (CellRange range8 in list2)
                    {
                        if (!this.IsValid(list, 0, list.Count, range8))
                        {
                            throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                        }
                        list.Add(range8);
                    }
                }
                this.items = list;
            }
            if (flag)
            {
                this.RaiseChanged(fromRow, fromColumn, rowCount, columnCount, SheetSpanModelChangedEventAction.SpanUpdated);
                this.RaiseChanged(toRow, toColumn, rowCount, columnCount, SheetSpanModelChangedEventAction.SpanUpdated);
            }
        }

        /// <summary>
        /// Swaps a range of columns with another range of columns.
        /// </summary>
        /// <param name="fromColumn">The column index at which to start the swap.</param>
        /// <param name="toColumn">The column index at which to swap columns.</param>
        /// <param name="count">The number of columns to swap.</param>
        public void SwapColumns(int fromColumn, int toColumn, int count)
        {
            bool flag = false;
            List<CellRange> list = new List<CellRange>();
            List<CellRange> list2 = new List<CellRange>();
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if ((fromColumn <= range.Column) && (range.Column < (fromColumn + count)))
                {
                    CellRange range2 = new CellRange(range.Row, (toColumn + range.Column) - fromColumn, range.RowCount, range.ColumnCount);
                    list2.Add(range2);
                    flag = true;
                }
                else if ((toColumn <= range.Column) && (range.Column < (toColumn + count)))
                {
                    CellRange range3 = new CellRange(range.Row, (fromColumn + range.Column) - toColumn, range.RowCount, range.ColumnCount);
                    list2.Add(range3);
                    flag = true;
                }
                else
                {
                    list.Add(range);
                }
            }
            if (list2.Count > 0)
            {
                foreach (CellRange range4 in list2)
                {
                    if (!this.IsValid(list, 0, list.Count, range4))
                    {
                        throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                    }
                    list.Add(range4);
                }
                this.items = list;
            }
            if (flag)
            {
                this.RaiseChanged(-1, fromColumn, 0, count, SheetSpanModelChangedEventAction.ModelUpdated);
                this.RaiseChanged(-1, toColumn, 0, count, SheetSpanModelChangedEventAction.ModelUpdated);
            }
        }

        /// <summary>
        /// Swaps a range of rows with another range of rows.
        /// </summary>
        /// <param name="fromRow">The row index at which to start the swap.</param>
        /// <param name="toRow">The row index at which to swap rows.</param>
        /// <param name="count">The number of rows to swap.</param>
        public void SwapRows(int fromRow, int toRow, int count)
        {
            bool flag = false;
            List<CellRange> list = new List<CellRange>();
            List<CellRange> list2 = new List<CellRange>();
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if ((fromRow <= range.Row) && (range.Row < (fromRow + count)))
                {
                    CellRange range2 = new CellRange((toRow + range.Row) - fromRow, range.Column, range.RowCount, range.ColumnCount);
                    list2.Add(range2);
                    flag = true;
                }
                else if ((toRow <= range.Row) && (range.Row < (toRow + count)))
                {
                    CellRange range3 = new CellRange((fromRow + range.Row) - toRow, range.Column, range.RowCount, range.ColumnCount);
                    list2.Add(range3);
                    flag = true;
                }
                else
                {
                    list.Add(range);
                }
            }
            if (list2.Count > 0)
            {
                foreach (CellRange range4 in list2)
                {
                    if (!this.IsValid(list, 0, list.Count, range4))
                    {
                        throw new InvalidOperationException(ResourceStrings.SpanModelOverlappingError);
                    }
                    list.Add(range4);
                }
                this.items = list;
            }
            if (flag)
            {
                this.RaiseChanged(fromRow, -1, count, 0, SheetSpanModelChangedEventAction.ModelUpdated);
                this.RaiseChanged(toRow, -1, count, 0, SheetSpanModelChangedEventAction.ModelUpdated);
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
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
            this.items.Clear();
            bool flag = false;
            while (reader.Read())
            {
                if (reader.NodeType != ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    goto Label_0101;
                }
                string name = reader.Name;
                if (name != null)
                {
                    if (name == "Items")
                    {
                        flag = true;
                    }
                    else if (name == "Item")
                    {
                        goto Label_005A;
                    }
                }
                continue;
            Label_005A:
                if (!flag)
                {
                    continue;
                }
                if (this.items == null)
                {
                    this.items = new List<CellRange>();
                }
                int? nullable = Serializer.ReadAttributeInt("r", reader);
                int? nullable2 = Serializer.ReadAttributeInt("c", reader);
                int? nullable3 = Serializer.ReadAttributeInt("rc", reader);
                int? nullable4 = Serializer.ReadAttributeInt("cc", reader);
                if ((nullable.HasValue && nullable2.HasValue) && (nullable3.HasValue && nullable4.HasValue))
                {
                    this.items.Add(new CellRange(nullable.Value, nullable2.Value, nullable3.Value, nullable4.Value));
                    continue;
                }
                throw new FormatException(ResourceStrings.SerializationError);
            Label_0101:
                if ((reader.Name == "Items") && (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.EndElement))))
                {
                    flag = false;
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
            if ((this.items != null) && (this.items.Count > 0))
            {
                Serializer.WriteStartObj("Items", writer);
                foreach (CellRange range in this.items)
                {
                    writer.WriteStartElement("Item");
                    Serializer.WriteAttr("r", (int) range.Row, writer);
                    Serializer.WriteAttr("c", (int) range.Column, writer);
                    Serializer.WriteAttr("rc", (int) range.RowCount, writer);
                    Serializer.WriteAttr("cc", (int) range.ColumnCount, writer);
                    writer.WriteEndElement();
                }
                Serializer.WriteEndObj(writer);
            }
        }

        bool IDataEmptySupport.IsDataEmpty
        {
            get { return  this.IsEmpty(); }
        }

        /// <summary>
        /// The SpanEnumerator Class
        /// </summary>
        class SpanEnumerator : IEnumerator
        {
            /// <summary>
            /// Whether all cell range.
            /// </summary>
            bool all;
            /// <summary>
            /// The column index.
            /// </summary>
            int column;
            /// <summary>
            /// The column count.
            /// </summary>
            int columnCount;
            /// <summary>
            /// The current index.
            /// </summary>
            int currentIndex;
            /// <summary>
            /// The Sheet span model.
            /// </summary>
            SheetSpanModel model;
            /// <summary>
            /// The row index.
            /// </summary>
            int row;
            /// <summary>
            /// The row count.
            /// </summary>
            int rowCount;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SheetSpanModel.SpanEnumerator" /> class.
            /// </summary>
            /// <param name="model">The model.</param>
            /// <param name="row">The row index.</param>
            /// <param name="column">The column index.</param>
            /// <param name="rowCount">The row count.</param>
            /// <param name="columnCount">The column count.</param>
            public SpanEnumerator(SheetSpanModel model, int row, int column, int rowCount, int columnCount)
            {
                this.model = model;
                this.row = row;
                this.column = column;
                this.rowCount = rowCount;
                this.columnCount = columnCount;
                this.currentIndex = -1;
                if ((row == -1) && (column == -1))
                {
                    this.all = true;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                this.currentIndex++;
                if (!this.all)
                {
                    while ((this.currentIndex < this.model.items.Count) && !this.model.items[this.currentIndex].Intersects(this.row, this.column, this.rowCount, this.columnCount))
                    {
                        this.currentIndex++;
                    }
                }
                return (this.currentIndex < this.model.items.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                this.currentIndex = -1;
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element or the collection was modified after the enumerator was created.</exception>
            public object Current
            {
                get
                {
                    if ((0 > this.currentIndex) || (this.currentIndex >= this.model.items.Count))
                    {
                        throw new InvalidOperationException();
                    }
                    return this.model.items[this.currentIndex];
                }
            }
        }
    }
}

