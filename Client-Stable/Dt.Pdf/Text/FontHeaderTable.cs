#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using System;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// Font header table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    public class FontHeaderTable : OpenTypeFontTable
    {
        private long checkSumAdjustment;
        private int flags;
        private float fontRevision;
        private short locaVersion;
        private MacStyles macStyle;
        private int unitsPerEm;
        private float version;
        private short xMax;
        private short xMin;
        private short yMax;
        private short yMin;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FontHeaderTable" /> class.
        /// </summary>
        public FontHeaderTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FontHeaderTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public FontHeaderTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
            base.Load();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            this.version = base.reader.ReadFixed();
            this.fontRevision = base.reader.ReadFixed();
            this.checkSumAdjustment = base.reader.ReadULONG();
            if (base.reader.ReadULONG() != 0x5f0f3cf5L)
            {
                throw new PdfReadFontException("magic number of header not match.");
            }
            this.flags = base.reader.ReadUSHORT();
            this.unitsPerEm = base.reader.ReadUSHORT();
            base.reader.Skip(0x10L);
            this.xMin = base.reader.ReadSHORT();
            this.yMin = base.reader.ReadSHORT();
            this.xMax = base.reader.ReadSHORT();
            this.yMax = base.reader.ReadSHORT();
            this.macStyle = (MacStyles)(base.reader.ReadUSHORT() & 0xfffffff);
            base.reader.Skip(4L);
            this.locaVersion = base.reader.ReadSHORT();
            base.LoadData();
        }

        /// <summary>
        /// Gets the check sum adjustment.
        /// </summary>
        /// <value>The check sum adjustment.</value>
        public long CheckSumAdjustment
        {
            get { return  this.checkSumAdjustment; }
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        public int Flags
        {
            get { return  this.flags; }
        }

        /// <summary>
        /// Gets the font revision.
        /// </summary>
        /// <value>The font revision.</value>
        public float FontRevision
        {
            get { return  this.fontRevision; }
        }

        /// <summary>
        /// Gets the loca version.
        /// </summary>
        /// <value>The loca version.</value>
        public short LocaVersion
        {
            get { return  this.locaVersion; }
        }

        /// <summary>
        /// Gets the mac style.
        /// </summary>
        /// <value>The mac style.</value>
        public MacStyles MacStyle
        {
            get { return  this.macStyle; }
        }

        /// <summary>
        /// Gets the units per em.
        /// </summary>
        /// <value>The units per em.</value>
        public int UnitsPerEm
        {
            get { return  this.unitsPerEm; }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public float Version
        {
            get { return  this.version; }
        }

        /// <summary>
        /// Gets the X max.
        /// </summary>
        /// <value>The X max.</value>
        public short XMax
        {
            get { return  this.xMax; }
        }

        /// <summary>
        /// Gets the X min.
        /// </summary>
        /// <value>The X min.</value>
        public short XMin
        {
            get { return  this.xMin; }
        }

        /// <summary>
        /// Gets the Y max.
        /// </summary>
        /// <value>The Y max.</value>
        public short YMax
        {
            get { return  this.yMax; }
        }

        /// <summary>
        /// Gets the Y min.
        /// </summary>
        /// <value>The Y min.</value>
        public short YMin
        {
            get { return  this.yMin; }
        }

        /// <summary>
        /// The Mac styles
        /// </summary>
        [Flags]
        public enum MacStyles
        {
            /// <summary>
            /// Bold
            /// </summary>
            Bold = 1,
            /// <summary>
            /// Condensed
            /// </summary>
            Condensed = 0x20,
            /// <summary>
            /// Extended
            /// </summary>
            Extended = 0x40,
            /// <summary>
            /// Italic
            /// </summary>
            Italic = 2,
            /// <summary>
            /// Outline
            /// </summary>
            Outline = 8,
            /// <summary>
            /// Shadow
            /// </summary>
            Shadow = 0x10,
            /// <summary>
            /// Underline
            /// </summary>
            Underline = 4
        }
    }
}

