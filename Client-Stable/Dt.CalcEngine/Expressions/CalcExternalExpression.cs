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
    /// Represents a cell reference expression.
    /// </summary>
    public abstract class CalcExternalExpression : CalcReferenceExpression
    {
        private ICalcSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcExternalCellExpression" /> class.
        /// </summary>
        /// <param name="source">The owner of cell.</param>
        public CalcExternalExpression(ICalcSource source)
        {
            this._source = source;
        }

        /// <summary>
        /// Gets the owner of current cell.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.ICalcSource" /> value that represents the owner of current cell.
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

