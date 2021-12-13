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
    /// Pdf file attachment annotation.
    /// </summary>
    public class PdfFileAttachmentAnnotation : PdfAnnotationBase
    {
        private readonly PdfFileSpecification fileSpecification;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFileAttachmentAnnotation" /> class.
        /// </summary>
        public PdfFileAttachmentAnnotation() : base(PdfName.FileAttachment)
        {
            this.fileSpecification = new PdfFileSpecification();
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns></returns>
        protected override PdfVersion GetVersion()
        {
            return PdfVersion.PDF1_3;
        }

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteProperties(PdfWriter writer)
        {
            base[PdfName.FS] = this.fileSpecification;
        }

        /// <summary>
        /// Gets the file specification.
        /// </summary>
        /// <value>The file specification.</value>
        public PdfFileSpecification FileSpecification
        {
            get { return  this.fileSpecification; }
        }
    }
}

