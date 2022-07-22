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
    /// Returns the <see cref="T:System.DateTime" /> that represents the date that is the
    /// indicated number of months before or after a specified date (the start_date). 
    /// Use EDATE to calculate maturity dates or due dates that fall on the same day
    /// of the month as the date of issue.
    /// </summary>
    public class CalcEDateFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.DateTime" /> that represents the date that is the
        /// indicated number of months before or after a specified date (the start_date).
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: start_date, months.
        /// </para>
        /// <para>
        /// Start_date is a date that represents the start date.
        /// </para>
        /// <para>
        /// Months is the number of months before or after start_date.
        /// A positive value for months yields a future date; a negative value yields a past date.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.DateTime" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            DateTime time;
            int num;
            base.CheckArgumentsLength(args);
            if (CalcConvert.TryToDateTime(args[0], out time, true) && CalcConvert.TryToInt(args[1], out num))
            {
                return time.AddMonths(num);
            }
            return CalcErrors.Value;
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
                return "EDATE";
            }
        }
    }
}

