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
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a color format.
    /// </summary>
    /// <remarks>
    /// The user can add a color condition for a format string, for example, "[Red]".
    /// For example,
    /// NFPartColor = [ INTL-COLOR / (NFPartStrColor NFPart1To56) ]
    /// </remarks>
    internal sealed class ColorFormatPart : FormatPartBase
    {
        /// <summary>
        /// color name.
        /// </summary>
        string colorName;
        /// <summary>
        /// the fore color.
        /// </summary>
        Windows.UI.Color foreColor;
        /// <summary>
        /// the color index.
        /// </summary>
        int index;

        /// <summary>
        /// Creates a new color format.
        /// </summary>
        /// <param name="token">The string expression for the color format.</param>
        /// <remarks>Use a string expression such as "[red]" or "[blue]" to specify the color format.</remarks>
        public ColorFormatPart(string token) : base(token)
        {
            this.foreColor = Colors.Black;
            this.index = -1;
            string color = DefaultTokens.TrimSquareBracket(token);
            if ((color == null) || (color == string.Empty))
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
            try
            {
                Windows.UI.Color? nullable = FormatterColorHelper.FromStringValue(color);
                if (nullable.HasValue)
                {
                    this.foreColor = nullable.Value;
                    this.colorName = color;
                    return;
                }

                if (color.Length > "Color".Length)
                {
                    color = color.Remove(0, "Color".Length);
                    int result = -1;
                    if ((int.TryParse(color, out result) && (result >= 1)) && (result <= 0x38))
                    {
                        this.foreColor = FormatterColorHelper.ColorFromIndex(result);
                        this.index = result;
                        return;
                    }
                }
            }
            catch
            {
            }
            throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
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
            if (str.Length < 3)
            {
                return false;
            }
            if (char.IsNumber(token[token.Length - 1]))
            {
                return token.StartsWith("Color", (StringComparison) StringComparison.CurrentCultureIgnoreCase);
            }
            return (token[0] != token[1]);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            if (this.index > -1)
            {
                return DefaultTokens.AddSquareBracket("Color" + ((int) this.index));
            }
            if (this.colorName == null)
            {
                throw new FormatException();
            }
            return DefaultTokens.AddSquareBracket(this.colorName);
        }

        /// <summary>
        /// Gets the format color.
        /// </summary>
        /// <value>
        /// The format color.
        /// The default value is the system window text color.
        /// </value>
        [DefaultValue(typeof(Windows.UI.Color), "WindowText")]
        public Windows.UI.Color ForeColor
        {
            get { return  this.foreColor; }
        }
    }
}

