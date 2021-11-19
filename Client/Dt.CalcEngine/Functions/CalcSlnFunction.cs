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
    /// Returns the <see cref="T:System.Double" /> straight-line depreciation of an asset for one period.
    /// </summary>
    public class CalcSlnFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> straight-line depreciation of an asset for one period.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: cost, salvage, life.
        /// </para>
        /// <para>
        /// Cost is the initial cost of the asset.
        /// </para>
        /// <para>
        /// Salvage is the value at the end of the depreciation (sometimes called the salvage value of the asset).
        /// </para>
        /// <para>
        /// Life is the number of periods over which the asset is depreciated (sometimes called the useful life of the asset).
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
            if (num3 == 0)
            {
                return CalcErrors.DivideByZero;
            }
            return (double) ((num - num2) / ((double) num3));
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
                return "SLN";
            }
        }
    }
}

