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
    /// The Horizontal Metrics Table table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    internal class HorizontalMetricsTable : OpenTypeFontTable
    {
        private FontHeaderTable header;
        private HorizontalHeaderTable hheader;
        private readonly Dictionary<int, float> widths;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HorizontalMetricsTable" /> class.
        /// </summary>
        public HorizontalMetricsTable()
        {
            this.widths = new Dictionary<int, float>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HorizontalMetricsTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public HorizontalMetricsTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
            this.widths = new Dictionary<int, float>();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            for (int i = 0; i < this.hheader.NumberOfHMetrics; i++)
            {
                this.widths[i] = (base.reader.ReadUSHORT() * 1000f) / ((float) this.header.UnitsPerEm);
                base.reader.Skip(2L);
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
                return this.widths.Count;
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
        /// Gets or sets the H header table.
        /// </summary>
        /// <value>The H header table.</value>
        public HorizontalHeaderTable HHeaderTable
        {
            get { return  this.hheader; }
            set { this.hheader = value; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Single" /> at the specified index.
        /// </summary>
        /// <value></value>
        public float this[int index]
        {
            get
            {
                base.CheckLoad();
                if (!this.widths.ContainsKey(index))
                {
                    return 1000f;
                }
                return this.widths[index];
            }
        }
    }
}

