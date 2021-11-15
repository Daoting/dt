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
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// Font object of Pdf
    /// </summary>
    public abstract class PdfFont : PdfDictionary
    {
        protected BaseFont baseFont;
        protected PdfName baseFontName;
        protected bool isStandard;
        protected readonly Dictionary<char, int> usedChars = new Dictionary<char, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFont" /> class.
        /// </summary>
        /// <param name="fontType">Type of the font.</param>
        /// <param name="baseFont">The base font.</param>
        protected PdfFont(PdfName fontType, BaseFont baseFont)
        {
            base.Add(PdfName.Type, PdfName.Font);
            base.Add(PdfName.Subtype, fontType);
            this.baseFont = baseFont;
        }

        /// <summary>
        /// Creates the specified font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        public static PdfFont Create(BaseFont font)
        {
            if (font is AdobeStandardFont)
            {
                return new PdfType1Font(font);
            }
            if (font is OpenTypeFont)
            {
                OpenTypeFont font2 = font as OpenTypeFont;
                switch (font.Encoding)
                {
                    case "WinAnsiEncoding":
                        if (!font2.IsCFF)
                        {
                            return new PdfTrueTypeFont(font);
                        }
                        return new PdfType1Font(font);

                    case "Identity-H":
                    case "Identity-V":
                        return new PdfType0Font(font);

                    case "UniGB-UCS2-H":
                    case "UniCNS-UCS2-H":
                    case "UniJIS-UCS2-H":
                    case "UniKS-UCS2-H":
                        return new PdfType0Font(font);
                }
            }
            if (font is SimpleTrueTypeFont)
            {
                switch (font.Encoding)
                {
                    case "WinAnsiEncoding":
                        return new PdfTrueTypeFont(font);

                    case "Identity-H":
                    case "Identity-V":
                        throw new PdfNotSupportedException();

                    case "UniGB-UCS2-H":
                    case "UniCNS-UCS2-H":
                    case "UniJIS-UCS2-H":
                    case "UniKS-UCS2-H":
                        return new PdfType0Font(font);
                }
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the index of the glyph.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected virtual int GetGlyphIndex(char c)
        {
            return this.baseFont.GetGlyphIndex(c);
        }

        /// <summary>
        /// Gets the show operan.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public byte[] GetShowOperan(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            if (this.isStandard)
            {
                return PdfLatin1Encoding.Instance.GetBytes(text);
            }
            switch (this.baseFont.Encoding)
            {
                case "WinAnsiEncoding":
                    foreach (char ch in text)
                    {
                        if ((this.baseFont is OpenTypeFont) || (this.baseFont is SimpleTrueTypeFont))
                        {
                            int glyphIndex = this.baseFont.GetGlyphIndex(ch);
                            if (glyphIndex != -1)
                            {
                                this.usedChars[ch] = glyphIndex;
                            }
                        }
                        else
                        {
                            this.usedChars[ch] = 0;
                        }
                    }
                    return PdfLatin1Encoding.Instance.GetBytes(text);

                case "Identity-H":
                case "Identity-V":
                {
                    List<char> list = new List<char>();
                    for (int i = 0; i < text.Length; i++)
                    {
                        int num3;
                        char key = text[i];
                        if (this.usedChars.ContainsKey(key))
                        {
                            num3 = this.usedChars[key];
                        }
                        else
                        {
                            num3 = this.baseFont.GetGlyphIndex(key);
                        }
                        if (num3 != -1)
                        {
                            list.Add((char) num3);
                            this.usedChars[key] = num3;
                        }
                    }
                    return PdfEncodingBase.UnicodeBigUnmarked.GetBytes(list.ToArray());
                }
                case "UniCNS-UCS2-H":
                case "UniGB-UCS2-H":
                case "UniJIS-UCS2-H":
                case "UniKS-UCS2-H":
                    foreach (char ch3 in text)
                    {
                        if ((this.baseFont is OpenTypeFont) || (this.baseFont is SimpleTrueTypeFont))
                        {
                            int num4 = this.baseFont.GetGlyphIndex(ch3);
                            if (num4 != -1)
                            {
                                this.usedChars[ch3] = num4;
                            }
                        }
                        else
                        {
                            this.usedChars[ch3] = 0;
                        }
                    }
                    return PdfEncodingBase.UnicodeBigUnmarked.GetBytes(text);
            }
            throw new PdfNotSupportedException();
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="glyphIndex">Index of the glyph.</param>
        /// <returns></returns>
        protected virtual float GetWidth(int glyphIndex)
        {
            return this.baseFont.GetWidth(glyphIndex);
        }

        /// <summary>
        /// Called when [base font changing].
        /// </summary>
        /// <param name="bf">The bf.</param>
        /// <returns></returns>
        protected virtual bool OnBaseFontChanging(BaseFont bf)
        {
            return true;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.FontType != PdfName.Type3)
            {
                base[PdfName.BaseFont] = this.BaseFontName;
            }
            if (this is PdfCIDFont)
            {
                base.ToPdf(writer);
            }
            else
            {
                if (this.baseFont.IsEmbedded)
                {
                    PdfFontDescriptor fontDescriptor = this.FontDescriptor;
                    if (fontDescriptor != null)
                    {
                        bool flag;
                        byte[] subFont = this.baseFont.GetSubFont(this.usedChars, out flag);
                        fontDescriptor.CreateFontFile(flag).Psw.WriteBytes(subFont);
                    }
                }
                if (this.FontType == PdfName.Type0)
                {
                    ((PdfType0Font) this).CIDFont.SetWidths(this.usedChars);
                }
                else if (!this.isStandard)
                {
                    char ch = (char)0xffff;
                    char ch2 = '\0';
                    string s = "";
                    foreach (char ch3 in this.usedChars.Keys)
                    {
                        s = s + ((char) ch3);
                    }
                    foreach (byte num in PdfLatin1Encoding.Instance.GetBytes(s))
                    {
                        if (num < ch)
                        {
                            ch = (char) num;
                        }
                        if (num > ch2)
                        {
                            ch2 = (char) num;
                        }
                    }
                    base[PdfName.FirstChar] = new PdfNumber((double) ch);
                    base[PdfName.LastChar] = new PdfNumber((double) ch2);
                    PdfArray array = new PdfArray {
                        IsLabeled = true
                    };
                    for (char ch4 = ch; ch4 <= ch2; ch4++)
                    {
                        int width = (int) this.GetWidth(this.GetGlyphIndex(ch4));
                        array.Add(new PdfNumber((double) width));
                    }
                    base[PdfName.Widths] = array;
                }
                base.ToPdf(writer);
            }
        }

        /// <summary>
        /// Gets or sets the base font.
        /// </summary>
        /// <value>The base font.</value>
        public BaseFont BaseFont
        {
            get { return  this.baseFont; }
            set
            {
                if (this.OnBaseFontChanging(value))
                {
                    this.baseFont = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the base font.
        /// </summary>
        /// <value>The name of the base font.</value>
        public virtual PdfName BaseFontName
        {
            get
            {
                if (this.baseFontName == null)
                {
                    this.baseFontName = new PdfName(this.baseFont.GetName());
                }
                return this.baseFontName;
            }
        }

        /// <summary>
        /// Gets the font descriptor.
        /// </summary>
        /// <value>The font descriptor.</value>
        public virtual PdfFontDescriptor FontDescriptor
        {
            get { return  (base[PdfName.FontDescriptor] as PdfFontDescriptor); }
        }

        /// <summary>
        /// Gets the type of the font.
        /// </summary>
        /// <value>The type of the font.</value>
        public PdfName FontType
        {
            get { return  (base[PdfName.Subtype] as PdfName); }
        }
    }
}

