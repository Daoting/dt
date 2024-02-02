#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
#endregion 

namespace Dt.Core.Mask
{
    /// <summary>
    /// 数值格式化
    /// </summary>
    public class NumericFormatter
    {
        #region 静态内容
        static string CreateCurrencyFormat(int precision, CultureInfo culture)
        {
            string str2;
            string str3;
            if (precision < 0)
            {
                precision = culture.NumberFormat.CurrencyDecimalDigits;
            }
            int num = 0x1d - precision;
            if (num < 1)
            {
                num = 1;
            }
            string str = new string('#', num - 1) + ",0";
            if (precision > 0)
            {
                str = str + '.' + new string('0', precision);
            }
            switch (culture.NumberFormat.CurrencyPositivePattern)
            {
                case 1:
                    str2 = "{0}$";
                    break;

                case 2:
                    str2 = "$ {0}";
                    break;

                case 3:
                    str2 = "{0} $";
                    break;

                default:
                    str2 = "${0}";
                    break;
            }
            switch (culture.NumberFormat.CurrencyNegativePattern)
            {
                case 1:
                    str3 = "{1}${0}";
                    break;

                case 2:
                    str3 = "${1}{0}";
                    break;

                case 3:
                    str3 = "${0}{1}";
                    break;

                case 4:
                    str3 = "({0}$)";
                    break;

                case 5:
                    str3 = "{1}{0}$";
                    break;

                case 6:
                    str3 = "{0}{1}$";
                    break;

                case 7:
                    str3 = "{0}${1}";
                    break;

                case 8:
                    str3 = "{1}{0} $";
                    break;

                case 9:
                    str3 = "{1}$ {0}";
                    break;

                case 10:
                    str3 = "{0} ${1}";
                    break;

                case 11:
                    str3 = "$ {0}{1}";
                    break;

                case 12:
                    str3 = "$ {1}{0}";
                    break;

                case 13:
                    str3 = "{0}{1} $";
                    break;

                case 14:
                    str3 = "($ {0})";
                    break;

                case 15:
                    str3 = "({0} $)";
                    break;

                default:
                    str3 = "(${0})";
                    break;
            }
            return string.Format(CultureInfo.InvariantCulture, str2 + ';' + str3, new object[] { str, GetNegativeSymbolPattern(culture) });
        }

        static string CreateDecimalFormat(int precision, CultureInfo culture)
        {
            string str;
            if (precision <= 0)
            {
                str = new string('#', 0x1c) + "0";
            }
            else
            {
                str = new string('0', precision);
            }
            return string.Format(CultureInfo.InvariantCulture, "{0};{1}{0}", new object[] { str, GetNegativeSymbolPattern(culture) });
        }

        static string CreateFixedPointFormat(int precision, CultureInfo culture)
        {
            if (precision < 0)
            {
                precision = culture.NumberFormat.NumberDecimalDigits;
            }
            int num = 0x1d - precision;
            if (num < 1)
            {
                num = 1;
            }
            string str = new string('#', num - 1) + "0";
            if (precision > 0)
            {
                str = str + '.' + new string('0', precision);
            }
            return string.Format(CultureInfo.InvariantCulture, "{0};{1}{0}", new object[] { str, GetNegativeSymbolPattern(culture) });
        }

        static string CreateFullNumberFormatFromPositiveFormat(string numberFormat, CultureInfo culture)
        {
            string str;
            switch (culture.NumberFormat.NumberNegativePattern)
            {
                case 1:
                    str = "{1}{0}";
                    break;

                case 2:
                    str = "{1} {0}";
                    break;

                case 3:
                    str = "{0}{1}";
                    break;

                case 4:
                    str = "{0} {1}";
                    break;

                default:
                    str = "({0})";
                    break;
            }
            return string.Format(CultureInfo.InvariantCulture, "{0};" + str, new object[] { numberFormat, GetNegativeSymbolPattern(culture) });
        }

        static string CreateNumberFormat(int precision, CultureInfo culture)
        {
            if (precision < 0)
            {
                precision = culture.NumberFormat.NumberDecimalDigits;
            }
            int num = 0x1d - precision;
            if (num < 1)
            {
                num = 1;
            }
            string numberFormat = new string('#', num - 1) + ",0";
            if (precision > 0)
            {
                numberFormat = numberFormat + '.' + new string('0', precision);
            }
            return CreateFullNumberFormatFromPositiveFormat(numberFormat, culture);
        }

