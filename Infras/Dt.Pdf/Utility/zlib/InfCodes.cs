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
    internal sealed class InfCodes
    {
        private const int BADCODE = 9;
        private const int COPY = 5;
        private byte dbits;
        private int dist;
        private const int DIST = 3;
        private const int DISTEXT = 4;
        private int[] dtree;
        private int dtree_index;
        private const int END = 8;
        private int get;
        private static int[] inflate_mask = new int[] { 
            0, 1, 3, 7, 15, 0x1f, 0x3f, 0x7f, 0xff, 0x1ff, 0x3ff, 0x7ff, 0xfff, 0x1fff, 0x3fff, 0x7fff, 
            0xffff
         };
        private byte lbits;
        private int len;
        private const int LEN = 1;
        private const int LENEXT = 2;
        private int lit;
        private const int LIT = 6;
        private int[] ltree;
        private int ltree_index;
        private int mode;
        private int need;
        private const int START = 0;
        private int[] tree;
        private int tree_index;
        private const int WASH = 7;
        private const int Z_BUF_ERROR = -5;
        private const int Z_DATA_ERROR = -3;
        private const int Z_ERRNO = -1;
        private const int Z_MEM_ERROR = -4;
        private const int Z_NEED_DICT = 2;
        private const int Z_OK = 0;
        private const int Z_STREAM_END = 1;
        private const int Z_STREAM_ERROR = -2;
        private const int Z_VERSION_ERROR = -6;

        internal InfCodes()
        {
        }

        internal void free(ZStream z)
        {
        }

        internal int inflate_fast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InfBlocks s, ZStream z)
        {
            int num12;
            int num6 = z.next_in_index;
            int num7 = z.avail_in;
            int bitb = s.bitb;
            int bitk = s.bitk;
            int write = s.write;
            int num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
            int num10 = inflate_mask[bl];
            int num11 = inflate_mask[bd];
        Label_0092:
            while (bitk < 20)
            {
                num7--;
                bitb |= (z.next_in[num6++] & 0xff) << bitk;
                bitk += 8;
            }
            int num = bitb & num10;
            int[] numArray = tl;
            int num2 = tl_index;
            int index = (num2 + num) * 3;
            int num3 = numArray[index];
            if (num3 == 0)
            {
                bitb = ((int) bitb) >> numArray[index + 1];
                bitk -= numArray[index + 1];
                s.window[write++] = (byte) numArray[index + 2];
                num9--;
                goto Label_05C0;
            }
        Label_00EC:
            bitb = ((int) bitb) >> numArray[index + 1];
            bitk -= numArray[index + 1];
            if ((num3 & 0x10) == 0)
            {
                if ((num3 & 0x40) == 0)
                {
                    num += numArray[index + 2];
                    num += bitb & inflate_mask[num3];
                    index = (num2 + num) * 3;
                    num3 = numArray[index];
                    if (num3 != 0)
                    {
                        goto Label_00EC;
                    }
                    bitb = ((int) bitb) >> numArray[index + 1];
                    bitk -= numArray[index + 1];
                    s.window[write++] = (byte) numArray[index + 2];
                    num9--;
                }
                else
                {
                    if ((num3 & 0x20) != 0)
                    {
                        num12 = z.avail_in - num7;
                        num12 = ((bitk >> 3) < num12) ? (bitk >> 3) : num12;
                        num7 += num12;
                        num6 -= num12;
                        bitk -= num12 << 3;
                        s.bitb = bitb;
                        s.bitk = bitk;
                        z.avail_in = num7;
                        z.total_in += num6 - z.next_in_index;
                        z.next_in_index = num6;
                        s.write = write;
                        return 1;
                    }
                    z.msg = "invalid literal/length code";
                    num12 = z.avail_in - num7;
                    num12 = ((bitk >> 3) < num12) ? (bitk >> 3) : num12;
                    num7 += num12;
                    num6 -= num12;
                    bitk -= num12 << 3;
                    s.bitb = bitb;
                    s.bitk = bitk;
                    z.avail_in = num7;
                    z.total_in += num6 - z.next_in_index;
                    z.next_in_index = num6;
                    s.write = write;
                    return -3;
                }
                goto Label_05C0;
            }
            num3 &= 15;
            num12 = numArray[index + 2] + (bitb & inflate_mask[num3]);
            bitb = ((int) bitb) >> num3;
            bitk -= num3;
            while (bitk < 15)
            {
                num7--;
                bitb |= (z.next_in[num6++] & 0xff) << bitk;
                bitk += 8;
            }
            num = bitb & num11;
            numArray = td;
            num2 = td_index;
            index = (num2 + num) * 3;
            num3 = numArray[index];
        Label_0181:
            bitb = ((int) bitb) >> numArray[index + 1];
            bitk -= numArray[index + 1];
            if ((num3 & 0x10) != 0)
            {
                int num14;
                num3 &= 15;
                while (bitk < num3)
                {
                    num7--;
                    bitb |= (z.next_in[num6++] & 0xff) << bitk;
                    bitk += 8;
                }
                int num13 = numArray[index + 2] + (bitb & inflate_mask[num3]);
                bitb = ((int) bitb) >> num3;
                bitk -= num3;
                num9 -= num12;
                if (write >= num13)
                {
                    num14 = write - num13;
                    if (((write - num14) > 0) && (2 > (write - num14)))
                    {
                        s.window[write++] = s.window[num14++];
                        s.window[write++] = s.window[num14++];
                        num12 -= 2;
                    }
                    else
                    {
                        Array.Copy(s.window, num14, s.window, write, 2);
                        write += 2;
                        num14 += 2;
                        num12 -= 2;
                    }
                }
                else
                {
                    num14 = write - num13;
                    do
                    {
                        num14 += s.end;
                    }
                    while (num14 < 0);
                    num3 = s.end - num14;
                    if (num12 > num3)
                    {
                        num12 -= num3;
                        if (((write - num14) > 0) && (num3 > (write - num14)))
                        {
                            do
                            {
                                s.window[write++] = s.window[num14++];
                            }
                            while (--num3 != 0);
                        }
                        else
                        {
                            Array.Copy(s.window, num14, s.window, write, num3);
                            write += num3;
                            num14 += num3;
                            num3 = 0;
                        }
                        num14 = 0;
                    }
                }
                if (((write - num14) > 0) && (num12 > (write - num14)))
                {
                    do
                    {
                        s.window[write++] = s.window[num14++];
                    }
                    while (--num12 != 0);
                }
                else
                {
                    Array.Copy(s.window, num14, s.window, write, num12);
                    write += num12;
                    num14 += num12;
                    num12 = 0;
                }
            }
            else
            {
                if ((num3 & 0x40) == 0)
                {
                    num += numArray[index + 2];
                    num += bitb & inflate_mask[num3];
                    index = (num2 + num) * 3;
                    num3 = numArray[index];
                    goto Label_0181;
                }
                z.msg = "invalid distance code";
                num12 = z.avail_in - num7;
                num12 = ((bitk >> 3) < num12) ? (bitk >> 3) : num12;
                num7 += num12;
                num6 -= num12;
                bitk -= num12 << 3;
                s.bitb = bitb;
                s.bitk = bitk;
                z.avail_in = num7;
                z.total_in += num6 - z.next_in_index;
                z.next_in_index = num6;
                s.write = write;
                return -3;
            }
        Label_05C0:
            if ((num9 >= 0x102) && (num7 >= 10))
            {
                goto Label_0092;
            }
            num12 = z.avail_in - num7;
            num12 = ((bitk >> 3) < num12) ? (bitk >> 3) : num12;
            num7 += num12;
            num6 -= num12;
            bitk -= num12 << 3;
            s.bitb = bitb;
            s.bitk = bitk;
            z.avail_in = num7;
            z.total_in += num6 - z.next_in_index;
            z.next_in_index = num6;
            s.write = write;
            return 0;
        }

        internal void init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, ZStream z)
        {
            this.mode = 0;
            this.lbits = (byte) bl;
            this.dbits = (byte) bd;
            this.ltree = tl;
            this.ltree_index = tl_index;
            this.dtree = td;
            this.dtree_index = td_index;
            this.tree = null;
        }

        internal int proc(InfBlocks s, ZStream z, int r)
        {
            int get;
            int num10;
            int bitb = 0;
            int bitk = 0;
            int num6 = 0;
            num6 = z.next_in_index;
            int num7 = z.avail_in;
            bitb = s.bitb;
            bitk = s.bitk;
            int write = s.write;
            int num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
        Label_0051:
            switch (this.mode)
            {
                case 0:
                    if ((num9 < 0x102) || (num7 < 10))
                    {
                        break;
                    }
                    s.bitb = bitb;
                    s.bitk = bitk;
                    z.avail_in = num7;
                    z.total_in += num6 - z.next_in_index;
                    z.next_in_index = num6;
                    s.write = write;
                    r = this.inflate_fast(this.lbits, this.dbits, this.ltree, this.ltree_index, this.dtree, this.dtree_index, s, z);
                    num6 = z.next_in_index;
                    num7 = z.avail_in;
                    bitb = s.bitb;
                    bitk = s.bitk;
                    write = s.write;
                    num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                    if (r == 0)
                    {
                        break;
                    }
                    this.mode = (r == 1) ? 7 : 9;
                    goto Label_0051;

                case 1:
                    goto Label_0199;

                case 2:
                    get = this.get;
                    while (bitk < get)
                    {
                        if (num7 != 0)
                        {
                            r = 0;
                        }
                        else
                        {
                            s.bitb = bitb;
                            s.bitk = bitk;
                            z.avail_in = num7;
                            z.total_in += num6 - z.next_in_index;
                            z.next_in_index = num6;
                            s.write = write;
                            return s.inflate_flush(z, r);
                        }
                        num7--;
                        bitb |= (z.next_in[num6++] & 0xff) << bitk;
                        bitk += 8;
                    }
                    this.len += bitb & inflate_mask[get];
                    bitb = ((int) bitb) >> get;
                    bitk -= get;
                    this.need = this.dbits;
                    this.tree = this.dtree;
                    this.tree_index = this.dtree_index;
                    this.mode = 3;
                    goto Label_0411;

                case 3:
                    goto Label_0411;

                case 4:
                    get = this.get;
                    while (bitk < get)
                    {
                        if (num7 != 0)
                        {
                            r = 0;
                        }
                        else
                        {
                            s.bitb = bitb;
                            s.bitk = bitk;
                            z.avail_in = num7;
                            z.total_in += num6 - z.next_in_index;
                            z.next_in_index = num6;
                            s.write = write;
                            return s.inflate_flush(z, r);
                        }
                        num7--;
                        bitb |= (z.next_in[num6++] & 0xff) << bitk;
                        bitk += 8;
                    }
                    this.dist += bitb & inflate_mask[get];
                    bitb = ((int) bitb) >> get;
                    bitk -= get;
                    this.mode = 5;
                    goto Label_0634;

                case 5:
                    goto Label_0634;

                case 6:
                    if (num9 == 0)
                    {
                        if ((write == s.end) && (s.read != 0))
                        {
                            write = 0;
                            num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                        }
                        if (num9 == 0)
                        {
                            s.write = write;
                            r = s.inflate_flush(z, r);
                            write = s.write;
                            num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                            if ((write == s.end) && (s.read != 0))
                            {
                                write = 0;
                                num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                            }
                            if (num9 == 0)
                            {
                                s.bitb = bitb;
                                s.bitk = bitk;
                                z.avail_in = num7;
                                z.total_in += num6 - z.next_in_index;
                                z.next_in_index = num6;
                                s.write = write;
                                return s.inflate_flush(z, r);
                            }
                        }
                    }
                    r = 0;
                    s.window[write++] = (byte) this.lit;
                    num9--;
                    this.mode = 0;
                    goto Label_0051;

                case 7:
                    if (bitk > 7)
                    {
                        bitk -= 8;
                        num7++;
                        num6--;
                    }
                    s.write = write;
                    r = s.inflate_flush(z, r);
                    write = s.write;
                    num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                    if (s.read != s.write)
                    {
                        s.bitb = bitb;
                        s.bitk = bitk;
                        z.avail_in = num7;
                        z.total_in += num6 - z.next_in_index;
                        z.next_in_index = num6;
                        s.write = write;
                        return s.inflate_flush(z, r);
                    }
                    this.mode = 8;
                    goto Label_0989;

                case 8:
                    goto Label_0989;

                case 9:
                    r = -3;
                    s.bitb = bitb;
                    s.bitk = bitk;
                    z.avail_in = num7;
                    z.total_in += num6 - z.next_in_index;
                    z.next_in_index = num6;
                    s.write = write;
                    return s.inflate_flush(z, r);

                default:
                    r = -2;
                    s.bitb = bitb;
                    s.bitk = bitk;
                    z.avail_in = num7;
                    z.total_in += num6 - z.next_in_index;
                    z.next_in_index = num6;
                    s.write = write;
                    return s.inflate_flush(z, r);
            }
            this.need = this.lbits;
            this.tree = this.ltree;
            this.tree_index = this.ltree_index;
            this.mode = 1;
        Label_0199:
            get = this.need;
            while (bitk < get)
            {
                if (num7 != 0)
                {
                    r = 0;
                }
                else
                {
                    s.bitb = bitb;
                    s.bitk = bitk;
                    z.avail_in = num7;
                    z.total_in += num6 - z.next_in_index;
                    z.next_in_index = num6;
                    s.write = write;
                    return s.inflate_flush(z, r);
                }
                num7--;
                bitb |= (z.next_in[num6++] & 0xff) << bitk;
                bitk += 8;
            }
            int index = (this.tree_index + (bitb & inflate_mask[get])) * 3;
            bitb = ((int) bitb) >> this.tree[index + 1];
            bitk -= this.tree[index + 1];
            int num3 = this.tree[index];
            if (num3 == 0)
            {
                this.lit = this.tree[index + 2];
                this.mode = 6;
                goto Label_0051;
            }
            if ((num3 & 0x10) != 0)
            {
                this.get = num3 & 15;
                this.len = this.tree[index + 2];
                this.mode = 2;
                goto Label_0051;
            }
            if ((num3 & 0x40) == 0)
            {
                this.need = num3;
                this.tree_index = (index / 3) + this.tree[index + 2];
                goto Label_0051;
            }
            if ((num3 & 0x20) != 0)
            {
                this.mode = 7;
                goto Label_0051;
            }
            this.mode = 9;
            z.msg = "invalid literal/length code";
            r = -3;
            s.bitb = bitb;
            s.bitk = bitk;
            z.avail_in = num7;
            z.total_in += num6 - z.next_in_index;
            z.next_in_index = num6;
            s.write = write;
            return s.inflate_flush(z, r);
        Label_0411:
            get = this.need;
            while (bitk < get)
            {
                if (num7 != 0)
                {
                    r = 0;
                }
                else
                {
                    s.bitb = bitb;
                    s.bitk = bitk;
                    z.avail_in = num7;
                    z.total_in += num6 - z.next_in_index;
                    z.next_in_index = num6;
                    s.write = write;
                    return s.inflate_flush(z, r);
                }
                num7--;
                bitb |= (z.next_in[num6++] & 0xff) << bitk;
                bitk += 8;
            }
            index = (this.tree_index + (bitb & inflate_mask[get])) * 3;
            bitb = ((int) bitb) >> this.tree[index + 1];
            bitk -= this.tree[index + 1];
            num3 = this.tree[index];
            if ((num3 & 0x10) != 0)
            {
                this.get = num3 & 15;
                this.dist = this.tree[index + 2];
                this.mode = 4;
                goto Label_0051;
            }
            if ((num3 & 0x40) == 0)
            {
                this.need = num3;
                this.tree_index = (index / 3) + this.tree[index + 2];
                goto Label_0051;
            }
            this.mode = 9;
            z.msg = "invalid distance code";
            r = -3;
            s.bitb = bitb;
            s.bitk = bitk;
            z.avail_in = num7;
            z.total_in += num6 - z.next_in_index;
            z.next_in_index = num6;
            s.write = write;
            return s.inflate_flush(z, r);
        Label_0634:
            num10 = write - this.dist;
            while (num10 < 0)
            {
                num10 += s.end;
            }
            while (this.len != 0)
            {
                if (num9 == 0)
                {
                    if ((write == s.end) && (s.read != 0))
                    {
                        write = 0;
                        num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                    }
                    if (num9 == 0)
                    {
                        s.write = write;
                        r = s.inflate_flush(z, r);
                        write = s.write;
                        num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                        if ((write == s.end) && (s.read != 0))
                        {
                            write = 0;
                            num9 = (write < s.read) ? ((s.read - write) - 1) : (s.end - write);
                        }
                        if (num9 == 0)
                        {
                            s.bitb = bitb;
                            s.bitk = bitk;
                            z.avail_in = num7;
                            z.total_in += num6 - z.next_in_index;
                            z.next_in_index = num6;
                            s.write = write;
                            return s.inflate_flush(z, r);
                        }
                    }
                }
                s.window[write++] = s.window[num10++];
                num9--;
                if (num10 == s.end)
                {
                    num10 = 0;
                }
                this.len--;
            }
            this.mode = 0;
            goto Label_0051;
        Label_0989:
            r = 1;
            s.bitb = bitb;
            s.bitk = bitk;
            z.avail_in = num7;
            z.total_in += num6 - z.next_in_index;
            z.next_in_index = num6;
            s.write = write;
            return s.inflate_flush(z, r);
        }
    }
}

