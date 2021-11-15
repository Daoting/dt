#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using System;
using System.IO;
#endregion

namespace Dt.Pdf.Utility.zlib
{
    /// <summary>
    /// Summary description for DeflaterOutputStream.
    /// </summary>
    public class ZInflaterInputStream : Stream
    {
        protected byte[] buf;
        private byte[] buf1;
        private const int BUFSIZE = 0x1060;
        protected int flushLevel;
        protected Stream inp;
        private bool nomoreinput;
        protected ZStream z;

        public ZInflaterInputStream(Stream inp) : this(inp, false)
        {
        }

        public ZInflaterInputStream(Stream inp, bool nowrap)
        {
            this.z = new ZStream();
            this.buf = new byte[0x1060];
            this.buf1 = new byte[1];
            this.inp = inp;
            this.z.inflateInit(nowrap);
            this.z.next_in = this.buf;
            this.z.next_in_index = 0;
            this.z.avail_in = 0;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.inp.Dispose();
        }

        public override void Flush()
        {
            this.inp.Flush();
        }

        public override int Read(byte[] b, int off, int len)
        {
            int num;
            if (len == 0)
            {
                return 0;
            }
            this.z.next_out = b;
            this.z.next_out_index = off;
            this.z.avail_out = len;
            do
            {
                if ((this.z.avail_in == 0) && !this.nomoreinput)
                {
                    this.z.next_in_index = 0;
                    this.z.avail_in = this.inp.Read(this.buf, 0, 0x1060);
                    if (this.z.avail_in == 0)
                    {
                        this.z.avail_in = 0;
                        this.nomoreinput = true;
                    }
                }
                num = this.z.inflate(this.flushLevel);
                if (this.nomoreinput && (num == -5))
                {
                    return -1;
                }
                if ((num != 0) && (num != 1))
                {
                    throw new IOException("inflating: " + this.z.msg);
                }
                if ((this.nomoreinput || (num == 1)) && (this.z.avail_out == len))
                {
                    return 0;
                }
            }
            while ((this.z.avail_out == len) && (num == 0));
            return (len - this.z.avail_out);
        }

        public override int ReadByte()
        {
            if (this.Read(this.buf1, 0, 1) <= 0)
            {
                return -1;
            }
            return (this.buf1[0] & 0xff);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0L;
        }

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] b, int off, int len)
        {
        }

        public override void WriteByte(byte b)
        {
        }

        public override bool CanRead
        {
            get { return  true; }
        }

        public override bool CanSeek
        {
            get { return  false; }
        }

        public override bool CanWrite
        {
            get { return  false; }
        }

        public override long Length
        {
            get { return  0L; }
        }

        public override long Position
        {
            get { return  0L; }
            set
            {
            }
        }
    }
}

