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
    /// Represent the border settings used in <see cref="T:Dt.Xls.ExtendedFormat" />
    /// </summary>
    public class ExcelBorder : IExcelBorder, IEquatable<IExcelBorder>
    {
        private IExcelBorderSide _bottom = new ExcelBorderSide();
        private IExcelBorderSide _left = new ExcelBorderSide();
        private IExcelBorderSide _right = new ExcelBorderSide();
        private IExcelBorderSide _top = new ExcelBorderSide();

        /// <summary>
        /// Create a new <see cref="T:Dt.Xls.ExcelBorder" /> based on the current item.
        /// </summary>
        /// <returns>
        /// An <see cref="T:Dt.Xls.ExcelBorder" /> represent a cloned <see cref="T:Dt.Xls.ExcelBorder" /> instance. 
        /// </returns>
        public virtual IExcelBorder Clone()
        {
            ExcelBorder border = new ExcelBorder();
            ExcelBorderSide side = new ExcelBorderSide {
                Color = this.Left.Color,
                LineStyle = this.Left.LineStyle
            };
            border.Left = side;
            ExcelBorderSide side2 = new ExcelBorderSide {
                Color = this.Right.Color,
                LineStyle = this.Right.LineStyle
            };
            border.Right = side2;
            ExcelBorderSide side3 = new ExcelBorderSide {
                Color = this.Top.Color,
                LineStyle = this.Top.LineStyle
            };
            border.Top = side3;
            ExcelBorderSide side4 = new ExcelBorderSide {
                Color = this.Bottom.Color,
                LineStyle = this.Bottom.LineStyle
            };
            border.Bottom = side4;
            return border;
        }

        /// <summary>
        /// Determine whether the specified ExcelBorder object is equal to this instance
        /// </summary>
        /// <param name="other"> An <see cref="T:Dt.Xls.ExcelBorder" /> instance used to compared with the current instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:Dt.Xls.ExcelBorder" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(IExcelBorder other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            return (((this.Left.Equals(other.Left) && this.Right.Equals(other.Right)) && this.Top.Equals(other.Top)) && this.Bottom.Equals(other.Bottom));
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
            return this.Equals((IExcelBorder) (obj as ExcelBorder));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (((this.Left.GetHashCode() ^ (this.Right.GetHashCode() << 8)) ^ (this.Top.GetHashCode() << 0x10)) ^ (this.Bottom.GetHashCode() << 0x18));
        }

        /// <summary>
        /// Gets or sets the bottom border line
        /// </summary>
        /// <value>The bottom border line</value>
        public IExcelBorderSide Bottom
        {
            get { return  this._bottom; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(ResourceHelper.GetResourceString("borderSideNullError"));
                }
                this._bottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the left border line
        /// </summary>
        /// <value>The left border line</value>
        public IExcelBorderSide Left
        {
            get { return  this._left; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(ResourceHelper.GetResourceString("borderSideNullError"));
                }
                this._left = value;
            }
        }

        /// <summary>
        /// Gets or sets the right border line
        /// </summary>
        /// <value>The right border line</value>
        public IExcelBorderSide Right
        {
            get { return  this._right; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(ResourceHelper.GetResourceString("borderSideNullError"));
                }
                this._right = value;
            }
        }

        /// <summary>
        /// Gets or sets the top border line
        /// </summary>
        /// <value>The top border line</value>
        public IExcelBorderSide Top
        {
            get { return  this._top; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(ResourceHelper.GetResourceString("borderSideNullError"));
                }
                this._top = value;
            }
        }
    }
}

