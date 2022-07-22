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
    /// Represents an expression with a named variable as the expression.
    /// </summary>
    public class CalcExternalNameExpression : CalcExpression
    {
        private string _name;
        private ICalcSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcExternalNameExpression" /> 
        /// class with a owner and a named variable.
        /// </summary>
        /// <param name="source">The owner which contains the named variable.</param>
        /// <param name="name">Named variable.</param>
        public CalcExternalNameExpression(ICalcSource source, string name)
        {
            base.ThrowIfArgumentNull<ICalcSource>(source, "source");
            this._source = source;
            this._name = name;
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
            return new CalcExternalNameExpression(this.Source, this.Name);
        }

        /// <summary>
        /// Gets the text representation of the named variable.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents the text representation of the named variable.
        /// </value>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// Gets the owner which contains the named variable.
        /// </summary>
        /// <value>owner which contains the named variable.</value>
        public ICalcSource Source
        {
            get
            {
                return this._source;
            }
        }
    }
}

