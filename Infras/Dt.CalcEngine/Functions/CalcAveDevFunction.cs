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
    /// Returns the average of the absolute deviations of data points from their mean.
    /// </summary>
    /// <remarks>
    /// AVEDEV is a measure of the variability in a data set.
    /// </remarks>
    public class CalcAveDevFunction : CalcBuiltinFunction
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
        /// Returns the average of the absolute deviations of data points from their mean.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// Number1, number2, ... are 1 to 255 arguments for which you want
        /// the average of the absolute deviations.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (CalcConvert.IsError(args[i]))
                {
                    return args[i];
                }
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    for (int k = 0; k < ArrayHelper.GetLength(args[i], 0); k++)
                    {
                        object obj2 = ArrayHelper.GetValue(args[i], k, 0);
                        if (CalcConvert.IsNumber(obj2))
                        {
                            num = CalcConvert.ToDouble(obj2);
                            num3 += num;
                            num5++;
                        }
                        else if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                    }
                }
                else
                {
                    if (!CalcConvert.TryToDouble(args[i], out num, true))
                    {
                        return CalcErrors.Value;
                    }
                    num3 += num;
                    num5++;
                }
            }
            double num2 = num3 / ((double) num5);
            for (int j = 0; j < args.Length; j++)
            {
                if (CalcConvert.IsError(args[j]))
                {
                    return args[j];
                }
                if (ArrayHelper.IsArrayOrRange(args[j]))
                {
                    for (int m = 0; m < ArrayHelper.GetLength(args[j], 0); m++)
                    {
                        object obj3 = ArrayHelper.GetValue(args[j], m, 0);
                        if (CalcConvert.IsNumber(obj3))
                        {
                            num = CalcConvert.ToDouble(obj3);
                            num4 += Math.Abs((double) (num - num2));
                        }
                        else if (obj3 is CalcError)
                        {
                            return obj3;
                        }
                    }
                }
                else
                {
                    if (!CalcConvert.TryToDouble(args[j], out num, true))
                    {
                        return CalcErrors.Value;
                    }
                    num4 += Math.Abs((double) (num - num2));
                }
            }
            if (num5 == 0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(num4 / ((double) num5));
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
                return "AVEDEV";
            }
        }
    }
}

