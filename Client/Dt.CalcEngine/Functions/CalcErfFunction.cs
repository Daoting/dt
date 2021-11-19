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
    /// Returns the <see cref="T:System.Double" /> error function integrated between lower_limit and upper_limit.
    /// </summary>
    public class CalcErfFunction : CalcBuiltinFunction
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
            return (i == 1);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> error function integrated between lower_limit and upper_limit.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: lower_limit, [upper_limit].
        /// </para>
        /// <para>
        /// Lower_limit is the lower bound for integrating ERF.
        /// </para>
        /// <para>
        /// Upper_limit is the upper bound for integrating ERF.
        /// If omitted, ERF integrates between zero and lower_limit.
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
            double number = 0.0;
            if (CalcHelper.ArgumentExists(args, 1) && !CalcConvert.TryToDouble(args[1], out number, true))
            {
                return CalcErrors.Value;
            }
            if ((num < 0.0) || (number < 0.0))
            {
                return CalcErrors.Number;
            }
            if ((num > 27.0) || (number > 27.0))
            {
                return CalcErrors.Number;
            }
            CalcBuiltinFunction function = new CalcNormSDistFunction();
            object obj2 = function.Evaluate(new object[] { (double) (num * Math.Sqrt(2.0)) });
            if (obj2 is CalcError)
            {
                return (double) double.NaN;
            }
            double num3 = (((double) obj2) * 2.0) - 1.0;
            if (CalcHelper.ArgumentExists(args, 1))
            {
                obj2 = function.Evaluate(new object[] { (double) (number * Math.Sqrt(2.0)) });
                if (obj2 is CalcError)
                {
                    return (double) double.NaN;
                }
                double num4 = (((double) obj2) * 2.0) - 1.0;
                num3 = num4 - num3;
            }
            return (double) num3;
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
                return "ERF";
            }
        }
    }
}

