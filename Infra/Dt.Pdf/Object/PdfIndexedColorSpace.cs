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
    /// An Indexed color space allows a PDF content stream 
    /// to use small integers as indices into a color map 
    /// or color table of arbitrary colors in some other space.
    /// </summary>
    public class PdfIndexedColorSpace : PdfArray
    {
        private PdfObjectBase baseColorSpace;
        private int hival;
        private byte[] lookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfIndexedColorSpace" /> class.
        /// </summary>
        /// <param name="baseColorSpace">The base color space.</param>
        /// <param name="hival">The hival.</param>
        /// <param name="lookup">The lookup.</param>
        public PdfIndexedColorSpace(PdfObjectBase baseColorSpace, int hival, byte[] lookup)
        {
            base.isLabeled = true;
            this.baseColorSpace = baseColorSpace;
            this.hival = hival;
            this.lookup = lookup;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base.Clear();
            base.Add(PdfName.Indexed);
            base.Add(this.baseColorSpace);
            base.Add(new PdfNumber((double) this.hival));
            PdfString item = new PdfString(this.lookup) {
                IsHexMode = true
            };
            base.Add(item);
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets the base color space.
        /// </summary>
        /// <value>The base color space.</value>
        public PdfObjectBase BaseColorSpace
        {
            get { return  this.baseColorSpace; }
            set { this.baseColorSpace = value; }
        }

        /// <summary>
        /// Gets or sets the hival.
        /// </summary>
        /// <value>The hival.</value>
        public int Hival
        {
            get { return  this.hival; }
            set { this.hival = value; }
        }

        /// <summary>
        /// Gets or sets the lookup.
        /// </summary>
        /// <value>The lookup.</value>
        public byte[] Lookup
        {
            get { return  this.lookup; }
            set { this.lookup = value; }
        }
    }
}

