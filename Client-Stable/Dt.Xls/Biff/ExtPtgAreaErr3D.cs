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
    internal class ExtPtgAreaErr3D : BiffRecord
    {
        public ExtPtgAreaErr3D()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public override void Read(BinaryReader reader)
        {
            this.itabFirst = reader.ReadInt16();
            this.itabLast = reader.ReadInt16();
            reader.ReadInt32();
            reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.itabFirst);
            writer.Write(this.itabLast);
            writer.Write(0);
            writer.Write(0);
        }

        public override short DataLength
        {
            get { return  12; }
        }

        public short itabFirst { get; set; }

        public short itabLast { get; set; }
    }
}

