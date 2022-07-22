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
    internal class RgceLocRel : BiffRecord
    {
        public RgceLocRel()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public override void Read(BinaryReader reader)
        {
            this.row = reader.ReadInt16();
            ushort num = reader.ReadUInt16();
            this.column = (short)(num & 0x3fff);
            this.colRelative = (num & 0x4000) == 0x4000;
            this.rowRelative = (num & 0x8000) == 0x8000;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.row);
            ushort num = 0;
            num |= (ushort) this.column;
            if (this.colRelative)
            {
                num |= 0x4000;
            }
            if (this.rowRelative)
            {
                num |= 0x8000;
            }
            writer.Write(num);
        }

        public bool colRelative { get; set; }

        public short column { get; set; }

        public override short DataLength
        {
            get { return  4; }
        }

        public short row { get; set; }

        public bool rowRelative { get; set; }
    }
}

