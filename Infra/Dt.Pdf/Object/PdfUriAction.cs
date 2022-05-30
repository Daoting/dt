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
    /// The URI action
    /// </summary>
    public class PdfUriAction : PdfActionBase
    {
        private bool isMap;
        private string uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfUriAction" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public PdfUriAction(string uri) : base(PdfName.URI)
        {
            this.uri = uri;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.URI] = new PdfString(this.uri);
            if (this.isMap)
            {
                base[PdfName.IsMap] = PdfBool.TRUE;
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is map.
        /// </summary>
        /// <value><c>true</c> if this instance is map; otherwise, <c>false</c>.</value>
        public bool IsMap
        {
            get { return  this.isMap; }
            set { this.isMap = value; }
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

