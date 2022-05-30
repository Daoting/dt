#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#endregion

namespace Dt.CalcEngine
{
    internal class FormatHelper
    {
        /// <summary>
        /// Represents the absolute time format.
        /// </summary>
        internal sealed class ABSTimeFormatPart : FormatHelper.FormatPartBase
        {
            /// <summary>
            /// the format string.
            /// </summary>
            private string formatString;
            /// <summary>
            /// Custom format for absolute hours in Excel (started with 1900-1-1 0:0:0).
            /// </summary>
            private const char HoursABSContent = 'h';
            /// <summary>
            /// Custom format for absolute minutes in Excel (started with 1900-1-1 0:0:0).
            /// </summary>
            private const char MinuteABSContent = 'm';
            /// <summary>
            /// Custom format for absolute seconds in Excel (started with 1900-1-1 0:0:0).
            /// </summary>
            private const char SecondABSContent = 's';
            /// <summary>
            /// the abs time token
            /// </summary>
            private string token;
            /// <summary>
            /// the type of time part.
            /// </summary>
            private FormatHelper.TimePart type;

            /// <summary>
            /// Creates a new absolute time format.
            /// </summary>
            /// <param name="token">The string expression of the absolute time format.</param>
            /// <remarks>To create the format, use a string such as "[h], [m], [s]".</remarks>
            internal ABSTimeFormatPart(string token) : base(token)
            {
                if (!EvaluateFormat(token))
                {
                    throw new ArgumentException("token is illegal.");
                }
                this.token = token.ToLower();
                if (this.token[1] == 'h')
                {
                    this.type = FormatHelper.TimePart.Hour;
                }
                else if (this.token[1] == 'm')
                {
                    this.type = FormatHelper.TimePart.Minute;
                }
                else
                {
                    if (this.token[1] != 's')
                    {
                        throw new ArgumentException("token is illegal.");
                    }
                    this.type = FormatHelper.TimePart.Second;
                }
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < (this.token.Length - 2); i++)
                {
                    builder.Append("0");
                }
                this.formatString = builder.ToString();
            }

            /// <summary>
            /// Determines whether the format string is valid.
            /// </summary>
            /// <param name="token">The token to evaluate.</param>
            /// <returns>
            /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
            /// </returns>
            internal static bool EvaluateFormat(string token)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }
                string str = FormatHelper.DefaultTokens.TrimSquareBracket(token);
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                str = str.ToLower();
                char ch = '\0';
                for (int i = 0; i < str.Length; i++)
                {
                    if (ch == '\0')
                    {
                        ch = str[i];
                    }
                    if (((ch != 'h') && (ch != 'm')) && (ch != 's'))
                    {
                        return false;
                    }
                    if (ch != str[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Gets the format string.
            /// </summary>
            /// <value>The format string. The default value is an empty string.</value>
            internal string FormatString
            {
                get
                {
                    return this.formatString;
                }
            }

            /// <summary>
            /// Gets the time part type.
            /// </summary>
            /// <value>
            /// A value that specifies the time part type.
            /// The default value is <see cref="T:Dt.CalcEngine.FormatHelper.TimePart">Hour</see>.
            /// </value>
            internal FormatHelper.TimePart TimePartType
            {
                get
                {
                    return this.type;
                }
            }

            /// <summary>
            /// Gets the token.
            /// </summary>
            /// <value>The token.</value>
            internal string Token
            {
                get
                {
                    return this.token;
                }
            }
        }

        private static class CultureHelper
        {
            private const string ChineseHongKongSarPRC = "zh-HK";
            private const string ChineseMacaoSAR = "zh-MO";
            private const string ChinesePRC = "zh-CN";
            private const string ChineseSimplified = "zh-Hans";
            private const string ChineseSingapore = "zh-SG";
            private const string ChineseTaiwan = "zh-TW";
            private const string ChineseTraditional = "zh-Hant";
            private const string EnglishCanada = "en-CA";
            private const string EnglishCaribbean = "en-029";
            private const string EnglishIreland = "en-IE";
            private const string EnglishJamaica = "en-JM";
            private const string EnglishNewZealand = "en-NZ";
            private const string EnglishPhilippines = "en-PH";
            private const string EnglishSouthAfrica = "en-ZA";
            private const string EnglishTrinidadandTobago = "en-TT";
            private const string EnglishUnitedKingdom = "en-GB";
            private const string EnglishUnitedStates = "en-US";
            private const string EnglishZimbabwe = "en-ZW";
            private const string Japanese = "ja";
            private const string JapaneseJapan = "ja-JP";

            /// <summary>
            /// Creates the calendar.
            /// </summary>
            /// <param name="cultureID">The culture ID</param>
            /// <returns>Returns the calendar.</returns>
            internal static Calendar CreateCalendar(int cultureID)
            {
                int num = cultureID & 0xff;
                if (num == 0x11)
                {
                    return new JapaneseCalendar();
                }
                return null;
            }

            /// <summary>
            /// Creates the culture information.
            /// </summary>
            /// <param name="cultureID">The culture ID</param>
            /// <returns>Returns the culture information.</returns>
            internal static CultureInfo CreateCultureInfo(int cultureID)
            {
                switch (cultureID)
                {
                    case 0x404:
                        return new CultureInfo("zh-TW");

                    case 0x409:
                        return new CultureInfo("en-US");

                    case 0x411:
                        return new CultureInfo("ja-JP");

                    case 4:
                        return new CultureInfo("zh-Hans");

                    case 0x11:
                        return new CultureInfo("ja");

                    case 0x804:
                        return new CultureInfo("zh-CN");

                    case 0x809:
                        return new CultureInfo("en-GB");

                    case 0xc04:
                        return new CultureInfo("zh-HK");

                    case 0x1004:
                        return new CultureInfo("zh-SG");

                    case 0x1009:
                        return new CultureInfo("en-CA");

                    case 0x1809:
                        return new CultureInfo("en-IE");

                    case 0x1c09:
                        return new CultureInfo("en-ZA");

                    case 0x2009:
                        return new CultureInfo("en-JM");

                    case 0x1404:
                        return new CultureInfo("zh-MO");

                    case 0x1409:
                        return new CultureInfo("en-NZ");

                    case 0x2409:
                        return new CultureInfo("en-029");

                    case 0x2c09:
                        return new CultureInfo("en-TT");

                    case 0x3009:
                        return new CultureInfo("en-ZW");

                    case 0x3409:
                        return new CultureInfo("en-PH");

                    case 0x7c04:
                        return new CultureInfo("zh-Hant");
                }
                return (CultureInfo.CurrentCulture.Clone() as CultureInfo);
            }
        }

        /// <summary>
        /// Represents a custom number format
        /// </summary>
        /// <remarks>
        /// The format can contain up to four sections. 
        /// You can specify format codes to describe how you want to display your numbers, dates, times, and text.
        /// </remarks>
        internal sealed class CustomNumberFormat
        {
            /// <summary>
            /// date time format info
            /// </summary>
            private DateTimeFormatInfo dateTimeFormatInfo = null;
            /// <summary>
            /// the [DBNum] part format.
            /// </summary>
            private FormatHelper.DBNumberFormatPart dbNumberFormatPart;
            /// <summary>
            /// the format cache.
            /// </summary>
            private string formatCached;
            /// <summary>
            /// the local id format part.
            /// </summary>
            private LocaleIDFormatPart localeIDFormatPart;
            /// <summary>
            /// the number format info.
            /// </summary>
            private FormatHelper.NumberFormatBase numberFormat;
            /// <summary>
            /// number format info
            /// </summary>
            private NumberFormatInfo numberFormatInfo = null;

            /// <summary>
            /// Creates a new custom number format.
            /// </summary>
            internal CustomNumberFormat()
            {
                this.formatCached = "General";
            }

            /// <summary>
            /// Creates a new custom number format with the specified format string.
            /// </summary>
            /// <param name="format">The format string.</param>
            internal CustomNumberFormat(string format)
            {
                this.Init(format);
            }

            /// <summary>
            /// Adds the specified format to the formatter.
            /// </summary>
            /// <param name="part">The format to add to this formatter.</param>
            private void AddPart(FormatHelper.FormatPartBase part)
            {
                if (part == null)
                {
                    throw new ArgumentNullException("part");
                }
                if (part is LocaleIDFormatPart)
                {
                    if (this.localeIDFormatPart != null)
                    {
                        throw new NotSupportedException("The type of descriptor was added.");
                    }
                    this.localeIDFormatPart = part as LocaleIDFormatPart;
                }
                else if (part is FormatHelper.DBNumberFormatPart)
                {
                    if (this.dbNumberFormatPart != null)
                    {
                        throw new NotSupportedException("The type of descriptor was added.");
                    }
                    this.dbNumberFormatPart = part as FormatHelper.DBNumberFormatPart;
                }
            }

            /// <summary>
            /// Determines whether the specified formatter is equal to the current formatter.
            /// </summary>
            /// <param name="obj">The formatter to compare with the current formatter.</param>
            /// <returns>
            /// <c>true</c> if the specified formatter is equal to the current formatter; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                return ((obj is FormatHelper.CustomNumberFormat) && ((object.Equals(this.numberFormatInfo, ((FormatHelper.CustomNumberFormat) obj).numberFormatInfo) && object.Equals(this.dateTimeFormatInfo, ((FormatHelper.CustomNumberFormat) obj).dateTimeFormatInfo)) && (this.formatCached == ((FormatHelper.CustomNumberFormat) obj).formatCached)));
            }

            /// <summary>
            /// Formats the specified value.
            /// </summary>
            /// <param name="obj">The object with cell data to format.</param>
            /// <returns>Returns the formatted string.</returns>
            internal string Format(object obj)
            {
                return this.numberFormat.Format(obj);
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="T:System.Object" />.
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            /// <summary>
            /// Initializes the specified format.
            /// </summary>
            /// <param name="format">Format String</param>
            private void Init(string format)
            {
                if (format == null)
                {
                    throw new ArgumentNullException("format");
                }
                this.localeIDFormatPart = null;
                this.dbNumberFormatPart = null;
                this.formatCached = format;
                StringBuilder builder = null;
                StringBuilder builder2 = null;
                bool flag = false;
                List<FormatHelper.ABSTimeFormatPart> list = new List<FormatHelper.ABSTimeFormatPart>();
                for (int i = 0; i < format.Length; i++)
                {
                    char ch = format[i];
                    if (ch == FormatHelper.DefaultTokens.LeftSquareBracket)
                    {
                        if (flag)
                        {
                            throw new FormatException("format is illegal.");
                        }
                        if (builder2 != null)
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder();
                            }
                            builder.Append(builder2.ToString());
                            builder2 = null;
                        }
                        builder2 = new StringBuilder();
                        builder2.Append(ch);
                        flag = true;
                    }
                    else if (ch == FormatHelper.DefaultTokens.RightSquareBracket)
                    {
                        if (!flag)
                        {
                            throw new FormatException("format is illegal.");
                        }
                        if (builder2 == null)
                        {
                            throw new FormatException("format is illegal.");
                        }
                        if (builder2 == null)
                        {
                            builder2 = new StringBuilder();
                        }
                        builder2.Append(ch);
                        FormatHelper.FormatPartBase part = FormatHelper.FormatPartBase.Create(builder2.ToString());
                        if ((part != null) && !(part is FormatHelper.ABSTimeFormatPart))
                        {
                            this.AddPart(part);
                        }
                        else
                        {
                            if (!(part is FormatHelper.ABSTimeFormatPart))
                            {
                                throw new FormatException("format is illegal.");
                            }
                            list.Add(part as FormatHelper.ABSTimeFormatPart);
                            if (builder == null)
                            {
                                builder = new StringBuilder();
                            }
                            builder.Append(builder2.ToString());
                        }
                        builder2 = null;
                        flag = false;
                    }
                    else
                    {
                        if (builder2 == null)
                        {
                            builder2 = new StringBuilder();
                        }
                        builder2.Append(ch);
                    }
                }
                if (builder2 != null)
                {
                    if (flag)
                    {
                        throw new FormatException("format is illegal.");
                    }
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                    }
                    builder.Append(builder2.ToString());
                }
                string str2 = (builder != null) ? builder.ToString() : string.Empty;
                if (FormatHelper.NumberFormatGeneral.EvaluateFormat(str2))
                {
                    this.numberFormat = new FormatHelper.NumberFormatGeneral(str2, this.LocaleIDFormatPart, this.dbNumberFormatPart);
                }
                else if (FormatHelper.NumberFormatDateTime.EvaluateFormat(str2))
                {
                    FormatHelper.ABSTimeFormatPart[] absTimeParts = (list.Count > 0) ? list.ToArray() : null;
                    this.numberFormat = new FormatHelper.NumberFormatDateTime(str2, absTimeParts, this.LocaleIDFormatPart, this.dbNumberFormatPart);
                }
                else if (FormatHelper.NumberFormatDigital.EvaluateFormat(str2))
                {
                    this.numberFormat = new FormatHelper.NumberFormatDigital(format, this.LocaleIDFormatPart, this.dbNumberFormatPart);
                }
                else
                {
                    if (!FormatHelper.NumberFormatText.EvaluateFormat(str2))
                    {
                        throw new FormatException("format is illegal.");
                    }
                    this.numberFormat = new FormatHelper.NumberFormatText(format, this.LocaleIDFormatPart, this.dbNumberFormatPart);
                }
            }

