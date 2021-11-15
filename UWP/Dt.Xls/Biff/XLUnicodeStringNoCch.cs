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
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
#endregion

namespace Dt.Xls.Biff
{
    /// <summary>
    /// This structure specifies a Unicode string. When an XLUnicodeStringNoCch is used, the count of 
    /// characters in the string MUST be specified in the structure that uses the XLUnicodeStringNoCch.
    /// </summary>
    internal class XLUnicodeStringNoCch : BiffRecord
    {
        public void Read(SimpleBinaryReader reader)
        {
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
        /// Writes the structure content to a binary stream.
        /// </summary>
        /// <param name="writer">A binary reader.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.fHighByte);
            writer.Write(this.rgb);
        }

        /// <summary>
        /// An unsigned integer that specifies the count of characters in the string.
        /// </summary>
        public ushort cch { get; set; }

        /// <summary>
        /// Gets the size of the structure in bytes.
        /// </summary>
        public override short DataLength
        {
            get { return  (short) (1 + this.rgb.Length); }
        }

        /// <summary>
        /// Gets the decoded text.
        /// </summary>
        /// <remarks>
        /// The decoded text is used by SupBook record.
        /// </remarks>
        public string DecodedText
        {
            get
            {
                string text = this.Text;
                char[] chArray = text.ToCharArray();
                List<string> list = new List<string>();
                char ch = chArray[0];
                if (ch != '\x0001')
                {
                    return text;
                }
                for (int i = 1; i < chArray.Length; i++)
                {
                    char ch2 = chArray[i];
                    try
                    {
                        switch (Convert.ToUInt16(ch2))
                        {
                            case 1:
                            {
                                ch2 = chArray[++i];
                                if (ch2 != '@')
                                {
                                    break;
                                }
                                list.Add(@"\\");
                                continue;
                            }
                            case 2:
                            {
                                list.Add(@"\");
                                continue;
                            }
                            case 3:
                            {
                                list.Add(@"\");
                                continue;
                            }
                            case 4:
                            {
                                list.Add(@"..\");
                                continue;
                            }
                            case 8:
                            {
                                continue;
                            }
                            default:
                                goto Label_00C3;
                        }
                        list.Add(((char) ch2) + @":\");
                        continue;
                    Label_00C3:
                        list.Add(((char) ch2).ToString());
                    }
                    catch
                    {
                        list.Add(((char) ch2).ToString());
                    }
                }
                StringBuilder builder = new StringBuilder();
                int num3 = list.Count;
                for (int j = 0; j < num3; j++)
                {
                    builder.Append(list[j]);
                }
                return builder.ToString();
            }
            set
            {
            }
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
        public byte fHighByte { get; set; }

        /// <summary>
        /// An array of bytes that specifies the characters. If fHighByte is 0x0, the size of the array 
        /// MUST be equal to cch. If fHighByte is 0x1, the size of the array MUST be equal to cch*2.
        /// </summary>
        public byte[] rgb { get; set; }

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
                    chArray = Encoding.Unicode.GetChars(this.rgb, 0, this.rgb.Length);
                }
                else
                {
                    chArray = EncodingHelper.GetASCIIChars(this.rgb, 0, this.rgb.Length);
                }
                return (string) new string(chArray);
            }
            set
            {
                this.rgb = Encoding.Unicode.GetBytes(value);
                this.cch = (ushort)(this.rgb.Length / 2);
                this.fHighByte = 1;
            }
        }
    }
}

