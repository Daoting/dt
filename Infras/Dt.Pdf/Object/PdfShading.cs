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
using System.Collections.Generic;
using System.Text;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The abstract class of shading of Pdf
    /// </summary>
    public abstract class PdfShading : PdfDictionary, IVersionDepend
    {
        private bool antiAlias;
        private PdfRectangle bBox;
        private const string calcTemplate = "{0} mul {1} add";
        private readonly PdfName colorSpace = PdfName.DeviceRGB;
        private const string condition2Template = "ifelse";
        private const string conditonTemplate = "dup {0} le";
        private const string dup = "dup";
        private const string exch = "exch";
        private const string leftBrace = "{";
        private const string lineBreak = "\r\n";
        private const string pop = "pop";
        private const string rightBrace = "}";
        private readonly ShadingType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfShading" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        protected PdfShading(ShadingType type)
        {
            this.type = type;
        }

        protected static void CreateCalcItemPostScriptCode(float start, float end, float min, float max, StringBuilder builder)
        {
            builder.Append("dup").Append("\r\n");
            if (max == min)
            {
                max += 0.01f;
            }
            float num = (end - start) / (max - min);
            float num2 = start - (min * num);
            builder.AppendFormat("{0} mul {1} add", new object[] { (float) num, (float) num2 }).Append("\r\n");
            builder.Append("exch").Append("\r\n");
        }

        protected static void CreateCalcPartPostScriptCode(PdfColor color0, PdfColor color1, float offset0, float offset1, StringBuilder builder)
        {
            CreateCalcItemPostScriptCode(color0.RedF, color1.RedF, offset0, offset1, builder);
            CreateCalcItemPostScriptCode(color0.GreenF, color1.GreenF, offset0, offset1, builder);
            CreateCalcItemPostScriptCode(color0.BlueF, color1.BlueF, offset0, offset1, builder);
            builder.Append("pop").Append("\r\n");
        }

        protected static void CreateCalcPostScriptCode(List<PdfColor> colors, List<float> offsets, StringBuilder builder)
        {
            if (colors.Count == 2)
            {
                CreateCalcPartPostScriptCode(colors[0], colors[1], offsets[0], offsets[1], builder);
            }
            else
            {
                builder.AppendFormat("dup {0} le", new object[] { (float) offsets[1] }).Append("\r\n");
                builder.Append("{").Append("\r\n");
                CreateCalcPartPostScriptCode(colors[0], colors[1], offsets[0], offsets[1], builder);
                builder.Append("}").Append("\r\n");
                builder.Append("{").Append("\r\n");
                colors.RemoveAt(0);
                offsets.RemoveAt(0);
                CreateCalcPostScriptCode(colors, offsets, builder);
                builder.Append("}").Append("\r\n");
                builder.Append("ifelse").Append("\r\n");
            }
        }

        protected static string GetPostScriptCode(List<PdfColor> colorsArgu, List<float> offsetsArgu)
        {
            List<PdfColor> colors = new List<PdfColor>(colorsArgu);
            List<float> offsets = new List<float>(offsetsArgu);
            if (offsets[0] != 0f)
            {
                colors.Insert(0, colors[0]);
                offsets.Insert(0, 0f);
            }
            if (offsets[offsets.Count - 1] != 1f)
            {
                colors.Add(colors[colors.Count - 1]);
                offsets.Add(1f);
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("{").Append("\r\n");
            CreateCalcPostScriptCode(colors, offsets, builder);
            builder.Append("}").Append("\r\n");
            return builder.ToString();
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.type > ShadingType.Radial)
            {
                throw new NotSupportedException();
            }
            base[PdfName.ShadingType] = new PdfNumber((double) this.type);
            base[PdfName.ColorSpace] = this.colorSpace;
            if (this.bBox != null)
            {
                base[PdfName.BBox] = this.bBox;
            }
            if (this.antiAlias)
            {
                base[PdfName.AntiAlias] = PdfBool.TRUE;
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            return PdfVersion.PDF1_3;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [anti alias].
        /// </summary>
        /// <value><c>true</c> if [anti alias]; otherwise, <c>false</c>.</value>
        public bool AntiAlias
        {
            get { return  this.antiAlias; }
            set { this.antiAlias = value; }
        }

        /// <summary>
        /// Gets or sets the B box.
        /// </summary>
        /// <value>The B box.</value>
        public PdfRectangle BBox
        {
            get { return  this.bBox; }
            set { this.bBox = value; }
        }

        /// <summary>
        /// Gets the color space.
        /// </summary>
        /// <value>The color space.</value>
        public PdfName ColorSpace
        {
            get { return  this.colorSpace; }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public ShadingType Type
        {
            get { return  this.type; }
        }

        /// <summary>
        /// Shading Types
        /// </summary>
        public enum ShadingType
        {
            /// <summary>
            /// Axial shading
            /// </summary>
            Axial = 2,
            /// <summary>
            /// Conns patch mesh shading
            /// </summary>
            ConnsPatchMesh = 6,
            /// <summary>
            /// Free form gouraud shaded trangle mesh shading
            /// </summary>
            FreeFormGouraudShadedTriangleMesh = 4,
            /// <summary>
            /// Functaion based shading
            /// </summary>
            FunctionBased = 1,
            /// <summary>
            /// lattice form gouraud shaded trangle mesh shading
            /// </summary>
            LatticeFormGouraudShadedTriangleMesh = 5,
            /// <summary>
            /// Radial shading
            /// </summary>
            Radial = 3,
            /// <summary>
            /// tensor product patch mesh shading
            /// </summary>
            TensorProductPatchMesh = 7
        }
    }
}

