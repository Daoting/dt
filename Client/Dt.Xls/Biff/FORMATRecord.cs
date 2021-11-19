#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Biff
{
    internal class FORMATRecord : BiffRecord
    {
        private byte GetFormatOptionFlagRecord()
        {
            byte num = 0;
            if (this.IsCompressed)
            {
                num |= 1;
            }
            if (this.ContainsAsianPhoneticSettings)
            {
                num |= 4;
            }
            if (this.ContainsRichTextSettings)
            {
                num |= 8;
            }
            return num;
        }

        public void Read(SimpleBinaryReader reader)
        {
            string str;
            this.FormatStringLength = reader.ReadUInt16();
            this.OptionFlags = reader.ReadByte();
            reader.Seek(-1, (SeekOrigin) SeekOrigin.Current);
            reader.ReadUncompressedString((short) this.FormatStringLength, out str);
            this.FormatString = str;
        }

        public override void Write(BinaryWriter writer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer2 = new BinaryWriter((Stream) stream))
                {
                    WriteHelper.WriteBiffStr(writer2, this.FormatString, false, false, false, false, 2);
                    ushort num = (ushort)(2L + stream.Length);
                    writer.Write(this.RecordNumber);
                    writer.Write(num);
                    writer.Write(this.FormatIndex);
                    writer.Write(stream.GetBuffer(), 0, (int) stream.Length);
                }
            }
        }

        public bool ContainsAsianPhoneticSettings
        {
            get { return  ((this.OptionFlags & 4) == 4); }
        }

        public bool ContainsRichTextSettings
        {
            get { return  ((this.OptionFlags & 8) == 8); }
        }

        public ushort FormatIndex { get; set; }

        public string FormatString { get; set; }

        public ushort FormatStringLength { get; set; }

        public bool IsCompressed
        {
            get { return  ((this.OptionFlags & 1) == 0); }
        }

        public byte OptionFlags { get; set; }

        public ushort RecordLength { get; set; }

        public ushort RecordNumber
        {
            get { return  0x41e; }
        }
    }
}

