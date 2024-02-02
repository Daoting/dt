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
    /// Returns the <see cref="T:System.Double" /> inverse of the cumulative beta probability density function for a specified beta distribution.
    /// </summary>
    public class CalcBetaInvFunction : CalcBuiltinFunction
    {
        private double xtrunc;

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

        internal double chebyshev_eval(double x, object[] a, int n)
        {
            double num2;
            if ((n < 1) || (n > 0x3e8))
            {
                return double.NaN;
            }
            if ((x < -1.1) || (x > 1.1))
            {
                return double.NaN;
            }
            double num4 = x * 2.0;
            double num3 = num2 = 0.0;
            double num = 0.0;
            for (int i = 1; i <= n; i++)
            {
                num3 = num2;
                num2 = num;
                num = ((num4 * num2) - num3) + ((double) a[n - i]);
            }
            return ((num - num3) * 0.5);
        }

        internal int chebyshev_init(object[] dos, int nos, double eta)
        {
            if (nos < 1)
            {
                return 0;
            }
            double num3 = 0.0;
            int index = 0;
            for (int i = 1; i <= nos; i++)
            {
                index = nos - i;
                num3 += Math.Abs((double) ((double) dos[index]));
                if (num3 > eta)
                {
                    return index;
                }
            }
            return index;
        }

        internal double d1mach(int i)
        {
            switch (i)
            {
                case 1:
                    return 2.2250738585072014E-308;

                case 2:
                    return double.MaxValue;

                case 3:
                    return Math.Pow(2.0, -53.0);

                case 4:
                    return Math.Pow(2.0, -52.0);

                case 5:
                    return Math.Log10(2.0);
            }
            return 0.0;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> inverse of the cumulative beta probability density function for a specified beta distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 5 items: probability, alpha, beta, [A], [B].
        /// </para>
        /// <para>
        /// Probability is a probability associated with the beta distribution.
        /// </para>
        /// <para>
        /// Alpha is a parameter of the distribution.
        /// </para>
        /// <para>
        /// Beta is a parameter the distribution.
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
            int num19;
            double num22;
            double num27;
            double num28;
            double num29;
            double num32;
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
            if ((num2 <= 0.0) || (1.0 <= num2))
            {
                return CalcErrors.Number;
            }
            if ((num3 <= 0.0) || (num4 <= 0.0))
            {
                return CalcErrors.Number;
            }
            double num7 = 2.30753;
            double num8 = 0.27061;
            double num9 = 0.99229;
            double num10 = 0.04481;
            double num11 = 0.0;
            double num12 = 3E-308;
            double num13 = 1E-300;
            double num14 = num12;
            double num15 = 0.99999999999999978;
            double num16 = 5.0;
            double num17 = 6.0;
            double num18 = 2.0;
            double x = num2;
            if (((num3 < num11) || (num4 < num11)) || ((num2 < num11) || (num2 > 1.0)))
            {
                return (double) double.NaN;
            }
            if ((num2 == num11) || (num2 == 1.0))
            {
                return (double) num2;
            }
            double num24 = this.lbeta(num3, num4);
            if (num2 <= 0.5)
            {
                num22 = num2;
                num27 = num3;
                num29 = num4;
                num19 = 0;
            }
            else
            {
                num22 = 1.0 - num2;
                num27 = num4;
                num29 = num3;
                num19 = 1;
            }
            double num30 = Math.Sqrt(-Math.Log(num22 * num22));
            double num35 = num30 - ((num7 + (num8 * num30)) / (1.0 + ((num9 + (num10 * num30)) * num30)));
            if ((num27 > 1.0) && (num29 > 1.0))
            {
                num30 = ((num35 * num35) - 3.0) / 6.0;
                double num31 = 1.0 / ((num27 + num27) - 1.0);
                num32 = 1.0 / ((num29 + num29) - 1.0);
                double num26 = 2.0 / (num31 + num32);
                double num34 = ((num35 * Math.Sqrt(num26 + num30)) / num26) - ((num32 - num31) * ((num30 + (num16 / num17)) - (num18 / (3.0 * num26))));
                x = num27 / (num27 + (num29 * Math.Exp(num34 + num34)));
            }
            else
            {
                num30 = num29 + num29;
                num32 = 1.0 / (9.0 * num29);
                num32 = num30 * Math.Pow((1.0 - num32) + (num35 * Math.Sqrt(num32)), 3.0);
                if (num32 <= num11)
                {
                    x = 1.0 - Math.Exp((Math.Log((1.0 - num22) * num29) + num24) / num29);
                }
                else
                {
                    num32 = (((4.0 * num27) + num30) - num18) / num32;
                    if (num32 <= 1.0)
                    {
                        x = Math.Exp((Math.Log(num22 * num27) + num24) / num27);
                    }
                    else
                    {
                        x = 1.0 - (num18 / (num32 + 1.0));
                    }
                }
            }
            num30 = 1.0 - num27;
            num32 = 1.0 - num29;
            double num36 = num11;
            double num23 = 1.0;
            if (x < num14)
            {
                x = num14;
            }
            else if (x > num15)
            {
                x = num15;
            }
            double num37 = Math.Max(num13, Math.Pow(10.0, (-13.0 - (2.5 / (num27 * num27))) - (0.5 / (num22 * num22))));
            double num33 = num28 = num11;
            for (int j = 0; j < 0x3e8; j++)
            {
                num35 = (this.pbeta_raw(x, num27, num29) - num22) * Math.Exp((num24 + (num30 * Math.Log(x))) + (num32 * Math.Log(1.0 - x)));
                if ((num35 * num36) <= num11)
                {
                    num28 = Math.Max(Math.Abs(num23), num12);
                }
                double num25 = 1.0;
                for (int k = 0; k < 0x3e8; k++)
                {
                    num23 = num25 * num35;
                    if (Math.Abs(num23) < num28)
                    {
                        num33 = x - num23;
                        if ((num33 >= num11) && (num33 <= 1.0))
                        {
                            if ((num28 <= num37) || (Math.Abs(num35) <= num37))
                            {
                                break;
                            }
                            if ((num33 != num11) && (num33 != 1.0))
                            {
                                break;
                            }
                        }
                    }
                    num25 /= 3.0;
                }
                this.xtrunc = num33;
                if (this.xtrunc == x)
                {
                    break;
                }
                x = num33;
                num36 = num35;
            }
            if (num19 != 0)
            {
                x = 1.0 - x;
            }
            return CalcConvert.ToResult(((num6 - number) * x) + number);
        }

        internal double lbeta(double a, double b)
        {
            double num;
            double num3;
            double x = num3 = a;
            if (b < x)
            {
                x = b;
            }
            if (b > num3)
            {
                num3 = b;
            }
            if (x < 0.0)
            {
                return double.NaN;
            }
            if (x == 0.0)
            {
                return double.MaxValue;
            }
            if (x >= 10.0)
            {
                num = (this.lgammacor(x) + this.lgammacor(num3)) - this.lgammacor(x + num3);
                return (((((Math.Log(num3) * -0.5) + 0.91893853320467278) + num) + ((x - 0.5) * Math.Log(x / (x + num3)))) + (num3 * this.logrelerr(-x / (x + num3))));
            }
            if (num3 >= 10.0)
            {
                num = this.lgammacor(num3) - this.lgammacor(x + num3);
                object obj2 = new CalcGammaLnFunction().Evaluate(new object[] { (double) x });
                if (obj2 is CalcError)
                {
                    return double.NaN;
                }
                return ((((((double) obj2) + num) + x) - (x * Math.Log(x + num3))) + ((num3 - 0.5) * this.logrelerr(-x / (x + num3))));
            }
            double num4 = EngineeringHelper.gamma(x);
            double num5 = EngineeringHelper.gamma(num3);
            double num6 = EngineeringHelper.gamma(x + num3);
            return Math.Log(num4 * (num5 / num6));
        }

        internal double lgammacor(double x)
        {
            object[] dos = new object[] { (double) 0.16663894804518634, (double) -1.3849481760675639E-05, (double) 9.81082564692473E-09, (double) -1.8091294755724941E-11, (double) 6.2210980418926055E-14, (double) -3.399615005417722E-16, (double) 2.6831819984826989E-18, (double) -2.8680424353346431E-20, (double) 3.9628370610464347E-22, (double) -6.8318887539857674E-24, (double) 1.4292273559424982E-25, (double) -3.5475981581010704E-27, (double) 1.0256800580104709E-28, (double) -3.4011022543167491E-30, (double) 1.276642195630063E-31 };
            int n = 0;
            double num2 = 0.0;
            double num3 = 0.0;
            if (n == 0)
            {
                n = this.chebyshev_init(dos, 15, this.d1mach(3));
                num2 = 1.0 / Math.Sqrt(this.d1mach(3));
                num3 = Math.Exp(Math.Min(Math.Log(this.d1mach(2) / 12.0), -Math.Log(12.0 * this.d1mach(1))));
            }
            if (x < 10.0)
            {
                return double.NaN;
            }
            if (x >= num3)
            {
                return 4.9303806576313238E-32;
            }
            if (x < num2)
            {
                double num4 = 10.0 / x;
                return (this.chebyshev_eval(((num4 * num4) * 2.0) - 1.0, dos, n) / x);
            }
            return (1.0 / (x * 12.0));
        }

        internal double logrelerr(double x)
        {
            object[] dos = new object[] { 
                (double) 1.037869356274377, (double) -0.13364301504908918, (double) 0.019408249135520562, (double) -0.0030107551127535777, (double) 0.00048694614797154852, (double) -8.1054881893175362E-05, (double) 1.3778847799559525E-05, (double) -2.380221089435897E-06, (double) 4.1640416213865184E-07, (double) -7.3595828378075992E-08, (double) 1.3117611876241675E-08, (double) -2.3546709317742423E-09, (double) 4.2522773276035E-10, (double) -7.71908941348408E-11, (double) 1.407574648135907E-11, (double) -2.5769072058024682E-12, 
                (double) 4.7342406666294419E-13, (double) -8.7249012674742641E-14, (double) 1.6124614902740551E-14, (double) -2.9875652015665774E-15, (double) 5.5480701209082887E-16, (double) -1.0324619158271569E-16, (double) 1.9250239203049852E-17, (double) -3.5955073465265147E-18, (double) 6.726454253787686E-19, (double) -1.260262416873522E-19, (double) 2.3644884408606211E-20, (double) -4.4419377050807936E-21, (double) 8.3546594464034255E-22, (double) -1.5731559416479563E-22, (double) 2.9653128740247425E-23, (double) -5.5949583481815949E-24, 
                (double) 1.0566354268835681E-24, (double) -1.9972483680670205E-25, (double) 3.7782977818839361E-26, (double) -7.1531586889081743E-27, (double) 1.3552488463674215E-27, (double) -2.5694673048487566E-28, (double) 4.8747756066216946E-29, (double) -9.254211253084972E-30, (double) 1.7578597841760241E-30, (double) -3.341002667773101E-31, (double) 6.3533936180236182E-32
             };
            int n = 0;
            if (n == 0)
            {
                n = this.chebyshev_init(dos, 0x2b, 0.1 * this.d1mach(3));
                Math.Sqrt(this.d1mach(4));
            }
            if (x <= -1.0)
            {
                return double.NaN;
            }
            if (Math.Abs(x) <= 0.375)
            {
                return (x * (1.0 - (x * this.chebyshev_eval(x / 0.375, dos, n))));
            }
            return Math.Log(x + 1.0);
        }

        internal double pbeta(double x, double pin, double qin)
        {
            if ((pin <= 0.0) || (qin <= 0.0))
            {
                return double.NaN;
            }
            if (x <= 0.0)
            {
                return 0.0;
            }
            if (x >= 1.0)
            {
                return 1.0;
            }
            return this.pbeta_raw(x, pin, qin);
        }

        internal double pbeta_raw(double x, double pin, double qin)
        {
            double num;
            double num8;
            double num9;
            double num10;
            int num12;
            int num13;
            double d = 0.0;
            double num16 = 0.0;
            double num17 = 0.0;
            double num18 = 0.0;
            if (d == 0.0)
            {
                d = this.d1mach(3);
                num16 = Math.Log(d);
                num17 = this.d1mach(1);
                num18 = Math.Log(num17);
            }
            double num11 = x;
            double num4 = pin;
            double b = qin;
            if ((num4 / (num4 + b)) < x)
            {
                num11 = 1.0 - num11;
                num4 = qin;
                b = pin;
            }
            if ((((num4 + b) * num11) / (num4 + 1.0)) < d)
            {
                num = 0.0;
                num9 = ((num4 * Math.Log(Math.Max(num11, num17))) - Math.Log(num4)) - this.lbeta(num4, b);
                if ((num9 > num18) && (num11 != 0.0))
                {
                    num = Math.Exp(num9);
                }
                if ((num11 == x) && (num4 == pin))
                {
                    return num;
                }
                return (1.0 - num);
            }
            double a = b - Math.Floor(b);
            if (a == 0.0)
            {
                a = 1.0;
            }
            num9 = ((num4 * Math.Log(num11)) - this.lbeta(a, num4)) - Math.Log(num4);
            num = 0.0;
            if (num9 >= num18)
            {
                num = Math.Exp(num9);
                num8 = num * num4;
                if (a != 1.0)
                {
                    num12 = (int) Math.Max((double) (num16 / Math.Log(num11)), (double) 4.0);
                    for (num13 = 1; num13 <= num12; num13++)
                    {
                        num10 = num13;
                        num8 = ((num8 * (num10 - a)) * num11) / num10;
                        num += num8 / (num4 + num10);
                    }
                }
            }
            if (b > 1.0)
            {
                num9 = (((num4 * Math.Log(num11)) + (b * Math.Log(1.0 - num11))) - this.lbeta(num4, b)) - Math.Log(b);
                int num14 = (int) Math.Max((double) (num9 / num18), (double) 0.0);
                num8 = Math.Exp(num9 - (num14 * num18));
                double num2 = 1.0 / (1.0 - num11);
                double num6 = (b * num2) / ((num4 + b) - 1.0);
                double num3 = 0.0;
                num12 = (int) b;
                if (b == num12)
                {
                    num12--;
                }
                for (num13 = 1; num13 <= num12; num13++)
                {
                    if ((num6 <= 1.0) && ((num8 / d) <= num3))
                    {
                        break;
                    }
                    num10 = num13;
                    num8 = ((((b - num10) + 1.0) * num2) * num8) / ((num4 + b) - num10);
                    if (num8 > 1.0)
                    {
                        num14--;
                        num8 *= num17;
                    }
                    if (num14 == 0)
                    {
                        num3 += num8;
                    }
                }
                num += num3;
            }
            if ((num11 != x) || (num4 != pin))
            {
                num = 1.0 - num;
            }
            return Math.Max(Math.Min(num, 1.0), 0.0);
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
                return "BETAINV";
            }
        }
    }
}

