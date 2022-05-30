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
    internal class SupBookBiff8Structure : BiffRecord
    {
        public SupBookBiff8Structure()
        {
            base.RecordType = BiffRecordNumber.SUPBOOK;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is SupBookBiff8Structure))
            {
                return false;
            }
            SupBookBiff8Structure structure = (SupBookBiff8Structure) obj;
            if (this.ctab != structure.ctab)
            {
                return false;
            }
            if (this.cch != structure.cch)
            {
                return false;
            }
            if ((this.virtPath != null) && (structure.virtPath == null))
            {
                return false;
            }
            if ((this.virtPath == null) && (structure.virtPath != null))
            {
                return false;
            }
            if (((this.virtPath != null) && (structure.virtPath != null)) && !this.virtPath.Text.Equals(structure.virtPath.Text))
            {
                return false;
            }
            if ((this.rgst != null) && (structure.rgst == null))
            {
                return false;
            }
            if ((this.rgst == null) && (structure.rgst != null))
            {
                return false;
            }
            if ((this.rgst != null) && (structure.rgst != null))
            {
                if (this.rgst.Length != structure.rgst.Length)
                {
                    return false;
                }
                for (int i = 0; i < this.rgst.Length; i++)
                {
                    if (!this.rgst[i].Text.Equals(structure.rgst[i].Text))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            this.ctab = reader.ReadUInt16();
            this.cch = reader.ReadUInt16();
            if ((this.cch > 0) && (this.cch <= 0xff))
            {
                this.virtPath = new XLUnicodeStringNoCch();
                this.virtPath.cch = this.cch;
                this.virtPath.Read(reader);
            }
            if ((this.cch > 0) && (this.cch <= 0xff))
            {
                this.rgst = new XLUnicodeString[this.ctab];
                for (int i = 0; i < this.rgst.Length; i++)
                {
                    this.rgst[i] = new XLUnicodeString();
                    this.rgst[i].Read(reader);
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(this.ctab);
            writer.Write(this.cch);
            if (this.virtPath != null)
            {
                this.virtPath.Write(writer);
            }
            if (this.rgst != null)
            {
                for (int i = 0; i < this.rgst.Length; i++)
                {
                    this.rgst[i].Write(writer);
                }
            }
        }

        public ushort cch { get; set; }

        public ushort ctab { get; set; }

        public override short DataLength
        {
            get
            {
                short num = 4;
                if (this.virtPath != null)
                {
                    num += this.virtPath.DataLength;
                }
                if (this.rgst != null)
                {
                    for (int i = 0; i < this.rgst.Length; i++)
                    {
                        num += this.rgst[i].DataLength;
                    }
                }
                return num;
            }
        }

        public XLUnicodeString[] rgst { get; set; }

        public XLUnicodeStringNoCch virtPath { get; set; }
    }
}

