#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.Generic;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// GcReportContext
    /// </summary>
    internal class GcReportContext : IMeasureable
    {
        GcSection.GcSectionCache bmCache;
        GcBottomMarginSection bottomMargin;
        internal static readonly Font defaultFont = new Font(DefaultStyleCollection.DefaultFontName, DefaultStyleCollection.DefaultFontSize);
        int dpi;
        readonly IMeasureable measure;
        GcPageFooterSection pageFooter;
        GcPageHeaderSection pageHeader;
        int pageHeaderFooterMaxWidth;
        readonly GcPageRectangles pageRects;
        readonly List<List<GcPageBlock>> pages;
        GcSection.GcSectionCache pfCache;
        GcSection.GcSectionCache phCache;
        readonly GcReport report;
        GcSection.GcSectionCache tmCache;
        GcTopMarginSection topMargin;
        int upi;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcReportContext" /> class.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="dpi">The dpi.</param>
        public GcReportContext(GcReport report, int dpi)
        {
            this.upi = 0x48;
            this.pages = new List<List<GcPageBlock>>();
            this.pageRects = new GcPageRectangles();
            this.measure = new Utilities.MeasureUtility(UnitType.CentileInch, defaultFont);
            this.pageFooter = null;
            this.pageHeader = null;
            this.report = report;
            this.dpi = dpi;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcReportContext" /> class.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="dpi">The dpi.</param>
        /// <param name="measure">The measurement.</param>
        public GcReportContext(GcReport report, int dpi, IMeasureable measure) : this(report, dpi)
        {
            this.measure = measure;
        }

        /// <summary>
        /// Creates the new page.
        /// </summary>
        /// <returns></returns>
        public GcPageBlock CreateNewPage()
        {
            GcPageBlock block = new GcPageBlock();
            Windows.Foundation.Size pageSize = Utilities.GetPageSize(this.Report.PaperSize, this.Report.Orientation);
            block.X = 0.0;
            block.Y = 0.0;
            block.Width = pageSize.Width;
            block.Height = pageSize.Height;
            GcPageRectangles rectangles = new GcPageRectangles {
                CropRectangle = this.pageRects.CropRectangle
            };
            block.Rectangles = rectangles;
            return block;
        }

        /// <summary>
        /// Generates the page blocks.
        /// </summary>
        public void GeneratePageBlocks()
        {
            this.pageHeader = this.Report.PageHeaderInner;
            this.pageFooter = this.Report.PageFooterInner;
            this.topMargin = this.Report.TopMarginInner;
            this.bottomMargin = this.Report.BottomMarginInner;
            if (this.pageHeader != null)
            {
                this.phCache = this.pageHeader.GetCache(this);
                this.pageHeaderFooterMaxWidth = Math.Max(this.pageHeaderFooterMaxWidth, (int) this.pageHeader.GetAllSize(this.phCache).Width);
            }
            if (this.pageFooter != null)
            {
                this.pfCache = this.pageFooter.GetCache(this);
                this.pageHeaderFooterMaxWidth = Math.Max(this.pageHeaderFooterMaxWidth, (int) this.pageFooter.GetAllSize(this.pfCache).Width);
            }
            if (this.topMargin != null)
            {
                this.tmCache = this.topMargin.GetCache(this);
            }
            if (this.bottomMargin != null)
            {
                this.bmCache = this.bottomMargin.GetCache(this);
            }
            List<GcMultiplePageSection> sortedSections = this.Report.GetSortedSections();
            bool autoFit = this.Report.AutoFit;
            this.Report.RealZoom = this.Report.ZoomFactor;
            double realZoom = this.Report.RealZoom;
            double num2 = 0.1;
            bool flag2 = false;
        Label_014B:
            this.pages.Clear();
            try
            {
                this.InitPageRectangles();
            }
            catch (Exception exception)
            {
                throw new ArgumentOutOfRangeException(ResourceStrings.ReportingMarginError, exception);
            }
            bool continuePage = false;
            double offset = 0.0;
            double pageHeaderOffset = 0.0;
            for (int i = 0; i < sortedSections.Count; i++)
            {
                continuePage = sortedSections[i].GeneratePages(this, continuePage, ref offset, ref pageHeaderOffset);
            }
            if (autoFit)
            {
                bool flag4 = false;
                if ((this.Report.FitPagesTall != -1) && (this.pages.Count > this.Report.FitPagesTall))
                {
                    flag4 = true;
                }
                if (this.Report.FitPagesWide != -1)
                {
                    int num6 = 0;
                    using (List<List<GcPageBlock>>.Enumerator enumerator = this.pages.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            num6 = Math.Max(enumerator.Current.Count, num6);
                        }
                    }
                    if (num6 > this.Report.FitPagesWide)
                    {
                        flag4 = true;
                    }
                }
                if ((flag4 || flag2) && ((this.Report.RealZoom > 0.1) && ((realZoom - num2) >= 0.05)))
                {
                    flag2 = true;
                    if (flag4)
                    {
                        realZoom = this.Report.RealZoom;
                    }
                    else
                    {
                        num2 = this.Report.RealZoom;
                    }
                    this.Report.RealZoom = (realZoom + num2) / 2.0;
                    goto Label_014B;
                }
                if ((flag4 && ((realZoom - num2) < 0.05)) && (this.Report.RealZoom != num2))
                {
                    this.Report.RealZoom = num2;
                    goto Label_014B;
                }
            }
        }

        /// <summary>
        /// Gets the dot measurement.
        /// </summary>
        /// <param name="n">The n value.</param>
        /// <returns></returns>
        public float GetDot(float n)
        {
            return (n * this.GetForce());
        }

        /// <summary>
        /// Gets the force.
        /// </summary>
        /// <returns></returns>
        public float GetForce()
        {
            return (((float) this.upi) / 100f);
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <returns></returns>
        public int GetPageCount()
        {
            int num = 0;
            foreach (List<GcPageBlock> list in this.Pages)
            {
                num += list.Count;
            }
            return num;
        }

        /// <summary>
        /// Init the page rectangles.
        /// </summary>
        void InitPageRectangles()
        {
            Windows.Foundation.Size pageSize = Utilities.GetPageSize(this.Report.PaperSize, this.Report.Orientation);
            Windows.Foundation.Rect rect = new Windows.Foundation.Rect(0.0, 0.0, pageSize.Width, pageSize.Height);
            this.pageRects.PageRectangle = new Windows.Foundation.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            rect.X += this.Report.Margin.Left;
            rect.Width -= this.Report.Margin.Left + this.Report.Margin.Right;
            rect.Y += this.Report.Margin.Top;
            rect.Height -= this.Report.Margin.Top + this.Report.Margin.Bottom;
            double realZoom = this.Report.RealZoom;
            this.pageRects.CropRectangle = new Windows.Foundation.Rect(rect.X, rect.Y, rect.Width / realZoom, rect.Height / realZoom);
            if (this.topMargin != null)
            {
                this.pageRects.TopMarginRectangle = new Windows.Foundation.Rect(rect.X, (double) this.Report.Margin.Header, rect.Width, (double) this.Report.Margin.Top);
            }
            if (this.bottomMargin != null)
            {
                this.pageRects.BottomMarginRectangle = new Windows.Foundation.Rect(rect.X, pageSize.Height - this.Report.Margin.Bottom, rect.Width, (double) this.Report.Margin.Bottom);
            }
            if (this.pageHeader != null)
            {
                Windows.Foundation.Size allSize = this.pageHeader.GetAllSize(this);
                this.pageRects.PageHeaderRectangle = new Windows.Foundation.Rect(rect.Left, rect.Top, allSize.Width, allSize.Height);
                rect.Y += allSize.Height;
                rect.Height -= allSize.Height;
            }
            if (this.pageFooter != null)
            {
                Windows.Foundation.Size size3 = this.pageFooter.GetAllSize(this);
                rect.Height -= size3.Height;
                this.pageRects.PageFooterRectangle = new Windows.Foundation.Rect(rect.X, rect.Y + rect.Height, size3.Width, size3.Height);
            }
            this.pageRects.ContentRectangle = new Windows.Foundation.Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        /// <summary>
        /// Measures the string with no wrapping.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        public Windows.Foundation.Size MeasureNoWrapString(string str, Font font)
        {
            return this.measure.MeasureNoWrapString(str, font);
        }

        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <param name="allowWrap">If set to <c>true</c>, [allow wrap]</param>
        /// <param name="width">The width (in 1/100 inches).</param>
        /// <returns>The measurement (in 1/100 inches).</returns>
        public Windows.Foundation.Size MeasureString(string str, Font font, bool allowWrap, int width)
        {
            return this.measure.MeasureString(str, font, allowWrap, width);
        }

        /// <summary>
        /// Gets the bottom margin.
        /// </summary>
        /// <value>The bottom margin.</value>
        public GcBottomMarginSection BottomMargin
        {
            get { return  this.bottomMargin; }
        }

        /// <summary>
        /// Gets the bottom margin cache.
        /// </summary>
        /// <value>The bottom margin cache.</value>
        public GcSection.GcSectionCache BottomMarginCache
        {
            get { return  this.bmCache; }
        }

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <value>The default font.</value>
        public Font DefaultFont
        {
            get { return  defaultFont; }
        }

        /// <summary>
        /// Gets or sets the dpi.
        /// </summary>
        /// <value>The dpi.</value>
        public int Dpi
        {
            get { return  this.dpi; }
            set { this.dpi = value; }
        }

        /// <summary>
        /// Gets the height of the page content.
        /// </summary>
        /// <value>The height of the page content.</value>
        public int PageContentHeight
        {
            get { return  (int) this.pageRects.ContentRectangle.Height; }
        }

        /// <summary>
        /// Gets the width of the page content.
        /// </summary>
        /// <value>The width of the page content.</value>
        public int PageContentWidth
        {
            get { return  (int) this.pageRects.ContentRectangle.Width; }
        }

        /// <summary>
        /// Gets the page footer.
        /// </summary>
        /// <value>The page footer.</value>
        public GcPageFooterSection PageFooter
        {
            get { return  this.pageFooter; }
        }

        /// <summary>
        /// Gets the page footer cache.
        /// </summary>
        /// <value>The page footer cache.</value>
        public GcSection.GcSectionCache PageFooterCache
        {
            get { return  this.pfCache; }
        }

        /// <summary>
        /// Gets the page header.
        /// </summary>
        /// <value>The page header.</value>
        public GcPageHeaderSection PageHeader
        {
            get { return  this.pageHeader; }
        }

        /// <summary>
        /// Gets the page header cache.
        /// </summary>
        /// <value>The page header cache.</value>
        public GcSection.GcSectionCache PageHeaderCache
        {
            get { return  this.phCache; }
        }

        /// <summary>
        /// Gets the width of the maximum page header footer.
        /// </summary>
        /// <value>The maximum width of the page header footer.</value>
        public int PageHeaderFooterMaxWidth
        {
            get { return  this.pageHeaderFooterMaxWidth; }
        }

        /// <summary>
        /// Gets the page rectangles.
        /// </summary>
        /// <value>The page rectangles.</value>
        public GcPageRectangles PageRects
        {
            get { return  this.pageRects; }
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public List<List<GcPageBlock>> Pages
        {
            get { return  this.pages; }
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <value>The report.</value>
        public GcReport Report
        {
            get { return  this.report; }
        }

        /// <summary>
        /// Gets the top margin.
        /// </summary>
        /// <value>The top margin.</value>
        public GcTopMarginSection TopMargin
        {
            get { return  this.topMargin; }
        }

        /// <summary>
        /// Gets the top margin cache.
        /// </summary>
        /// <value>The top margin cache.</value>
        public GcSection.GcSectionCache TopMarginCache
        {
            get { return  this.tmCache; }
        }

        /// <summary>
        /// Gets or sets the units per inch.
        /// </summary>
        /// <value>The units per inch.</value>
        public int UnitsPerInch
        {
            get { return  this.upi; }
            set { this.upi = value; }
        }
    }
}

