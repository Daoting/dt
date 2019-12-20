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
    /// Returns the <see cref="T:System.String" /> complex number given the real and imaginary coefficient.
    /// </summary>
    public class CalcComplexFunction : CalcBuiltinFunction
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
            return (i == 2);
        }

        /// <summary>
        /// Returns the <see cref="T:System.String" /> complex number given the real and imaginary coefficient.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: real_num, i_num, [suffix].
        /// </para>
        /// <para>
        /// Real_num is the real coefficient of the complex number.
        /// </para>
        /// <para>
        /// I_num is the imaginary coefficient of the complex number.
        /// </para>
        /// <para>
        /// Suffix is the suffix for the imaginary component of the complex number.
        /// If omitted, suffix is assumed to be "i".
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true))
            {
                return CalcErrors.Value;
            }
            string suffix = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToString(args[2]) : "i";
            if ((suffix != "i") && (suffix != "j"))
            {
                return CalcErrors.Value;
            }
            return ComplexConvert.ToResult(new Complex(num, num2), suffix);
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
                return "COMPLEX";
            }
        }
    }
}

