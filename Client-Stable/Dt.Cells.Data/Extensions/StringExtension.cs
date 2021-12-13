#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class StringExtension
    {
        public static bool InvariantEquals(this string This, string to, CompareOptions options)
        {
            return (CultureInfo.InvariantCulture.CompareInfo.Compare(This, to, options) == 0);
        }

        public static string ToLower(this string This, CultureInfo culture)
        {
            if (culture == null)
            {
                return This.ToLowerInvariant();
            }
            return culture.TextInfo.ToLower(This);
        }

        public static string ToUpper(this string This, CultureInfo culture)
        {
            if (culture == null)
            {
                return This.ToUpperInvariant();
            }
            return culture.TextInfo.ToUpper(This);
        }
    }
}

