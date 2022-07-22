#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent a source, usually it bind to a worksheet, however, you can setup a group elements and treat as a source.
    /// </summary>
    public interface ICalcSource : IEqualityComparer<ICalcSource>
    {
        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <returns></returns>
        int GetColumnCount();

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <returns></returns>
        int GetRowCount();

        /// <summary>
        /// Gets an expression for the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="column">the base row</param>
        /// <param name="row">the base column</param>
        /// <returns></returns>
        CalcExpression GetDefinedName(string name, int row, int column);
        /// <summary>
        /// Gets the evaluator context.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <returns></returns>
        CalcEvaluatorContext GetEvaluatorContext(CalcLocalIdentity baseAddress);
        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        CalcFunction GetFunction(string functionName);
        /// <summary>
        /// Gets the parser context.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <returns></returns>
        CalcParserContext GetParserContext(CalcLocalIdentity baseAddress);
        /// <summary>
        /// Gets the reference.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        object GetReference(CalcLocalIdentity id);
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        object GetValue(CalcLocalIdentity id);
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="value">The value.</param>
        void SetValue(CalcLocalIdentity id, object value);
    }
}

