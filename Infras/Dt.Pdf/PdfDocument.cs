#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Object;
using System;
using System.IO;
#endregion

namespace Dt.Pdf
{
    /// <summary>
    /// The pdf document
    /// </summary>
    public class PdfDocument
    {
        private readonly PdfDocumentCatalog pdc;
        private readonly PdfResourcesManager resourcesManager = new PdfResourcesManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.PdfDocument" /> class.
        /// </summary>
        public PdfDocument()
        {
            this.pdc = new PdfDocumentCatalog(this);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(PdfName key, PdfObjectBase value)
        {
            this.pdc.Add(key, value);
        }

        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new PdfArgumentNullException("stream");
            }
            if (!stream.CanWrite)
            {
                throw new PdfArgumentException("stream");
            }
            new PdfWriter(stream).Start(this.pdc);
        }

        /// <summary>
        /// Gets the information of Pdf.
        /// </summary>
        /// <value>The info.</value>
        public PdfInfo Info
        {
            get { return  this.pdc.Info; }
        }

        /// <summary>
        /// Gets or sets the type of the open.
        /// </summary>
        /// <value>The type of the open.</value>
        public OpenType OpenType
        {
            get { return  this.pdc.OpenType; }
            set { this.pdc.OpenType = value; }
        }

        /// <summary>
        /// Gets the outlines.
        /// </summary>
        /// <value>The outlines.</value>
        public PdfOutline Outlines
        {
            get { return  this.pdc.Outlines; }
        }

        /// <summary>
        /// Gets or sets the page layout.
        /// </summary>
        /// <value>The page layout.</value>
        public PageLayoutType PageLayout
        {
            get { return  this.pdc.PageLayout; }
            set { this.pdc.PageLayout = value; }
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public PdfPageTree Pages
        {
            get { return  this.pdc.Pages; }
        }

        /// <summary>
        /// Gets the resources manager.
        /// </summary>
        /// <value>The resources manager.</value>
        internal PdfResourcesManager ResourcesManager
        {
            get { return  this.resourcesManager; }
        }

        /// <summary>
        /// Gets the viewer preferences.
        /// </summary>
        /// <value>The viewer preferences.</value>
        public PdfViewerPreferences ViewerPreferences
        {
            get { return  this.pdc.ViewerPreferences; }
        }
    }
}

