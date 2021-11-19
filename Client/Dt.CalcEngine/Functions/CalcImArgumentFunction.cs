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
    /// Returns the <see cref="T:System.Double" /> argument (theta), an angle expressed in radians.
    /// </summary>
    public class CalcImArgumentFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> argument (theta), an angle expressed in radians.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: inumber.
        /// </para>
        /// <para>
        /// Inumber is a complex number for which you want the argument .
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
            if ((real == 0.0) && (imag == 0.0))
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(Math.Atan2(imag, real));
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
                return "IMARGUMENT";
            }
        }
    }
}

