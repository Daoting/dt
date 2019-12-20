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
    /// Returns the <see cref="T:System.Double" /> normal cumulative distribution.
    /// </summary>
    public class CalcNormDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> normal cumulative distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: x, mean, standard_dev, cumulative.
        /// </para>
        /// <para>
        /// X is the value for which you want the distribution.
        /// </para>
        /// <para>
        /// Mean is the arithmetic mean of the distribution.
        /// </para>
        /// <para>
        /// Standard_dev is the standard deviation of the distribution.
        /// </para>
        /// <para>
        /// Cumulative is a logical value that determines the form of the function. If cumulative is TRUE, NORMDIST returns the cumulative distribution function; if FALSE, it returns the probability mass function.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            bool flag;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true)) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToBool(args[3], out flag))
            {
                return CalcErrors.Value;
            }
            if (num3 <= 0.0)
            {
                return CalcErrors.Number;
            }
            if (flag)
            {
                CalcBuiltinFunction function = new CalcNormSDistFunction();
                object[] objArray = new object[] { (double) ((num - num2) / num3) };
                return function.Evaluate(objArray);
            }
            return CalcConvert.ToResult(Math.Exp(-((num - num2) * (num - num2)) / ((2.0 * num3) * num3)) / (Math.Sqrt(6.2831853071795862) * num3));
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
                return "NORMDIST";
            }
        }
    }
}

