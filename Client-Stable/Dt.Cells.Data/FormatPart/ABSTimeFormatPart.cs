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
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the absolute time format.
    /// </summary>
    internal sealed class ABSTimeFormatPart : FormatPartBase
    {
        /// <summary>
        /// the format string.
        /// </summary>
        string formatString;
        /// <summary>
        /// Custom format for absolute hours in Excel (started with 1900-1-1 0:0:0).
        /// </summary>
        static readonly char HoursABSContent = 'h';
        /// <summary>
        /// Custom format for absolute minutes in Excel (started with 1900-1-1 0:0:0).
        /// </summary>
        static readonly char MinuteABSContent = 'm';
        /// <summary>
        /// Custom format for absolute seconds in Excel (started with 1900-1-1 0:0:0).
        /// </summary>
        static readonly char SecondABSContent = 's';
        /// <summary>
        /// the abs time token
        /// </summary>
        string token;
        /// <summary>
        /// the type of time part.
        /// </summary>
        TimePart type;

        /// <summary>
        /// Creates a new absolute time format.
        /// </summary>
        /// <param name="token">The string expression of the absolute time format.</param>
        /// <remarks>To create the format, use a string such as "[h], [m], [s]".</remarks>
        public ABSTimeFormatPart(string token) : base(token)
        {
            if (!EvaluateFormat(token))
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
            this.token = token.ToLower();
            if (this.token[1] == HoursABSContent)
            {
                this.type = TimePart.Hour;
            }
            else if (this.token[1] == MinuteABSContent)
            {
                this.type = TimePart.Minute;
            }
            else
            {
                if (this.token[1] != SecondABSContent)
                {
                    throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
                }
                this.type = TimePart.Second;
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
            if ((token == null) || (token == string.Empty))
            {
                return false;
            }
            string str = DefaultTokens.TrimSquareBracket(token);
            if ((str == null) || (str == string.Empty))
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
                if (((ch != HoursABSContent) && (ch != MinuteABSContent)) && (ch != SecondABSContent))
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
        [DefaultValue("")]
        public string FormatString
        {
            get { return  this.formatString; }
        }

        /// <summary>
        /// Gets the time unit type.
        /// </summary>
        /// <value>
        /// A value that specifies the time unit type.
        /// The default value is <see cref="T:Dt.Cells.Data.TimePart">Hour</see>.
        /// </value>
        [DefaultValue(0)]
        public TimePart TimePartType
        {
            get { return  this.type; }
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>The token.</value>
        [DefaultValue("")]
        internal string Token
        {
            get { return  this.token; }
        }
    }
}

