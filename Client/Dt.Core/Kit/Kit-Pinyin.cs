#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 拼音码
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 获取给定字符串的拼音码(小写)
        /// </summary>
        /// <param name="p_str">字符串</param>
        public static string GetPinYin(string p_str)
        {
            if (string.IsNullOrWhiteSpace(p_str))
                return null;

            bool exists = false;
            foreach (var ch in p_str)
            {
                if (0x4e00 <= ch && ch <= 0x9fa5)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
                return p_str;

            if (_gbk == null)
                _gbk = Encoding.GetEncoding("GBK");

            var unis = Encoding.Unicode.GetBytes(p_str);
            var gbks = Encoding.Convert(Encoding.Unicode, _gbk, unis);
            StringBuilder sb = new StringBuilder();
            int i = 0;
            ushort c = 0;

            while (i < gbks.Length)
            {
                var b = gbks[i];
                if (b <= 127)
                {
                    sb.Append((char)b);
                    i++;
                }
                else
                {
                    c = (ushort)(gbks[i] * 256 + gbks[i + 1]);
                    if (c >= 0xB0A1 && c <= 0xB0C4)
                    {
                        sb.Append("a");
                    }
                    else if (c >= 0xB0C5 && c <= 0xB2C0)
                    {
                        sb.Append("b");
                    }
                    else if (c >= 0xB2C1 && c <= 0xB4ED)
                    {
                        sb.Append("c");
                    }
                    else if (c >= 0xB4EE && c <= 0xB6E9)
                    {
                        sb.Append("d");
                    }
                    else if (c >= 0xB6EA && c <= 0xB7A1)
                    {
                        sb.Append("e");
                    }
                    else if (c >= 0xB7A2 && c <= 0xB8C0)
                    {
                        sb.Append("f");
                    }
                    else if (c >= 0xB8C1 && c <= 0xB9FD)
                    {
                        sb.Append("g");
                    }
                    else if (c >= 0xB9FE && c <= 0xBBF6)
                    {
                        sb.Append("h");
                    }
                    else if (c >= 0xBBF7 && c <= 0xBFA5)
                    {
                        sb.Append("j");
                    }
                    else if (c >= 0xBFA6 && c <= 0xC0AB)
                    {
                        sb.Append("k");
                    }
                    else if (c >= 0xC0AC && c <= 0xC2E7)
                    {
                        sb.Append("l");
                    }
                    else if (c >= 0xC2E8 && c <= 0xC4C2)
                    {
                        sb.Append("m");
                    }
                    else if (c >= 0xC4C3 && c <= 0xC5B5)
                    {
                        sb.Append("n");
                    }
                    else if (c >= 0xC5B6 && c <= 0xC5BD)
                    {
                        sb.Append("o");
                    }
                    else if (c >= 0xC5BE && c <= 0xC6D9)
                    {
                        sb.Append("p");
                    }
                    else if (c >= 0xC6DA && c <= 0xC8BA)
                    {
                        sb.Append("q");
                    }
                    else if (c >= 0xC8BB && c <= 0xC8F5)
                    {
                        sb.Append("r");
                    }
                    else if (c >= 0xC8F6 && c <= 0xCBF9)
                    {
                        sb.Append("s");
                    }
                    else if (c >= 0xCBFA && c <= 0xCDD9)
                    {
                        sb.Append("t");
                    }
                    else if (c >= 0xCDDA && c <= 0xCEF3)
                    {
                        sb.Append("w");
                    }
                    else if (c >= 0xCEF4 && c <= 0xD188)
                    {
                        sb.Append("x");
                    }
                    else if (c >= 0xD1B9 && c <= 0xD4D0)
                    {
                        sb.Append("y");
                    }
                    else if (c >= 0xD4D1 && c <= 0xD7F9)
                    {
                        sb.Append("z");
                    }
                    else
                    {
                        sb.Append("?");
                    }
                    i = i + 2;
                }
            }
            return sb.ToString();
        }
    }
}