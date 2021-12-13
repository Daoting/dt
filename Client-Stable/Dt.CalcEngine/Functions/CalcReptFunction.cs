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
using System.Text;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Repeats text a given number of times. 
    /// </summary>
    public class CalcReptFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Repeats text a given number of times.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: text, number_times.
        /// </para>
        /// <para>
        /// Text is the text you want to repeat.
        /// </para>
        /// <para>
        /// Number_times is a positive number specifying the number of times to repeat text.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            int num = CalcConvert.ToInt(args[1]);
            if ((num < 0) || (0x7fff < (num * str.Length)))
            {
                return CalcErrors.Value;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                builder.Append(str);
            }
            return builder.ToString();
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
                return "REPT";
            }
        }
    }
}

