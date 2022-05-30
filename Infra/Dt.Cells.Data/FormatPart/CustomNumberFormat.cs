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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a custom number format
    /// </summary>
    /// <remarks>
    /// The format can contain up to four sections. 
    /// You can specify format codes to describe how you want to display your numbers, dates, times, and text.
    /// </remarks>
    internal sealed class CustomNumberFormat : IFormatter, IFormatProviderSupport, IXmlSerializable
    {
        /// <summary>
        /// the color format part.
        /// </summary>
        Dt.Cells.Data.ColorFormatPart colorFormatPart;
        readonly string colorFormatString;
        /// <summary>
        /// the condition format part.
        /// </summary>
        Dt.Cells.Data.ConditionFormatPart conditionFormatPart;
        readonly string conditionFormatString;
        static readonly string[] CurrencySymbols = new string[] { 
            "$", "Kč", @"kr\.", "Ft", "kr", "zł", "R$", @"fr\.", "RON", @"р\.", "kn", "EUR", "Lek", "Rs", "Rp", "Ls", 
            "Lt", "manat", "R", "RM", "so'm", "ETB", "DZD", "PhP", "XOF", "$b", "ERN", "RWF", "NT$", "Kč", "Ft", "zł", 
            "RON", "kn", "Lek", "Rs", "Rp", "manat", "Q", @"din\.", "HK$", "KM", "MOP", "CHF", "RD$", "J$", @"Bs\.F\.", "EC$", 
            "BZ$", @"S\/\.", "TT$", "$U", @"L\.", "C$", @"B\/\."
         };
        /// <summary>
        /// date time format info
        /// </summary>
        System.Globalization.DateTimeFormatInfo dateTimeFormatInfo;
        /// <summary>
        /// the [DBNum] part format.
        /// </summary>
        Dt.Cells.Data.DBNumberFormatPart dbNumberFormatPart;
        readonly string dbNumberFormatString;
        /// <summary>
        /// the format cache.
        /// </summary>
        string formatCached;
        StringBuilder formatStringTemplate;
        /// <summary>
        /// the local id format part.
        /// </summary>
        Dt.Cells.Data.LocaleIDFormatPart localeIDFormatPart;
        readonly string localIDFormatString;
        /// <summary>
        /// the number format info.
        /// </summary>
        NumberFormatBase numberFormat;
        /// <summary>
        /// number format info
        /// </summary>
        System.Globalization.NumberFormatInfo numberFormatInfo;

        /// <summary>
        /// Creates a new custom number format.
        /// </summary>
        public CustomNumberFormat()
        {
            this.conditionFormatString = "[conditionFormaString]";
            this.colorFormatString = "[colorFormatString]";
            this.localIDFormatString = "[localIDFormatString]";
            this.dbNumberFormatString = "[dbNumberFormatString]";
            this.formatCached = NumberFormatBase.General;
            this.numberFormat = new NumberFormatGeneral();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        public CustomNumberFormat(CultureInfo culture)
        {
            this.conditionFormatString = "[conditionFormaString]";
            this.colorFormatString = "[colorFormatString]";
            this.localIDFormatString = "[localIDFormatString]";
            this.dbNumberFormatString = "[dbNumberFormatString]";
            this.formatCached = NumberFormatBase.General;
            this.numberFormat = new NumberFormatGeneral();
            this.numberFormat.Culture = culture;
        }

        /// <summary>
        /// Creates a new custom number format with the specified format string.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="culture">The culture.</param>
        public CustomNumberFormat(string format, CultureInfo culture)
        {
            this.conditionFormatString = "[conditionFormaString]";
            this.colorFormatString = "[colorFormatString]";
            this.localIDFormatString = "[localIDFormatString]";
            this.dbNumberFormatString = "[dbNumberFormatString]";
            this.Init(format, culture);
        }

        /// <summary>
        /// Adds the specified format to the formatter.
        /// </summary>
        /// <param name="part">The format to add to this formatter.</param>
        public void AddPart(FormatPartBase part)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            if (part is Dt.Cells.Data.ConditionFormatPart)
            {
                if (this.conditionFormatPart != null)
                {
                    throw new NotSupportedException(ResourceStrings.FormatterCustomNumberFormatAddPartError);
                }
                this.conditionFormatPart = part as Dt.Cells.Data.ConditionFormatPart;
                if (this.formatStringTemplate == null)
                {
                    this.formatStringTemplate = new StringBuilder();
                }
                this.formatStringTemplate.Append(this.conditionFormatString);
            }
            else if (part is Dt.Cells.Data.ColorFormatPart)
            {
                if (this.colorFormatPart != null)
                {
                    throw new NotSupportedException(ResourceStrings.FormatterCustomNumberFormatAddPartError);
                }
                this.colorFormatPart = part as Dt.Cells.Data.ColorFormatPart;
                if (this.formatStringTemplate == null)
                {
                    this.formatStringTemplate = new StringBuilder();
                }
                this.formatStringTemplate.Append(this.colorFormatString);
            }
            else if (part is Dt.Cells.Data.LocaleIDFormatPart)
            {
                if (this.localeIDFormatPart != null)
                {
                    throw new NotSupportedException(ResourceStrings.FormatterCustomNumberFormatAddPartError);
                }
                this.localeIDFormatPart = part as Dt.Cells.Data.LocaleIDFormatPart;
                if (this.formatStringTemplate == null)
                {
                    this.formatStringTemplate = new StringBuilder();
                }
                this.formatStringTemplate.Append(this.localIDFormatString);
            }
            else if (part is Dt.Cells.Data.DBNumberFormatPart)
            {
                if (this.dbNumberFormatPart != null)
                {
                    throw new NotSupportedException(ResourceStrings.FormatterCustomNumberFormatAddPartError);
                }
                this.dbNumberFormatPart = part as Dt.Cells.Data.DBNumberFormatPart;
                if (this.formatStringTemplate == null)
                {
                    this.formatStringTemplate = new StringBuilder();
                }
                this.formatStringTemplate.Append(this.dbNumberFormatString);
            }
        }

        bool ContainsCurrencySymbol(string format)
        {
            foreach (string str in CurrencySymbols)
            {
                if (format.Contains(str))
                {
                    return true;
                }
            }
            return false;
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
            return ((obj is CustomNumberFormat) && ((object.Equals(this.numberFormatInfo, ((CustomNumberFormat) obj).numberFormatInfo) && object.Equals(this.dateTimeFormatInfo, ((CustomNumberFormat) obj).dateTimeFormatInfo)) && (this.formatCached == ((CustomNumberFormat) obj).formatCached)));
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="obj">The object with cell data to format.</param>
        /// <returns>Returns the formatted string.</returns>
        public string Format(object obj)
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
        /// <param name="culture">The culture.</param>
        void Init(string format, CultureInfo culture)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            this.conditionFormatPart = null;
            this.colorFormatPart = null;
            this.localeIDFormatPart = null;
            this.dbNumberFormatPart = null;
            this.formatCached = format;
            StringBuilder builder = null;
            StringBuilder builder2 = null;
            bool flag = false;
            bool flag2 = false;
            List<ABSTimeFormatPart> list = new List<ABSTimeFormatPart>();
            for (int i = 0; i < format.Length; i++)
            {
                char ch = format[i];
                if ((ch == DefaultTokens.LeftSquareBracket) && !flag2)
                {
                    if (flag)
                    {
                        throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                    }
                    if (builder2 != null)
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder();
                        }
                        builder.Append(builder2.ToString());
                        if (this.formatStringTemplate == null)
                        {
                            this.formatStringTemplate = new StringBuilder();
                        }
                        this.formatStringTemplate.Append(builder2.ToString());
                        builder2 = null;
                    }
                    builder2 = new StringBuilder();
                    builder2.Append(ch);
                    flag = true;
                }
                else if ((ch == DefaultTokens.RightSquareBracket) && !flag2)
                {
                    if (!flag)
                    {
                        throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                    }
                    if (builder2 == null)
                    {
                        throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                    }
                    if (builder2 == null)
                    {
                        builder2 = new StringBuilder();
                    }
                    builder2.Append(ch);
                    FormatPartBase part = FormatPartBase.Create(builder2.ToString());
                    if ((part != null) && !(part is ABSTimeFormatPart))
                    {
                        this.AddPart(part);
                    }
                    else
                    {
                        if (!(part is ABSTimeFormatPart))
                        {
                            throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                        }
                        list.Add(part as ABSTimeFormatPart);
                        if (builder == null)
                        {
                            builder = new StringBuilder();
                        }
                        builder.Append(builder2.ToString());
                        if (this.formatStringTemplate == null)
                        {
                            this.formatStringTemplate = new StringBuilder();
                        }
                        this.formatStringTemplate.Append(builder2.ToString());
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
                if (ch == DefaultTokens.ReverseSolidusSign)
                {
                    flag2 = !flag2;
                }
                else
                {
                    flag2 = false;
                }
            }
            if (builder2 != null)
            {
                if (flag)
                {
                    throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                }
                if (builder == null)
                {
                    builder = new StringBuilder();
                }
                builder.Append(builder2.ToString());
                if (this.formatStringTemplate == null)
                {
                    this.formatStringTemplate = new StringBuilder();
                }
                this.formatStringTemplate.Append(builder2.ToString());
            }
            string str2 = (builder != null) ? builder.ToString() : string.Empty;
            if (NumberFormatGeneral.EvaluateFormat(str2))
            {
                this.numberFormat = new NumberFormatGeneral(str2, this.LocaleIDFormatPart, this.dbNumberFormatPart, culture);
            }
            else if (NumberFormatDateTime.EvaluateFormat(str2) && !this.ContainsCurrencySymbol(str2))
            {
                ABSTimeFormatPart[] absTimeParts = (list.Count > 0) ? list.ToArray() : null;
                this.numberFormat = new NumberFormatDateTime(str2, absTimeParts, this.LocaleIDFormatPart, this.dbNumberFormatPart, culture);
            }
            else if (NumberFormatDigital.EvaluateFormat(str2))
            {
                this.numberFormat = new NumberFormatDigital(format, this.LocaleIDFormatPart, this.dbNumberFormatPart, culture);
            }
            else
            {
                if (!NumberFormatText.EvaluateFormat(str2))
                {
                    throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                }
                this.numberFormat = new NumberFormatText(format, this.LocaleIDFormatPart, this.dbNumberFormatPart, culture);
            }
        }

        /// <summary>
        /// Parses the specified string using the format.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Returns the parsed object.</returns>
        public object Parse(string str)
        {
            return this.numberFormat.Parse(str);
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
            this.conditionFormatPart = null;
            this.colorFormatPart = null;
            this.localeIDFormatPart = null;
            this.dbNumberFormatPart = null;
            this.formatCached = null;
            System.Globalization.DateTimeFormatInfo info = null;
            System.Globalization.NumberFormatInfo info2 = null;
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "FormatString")
                    {
                        if (str == "DateTimeFormatInfo")
                        {
                            goto Label_00A6;
                        }
                        if (str == "NumberFormatInfo")
                        {
                            goto Label_00AF;
                        }
                    }
                    else
                    {
                        this.formatCached = Serializer.ReadAttribute("value", reader);
                    }
                }
                continue;
            Label_00A6:
                info = Serializer.DeserializeDateTimeFormatInfo(reader);
                continue;
            Label_00AF:
                info2 = Serializer.DeserializeNumberFormatInfo(reader);
            }
            this.Init(this.formatCached, null);
            this.DateTimeFormatInfo = info;
            this.NumberFormatInfo = info2;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
        }

        /// <summary>
        /// Gets the color format for this formatter.
        /// </summary>
        /// <value>The color format for this formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public Dt.Cells.Data.ColorFormatPart ColorFormatPart
        {
            get { return  this.colorFormatPart; }
        }

        /// <summary>
        /// Gets the condition format for this formatter.
        /// </summary>
        /// <value>The condition format for this formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public Dt.Cells.Data.ConditionFormatPart ConditionFormatPart
        {
            get { return  this.conditionFormatPart; }
        }

        /// <summary>
        /// Gets the date-time format information for this formatter.
        /// </summary>
        /// <value>
        /// The date-time format information for this formatter. 
        /// The default value is the System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat object.
        /// </value>
        public System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
        {
            get { return  this.Formatter.DateTimeFormatInfo; }
            set
            {
                this.Formatter.DateTimeFormatInfo = value;
                this.dateTimeFormatInfo = value;
            }
        }

        /// <summary>
        /// Gets the DB number format for this formatter.
        /// </summary>
        /// <value>The DB number format for this formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public Dt.Cells.Data.DBNumberFormatPart DBNumberFormatPart
        {
            get { return  this.dbNumberFormatPart; }
        }

        /// <summary>
        /// Gets the Excel-compatible format string.
        /// </summary>
        /// <value>The Excel-compatible format string.</value>
        internal string ExcelCompatibleFormatString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                if ((this.numberFormat != null) && (this.numberFormat.ExcelCompatibleFormatString != null))
                {
                    if (!(this.numberFormat is NumberFormatDigital))
                    {
                        if (this.DBNumberFormatPart != null)
                        {
                            builder.Append(this.DBNumberFormatPart.ToString());
                        }
                        if (this.LocaleIDFormatPart != null)
                        {
                            builder.Append(this.LocaleIDFormatPart.ToString());
                        }
                        if (this.ConditionFormatPart != null)
                        {
                            builder.Append(this.ConditionFormatPart.ToString());
                        }
                        if (this.ColorFormatPart != null)
                        {
                            builder.Append(this.ColorFormatPart.ToString());
                        }
                    }
                    builder.Append(this.numberFormat.ExcelCompatibleFormatString);
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the format string.
        /// </summary>
        /// <value>The format string. The default value is "General".</value>
        [DefaultValue("General")]
        public string FormatString
        {
            get
            {
                string s = string.Empty;
                if (this.formatStringTemplate != null)
                {
                    s = this.formatStringTemplate.ToString();
                }
                if ((this.numberFormat != null) && (this.numberFormat.FormatString != null))
                {
                    if (this.DBNumberFormatPart != null)
                    {
                        s = DefaultTokens.ReplaceKeyword(s, this.dbNumberFormatString, this.DBNumberFormatPart.ToString());
                    }
                    if (this.LocaleIDFormatPart != null)
                    {
                        s = DefaultTokens.ReplaceKeyword(s, this.localIDFormatString, this.LocaleIDFormatPart.ToString());
                    }
                    if (this.ConditionFormatPart != null)
                    {
                        s = DefaultTokens.ReplaceKeyword(s, this.conditionFormatString, this.ConditionFormatPart.ToString());
                    }
                    if (this.ColorFormatPart != null)
                    {
                        s = DefaultTokens.ReplaceKeyword(s, this.colorFormatString, this.ColorFormatPart.ToString());
                    }
                }
                if (string.IsNullOrEmpty(s))
                {
                    return NumberFormatBase.General;
                }
                return s;
            }
        }

        /// <summary>
        /// Gets the number format for this formatter.
        /// </summary>
        /// <value>The number format for this formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public NumberFormatBase Formatter
        {
            get { return  this.numberFormat; }
        }

        /// <summary>
        /// Gets the locale format for this formatter.
        /// </summary>
        /// <value>The locale format for this formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public Dt.Cells.Data.LocaleIDFormatPart LocaleIDFormatPart
        {
            get { return  this.localeIDFormatPart; }
        }

        /// <summary>
        /// Gets or sets the number format information for this formatter.
        /// </summary>
        /// <value>
        /// The number format information for this formatter. 
        /// The default value is the System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormatInfo object.
        /// </value>
        public System.Globalization.NumberFormatInfo NumberFormatInfo
        {
            get { return  this.Formatter.NumberFormatInfo; }
            set
            {
                this.Formatter.NumberFormatInfo = value;
                this.numberFormatInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the number string converter for this formatter.
        /// </summary>
        /// <value>The number string converter for this formatter. The default value is null.</value>
        [DefaultValue((string) null)]
        public INumberStringConverter NumberStringConverter
        {
            get
            {
                if (this.numberFormat != null)
                {
                    return this.numberFormat.NumberStringConverter;
                }
                return null;
            }
            set
            {
                if (this.numberFormat != null)
                {
                    this.numberFormat.NumberStringConverter = value;
                }
            }
        }

        internal string OriginalFormat
        {
            get { return  this.formatCached; }
        }
    }
}

