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
    internal class ExternDocName : BiffRecord
    {
        public ExternDocName()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public override void Read(BinaryReader reader)
        {
            this.ixals = reader.ReadUInt16();
            reader.ReadUInt16();
            this.extName = new ShortXLUnicodeString();
            this.extName.Read(reader);
            this.nameDefinition = new ExtNameParsedFormula();
            this.nameDefinition.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.ixals);
            writer.Write((short) 0);
            this.extName.Write(writer);
            this.nameDefinition.Write(writer);
        }

        public override short DataLength
        {
            get { return  (short) ((4 + this.extName.DataLength) + this.nameDefinition.DataLength); }
        }

        public ShortXLUnicodeString extName { get; set; }

        public ushort ixals { get; set; }

        public ExtNameParsedFormula nameDefinition { get; set; }
    }
}

