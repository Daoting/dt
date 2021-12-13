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
    /// Uses index_num to return a value from the list of value arguments.
    /// </summary>
    /// <remarks>
    /// Use CHOOSE to select one of up to 254 values based on the index number.
    /// </remarks>
    public class CalcChooseFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process CalcError values.
        /// </summary>
        public override bool AcceptsError(int i)
        {
            return (i >= 1);
        }

        /// <summary>
        /// Uses index_num to return a value from the list of value arguments.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 255 items: index_num, value1, [value2], ..
        /// </para>
        /// <para>
        /// Index_num specifies which value argument is selected.
        /// </para>
        /// <para>
        /// Value1,value2,... are 1 to 254 value arguments from which CHOOSE selects
        /// a value or an action to perform based on index_num. The arguments can
        /// be numbers, cell references, defined names, formulas, functions, or text.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int index = CalcConvert.ToInt(args[0]);
            if ((index < 1) || (args.Length <= index))
            {
                return CalcErrors.Value;
            }
            return (args[index] ?? ((double) 0.0));
        }

        /// <summary>
        /// Finds the branch argument.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns></returns>
        public override int FindBranchArgument(object test)
        {
            return CalcConvert.ToInt(test);
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
                return 0xff;
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
                return "CHOOSE";
            }
        }
    }
}

