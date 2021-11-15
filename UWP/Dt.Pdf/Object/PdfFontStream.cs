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
    /// Font stream for embedded font file
    /// </summary>
    public class PdfFontStream : PdfStream, IVersionDepend
    {
        private readonly PdfName type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFontStream" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="isCFF">if set to <c>true</c> [is CFF].</param>
        internal PdfFontStream(PdfName type, bool isCFF)
        {
            if (type == null)
            {
                throw new PdfArgumentNullException("type");
            }
            if (type.Equals(PdfName.Type1) || type.Equals(PdfName.MMType1))
            {
                if (isCFF)
                {
                    this.type = PdfName.FontFile3;
                    base.Properties[PdfName.Subtype] = PdfName.Type1C;
                }
                else
                {
                    this.type = PdfName.FontFile;
                }
            }
            else if (type.Equals(PdfName.CIDFontType0))
            {
                this.type = PdfName.FontFile3;
                base.Properties[PdfName.Subtype] = PdfName.CIDFontType0C;
            }
            else if (type.Equals(PdfName.TrueType) || type.Equals(PdfName.CIDFontType2))
            {
                if (isCFF)
                {
                    this.type = PdfName.FontFile3;
                    base.Properties[PdfName.Subtype] = PdfName.OpenType;
                }
                else
                {
                    this.type = PdfName.FontFile2;
                }
            }
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (PdfName.FontFile2.Equals(this.type))
            {
                base.Properties[PdfName.Length1] = new PdfNumber((double) base.Length);
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            if (PdfName.FontFile2.Equals(this.type))
            {
                return PdfVersion.PDF1_1;
            }
            if (PdfName.FontFile3.Equals(this.type))
            {
                PdfName name = base.Properties[PdfName.Subtype] as PdfName;
                if (PdfName.Type1C.Equals(name))
                {
                    return PdfVersion.PDF1_2;
                }
                if (PdfName.CIDFontType0C.Equals(name))
                {
                    return PdfVersion.PDF1_3;
                }
                if (PdfName.OpenType.Equals(name))
                {
                    return PdfVersion.PDF1_6;
                }
            }
            return PdfVersion.PDF1_0;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public PdfName Type
        {
            get { return  this.type; }
        }
    }
}

