#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Biff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents the <see cref="T:Dt.Xls.SimpleBinaryReader" /> class which depends on BitConverter helper.
    /// </summary>
    internal class SimpleBinaryReader
    {
        private byte[] _data;
        private int _length;
        private int _position;

        public SimpleBinaryReader(byte[] buffer)
        {
            this._data = buffer;
            this._length = buffer.Length;
            this._position = 0;
        }

        public static string BytesToString(byte[] b, int charCount, bool highByte)
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

        public static string DecodeText(string text)
        {
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
                        default:
                            goto Label_00A8;
                    }
                    list.Add(((char) ch2) + @":\");
                    continue;
                Label_00A8:
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

        public bool ReadBoolean()
        {
            bool flag = this._data[this._position] != 0;
            this._position++;
            return flag;
        }

        public byte ReadByte()
        {
            byte num = this._data[this._position];
            this._position++;
            return num;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] buffer = new byte[count];
            if (count > (this._data.Length - this._position))
            {
                count = this._data.Length - this._position;
            }
            Array.Copy(this._data, this._position, buffer, 0, count);
            this._position += count;
            return buffer;
        }

        public bool ReadCompressedString(short byteSizeOfCharCount, out string outString)
        {
            short num;
            if (byteSizeOfCharCount == 1)
            {
                num = this.ReadByte();
            }
            else
            {
                if (byteSizeOfCharCount != 2)
                {
                    throw new NotSupportedException();
                }
                num = this.ReadInt16();
            }
            return this.ReadUncompressedString(num, out outString);
        }

        public double ReadDouble()
        {
            double num = BitConverter.ToDouble(this._data, this._position);
            this._position += 8;
            return num;
        }

        public short ReadInt16()
        {
            short num = BitConverter.ToInt16(this._data, this._position);
            this._position += 2;
            return num;
        }

        public int ReadInt32()
        {
            int num = BitConverter.ToInt32(this._data, this._position);
            this._position += 4;
            return num;
        }

        public long ReadInt64()
        {
            long num = BitConverter.ToInt64(this._data, this._position);
            this._position += 8;
            return num;
        }

        public ushort ReadUInt16()
        {
            ushort num = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            return num;
        }

        public uint ReadUInt32()
        {
            uint num = BitConverter.ToUInt32(this._data, this._position);
            this._position += 4;
            return num;
        }

        public ulong ReadUInt64()
        {
            ulong num = BitConverter.ToUInt64(this._data, this._position);
            this._position += 8;
            return num;
        }

        public bool ReadUncompressedString(int charCount, out string outString)
        {
            outString = null;
            if (charCount == -1)
            {
                return false;
            }
            if (charCount == 0)
            {
                outString = string.Empty;
                return true;
            }
            bool highByte = (this.ReadByte() & 1) == 1;
            outString = BytesToString(this.ReadBytes(highByte ? (charCount * 2) : charCount), charCount, highByte);
            return true;
        }

        public string ReadUnicodeString(int charCount)
        {
            return BytesToString(this.ReadBytes(charCount * 2), charCount, true);
        }

        public void Seek(int position, SeekOrigin seekOrigin = (SeekOrigin)1)
        {
            this._position += position;
        }

        internal int Remaining
        {
            get { return  (this._length - this._position); }
        }
    }
}

