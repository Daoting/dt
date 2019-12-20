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
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Xls.Utils
{
    internal static class StringExtension
    {
        private static Dictionary<char, string> _specialCharTable = new Dictionary<char, string>();
        private static Regex _specialPattern = new Regex("_x00[0-1][0-9A-Fa-f]_");
        private static StringBuilder sb = new StringBuilder();

        static StringExtension()
        {
            _specialCharTable.Add('\0', "_x0000_");
            _specialCharTable.Add('\x0001', "_x0001_");
            _specialCharTable.Add('\x0002', "_x0002_");
            _specialCharTable.Add('\x0003', "_x0003_");
            _specialCharTable.Add('\x0004', "_x0004_");
            _specialCharTable.Add('\x0005', "_x0005_");
            _specialCharTable.Add('\x0006', "_x0006_");
            _specialCharTable.Add('\a', "_x0007_");
            _specialCharTable.Add('\b', "_x0008_");
            _specialCharTable.Add('\t', "_x0009_");
            _specialCharTable.Add('\n', "_x000A_");
            _specialCharTable.Add('\v', "_x000B_");
            _specialCharTable.Add('\f', "_x000C_");
            _specialCharTable.Add('\r', "_x000D_");
            _specialCharTable.Add('\x000e', "_x000E_");
            _specialCharTable.Add('\x000f', "_x000F_");
            _specialCharTable.Add('\x0010', "_x0010_");
            _specialCharTable.Add('\x0011', "_x0011_");
            _specialCharTable.Add('\x0012', "_x0012_");
            _specialCharTable.Add('\x0013', "_x0013_");
            _specialCharTable.Add('\x0014', "_x0014_");
            _specialCharTable.Add('\x0015', "_x0015_");
            _specialCharTable.Add('\x0016', "_x0016_");
            _specialCharTable.Add('\x0017', "_x0017_");
            _specialCharTable.Add('\x0018', "_x0018_");
            _specialCharTable.Add('\x0019', "_x0019_");
            _specialCharTable.Add('\x001a', "_x001A_");
            _specialCharTable.Add('\x001b', "_x001B_");
            _specialCharTable.Add('\x001c', "_x001C_");
            _specialCharTable.Add('\x001d', "_x001D_");
            _specialCharTable.Add('\x001e', "_x001E_");
            _specialCharTable.Add('\x001f', "_x001F_");
        }

        internal static string EncodeEvaluator(Match match)
        {
            return string.Format("_x005F{0}", (object[]) new object[] { match.Value });
        }

        internal static string ToCamelCase(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            return (source.Substring(0, 1).ToLower() + ((source.Length == 1) ? "" : source.Substring(1)));
        }

        internal static string ToSpecialEncodeForXML(this string text)
        {
            if ((text == null) || (text == ""))
            {
                return text;
            }
            if (_specialPattern.Match(text) != null)
            {
                text = Regex.Replace(text, "_x00[0-1][0-9A-Fa-f]_", new MatchEvaluator(StringExtension.EncodeEvaluator));
            }
            sb.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if ((ch <= '\x001f') && (ch != '\n'))
                {
                    sb.Append(_specialCharTable[ch]);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        internal static string TrimEndIfNeeded(this string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                int index = source.IndexOf('\0');
                if (index != -1)
                {
                    return source.Substring(0, index);
                }
            }
            return source;
        }
    }
}

