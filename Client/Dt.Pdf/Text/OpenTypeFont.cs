#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Drawing;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Object;
using Dt.Pdf.Object.Filter;
using Dt.Pdf.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The open type font object
    /// </summary>
    public class OpenTypeFont : BaseFont
    {
        private const string cffTable = "CFF ";
        private const string cmapTable = "cmap";
        private const string controlValueTable = "cvt ";
        private const string cvtProgramTable = "prep";
        private const string fontProgramTable = "fpgm";
        private const string glyphTable = "glyf";
        private const string headerTable = "head";
        private const string horizontalHeaderTable = "hhea";
        private const string horizontalMetricsTable = "hmtx";
        private const string locationTable = "loca";
        private const string maximumProfileTable = "maxp";
        private const string nameTable = "name";
        private const string os_2Table = "OS/2";
        private const string postScriptTable = "post";
        private readonly OpenTypeFontReader reader;
        private string sfntVersion;
        private readonly Dictionary<string, OpenTypeFontTable> tables = new Dictionary<string, OpenTypeFontTable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OpenTypeFont" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private OpenTypeFont(OpenTypeFontReader reader)
        {
            if (reader == null)
            {
                throw new PdfArgumentNullException("reader");
            }
            this.reader = reader;
            this.InitFontTables();
        }

        /// <summary>
        /// Gets the ascent.
        /// </summary>
        /// <returns></returns>
        public override float GetAscent()
        {
            return ((this.OS_2Table.STypoAscender * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm));
        }

        /// <summary>
        /// Gets the height of the capital letter.
        /// </summary>
        /// <returns></returns>
        public override float GetCapitalLetterHeight()
        {
            return ((this.OS_2Table.SCapHeight * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm));
        }

        /// <summary>
        /// Gets the code pages.
        /// </summary>
        /// <returns></returns>
        public override int[] GetCodePages()
        {
            return this.OS_2Table.SupportedCodePages;
        }

        /// <summary>
        /// Gets the default width.
        /// </summary>
        /// <returns></returns>
        public override float GetDefaultWidth()
        {
            return ((this.GetTable<HorizontalHeaderTable>("hhea").AdvanceWidthMax * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm));
        }

        /// <summary>
        /// Gets the descent.
        /// </summary>
        /// <returns></returns>
        public override float GetDescent()
        {
            return ((this.OS_2Table.STypoDescender * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm));
        }

        /// <summary>
        /// Gets the flag.
        /// </summary>
        /// <returns></returns>
        public override PdfFontDescriptor.Flag GetFlag()
        {
            PdfFontDescriptor.Flag flag = this.IsSymbolic ? PdfFontDescriptor.Flag.Symbolic : PdfFontDescriptor.Flag.Nonsymbolic;
            if (this.tables.ContainsKey("post") && this.GetTable<PostScriptTable>("post").IsFixedPicth)
            {
                flag |= PdfFontDescriptor.Flag.FixedPitch;
            }
            FontHeaderTable table = this.GetTable<FontHeaderTable>("head");
            if ((table.MacStyle & FontHeaderTable.MacStyles.Italic) == FontHeaderTable.MacStyles.Italic)
            {
                flag |= PdfFontDescriptor.Flag.Italic;
            }
            if ((table.MacStyle & FontHeaderTable.MacStyles.Bold) == FontHeaderTable.MacStyles.Bold)
            {
                flag |= PdfFontDescriptor.Flag.ForceBold;
            }
            return flag;
        }

        /// <summary>
        /// Gets the font bounding box.
        /// </summary>
        /// <returns></returns>
        public override PdfRectangle GetFontBoundingBox()
        {
            return new PdfRectangle((this.FontHeaderTable.XMin * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm), (this.FontHeaderTable.YMin * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm), (this.FontHeaderTable.XMax * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm), (this.FontHeaderTable.YMax * 1000f) / ((float) this.FontHeaderTable.UnitsPerEm));
        }

        /// <summary>
        /// Gets the index of the glyph.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public override int GetGlyphIndex(char c)
        {
            return this.CMap[c];
        }

        /// <summary>
        /// Gets the italic angle.
        /// </summary>
        /// <returns></returns>
        public override float GetItalicAngle()
        {
            if (this.tables.ContainsKey("post"))
            {
                return this.GetTable<PostScriptTable>("post").ItalicAngle;
            }
            return (float) ((-Math.Atan2((double) this.GetTable<HorizontalHeaderTable>("hhea").CaretSlopeRun, (double) this.GetTable<HorizontalHeaderTable>("hhea").CaretSlopeRise) * 180.0) / 3.1415926535897931);
        }

        /// <summary>
        /// Gets the name of the post script.
        /// </summary>
        /// <returns></returns>
        protected override string GetPostScriptName()
        {
            NameTable names = this.Names;
            if (names.Names.ContainsKey(6))
            {
                return names.Names[6];
            }
            return names.UnicodeNames[6];
        }

        /// <summary>
        /// Gets the sub font.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="isCFF">if set to <c>true</c> [is CFF].</param>
        /// <returns></returns>
        public override byte[] GetSubFont(Dictionary<char, int> chars, out bool isCFF)
        {
            if (this.IsCFF)
            {
                isCFF = true;
                OpenTypeFontReader reader = this.tables["CFF "].Reader;
                reader.Seek(0L);
                return reader.ReadBytes((int) reader.Length);
            }
            isCFF = false;
            if (!base.isSubSet)
            {
                return this.reader.ToArray();
            }
            List<string> list = new List<string> { "head", "glyf", "loca" };
            if (base.encoding == "WinAnsiEncoding")
            {
                list.Add("cmap");
            }
            list.Add("hhea");
            list.Add("maxp");
            list.Add("hmtx");
            list.Add("cvt ");
            list.Add("fpgm");
            list.Add("prep");
            if (!base.isSubSet)
            {
                list.Add("OS/2");
                if (!list.Contains("cmap"))
                {
                    list.Add("cmap");
                }
                list.Add("name");
            }
            OpenTypeFontWriter writer = new OpenTypeFontWriter();
            int i = 0;
            int num2 = 0;
            int num3 = 1;
            foreach (string str in list)
            {
                if (this.tables.ContainsKey(str))
                {
                    i++;
                }
            }
            for (int j = 0; j < 0x20; j++)
            {
                if (num3 > i)
                {
                    break;
                }
                num3 = num3 << 1;
                num2++;
            }
            num2--;
            int num5 = (num3 / 2) * 0x10;
            int num6 = (i * 0x10) - num5;
            writer.WriteFixed(1f);
            writer.WriteSHORT(i);
            writer.WriteSHORT(num5);
            writer.WriteSHORT(num2);
            writer.WriteSHORT(num6);
            long position = writer.Position;
            writer.WriteBytes((int) (i * 0x10));
            List<OpenTypeFontTable> list2 = new List<OpenTypeFontTable>();
            foreach (string str2 in list)
            {
                if (this.tables.ContainsKey(str2))
                {
                    OpenTypeFontTable subset = this.tables[str2].GetSubset(chars);
                    long num8 = writer.Position;
                    writer.Write(subset.Reader);
                    list2.Add(subset);
                    subset.Offset = num8;
                }
            }
            writer.Seek(position);
            foreach (OpenTypeFontTable table2 in list2)
            {
                writer.WriteString(table2.Tag);
                writer.WriteLONG(table2.CheckSum);
                writer.WriteLONG(table2.Offset);
                writer.WriteLONG(table2.Length);
            }
            return writer.ToArray();
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal T GetTable<T>(string name) where T: OpenTypeFontTable
        {
            return (this.tables[name] as T);
        }

        /// <summary>
        /// Gets the vertical stem.
        /// </summary>
        /// <returns></returns>
        public override float GetVerticalStem()
        {
            return 80f;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override float GetWidth(int index)
        {
            if (index >= this.GlyphsWidth.Count)
            {
                index = this.GlyphsWidth.Count - 1;
            }
            return this.GlyphsWidth[index];
        }

        /// <summary>
        /// Inits the font tables.
        /// </summary>
        private void InitFontTables()
        {
            switch (this.reader.ReadLONG())
            {
                case 0x10000:
                    this.sfntVersion = "1.0";
                    break;

                case 0x4f54544f:
                    this.sfntVersion = "OTTO";
                    break;

                default:
                    throw new PdfIncorrectFontData();
            }
            int num2 = this.reader.ReadUSHORT();
            this.reader.Skip(6L);
            for (int i = 0; i < num2; i++)
            {
                string key = this.reader.ReadTagString();
                switch (key)
                {
                    case "head":
                        this.tables.Add(key, new FontHeaderTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "hhea":
                        this.tables.Add(key, new HorizontalHeaderTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "name":
                        this.tables.Add(key, new NameTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "cmap":
                        this.tables.Add(key, new CMapTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "hmtx":
                        this.tables.Add(key, new HorizontalMetricsTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "OS/2":
                        this.tables.Add(key, new OS_2Table(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "post":
                        this.tables.Add(key, new PostScriptTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "loca":
                        this.tables.Add(key, new LocationTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    case "glyf":
                        this.tables.Add(key, new GlyphTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;

                    default:
                        this.tables.Add(key, new OpenTypeFontTable(key, this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader.ReadULONG(), this.reader));
                        break;
                }
            }
            FontHeaderTable table = this.GetTable<FontHeaderTable>("head");
            HorizontalMetricsTable table2 = this.GetTable<HorizontalMetricsTable>("hmtx");
            table2.HHeaderTable = this.GetTable<HorizontalHeaderTable>("hhea");
            table2.HeaderTable = table;
            this.GetTable<OS_2Table>("OS/2").HeaderTable = table;
            if (this.tables.ContainsKey("loca"))
            {
                this.GetTable<LocationTable>("loca").HeaderTable = table;
            }
            if (this.tables.ContainsKey("glyf"))
            {
                this.GetTable<GlyphTable>("glyf").LocaTable = this.GetTable<LocationTable>("loca");
            }
            if (base.isEmbedded && (this.OS_2Table.FsType == 2))
            {
                throw new PdfReadFontException("Font has 'Restricted License' for embedding.");
            }
        }

        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static OpenTypeFont Load(OpenTypeFontReader reader)
        {
            return Load(reader, 0);
        }

        /// <summary>
        /// Loads the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static OpenTypeFont Load(byte[] data)
        {
            return Load(data, 0);
        }

        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static OpenTypeFont Load(OpenTypeFontReader reader, int index)
        {
            if (reader == null)
            {
                throw new PdfArgumentNullException("reader");
            }
            long position = reader.Position;
            if (reader.ReadLONG() == 0x74746366)
            {
                reader.ReadFixed();
                long num3 = reader.ReadULONG();
                if ((index < 0) || (index >= num3))
                {
                    throw new PdfReadFontException("index must be bigger than 0 and less than " + ((long) num3) + ".");
                }
                reader.Skip((long) (4 * index));
                long pos = reader.ReadULONG();
                reader.Seek(pos);
            }
            else
            {
                reader.Seek(position);
            }
            return new OpenTypeFont(reader);
        }

        /// <summary>
        /// Loads the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static OpenTypeFont Load(byte[] data, int index)
        {
            return Load(new OpenTypeFontReader(data), index);
        }

        public void SaveAsSimpleFont(Stream stream)
        {
            if (stream == null)
            {
                throw new PdfArgumentNullException("stream");
            }
            if (!stream.CanWrite)
            {
                throw new PdfArgumentException("stream");
            }
            CT_SimpleFont font = new CT_SimpleFont {
                Name = this.GetPostScriptName()
            };
            PdfRectangle fontBoundingBox = this.GetFontBoundingBox();
            font.BBox0 = fontBoundingBox.LowerLeftX;
            font.BBox1 = fontBoundingBox.LowerLeftY;
            font.BBox2 = fontBoundingBox.UpperRightX;
            font.BBox3 = fontBoundingBox.UpperRightY;
            font.Flags = (int) this.GetFlag();
            font.ItalicAngle = this.GetItalicAngle();
            font.Ascent = this.GetAscent();
            font.Descent = this.GetDescent();
            font.CapHeight = this.GetCapitalLetterHeight();
            font.StemV = this.GetVerticalStem();
            List<CT_Width> list = new List<CT_Width>();
            foreach (KeyValuePair<int, int> pair in this.CMap.CurrentMap)
            {
                CT_Width width;
                width = new CT_Width {
                    Index = pair.Key,
                    Value = this.GetWidth(pair.Value)
                };
                list.Add(width);
            }
            font.Widths = list.ToArray();
            IStreamSerializer serializerByType = SerializerBuilder.GetSerializerByType(typeof(CT_SimpleFont));
            MemoryStream stream2 = new MemoryStream();
            serializerByType.Serialize(stream2, font);
            stream2.Seek(0L, SeekOrigin.Begin);
            PdfFilter.FlateFilter.Encode(stream2, stream, null);
        }

        /// <summary>
        /// Gets the Cmap.
        /// </summary>
        /// <value>The Cmap.</value>
        private CMapTable CMap
        {
            get { return  this.GetTable<CMapTable>("cmap"); }
        }

        /// <summary>
        /// Gets the font header table.
        /// </summary>
        /// <value>The font header table.</value>
        public FontHeaderTable FontHeaderTable
        {
            get { return  this.GetTable<FontHeaderTable>("head"); }
        }

        /// <summary>
        /// Gets the width of the glyphs.
        /// </summary>
        /// <value>The width of the glyphs.</value>
        private HorizontalMetricsTable GlyphsWidth
        {
            get { return  this.GetTable<HorizontalMetricsTable>("hmtx"); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is CFF.
        /// </summary>
        /// <value><c>true</c> if this instance is CFF; otherwise, <c>false</c>.</value>
        public bool IsCFF
        {
            get { return  this.tables.ContainsKey("CFF "); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is symbolic.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is symbolic; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSymbolic
        {
            get { return  (this.CMap.SymbolMap != null); }
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>The names.</value>
        private NameTable Names
        {
            get { return  this.GetTable<NameTable>("name"); }
        }

        /// <summary>
        /// Gets the O S_2 table.
        /// </summary>
        /// <value>The O S_2 table.</value>
        public OS_2Table OS_2Table
        {
            get { return  this.GetTable<OS_2Table>("OS/2"); }
        }

        /// <summary>
        /// Gets the SFNT version.
        /// </summary>
        /// <value>The SFNT version.</value>
        public string SfntVersion
        {
            get { return  this.sfntVersion; }
        }

        /// <summary>
        /// Gets the tables.
        /// </summary>
        /// <value>The tables.</value>
        public Dictionary<string, OpenTypeFontTable> Tables
        {
            get { return  this.tables; }
        }
    }
}

