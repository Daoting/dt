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
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the least common multiple of integers.
    /// The least common multiple is the smallest positive integer 
    /// that is a multiple of all integer arguments number1, number2,
    /// and so on. Use LCM to add fractions with different denominators.
    /// </summary>
    public class CalcLcmFunction : CalcBuiltinFunction
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
        /// Returns the least common multiple of integers.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// number1, [number2], [number3], ... are to 255 values for which you
        /// want the least common multiple. If value is not an integer, it is truncated.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int a = 1;
            List<int> list = new List<int>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    for (int j = 0; j < ArrayHelper.GetLength(args[i], 0); j++)
                    {
                        object obj2 = ArrayHelper.GetValue(args[i], j, 0);
                        if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                        int item = CalcConvert.ToInt(obj2);
                        list.Add(item);
                    }
                }
                else
                {
                    int num5 = CalcConvert.ToInt(args[i]);
                    list.Add(num5);
                }
                foreach (int num6 in list)
                {
                    if (num6 < 0)
                    {
                        return CalcErrors.Number;
                    }
                    if (num6 == 0)
                    {
                        return (double) 0.0;
                    }
                    a /= CalcGcdFunction.Gcd(a, num6);
                    a *= num6;
                }
            }
            return (double) a;
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
                return "LCM";
            }
        }
    }
}

