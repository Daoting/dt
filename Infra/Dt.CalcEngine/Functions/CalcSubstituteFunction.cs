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
    /// Substitutes new_text for old_text in a text string. 
    /// </summary>
    /// <remarks> 
    /// Use SUBSTITUTE when you want to replace specific text in a text string. 
    /// use REPLACE when you want to replace any text that occurs in a 
    /// specific location in a text string.
    /// </remarks>
    public class CalcSubstituteFunction : CalcBuiltinFunction
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
            return (i == 3);
        }

        /// <summary>
        /// Replace specific text in a new text string
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 4 items: text, old_text, new_text, [instance_num].
        /// </para>
        /// <para>
        /// Text is the text or the reference to a cell containing text
        /// for which you want to substitute characters.
        /// </para>
        /// <para>
        /// Old_text is the text you want to replace.
        /// </para>
        /// <para>
        /// New_text is the text you want to replace old_text with.
        /// </para>
        /// <para>
        /// [Instance_num]  specifies which occurrence of old_text you
        /// want to replace with new_text. If you specify instance_num,
        /// only that instance of old_text is replaced. Otherwise,
        /// every occurrence of old_text in text is changed to new_text.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            string str2 = CalcConvert.ToString(args[1]);
            string str3 = CalcConvert.ToString(args[2]);
            if (string.IsNullOrEmpty(str2))
            {
                return str;
            }
            if (args.Length > 3)
            {
                int num = CalcConvert.ToInt(args[3]);
                int startIndex = 0;
                if (num < 1)
                {
                    return CalcErrors.Value;
                }
                for (int i = 0; i < num; i++)
                {
                    startIndex = str.IndexOf(str2, startIndex);
                    if (startIndex == -1)
                    {
                        return str;
                    }
                    startIndex += str2.Length;
                }
                startIndex -= str2.Length;
                return str.Remove(startIndex, str2.Length).Insert(startIndex, str3);
            }
            return str.Replace(str2, str3);
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
                return "SUBSTITUTE";
            }
        }
    }
}

