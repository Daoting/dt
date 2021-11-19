#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    internal class ArrayHelper
    {
        public static int GetColumnCount(object o, short rangeId = 0)
        {
            if (o is CalcArray)
            {
                return ((CalcArray) o).ColumnCount;
            }
            if (o is CalcReference)
            {
                return ((CalcReference) o).GetColumnCount(rangeId);
            }
            return 1;
        }

        public static int GetLength(object o, short rangeId = 0)
        {
            if (o is CalcArray)
            {
                CalcArray array = (CalcArray) o;
                return (array.RowCount * array.ColumnCount);
            }
            if (o is CalcReference)
            {
                CalcReference reference = (CalcReference) o;
                return (reference.GetRowCount(rangeId) * reference.GetColumnCount(rangeId));
            }
            return 1;
        }

        public static int GetRangeCount(object o)
        {
            if (o is CalcReference)
            {
                return (o as CalcReference).RangeCount;
            }
            return 1;
        }

        public static int GetRowCount(object o, short rangeId = 0)
        {
            if (o is CalcArray)
            {
                return ((CalcArray) o).RowCount;
            }
            if (o is CalcReference)
            {
                return ((CalcReference) o).GetRowCount(rangeId);
            }
            return 1;
        }

        public static object GetValue(object o, int i, short rangeId = 0)
        {
            if (o is CalcArray)
            {
                CalcArray array = (CalcArray) o;
                int columnCount = array.ColumnCount;
                return array.GetValue(i / columnCount, i % columnCount);
            }
            if (o is CalcReference)
            {
                CalcReference reference = (CalcReference) o;
                int num2 = reference.GetColumnCount(rangeId);
                return reference.GetValue(rangeId, i / num2, i % num2);
            }
            return o;
        }

        public static object GetValue(object o, int row, int column, short rangeId = 0)
        {
            if (o is CalcArray)
            {
                CalcArray array = (CalcArray) o;
                return array.GetValue(row, column);
            }
            if (o is CalcReference)
            {
                CalcReference reference = (CalcReference) o;
                return reference.GetValue(rangeId, row, column);
            }
            return o;
        }

        public static bool IsArrayOrRange(object o)
        {
            return ((o is CalcArray) || (o is CalcReference));
        }

        public static bool IsSubtotal(object o, int i, short rangeId = 0)
        {
            if (o is CalcReference)
            {
                CalcReference reference = (CalcReference) o;
                int columnCount = reference.GetColumnCount(rangeId);
                return reference.IsSubtotal(rangeId, i / columnCount, i % columnCount);
            }
            return false;
        }

        public static bool IsSubtotal(object o, int row, int column, short rangeId = 0)
        {
            if (o is CalcReference)
            {
                CalcReference reference = (CalcReference) o;
                return reference.IsSubtotal(rangeId, row, column);
            }
            return false;
        }
    }
}

