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
using System.Globalization;
using System.IO;
#endregion

namespace Dt.Pdf
{
    /// <summary>
    /// The pdf stream writer
    /// </summary>
    internal class PdfStreamWriter
    {
        private readonly PdfEncodingBase encoding;
        private readonly System.IO.Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.PdfStreamWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        public PdfStreamWriter(System.IO.Stream stream, PdfEncodingBase encoding)
        {
            if (stream == null)
            {
                throw new PdfArgumentNullException("stream");
            }
            if (encoding == null)
            {
                throw new PdfArgumentNullException("encoding");
            }
            if (!stream.CanWrite)
            {
                throw new PdfArgumentException("stream");
            }
            this.stream = stream;
            this.encoding = encoding;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            if (!this.stream.CanSeek || !this.stream.CanRead)
            {
                throw new PdfException("stream can not read or seek.");
            }
            byte[] buffer = new byte[this.stream.Length];
            long position = this.stream.Position;
            this.stream.Seek(0L, SeekOrigin.Begin);
            this.stream.Read(buffer, 0, buffer.Length);
            if (position != this.stream.Position)
            {
                this.stream.Seek(position, SeekOrigin.Begin);
            }
            return buffer;
        }

        /// <summary>
        /// Writes the byte.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public PdfStreamWriter WriteByte(byte b)
        {
            this.stream.WriteByte(b);
            return this;
        }

        /// <summary>
        /// Writes the bytes.
        /// </summary>
        /// <param name="buf">The buf.</param>
        /// <returns></returns>
        public PdfStreamWriter WriteBytes(byte[] buf)
        {
            this.stream.Write(buf, 0, buf.Length);
            return this;
        }

        /// <summary>
        /// Writes the char.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public PdfStreamWriter WriteChar(char c)
        {
            byte[] bytes = this.encoding.GetBytes(new char[] { c });
            this.stream.Write(bytes, 0, bytes.Length);
            return this;
        }

        /// <summary>
        /// Writes the double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public PdfStreamWriter WriteDouble(double value)
        {
            return this.WriteString(((double) value).ToString("0.#########", CultureInfo.InvariantCulture.NumberFormat));
        }

        /// <summary>
        /// Writes the int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public PdfStreamWriter WriteInt(int value)
        {
            return this.WriteDouble((double) value);
        }

        /// <summary>
        /// Writes the line end.
        /// </summary>
        /// <returns></returns>
        public PdfStreamWriter WriteLineEnd()
        {
            return this.WriteByte(13).WriteByte(10);
        }

        /// <summary>
        /// Writes the space.
        /// </summary>
        /// <returns></returns>
        public PdfStreamWriter WriteSpace()
        {
            return this.WriteByte(0x20);
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public PdfStreamWriter WriteString(string str)
        {
            byte[] bytes = this.encoding.GetBytes(str);
            this.stream.Write(bytes, 0, bytes.Length);
            return this;
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public PdfEncodingBase Encoding
        {
            get { return  this.encoding; }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public long Position
        {
            get { return  this.stream.Position; }
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public System.IO.Stream Stream
        {
            get { return  this.stream; }
        }
    }
}

