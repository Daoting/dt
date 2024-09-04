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
    /// The Fixed Print
    /// </summary>
    public class PdfFixedPrint : PdfDictionary
    {
        private double h;
        private PdfMatrix matrix = new PdfMatrix();
        private double v;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFixedPrint" /> class.
        /// </summary>
        public PdfFixedPrint()
        {
            base.Add(PdfName.Type, PdfName.FixedPrint);
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (!this.matrix.IsDefault)
            {
                base[PdfName.Matrix] = this.matrix;
            }
            if (this.h != 0.0)
            {
                base[PdfName.H] = new PdfNumber(this.h);
            }
            if (this.v != 0.0)
            {
                base[PdfName.V] = new PdfNumber(this.v);
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets the H.
        /// </summary>
        /// <value>The H.</value>
        public double H
        {
            get { return  this.h; }
            set { this.h = value; }
        }

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <value>The matrix.</value>
        public PdfMatrix Matrix
        {
            get { return  this.matrix; }
        }

        /// <summary>
        /// Gets or sets the V.
        /// </summary>
        /// <value>The V.</value>
        public double V
        {
            get { return  this.v; }
            set { this.v = value; }
        }
    }
}

