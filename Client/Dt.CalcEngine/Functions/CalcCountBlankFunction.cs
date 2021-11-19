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
    /// Counts empty cells in a specified range of cells.
    /// </summary>
    public class CalcCountBlankFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts array values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsArray(int i)
        {
            return true;
        }

        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts CalcReference values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return true;
        }

        /// <summary>
        /// Counts empty cells in a specified range of cells.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: range.
        /// </para>
        /// <para>
        /// Range is the range from which you want to count the blank cells.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num = 0;
            for (short i = 0; i < ArrayHelper.GetRangeCount(args[0]); i++)
            {
                for (int j = 0; j < ArrayHelper.GetLength(args[0], i); j++)
                {
                    if (ArrayHelper.GetValue(args[0], j, i) == null)
                    {
                        num++;
                    }
                }
            }
            return (double) num;
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value></value>
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
        /// <value></value>
        public override int MinArgs
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public override string Name
        {
            get
            {
                return "COUNTBLANK";
            }
        }
    }
}

