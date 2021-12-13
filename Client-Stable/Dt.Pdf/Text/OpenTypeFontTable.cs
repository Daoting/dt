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
    /// The open type font table
    /// </summary>
    public class OpenTypeFontTable
    {
        private long checkSum;
        private bool isLoad;
        private bool isReaderCloned;
        private readonly long length;
        private long offset;
        protected OpenTypeFontReader reader;
        private readonly string tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OpenTypeFontTable" /> class.
        /// </summary>
        public OpenTypeFontTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OpenTypeFontTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public OpenTypeFontTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader)
        {
            this.tag = tag;
            this.checkSum = checkSum;
            this.offset = offset;
            this.length = length;
            this.reader = reader;
        }

        /// <summary>
        /// Checks the load.
        /// </summary>
        public void CheckLoad()
        {
            if (!this.IsLoad)
            {
                this.Load();
            }
        }

        /// <summary>
        /// Checks the reader.
        /// </summary>
        protected void CheckReader()
        {
            if (!this.isReaderCloned)
            {
                this.reader = this.reader.GetTableReader(this.tag, this.offset, this.length);
                this.isReaderCloned = true;
            }
        }

        /// <summary>
        /// Gets the subset.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <returns></returns>
        public virtual OpenTypeFontTable GetSubset(Dictionary<char, int> chars)
        {
            this.CheckReader();
            return new OpenTypeFontTable(this.tag, this.checkSum, 0L, this.reader.Length, this.reader) { isReaderCloned = true };
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            if (!this.IsLoad)
            {
                this.CheckReader();
                this.LoadData();
                this.isLoad = true;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public virtual void LoadData()
        {
        }

        /// <summary>
        /// Gets the check sum.
        /// </summary>
        /// <value>The check sum.</value>
        public long CheckSum
        {
            get
            {
                if (this.checkSum == 0L)
                {
                    byte[] buffer = this.Reader.ToArray();
                    int num = buffer.Length / 4;
                    int num2 = 0;
                    int num3 = 0;
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = 0;
                    for (int i = 0; i < num; i++)
                    {
                        num5 += buffer[num6++] & 0xff;
                        num4 += buffer[num6++] & 0xff;
                        num3 += buffer[num6++] & 0xff;
                        num2 += buffer[num6++] & 0xff;
                    }
                    this.checkSum = ((num2 + (num3 << 8)) + (num4 << 0x10)) + (num5 << 0x18);
                }
                return this.checkSum;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is load.
        /// </summary>
        /// <value><c>true</c> if this instance is load; otherwise, <c>false</c>.</value>
        public bool IsLoad
        {
            get { return  this.isLoad; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public long Length
        {
            get { return  this.length; }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public long Offset
        {
            get { return  this.offset; }
            internal set { this.offset = value; }
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        public OpenTypeFontReader Reader
        {
            get
            {
                this.CheckReader();
                return this.reader;
            }
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag
        {
            get { return  this.tag; }
        }
    }
}

