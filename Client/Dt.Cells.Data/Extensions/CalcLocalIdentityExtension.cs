#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class CalcLocalIdentityExtension
    {
        static CompareResult Compare(int start1, int count1, int start2, int count2)
        {
            if (start1 == -1)
            {
                if (start2 == -1)
                {
                    return CompareResult.Equal;
                }
                return CompareResult.Contains;
            }
            if (start2 == -1)
            {
                return CompareResult.Contained;
            }
            int num = (start1 + count1) - 1;
            int num2 = (start2 + count2) - 1;
            if (num2 < start1)
            {
                return CompareResult.Less;
            }
            if (start2 > num)
            {
                return CompareResult.Great;
            }
            if (start2 < start1)
            {
                if (num2 >= num)
                {
                    return CompareResult.Contained;
                }
                return CompareResult.Less_Intersected;
            }
            if (start2 == start1)
            {
                if (num2 < num)
                {
                    return CompareResult.Contains;
                }
                if (num2 == num)
                {
                    return CompareResult.Equal;
                }
                return CompareResult.Contained;
            }
            if (num2 <= num)
            {
                return CompareResult.Contains;
            }
            return CompareResult.Great_Intersected;
        }

        public static CompareResult CompareColumnTo(this CalcLocalIdentity This, CalcLocalIdentity other)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            int num8;
            if (This.ExtractIdentity(out num, out num3, out num2, out num4) && other.ExtractIdentity(out num5, out num7, out num6, out num8))
            {
                return Compare(num3, num4, num7, num8);
            }
            return CompareResult.None;
        }

        public static CompareResult CompareRowTo(this CalcLocalIdentity This, CalcLocalIdentity other)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            int num8;
            if (This.ExtractIdentity(out num, out num3, out num2, out num4) && other.ExtractIdentity(out num5, out num7, out num6, out num8))
            {
                return Compare(num, num2, num5, num6);
            }
            return CompareResult.None;
        }

        public static bool ExtractIdentity(this CalcLocalIdentity id, out int row, out int column, out int rowCount, out int columnCount)
        {
            int num7;
            int num8;
            CalcCellIdentity objA = id as CalcCellIdentity;
            CalcRangeIdentity identity2 = id as CalcRangeIdentity;
            ConditionalGraph.ConditionalIdentity identity3 = id as ConditionalGraph.ConditionalIdentity;
            if (!object.ReferenceEquals(objA, null))
            {
                row = objA.RowIndex;
                column = objA.ColumnIndex;
                rowCount = columnCount = 1;
                return true;
            }
            if (!object.ReferenceEquals(identity2, null))
            {
                if (identity2.IsFullRow && identity2.IsFullColumn)
                {
                    int num2;
                    int num3;
                    columnCount = num2 = -1;
                    rowCount = num3 = num2;
                    row = column = num3;
                }
                else if (identity2.IsFullRow)
                {
                    column = columnCount = -1;
                    row = identity2.RowIndex;
                    rowCount = identity2.RowCount;
                }
                else if (identity2.IsFullColumn)
                {
                    row = rowCount = -1;
                    column = identity2.ColumnIndex;
                    columnCount = identity2.ColumnCount;
                }
                else
                {
                    row = identity2.RowIndex;
                    rowCount = identity2.RowCount;
                    column = identity2.ColumnIndex;
                    columnCount = identity2.ColumnCount;
                }
                return true;
            }
            if (!object.ReferenceEquals(identity3, null))
            {
                return identity3.ActualIdentity.ExtractIdentity(out row, out column, out rowCount, out columnCount);
            }
            columnCount = num7 = -2147483648;
            rowCount = num8 = num7;
            row = column = num8;
            return false;
        }

        public static bool TryCompareColumnTo(this CalcLocalIdentity This, CalcLocalIdentity other, out CompareResult columnCompareResult)
        {
            columnCompareResult = This.CompareColumnTo(other);
            return (columnCompareResult != CompareResult.None);
        }

        public static bool TryCompareRowTo(this CalcLocalIdentity This, CalcLocalIdentity other, out CompareResult rowCompareResult)
        {
            rowCompareResult = This.CompareRowTo(other);
            return (rowCompareResult != CompareResult.None);
        }

        public static bool TryCompareTo(this CalcLocalIdentity This, CalcLocalIdentity other, out CompareResult rowCompareResult, out CompareResult columnCompareResult)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            int num8;
            if (This.ExtractIdentity(out num, out num3, out num2, out num4) && other.ExtractIdentity(out num5, out num7, out num6, out num8))
            {
                rowCompareResult = Compare(num, num2, num5, num6);
                columnCompareResult = Compare(num3, num4, num7, num8);
                return true;
            }
            rowCompareResult = CompareResult.None;
            columnCompareResult = CompareResult.None;
            return false;
        }

        internal enum CompareResult
        {
            None,
            Less,
            Less_Intersected,
            Equal,
            Contained,
            Contains,
            Great_Intersected,
            Great
        }
    }
}

