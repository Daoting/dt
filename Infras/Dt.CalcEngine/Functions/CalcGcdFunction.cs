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
    /// Returns the greatest common divisor of two or more integers. 
    /// The greatest common divisor is the largest integer that divides 
    /// both number1 and number2 without a remainder.
    /// </summary>
    public class CalcGcdFunction : CalcBuiltinFunction
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
        /// Returns the greatest common divisor of two or more integers.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// number1, [number2], [number3], ... are 1 to 255 values.
        /// If any value is not an integer, it is truncated.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int a = 0;
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
                        int b = CalcConvert.ToInt(obj2);
                        if (b < 0)
                        {
                            return CalcErrors.Number;
                        }
                        a = Gcd(a, b);
                    }
                }
                else
                {
                    int num5 = CalcConvert.ToInt(args[i]);
                    if (num5 < 0)
                    {
                        return CalcErrors.Number;
                    }
                    a = Gcd(a, num5);
                }
            }
            return (double) a;
        }

        internal static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                int num = a % b;
                a = b;
                b = num;
            }
            return a;
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
                return "GCD";
            }
        }
    }
}

