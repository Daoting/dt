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
    internal class ExtPtgErr : BiffRecord
    {
        public ExtPtgErr()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public override void Read(BinaryReader reader)
        {
            this.err = (BErr) reader.ReadByte();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte) this.err);
        }

        public override short DataLength
        {
            get { return  1; }
        }

        public BErr err { get; set; }
    }
}

