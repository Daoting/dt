#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The viewer proferences of Pdf.
    /// </summary>
    public class PdfViewerPreferences : PdfDictionary, IVersionDepend
    {
        private bool centerWindow;
        private PdfDirection direction;
        private bool displayDocTitle;
        private DuplexMode duplex;
        private bool fitWindow;
        private bool hideMenubar;
        private bool hideToolbar;
        private bool hideWindowUI;
        private FullScreenPageMode nonFullScreenPageMode;
        private int numberOfCopies = 1;
        private bool pageSourceByPageSize;
        private readonly PdfArray printPageRange = new PdfArray();
        private PrintScalingType printScaling;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfViewerPreferences" /> class.
        /// </summary>
        public PdfViewerPreferences()
        {
            base.isLabeled = true;
        }

        /// <summary>
        /// Determines whether this instance is default.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDefault()
        {
            return (((((!this.hideToolbar && !this.hideMenubar) && (!this.hideWindowUI && !this.fitWindow)) && ((!this.centerWindow && !this.displayDocTitle) && ((this.nonFullScreenPageMode == FullScreenPageMode.UseNone) && (this.direction == PdfDirection.L2R)))) && (((this.printScaling == PrintScalingType.AppDefault) && (this.duplex == DuplexMode.Default)) && (!this.pageSourceByPageSize && (this.printPageRange.Count <= 0)))) && (this.numberOfCopies == 1));
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.hideToolbar)
            {
                base[PdfName.HideToolbar] = PdfBool.TRUE;
            }
            if (this.hideMenubar)
            {
                base[PdfName.HideMenubar] = PdfBool.TRUE;
            }
            if (this.hideWindowUI)
            {
                base[PdfName.HideWindowUI] = PdfBool.TRUE;
            }
            if (this.fitWindow)
            {
                base[PdfName.FitWindow] = PdfBool.TRUE;
            }
            if (this.centerWindow)
            {
                base[PdfName.CenterWindow] = PdfBool.TRUE;
            }
            if (this.displayDocTitle)
            {
                base[PdfName.DisplayDocTitle] = PdfBool.TRUE;
            }
            switch (this.nonFullScreenPageMode)
            {
                case FullScreenPageMode.UseOutlines:
                    base[PdfName.NonFullScreenPageMode] = PdfName.UseOutlines;
                    break;

                case FullScreenPageMode.UseThumbs:
                    base[PdfName.NonFullScreenPageMode] = PdfName.UseThumbs;
                    break;

                case FullScreenPageMode.UseOC:
                    base[PdfName.NonFullScreenPageMode] = PdfName.UseOC;
                    break;
            }
            if (this.direction == PdfDirection.R2L)
            {
                base[PdfName.Direction] = PdfName.R2L;
            }
            switch (this.printScaling)
            {
                case PrintScalingType.AppDefault:
                    break;

                case PrintScalingType.None:
                    base[PdfName.PrintScaling] = PdfName.None;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (this.duplex)
            {
                case DuplexMode.Default:
                    break;

                case DuplexMode.Simplex:
                    base[PdfName.Duplex] = PdfName.Simplex;
                    break;

                case DuplexMode.DuplexFlipShortEdge:
                    base[PdfName.Duplex] = PdfName.DuplexFlipShortEdge;
                    break;

                case DuplexMode.DuplexFlipLongEdge:
                    base[PdfName.Duplex] = PdfName.DuplexFlipLongEdge;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (this.pageSourceByPageSize)
            {
                base[PdfName.PickTrayByPDFSize] = PdfBool.TRUE;
            }
            if (this.printPageRange.Count > 0)
            {
                base[PdfName.PrintPageRange] = this.printPageRange;
            }
            if (this.numberOfCopies > 1)
            {
                base[PdfName.NumCopies] = new PdfNumber((double) this.numberOfCopies);
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            if (((this.duplex != DuplexMode.Default) || this.pageSourceByPageSize) || ((this.printPageRange.Count > 0) || (this.numberOfCopies > 1)))
            {
                return PdfVersion.PDF1_7;
            }
            if (this.printScaling != PrintScalingType.AppDefault)
            {
                return PdfVersion.PDF1_6;
            }
            if (this.displayDocTitle)
            {
                return PdfVersion.PDF1_4;
            }
            if (this.direction == PdfDirection.R2L)
            {
                return PdfVersion.PDF1_3;
            }
            return PdfVersion.PDF1_2;
        }

        /// <summary>
        /// A flag specifying whether to position the document’s window
        /// in the center of the screen. Default value: false.
        /// </summary>
        public bool CenterWindow
        {
            get { return  this.centerWindow; }
            set { this.centerWindow = value; }
        }

        /// <summary>
        /// The predominant reading order for text:
        /// L2R   Left to right
        /// R2L   Right to left (including vertical writing systems, 
        /// such as Chinese, Japanese, and Korean)
        /// This entry has no direct effect on the document’s contents
        /// or page numbering but can be used to determine the relative
        /// positioning of pages when displayed side by side or printed
        /// n-up. Default value: L2R.
        /// </summary>
        public PdfDirection Direction
        {
            get { return  this.direction; }
            set { this.direction = value; }
        }

        /// <summary>
        /// A flag specifying whether the window’s title bar should display
        /// the document title taken from the Title entry of the document
        /// information dictionary. If false, the title bar should instead
        /// display the name of the PDF file containing the document.
        /// Default value: false.
        /// </summary>
        public bool DisplayDocTitle
        {
            get { return  this.displayDocTitle; }
            set { this.displayDocTitle = value; }
        }

        /// <summary>
        /// Gets or sets the duplex.
        /// </summary>
        /// <value>The duplex.</value>
        public DuplexMode Duplex
        {
            get { return  this.duplex; }
            set { this.duplex = value; }
        }

        /// <summary>
        /// A flag specifying whether to resize the document’s window
        /// to fit the size of the first displayed page. Default value: false.
        /// </summary>
        public bool FitWindow
        {
            get { return  this.fitWindow; }
            set { this.fitWindow = value; }
        }

        /// <summary>
        /// A flag specifying whether to hide the viewer application’s
        /// menu bar when the document is active. Default value: false.
        /// </summary>
        public bool HideMenubar
        {
            get { return  this.hideMenubar; }
            set { this.hideMenubar = value; }
        }

        /// <summary>
        /// A flag specifying whether to hide the viewer application’s
        /// tool bars when the document is active. Default value: false.
        /// </summary>
        public bool HideToolbar
        {
            get { return  this.hideToolbar; }
            set { this.hideToolbar = value; }
        }

        /// <summary>
        /// A flag specifying whether to hide user interface elements in
        /// the document’s window (such as scroll bars and navigation controls),
        /// leaving only the document’s contents displayed. Default value: false.
        /// </summary>
        public bool HideWindowUI
        {
            get { return  this.hideWindowUI; }
            set { this.hideWindowUI = value; }
        }

        /// <summary>
        /// The document’s page mode, specifying how to display the document on
        /// exiting full-screen mode:
        /// UseNone       Neither document outline nor thumbnail images visible
        /// UseOutlines   Document outline visible
        /// UseThumbs     Thumbnail images visible
        /// UseOC         Optional content group panel visible
        /// This entry is meaningful only if the value of the PageMode entry in
        /// the catalog dictionary is FullScreen; it is ignored otherwise. Default value: UseNone.
        /// </summary>
        public FullScreenPageMode NonFullScreenPageMode
        {
            get { return  this.nonFullScreenPageMode; }
            set { this.nonFullScreenPageMode = value; }
        }

        /// <summary>
        /// Gets or sets the number of copies.
        /// </summary>
        /// <value>The number of copies.</value>
        public int NumberOfCopies
        {
            get { return  this.numberOfCopies; }
            set
            {
                if ((value < 1) || (value > 5))
                {
                    throw new PdfArgumentOutOfRangeException("NumberOfCopies");
                }
                this.numberOfCopies = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [page source by page size].
        /// </summary>
        /// <value>
        /// <c>true</c> if [page source by page size]; otherwise, <c>false</c>.
        /// </value>
        public bool PageSourceByPageSize
        {
            get { return  this.pageSourceByPageSize; }
            set { this.pageSourceByPageSize = value; }
        }

        /// <summary>
        /// Gets the print page range.
        /// </summary>
        /// <value>The print page range.</value>
        public PdfArray PrintPageRange
        {
            get { return  this.printPageRange; }
        }

        /// <summary>
        /// Gets or sets the print scaling.
        /// </summary>
        /// <value>The print scaling.</value>
        public PrintScalingType PrintScaling
        {
            get { return  this.printScaling; }
            set { this.printScaling = value; }
        }

        /// <summary>
        /// Full screen page modes
        /// </summary>
        public enum FullScreenPageMode
        {
            UseNone,
            UseOutlines,
            UseThumbs,
            UseOC
        }

        /// <summary>
        /// Pdf directions
        /// </summary>
        public enum PdfDirection
        {
            L2R,
            R2L
        }
    }
}

