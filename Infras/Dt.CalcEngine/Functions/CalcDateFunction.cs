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
    /// Returns the <see cref="T:System.DateTime" /> that represents a particular date.
    /// </summary>
    public class CalcDateFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.DateTime" /> that represents a particular date.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: year, month, day.
        /// </para>
        /// <para>
        /// year The year argument can be one to four digits.
        /// Spread uses the 1900 date system.
        /// </para>
        /// <para>
        /// month is a positive or negative integer representing the month of the year from 1 to 12 (January to December).
        /// </para>
        /// <para>
        /// day is a positive or negative integer representing the day of the month from 1 to 31.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.DateTime" /> value that indicates the evaluate result.
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
                if ((num < 0) || (0x270f < num))
                {
                    return CalcErrors.Number;
                }
                if (num <= 0x76b)
                {
                    num += 0x76c;
                }
                DateTime time = new DateTime(num, 1, 1);
                time = time.AddMonths(num2 - 1).AddDays((double) (num3 - 1));
                if (time < new DateTime(0x76b, 12, 30))
                {
                    return CalcErrors.Number;
                }
                return time;
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
                return "DATE";
            }
        }
    }
}

