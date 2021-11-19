#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Mask;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Core;
using Windows.UI.Notifications;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 掩码解析
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 执行掩码解析
        /// </summary>
        /// <param name="p_srcValue">数据值</param>
        /// <param name="p_maskType">掩码类型</param>
        /// <param name="p_mask">掩码格式</param>
        /// <param name="p_showPlaceHolders">是否显示占位符</param>
        /// <param name="p_placeHolder">占位符样式</param>
        /// <param name="p_saveLiteral">是否保存掩码后的文字</param>
        /// <returns>解析后的表达式</returns>
        public static string ParseMask(object p_srcValue, MaskType p_maskType, string p_mask, bool p_showPlaceHolders, char p_placeHolder, bool p_saveLiteral)
        {
            if (p_srcValue == null)
            {
                return string.Empty;
            }

            switch (p_maskType)
            {
                case MaskType.Numeric:
                    return ParseNumericMask(p_srcValue, p_mask);
                case MaskType.RegEx:
                    return ParseRegExMask(p_srcValue, p_mask, p_showPlaceHolders, p_placeHolder);
                case MaskType.Regular:
                    return ParseRegularMask(p_srcValue, p_mask, p_placeHolder, p_saveLiteral);
                case MaskType.Simple:
                    return ParseSimpleMask(p_srcValue, p_mask, p_placeHolder, p_saveLiteral);
                case MaskType.DateTime:
                    return ParseDateTimeMask(p_srcValue, p_mask);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 解析数字型内容
        /// </summary>
        /// <param name="p_srcValue">掩码类型</param>
        /// <param name="p_mask">掩码格式</param>
        /// <returns>解析后的表达式</returns>
        public static string ParseNumericMask(object p_srcValue, string p_mask)
        {
            p_mask = NumericFormatter.Expand(p_mask, CultureInfo.CurrentCulture);
            int index = p_mask.Replace(@"\\", "//").Replace(@"\;", "/:").IndexOf(';');
            NumericFormatter formatter;
            if (index < 0)
            {
                formatter = new NumericFormatter(p_mask, CultureInfo.CurrentCulture);
            }
            else
            {
                formatter = new NumericFormatter(p_mask.Substring(0, index), CultureInfo.CurrentCulture);
            }
            NumericMaskLogic logic = new NumericMaskLogic(formatter.MaxDigitsBeforeDecimalSeparator, formatter.MinDigitsBeforeDecimalSeparator, formatter.MinDigitsAfterDecimalSeparator, formatter.MaxDigitsAfterDecimalSeparator, CultureInfo.CurrentCulture);

            int maxDigitsAfterDecimalSeparator = formatter.MaxDigitsAfterDecimalSeparator;
            if (formatter._Is100Multiplied)
            {
                maxDigitsAfterDecimalSeparator += 2;
            }
            string format = "{0:f" + maxDigitsAfterDecimalSeparator.ToString(CultureInfo.InvariantCulture) + "}";
            string input = string.Format(CultureInfo.InvariantCulture, format, new object[] { p_srcValue });
            if (formatter._Is100Multiplied)
            {
                input = NumericMaskLogic.Mul100(input);
            }

            MaskLogicResult result;
            string testedString = input.Trim();
            bool isNegative = false;
            if (testedString.StartsWith("-"))
            {
                testedString = testedString.Substring(1);
                isNegative = true;
            }
            if (IsDecimal(testedString))
            {
                result = logic.GetEditResult(testedString, string.Empty, string.Empty, string.Empty);
                if (result != null)
                {
                    result = logic.GetEditResult(string.Empty, string.Empty, result.EditText, string.Empty);
                }
            }
            else
            {
                result = logic.GetEditResult(string.Empty, string.Empty, string.Empty, input);
            }

            if (result == null)
            {
                result = logic.GetEditResult(string.Empty, string.Empty, string.Empty, string.Empty);
            }
            if (!isNegative)
            {
                return formatter.Format(result.EditText);
            }
            return "-" + formatter.Format(result.EditText);
        }

        /// <summary>
        /// 解析正则表达内容
        /// </summary>
        /// <param name="p_srcValue">数据值</param>
        /// <param name="p_mask">掩码格式</param>
        /// <param name="p_showPlaceHolders">是否显示占位符</param>
        /// <param name="p_placeHolder">占位符样式</param>
        /// <returns>解析后的表达式</returns>
        public static string ParseRegExMask(object p_srcValue, string p_mask, bool p_showPlaceHolders, char p_placeHolder)
        {
            RegExpMaskLogic _logic = new RegExpMaskLogic(RegExpDfa.Parse(p_mask, false, CultureInfo.CurrentCulture), false);

            string initialEditText = string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { p_srcValue });
            MaskLogicResult result = _logic.GetReplaceResult(initialEditText ?? string.Empty, string.Empty, string.Empty, string.Empty);
            if (result != null)
                return p_showPlaceHolders ? _logic.GetMaskedText(result.EditText, p_placeHolder) : result.EditText;

            result = _logic.GetReplaceResult(string.Empty, string.Empty, string.Empty, initialEditText ?? string.Empty);
            if (result != null)
                return p_showPlaceHolders ? _logic.GetMaskedText(result.EditText, p_placeHolder) : result.EditText;
            return string.Empty;
        }

        /// <summary>
        /// 解析Regular简单正则表达式
        /// </summary>
        /// <param name="p_srcValue">数据值</param>
        /// <param name="p_mask">掩码格式</param>
        /// <param name="p_placeHolder">占位符样式</param>
        /// <param name="p_saveLiteral">是否保存掩码后的文字</param>
        /// <returns>格式字符串</returns>
        public static string ParseRegularMask(object p_srcValue, string p_mask, char p_placeHolder, bool p_saveLiteral)
        {
            if (p_saveLiteral)
                return p_srcValue.ToString();
            LegacyMaskInfo info = LegacyMaskInfo.GetRegularMaskInfo(p_mask, CultureInfo.CurrentCulture);
            return ParseLegacy(p_srcValue, p_mask, p_placeHolder, info);
        }

        /// <summary>
        /// 解析简单型表达式
        /// </summary>
        /// <param name="p_srcValue">数据值</param>
        /// <param name="p_mask">掩码格式</param>
        /// <param name="p_placeHolder">占位符样式</param>
        /// <param name="p_saveLiteral">是否保存掩码后的文字</param>
        /// <returns>解析后的表达式</returns>
        public static string ParseSimpleMask(object p_srcValue, string p_mask, char p_placeHolder, bool p_saveLiteral)
        {
            if (p_saveLiteral)
                return p_srcValue.ToString();
            LegacyMaskInfo info = LegacyMaskInfo.GetSimpleMaskInfo(p_mask, CultureInfo.CurrentCulture);
            return ParseLegacy(p_srcValue, p_mask, p_placeHolder, info);
        }

        /// <summary>
        /// 解析日期时间型表达式
        /// </summary>
        /// <param name="p_srcValue">数据值</param>
        /// <param name="p_mask">掩码格式</param>
        /// <returns>解析后的表达式</returns>
        public static string ParseDateTimeMask(object p_srcValue, string p_mask)
        {
            DateTimeFormatInfo formatInfo = DateTimeMaskManager.GetGoodCalendarDateTimeFormatInfo(CultureInfo.CurrentCulture);
            DateTimeMaskFormatInfo maskInfo = new DateTimeMaskFormatInfo(p_mask, formatInfo);
            DateTime? dt = null;
            if ((p_srcValue is DateTime) || (p_srcValue == null))
            {
                dt = (DateTime?)p_srcValue;
            }
            else if (p_srcValue is TimeSpan)
            {
                TimeSpan span = (TimeSpan)p_srcValue;
                dt = (DateTime?)new DateTime(span.Ticks);
            }
            else
            {
                try
                {
                    dt = new DateTime?(DateTime.Parse(p_srcValue.ToString(), CultureInfo.InvariantCulture));
                }
                catch
                { }
            }

            if (dt.HasValue)
                return maskInfo.Format(dt.Value);
            return string.Empty;
        }

        /// <summary>
        /// 输入字符串是否为纯数字
        /// </summary>
        /// <param name="testedString">输入字符串</param>
        /// <returns>是否纯数字</returns>
        static bool IsDecimal(string testedString)
        {
            int index = testedString.IndexOf('.');
            for (int i = 0; i < testedString.Length; i++)
            {
                if (i != index)
                {
                    char ch = testedString[i];
                    if ((ch < '0') || (ch > '9'))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 解析简单型表达式
        /// </summary>
        /// <param name="p_srcValue">数据值</param>
        /// <param name="p_mask">掩码格式</param>
        /// <param name="p_placeHolder">占位符样式</param>
        /// <param name="p_info">简单正则表达式信息</param>
        /// <returns>解析后的表达式</returns>
        static string ParseLegacy(object p_srcValue, string p_mask, char p_placeHolder, LegacyMaskInfo p_info)
        {
            LegacyMaskManagerState state = new LegacyMaskManagerState(p_info);
            string initialEditText = string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { p_srcValue });
            state.Insert(initialEditText);
            return state.GetDisplayText(p_placeHolder);
        }
    }
}