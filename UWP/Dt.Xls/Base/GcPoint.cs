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
    /// Represents an ordered pair of integer x- and y- coordinates that defines a point in a two-dimensional plane
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GcPoint
    {
        internal double _x;
        internal double _y;
        /// <summary>
        /// Initializes a new instance of the Dt.Xls.GcPoint class with the specified coordinates.
        /// </summary>
        /// <param name="x">The horizontal position of the point.</param>
        /// <param name="y"> The vertical position of the point.</param>
        public GcPoint(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        /// <summary>
        /// Gets or sets the x-coordinate of this  Dt.Xls.GcPoint
        /// </summary>
        public double X
        {
            get { return  this._x; }
            set { this._x = value; }
        }
        /// <summary>
        /// Gets or sets the y-coordinate of this  Dt.Xls.GcPoint
        /// </summary>
        public double Y
        {
            get { return  this._y; }
            set { this._y = value; }
        }
        /// <summary>
        /// Compares two  Dt.Xls.GcPoint objects.  The result specifies whether the values of the two objects are equal
        /// </summary>
        /// <param name="point1">A  Dt.Xls.GcPoint object to compare</param>
        /// <param name="point2">A  Dt.Xls.GcPoint object to compare</param>
        /// <returns>true if they are equal; otherwise, false.</returns>
        public static bool operator ==(GcPoint point1, GcPoint point2)
        {
            return ((point1.X == point2.X) && (point1.Y == point2.Y));
        }

        /// <summary>
        /// Compares two  Dt.Xls.GcPoint objects.  The result specifies whether the values of the two objects are unequal
        /// </summary>
        /// <param name="point1">A  Dt.Xls.GcPoint object to compare</param>
        /// <param name="point2">A  Dt.Xls.GcPoint object to compare</param>
        /// <returns>true if they are unequal; otherwise, false.</returns>
        public static bool operator !=(GcPoint point1, GcPoint point2)
        {
            return !(point1 == point2);
        }

        /// <summary>
        /// Compares two  Dt.Xls.GcPoint objects.  The result specifies whether the values of the two objects are unequal
        /// </summary>
        /// <param name="point1">A  Dt.Xls.GcPoint object to compare</param>
        /// <param name="point2">A  Dt.Xls.GcPoint object to compare</param>
        /// <returns>true if they are unequal; otherwise, false.</returns>
        public static bool Equals(GcPoint point1, GcPoint point2)
        {
            return (((double) point1.X).Equals(point2.X) && ((double) point1.Y).Equals(point2.Y));
        }

        /// <summary>
        /// Specifies whether this Dt.Xls.GcPoint contains the same coordinates as the specified System.Object
        /// </summary>
        /// <param name="o">The System.Object to test.</param>
        /// <returns>true if obj is a  Dt.Xls.GcPoint and has the same coordinates as this  Dt.Xls.GcPoint</returns>
        public override bool Equals(object o)
        {
            if ((o == null) || !(o is GcPoint))
            {
                return false;
            }
            GcPoint point = (GcPoint) o;
            return Equals(this, point);
        }

        /// <summary>
        /// Specifies whether this Dt.Xls.GcPoint contains the same coordinates as the specified Dt.Xls.GcPoint
        /// </summary>
        /// <param name="value">A Dt.Xls.GcPoint to compare.</param>
        /// <returns>true if the Dt.Xls.GcPoint has the same coordinates as this Dt.Xls.GcPoint</returns>
        public bool Equals(GcPoint value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Returns a hash code for this Dt.Xls.GcPoint
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this object</returns>
        public override int GetHashCode()
        {
            return (((double) this.X).GetHashCode() ^ ((double) this.Y).GetHashCode());
        }
    }
}

