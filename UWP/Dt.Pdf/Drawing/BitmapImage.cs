#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Object;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Bitmap Image
    /// </summary>
    internal class BitmapImage
    {
        private int alphaMask;
        private const int BI_BITFIELDS = 3;
        private const int BI_RGB = 0;
        private const int BI_RLE4 = 2;
        private const int BI_RLE8 = 1;
        private long bitmapFileSize;
        private long bitmapOffset;
        private int bitsPerPixel;
        private int blueMask;
        private long compression;
        private int greenMask;
        private int height;
        private long imageSize;
        private int imageType;
        private Stream inputStream;
        private bool isBottomUp;
        private const int LCS_CALIBRATED_RGB = 0;
        private const int LCS_CMYK = 2;
        private const int LCS_sRGB = 1;
        private byte[] palette;
        public Dictionary<string, object> properties = new Dictionary<string, object>();
        private int redMask;
        private const int VERSION_2_1_BIT = 0;
        private const int VERSION_2_24_BIT = 3;
        private const int VERSION_2_4_BIT = 1;
        private const int VERSION_2_8_BIT = 2;
        private const int VERSION_3_1_BIT = 4;
        private const int VERSION_3_24_BIT = 7;
        private const int VERSION_3_4_BIT = 5;
        private const int VERSION_3_8_BIT = 6;
        private const int VERSION_3_NT_16_BIT = 8;
        private const int VERSION_3_NT_32_BIT = 9;
        private const int VERSION_4_1_BIT = 10;
        private const int VERSION_4_16_BIT = 13;
        private const int VERSION_4_24_BIT = 14;
        private const int VERSION_4_32_BIT = 15;
        private const int VERSION_4_4_BIT = 11;
        private const int VERSION_4_8_BIT = 12;
        private int width;
        private long xPelsPerMeter;
        private long yPelsPerMeter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BitmapImage" /> class.
        /// </summary>
        /// <param name="isp">The isp.</param>
        /// <param name="noHeader">if set to <c>true</c> [no header].</param>
        /// <param name="size">The size.</param>
        internal BitmapImage(Stream isp, bool noHeader, int size)
        {
            this.bitmapFileSize = size;
            this.bitmapOffset = 0L;
            this.LoadImage(isp, noHeader);
        }

        /// <summary>
        /// Checks the type.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <param name="c3">The c3.</param>
        /// <param name="c4">The c4.</param>
        /// <returns></returns>
        public static bool CheckType(byte c1, byte c2, byte c3, byte c4)
        {
            return ((c1 == 0x42) && (c2 == 0x4d));
        }

        /// <summary>
        /// Decodes the RLE.
        /// </summary>
        /// <param name="is8">if set to <c>true</c> [is8].</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private byte[] DecodeRLE(bool is8, byte[] values)
        {
            byte[] buffer = new byte[this.width * this.height];
            try
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                while ((num4 < this.height) && (num < values.Length))
                {
                    int num5 = values[num++] & 0xff;
                    if (num5 != 0)
                    {
                        int num6 = values[num++] & 0xff;
                        if (is8)
                        {
                            for (int i = num5; i != 0; i--)
                            {
                                buffer[num3++] = (byte) num6;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < num5; j++)
                            {
                                buffer[num3++] = ((j & 1) == 1) ? ((byte) (num6 & 15)) : ((byte) ((num6 >> 4) & 15));
                            }
                        }
                        num2 += num5;
                        continue;
                    }
                    num5 = values[num++] & 0xff;
                    switch (num5)
                    {
                        case 0:
                        {
                            num2 = 0;
                            num4++;
                            num3 = num4 * this.width;
                            continue;
                        }
                        case 2:
                        {
                            num2 += values[num++] & 0xff;
                            num4 += values[num++] & 0xff;
                            num3 = (num4 * this.width) + num2;
                            continue;
                        }
                        case 1:
                            return buffer;
                    }
                    if (is8)
                    {
                        for (int k = num5; k != 0; k--)
                        {
                            buffer[num3++] = (byte)(values[num++] & 0xff);
                        }
                    }
                    else
                    {
                        int num10 = 0;
                        for (int m = 0; m < num5; m++)
                        {
                            if ((m & 1) == 0)
                            {
                                num10 = values[num++] & 0xff;
                            }
                            buffer[num3++] = ((m & 1) == 1) ? ((byte) (num10 & 15)) : ((byte) ((num10 >> 4) & 15));
                        }
                    }
                    num2 += num5;
                    if (is8)
                    {
                        if ((num5 & 1) == 1)
                        {
                            num++;
                        }
                    }
                    else if (((num5 & 3) == 1) || ((num5 & 3) == 2))
                    {
                        num++;
                    }
                }
            }
            catch
            {
            }
            return buffer;
        }

        /// <summary>
        /// Finds the mask.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns></returns>
        private static int FindMask(int mask)
        {
            for (int i = 0; i < 0x20; i++)
            {
                if ((mask & 1) == 1)
                {
                    return mask;
                }
                mask = Image.USR(mask, 1);
            }
            return mask;
        }

        /// <summary>
        /// Finds the shift.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns></returns>
        private static int FindShift(int mask)
        {
            int num = 0;
            while (num < 0x20)
            {
                if ((mask & 1) == 1)
                {
                    return num;
                }
                mask = Image.USR(mask, 1);
                num++;
            }
            return num;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns></returns>
        private Image GetImage()
        {
            byte[] buffer;
            switch (this.imageType)
            {
                case 0:
                    return this.Read1Bit(3);

                case 1:
                    return this.Read4Bit(3);

                case 2:
                    return this.Read8Bit(3);

                case 3:
                    buffer = new byte[(this.width * this.height) * 3];
                    this.Read24Bit(buffer);
                    return new RawImage(this.width, this.height, 3, 8, buffer);

                case 4:
                    return this.Read1Bit(4);

                case 5:
                    switch (((int) this.compression))
                    {
                        case 0:
                            return this.Read4Bit(4);

                        case 2:
                            return this.ReadRLE4();
                    }
                    break;

                case 6:
                    switch (((int) this.compression))
                    {
                        case 0:
                            return this.Read8Bit(4);

                        case 1:
                            return this.ReadRLE8();
                    }
                    throw new Exception("Invalid compression specified for BMP file.");

                case 7:
                    buffer = new byte[(this.width * this.height) * 3];
                    this.Read24Bit(buffer);
                    return new RawImage(this.width, this.height, 3, 8, buffer);

                case 8:
                    return this.Read1632Bit(false);

                case 9:
                    return this.Read1632Bit(true);

                case 10:
                    return this.Read1Bit(4);

                case 11:
                    switch (((int) this.compression))
                    {
                        case 0:
                            return this.Read4Bit(4);

                        case 2:
                            return this.ReadRLE4();
                    }
                    throw new Exception("Invalid compression specified for BMP file.");

                case 12:
                    switch (((int) this.compression))
                    {
                        case 0:
                            return this.Read8Bit(4);

                        case 1:
                            return this.ReadRLE8();
                    }
                    throw new Exception("Invalid compression specified for BMP file.");

                case 13:
                    return this.Read1632Bit(false);

                case 14:
                    buffer = new byte[(this.width * this.height) * 3];
                    this.Read24Bit(buffer);
                    return new RawImage(this.width, this.height, 3, 8, buffer);

                case 15:
                    return this.Read1632Bit(true);

                default:
                    return null;
            }
            throw new Exception("Invalid compression specified for BMP file.");
        }

        /// <summary>
        /// Reads a BMP from a stream. The stream is not closed.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Image GetImage(Stream stream)
        {
            return GetImage(stream, false, 0);
        }

        /// <summary>
        /// Reads a BMP from a byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Image GetImage(byte[] data)
        {
            Stream stream = new MemoryStream(data);
            Image image = GetImage(stream);
            image.OriginalData = data;
            return image;
        }

        /// <summary>
        /// Reads a BMP from a stream. The stream is not closed.
        /// The BMP may not have a header and be considered as a plain DIB.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="noHeader"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image GetImage(Stream stream, bool noHeader, int size)
        {
            BitmapImage image = new BitmapImage(stream, noHeader, size);
            Image image2 = image.GetImage();
            image2.SetDpi((int) ((image.xPelsPerMeter * 0.0254) + 0.5), (int) ((image.yPelsPerMeter * 0.0254) + 0.5));
            image2.OriginalType = ImageType.BMP;
            return image2;
        }

        /// <summary>
        /// Gets the palette.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        private byte[] GetPalette(int group)
        {
            if (this.palette == null)
            {
                return null;
            }
            byte[] buffer = new byte[(this.palette.Length / group) * 3];
            int num = this.palette.Length / group;
            for (int i = 0; i < num; i++)
            {
                int index = i * group;
                int num4 = i * 3;
                buffer[num4 + 2] = this.palette[index++];
                buffer[num4 + 1] = this.palette[index++];
                buffer[num4] = this.palette[index];
            }
            return buffer;
        }

        /// <summary>
        /// Indexeds the model.
        /// </summary>
        /// <param name="bdata">The bdata.</param>
        /// <param name="bpc">The BPC.</param>
        /// <param name="paletteEntries">The palette entries.</param>
        /// <returns></returns>
        private Image IndexedModel(byte[] bdata, int bpc, int paletteEntries)
        {
            Image image = new RawImage(this.width, this.height, 1, bpc, bdata);
            byte[] palette = this.GetPalette(paletteEntries);
            int length = palette.Length;
            PdfIndexedColorSpace space = new PdfIndexedColorSpace(PdfName.DeviceRGB, (length / 3) - 1, palette);
            PdfDictionary dictionary2 = new PdfDictionary();
            dictionary2.Add(PdfName.ColorSpace, space);
            PdfDictionary dictionary = dictionary2;
            image.Properties = dictionary;
            return image;
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="noHeader">if set to <c>true</c> [no header].</param>
        protected void LoadImage(Stream stream, bool noHeader)
        {
            long num5;
            long num6;
            long num9;
            int num10;
            int num11;
            int num12;
            int num13;
            int num14;
            int num15;
            int num16;
            int num17;
            int num18;
            long num19;
            long num20;
            long num21;
            int num23;
            if (noHeader || (stream is MemoryStream))
            {
                this.inputStream = stream;
            }
            else
            {
                this.inputStream = stream;
            }
            if (!noHeader)
            {
                if ((ReadUnsignedByte(this.inputStream) != 0x42) || (ReadUnsignedByte(this.inputStream) != 0x4d))
                {
                    throw new Exception("Invalid magic value for BMP file.");
                }
                this.bitmapFileSize = ReadDWord(this.inputStream);
                ReadWord(this.inputStream);
                ReadWord(this.inputStream);
                this.bitmapOffset = ReadDWord(this.inputStream);
            }
            long num = ReadDWord(this.inputStream);
            if (num == 12L)
            {
                this.width = ReadWord(this.inputStream);
                this.height = ReadWord(this.inputStream);
            }
            else
            {
                this.width = ReadLong(this.inputStream);
                this.height = ReadLong(this.inputStream);
            }
            int num2 = ReadWord(this.inputStream);
            this.bitsPerPixel = ReadWord(this.inputStream);
            this.properties["color_planes"] = (int) num2;
            this.properties["bits_per_pixel"] = (int) this.bitsPerPixel;
            if (this.bitmapOffset == 0L)
            {
                this.bitmapOffset = num;
            }
            if (num != 12L)
            {
                this.compression = ReadDWord(this.inputStream);
                this.imageSize = ReadDWord(this.inputStream);
                this.xPelsPerMeter = ReadLong(this.inputStream);
                this.yPelsPerMeter = ReadLong(this.inputStream);
                num5 = ReadDWord(this.inputStream);
                num6 = ReadDWord(this.inputStream);
                switch (((int) this.compression))
                {
                    case 0:
                        this.properties["compression"] = "BI_RGB";
                        break;

                    case 1:
                        this.properties["compression"] = "BI_RLE8";
                        break;

                    case 2:
                        this.properties["compression"] = "BI_RLE4";
                        break;

                    case 3:
                        this.properties["compression"] = "BI_BITFIELDS";
                        break;
                }
            }
            else
            {
                this.properties["bmp_version"] = "BMP v. 2.x";
                if (this.bitsPerPixel == 1)
                {
                    this.imageType = 0;
                }
                else if (this.bitsPerPixel == 4)
                {
                    this.imageType = 1;
                }
                else if (this.bitsPerPixel == 8)
                {
                    this.imageType = 2;
                }
                else if (this.bitsPerPixel == 0x18)
                {
                    this.imageType = 3;
                }
                int num3 = (int)(((this.bitmapOffset - 14L) - num) / 3L);
                int sizeOfPalette = num3 * 3;
                if (this.bitmapOffset == num)
                {
                    switch (this.imageType)
                    {
                        case 0:
                            sizeOfPalette = 6;
                            break;

                        case 1:
                            sizeOfPalette = 0x30;
                            break;

                        case 2:
                            sizeOfPalette = 0x300;
                            break;

                        case 3:
                            sizeOfPalette = 0;
                            break;
                    }
                    this.bitmapOffset = num + sizeOfPalette;
                }
                this.ReadPalette(sizeOfPalette);
                goto Label_0AEA;
            }
            this.properties["x_pixels_per_meter"] = (long) this.xPelsPerMeter;
            this.properties["y_pixels_per_meter"] = (long) this.yPelsPerMeter;
            this.properties["colors_used"] = (long) num5;
            this.properties["colors_important"] = (long) num6;
            if (num != 40L)
            {
                if (num != 0x6cL)
                {
                    this.properties["bmp_version"] = "BMP v. 5.x";
                    throw new Exception("BMP version 5 not implemented yet.");
                }
                this.properties["bmp_version"] = "BMP v. 4.x";
                this.redMask = (int) ReadDWord(this.inputStream);
                this.greenMask = (int) ReadDWord(this.inputStream);
                this.blueMask = (int) ReadDWord(this.inputStream);
                this.alphaMask = (int) ReadDWord(this.inputStream);
                num9 = ReadDWord(this.inputStream);
                num10 = ReadLong(this.inputStream);
                num11 = ReadLong(this.inputStream);
                num12 = ReadLong(this.inputStream);
                num13 = ReadLong(this.inputStream);
                num14 = ReadLong(this.inputStream);
                num15 = ReadLong(this.inputStream);
                num16 = ReadLong(this.inputStream);
                num17 = ReadLong(this.inputStream);
                num18 = ReadLong(this.inputStream);
                num19 = ReadDWord(this.inputStream);
                num20 = ReadDWord(this.inputStream);
                num21 = ReadDWord(this.inputStream);
                if (this.bitsPerPixel == 1)
                {
                    this.imageType = 10;
                }
                else if (this.bitsPerPixel == 4)
                {
                    this.imageType = 11;
                }
                else if (this.bitsPerPixel == 8)
                {
                    this.imageType = 12;
                }
                else if (this.bitsPerPixel == 0x10)
                {
                    this.imageType = 13;
                    if (((int) this.compression) == 0)
                    {
                        this.redMask = 0x7c00;
                        this.greenMask = 0x3e0;
                        this.blueMask = 0x1f;
                    }
                }
                else if (this.bitsPerPixel == 0x18)
                {
                    this.imageType = 14;
                }
                else if (this.bitsPerPixel == 0x20)
                {
                    this.imageType = 15;
                    if (((int) this.compression) == 0)
                    {
                        this.redMask = 0xff0000;
                        this.greenMask = 0xff00;
                        this.blueMask = 0xff;
                    }
                }
                this.properties["red_mask"] = (int) this.redMask;
                this.properties["green_mask"] = (int) this.greenMask;
                this.properties["blue_mask"] = (int) this.blueMask;
                this.properties["alpha_mask"] = (int) this.alphaMask;
                int num22 = (int)(((this.bitmapOffset - 14L) - num) / 4L);
                num23 = num22 * 4;
                if (this.bitmapOffset != num)
                {
                    goto Label_093A;
                }
                switch (this.imageType)
                {
                    case 10:
                        num23 = ((num5 == 0L) ? ((int) 2L) : ((int) num5)) * 4;
                        goto Label_092F;

                    case 11:
                        num23 = ((num5 == 0L) ? ((int) 0x10L) : ((int) num5)) * 4;
                        goto Label_092F;

                    case 12:
                        num23 = ((num5 == 0L) ? ((int) 0x100L) : ((int) num5)) * 4;
                        goto Label_092F;
                }
                num23 = 0;
            }
            else
            {
                int num8;
                switch (((int) this.compression))
                {
                    case 0:
                    case 1:
                    case 2:
                        if (this.bitsPerPixel != 1)
                        {
                            if (this.bitsPerPixel == 4)
                            {
                                this.imageType = 5;
                            }
                            else if (this.bitsPerPixel == 8)
                            {
                                this.imageType = 6;
                            }
                            else if (this.bitsPerPixel == 0x18)
                            {
                                this.imageType = 7;
                            }
                            else if (this.bitsPerPixel == 0x10)
                            {
                                this.imageType = 8;
                                this.redMask = 0x7c00;
                                this.greenMask = 0x3e0;
                                this.blueMask = 0x1f;
                                this.properties["red_mask"] = (int) this.redMask;
                                this.properties["green_mask"] = (int) this.greenMask;
                                this.properties["blue_mask"] = (int) this.blueMask;
                            }
                            else if (this.bitsPerPixel == 0x20)
                            {
                                this.imageType = 9;
                                this.redMask = 0xff0000;
                                this.greenMask = 0xff00;
                                this.blueMask = 0xff;
                                this.properties["red_mask"] = (int) this.redMask;
                                this.properties["green_mask"] = (int) this.greenMask;
                                this.properties["blue_mask"] = (int) this.blueMask;
                            }
                            break;
                        }
                        this.imageType = 4;
                        break;

                    case 3:
                        if (this.bitsPerPixel != 0x10)
                        {
                            if (this.bitsPerPixel == 0x20)
                            {
                                this.imageType = 9;
                            }
                        }
                        else
                        {
                            this.imageType = 8;
                        }
                        this.redMask = (int) ReadDWord(this.inputStream);
                        this.greenMask = (int) ReadDWord(this.inputStream);
                        this.blueMask = (int) ReadDWord(this.inputStream);
                        this.properties["red_mask"] = (int) this.redMask;
                        this.properties["green_mask"] = (int) this.greenMask;
                        this.properties["blue_mask"] = (int) this.blueMask;
                        if (num5 != 0L)
                        {
                            num8 = ((int) num5) * 4;
                            this.ReadPalette(num8);
                        }
                        this.properties["bmp_version"] = "BMP v. 3.x NT";
                        goto Label_0AEA;

                    default:
                        throw new Exception("Invalid compression specified in BMP file.");
                }
                int num7 = (int)(((this.bitmapOffset - 14L) - num) / 4L);
                num8 = num7 * 4;
                if (this.bitmapOffset == num)
                {
                    switch (this.imageType)
                    {
                        case 4:
                            num8 = ((num5 == 0L) ? ((int) 2L) : ((int) num5)) * 4;
                            break;

                        case 5:
                            num8 = ((num5 == 0L) ? ((int) 0x10L) : ((int) num5)) * 4;
                            break;

                        case 6:
                            num8 = ((num5 == 0L) ? ((int) 0x100L) : ((int) num5)) * 4;
                            break;

                        default:
                            num8 = 0;
                            break;
                    }
                    this.bitmapOffset = num + num8;
                }
                this.ReadPalette(num8);
                this.properties["bmp_version"] = "BMP v. 3.x";
                goto Label_0AEA;
            }
        Label_092F:
            this.bitmapOffset = num + num23;
        Label_093A:
            this.ReadPalette(num23);
            switch (((int) num9))
            {
                case 0:
                    this.properties["color_space"] = "LCS_CALIBRATED_RGB";
                    this.properties["redX"] = (int) num10;
                    this.properties["redY"] = (int) num11;
                    this.properties["redZ"] = (int) num12;
                    this.properties["greenX"] = (int) num13;
                    this.properties["greenY"] = (int) num14;
                    this.properties["greenZ"] = (int) num15;
                    this.properties["blueX"] = (int) num16;
                    this.properties["blueY"] = (int) num17;
                    this.properties["blueZ"] = (int) num18;
                    this.properties["gamma_red"] = (long) num19;
                    this.properties["gamma_green"] = (long) num20;
                    this.properties["gamma_blue"] = (long) num21;
                    throw new Exception("Not implemented yet.");

                case 1:
                    this.properties["color_space"] = "LCS_sRGB";
                    break;

                case 2:
                    this.properties["color_space"] = "LCS_CMYK";
                    throw new Exception("Not implemented yet.");
            }
        Label_0AEA:
            if (this.height > 0)
            {
                this.isBottomUp = true;
            }
            else
            {
                this.isBottomUp = false;
                this.height = Math.Abs(this.height);
            }
            if (((this.bitsPerPixel == 1) || (this.bitsPerPixel == 4)) || (this.bitsPerPixel == 8))
            {
                byte[] buffer;
                byte[] buffer2;
                byte[] buffer3;
                int num24;
                if (((this.imageType == 0) || (this.imageType == 1)) || (this.imageType == 2))
                {
                    num24 = this.palette.Length / 3;
                    if (num24 > 0x100)
                    {
                        num24 = 0x100;
                    }
                    buffer = new byte[num24];
                    buffer2 = new byte[num24];
                    buffer3 = new byte[num24];
                    for (int i = 0; i < num24; i++)
                    {
                        int index = 3 * i;
                        buffer3[i] = this.palette[index];
                        buffer2[i] = this.palette[index + 1];
                        buffer[i] = this.palette[index + 2];
                    }
                }
                else
                {
                    num24 = this.palette.Length / 4;
                    if (num24 > 0x100)
                    {
                        num24 = 0x100;
                    }
                    buffer = new byte[num24];
                    buffer2 = new byte[num24];
                    buffer3 = new byte[num24];
                    for (int j = 0; j < num24; j++)
                    {
                        int num28 = 4 * j;
                        buffer3[j] = this.palette[num28];
                        buffer2[j] = this.palette[num28 + 1];
                        buffer[j] = this.palette[num28 + 2];
                    }
                }
            }
            else if (this.bitsPerPixel != 0x10)
            {
                int bitsPerPixel = this.bitsPerPixel;
            }
        }

        /// <summary>
        /// Read1632s the bit.
        /// </summary>
        /// <param name="is32">if set to <c>true</c> [is32].</param>
        /// <returns></returns>
        private Image Read1632Bit(bool is32)
        {
            int num14;
            int num = FindMask(this.redMask);
            int num2 = FindShift(this.redMask);
            int num3 = num + 1;
            int num4 = FindMask(this.greenMask);
            int num5 = FindShift(this.greenMask);
            int num6 = num4 + 1;
            int num7 = FindMask(this.blueMask);
            int num8 = FindShift(this.blueMask);
            int num9 = num7 + 1;
            byte[] data = new byte[(this.width * this.height) * 3];
            int num10 = 0;
            if (!is32)
            {
                int num11 = this.width * 0x10;
                if ((num11 % 0x20) != 0)
                {
                    num10 = (((num11 / 0x20) + 1) * 0x20) - num11;
                    num10 = (int) Math.Ceiling((double) (((double) num10) / 8.0));
                }
            }
            if (((int) this.imageSize) == 0)
            {
                int num12 = (int)(this.bitmapFileSize - this.bitmapOffset);
            }
            int num13 = 0;
            if (this.isBottomUp)
            {
                for (int i = this.height - 1; i >= 0; i--)
                {
                    num13 = (this.width * 3) * i;
                    for (int j = 0; j < this.width; j++)
                    {
                        if (is32)
                        {
                            num14 = (int) ReadDWord(this.inputStream);
                        }
                        else
                        {
                            num14 = ReadWord(this.inputStream);
                        }
                        data[num13++] = (byte)(((Image.USR(num14, num2) & num) * 0x100) / num3);
                        data[num13++] = (byte)(((Image.USR(num14, num5) & num4) * 0x100) / num6);
                        data[num13++] = (byte)(((Image.USR(num14, num8) & num7) * 0x100) / num9);
                    }
                    for (int k = 0; k < num10; k++)
                    {
                        this.inputStream.ReadByte();
                    }
                }
            }
            else
            {
                for (int m = 0; m < this.height; m++)
                {
                    for (int n = 0; n < this.width; n++)
                    {
                        if (is32)
                        {
                            num14 = (int) ReadDWord(this.inputStream);
                        }
                        else
                        {
                            num14 = ReadWord(this.inputStream);
                        }
                        data[num13++] = (byte)(((Image.USR(num14, num2) & num) * 0x100) / num3);
                        data[num13++] = (byte)(((Image.USR(num14, num5) & num4) * 0x100) / num6);
                        data[num13++] = (byte)(((Image.USR(num14, num8) & num7) * 0x100) / num9);
                    }
                    for (int num20 = 0; num20 < num10; num20++)
                    {
                        this.inputStream.ReadByte();
                    }
                }
            }
            return new RawImage(this.width, this.height, 3, 8, data);
        }

        /// <summary>
        /// Deal with 1 Bit images using IndexColorModels
        /// </summary>
        /// <param name="paletteEntries"></param>
        /// <returns></returns>
        private Image Read1Bit(int paletteEntries)
        {
            byte[] destinationArray = new byte[((this.width + 7) / 8) * this.height];
            int num = 0;
            int length = (int) Math.Ceiling((double) (((double) this.width) / 8.0));
            int num3 = length % 4;
            if (num3 != 0)
            {
                num = 4 - num3;
            }
            int num4 = (length + num) * this.height;
            byte[] buffer = new byte[num4];
            for (int i = 0; i < num4; i += this.inputStream.Read(buffer, i, num4 - i))
            {
            }
            if (this.isBottomUp)
            {
                for (int j = 0; j < this.height; j++)
                {
                    Array.Copy(buffer, num4 - ((j + 1) * (length + num)), destinationArray, j * length, length);
                }
            }
            else
            {
                for (int k = 0; k < this.height; k++)
                {
                    Array.Copy(buffer, k * (length + num), destinationArray, k * length, length);
                }
            }
            return this.IndexedModel(destinationArray, 1, paletteEntries);
        }

        /// <summary>
        /// Method to read 24 bit BMP image data
        /// </summary>
        /// <param name="bdata"></param>
        private void Read24Bit(byte[] bdata)
        {
            int num5;
            int num7;
            int num = 0;
            int num2 = this.width * 0x18;
            if ((num2 % 0x20) != 0)
            {
                num = (((num2 / 0x20) + 1) * 0x20) - num2;
                num = (int) Math.Ceiling((double) (((double) num) / 8.0));
            }
            int num3 = ((((this.width * 3) + 3) / 4) * 4) * this.height;
            byte[] buffer = new byte[num3];
            for (int i = 0; i < num3; i += num5)
            {
                num5 = this.inputStream.Read(buffer, i, num3 - i);
                if (num5 < 0)
                {
                    break;
                }
            }
            int index = 0;
            if (this.isBottomUp)
            {
                int num8 = ((this.width * this.height) * 3) - 1;
                num7 = -num;
                for (int j = 0; j < this.height; j++)
                {
                    index = (num8 - (((j + 1) * this.width) * 3)) + 1;
                    num7 += num;
                    for (int k = 0; k < this.width; k++)
                    {
                        bdata[index + 2] = buffer[num7++];
                        bdata[index + 1] = buffer[num7++];
                        bdata[index] = buffer[num7++];
                        index += 3;
                    }
                }
            }
            else
            {
                num7 = -num;
                for (int m = 0; m < this.height; m++)
                {
                    num7 += num;
                    for (int n = 0; n < this.width; n++)
                    {
                        bdata[index + 2] = buffer[num7++];
                        bdata[index + 1] = buffer[num7++];
                        bdata[index] = buffer[num7++];
                        index += 3;
                    }
                }
            }
        }

        /// <summary>
        /// Method to read a 4 bit BMP image data
        /// </summary>
        /// <param name="paletteEntries"></param>
        /// <returns></returns>
        private Image Read4Bit(int paletteEntries)
        {
            byte[] destinationArray = new byte[((this.width + 1) / 2) * this.height];
            int num = 0;
            int length = (int) Math.Ceiling((double) (((double) this.width) / 2.0));
            int num3 = length % 4;
            if (num3 != 0)
            {
                num = 4 - num3;
            }
            int num4 = (length + num) * this.height;
            byte[] buffer = new byte[num4];
            for (int i = 0; i < num4; i += this.inputStream.Read(buffer, i, num4 - i))
            {
            }
            if (this.isBottomUp)
            {
                for (int j = 0; j < this.height; j++)
                {
                    Array.Copy(buffer, num4 - ((j + 1) * (length + num)), destinationArray, j * length, length);
                }
            }
            else
            {
                for (int k = 0; k < this.height; k++)
                {
                    Array.Copy(buffer, k * (length + num), destinationArray, k * length, length);
                }
            }
            return this.IndexedModel(destinationArray, 4, paletteEntries);
        }

        /// <summary>
        /// Method to read 8 bit BMP image data
        /// </summary>
        /// <param name="paletteEntries"></param>
        /// <returns></returns>
        private Image Read8Bit(int paletteEntries)
        {
            byte[] destinationArray = new byte[this.width * this.height];
            int num = 0;
            int num2 = this.width * 8;
            if ((num2 % 0x20) != 0)
            {
                num = (((num2 / 0x20) + 1) * 0x20) - num2;
                num = (int) Math.Ceiling((double) (((double) num) / 8.0));
            }
            int num3 = (this.width + num) * this.height;
            byte[] buffer = new byte[num3];
            for (int i = 0; i < num3; i += this.inputStream.Read(buffer, i, num3 - i))
            {
            }
            if (this.isBottomUp)
            {
                for (int j = 0; j < this.height; j++)
                {
                    Array.Copy(buffer, num3 - ((j + 1) * (this.width + num)), destinationArray, j * this.width, this.width);
                }
            }
            else
            {
                for (int k = 0; k < this.height; k++)
                {
                    Array.Copy(buffer, k * (this.width + num), destinationArray, k * this.width, this.width);
                }
            }
            return this.IndexedModel(destinationArray, 8, paletteEntries);
        }

        /// <summary>
        /// Unsigned 4 bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static long ReadDWord(Stream stream)
        {
            return ReadUnsignedInt(stream);
        }

        /// <summary>
        /// Signed 4 bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static int ReadInt(Stream stream)
        {
            int num = ReadUnsignedByte(stream);
            int num2 = ReadUnsignedByte(stream);
            int num3 = ReadUnsignedByte(stream);
            return ((((ReadUnsignedByte(stream) << 0x18) | (num3 << 0x10)) | (num2 << 8)) | num);
        }

        /// <summary>
        /// 32 bit signed value
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static int ReadLong(Stream stream)
        {
            return ReadInt(stream);
        }

        /// <summary>
        /// Reads the palette.
        /// </summary>
        /// <param name="sizeOfPalette">The size of palette.</param>
        private void ReadPalette(int sizeOfPalette)
        {
            if (sizeOfPalette != 0)
            {
                int num2;
                this.palette = new byte[sizeOfPalette];
                for (int i = 0; i < sizeOfPalette; i += num2)
                {
                    num2 = this.inputStream.Read(this.palette, i, sizeOfPalette - i);
                    if (num2 <= 0)
                    {
                        throw new IOException("incomplete palette");
                    }
                }
                this.properties["palette"] = this.palette;
            }
        }

        /// <summary>
        /// Reads the RL e4.
        /// </summary>
        /// <returns></returns>
        private Image ReadRLE4()
        {
            int imageSize = (int) this.imageSize;
            if (imageSize == 0)
            {
                imageSize = (int)(this.bitmapFileSize - this.bitmapOffset);
            }
            byte[] buffer = new byte[imageSize];
            for (int i = 0; i < imageSize; i += this.inputStream.Read(buffer, i, imageSize - i))
            {
            }
            byte[] buffer2 = this.DecodeRLE(false, buffer);
            if (this.isBottomUp)
            {
                byte[] buffer3 = buffer2;
                buffer2 = new byte[this.width * this.height];
                int num3 = 0;
                for (int k = this.height - 1; k >= 0; k--)
                {
                    int num5 = k * this.width;
                    int num6 = num3 + this.width;
                    while (num3 != num6)
                    {
                        buffer2[num3++] = buffer3[num5++];
                    }
                }
            }
            int num7 = (this.width + 1) / 2;
            byte[] bdata = new byte[num7 * this.height];
            int num8 = 0;
            int num9 = 0;
            for (int j = 0; j < this.height; j++)
            {
                for (int m = 0; m < this.width; m++)
                {
                    if ((m & 1) == 0)
                    {
                        bdata[num9 + (m / 2)] = (byte)(buffer2[num8++] << 4);
                    }
                    else
                    {
                        bdata[num9 + (m / 2)] |= (byte)(buffer2[num8++] & 15);
                    }
                }
                num9 += num7;
            }
            return this.IndexedModel(bdata, 4, 4);
        }

        /// <summary>
        /// Reads the RL e8.
        /// </summary>
        /// <returns></returns>
        private Image ReadRLE8()
        {
            int imageSize = (int) this.imageSize;
            if (imageSize == 0)
            {
                imageSize = (int)(this.bitmapFileSize - this.bitmapOffset);
            }
            byte[] buffer = new byte[imageSize];
            for (int i = 0; i < imageSize; i += this.inputStream.Read(buffer, i, imageSize - i))
            {
            }
            byte[] sourceArray = this.DecodeRLE(true, buffer);
            imageSize = this.width * this.height;
            if (this.isBottomUp)
            {
                byte[] destinationArray = new byte[sourceArray.Length];
                int width = this.width;
                for (int j = 0; j < this.height; j++)
                {
                    Array.Copy(sourceArray, imageSize - ((j + 1) * width), destinationArray, j * width, width);
                }
                sourceArray = destinationArray;
            }
            return this.IndexedModel(sourceArray, 8, 4);
        }

        /// <summary>
        /// Unsigned 8 bits
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static int ReadUnsignedByte(Stream stream)
        {
            return (stream.ReadByte() & 0xff);
        }

        /// <summary>
        /// Unsigned 4 bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static long ReadUnsignedInt(Stream stream)
        {
            int num = ReadUnsignedByte(stream);
            int num2 = ReadUnsignedByte(stream);
            int num3 = ReadUnsignedByte(stream);
            long num5 = (((ReadUnsignedByte(stream) << 0x18) | (num3 << 0x10)) | (num2 << 8)) | num;
            return (num5 & 0xffffffffL);
        }

        /// <summary>
        /// Unsigned 2 bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static int ReadUnsignedShort(Stream stream)
        {
            int num = ReadUnsignedByte(stream);
            return (((ReadUnsignedByte(stream) << 8) | num) & 0xffff);
        }

        /// <summary>
        /// Unsigned 16 bits
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static int ReadWord(Stream stream)
        {
            return ReadUnsignedShort(stream);
        }
    }
}

