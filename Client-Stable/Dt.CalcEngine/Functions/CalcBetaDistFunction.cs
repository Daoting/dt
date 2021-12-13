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
    /// Returns the <see cref="T:System.Double" /> cumulative beta probability density function. 
    /// </summary>
    public class CalcBetaDistFunction : CalcBuiltinFunction
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
            if (i != 3)
            {
                return (i == 4);
            }
            return true;
        }

        private double betacf(double a, double b, double x)
        {
            int num11 = 300;
            double num12 = 1E-50;
            double num13 = 1E-20;
            double num8 = a + b;
            double num10 = a + 1.0;
            double num9 = a - 1.0;
            double num5 = 1.0;
            double num6 = 1.0 - ((num8 * x) / num10);
            if (Math.Abs(num6) < num12)
            {
                num6 = num12;
            }
            num6 = 1.0 / num6;
            double num3 = num6;
            for (int i = 1; i <= num11; i++)
            {
                int num2 = i + i;
                double num4 = (((b - i) * i) * x) / ((num9 + num2) * (a + num2));
                num6 = 1.0 + (num4 * num6);
                if (Math.Abs(num6) < num12)
                {
                    num6 = num12;
                }
                num5 = 1.0 + (num4 / num5);
                if (Math.Abs(num5) < num12)
                {
                    num5 = num12;
                }
                num6 = 1.0 / num6;
                num3 *= num6 * num5;
                num4 = 0.0 - ((((a + i) * (num8 + i)) * x) / ((a + num2) * (num10 + num2)));
                num6 = 1.0 + (num4 * num6);
                if (Math.Abs(num6) < num12)
                {
                    num6 = num12;
                }
                num5 = 1.0 + (num4 / num5);
                if (Math.Abs(num5) < num12)
                {
                    num5 = num12;
                }
                num6 = 1.0 / num6;
                double num7 = num6 * num5;
                num3 *= num7;
                if (Math.Abs((double) (num7 - 1.0)) < num13)
                {
                    return num3;
                }
            }
            return num3;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> cumulative beta probability density function.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 5 items: x, alpha, beta, [A], [B].
        /// </para>
        /// <para>
        /// X is the value between A and B at which to evaluate the function.
        /// </para>
        /// <para>
        /// Alpha is a parameter of the distribution.
        /// </para>
        /// <para>
        /// Beta is a parameter of the distribution.
        /// </para>
        /// <para>
        /// A is an optional lower bound to the interval of x.
        /// </para>
        /// <para>
        /// B is an optional upper bound to the interval of x.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num2;
            double num3;
            double num4;
            double num7;
            base.CheckArgumentsLength(args);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
            }
            if ((!CalcConvert.TryToDouble(args[0], out num2, true) || !CalcConvert.TryToDouble(args[1], out num3, true)) || !CalcConvert.TryToDouble(args[2], out num4, true))
            {
                return CalcErrors.Value;
            }
            double number = 0.0;
            if (CalcHelper.ArgumentExists(args, 3) && !CalcConvert.TryToDouble(args[3], out number, true))
            {
                return CalcErrors.Value;
            }
            double num6 = 1.0;
            if (CalcHelper.ArgumentExists(args, 4) && !CalcConvert.TryToDouble(args[4], out num6, true))
            {
                return CalcErrors.Value;
            }
            if ((num3 <= 0.0) || (num4 <= 0.0))
            {
                return CalcErrors.Number;
            }
            if (((num2 < number) || (num6 < num2)) || (number == num6))
            {
                return CalcErrors.Number;
            }
            double d = (num2 - number) / (num6 - number);
            bool flag = false;
            double num10 = this.Lanczos(num3 + num4);
            double num11 = this.Lanczos(num3);
            double num12 = this.Lanczos(num4);
            double num13 = Math.Log(d);
            double num14 = Math.Log(1.0 - d);
            double num8 = Math.Exp((((num10 - num11) - num12) + (num3 * num13)) + (num4 * num14));
            if (d < ((num3 + 1.0) / ((num3 + num4) + 2.0)))
            {
                num7 = (num8 * this.betacf(num3, num4, d)) / num3;
            }
            else
            {
                num7 = 1.0 - ((num8 * this.betacf(num4, num3, 1.0 - d)) / num4);
            }
            if (flag)
            {
                num7 = -1.0;
            }
            return CalcConvert.ToResult(num7);
        }

        private double Lanczos(double p)
        {
            double num = p;
            double d = num + 5.5;
            d -= (num + 0.5) * Math.Log(d);
            double num3 = 1.0000000001900149 + (76.180091729471457 / (p + 1.0));
            num3 -= 86.505320329416776 / (p + 2.0);
            num3 += 24.014098240830911 / (p + 3.0);
            num3 -= 1.231739572450155 / (p + 4.0);
            num3 += 0.001208650973866179 / (p + 5.0);
            num3 -= 5.395239384953E-06 / (p + 6.0);
            return (Math.Log((2.5066282746310011 * num3) / num) - d);
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
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "BETADIST";
            }
        }
    }
}

