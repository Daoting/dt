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
    /// Returns the <see cref="T:System.Double" /> effective annual interest rate, given the nominal annual interest rate and the number of compounding periods per year.
    /// </summary>
    public class CalcEffectFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> effective annual interest rate, given the nominal annual interest rate and the number of compounding periods per year.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: nominal_rate, npery.
        /// </para>
        /// <para>
        /// Nominal_rate is the nominal interest rate.
        /// </para>
        /// <para>
        /// Npery is the number of compounding periods per year.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            int num2 = CalcConvert.ToInt(args[1]);
            if ((num > 0.0) && (num2 >= 1))
            {
                return (double) (Math.Pow(1.0 + (num / ((double) num2)), (double) num2) - 1.0);
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
                return "EFFECT";
            }
        }
    }
}

