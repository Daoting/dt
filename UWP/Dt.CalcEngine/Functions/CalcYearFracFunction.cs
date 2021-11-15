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
    /// Calculates the fraction of the year represented by the number of whole days 
    /// between two dates (the start_date and the end_date). 
    /// Use the YEARFRAC worksheet function to identify the proportion of a 
    /// whole year's benefits or obligations to assign to a specific term.
    /// </summary>
    public class CalcYearFracFunction : CalcBuiltinFunction
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
            return (i == 2);
        }

        /// <summary>
        /// Calculates the fraction of the year represented by the number of whole days
        /// between two dates (the start_date and the end_date).
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: start_date, ,end_date, [basis].
        /// </para>
        /// <para>
        /// Start_date is a date that represents the start date.
        /// </para>
        /// <para>
        /// End_date is a date that represents the end date.
        /// </para>
        /// <para>
        /// [basis] is the type of day count basis to use.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            DateTime from = CalcConvert.ToDateTime(args[0]);
            DateTime to = CalcConvert.ToDateTime(args[1]);
            int basis = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToInt(args[2]) : 0;
            if ((basis >= 0) && (basis <= 4))
            {
                return (double) this.Yearfrac(from, to, basis);
            }
            return CalcErrors.Number;
        }

        internal double Yearfrac(DateTime from, DateTime to, int basis)
        {
            double num2;
            int num = FinancialHelper.Days_Between_Basis(from, to, basis);
            if (num < 0)
            {
                num = -num;
                DateTime time = from;
                from = to;
                to = time;
            }
            if (basis == 1)
            {
                int num5;
                int num6;
                int year = from.Year;
                int num4 = to.Year;
                DateTime time2 = from;
                time2.AddYears(1);
                if (DateTime.Compare(to, time2) > 0)
                {
                    num6 = (num4 + 1) - year;
                    time2 = new DateTime(year, 1, 1);
                    DateTime @this = new DateTime(num4 + 1, 1, 1);
                    num5 = Convert.ToInt32((double) (@this.ToOADate() - time2.ToOADate())) - (0x16d * ((num4 + 1) - year));
                }
                else
                {
                    num6 = 1;
                    if ((DateTime.IsLeapYear(year) && (from.Month < 3)) || (DateTime.IsLeapYear(num4) && (((to.Month * 0x100) + to.Day) >= 0x21d)))
                    {
                        num5 = 1;
                    }
                    else
                    {
                        num5 = 0;
                    }
                }
                double num7 = ((double) num5) / ((double) num6);
                num2 = 365.0 + num7;
            }
            else
            {
                num2 = FinancialHelper.Annual_Year_Basis(DateTime.Today, basis);
            }
            return (((double) num) / num2);
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
                return "YEARFRAC";
            }
        }
    }
}

