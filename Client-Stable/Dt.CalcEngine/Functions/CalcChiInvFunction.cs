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
    /// Returns the <see cref="T:System.Double" /> inverse of the one-tailed probability of the chi-squared distribution. 
    /// </summary>
    public class CalcChiInvFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> inverse of the one-tailed probability of the chi-squared distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: probability, degrees_freedom.
        /// </para>
        /// <para>
        /// Probability is a probability associated with the chi-squared distribution.
        /// </para>
        /// <para>
        /// Degrees_freedom is the number of degrees of freedom.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num2;
            int num3;
            base.CheckArgumentsLength(args);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
            }
            if (!CalcConvert.TryToDouble(args[0], out num2, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToInt(args[1], out num3))
            {
                return CalcErrors.Value;
            }
            if ((num2 < 0.0) || (num2 > 1.0))
            {
                return CalcErrors.Number;
            }
            if ((num3 < 1.0) || (num3 > Math.Pow(10.0, 10.0)))
            {
                return CalcErrors.Number;
            }
            double num4 = 1.0 - num2;
            CalcBuiltinFunction function = new CalcGammaInvFunction();
            return (double) function.Evaluate(new object[] { (double) num4, (double) (0.5 * num3), (double) 2.0 });
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
                return "CHIINV";
            }
        }
    }
}

