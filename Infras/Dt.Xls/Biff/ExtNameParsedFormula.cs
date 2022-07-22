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
    internal class ExtNameParsedFormula : BiffRecord
    {
        public ExtNameParsedFormula()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public override void Read(BinaryReader reader)
        {
            this.cb = reader.ReadUInt16();
            if (this.cb > 0)
            {
                this.extPtg = reader.ReadByte();
                if (this.extPtg == 0x3a)
                {
                    this.val = new ExtPtgRef3D();
                }
                else if (this.extPtg == 0x3b)
                {
                    this.val = new ExtPtgArea3D();
                }
                else if (this.extPtg == 60)
                {
                    this.val = new ExtPtgRefErr3D();
                }
                else if (this.extPtg == 0x3d)
                {
                    this.val = new ExtPtgAreaErr3D();
                }
                else if (this.extPtg == 0x1c)
                {
                    this.val = new ExtPtgErr();
                }
                this.val.Read(reader);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.cb);
            if (this.cb > 0)
            {
                writer.Write(this.extPtg);
                this.val.Write(writer);
            }
        }

        public ushort cb { get; set; }

        public override short DataLength
        {
            get
            {
                if (this.cb == 0)
                {
                    return 2;
                }
                return (short) (3 + this.val.DataLength);
            }
        }

        public byte extPtg { get; set; }

        public BiffRecord val { get; set; }
    }
}

