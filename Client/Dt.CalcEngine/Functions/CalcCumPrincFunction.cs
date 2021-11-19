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
    /// Returns the <see cref="T:System.Double" /> cumulative principal paid on a loan between start_period and end_period.
    /// </summary>
    public class CalcCumPrincFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> cumulative principal paid on a loan between start_period and end_period.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 6 items: rate, nper, pv, start_period, end_period, type.
        /// </para>
        /// <para>
        /// Rate is the interest rate.
        /// </para>
        /// <para>
        /// Nper is the total number of payment periods.
        /// </para>
        /// <para>
        /// Pv is the present value.
        /// </para>
        /// <para>
        /// Start_period is the first period in the calculation. Payment periods are numbered beginning with 1.
        /// </para>
        /// <para>
        /// End_period is the last period in the calculation.
        /// </para>
        /// <para>
        /// Type is the timing of the payment.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double fRate = CalcConvert.ToDouble(args[0]);
            int nNumPeriods = CalcConvert.ToInt(args[1]);
            double fVal = CalcConvert.ToDouble(args[2]);
            int nStart = CalcConvert.ToInt(args[3]);
            int nEnd = CalcConvert.ToInt(args[4]);
            int nPayType = CalcConvert.ToInt(args[5]);
            if ((((nStart >= 1) && (nEnd >= nStart)) && ((fRate > 0.0) && (nEnd <= nNumPeriods))) && (((nNumPeriods > 0) && (fVal > 0.0)) && ((nPayType == 0) || (nPayType == 1))))
            {
                return (double) this.get_cumprinc(fRate, nNumPeriods, fVal, nStart, nEnd, nPayType);
            }
            return CalcErrors.Number;
        }

        private double get_cumprinc(double fRate, int nNumPeriods, double fVal, int nStart, int nEnd, int nPayType)
        {
            double fRmz = FinancialHelper.GetRmz(fRate, (double) nNumPeriods, fVal, 0.0, nPayType);
            double num2 = 0.0;
            if (nStart == 1)
            {
                if (nPayType <= 0)
                {
                    num2 = fRmz + (fVal * fRate);
                }
                else
                {
                    num2 = fRmz;
                }
                nStart++;
            }
            for (int i = nStart; i <= nEnd; i++)
            {
                if (nPayType > 0)
                {
                    num2 += fRmz - ((FinancialHelper.GetZw(fRate, (double) (i - 2), fRmz, fVal, 1) - fRmz) * fRate);
                }
                else
                {
                    num2 += fRmz - (FinancialHelper.GetZw(fRate, (double) (i - 1), fRmz, fVal, 0) * fRate);
                }
            }
            return num2;
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
                return 6;
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
                return "CUMPRINC";
            }
        }
    }
}

