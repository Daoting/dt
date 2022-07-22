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
    internal class RgceArea : BiffRecord
    {
        public RgceArea()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public override void Read(BinaryReader reader)
        {
            this.rowFirst = reader.ReadInt16();
            this.rowLast = reader.ReadInt16();
            ushort num = reader.ReadUInt16();
            this.columnFirst = (short)(num & 0x3fff);
            this.colFirstRelative = (num & 0x4000) == 0x4000;
            this.rowFirstRelative = (num & 0x8000) == 0x8000;
            num = reader.ReadUInt16();
            this.columnLast = (short)(num & 0x3fff);
            this.colLastRelative = (num & 0x4000) == 0x4000;
            this.rowLastRelative = (num & 0x8000) == 0x8000;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.rowFirst);
            writer.Write(this.rowLast);
            ushort num = 0;
            num |= (ushort)this.columnFirst;
            if (this.colFirstRelative)
            {
                num |= 0x4000;
            }
            if (this.rowFirstRelative)
            {
                num |= 0x8000;
            }
            writer.Write(num);
            num = 0;
            num |= (ushort)this.columnLast;
            if (this.colLastRelative)
            {
                num |= 0x4000;
            }
            if (this.rowLastRelative)
            {
                num |= 0x8000;
            }
            writer.Write(num);
        }

        public bool colFirstRelative { get; set; }

        public bool colLastRelative { get; set; }

        public short columnFirst { get; set; }

        public short columnLast { get; set; }

        public override short DataLength
        {
            get { return 8; }
        }

        public short rowFirst { get; set; }

        public bool rowFirstRelative { get; set; }

        public short rowLast { get; set; }

        public bool rowLastRelative { get; set; }
    }
}

