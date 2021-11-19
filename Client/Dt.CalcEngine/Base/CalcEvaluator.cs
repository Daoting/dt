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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represents an evaluator which is used for evaluating a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> with specified <see cref="T:Dt.CalcEngine.CalcEvaluatorContext" />.
    /// </summary>
    public class CalcEvaluator
    {
        private static readonly CalcParser DefaultParser = new CalcParser();
        private static CultureInfo evaluatorCulture;

        /// <summary>
        /// Represents the CultureInfo which used to evaluate functions
        /// </summary>
        public static CultureInfo EvaluatorCulture
        {
            get
            {
                if (evaluatorCulture == null)
                {
                    return CultureInfo.CurrentCulture;
                }
                return evaluatorCulture;
            }
            set
            {
                evaluatorCulture = value;
            }
        }

        /// <summary>
        /// Evaluate a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> with specified <see cref="T:Dt.CalcEngine.CalcEvaluatorContext" />.
        /// </summary>
        /// <param name="expr">The expression to be evaluated.</param>
        /// <param name="context">The context for evaluator to query data.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> represent the result of expression.
        /// </returns>
        public object Evaluate(CalcExpression expr, CalcEvaluatorContext context = null)
        {
            return this.Evaluate(expr, context, false, false);
        }

        /// <summary>
        /// Evaluates the specified formula.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <param name="parserContext">The parser context.</param>
        /// <param name="evaluatorContext">The evaluator context.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> represent the result of expression.
        /// </returns>
        /// <exception cref="T:Dt.CalcEngine.CalcParseException">There are some errors in string formula.</exception>
        public object Evaluate(string formula, CalcParserContext parserContext = null, CalcEvaluatorContext evaluatorContext = null)
        {
            return this.Evaluate(DefaultParser.Parse(formula, parserContext), evaluatorContext);
        }

        /// <summary>
        /// hdt 唐忠宝 将 private 改成 public
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="context"></param>
        /// <param name="acceptsArray"></param>
        /// <param name="acceptsReference"></param>
        /// <returns></returns>
        public object Evaluate(CalcExpression expr, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference)
        {
            if (expr == null)
            {
                throw new ArgumentNullException();
            }
            while (true)
            {
                if (!(expr is CalcParenthesesExpression))
                {
                    if (expr is CalcConstantExpression)
                    {
                        return this.EvaluateConstant(expr as CalcConstantExpression, context, acceptsArray);
                    }
                    if (expr is CalcReferenceExpression)
                    {
                        return this.EvaluateReference(expr as CalcReferenceExpression, context, acceptsReference);
                    }
                    if (expr is CalcExternalNameExpression)
                    {
                        CalcExternalNameExpression expression = expr as CalcExternalNameExpression;
                        return this.EvaluateName(expression.Name, expression.Source.GetEvaluatorContext(new CalcCellIdentity(context.Row, context.Column)), context, acceptsArray, acceptsReference);
                    }
                    if (expr is CalcNameExpression)
                    {
                        return this.EvaluateName((expr as CalcNameExpression).Name, context, context, acceptsArray, acceptsReference);
                    }
                    if (expr is CalcUnaryOperatorExpression)
                    {
                        return this.EvaluateUnaryOperation(expr as CalcUnaryOperatorExpression, context, acceptsArray);
                    }
                    if (expr is CalcBinaryOperatorExpression)
                    {
                        return this.EvaluateBinaryOperation(expr as CalcBinaryOperatorExpression, context, acceptsArray);
                    }
                    if (expr is CalcFunctionExpression)
                    {
                        return this.EvaluateFunction(expr as CalcFunctionExpression, context, acceptsArray, acceptsReference);
                    }
                    if (!(expr is CalcSharedExpression))
                    {
                        throw new ArgumentException("Exceptions.NotSupportExpression", expr.ToString());
                    }
                    return this.Evaluate((expr as CalcSharedExpression).Expression, context, acceptsArray, acceptsReference);
                }
                expr = (expr as CalcParenthesesExpression).Arg;
            }
        }

        private object EvaluateBinaryOperation(CalcBinaryOperatorExpression expr, CalcEvaluatorContext context, bool acceptsArray)
        {
            CalcExpression[] expressionArray = new CalcExpression[] { expr.Left, expr.Right };
            object[] objArray = new object[2];
            for (int i = 0; i < 2; i++)
            {
                object obj2 = this.Evaluate(expressionArray[i], context, acceptsArray, expr.Operator.AcceptsReference(i));
                if (obj2 is CalcError)
                {
                    return obj2;
                }
                if (obj2 is CalcMissingArgument)
                {
                    return CalcErrors.NotAvailable;
                }
                objArray[i] = obj2;
            }
            try
            {
                return expr.Operator.Evaluate(objArray[0], objArray[1], context);
            }
            catch (InvalidCastException)
            {
                return CalcErrors.Value;
            }
        }

        private object EvaluateConstant(CalcConstantExpression expr, CalcEvaluatorContext context, bool acceptsArray)
        {
            object obj2 = expr.Value;
            if ((obj2 is CalcArray) && (!acceptsArray && ((context == null) || (!context.ArrayFormulaMode && !context.ExpandArrayToMultiCall))))
            {
                return CalcHelper.GetArrayValue(obj2 as CalcArray, 0, 0);
            }
            return obj2;
        }

        private object EvaluateFunction(CalcFunctionExpression expr, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference)
        {
            CalcFunction function;
            object[] objArray;
            object obj2;
            List<int> list;
            if (!this.PrepareFunctionArguments(expr, context, out function, out objArray, out list, out obj2))
            {
                return obj2;
            }
            int argCount = expr.ArgCount;
            if ((context != null) && context.ArrayFormulaMode)
            {
                if (argCount > 0)
                {
                    return this.EvaluateFunctionWithArrayFormulaMode(function, objArray, context, acceptsArray, acceptsReference);
                }
                return this.EvaluateFunction(function, objArray, context, acceptsArray, acceptsReference, 0, 0);
            }
            if (((context != null) && context.ExpandArrayToMultiCall) && (list.Count > 0))
            {
                Dictionary<int, object> dictionary = new Dictionary<int, object>(argCount);
                bool[] flagArray = new bool[argCount];
                bool flag = true;
                for (int i = 0; i < argCount; i++)
                {
                    if (list.Contains(i))
                    {
                        CalcArray array = objArray[i] as CalcArray;
                        if (array == null)
                        {
                            dictionary[i] = null;
                            flagArray[i] = true;
                        }
                        else if (array.Length == 1)
                        {
                            dictionary[i] = array.GetValue(0);
                            flagArray[i] = true;
                        }
                        else
                        {
                            dictionary[i] = array;
                            flag = false;
                        }
                    }
                    else
                    {
                        flagArray[i] = true;
                    }
                }
                if (!flag)
                {
                    int[] numArray = new int[argCount];
                    int[] numArray2 = new int[argCount];
                    for (int j = 0; j < argCount; j++)
                    {
                        CalcArray array2 = dictionary.ContainsKey(j) ? (dictionary[j] as CalcArray) : null;
                        numArray[j] = (flagArray[j] || (array2 == null)) ? -1 : array2.RowCount;
                        numArray2[j] = (flagArray[j] || (array2 == null)) ? -1 : array2.ColumnCount;
                    }
                    int num4 = -1;
                    int num5 = -1;
                    if (!flagArray[0])
                    {
                        num4 = numArray[0];
                        num5 = numArray2[0];
                    }
                    for (int k = 1; k < argCount; k++)
                    {
                        if (!flagArray[k])
                        {
                            int num7 = numArray[k];
                            int num8 = numArray2[k];
                            num4 = (num4 == -1) ? num7 : ((num7 > 1) ? ((num4 > 1) ? Math.Min(num7, num4) : num7) : num4);
                            num5 = (num5 == -1) ? num8 : ((num8 > 1) ? ((num5 > 1) ? Math.Min(num8, num5) : num8) : num5);
                        }
                    }
                    object[,] values = new object[num4, num5];
                    object[] objArray3 = new object[objArray.Length];
                    objArray.CopyTo(objArray3, 0);
                    using (context.EnterExpandArrayToMultiCallMode())
                    {
                        for (int m = 0; m < num4; m++)
                        {
                            for (int n = 0; n < num5; n++)
                            {
                                foreach (KeyValuePair<int, object> pair in dictionary)
                                {
                                    objArray3[pair.Key] = flagArray[pair.Key] ? pair.Value : CalcHelper.GetArrayValue(pair.Value as CalcArray, m, n);
                                }
                                values[m, n] = this.EvaluateFunction(function, objArray3, context, acceptsArray, acceptsReference, m, n);
                            }
                        }
                        return new ConcreteArray<object>(values);
                    }
                }
                foreach (KeyValuePair<int, object> pair2 in dictionary)
                {
                    objArray[pair2.Key] = pair2.Value;
                }
            }
            return this.EvaluateFunction(function, objArray, context, acceptsArray, acceptsReference, 0, 0);
        }

        private object EvaluateFunction(CalcFunction func, object[] args, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference, int offsetRow, int offsetColumn)
        {
            try
            {
                CalcEvaluatorContext context2 = ((offsetRow != 0) && (offsetColumn != 0)) ? ((context != null) ? context.Offset(offsetRow, offsetColumn) : null) : context;
                object obj2 = func.IsContextSensitive ? func.Evaluate(args, context2) : func.Evaluate(args);
                if (obj2 is CalcReference)
                {
                    if (context2 != null)
                    {
                        if (context2.ArrayFormulaMode)
                        {
                            if (context.ExpandArrayToMultiCall)
                            {
                                obj2 = CalcHelper.GetArrayValue(CalcConvert.ToArray(obj2), offsetRow, offsetColumn) ?? ((int) 0);
                            }
                        }
                        else if (!acceptsReference)
                        {
                            obj2 = this.ExtractValueFromReference(obj2 as CalcReference, context2.Row, context2.Column) ?? ((int) 0);
                        }
                    }
                }
                else if (obj2 is CalcArray)
                {
                    if (context == null)
                    {
                        obj2 = CalcHelper.GetArrayValue(obj2 as CalcArray, offsetRow, offsetColumn);
                    }
                    else if (context.ArrayFormulaMode)
                    {
                        if (context.ExpandArrayToMultiCall)
                        {
                            obj2 = CalcHelper.GetArrayValue(obj2 as CalcArray, offsetRow, offsetColumn);
                        }
                    }
                    else if (!acceptsArray && context.ExpandArrayToMultiCall)
                    {
                        obj2 = CalcHelper.GetArrayValue(obj2 as CalcArray, offsetRow, offsetColumn);
                    }
                }
                return obj2;
            }
            catch (InvalidCastException)
            {
                return CalcErrors.Value;
            }
        }

        private bool EvaluateFunctionArgument(CalcExpression argExpr, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference, bool acceptsError, bool acceptsMissingArgument, out object result)
        {
            if (((context != null) && (acceptsArray || context.ExpandArrayToMultiCall)) && !context.ArrayFormulaMode)
            {
                using (context.EnterExpandArrayToMultiCallMode())
                {
                    return this.EvaluateFunctionArgumentImp(argExpr, context, false, acceptsReference, acceptsError, acceptsMissingArgument, out result);
                }
            }
            return this.EvaluateFunctionArgumentImp(argExpr, context, acceptsArray, acceptsReference, acceptsError, acceptsMissingArgument, out result);
        }

        private bool EvaluateFunctionArgumentImp(CalcExpression argExpr, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference, bool acceptsError, bool acceptsMissingArgument, out object result)
        {
            result = this.Evaluate(argExpr, context, acceptsArray, acceptsReference);
            if (!acceptsError && (result is CalcError))
            {
                return false;
            }
            if (!acceptsMissingArgument && (result is CalcMissingArgument))
            {
                result = null;
                return true;
            }
            if ((context != null) && !context.ArrayFormulaMode)
            {
                if (((result is CalcArray) && !acceptsArray) && !context.ExpandArrayToMultiCall)
                {
                    CalcArray array = result as CalcArray;
                    if (array.Length > 0)
                    {
                        result = array.GetValue(0);
                        return true;
                    }
                    result = CalcErrors.NotAvailable;
                    return false;
                }
                if ((result is CalcReference) && !acceptsReference)
                {
                    result = this.ExtractValueFromReference(result as CalcReference, context.Row, context.Column) ?? ((int) 0);
                    if (!acceptsError && (result is CalcError))
                    {
                        return false;
                    }
                    if (!acceptsMissingArgument && (result is CalcMissingArgument))
                    {
                        result = null;
                        return true;
                    }
                }
            }
            return true;
        }

        private object EvaluateFunctionOneArg(CalcFunction func, object arg, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference)
        {
            if (func.AcceptsReference(0) || CalcHelper.TryExtractToSingleValue(arg, out arg))
            {
                return this.EvaluateFunction(func, new object[] { arg }, context, acceptsArray, acceptsReference, 0, 0);
            }
            CalcArray array = arg as CalcArray;
            object[,] values = new object[array.RowCount, array.ColumnCount];
            for (int i = 0; i < array.RowCount; i++)
            {
                for (int j = 0; j < array.ColumnCount; j++)
                {
                    values[i, j] = this.EvaluateFunction(func, new object[] { array.GetValue(i, j) }, context, acceptsArray, acceptsReference, i, j);
                }
            }
            return new ConcreteArray<object>(values);
        }

        private object EvaluateFunctionTwoOrMoreArgs(CalcFunction func, object[] args, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference)
        {
            int length = args.Length;
            bool[] flagArray = new bool[length];
            bool flag = true;
            for (int i = 0; i < length; i++)
            {
                object obj2 = null;
                bool flag2 = func.AcceptsReference(i);
                bool flag3 = flag2 || CalcHelper.TryExtractToSingleValue(args[i], out obj2);
                flag = flag && flag3;
                flagArray[i] = flag3;
                if (!flag2)
                {
                    args[i] = obj2;
                }
            }
            if (flag)
            {
                return this.EvaluateFunction(func, args, context, acceptsArray, acceptsReference, 0, 0);
            }
            int[] numArray = new int[length];
            int[] numArray2 = new int[length];
            for (int j = 0; j < length; j++)
            {
                CalcArray array = args[j] as CalcArray;
                numArray[j] = (flagArray[j] || (array == null)) ? -1 : array.RowCount;
                numArray2[j] = (flagArray[j] || (array == null)) ? -1 : array.ColumnCount;
            }
            int num4 = -1;
            int num5 = -1;
            if (!flagArray[0])
            {
                num4 = numArray[0];
                num5 = numArray2[0];
            }
            for (int k = 1; k < length; k++)
            {
                if (!flagArray[k])
                {
                    int num7 = numArray[k];
                    int num8 = numArray2[k];
                    num4 = (num4 == -1) ? num7 : ((num7 > 1) ? ((num4 > 1) ? Math.Min(num7, num4) : num7) : num4);
                    num5 = (num5 == -1) ? num8 : ((num8 > 1) ? ((num5 > 1) ? Math.Min(num8, num5) : num8) : num5);
                }
            }
            object[,] values = new object[num4, num5];
            using (context.EnterExpandArrayToMultiCallMode())
            {
                for (int m = 0; m < num4; m++)
                {
                    for (int n = 0; n < num5; n++)
                    {
                        object[] objArray2 = new object[length];
                        for (int num11 = 0; num11 < length; num11++)
                        {
                            objArray2[num11] = flagArray[num11] ? args[num11] : CalcHelper.GetArrayValue(args[num11] as CalcArray, m, n);
                        }
                        values[m, n] = this.EvaluateFunction(func, objArray2, context, acceptsArray, acceptsReference, m, n);
                    }
                }
                return new ConcreteArray<object>(values);
            }
        }

        private object EvaluateFunctionWithArrayFormulaMode(CalcFunction func, object[] args, CalcEvaluatorContext context, bool acceptsArray, bool acceptsReference)
        {
            if (args.Length == 0)
            {
                return this.EvaluateFunction(func, args, context, acceptsArray, acceptsReference, 0, 0);
            }
            if (func.MaxArgs == 1)
            {
                return this.EvaluateFunctionOneArg(func, args[0], context, acceptsArray, acceptsReference);
            }
            return this.EvaluateFunctionTwoOrMoreArgs(func, args, context, acceptsArray, acceptsReference);
        }

        private object EvaluateName(string name, CalcEvaluatorContext nameContext, CalcEvaluatorContext evalContext, bool acceptsArray, bool acceptsReference)
        {
            if (object.ReferenceEquals(evalContext, null))
            {
                return CalcErrors.Name;
            }
            CalcExpression objA = nameContext.GetName(name);
            if (object.ReferenceEquals(objA, null))
            {
                return CalcErrors.Name;
            }
            try
            {
                new ValidateNameVisitor(nameContext, name).Visit(objA, evalContext.Row, evalContext.Column);
            }
            catch (ArgumentException)
            {
                return CalcErrors.Name;
            }
            return this.Evaluate(objA, evalContext, acceptsArray, acceptsReference);
        }

        private object EvaluateReference(CalcReferenceExpression expr, CalcEvaluatorContext context, bool acceptsReference)
        {
            if (object.ReferenceEquals(context, null))
            {
                return CalcErrors.Value;
            }
            CalcIdentity id = expr.GetId(context.Row, context.Column);
            if (!acceptsReference && !context.ArrayFormulaMode)
            {
                return context.GetValue(id);
            }
            return context.GetReference(id);
        }

        private object EvaluateUnaryOperation(CalcUnaryOperatorExpression expr, CalcEvaluatorContext context, bool acceptsArray)
        {
            object operand = this.Evaluate(expr.Operand, context, acceptsArray, false);
            if (operand is CalcError)
            {
                return operand;
            }
            if (operand is CalcMissingArgument)
            {
                return CalcErrors.NotAvailable;
            }
            try
            {
                return expr.Operator.Evaluate(operand, context);
            }
            catch (InvalidCastException)
            {
                return CalcErrors.Value;
            }
        }

        private object ExtractValueFromReference(CalcReference reference, int row, int col)
        {
            SheetRangeReference reference2 = reference as SheetRangeReference;
            if ((reference2 != null) && (reference2.SheetCount != 1))
            {
                return CalcErrors.Reference;
            }
            int rowCount = reference.GetRowCount(0);
            int columnCount = reference.GetColumnCount(0);
            if (((reference.RangeCount <= 0) || (rowCount <= 0)) || (columnCount <= 0))
            {
                return CalcErrors.Reference;
            }
            try
            {
                if ((reference.RangeCount == 1) && ((rowCount <= 1) || (columnCount <= 1)))
                {
                    int rowOffset = row - reference.GetRow(0);
                    int columnOffset = col - reference.GetColumn(0);
                    if ((rowCount == 1) && (columnCount == 1))
                    {
                        return reference.GetValue(0, 0, 0);
                    }
                    if (((rowCount == 1) && (columnCount > 1)) && ((columnOffset >= 0) && (columnOffset < columnCount)))
                    {
                        return reference.GetValue(0, 0, columnOffset);
                    }
                    if (((rowCount > 1) && (columnCount == 1)) && ((rowOffset >= 0) && (rowOffset < rowCount)))
                    {
                        return reference.GetValue(0, rowOffset, 0);
                    }
                }
                return CalcErrors.Value;
            }
            catch (InvalidCastException)
            {
                return CalcErrors.Value;
            }
        }

        private bool PrepareFunctionArguments(CalcFunctionExpression expr, CalcEvaluatorContext context, out CalcFunction func, out object[] args, out List<int> expandArrayArgIndex, out object error)
        {
            if (!this.TryGetFunction(expr, context, out func, out error))
            {
                args = null;
                expandArrayArgIndex = null;
                return false;
            }
            expandArrayArgIndex = new List<int>();
            int argCount = expr.ArgCount;
            args = new object[argCount];
            error = null;
            if (func.IsBranch)
            {
                int i = func.FindTestArgument();
                if ((i >= 0) && (i < argCount))
                {
                    object obj2;
                    if (!this.EvaluateFunctionArgument(expr.GetArg(i), context, func.AcceptsArray(i), func.AcceptsReference(i), func.AcceptsError(i), func.AcceptsMissingArgument(i), out obj2))
                    {
                        error = obj2;
                        expandArrayArgIndex = null;
                        return false;
                    }
                    if ((context != null) && !context.ArrayFormulaMode)
                    {
                        if (obj2 is CalcReference)
                        {
                            obj2 = this.ExtractValueFromReference(obj2 as CalcReference, context.Row, context.Column);
                            if (!func.AcceptsError(i) && (obj2 is CalcError))
                            {
                                error = obj2;
                                expandArrayArgIndex = null;
                                return false;
                            }
                            if (!func.AcceptsMissingArgument(i) && (obj2 is CalcMissingArgument))
                            {
                                obj2 = null;
                            }
                        }
                        else if ((obj2 is CalcArray) && !func.AcceptsArray(i))
                        {
                            if (!context.ExpandArrayToMultiCall)
                            {
                                obj2 = CalcHelper.GetArrayValue(obj2 as CalcArray, 0, 0);
                                if (!func.AcceptsError(i) && (obj2 is CalcError))
                                {
                                    error = obj2;
                                    expandArrayArgIndex = null;
                                    return false;
                                }
                                if (!func.AcceptsMissingArgument(i) && (obj2 is CalcMissingArgument))
                                {
                                    obj2 = null;
                                }
                            }
                            else
                            {
                                expandArrayArgIndex.Add(i);
                            }
                        }
                    }
                    args[i] = obj2;
                }
                List<int> list = new List<int>();
                CalcArray array = CalcConvert.ToArray(args[i]);
                for (int j = 0; j < array.RowCount; j++)
                {
                    for (int k = 0; k < array.ColumnCount; k++)
                    {
                        int item = -1;
                        try
                        {
                            item = func.FindBranchArgument(array.GetValue(j, k));
                        }
                        catch (InvalidCastException)
                        {
                        }
                        if (item != -1)
                        {
                            if (!list.Contains(item))
                            {
                                list.Add(item);
                            }
                            if (list.Count >= (func.MaxArgs - 1))
                            {
                                break;
                            }
                        }
                    }
                }
                if (list.Count == 0)
                {
                    error = CalcErrors.Value;
                    expandArrayArgIndex = null;
                    return false;
                }
                foreach (int num6 in list)
                {
                    if ((num6 >= 0) && (num6 < argCount))
                    {
                        object obj3;
                        if (!this.EvaluateFunctionArgument(expr.GetArg(num6), context, func.AcceptsArray(num6), func.AcceptsReference(num6), func.AcceptsError(num6), func.AcceptsMissingArgument(num6), out obj3))
                        {
                            error = obj3;
                            expandArrayArgIndex = null;
                            return false;
                        }
                        if (((context != null) && !context.ArrayFormulaMode) && ((obj3 is CalcArray) && !func.AcceptsArray(num6)))
                        {
                            if (!context.ExpandArrayToMultiCall)
                            {
                                obj3 = CalcHelper.GetArrayValue(obj3 as CalcArray, 0, 0);
                                if (!func.AcceptsError(num6) && (obj3 is CalcError))
                                {
                                    error = obj3;
                                    expandArrayArgIndex = null;
                                    return false;
                                }
                                if (!func.AcceptsMissingArgument(num6) && (obj3 is CalcMissingArgument))
                                {
                                    obj3 = null;
                                }
                            }
                            else
                            {
                                expandArrayArgIndex.Add(num6);
                            }
                        }
                        args[num6] = obj3;
                    }
                }
                if (expandArrayArgIndex.Count > 0)
                {
                    expandArrayArgIndex.Sort();
                }
            }
            else
            {
                for (int m = 0; m < argCount; m++)
                {
                    object obj4;
                    if (!this.EvaluateFunctionArgument(expr.GetArg(m), context, func.AcceptsArray(m), func.AcceptsReference(m), func.AcceptsError(m), func.AcceptsMissingArgument(m), out obj4))
                    {
                        error = obj4;
                        expandArrayArgIndex = null;
                        return false;
                    }
                    if (((context != null) && !context.ArrayFormulaMode) && ((obj4 is CalcArray) && !func.AcceptsArray(m)))
                    {
                        if (!context.ExpandArrayToMultiCall)
                        {
                            obj4 = CalcHelper.GetArrayValue(obj4 as CalcArray, 0, 0);
                            if (!func.AcceptsError(m) && (obj4 is CalcError))
                            {
                                error = obj4;
                                expandArrayArgIndex = null;
                                return false;
                            }
                            if (!func.AcceptsMissingArgument(m) && (obj4 is CalcMissingArgument))
                            {
                                obj4 = null;
                            }
                        }
                        else
                        {
                            expandArrayArgIndex.Add(m);
                        }
                    }
                    args[m] = obj4;
                }
            }
            return true;
        }

        private bool TryGetFunction(CalcFunctionExpression expr, CalcEvaluatorContext context, out CalcFunction func, out object error)
        {
            func = expr.Function;
            int argCount = expr.ArgCount;
            if (object.ReferenceEquals(func, null))
            {
                if (object.ReferenceEquals(context, null))
                {
                    error = CalcErrors.Name;
                    return false;
                }
                func = context.GetFunction(expr.FunctionName);
                if (object.ReferenceEquals(func, null))
                {
                    error = CalcErrors.Name;
                    return false;
                }
                if ((argCount < func.MinArgs) || (func.MaxArgs < argCount))
                {
                    error = CalcErrors.Value;
                    return false;
                }
            }
            error = null;
            return true;
        }

        private class ValidateNameVisitor : ExpressionVisitor
        {
            private readonly CalcEvaluatorContext _context;
            private readonly string _name;

            public ValidateNameVisitor(CalcEvaluatorContext context, string name)
            {
                this._context = context;
                this._name = name;
            }

            protected override CalcExpression VisitExternalNameExpression(CalcExternalNameExpression expr, int baseRow, int baseColumn)
            {
                if ((expr.Source == this._context.Source) && (CultureInfo.InvariantCulture.CompareInfo.Compare(expr.Name, this._name, CompareOptions.IgnoreCase) == 0))
                {
                    throw new ArgumentException();
                }
                CalcEvaluator.ValidateNameVisitor visitor = new CalcEvaluator.ValidateNameVisitor(this._context, this._name);
                CalcExpression name = expr.Source.GetEvaluatorContext(new CalcCellIdentity(this._context.Row, this._context.Column)).GetName(expr.Name);
                if (name == null)
                {
                    throw new ArgumentException();
                }
                visitor.Visit(name, baseRow, baseColumn);
                return base.VisitExternalNameExpression(expr, baseRow, baseColumn);
            }

            protected override CalcExpression VisitNameExpression(CalcNameExpression expr, int baseRow, int baseColumn)
            {
                if (CultureInfo.InvariantCulture.CompareInfo.Compare(expr.Name, this._name, CompareOptions.IgnoreCase) == 0)
                {
                    throw new ArgumentException();
                }
                CalcEvaluator.ValidateNameVisitor visitor = new CalcEvaluator.ValidateNameVisitor(this._context, this._name);
                CalcExpression name = this._context.GetName(expr.Name);
                if (name == null)
                {
                    throw new ArgumentException();
                }
                visitor.Visit(name, baseRow, baseColumn);
                return base.VisitNameExpression(expr, baseRow, baseColumn);
            }
        }
    }
}

