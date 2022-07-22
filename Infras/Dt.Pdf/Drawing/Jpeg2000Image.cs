#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.Exceptions;
using System;
using System.IO;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Jpeg2000 Image
    /// </summary>
    internal class Jpeg2000Image : Image
    {
        private int boxLength;
        private int boxType;
        private Stream inp;
        public const int JP2_BPCC = 0x62706363;
        public const int JP2_COLR = 0x636f6c72;
        public const int JP2_DBTL = 0x6474626c;
        public const int JP2_FTYP = 0x66747970;
        public const int JP2_IHDR = 0x69686472;
        public const int JP2_JP = 0x6a502020;
        public const int JP2_JP2 = 0x6a703220;
        public const int JP2_JP2C = 0x6a703263;
        public const int JP2_JP2H = 0x6a703268;
        public const int JP2_URL = 0x75726c20;
        public const int JPIP_JPIP = 0x6a706970;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Jpeg2000Image" /> class.
        /// </summary>
        /// <param name="img">The img.</param>
        public Jpeg2000Image(byte[] img)
        {
            base.rawData = img;
            base.originalData = img;
            this.LoadImage();
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
            return ((((c1 == 0) && (c2 == 0)) && ((c3 == 0) && (c4 == 12))) || ((((c1 == 0xff) && (c2 == 0x4f)) && (c3 == 0xff)) && (c4 == 0x51)));
        }

        /// <summary>
        /// Cio_reads the specified n.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        private int Cio_read(int n)
        {
            int num = 0;
            for (int i = n - 1; i >= 0; i--)
            {
                num += ((int) this.inp.ReadByte()) << (i << 3);
            }
            return num;
        }

        /// <summary>
        /// Jp2_read_boxhdrs this instance.
        /// </summary>
        public void Jp2_read_boxhdr()
        {
            this.boxLength = this.Cio_read(4);
            this.boxType = this.Cio_read(4);
            if (this.boxLength == 1)
            {
                if (this.Cio_read(4) != 0)
                {
                    throw new PdfUnrecognizedImageFormatException("Cannot handle box sizes higher than 2^32");
                }
                this.boxLength = this.Cio_read(4);
                if (this.boxLength == 0)
                {
                    throw new PdfUnrecognizedImageFormatException("Unsupported box size == 0");
                }
            }
            else if (this.boxLength == 0)
            {
                throw new PdfUnrecognizedImageFormatException("Unsupported box size == 0");
            }
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        private void LoadImage()
        {
            base.type = ImageType.JPEG2000;
            this.inp = null;
            try
            {
                this.inp = new MemoryStream(base.rawData);
                this.boxLength = this.Cio_read(4);
                if (this.boxLength == 12)
                {
                    this.boxType = this.Cio_read(4);
                    if (0x6a502020 != this.boxType)
                    {
                        throw new PdfUnrecognizedImageFormatException("Expected JP Marker");
                    }
                    if (0xd0a870a != this.Cio_read(4))
                    {
                        throw new PdfUnrecognizedImageFormatException("Error with JP Marker");
                    }
                    this.Jp2_read_boxhdr();
                    if (0x66747970 != this.boxType)
                    {
                        throw new PdfUnrecognizedImageFormatException("Expected FTYP Marker");
                    }
                    Image.Skip(this.inp, this.boxLength - 8);
                    this.Jp2_read_boxhdr();
                    do
                    {
                        if (0x6a703268 != this.boxType)
                        {
                            if (this.boxType == 0x6a703263)
                            {
                                throw new PdfUnrecognizedImageFormatException("Expected JP2H Marker");
                            }
                            Image.Skip(this.inp, this.boxLength - 8);
                            this.Jp2_read_boxhdr();
                        }
                    }
                    while (0x6a703268 != this.boxType);
                    this.Jp2_read_boxhdr();
                    if (0x69686472 != this.boxType)
                    {
                        throw new PdfUnrecognizedImageFormatException("Expected IHDR Marker");
                    }
                    base.height = this.Cio_read(4);
                    base.width = this.Cio_read(4);
                    base.bpc = -1;
                }
                else
                {
                    if (this.boxLength != -11534511)
                    {
                        throw new PdfUnrecognizedImageFormatException("Not a valid Jpeg2000 file");
                    }
                    Image.Skip(this.inp, 4);
                    int num = this.Cio_read(4);
                    int num2 = this.Cio_read(4);
                    int num3 = this.Cio_read(4);
                    int num4 = this.Cio_read(4);
                    Image.Skip(this.inp, 0x10);
                    base.colorspace = this.Cio_read(2);
                    base.bpc = 8;
                    base.height = num2 - num4;
                    base.width = num - num3;
                }
            }
            finally
            {
                if (this.inp != null)
                {
                    try
                    {
                        this.inp.Dispose();
                    }
                    catch
                    {
                    }
                    this.inp = null;
                }
            }
        }
    }
}

