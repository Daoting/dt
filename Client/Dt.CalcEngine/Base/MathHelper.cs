#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.CalcEngine
{
    internal static class MathHelper
    {
        /// <summary>
        /// Provides quick lookup for some common powers of 10.
        /// </summary>
        private static Dictionary<int, double> _pow10 = new Dictionary<int, double>(0x11);

        static MathHelper()
        {
            _pow10[0] = 1.0;
            _pow10[1] = 10.0;
            _pow10[2] = 100.0;
            _pow10[3] = 1000.0;
            _pow10[4] = 10000.0;
            _pow10[5] = 100000.0;
            _pow10[6] = 1000000.0;
            _pow10[7] = 10000000.0;
            _pow10[8] = 100000000.0;
            _pow10[9] = 1000000000.0;
            _pow10[10] = 10000000000;
            _pow10[11] = 100000000000;
            _pow10[12] = 1000000000000;
            _pow10[13] = 10000000000000;
            _pow10[14] = 100000000000000;
            _pow10[15] = 1E+15;
            _pow10[0x10] = 1E+16;
        }

        /// <summary>
        /// A ceiling method taking ApproxEqual into account.
        /// </summary>
        public static double ApproxCeiling(this double x)
        {
            double num = Math.Ceiling(x);
            if (x.ApproxEqual(num - 1.0))
            {
                return (num - 1.0);
            }
            return num;
        }

        /// <summary>
        /// Tests the equal of two numbers with an accuracy of the magnitude
        /// of the given values scaled by 2^-48 (4 bits stripped).  This is used
        /// by the rounding and truncating methods to iron out small round off
        /// errors.
        /// </summary>
        public static bool ApproxEqual(this double x, double y)
        {
            return ((x == y) || (Math.Abs((double) (x - y)) < (Math.Abs(x) / 281474976710656)));
        }

        /// <summary>
        /// A floor method taking ApproxEqual into account.
        /// </summary>
        public static double ApproxFloor(this double x)
        {
            double num = Math.Floor(x);
            if (x.ApproxEqual(num + 1.0))
            {
                return (num + 1.0);
            }
            return num;
        }

        private static Func<object, bool> BuildCriteria(int crit, object criteria)
        {
            Func<string, string, bool> compareString;
            Func<double, double, bool> compareDouble;
            switch (crit)
            {
                case 0:
                    compareString = delegate (string v1, string v2) {
                        return string.Compare(v1, v2) < 1;
                    };
                    compareDouble = delegate (double v1, double v2) {
                        return v1 <= v2;
                    };
                    break;

                case 1:
                    compareString = delegate (string v1, string v2) {
                        return string.Compare(v1, v2) > -1;
                    };
                    compareDouble = delegate (double v1, double v2) {
                        return v1 >= v2;
                    };
                    break;

                case 2:
                    compareString = delegate (string v1, string v2) {
                        return string.Compare(v1, v2) != 0;
                    };
                    compareDouble = delegate (double v1, double v2) {
                        return !(v1 == v2);
                    };
                    break;

                case 3:
                    compareString = delegate (string v1, string v2) {
                        return string.Compare(v1, v2) == -1;
                    };
                    compareDouble = delegate (double v1, double v2) {
                        return v1 < v2;
                    };
                    break;

                case 4:
                    compareString = delegate (string v1, string v2) {
                        return string.Compare(v1, v2) == 0;
                    };
                    compareDouble = delegate (double v1, double v2) {
                        return v1 == v2;
                    };
                    break;

                case 5:
                    compareString = delegate (string v1, string v2) {
                        return string.Compare(v1, v2) == 1;
                    };
                    compareDouble = delegate (double v1, double v2) {
                        return v1 > v2;
                    };
                    break;

                default:
                    return delegate (object value) {
                        return false;
                    };
            }
            double critVal = -1.0;
            bool isCritNumber = true;
            if (criteria == null)
            {
                critVal = 0.0;
            }
            else if (!double.TryParse(criteria.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out critVal))
            {
                isCritNumber = false;
            }
            return delegate (object value) {
                if (value == null)
                {
                    return false;
                }
                if (isCritNumber)
                {
                    double number = -1.0;
                    if (CalcConvert.TryToDouble(value, out number, false))
                    {
                        return compareDouble(number, critVal);
                    }
                }
                return compareString(value.ToString().ToUpper(), (criteria != null) ? criteria.ToString().ToUpper() : "");
            };
        }

        public static double log1p(double x)
        {
            return (Math.Log(1.0 + x) - ((((1.0 + x) - 1.0) - x) / (1.0 + x)));
        }

        public static Func<object, bool> ParseCriteria(object criteria)
        {
            if ((!(criteria is double) && !(criteria is int)) && (!(criteria is float) && !(criteria is decimal)))
            {
                string str = (criteria != null) ? criteria.ToString().ToUpper() : "";
                char ch = '\0';
                for (int i = 0; (i < 2) && (i < str.Length); i++)
                {
                    char ch2 = str[i];
                    if ("=><".IndexOf(ch2) != -1)
                    {
                        if (ch2 == '=')
                        {
                            switch (ch)
                            {
                                case '<':
                                    return BuildCriteria(0, str.Substring(2));

                                case '>':
                                    return BuildCriteria(1, str.Substring(2));
                            }
                            return BuildCriteria(4, (ch == '\0') ? str.Substring(1) : criteria);
                        }
                        switch (ch)
                        {
                            case '\0':
                            {
                                ch = ch2;
                                continue;
                            }
                            case '<':
                                if (ch2 == '>')
                                {
                                    return BuildCriteria(2, str.Substring(2));
                                }
                                return BuildCriteria(3, str.Substring(1));

                            default:
                                if (ch != '>')
                                {
                                    continue;
                                }
                                return BuildCriteria(5, str.Substring(1));
                        }
                    }
                    switch (ch)
                    {
                        case '<':
                            return BuildCriteria(3, str.Substring(1));

                        case '>':
                            return BuildCriteria(5, str.Substring(1));
                    }
                }
            }
            return BuildCriteria(4, criteria);
        }

        /// <summary>
        /// Returns 10 raised to the specified power.
        /// </summary>
        public static double Pow10(int n)
        {
            double num;
            if (_pow10.TryGetValue(n, out num))
            {
                return num;
            }
            return Math.Pow(10.0, (double) n);
        }

        public static double pow1p(double x, double y)
        {
            double epsilon;
            if (Math.Abs(x) > 0.5)
            {
                epsilon = Math.Pow(1.0 + x, y);
            }
            else
            {
                epsilon = Math.Exp(y * log1p(x));
            }
            if (double.IsPositiveInfinity(epsilon))
            {
                return double.MaxValue;
            }
            if (double.IsNegativeInfinity(epsilon))
            {
                return double.MinValue;
            }
            if (double.IsNaN(epsilon))
            {
                epsilon = double.Epsilon;
            }
            return epsilon;
        }
    }
}

