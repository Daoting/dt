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
using System.Text;
#endregion

namespace Dt.Xls.Biff
{
    internal class WriteHelper
    {
        internal static bool BuildBiffStrComponents(string stringIn, BiffRecordWriter.StringConvert stringConvert, ref short charCount, ref byte grbit, ref byte[] biffStrBuffer)
        {
            bool flag = false;
            if ((stringIn != null) && (0x7fff < stringIn.Length))
            {
                stringIn = stringIn.Substring(0, 0x7fff);
            }
            if (stringIn != null)
            {
                charCount = (short) stringIn.Length;
            }
            else
            {
                charCount = 0;
            }
            if (stringConvert == BiffRecordWriter.StringConvert.Unicode)
            {
                flag = true;
            }
            else if (stringConvert == BiffRecordWriter.StringConvert.Ascii)
            {
                flag = false;
            }
            else
            {
                for (int i = 0; i < charCount; i++)
                {
                    char ch = stringIn[i];
                    if ((ch < '\0') || (ch > '\x007f'))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            grbit = flag ? ((byte) 1) : ((byte) 0);
            if ((stringIn != null) && (charCount > 0))
            {
                string[] strArray = stringIn.Split(new char[] { '\r' });
                StringBuilder builder = new StringBuilder();
                foreach (string str in strArray)
                {
                    builder.Append(str);
                }
                stringIn = builder.ToString();
                if (flag)
                {
                    biffStrBuffer = Encoding.Unicode.GetBytes(stringIn);
                }
                else
                {
                    biffStrBuffer = EncodingHelper.GetASCIIBytes(stringIn);
                }
                if (flag)
                {
                    charCount = (short)(biffStrBuffer.Length / 2);
                }
                else
                {
                    charCount = (short)(biffStrBuffer.Length);
                }
            }
            else
            {
                charCount = 0;
            }
            return true;
        }

        internal static bool WriteBiffStr(BinaryWriter writer, string stringIn, bool useHighByteParam, bool highByte, bool extString, bool richString, short byteSizeOfCharCount)
        {
            short charCount = 0;
            byte grbit = 0;
            byte[] biffStrBuffer = null;
            BiffRecordWriter.StringConvert stringConvert = useHighByteParam ? (highByte ? BiffRecordWriter.StringConvert.Unicode : BiffRecordWriter.StringConvert.Ascii) : BiffRecordWriter.StringConvert.None;
            BuildBiffStrComponents(stringIn, stringConvert, ref charCount, ref grbit, ref biffStrBuffer);
            grbit |= (byte)((extString ? 4 : 0) | (richString ? 8 : 0));
            if ((byteSizeOfCharCount == 1) && (charCount >= 0))
            {
                writer.Write((byte) ((byte) charCount));
            }
            else
            {
                writer.Write((ushort) ((ushort) charCount));
            }
            writer.Write(grbit);
            if ((biffStrBuffer != null) && (biffStrBuffer.Length > 0))
            {
                writer.Write(biffStrBuffer);
            }
            return true;
        }
    }
}

