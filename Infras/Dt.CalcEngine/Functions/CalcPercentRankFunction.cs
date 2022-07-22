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
    /// Returns the rank of a value in a data set as a percentage of the data set.
    /// </summary>
    /// <remarks>
    /// This function can be used to evaluate the relative standing of a value 
    /// within a data set.
    /// </remarks>
    public class CalcPercentRankFunction : CalcBuiltinFunction
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
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
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
            return (i == 0);
        }

        /// <summary>
        /// Returns the rank of a value in a data set as a percentage of the data set.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: array, x, [significance].
        /// </para>
        /// <para>
        /// Array is the array or range of data with numeric values that defines relative standing.
        /// </para>
        /// <para>
        /// X is the value for which you want to know the rank.
        /// </para>
        /// <para>
        /// [Significance] is an optional value that identifies the number
        /// of significant digits for the returned percentage value.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num4;
            base.CheckArgumentsLength(args);
            object o = args[0];
            if (!CalcConvert.TryToDouble(args[1], out num, true))
            {
                return CalcErrors.Value;
            }
            int result = 3;
            if (CalcHelper.ArgumentExists(args, 2) && !CalcConvert.TryToInt(args[2], out result))
            {
                return CalcErrors.Value;
            }
            double num3 = num;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            double num8 = num3;
            double num9 = num3;
            if (result < 1)
            {
                return CalcErrors.Number;
            }
            for (int i = 0; i < ArrayHelper.GetLength(o, 0); i++)
            {
                object obj3 = ArrayHelper.GetValue(o, i, 0);
                if (obj3 != null)
                {
                    double num11;
                    if (!CalcConvert.TryToDouble(obj3, out num11, true))
                    {
                        return CalcErrors.Value;
                    }
                    if (num11 < num3)
                    {
                        num5++;
                        if ((num8 == num3) || (num8 < num11))
                        {
                            num8 = num11;
                        }
                    }
                    else if (num11 > num3)
                    {
                        num6++;
                        if ((num9 == num3) || (num9 > num11))
                        {
                            num9 = num11;
                        }
                    }
                    else
                    {
                        num7++;
                    }
                }
            }
            if (((num5 + num7) == 0) || ((num6 + num7) == 0))
            {
                return CalcErrors.NotAvailable;
            }
            if (((num6 == 0) && (num5 == 0)) && (num7 != 0))
            {
                return (double) 1.0;
            }
            if (num7 == 1)
            {
                num4 = ((double) num5) / ((double) (num5 + num6));
            }
            else if (num7 == 0)
            {
                double num12 = (num3 - num8) / (num9 - num8);
                num4 = ((num5 + num12) - 1.0) / ((num6 + num5) - 1.0);
            }
            else
            {
                num4 = (num5 + (0.5 * num7)) / ((num5 + num7) + num6);
            }
            return (double) Math.Round(num4, (result > 15) ? 15 : result);
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
                return "PERCENTRANK";
            }
        }
    }
}

