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
    /// GcPageRectangles
    /// </summary>
    internal class GcPageRectangles
    {
        Windows.Foundation.Rect bottomMarginRectangle;
        Windows.Foundation.Rect contentRectangle;
        Windows.Foundation.Rect cropRectangle;
        Windows.Foundation.Rect pageFooterRectangle;
        Windows.Foundation.Rect pageHeaderRectangle;
        Windows.Foundation.Rect pageRectangle;
        Windows.Foundation.Rect topMarginRectangle;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcPageRectangles" /> class.
        /// </summary>
        public GcPageRectangles()
        {
            this.contentRectangle = Windows.Foundation.Rect.Empty;
            this.cropRectangle = Windows.Foundation.Rect.Empty;
            this.pageHeaderRectangle = Windows.Foundation.Rect.Empty;
            this.pageFooterRectangle = Windows.Foundation.Rect.Empty;
            this.pageRectangle = Windows.Foundation.Rect.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcPageRectangles" /> class.
        /// </summary>
        /// <param name="rects">The rectangles.</param>
        public GcPageRectangles(GcPageRectangles rects)
        {
            this.contentRectangle = Windows.Foundation.Rect.Empty;
            this.cropRectangle = Windows.Foundation.Rect.Empty;
            this.pageHeaderRectangle = Windows.Foundation.Rect.Empty;
            this.pageFooterRectangle = Windows.Foundation.Rect.Empty;
            this.pageRectangle = Windows.Foundation.Rect.Empty;
            this.cropRectangle = rects.cropRectangle;
            this.contentRectangle = rects.contentRectangle;
            this.pageHeaderRectangle = rects.pageHeaderRectangle;
            this.pageFooterRectangle = rects.pageFooterRectangle;
        }

        /// <summary>
        /// Gets or sets the bottom margin rectangle.
        /// </summary>
        /// <value>The bottom margin rectangle.</value>
        public Windows.Foundation.Rect BottomMarginRectangle
        {
            get { return  this.bottomMarginRectangle; }
            set { this.bottomMarginRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the rectangle content.
        /// </summary>
        /// <value>The rectangle content.</value>
        public Windows.Foundation.Rect ContentRectangle
        {
            get { return  this.contentRectangle; }
            set { this.contentRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the crop rectangle.
        /// </summary>
        /// <value>The crop rectangle.</value>
        public Windows.Foundation.Rect CropRectangle
        {
            get { return  this.cropRectangle; }
            set { this.cropRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the page footer rectangle.
        /// </summary>
        /// <value>The page footer rectangle.</value>
        public Windows.Foundation.Rect PageFooterRectangle
        {
            get { return  this.pageFooterRectangle; }
            set { this.pageFooterRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the page header rectangle.
        /// </summary>
        /// <value>The page header rectangle.</value>
        public Windows.Foundation.Rect PageHeaderRectangle
        {
            get { return  this.pageHeaderRectangle; }
            set { this.pageHeaderRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the page rectangle.
        /// </summary>
        /// <value>The page rectangle.</value>
        public Windows.Foundation.Rect PageRectangle
        {
            get { return  this.pageRectangle; }
            set { this.pageRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the top margin rectangle.
        /// </summary>
        /// <value>The top margin rectangle.</value>
        public Windows.Foundation.Rect TopMarginRectangle
        {
            get { return  this.topMarginRectangle; }
            set { this.topMarginRectangle = value; }
        }
    }
}

