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
    /// <summary>
    /// Represents the <see cref="T:Dt.CalcEngine.CalcHelper" /> class.
    /// </summary>
    internal class CalcHelper
    {
        public const int MAX_ARGS = 0xff;
        public const int MAX_COLUMN_INDEX = 0x3fff;
        public const int MAX_ROW_INDEX = 0xfffff;

        public static bool ArgumentExists(object[] args, int index)
        {
            return (((args != null) && (index < args.Length)) && !(args[index] is CalcMissingArgument));
        }

        public static object GetArrayValue(CalcArray array, int row, int col)
        {
            if ((row < array.RowCount) && (col < array.ColumnCount))
            {
                return array.GetValue(row, col);
            }
            if (((col >= array.ColumnCount) && (array.ColumnCount == 1)) && (row < array.RowCount))
            {
                return array.GetValue(row, 0);
            }
            if (((row >= array.RowCount) && (array.RowCount == 1)) && (col < array.ColumnCount))
            {
                return array.GetValue(0, col);
            }
            return CalcErrors.Value;
        }

        public static int NormalizeColumnIndex(int column, int maxColumnIndex = 0x3fff)
        {
            return NormalizeIndex(column, maxColumnIndex);
        }

        private static int NormalizeIndex(int index, int maxIndex)
        {
            int num = maxIndex + 1;
            if (index < 0)
            {
                return ((index % num) + num);
            }
            if (index > maxIndex)
            {
                return (index % num);
            }
            return index;
        }

        public static int NormalizeRowIndex(int row, int maxRowIndex = 0xfffff)
        {
            return NormalizeIndex(row, maxRowIndex);
        }

        public static bool TryExtractToSingleValue(object param, out object value)
        {
            if (param is CalcReference)
            {
                CalcArray array = CalcConvert.ToArray(param as CalcReference);
                if ((array.RowCount == 1) && (array.ColumnCount == 1))
                {
                    param = array.GetValue(0);
                }
                else
                {
                    param = array;
                }
            }
            if (param is CalcArray)
            {
                CalcArray array2 = param as CalcArray;
                if ((array2.RowCount == 1) && (array2.ColumnCount == 1))
                {
                    param = array2.GetValue(0);
                }
            }
            value = param;
            return !(param is CalcArray);
        }
    }
}

