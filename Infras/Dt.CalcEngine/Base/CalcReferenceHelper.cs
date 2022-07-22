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
    internal static class CalcReferenceHelper
    {
        internal static void Id2Range(ICalcSource source, CalcIdentity Id, out int row, out int col, out int rowCount, out int colCount, out bool searchRange)
        {
            int num;
            int num2;
            colCount = num = 1;
            rowCount = num2 = num;
            row = col = num2;
            searchRange = false;
            CalcCellIdentity objA = Id as CalcCellIdentity;
            CalcRangeIdentity identity2 = Id as CalcRangeIdentity;
            if (!object.ReferenceEquals(objA, null))
            {
                row = objA.RowIndex;
                col = objA.ColumnIndex;
                rowCount = colCount = 1;
                searchRange = true;
            }
            else if (!object.ReferenceEquals(identity2, null))
            {
                if (identity2.IsFullColumn || identity2.IsFullRow)
                {
                    CalcReference reference = source.GetReference(identity2) as CalcReference;
                    row = reference.GetRow(0);
                    col = reference.GetColumn(0);
                    rowCount = reference.GetRowCount(0);
                    colCount = reference.GetColumnCount(0);
                }
                if (identity2.IsFullRow)
                {
                    row = identity2.RowIndex;
                    rowCount = identity2.RowCount;
                }
                else if (identity2.IsFullColumn)
                {
                    col = identity2.ColumnIndex;
                    colCount = identity2.ColumnCount;
                }
                else
                {
                    row = identity2.RowIndex;
                    rowCount = identity2.RowCount;
                    col = identity2.ColumnIndex;
                    colCount = identity2.ColumnCount;
                }
                searchRange = true;
            }
            else
            {
                int num5;
                int num6;
                colCount = num5 = 0;
                rowCount = num6 = num5;
                row = col = num6;
            }
        }
    }
}

