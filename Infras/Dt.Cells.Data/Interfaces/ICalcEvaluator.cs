#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an object that can evaluate an expression or a function.
    /// </summary>
    public interface ICalcEvaluator
    {
        /// <summary>
        /// Creates an expression from a function name.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The expression.</returns>
        object CreateExpression(string functionName, params object[] parameters);
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="columnOffset">The column offset.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="isArrayFormula">if set to <c>true</c> the expression is an array formula.</param>
        /// <returns>The result of the specified expression.</returns>
        object EvaluateExpression(object expression, int rowOffset, int columnOffset, int baseRow, int baseColumn, bool isArrayFormula);
        /// <summary>
        /// Evaluates the function.
        /// </summary>
        /// <param name="function">The function object.</param>
        /// <param name="parameters">The parameters which the function needs.</param>
        /// <returns>The result of the specified function.</returns>
        object EvaluateFunction(object function, params object[] parameters);
        /// <summary>
        /// Converts an expression to a formula.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <returns>The formula.</returns>
        string Expression2Formula(object expression, int baseRow, int baseColumn);
        /// <summary>
        /// Converts a formula to an expression.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <returns>The expression.</returns>
        object Formula2Expression(string formula, int baseRow, int baseColumn);
        /// <summary>
        /// Determines whether the specified value is a calculation error.
        /// </summary>
        /// <param name="value">The specified value.</param>
        /// <returns><c>true</c> if  the specified value is a calculation error; otherwise, <c>false</c>.</returns>
        bool IsCalcError(object value);
    }
}

