#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a General number format.  
    /// </summary>
    internal sealed class NumberFormatGeneral : NumberFormatBase
    {
        static string[] alternativeYearMonthDay = null;
        /// <summary>
        /// the number formatter.
        /// </summary>
        NumberFormatDigital digitalFormat;
        /// <summary>
        /// the formatter for exponential.
        /// </summary>
        NumberFormatDigital exponentialDigitalFormat;
        /// <summary>
        /// the full format string.
        /// </summary>
        string fullFormatString;
        /// <summary>
        /// hour minute
        /// </summary>
        static readonly string[] generalHourMinute = new string[] { "H:m", "h:m tt" };
        /// <summary>
        /// hour minute second
        /// </summary>
        static readonly string[] generalHourMinuteSecond = new string[] { "H:m:s", "h:m:s tt", "H:m:s", "h:mm:ss tt" };
        /// <summary>
        /// hour minute second millisecond 
        /// </summary>
        static readonly string[] generalHourMinuteSecondSubSecond = new string[] { "H:m:s.FFF", "h:m:s.FFF tt" };
        /// <summary>
        /// hour minute second millisecond with date
        /// </summary>
        static string[] generalHourMinuteSecondSubSecondWithDate = null;
        /// <summary>
        /// hour minute second with date
        /// </summary>
        static string[] generalHourMinuteSecondWithDate = null;
        /// <summary>
        /// hour minute with date
        /// </summary>
        static string[] generalHourMinuteWithDate = null;
        /// <summary>
        /// month day
        /// </summary>
        static string[] generalMonthDay = null;
        /// <summary>
        /// 
        /// </summary>
        static readonly string GeneralNumber = "##################0.################";
        /// <summary>
        /// general of placeholder 
        /// </summary>
        static readonly string GeneralPlaceholder = "@NumberFormat";
        /// <summary>
        /// year month
        /// </summary>
        static readonly string[] generalYearMonth = new string[] { "M/y", "MMM/y", "M/yyyy", "MMM/yyyy", "M-y", "MMM-y", "M-yyyy", "MMM-yyyy" };
        /// <summary>
        /// year month day
        /// </summary>
        static string[] generalYearMonthDay = null;

        /// <summary>
        /// Creates a new normal general number format.
        /// </summary>
        public NumberFormatGeneral() : base(null, null, null)
        {
            this.fullFormatString = NumberFormatBase.General;
        }

        /// <summary>
        /// Creates a new general number format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="partLocaleID">The locale ID.</param>
        /// <param name="dbNumberFormatPart">The db number format.</param>
        /// <param name="culture">The culture.</param>
        public NumberFormatGeneral(string format, object partLocaleID, object dbNumberFormatPart, CultureInfo culture) : base(partLocaleID, dbNumberFormatPart, culture)
        {
            if ((format == null) || (format == string.Empty))
            {
                throw new FormatException("format is illegal.");
            }
            if (!EvaluateFormat(format))
            {
                throw new ArgumentException("format is illegal.");
            }
            if ((format.Contains(((char) DefaultTokens.Zero).ToString()) || format.Contains(((char) DefaultTokens.NumberSign).ToString())) || (format.Contains(DefaultTokens.DecimalSeparator) || format.Contains(((char) DefaultTokens.CommercialAt).ToString())))
            {
                throw new FormatException("format is illegal.");
            }
            this.fullFormatString = format;
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
            if ((format == null) || (format == string.Empty))
            {
                return false;
            }
            return NumberFormatBase.ContainsKeywords(format, new string[] { NumberFormatBase.General.ToLower() });
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// Returns the string of the formatted value.
        /// </returns>
        public override string Format(object obj)
        {
            string str = string.Empty;
            if (FormatConverter.IsNumber(obj))
            {
                bool flag = (base.PartLocaleID == null) || base.PartLocaleID.AllowScience;
                double? nullable = FormatConverter.TryDouble(obj, true);
                if (!nullable.HasValue)
                {
                    return "";
                }
                double num = nullable.Value;
                if (((Math.Abs(num) <= 99999999999) || !flag) && ((Math.Abs(num) >= 1E-11) || (num == 0.0)))
                {
                    return this.DigitalFormat.Format(obj);
                }
                return this.ExponentialDigitalFormat.Format(obj);
            }
            if (obj is string)
            {
                string newValue = FormatConverter.ToString(obj, true);
                string str3 = DefaultTokens.TrimEscape(this.FormatString.Replace("\"", ""));
                if (str3 != null)
                {
                    newValue = str3.Replace("General", newValue);
                }
                return newValue;
            }
            if (obj is bool)
            {
                bool flag2 = (bool) ((bool) obj);
                str = flag2.ToString().ToUpper();
            }
            return str;
        }

        /// <summary>
        /// Parses the specified format.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>Returns the value.</returns>
        public override object Parse(string s)
        {
            if ((s == null) || (s == string.Empty))
            {
                return null;
            }
            s = NumberHelper.FixJapaneseChars(s);
            bool flag = false;
            int index = s.IndexOf("-");
            if ((index > 0) && !DefaultTokens.IsEquals(s[index - 1], DefaultTokens.ExponentialSymbol, true))
            {
                flag = true;
            }
            List<string> list = new List<string>(new string[] { "/", ":", "." });
            list.Remove(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            bool flag2 = false;
            if (flag)
            {
                flag2 = true;
            }
            if (!flag2)
            {
                foreach (string str in list)
                {
                    if (s.Contains(str))
                    {
                        flag2 = true;
                        break;
                    }
                }
            }
            if (flag2)
            {
                DateTime time;
                if (DateTime.TryParseExact(s, GeneralMonthDay, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                {
                    return time;
                }
                if (DateTime.TryParseExact(s, generalYearMonth, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                {
                    return time;
                }
                if (DateTime.TryParseExact(s, GeneralYearMonthDay, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                {
                    return time;
                }
                if (DateTime.TryParseExact(s, AlternativeYearMonthDay, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                {
                    return time;
                }
                if (DateTime.TryParseExact(s, generalHourMinute, null, ((DateTimeStyles)DateTimeStyles.NoCurrentDateDefault) | ((DateTimeStyles)DateTimeStyles.AllowWhiteSpaces), out time))
                {
                    if (((time.Year == 1) && (time.Month == 1)) && (time.Day == 1))
                    {
                        return new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond);
                    }
                    return time;
                }
                if (DateTime.TryParseExact(s, generalHourMinuteSecond, null, ((DateTimeStyles)DateTimeStyles.NoCurrentDateDefault) | ((DateTimeStyles)DateTimeStyles.AllowWhiteSpaces), out time))
                {
                    if (((time.Year == 1) && (time.Month == 1)) && (time.Day == 1))
                    {
                        return new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond);
                    }
                    return time;
                }
                if (DateTime.TryParseExact(s, generalHourMinuteSecondSubSecond, null, ((DateTimeStyles)DateTimeStyles.NoCurrentDateDefault) | ((DateTimeStyles)DateTimeStyles.AllowWhiteSpaces), out time))
                {
                    if (((time.Year == 1) && (time.Month == 1)) && (time.Day == 1))
                    {
                        return new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond);
                    }
                    return time;
                }
                if (DateTime.TryParseExact(s, GeneralHourMinuteWithDate, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                {
                    return time;
                }
                if (DateTime.TryParseExact(s, GeneralHourMinuteSecondWithDate, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                {
                    return time;
                }
                if (DateTime.TryParseExact(s, GeneralHourMinuteSecondSubSecondWithDate, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                {
                    return time;
                }
            }
            string str2 = s;
            bool? nullable = null;
            if (str2.Substring(0, 1) == DefaultTokens.NegativeSign)
            {
                nullable = true;
            }
            else if (str2.Substring(0, 1) == DefaultTokens.PositiveSign)
            {
                nullable = false;
            }
            bool flag3 = false;
            if ((nullable.HasValue && (str2.Length > 3)) && ((str2[1] == DefaultTokens.LeftParenthesis) && (str2[str2.Length - 1] == DefaultTokens.RightParenthesis)))
            {
                flag3 = true;
            }
            if (nullable.HasValue && flag3)
            {
                string str3 = s.Remove(0, 1);
                object obj2 = this.DigitalFormat.Parse(str3);
                if (obj2 == null)
                {
                    return s;
                }
                if (obj2 is double)
                {
                    if (nullable.Value)
                    {
                        return (double) -Math.Abs((double) ((double) obj2));
                    }
                    return (double) Math.Abs((double) ((double) obj2));
                }
                if (!(obj2 is int))
                {
                    return obj2;
                }
                if (nullable.Value)
                {
                    return (int) -Math.Abs((int) ((int) obj2));
                }
                return (int) Math.Abs((int) ((int) obj2));
            }
            object obj3 = this.DigitalFormat.Parse(s);
            if (obj3 != null)
            {
                return obj3;
            }
            return s;
        }

        internal static string[] AlternativeYearMonthDay
        {
            get
            {
                if (alternativeYearMonthDay == null)
                {
                    alternativeYearMonthDay = new string[] { "d-MMM-yy", "dd-MMMM-yy" };
                }
                return alternativeYearMonthDay;
            }
        }

        /// <summary>
        /// Gets the format information.
        /// </summary>
        /// <value>The format information.</value>
        public override System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
        {
            get
            {
                if (base.DateTimeFormatInfo != null)
                {
                    return base.DateTimeFormatInfo;
                }
                if ((base.PartLocaleID != null) && (base.PartLocaleID.CultureInfo != null))
                {
                    return base.PartLocaleID.CultureInfo.DateTimeFormat;
                }
                if (base.Culture != null)
                {
                    return base.Culture.DateTimeFormat;
                }
                return DefaultTokens.DateTimeFormatInfo;
            }
            set { base.DateTimeFormatInfo = value; }
        }

        /// <summary>
        /// Gets the digital format.
        /// </summary>
        /// <value>The digital format.</value>
        NumberFormatDigital DigitalFormat
        {
            get
            {
                if (this.digitalFormat == null)
                {
                    string format = DefaultTokens.ReplaceKeyword(this.fullFormatString, NumberFormatBase.General, GeneralNumber);
                    this.digitalFormat = new NumberFormatDigital(format, base.PartLocaleID, base.PartDBNumberFormat, this.Culture);
                    this.digitalFormat.IsGeneralNumber = true;
                }
                return this.digitalFormat;
            }
        }

        /// <summary>
        /// Gets the Excel-compatible format string.
        /// </summary>
        /// <value>The Excel-compatible format string.</value>
        internal override string ExcelCompatibleFormatString
        {
            get { return  this.FormatString; }
        }

        /// <summary>
        /// Gets the digital format.
        /// </summary>
        /// <value>The digital format.</value>
        NumberFormatDigital ExponentialDigitalFormat
        {
            get
            {
                if (this.exponentialDigitalFormat == null)
                {
                    this.exponentialDigitalFormat = new NumberFormatDigital("0.#####E+00", base.PartLocaleID, base.PartDBNumberFormat, this.Culture);
                    this.exponentialDigitalFormat.IsGeneralNumber = true;
                }
                return this.exponentialDigitalFormat;
            }
        }

        /// <summary>
        /// Gets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public override string FormatString
        {
            get
            {
                if (this.fullFormatString == null)
                {
                    throw new FormatException();
                }
                return this.fullFormatString.Replace(GeneralPlaceholder, NumberFormatBase.General);
            }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralHourMinute
        {
            get { return  generalHourMinute; }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralHourMinuteSecond
        {
            get { return  generalHourMinuteSecond; }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralHourMinuteSecondSubSecond
        {
            get { return  generalHourMinuteSecondSubSecond; }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralHourMinuteSecondSubSecondWithDate
        {
            get
            {
                if (generalHourMinuteSecondSubSecondWithDate == null)
                {
                    if (CultureInfo.CurrentCulture.DateTimeFormat.MonthDayPattern.Trim().StartsWith("M"))
                    {
                        generalHourMinuteSecondSubSecondWithDate = new string[] { 
                            "M/d H:m:s.FFF", "MMM/d H:m:s.FFF", "MMMM/d H:m:s.FFF", "M/y H:m:s.FFF", "MMM/y H:m:s.FFF", "M/yyyy H:m:s.FFF", "MMM/yyyy H:m:s.FFF", "yyyy/M/d H:m", "M/d/y H:m:s.FFF", "MMM/d/y H:m:s.FFF", "MMMM/d/y H:m:s.FFF", "M/d/yyyy H:m:s", "MMM/d/yyyy H:m:s.FFF", "MMMM/d/yyyy H:m:s.FFF", "yyyy/M/d H:m:s.FFF", "M-d H:m:s.FFF", 
                            "MMM-d H:m:s.FFF", "MMMM-d H:m:s.FFF", "M-y H:m:s.FFF", "MMM-y H:m:s.FFF", "M-yyyy H:m:s.FFF", "MMM-Yyyy H:m:s.FFF", "yyyy-M-d H:m", "M-d-y H:m:s.FFF", "MMM-d-y H:m:s.FFF", "MMMM-d-y H:m:s.FFF", "M-d-yyyy H:m:s", "MMM-d-yyyy H:m:s.FFF", "MMMM-d-yyyy H:m:s.FFF", "yyyy-M-d H:m:s.FFF", "M/d h:m:s.FFF tt", "MMM/d h:m:s.FFF tt", 
                            "MMMM/d h:m:s.FFF tt", "M/y h:m:s.FFF tt", "MMM/y h:m:s.FFF tt", "M/yyyy h:m:s.FFF tt", "MMM/yyyy h:m:s.FFF tt", "yyyy/M/d h:m tt", "M/d/y h:m:s.FFF tt", "MMM/d/y h:m:s.FFF tt", "MMMM/d/y h:m:s.FFF tt", "M/d/yyyy h:m:s tt", "MMM/d/yyyy h:m:s.FFF tt", "MMMM/d/yyyy h:m:s.FFF tt", "yyyy/M/d h:m:s.FFF tt", "M-d h:m:s.FFF tt", "MMM-d h:m:s.FFF tt", "MMMM-d h:m:s.FFF tt", 
                            "M-y h:m:s.FFF tt", "MMM-y h:m:s.FFF tt", "M-yyyy h:m:s.FFF tt", "MMM-Yyyy h:m:s.FFF tt", "yyyy-M-d h:m tt", "M-d-y h:m:s.FFF tt", "MMM-d-y h:m:s.FFF tt", "MMMM-d-y h:m:s.FFF tt", "M-d-yyyy H:m:s tt", "MMM-d-yyyy H:m:s.FFF tt", "MMMM-d-yyyy h:m:s.FFF tt", "yyyy-M-d h:m:s.FFF tt"
                         };
                    }
                    else
                    {
                        generalHourMinuteSecondSubSecondWithDate = new string[] { 
                            "d/M H:m:s.FFF", "d/MMM H:m:s.FFF", "d/MMMM H:m:s.FFF", "d/M/y H:m", "d/MMM/y H:m", "d/MMMM/y H:m", "d/M/yyyy H:m", "d/mmm/yyyy H:m", "d/MMMM/yyyy H:m", "yyyy/M/d H:m", "d/M/y H:m:s.FFF", "d/MMM/y H:m:s.FFF", "d/MMMM/y H:m:s.FFF", "d/M/yyyy H:m:s.FFF", "d/MMM/yyyy H:m:s.FFF", "d/MMMM/yyyy H:m:s.FFF", 
                            "yyyy/M/d H:m:s.FFF", "d-M H:m:s.FFF", "d-MMM H:m:s.FFF", "d-MMMM H:m:s.FFF", "d-M-y H:m", "d-MMM-y H:m", "d-MMMM-y H:m", "d-M-yyyy H:m", "d-MMM-yyyy H:m", "d-MMMM-yyyy H:m", "yyyy-M-d H:m", "d-M-y H:m:s.FFF", "d-MMM-y H:m:s.FFF", "d-MMMM-y H:m:s.FFF", "d-M-yyyy H:m:s.FFF", "d-MMM-yyyy H:m:s.FFF", 
                            "d-MMMM-yyyy H:m:s.FFF", "yyyy-M-d H:m:s.FFF", "d/M h:m:s.FFF tt", "d/MMM h:m:s.FFF tt", "d/MMMM h:m:s.FFF tt", "d/M/y h:m tt", "d/MMM/y h:m tt", "d/MMMM/y h:m tt", "d/M/yyyy h:m tt", "d/mmm/yyyy h:m tt", "d/MMMM/yyyy h:m tt", "yyyy/M/d h:m tt", "d/M/y h:m:s.FFF tt", "d/MMM/y h:m:s.FFF tt", "d/MMMM/y h:m:s.FFF tt", "d/M/yyyy h:m:s.FFF tt", 
                            "d/MMM/yyyy h:m:s.FFF tt", "d/MMMM/yyyy h:m:s.FFF tt", "yyyy/M/d h:m:s.FFF tt", "d-M h:m:s.FFF tt", "d-MMM h:m:s.FFF tt", "d-MMMM h:m:s.FFF tt", "d-M-y h:m tt", "d-MMM-y h:m tt", "d-MMMM-y h:m tt", "d-M-yyyy h:m tt", "d-MMM-yyyy h:m tt", "d-MMMM-yyyy h:m tt", "yyyy-M-d h:m tt", "d-M-y h:m:s.FFF tt", "d-MMM-y h:m:s.FFF tt", "d-MMMM-y h:m:s.FFF tt", 
                            "d-M-yyyy h:m:s.FFF tt", "d-MMM-yyyy h:m:s.FFF tt", "d-MMMM-yyyy h:m:s.FFF tt", "yyyy-M-d h:m:s.FFF tt"
                         };
                    }
                }
                return generalHourMinuteSecondSubSecondWithDate;
            }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralHourMinuteSecondWithDate
        {
            get
            {
                if (generalHourMinuteSecondWithDate == null)
                {
                    if (CultureInfo.CurrentCulture.DateTimeFormat.MonthDayPattern.Trim().StartsWith("M"))
                    {
                        generalHourMinuteSecondWithDate = new string[] { 
                            "M/d H:m:s", "MMM/d H:m:s", "MMMM/d H:m:s", "M/y H:m:s", "MMM/y H:m:s", "M/yyyy H:m:s", "MMM/yyyy H:m:s", "M/d/y H:m:s", "MMM/d/y H:m:s", "MMMM/d/y H:m:s", "M/d/yyyy H:m:s", "MMM/d/yyyy H:m:s", "MMMM/d/yyyy H:m:s", "yyyy/M/d H:m:s", "M-d H:m:s", "MMM-d H:m:s", 
                            "MMMM-d H:m:s", "M-y H:m:s", "MMM-y H:m:s", "M-yyyy H:m:s", "MMM-yyyy H:m:s", "M-d-y H:m:s", "MMM-d-y H:m:s", "MMMM-d-y H:m:s", "M-d-yyyy H:m:s", "MMM-d-yyyy H:m:s", "MMMM-d-yyyy H:m:s", "yyyy-M-d H:m:s", "M/d h:m:s tt", "MMM/d h:m:s tt", "MMMM/d h:m:s tt", "M/y h:m:s tt", 
                            "MMM/y h:m:s tt", "M/yyyy h:m:s tt", "MMM/yyyy h:m:s tt", "M/d/y h:m:s tt", "MMM/d/y h:m:s tt", "MMMM/d/y h:m:s tt", "M/d/yyyy h:m:s tt", "MMM/d/yyyy h:m:s tt", "MMMM/d/yyyy h:m:s tt", "yyyy/M/d h:m:s tt", "M/d/yyyy h:mm:ss tt", "M-d h:m:s tt", "MMM-d h:m:s tt", "MMMM-d h:m:s tt", "M-y h:m:s tt", "MMM-y h:m:s tt", 
                            "M-yyyy h:m:s tt", "MMM-yyyy h:m:s tt", "M-d-y h:m:s tt", "MMM-d-y h:m:s tt", "MMMM-d-y h:m:s tt", "M-d-yyyy h:m:s tt", "MMM-d-yyyy h:m:s tt", "MMMM-d-yyyy h:m:s tt", "yyyy-M-d h:m:s tt"
                         };
                    }
                    else
                    {
                        generalHourMinuteSecondWithDate = new string[] { 
                            "d/M H:m:s", "d/MMM H:m:s", "d/MMMM H:m:s", "M/y H:m:s", "MMM/y H:m:s", "M/yyyy H:m:s", "MMM/yyyy H:m:s", "d/M/y H:m:s", "d/MMM/y H:m:s", "d/MMMM/y H:m:s", "d/M/yyyy H:m:s", "d/MMM/yyyy H:m:s", "d/MMMM/yyyy H:m:s", "yyyy/M/d H:m:s", "d-M H:m:s", "d-MMM H:m:s", 
                            "d-MMMM H:m:s", "M-y H:m:s", "MMM-y H:m:s", "M-yyyy H:m:s", "MMM-yyyy H:m:s", "d-M-y H:m:s", "d-MMM-y H:m:s", "d-MMMM-y H:m:s", "d-M-yyyy H:m:s", "d-MMM-yyyy H:m:s", "d-MMMM-yyyy H:m:s", "yyyy-M-d H:m:s", "d/M h:m:s tt", "d/MMM h:m:s tt", "d/MMMM h:m:s tt", "M/y h:m:s tt", 
                            "MMM/y h:m:s tt", "M/yyyy h:m:s tt", "MMM/yyyy h:m:s tt", "d/M/y h:m:s tt", "d/MMM/y h:m:s tt", "d/MMMM/y h:m:s tt", "d/M/yyyy h:m:s tt", "d/MMM/yyyy h:m:s tt", "d/MMMM/yyyy h:m:s tt", "yyyy/M/d h:m:s tt", "M/d/yyyy h:mm:ss tt", "d-M h:m:s tt", "d-MMM h:m:s tt", "d-MMMM h:m:s tt", "M-y h:m:s tt", "MMM-y h:m:s tt", 
                            "M-yyyy h:m:s tt", "MMM-yyyy h:m:s tt", "d-M-y h:m:s tt", "d-MMM-y h:m:s tt", "d-MMMM-y h:m:s tt", "d-M-yyyy h:m:s tt", "d-MMM-yyyy h:m:s tt", "d-MMMM-yyyy h:m:s tt", "yyyy-M-d h:m:s tt"
                         };
                    }
                }
                return generalHourMinuteSecondWithDate;
            }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralHourMinuteWithDate
        {
            get
            {
                if (generalHourMinuteWithDate == null)
                {
                    if (CultureInfo.CurrentCulture.DateTimeFormat.MonthDayPattern.Trim().StartsWith("M"))
                    {
                        generalHourMinuteWithDate = new string[] { 
                            "M/d H:m", "MMM/d H:m", "MMMM/d H:m", "M/y H:m", "MMM/y H:m", "M/yyyy H:m", "MMM/yyyy H:m", "M/d/y H:m", "MMM/d/y H:m", "MMMM/d/y H:m", "M/d/yyyy H:m", "MMM/d/yyyy H:m", "MMMM/d/yyyy H:m", "M-d H:m", "MMM-d H:m", "MMMM-d H:m", 
                            "M-y H:m", "MMM-y H:m", "M-yyyy H:m", "MMM-yyyy H:m", "M-d-y H:m", "MMM-d-y H:m", "MMMM-d-y H:m", "M-d-yyyy H:m", "MMM-d-yyyy H:m", "MMMM-d-yyyy H:m", "M/d h:m tt", "MMM/d h:m tt", "MMMM/d h:m tt", "M/y h:m tt", "MMM/y h:m tt", "M/yyyy h:m tt", 
                            "MMM/yyyy h:m tt", "M/d/y h:m tt", "MMM/d/y h:m tt", "MMMM/d/y h:m tt", "M/d/yyyy h:m tt", "MMM/d/yyyy h:m tt", "MMMM/d/yyyy h:m tt", "M-d h:m tt", "MMM-d h:m tt", "MMMM-d h:m tt", "M-y h:m tt", "MMM-y h:m tt", "M-yyyy h:m tt", "MMM-yyyy h:m tt", "M-d-y h:m tt", "MMM-d-y h:m tt", 
                            "MMMM-d-y h:m tt", "M-d-yyyy h:m tt", "MMM-d-yyyy h:m tt", "MMMM-d-yyyy h:m tt"
                         };
                    }
                    else
                    {
                        generalHourMinuteWithDate = new string[] { 
                            "d/M H:m", "d/MMM H:m", "d/MMMM H:m", "M/y H:m", "MMM/y H:m", "M/yyyy H:m", "MMM/yyyy H:m", "d/M/y H:m", "d/MMM/y H:m", "d/MMMM/y H:m", "d/M/yyyy H:m", "d/MMM/yyyy H:m", "d/MMMM/yyyy H:m", "d-M H:m", "d-MMM H:m", "d-MMMM H:m", 
                            "M-y H:m", "MMM-y H:m", "M-yyyy H:m", "MMM-yyyy H:m", "d-M-y H:m", "d-MMM-y H:m", "d-MMMM-y H:m", "d-M-yyyy H:m", "d-MMM-yyyy H:m", "d-MMMM-yyyy H:m", "d/M h:m tt", "d/MMM h:m tt", "d/MMMM h:m tt", "M/y h:m tt", "MMM/y h:m tt", "M/yyyy h:m tt", 
                            "MMM/yyyy h:m tt", "d/M/y h:m tt", "d/MMM/y h:m tt", "d/MMMM/y h:m tt", "d/M/yyyy h:m tt", "d/MMM/yyyy h:m tt", "d/MMMM/yyyy h:m tt", "d-M h:m tt", "d-MMM h:m tt", "d-MMMM h:m tt", "M-y h:m tt", "MMM-y h:m tt", "M-yyyy h:m tt", "MMM-yyyy h:m tt", "d-M-y h:m tt", "d-MMM-y h:m tt", 
                            "d-MMMM-y h:m tt", "d-M-yyyy h:m tt", "d-MMM-yyyy h:m tt", "d-MMMM-yyyy h:m tt"
                         };
                    }
                }
                return generalHourMinuteWithDate;
            }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralMonthDay
        {
            get
            {
                if (generalMonthDay == null)
                {
                    if (CultureInfo.CurrentCulture.DateTimeFormat.MonthDayPattern.Trim().StartsWith("M"))
                    {
                        generalMonthDay = new string[] { "M/d", "MMM/d", "MMMM/d", "M-d", "MMM-d", "MMMM-d" };
                    }
                    else
                    {
                        generalMonthDay = new string[] { "d/M", "d/MMM", "d/MMMM", "d-M", "d-MMM", "d-MMMM" };
                    }
                }
                return generalMonthDay;
            }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralYearMonth
        {
            get { return  generalYearMonth; }
        }

        /// <summary>
        /// Gets the general date time patterns.
        /// </summary>
        /// <value>The general date time patterns.</value>
        internal static string[] GeneralYearMonthDay
        {
            get
            {
                if (generalYearMonthDay == null)
                {
                    if (CultureInfo.CurrentCulture.DateTimeFormat.MonthDayPattern.Trim().StartsWith("M"))
                    {
                        generalYearMonthDay = new string[] { "M/d/y", "MMM/d/y", "MMMM/d/y", "M/d/yyyy", "MMM/d/yyyy", "MMMM/d/yyyy", "yyyy/M/d", "M-d-y", "MMM-d-y", "MMMM-d-y", "M-d-yyyy", "MMM-d-yyyy", "MMMM-d-yyyy", "yyyy-M-d" };
                    }
                    else
                    {
                        generalYearMonthDay = new string[] { "d/M/y", "d/MMM/y", "d/MMMM/y", "d/M/yyyy", "d/MMM/yyyy", "d/MMMM/yyyy", "yyyy/M/d", "d-M-y", "d-MMM-y", "d-MMMM-y", "d-M-yyyy", "d-MMM-yyyy", "d-MMMM-yyyy", "yyyy-M-d" };
                    }
                }
                return generalYearMonthDay;
            }
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
                if (this.DigitalFormat != null)
                {
                    this.DigitalFormat.NumberFormatInfo = value;
                }
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
                if (this.DigitalFormat != null)
                {
                    return this.DigitalFormat.NumberStringConverter;
                }
                return null;
            }
            set
            {
                if (this.DigitalFormat != null)
                {
                    this.DigitalFormat.NumberStringConverter = value;
                }
            }
        }
    }
}

