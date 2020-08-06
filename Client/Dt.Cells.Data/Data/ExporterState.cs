#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Object;
using System;
using System.Collections.Generic;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// ExporterState
    /// </summary>
    internal class ExporterState
    {
        bool blackAndWhite;
        readonly List<BookmarkState> bookmarks = new List<BookmarkState>();
        readonly GcReportContext context;
        int currentHPageNumber;
        PdfPage currentPage;
        int currentPageNumber;
        int currentVPageNumber;
        System.DateTime dateTime = System.DateTime.Now;
        PdfPage firstPage;
        int pageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ExporterState" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ExporterState(GcReportContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets or sets whether to print the colors as they appear on the screen.  
        /// </summary>
        public bool BlackAndWhite
        {
            get { return  this.blackAndWhite; }
            set { this.blackAndWhite = value; }
        }

        /// <summary>
        /// Gets the bookmarks.
        /// </summary>
        /// <value>The bookmarks.</value>
        public List<BookmarkState> Bookmarks
        {
            get { return  this.bookmarks; }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public GcReportContext Context
        {
            get { return  this.context; }
        }

        /// <summary>
        /// Gets or sets the current horizontal page number.
        /// </summary>
        /// <value>The current horizontal page number.</value>
        /// <remarks>
        /// Pages can be displayed in a grid format, with multiple pages in the horizontal and vertical directions.
        /// Use this property and the <see cref="P:Dt.Cells.Data.ExporterState.CurrentVPageNumber" /> property to set or return the current page numbers.
        /// </remarks>
        /// <seealso cref="P:Dt.Cells.Data.ExporterState.CurrentVPageNumber" />
        public int CurrentHPageNumber
        {
            get { return  this.currentHPageNumber; }
            set { this.currentHPageNumber = value; }
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public PdfPage CurrentPage
        {
            get { return  this.currentPage; }
            set
            {
                if (this.FirstPage == null)
                {
                    this.FirstPage = value;
                }
                this.currentPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        /// <value>The current page number.</value>
        public int CurrentPageNumber
        {
            get { return  this.currentPageNumber; }
            set { this.currentPageNumber = value; }
        }

        /// <summary>
        /// Gets or sets the current vertical page number.
        /// </summary>
        /// <value>The current vertical page number.</value>
        /// <remarks>
        /// Pages can be displayed in a grid format, with multiple pages in the horizontal and vertical directions.
        /// Use this property and the <see cref="P:Dt.Cells.Data.ExporterState.CurrentHPageNumber" /> property to set or return the current page numbers.
        /// </remarks>
        /// <seealso cref="P:Dt.Cells.Data.ExporterState.CurrentHPageNumber" />
        public int CurrentVPageNumber
        {
            get { return  this.currentVPageNumber; }
            set { this.currentVPageNumber = value; }
        }

        /// <summary>
        /// Gets or sets the date and time.
        /// </summary>
        /// <value>The date and time.</value>
        public System.DateTime DateTime
        {
            get { return  this.dateTime; }
            set { this.dateTime = value; }
        }

        /// <summary>
        /// Gets or sets the first page.
        /// </summary>
        /// <value>The first page.</value>
        public PdfPage FirstPage
        {
            get { return  this.firstPage; }
            set { this.firstPage = value; }
        }

        /// <summary>
        /// Gets or sets the page count.
        /// </summary>
        /// <value>The page count.</value>
        public int PageCount
        {
            get { return  this.pageCount; }
            set { this.pageCount = value; }
        }

        public class BookmarkState
        {
            Dt.Cells.Data.Bookmark bookmark;
            Windows.Foundation.Point location;
            PdfPage page;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ExporterState.BookmarkState" /> class.
            /// </summary>
            /// <param name="bookmark">The bookmark.</param>
            /// <param name="page">The page.</param>
            /// <param name="location">The location.</param>
            public BookmarkState(Dt.Cells.Data.Bookmark bookmark, PdfPage page, Windows.Foundation.Point location)
            {
                this.bookmark = bookmark;
                this.page = page;
                this.location = location;
            }

            /// <summary>
            /// Gets or sets the bookmark.
            /// </summary>
            /// <value>The bookmark.</value>
            public Dt.Cells.Data.Bookmark Bookmark
            {
                get { return  this.bookmark; }
                set { this.bookmark = value; }
            }

            /// <summary>
            /// Gets or sets the location.
            /// </summary>
            /// <value>The location.</value>
            public Windows.Foundation.Point Location
            {
                get { return  this.location; }
                set { this.location = value; }
            }

            /// <summary>
            /// Gets or sets the page.
            /// </summary>
            /// <value>The page.</value>
            public PdfPage Page
            {
                get { return  this.page; }
                set { this.page = value; }
            }
        }
    }
}

