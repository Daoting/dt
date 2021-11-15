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
    /// Returns the <see cref="T:System.Double" /> number of permutations from a given number.
    /// </summary>
    public class CalcPermutFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> number of permutations from a given number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: number, number_chosen.
        /// </para>
        /// <para>
        /// Number is an integer that describes the number of objects.
        /// </para>
        /// <para>
        /// Number_chosen is an integer that describes the number of objects in each permutation.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToInt(args[0]);
            double num2 = CalcConvert.ToInt(args[1]);
            double num3 = 1.0;
            if (((num < 0.0) || (num2 < 0.0)) || (num < num2))
            {
                return CalcErrors.Number;
            }
            for (double i = (num - num2) + 1.0; i <= num; i++)
            {
                num3 *= i;
            }
            return CalcConvert.ToResult(num3);
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
                return "PERMUT";
            }
        }
    }
}

