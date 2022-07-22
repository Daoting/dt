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
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the report. 
    /// </summary>
    internal class GcReport : IXmlSerializable
    {
        bool blackAndWhite;
        string bookmark;
        Dt.Cells.Data.Centering centering;
        int firstPageNumber;
        int fitPagesTall;
        int fitPagesWide;
        Margins margin;
        PrintPageOrientation orientation;
        PrintPageOrder pageOrder;
        string pageRange;
        Dt.Cells.Data.PaperSize paperSize;
        double realZoom;
        SectionCollection sections;
        Dt.Cells.Data.Watermark watermark;
        double zoomFactor;

        /// <summary>
        /// Creates a new report.
        /// </summary>
        public GcReport()
        {
            this.Init();
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <param name="ignorePageRange">If set to <c>true</c> the report ignores the PageRange property.</param>
        /// <returns>Returns the page count.</returns>
        public int GetPageCount(bool ignorePageRange)
        {
            int pageCount = new PdfExporter(this).GetPageCount();
            if (!ignorePageRange && !string.IsNullOrEmpty(this.PageRange))
            {
                return Utilities.GetPageRange(this.PageRange, pageCount).Count;
            }
            return pageCount;
        }

        /// <summary>
        /// Gets the sorted sections.
        /// </summary>
        /// <returns></returns>
        internal List<GcMultiplePageSection> GetSortedSections()
        {
            List<GcMultiplePageSection> list = new List<GcMultiplePageSection>();
            foreach (GcSection section in this.sections)
            {
                if (!(section is IGcSpecialSection) && (section is GcMultiplePageSection))
                {
                    list.Add((GcMultiplePageSection) section);
                }
                if ((section is GcAllSheetSection) && (((GcAllSheetSection) section).Workbook != null))
                {
                    foreach (Worksheet worksheet in ((GcAllSheetSection) section).Workbook.Sheets)
                    {
                        list.Add(new GcSheetSection(worksheet));
                    }
                }
            }
            if (this.ReportHeaderInner != null)
            {
                list.Insert(0, this.ReportHeaderInner);
            }
            if (this.ReportFooterInner != null)
            {
                list.Add(this.ReportFooterInner);
            }
            return list;
        }

        /// <summary>
        /// Gets the standalone section.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetStandAloneSection<T>() where T: GcSection
        {
            List<T> sections = this.sections.GetSections<T>();
            T local = (sections.Count <= 0) ? default(T) : sections[0];
            if ((local != null) && (local is GcMarginSection))
            {
                GcMarginSection section = local as GcMarginSection;
                if (section.Report != this)
                {
                    section.Report = this;
                }
            }
            return local;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Init()
        {
            this.sections = new SectionCollection();
            this.paperSize = new Dt.Cells.Data.PaperSize();
            this.centering = Dt.Cells.Data.Centering.None;
            this.zoomFactor = 1.0;
            this.pageOrder = PrintPageOrder.Auto;
            this.fitPagesTall = -1;
            this.fitPagesWide = -1;
            this.realZoom = -1.0;
            this.bookmark = string.Empty;
            this.watermark = new Dt.Cells.Data.Watermark();
            this.pageRange = string.Empty;
            this.margin = new Margins();
            this.orientation = PrintPageOrientation.Portrait;
            this.firstPageNumber = 1;
            this.blackAndWhite = false;
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadXmlBase(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
        }

        /// <summary>
        /// Sets the standalone section.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t</param>
        void SetStandAloneSection<T>(T t) where T: GcSection
        {
            this.sections.RemoveSections<T>();
            if (t is GcMarginSection)
            {
                GcMarginSection section = t as GcMarginSection;
                section.Report = this;
            }
            if (t != null)
            {
                this.sections.Add(t);
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            this.Init();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    this.ReadXmlBase(reader);
                }
            }
            if (this.TopMarginInner != null)
            {
                this.TopMarginInner.Report = this;
            }
            if (this.BottomMarginInner != null)
            {
                this.BottomMarginInner.Report = this;
            }
            if (this.sections != null)
            {
                Dictionary<int, Dt.Cells.Data.Bookmark> dictionary = new Dictionary<int, Dt.Cells.Data.Bookmark>();
                foreach (GcSection section in this.sections)
                {
                    if (section.Controls != null)
                    {
                        foreach (GcControl control in section.Controls)
                        {
                            if (control is GcPrintableControl)
                            {
                                Dt.Cells.Data.Bookmark bookmark = ((GcPrintableControl) control).Bookmark;
                                if (!dictionary.ContainsKey(bookmark.Index))
                                {
                                    dictionary.Add(bookmark.Index, bookmark);
                                    Dt.Cells.Data.Bookmark.SIndex = Math.Max(Dt.Cells.Data.Bookmark.SIndex, bookmark.Index);
                                }
                            }
                        }
                    }
                }
                foreach (Dt.Cells.Data.Bookmark bookmark2 in dictionary.Values)
                {
                    if (bookmark2.ParentIndex >= 0)
                    {
                        bookmark2.Parent = dictionary[bookmark2.ParentIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.WriteXmlBase(writer);
        }

        /// <summary>
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void WriteXmlBase(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets a value indicating whether auto fit.
        /// </summary>
        internal bool AutoFit
        {
            get
            {
                if (this.FitPagesTall == -1)
                {
                    return (this.FitPagesWide != -1);
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets whether to print the report in black and white or in color.  
        /// </summary>
        /// <value>
        /// A value that specifies whether to print the report in black and white or in color.
        /// The default value is <c>false</c>, which means the report is printed in color.
        /// </value>
        [DefaultValue(false)]
        public bool BlackAndWhite
        {
            get { return  this.blackAndWhite; }
            set { this.blackAndWhite = value; }
        }

        /// <summary>
        /// Gets or sets the root bookmark text.
        /// </summary>
        /// <value>The text for the root bookmark. The default value is "Report".</value>
        [DefaultValue("Report")]
        public string Bookmark
        {
            get
            {
                if (!string.IsNullOrEmpty(this.bookmark))
                {
                    return this.bookmark;
                }
                return "Report";
            }
            set { this.bookmark = value; }
        }

        /// <summary>
        /// Gets or sets the bottom margin section.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.GcBottomMarginSection" /> object that represents the bottom margin section.</value>
        public GcBottomMarginSection BottomMargin
        {
            get
            {
                GcBottomMarginSection bottomMarginInner = this.BottomMarginInner;
                if (bottomMarginInner == null)
                {
                    bottomMarginInner = new GcBottomMarginSection();
                    this.SetStandAloneSection<GcBottomMarginSection>(bottomMarginInner);
                }
                return bottomMarginInner;
            }
            set { this.SetStandAloneSection<GcBottomMarginSection>(value); }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the bottom margin section.
        /// </summary>
        /// <value>The bottom margin</value>
        internal GcBottomMarginSection BottomMarginInner
        {
            get { return  this.GetStandAloneSection<GcBottomMarginSection>(); }
            set { this.SetStandAloneSection<GcBottomMarginSection>(value); }
        }

        /// <summary>
        /// Gets or sets how the printed page is centered.  
        /// </summary>
        /// <value>
        /// A value that specifies how the printed page is centered. 
        /// The default value is <see cref="P:Dt.Cells.Data.GcReport.Centering">None</see>.
        /// </value>
        [DefaultValue(0)]
        public Dt.Cells.Data.Centering Centering
        {
            get { return  this.centering; }
            set { this.centering = value; }
        }

        /// <summary>
        /// Gets or sets the page number to print on the first page. 
        /// </summary>
        /// <value>The page number to print on the first page. The default value is 1.</value>
        [DefaultValue(1)]
        public int FirstPageNumber
        {
            get { return  this.firstPageNumber; }
            set { this.firstPageNumber = value; }
        }

        /// <summary>
        /// Gets or sets the number of vertical pages to check when optimizing printing.  
        /// </summary>
        /// <value>The number of vertical pages to check.</value>
        [DefaultValue(-1)]
        public int FitPagesTall
        {
            get { return  this.fitPagesTall; }
            set
            {
                if ((value <= 0) && (value != -1))
                {
                    throw new ArgumentException();
                }
                this.fitPagesTall = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of horizontal pages to check when optimizing the printing.  
        /// </summary>
        /// <value>The number of horizontal pages to check.</value>
        [DefaultValue(-1)]
        public int FitPagesWide
        {
            get { return  this.fitPagesWide; }
            set
            {
                if ((value <= 0) && (value != -1))
                {
                    throw new ArgumentException();
                }
                this.fitPagesWide = value;
            }
        }

        /// <summary>
        /// Gets or sets the margins of the report's pages, in hundredths of an inch. 
        /// </summary>
        /// <value>The margin of the report's pages.</value>
        public Margins Margin
        {
            get
            {
                if (this.margin == null)
                {
                    this.margin = new Margins();
                }
                return this.margin;
            }
            set { this.margin = value; }
        }

        /// <summary>
        /// Gets or sets the page orientation used for printing.  
        /// </summary>
        /// <value>The orientation of the page. The default value is <see cref="T:Dt.Cells.Data.PrintPageOrientation">Portrait</see>.</value>
        [DefaultValue(1)]
        public PrintPageOrientation Orientation
        {
            get { return  this.orientation; }
            set { this.orientation = value; }
        }

        /// <summary>
        /// Gets or sets the page footer section.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.GcPageFooterSection" /> object that represents the page footer section.</value>
        public GcPageFooterSection PageFooter
        {
            get
            {
                GcPageFooterSection pageFooterInner = this.PageFooterInner;
                if (pageFooterInner == null)
                {
                    pageFooterInner = new GcPageFooterSection();
                    this.SetStandAloneSection<GcPageFooterSection>(pageFooterInner);
                }
                return pageFooterInner;
            }
            set { this.SetStandAloneSection<GcPageFooterSection>(value); }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the page footer section.
        /// </summary>
        /// <value>The page footer</value>
        internal GcPageFooterSection PageFooterInner
        {
            get { return  this.GetStandAloneSection<GcPageFooterSection>(); }
            set { this.SetStandAloneSection<GcPageFooterSection>(value); }
        }

        /// <summary>
        /// Gets or sets the page header section.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.GcPageHeaderSection" /> object that represents the page header section.</value>
        public GcPageHeaderSection PageHeader
        {
            get
            {
                GcPageHeaderSection pageHeaderInner = this.PageHeaderInner;
                if (pageHeaderInner == null)
                {
                    pageHeaderInner = new GcPageHeaderSection();
                    this.SetStandAloneSection<GcPageHeaderSection>(pageHeaderInner);
                }
                return pageHeaderInner;
            }
            set { this.SetStandAloneSection<GcPageHeaderSection>(value); }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the page header section.
        /// </summary>
        /// <value>The page header</value>
        internal GcPageHeaderSection PageHeaderInner
        {
            get { return  this.GetStandAloneSection<GcPageHeaderSection>(); }
            set { this.SetStandAloneSection<GcPageHeaderSection>(value); }
        }

        /// <summary>
        /// Gets or sets the order in which pages print.  
        /// </summary>
        /// <value>A value that specifies the order for printing pages. The default value is <see cref="T:Dt.Cells.Data.PrintPageOrder">Auto</see>.</value>
        [DefaultValue(0)]
        public PrintPageOrder PageOrder
        {
            get { return  this.pageOrder; }
            set { this.pageOrder = value; }
        }

        /// <summary>
        /// Gets or sets the page range.
        /// </summary>
        /// <remarks>
        /// Type page numbers or page ranges separated by commas
        /// counting from the start of the document.
        /// For example, type "1,3,5-12".
        /// </remarks>
        /// <value>The page range.</value>
        [DefaultValue((string) null)]
        public string PageRange
        {
            get { return  this.pageRange; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !Utilities.CheckPageRange(value))
                {
                    throw new ArgumentException(ResourceStrings.ReportingPageRangeError);
                }
                this.pageRange = value;
            }
        }

        /// <summary>
        /// Gets or sets the paper size for printing.  
        /// </summary>
        /// <value>A <see cref="P:Dt.Cells.Data.GcReport.PaperSize" /> object that represents the paper size for printing.</value>
        public Dt.Cells.Data.PaperSize PaperSize
        {
            get { return  this.paperSize; }
            internal set { this.paperSize = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the real zoom.
        /// </summary>
        /// <value>The real zoom</value>
        internal double RealZoom
        {
            get
            {
                if (!this.AutoFit)
                {
                    return this.ZoomFactor;
                }
                return Math.Min(this.realZoom, this.ZoomFactor);
            }
            set { this.realZoom = value; }
        }

        /// <summary>
        /// Gets or sets the report footer section.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.GcReportHeaderSection" /> object that represents the report footer section.</value>
        public GcReportFooterSection ReportFooter
        {
            get
            {
                GcReportFooterSection reportFooterInner = this.ReportFooterInner;
                if (reportFooterInner == null)
                {
                    reportFooterInner = new GcReportFooterSection();
                    this.SetStandAloneSection<GcReportFooterSection>(reportFooterInner);
                }
                return reportFooterInner;
            }
            set { this.SetStandAloneSection<GcReportFooterSection>(value); }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the report footer section.
        /// </summary>
        /// <value>The report footer</value>
        internal GcReportFooterSection ReportFooterInner
        {
            get { return  this.GetStandAloneSection<GcReportFooterSection>(); }
            set { this.SetStandAloneSection<GcReportFooterSection>(value); }
        }

        /// <summary>
        /// Gets or sets the report header section.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.GcReportHeaderSection" /> object that represents the report header section.</value>
        public GcReportHeaderSection ReportHeader
        {
            get
            {
                GcReportHeaderSection reportHeaderInner = this.ReportHeaderInner;
                if (reportHeaderInner == null)
                {
                    reportHeaderInner = new GcReportHeaderSection();
                    this.SetStandAloneSection<GcReportHeaderSection>(reportHeaderInner);
                }
                return reportHeaderInner;
            }
            set { this.SetStandAloneSection<GcReportHeaderSection>(value); }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the report header section.
        /// </summary>
        /// <value>The report header</value>
        internal GcReportHeaderSection ReportHeaderInner
        {
            get { return  this.GetStandAloneSection<GcReportHeaderSection>(); }
            set { this.SetStandAloneSection<GcReportHeaderSection>(value); }
        }

        /// <summary>
        /// Gets the collection of the sections contained in a report.
        /// </summary>
        /// <value>An object of the <see cref="T:Dt.Cells.Data.SectionCollection" /> class that represents the collection of contained sections.</value>
        public SectionCollection Sections
        {
            get { return  this.sections; }
        }

        /// <summary>
        /// Gets or sets the top margin section.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.GcTopMarginSection" /> object that represents the top margin section.</value>
        public GcTopMarginSection TopMargin
        {
            get
            {
                GcTopMarginSection topMarginInner = this.TopMarginInner;
                if (topMarginInner == null)
                {
                    topMarginInner = new GcTopMarginSection();
                    this.SetStandAloneSection<GcTopMarginSection>(topMarginInner);
                }
                return topMarginInner;
            }
            set { this.SetStandAloneSection<GcTopMarginSection>(value); }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the top margin section.
        /// </summary>
        /// <value>The top margin</value>
        internal GcTopMarginSection TopMarginInner
        {
            get { return  this.GetStandAloneSection<GcTopMarginSection>(); }
            set { this.SetStandAloneSection<GcTopMarginSection>(value); }
        }

        /// <summary>
        /// Gets the report's watermark.
        /// </summary>
        /// <value>A <see cref="P:Dt.Cells.Data.GcReport.Watermark" /> object that specifies the report's watermark.</value>
        public Dt.Cells.Data.Watermark Watermark
        {
            get { return  this.watermark; }
            internal set { this.watermark = value; }
        }

        /// <summary>
        /// Gets or sets the zoom factor used for printing this report.  
        /// </summary>
        /// <value>
        /// The value as a percentage. The value must be between 0.1 (10%) and 4 (400%).
        /// The default value is 0.
        /// </value>
        [DefaultValue(0)]
        public double ZoomFactor
        {
            get { return  this.zoomFactor; }
            set
            {
                if ((value < 0.1) || (value > 4.0))
                {
                    throw new ArgumentOutOfRangeException("ZoomFactor", ResourceStrings.ZoomFactorOutOfRange);
                }
                this.zoomFactor = value;
            }
        }
    }
}

