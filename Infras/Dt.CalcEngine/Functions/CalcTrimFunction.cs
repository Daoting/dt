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
    /// Removes all spaces from text except for single spaces between words. 
    /// Use TRIM on text that you have received from another application
    /// that may have irregular spacing.
    /// </summary>
    public class CalcTrimFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Removes all spaces from text except for single spaces between words.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: text.
        /// </para>
        /// <para>
        /// Text is the text from which you want spaces removed.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]).Trim();
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsWhiteSpace(str[i]) || flag)
                {
                    builder.Append(str[i]);
                }
                if (char.IsWhiteSpace(str[i]))
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
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
                return "TRIM";
            }
        }
    }
}

