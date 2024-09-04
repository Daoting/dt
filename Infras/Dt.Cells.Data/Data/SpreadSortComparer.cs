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
using System.Globalization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// The SpreadSortComparer Class
    /// </summary>
    internal class SpreadSortComparer : IComparer, IDisposable
    {
        /// <summary>
        /// The sort by row.
        /// </summary>
        bool byRows;
        Dictionary<string, object> cachedValues = new Dictionary<string, object>();
        /// <summary>
        /// The sort info array.
        /// </summary>
        SortInfo[] sortInfo;
        /// <summary>
        /// The sheet object.
        /// </summary>
        Worksheet worksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadSortComparer" /> class.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="byRows">If set to <c>true</c>, [by rows].</param>
        /// <param name="sortInfo">The sort information.</param>
        public SpreadSortComparer(Worksheet worksheet, bool byRows, SortInfo[] sortInfo)
        {
            this.worksheet = worksheet;
            this.byRows = byRows;
            this.sortInfo = sortInfo;
        }

        /// <summary>
        /// Performs the comparison of rows or columns using the specified order and comparer,
        /// or using the default comparer if one was not specified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sortKey">The sort key</param>
        /// <param name="row">The row index</param>
        /// <param name="col">The column index</param>
        /// <param name="row2">The row index2</param>
        /// <param name="col2">The column index2</param>
        /// <returns>The sort result</returns>
        int _SortCompare(int index, int sortKey, int row, int col, int row2, int col2)
        {
            bool ascending = this.sortInfo[sortKey].Ascending;
            IComparer comparer = this.sortInfo[sortKey].Comparer;
            int num = 0;
            if (this.byRows)
            {
                col = col2 = this.worksheet.GetModelColumnFromViewColumn(index);
            }
            else
            {
                row = row2 = this.worksheet.GetModelRowFromViewRow(index);
            }
            object obj2 = this.worksheet.GetValue(row, col);
            object obj3 = this.worksheet.GetValue(row2, col2);
            StyleInfo info = this.worksheet.GetActualStyleInfo(row, col, SheetArea.Cells);
            if ((info.Formatter != null) && (info.Formatter is IFormatValueFlag))
            {
                obj2 = info.Formatter.Format(obj2);
            }
            info = this.worksheet.GetActualStyleInfo(row2, col2, SheetArea.Cells);
            if ((info.Formatter != null) && (info.Formatter is IFormatValueFlag))
            {
                obj3 = info.Formatter.Format(obj3);
            }
            if (comparer == null)
            {
                comparer = Comparer.Default;
                if (((obj2 != null) && (obj3 != null)) && !obj2.GetType().Equals(obj3.GetType()))
                {
                    if (obj2 is int)
                    {
                        obj2 = (double) Convert.ToDouble(obj2, (IFormatProvider) CultureInfo.InvariantCulture);
                    }
                    try
                    {
                        try
                        {
                            obj3 = Convert.ChangeType(obj3, obj2.GetType(), (IFormatProvider) CultureInfo.CurrentCulture);
                        }
                        catch
                        {
                            throw new InvalidCastException();
                        }
                    }
                    catch (InvalidCastException)
                    {
                        obj2 = obj2.ToString();
                        obj3 = obj3.ToString();
                    }
                }
                if (((obj2 == null) || (string.Empty.Equals(obj2) && !string.Empty.Equals(obj3))) && (obj3 != null))
                {
                    return 1;
                }
                if ((obj2 != null) && ((obj3 == null) || (string.Empty.Equals(obj3) && !string.Empty.Equals(obj2))))
                {
                    return -1;
                }
            }
            try
            {
                num = comparer.Compare(obj2, obj3);
            }
            catch (ArgumentException)
            {
            }
            if (!ascending)
            {
                return -num;
            }
            return num;
        }

        public void Dispose()
        {
            this.cachedValues.Clear();
        }

        /// <summary>
        /// Compares two objects and returns a value that indicates whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition less than zero: <paramref name="x" /> is less than <paramref name="y" />. Zero: <paramref name="x" /> equals <paramref name="y" />. Greater than zero: <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither <paramref name="x" /> nor <paramref name="y" /> implements the <see cref="T:System.IComparable" /> interface -or- <paramref name="x" /> and <paramref name="y" /> are of different types and neither one can handle comparisons with the other. </exception>
        int IComparer.Compare(object x, object y)
        {
            int row = 0;
            int col = 0;
            int num3 = 0;
            int num4 = 0;
            int rowCount = this.worksheet.RowCount;
            int columnCount = this.worksheet.ColumnCount;
            if (this.byRows)
            {
                row = (int) ((int) x);
                num3 = (int) ((int) y);
            }
            else
            {
                col = (int) ((int) x);
                num4 = (int) ((int) y);
            }
            int index = 0;
            int num6 = 0;
            while ((index < this.sortInfo.Length) && (num6 == 0))
            {
                if (this.sortInfo[index] != null)
                {
                    int num7 = this.sortInfo[index].Index;
                    if ((0 <= num7) && ((this.byRows && (num7 < columnCount)) || (!this.byRows && (num7 < rowCount))))
                    {
                        num6 = this._SortCompare(num7, index, row, col, num3, num4);
                    }
                }
                index++;
            }
            return num6;
        }
    }
}

