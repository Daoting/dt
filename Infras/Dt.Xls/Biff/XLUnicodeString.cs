#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.IO;
using System.Text;
#endregion

namespace Dt.Xls.Biff
{
    internal class XLUnicodeString : BiffRecord
    {
        private short _cch;
        private byte _fHighByte;
        private byte[] _rgb;

        public XLUnicodeString()
        {
            base.RecordType = BiffRecordNumber.NOTAVAILABLE;
        }

        private string BytesToString(byte[] b, int charCount, bool highByte)
        {
            char[] chArray = null;
            bool flag = false;
            if (highByte)
            {
                flag = true;
            }
            else
            {
                for (int i = 0; i < b.Length; i++)
                {
                    if (b[i] > 0x7f)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                if (!highByte)
                {
                    byte[] buffer = new byte[charCount * 2];
                    for (int j = 0; j < charCount; j++)
                    {
                        buffer[j * 2] = b[j];
                        buffer[(j * 2) + 1] = 0;
                    }
                    b = buffer;
                }
                chArray = Encoding.Unicode.GetChars(b, 0, charCount * 2);
            }
            else
            {
                chArray = EncodingHelper.GetASCIIChars(b, 0, charCount);
            }
            return (string) new string(chArray);
        }

        /// <summary>
        /// Reads the structure content from a binary stream.
        /// </summary>
        /// <param name="reader">A binary reader.</param>
        public void Read(SimpleBinaryReader reader)
        {
            this.cch = reader.ReadInt16();
            this.fHighByte = (byte)(reader.ReadByte() & 1);
            if (this.fHighByte == 1)
            {
                this.rgb = reader.ReadBytes(this.cch * 2);
            }
            else
            {
                this.rgb = reader.ReadBytes(this.cch);
            }
        }

        /// <summary>
        /// Reads the structure content from a binary stream.
        /// </summary>
        /// <param name="reader">A binary reader.</param>
        public override void Read(BinaryReader reader)
        {
            this.cch = reader.ReadInt16();
            this.fHighByte = (byte)(reader.ReadByte() & 1);
            if (this.fHighByte == 1)
            {
                this.rgb = reader.ReadBytes(this.cch * 2);
            }
            else
            {
                this.rgb = reader.ReadBytes(this.cch);
            }
        }

        private string ReadUncompressedString()
        {
            return this.BytesToString(this.rgb, this.cch, false);
        }

        /// <summary>
        /// Writes the structure content to a binary stream.
        /// </summary>
        /// <param name="writer">A binary reader.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.cch);
            writer.Write(this.fHighByte);
            writer.Write(this.rgb);
        }

        /// <summary>
        /// An unsigned integer that specifies the count of characters in the string.
        /// </summary>
        public short cch
        {
            get { return  this._cch; }
            set { this._cch = value; }
        }

        /// <summary>
        /// Gets the size of the structure in bytes.
        /// </summary>
        public override short DataLength
        {
            get { return  (short) (3 + this.rgb.Length); }
        }

        /// <summary>
        /// A bit that specifies whether the characters in rgb are double-byte characters. 
        /// </summary>
        /// <remarks>
        /// VALUE     |         MEANING
        /// 0x0                 All the characters in the string have a high byte of 0x00 and only the 
        /// low bytes are in rgb.
        /// 0x1                 All the characters in the string are saved as double-byte characters in rgb.
        /// </remarks>
        public byte fHighByte
        {
            get { return  this._fHighByte; }
            set { this._fHighByte = value; }
        }

        /// <summary>
        /// An array of bytes that specifies the characters. If fHighByte is 0x0, the size of the array 
        /// MUST be equal to cch. If fHighByte is 0x1, the size of the array MUST be equal to cch*2.
        /// </summary>
        public byte[] rgb
        {
            get { return  this._rgb; }
            set { this._rgb = value; }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
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
                if (this.fHighByte == 1)
                {
                    flag = true;
                }
                if (flag)
                {
                    chArray = Encoding.Unicode.GetChars(this.rgb, 0, Math.Min((int) this.rgb.Length, (int) (this.cch * 2)));
                    return (string) new string(chArray);
                }
                return this.ReadUncompressedString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (this.fHighByte == 1)
                    {
                        this.rgb = new byte[2];
                        this.cch = 1;
                    }
                    else
                    {
                        this.rgb = new byte[1];
                        this.cch = 1;
                    }
                }
                else if (this.fHighByte == 1)
                {
                    this.rgb = Encoding.Unicode.GetBytes(value);
                    this.cch = (short)(this.rgb.Length / 2);
                }
                else
                {
                    bool flag = true;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] > '\x007f')
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this.fHighByte = 0;
                        this.rgb = EncodingHelper.GetASCIIBytes(value);
                        this.cch = (short)this.rgb.Length;
                    }
                    else
                    {
                        this.fHighByte = 1;
                        this.rgb = Encoding.Unicode.GetBytes(value);
                        this.cch = (short)(this.rgb.Length / 2);
                    }
                }
            }
        }
    }
}

