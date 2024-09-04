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
    /// Returns the <see cref="T:System.DateTime" /> of the date represented by date_text.
    /// </summary>
    public class CalcDateValueFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.DateTime" /> of the date represented by date_text.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: date_text.
        /// </para>
        /// <para>
        /// Date_text is text that represents a date in .Net date format.
        /// For example, "1/30/2008" or "30-Jan-2008" are text strings within
        /// quotation marks that represent dates. Date_text must represent a date from
        /// January 1, 1900, to December 31, 9999.
        /// error value if date_text is out of this range.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.DateTime" /> value that indicates the evaluate result.
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
                return DateTime.Parse(str, CultureInfo.CurrentCulture);
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
                return "DATEVALUE";
            }
        }
    }
}

