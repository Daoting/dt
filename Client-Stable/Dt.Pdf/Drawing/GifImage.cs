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
using Dt.Pdf.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Gif Image
    /// </summary>
    internal class GifImage
    {
        protected int bgIndex;
        protected byte[] block;
        protected int blockSize;
        protected int delay;
        protected int dispose;
        protected List<GifFrame> frames;
        protected byte[] fromData;
        protected bool gctFlag;
        protected int height;
        protected int ih;
        protected Stream inp;
        protected bool interlace;
        protected int iw;
        protected int ix;
        protected int iy;
        protected bool lctFlag;
        protected int lctSize;
        protected int m_bpc;
        protected byte[] m_curr_table;
        protected int m_gbpc;
        protected byte[] m_global_table;
        protected int m_line_stride;
        protected byte[] m_out;
        protected const int MaxStackSize = 0x1000;
        protected int pixelAspect;
        protected byte[] pixelStack;
        protected short[] prefix;
        protected byte[] suffix;
        protected int transIndex;
        protected bool transparency;
        protected int width;

        /// <summary>
        /// Reads gif images from a byte array.
        /// </summary>
        /// <param name="data">the byte array</param>
        public GifImage(byte[] data)
        {
            this.block = new byte[0x100];
            this.frames = new List<GifFrame>();
            this.fromData = data;
            Stream stream = null;
            try
            {
                stream = new MemoryStream(data);
                this.LoadImage(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Reads gif images from a stream. The stream stream not closed.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public GifImage(Stream stream)
        {
            this.block = new byte[0x100];
            this.frames = new List<GifFrame>();
            this.LoadImage(stream);
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
            return (((c1 == 0x47) && (c2 == 0x49)) && (c3 == 70));
        }

        /// <summary>
        /// Decodes the image data.
        /// </summary>
        /// <returns></returns>
        protected bool DecodeImageData()
        {
            int num2;
            int num4;
            int num6;
            int num7;
            int num8;
            int num = this.iw * this.ih;
            bool flag = false;
            if (this.prefix == null)
            {
                this.prefix = new short[0x1000];
            }
            if (this.suffix == null)
            {
                this.suffix = new byte[0x1000];
            }
            if (this.pixelStack == null)
            {
                this.pixelStack = new byte[0x1001];
            }
            this.m_line_stride = ((this.iw * this.m_bpc) + 7) / 8;
            this.m_out = new byte[this.m_line_stride * this.ih];
            int num9 = 1;
            int num10 = this.interlace ? 8 : 1;
            int y = 0;
            int x = 0;
            int num13 = this.inp.ReadByte();
            int num14 = ((int) 1) << num13;
            int num15 = num14 + 1;
            int index = num14 + 2;
            int num17 = -1;
            int num18 = num13 + 1;
            int num19 = (((int) 1) << num18) - 1;
            int num3 = 0;
            while (num3 < num14)
            {
                this.prefix[num3] = 0;
                this.suffix[num3] = (byte) num3;
                num3++;
            }
            int num20 = num2 = num4 = num6 = num7 = num8 = 0;
            int num5 = 0;
            while (num5 < num)
            {
                if (num7 == 0)
                {
                    if (num2 < num18)
                    {
                        if (num4 == 0)
                        {
                            num4 = this.ReadBlock();
                            if (num4 <= 0)
                            {
                                return true;
                            }
                            num8 = 0;
                        }
                        num20 += (this.block[num8] & 0xff) << num2;
                        num2 += 8;
                        num8++;
                        num4--;
                        continue;
                    }
                    num3 = num20 & num19;
                    num20 = ((int) num20) >> num18;
                    num2 -= num18;
                    if ((num3 > index) || (num3 == num15))
                    {
                        return flag;
                    }
                    if (num3 == num14)
                    {
                        num18 = num13 + 1;
                        num19 = (((int) 1) << num18) - 1;
                        index = num14 + 2;
                        num17 = -1;
                        continue;
                    }
                    if (num17 == -1)
                    {
                        this.pixelStack[num7++] = this.suffix[num3];
                        num17 = num3;
                        num6 = num3;
                        continue;
                    }
                    int num21 = num3;
                    if (num3 == index)
                    {
                        this.pixelStack[num7++] = (byte) num6;
                        num3 = num17;
                    }
                    while (num3 > num14)
                    {
                        this.pixelStack[num7++] = this.suffix[num3];
                        num3 = this.prefix[num3];
                    }
                    num6 = this.suffix[num3] & 0xff;
                    if (index >= 0x1000)
                    {
                        return flag;
                    }
                    this.pixelStack[num7++] = (byte) num6;
                    this.prefix[index] = (short) num17;
                    this.suffix[index] = (byte) num6;
                    index++;
                    if (((index & num19) == 0) && (index < 0x1000))
                    {
                        num18++;
                        num19 += index;
                    }
                    num17 = num21;
                }
                num7--;
                num5++;
                this.SetPixel(x, y, this.pixelStack[num7]);
                x++;
                if (x >= this.iw)
                {
                    x = 0;
                    y += num10;
                    if (y >= this.ih)
                    {
                        if (this.interlace)
                        {
                            do
                            {
                                num9++;
                                switch (num9)
                                {
                                    case 2:
                                        y = 4;
                                        break;

                                    case 3:
                                        y = 2;
                                        num10 = 4;
                                        break;

                                    case 4:
                                        y = 1;
                                        num10 = 2;
                                        break;

                                    default:
                                        y = this.ih - 1;
                                        num10 = 0;
                                        break;
                                }
                            }
                            while (y >= this.ih);
                            continue;
                        }
                        y = this.ih - 1;
                        num10 = 0;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Gets the number of frames the gif has.
        /// </summary>
        /// <returns></returns>
        public int GetFrameCount()
        {
            return this.frames.Count;
        }

        /// <summary>
        /// Gets the [x,y] position of the frame in reference to the
        /// logical screen.
        /// </summary>
        /// <param name="frame">the frame</param>
        /// <returns>the [x,y] position of the frame</returns>
        public int[] GetFramePosition(int frame)
        {
            GifFrame frame2 = this.frames[frame - 1];
            return new int[] { frame2.X, frame2.Y };
        }

        /// <summary>
        /// Gets the image from a frame. The first frame isp 1.
        /// </summary>
        /// <param name="frame">the frame to get the image from</param>
        /// <returns>the image</returns>
        public Image GetImage(int frame)
        {
            GifFrame frame2 = this.frames[frame - 1];
            return frame2.Image;
        }

        /// <summary>
        /// Gets the logical screen. The images may be smaller and placed
        /// in some position in this screen to playback some animation.
        /// No image will be be bigger that this.
        /// </summary>
        /// <returns>the logical screen dimensions as [x,y]</returns>
        public int[] GetLogicalScreen()
        {
            return new int[] { this.width, this.height };
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void LoadImage(Stream stream)
        {
            this.inp = stream;
            this.ReadHeader();
            this.ReadContents();
            if (this.frames.Count == 0)
            {
                throw new IOException("The file does not contain any valid image.");
            }
        }

        /// <summary>
        /// News the BPC.
        /// </summary>
        /// <param name="bpc">The BPC.</param>
        /// <returns></returns>
        protected static int NewBpc(int bpc)
        {
            switch (bpc)
            {
                case 1:
                case 2:
                case 4:
                    return bpc;

                case 3:
                    return 4;
            }
            return 8;
        }

        /// <summary>
        /// Reads next variable length block from input.
        /// </summary>
        /// <returns>number of bytes stored in "buffer"</returns>
        protected int ReadBlock()
        {
            this.blockSize = this.inp.ReadByte();
            if (this.blockSize <= 0)
            {
                return (this.blockSize = 0);
            }
            for (int i = 0; i < this.blockSize; i++)
            {
                int num2 = this.inp.ReadByte();
                if (num2 < 0)
                {
                    return (this.blockSize = i);
                }
                this.block[i] = (byte) num2;
            }
            return this.blockSize;
        }

        /// <summary>
        /// Reads the color table.
        /// </summary>
        /// <param name="bpc">The BPC.</param>
        /// <returns></returns>
        protected byte[] ReadColorTable(int bpc)
        {
            int num = ((int) 1) << bpc;
            int count = 3 * num;
            bpc = NewBpc(bpc);
            byte[] b = new byte[(((int) 1) << bpc) * 3];
            this.ReadFully(b, 0, count);
            return b;
        }

        /// <summary>
        /// Reads the contents.
        /// </summary>
        protected void ReadContents()
        {
            bool flag = false;
            while (!flag)
            {
                int num2 = this.inp.ReadByte();
                if (num2 != 0x21)
                {
                    if (num2 != 0x2c)
                    {
                        goto Label_0063;
                    }
                    this.ReadImage();
                }
                else
                {
                    switch (this.inp.ReadByte())
                    {
                        case 0xf9:
                        {
                            this.ReadGraphicControlExt();
                            continue;
                        }
                        case 0xff:
                        {
                            this.ReadBlock();
                            this.Skip();
                            continue;
                        }
                    }
                    this.Skip();
                }
                continue;
            Label_0063:
                flag = true;
            }
        }

        /// <summary>
        /// Reads the fully.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        private void ReadFully(byte[] b, int offset, int count)
        {
            while (count > 0)
            {
                int num = this.inp.Read(b, offset, count);
                if (num <= 0)
                {
                    throw new IOException("Insufficient data.");
                }
                count -= num;
                offset += num;
            }
        }

        /// <summary>
        /// Reads Graphics Control Extension values
        /// </summary>
        protected void ReadGraphicControlExt()
        {
            this.inp.ReadByte();
            int num = this.inp.ReadByte();
            this.dispose = (num & 0x1c) >> 2;
            if (this.dispose == 0)
            {
                this.dispose = 1;
            }
            this.transparency = (num & 1) != 0;
            this.delay = this.ReadShort() * 10;
            this.transIndex = this.inp.ReadByte();
            this.inp.ReadByte();
        }

        /// <summary>
        /// Reads GIF file header information.
        /// </summary>
        protected void ReadHeader()
        {
            string str = "";
            for (int i = 0; i < 6; i++)
            {
                str = str + ((char) this.inp.ReadByte());
            }
            if (!str.StartsWith("GIF8"))
            {
                throw new IOException("Gif signature nor found.");
            }
            this.ReadLSD();
            if (this.gctFlag)
            {
                this.m_global_table = this.ReadColorTable(this.m_gbpc);
            }
        }

        /// <summary>
        /// Reads next frame image
        /// </summary>
        protected void ReadImage()
        {
            this.ix = this.ReadShort();
            this.iy = this.ReadShort();
            this.iw = this.ReadShort();
            this.ih = this.ReadShort();
            int num = this.inp.ReadByte();
            this.lctFlag = (num & 0x80) != 0;
            this.interlace = (num & 0x40) != 0;
            this.lctSize = ((int) 2) << (num & 7);
            this.m_bpc = NewBpc(this.m_gbpc);
            if (this.lctFlag)
            {
                this.m_curr_table = this.ReadColorTable((num & 7) + 1);
                this.m_bpc = NewBpc((num & 7) + 1);
            }
            else
            {
                this.m_curr_table = this.m_global_table;
            }
            if (this.transparency && (this.transIndex >= (this.m_curr_table.Length / 3)))
            {
                this.transparency = false;
            }
            if (this.transparency && (this.m_bpc == 1))
            {
                byte[] destinationArray = new byte[12];
                Array.Copy(this.m_curr_table, 0, destinationArray, 0, 6);
                this.m_curr_table = destinationArray;
                this.m_bpc = 2;
            }
            if (!this.DecodeImageData())
            {
                this.Skip();
            }
            Image image = new RawImage(this.iw, this.ih, 1, this.m_bpc, this.m_out);
            int length = this.m_curr_table.Length;
            PdfIndexedColorSpace space = new PdfIndexedColorSpace(PdfName.DeviceRGB, (length / 3) - 1, this.m_curr_table);
            PdfDictionary dictionary2 = new PdfDictionary();
            dictionary2.Add(PdfName.ColorSpace, space);
            PdfDictionary dictionary = dictionary2;
            image.Properties = dictionary;
            if (this.transparency)
            {
                image.Transparency = new int[] { this.transIndex, this.transIndex };
            }
            image.OriginalType = ImageType.GIF;
            image.OriginalData = this.fromData;
            GifFrame item = new GifFrame {
                Image = image,
                X = this.ix,
                Y = this.iy
            };
            this.frames.Add(item);
        }

        /// <summary>
        /// Reads Logical Screen Descriptor
        /// </summary>
        protected void ReadLSD()
        {
            this.width = this.ReadShort();
            this.height = this.ReadShort();
            int num = this.inp.ReadByte();
            this.gctFlag = (num & 0x80) != 0;
            this.m_gbpc = (num & 7) + 1;
            this.bgIndex = this.inp.ReadByte();
            this.pixelAspect = this.inp.ReadByte();
        }

        /// <summary>
        /// Reads next 16-bit value, LSB first
        /// </summary>
        /// <returns></returns>
        protected int ReadShort()
        {
            return (this.inp.ReadByte() | (this.inp.ReadByte() << 8));
        }

        /// <summary>
        /// Sets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="v">The v.</param>
        protected void SetPixel(int x, int y, int v)
        {
            if (this.m_bpc == 8)
            {
                int index = x + (this.iw * y);
                this.m_out[index] = (byte) v;
            }
            else
            {
                int num2 = (this.m_line_stride * y) + (x / (8 / this.m_bpc));
                int num3 = ((int) v) << ((8 - (this.m_bpc * (x % (8 / this.m_bpc)))) - this.m_bpc);
                this.m_out[num2] |= (byte) num3;
            }
        }

        /// <summary>
        /// Skips variable length blocks up to and including
        /// next zero length block.
        /// </summary>
        protected void Skip()
        {
            do
            {
                this.ReadBlock();
            }
            while (this.blockSize > 0);
        }

        internal class GifFrame
        {
            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            /// <value>The image.</value>
            public Image Image { get; set; }

            /// <summary>
            /// Gets or sets the X.
            /// </summary>
            /// <value>The X.</value>
            public int X { get; set; }

            /// <summary>
            /// Gets or sets the Y.
            /// </summary>
            /// <value>The Y.</value>
            public int Y { get; set; }
        }
    }
}

