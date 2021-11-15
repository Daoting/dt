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
    /// Returns the <see cref="T:System.Double" /> intersection with the y-axis using a linear regression plotted through known values.
    /// </summary>
    public class CalcInterceptFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> intersection with the y-axis using a linear regression plotted through known values.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: known_y's, known_x's.
        /// </para>
        /// <para>
        /// Known_y's is the dependent set of observations or data.
        /// </para>
        /// <para>
        /// Known_x's is the independent set of observations or data.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            int length = ArrayHelper.GetLength(args[0], 0);
            if (length != ArrayHelper.GetLength(args[1], 0))
            {
                return CalcErrors.NotAvailable;
            }
            int num9 = 0;
            for (int i = 0; i < length; i++)
            {
                object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                object obj3 = ArrayHelper.GetValue(args[1], i, 0);
                if (CalcConvert.IsNumber(obj2) && CalcConvert.IsNumber(obj3))
                {
                    double num = CalcConvert.ToDouble(obj2);
                    double num2 = CalcConvert.ToDouble(obj3);
                    num3 += num;
                    num4 += num2;
                    num5 += num2 * num2;
                    num6 += num2 * num;
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
            if (num9 == 0)
            {
                return CalcErrors.DivideByZero;
            }
            if (((num9 * num5) - (num4 * num4)) == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            double num7 = ((num9 * num6) - (num4 * num3)) / ((num9 * num5) - (num4 * num4));
            return CalcConvert.ToResult((num3 / ((double) num9)) - (num7 * (num4 / ((double) num9))));
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
                return "INTERCEPT";
            }
        }
    }
}

