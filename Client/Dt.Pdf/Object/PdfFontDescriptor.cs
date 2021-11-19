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
using Dt.Pdf.Drawing;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Object.Filter;
using Dt.Pdf.Text;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// A font descriptor specifies metrics and other attributes of 
    /// a simple font or a CIDFont as a whole, as distinct from the 
    /// metrics of individual glyphs.
    /// </summary>
    public class PdfFontDescriptor : PdfDictionary
    {
        private float ascent;
        private readonly BaseFont baseFont;
        private float capHeight = 500f;
        private float descent;
        private Flag flags = Flag.Nonsymbolic;
        private PdfRectangle fontBBox;
        private PdfFontStream fontFile;
        private float italicAngle;
        private readonly PdfName parentFontType;
        private float stemV;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFontDescriptor" /> class.
        /// </summary>
        /// <param name="baseFont">The base font.</param>
        /// <param name="parentType">Type of the parent.</param>
        public PdfFontDescriptor(BaseFont baseFont, PdfName parentType)
        {
            if (baseFont == null)
            {
                throw new PdfArgumentNullException("baseFont");
            }
            base.Add(PdfName.Type, PdfName.FontDescriptor);
            this.baseFont = baseFont;
            this.parentFontType = parentType;
            base.isLabeled = true;
            this.flags = baseFont.GetFlag();
            this.fontBBox = baseFont.GetFontBoundingBox();
            this.italicAngle = baseFont.GetItalicAngle();
            this.ascent = baseFont.GetAscent();
            this.descent = baseFont.GetDescent();
            this.capHeight = baseFont.GetCapitalLetterHeight();
            this.stemV = baseFont.GetVerticalStem();
        }

        /// <summary>
        /// Creates the font file.
        /// </summary>
        /// <param name="isCFF">if set to <c>true</c> [is CFF].</param>
        /// <returns></returns>
        public PdfFontStream CreateFontFile(bool isCFF)
        {
            this.fontFile = new PdfFontStream(this.parentFontType, isCFF);
            this.fontFile.Filters.Enqueue(PdfFilter.FlateFilter);
            return this.fontFile;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.FontName] = new PdfName(this.baseFont.GetName());
            base[PdfName.Flags] = new PdfNumber((double) this.flags);
            if ((this.parentFontType != PdfName.Type3) || (this.fontBBox != null))
            {
                base[PdfName.FontBBox] = this.fontBBox;
            }
            base[PdfName.ItalicAngle] = new PdfNumber((double) this.italicAngle);
            if ((this.parentFontType != PdfName.Type3) || (this.ascent != 0f))
            {
                base[PdfName.Ascent] = new PdfNumber((double) this.ascent);
            }
            if ((this.parentFontType != PdfName.Type3) || (this.descent != 0f))
            {
                base[PdfName.Descent] = new PdfNumber((double) this.descent);
            }
            if ((this.parentFontType != PdfName.Type3) || (this.capHeight != 500f))
            {
                base[PdfName.CapHeight] = new PdfNumber((double) this.capHeight);
            }
            if ((this.parentFontType != PdfName.Type3) || (this.stemV != 0f))
            {
                base[PdfName.StemV] = new PdfNumber((double) this.stemV);
            }
            if (this.fontFile != null)
            {
                base[this.fontFile.Type] = this.fontFile;
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// The maximum height above the baseline reached by glyphs 
        /// in this font, excluding the height of glyphs for accented characters.
        /// </summary>
        /// <value>The ascent.</value>
        public float Ascent
        {
            get { return  this.ascent; }
            set { this.ascent = value; }
        }

        /// <summary>
        /// Gets the base font.
        /// </summary>
        /// <value>The base font.</value>
        public BaseFont BaseFont
        {
            get { return  this.baseFont; }
        }

        /// <summary>
        /// The vertical coordinate of the top of flat capital 
        /// letters, measured from the baseline.
        /// </summary>
        /// <value>The height of the cap.</value>
        public float CapHeight
        {
            get { return  this.capHeight; }
            set { this.capHeight = value; }
        }

        /// <summary>
        /// The maximum depth below the baseline reached by glyphs 
        /// in this font. The value is a negative number.
        /// </summary>
        /// <value>The descent.</value>
        public float Descent
        {
            get { return  this.descent; }
            set { this.descent = value; }
        }

        /// <summary>
        /// A collection of flags defining various characteristics of the font
        /// </summary>
        /// <value>The flags.</value>
        public Flag Flags
        {
            get { return  this.flags; }
            set { this.flags = value; }
        }

        /// <summary>
        /// A rectangle, expressed in the glyph coordinate system,
        /// specifying the font bounding box. This is the smallest
        /// rectangle enclosing the shape that would result if all
        /// of the glyphs of the font were placed with their origins
        /// coincident and then filled.
        /// </summary>
        /// <value>The font B box.</value>
        public PdfRectangle FontBBox
        {
            get { return  this.fontBBox; }
            set { this.fontBBox = value; }
        }

        /// <summary>
        /// Gets the font file.
        /// </summary>
        /// <value>The font file.</value>
        public PdfFontStream FontFile
        {
            get { return  this.fontFile; }
        }

        /// <summary>
        /// The angle, expressed in degrees counterclockwise 
        /// from the vertical, of the dominant vertical strokes of the font.
        /// </summary>
        /// <value>The italic angle.</value>
        public float ItalicAngle
        {
            get { return  this.italicAngle; }
            set { this.italicAngle = value; }
        }

        /// <summary>
        /// The thickness, measured horizontally, of the dominant 
        /// vertical stems of glyphs in the font.
        /// </summary>
        /// <value>The stem V.</value>
        public float StemV
        {
            get { return  this.stemV; }
            set { this.stemV = value; }
        }

        /// <summary>
        /// Specifying various characteristics of the font
        /// </summary>
        [Flags]
        public enum Flag
        {
            /// <summary>
            /// Font contains no lowercase letters; typically used for display purposes, such as for titles or headlines.
            /// </summary>
            AllCap = 0x10000,
            /// <summary>
            /// All glyphs have the same width (as opposed to proportional or 
            /// variable-pitch fonts, which have different widths).
            /// </summary>
            FixedPitch = 1,
            /// <summary>
            /// 
            /// </summary>
            ForceBold = 0x40000,
            /// <summary>
            /// Glyphs have dominant vertical strokes that are slanted.
            /// </summary>
            Italic = 0x40,
            /// <summary>
            /// Font uses the Adobe standard Latin character set or a subset of it.
            /// </summary>
            Nonsymbolic = 0x20,
            /// <summary>
            /// Glyphs resemble cursive handwriting.
            /// </summary>
            Script = 8,
            /// <summary>
            /// Glyphs have serifs, which are short strokes drawn at an angle 
            /// on the top and bottom of glyph stems. (Sans serif fonts do not have serifs.)
            /// </summary>
            Serif = 2,
            /// <summary>
            /// Font contains both uppercase and lowercase letters. The uppercase 
            /// letters are similar to those in the regular version of the same typeface 
            /// family. The glyphs for the lowercase letters have the same shapes as 
            /// the corresponding uppercase letters, but they are sized and their 
            /// proportions adjusted so that they have the same size and stroke weight 
            /// as lowercase glyphs in the same typeface family.
            /// </summary>
            SmallCap = 0x20000,
            /// <summary>
            /// Font contains glyphs outside the Adobe standard Latin 
            /// character set. This flag and the Nonsymbolic flag cannot both be set or both be clear.
            /// </summary>
            Symbolic = 4
        }
    }
}

