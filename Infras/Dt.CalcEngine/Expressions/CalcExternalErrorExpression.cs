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
    /// Represents an external error value.
    /// </summary>
    public class CalcExternalErrorExpression : CalcErrorExpression
    {
        private ICalcSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcExternalErrorExpression" /> class.
        /// </summary>
        /// <param name="source">The owner of error.</param>
        /// <param name="value">The error value.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
        public CalcExternalErrorExpression(ICalcSource source, CalcError value) : base(value)
        {
            this._source = source;
        }

        /// <summary>
        /// Gets the owner of current error.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.ICalcSource" /> value that represents the owner of current error.
        /// </value>
        public ICalcSource Source
        {
            get
            {
                return this._source;
            }
        }
    }
}

