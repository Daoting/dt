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
    public class ZDeflaterOutputStream : Stream
    {
        protected byte[] buf;
        private byte[] buf1;
        private const int BUFSIZE = 0x1060;
        protected int flushLevel;
        protected Stream outp;
        protected ZStream z;

        public ZDeflaterOutputStream(Stream outp) : this(outp, 6, false)
        {
        }

        public ZDeflaterOutputStream(Stream outp, int level) : this(outp, level, false)
        {
        }

        public ZDeflaterOutputStream(Stream outp, int level, bool nowrap)
        {
            this.z = new ZStream();
            this.buf = new byte[0x1060];
            this.buf1 = new byte[1];
            this.outp = outp;
            this.z.deflateInit(level, nowrap);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            {
                this.Finish();
            }
            catch (IOException)
            {
            }
            finally
            {
                this.End();
                this.outp.Dispose();
                this.outp = null;
            }
        }

        public void End()
        {
            if (this.z != null)
            {
                this.z.deflateEnd();
                this.z.free();
                this.z = null;
            }
        }

        public void Finish()
        {
            do
            {
                this.z.next_out = this.buf;
                this.z.next_out_index = 0;
                this.z.avail_out = 0x1060;
                int num = this.z.deflate(4);
                if ((num != 1) && (num != 0))
                {
                    throw new IOException("deflating: " + this.z.msg);
                }
                if ((0x1060 - this.z.avail_out) > 0)
                {
                    this.outp.Write(this.buf, 0, 0x1060 - this.z.avail_out);
                }
            }
            while ((this.z.avail_in > 0) || (this.z.avail_out == 0));
            this.Flush();
        }

        public override void Flush()
        {
            this.outp.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
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
            if (len != 0)
            {
                this.z.next_in = b;
                this.z.next_in_index = off;
                this.z.avail_in = len;
                do
                {
                    this.z.next_out = this.buf;
                    this.z.next_out_index = 0;
                    this.z.avail_out = 0x1060;
                    if (this.z.deflate(this.flushLevel) != 0)
                    {
                        throw new IOException("deflating: " + this.z.msg);
                    }
                    if (this.z.avail_out < 0x1060)
                    {
                        this.outp.Write(this.buf, 0, 0x1060 - this.z.avail_out);
                    }
                }
                while ((this.z.avail_in > 0) || (this.z.avail_out == 0));
            }
        }

        public override void WriteByte(byte b)
        {
            this.buf1[0] = b;
            this.Write(this.buf1, 0, 1);
        }

        public override bool CanRead
        {
            get { return  false; }
        }

        public override bool CanSeek
        {
            get { return  false; }
        }

        public override bool CanWrite
        {
            get { return  true; }
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

