#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    internal class RangeHelper
    {
        public static int GetHashCode(DataSheetElementType type, int row, int rowCount, int column, int columnCount)
        {
            return (int)(((DataSheetElementType) (((((row << 0x18) | (row >> 8)) ^ ((rowCount << 0x10) | (rowCount >> 0x10))) ^ ((column << 10) | (column >> 0x16))) ^ ((columnCount << 4) | (columnCount >> 0x1c)))) ^ type);
        }
    }
}

