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
    /// Returns the <see cref="T:System.Double" /> number of periods for an investment based on periodic, constant payments and a constant interest rate.
    /// </summary>
    public class CalcNperFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            if (i != 3)
            {
                return (i == 4);
            }
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> number of periods for an investment based on periodic, constant payments and a constant interest rate.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 5 items: rate, pmt, pv, [fv], [type].
        /// </para>
        /// <para>
        /// Rate is the interest rate per period.
        /// </para>
        /// <para>
        /// Pmt is the payment made each period; it cannot change over the life of the annuity.
        /// Typically, pmt contains principal and interest but no other fees or taxes.
        /// </para>
        /// <para>
        /// Pv is the present value, or the lump-sum amount that a series of future payments is worth right now.
        /// </para>
        /// <para>
        /// Fv is the future value, or a cash balance you want to attain after the last payment is made.
        /// If fv is omitted, it is assumed to be 0 (the future value of a loan, for example, is 0).
        /// </para>
        /// <para>
        /// Type is the number 0 or 1 and indicates when payments are due.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            double num2 = CalcConvert.ToDouble(args[1]);
            double num3 = CalcConvert.ToDouble(args[2]);
            double num4 = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToDouble(args[3]) : 0.0;
            double num5 = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToDouble(args[4]) : 0.0;
            if (num5 != 0.0)
            {
                num5 = 1.0;
            }
            if (num == 0.0)
            {
                if (num2 == 0.0)
                {
                    return CalcErrors.DivideByZero;
                }
                return CalcConvert.ToResult(-(num3 + num4) / num2);
            }
            if (num <= -1.0)
            {
                return CalcErrors.Number;
            }
            return CalcConvert.ToResult(Math.Log(((num2 * (1.0 + (num * num5))) - (num4 * num)) / ((num3 * num) + (num2 * (1.0 + (num * num5))))) / Math.Log(1.0 + num));
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
                return 5;
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
                return "NPER";
            }
        }
    }
}

