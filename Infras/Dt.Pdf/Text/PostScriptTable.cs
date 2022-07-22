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
    /// The post script table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    internal class PostScriptTable : OpenTypeFontTable
    {
        private bool isFixedPicth;
        private float italicAngle;
        private short underlinePostion;
        private short underlineThickness;
        private float version;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PostScriptTable" /> class.
        /// </summary>
        public PostScriptTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PostScriptTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public PostScriptTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
            base.Load();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            this.version = base.reader.ReadFixed();
            this.italicAngle = base.reader.ReadFixed();
            this.underlinePostion = base.reader.ReadFWORD();
            this.underlineThickness = base.reader.ReadFWORD();
            this.isFixedPicth = base.reader.ReadULONG() != 0L;
            base.LoadData();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed picth.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed picth; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedPicth
        {
            get { return  this.isFixedPicth; }
        }

        /// <summary>
        /// Gets the italic angle.
        /// </summary>
        /// <value>The italic angle.</value>
        public float ItalicAngle
        {
            get { return  this.italicAngle; }
        }

        /// <summary>
        /// Gets the underline postion.
        /// </summary>
        /// <value>The underline postion.</value>
        public short UnderlinePostion
        {
            get { return  this.underlinePostion; }
        }

        /// <summary>
        /// Gets the underline thickness.
        /// </summary>
        /// <value>The underline thickness.</value>
        public short UnderlineThickness
        {
            get { return  this.underlineThickness; }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public float Version
        {
            get { return  this.version; }
        }
    }
}

