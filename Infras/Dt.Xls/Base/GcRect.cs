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
    /// Stores a set of four integers that represent the location and size of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GcRect
    {
        internal double _x;
        internal double _y;
        internal double _width;
        internal double _height;
        private static readonly GcRect s_empty;
        static GcRect()
        {
            GcRect rect = new GcRect {
                _x = double.PositiveInfinity,
                _y = double.PositiveInfinity,
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };
            s_empty = rect;
        }

        /// <summary>
        /// Initializes a new instance of the Dt.Xls.GcRect class at the specified location and size.
        /// </summary>
        /// <param name="point1">A Dt.Xls.GcPoint object that represents the upper-left corner of the rectangular region</param>
        /// <param name="point2">A GrepaCityExcel.GcPoint object that represents the lower-right corner of the rectangular region</param>
        public GcRect(GcPoint point1, GcPoint point2)
        {
            this._x = Math.Min(point1._x, point2._x);
            this._y = Math.Min(point1._y, point2._y);
            this._width = Math.Max((double) (Math.Max(point1._x, point2._x) - this._x), (double) 0.0);
            this._height = Math.Max((double) (Math.Max(point1._y, point2._y) - this._y), (double) 0.0);
        }

        /// <summary>
        /// Initializes a new instance of the Dt.Xls.GcRect class with the specified location and size.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public GcRect(double x, double y, double width, double height)
        {
            if ((width < 0.0) || (height < 0.0))
            {
                throw new InvalidOperationException(ResourceHelper.GetResourceString("invalidHeightOrWidthError"));
            }
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
        }

        /// <summary>
        /// Represents a Dt.Xls.GcRect structure with its properties left uninitialized.
        /// </summary>
        public static GcRect Empty
        {
            get { return  s_empty; }
        }
        /// <summary>
        /// Gets a value that indicates whether the rectangle is the Dt.Xls.Rect is the Dt.Xls.GcRect.Empty rectangle.
        /// </summary>
        public bool IsEmpty
        {
            get { return  (this._width < 0.0); }
        }
        /// <summary>
        /// Gets or sets the position of the top-left corner of the rectangle.
        /// </summary>
        public GcPoint Location
        {
            get { return  new GcPoint(this._x, this._y); }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("gcRectEmptyError"));
                }
                this._x = value._x;
                this._y = value._y;
            }
        }
        /// <summary>
        /// A Dt.Xls.GcSize structure that specifies the width and height of the rectangle.
        /// </summary>
        public GcSize Size
        {
            get
            {
                if (this.IsEmpty)
                {
                    return GcSize.Empty;
                }
                return new GcSize(this._width, this._height);
            }
            set
            {
                if (value.IsEmpty)
                {
                    this = s_empty;
                }
                else
                {
                    if (this.IsEmpty)
                    {
                        throw new InvalidOperationException(ResourceHelper.GetResourceString("gcRectEmptyError"));
                    }
                    this._width = value._width;
                    this._height = value._height;
                }
            }
        }
        /// <summary>
        /// Gets or sets the x-axis value of the left side of the rectangle.
        /// </summary>
        public double X
        {
            get { return  this._x; }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("gcRectEmptyError"));
                }
                this._x = value;
            }
        }
        /// <summary>
        /// Gets or sets the y-axis value of the top side of the rectangle.
        /// </summary>
        public double Y
        {
            get { return  this._y; }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("gcRectEmptyError"));
                }
                this._y = value;
            }
        }
        /// <summary>
        /// A positive number that represents the width of the rectangle. The default is 0.
        /// </summary>
        public double Width
        {
            get { return  this._width; }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("gcRectEmptyError"));
                }
                if (value < 0.0)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("invalidHeightOrWidthError"));
                }
                this._width = value;
            }
        }
        /// <summary>
        /// A positive number that represents the height of the rectangle. The default is 0.
        /// </summary>
        public double Height
        {
            get { return  this._height; }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("gcRectEmptyError"));
                }
                if (value < 0.0)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("invalidHeightOrWidthError"));
                }
                this._height = value;
            }
        }
        /// <summary>
        /// Gets the x-axis value of the left side of the rectangle.
        /// </summary>
        public double Left
        {
            get { return  this._x; }
        }
        /// <summary>
        /// Gets the y-axis position of the top of the rectangle.
        /// </summary>
        public double Top
        {
            get { return  this._y; }
        }
        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle.
        /// </summary>
        public double Right
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return (this._x + this._width);
            }
        }
        /// <summary>
        /// Gets the y-axis value of the bottom of the rectangle.
        /// </summary>
        public double Bottom
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return (this._y + this._height);
            }
        }
        /// <summary>
        /// Compares two rectangles for exact equality.
        /// </summary>
        /// <param name="rect1"> The first rectangle to compare.</param>
        /// <param name="rect2"> The second rectangle to compare.</param>
        /// <returns>true if the rectangles have the same location and size; otherwise, false.</returns>
        public static bool operator ==(GcRect rect1, GcRect rect2)
        {
            return ((((rect1.X == rect2.X) && (rect1.Y == rect2.Y)) && (rect1.Width == rect2.Width)) && (rect1.Height == rect2.Height));
        }

        /// <summary>
        /// Compares two rectangles for exact inequality.
        /// </summary>
        /// <param name="rect1"> The first rectangle to compare.</param>
        /// <param name="rect2"> The second rectangle to compare.</param>
        /// <returns>true if the rectangles have the different location and size; otherwise, false.</returns>
        public static bool operator !=(GcRect rect1, GcRect rect2)
        {
            return !(rect1 == rect2);
        }

        /// <summary>
        /// Indicates whether the specified rectangles are equal.
        /// </summary>
        /// <param name="rect1"> The first rectangle to compare.</param>
        /// <param name="rect2">The second rectangle to compare.</param>
        /// <returns>true if the rectangles have the same location and size; otherwise, false.</returns>
        public static bool Equals(GcRect rect1, GcRect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            return (((((double) rect1.X).Equals(rect2.X) && ((double) rect1.Y).Equals(rect2.Y)) && ((double) rect1.Width).Equals(rect2.Width)) && ((double) rect1.Height).Equals(rect2.Height));
        }

        /// <summary>
        /// Indicates whether the specified rectangle is equal to the current rectangle.
        /// </summary>
        /// <param name="o"> The object to compare to the current rectangle..</param>
        /// <returns>true if the rectangles have the same location and size; otherwise, false.</returns>
        public override bool Equals(object o)
        {
            if ((o == null) || !(o is GcRect))
            {
                return false;
            }
            GcRect rect = (GcRect) o;
            return Equals(this, rect);
        }

        /// <summary>
        /// Indicates whether the specified rectangle is equal to the current rectangle.
        /// </summary>
        /// <param name="value"> The rectangle to compare to the current rectangle.</param>
        /// <returns>true if the rectangles have the same location and size; otherwise, false.</returns>
        public bool Equals(GcRect value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Creates a hash code for the rectangle.
        /// </summary>
        /// <returns>  A hash code for the current Dt.Xls.GcRect structure.</returns>
        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            return (((((double) this.X).GetHashCode() ^ ((double) this.Y).GetHashCode()) ^ ((double) this.Width).GetHashCode()) ^ ((double) this.Height).GetHashCode());
        }
    }
}

