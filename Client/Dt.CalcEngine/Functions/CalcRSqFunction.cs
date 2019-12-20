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
    /// Returns the square of the Pearson product moment correlation
    /// coefficient through data points in known_y's and known_x's.
    /// </summary>
    /// <remarks>
    /// For more information, see PEARSON. The r-squared value can be
    /// interpreted as the proportion of the variance in y attributable 
    /// to the variance in x.
    /// </remarks>
    public class CalcRSqFunction : CalcBuiltinFunction
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
            if (i != 0)
            {
                return (i == 1);
            }
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
            if (i != 0)
            {
                return (i == 1);
            }
            return true;
        }

        /// <summary>
        /// Returns the square of the Pearson product moment correlation
        /// coefficient through data points in known_y's and known_x's.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: known_y's, known_x's.
        /// </para>
        /// <para>
        /// Known_y's is an array or range of data points.
        /// </para>
        /// <para>
        /// Known_x's is an array or range of data points.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object o = args[0];
            object obj3 = args[1];
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            int num6 = 0;
            int length = ArrayHelper.GetLength(obj3, 0);
            if (length != ArrayHelper.GetLength(o, 0))
            {
                return CalcErrors.NotAvailable;
            }
            for (int i = 0; i < length; i++)
            {
                object obj4 = ArrayHelper.GetValue(obj3, i, 0);
                object obj5 = ArrayHelper.GetValue(o, i, 0);
                if (CalcConvert.IsNumber(obj4) && CalcConvert.IsNumber(obj5))
                {
                    double num9 = CalcConvert.ToDouble(obj4);
                    double num10 = CalcConvert.ToDouble(obj5);
                    num += num9;
                    num2 += num10;
                    num3 += num9 * num9;
                    num4 += num10 * num10;
                    num5 += num9 * num10;
                    num6++;
                }
            }
            double num11 = Math.Sqrt(((num6 * num3) - (num * num)) * ((num6 * num4) - (num2 * num2)));
            if (num11 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            double num12 = ((num6 * num5) - (num * num2)) / num11;
            return CalcConvert.ToResult(num12 * num12);
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
                return "RSQ";
            }
        }
    }
}

