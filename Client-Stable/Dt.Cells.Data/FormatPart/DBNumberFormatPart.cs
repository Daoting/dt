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
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a number format.
    /// </summary>
    internal sealed class DBNumberFormatPart : FormatPartBase
    {
        /// <summary>
        /// the abs time token
        /// </summary>
        string token;
        /// <summary>
        /// the type of time part.
        /// </summary>
        int type;

        /// <summary>
        /// Creates a new DB number format. 
        /// </summary>
        /// <param name="token">The string expression for the format.</param>
        public DBNumberFormatPart(string token) : base(token)
        {
            if (!EvaluateFormat(token))
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
            this.token = token;
            string s = DefaultTokens.TrimSquareBracket(token).Remove(0, "dbnum".Length);
            this.type = int.Parse(s);
            if ((this.type < 0) || (this.type > 3))
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
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
            if ((token == null) || (token == string.Empty))
            {
                return false;
            }
            string str = DefaultTokens.TrimSquareBracket(token);
            if ((str == null) || (str == string.Empty))
            {
                return false;
            }
            return str.StartsWith("DBNum", (StringComparison) StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Numbers the string.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="numbers">The numbers</param>
        /// <returns>Returns the formatted number.</returns>
        static string FormatNumberString(string value, IList<string> numbers)
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
        static string FormatNumberString(string value, IList<string> units, IList<string> numbers)
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
        /// <param name="value">The value to convert</param>
        /// <param name="dbNumber">Data number part.</param>
        /// <param name="isNumber">if set to <c>true</c> to convert the number to string with the special number char.</param>
        /// <returns>Returns the formatted value.</returns>
        internal string NumberString(double value, DBNumber dbNumber, bool isNumber)
        {
            string s = Convert.ToString(value);
            return this.ReplaceNumberString(s, dbNumber, isNumber);
        }

        /// <summary>
        /// Converts numbers the string.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="dbNumber">The db number</param>
        /// <param name="isNumber">Indicates whether the number should be formatted by number rules</param>
        /// <returns>Returns the formatted value.</returns>
        internal string NumberString(int value, DBNumber dbNumber, bool isNumber)
        {
            return FormatNumberString(Convert.ToString(value), isNumber ? dbNumber.Units : null, dbNumber.Numbers);
        }

        /// <summary>
        /// Converts numbers the string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="dbNumber">The db number.</param>
        /// <param name="isNumber">if set to <c>true</c> to convert the number to string with the special number char.</param>
        /// <returns>Returns the formatted value.</returns>
        internal string NumberString(long value, DBNumber dbNumber, bool isNumber)
        {
            return FormatNumberString(Convert.ToString(value), isNumber ? dbNumber.Units : null, dbNumber.Numbers);
        }

        /// <summary>
        /// Converts numbers the string.
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <param name="dbNumber">Data number part.</param>
        /// <param name="isNumber">Indicates whether the number should be formatted by number rules</param>
        /// <returns>Returns the formatted value.</returns>
        internal string NumberString(string s, DBNumber dbNumber, bool isNumber)
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
            throw new ArgumentException(ResourceStrings.FormatterIllegalValueError);
        }

        /// <summary>
        /// Numbers the string.
        /// </summary>
        /// <param name="s">The string to replace</param>
        /// <param name="dbNumber">The db number</param>
        /// <param name="isNumber">Indicates whether the number should be formatted by number rules</param>
        /// <returns>Returns the formatted value</returns>
        internal string ReplaceNumberString(string s, DBNumber dbNumber, bool isNumber)
        {
            if ((s == null) || (s == string.Empty))
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
                if (char.IsNumber(c) || (DefaultTokens.IsEquals(c, DefaultTokens.DecimalSeparator, false) && !flag))
                {
                    if (DefaultTokens.IsEquals(c, DefaultTokens.DecimalSeparator, false))
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
            return DefaultTokens.AddSquareBracket("DBNum" + ((int) this.type));
        }

        /// <summary>
        /// Gets the token for the format part.
        /// </summary>
        /// <value>The token of the format part. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Token
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
        [DefaultValue(1)]
        public int Type
        {
            get { return  this.type; }
        }
    }
}

