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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a text number format.  
    /// </summary>
    internal sealed class NumberFormatText : NumberFormatBase
    {
        public static readonly string CommercialAt = "@";
        string excelFormatString;
        string formatString;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.NumberFormatText" /> class.
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="partLocaleID">The part locale ID.</param>
        /// <param name="dbNumberFormatPart">The db number format part.</param>
        /// <param name="culture">The culture.</param>
        internal NumberFormatText(string format, object partLocaleID, object dbNumberFormatPart, CultureInfo culture) : base(partLocaleID, dbNumberFormatPart, culture)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            string s = NumberFormatBase.TrimNotSupportSymbol(format, false);
            if (partLocaleID != null)
            {
                s = DefaultTokens.ReplaceKeyword(s, base.PartLocaleID.OriginalToken, base.PartLocaleID.CurrencySymbol);
            }
            this.excelFormatString = s;
            s = DefaultTokens.TrimEscape(DefaultTokens.Filter(s, DefaultTokens.LeftSquareBracket, DefaultTokens.RightSquareBracket));
            this.formatString = s;
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
            return true;
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns the formatted value.</returns>
        public override string Format(object value)
        {
            try
            {
                string newValue = FormatConverter.ToString(value, true);
                string str2 = this.formatString.Replace("\"", "");
                if (str2 != null)
                {
                    newValue = str2.Replace(CommercialAt, newValue);
                }
                return newValue;
            }
            catch
            {
            }
            return string.Empty;
        }

        /// <summary>
        /// Parses the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>Returns the parsed value.</returns>
        public override object Parse(string format)
        {
            if (format == null)
            {
                return string.Empty;
            }
            return format;
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
            get { return  this.formatString; }
        }
    }
}

