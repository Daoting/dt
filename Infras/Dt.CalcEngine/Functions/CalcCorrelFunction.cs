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
    /// Returns the <see cref="T:System.Double" /> correlation coefficient of the array1 and array2 cell ranges.
    /// </summary>
    public class CalcCorrelFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> correlation coefficient of the array1 and array2 cell ranges.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array1, array2.
        /// </para>
        /// <para>
        /// Array1 is a cell range of values.
        /// </para>
        /// <para>
        /// Array2 is a second cell range of values.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            int length = ArrayHelper.GetLength(args[0], 0);
            if (length != ArrayHelper.GetLength(args[1], 0))
            {
                return CalcErrors.NotAvailable;
            }
            int num11 = 0;
            for (int i = 0; i < length; i++)
            {
                object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                object obj3 = ArrayHelper.GetValue(args[1], i, 0);
                if (CalcConvert.IsNumber(obj2) && CalcConvert.IsNumber(obj3))
                {
                    double num13 = CalcConvert.ToDouble(obj2);
                    double num14 = CalcConvert.ToDouble(obj3);
                    num += num13;
                    num2 += num14;
                    num3 += num13 * num13;
                    num4 += num14 * num14;
                    num11++;
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
            if (num11 <= 1)
            {
                return CalcErrors.DivideByZero;
            }
            double num6 = num / ((double) num11);
            double num7 = num2 / ((double) num11);
            double num8 = Math.Sqrt(((num11 * num3) - (num * num)) / ((double) (num11 * (num11 - 1))));
            double num9 = Math.Sqrt(((num11 * num4) - (num2 * num2)) / ((double) (num11 * (num11 - 1))));
            if ((num8 == 0.0) || (num9 == 0.0))
            {
                return CalcErrors.DivideByZero;
            }
            for (int j = 0; j < length; j++)
            {
                object obj4 = ArrayHelper.GetValue(args[0], j, 0);
                object obj5 = ArrayHelper.GetValue(args[1], j, 0);
                if (CalcConvert.IsNumber(obj4) && CalcConvert.IsNumber(obj5))
                {
                    double num16 = CalcConvert.ToDouble(obj4);
                    double num17 = CalcConvert.ToDouble(obj5);
                    num5 += (num16 - num6) * (num17 - num7);
                }
            }
            return CalcConvert.ToResult(num5 / (((num11 - 1) * num8) * num9));
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
                return "CORREL";
            }
        }
    }
}

