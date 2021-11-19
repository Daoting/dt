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
    /// Returns the <see cref="T:System.Double" /> depreciation of an asset for a specified period using the double-declining balance method or some other method you specify.
    /// </summary>
    public class CalcDdbFunction : CalcBuiltinFunction
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
            return (i == 4);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> depreciation of an asset for a specified period using the double-declining balance method or some other method you specify.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 - 5 items: cost, salvage, life, period, [factor].
        /// </para>
        /// <para>
        /// Cost is the initial cost of the asset.
        /// </para>
        /// <para>
        /// Salvage is the value at the end of the depreciation (sometimes called the salvage value of the asset). This value can be 0.
        /// </para>
        /// <para>
        /// Life is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset).
        /// </para>
        /// <para>
        /// Period is the period for which you want to calculate the depreciation. Period must use the same units as life.
        /// </para>
        /// <para>
        /// Factor is the rate at which the balance declines. If factor is omitted, it is assumed to be 2 (the double-declining balance method).
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            double num2 = CalcConvert.ToDouble(args[1]);
            int num3 = CalcConvert.ToInt(args[2]);
            int num4 = CalcConvert.ToInt(args[3]);
            double num5 = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToDouble(args[4]) : 2.0;
            double num6 = 0.0;
            double num7 = 0.0;
            if ((num3 <= 0) || (num < 0.0))
            {
                return CalcErrors.Number;
            }
            if (num3 < num4)
            {
                return CalcErrors.Number;
            }
            if (num5 <= 0.0)
            {
                return CalcErrors.Number;
            }
            if (num4 <= 0)
            {
                return CalcErrors.Number;
            }
            if (num <= num2)
            {
                return (double) 0.0;
            }
            for (int i = 1; i <= num4; i++)
            {
                num7 = (num - num6) * (num5 / ((double) num3));
                num7 = Math.Min(num7, (num - num6) - num2);
                num6 += num7;
            }
            return (double) num7;
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
                return 4;
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
                return "DDB";
            }
        }
    }
}

