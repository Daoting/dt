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
    /// Returns the <see cref="T:System.Double" /> one tailed probability of the chi-squared distribution.
    /// </summary>
    public class CalcChiDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> one tailed probability of the chi-squared distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: x, degrees_freedom.
        /// </para>
        /// <para>
        /// X is the value at which you want to evaluate the distribution.
        /// </para>
        /// <para>
        /// Degrees_freedom is the number of degrees of freedom.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num2;
            double num3;
            double num6;
            double num9;
            base.CheckArgumentsLength(args);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
            }
            if (!CalcConvert.TryToDouble(args[0], out num2, true) || !CalcConvert.TryToDouble(args[1], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (num2 < 0.0)
            {
                return CalcErrors.Number;
            }
            if ((num3 < 1.0) || (num3 > Math.Pow(10.0, 10.0)))
            {
                return CalcErrors.Number;
            }
            double num4 = Math.Log(Math.Sqrt(3.1415926535897931));
            double num5 = 1.0 / Math.Sqrt(3.1415926535897931);
            double num11 = 0.0;
            double d = num2;
            double num10 = 0.5 * d;
            bool flag = (num3 % 2.0) == 0.0;
            if (num3 > 1.0)
            {
                num11 = Math.Exp(-num10);
            }
            object obj2 = new CalcNormSDistFunction().Evaluate(new object[] { (double) -Math.Sqrt(d) });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            double num13 = (double) ((double) obj2);
            double num7 = flag ? num11 : (2.0 * num13);
            if (num3 <= 2.0)
            {
                return (double) num7;
            }
            d = 0.5 * (num3 - 1.0);
            double num8 = flag ? 1.0 : 0.5;
            if (num10 > 20.0)
            {
                num6 = flag ? 0.0 : num4;
                num9 = Math.Log(num10);
                while (num8 <= d)
                {
                    num6 = Math.Log(num8) + num6;
                    num7 += Math.Exp(((num9 * num8) - num10) - num6);
                    num8++;
                }
                return (double) num7;
            }
            num6 = flag ? 1.0 : (num5 / Math.Sqrt(num10));
            num9 = 0.0;
            while (num8 <= d)
            {
                num6 *= num10 / num8;
                num9 += num6;
                num8++;
            }
            return (double) ((num9 * num11) + num7);
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
                return "CHIDIST";
            }
        }
    }
}

