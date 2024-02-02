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
    /// The link annotation
    /// </summary>
    public class PdfLinkAnnotation : PdfAnnotationBase
    {
        private PdfDestination dest;
        private string uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLinkAnnotation" /> class.
        /// </summary>
        public PdfLinkAnnotation() : base(PdfName.Link)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLinkAnnotation" /> class.
        /// </summary>
        /// <param name="dest">The dest.</param>
        public PdfLinkAnnotation(PdfDestination dest) : this()
        {
            this.dest = dest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLinkAnnotation" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public PdfLinkAnnotation(string uri) : this()
        {
            this.uri = uri;
        }

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteProperties(PdfWriter writer)
        {
            if (!string.IsNullOrEmpty(this.uri))
            {
                base[PdfName.A] = new PdfUriAction(this.uri);
            }
            else
            {
                base[PdfName.Dest] = this.dest;
            }
        }

        /// <summary>
        /// Gets or sets the destintation.
        /// </summary>
        /// <value>The destintation.</value>
        public PdfDestination Destintation
        {
            get { return  this.dest; }
            set { this.dest = value; }
        }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string URI
        {
            get { return  this.uri; }
            set { this.uri = value; }
        }
    }
}

