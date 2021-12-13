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
using Dt.Pdf.Drawing;
using Dt.Pdf.Exceptions;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The page object of Pdf
    /// </summary>
    public class PdfPage : PdfDictionary, IGraphicsStream
    {
        /// <summary>
        /// Annotations
        /// </summary>
        private readonly List<PdfAnnotationBase> annotations;
        /// <summary>
        /// pageDuration
        /// </summary>
        private int pageDuration;
        /// <summary>
        /// pageTransition
        /// </summary>
        private PdfPageTransition pageTransition;
        /// <summary>
        /// Warning: can be null
        /// </summary>
        private readonly PdfDocument pdf;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPage" /> class.
        /// </summary>
        public PdfPage()
        {
            this.annotations = new List<PdfAnnotationBase>();
            this.pageDuration = -1;
            this.pageTransition = new PdfPageTransition();
            base.Add(PdfName.Type, PdfName.Page);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPage" /> class.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public PdfPage(PdfRectangle rect) : this(rect, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPage" /> class.
        /// </summary>
        /// <param name="pdf">The PDF.</param>
        internal PdfPage(PdfDocument pdf) : this()
        {
            this.pdf = pdf;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPage" /> class.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="pdf">The PDF.</param>
        internal PdfPage(PdfRectangle rect, PdfDocument pdf) : this(pdf)
        {
            if (rect == null)
            {
                throw new PdfArgumentNullException("rect");
            }
            base.Add(PdfName.MediaBox, rect);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPage" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfPage(float width, float height) : this(width, height, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPage" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="pdf">The PDF.</param>
        internal PdfPage(float width, float height, PdfDocument pdf) : this(new PdfRectangle(0f, 0f, width, height), pdf)
        {
        }

        /// <summary>
        /// Adds the new content.
        /// </summary>
        /// <returns></returns>
        public PdfContent AddNewContent()
        {
            PdfContent content = new PdfContent(this);
            if (this.LastContent == null)
            {
                base.Add(PdfName.Contents, content);
                return content;
            }
            PdfObjectBase base2 = base[PdfName.Contents];
            if (base2 is PdfContent)
            {
                PdfArray array = new PdfArray {
                    base2,
                    content
                };
                base[PdfName.Contents] = array;
                return content;
            }
            if (base2 is PdfArray)
            {
                ((PdfArray) base2).Add(content);
            }
            return content;
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns></returns>
        public PdfResources GetResources()
        {
            if (base.ContainsKey(PdfName.Resources))
            {
                return (base[PdfName.Resources] as PdfResources);
            }
            PdfResources resources = (this.pdf != null) ? new PdfResources(this.pdf.ResourcesManager) : new PdfResources();
            resources.IsLabeled = true;
            base.Add(PdfName.Resources, resources);
            return resources;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.annotations.Count > 0)
            {
                base[PdfName.Annots] = new PdfArray(this.annotations.ToArray());
            }
            if (this.pageDuration >= 0)
            {
                base[PdfName.Dur] = new PdfNumber((double) this.pageDuration);
            }
            if (this.pageTransition.TransitionType != PageTransitionType.Default)
            {
                base[PdfName.Trans] = this.pageTransition;
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the annotations.
        /// </summary>
        /// <value>The annotations.</value>
        public List<PdfAnnotationBase> Annotations
        {
            get { return  this.annotations; }
        }

        /// <summary>
        /// Gets the last content.
        /// </summary>
        /// <value>The last content.</value>
        public PdfContent LastContent
        {
            get
            {
                if (!base.ContainsKey(PdfName.Contents))
                {
                    return null;
                }
                PdfObjectBase base2 = base[PdfName.Contents];
                if (base2 is PdfContent)
                {
                    return (PdfContent) base2;
                }
                if (!(base2 is PdfArray))
                {
                    throw new PdfException("Unkown type in Contents of Page.");
                }
                return (PdfContent) ((PdfArray) base2).Last;
            }
        }

        /// <summary>
        /// Gets the media box.
        /// </summary>
        /// <value>The media box.</value>
        public PdfRectangle MediaBox
        {
            get { return  (PdfRectangle) base[PdfName.MediaBox]; }
        }

        /// <summary>
        /// The page¡¯s display duration (also called its advance timing): the maximum length of time, in seconds, 
        /// that the page is displayed during presentations before the viewer 
        /// application automatically advances to the next page.
        /// </summary>
        public int PageDuration
        {
            get { return  this.pageDuration; }
            set { this.pageDuration = value; }
        }

        /// <summary>
        /// Describing the transition effect to be used when displaying the page during presentations.
        /// </summary>
        public PdfPageTransition PageTransition
        {
            get { return  this.pageTransition; }
            set { this.pageTransition = value; }
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public PdfResources Resources
        {
            get { return  this.GetResources(); }
        }
    }
}

