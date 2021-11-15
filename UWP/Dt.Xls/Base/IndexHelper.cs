#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
#endregion

namespace Dt.Xls
{
    internal static class IndexHelper
    {
        private static Vector<string> _columnsInA1Letter = new Vector<string>();

        static IndexHelper()
        {
            string[] strArray = new string[] { 
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", 
                "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
             };
            for (int i = 0; i < 0x1a; i++)
            {
                _columnsInA1Letter[i] = strArray[i];
            }
        }

        internal static string GetColumnIndexInA1Letter(int coord)
        {
            if (coord < 0)
            {
                return string.Empty;
            }
            string str = _columnsInA1Letter[coord];
            if (str == null)
            {
                int num = coord;
                StringBuilder builder = new StringBuilder();
                int length = builder.Length;
                coord++;
                while (coord > 0)
                {
                    builder.Insert(length, (char[]) new char[] { ((char) (0x41 + ((coord - 1) % 0x1a))) });
                    coord = (coord - 1) / 0x1a;
                }
                str = builder.ToString();
                _columnsInA1Letter[num] = str;
            }
            return str;
        }

        internal static int GetColumnIndexInNumber(string s)
        {
            int num4;
            int num = 0;
            int length = s.Length;
            int num3 = 0;
            if (num >= length)
            {
                goto Label_004F;
            }
            char ch = s[num];
        Label_0017:
            num4 = ch - 'a';
            if (num4 <= 0x19)
            {
                if (num4 < 0)
                {
                    num4 = ch - 'A';
                }
                if (num4 >= 0)
                {
                    num3 = ((0x1a * num3) + num4) + 1;
                    num++;
                    if (num < length)
                    {
                        ch = s[num];
                        goto Label_0017;
                    }
                }
            }
        Label_004F:
            num3--;
            return num3;
        }

        internal static int GetRowIndexInNumber(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            int num = 0;
            int index = 0;
            while ((index < s.Length) && !char.IsDigit(s, index))
            {
                index++;
            }
            if (index < s.Length)
            {
                num = int.Parse(s.Substring(index));
            }
            return (num - 1);
        }
    }
}

