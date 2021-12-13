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
    /// Returns the <see cref="T:System.TimeSpan" /> for a particular time. 
    /// </summary>
    public class CalcTimeFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.TimeSpan" /> for a particular time.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: hour, minute, second.
        /// </para>
        /// <para>
        /// Hour is a number from 0 (zero) to 32767 representing the hour.
        /// Any value greater than 23 will be divided by 24 and the remainder will be
        /// treated as the hour value. For example, TIME(27,0,0) = TIME(3,0,0) = .125 or 3:00 AM.
        /// </para>
        /// <para>
        /// Minute is a number from 0 to 32767 representing the minute.
        /// Any value greater than 59 will be converted to hours and minutes.
        /// For example, TIME(0,750,0) = TIME(12,30,0) = .520833 or 12:30 PM.
        /// </para>
        /// <para>
        /// Second is a number from 0 to 32767 representing the second.
        /// Any value greater than 59 will be converted to hours, minutes, and seconds.
        /// For example, TIME(0,0,2000) = TIME(0,33,22) = .023148 or 12:33:20 AM
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num;
            int num2;
            int num3;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToInt(args[0], out num) || !CalcConvert.TryToInt(args[1], out num2)) || !CalcConvert.TryToInt(args[2], out num3))
            {
                return CalcErrors.Value;
            }
            try
            {
                TimeSpan span = new TimeSpan(num, num2, num3);
                if (span.Days > 0)
                {
                    span = new TimeSpan(span.Hours, span.Minutes, span.Seconds);
                }
                if (span < TimeSpan.Zero)
                {
                    return CalcErrors.Number;
                }
                return span;
            }
            catch (ArgumentOutOfRangeException)
            {
                return CalcErrors.Number;
            }
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
                return 3;
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
                return "TIME";
            }
        }
    }
}

