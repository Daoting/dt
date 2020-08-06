#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a number format.
    /// </summary>
    internal abstract class NumberFormatBase : IFormatter, IFormatProviderSupport
    {
        /// <summary>
        /// the culture for date time and number format.
        /// </summary>
        CultureInfo culture;
        /// <summary>
        /// the date time format info.
        /// </summary>
        System.Globalization.DateTimeFormatInfo dateTimeFormatInfo;
        /// <summary>
        /// the general format.
        /// </summary>
        internal static readonly string General = "General";
        /// <summary>
        /// the number format info.
        /// </summary>
        System.Globalization.NumberFormatInfo numberFormatInfo;
        /// <summary>
        /// the number string Converter.
        /// </summary>
        INumberStringConverter numberStringConverter;
        /// <summary>
        /// DBNum part.
        /// </summary>
        object partDbNumberFormat;
        /// <summary>
        /// the part locale id.
        /// </summary>
        object partLocaleID;

        /// <summary>
        /// Creates a new number format with the specified locale and db number formats.
        /// </summary>
        /// <param name="partLocaleID">The locale ID.</param>
        /// <param name="dbNumberFormatPart">The data number format.</param>
        /// <param name="culture">The culture.</param>
        protected NumberFormatBase(object partLocaleID, object dbNumberFormatPart, CultureInfo culture)
        {
            this.partLocaleID = partLocaleID;
            this.partDbNumberFormat = dbNumberFormatPart;
            this.culture = culture;
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
                    else if ((!flag && (ch2 != DefaultTokens.UnderLine)) && (ch != DefaultTokens.UnderLine))
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
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        /// <returns> <c>true</c> if reading the XML from the specified reader was successful; otherwise, <c>false</c>.</returns>
        protected bool DoReadXml(XmlReader reader)
        {
            return true;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        protected void DoWriteXml(XmlWriter writer)
        {
        }

        /// <summary>
        /// Formats the specified object as a string.
        /// </summary>
        /// <param name="obj">The object with cell data to format.</param>
        /// <returns>Returns the formatted string.</returns>
        public abstract string Format(object obj);
        /// <summary>
        /// Determines whether the specified string is transform.
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="currentpos">The current position</param>
        /// <returns>
        /// <c>true</c> if the specified string is transform; otherwise, <c>false</c>.
        /// </returns>
        static bool IsTransform(string str, int currentpos)
        {
            if (str[currentpos] == '\\')
            {
                throw new ArgumentException(ResourceStrings.FormatterTransformEscapeSymbolError);
            }
            if ((((currentpos - 1) > 0) && ((currentpos - 1) < str.Length)) && (str[currentpos - 1] == '\\'))
            {
                if ((currentpos - 2) < 0)
                {
                    return true;
                }
                if (((currentpos - 2) > 0) && ((currentpos - 2) < str.Length))
                {
                    char ch = str[currentpos - 2];
                    return (((ch != '\\') && (ch != '*')) && (ch != '_'));
                }
            }
            return false;
        }

        /// <summary>
        /// Parses the specified string into an object.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Returns the parsed object.</returns>
        public abstract object Parse(string str);
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
        /// Gets  the customer culture.
        /// </summary>
        public virtual CultureInfo Culture
        {
            get { return  this.culture; }
            internal set { this.culture = value; }
        }

        /// <summary>
        /// Gets the date-time format information for this formatter.
        /// </summary>
        /// <value>
        /// The date-time format information for this formatter. 
        /// The default value is the System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat object.
        /// </value>
        public virtual System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
        {
            get { return  this.dateTimeFormatInfo; }
            set { this.dateTimeFormatInfo = value; }
        }

        /// <summary>
        /// Gets the decimal separator.
        /// </summary>
        /// <value>The decimal separator. The default value is a period (.).</value>
        [DefaultValue(".")]
        protected string DecimalSeparator
        {
            get
            {
                if (this.NumberFormatInfo != null)
                {
                    return this.NumberFormatInfo.NumberDecimalSeparator;
                }
                return DefaultTokens.DecimalSeparator;
            }
        }

        /// <summary>
        /// Gets the Excel-compatible format string.
        /// </summary>
        /// <value>The Excel-compatible format string.</value>
        internal abstract string ExcelCompatibleFormatString { get; }

        /// <summary>
        /// Gets the format string for the formatter.
        /// </summary>
        /// <value>The format string of the formatter.</value>
        public abstract string FormatString { get; }

        /// <summary>
        /// Gets the NaN symbol.
        /// </summary>
        /// <value>The NaN symbol. The default value is "NaN".</value>
        [DefaultValue("NaN")]
        protected string NaNSymbol
        {
            get
            {
                if (this.NumberFormatInfo != null)
                {
                    return this.NumberFormatInfo.NaNSymbol;
                }
                return DefaultTokens.NaNSymbol;
            }
        }

        /// <summary>
        /// Gets the negative sign.
        /// </summary>
        /// <value>The negative sign. The default value is a minus sign (-).</value>
        [DefaultValue("-")]
        protected string NegativeSign
        {
            get
            {
                if (this.NumberFormatInfo != null)
                {
                    return this.NumberFormatInfo.NegativeSign;
                }
                return DefaultTokens.NegativeSign;
            }
        }

        /// <summary>
        /// Gets or sets the number format information for this formatter.
        /// </summary>
        /// <value>
        /// The number format information for this formatter. 
        /// The default value is the System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormatInfo object.
        /// </value>
        public virtual System.Globalization.NumberFormatInfo NumberFormatInfo
        {
            get { return  this.numberFormatInfo; }
            set { this.numberFormatInfo = value; }
        }

        /// <summary>
        /// Gets the group number separator.
        /// </summary>
        /// <value>The group number separator. The default value is a semi-colon (;).</value>
        [DefaultValue(";")]
        protected string NumberGroupSeparator
        {
            get
            {
                if (this.NumberFormatInfo != null)
                {
                    return this.NumberFormatInfo.NumberGroupSeparator;
                }
                return DefaultTokens.NumberGroupSeparator;
            }
        }

        /// <summary>
        /// Gets or sets the number string converter.
        /// </summary>
        /// <value>The number string converter. The default value is null.</value>
        [DefaultValue((string) null)]
        public virtual INumberStringConverter NumberStringConverter
        {
            get
            {
                if (this.numberStringConverter != null)
                {
                    return this.numberStringConverter;
                }
                return null;
            }
            set { this.numberStringConverter = value; }
        }

        /// <summary>
        /// Gets the DB number format for the formatter.
        /// </summary>
        /// <value>The DB number format of the formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public DBNumberFormatPart PartDBNumberFormat
        {
            get { return  (this.partDbNumberFormat as DBNumberFormatPart); }
        }

        /// <summary>
        /// Gets the locale format for the formatter.
        /// </summary>
        /// <value>The locale format of the formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public LocaleIDFormatPart PartLocaleID
        {
            get { return  (this.partLocaleID as LocaleIDFormatPart); }
        }

        /// <summary>
        /// Gets the percent symbol.
        /// </summary>
        /// <value>The percent symbol. The default value is "%".</value>
        [DefaultValue("%")]
        protected string PercentSymbol
        {
            get
            {
                if (this.NumberFormatInfo != null)
                {
                    return this.NumberFormatInfo.PercentSymbol;
                }
                return DefaultTokens.PercentSymbol;
            }
        }

        /// <summary>
        /// Gets the positive sign.
        /// </summary>
        /// <value>The positive sign. The default value is a plus sign (+).</value>
        [DefaultValue("+")]
        protected string PositiveSign
        {
            get
            {
                if (this.NumberFormatInfo != null)
                {
                    return this.NumberFormatInfo.PositiveSign;
                }
                return DefaultTokens.PositiveSign;
            }
        }
    }
}

