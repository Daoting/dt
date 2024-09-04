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
    /// Returns the <see cref="T:System.Double" /> inverse of the Fisher transformation.
    /// </summary>
    public class CalcFisherInvFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> inverse of the Fisher transformation.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: y.
        /// </para>
        /// <para>
        /// Y is the value for which you want to perform the inverse of the transformation.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            double d = Math.Exp(2.0 * num) - 1.0;
            double num3 = Math.Exp(2.0 * num) + 1.0;
            if (double.IsPositiveInfinity(d) && double.IsPositiveInfinity(num3))
            {
                return (double) 1.0;
            }
            return (double) (d / num3);
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
                return 1;
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
                return 1;
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
                return "FISHERINV";
            }
        }
    }
}

