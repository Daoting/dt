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
    /// Represents an array constant value.
    /// </summary>
    public class CalcArrayExpression : CalcConstantExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcArrayExpression" /> class.
        /// </summary>
        /// <param name="values">The array values.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="values" /> is <see langword="null" />.</exception>
        public CalcArrayExpression(object[,] values) : base(new ConcreteArray<object>(values))
        {
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
            object[,] values = new object[this.ArrayValue.RowCount, this.ArrayValue.ColumnCount];
            for (int i = 0; i < this.ArrayValue.RowCount; i++)
            {
                for (int j = 0; j < this.ArrayValue.ColumnCount; j++)
                {
                    values[i, j] = this.ArrayValue.GetValue(i, j);
                }
            }
            return new CalcArrayExpression(values);
        }

        /// <summary>
        /// Gets the value of the array expression.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.CalcEngine.CalcArray" /> equal 
        /// to the value of the represented expression.
        /// </value>
        public CalcArray ArrayValue
        {
            get
            {
                return (CalcArray) base.Value;
            }
        }
    }
}

