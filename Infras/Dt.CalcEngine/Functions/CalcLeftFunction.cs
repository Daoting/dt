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
    /// Returns the first character or characters in a text string, 
    /// based on the number of characters you specify.
    /// </summary>
    public class CalcLeftFunction : CalcBuiltinFunction
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
            return (i == 1);
        }

        /// <summary>
        /// Returns the first character or characters in a text string,
        /// based on the number of characters you specify.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: text, [num_chars].
        /// </para>
        /// <para>
        /// Text is the text string that contains the characters you want to extract.
        /// </para>
        /// <para>
        /// [Num_bytes] specifies the number of characters you want LEFTB
        /// to extract, based on bytes, the default value is 1.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            int length = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToInt(args[1]) : 1;
            if (length < 0)
            {
                return CalcErrors.Value;
            }
            if (length >= str.Length)
            {
                return str;
            }
            return str.Substring(0, length);
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
                return "LEFT";
            }
        }
    }
}

