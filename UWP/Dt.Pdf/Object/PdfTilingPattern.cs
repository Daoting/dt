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
    /// The tiling pattern object
    /// </summary>
    public class PdfTilingPattern : PdfStream, IGraphicsStream, IVersionDepend
    {
        private PdfRectangle bBox;
        private PdfGraphics graphics;
        /// <summary>
        /// Warning: can be null
        /// </summary>
        private readonly PdfDocument pdf;
        private readonly bool stencil;
        private TilingType tilingType;
        private float xStep;
        private float yStep;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingPattern" /> class.
        /// </summary>
        public PdfTilingPattern() : this(null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingPattern" /> class.
        /// </summary>
        /// <param name="pdf">The PDF.</param>
        public PdfTilingPattern(PdfDocument pdf) : this(pdf, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingPattern" /> class.
        /// </summary>
        /// <param name="stencil">if set to <c>true</c> [stencil].</param>
        public PdfTilingPattern(bool stencil) : this(null, stencil)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTilingPattern" /> class.
        /// </summary>
        /// <param name="pdf">The PDF.</param>
        /// <param name="stencil">if set to <c>true</c> [stencil].</param>
        public PdfTilingPattern(PdfDocument pdf, bool stencil)
        {
            this.bBox = new PdfRectangle(0f, 0f, 50f, 50f);
            this.tilingType = TilingType.ConstantSpacing;
            this.xStep = 50f;
            this.yStep = 50f;
            this.pdf = pdf;
            base.Properties.Add(PdfName.Type, PdfName.Pattern);
            base.Properties.Add(PdfName.PatternType, PdfNumber.One);
            base.Properties[PdfName.PaintType] = stencil ? PdfNumber.Two : PdfNumber.One;
            this.stencil = stencil;
            PdfResources resources = (this.pdf != null) ? new PdfResources(this.pdf.ResourcesManager) : new PdfResources();
            resources.IsLabeled = true;
            base.Properties.Add(PdfName.Resources, resources);
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns></returns>
        public PdfResources GetResources()
        {
            return (base.Properties[PdfName.Resources] as PdfResources);
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base.Properties[PdfName.TilingType] = new PdfNumber((double) this.tilingType);
            base.Properties[PdfName.BBox] = this.bBox;
            base.Properties[PdfName.XStep] = new PdfNumber((double) this.xStep);
            base.Properties[PdfName.YStep] = new PdfNumber((double) this.yStep);
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
        /// Gets or sets the bound box.
        /// </summary>
        /// <value>The bound box.</value>
        public PdfRectangle BBox
        {
            get { return  this.bBox; }
            set { this.bBox = value; }
        }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public PdfGraphics Graphics
        {
            get
            {
                if (this.graphics == null)
                {
                    this.graphics = new PdfGraphics(base.Psw, this, this.stencil);
                }
                return this.graphics;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is stencil.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is stencil; otherwise, <c>false</c>.
        /// </value>
        public bool IsStencil
        {
            get { return  this.stencil; }
        }

        /// <summary>
        /// Gets or sets the type of the tiling_.
        /// </summary>
        /// <value>The type of the tiling_.</value>
        public TilingType Tiling_Type
        {
            get { return  this.tilingType; }
            set { this.tilingType = value; }
        }

        /// <summary>
        /// Gets or sets the X step.
        /// </summary>
        /// <value>The X step.</value>
        public float XStep
        {
            get { return  this.xStep; }
            set { this.xStep = value; }
        }

        /// <summary>
        /// Gets or sets the Y step.
        /// </summary>
        /// <value>The Y step.</value>
        public float YStep
        {
            get { return  this.yStep; }
            set { this.yStep = value; }
        }

        /// <summary>
        /// Tiling type
        /// </summary>
        public enum TilingType
        {
            /// <summary>
            /// Constant spacing
            /// </summary>
            ConstantSpacing = 1,
            /// <summary>
            /// Constant spacing and faster tiling
            /// </summary>
            ConstantSpacingAndFasterTiling = 3,
            /// <summary>
            /// No distortion
            /// </summary>
            NoDistortion = 2
        }
    }
}

