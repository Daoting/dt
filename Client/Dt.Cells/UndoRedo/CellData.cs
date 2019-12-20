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
        private int row;
        private int column;
        private object value;
        public CellData(int row, int column, object value)
        {
            this.row = row;
            this.column = column;
            this.value = value;
        }

        public int Row
        {
            get { return  this.row; }
        }
        public int Column
        {
            get { return  this.column; }
        }
        public object Value
        {
            get { return  this.value; }
        }
    }
}

