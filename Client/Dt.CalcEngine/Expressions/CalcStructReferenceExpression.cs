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
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents a struct reference expression.
    /// </summary>
    public class CalcStructReferenceExpression : CalcReferenceExpression
    {
        private string _structReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcStructReferenceExpression" /> class.
        /// </summary>
        /// <param name="structReference">The struct reference string.</param>
        public CalcStructReferenceExpression(string structReference)
        {
            this._structReference = structReference;
        }

        /// <summary>
        /// Gets the identity of current expressions based on <paramref name="row" /> and <paramref name="column" />.
        /// </summary>
        /// <param name="row">The base row.</param>
        /// <param name="column">The base column.</param>
        /// <returns></returns>
        public override CalcIdentity GetId(int row, int column)
        {
            return new CalcStructReferenceIndentity(this._structReference);
        }

        /// <summary>
        /// Get a new expression with specific offset.
        /// </summary>
        /// <param name="row">the row offset</param>
        /// <param name="column">the column offset</param>
        /// <param name="offsetAbsolute"><c>true</c> if offset the absolute indexes.</param>
        /// <param name="offsetRelative"><c>true</c> if offset the relative indexes.</param>
        /// <returns>
        /// Return a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> which offset from current expression.
        /// </returns>
        public override CalcExpression Offset(int row, int column, bool offsetAbsolute = false, bool offsetRelative = true)
        {
            return new CalcStructReferenceExpression(this._structReference);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return this._structReference;
        }
    }
}

