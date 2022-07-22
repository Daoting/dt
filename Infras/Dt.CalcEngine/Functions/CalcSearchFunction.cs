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
using System.Text.RegularExpressions;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// locate one text string within a second text string, 
    /// and return the number of the starting position of the first 
    /// text string from the first character of the second text string.
    /// </summary>
    public class CalcSearchFunction : CalcBuiltinFunction
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
        /// locate one text string within a second text string,
        /// and return the number of the starting position of the first
        /// text string from the first character of the second text string.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: find_text, within_text, [start_num].
        /// </para>
        /// <para>
        /// Find_text is the text you want to find.
        /// </para>
        /// <para>
        /// Within_text is the text in which you want to search for find_text.
        /// </para>
        /// <para>
        /// [Start_num] is the character number in within_text at which you want to
        /// start searching, the default value is 1.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string s = CalcConvert.ToString(args[0]);
            string str2 = CalcConvert.ToString(args[1]);
            int startIndex = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToInt(args[2]) : 1;
            startIndex--;
            if (startIndex < 0)
            {
                return CalcErrors.Value;
            }
            int index = -1;
            try
            {
                if ((s.IndexOf('*') == -1) && (s.IndexOf('?') == -1))
                {
                    index = str2.ToLower().IndexOf(s.ToLower(), startIndex);
                }
                else
                {
                    Match match = new Regex(LookupHelper.CreateStringcomparisonRegexPattern(s).ToLower()).Match(str2.ToLower(), startIndex);
                    if ((match != null) && match.Success)
                    {
                        index = match.Index;
                    }
                }
            }
            catch
            {
            }
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
                return "SEARCH";
            }
        }
    }
}

