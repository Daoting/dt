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
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The watermark annotation
    /// </summary>
    public class PdfWatermarkAnnotation : PdfAnnotationBase
    {
        private PdfFixedPrint fixedPrint;
        private PdfFormXObject formXObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfWatermarkAnnotation" /> class.
        /// </summary>
        public PdfWatermarkAnnotation() : base(PdfName.Watermark)
        {
            this.fixedPrint = new PdfFixedPrint();
            this.formXObject = new PdfFormXObject();
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns></returns>
        protected override PdfVersion GetVersion()
        {
            return PdfVersion.PDF1_6;
        }

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteProperties(PdfWriter writer)
        {
            base[PdfName.FixedPrint] = this.fixedPrint;
            base[PdfName.AP] = new PdfAppearanceStream(this.formXObject);
        }

        /// <summary>
        /// Gets the fixed print.
        /// </summary>
        /// <value>The fixed print.</value>
        public PdfFixedPrint FixedPrint
        {
            get { return  this.fixedPrint; }
        }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public PdfGraphics Graphics
        {
            get { return  this.formXObject.Graphics; }
        }
    }
}

