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
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    internal class StorageBlock : IXmlSerializable
    {
        int columnCount;
        DataMatrix<object> data;
        const int DEFAULT_COLUMN_COUNT = 250;
        const int DEFAULT_ROW_COUNT = 0xfde8;
        int rowCount;
        DataMatrix<Sparkline> sparklines;
        DataMatrix<object> style;
        DataMatrix<object> tags;

        public StorageBlock() : this(0xfde8, 250)
        {
        }

        public StorageBlock(int rowCount, int columnCount)
        {
            this.Init(rowCount, columnCount);
        }

        public void AddColumns(int column, int count)
        {
            this.data.AddColumns(column, count);
            this.style.AddColumns(column, count);
            this.tags.AddColumns(column, count);
            this.sparklines.AddColumns(column, count);
            this.columnCount = this.data.ColumnCount;
        }

        public void AddRows(int row, int count)
        {
            this.data.AddRows(row, count);
            this.style.AddRows(row, count);
            this.tags.AddRows(row, count);
            this.sparklines.AddRows(row, count);
            this.rowCount = this.data.RowCount;
        }

        public void Clear(StorageType type)
        {
            if ((type & StorageType.Style) > ((StorageType) 0))
            {
                this.style.Clear();
            }
            if ((type & StorageType.Data) > ((StorageType) 0))
            {
                this.data.Clear();
            }
            if ((type & StorageType.Sparkline) > ((StorageType) 0))
            {
                this.sparklines.Clear();
            }
            if ((type & StorageType.Tag) > ((StorageType) 0))
            {
                this.tags.Clear();
            }
        }

        public void Clear(int row, int column, int rowCount, int columnCount, StorageType type)
        {
            if ((type & StorageType.Style) > ((StorageType) 0))
            {
                this.style.Clear(row, column, rowCount, columnCount);
            }
            if ((type & StorageType.Data) > ((StorageType) 0))
            {
                this.data.Clear(row, column, rowCount, columnCount);
            }
            if ((type & StorageType.Sparkline) > ((StorageType) 0))
            {
                this.sparklines.Clear(row, column, rowCount, columnCount);
            }
            if ((type & StorageType.Tag) > ((StorageType) 0))
            {
                this.tags.Clear(row, column, rowCount, columnCount);
            }
        }

        public int FirstNonEmptyRow()
        {
            return this.NextNonEmptyRow(-1);
        }

        public int FirstNonEmptyRow(StorageType type)
        {
            return this.NextNonEmptyRow(-1, type);
        }

        public int GetLastDirtyColumn(StorageType type)
        {
            int lastDirtyColumn;
            int num = -1;
            if ((type & StorageType.Style) > ((StorageType) 0))
            {
                lastDirtyColumn = this.style.LastDirtyColumn;
                num = (lastDirtyColumn > num) ? lastDirtyColumn : num;
            }
            if ((type & StorageType.Data) > ((StorageType) 0))
            {
                lastDirtyColumn = this.data.LastDirtyColumn;
                num = (lastDirtyColumn > num) ? lastDirtyColumn : num;
            }
            if ((type & StorageType.Sparkline) > ((StorageType) 0))
            {
                lastDirtyColumn = this.sparklines.LastDirtyColumn;
                num = (lastDirtyColumn > num) ? lastDirtyColumn : num;
            }
            if ((type & StorageType.Tag) > ((StorageType) 0))
            {
                lastDirtyColumn = this.tags.LastDirtyColumn;
                num = (lastDirtyColumn > num) ? lastDirtyColumn : num;
            }
            return num;
        }

        public int GetLastDirtyRow(StorageType type)
        {
            int lastDirtyRow;
            int num = -1;
            if ((type & StorageType.Style) > ((StorageType) 0))
            {
                lastDirtyRow = this.style.LastDirtyRow;
                num = (lastDirtyRow > num) ? lastDirtyRow : num;
            }
            if ((type & StorageType.Data) > ((StorageType) 0))
            {
                lastDirtyRow = this.data.LastDirtyRow;
                num = (lastDirtyRow > num) ? lastDirtyRow : num;
            }
            if ((type & StorageType.Sparkline) > ((StorageType) 0))
            {
                lastDirtyRow = this.sparklines.LastDirtyRow;
                num = (lastDirtyRow > num) ? lastDirtyRow : num;
            }
            if ((type & StorageType.Tag) > ((StorageType) 0))
            {
                lastDirtyRow = this.tags.LastDirtyRow;
                num = (lastDirtyRow > num) ? lastDirtyRow : num;
            }
            return num;
        }

        public List<int> GetNonEmptyColumns()
        {
            List<int> nonEmptyColumns = this.data.GetNonEmptyColumns();
            List<int> list2 = this.style.GetNonEmptyColumns();
            List<int> list3 = this.tags.GetNonEmptyColumns();
            List<int> list4 = this.sparklines.GetNonEmptyColumns();
            return new List<int>(Enumerable.Union<int>(Enumerable.Union<int>(Enumerable.Union<int>((IEnumerable<int>) nonEmptyColumns, (IEnumerable<int>) list2), (IEnumerable<int>) list3), (IEnumerable<int>) list4));
        }

        public List<int> GetNonEmptyRows()
        {
            List<int> nonEmptyRows = this.data.GetNonEmptyRows();
            List<int> list2 = this.style.GetNonEmptyRows();
            List<int> list3 = this.tags.GetNonEmptyRows();
            List<int> list4 = this.sparklines.GetNonEmptyRows();
            return new List<int>(Enumerable.Union<int>(Enumerable.Union<int>(Enumerable.Union<int>((IEnumerable<int>) nonEmptyRows, (IEnumerable<int>) list2), (IEnumerable<int>) list3), (IEnumerable<int>) list4));
        }

        public Sparkline GetSparkline(int row, int column)
        {
            return this.sparklines.GetValue(row, column);
        }

        public object GetStyle(int row, int column)
        {
            return this.style.GetValue(row, column);
        }

        public object GetTag(int row, int column)
        {
            return this.tags.GetValue(row, column);
        }

        public object GetValue(int row, int column)
        {
            return this.data.GetValue(row, column);
        }

        void Init(int rowCount, int columnCount)
        {
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.data = new DataMatrix<object>(rowCount, columnCount);
            this.style = new DataMatrix<object>(rowCount, columnCount);
            this.tags = new DataMatrix<object>(rowCount, columnCount);
            this.sparklines = new DataMatrix<Sparkline>(rowCount, columnCount);
        }

        public int NextNonEmptyRow(int row)
        {
            int num = this.data.NextNonEmptyRow(row);
            if (num == (row + 1))
            {
                return num;
            }
            int num2 = this.style.NextNonEmptyRow(row);
            if ((num < 0) || ((num2 >= 0) && (num2 < num)))
            {
                num = num2;
                if (num == (row + 1))
                {
                    return num;
                }
            }
            num2 = this.tags.NextNonEmptyRow(row);
            if ((num < 0) || ((num2 >= 0) && (num2 < num)))
            {
                num = num2;
            }
            num2 = this.sparklines.NextNonEmptyRow(row);
            if ((num >= 0) && ((num2 < 0) || (num2 >= num)))
            {
                return num;
            }
            return num2;
        }

        public int NextNonEmptyRow(int row, StorageType type)
        {
            int num2;
            int num = -1;
            if ((type & StorageType.Style) > ((StorageType) 0))
            {
                num2 = this.style.NextNonEmptyRow(row);
                num = ((num2 > -1) && ((num == -1) || (num2 < num))) ? num2 : num;
            }
            if ((type & StorageType.Data) > ((StorageType) 0))
            {
                num2 = this.data.NextNonEmptyRow(row);
                num = ((num2 > -1) && ((num == -1) || (num2 < num))) ? num2 : num;
            }
            if ((type & StorageType.Sparkline) > ((StorageType) 0))
            {
                num2 = this.sparklines.NextNonEmptyRow(row);
                num = ((num2 > -1) && ((num == -1) || (num2 < num))) ? num2 : num;
            }
            if ((type & StorageType.Tag) <= ((StorageType) 0))
            {
                return num;
            }
            num2 = this.tags.NextNonEmptyRow(row);
            return (((num2 > -1) && ((num == -1) || (num2 < num))) ? num2 : num);
        }

        public void RemoveColumns(int column, int count)
        {
            this.data.RemoveColumns(column, count);
            this.style.RemoveColumns(column, count);
            this.tags.RemoveColumns(column, count);
            this.sparklines.RemoveColumns(column, count);
            this.columnCount = this.data.ColumnCount;
        }

        public void RemoveRows(int row, int count)
        {
            this.data.RemoveRows(row, count);
            this.style.RemoveRows(row, count);
            this.tags.RemoveRows(row, count);
            this.sparklines.RemoveRows(row, count);
            this.rowCount = this.data.RowCount;
        }

        internal void SerializeData(XmlWriter writer)
        {
            if (this.data.NextNonEmptyRow(-1) > -1)
            {
                Serializer.WriteStartObj("Data", writer);
                Serializer.SerializeMatrix<object>(this.data, writer);
                Serializer.WriteEndObj(writer);
            }
        }

        internal void SerializeStyle(XmlWriter writer)
        {
            if (this.style.NextNonEmptyRow(-1) > -1)
            {
                Serializer.WriteStartObj("Style", writer);
                Serializer.SerializeMatrix<object>(this.style, writer);
                Serializer.WriteEndObj(writer);
            }
            // hdt 报表打印序列化时出错 ExcelPrinter的258行
            //if (this.tags.NextNonEmptyRow(-1) > -1)
            //{
            //    Serializer.WriteStartObj("Tag", writer);
            //    Serializer.SerializeMatrix<object>(this.tags, writer);
            //    Serializer.WriteEndObj(writer);
            //}
        }

        public void SetSparkline(int row, int column, Sparkline value)
        {
            this.sparklines.SetValue(row, column, value);
        }

        public void SetStyle(int row, int column, object value)
        {
            this.style.SetValue(row, column, value);
        }

        public void SetTag(int row, int column, object value)
        {
            this.tags.SetValue(row, column, value);
        }

        public void SetValue(int row, int column, object value)
        {
            this.data.SetValue(row, column, value);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            this.rowCount = Serializer.ReadAttributeInt("rc", 0xfde8, reader);
            this.columnCount = Serializer.ReadAttributeInt("cc", 250, reader);
            this.Init(this.rowCount, this.columnCount);
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "Data")
                    {
                        if (str == "Style")
                        {
                            goto Label_0091;
                        }
                        if (str == "Tag")
                        {
                            goto Label_00A0;
                        }
                    }
                    else
                    {
                        Serializer.DeserializeMatrix<object>(this.data, null, reader);
                    }
                }
                continue;
            Label_0091:
                Serializer.DeserializeMatrix<object>(this.style, null, reader);
                continue;
            Label_00A0:
                Serializer.DeserializeMatrix<object>(this.tags, null, reader);
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            Serializer.WriteAttribute("rc", this.RowCount, writer);
            Serializer.WriteAttribute("cc", this.ColumnCount, writer);
            this.SerializeData(writer);
            this.SerializeStyle(writer);
        }

        public int ColumnCount
        {
            get { return  this.columnCount; }
            set
            {
                this.data.ColumnCount = value;
                this.style.ColumnCount = value;
                this.tags.ColumnCount = value;
                this.sparklines.ColumnCount = value;
                this.columnCount = value;
            }
        }

        public int RowCount
        {
            get { return  this.rowCount; }
            set
            {
                this.data.RowCount = value;
                this.style.RowCount = value;
                this.tags.RowCount = value;
                this.sparklines.RowCount = value;
                this.rowCount = value;
            }
        }
    }
}

