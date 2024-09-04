#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Biff
{
    internal class ExternSheet : BiffRecord
    {
        public ExternSheet()
        {
            base.RecordType = BiffRecordNumber.EXTERNSHEET;
        }

        public void AddXti(XTI xti)
        {
            if (this.rgXTI == null)
            {
                this.rgXTI = new List<XTI>();
            }
            this.rgXTI.Add(xti);
            this.cXTI = (ushort) this.rgXTI.Count;
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            this.cXTI = reader.ReadUInt16();
            this.rgXTI = new List<XTI>();
            for (int i = 0; i < this.cXTI; i++)
            {
                XTI xti = new XTI();
                xti.Read(reader);
                this.rgXTI.Add(xti);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(this.cXTI);
            for (int i = 0; i < this.rgXTI.Count; i++)
            {
                this.rgXTI[i].Write(writer);
            }
        }

        public ushort cXTI { get; set; }

        public override short DataLength
        {
            get { return  (short) (2 + (this.cXTI * 6)); }
        }

        public List<XTI> rgXTI { get; set; }
    }
}

