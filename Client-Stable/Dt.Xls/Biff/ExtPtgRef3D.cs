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
    internal class ExtPtgRef3D : BiffRecord
    {
        public ExtPtgRef3D()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public override void Read(BinaryReader reader)
        {
            this.itabFirst = reader.ReadInt16();
            this.itabLast = reader.ReadInt16();
            this.loc = new RgceLocRel();
            this.loc.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.itabFirst);
            writer.Write(this.itabLast);
            this.loc.Write(writer);
        }

        public override short DataLength
        {
            get { return  8; }
        }

        public short itabFirst { get; set; }

        public short itabLast { get; set; }

        public RgceLocRel loc { get; set; }
    }
}

