#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Xls
{
    internal class UrlHelper
    {
        public static string Decode(string str)
        {
            return Regex.Replace(str.Replace('+', ' '), "%[0-9a-zA-Z][0-9a-zA-Z]", new MatchEvaluator(UrlHelper.DecodeEvaluator));
        }

        public static string DecodeEvaluator(Match match)
        {
            return ((char) Convert.ToChar(int.Parse(match.Value.Substring(1), (NumberStyles) NumberStyles.HexNumber))).ToString();
        }

        public static string Encode(string str)
        {
            string str2 = string.Format("0-9a-zA-Z{0}", (object[]) new object[] { Regex.Escape(":/-_.!~*'()-") });
            return Regex.Replace(str, string.Format("[^{0}]", (object[]) new object[] { str2 }), new MatchEvaluator(UrlHelper.EncodeEvaluator));
        }

        public static string EncodeEvaluator(Match match)
        {
            if (match.Value != " ")
            {
                return string.Format("%{0:X2}", (object[]) new object[] { ((int) Convert.ToInt32(match.Value[0])) });
            }
            return "+";
        }
    }
}

