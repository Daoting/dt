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
    internal class XTI : BiffRecord
    {
        public XTI()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public override void Read(BinaryReader reader)
        {
            this.iSupBook = reader.ReadUInt16();
            this.itabFirst = reader.ReadInt16();
            this.itabLast = reader.ReadInt16();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.iSupBook);
            writer.Write(this.itabFirst);
            writer.Write(this.itabLast);
        }

        public override short DataLength
        {
            get { return  6; }
        }

        public ushort iSupBook { get; set; }

        public short itabFirst { get; set; }

        public short itabLast { get; set; }
    }
}

