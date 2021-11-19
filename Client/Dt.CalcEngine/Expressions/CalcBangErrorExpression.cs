#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents an error value on current sheet.
    /// </summary>
    public class CalcBangErrorExpression : CalcErrorExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcBangErrorExpression" /> class.
        /// </summary>
        /// <param name="value">The error value.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
        public CalcBangErrorExpression(CalcError value) : base(value)
        {
        }
    }
}

