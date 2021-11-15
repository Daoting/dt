#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// GcPageBlock
    /// </summary>
    internal class GcPageBlock : GcBlock
    {
        readonly GcBlockCollection blocks = new GcBlockCollection();
        GcRangeBlock bottomMargin;
        GcRangeBlock pageFooter;
        GcRangeBlock pageHeader;
        GcPageRectangles rectangles = new GcPageRectangles();
        GcRangeBlock topMargin;

        /// <summary>
        /// Gets the blocks.
        /// </summary>
        /// <value>The blocks.</value>
        public GcBlockCollection Blocks
        {
            get { return  this.blocks; }
        }

        /// <summary>
        /// Gets or sets the bottom margin.
        /// </summary>
        /// <value>The bottom margin.</value>
        public GcRangeBlock BottomMargin
        {
            get { return  this.bottomMargin; }
            set { this.bottomMargin = value; }
        }

        /// <summary>
        /// Gets or sets the page footer.
        /// </summary>
        /// <value>The page footer.</value>
        public GcRangeBlock PageFooter
        {
            get { return  this.pageFooter; }
            set { this.pageFooter = value; }
        }

        /// <summary>
        /// Gets or sets the page header.
        /// </summary>
        /// <value>The page header.</value>
        public GcRangeBlock PageHeader
        {
            get { return  this.pageHeader; }
            set { this.pageHeader = value; }
        }

        /// <summary>
        /// Gets or sets the rectangles.
        /// </summary>
        /// <value>The rectangles.</value>
        public GcPageRectangles Rectangles
        {
            get { return  this.rectangles; }
            set { this.rectangles = value; }
        }

        /// <summary>
        /// Gets or sets the top margin.
        /// </summary>
        /// <value>The top margin.</value>
        public GcRangeBlock TopMargin
        {
            get { return  this.topMargin; }
            set { this.topMargin = value; }
        }
    }
}

