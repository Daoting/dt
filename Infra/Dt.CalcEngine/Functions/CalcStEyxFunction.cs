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
    /// Returns the <see cref="T:System.Double" /> standard error of the predicted y-value for each x in the regression.
    /// </summary>
    public class CalcStEyxFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> standard error of the predicted y-value for each x in the regression.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: known_y's, known_x's.
        /// </para>
        /// <para>
        /// Known_y's is an array or range of dependent data points.
        /// </para>
        /// <para>
        /// Known_x's is an array or range of independent data points.
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
            double num7 = 0.0;
            int num8 = 0;
            int length = ArrayHelper.GetLength(args[0], 0);
            if (length != ArrayHelper.GetLength(args[1], 0))
            {
                return CalcErrors.NotAvailable;
            }
            for (int i = 0; i < length; i++)
            {
                object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                object obj3 = ArrayHelper.GetValue(args[1], i, 0);
                if (CalcConvert.IsNumber(obj2) && CalcConvert.IsNumber(obj3))
                {
                    double num2 = CalcConvert.ToDouble(obj2);
                    double num = CalcConvert.ToDouble(obj3);
                    num3 += num2;
                    num4 += num2 * num2;
                    num5 += num;
                    num6 += num * num;
                    num7 += num * num2;
                    num8++;
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
            if ((num8 * (num8 - 2)) == 0)
            {
                return CalcErrors.DivideByZero;
            }
            if (((num8 * num6) - (num5 * num5)) == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            return (double) Math.Sqrt((((num8 * num4) - (num3 * num3)) - ((((num8 * num7) - (num5 * num3)) * ((num8 * num7) - (num5 * num3))) / ((num8 * num6) - (num5 * num5)))) / ((double) (num8 * (num8 - 2))));
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
                return "STEYX";
            }
        }
    }
}

