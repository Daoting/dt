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
    /// Returns the <see cref="T:System.TimeSpan" /> represented by a text string.
    /// </summary>
    public class CalcTimeValueFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.DateTime" /> of the date represented by date_text.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: time_text.
        /// </para>
        /// <para>
        /// Time_text is a text string that represents a time in any one of the Microsoft Excel time formats; for example, "6:45 PM" and "18:45" text strings within quotation marks that represent time.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            if (string.IsNullOrEmpty(str))
            {
                return CalcErrors.Value;
            }
            try
            {
                return TimeSpan.FromDays(DateTime.Parse(str, CultureInfo.CurrentCulture).TimeOfDay.TotalDays);
            }
            catch
            {
                return CalcErrors.Value;
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
                return "TIMEVALUE";
            }
        }
    }
}

