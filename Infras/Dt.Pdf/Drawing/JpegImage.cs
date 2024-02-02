#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using Dt.Pdf.Text;
using System;
using System.IO;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Jpeg Image
    /// </summary>
    internal class JpegImage : Image
    {
        private static readonly byte[] JFIF_ID = new byte[] { 0x4a, 70, 0x49, 70, 0 };
        private const int M_APP0 = 0xe0;
        private const int M_APPE = 0xee;
        private const int NOPARAM_MARKER = 2;
        private static readonly int[] NOPARAM_MARKERS = new int[] { 0xd0, 0xd1, 210, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 1 };
        private const int NOT_A_MARKER = -1;
        private const int UNSUPPORTED_MARKER = 1;
        private static readonly int[] UNSUPPORTED_MARKERS = new int[] { 0xc3, 0xc5, 0xc6, 0xc7, 200, 0xc9, 0xca, 0xcb, 0xcd, 0xce, 0xcf };
        private const int VALID_MARKER = 0;
        private static readonly int[] VALID_MARKERS = new int[] { 0xc0, 0xc1, 0xc2 };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:JpegImage" /> class.
        /// </summary>
        public JpegImage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:JpegImage" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public JpegImage(byte[] data)
        {
            if ((data == null) || (data.Length <= 0))
            {
                throw new ArgumentNullException("data");
            }
            base.rawData = new byte[data.Length];
            data.CopyTo(base.rawData, 0);
            this.LoadImage(new MemoryStream(data));
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
            return ((c1 == 0xff) && (c2 == 0xd8));
        }

        /// <summary>
        /// Reads the short value from the stream.
        /// </summary>
        /// <param name="istr">Image stream</param>
        private static int GetShort(Stream istr)
        {
            return ((istr.ReadByte() << 8) + istr.ReadByte());
        }

        /// <summary>
        /// Gets the image parameters such as width, height, and colorspace.
        /// </summary>
        /// <param name="istr">Input image stream</param>
        /// <returns>ImageParameters structure contains image details</returns>
        protected void LoadImage(Stream istr)
        {
            base.type = ImageType.JPEG;
            base.bpc = 8;
            bool flag = true;
            while (true)
            {
                int @short;
                while (istr.ReadByte() != 0xff)
                {
                }
                int marker = istr.ReadByte();
                if (!flag || (marker != 0xe0))
                {
                    if (marker == 0xee)
                    {
                        @short = GetShort(istr);
                        byte[] bytes = new byte[@short];
                        for (int i = 0; i < @short; i++)
                        {
                            bytes[i] = (byte) istr.ReadByte();
                        }
                        if (bytes.Length > 12)
                        {
                            if (PdfASCIIEncoding.Instance.GetString(bytes, 0, 5).ToLower() != "adobe")
                            {
                                throw new PdfUnrecognizedImageFormatException("Not a supported Jpeg File.");
                            }
                            do
                            {
                                if (istr.ReadByte() == 0xff)
                                {
                                    marker = istr.ReadByte();
                                }
                            }
                            while (MarkerType(marker) != 0);
                        }
                    }
                    flag = false;
                    int num6 = MarkerType(marker);
                    switch (num6)
                    {
                        case 0:
                            Image.Skip(istr, 2);
                            if (istr.ReadByte() != 8)
                            {
                                throw new PdfUnrecognizedImageFormatException("Must have 8 bits per component.");
                            }
                            base.height = GetShort(istr);
                            base.width = GetShort(istr);
                            base.colorspace = (byte) istr.ReadByte();
                            return;

                        case 1:
                            throw new PdfUnrecognizedImageFormatException("Unsupported JPEG marker: " + ((int) marker));
                    }
                    if (num6 != 2)
                    {
                        Image.Skip(istr, GetShort(istr) - 2);
                    }
                }
                else
                {
                    flag = false;
                    @short = GetShort(istr);
                    if (@short < 0x10)
                    {
                        Image.Skip(istr, @short - 2);
                    }
                    else
                    {
                        byte[] buffer = new byte[JFIF_ID.Length];
                        if (istr.Read(buffer, 0, buffer.Length) != buffer.Length)
                        {
                            throw new PdfUnrecognizedImageFormatException("Corrupted JFIF marker.");
                        }
                        bool flag2 = true;
                        for (int j = 0; j < buffer.Length; j++)
                        {
                            if (buffer[j] != JFIF_ID[j])
                            {
                                flag2 = false;
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            Image.Skip(istr, (@short - 2) - buffer.Length);
                        }
                        else
                        {
                            Image.Skip(istr, 2);
                            Image.Skip(istr, ((@short - 2) - buffer.Length) - 7);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Markers the type.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <returns></returns>
        private static int MarkerType(int marker)
        {
            for (int i = 0; i < VALID_MARKERS.Length; i++)
            {
                if (marker == VALID_MARKERS[i])
                {
                    return 0;
                }
            }
            for (int j = 0; j < NOPARAM_MARKERS.Length; j++)
            {
                if (marker == NOPARAM_MARKERS[j])
                {
                    return 2;
                }
            }
            for (int k = 0; k < UNSUPPORTED_MARKERS.Length; k++)
            {
                if (marker == UNSUPPORTED_MARKERS[k])
                {
                    return 1;
                }
            }
            return -1;
        }
    }
}

