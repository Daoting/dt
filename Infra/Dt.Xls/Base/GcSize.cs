#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Implements a structure that is used to describe the Dt.Xls.GcSize of an object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GcSize
    {
        internal double _width;
        internal double _height;
        internal static readonly GcSize _empty;
        static GcSize()
        {
            GcSize size = new GcSize {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };
            _empty = size;
        }

        /// <summary>
        /// Initializes a new instance of the GrepaCity.Excel.GcSize structure and assigns it an initial with and height
        /// </summary>
        /// <param name="width">The initial width of the instance of Dt.Xls.GcSize</param>
        /// <param name="height">The initial height of the instance of GrapeCity.Ecel.SGcSizeize</param>
        public GcSize(double width, double height)
        {
            if ((width < 0.0) || (height < 0.0))
            {
                throw new InvalidOperationException(ResourceHelper.GetResourceString("invalidHeightOrWidthError"));
            }
            this._width = width;
            this._height = height;
        }

        /// <summary>
        /// Gets a value that indicates whether this instance of Dt.Xls.Size is Dt.Xls.GcSize.Empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return  (this._width < 0.0); }
        }
        /// <summary>
        /// Gets a value that represents a static empty Dt.Xls.GcSize
        /// </summary>
        public static GcSize Empty
        {
            get { return  _empty; }
        }
        /// <summary>
        /// Gets or sets the height of this instance of Dt.Xls.GcSize
        /// </summary>
        public double Height
        {
            get { return  this._height; }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("gcSizeEmptyError"));
                }
                if (value < 0.0)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("invalidHeightOrWidthError"));
                }
                this._height = value;
            }
        }
        /// <summary>
        /// Gets or sets the width of this instance of the Dt.Xls.GcSize
        /// </summary>
        public double Width
        {
            get { return  this._width; }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("gcSizeEmptyError"));
                }
                if (value < 0.0)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("invalidHeightOrWidthError"));
                }
                this._width = value;
            }
        }
        /// <summary>
        /// Compares two instance of Dt.Xls.GcSize for equality
        /// </summary>
        /// <param name="size1">A Dt.Xls.GcSize to compare</param>
        /// <param name="size2">A Dt.Xls.GcSize to compare</param>
        /// <returns>true if the two instance of Dt.Xls.GcSize are equal; otherwise, false.</returns>
        public static bool operator ==(GcSize size1, GcSize size2)
        {
            return ((size1.Width == size2.Width) && (size1.Height == size2.Height));
        }

        /// <summary>
        /// Compares two instance of Dt.Xls.GcSize for inequality
        /// </summary>
        /// <param name="size1">A Dt.Xls.GcSize to compare</param>
        /// <param name="size2">A Dt.Xls.GcSize to compare</param>
        /// <returns>true if the two instance of Dt.Xls.GcSize are not equal; otherwise, false.</returns>
        public static bool operator !=(GcSize size1, GcSize size2)
        {
            return !(size1 == size2);
        }

        /// <summary>
        /// Compares two instance of Dt.Xls.GcSize for equality
        /// </summary>
        /// <param name="size1">A Dt.Xls.GcSize to compare</param>
        /// <param name="size2">A Dt.Xls.GcSize to compare</param>
        /// <returns>true if the two instance of Dt.Xls.GcSize are equal; otherwise, false.</returns>
        public static bool Equals(GcSize size1, GcSize size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            return (((double) size1.Width).Equals(size2.Width) && ((double) size1.Height).Equals(size2.Height));
        }

        /// <summary>
        /// Compares an object to an instance of Dt.Xls.GcSize for equality
        /// </summary>
        /// <param name="obj">The System.Object to compare.</param>
        /// <returns>true is the sizes are equal; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is GcSize))
            {
                return false;
            }
            GcSize size = (GcSize) obj;
            return Equals(this, size);
        }

        /// <summary>
        /// Gets the hash code for this instance of Dt.Xls.GcSize
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            return (((double) this.Width).GetHashCode() ^ ((double) this.Height).GetHashCode());
        }
    }
}

