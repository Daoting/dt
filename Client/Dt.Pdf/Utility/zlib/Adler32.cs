#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Pdf.Utility.zlib
{
    internal sealed class Adler32
    {
        private const int BASE = 0xfff1;
        private const int NMAX = 0x15b0;

        internal long adler32(long adler, byte[] buf, int index, int len)
        {
            if (buf == null)
            {
                return 1L;
            }
            long num = adler & 0xffffL;
            long num2 = (adler >> 0x10) & 0xffffL;
            while (len > 0)
            {
                int num3 = (len < 0x15b0) ? len : 0x15b0;
                len -= num3;
                while (num3 >= 0x10)
                {
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num += buf[index++] & 0xff;
                    num2 += num;
                    num3 -= 0x10;
                }
                if (num3 != 0)
                {
                    do
                    {
                        num += buf[index++] & 0xff;
                        num2 += num;
                    }
                    while (--num3 != 0);
                }
                num = num % 0xfff1L;
                num2 = num2 % 0xfff1L;
            }
            return ((num2 << 0x10) | num);
        }
    }
}

