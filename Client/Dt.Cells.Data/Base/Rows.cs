#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
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
    /// Represents a range of <see cref="T:Dt.Cells.Data.Row" /> objects.
    /// </summary>
    public sealed class Rows
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
        /// Creates a new set of rows for the specified worksheet and specifies the sheet area for them.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="sheetArea">The sheet area.</param>
        internal Rows(Worksheet worksheet, SheetArea sheetArea)
        {
            this.worksheet = worksheet;
            this.sheetArea = sheetArea;
        }

        /// <summary>
        /// Finds the row that has the specified label.
        /// </summary>
        /// <param name="label">The label of the row.</param>
        /// <returns>Returns a <see cref="T:Dt.Cells.Data.Row" /> object for the specified label.</returns>
        public Row GetByLabel(string label)
        {
            return this.GetByLabel(label, 0, false);
        }

        /// <summary>
        /// Finds the row that has the specified label and lets the search ignore the case.
        /// </summary>
        /// <param name="label">The label of the row.</param>
        /// <param name="ignoreCase">Whether to ignore case in the search.</param>
        /// <returns>Returns a <see cref="T:Dt.Cells.Data.Row" /> object for the specified label.</returns>
        /// <remarks>Set <paramref name="ignoreCase" /> to <c>true</c> to ignore case in the search.</remarks>
        public Row GetByLabel(string label, bool ignoreCase)
        {
            return this.GetByLabel(label, 0, ignoreCase);
        }

        /// <summary>
        /// Finds the row that has the specified label starting at a specified row index and lets the search ignore the case.
        /// </summary>
        /// <param name="label">The label of the row.</param>
        /// <param name="startIndex">The row index at which to start the search.</param>
        /// <param name="ignoreCase">Whether to ignore case in the search.</param>
        /// <returns>Returns a <see cref="T:Dt.Cells.Data.Row" /> object for the specified label.</returns>
        /// <remarks>Set <paramref name="ignoreCase" /> to <c>true</c> to ignore case in the search.</remarks>
        public Row GetByLabel(string label, int startIndex, bool ignoreCase)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }
            if ((this.sheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) || (this.sheetArea == SheetArea.Cells))
            {
                int autoTextIndex = this.worksheet.RowHeader.AutoTextIndex;
                if ((autoTextIndex < 0) || (autoTextIndex >= this.worksheet.RowHeaderColumnCount))
                {
                    autoTextIndex = this.worksheet.RowHeaderColumnCount - 1;
                }
                for (int i = startIndex; i < this.Count; i++)
                {
                    string rowLabel = this.worksheet.GetRowLabel(i, autoTextIndex);
                    if (label.Equals(rowLabel, ignoreCase ? ((StringComparison) StringComparison.CurrentCultureIgnoreCase) : ((StringComparison) StringComparison.CurrentCulture)))
                    {
                        return new Row(this.worksheet, i, this.sheetArea);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the number of rows in the collection.
        /// </summary>
        /// <value>
        /// The number of rows in the collection. The default value is 500.
        /// </value>
        [DefaultValue(500)]
        public int Count
        {
            get
            {
                if (this.worksheet != null)
                {
                    return this.worksheet.GetRowCount(this.sheetArea);
                }
                return 0;
            }
            set
            {
                if (this.worksheet != null)
                {
                    this.worksheet.SetRowCount(this.sheetArea, value);
                }
            }
        }

        /// <summary>
        /// Gets the row for the specified tag.
        /// </summary>
        /// <param name="tag">The tag for which to retrieve the row index.</param>
        /// <value>The row object.</value>
        public Row this[string tag]
        {
            get
            {
                if ((tag != null) && (this.worksheet != null))
                {
                    int rowCount = this.worksheet.GetRowCount(this.sheetArea);
                    for (int i = 0; i < rowCount; i++)
                    {
                        if (tag.Equals(this.worksheet.GetTag(i, -1, this.sheetArea)))
                        {
                            return new Row(this.worksheet, i, this.sheetArea);
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the specified range of rows.
        /// </summary>
        /// <param name="row">The starting row index.</param>
        /// <param name="row2">The ending row index.</param>
        /// <value>The <see cref="T:Dt.Cells.Data.Row" /> object.</value>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified row index is out of range. It must be less than the row count or -1 for all.
        /// </exception>
        public Row this[int row, int row2]
        {
            get
            {
                if (this.worksheet != null)
                {
                    int rowCount = this.worksheet.GetRowCount(this.sheetArea);
                    if (((-1 <= row) && (row < rowCount)) && ((row <= row2) && (row2 < rowCount)))
                    {
                        return new Row(this.worksheet, row, row2, this.sheetArea);
                    }
                }
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.RowIndexerOutOfRangeError, (object[]) new object[] { ((int) row), ((int) row2) }));
            }
        }

        /// <summary>
        /// Gets the specified row.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <value>The <see cref="T:Dt.Cells.Data.Row" /> object.</value>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified row index is out of range. It must be less than the row count or -1 for all.
        /// </exception>
        public Row this[int row]
        {
            get
            {
                if (((this.worksheet == null) || (-1 > row)) || (row >= this.worksheet.GetRowCount(this.sheetArea)))
                {
                    throw new IndexOutOfRangeException("row");
                }
                return new Row(this.worksheet, row, this.sheetArea);
            }
        }

        /// <summary>
        /// Gets the parent object containing this collection of rows.
        /// </summary>
        /// <value>The parent object that contains this <see cref="T:Dt.Cells.Data.Rows" /> object.</value>
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

