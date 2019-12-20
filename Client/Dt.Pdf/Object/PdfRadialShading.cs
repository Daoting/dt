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
    /// The radial shading of Pdf
    /// </summary>
    public class PdfRadialShading : PdfShading
    {
        private readonly float[] coords;
        private float[] domain;
        private bool extendEnd;
        private bool extendStart;
        private readonly PdfObjectBase function;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfRadialShading" /> class.
        /// </summary>
        /// <param name="coords">The coords.</param>
        /// <param name="func">The func.</param>
        public PdfRadialShading(float[] coords, PdfObjectBase func) : base(PdfShading.ShadingType.Radial)
        {
            if (coords == null)
            {
                throw new PdfArgumentNullException("coords");
            }
            if (func == null)
            {
                throw new PdfArgumentNullException("func");
            }
            this.coords = coords;
            this.function = func;
        }

        /// <summary>
        /// Creates the PdfRadialShading
        /// </summary>
        /// <param name="x0">The x0.</param>
        /// <param name="y0">The y0.</param>
        /// <param name="r0">The r0.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="r1">The r1.</param>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <returns></returns>
        public static PdfRadialShading Create(float x0, float y0, float r0, float x1, float y1, float r1, PdfColor startColor, PdfColor endColor)
        {
            return Create(x0, y0, r0, x1, y1, r1, startColor, endColor, true, true);
        }

        /// <summary>
        /// Creates the specified x0.
        /// </summary>
        /// <param name="x0">The x0.</param>
        /// <param name="y0">The y0.</param>
        /// <param name="r0">The r0.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="r1">The r1.</param>
        /// <param name="colors">The colors.</param>
        /// <param name="offsets">The offsets.</param>
        /// <returns></returns>
        public static PdfRadialShading Create(float x0, float y0, float r0, float x1, float y1, float r1, List<PdfColor> colors, List<float> offsets)
        {
            return Create(x0, y0, r0, x1, y1, r1, colors, offsets, true, true);
        }

        /// <summary>
        /// Creates the PdfRadialShading
        /// </summary>
        /// <param name="x0">The x0.</param>
        /// <param name="y0">The y0.</param>
        /// <param name="r0">The r0.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="r1">The r1.</param>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <param name="extendStart">if set to <c>true</c> [extend start].</param>
        /// <param name="extendEnd">if set to <c>true</c> [extend end].</param>
        /// <returns></returns>
        public static PdfRadialShading Create(float x0, float y0, float r0, float x1, float y1, float r1, PdfColor startColor, PdfColor endColor, bool extendStart, bool extendEnd)
        {
            if (startColor == null)
            {
                throw new PdfArgumentNullException("startColor");
            }
            if (endColor == null)
            {
                throw new PdfArgumentNullException("endColor");
            }
            float[] domain = new float[2];
            domain[1] = 1f;
            return new PdfRadialShading(new float[] { x0, y0, r0, x1, y1, r1 }, PdfFunction.Type2(domain, null, 1f, startColor.ToArray(), endColor.ToArray())) { ExtendStart = extendStart, ExtendEnd = extendEnd };
        }

        /// <summary>
        /// Creates the specified x0.
        /// </summary>
        /// <param name="x0">The x0.</param>
        /// <param name="y0">The y0.</param>
        /// <param name="r0">The r0.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="r1">The r1.</param>
        /// <param name="colors">The colors.</param>
        /// <param name="offsets">The offsets.</param>
        /// <param name="extendStart">if set to <c>true</c> [extend start].</param>
        /// <param name="extendEnd">if set to <c>true</c> [extend end].</param>
        /// <returns></returns>
        public static PdfRadialShading Create(float x0, float y0, float r0, float x1, float y1, float r1, List<PdfColor> colors, List<float> offsets, bool extendStart, bool extendEnd)
        {
            if ((colors == null) || (colors.Count <= 0))
            {
                throw new PdfArgumentNullException("colors");
            }
            if ((offsets == null) || (offsets.Count <= 0))
            {
                throw new PdfArgumentNullException("offsets");
            }
            float[] domain = new float[2];
            domain[1] = 1f;
            return new PdfRadialShading(new float[] { x0, y0, r0, x1, y1, r1 }, PdfFunction.Type4(domain, new float[] { 0f, 1f, 0f, 1f, 0f, 1f }, PdfShading.GetPostScriptCode(colors, offsets))) { ExtendStart = extendStart, ExtendEnd = extendEnd };
        }

        /// <summary>
        /// Toes the PDF.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Coords] = PdfArray.Convert(this.coords);
            base[PdfName.Function] = this.function;
            if (this.domain != null)
            {
                base[PdfName.Domain] = PdfArray.Convert(this.domain);
            }
            if (this.extendStart || this.extendEnd)
            {
                PdfArray array = new PdfArray {
                    this.extendStart ? PdfBool.TRUE : PdfBool.FALSE,
                    this.extendEnd ? PdfBool.TRUE : PdfBool.FALSE
                };
                base[PdfName.Extend] = array;
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the coords.
        /// </summary>
        /// <value>The coords.</value>
        public float[] Coords
        {
            get { return  this.coords; }
        }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public float[] Domain
        {
            get { return  this.domain; }
            set { this.domain = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [extend end].
        /// </summary>
        /// <value><c>true</c> if [extend end]; otherwise, <c>false</c>.</value>
        public bool ExtendEnd
        {
            get { return  this.extendEnd; }
            set { this.extendEnd = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [extend start].
        /// </summary>
        /// <value><c>true</c> if [extend start]; otherwise, <c>false</c>.</value>
        public bool ExtendStart
        {
            get { return  this.extendStart; }
            set { this.extendStart = value; }
        }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>The function.</value>
        public PdfObjectBase Function
        {
            get { return  this.function; }
        }
    }
}

