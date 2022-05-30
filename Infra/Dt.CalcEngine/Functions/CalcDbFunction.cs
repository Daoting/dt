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
    /// Returns the <see cref="T:System.Double" /> depreciation of an asset for a specified period using the fixed-declining balance method.
    /// </summary>
    public class CalcDbFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> depreciation of an asset for a specified period using the fixed-declining balance method.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 - 5 items: cost, salvage, life, period, [month].
        /// </para>
        /// <para>
        /// Cost is the initial cost of the asset.
        /// </para>
        /// <para>
        /// Salvage is the value at the end of the depreciation (sometimes called the salvage value of the asset).
        /// </para>
        /// <para>
        /// Life is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset).
        /// </para>
        /// <para>
        /// Period is the period for which you want to calculate the depreciation. Period must use the same units as life.
        /// </para>
        /// <para>
        /// Month is the number of months in the first year. If month is omitted, it is assumed to be 12.
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
            int num5 = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToInt(args[4]) : 12;
            int num6 = num3 + ((num5 < 12) ? 1 : 0);
            if ((((num < 0.0) || (num3 < 1)) || ((num4 < 1) || (num6 < num4))) || ((num5 < 1) || (12 < num5)))
            {
                return CalcErrors.Number;
            }
            if (num == 0.0)
            {
                return (double) 0.0;
            }
            double num7 = Math.Round((double) (1.0 - Math.Pow(num2 / num, 1.0 / ((double) num3))), 3);
            double num8 = 0.0;
            double num9 = 0.0;
            for (int i = 1; i <= num4; i++)
            {
                if (i == 1)
                {
                    num9 = ((num * num7) * num5) / 12.0;
                }
                else if (i == (num3 + 1))
                {
                    num9 = (((num - num8) * num7) * (12.0 - num5)) / 12.0;
                }
                else
                {
                    num9 = (num - num8) * num7;
                }
                num8 += num9;
            }
            return (double) num9;
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
                return "DB";
            }
        }
    }
}

