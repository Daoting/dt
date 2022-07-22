#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// GcBlock
    /// </summary>
    internal class GcBlock
    {
        object cache;
        readonly GcControl data;
        double height;
        double width;
        double x;
        double y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcBlock" /> class.
        /// </summary>
        public GcBlock()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcBlock" /> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="data">The data.</param>
        public GcBlock(double x, double y, double width, double height, GcControl data)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcBlock" /> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="data">The data.</param>
        /// <param name="cache">The cache.</param>
        public GcBlock(double x, double y, double width, double height, GcControl data, object cache) : this(x, y, width, height, data)
        {
            this.cache = cache;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public GcBlock Clone()
        {
            return new GcBlock(this.x, this.y, this.width, this.height, this.data, this.cache);
        }

        /// <summary>
        /// Indicates whether the GcBlock is intersected with a specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        public bool IntersectWith(GcBlock block)
        {
            return ((block != null) && this.IntersectWith(block.X, block.Y, block.Width, block.Height));
        }

        /// <summary>
        /// Indicates whether a specified rectangle intersects the GcBlock object.
        /// </summary>
        /// <param name="xx">The left of the rectangle.</param>
        /// <param name="yy">The top of the rectangle.</param>
        /// <param name="ww">The width of the rectangle.</param>
        /// <param name="hh">The height of the rectangle.</param>
        /// <returns>
        /// <c>true</c> if the GcBlock is intersected with the rectangle; otherwise, <c>false</c>.
        /// </returns>
        public bool IntersectWith(double xx, double yy, double ww, double hh)
        {
            return ((((this.Right > xx) && (this.Left < (xx + ww))) && (this.Bottom > yy)) && (this.Top < (yy + hh)));
        }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public double Bottom
        {
            get { return  (this.Y + this.Height); }
        }

        /// <summary>
        /// Gets or sets the cache.
        /// </summary>
        /// <value>The cache.</value>
        public object Cache
        {
            get { return  this.cache; }
            set { this.cache = value; }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public GcControl Data
        {
            get { return  this.data; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return  this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
        public double Left
        {
            get { return  this.X; }
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public Windows.Foundation.Rect Rect
        {
            get { return  new Windows.Foundation.Rect(this.x, this.y, this.width, this.Height); }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right.</value>
        public double Right
        {
            get { return  (this.X + this.Width); }
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>The top.</value>
        public double Top
        {
            get { return  this.Y; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return  this.width; }
            set { this.width = value; }
        }

        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get { return  this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return  this.y; }
            set { this.y = value; }
        }
    }
}

