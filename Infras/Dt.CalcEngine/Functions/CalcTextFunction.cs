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
    /// Converts a value to text in a specific number format.
    /// </summary>
    public class CalcTextFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Converts a value to text in a specific number format.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: value, format_text.
        /// </para>
        /// <para>
        /// Value is a numeric value, a formula that evaluates to a numeric value,
        /// or a reference to a cell containing a numeric value.
        /// </para>
        /// <para>
        /// Format_text is a numeric format as a text string enclosed in quotation marks.
        /// You can see various numeric formats by clicking the Number,
        /// Date, Time, Currency, or Custom in the Category box of the Number tab
        /// in the Format Cells dialog box, and then viewing the formats displayed.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object obj2 = args[0];
            string str = CalcConvert.ToString(args[1]);
            if (string.IsNullOrEmpty(str))
            {
                return CalcConvert.ToString(obj2);
            }
            if (obj2 == null)
            {
                obj2 = 0;
            }
            str = this.RemoveColorFormat(str);
            string str2 = string.Empty;
            try
            {
                FormatHelper.CustomNumberFormat format = new FormatHelper.CustomNumberFormat(str);
                return format.Format(obj2);
            }
            catch
            {
            }
            try
            {
                str2 = string.Format("{0:" + str + "}", new object[] { obj2 });
            }
            catch (FormatException)
            {
                return CalcErrors.Value;
            }
            return str2;
        }

        private string RemoveColorFormat(string format)
        {
            if (format.Contains("[") && format.Contains("]"))
            {
                int index = format.IndexOf("[");
                for (int i = format.IndexOf("]", index); (index != -1) && (i != -1); i = (index == -1) ? -1 : format.IndexOf("]", index))
                {
                    format = format.Remove(index, (i - index) + 1);
                    index = format.IndexOf("[");
                }
            }
            return format;
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
                return "TEXT";
            }
        }
    }
}

