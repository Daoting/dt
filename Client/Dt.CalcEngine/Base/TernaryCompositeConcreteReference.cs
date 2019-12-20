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
    internal class TernaryCompositeConcreteReference : ConcreteReference
    {
        public TernaryCompositeConcreteReference(CalcReference source, int row, int column, int rowCount, int columnCount, Func<object, object, object, object> action, object operand1, object operand2) : base(source, row, column, rowCount, columnCount)
        {
            this.Action = action;
            this.Operand1 = operand1;
            this.Operand2 = operand2;
        }

        public override object GetValue(int area, int rowOffset, int columnOffset)
        {
            return this.Action(base.GetActualValue(area, rowOffset, columnOffset), this.Operand1, this.Operand2);
        }

        public Func<object, object, object, object> Action { get; private set; }

        public object Operand1 { get; private set; }

        public object Operand2 { get; private set; }
    }
}

