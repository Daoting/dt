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
using System.Globalization;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns a number that indicates where the week falls numerically within a year.
    /// </summary>
    /// <remarks>
    /// The WEEKNUM function considers the week containing January 1 to be the first week of the year. 
    /// However, there is a European standard that defines the first week as the one 
    /// with the majority of days (four or more) falling in the new year.
    /// This means that for years in which there are three days or less in the first week of January, 
    /// the WEEKNUM function returns week numbers that are incorrect according to the European standard.
    /// </remarks>
    public class CalcWeekNumFunction : CalcBuiltinFunction
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
        /// Returns a number that indicates where the week falls numerically within a year.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: serial_num, [return_type].
        /// </para>
        /// <para>
        /// Serial_num is a date within the week.
        /// </para>
        /// <para>
        /// [Return_type] is a number that determines on which day the week begins.
        /// 1 means Week begins on Sunday, 2 means Week begins on Monday.  The default value is 1.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            DateTime time;
            int num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDateTime(args[0], out time, true) || !CalcConvert.TryToInt(CalcHelper.ArgumentExists(args, 1) ? args[1] : ((int) 1), out num))
            {
                return CalcErrors.Value;
            }
            GregorianCalendar calendar = new GregorianCalendar();
            switch (num)
            {
                case 1:
                    return (int) calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                case 2:
                    return (int) calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            }
            return CalcErrors.Number;
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
                return "WEEKNUM";
            }
        }
    }
}

