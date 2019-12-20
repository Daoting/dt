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
    /// Returns the hour of a time value. The hour is given as an <see cref="T:System.Int32" />,
    /// ranging from 0 (12:00 A.M.) to 23 (11:00 P.M.).
    /// </summary>
    public class CalcHourFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the hour of a time value. The hour is given as an <see cref="T:System.Int32" />,
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: serial_number.
        /// </para>
        /// <para>
        /// Serial_number is the time that contains the hour you want to find.
        /// Times may be entered as text strings within quotation marks (for example, "6:45 PM"),
        /// as decimal numbers (for example, 0.78125, which represents 6:45 PM),
        /// or as results of other formulas or functions (for example, TIMEVALUE("6:45 PM")).
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
            return (int) time.Hour;
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
                return "HOUR";
            }
        }
    }
}

