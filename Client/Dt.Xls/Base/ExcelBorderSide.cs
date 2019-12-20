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
    /// Represent the border line properties.
    /// </summary>
    public class ExcelBorderSide : IExcelBorderSide, IEquatable<IExcelBorderSide>
    {
        /// <summary>
        /// Equals the specified other.
        /// </summary>
        /// <param name="other">The other <see cref="T:Dt.Xls.IExcelBorderSide" /> used to compared with the current instance</param>
        /// <returns></returns>
        public bool Equals(IExcelBorderSide other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            if (this.Color == null)
            {
                if (other.Color != null)
                {
                    return false;
                }
                return (this.LineStyle == other.LineStyle);
            }
            return (this.Color.Equals(other.Color) && (this.LineStyle == other.LineStyle));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise,  <see langword="false" />
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as IExcelBorderSide);
        }

        /// <summary>
        /// Returns a hash code for the current type.
        /// </summary>
        /// <returns>An integer representing a hash code.</returns>
        public override int GetHashCode()
        {
            if (this.Color != null)
            {
                return (this.Color.GetHashCode() ^ (((ExcelBorderStyle) this.LineStyle).GetHashCode() << 8));
            }
            return this.LineStyle.GetHashCode();
        }

        /// <summary>
        /// Gets or sets the color of the border line
        /// </summary>
        /// <value>The color of the border line</value>
        public IExcelColor Color { get; set; }

        /// <summary>
        /// Gets or sets the border line style.
        /// </summary>
        /// <value>The border line style.</value>
        public ExcelBorderStyle LineStyle { get; set; }
    }
}

