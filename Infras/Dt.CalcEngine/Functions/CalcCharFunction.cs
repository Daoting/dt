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
    /// Returns the character specified by a number. 
    /// Use CHAR to translate code page numbers you might get from 
    /// files on other types of computers into characters.
    /// </summary>
    public class CalcCharFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the character specified by a number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: number.
        /// </para>
        /// <para>
        /// Number is a number between 1 and 255 specifying which character you want.
        /// The character is from the character set used by your computer.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int num = CalcConvert.ToInt(args[0]);
            if ((num >= 1) && (0xff >= num))
            {
                return new string((char) num, 1);
            }
            return CalcErrors.Value;
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
                return "CHAR";
            }
        }
    }
}

