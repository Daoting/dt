#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using System;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Raw Image Object
    /// </summary>
    internal class RawImage : Image
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RawImage" /> class.
        /// </summary>
        /// <param name="image">The image.</param>
        public RawImage(Image image) : base(image)
        {
        }

        /// <summary>
        /// Creats an Image in raw mode.
        /// </summary>
        /// <param name="width">the exact width of the image</param>
        /// <param name="height">the exact height of the image</param>
        /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
        /// <param name="bpc">bits per component. Must be 1,2,4 or 8</param>
        /// <param name="data">data the image data</param>
        public RawImage(int width, int height, int components, int bpc, byte[] data)
        {
            if (((components != 1) && (components != 3)) && (components != 4))
            {
                throw new PdfArgumentException("Components must be 1, 3, or 4.");
            }
            if (((bpc != 1) && (bpc != 2)) && ((bpc != 4) && (bpc != 8)))
            {
                throw new PdfArgumentException("Bits-per-component must be 1, 2, 4, or 8.");
            }
            base.width = width;
            base.height = height;
            base.colorspace = components;
            base.bpc = bpc;
            base.rawData = data;
        }
    }
}

