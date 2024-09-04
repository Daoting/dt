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
    /// Returns the <see cref="T:System.Double" /> inverse of the F probability distribution.
    /// </summary>
    public class CalcFInvFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> inverse of the F probability distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: probability, degrees_freedom1, degrees_freedom2.
        /// </para>
        /// <para>
        /// Probability is a probability associated with the F cumulative distribution.
        /// </para>
        /// <para>
        /// Degrees_freedom1 is the numerator degrees of freedom.
        /// </para>
        /// <para>
        /// Degrees_freedom2 is the denominator degrees of freedom.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true)) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if ((((num < 0.0) || (1.0 < num)) || ((num2 < 1.0) || (num2 >= Math.Pow(10.0, 10.0)))) || ((num3 < 1.0) || (num3 >= Math.Pow(10.0, 10.0))))
            {
                return CalcErrors.Number;
            }
            double num4 = 1.0 - num;
            object obj2 = new CalcBetaInvFunction().Evaluate(new object[] { (double) (1.0 - num4), (double) (num3 / 2.0), (double) (num2 / 2.0) });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            return (double) (((1.0 / ((double) obj2)) - 1.0) * (num3 / num2));
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
                return "FINV";
            }
        }
    }
}

