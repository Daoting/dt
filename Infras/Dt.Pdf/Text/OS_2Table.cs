#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The OS/2 table
    /// more info: http://www.microsoft.com/typography/OTSPEC/os2.htm
    /// </summary>
    public class OS_2Table : OpenTypeFontTable
    {
        private byte[] achVendID;
        private static readonly int[] codePages = new int[] { 
            0x4e4, 0x4e2, 0x4e3, 0x4e5, 0x4e6, 0x4e7, 0x4e8, 0x4e9, 0x4ea, 0, 0, 0, 0, 0, 0, 0, 
            0x36a, 0x3a4, 0x3a8, 0x3b5, 950, 0x551, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x4ea, 
            0x365, 0x362, 0x361, 0x360, 0x35f, 0x35e, 0x35d, 860, 0x359, 0x357, 0x354, 0x307, 0x2e1, 0x2c4, 850, 0x1b5
         };
        private int fsSelection;
        private int fsType;
        private FontHeaderTable headerTable;
        private byte[] panose;
        private short sCapHeight;
        private short sFamilyClass;
        private short sTypoAscender;
        private short sTypoDescender;
        private short sTypoLineGap;
        private short sxHeight;
        private long ulCodePageRange1;
        private long ulCodePageRange2;
        private long ulUnicodeRange1;
        private long ulUnicodeRange2;
        private long ulUnicodeRange3;
        private long ulUnicodeRange4;
        private int usBreakChar;
        private int usDefaultChar;
        private int usFirstCharIndex;
        private int usLastCharIndex;
        private int usMaxContent;
        private int usWeightClass;
        private int usWidthClass;
        private int usWinAscent;
        private int usWinDescent;
        private int version;
        private short xAvgCharWidth;
        private short yStrikeoutPosition;
        private short yStrikeoutSize;
        private short ySubscriptXOffset;
        private short ySubscriptXSize;
        private short ySubscriptYOffset;
        private short ySubscriptYSize;
        private short ySuperscriptXOffset;
        private short ySuperscriptXSize;
        private short ySuperscriptYOffset;
        private short ySuperscriptYSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OS_2Table" /> class.
        /// </summary>
        public OS_2Table()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OS_2Table" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public OS_2Table(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            this.version = base.reader.ReadUSHORT();
            this.xAvgCharWidth = base.reader.ReadSHORT();
            this.usWeightClass = base.reader.ReadUSHORT();
            this.usWidthClass = base.reader.ReadUSHORT();
            this.fsType = base.reader.ReadUSHORT();
            this.ySubscriptXSize = base.reader.ReadSHORT();
            this.ySubscriptYSize = base.reader.ReadSHORT();
            this.ySubscriptXOffset = base.reader.ReadSHORT();
            this.ySubscriptYOffset = base.reader.ReadSHORT();
            this.ySuperscriptXSize = base.reader.ReadSHORT();
            this.ySuperscriptYSize = base.reader.ReadSHORT();
            this.ySuperscriptXOffset = base.reader.ReadSHORT();
            this.ySuperscriptYOffset = base.reader.ReadSHORT();
            this.yStrikeoutSize = base.reader.ReadSHORT();
            this.yStrikeoutPosition = base.reader.ReadSHORT();
            this.sFamilyClass = base.reader.ReadSHORT();
            this.panose = base.reader.ReadBytes(10);
            this.ulUnicodeRange1 = base.reader.ReadULONG();
            this.ulUnicodeRange2 = base.reader.ReadULONG();
            this.ulUnicodeRange3 = base.reader.ReadULONG();
            this.ulUnicodeRange4 = base.reader.ReadULONG();
            this.achVendID = base.reader.ReadBytes(4);
            this.fsSelection = base.reader.ReadUSHORT();
            this.usFirstCharIndex = base.reader.ReadUSHORT();
            this.usLastCharIndex = base.reader.ReadUSHORT();
            this.sTypoAscender = base.reader.ReadSHORT();
            this.sTypoDescender = base.reader.ReadSHORT();
            if (this.sTypoDescender > 0)
            {
                this.sTypoDescender = (short) -this.sTypoDescender;
            }
            this.sTypoLineGap = base.reader.ReadSHORT();
            this.usWinAscent = base.reader.ReadUSHORT();
            this.usWinDescent = base.reader.ReadUSHORT();
            if (this.version > 0)
            {
                this.ulCodePageRange1 = base.reader.ReadULONG();
                this.ulCodePageRange2 = base.reader.ReadULONG();
            }
            if (this.version > 1)
            {
                this.sxHeight = base.reader.ReadSHORT();
                this.sCapHeight = base.reader.ReadSHORT();
            }
            else
            {
                this.sCapHeight = (short)(this.headerTable.UnitsPerEm * 0.7);
            }
            this.usDefaultChar = base.reader.ReadUSHORT();
            this.usBreakChar = base.reader.ReadUSHORT();
            this.usMaxContent = base.reader.ReadUSHORT();
            base.LoadData();
        }

        /// <summary>
        /// Gets the ach vend ID.
        /// </summary>
        /// <value>The ach vend ID.</value>
        public byte[] AchVendID
        {
            get
            {
                base.CheckLoad();
                return this.achVendID;
            }
        }

        /// <summary>
        /// Gets the fs selection.
        /// </summary>
        /// <value>The fs selection.</value>
        public int FsSelection
        {
            get
            {
                base.CheckLoad();
                return this.fsSelection;
            }
        }

        /// <summary>
        /// Gets the type of the fs.
        /// </summary>
        /// <value>The type of the fs.</value>
        public int FsType
        {
            get
            {
                base.CheckLoad();
                return this.fsType;
            }
        }

        /// <summary>
        /// Gets or sets the header table.
        /// </summary>
        /// <value>The header table.</value>
        public FontHeaderTable HeaderTable
        {
            get { return  this.headerTable; }
            set { this.headerTable = value; }
        }

        /// <summary>
        /// Gets the panose.
        /// </summary>
        /// <value>The panose.</value>
        public byte[] Panose
        {
            get
            {
                base.CheckLoad();
                return this.panose;
            }
        }

        /// <summary>
        /// Gets the height of the S cap.
        /// </summary>
        /// <value>The height of the S cap.</value>
        public short SCapHeight
        {
            get
            {
                base.CheckLoad();
                return this.sCapHeight;
            }
        }

        /// <summary>
        /// Gets the S family class.
        /// </summary>
        /// <value>The S family class.</value>
        public short SFamilyClass
        {
            get
            {
                base.CheckLoad();
                return this.sFamilyClass;
            }
        }

        /// <summary>
        /// Gets the S typo ascender.
        /// </summary>
        /// <value>The S typo ascender.</value>
        public short STypoAscender
        {
            get
            {
                base.CheckLoad();
                return this.sTypoAscender;
            }
        }

        /// <summary>
        /// Gets the S typo descender.
        /// </summary>
        /// <value>The S typo descender.</value>
        public short STypoDescender
        {
            get
            {
                base.CheckLoad();
                return this.sTypoDescender;
            }
        }

        /// <summary>
        /// Gets the S typo line gap.
        /// </summary>
        /// <value>The S typo line gap.</value>
        public short STypoLineGap
        {
            get
            {
                base.CheckLoad();
                return this.sTypoLineGap;
            }
        }

        /// <summary>
        /// Gets the supported code pages.
        /// </summary>
        /// <value>The supported code pages.</value>
        public int[] SupportedCodePages
        {
            get
            {
                base.CheckLoad();
                long num = (this.ulCodePageRange2 << 0x20) + (this.ulCodePageRange1 & 0xffffffffL);
                int num2 = 0;
                long num3 = 1L;
                for (int i = 0; i < 0x40; i++)
                {
                    if (((num & num3) != 0L) && (codePages[i] != 0))
                    {
                        num2++;
                    }
                    num3 = num3 << 1;
                }
                int[] numArray = new int[num2];
                num2 = 0;
                num3 = 1L;
                for (int j = 0; j < 0x40; j++)
                {
                    if (((num & num3) != 0L) && (codePages[j] != 0))
                    {
                        numArray[num2++] = codePages[j];
                    }
                    num3 = num3 << 1;
                }
                return numArray;
            }
        }

        /// <summary>
        /// Gets the height of the sx.
        /// </summary>
        /// <value>The height of the sx.</value>
        public short SxHeight
        {
            get
            {
                base.CheckLoad();
                return this.sxHeight;
            }
        }

        /// <summary>
        /// Gets the ul code page range1.
        /// </summary>
        /// <value>The ul code page range1.</value>
        public long UlCodePageRange1
        {
            get
            {
                base.CheckLoad();
                return this.ulCodePageRange1;
            }
        }

        /// <summary>
        /// Gets the ul code page range2.
        /// </summary>
        /// <value>The ul code page range2.</value>
        public long UlCodePageRange2
        {
            get
            {
                base.CheckLoad();
                return this.ulCodePageRange2;
            }
        }

        /// <summary>
        /// Gets the ul unicode range1.
        /// </summary>
        /// <value>The ul unicode range1.</value>
        public long UlUnicodeRange1
        {
            get
            {
                base.CheckLoad();
                return this.ulUnicodeRange1;
            }
        }

        /// <summary>
        /// Gets the ul unicode range2.
        /// </summary>
        /// <value>The ul unicode range2.</value>
        public long UlUnicodeRange2
        {
            get
            {
                base.CheckLoad();
                return this.ulUnicodeRange2;
            }
        }

        /// <summary>
        /// Gets the ul unicode range3.
        /// </summary>
        /// <value>The ul unicode range3.</value>
        public long UlUnicodeRange3
        {
            get
            {
                base.CheckLoad();
                return this.ulUnicodeRange3;
            }
        }

        /// <summary>
        /// Gets the ul unicode range4.
        /// </summary>
        /// <value>The ul unicode range4.</value>
        public long UlUnicodeRange4
        {
            get
            {
                base.CheckLoad();
                return this.ulUnicodeRange4;
            }
        }

        /// <summary>
        /// Gets the us break char.
        /// </summary>
        /// <value>The us break char.</value>
        public int UsBreakChar
        {
            get
            {
                base.CheckLoad();
                return this.usBreakChar;
            }
        }

        /// <summary>
        /// Gets the us default char.
        /// </summary>
        /// <value>The us default char.</value>
        public int UsDefaultChar
        {
            get
            {
                base.CheckLoad();
                return this.usDefaultChar;
            }
        }

        /// <summary>
        /// Gets the index of the us first char.
        /// </summary>
        /// <value>The index of the us first char.</value>
        public int UsFirstCharIndex
        {
            get
            {
                base.CheckLoad();
                return this.usFirstCharIndex;
            }
        }

        /// <summary>
        /// Gets the index of the us last char.
        /// </summary>
        /// <value>The index of the us last char.</value>
        public int UsLastCharIndex
        {
            get
            {
                base.CheckLoad();
                return this.usLastCharIndex;
            }
        }

        /// <summary>
        /// Gets the content of the us max.
        /// </summary>
        /// <value>The content of the us max.</value>
        public int UsMaxContent
        {
            get
            {
                base.CheckLoad();
                return this.usMaxContent;
            }
        }

        /// <summary>
        /// Gets the us weight class.
        /// </summary>
        /// <value>The us weight class.</value>
        public int UsWeightClass
        {
            get
            {
                base.CheckLoad();
                return this.usWeightClass;
            }
        }

        /// <summary>
        /// Gets the us width class.
        /// </summary>
        /// <value>The us width class.</value>
        public int UsWidthClass
        {
            get
            {
                base.CheckLoad();
                return this.usWidthClass;
            }
        }

        /// <summary>
        /// Gets the us win ascent.
        /// </summary>
        /// <value>The us win ascent.</value>
        public int UsWinAscent
        {
            get
            {
                base.CheckLoad();
                return this.usWinAscent;
            }
        }

        /// <summary>
        /// Gets the us win descent.
        /// </summary>
        /// <value>The us win descent.</value>
        public int UsWinDescent
        {
            get
            {
                base.CheckLoad();
                return this.usWinDescent;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version
        {
            get
            {
                base.CheckLoad();
                return this.version;
            }
        }

        /// <summary>
        /// Gets the width of the X avg char.
        /// </summary>
        /// <value>The width of the X avg char.</value>
        public short XAvgCharWidth
        {
            get
            {
                base.CheckLoad();
                return this.xAvgCharWidth;
            }
        }

        /// <summary>
        /// Gets the Y strikeout position.
        /// </summary>
        /// <value>The Y strikeout position.</value>
        public short YStrikeoutPosition
        {
            get
            {
                base.CheckLoad();
                return this.yStrikeoutPosition;
            }
        }

        /// <summary>
        /// Gets the size of the Y strikeout.
        /// </summary>
        /// <value>The size of the Y strikeout.</value>
        public short YStrikeoutSize
        {
            get
            {
                base.CheckLoad();
                return this.yStrikeoutSize;
            }
        }

        /// <summary>
        /// Gets the Y subscript X offset.
        /// </summary>
        /// <value>The Y subscript X offset.</value>
        public short YSubscriptXOffset
        {
            get
            {
                base.CheckLoad();
                return this.ySubscriptXOffset;
            }
        }

        /// <summary>
        /// Gets the size of the Y subscript X.
        /// </summary>
        /// <value>The size of the Y subscript X.</value>
        public short YSubscriptXSize
        {
            get
            {
                base.CheckLoad();
                return this.ySubscriptXSize;
            }
        }

        /// <summary>
        /// Gets the Y subscript Y offset.
        /// </summary>
        /// <value>The Y subscript Y offset.</value>
        public short YSubscriptYOffset
        {
            get
            {
                base.CheckLoad();
                return this.ySubscriptYOffset;
            }
        }

        /// <summary>
        /// Gets the size of the Y subscript Y.
        /// </summary>
        /// <value>The size of the Y subscript Y.</value>
        public short YSubscriptYSize
        {
            get
            {
                base.CheckLoad();
                return this.ySubscriptYSize;
            }
        }

        /// <summary>
        /// Gets the Y superscript X offset.
        /// </summary>
        /// <value>The Y superscript X offset.</value>
        public short YSuperscriptXOffset
        {
            get
            {
                base.CheckLoad();
                return this.ySuperscriptXOffset;
            }
        }

        /// <summary>
        /// Gets the size of the Y superscript X.
        /// </summary>
        /// <value>The size of the Y superscript X.</value>
        public short YSuperscriptXSize
        {
            get
            {
                base.CheckLoad();
                return this.ySuperscriptXSize;
            }
        }

        /// <summary>
        /// Gets the Y superscript Y offset.
        /// </summary>
        /// <value>The Y superscript Y offset.</value>
        public short YSuperscriptYOffset
        {
            get
            {
                base.CheckLoad();
                return this.ySuperscriptYOffset;
            }
        }

        /// <summary>
        /// Gets the size of the Y superscript Y.
        /// </summary>
        /// <value>The size of the Y superscript Y.</value>
        public short YSuperscriptYSize
        {
            get
            {
                base.CheckLoad();
                return this.ySuperscriptYSize;
            }
        }
    }
}

