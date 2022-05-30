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
    /// Returns the <see cref="T:System.Double" /> Bessel function, which is also called the Weber function or the Neumann function.
    /// </summary>
    public class CalcBesselYFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> Bessel function, which is also called the Weber function or the Neumann function.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: x, n.
        /// </para>
        /// <para>
        /// X is the value at which to evaluate the function.
        /// </para>
        /// <para>
        /// N is the order of the function. If n is not an integer, it is truncated.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            int n = CalcConvert.ToInt(args[1]);
            if (num <= 0.0)
            {
                return CalcErrors.Number;
            }
            if (n < 0)
            {
                return CalcErrors.Number;
            }
            return CalcConvert.ToResult(EngineeringHelper.yn(n, num));
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
                return "BESSELY";
            }
        }
    }
}

