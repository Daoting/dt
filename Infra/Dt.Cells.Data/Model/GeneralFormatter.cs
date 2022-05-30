#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a general formatter.
    /// </summary>
    /// <remarks>
    /// {PositiveNumbers; NegativeNumbers; Zero; Text]
    /// </remarks>
    public class GeneralFormatter : IFormatter, INotifyPropertyChanged, IColorFormatter, ICloneable, IXmlSerializable
    {
        /// <summary>
        /// customer culture for ssl
        /// </summary>
        CultureInfo customerCulture;
        /// <summary>
        /// date time format info.
        /// </summary>
        System.Globalization.DateTimeFormatInfo dateTimeFormatInfo;
        static GeneralFormatter defaultComboNumberFormatter1;
        static GeneralFormatter defaultComboNumberFormatter2;
        static GeneralFormatter defaultDMMMFormatter;
        static GeneralFormatter defaultGeneralFormatter;
        static GeneralFormatter defaultHMMFormatter;
        static GeneralFormatter defaultHMMSS0Formatter;
        static GeneralFormatter defaultHMMSSFormatter;
        static GeneralFormatter defaultLongTimePatternFormatter;
        static GeneralFormatter defaultMMMYYFormatter;
        static GeneralFormatter defaultNumberFormatter;
        static GeneralFormatter defaultNumberFormatter2;
        static GeneralFormatter defaultShortDatePatternFormatter;
        static GeneralFormatter defaultShortDatePatternHMMFormatter;
        static GeneralFormatter defaultShortDatePatternHMMSS0Formatter;
        static GeneralFormatter defaultShortDatePatternHMMSSFormatter;
        static GeneralFormatter defaultStandardGroupNumberFormatter1;
        static GeneralFormatter defaultStandardGroupNumberFormatter2;
        static GeneralFormatter defaultStandardNumberFormatter;
        static GeneralFormatter defaultStandardPercentFormatter1;
        static GeneralFormatter defaultStandardPercentFormatter2;
        static GeneralFormatter defaultSXDatetimePatternFormatter;
        /// <summary>
        /// the format cached.
        /// </summary>
        string formatCached;
        /// <summary>
        /// the format mode.
        /// </summary>
        Dt.Cells.Data.FormatMode formatModeType;
        /// <summary>
        /// the list of all formats
        /// </summary>
        Collection<IFormatter> formatters;
        /// <summary>
        /// whether constructed
        /// </summary>
        bool isConstructed;
        /// <summary>
        /// is default
        /// </summary>
        bool isDefault;
        /// <summary>
        /// if format string is "0.00" that without ";" at the end of the format string.
        /// </summary>
        bool isSingleFormatterInfo;
        /// <summary>
        /// number format info.
        /// </summary>
        System.Globalization.NumberFormatInfo numberFormatInfo;

        /// <summary>
        /// Occurs when a formatter property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new default general formatter.
        /// </summary>
        public GeneralFormatter() : this(Dt.Cells.Data.FormatMode.CustomMode, NumberFormatBase.General, CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Creates a new custom formatter with the specified format string.
        /// </summary>
        /// <param name="format">The format string.</param>
        public GeneralFormatter(string format) : this(Dt.Cells.Data.FormatMode.CustomMode, format)
        {
        }

        /// <summary>
        /// Creates a new formatter with the specified format mode and format string.
        /// </summary>
        /// <param name="formatMode">The format mode.</param>
        /// <param name="format">The format string.</param>
        public GeneralFormatter(Dt.Cells.Data.FormatMode formatMode, string format)
        {
            this.isSingleFormatterInfo = true;
            this.isDefault = true;
            if (format == null)
            {
                throw new ArgumentException(ResourceStrings.FormatIllegalFormatError);
            }
            this.formatCached = format;
            this.formatModeType = formatMode;
            this.isDefault = this.formatCached.ToLower() == NumberFormatBase.General.ToLower();
            this.isConstructed = false;
        }

        internal GeneralFormatter(Dt.Cells.Data.FormatMode formatMode, string format, CultureInfo culture) : this(formatMode, format)
        {
            this.customerCulture = culture;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return new GeneralFormatter(this.formatModeType, this.formatCached, this.customerCulture);
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
            return ((obj is GeneralFormatter) && ((object.Equals(this.dateTimeFormatInfo, ((GeneralFormatter) obj).dateTimeFormatInfo) && object.Equals(this.numberFormatInfo, ((GeneralFormatter) obj).numberFormatInfo)) && (this.FormatString == ((GeneralFormatter) obj).FormatString)));
        }

        /// <summary>
        /// Formats the specified object as a string.
        /// </summary>
        /// <param name="obj">The object with cell data to format.</param>
        /// <returns>Returns the formatted string.</returns>
        public virtual string Format(object obj)
        {
            Color? conditionalForeColor = null;
            return this.Format(obj, out conditionalForeColor);
        }

        /// <summary>
        /// Formats the specified object as a string with a conditional color.
        /// </summary>
        /// <param name="obj">The object value to format.</param>
        /// <param name="conditionalForeColor">The conditional foreground color.</param>
        /// <returns>Returns the formatted string.</returns>
        public virtual string Format(object obj, out Color? conditionalForeColor)
        {
            conditionalForeColor = null;
            this.Init();
            IFormatter formatInfo = this.GetFormatInfo(obj);
            if ((formatInfo is CustomNumberFormat) && (((CustomNumberFormat) formatInfo).ColorFormatPart != null))
            {
                conditionalForeColor = new Color?(((CustomNumberFormat) formatInfo).ColorFormatPart.ForeColor);
            }
            double num = 0.0;
            bool flag = FormatConverter.IsNumber(obj);
            if (flag)
            {
                double? nullable = FormatConverter.TryDouble(obj, true);
                if (!nullable.HasValue)
                {
                    if (obj != null)
                    {
                        return obj.ToString();
                    }
                    return string.Empty;
                }
                num = nullable.Value;
            }
            if (formatInfo != null)
            {
                string str = null;
                if (flag && (formatInfo == this.NegativeExpression))
                {
                    str = formatInfo.Format((double) Math.Abs(num));
                    CustomNumberFormat format = formatInfo as CustomNumberFormat;
                    if (((format != null) && (format.ConditionFormatPart != null)) && ((format.ConditionFormatPart.Value > 0.0) && (num < 0.0)))
                    {
                        str = DefaultTokens.NegativeSign + str;
                    }
                }
                else
                {
                    try
                    {
                        str = formatInfo.Format(obj);
                    }
                    catch (InvalidCastException)
                    {
                        if (obj is string)
                        {
                            str = obj.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        if (obj is string)
                        {
                            str = obj.ToString();
                        }
                    }
                }
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            if (flag && (num < 0.0))
            {
                return ((char) DefaultTokens.HyphenMinus).ToString();
            }
            if (obj is string)
            {
                return obj.ToString();
            }
            if (obj != null)
            {
                return obj.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the format information.
        /// </summary>
        /// <param name="obj">Object to get the formatter</param>
        /// <returns>Returns the format information.</returns>
        IFormatter GetFormatInfo(object obj)
        {
            if (this.FormatMode == Dt.Cells.Data.FormatMode.CustomMode)
            {
                if (obj is string)
                {
                    if (this.TextExpression != null)
                    {
                        return this.TextExpression;
                    }
                    return this.PositiveExpression;
                }
                if (FormatConverter.IsNumber(obj) || (obj is bool))
                {
                    CustomNumberFormat positiveExpression = this.PositiveExpression;
                    CustomNumberFormat negativeExpression = this.NegativeExpression;
                    double? nullable = FormatConverter.TryDouble(obj, true);
                    if (!nullable.HasValue)
                    {
                        return null;
                    }
                    double num = nullable.Value;
                    bool flag = (positiveExpression != null) && (positiveExpression.ConditionFormatPart != null);
                    bool flag2 = (negativeExpression != null) && (negativeExpression.ConditionFormatPart != null);
                    IFormatter zeroExpression = this.isSingleFormatterInfo ? this.PositiveExpression : null;
                    if (this.PositiveExpression != null)
                    {
                        if (flag)
                        {
                            if (positiveExpression.ConditionFormatPart.IsMeetCondition(num))
                            {
                                zeroExpression = positiveExpression;
                            }
                        }
                        else if ((num > 0.0) || ((num == 0.0) && (this.ZeroExpression == null)))
                        {
                            zeroExpression = positiveExpression;
                        }
                    }
                    if ((zeroExpression == null) && (this.NegativeExpression != null))
                    {
                        if (flag2)
                        {
                            if (negativeExpression.ConditionFormatPart.IsMeetCondition(num))
                            {
                                zeroExpression = negativeExpression;
                            }
                        }
                        else if (num < 0.0)
                        {
                            zeroExpression = negativeExpression;
                        }
                    }
                    if (((zeroExpression == null) && (this.ZeroExpression != null)) && (num == 0.0))
                    {
                        zeroExpression = this.ZeroExpression;
                    }
                    if ((zeroExpression == null) && (this.ZeroExpression != null))
                    {
                        zeroExpression = this.ZeroExpression;
                    }
                    if ((zeroExpression == null) && (this.NegativeExpression != null))
                    {
                        zeroExpression = this.NegativeExpression;
                    }
                    return zeroExpression;
                }
            }
            else if (((this.FormatMode == Dt.Cells.Data.FormatMode.StandardDateTimeMode) || (this.FormatMode == Dt.Cells.Data.FormatMode.StandardNumericMode)) && ((this.formatters != null) && (this.formatters.Count == 1)))
            {
                return this.formatters[0];
            }
            return null;
        }

        /// <summary>
        /// Gets the preferred formatter type for a specified object.
        /// </summary>
        /// <param name="obj">The object value to format.</param>
        /// <returns>
        /// Returns a <see cref="T:Dt.Cells.Data.NumberFormatType" /> enumeration that specifies the format type.
        /// </returns>
        public NumberFormatType GetFormatType(object obj)
        {
            this.Init();
            IFormatter formatInfo = this.GetFormatInfo(obj);
            if (formatInfo is CustomNumberFormat)
            {
                if (((CustomNumberFormat) formatInfo).Formatter is NumberFormatDigital)
                {
                    return NumberFormatType.Number;
                }
                if (((CustomNumberFormat) formatInfo).Formatter is NumberFormatDateTime)
                {
                    return NumberFormatType.DateTime;
                }
                if (((CustomNumberFormat) formatInfo).Formatter is NumberFormatText)
                {
                    return NumberFormatType.Text;
                }
            }
            else
            {
                if (formatInfo is NumberFormatDigital)
                {
                    return NumberFormatType.Number;
                }
                if (formatInfo is NumberFormatDateTime)
                {
                    return NumberFormatType.DateTime;
                }
                if (formatInfo is NumberFormatText)
                {
                    return NumberFormatType.Text;
                }
            }
            return NumberFormatType.General;
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
        /// Gets the preferred display format string.
        /// </summary>
        /// <param name="s">The formatted data string.</param>
        /// <returns>
        /// Returns the preferred formatter for the string.
        /// </returns>
        public IFormatter GetPreferredDisplayFormatter(string s)
        {
            object obj2;
            return this.GetPreferredDisplayFormatter(s, out obj2);
        }

        /// <summary>
        /// Gets the preferred display format string.
        /// </summary>
        /// <param name="s">The formatted data string.</param>
        /// <param name="value">The parsed value.</param>
        /// <returns>
        /// Returns the preferred formatter for the string.
        /// </returns>
        public IFormatter GetPreferredDisplayFormatter(string s, out object value)
        {
            value = null;
            this.Init();
            if (string.IsNullOrEmpty(s))
            {
                return new GeneralFormatter();
            }
            string str = s;
            value = this.Parse(str);
            if ((value is DateTime) || (value is TimeSpan))
            {
                DateTime time;
                foreach (string str2 in NumberFormatGeneral.GeneralMonthDay)
                {
                    if (DateTime.TryParseExact(s, str2, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        return DefaultDMMMFormatter;
                    }
                }
                foreach (string str3 in NumberFormatGeneral.GeneralYearMonth)
                {
                    if (DateTime.TryParseExact(s, str3, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        return DefaultMMMYYFormatter;
                    }
                }
                foreach (string str4 in NumberFormatGeneral.GeneralYearMonthDay)
                {
                    if (DateTime.TryParseExact(s, str4, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        return DefaultShortDatePatternFormatter;
                    }
                }
                foreach (string str5 in NumberFormatGeneral.AlternativeYearMonthDay)
                {
                    if (DateTime.TryParseExact(s, str5, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        return DefaultShortDatePatternFormatter;
                    }
                }
                foreach (string str6 in NumberFormatGeneral.GeneralHourMinute)
                {
                    if (DateTime.TryParseExact(s, str6, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        return DefaultHMMFormatter;
                    }
                }
                foreach (string str7 in NumberFormatGeneral.GeneralHourMinuteSecond)
                {
                    if (DateTime.TryParseExact(s, str7, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        return DefaultHMMSSFormatter;
                    }
                }
                foreach (string str8 in NumberFormatGeneral.GeneralHourMinuteSecondSubSecond)
                {
                    if (DateTime.TryParseExact(s, str8, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        return DefaultHMMSS0Formatter;
                    }
                }
                foreach (string str9 in NumberFormatGeneral.GeneralHourMinuteWithDate)
                {
                    if (DateTime.TryParseExact(s, str9, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        if (defaultShortDatePatternHMMFormatter == null)
                        {
                            defaultShortDatePatternHMMFormatter = new GeneralFormatter(this.DateTimeFormatInfo.ShortDatePattern + " h:mm");
                        }
                        return defaultShortDatePatternHMMFormatter;
                    }
                }
                foreach (string str10 in NumberFormatGeneral.GeneralHourMinuteSecondWithDate)
                {
                    if (DateTime.TryParseExact(s, str10, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        if (defaultShortDatePatternHMMSSFormatter == null)
                        {
                            defaultShortDatePatternHMMSSFormatter = new GeneralFormatter(this.DateTimeFormatInfo.ShortDatePattern + " h:mm:ss");
                        }
                        return defaultShortDatePatternHMMSSFormatter;
                    }
                }
                foreach (string str11 in NumberFormatGeneral.GeneralHourMinuteSecondSubSecondWithDate)
                {
                    if (DateTime.TryParseExact(s, str11, null, (DateTimeStyles)DateTimeStyles.AllowWhiteSpaces, out time))
                    {
                        if (defaultShortDatePatternHMMSS0Formatter == null)
                        {
                            defaultShortDatePatternHMMSS0Formatter = new GeneralFormatter(this.DateTimeFormatInfo.ShortDatePattern + " h:mm:ss.0");
                        }
                        return defaultShortDatePatternHMMSS0Formatter;
                    }
                }
            }
            else if (FormatConverter.IsNumber(value))
            {
                double? nullable = FormatConverter.TryDouble(value, true);
                if (!nullable.HasValue)
                {
                    return DefaultStandardNumberFormatter;
                }
                double num = nullable.Value;
                if (str[0] == DefaultTokens.NumberFormatInfo.CurrencySymbol[0])
                {
                    if (str.Contains(DefaultTokens.DecimalSeparator))
                    {
                        return DefaultComboNumberFormatter1;
                    }
                    return DefaultComboNumberFormatter2;
                }
                if ((str.IndexOf("e", (StringComparison) StringComparison.CurrentCultureIgnoreCase) > -1) || ((Math.Abs(num) <= NumberFormatDigital.MinDisplayNumber) && (num != 0.0)))
                {
                    return DefaultStandardNumberFormatter;
                }
                char ch = str[0];
                if (!(((char) ch).ToString() == DefaultTokens.PercentSymbol))
                {
                    char ch2 = str[str.Length - 1];
                    if (!(((char) ch2).ToString() == DefaultTokens.PercentSymbol))
                    {
                        if (str.Contains(DefaultTokens.NumberGroupSeparator))
                        {
                            if (str.Contains(DefaultTokens.DecimalSeparator))
                            {
                                return DefaultStandardGroupNumberFormatter1;
                            }
                            return DefaultStandardGroupNumberFormatter2;
                        }
                        goto Label_0406;
                    }
                }
                if (str.Contains(DefaultTokens.DecimalSeparator))
                {
                    return DefaultStandardPercentFormatter1;
                }
                return DefaultStandardPercentFormatter2;
            }
        Label_0406:
            return DefaultGeneralFormatter;
        }

        /// <summary>
        /// Gets the preferred editing formatter.
        /// </summary>
        /// <param name="obj">The object value to format.</param>
        /// <returns>
        /// Returns the preferred editing formatter for the object.
        /// </returns>
        public IFormatter GetPreferredEditingFormatter(object obj)
        {
            if (obj is DateTime)
            {
                DateTime time = (DateTime) obj;
                if (((time.Hour == 0) && (time.Minute == 0)) && ((time.Second == 0) && (time.Millisecond == 0)))
                {
                    return DefaultShortDatePatternFormatter;
                }
                return DefaultSXDatetimePatternFormatter;
            }
            if (obj is TimeSpan)
            {
                return DefaultLongTimePatternFormatter;
            }
            if (FormatConverter.IsNumber(obj))
            {
                double? nullable = FormatConverter.TryDouble(obj, true);
                if (!nullable.HasValue)
                {
                    return null;
                }
                double num2 = Math.Abs(nullable.Value);
                if ((num2 <= NumberFormatDigital.MaxEdittingNumber) && ((num2 >= NumberFormatDigital.MinEdittingNumber) || (num2 == 0.0)))
                {
                    return DefaultNumberFormatter;
                }
                return DefaultNumberFormatter2;
            }
            if (obj is string)
            {
                return DefaultGeneralFormatter;
            }
            return DefaultGeneralFormatter;
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        void Init()
        {
            if (!this.isConstructed)
            {
                this.isConstructed = true;
                switch (this.formatModeType)
                {
                    case Dt.Cells.Data.FormatMode.CustomMode:
                        this.InitExcelCompatibleMode(this.formatCached);
                        return;

                    case Dt.Cells.Data.FormatMode.StandardDateTimeMode:
                        this.InitStandardDateTimeMode(this.formatCached);
                        return;

                    case Dt.Cells.Data.FormatMode.StandardNumericMode:
                        this.InitStandardNumericMode(this.formatCached);
                        break;

                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Initializes the Excel-compatible mode.
        /// </summary>
        /// <param name="format">Format string</param>
        void InitExcelCompatibleMode(string format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            this.formatters = new Collection<IFormatter>();
            if (this.isDefault)
            {
                this.formatters.Add(new CustomNumberFormat(CultureInfo.CurrentCulture));
            }
            else
            {
                this.isSingleFormatterInfo = !format.Contains(((char) DefaultTokens.FormatSeparator).ToString());
                string[] strArray = format.Split(new char[] { DefaultTokens.FormatSeparator });
                if (strArray == null)
                {
                    throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                }
                if ((strArray.Length < 1) || (strArray.Length > 5))
                {
                    throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                }
                int num = 0;
                foreach (string str in strArray)
                {
                    num++;
                    if (num > 4)
                    {
                        break;
                    }
                    CustomNumberFormat format2 = new CustomNumberFormat(str, this.CustomerCulture);
                    if (format2 != null)
                    {
                        this.formatters.Add(format2);
                    }
                }
                if (this.PositiveExpression == null)
                {
                    throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                }
            }
        }

        /// <summary>
        /// Initializes the standard date time mode.
        /// </summary>
        /// <param name="format">Format string</param>
        void InitStandardDateTimeMode(string format)
        {
            if (!StandardDateTimeFormatter.EvaluateFormat(format))
            {
                throw new FormatException(ResourceStrings.FormatIllegalFormatError);
            }
            IFormatter formatter = new StandardDateTimeFormatter(format);
            this.formatters = new Collection<IFormatter>();
            this.formatters.Add(formatter);
        }

        /// <summary>
        /// Initializes the standard numeric mode.
        /// </summary>
        /// <param name="format">Format string</param>
        void InitStandardNumericMode(string format)
        {
            if (!StandardNumberFormatter.EvaluateFormat(format))
            {
                throw new FormatException(ResourceStrings.FormatIllegalFormatError);
            }
            IFormatter formatter = new StandardNumberFormatter(format);
            this.formatters = new Collection<IFormatter>();
            this.formatters.Add(formatter);
        }

        /// <summary>
        /// Parses a specified string to an object.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Returns the parsed object.</returns>
        public virtual object Parse(string str)
        {
            this.Init();
            if ((this.formatters != null) && (this.formatters.Count > 0))
            {
                return this.formatters[0].Parse(str);
            }
            return null;
        }

        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            this.isSingleFormatterInfo = true;
            this.isDefault = true;
            this.formatCached = null;
            this.formatModeType = Dt.Cells.Data.FormatMode.CustomMode;
            this.formatters = null;
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "FormatString")
                    {
                        if (str == "Type")
                        {
                            goto Label_00B5;
                        }
                        if (str == "DateTimeFormatInfo")
                        {
                            goto Label_00C9;
                        }
                        if (str == "NumberFormatInfo")
                        {
                            goto Label_00D7;
                        }
                    }
                    else
                    {
                        this.formatCached = Serializer.ReadAttribute("value", reader);
                    }
                }
                continue;
            Label_00B5:
                this.formatModeType = Serializer.ReadAttributeEnum<Dt.Cells.Data.FormatMode>("value", Dt.Cells.Data.FormatMode.CustomMode, reader);
                continue;
            Label_00C9:
                this.dateTimeFormatInfo = Serializer.DeserializeDateTimeFormatInfo(reader);
                continue;
            Label_00D7:
                this.numberFormatInfo = Serializer.DeserializeNumberFormatInfo(reader);
            }
            this.isDefault = this.formatCached.ToLower() == NumberFormatBase.General.ToLower();
            this.isConstructed = false;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
        }

        /// <summary>
        /// Gets or sets the customer culture.
        /// </summary>
        public CultureInfo CustomerCulture
        {
            get { return  this.customerCulture; }
            set { this.customerCulture = value; }
        }

        /// <summary>
        /// Gets the date time format information for this formatter.
        /// </summary>
        /// <value>
        /// The date time format information for this formatter. 
        /// The default value equals the System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.
        /// </value>
        internal System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
        {
            get
            {
                this.Init();
                if (this.dateTimeFormatInfo != null)
                {
                    return this.dateTimeFormatInfo;
                }
                return DefaultTokens.DateTimeFormatInfo;
            }
            set
            {
                this.Init();
                this.dateTimeFormatInfo = value;
                if (this.formatters != null)
                {
                    foreach (IFormatter formatter in this.formatters)
                    {
                        if (formatter is IFormatProviderSupport)
                        {
                            ((IFormatProviderSupport) formatter).DateTimeFormatInfo = value;
                        }
                    }
                }
                this.RaisePropertyChanged("DateTimeFormatInfo");
            }
        }

        /// <summary>
        /// Gets the "{0}#,##0.00;[Red]({0}#,##0.00)" number formatter.
        /// </summary>
        /// <value>The "{0}#,##0.00;[Red]({0}#,##0.00)" number formatter.</value>
        internal static GeneralFormatter DefaultComboNumberFormatter1
        {
            get
            {
                if (defaultComboNumberFormatter1 == null)
                {
                    defaultComboNumberFormatter1 = new GeneralFormatter(string.Format("{0}#,##0.00;[Red]({0}#,##0.00)", (object[]) new object[] { DefaultTokens.NumberFormatInfo.CurrencySymbol }));
                }
                return defaultComboNumberFormatter1;
            }
        }

        /// <summary>
        /// Gets the "{0}#,##0;[Red]({0}#,##0)" number formatter.
        /// </summary>
        /// <value>The "{0}#,##0;[Red]({0}#,##0)" number formatter.</value>
        internal static GeneralFormatter DefaultComboNumberFormatter2
        {
            get
            {
                if (defaultComboNumberFormatter2 == null)
                {
                    defaultComboNumberFormatter2 = new GeneralFormatter(string.Format("{0}#,##0;[Red]({0}#,##0)", (object[]) new object[] { DefaultTokens.NumberFormatInfo.CurrencySymbol }));
                }
                return defaultComboNumberFormatter2;
            }
        }

        /// <summary>
        /// Gets the "d-mmm" date formatter.
        /// </summary>
        /// <value>The "d-mmm" date formatter.</value>
        internal static GeneralFormatter DefaultDMMMFormatter
        {
            get
            {
                if (defaultDMMMFormatter == null)
                {
                    defaultDMMMFormatter = new GeneralFormatter("d-mmm");
                }
                return defaultDMMMFormatter;
            }
        }

        /// <summary>
        /// Gets the default general formatter.
        /// </summary>
        /// <value>The default general formatter.</value>
        internal static GeneralFormatter DefaultGeneralFormatter
        {
            get
            {
                if (defaultGeneralFormatter == null)
                {
                    defaultGeneralFormatter = new GeneralFormatter();
                }
                return defaultGeneralFormatter;
            }
        }

        /// <summary>
        /// Gets the "h:mm" time formatter.
        /// </summary>
        /// <value>The "h:mm" time formatter.</value>
        internal static GeneralFormatter DefaultHMMFormatter
        {
            get
            {
                if (defaultHMMFormatter == null)
                {
                    defaultHMMFormatter = new GeneralFormatter("h:mm");
                }
                return defaultHMMFormatter;
            }
        }

        /// <summary>
        /// Gets the "h:mm:ss.0" time formatter.
        /// </summary>
        /// <value>The "h:mm:ss.0" time formatter.</value>
        internal static GeneralFormatter DefaultHMMSS0Formatter
        {
            get
            {
                if (defaultHMMSS0Formatter == null)
                {
                    defaultHMMSS0Formatter = new GeneralFormatter("h:mm:ss.0");
                }
                return defaultHMMSS0Formatter;
            }
        }

        /// <summary>
        /// Gets the "h:mm:ss" time formatter.
        /// </summary>
        /// <value>The "h:mm:ss" time formatter.</value>
        internal static GeneralFormatter DefaultHMMSSFormatter
        {
            get
            {
                if (defaultHMMSSFormatter == null)
                {
                    defaultHMMSSFormatter = new GeneralFormatter("h:mm:ss");
                }
                return defaultHMMSSFormatter;
            }
        }

        /// <summary>
        /// Gets the long time pattern formatter.
        /// </summary>
        /// <value>The long time pattern formatter.</value>
        internal static GeneralFormatter DefaultLongTimePatternFormatter
        {
            get
            {
                if (defaultLongTimePatternFormatter == null)
                {
                    defaultLongTimePatternFormatter = new GeneralFormatter(DefaultTokens.DateTimeFormatInfo.LongTimePattern);
                }
                return defaultLongTimePatternFormatter;
            }
        }

        /// <summary>
        /// Gets the "mmm-yy" date formatter.
        /// </summary>
        /// <value>The "mmm-yy" date formatter.</value>
        internal static GeneralFormatter DefaultMMMYYFormatter
        {
            get
            {
                if (defaultMMMYYFormatter == null)
                {
                    defaultMMMYYFormatter = new GeneralFormatter("mmm-yy");
                }
                return defaultMMMYYFormatter;
            }
        }

        /// <summary>
        /// Gets the "###################0.################" number formatter.
        /// </summary>
        /// <value>The "###################0.################" number formatter.</value>
        internal static GeneralFormatter DefaultNumberFormatter
        {
            get
            {
                if (defaultNumberFormatter == null)
                {
                    defaultNumberFormatter = new GeneralFormatter("###################0.################");
                }
                return defaultNumberFormatter;
            }
        }

        /// <summary>
        /// Gets the "0.##E+00" number formatter.
        /// </summary>
        /// <value>The "0.##E+00" number formatter.</value>
        internal static GeneralFormatter DefaultNumberFormatter2
        {
            get
            {
                if (defaultNumberFormatter2 == null)
                {
                    defaultNumberFormatter2 = new GeneralFormatter("0.##E+00");
                }
                return defaultNumberFormatter2;
            }
        }

        /// <summary>
        /// Gets the short date pattern formatter.
        /// </summary>
        /// <value>The short date pattern formatter.</value>
        internal static GeneralFormatter DefaultShortDatePatternFormatter
        {
            get
            {
                if (defaultShortDatePatternFormatter == null)
                {
                    defaultShortDatePatternFormatter = new GeneralFormatter(DefaultTokens.DateTimeFormatInfo.ShortDatePattern);
                }
                return defaultShortDatePatternFormatter;
            }
        }

        /// <summary>
        /// Gets the "#,##0.00" number formatter.
        /// </summary>
        /// <value>The "#,##0.00" number formatter.</value>
        internal static GeneralFormatter DefaultStandardGroupNumberFormatter1
        {
            get
            {
                if (defaultStandardGroupNumberFormatter1 == null)
                {
                    defaultStandardGroupNumberFormatter1 = new GeneralFormatter("#,##0.00");
                }
                return defaultStandardGroupNumberFormatter1;
            }
        }

        /// <summary>
        /// Gets the "#,##0" number formatter.
        /// </summary>
        /// <value>The "#,##0" number formatter.</value>
        internal static GeneralFormatter DefaultStandardGroupNumberFormatter2
        {
            get
            {
                if (defaultStandardGroupNumberFormatter2 == null)
                {
                    defaultStandardGroupNumberFormatter2 = new GeneralFormatter("#,##0");
                }
                return defaultStandardGroupNumberFormatter2;
            }
        }

        /// <summary>
        /// Gets the "0.00E+00" number formatter.
        /// </summary>
        /// <value>The "0.00E+00" number formatter.</value>
        internal static GeneralFormatter DefaultStandardNumberFormatter
        {
            get
            {
                if (defaultStandardNumberFormatter == null)
                {
                    defaultStandardNumberFormatter = new GeneralFormatter("0.00E+00");
                }
                return defaultStandardNumberFormatter;
            }
        }

        /// <summary>
        /// Gets the "0.00%" number formatter.
        /// </summary>
        /// <value>The "0.00%" number formatter.</value>
        internal static GeneralFormatter DefaultStandardPercentFormatter1
        {
            get
            {
                if (defaultStandardPercentFormatter1 == null)
                {
                    defaultStandardPercentFormatter1 = new GeneralFormatter("0.00%");
                }
                return defaultStandardPercentFormatter1;
            }
        }

        /// <summary>
        /// Gets the "0%" number formatter.
        /// </summary>
        /// <value>The "0%" number formatter.</value>
        internal static GeneralFormatter DefaultStandardPercentFormatter2
        {
            get
            {
                if (defaultStandardPercentFormatter2 == null)
                {
                    defaultStandardPercentFormatter2 = new GeneralFormatter("0%");
                }
                return defaultStandardPercentFormatter2;
            }
        }

        /// <summary>
        /// Gets the long time pattern formatter.
        /// </summary>
        /// <value>The long time pattern formatter.</value>
        internal static GeneralFormatter DefaultSXDatetimePatternFormatter
        {
            get
            {
                if (defaultSXDatetimePatternFormatter == null)
                {
                    defaultSXDatetimePatternFormatter = new GeneralFormatter("m/d/yyyy h:mm:ss tt");
                }
                return defaultSXDatetimePatternFormatter;
            }
        }

        /// <summary>
        /// Gets the Excel-compatible format string.
        /// </summary>
        /// <value>The Excel-compatible format string. The default value is an empty string.</value>
        [DefaultValue("")]
        public string ExcelCompatibleFormatString
        {
            get
            {
                this.Init();
                StringBuilder builder = null;
                switch (this.FormatMode)
                {
                    case Dt.Cells.Data.FormatMode.CustomMode:
                        if (this.formatters != null)
                        {
                            foreach (IFormatter formatter in this.formatters)
                            {
                                if (formatter is CustomNumberFormat)
                                {
                                    if (builder == null)
                                    {
                                        builder = new StringBuilder();
                                    }
                                    else
                                    {
                                        builder.Append(DefaultTokens.FormatSeparator);
                                    }
                                    string excelCompatibleFormatString = ((CustomNumberFormat) formatter).ExcelCompatibleFormatString;
                                    builder.Append(excelCompatibleFormatString);
                                }
                            }
                        }
                        break;

                    case Dt.Cells.Data.FormatMode.StandardDateTimeMode:
                        if (this.formatters[0] is StandardDateTimeFormatter)
                        {
                            return ((StandardDateTimeFormatter) this.formatters[0]).ExcelCompatibleFormatString;
                        }
                        break;

                    case Dt.Cells.Data.FormatMode.StandardNumericMode:
                        if (this.formatters[0] is StandardNumberFormatter)
                        {
                            return ((StandardNumberFormatter) this.formatters[0]).ExcelCompatibleFormatString;
                        }
                        break;
                }
                if (builder != null)
                {
                    return builder.ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the format mode for this formatter.
        /// </summary>
        /// <value>
        /// The format mode for this formatter.
        /// The default value is <see cref="P:Dt.Cells.Data.GeneralFormatter.FormatMode">CustomMode</see>.
        /// </value>
        [DefaultValue(0)]
        public Dt.Cells.Data.FormatMode FormatMode
        {
            get { return  this.formatModeType; }
            internal set
            {
                this.formatModeType = value;
                this.RaisePropertyChanged("FormatMode");
            }
        }

        /// <summary>
        /// Gets the format string for this formatter.
        /// </summary>
        /// <value>The format string for this formatter. The default value is "General".</value>
        [DefaultValue("General")]
        public string FormatString
        {
            get
            {
                this.Init();
                StringBuilder builder = null;
                switch (this.FormatMode)
                {
                    case Dt.Cells.Data.FormatMode.CustomMode:
                        if (this.formatters != null)
                        {
                            int num = this.formatters.Count;
                            foreach (CustomNumberFormat format in this.formatters)
                            {
                                if (format != null)
                                {
                                    if (builder == null)
                                    {
                                        builder = new StringBuilder();
                                    }
                                    else
                                    {
                                        builder.Append(DefaultTokens.FormatSeparator);
                                    }
                                    if ((num == 1) || !string.IsNullOrEmpty(format.OriginalFormat))
                                    {
                                        builder.Append(format.FormatString);
                                    }
                                }
                            }
                        }
                        break;

                    case Dt.Cells.Data.FormatMode.StandardDateTimeMode:
                        if (this.formatters[0] is StandardDateTimeFormatter)
                        {
                            return ((StandardDateTimeFormatter) this.formatters[0]).FormatString;
                        }
                        break;

                    case Dt.Cells.Data.FormatMode.StandardNumericMode:
                        if (this.formatters[0] is StandardNumberFormatter)
                        {
                            return ((StandardNumberFormatter) this.formatters[0]).FormatString;
                        }
                        break;
                }
                if (builder != null)
                {
                    return builder.ToString();
                }
                return string.Empty;
            }
            internal set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.formatters = null;
                this.formatCached = value;
                this.isDefault = this.formatCached.ToLower() == NumberFormatBase.General.ToLower();
                this.isConstructed = false;
                this.Init();
                this.RaisePropertyChanged("FormatString");
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this formatted text contains a foreground color.
        /// </summary>
        /// <value>
        /// <c>true</c> if this formatted text contains a foreground color; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool HasFormatedColor
        {
            get
            {
                if (this.isDefault)
                {
                    return false;
                }
                return (((this.PositiveExpression != null) && (this.PositiveExpression.ColorFormatPart != null)) || (((this.NegativeExpression != null) && (this.NegativeExpression.ColorFormatPart != null)) || (((this.ZeroExpression != null) && (this.ZeroExpression.ColorFormatPart != null)) || ((this.TextExpression != null) && (this.TextExpression.ColorFormatPart != null)))));
            }
        }

        /// <summary>
        /// Gets whether this formatter is the default formatter.
        /// </summary>
        /// <value>
        /// <c>true</c> if this formatter is the default formatter; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool IsDefaultFormat
        {
            get { return  this.isDefault; }
        }

        /// <summary>
        /// Gets the negative formatter expression.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.Cells.Data.CustomNumberFormat" /> object that specifies the formatter when a value is negative.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        internal CustomNumberFormat NegativeExpression
        {
            get
            {
                this.Init();
                if ((this.formatters != null) && (this.formatters.Count > 1))
                {
                    return (this.formatters[1] as CustomNumberFormat);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the number format information for this formatter.
        /// </summary>
        /// <value>
        /// The number format information for this formatter. 
        /// The default value equals the System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormatInfo.
        /// </value>
        internal System.Globalization.NumberFormatInfo NumberFormatInfo
        {
            get
            {
                this.Init();
                if (this.numberFormatInfo != null)
                {
                    return this.numberFormatInfo;
                }
                return DefaultTokens.NumberFormatInfo;
            }
            set
            {
                this.Init();
                this.numberFormatInfo = value;
                if (this.formatters != null)
                {
                    foreach (IFormatter formatter in this.formatters)
                    {
                        if (formatter is IFormatProviderSupport)
                        {
                            ((IFormatProviderSupport) formatter).NumberFormatInfo = value;
                        }
                    }
                }
                this.RaisePropertyChanged("NumberFormatInfo");
            }
        }

        /// <summary>
        /// Gets the positive formatter expression.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.Cells.Data.CustomNumberFormat" /> object that specifies the formatter when a value is positive.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        internal CustomNumberFormat PositiveExpression
        {
            get
            {
                this.Init();
                if ((this.formatters != null) && (this.formatters.Count > 0))
                {
                    return (this.formatters[0] as CustomNumberFormat);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the text expression.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.Cells.Data.CustomNumberFormat" /> object that specifies the formatter when a value is text.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        internal CustomNumberFormat TextExpression
        {
            get
            {
                this.Init();
                if ((this.formatters != null) && (this.formatters.Count > 3))
                {
                    return (this.formatters[3] as CustomNumberFormat);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the zero expression.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.Cells.Data.CustomNumberFormat" /> object that specifies the formatter when a value is zero.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        internal CustomNumberFormat ZeroExpression
        {
            get
            {
                this.Init();
                if ((this.formatters != null) && (this.formatters.Count > 2))
                {
                    return (this.formatters[2] as CustomNumberFormat);
                }
                return null;
            }
        }
    }
}