        static string CreatePercentFormat(int precision, CultureInfo culture, string percentSymbol)
        {
            string str2;
            string str3;
            if (precision < 0)
            {
                precision = culture.NumberFormat.PercentDecimalDigits;
            }
            int num = 0x1d - precision;
            if (num < 1)
            {
                num = 1;
            }
            string str = new string('#', num - 1) + ",0";
            if (precision > 0)
            {
                str = str + '.' + new string('0', precision);
            }
            switch (culture.NumberFormat.PercentPositivePattern)
            {
                case 1:
                    str2 = "{0}{1}";
                    break;

                case 2:
                    str2 = "{1}{0}";
                    break;

                default:
                    str2 = "{0} {1}";
                    break;
            }
            switch (culture.NumberFormat.PercentNegativePattern)
            {
                case 1:
                    str3 = "{2}{0}{1}";
                    break;

                case 2:
                    str3 = "{2}{1}{0}";
                    break;

                default:
                    str3 = "{2}{0} {1}";
                    break;
            }
            return string.Format(CultureInfo.InvariantCulture, str2 + ';' + str3, new object[] { str, percentSymbol, GetNegativeSymbolPattern(culture) });
        }

        /// <summary>
        /// 根据掩码表达式构造不同类型的格式串
        /// </summary>
        /// <param name="formatString">掩码表达式</param>
        /// <param name="culture">区域信息</param>
        /// <returns>格式字符串</returns>
        public static string Expand(string formatString, CultureInfo culture)
        {
            int num2;
            if ((formatString == null) || (formatString.Length == 0))
            {
                formatString = new string('#', 0x1d) + ",0." + new string('#', 30);
            }
            if (Regex.IsMatch(formatString, "^[cCdDgGfFnNpP][0-9]{0,2}$"))
            {
                int precision = (formatString.Length > 1) ? Convert.ToInt32(formatString.Substring(1)) : -1;
                switch (formatString[0])
                {
                    case 'C':
                    case 'c':
                        formatString = CreateCurrencyFormat(precision, culture);
                        break;
                    case 'D':
                    case 'd':
                        formatString = CreateDecimalFormat(precision, culture);
                        break;
                    case 'F':
                    case 'G':
                    case 'f':
                    case 'g':
                        formatString = CreateFixedPointFormat(precision, culture);
                        break;
                    case 'N':
                    case 'n':
                        formatString = CreateNumberFormat(precision, culture);
                        break;
                    case 'P':
                        formatString = CreatePercentFormat(precision, culture, "%%");
                        break;
                    case 'p':
                        formatString = CreatePercentFormat(precision, culture, "%");
                        break;
                    default:
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Internal error: non-covered case '{0}'", new object[] { formatString }));
                }
            }

            num2 = formatString.Replace(@"\\", "//").Replace(@"\;", "/:").IndexOf(';');
            if (num2 < 0)
            {
                formatString = CreateFullNumberFormatFromPositiveFormat(formatString, culture);
                return formatString;
            }
            if (num2 == (formatString.Length - 1))
            {
                formatString = formatString.Substring(0, formatString.Length - 1);
            }
            return formatString;
        }

        static string GetNegativeSymbolPattern(CultureInfo culture)
        {
            string str = string.Empty;
            foreach (char ch in culture.NumberFormat.NegativeSign)
            {
                str = str + '\\' + ch;
            }
            return str;
        }
        #endregion

        #region 成员变量
        // 十进制小数点.
        readonly string _decimalSeparator;
        readonly List<string> _formatMask = new List<string>();
        readonly string _groupSeparator;
        readonly int[] _groupSizes;

        /// <summary>
        /// 
        /// </summary>
        public readonly bool _Is100Multiplied;

