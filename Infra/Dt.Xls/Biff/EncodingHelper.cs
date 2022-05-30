#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls.Biff
{
    internal class EncodingHelper
    {
        internal static byte[] GetASCIIBytes(string s)
        {
            byte[] buffer = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                buffer[i] = (byte) s[i];
            }
            return buffer;
        }

        public static char[] GetASCIIChars(byte[] bytes)
        {
            return GetASCIIChars(bytes, 0, bytes.Length);
        }

        public static char[] GetASCIIChars(byte[] bytes, int index, int count)
        {
            char[] chArray = new char[count];
            for (int i = index; i < (index + count); i++)
            {
                chArray[i - index] = (char) bytes[i];
            }
            return chArray;
        }
    }
}

