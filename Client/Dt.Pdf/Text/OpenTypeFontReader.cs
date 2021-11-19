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
using System.Text;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The open type font reader
    /// </summary>
    public class OpenTypeFontReader : IDisposable
    {
        private readonly MemoryStream stm;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OpenTypeFontReader" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public OpenTypeFontReader(byte[] data)
        {
            if (data == null)
            {
                throw new PdfArgumentNullException("data");
            }
            this.stm = new MemoryStream(data);
        }

        public OpenTypeFontReader(MemoryStream stm)
        {
            if (stm == null)
            {
                throw new PdfArgumentNullException("stm");
            }
            this.stm = stm;
        }

        /// <summary>
        /// Clones the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public OpenTypeFontReader Clone(long offset, long length)
        {
            if (offset < 0L)
            {
                throw new PdfArgumentException("offset");
            }
            if (length < 0L)
            {
                throw new PdfArgumentException("length");
            }
            if ((offset + length) > this.stm.Length)
            {
                throw new PdfArgumentException("out of range");
            }
            long position = this.Position;
            byte[] buffer = new byte[length];
            this.Seek(offset);
            this.stm.Read(buffer, 0, buffer.Length);
            this.Seek(position);
            return new OpenTypeFontReader(buffer);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.stm != null)
            {
                this.stm.Dispose();
            }
        }

        /// <summary>
        /// Gets the table reader.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public OpenTypeFontReader GetTableReader(string id, long offset, long length)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new PdfArgumentNullException("id");
            }
            if (length < 0L)
            {
                throw new PdfArgumentException("length");
            }
            if (this.stm is IFontTableStream)
            {
                return new OpenTypeFontReader(((IFontTableStream) this.stm).GetFontTable(id, length));
            }
            return this.Clone(offset, length);
        }

        /// <summary>
        /// Reads the big endian unicode string.
        /// </summary>
        /// <param name="len">The len.</param>
        /// <returns></returns>
        public string ReadBigEndianUnicodeString(int len)
        {
            StringBuilder builder = new StringBuilder();
            len /= 2;
            for (int i = 0; i < len; i++)
            {
                builder.Append(this.ReadChar());
            }
            return builder.ToString();
        }

        /// <summary>
        /// Reads the BYTE.
        /// </summary>
        /// <returns></returns>
        public byte ReadBYTE()
        {
            return (byte) this.stm.ReadByte();
        }

        /// <summary>
        /// Reads the bytes.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            this.stm.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// Reads the char.
        /// </summary>
        /// <returns></returns>
        public char ReadChar()
        {
            byte num = this.ReadBYTE();
            byte num2 = this.ReadBYTE();
            return (char) ((num << 8) + num2);
        }

        /// <summary>
        /// Reads the CHAR.
        /// </summary>
        /// <returns></returns>
        public sbyte ReadCHAR()
        {
            return (sbyte) this.ReadBYTE();
        }

        /// <summary>
        /// Reads the fixed.
        /// </summary>
        /// <returns></returns>
        public float ReadFixed()
        {
            short num = this.ReadSHORT();
            float num2 = ((float) this.ReadSHORT()) / 65536f;
            return (num + num2);
        }

        /// <summary>
        /// Reads the FWORD.
        /// </summary>
        /// <returns></returns>
        public short ReadFWORD()
        {
            return this.ReadSHORT();
        }

        /// <summary>
        /// Reads the glyph ID.
        /// </summary>
        /// <returns></returns>
        public int ReadGlyphID()
        {
            return this.ReadUSHORT();
        }

        /// <summary>
        /// Reads the LONG.
        /// </summary>
        /// <returns></returns>
        public int ReadLONG()
        {
            byte num = this.ReadBYTE();
            byte num2 = this.ReadBYTE();
            byte num3 = this.ReadBYTE();
            byte num4 = this.ReadBYTE();
            return ((((num << 0x18) | (num2 << 0x10)) | (num3 << 8)) | num4);
        }

        /// <summary>
        /// Reads the offset.
        /// </summary>
        /// <returns></returns>
        public int ReadOffset()
        {
            return this.ReadUSHORT();
        }

        /// <summary>
        /// Reads the SHORT.
        /// </summary>
        /// <returns></returns>
        public short ReadSHORT()
        {
            byte num = this.ReadBYTE();
            byte num2 = this.ReadBYTE();
            return (short) ((num << 8) | num2);
        }

        /// <summary>
        /// Reads the standard string.
        /// </summary>
        /// <param name="len">The len.</param>
        /// <returns></returns>
        public string ReadStandardString(int len)
        {
            byte[] buffer = new byte[len];
            this.stm.Read(buffer, 0, buffer.Length);
            return PdfLatin1Encoding.Instance.GetString(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Reads the tag.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadTag()
        {
            return new byte[] { this.ReadBYTE(), this.ReadBYTE(), this.ReadBYTE(), this.ReadBYTE() };
        }

        /// <summary>
        /// Reads the tag string.
        /// </summary>
        /// <returns></returns>
        public string ReadTagString()
        {
            byte[] bytes = this.ReadTag();
            return PdfASCIIEncoding.Instance.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Reads the UFWORD.
        /// </summary>
        /// <returns></returns>
        public int ReadUFWORD()
        {
            return this.ReadUSHORT();
        }

        /// <summary>
        /// Reads the UIN T24.
        /// </summary>
        /// <returns></returns>
        public int ReadUINT24()
        {
            byte num = this.ReadBYTE();
            byte num2 = this.ReadBYTE();
            byte num3 = this.ReadBYTE();
            return (((num << 0x10) | (num2 << 8)) | num3);
        }

        /// <summary>
        /// Reads the ULONG.
        /// </summary>
        /// <returns></returns>
        public long ReadULONG()
        {
            byte num = this.ReadBYTE();
            byte num2 = this.ReadBYTE();
            byte num3 = this.ReadBYTE();
            byte num4 = this.ReadBYTE();
            return (long) ((((num << 0x18) | (num2 << 0x10)) | (num3 << 8)) | num4);
        }

        /// <summary>
        /// Reads the USHORT.
        /// </summary>
        /// <returns></returns>
        public int ReadUSHORT()
        {
            byte num = this.ReadBYTE();
            byte num2 = this.ReadBYTE();
            return ((num << 8) | num2);
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

