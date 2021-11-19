#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a range of <see cref="T:Column" /> objects.
    /// </summary>
    public sealed class Columns
    {
        /// <summary>
        /// Indicate which area contains this cell.
        /// </summary>
        SheetArea sheetArea;
        /// <summary>
        /// sheet containing Column
        /// </summary>
        Worksheet worksheet;

        /// <summary>
        /// Creates a collection of columns.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="sheetArea">The sheet area.</param>
        internal Columns(Worksheet worksheet, SheetArea sheetArea)
        {
            this.worksheet = worksheet;
            this.sheetArea = sheetArea;
        }

        /// <summary>
        /// Finds the column that uses the specified data field.
        /// </summary>
        /// <param name="dataField">The data field for the column.</param>
        /// <returns>Returns a <see cref="T:Column" /> object.</returns>
        public Column GetByDataField(string dataField)
        {
            if (dataField == null)
            {
                throw new ArgumentNullException("dataField");
            }
            if ((this.sheetArea == SheetArea.ColumnHeader) || (this.sheetArea == SheetArea.Cells))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    string dataColumnName = this.worksheet.GetDataColumnName(i);
                    if (dataField.Equals(dataColumnName))
                    {
                        return new Column(this.worksheet, i, this.sheetArea);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the column that has the specified label.
        /// </summary>
        /// <param name="label">The label for the column.</param>
        /// <returns>Returns a <see cref="T:Column" /> object.</returns>
        public Column GetByLabel(string label)
        {
            return this.GetByLabel(label, 0, false);
        }

        /// <summary>
        /// Finds the column that has the specified label and lets the search ignore case.
        /// </summary>
        /// <param name="label">The label of the column.</param>
        /// <param name="ignoreCase">Set to <c>true</c> to ignore case in the search.</param>
        public Column GetByLabel(string label, bool ignoreCase)
        {
            return this.GetByLabel(label, 0, ignoreCase);
        }

        /// <summary>
        /// Finds the column that has the specified label starting at a specified column index and lets the search ignore case.
        /// </summary>
        /// <param name="label">The label of the column.</param>
        /// <param name="startIndex">Column index at which to start the search.</param>
        /// <param name="ignoreCase">Set to <c>true</c> to ignore case in the search.</param>
        public Column GetByLabel(string label, int startIndex, bool ignoreCase)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }
            if ((this.sheetArea == SheetArea.ColumnHeader) || (this.sheetArea == SheetArea.Cells))
            {
                int autoTextIndex = this.worksheet.ColumnHeader.AutoTextIndex;
                if ((autoTextIndex < 0) || (autoTextIndex >= this.worksheet.ColumnHeaderRowCount))
                {
                    autoTextIndex = this.worksheet.ColumnHeaderRowCount - 1;
                }
                for (int i = startIndex; i < this.Count; i++)
                {
                    string columnLabel = this.worksheet.GetColumnLabel(autoTextIndex, i);
                    if (label.Equals(columnLabel, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                    {
                        return new Column(this.worksheet, i, this.sheetArea);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the number of columns in the collection.
        /// </summary>
        /// <value>
        /// The number of columns in the collection. The default value is 500.
        /// </value>
        [DefaultValue(500)]
        public int Count
        {
            get
            {
                if (this.worksheet != null)
                {
                    return this.worksheet.GetColumnCount(this.sheetArea);
                }
                return 0;
            }
            set
            {
                if (this.worksheet != null)
                {
                    this.worksheet.SetColumnCount(this.sheetArea, value);
                }
            }
        }

        /// <summary>
        /// Gets the column for the specified tag.
        /// </summary>
        /// <param name="tag">The tag for which to retrieve the column index.</param>
        public Column this[string tag]
        {
            get
            {
                if ((tag != null) && (this.worksheet != null))
                {
                    int columnCount = this.worksheet.GetColumnCount(this.sheetArea);
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (tag.Equals(this.worksheet.GetTag(-1, i, this.sheetArea)))
                        {
                            return new Column(this.worksheet, i, this.sheetArea);
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a new column for the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <value>The <see cref="T:Column" /> object in the specified column index.</value>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified column index is out of range. It must be less than the column count or -1 for all.
        /// </exception>
        public Column this[int column]
        {
            get
            {
                if (this.worksheet != null)
                {
                    int columnCount = this.worksheet.GetColumnCount(this.sheetArea);
                    if ((-1 <= column) && (column < columnCount))
                    {
                        return new Column(this.worksheet, column, this.sheetArea);
                    }
                }
                throw new IndexOutOfRangeException(ResourceStrings.InvalidColumnIndex + ((int) column));
            }
        }

        /// <summary>
        /// Gets a new column for the specified range of columns.
        /// </summary>
        /// <param name="column">The starting column index.</param>
        /// <param name="column2">The ending column index.</param>
        /// <value>The <see cref="T:Column" /> object in the specified starting column and ending column index.</value>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified column index is out of range. It must be less than the column count or -1 for all.
        /// </exception>
        public Column this[int column, int column2]
        {
            get
            {
                if (this.worksheet != null)
                {
                    int columnCount = this.worksheet.GetColumnCount(this.sheetArea);
                    if (((-1 <= column) && (column < columnCount)) && ((column <= column2) && (column2 < columnCount)))
                    {
                        return new Column(this.worksheet, column, column2, this.sheetArea);
                    }
                }
                throw new IndexOutOfRangeException(string.Concat((object[]) new object[] { ResourceStrings.InvalidColumnIndex, ((int) column), " or ", ((int) column2) }));
            }
        }

        /// <summary>
        /// Gets the parent object containing this collection of columns.
        /// </summary>
        /// <value>The parent object that contains this <see cref="T:Columns" /> object.</value>
        internal object Parent
        {
            get
            {
                if (this.worksheet != null)
                {
                    switch (this.sheetArea)
                    {
                        case SheetArea.Cells:
                            return this.worksheet;

                        case (SheetArea.CornerHeader | SheetArea.RowHeader):
                            return this.worksheet.RowHeader;

                        case SheetArea.ColumnHeader:
                            return this.worksheet.ColumnHeader;
                    }
                }
                return null;
            }
        }
    }
}

