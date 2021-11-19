#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.CalcEngine
{
    internal class ConcreteArray<T> : CalcArray
    {
        private T[,] _values;

        public ConcreteArray(T[,] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            this._values = (T[,]) values.Clone();
        }

        public override object GetValue(int row, int column)
        {
            return this._values[row, column];
        }

        public override int ColumnCount
        {
            get
            {
                return this._values.GetLength(1);
            }
        }

        public override int RowCount
        {
            get
            {
                return this._values.GetLength(0);
            }
        }
    }
}

