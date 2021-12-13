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
    internal class PositionHelper
    {
        public static int GetHashCode(DataSheetElementType type, int row, int column)
        {
            int num = (int) type;
            return ((((num << 0x1c) | (num >> 4)) ^ ((column << 0x12) | (column >> 14))) ^ row);
        }
    }
}

