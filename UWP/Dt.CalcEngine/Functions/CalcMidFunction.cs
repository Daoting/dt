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
    /// Returns a specific number of characters from a text string, 
    /// starting at the position you specify, based on the number of characters you specify.
    /// </summary>
    public class CalcMidFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns a specific number of characters from a text string,
        /// starting at the position you specify, based on the number of characters you specify.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: text, start_num, num_chars.
        /// </para>
        /// <para>
        /// Text is the text string containing the characters you want to extract.
        /// </para>
        /// <para>
        /// Start_num is the position of the first character you want to extract in text.
        /// The first character in text has start_num 1, and so on.
        /// </para>
        /// <para>
        /// Num_chars specifies the number of characters you want MID to return from text.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            int startIndex = CalcConvert.ToInt(args[1]) - 1;
            int length = CalcConvert.ToInt(args[2]);
            if ((startIndex < 0) || (length < 0))
            {
                return CalcErrors.Value;
            }
            if (startIndex >= str.Length)
            {
                return "";
            }
            if (str.Length < (startIndex + length))
            {
                return str.Substring(startIndex);
            }
            return str.Substring(startIndex, length);
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
                return "MID";
            }
        }
    }
}

