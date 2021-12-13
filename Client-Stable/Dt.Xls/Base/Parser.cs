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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
#endregion

namespace Dt.Xls
{
    internal class Parser
    {
        private static Dictionary<string, ExcelCalcError> _errorTable = new Dictionary<string, ExcelCalcError>();
        private static Dictionary<string, BinaryOperatorInfo> _infixOperators = new Dictionary<string, BinaryOperatorInfo>();
        private static object commaPlaceHolder = new object();
        private static ExcelCalcError[] errors = new ExcelCalcError[] { ExcelCalcError.DivideByZero, ExcelCalcError.WrongFunctionOrRangeName, ExcelCalcError.ArgumentOrFunctionNotAvailable, ExcelCalcError.InterSectionOfTwoCellRangesIsEmpty, ExcelCalcError.ValueRangeOverflow, ExcelCalcError.IllegalOrDeletedCellReference, ExcelCalcError.WrongTypeOfOperand };
        private const int excel12ColumnCount = 0x4000;
        private const int excel12RowCount = 0x100000;
        private const int excel12WrapCount = 0x10;
        private static HashSet<string> functionNameSets = new HashSet<string>();
        private static FieldInfo[] functions = Enumerable.ToArray<FieldInfo>(IntrospectionExtensions.GetTypeInfo((Type) typeof(Xlf)).DeclaredFields);
        private static object leftParenthesesPlaceHolder = new object();
        private static UnaryOperatorInfo[] postfixOperators = new UnaryOperatorInfo[] { UnaryOperatorInfo.PercentOperator };
        private static Dictionary<OperatorInfo, int> PrecedenceTable = new Dictionary<OperatorInfo, int>();
        private static UnaryOperatorInfo[] prefixOperators = new UnaryOperatorInfo[] { UnaryOperatorInfo.NegateOperator, UnaryOperatorInfo.PlusOperator };
        internal static List<Tuple<int, int>> selections = null;
        internal static List<string> sheetNames = null;

        static Parser()
        {
            InitErrorTable();
            InitPrecedenceTable();
            InitFunctionNameSets();
            InitInfixOperators();
        }

        private static string AppendA1Letter(int coord, bool relative, int baseCoord)
        {
            StringBuilder builder = new StringBuilder();
            if (relative)
            {
                coord += baseCoord;
            }
            else
            {
                builder.Append("$");
            }
            if (coord < 0)
            {
                coord = 0x4000 + coord;
            }
            if (coord >= 0x4000)
            {
                coord -= 0x4000;
            }
            if (coord >= 0)
            {
                int length = builder.Length;
                coord++;
                while (coord > 0)
                {
                    builder.Insert(length, ((char) (0x41 + ((coord - 1) % 0x1a))));
                    coord = (coord - 1) / 0x1a;
                }
            }
            else
            {
                builder.Append("?");
            }
            return builder.ToString();
        }

        private static string AppendA1Number(int coord, bool relative, int baseCoord)
        {
            StringBuilder builder = new StringBuilder();
            if (relative)
            {
                coord += baseCoord;
            }
            else
            {
                builder.Append("$");
            }
            if (coord < 0)
            {
                coord = 0x100000 + coord;
            }
            if (coord >= 0x100000)
            {
                coord -= 0x100000;
            }
            if (coord >= 0)
            {
                builder.Append((int) (coord + 1));
            }
            else
            {
                builder.Append("?");
            }
            return builder.ToString();
        }

        private static string AppendExternalName(object source)
        {
            StringBuilder builder = new StringBuilder();
            string str = (string) (source as string);
            if ((str != null) && (str.Length > 0))
            {
                bool flag = !char.IsLetter(str[0]) && (str[0] != '_');
                if (!flag)
                {
                    for (int i = 1; !flag && (i < str.Length); i++)
                    {
                        flag = !char.IsLetterOrDigit(str[i]) && (str[i] != '_');
                    }
                }
                if (_errorTable.ContainsKey(str.ToUpperInvariant()))
                {
                    flag = false;
                }
                if (flag)
                {
                    builder.Append("'");
                    builder.Append(str.Replace("'", "''"));
                    builder.Append("'");
                }
                else
                {
                    builder.Append(str);
                }
            }
            return builder.ToString();
        }

