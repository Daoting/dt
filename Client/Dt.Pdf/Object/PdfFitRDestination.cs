#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// Display the page designated by page, with its contents magnified 
    /// just enough to fit the rectangle specified by the coordinates left, 
    /// bottom, right, and topentirely within the window both horizontally and 
    /// vertically. If the required horizontal and vertical magnification factors 
    /// are different, use the smaller of the two, centering the rectangle within 
    /// the window in the other dimension. A null value for any of the parameters 
    /// may result in unpredictable behavior.
    /// </summary>
    public class PdfFitRDestination : PdfDestination
    {
        private float bottom;
        private float left;
        private float right;
        private float top;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFitRDestination" /> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="right">The right.</param>
        /// <param name="top">The top.</param>
        public PdfFitRDestination(PdfPage page, float left, float bottom, float right, float top) : base(page)
        {
            this.left = left;
            this.bottom = bottom;
            this.right = right;
            this.top = top;
        }

        /// <summary>
        /// Adds the type and arguments.
        /// </summary>
        protected override void AddTypeAndArguments()
        {
            base.Add(PdfName.FitR);
            base.Add(new PdfNumber((double) this.left));
            base.Add(new PdfNumber((double) this.bottom));
            base.Add(new PdfNumber((double) this.right));
            base.Add(new PdfNumber((double) this.top));
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public float Bottom
        {
            get { return  this.bottom; }
            set { this.bottom = value; }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public float Left
        {
            get { return  this.left; }
            set { this.left = value; }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public float Right
        {
            get { return  this.right; }
            set { this.right = value; }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public float Top
        {
            get { return  this.top; }
            set { this.top = value; }
        }
    }
}

