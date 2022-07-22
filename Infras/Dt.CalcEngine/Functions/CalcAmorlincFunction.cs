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
    public class CalcAmorlincFunction : CalcBuiltinFunction
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
            if ((DateTime.Compare(time, time2) <= 0) && (((nBase >= 0) && (nBase <= 4)) && ((fRate > 0.0) && (nBase != 2))))
            {
                return (double) this.get_amorlinc(fCost, time, time2, fRestVal, nPer, fRate, nBase);
            }
            return CalcErrors.Number;
        }

        private double get_amorlinc(double fCost, DateTime nDate, DateTime nFirstPer, double fRestVal, int nPer, double fRate, int nBase)
        {
            double num = fCost * fRate;
            double num2 = fCost - fRestVal;
            object obj2 = new CalcYearFracFunction().Evaluate(new object[] { nDate, nFirstPer, (int) nBase });
            if (!(obj2 is CalcError))
            {
                double num3 = (double) ((double) obj2);
                double num4 = (num3 * fRate) * fCost;
                double num5 = ((fCost - fRestVal) - num4) / num;
                int num6 = Convert.ToInt32((double) (((fCost - fRestVal) - num4) / num));
                if (nPer == 0)
                {
                    return num4;
                }
                if (nPer <= num6)
                {
                    return (num * ((num5 < 1.0) ? num5 : 1.0));
                }
                if (nPer == (num6 + 1))
                {
                    return ((num2 - (num * num6)) - num4);
                }
            }
            return 0.0;
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
                return "AMORLINC";
            }
        }
    }
}

