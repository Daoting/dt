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
    /// Returns the day of a date, represented by a serial number. 
    /// The day is given as an <see cref="T:System.Int32" /> ranging from 1 to 31.
    /// </summary>
    public class CalcDayFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the day of a date, represented by a serial number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: serial_number.
        /// </para>
        /// <para>
        /// Serial_number is the date of the day you are trying to find.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            DateTime time;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDateTime(args[0], out time, true))
            {
                return CalcErrors.Value;
            }
            return (int) time.Day;
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
                return 1;
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
                return "DAY";
            }
        }
    }
}

