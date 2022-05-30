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
    /// Returns the logarithm of a number to the base you specify.
    /// </summary>
    public class CalcLogFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 1);
        }

        /// <summary>
        /// Returns the logarithm of a number to the base you specify.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: number, [base].
        /// </para>
        /// <para>
        /// Number is the positive real number for which you want the logarithm.
        /// </para>
        /// <para>
        /// [Base] is the base of the logarithm. If base is omitted, it is assumed to be 10.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            if (CalcHelper.ArgumentExists(args, 1))
            {
                if (!CalcConvert.TryToDouble(args[1], out num2, true))
                {
                    return CalcErrors.Value;
                }
            }
            else
            {
                num2 = 10.0;
            }
            if ((num <= 0.0) || (num2 <= 0.0))
            {
                return CalcErrors.Number;
            }
            if (num2 == 1.0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(Math.Log(num, num2));
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
                return "LOG";
            }
        }
    }
}

