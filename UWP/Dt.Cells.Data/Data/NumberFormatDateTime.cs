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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a date time number formatter that formats a date time object.  
    /// </summary>
    internal sealed class NumberFormatDateTime : NumberFormatBase
    {
        /// <summary>
        /// Custom format for short AM/PM symbol in excel.
        /// </summary>
        static readonly string AMPMSingleDigit = "A/P";
        /// <summary>
        /// Custom format for default AM/PM symbol in excel.
        /// </summary>
        static readonly string AMPMTwoDigit = "AM/PM";
        /// <summary>
        /// Custom format for short day in excel.
        /// </summary>
        static readonly string DaySingleDigit = "d";
        /// <summary>
        /// Custom format for long day in excel.
        /// </summary>
        static readonly string DayTwoDigit = "dd";
        /// <summary>
        /// Custom format for week day abbreviation name in excel.
        /// </summary>
        static readonly string DayWeekDayAbbreviation = "aaa";
        /// <summary>
        /// Custom format for week day unabbreviated name in excel.
        /// </summary>
        static readonly string DayWeekDayUnabbreviated = "aaaa";
        /// <summary>
        /// the absolute time.
        /// </summary>
        static readonly DateTime defaultAbsoluteTime = new DateTime(0x76b, 12, 30, 0, 0, 0, 0);
        /// <summary>
        /// the default converter.
        /// </summary>
        static readonly INumberStringConverter defaultNumberStringConverter = new DefaultDateTimeNumberStringConverter();
        /// <summary>
        /// Custom format for era year in excel.
        /// </summary>
        static readonly string EraYear = "e";
        /// <summary>
        /// Custom format for short hours in excel.
        /// </summary>
        static readonly string HoursSingleDigit = "h";
        /// <summary>
        /// Custom format for long hours in excel.
        /// </summary>
        static readonly string HoursTwoDigit = "hh";
        static readonly string JanpanAraYearSymbol1 = "ge";
        static readonly string JanpanAraYearSymbol2 = "g e";
        
        /// <summary>
        /// Custom format for short minute in excel.
        /// </summary>
        static readonly string MinuteSingleDigit = "m";
        /// <summary>
        /// Custom format for long minute in excel.
        /// </summary>
        static readonly string MinuteTwoDigit = "mm";
        /// <summary>
        /// Custom format for month abbreviation name in excel.
        /// </summary>
        static readonly string MonthAbbreviation = "mmm";
        /// <summary>
        /// Custom format for the first char of month name in excel.
        /// </summary>
        static readonly string MonthJD = "mmmmm";
        /// <summary>
        /// Custom format for short month in excel.
        /// </summary>
        static readonly string MonthSingleDigit = "m";
        /// <summary>
        /// Custom format for long month in excel.
        /// </summary>
        static readonly string MonthTwoDigit = "mm";
        /// <summary>
        /// Custom format for month unabbreviated name in excel.
        /// </summary>
        static readonly string MonthUnabbreviated = "mmmm";
        /// <summary>
        /// the placeholder for month JD.
        /// </summary>
        static readonly string PlaceholderMonthJD = (DefaultTokens.ReplacePlaceholder + MonthJD);
        /// <summary>
        /// Custom format for short second in excel.
        /// </summary>
        static readonly string SecondSingleDigit = "s";
        /// <summary>
        /// Custom format for long second in excel.
        /// </summary>
        static readonly string SecondTwoDigit = "ss";
        /// <summary>
        /// Custom format for AM/PM symbol in string.format.
        /// </summary>
        static readonly string StandardAMPMSingleDigit = "tt";
        /// <summary>
        /// Custom format for week day abbreviation name in excel.
        /// </summary>
        static readonly string StandardDayWeekDayAbbreviation = "ddd";
        /// <summary>
        /// Custom format for week day unabbreviated name in excel.
        /// </summary>
        static readonly string StandardDayWeekDayUnabbreviated = "dddd";
        /// <summary>
        /// Custom format for short hour in string.format.
        /// </summary>
        static readonly string StandardHourSingleDigit = "H";
        /// <summary>
        /// Custom format for short hour in string.format.
        /// </summary>
        static readonly string StandardHourTwoDigit = "HH";
        /// <summary>
        /// Custom format for short minute in string.format.
        /// </summary>
        static readonly string StandardMinuteSingleDigit = "%m";
        /// <summary>
        /// Custom format for month abbreviation name in string.format.
        /// </summary>
        static readonly string StandardMonthAbbreviation = "MMM";
        /// <summary>
        /// Custom format for short month in string.format.
        /// </summary>
        static readonly string StandardMonthSingleDigit = "%M";
        /// <summary>
        /// Custom format for long month in string.format.
        /// </summary>
        static readonly string StandardMonthTwoDigit = "MM";
        /// <summary>
        /// Custom format for month unabbreviated name in string.format.
        /// </summary>
        static readonly string StandardMonthUnabbreviated = "MMMM";
        /// <summary>
        /// Custom format for short second in string.format.
        /// </summary>
        static readonly string StandardSecondSingleDigit = "%s";
        /// <summary>
        /// Custom format for short sub second in string.format.
        /// </summary>
        static readonly string StandardSubSecondSingleDigit = ".f";
        /// <summary>
        /// Custom format for full sub second in string.format.
        /// </summary>
        static readonly string StandardSubSecondThreeDigit = ".fff";
        /// <summary>
        /// Custom format for long sub second in string.format.
        /// </summary>
        static readonly string StandardSubSecondTwoDigit = ".ff";
        /// <summary>
        /// Custom format for short year in string.format.
        /// </summary>
        static readonly string StandardYearSingleDigit = "%y";
        /// <summary>
        /// Custom format for short sub-second in excel.
        /// </summary>
        static readonly string SubSecondSingleDigit = ".0";
        /// <summary>
        /// Custom format for full sub-second in excel.
        /// </summary>
        static readonly string SubSecondThreeDigit = ".000";
        /// <summary>
        /// Custom format for long sub-second in excel.
        /// </summary>
        static readonly string SubSecondTwoDigit = ".00";
        
        /// <summary>
        /// Custom format for long year in excel.
        /// </summary>
        internal static readonly string YearFourDigit = "yyyy";
        /// <summary>
        /// Custom format for short year in excel.
        /// </summary>
        static readonly string YearSingleDigit = "y";
        /// <summary>
        /// Custom format for short year in excel.
        /// </summary>
        internal static readonly string YearTwoDigit = "yy";

        /// <summary>
        /// the date time keyword
        /// </summary>
        static readonly string[] keywords = new string[] { 
            YearTwoDigit, YearFourDigit, MonthSingleDigit, MonthTwoDigit, MonthAbbreviation, MonthUnabbreviated, MonthJD, DaySingleDigit, DayTwoDigit, DayWeekDayAbbreviation, DayWeekDayUnabbreviated, HoursSingleDigit, HoursTwoDigit, MinuteSingleDigit, MinuteTwoDigit, SecondSingleDigit, 
            SecondTwoDigit, JanpanAraYearSymbol1, JanpanAraYearSymbol2
         };

        /// <summary>
        /// the absolute time.
        /// </summary>
        DateTime? absoluteTime;
        /// <summary>
        /// the absTimeParts
        /// </summary>
        ABSTimeFormatPart[] absTimeParts;
        /// <summary>
        /// the format string.
        /// </summary>
        string formatString;
        /// <summary>
        /// is the format has JD.
        /// </summary>
        bool hasJD;
        /// <summary>
        /// whether the year formatting is delay.
        /// </summary>
        bool hasYearDelay;
        /// <summary>
        /// the valid date time format string.
        /// </summary>
        string validDateTimeFormatString;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.NumberFormatDateTime" /> class.
        /// </summary>
        /// <param name="format">The format</param>
        /// <param name="absTimeParts">The abs time parts.</param>
        /// <param name="partLocaleID">The part locale ID.</param>
        /// <param name="dbNumberFormatPart">The db number format part.</param>
        /// <param name="culture">The culture.</param>
        internal NumberFormatDateTime(string format, ABSTimeFormatPart[] absTimeParts, object partLocaleID, object dbNumberFormatPart, CultureInfo culture) : base(partLocaleID, dbNumberFormatPart, culture)
        {
            this.absoluteTime = null;
            this.ExactlyMatch = false;
            this.formatString = this.FixFormat(NumberFormatBase.TrimNotSupportSymbol(format));
            string formatString = this.formatString;
            this.absTimeParts = absTimeParts;
            if (!EvaluateFormat(formatString))
            {
                throw new ArgumentException(ResourceStrings.FormatIllegalFormatError);
            }
            bool flag = this.ProcessAMPM(ref formatString);
            this.hasJD = this.Replace(formatString, MonthJD, "\"" + PlaceholderMonthJD + "\"", true, false, out formatString, false, false);
            this.Replace(formatString, MonthUnabbreviated, StandardMonthUnabbreviated, true, false, out formatString, false, false);
            this.Replace(formatString, MonthAbbreviation, StandardMonthAbbreviation, true, false, out formatString, false, false);
            this.Replace(formatString, MonthTwoDigit, StandardMonthTwoDigit, true, false, out formatString, false, false);
            this.Replace(formatString, MonthSingleDigit, StandardMonthSingleDigit, true, false, out formatString, false, false);
            this.Replace(formatString, DayWeekDayAbbreviation, StandardDayWeekDayAbbreviation, true, true, out formatString, false, true);
            this.Replace(formatString, DayWeekDayUnabbreviated, StandardDayWeekDayUnabbreviated, true, true, out formatString, false, true);
            this.Replace(formatString, MinuteSingleDigit, StandardMinuteSingleDigit, false, true, out formatString, false, false);
            if (!flag)
            {
                this.Replace(formatString, HoursSingleDigit, StandardHourSingleDigit, true, true, out formatString, false, false);
                this.Replace(formatString, HoursTwoDigit, StandardHourTwoDigit, true, true, out formatString, false, false);
            }
            this.Replace(formatString, SecondSingleDigit, StandardSecondSingleDigit, true, true, out formatString, false, true);
            this.Replace(formatString, SubSecondThreeDigit, StandardSubSecondThreeDigit, true, true, out formatString, false, true);
            this.Replace(formatString, SubSecondTwoDigit, StandardSubSecondTwoDigit, true, true, out formatString, false, true);
            this.Replace(formatString, SubSecondSingleDigit, StandardSubSecondSingleDigit, true, true, out formatString, false, true);
            if ((base.PartDBNumberFormat != null) && (base.PartLocaleID != null))
            {
                this.hasYearDelay |= this.Replace(formatString, YearFourDigit, "\"" + DefaultTokens.ReplacePlaceholder + YearFourDigit + "\"", true, false, out formatString, false, true);
                this.hasYearDelay |= this.Replace(formatString, YearTwoDigit, "\"" + DefaultTokens.ReplacePlaceholder + YearTwoDigit + "\"", true, false, out formatString, false, true);
            }
            if (this.IsJanpaneseCulture())
            {
                this.Replace(formatString, EraYear, YearSingleDigit, true, false, out formatString, false, true);
                this.Replace(formatString, EraYear + EraYear, YearTwoDigit, true, false, out formatString, false, true);
                this.Replace(formatString, EraYear + EraYear + EraYear, YearTwoDigit, true, false, out formatString, false, true);
            }
            else
            {
                this.Replace(formatString, EraYear, YearFourDigit, true, false, out formatString, false, true);
                this.Replace(formatString, EraYear + EraYear, YearFourDigit, true, false, out formatString, false, true);
                this.Replace(formatString, EraYear + EraYear + EraYear, YearFourDigit, true, false, out formatString, false, true);
            }
            this.Replace(formatString, YearSingleDigit, StandardYearSingleDigit, true, false, out formatString, false, true);
            if (this.absTimeParts != null)
            {
                foreach (ABSTimeFormatPart part in this.absTimeParts)
                {
                    this.Replace(formatString, part.Token, "\"" + DefaultTokens.ReplacePlaceholder + part.Token + "\"", true, true, out formatString, false, true);
                }
            }
            this.validDateTimeFormatString = formatString;
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
        /// Fixes the format.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>Returns the fixed format string</returns>
        string FixFormat(string format)
        {
            string str = format;
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            char ch2 = HoursSingleDigit[0];
            if (str.IndexOf(((char) ch2).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase) <= -1)
            {
                char ch3 = SecondSingleDigit[0];
                str.IndexOf(((char) ch3).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase);
            }
            char ch4 = YearTwoDigit[0];
            if (str.IndexOf(((char) ch4).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase) <= -1)
            {
                char ch5 = DaySingleDigit[0];
                str.IndexOf(((char) ch5).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase);
            }
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '"')
                {
                    flag = !flag;
                }
                else if (!flag)
                {
                    switch (c)
                    {
                        case 'Y':
                        case 'D':
                        case 'S':
                        case 'E':
                        case 'G':
                            c = char.ToLower(c);
                            goto Label_010B;
                    }
                    if (c == 'M')
                    {
                        if (i > 1)
                        {
                            if (!DefaultTokens.IsEquals('A', str[i - 1], true) && !DefaultTokens.IsEquals('P', str[i - 1], true))
                            {
                                c = char.ToLower(c);
                            }
                        }
                        else
                        {
                            c = char.ToLower(c);
                        }
                    }
                }
            Label_010B:
                builder.Append(c);
            }
            return builder.ToString();
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
            string number = string.Empty;
            DateTime time = DateTime.Now;
            DateTime? nullable = FormatConverter.TryDateTime(obj, true);
            if (nullable.HasValue)
            {
                time = nullable.Value;
                if ((((this.validDateTimeFormatString == "H") || (this.validDateTimeFormatString == "h")) || ((this.validDateTimeFormatString == "m") || (this.validDateTimeFormatString == "M"))) || (((this.validDateTimeFormatString == "d") || (this.validDateTimeFormatString == "s")) || (this.validDateTimeFormatString == "y")))
                {
                    this.validDateTimeFormatString = "%" + this.validDateTimeFormatString;
                }
                number = time.ToString(this.validDateTimeFormatString, (IFormatProvider) this.DateTimeFormatInfo);
                if (this.hasJD)
                {
                    string monthName = DefaultTokens.DateTimeFormatInfo.GetMonthName(time.Month);
                    number = number.Replace(PlaceholderMonthJD, monthName.Substring(0, 1));
                }
                if (this.absTimeParts != null)
                {
                    TimeSpan span = (TimeSpan) (time - this.AbsoluteTime);
                    foreach (ABSTimeFormatPart part in this.absTimeParts)
                    {
                        string newValue = null;
                        switch (part.TimePartType)
                        {
                            case TimePart.Hour:
                                newValue = ((double) Math.Floor(span.TotalHours)).ToString(part.FormatString);
                                break;

                            case TimePart.Minute:
                                newValue = ((double) Math.Floor(span.TotalMinutes)).ToString(part.FormatString);
                                break;

                            case TimePart.Second:
                                newValue = ((double) Math.Floor(span.TotalSeconds)).ToString(part.FormatString);
                                break;
                        }
                        if (newValue != null)
                        {
                            number = number.Replace(DefaultTokens.ReplacePlaceholder + part.Token, newValue);
                        }
                    }
                }
            }
            else
            {
                number = FormatConverter.ToString(obj, true);
            }
            if (this.NumberStringConverter is DefaultDateTimeNumberStringConverter)
            {
                if (this.NumberStringConverter != null)
                {
                    number = this.NumberStringConverter.ConvertTo(number, obj, false, base.PartLocaleID, base.PartDBNumberFormat);
                }
                return number;
            }
            if (this.NumberStringConverter == null)
            {
                return number;
            }
            if (this.hasYearDelay)
            {
                number = number.Replace(DefaultTokens.ReplacePlaceholder + YearFourDigit, time.ToString(YearFourDigit)).Replace(DefaultTokens.ReplacePlaceholder + YearTwoDigit, time.ToString(YearTwoDigit));
            }
            return this.NumberStringConverter.ConvertTo(number, obj, true, base.PartLocaleID, base.PartDBNumberFormat);
        }

        bool IsJanpaneseCulture()
        {
            if ((base.PartLocaleID != null) && (base.PartLocaleID.CultureInfo != null))
            {
                if ((base.PartLocaleID.CultureInfo.Name == "ja-JP") || (base.PartLocaleID.CultureInfo.Name == "ja"))
                {
                    return true;
                }
            }
            else if (base.Culture != null)
            {
                if ((base.Culture.Name == "ja-JP") || (base.Culture.Name == "ja"))
                {
                    return true;
                }
            }
            else if ((CultureInfo.CurrentCulture.Name == "ja-JP") || (CultureInfo.CurrentCulture.Name == "ja"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the specified format.
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>Returns the value.</returns>
        public override object Parse(string s)
        {
            bool flag;
            DateTime time;
            if ((s == null) || (s == string.Empty))
            {
                return null;
            }
            string str = NumberHelper.FixJapaneseChars(s);
            if (bool.TryParse(s, out flag))
            {
                return (bool) flag;
            }
            if ((this.validDateTimeFormatString != null) && DateTime.TryParseExact(str, this.validDateTimeFormatString, (IFormatProvider)this.DateTimeFormatInfo, (DateTimeStyles)DateTimeStyles.NoCurrentDateDefault, out time))
            {
                if (((time.Year == 1) && (time.Month == 1)) && (time.Day == 1))
                {
                    return new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond);
                }
                return time;
            }
            if (this.ExactlyMatch || !DateTime.TryParse(str, (IFormatProvider)this.DateTimeFormatInfo, (DateTimeStyles)DateTimeStyles.NoCurrentDateDefault, out time))
            {
                return null;
            }
            if (((time.Year == 1) && (time.Month == 1)) && (time.Day == 1))
            {
                return new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond);
            }
            return time;
        }

        /// <summary>
        /// Processes the AMPM.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>
        /// <c>true</c> if the specified format contains the Am/Pm information; otherwise, <c>false</c>.
        /// </returns>
        bool ProcessAMPM(ref string format)
        {
            bool flag = false;
            int startIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(format, AMPMTwoDigit, (CompareOptions) CompareOptions.IgnoreCase);
            if (startIndex != -1)
            {
                format = format.Remove(startIndex, AMPMTwoDigit.Length);
                format = format.Insert(startIndex, StandardAMPMSingleDigit);
                flag = true;
            }
            int num2 = CultureInfo.InvariantCulture.CompareInfo.IndexOf(format, AMPMSingleDigit, (CompareOptions) CompareOptions.IgnoreCase);
            if (num2 != -1)
            {
                format = format.Remove(num2, AMPMSingleDigit.Length);
                format = format.Insert(num2, StandardAMPMSingleDigit);
                if (this.DateTimeFormatInfo != null)
                {
                    if (this.DateTimeFormatInfo.IsReadOnly)
                    {
                        this.DateTimeFormatInfo = this.DateTimeFormatInfo.Clone() as System.Globalization.DateTimeFormatInfo;
                    }
                    this.DateTimeFormatInfo.AMDesignator = AMPMSingleDigit.Substring(0, 1);
                    this.DateTimeFormatInfo.PMDesignator = AMPMSingleDigit.Substring(2, 1);
                }
                flag = true;
            }
            int num3 = CultureInfo.InvariantCulture.CompareInfo.IndexOf(format, this.CurrentAMPM, (CompareOptions) CompareOptions.IgnoreCase);
            if (num3 == -1)
            {
                return flag;
            }
            format = format.Remove(num3, this.CurrentAMPM.Length);
            format = format.Insert(num3, StandardAMPMSingleDigit);
            string[] strArray = this.CurrentAMPM.Split(new char[] { '/' });
            if ((strArray != null) && (strArray.Length == 2))
            {
                if (this.DateTimeFormatInfo.IsReadOnly)
                {
                    this.DateTimeFormatInfo = this.DateTimeFormatInfo.Clone() as System.Globalization.DateTimeFormatInfo;
                }
                this.DateTimeFormatInfo.AMDesignator = strArray[0];
                this.DateTimeFormatInfo.PMDesignator = strArray[1];
            }
            return true;
        }

        /// <summary>
        /// Replaces the specified format.
        /// </summary>
        /// <param name="format">The format</param>
        /// <param name="oldToken">The old token</param>
        /// <param name="newToken">The new token</param>
        /// <param name="isReplaceInDateFormat">if set to <c>true</c> should replace token in date area</param>
        /// <param name="isReplaceInTimeFormat">if set to <c>true</c>  should replace token in time area</param>
        /// <param name="result">The result.</param>
        /// <param name="justSearch">if set to <c>true</c> not really replace.</param>
        /// <param name="isIgnoreCase">if set to <c>true</c> is ignore case.</param>
        /// <returns>
        /// <c>true</c> if replace token successful; otherwise, <c>false</c>.
        /// </returns>
        bool Replace(string format, string oldToken, string newToken, bool isReplaceInDateFormat, bool isReplaceInTimeFormat, out string result, bool justSearch, bool isIgnoreCase)
        {
            result = format;
            if (!isReplaceInDateFormat && !isReplaceInTimeFormat)
            {
                return false;
            }
            List<int> list = new List<int>();
            bool flag = true;
            char ch3 = HoursSingleDigit[0];
            bool flag2 = (result.IndexOf(((char) ch3).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase) > -1) || (result.IndexOf(((char) SecondSingleDigit[0]).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase) > -1);
            char ch5 = YearTwoDigit[0];
            if (((result.IndexOf(((char) ch5).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase) <= -1) && (result.IndexOf(((char) DaySingleDigit[0]).ToString(), (StringComparison) StringComparison.CurrentCultureIgnoreCase) <= -1)) && flag2)
            {
                flag = false;
            }
            bool flag4 = false;
            int num = 0;
            for (int i = 0; i < result.Length; i++)
            {
                char a = result[i];
                if (DefaultTokens.IsEquals(a, HoursSingleDigit[0], true) || DefaultTokens.IsEquals(a, SecondSingleDigit[0], true))
                {
                    flag = false;
                }
                else if (DefaultTokens.IsEquals(a, YearTwoDigit[0], true) || DefaultTokens.IsEquals(a, DaySingleDigit[0], true))
                {
                    flag = true;
                }
                if (((isReplaceInDateFormat && DefaultTokens.IsEquals(a, oldToken[num], isIgnoreCase)) && flag) || ((isReplaceInTimeFormat && DefaultTokens.IsEquals(a, oldToken[num], isIgnoreCase)) && !flag))
                {
                    bool flag5 = true;
                    for (int j = 0; j < oldToken.Length; j++)
                    {
                        if (((j + i) >= format.Length) || !DefaultTokens.IsEquals(oldToken[j], result[j + i], isIgnoreCase))
                        {
                            flag5 = false;
                            break;
                        }
                    }
                    int num4 = (i + oldToken.Length) - 1;
                    if (flag5 && ((num4 + 1) < result.Length))
                    {
                        char ch2 = result[num4];
                        int num5 = -1;
                        num5 = num4 + 1;
                        while (num5 < result.Length)
                        {
                            if (!DefaultTokens.IsEquals(ch2, result[num5], isIgnoreCase))
                            {
                                break;
                            }
                            num5++;
                        }
                        if (num5 > (num4 + 1))
                        {
                            i = num5;
                            flag5 = false;
                        }
                    }
                    if (flag5 && !flag4)
                    {
                        list.Insert(0, i);
                    }
                }
                if (a == '"')
                {
                    flag4 = !flag4;
                }
            }
            if (list.Count <= 0)
            {
                return false;
            }
            if (!justSearch)
            {
                foreach (int num6 in list)
                {
                    result = result.Remove(num6, oldToken.Length);
                    result = result.Insert(num6, newToken);
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the absolute time.
        /// </summary>
        /// <value>The absolute time.</value>
        public DateTime AbsoluteTime
        {
            get
            {
                if (this.absoluteTime.HasValue)
                {
                    return this.absoluteTime.Value;
                }
                return defaultAbsoluteTime;
            }
        }

        /// <summary>
        /// Gets the current AMPM.
        /// </summary>
        /// <value>The current AMPM.</value>
        string CurrentAMPM
        {
            get
            {
                System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = null;
                if (this.DateTimeFormatInfo != null)
                {
                    dateTimeFormatInfo = this.DateTimeFormatInfo;
                }
                else
                {
                    dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
                }
                if ((((dateTimeFormatInfo != null) && (dateTimeFormatInfo.AMDesignator != null)) && ((dateTimeFormatInfo.AMDesignator != string.Empty) && (dateTimeFormatInfo.PMDesignator != null))) && (dateTimeFormatInfo.PMDesignator != string.Empty))
                {
                    return string.Format("{0}/{1}", (object[]) new object[] { dateTimeFormatInfo.AMDesignator, dateTimeFormatInfo.PMDesignator });
                }
                return AMPMTwoDigit;
            }
        }

        /// <summary>
        /// Gets the format provider.
        /// </summary>
        /// <value>The format provider.</value>
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
        /// Gets or sets whether the values match exactly.
        /// </summary>
        /// <value><c>true</c> if the values match exactly; otherwise, <c>false</c>.</value>
        public bool ExactlyMatch { get; set; }

        /// <summary>
        /// Gets the Excel-compatible format string.
        /// </summary>
        /// <value>The Excel-compatible format string.</value>
        internal override string ExcelCompatibleFormatString
        {
            get
            {
                string formatString = this.FormatString;
                this.Replace(formatString, StandardAMPMSingleDigit, this.CurrentAMPM, true, true, out formatString, false, true);
                this.Replace(formatString, StandardMonthUnabbreviated, MonthUnabbreviated, true, false, out formatString, false, true);
                this.Replace(formatString, StandardMonthAbbreviation, MonthAbbreviation, true, false, out formatString, false, true);
                this.Replace(formatString, StandardMonthTwoDigit, MonthTwoDigit, true, false, out formatString, false, true);
                this.Replace(formatString, StandardMonthSingleDigit, MonthSingleDigit, true, false, out formatString, false, true);
                this.Replace(formatString, StandardDayWeekDayAbbreviation, DayWeekDayAbbreviation, true, true, out formatString, false, true);
                this.Replace(formatString, StandardDayWeekDayUnabbreviated, DayWeekDayUnabbreviated, true, true, out formatString, false, true);
                this.Replace(formatString, StandardMinuteSingleDigit, MinuteSingleDigit, false, true, out formatString, false, true);
                this.Replace(formatString, StandardHourSingleDigit, HoursSingleDigit, true, true, out formatString, false, false);
                this.Replace(formatString, StandardHourTwoDigit, HoursTwoDigit, true, true, out formatString, false, false);
                this.Replace(formatString, StandardSecondSingleDigit, SecondSingleDigit, true, true, out formatString, false, true);
                this.Replace(formatString, StandardSubSecondThreeDigit, SubSecondThreeDigit, true, true, out formatString, false, true);
                this.Replace(formatString, StandardSubSecondTwoDigit, SubSecondTwoDigit, true, true, out formatString, false, true);
                this.Replace(formatString, StandardSubSecondSingleDigit, SubSecondSingleDigit, true, true, out formatString, false, true);
                this.Replace(formatString, StandardYearSingleDigit, YearSingleDigit, true, true, out formatString, false, true);
                return formatString;
            }
        }

        /// <summary>
        /// Gets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public override string FormatString
        {
            get { return  this.formatString; }
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

