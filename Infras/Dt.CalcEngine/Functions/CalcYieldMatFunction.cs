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
    /// Returns the <see cref="T:System.Double" /> annual yield of a security that pays interest at maturity.
    /// </summary>
    public class CalcYieldMatFunction : CalcBuiltinFunction
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
            return (i == 5);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> annual yield of a security that pays interest at maturity.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 5 - 6 items: settlement, maturity, issue, rate, pr, [basis].
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
        /// Issue is the security's issue date, expressed as a serial date number.
        /// </para>
        /// <para>
        /// Rate is the security's interest rate at date of issue.
        /// </para>
        /// <para>
        /// Pr is the security's price per $100 face value.
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
            DateTime nSettle = CalcConvert.ToDateTime(args[0]);
            DateTime nMat = CalcConvert.ToDateTime(args[1]);
            DateTime nIssue = CalcConvert.ToDateTime(args[2]);
            double fRate = CalcConvert.ToDouble(args[3]);
            double fPrice = CalcConvert.ToDouble(args[4]);
            int nBase = CalcHelper.ArgumentExists(args, 5) ? CalcConvert.ToInt(args[5]) : 0;
            if (((nBase >= 0) && (nBase <= 4)) && (fRate >= 0.0))
            {
                return (double) this.get_yieldmat(nSettle, nMat, nIssue, fRate, fPrice, nBase);
            }
            return CalcErrors.Number;
        }

        private double get_yieldmat(DateTime nSettle, DateTime nMat, DateTime nIssue, double fRate, double fPrice, int nBase)
        {
            CalcYearFracFunction function = new CalcYearFracFunction();
            double num = function.Yearfrac(nIssue, nMat, nBase);
            double num2 = function.Yearfrac(nIssue, nSettle, nBase);
            double num3 = function.Yearfrac(nSettle, nMat, nBase);
            double num4 = 1.0 + (num * fRate);
            num4 /= (fPrice / 100.0) + (num2 * fRate);
            num4--;
            return (num4 / num3);
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
                return 6;
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
                return 5;
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
                return "YIELDMAT";
            }
        }
    }
}

