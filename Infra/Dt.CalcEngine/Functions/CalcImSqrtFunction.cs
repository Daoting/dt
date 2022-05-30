#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the <see cref="T:System.String" /> square root of a complex number in x + yi or x + yj text format.
    /// </summary>
    public class CalcImSqrtFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.String" /> square root of a complex number in x + yi or x + yj text format.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: inumber.
        /// </para>
        /// <para>
        /// Inumber is a complex number for which you want the square root.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            Complex complex = ComplexConvert.ToComplex(args[0]);
            double real = complex.Real;
            double imag = complex.Imag;
            if ((real == 0.0) && (imag == 0.0))
            {
                return ComplexConvert.ToResult(new Complex(0.0, 0.0));
            }
            double d = Math.Sqrt((real * real) + (imag * imag));
            double num4 = Math.Atan2(imag, real);
            return ComplexConvert.ToResult(new Complex(Math.Sqrt(d) * Math.Cos(num4 / 2.0), Math.Sqrt(d) * Math.Sin(num4 / 2.0)));
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
                return "IMSQRT";
            }
        }
    }
}

