#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    internal class MsoCrc32
    {
        private uint[] cache = new uint[0x100];
        private bool calledOnce;

        internal static uint CRC(byte[] array)
        {
            MsoCrc32 crc = new MsoCrc32();
            return crc.CRC(0, array);
        }

        internal uint CRC(uint crcValue, byte[] array)
        {
            this.InitCrcCache();
            foreach (byte num in array)
            {
                uint index = crcValue;
                index = index >> 0x18;
                index ^= num;
                crcValue = crcValue << 8;
                crcValue ^= this.cache[index];
            }
            return crcValue;
        }

        public void InitCrcCache()
        {
            if (!this.calledOnce)
            {
                for (uint i = 0; i <= 0xff; i++)
                {
                    uint num2 = i;
                    num2 = num2 << 0x18;
                    for (uint j = 0; j <= 7; j++)
                    {
                        if ((num2 & 0x80000000) != 0)
                        {
                            num2 = num2 << 1;
                            num2 ^= 0xaf;
                        }
                        else
                        {
                            num2 = num2 << 1;
                        }
                    }
                    this.cache[i] = num2 & 0xffff;
                }
                this.calledOnce = true;
            }
        }
    }
}