            /// <summary>
            /// Gets the locale format for this formatter.
            /// </summary>
            /// <value>The locale format for this formatter. The default value is null.</value>
            public LocaleIDFormatPart LocaleIDFormatPart
            {
                get
                {
                    return this.localeIDFormatPart;
                }
            }
        }

        /// <summary>
        /// Represents a db number class.
        /// </summary>
        internal sealed class DBNumber
        {
            /// <summary>
            /// the Chinese number1.
            /// </summary>
            private static FormatHelper.DBNumber chineseDBNum1 = null;
            /// <summary>
            /// the Chinese number2.
            /// </summary>
            private static FormatHelper.DBNumber chineseDBNum2 = null;
            /// <summary>
            /// the Chinese number3.
            /// </summary>
            private static FormatHelper.DBNumber chineseDBNum3 = null;
            /// <summary>
            /// ○一二三四五六七八九
            /// </summary>
            private static int[] ChineseNumberLetterValues1 = new int[] { 0x25cb, 0x4e00, 0x4e8c, 0x4e09, 0x56db, 0x4e94, 0x516d, 0x4e03, 0x516b, 0x4e5d };
            /// <summary>
            /// 零壹贰叁肆伍陆柒捌玖
            /// </summary>
            private static int[] ChineseNumberLetterValues2 = new int[] { 0x96f6, 0x58f9, 0x8d30, 0x53c1, 0x8086, 0x4f0d, 0x9646, 0x67d2, 0x634c, 0x7396 };
            /// <summary>
            /// ０１２３４５６７８９
            /// </summary>
            private static int[] ChineseNumberLetterValues3 = new int[] { 0xff10, 0xff11, 0xff12, 0xff13, 0xff14, 0xff15, 0xff16, 0xff17, 0xff18, 0xff19 };
            /// <summary>
            /// 千百十兆千百十亿千百十万千百十
            /// </summary>
            private static int[] ChineseNumberUnitLetter1 = new int[] { 0x5343, 0x767e, 0x5341, 0x5146, 0x5343, 0x767e, 0x5341, 0x4ebf, 0x5343, 0x767e, 0x5341, 0x4e07, 0x5343, 0x767e, 0x5341, 0 };
            /// <summary>
            /// 仟佰拾兆仟佰拾亿仟佰拾万仟佰拾
            /// </summary>
            private static int[] ChineseNumberUnitLetter2 = new int[] { 0x4edf, 0x4f70, 0x62fe, 0x5146, 0x4edf, 0x4f70, 0x62fe, 0x4ebf, 0x4edf, 0x4f70, 0x62fe, 0x4e07, 0x4edf, 0x4f70, 0x62fe, 0 };
            /// <summary>
            /// the Japanese number1.
            /// </summary>
            private static FormatHelper.DBNumber japaneseDBNum1 = null;
            /// <summary>
            /// the Japanese number2.
            /// </summary>
            private static FormatHelper.DBNumber japaneseDBNum2 = null;
            /// <summary>
            /// the Japanese number3.
            /// </summary>
            private static FormatHelper.DBNumber japaneseDBNum3 = null;
            /// <summary>
            /// 〇一二三四五六七八九
            /// </summary>
            private static int[] JapaneseNumberLetterValues1 = new int[] { 0x3007, 0x4e00, 0x4e8c, 0x4e09, 0x56db, 0x4e94, 0x516d, 0x4e03, 0x516b, 0x4e5d };
            /// <summary>
            /// 〇壱弐参四伍六七八九
            /// </summary>
            private static int[] JapaneseNumberLetterValues2 = new int[] { 0x3007, 0x58f1, 0x5f10, 0x53c2, 0x56db, 0x4f0d, 0x516d, 0x4e03, 0x516b, 0x4e5d };
            /// <summary>
            /// ０１２３４５６７８９
            /// </summary>
            private static int[] JapaneseNumberLetterValues3 = new int[] { 0xff10, 0xff11, 0xff12, 0xff13, 0xff14, 0xff15, 0xff16, 0xff17, 0xff18, 0xff19 };
            /// <summary>
            /// 千百十兆千百十億千百十万千百十
            /// </summary>
            private static int[] JapaneseNumberUnitLetter1 = new int[] { 0x5343, 0x767e, 0x5341, 0x5146, 0x5343, 0x767e, 0x5341, 0x5104, 0x5343, 0x767e, 0x5341, 0x4e07, 0x5343, 0x767e, 0x5341, 0 };
            /// <summary>
            /// 阡百拾兆阡百拾億阡百拾萬阡百拾
            /// </summary>
            private static int[] JapaneseNumberUnitLetter2 = new int[] { 0x9621, 0x767e, 0x62fe, 0x5146, 0x9621, 0x767e, 0x62fe, 0x5104, 0x9621, 0x767e, 0x62fe, 0x842c, 0x9621, 0x767e, 0x62fe, 0 };
            /// <summary>
            /// the numbers
            /// </summary>
            private IList<string> numbers;
            /// <summary>
            /// the units
            /// </summary>
            private IList<string> units;

            /// <summary>
            /// Creates a <see cref="T:Dt.CalcEngine.FormatHelper.DBNumber" /> object with the specified numbers and units.
            /// </summary>
            /// <param name="units">The units of numbers.</param>
            /// <param name="numbers">The numbers.</param>
            private DBNumber(int[] units, int[] numbers)
            {
                if (units != null)
                {
                    this.units = new List<string>();
                    foreach (int num in units)
                    {
                        if (num == 0)
                        {
                            this.units.Add(string.Empty);
                        }
                        else
                        {
                            char ch = (char) num;
                            this.units.Add(((char) ch).ToString());
                        }
                    }
                }
                if (numbers != null)
                {
                    this.numbers = new List<string>();
                    foreach (int num2 in numbers)
                    {
                        if (num2 == 0)
                        {
                            this.numbers.Add(string.Empty);
                        }
                        else
                        {
                            char ch2 = (char) num2;
                            this.numbers.Add(((char) ch2).ToString());
                        }
                    }
                }
            }

            /// <summary>
            /// Gets the Chinese number letters.
            /// </summary>
            /// <value>The Chinese number letters.</value>
            internal static FormatHelper.DBNumber ChineseDBNum1
            {
                get
                {
                    if (chineseDBNum1 == null)
                    {
                        chineseDBNum1 = new FormatHelper.DBNumber(ChineseNumberUnitLetter1, ChineseNumberLetterValues1);
                    }
                    return chineseDBNum1;
                }
            }

            /// <summary>
            /// Gets the Chinese number letters.
            /// </summary>
            /// <value>The Chinese number letters.</value>
            internal static FormatHelper.DBNumber ChineseDBNum2
            {
                get
                {
                    if (chineseDBNum2 == null)
                    {
                        chineseDBNum2 = new FormatHelper.DBNumber(ChineseNumberUnitLetter2, ChineseNumberLetterValues2);
                    }
                    return chineseDBNum2;
                }
            }

            /// <summary>
            /// Gets the Chinese number letters.
            /// </summary>
            /// <value>The Chinese number letters.</value>
            internal static FormatHelper.DBNumber ChineseDBNum3
            {
                get
                {
                    if (chineseDBNum3 == null)
                    {
                        chineseDBNum3 = new FormatHelper.DBNumber(null, ChineseNumberLetterValues3);
                    }
                    return chineseDBNum3;
                }
            }

            /// <summary>
            /// Gets the Japanese number letters.
            /// </summary>
            /// <value>The Japanese number letters.</value>
            internal static FormatHelper.DBNumber JapaneseDBNum1
            {
                get
                {
                    if (japaneseDBNum1 == null)
                    {
                        japaneseDBNum1 = new FormatHelper.DBNumber(JapaneseNumberUnitLetter1, JapaneseNumberLetterValues1);
                    }
                    return japaneseDBNum1;
                }
            }

            /// <summary>
            /// Gets the Japanese number letters.
            /// </summary>
            /// <value>The Japanese number letters.</value>
            internal static FormatHelper.DBNumber JapaneseDBNum2
            {
                get
                {
                    if (japaneseDBNum2 == null)
                    {
                        japaneseDBNum2 = new FormatHelper.DBNumber(JapaneseNumberUnitLetter2, JapaneseNumberLetterValues2);
                    }
                    return japaneseDBNum2;
                }
            }

            /// <summary>
            /// Gets the Japanese number letters.
            /// </summary>
            /// <value>The Japanese number letters.</value>
            internal static FormatHelper.DBNumber JapaneseDBNum3
            {
                get
                {
                    if (japaneseDBNum3 == null)
                    {
                        japaneseDBNum3 = new FormatHelper.DBNumber(null, JapaneseNumberLetterValues3);
                    }
                    return japaneseDBNum3;
                }
            }

            /// <summary>
            /// Gets the numbers.
            /// </summary>
            /// <value>The numbers.</value>
            internal IList<string> Numbers
            {
                get
                {
                    return this.numbers;
                }
            }

            /// <summary>
            /// Gets the units.
            /// </summary>
            /// <value>The units.</value>
            internal IList<string> Units
            {
                get
                {
                    return this.units;
                }
            }
        }

        /// <summary>
        /// Represents a number format.
        /// </summary>
        internal sealed class DBNumberFormatPart : FormatHelper.FormatPartBase
        {
            /// <summary>
            /// the abs time token
            /// </summary>
            private string token;
            /// <summary>
            /// the type of time part.
            /// </summary>
            private int type;

            /// <summary>
            /// Creates a new DB number format. 
            /// </summary>
            /// <param name="token">The string expression for the format.</param>
            internal DBNumberFormatPart(string token) : base(token)
            {
                if (!EvaluateFormat(token))
                {
                    throw new ArgumentException("token is illegal.");
                }
                this.token = token;
                string s = FormatHelper.DefaultTokens.TrimSquareBracket(token).Remove(0, "dbnum".Length);
                this.type = int.Parse(s);
                if ((this.type < 0) || (this.type > 3))
                {
                    throw new ArgumentException("token is illegal.");
                }
            }

            /// <summary>
            /// Determines whether the format string is valid.
            /// </summary>
            /// <param name="token">The token to evaluate.</param>
            /// <returns>
            /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
            /// </returns>
            internal static bool EvaluateFormat(string token)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }
                string str = FormatHelper.DefaultTokens.TrimSquareBracket(token);
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                return str.StartsWith("DBNum", StringComparison.CurrentCultureIgnoreCase);
            }

            /// <summary>
            /// Numbers the string.
            /// </summary>
            /// <param name="value">The value</param>
            /// <param name="numbers">The numbers</param>
            /// <returns>Returns the formatted number.</returns>
            private static string FormatNumberString(string value, IList<string> numbers)
            {
                string str = value;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    int num2 = Convert.ToInt32(str.Substring(i, 1));
                    builder.Append(numbers[num2]);
                }
                return builder.ToString();
            }

            /// <summary>
            /// Numbers the string.
            /// </summary>
            /// <param name="value">The value</param>
            /// <param name="units">The units</param>
            /// <param name="numbers">The numbers</param>
            /// <returns>Returns the formatted number.</returns>
            private static string FormatNumberString(string value, IList<string> units, IList<string> numbers)
            {
                if (units == null)
                {
                    return FormatNumberString(value, numbers);
                }
                int num = 0;
                string str = null;
                string str2 = value;
                int length = str2.Length;
                bool flag = false;
                List<string> list = new List<string>();
                for (int i = 0; i < length; i++)
                {
                    int num4 = (units.Count - 1) - i;
                    if (num4 > -1)
                    {
                        list.Insert(0, units[num4].ToString());
                    }
                    else
                    {
                        list.Insert(0, string.Empty);
                    }
                }
                bool flag2 = false;
                for (int j = 0; j < length; j++)
                {
                    string str3 = str2.Substring(j, 1);
                    int num6 = Convert.ToInt32(str3);
                    string str4 = string.Empty;
                    string str5 = string.Empty;
                    if (((length - j) - 0x10) > 0)
                    {
                        str4 = numbers[num6];
                        str5 = "";
                        flag2 = true;
                    }
                    else if (((j != (length - 1)) && (j != (length - 5))) && ((j != (length - 9)) && (j != (length - 13))))
                    {
                        if (str3 == "0")
                        {
                            str4 = "";
                            str5 = "";
                            num++;
                        }
                        else if ((str3 != "0") && (num != 0))
                        {
                            str4 = numbers[0] + numbers[num6];
                            str5 = list[j];
                            num = 0;
                        }
                        else
                        {
                            str4 = numbers[num6];
                            str5 = list[j];
                            num = 0;
                        }
                    }
                    else if ((str3 != "0") && (num != 0))
                    {
                        str4 = numbers[0] + numbers[num6];
                        str5 = list[j];
                        num = 0;
                    }
                    else if (((str3 != "0") && (num == 0)) || flag2)
                    {
                        str4 = numbers[num6];
                        str5 = list[j];
                        num = 0;
                        flag2 = false;
                    }
                    else if ((str3 == "0") && (num >= 3))
                    {
                        str4 = "";
                        str5 = "";
                        num++;
                    }
                    else if (length >= 11)
                    {
                        str4 = "";
                        num++;
                    }
                    else
                    {
                        str4 = "";
                        str5 = list[j];
                        num++;
                    }
                    if (!((str4 + str5) == string.Empty))
                    {
                        flag = false;
                    }
                    if ((j == (length - 13)) && !flag)
                    {
                        str5 = list[j];
                        flag = true;
                    }
                    if ((j == (length - 9)) && !flag)
                    {
                        str5 = list[j];
                        flag = true;
                    }
                    if (j == (length - 1))
                    {
                        str5 = list[j];
                        flag = true;
                    }
                    str = str + str4 + str5;
                }
                int result = -1;
                if (int.TryParse(value, out result) && (result == 0))
                {
                    return numbers[0];
                }
                return str;
            }

            /// <summary>
            /// Converts numbers the string.
            /// </summary>
            /// <param name="s">The string to convert</param>
            /// <param name="dbNumber">Data number part.</param>
            /// <param name="isNumber">Indicates whether the number should be formatted by number rules</param>
            /// <returns>Returns the formatted value.</returns>
            private string NumberString(string s, FormatHelper.DBNumber dbNumber, bool isNumber)
            {
                string[] strArray = s.Split(new char[] { '.' });
                if (strArray != null)
                {
                    if (strArray.Length == 1)
                    {
                        return FormatNumberString(strArray[0], isNumber ? dbNumber.Units : null, dbNumber.Numbers);
                    }
                    if (strArray.Length == 2)
                    {
                        string str = FormatNumberString(strArray[0], isNumber ? dbNumber.Units : null, dbNumber.Numbers);
                        string str2 = FormatNumberString(strArray[1], dbNumber.Numbers);
                        return (str + "." + str2);
                    }
                }
                throw new ArgumentException("value is illegal.");
            }

            /// <summary>
            /// Numbers the string.
            /// </summary>
            /// <param name="s">The string to replace</param>
            /// <param name="dbNumber">The db number</param>
            /// <param name="isNumber">Indicates whether the number should be formatted by number rules</param>
            /// <returns>Returns the formatted value</returns>
            internal string ReplaceNumberString(string s, FormatHelper.DBNumber dbNumber, bool isNumber)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return s;
                }
                string str = s;
                string str2 = s;
                int num = -1;
                int startIndex = -1;
                bool flag = false;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    char c = str2[i];
                    if (char.IsNumber(c) || (FormatHelper.DefaultTokens.IsEquals(c, FormatHelper.DefaultTokens.DecimalSeparator, false) && !flag))
                    {
                        if (FormatHelper.DefaultTokens.IsEquals(c, FormatHelper.DefaultTokens.DecimalSeparator, false))
                        {
                            flag = true;
                        }
                        if (num == -1)
                        {
                            num = i;
                        }
                        startIndex = i;
                    }
                    else if ((startIndex > -1) && (num > -1))
                    {
                        double num4;
                        string str3 = str2.Substring(startIndex, (num - startIndex) + 1);
                        if (double.TryParse(str3, out num4))
                        {
                            string str4 = this.NumberString(str3, dbNumber, isNumber);
                            str = str.Remove(startIndex, (num - startIndex) + 1).Insert(startIndex, str4);
                        }
                        num = -1;
                        startIndex = -1;
                        flag = false;
                    }
                }
                if ((startIndex > -1) && (num > -1))
                {
                    double num5;
                    string str5 = str2.Substring(startIndex, (num - startIndex) + 1);
                    if (double.TryParse(str5, out num5))
                    {
                        string str6 = this.NumberString(str5, dbNumber, isNumber);
                        str = str.Remove(startIndex, (num - startIndex) + 1).Insert(startIndex, str6);
                    }
                    num = -1;
                    startIndex = -1;
                    flag = false;
                }
                return str;
            }

            /// <summary>
            /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
            /// </returns>
            public override string ToString()
            {
                if (this.type <= -1)
                {
                    throw new FormatException();
                }
                return FormatHelper.DefaultTokens.AddSquareBracket("DBNum" + ((int) this.type));
            }

            /// <summary>
            /// Gets the token of the format part.
            /// </summary>
            /// <value>The token of the format part. The default value is an empty string.</value>
            internal string Token
            {
                get
                {
                    if (this.token == null)
                    {
                        return string.Empty;
                    }
                    return this.token;
                }
            }

            /// <summary>
            /// Gets the number letter type.
            /// </summary>
            /// <value>The number letter type. The default value is 1.</value>
            internal int Type
            {
                get
                {
                    return this.type;
                }
            }
        }

        /// <summary>
        /// Represents a default date time converter class that converts number strings to Japanese or Chinese. 
        /// </summary>
        internal class DefaultDateTimeNumberStringConverter : FormatHelper.INumberStringConverter
        {
            /// <summary>
            /// Converts a specified number string to other representation.
            /// </summary>
            /// <param name="number">Formatted data string</param>
            /// <param name="value">Original data value</param>
            /// <param name="isGeneralNumber">Whether the number is a general format number</param>
            /// <param name="locale">A <see cref="T:Dt.CalcEngine.FormatHelper.LocaleIDFormatPart" /> object that specifies the locale information</param>
            /// <param name="dbNumber">A <see cref="T:Dt.CalcEngine.FormatHelper.DBNumberFormatPart" /> object that specifies the number format information</param>
            /// <returns>
            /// Returns the string that represents the original data value.
            /// </returns>
            public string ConvertTo(string number, object value, bool isGeneralNumber, FormatHelper.LocaleIDFormatPart locale, FormatHelper.DBNumberFormatPart dbNumber)
            {
                string s = number;
                if (((locale != null) && (dbNumber != null)) && (value is DateTime))
                {
                    FormatHelper.DBNumber dBNumber = locale.GetDBNumber(dbNumber.Type);
                    DateTime time = (DateTime) value;
                    DateTime time2 = (DateTime) value;
                    s = dbNumber.ReplaceNumberString(s, dBNumber, true).Replace(FormatHelper.DefaultTokens.ReplacePlaceholder + "yyyy", time.ToString("yyyy")).Replace(FormatHelper.DefaultTokens.ReplacePlaceholder + "yy", time2.ToString("yy"));
                    s = dbNumber.ReplaceNumberString(s, dBNumber, false);
                }
                return s;
            }
        }

        /// <summary>
        /// Represents a default number string converter class that converts number strings to Japanese or Chinese. 
        /// </summary>
        internal class DefaultNumberStringConverter : FormatHelper.INumberStringConverter
        {
            /// <summary>
            /// Converts a specified number string to another representation.
            /// </summary>
            /// <param name="number">Formatted data string</param>
            /// <param name="value">Original data value</param>
            /// <param name="isGeneralNumber">Whether the number is a general format number</param>
            /// <param name="locale">A <see cref="T:Dt.CalcEngine.FormatHelper.LocaleIDFormatPart" /> object that specifies the locale information</param>
            /// <param name="dbNumber">A <see cref="T:Dt.CalcEngine.FormatHelper.DBNumberFormatPart" /> object that specifies the number format information</param>
            /// <returns>
            /// Returns the string that represents the original data value.
            /// </returns>
            public string ConvertTo(string number, object value, bool isGeneralNumber, FormatHelper.LocaleIDFormatPart locale, FormatHelper.DBNumberFormatPart dbNumber)
            {
                if ((locale != null) && (dbNumber != null))
                {
                    FormatHelper.DBNumber dBNumber = locale.GetDBNumber(dbNumber.Type);
                    return dbNumber.ReplaceNumberString(number, dBNumber, isGeneralNumber);
                }
                return number;
            }
        }

        internal class DefaultTokens
        {
            /// <summary>
            /// the asterisk
            /// </summary>
            internal static readonly char Asterisk = '*';
            /// <summary>
            /// Colon Sign
            /// </summary>
            internal static readonly char Colon = ':';
            /// <summary>
            /// Comma sign
            /// </summary>
            internal static readonly char Comma = ',';
            /// <summary>
            /// Commercial At
            /// </summary>
            internal static readonly char CommercialAt = '@';
            /// <summary>
            /// dollar sign
            /// </summary>
            internal static readonly char Dollar = '$';
            /// <summary>
            /// Gets the double quote.
            /// </summary>
            internal static readonly char DoubleQuote = '"';
            /// <summary>
            /// the end char of array.
            /// </summary>
            internal static readonly char EndCharOfArray = '\0';
            /// <summary>
            /// Gets the equals sign
            /// </summary>
            internal static readonly char EqualsThanSign = '=';
            /// <summary>
            /// Gets the greater than sign
            /// </summary>
            internal static readonly char GreaterThanSign = '>';
            /// <summary>
            /// Hyphen minus
            /// </summary>
            internal static readonly char HyphenMinus = '-';
            /// <summary>
            /// Left Parenthesis
            /// </summary>
            internal static readonly char LeftParenthesis = '(';
            /// <summary>
            /// Gets the left square bracket.
            /// </summary>
            internal static readonly char LeftSquareBracket = '[';
            /// <summary>
            /// Gets the less than sign
            /// </summary>
            internal static readonly char LessThanSign = '<';
            /// <summary>
            /// Number sign
            /// </summary>
            internal static readonly char NumberSign = '#';
            /// <summary>
            /// Plus sign
            /// </summary>
            internal static readonly char PlusSign = '+';
            /// <summary>
            /// QuestionMark
            /// </summary>
            internal static readonly char QuestionMark = '?';
            /// <summary>
            /// Reverse solidus sign
            /// </summary>
            internal static readonly char ReverseSolidusSign = '\\';
            /// <summary>
            /// Right Parenthesis
            /// </summary>
            internal static readonly char RightParenthesis = ')';
            /// <summary>
            /// Gets the right square bracket.
            /// </summary>
            internal static readonly char RightSquareBracket = ']';
            /// <summary>
            /// Semicolon Sign
            /// </summary>
            internal static readonly char Semicolon = ';';
            /// <summary>
            /// Sharp Sign.
            /// </summary>
            internal static readonly char Sharp = '#';
            /// <summary>
            /// Gets the single quote.
            /// </summary>
            internal static readonly char SingleQuote = '\'';
            /// <summary>
            /// Solidus sign
            /// </summary>
            internal static readonly char SolidusSign = '/';
            /// <summary>
            /// char space
            /// </summary>
            internal static readonly char Space = ' ';
            /// <summary>
            /// Gets the tab char.
            /// </summary>
            internal static readonly char Tab = '\t';
            /// <summary>
            /// under line
            /// </summary>
            internal static readonly char UnderLine = '_';
            /// <summary>
            /// Digit Zero
            /// </summary>
            internal static readonly char Zero = '0';

            /// <summary>
            /// Adds a square bracket.
            /// </summary>
            /// <param name="token">The token</param>
            /// <returns>Returns formatted string</returns>
            internal static string AddSquareBracket(string token)
            {
                if (token == null)
                {
                    throw new ArgumentNullException("token");
                }
                if ((token.Length == 0) || (token[0] != LeftSquareBracket))
                {
                    token = token.Insert(0, ((char) LeftSquareBracket).ToString());
                }
                if ((token.Length == 0) || (token[token.Length - 1] != RightSquareBracket))
                {
                    token = token.Insert(token.Length, ((char) RightSquareBracket).ToString());
                }
                return token;
            }

            /// <summary>
            /// Filters the specified string.
            /// </summary>
            /// <param name="s">The string</param>
            /// <param name="bracketsStart">The brackets start</param>
            /// <param name="bracketsEnd">The brackets end</param>
            /// <returns>Returns the filtered string.</returns>
            internal static string Filter(string s, char bracketsStart, char bracketsEnd)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return s;
                }
                StringBuilder builder = new StringBuilder();
                int num = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    char ch = s[i];
                    if (ch == bracketsStart)
                    {
                        num++;
                    }
                    else if (ch == bracketsEnd)
                    {
                        num--;
                        if (num < 0)
                        {
                            num = 0;
                        }
                    }
                    else if (num == 0)
                    {
                        builder.Append(ch);
                    }
                }
                return builder.ToString();
            }

            /// <summary>
            /// Determines whether the specified characters are equal.
            /// </summary>
            /// <param name="a">First character</param>
            /// <param name="b">Second character</param>
            /// <param name="isIgnoreCase">if set to <c>true</c>, ignore the case when comparing</param>
            /// <returns>
            /// <c>true</c> if the two characters are equal; otherwise, <c>false</c>.
            /// </returns>
            internal static bool IsEquals(char a, char b, bool isIgnoreCase)
            {
                if (isIgnoreCase)
                {
                    return (char.ToLower(a) == char.ToLower(b));
                }
                return (a == b);
            }

            /// <summary>
            /// Determines whether the specified character equals the specified string.
            /// </summary>
            /// <param name="a">Character</param>
            /// <param name="b">String</param>
            /// <param name="isIgnoreCase">if set to <c>true</c>, ignore the case when comparing</param>
            /// <returns>
            /// <c>true</c> if the character is equal to the string; otherwise, <c>false</c>.
            /// </returns>
            internal static bool IsEquals(char a, string b, bool isIgnoreCase)
            {
                if (b == null)
                {
                    return false;
                }
                if (b.Length != 1)
                {
                    return false;
                }
                return IsEquals(a, b[0], isIgnoreCase);
            }

            /// <summary>
            /// Determines whether the specified character is an operator.
            /// </summary>
            /// <param name="c">The character</param>
            /// <returns>
            /// <c>true</c> if the specified character is an operator; otherwise, <c>false</c>.
            /// </returns>
            internal static bool IsOperator(char c)
            {
                if ((c != LessThanSign) && (c != GreaterThanSign))
                {
                    return (c == EqualsThanSign);
                }
                return true;
            }

            /// <summary>
            /// Replaces the specified string with a new string.
            /// </summary>
            /// <param name="s">The string</param>
            /// <param name="oldString">The old string</param>
            /// <param name="newString">The new string</param>
            /// <returns>Returns replaced format string</returns>
            internal static string ReplaceKeyword(string s, string oldString, string newString)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return s;
                }
                string str = s;
                int startIndex = 0;
                while (true)
                {
                    int num2 = str.IndexOf(oldString, startIndex, StringComparison.CurrentCultureIgnoreCase);
                    if (num2 <= -1)
                    {
                        return str;
                    }
                    str = str.Remove(num2, oldString.Length).Insert(num2, newString);
                    startIndex = num2 + newString.Length;
                }
            }

            internal static string TrimSquareBracket(string token)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    if (token[0] == LeftSquareBracket)
                    {
                        token = token.TrimStart(new char[] { LeftSquareBracket });
                    }
                    if (token[token.Length - 1] == RightSquareBracket)
                    {
                        token = token.TrimEnd(new char[] { RightSquareBracket });
                    }
                }
                return token;
            }

            /// <summary>
            /// Gets the date time format information.
            /// </summary>
            /// <value>The date time format information.</value>
            internal static System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
            {
                get
                {
                    return CultureInfo.CurrentCulture.DateTimeFormat;
                }
            }

            /// <summary>
            /// Gets the decimal separator.
            /// </summary>
            /// <remarks>By default, the decimal separator is ".".</remarks>
            internal static string DecimalSeparator
            {
                get
                {
                    return NumberFormatInfo.NumberDecimalSeparator;
                }
            }

            /// <summary>
            /// Gets the exponential positive symbol.
            /// </summary>
            /// <remarks>By default, the exponential positive symbol is "e+".</remarks>
            internal static string Exponential1
            {
                get
                {
                    return "E+";
                }
            }

            /// <summary>
            /// Gets the exponential negative symbol.
            /// </summary>
            /// <remarks>By default, the exponential negative symbol is "e-".</remarks>
            internal static string Exponential2
            {
                get
                {
                    return "E-";
                }
            }

            /// <summary>
            /// Gets the na N symbol.
            /// </summary>
            /// <value>The na N symbol.</value>
            internal static string NaNSymbol
            {
                get
                {
                    return NumberFormatInfo.NaNSymbol;
                }
            }

            /// <summary>
            /// Gets the minus sign.
            /// </summary>
            /// <remarks>By default, the negative sign is "-".</remarks>
            internal static string NegativeSign
            {
                get
                {
                    return NumberFormatInfo.NegativeSign;
                }
            }

            /// <summary>
            /// Gets the number format information.
            /// </summary>
            /// <value>The number format information.</value>
            internal static System.Globalization.NumberFormatInfo NumberFormatInfo
            {
                get
                {
                    return CultureInfo.CurrentCulture.NumberFormat;
                }
            }

            /// <summary>
            /// Gets the number group separator.
            /// </summary>
            /// <remarks>By default, the separator is ",".</remarks>
            internal static string NumberGroupSeparator
            {
                get
                {
                    return NumberFormatInfo.NumberGroupSeparator;
                }
            }

            /// <summary>
            /// Gets the percent sign.
            /// </summary>
            /// <remarks>By default, the percent sign is "%".</remarks>
            internal static string PercentSymbol
            {
                get
                {
                    return NumberFormatInfo.PercentSymbol;
                }
            }

            /// <summary>
            /// Gets the plus sign.
            /// </summary>
            /// <remarks>By default, the positive sign is "+".</remarks>
            internal static string PositiveSign
            {
                get
                {
                    return NumberFormatInfo.PositiveSign;
                }
            }

            /// <summary>
            /// Gets the prefix of the placeholder.
            /// </summary>
            internal static string ReplacePlaceholder
            {
                get
                {
                    return "@";
                }
            }
        }

        /// <summary>
        /// Represents the converting of a value to a specified data type.
        /// </summary>
        internal static class FormatConverter
        {
            /// <summary>
            /// Determines whether the specified value is a numeric data type.
            /// </summary>
            /// <param name="value">Value for which to determine data type</param>
            /// <returns><c>true</c> if the value is numeric; otherwise <c>false</c>.</returns>
            internal static bool IsNumber(object value)
            {
                return ((((((value is double) || (value is float)) || ((value is decimal) || (value is long))) || (((value is int) || (value is short)) || ((value is sbyte) || (value is ulong)))) || (((value is uint) || (value is ushort)) || ((value is byte) || (value is DateTime)))) || (value is TimeSpan));
            }

            /// <summary>
            /// Converts the specified value to a date time value.
            /// </summary>
            /// <param name="value">Value to convert</param>
            /// <returns>Date time value equivalent to the specified value.</returns>
            /// <exception cref="T:System.InvalidCastException">Value cannot be converted.</exception>
            internal static DateTime ToDateTime(object value)
            {
                DateTime time;
                try
                {
                    if (value == null)
                    {
                        return DateTimeExtension.FromOADate(0.0);
                    }
                    if (value is double)
                    {
                        return DateTimeExtension.FromOADate((double) ((double) value));
                    }
                    if (value is float)
                    {
                        return DateTimeExtension.FromOADate((double) ((float) value));
                    }
                    if (value is decimal)
                    {
                        return DateTimeExtension.FromOADate((double) ((double) ((decimal) value)));
                    }
                    if (value is long)
                    {
                        return DateTimeExtension.FromOADate((double) ((long) value));
                    }
                    if (value is int)
                    {
                        return DateTimeExtension.FromOADate((double) ((int) value));
                    }
                    if (value is short)
                    {
                        return DateTimeExtension.FromOADate((double) ((short) value));
                    }
                    if (value is sbyte)
                    {
                        return DateTimeExtension.FromOADate((double) ((sbyte) value));
                    }
                    if (value is ulong)
                    {
                        return DateTimeExtension.FromOADate((double) ((ulong) value));
                    }
                    if (value is uint)
                    {
                        return DateTimeExtension.FromOADate((double) ((uint) value));
                    }
                    if (value is ushort)
                    {
                        return DateTimeExtension.FromOADate((double) ((ushort) value));
                    }
                    if (value is byte)
                    {
                        return DateTimeExtension.FromOADate((double) ((byte) value));
                    }
                    if (value is bool)
                    {
                        return DateTimeExtension.FromOADate(((bool) value) ? 1.0 : 0.0);
                    }
                    if (value is string)
                    {
                        return DateTime.Parse((string) ((string) value), CultureInfo.CurrentCulture);
                    }
                    if (value is DateTime)
                    {
                        return (DateTime) value;
                    }
                    if (value is TimeSpan)
                    {
                        TimeSpan span = (TimeSpan) value;
                        return DateTimeExtension.FromOADate(span.TotalDays);
                    }
                    time = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
                }
                catch
                {
                    throw new InvalidCastException();
                }
                return time;
            }

            /// <summary>
            /// Converts the specified value to a double-precision, floating-point number.
            /// </summary>
            /// <param name="value">Value to convert</param>
            /// <returns>Double-precision, floating-point number equivalent to the specified value.</returns>
            /// <exception cref="T:System.InvalidCastException">Value cannot be converted.</exception>
            internal static double ToDouble(object value)
            {
                double num;
                try
                {
                    if (value == null)
                    {
                        return 0.0;
                    }
                    if (value is double)
                    {
                        return (double) ((double) value);
                    }
                    if (value is float)
                    {
                        return (double) ((float) value);
                    }
                    if (value is decimal)
                    {
                        return (double) ((double) ((decimal) value));
                    }
                    if (value is long)
                    {
                        return (double) ((long) value);
                    }
                    if (value is int)
                    {
                        return (double) ((int) value);
                    }
                    if (value is short)
                    {
                        return (double) ((short) value);
                    }
                    if (value is sbyte)
                    {
                        return (double) ((sbyte) value);
                    }
                    if (value is ulong)
                    {
                        return (double) ((ulong) value);
                    }
                    if (value is uint)
                    {
                        return (double) ((uint) value);
                    }
                    if (value is ushort)
                    {
                        return (double) ((ushort) value);
                    }
                    if (value is byte)
                    {
                        return (double) ((byte) value);
                    }
                    if (value is bool)
                    {
                        return (((bool) value) ? 1.0 : 0.0);
                    }
                    if (value is string)
                    {
                        return double.Parse((string) ((string) value), NumberStyles.Any, CultureInfo.CurrentCulture);
                    }
                    if (value is DateTime)
                    {
                        return ((DateTime) value).ToOADate();
                    }
                    if (value is TimeSpan)
                    {
                        TimeSpan span = (TimeSpan) value;
                        return span.TotalDays;
                    }
                    num = Convert.ToDouble(value, CultureInfo.CurrentCulture);
                }
                catch
                {
                    throw new InvalidCastException();
                }
                return num;
            }

            /// <summary>
            /// Converts the specified value to a string representation.
            /// </summary>
            /// <param name="value">Value to convert</param>
            /// <returns>String representation of the specified value.</returns>
            /// <exception cref="T:System.InvalidCastException">Value cannot be converted.</exception>
            internal static string ToString(object value)
            {
                string str;
                try
                {
                    if (value == null)
                    {
                        return "";
                    }
                    if (value is bool)
                    {
                        return (((bool) value) ? "TRUE" : "FALSE");
                    }
                    if (value is string)
                    {
                        return (string) ((string) value);
                    }
                    if (value is CalcArray)
                    {
                        throw new InvalidCastException();
                    }
                    str = Convert.ToString(value, CultureInfo.CurrentCulture);
                }
                catch
                {
                    throw new InvalidCastException();
                }
                return str;
            }
        }

        /// <summary>
        /// Represents a format.
        /// </summary>
        internal abstract class FormatPartBase
        {
            /// <summary>
            /// 
            /// </summary>
            private string originalToken;

            /// <summary>
            /// Creates a new format.
            /// </summary>
            /// <param name="token">Token</param>
            protected FormatPartBase(string token)
            {
                this.originalToken = token;
            }

            /// <summary>
            /// Creates the specified token.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <returns>Returns the format part object.</returns>
            internal static FormatHelper.FormatPartBase Create(string token)
            {
                if (FormatHelper.DBNumberFormatPart.EvaluateFormat(token))
                {
                    return new FormatHelper.DBNumberFormatPart(token);
                }
                if (FormatHelper.LocaleIDFormatPart.EvaluateFormat(token))
                {
                    return new FormatHelper.LocaleIDFormatPart(token);
                }
                if (FormatHelper.ABSTimeFormatPart.EvaluateFormat(token))
                {
                    return new FormatHelper.ABSTimeFormatPart(token);
                }
                return null;
            }

            internal string OriginalToken
            {
                get
                {
                    return this.originalToken;
                }
            }
        }

        internal enum GeneralCompareType
        {
            LessThan,
            LessThanOrEqualsTo,
            EqualsTo,
            GreaterThanOrEqualsTo,
            GreaterThan,
            NotEqualsTo
        }

        /// <summary>
        /// Interface that defines the methods that convert a number string to another number string.
        /// </summary>
        internal interface INumberStringConverter
        {
            /// <summary>
            /// Converts a specified number string to a different representation.
            /// </summary>
            /// <param name="number">The formatted data string.</param>
            /// <param name="value">The original data value.</param>
            /// <param name="isGeneralNumber">Whether the number is a general format number.</param>
            /// <param name="locale">The locale information.</param>
            /// <param name="dbNumber">The number format information.</param>
            /// <returns>
            /// Returns the string that represents the original data value.
            /// </returns>
            string ConvertTo(string number, object value, bool isGeneralNumber, FormatHelper.LocaleIDFormatPart locale, FormatHelper.DBNumberFormatPart dbNumber);
        }

        internal sealed class LocaleIDFormatPart : FormatHelper.FormatPartBase
        {
            /// <summary>
            /// the content.
            /// </summary>
            private string content;
            /// <summary>
            /// culture info.
            /// </summary>
            private System.Globalization.CultureInfo cultureInfo;
            /// <summary>
            /// the utf16any
            /// </summary>
            private string currencySymbol;
            /// <summary>
            /// the locate id.
            /// </summary>
            private int locateID;

            /// <summary>
            /// Creates a new locale format with the specified token.
            /// </summary>
            /// <param name="token">The string expression for the format.</param>
            internal LocaleIDFormatPart(string token) : base(token)
            {
                this.locateID = -1;
                if (token == null)
                {
                    throw new ArgumentNullException("token");
                }
                if (token == string.Empty)
                {
                    throw new FormatException("token is illegal.");
                }
                this.content = FormatHelper.DefaultTokens.TrimSquareBracket(token);
                string content = this.content;
                if ((content == null) || (content == string.Empty))
                {
                    throw new FormatException("token is illegal.");
                }
                if (!FormatHelper.DefaultTokens.IsEquals(content[0], FormatHelper.DefaultTokens.Dollar, false))
                {
                    throw new FormatException("token is illegal.");
                }
                content = content.Remove(0, 1);
                int index = content.IndexOf(FormatHelper.DefaultTokens.HyphenMinus);
                if (index > -1)
                {
                    this.currencySymbol = content.Substring(0, index);
                    content = content.Remove(0, index);
                }
                else
                {
                    this.currencySymbol = content;
                    return;
                }
                if (!FormatHelper.DefaultTokens.IsEquals(content[0], FormatHelper.DefaultTokens.HyphenMinus, false))
                {
                    throw new FormatException("token is illegal.");
                }
                content = content.Remove(0, 1);
                if (content.Length <= 0)
                {
                    throw new FormatException("token is illegal.");
                }
                this.locateID = FormatHelper.NumberHelper.ParseHexString(content);
            }

            /// <summary>
            /// Encodes the symbol.
            /// </summary>
            /// <param name="symbol">The format string to encode</param>
            /// <returns>Returns the encoded format string.</returns>
            private string EncodeSymbol(string symbol)
            {
                return symbol.Replace(".", "'.'");
            }

            /// <summary>
            /// Determines whether the format string is valid.
            /// </summary>
            /// <param name="token">The token to evaluate.</param>
            /// <returns>
            /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
            /// </returns>
            internal static bool EvaluateFormat(string token)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }
                string str = FormatHelper.DefaultTokens.TrimSquareBracket(token);
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                return FormatHelper.DefaultTokens.IsEquals(str[0], FormatHelper.DefaultTokens.Dollar, false);
            }

            /// <summary>
            /// Gets a <see cref="T:Dt.CalcEngine.FormatHelper.DBNumber" /> object by the number letter type and culture information for this format.
            /// </summary>
            /// <param name="type">The number letter type.</param>
            /// <returns>
            /// Returns a <see cref="T:Dt.CalcEngine.FormatHelper.DBNumber" /> object that indicates how the number should be formatted.
            /// </returns>
            internal FormatHelper.DBNumber GetDBNumber(int type)
            {
                switch ((this.locateID & 0xff))
                {
                    case 4:
                        switch (type)
                        {
                            case 1:
                                return FormatHelper.DBNumber.ChineseDBNum1;

                            case 2:
                                return FormatHelper.DBNumber.ChineseDBNum2;

                            case 3:
                                return FormatHelper.DBNumber.ChineseDBNum3;
                        }
                        break;

                    case 0x11:
                        switch (type)
                        {
                            case 1:
                                return FormatHelper.DBNumber.JapaneseDBNum1;

                            case 2:
                                return FormatHelper.DBNumber.JapaneseDBNum2;

                            case 3:
                                return FormatHelper.DBNumber.JapaneseDBNum3;
                        }
                        break;
                }
                return null;
            }

            /// <summary>
            /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
            /// </returns>
            public override string ToString()
            {
                if (this.content != null)
                {
                    return FormatHelper.DefaultTokens.AddSquareBracket(this.content);
                }
                return string.Empty;
            }

            /// <summary>
            /// Gets the culture information for the format.
            /// </summary>
            /// <value>The culture information for the format.</value>
            internal System.Globalization.CultureInfo CultureInfo
            {
                get
                {
                    if (this.cultureInfo == null)
                    {
                        this.cultureInfo = FormatHelper.CultureHelper.CreateCultureInfo(this.locateID);
                        if (((this.currencySymbol != null) && (this.currencySymbol != string.Empty)) && !this.cultureInfo.NumberFormat.IsReadOnly)
                        {
                            this.cultureInfo.NumberFormat.CurrencySymbol = this.currencySymbol;
                        }
                        Calendar calendar = FormatHelper.CultureHelper.CreateCalendar(this.locateID);
                        if (calendar != null)
                        {
                            this.cultureInfo.DateTimeFormat.Calendar = calendar;
                        }
                    }
                    return this.cultureInfo;
                }
            }

            /// <summary>
            /// Gets the currency symbol for the format.
            /// </summary>
            /// <value>The currency symbol for the format. The default value is a dollar sign ($).</value>
            internal string CurrencySymbol
            {
                get
                {
                    if (this.currencySymbol != null)
                    {
                        return this.EncodeSymbol(this.currencySymbol);
                    }
                    return string.Empty;
                }
            }
        }

        internal abstract class NumberFormatBase
        {
            /// <summary>
            /// the date time format info.
            /// </summary>
            private System.Globalization.DateTimeFormatInfo dateTimeFormatInfo;
            /// <summary>
            /// the general format.
            /// </summary>
            internal const string General = "General";
            /// <summary>
            /// the number format info.
            /// </summary>
            private System.Globalization.NumberFormatInfo numberFormatInfo;
            /// <summary>
            /// the number string Converter.
            /// </summary>
            private FormatHelper.INumberStringConverter numberStringConverter;
            /// <summary>
            /// DBNum part.
            /// </summary>
            private object partDbNumberFormat;
            /// <summary>
            /// the part locale id.
            /// </summary>
            private object partLocaleID;

            /// <summary>
            /// Creates a new number format with the specified locale and db number formats.
            /// </summary>
            /// <param name="partLocaleID">The locale ID.</param>
            /// <param name="dbNumberFormatPart">The data number format.</param>
            protected NumberFormatBase(object partLocaleID, object dbNumberFormatPart)
            {
                this.partLocaleID = partLocaleID;
                this.partDbNumberFormat = dbNumberFormatPart;
            }

            /// <summary>
            /// Determines whether the specified format contains the specified text.
            /// </summary>
            /// <param name="format">Format</param>
            /// <param name="keywords">Keywords</param>
            /// <returns>
            /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
            /// </returns>
            internal static bool ContainsKeywords(string format, string[] keywords)
            {
                if ((format != null) && (format != string.Empty))
                {
                    StringBuilder builder = new StringBuilder();
                    bool flag = false;
                    char ch = '\0';
                    for (int i = 0; i < format.Length; i++)
                    {
                        char ch2 = format[i];
                        if (ch2 == '"')
                        {
                            flag = !flag;
                        }
                        else if ((!flag && (ch2 != FormatHelper.DefaultTokens.UnderLine)) && (ch != FormatHelper.DefaultTokens.UnderLine))
                        {
                            builder.Append(ch2);
                        }
                        ch = ch2;
                    }
                    string str = builder.ToString().ToLower();
                    foreach (string str2 in keywords)
                    {
                        if (str.Contains(str2))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// Formats the specified object as a string.
            /// </summary>
            /// <param name="obj">The object with cell data to format.</param>
            /// <returns>Returns the formatted string.</returns>
            internal abstract string Format(object obj);
            /// <summary>
            /// Determines whether the specified STR is transform.
            /// </summary>
            /// <param name="str">The STR</param>
            /// <param name="currentpos">The current position</param>
            /// <returns>
            /// <c>true</c> if the specified STR is transform; otherwise, <c>false</c>.
            /// </returns>
            internal static bool IsTransform(string str, int currentpos)
            {
                if (str[currentpos] == '\\')
                {
                    throw new ArgumentException(@"the '\' can't be evaluated");
                }
                if ((((currentpos - 1) > 0) && ((currentpos - 1) < str.Length)) && (str[currentpos - 1] == '\\'))
                {
                    if ((currentpos - 2) < 0)
                    {
                        return true;
                    }
                    if (((currentpos - 2) > 0) && ((currentpos - 2) < str.Length))
                    {
                        if (str[currentpos - 2] == '\\')
                        {
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Trims the not supported symbol.
            /// </summary>
            /// <param name="format">Format</param>
            /// <returns>Returns the format string without illegal char</returns>
            internal static string TrimNotSupportSymbol(string format)
            {
                return TrimNotSupportSymbol(format, true);
            }

            /// <summary>
            /// Trims the not supported symbol.
            /// </summary>
            /// <param name="format">Format</param>
            /// <param name="isSupportFraction">Whether the format supports fractions</param>
            /// <returns>Returns the format string without illegal char</returns>
            internal static string TrimNotSupportSymbol(string format, bool isSupportFraction)
            {
                bool flag = false;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < format.Length; i++)
                {
                    char ch = format[i];
                    bool flag2 = true;
                    if (ch == '"')
                    {
                        flag = !flag;
                    }
                    else if (!flag)
                    {
                        if (!isSupportFraction)
                        {
                            if ((ch == '?') && !IsTransform(format, i))
                            {
                                flag2 = false;
                            }
                            if ((ch == '/') && !IsTransform(format, i))
                            {
                                flag2 = false;
                            }
                        }
                        if (ch == '_')
                        {
                            if (!IsTransform(format, i))
                            {
                                flag2 = false;
                                i++;
                            }
                        }
                        else if ((ch == '*') && !IsTransform(format, i))
                        {
                            flag2 = false;
                        }
                    }
                    if (flag2)
                    {
                        builder.Append(ch);
                    }
                }
                return builder.ToString();
            }

            /// <summary>
            /// Gets the date-time format information for this formatter.
            /// </summary>
            /// <value>
            /// The date-time format information for this formatter. 
            /// The default value is the System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.
            /// </value>
            internal virtual System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
            {
                get
                {
                    return this.dateTimeFormatInfo;
                }
                set
                {
                    this.dateTimeFormatInfo = value;
                }
            }

            /// <summary>
            /// Gets the decimal separator.
            /// </summary>
            /// <value>The decimal separator. The default value is a period (.).</value>
            protected string DecimalSeparator
            {
                get
                {
                    if (this.NumberFormatInfo != null)
                    {
                        return this.NumberFormatInfo.NumberDecimalSeparator;
                    }
                    return FormatHelper.DefaultTokens.DecimalSeparator;
                }
            }

            /// <summary>
            /// Gets the NaN symbol.
            /// </summary>
            /// <value>The NaN symbol. The default value is "NaN".</value>
            protected string NaNSymbol
            {
                get
                {
                    if (this.NumberFormatInfo != null)
                    {
                        return this.NumberFormatInfo.NaNSymbol;
                    }
                    return FormatHelper.DefaultTokens.NaNSymbol;
                }
            }

            /// <summary>
            /// Gets the negative sign.
            /// </summary>
            /// <value>The negative sign. The default value is a minus sign (-).</value>
            protected string NegativeSign
            {
                get
                {
                    if (this.NumberFormatInfo != null)
                    {
                        return this.NumberFormatInfo.NegativeSign;
                    }
                    return FormatHelper.DefaultTokens.NegativeSign;
                }
            }

            /// <summary>
            /// Gets or sets the number format information for this formatter.
            /// </summary>
            /// <value>
            /// The number format information for this formatter. 
            /// The default value is the System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormatInfo.
            /// </value>
            internal virtual System.Globalization.NumberFormatInfo NumberFormatInfo
            {
                get
                {
                    return this.numberFormatInfo;
                }
                set
                {
                    this.numberFormatInfo = value;
                }
            }

            /// <summary>
            /// Gets the number group separator.
            /// </summary>
            /// <value>The number group separator. The default value is a semi-colon (;).</value>
            protected string NumberGroupSeparator
            {
                get
                {
                    if (this.NumberFormatInfo != null)
                    {
                        return this.NumberFormatInfo.NumberGroupSeparator;
                    }
                    return FormatHelper.DefaultTokens.NumberGroupSeparator;
                }
            }

            /// <summary>
            /// Gets or sets the number string converter.
            /// </summary>
            /// <value>The number string converter. The default value is null.</value>
            internal virtual FormatHelper.INumberStringConverter NumberStringConverter
            {
                get
                {
                    if (this.numberStringConverter != null)
                    {
                        return this.numberStringConverter;
                    }
                    return null;
                }
                set
                {
                    this.numberStringConverter = value;
                }
            }

            /// <summary>
            /// Gets the DB number format for the formatter.
            /// </summary>
            /// <value>The DB number format of the formatter. The default value is null.</value>
            internal FormatHelper.DBNumberFormatPart PartDBNumberFormat
            {
                get
                {
                    return (this.partDbNumberFormat as FormatHelper.DBNumberFormatPart);
                }
            }

            /// <summary>
            /// Gets the locale format for the formatter.
            /// </summary>
            /// <value>The locale format of the formatter. The default value is null.</value>
            internal FormatHelper.LocaleIDFormatPart PartLocaleID
            {
                get
                {
                    return (this.partLocaleID as FormatHelper.LocaleIDFormatPart);
                }
            }

            /// <summary>
            /// Gets the percent symbol.
            /// </summary>
            /// <value>The percent symbol. The default value is "%".</value>
            protected string PercentSymbol
            {
                get
                {
                    if (this.NumberFormatInfo != null)
                    {
                        return this.NumberFormatInfo.PercentSymbol;
                    }
                    return FormatHelper.DefaultTokens.PercentSymbol;
                }
            }

            /// <summary>
            /// Gets the positive sign.
            /// </summary>
            /// <value>The positive sign. The default value is a plus sign (+).</value>
            protected string PositiveSign
            {
                get
                {
                    if (this.NumberFormatInfo != null)
                    {
                        return this.NumberFormatInfo.PositiveSign;
                    }
                    return FormatHelper.DefaultTokens.PositiveSign;
                }
            }
        }

        internal sealed class NumberFormatDateTime : FormatHelper.NumberFormatBase
        {
            /// <summary>
            /// the absolute time.
            /// </summary>
            private DateTime? absoluteTime;
            /// <summary>
            /// the absTimeParts
            /// </summary>
            private FormatHelper.ABSTimeFormatPart[] absTimeParts;
            /// <summary>
            /// Custom format for short AM/PM symbol in excel.
            /// </summary>
            internal const string AMPMSingleDigit = "A/P";
            /// <summary>
            /// Custom format for default AM/PM symbol in excel.
            /// </summary>
            internal const string AMPMTwoDigit = "AM/PM";
            /// <summary>
            /// Custom format for short day in excel.
            /// </summary>
            internal const string DaySingleDigit = "d";
            /// <summary>
            /// Custom format for long day in excel.
            /// </summary>
            internal const string DayTwoDigit = "dd";
            /// <summary>
            /// Custom format for week day abbreviation name in excel.
            /// </summary>
            internal const string DayWeekDayAbbreviation = "aaa";
            /// <summary>
            /// Custom format for week day unabbreviated name in excel.
            /// </summary>
            internal const string DayWeekDayUnabbreviated = "aaaa";
            /// <summary>
            /// the absolute time.
            /// </summary>
            internal static readonly DateTime defaultAbsoluteTime = new DateTime(0x76b, 12, 30, 0, 0, 0, 0);
            /// <summary>
            /// the default converter.
            /// </summary>
            internal static readonly FormatHelper.INumberStringConverter defaultNumberStringConverter = new FormatHelper.DefaultDateTimeNumberStringConverter();
            /// <summary>
            /// Custom format for era year in excel.
            /// </summary>
            internal const string EraYear = "e";
            /// <summary>
            /// the format string.
            /// </summary>
            private string formatString;
            /// <summary>
            /// is the format has JD.
            /// </summary>
            private bool hasJD;
            /// <summary>
            /// whether the year formatting is delay.
            /// </summary>
            private bool hasYearDelay;
            /// <summary>
            /// Custom format for short hours in excel.
            /// </summary>
            internal const string HoursSingleDigit = "h";
            /// <summary>
            /// Custom format for long hours in excel.
            /// </summary>
            internal const string HoursTwoDigit = "hh";
            /// <summary>
            /// the date time keyword
            /// </summary>
            internal static readonly string[] keywords = new string[] { 
                "yy", "yyyy", "m", "mm", "mmm", "mmmm", "mmmmm", "d", "dd", "aaa", "aaaa", "h", "hh", "m", "mm", "s", 
                "ss"
             };
            /// <summary>
            /// Custom format for short minute in excel.
            /// </summary>
            private const string MinuteSingleDigit = "m";
            /// <summary>
            /// Custom format for long minute in excel.
            /// </summary>
            internal const string MinuteTwoDigit = "mm";
            /// <summary>
            /// Custom format for month abbreviation name in excel.
            /// </summary>
            internal const string MonthAbbreviation = "mmm";
            /// <summary>
            /// Custom format for the first char of month name in excel.
            /// </summary>
            internal const string MonthJD = "mmmmm";
            /// <summary>
            /// Custom format for short month in excel.
            /// </summary>
            internal const string MonthSingleDigit = "m";
            /// <summary>
            /// Custom format for long month in excel.
            /// </summary>
            internal const string MonthTwoDigit = "mm";
            /// <summary>
            /// Custom format for month unabbreviated name in excel.
            /// </summary>
            internal const string MonthUnabbreviated = "mmmm";
            /// <summary>
            /// the placeholder for month JD.
            /// </summary>
            internal static readonly string PlaceholderMonthJD = (FormatHelper.DefaultTokens.ReplacePlaceholder + "mmmmm");
            /// <summary>
            /// Custom format for short second in excel.
            /// </summary>
            internal const string SecondSingleDigit = "s";
            /// <summary>
            /// Custom format for long second in excel.
            /// </summary>
            private const string SecondTwoDigit = "ss";
            /// <summary>
            /// Custom format for AM/PM symbol in string.format.
            /// </summary>
            internal const string StandardAMPMSingleDigit = "tt";
            /// <summary>
            /// Custom format for week day abbreviation name in excel.
            /// </summary>
            internal const string StandardDayWeekDayAbbreviation = "ddd";
            /// <summary>
            /// Custom format for week day unabbreviated name in excel.
            /// </summary>
            internal const string StandardDayWeekDayUnabbreviated = "dddd";
            /// <summary>
            /// Custom format for short hour in string.format.
            /// </summary>
            internal const string StandardHourSingleDigit = "H";
            /// <summary>
            /// Custom format for short hour in string.format.
            /// </summary>
            internal const string StandardHourTwoDigit = "HH";
            /// <summary>
            /// Custom format for short minute in string.format.
            /// </summary>
            internal const string StandardMinuteSingleDigit = "%m";
            /// <summary>
            /// Custom format for month abbreviation name in string.format.
            /// </summary>
            internal const string StandardMonthAbbreviation = "MMM";
            /// <summary>
            /// Custom format for short month in string.format.
            /// </summary>
            internal const string StandardMonthSingleDigit = "%M";
            /// <summary>
            /// Custom format for long month in string.format.
            /// </summary>
            internal const string StandardMonthTwoDigit = "MM";
            /// <summary>
            /// Custom format for month unabbreviated name in string.format.
            /// </summary>
            internal const string StandardMonthUnabbreviated = "MMMM";
            /// <summary>
            /// Custom format for short second in string.format.
            /// </summary>
            internal const string StandardSecondSingleDigit = "%s";
            /// <summary>
            /// Custom format for short sub second in string.format.
            /// </summary>
            internal const string StandardSubSecondSingleDigit = ".f";
            /// <summary>
            /// Custom format for full sub second in string.format.
            /// </summary>
            internal const string StandardSubSecondThreeDigit = ".fff";
            /// <summary>
            /// Custom format for long sub second in string.format.
            /// </summary>
            internal const string StandardSubSecondTwoDigit = ".ff";
            /// <summary>
            /// Custom format for short year in string.format.
            /// </summary>
            internal const string StandardYearSingleDigit = "%y";
            /// <summary>
            /// Custom format for short sub-second in excel.
            /// </summary>
            internal const string SubSecondSingleDigit = ".0";
            /// <summary>
            /// Custom format for full sub-second in excel.
            /// </summary>
            internal const string SubSecondThreeDigit = ".000";
            /// <summary>
            /// Custom format for long sub-second in excel.
            /// </summary>
            internal const string SubSecondTwoDigit = ".00";
            /// <summary>
            /// the valid date time format string.
            /// </summary>
            private string validDateTimeFormatString;
            /// <summary>
            /// Custom format for long year in excel.
            /// </summary>
            internal const string YearFourDigit = "yyyy";
            /// <summary>
            /// Custom format for short year in excel.
            /// </summary>
            internal const string YearSingleDigit = "y";
            /// <summary>
            /// Custom format for short year in excel.
            /// </summary>
            internal const string YearTwoDigit = "yy";

            internal NumberFormatDateTime(string format, FormatHelper.ABSTimeFormatPart[] absTimeParts, object partLocaleID, object dbNumberFormatPart) : base(partLocaleID, dbNumberFormatPart)
            {
                this.absoluteTime = null;
                this.formatString = this.FixFormat(FormatHelper.NumberFormatBase.TrimNotSupportSymbol(format));
                string formatString = this.formatString;
                this.absTimeParts = absTimeParts;
                if (!EvaluateFormat(formatString))
                {
                    throw new ArgumentException("format is illegal.");
                }
                this.ProcessAMPM(ref formatString);
                this.hasJD = this.Replace(formatString, "mmmmm", "\"" + PlaceholderMonthJD + "\"", true, false, out formatString, false, false);
                this.Replace(formatString, "mmmm", "MMMM", true, false, out formatString, false, false);
                this.Replace(formatString, "mmm", "MMM", true, false, out formatString, false, false);
                this.Replace(formatString, "mm", "MM", true, false, out formatString, false, false);
                this.Replace(formatString, "m", "%M", true, false, out formatString, false, false);
                this.Replace(formatString, "aaa", "ddd", true, true, out formatString, false, true);
                this.Replace(formatString, "aaaa", "dddd", true, true, out formatString, false, true);
                this.Replace(formatString, "m", "%m", false, true, out formatString, false, false);
                this.Replace(formatString, "h", "H", true, true, out formatString, false, false);
                this.Replace(formatString, "hh", "HH", true, true, out formatString, false, false);
                this.Replace(formatString, "s", "%s", true, true, out formatString, false, true);
                this.Replace(formatString, ".000", ".fff", true, true, out formatString, false, true);
                this.Replace(formatString, ".00", ".ff", true, true, out formatString, false, true);
                this.Replace(formatString, ".0", ".f", true, true, out formatString, false, true);
                if ((base.PartDBNumberFormat != null) && (base.PartLocaleID != null))
                {
                    this.hasYearDelay |= this.Replace(formatString, "yyyy", "\"" + FormatHelper.DefaultTokens.ReplacePlaceholder + "yyyy\"", true, false, out formatString, false, true);
                    this.hasYearDelay |= this.Replace(formatString, "yy", "\"" + FormatHelper.DefaultTokens.ReplacePlaceholder + "yy\"", true, false, out formatString, false, true);
                }
                this.Replace(formatString, "e", "yyyy", true, false, out formatString, false, true);
                this.Replace(formatString, "ee", "yyyy", true, false, out formatString, false, true);
                this.Replace(formatString, "eee", "yyyy", true, false, out formatString, false, true);
                this.Replace(formatString, "y", "%y", true, false, out formatString, false, true);
                if (this.absTimeParts != null)
                {
                    foreach (FormatHelper.ABSTimeFormatPart part in this.absTimeParts)
                    {
                        this.Replace(formatString, part.Token, "\"" + FormatHelper.DefaultTokens.ReplacePlaceholder + part.Token + "\"", true, true, out formatString, false, true);
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
            internal static bool EvaluateFormat(string format)
            {
                return FormatHelper.NumberFormatBase.ContainsKeywords(format, keywords);
            }

            /// <summary>
            /// Fixes the format.
            /// </summary>
            /// <param name="format">The format</param>
            /// <returns>Returns the fixed format string</returns>
            private string FixFormat(string format)
            {
                string str = format;
                StringBuilder builder = new StringBuilder();
                bool flag = false;
                char ch2 = "h"[0];
                if (str.IndexOf(((char) ch2).ToString(), StringComparison.CurrentCultureIgnoreCase) <= -1)
                {
                    char ch3 = "s"[0];
                    str.IndexOf(((char) ch3).ToString(), StringComparison.CurrentCultureIgnoreCase);
                }
                char ch4 = "yy"[0];
                if (str.IndexOf(((char) ch4).ToString(), StringComparison.CurrentCultureIgnoreCase) <= -1)
                {
                    char ch5 = "d"[0];
                    str.IndexOf(((char) ch5).ToString(), StringComparison.CurrentCultureIgnoreCase);
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
                                break;

                            case 'M':
                                if (i > 1)
                                {
                                    if (!FormatHelper.DefaultTokens.IsEquals('A', str[i - 1], true) && !FormatHelper.DefaultTokens.IsEquals('P', str[i - 1], true))
                                    {
                                        c = char.ToLower(c);
                                    }
                                }
                                else
                                {
                                    c = char.ToLower(c);
                                }
                                break;

                            case 't':
                            case 'f':
                            case 'F':
                            case 'K':
                            case 'z':
                            {
                                builder.Append('\'');
                                builder.Append(c);
                                builder.Append('\'');
                                continue;
                            }
                        }
                    }
                    builder.Append(c);
                }
                return builder.ToString();
            }

            /// <summary>
            /// Formats the specified value.
            /// </summary>
            /// <param name="obj">The object</param>
            /// <returns>Returns the string of the formatted value.</returns>
            internal override string Format(object obj)
            {
                string number = string.Empty;
                DateTime now = DateTime.Now;
                try
                {
                    now = FormatHelper.FormatConverter.ToDateTime(obj);
                    number = now.ToString(this.validDateTimeFormatString, this.DateTimeFormatInfo);
                    if (this.hasJD)
                    {
                        string monthName = FormatHelper.DefaultTokens.DateTimeFormatInfo.GetMonthName(now.Month);
                        number = number.Replace(PlaceholderMonthJD, monthName.Substring(0, 1));
                    }
                    if (this.absTimeParts != null)
                    {
                        TimeSpan span = (TimeSpan) (now - this.AbsoluteTime);
                        foreach (FormatHelper.ABSTimeFormatPart part in this.absTimeParts)
                        {
                            string newValue = null;
                            switch (part.TimePartType)
                            {
                                case FormatHelper.TimePart.Hour:
                                    newValue = ((double) span.TotalHours).ToString(part.FormatString);
                                    break;

                                case FormatHelper.TimePart.Minute:
                                    newValue = ((double) span.TotalMinutes).ToString(part.FormatString);
                                    break;

                                case FormatHelper.TimePart.Second:
                                    newValue = ((double) span.TotalSeconds).ToString(part.FormatString);
                                    break;
                            }
                            if (newValue != null)
                            {
                                number = number.Replace(FormatHelper.DefaultTokens.ReplacePlaceholder + part.Token, newValue);
                            }
                        }
                    }
                }
                catch
                {
                    number = FormatHelper.FormatConverter.ToString(obj);
                }
                if (this.NumberStringConverter is FormatHelper.DefaultDateTimeNumberStringConverter)
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
                    number = number.Replace(FormatHelper.DefaultTokens.ReplacePlaceholder + "yyyy", now.ToString("yyyy")).Replace(FormatHelper.DefaultTokens.ReplacePlaceholder + "yy", now.ToString("yy"));
                }
                return this.NumberStringConverter.ConvertTo(number, obj, true, base.PartLocaleID, base.PartDBNumberFormat);
            }

            /// <summary>
            /// Processes the AMPM.
            /// </summary>
            /// <param name="format">The format</param>
            /// <returns>
            /// <c>true</c> if the specified format contains the Am/Pm information; otherwise, <c>false</c>.
            /// </returns>
            private bool ProcessAMPM(ref string format)
            {
                bool flag = false;
                if (format.Contains("AM/PM"))
                {
                    format = format.Replace("AM/PM", "tt");
                    flag = true;
                }
                if (format.Contains("A/P"))
                {
                    format = format.Replace("A/P", "tt");
                    if (this.DateTimeFormatInfo != null)
                    {
                        if (this.DateTimeFormatInfo.IsReadOnly)
                        {
                            this.DateTimeFormatInfo = this.DateTimeFormatInfo.Clone() as System.Globalization.DateTimeFormatInfo;
                        }
                        this.DateTimeFormatInfo.AMDesignator = "A/P".Substring(0, 1);
                        this.DateTimeFormatInfo.PMDesignator = "A/P".Substring(2, 1);
                    }
                    flag = true;
                }
                if (!format.Contains(this.CurrentAMPM))
                {
                    return flag;
                }
                format = format.Replace(this.CurrentAMPM, "tt");
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
            private bool Replace(string format, string oldToken, string newToken, bool isReplaceInDateFormat, bool isReplaceInTimeFormat, out string result, bool justSearch, bool isIgnoreCase)
            {
                result = format;
                if (!isReplaceInDateFormat && !isReplaceInTimeFormat)
                {
                    return false;
                }
                List<int> list = new List<int>();
                bool flag = true;
                char ch3 = "h"[0];
                bool flag2 = (result.IndexOf(((char) ch3).ToString(), StringComparison.CurrentCultureIgnoreCase) > -1) || (result.IndexOf(((char) "s"[0]).ToString(), StringComparison.CurrentCultureIgnoreCase) > -1);
                char ch5 = "yy"[0];
                if (((result.IndexOf(((char) ch5).ToString(), StringComparison.CurrentCultureIgnoreCase) <= -1) && (result.IndexOf(((char) "d"[0]).ToString(), StringComparison.CurrentCultureIgnoreCase) <= -1)) && flag2)
                {
                    flag = false;
                }
                bool flag4 = false;
                int num = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    char a = result[i];
                    if (FormatHelper.DefaultTokens.IsEquals(a, "h"[0], true) || FormatHelper.DefaultTokens.IsEquals(a, "s"[0], true))
                    {
                        flag = false;
                    }
                    else if (FormatHelper.DefaultTokens.IsEquals(a, "yy"[0], true) || FormatHelper.DefaultTokens.IsEquals(a, "d"[0], true))
                    {
                        flag = true;
                    }
                    if (((isReplaceInDateFormat && FormatHelper.DefaultTokens.IsEquals(a, oldToken[num], isIgnoreCase)) && flag) || ((isReplaceInTimeFormat && FormatHelper.DefaultTokens.IsEquals(a, oldToken[num], isIgnoreCase)) && !flag))
                    {
                        bool flag5 = true;
                        for (int j = 0; j < oldToken.Length; j++)
                        {
                            if (((j + i) >= format.Length) || !FormatHelper.DefaultTokens.IsEquals(oldToken[j], result[j + i], isIgnoreCase))
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
                                if (!FormatHelper.DefaultTokens.IsEquals(ch2, result[num5], isIgnoreCase))
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
            private DateTime AbsoluteTime
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
            private string CurrentAMPM
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
                        return string.Format("{0}/{1}", new object[] { dateTimeFormatInfo.AMDesignator, dateTimeFormatInfo.PMDesignator });
                    }
                    return "AM/PM";
                }
            }

            /// <summary>
            /// Gets the format provider.
            /// </summary>
            /// <value>The format provider.</value>
            internal override System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
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
                    return FormatHelper.DefaultTokens.DateTimeFormatInfo;
                }
                set
                {
                    base.DateTimeFormatInfo = value;
                }
            }

            /// <summary>
            /// Gets or sets the number string converter.
            /// </summary>
            /// <value>The number string converter.</value>
            internal override FormatHelper.INumberStringConverter NumberStringConverter
            {
                get
                {
                    if (base.NumberStringConverter != null)
                    {
                        return base.NumberStringConverter;
                    }
                    return defaultNumberStringConverter;
                }
                set
                {
                    base.NumberStringConverter = value;
                }
            }
        }

        /// <summary>
        /// Represents a number format.  
        /// </summary>
        internal sealed class NumberFormatDigital : FormatHelper.NumberFormatBase
        {
            /// <summary>
            /// the default converter.
            /// </summary>
            private static readonly FormatHelper.INumberStringConverter defaultNumberStringConverter = new FormatHelper.DefaultNumberStringConverter();
            /// <summary>
            /// the excel format string.
            /// </summary>
            private string excelFormatString;
            /// <summary>
            /// denominator format.
            /// </summary>
            private string fractionDenominatorFormat;
            /// <summary>
            /// integer format.
            /// </summary>
            private string fractionIntegerFormat;
            /// <summary>
            /// numerator format.
            /// </summary>
            private string fractionNumeratorFormat;
            /// <summary>
            /// 
            /// </summary>
            private string fullFormatString;
            /// <summary>
            /// whether has "General" keyword.
            /// </summary>
            private bool isGeneralNumber;
            /// <summary>
            /// the date time keyword
            /// </summary>
            private static readonly string[] keywords = new string[] { FormatHelper.DefaultTokens.Exponential1, FormatHelper.DefaultTokens.Exponential2, ((char) FormatHelper.DefaultTokens.NumberSign).ToString(), FormatHelper.DefaultTokens.DecimalSeparator, FormatHelper.DefaultTokens.NumberGroupSeparator, FormatHelper.DefaultTokens.PercentSymbol, ((char) FormatHelper.DefaultTokens.Zero).ToString(), ((char) FormatHelper.DefaultTokens.SolidusSign).ToString() };
            /// <summary>
            /// the valid date time format string.
            /// </summary>
            private string numberFormatString;

            internal NumberFormatDigital(string format, object partLocaleID, object dbNumberFormatPart) : base(partLocaleID, dbNumberFormatPart)
            {
                this.excelFormatString = string.Empty;
                string s = FormatHelper.NumberFormatBase.TrimNotSupportSymbol(format);
                this.fullFormatString = FormatHelper.DefaultTokens.Filter(format, FormatHelper.DefaultTokens.LeftSquareBracket, FormatHelper.DefaultTokens.RightSquareBracket);
                this.excelFormatString = s;
                if (partLocaleID != null)
                {
                    string str2 = s;
                    s = FormatHelper.DefaultTokens.ReplaceKeyword(str2, base.PartLocaleID.OriginalToken, base.PartLocaleID.CurrencySymbol);
                }
                if (base.PartDBNumberFormat != null)
                {
                    this.excelFormatString = FormatHelper.DefaultTokens.ReplaceKeyword(this.excelFormatString, base.PartDBNumberFormat.OriginalToken, base.PartDBNumberFormat.ToString());
                }
                s = FormatHelper.DefaultTokens.Filter(s, FormatHelper.DefaultTokens.LeftSquareBracket, FormatHelper.DefaultTokens.RightSquareBracket);
                if (s.IndexOf(FormatHelper.DefaultTokens.SolidusSign) > -1)
                {
                    s = s.Replace(FormatHelper.DefaultTokens.QuestionMark, FormatHelper.DefaultTokens.Zero);
                    string[] strArray = s.Split(new char[] { FormatHelper.DefaultTokens.SolidusSign });
                    if ((strArray != null) && (strArray.Length == 2))
                    {
                        this.fractionDenominatorFormat = strArray[1];
                        string str3 = strArray[0];
                        if (str3 != null)
                        {
                            int length = str3.LastIndexOf(FormatHelper.DefaultTokens.Space);
                            if (length > -1)
                            {
                                this.fractionIntegerFormat = str3.Substring(0, length);
                                this.fractionNumeratorFormat = str3.Substring(length + 1, (str3.Length - length) - 1);
                            }
                            else
                            {
                                this.fractionNumeratorFormat = str3;
                            }
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
            private string EncodeNumberFormat(string format)
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
            internal static bool EvaluateFormat(string format)
            {
                return FormatHelper.NumberFormatBase.ContainsKeywords(format, keywords);
            }

            /// <summary>
            /// Formats the specified value.
            /// </summary>
            /// <param name="obj">The object</param>
            /// <returns>Returns the string of the formatted value.</returns>
            internal override string Format(object obj)
            {
                double num = FormatHelper.FormatConverter.ToDouble(obj);
                string naNSymbol = base.NaNSymbol;
                if ((this.fractionNumeratorFormat != null) && (this.fractionDenominatorFormat != null))
                {
                    double num2;
                    double num3;
                    double num4;
                    int length = this.fractionDenominatorFormat.Length;
                    if (!GetFraction(num, length, out num2, out num3, out num4))
                    {
                        return ((double) num).ToString(this.NumberFormatInfo);
                    }
                    if (this.fractionIntegerFormat != null)
                    {
                        StringBuilder builder = new StringBuilder();
                        if (num2 != 0.0)
                        {
                            builder.Append(((double) num2).ToString(this.fractionIntegerFormat, this.NumberFormatInfo));
                            builder.Append(FormatHelper.DefaultTokens.Space);
                        }
                        builder.Append(((double) num3).ToString(this.fractionNumeratorFormat, this.NumberFormatInfo));
                        builder.Append(FormatHelper.DefaultTokens.SolidusSign);
                        builder.Append(((double) num4).ToString(this.fractionDenominatorFormat, this.NumberFormatInfo));
                        return builder.ToString();
                    }
                    StringBuilder builder2 = new StringBuilder();
                    double num6 = (num2 * num4) + num3;
                    builder2.Append(((double) num6).ToString(this.fractionNumeratorFormat, this.NumberFormatInfo));
                    builder2.Append(FormatHelper.DefaultTokens.SolidusSign);
                    builder2.Append(((double) num4).ToString(this.fractionDenominatorFormat, this.NumberFormatInfo));
                    return builder2.ToString();
                }
                naNSymbol = ((double) num).ToString(this.EncodeNumberFormat(this.numberFormatString), this.NumberFormatInfo);
                if (this.NumberStringConverter != null)
                {
                    naNSymbol = this.NumberStringConverter.ConvertTo(naNSymbol, obj, this.isGeneralNumber, base.PartLocaleID, base.PartDBNumberFormat);
                }
                return naNSymbol;
            }

            /// <summary>
            /// Gets the fraction.
            /// </summary>
            /// <param name="value">The value</param>
            /// <param name="denominatorDigits">The denominator digits</param>
            /// <param name="integer">The integer</param>
            /// <param name="numerator">The numerator</param>
            /// <param name="denominator">The denominator</param>
            /// <returns>Returns the fraction.</returns>
            private static bool GetFraction(double value, int denominatorDigits, out double integer, out double numerator, out double denominator)
            {
                return FormatHelper.NumberHelper.GetFraction(value, denominatorDigits, out integer, out numerator, out denominator);
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is a general number.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is a general number; otherwise, <c>false</c>.
            /// </value>
            internal bool IsGeneralNumber
            {
                get
                {
                    return this.isGeneralNumber;
                }
                set
                {
                    this.isGeneralNumber = value;
                }
            }

            /// <summary>
            /// Gets the format provider.
            /// </summary>
            /// <value>The format provider.</value>
            internal override System.Globalization.NumberFormatInfo NumberFormatInfo
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
                    return FormatHelper.DefaultTokens.NumberFormatInfo;
                }
                set
                {
                    base.NumberFormatInfo = value;
                }
            }

            /// <summary>
            /// Gets or sets the number string converter.
            /// </summary>
            /// <value>The number string converter.</value>
            internal override FormatHelper.INumberStringConverter NumberStringConverter
            {
                get
                {
                    if (base.NumberStringConverter != null)
                    {
                        return base.NumberStringConverter;
                    }
                    return defaultNumberStringConverter;
                }
                set
                {
                    base.NumberStringConverter = value;
                }
            }
        }

        /// <summary>
        /// Represents a General number format.  
        /// </summary>
        internal sealed class NumberFormatGeneral : FormatHelper.NumberFormatBase
        {
            /// <summary>
            /// the number formatter.
            /// </summary>
            private FormatHelper.NumberFormatDigital digitalFormat;
            /// <summary>
            /// the formatter for exponential.
            /// </summary>
            private FormatHelper.NumberFormatDigital exponentialDigitalFormat;
            /// <summary>
            /// the full format string.
            /// </summary>
            private string fullFormatString;
            /// <summary>
            /// 
            /// </summary>
            private const string GeneralNumber = "##########0.###########";

            /// <summary>
            /// Creates a new normal general number format.
            /// </summary>
            internal NumberFormatGeneral() : base(null, null)
            {
                this.fullFormatString = "General";
            }

            /// <summary>
            /// Creates a new general number format.
            /// </summary>
            /// <param name="format">The format</param>
            /// <param name="partLocaleID">Locale ID</param>
            /// <param name="dbNumberFormatPart">db number format</param>
            internal NumberFormatGeneral(string format, object partLocaleID, object dbNumberFormatPart) : base(partLocaleID, dbNumberFormatPart)
            {
                if ((format == null) || (format == string.Empty))
                {
                    throw new FormatException("format is illegal.");
                }
                if (!EvaluateFormat(format))
                {
                    throw new ArgumentException("format is illegal.");
                }
                if ((format.Contains(((char) FormatHelper.DefaultTokens.Zero).ToString()) || format.Contains(((char) FormatHelper.DefaultTokens.NumberSign).ToString())) || (format.Contains(FormatHelper.DefaultTokens.DecimalSeparator) || format.Contains(((char) FormatHelper.DefaultTokens.CommercialAt).ToString())))
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
            internal static bool EvaluateFormat(string format)
            {
                if ((format == null) || (format == string.Empty))
                {
                    return false;
                }
                return FormatHelper.NumberFormatBase.ContainsKeywords(format, new string[] { "General".ToLower() });
            }

            /// <summary>
            /// Formats the specified value.
            /// </summary>
            /// <param name="obj">The object</param>
            /// <returns>
            /// Returns the string of the formatted value.
            /// </returns>
            internal override string Format(object obj)
            {
                string str = string.Empty;
                if (FormatHelper.FormatConverter.IsNumber(obj))
                {
                    if (Math.Abs(FormatHelper.FormatConverter.ToDouble(obj)) > 99999999999)
                    {
                        return this.ExponentialDigitalFormat.Format(obj);
                    }
                    return this.DigitalFormat.Format(obj);
                }
                if (obj is string)
                {
                    return obj.ToString();
                }
                if (obj is bool)
                {
                    bool flag = (bool) ((bool) obj);
                    return flag.ToString().ToUpper();
                }
                if (obj is CalcError)
                {
                    str = ((CalcError) obj).ToString();
                }
                return str;
            }

            /// <summary>
            /// Gets the format information.
            /// </summary>
            /// <value>The format information.</value>
            internal override System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
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
                    return FormatHelper.DefaultTokens.DateTimeFormatInfo;
                }
                set
                {
                    base.DateTimeFormatInfo = value;
                }
            }

            /// <summary>
            /// Gets the digital format.
            /// </summary>
            /// <value>The digital format.</value>
            private FormatHelper.NumberFormatDigital DigitalFormat
            {
                get
                {
                    if (this.digitalFormat == null)
                    {
                        string format = FormatHelper.DefaultTokens.ReplaceKeyword(this.fullFormatString, "General", "##########0.###########");
                        this.digitalFormat = new FormatHelper.NumberFormatDigital(format, base.PartLocaleID, base.PartDBNumberFormat);
                        this.digitalFormat.IsGeneralNumber = true;
                    }
                    return this.digitalFormat;
                }
            }

            /// <summary>
            /// Gets the digital format.
            /// </summary>
            /// <value>The digital format.</value>
            private FormatHelper.NumberFormatDigital ExponentialDigitalFormat
            {
                get
                {
                    if (this.exponentialDigitalFormat == null)
                    {
                        this.exponentialDigitalFormat = new FormatHelper.NumberFormatDigital("0.00E+00", base.PartLocaleID, base.PartDBNumberFormat);
                        this.exponentialDigitalFormat.IsGeneralNumber = true;
                    }
                    return this.exponentialDigitalFormat;
                }
            }

            /// <summary>
            /// Gets the format provider.
            /// </summary>
            /// <value>The format provider.</value>
            internal override System.Globalization.NumberFormatInfo NumberFormatInfo
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
                    return FormatHelper.DefaultTokens.NumberFormatInfo;
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
            internal override FormatHelper.INumberStringConverter NumberStringConverter
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

        /// <summary>
        /// Represents a text number format.  
        /// </summary>
        internal sealed class NumberFormatText : FormatHelper.NumberFormatBase
        {
            /// <summary>
            /// the @ symbol
            /// </summary>
            private const string CommercialAt = "@";
            /// <summary>
            /// the format string
            /// </summary>
            private string formatString;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.FormatHelper.NumberFormatText" /> class.
            /// </summary>
            /// <param name="format">The format string</param>
            /// <param name="partLocaleID">The part locale ID.</param>
            /// <param name="dbNumberFormatPart">The db number format part.</param>
            internal NumberFormatText(string format, object partLocaleID, object dbNumberFormatPart) : base(partLocaleID, dbNumberFormatPart)
            {
                if (format == null)
                {
                    throw new ArgumentNullException("format");
                }
                string s = FormatHelper.NumberFormatBase.TrimNotSupportSymbol(format, false);
                if (partLocaleID != null)
                {
                    s = FormatHelper.DefaultTokens.ReplaceKeyword(s, base.PartLocaleID.OriginalToken, base.PartLocaleID.CurrencySymbol);
                }
                this.formatString = s;
            }

            /// <summary>
            /// Determines whether the format string is valid.
            /// </summary>
            /// <param name="format">The token to evaluate.</param>
            /// <returns>
            /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
            /// </returns>
            internal static bool EvaluateFormat(string format)
            {
                return true;
            }

            /// <summary>
            /// Formats the specified value.
            /// </summary>
            /// <param name="value">The value</param>
            /// <returns>Returns the formatted value.</returns>
            internal override string Format(object value)
            {
                try
                {
                    string newValue = FormatHelper.FormatConverter.ToString(value);
                    string str2 = this.formatString.Replace("\"", "");
                    if (str2 != null)
                    {
                        newValue = str2.Replace("@", newValue);
                    }
                    return newValue;
                }
                catch
                {
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Represents a number helper class.
        /// </summary>
        private static class NumberHelper
        {
            /// <summary>
            /// Formats the specified formatted string.
            /// </summary>
            /// <typeparam name="T">The type of the formattable object</typeparam>
            /// <param name="t">The formattable object.</param>
            /// <param name="formatString">The format string.</param>
            /// <param name="formatProvider">The format provider.</param>
            /// <returns>Returns the formatted string.</returns>
            internal static string Format<T>(T t, string formatString, IFormatProvider formatProvider) where T: IFormattable
            {
                return t.ToString(formatString, formatProvider);
            }

            /// <summary>
            /// Gets the fraction.
            /// </summary>
            /// <param name="value">The value</param>
            /// <param name="denominatorDigits">The denominator digits (1-3)</param>
            /// <param name="integer">The integer</param>
            /// <param name="numerator">The numerator</param>
            /// <param name="denominator">The denominator</param>
            /// <returns>Return the fraction number information</returns>
            internal static bool GetFraction(double value, int denominatorDigits, out double integer, out double numerator, out double denominator)
            {
                integer = (int) value;
                numerator = 0.0;
                denominator = 0.0;
                double num = value - ((int) value);
                double num2 = 2.0;
                double num3 = 9.0;
                num2 = Math.Pow(10.0, (double) (denominatorDigits - 1));
                num3 = Math.Pow(10.0, (double) denominatorDigits) - 1.0;
                if (num2 < 2.0)
                {
                    num2 = 2.0;
                }
                if (num3 < 2.0)
                {
                    num3 = 2.0;
                }
                bool flag = false;
                double num4 = 0.0;
                for (double i = num2; i <= num3; i++)
                {
                    double a = i * num;
                    double num7 = Math.Round(a);
                    double num8 = num7 / i;
                    double num9 = Math.Abs((double) (num8 - num));
                    if (flag ? (num9 < Math.Abs((double) (num4 - num))) : true)
                    {
                        flag = true;
                        num4 = num8;
                        numerator = num7;
                        denominator = i;
                    }
                }
                return flag;
            }

            /// <summary>
            /// Parses the hex char.
            /// </summary>
            /// <param name="c">The char to parse.</param>
            /// <returns>Returns the hex value</returns>
            internal static int ParseHexChar(char c)
            {
                int num = c;
                if ((num >= 0x30) && (num <= 0x39))
                {
                    return (num - 0x30);
                }
                if ((num >= 0x61) && (num <= 0x66))
                {
                    return ((num - 0x61) + 10);
                }
                if ((num < 0x41) || (num > 70))
                {
                    throw new FormatException("char is illegal.");
                }
                return ((num - 0x41) + 10);
            }

            /// <summary>
            /// Parses the hex string.
            /// </summary>
            /// <param name="str">The string to parse.</param>
            /// <returns>Returns the hex value</returns>
            internal static int ParseHexString(string str)
            {
                if ((str == null) || (str == string.Empty))
                {
                    throw new FormatException("string is illegal.");
                }
                int num = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    num += ((int) ParseHexChar(str[(str.Length - i) - 1])) << (i * 4);
                }
                return num;
            }
        }

        /// <summary>
        /// Specifies the time part as hour, minute, or second.
        /// </summary>
        internal enum TimePart
        {
            Hour,
            Minute,
            Second
        }
    }
}

