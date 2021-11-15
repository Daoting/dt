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

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The function based shading, the color at every point in the domain is defined by a specified mathematical function.
    /// </summary>
    public class PdfFunctionBasedShading : PdfShading
    {
        private float[] domain;
        private readonly PdfObjectBase function;
        private float[] matrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFunctionBasedShading" /> class.
        /// </summary>
        /// <param name="func">The func.</param>
        public PdfFunctionBasedShading(PdfObjectBase func) : base(PdfShading.ShadingType.FunctionBased)
        {
            if (func == null)
            {
                throw new PdfArgumentNullException("func");
            }
            this.function = func;
        }

        /// <summary>
        /// Toes the PDF.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Function] = this.function;
            if (this.domain != null)
            {
                base[PdfName.Domain] = PdfArray.Convert(this.domain);
            }
            if (this.matrix != null)
            {
                base[PdfName.Matrix] = PdfArray.Convert(this.matrix);
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// An array of four numbers [ xmin xmax ymin ymax ] specifying the rectangular 
        /// domain of coordinates over which the color function(s) are defined.
        /// </summary>
        /// <value>The domain.</value>
        public float[] Domain
        {
            get { return  this.domain; }
            set { this.domain = value; }
        }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>The function.</value>
        public PdfObjectBase Function
        {
            get { return  this.function; }
        }

        /// <summary>
        /// An array of six numbers specifying a transformation matrix mapping the 
        /// coordinate space specified by the Domain entry into the shading¡¯s target coordinate space.
        /// </summary>
        /// <value>The matrix.</value>
        public float[] Matrix
        {
            get { return  this.matrix; }
            set { this.matrix = value; }
        }
    }
}

