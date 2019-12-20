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
    /// Compares two text strings and returns <see langword="true" /> if 
    /// they are exactly the same, <see langword="false" /> otherwise. 
    /// </summary>
    /// <remarks>
    /// EXACT is case-sensitive but ignores formatting differences. 
    /// Use EXACT to test text being entered into a document.
    /// </remarks>
    public class CalcExactFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Compares two text strings and returns <see langword="true" /> if
        /// they are exactly the same, <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: text1, text2.
        /// </para>
        /// <para>
        /// Text1 is the first text string.
        /// </para>
        /// <para>
        /// Text2 is the second text string.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Boolean" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            string strA = CalcConvert.ToString(args[0]);
            string strB = CalcConvert.ToString(args[1]);
            return (bool) (string.Compare(strA, strB, StringComparison.CurrentCulture) == 0);
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
                return "EXACT";
            }
        }
    }
}

