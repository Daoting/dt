#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.BaseObject;
using Dt.Pdf.Object;

using System;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the printer settings for the PDF viewer.
    /// </summary>
    public class PrintPreset
    {
        Dt.Pdf.Object.DuplexMode duplexMode;
        bool isPageSourceByPageSize;
        int numberOfCopies = 1;
        readonly List<PrintRange> printRanges = new List<PrintRange>();
        PrintScalingType scalingType;

        /// <summary>
        /// Appends to PdfDocument.
        /// </summary>
        /// <param name="doc">The document</param>
        internal void AppendTo(PdfDocument doc)
        {
            if (this.scalingType != PrintScalingType.AppDefault)
            {
                doc.ViewerPreferences.PrintScaling = this.scalingType;
            }
            if (this.duplexMode != Dt.Pdf.Object.DuplexMode.Default)
            {
                doc.ViewerPreferences.Duplex = this.duplexMode;
            }
            if (this.isPageSourceByPageSize)
            {
                doc.ViewerPreferences.PageSourceByPageSize = this.isPageSourceByPageSize;
            }
            if ((this.numberOfCopies > 1) && (this.numberOfCopies <= 5))
            {
                doc.ViewerPreferences.NumberOfCopies = this.numberOfCopies;
            }
            if (this.printRanges.Count > 0)
            {
                foreach (PrintRange range in this.printRanges)
                {
                    doc.ViewerPreferences.PrintPageRange.Add(new PdfNumber((double) range.Index));
                    doc.ViewerPreferences.PrintPageRange.Add(new PdfNumber((double) ((range.Index + range.Count) - 1)));
                }
            }
        }

        /// <summary>
        /// Copies from other PrintPreset.
        /// </summary>
        /// <param name="pp">The pp</param>
        internal void CopyFrom(PrintPreset pp)
        {
            this.scalingType = pp.scalingType;
            this.duplexMode = pp.duplexMode;
            this.isPageSourceByPageSize = pp.isPageSourceByPageSize;
            this.printRanges.AddRange((IEnumerable<PrintRange>) pp.printRanges);
            this.numberOfCopies = pp.numberOfCopies;
        }

        /// <summary>
        /// Gets or sets the duplex mode for the PDF print viewer settings.
        /// </summary>
        /// <value>The duplex mode. The default value is <see cref="P:Dt.Cells.Data.PrintPreset.DuplexMode">Default</see>.</value>
        [DefaultValue(0)]
        public Dt.Pdf.Object.DuplexMode DuplexMode
        {
            get { return  this.duplexMode; }
            set { this.duplexMode = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to set the paper source based on the PDF page size.
        /// </summary>
        /// <value>
        /// <c>true</c> if the paper source is determined by page size; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool IsPageSourceByPageSize
        {
            get { return  this.isPageSourceByPageSize; }
            set { this.isPageSourceByPageSize = value; }
        }

        /// <summary>
        /// Gets or sets the number of copies for the PDF print viewer settings.
        /// </summary>
        /// <value>The number of copies. The default value is 1.</value>
        [DefaultValue(1)]
        public int NumberOfCopies
        {
            get { return  this.numberOfCopies; }
            set
            {
                if ((value < 1) || (value > 5))
                {
                    throw new ArgumentOutOfRangeException("numberOfCopies", ResourceStrings.PdfSetNumberOfCopiesError);
                }
                this.numberOfCopies = value;
            }
        }

        /// <summary>
        /// Gets the page ranges to print for the PDF print viewer settings.
        /// </summary>
        /// <value>The print ranges. The default value is never null.</value>
        public List<PrintRange> PrintRanges
        {
            get { return  this.printRanges; }
        }

        /// <summary>
        /// Gets or sets the type of scaling for the PDF print viewer settings.
        /// </summary>
        /// <value>The type of scaling. The default value is <see cref="T:Dt.Pdf.Object.PrintScalingType">AppDefault</see>.</value>
        [DefaultValue(0)]
        public PrintScalingType ScalingType
        {
            get { return  this.scalingType; }
            set { this.scalingType = value; }
        }

        /// <summary>
        /// Represents a preset print range.
        /// </summary>
        public class PrintRange
        {
            readonly ushort count;
            readonly ushort index;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PrintPreset.PrintRange" /> class.
            /// </summary>
            /// <param name="index">The index (zero-based).</param>
            /// <param name="count">The count.</param>
            public PrintRange(ushort index, ushort count)
            {
                if (count == 0)
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                this.index = index;
                this.count = count;
            }

            /// <summary>
            /// Gets the number of ranges.
            /// </summary>
            /// <value>The count.</value>
            [DefaultValue(0)]
            public ushort Count
            {
                get { return  this.count; }
            }

            /// <summary>
            /// Gets the start index of this range.
            /// </summary>
            /// <value>The index.</value>
            [DefaultValue(0)]
            public ushort Index
            {
                get { return  this.index; }
            }
        }
    }
}

