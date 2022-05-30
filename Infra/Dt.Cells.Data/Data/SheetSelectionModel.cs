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
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the full (concrete) implementation of the ISheetSelectionModel interface for a selection model for a sheet, 
    /// which represents cell selections. 
    /// </summary>
    internal sealed class SheetSelectionModel : SheetSelectionModelBase, IEnumerable, IXmlSerializable, IDataEmptySupport, IRangeSupport
    {
        /// <summary>
        /// anchor cell column.
        /// </summary>
        int anchorColumn;
        /// <summary>
        /// anchor cell row.
        /// </summary>
        int anchorRow;
        /// <summary>
        /// the selected cell ranges.
        /// </summary>
        List<CellRange> items = new List<CellRange>();
        /// <summary>
        /// the selection policy.
        /// </summary>
        Dt.Cells.Data.SelectionPolicy selectionPolicy = Dt.Cells.Data.SelectionPolicy.MultiRange;
        /// <summary>
        /// the selection unit.
        /// </summary>
        Dt.Cells.Data.SelectionUnit selectionUnit;

        /// <summary>
        /// Adds a cell or cells to the selection.
        /// </summary>
        /// <param name="row">The row index of the first cell to add.</param>
        /// <param name="column">The column index of the first cell to add.</param>
        /// <param name="rowCount">The number of rows to add.</param>
        /// <param name="columnCount">The number of columns to add.</param>
        public override void AddSelection(int row, int column, int rowCount, int columnCount)
        {
            this.AddSelection(row, column, rowCount, columnCount, true);
        }

        /// <summary>
        /// Adds a cell or cells to the selection and can move the anchor cell.
        /// </summary>
        /// <param name="row">The row index of the first cell to add.</param>
        /// <param name="column">The column index of the first cell to add.</param>
        /// <param name="rowCount">The number of rows to add.</param>
        /// <param name="columnCount">The number of columns to add.</param>
        /// <param name="moveAnchorCell">If set to <c>true</c>, moves the anchor cell to the top-left corner of the selection area.</param>
        public override void AddSelection(int row, int column, int rowCount, int columnCount, bool moveAnchorCell)
        {
            if (row < -1)
            {
                throw new ArgumentOutOfRangeException("row");
            }
            if (column < -1)
            {
                throw new ArgumentOutOfRangeException("column");
            }
            if ((row != -1) && (rowCount < 1))
            {
                throw new ArgumentOutOfRangeException("rowCount");
            }
            if ((column != -1) && (columnCount < 1))
            {
                throw new ArgumentOutOfRangeException("columnCount");
            }
            if (moveAnchorCell)
            {
                this.anchorRow = (row >= 0) ? row : 0;
                this.anchorColumn = (column >= 0) ? column : 0;
            }
            if (this.selectionPolicy == Dt.Cells.Data.SelectionPolicy.Single)
            {
                rowCount = Math.Min(rowCount, 1);
                columnCount = Math.Min(columnCount, 1);
                this.ClearSelection();
            }
            else if (this.selectionPolicy == Dt.Cells.Data.SelectionPolicy.Range)
            {
                this.ClearSelection();
            }
            if (this.selectionUnit == Dt.Cells.Data.SelectionUnit.Row)
            {
                column = -1;
                columnCount = -1;
            }
            else if (this.selectionUnit == Dt.Cells.Data.SelectionUnit.Column)
            {
                row = -1;
                rowCount = -1;
            }
            this.items.Add(new CellRange(row, column, rowCount, columnCount));
            base.FireChanged(row, column, rowCount, columnCount);
        }

        /// <summary>
        /// Removes all the selections from the worksheet so that cells are no longer selected.
        /// </summary>
        public override void ClearSelection()
        {
            while (this.items.Count > 0)
            {
                CellRange range = this.items[this.items.Count - 1];
                this.items.RemoveAt(this.items.Count - 1);
                base.FireChanged(range.Row, range.Column, range.RowCount, range.ColumnCount);
            }
        }

        internal void CopyFrom(SheetSelectionModel model)
        {
            this.items = (model == null) ? new List<CellRange>() : model.items;
            this.anchorRow = (model == null) ? 0 : model.anchorRow;
            this.anchorColumn = (model == null) ? 0 : model.anchorColumn;
            this.selectionPolicy = (model == null) ? Dt.Cells.Data.SelectionPolicy.MultiRange : model.selectionPolicy;
            this.selectionUnit = (model == null) ? Dt.Cells.Data.SelectionUnit.Cell : model.selectionUnit;
        }

        /// <summary>
        /// Extends the selection.
        /// </summary>
        /// <param name="row">The row index to which to extend the selection.</param>
        /// <param name="column">The column index to which to extend the selection.</param>
        /// <param name="rowCount">The number of rows to which to extend the selection.</param>
        /// <param name="columnCount">The number of columns to which to extend the selection.</param>
        public override void ExtendSelection(int row, int column, int rowCount, int columnCount)
        {
            int num;
            int num2;
            int num3;
            int num4;
            if ((row == -1) && (column == -1))
            {
                num = -1;
                num3 = -1;
                num2 = -1;
                num4 = -1;
            }
            else if (row == -1)
            {
                num = -1;
                num3 = -1;
                num2 = Math.Min(this.anchorColumn, column);
                num4 = (Math.Max(this.anchorColumn, column) - num2) + 1;
            }
            else if (column == -1)
            {
                num = Math.Min(this.anchorRow, row);
                num3 = (Math.Max(this.anchorRow, row) - num) + 1;
                num2 = -1;
                num4 = -1;
            }
            else
            {
                num = row;
                num3 = rowCount;
                num2 = column;
                num4 = columnCount;
            }
            if (this.selectionPolicy != Dt.Cells.Data.SelectionPolicy.Single)
            {
                if (this.selectionPolicy == Dt.Cells.Data.SelectionPolicy.Range)
                {
                    while (this.items.Count > 1)
                    {
                        CellRange range = this.items[0];
                        base.FireChanged(range.Row, range.Column, range.RowCount, range.ColumnCount);
                        this.items.RemoveAt(0);
                    }
                }
                if (this.selectionUnit == Dt.Cells.Data.SelectionUnit.Row)
                {
                    num2 = -1;
                    num4 = -1;
                }
                else if (this.selectionUnit == Dt.Cells.Data.SelectionUnit.Column)
                {
                    num = -1;
                    num3 = -1;
                }
                if ((this.selectionUnit == Dt.Cells.Data.SelectionUnit.Row) && !this.IsSelected(this.anchorRow, this.anchorColumn))
                {
                    this.RemoveSelection(num, num2, num3, num4);
                }
                else if (this.items.Count <= 0)
                {
                    CellRange range6 = new CellRange(num, num2, num3, num4);
                    this.items.Add(range6);
                    base.FireChanged(range6.Row, range6.Column, range6.RowCount, range6.ColumnCount);
                }
                else
                {
                    CellRange range2 = null;
                    int num5 = this.items.Count - 1;
                    while (num5 >= 0)
                    {
                        if (this.items[num5].Contains(this.anchorRow, this.anchorColumn))
                        {
                            range2 = this.items[num5];
                            break;
                        }
                        num5--;
                    }
                    CellRange original = new CellRange(num, num2, num3, num4);
                    if (!original.Equals(range2))
                    {
                        if (range2 != null)
                        {
                            this.items[num5] = original;
                            List<CellRange> items = new List<CellRange>();
                            this.Split(items, original, range2);
                            foreach (CellRange range4 in items)
                            {
                                base.FireChanged(range4.Row, range4.Column, range4.RowCount, range4.ColumnCount);
                            }
                            items.Clear();
                            this.Split(items, range2, original);
                            foreach (CellRange range5 in items)
                            {
                                base.FireChanged(range5.Row, range5.Column, range5.RowCount, range5.ColumnCount);
                            }
                        }
                        else
                        {
                            this.items.Add(original);
                            base.FireChanged(original.Row, original.Column, original.RowCount, original.ColumnCount);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets an enumerator that can iterate through the selections.
        /// </summary>
        /// <returns>
        /// Returns an enumerator to enumerate through all the selections.
        /// </returns>
        public override IEnumerator GetEnumerator()
        {
            return (IEnumerator) this.items.GetEnumerator();
        }

        /// <summary>
        /// Returns an ordered array of CellRange objects, from largest to smallest, that contain the selected cells
        /// with minimal overlap between the ranges.
        /// </summary>
        /// <param name="rowCount">The number of rows in the sheet in which to look.</param>
        /// <param name="columnCount">The number of columns in the sheet in which to look.</param>
        /// <returns>Returns an ordered array of CellRange objects, from largest to smallest, that contain the selected cells with minimal overlap between the ranges.</returns>
        public CellRange[] GetSelections(int rowCount, int columnCount)
        {
            List<CellRange> items = new List<CellRange>((IEnumerable<CellRange>) this.items);
            List<CellRange> list2 = new List<CellRange>();
            while (items.Count > 0)
            {
                CellRange range;
                long num = 0L;
                int num2 = -1;
                for (int i = 0; i < items.Count; i++)
                {
                    long num3;
                    range = items[i];
                    if (range.ColumnCount == -1)
                    {
                        if (range.Column == -1)
                        {
                            num3 = columnCount;
                        }
                        else
                        {
                            num3 = columnCount - range.Column;
                        }
                    }
                    else
                    {
                        num3 = range.ColumnCount;
                    }
                    if (range.RowCount == -1)
                    {
                        if (range.Row == -1)
                        {
                            num3 *= rowCount;
                        }
                        else
                        {
                            num3 *= rowCount - range.Row;
                        }
                    }
                    else
                    {
                        num3 *= range.RowCount;
                    }
                    if (num3 > num)
                    {
                        num2 = i;
                        num = num3;
                    }
                }
                if (num2 < 0)
                {
                    num2 = items.Count - 1;
                }
                range = items[num2];
                CellRange original = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount);
                list2.Add(original);
                items.RemoveAt(num2);
                int num5 = 0;
                int num6 = items.Count;
                while (num5 < num6)
                {
                    range = items[num5];
                    if (original.Intersects(range.Row, range.Column, range.RowCount, range.ColumnCount))
                    {
                        CellRange split = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount);
                        items.RemoveAt(num5);
                        this.Split(items, original, split);
                        num6--;
                    }
                    else
                    {
                        num5++;
                    }
                }
            }
            CellRange[] rangeArray = new CellRange[list2.Count];
            if (list2.Count > 0)
            {
                list2.CopyTo(rangeArray);
            }
            return rangeArray;
        }

        void IRangeSupport.AddColumns(int column, int count)
        {
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Column >= column)
                {
                    this.items[i] = new CellRange(range.Row, range.Column + count, range.RowCount, range.ColumnCount);
                }
                else if ((range.Column < column) && (column < (range.Column + range.ColumnCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount + count);
                }
            }
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Row >= row)
                {
                    this.items[i] = new CellRange(range.Row + count, range.Column, range.RowCount, range.ColumnCount);
                }
                else if ((range.Row < row) && (row < (range.Row + range.RowCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount + count, range.ColumnCount);
                }
            }
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                CellRange range = this.items[i];
                if (((row == -1) || ((row <= range.Row) && (range.Row < (row + rowCount)))) && ((column == -1) || ((column <= range.Column) && (range.Column < (column + columnCount)))))
                {
                    this.items.RemoveAt(i--);
                }
            }
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            List<CellRange> list = new List<CellRange>();
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Column >= column)
                {
                    if ((range.Column + range.ColumnCount) <= (column + count))
                    {
                        list.Add(range);
                    }
                    else if (range.Column < (column + count))
                    {
                        this.items[i] = new CellRange(range.Row, column, range.RowCount, (range.Column + range.ColumnCount) - (column + count));
                    }
                    else
                    {
                        this.items[i] = new CellRange(range.Row, range.Column - count, range.RowCount, range.ColumnCount);
                    }
                }
                else if ((range.Column < column) && (column < (range.Column + range.ColumnCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount - Math.Min((range.Column + range.ColumnCount) - column, count));
                }
            }
            foreach (CellRange range2 in list)
            {
                this.items.Remove(range2);
            }
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            List<CellRange> list = new List<CellRange>();
            int num = this.items.Count;
            for (int i = 0; i < num; i++)
            {
                CellRange range = this.items[i];
                if (range.Row >= row)
                {
                    if ((range.Row + range.RowCount) <= (row + count))
                    {
                        list.Add(range);
                    }
                    else if (range.Row < (row + count))
                    {
                        this.items[i] = new CellRange(row, range.Column, (range.Row + range.RowCount) - (row + count), range.ColumnCount);
                    }
                    else
                    {
                        this.items[i] = new CellRange(range.Row - count, range.Column, range.RowCount, range.ColumnCount);
                    }
                }
                else if ((range.Row < row) && (row < (range.Row + range.RowCount)))
                {
                    this.items[i] = new CellRange(range.Row, range.Column, range.RowCount - Math.Min((range.Row + range.RowCount) - row, count), range.ColumnCount);
                }
            }
            foreach (CellRange range2 in list)
            {
                this.items.Remove(range2);
            }
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
        }

        /// <summary>
        /// Determines whether any cell in a specified column is in the selection.
        /// </summary>
        /// <param name="column">The column index to check.</param>
        /// <returns>
        /// <c>true</c> if any cell in the specified column is in the selection; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsAnyCellInColumnSelected(int column)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].IntersectColumn(column))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether any cell in the specified row is in the selection.
        /// </summary>
        /// <param name="row">The row index to check.</param>
        /// <returns>
        /// <c>true</c> if any cell in the specified row is in the selection; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsAnyCellInRowSelected(int row)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].IntersectRow(row))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the model has no selections.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the model has no selections; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsEmpty()
        {
            return (this.items.Count == 0);
        }

        /// <summary>
        /// Determines whether the specified cell is in the selection.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>  
        /// <returns>
        /// <c>true</c> if the specified cell is in the selection; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsSelected(int row, int column)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].Contains(row, column))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the specified selected range from the selection list, if one exists.
        /// </summary>
        /// <param name="row">The row index of the cell at which to start.</param>
        /// <param name="column">The column index of the cell at which to start.</param>
        /// <param name="rowCount">The number of rows to deselect.</param>
        /// <param name="columnCount">The number of columns to deselect.</param>
        public override void RemoveSelection(int row, int column, int rowCount, int columnCount)
        {
            if (this.selectionUnit == Dt.Cells.Data.SelectionUnit.Row)
            {
                for (int i = 0; i < this.items.Count; i++)
                {
                    CellRange range = this.items[i];
                    if (row <= range.Row)
                    {
                        if ((range.Row + range.RowCount) <= (row + rowCount))
                        {
                            this.items.RemoveAt(i--);
                            base.FireChanged(range.Row, range.Column, range.RowCount, range.ColumnCount);
                        }
                        else if (range.Row < (row + rowCount))
                        {
                            this.items[i] = new CellRange(row + rowCount, range.Column, (range.Row + range.RowCount) - (row + rowCount), range.ColumnCount);
                            base.FireChanged(range.Row, range.Column, (row + rowCount) - range.Row, range.ColumnCount);
                        }
                    }
                    else if (row < (range.Row + range.RowCount))
                    {
                        if ((range.Row + range.RowCount) <= (row + rowCount))
                        {
                            this.items[i] = new CellRange(range.Row, range.Column, row - range.Row, range.ColumnCount);
                            base.FireChanged(row, range.Column, (range.Row + range.RowCount) - row, range.ColumnCount);
                        }
                        else
                        {
                            this.items[i++] = new CellRange(range.Row, range.Column, row - range.Row, range.ColumnCount);
                            this.items.Insert(i, new CellRange(row + rowCount, range.Column, (range.Row + range.RowCount) - (row + rowCount), range.ColumnCount));
                            base.FireChanged(row, range.Column, rowCount, range.ColumnCount);
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < this.items.Count; j++)
                {
                    if (this.items[j].Equals(row, column, rowCount, columnCount))
                    {
                        this.items.RemoveAt(j--);
                        base.FireChanged(row, column, rowCount, columnCount);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies the first cell selected in the range.
        /// </summary>
        /// <param name="row">The row index of the first selected cell.</param>
        /// <param name="column">The column index of the first selected cell.</param>
        public override void SetAnchor(int row, int column)
        {
            this.anchorRow = row;
            this.anchorColumn = column;
        }

        /// <summary>
        /// Selects the specified cells.
        /// </summary>
        /// <param name="row">The row index of the first cell.</param>
        /// <param name="column">The column index of the first cell.</param>
        /// <param name="rowCount">The number of rows to select after the first cell.</param>
        /// <param name="columnCount">The number of columns to select after the first cell.</param>
        public override void SetSelection(int row, int column, int rowCount, int columnCount)
        {
            if ((this.items.Count != 1) || !this.items[0].Equals(row, column, rowCount, columnCount))
            {
                this.ClearSelection();
                this.AddSelection(row, column, rowCount, columnCount);
            }
        }

        /// <summary>
        /// Splits the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="original">The original</param>
        /// <param name="split">The split</param>
        void Split(List<CellRange> items, CellRange original, CellRange split)
        {
            CellRange range;
            int row = split.Row;
            int column = split.Column;
            int num3 = (split.Row == -1) ? -1 : ((split.Row + split.RowCount) - 1);
            int num4 = (split.Column == -1) ? -1 : ((split.Column + split.ColumnCount) - 1);
            if (row < original.Row)
            {
                range = new CellRange(row, column, (Math.Min(num3, original.Row - 1) - row) + 1, (num4 - column) + 1);
                items.Add(range);
                row = original.Row;
            }
            if (column < original.Column)
            {
                range = new CellRange(row, column, (num3 - row) + 1, (Math.Min(num4, original.Column - 1) - column) + 1);
                items.Add(range);
                column = original.Column;
            }
            if ((original.Column != -1) && (num4 > ((original.Column + original.ColumnCount) - 1)))
            {
                range = new CellRange(row, Math.Max(column, original.Column + original.ColumnCount), (num3 - row) + 1, (num4 - Math.Max(column, original.Column + original.ColumnCount)) + 1);
                items.Add(range);
                num4 = (original.Column + original.ColumnCount) - 1;
            }
            if ((original.Row != -1) && (num3 > ((original.Row + original.RowCount) - 1)))
            {
                range = new CellRange(Math.Max(row, original.Row + original.RowCount), column, (num3 - Math.Max(row, original.Row + original.RowCount)) + 1, (num4 - column) + 1);
                items.Add(range);
                num4 = (original.Column + original.ColumnCount) - 1;
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
            this.items = new List<CellRange>();
            this.anchorRow = 0;
            this.anchorColumn = 0;
            this.selectionPolicy = Dt.Cells.Data.SelectionPolicy.MultiRange;
            this.selectionUnit = Dt.Cells.Data.SelectionUnit.Cell;
            bool flag = false;
            while (reader.Read())
            {
                if (reader.NodeType != ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    goto Label_01B1;
                }
                string name = reader.Name;
                if (name != null)
                {
                    if (name != "SelectionPolicy")
                    {
                        if (name == "SelectionUnit")
                        {
                            goto Label_00BE;
                        }
                        if (name == "AnchorCellRow")
                        {
                            goto Label_00D5;
                        }
                        if (name == "AnchorColumnRow")
                        {
                            goto Label_00EC;
                        }
                        if (name == "Items")
                        {
                            goto Label_0103;
                        }
                        if (name == "Item")
                        {
                            goto Label_010A;
                        }
                    }
                    else
                    {
                        this.selectionPolicy = Serializer.ReadAttributeEnum<Dt.Cells.Data.SelectionPolicy>("value", Dt.Cells.Data.SelectionPolicy.MultiRange, reader);
                    }
                }
                continue;
            Label_00BE:
                this.selectionUnit = Serializer.ReadAttributeEnum<Dt.Cells.Data.SelectionUnit>("value", Dt.Cells.Data.SelectionUnit.Cell, reader);
                continue;
            Label_00D5:
                this.anchorRow = Serializer.ReadAttributeInt("value", 0, reader);
                continue;
            Label_00EC:
                this.anchorColumn = Serializer.ReadAttributeInt("value", 0, reader);
                continue;
            Label_0103:
                flag = true;
                continue;
            Label_010A:
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
            Label_01B1:
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
            if (this.SelectionPolicy != Dt.Cells.Data.SelectionPolicy.MultiRange)
            {
                Serializer.SerializeObj(this.selectionPolicy, "SelectionPolicy", writer);
            }
            if (this.SelectionUnit != Dt.Cells.Data.SelectionUnit.Cell)
            {
                Serializer.SerializeObj(this.selectionUnit, "SelectionUnit", writer);
            }
            if ((this.items != null) && (this.items.Count > 0))
            {
                Serializer.WriteStartObj("Items", writer);
                foreach (CellRange range in this.items)
                {
                    Serializer.WriteStartObj("Item", writer);
                    Serializer.WriteAttr("r", (int) range.Row, writer);
                    Serializer.WriteAttr("c", (int) range.Column, writer);
                    Serializer.WriteAttr("rc", (int) range.RowCount, writer);
                    Serializer.WriteAttr("cc", (int) range.ColumnCount, writer);
                    Serializer.WriteEndObj(writer);
                }
                Serializer.WriteEndObj(writer);
            }
            if (this.AnchorRow > 0)
            {
                Serializer.SerializeObj((int) this.anchorRow, "AnchorCellRow", writer);
            }
            if (this.AnchorColumn > 0)
            {
                Serializer.SerializeObj((int) this.anchorColumn, "AnchorColumnRow", writer);
            }
        }

        /// <summary>
        /// Gets the column index of the first cell selected in the range.
        /// </summary>
        /// <value>The column index of the first cell selected in the range. The default value is 0.</value>
        [DefaultValue(0)]
        public override int AnchorColumn
        {
            get { return  this.anchorColumn; }
        }

        /// <summary>
        /// Gets the row index of the first cell selected in the range.
        /// </summary>
        /// <value>The row index of the first cell selected in the range. The default value is 0.</value>
        [DefaultValue(0)]
        public override int AnchorRow
        {
            get { return  this.anchorRow; }
        }

        /// <summary>
        /// Gets the number of selections.
        /// </summary>
        /// <value>The number of selections.</value>
        public override int Count
        {
            get { return  this.items.Count; }
        }

        bool IDataEmptySupport.IsDataEmpty
        {
            get
            {
                if (this.anchorRow != 0)
                {
                    return false;
                }
                if (this.anchorColumn != 0)
                {
                    return false;
                }
                if (this.selectionPolicy != Dt.Cells.Data.SelectionPolicy.MultiRange)
                {
                    return false;
                }
                if (this.selectionUnit != Dt.Cells.Data.SelectionUnit.Cell)
                {
                    return false;
                }
                if (!this.IsEmpty())
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a selection in the model.
        /// </summary>
        /// <param name="index">The index of the selection.</param>
        /// <value>A cell range to represent a selection in the model.</value>
        public override CellRange this[int index]
        {
            get
            {
                if ((index >= 0) && (index < this.Count))
                {
                    return this.items[index];
                }
                return null;
            }
        }

        public override ReadOnlyCollection<CellRange> Items
        {
            get { return  new ReadOnlyCollection<CellRange>((IList<CellRange>) this.items); }
        }

        /// <summary>
        /// Gets or sets whether and how users can select ranges of items.
        /// </summary>
        /// <value>
        /// The <see cref="P:Dt.Cells.Data.SheetSelectionModel.SelectionPolicy" /> enumeration that indicates how to add selections.
        /// </value>
        public override Dt.Cells.Data.SelectionPolicy SelectionPolicy
        {
            get { return  this.selectionPolicy; }
            set { this.selectionPolicy = value; }
        }

        /// <summary>
        /// Gets or sets whether users can select cells, rows, or columns.
        /// </summary>
        /// <value>
        /// The <see cref="P:Dt.Cells.Data.SheetSelectionModel.SelectionUnit" /> enumeration that specifies the selection type.
        /// </value>
        public override Dt.Cells.Data.SelectionUnit SelectionUnit
        {
            get { return  this.selectionUnit; }
            set { this.selectionUnit = value; }
        }
    }
}

