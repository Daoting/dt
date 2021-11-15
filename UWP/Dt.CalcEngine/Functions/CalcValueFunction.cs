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
    /// Converts a text string that represents a number to a number.
    /// </summary>
    /// <remarks>
    /// Text can be in any of the constant number, date, or time formats
    /// recognized by Microsoft Excel. If text is not in one of these formats, 
    /// VALUE returns the #VALUE! error value. 
    /// </remarks>
    public class CalcValueFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Converts a text string that represents a number to a number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: text.
        /// </para>
        /// <para>
        /// Text is the text enclosed in quotation marks or a
        /// reference to a cell containing the text you want to convert.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            TimeSpan span;
            DateTime time;
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            if (CalcConvert.TryToDouble(str, out num, true))
            {
                return (double) num;
            }
            if (CalcConvert.TryToTimeSpan(str, out span, true))
            {
                return (double) span.TotalDays;
            }
            if (CalcConvert.TryToDateTime(str, out time, true))
            {
                return (double) time.ToOADate();
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
                return "VALUE";
            }
        }
    }
}

