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
    /// Returns the <see cref="T:System.String" /> sum of two or more complex numbers in x + yi or x + yj text format.
    /// </summary>
    public class CalcImSumFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i != 0);
        }

        /// <summary>
        /// Returns the <see cref="T:System.String" /> sum of two or more complex numbers in x + yi or x + yj text format.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: inumber1, [inumber2], [inumber3], ..
        /// </para>
        /// <para>
        /// Inumber1,inumber2,...   are 1 to 255 complex numbers to add.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double real = 0.0;
            double imag = 0.0;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcArray)
                {
                    CalcArray array = (CalcArray) args[i];
                    for (int j = 0; j < array.RowCount; j++)
                    {
                        for (int k = 0; k < array.ColumnCount; k++)
                        {
                            Complex complex = ComplexConvert.ToComplex(array.GetValue(j, k));
                            double num6 = real;
                            double num7 = imag;
                            double num8 = complex.Real;
                            double num9 = complex.Imag;
                            real = num6 + num8;
                            imag = num7 + num9;
                        }
                    }
                }
                else
                {
                    Complex complex2 = ComplexConvert.ToComplex(args[i]);
                    double num10 = real;
                    double num11 = imag;
                    double num12 = complex2.Real;
                    double num13 = complex2.Imag;
                    real = num10 + num12;
                    imag = num11 + num13;
                }
            }
            return ComplexConvert.ToResult(new Complex(real, imag));
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
                return 0xff;
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
                return "IMSUM";
            }
        }
    }
}

