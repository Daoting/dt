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
using System.Reflection;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The location table table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    internal class LocationTable : OpenTypeFontTable
    {
        private FontHeaderTable header;
        private readonly List<int> locaTable;
        private List<int> newLoca;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LocationTable" /> class.
        /// </summary>
        public LocationTable()
        {
            this.locaTable = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LocationTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public LocationTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
            this.locaTable = new List<int>();
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
            OpenTypeFontWriter writer = new OpenTypeFontWriter();
            foreach (int num in this.newLoca)
            {
                if (this.header.LocaVersion == 0)
                {
                    writer.WriteSHORT(num / 2);
                }
                else
                {
                    writer.WriteLONG((long) num);
                }
            }
            return new GlyphTable(base.Tag, 0L, 0L, writer.Length, new OpenTypeFontReader(writer.ToArray()));
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            if (this.header.LocaVersion == 0)
            {
                for (int i = 0; i < (base.Length / 2L); i++)
                {
                    this.locaTable.Add(base.reader.ReadUSHORT() * 2);
                }
            }
            else
            {
                for (int j = 0; j < (base.Length / 4L); j++)
                {
                    this.locaTable.Add(base.reader.ReadLONG());
                }
            }
            base.LoadData();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                base.CheckLoad();
                return this.locaTable.Count;
            }
        }

        /// <summary>
        /// Gets or sets the header table.
        /// </summary>
        /// <value>The header table.</value>
        public FontHeaderTable HeaderTable
        {
            get { return  this.header; }
            set { this.header = value; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Int32" /> at the specified index.
        /// </summary>
        /// <value></value>
        public int this[int index]
        {
            get
            {
                base.CheckLoad();
                return this.locaTable[index];
            }
        }

        /// <summary>
        /// Sets the new loca.
        /// </summary>
        /// <value>The new loca.</value>
        internal List<int> NewLoca
        {
            set { this.newLoca = value; }
        }
    }
}

