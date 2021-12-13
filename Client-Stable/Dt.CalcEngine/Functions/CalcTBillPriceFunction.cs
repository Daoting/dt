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
    /// Returns the <see cref="T:System.Double" /> price per $100 face value for a Treasury bill.
    /// </summary>
    public class CalcTBillPriceFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> price per $100 face value for a Treasury bill.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: settlement, maturity, discount.
        /// </para>
        /// <para>
        /// Settlement is the Treasury bill's settlement date.
        /// The security settlement date is the date after the issue date when the Treasury bill is traded to the buyer.
        /// </para>
        /// <para>
        /// Maturity is the Treasury bill's maturity date.
        /// The maturity date is the date when the Treasury bill expires.
        /// </para>
        /// <para>
        /// Discount is the Treasury bill's discount rate.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            DateTime time;
            DateTime time2;
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[2]);
            if (((args[0] is double) || (args[0] is decimal)) || (args[0] is float))
            {
                time = CalcConvert.ToDateTime((int) CalcConvert.ToInt(args[0]));
            }
            else
            {
                time = CalcConvert.ToDateTime(args[0]);
            }
            if (((args[1] is double) || (args[1] is decimal)) || (args[1] is float))
            {
                time2 = CalcConvert.ToDateTime((int) CalcConvert.ToInt(args[1]));
            }
            else
            {
                time2 = CalcConvert.ToDateTime(args[1]);
            }
            if (DateTime.Compare(time, time2) >= 0)
            {
                return CalcErrors.Number;
            }
            if (num <= 0.0)
            {
                return CalcErrors.Number;
            }
            double num2 = time2.ToOADate() - time.ToOADate();
            if (num2 > 365.0)
            {
                return CalcErrors.Number;
            }
            return (double) (100.0 * (1.0 - ((num * num2) / 360.0)));
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
                return "TBILLPRICE";
            }
        }
    }
}

