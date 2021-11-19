#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a table which can be added in a sheet.
    /// </summary>
    public sealed class SheetTable : IXmlSerializable, INotifyPropertyChanged, IRangeSupport
    {
        bool bandedColumns;
        bool bandedRows;
        int column;
        static string COLUMN_NAME_PREFIX = "Column";
        int columnCount;
        SparseArray<TableColumn> columns;
        bool copying;
        ConnectionBase dataConnection;
        int footerRowIndex;
        int headerRowIndex;
        bool highlightFirstColumn;
        bool highlightLastColumn;
        string name;
        internal EricTables owner;
        int row;
        int rowCount;
        TableFilter rowfilter;
        bool showFooter;
        bool showHeader;
        TableStyle style;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SheetTable" /> class.
        /// </summary>
        public SheetTable()
        {
        }

        internal SheetTable(string name, int row, int column, int rowCount, int columnCount, ConnectionBase dataConnection, TableStyle style)
        {
            this.Init();
            this.name = name;
            this.headerRowIndex = this.row = row;
            this.column = column;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.dataConnection = dataConnection;
            this.style = style;
            int length = dataConnection.DataFields.Length;
            this.columns = new SparseArray<TableColumn>(length);
            for (int i = 0; i < length; i++)
            {
                this.columns[i] = new TableColumn();
                this.columns[i].SetName(dataConnection.DataFields[i]);
            }
        }

        short CalcAutoIdx()
        {
            List<short> list = new List<short>();
            for (int i = 0; i < this.columns.Length; i++)
            {
                TableColumn column = this.columns[i];
                if ((column != null) && column.IsAutoName)
                {
                    short? nullable = ParseIdx(column.Name);
                    if (nullable.HasValue)
                    {
                        list.Add(nullable.Value);
                    }
                }
            }
            list.Sort();
            short num2 = 1;
            short num3 = 0;
            while (num3 < list.Count)
            {
                if ((num3 + 1) != list[num3])
                {
                    return num2;
                }
                num3++;
                num2++;
            }
            return num2;
        }

        CellRange CalcDataRange()
        {
            int row = this.showHeader ? (this.row + 1) : this.row;
            int rowCount = this.rowCount;
            if (this.showHeader)
            {
                rowCount--;
            }
            if (this.showFooter)
            {
                rowCount--;
            }
            return new CellRange(row, this.column, rowCount, this.columnCount);
        }

        int CalcFooterIndex()
        {
            if (!this.showFooter)
            {
                return -1;
            }
            return ((this.row + this.rowCount) - 1);
        }

        int CalcHeaderIndex()
        {
            if (!this.showHeader)
            {
                return -1;
            }
            return this.row;
        }

        void CheckSheet()
        {
            if (this.Sheet == null)
            {
                throw new NotSupportedException(ResourceStrings.TableOwnerNullError);
            }
        }

        internal void Clear()
        {
            for (int i = 0; i < this.rowCount; i++)
            {
                int row = i + this.row;
                for (int j = 0; j < this.columnCount; j++)
                {
                    int column = j + this.column;
                    this.owner.innerSheet.SetCellText(row, column, null);
                    this.owner.innerSheet.SetCellFormula(row, column, null);
                }
            }
        }

        void ClearTableRow(int sheetRowIndex)
        {
            if ((sheetRowIndex >= this.row) && (sheetRowIndex <= (this.row + this.rowCount)))
            {
                for (int i = 0; i < this.columnCount; i++)
                {
                    int column = this.column + i;
                    this.owner.innerSheet.SetCellFormula(sheetRowIndex, column, null);
                    this.owner.innerSheet.SetCellText(sheetRowIndex, column, null);
                }
            }
        }

        internal TableColumn GetColumn(int column)
        {
            if (((this.columns != null) && (column >= 0)) && (column < this.columns.Length))
            {
                return this.columns[column];
            }
            return null;
        }

        /// <summary>
        /// Gets the table footer formula with a specified index.
        /// </summary>
        /// <param name="tableColumnIndex">The column index of the table footer. The index is zero-based.</param>
        /// <returns>The text of the specified header cell. </returns>
        /// <remarks>
        /// This method returns null if there is no footer setting and the cell value of the sheet is displayed.
        /// </remarks>
        public object GetColumnFormula(int tableColumnIndex)
        {
            if ((tableColumnIndex < 0) || (tableColumnIndex >= this.columns.Length))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(this.columns[tableColumnIndex].Formula))
            {
                return this.columns[tableColumnIndex].Value;
            }
            return this.columns[tableColumnIndex].Formula;
        }

        /// <summary>
        /// Gets the table header text with a specified index.
        /// </summary>
        /// <param name="tableColumnIndex">The column index of the table header. The index is zero-based.</param>
        /// <returns>The text of the specified header cell. </returns>
        /// <remarks>
        /// This method returns null when there is no header setting and the cell value of the sheet is displayed.
        /// </remarks>
        public string GetColumnName(int tableColumnIndex)
        {
            if (((this.columns != null) && (tableColumnIndex >= 0)) && (tableColumnIndex < this.columns.Length))
            {
                var col = columns[tableColumnIndex];
                if (col != null)
                    return col.Name;
            }
            return string.Empty;
        }

        void IRangeSupport.AddColumns(int column, int count)
        {
            if (this.Sheet == null)
            {
                throw new NotSupportedException(ResourceStrings.TableOwnerNullError);
            }
            if ((column < 0) || (column > (this.Sheet.ColumnCount - 1)))
            {
                throw new ArgumentOutOfRangeException(string.Format(ResourceStrings.TableColumnDestinationOutOfRange, (object[])new object[] { ((int)column) }));
            }
            if (column < (this.column + this.columnCount))
            {
                int num = this.column;
                if (column <= this.column)
                {
                    this.column += count;
                }
                else if (column < (num + this.columnCount))
                {
                    this.columnCount += count;
                    if (this.columns != null)
                    {
                        this.columns.InsertRange(column - num, count);
                    }
                }
                this.UpdateHeader(0);
                if (this.rowfilter != null)
                {
                    ((IRangeSupport)this.rowfilter).AddColumns(column, count);
                }
            }
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            if (this.Sheet == null)
            {
                throw new NotSupportedException(ResourceStrings.TableOwnerNullError);
            }
            if ((row < 0) || (row >= this.Sheet.RowCount))
            {
                throw new ArgumentOutOfRangeException(string.Format(ResourceStrings.TableRowDestinationOutOfRangeError, (object[])new object[] { ((int)row) }));
            }
            if (row < (this.row + this.rowCount))
            {
                CellRange range = this.CalcDataRange();
                int num = this.showHeader ? this.CalcHeaderIndex() : range.Row;
                int num2 = this.ShowFooter ? this.CalcFooterIndex() : ((range.Row + range.RowCount) - 1);
                if (row <= num)
                {
                    this.row += count;
                    this.headerRowIndex = this.CalcHeaderIndex();
                }
                else if (row <= num2)
                {
                    this.rowCount += count;
                }
                this.footerRowIndex = this.CalcFooterIndex();
                if (this.rowfilter != null)
                {
                    ((IRangeSupport)this.rowfilter).AddRows(row, count);
                }
            }
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            ITableSheet sheet = this.Sheet;
            if (sheet != null)
            {
                int num = (row < 0) ? 0 : row;
                int num2 = (column < 0) ? 0 : column;
                int num3 = (row < 0) ? sheet.RowCount : rowCount;
                int num4 = (column < 0) ? sheet.ColumnCount : columnCount;
                CellRange range = this.Range;
                if ((this.ShowHeader && (num <= this.headerRowIndex)) && ((num + num3) >= this.headerRowIndex))
                {
                    CellRange range2 = CellRange.GetIntersect(new CellRange(num, num2, num3, num4), new CellRange(this.headerRowIndex, range.Column, 1, range.ColumnCount), sheet.RowCount, sheet.ColumnCount);
                    if (range2 != null)
                    {
                        int num5 = range2.Column - range.Column;
                        for (int i = 0; i < range2.ColumnCount; i++)
                        {
                            int num7 = i + num5;
                            if (((this.columns != null) && (num7 >= 0)) && (num7 < this.columns.Length))
                            {
                                TableColumn column2 = this.columns[num7];
                                if ((column2 != null) && !column2.IsAutoName)
                                {
                                    column2.SetName(this.CalcAutoIdx());
                                }
                            }
                        }
                    }
                }
                if ((this.ShowFooter && (num <= this.footerRowIndex)) && ((num + num3) >= this.footerRowIndex))
                {
                    CellRange range3 = CellRange.GetIntersect(new CellRange(num, num2, num3, num4), new CellRange(this.footerRowIndex, (range.Column + range.ColumnCount) - 1, 1, range.ColumnCount), sheet.RowCount, sheet.ColumnCount);
                    if (range3 != null)
                    {
                        int num8 = range3.Column - range.Column;
                        for (int j = 0; j < range3.ColumnCount; j++)
                        {
                            int num10 = j + num8;
                            if (((this.columns != null) && (num10 >= 0)) && (num10 < this.columns.Length))
                            {
                                this.columns[num10].SetFormula(null, null);
                            }
                        }
                    }
                }
            }
            if (this.rowfilter != null)
            {
                ((IRangeSupport)this.rowfilter).Clear(row, column, rowCount, columnCount);
            }
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            ITableSheet sheet = this.Sheet;
            if (sheet != null)
            {
                int row = (fromRow < 0) ? 0 : fromRow;
                int column = (fromColumn < 0) ? 0 : fromColumn;
                int num3 = (fromRow < 0) ? sheet.RowCount : rowCount;
                int num4 = (fromColumn < 0) ? sheet.ColumnCount : columnCount;
                int num5 = (toRow < 0) ? 0 : toRow;
                int num6 = (toColumn < 0) ? 0 : toColumn;
                if ((this.ShowHeader && (row <= this.headerRowIndex)) && ((row + num3) >= this.headerRowIndex))
                {
                    CellRange range = this.Range;
                    CellRange range2 = CellRange.GetIntersect(new CellRange(row, column, num3, num4), new CellRange(this.headerRowIndex, range.Column, 1, range.ColumnCount), sheet.RowCount, sheet.ColumnCount);
                    if (range2 != null)
                    {
                        int num7 = range2.ColumnCount;
                        int num8 = range2.Row - range.Row;
                        int num9 = range2.Column - range.Column;
                        string[] strArray = new string[num7];
                        for (int i = 0; i < num7; i++)
                        {
                            strArray[i] = this.GetColumnName(i + num9);
                        }
                        num8 = range2.Row - row;
                        num9 = range2.Column - column;
                        for (int j = 0; j < num7; j++)
                        {
                            sheet.SetValue(num5 + num8, (num6 + num9) + j, strArray[j]);
                        }
                    }
                }
            }
            if (this.rowfilter != null)
            {
                ((IRangeSupport)this.rowfilter).Copy(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            ITableSheet sheet = this.Sheet;
            if (sheet != null)
            {
                int row = (fromRow < 0) ? 0 : fromRow;
                int column = (fromColumn < 0) ? 0 : fromColumn;
                int num3 = (fromRow < 0) ? sheet.RowCount : rowCount;
                int num4 = (fromColumn < 0) ? sheet.ColumnCount : columnCount;
                int num5 = (toRow < 0) ? 0 : toRow;
                int num6 = (toColumn < 0) ? 0 : toColumn;
                if ((this.ShowHeader && (row <= this.headerRowIndex)) && ((row + num3) >= this.headerRowIndex))
                {
                    CellRange range = this.Range;
                    CellRange range2 = CellRange.GetIntersect(new CellRange(row, column, num3, num4), new CellRange(this.headerRowIndex, range.Column, 1, range.ColumnCount), sheet.RowCount, sheet.ColumnCount);
                    if (range2 != null)
                    {
                        int num7 = range2.ColumnCount;
                        int num8 = range2.Row - range.Row;
                        int num9 = range2.Column - range.Column;
                        string[] strArray = new string[num7];
                        for (int i = 0; i < num7; i++)
                        {
                            strArray[i] = this.GetColumnName(i + num9);
                        }
                        ((IRangeSupport)this).Clear(row, column, num3, num4);
                        num8 = range2.Row - row;
                        num9 = range2.Column - column;
                        for (int j = 0; j < num7; j++)
                        {
                            sheet.SetValue(num5 + num8, (num6 + num9) + j, strArray[j]);
                        }
                    }
                }
            }
            if (this.rowfilter != null)
            {
                ((IRangeSupport)this.rowfilter).Move(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            if (this.Sheet == null)
            {
                throw new NotSupportedException(ResourceStrings.TableOwnerNullError);
            }
            if (column < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format(ResourceStrings.TableColumnDestinationOutOfRange, (object[])new object[] { ((int)column) }));
            }
            if (column < (this.column + this.columnCount))
            {
                int num = this.column;
                if (column < this.column)
                {
                    if ((column + count) <= this.column)
                    {
                        this.column -= count;
                    }
                    else
                    {
                        this.columnCount -= (column + count) - this.column;
                        this.column = column;
                    }
                }
                else
                {
                    int num2 = (count > ((this.column + this.columnCount) - column)) ? ((this.column + this.columnCount) - column) : count;
                    this.columnCount -= num2;
                }
                if (this.columns != null)
                {
                    this.columns.RemoveRange(column - num, count);
                }
                this.UpdateHeader(0);
                if (this.rowfilter != null)
                {
                    ((IRangeSupport)this.rowfilter).RemoveColumns(column, count);
                }
            }
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            if (this.Sheet == null)
            {
                throw new NotSupportedException(ResourceStrings.TableOwnerNullError);
            }
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format(ResourceStrings.TableRowDestinationOutOfRangeError, (object[])new object[] { ((int)row) }));
            }
            if (row < (this.row + this.rowCount))
            {
                CellRange range = this.CalcDataRange();
                int num = this.showHeader ? this.CalcHeaderIndex() : range.Row;
                int num2 = this.ShowFooter ? this.CalcFooterIndex() : ((range.Row + range.RowCount) - 1);
                if (row < num)
                {
                    this.row -= count;
                    this.headerRowIndex = this.CalcHeaderIndex();
                    this.footerRowIndex = this.CalcFooterIndex();
                }
                else if ((row == num2) && this.showFooter)
                {
                    this.ShowFooter = false;
                }
                else if (((row <= num2) && (row > num)) || ((row == num) && !this.showHeader))
                {
                    int num3 = (count > ((num2 - row) + 1)) ? ((num2 - row) + 1) : count;
                    this.rowCount -= num3;
                    this.footerRowIndex = this.CalcFooterIndex();
                }
                if (this.rowfilter != null)
                {
                    ((IRangeSupport)this.rowfilter).RemoveRows(row, count);
                }
            }
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (this.rowfilter != null)
            {
                ((IRangeSupport)this.rowfilter).Swap(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
        }

        void Init()
        {
            this.owner = null;
            this.name = null;
            this.row = -1;
            this.column = -1;
            this.rowCount = -1;
            this.columnCount = -1;
            if (this.dataConnection != null)
            {
                this.dataConnection.Close();
                this.dataConnection = null;
            }
            this.style = null;
            this.columns = null;
            this.rowfilter = null;
            this.showHeader = true;
            this.showFooter = false;
            this.highlightFirstColumn = false;
            this.highlightLastColumn = false;
            this.bandedRows = true;
            this.bandedColumns = false;
            this.headerRowIndex = this.row;
            this.footerRowIndex = -1;
        }

        internal void LoadData()
        {
            int col;
            if (this.dataConnection != null)
            {
                this.CheckSheet();
                col = this.column;
                int num = 0;
                while (num < this.DataRange.ColumnCount)
                {
                    int num2 = this.showHeader ? 1 : 0;
                    string field = this.columns[num].Name;
                    int recordIndex = 0;
                    while (recordIndex < this.DataRange.RowCount)
                    {
                        if (field != null)
                        {
                            object obj2 = this.dataConnection.GetValue(recordIndex, field);
                            this.Sheet.SetValue(num2 + this.row, col, obj2);
                        }
                        recordIndex++;
                        num2++;
                    }
                    num++;
                    col++;
                }
            }
        }

        internal void MoveTo(int row, int column)
        {
            this.CheckSheet();
            if ((row < 0) || (row > (this.Sheet.RowCount - this.rowCount)))
            {
                throw new ArgumentOutOfRangeException("row", ResourceStrings.TableMoveDestinationOutOfRange);
            }
            if ((column < 0) || (column > (this.Sheet.ColumnCount - this.columnCount)))
            {
                throw new ArgumentOutOfRangeException("column", ResourceStrings.TableMoveDestinationOutOfRange);
            }
            CellRange range1 = this.Range;
            CellRange range = this.CalcDataRange();
            this.CalcHeaderIndex();
            this.CalcFooterIndex();
            this.UpdateHeader(1);
            this.UpdateFooter(1);
            this.ClearTableRow(this.headerRowIndex);
            this.ClearTableRow(this.footerRowIndex);
            int toRow = this.showHeader ? (row + 1) : row;
            if ((this.owner != null) && (this.owner.InnerSheet != null))
            {
                this.owner.InnerSheet.Move(range.Row, range.Column, toRow, column, range.RowCount, range.ColumnCount);
            }
            this.row = row;
            this.column = column;
            this.headerRowIndex = this.CalcHeaderIndex();
            this.footerRowIndex = this.CalcFooterIndex();
            this.UpdateHeader(2);
            this.UpdateFooter(2);
            this.UpdateFilter();
        }

        static short? ParseIdx(string token)
        {
            short num;
            if ((!string.IsNullOrEmpty(token) && token.StartsWith(COLUMN_NAME_PREFIX)) && short.TryParse(token.Substring(COLUMN_NAME_PREFIX.Length), out num))
            {
                return new short?(num);
            }
            return null;
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Reloads the data from a data source.
        /// </summary>
        public void Refresh()
        {
            if (this.dataConnection != null)
            {
                this.dataConnection.ClearCachedData();
                if (this.dataConnection is CsvFileConnection)
                {
                    CsvFileConnection dataConnection = this.dataConnection as CsvFileConnection;
                    if (dataConnection.IsOpen)
                    {
                        dataConnection.ReOpen();
                    }
                }
                this.LoadData();
            }
        }

        internal void Render(int row, int column, StyleInfo dest)
        {
            bool firstRow = this.showHeader ? (row == (this.row + 1)) : (row == this.row);
            bool lastRow = this.showFooter ? (row == ((this.row + this.rowCount) - 2)) : (row == ((this.row + this.rowCount) - 1));
            bool firstColumn = column == this.column;
            bool lastColumn = column == ((this.column + this.columnCount) - 1);
            bool flag5 = this.showHeader ? (row == this.headerRowIndex) : (row == this.row);
            bool flag6 = this.showFooter ? (row == this.footerRowIndex) : (row == ((this.row + this.rowCount) - 1));
            if ((this.Sheet.GetActualRowHeight(row) > 0.0) && (this.style != null))
            {
                if (this.style.WholeTableStyle != null)
                {
                    this.style.WholeTableStyle.Render(dest, flag5, firstColumn, flag6, lastColumn);
                }
                if (this.headerRowIndex == row)
                {
                    if ((firstColumn && this.highlightFirstColumn) && (this.style.HighlightFirstColumnStyle != null))
                    {
                        this.style.HighlightFirstColumnStyle.Render(dest, true, true, false, true);
                    }
                    if ((lastColumn && this.highlightLastColumn) && (this.style.HighlightLastColumnStyle != null))
                    {
                        this.style.HighlightLastColumnStyle.Render(dest, true, true, false, true);
                    }
                    if (this.style.HeaderRowStyle != null)
                    {
                        this.style.HeaderRowStyle.Render(dest, true, firstColumn, true, lastColumn);
                    }
                    if ((firstColumn && this.highlightFirstColumn) && (this.style.FirstHeaderCellStyle != null))
                    {
                        this.style.FirstHeaderCellStyle.Render(dest, true, true, true, true);
                    }
                    if ((lastColumn && this.highlightLastColumn) && (this.style.LastHeaderCellStyle != null))
                    {
                        this.style.LastHeaderCellStyle.Render(dest, true, true, true, true);
                    }
                }
                else if (this.footerRowIndex == row)
                {
                    if ((firstColumn && this.highlightFirstColumn) && (this.style.HighlightFirstColumnStyle != null))
                    {
                        this.style.HighlightFirstColumnStyle.Render(dest, false, true, true, true);
                    }
                    if ((lastColumn && this.highlightLastColumn) && (this.style.HighlightLastColumnStyle != null))
                    {
                        this.style.HighlightLastColumnStyle.Render(dest, false, true, true, true);
                    }
                    if (this.style.FooterRowStyle != null)
                    {
                        this.style.FooterRowStyle.Render(dest, true, firstColumn, true, lastColumn);
                    }
                    if ((firstColumn && this.highlightFirstColumn) && (this.style.FirstFooterCellStyle != null))
                    {
                        this.style.FirstFooterCellStyle.Render(dest, true, true, true, true);
                    }
                    if ((lastColumn && this.highlightLastColumn) && (this.style.LastFooterCellStyle != null))
                    {
                        this.style.LastFooterCellStyle.Render(dest, true, true, true, true);
                    }
                }
                else
                {
                    int num = 0;
                    for (int i = this.DataRange.Row; i < row; i++)
                    {
                        if (this.Sheet.GetActualRowHeight(i) > 0.0)
                        {
                            num++;
                        }
                    }
                    int num4 = 0;
                    for (int j = this.column; j < column; j++)
                    {
                        if (this.Sheet.GetActualColumnWidth(j) > 0.0)
                        {
                            num4++;
                        }
                    }
                    int num6 = num;
                    int num7 = num4;
                    if (this.bandedColumns)
                    {
                        int num8 = this.style.FirstColumnStripSize + this.style.SecondColumnStripSize;
                        if (num8 > 0)
                        {
                            int num9 = num7 % num8;
                            if ((num9 < this.style.FirstColumnStripSize) && (this.style.FirstColumnStripStyle != null))
                            {
                                bool flag7 = num9 == 0;
                                bool flag8 = num9 == (this.style.FirstColumnStripSize - 1);
                                if (!flag8 && (column == ((this.column + this.columnCount) - 1)))
                                {
                                    flag8 = true;
                                }
                                this.style.FirstColumnStripStyle.Render(dest, firstRow, flag7, lastRow, flag8);
                            }
                            else if ((num9 >= this.style.FirstColumnStripSize) && (this.style.SecondColumnStripStyle != null))
                            {
                                bool flag9 = num9 == this.style.FirstColumnStripSize;
                                bool flag10 = num9 == (num8 - 1);
                                if (!flag10 && (column == ((this.column + this.columnCount) - 1)))
                                {
                                    flag10 = true;
                                }
                                this.style.SecondColumnStripStyle.Render(dest, firstRow, flag9, lastRow, flag10);
                            }
                        }
                    }
                    if (this.bandedRows)
                    {
                        int num10 = this.style.FirstRowStripSize + this.style.SecondRowStripSize;
                        if (num10 > 0)
                        {
                            int num11 = num6 % num10;
                            if ((num11 < this.style.FirstRowStripSize) && (this.style.FirstRowStripStyle != null))
                            {
                                bool flag11 = num11 == 0;
                                bool flag12 = num11 == (this.style.FirstRowStripSize - 1);
                                CellRange dataRange = this.DataRange;
                                if (!flag12 && (row == ((dataRange.Row + dataRange.RowCount) - 1)))
                                {
                                    flag12 = true;
                                }
                                this.style.FirstRowStripStyle.Render(dest, flag11, firstColumn, flag12, lastColumn);
                            }
                            else if ((num11 >= this.style.FirstRowStripSize) && (this.style.SecondRowStripStyle != null))
                            {
                                bool flag13 = num11 == this.style.FirstRowStripSize;
                                bool flag14 = num11 == (num10 - 1);
                                CellRange range2 = this.DataRange;
                                if (!flag14 && (row == ((range2.Row + range2.RowCount) - 1)))
                                {
                                    flag14 = true;
                                }
                                this.style.SecondRowStripStyle.Render(dest, flag13, firstColumn, flag14, lastColumn);
                            }
                        }
                    }
                    if ((firstColumn && this.highlightFirstColumn) && (this.style.HighlightFirstColumnStyle != null))
                    {
                        this.style.HighlightFirstColumnStyle.Render(dest, flag5, true, flag6, true);
                    }
                    if ((lastColumn && this.highlightLastColumn) && (this.style.HighlightLastColumnStyle != null))
                    {
                        this.style.HighlightLastColumnStyle.Render(dest, flag5, true, flag6, true);
                    }
                }
            }
        }

        internal void Resize(int rowCount, int columnCount)
        {
            if (this.Sheet == null)
            {
                throw new NotSupportedException(ResourceStrings.TableOwnerNullError);
            }
            int num = 0;
            if (this.showHeader)
            {
                num++;
            }
            if (this.showFooter)
            {
                num++;
            }
            if (rowCount < num)
            {
                throw new ArgumentOutOfRangeException("row", string.Format(ResourceStrings.TableResizeOutOfRangeError, (object[])new object[] { "row" }));
            }
            if (columnCount < 1)
            {
                throw new ArgumentOutOfRangeException("column", string.Format(ResourceStrings.TableResizeOutOfRangeError, (object[])new object[] { "column" }));
            }
            if (rowCount > this.owner.innerSheet.RowCount)
            {
                throw new ArgumentOutOfRangeException("rowCount");
            }
            if (columnCount > this.owner.innerSheet.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("columnCount");
            }
            if (columnCount < this.columnCount)
            {
                this.UpdateHeader(1);
                this.ClearTableRow(this.headerRowIndex);
            }
            if (rowCount != this.rowCount)
            {
                this.UpdateFooter(1);
                this.ClearTableRow(this.footerRowIndex);
            }
            if (columnCount > this.columnCount)
            {
                this.columns.Length = columnCount;
                this.columnCount = columnCount;
                this.UpdateHeader(1);
                this.UpdateHeader(2);
            }
            else if (columnCount < this.columnCount)
            {
                this.columns.Length = columnCount;
                this.columnCount = columnCount;
                this.UpdateHeader(2);
            }
            if (rowCount > this.rowCount)
            {
                this.rowCount = rowCount;
                this.footerRowIndex = this.CalcFooterIndex();
                this.UpdateFooter(2);
            }
            else if (rowCount < this.rowCount)
            {
                this.rowCount = rowCount;
                this.footerRowIndex = this.CalcFooterIndex();
                this.UpdateFooter(2);
            }
            this.UpdateFilter();
        }

        void rowfilter_Changed(object sender, FilterEventArgs e)
        {
            this.RaisePropertyChanged("TableFilter");
        }

        /// <summary>
        /// Sets the table footer formula with a specified index.
        /// </summary>
        /// <param name="tableColumnIndex">The column index of the table footer. The index is zero-based.</param>
        /// <param name="formula">The footer text. A null value indicates that the cell value of the sheet is displayed.</param>
        public void SetColumnFormula(int tableColumnIndex, string formula)
        {
            if (((this.columns != null) && (tableColumnIndex >= 0)) && (tableColumnIndex < this.columns.Length))
            {
                TableColumn column = this.columns[tableColumnIndex];
                if (column == null)
                {
                    column = this.columns[tableColumnIndex] = new TableColumn();
                }
                if (!string.IsNullOrEmpty(formula) && formula.StartsWith("="))
                {
                    string str = formula.Substring(1);
                    column.SetFormula(str, column.Value);
                    if (this.showFooter)
                    {
                        this.Sheet.SetCellFormula(this.footerRowIndex, this.column + tableColumnIndex, str);
                    }
                }
                else
                {
                    column.SetFormula(column.Formula, formula);
                    if (this.showFooter)
                    {
                        this.Sheet.SetCellText(this.footerRowIndex, this.column + tableColumnIndex, formula);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the table header text with a specified index.
        /// </summary>
        /// <param name="tableColumnIndex">The column index of the table header. The index is zero-based.</param>
        /// <param name="name">The header text. A null value indicates that the cell value of the sheet is displayed.</param>
        public void SetColumnName(int tableColumnIndex, string name)
        {
            if (((this.columns != null) && (tableColumnIndex >= 0)) && (tableColumnIndex < this.columns.Length))
            {
                TableColumn column = this.columns[tableColumnIndex];
                if (column == null)
                {
                    column = this.columns[tableColumnIndex] = new TableColumn();
                }
                if (name == null)
                {
                    if (!column.IsAutoName)
                    {
                        column.SetName(this.CalcAutoIdx());
                    }
                }
                else
                {
                    column.SetName(name);
                }
                if (this.showHeader)
                {
                    this.Sheet.SetCellText(this.headerRowIndex, this.column + tableColumnIndex, column.Name);
                }
            }
        }

        internal bool SetFooter(int row, int column, string formula, object value)
        {
            if (this.FooterIndex != row)
            {
                return false;
            }
            int tableColumnIndex = column - this.Range.Column;
            if (string.IsNullOrEmpty(formula))
            {
                this.SetColumnFormula(tableColumnIndex, (value == null) ? null : value.ToString());
            }
            else
            {
                this.SetColumnFormula(tableColumnIndex, "=" + formula);
            }
            return true;
        }

        internal bool SetHeader(int row, int column, object value)
        {
            if (this.headerRowIndex == row)
            {
                int tableColumnIndex = column - this.Range.Column;
                this.SetColumnName(tableColumnIndex, (value == null) ? ((string)(value as string)) : value.ToString());
                return true;
            }
            return false;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Init();
            List<TableColumn> list = null;
            while (reader.Read())
            {
                string[] strArray;
                int num;
                string str2;
                if (reader.NodeType == XmlNodeType.Element && !string.IsNullOrEmpty(reader.Name))
                {
                    switch (reader.Name)
                    {
                        case "Name":
                            this.name = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            break;

                        case "Row":
                            this.row = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "Column":
                            this.column = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "RowCount":
                            this.rowCount = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "ColumnCount":
                            this.columnCount = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "DataSource":
                            {
                                object source = Serializer.DeserializeObj(null, reader);
                                if (source != null)
                                {
                                    this.dataConnection = ConnectionBuilder.Build(source);
                                    if (this.dataConnection.CanOpen())
                                    {
                                        this.dataConnection.Open();
                                    }
                                }
                                break;
                            }
                        case "Style":
                            this.style = Serializer.DeserializeObj(typeof(TableStyle), reader) as TableStyle;
                            break;

                        case "Items":
                            list = new List<TableColumn>();
                            strArray = ((string)(Serializer.DeserializeObj(typeof(string), reader) as string)).Split(new string[] { "\r\r" }, StringSplitOptions.None);
                            if ((strArray.Length % 3) != 0)
                            {
                                break;
                            }

                            num = 0;
                            while (num < strArray.Length)
                            {
                                str2 = strArray[num];
                                string str3 = strArray[num + 1];
                                string str4 = strArray[num + 2];
                                TableColumn item = new TableColumn();
                                item.SetName(str2);
                                item.SetFormula(string.IsNullOrEmpty(str3) ? null : str3.Trim(new char[] { '"' }), string.IsNullOrEmpty(str4) ? null : str4.Trim(new char[] { '"' }));
                                list.Add(item);
                                num += 3;
                            }
                            break;

                        case "ShowHeader":
                            this.showHeader = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "ShowFooter":
                            this.showFooter = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "HighlightFirstColumn":
                            this.highlightFirstColumn = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "HighlightLastColumn":
                            this.highlightLastColumn = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "BandedRows":
                            this.bandedRows = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "BandedColumns":
                            this.bandedColumns = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "HeaderRowIndex":
                            this.headerRowIndex = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "FooterRowIndex":
                            this.footerRowIndex = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "RowFilter":
                            this.rowfilter = Serializer.DeserializeObj(null, reader) as TableFilter;
                            if (this.rowfilter != null)
                            {
                                this.rowfilter.Table = this;
                            }
                            break;
                    }
                }
            }

            if (list != null)
            {
                this.columns = new SparseArray<TableColumn>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    this.columns[i] = list[i];
                }
            }
            else
            {
                this.columns = new SparseArray<TableColumn>(this.columnCount);
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.name != null)
            {
                Serializer.SerializeObj(this.name, "Name", writer);
            }
            if (this.row != -1)
            {
                Serializer.SerializeObj((int)this.row, "Row", writer);
            }
            if (this.column != -1)
            {
                Serializer.SerializeObj((int)this.column, "Column", writer);
            }
            if (this.rowCount != -1)
            {
                Serializer.SerializeObj((int)this.rowCount, "RowCount", writer);
            }
            if (this.columnCount != -1)
            {
                Serializer.SerializeObj((int)this.columnCount, "ColumnCount", writer);
            }
            if ((this.dataConnection != null) && (this.dataConnection.DataSource is IXmlSerializable))
            {
                Serializer.SerializeObj(this.dataConnection.DataSource, "DataSource", writer);
            }
            if (this.style != null)
            {
                Serializer.SerializeObj(this.style, "Style", writer);
            }
            if (this.columns != null)
            {
                using (IEnumerator<TableColumn> enumerator = this.columns.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Serializer.SerializeObj(enumerator.Current, "TableColumn", writer);
                    }
                }
            }
            if (!this.showHeader)
            {
                Serializer.SerializeObj((bool)this.showHeader, "ShowHeader", writer);
            }
            if (this.showFooter)
            {
                Serializer.SerializeObj((bool)this.showFooter, "ShowFooter", writer);
            }
            if (this.highlightFirstColumn)
            {
                Serializer.SerializeObj((bool)this.highlightFirstColumn, "HighlightFirstColumn", writer);
            }
            if (this.highlightLastColumn)
            {
                Serializer.SerializeObj((bool)this.highlightLastColumn, "HighlightLastColumn", writer);
            }
            if (!this.bandedRows)
            {
                Serializer.SerializeObj((bool)this.bandedRows, "BandedRows", writer);
            }
            if (this.bandedColumns)
            {
                Serializer.SerializeObj((bool)this.bandedColumns, "BandedColumns", writer);
            }
            Serializer.SerializeObj((int)this.headerRowIndex, "HeaderRowIndex", writer);
            Serializer.SerializeObj((int)this.footerRowIndex, "FooterRowIndex", writer);
            if (this.rowfilter != null)
            {
                Serializer.SerializeObj(this.rowfilter, "RowFilter", writer);
            }
        }

        internal void UpdateFilter()
        {
            if (this.rowfilter != null)
            {
                this.rowfilter.Sheet = this.Sheet as Worksheet;
                this.rowfilter.UpdateRange(this.DataRange);
                this.rowfilter.ReFilter();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direct">
        /// 1) cell to cached
        /// 2) cached to cell
        /// 3) auto,when column value and formula are null, then it is equals to 1 (cell to cahche) or it equals to 2.
        /// 0) nothing
        /// </param>
        internal void UpdateFooter(int direct)
        {
            if (this.showFooter)
            {
                for (int i = 0; i < this.columnCount; i++)
                {
                    int num2 = this.column + i;
                    int num3 = direct;
                    TableColumn column = this.columns[i];
                    if (column == null)
                    {
                        column = new TableColumn();
                        this.columns[i] = column;
                    }
                    switch (num3)
                    {
                        case 3:
                            num3 = ((column.Value == null) && (column.Formula == null)) ? 1 : 2;
                            break;

                        case 1:
                            {
                                string cellFormula = this.Sheet.GetCellFormula(this.footerRowIndex, num2);
                                if (string.IsNullOrEmpty(cellFormula))
                                {
                                    string cellText = this.Sheet.GetCellText(this.footerRowIndex, num2);
                                    column.SetFormula(null, cellText);
                                }
                                else
                                {
                                    column.SetFormula(cellFormula, null);
                                }
                                break;
                            }
                    }
                    if (num3 == 2)
                    {
                        if (column.Formula != null)
                        {
                            this.Sheet.SetCellFormula(this.footerRowIndex, num2, column.Formula);
                        }
                        if (column.Value != null)
                        {
                            this.Sheet.SetCellText(this.footerRowIndex, num2, (column.Value == null) ? null : column.Value.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direct">
        /// 1) cell to cached
        /// 2) cached to cell
        /// 3) auto, when column name is auto-name, then it equals to 1(cell to cached) or it equals to 2.
        /// 0) nothing
        /// </param>
        internal void UpdateHeader(int direct)
        {
            if (this.showHeader)
            {
                for (int i = 0; i < this.columnCount; i++)
                {
                    int num2 = this.column + i;
                    int num3 = direct;
                    TableColumn column = this.columns[i];
                    if (column == null)
                    {
                        column = new TableColumn();
                        this.columns[i] = column;
                    }
                    switch (num3)
                    {
                        case 3:
                            num3 = column.IsAutoName ? 1 : 2;
                            break;

                        case 1:
                            {
                                string cellText = this.Sheet.GetCellText(this.headerRowIndex, num2);
                                if (cellText != null)
                                {
                                    column.SetName(cellText);
                                }
                                break;
                            }
                    }
                    if (column.Name == null)
                    {
                        column.SetName(this.CalcAutoIdx());
                    }
                    if (num3 == 2)
                    {
                        this.Sheet.SetCellText(this.headerRowIndex, num2, column.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display an alternating column style.
        /// </summary>
        [DefaultValue(false)]
        public bool BandedColumns
        {
            get { return this.bandedColumns; }
            set
            {
                if (this.bandedColumns != value)
                {
                    this.bandedColumns = value;
                    this.RaisePropertyChanged("BandedColumns");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display an alternating row style.
        /// </summary>
        [DefaultValue(true)]
        public bool BandedRows
        {
            get { return this.bandedRows; }
            set
            {
                if (this.bandedRows != value)
                {
                    this.bandedRows = value;
                    this.RaisePropertyChanged("BandedRows");
                }
            }
        }

        /// <summary>
        /// Gets the cell range for the table data area.
        /// </summary>
        public CellRange DataRange
        {
            get { return this.CalcDataRange(); }
        }

        /// <summary>
        /// Gets the footer index in the sheet.
        /// </summary>
        public int FooterIndex
        {
            get { return this.footerRowIndex; }
        }

        /// <summary>
        /// Gets the header index in the sheet.
        /// </summary>
        public int HeaderIndex
        {
            get { return this.headerRowIndex; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to highlight the first column.
        /// </summary>
        [DefaultValue(false)]
        public bool HighlightFirstColumn
        {
            get { return this.highlightFirstColumn; }
            set
            {
                if (this.highlightFirstColumn != value)
                {
                    this.highlightFirstColumn = value;
                    this.RaisePropertyChanged("HighlightFirstColumn");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to highlight the last column.
        /// </summary>
        [DefaultValue(false)]
        public bool HighlightLastColumn
        {
            get { return this.highlightLastColumn; }
            set
            {
                if (this.highlightLastColumn != value)
                {
                    this.highlightLastColumn = value;
                    this.RaisePropertyChanged("HighlightLastColumn");
                }
            }
        }

        internal bool IsCopying
        {
            get { return this.copying; }
            set { this.copying = value; }
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Represents a range of the whole table.
        /// </summary>
        public CellRange Range
        {
            get { return new CellRange(this.row, this.column, this.rowCount, this.columnCount); }
        }

        /// <summary>
        /// Gets the row filter of the table.
        /// </summary>
        public RowFilterBase RowFilter
        {
            get
            {
                if (this.rowfilter == null)
                {
                    this.rowfilter = new TableFilter(this);
                    this.rowfilter.Sheet = this.Sheet as Worksheet;
                    this.rowfilter.UpdateRange(this.DataRange);
                    this.rowfilter.Changed += new EventHandler<FilterEventArgs>(this.rowfilter_Changed);
                }
                return this.rowfilter;
            }
        }

        ITableSheet Sheet
        {
            get
            {
                if (this.owner != null)
                {
                    return this.owner.InnerSheet;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display a footer.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowFooter
        {
            get { return this.showFooter; }
            set
            {
                if (this.showFooter != value)
                {
                    if (value)
                    {
                        this.rowCount++;
                        if (this.rowCount > this.owner.InnerSheet.RowCount)
                        {
                            throw new ArgumentOutOfRangeException(string.Format(ResourceStrings.TableShowFooterError, (object[])new object[] { ((int)this.rowCount) }));
                        }
                        this.showFooter = value;
                        this.footerRowIndex = this.CalcFooterIndex();
                        this.UpdateFooter(3);
                    }
                    else
                    {
                        this.UpdateFooter(1);
                        this.ClearTableRow(this.footerRowIndex);
                        this.showFooter = value;
                        this.footerRowIndex = this.CalcFooterIndex();
                        this.rowCount--;
                    }
                    this.RaisePropertyChanged("ShowFooter");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display a header.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowHeader
        {
            get { return this.showHeader; }
            set
            {
                if (this.showHeader != value)
                {
                    if (value)
                    {
                        if (this.row <= 0)
                        {
                            throw new NotSupportedException(ResourceStrings.TableShowHeaderError);
                        }
                        this.row--;
                        this.showHeader = value;
                        this.headerRowIndex = this.CalcHeaderIndex();
                        this.rowCount++;
                        this.UpdateHeader(3);
                    }
                    else
                    {
                        this.UpdateHeader(1);
                        this.ClearTableRow(this.headerRowIndex);
                        this.row++;
                        this.showHeader = value;
                        this.headerRowIndex = this.CalcHeaderIndex();
                        this.rowCount--;
                        if (this.rowfilter != null)
                        {
                            this.rowfilter.Unfilter();
                            this.rowfilter.Reset();
                        }
                    }
                    this.RaisePropertyChanged("ShowHeader");
                }
            }
        }

        /// <summary>
        /// Gets or sets a style for the table.
        /// </summary>
        [DefaultValue((string)null)]
        public TableStyle Style
        {
            get { return this.style; }
            set
            {
                this.style = (value == null) ? null : (value.Clone() as TableStyle);
                this.RaisePropertyChanged("Style");
            }
        }

        internal class TableColumn : IXmlSerializable
        {
            public TableColumn()
            {
                this.Name = null;
                this.Formula = null;
            }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                this.Name = null;
                this.Formula = null;
                this.Value = null;
                while (reader.Read())
                {
                    if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
                    {
                        switch (reader.Name)
                        {
                            case "TableColumnName":
                                {
                                    string name = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                    this.SetName(name);
                                    break;
                                }
                            case "TableColumnFormula":
                                this.Formula = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                break;

                            case "TableColumnValue":
                                this.Value = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                break;

                            case "TableColumnValueTotalsRowLabel":
                                this.TotalsRowLabel = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                break;

                            case "TableColumnValueTotalsRowFunction":
                                this.TotalsRowFunction = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                break;

                            case "TableColumnValueTotalsRowCustomFunction":
                                this.TotalsRowCustomFunction = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                break;

                            case "TableColumnValueCalculatedColumnFormula":
                                this.CalculatedColumnFormula = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                break;

                            case "TableColumnValueTotalsRowFunctionIsArrayFormula":
                                this.TotalsRowFunctionIsArrayFormula = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                                break;

                            case "TableColumnValueCalculatedColumnFormulaIsArrayFormula":
                                this.CalculatedColumnFormulaIsArrayFormula = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                                break;
                        }
                    }
                }
            }

            public void SetFormula(string formula, string value)
            {
                this.Formula = formula;
                this.Value = value;
            }

            public void SetName(short idx)
            {
                this.Name = SheetTable.COLUMN_NAME_PREFIX + ((short)idx);
                this.IsAutoName = true;
            }

            public void SetName(string name)
            {
                short num;
                if ((!string.IsNullOrEmpty(name) && name.StartsWith(SheetTable.COLUMN_NAME_PREFIX)) && short.TryParse(name.Substring(SheetTable.COLUMN_NAME_PREFIX.Length), out num))
                {
                    this.SetName(num);
                }
                else
                {
                    this.Name = name;
                    this.IsAutoName = false;
                }
            }

            public void WriteXml(XmlWriter writer)
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    Serializer.SerializeObj(this.Name, "TableColumnName", writer);
                }
                if (!string.IsNullOrWhiteSpace(this.Formula))
                {
                    Serializer.SerializeObj(this.Formula, "TableColumnFormula", writer);
                }
                if (!string.IsNullOrWhiteSpace(this.Value))
                {
                    Serializer.SerializeObj(this.Value, "TableColumnValue", writer);
                }
                if (!string.IsNullOrWhiteSpace(this.TotalsRowLabel))
                {
                    Serializer.SerializeObj(this.TotalsRowLabel, "TableColumnValueTotalsRowLabel", writer);
                }
                if (!string.IsNullOrWhiteSpace(this.TotalsRowFunction))
                {
                    Serializer.SerializeObj(this.TotalsRowFunction, "TableColumnValueTotalsRowFunction", writer);
                }
                if (!string.IsNullOrWhiteSpace(this.TotalsRowCustomFunction))
                {
                    Serializer.SerializeObj(this.TotalsRowCustomFunction, "TableColumnValueTotalsRowCustomFunction", writer);
                }
                if (!string.IsNullOrWhiteSpace(this.CalculatedColumnFormula))
                {
                    Serializer.SerializeObj(this.CalculatedColumnFormula, "TableColumnValueCalculatedColumnFormula", writer);
                }
                Serializer.SerializeObj((bool)this.TotalsRowFunctionIsArrayFormula, "TableColumnValueTotalsRowFunctionIsArrayFormula", writer);
                Serializer.SerializeObj((bool)this.CalculatedColumnFormulaIsArrayFormula, "TableColumnValueCalculatedColumnFormulaIsArrayFormula", writer);
            }

            internal string CalculatedColumnFormula { get; set; }

            internal bool CalculatedColumnFormulaIsArrayFormula { get; set; }

            public string Formula { get; private set; }

            public bool IsAutoName { get; private set; }

            public string Name { get; private set; }

            internal string TotalsRowCustomFunction { get; set; }

            internal string TotalsRowFunction { get; set; }

            internal bool TotalsRowFunctionIsArrayFormula { get; set; }

            internal string TotalsRowLabel { get; set; }

            public string Value { get; private set; }
        }
    }
}

