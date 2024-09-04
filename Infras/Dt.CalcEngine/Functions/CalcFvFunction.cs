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
    /// Returns the <see cref="T:System.Double" /> future value of an investment based on periodic, constant payments and a constant interest rate.
    /// </summary>
    public class CalcFvFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> future value of an investment based on periodic, constant payments and a constant interest rate.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 5 items: rate, nper, pmt, [pv], [type].
        /// </para>
        /// <para>
        /// Rate is the interest rate per period.
        /// </para>
        /// <para>
        /// Nper is the total number of payment periods in an annuity.
        /// </para>
        /// <para>
        /// Pmt is the payment made each period;
        /// it cannot change over the life of the annuity.
        /// Typically, pmt contains principal and interest but no other fees or taxes.
        /// If pmt is omitted, you must include the pv argument.
        /// </para>
        /// <para>
        /// Pv is the present value, or the lump-sum amount that a series of future payments is worth right now.
        /// If pv is omitted, it is assumed to be 0 (zero), and you must include the pmt argument.
        /// </para>
        /// <para>
        /// Type is the number 0 or 1 and indicates when payments are due.
        /// If type is omitted, it is assumed to be 0.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            double y = CalcConvert.ToDouble(args[1]);
            double num3 = CalcConvert.ToDouble(args[2]);
            double num4 = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToDouble(args[3]) : 0.0;
            double num5 = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToDouble(args[4]) : 0.0;
            if (num5 != 0.0)
            {
                num5 = 1.0;
            }
            if (num == 0.0)
            {
                return CalcConvert.ToResult(-((num3 * y) + num4));
            }
            return CalcConvert.ToResult(-((num4 * Math.Pow(1.0 + num, y)) + (((num3 * (1.0 + (num * num5))) * (Math.Pow(1.0 + num, y) - 1.0)) / num)));
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
                return "FV";
            }
        }
    }
}

