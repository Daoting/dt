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
    internal abstract class ExpressionVisitor
    {
        protected ExpressionVisitor()
        {
        }

        public virtual CalcExpression Visit(CalcExpression expr, int baseRow, int baseColumn)
        {
            CalcConstantExpression expression = expr as CalcConstantExpression;
            if (expression != null)
            {
                return this.VisitConstantExpression(expression);
            }
            CalcCellExpression expression2 = expr as CalcCellExpression;
            if (expression2 != null)
            {
                return this.VisitCellExpression(expression2, baseRow, baseColumn);
            }
            CalcRangeExpression expression3 = expr as CalcRangeExpression;
            if (expression3 != null)
            {
                return this.VisitRangeExpression(expression3, baseRow, baseColumn);
            }
            CalcUnaryOperatorExpression expression4 = expr as CalcUnaryOperatorExpression;
            if (expression4 != null)
            {
                return this.VisitUnaryOperatorExpression(expression4, baseRow, baseColumn);
            }
            CalcBinaryOperatorExpression expression5 = expr as CalcBinaryOperatorExpression;
            if (expression5 != null)
            {
                return this.VisitBinaryOperatorExpression(expression5, baseRow, baseColumn);
            }
            CalcFunctionExpression expression6 = expr as CalcFunctionExpression;
            if (expression6 != null)
            {
                return this.VisitFunctionExpression(expression6, baseRow, baseColumn);
            }
            CalcExternalCellExpression expression7 = expr as CalcExternalCellExpression;
            if (expression7 != null)
            {
                return this.VisitExternalCellExpression(expression7, baseRow, baseColumn);
            }
            CalcExternalRangeExpression expression8 = expr as CalcExternalRangeExpression;
            if (expression8 != null)
            {
                return this.VisitExternalRangeExpression(expression8, baseRow, baseColumn);
            }
            if (expr is CalcSharedExpression)
            {
                CalcExpression expression9 = (expr as CalcSharedExpression).Expression;
                CalcExpression expression10 = this.Visit(expression9, baseRow, baseColumn);
                if (expression10 == expression9)
                {
                    return expr;
                }
                return new CalcSharedExpression(expression10);
            }
            CalcParenthesesExpression expression11 = expr as CalcParenthesesExpression;
            if (expression11 != null)
            {
                return this.VisitParenthesesExpression(expression11, baseRow, baseColumn);
            }
            CalcNameExpression expression12 = expr as CalcNameExpression;
            if (expression12 != null)
            {
                return this.VisitNameExpression(expression12, baseRow, baseColumn);
            }
            CalcExternalNameExpression expression13 = expr as CalcExternalNameExpression;
            if (expression13 != null)
            {
                return this.VisitExternalNameExpression(expression13, baseRow, baseColumn);
            }
            CalcSheetRangeExpression expression14 = expr as CalcSheetRangeExpression;
            if (expression14 != null)
            {
                return this.VisitSheetRangeExpression(expression14, baseRow, baseColumn);
            }
            return expr;
        }

        protected virtual CalcExpression VisitBinaryOperatorExpression(CalcBinaryOperatorExpression expr, int baseRow, int baseColumn)
        {
            CalcExpression left = expr.Left;
            CalcExpression right = expr.Right;
            CalcExpression expression3 = this.Visit(left, baseRow, baseColumn);
            CalcExpression expression4 = this.Visit(right, baseRow, baseColumn);
            if ((left == expression3) && (right == expression4))
            {
                return expr;
            }
            return new CalcBinaryOperatorExpression(expr.Operator, expression3, expression4);
        }

        protected virtual CalcExpression VisitCellExpression(CalcCellExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        protected virtual CalcExpression VisitConstantExpression(CalcConstantExpression expr)
        {
            return expr;
        }

        protected virtual CalcExpression VisitExternalCellExpression(CalcExternalCellExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        protected virtual CalcExpression VisitExternalNameExpression(CalcExternalNameExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        protected virtual CalcExpression VisitExternalRangeExpression(CalcExternalRangeExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        protected virtual CalcExpression VisitFunctionExpression(CalcFunctionExpression expr, int baseRow, int baseColumn)
        {
            int argCount = expr.ArgCount;
            CalcExpression[] args = new CalcExpression[argCount];
            bool flag = false;
            for (int i = 0; i < argCount; i++)
            {
                CalcExpression arg = expr.GetArg(i);
                args[i] = this.Visit(arg, baseRow, baseColumn);
                if (arg != args[i])
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                return expr;
            }
            if (object.ReferenceEquals(expr.Function, null))
            {
                return new CalcFunctionExpression(expr.FunctionName, args);
            }
            return new CalcFunctionExpression(expr.Function, args);
        }

        protected virtual CalcExpression VisitNameExpression(CalcNameExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        CalcExpression VisitParenthesesExpression(CalcParenthesesExpression expr, int baseRow, int baseColumn)
        {
            CalcExpression arg = expr.Arg;
            CalcExpression expression2 = this.Visit(arg, baseRow, baseColumn);
            if (arg != expression2)
            {
                return new CalcParenthesesExpression(expression2);
            }
            return expr;
        }

        protected virtual CalcExpression VisitRangeExpression(CalcRangeExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        protected virtual CalcExpression VisitSheetRangeExpression(CalcSheetRangeExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        protected virtual CalcExpression VisitUnaryOperatorExpression(CalcUnaryOperatorExpression expr, int baseRow, int baseColumn)
        {
            CalcExpression operand = expr.Operand;
            CalcExpression expression2 = this.Visit(operand, baseRow, baseColumn);
            if (operand != expression2)
            {
                return new CalcUnaryOperatorExpression(expr.Operator, expression2);
            }
            return expr;
        }
    }
}

