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

namespace Dt.CalcEngine.Operators
{
    /// <summary>
    /// Represents an operator. This is an abstract class.
    /// </summary>
    public abstract class CalcOperator
    {
        protected CalcOperator()
        {
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" />
        /// is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> is 
        /// equal to the current <see cref="T:System.Object" />;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }
            return (base.GetType() == obj.GetType());
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetType().GetHashCode();
        }

        /// <summary>
        /// Tests whether two operator structures are equal.
        /// </summary>
        /// <param name="left">
        /// The operator on the left side of the equality operator.
        /// </param>
        /// <param name="right">
        /// The operator on the right side of the equality operator.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if two operator are equal; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(CalcOperator left, CalcOperator right)
        {
            return (object.ReferenceEquals(left, right) || ((!object.ReferenceEquals(left, null) && !object.ReferenceEquals(right, null)) && left.Equals(right)));
        }

        /// <summary>
        /// Tests whether two operator structures are different.
        /// </summary>
        /// <param name="left">
        /// The operator on the left side of the inequality operator.
        /// </param>
        /// <param name="right">
        /// The operator on the right side of the inequality operator.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if two operator are different; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator !=(CalcOperator left, CalcOperator right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current operator.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents the current operator
        /// </value>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Gets the name of the operator.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents the name of the operator.
        /// </value>
        public abstract string Name { get; }

        internal class OperatorArray : CalcArray
        {
            private object[,] values;

            public OperatorArray(object[,] values)
            {
                this.values = values;
            }

            public override object GetValue(int row, int column)
            {
                return this.values[row, column];
            }

            public override int ColumnCount
            {
                get
                {
                    return this.values.GetLength(1);
                }
            }

            public override int RowCount
            {
                get
                {
                    return this.values.GetLength(0);
                }
            }
        }
    }
}

