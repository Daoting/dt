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
    /// GcRangeBlock
    /// </summary>
    internal class GcRangeBlock : GcBlock
    {
        readonly GcBlockCollection blocks;
        double offsetX;
        double offsetY;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcRangeBlock" /> class.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        public GcRangeBlock(Windows.Foundation.Rect rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcRangeBlock" /> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public GcRangeBlock(double x, double y, double width, double height) : base(x, y, width, height, null)
        {
            this.blocks = new GcBlockCollection();
        }

        /// <summary>
        /// Gets the blocks.
        /// </summary>
        /// <value>The blocks.</value>
        public GcBlockCollection Blocks
        {
            get { return  this.blocks; }
        }

        /// <summary>
        /// Gets or sets the X offset.
        /// </summary>
        /// <value>The X offset.</value>
        public double OffsetX
        {
            get { return  this.offsetX; }
            set { this.offsetX = value; }
        }

        /// <summary>
        /// Gets or sets the Y offset.
        /// </summary>
        /// <value>The Y offset.</value>
        public double OffsetY
        {
            get { return  this.offsetY; }
            set { this.offsetY = value; }
        }
    }
}

