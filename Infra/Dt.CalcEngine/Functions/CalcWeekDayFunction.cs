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
    /// Returns the day of the week corresponding to a date. 
    /// The day is given as an <see cref="T:System.Int32" />, 
    /// ranging from 1 (Sunday) to 7 (Saturday), by default.
    /// </summary>
    public class CalcWeekDayFunction : CalcBuiltinFunction
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
            return (i == 1);
        }

        /// <summary>
        /// Returns the day of the week corresponding to a date.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: serial_num, [return_type].
        /// </para>
        /// <para>
        /// Serial_num is a sequential number that represents the date of the day you are trying to find.
        /// </para>
        /// <para>
        /// [Return_type] is a number that determines the type of return value.
        /// 1 means Numbers 1 (Sunday) through 7 (Saturday), 2 means Numbers 1 (Monday) through 7 (Sunday),
        /// 3 means Numbers 0 (Monday) through 6 (Sunday).  The default value is 1.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            DateTime time;
            int num;
            int dayOfWeek;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDateTime(args[0], out time, true) || !CalcConvert.TryToInt(CalcHelper.ArgumentExists(args, 1) ? args[1] : ((int) 1), out num))
            {
                return CalcErrors.Value;
            }
            switch (num)
            {
                case 1:
                    dayOfWeek = (int) (time.DayOfWeek + 1);
                    break;

                case 2:
                    if (time.DayOfWeek != DayOfWeek.Sunday)
                    {
                        dayOfWeek = (int) time.DayOfWeek;
                        break;
                    }
                    dayOfWeek = 7;
                    break;

                case 3:
                    if (time.DayOfWeek != DayOfWeek.Sunday)
                    {
                        dayOfWeek = (int) (time.DayOfWeek - ((DayOfWeek) 1));
                        break;
                    }
                    dayOfWeek = 6;
                    break;

                default:
                    return CalcErrors.Number;
            }
            return (int) dayOfWeek;
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
                return 1;
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
                return "WEEKDAY";
            }
        }
    }
}

