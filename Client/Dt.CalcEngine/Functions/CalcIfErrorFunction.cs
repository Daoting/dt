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
    /// Returns a value you specify if a formula evaluates to an error; otherwise,
    /// returns the result of the formula. Use the IFERROR function to trap and 
    /// handle errors in a formula (formula: A sequence of values, cell references, 
    /// names, functions, or operators in a cell that together produce a new value. 
    /// A formula always begins with an equal sign (=).).
    /// </summary>
    public class CalcIfErrorFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process CalcError values.
        /// </summary>
        public override bool AcceptsError(int i)
        {
            return (i == 0);
        }

        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// True if the function accepts CalcReference values for the specified argument; false otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return (i == 0);
        }

        /// <summary>
        /// Returns a value you specify if a formula evaluates to an error; otherwise,
        /// returns the result of the formula. Use the IFERROR function to trap and
        /// handle errors in a formula (formula: A sequence of values, cell references,
        /// names, functions, or operators in a cell that together produce a new value.
        /// A formula always begins with an equal sign (=).).
        /// </summary>
        /// <param name="args"><para>
        /// The args contains three items: value, value_if_error.
        /// </para>
        /// <para>
        /// Value is the argument that is checked for an error.
        /// </para>
        /// <para>
        /// Value_if_error is the value to return if the formula evaluates to an error.
        /// The following error types are evaluated: #N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, or #NULL!.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            bool flag = args[0] is CalcError;
            if (!flag && (args[0] is CalcReference))
            {
                CalcReference reference = args[0] as CalcReference;
                if (reference.RangeCount > 1)
                {
                    return CalcErrors.Value;
                }
                return new BinaryCompositeConcreteReference(reference.GetSource(), reference.GetRow(0), reference.GetColumn(0), reference.GetRowCount(0), reference.GetColumnCount(0), new Func<object, object, object>(CalcIfErrorFunction.EvaluateSingle), args[1], false);
            }
            if (!flag)
            {
                object obj1 = args[0];
                if (obj1 != null)
                {
                    return obj1;
                }
                return (int) 0;
            }
            return (args[1] ?? ((int) 0));
        }

        private static object EvaluateSingle(object value, object valueIfError)
        {
            if (!(value is CalcError) && (value == null))
            {
                return (int) 0;
            }
            return (valueIfError ?? ((int) 0));
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
                return 2;
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
                return "IFERROR";
            }
        }
    }
}

