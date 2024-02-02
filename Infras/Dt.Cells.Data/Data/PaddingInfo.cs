#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents padding information, in hundredths of an inch. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PaddingInfo
    {
        bool all;
        int top;
        int left;
        int right;
        int bottom;
        public static readonly PaddingInfo Empty;
        /// <summary>
        /// Gets or sets all padding information, in hundredths of an inch.
        /// </summary>
        /// <value><c>-1</c> if all parts are not equal.</value>
        public int All
        {
            get
            {
                if (!this.all)
                {
                    return -1;
                }
                return this.top;
            }
            set
            {
                if (!this.all || (this.top != value))
                {
                    this.all = true;
                    this.top = this.left = this.right = this.bottom = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the bottom padding information, in hundredths of an inch.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom
        {
            get
            {
                if (this.all)
                {
                    return this.top;
                }
                return this.bottom;
            }
            set
            {
                if (this.all || (this.bottom != value))
                {
                    this.all = false;
                    this.bottom = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the left padding information, in hundredths of an inch.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get
            {
                if (this.all)
                {
                    return this.top;
                }
                return this.left;
            }
            set
            {
                if (this.all || (this.left != value))
                {
                    this.all = false;
                    this.left = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the right padding information, in hundredths of an inch.
        /// </summary>
        /// <value>The right.</value>
        public int Right
        {
            get
            {
                if (this.all)
                {
                    return this.top;
                }
                return this.right;
            }
            set
            {
                if (this.all || (this.right != value))
                {
                    this.all = false;
                    this.right = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the top padding information, in hundredths of an inch.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get { return  this.top; }
            set
            {
                if (this.all || (this.top != value))
                {
                    this.all = false;
                    this.top = value;
                }
            }
        }
        /// <summary>
        /// Gets the horizontal size, in hundredths of an inch.
        /// </summary>
        /// <value>The horizontal size.</value>
        public int Horizontal
        {
            get { return  (this.Left + this.Right); }
        }
        /// <summary>
        /// Gets the vertical size, in hundredths of an inch.
        /// </summary>
        /// <value>The vertical size.</value>
        public int Vertical
        {
            get { return  (this.Top + this.Bottom); }
        }
        /// <summary>
        /// Gets the size, in hundredths of an inch.
        /// </summary>
        /// <value>The size.</value>
        public Windows.Foundation.Size Size
        {
            get { return  new Windows.Foundation.Size((double) this.Horizontal, (double) this.Vertical); }
        }
        /// <summary>
        /// Gets a value that indicates whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return  ((((this.top == 0) && (this.right == 0)) && (this.left == 0)) && (this.bottom == 0)); }
        }
        /// <summary>
        /// Creates a new set of padding information with the specified setting.
        /// </summary>
        /// <param name="all">The size of all padding information parts, in hundredths of an inch.</param>
        public PaddingInfo(int all)
        {
            this.all = true;
            this.top = this.left = this.right = this.bottom = all;
        }

        /// <summary>
        /// Creates a new set of padding information with the specified left, top, right, and bottom settings.
        /// </summary>
        /// <param name="left">The left padding information, in hundredths of an inch.</param>
        /// <param name="top">The top padding information, in hundredths of an inch.</param>
        /// <param name="right">The right padding information, in hundredths of an inch.</param>
        /// <param name="bottom">The bottom padding information, in hundredths of an inch.</param>
        public PaddingInfo(int left, int top, int right, int bottom)
        {
            this.top = top;
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.all = ((this.top == this.left) && (this.top == this.right)) && (this.top == this.bottom);
        }

        /// <summary>
        /// Creates a new set of padding information.
        /// </summary>
        static PaddingInfo()
        {
            Empty = new PaddingInfo(0);
        }

        /// <summary>
        /// Adds the specified <see cref="T:Dt.Cells.Data.PaddingInfo" /> objects.
        /// </summary>
        /// <param name="p1">The first padding information object.</param>
        /// <param name="p2">The second padding information object.</param>
        /// <returns>A new PaddingInfo object that is the result of adding the two specified objects.</returns>
        public static PaddingInfo Add(PaddingInfo p1, PaddingInfo p2)
        {
            return (p1 + p2);
        }

        /// <summary>
        /// Subtracts the specified <see cref="T:Dt.Cells.Data.PaddingInfo" /> object.
        /// </summary>
        /// <param name="p1">The first padding information object.</param>
        /// <param name="p2">The second padding information object.</param>
        /// <returns>A new PaddingInfo object that is the result of subtracting the second object from the first.</returns>
        public static PaddingInfo Subtract(PaddingInfo p1, PaddingInfo p2)
        {
            return (p1 - p2);
        }

        /// <summary>
        /// Returns whether the current PaddingInfo object equals another specified object.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            if (other.GetType() != typeof(PaddingInfo))
            {
                return false;
            }
            return this.Equals((PaddingInfo) other);
        }

        /// <summary>
        /// Implements the plus operator.
        /// </summary>
        /// <param name="p1">The first padding information object.</param>
        /// <param name="p2">The second padding information object.</param>
        /// <returns>The result of the operator.</returns>
        public static PaddingInfo operator +(PaddingInfo p1, PaddingInfo p2)
        {
            return new PaddingInfo(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }

        /// <summary>
        /// Implements the minus operator.
        /// </summary>
        /// <param name="p1">The first padding information object.</param>
        /// <param name="p2">The second padding information object.</param>
        /// <returns>The result of the operator.</returns>
        public static PaddingInfo operator -(PaddingInfo p1, PaddingInfo p2)
        {
            return new PaddingInfo(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
        }

        /// <summary>
        /// Implements the equal equal operator.
        /// </summary>
        /// <param name="p1">The first padding information object.</param>
        /// <param name="p2">The second padding information object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PaddingInfo p1, PaddingInfo p2)
        {
            return ((((p1.Left == p2.Left) && (p1.Top == p2.Top)) && (p1.Right == p2.Right)) && (p1.Bottom == p2.Bottom));
        }

        /// <summary>
        /// Implements the not equal operator.
        /// </summary>
        /// <param name="p1">The first padding information object.</param>
        /// <param name="p2">The second padding information object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(PaddingInfo p1, PaddingInfo p2)
        {
            return !(p1 == p2);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            int num = (this.all.GetHashCode() * 0x18d) ^ this.top;
            num = (num * 0x18d) ^ this.left;
            num = (num * 0x18d) ^ this.right;
            return ((num * 0x18d) ^ this.bottom);
        }

        /// <summary>
        /// Returns whether the PaddingInfo object equals the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(PaddingInfo obj)
        {
            return (((obj.all.Equals(this.all) && (obj.top == this.top)) && ((obj.left == this.left) && (obj.right == this.right))) && (obj.bottom == this.bottom));
        }

        /// <summary>
        /// Scales the specified dx.
        /// </summary>
        /// <param name="dx">The dx</param>
        /// <param name="dy">The dy</param>
        internal void Scale(float dx, float dy)
        {
            this.top = (int) (this.top * dy);
            this.left = (int)(this.left * dx);
            this.right = (int)(this.right * dx);
            this.bottom = (int)(this.bottom * dy);
        }

        /// <summary>
        /// Internal only.
        /// Should the serialize all.
        /// </summary>
        /// <returns></returns>
        internal bool ShouldSerializeAll()
        {
            return this.all;
        }
    }
}

