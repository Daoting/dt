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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    internal static class StringExtension
    {
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

