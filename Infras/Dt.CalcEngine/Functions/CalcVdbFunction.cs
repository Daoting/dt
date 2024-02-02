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
    /// Returns the <see cref="T:System.Double" /> amount an asset depreciates in a single period using different declining balance methods.
    /// </summary>
    public class CalcVdbFunction : CalcBuiltinFunction
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
            if (i != 5)
            {
                return (i == 6);
            }
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> amount an asset depreciates in a single period using different declining balance methods.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 5 - 7 items: cost, salvage, life, start_period, end_period, [factor], [no_switch].
        /// </para>
        /// <para>
        /// Cost is the initial cost of the asset.
        /// </para>
        /// <para>
        /// Salvage is the value at the end of the depreciation (sometimes called the salvage value of the asset). This value can be 0.
        /// </para>
        /// <para>
        /// Life is the number of periods over which the asset is depreciated (sometimes called the useful life of the asset).
        /// </para>
        /// <para>
        /// Start_period is the starting period for which you want to calculate the depreciation. Start_period must use the same units as life.
        /// </para>
        /// <para>
        /// End_period is the ending period for which you want to calculate the depreciation. End_period must use the same units as life.
        /// </para>
        /// <para>
        /// Factor is the rate at which the balance declines.
        /// </para>
        /// <para>
        /// No_switch is a logical value specifying whether to switch to straight-line depreciation when depreciation is greater than the declining balance calculation.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double cost = CalcConvert.ToDouble(args[0]);
            double salvage = CalcConvert.ToDouble(args[1]);
            int num3 = CalcConvert.ToInt(args[2]);
            double num4 = CalcConvert.ToDouble(args[3]);
            double num5 = CalcConvert.ToDouble(args[4]);
            double factor = CalcHelper.ArgumentExists(args, 5) ? CalcConvert.ToDouble(args[5]) : 2.0;
            bool flag = CalcHelper.ArgumentExists(args, 6) ? CalcConvert.ToBool(args[6]) : false;
            if ((((cost < 0.0) || (salvage < 0.0)) || ((num3 < 0) || (num4 < 0.0))) || ((num5 < 0.0) || (num5 < num4)))
            {
                return CalcErrors.Number;
            }
            if (((cost < salvage) && (num4 == 0.0)) && (num5 == 1.0))
            {
                return (double) (cost - salvage);
            }
            return (double) this.get_vdb(cost, salvage, (double) num3, (double) num4, (double) num5, factor, flag);
        }

        private double get_vdb(double cost, double salvage, double life, double start_period, double end_period, double factor, bool flag)
        {
            double num2 = Math.Floor(start_period);
            double num3 = Math.Ceiling(end_period);
            int num5 = (int) num2;
            int num6 = (int) num3;
            double num = 0.0;
            if (flag)
            {
                for (int i = num5 + 1; i <= num6; i++)
                {
                    double num7 = this.ScGetGDA(cost, salvage, life, (double) i, factor);
                    if (i == (num5 + 1))
                    {
                        num7 *= Math.Min(end_period, num2 + 1.0) - start_period;
                    }
                    else if (i == num6)
                    {
                        num7 *= (end_period + 1.0) - num3;
                    }
                    num += num7;
                }
                return num;
            }
            double num8 = life;
            if (((start_period != Math.Floor(start_period)) && (factor > 1.0)) && (start_period >= (life / 2.0)))
            {
                double num9 = start_period - (life / 2.0);
                start_period = life / 2.0;
                end_period -= num9;
                num8++;
            }
            cost -= this.ScInterVDB(cost, salvage, life, num8, start_period, factor);
            return this.ScInterVDB(cost, salvage, life, life - start_period, end_period - start_period, factor);
        }

        internal double ScGetGDA(double fWert, double fRest, double fDauer, double fPeriode, double fFaktor)
        {
            double num;
            double num3;
            double num2 = fFaktor / fDauer;
            if (num2 >= 1.0)
            {
                num2 = 1.0;
                if (fPeriode == 1.0)
                {
                    num3 = fWert;
                }
                else
                {
                    num3 = 0.0;
                }
            }
            else
            {
                num3 = fWert * Math.Pow(1.0 - num2, fPeriode - 1.0);
            }
            double num4 = fWert * Math.Pow(1.0 - num2, fPeriode);
            if (num4 < fRest)
            {
                num = num3 - fRest;
            }
            else
            {
                num = num3 - num4;
            }
            if (num < 0.0)
            {
                num = 0.0;
            }
            return num;
        }

        internal double ScInterVDB(double cost, double salvage, double life, double life1, double period, double factor)
        {
            double num = 0.0;
            double num2 = Math.Ceiling(period);
            int num3 = (int) num2;
            double num6 = cost - salvage;
            bool flag = false;
            double num5 = 0.0;
            for (int i = 1; i <= num3; i++)
            {
                double num4;
                if (!flag)
                {
                    double num7 = this.ScGetGDA(cost, salvage, life, (double) i, factor);
                    num5 = num6 / (life1 - (i - 1));
                    if (num5 > num7)
                    {
                        num4 = num5;
                        flag = true;
                    }
                    else
                    {
                        num4 = num7;
                        num6 -= num7;
                    }
                }
                else
                {
                    num4 = num5;
                }
                if (i == num3)
                {
                    num4 *= (period + 1.0) - num2;
                }
                num += num4;
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
                return "VDB";
            }
        }
    }
}

