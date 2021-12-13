#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.CalcEngine.Functions
{
    internal class EngineeringHelper
    {
        internal static double EUL = 0.57721566490153287;
        internal static double INFINITY = double.MaxValue;
        internal static double MACHEP = 1.1102230246251565E-16;
        internal static double MAXNUM = double.MaxValue;
        internal static double PIO4 = 0.78539816339744828;
        internal static double SQ2OPI = 0.79788456080286541;
        internal static double THPIO4 = 2.3561944901923448;
        internal static double TWOOPI = 0.63661977236758138;

        public static double Bessel(double num, int order, bool modfied)
        {
            if (order < 0)
            {
                return double.NaN;
            }
            int num10 = 100;
            double x = num * 0.5;
            double num3 = x * x;
            x = Math.Pow(x, (double) order);
            double num4 = Fact(order);
            double num6 = 0.0;
            double num5 = 1.0;
            double num7 = order;
            double num8 = x / num4;
            double num9 = num8 * 0.9;
            if (modfied)
            {
                while ((num8 != num9) && (num10 != 0))
                {
                    x *= num3;
                    num6++;
                    num4 *= num6;
                    num7++;
                    num5 *= num7;
                    num9 = num8;
                    num8 += (x / num4) / num5;
                    num10--;
                }
                return num8;
            }
            for (bool flag = false; (num8 != num9) && (num10 != 0); flag = !flag)
            {
                x *= num3;
                num6++;
                num4 *= num6;
                num7++;
                num5 *= num7;
                num9 = num8;
                if (flag)
                {
                    num8 += (x / num4) / num5;
                }
                else
                {
                    num8 -= (x / num4) / num5;
                }
                num10--;
            }
            return num8;
        }

        internal static double expx2(double x, int sign)
        {
            x = Math.Abs(x);
            if (sign < 0)
            {
                x = -x;
            }
            double num3 = 0.0078125 * Math.Floor((double) ((128.0 * x) + 0.5));
            double num4 = x - num3;
            double d = num3 * num3;
            double num2 = ((2.0 * num3) * num4) + (num4 * num4);
            if (sign < 0)
            {
                d = -d;
                num2 = -num2;
            }
            if ((d + num2) > Math.Log(MAXNUM))
            {
                return INFINITY;
            }
            return (Math.Exp(d) * Math.Exp(num2));
        }

        private static double Fact(int n)
        {
            double num = 1.0;
            for (int i = n; i > 1; i--)
            {
                num *= i;
            }
            return num;
        }

        internal static double gamma(double x)
        {
            int num2;
            double num4;
            double num6;
            double num7 = 1.0;
            double[] numArray = new double[] { 
                1.0, 0.57721566490153287, -0.65587807152025379, -0.0420026350340952, 0.16653861138229151, -0.0421977345555443, -0.009621971527877, 0.007218943246663, -0.0011651675918591, -0.0002152416741149, 0.0001280502823882, -2.01348547807E-05, -1.2504934821E-06, 1.133027232E-06, -2.056338417E-07, 6.116095E-09, 
                5.0020075E-09, -1.1812746E-09, 1.043427E-10, 7.7823E-12, -3.6968E-12, 5.1E-13, -2.06E-14, -5.4E-15, 1.4E-15
             };
            if (x > 171.0)
            {
                return 1E+308;
            }
            if (x == ((int) x))
            {
                if (x > 0.0)
                {
                    num4 = 1.0;
                    for (int i = 2; i < x; i++)
                    {
                        num4 *= i;
                    }
                    return num4;
                }
                return 1E+308;
            }
            if (Math.Abs(x) > 1.0)
            {
                num6 = Math.Abs(x);
                int num3 = (int) num6;
                num7 = 1.0;
                for (num2 = 1; num2 <= num3; num2++)
                {
                    num7 *= num6 - num2;
                }
                num6 -= num3;
            }
            else
            {
                num6 = x;
            }
            double num5 = numArray[0x18];
            for (num2 = 0x17; num2 >= 0; num2--)
            {
                num5 = (num5 * num6) + numArray[num2];
            }
            num4 = 1.0 / (num5 * num6);
            if (Math.Abs(x) > 1.0)
            {
                num4 *= num7;
                if (x < 0.0)
                {
                    num4 = -3.1415926535897931 / ((x * num4) * Math.Sin(3.1415926535897931 * x));
                }
            }
            return num4;
        }

        internal static double j0(double x)
        {
            double num7;
            object[] coef = new object[] { (double) 0.00079693672929734706, (double) 0.08283523921074408, (double) 1.2395337164641429, (double) 5.4472500305876874, (double) 8.74716500199817, (double) 5.3032403823539491, (double) 1.0 };
            object[] objArray2 = new object[] { (double) 0.00092440881055886367, (double) 0.08562884743544745, (double) 1.2535274390105895, (double) 5.4709774033041709, (double) 8.7619088323706951, (double) 5.3060528823539466, (double) 1.0 };
            object[] objArray3 = new object[] { (double) -0.011366383889846916, (double) -1.2825271867050931, (double) -19.553954425773597, (double) -93.206015212376826, (double) -177.68116798048806, (double) -147.07750515495118, (double) -51.410532676659933, (double) -6.0501435060072852 };
            object[] objArray4 = new object[] { (double) 64.3178256118178, (double) 856.43002597698057, (double) 3882.4018360540163, (double) 7240.4677419565251, (double) 5930.7270118731694, (double) 2062.0933166032783, (double) 242.0057402402914 };
            object[] objArray5 = new object[] { (double) 15592.436785523574, (double) -14663929.590397161, (double) 5435264770.5187654, (double) -982136065717.9115, (double) 87590639439536.7, (double) -3466283033847297, (double) 44273326857256984, (double) -18495080043698668 };
            object[] objArray6 = new object[] { (double) 1041.2835366425984, (double) 626107.330137135, (double) 268919633.39381415, (double) 86400248710.3935, (double) 20297961275010.555, (double) 3171577528429750.5, (double) 2.5059625617265306E+17 };
            double num = 5.7831859629467841;
            double num2 = 30.471262343662087;
            object[] objArray7 = new object[] { (double) -4794432209.7820177, (double) 1956174919465.5657, (double) -249248344360967.72, (double) 9708622510473064 };
            object[] objArray8 = new object[] { (double) 499.563147152651, (double) 173785.40167637469, (double) 48440965.833996207, (double) 11185553704.535683, (double) 2112775201154.8921, (double) 310518229857422.56, (double) 31812195594320496, (double) 1.7108629408104315E+18 };
            double num3 = 0.78539816339744828;
            double num4 = 0.79788456080286541;
            if (x < 0.0)
            {
                x = -x;
            }
            if (x <= 5.0)
            {
                double num6 = x * x;
                if (x < 1E-05)
                {
                    return (1.0 - (num6 / 4.0));
                }
                num7 = (num6 - num) * (num6 - num2);
                return ((num7 * polevl(num6, objArray7, 3)) / p1evl(num6, objArray8, 8));
            }
            double num5 = 5.0 / x;
            double num8 = 25.0 / (x * x);
            num7 = polevl(num8, coef, 6) / polevl(num8, objArray2, 6);
            num8 = polevl(num8, objArray3, 7) / p1evl(num8, objArray4, 7);
            double d = x - num3;
            num7 = (num7 * Math.Cos(d)) - ((num5 * num8) * Math.Sin(d));
            return ((num7 * num4) / Math.Sqrt(x));
        }

        internal static double j1(double x)
        {
            double num4;
            object[] coef = new object[] { (double) -899971225.70555937, (double) 452228297998.19403, (double) -72749424522181.828, (double) 3682957328638529 };
            object[] objArray2 = new object[] { (double) 620.83647811805429, (double) 256987.25675774884, (double) 83514679.143194929, (double) 22151159547.979252, (double) 4749141220799.9141, (double) 784369607876235.88, (double) 89522233618462736, (double) 5.3227862033268009E+18 };
            object[] objArray3 = new object[] { (double) 0.051086259475017659, (double) 4.9821387295123341, (double) 75.823828413254532, (double) 366.7796093601508, (double) 710.85630499892613, (double) 597.48961240061362, (double) 211.68875710057213, (double) 25.207020585802372 };
            object[] objArray4 = new object[] { (double) 74.237327703567516, (double) 1056.4488603826283, (double) 4986.4105833765361, (double) 9562.3189240475622, (double) 7997.0416044735066, (double) 2826.1927851763908, (double) 336.0936078106983 };
            object[] objArray5 = new object[] { (double) 0.00076212561620817314, (double) 0.073139705694091756, (double) 1.1271960812968493, (double) 5.1120795114680764, (double) 8.424045901417724, (double) 5.2145159868236153, (double) 1.0 };
            object[] objArray6 = new object[] { (double) 0.00057132312807254865, (double) 0.068845590875449544, (double) 1.105142326340617, (double) 5.0738638612860152, (double) 8.3998555432760416, (double) 5.2098284868236187, (double) 1.0 };
            double num = 14.681970642123893;
            double num2 = 49.2184563216946;
            double num8 = 0.79788456080286541;
            double num9 = 2.3561944901923448;
            double num3 = x;
            if (x < 0.0)
            {
                num3 = -x;
            }
            if (num3 <= 5.0)
            {
                num4 = x * x;
                num3 = polevl(num4, coef, 3) / p1evl(num4, objArray2, 8);
                return (((num3 * x) * (num4 - num)) * (num4 - num2));
            }
            num3 = 5.0 / x;
            num4 = num3 * num3;
            double num5 = polevl(num4, objArray5, 6) / polevl(num4, objArray6, 6);
            double num6 = polevl(num4, objArray3, 7) / p1evl(num4, objArray4, 7);
            double d = x - num9;
            num5 = (num5 * Math.Cos(d)) - ((num3 * num6) * Math.Sin(d));
            return ((num5 * num8) / Math.Sqrt(x));
        }

        internal static double jn(int n, double x)
        {
            int num8;
            double num9 = 1.1102230246251565E-16;
            if (n < 0)
            {
                n = -n;
                if ((n & 1) == 0)
                {
                    num8 = 1;
                }
                else
                {
                    num8 = -1;
                }
            }
            else
            {
                num8 = 1;
            }
            if (x < 0.0)
            {
                if ((n & 1) != 0)
                {
                    num8 = -num8;
                }
                x = -x;
            }
            if (n == 0)
            {
                return (num8 * j0(x));
            }
            if (n == 1)
            {
                return (num8 * j1(x));
            }
            if (n == 2)
            {
                return (num8 * (((2.0 * j1(x)) / x) - j0(x)));
            }
            if (x < num9)
            {
                return 0.0;
            }
            int num7 = 0x38;
            double num3 = 2 * (n + num7);
            double num6 = num3;
            double num4 = x * x;
            do
            {
                num3 -= 2.0;
                num6 = num3 - (num4 / num6);
            }
            while (--num7 > 0);
            num6 = x / num6;
            num3 = 1.0;
            double num2 = 1.0 / num6;
            num7 = n - 1;
            double num5 = 2 * num7;
            do
            {
                double num = ((num2 * num5) - (num3 * x)) / x;
                num3 = num2;
                num2 = num;
                num5 -= 2.0;
            }
            while (--num7 > 0);
            if (Math.Abs(num3) > Math.Abs(num2))
            {
                num6 = j1(x) / num3;
            }
            else
            {
                num6 = j0(x) / num2;
            }
            return (num8 * num6);
        }

        internal static double kn(int nn, double x)
        {
            double num;
            double num3;
            double num6;
            double num7;
            double num8;
            double num9;
            double num11;
            double num12;
            double num13;
            int num17;
            int num18;
            if (nn < 0)
            {
                num18 = -nn;
            }
            else
            {
                num18 = nn;
            }
            if (num18 > 0x1f)
            {
                return MAXNUM;
            }
            if (x <= 0.0)
            {
                return MAXNUM;
            }
            if (x <= 9.55)
            {
                double num10 = 0.0;
                num8 = (0.25 * x) * x;
                num11 = 1.0;
                num12 = 0.0;
                double num14 = 1.0;
                double num16 = 2.0 / x;
                if (num18 > 0)
                {
                    num12 = -EUL;
                    num = 1.0;
                    for (num17 = 1; num17 < num18; num17++)
                    {
                        num12 += 1.0 / num;
                        num++;
                        num11 *= num;
                    }
                    num14 = num16;
                    if (num18 == 1)
                    {
                        num10 = 1.0 / x;
                    }
                    else
                    {
                        num3 = num11 / ((double) num18);
                        double num2 = 1.0;
                        num7 = num3;
                        num9 = -num8;
                        double num5 = 1.0;
                        for (num17 = 1; num17 < num18; num17++)
                        {
                            num3 /= (double) (num18 - num17);
                            num2 *= num17;
                            num5 *= num9;
                            num6 = (num3 * num5) / num2;
                            num7 += num6;
                            if ((MAXNUM - Math.Abs(num6)) < Math.Abs(num7))
                            {
                                return MAXNUM;
                            }
                            if ((num16 > 1.0) && ((MAXNUM / num16) < num14))
                            {
                                return MAXNUM;
                            }
                            num14 *= num16;
                        }
                        num7 *= 0.5;
                        num6 = Math.Abs(num7);
                        if ((num14 > 1.0) && ((MAXNUM / num14) < num6))
                        {
                            return MAXNUM;
                        }
                        if ((num6 > 1.0) && ((MAXNUM / num6) < num14))
                        {
                            return MAXNUM;
                        }
                        num10 = num7 * num14;
                    }
                }
                double num15 = 2.0 * Math.Log(0.5 * x);
                num13 = -EUL;
                if (num18 == 0)
                {
                    num12 = num13;
                    num6 = 1.0;
                }
                else
                {
                    num12 += 1.0 / ((double) num18);
                    num6 = 1.0 / num11;
                }
                num7 = ((num13 + num12) - num15) * num6;
                num = 1.0;
                do
                {
                    num6 *= num8 / (num * (num + num18));
                    num13 += 1.0 / num;
                    num12 += 1.0 / (num + num18);
                    num7 += ((num13 + num12) - num15) * num6;
                    num++;
                }
                while (Math.Abs((double) (num6 / num7)) > MACHEP);
                num7 = (0.5 * num7) / num14;
                if ((num18 & 1) != 0)
                {
                    num7 = -num7;
                }
                return (num10 + num7);
            }
            if (x > Math.Log(MAXNUM))
            {
                return 0.0;
            }
            num = num18;
            num12 = (4.0 * num) * num;
            num13 = 1.0;
            num8 = 8.0 * x;
            num11 = 1.0;
            num6 = 1.0;
            num7 = num6;
            double mAXNUM = MAXNUM;
            num17 = 0;
            do
            {
                num9 = num12 - (num13 * num13);
                num6 = (num6 * num9) / (num11 * num8);
                num3 = Math.Abs(num6);
                if ((num17 >= num18) && (num3 > mAXNUM))
                {
                    break;
                }
                mAXNUM = num3;
                num7 += num6;
                num11++;
                num13 += 2.0;
                num17++;
            }
            while (Math.Abs((double) (num6 / num7)) > MACHEP);
            return ((Math.Exp(-x) * Math.Sqrt(3.1415926535897931 / (2.0 * x))) * num7);
        }

        internal static double lgamma(double x)
        {
            int num6;
            int num7 = 0;
            double[] numArray = new double[] { 0.083333333333333329, -0.0027777777777777779, 0.00079365079365079365, -0.00059523809523809518, 0.00084175084175084182, -0.0019175269175269181, 0.00641025641025641, -0.029550653594771239, 0.17964437236883071, -1.3924322169059 };
            double d = x;
            if (x <= 0.0)
            {
                return 1E+308;
            }
            if ((x == 1.0) || (x == 2.0))
            {
                return 0.0;
            }
            if (x <= 7.0)
            {
                num7 = (int)(7 - x);
                d = x + num7;
            }
            double num2 = 1.0 / (d * d);
            double num3 = 6.2831853071795862;
            double num5 = numArray[9];
            for (num6 = 8; num6 >= 0; num6--)
            {
                num5 = (num5 * num2) + numArray[num6];
            }
            double num4 = (((num5 / d) + (0.5 * Math.Log(num3))) + ((d - 0.5) * Math.Log(d))) - d;
            if (x <= 7.0)
            {
                for (num6 = 1; num6 <= num7; num6++)
                {
                    num4 -= Math.Log(d - 1.0);
                    d--;
                }
            }
            return num4;
        }

        /// <summary>
        /// Converts the numeric value to its equivalent string representation in a given base.
        /// </summary>
        /// <param name="number">Numeric value</param>
        /// <param name="radix">Radix used in string representation</param>
        /// <param name="places">Minimum length of string representation</param>
        /// <returns>String representation of number</returns>
        public static string LongToString(long number, long radix, long places)
        {
            StringBuilder builder = new StringBuilder();
            if (number < 0L)
            {
                number += (long) Math.Pow((double) radix, 10.0);
            }
            while (number > 0L)
            {
                long num = number % radix;
                if ((0L <= num) && (num <= 9L))
                {
                    char ch = (char) ((ushort) (0x30L + num));
                    builder.Insert(0, ((char) ch).ToString());
                }
                else if ((10L <= num) && (num <= 15L))
                {
                    char ch2 = (char) ((ushort) (0x41L + (num - 10L)));
                    builder.Insert(0, ((char) ch2).ToString());
                }
                number /= radix;
            }
            while (builder.Length < places)
            {
                builder.Insert(0, "0");
            }
            return builder.ToString();
        }

        internal static double p1evl(double x, object[] coef, int N)
        {
            int num3 = 0;
            double num = x + ((double) coef[num3++]);
            int num2 = N - 1;
            do
            {
                num = (num * x) + ((double) coef[num3++]);
            }
            while (--num2 != 0);
            return num;
        }

        internal static double polevl(double x, object[] coef, int N)
        {
            int num3 = 0;
            double num = (double) ((double) coef[num3++]);
            int num2 = N;
            do
            {
                num = (num * x) + ((double) coef[num3++]);
            }
            while (--num2 != 0);
            return num;
        }

        /// <summary>
        /// Converts the string representation of a number in a given base to its equivalent numeric value.
        /// </summary>
        /// <param name="s">String representation of number</param>
        /// <param name="radix">Radix used in string representation</param>
        /// <param name="pos">Position of first non-parsed character</param>
        /// <returns>Numeric value</returns>
        /// <remarks>The empty string is treated as zero.</remarks>
        public static long StringToLong(string s, int radix, out int pos)
        {
            long num = (long) Math.Pow((double) radix, 10.0);
            long num2 = 0L;
            pos = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int num4 = 0;
                if (('0' <= s[i]) && (s[i] <= '9'))
                {
                    num4 = s[i] - '0';
                }
                else if (('A' <= s[i]) && (s[i] <= 'F'))
                {
                    num4 = (s[i] - 'A') + 10;
                }
                else
                {
                    if (('a' > s[i]) || (s[i] > 'f'))
                    {
                        break;
                    }
                    num4 = (s[i] - 'a') + 10;
                }
                if ((num4 < 0) || (radix <= num4))
                {
                    break;
                }
                num2 = (num2 * radix) + num4;
                pos++;
            }
            if ((num / 2L) <= num2)
            {
                num2 -= num;
            }
            return num2;
        }

        internal static double y0(double x)
        {
            double num;
            double num2;
            object[] coef = new object[] { (double) 0.00079693672929734706, (double) 0.08283523921074408, (double) 1.2395337164641429, (double) 5.4472500305876874, (double) 8.74716500199817, (double) 5.3032403823539491, (double) 1.0 };
            object[] objArray2 = new object[] { (double) 0.00092440881055886367, (double) 0.08562884743544745, (double) 1.2535274390105895, (double) 5.4709774033041709, (double) 8.7619088323706951, (double) 5.3060528823539466, (double) 1.0 };
            object[] objArray3 = new object[] { (double) -0.011366383889846916, (double) -1.2825271867050931, (double) -19.553954425773597, (double) -93.206015212376826, (double) -177.68116798048806, (double) -147.07750515495118, (double) -51.410532676659933, (double) -6.0501435060072852 };
            object[] objArray4 = new object[] { (double) 64.3178256118178, (double) 856.43002597698057, (double) 3882.4018360540163, (double) 7240.4677419565251, (double) 5930.7270118731694, (double) 2062.0933166032783, (double) 242.0057402402914 };
            object[] objArray5 = new object[] { (double) 15592.436785523574, (double) -14663929.590397161, (double) 5435264770.5187654, (double) -982136065717.9115, (double) 87590639439536.7, (double) -3466283033847297, (double) 44273326857256984, (double) -18495080043698668 };
            object[] objArray6 = new object[] { (double) 1041.2835366425984, (double) 626107.330137135, (double) 268919633.39381415, (double) 86400248710.3935, (double) 20297961275010.555, (double) 3171577528429750.5, (double) 2.5059625617265306E+17 };
            if (x <= 5.0)
            {
                if (x <= 0.0)
                {
                    return -MAXNUM;
                }
                num2 = x * x;
                num = polevl(num2, objArray5, 7) / p1evl(num2, objArray6, 7);
                return (num + ((TWOOPI * Math.Log(x)) * j0(x)));
            }
            num = 5.0 / x;
            num2 = 25.0 / (x * x);
            double num3 = polevl(num2, coef, 6) / polevl(num2, objArray2, 6);
            double num4 = polevl(num2, objArray3, 7) / p1evl(num2, objArray4, 7);
            double a = x - PIO4;
            num3 = (num3 * Math.Sin(a)) + ((num * num4) * Math.Cos(a));
            return ((num3 * SQ2OPI) / Math.Sqrt(x));
        }

        internal static double y1(double x)
        {
            double num;
            double num2;
            object[] coef = new object[] { (double) 0.051086259475017659, (double) 4.9821387295123341, (double) 75.823828413254532, (double) 366.7796093601508, (double) 710.85630499892613, (double) 597.48961240061362, (double) 211.68875710057213, (double) 25.207020585802372 };
            object[] objArray2 = new object[] { (double) 74.237327703567516, (double) 1056.4488603826283, (double) 4986.4105833765361, (double) 9562.3189240475622, (double) 7997.0416044735066, (double) 2826.1927851763908, (double) 336.0936078106983 };
            object[] objArray3 = new object[] { (double) 0.00076212561620817314, (double) 0.073139705694091756, (double) 1.1271960812968493, (double) 5.1120795114680764, (double) 8.424045901417724, (double) 5.2145159868236153, (double) 1.0 };
            object[] objArray4 = new object[] { (double) 0.00057132312807254865, (double) 0.068845590875449544, (double) 1.105142326340617, (double) 5.0738638612860152, (double) 8.3998555432760416, (double) 5.2098284868236187, (double) 1.0 };
            object[] objArray5 = new object[] { (double) 1263204747.9017804, (double) -647355876379.16028, (double) 114509511541823.73, (double) -8127702555013251, (double) 2.0243947571359491E+17, (double) -7.7887719626595008E+17 };
            object[] objArray6 = new object[] { (double) 594.30159234612825, (double) 235564.09294306857, (double) 73481194.445972174, (double) 18760131610.870617, (double) 3882312774962.3857, (double) 620557727146953.75, (double) 68714108735530048, (double) 3.9727060811656064E+18 };
            if (x <= 5.0)
            {
                if (x <= 0.0)
                {
                    return -MAXNUM;
                }
                num2 = x * x;
                num = x * (polevl(num2, objArray5, 5) / p1evl(num2, objArray6, 8));
                return (num + (TWOOPI * ((j1(x) * Math.Log(x)) - (1.0 / x))));
            }
            num = 5.0 / x;
            num2 = num * num;
            double num3 = polevl(num2, objArray3, 6) / polevl(num2, objArray4, 6);
            double num4 = polevl(num2, coef, 7) / p1evl(num2, objArray2, 7);
            double a = x - THPIO4;
            num3 = (num3 * Math.Sin(a)) + ((num * num4) * Math.Cos(a));
            return ((num3 * SQ2OPI) / Math.Sqrt(x));
        }

        internal static double yn(int n, double x)
        {
            double num;
            int num6;
            if (n < 0)
            {
                n = -n;
                if ((n & 1) == 0)
                {
                    num6 = 1;
                }
                else
                {
                    num6 = -1;
                }
            }
            else
            {
                num6 = 1;
            }
            if (n == 0)
            {
                return (num6 * y0(x));
            }
            if (n == 1)
            {
                return (num6 * y1(x));
            }
            if (x <= 0.0)
            {
                return -MAXNUM;
            }
            double num3 = y0(x);
            double num2 = y1(x);
            int num5 = 1;
            double num4 = 2 * num5;
            do
            {
                num = ((num4 * num2) / x) - num3;
                num3 = num2;
                num2 = num;
                num4 += 2.0;
                num5++;
            }
            while (num5 < n);
            return (num6 * num);
        }
    }
}

