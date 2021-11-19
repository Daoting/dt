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
using Dt.Pdf.Text;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The type0 font object of Pdf
    /// </summary>
    public class PdfType0Font : PdfFont
    {
        private readonly PdfCIDFont cidFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfType0Font" /> class.
        /// </summary>
        /// <param name="baseFont">The base font.</param>
        public PdfType0Font(BaseFont baseFont) : base(PdfName.Type0, baseFont)
        {
            if (baseFont == null)
            {
                throw new PdfArgumentNullException("baseFont");
            }
            if (baseFont is OpenTypeFont)
            {
                OpenTypeFont font = baseFont as OpenTypeFont;
                this.cidFont = font.IsCFF ? new PdfCIDFont(PdfName.CIDFontType0, baseFont) : new PdfCIDFont(PdfName.CIDFontType2, baseFont);
                base[PdfName.DescendantFonts] = new PdfArray(new PdfCIDFont[] { this.cidFont });
            }
            else
            {
                if (!(baseFont is SimpleTrueTypeFont))
                {
                    throw new PdfNotSupportedException("Didn't support this font");
                }
                this.cidFont = new PdfCIDFont(PdfName.CIDFontType2, baseFont);
                base[PdfName.DescendantFonts] = new PdfArray(new PdfCIDFont[] { this.cidFont });
            }
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.ToUnicode] = new PdfToUnicode(base.usedChars);
            base[PdfName.Encoding] = new PdfName(base.baseFont.Encoding, true);
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the CID font.
        /// </summary>
        /// <value>The CID font.</value>
        public PdfCIDFont CIDFont
        {
            get { return  this.cidFont; }
        }

        /// <summary>
        /// Gets the font descriptor.
        /// </summary>
        /// <value>The font descriptor.</value>
        public override PdfFontDescriptor FontDescriptor
        {
            get { return  this.CIDFont.FontDescriptor; }
        }
    }
}

