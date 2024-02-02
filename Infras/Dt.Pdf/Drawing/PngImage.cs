#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Object;
using Dt.Pdf.Utility.zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Png Image Object
    /// </summary>
    internal class PngImage
    {
        private int bitDepth;
        private int bytesPerPixel;
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string cHRM = "cHRM";
        private int colorType;
        private Stream dataStream;
        private int dpiX;
        private int dpiY;
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string gAMA = "gAMA";
        private float gamma = 1f;
        private bool genBWMask;
        private bool hasCHRM;
        private int height;
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string iCCP = "iCCP";
        private readonly MemoryStream idat = new MemoryStream();
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string IDAT = "IDAT";
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string IEND = "IEND";
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string IHDR = "IHDR";
        private byte[] image;
        private int inputBands;
        private PdfName intent;
        private static readonly PdfName[] intents = new PdfName[] { PdfName.Perceptual, PdfName.RelativeColorimetric, PdfName.Saturation, PdfName.AbsoluteColorimetric };
        private int interlaceMethod;
        private bool palShades;
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string pHYs = "pHYs";
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string PLTE = "PLTE";
        private const int PNG_FILTER_AVERAGE = 3;
        private const int PNG_FILTER_NONE = 0;
        private const int PNG_FILTER_PAETH = 4;
        private const int PNG_FILTER_SUB = 1;
        private const int PNG_FILTER_UP = 2;
        /// <summary>
        /// Some PNG specific values.
        /// </summary>
        public static int[] PNGID = new int[] { 0x89, 80, 0x4e, 0x47, 13, 10, 0x1a, 10 };
        private readonly PdfDictionary properties = new PdfDictionary();
        private byte[] smask;
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string sRGB = "sRGB";
        private readonly Stream stream;
        private byte[] trans;
        private int transBlue = -1;
        private const int TRANSFERSIZE = 0x1000;
        private int transGreen = -1;
        private int transRedGray = -1;
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string tRNS = "tRNS";
        private int width;
        private float xB;
        private float xG;
        private float xR;
        private float xW;
        private float XYRatio;
        private float yB;
        private float yG;
        private float yR;
        private float yW;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PngImage" /> class.
        /// </summary>
        /// <param name="isp">The isp.</param>
        private PngImage(Stream isp)
        {
            this.stream = isp;
        }

        /// <summary>
        /// Checks the marker.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        private static bool CheckMarker(string s)
        {
            if (s.Length != 4)
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                char ch = s[i];
                if (((ch < 'a') || (ch > 'z')) && ((ch < 'A') || (ch > 'Z')))
                {
                    return false;
                }
            }
            return true;
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
            return ((((c1 == PNGID[0]) && (c2 == PNGID[1])) && (c3 == PNGID[2])) && (c4 == PNGID[3]));
        }

        /// <summary>
        /// Decodes the average filter.
        /// </summary>
        /// <param name="curr">The curr.</param>
        /// <param name="prev">The prev.</param>
        /// <param name="count">The count.</param>
        /// <param name="bpp">The BPP.</param>
        private static void DecodeAverageFilter(byte[] curr, byte[] prev, int count, int bpp)
        {
            int num;
            int num2;
            for (int i = 0; i < bpp; i++)
            {
                num = curr[i] & 0xff;
                num2 = prev[i] & 0xff;
                curr[i] = (byte)(num + (num2 / 2));
            }
            for (int j = bpp; j < count; j++)
            {
                num = curr[j] & 0xff;
                int num5 = curr[j - bpp] & 0xff;
                num2 = prev[j] & 0xff;
                curr[j] = (byte)(num + ((num5 + num2) / 2));
            }
        }

        /// <summary>
        /// Decodes the idat.
        /// </summary>
        private void DecodeIdat()
        {
            int bitDepth = this.bitDepth;
            if (bitDepth == 0x10)
            {
                bitDepth = 8;
            }
            int num2 = -1;
            this.bytesPerPixel = (this.bitDepth == 0x10) ? 2 : 1;
            switch (this.colorType)
            {
                case 0:
                    num2 = (((bitDepth * this.width) + 7) / 8) * this.height;
                    break;

                case 2:
                    num2 = (this.width * 3) * this.height;
                    this.bytesPerPixel *= 3;
                    break;

                case 3:
                    if (this.interlaceMethod == 1)
                    {
                        num2 = (((bitDepth * this.width) + 7) / 8) * this.height;
                    }
                    this.bytesPerPixel = 1;
                    break;

                case 4:
                    num2 = this.width * this.height;
                    this.bytesPerPixel *= 2;
                    break;

                case 6:
                    num2 = (this.width * 3) * this.height;
                    this.bytesPerPixel *= 4;
                    break;
            }
            if (num2 >= 0)
            {
                this.image = new byte[num2];
            }
            if (this.palShades)
            {
                this.smask = new byte[this.width * this.height];
            }
            else if (this.genBWMask)
            {
                this.smask = new byte[((this.width + 7) / 8) * this.height];
            }
            this.idat.Position = 0L;
            this.dataStream = new ZInflaterInputStream(this.idat);
            if (this.interlaceMethod != 1)
            {
                this.DecodePass(0, 0, 1, 1, this.width, this.height);
            }
            else
            {
                this.DecodePass(0, 0, 8, 8, (this.width + 7) / 8, (this.height + 7) / 8);
                this.DecodePass(4, 0, 8, 8, (this.width + 3) / 8, (this.height + 7) / 8);
                this.DecodePass(0, 4, 4, 8, (this.width + 3) / 4, (this.height + 3) / 8);
                this.DecodePass(2, 0, 4, 4, (this.width + 1) / 4, (this.height + 3) / 4);
                this.DecodePass(0, 2, 2, 4, (this.width + 1) / 2, (this.height + 1) / 4);
                this.DecodePass(1, 0, 2, 2, this.width / 2, (this.height + 1) / 2);
                this.DecodePass(0, 1, 1, 2, this.width, this.height / 2);
            }
        }

        /// <summary>
        /// Decodes the paeth filter.
        /// </summary>
        /// <param name="curr">The curr.</param>
        /// <param name="prev">The prev.</param>
        /// <param name="count">The count.</param>
        /// <param name="bpp">The BPP.</param>
        private static void DecodePaethFilter(byte[] curr, byte[] prev, int count, int bpp)
        {
            int num;
            int num2;
            for (int i = 0; i < bpp; i++)
            {
                num = curr[i] & 0xff;
                num2 = prev[i] & 0xff;
                curr[i] = (byte)(num + num2);
            }
            for (int j = bpp; j < count; j++)
            {
                num = curr[j] & 0xff;
                int a = curr[j - bpp] & 0xff;
                num2 = prev[j] & 0xff;
                int c = prev[j - bpp] & 0xff;
                curr[j] = (byte)(num + PaethPredictor(a, num2, c));
            }
        }

        /// <summary>
        /// Decodes the pass.
        /// </summary>
        /// <param name="xOffset">The x offset.</param>
        /// <param name="yOffset">The y offset.</param>
        /// <param name="xStep">The x step.</param>
        /// <param name="yStep">The y step.</param>
        /// <param name="passWidth">Width of the pass.</param>
        /// <param name="passHeight">Height of the pass.</param>
        private void DecodePass(int xOffset, int yOffset, int xStep, int yStep, int passWidth, int passHeight)
        {
            if ((passWidth != 0) && (passHeight != 0))
            {
                int count = (((this.inputBands * passWidth) * this.bitDepth) + 7) / 8;
                byte[] b = new byte[count];
                byte[] prev = new byte[count];
                int num2 = 0;
                for (int i = yOffset; num2 < passHeight; i += yStep)
                {
                    int num4 = 0;
                    try
                    {
                        num4 = this.dataStream.ReadByte();
                        ReadFully(this.dataStream, b, 0, count);
                    }
                    catch
                    {
                    }
                    switch (num4)
                    {
                        case 0:
                            break;

                        case 1:
                            DecodeSubFilter(b, count, this.bytesPerPixel);
                            break;

                        case 2:
                            DecodeUpFilter(b, prev, count);
                            break;

                        case 3:
                            DecodeAverageFilter(b, prev, count, this.bytesPerPixel);
                            break;

                        case 4:
                            DecodePaethFilter(b, prev, count, this.bytesPerPixel);
                            break;

                        default:
                            throw new Exception("PNG filter unknown.");
                    }
                    this.ProcessPixels(b, xOffset, xStep, i, passWidth);
                    byte[] buffer3 = prev;
                    prev = b;
                    b = buffer3;
                    num2++;
                }
            }
        }

        /// <summary>
        /// Decodes the sub filter.
        /// </summary>
        /// <param name="curr">The curr.</param>
        /// <param name="count">The count.</param>
        /// <param name="bpp">The BPP.</param>
        private static void DecodeSubFilter(byte[] curr, int count, int bpp)
        {
            for (int i = bpp; i < count; i++)
            {
                int num2 = curr[i] & 0xff;
                num2 += curr[i - bpp] & 0xff;
                curr[i] = (byte) num2;
            }
        }

        /// <summary>
        /// Decodes up filter.
        /// </summary>
        /// <param name="curr">The curr.</param>
        /// <param name="prev">The prev.</param>
        /// <param name="count">The count.</param>
        private static void DecodeUpFilter(byte[] curr, byte[] prev, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int num2 = curr[i] & 0xff;
                int num3 = prev[i] & 0xff;
                curr[i] = (byte)(num2 + num3);
            }
        }

        /// <summary>
        /// Gets the colorspace.
        /// </summary>
        /// <returns></returns>
        private PdfObjectBase GetColorspace()
        {
            if ((this.gamma == 1f) && !this.hasCHRM)
            {
                if ((this.colorType & 2) != 0)
                {
                    return PdfName.DeviceRGB;
                }
                return PdfName.DeviceGray;
            }
            PdfArray array = new PdfArray();
            PdfDictionary item = new PdfDictionary();
            if ((this.colorType & 2) == 0)
            {
                if (this.gamma == 1f)
                {
                    return PdfName.DeviceGray;
                }
                array.Add(PdfName.CalGray);
                item.Add(PdfName.Gamma, new PdfNumber((double) this.gamma));
                item.Add(PdfName.WhitePoint, PdfArray.Convert(new int[] { 1, 1, 1 }));
                array.Add(item);
                return array;
            }
            PdfArray array2 = PdfArray.Convert(new int[] { 1, 1, 1 });
            array.Add(PdfName.CalRGB);
            if (this.gamma != 1f)
            {
                PdfArray array3 = new PdfArray();
                PdfNumber number = new PdfNumber((double) this.gamma);
                array3.Add(number);
                array3.Add(number);
                array3.Add(number);
                item.Add(PdfName.Gamma, array3);
            }
            if (this.hasCHRM)
            {
                float num = this.yW * ((((this.xG - this.xB) * this.yR) - ((this.xR - this.xB) * this.yG)) + ((this.xR - this.xG) * this.yB));
                float num2 = (this.yR * ((((this.xG - this.xB) * this.yW) - ((this.xW - this.xB) * this.yG)) + ((this.xW - this.xG) * this.yB))) / num;
                float num3 = (num2 * this.xR) / this.yR;
                float num4 = num2 * (((1f - this.xR) / this.yR) - 1f);
                float num5 = (-this.yG * ((((this.xR - this.xB) * this.yW) - ((this.xW - this.xB) * this.yR)) + ((this.xW - this.xR) * this.yB))) / num;
                float num6 = (num5 * this.xG) / this.yG;
                float num7 = num5 * (((1f - this.xG) / this.yG) - 1f);
                float num8 = (this.yB * ((((this.xR - this.xG) * this.yW) - ((this.xW - this.xG) * this.yW)) + ((this.xW - this.xR) * this.yG))) / num;
                float num9 = (num8 * this.xB) / this.yB;
                float num10 = num8 * (((1f - this.xB) / this.yB) - 1f);
                float num11 = (num3 + num6) + num9;
                float num12 = (num4 + num7) + num10;
                array2 = new PdfArray {
                    new PdfNumber((double) num11),
                    new PdfNumber(1.0),
                    new PdfNumber((double) num12)
                };
                PdfArray array5 = new PdfArray {
                    new PdfNumber((double) num3),
                    new PdfNumber((double) num2),
                    new PdfNumber((double) num4),
                    new PdfNumber((double) num6),
                    new PdfNumber((double) num5),
                    new PdfNumber((double) num7),
                    new PdfNumber((double) num9),
                    new PdfNumber((double) num8),
                    new PdfNumber((double) num10)
                };
                item.Add(PdfName.Matrix, array5);
            }
            item.Add(PdfName.WhitePoint, array2);
            array.Add(item);
            return array;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns></returns>
        private Image GetImage()
        {
            Image image;
            this.LoadImage();
            int num = 0;
            int num2 = 0;
            this.palShades = false;
            if (this.trans != null)
            {
                for (int i = 0; i < this.trans.Length; i++)
                {
                    int num4 = this.trans[i] & 0xff;
                    if (num4 == 0)
                    {
                        num++;
                        num2 = i;
                    }
                    if ((num4 != 0) && (num4 != 0xff))
                    {
                        this.palShades = true;
                        break;
                    }
                }
            }
            if ((this.colorType & 4) != 0)
            {
                this.palShades = true;
            }
            this.genBWMask = !this.palShades && ((num > 1) || (this.transRedGray >= 0));
            if ((!this.palShades && !this.genBWMask) && (num == 1))
            {
                this.properties.Add(PdfName.Mask, PdfArray.Convert(new int[] { num2, num2 }));
            }
            bool flag = (((this.interlaceMethod == 1) || (this.bitDepth == 0x10)) || (((this.colorType & 4) != 0) || this.palShades)) || this.genBWMask;
            switch (this.colorType)
            {
                case 0:
                    this.inputBands = 1;
                    break;

                case 2:
                    this.inputBands = 3;
                    break;

                case 3:
                    this.inputBands = 1;
                    break;

                case 4:
                    this.inputBands = 2;
                    break;

                case 6:
                    this.inputBands = 4;
                    break;
            }
            if (flag)
            {
                this.DecodeIdat();
            }
            int inputBands = this.inputBands;
            if ((this.colorType & 4) != 0)
            {
                inputBands--;
            }
            int bitDepth = this.bitDepth;
            if (bitDepth == 0x10)
            {
                bitDepth = 8;
            }
            if (this.image != null)
            {
                image = new RawImage(this.width, this.height, inputBands, bitDepth, this.image);
            }
            else
            {
                RawImage image2 = new RawImage(this.width, this.height, inputBands, bitDepth, this.idat.ToArray()) {
                    Deflated = true
                };
                image = image2;
                PdfDictionary dictionary2 = new PdfDictionary();
                dictionary2.Add(PdfName.BitsPerComponent, new PdfNumber((double) this.bitDepth));
                dictionary2.Add(PdfName.Predictor, new PdfNumber(15.0));
                dictionary2.Add(PdfName.Columns, new PdfNumber((double) this.width));
                dictionary2.Add(PdfName.Colors, new PdfNumber(((this.colorType == 3) || ((this.colorType & 2) == 0)) ? ((double) 1) : ((double) 3)));
                PdfDictionary dictionary = dictionary2;
                this.properties.Add(PdfName.DecodeParms, dictionary);
            }
            if (!this.properties.ContainsKey(PdfName.ColorSpace))
            {
                this.properties.Add(PdfName.ColorSpace, this.GetColorspace());
            }
            if (this.intent != null)
            {
                this.properties.Add(PdfName.Intent, this.intent);
            }
            if (this.properties.Count > 0)
            {
                image.Properties = this.properties;
            }
            if (this.palShades)
            {
                Image image3 = new RawImage(this.width, this.height, 1, 8, this.smask);
                image3.MakeMask();
                image.ImageMask = image3;
            }
            if (this.genBWMask)
            {
                Image image4 = new RawImage(this.width, this.height, 1, 1, this.smask);
                image4.MakeMask();
                image.ImageMask = image4;
            }
            image.SetDpi(this.dpiX, this.dpiY);
            image.XYRatio = this.XYRatio;
            image.OriginalType = ImageType.PNG;
            return image;
        }

        /// <summary>
        /// Reads a PNG from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Image GetImage(Stream stream)
        {
            PngImage image = new PngImage(stream);
            return image.GetImage();
        }

        /// <summary>
        /// Reads a PNG from a byte array.
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
        /// Gets the int.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static int GetInt(Stream stream)
        {
            return ((((stream.ReadByte() << 0x18) + (stream.ReadByte() << 0x10)) + (stream.ReadByte() << 8)) + stream.ReadByte());
        }

        /// <summary>
        /// Gets the pixel.
        /// </summary>
        /// <param name="curr">The curr.</param>
        /// <returns></returns>
        private int[] GetPixel(byte[] curr)
        {
            switch (this.bitDepth)
            {
                case 8:
                {
                    int[] numArray = new int[curr.Length];
                    for (int j = 0; j < numArray.Length; j++)
                    {
                        numArray[j] = curr[j] & 0xff;
                    }
                    return numArray;
                }
                case 0x10:
                {
                    int[] numArray2 = new int[curr.Length / 2];
                    for (int k = 0; k < numArray2.Length; k++)
                    {
                        numArray2[k] = ((curr[k * 2] & 0xff) << 8) + (curr[(k * 2) + 1] & 0xff);
                    }
                    return numArray2;
                }
            }
            int[] numArray3 = new int[(curr.Length * 8) / this.bitDepth];
            int num3 = 0;
            int num4 = 8 / this.bitDepth;
            int num5 = (((int) 1) << this.bitDepth) - 1;
            for (int i = 0; i < curr.Length; i++)
            {
                for (int m = num4 - 1; m >= 0; m--)
                {
                    numArray3[num3++] = Image.USR(curr[i], this.bitDepth * m) & num5;
                }
            }
            return numArray3;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static string GetString(Stream stream)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                builder.Append((char) stream.ReadByte());
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static int GetWord(Stream stream)
        {
            return ((stream.ReadByte() << 8) + stream.ReadByte());
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        private void LoadImage()
        {
            for (int i = 0; i < PNGID.Length; i++)
            {
                if (PNGID[i] != this.stream.ReadByte())
                {
                    throw new PdfUnrecognizedImageFormatException("File is not a valid PNG.");
                }
            }
            byte[] buffer = new byte[0x1000];
            while (true)
            {
                int @int = GetInt(this.stream);
                string s = GetString(this.stream);
                if ((@int < 0) || !CheckMarker(s))
                {
                    throw new PdfUnrecognizedImageFormatException("Corrupted PNG file.");
                }
                if ("IDAT".Equals(s))
                {
                    while (@int != 0)
                    {
                        int count = this.stream.Read(buffer, 0, Math.Min(@int, 0x1000));
                        if (count <= 0)
                        {
                            return;
                        }
                        this.idat.Write(buffer, 0, count);
                        @int -= count;
                    }
                }
                else if (!"tRNS".Equals(s))
                {
                    if ("IHDR".Equals(s))
                    {
                        this.width = GetInt(this.stream);
                        this.height = GetInt(this.stream);
                        this.bitDepth = this.stream.ReadByte();
                        this.colorType = this.stream.ReadByte();
                        this.stream.ReadByte();
                        this.stream.ReadByte();
                        this.interlaceMethod = this.stream.ReadByte();
                    }
                    else if ("PLTE".Equals(s))
                    {
                        if (this.colorType == 3)
                        {
                            List<byte> list = new List<byte>();
                            while (@int-- > 0)
                            {
                                list.Add((byte) this.stream.ReadByte());
                            }
                            list.ToArray();
                            PdfIndexedColorSpace space = new PdfIndexedColorSpace(this.GetColorspace(), (@int / 3) - 1, list.ToArray());
                            this.properties.Add(PdfName.ColorSpace, space);
                        }
                        else
                        {
                            Image.Skip(this.stream, @int);
                        }
                    }
                    else if ("pHYs".Equals(s))
                    {
                        int num9 = GetInt(this.stream);
                        int num10 = GetInt(this.stream);
                        if (this.stream.ReadByte() == 1)
                        {
                            this.dpiX = (int)((num9 * 0.0254f) + 0.5f);
                            this.dpiY = (int)((num10 * 0.0254f) + 0.5f);
                        }
                        else if (num10 != 0)
                        {
                            this.XYRatio = ((float) num9) / ((float) num10);
                        }
                    }
                    else if ("cHRM".Equals(s))
                    {
                        this.xW = ((float) GetInt(this.stream)) / 100000f;
                        this.yW = ((float) GetInt(this.stream)) / 100000f;
                        this.xR = ((float) GetInt(this.stream)) / 100000f;
                        this.yR = ((float) GetInt(this.stream)) / 100000f;
                        this.xG = ((float) GetInt(this.stream)) / 100000f;
                        this.yG = ((float) GetInt(this.stream)) / 100000f;
                        this.xB = ((float) GetInt(this.stream)) / 100000f;
                        this.yB = ((float) GetInt(this.stream)) / 100000f;
                        this.hasCHRM = ((((Math.Abs(this.xW) >= 0.0001f) && (Math.Abs(this.yW) >= 0.0001f)) && ((Math.Abs(this.xR) >= 0.0001f) && (Math.Abs(this.yR) >= 0.0001f))) && (((Math.Abs(this.xG) >= 0.0001f) && (Math.Abs(this.yG) >= 0.0001f)) && (Math.Abs(this.xB) >= 0.0001f))) && (Math.Abs(this.yB) >= 0.0001f);
                    }
                    else if ("sRGB".Equals(s))
                    {
                        int index = this.stream.ReadByte();
                        this.intent = intents[index];
                        this.gamma = 2.2f;
                        this.xW = 0.3127f;
                        this.yW = 0.329f;
                        this.xR = 0.64f;
                        this.yR = 0.33f;
                        this.xG = 0.3f;
                        this.yG = 0.6f;
                        this.xB = 0.15f;
                        this.yB = 0.06f;
                        this.hasCHRM = true;
                    }
                    else if ("gAMA".Equals(s))
                    {
                        int num13 = GetInt(this.stream);
                        if (num13 != 0)
                        {
                            this.gamma = 100000f / ((float) num13);
                            if (!this.hasCHRM)
                            {
                                this.xW = 0.3127f;
                                this.yW = 0.329f;
                                this.xR = 0.64f;
                                this.yR = 0.33f;
                                this.xG = 0.3f;
                                this.yG = 0.6f;
                                this.xB = 0.15f;
                                this.yB = 0.06f;
                                this.hasCHRM = true;
                            }
                        }
                    }
                    else if (!"iCCP".Equals(s))
                    {
                        if ("IEND".Equals(s))
                        {
                            return;
                        }
                        Image.Skip(this.stream, @int);
                    }
                }
                else
                {
                    switch (this.colorType)
                    {
                        case 0:
                            if (@int >= 2)
                            {
                                @int -= 2;
                                int word = GetWord(this.stream);
                                if (this.bitDepth != 0x10)
                                {
                                    this.properties.Add(PdfName.Mask, PdfArray.Convert(new int[] { word, word }));
                                    break;
                                }
                                this.transRedGray = word;
                            }
                            break;

                        case 2:
                            if (@int >= 6)
                            {
                                @int -= 6;
                                int num5 = GetWord(this.stream);
                                int num6 = GetWord(this.stream);
                                int num7 = GetWord(this.stream);
                                if (this.bitDepth != 0x10)
                                {
                                    this.properties.Add(PdfName.Mask, PdfArray.Convert(new int[] { num5, num5, num6, num6, num7, num7 }));
                                    break;
                                }
                                this.transRedGray = num5;
                                this.transGreen = num6;
                                this.transBlue = num7;
                            }
                            break;

                        case 3:
                            if (@int > 0)
                            {
                                this.trans = new byte[@int];
                                for (int j = 0; j < @int; j++)
                                {
                                    this.trans[j] = (byte) this.stream.ReadByte();
                                }
                                @int = 0;
                            }
                            break;
                    }
                    Image.Skip(this.stream, @int);
                }
                Image.Skip(this.stream, 4);
            }
        }

        /// <summary>
        /// Paethes the predictor.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        private static int PaethPredictor(int a, int b, int c)
        {
            int num = (a + b) - c;
            int num2 = Math.Abs((int) (num - a));
            int num3 = Math.Abs((int) (num - b));
            int num4 = Math.Abs((int) (num - c));
            if ((num2 <= num3) && (num2 <= num4))
            {
                return a;
            }
            if (num3 > num4)
            {
                return c;
            }
            return b;
        }

        /// <summary>
        /// Processes the pixels.
        /// </summary>
        /// <param name="curr">The curr.</param>
        /// <param name="xOffset">The x offset.</param>
        /// <param name="step">The step.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        private void ProcessPixels(byte[] curr, int xOffset, int step, int y, int w)
        {
            int num;
            int num2;
            int[] pixel = this.GetPixel(curr);
            int size = 0;
            switch (this.colorType)
            {
                case 0:
                case 3:
                case 4:
                    size = 1;
                    break;

                case 2:
                case 6:
                    size = 3;
                    break;
            }
            if (this.image != null)
            {
                num2 = xOffset;
                int bytesPerRow = (((size * this.width) * ((this.bitDepth == 0x10) ? 8 : this.bitDepth)) + 7) / 8;
                for (num = 0; num < w; num++)
                {
                    SetPixel(this.image, pixel, this.inputBands * num, size, num2, y, this.bitDepth, bytesPerRow);
                    num2 += step;
                }
            }
            if (this.palShades)
            {
                if ((this.colorType & 4) != 0)
                {
                    if (this.bitDepth == 0x10)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            int index = (i * this.inputBands) + size;
                            pixel[index] = Image.USR(pixel[index], 8);
                        }
                    }
                    int width = this.width;
                    num2 = xOffset;
                    for (num = 0; num < w; num++)
                    {
                        SetPixel(this.smask, pixel, (this.inputBands * num) + size, 1, num2, y, 8, width);
                        num2 += step;
                    }
                }
                else
                {
                    int num8 = this.width;
                    int[] data = new int[1];
                    num2 = xOffset;
                    for (num = 0; num < w; num++)
                    {
                        int num9 = pixel[num];
                        if (num9 < this.trans.Length)
                        {
                            data[0] = this.trans[num9];
                        }
                        SetPixel(this.smask, data, 0, 1, num2, y, 8, num8);
                        num2 += step;
                    }
                }
            }
            else if (this.genBWMask)
            {
                switch (this.colorType)
                {
                    case 0:
                    {
                        int num12 = (this.width + 7) / 8;
                        int[] numArray4 = new int[1];
                        num2 = xOffset;
                        num = 0;
                        while (num < w)
                        {
                            int num13 = pixel[num];
                            numArray4[0] = (num13 == this.transRedGray) ? 1 : 0;
                            SetPixel(this.smask, numArray4, 0, 1, num2, y, 1, num12);
                            num2 += step;
                            num++;
                        }
                        return;
                    }
                    case 1:
                        break;

                    case 2:
                    {
                        int num14 = (this.width + 7) / 8;
                        int[] numArray5 = new int[1];
                        num2 = xOffset;
                        num = 0;
                        while (num < w)
                        {
                            int num15 = this.inputBands * num;
                            numArray5[0] = (((pixel[num15] == this.transRedGray) && (pixel[num15 + 1] == this.transGreen)) && (pixel[num15 + 2] == this.transBlue)) ? 1 : 0;
                            SetPixel(this.smask, numArray5, 0, 1, num2, y, 1, num14);
                            num2 += step;
                            num++;
                        }
                        break;
                    }
                    case 3:
                    {
                        int num10 = (this.width + 7) / 8;
                        int[] numArray3 = new int[1];
                        num2 = xOffset;
                        for (num = 0; num < w; num++)
                        {
                            int num11 = pixel[num];
                            if (num11 < this.trans.Length)
                            {
                                numArray3[0] = (this.trans[num11] == 0) ? 1 : 0;
                            }
                            SetPixel(this.smask, numArray3, 0, 1, num2, y, 1, num10);
                            num2 += step;
                        }
                        return;
                    }
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Reads the fully.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="b">The b.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        private static void ReadFully(Stream stream, byte[] b, int offset, int count)
        {
            while (count > 0)
            {
                int num = stream.Read(b, offset, count);
                if (num <= 0)
                {
                    throw new IOException("Insufficient data.");
                }
                count -= num;
                offset += num;
            }
        }

        /// <summary>
        /// Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="bitDepth">The bit depth.</param>
        /// <param name="bytesPerRow">The bytes per row.</param>
        private static void SetPixel(byte[] image, int[] data, int offset, int size, int x, int y, int bitDepth, int bytesPerRow)
        {
            if (bitDepth == 8)
            {
                int num = (bytesPerRow * y) + (size * x);
                for (int i = 0; i < size; i++)
                {
                    image[num + i] = (byte) data[i + offset];
                }
            }
            else if (bitDepth == 0x10)
            {
                int num3 = (bytesPerRow * y) + (size * x);
                for (int j = 0; j < size; j++)
                {
                    image[num3 + j] = (byte)(data[j + offset] >> 8);
                }
            }
            else
            {
                int index = (bytesPerRow * y) + (x / (8 / bitDepth));
                int num6 = ((int) data[offset]) << ((8 - (bitDepth * (x % (8 / bitDepth)))) - bitDepth);
                image[index] |= (byte) num6;
            }
        }
    }
}

