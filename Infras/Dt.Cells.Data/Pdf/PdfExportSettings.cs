#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the settings for the PDF exporter.
    /// </summary>
    public class PdfExportSettings
    {
        string author;
        bool centerWindow;
        string creator;
        Dt.Cells.Data.DestinationType destinationType;
        bool displayDocTitle;
        readonly List<DocumentAttachment> documentAttachments = new List<DocumentAttachment>();
        bool fitWindow;
        bool hideMenubar;
        bool hideToolbar;
        bool hideWindowUI;
        string keywords;
        int openPageNumber = 1;
        Dt.Pdf.Object.OpenType openType;
        int pageDuration = -1;
        PageLayoutType pageLayout;
        PageTransitionType pageTransition;
        readonly Dt.Cells.Data.PrintPreset printPreset = new Dt.Cells.Data.PrintPreset();
        string subject;
        string title;

        /// <summary>
        /// Appends to PdfDocument.
        /// </summary>
        /// <param name="doc">The document</param>
        internal void AppendTo(PdfDocument doc)
        {
            if (!string.IsNullOrEmpty(this.Author))
            {
                doc.Info.Author = this.Author;
            }
            if (!string.IsNullOrEmpty(this.Title))
            {
                doc.Info.Title = this.Title;
            }
            if (!string.IsNullOrEmpty(this.Subject))
            {
                doc.Info.Subject = this.Subject;
            }
            if (!string.IsNullOrEmpty(this.Keywords))
            {
                doc.Info.Keywords = this.Keywords;
            }
            if (!string.IsNullOrEmpty(this.Creator))
            {
                doc.Info.Creator = this.Creator;
            }
            doc.ViewerPreferences.CenterWindow = this.CenterWindow;
            doc.ViewerPreferences.DisplayDocTitle = this.DisplayDocTitle;
            doc.ViewerPreferences.HideMenubar = this.HideMenubar;
            doc.ViewerPreferences.HideToolbar = this.HideToolbar;
            doc.ViewerPreferences.HideWindowUI = this.HideWindowUI;
            doc.ViewerPreferences.FitWindow = this.FitWindow;
            doc.PageLayout = this.PageLayout;
            doc.OpenType = this.OpenType;
            this.printPreset.AppendTo(doc);
        }

        /// <summary>
        /// Copies from other settings.
        /// </summary>
        /// <param name="settings">The settings</param>
        internal void CopyFrom(PdfExportSettings settings)
        {
            if (settings != null)
            {
                this.author = settings.author;
                this.title = settings.title;
                this.subject = settings.subject;
                this.keywords = settings.keywords;
                this.creator = settings.creator;
                this.centerWindow = settings.centerWindow;
                this.displayDocTitle = settings.displayDocTitle;
                this.hideMenubar = settings.hideMenubar;
                this.hideToolbar = settings.hideToolbar;
                this.hideWindowUI = settings.hideWindowUI;
                this.fitWindow = settings.fitWindow;
                this.pageLayout = settings.pageLayout;
                this.openType = settings.openType;
                this.pageDuration = settings.pageDuration;
                this.pageTransition = settings.pageTransition;
                this.destinationType = settings.destinationType;
                this.openPageNumber = settings.openPageNumber;
                this.printPreset.CopyFrom(settings.printPreset);
                this.documentAttachments.AddRange((IEnumerable<DocumentAttachment>) settings.documentAttachments);
            }
        }

        /// <summary>
        /// Gets or sets the author of the PDF file.
        /// </summary>
        /// <value>The author. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Author
        {
            get { return  this.author; }
            set { this.author = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to center the document in the window.
        /// </summary>
        /// <value><c>true</c> if the document is to be centered in the display window; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
        [DefaultValue(false)]
        public bool CenterWindow
        {
            get { return  this.centerWindow; }
            set { this.centerWindow = value; }
        }

        /// <summary>
        /// Gets or sets the creator of the PDF file.
        /// </summary>
        /// <value>The creator. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Creator
        {
            get { return  this.creator; }
            set { this.creator = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates how to open the document. 
        /// </summary>
        /// <value>A value that indicates how to open the document. The default value is <see cref="P:Dt.Cells.Data.PdfExportSettings.DestinationType">Auto</see>.</value>
        [DefaultValue(0)]
        public Dt.Cells.Data.DestinationType DestinationType
        {
            get { return  this.destinationType; }
            set { this.destinationType = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the window's title bar should display
        /// the title specified for the PDF file.
        /// </summary>
        /// /// <remarks>
        /// This property sets a flag that specifies whether the window’s title bar should display
        /// the document title taken from the Title entry of the document
        /// information dictionary. Set the title by setting the <see cref="P:Dt.Cells.Data.PdfExportSettings.Title" /> property.
        /// If the DisplayDocTitle property is false, the title bar 
        /// displays the name of the PDF file instead.
        /// </remarks>
        /// <value>
        /// <c>true</c> if the window's title bar displays the title specified for the PDF file; otherwise, <c>false</c>. 
        /// The default value is <c>false</c>, which means the window's title bar displays the PDF file name.
        /// </value>
        [DefaultValue(false)]
        public bool DisplayDocTitle
        {
            get { return  this.displayDocTitle; }
            set { this.displayDocTitle = value; }
        }

        /// <summary>
        /// Gets the document attachments.
        /// </summary>
        /// <value>The document attachments. The default value is never null.</value>
        public List<DocumentAttachment> DocumentAttachments
        {
            get { return  this.documentAttachments; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to resize the document’s window
        /// to fit the size of the first displayed page.
        /// </summary>
        /// <value><c>true</c> if the document's window is resized; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool FitWindow
        {
            get { return  this.fitWindow; }
            set { this.fitWindow = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to hide the viewer application’s
        /// menu bar when the document is active.
        /// </summary>
        /// <value><c>true</c> if the viewer application hides its menu bar; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
        [DefaultValue(false)]
        public bool HideMenubar
        {
            get { return  this.hideMenubar; }
            set { this.hideMenubar = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to hide the viewer application’s
        /// toolbars when the document is active.
        /// </summary>
        /// <value><c>true</c> if the viewer application hides its toolbars; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
        [DefaultValue(false)]
        public bool HideToolbar
        {
            get { return  this.hideToolbar; }
            set { this.hideToolbar = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to hide user interface elements for the document.
        /// </summary>
        /// /// <remarks>
        /// This property specifies whether to hide user interface elements, such as scroll bars and navigation controls, when displaying the document.
        /// Only the document’s contents are displayed if this property is true.
        /// </remarks>
        /// <value><c>true</c> if the document's user interface elements are hidden; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
        [DefaultValue(false)]
        public bool HideWindowUI
        {
            get { return  this.hideWindowUI; }
            set { this.hideWindowUI = value; }
        }

        /// <summary>
        /// Gets or sets the keywords for the PDF file.
        /// </summary>
        /// <value>The keywords. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Keywords
        {
            get { return  this.keywords; }
            set { this.keywords = value; }
        }

        /// <summary>
        /// Gets or sets the page number of the page to display when the document is opened.
        /// </summary>
        /// <value>The page number to display. The default value is 1.</value>
        [DefaultValue(1)]
        public int OpenPageNumber
        {
            get { return  this.openPageNumber; }
            set { this.openPageNumber = value; }
        }

        /// <summary>
        /// Gets or sets how to display the document when it is opened.
        /// </summary>
        /// <value>A value that specifies how to display the document. The default value is <see cref="P:Dt.Cells.Data.PdfExportSettings.OpenType">Auto</see>.</value>
        [DefaultValue(0)]
        public Dt.Pdf.Object.OpenType OpenType
        {
            get { return  this.openType; }
            set { this.openType = value; }
        }

        /// <summary>
        /// Gets or sets an integer value that indicates the duration in seconds for the current page during a presentation.
        /// </summary>
        /// <value>The duration of the page.</value>
        [DefaultValue(-1)]
        public int PageDuration
        {
            get { return  this.pageDuration; }
            set { this.pageDuration = value; }
        }

        /// <summary>
        /// Gets or sets the page layout to be used when the document is opened.
        /// </summary>
        /// <value>A value that specifies the page layout for the document. The default value is <see cref="T:Dt.Pdf.Object.PageLayoutType">Auto</see>.</value>
        [DefaultValue(0)]
        public PageLayoutType PageLayout
        {
            get { return  this.pageLayout; }
            set { this.pageLayout = value; }
        }

        /// <summary>
        /// Gets or sets the transition style to use when moving to this page from another during a presentation.
        /// </summary>
        /// <value>The page transition. The default value is <see cref="T:Dt.Pdf.Object.PageTransitionType">Default</see>.</value>
        [DefaultValue(0)]
        public PageTransitionType PageTransition
        {
            get { return  this.pageTransition; }
            set { this.pageTransition = value; }
        }

        /// <summary>
        /// Gets the print settings for the PDF viewer.
        /// </summary>
        /// <value>The print settings for the viewer. The default value is never null.</value>
        public Dt.Cells.Data.PrintPreset PrintPreset
        {
            get { return  this.printPreset; }
        }

        /// <summary>
        /// Gets or sets the subject of the PDF file.
        /// </summary>
        /// <value>The subject. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Subject
        {
            get { return  this.subject; }
            set { this.subject = value; }
        }

        /// <summary>
        /// Gets or sets the title of the PDF file.
        /// </summary>
        /// <value>The title. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Title
        {
            get { return  this.title; }
            set { this.title = value; }
        }
    }
}

