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
    /// Returns the Poisson distribution.
    /// </summary>
    /// <remarks>
    /// A common application of the Poisson distribution is predicting the 
    /// number of events over a specific time, such as the number of cars
    /// arriving at a toll plaza in 1 minute.
    /// </remarks>
    public class CalcPoissonFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the Poisson distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: x, mean, cumulative.
        /// </para>
        /// <para>
        /// X  is the number of events.
        /// </para>
        /// <para>
        /// Mean  is the expected numeric value.
        /// </para>
        /// <para>
        /// Cumulative  is a logical value that determines the form of the
        /// probability distribution returned.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num;
            double num2;
            bool flag;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToInt(args[0], out num))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToDouble(args[1], out num2, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToBool(args[2], out flag))
            {
                return CalcErrors.Value;
            }
            double num3 = 0.0;
            if ((num <= 0) || (num2 <= 0.0))
            {
                return CalcErrors.Number;
            }
            if (flag)
            {
                for (int i = 0; i <= num; i++)
                {
                    num3 += (Math.Exp(-num2) * Math.Pow(num2, (double) i)) / this.Fact(i);
                }
            }
            else
            {
                num3 = (Math.Exp(-num2) * Math.Pow(num2, (double) num)) / this.Fact(num);
            }
            return CalcConvert.ToResult(num3);
        }

        private double Fact(int x)
        {
            double num = 1.0;
            for (int i = x; i > 1; i--)
            {
                num *= i;
            }
            return num;
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
                return "POISSON";
            }
        }
    }
}

