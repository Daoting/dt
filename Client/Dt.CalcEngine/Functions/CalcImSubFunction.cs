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
    /// Returns the <see cref="T:System.String" /> difference of two complex numbers in x + yi or x + yj text format.
    /// </summary>
    public class CalcImSubFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.String" /> difference of two complex numbers in x + yi or x + yj text format.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: inumber1, inumber2.
        /// </para>
        /// <para>
        /// Inumber1 is the complex number from which to subtract inumber2.
        /// </para>
        /// <para>
        /// Inumber2 is the complex number to subtract from inumber1.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            Complex complex = ComplexConvert.ToComplex(args[0]);
            Complex complex2 = ComplexConvert.ToComplex(args[1]);
            double real = complex.Real;
            double imag = complex.Imag;
            double num3 = complex2.Real;
            double num4 = complex2.Imag;
            return ComplexConvert.ToResult(new Complex(real - num3, imag - num4));
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
                return "IMSUB";
            }
        }
    }
}

