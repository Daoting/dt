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
    /// The truetype font object of Pdf
    /// </summary>
    public class PdfTrueTypeFont : PdfFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTrueTypeFont" /> class.
        /// </summary>
        /// <param name="baseFont">The base font.</param>
        public PdfTrueTypeFont(BaseFont baseFont) : base(PdfName.TrueType, baseFont)
        {
            if (baseFont == null)
            {
                throw new PdfArgumentNullException("baseFont");
            }
            if (!(baseFont is OpenTypeFont) && !(baseFont is SimpleTrueTypeFont))
            {
                throw new PdfArgumentException("baseFont must be OpenType/TrueType font.");
            }
            base.baseFont = baseFont;
            this.InitPdfFontDescriptor();
        }

        /// <summary>
        /// Inits the PDF font descriptor.
        /// </summary>
        private void InitPdfFontDescriptor()
        {
            base[PdfName.FontDescriptor] = new PdfFontDescriptor(base.baseFont, base.FontType);
        }

        /// <summary>
        /// Called when [base font changing].
        /// </summary>
        /// <param name="bf">The bf.</param>
        /// <returns></returns>
        protected override bool OnBaseFontChanging(BaseFont bf)
        {
            this.InitPdfFontDescriptor();
            return base.OnBaseFontChanging(bf);
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Encoding] = new PdfName(base.baseFont.Encoding, true);
            base.ToPdf(writer);
        }
    }
}

