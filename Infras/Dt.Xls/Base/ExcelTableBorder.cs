#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent the excel table border settings
    /// </summary>
    public class ExcelTableBorder : ExcelBorder, IExcelTableBorder, IExcelBorder, IEquatable<IExcelBorder>
    {
        private IExcelBorderSide _horizontal;
        private IExcelBorderSide _vertical;

        /// <summary>
        /// Create a new <see cref="T:Dt.Xls.ExcelBorder" /> based on the current item.
        /// </summary>
        /// <returns>
        /// An <see cref="T:Dt.Xls.ExcelBorder" /> represent a cloned <see cref="T:Dt.Xls.ExcelBorder" /> instance. 
        /// </returns>
        public override IExcelBorder Clone()
        {
            ExcelTableBorder border = new ExcelTableBorder();
            ExcelBorderSide side = new ExcelBorderSide {
                Color = base.Left.Color,
                LineStyle = base.Left.LineStyle
            };
            border.Left = side;
            ExcelBorderSide side2 = new ExcelBorderSide {
                Color = base.Right.Color,
                LineStyle = base.Right.LineStyle
            };
            border.Right = side2;
            ExcelBorderSide side3 = new ExcelBorderSide {
                Color = base.Top.Color,
                LineStyle = base.Top.LineStyle
            };
            border.Top = side3;
            ExcelBorderSide side4 = new ExcelBorderSide {
                Color = base.Bottom.Color,
                LineStyle = base.Bottom.LineStyle
            };
            border.Bottom = side4;
            border.Vertical = (this.Vertical != null) ? new ExcelBorderSide() : null;
            border.Horizontal = (this.Horizontal != null) ? new ExcelBorderSide() : null;
            return border;
        }

        /// <summary>
        /// Determine whether the specified ExcelBorder object is equal to this instance
        /// </summary>
        /// <param name="other"> An <see cref="T:Dt.Xls.ExcelBorder" /> instance used to compared with the current instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:Dt.Xls.ExcelBorder" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(IExcelTableBorder other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            bool flag = ((base.Left.Equals(other.Left) && base.Right.Equals(other.Right)) && base.Top.Equals(other.Top)) && base.Bottom.Equals(other.Bottom);
            if (!flag)
            {
                return false;
            }
            if ((this.Horizontal != null) && !this.Horizontal.Equals(other.Horizontal))
            {
                return false;
            }
            if (other.Horizontal != null)
            {
                return false;
            }
            if ((this.Vertical != null) && !this.Vertical.Equals(other.Vertical))
            {
                return false;
            }
            if (other.Vertical != null)
            {
                return false;
            }
            return flag;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals((IExcelTableBorder) (obj as ExcelTableBorder));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int num = ((base.Left.GetHashCode() ^ (base.Right.GetHashCode() << 8)) ^ (base.Top.GetHashCode() << 0x10)) ^ (base.Bottom.GetHashCode() << 0x18);
            if (this.Horizontal != null)
            {
                num ^= this.Horizontal.GetHashCode() << 10;
            }
            if (this.Vertical != null)
            {
                num ^= this.Vertical.GetHashCode() << 20;
            }
            return num;
        }

        /// <summary>
        /// Gets or sets the horizontal border line
        /// </summary>
        /// <value>The horizontal border line</value>
        public IExcelBorderSide Horizontal
        {
            get { return  this._horizontal; }
            set { this._horizontal = value; }
        }

        /// <summary>
        /// Gets or sets the vertical border line
        /// </summary>
        /// <value>The vertical border line</value>
        public IExcelBorderSide Vertical
        {
            get { return  this._vertical; }
            set { this._vertical = value; }
        }
    }
}

