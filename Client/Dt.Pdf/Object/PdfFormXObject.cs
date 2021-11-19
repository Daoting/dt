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
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The Pdf form xobject
    /// </summary>
    public class PdfFormXObject : PdfStream, IVersionDepend, IGraphicsStream
    {
        private PdfRectangle bbox = new PdfRectangle(0f, 0f, 1000f, 1000f);
        private readonly PdfGraphics graphics;
        private PdfResources resources = new PdfResources();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFormXObject" /> class.
        /// </summary>
        public PdfFormXObject()
        {
            base.Properties.Add(PdfName.Type, PdfName.XObject);
            base.Properties.Add(PdfName.Subtype, PdfName.Form);
            this.graphics = new PdfGraphics(base.Psw, this);
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns></returns>
        public PdfResources GetResources()
        {
            return this.resources;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base.Properties[PdfName.Resources] = this.resources;
            base.Properties[PdfName.BBox] = this.bbox;
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            return PdfVersion.PDF1_2;
        }

        /// <summary>
        /// Gets or sets the Bound box.
        /// </summary>
        /// <value>The B box.</value>
        public PdfRectangle BBox
        {
            get { return  this.bbox; }
            set { this.bbox = value; }
        }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public PdfGraphics Graphics
        {
            get { return  this.graphics; }
        }
    }
}

