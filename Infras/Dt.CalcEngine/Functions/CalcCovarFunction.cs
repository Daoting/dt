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
    /// Returns the <see cref="T:System.Double" /> covariance of two lists of numbers.
    /// </summary>
    public class CalcCovarFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> covariance of two lists of numbers.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array1, array2.
        /// </para>
        /// <para>
        /// Array1 is the first cell range of integers.
        /// </para>
        /// <para>
        /// Array2 is the second cell range of integers.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            int length = ArrayHelper.GetLength(args[0], 0);
            if (length == 0)
            {
                return CalcErrors.DivideByZero;
            }
            if (length != ArrayHelper.GetLength(args[1], 0))
            {
                return CalcErrors.NotAvailable;
            }
            int num9 = 0;
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                for (int i = 0; i < length; i++)
                {
                    object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                    object obj3 = ArrayHelper.GetValue(args[1], i, 0);
                    if (CalcConvert.IsNumber(obj2) && CalcConvert.IsNumber(obj3))
                    {
                        num = CalcConvert.ToDouble(obj2);
                        num2 = CalcConvert.ToDouble(obj3);
                        num5 += num;
                        num6 += num2;
                        num9++;
                    }
                    else
                    {
                        if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                        if (obj3 is CalcError)
                        {
                            return obj3;
                        }
                    }
                }
            }
            else
            {
                if (!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true))
                {
                    return CalcErrors.Value;
                }
                num5 += num;
                num6 += num2;
                num9++;
            }
            double num3 = num5 / ((double) num9);
            double num4 = num6 / ((double) num9);
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                for (int j = 0; j < length; j++)
                {
                    object obj4 = ArrayHelper.GetValue(args[0], j, 0);
                    object obj5 = ArrayHelper.GetValue(args[1], j, 0);
                    if (CalcConvert.IsNumber(obj4) && CalcConvert.IsNumber(obj5))
                    {
                        num = CalcConvert.ToDouble(obj4);
                        num2 = CalcConvert.ToDouble(obj5);
                        num7 += (num - num3) * (num2 - num4);
                    }
                    else
                    {
                        if (obj4 is CalcError)
                        {
                            return obj4;
                        }
                        if (obj5 is CalcError)
                        {
                            return obj5;
                        }
                    }
                }
            }
            else
            {
                if (!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true))
                {
                    return CalcErrors.Value;
                }
                num7 += (num - num3) * (num2 - num4);
            }
            return CalcConvert.ToResult(num7 / ((double) num9));
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
                return "COVAR";
            }
        }
    }
}

