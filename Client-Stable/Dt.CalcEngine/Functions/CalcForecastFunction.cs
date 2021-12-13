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
    /// Returns the <see cref="T:System.Double" /> future value along a linear trend by using existing values.
    /// </summary>
    public class CalcForecastFunction : CalcBuiltinFunction
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
            return (i > 0);
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
            return (i > 0);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> future value along a linear trend by using existing values.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: x, known_y's, known_x's.
        /// </para>
        /// <para>
        /// X is the data point for which you want to predict a value.
        /// </para>
        /// <para>
        /// Known_y's is the dependent array or range of data.
        /// </para>
        /// <para>
        /// Known_x's is the independent array or range of data.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            int length = ArrayHelper.GetLength(args[1], 0);
            if (length != ArrayHelper.GetLength(args[2], 0))
            {
                return CalcErrors.NotAvailable;
            }
            int num11 = 0;
            for (int i = 0; i < length; i++)
            {
                object obj2 = ArrayHelper.GetValue(args[1], i, 0);
                object obj3 = ArrayHelper.GetValue(args[2], i, 0);
                if (CalcConvert.IsNumber(obj2) && CalcConvert.IsNumber(obj3))
                {
                    double num2 = CalcConvert.ToDouble(obj2);
                    double num3 = CalcConvert.ToDouble(obj3);
                    num4 += num2;
                    num5 += num3;
                    num6 += num3 * num3;
                    num7 += num3 * num2;
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
            if (num11 == 0)
            {
                return CalcErrors.DivideByZero;
            }
            if (((num11 * num6) - (num5 * num5)) == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            double num9 = ((num11 * num7) - (num5 * num4)) / ((num11 * num6) - (num5 * num5));
            double num8 = (num4 / ((double) num11)) - (num9 * (num5 / ((double) num11)));
            return CalcConvert.ToResult(num8 + (num9 * num));
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
                return 3;
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
                return 3;
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
                return "FORECAST";
            }
        }
    }
}

