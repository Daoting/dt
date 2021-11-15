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
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents all tokens.
    /// </summary>
    internal class DefaultTokens
    {
        /// <summary>
        /// Gets the asterisk.
        /// </summary>
        public static readonly char Asterisk = '*';
        /// <summary>
        /// Gets the colon sign.
        /// </summary>
        public static readonly char Colon = ':';
        /// <summary>
        /// Gets the comma sign.
        /// </summary>
        public static readonly char Comma = ',';
        /// <summary>
        /// Gets the commercial at sign.
        /// </summary>
        public static readonly char CommercialAt = '@';
        /// <summary>
        /// Gets the dollar sign.
        /// </summary>
        public static readonly char Dollar = '$';
        /// <summary>
        /// Gets the double quote.
        /// </summary>
        public static readonly char DoubleQuote = '"';
        /// <summary>
        /// Gets the array end character.
        /// </summary>
        public static readonly char EndCharOfArray = '\0';
        /// <summary>
        /// Gets the equals sign.
        /// </summary>
        public static readonly char EqualsThanSign = '=';
        /// <summary>
        /// Gets the greater than sign.
        /// </summary>
        public static readonly char GreaterThanSign = '>';
        /// <summary>
        /// Gets the hyphen minus.
        /// </summary>
        public static readonly char HyphenMinus = '-';
        /// <summary>
        /// Gets the left parenthesis.
        /// </summary>
        public static readonly char LeftParenthesis = '(';
        /// <summary>
        /// Gets the left square bracket.
        /// </summary>
        public static readonly char LeftSquareBracket = '[';
        /// <summary>
        /// Gets the less than sign.
        /// </summary>
        public static readonly char LessThanSign = '<';
        /// <summary>
        /// Gets the number sign.
        /// </summary>
        public static readonly char NumberSign = '#';
        /// <summary>
        /// Gets the plus sign.
        /// </summary>
        public static readonly char PlusSign = '+';
        /// <summary>
        /// Gets the question mark.
        /// </summary>
        public static readonly char QuestionMark = '?';
        /// <summary>
        /// Gets the reverse solidus sign.
        /// </summary>
        public static readonly char ReverseSolidusSign = '\\';
        /// <summary>
        /// Gets the right parenthesis.
        /// </summary>
        public static readonly char RightParenthesis = ')';
        /// <summary>
        /// Gets the right square bracket.
        /// </summary>
        public static readonly char RightSquareBracket = ']';
        /// <summary>
        /// Gets the semicolon sign.
        /// </summary>
        public static readonly char Semicolon = ';';
        /// <summary>
        /// Gets the sharp sign.
        /// </summary>
        public static readonly char Sharp = '#';
        /// <summary>
        /// Gets the single quote.
        /// </summary>
        public static readonly char SingleQuote = '\'';
        /// <summary>
        /// Gets the solidus sign.
        /// </summary>
        public static readonly char SolidusSign = '/';
        /// <summary>
        /// Gets the space character.
        /// </summary>
        public static readonly char Space = ' ';
        /// <summary>
        /// Gets the tab character.
        /// </summary>
        public static readonly char Tab = '\t';
        /// <summary>
        /// Gets the under line.
        /// </summary>
        public static readonly char UnderLine = '_';
        /// <summary>
        /// Gets the zero digit.
        /// </summary>
        public static readonly char Zero = '0';

        /// <summary>
        /// Adds a square bracket.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Returns the formatted string.</returns>
        public static string AddSquareBracket(string token)
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
        /// <param name="s">The string.</param>
        /// <param name="bracketsStart">The start bracket.</param>
        /// <param name="bracketsEnd">The end bracket.</param>
        /// <returns>Returns the filtered string.</returns>
        public static string Filter(string s, char bracketsStart, char bracketsEnd)
        {
            if ((s == null) || (s == string.Empty))
            {
                return s;
            }
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            int num = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                if ((ch == bracketsStart) && !flag)
                {
                    num++;
                }
                else if ((ch == bracketsEnd) && !flag)
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
                if (ch == ReverseSolidusSign)
                {
                    flag = !flag;
                }
                else
                {
                    flag = false;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Determines whether the specified string is a decimal.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="numberFormatInfo">The number format information.</param>
        /// <returns>
        /// <c>true</c> if the specified string is a decimal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDecimal(string s, System.Globalization.NumberFormatInfo numberFormatInfo)
        {
            string decimalSeparator = DecimalSeparator;
            if (numberFormatInfo != null)
            {
                decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            }
            return (s.IndexOf(decimalSeparator) > -1);
        }

        /// <summary>
        /// Determines whether the specified characters are equal.
        /// </summary>
        /// <param name="a">The first character.</param>
        /// <param name="b">The second character.</param>
        /// <param name="isIgnoreCase">if set to <c>true</c>, ignore the case when comparing.</param>
        /// <returns>
        /// <c>true</c> if the two characters are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEquals(char a, char b, bool isIgnoreCase)
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
        /// <param name="a">The character.</param>
        /// <param name="b">The string.</param>
        /// <param name="isIgnoreCase">if set to <c>true</c>, ignore the case when comparing.</param>
        /// <returns>
        /// <c>true</c> if the character is equal to the string; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEquals(char a, string b, bool isIgnoreCase)
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
        /// Determines whether the specified string equals the specified character.
        /// </summary>
        /// <param name="a">The string.</param>
        /// <param name="b">The character.</param>
        /// <param name="isIgnoreCase">if set to <c>true</c>, ignore the case when comparing.</param>
        /// <returns>
        /// <c>true</c> if the string is equal to the character; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEquals(string a, char b, bool isIgnoreCase)
        {
            if (a == null)
            {
                return false;
            }
            if (a.Length != 1)
            {
                return false;
            }
            return IsEquals(a[0], b, isIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified character is an operator.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>
        /// <c>true</c> if the specified character is an operator; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOperator(char c)
        {
            if ((c != LessThanSign) && (c != GreaterThanSign))
            {
                return (c == EqualsThanSign);
            }
            return true;
        }

        public static string ReplaceChar(string s, char oldChar, char newChar)
        {
            if ((s == null) || (s == string.Empty))
            {
                return string.Empty;
            }
            bool flag = false;
            bool flag2 = false;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                if ((ch == DoubleQuote) && !flag)
                {
                    flag2 = !flag2;
                }
                if ((!flag && !flag2) && (ch == oldChar))
                {
                    builder.Append(newChar);
                }
                else
                {
                    builder.Append(ch);
                }
                if (ch == ReverseSolidusSign)
                {
                    flag = !flag;
                }
                else
                {
                    flag = false;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Replaces the specified string with a new string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="oldString">The old string.</param>
        /// <param name="newString">The new string.</param>
        /// <returns>Returns the replaced format string.</returns>
        public static string ReplaceKeyword(string s, string oldString, string newString)
        {
            if ((s == null) || (s == string.Empty))
            {
                return s;
            }
            string str = s;
            int num = 0;
            while (true)
            {
                int startIndex = str.IndexOf(oldString, num, (StringComparison) StringComparison.CurrentCultureIgnoreCase);
                if (startIndex <= -1)
                {
                    return str;
                }
                str = str.Remove(startIndex, oldString.Length).Insert(startIndex, newString);
                num = startIndex + newString.Length;
            }
        }

        public static string[] Split(string s, char spliter)
        {
            List<string> list = new List<string>();
            char ch = '"';
            if ((s != null) && (s != string.Empty))
            {
                bool flag = false;
                StringBuilder builder = new StringBuilder();
                bool flag2 = false;
                for (int i = 0; i < s.Length; i++)
                {
                    char ch2 = s[i];
                    if ((ch2 == ch) && !flag)
                    {
                        flag2 = !flag2;
                    }
                    if ((!flag && !flag2) && (ch2 == spliter))
                    {
                        list.Add(builder.ToString());
                        builder.Clear();
                    }
                    else
                    {
                        builder.Append(ch2);
                    }
                    if (ch2 == ReverseSolidusSign)
                    {
                        flag = !flag;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                list.Add(builder.ToString());
            }
            return list.ToArray();
        }

        public static string TrimEscape(string token)
        {
            string str = token;
            int length = str.Length;
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                char ch = str[i];
                if (ch == ReverseSolidusSign)
                {
                    flag = !flag;
                    if (!flag)
                    {
                        builder.Append(ch);
                    }
                }
                else
                {
                    flag = false;
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Trims the square bracket.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Returns the trimmed format string.</returns>
        public static string TrimSquareBracket(string token)
        {
            if ((token != null) && (token != string.Empty))
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
        /// Gets the custom format for the short AM or PM symbol in Excel.
        /// </summary>
        public static string AMPMSingleDigit
        {
            get { return  "A/P"; }
        }

        /// <summary>
        /// Gets the custom format for the default AM or PM symbol in Excel.
        /// </summary>
        public static string AMPMTwoDigit
        {
            get { return  "AM/PM"; }
        }

        /// <summary>
        /// Gets the asterisk wildcard.
        /// </summary>
        /// <value>The asterisk wildcard.</value>
        public static string AsteriskWildcard
        {
            get { return  ((char) Asterisk).ToString(); }
        }

        /// <summary>
        /// Gets the asterisk wildcard regular expression.
        /// </summary>
        /// <value>The asterisk wildcard regular expression.</value>
        public static string AsteriskWildcardRegularExpression
        {
            get { return  @"((.|\n)*)"; }
        }

        /// <summary>
        /// Gets the date time format information.
        /// </summary>
        /// <value>The date time format information.</value>
        public static System.Globalization.DateTimeFormatInfo DateTimeFormatInfo
        {
            get { return  CultureInfo.CurrentCulture.DateTimeFormat; }
        }

        /// <summary>
        /// Gets the decimal separator.
        /// </summary>
        /// <remarks>By default, the decimal separator is ".".</remarks>
        public static string DecimalSeparator
        {
            get { return  NumberFormatInfo.NumberDecimalSeparator; }
        }

        /// <summary>
        /// Gets the exponential positive symbol.
        /// </summary>
        /// <remarks>By default, the exponential positive symbol is "e+".</remarks>
        public static string Exponential1
        {
            get { return  "E+"; }
        }

        /// <summary>
        /// Gets the exponential negative symbol.
        /// </summary>
        /// <remarks>By default, the exponential negative symbol is "e-".</remarks>
        public static string Exponential2
        {
            get { return  "E-"; }
        }

        /// <summary>
        /// Gets the exponential symbol.
        /// </summary>
        /// <remarks>By default, the exponential symbol is "e".</remarks>
        public static string ExponentialSymbol
        {
            get { return  "E"; }
        }

        /// <summary>
        /// Gets the format separator.
        /// </summary>
        public static char FormatSeparator
        {
            get { return  Semicolon; }
        }

        /// <summary>
        /// Gets the list separator.
        /// </summary>
        /// <value>The list separator.</value>
        public static string ListSeparator
        {
            get { return  CultureInfo.CurrentCulture.TextInfo.ListSeparator; }
        }

        /// <summary>
        /// Gets the NaN symbol.
        /// </summary>
        /// <value>The NaN symbol.</value>
        public static string NaNSymbol
        {
            get { return  NumberFormatInfo.NaNSymbol; }
        }

        /// <summary>
        /// Gets the minus sign.
        /// </summary>
        /// <remarks>By default, the negative sign is "-".</remarks>
        public static string NegativeSign
        {
            get { return  NumberFormatInfo.NegativeSign; }
        }

        /// <summary>
        /// Gets the number format information.
        /// </summary>
        /// <value>The number format information.</value>
        public static System.Globalization.NumberFormatInfo NumberFormatInfo
        {
            get { return  CultureInfo.CurrentCulture.NumberFormat; }
        }

        /// <summary>
        /// Gets the number group separator.
        /// </summary>
        /// <remarks>By default, the separator is ",".</remarks>
        public static string NumberGroupSeparator
        {
            get { return  NumberFormatInfo.NumberGroupSeparator.Replace('\x00a0', ' '); }
        }

        /// <summary>
        /// Gets the percent sign.
        /// </summary>
        /// <remarks>By default, the percent sign is "%".</remarks>
        public static string PercentSymbol
        {
            get { return  NumberFormatInfo.PercentSymbol; }
        }

        /// <summary>
        /// Gets the plus sign.
        /// </summary>
        /// <remarks>By default, the positive sign is "+".</remarks>
        public static string PositiveSign
        {
            get { return  NumberFormatInfo.PositiveSign; }
        }

        /// <summary>
        /// Gets the question mark wildcard.
        /// </summary>
        /// <value>The question mark wildcard.</value>
        public static string QuestionMarkWildcard
        {
            get { return  ((char) QuestionMark).ToString(); }
        }

        /// <summary>
        /// Gets the question mark wildcard regular expression.
        /// </summary>
        /// <value>The question mark wildcard regular expression.</value>
        public static string QuestionMarkWildcardRegularExpression
        {
            get { return  "."; }
        }

        /// <summary>
        /// Gets the prefix of the placeholder.
        /// </summary>
        public static string ReplacePlaceholder
        {
            get { return  "@"; }
        }
    }
}

