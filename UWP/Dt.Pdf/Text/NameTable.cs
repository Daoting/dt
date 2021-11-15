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
    /// The name table table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    internal class NameTable : OpenTypeFontTable
    {
        public const int CopyrightId = 0;
        public const int FontFamilyNameId = 1;
        public const int FontSubFamilyNameId = 2;
        public const int FullFontName = 4;
        private readonly Dictionary<int, string> names;
        public const int PostScriptName = 6;
        private readonly Dictionary<int, string> uniNames;
        public const int UniqueFontIdentifier = 3;
        public const int Version = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NameTable" /> class.
        /// </summary>
        public NameTable()
        {
            this.names = new Dictionary<int, string>();
            this.uniNames = new Dictionary<int, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NameTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public NameTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
            this.names = new Dictionary<int, string>();
            this.uniNames = new Dictionary<int, string>();
            base.Load();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            base.reader.Skip(2L);
            int num = base.reader.ReadUSHORT();
            int num2 = base.reader.ReadUSHORT();
            for (int i = 0; i < num; i++)
            {
                string str;
                int num4 = base.reader.ReadUSHORT();
                int num5 = base.reader.ReadUSHORT();
                base.reader.ReadUSHORT();
                int num6 = base.reader.ReadUSHORT();
                int len = base.reader.ReadUSHORT();
                int num8 = base.reader.ReadUSHORT();
                long position = base.reader.Position;
                base.reader.Seek((long) (num2 + num8));
                if (((num4 == 0) || (num4 == 3)) || ((num4 == 2) && (num5 == 1)))
                {
                    str = base.reader.ReadBigEndianUnicodeString(len);
                    this.uniNames[num6] = str;
                }
                else
                {
                    str = base.reader.ReadStandardString(len);
                    this.names[num6] = str;
                }
                base.reader.Seek(position);
            }
            base.LoadData();
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>The names.</value>
        public Dictionary<int, string> Names
        {
            get
            {
                base.CheckLoad();
                return this.names;
            }
        }

        /// <summary>
        /// Gets the unicode names.
        /// </summary>
        /// <value>The unicode names.</value>
        public Dictionary<int, string> UnicodeNames
        {
            get
            {
                base.CheckLoad();
                return this.uniNames;
            }
        }
    }
}

