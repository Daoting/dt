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
    /// Display the page designated by page, with the vertical coordinate 
    /// top positioned at the top edge of the window and the contents of 
    /// the page magnified just enough to fit the entire width of its 
    /// bounding box within the window. A null value for top specifies 
    /// that the current value of that parameter is to be retained unchanged.
    /// </summary>
    public class PdfFitBHDestination : PdfDestination
    {
        private float top;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFitBHDestination" /> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="top">The top.</param>
        public PdfFitBHDestination(PdfPage page, float top) : base(page)
        {
            this.top = top;
        }

        /// <summary>
        /// Adds the type and arguments.
        /// </summary>
        protected override void AddTypeAndArguments()
        {
            base.Add(PdfName.FitBH);
            base.Add(new PdfNumber((double) this.top));
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

