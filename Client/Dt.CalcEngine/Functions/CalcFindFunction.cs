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
    /// Locate one text string within a second text string, 
    /// and return the number of the starting position of the first text
    /// string from the first character of the second text string.
    /// </summary>
    public class CalcFindFunction : CalcBuiltinFunction
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
        /// Locate one text string within a second text string,
        /// and return the number of the starting position of the first text
        /// string from the first character of the second text string.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: find_text, within_text, [start_num].
        /// </para>
        /// <para>
        /// Find_text is the text you want to find.
        /// </para>
        /// <para>
        /// Within_text is the text containing the text you want to find.
        /// </para>
        /// <para>
        /// [Start_num] specifies the character at which to start the search.
        /// The first character in within_text is character number 1.
        /// The default value is 1.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            string str2 = CalcConvert.ToString(args[1]);
            int num = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToInt(args[2]) : 1;
            if ((num < 1) || (str2.Length < num))
            {
                return CalcErrors.Value;
            }
            int index = str2.IndexOf(str, (int) (num - 1));
            if (index == -1)
            {
                return CalcErrors.Value;
            }
            return (int) (index + 1);
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
                return "FIND";
            }
        }
    }
}

