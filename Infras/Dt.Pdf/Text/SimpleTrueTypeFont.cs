#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.Drawing;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Object;
using Dt.Pdf.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
#endregion

namespace Dt.Pdf.Text
{
    public class SimpleTrueTypeFont : BaseFont
    {
        private static readonly XmlReaderSettings _settings = new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true };
        private static readonly Dictionary<string, SimpleTrueTypeFont> _dict = new Dictionary<string, SimpleTrueTypeFont>();
        private readonly CT_SimpleFont simpleFont;
        private readonly Dictionary<int, float> widths;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleTrueTypeFont" /> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public SimpleTrueTypeFont(CT_SimpleFont font)
        {
            this.widths = new Dictionary<int, float>();
            if (font == null)
            {
                throw new PdfArgumentNullException("font");
            }
            this.simpleFont = font;
            this.InitWidths();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleTrueTypeFont" /> class.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        public SimpleTrueTypeFont(Stream fontStream)
        {
            this.widths = new Dictionary<int, float>();
            if ((fontStream == null) || !fontStream.CanRead)
            {
                throw new PdfArgumentNullException("fontStream");
            }
            byte[] buffer = new byte[(int)fontStream.Length];
            fontStream.Read(buffer, 0, buffer.Length);

            // hdt 替换原有的XmlSerializer，Release版不支持
            CT_SimpleFont font;
            using (MemoryStream stream = new MemoryStream(FlateUtility.FlateDecode(buffer, true)))
            using (XmlReader reader = XmlReader.Create(stream, _settings))
            {
                font = new CT_SimpleFont();
                reader.ReadToFollowing("SimpleFont");
                font.Ascent = Convert.ToSingle(reader.GetAttribute("Ascent"));
                font.BBox0 = Convert.ToSingle(reader.GetAttribute("BBox0"));
                font.BBox1 = Convert.ToSingle(reader.GetAttribute("BBox1"));
                font.BBox2 = Convert.ToSingle(reader.GetAttribute("BBox2"));
                font.BBox3 = Convert.ToSingle(reader.GetAttribute("BBox3"));
                font.CapHeight = Convert.ToSingle(reader.GetAttribute("CapHeight"));
                font.Descent = Convert.ToSingle(reader.GetAttribute("Descent"));
                font.Flags = Convert.ToInt32(reader.GetAttribute("Flags"));
                font.ItalicAngle = Convert.ToSingle(reader.GetAttribute("ItalicAngle"));
                font.Name = reader.GetAttribute("Name");
                font.StemV = Convert.ToSingle(reader.GetAttribute("StemV"));

                List<CT_Width> widths = new List<CT_Width>();
                while (reader.Read() && reader.Name == "Widths" && reader.NodeType != XmlNodeType.EndElement)
                {
                    CT_Width w = new CT_Width();
                    w.Index = Convert.ToInt32(reader.GetAttribute("Index"));
                    w.Value = Convert.ToSingle(reader.GetAttribute("Value"));
                    widths.Add(w);
                }
                font.Widths = widths.ToArray();
            }
            this.simpleFont = font;
            this.InitWidths();
        }

        /// <summary>
        /// Creates the name of the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static SimpleTrueTypeFont CreateByName(string name)
        {
            SimpleTrueTypeFont font;
            if (!_dict.TryGetValue(name, out font))
            {
                using (Stream stream = FlateUtility.GetResource(name))
                {
                    font = new SimpleTrueTypeFont(stream);
                    _dict[name] = font;
                }
            }
            return font;
        }

        /// <summary>
        /// Gets the ascent.
        /// </summary>
        /// <returns></returns>
        public override float GetAscent()
        {
            return this.simpleFont.Ascent;
        }

        /// <summary>
        /// Gets the height of the capital letter.
        /// </summary>
        /// <returns></returns>
        public override float GetCapitalLetterHeight()
        {
            return this.simpleFont.CapHeight;
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
        /// Gets the descent.
        /// </summary>
        /// <returns></returns>
        public override float GetDescent()
        {
            return this.simpleFont.Descent;
        }

        /// <summary>
        /// Gets the flag.
        /// </summary>
        /// <returns></returns>
        public override PdfFontDescriptor.Flag GetFlag()
        {
            PdfFontDescriptor.Flag flag = base.GetFlag();
            int flags = this.simpleFont.Flags;
            if ((flags / 0x40000) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.ForceBold;
                flags = flags % 0x40000;
            }
            if ((flags / 0x20000) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.SmallCap;
                flags = flags % 0x20000;
            }
            if ((flags / 0x10000) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.AllCap;
                flags = flags % 0x10000;
            }
            if ((flags / 0x40) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.Italic;
                flags = flags % 0x40;
            }
            if ((flags / 0x20) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.Nonsymbolic;
                flags = flags % 0x20;
            }
            if ((flags / 8) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.Script;
                flags = flags % 8;
            }
            if ((flags / 4) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.Symbolic;
                flags = flags % 4;
            }
            if ((flags / 2) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.Serif;
                flags = flags % 2;
            }
            if ((flags / 1) >= 1)
            {
                flag |= PdfFontDescriptor.Flag.FixedPitch;
                flags = flags % 1;
            }
            return flag;
        }

        /// <summary>
        /// Gets the font bounding box.
        /// </summary>
        /// <returns></returns>
        public override PdfRectangle GetFontBoundingBox()
        {
            return new PdfRectangle(this.simpleFont.BBox0, this.simpleFont.BBox1, this.simpleFont.BBox2, this.simpleFont.BBox3);
        }

        /// <summary>
        /// Gets the index of the glyph.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public override int GetGlyphIndex(char c)
        {
            return c;
        }

        /// <summary>
        /// Gets the italic angle.
        /// </summary>
        /// <returns></returns>
        public override float GetItalicAngle()
        {
            return this.simpleFont.ItalicAngle;
        }

        /// <summary>
        /// Gets the name of the post script.
        /// </summary>
        /// <returns></returns>
        protected override string GetPostScriptName()
        {
            if (!string.IsNullOrEmpty(this.simpleFont.Name))
            {
                return this.simpleFont.Name;
            }
            return "MicrosoftSansSerif";
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
        /// Gets the vertical stem.
        /// </summary>
        /// <returns></returns>
        public override float GetVerticalStem()
        {
            return this.simpleFont.StemV;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override float GetWidth(int index)
        {
            if (this.widths.ContainsKey(index))
            {
                return this.widths[index];
            }
            return this.GetDefaultWidth();
        }

        /// <summary>
        /// Inits the widths.
        /// </summary>
        private void InitWidths()
        {
            if ((this.simpleFont.Widths != null) && (this.simpleFont.Widths.Length > 0))
            {
                foreach (CT_Width width in this.simpleFont.Widths)
                {
                    if (!widths.ContainsKey(width.Index))
                        widths.Add(width.Index, width.Value);
                }
            }
        }

        /// <summary>
        /// Gets the arial black font.
        /// </summary>
        /// <value>The arial black.</value>
        public static SimpleTrueTypeFont ArialBlack
        {
            get { return CreateByName("ariblk.sfont"); }
        }

        /// <summary>
        /// Gets the arial MT font.
        /// </summary>
        /// <value>The arial MT.</value>
        public static SimpleTrueTypeFont ArialMT
        {
            get { return CreateByName("arial.sfont"); }
        }

        /// <summary>
        /// Gets the arial narrow font.
        /// </summary>
        /// <value>The arial narrow.</value>
        public static SimpleTrueTypeFont ArialNarrow
        {
            get { return CreateByName("arialn.sfont"); }
        }

        public static SimpleTrueTypeFont Batang
        {
            get
            {
                SimpleTrueTypeFont font = CreateByName("batang.sfont");
                font.Encoding = "UniKS-UCS2-H";
                return font;
            }
        }

        /// <summary>
        /// Gets the comic sans MS font.
        /// </summary>
        /// <value>The comic sans MS.</value>
        public static SimpleTrueTypeFont ComicSansMS
        {
            get { return CreateByName("comic.sfont"); }
        }

        /// <summary>
        /// Gets the courier new PSMT font.
        /// </summary>
        /// <value>The courier new PSMT.</value>
        public static SimpleTrueTypeFont CourierNewPSMT
        {
            get { return CreateByName("cour.sfont"); }
        }

        /// <summary>
        /// Gets the georgia font.
        /// </summary>
        /// <value>The georgia.</value>
        public static SimpleTrueTypeFont Georgia
        {
            get { return CreateByName("georgia.sfont"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is embedded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is embedded; otherwise, <c>false</c>.
        /// </value>
        public override bool IsEmbedded
        {
            get { return false; }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is sub set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is sub set; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSubSet
        {
            get { return false; }
            set
            {
            }
        }

        /// <summary>
        /// Gets the lucida sans unicode font.
        /// </summary>
        /// <value>The lucida sans unicode.</value>
        public static SimpleTrueTypeFont LucidaSansUnicode
        {
            get { return CreateByName("l_10646.sfont"); }
        }

        /// <summary>
        /// Gets the microsoft sans serif font.
        /// </summary>
        /// <value>The microsoft sans serif.</value>
        public static SimpleTrueTypeFont MicrosoftSansSerif
        {
            get { return CreateByName("micross.sfont"); }
        }

        public static SimpleTrueTypeFont MSMincho
        {
            get
            {
                SimpleTrueTypeFont font = CreateByName("msmincho.sfont");
                font.Encoding = "UniJIS-UCS2-H";
                return font;
            }
        }

        public static SimpleTrueTypeFont SimSun
        {
            get
            {
                SimpleTrueTypeFont font = CreateByName("simsun.sfont");
                font.Encoding = "UniGB-UCS2-H";
                return font;
            }
        }

        /// <summary>
        /// Gets the times new roman PSMT font.
        /// </summary>
        /// <value>The times new roman PSMT.</value>
        public static SimpleTrueTypeFont TimesNewRomanPSMT
        {
            get { return CreateByName("times.sfont"); }
        }

        /// <summary>
        /// Gets the trebuchet MS font.
        /// </summary>
        /// <value>The trebuchet MS.</value>
        public static SimpleTrueTypeFont TrebuchetMS
        {
            get { return CreateByName("trebuc.sfont"); }
        }

        /// <summary>
        /// Gets the verdana.
        /// </summary>
        /// <value>The verdana.</value>
        public static SimpleTrueTypeFont Verdana
        {
            get { return CreateByName("verdana.sfont"); }
        }

        /// <summary>
        /// Gets the webdings font.
        /// </summary>
        /// <value>The webdings.</value>
        public static SimpleTrueTypeFont Webdings
        {
            get { return CreateByName("webdings.sfont"); }
        }
    }
}

