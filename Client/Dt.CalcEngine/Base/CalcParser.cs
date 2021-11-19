#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using Dt.CalcEngine.Operators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represents a parser which is used for
    /// parsing a string formula to <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" />
    /// and unparsing a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> to string.
    /// </summary>
    public class CalcParser
    {
        private char _arraryArgSeparator;
        private char _arraryGroupSeparator;
        private CultureInfo _catchedCulture;
        private static Dictionary<string, CalcFunction> _functions;
        private char _listSeparator;
        private char _mumberDecimalSeparator;
        internal const int BAND_INDEX_CONST = -2147483648;
        private static readonly string[] ERRORS = new string[] { "#NULL!", "#DIV/0!", "#VALUE!", "#REF!", "#NAME?", "#NUM!", "#N/A" };
        private static int[] LetterPows = new int[] { 1, 0x1a, 0x2a4 };
        private static int[] LetterPowsForUnParse = new int[] { 1, 0x1a, 0x2be };
        internal const int MaxColumnCount = 0x4000;
        internal const int MaxRowCount = 0x100000;
        private string OPERATORS_INFIX = "+-*/^&=><:, ";

        static CalcParser()
        {
            // hdt
            _functions = EnsureFunctions();
        }

        /// <summary>
        /// Initialize a new <see cref="T:Dt.CalcEngine.CalcParser" />.
        /// </summary>
        public CalcParser()
        {
            this.UpdateCulture(CultureInfo.InvariantCulture);
        }

        private CalcExpression BuildArraryExpression(CalcParserContext context, FormulaToken rootToken)
        {
            List<List<object>> list = new List<List<object>>();
            int count = -1;
            for (int i = 0; i < rootToken.Children.Count; i++)
            {
                FormulaToken token = rootToken.Children[i];
                FormulaToken token2 = (i == 0) ? null : rootToken.Children[i - 1];
                if (token.Type == ExcelFormulaTokenType.Argument)
                {
                    if (((token2 == null) || (token2.Type == ExcelFormulaTokenType.Argument)) || (i == (rootToken.Children.Count - 1)))
                    {
                        throw new CalcParseException("Exceptions.InvalidArray", token.Index);
                    }
                }
                else
                {
                    List<object> list2;
                    if ((token2 != null) && (token2.Type != ExcelFormulaTokenType.Argument))
                    {
                        throw new CalcParseException("Exceptions.InvalidArray", token.Index);
                    }
                    if ((token.Type != ExcelFormulaTokenType.Function) || (token.Value != "ARRAYROW"))
                    {
                        throw new CalcParseException("Exceptions.InvalidArray", token.Index);
                    }
                    if ((count != -1) && (token.Children.Count != count))
                    {
                        throw new CalcParseException("Exceptions.InvalidArrayLength", token.Index);
                    }
                    count = token.Children.Count;
                    list2 = new List<object>();
                    int num3 = token.Children.Count;
                    FormulaToken token3 = null;
                    for (int k = 0; k < num3; k++)
                    {
                        FormulaToken token4 = token.Children[k];
                        if (token4.Type == ExcelFormulaTokenType.Argument)
                        {
                            if (((token3 == null) || (token3.Type == ExcelFormulaTokenType.Argument)) || (k == (num3 - 1)))
                            {
                                list2.Add(CalcMissingArgument.Instance);
                            }
                        }
                        else
                        {
                            if ((token3 != null) && (token3.Type != ExcelFormulaTokenType.Argument))
                            {
                                throw new CalcParseException("Exceptions.InvalidArray", token.Index);
                            }
                            if (token4.Type == ExcelFormulaTokenType.OperatorPrefix)
                            {
                                if ((k + 1) >= num3)
                                {
                                    throw new CalcParseException("Exceptions.InvalidArray", token.Index);
                                }
                                FormulaToken token5 = token.Children[k + 1];
                                token5.Value = token4.Value + token5.Value;
                                token4 = token5;
                                k++;
                            }
                            else if (((k + 1) < num3) && (token.Children[k + 1].Type == ExcelFormulaTokenType.OperatorPostfix))
                            {
                                FormulaToken token6 = token.Children[k + 1];
                                token4.Value = token4.Value + token6.Value;
                                k++;
                            }
                            CalcConstantExpression expression = this.BuildExpressionNode(context, token4) as CalcConstantExpression;
                            if (expression == null)
                            {
                                throw new CalcParseException("Exceptions.InvalidArray", token.Index);
                            }
                            list2.Add(expression.Value);
                        }
                        token3 = token4;
                    }
                }
            }
            if (((list.Count == 0) || (list[0] == null)) || (list[0].Count == 0))
            {
                throw new CalcParseException("Exceptions.InvalidArray", rootToken.Index);
            }
            object[,] values = new object[list.Count, list[0].Count];
            for (int j = 0; j < list.Count; j++)
            {
                for (int m = 0; m < list[j].Count; m++)
                {
                    values[j, m] = list[j][m];
                }
            }
            return new CalcArrayExpression(values);
        }

        private CalcExpression BuildCellReferenceOrNameExpression(CalcParserContext context, string value, out int endIndex)
        {
            endIndex = 0;
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            int length = value.Length;
            if (value.StartsWith("#REF!"))
            {
                endIndex = length;
                return new CalcErrorExpression(CalcErrors.Reference);
            }
            bool isBang = false;
            int startIndex = 0;
            if (value[0] == '!')
            {
                isBang = true;
                startIndex = 1;
            }
            if (length < 1)
            {
                return null;
            }
            int num3 = -1;
            string str = "";
            if (isBang)
            {
                num3 = startIndex;
            }
            else if (value[0] == '\'')
            {
                num3 = value.IndexOf('!') + 1;
                str = value.Substring(0, num3 - 1);
                if (num3 < length)
                {
                }
            }
            else
            {
                int index = value.IndexOf('!');
                int num5 = value.IndexOf(':');
                if (index != -1)
                {
                    string str2 = value.Substring(startIndex, index - startIndex);
                    if ((num5 < 0) || (index < num5))
                    {
                        int num1 = length - 1;
                        str = str2;
                        num3 = index + 1;
                    }
                    else
                    {
                        CalcExpression expression = ParseToCellReference(context, str2, isBang, null, null, out endIndex);
                        if (expression != null)
                        {
                            return expression;
                        }
                        int num6 = length - 1;
                        str = str2;
                        num3 = index + 1;
                    }
                }
            }
            string workBookName = "";
            string startSheetName = "";
            string endSheetName = "";
            if ((num3 != -1) && !string.IsNullOrEmpty(str))
            {
                if (!this.ReadSheetReference(str, context.UseR1C1, false, out workBookName, out startSheetName, out endSheetName))
                {
                    return null;
                }
            }
            else
            {
                num3 = startIndex;
            }
            ICalcSource startCalcSource = null;
            ICalcSource endCalcSource = null;
            bool flag3 = false;
            if (!string.IsNullOrEmpty(workBookName) || !string.IsNullOrEmpty(startSheetName))
            {
                flag3 = true;
                startCalcSource = context.GetExternalSource(workBookName, startSheetName);
                if (startCalcSource == null)
                {
                    return new CalcErrorExpression(CalcErrors.Reference);
                }
            }
            if (!string.IsNullOrEmpty(endSheetName))
            {
                flag3 = true;
                endCalcSource = context.GetExternalSource(workBookName, endSheetName);
                if (endCalcSource == null)
                {
                    return new CalcErrorExpression(CalcErrors.Reference);
                }
            }
            string str6 = value.Substring(num3);
            CalcExpression expression2 = ParseToCellReference(context, str6, isBang, startCalcSource, endCalcSource, out endIndex);
            if (expression2 == null)
            {
                if (isBang)
                {
                    expression2 = new CalcBangNameExpression(str6);
                }
                else if (!string.IsNullOrEmpty(startSheetName))
                {
                    if (!string.IsNullOrEmpty(endSheetName))
                    {
                        return null;
                    }
                    ICalcSource externalSource = context.GetExternalSource(workBookName, startSheetName);
                    if (externalSource == null)
                    {
                        return new CalcErrorExpression(CalcErrors.Reference);
                    }
                    expression2 = new CalcExternalNameExpression(externalSource, str6);
                }
                else
                {
                    expression2 = new CalcNameExpression(str6);
                }
                endIndex = length;
            }
            else
            {
                if (((flag3 && !(expression2 is CalcSheetRangeExpression)) && (!(expression2 is CalcExternalExpression) && !(expression2 is CalcExternalErrorExpression))) && (!(expression2 is CalcSheetRangeErrorExpression) && !(expression2 is CalcExternalNameExpression)))
                {
                    endIndex = length;
                    return new CalcErrorExpression(CalcErrors.Reference);
                }
                if (endIndex <= (str6.Length - 1))
                {
                    endIndex = length - (str6.Length - endIndex);
                    if (value[endIndex] != ':')
                    {
                        if (!string.IsNullOrEmpty(startSheetName))
                        {
                            if (!string.IsNullOrEmpty(endSheetName))
                            {
                                return null;
                            }
                            ICalcSource source = context.GetExternalSource(workBookName, startSheetName);
                            if (source == null)
                            {
                                return new CalcErrorExpression(CalcErrors.Reference);
                            }
                            expression2 = new CalcExternalNameExpression(source, str6);
                        }
                        else
                        {
                            expression2 = new CalcNameExpression(str6);
                        }
                        endIndex = length;
                    }
                }
                else
                {
                    endIndex = length;
                }
            }
            if ((expression2 is CalcNameExpression) || (expression2 is CalcExternalNameExpression))
            {
                CalcStructReferenceExpression expression3 = CreateStructExpression(str6);
                if (expression3 != null)
                {
                    return expression3;
                }
                if (!ValidateName(str6))
                {
                    return null;
                }
            }
            return expression2;
        }

        private CalcExpression BuildCellReferenceOrNameExpressions(CalcParserContext context, FormulaToken token)
        {
            int num;
            string str = token.Value;
            CalcExpression item = this.BuildCellReferenceOrNameExpression(context, str, out num);
            if (item == null)
            {
                throw new CalcParseException("Exceptions.InvalidReference", token.Index);
            }
            if ((num > 0) && (num < str.Length))
            {
                if (str[num] != ':')
                {
                    throw new CalcParseException("Exceptions.InvalidReference", token.Index);
                }
                num++;
                List<CalcExpression> list = new List<CalcExpression> {
                    item
                };
                while ((num > 0) && (num < str.Length))
                {
                    int num2 = num;
                    string introduced6 = str.Substring(num);
                    item = this.BuildCellReferenceOrNameExpression(context, introduced6, out num);
                    if (item == null)
                    {
                        throw new CalcParseException("Exceptions.InvalidReference", token.Index);
                    }
                    list.Add(item);
                    num += num2;
                    if ((num < str.Length) && (str[num] != ':'))
                    {
                        throw new CalcParseException("Exceptions.InvalidReference", token.Index);
                    }
                    num++;
                }
                item = list[0];
                for (int i = 1; i < list.Count; i++)
                {
                    item = new CalcBinaryOperatorExpression(CalcBinaryOperators.Range, item, list[i]);
                }
            }
            return item;
        }

        private CalcExpression BuildExpressionNode(CalcParserContext context, FormulaToken token)
        {
            CalcExpression expression = null;
            if (token.Type == ExcelFormulaTokenType.Function)
            {
                if (token.Value == "ARRAY")
                {
                    return this.BuildArraryExpression(context, token);
                }
                return this.BuildFunctionExpression(context, token);
            }
            if (token.Type == ExcelFormulaTokenType.Subexpression)
            {
                return this.BuildSubExpression(context, token);
            }
            if (token.Type != ExcelFormulaTokenType.Operand)
            {
                return expression;
            }
            if (token.Subtype == ExcelFormulaTokenSubtype.Number)
            {
                return new CalcDoubleExpression(double.Parse(token.Value), token.Value);
            }
            if (token.Subtype == ExcelFormulaTokenSubtype.Error)
            {
                return new CalcErrorExpression(CalcErrors.Parse(token.Value, context.Culture));
            }
            if (token.Subtype == ExcelFormulaTokenSubtype.Logical)
            {
                if (token.Value.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                {
                    return new CalcBooleanExpression(true);
                }
                if (token.Value.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
                {
                    expression = new CalcBooleanExpression(false);
                }
                return expression;
            }
            if (token.Subtype == ExcelFormulaTokenSubtype.RangeOrName)
            {
                return this.BuildCellReferenceOrNameExpressions(context, token);
            }
            return new CalcStringExpression(token.Value);
        }

        private CalcExpression BuildExpressionTree(CalcParserContext context, List<FormulaToken> tokens)
        {
            List<object> list = this.PaseToBinaryOperatorList(context, tokens);
            int num = 3;
            while (num < list.Count)
            {
                FormulaToken token = list[num] as FormulaToken;
                FormulaToken token2 = list[num - 2] as FormulaToken;
                if ((token != null) && (token.Type == ExcelFormulaTokenType.OperatorInfix))
                {
                    while ((num >= 3) && (GetOpeatorPriority(token.Value) >= GetOpeatorPriority(token2.Value)))
                    {
                        CalcExpression left = list[num - 3] as CalcExpression;
                        CalcExpression right = list[num - 1] as CalcExpression;
                        CalcExpression expression3 = new CalcBinaryOperatorExpression(GetBinaryOperator(token2), left, right);
                        list.RemoveAt(num - 3);
                        list.RemoveAt(num - 3);
                        list.RemoveAt(num - 3);
                        list.Insert(num - 3, expression3);
                        num -= 2;
                        if (num >= 3)
                        {
                            token2 = list[num - 2] as FormulaToken;
                        }
                    }
                    num += 2;
                }
                else
                {
                    num++;
                }
            }
            if (list.Count == 1)
            {
                return (list[0] as CalcExpression);
            }
            CalcExpression item = null;
            for (int i = list.Count - 2; i > 0; i -= 2)
            {
                CalcExpression expression5 = list[i - 1] as CalcExpression;
                CalcExpression expression6 = list[i + 1] as CalcExpression;
                item = new CalcBinaryOperatorExpression(GetBinaryOperator(list[i] as FormulaToken), expression5, expression6);
                list.RemoveAt(i - 1);
                list.RemoveAt(i - 1);
                list.RemoveAt(i - 1);
                list.Add(item);
            }
            return item;
        }

        private CalcExpression BuildFunctionExpression(CalcParserContext context, FormulaToken rootToken)
        {
            List<CalcExpression> list = new List<CalcExpression>();
            List<FormulaToken> tokens = new List<FormulaToken>();
            for (int i = 0; i < rootToken.Children.Count; i++)
            {
                FormulaToken item = rootToken.Children[i];
                if (item.Type != ExcelFormulaTokenType.Argument)
                {
                    tokens.Add(item);
                }
                else if (tokens.Count == 0)
                {
                    list.Add(new CalcMissingArgumentExpression());
                }
                else
                {
                    list.Add(this.BuildExpressionTree(context, tokens));
                    tokens.Clear();
                }
            }
            if (tokens.Count != 0)
            {
                list.Add(this.BuildExpressionTree(context, tokens));
                tokens.Clear();
            }
            else if (rootToken.Children.Count != 0)
            {
                list.Add(new CalcMissingArgumentExpression());
            }
            CalcFunction calcFunction = GetFunction(rootToken.Value);
            if (calcFunction == null)
            {
                return new CalcFunctionExpression(rootToken.Value, list.ToArray());
            }
            if ((list.Count < calcFunction.MinArgs) || (list.Count > calcFunction.MaxArgs))
            {
                throw new CalcParseException("Exceptions.InvalidArray", rootToken.Index);
            }
            return new CalcFunctionExpression(calcFunction, list.ToArray());
        }

        private CalcExpression BuildSubExpression(CalcParserContext context, FormulaToken rootToken)
        {
            return new CalcParenthesesExpression(this.BuildExpressionTree(context, rootToken.Children));
        }

        private static CalcExpression CreateCellReferenceExpression(CalcParserContext context, int baseRowIndex, int baseColIndex, int startRow, int startColumn, int endRow, int endColumn, bool startRowRelative, bool startColumnRelative, bool endRowRelative, bool endColumnRelative, bool isBang, ICalcSource startCalcSource, ICalcSource endCalcSource)
        {
            startRow = (startRowRelative && (startRow != -2147483648)) ? (startRow - baseRowIndex) : startRow;
            startColumn = (startColumnRelative && (startColumn != -2147483648)) ? (startColumn - baseColIndex) : startColumn;
            endRow = (endRowRelative && (endRow != -2147483648)) ? (endRow - baseRowIndex) : endRow;
            endColumn = (endColumnRelative && (endColumn != -2147483648)) ? (endColumn - baseColIndex) : endColumn;
            if ((startCalcSource != null) && (endCalcSource != null))
            {
                if (startRow == -2147483648)
                {
                    return new CalcSheetRangeExpression(startCalcSource, endCalcSource, startColumn, endColumn, startColumnRelative, endColumnRelative, false);
                }
                if (startColumn == -2147483648)
                {
                    return new CalcSheetRangeExpression(startCalcSource, endCalcSource, startRow, endRow, startRowRelative, endRowRelative, true);
                }
                return new CalcSheetRangeExpression(startCalcSource, endCalcSource, startRow, startColumn, endRow, endColumn, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
            }
            if ((endRow == -2147483648) && (endColumn == -2147483648))
            {
                if (isBang)
                {
                    if ((startCalcSource == null) && (endCalcSource == null))
                    {
                        return new CalcBangCellExpression(startRow, startColumn, startRowRelative, startColumnRelative);
                    }
                    return null;
                }
                if (startCalcSource != null)
                {
                    return new CalcExternalCellExpression(startCalcSource, startRow, startColumn, startRowRelative, startColumnRelative);
                }
                return new CalcCellExpression(startRow, startColumn, startRowRelative, startColumnRelative);
            }
            if (isBang)
            {
                if ((startCalcSource != null) || (endCalcSource != null))
                {
                    return null;
                }
                if (startRow == -2147483648)
                {
                    return new CalcBangRangeExpression(startColumn, endColumn, startColumnRelative, endColumnRelative, false);
                }
                if (startColumn == -2147483648)
                {
                    return new CalcBangRangeExpression(startRow, endRow, startRowRelative, endRowRelative, true);
                }
                return new CalcBangRangeExpression(startRow, startColumn, endRow, endColumn, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
            }
            if (startCalcSource != null)
            {
                if (startRow == -2147483648)
                {
                    return new CalcExternalRangeExpression(startCalcSource, startColumn, endColumn, startColumnRelative, endColumnRelative, false);
                }
                if (startColumn == -2147483648)
                {
                    return new CalcExternalRangeExpression(startCalcSource, startRow, endRow, startRowRelative, endRowRelative, true);
                }
                return new CalcExternalRangeExpression(startCalcSource, startRow, startColumn, endRow, endColumn, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
            }
            if (startRow == -2147483648)
            {
                return new CalcRangeExpression(startColumn, endColumn, startColumnRelative, endColumnRelative, false);
            }
            if (startColumn == -2147483648)
            {
                return new CalcRangeExpression(startRow, endRow, startRowRelative, endRowRelative, true);
            }
            return new CalcRangeExpression(startRow, startColumn, endRow, endColumn, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
        }

        private static CalcExpression CreateExternalErrorExpression(CalcError error, bool isBang, ICalcSource startCalcSource, ICalcSource endCalcSource)
        {
            if (isBang)
            {
                return new CalcBangErrorExpression(error);
            }
            if ((startCalcSource != null) && (endCalcSource != null))
            {
                return new CalcSheetRangeErrorExpression(startCalcSource, endCalcSource, error);
            }
            if (startCalcSource != null)
            {
                return new CalcExternalErrorExpression(startCalcSource, error);
            }
            return new CalcErrorExpression(error);
        }

        private static CalcStructReferenceExpression CreateStructExpression(string value)
        {
            if (value[value.Length - 1] == ']')
            {
                return new CalcStructReferenceExpression(value);
            }
            return null;
        }

        private static Dictionary<string, CalcFunction> EnsureFunctions()
        {
            Dictionary<string, CalcFunction> dictionary = new Dictionary<string, CalcFunction>();
            IEnumerable<Type> exportedTypes = typeof(CalcFunction).GetTypeInfo().Assembly.ExportedTypes;
            TypeInfo typeInfo = typeof(CalcFunction).GetTypeInfo();
            foreach (Type type in exportedTypes)
            {
                TypeInfo info2 = type.GetTypeInfo();
                if ((info2.IsPublic && !info2.IsAbstract) && typeInfo.IsAssignableFrom(info2))
                {
                    ConstructorInfo info3 = null;
                    foreach (ConstructorInfo info4 in info2.DeclaredConstructors)
                    {
                        if ((info4.GetParameters().Length == 0) && !info4.IsStatic)
                        {
                            info3 = info4;
                            break;
                        }
                    }
                    if (info3 != null)
                    {
                        try
                        {
                            CalcFunction function = info3.Invoke(new object[0]) as CalcFunction;
                            dictionary.Add(function.Name, function);
                        }
                        catch
                        { }
                    }
                }
            }
            return dictionary;
        }

        private static CalcBinaryOperator GetBinaryOperator(FormulaToken token)
        {
            if (token.Subtype == ExcelFormulaTokenSubtype.RangeOp)
            {
                return CalcBinaryOperators.Range;
            }
            if (token.Subtype == ExcelFormulaTokenSubtype.Union)
            {
                return CalcBinaryOperators.Union;
            }
            if (token.Subtype == ExcelFormulaTokenSubtype.Intersection)
            {
                return CalcBinaryOperators.Intersection;
            }
            switch (token.Value)
            {
                case "^":
                    return CalcBinaryOperators.Exponent;

                case "*":
                    return CalcBinaryOperators.Multiply;

                case "/":
                    return CalcBinaryOperators.Divide;

                case "+":
                    return CalcBinaryOperators.Add;

                case "-":
                    return CalcBinaryOperators.Subtract;

                case "&":
                    return CalcBinaryOperators.Concatenate;

                case "<":
                    return CalcBinaryOperators.LessThan;

                case "=":
                    return CalcBinaryOperators.Equal;

                case ">":
                    return CalcBinaryOperators.GreaterThan;

                case ">=":
                    return CalcBinaryOperators.GreaterThanOrEqual;

                case "<=":
                    return CalcBinaryOperators.LessThanOrEqual;

                case "<>":
                    return CalcBinaryOperators.NotEqual;

                case ":":
                    return CalcBinaryOperators.Range;

                case "!":
                    return CalcBinaryOperators.Range;

                case " ":
                    return CalcBinaryOperators.Intersection;

                case ",":
                    return CalcBinaryOperators.Intersection;
            }
            return CalcBinaryOperators.Add;
        }

        private static CalcFunction GetFunction(string functionName)
        {
            CalcFunction function;
            if (!Functions.TryGetValue(functionName, out function))
            {
                return null;
            }
            return function;
        }

        private static int GetOpeatorPriority(string op)
        {
            switch (op)
            {
                case "^":
                case ":":
                    return 1;

                case "*":
                case "/":
                case " ":
                    return 2;

                case "+":
                case "-":
                case ",":
                    return 3;

                case "&":
                    return 4;

                case "<":
                case "=":
                case ">":
                case ">=":
                case "<=":
                case "<>":
                    return 5;
            }
            return 5;
        }

        internal string GetValidSource(string value, bool isR1C1)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string str;
                string str2;
                string str3;
                if (this.ReadSheetReference(value, isR1C1, true, out str, out str2, out str3))
                {
                    if ((value[0] == '\'') && (value[value.Length - 1] == '\''))
                    {
                        value = value.Substring(1, value.Length - 2);
                        value = value.Replace("'", "''");
                        value = "'" + value + "'";
                    }
                    return value;
                }
                if (this.ReadSheetReference("'" + value + "'", isR1C1, true, out str, out str2, out str3))
                {
                    value = value.Replace("'", "''");
                    value = "'" + value + "'";
                    return value;
                }
            }
            return string.Empty;
        }

        private static bool IsA1CellReferance(string value, out int startRow, out int startColumn, out int endRow, out int endColumn, out bool startRowRelative, out bool startColumnRelative, out bool endRowRelative, out bool endColumnRelative, out int endIndex)
        {
            int num2;
            bool flag;
            bool flag2;
            startRow = -2147483648;
            startColumn = -2147483648;
            endRow = -2147483648;
            endColumn = -2147483648;
            startRowRelative = true;
            startColumnRelative = true;
            endRowRelative = true;
            endColumnRelative = true;
            endIndex = 0;
            value = value.Replace(" ", "");
            int length = value.Length;
            if (!ReadOneA1Element(value, 0, out endIndex, out num2, out flag, out flag2))
            {
                return false;
            }
            if (endIndex >= length)
            {
                return false;
            }
            if (flag)
            {
                startRow = num2;
                startRowRelative = flag2;
                if (value[endIndex] != ':')
                {
                    return false;
                }
                endIndex++;
                if (!ReadOneA1Element(value, endIndex, out endIndex, out num2, out flag, out flag2))
                {
                    return false;
                }
                if (!flag)
                {
                    return false;
                }
                endRow = num2;
                endRowRelative = flag2;
                return true;
            }
            if (!flag)
            {
                startColumn = num2;
                startColumnRelative = flag2;
                if (value[endIndex] == ':')
                {
                    endIndex++;
                    if (!ReadOneA1Element(value, endIndex, out endIndex, out num2, out flag, out flag2))
                    {
                        return false;
                    }
                    if (flag)
                    {
                        return false;
                    }
                    endColumn = num2;
                    endColumnRelative = flag2;
                    return true;
                }
            }
            if (!char.IsNumber(value[endIndex]) && (value[endIndex] != '$'))
            {
                return false;
            }
            if (!ReadOneA1Element(value, endIndex, out endIndex, out num2, out flag, out flag2))
            {
                return false;
            }
            if (!flag)
            {
                return false;
            }
            startRow = num2;
            startRowRelative = flag2;
            int num3 = endIndex;
            if ((endIndex < (value.Length - 1)) && (value[endIndex] == ':'))
            {
                if (value[endIndex + 1] == '\'')
                {
                    return true;
                }
                int index = value.IndexOf('!', endIndex + 1);
                int num5 = value.IndexOf(':', endIndex + 1);
                if ((index != -1) && ((num5 == -1) || (num5 > index)))
                {
                    return true;
                }
                endIndex++;
                if (!ReadOneA1Element(value, endIndex, out endIndex, out num2, out flag, out flag2))
                {
                    endIndex = num3;
                    return true;
                }
                if (flag)
                {
                    endIndex = num3;
                    return true;
                }
                endColumn = num2;
                endColumnRelative = flag2;
                if (!ReadOneA1Element(value, endIndex, out endIndex, out num2, out flag, out flag2))
                {
                    endIndex = num3;
                    return true;
                }
                if (!flag)
                {
                    endIndex = num3;
                    return true;
                }
                endRow = num2;
                endRowRelative = flag2;
            }
            return true;
        }

        private static bool IsCellIndexsError(int baseRowIndex, int baseColIndex, int rowIndex, int colIndex, bool isRowRelative, bool isColRelative, CalcRangeType rangeType)
        {
            bool flag = false;
            switch (rangeType)
            {
                case CalcRangeType.Cell:
                    flag |= rowIndex < 0;
                    return (flag | (colIndex < 0));

                case CalcRangeType.Row:
                    return (flag | (rowIndex < 0));

                case CalcRangeType.Column:
                    return (flag | (colIndex < 0));

                case CalcRangeType.Sheet:
                    return flag;
            }
            return flag;
        }

        private static bool IsNumber(string s, int startIndex, char mumberDecimalSeparator, out int endIndex)
        {
            int length = s.Length;
            endIndex = startIndex;
            NumberState none = NumberState.None;
            for (int i = startIndex; i < length; i++)
            {
                char c = s[i];
                if (char.IsDigit(c))
                {
                    switch (none)
                    {
                        case NumberState.None:
                            none = NumberState.Int;
                            break;

                        case NumberState.Dot:
                            none = NumberState.Decimal;
                            break;

                        case NumberState.Sign:
                            none = NumberState.Int;
                            break;

                        case NumberState.Exponent:
                        case NumberState.SignExponent:
                            none = NumberState.ScientificNotation;
                            break;
                    }
                }
                else if (c == mumberDecimalSeparator)
                {
                    if (none == NumberState.Int)
                    {
                        none = NumberState.Decimal;
                    }
                    else
                    {
                        if ((none != NumberState.None) && (none != NumberState.Sign))
                        {
                            return false;
                        }
                        none = NumberState.Dot;
                    }
                }
                else
                {
                    if ((c == '+') || (c == '-'))
                    {
                        switch (none)
                        {
                            case NumberState.None:
                                {
                                    none = NumberState.Sign;
                                    continue;
                                }
                            case NumberState.Exponent:
                                {
                                    none = NumberState.SignExponent;
                                    continue;
                                }
                        }
                        endIndex = i - 1;
                        return true;
                    }
                    if ((c == 'E') || (c == 'e'))
                    {
                        if ((none != NumberState.Int) && (none != NumberState.Decimal))
                        {
                            return false;
                        }
                        none = NumberState.Exponent;
                    }
                    else
                    {
                        switch (none)
                        {
                            case NumberState.Int:
                            case NumberState.Decimal:
                            case NumberState.ScientificNotation:
                                endIndex = i - 1;
                                return true;
                        }
                    }
                }
            }
            if (((none != NumberState.Int) && (none != NumberState.Decimal)) && (none != NumberState.ScientificNotation))
            {
                return false;
            }
            endIndex = length - 1;
            return true;
        }

        private static bool IsR1C1CellReferance(string value, int baseRow, int baseColumn, out int startRow, out int startColumn, out int endRow, out int endColumn, out bool startRowRelative, out bool startColumnRelative, out bool endRowRelative, out bool endColumnRelative, out int endIndex)
        {
            int num2;
            bool flag;
            bool flag2;
            startRow = -2147483648;
            startColumn = -2147483648;
            endRow = -2147483648;
            endColumn = -2147483648;
            startRowRelative = true;
            startColumnRelative = true;
            endRowRelative = true;
            endColumnRelative = true;
            endIndex = 0;
            value = value.Replace(" ", "");
            int length = value.Length;
            if (!ReadOneR1C1Element(value, baseRow, baseColumn, 0, out endIndex, out num2, out flag, out flag2))
            {
                return false;
            }
            if (flag)
            {
                startRow = num2;
                startRowRelative = flag2;
                if (endIndex >= length)
                {
                    return true;
                }
                if (value[endIndex] == ':')
                {
                    endIndex++;
                    if (!ReadOneR1C1Element(value, baseRow, baseColumn, endIndex, out endIndex, out num2, out flag, out flag2))
                    {
                        return false;
                    }
                    if (!flag)
                    {
                        return false;
                    }
                    endRow = num2;
                    endRowRelative = flag2;
                    return true;
                }
            }
            if (!flag)
            {
                startColumn = num2;
                startColumnRelative = flag2;
                if (endIndex < length)
                {
                    if (value[endIndex] != ':')
                    {
                        return false;
                    }
                    endIndex++;
                    if (!ReadOneR1C1Element(value, baseRow, baseColumn, endIndex, out endIndex, out num2, out flag, out flag2))
                    {
                        return false;
                    }
                    if (flag)
                    {
                        return false;
                    }
                    endColumn = num2;
                    endColumnRelative = flag2;
                }
                return true;
            }
            if ((value[endIndex] != 'C') && (value[endIndex] != 'c'))
            {
                return false;
            }
            if (!ReadOneR1C1Element(value, baseRow, baseColumn, endIndex, out endIndex, out num2, out flag, out flag2))
            {
                return false;
            }
            if (flag)
            {
                return false;
            }
            startColumn = num2;
            startColumnRelative = flag2;
            int num3 = endIndex;
            if ((endIndex < (value.Length - 1)) && (value[endIndex] == ':'))
            {
                if (value[endIndex + 1] == '\'')
                {
                    return true;
                }
                int index = value.IndexOf('!', endIndex + 1);
                int num5 = value.IndexOf(':', endIndex + 1);
                if ((index != -1) && ((num5 == -1) || (num5 > index)))
                {
                    return true;
                }
                endIndex++;
                if (!ReadOneR1C1Element(value, baseRow, baseColumn, endIndex, out endIndex, out num2, out flag, out flag2))
                {
                    endIndex = num3;
                    return true;
                }
                if (!flag)
                {
                    endIndex = num3;
                    return true;
                }
                endRow = num2;
                endRowRelative = flag2;
                if (!ReadOneR1C1Element(value, baseRow, baseColumn, endIndex, out endIndex, out num2, out flag, out flag2))
                {
                    endIndex = num3;
                    return true;
                }
                if (flag)
                {
                    endIndex = num3;
                    return true;
                }
                endColumn = num2;
                endColumnRelative = flag2;
            }
            return true;
        }

        private static bool IsStartWithCellReference(string value, bool isR1C1, out int endIndex)
        {
            int num;
            int num2;
            bool flag;
            bool flag2;
            int num3;
            int num4;
            bool flag3;
            bool flag4;
            if (!isR1C1)
            {
                return IsA1CellReferance(value, out num, out num2, out num3, out num4, out flag, out flag2, out flag3, out flag4, out endIndex);
            }
            return IsR1C1CellReferance(value, 0, 0, out num, out num2, out num3, out num4, out flag, out flag2, out flag3, out flag4, out endIndex);
        }

        /// <summary>
        /// Parse a string formula to <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> with specified <see cref="T:Dt.CalcEngine.CalcParserContext" />.
        /// </summary>
        /// <param name="text">A string formula.</param>
        /// <param name="context">A <see cref="T:Dt.CalcEngine.CalcParserContext" /> indicates the setting for parser.</param>
        /// <returns>
        /// A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> indicate the expression tree.
        /// If <paramref name="text" /> is <see cref="F:System.String.Empty" /> or <see langword="null" />, return <see langword="null" />.
        /// </returns>
        /// <exception cref="T:Dt.CalcEngine.CalcParseException">There are some errors in string formula.</exception>
        public CalcExpression Parse(string text, CalcParserContext context)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            if (context == null)
            {
                context = new CalcParserContext(false, 0, 0, null);
            }
            this.UpdateCulture(context.Culture);
            List<FormulaToken> tokens = this.ParseToTokens(text);
            return this.BuildExpressionTree(context, tokens);
        }

        private static CalcExpression ParseToCellReference(CalcParserContext context, string value, bool isBang, ICalcSource startCalcSource, ICalcSource endCalcSource, out int endIndex)
        {
            bool flag;
            int num;
            int num2;
            bool flag2;
            bool flag3;
            int num3;
            int num4;
            bool flag4;
            bool flag5;
            if (value[0] == '#')
            {
                string str = ReadError(value, 0, out endIndex);
                endIndex++;
                CalcError error = CalcErrors.Parse(str, context.Culture);
                if (error != null)
                {
                    return CreateExternalErrorExpression(error, isBang, startCalcSource, endCalcSource);
                }
            }
            if (context.UseR1C1)
            {
                flag = IsR1C1CellReferance(value, context.Row, context.Column, out num, out num2, out num3, out num4, out flag2, out flag3, out flag4, out flag5, out endIndex);
            }
            else
            {
                flag = IsA1CellReferance(value, out num, out num2, out num3, out num4, out flag2, out flag3, out flag4, out flag5, out endIndex);
            }
            if (((flag && context.UseR1C1) && ((num != -2147483648) && (num2 == -2147483648))) && ((num3 == -2147483648) && (num4 == -2147483648)))
            {
                num3 = num;
                flag4 = flag2;
            }
            else if (((flag && context.UseR1C1) && ((num2 != -2147483648) && (num == -2147483648))) && ((num3 == -2147483648) && (num4 == -2147483648)))
            {
                num4 = num2;
                flag5 = flag3;
            }
            else if ((!flag || ((num == -2147483648) && ((num2 == -2147483648) || (num4 == -2147483648)))) || ((num2 == -2147483648) && (num3 == -2147483648)))
            {
                return null;
            }
            return CreateCellReferenceExpression(context, context.Row, context.Column, num, num2, num3, num4, flag2, flag3, flag4, flag5, isBang, startCalcSource, endCalcSource);
        }

        private List<FormulaToken> ParseToTokens(string formula)
        {
            int length = formula.Length;
            FormulaTokenList list = new FormulaTokenList();
            ExcelFormulaStack stack = new ExcelFormulaStack();
            StringBuilder builder = new StringBuilder();
            int index = 0;
            int num3 = 0;
            while ((num3 < length) && (formula[num3] == ' '))
            {
                num3++;
            }
            if (formula[num3] == '=')
            {
                num3++;
            }
            if (num3 == length)
            {
                ThrowParserError('=', num3 - 1);
            }
            for (int i = num3; i < length; i++)
            {
                char c = formula[i];
                switch (c)
                {
                    case '"':
                        {
                            int num5;
                            string str = ReadString(formula, i, '"', out num5);
                            list.Add(new FormulaToken(str, ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.Text, i));
                            i = num5;
                            index = i + 1;
                            break;
                        }
                    case '\'':
                        {
                            int num6;
                            string str2 = ReadString(formula, i, '\'', out num6);
                            builder.Append('\'');
                            builder.Append(str2);
                            builder.Append('\'');
                            i = num6;
                            break;
                        }
                    case '[':
                        {
                            int num7;
                            string str3 = ReadString(formula, i, '[', ']', out num7);
                            builder.Append("[");
                            builder.Append(str3);
                            builder.Append("]");
                            i = num7;
                            break;
                        }
                    case '\r':
                    case '\n':
                        break;

                    case '#':
                        {
                            int num8;
                            string str4 = ReadError(formula, i, out num8);
                            char ch2 = (i < length) ? formula[i + 1] : '\0';
                            if ((i > 0) && (formula[i - 1] == '!'))
                            {
                                builder.Append(str4);
                            }
                            else if (((CultureInfo.InvariantCulture.CompareInfo.Compare(str4, "#REF!", CompareOptions.IgnoreCase) == 0) && (i < length)) && (char.IsLetterOrDigit(ch2) || (ch2 == '$')))
                            {
                                builder.Append(str4);
                            }
                            else
                            {
                                list.Add(new FormulaToken(str4, ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.Error, i));
                                index = i + 1;
                            }
                            i = num8;
                            break;
                        }
                    case '+':
                    case '-':
                        {
                            FormulaToken token = list[list.Count - 1];
                            if (builder.Length != 0)
                            {
                                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                list.Add(new FormulaToken(((char)c).ToString(), ExcelFormulaTokenType.OperatorInfix, i));
                                builder.Clear();
                                index = i + 1;
                            }
                            else
                            {
                                if ((token != null) && (token.Type == ExcelFormulaTokenType.Whitespace))
                                {
                                    list.RemoveAt(list.Count - 1);
                                    token = list[list.Count - 1];
                                }
                                if ((token != null) && ((((token.Type == ExcelFormulaTokenType.Function) && (token.Subtype == ExcelFormulaTokenSubtype.Stop)) || ((token.Type == ExcelFormulaTokenType.Subexpression) && (token.Subtype == ExcelFormulaTokenSubtype.Stop))) || ((token.Type == ExcelFormulaTokenType.OperatorPostfix) || (token.Type == ExcelFormulaTokenType.Operand))))
                                {
                                    list.Add(new FormulaToken(((char)c).ToString(), ExcelFormulaTokenType.OperatorInfix, i));
                                    index = i + 1;
                                }
                                else
                                {
                                    list.Add(new FormulaToken(((char)c).ToString(), ExcelFormulaTokenType.OperatorPrefix, i));
                                    index = i + 1;
                                }
                            }
                            break;
                        }
                    default:
                        if ((c == this._mumberDecimalSeparator) || char.IsDigit(c))
                        {
                            if (builder.Length > 0)
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                int num9;
                                if (IsNumber(formula, i, this._mumberDecimalSeparator, out num9))
                                {
                                    string str5 = formula.Substring(i, (num9 - i) + 1);
                                    while ((num9 <= (length - 2)) && (formula[num9 + 1] == ' '))
                                    {
                                        num9++;
                                    }
                                    if ((num9 <= (length - 2)) && (formula[num9 + 1] == ':'))
                                    {
                                        builder.Append(str5);
                                        builder.Append(":");
                                        num9++;
                                    }
                                    else
                                    {
                                        list.Add(new FormulaToken(str5, ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.Number, i));
                                    }
                                    index = i + 1;
                                    i = num9;
                                }
                                else
                                {
                                    builder.Append(c);
                                }
                            }
                        }
                        else if (c == '{')
                        {
                            if (builder.Length > 0)
                            {
                                ThrowParserError('{', i);
                            }
                            stack.Push(list.Add(new FormulaToken("ARRAY", ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start, i)));
                            stack.Push(list.Add(new FormulaToken("ARRAYROW", ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start, i)));
                            index = i + 1;
                        }
                        else if (((c == this._arraryGroupSeparator) && (stack.Current != null)) && ((stack.Current.Value == "ARRAY") || (stack.Current.Value == "ARRAYROW")))
                        {
                            if (builder.Length > 0)
                            {
                                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                builder.Clear();
                            }
                            if (stack.Current == null)
                            {
                                ThrowParserError(c, i);
                            }
                            list.Add(stack.Pop());
                            list.Add(new FormulaToken(((char)this._listSeparator).ToString(), ExcelFormulaTokenType.Argument, i));
                            stack.Push(list.Add(new FormulaToken("ARRAYROW", ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start, i + 1)));
                            index = i + 1;
                        }
                        else if (c == '}')
                        {
                            if (builder.Length > 0)
                            {
                                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                builder.Clear();
                            }
                            if (stack.Current == null)
                            {
                                ThrowParserError(c, i);
                            }
                            list.Add(stack.Pop());
                            list.Add(stack.Pop());
                            index = i + 1;
                        }
                        else if (c == ' ')
                        {
                            int num10 = i;
                            i++;
                            while ((i < length) && (formula[i] == ' '))
                            {
                                i++;
                            }
                            if (((builder.Length > 0) && (builder[builder.Length - 1] != ':')) && ((i < (length - 1)) && (formula[i] != ':')))
                            {
                                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                builder.Clear();
                                list.Add(new FormulaToken("", ExcelFormulaTokenType.Whitespace, num10));
                                index = i;
                            }
                            i--;
                        }
                        else if (((i + 2) <= length) && ((((c == '<') && (formula[i + 1] == '=')) || ((c == '>') && (formula[i + 1] == '='))) || ((c == '<') && (formula[i + 1] == '>'))))
                        {
                            char ch1 = formula[i + 1];
                            if (builder.Length > 0)
                            {
                                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                builder.Clear();
                            }
                            list.Add(new FormulaToken(formula.Substring(i, 2), ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Logical, i));
                            i++;
                            index = i + 1;
                        }
                        else
                        {
                            switch (c)
                            {
                                case '%':
                                    {
                                        if (builder.Length > 0)
                                        {
                                            list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                            builder.Clear();
                                        }
                                        char ch4 = formula[i];
                                        list.Add(new FormulaToken(((char)ch4).ToString(), ExcelFormulaTokenType.OperatorPostfix, i));
                                        index = i + 1;
                                        continue;
                                    }
                                case '+':
                                case '-':
                                case '*':
                                case '/':
                                case '=':
                                case '>':
                                case '<':
                                case '&':
                                case '^':
                                    {
                                        if (builder.Length > 0)
                                        {
                                            list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                            builder.Clear();
                                        }
                                        list.Add(new FormulaToken(((char)c).ToString(), ExcelFormulaTokenType.OperatorInfix, i));
                                        index = i + 1;
                                        continue;
                                    }
                                case '(':
                                    {
                                        if (builder.Length > 0)
                                        {
                                            char ch3 = builder[builder.Length - 1];
                                            if (((ch3 == ':') || (ch3 == this._listSeparator)) || (ch3 == ' '))
                                            {
                                                builder.Remove(builder.Length - 1, 1);
                                                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.RangeOrName, index));
                                                list.Add(new FormulaToken(((char)ch3).ToString(), ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Nothing, i - 1));
                                                stack.Push(list.Add(new FormulaToken("", ExcelFormulaTokenType.Subexpression, ExcelFormulaTokenSubtype.Start, i)));
                                            }
                                            else
                                            {
                                                int num11 = builder.ToString().IndexOf(':');
                                                string str6 = ":";
                                                if (num11 == -1)
                                                {
                                                    num11 = builder.ToString().IndexOf(this._listSeparator);
                                                    str6 = ((char)this._listSeparator).ToString();
                                                }
                                                if (num11 == -1)
                                                {
                                                    num11 = builder.ToString().IndexOf(' ');
                                                    str6 = " ";
                                                }
                                                if ((num11 != -1) && (num11 > 0))
                                                {
                                                    list.Add(new FormulaToken(builder.ToString().Substring(0, num11), ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.RangeOrName, index));
                                                    list.Add(new FormulaToken(str6, ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Nothing, num11));
                                                    builder.Remove(0, num11 + 1);
                                                    stack.Push(list.Add(new FormulaToken(builder.ToString().ToUpper(CultureInfo.CurrentCulture), ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start, i)));
                                                }
                                                else
                                                {
                                                    stack.Push(list.Add(new FormulaToken(builder.ToString().ToUpper(CultureInfo.CurrentCulture), ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start, i)));
                                                }
                                            }
                                            builder.Clear();
                                        }
                                        else
                                        {
                                            stack.Push(list.Add(new FormulaToken("", ExcelFormulaTokenType.Subexpression, ExcelFormulaTokenSubtype.Start, i)));
                                        }
                                        index = i + 1;
                                        continue;
                                    }
                            }
                            if ((c == this._listSeparator) || (c == this._arraryArgSeparator))
                            {
                                if (builder.Length > 0)
                                {
                                    list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                    builder.Clear();
                                }
                                if ((stack.Current == null) || (stack.Current.Type != ExcelFormulaTokenType.Function))
                                {
                                    list.Add(new FormulaToken(((char)this._listSeparator).ToString(), ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Union, i));
                                }
                                else
                                {
                                    list.Add(new FormulaToken(((char)this._arraryArgSeparator).ToString(), ExcelFormulaTokenType.Argument, i));
                                }
                                index = i + 1;
                            }
                            else
                            {
                                switch (c)
                                {
                                    case ')':
                                        {
                                            if (builder.Length > 0)
                                            {
                                                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
                                                builder.Clear();
                                            }
                                            if (stack.Current == null)
                                            {
                                                ThrowParserError(c, i);
                                            }
                                            list.Add(stack.Pop());
                                            index = i + 1;
                                            continue;
                                        }
                                    case ':':
                                        {
                                            if ((builder.Length == 0) && (list[list.Count - 1].Subtype == ExcelFormulaTokenSubtype.Stop))
                                            {
                                                list.Add(new FormulaToken(":", ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.RangeOp, i));
                                                index = i + 1;
                                            }
                                            else
                                            {
                                                builder.Append(":");
                                            }
                                            continue;
                                        }
                                }
                                builder.Append(c);
                            }
                        }
                        break;
                }
            }
            if (builder.Length > 0)
            {
                list.Add(new FormulaToken(builder.ToString(), ExcelFormulaTokenType.Operand, index));
            }
            return ProcessTokens(list);
        }

        private List<object> PaseToBinaryOperatorList(CalcParserContext context, List<FormulaToken> tokens)
        {
            List<object> list = new List<object>();
            CalcExpression operand = null;
            for (int i = 0; i < tokens.Count; i++)
            {
                FormulaToken item = tokens[i];
                if (item.Type == ExcelFormulaTokenType.OperatorPrefix)
                {
                    Stack<CalcUnaryOperator> stack = new Stack<CalcUnaryOperator>();
                    while (item.Type == ExcelFormulaTokenType.OperatorPrefix)
                    {
                        stack.Push((item.Value == "+") ? CalcUnaryOperators.Plus : CalcUnaryOperators.Negate);
                        i++;
                        item = tokens[i];
                    }
                    FormulaToken token = tokens[i];
                    operand = new CalcUnaryOperatorExpression(stack.Pop(), this.BuildExpressionNode(context, token));
                    while (stack.Count > 0)
                    {
                        operand = new CalcUnaryOperatorExpression(stack.Pop(), operand);
                    }
                    list.Add(operand);
                }
                else if (item.Type == ExcelFormulaTokenType.OperatorPostfix)
                {
                    int count = list.Count;
                    CalcExpression expression2 = list[list.Count - 1] as CalcExpression;
                    operand = new CalcUnaryOperatorExpression(CalcUnaryOperators.Percent, expression2);
                    list.RemoveAt(list.Count - 1);
                    list.Add(operand);
                }
                else if (item.Type == ExcelFormulaTokenType.OperatorInfix)
                {
                    list.Add(item);
                }
                else
                {
                    operand = this.BuildExpressionNode(context, item);
                    list.Add(operand);
                }
            }
            return list;
        }

        private static List<FormulaToken> ProcessTokens(FormulaTokenList tokens1)
        {
            FormulaTokenList list = RemoveWhiteSpace(tokens1);
            Stack<FormulaToken> stack = new Stack<FormulaToken>();
            FormulaToken item = new FormulaToken("", ExcelFormulaTokenType.Unknown, ExcelFormulaTokenSubtype.Start, 0);
            stack.Push(item);
            while (list.MoveNext())
            {
                FormulaToken current = list.Current;
                if (current == null)
                {
                    continue;
                }
                FormulaToken previous = list.Previous;
                FormulaToken next = list.Next;
                if ((current.Type == ExcelFormulaTokenType.Operand) && (current.Subtype == ExcelFormulaTokenSubtype.Nothing))
                {
                    string str = current.Value.ToUpper(CultureInfo.CurrentCulture);
                    switch (str)
                    {
                        case "TRUE":
                        case "FALSE":
                            current.Subtype = ExcelFormulaTokenSubtype.Logical;
                            current.Value = str;
                            goto Label_00E2;
                    }
                    current.Subtype = ExcelFormulaTokenSubtype.RangeOrName;
                }
                else if (((current.Type == ExcelFormulaTokenType.Function) && (current.Value.Length > 0)) && (current.Value[0] == '@'))
                {
                    current.Value = current.Value.Substring(1);
                }
            Label_00E2:
                if (stack.Count == 0)
                {
                    throw new CalcParseException("Exceptions.ParserError", current.Index);
                }
                FormulaToken token2 = stack.Peek();
                if ((((token2.Value == "ARRAYROW") && (current.Type != ExcelFormulaTokenType.Argument)) && ((current.Subtype != ExcelFormulaTokenSubtype.Error) && (current.Subtype != ExcelFormulaTokenSubtype.Stop))) && (((current.Subtype != ExcelFormulaTokenSubtype.Logical) && (current.Subtype != ExcelFormulaTokenSubtype.Number)) && ((current.Subtype != ExcelFormulaTokenSubtype.Text) && (current.Type != ExcelFormulaTokenType.OperatorPrefix))))
                {
                    throw new CalcParseException("Exceptions.ParserError", current.Index);
                }
                switch (current.Type)
                {
                    case ExcelFormulaTokenType.Operand:
                        if ((previous != null) && ((((previous.Type == ExcelFormulaTokenType.Operand) || (previous.Type == ExcelFormulaTokenType.OperatorPostfix)) || ((previous.Type == ExcelFormulaTokenType.Function) && (previous.Subtype == ExcelFormulaTokenSubtype.Stop))) || ((previous.Type == ExcelFormulaTokenType.Subexpression) && (previous.Subtype == ExcelFormulaTokenSubtype.Stop))))
                        {
                            ThrowParserError(current);
                        }
                        break;

                    case ExcelFormulaTokenType.Function:
                    case ExcelFormulaTokenType.Subexpression:
                        if ((((current.Value != "ARRAY") || (current.Type != ExcelFormulaTokenType.Function)) || ((current.Subtype != ExcelFormulaTokenSubtype.Start) || (previous != null))) && (((current.Subtype == ExcelFormulaTokenSubtype.Stop) && (((previous == null) || (previous.Type == ExcelFormulaTokenType.OperatorPrefix)) || (previous.Type == ExcelFormulaTokenType.OperatorInfix))) || (((current.Subtype == ExcelFormulaTokenSubtype.Start) && (previous != null)) && (((next == null) || (previous.Type == ExcelFormulaTokenType.OperatorPostfix)) || (previous.Subtype == ExcelFormulaTokenSubtype.Stop)))))
                        {
                            ThrowParserError(current);
                        }
                        if (((current.Subtype == ExcelFormulaTokenSubtype.Stop) && (current.Type == ExcelFormulaTokenType.Subexpression)) && (previous.Subtype == ExcelFormulaTokenSubtype.Start))
                        {
                            ThrowParserError(current);
                        }
                        if (((current.Subtype == ExcelFormulaTokenSubtype.Stop) && (current.Type == ExcelFormulaTokenType.Function)) && ((previous.Type == ExcelFormulaTokenType.Subexpression) && (previous.Subtype == ExcelFormulaTokenSubtype.Start)))
                        {
                            ThrowParserError(current);
                        }
                        break;

                    case ExcelFormulaTokenType.Argument:
                        if (((next == null) || (previous == null)) || ((previous.Type == ExcelFormulaTokenType.OperatorInfix) || (previous.Type == ExcelFormulaTokenType.OperatorPrefix)))
                        {
                            ThrowParserError(current);
                        }
                        break;

                    case ExcelFormulaTokenType.OperatorPrefix:
                        if ((next == null) || ((previous != null) && (previous.Type == ExcelFormulaTokenType.OperatorPostfix)))
                        {
                            ThrowParserError(current);
                        }
                        break;

                    case ExcelFormulaTokenType.OperatorInfix:
                        if (((((next == null) || (previous == null)) || ((previous.Type == ExcelFormulaTokenType.OperatorInfix) || (previous.Type == ExcelFormulaTokenType.OperatorPrefix))) || ((previous.Type == ExcelFormulaTokenType.Argument) || ((previous.Type == ExcelFormulaTokenType.Function) && (previous.Subtype == ExcelFormulaTokenSubtype.Start)))) || ((previous.Type == ExcelFormulaTokenType.Subexpression) && (previous.Subtype == ExcelFormulaTokenSubtype.Start)))
                        {
                            ThrowParserError(current);
                        }
                        break;

                    case ExcelFormulaTokenType.OperatorPostfix:
                        if (((((previous == null) || (previous.Type == ExcelFormulaTokenType.OperatorPrefix)) || (previous.Type == ExcelFormulaTokenType.OperatorInfix)) || ((previous.Type == ExcelFormulaTokenType.Function) && (previous.Subtype == ExcelFormulaTokenSubtype.Start))) || ((previous.Type == ExcelFormulaTokenType.Subexpression) && (previous.Subtype == ExcelFormulaTokenSubtype.Start)))
                        {
                            ThrowParserError(current);
                        }
                        break;

                    default:
                        ThrowParserError(current);
                        break;
                }
                if (current.Subtype == ExcelFormulaTokenSubtype.Start)
                {
                    stack.Push(current);
                    token2.Children.Add(current);
                }
                else
                {
                    if (current.Subtype == ExcelFormulaTokenSubtype.Stop)
                    {
                        if (stack.Count == 0)
                        {
                            char ch;
                            if ((current.Value == "ARRAY") || (current.Value == "ARRAYROW"))
                            {
                                ch = '}';
                            }
                            else
                            {
                                ch = ')';
                            }
                            ThrowParserError(ch, current.Index);
                        }
                        stack.Pop();
                        continue;
                    }
                    token2.Children.Add(current);
                }
            }
            return item.Children;
        }

        private static string ReadError(string formula, int startIndex, out int endIndex)
        {
            int length = formula.Length;
            new StringBuilder();
            int num2 = length - startIndex;
            foreach (string str in ERRORS)
            {
                if ((startIndex + str.Length) <= formula.Length)
                {
                    string str2 = formula.Substring(startIndex, str.Length);
                    if ((str.Length <= num2) && ((str == str2) || (str == str2.ToUpper())))
                    {
                        endIndex = (startIndex + str.Length) - 1;
                        return str;
                    }
                }
            }
            throw new CalcParseException("Exceptions.InvalidError", startIndex);
        }

        private static bool ReadOneA1Element(string value, int startIndex, out int endIndex, out int elementIndex, out bool isRow, out bool isRelative)
        {
            endIndex = startIndex;
            elementIndex = -2147483648;
            isRow = true;
            isRelative = true;
            int length = value.Length;
            if (startIndex < length)
            {
                if (value[startIndex] == '$')
                {
                    isRelative = false;
                    startIndex++;
                }
                if (startIndex >= length)
                {
                    return false;
                }
                int num2 = startIndex;
                char c = value[num2];
                if (char.IsNumber(c) && (c != '0'))
                {
                    int num3;
                    isRow = true;
                    while ((num2 < length) && char.IsNumber(c))
                    {
                        num2++;
                        if (num2 < length)
                        {
                            c = value[num2];
                        }
                    }
                    if ((int.TryParse(value.Substring(startIndex, num2 - startIndex), out num3) && (num3 >= 1)) && (num3 <= 0x100000))
                    {
                        elementIndex = num3 - 1;
                        endIndex = num2;
                        return true;
                    }
                }
                else if (char.IsLetter(c))
                {
                    isRow = false;
                    while ((num2 < length) && char.IsLetter(c))
                    {
                        num2++;
                        if (num2 < length)
                        {
                            c = value[num2];
                        }
                    }
                    string str = value.Substring(startIndex, num2 - startIndex);
                    if (str.Length > 3)
                    {
                        return false;
                    }
                    str = str.ToUpper();
                    int num4 = 0;
                    for (int i = str.Length - 1; i >= 0; i--)
                    {
                        num4 += ((str[i] - 'A') + 1) * LetterPows[(str.Length - i) - 1];
                    }
                    if (num4 <= 0x4000)
                    {
                        elementIndex = num4 - 1;
                        endIndex = num2;
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool ReadOneR1C1Element(string value, int baseRow, int baseColumn, int startIndex, out int endIndex, out int elementIndex, out bool isRow, out bool isRelative)
        {
            int num2;
            int num3;
            endIndex = startIndex;
            elementIndex = -2147483648;
            isRow = true;
            isRelative = false;
            int length = value.Length;
            if (startIndex < length)
            {
                num2 = startIndex;
                switch (value[num2])
                {
                    case 'R':
                    case 'r':
                        isRow = true;
                        num3 = baseRow;
                        goto Label_0051;

                    case 'C':
                    case 'c':
                        isRow = false;
                        num3 = baseColumn;
                        goto Label_0051;
                }
            }
            return false;
        Label_0051:
            startIndex++;
            num2++;
            if (startIndex >= length)
            {
                endIndex = startIndex;
                elementIndex = num3;
                isRelative = true;
                return true;
            }
            char c = value[num2];
            if (c == '[')
            {
                startIndex++;
                num2++;
                isRelative = true;
            }
            if (startIndex >= length)
            {
                return false;
            }
            bool flag = false;
            c = value[num2];
            if (isRelative && (c == '-'))
            {
                startIndex++;
                num2++;
                flag = true;
            }
            if (startIndex >= length)
            {
                return false;
            }
            c = value[num2];
            if (char.IsNumber(c))
            {
                int num4;
                while ((num2 < length) && char.IsNumber(c))
                {
                    num2++;
                    if (num2 < length)
                    {
                        c = value[num2];
                    }
                }
                bool flag2 = int.TryParse(value.Substring(startIndex, num2 - startIndex), out num4);
                if (isRelative)
                {
                    if ((num2 >= length) || (value[num2] != ']'))
                    {
                        return false;
                    }
                    num2++;
                }
                if (!flag2 || (num4 > (isRow ? 0x100000 : 0x4000)))
                {
                    return false;
                }
                if (isRelative)
                {
                    num4 = flag ? -num4 : num4;
                    elementIndex = num4 + num3;
                }
                else
                {
                    elementIndex = num4 - 1;
                }
                endIndex = num2;
                return true;
            }
            if (isRelative)
            {
                return false;
            }
            endIndex = startIndex;
            elementIndex = num3;
            isRelative = true;
            return true;
        }

        private bool ReadSheetReference(string value, bool isR1C1, bool isUnparse, out string workBookName, out string startSheetName, out string endSheetName)
        {
            workBookName = "";
            startSheetName = "";
            endSheetName = "";
            bool containsSpecial = false;
            if ((value[0] == '\'') && (value[value.Length - 1] == '\''))
            {
                containsSpecial = true;
                value = value.Substring(1, value.Length - 2);
            }
            int index = value.IndexOf('[');
            if (index != -1)
            {
                int num2;
                if (index != 0)
                {
                    workBookName = value.Substring(0, index);
                    if (workBookName[index - 1] != '\\')
                    {
                        workBookName = workBookName + ((char)'\\');
                    }
                }
                workBookName = workBookName + ReadString(value, index, '[', ']', out num2);
                value = value.Substring(num2 + 1);
            }
            int length = value.IndexOf(':');
            if (length == -1)
            {
                startSheetName = value;
            }
            else
            {
                startSheetName = value.Substring(0, length);
                endSheetName = value.Substring(length + 1);
            }
            if (string.IsNullOrEmpty(startSheetName))
            {
                return false;
            }
            bool flag2 = this.ValidateWorkbook(workBookName, containsSpecial);
            flag2 = this.ValidateSheetName(startSheetName, isR1C1, containsSpecial) & this.ValidateSheetName(endSheetName, isR1C1, containsSpecial);
            if (isUnparse)
            {
                startSheetName = startSheetName.Replace("'", "''");
                endSheetName = endSheetName.Replace("'", "''");
            }
            return flag2;
        }

        private static string ReadString(string formula, int startIndex, char sign, out int endIndex)
        {
            return ReadString(formula, startIndex, sign, sign, out endIndex);
        }

        private static string ReadString(string formula, int startIndex, char startSign, char endSign, out int endIndex)
        {
            int length = formula.Length;
            StringBuilder builder = new StringBuilder();
            int num2 = (startSign == endSign) ? 0 : 1;
            for (int i = startIndex + 1; i < length; i++)
            {
                char ch = formula[i];
                if (ch == startSign)
                {
                    num2++;
                }
                if (ch == endSign)
                {
                    num2--;
                    if (((startSign != endSign) || ((i + 2) > length)) || (formula[i + 1] != startSign))
                    {
                        if (num2 == 0)
                        {
                            endIndex = i;
                            return builder.ToString();
                        }
                        builder.Append(ch);
                    }
                    else
                    {
                        builder.Append(startSign);
                        i++;
                    }
                }
                else
                {
                    builder.Append(ch);
                }
            }
            throw new CalcParseException("Exceptions.ParserErrorNotMatch", startIndex);
        }

        private static bool RemoveApostrophe(StringBuilder sb)
        {
            int length = sb.Length;
            if (sb.ToString()[length - 1] == '\'')
            {
                sb.Remove(length - 1, 1);
                sb.Remove(0, 1);
                return true;
            }
            return false;
        }

        private static FormulaTokenList RemoveWhiteSpace(FormulaTokenList tokens1)
        {
            FormulaTokenList list = new FormulaTokenList();
            while (tokens1.MoveNext())
            {
                FormulaToken current = tokens1.Current;
                if (current != null)
                {
                    if (current.Type != ExcelFormulaTokenType.Whitespace)
                    {
                        list.Add(current);
                    }
                    else if (!tokens1.BOF && !tokens1.EOF)
                    {
                        FormulaToken previous = tokens1.Previous;
                        FormulaToken next = tokens1.Next;
                        if ((((previous != null) && (next != null)) && ((((previous.Type == ExcelFormulaTokenType.Function) && (previous.Subtype == ExcelFormulaTokenSubtype.Stop)) || ((previous.Type == ExcelFormulaTokenType.Subexpression) && (previous.Subtype == ExcelFormulaTokenSubtype.Stop))) || (previous.Type == ExcelFormulaTokenType.Operand))) && ((((next.Type == ExcelFormulaTokenType.Function) && (next.Subtype == ExcelFormulaTokenSubtype.Start)) || ((next.Type == ExcelFormulaTokenType.Subexpression) && (next.Subtype == ExcelFormulaTokenSubtype.Start))) || (next.Type == ExcelFormulaTokenType.Operand)))
                        {
                            list.Add(new FormulaToken(" ", ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Intersection, current.Index));
                        }
                    }
                }
            }
            return list;
        }

        private static bool RemoveWorkbook(StringBuilder sb, out string workBookName)
        {
            if (sb[0] != '[')
            {
                workBookName = "";
                return false;
            }
            string str = sb.ToString();
            int index = str.IndexOf(']');
            sb.Remove(0, index + 1);
            workBookName = str.Substring(0, index + 1);
            return true;
        }

        private static void ThrowParserError(FormulaToken token)
        {
            throw new CalcParseException("Exceptions.ParserErrorToken", token.Index);
        }

        private static void ThrowParserError(char errorChar, int index)
        {
            throw new CalcParseException("Exceptions.ParserError", index);
        }

        /// <summary>
        /// Unparse a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> to string with specified <see cref="T:Dt.CalcEngine.CalcParserContext" />.
        /// </summary>
        /// <param name="expr">A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> indicate the expression tree.</param>
        /// <param name="context">A <see cref="T:Dt.CalcEngine.CalcParserContext" /> indicates the setting for parser.</param>
        /// <returns>
        /// The string formula.
        /// If <paramref name="expr" /> is <see langword="null" />, return <see cref="F:System.String.Empty" />
        /// </returns>
        public string Unparse(CalcExpression expr, CalcParserContext context)
        {
            if (expr == null)
            {
                return string.Empty;
            }
            if (context == null)
            {
                context = new CalcParserContext(false, 0, 0, null);
            }
            this.UpdateCulture(context.Culture);
            StringBuilder sb = new StringBuilder();
            this.UnparseExpression(expr, context, sb);
            return sb.ToString();
        }

        internal static void UnParseCell(bool useR1C1, int baseRowIndex, int baseColIndex, int rowIndex, int colIndex, bool isRowRelative, bool isColRelative, StringBuilder sb, CalcRangeType rangeType = 0)
        {
            rowIndex = isRowRelative ? (rowIndex + baseRowIndex) : rowIndex;
            colIndex = isColRelative ? (colIndex + baseColIndex) : colIndex;
            if (((rangeType == CalcRangeType.Cell) || (rangeType == CalcRangeType.Row)) && (((rowIndex != -2147483648) && (rowIndex > -1048575)) && (rowIndex <= 0x1ffffe)))
            {
                rowIndex = CalcHelper.NormalizeRowIndex(rowIndex, 0xfffff);
            }
            if (((rangeType == CalcRangeType.Cell) || (rangeType == CalcRangeType.Column)) && (((colIndex != -2147483648) && (colIndex > -16383)) && (colIndex <= 0x7ffe)))
            {
                colIndex = CalcHelper.NormalizeColumnIndex(colIndex, 0x3fff);
            }
            if (IsCellIndexsError(baseRowIndex, baseColIndex, rowIndex, colIndex, isRowRelative, isColRelative, rangeType))
            {
                sb.Append(CalcErrors.Reference.ToString());
            }
            else
            {
                string str = "";
                string str2 = "";
                if (useR1C1)
                {
                    str = "R";
                    if ((rowIndex >= 0) && ((baseRowIndex != rowIndex) || !isRowRelative))
                    {
                        if (isRowRelative)
                        {
                            int num4 = rowIndex - baseRowIndex;
                            str = str + "[" + ((int)num4).ToString() + "]";
                        }
                        else
                        {
                            rowIndex++;
                            str = str + ((int)rowIndex).ToString();
                        }
                    }
                    if (colIndex < 0)
                    {
                        sb.Append(str);
                    }
                    else
                    {
                        str2 = "C";
                        if ((baseColIndex != colIndex) || !isColRelative)
                        {
                            if (isColRelative)
                            {
                                int num5 = colIndex - baseColIndex;
                                str2 = str2 + "[" + ((int)num5).ToString() + "]";
                            }
                            else
                            {
                                colIndex++;
                                str2 = str2 + ((int)colIndex).ToString();
                            }
                        }
                        if ((isRowRelative && (rowIndex < 0)) || (!isRowRelative && (rowIndex <= 0)))
                        {
                            sb.Append(str2);
                        }
                        else
                        {
                            sb.Append(str);
                            sb.Append(str2);
                        }
                    }
                }
                else
                {
                    rowIndex++;
                    str = ((int)rowIndex).ToString();
                    if (!isRowRelative)
                    {
                        str = "$" + str;
                    }
                    if (colIndex < 0)
                    {
                        sb.Append(str);
                    }
                    else
                    {
                        int num = colIndex;
                        for (int i = 1; i < LetterPowsForUnParse.Length; i++)
                        {
                            colIndex = 0;
                            int num3 = LetterPowsForUnParse[LetterPowsForUnParse.Length - i];
                            if (num >= num3)
                            {
                                colIndex++;
                                num -= num3;
                                num3 = LetterPows[LetterPowsForUnParse.Length - i];
                                colIndex += num / num3;
                                num = num % num3;
                            }
                            if (!string.IsNullOrEmpty(str2))
                            {
                                str2 = str2 + ((char)(colIndex + 0x41));
                            }
                            else if (colIndex != 0)
                            {
                                str2 = str2 + ((char)((colIndex + 0x41) - 1));
                            }
                        }
                        str2 = str2 + ((char)(num + 0x41));
                        if (!isColRelative)
                        {
                            str2 = "$" + str2;
                        }
                        if (rowIndex <= 0)
                        {
                            sb.Append(str2);
                        }
                        else
                        {
                            sb.Append(str2);
                            sb.Append(str);
                        }
                    }
                }
            }
        }

        private void UnParseConstantExpressions(CalcExpression expr, CalcParserContext context, StringBuilder sb)
        {
            if (expr is CalcStringExpression)
            {
                sb.Append("\"");
                sb.Append((expr as CalcStringExpression).StringValue.Replace("\"", "\"\""));
                sb.Append("\"");
            }
            else if (expr is CalcDoubleExpression)
            {
                CalcDoubleExpression expression = expr as CalcDoubleExpression;
                sb.Append(expression.OriginalValue);
            }
            else if (expr is CalcBooleanExpression)
            {
                sb.Append((expr as CalcBooleanExpression).BooleanValue ? ((string)"TRUE") : ((string)"FALSE"));
            }
            else if (expr is CalcArrayExpression)
            {
                CalcArray arrayValue = (expr as CalcArrayExpression).ArrayValue;
                sb.Append("{");
                if (arrayValue.RowCount <= 0)
                {
                    throw new CalcUnParseException("Exceptions.ArrayEmpty", expr);
                }
                int num = -2147483648;
                for (int i = 0; i < arrayValue.RowCount; i++)
                {
                    if (i >= 1)
                    {
                        sb.Append(this._arraryGroupSeparator);
                    }
                    for (int j = 0; j < arrayValue.ColumnCount; j++)
                    {
                        if (((num != -2147483648) && (num != arrayValue.ColumnCount)) || (arrayValue.ColumnCount == 0))
                        {
                            throw new CalcUnParseException("Exceptions.InvalidArray", expr);
                        }
                        if (j != 0)
                        {
                            sb.Append(this._arraryArgSeparator);
                        }
                        object obj2 = arrayValue.GetValue(i, j);
                        CalcExpression expression2 = obj2 as CalcExpression;
                        if (obj2 == null)
                        {
                            throw new CalcUnParseException("Exceptions.InvalidArray", expr);
                        }
                        if (expression2 == null)
                        {
                            if (obj2 is string)
                            {
                                sb.Append("\"");
                                sb.Append(obj2.ToString().Replace("\"", "\"\""));
                                sb.Append("\"");
                            }
                            else if (obj2 is bool)
                            {
                                if ((bool)obj2)
                                {
                                    sb.Append("TRUE");
                                }
                                else
                                {
                                    sb.Append("FALSE");
                                }
                            }
                            else
                            {
                                sb.Append(obj2.ToString());
                            }
                        }
                        else
                        {
                            this.UnparseExpression(expression2, context, sb);
                        }
                    }
                }
                sb.Append("}");
            }
            else if (expr is CalcExternalErrorExpression)
            {
                CalcExternalErrorExpression expression3 = expr as CalcExternalErrorExpression;
                this.UnparseSource(expression3.Source, context, sb);
                sb.Append("!");
                sb.Append(expression3.ErrorValue.ToString());
            }
            else if (expr is CalcSheetRangeErrorExpression)
            {
                CalcSheetRangeErrorExpression expression4 = expr as CalcSheetRangeErrorExpression;
                this.UnparseSource(expression4.StartSource, expression4.EndSource, context, sb);
                sb.Append("!");
                sb.Append(expression4.ErrorValue.ToString());
            }
            else if (expr is CalcBangErrorExpression)
            {
                sb.Append("!");
                sb.Append((expr as CalcBangErrorExpression).ErrorValue.ToString());
            }
            else if (expr is CalcErrorExpression)
            {
                sb.Append((expr as CalcErrorExpression).ErrorValue.ToString());
            }
            else if (!(expr is CalcMissingArgumentExpression))
            {
                throw new CalcUnParseException("Exceptions.NotSupportExpression", expr);
            }
        }

        private void UnparseExpression(CalcExpression expr, CalcParserContext context, StringBuilder sb)
        {
            if (expr is CalcConstantExpression)
            {
                this.UnParseConstantExpressions(expr, context, sb);
            }
            else if (expr is CalcOperatorExpression)
            {
                this.UnParseOperatorExpressions(expr, context, sb);
            }
            else if (expr is CalcReferenceExpression)
            {
                this.UnParseRefenceExpressions(expr, context, sb);
            }
            else if (expr is CalcBangNameExpression)
            {
                sb.Append("!");
                sb.Append((expr as CalcBangNameExpression).Name);
            }
            else if (expr is CalcNameExpression)
            {
                sb.Append((expr as CalcNameExpression).Name);
            }
            else if (expr is CalcExternalNameExpression)
            {
                this.UnparseSource((expr as CalcExternalNameExpression).Source, context, sb);
                sb.Append("!");
                sb.Append((expr as CalcExternalNameExpression).Name);
            }
            else if (expr is CalcParenthesesExpression)
            {
                sb.Append("(");
                this.UnparseExpression((expr as CalcParenthesesExpression).Arg, context, sb);
                sb.Append(")");
            }
            else if (expr is CalcFunctionExpression)
            {
                CalcFunctionExpression expression = expr as CalcFunctionExpression;
                sb.Append(expression.FunctionName);
                sb.Append("(");
                for (int i = 0; i < expression.ArgCount; i++)
                {
                    if (i != 0)
                    {
                        sb.Append(this._listSeparator);
                    }
                    this.UnparseExpression(expression.GetArg(i), context, sb);
                }
                sb.Append(")");
            }
            else
            {
                if (!(expr is CalcSharedExpression))
                {
                    throw new CalcUnParseException("Exceptions.NotSupportExpression", expr);
                }
                expr = (expr as CalcSharedExpression).Expression;
                this.UnparseExpression(expr, context, sb);
            }
        }

        private void UnParseOperatorExpressions(CalcExpression expr, CalcParserContext context, StringBuilder sb)
        {
            if (expr is CalcUnaryOperatorExpression)
            {
                CalcUnaryOperatorExpression expression = expr as CalcUnaryOperatorExpression;
                CalcUnaryOperator @operator = expression.Operator;
                if (@operator is CalcPercentOperator)
                {
                    this.UnparseExpression(expression.Operand, context, sb);
                    sb.Append(@operator.Name);
                }
                else
                {
                    sb.Append(@operator.Name);
                    this.UnparseExpression(expression.Operand, context, sb);
                }
            }
            else
            {
                if (!(expr is CalcBinaryOperatorExpression))
                {
                    throw new CalcUnParseException("Exceptions.NotSupportExpression", expr);
                }
                CalcBinaryOperatorExpression expression2 = expr as CalcBinaryOperatorExpression;
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                this.UnparseExpression(expression2.Right, context, builder2);
                CalcBinaryOperatorExpression left = expression2.Left as CalcBinaryOperatorExpression;
                CalcBinaryOperatorExpression right = expression2.Right as CalcBinaryOperatorExpression;
                int opeatorPriority = GetOpeatorPriority(expression2.Operator.Name);
                if ((left != null) && (GetOpeatorPriority(left.Operator.Name) > opeatorPriority))
                {
                    builder.Append("(");
                    this.UnparseExpression(expression2.Left, context, builder);
                    builder.Append(")");
                }
                else
                {
                    this.UnparseExpression(expression2.Left, context, builder);
                }
                if ((right != null) && (GetOpeatorPriority(right.Operator.Name) > opeatorPriority))
                {
                    builder2.Append("(");
                    this.UnparseExpression(expression2.Right, context, builder2);
                    builder2.Append(")");
                }
                sb.Append(builder);
                string str = (expression2.Operator is CalcUnionOperator) ? ((char)this._listSeparator).ToString() : expression2.Operator.Name;
                sb.Append(str);
                sb.Append(builder2);
            }
        }

        internal static void UnParseRange(bool useR1C1, int baseRowIndex, int baseColIndex, int startRowIndex, int startColIndex, int endRowIndex, int endColIndex, bool isStartRowRelative, bool isStartColRelative, bool isEndRowRelative, bool isEndColRelative, StringBuilder sb, CalcRangeType rangeType = 0)
        {
            UnParseCell(useR1C1, baseRowIndex, baseColIndex, startRowIndex, startColIndex, isStartRowRelative, isStartColRelative, sb, rangeType);
            if ((((!useR1C1 || (startRowIndex != endRowIndex)) || (startColIndex != endColIndex)) || ((endRowIndex != -2147483648) && (endColIndex != -2147483648))) && ((endRowIndex != -2147483648) || (endColIndex != -2147483648)))
            {
                sb.Append(":");
                UnParseCell(useR1C1, baseRowIndex, baseColIndex, endRowIndex, endColIndex, isEndRowRelative, isEndColRelative, sb, rangeType);
            }
        }

        private void UnParseRefenceExpressions(CalcExpression expr, CalcParserContext context, StringBuilder sb)
        {
            int row = context.Row;
            int column = context.Column;
            if (expr is CalcBangCellExpression)
            {
                CalcBangCellExpression expression = expr as CalcBangCellExpression;
                sb.Append("!");
                UnParseCell(context.UseR1C1, row, column, expression.Row, expression.Column, expression.RowRelative, expression.ColumnRelative, sb, CalcRangeType.Cell);
            }
            else if (expr is CalcCellExpression)
            {
                CalcCellExpression expression2 = expr as CalcCellExpression;
                UnParseCell(context.UseR1C1, row, column, expression2.Row, expression2.Column, expression2.RowRelative, expression2.ColumnRelative, sb, CalcRangeType.Cell);
            }
            else if (expr is CalcExternalCellExpression)
            {
                CalcExternalCellExpression expression3 = expr as CalcExternalCellExpression;
                this.UnparseSource(expression3.Source, context, sb);
                sb.Append("!");
                UnParseCell(context.UseR1C1, row, column, expression3.Row, expression3.Column, expression3.RowRelative, expression3.ColumnRelative, sb, CalcRangeType.Cell);
            }
            else if (expr is CalcBangRangeExpression)
            {
                CalcBangRangeExpression expression4 = expr as CalcBangRangeExpression;
                sb.Append("!");
                UnParseRange(context.UseR1C1, row, column, expression4.StartRow, expression4.StartColumn, expression4.EndRow, expression4.EndColumn, expression4.StartRowRelative, expression4.StartColumnRelative, expression4.EndRowRelative, expression4.EndColumnRelative, sb, expression4.RangeType);
            }
            else if (expr is CalcRangeExpression)
            {
                CalcRangeExpression expression5 = expr as CalcRangeExpression;
                UnParseRange(context.UseR1C1, row, column, expression5.StartRow, expression5.StartColumn, expression5.EndRow, expression5.EndColumn, expression5.StartRowRelative, expression5.StartColumnRelative, expression5.EndRowRelative, expression5.EndColumnRelative, sb, expression5.RangeType);
            }
            else if (expr is CalcExternalRangeExpression)
            {
                CalcExternalRangeExpression expression6 = expr as CalcExternalRangeExpression;
                this.UnparseSource(expression6.Source, context, sb);
                sb.Append("!");
                UnParseRange(context.UseR1C1, row, column, expression6.StartRow, expression6.StartColumn, expression6.EndRow, expression6.EndColumn, expression6.StartRowRelative, expression6.StartColumnRelative, expression6.EndRowRelative, expression6.EndColumnRelative, sb, expression6.RangeType);
            }
            else if (expr is CalcSheetRangeExpression)
            {
                CalcSheetRangeExpression expression7 = expr as CalcSheetRangeExpression;
                this.UnparseSource(expression7.StartSource, expression7.EndSource, context, sb);
                sb.Append("!");
                UnParseRange(context.UseR1C1, row, column, expression7.StartRow, expression7.StartColumn, expression7.EndRow, expression7.EndColumn, expression7.StartRowRelative, expression7.StartColumnRelative, expression7.EndRowRelative, expression7.EndColumnRelative, sb, expression7.RangeType);
            }
            else
            {
                if (!(expr is CalcStructReferenceExpression))
                {
                    throw new CalcUnParseException("Exceptions.NotSupportExpression", expr);
                }
                sb.Append(expr.ToString());
            }
        }

        private void UnparseSource(ICalcSource source, CalcParserContext context, StringBuilder sb)
        {
            sb.Append(this.GetValidSource(context.GetExternalSourceToken(source), context.UseR1C1));
        }

        private void UnparseSource(ICalcSource startSource, ICalcSource endSource, CalcParserContext context, StringBuilder sb)
        {
            string str;
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            this.UnparseSource(startSource, context, builder);
            bool flag = RemoveApostrophe(builder);
            bool flag2 = RemoveWorkbook(builder, out str);
            this.UnparseSource(endSource, context, builder2);
            flag |= RemoveApostrophe(builder2);
            RemoveWorkbook(builder2, out str);
            if (flag)
            {
                sb.Append("'");
                if (flag2)
                {
                    sb.Append(str);
                }
                sb.Append(builder);
                sb.Append(":");
                sb.Append(builder2);
                sb.Append("'");
            }
            else
            {
                if (flag2)
                {
                    sb.Append(str);
                }
                sb.Append(builder);
                sb.Append(":");
                sb.Append(builder2);
            }
        }

        private void UpdateCulture(CultureInfo newCulture)
        {
            if (this._catchedCulture != newCulture)
            {
                this._catchedCulture = newCulture;
                this._listSeparator = newCulture.TextInfo.ListSeparator[0];
                this._mumberDecimalSeparator = newCulture.NumberFormat.NumberDecimalSeparator[0];
                this._arraryGroupSeparator = ';';
                this._arraryArgSeparator = (this._listSeparator == this._arraryGroupSeparator) ? '\\' : this._listSeparator;
                this.OPERATORS_INFIX = "+-*/^&=><: " + ((char)this._listSeparator);
            }
        }

        /// <summary>
        /// Validate the name.
        /// </summary>
        /// <param name="name">
        /// The name you wants to validate.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the name is valid, otherwise, <see langword="false" />.
        /// </returns>
        public static bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            if (((name.Length == 1) && (name == "r")) || (((name == "R") || (name == "c")) || (name == "C")))
            {
                return false;
            }
            char c = name[0];
            if (((c != '_') && (c != '\\')) && (!char.IsLetter(c) && !char.IsSymbol(c)))
            {
                return false;
            }
            for (int i = 1; i < name.Length; i++)
            {
                c = name[i];
                if ((((c != '_') && (c != '\\')) && ((c != '?') && (c != '.'))) && (!char.IsLetterOrDigit(c) && !char.IsSymbol(c)))
                {
                    return false;
                }
            }
            return true;
        }

        public List<ExpressionInfo> ParseReferenceExpressionInfos(string formula, CalcParserContext context)
        {
            if (context == null)
            {
                context = new CalcParserContext(false, 0, 0, null);
            }
            this.UpdateCulture(context.Culture);
            List<FormulaToken> list = this.ParseToTokens(formula);
            List<FormulaToken> list2 = new List<FormulaToken>();
            List<ExpressionInfo> list3 = new List<ExpressionInfo>();
            foreach (FormulaToken token in list)
            {
                this.GetReferenceTokenList(token, list2);
            }
            foreach (FormulaToken token2 in list2)
            {
                try
                {
                    this.BuildExpressionInfo(context, token2, list3);
                }
                catch
                {
                }
            }
            return list3;
        }
        private void BuildExpressionInfo(CalcParserContext context, FormulaToken token, List<ExpressionInfo> list)
        {
            int num;
            string str = token.Value;
            //hdt
            CalcExpression expr = this.BuildCellReferenceOrNameExpression(context, str, out num);
            if (expr != null)
            {
                ExpressionInfo info = new ExpressionInfo(token.Index, (token.Index + num) - 1, expr);
                list.Add(info);
                if (((num > 0) && (num < str.Length)) && (str[num] == ':'))
                {
                    num++;
                    while ((num > 0) && (num < str.Length))
                    {
                        int num3;
                        int startIndex = num + token.Index;
                        //hdt
                        expr = this.BuildCellReferenceOrNameExpression(context, str.Substring(num), out num3);
                        if (expr == null)
                        {
                            return;
                        }
                        info = new ExpressionInfo(startIndex, (startIndex + num3) - 1, expr);
                        list.Add(info);
                        num += num3;
                        if ((num < str.Length) && (str[num] != ':'))
                        {
                            return;
                        }
                        num++;
                    }
                }
            }
        }

        private void GetReferenceTokenList(FormulaToken token, List<FormulaToken> list)
        {
            switch (token.Type)
            {
                case ExcelFormulaTokenType.Operand:
                    if (token.Subtype != ExcelFormulaTokenSubtype.RangeOrName)
                    {
                        break;
                    }
                    list.Add(token);
                    return;

                case ExcelFormulaTokenType.Function:
                    if (token.Value == "ARRAY")
                    {
                        break;
                    }
                    if (token.Value != "ARRAYROW")
                    {
                        foreach (FormulaToken token2 in token.Children)
                        {
                            this.GetReferenceTokenList(token2, list);
                        }
                        break;
                    }
                    return;

                case ExcelFormulaTokenType.Subexpression:
                    foreach (FormulaToken token3 in token.Children)
                    {
                        this.GetReferenceTokenList(token3, list);
                    }
                    break;

                case ExcelFormulaTokenType.Argument:
                case ExcelFormulaTokenType.OperatorPrefix:
                case ExcelFormulaTokenType.OperatorInfix:
                case ExcelFormulaTokenType.OperatorPostfix:
                case ExcelFormulaTokenType.Whitespace:
                case ExcelFormulaTokenType.Unknown:
                    break;

                default:
                    return;
            }
        }

        internal bool ValidateSheetName(string name, bool isR1C1, bool containsSpecial)
        {
            if (!string.IsNullOrEmpty(name))
            {
                int num;
                if (!containsSpecial && char.IsDigit(name[0]))
                {
                    return false;
                }
                if ((!containsSpecial && IsStartWithCellReference(name, isR1C1, out num)) && (num == name.Length))
                {
                    return false;
                }
                for (int i = 0; i < name.Length; i++)
                {
                    char ch = name[i];
                    if (containsSpecial)
                    {
                        switch (ch)
                        {
                            case '*':
                            case ':':
                            case '[':
                            case ']':
                            case '?':
                            case '\\':
                            case '/':
                                return false;
                        }
                    }
                    else if ((((ch == '\'') || (ch == '[')) || ((ch == ']') || (ch == '?'))) || (((ch == '\\') || (ch == '%')) || ((ch == '"') || (this.OPERATORS_INFIX.IndexOf(ch) != -1))))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal bool ValidateWorkbook(string name, bool containsSpecial)
        {
            if (!string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < name.Length; i++)
                {
                    char ch = name[i];
                    if (containsSpecial)
                    {
                        switch (ch)
                        {
                            case '*':
                            case ':':
                            case '[':
                            case ']':
                            case '?':
                                return false;
                        }
                        if ((ch == '\'') && (((i == 0) || (i >= (name.Length - 1))) || (name[i + 1] != '\'')))
                        {
                            return false;
                        }
                    }
                    else if ((((ch == '\'') || (ch == '[')) || ((ch == ']') || (ch == '?'))) || ((ch == '%') || (this.OPERATORS_INFIX.IndexOf(ch) != -1)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static Dictionary<string, CalcFunction> Functions
        {
            get { return _functions; }
        }

        private class ExcelFormulaStack
        {
            private Stack<CalcParser.FormulaToken> _stack = new Stack<CalcParser.FormulaToken>();

            public CalcParser.FormulaToken Pop()
            {
                if (this._stack.Count == 0)
                {
                    return null;
                }
                CalcParser.FormulaToken token = this._stack.Pop();
                return new CalcParser.FormulaToken("", token.Type, CalcParser.ExcelFormulaTokenSubtype.Stop, token.Index);
            }

            public void Push(CalcParser.FormulaToken token)
            {
                this._stack.Push(token);
            }

            public CalcParser.FormulaToken Current
            {
                get
                {
                    if (this._stack.Count <= 0)
                    {
                        return null;
                    }
                    return this._stack.Peek();
                }
            }
        }

        private enum ExcelFormulaTokenSubtype
        {
            Nothing,
            Start,
            Stop,
            Text,
            Number,
            Logical,
            Error,
            RangeOrName,
            Concatenation,
            Intersection,
            Union,
            RangeOp
        }

        private enum ExcelFormulaTokenType
        {
            Operand,
            Function,
            Subexpression,
            Argument,
            OperatorPrefix,
            OperatorInfix,
            OperatorPostfix,
            Whitespace,
            Unknown
        }

        private class FormulaToken
        {
            private List<CalcParser.FormulaToken> _children;
            public int Index;
            public CalcParser.ExcelFormulaTokenSubtype Subtype;
            public CalcParser.ExcelFormulaTokenType Type;
            public string Value;

            private FormulaToken()
            {
            }

            internal FormulaToken(string value, CalcParser.ExcelFormulaTokenType type, int index)
                : this(value, type, CalcParser.ExcelFormulaTokenSubtype.Nothing, index)
            {
            }

            internal FormulaToken(string value, CalcParser.ExcelFormulaTokenType type, CalcParser.ExcelFormulaTokenSubtype subtype, int index)
            {
                this.Value = value;
                this.Type = type;
                this.Subtype = subtype;
                this.Index = index;
            }

            public override string ToString()
            {
                return string.Concat((object[])new object[] { this.Value, ": ", this.Type, ",", this.Subtype });
            }

            public List<CalcParser.FormulaToken> Children
            {
                get
                {
                    if (this._children == null)
                    {
                        this._children = new List<CalcParser.FormulaToken>();
                    }
                    return this._children;
                }
            }
        }

        private class FormulaTokenList
        {
            private int _index = -1;
            private List<CalcParser.FormulaToken> _tokens = new List<CalcParser.FormulaToken>(0x400);

            public CalcParser.FormulaToken Add(CalcParser.FormulaToken token)
            {
                this._tokens.Add(token);
                return token;
            }

            public bool MoveNext()
            {
                if (this.EOF)
                {
                    return false;
                }
                this._index++;
                return true;
            }

            public void RemoveAt(int index)
            {
                this._tokens.RemoveAt(index);
            }

            public void Reset()
            {
                this._index = -1;
            }

            public bool BOF
            {
                get
                {
                    return (this._index <= 0);
                }
            }

            public int Count
            {
                get
                {
                    return this._tokens.Count;
                }
            }

            public CalcParser.FormulaToken Current
            {
                get
                {
                    if (this._index == -1)
                    {
                        return null;
                    }
                    return this._tokens[this._index];
                }
            }

            public bool EOF
            {
                get
                {
                    return (this._index >= (this._tokens.Count - 1));
                }
            }

            public CalcParser.FormulaToken this[int i]
            {
                get
                {
                    if ((i >= 0) && (i < this._tokens.Count))
                    {
                        return this._tokens[i];
                    }
                    return null;
                }
            }

            public CalcParser.FormulaToken Next
            {
                get
                {
                    if (this.EOF)
                    {
                        return null;
                    }
                    return this._tokens[this._index + 1];
                }
            }

            public CalcParser.FormulaToken Previous
            {
                get
                {
                    if (this._index < 1)
                    {
                        return null;
                    }
                    return this._tokens[this._index - 1];
                }
            }
        }

        private enum NumberState
        {
            None,
            Sign,
            Int,
            Dot,
            Decimal,
            Exponent,
            SignExponent,
            ScientificNotation,
            Number
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ExpressionInfo
    {
        public int StartIndex;
        public int EndIndex;
        public CalcExpression Expression;
        public ExpressionInfo(int startIndex, int endIndex, CalcExpression expr)
        {
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.Expression = expr;
        }
    }
}

