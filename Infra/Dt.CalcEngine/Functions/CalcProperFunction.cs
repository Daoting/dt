#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Capitalizes the first letter in a text string and any other 
    /// letters in text that follow any character other than a letter. 
    /// Converts all other letters to lowercase letters.
    /// </summary>
    public class CalcProperFunction : CalcBuiltinFunction
    {
        private System.Globalization.TextInfo _textInfo;

        private static int AddNonLetter(ref StringBuilder result, ref string input, int inputIndex, int charLen)
        {
            if (charLen == 2)
            {
                result.Append(input[inputIndex++]);
                result.Append(input[inputIndex]);
                return inputIndex;
            }
            result.Append(input[inputIndex]);
            return inputIndex;
        }

        private int AddTitlecaseLetter(ref StringBuilder result, ref string input, int inputIndex, int charLen)
        {
            if (charLen == 2)
            {
                result.Append(this.TextInfo.ToUpper(input.Substring(inputIndex, charLen)));
                inputIndex++;
                return inputIndex;
            }
            switch (input[inputIndex])
            {
                case 'Ǆ':
                case 'ǅ':
                case 'ǆ':
                    result.Append('ǅ');
                    return inputIndex;

                case 'Ǉ':
                case 'ǈ':
                case 'ǉ':
                    result.Append('ǈ');
                    return inputIndex;

                case 'Ǌ':
                case 'ǋ':
                case 'ǌ':
                    result.Append('ǋ');
                    return inputIndex;

                case 'Ǳ':
                case 'ǲ':
                case 'ǳ':
                    result.Append('ǲ');
                    return inputIndex;
            }
            result.Append(this.TextInfo.ToUpper(input[inputIndex]));
            return inputIndex;
        }

        private static bool CheckLetter(UnicodeCategory uc)
        {
            switch (uc)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Capitalizes the first letter in a text string and any other
        /// letters in text that follow any character other than a letter.
        /// Converts all other letters to lowercase letters.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: text.
        /// </para>
        /// <para>
        /// Text is text enclosed in quotation marks,
        /// a formula that returns text you want to partially capitalize.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string @this = CalcConvert.ToString(args[0]);
            return this.ToTitleCase(@this.ToLower(CultureInfo.CurrentCulture));
        }

        private static int InternalConvertToUtf32(string s, int index, out int charLength)
        {
            charLength = 1;
            if (index < (s.Length - 1))
            {
                int num = s[index] - 0xd800;
                if ((num >= 0) && (num <= 0x3ff))
                {
                    int num2 = s[index + 1] - 0xdc00;
                    if ((num2 >= 0) && (num2 <= 0x3ff))
                    {
                        charLength++;
                        return (((num * 0x400) + num2) + 0x10000);
                    }
                }
            }
            return s[index];
        }

        private static bool IsLetterCategory(UnicodeCategory uc)
        {
            if (((uc != UnicodeCategory.UppercaseLetter) && (uc != UnicodeCategory.LowercaseLetter)) && ((uc != UnicodeCategory.TitlecaseLetter) && (uc != UnicodeCategory.ModifierLetter)))
            {
                return (uc == UnicodeCategory.OtherLetter);
            }
            return true;
        }

        private static bool IsWordSeparator(UnicodeCategory category)
        {
            return ((0x1ffcf800 & (1 << (int)category)) != 0);
        }

        private string ToTitleCase(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if (str.Length == 0)
            {
                return str;
            }
            StringBuilder result = new StringBuilder();
            string str2 = null;
            for (int i = 0; i < str.Length; i++)
            {
                int num2;
                InternalConvertToUtf32(str, i, out num2);
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(str, i);
                if (CheckLetter(unicodeCategory))
                {
                    i = this.AddTitlecaseLetter(ref result, ref str, i, num2) + 1;
                    int startIndex = i;
                    bool flag = unicodeCategory == UnicodeCategory.LowercaseLetter;
                    while (i < str.Length)
                    {
                        InternalConvertToUtf32(str, i, out num2);
                        unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(str, i);
                        if (IsLetterCategory(unicodeCategory))
                        {
                            if (unicodeCategory == UnicodeCategory.LowercaseLetter)
                            {
                                flag = true;
                            }
                            i += num2;
                        }
                        else
                        {
                            if (str[i] == '\'')
                            {
                                i++;
                                if (flag)
                                {
                                    if (str2 == null)
                                    {
                                        str2 = this.TextInfo.ToLower(str);
                                    }
                                    result.Append(str2, startIndex, i - startIndex);
                                }
                                else
                                {
                                    result.Append(str, startIndex, i - startIndex);
                                }
                                startIndex = i;
                                flag = true;
                                continue;
                            }
                            if (IsWordSeparator(unicodeCategory))
                            {
                                break;
                            }
                            i += num2;
                        }
                    }
                    int count = i - startIndex;
                    if (count > 0)
                    {
                        if (flag)
                        {
                            if (str2 == null)
                            {
                                str2 = this.TextInfo.ToLower(str);
                            }
                            result.Append(str2, startIndex, count);
                        }
                        else
                        {
                            result.Append(str, startIndex, count);
                        }
                    }
                    if (i < str.Length)
                    {
                        i = AddNonLetter(ref result, ref str, i, num2);
                    }
                    continue;
                }
                i = AddNonLetter(ref result, ref str, i, num2);
            }
            return result.ToString();
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public override int MaxArgs
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public override int MinArgs
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "PROPER";
            }
        }

        private System.Globalization.TextInfo TextInfo
        {
            get
            {
                if (this._textInfo == null)
                {
                    this._textInfo = CultureInfo.CurrentCulture.TextInfo;
                }
                return this._textInfo;
            }
        }
    }
}

