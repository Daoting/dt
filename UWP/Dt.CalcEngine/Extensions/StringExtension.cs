#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Globalization;

#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// string扩展类
    /// </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// 按区域转小写
        /// </summary>
        /// <param name="This"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToLower(this string This, CultureInfo culture)
        {
            if (culture == null)
            {
                return This.ToLowerInvariant();
            }
            return culture.TextInfo.ToLower(This);
        }

        /// <summary>
        /// 按区域转大写
        /// </summary>
        /// <param name="This"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToUpper(this string This, CultureInfo culture)
        {
            if (culture == null)
            {
                return This.ToUpperInvariant();
            }
            return culture.TextInfo.ToUpper(This);
        }

        /// <summary>
        /// 字符串比较
        /// </summary>
        /// <param name="This"></param>
        /// <param name="to"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool InvariantEquals(this string This, string to, CompareOptions options)
        {
            return (CultureInfo.InvariantCulture.CompareInfo.Compare(This, to, options) == 0);
        }
    }
}

