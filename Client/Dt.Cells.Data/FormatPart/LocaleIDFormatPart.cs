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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a locale format.
    /// </summary>
    /// <remarks>
    /// The user can add a locale format to a formatter, for example "[$$-1009]".
    /// For example, 
    /// NFPartLocaleID = ASCII-LEFT-SQUARE-BRACKET ASCII-DOLLAR-SIGN 1*UTF16-ANY [ASCII-HYPHEN-MINUS 3*8ASCII-DIGIT-HEXADECIMAL] ASCII-RIGHT-SQUARE-BRACKET
    /// </remarks>
    internal sealed class LocaleIDFormatPart : FormatPartBase
    {
        /// <summary>
        /// the content.
        /// </summary>
        string content;
        /// <summary>
        /// culture info.
        /// </summary>
        System.Globalization.CultureInfo cultureInfo;
        /// <summary>
        /// the utf16any
        /// </summary>
        string currencySymbol;
        /// <summary>
        /// the locate id.
        /// </summary>
        int locateID;

        /// <summary>
        /// Creates a new locale format with the specified token.
        /// </summary>
        /// <param name="token">The string expression for the format.</param>
        public LocaleIDFormatPart(string token) : base(token)
        {
            this.locateID = -1;
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }
            if (token == string.Empty)
            {
                throw new FormatException(ResourceStrings.FormatterIllegaTokenError);
            }
            this.content = DefaultTokens.TrimSquareBracket(token);
            string content = this.content;
            if ((content == null) || (content == string.Empty))
            {
                throw new FormatException(ResourceStrings.FormatterIllegaTokenError);
            }
            if (!DefaultTokens.IsEquals(content[0], DefaultTokens.Dollar, false))
            {
                throw new FormatException(ResourceStrings.FormatterIllegaTokenError);
            }
            content = content.Remove(0, 1);
            int index = content.IndexOf(DefaultTokens.HyphenMinus);
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
            if (!DefaultTokens.IsEquals(content[0], DefaultTokens.HyphenMinus, false))
            {
                throw new FormatException(ResourceStrings.FormatterIllegaTokenError);
            }
            content = content.Remove(0, 1);
            if (content.Length <= 0)
            {
                throw new FormatException(ResourceStrings.FormatterIllegaTokenError);
            }
            this.locateID = NumberHelper.ParseHexString(content);
        }

        /// <summary>
        /// Encodes the symbol.
        /// </summary>
        /// <param name="symbol">The format string to encode</param>
        /// <returns>Returns the encoded format string.</returns>
        string EncodeSymbol(string symbol)
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
            if ((token == null) || (token == string.Empty))
            {
                return false;
            }
            string str = DefaultTokens.TrimSquareBracket(token);
            return (((str != null) && !(str == string.Empty)) && DefaultTokens.IsEquals(str[0], DefaultTokens.Dollar, false));
        }

        /// <summary>
        /// Gets a <see cref="T:Dt.Cells.Data.DBNumber" /> object by the number letter type and culture information for this format.
        /// </summary>
        /// <param name="type">The number letter type.</param>
        /// <returns>
        /// Returns a <see cref="T:Dt.Cells.Data.DBNumber" /> object that indicates how the number is formatted.
        /// </returns>
        public DBNumber GetDBNumber(int type)
        {
            switch ((this.locateID & 0xff))
            {
                case 4:
                    switch (type)
                    {
                        case 1:
                            return DBNumber.ChineseDBNum1;

                        case 2:
                            return DBNumber.ChineseDBNum2;

                        case 3:
                            return DBNumber.ChineseDBNum3;
                    }
                    break;

                case 0x11:
                    switch (type)
                    {
                        case 1:
                            return DBNumber.JapaneseDBNum1;

                        case 2:
                            return DBNumber.JapaneseDBNum2;

                        case 3:
                            return DBNumber.JapaneseDBNum3;
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
                return DefaultTokens.AddSquareBracket(this.content);
            }
            return string.Empty;
        }

        /// <summary>
        /// Indicates whether the current culture supports scientific number display.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowScience
        {
            get { return  CultureHelper.AllowScience(this.cultureInfo); }
        }

        /// <summary>
        /// Gets the culture information for the format.
        /// </summary>
        /// <value>The culture information for the format.</value>
        public System.Globalization.CultureInfo CultureInfo
        {
            get
            {
                if (this.cultureInfo == null)
                {
                    this.cultureInfo = CultureHelper.CreateCultureInfo(this.locateID);
                    if (((this.currencySymbol != null) && (this.currencySymbol != string.Empty)) && !this.cultureInfo.NumberFormat.IsReadOnly)
                    {
                        this.cultureInfo.NumberFormat.CurrencySymbol = this.currencySymbol;
                    }
                    if (((CultureHelper.CreateCalendar(this.locateID) != null) && ((this.cultureInfo.Name == "ja-JP") || (this.cultureInfo.Name == "ja"))) && (this.cultureInfo.OptionalCalendars.Length > 1))
                    {
                        this.cultureInfo.DateTimeFormat.Calendar = this.cultureInfo.OptionalCalendars[1];
                    }
                }
                return this.cultureInfo;
            }
        }

        /// <summary>
        /// Gets the currency symbol for the format.
        /// </summary>
        /// <value>The currency symbol for the format. The default value is a dollar sign ($).</value>
        [DefaultValue("$")]
        public string CurrencySymbol
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
}

