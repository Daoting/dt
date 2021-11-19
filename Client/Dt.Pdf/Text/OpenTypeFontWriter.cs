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

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The open type font writer
    /// </summary>
    public class OpenTypeFontWriter : IDisposable
    {
        private readonly MemoryStream stm = new MemoryStream();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.stm.Dispose();
        }

        /// <summary>
        /// Seeks the specified pos.
        /// </summary>
        /// <param name="pos">The pos.</param>
        public void Seek(long pos)
        {
            this.stm.Seek(pos, SeekOrigin.Begin);
        }

        /// <summary>
        /// Skips the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Skip(long offset)
        {
            this.stm.Seek(offset, SeekOrigin.Current);
        }

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return this.stm.ToArray();
        }

        /// <summary>
        /// Writes the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Write(OpenTypeFontReader reader)
        {
            if (reader == null)
            {
                throw new PdfArgumentNullException("reader");
            }
            this.stm.Write(reader.ToArray(), 0, (int) reader.Length);
        }

        /// <summary>
        /// Writes the BYTE.
        /// </summary>
        /// <param name="b">The b.</param>
        public void WriteBYTE(byte b)
        {
            this.stm.WriteByte(b);
        }

        /// <summary>
        /// Writes the bytes.
        /// </summary>
        /// <param name="b">The b.</param>
        public void WriteBytes(byte[] b)
        {
            if (b == null)
            {
                throw new PdfArgumentNullException("b");
            }
            this.stm.Write(b, 0, b.Length);
        }

        /// <summary>
        /// Writes the bytes.
        /// </summary>
        /// <param name="length">The length.</param>
        public void WriteBytes(int length)
        {
            byte[] buffer = new byte[length];
            this.stm.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes the fixed.
        /// </summary>
        /// <param name="f">The f.</param>
        public void WriteFixed(float f)
        {
            this.WriteSHORT((int) f);
            this.WriteSHORT((int) ((f - ((int) f)) * 65536f));
        }

        /// <summary>
        /// Writes the LONG.
        /// </summary>
        /// <param name="l">The l.</param>
        public void WriteLONG(long l)
        {
            this.stm.WriteByte((byte) (l >> 0x18));
            this.stm.WriteByte((byte) (l >> 0x10));
            this.stm.WriteByte((byte) (l >> 8));
            this.stm.WriteByte((byte) l);
        }

        /// <summary>
        /// Writes the SHORT.
        /// </summary>
        /// <param name="i">The i.</param>
        public void WriteSHORT(int i)
        {
            this.stm.WriteByte((byte) (i >> 8));
            this.stm.WriteByte((byte) i);
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="str">The STR.</param>
        public void WriteString(string str)
        {
            if (str == null)
            {
                throw new PdfArgumentNullException("str");
            }
            this.stm.Write(PdfASCIIEncoding.Instance.GetBytes(str), 0, str.Length);
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public long Length
        {
            get { return  this.Stream.Length; }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public long Position
        {
            get { return  this.Stream.Position; }
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public MemoryStream Stream
        {
            get { return  this.stm; }
        }
    }
}

