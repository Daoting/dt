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
    /// Returns the <see cref="T:System.Double" /> Pearson product moment correlation coefficient.
    /// </summary>
    public class CalcPearsonFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> Pearson product moment correlation coefficient.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array1, array2.
        /// </para>
        /// <para>
        /// Array1 is a set of independent values.
        /// </para>
        /// <para>
        /// Array2 is a set of dependent values.
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
            int num7 = 0;
            for (int i = 0; i < length; i++)
            {
                object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                object obj3 = ArrayHelper.GetValue(args[1], i, 0);
                if (CalcConvert.IsNumber(obj2) && CalcConvert.IsNumber(obj3))
                {
                    double num9 = CalcConvert.ToDouble(obj2);
                    double num10 = CalcConvert.ToDouble(obj3);
                    num += num9;
                    num2 += num10;
                    num3 += num9 * num9;
                    num4 += num10 * num10;
                    num5 += num9 * num10;
                    num7++;
                }
            }
            if ((((num7 * num3) - (num * num)) != 0.0) && (((num7 * num4) - (num2 * num2)) != 0.0))
            {
                return (double) (((num7 * num5) - (num * num2)) / Math.Sqrt(((num7 * num3) - (num * num)) * ((num7 * num4) - (num2 * num2))));
            }
            return CalcErrors.DivideByZero;
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
                return "PEARSON";
            }
        }
    }
}

