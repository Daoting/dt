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
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The root of a document’s object hierarchy is the catalog dictionary.
    /// </summary>
    public class PdfDocumentCatalog : PdfDictionary, IVersionDepend
    {
        private readonly PdfInfo info;
        private OpenType openType;
        private readonly PdfOutline outlines;
        private PageLayoutType pageLayout;
        /// <summary>
        /// Warning: can be null;
        /// </summary>
        private readonly PdfDocument pdf;
        private PdfViewerPreferences viewerPreferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfDocumentCatalog" /> class.
        /// </summary>
        public PdfDocumentCatalog()
        {
            this.info = new PdfInfo();
            this.outlines = new PdfOutline();
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfDocumentCatalog" /> class.
        /// </summary>
        /// <param name="pdf">The PDF.</param>
        internal PdfDocumentCatalog(PdfDocument pdf)
        {
            this.info = new PdfInfo();
            this.outlines = new PdfOutline();
            this.pdf = pdf;
            this.Init();
        }

        /// <summary>
        /// Creates the default page outlines.
        /// </summary>
        public void CreateDefaultPageOutlines()
        {
            PdfPageTree pages = this.Pages;
            this.ScanPageTreeAndCreateOutline(pages, null);
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        private void Init()
        {
            base.Add(PdfName.Type, PdfName.Catalog);
            PdfPageTree tree = new PdfPageTree(this.pdf) {
                IsLabeled = true,
                IsFix = true
            };
            PdfDictionary dictionary = tree;
            base.Add(PdfName.Pages, dictionary);
        }

        /// <summary>
        /// Scans the page tree and create outline.
        /// </summary>
        /// <param name="pages">The pages.</param>
        /// <param name="outline">The outline.</param>
        private void ScanPageTreeAndCreateOutline(PdfPageTree pages, PdfOutlineItem outline)
        {
            List<PdfOutlineItem> list = (outline == null) ? this.outlines.Items : outline.ChildItems;
            foreach (PdfObjectBase base2 in pages.Kids)
            {
                if (base2 is PdfPage)
                {
                    list.Add(new PdfOutlineItem((PdfPage) base2));
                }
                else
                {
                    PdfOutlineItem item = new PdfOutlineItem();
                    list.Add(item);
                    this.ScanPageTreeAndCreateOutline((PdfPageTree) base2, item);
                }
            }
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if ((this.viewerPreferences != null) && !this.viewerPreferences.IsDefault())
            {
                base[PdfName.ViewerPreferences] = this.viewerPreferences;
            }
            switch (this.pageLayout)
            {
                case PageLayoutType.SinglePage:
                    base[PdfName.PageLayout] = PdfName.SinglePage;
                    break;

                case PageLayoutType.OneColumn:
                    base[PdfName.PageLayout] = PdfName.OneColumn;
                    break;

                case PageLayoutType.TwoColumnLeft:
                    base[PdfName.PageLayout] = PdfName.TwoColumnLeft;
                    break;

                case PageLayoutType.TwoColumnRight:
                    base[PdfName.PageLayout] = PdfName.TwoColumnRight;
                    break;

                case PageLayoutType.TwoPageLeft:
                    base[PdfName.PageLayout] = PdfName.TwoPageLeft;
                    break;

                case PageLayoutType.TwoPageRight:
                    base[PdfName.PageLayout] = PdfName.TwoPageRight;
                    break;
            }
            switch (this.openType)
            {
                case OpenType.UseNone:
                    base[PdfName.PageMode] = PdfName.UseNone;
                    break;

                case OpenType.UseOutlines:
                    base[PdfName.PageMode] = PdfName.UseOutlines;
                    break;

                case OpenType.UseThumbs:
                    base[PdfName.PageMode] = PdfName.UseThumbs;
                    break;

                case OpenType.FullScreen:
                    base[PdfName.PageMode] = PdfName.FullScreen;
                    break;

                case OpenType.UseOC:
                    base[PdfName.PageMode] = PdfName.UseOC;
                    break;

                case OpenType.UseAttachments:
                    base[PdfName.PageMode] = PdfName.UseAttachments;
                    break;
            }
            if (this.outlines.Items.Count <= 0)
            {
                this.CreateDefaultPageOutlines();
            }
            base[PdfName.Outlines] = this.outlines;
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            PdfVersion version = PdfVersion.PDF1_0;
            if ((this.pageLayout == PageLayoutType.TwoPageLeft) || (this.pageLayout == PageLayoutType.TwoPageRight))
            {
                version = PdfVersion.PDF1_5;
            }
            if (this.openType == OpenType.UseOC)
            {
                version = PdfVersion.PDF1_5;
            }
            if (this.openType == OpenType.UseAttachments)
            {
                version = PdfVersion.PDF1_6;
            }
            return version;
        }

        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <value>The info.</value>
        public PdfInfo Info
        {
            get { return  this.info; }
        }

        /// <summary>
        /// A name object specifying how the document should be displayed when opened
        /// </summary>
        /// <value>The page type.</value>
        public OpenType OpenType
        {
            get { return  this.openType; }
            set { this.openType = value; }
        }

        /// <summary>
        /// Gets the outlines.
        /// </summary>
        /// <value>The outlines.</value>
        public PdfOutline Outlines
        {
            get { return  this.outlines; }
        }

        /// <summary>
        /// A name object specifying the page layout to be used when the document is opened
        /// </summary>
        /// <value>The page layout.</value>
        public PageLayoutType PageLayout
        {
            get { return  this.pageLayout; }
            set { this.pageLayout = value; }
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public PdfPageTree Pages
        {
            get { return  (PdfPageTree) base[PdfName.Pages]; }
        }

        /// <summary>
        /// Gets the viewer preferences.
        /// </summary>
        /// <value>The viewer preferences.</value>
        public PdfViewerPreferences ViewerPreferences
        {
            get
            {
                if (this.viewerPreferences == null)
                {
                    this.viewerPreferences = new PdfViewerPreferences();
                }
                return this.viewerPreferences;
            }
        }
    }
}

