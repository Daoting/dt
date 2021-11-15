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
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a number format.  
    /// </summary>
    internal sealed class NumberFormatDigital : NumberFormatBase
    {
        string constString;
        /// <summary>
        /// the default converter.
        /// </summary>
        static readonly INumberStringConverter defaultNumberStringConverter = new DefaultNumberStringConverter();
        /// <summary>
        /// the excel format string.
        /// </summary>
        string excelFormatString;
        /// <summary>
        /// denominator format.
        /// </summary>
        string fractionDenominatorFormat;
        /// <summary>
        /// integer format.
        /// </summary>
        string fractionIntegerFormat;
        /// <summary>
        /// numerator format.
        /// </summary>
        string fractionNumeratorFormat;
        /// <summary>
        /// 
        /// </summary>
        string fullFormatString;
        /// <summary>
        /// whether has "General" keyword.
        /// </summary>
        bool isGeneralNumber;
        /// <summary>
        /// the date time keyword
        /// </summary>
        static readonly string[] keywords = new string[] { DefaultTokens.Exponential1, DefaultTokens.Exponential2, ((char) DefaultTokens.NumberSign).ToString(), DefaultTokens.DecimalSeparator, DefaultTokens.NumberGroupSeparator, DefaultTokens.PercentSymbol, ((char) DefaultTokens.Zero).ToString(), ((char) DefaultTokens.SolidusSign).ToString() };
        static readonly double maxEdittingNumber = 1E+20;
        static readonly double minDisplayNumber = 1E-09;
        static readonly double minEdittingNumber = 1E-19;
        /// <summary>
        /// the valid date time format string.
        /// </summary>
        string numberFormatString;
        /// <summary>
        /// regex for number.
        /// </summary>
        static Regex numberRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.NumberFormatDigital" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="partLocaleID">The part locale ID.</param>
        /// <param name="dbNumberFormatPart">The number format part.</param>
        /// <param name="culture">The culture.</param>
        public NumberFormatDigital(string format, object partLocaleID, object dbNumberFormatPart, CultureInfo culture) : base(partLocaleID, dbNumberFormatPart, culture)
        {
            this.excelFormatString = string.Empty;
            string s = NumberFormatBase.TrimNotSupportSymbol(format);
            this.fullFormatString = DefaultTokens.Filter(s, DefaultTokens.LeftSquareBracket, DefaultTokens.RightSquareBracket);
            this.excelFormatString = s;
            if (partLocaleID != null)
            {
                string str2 = s;
                s = DefaultTokens.ReplaceKeyword(str2, base.PartLocaleID.OriginalToken, base.PartLocaleID.CurrencySymbol);
            }
            if (base.PartDBNumberFormat != null)
            {
                this.excelFormatString = DefaultTokens.ReplaceKeyword(this.excelFormatString, base.PartDBNumberFormat.OriginalToken, base.PartDBNumberFormat.ToString());
            }
            s = DefaultTokens.Filter(s, DefaultTokens.LeftSquareBracket, DefaultTokens.RightSquareBracket);
            if (s.IndexOf(DefaultTokens.SolidusSign) > -1)
            {
                s = s.Replace(DefaultTokens.QuestionMark, DefaultTokens.Zero);
                string[] strArray = DefaultTokens.Split(s, DefaultTokens.SolidusSign);
                if ((strArray != null) && (strArray.Length == 2))
                {
                    this.fractionDenominatorFormat = strArray[1];
                    string[] integerAndNumeratorParts = this.GetIntegerAndNumeratorParts(strArray[0]);
                    if ((integerAndNumeratorParts != null) && (integerAndNumeratorParts.Length == 3))
                    {
                        this.fractionIntegerFormat = integerAndNumeratorParts[0];
                        this.constString = integerAndNumeratorParts[1];
                        this.fractionNumeratorFormat = integerAndNumeratorParts[2];
                    }
                }
            }
            this.numberFormatString = s;
        }

        /// <summary>
        /// Encodes the number format.
        /// </summary>
        /// <param name="format">The format to encode.</param>
        /// <returns>Returns the encoded format string.</returns>
        string EncodeNumberFormat(string format)
        {
            format = format.Replace(@"\\", @"'\'");
            format = format.Replace(@"\ ", " ");
            return format;
        }

        /// <summary>
        /// Determines whether the format string is valid.
        /// </summary>
        /// <param name="format">The token to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
        /// </returns>
        public static bool EvaluateFormat(string format)
        {
            return NumberFormatBase.ContainsKeywords(format, keywords);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the string of the formatted value.</returns>
        public override string Format(object obj)
        {
            if (obj is bool)
            {
                return FormatConverter.ToString(obj, true);
            }
            double? nullable = FormatConverter.TryDouble(obj, true);
            if (!nullable.HasValue)
            {
                if (obj is string)
                {
                    return (string) (obj as string);
                }
                return null;
            }
            double num = nullable.Value;
            string naNSymbol = base.NaNSymbol;
            if ((this.fractionNumeratorFormat != null) && (this.fractionDenominatorFormat != null))
            {
                double num2;
                double num3;
                double num4;
                double num13;
                int length = this.fractionDenominatorFormat.Length;
                if (!GetFraction(num, length, out num2, out num3, out num4))
                {
                    return ((double) num).ToString((IFormatProvider) this.NumberFormatInfo);
                }
                double mincCommonMultiple = this.GetMincCommonMultiple(num3, num4);
                if (mincCommonMultiple > 1.0)
                {
                    num3 /= mincCommonMultiple;
                    num4 /= mincCommonMultiple;
                }
                if (this.fractionIntegerFormat != null)
                {
                    double num7;
                    double num9;
                    StringBuilder builder = new StringBuilder();
                    if (num2 != 0.0)
                    {
                        builder.Append(((double) num2).ToString(this.fractionIntegerFormat, (IFormatProvider) this.NumberFormatInfo));
                    }
                    if (this.constString != null)
                    {
                        builder.Append(this.constString);
                    }
                    if ((num2 == 0.0) && (num < 0.0))
                    {
                        builder.Append(base.NegativeSign);
                    }
                    if (num == 0.0)
                    {
                        builder.Append("0");
                    }
                    string fractionDenominatorFormat = this.fractionDenominatorFormat;
                    if (double.TryParse(fractionDenominatorFormat, out num7) && (num7 > 0.0))
                    {
                        num3 *= num7 / num4;
                        fractionDenominatorFormat = string.Empty;
                        num4 = num7;
                        double num8 = Math.Ceiling(num3) - num3;
                        if ((num8 <= 0.5) && (num8 > 0.0))
                        {
                            num3 = ((int) num3) + 1;
                        }
                        else
                        {
                            num3 = (int) num3;
                        }
                    }
                    string fractionNumeratorFormat = this.fractionNumeratorFormat;
                    if (double.TryParse(fractionNumeratorFormat, out num9) && (num9 == 0.0))
                    {
                        int num10 = fractionNumeratorFormat.Length;
                        string str4 = ((double) num3).ToString();
                        int num11 = str4.Length;
                        if (num10 > num11)
                        {
                            fractionNumeratorFormat = fractionNumeratorFormat.Substring(0, fractionNumeratorFormat.Length - (num10 - num11));
                        }
                        else if (num10 < num11)
                        {
                            num3 = int.Parse(str4.Substring(0, num10));
                        }
                    }
                    if (num3 != 0.0)
                    {
                        builder.Append(((double) num3).ToString(fractionNumeratorFormat, (IFormatProvider) this.NumberFormatInfo).TrimStart(new char[] { '0' }));
                        builder.Append(DefaultTokens.SolidusSign);
                        builder.Append(((double) num4).ToString(fractionDenominatorFormat, (IFormatProvider) this.NumberFormatInfo).TrimStart(new char[] { '0' }));
                    }
                    return builder.ToString();
                }
                StringBuilder builder2 = new StringBuilder();
                double a = (num2 * num4) + num3;
                if (double.TryParse(this.fractionDenominatorFormat, out num13) && (num13 > 0.0))
                {
                    a *= num13 / num4;
                    num4 = num13;
                    double num14 = Math.Ceiling(a) - a;
                    if ((num14 <= 0.5) && (num14 > 0.0))
                    {
                        a = ((int) a) + 1;
                    }
                    else
                    {
                        a = (int) a;
                    }
                    builder2.Append(string.Format("{0}/{1}", (object[]) new object[] { ((double) a), ((double) num4) }));
                }
                else
                {
                    builder2.Append(((double) a).ToString(this.fractionNumeratorFormat, (IFormatProvider) this.NumberFormatInfo).TrimStart(new char[] { '0' }));
                    builder2.Append(DefaultTokens.SolidusSign);
                    builder2.Append(((double) num4).ToString(this.fractionDenominatorFormat, (IFormatProvider) this.NumberFormatInfo).TrimStart(new char[] { '0' }));
                }
                return builder2.ToString();
            }
            naNSymbol = ((double) num).ToString(this.EncodeNumberFormat(this.numberFormatString), (IFormatProvider) this.NumberFormatInfo);
            if (this.NumberStringConverter != null)
            {
                naNSymbol = this.NumberStringConverter.ConvertTo(naNSymbol, obj, this.isGeneralNumber, base.PartLocaleID, base.PartDBNumberFormat);
            }
            return naNSymbol;
        }

        /// <summary>
        /// Gets the fraction.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="denominatorDigits">The denominator digits.</param>
        /// <param name="integer">The integer.</param>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        /// <returns>Returns the fraction.</returns>
        public static bool GetFraction(double value, int denominatorDigits, out double integer, out double numerator, out double denominator)
        {
            return NumberHelper.GetFraction(value, denominatorDigits, out integer, out numerator, out denominator);
        }

        string[] GetIntegerAndNumeratorParts(string s)
        {
            if ((s == null) || (s == string.Empty))
            {
                return null;
            }
            char ch = '"';
            bool flag = false;
            bool flag2 = false;
            string[] strArray = new string[3];
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char ch2 = s[i];
                if ((ch2 == ch) && !flag)
                {
                    flag2 = !flag2;
                }
                if (!flag && (flag2 || (ch2 == ' ')))
                {
                    if (ch2 != ch)
                    {
                        builder3.Append(ch2);
                    }
                }
                else if (builder3.Length > 0)
                {
                    if (ch2 != ch)
                    {
                        builder2.Append(ch2);
                    }
                }
                else
                {
                    builder.Append(ch2);
                }
                if (ch2 == DefaultTokens.ReverseSolidusSign)
                {
                    flag = !flag;
                }
                else
                {
                    flag = false;
                }
            }
            if ((builder2.Length == 0) && (builder.Length > 0))
            {
                builder2 = builder;
                builder = null;
            }
            if (builder != null)
            {
                strArray[0] = builder.ToString();
            }
            if (builder3 != null)
            {
                strArray[1] = builder3.ToString();
            }
            if (builder2 != null)
            {
                strArray[2] = builder2.ToString();
            }
            return strArray;
        }

        double GetMincCommonMultiple(double value1, double value2)
        {
            if (value1 != 0.0)
            {
                if (value2 == 0.0)
                {
                    return Math.Abs(value1);
                }
                double num = 0.0;
                if (value1 < value2)
                {
                    num = value1;
                    value1 = value2;
                    value2 = num;
                }
                for (num = value1 % value2; num != 0.0; num = value1 % value2)
                {
                    value1 = value2;
                    value2 = num;
                }
            }
            return Math.Abs(value2);
        }

        Regex InitRegex()
        {
            int num = (this.NumberFormatInfo.CurrencyGroupSizes.Length > 1) ? this.NumberFormatInfo.CurrencyGroupSizes[1] : this.NumberFormatInfo.CurrencyGroupSizes[0];
            return new Regex(string.Format(@"^(\-|\+)?([0-9]{{1,{4}}})*(\{0}[0-9]{{{2}}})*(\{0}[0-9]{{{3}}})*(\{1})?([0-9]+)*$", (object[]) new object[] { (string.IsNullOrWhiteSpace(this.NumberFormatInfo.CurrencyGroupSeparator) ? (string.IsNullOrEmpty(this.NumberFormatInfo.CurrencyGroupSeparator) ? "*" : "s") : this.NumberFormatInfo.CurrencyGroupSeparator), this.NumberFormatInfo.CurrencyDecimalSeparator, ((int) num), ((int) this.NumberFormatInfo.CurrencyGroupSizes[0]), ((int) 100) }), (RegexOptions) RegexOptions.CultureInvariant);
        }

        bool IsCurrency(string value, out string result)
        {
            result = string.Empty;
            string currencySymbol = this.NumberFormatInfo.CurrencySymbol;
            int index = value.IndexOf(currencySymbol);
            if ((index == -1) || ((index != 0) && (index != (value.Length - currencySymbol.Length))))
            {
                return false;
            }
            result = value.Remove(index, currencySymbol.Length);
            return (result.IndexOf(currencySymbol) == -1);
        }

        /// <summary>
        /// Determines whether the specified character is a special symbol.
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>
        /// <c>true</c> if the character is a special symbol; otherwise, <c>false</c>.
        /// </returns>
        bool IsSpecialSymbol(char c)
        {
            if (this.IsStandardNumberSymbol(c))
            {
                return false;
            }
            return char.IsWhiteSpace(c);
        }

        /// <summary>
        /// Determines whether the specified character is the standard number symbol.
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>
        /// <c>true</c> if the character is the standard number symbol; otherwise, <c>false</c>.
        /// </returns>
        bool IsStandardNumberSymbol(char c)
        {
            System.Globalization.NumberFormatInfo info = (this.NumberFormatInfo != null) ? this.NumberFormatInfo : DefaultTokens.NumberFormatInfo;
            if (info != null)
            {
                string str = ((char) c).ToString();
                if (((((str == info.CurrencyDecimalSeparator) || (str == info.CurrencyGroupSeparator)) || ((str == info.CurrencySymbol) || (str == info.NaNSymbol))) || (((str == info.NegativeInfinitySymbol) || (str == info.NegativeSign)) || ((str == info.NumberDecimalSeparator) || (str == info.NumberGroupSeparator)))) || ((((str == info.PercentDecimalSeparator) || (str == info.PercentGroupSeparator)) || ((str == info.PercentSymbol) || (str == info.PerMilleSymbol))) || ((str == info.PositiveInfinitySymbol) || (str == info.PositiveSign))))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parses the specified format.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>Returns the value.</returns>
        public override object Parse(string s)
        {
            s = this.TrimSpecialSymbol(s);
            if ((s != null) && (s != string.Empty))
            {
                bool flag;
                string str = NumberHelper.FixJapaneseChars(s);
                if (bool.TryParse(s, out flag))
                {
                    return (bool) flag;
                }
                bool flag2 = DefaultTokens.IsDecimal(str, this.NumberFormatInfo);
                bool flag3 = str.IndexOf(DefaultTokens.ExponentialSymbol, (StringComparison) StringComparison.CurrentCultureIgnoreCase) > -1;
                bool flag4 = (str[0] == DefaultTokens.LeftParenthesis) && (str[str.Length - 1] == DefaultTokens.RightParenthesis);
                if (this.numberFormatString != null)
                {
                    double num2;
                    int count = 0;
                    s = this.TrimPercentSign(s, out count);
                    if (this.TryParse(s, out num2, flag3 | flag4))
                    {
                        if (count > 0)
                        {
                            num2 /= 100.0 * count;
                        }
                        if ((num2 != 0.0) && (Math.Abs((double) (num2 - Math.Floor(num2))) != 0.0))
                        {
                            flag2 = true;
                        }
                        if (flag3)
                        {
                            flag2 = true;
                        }
                        if (!flag2)
                        {
                            int num3;
                            long num4;
                            if (NumberHelper.TryInteger(num2, out num3))
                            {
                                return (int) num3;
                            }
                            if (NumberHelper.TryLong(num2, out num4))
                            {
                                return (long) num4;
                            }
                        }
                        return (double) num2;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Toes the object.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="isDecimal">if set to <c>true</c> to convert decimal value</param>
        /// <returns>Return the value</returns>
        object ToObject(double value, bool isDecimal)
        {
            if (!isDecimal)
            {
                if ((value <= 2147483647.0) && (value >= -2147483648.0))
                {
                    return (int) ((int) value);
                }
                if ((value <= 9.2233720368547758E+18) && (value >= -9.2233720368547758E+18))
                {
                    return (long) ((long) value);
                }
            }
            return (double) value;
        }

        /// <summary>
        /// Trims the number string.
        /// </summary>
        /// <param name="s">The string to trim</param>
        /// <param name="count">The number of percent sign.</param>
        /// <returns>Returns the trimmed string</returns>
        string TrimPercentSign(string s, out int count)
        {
            count = 0;
            if ((s == null) || (s == string.Empty))
            {
                return s;
            }
            string str = s;
            str = str.Replace(DefaultTokens.PercentSymbol, "");
            count += (s.Length - str.Length) / DefaultTokens.PercentSymbol.Length;
            return str;
        }

        /// <summary>
        /// Trims the special symbol.
        /// </summary>
        /// <param name="s">The string to trim</param>
        /// <returns>Returns the trimmed string.</returns>
        string TrimSpecialSymbol(string s)
        {
            if ((s == null) || (s == string.Empty))
            {
                return s;
            }
            string str = s;
            int num = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsDigit(str[i]))
                {
                    num = i;
                    break;
                }
            }
            int num3 = -1;
            for (int j = str.Length - 1; j > -1; j--)
            {
                if (char.IsDigit(str[j]))
                {
                    num3 = j;
                    break;
                }
            }
            for (int k = str.Length - 1; k > -1; k--)
            {
                char c = str[k];
                if (this.IsSpecialSymbol(c))
                {
                    if (char.IsWhiteSpace(c))
                    {
                        if ((k < num) || (num3 < k))
                        {
                            str = str.Remove(k, 1);
                        }
                    }
                    else
                    {
                        str = str.Remove(k, 1);
                    }
                }
                else if (((c == '-') || (c == '+')) && (((k > 0) && (str[k - 1] != 'e')) && ((str[k - 1] != 'E') && (str[k - 1] != '('))))
                {
                    char ch2 = str[k - 1];
                    if ((((char) ch2).ToString() != DefaultTokens.NumberFormatInfo.CurrencySymbol) && (str[k - 1] == '-'))
                    {
                        str = str.Remove(k - 1, 2);
                        k--;
                    }
                }
            }
            return str;
        }

        bool TryParse(string value, out double result, bool forceParse)
        {
            result = 0.0;
            if (string.IsNullOrEmpty(value) || (this.NumberFormatInfo == null))
            {
                return false;
            }
            string str = null;
            if (this.IsCurrency(value, out str))
            {
                value = str;
            }
            bool flag2 = forceParse;
            if (!flag2 && (value != null))
            {
                flag2 = true;
                foreach (char ch in value)
                {
                    if (char.IsLetter(ch) && (char.ToUpperInvariant(ch) != 'E'))
                    {
                        flag2 = false;
                        break;
                    }
                }
                if (flag2)
                {
                    flag2 = this.NumberRegex.IsMatch(value.Trim());
                }
            }
            return (flag2 && double.TryParse(value, ((NumberStyles)NumberStyles.AllowCurrencySymbol) | (((NumberStyles)NumberStyles.Float) | (((NumberStyles)NumberStyles.AllowThousands) | ((NumberStyles)NumberStyles.AllowParentheses))), (IFormatProvider)this.NumberFormatInfo, out result));
        }

        /// <summary>
        /// Gets the Excel-compatible format string.
        /// </summary>
        /// <value>The Excel-compatible format string.</value>
        internal override string ExcelCompatibleFormatString
        {
            get { return  this.excelFormatString; }
        }

        /// <summary>
        /// Gets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public override string FormatString
        {
            get { return  this.fullFormatString; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this instance is a general number.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a general number; otherwise, <c>false</c>.
        /// </value>
        public bool IsGeneralNumber
        {
            get { return  this.isGeneralNumber; }
            set { this.isGeneralNumber = value; }
        }

        /// <summary>
        /// Gets the maximum editing number.
        /// </summary>
        public static double MaxEdittingNumber
        {
            get { return  maxEdittingNumber; }
        }

        /// <summary>
        /// Gets the minimum display number.
        /// </summary>
        public static double MinDisplayNumber
        {
            get { return  minDisplayNumber; }
        }

        /// <summary>
        /// Gets the minimum editing display number.
        /// </summary>
        public static double MinEdittingNumber
        {
            get { return  minEdittingNumber; }
        }

        /// <summary>
        /// Gets the format provider.
        /// </summary>
        /// <value>The format provider.</value>
        public override System.Globalization.NumberFormatInfo NumberFormatInfo
        {
            get
            {
                if (base.NumberFormatInfo != null)
                {
                    return base.NumberFormatInfo;
                }
                if ((base.PartLocaleID != null) && (base.PartLocaleID.CultureInfo != null))
                {
                    return base.PartLocaleID.CultureInfo.NumberFormat;
                }
                if (base.Culture != null)
                {
                    return base.Culture.NumberFormat;
                }
                return DefaultTokens.NumberFormatInfo;
            }
            set
            {
                base.NumberFormatInfo = value;
                numberRegex = this.InitRegex();
            }
        }

        Regex NumberRegex
        {
            get
            {
                if (numberRegex == null)
                {
                    numberRegex = this.InitRegex();
                }
                return numberRegex;
            }
        }

        /// <summary>
        /// Gets or sets the number string converter.
        /// </summary>
        /// <value>The number string converter.</value>
        public override INumberStringConverter NumberStringConverter
        {
            get
            {
                if (base.NumberStringConverter != null)
                {
                    return base.NumberStringConverter;
                }
                return defaultNumberStringConverter;
            }
            set { base.NumberStringConverter = value; }
        }
    }
}

