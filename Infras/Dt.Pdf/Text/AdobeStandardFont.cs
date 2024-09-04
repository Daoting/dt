#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// Adobe standard font.
    /// </summary>
    public class AdobeStandardFont : BaseFont
    {
        public static AdobeStandardFont Courier = new AdobeStandardFont(PdfName.Courier);
        public static AdobeStandardFont Courier_Bold = new AdobeStandardFont(PdfName.Courier_Bold);
        public static AdobeStandardFont Courier_BoldOblique = new AdobeStandardFont(PdfName.Courier_BoldOblique);
        public static AdobeStandardFont Courier_Oblique = new AdobeStandardFont(PdfName.Courier_Oblique);
        public static AdobeStandardFont Helvetica = new AdobeStandardFont(PdfName.Helvetica);
        public static AdobeStandardFont Helvetica_Bold = new AdobeStandardFont(PdfName.Helvetica_Bold);
        public static AdobeStandardFont Helvetica_BoldOblique = new AdobeStandardFont(PdfName.Helvetica_BoldOblique);
        public static AdobeStandardFont Helvetica_Oblique = new AdobeStandardFont(PdfName.Helvetica_Oblique);
        private readonly PdfName name;
        public static AdobeStandardFont Symbol = new AdobeStandardFont(PdfName.Symbol);
        public static AdobeStandardFont Times_Bold = new AdobeStandardFont(PdfName.Times_Bold);
        public static AdobeStandardFont Times_BoldItalic = new AdobeStandardFont(PdfName.Times_BoldItalic);
        public static AdobeStandardFont Times_Italic = new AdobeStandardFont(PdfName.Times_Italic);
        public static AdobeStandardFont Times_Roman = new AdobeStandardFont(PdfName.Times_Roman);
        public static AdobeStandardFont ZapfDingbats = new AdobeStandardFont(PdfName.ZapfDingbats);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AdobeStandardFont" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        private AdobeStandardFont(PdfName name)
        {
            if (name == null)
            {
                throw new PdfArgumentNullException("name");
            }
            this.name = name;
        }

        /// <summary>
        /// Gets the code pages.
        /// </summary>
        /// <returns></returns>
        public override int[] GetCodePages()
        {
            return new int[] { 0x4e4 };
        }

        /// <summary>
        /// Gets the index of the glyph.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public override int GetGlyphIndex(char c)
        {
            throw new PdfNotSupportedException();
        }

        /// <summary>
        /// Gets the name of the PDF.
        /// </summary>
        /// <returns></returns>
        public PdfName GetPdfName()
        {
            return this.name;
        }

        /// <summary>
        /// Gets the name of the post script.
        /// </summary>
        /// <returns></returns>
        protected override string GetPostScriptName()
        {
            return this.name.Name;
        }

        /// <summary>
        /// Gets the sub font.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="isCFF">if set to <c>true</c> [is CFF].</param>
        /// <returns></returns>
        public override byte[] GetSubFont(Dictionary<char, int> chars, out bool isCFF)
        {
            throw new PdfNotSupportedException();
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override float GetWidth(int index)
        {
            throw new PdfNotSupportedException();
        }
    }
}

