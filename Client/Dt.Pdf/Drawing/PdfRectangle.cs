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

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Rectangle object for Pdf
    /// </summary>
    public class PdfRectangle : PdfArray
    {
        private float ll_x;
        private float ll_y;
        private float ur_x;
        private float ur_y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfRectangle" /> class.
        /// </summary>
        public PdfRectangle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfRectangle" /> class.
        /// </summary>
        /// <param name="ll_x">The LL_X.</param>
        /// <param name="ll_y">The ll_y.</param>
        /// <param name="ur_x">The ur_x.</param>
        /// <param name="ur_y">The ur_y.</param>
        public PdfRectangle(float ll_x, float ll_y, float ur_x, float ur_y)
        {
            this.ll_x = ll_x;
            this.ll_y = ll_y;
            this.ur_x = ur_x;
            this.ur_y = ur_y;
        }

        /// <summary>
        /// Writes the content.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteContent(PdfWriter writer)
        {
            PdfStreamWriter psw = writer.Psw;
            psw.WriteDouble((double) this.ll_x).WriteSpace();
            psw.WriteDouble((double) this.ll_y).WriteSpace();
            psw.WriteDouble((double) this.ur_x).WriteSpace();
            psw.WriteDouble((double) this.ur_y);
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get { return  Math.Abs((float) (this.UpperRightY - this.LowerLeftY)); }
        }

        /// <summary>
        /// Gets or sets the lower left X.
        /// </summary>
        /// <value>The lower left X.</value>
        public float LowerLeftX
        {
            get { return  this.ll_x; }
            set { this.ll_x = value; }
        }

        /// <summary>
        /// Gets or sets the lower left Y.
        /// </summary>
        /// <value>The lower left Y.</value>
        public float LowerLeftY
        {
            get { return  this.ll_y; }
            set { this.ll_y = value; }
        }

        /// <summary>
        /// Gets or sets the upper right X.
        /// </summary>
        /// <value>The upper right X.</value>
        public float UpperRightX
        {
            get { return  this.ur_x; }
            set { this.ur_x = value; }
        }

        /// <summary>
        /// Gets or sets the upper right Y.
        /// </summary>
        /// <value>The upper right Y.</value>
        public float UpperRightY
        {
            get { return  this.ur_y; }
            set { this.ur_y = value; }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get { return  Math.Abs((float) (this.UpperRightX - this.LowerLeftX)); }
        }
    }
}

