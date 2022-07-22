#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.CalcEngine
{
    internal class CellInfoReference : ConcreteReference
    {
        public CellInfoReference(CalcReference source, int row, int column, int rowCount, int columnCount, CellInfoType type) : base(source, row, column, rowCount, columnCount)
        {
            this.Type = type;
        }

        public override object GetValue(int area, int rowOffset, int columnOffset)
        {
            switch (this.Type)
            {
                case CellInfoType.Row:
                    return (int) ((this.GetRow(0) + 1) + rowOffset);

                case CellInfoType.Column:
                    return (int) ((this.GetColumn(0) + 1) + columnOffset);
            }
            return base.GetActualValue(area, rowOffset, columnOffset);
        }

        public CellInfoType Type { get; private set; }

        public enum CellInfoType
        {
            Row,
            Column
        }
    }
}

