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
    /// Returns the geometric mean of an array or range of positive data.
    /// </summary>
    public class CalcGeoMeanFunction : CalcBuiltinFunction
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
        /// Returns the geometric mean of an array or range of positive data.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// Number1, number2, ... are 1 to 255 arguments for which you want
        /// to calculate the mean.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double x = 1.0;
            int num3 = 0;
            for (int i = 0; i < args.Length; i++)
            {
                double num;
                if (args[i] is CalcError)
                {
                    return args[i];
                }
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    for (int j = 0; j < ArrayHelper.GetLength(args[i], 0); j++)
                    {
                        object obj2 = ArrayHelper.GetValue(args[i], j, 0);
                        if (CalcConvert.IsNumber(obj2))
                        {
                            num = CalcConvert.ToDouble(obj2);
                            if (num <= 0.0)
                            {
                                return CalcErrors.Number;
                            }
                            x *= num;
                            num3++;
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
                    if (num <= 0.0)
                    {
                        return CalcErrors.Number;
                    }
                    x *= num;
                    num3++;
                }
            }
            if (num3 <= 0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(Math.Pow(x, 1.0 / ((double) num3)));
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
                return "GEOMEAN";
            }
        }
    }
}

