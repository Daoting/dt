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
    /// Returns the <see cref="T:System.Double" /> dollar decimal expressed as a fraction.
    /// </summary>
    public class CalcDollarFrFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> dollar decimal expressed as a fraction.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: decimal_dollar, fraction.
        /// </para>
        /// <para>
        /// Decimal_dollar is a decimal number.
        /// </para>
        /// <para>
        /// Fraction is the integer to use in the denominator of a fraction.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double a = CalcConvert.ToDouble(args[0]);
            double d = CalcConvert.ToInt(args[1]);
            if (d == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            if (d < 0.0)
            {
                return CalcErrors.Number;
            }
            double num3 = (a < 0.0) ? Math.Ceiling(a) : Math.Floor(a);
            double num4 = a - num3;
            double num5 = Math.Pow(10.0, Math.Ceiling(Math.Log10(d)));
            return CalcConvert.ToResult(num3 + ((num4 * d) / num5));
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
                return "DOLLARFR";
            }
        }
    }
}

