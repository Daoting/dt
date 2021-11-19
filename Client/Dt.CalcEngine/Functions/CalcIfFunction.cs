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
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns one value if a condition you specify evaluates to <see langword="true" /> 
    /// and another value if it evaluates to <see langword="false" />.
    /// </summary>
    public class CalcIfFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts array values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsArray(int i)
        {
            if (i != 1)
            {
                return (i == 2);
            }
            return true;
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process CalcError values.
        /// </summary>
        public override bool AcceptsError(int i)
        {
            if (i != 1)
            {
                return (i == 2);
            }
            return true;
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 2);
        }

        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts CalcReference values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            if (i != 1)
            {
                return (i == 2);
            }
            return true;
        }

        /// <summary>
        /// Returns one value if a condition you specify evaluates to <see langword="true" />
        /// and another value if it evaluates to <see langword="false" />.
        /// </summary>
        /// <param name="args">
        /// <para>
        /// The args contains three items: logical_test, value_if_true and value_if_false.
        /// </para>
        /// <para>
        /// logical_test is any value or expression that can be evaluated to <see langword="true" /> or <see langword="false" />.
        /// For example, A10=100 is a logical expression; if the value in cell A10 is equal to 100,
        /// the expression evaluates to <see langword="true" />. Otherwise, the expression evaluates to <see langword="false" />.
        /// This argument can use any comparison calculation operator.
        /// </para>
        /// <para>
        /// value_if_true is the value that is returned if logical_test is <see langword="true" />. For example,
        /// if this argument is the text string "Within budget" and the logical_test argument evaluates to <see langword="true" />,
        /// then the IF function displays the text "Within budget". If logical_test is <see langword="true" /> and value_if_true is blank,
        /// this argument returns 0 (zero). To display the word <see langword="true" />, use the logical value <see langword="true" /> for this argument.
        /// Value_if_true can be another formula.
        /// </para>
        /// <para>
        /// value_if_false is the value that is returned if logical_test is <see langword="false" />. For example,
        /// if this argument is the text string "Over budget" and the logical_test argument evaluates to <see langword="false" />,
        /// then the IF function displays the text "Over budget". If logical_test is <see langword="false" /> and value_if_false is omitted,
        /// (that is, after value_if_true, there is no comma), then the logical value <see langword="false" /> is returned.
        /// If logical_test is <see langword="false" /> and value_if_false is blank (that is, after value_if_true,
        /// there is a comma followed by the closing parenthesis),
        /// then the value 0 (zero) is returned. Value_if_false can be another formula.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object param = args[0];
            object obj3 = args[1];
            object obj4 = CalcHelper.ArgumentExists(args, 2) ? args[2] : ((bool) false);
            if (CalcHelper.TryExtractToSingleValue(param, out param))
            {
                return EvaluateSingleValue(param, obj3, obj4);
            }
            bool flag = CalcHelper.TryExtractToSingleValue(obj3, out obj3);
            bool flag2 = CalcHelper.TryExtractToSingleValue(obj4, out obj4);
            CalcArray array = param as CalcArray;
            object[,] values = new object[array.RowCount, array.ColumnCount];
            for (int i = 0; i < array.RowCount; i++)
            {
                for (int j = 0; j < array.ColumnCount; j++)
                {
                    values[i, j] = EvaluateSingleValue(array.GetValue(i, j), flag ? obj3 : CalcHelper.GetArrayValue(obj3 as CalcArray, i, j), flag2 ? obj4 : CalcHelper.GetArrayValue(obj4 as CalcArray, i, j));
                }
            }
            return new ConcreteArray<object>(values);
        }

        private static object EvaluateSingleValue(object arg0, object arg1, object arg2)
        {
            bool flag;
            if (!CalcConvert.TryToBool(arg0, out flag))
            {
                return CalcErrors.Value;
            }
            object obj2 = flag ? arg1 : arg2;
            return (obj2 ?? ((int) 0));
        }

        /// <summary>
        /// Finds the branch argument.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns></returns>
        public override int FindBranchArgument(object test)
        {
            if (test is CalcError)
            {
                return -1;
            }
            bool flag = false;
            try
            {
                flag = CalcConvert.ToBool(test);
            }
            catch (InvalidCastException)
            {
            }
            if (!flag)
            {
                return 2;
            }
            return 1;
        }

        /// <summary>
        /// Finds the test argument when this function is branched.
        /// </summary>
        /// <returns>
        /// An index indicates the argument which would be treated as test condition
        /// </returns>
        public override int FindTestArgument()
        {
            return 0;
        }

        /// <summary>
        /// Gets a value indicating whether this function is branched by arguments as condition.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is branch; otherwise, <c>false</c>.
        /// </value>
        public override bool IsBranch
        {
            get
            {
                return true;
            }
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
                return 3;
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
                return 2;
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
                return "IF";
            }
        }
    }
}

