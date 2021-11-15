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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal abstract class OperatorExpressionVisistorBase : ExpressionVisitor
    {
        public OperatorExpressionVisistorBase(ICalcSource currentCalcSource)
        {
            this.CurrentCalcSource = currentCalcSource;
        }

        public ICalcSource CurrentCalcSource { get; set; }

        public bool OffsetExternalOnly { get; set; }
    }
}

