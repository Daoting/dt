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
    internal class UnaryCompositeConcreteReference : ConcreteReference
    {
        public UnaryCompositeConcreteReference(CalcReference source, int row, int column, int rowCount, int columnCount, Func<object, object> action) : base(source, row, column, rowCount, columnCount)
        {
            this.Action = action;
        }

        public override object GetValue(int area, int rowOffset, int columnOffset)
        {
            return this.Action(base.GetActualValue(area, rowOffset, columnOffset));
        }

        public Func<object, object> Action { get; private set; }
    }
}

