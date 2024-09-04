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
    /// Returns the <see cref="T:System.Double" /> payment on the principal for a given period for an investment based on periodic,
    /// constant payments and a constant interest rate.
    /// </summary>
    public class CalcPpmtFunction : CalcBuiltinFunction
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
            if (i != 4)
            {
                return (i == 5);
            }
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> payment on the principal for a given period for an investment based on periodic,
        /// constant payments and a constant interest rate.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 - 6 items: rate, per, nper, pv, [fv], [type].
        /// </para>
        /// <para>
        /// Rate is the interest rate per period.
        /// </para>
        /// <para>
        /// Per specifies the period and must be in the range 1 to nper.
        /// </para>
        /// <para>
        /// Nper is the total number of payment periods in an annuity.
        /// </para>
        /// <para>
        /// Pv is the present value ¡ª the total amount that a series of future payments is worth now.
        /// </para>
        /// <para>
        /// Fv is the future value, or a cash balance you want to attain after the last payment is made.
        /// If fv is omitted, it is assumed to be 0 (zero), that is, the future value of a loan is 0.
        /// </para>
        /// <para>
        /// Type is the number 0 or 1 and indicates when payments are due.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double rate = CalcConvert.ToDouble(args[0]);
            double num2 = CalcConvert.ToDouble(args[1]);
            double nper = CalcConvert.ToDouble(args[2]);
            double pv = CalcConvert.ToDouble(args[3]);
            double fv = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToDouble(args[4]) : 0.0;
            bool flag = CalcHelper.ArgumentExists(args, 5) ? CalcConvert.ToBool(args[5]) : false;
            if ((num2 < 1.0) || (num2 >= (nper + 1.0)))
            {
                return CalcErrors.Number;
            }
            double pmt = FinancialHelper.calculate_pmt(rate, nper, pv, fv, flag ? 1 : 0);
            double num7 = FinancialHelper.calculate_interest_part(pv, pmt, rate, num2 - 1.0);
            return (double) (pmt - num7);
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
                return "PPMT";
            }
        }
    }
}

