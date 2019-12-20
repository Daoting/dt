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
    /// Counts the number of cells that are not empty and the values within 
    /// the list of arguments.
    /// </summary>
    /// <remarks>
    /// Use COUNTA to count the number of cells that contain data in a range or array.
    /// </remarks>
    public class CalcCountAFunction : CalcBuiltinFunction
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
        /// Counts the number of cells that are not empty and the values within
        /// the list of arguments.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: value1, [value2], [value3], ..
        /// </para>
        /// <para>
        /// Value1, value2, ... are 1 to 255 arguments representing the values you want to count.
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
                    for (short j = 0; j < ArrayHelper.GetRangeCount(args[i]); j++)
                    {
                        for (int k = 0; k < ArrayHelper.GetLength(args[i], j); k++)
                        {
                            if ((includeSubtotals || !ArrayHelper.IsSubtotal(args[i], k, j)) && (ArrayHelper.GetValue(args[i], k, j) != null))
                            {
                                num++;
                            }
                        }
                    }
                }
                else if (args[i] != null)
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
                return "COUNTA";
            }
        }
    }
}

