#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.BaseObject;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// A destination defines a particular view of a document.
    /// </summary>
    public abstract class PdfDestination : PdfArray
    {
        private PdfPage targetPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfDestination" /> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PdfDestination(PdfPage page)
        {
            this.targetPage = page;
        }

        /// <summary>
        /// Adds the type and arguments.
        /// </summary>
        protected abstract void AddTypeAndArguments();
        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base.Add(this.targetPage);
            this.AddTypeAndArguments();
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets the target page.
        /// </summary>
        /// <value>The target page.</value>
        public PdfPage TargetPage
        {
            get { return  this.targetPage; }
            set { this.targetPage = value; }
        }
    }
}

