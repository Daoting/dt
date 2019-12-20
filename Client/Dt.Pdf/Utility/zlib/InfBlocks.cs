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
    internal sealed class InfBlocks
    {
        private const int BAD = 9;
        internal int[] bb = new int[1];
        internal int bitb;
        internal int bitk;
        internal int[] blens;
        private static int[] border = new int[] { 
            0x10, 0x11, 0x12, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 
            14, 1, 15
         };
        private const int BTREE = 4;
        internal long check;
        internal object checkfn;
        internal InfCodes codes = new InfCodes();
        private const int CODES = 6;
        private const int DONE = 8;
        private const int DRY = 7;
        private const int DTREE = 5;
        internal int end;
        internal int[] hufts = new int[0x10e0];
        internal int index;
        private static int[] inflate_mask = new int[] { 
            0, 1, 3, 7, 15, 0x1f, 0x3f, 0x7f, 0xff, 0x1ff, 0x3ff, 0x7ff, 0xfff, 0x1fff, 0x3fff, 0x7fff, 
            0xffff
         };
        internal InfTree inftree = new InfTree();
        private int last;
        internal int left;
        private const int LENS = 1;
        private const int MANY = 0x5a0;
        internal int mode;
        internal int read;
        private const int STORED = 2;
        internal int table;
        private const int TABLE = 3;
        internal int[] tb = new int[1];
        private const int TYPE = 0;
        internal byte[] window;
        internal int write;
        private const int Z_BUF_ERROR = -5;
        private const int Z_DATA_ERROR = -3;
        private const int Z_ERRNO = -1;
        private const int Z_MEM_ERROR = -4;
        private const int Z_NEED_DICT = 2;
        private const int Z_OK = 0;
        private const int Z_STREAM_END = 1;
        private const int Z_STREAM_ERROR = -2;
        private const int Z_VERSION_ERROR = -6;

        internal InfBlocks(ZStream z, object checkfn, int w)
        {
            this.window = new byte[w];
            this.end = w;
            this.checkfn = checkfn;
            this.mode = 0;
            this.reset(z, null);
        }

        internal void free(ZStream z)
        {
            this.reset(z, null);
            this.window = null;
            this.hufts = null;
        }

        internal int inflate_flush(ZStream z, int r)
        {
            int destinationIndex = z.next_out_index;
            int read = this.read;
            int len = ((read <= this.write) ? this.write : this.end) - read;
            if (len > z.avail_out)
            {
                len = z.avail_out;
            }
            if ((len != 0) && (r == -5))
            {
                r = 0;
            }
            z.avail_out -= len;
            z.total_out += len;
            if (this.checkfn != null)
            {
                z.adler = this.check = z._adler.adler32(this.check, this.window, read, len);
            }
            Array.Copy(this.window, read, z.next_out, destinationIndex, len);
            destinationIndex += len;
            read += len;
            if (read == this.end)
            {
                read = 0;
                if (this.write == this.end)
                {
                    this.write = 0;
                }
                len = this.write - read;
                if (len > z.avail_out)
                {
                    len = z.avail_out;
                }
                if ((len != 0) && (r == -5))
                {
                    r = 0;
                }
                z.avail_out -= len;
                z.total_out += len;
                if (this.checkfn != null)
                {
                    z.adler = this.check = z._adler.adler32(this.check, this.window, read, len);
                }
                Array.Copy(this.window, read, z.next_out, destinationIndex, len);
                destinationIndex += len;
                read += len;
            }
            z.next_out_index = destinationIndex;
            this.read = read;
            return r;
        }

        internal int proc(ZStream z, int r)
        {
            int table;
            int sourceIndex = z.next_in_index;
            int num5 = z.avail_in;
            int bitb = this.bitb;
            int bitk = this.bitk;
            int write = this.write;
            int num7 = (write < this.read) ? ((this.read - write) - 1) : (this.end - write);
        Label_0047:
            switch (this.mode)
            {
                case 0:
                    while (bitk < 3)
                    {
                        if (num5 != 0)
                        {
                            r = 0;
                        }
                        else
                        {
                            this.bitb = bitb;
                            this.bitk = bitk;
                            z.avail_in = num5;
                            z.total_in += sourceIndex - z.next_in_index;
                            z.next_in_index = sourceIndex;
                            this.write = write;
                            return this.inflate_flush(z, r);
                        }
                        num5--;
                        bitb |= (z.next_in[sourceIndex++] & 0xff) << bitk;
                        bitk += 8;
                    }
                    table = bitb & 7;
                    this.last = table & 1;
                    switch ((table >> 1))
                    {
                        case 0:
                            bitb = bitb >> 3;
                            bitk -= 3;
                            table = bitk & 7;
                            bitb = ((int) bitb) >> table;
                            bitk -= table;
                            this.mode = 1;
                            break;

                        case 1:
                        {
                            int[] numArray = new int[1];
                            int[] numArray2 = new int[1];
                            int[][] numArray3 = new int[1][];
                            int[][] numArray4 = new int[1][];
                            InfTree.inflate_trees_fixed(numArray, numArray2, numArray3, numArray4, z);
                            this.codes.init(numArray[0], numArray2[0], numArray3[0], 0, numArray4[0], 0, z);
                            bitb = bitb >> 3;
                            bitk -= 3;
                            this.mode = 6;
                            break;
                        }
                        case 2:
                            bitb = bitb >> 3;
                            bitk -= 3;
                            this.mode = 3;
                            break;

                        case 3:
                            bitb = bitb >> 3;
                            bitk -= 3;
                            this.mode = 9;
                            z.msg = "invalid block type";
                            r = -3;
                            this.bitb = bitb;
                            this.bitk = bitk;
                            z.avail_in = num5;
                            z.total_in += sourceIndex - z.next_in_index;
                            z.next_in_index = sourceIndex;
                            this.write = write;
                            return this.inflate_flush(z, r);
                    }
                    goto Label_0047;

                case 1:
                    while (bitk < 0x20)
                    {
                        if (num5 != 0)
                        {
                            r = 0;
                        }
                        else
                        {
                            this.bitb = bitb;
                            this.bitk = bitk;
                            z.avail_in = num5;
                            z.total_in += sourceIndex - z.next_in_index;
                            z.next_in_index = sourceIndex;
                            this.write = write;
                            return this.inflate_flush(z, r);
                        }
                        num5--;
                        bitb |= (z.next_in[sourceIndex++] & 0xff) << bitk;
                        bitk += 8;
                    }
                    if (((~bitb >> 0x10) & 0xffff) != (bitb & 0xffff))
                    {
                        this.mode = 9;
                        z.msg = "invalid stored block lengths";
                        r = -3;
                        this.bitb = bitb;
                        this.bitk = bitk;
                        z.avail_in = num5;
                        z.total_in += sourceIndex - z.next_in_index;
                        z.next_in_index = sourceIndex;
                        this.write = write;
                        return this.inflate_flush(z, r);
                    }
                    this.left = bitb & 0xffff;
                    bitb = bitk = 0;
                    this.mode = (this.left != 0) ? 2 : ((this.last != 0) ? 7 : 0);
                    goto Label_0047;

                case 2:
                    if (num5 != 0)
                    {
                        if (num7 == 0)
                        {
                            if ((write == this.end) && (this.read != 0))
                            {
                                write = 0;
                                num7 = (write < this.read) ? ((this.read - write) - 1) : (this.end - write);
                            }
                            if (num7 == 0)
                            {
                                this.write = write;
                                r = this.inflate_flush(z, r);
                                write = this.write;
                                num7 = (write < this.read) ? ((this.read - write) - 1) : (this.end - write);
                                if ((write == this.end) && (this.read != 0))
                                {
                                    write = 0;
                                    num7 = (write < this.read) ? ((this.read - write) - 1) : (this.end - write);
                                }
                                if (num7 == 0)
                                {
                                    this.bitb = bitb;
                                    this.bitk = bitk;
                                    z.avail_in = num5;
                                    z.total_in += sourceIndex - z.next_in_index;
                                    z.next_in_index = sourceIndex;
                                    this.write = write;
                                    return this.inflate_flush(z, r);
                                }
                            }
                        }
                        r = 0;
                        table = this.left;
                        if (table > num5)
                        {
                            table = num5;
                        }
                        if (table > num7)
                        {
                            table = num7;
                        }
                        Array.Copy(z.next_in, sourceIndex, this.window, write, table);
                        sourceIndex += table;
                        num5 -= table;
                        write += table;
                        num7 -= table;
                        this.left -= table;
                        if (this.left == 0)
                        {
                            this.mode = (this.last != 0) ? 7 : 0;
                        }
                        goto Label_0047;
                    }
                    this.bitb = bitb;
                    this.bitk = bitk;
                    z.avail_in = num5;
                    z.total_in += sourceIndex - z.next_in_index;
                    z.next_in_index = sourceIndex;
                    this.write = write;
                    return this.inflate_flush(z, r);

                case 3:
                    while (bitk < 14)
                    {
                        if (num5 != 0)
                        {
                            r = 0;
                        }
                        else
                        {
                            this.bitb = bitb;
                            this.bitk = bitk;
                            z.avail_in = num5;
                            z.total_in += sourceIndex - z.next_in_index;
                            z.next_in_index = sourceIndex;
                            this.write = write;
                            return this.inflate_flush(z, r);
                        }
                        num5--;
                        bitb |= (z.next_in[sourceIndex++] & 0xff) << bitk;
                        bitk += 8;
                    }
                    this.table = table = bitb & 0x3fff;
                    if (((table & 0x1f) > 0x1d) || (((table >> 5) & 0x1f) > 0x1d))
                    {
                        this.mode = 9;
                        z.msg = "too many length or distance symbols";
                        r = -3;
                        this.bitb = bitb;
                        this.bitk = bitk;
                        z.avail_in = num5;
                        z.total_in += sourceIndex - z.next_in_index;
                        z.next_in_index = sourceIndex;
                        this.write = write;
                        return this.inflate_flush(z, r);
                    }
                    table = (0x102 + (table & 0x1f)) + ((table >> 5) & 0x1f);
                    if ((this.blens == null) || (this.blens.Length < table))
                    {
                        this.blens = new int[table];
                    }
                    else
                    {
                        for (int i = 0; i < table; i++)
                        {
                            this.blens[i] = 0;
                        }
                    }
                    bitb = bitb >> 14;
                    bitk -= 14;
                    this.index = 0;
                    this.mode = 4;
                    break;

                case 4:
                    break;

                case 5:
                    goto Label_07CB;

                case 6:
                    goto Label_0B71;

                case 7:
                    goto Label_0C3A;

                case 8:
                    goto Label_0CCF;

                case 9:
                    r = -3;
                    this.bitb = bitb;
                    this.bitk = bitk;
                    z.avail_in = num5;
                    z.total_in += sourceIndex - z.next_in_index;
                    z.next_in_index = sourceIndex;
                    this.write = write;
                    return this.inflate_flush(z, r);

                default:
                    r = -2;
                    this.bitb = bitb;
                    this.bitk = bitk;
                    z.avail_in = num5;
                    z.total_in += sourceIndex - z.next_in_index;
                    z.next_in_index = sourceIndex;
                    this.write = write;
                    return this.inflate_flush(z, r);
            }
            while (this.index < (4 + (this.table >> 10)))
            {
                while (bitk < 3)
                {
                    if (num5 != 0)
                    {
                        r = 0;
                    }
                    else
                    {
                        this.bitb = bitb;
                        this.bitk = bitk;
                        z.avail_in = num5;
                        z.total_in += sourceIndex - z.next_in_index;
                        z.next_in_index = sourceIndex;
                        this.write = write;
                        return this.inflate_flush(z, r);
                    }
                    num5--;
                    bitb |= (z.next_in[sourceIndex++] & 0xff) << bitk;
                    bitk += 8;
                }
                this.blens[border[this.index++]] = bitb & 7;
                bitb = bitb >> 3;
                bitk -= 3;
            }
            while (this.index < 0x13)
            {
                this.blens[border[this.index++]] = 0;
            }
            this.bb[0] = 7;
            table = this.inftree.inflate_trees_bits(this.blens, this.bb, this.tb, this.hufts, z);
            if (table != 0)
            {
                r = table;
                if (r == -3)
                {
                    this.blens = null;
                    this.mode = 9;
                }
                this.bitb = bitb;
                this.bitk = bitk;
                z.avail_in = num5;
                z.total_in += sourceIndex - z.next_in_index;
                z.next_in_index = sourceIndex;
                this.write = write;
                return this.inflate_flush(z, r);
            }
            this.index = 0;
            this.mode = 5;
        Label_07CB:
            table = this.table;
            if (this.index < ((0x102 + (table & 0x1f)) + ((table >> 5) & 0x1f)))
            {
                table = this.bb[0];
                while (bitk < table)
                {
                    if (num5 != 0)
                    {
                        r = 0;
                    }
                    else
                    {
                        this.bitb = bitb;
                        this.bitk = bitk;
                        z.avail_in = num5;
                        z.total_in += sourceIndex - z.next_in_index;
                        z.next_in_index = sourceIndex;
                        this.write = write;
                        return this.inflate_flush(z, r);
                    }
                    num5--;
                    bitb |= (z.next_in[sourceIndex++] & 0xff) << bitk;
                    bitk += 8;
                }
                int num1 = this.tb[0];
                table = this.hufts[((this.tb[0] + (bitb & inflate_mask[table])) * 3) + 1];
                int num11 = this.hufts[((this.tb[0] + (bitb & inflate_mask[table])) * 3) + 2];
                if (num11 < 0x10)
                {
                    bitb = ((int) bitb) >> table;
                    bitk -= table;
                    this.blens[this.index++] = num11;
                }
                else
                {
                    int index = (num11 == 0x12) ? 7 : (num11 - 14);
                    int num10 = (num11 == 0x12) ? 11 : 3;
                    while (bitk < (table + index))
                    {
                        if (num5 != 0)
                        {
                            r = 0;
                        }
                        else
                        {
                            this.bitb = bitb;
                            this.bitk = bitk;
                            z.avail_in = num5;
                            z.total_in += sourceIndex - z.next_in_index;
                            z.next_in_index = sourceIndex;
                            this.write = write;
                            return this.inflate_flush(z, r);
                        }
                        num5--;
                        bitb |= (z.next_in[sourceIndex++] & 0xff) << bitk;
                        bitk += 8;
                    }
                    bitb = ((int) bitb) >> table;
                    bitk -= table;
                    num10 += bitb & inflate_mask[index];
                    bitb = ((int) bitb) >> index;
                    bitk -= index;
                    index = this.index;
                    table = this.table;
                    if (((index + num10) > ((0x102 + (table & 0x1f)) + ((table >> 5) & 0x1f))) || ((num11 == 0x10) && (index < 1)))
                    {
                        this.blens = null;
                        this.mode = 9;
                        z.msg = "invalid bit length repeat";
                        r = -3;
                        this.bitb = bitb;
                        this.bitk = bitk;
                        z.avail_in = num5;
                        z.total_in += sourceIndex - z.next_in_index;
                        z.next_in_index = sourceIndex;
                        this.write = write;
                        return this.inflate_flush(z, r);
                    }
                    num11 = (num11 == 0x10) ? this.blens[index - 1] : 0;
                    do
                    {
                        this.blens[index++] = num11;
                    }
                    while (--num10 != 0);
                    this.index = index;
                }
                goto Label_07CB;
            }
            this.tb[0] = -1;
            int[] bl = new int[1];
            int[] bd = new int[1];
            int[] tl = new int[1];
            int[] td = new int[1];
            bl[0] = 9;
            bd[0] = 6;
            table = this.table;
            table = this.inftree.inflate_trees_dynamic(0x101 + (table & 0x1f), 1 + ((table >> 5) & 0x1f), this.blens, bl, bd, tl, td, this.hufts, z);
            switch (table)
            {
                case 0:
                    this.codes.init(bl[0], bd[0], this.hufts, tl[0], this.hufts, td[0], z);
                    this.mode = 6;
                    goto Label_0B71;

                case -3:
                    this.blens = null;
                    this.mode = 9;
                    break;
            }
            r = table;
            this.bitb = bitb;
            this.bitk = bitk;
            z.avail_in = num5;
            z.total_in += sourceIndex - z.next_in_index;
            z.next_in_index = sourceIndex;
            this.write = write;
            return this.inflate_flush(z, r);
        Label_0B71:
            this.bitb = bitb;
            this.bitk = bitk;
            z.avail_in = num5;
            z.total_in += sourceIndex - z.next_in_index;
            z.next_in_index = sourceIndex;
            this.write = write;
            if ((r = this.codes.proc(this, z, r)) != 1)
            {
                return this.inflate_flush(z, r);
            }
            r = 0;
            this.codes.free(z);
            sourceIndex = z.next_in_index;
            num5 = z.avail_in;
            bitb = this.bitb;
            bitk = this.bitk;
            write = this.write;
            num7 = (write < this.read) ? ((this.read - write) - 1) : (this.end - write);
            if (this.last == 0)
            {
                this.mode = 0;
                goto Label_0047;
            }
            this.mode = 7;
        Label_0C3A:
            this.write = write;
            r = this.inflate_flush(z, r);
            write = this.write;
            num7 = (write < this.read) ? ((this.read - write) - 1) : (this.end - write);
            if (this.read != this.write)
            {
                this.bitb = bitb;
                this.bitk = bitk;
                z.avail_in = num5;
                z.total_in += sourceIndex - z.next_in_index;
                z.next_in_index = sourceIndex;
                this.write = write;
                return this.inflate_flush(z, r);
            }
            this.mode = 8;
        Label_0CCF:
            r = 1;
            this.bitb = bitb;
            this.bitk = bitk;
            z.avail_in = num5;
            z.total_in += sourceIndex - z.next_in_index;
            z.next_in_index = sourceIndex;
            this.write = write;
            return this.inflate_flush(z, r);
        }

        internal void reset(ZStream z, long[] c)
        {
            if (c != null)
            {
                c[0] = this.check;
            }
            if (this.mode != 4)
            {
                int mode = this.mode;
            }
            if (this.mode == 6)
            {
                this.codes.free(z);
            }
            this.mode = 0;
            this.bitk = 0;
            this.bitb = 0;
            this.read = this.write = 0;
            if (this.checkfn != null)
            {
                z.adler = this.check = z._adler.adler32(0L, null, 0, 0);
            }
        }

        internal void set_dictionary(byte[] d, int start, int n)
        {
            Array.Copy(d, start, this.window, 0, n);
            this.read = this.write = n;
        }

        internal int sync_point()
        {
            if (this.mode != 1)
            {
                return 0;
            }
            return 1;
        }
    }
}

