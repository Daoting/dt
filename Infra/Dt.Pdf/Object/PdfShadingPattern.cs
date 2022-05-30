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
    /// The shading pattern object
    /// </summary>
    public class PdfShadingPattern : PdfVersionDependDictionary
    {
        private readonly PdfShading shading;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfShadingPattern" /> class.
        /// </summary>
        public PdfShadingPattern()
        {
            base.version = PdfVersion.PDF1_3;
            base.Add(PdfName.Type, PdfName.Pattern);
            base.Add(PdfName.PatternType, PdfNumber.Two);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfShadingPattern" /> class.
        /// </summary>
        /// <param name="shading">The shading.</param>
        public PdfShadingPattern(PdfShading shading) : this()
        {
            this.shading = shading;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Shading] = this.shading;
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the shading.
        /// </summary>
        /// <value>The shading.</value>
        public PdfShading Shading
        {
            get { return  this.shading; }
        }
    }
}

