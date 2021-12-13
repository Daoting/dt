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
    /// Returns the probability that values in a range are between two limits.
    /// </summary>
    /// <remarks>
    /// If upper_limit is not supplied, returns the probability that values in
    /// x_range are equal to lower_limit.
    /// </remarks>
    public class CalcProbFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsArray(int i)
        {
            if (i != 0)
            {
                return (i == 1);
            }
            return true;
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 3);
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
            if (i != 0)
            {
                return (i == 1);
            }
            return true;
        }

        /// <summary>
        /// Returns the probability that values in a range are between two limits.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 4 items: x_range, prob_range, lower_limit, [upper_limit].
        /// </para>
        /// <para>
        /// X_range is the range of numeric values of x with which there are associated probabilities.
        /// </para>
        /// <para>
        /// Prob_range is a set of probabilities associated with values in x_range.
        /// </para>
        /// <para>
        /// Lower_limit is the lower bound on the value for which you want a probability.
        /// </para>
        /// <para>
        /// [Upper_limit] is the optional upper bound on the value for which you want a probability.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[2], out num, true))
            {
                return CalcErrors.Value;
            }
            double number = num;
            if (CalcHelper.ArgumentExists(args, 3) && !CalcConvert.TryToDouble(args[3], out number, true))
            {
                return CalcErrors.Value;
            }
            double num3 = 0.0;
            double num4 = 0.0;
            int length = ArrayHelper.GetLength(args[0], 0);
            if (length != ArrayHelper.GetLength(args[1], 0))
            {
                return CalcErrors.NotAvailable;
            }
            for (int i = 0; i < length; i++)
            {
                if ((ArrayHelper.GetValue(args[0], i, 0) != null) && (ArrayHelper.GetValue(args[1], i, 0) != null))
                {
                    double num7;
                    double num8;
                    if (!CalcConvert.TryToDouble(ArrayHelper.GetValue(args[0], i, 0), out num7, true) || !CalcConvert.TryToDouble(ArrayHelper.GetValue(args[1], i, 0), out num8, true))
                    {
                        return CalcErrors.Value;
                    }
                    if ((num8 <= 0.0) || (1.0 < num8))
                    {
                        return CalcErrors.Number;
                    }
                    if ((num <= num7) && (num7 <= number))
                    {
                        num3 += num8;
                    }
                    num4 += num8;
                }
            }
            if (num4 != 1.0)
            {
                return CalcErrors.Number;
            }
            return (double) num3;
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
                return 4;
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
                return 3;
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
                return "PROB";
            }
        }
    }
}

