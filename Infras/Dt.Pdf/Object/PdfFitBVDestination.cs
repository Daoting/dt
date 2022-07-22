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
    /// Display the page designated by page, with the horizontal coordinate 
    /// left positioned at the left edge of the window and the contents of 
    /// the page magnified just enough to fit the entire height of its 
    /// bounding box within the window. A null value for left specifies 
    /// that the current value of that parameter is to be retained unchanged.
    /// </summary>
    public class PdfFitBVDestination : PdfDestination
    {
        private float left;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFitBVDestination" /> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        public PdfFitBVDestination(PdfPage page, float left) : base(page)
        {
            this.left = left;
        }

        /// <summary>
        /// Adds the type and arguments.
        /// </summary>
        protected override void AddTypeAndArguments()
        {
            base.Add(PdfName.FitBV);
            base.Add(new PdfNumber((double) this.left));
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
    }
}

