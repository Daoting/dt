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
using Dt.Pdf.Text;
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The Pdf resources
    /// </summary>
    public class PdfResources : PdfDictionary
    {
        private readonly PdfResourcesManager manager;
        private byte procSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfResources" /> class.
        /// </summary>
        public PdfResources()
        {
            this.manager = new PdfResourcesManager();
            this.procSet = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfResources" /> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        internal PdfResources(PdfResourcesManager manager)
        {
            this.manager = new PdfResourcesManager();
            this.procSet = 1;
            this.manager = manager;
        }

        /// <summary>
        /// Adds the Extend Graphics State.
        /// </summary>
        /// <param name="egs">The Extend Graphics State.</param>
        /// <returns></returns>
        public PdfName AddExtGState(PdfExtGraphicState egs)
        {
            if (egs == null)
            {
                throw new PdfArgumentNullException("egs");
            }
            egs.IsLabeled = true;
            PdfName key = this.manager.AddExtGState(ref egs);
            if (!this.ExtGStates.ContainsKey(key))
            {
                this.ExtGStates.Add(key, egs);
            }
            return key;
        }

        /// <summary>
        /// Adds the font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="pdfFont">The PDF font.</param>
        /// <returns></returns>
        public PdfName AddFont(BaseFont font, out PdfFont pdfFont)
        {
            if (font == null)
            {
                throw new PdfArgumentNullException("font");
            }
            PdfName key = this.manager.AddFont(font, out pdfFont);
            pdfFont.IsLabeled = true;
            if (!this.Fonts.ContainsKey(key))
            {
                this.Fonts.Add(key, pdfFont);
            }
            this.SetProcSet(OperatorCategory.Text);
            return key;
        }

        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public PdfName AddImage(Image image)
        {
            PdfImage image2;
            PdfName name;
            if (image == null)
            {
                throw new PdfArgumentNullException("image");
            }
            PdfImage pdfImage = null;
            if (image.ImageMask != null)
            {
                name = this.manager.AddImage(image.ImageMask, null, out pdfImage);
                if (!this.XObjects.ContainsKey(name))
                {
                    this.XObjects.Add(name, pdfImage);
                }
                this.SetProcSet(OperatorCategory.ImageB);
                if (CheckImageIorC(image.ImageMask))
                {
                    this.SetProcSet(OperatorCategory.ImageI);
                }
                else
                {
                    this.SetProcSet(OperatorCategory.ImageC);
                }
            }
            name = this.manager.AddImage(image, pdfImage, out image2);
            if (!this.XObjects.ContainsKey(name))
            {
                this.XObjects.Add(name, image2);
            }
            if (CheckImageIorC(image))
            {
                this.SetProcSet(OperatorCategory.ImageI);
                return name;
            }
            this.SetProcSet(OperatorCategory.ImageC);
            return name;
        }

        /// <summary>
        /// Adds the pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public PdfName AddPattern(PdfObjectBase pattern)
        {
            if (pattern == null)
            {
                throw new PdfArgumentNullException("pattern");
            }
            if (pattern is PdfShadingPattern)
            {
                this.AddShading(((PdfShadingPattern) pattern).Shading);
            }
            PdfName key = this.manager.AddPattern(pattern);
            if (!this.Patterns.ContainsKey(key))
            {
                this.Patterns.Add(key, pattern);
            }
            return key;
        }

        /// <summary>
        /// Adds the shading.
        /// </summary>
        /// <param name="shading">The shading.</param>
        /// <returns></returns>
        public PdfName AddShading(PdfShading shading)
        {
            if (shading == null)
            {
                throw new PdfArgumentNullException("shading");
            }
            shading.IsLabeled = true;
            PdfName key = this.manager.AddShading(shading);
            if (!this.Shadings.ContainsKey(key))
            {
                this.Shadings.Add(key, shading);
            }
            return key;
        }

        /// <summary>
        /// Checks the image ior C.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        internal static bool CheckImageIorC(Image image)
        {
            if ((image.Properties != null) && image.Properties.ContainsKey(PdfName.ColorSpace))
            {
                PdfIndexedColorSpace space = image.Properties[PdfName.ColorSpace] as PdfIndexedColorSpace;
                return (space != null);
            }
            return false;
        }

        /// <summary>
        /// Sets the proc set.
        /// </summary>
        /// <param name="oc">The oc.</param>
        public void SetProcSet(OperatorCategory oc)
        {
            this.procSet = (byte) (this.procSet | (byte)oc);
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.procSet > 0)
            {
                PdfArray procSet = this.ProcSet;
                if ((this.procSet & 1) == 1)
                {
                    procSet.Add(PdfName.PDF);
                }
                if ((this.procSet & 2) == 2)
                {
                    procSet.Add(PdfName.Text);
                }
                if ((this.procSet & 4) == 4)
                {
                    procSet.Add(PdfName.ImageB);
                }
                if ((this.procSet & 8) == 8)
                {
                    procSet.Add(PdfName.ImageC);
                }
                if ((this.procSet & 0x10) == 0x10)
                {
                    procSet.Add(PdfName.ImageI);
                }
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the color spaces.
        /// </summary>
        /// <value>The color spaces.</value>
        public PdfDictionary ColorSpaces
        {
            get { return  base.GetOrLazyCreate<PdfDictionary>(PdfName.ColorSpace); }
        }

        /// <summary>
        /// Gets the extend Graphic states.
        /// </summary>
        /// <value>The extend Graphic states.</value>
        public PdfDictionary ExtGStates
        {
            get { return  base.GetOrLazyCreate<PdfDictionary>(PdfName.ExtGState); }
        }

        /// <summary>
        /// Gets the fonts.
        /// </summary>
        /// <value>The fonts.</value>
        public PdfDictionary Fonts
        {
            get { return  base.GetOrLazyCreate<PdfDictionary>(PdfName.Font); }
        }

        /// <summary>
        /// Gets the patterns.
        /// </summary>
        /// <value>The patterns.</value>
        public PdfDictionary Patterns
        {
            get { return  base.GetOrLazyCreate<PdfDictionary>(PdfName.Pattern); }
        }

        /// <summary>
        /// Gets the proc set.
        /// </summary>
        /// <value>The proc set.</value>
        public PdfArray ProcSet
        {
            get { return  base.GetOrLazyCreate<PdfArray>(PdfName.ProcSet); }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public PdfDictionary Properties
        {
            get { return  base.GetOrLazyCreate<PdfDictionary>(PdfName.Properties); }
        }

        /// <summary>
        /// Gets the shadings.
        /// </summary>
        /// <value>The shadings.</value>
        public PdfDictionary Shadings
        {
            get { return  base.GetOrLazyCreate<PdfDictionary>(PdfName.Shading); }
        }

        /// <summary>
        /// Gets the X objects.
        /// </summary>
        /// <value>The X objects.</value>
        public PdfDictionary XObjects
        {
            get { return  base.GetOrLazyCreate<PdfDictionary>(PdfName.XObject); }
        }

        /// <summary>
        /// PreSet of Pdf
        /// </summary>
        [Flags]
        public enum OperatorCategory : byte
        {
            /// <summary>
            /// Contains Image B
            /// </summary>
            ImageB = 4,
            /// <summary>
            /// Contains Image C
            /// </summary>
            ImageC = 8,
            /// <summary>
            /// Contains Image I
            /// </summary>
            ImageI = 0x10,
            /// <summary>
            /// Contains Pdf
            /// </summary>
            PDF = 1,
            /// <summary>
            /// Contains Text
            /// </summary>
            Text = 2
        }
    }
}

