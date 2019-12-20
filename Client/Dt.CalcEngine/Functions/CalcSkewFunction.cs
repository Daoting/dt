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
    /// Returns the skew of a distribution.
    /// </summary>
    /// <remarks>
    /// Skew characterizes the degree of asymmetry of a distribution a
    /// round its mean. Positive skew indicates a distribution with an 
    /// asymmetric tail extending toward more positive values. Negative skew
    /// indicates a distribution with an asymmetric tail extending toward 
    /// more negative values.
    /// </remarks>
    public class CalcSkewFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsArray(int i)
        {
            return true;
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process reference.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process reference; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsReference(int i)
        {
            return true;
        }

        /// <summary>
        /// Returns the skew of a distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// Number1, number2 ... are 1 to 255 arguments for which you want to calculate skew.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            if (args[0] == null)
            {
                throw new ArgumentException();
            }
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            int num6 = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    for (int k = 0; k < ArrayHelper.GetLength(args[i], 0); k++)
                    {
                        object obj2 = ArrayHelper.GetValue(args[i], k, 0);
                        if (CalcConvert.IsNumber(obj2))
                        {
                            double num9 = CalcConvert.ToDouble(obj2);
                            num += num9;
                            num2 += num9 * num9;
                            num6++;
                        }
                        else if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                    }
                }
                else
                {
                    double num10;
                    if (args[i] is CalcError)
                    {
                        return args[i];
                    }
                    if (!CalcConvert.TryToDouble(args[i], out num10, true))
                    {
                        return CalcErrors.Value;
                    }
                    num += num10;
                    num2 += num10 * num10;
                    num6++;
                }
            }
            if (num6 <= 2)
            {
                return CalcErrors.DivideByZero;
            }
            double num4 = num / ((double) num6);
            double num5 = Math.Sqrt(((num6 * num2) - (num * num)) / ((double) (num6 * (num6 - 1))));
            if (num5 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            for (int j = 0; j < args.Length; j++)
            {
                if (ArrayHelper.IsArrayOrRange(args[j]))
                {
                    for (int m = 0; m < ArrayHelper.GetLength(args[j], 0); m++)
                    {
                        object obj3 = ArrayHelper.GetValue(args[j], m, 0);
                        if (CalcConvert.IsNumber(obj3))
                        {
                            double num13 = CalcConvert.ToDouble(obj3);
                            num3 += Math.Pow((num13 - num4) / num5, 3.0);
                        }
                        else if (obj3 is CalcError)
                        {
                            return obj3;
                        }
                    }
                }
                else
                {
                    double num14;
                    if (!CalcConvert.TryToDouble(args[j], out num14, true))
                    {
                        return CalcErrors.Value;
                    }
                    num3 += Math.Pow((num14 - num4) / num5, 3.0);
                }
            }
            return CalcConvert.ToResult((num6 * num3) / ((double) ((num6 - 1) * (num6 - 2))));
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
                return "SKEW";
            }
        }
    }
}

