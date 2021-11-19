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
    /// The stream dictionary describing an embedded file contains 
    /// the standard entries for any stream.
    /// </summary>
    public class PdfEmbeddedFileStream : PdfStream
    {
        private DateTime creationDate;
        private string mime = string.Empty;
        private DateTime modifyDate;
        private long size;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfEmbeddedFileStream" /> class.
        /// </summary>
        public PdfEmbeddedFileStream()
        {
            base.Properties[PdfName.Type] = PdfName.EmbeddedFile;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (!string.IsNullOrEmpty(this.mime))
            {
                base.Properties[PdfName.Subtype] = new PdfName(this.mime, true);
            }
            if (this.creationDate.Ticks != 0L)
            {
                base.Properties[PdfName.CreationDate] = new PdfDate(this.creationDate);
            }
            if (this.modifyDate.Ticks != 0L)
            {
                base.Properties[PdfName.ModDate] = new PdfDate(this.modifyDate);
            }
            if (this.size > 0L)
            {
                base.Properties[PdfName.Size] = new PdfNumber((double) this.size);
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>The creation date.</value>
        public DateTime CreationDate
        {
            get { return  this.creationDate; }
            set { this.creationDate = value; }
        }

        /// <summary>
        /// Gets or sets the MIME.
        /// </summary>
        /// <value>The MIME.</value>
        public string MIME
        {
            get { return  this.mime; }
            set { this.mime = value; }
        }

        /// <summary>
        /// Gets or sets the modify date.
        /// </summary>
        /// <value>The modify date.</value>
        public DateTime ModifyDate
        {
            get { return  this.modifyDate; }
            set { this.modifyDate = value; }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size
        {
            get { return  this.size; }
            set { this.size = value; }
        }
    }
}

