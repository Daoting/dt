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
using System.Collections;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Multiplies all the numbers given as arguments and returns the product.
    /// </summary>
    public class CalcProductFunction : CalcBuiltinFunction
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
        /// Multiplies all the numbers given as arguments and returns the product.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// number1, [number2], [number3], ... are
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return Evaluate(args, true);
        }

        internal static object Evaluate(object[] args, bool includeSubtotals)
        {
            double num = 1.0;
            int num2 = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    IEnumerator enumerator = includeSubtotals ? CalcConvert.ToEnumerator(args[i], false) : CalcConvert.ToEnumeratorExcludingSubtotals(args[i]);
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (CalcConvert.IsNumber(current))
                        {
                            num *= CalcConvert.ToDouble(current);
                            num2++;
                        }
                        else if (current is CalcError)
                        {
                            return current;
                        }
                    }
                }
                else
                {
                    double num4;
                    if (!CalcConvert.TryToDouble(args[i], out num4, true))
                    {
                        return CalcErrors.Value;
                    }
                    num *= num4;
                    num2++;
                }
            }
            return CalcConvert.ToResult((num2 > 0) ? num : 0.0);
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
                return "PRODUCT";
            }
        }
    }
}

