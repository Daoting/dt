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
    /// Returns the ratio of the factorial of a sum of values to the product 
    /// of factorials.
    /// </summary>
    public class CalcMultinomialFunction : CalcBuiltinFunction
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
        /// Returns the ratio of the factorial of a sum of values to the product
        /// of factorials.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// Number1,number2, ... are 1 to 255 values for which you want the multinomial.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int x = 0;
            double num2 = 1.0;
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
                        int num5 = CalcConvert.ToInt(obj2);
                        if ((num5 < 0) || (170 < num5))
                        {
                            return CalcErrors.Number;
                        }
                        x += num5;
                        num2 *= Fact(num5);
                    }
                }
                else
                {
                    int num6 = CalcConvert.ToInt(args[i]);
                    if ((num6 < 0) || (170 < num6))
                    {
                        return CalcErrors.Number;
                    }
                    x += num6;
                    num2 *= Fact(num6);
                }
            }
            if ((x >= 0) && (170 >= x))
            {
                return (double) (Fact(x) / num2);
            }
            return CalcErrors.Number;
        }

        private static double Fact(int x)
        {
            double num = 1.0;
            for (int i = x; i > 1; i--)
            {
                num *= i;
            }
            return num;
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
                return "MULTINOMIAL";
            }
        }
    }
}

