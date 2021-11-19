#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class NoneVisitor : Dt.Cells.Data.ExpressionVisitor
    {
        public override CalcExpression Visit(CalcExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }
    }
}

