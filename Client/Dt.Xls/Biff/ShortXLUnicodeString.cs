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
using System.Text;
#endregion

namespace Dt.Xls.Biff
{
    internal class ShortXLUnicodeString : BiffRecord
    {
        public ShortXLUnicodeString()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        /// <summary>
        /// Reads the structure content from a binary stream.
        /// </summary>
        /// <param name="reader">A binary reader.</param>
        public override void Read(BinaryReader reader)
        {
            this.cch = reader.ReadByte();
            this.fHighByte = (reader.ReadByte() & 1) == 1;
            if (this.fHighByte)
            {
                this.rgb = reader.ReadBytes(this.cch * 2);
            }
            else
            {
                this.rgb = reader.ReadBytes(this.cch);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.cch);
            if (this.fHighByte)
            {
                writer.Write((byte) 1);
            }
            else
            {
                writer.Write((byte) 0);
            }
            writer.Write(this.rgb);
        }

        public byte cch { get; set; }

        public override short DataLength
        {
            get { return  (short) (2 + this.rgb.Length); }
        }

        public bool fHighByte { get; set; }

        public byte[] rgb { get; set; }

        public string Text
        {
            get
            {
                if (this.rgb == null)
                {
                    return null;
                }
                bool flag = false;
                char[] chArray = null;
                if (this.fHighByte)
                {
                    flag = true;
                }
                else
                {
                    for (int i = 0; i < this.rgb.Length; i++)
                    {
                        if (this.rgb[i] > 0x7f)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    chArray = Encoding.Unicode.GetChars(this.rgb, 0, this.cch * 2);
                }
                else
                {
                    chArray = EncodingHelper.GetASCIIChars(this.rgb, 0, this.cch);
                }
                return (string) new string(chArray);
            }
            set
            {
                if (this.fHighByte)
                {
                    this.rgb = Encoding.Unicode.GetBytes(value);
                    this.cch = (byte)(this.rgb.Length / 2);
                }
                else
                {
                    this.rgb = EncodingHelper.GetASCIIBytes(value);
                    this.cch = (byte)this.rgb.Length;
                }
            }
        }
    }
}

