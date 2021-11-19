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
    /// Converts a currency value from one source to another.
    /// </summary>
    public class CalcEuroConvertFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns></returns>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments;
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            if (i != 3)
            {
                return (i == 4);
            }
            return true;
        }

        /// <summary>
        /// Evaluates the function with the given arguments.
        /// </summary>
        /// <param name="args">Arguments for the function evaluation</param>
        /// <returns>
        /// Result of the function applied to the arguments
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num = CalcConvert.ToDouble(args[0]);
            string str = CalcConvert.ToString(args[1]);
            string str2 = CalcConvert.ToString(args[2]);
            bool flag = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToBool(args[3]) : false;
            int prec = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToInt(args[4]) : 3;
            int digits = 0;
            if (prec >= 3)
            {
                if (!flag)
                {
                    digits = FinancialHelper.displayPrecision(str2);
                }
                if (!CalcHelper.ArgumentExists(args, 4))
                {
                    prec = FinancialHelper.calcPrecision(str);
                }
                double num4 = 0.0;
                double num5 = FinancialHelper.one_euro(str, prec);
                double num6 = FinancialHelper.one_euro(str2, prec);
                if ((num5 >= 0.0) && (num6 >= 0.0))
                {
                    num4 = (num * num6) / num5;
                    if (!flag)
                    {
                        num4 = Math.Round(num4, digits);
                    }
                    return (double) num4;
                }
            }
            return CalcErrors.Value;
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
                return 5;
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
                return 3;
            }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "EUROCONVERT";
            }
        }
    }
}

