#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    internal class Tokenizer
    {
        private int currentLength;
        private int currentStart;
        private static HashSet<char> delimiteHashSet = new HashSet<char>();
        private static string[] delimiters = new string[] { 
            "<>", "<=", ">=", "+", "-", "*", "/", "%", "^", "&", "=", "<", ">", "(", ")", "{", 
            "}", ",", ";"
         };
        private static string[] errors = new string[] { "#DIV/0!", "#NAME?", "#N/A", "#NULL!", "#NUM!", "#REF!", "#VALUE!" };
        private static SurfixTree errorsSurfixTree = new SurfixTree();
        private string text;

        /// <summary>
        /// Constructs a tokenizer for the specified string.
        /// </summary>
        static Tokenizer()
        {
            foreach (string str in errors)
            {
                errorsSurfixTree.AddItem(str);
            }
            foreach (string str2 in delimiters)
            {
                if (str2.Length == 1)
                {
                    delimiteHashSet.Add(str2[0]);
                }
            }
        }

        public Tokenizer(string text)
        {
            this.text = text;
            this.currentStart = 0;
            this.currentLength = 0;
        }

        public Token GetNextToken()
        {
            string str2;
            this.currentStart += this.currentLength;
            this.currentLength = 0;
            int length = this.text.Length;
            if (this.currentStart < length)
            {
                char c = this.text[this.currentStart];
                while ((c == '!') || char.IsWhiteSpace(c))
                {
                    this.currentStart++;
                    if (this.currentStart >= length)
                    {
                        break;
                    }
                    c = this.text[this.currentStart];
                }
                if (c == ':')
                {
                    this.currentLength = 1;
                    return new Token { Category = ToekCategory.Delimiters, Text = ":" };
                }
            }
            if (this.currentStart >= length)
            {
                return null;
            }
            string str = this.StartsWithDelimiter(this.text, this.currentStart);
            if (str != null)
            {
                this.currentLength = str.Length;
                return new Token { Category = ToekCategory.Delimiters, Text = str };
            }
            if ((this.text[this.currentStart] == '#') && ((str2 = this.StartsWithError(this.text, this.currentStart)) != null))
            {
                this.currentLength = str2.Length;
                return new Token { Category = ToekCategory.Error, Text = this.text.Substring(this.currentStart, this.currentLength) };
            }
            if (char.IsDigit(this.text[this.currentStart]) || (this.text[this.currentStart] == '.'))
            {
                int num2 = this.currentStart + this.currentLength;
                bool flag = false;
                while ((num2 < length) && char.IsDigit(this.text[num2]))
                {
                    this.currentLength++;
                    num2++;
                }
                if ((num2 < length) && (this.text[num2] == '.'))
                {
                    this.currentLength++;
                    num2++;
                }
                if ((num2 < length) && (this.text[num2] == ':'))
                {
                    flag = true;
                    this.currentLength++;
                    num2++;
                    if (this.text[num2] == '$')
                    {
                        this.currentLength++;
                        num2++;
                    }
                }
                while ((num2 < length) && char.IsDigit(this.text[num2]))
                {
                    this.currentLength++;
                    num2++;
                }
                if ((num2 < length) && ((this.text[num2] == 'e') || (this.text[num2] == 'E')))
                {
                    this.currentLength++;
                    num2++;
                    if ((num2 < length) && ((this.text[num2] == '-') || (this.text[num2] == '+')))
                    {
                        this.currentLength++;
                        num2++;
                    }
                    while ((num2 < length) && char.IsDigit(this.text[num2]))
                    {
                        this.currentLength++;
                        num2++;
                    }
                }
                if (flag)
                {
                    return new Token { Category = ToekCategory.Unknown, Text = this.text.Substring(this.currentStart, this.currentLength) };
                }
                return new Token { Category = ToekCategory.Number, Text = this.text.Substring(this.currentStart, this.currentLength) };
            }
            if (this.text[this.currentStart] == '"')
            {
                this.currentLength = 1;
                int num3 = this.currentStart + this.currentLength;
                while (((num3 < length) && (this.text[num3] != '"')) || ((((num3 + 1) < length) && (this.text[num3] == '"')) && (this.text[num3 + 1] == '"')))
                {
                    this.currentLength += (this.text[num3] != '"') ? 1 : 2;
                    num3 = this.currentStart + this.currentLength;
                }
                if ((num3 < length) && (this.text[num3] == '"'))
                {
                    this.currentLength++;
                }
                return new Token { Category = ToekCategory.String, Text = this.text.Substring(this.currentStart + 1, this.currentLength - 2) };
            }
            if (!char.IsLetter(this.text[this.currentStart]) && (this.text[this.currentStart] != '_'))
            {
                return this.GetUnknownToken(length);
            }
            int num4 = this.currentStart + 1;
            while ((num4 < length) && this.IsValidRemainingChar(this.text[num4]))
            {
                num4++;
            }
            int num5 = num4;
            while ((num4 < length) && char.IsWhiteSpace(this.text[num4]))
            {
                num4++;
            }
            if ((num4 < length) && (this.text[num4] == '('))
            {
                this.currentLength = (num4 - this.currentStart) + 1;
                return new Token { Category = ToekCategory.Function, Text = this.text.Substring(this.currentStart, num5 - this.currentStart) };
            }
            if ((num4 < length) && (((this.text[num4] == '!') || (this.text[num4] == ':')) || (this.text[num4] == '$')))
            {
                while (((num4 < length) && !char.IsWhiteSpace(this.text[num4])) && (!delimiteHashSet.Contains(this.text[num4]) || ((this.text[num4] == '-') && (this.text[num4 - 1] == '['))))
                {
                    this.currentLength++;
                    if ((this.text[num4] == '-') && (this.text[num4 - 1] == '['))
                    {
                        this.currentLength++;
                        num4++;
                    }
                    num4++;
                }
                num5 = num4;
                this.currentLength = num5 - this.currentStart;
                return new Token { Category = ToekCategory.Unknown, Text = this.text.Substring(this.currentStart, this.currentLength) };
            }
            if ((num4 < length) && (this.text[num4] == '['))
            {
                while (((num4 < length) && !char.IsWhiteSpace(this.text[num4])) && (!delimiteHashSet.Contains(this.text[num4]) || ((this.text[num4] == '-') && (this.text[num4 - 1] == '['))))
                {
                    this.currentLength++;
                    if ((((num4 + 1) < this.text.Length) && (this.text[num4 + 1] == '-')) && (this.text[num4] == '['))
                    {
                        this.currentLength++;
                        num4++;
                    }
                    num4++;
                }
                num5 = num4;
                this.currentLength = num5 - this.currentStart;
                return new Token { Category = ToekCategory.Unknown, Text = this.text.Substring(this.currentStart, this.currentLength) };
            }
            this.currentLength = num5 - this.currentStart;
            return new Token { Category = ToekCategory.Unknown, Text = this.text.Substring(this.currentStart, this.currentLength) };
        }

        private Token GetUnknownToken(int length)
        {
            this.currentLength = 1;
            if (this.text[this.currentStart] == '\'')
            {
                bool flag;
                int num = this.currentStart + this.currentLength;
                do
                {
                    flag = true;
                    while ((num < length) && (this.text[num] != '\''))
                    {
                        num++;
                        flag = false;
                    }
                    while ((((num + 1) < length) && (this.text[num] == '\'')) && (this.text[num + 1] == '\''))
                    {
                        num += 2;
                        flag = false;
                    }
                }
                while (!flag);
                this.currentLength = num - this.currentStart;
                if ((num < length) && (this.text[num] == '\''))
                {
                    this.currentLength++;
                    num++;
                }
            }
            int num2 = this.currentStart + this.currentLength;
            if (this.text[num2] == '!')
            {
                num2++;
                this.currentLength++;
            }
            while ((num2 < length) && (!delimiteHashSet.Contains(this.text[num2]) || ((this.text[num2] == '-') && (this.text[num2 - 1] == '['))))
            {
                this.currentLength++;
                if ((this.text[num2] == '-') && (this.text[num2 - 1] == '['))
                {
                    this.currentLength++;
                    num2++;
                }
                num2++;
            }
            return new Token { Category = ToekCategory.Unknown, Text = this.text.Substring(this.currentStart, this.currentLength) };
        }

        private bool IsValidRemainingChar(char c)
        {
            if (!char.IsLetterOrDigit(c) && (c != '_'))
            {
                return (c == '.');
            }
            return true;
        }

        public Token PeekNextToken()
        {
            int currentStart = this.currentStart;
            int currentLength = this.currentLength;
            Token nextToken = this.GetNextToken();
            this.currentStart = currentStart;
            this.currentLength = currentLength;
            return nextToken;
        }

        /// <summary>
        /// Backs up to the beginning of the current token.
        /// </summary>
        public void PushBack()
        {
            this.currentLength = 0;
        }

        /// <summary>
        /// Determines whether the string begins with a delimiter at
        /// the specified offset.
        /// </summary>
        private string StartsWithDelimiter(string s, int offset)
        {
            int num;
            switch (s[offset])
            {
                case '{':
                    return "{";

                case '}':
                    return "}";

                case '^':
                    return "^";

                case '%':
                    return "%";

                case '&':
                    return "&";

                case '(':
                    return "(";

                case ')':
                    return ")";

                case '*':
                    return "*";

                case '+':
                    return "+";

                case ',':
                    return ",";

                case '-':
                    return "-";

                case '/':
                    return "/";

                case ';':
                    return ";";

                case '<':
                    num = offset + 1;
                    if (s.Length <= num)
                    {
                        break;
                    }
                    if (s[num] != '>')
                    {
                        if (s[num] == '=')
                        {
                            return "<=";
                        }
                        break;
                    }
                    return "<>";

                case '=':
                    return "=";

                case '>':
                    num = offset + 1;
                    if ((s.Length <= num) || (s[num] != '='))
                    {
                        return ">";
                    }
                    return ">=";

                default:
                    return null;
            }
            return "<";
        }

        /// <summary>
        /// Determines whether the string begins with an error value
        /// at the specified offset.
        /// </summary>
        private string StartsWithError(string s, int offset)
        {
            return errorsSurfixTree.GetFirstMatch(s, offset);
        }

        /// <summary>
        /// Returns the current token.
        /// </summary>
        public string Current
        {
            get
            {
                if (this.currentLength == 0)
                {
                    throw new InvalidOperationException();
                }
                return this.text.Substring(this.currentStart, this.currentLength);
            }
        }

        /// <summary>
        /// Returns the current token.
        /// </summary>
        public int CurrentOffset
        {
            get
            {
                if (this.currentLength == 0)
                {
                    throw new InvalidOperationException();
                }
                return this.currentStart;
            }
        }
    }
}

