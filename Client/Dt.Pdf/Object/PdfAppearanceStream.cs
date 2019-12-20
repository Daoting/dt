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
using Dt.Pdf.Exceptions;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The Appearance stream for Annotation
    /// </summary>
    public class PdfAppearanceStream : PdfDictionary
    {
        private readonly PdfObjectBase normalAppearance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfAppearanceStream" /> class.
        /// </summary>
        /// <param name="normal">The normal.</param>
        public PdfAppearanceStream(PdfObjectBase normal)
        {
            if (normal == null)
            {
                throw new PdfArgumentNullException("normal");
            }
            if (!normal.IsLabeled)
            {
                normal.IsLabeled = true;
            }
            this.normalAppearance = normal;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.normalAppearance != null)
            {
                base[PdfName.N] = this.normalAppearance;
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the normal appearance.
        /// </summary>
        /// <value>The normal appearance.</value>
        public PdfObjectBase NormalAppearance
        {
            get { return  this.normalAppearance; }
        }
    }
}

