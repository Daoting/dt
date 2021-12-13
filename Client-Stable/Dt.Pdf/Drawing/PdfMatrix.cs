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
using Dt.Pdf.Exceptions;
using System;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Matrix
    /// </summary>
    public class PdfMatrix : PdfArray
    {
        private double a;
        private double b;
        private double c;
        private double d;
        private double x;
        private double y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfMatrix" /> class.
        /// </summary>
        public PdfMatrix()
        {
            this.a = 1.0;
            this.d = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfMatrix" /> class.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <param name="e">The x.</param>
        /// <param name="f">The f.</param>
        public PdfMatrix(double a, double b, double c, double d, double e, double f)
        {
            this.a = 1.0;
            this.d = 1.0;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.x = e;
            this.y = f;
        }

        public void Apply(float aa, float bb, float cc, float dd, float xx, float yy)
        {
            this.a *= this.a;
            this.b *= this.b;
            this.c *= this.c;
            this.d *= this.d;
            this.x += this.x;
            this.y += this.y;
        }

        public void CopyFrom(PdfMatrix matrix)
        {
            if (matrix == null)
            {
                throw new PdfArgumentNullException("matrix");
            }
            this.a = matrix.a;
            this.b = matrix.b;
            this.c = matrix.c;
            this.d = matrix.d;
            this.x = matrix.x;
            this.y = matrix.y;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base.Clear();
            base.Add(new PdfNumber(this.a));
            base.Add(new PdfNumber(this.b));
            base.Add(new PdfNumber(this.c));
            base.Add(new PdfNumber(this.d));
            base.Add(new PdfNumber(this.x));
            base.Add(new PdfNumber(this.y));
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets the A.
        /// </summary>
        /// <value>The A.</value>
        public double A
        {
            get { return  this.a; }
            set { this.a = value; }
        }

        /// <summary>
        /// Gets or sets the B.
        /// </summary>
        /// <value>The B.</value>
        public double B
        {
            get { return  this.b; }
            set { this.b = value; }
        }

        /// <summary>
        /// Gets or sets the C.
        /// </summary>
        /// <value>The C.</value>
        public double C
        {
            get { return  this.c; }
            set { this.c = value; }
        }

        /// <summary>
        /// Gets or sets the D.
        /// </summary>
        /// <value>The D.</value>
        public double D
        {
            get { return  this.d; }
            set { this.d = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
        {
            get { return  (((((this.a == 1.0) && (this.b == 0.0)) && ((this.c == 0.0) && (this.d == 1.0))) && (this.x == 0.0)) && (this.y == 0.0)); }
        }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public double X
        {
            get { return  this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Gets or sets the F.
        /// </summary>
        /// <value>The F.</value>
        public double Y
        {
            get { return  this.y; }
            set { this.y = value; }
        }
    }
}

