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
    /// Removes all nonprintable characters from text. 
    /// Use CLEAN on text imported from other applications that 
    /// contains characters that may not print with your operating system. 
    /// For example, you can use CLEAN to remove some low-level computer code
    /// that is frequently at the beginning and end of data files and cannot be printed.
    /// </summary>
    public class CalcCleanFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Removes all nonprintable characters from text.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: text.
        /// </para>
        /// <para>
        /// Text is any text which you want to remove nonprintable characters.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsControl(str[i]))
                {
                    builder.Append(str[i]);
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
                return "CLEAN";
            }
        }
    }
}

