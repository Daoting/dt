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
    /// Returns the number of whole working days between start_date and end_date.
    /// Working days exclude weekends and any dates identified in holidays. 
    /// Use NETWORKDAYS to calculate employee benefits that accrue based on 
    /// the number of days worked during a specific term.
    /// </summary>
    public class CalcNetWorkdaysFunction : CalcBuiltinFunction
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
        /// Returns the number of whole working days between start_date and end_date.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: start_date, ,end_date, [holidays].
        /// </para>
        /// <para>
        /// Start_date is a date that represents the start date.
        /// </para>
        /// <para>
        /// End_date is a date that represents the end date.
        /// </para>
        /// <para>
        /// [Holidays] is an optional range of one or more dates to exclude from the working calendar,
        /// such as state and federal holidays and floating holidays.
        /// The list can be either a range of cells that contains the dates or an array
        /// constant (array: Used to build single formulas that produce multiple results or
        /// that operate on a group of arguments that are arranged in rows and columns.
        /// An array range shares a common formula; an array constant is a group of constants
        /// used as an argument.) of the serial numbers that represent the dates.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            DateTime @this = CalcConvert.ToDateTime(args[0]);
            DateTime time2 = CalcConvert.ToDateTime(args[1]);
            int num = CalcConvert.ToInt((double) @this.ToOADate());
            int num2 = CalcConvert.ToInt((double) time2.ToOADate());
            bool flag = false;
            if (num > num2)
            {
                DateTime time3 = @this;
                int num3 = num;
                num = num2;
                num2 = num3;
                @this = time2;
                time2 = time3;
                flag = true;
            }
            int res = (num2 - num) + 1;
            res = RemoveWeekends(@this, time2, res);
            if (res <= 0)
            {
                return (int) 0;
            }
            res = this.RemoveHolidaies(args, num, num2, res);
            res = flag ? -res : res;
            return (int) res;
        }

        private bool IsWeekEnd(DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday)
            {
                return (date.DayOfWeek == DayOfWeek.Sunday);
            }
            return true;
        }

        private int RemoveHolidaies(object[] args, int start_serial, int end_serial, int res)
        {
            int num = CalcHelper.ArgumentExists(args, 2) ? ArrayHelper.GetLength(args[2], 0) : 0;
            List<DateTime> list = new List<DateTime>();
            for (int i = 0; i < num; i++)
            {
                DateTime @this = CalcConvert.ToDateTime(ArrayHelper.GetValue(args[2], i, 0));
                double num3 = @this.ToOADate();
                if ((!list.Contains(@this) && !this.IsWeekEnd(@this)) && ((num3 >= start_serial) && (num3 <= end_serial)))
                {
                    list.Add(@this);
                }
            }
            res -= list.Count;
            return res;
        }

        private static int RemoveWeekends(DateTime start_date, DateTime end_date, int res)
        {
            int num = ((int) (end_date.ToOADate() - start_date.ToOADate())) % 7;
            CalcWeekDayFunction function = new CalcWeekDayFunction();
            int num2 = ((int) function.Evaluate(new object[] { start_date, (int) 2 })) + num;
            num = (num2 > 5) ? (num2 - 5) : 0;
            int num3 = (start_date.DayOfWeek == DayOfWeek.Sunday) ? 1 : 2;
            num = (num > num3) ? num3 : num;
            res -= num;
            int num4 = (res / 7) * 2;
            res -= num4;
            return res;
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
                return "NETWORKDAYS";
            }
        }
    }
}

