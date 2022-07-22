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
    /// Returns the average (arithmetic mean) of the arguments.
    /// </summary>
    public class CalcAverageFunction : CalcBuiltinFunction
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
        /// Returns the average (arithmetic mean) of the arguments.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// Number1, number2, ... are 1 to 255 numeric arguments for which you want the average.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return Evaluate(args, true);
        }

        internal static object Evaluate(object[] args, bool includeSubtotals)
        {
            double num = 0.0;
            double num2 = 0.0;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    for (short j = 0; j < ArrayHelper.GetRangeCount(args[i]); j++)
                    {
                        for (int k = 0; k < ArrayHelper.GetLength(args[i], j); k++)
                        {
                            if (includeSubtotals || !ArrayHelper.IsSubtotal(args[i], k, j))
                            {
                                object obj2 = ArrayHelper.GetValue(args[i], k, j);
                                if (CalcConvert.IsNumber(obj2))
                                {
                                    num += CalcConvert.ToDouble(obj2);
                                    num2++;
                                }
                                else if (obj2 is CalcError)
                                {
                                    return obj2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    double num6;
                    if (!CalcConvert.TryToDouble(args[i], out num6, true))
                    {
                        return CalcErrors.Value;
                    }
                    num += num6;
                    num2++;
                }
            }
            if (num2 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(num / num2);
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
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
                return "AVERAGE";
            }
        }
    }
}

