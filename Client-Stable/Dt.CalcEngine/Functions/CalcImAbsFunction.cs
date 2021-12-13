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
    /// Returns the <see cref="T:System.Double" /> absolute value (modulus) of a complex number in x + yi or x + yj text format.
    /// </summary>
    public class CalcImAbsFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> absolute value (modulus) of a complex number in x + yi or x + yj text format.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: inumber.
        /// </para>
        /// <para>
        /// Inumber is a complex number for which you want the absolute value.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            Complex complex = ComplexConvert.ToComplex(args[0]);
            double real = complex.Real;
            double imag = complex.Imag;
            return CalcConvert.ToResult(Math.Sqrt((real * real) + (imag * imag)));
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
                return "IMABS";
            }
        }
    }
}

