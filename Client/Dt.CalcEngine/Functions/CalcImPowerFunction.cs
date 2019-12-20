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
    /// Returns a <see cref="T:System.String" /> complex number in x + yi or x + yj text format raised to a power.
    /// </summary>
    public class CalcImPowerFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns a <see cref="T:System.String" /> complex number in x + yi or x + yj text format raised to a power.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: inumber, number.
        /// </para>
        /// <para>
        /// Inumber is a complex number you want to raise to a power.
        /// </para>
        /// <para>
        /// Number is the power to which you want to raise the complex number.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            Complex complex = ComplexConvert.ToComplex(args[0]);
            if (!CalcConvert.TryToDouble(args[1], out num, true))
            {
                return CalcErrors.Value;
            }
            double real = complex.Real;
            double imag = complex.Imag;
            if ((real == 0.0) && (imag == 0.0))
            {
                if (num > 0.0)
                {
                    return ComplexConvert.ToResult(new Complex(0.0, 0.0));
                }
                return CalcErrors.Number;
            }
            double x = Math.Sqrt((real * real) + (imag * imag));
            double num5 = Math.Atan2(imag, real);
            return ComplexConvert.ToResult(new Complex(Math.Pow(x, num) * Math.Cos(num * num5), Math.Pow(x, num) * Math.Sin(num * num5)));
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
                return "IMPOWER";
            }
        }
    }
}

