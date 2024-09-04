#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal static class PositionExtensions
    {
        public static CompositeRange GetFixedRange(this CompositeRange comRange, int rowCount, int columnCount)
        {
            CompositeRange empty = CompositeRange.Empty;
            if ((comRange.Row != -1) && (comRange.Column != -1))
            {
                return new CompositeRange(DataSheetElementType.Cell, comRange.Row, comRange.Column, comRange.RowCount, comRange.ColumnCount);
            }
            if ((comRange.Row == -1) && (comRange.Column != -1))
            {
                return new CompositeRange(DataSheetElementType.Cell, 0, comRange.Column, rowCount, comRange.ColumnCount);
            }
            if ((comRange.Row != -1) && (comRange.Column == -1))
            {
                return new CompositeRange(DataSheetElementType.Cell, comRange.Row, 0, comRange.RowCount, columnCount);
            }
            if ((comRange.Row == -1) && (comRange.Column == -1))
            {
                empty = new CompositeRange(DataSheetElementType.Cell, 0, 0, rowCount, columnCount);
            }
            return empty;
        }
    }
}

