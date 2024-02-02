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
#endregion

namespace Dt.Cells.Data
{
    internal class ObjectCalcArray : CalcArray
    {
        object[,] values;

        public ObjectCalcArray(object[,] values)
        {
            this.values = values;
        }

        public override object GetValue(int row, int column)
        {
            return this.values[row, column];
        }

        public override int ColumnCount
        {
            get { return  this.values.GetLength(1); }
        }

        public override int RowCount
        {
            get { return  this.values.GetLength(0); }
        }
    }
}

