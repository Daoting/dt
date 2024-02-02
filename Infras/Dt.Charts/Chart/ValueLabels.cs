#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Charts
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ValueLabels
    {
        public double[] Vals;
        public string[] Lbls;
        static long zero_ticks;
        public static ValueLabels CreateTime(double min, double max, IAxis ax)
        {
            DateTime time = FromOADate(min);
            DateTime time2 = FromOADate(max);
            ValueLabels labels = new ValueLabels();
            string annoFormat = ax.AnnoFormat;
            int annoNumber = ax.GetAnnoNumber();
            decimal num2 = (decimal)((decimal)((86400.0 * (max - min)) / ((double)annoNumber)));
            TimeSpan ts = TimeSpan.FromTicks((long) (num2 * 10000000M));
            TimeSpan span2 = double.IsNaN(ax.MajorUnit) ? TimeAxis.NiceTimeSpan(ts, annoFormat) : TimeSpan.FromDays(ax.MajorUnit);
            long ticks = span2.Ticks;
            double d = TimeAxis.RoundTime(min, span2.TotalDays, false);
            if (!double.IsInfinity(d) && !double.IsNaN(d))
            {
                min = d;
            }
            double num5 = TimeAxis.RoundTime(max, span2.TotalDays, true);
            if (!double.IsInfinity(num5) && !double.IsNaN(num5))
            {
                max = num5;
            }
            DateTime time3 = FromOADate(min);
            FromOADate(max);
            List<double> list = new List<double>();
            List<string> list2 = new List<string>();
            if ((span2.TotalDays >= 365.0) && double.IsNaN(ax.MajorUnit))
            {
                time3 = new DateTime(time.Year, 1, 1);
                if (time3 < time)
                {
                    time3 = time3.AddYears(1);
                }
                int num6 = (int)(span2.TotalDays / 365.0);
                for (DateTime time4 = time3; time4 <= time2; time4 = time4.AddYears(num6))
                {
                    double val = time4.ToOADate();
                    list.Add(val);
                    list2.Add(ax.Format(val));
                }
            }
            else if ((span2.TotalDays >= 30.0) && double.IsNaN(ax.MajorUnit))
            {
                time3 = new DateTime(time.Year, time.Month, 1);
                if (time3 < time)
                {
                    time3 = time3.AddMonths(1);
                }
                int num8 = (int)(span2.TotalDays / 30.0);
                for (DateTime time5 = time3; time5 <= time2; time5 = time5.AddMonths(num8))
                {
                    double num9 = time5.ToOADate();
                    list.Add(num9);
                    list2.Add(ax.Format(num9));
                }
            }
            else
            {
                for (DateTime time6 = time3; time6 <= time2; time6 = time6.AddTicks(ticks))
                {
                    double num10 = ToOADate(time6);
                    list.Add(num10);
                    list2.Add(ax.Format(num10));
                }
            }
            labels.Vals = list.ToArray();
            labels.Lbls = list2.ToArray();
            return labels;
        }

        static double ToOADate(DateTime dt)
        {
            decimal num = dt.Ticks - zero_ticks;
            num /= 864000000000M;
            return (double) ((double) num);
        }

        static DateTime FromOADate(double val)
        {
            decimal num = (decimal)((((decimal)val) * 864000000000M) + zero_ticks);
            return new DateTime((long) num);
        }

        public static ValueLabels Create(double min, double max, IAxis ax)
        {
            return Create(min, max, ax, 0.0);
        }

        public static ValueLabels Create(double min, double max, IAxis ax, double delta)
        {
            if ((ax != null) && ax.IsTime)
            {
                return CreateTime(min, max, ax);
            }
            ValueLabels labels = new ValueLabels();
            double range = max - min;
            if (range == 0.0)
            {
                labels.Vals = new double[] { min };
                labels.Lbls = new string[1];
                if (ax != null)
                {
                    labels.Lbls[0] = ax.Format(min);
                    return labels;
                }
                labels.Lbls[0] = ((double) min).ToString((IFormatProvider) CultureInfo.CurrentCulture);
                return labels;
            }
            if (ax != null)
            {
                if (!double.IsNaN(ax.LogBase))
                {
                    return CreateLogarithmic(min, max, ax);
                }
                delta = Utils.GetMajorUnit(range, ax, delta);
            }
            if (delta == 0.0)
            {
                delta = Utils.NiceTickNumber(range) * 0.1;
            }
            int num2 = (int)((range / delta) + 1);
            labels.Vals = new double[num2];
            labels.Lbls = new string[num2];
            double num3 = (min / delta) * delta;
            if (num3 < min)
            {
                num3 += delta;
            }
            for (int i = 0; i < num2; i++)
            {
                labels.Vals[i] = Math.Round((double) (num3 + (delta * i)), 14);
                if (ax != null)
                {
                    labels.Lbls[i] = ax.Format(labels.Vals[i]);
                }
                else
                {
                    labels.Lbls[i] = ((double) labels.Vals[i]).ToString((IFormatProvider) CultureInfo.CurrentCulture);
                }
            }
            return labels;
        }

        public static double[] CreateLogarithmicValues(double min, double max, double unit, IAxis ax, bool islabels)
        {
            double[] numArray = null;
            double logBase = ax.LogBase;
            try
            {
                int num2 = (int) Math.Floor(Math.Log(min, logBase));
                int num3 = (int) Math.Ceiling(Math.Log(max, logBase));
                double num5 = logBase;
                if (!double.IsNaN(unit))
                {
                    num5 = unit;
                    islabels = false;
                }
                if (num5 > logBase)
                {
                    num5 = logBase;
                }
                if (num5 <= 0.0)
                {
                    return numArray;
                }
                int num4 = (int)((((num3 - num2) + 1) * logBase) / num5);
                if (num4 > 0x80)
                {
                    if (isPowerOf(logBase, 10.0))
                    {
                        num5 = logBase / 10.0;
                    }
                    else if (isPowerOf(logBase, 5.0))
                    {
                        num5 = logBase / 5.0;
                    }
                    else
                    {
                        num5 = logBase / 16.0;
                    }
                    num4 = (int)((((num3 - num2) + 1) * logBase) / num5);
                    if (num4 > 0x80)
                    {
                        num5 = logBase;
                    }
                }
                int num6 = 1;
                if (islabels)
                {
                    int annoNumber = ax.GetAnnoNumber();
                    if (num4 > annoNumber)
                    {
                        num6 = (num4 / annoNumber) + 1;
                    }
                }
                double[] numArray2 = new double[num4];
                int index = 0;
                for (int i = num2; i <= num3; i += num6)
                {
                    double num10 = Math.Pow(logBase, (double) i);
                    int num11 = 0;
                    num11 = 0;
                    while (true)
                    {
                        if ((num11 * num5) >= (logBase - 1.0))
                        {
                            break;
                        }
                        double num12 = num10 * (1.0 + (num11 * num5));
                        if ((num12 >= min) && (num12 <= max))
                        {
                            numArray2[index] = num12;
                            index++;
                        }
                        num11++;
                    }
                }
                numArray = new double[index];
                Array.Copy(numArray2, numArray, index);
            }
            catch
            {
            }
            return numArray;
        }

        public static ValueLabels CreateLogarithmic(double min, double max, IAxis ax)
        {
            ValueLabels labels = new ValueLabels {
                Vals = CreateLogarithmicValues(min, max, ax.MajorUnit, ax, true)
            };
            if (labels.Vals != null)
            {
                int num = labels.Vals.Length;
                labels.Lbls = new string[num];
                for (int j = 0; j < num; j++)
                {
                    labels.Lbls[j] = ax.Format(labels.Vals[j]);
                }
                return labels;
            }
            labels.Vals = new double[] { min, max };
            int length = labels.Vals.Length;
            labels.Lbls = new string[length];
            for (int i = 0; i < length; i++)
            {
                labels.Lbls[i] = ax.Format(labels.Vals[i]);
            }
            return labels;
        }

        static bool isPowerOf(double lb, double v)
        {
            if ((lb % v) != 0.0)
            {
                return false;
            }
            double num = lb / v;
            while (num > 1.0)
            {
                num /= v;
            }
            return (num == 1.0);
        }

        static ValueLabels()
        {
            zero_ticks = 0.0.FromOADate().Ticks;
        }
    }
}

