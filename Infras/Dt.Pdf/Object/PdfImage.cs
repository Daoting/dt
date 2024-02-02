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
using Dt.Pdf.Exceptions;
using Dt.Pdf.Object.Filter;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The Image of Pdf object
    /// </summary>
    public class PdfImage : PdfStream, IVersionDepend
    {
        private readonly PdfVersion version = PdfVersion.PDF1_0;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfImage" /> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="maskImage">The mask image.</param>
        public PdfImage(Image image, PdfImage maskImage)
        {
            if (image == null)
            {
                throw new PdfArgumentNullException("image");
            }
            base.Properties.Add(PdfName.Type, PdfName.XObject);
            base.Properties.Add(PdfName.Subtype, PdfName.Image);
            base.Properties.Add(PdfName.Width, new PdfNumber((double) image.Width));
            base.Properties.Add(PdfName.Height, new PdfNumber((double) image.Height));
            if (image.Mask && ((image.Bpc == 1) || (image.Bpc > 0xff)))
            {
                base.Properties.Add(PdfName.ImageMask, PdfBool.TRUE);
            }
            if (maskImage != null)
            {
                if (image.Smask)
                {
                    base.Properties.Add(PdfName.SMask, maskImage);
                    this.version = PdfVersion.PDF1_4;
                }
                else
                {
                    base.Properties.Add(PdfName.Mask, maskImage);
                    this.version = PdfVersion.PDF1_3;
                }
            }
            if (image is RawImage)
            {
                base.Data.Write(image.RawData, 0, image.RawData.Length);
                int[] transparency = image.Transparency;
                if (transparency != null)
                {
                    PdfArray array = PdfArray.Convert(transparency);
                    base.Properties.Add(PdfName.Mask, array);
                }
                if (image.Bpc <= 0xff)
                {
                    switch (image.Colorspace)
                    {
                        case 1:
                            base.Properties.Add(PdfName.ColorSpace, PdfName.DeviceGray);
                            break;

                        case 3:
                            base.Properties.Add(PdfName.ColorSpace, PdfName.DeviceRGB);
                            break;

                        default:
                            base.Properties.Add(PdfName.ColorSpace, PdfName.DeviceCMYK);
                            break;
                    }
                    if (image.Properties != null)
                    {
                        foreach (KeyValuePair<PdfName, PdfObjectBase> pair in image.Properties)
                        {
                            base.Properties[pair.Key] = pair.Value;
                        }
                    }
                    base.Properties.Add(PdfName.BitsPerComponent, new PdfNumber((double) image.Bpc));
                    if (image.Deflated)
                    {
                        base.Filters.Enqueue(MockFilter.FlateFilter);
                        return;
                    }
                    bool flag = false;
                    foreach (PdfFilter filter in base.Filters)
                    {
                        if (filter is FlateFilter)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        base.Filters.Enqueue(PdfFilter.FlateFilter);
                    }
                }
                return;
            }
            else
            {
                ImageType type = image.Type;
                if ((type != ImageType.JPEG) && (type != ImageType.JPEG2000))
                {
                    throw new PdfUnrecognizedImageFormatException("Not a recognized Image Format.");
                }
                base.Filters.Enqueue((image.Type == ImageType.JPEG) ? MockFilter.DCTFilter : MockFilter.JPXFilter);
                if (image.Type == ImageType.JPEG2000)
                {
                    this.version = PdfVersion.PDF1_5;
                }
                if (image.Colorspace <= 0)
                {
                    goto Label_034F;
                }
                switch (image.Colorspace)
                {
                    case 1:
                        base.Properties.Add(PdfName.ColorSpace, PdfName.DeviceGray);
                        goto Label_0333;

                    case 3:
                        base.Properties.Add(PdfName.ColorSpace, PdfName.DeviceRGB);
                        goto Label_0333;
                }
                base.Properties.Add(PdfName.ColorSpace, PdfName.DeviceCMYK);
            }
        Label_0333:
            base.Properties.Add(PdfName.BitsPerComponent, new PdfNumber((double) image.Bpc));
        Label_034F:
            if (image.RawData != null)
            {
                base.Data.Write(image.RawData, 0, image.RawData.Length);
            }
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            return this.version;
        }
    }
}

