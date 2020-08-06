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
    /// Represents a standard date and time format
    /// </summary>
    internal sealed class StandardDateTimeFormatter : IFormatter, IXmlSerializable
    {
        /// <summary>
        /// the format provider.
        /// </summary>
        DateTimeFormatInfo formatProvider;
        /// <summary>
        /// the format string
        /// </summary>
        string formatString;
        static readonly string FullDatePatternLongTime = "F";
        static readonly string FullDatePatternShortTime = "f";
        static readonly string GeneralDatePatternLongTimeLongTime = "G";
        static readonly string GeneralDatePatternLongTimeShortTime = "g";
        static readonly string LongDatePattern = "D";
        static readonly string LongTimePattern = "T";
        static readonly string MonthDayPattern1 = "m";
        static readonly string MonthDayPattern2 = "M";
        static readonly string RFC1123Pattern1 = "r";
        static readonly string RFC1123Pattern2 = "R";
        static readonly string RoundTripDatePattern1 = "o";
        static readonly string RoundTripDatePattern2 = "O";
        static readonly string ShortDatePattern = "d";
        static readonly string ShortTimePattern = "t";
        static readonly string SortableDatePattern = "s";
        static readonly string UniversalFullDatePattern = "U";
        static readonly string UniversalSortableDatePattern = "u";
        static readonly string YearMonthPattern1 = "y";
        static readonly string YearMonthPattern2 = "Y";

        /// <summary>
        /// Creates a new standard date and time format.
        /// </summary>
        public StandardDateTimeFormatter() : this(GeneralDatePatternLongTimeShortTime)
        {
        }

        /// <summary>
        /// Creates a new standard date and time format with the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        public StandardDateTimeFormatter(string format)
        {
            if (!EvaluateFormat(format))
            {
                throw new FormatException(ResourceStrings.FormatIllegalFormatError);
            }
            this.formatString = format;
        }

        /// <summary>
        /// Determines whether the specified formatter is equal to the current formatter.
        /// </summary>
        /// <param name="obj">The StandardDataTimeFormatter object to compare with the current StandardDataTimeFormatter.</param>
        /// <returns>
        /// true if the specified StandardDataTimeFormatter is equal to the current StandardDataTimeFormatter; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ((obj is StandardDateTimeFormatter) && (object.Equals(this.formatProvider, ((StandardDateTimeFormatter) obj).formatProvider) && (this.formatString == ((StandardDateTimeFormatter) obj).formatString)));
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
            if ((((!(format == ShortDatePattern) && !(format == LongDatePattern)) && (!(format == FullDatePatternShortTime) && !(format == FullDatePatternLongTime))) && ((!(format == GeneralDatePatternLongTimeShortTime) && !(format == GeneralDatePatternLongTimeLongTime)) && (!(format == MonthDayPattern1) && !(format == MonthDayPattern2)))) && (((!(format == RoundTripDatePattern1) && !(format == RoundTripDatePattern2)) && (!(format == RFC1123Pattern1) && !(format == RFC1123Pattern2))) && (((!(format == SortableDatePattern) && !(format == ShortTimePattern)) && (!(format == LongTimePattern) && !(format == UniversalSortableDatePattern))) && ((!(format == UniversalFullDatePattern) && !(format == YearMonthPattern1)) && !(format == YearMonthPattern2)))))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Formats the specified object as a string.
        /// </summary>
        /// <param name="obj">Object with cell data to format.</param>
        /// <returns>Returns the formatted string.</returns>
        public string Format(object obj)
        {
            DateTime time = DateTime.Now;
            DateTime? nullable = FormatConverter.TryDateTime(obj, true);
            if (!nullable.HasValue)
            {
                return FormatConverter.ToString(obj, true);
            }
            return nullable.Value.ToString(this.formatString, (IFormatProvider) this.FormatProvider);
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
        /// <param name="s">The string to parse.</param>
        /// <returns>Returns the parsed object.</returns>
        public object Parse(string s)
        {
            DateTime time;
            if (((s != null) && (s != string.Empty)) && DateTime.TryParseExact(s, this.formatString, (IFormatProvider)this.FormatProvider, (DateTimeStyles)DateTimeStyles.None, out time))
            {
                return time;
            }
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
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
                string format = null;
                switch (this.formatString)
                {
                    case "D":
                        format = DefaultTokens.DateTimeFormatInfo.LongDatePattern;
                        break;

                    case "d":
                        format = DefaultTokens.DateTimeFormatInfo.ShortDatePattern;
                        break;

                    case "F":
                        format = DefaultTokens.DateTimeFormatInfo.FullDateTimePattern;
                        break;

                    case "f":
                        format = DefaultTokens.DateTimeFormatInfo.LongDatePattern + " " + DefaultTokens.DateTimeFormatInfo.ShortTimePattern;
                        break;

                    case "G":
                        format = DefaultTokens.DateTimeFormatInfo.ShortDatePattern + " " + DefaultTokens.DateTimeFormatInfo.LongTimePattern;
                        break;

                    case "g":
                        format = DefaultTokens.DateTimeFormatInfo.ShortDatePattern + " " + DefaultTokens.DateTimeFormatInfo.ShortTimePattern;
                        break;

                    case "M":
                    case "m":
                        format = DefaultTokens.DateTimeFormatInfo.MonthDayPattern;
                        break;

                    case "R":
                    case "r":
                        format = DefaultTokens.DateTimeFormatInfo.RFC1123Pattern;
                        break;

                    case "s":
                        format = DefaultTokens.DateTimeFormatInfo.SortableDateTimePattern;
                        break;

                    case "T":
                        format = DefaultTokens.DateTimeFormatInfo.LongTimePattern;
                        break;

                    case "t":
                        format = DefaultTokens.DateTimeFormatInfo.ShortTimePattern;
                        break;

                    case "u":
                        format = DefaultTokens.DateTimeFormatInfo.UniversalSortableDateTimePattern;
                        break;

                    case "U":
                        format = DefaultTokens.DateTimeFormatInfo.FullDateTimePattern;
                        break;

                    case "Y":
                    case "y":
                        format = DefaultTokens.DateTimeFormatInfo.YearMonthPattern;
                        break;
                }
                if (format != null)
                {
                    NumberFormatDateTime time = new NumberFormatDateTime(format, null, null, null, null);
                    return time.ExcelCompatibleFormatString;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the format provider.
        /// </summary>
        /// <value>The format provider.</value>
        public DateTimeFormatInfo FormatProvider
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
    }
}

