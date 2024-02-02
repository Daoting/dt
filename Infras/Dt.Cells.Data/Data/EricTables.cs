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
using System.Globalization;
using System.Reflection;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a table collection for tables.
    /// </summary>
    /// <remarks>
    /// I promised Eric to name the collection to EricTables. Just for fun.
    /// </remarks>
    internal sealed class EricTables : IRangeSupport
    {
        List<SheetTable> innerList;
        internal ITableSheet innerSheet;

        public event EventHandler<TableChangedArgs> SheetTableChanged;

        public EricTables() : this(null)
        {
        }

        public EricTables(ITableSheet innerSheet)
        {
            this.innerSheet = innerSheet;
        }

        public SheetTable Add(SheetTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (this.innerList != null)
            {
                foreach (SheetTable table2 in this.innerList)
                {
                    if ((table2 != null) && (CultureInfo.InvariantCulture.CompareInfo.Compare(table.Name, table2.Name, (CompareOptions) CompareOptions.IgnoreCase) == 0))
                    {
                        throw new NotSupportedException(string.Format(ResourceStrings.TableCollectionAddTableError, (object[]) new object[] { table2.Name }));
                    }
                }
            }
            table.owner = this;
            if (this.innerList == null)
            {
                this.innerList = new List<SheetTable>();
            }
            this.innerList.Add(table);
            table.PropertyChanged += new PropertyChangedEventHandler(this.OnTablePropertyChanged);
            return table;
        }

        public SheetTable FindTable(string tableName)
        {
            if (this.innerList != null)
            {
                foreach (SheetTable table in this.innerList)
                {
                    if (string.Compare(tableName, table.Name) == 0)
                    {
                        return table;
                    }
                }
            }
            return null;
        }

        public SheetTable FindTable(int row, int column)
        {
            if (this.innerList != null)
            {
                for (int i = 0; i < this.innerList.Count; i++)
                {
                    SheetTable table = this.innerList[i];
                    CellRange range = table.Range;
                    if (((range.Row <= row) && (row <= ((range.Row + range.RowCount) - 1))) && ((range.Column <= column) && (column <= ((range.Column + range.ColumnCount) - 1))))
                    {
                        return table;
                    }
                }
            }
            return null;
        }

        void IRangeSupport.AddColumns(int column, int count)
        {
            if (this.innerList != null)
            {
                foreach (SheetTable table in this.innerList)
                {
                    if (table != null)
                    {
                        ((IRangeSupport) table).AddColumns(column, count);
                    }
                }
            }
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            if (this.innerList != null)
            {
                foreach (SheetTable table in this.innerList)
                {
                    if (table != null)
                    {
                        ((IRangeSupport) table).AddRows(row, count);
                    }
                }
            }
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            if (this.innerList != null)
            {
                List<SheetTable> list = new List<SheetTable>();
                CellRange range = new CellRange(row, column, rowCount, columnCount);
                foreach (SheetTable table in this.innerList)
                {
                    if (range.Contains(table.Range))
                    {
                        list.Add(table);
                    }
                }
                foreach (SheetTable table2 in list)
                {
                    this.innerList.Remove(table2);
                }
                for (int i = 0; i < this.innerList.Count; i++)
                {
                    SheetTable table3 = this.innerList[i];
                    if (table3 != null)
                    {
                        ((IRangeSupport) table3).Clear(row, column, rowCount, columnCount);
                    }
                }
            }
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (this.innerList != null)
            {
                for (int i = 0; i < this.innerList.Count; i++)
                {
                    SheetTable table = this.innerList[i];
                    if (table != null)
                    {
                        ((IRangeSupport) table).Copy(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
                    }
                }
            }
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (this.innerList != null)
            {
                foreach (SheetTable table in this.innerList)
                {
                    if (table != null)
                    {
                        ((IRangeSupport) table).Move(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
                    }
                }
            }
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            if (this.innerList != null)
            {
                List<SheetTable> list = new List<SheetTable>();
                foreach (SheetTable table in this.innerList)
                {
                    if ((table != null) && !this.IsCoverTable(-1, column, this.innerSheet.RowCount, count, table))
                    {
                        ((IRangeSupport) table).RemoveColumns(column, count);
                        list.Add(table);
                    }
                }
                if (list != null)
                {
                    this.innerList = list;
                }
            }
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            if (this.innerList != null)
            {
                List<SheetTable> list = new List<SheetTable>();
                foreach (SheetTable table in this.innerList)
                {
                    if ((table != null) && !this.IsCoverTable(row, -1, count, this.innerSheet.ColumnCount, table))
                    {
                        ((IRangeSupport) table).RemoveRows(row, count);
                        list.Add(table);
                    }
                }
                if (list != null)
                {
                    this.innerList = list;
                }
            }
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (this.innerList != null)
            {
                foreach (SheetTable table in this.innerList)
                {
                    if (table != null)
                    {
                        ((IRangeSupport) table).Swap(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
                    }
                }
            }
        }

        public bool Intersects(int row, int column, int rowCount, int columnCount)
        {
            if (this.innerList != null)
            {
                using (List<SheetTable>.Enumerator enumerator = this.innerList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Range.Intersects(row, column, rowCount, columnCount))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        bool IsCoverTable(int row, int column, int rowCount, int columnCount, SheetTable table)
        {
            CellRange range = table.Range;
            return ((((row <= range.Row) && (column <= range.Column)) && ((row + rowCount) >= (range.Row + range.RowCount))) && ((column + columnCount) >= (range.Column + range.ColumnCount)));
        }

        void OnTablePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                SheetTable table = sender as SheetTable;
                string str = (table == null) ? string.Empty : table.Name;
                if (this.innerList != null)
                {
                    foreach (SheetTable table2 in this.innerList)
                    {
                        if (!object.ReferenceEquals(table2, sender) && (CultureInfo.InvariantCulture.CompareInfo.Compare(table2.Name, str, (CompareOptions) CompareOptions.IgnoreCase) == 0))
                        {
                            throw new NotSupportedException(string.Format(ResourceStrings.TableCollectionAddTableError, (object[]) new object[] { str }));
                        }
                    }
                }
            }
            if (this.SheetTableChanged != null)
            {
                this.SheetTableChanged(this, new TableChangedArgs(sender as SheetTable, e.PropertyName));
            }
        }

        public void RemoveTable(SheetTable table)
        {
            if (table == null)
            {
                throw new NullReferenceException("table");
            }
            if (this.innerList != null)
            {
                table.PropertyChanged -= new PropertyChangedEventHandler(this.OnTablePropertyChanged);
                if (this.innerList.Remove(table))
                {
                    table.Clear();
                }
            }
        }

        internal SheetTable[] ToArray()
        {
            if (this.innerList != null)
            {
                return this.innerList.ToArray();
            }
            return new SheetTable[0];
        }

        public int Count
        {
            get
            {
                if (this.innerList == null)
                {
                    return 0;
                }
                return this.innerList.Count;
            }
        }

        internal ITableSheet InnerSheet
        {
            get { return  this.innerSheet; }
        }

        internal SheetTable this[int index]
        {
            get { return  this.innerList[index]; }
        }
    }
}

