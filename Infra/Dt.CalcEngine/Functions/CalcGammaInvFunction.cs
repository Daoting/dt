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
    /// Returns the <see cref="T:System.Double" /> inverse of the gamma cumulative distribution.
    /// </summary>
    public class CalcGammaInvFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> inverse of the gamma cumulative distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: probability, alpha, beta.
        /// </para>
        /// <para>
        /// Probability is the probability associated with the gamma distribution.
        /// </para>
        /// <para>
        /// Alpha is a parameter to the distribution.
        /// </para>
        /// <para>
        /// Beta is a parameter to the distribution. If beta = 1, GAMMAINV returns the standard gamma distribution.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            double num42;
            double num45;
            double num47;
            double num49;
            double num50;
            double num57;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true)) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            double num4 = 4.67;
            double num5 = 6.66;
            double num6 = 6.73;
            double num7 = 13.32;
            double num8 = 60.0;
            double num9 = 70.0;
            double num10 = 84.0;
            double num11 = 105.0;
            double num12 = 120.0;
            double num13 = 127.0;
            double num14 = 140.0;
            double num15 = 1175.0;
            double num16 = 210.0;
            double num17 = 252.0;
            double num18 = 2264.0;
            double num19 = 294.0;
            double num20 = 346.0;
            double num21 = 420.0;
            double num22 = 462.0;
            double num23 = 606.0;
            double num24 = 672.0;
            double num25 = 707.0;
            double num26 = 735.0;
            double num27 = 889.0;
            double num28 = 932.0;
            double num29 = 966.0;
            double num30 = 1141.0;
            double num31 = 1182.0;
            double num32 = 1278.0;
            double num33 = 1740.0;
            double num34 = 2520.0;
            double num35 = 5040.0;
            double num36 = 5E-07;
            double num37 = 0.01;
            double num38 = 5E-07;
            double num39 = 20.0;
            double num40 = 2E-06;
            double num41 = 0.999998;
            if (((num < 0.0) || (1.0 < num)) || ((num2 <= 0.0) || (num3 <= 0.0)))
            {
                return CalcErrors.Number;
            }
            if (num < num40)
            {
                return (double) 0.0;
            }
            if (num > num41)
            {
                return (double) double.MaxValue;
            }
            double num48 = 2.0 * num2;
            double num44 = num2 - 1.0;
            object obj2 = new CalcGammaLnFunction().Evaluate(new object[] { (double) num2 });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            double num46 = (double) ((double) obj2);
            if (num48 < (-1.24 * Math.Log(num)))
            {
                num45 = Math.Pow((num * num2) * Math.Exp(num46 + (num2 * 0.69314718055994529)), 1.0 / num2);
                if (num45 < num36)
                {
                    return (double) double.NaN;
                }
            }
            else if (num48 > 0.32)
            {
                obj2 = new CalcNormInvFunction().Evaluate(new object[] { (double) num, (double) 0.0, (double) 1.0 });
                if (obj2 is CalcError)
                {
                    return obj2;
                }
                double num58 = (double) ((double) obj2);
                num47 = 0.222222 / num48;
                num45 = num48 * Math.Pow(((num58 * Math.Sqrt(num47)) + 1.0) - num47, 3.0);
                if (num45 > ((2.2 * num48) + 6.0))
                {
                    num45 = -2.0 * ((Math.Log(1.0 - num) - (num44 * Math.Log(0.5 * num45))) + num46);
                }
            }
            else
            {
                num45 = 0.4;
                num42 = (Math.Log(1.0 - num) + num46) + (num44 * 0.69314718055994529);
                do
                {
                    num50 = num45;
                    num47 = 1.0 + (num45 * (num4 + num45));
                    num49 = num45 * (num6 + (num45 * (num5 + num45)));
                    num57 = (-0.5 + ((num4 + (2.0 * num45)) / num47)) - ((num6 + (num45 * (num7 + (3.0 * num45)))) / num49);
                    num45 -= (1.0 - ((Math.Exp(num42 + (0.5 * num45)) * num49) / num47)) / num57;
                }
                while (Math.Abs((double) ((num50 / num45) - 1.0)) > num37);
            }
            for (int i = 1; i <= num39; i++)
            {
                num50 = num45;
                num47 = 0.5 * num45;
                obj2 = new CalcGammaDistFunction().Evaluate(new object[] { (double) num47, (double) num2, (double) 1.0, (bool) true });
                if (obj2 is CalcError)
                {
                    return obj2;
                }
                num49 = num - ((double) obj2);
                num57 = num49 * Math.Exp((((num2 * 0.69314718055994529) + num46) + num47) - (num44 * Math.Log(num45)));
                double num43 = num57 / num45;
                num42 = (0.5 * num57) - (num43 * num44);
                double num51 = (num16 + (num42 * (num14 + (num42 * (num11 + (num42 * (num10 + (num42 * (num9 + (num8 * num42)))))))))) / num21;
                double num52 = (num21 + (num42 * (num26 + (num42 * (num29 + (num42 * (num30 + (num32 * num42)))))))) / num34;
                double num53 = (num16 + (num42 * (num22 + (num42 * (num25 + (num28 * num42)))))) / num34;
                double num54 = ((num17 + (num42 * (num24 + (num31 * num42)))) + (num44 * (num19 + (num42 * (num27 + (num33 * num42)))))) / num35;
                double num55 = ((num10 + (num18 * num42)) + (num44 * (num15 + (num23 * num42)))) / num34;
                double num56 = (num12 + (num44 * (num20 + (num13 * num44)))) / num35;
                num45 += num57 * ((1.0 + ((0.5 * num57) * num51)) - ((num43 * num44) * (num51 - (num43 * (num52 - (num43 * (num53 - (num43 * (num54 - (num43 * (num55 - (num43 * num56))))))))))));
                if (Math.Abs((double) ((num50 / num45) - 1.0)) > num38)
                {
                    return (double) ((0.5 * num3) * num45);
                }
            }
            return (double) ((0.5 * num3) * num45);
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
                return "GAMMAINV";
            }
        }
    }
}

