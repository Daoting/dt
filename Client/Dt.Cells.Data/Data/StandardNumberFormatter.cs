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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a standard number format.
    /// </summary>
    internal sealed class StandardNumberFormatter : IFormatter, IXmlSerializable
    {
        static readonly string CurrencyPattern1 = "c";
        static readonly string CurrencyPattern2 = "C";
        static readonly string DecimalPattern1 = "d";
        static readonly string DecimalPattern2 = "D";
        static readonly string FixedPointPattern1 = "f";
        static readonly string FixedPointPattern2 = "F";
        /// <summary>
        /// the format provider.
        /// </summary>
        NumberFormatInfo formatProvider;
        /// <summary>
        /// the format string
        /// </summary>
        string formatString;
        static readonly string GeneralPattern1 = "g";
        static readonly string GeneralPattern2 = "G";
        static readonly string HexadecimalPattern1 = "x";
        static readonly string HexadecimalPattern2 = "X";
        static readonly string NumberPattern1 = "n";
        static readonly string NumberPattern2 = "N";
        static readonly string PercentPattern1 = "p";
        static readonly string PercentPattern2 = "P";
        static readonly string RoundTripPattern1 = "r";
        static readonly string RoundTripPattern2 = "R";
        static readonly string ScientificPattern1 = "e";
        static readonly string ScientificPattern2 = "E";

        /// <summary>
        /// Creates a new standard number format.
        /// </summary>
        public StandardNumberFormatter() : this(GeneralPattern1)
        {
        }

        /// <summary>
        /// Creates a new standard number format.
        /// </summary>
        /// <param name="format">The format.</param>
        public StandardNumberFormatter(string format)
        {
            if (!EvaluateFormat(format))
            {
                throw new FormatException(ResourceStrings.FormatIllegalFormatError);
            }
            this.formatString = format;
        }

        /// <summary>
        /// Adds the decimal precision.
        /// </summary>
        /// <param name="baseFormat">The base format</param>
        /// <param name="c">The precision char for string format.</param>
        /// <param name="count">The number of the decimal precision.</param>
        /// <returns>Returns the string format.</returns>
        static string AddDecimalPrecision(string baseFormat, char c, int count)
        {
            string str = baseFormat;
            if (count > 0)
            {
                str = str + DefaultTokens.DecimalSeparator + ((string) new string(c, count));
            }
            return str;
        }

        /// <summary>
        /// Adds the integral precision.
        /// </summary>
        /// <param name="baseFormat">The base format</param>
        /// <param name="c">The precision char for string format.</param>
        /// <param name="count">The number of the decimal precision.</param>
        /// <returns>Returns the string format.</returns>
        static string AddIntegralPrecision(string baseFormat, char c, int count)
        {
            string str = baseFormat;
            if (count > 0)
            {
                str = ((string) new string(c, count)) + str;
            }
            return str;
        }

        /// <summary>
        /// Determines whether the specified formatter is equal to the current formatter.
        /// </summary>
        public override bool Equals(object obj)
        {
            return ((obj is StandardNumberFormatter) && (object.Equals(this.formatProvider, ((StandardNumberFormatter) obj).formatProvider) && (this.formatString == ((StandardNumberFormatter) obj).formatString)));
        }

        /// <summary>
        /// Determines whether the specified format string is valid.
        /// </summary>
        /// <param name="format">The format to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
        /// </returns>
        public static bool EvaluateFormat(string format)
        {
            if (((format != null) && (format != string.Empty)) && (format.Length > 0))
            {
                string str = format.Substring(0, 1);
                if (((((str == CurrencyPattern1) || (str == CurrencyPattern2)) || ((str == DecimalPattern1) || (str == DecimalPattern2))) || (((str == ScientificPattern1) || (str == ScientificPattern2)) || ((str == FixedPointPattern1) || (str == FixedPointPattern2)))) || ((((str == GeneralPattern1) || (str == GeneralPattern2)) || ((str == NumberPattern1) || (str == NumberPattern2))) || ((((str == PercentPattern1) || (str == PercentPattern2)) || ((str == RoundTripPattern1) || (str == RoundTripPattern2))) || ((str == HexadecimalPattern1) || (str == HexadecimalPattern2)))))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Formats the specified object as a string.
        /// </summary>
        /// <param name="obj">Object with cell data to format.</param>
        /// <returns>Returns the formatted string.</returns>
        public string Format(object obj)
        {
            string str = string.Empty;
            try
            {
                if (obj is IFormattable)
                {
                    return NumberHelper.Format<IFormattable>(obj as IFormattable, this.formatString, (IFormatProvider) this.FormatProvider);
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
                return ((double) nullable.Value).ToString(this.formatString, (IFormatProvider) this.FormatProvider);
            }
            catch
            {
                str = FormatConverter.ToString(obj, true);
            }
            return str;
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
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>Returns the parsed object.</returns>
        public object Parse(string s)
        {
            if ((s != null) && (s != string.Empty))
            {
                if (DefaultTokens.IsDecimal(s, this.FormatProvider))
                {
                    double num;
                    if (double.TryParse(s, (NumberStyles)NumberStyles.Any, (IFormatProvider)this.FormatProvider, out num))
                    {
                        return (double) num;
                    }
                }
                else
                {
                    int num2;
                    if (int.TryParse(s, (NumberStyles)NumberStyles.Any, (IFormatProvider)this.FormatProvider, out num2))
                    {
                        return (int) num2;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
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
            this.formatString = null;
            this.formatProvider = null;
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str == "FormatString")
                    {
                        this.formatString = Serializer.ReadAttribute("value", reader);
                    }
                    else if (str == "NumberFormatInfo")
                    {
                        goto Label_0080;
                    }
                }
                continue;
            Label_0080:
                this.formatProvider = Serializer.DeserializeNumberFormatInfo(reader);
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
        }

        /// <summary>
        /// Gets the Excel-compatible format string.
        /// </summary>
        /// <value>The Excel-compatible format string.</value>
        internal string ExcelCompatibleFormatString
        {
            get
            {
                if (this.FormatString.StartsWith(CurrencyPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    string baseFormat = string.Format("{0}{1}", (object[]) new object[] { DefaultTokens.NumberFormatInfo.CurrencySymbol, "#,##0" });
                    int currencyDecimalDigits = DefaultTokens.NumberFormatInfo.CurrencyDecimalDigits;
                    if (this.Precision.HasValue)
                    {
                        currencyDecimalDigits = this.Precision.Value;
                    }
                    return AddDecimalPrecision(baseFormat, DefaultTokens.Zero, currencyDecimalDigits);
                }
                if (this.FormatString.StartsWith(DecimalPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    string str3 = "0";
                    int num2 = 1;
                    if (this.Precision.HasValue)
                    {
                        num2 = this.Precision.Value;
                    }
                    return AddIntegralPrecision(str3, DefaultTokens.Zero, num2 - 1);
                }
                if (this.FormatString.StartsWith(ScientificPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    string str5 = "0";
                    string str6 = "E+000";
                    if (this.Precision.HasValue)
                    {
                        return (AddDecimalPrecision(str5, DefaultTokens.Zero, this.Precision.Value) + str6);
                    }
                    return "0.########################################E+000";
                }
                if (this.FormatString.StartsWith(FixedPointPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    string str8 = "0";
                    int numberDecimalDigits = DefaultTokens.NumberFormatInfo.NumberDecimalDigits;
                    if (this.Precision.HasValue)
                    {
                        numberDecimalDigits = this.Precision.Value;
                    }
                    return AddDecimalPrecision(str8, DefaultTokens.Zero, numberDecimalDigits);
                }
                if (this.FormatString.StartsWith(GeneralPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    return NumberFormatBase.General;
                }
                if (this.FormatString.StartsWith(NumberPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    string str10 = "#,##0";
                    int count = DefaultTokens.NumberFormatInfo.NumberDecimalDigits;
                    if (this.Precision.HasValue)
                    {
                        count = this.Precision.Value;
                    }
                    return AddDecimalPrecision(str10, DefaultTokens.Zero, count);
                }
                if (this.FormatString.StartsWith(PercentPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    string str12 = "0";
                    int num5 = DefaultTokens.NumberFormatInfo.NumberDecimalDigits;
                    if (this.Precision.HasValue)
                    {
                        num5 = this.Precision.Value;
                    }
                    return (AddDecimalPrecision(str12, DefaultTokens.Zero, num5) + DefaultTokens.PercentSymbol);
                }
                if (this.FormatString.StartsWith(RoundTripPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    return NumberFormatBase.General;
                }
                if (this.FormatString.StartsWith(HexadecimalPattern1, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    return NumberFormatBase.General;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the format provider.
        /// </summary>
        /// <value>The format provider.</value>
        public NumberFormatInfo FormatProvider
        {
            get { return  this.formatProvider; }
            set { this.formatProvider = value; }
        }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        [DefaultValue("g")]
        public string FormatString
        {
            get { return  this.formatString; }
            set
            {
                if (!EvaluateFormat(value))
                {
                    throw new FormatException(ResourceStrings.FormatIllegalFormatError);
                }
                this.formatString = value;
            }
        }

        /// <summary>
        /// Gets the precision.
        /// </summary>
        /// <value>The precision.</value>
        internal int? Precision
        {
            get
            {
                int num;
                if (((this.FormatString != null) && (this.FormatString.Length > 0)) && int.TryParse(this.FormatString.Remove(0, 1), out num))
                {
                    return new int?(num);
                }
                return null;
            }
        }
    }
}

