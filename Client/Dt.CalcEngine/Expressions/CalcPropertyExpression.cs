#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents an expression with a property name as the expression.
    /// </summary>
    public class CalcPropertyExpression : CalcExpression
    {
        private string _name;
        private object _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcPropertyExpression" /> 
        /// class with the property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        public CalcPropertyExpression(string name) : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcPropertyExpression" /> 
        /// class with the property name and the object that declares the property.
        /// </summary>
        /// <param name="source">The object that declares the property.</param>
        /// <param name="name">The property name.</param>
        public CalcPropertyExpression(object source, string name)
        {
            base.ThrowIfArgumentNull<string>(name, "name");
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
            return new CalcPropertyExpression(this.Source, this.Name);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents the name of the property.
        /// </value>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// Gets the object that declares the property.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Object" /> value that declares the property.
        /// </value>
        public object Source
        {
            get
            {
                return this._source;
            }
        }
    }
}

