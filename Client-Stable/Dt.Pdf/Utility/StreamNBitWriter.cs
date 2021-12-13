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
using System.IO;
#endregion

namespace Dt.Pdf.Utility
{
    /// <summary>
    /// An n-bit writer for stream.
    /// </summary>
    internal class StreamNBitWriter
    {
        private const int byteBits = 7;
        private byte curByte;
        private int n;
        private int posInCurByte;
        private readonly Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Utility.StreamNBitWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="n">The n.</param>
        public StreamNBitWriter(Stream stream, int n)
        {
            this.stream = stream;
            this.n = n;
            this.curByte = 0;
            this.posInCurByte = 7;
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            if (this.posInCurByte < 7)
            {
                this.stream.WriteByte(this.curByte);
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte[] data)
        {
            if (data == null)
            {
                throw new PdfArgumentNullException("data");
            }
            for (int i = 0; i < data.Length; i++)
            {
                this.Write(data[i]);
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(int[] data)
        {
            if (data == null)
            {
                throw new PdfArgumentNullException("data");
            }
            for (int i = 0; i < data.Length; i++)
            {
                this.Write(data[i]);
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(int data)
        {
            for (int i = this.n - 1; i >= 0; i--)
            {
                byte num2 = (byte)((((int) data) >> i) & 1);
                num2 = (byte)(num2 << this.posInCurByte);
                this.curByte |= num2;
                this.posInCurByte--;
                if (this.posInCurByte < 0)
                {
                    this.stream.WriteByte(this.curByte);
                    this.curByte = 0;
                    this.posInCurByte = 7;
                }
            }
        }

        /// <summary>
        /// Gets or sets the N.
        /// </summary>
        /// <value>The N.</value>
        public int N
        {
            get { return  this.n; }
            set { this.n = value; }
        }
    }
}

