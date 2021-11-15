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
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the k-th percentile of values in a range.
    /// </summary>
    /// <remarks>
    /// You can use this function to establish a threshold of acceptance.
    /// </remarks>
    public class CalcPercentileFunction : CalcBuiltinFunction
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
            return (i == 0);
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
            return (i == 0);
        }

        /// <summary>
        /// Returns the k-th percentile of values in a range.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array, k.
        /// </para>
        /// <para>
        /// Array is the array or range of data that defines relative standing.
        /// </para>
        /// <para>
        /// K is the percentile value in the range 0..1, inclusive.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[1], out num, true))
            {
                return CalcErrors.Value;
            }
            List<double> list = new List<double>();
            if (CalcConvert.IsError(args[0]))
            {
                return args[0];
            }
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                for (int i = 0; i < ArrayHelper.GetLength(args[0], 0); i++)
                {
                    object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                    if (CalcConvert.IsNumber(obj2))
                    {
                        double item = CalcConvert.ToDouble(obj2);
                        list.Add(item);
                    }
                    else if (obj2 is CalcError)
                    {
                        return obj2;
                    }
                }
            }
            else
            {
                double num4;
                if (!CalcConvert.TryToDouble(args[0], out num4, true))
                {
                    return CalcErrors.Value;
                }
                list.Add(num4);
            }
            list.Sort();
            if (list.Count == 0)
            {
                return CalcErrors.Number;
            }
            if ((num < 0.0) || (1.0 < num))
            {
                return CalcErrors.Number;
            }
            double num5 = num * (list.Count - 1);
            double num6 = num5 % 1.0;
            if (num6 == 0.0)
            {
                return (double) list[(int) num5];
            }
            return (double) (((double) list[(int) num5]) + (num6 * (((double) list[((int) num5) + 1]) - ((double) list[(int) num5]))));
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
                return "PERCENTILE";
            }
        }
    }
}

