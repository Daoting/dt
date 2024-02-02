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
    /// Joins two or more text strings into one text string.
    /// </summary>
    public class CalcConcatenateFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// True if the function accepts array values for the specified argument; false otherwise.
        /// </returns>
        public override bool AcceptsArray(int i)
        {
            return true;
        }

        /// <summary>
        /// Joins two or more text strings into one text string.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 255 items: text1, text2, [text3], [text4], ..
        /// </para>
        /// <para>
        /// Text1, text2, ... are 2 to 255 text items to be joined into a single text item. The text items can be text strings, numbers, or single-cell references.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                for (int j = 0; j < ArrayHelper.GetLength(args[i], 0); j++)
                {
                    object obj2 = ArrayHelper.GetValue(args[i], j, 0);
                    if (obj2 != null)
                    {
                        builder.Append(CalcConvert.ToString(obj2));
                    }
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
                return 0xff;
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
                return "CONCATENATE";
            }
        }
    }
}

