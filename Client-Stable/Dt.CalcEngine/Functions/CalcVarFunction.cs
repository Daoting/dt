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
    /// Returns the <see cref="T:System.Double" /> compound variance based on a sample.
    /// </summary>
    public class CalcVarFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
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
        /// Returns the <see cref="T:System.Double" /> compound variance based on a sample.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// Number1, number2, ... are 1 to 255 number arguments corresponding to a sample of a population.
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
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
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
                        if (includeSubtotals || !ArrayHelper.IsSubtotal(args[i], j, 0))
                        {
                            object obj2 = ArrayHelper.GetValue(args[i], j, 0);
                            if (CalcConvert.IsNumber(obj2))
                            {
                                double num6 = CalcConvert.ToDouble(obj2);
                                num += num6;
                                num2 += num6 * num6;
                                num3++;
                            }
                            else if (obj2 is CalcError)
                            {
                                return obj2;
                            }
                        }
                    }
                }
                else
                {
                    double num7;
                    if (!CalcConvert.TryToDouble(args[i], out num7, true))
                    {
                        return CalcErrors.Value;
                    }
                    num += num7;
                    num2 += num7 * num7;
                    num3++;
                }
            }
            if (num3 <= 1.0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(Math.Max((double) 0.0, (double) (((num3 * num2) - (num * num)) / (num3 * (num3 - 1.0)))));
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
                return "VAR";
            }
        }
    }
}