        private static string AppendR1C1Number(string prefix, int coord, bool relative)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(prefix);
            if (relative)
            {
                if (coord != 0)
                {
                    builder.Append("[");
                    builder.Append(((int) coord).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat));
                    builder.Append("]");
                }
            }
            else
            {
                builder.Append((int) (coord + 1));
            }
            return builder.ToString();
        }

        private static int GetPrecedenceLevel(OperatorInfo oper)
        {
            int num = -1;
            PrecedenceTable.TryGetValue(oper, out num);
            return num;
        }

        private static void InitErrorTable()
        {
            _errorTable.Add("#DIV/0!", ExcelCalcError.DivideByZero);
            _errorTable.Add("#NAME?", ExcelCalcError.WrongFunctionOrRangeName);
            _errorTable.Add("#N/A", ExcelCalcError.ArgumentOrFunctionNotAvailable);
            _errorTable.Add("#NULL!", ExcelCalcError.InterSectionOfTwoCellRangesIsEmpty);
            _errorTable.Add("#NUM!", ExcelCalcError.ValueRangeOverflow);
            _errorTable.Add("#REF!", ExcelCalcError.IllegalOrDeletedCellReference);
            _errorTable.Add("#VALUE!", ExcelCalcError.WrongTypeOfOperand);
        }

        private static void InitFunctionNameSets()
        {
            foreach (FieldInfo info in functions)
            {
                functionNameSets.Add(info.Name.ToUpperInvariant());
            }
        }

        private static void InitInfixOperators()
        {
            _infixOperators.Add(BinaryOperatorInfo.AddOperator.Name, BinaryOperatorInfo.AddOperator);
            _infixOperators.Add(BinaryOperatorInfo.SubtractOperator.Name, BinaryOperatorInfo.SubtractOperator);
            _infixOperators.Add(BinaryOperatorInfo.MultiplyOperator.Name, BinaryOperatorInfo.MultiplyOperator);
            _infixOperators.Add(BinaryOperatorInfo.DivideOperator.Name, BinaryOperatorInfo.DivideOperator);
            _infixOperators.Add(BinaryOperatorInfo.ExponentOperator.Name, BinaryOperatorInfo.ExponentOperator);
            _infixOperators.Add(BinaryOperatorInfo.ConcatenateOperator.Name, BinaryOperatorInfo.ConcatenateOperator);
            _infixOperators.Add(BinaryOperatorInfo.EqualOperator.Name, BinaryOperatorInfo.EqualOperator);
            _infixOperators.Add(BinaryOperatorInfo.NotEqualOperator.Name, BinaryOperatorInfo.NotEqualOperator);
            _infixOperators.Add(BinaryOperatorInfo.LessThanOperator.Name, BinaryOperatorInfo.LessThanOperator);
            _infixOperators.Add(BinaryOperatorInfo.GreaterThanOperator.Name, BinaryOperatorInfo.GreaterThanOperator);
            _infixOperators.Add(BinaryOperatorInfo.LessThanOrEqualOperator.Name, BinaryOperatorInfo.LessThanOrEqualOperator);
            _infixOperators.Add(BinaryOperatorInfo.GreaterThanOrEqualOperator.Name, BinaryOperatorInfo.GreaterThanOrEqualOperator);
        }

        private static void InitPrecedenceTable()
        {
            PrecedenceTable.Add(BinaryOperatorInfo.EqualOperator, 0);
            PrecedenceTable.Add(BinaryOperatorInfo.NotEqualOperator, 0);
            PrecedenceTable.Add(BinaryOperatorInfo.LessThanOperator, 0);
            PrecedenceTable.Add(BinaryOperatorInfo.GreaterThanOperator, 0);
            PrecedenceTable.Add(BinaryOperatorInfo.LessThanOrEqualOperator, 0);
            PrecedenceTable.Add(BinaryOperatorInfo.GreaterThanOrEqualOperator, 0);
            PrecedenceTable.Add(BinaryOperatorInfo.ConcatenateOperator, 1);
            PrecedenceTable.Add(BinaryOperatorInfo.AddOperator, 2);
            PrecedenceTable.Add(BinaryOperatorInfo.SubtractOperator, 2);
            PrecedenceTable.Add(BinaryOperatorInfo.MultiplyOperator, 3);
            PrecedenceTable.Add(BinaryOperatorInfo.DivideOperator, 3);
            PrecedenceTable.Add(BinaryOperatorInfo.ExponentOperator, 4);
            PrecedenceTable.Add(UnaryOperatorInfo.PercentOperator, 5);
            PrecedenceTable.Add(UnaryOperatorInfo.NegateOperator, 6);
            PrecedenceTable.Add(UnaryOperatorInfo.PlusOperator, 6);
        }

        private static bool IsDouble(string s)
        {
            double num;
            return double.TryParse(s, out num);
        }

        private static bool IsFunction(string s)
        {
            string str = s.ToUpperInvariant();
            if (functionNameSets.Contains(str))
            {
                return true;
            }
            if (str == "ERROR.TYPE")
            {
                for (int i = 0; i < functions.Length; i++)
                {
                    if (functions[i].Name == "ERRORTYPE")
                    {
                        return true;
                    }
                }
            }
            return FormulaProcess.externalFunctionList.Contains(s);
        }

        private static bool IsName(string s)
        {
            if (!((s.Length > 0) && IsValidFirstChar(s[0])))
            {
                return false;
            }
            for (int i = 1; i < s.Length; i++)
            {
                if (!IsValidRemainingChar(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsString(string s)
        {
            if (((s.Length < 2) || (s[0] != '"')) || (s[s.Length - 1] != '"'))
            {
                return false;
            }
            for (int i = 1; i < (s.Length - 1); i++)
            {
                if (s[i] == '"')
                {
                    if ((i >= (s.Length - 2)) || (s[i + 1] != '"'))
                    {
                        return false;
                    }
                    i++;
                }
            }
            return true;
        }

        private static bool IsValidFirstChar(char c)
        {
            if (!char.IsLetter(c))
            {
                return (c == '_');
            }
            return true;
        }

        private static bool IsValidRemainingChar(char c)
        {
            if (!char.IsLetterOrDigit(c) && (c != '_'))
            {
                return (c == '.');
            }
            return true;
        }

        private static ExcelCalcError LookupError(string s)
        {
            if (_errorTable.ContainsKey(s))
            {
                return _errorTable[s];
            }
            return null;
        }

        private static BinaryOperatorInfo LookupInfixOperator(string s)
        {
            BinaryOperatorInfo info = null;
            _infixOperators.TryGetValue(s, out info);
            return info;
        }

        private static UnaryOperatorInfo LookupPostfixOperator(string s)
        {
            for (int i = 0; i < postfixOperators.Length; i++)
            {
                if (s == postfixOperators[i].Name)
                {
                    return postfixOperators[i];
                }
            }
            return null;
        }

        private static UnaryOperatorInfo LookupPrefixOperator(string s)
        {
            for (int i = 0; i < prefixOperators.Length; i++)
            {
                if (s == prefixOperators[i].Name)
                {
                    return prefixOperators[i];
                }
            }
            return null;
        }

        public static ParsedToken Parse(string text, int baseRow, int baseColumn, bool useR1C1, LinkTable linkTable)
        {
            if ((text == null) || (text.Length == 0))
            {
                return null;
            }
            Tokenizer tokenizer = new Tokenizer(text);
            Stack<object> operStack = new Stack<object>();
            Stack<object> exprStack = new Stack<object>();
            bool flag = false;
            Token nextToken = tokenizer.GetNextToken();
            if ((nextToken != null) && (nextToken.Text == ":"))
            {
                nextToken = tokenizer.GetNextToken();
            }
            Token token2 = null;
            while (nextToken != null)
            {
                ExcelCalcError error;
                UnaryOperatorInfo info;
                BinaryOperatorInfo info2;
                string str;
                object[,] objArray;
                ExternalRangeExpression expression2;
                ExternalRangeExpression2 expression3;
                switch (nextToken.Category)
                {
                    case ToekCategory.Function:
                    {
                        if (!IsFunction(nextToken.Text))
                        {
                            break;
                        }
                        ParsedToken token3 = new ParsedToken {
                            Token = nextToken.Text,
                            TokenType = TokenType.Function
                        };
                        operStack.Push(token3);
                        flag = false;
                        goto Label_148E;
                    }
                    case ToekCategory.Error:
                    {
                        error = LookupError(nextToken.Text);
                        ParsedToken token72 = new ParsedToken {
                            Token = nextToken.Text,
                            TokenType = TokenType.Error,
                            Value = error
                        };
                        exprStack.Push(token72);
                        flag = true;
                        goto Label_148E;
                    }
                    case ToekCategory.String:
                    {
                        ParsedToken token73 = new ParsedToken {
                            Token = nextToken.Text,
                            TokenType = TokenType.String,
                            Value = nextToken.Text
                        };
                        exprStack.Push(token73);
                        flag = true;
                        goto Label_148E;
                    }
                    case ToekCategory.Delimiters:
                        str = nextToken.Text;
                        if (flag || ((info = LookupPrefixOperator(str)) == null))
                        {
                            goto Label_01A3;
                        }
                        operStack.Push(info);
                        flag = false;
                        goto Label_148E;

                    case ToekCategory.Number:
                    {
                        ParsedToken token74 = new ParsedToken {
                            Token = nextToken.Text,
                            TokenType = TokenType.Double,
                            Value = (double) double.Parse(nextToken.Text, (IFormatProvider) CultureInfo.InvariantCulture)
                        };
                        exprStack.Push(token74);
                        flag = true;
                        goto Label_148E;
                    }
                    case ToekCategory.Unknown:
                    {
                        str = nextToken.Text;
                        if (CultureInfo.InvariantCulture.CompareInfo.Compare(str, "FALSE", (CompareOptions) CompareOptions.IgnoreCase) != 0)
                        {
                            goto Label_094E;
                        }
                        ParsedToken token21 = new ParsedToken {
                            Token = str,
                            TokenType = TokenType.Boolean,
                            Value = false
                        };
                        exprStack.Push(token21);
                        flag = true;
                        goto Label_148E;
                    }
                    default:
                        goto Label_148E;
                }
                if (IsName(nextToken.Text))
                {
                    ParsedToken token4 = new ParsedToken {
                        Token = nextToken.Text,
                        TokenType = TokenType.UndefinedFunction
                    };
                    operStack.Push(token4);
                    flag = false;
                    goto Label_148E;
                }
                throw new ExcelException(text, ExcelExceptionCode.ParseException);
            Label_01A3:
                if (flag && ((info2 = LookupInfixOperator(str)) != null))
                {
                    TransferOperators(exprStack, operStack, GetPrecedenceLevel(info2));
                    operStack.Push(info2);
                    flag = false;
                    goto Label_148E;
                }
                if (flag && ((info = LookupPostfixOperator(str)) != null))
                {
                    TransferOperators(exprStack, operStack, GetPrecedenceLevel(info));
                    ParsedToken token5 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.UnaryOperation,
                        Value = (ParsedToken) exprStack.Pop()
                    };
                    exprStack.Push(token5);
                    flag = true;
                    goto Label_148E;
                }
                if (str == "(")
                {
                    if (flag)
                    {
                        throw new ExcelException(text, ExcelExceptionCode.ParseException);
                    }
                    operStack.Push(leftParenthesesPlaceHolder);
                    flag = false;
                    goto Label_148E;
                }
                if (CultureInfo.InvariantCulture.CompareInfo.Compare(str, ",", (CompareOptions) CompareOptions.None) == 0)
                {
                    if (!flag)
                    {
                        ParsedToken token6 = new ParsedToken {
                            Token = "",
                            TokenType = TokenType.MissArg,
                            Value = ""
                        };
                        exprStack.Push(token6);
                    }
                    TransferOperators(exprStack, operStack, 0);
                    operStack.Push(commaPlaceHolder);
                    flag = false;
                    goto Label_148E;
                }
                if (str == ")")
                {
                    if ((token2 != null) && (token2.Text == ","))
                    {
                        flag = true;
                        ParsedToken token7 = new ParsedToken {
                            Token = "",
                            TokenType = TokenType.MissArg,
                            Value = ""
                        };
                        exprStack.Push(token7);
                    }
                    int num = 0;
                    TransferOperators(exprStack, operStack, 0);
                    while ((operStack.Count > 0) && object.ReferenceEquals(operStack.Peek(), commaPlaceHolder))
                    {
                        operStack.Pop();
                        num++;
                    }
                    if ((operStack.Count > 0) && object.ReferenceEquals(operStack.Peek(), leftParenthesesPlaceHolder))
                    {
                        if (num > 0)
                        {
                            throw new ExcelException(text, ExcelExceptionCode.ParseException);
                        }
                        operStack.Pop();
                        ParsedToken token8 = (ParsedToken) exprStack.Pop();
                        ParsedToken token9 = new ParsedToken {
                            Token = str,
                            TokenType = TokenType.Parentheses,
                            Value = token8
                        };
                        exprStack.Push(token9);
                    }
                    else
                    {
                        if (((operStack.Count <= 0) || !(operStack.Peek() is ParsedToken)) || ((((ParsedToken) operStack.Peek()).TokenType != TokenType.Function) && (((ParsedToken) operStack.Peek()).TokenType != TokenType.UndefinedFunction)))
                        {
                            throw new ExcelException(text, ExcelExceptionCode.ParseException);
                        }
                        int num2 = num + (flag ? 1 : 0);
                        ParsedToken token10 = (ParsedToken) operStack.Pop();
                        ParsedToken[] tokenArray = new ParsedToken[num2];
                        for (int j = num2 - 1; j >= 0; j--)
                        {
                            tokenArray[j] = (ParsedToken) exprStack.Pop();
                        }
                        if (token10.TokenType == TokenType.UndefinedFunction)
                        {
                            ParsedToken token11 = new ParsedToken {
                                Token = token10.Token,
                                TokenType = TokenType.Function,
                                Value = tokenArray
                            };
                            exprStack.Push(token11);
                        }
                        else
                        {
                            ParsedToken token12 = new ParsedToken {
                                Token = token10.Token,
                                TokenType = TokenType.Function,
                                Value = tokenArray
                            };
                            exprStack.Push(token12);
                        }
                    }
                    flag = true;
                    goto Label_148E;
                }
                if (str != "{")
                {
                    goto Label_148E;
                }
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
            Label_04D9:
                nextToken = tokenizer.GetNextToken();
                if (nextToken != null)
                {
                    bool flag2;
                    if (flag2 = CultureInfo.InvariantCulture.CompareInfo.Compare(nextToken.Text, "-", (CompareOptions) CompareOptions.None) == 0)
                    {
                        nextToken = tokenizer.GetNextToken();
                        if (nextToken == null)
                        {
                            goto Label_0837;
                        }
                    }
                    if (IsDouble(nextToken.Text))
                    {
                        ParsedToken token13 = new ParsedToken {
                            Token = nextToken.Text,
                            TokenType = TokenType.Double,
                            Value = (double) double.Parse(nextToken.Text, (IFormatProvider) CultureInfo.InvariantCulture)
                        };
                        operStack.Push(token13);
                    }
                    else if (IsString(nextToken.Text))
                    {
                        ParsedToken token14 = new ParsedToken {
                            Token = nextToken.Text,
                            TokenType = TokenType.String,
                            Value = nextToken
                        };
                        operStack.Push(token14);
                    }
                    else
                    {
                        error = LookupError(nextToken.Text);
                        if (error != null)
                        {
                            ParsedToken token15 = new ParsedToken {
                                Token = nextToken.Text,
                                TokenType = TokenType.Error,
                                Value = error
                            };
                            operStack.Push(token15);
                        }
                        else if (CultureInfo.InvariantCulture.CompareInfo.Compare(nextToken.Text, "FALSE", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                        {
                            ParsedToken token16 = new ParsedToken {
                                Token = nextToken.Text,
                                TokenType = TokenType.Boolean,
                                Value = false
                            };
                            operStack.Push(token16);
                        }
                        else if (CultureInfo.InvariantCulture.CompareInfo.Compare(nextToken.Text, "TRUE", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                        {
                            ParsedToken token17 = new ParsedToken {
                                Token = nextToken.Text,
                                TokenType = TokenType.Boolean,
                                Value = true
                            };
                            operStack.Push(token17);
                        }
                        else
                        {
                            if (!IsString("\"" + nextToken.Text + "\""))
                            {
                                throw new ExcelException(text, ExcelExceptionCode.ParseException);
                            }
                            ParsedToken token18 = new ParsedToken {
                                Token = "\"" + nextToken.Text + "\"",
                                TokenType = TokenType.String,
                                Value = nextToken
                            };
                            operStack.Push(token18);
                        }
                    }
                    if (flag2)
                    {
                        if (!(operStack.Peek() is ParsedToken) || (((ParsedToken) operStack.Peek()).TokenType != TokenType.Double))
                        {
                            throw new ExcelException(text, ExcelExceptionCode.ParseException);
                        }
                        ParsedToken token19 = (ParsedToken) operStack.Pop();
                        token19.Value = -1.0 * ((double) token19.Value);
                        operStack.Push(token19);
                    }
                    nextToken = tokenizer.GetNextToken();
                    if (nextToken == null)
                    {
                        throw new ExcelException(text, ExcelExceptionCode.ParseException);
                    }
                    if (CultureInfo.InvariantCulture.CompareInfo.Compare(nextToken.Text, ",", (CompareOptions) CompareOptions.None) == 0)
                    {
                        num6++;
                        num5++;
                        goto Label_04D9;
                    }
                    if (CultureInfo.InvariantCulture.CompareInfo.Compare(nextToken.Text, ";", (CompareOptions) CompareOptions.None) == 0)
                    {
                        num6++;
                        num5++;
                        num4++;
                        if (num6 != (num4 * num5))
                        {
                            throw new ExcelException(text, ExcelExceptionCode.ParseException);
                        }
                        num5 = 0;
                        goto Label_04D9;
                    }
                    if (CultureInfo.InvariantCulture.CompareInfo.Compare(nextToken.Text, "}", (CompareOptions) CompareOptions.None) != 0)
                    {
                        throw new ExcelException(text, ExcelExceptionCode.ParseException);
                    }
                    num6++;
                    num5++;
                    num4++;
                    if (num6 != (num4 * num5))
                    {
                        throw new ExcelException(text, ExcelExceptionCode.ParseException);
                    }
                }
            Label_0837:
                objArray = new object[num4, num5];
                for (int i = num4 - 1; i >= 0; i--)
                {
                    for (int k = num5 - 1; k >= 0; k--)
                    {
                        objArray[i, k] = operStack.Pop();
                    }
                }
                ParsedToken token20 = new ParsedToken {
                    Token = "",
                    TokenType = TokenType.Array,
                    Value = objArray
                };
                exprStack.Push(token20);
                flag = true;
                goto Label_148E;
            Label_094E:
                if (CultureInfo.InvariantCulture.CompareInfo.Compare(str, "TRUE", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                {
                    ParsedToken token22 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.Boolean,
                        Value = true
                    };
                    exprStack.Push(token22);
                    flag = true;
                    goto Label_148E;
                }
                if (((((str[0] != '$') && (str[0] != '_')) && ((str[0] != '\'') && !char.IsLetter(str[0]))) && (!char.IsNumber(str[0]) || (str.IndexOf(":") == -1))) && (str.IndexOf('!') == -1))
                {
                    throw new ExcelException(text, ExcelExceptionCode.ParseException);
                }
                if (IsName(str) && linkTable.HasDefindName(str, -1))
                {
                    ParsedToken token23 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.Name,
                        Value = str
                    };
                    exprStack.Push(token23);
                    flag = true;
                    goto Label_148E;
                }
                if ((str.IndexOf("!") != -1) && ((expression2 = ParseExternalReference(str, baseRow, baseColumn, useR1C1, linkTable)) != null))
                {
                    ParsedToken token24 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.ExReference,
                        Value = expression2
                    };
                    exprStack.Push(token24);
                    flag = true;
                    goto Label_148E;
                }
                if ((str.IndexOf("!") != -1) && ((expression3 = ParseExternalReference2(str, baseRow, baseColumn, useR1C1, linkTable)) != null))
                {
                    ParsedToken token25 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.ExReference,
                        Value = expression3
                    };
                    exprStack.Push(token25);
                    flag = true;
                    goto Label_148E;
                }
                RangeExpression expression = ParseReference(str, baseRow, baseColumn, useR1C1);
                if (expression != null)
                {
                    if (((exprStack.Count != 0) && (((ParsedToken) exprStack.Peek()).TokenType == TokenType.Error)) && (operStack.Count == 0))
                    {
                        ParsedToken token26 = (ParsedToken) exprStack.Pop();
                        ExternalRangeExpression expression4 = new ExternalRangeExpression(token26.Token, expression.Row, expression.Column, expression.RowCount, expression.ColumnCount);
                        ParsedToken token27 = new ParsedToken {
                            Token = token26.Token + str,
                            TokenType = TokenType.ExReference,
                            Value = expression4
                        };
                        exprStack.Push(token27);
                    }
                    else if (((exprStack.Count != 0) && (((ParsedToken) exprStack.Peek()).TokenType == TokenType.Error)) && (operStack.Count > 0))
                    {
                        if (operStack.Peek() is BinaryOperatorInfo)
                        {
                            ParsedToken token30 = new ParsedToken {
                                Token = str,
                                TokenType = TokenType.Reference,
                                Value = expression
                            };
                            exprStack.Push(token30);
                        }
                        else
                        {
                            ParsedToken token28 = (ParsedToken) exprStack.Pop();
                            ExternalRangeExpression expression5 = new ExternalRangeExpression(token28.Token, expression.Row, expression.Column, expression.RowCount, expression.ColumnCount);
                            ParsedToken token29 = new ParsedToken {
                                Token = token28.Token + str,
                                TokenType = TokenType.ExReference,
                                Value = expression5
                            };
                            exprStack.Push(token29);
                        }
                    }
                    else
                    {
                        ParsedToken token31 = new ParsedToken {
                            Token = str,
                            TokenType = TokenType.Reference,
                            Value = expression
                        };
                        exprStack.Push(token31);
                    }
                    flag = true;
                    goto Label_148E;
                }
                RangeExprssion2 exprssion = ParseReference2(str, baseRow, baseColumn, useR1C1);
                if (exprssion != null)
                {
                    ParsedToken token32 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.Reference,
                        Value = exprssion
                    };
                    exprStack.Push(token32);
                    flag = true;
                    goto Label_148E;
                }
                if (IsName(str))
                {
                    ParsedToken token33 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.Name,
                        Value = str
                    };
                    exprStack.Push(token33);
                    flag = true;
                    goto Label_148E;
                }
                if (str.IndexOf("!") < 0)
                {
                    goto Label_1044;
                }
                string name = str.Substring(str.IndexOf("!") + 1);
                string str3 = RemoveQuotasAndSquareBrackets(str.Substring(0, str.IndexOf("!")));
                if ((linkTable.ExternalNamedCellRanges == null) || !linkTable.ExternalNamedCellRanges.ContainsKey(str3))
                {
                    goto Label_0E32;
                }
                List<IName> list = linkTable.ExternalNamedCellRanges[str3];
                bool flag3 = false;
                using (List<IName>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Name == name)
                        {
                            ParsedToken token34 = new ParsedToken {
                                Token = str,
                                TokenType = TokenType.Name,
                                Value = str
                            };
                            exprStack.Push(token34);
                            flag3 = true;
                            flag = true;
                            goto Label_0DF7;
                        }
                    }
                }
            Label_0DF7:
                if (!flag3)
                {
                    ParsedToken token35 = new ParsedToken {
                        Token = "#REF!",
                        TokenType = TokenType.Error,
                        Value = ExcelCalcError.IllegalOrDeletedCellReference
                    };
                    exprStack.Push(token35);
                }
                goto Label_148E;
            Label_0E32:
                if (linkTable.InternalSheetNames.Contains(str3))
                {
                    if (name.IndexOf(":") != -1)
                    {
                        string[] strArray = str.Split(new char[] { ':' });
                        ExternalRangeExpression expression6 = ParseExternalReference(strArray[0], baseRow, baseColumn, useR1C1, linkTable);
                        ExternalRangeExpression expression7 = ParseExternalReference(strArray[1], baseRow, baseColumn, useR1C1, linkTable);
                        if (((strArray.Length != 2) || (expression6 == null)) || (expression7 == null))
                        {
                            nextToken = tokenizer.PeekNextToken();
                            if (nextToken != null)
                            {
                                if (nextToken.Text == "(")
                                {
                                    throw new NotSupportedException(text);
                                }
                            }
                            else
                            {
                                ParsedToken token39 = new ParsedToken {
                                    Token = str,
                                    TokenType = TokenType.ExReference,
                                    Value = str
                                };
                                exprStack.Push(token39);
                                flag = true;
                            }
                        }
                        else
                        {
                            SubExternalRangeExpression expression8 = new SubExternalRangeExpression(":", expression6, expression7);
                            ParsedToken token38 = new ParsedToken {
                                Token = str,
                                TokenType = TokenType.BinaryReferenceOperation,
                                Value = expression8
                            };
                            exprStack.Push(token38);
                            flag = true;
                        }
                    }
                    else if (linkTable.HasDefindName(name, linkTable.InternalSheetNames.IndexOf(str3)))
                    {
                        ParsedToken token36 = new ParsedToken {
                            Token = str,
                            TokenType = TokenType.Name,
                            Value = str
                        };
                        exprStack.Push(token36);
                        flag = true;
                    }
                    else if (LookupError(name) != null)
                    {
                        ExcelCalcError error2 = LookupError(name);
                        ParsedToken token37 = new ParsedToken {
                            Token = nextToken.Text,
                            TokenType = TokenType.Error,
                            Value = error2
                        };
                        exprStack.Push(token37);
                        flag = true;
                    }
                }
                else if (linkTable.HasDefindName(name, -1))
                {
                    ParsedToken token40 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.Name,
                        Value = str
                    };
                    exprStack.Push(token40);
                    flag = true;
                }
                else
                {
                    ParsedToken token41 = new ParsedToken {
                        Token = str,
                        TokenType = TokenType.ExReference,
                        Value = str
                    };
                    exprStack.Push(token41);
                    flag = true;
                }
                goto Label_148E;
            Label_1044:
                if (str.IndexOf(":") > 0)
                {
                    string[] strArray2 = str.Split(new char[] { ':' });
                    if (strArray2.Length != 2)
                    {
                        throw new NotSupportedException(text);
                    }
                    string str4 = strArray2[0];
                    string str5 = strArray2[1];
                    if (linkTable == null)
                    {
                        throw new NotSupportedException(text);
                    }
                    if (!linkTable.HasDefindName(str4, -1))
                    {
                        if (!linkTable.HasDefindName(str5, -1) || (ParseCellReference(str4, baseRow, baseColumn, useR1C1) == null))
                        {
                            throw new NotSupportedException(text);
                        }
                        ParsedToken token64 = new ParsedToken {
                            Token = str5,
                            TokenType = TokenType.Name,
                            Value = str5
                        };
                        ParsedToken token65 = new ParsedToken {
                            Token = str4,
                            TokenType = TokenType.Reference,
                            Value = ParseCellReference(str4, baseRow, baseColumn, useR1C1)
                        };
                        ParsedToken token66 = new ParsedToken {
                            Token = ":",
                            TokenType = TokenType.BinaryOperation,
                            Value = new ParsedToken[] { token65, token64 }
                        };
                        ParsedToken token70 = new ParsedToken {
                            Token = str,
                            TokenType = TokenType.SubExpression,
                            Value = token66
                        };
                        exprStack.Push(token70);
                        flag = true;
                    }
                    else if (!linkTable.HasDefindName(str5, -1))
                    {
                        if (ParseCellReference(str5, baseRow, baseColumn, useR1C1) == null)
                        {
                            if (str5 != "")
                            {
                                throw new NotSupportedException(text);
                            }
                            if (tokenizer.GetNextToken().Text != "(")
                            {
                                throw new NotSupportedException(text);
                            }
                            Token token56 = tokenizer.GetNextToken();
                            if (!linkTable.HasDefindName(token56.Text, -1) || (tokenizer.GetNextToken().Text != ")"))
                            {
                                throw new NotSupportedException(text);
                            }
                            ParsedToken token57 = new ParsedToken {
                                Token = str4,
                                TokenType = TokenType.Name,
                                Value = str4
                            };
                            ParsedToken token58 = new ParsedToken {
                                Token = str5,
                                TokenType = TokenType.Name,
                                Value = token56.Text
                            };
                            ParsedToken token59 = new ParsedToken {
                                Token = ":",
                                TokenType = TokenType.BinaryOperation,
                                Value = new ParsedToken[] { token57, token58 }
                            };
                            ParsedToken token63 = new ParsedToken {
                                Token = str,
                                TokenType = TokenType.SubExpression,
                                Value = token59
                            };
                            exprStack.Push(token63);
                            flag = true;
                        }
                        else
                        {
                            ParsedToken token49 = new ParsedToken {
                                Token = str4,
                                TokenType = TokenType.Name,
                                Value = str4
                            };
                            ParsedToken token50 = new ParsedToken {
                                Token = str5,
                                TokenType = TokenType.Reference,
                                Value = ParseCellReference(str5, baseRow, baseColumn, useR1C1)
                            };
                            ParsedToken token51 = new ParsedToken {
                                Token = ":",
                                TokenType = TokenType.BinaryOperation,
                                Value = new ParsedToken[] { token49, token50 }
                            };
                            ParsedToken token55 = new ParsedToken {
                                Token = str,
                                TokenType = TokenType.SubExpression,
                                Value = token51
                            };
                            exprStack.Push(token55);
                            flag = true;
                        }
                    }
                    else
                    {
                        ParsedToken token42 = new ParsedToken {
                            Token = str4,
                            TokenType = TokenType.Name,
                            Value = str4
                        };
                        ParsedToken token43 = new ParsedToken {
                            Token = str5,
                            TokenType = TokenType.Name,
                            Value = str5
                        };
                        ParsedToken token44 = new ParsedToken {
                            Token = ":",
                            TokenType = TokenType.BinaryOperation,
                            Value = new ParsedToken[] { token42, token43 }
                        };
                        ParsedToken token48 = new ParsedToken {
                            Token = str,
                            TokenType = TokenType.SubExpression,
                            Value = token44
                        };
                        exprStack.Push(token48);
                        flag = true;
                    }
                }
                else
                {
                    Token token71 = tokenizer.GetNextToken();
                    if ((token71 != null) && (token71.Text == "("))
                    {
                        throw new NotSupportedException(text);
                    }
                    throw new ExcelException(text, ExcelExceptionCode.ParseException);
                }
            Label_148E:
                token2 = nextToken;
                nextToken = tokenizer.GetNextToken();
                if ((nextToken != null) && (nextToken.Text == ":"))
                {
                    Token token75 = tokenizer.GetNextToken();
                    if ((token75 != null) && (token75.Text == "INDEX"))
                    {
                        operStack.Push(BinaryOperatorInfo.RangeOperator);
                    }
                    nextToken = token75;
                }
            }
            TransferOperators(exprStack, operStack, 0);
            if (!TransferOperators(exprStack, operStack))
            {
                throw new ExcelException(text, ExcelExceptionCode.ParseException);
            }
            if (operStack.Count != 0)
            {
                throw new ExcelException(text, ExcelExceptionCode.ParseException);
            }
            if (exprStack.Count == 2)
            {
                ParsedToken token76 = (ParsedToken) exprStack.Pop();
                if (((ParsedToken) exprStack.Peek()).TokenType != TokenType.Error)
                {
                    throw new ExcelException(text, ExcelExceptionCode.ParseException);
                }
                ParsedToken token77 = (ParsedToken) exprStack.Pop();
                ParsedToken token78 = new ParsedToken {
                    Token = token77.Token + token76.Token,
                    TokenType = TokenType.ExReference,
                    Value = new ExternalRangeExpression(null, 0, 0, 0, 0)
                };
                exprStack.Push(token78);
            }
            if (operStack.Count != 0)
            {
                throw new ExcelException(text, ExcelExceptionCode.ParseException);
            }
            return (ParsedToken) exprStack.Pop();
        }

        private static RangeExpression ParseCellReference(string s, int rowBase, int columnBase, bool useR1C1)
        {
            int num = 0;
            int row = 0;
            int column = 0;
            bool rowRelative = false;
            bool columnRelative = false;
            bool flag3 = false;
            bool flag4 = false;
            if (!useR1C1)
            {
                if (!(columnRelative = (num >= s.Length) || (s[num] != '$')))
                {
                    num++;
                }
                if (num < s.Length)
                {
                    char ch = s[num];
                    if (((ch < 'a') || (ch > 'z')) && ((ch < 'A') || (ch > 'Z')))
                    {
                        return null;
                    }
                    while ((('A' <= ch) && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z')))
                    {
                        int num4 = ch - 'a';
                        if (num4 < 0)
                        {
                            num4 = ch - 'A';
                        }
                        if (num4 < 0)
                        {
                            break;
                        }
                        column = ((0x1a * column) + num4) + 1;
                        if (column < 0)
                        {
                            return null;
                        }
                        num++;
                        if (num >= s.Length)
                        {
                            break;
                        }
                        ch = s[num];
                    }
                }
            }
            else
            {
                if ((num < s.Length) && ((s[num] == 'r') || (s[num] == 'R')))
                {
                    num++;
                }
                if (!(rowRelative = (num < s.Length) && (s[num] == '[')))
                {
                    if ((num < s.Length) && char.IsDigit(s[num]))
                    {
                        while ((num < s.Length) && char.IsDigit(s[num]))
                        {
                            row = (10 * row) + (s[num] - '0');
                            if (row < 0)
                            {
                                return null;
                            }
                            num++;
                        }
                        if (row <= 0)
                        {
                            return null;
                        }
                        row--;
                    }
                    else
                    {
                        rowRelative = true;
                    }
                }
                else
                {
                    num++;
                    if (flag3 = (num < s.Length) && (s[num] == '-'))
                    {
                        num++;
                    }
                    if ((num < s.Length) && char.IsDigit(s[num]))
                    {
                        while ((num < s.Length) && char.IsDigit(s[num]))
                        {
                            row = (10 * row) + (s[num] - '0');
                            if (row < 0)
                            {
                                return null;
                            }
                            num++;
                        }
                        if ((num < s.Length) && (s[num] == ']'))
                        {
                            num++;
                        }
                        if (flag3)
                        {
                            row = -row;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                if ((num < s.Length) && ((s[num] == 'c') || (s[num] == 'C')))
                {
                    num++;
                }
                if (!(columnRelative = (num < s.Length) && (s[num] == '[')))
                {
                    if ((num < s.Length) && char.IsDigit(s[num]))
                    {
                        while ((num < s.Length) && char.IsDigit(s[num]))
                        {
                            column = (10 * column) + (s[num] - '0');
                            if (column < 0)
                            {
                                return null;
                            }
                            num++;
                        }
                        if (column <= 0)
                        {
                            return null;
                        }
                        column--;
                    }
                    else
                    {
                        columnRelative = true;
                    }
                    goto Label_03AF;
                }
                num++;
                if (flag4 = (num < s.Length) && (s[num] == '-'))
                {
                    num++;
                }
                if ((num < s.Length) && char.IsDigit(s[num]))
                {
                    while ((num < s.Length) && char.IsDigit(s[num]))
                    {
                        column = (10 * column) + (s[num] - '0');
                        if (column < 0)
                        {
                            return null;
                        }
                        num++;
                    }
                    if ((num < s.Length) && (s[num] == ']'))
                    {
                        num++;
                    }
                    if (flag4)
                    {
                        column = -column;
                    }
                    goto Label_03AF;
                }
                return null;
            }
            if (column > 0)
            {
                column--;
                if (!(rowRelative = (num >= s.Length) || (s[num] != '$')))
                {
                    num++;
                }
                if ((num < s.Length) && char.IsDigit(s[num]))
                {
                    while ((num < s.Length) && char.IsDigit(s[num]))
                    {
                        row = (10 * row) + (s[num] - '0');
                        if (row < 0)
                        {
                            return null;
                        }
                        num++;
                    }
                    if (row <= 0)
                    {
                        return null;
                    }
                    row--;
                    if (columnRelative)
                    {
                        column -= columnBase;
                    }
                    if (rowRelative)
                    {
                        row -= rowBase;
                    }
                    goto Label_03AF;
                }
            }
            return null;
        Label_03AF:
            if (num == s.Length)
            {
                return new RangeExpression(row, column, 1, 1, rowRelative, columnRelative);
            }
            return null;
        }

        private static RangeExpression ParseColumnReference(string s, int columnBase, bool useR1C1)
        {
            int num = 0;
            int column = 0;
            bool columnRelative = false;
            bool flag2 = false;
            if (!useR1C1)
            {
                if (!(columnRelative = (num >= s.Length) || (s[num] != '$')))
                {
                    num++;
                }
                if (num < s.Length)
                {
                    char ch = CultureInfo.InvariantCulture.TextInfo.ToUpper(s[num]);
                    if ((ch >= 'A') && (ch <= 'Z'))
                    {
                        while (((num < s.Length) && ('A' <= ch)) && (ch <= 'Z'))
                        {
                            column = (((0x1a * column) + CultureInfo.InvariantCulture.TextInfo.ToUpper(s[num])) - 0x41) + 1;
                            if (column < 0)
                            {
                                return null;
                            }
                            num++;
                            if (num < s.Length)
                            {
                                ch = CultureInfo.InvariantCulture.TextInfo.ToUpper(s[num]);
                            }
                        }
                        if (column <= 0)
                        {
                            return null;
                        }
                        column--;
                        if (columnRelative)
                        {
                            column -= columnBase;
                        }
                        goto Label_0227;
                    }
                }
                return null;
            }
            if ((num < s.Length) && ((s[num] == 'c') || (s[num] == 'C')))
            {
                num++;
            }
            if (!(columnRelative = (num < s.Length) && (s[num] == '[')))
            {
                if (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                {
                    while (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                    {
                        column = (10 * column) + (s[num] - '0');
                        if (column < 0)
                        {
                            return null;
                        }
                        num++;
                    }
                    if (column <= 0)
                    {
                        return null;
                    }
                    column--;
                }
                else
                {
                    columnRelative = true;
                }
            }
            else
            {
                num++;
                if (flag2 = (num < s.Length) && (s[num] == '-'))
                {
                    num++;
                }
                if (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                {
                    while (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                    {
                        column = (10 * column) + (s[num] - '0');
                        if (column < 0)
                        {
                            return null;
                        }
                        num++;
                    }
                    if ((num < s.Length) && (s[num] == ']'))
                    {
                        num++;
                    }
                    if (flag2)
                    {
                        column = -column;
                    }
                }
                else
                {
                    return null;
                }
            }
        Label_0227:
            if (columnRelative && (column >= 0x3ff0))
            {
                column -= 0x4000;
            }
            if (num == s.Length)
            {
                return new RangeExpression(-1, column, 0, 1, false, columnRelative) { isEntireColumn = true };
            }
            return null;
        }

        private static ExternalRangeExpression ParseExternalReference(string s, int rowBase, int columnBase, bool useR1C1, LinkTable linkTable)
        {
            if ((s != null) && (0 < s.Length))
            {
                string str = null;
                string name = null;
                if (s[0] == '\'')
                {
                    string[] strArray = s.Split(new char[] { '!' });
                    if (strArray.Length == 2)
                    {
                        str = strArray[0];
                        if (((str != null) && (str.Length >= 2)) && ((str[0] == str[str.Length - 1]) && (str[0] == '\'')))
                        {
                            str = str.Substring(1, str.Length - 2);
                        }
                        str = str.Replace("''", "'");
                        name = strArray[1];
                    }
                }
                else
                {
                    string[] strArray2 = s.Split(new char[] { '!' });
                    if (strArray2.Length == 2)
                    {
                        str = strArray2[0];
                        name = strArray2[1];
                    }
                    else if (s.IndexOf('!') != -1)
                    {
                        name = s;
                    }
                }
                if ((str != null) && (name != null))
                {
                    if (((linkTable != null) && (linkTable.InternalSheetNames != null)) && linkTable.HasDefindName(name, linkTable.InternalSheetNames.IndexOf(str)))
                    {
                        return null;
                    }
                    object source = null;
                    int index = str.IndexOf(":");
                    if ((index == -1) || (((index + 1) < str.Length) && (str[index + 1] == '\\')))
                    {
                        source = str;
                    }
                    else
                    {
                        List<object> list = new List<object>();
                        string[] strArray3 = str.Split(new char[] { ':' });
                        for (int i = 0; i < strArray3.Length; i++)
                        {
                            list.Add(strArray3[i]);
                        }
                        source = list;
                    }
                    if (source != null)
                    {
                        if (((rowBase == -1) && (columnBase == -1)) && ((selections != null) && (sheetNames != null)))
                        {
                            string str3 = "";
                            if (source is List<object>)
                            {
                                List<object> list2 = source as List<object>;
                                if ((list2 != null) && (list2.Count > 0))
                                {
                                    str3 = list2[0].ToString();
                                }
                            }
                            else
                            {
                                str3 = source.ToString();
                            }
                            int num3 = sheetNames.IndexOf(str3);
                            if ((num3 != -1) && (selections.Count > num3))
                            {
                                rowBase = selections[num3].Item1;
                                columnBase = selections[num3].Item2;
                            }
                        }
                        RangeExpression expression = ParseReference(name, rowBase, columnBase, useR1C1);
                        if (expression != null)
                        {
                            return new ExternalRangeExpression(source, expression.Row, expression.Column, expression.RowCount, expression.ColumnCount, expression.RowRelative, expression.ColumnRelative);
                        }
                    }
                }
            }
            return null;
        }

        private static ExternalRangeExpression2 ParseExternalReference2(string s, int rowBase, int columnBase, bool useR1C1, LinkTable linkTable)
        {
            if ((s != null) && (0 < s.Length))
            {
                string str = null;
                string str2 = null;
                if (s[0] == '\'')
                {
                    int num = 1;
                    while (((num < s.Length) && (s[num] != '\'')) || ((((num + 1) < s.Length) && (s[num] == '\'')) && (s[num + 1] == '\'')))
                    {
                        if (s[num] == '\'')
                        {
                            num++;
                        }
                        num++;
                    }
                    if ((((num + 1) < s.Length) && (s[num] == '\'')) && (s[num + 1] == '!'))
                    {
                        str = s.Substring(1, num - 1).Replace("''", "'");
                        str2 = s.Substring(num + 2, (s.Length - num) - 2);
                    }
                }
                else
                {
                    int length = 0;
                    while ((length < s.Length) && (s[length] != '!'))
                    {
                        length++;
                    }
                    if ((length < s.Length) && (s[length] == '!'))
                    {
                        str = s.Substring(0, length);
                        str2 = s.Substring(length + 1, (s.Length - length) - 1);
                    }
                }
                if ((str != null) && (str2 != null))
                {
                    object obj2 = null;
                    if (str.IndexOf(":") == -1)
                    {
                        obj2 = str;
                    }
                    else
                    {
                        List<object> list = new List<object>();
                        string[] strArray = str.Split(new char[] { ':' });
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            list.Add(strArray[i]);
                        }
                        obj2 = list;
                    }
                    if (obj2 != null)
                    {
                        RangeExprssion2 exprssion = ParseReference2(str2, rowBase, columnBase, useR1C1);
                        if (exprssion != null)
                        {
                            return new ExternalRangeExpression2 { Source = obj2, RefExpr = exprssion, rowFirst = exprssion.rowFirst, rowLast = exprssion.rowLast, colFirst = exprssion.colFirst, colLast = exprssion.colLast, isWholeColumn = exprssion.isWholeColumn, isWholeRow = exprssion.isWholeRow, isColumnFirstRel = exprssion.isColumnFirstRel, isColumnLastRel = exprssion.isColumnLastRel, isRowFirstRel = exprssion.isRowFirstRel, isRowLastRel = exprssion.isRowLastRel };
                        }
                    }
                }
            }
            return null;
        }

        private static RangeExpression ParseReference(string s, int rowBase, int columnBase, bool useR1C1)
        {
            int index = s.IndexOf(':');
            if (index >= 0)
            {
                RangeExpression expression2;
                string str = s.Substring(0, index);
                string str2 = s.Substring(index + 1, (s.Length - index) - 1);
                RangeExpression expression = ParseRowReference(str, rowBase, useR1C1);
                if (expression != null)
                {
                    expression2 = ParseRowReference(str2, rowBase, useR1C1);
                    if ((expression2 != null) && (expression.RowRelative == expression2.RowRelative))
                    {
                        int row = Math.Min(expression.Row, expression2.Row);
                        int rowCount = (Math.Max(expression.Row, expression2.Row) - row) + 1;
                        if (expression.RowRelative && (row >= 0xffff0))
                        {
                            row -= 0x100000;
                        }
                        return new RangeExpression(row, -1, rowCount, 0, expression.RowRelative, false) { isEntrieRow = true };
                    }
                }
                expression = ParseColumnReference(str, columnBase, useR1C1);
                if (expression != null)
                {
                    expression2 = ParseColumnReference(str2, columnBase, useR1C1);
                    if ((expression2 != null) && (expression.ColumnRelative == expression2.ColumnRelative))
                    {
                        int column = Math.Min(expression.Column, expression2.Column);
                        int columnCount = (Math.Max(expression.Column, expression2.Column) - column) + 1;
                        if (expression2.ColumnRelative && (column >= 0x3ff0))
                        {
                            column -= 0x4000;
                        }
                        return new RangeExpression(-1, column, 0, columnCount, false, expression2.ColumnRelative) { isEntireColumn = true };
                    }
                }
                expression = ParseCellReference(str, rowBase, columnBase, useR1C1);
                if (expression != null)
                {
                    expression2 = ParseCellReference(str2, rowBase, columnBase, useR1C1);
                    if (((expression2 != null) && (expression.RowRelative == expression2.RowRelative)) && (expression.ColumnRelative == expression2.ColumnRelative))
                    {
                        int num6 = Math.Min(expression.Row, expression2.Row);
                        int num7 = Math.Min(expression.Column, expression2.Column);
                        int num8 = (Math.Max(expression.Row, expression2.Row) - num6) + 1;
                        int num9 = (Math.Max(expression.Column, expression2.Column) - num7) + 1;
                        if (expression.RowRelative && (num6 >= 0xffff0))
                        {
                            num6 -= 0x100000;
                        }
                        if (expression2.ColumnRelative && (num7 >= 0x3ff0))
                        {
                            num7 -= 0x4000;
                        }
                        return new RangeExpression(num6, num7, num8, num9, expression.RowRelative, expression2.ColumnRelative);
                    }
                }
                return null;
            }
            if (useR1C1)
            {
                RangeExpression expression5 = ParseRowReference(s, rowBase, useR1C1);
                if (expression5 != null)
                {
                    return expression5;
                }
                expression5 = ParseColumnReference(s, columnBase, useR1C1);
                if (expression5 != null)
                {
                    return expression5;
                }
            }
            return ParseCellReference(s, rowBase, columnBase, useR1C1);
        }

        private static RangeExprssion2 ParseReference2(string s, int rowBase, int columnBase, bool useR1C1)
        {
            int index = s.IndexOf(':');
            if (index >= 0)
            {
                string str = s.Substring(0, index);
                string str2 = s.Substring(index + 1, (s.Length - index) - 1);
                RangeExpression expression = ParseRowReference(str, rowBase, useR1C1);
                RangeExpression expression2 = ParseRowReference(str2, rowBase, useR1C1);
                if (((expression != null) && (expression2 != null)) && (expression.RowRelative != expression2.RowRelative))
                {
                    RangeExprssion2 exprssion = new RangeExprssion2 {
                        rowFirst = expression.RowRelative ? (expression.Row + rowBase) : expression.Row,
                        colFirst = expression.ColumnRelative ? (expression.Column + columnBase) : expression.Column,
                        rowLast = expression2.RowRelative ? (expression2.Row + rowBase) : expression2.Row,
                        colLast = expression2.ColumnRelative ? (expression2.Column + columnBase) : expression2.Column,
                        isRowFirstRel = expression.RowRelative,
                        isColumnFirstRel = expression.ColumnRelative,
                        isRowLastRel = expression2.RowRelative,
                        isColumnLastRel = expression2.ColumnRelative
                    };
                    if ((expression.Column == expression2.Column) && (expression.Column == -1))
                    {
                        exprssion.isWholeRow = true;
                    }
                    return exprssion;
                }
                expression = ParseColumnReference(str, columnBase, useR1C1);
                expression2 = ParseColumnReference(str2, columnBase, useR1C1);
                if (((expression != null) && (expression2 != null)) && (expression.ColumnRelative != expression2.ColumnRelative))
                {
                    RangeExprssion2 exprssion2 = new RangeExprssion2 {
                        rowFirst = expression.RowRelative ? (expression.Row + rowBase) : expression.Row,
                        colFirst = expression.ColumnRelative ? (expression.Column + columnBase) : expression.Column,
                        rowLast = expression2.RowRelative ? (expression2.Row + rowBase) : expression2.Row,
                        colLast = expression2.ColumnRelative ? (expression2.Column + columnBase) : expression2.Column,
                        isRowFirstRel = expression.RowRelative,
                        isColumnFirstRel = expression.ColumnRelative,
                        isRowLastRel = expression2.RowRelative,
                        isColumnLastRel = expression2.ColumnRelative
                    };
                    if ((expression.Row == expression2.Row) && (expression.Row == -1))
                    {
                        exprssion2.isWholeColumn = true;
                    }
                    return exprssion2;
                }
                expression = ParseCellReference(str, rowBase, columnBase, useR1C1);
                expression2 = ParseCellReference(str2, rowBase, columnBase, useR1C1);
                if ((expression != null) && (expression2 != null))
                {
                    return new RangeExprssion2 { rowFirst = expression.RowRelative ? (expression.Row + rowBase) : expression.Row, colFirst = expression.ColumnRelative ? (expression.Column + columnBase) : expression.Column, rowLast = expression2.RowRelative ? (expression2.Row + rowBase) : expression2.Row, colLast = expression2.ColumnRelative ? (expression2.Column + columnBase) : expression2.Column, isRowFirstRel = expression.RowRelative, isColumnFirstRel = expression.ColumnRelative, isRowLastRel = expression2.RowRelative, isColumnLastRel = expression2.ColumnRelative };
                }
                return null;
            }
            if (useR1C1 && useR1C1)
            {
                ParseRowReference(s, rowBase, useR1C1);
                ParseColumnReference(s, columnBase, useR1C1);
            }
            return null;
        }

        private static RangeExpression ParseRowReference(string s, int rowBase, bool useR1C1)
        {
            int num = 0;
            int row = 0;
            bool rowRelative = false;
            bool flag2 = false;
            if (!useR1C1)
            {
                if (!(rowRelative = (num >= s.Length) || (s[num] != '$')))
                {
                    num++;
                }
                if (((num >= s.Length) || (s[num] < '1')) || (s[num] > '9'))
                {
                    return null;
                }
                while (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                {
                    row = (10 * row) + (s[num] - '0');
                    if (row < 0)
                    {
                        return null;
                    }
                    num++;
                }
                if (row <= 0)
                {
                    return null;
                }
                row--;
                if (rowRelative)
                {
                    row -= rowBase;
                }
            }
            else
            {
                if ((num < s.Length) && ((s[num] == 'r') || (s[num] == 'R')))
                {
                    num++;
                }
                if (!(rowRelative = (num < s.Length) && (s[num] == '[')))
                {
                    if (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                    {
                        while (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                        {
                            row = (10 * row) + (s[num] - '0');
                            if (row < 0)
                            {
                                return null;
                            }
                            num++;
                        }
                        if (row <= 0)
                        {
                            return null;
                        }
                        row--;
                    }
                    else
                    {
                        rowRelative = true;
                    }
                }
                else
                {
                    num++;
                    if (flag2 = (num < s.Length) && (s[num] == '-'))
                    {
                        num++;
                    }
                    if (((num < s.Length) && (s[num] >= '0')) && (s[num] <= '9'))
                    {
                        while (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
                        {
                            row = (10 * row) + (s[num] - '0');
                            if (row < 0)
                            {
                                return null;
                            }
                            num++;
                        }
                        if ((num < s.Length) && (s[num] == ']'))
                        {
                            num++;
                        }
                        if (flag2)
                        {
                            row = -row;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            if (rowRelative && (row >= 0xffff0))
            {
                row -= 0x100000;
            }
            if ((num == s.Length) && (num == s.Length))
            {
                return new RangeExpression(row, -1, 1, 0, rowRelative, false) { isEntrieRow = true };
            }
            return null;
        }

        private static string RemoveQuotasAndSquareBrackets(string fileName)
        {
            if (((fileName != null) && (fileName.Length >= 2)) && ((fileName[0] == '\'') && (fileName[fileName.Length - 1] == '\'')))
            {
                fileName = fileName.Substring(1, fileName.Length - 2);
            }
            int index = fileName.IndexOf(']');
            if (index != -1)
            {
                fileName = fileName.Substring(0, index);
                index = fileName.LastIndexOf('[');
                if (index != -1)
                {
                    fileName = fileName.Substring(0, index) + fileName.Substring(index + 1);
                }
            }
            return fileName;
        }

        private static bool TransferOperators(Stack<object> exprStack, Stack<object> operStack)
        {
            while ((operStack.Count > 0) && object.ReferenceEquals(operStack.Peek(), commaPlaceHolder))
            {
                object obj2 = operStack.Pop();
                if (exprStack.Count < 2)
                {
                    return false;
                }
                ParsedToken token = (ParsedToken) exprStack.Pop();
                ParsedToken token2 = (ParsedToken) exprStack.Pop();
                if ((((token2.TokenType == TokenType.BinaryOperation) && (token2.Token == ",")) || ((token2.TokenType == TokenType.Reference) || (token2.TokenType == TokenType.ExReference))) && (((token.TokenType == TokenType.BinaryOperation) && (token.Token == ",")) || ((token.TokenType == TokenType.Reference) || (token.TokenType == TokenType.ExReference))))
                {
                    ParsedToken token3 = new ParsedToken {
                        Token = ",",
                        TokenType = TokenType.BinaryOperation,
                        Value = new ParsedToken[] { token2, token }
                    };
                    exprStack.Push(token3);
                }
                else if ((token2.TokenType == TokenType.Error) && (token.TokenType == TokenType.Error))
                {
                    ParsedToken token4 = new ParsedToken {
                        Token = "#REF!",
                        TokenType = TokenType.Error,
                        Value = ExcelCalcError.IllegalOrDeletedCellReference
                    };
                    exprStack.Push(token4);
                }
                else
                {
                    operStack.Push(obj2);
                }
            }
            return true;
        }

        private static void TransferOperators(Stack<object> exprStack, Stack<object> operStack, int precedence)
        {
            if (operStack.Count > 0)
            {
                for (object obj2 = operStack.Peek(); (obj2 is OperatorInfo) && (GetPrecedenceLevel((OperatorInfo) obj2) >= precedence); obj2 = operStack.Peek())
                {
                    if (obj2 is BinaryOperatorInfo)
                    {
                        BinaryOperatorInfo info = (BinaryOperatorInfo) operStack.Pop();
                        ParsedToken token = (ParsedToken) exprStack.Pop();
                        ParsedToken token2 = (ParsedToken) exprStack.Pop();
                        ParsedToken token3 = new ParsedToken {
                            Token = info.Name,
                            TokenType = TokenType.BinaryOperation,
                            Value = new ParsedToken[] { token2, token }
                        };
                        exprStack.Push(token3);
                    }
                    else if (obj2 is UnaryOperatorInfo)
                    {
                        UnaryOperatorInfo info2 = (UnaryOperatorInfo) operStack.Pop();
                        ParsedToken token4 = (ParsedToken) exprStack.Pop();
                        ParsedToken token5 = new ParsedToken {
                            Token = info2.Name,
                            TokenType = TokenType.UnaryOperation,
                            Value = token4
                        };
                        exprStack.Push(token5);
                    }
                    if (operStack.Count <= 0)
                    {
                        break;
                    }
                }
            }
        }

        public static string Unparse(ParsedToken token, int baseRow, int baseColumn, bool useR1C1)
        {
            if (token != null)
            {
                Stack<string> output = new Stack<string>();
                Unparse(output, token, baseRow, baseColumn, useR1C1, null);
                if (output.Count > 0)
                {
                    return output.Pop();
                }
            }
            return string.Empty;
        }

        private static void Unparse(Stack<string> output, ParsedToken expr, int baseRow, int baseColumn, bool useR1C1, INameSupport source)
        {
            if (expr.TokenType == TokenType.Double)
            {
                double num8 = (double) ((double) expr.Value);
                output.Push(((double) num8).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            else if (expr.TokenType == TokenType.Boolean)
            {
                output.Push(((bool) expr.Value) ? "TRUE" : "FALSE");
            }
            else if (expr.TokenType == TokenType.String)
            {
                output.Push("\"" + ((string) expr.Value).Replace("\"", "\"\"") + "\"");
            }
            else if (expr.TokenType == TokenType.Error)
            {
                output.Push(expr.Value.ToString());
            }
            else if (expr.TokenType == TokenType.Array)
            {
                StringBuilder builder = new StringBuilder();
                object[,] objArray = expr.Value as object[,];
                builder.Append("{");
                int num = objArray.GetUpperBound(0) + 1;
                for (int i = 0; i < num; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(";");
                    }
                    int num3 = objArray.GetUpperBound(1) + 1;
                    for (int j = 0; j < num3; j++)
                    {
                        ParsedToken token;
                        double num10;
                        if (j != 0)
                        {
                            builder.Append(",");
                        }
                        object obj2 = objArray.GetValue(new int[] { i, j });
                        if (obj2 is double)
                        {
                            double d = (double) ((double) obj2);
                            if (double.IsNaN(d) || double.IsInfinity(d))
                            {
                                builder.Append(ExcelCalcError.ValueRangeOverflow.ToString());
                            }
                            else
                            {
                                double num9 = (double) ((double) obj2);
                                builder.Append(((double) num9).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                        else if (obj2 is string)
                        {
                            builder.Append("\"");
                            builder.Append(((string) obj2).Replace("\"", "\"\""));
                            builder.Append("\"");
                        }
                        else if (obj2 is bool)
                        {
                            builder.Append(((bool) obj2) ? ((string) "TRUE") : ((string) "FALSE"));
                        }
                        else if (obj2 is ExcelCalcError)
                        {
                            builder.Append(obj2.ToString());
                        }
                        else if (obj2 is ParsedToken)
                        {
                            token = obj2 as ParsedToken;
                            switch (token.TokenType)
                            {
                                case TokenType.Double:
                                {
                                    double num6 = (double) ((double) token.Value);
                                    if (!double.IsNaN(num6) && !double.IsInfinity(num6))
                                    {
                                        goto Label_02AE;
                                    }
                                    builder.Append(ExcelCalcError.ValueRangeOverflow.ToString());
                                    break;
                                }
                                case TokenType.String:
                                    builder.Append("\"");
                                    if (!(token.Value is string))
                                    {
                                        goto Label_0312;
                                    }
                                    builder.Append(((string) token.Value).Replace("\"", "\"\""));
                                    goto Label_0347;

                                case TokenType.Error:
                                    builder.Append(token.Value.ToString());
                                    break;

                                case TokenType.Boolean:
                                    builder.Append(((bool) token.Value) ? ((string) "TRUE") : ((string) "FALSE"));
                                    break;
                            }
                        }
                        continue;
                    Label_02AE:
                        num10 = (double) ((double) token.Value);
                        builder.Append(((double) num10).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        continue;
                    Label_0312:
                        if (token.Value is Token)
                        {
                            builder.Append((token.Value as Token).Text.Replace("\"", "\"\""));
                        }
                    Label_0347:
                        builder.Append("\"");
                    }
                }
                builder.Append("}");
                output.Push(builder.ToString());
            }
            else if (expr.TokenType == TokenType.Reference)
            {
                StringBuilder builder2 = new StringBuilder();
                if (expr.Value is RangeExpression)
                {
                    RangeExpression expression = (RangeExpression) expr.Value;
                    if (source != null)
                    {
                        builder2.Append(AppendExternalName(source));
                        builder2.Append("!");
                    }
                    if ((expression.RowCount == 1) && (expression.ColumnCount == 1))
                    {
                        if (useR1C1)
                        {
                            string str = AppendR1C1Number("R", expression.Row, expression.RowRelative) + AppendR1C1Number("C", expression.Column, expression.ColumnRelative);
                            builder2.Append(str);
                        }
                        else
                        {
                            string str2 = AppendA1Letter(expression.Column, expression.ColumnRelative, baseColumn) + AppendA1Number(expression.Row, expression.RowRelative, baseRow);
                            builder2.Append(str2);
                        }
                    }
                    else if (useR1C1)
                    {
                        if (expression.RowRelative || (expression.Row >= 0))
                        {
                            builder2.Append(AppendR1C1Number("R", expression.Row, expression.RowRelative));
                        }
                        if (expression.ColumnRelative || (expression.Column >= 0))
                        {
                            builder2.Append(AppendR1C1Number("C", expression.Column, expression.ColumnRelative));
                        }
                        builder2.Append(":");
                        if (expression.RowRelative || (expression.Row >= 0))
                        {
                            builder2.Append(AppendR1C1Number("R", (expression.Row + expression.RowCount) - 1, expression.RowRelative));
                        }
                        if (expression.ColumnRelative || (expression.Column >= 0))
                        {
                            builder2.Append(AppendR1C1Number("C", (expression.Column + expression.ColumnCount) - 1, expression.ColumnRelative));
                        }
                    }
                    else
                    {
                        if ((expression.ColumnRelative || (expression.Column >= 0)) && !expression.isEntrieRow)
                        {
                            builder2.Append(AppendA1Letter(expression.Column, expression.ColumnRelative, baseColumn));
                        }
                        if ((expression.RowRelative || (expression.Row >= 0)) && !expression.isEntireColumn)
                        {
                            builder2.Append(AppendA1Number(expression.Row, expression.RowRelative, baseRow));
                        }
                        builder2.Append(":");
                        if ((expression.ColumnRelative || (expression.Column >= 0)) && !expression.isEntrieRow)
                        {
                            builder2.Append(AppendA1Letter((expression.Column + expression.ColumnCount) - 1, expression.ColumnRelative, baseColumn));
                        }
                        if ((expression.RowRelative || (expression.Row >= 0)) && !expression.isEntireColumn)
                        {
                            builder2.Append(AppendA1Number((expression.Row + expression.RowCount) - 1, expression.RowRelative, baseRow));
                        }
                    }
                }
                else if (expr.Value is RangeExprssion2)
                {
                    RangeExprssion2 exprssion = (RangeExprssion2) expr.Value;
                    if (source != null)
                    {
                        builder2.Append(AppendExternalName(source));
                        builder2.Append("!");
                    }
                    if ((exprssion.rowFirst == exprssion.rowLast) && (exprssion.colFirst == exprssion.colLast))
                    {
                        if (useR1C1)
                        {
                            string str3 = AppendR1C1Number("R", exprssion.rowFirst, exprssion.isRowFirstRel) + AppendR1C1Number("C", exprssion.colFirst, exprssion.isColumnFirstRel);
                            builder2.Append(str3);
                        }
                        else
                        {
                            string str4 = AppendA1Letter(exprssion.colFirst, exprssion.isColumnFirstRel, baseColumn) + AppendA1Number(exprssion.rowFirst, exprssion.isRowFirstRel, baseRow);
                            builder2.Append(str4);
                        }
                    }
                    else if (useR1C1)
                    {
                        if (exprssion.isRowFirstRel || (exprssion.rowFirst >= 0))
                        {
                            builder2.Append(AppendR1C1Number("R", exprssion.isRowFirstRel ? (exprssion.rowFirst - baseRow) : exprssion.rowFirst, exprssion.isRowFirstRel));
                        }
                        if (exprssion.isColumnFirstRel || (exprssion.colFirst >= 0))
                        {
                            builder2.Append(AppendR1C1Number("C", exprssion.isColumnFirstRel ? (exprssion.colFirst - baseColumn) : exprssion.colFirst, exprssion.isColumnFirstRel));
                        }
                        builder2.Append(":");
                        if (exprssion.isRowLastRel || (exprssion.rowLast >= 0))
                        {
                            builder2.Append(AppendR1C1Number("R", exprssion.isRowLastRel ? (exprssion.rowLast - baseRow) : exprssion.rowLast, exprssion.isRowLastRel));
                        }
                        if (exprssion.isColumnLastRel || (exprssion.colLast >= 0))
                        {
                            builder2.Append(AppendR1C1Number("C", exprssion.isColumnLastRel ? (exprssion.colLast - baseColumn) : exprssion.colLast, exprssion.isColumnLastRel));
                        }
                    }
                    else
                    {
                        if ((exprssion.isColumnFirstRel || (exprssion.colFirst >= 0)) && !exprssion.isWholeRow)
                        {
                            builder2.Append(AppendA1Letter(exprssion.isColumnFirstRel ? (exprssion.colFirst - baseColumn) : exprssion.colFirst, exprssion.isColumnFirstRel, baseColumn));
                        }
                        if ((exprssion.isRowFirstRel || (exprssion.rowFirst >= 0)) && !exprssion.isWholeColumn)
                        {
                            builder2.Append(AppendA1Number(exprssion.isRowFirstRel ? (exprssion.rowFirst - baseRow) : exprssion.rowFirst, exprssion.isRowFirstRel, baseRow));
                        }
                        builder2.Append(":");
                        if ((exprssion.isColumnLastRel || (exprssion.colLast >= 0)) && !exprssion.isWholeRow)
                        {
                            builder2.Append(AppendA1Letter(exprssion.isColumnLastRel ? (exprssion.colLast - baseColumn) : exprssion.colLast, exprssion.isColumnLastRel, baseColumn));
                        }
                        if ((exprssion.isRowLastRel || (exprssion.rowLast >= 0)) && !exprssion.isWholeColumn)
                        {
                            builder2.Append(AppendA1Number(exprssion.isRowLastRel ? (exprssion.rowLast - baseRow) : exprssion.rowLast, exprssion.isRowLastRel, baseRow));
                        }
                    }
                }
                output.Push(builder2.ToString());
            }
            else if (expr.TokenType == TokenType.ExReference)
            {
                StringBuilder builder3 = new StringBuilder();
                if (expr.Value is ExternalRangeExpression)
                {
                    ExternalRangeExpression expression2 = (ExternalRangeExpression) expr.Value;
                    builder3.Append(AppendExternalName(expression2.Source));
                    if ((builder3.Length > 0) && (builder3[builder3.Length - 1] != '!'))
                    {
                        builder3.Append("!");
                    }
                    if ((expression2.RowCount == 1) && (expression2.ColumnCount == 1))
                    {
                        if (useR1C1)
                        {
                            builder3.Append(AppendR1C1Number("R", expression2.Row, expression2.RowRelative));
                            builder3.Append(AppendR1C1Number("C", expression2.Column, expression2.ColumnRelative));
                        }
                        else
                        {
                            builder3.Append(AppendA1Letter(expression2.Column, expression2.ColumnRelative, baseColumn));
                            builder3.Append(AppendA1Number(expression2.Row, expression2.RowRelative, baseRow));
                        }
                    }
                    else if (useR1C1)
                    {
                        if (expression2.RowRelative || (expression2.Row >= 0))
                        {
                            builder3.Append(AppendR1C1Number("R", expression2.Row, expression2.RowRelative));
                        }
                        if (expression2.ColumnRelative || (expression2.Column >= 0))
                        {
                            builder3.Append(AppendR1C1Number("C", expression2.Column, expression2.ColumnRelative));
                        }
                        builder3.Append(":");
                        if (expression2.RowRelative || (expression2.Row >= 0))
                        {
                            builder3.Append(AppendR1C1Number("R", (expression2.Row + expression2.RowCount) - 1, expression2.RowRelative));
                        }
                        if (expression2.ColumnRelative || (expression2.Column >= 0))
                        {
                            builder3.Append(AppendR1C1Number("C", (expression2.Column + expression2.ColumnCount) - 1, expression2.ColumnRelative));
                        }
                    }
                    else
                    {
                        if ((expression2.ColumnRelative || (expression2.Column >= 0)) && !expression2.isEntireColumn)
                        {
                            builder3.Append(AppendA1Letter(expression2.Column, expression2.ColumnRelative, baseColumn));
                        }
                        if ((expression2.RowRelative || (expression2.Row >= 0)) && !expression2.isEntireColumn)
                        {
                            builder3.Append(AppendA1Number(expression2.Row, expression2.RowRelative, baseRow));
                        }
                        builder3.Append(":");
                        if ((expression2.ColumnRelative || (expression2.Column >= 0)) && !expression2.isEntireColumn)
                        {
                            builder3.Append(AppendA1Letter((expression2.Column + expression2.ColumnCount) - 1, expression2.ColumnRelative, baseColumn));
                        }
                        if ((expression2.RowRelative || (expression2.Row >= 0)) && !expression2.isEntrieRow)
                        {
                            builder3.Append(AppendA1Number((expression2.Row + expression2.RowCount) - 1, expression2.RowRelative, baseRow));
                        }
                    }
                }
                else if (expr.Value is ExternalRangeExpression2)
                {
                    ExternalRangeExpression2 expression3 = expr.Value as ExternalRangeExpression2;
                    builder3.Append(AppendExternalName(expression3.Source));
                    builder3.Append("!");
                    if ((expression3.colFirst == expression3.colLast) && (expression3.rowFirst == expression3.rowLast))
                    {
                        if (useR1C1)
                        {
                            builder3.Append(AppendR1C1Number("R", expression3.rowFirst, expression3.isRowFirstRel));
                            builder3.Append(AppendR1C1Number("C", expression3.colFirst, expression3.isColumnFirstRel));
                        }
                        else
                        {
                            builder3.Append(AppendA1Letter(expression3.colFirst, expression3.isColumnFirstRel, baseColumn));
                            builder3.Append(AppendA1Number(expression3.rowFirst, expression3.isRowFirstRel, baseRow));
                        }
                    }
                    else if (useR1C1)
                    {
                        if (expression3.isRowFirstRel || (expression3.rowFirst >= 0))
                        {
                            builder3.Append(AppendR1C1Number("R", expression3.isRowFirstRel ? (expression3.rowFirst - baseRow) : expression3.rowFirst, expression3.isRowFirstRel));
                        }
                        if (expression3.isColumnFirstRel || (expression3.colFirst >= 0))
                        {
                            builder3.Append(AppendR1C1Number("C", expression3.isColumnFirstRel ? (expression3.colFirst - baseColumn) : expression3.colFirst, expression3.isColumnFirstRel));
                        }
                        builder3.Append(":");
                        if (expression3.isRowLastRel || (expression3.rowLast >= 0))
                        {
                            builder3.Append(AppendR1C1Number("R", expression3.isRowLastRel ? (expression3.rowLast - baseRow) : expression3.rowLast, expression3.isRowLastRel));
                        }
                        if (expression3.isColumnLastRel || (expression3.colLast >= 0))
                        {
                            builder3.Append(AppendR1C1Number("C", expression3.isColumnLastRel ? (expression3.colLast - baseColumn) : expression3.colLast, expression3.isColumnLastRel));
                        }
                    }
                    else
                    {
                        if ((expression3.isColumnFirstRel || (expression3.colFirst >= 0)) && !expression3.isWholeRow)
                        {
                            builder3.Append(AppendA1Letter(expression3.isColumnFirstRel ? (expression3.colFirst - baseColumn) : expression3.colFirst, expression3.isColumnFirstRel, baseColumn));
                        }
                        if ((expression3.isRowFirstRel || (expression3.rowFirst >= 0)) && !expression3.isWholeColumn)
                        {
                            builder3.Append(AppendA1Number(expression3.isRowFirstRel ? (expression3.rowFirst - baseRow) : expression3.rowFirst, expression3.isRowFirstRel, baseRow));
                        }
                        builder3.Append(":");
                        if ((expression3.isColumnLastRel || (expression3.colLast >= 0)) && !expression3.isWholeRow)
                        {
                            builder3.Append(AppendA1Letter(expression3.isColumnLastRel ? (expression3.colLast - baseColumn) : expression3.colLast, expression3.isColumnLastRel, baseColumn));
                        }
                        if ((expression3.isRowLastRel || (expression3.rowLast >= 0)) && !expression3.isWholeColumn)
                        {
                            builder3.Append(AppendA1Number(expression3.isRowLastRel ? (expression3.rowLast - baseRow) : expression3.rowLast, expression3.isRowLastRel, baseRow));
                        }
                    }
                }
                else
                {
                    builder3.Append(expr.Value);
                }
                output.Push(builder3.ToString());
            }
            else if (expr.TokenType == TokenType.Name)
            {
                StringBuilder builder4 = new StringBuilder();
                if (source != null)
                {
                    builder4.Append(AppendExternalName(source));
                    builder4.Append("!");
                }
                builder4.Append(expr.Token);
                output.Push(builder4.ToString());
            }
            else if (expr.TokenType == TokenType.UnaryOperation)
            {
                if (LookupPrefixOperator(expr.Token) != null)
                {
                    Unparse(output, (ParsedToken) expr.Value, baseRow, baseColumn, useR1C1, source);
                    output.Push(expr.Token);
                }
                else
                {
                    output.Push(expr.Token);
                    Unparse(output, (ParsedToken) expr.Value, baseRow, baseColumn, useR1C1, source);
                }
                output.Push(string.Format("{0}{1}", (object[]) new object[] { output.Pop(), output.Pop() }));
            }
            else if (expr.TokenType == TokenType.BinaryOperation)
            {
                ParsedToken[] tokenArray = (ParsedToken[]) expr.Value;
                Unparse(output, tokenArray[1], baseRow, baseColumn, useR1C1, source);
                output.Push(expr.Token);
                Unparse(output, tokenArray[0], baseRow, baseColumn, useR1C1, source);
                output.Push(string.Format("{0}{1}{2}", (object[]) new object[] { output.Pop(), output.Pop(), output.Pop() }));
            }
            else if ((expr.TokenType == TokenType.Function) || (expr.TokenType == TokenType.UndefinedFunction))
            {
                StringBuilder builder5 = new StringBuilder();
                if (expr.Token.ToLower(CultureInfo.InvariantCulture) == "errortype")
                {
                    builder5.Append("ERROR.TYPE");
                }
                else
                {
                    builder5.Append(expr.Token);
                }
                builder5.Append("(");
                ParsedToken[] tokenArray2 = expr.Value as ParsedToken[];
                for (int k = 0; k < tokenArray2.Length; k++)
                {
                    if (k != 0)
                    {
                        builder5.Append(",");
                    }
                    Stack<string> stack = new Stack<string>();
                    Unparse(stack, tokenArray2[k], baseRow, baseColumn, useR1C1, null);
                    if (stack.Count > 0)
                    {
                        builder5.Append(stack.Pop());
                    }
                }
                builder5.Append(")");
                output.Push(builder5.ToString());
            }
            else if (expr.TokenType == TokenType.Parentheses)
            {
                StringBuilder builder6 = new StringBuilder();
                builder6.Append("(");
                Stack<string> stack2 = new Stack<string>();
                Unparse(stack2, (ParsedToken) expr.Value, baseRow, baseColumn, useR1C1, null);
                if (stack2.Count > 0)
                {
                    builder6.Append(stack2.Pop());
                }
                builder6.Append(")");
                output.Push(builder6.ToString());
            }
            else if (expr.TokenType == TokenType.BinaryReferenceOperation)
            {
                StringBuilder sb = new StringBuilder();
                SubExternalRangeExpression expression4 = expr.Value as SubExternalRangeExpression;
                UnparseExternalRangeExpression(expression4.ExternalReference1, sb, baseRow, baseColumn, useR1C1);
                sb.Append(expression4.RangeOperator);
                UnparseExternalRangeExpression(expression4.ExternalReference2, sb, baseRow, baseColumn, useR1C1);
                output.Push(sb.ToString());
            }
        }

        private static void UnparseExternalRangeExpression(ExternalRangeExpression refExpr, StringBuilder sb, int baseRow, int baseColumn, bool useR1C1)
        {
            sb.Append(AppendExternalName(refExpr.Source));
            sb.Append("!");
            if ((refExpr.RowCount == 1) && (refExpr.ColumnCount == 1))
            {
                if (useR1C1)
                {
                    sb.Append(AppendR1C1Number("R", refExpr.Row, refExpr.RowRelative));
                    sb.Append(AppendR1C1Number("C", refExpr.Column, refExpr.ColumnRelative));
                }
                else
                {
                    sb.Append(AppendA1Letter(refExpr.Column, refExpr.ColumnRelative, baseColumn));
                    sb.Append(AppendA1Number(refExpr.Row, refExpr.RowRelative, baseRow));
                }
            }
            else if (useR1C1)
            {
                if (refExpr.RowRelative || (refExpr.Row >= 0))
                {
                    sb.Append(AppendR1C1Number("R", refExpr.Row, refExpr.RowRelative));
                }
                if (refExpr.ColumnRelative || (refExpr.Column >= 0))
                {
                    sb.Append(AppendR1C1Number("C", refExpr.Column, refExpr.ColumnRelative));
                }
                sb.Append(":");
                if (refExpr.RowRelative || (refExpr.Row >= 0))
                {
                    sb.Append(AppendR1C1Number("R", (refExpr.Row + refExpr.RowCount) - 1, refExpr.RowRelative));
                }
                if (refExpr.ColumnRelative || (refExpr.Column >= 0))
                {
                    sb.Append(AppendR1C1Number("C", (refExpr.Column + refExpr.ColumnCount) - 1, refExpr.ColumnRelative));
                }
            }
            else
            {
                if ((refExpr.ColumnRelative || (refExpr.Column >= 0)) && !refExpr.isEntireColumn)
                {
                    sb.Append(AppendA1Letter(refExpr.Column, refExpr.ColumnRelative, baseColumn));
                }
                if ((refExpr.RowRelative || (refExpr.Row >= 0)) && !refExpr.isEntireColumn)
                {
                    sb.Append(AppendA1Number(refExpr.Row, refExpr.RowRelative, baseRow));
                }
                sb.Append(":");
                if ((refExpr.ColumnRelative || (refExpr.Column >= 0)) && !refExpr.isEntireColumn)
                {
                    sb.Append(AppendA1Letter((refExpr.Column + refExpr.ColumnCount) - 1, refExpr.ColumnRelative, baseColumn));
                }
                if ((refExpr.RowRelative || (refExpr.Row >= 0)) && !refExpr.isEntrieRow)
                {
                    sb.Append(AppendA1Number((refExpr.Row + refExpr.RowCount) - 1, refExpr.RowRelative, baseRow));
                }
            }
        }
    }
}

