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
using System.Collections;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Counts the number of cells that contain numbers and counts numbers
    /// within the list of arguments.
    /// </summary>
    /// <remarks>
    /// Use COUNT to get the number of entries in a number field that is in 
    /// a range or array of numbers.
    /// </remarks>
    public class CalcCountFunction : CalcBuiltinFunction
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
            return true;
        }

        /// <summary>
        /// Indicates whether the function can process CalcError values.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function can process CalcError values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsError(int i)
        {
            return true;
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
            return true;
        }

        /// <summary>
        /// Counts the number of cells that contain numbers and counts numbers
        /// within the list of arguments.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: value1, [value2], [value3], ..
        /// </para>
        /// <para>
        /// Value1, value2, ... are 1 to 255 arguments that can contain or refer
        /// to a variety of different types of data, but only numbers are counted.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return Evaluate(args, true);
        }

        internal static object Evaluate(object[] args, bool includeSubtotals)
        {
            double num = 0.0;
            for (int i = 0; i < args.Length; i++)
            {
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    IEnumerator enumerator = includeSubtotals ? CalcConvert.ToEnumerator(args[i], false) : CalcConvert.ToEnumeratorExcludingSubtotals(args[i]);
                    while (enumerator.MoveNext())
                    {
                        if (CalcConvert.IsNumber(enumerator.Current))
                        {
                            num++;
                        }
                    }
                }
                else if (CalcConvert.IsNumber(args[i]))
                {
                    num++;
                }
            }
            return CalcConvert.ToResult(num);
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
                return "COUNT";
            }
        }
    }
}

