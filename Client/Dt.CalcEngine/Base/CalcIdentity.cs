#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Provides a data address.
    /// </summary>
    public abstract class CalcIdentity : IEqualityComparer<CalcIdentity>
    {
        protected CalcIdentity()
        {
        }

        internal static bool Compare(CalcIdentity left, CalcIdentity right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }
            if (!object.ReferenceEquals(left, null))
            {
                return left.CompareTo(right);
            }
            return object.ReferenceEquals(right, null);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> 
        /// is equal to the current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />.
        /// </summary>
        /// <param name="other">
        /// The <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> to compare with the
        /// current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />. 
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> 
        /// is equal to the current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        protected virtual bool CompareTo(CalcIdentity other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            return object.ReferenceEquals(this, other);
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected virtual int ComputeHash()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <see langword="false" />.
        /// </returns>
        public sealed override bool Equals(object obj)
        {
            return this.CompareTo(obj as CalcIdentity);
        }

        internal static string GetCoord(int rowIndex, int columnIndex)
        {
            StringBuilder sb = new StringBuilder();
            CalcParser.UnParseCell(false, 0, 0, rowIndex, columnIndex, true, true, sb, CalcRangeType.Cell);
            return sb.ToString();
        }

        internal static string GetCoord(int rowIndex, int columnIndex, int rowCount, int colCount, CalcRangeType rangeType)
        {
            StringBuilder sb = new StringBuilder();
            CalcParser.UnParseRange(false, 0, 0, rowIndex, columnIndex, (rowIndex + rowCount) - 1, (columnIndex + colCount) - 1, true, true, true, true, sb, rangeType);
            return sb.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public sealed override int GetHashCode()
        {
            return this.ComputeHash();
        }

        /// <summary>
        /// Tests whether two identity structures are equal.
        /// </summary>
        /// <param name="left">
        /// The identity on the left side of the equality operator.
        /// </param>
        /// <param name="right">
        /// The identity on the right side of the equality operator.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if two identity are equal; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(CalcIdentity left, CalcIdentity right)
        {
            return Compare(left, right);
        }

        /// <summary>
        /// Tests whether two identity structures are different.
        /// </summary>
        /// <param name="left">
        /// The identity on the left side of the inequality operator.
        /// </param>
        /// <param name="right">
        /// The identity on the right side of the inequality operator.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if two identity are different; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator !=(CalcIdentity left, CalcIdentity right)
        {
            return !Compare(left, right);
        }

        bool IEqualityComparer<CalcIdentity>.Equals(CalcIdentity x, CalcIdentity y)
        {
            return Compare(x, y);
        }

        int IEqualityComparer<CalcIdentity>.GetHashCode(CalcIdentity obj)
        {
            if (!object.ReferenceEquals(obj, null))
            {
                return obj.ComputeHash();
            }
            return 0;
        }
    }
}

