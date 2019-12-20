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
    /// Returns the sum of a power series based on the formula.
    /// </summary>
    /// <remarks>
    /// Many functions can be approximated by a power series expansion.
    /// </remarks>
    public class CalcSeriesSumFunction : CalcBuiltinFunction
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
            return (i == 3);
        }

        /// <summary>
        /// Returns the sum of a power series based on the formula.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: x, n, m, coefficients.
        /// </para>
        /// <para>
        /// X is the input value to the power series.
        /// </para>
        /// <para>
        /// N is the initial power to which you want to raise x.
        /// </para>
        /// <para>
        /// M is the step by which to increase n for each term in the series.
        /// </para>
        /// <para>
        /// Coefficients   is a set of coefficients by which each successive
        /// power of x is multiplied.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            int num2;
            int num3;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToInt(args[1], out num3)) || !CalcConvert.TryToInt(args[2], out num2))
            {
                return CalcErrors.Value;
            }
            object o = args[3];
            double num4 = 0.0;
            for (int i = 0; i < ArrayHelper.GetLength(o, 0); i++)
            {
                double num6;
                if (!CalcConvert.TryToDouble(ArrayHelper.GetValue(o, i, 0), out num6, true))
                {
                    return CalcErrors.Value;
                }
                num4 += num6 * Math.Pow(num, (double) (num3 + (i * num2)));
            }
            return CalcConvert.ToResult(num4);
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
                return 4;
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
                return "SERIESSUM";
            }
        }
    }
}

