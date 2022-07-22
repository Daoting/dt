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
    /// Returns the mean of the interior of a data set.
    /// </summary>
    /// <remarks>
    /// RIMMEAN calculates the mean taken by excluding a percentage of data
    /// points from the top and bottom tails of a data set. You can use this
    /// function when you wish to exclude outlying data from your analysis.
    /// </remarks>
    public class CalcTrimMeanFunction : CalcBuiltinFunction
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
        /// Returns the mean of the interior of a data set.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array, percent.
        /// </para>
        /// <para>
        /// Array is the array or range of values to trim and average.
        /// </para>
        /// <para>
        /// Percent is the fractional number of data points to exclude from the calculation.
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
            double num2 = 0.0;
            List<double> list = new List<double>();
            if ((num < 0.0) || (1.0 <= num))
            {
                return CalcErrors.Number;
            }
            if (args[0] is CalcError)
            {
                return args[0];
            }
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                for (int j = 0; j < ArrayHelper.GetLength(args[0], 0); j++)
                {
                    object obj2 = ArrayHelper.GetValue(args[0], j, 0);
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
                double num5;
                if (!CalcConvert.TryToDouble(args[0], out num5, true))
                {
                    return CalcErrors.Value;
                }
                list.Add(num5);
            }
            list.Sort();
            int num6 = ((int) (list.Count * num)) / 2;
            for (int i = num6; i < (list.Count - num6); i++)
            {
                num2 += (double) list[i];
            }
            return (double) (num2 / ((double) (list.Count - (2 * num6))));
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
                return "TRIMMEAN";
            }
        }
    }
}

