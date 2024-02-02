#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The glyph table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    internal class GlyphTable : OpenTypeFontTable
    {
        private const int arg1And2AreWords = 1;
        private LocationTable locaTable;
        private const int moreComponents = 0x20;
        private readonly List<int> usedGlyph;
        private const int weHaveA2By2 = 0x80;
        private const int weHaveAnXAndYScale = 0x40;
        private const int weHaveAScale = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GlyphTable" /> class.
        /// </summary>
        public GlyphTable()
        {
            this.usedGlyph = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GlyphTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public GlyphTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
            this.usedGlyph = new List<int>();
        }

        /// <summary>
        /// Checks the glyph composite.
        /// </summary>
        /// <param name="glyf">The glyf.</param>
        private void CheckGlyphComposite(int glyf)
        {
            int num = this.locaTable[glyf];
            if (num == this.locaTable[glyf + 1])
            {
                return;
            }
            base.reader.Seek((long) num);
            if (base.reader.ReadSHORT() >= 0)
            {
                return;
            }
            base.reader.Skip(8L);
            while (true)
            {
                int num5;
                int num3 = base.reader.ReadUSHORT();
                int item = base.reader.ReadUSHORT();
                if (!this.usedGlyph.Contains(item))
                {
                    this.usedGlyph.Add(item);
                }
                if ((num3 & 0x20) == 0)
                {
                    return;
                }
                if ((num3 & 1) != 0)
                {
                    num5 = 4;
                }
                else
                {
                    num5 = 2;
                }
                if ((num3 & 8) != 0)
                {
                    num5 += 2;
                }
                else if ((num3 & 0x40) != 0)
                {
                    num5 += 4;
                }
                if ((num3 & 0x80) != 0)
                {
                    num5 += 8;
                }
                base.reader.Skip((long) num5);
            }
        }

        /// <summary>
        /// Creates the new glyph tables.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        private List<int> CreateNewGlyphTables(OpenTypeFontWriter writer)
        {
            List<int> list = new List<int>();
            this.usedGlyph.Sort();
            int item = 0;
            int num2 = 0;
            for (int i = 0; i < this.locaTable.Count; i++)
            {
                list.Add(item);
                if ((num2 < this.usedGlyph.Count) && (this.usedGlyph[num2] == i))
                {
                    num2++;
                    list[i] = item;
                    int num4 = this.locaTable[i];
                    int length = this.locaTable[i + 1] - num4;
                    if (length > 0)
                    {
                        base.reader.Seek((long) num4);
                        writer.WriteBytes(base.reader.ReadBytes(length));
                        item += length;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the subset.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <returns></returns>
        public override OpenTypeFontTable GetSubset(Dictionary<char, int> chars)
        {
            if (chars == null)
            {
                return base.GetSubset(chars);
            }
            base.CheckReader();
            base.reader.Seek(0L);
            Dictionary<int, char> dictionary = new Dictionary<int, char>();
            foreach (KeyValuePair<char, int> pair in chars)
            {
                dictionary[pair.Value] = pair.Key;
            }
            this.usedGlyph.AddRange(dictionary.Keys);
            if (!this.usedGlyph.Contains(0))
            {
                this.usedGlyph.Add(0);
            }
            for (int i = 0; i < this.usedGlyph.Count; i++)
            {
                int glyf = this.usedGlyph[i];
                if (glyf >= 0)
                {
                    this.CheckGlyphComposite(glyf);
                }
            }
            OpenTypeFontWriter writer = new OpenTypeFontWriter();
            this.locaTable.NewLoca = this.CreateNewGlyphTables(writer);
            return new GlyphTable(base.Tag, 0L, 0L, writer.Length, new OpenTypeFontReader(writer.ToArray()));
        }

        /// <summary>
        /// Gets or sets the loca table.
        /// </summary>
        /// <value>The loca table.</value>
        public LocationTable LocaTable
        {
            get { return  this.locaTable; }
            set { this.locaTable = value; }
        }

        /// <summary>
        /// Gets the used glyph.
        /// </summary>
        /// <value>The used glyph.</value>
        public List<int> UsedGlyph
        {
            get { return  this.usedGlyph; }
        }
    }
}

