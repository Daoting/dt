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
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    internal class BinaryCompositeConcreteReference : ConcreteReference
    {
        public BinaryCompositeConcreteReference(CalcReference source, int row, int column, int rowCount, int columnCount, Func<object, object, object> action, object operand1, bool switchOrder = false) : base(source, row, column, rowCount, columnCount)
        {
            this.Action = action;
            this.Operand1 = operand1;
            this.SwithOrder = switchOrder;
        }

        public override object GetValue(int area, int rowOffset, int columnOffset)
        {
            if (!this.SwithOrder)
            {
                return this.Action(base.GetActualValue(area, rowOffset, columnOffset), this.Operand1);
            }
            return this.Action(this.Operand1, base.GetActualValue(area, rowOffset, columnOffset));
        }

        public Func<object, object, object> Action { get; private set; }

        public object Operand1 { get; private set; }

        public bool SwithOrder { get; private set; }
    }
}

