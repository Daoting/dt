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
using Dt.Pdf.Drawing;
using Dt.Pdf.Object;
using Dt.Pdf.Object.Filter;
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the PDF file exporter.
    /// </summary>
    internal class PdfExporter
    {
        GcReportContext context;
        PdfDocument currentDoc;
        int dpi;
        readonly Dictionary<ImageSource, Image> imageCaches;
        OpenTypeFontUtility openTypeFontUtility;
        readonly GcReport report;
        readonly PdfExportSettings settings;
        internal const int Upi = 0x48;

        /// <summary>
        /// Occurs when none of the built-in Silverlight fonts are used.
        /// </summary>
        public event EventHandler<ExternalFontEventArgs> ExternalFont;

        /// <summary>
        /// Creates a new PDF exporter.
        /// </summary>
        /// <param name="report">The report object.</param>
        public PdfExporter(GcReport report)
        {
            this.dpi = (int) UnitManager.Dpi;
            this.settings = new PdfExportSettings();
            this.imageCaches = new Dictionary<ImageSource, Image>();
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }
            this.report = report;
        }

        /// <summary>
        /// Creates a new PDF exporter with the specified settings.
        /// </summary>
        /// <param name="report">The report object.</param>
        /// <param name="settings">The settings for the PDF exporter.</param>
        public PdfExporter(GcReport report, PdfExportSettings settings) : this(report)
        {
            if (settings != null)
            {
                this.settings.CopyFrom(settings);
            }
        }

        /// <summary>
        /// Creates the PDF document.
        /// </summary>
        /// <returns></returns>
        internal static PdfDocument CreatePdfDocument()
        {
            return new PdfDocument();
        }

        /// <summary>
        /// Exports the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Export(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanWrite)
            {
                throw new ArgumentException();
            }
            this.ExportInner(stream, null, true, true, true, null, null, -1);
        }

        /// <summary>
        /// Exports the specified file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void Export(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
        }

        /// <summary>
        /// Exports the empty page.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="doc">The document.</param>
        void ExportEmptyPage(ExporterState state, PdfDocument doc)
        {
            PdfPage page = doc.Pages.AddNewPage(this.context.GetDot((float) this.context.PageRects.PageRectangle.Width), this.context.GetDot((float) this.context.PageRects.PageRectangle.Height));
            state.CurrentPage = page;
            PdfContent content = page.AddNewContent();
            content.Filters.Enqueue(PdfFilter.FlateFilter);
            PdfGraphics pdfGraphics = content.Graphics;
            Graphics g = new Graphics(this.context, this, pdfGraphics);
            g.SaveState();
            g.Scale((double) this.context.GetForce(), (double) -this.context.GetForce());
            g.Translate(0.0, -this.context.PageRects.PageRectangle.Height);
            GcLabel label = new GcLabel((int) ((this.context.PageRects.PageRectangle.Width / 2.0) - 220.0), (int) (this.context.PageRects.PageRectangle.Height / 3.0), 440, 100) {
                Text = "No Page can be print, please check the settings.",
                Font = new Font(this.context.DefaultFont.FontFamily.ToString(), 20.0)
            };
            label.Alignment.VerticalAlignment = TextVerticalAlignment.Center;
            label.Alignment.HorizontalAlignment = TextHorizontalAlignment.Center;
            label.Alignment.WordWrap = true;
            label.Foreground = FillEffects.Red;
            label.Background = new SolidColorBrush(Colors.Red);
            GcPaintHelper.PaintLabel(label.GetBlock(this.context), g, state);
            g.RestoreState();
        }

        /// <summary>
        /// Exports the inner.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="doc">The doc.</param>
        /// <param name="saveAuto">if set to <c>true</c> [save auto].</param>
        /// <param name="checkEmpty">if set to <c>true</c> [check empty].</param>
        /// <param name="processSettings">if set to <c>true</c> [process settings].</param>
        /// <param name="otfu">The otfu.</param>
        /// <param name="frc">The FRC.</param>
        /// <param name="pageCount">The page count.</param>
        internal void ExportInner(Stream stream, PdfDocument doc, bool saveAuto, bool checkEmpty, bool processSettings, OpenTypeFontUtility otfu, GcReportContext frc, int pageCount)
        {
            bool flag2;
            doc = doc ?? CreatePdfDocument();
            this.currentDoc = doc;
            if (processSettings)
            {
                this.settings.AppendTo(doc);
            }
            bool flag = false;
            if (otfu != null)
            {
                this.openTypeFontUtility = otfu;
                this.openTypeFontUtility.Dpi = this.Dpi;
            }
            else
            {
                this.openTypeFontUtility = new OpenTypeFontUtility(GcReportContext.defaultFont, this.Dpi);
                flag = true;
                this.openTypeFontUtility.ExternalFont += this.ExternalFont;
            }
            if (frc != null)
            {
                this.context = frc;
            }
            else
            {
                GcReportContext context = new GcReportContext(this.report, this.Dpi, this.openTypeFontUtility) {
                    UnitsPerInch = 0x48
                };
                this.context = context;
                this.context.GeneratePageBlocks();
            }
            if (this.report.Watermark != null)
            {
                this.report.Watermark.Height = (int) this.context.PageRects.PageRectangle.Height;
                this.report.Watermark.Width = (int) this.context.PageRects.PageRectangle.Width;
            }
            ExporterState state = new ExporterState(this.context) {
                BlackAndWhite = this.report.BlackAndWhite
            };
            int num = 0;
            foreach (List<GcPageBlock> list in this.context.Pages)
            {
                state.PageCount += list.Count;
                num = Math.Max(list.Count, num);
            }
            if (pageCount >= 0)
            {
                state.PageCount = pageCount;
            }
            else
            {
                state.PageCount += this.report.FirstPageNumber - 1;
            }
            switch (this.context.Report.PageOrder)
            {
                case PrintPageOrder.Auto:
                    flag2 = this.context.Pages.Count >= num;
                    break;

                case PrintPageOrder.DownThenOver:
                    flag2 = true;
                    break;

                case PrintPageOrder.OverThenDown:
                    flag2 = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            List<PageState> list2 = new List<PageState>();
            if (flag2)
            {
                for (int i = 0; i < num; i++)
                {
                    state.CurrentHPageNumber++;
                    state.CurrentVPageNumber = 0;
                    foreach (List<GcPageBlock> list3 in this.context.Pages)
                    {
                        if (list3.Count > i)
                        {
                            state.CurrentPageNumber++;
                            state.CurrentVPageNumber++;
                            list2.Add(new PageState(state.CurrentHPageNumber, state.CurrentVPageNumber, state.CurrentPageNumber, list3[i]));
                        }
                    }
                }
            }
            else
            {
                foreach (List<GcPageBlock> list4 in this.context.Pages)
                {
                    state.CurrentVPageNumber++;
                    state.CurrentHPageNumber = 0;
                    foreach (GcPageBlock block in list4)
                    {
                        state.CurrentPageNumber++;
                        state.CurrentHPageNumber++;
                        list2.Add(new PageState(state.CurrentHPageNumber, state.CurrentVPageNumber, state.CurrentPageNumber, block));
                    }
                }
            }
            if (list2.Count > 0)
            {
                if (string.IsNullOrEmpty(this.report.PageRange))
                {
                    foreach (PageState state2 in list2)
                    {
                        state.CurrentHPageNumber = state2.HPageIndex;
                        state.CurrentVPageNumber = state2.VPageIndex;
                        state.CurrentPageNumber = (state2.PageIndex + this.report.FirstPageNumber) - 1;
                        this.ExportPage(state, doc, state2.PageBlock);
                    }
                }
                else
                {
                    foreach (int num3 in Utilities.GetPageRange(this.report.PageRange, list2.Count))
                    {
                        PageState state3 = list2[num3 - 1];
                        state.CurrentHPageNumber = state3.HPageIndex;
                        state.CurrentVPageNumber = state3.VPageIndex;
                        state.CurrentPageNumber = (state3.PageIndex + this.report.FirstPageNumber) - 1;
                        this.ExportPage(state, doc, state3.PageBlock);
                    }
                }
            }
            if (checkEmpty && (doc.Pages.PageCount <= 0))
            {
                this.ExportEmptyPage(state, doc);
            }
            if (state.Bookmarks.Count > 0)
            {
                PdfOutlineItem item;
                if (doc.Outlines.Items.Count > 0)
                {
                    item = doc.Outlines.Items[0];
                }
                else
                {
                    item = new PdfOutlineItem(this.report.Bookmark, state.FirstPage);
                    doc.Outlines.Items.Add(item);
                }
                Dictionary<Bookmark, List<PdfOutlineItem>> dictionary = new Dictionary<Bookmark, List<PdfOutlineItem>>();
                foreach (ExporterState.BookmarkState state4 in state.Bookmarks)
                {
                    PdfOutlineItem item2 = new PdfOutlineItem(state4.Bookmark.Text, new PdfXYZDestination(state4.Page, (float) state4.Location.X, (float) state4.Location.Y, 0f));
                    if (!dictionary.ContainsKey(state4.Bookmark))
                    {
                        dictionary.Add(state4.Bookmark, new List<PdfOutlineItem>());
                    }
                    dictionary[state4.Bookmark].Add(item2);
                }
                foreach (KeyValuePair<Bookmark, List<PdfOutlineItem>> pair in dictionary)
                {
                    Bookmark bookmark = pair.Key;
                    List<PdfOutlineItem> list6 = pair.Value;
                    if ((bookmark.Parent == null) || !dictionary.ContainsKey(bookmark.Parent))
                    {
                        foreach (PdfOutlineItem item3 in list6)
                        {
                            item.ChildItems.Add(item3);
                        }
                    }
                    else
                    {
                        List<PdfOutlineItem> list7 = dictionary[bookmark.Parent];
                        foreach (PdfOutlineItem item4 in list6)
                        {
                            PdfOutlineItem item5 = list7[0];
                            if (list7.Count > 1)
                            {
                                foreach (PdfOutlineItem item6 in list7)
                                {
                                    if (item6.Dest.TargetPage == item4.Dest.TargetPage)
                                    {
                                        item5 = item6;
                                        break;
                                    }
                                }
                            }
                            item5.ChildItems.Add(item4);
                        }
                    }
                }
            }
            if (processSettings)
            {
                if ((this.settings.DestinationType != DestinationType.Auto) || (doc.Pages.PageCount > 0))
                {
                    PdfArray array = new PdfArray();
                    if ((this.settings.OpenPageNumber <= doc.Pages.PageCount) && (doc.Pages.PageCount > 0))
                    {
                        array.Add(doc.Pages.Kids[this.settings.OpenPageNumber - 1]);
                    }
                    else
                    {
                        array.Add(doc.Pages.Kids[0]);
                    }
                    switch (this.settings.DestinationType)
                    {
                        case DestinationType.Auto:
                            break;

                        case DestinationType.FitPage:
                            array.Add(PdfName.Fit);
                            break;

                        case DestinationType.FitWidth:
                            array.Add(PdfName.FitH);
                            break;

                        case DestinationType.FitHeight:
                            array.Add(PdfName.FitV);
                            array.Add(PdfNumber.Zero);
                            break;

                        case DestinationType.FitBox:
                            array.Add(PdfName.FitB);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    doc.Add(PdfName.OpenAction, array);
                }
                if ((this.settings.DocumentAttachments.Count > 0) && (doc.Pages.PageCount > 0))
                {
                    PdfPage page = doc.Pages.Kids[doc.Pages.PageCount - 1] as PdfPage;
                    if (page != null)
                    {
                        foreach (DocumentAttachment attachment in this.settings.DocumentAttachments)
                        {
                            if ((!string.IsNullOrEmpty(attachment.Name) && (attachment.FileStreamInner != null)) && attachment.FileStreamInner.CanRead)
                            {
                                PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation {
                                    FileSpecification = { FileName = attachment.Name }
                                };
                                attachment.FileStreamInner.WriteTo((Stream) annotation.FileSpecification.FileStream.Data);
                                annotation.FileSpecification.FileStream.Size = attachment.FileStreamInner.Length;
                                annotation.FileSpecification.FileStream.MIME = attachment.ContentType;
                                annotation.ModifiedDate = DateTime.Now;
                                annotation.FileSpecification.FileStream.ModifyDate = DateTime.Now;
                                annotation.FileSpecification.FileStream.CreationDate = DateTime.Now;
                                if (!string.IsNullOrEmpty(attachment.Description))
                                {
                                    annotation.Contents = attachment.Description;
                                }
                                if (attachment.Compress)
                                {
                                    annotation.FileSpecification.FileStream.Filters.Enqueue(PdfFilter.FlateFilter);
                                }
                                annotation.Flags = PdfAnnotationBase.Flag.NoView | PdfAnnotationBase.Flag.Hidden | PdfAnnotationBase.Flag.Invisible;
                                annotation.Rectangle = new PdfRectangle(-100f, -100f, 1f, 1f);
                                page.Annotations.Add(annotation);
                            }
                        }
                    }
                }
            }
            if (saveAuto)
            {
                SavePdfDocment(doc, stream);
            }
            if (flag)
            {
                this.openTypeFontUtility.ExternalFont -= this.ExternalFont;
            }
        }

        /// <summary>
        /// Exports the page.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="doc">The document.</param>
        /// <param name="page">The page.</param>
        void ExportPage(ExporterState state, PdfDocument doc, GcPageBlock page)
        {
            PdfPage page2 = doc.Pages.AddNewPage(this.context.GetDot((float) page.Width), this.context.GetDot((float) page.Height));
            if (this.settings.PageDuration >= 0)
            {
                page2.PageDuration = this.settings.PageDuration;
            }
            if (this.settings.PageTransition != PageTransitionType.Default)
            {
                page2.PageTransition.TransitionType = this.settings.PageTransition;
            }
            state.CurrentPage = page2;
            PdfContent content = page2.AddNewContent();
            content.Filters.Enqueue(PdfFilter.FlateFilter);
            PdfGraphics pdfGraphics = content.Graphics;
            Graphics g = new Graphics(this.context, this, pdfGraphics);
            if (state.BlackAndWhite)
            {
                g.GrayMode = true;
            }
            g.SaveState();
            g.Scale((double) this.context.GetForce(), (double) -this.context.GetForce());
            g.Translate(0.0, -page.Height);
            new GcPaintHelper().Paint(page, g, state);
            g.RestoreState();
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        internal BaseFont GetFont(Font font)
        {
            return this.openTypeFontUtility.GetFont(font);
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns></returns>
        internal Image GetImage(ImageSource source)
        {
            if (source == null)
            {
                return null;
            }
            if (this.imageCaches.ContainsKey(source))
            {
                return this.imageCaches[source];
            }
            Windows.UI.Xaml.Media.Imaging.BitmapImage image = source as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            Stream stream = null;
            try
            {
                if ((image != null) && (image.UriSource != null))
                {
                    try
                    {
                        Uri uri1 = ((Windows.UI.Xaml.Media.Imaging.BitmapImage)source).UriSource;
                        Uri uri = new Uri("ms-appx:///" + uri1.LocalPath.TrimStart(new char[] { '/' }));
                        stream = WindowsRuntimeStreamExtensions.AsStreamForRead(StorageFile.GetFileFromApplicationUriAsync(uri).GetResultSynchronously<StorageFile>().OpenSequentialReadAsync().GetResultSynchronously<IInputStream>());
                    }
                    catch (Exception)
                    {
                        stream = Utility.GetImageStream(source, ImageFormat.Png, PictureSerializationMode.Normal);
                    }
                }
                else if (image != null)
                {
                    stream = Utility.GetImageStream(source, ImageFormat.Png, PictureSerializationMode.Normal);
                }
                if (stream != null)
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    Image instance = Image.GetInstance(buffer);
                    this.imageCaches.Add(source, instance);
                    stream.Dispose();
                    return instance;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <remarks>The PageRange property of the GcReport class is ignored.</remarks>
        /// <returns>Returns the number of pages in the PDF file.</returns>
        public int GetPageCount()
        {
            this.openTypeFontUtility = new OpenTypeFontUtility(GcReportContext.defaultFont, this.Dpi);
            GcReportContext context = new GcReportContext(this.report, this.Dpi, this.openTypeFontUtility) {
                UnitsPerInch = 0x48
            };
            this.context = context;
            this.context.GeneratePageBlocks();
            int num = 0;
            foreach (List<GcPageBlock> list in this.context.Pages)
            {
                num += list.Count;
            }
            return num;
        }

        /// <summary>
        /// Saves the PDF document.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="stream">The stream.</param>
        internal static void SavePdfDocment(PdfDocument doc, Stream stream)
        {
            doc.Save(stream);
        }

        /// <summary>
        /// Internal only.
        /// Gets the current document.
        /// </summary>
        /// <value>The current document.</value>
        internal PdfDocument CurrentDocument
        {
            get { return  this.currentDoc; }
        }

        /// <summary>
        /// Gets or sets the dpi.
        /// </summary>
        /// <value>The dpi.</value>
        internal int Dpi
        {
            get { return  this.dpi; }
            set { this.dpi = value; }
        }

        /// <summary>
        /// Gets the settings of the PDF exporter.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>The default value is never null.</remarks>
        public PdfExportSettings Settings
        {
            get { return  this.settings; }
        }

        /// <summary>
        /// PageState, Internal only.
        /// </summary>
        class PageState
        {
            int hPageIndex;
            GcPageBlock pageBlock;
            int pageIndex;
            int vPageIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PdfExporter.PageState" /> class.
            /// </summary>
            /// <param name="hPageIndex">Index of the horizontal page.</param>
            /// <param name="vPageIndex">Index of the vertical page.</param>
            /// <param name="pageIndex">Index of the page.</param>
            /// <param name="pageBlock">The page block.</param>
            public PageState(int hPageIndex, int vPageIndex, int pageIndex, GcPageBlock pageBlock)
            {
                this.hPageIndex = hPageIndex;
                this.vPageIndex = vPageIndex;
                this.pageIndex = pageIndex;
                this.pageBlock = pageBlock;
            }

            /// <summary>
            /// Gets or sets the horizontal page index.
            /// </summary>
            /// <value>The index of the horizontal page.</value>
            public int HPageIndex
            {
                get { return  this.hPageIndex; }
                set { this.hPageIndex = value; }
            }

            /// <summary>
            /// Gets or sets the page block.
            /// </summary>
            /// <value>The page block.</value>
            public GcPageBlock PageBlock
            {
                get { return  this.pageBlock; }
                set { this.pageBlock = value; }
            }

            /// <summary>
            /// Gets or sets the index of the page.
            /// </summary>
            /// <value>The index of the page.</value>
            public int PageIndex
            {
                get { return  this.pageIndex; }
                set { this.pageIndex = value; }
            }

            /// <summary>
            /// Gets or sets the vertical page index.
            /// </summary>
            /// <value>The index of the vertical page.</value>
            public int VPageIndex
            {
                get { return  this.vPageIndex; }
                set { this.vPageIndex = value; }
            }
        }
    }
}

