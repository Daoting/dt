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
    /// Returns the <see cref="T:System.Double" /> modified internal rate of return for a series of periodic cash flows.
    /// </summary>
    public class CalcMIrrFunction : CalcBuiltinFunction
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
            return (i == 0);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process references.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process references; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsReference(int i)
        {
            return (i == 0);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> modified internal rate of return for a series of periodic cash flows.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: values, finance_rate, reinvest_rate.
        /// </para>
        /// <para>
        /// Values is an array or a reference to cells that contain numbers.
        /// These numbers represent a series of payments (negative values) and income (positive values) occurring at regular periods.
        /// </para>
        /// <para>
        /// Finance_rate is the interest rate you pay on the money used in the cash flows.
        /// </para>
        /// <para>
        /// Reinvest_rate is the interest rate you receive on the cash flows as you reinvest them.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[1]);
            double num2 = CalcConvert.ToDouble(args[2]);
            int num3 = 0;
            int num4 = 0;
            double y = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            object[] objArray = new object[ArrayHelper.GetLength(args[0], 0)];
            if (!ArrayHelper.IsArrayOrRange(args[0]))
            {
                return CalcErrors.DivideByZero;
            }
            for (int i = 0; i < ArrayHelper.GetLength(args[0], 0); i++)
            {
                object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                if (CalcConvert.IsNumber(obj2))
                {
                    double num10 = CalcConvert.ToDouble(ArrayHelper.GetValue(args[0], i, 0));
                    objArray[i] = (double) num10;
                    if (num10 >= 0.0)
                    {
                        num3++;
                    }
                    else
                    {
                        num4++;
                    }
                }
                else if (obj2 is CalcError)
                {
                    return obj2;
                }
            }
            y = num4 + num3;
            for (int j = 0; j < y; j++)
            {
                double num12 = (double) ((double) objArray[j]);
                if (num12 >= 0.0)
                {
                    num6 += num12 / Math.Pow(1.0 + num2, (double) j);
                }
                else
                {
                    num7 += num12 / Math.Pow(1.0 + num, (double) j);
                }
            }
            if (((num7 == 0.0) || (num6 == 0.0)) || (num2 <= -1.0))
            {
                return CalcErrors.DivideByZero;
            }
            double num13 = Math.Pow((-num6 * Math.Pow(1.0 + num2, y)) / (num7 * (1.0 + num2)), 1.0 / (y - 1.0)) - 1.0;
            return (double) num13;
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
                return "MIRR";
            }
        }
    }
}

