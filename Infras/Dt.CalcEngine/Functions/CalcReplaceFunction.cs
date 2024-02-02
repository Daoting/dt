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
    /// Replaces part of a text string, based on the number of 
    /// characters you specify, with a different text string.
    /// </summary>
    public class CalcReplaceFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 2);
        }

        /// <summary>
        /// Replaces part of a text string, based on the number of
        /// characters you specify, with a different text string.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: old_text,start_num, num_chars, new_text.
        /// </para>
        /// <para>
        /// Old_text is text in which you want to replace some characters.
        /// </para>
        /// <para>
        /// Start_num is the position of the character in old_text
        /// that you want to replace with new_text.
        /// </para>
        /// <para>
        /// Num_chars is the number of characters in old_text
        /// that you want REPLACE to replace with new_text.
        /// </para>
        /// <para>
        /// New_text is the text that will replace characters in old_text.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            int num = CalcConvert.ToInt(args[1]);
            int num2 = CalcConvert.ToInt(args[2]);
            string str2 = CalcConvert.ToString(args[3]);
            if ((num < 1) || (num2 < 0))
            {
                return CalcErrors.Value;
            }
            StringBuilder builder = new StringBuilder(str);
            num = Math.Min(num, str.Length + 1);
            num2 = Math.Min(num2, (str.Length - num) + 1);
            builder.Remove(num - 1, num2);
            builder.Insert(num - 1, str2);
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
                return 4;
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
                return 4;
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
                return "REPLACE";
            }
        }
    }
}

