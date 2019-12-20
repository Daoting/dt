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
    /// Returns the <see cref="T:System.Double" /> sum-of-years' digits depreciation of an asset for a specified period.
    /// </summary>
    public class CalcSydFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> sum-of-years' digits depreciation of an asset for a specified period.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: cost, salvage, life, per.
        /// </para>
        /// <para>
        /// Cost is the initial cost of the asset.
        /// </para>
        /// <para>
        /// Salvage is the value at the end of the depreciation (sometimes called the salvage value of the asset).
        /// </para>
        /// <para>
        /// Life is the number of periods over which the asset is depreciated (sometimes called the useful life of the asset).
        /// </para>
        /// <para>
        /// Per is the period and must use the same units as life.
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
            if (((num2 >= 0.0) && (num3 >= 1)) && ((num4 > 0) && (num4 <= num3)))
            {
                return (double) ((((num - num2) * ((num3 - num4) + 1)) * 2.0) / ((double) (num3 * (num3 + 1))));
            }
            return CalcErrors.Number;
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
                return 4;
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
                return "SYD";
            }
        }
    }
}

