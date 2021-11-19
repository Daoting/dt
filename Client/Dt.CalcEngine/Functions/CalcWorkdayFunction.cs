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
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns a number that represents a date that is the indicated number of 
    /// working days before or after a date (the starting date). 
    /// Working days exclude weekends and any dates identified as holidays.
    /// Use WORKDAY to exclude weekends or holidays when you calculate invoice due dates, 
    /// expected delivery times, or the number of days of work performed.
    /// </summary>
    public class CalcWorkdayFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsArray(int i)
        {
            return (i == 2);
        }

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
        /// Indicates whether the Evaluate method can process references.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process references; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsReference(int i)
        {
            return (i == 2);
        }

        /// <summary>
        /// Returns a number that represents a date that is the indicated number of
        /// working days before or after a date (the starting date).
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: start_date, days, [holidays].
        /// </para>
        /// <para>
        /// Start_date is a date that represents the start date.
        /// </para>
        /// <para>
        /// Days is the number of nonweekend and nonholiday days before or after start_date.
        /// A positive value for days yields a future date; a negative value yields a past date.
        /// </para>
        /// <para>
        /// [Holidays] is an optional list of one or more dates to exclude from the working calendar,
        /// such as state and federal holidays and floating holidays.
        /// The list can be either a range of cells that contain the dates or an
        /// array constant (array: Used to build single formulas that produce multiple
        /// results or that operate on a group of arguments that are arranged in rows and columns.
        /// An array range shares a common formula; an array constant is a group of constants
        /// used as an argument.) of the serial numbers that represent the dates.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            DateTime item = CalcConvert.ToDateTime(args[0]);
            int num = CalcConvert.ToInt(args[1]);
            int num2 = CalcHelper.ArgumentExists(args, 2) ? ArrayHelper.GetLength(args[2], 0) : 0;
            List<DateTime> list = new List<DateTime>();
            for (int i = 0; i < num2; i++)
            {
                list.Add(CalcConvert.ToDateTime(ArrayHelper.GetValue(args[2], i, 0)));
            }
            object obj2 = new CalcWeekDayFunction().Evaluate(new object[] { item, (int) 3 });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            int num4 = CalcConvert.ToInt(obj2);
            while (num < 0)
            {
                try
                {
                    item = item.AddDays(-1.0);
                }
                catch
                {
                    return CalcErrors.Number;
                }
                if (num4 == 0)
                {
                    num4 = 6;
                }
                else
                {
                    num4--;
                }
                if ((num4 == 5) || (num4 == 6))
                {
                    num--;
                }
                else if (list.Contains(item))
                {
                    num--;
                }
                num++;
            }
            while (num > 0)
            {
                try
                {
                    item = item.AddDays(1.0);
                }
                catch
                {
                    return CalcErrors.Number;
                }
                if (num4 == 6)
                {
                    num4 = 0;
                }
                else
                {
                    num4++;
                }
                if ((num4 == 5) || (num4 == 6))
                {
                    num++;
                }
                else if (list.Contains(item))
                {
                    num++;
                }
                num--;
            }
            return (double) item.ToOADate();
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
                return "WORKDAY";
            }
        }
    }
}

