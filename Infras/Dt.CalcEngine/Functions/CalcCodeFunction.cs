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
    /// Returns a numeric code for the first character in a text string. 
    /// The returned code corresponds to the character set used by your computer.
    /// </summary>
    public class CalcCodeFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns a numeric code for the first character in a text string.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: text.
        /// </para>
        /// <para>
        /// Text is the text for which you want the code of the first character.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string str = CalcConvert.ToString(args[0]);
            if (string.IsNullOrEmpty(str))
            {
                return CalcErrors.Value;
            }
            try
            {
                return (int) CalcConvert.ToInt((char) str[0]);
            }
            catch
            {
                return CalcErrors.Value;
            }
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
                return "CODE";
            }
        }
    }
}

