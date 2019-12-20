#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Biff
{
    internal class Lbl : BiffRecord
    {
        public Lbl()
        {
            base.RecordType = BiffRecordNumber.NAME;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            ushort num = reader.ReadUInt16();
            this.fHidden = (num & 1) == 1;
            this.fFunc = (num & 2) == 2;
            this.fOB = (num & 4) == 4;
            this.fProc = (num & 8) == 8;
            this.fCalcExp = (num & 0x10) == 0x10;
            this.fBuiltin = (num & 0x20) == 0x20;
            this.fGrp = (byte)((num & 0xfc0) >> 6);
            this.fPublished = (num & 0x2000) == 0x2000;
            this.fWorkbookParam = (num & 0x4000) == 0x4000;
            this.chKey = reader.ReadByte();
            this.cch = reader.ReadByte();
            this.cce = reader.ReadUInt16();
            reader.ReadUInt16();
            this.itab = reader.ReadUInt16();
            reader.ReadUInt32();
            this.Name = new XLUnicodeStringNoCch();
            this.Name.cch = this.cch;
            this.Name.Read(reader);
            this.rgce = reader.ReadBytes(this.cce);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            ushort num = 0;
            if (this.fHidden)
            {
                num |= 1;
            }
            if (this.fFunc)
            {
                num |= 2;
            }
            if (this.fOB)
            {
                num |= 4;
            }
            if (this.fProc)
            {
                num |= 8;
            }
            if (this.fCalcExp)
            {
                num |= 0x10;
            }
            if (this.fBuiltin)
            {
                num |= 0x20;
            }
            num |= (ushort)(this.fGrp << 6);
            if (this.fPublished)
            {
                num |= 0x2000;
            }
            if (this.fWorkbookParam)
            {
                num |= 0x4000;
            }
            writer.Write(num);
            writer.Write(this.chKey);
            writer.Write(this.cch);
            writer.Write(this.cce);
            writer.Write((ushort) 0);
            writer.Write(this.itab);
            writer.Write((uint) 0);
            if (this.Name != null)
            {
                this.Name.Write(writer);
            }
            if (this.rgce != null)
            {
                writer.Write(this.rgce);
            }
            if ((this.extra != null) && (this.extra.Length > 0))
            {
                writer.Write(this.extra);
            }
        }

        public ushort cce { get; set; }

        public byte cch { get; set; }

        public byte chKey { get; set; }

        public override short DataLength
        {
            get
            {
                int num = 14;
                if (this.Name != null)
                {
                    num += this.Name.DataLength;
                }
                if (this.rgce != null)
                {
                    num += this.rgce.Length;
                }
                if (this.extra != null)
                {
                    num += this.extra.Length;
                }
                return (short) num;
            }
        }

        public byte[] extra { get; set; }

        public bool fBuiltin { get; set; }

        public bool fCalcExp { get; set; }

        public bool fFunc { get; set; }

        public byte fGrp { get; set; }

        public bool fHidden { get; set; }

        public bool fOB { get; set; }

        public bool fProc { get; set; }

        public bool fPublished { get; set; }

        public bool fWorkbookParam { get; set; }

        public ushort itab { get; set; }

        public XLUnicodeStringNoCch Name { get; set; }

        public byte[] rgce { get; set; }
    }
}

