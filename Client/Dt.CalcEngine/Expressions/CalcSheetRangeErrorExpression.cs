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
    public class CalcSheetRangeErrorExpression : CalcErrorExpression
    {
        private ICalcSource _endSource;
        private ICalcSource _startSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcSheetRangeErrorExpression" /> class.
        /// </summary>
        /// <param name="startSource">Starting owner of error.</param>
        /// <param name="endSource">Ending owner of error.</param>
        /// <param name="value">The error value.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
        public CalcSheetRangeErrorExpression(ICalcSource startSource, ICalcSource endSource, CalcError value) : base(value)
        {
            this._startSource = startSource;
            this._endSource = endSource;
        }

        /// <summary>
        /// Gets the end owner of current error.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.ICalcSource" /> value that represents the owner of current error.
        /// </value>
        public ICalcSource EndSource
        {
            get
            {
                return this._endSource;
            }
        }

        /// <summary>
        /// Gets the start owner of current error.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.ICalcSource" /> value that represents the owner of current error.
        /// </value>
        public ICalcSource StartSource
        {
            get
            {
                return this._startSource;
            }
        }
    }
}

