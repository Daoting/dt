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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Implements sorting.
    /// </summary>
    internal class SortImp
    {
        /// <summary>
        /// The sheet object.
        /// </summary>
        Worksheet worksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SortImp" /> class.
        /// </summary>
        /// <param name="worksheet">The sheet object.</param>
        public SortImp(Worksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        /// <summary>
        /// Sort with bubble sorting
        /// </summary>
        /// <param name="list">List to sort</param>
        /// <param name="comparer">The comparer</param>
        void BubbleSort(Array list, IComparer comparer)
        {
            bool flag = false;
            for (int i = 1; (i < list.Length) && !flag; i++)
            {
                flag = true;
                for (int j = 0; j < (list.Length - i); j++)
                {
                    if (comparer.Compare(list.GetValue(new int[] { j }), list.GetValue(new int[] { j + 1 })) > 0)
                    {
                        flag = false;
                        object obj2 = list.GetValue(new int[] { j });
                        list.SetValue(list.GetValue(new int[] { j + 1 }), new int[] { j });
                        list.SetValue(obj2, new int[] { j + 1 });
                    }
                }
            }
        }

        /// <summary>
        /// Sort with insertion sorting
        /// </summary>
        /// <param name="list">List to sort</param>
        /// <param name="comparer">The comparer</param>
        void InsertionSort(Array list, IComparer comparer)
        {
            for (int i = 1; i < list.Length; i++)
            {
                object y = list.GetValue(new int[] { i });
                int num2 = i;
                while ((num2 > 0) && (comparer.Compare(list.GetValue(new int[] { num2 - 1 }), y) > 0))
                {
                    list.SetValue(list.GetValue(new int[] { num2 - 1 }), new int[] { num2 });
                    num2--;
                }
                list.SetValue(y, new int[] { num2 });
            }
        }

        /// <summary>
        /// Performs a quick sort on the specified range and returns an array of new
        /// model row or column indexes for the range.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column coun.t</param>
        /// <param name="byRows">If set to <c>true</c>, [by rows]</param>
        /// <param name="sortInfo">The sort information.</param>
        /// <param name="sortingAlgorithmType">The sorting algorithm type.</param>
        /// <returns>Return result</returns>
        public int[] QuickSort(int row, int column, int rowCount, int columnCount, bool byRows, SortInfo[] sortInfo, SortingAlgorithmType sortingAlgorithmType)
        {
            int num2;
            int num = byRows ? rowCount : columnCount;
            int[] list = new int[num];
            if (byRows)
            {
                for (num2 = 0; num2 < num; num2++)
                {
                    list[num2] = this.worksheet.GetModelRowFromViewRow(row + num2);
                }
            }
            else
            {
                for (num2 = 0; num2 < num; num2++)
                {
                    list[num2] = this.worksheet.GetModelColumnFromViewColumn(column + num2);
                }
            }
            using (SpreadSortComparer comparer = new SpreadSortComparer(this.worksheet, byRows, sortInfo))
            {
                switch (sortingAlgorithmType)
                {
                    case SortingAlgorithmType.QuickSort:
                        Array.Sort(list, 0, num, comparer);
                        break;

                    case SortingAlgorithmType.BubbleSort:
                        this.BubbleSort(list, comparer);
                        break;

                    case SortingAlgorithmType.InsertionSort:
                        this.InsertionSort(list, comparer);
                        break;

                    default:
                        Array.Sort(list, 0, num, comparer);
                        break;
                }
                this.BubbleSort(list, comparer);
            }
            return list;
        }

        /// <summary>
        /// Determines whether the specified range contains cell spans.
        /// Returns true if the specified range contains cell spans.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <returns>Whether the range contains cell spans.</returns>
        public bool RangeContainsSpans(int row, int column, int rowCount, int columnCount)
        {
            for (int i = row; i < (row + rowCount); i++)
            {
                int modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(i);
                for (int j = column; j < (column + columnCount); j++)
                {
                    int modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(j);
                    if (this.worksheet.GetSpanCell(modelRowFromViewRow, modelColumnFromViewColumn, SheetArea.Cells) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

