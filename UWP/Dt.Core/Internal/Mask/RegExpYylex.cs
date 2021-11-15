#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.IO;
#endregion

namespace Dt.Core.Mask
{
    internal class RegExpYylex : IYyInput
    {
        #region 静态内容
        static char ReadCharCharCode(RegExpPokeableReader reader, int digitsWanted)
        {
            string s = string.Empty;
            for (int i = 0; i < digitsWanted; i++)
            {
                int num2 = reader.Read();
                if (num2 == -1)
                {
                    break;
                }
                s = s + ((char)num2);
            }
            return (char)int.Parse(s, NumberStyles.HexNumber);
        }

        static string ReadUnicodeCategoryName(RegExpPokeableReader reader)
        {
            if (reader.Read() != 0x7b)
            {
                throw new ArgumentException(@"Incorrect mask: '{' expected after '\p' or '\P'");
            }
            string str = string.Empty;
            while (true)
            {
                int num = reader.Read();
                switch (num)
                {
                    case -1:
                        throw new ArgumentException(@"Incorrect mask: '}' expected after '\p{unicode_category_name' or '\P{unicode_category_name'");

                    case 0x7d:
                        return str;
                }
                str = str + ((char)num);
            }
        }
        #endregion

        #region 成员变量
        bool _inBracketExpression;
        bool _inBracketExpressionStart;
        bool _inDupCount;
        readonly CultureInfo _parseCulture;
        readonly RegExpPokeableReader _reader;
        int _remembered_token;
        object _remembered_value;
        #endregion

        #region 构造方法
        public RegExpYylex(TextReader reader, CultureInfo parseCulture)
        {
            this._reader = new RegExpPokeableReader(reader);
            this._parseCulture = parseCulture;
        }
        #endregion

        #region 外部方法
        public bool advance()
        {
            this._remembered_value = this._reader.Read();
            if (((int) this._remembered_value) == -1)
            {
                return false;
            }
            this._remembered_value = (char) ((int) this._remembered_value);
            bool flag = false;
            this._remembered_token = 0x101;
            switch (((char) this._remembered_value))
            {
                case '(':
                case ')':
                case '*':
                case '+':
                case '.':
                case '?':
                case '|':
                    if (!this._inBracketExpression)
                    {
                        this._remembered_token = (char) this._remembered_value;
                    }
                    break;

                case ',':
                    if (this._inDupCount)
                    {
                        this._remembered_token = (char) this._remembered_value;
                    }
                    break;

                case '-':
                    if ((this._inBracketExpression && !this._inBracketExpressionStart) && (this._reader.Peek() != 0x5d))
                    {
                        this._remembered_token = (char) this._remembered_value;
                    }
                    break;

                case '[':
                    if (!this._inBracketExpression)
                    {
                        this._remembered_token = (char) this._remembered_value;
                        this._inBracketExpression = true;
                        flag = true;
                    }
                    break;

                case '\\':
                    this._remembered_value = this._reader.Read();
                    if (((int) this._remembered_value) == -1)
                    {
                        throw new ArgumentException(@"Incorrect mask: character expected after '\'");
                    }
                    this._remembered_value = (char) ((int) this._remembered_value);
                    switch (((char) this._remembered_value))
                    {
                        case 'p':
                            this._remembered_token = 0x109;
                            this._remembered_value = ReadUnicodeCategoryName(this._reader);
                            break;

                        case 's':
                            this._remembered_token = 0x105;
                            break;

                        case 'u':
                            this._remembered_value = ReadCharCharCode(this._reader, 4);
                            break;

                        case 'w':
                            this._remembered_token = 0x107;
                            break;

                        case 'x':
                            this._remembered_value = ReadCharCharCode(this._reader, 2);
                            break;

                        case 'd':
                            this._remembered_token = 0x103;
                            break;

                        case 'W':
                            this._remembered_token = 0x108;
                            break;

                        case 'P':
                            this._remembered_token = 0x10a;
                            this._remembered_value = ReadUnicodeCategoryName(this._reader);
                            break;

                        case 'R':
                            this._remembered_value = this._reader.Read();
                            if (((int) this._remembered_value) == -1)
                            {
                                throw new ArgumentException(@"Incorrect mask: character expected after '\R'");
                            }
                            this._remembered_value = (char) ((int) this._remembered_value);
                            switch (((char) this._remembered_value))
                            {
                                case '.':
                                    this._reader.Poke(RegExpNamedMasks.NumberDecimalSeparator);
                                    return this.advance();

                                case '/':
                                    this._reader.Poke(RegExpNamedMasks.DateSeparator);
                                    return this.advance();

                                case ':':
                                    this._reader.Poke(RegExpNamedMasks.TimeSeparator);
                                    return this.advance();

                                case '{':
                                {
                                    int num;
                                    string maskName = string.Empty;
                                Label_0318:
                                    num = this._reader.Read();
                                    if (num == -1)
                                    {
                                        throw new ArgumentException(@"Incorrect mask: '}' expected after '\R{pattern_name'");
                                    }
                                    if (((ushort) num) != 0x7d)
                                    {
                                        maskName = maskName + ((char) num);
                                        goto Label_0318;
                                    }
                                    this._reader.Poke(RegExpNamedMasks.GetNamedMask(maskName, this._parseCulture));
                                    return this.advance();
                                }
                            }
                            throw new ArgumentException(@"Incorrect mask: only '.', ':', '/' and '{' are allowed after '\R'");

                        case 'S':
                            this._remembered_token = 0x106;
                            break;

                        case 'D':
                            this._remembered_token = 260;
                            break;
                    }
                    break;

                case ']':
                    if (this._inBracketExpression)
                    {
                        this._remembered_token = (char) this._remembered_value;
                        this._inBracketExpression = false;
                    }
                    break;

                case '^':
                    if (this._inBracketExpressionStart)
                    {
                        this._remembered_token = (char) this._remembered_value;
                        flag = true;
                    }
                    break;

                case '{':
                    if (!this._inBracketExpression)
                    {
                        this._remembered_token = (char) this._remembered_value;
                        this._inDupCount = true;
                    }
                    break;

                case '}':
                    if (this._inDupCount)
                    {
                        this._inDupCount = false;
                        this._remembered_token = (char) this._remembered_value;
                    }
                    break;

                default:
                    if ((((char) this._remembered_value) >= '0') && (((char) this._remembered_value) <= '9'))
                    {
                        this._remembered_token = 0x102;
                    }
                    break;
            }
            this._inBracketExpressionStart = flag;
            return true;
        }
        
        public int token()
        {
            return this._remembered_token;
        }

        public object value()
        {
            return this._remembered_value;
        }
        #endregion
    }

    internal interface IYyInput
    {
        bool advance();
        int token();
        object value();
    }
}

