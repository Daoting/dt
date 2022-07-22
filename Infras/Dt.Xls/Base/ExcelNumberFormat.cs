#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements <see cref="T:Dt.Xls.IExcelNumberFormat" /> represents a number format settings
    /// </summary>
    public class ExcelNumberFormat : IExcelNumberFormat, IEquatable<IExcelNumberFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelNumberFormat" /> class.
        /// </summary>
        /// <param name="id">The id of the number format</param>
        /// <param name="code">The format code of the number format</param>
        public ExcelNumberFormat(int id, string code)
        {
            this.NumberFormatId = id;
            this.NumberFormatCode = code;
        }

        /// <summary>
        /// Determines whether the current instance is equals to the specific <see cref="T:Dt.Xls.IExcelNumberFormat" /> instance.
        /// </summary>
        /// <param name="other">The other <see cref="T:Dt.Xls.IExcelNumberFormat" /> used to compared with the current object..</param>
        /// <returns>True if the two object are equals.</returns>
        public bool Equals(IExcelNumberFormat other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            return ((this.NumberFormatId == other.NumberFormatId) && (this.NumberFormatCode == other.NumberFormatCode));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = ((int) this.NumberFormatId).GetHashCode();
            if (this.NumberFormatCode != null)
            {
                hashCode ^= this.NumberFormatCode.GetHashCode() << 7;
            }
            return hashCode;
        }

        /// <summary>
        /// Gets the number format code.
        /// </summary>
        /// <value>The number format code.</value>
        public string NumberFormatCode { get; private set; }

        /// <summary>
        /// Gets the number format id.
        /// </summary>
        /// <value>The number format id.</value>
        public int NumberFormatId { get; private set; }
    }
}

