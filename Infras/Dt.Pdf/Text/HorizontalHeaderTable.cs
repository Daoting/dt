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
    /// The Horizontal Header Table table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    internal class HorizontalHeaderTable : OpenTypeFontTable
    {
        private int advanceWidthMax;
        private short ascender;
        private short caretOffset;
        private short caretSlopeRise;
        private short caretSlopeRun;
        private short descender;
        private short lineGap;
        private short metricDataFormat;
        private short minLeftSideBearing;
        private short minRightSideBearing;
        private int numberOfHMetrics;
        private float version;
        private short xMaxExtent;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HorizontalHeaderTable" /> class.
        /// </summary>
        public HorizontalHeaderTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HorizontalHeaderTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public HorizontalHeaderTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
            base.Load();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            this.version = base.reader.ReadFixed();
            this.ascender = base.reader.ReadFWORD();
            this.descender = base.reader.ReadFWORD();
            this.lineGap = base.reader.ReadFWORD();
            this.advanceWidthMax = base.reader.ReadUFWORD();
            this.minLeftSideBearing = base.reader.ReadFWORD();
            this.minRightSideBearing = base.reader.ReadFWORD();
            this.xMaxExtent = base.reader.ReadFWORD();
            this.caretSlopeRise = base.reader.ReadSHORT();
            this.caretSlopeRun = base.reader.ReadSHORT();
            this.caretOffset = base.reader.ReadSHORT();
            base.reader.Skip(8L);
            this.metricDataFormat = base.reader.ReadSHORT();
            this.numberOfHMetrics = base.reader.ReadUSHORT();
            base.LoadData();
        }

        /// <summary>
        /// Gets the advance width max.
        /// </summary>
        /// <value>The advance width max.</value>
        public int AdvanceWidthMax
        {
            get { return  this.advanceWidthMax; }
        }

        /// <summary>
        /// Gets the ascender.
        /// </summary>
        /// <value>The ascender.</value>
        public short Ascender
        {
            get { return  this.ascender; }
        }

        /// <summary>
        /// Gets the caret offset.
        /// </summary>
        /// <value>The caret offset.</value>
        public short CaretOffset
        {
            get { return  this.caretOffset; }
        }

        /// <summary>
        /// Gets the caret slope rise.
        /// </summary>
        /// <value>The caret slope rise.</value>
        public short CaretSlopeRise
        {
            get { return  this.caretSlopeRise; }
        }

        /// <summary>
        /// Gets the caret slope run.
        /// </summary>
        /// <value>The caret slope run.</value>
        public short CaretSlopeRun
        {
            get { return  this.caretSlopeRun; }
        }

        /// <summary>
        /// Gets the descender.
        /// </summary>
        /// <value>The descender.</value>
        public short Descender
        {
            get { return  this.descender; }
        }

        /// <summary>
        /// Gets the line gap.
        /// </summary>
        /// <value>The line gap.</value>
        public short LineGap
        {
            get { return  this.lineGap; }
        }

        /// <summary>
        /// Gets the metric data format.
        /// </summary>
        /// <value>The metric data format.</value>
        public short MetricDataFormat
        {
            get { return  this.metricDataFormat; }
        }

        /// <summary>
        /// Gets the min left side bearing.
        /// </summary>
        /// <value>The min left side bearing.</value>
        public short MinLeftSideBearing
        {
            get { return  this.minLeftSideBearing; }
        }

        /// <summary>
        /// Gets the min right side bearing.
        /// </summary>
        /// <value>The min right side bearing.</value>
        public short MinRightSideBearing
        {
            get { return  this.minRightSideBearing; }
        }

        /// <summary>
        /// Gets the number of H metrics.
        /// </summary>
        /// <value>The number of H metrics.</value>
        public int NumberOfHMetrics
        {
            get { return  this.numberOfHMetrics; }
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
        /// Gets the X max extent.
        /// </summary>
        /// <value>The X max extent.</value>
        public short XMaxExtent
        {
            get { return  this.xMaxExtent; }
        }
    }
}

