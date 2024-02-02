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
    /// Returns the <see cref="T:System.Double" /> inverse of the lognormal cumulative distribution function of x, where ln(x) is normally distributed with parameters mean and standard_dev
    /// </summary>
    public class CalcLogInvFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> inverse of the lognormal cumulative distribution function of x, where ln(x) is normally distributed with parameters mean and standard_dev
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: probability, mean, standard_dev.
        /// </para>
        /// <para>
        /// Probability is a probability associated with the lognormal distribution.
        /// </para>
        /// <para>
        /// Mean is the mean of ln(x).
        /// </para>
        /// <para>
        /// Standard_dev is the standard deviation of ln(x).
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true)) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (((num3 <= 0.0) || (num < 0.0)) || (num > 1.0))
            {
                return CalcErrors.Number;
            }
            object obj2 = new CalcNormSInvFunction().Evaluate(new object[] { (double) num });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            double num4 = (double) ((double) obj2);
            return CalcConvert.ToResult(Math.Exp(num2 + (num3 * num4)));
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
                return "LOGINV";
            }
        }
    }
}

