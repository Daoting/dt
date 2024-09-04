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
    /// Returns the <see cref="T:System.Double" /> depreciation for each accounting period. 
    /// </summary>
    public class CalcAmordegrcFunction : CalcBuiltinFunction
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
            return (i == 6);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> depreciation for each accounting period.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 6 - 7 items: cost, date_purchased, first_period, salvage, period, rate, [basis].
        /// </para>
        /// <para>
        /// Cost is the cost of the asset.
        /// </para>
        /// <para>
        /// Date_purchased is the date of the purchase of the asset.
        /// </para>
        /// <para>
        /// First_period is the date of the end of the first period.
        /// </para>
        /// <para>
        /// Salvage is the salvage value at the end of the life of the asset.
        /// </para>
        /// <para>
        /// Period is the period.
        /// </para>
        /// <para>
        /// Rate is the rate of depreciation.
        /// </para>
        /// <para>
        /// Basis is the year basis to be used.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double fCost = CalcConvert.ToDouble(args[0]);
            DateTime time = CalcConvert.ToDateTime(args[1]);
            DateTime time2 = CalcConvert.ToDateTime(args[2]);
            double fRestVal = CalcConvert.ToDouble(args[3]);
            int nPer = CalcConvert.ToInt(args[4]);
            double fRate = CalcConvert.ToDouble(args[5]);
            int nBase = CalcHelper.ArgumentExists(args, 6) ? CalcConvert.ToInt(args[6]) : 0;
            double num6 = 1.0 / fRate;
            if (((((num6 <= 0.0) || (num6 >= 1.0)) && ((num6 <= 1.0) || (num6 >= 2.0))) && (((num6 <= 2.0) || (num6 >= 3.0)) && ((num6 <= 4.0) || (num6 >= 5.0)))) && ((DateTime.Compare(time, time2) <= 0) && (((nBase >= 0) && (nBase <= 4)) && ((fRate > 0.0) && (nBase != 2)))))
            {
                return (double) this.get_amordegrc(fCost, time, time2, fRestVal, nPer, fRate, nBase);
            }
            return CalcErrors.Number;
        }

        private double get_amordegrc(double fCost, DateTime nDate, DateTime nFirstPer, double fRestVal, int nPer, double fRate, int nBase)
        {
            double num2;
            double num5 = 1.0 / fRate;
            if (num5 < 3.0)
            {
                num2 = 1.0;
            }
            else if (num5 < 5.0)
            {
                num2 = 1.5;
            }
            else if (num5 <= 6.0)
            {
                num2 = 2.0;
            }
            else
            {
                num2 = 2.5;
            }
            fRate *= num2;
            object obj2 = new CalcYearFracFunction().Evaluate(new object[] { nDate, nFirstPer, (int) nBase });
            if (obj2 is CalcError)
            {
                return 0.0;
            }
            double num6 = (double) ((double) obj2);
            double num3 = Math.Floor((double) (((num6 * fRate) * fCost) + 0.5));
            fCost -= num3;
            double num4 = fCost - fRestVal;
            for (int i = 0; i < nPer; i++)
            {
                num3 = Math.Floor((double) ((fRate * fCost) + 0.5));
                num4 -= num3;
                if (num4 < 0.0)
                {
                    switch ((nPer - i))
                    {
                        case 0:
                        case 1:
                            return Math.Floor((double) ((fCost * 0.5) + 0.5));
                    }
                    return 0.0;
                }
                fCost -= num3;
            }
            return num3;
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
                return 7;
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
                return "AMORDEGRC";
            }
        }
    }
}

