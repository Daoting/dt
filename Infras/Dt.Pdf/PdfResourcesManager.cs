#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Drawing;
using Dt.Pdf.Object;
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Pdf
{
    /// <summary>
    /// The Pdf resources manager
    /// </summary>
    internal class PdfResourcesManager
    {
        private const string extGStatePrefix = "G";
        private readonly Dictionary<PdfExtGraphicState, KeyValuePair<PdfName, PdfExtGraphicState>> extGStates = new Dictionary<PdfExtGraphicState, KeyValuePair<PdfName, PdfExtGraphicState>>();
        private const string fontPrefix = "F";
        private readonly Dictionary<BaseFont, KeyValuePair<PdfName, PdfFont>> fonts = new Dictionary<BaseFont, KeyValuePair<PdfName, PdfFont>>();
        private const string imagePrefix = "Im";
        private readonly Dictionary<Image, KeyValuePair<PdfName, PdfImage>> images = new Dictionary<Image, KeyValuePair<PdfName, PdfImage>>();
        private const string patternPrefix = "P";
        private readonly Dictionary<PdfObjectBase, PdfName> patterns = new Dictionary<PdfObjectBase, PdfName>();
        private const string shadingPrefix = "S";
        private readonly Dictionary<PdfShading, PdfName> shadings = new Dictionary<PdfShading, PdfName>();

        /// <summary>
        /// Adds the Extend Graphics State.
        /// </summary>
        /// <param name="egs">The Extend Graphics State.</param>
        /// <returns></returns>
        public PdfName AddExtGState(ref PdfExtGraphicState egs)
        {
            if (this.extGStates.ContainsKey(egs))
            {
                KeyValuePair<PdfName, PdfExtGraphicState> pair = this.extGStates[egs];
                egs = pair.Value;
                return pair.Key;
            }
            PdfName key = new PdfName("G" + ((int) this.extGStates.Count), true);
            this.extGStates.Add(egs, new KeyValuePair<PdfName, PdfExtGraphicState>(key, egs));
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
            if (this.fonts.ContainsKey(font))
            {
                KeyValuePair<PdfName, PdfFont> pair = this.fonts[font];
                pdfFont = pair.Value;
                KeyValuePair<PdfName, PdfFont> pair2 = this.fonts[font];
                return pair2.Key;
            }
            PdfName key = new PdfName("F" + ((int) this.fonts.Count), true);
            pdfFont = PdfFont.Create(font);
            this.fonts.Add(font, new KeyValuePair<PdfName, PdfFont>(key, pdfFont));
            return key;
        }

        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="mask">The mask.</param>
        /// <param name="pdfImage">The PDF image.</param>
        /// <returns></returns>
        public PdfName AddImage(Image image, PdfImage mask, out PdfImage pdfImage)
        {
            if (this.images.ContainsKey(image))
            {
                KeyValuePair<PdfName, PdfImage> pair = this.images[image];
                pdfImage = pair.Value;
                KeyValuePair<PdfName, PdfImage> pair2 = this.images[image];
                return pair2.Key;
            }
            pdfImage = new PdfImage(image, mask);
            PdfName key = new PdfName("Im" + ((int) this.images.Count), true);
            this.images.Add(image, new KeyValuePair<PdfName, PdfImage>(key, pdfImage));
            return key;
        }

        /// <summary>
        /// Adds the pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public PdfName AddPattern(PdfObjectBase pattern)
        {
            if (this.patterns.ContainsKey(pattern))
            {
                return this.patterns[pattern];
            }
            PdfName name = new PdfName("P" + ((int) this.patterns.Count), true);
            this.patterns.Add(pattern, name);
            return name;
        }

        /// <summary>
        /// Adds the shading.
        /// </summary>
        /// <param name="shading">The shading.</param>
        /// <returns></returns>
        public PdfName AddShading(PdfShading shading)
        {
            if (this.shadings.ContainsKey(shading))
            {
                return this.shadings[shading];
            }
            PdfName name = new PdfName("S" + ((int) this.shadings.Count), true);
            this.shadings.Add(shading, name);
            return name;
        }
    }
}

