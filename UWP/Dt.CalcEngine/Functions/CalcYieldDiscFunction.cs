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
    /// Returns the <see cref="T:System.Double" /> annual yield for a discounted security.
    /// </summary>
    public class CalcYieldDiscFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 4);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> annual yield for a discounted security.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 - 5 items: settlement, maturity, pr, redemption, [basis].
        /// </para>
        /// <para>
        /// Settlement is the security's settlement date.
        /// The security settlement date is the date after the issue date when the security is traded to the buyer.
        /// </para>
        /// <para>
        /// Maturity is the security's maturity date.
        /// The maturity date is the date when the security expires.
        /// </para>
        /// <para>
        /// Pr is the security's price per $100 face value.
        /// </para>
        /// <para>
        /// Redemption is the security's redemption value per $100 face value.
        /// </para>
        /// <para>
        /// Basis is the type of day count basis to use.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            DateTime time = CalcConvert.ToDateTime(args[0]);
            DateTime time2 = CalcConvert.ToDateTime(args[1]);
            double num = CalcConvert.ToDouble(args[2]);
            double num2 = CalcConvert.ToDouble(args[3]);
            int basis = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToInt(args[4]) : 0;
            CalcYearFracFunction function = new CalcYearFracFunction();
            if ((basis < 0) || (basis > 4))
            {
                return CalcErrors.Number;
            }
            if (((num2 <= 0.0) || (num <= 0.0)) || (DateTime.Compare(time, time2) >= 0))
            {
                return CalcErrors.Number;
            }
            double num4 = (num2 / num) - 1.0;
            double num5 = function.Yearfrac(time, time2, basis);
            return (double) (num4 / num5);
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
                return 5;
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
                return "YIELDDISC";
            }
        }
    }
}

