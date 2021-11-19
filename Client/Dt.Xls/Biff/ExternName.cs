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
    internal class ExternName : BiffRecord
    {
        public ExternName()
        {
            base.RecordType = BiffRecordNumber.EXTERNNAME;
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            ushort num = reader.ReadUInt16();
            this.fBuiltIn = (num & 1) == 1;
            this.fWantAdvise = (num & 2) == 2;
            this.fWantPict = (num & 4) == 4;
            this.fOle = (num & 8) == 8;
            this.fOleLink = (num & 0x10) == 0x10;
            this.cf = (short)((num >> 5) & 0x3ff);
            this.fIcon = (num & 0x8000) == 0x8000;
            if (!this.fOle && !this.fOleLink)
            {
                this.body = new ExternDocName();
                this.body.Read(reader);
            }
            else
            {
                reader.ReadBytes(this.DataLength - 2);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (!this.fOle && !this.fOleLink)
            {
                base.Write(writer);
                ushort num = 0;
                if (this.fBuiltIn)
                {
                    num |= 1;
                }
                if (this.fWantAdvise)
                {
                    num |= 2;
                }
                if (this.fWantPict)
                {
                    num |= 4;
                }
                if (this.fOle)
                {
                    num |= 8;
                }
                if (this.fOleLink)
                {
                    num |= 0x10;
                }
                num |= (ushort)(this.cf << 5);
                if (this.fIcon)
                {
                    num |= 0x8000;
                }
                writer.Write(num);
                this.body.Write(writer);
            }
        }

        public ExternDocName body { get; set; }

        public short cf { get; set; }

        public override short DataLength
        {
            get
            {
                if (this.body == null)
                {
                    return base.DataLength;
                }
                return (short) (2 + this.body.DataLength);
            }
        }

        public bool fBuiltIn { get; set; }

        public bool fIcon { get; set; }

        public bool fOle { get; set; }

        public bool fOleLink { get; set; }

        public bool fWantAdvise { get; set; }

        public bool fWantPict { get; set; }
    }
}

