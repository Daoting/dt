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
    /// <summary>
    /// Represents a custom named expression that can be used by formulas.
    /// </summary>
    public class NameInfo
    {
        int baseColumn;
        int baseRow;
        CalcExpression expression;
        string name;

        /// <summary>
        /// Constructs a new custom named expression.
        /// </summary>
        /// <param name="name">The name of the custom named expression.</param>
        /// <param name="baseRow">The base row index of the custom named expression.</param>
        /// <param name="baseColumn">The base column index of the custom named expression.</param>
        /// <param name="expression">The calculate expression that is used to customize the named expression.</param>
        public NameInfo(string name, int baseRow, int baseColumn, CalcExpression expression)
        {
            this.name = name;
            this.baseRow = baseRow;
            this.baseColumn = baseColumn;
            this.expression = expression;
        }

        /// <summary>
        /// Gets the base column of the custom named expression.
        /// </summary>
        /// <value>The base column.</value>
        public int BaseColumn
        {
            get { return  this.baseColumn; }
        }

        /// <summary>
        /// Gets the base row of the custom named expression.
        /// </summary>
        /// <value>The base row.</value>
        public int BaseRow
        {
            get { return  this.baseRow; }
        }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public CalcExpression Expression
        {
            get { return  this.expression; }
            internal set { this.expression = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name string.</value>
        public string Name
        {
            get { return  this.name; }
        }
    }
}

