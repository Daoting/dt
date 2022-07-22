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
    /// Returns the <see cref="T:System.Double" /> confidence interval for a population mean.
    /// </summary>
    public class CalcConfidenceFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> confidence interval for a population mean.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: alpha, standard_dev, size.
        /// </para>
        /// <para>
        /// Alpha is the significance level used to compute the confidence level. The confidence level equals 100*(1 - alpha)%, or in other words, an alpha of 0.05 indicates a 95 percent confidence level.
        /// </para>
        /// <para>
        /// Standard_dev is the population standard deviation for the data range and is assumed to be known.
        /// </para>
        /// <para>
        /// Size is the sample size.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true))
            {
                return CalcErrors.Value;
            }
            int num3 = CalcConvert.ToInt(args[2]);
            if ((num <= 0.0) || (num >= 1.0))
            {
                return CalcErrors.Number;
            }
            if (num2 <= 0.0)
            {
                return CalcErrors.Number;
            }
            if (num3 < 1)
            {
                return CalcErrors.Number;
            }
            object obj2 = new CalcNormSInvFunction().Evaluate(new object[] { (double) (num / 2.0) });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            return (double) (-((double) obj2) * (num2 / Math.Sqrt((double) num3)));
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
                return "CONFIDENCE";
            }
        }
    }
}