        // 小数点后的最多位数
        readonly int _maxDigitsAfterDecimalSeparator;
        // 小数点后的最小位数
        readonly int _minDigitsAfterDecimalSeparator;
        // 小数点前的最多位数
        readonly int _maxDigitsBeforeDecimalSeparator;
        // 小数点前的最小位数
        readonly int _minDigitsBeforeDecimalSeparator;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="formatString">格式串</param>
        /// <param name="formattingCulture"></param>
        public NumericFormatter(string formatString, CultureInfo formattingCulture)
        {
            // 是否已过小数点.
            bool passSeparator = false;
            // 是否存在逗号分隔符','
            bool exitComma = false;
            NumericMaskSubType number = NumericMaskSubType.Number;

            using (CharEnumerator enumerator = new CharEnumerator(formatString))
            {
                while (enumerator.MoveNext())
                {
                    switch (enumerator.Current)
                    {
                        case '#':
                            // 若为小数点前
                            if (!passSeparator)
                            {
                                this._maxDigitsBeforeDecimalSeparator++;
                                this._formatMask.Add(null);
                                continue;
                            }
                            // 小数点后
                            this._maxDigitsAfterDecimalSeparator++;
                            this._formatMask.Add(null);
                            continue;
                        case '$':
                            number = NumericMaskSubType.Currency;
                            this._formatMask.Add(formattingCulture.NumberFormat.CurrencySymbol);
                            continue;
                        case '%':
                            CharEnumerator enumerator2 = (CharEnumerator)enumerator.Clone();
                            if (!enumerator2.MoveNext() || (enumerator2.Current != '%'))
                            {
                                // 百分数
                                number = NumericMaskSubType.Percent;
                                this._formatMask.Add(formattingCulture.NumberFormat.PercentSymbol);
                                continue;
                            }
                            enumerator.MoveNext();
                            // 百百分数
                            number = NumericMaskSubType.PercentPercent;
                            this._formatMask.Add(formattingCulture.NumberFormat.PercentSymbol);
                            continue;
                        case ',':
                            // 若为小数点后
                            if (passSeparator)
                            {
                                break;
                            }
                            exitComma = true;
                            continue;
                        case '.':
                            // 若为小数点后
                            if (passSeparator)
                            {
                                break;
                            }
                            passSeparator = true;
                            this._formatMask.Add(null);
                            continue;
                        case '0':
                            // 若为小数点前
                            if (!passSeparator)
                            {
                                this._maxDigitsBeforeDecimalSeparator++;
                                this._minDigitsBeforeDecimalSeparator++;
                                this._formatMask.Add(null);
                                continue;
                            }
                            this._maxDigitsAfterDecimalSeparator++;
                            this._minDigitsAfterDecimalSeparator++;
                            this._formatMask.Add(null);
                            continue;
                        case '\\':
                            if (!enumerator.MoveNext())
                            {
                                continue;
                            }
                            break;
                        default:
                            break;
                    }
                    // 若为非特殊字符或
                    // 已过小数点(小数点后)，且当前字符不是 '# $ %'
                    char ch = enumerator.Current;

                    // 若上一字符为 '$' 或 '%'则将当前字符添加到上一掩码串
                    if ((this._formatMask.Count > 0) && (this._formatMask[this._formatMask.Count - 1] != null))
                    {
                        this._formatMask[this._formatMask.Count - 1] = this._formatMask[this._formatMask.Count - 1] + ch;
                    }
                    else
                    {
                        this._formatMask.Add(ch.ToString());
                    }
                }
            }

            switch (number)
            {
                case NumericMaskSubType.Currency:
                    this._decimalSeparator = formattingCulture.NumberFormat.CurrencyDecimalSeparator;
                    this._groupSizes = formattingCulture.NumberFormat.CurrencyGroupSizes;
                    this._groupSeparator = formattingCulture.NumberFormat.CurrencyGroupSeparator;
                    break;

                case NumericMaskSubType.Percent:
                case NumericMaskSubType.PercentPercent:
                    this._decimalSeparator = formattingCulture.NumberFormat.PercentDecimalSeparator;
                    this._groupSizes = formattingCulture.NumberFormat.PercentGroupSizes;
                    this._groupSeparator = formattingCulture.NumberFormat.PercentGroupSeparator;
                    break;

                default:
                    this._decimalSeparator = formattingCulture.NumberFormat.NumberDecimalSeparator;
                    this._groupSizes = formattingCulture.NumberFormat.NumberGroupSizes;
                    this._groupSeparator = formattingCulture.NumberFormat.NumberGroupSeparator;
                    break;
            }
            if (!exitComma)
            {
                this._groupSizes = null;
            }
            this._Is100Multiplied = number == NumericMaskSubType.Percent;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int MaxDigitsAfterDecimalSeparator
        {
            get { return this._maxDigitsAfterDecimalSeparator; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxDigitsBeforeDecimalSeparator
        {
            get { return this._maxDigitsBeforeDecimalSeparator; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinDigitsAfterDecimalSeparator
        {
            get { return this._minDigitsAfterDecimalSeparator; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinDigitsBeforeDecimalSeparator
        {
            get { return this._minDigitsBeforeDecimalSeparator; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string Format(string source)
        {
            return this.Format(source, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourcePosition"></param>
        /// <returns></returns>
        public int GetPositionFormatted(string source, int sourcePosition)
        {
            return this.Format(source, sourcePosition).Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">源数据串</param>
        /// <param name="formattedPosition"></param>
        /// <returns></returns>
        public int GetPositionSource(string source, int formattedPosition)
        {
            for (int i = 0; i < source.Length; i++)
            {
                int positionFormatted = this.GetPositionFormatted(source, i);
                if (formattedPosition <= positionFormatted)
                {
                    return i;
                }
            }
            return source.Length;
        }
        #endregion

        #region 内部方法
        string Format(string source, int sourcePositionForTerminate)
        {
            int index = source.IndexOf('.');
            if (index < 0)
            {
                index = source.Length;
            }
            string str = string.Empty;
            int num2 = 0;
            int maxDigitsBeforeDecimalSeparator = this._maxDigitsBeforeDecimalSeparator;
            foreach (string str2 in this._formatMask)
            {
                if (str2 != null)
                {
                    str = str + str2;
                    continue;
                }
                if (num2 == sourcePositionForTerminate)
                {
                    return str;
                }
                if (maxDigitsBeforeDecimalSeparator == 0)
                {
                    num2++;
                    str = str + this._decimalSeparator;
                }
                else
                {
                    if (maxDigitsBeforeDecimalSeparator == this._maxDigitsBeforeDecimalSeparator)
                    {
                        while ((index - num2) > this._maxDigitsBeforeDecimalSeparator)
                        {
                            str = str + source[num2];
                            str = str + this.GetSeparator(index - num2);
                            num2++;
                            if (num2 == sourcePositionForTerminate)
                            {
                                return str;
                            }
                        }
                    }
                    if ((index >= maxDigitsBeforeDecimalSeparator) && (num2 < source.Length))
                    {
                        str = str + source[num2];
                        str = str + this.GetSeparator(index - num2);
                        num2++;
                        if (num2 == sourcePositionForTerminate)
                        {
                            return str;
                        }
                    }
                }
                maxDigitsBeforeDecimalSeparator--;
            }
            return str;
        }
        
        string GetSeparator(int positionFromDecimalSeparator)
        {
            if (((positionFromDecimalSeparator > 1) && (this._groupSizes != null)) && (this._groupSizes.Length != 0))
            {
                int num = 1;
                for (int i = 0; i < (this._groupSizes.Length - 1); i++)
                {
                    int num3 = num + this._groupSizes[i];
                    if (num3 == positionFromDecimalSeparator)
                    {
                        return this._groupSeparator;
                    }
                    if (num3 > positionFromDecimalSeparator)
                    {
                        return string.Empty;
                    }
                    num = num3;
                }
                int num4 = this._groupSizes[this._groupSizes.Length - 1];
                if ((num4 != 0) && (((positionFromDecimalSeparator - num) % num4) == 0))
                {
                    return this._groupSeparator;
                }
            }
            return string.Empty;
        }
        #endregion

        #region 枚举成员
        enum NumericMaskSubType
        {
            Number,
            Currency,
            Percent,
            PercentPercent
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class CharEnumerator : IEnumerator<char>, IEnumerator, IDisposable
    {
        char currentElement;
        int index;
        string str;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public CharEnumerator(string str)
        {
            this.str = str;
            this.index = -1;
        }

        /// <summary>
        /// 克隆数据
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return base.MemberwiseClone();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (this.index < (this.str.Length - 1))
            {
                this.index++;
                this.currentElement = this.str[this.index];
                return true;
            }
            this.index = this.str.Length;
            return false;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            this.currentElement = '\0';
            this.index = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public char Current
        {
            get
            {
                if (this.index == -1)
                {
                    throw new InvalidOperationException(SLEnvironment.GetResourceString("InvalidOperation_EnumNotStarted"));
                }
                if (this.index >= this.str.Length)
                {
                    throw new InvalidOperationException(SLEnvironment.GetResourceString("InvalidOperation_EnumEnded"));
                }
                return this.currentElement;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (this.index == -1)
                {
                    throw new InvalidOperationException(SLEnvironment.GetResourceString("InvalidOperation_EnumNotStarted"));
                }
                if (this.index >= this.str.Length)
                {
                    throw new InvalidOperationException(SLEnvironment.GetResourceString("InvalidOperation_EnumEnded"));
                }
                return this.currentElement;
            }
        }

        static class SLEnvironment
        {
            public static string GetResourceString(string key)
            {
                return "";
            }
        }
    }
}
