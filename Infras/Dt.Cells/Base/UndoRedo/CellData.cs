#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UndoRedo
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CellData
    {
        public CellData(int row, int column, object value)
        {
            Row = row;
            Column = column;
            Value = value;
        }

        public int Row { get; }

        public int Column { get; }

        public object Value { get; }
    }
}

