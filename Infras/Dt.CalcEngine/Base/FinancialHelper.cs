#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Functions;
using System;
#endregion

namespace Dt.CalcEngine
{
    internal class FinancialHelper
    {
        internal static int annual_year_basis(DateTime date, int basis)
        {
            return Annual_Year_Basis(date, basis);
        }

        internal static int Annual_Year_Basis(DateTime date, int basis)
        {
            switch (basis)
            {
                case 0:
                    return 360;

                case 1:
                    if (DateTime.IsLeapYear(date.Year))
                    {
                        return 0x16e;
                    }
                    return 0x16d;

                case 2:
                    return 360;

                case 3:
                    return 0x16d;

                case 4:
                    return 360;
            }
            return -1;
        }

        internal static double calc_oddfprice(DateTime settlement, DateTime maturity, DateTime issue, DateTime first_coupon, double rate, double yield, double redemption, int freq, int basis)
        {
            double num9;
            double num = days_between_basis(issue, settlement, basis);
            double num2 = days_between_basis(settlement, first_coupon, basis);
            double num3 = days_between_basis(issue, first_coupon, basis);
            double num4 = coupdays(settlement, maturity, freq, basis);
            int num5 = coupnum(settlement, maturity, freq);
            double num6 = (100.0 * rate) / ((double) freq);
            double x = 1.0 + (yield / ((double) freq));
            if (num2 > num4)
            {
                switch (basis)
                {
                    case 0:
                    case 4:
                    {
                        int num11 = days_between_basis(first_coupon, maturity, basis);
                        num5 = 1 + ((int) Math.Ceiling((double) (((double) num11) / num4)));
                        goto Label_014A;
                    }
                    default:
                    {
                        DateTime time = new DateTime(first_coupon.Year, first_coupon.Month, first_coupon.Day);
                        num5 = 0;
                        while (num5 < 0x7fffffff)
                        {
                            DateTime from = new DateTime(time.Year, time.Month, time.Day);
                            time = time.AddMonths(12 / freq);
                            if (DateTime.Compare(time, maturity) >= 0)
                            {
                                num5 += ((int) Math.Ceiling((double) (((double) days_between_basis(from, maturity, basis)) / coupdays(from, time, freq, basis)))) + 1;
                                break;
                            }
                            num5++;
                        }
                        break;
                    }
                }
                num = num4 * date_ratio(issue, settlement, first_coupon, freq, basis);
                num2 = num4 * date_ratio(settlement, first_coupon, first_coupon, freq, basis);
                num3 = num4 * date_ratio(issue, first_coupon, first_coupon, freq, basis);
            }
        Label_014A:
            num9 = redemption / Math.Pow(x, (num5 - 1.0) + (num2 / num4));
            double num10 = (num3 / num4) / Math.Pow(x, num2 / num4);
            double num8 = (Math.Pow(x, -num2 / num4) * (Math.Pow(x, (double) -num5) - (1.0 / x))) / ((1.0 / x) - 1.0);
            return (num9 + (num6 * ((num10 + num8) - (num / num4))));
        }

        internal static int calcPrecision(string str)
        {
            switch (str[0])
            {
                case 'A':
                    if (string.Compare("ATS", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'B':
                    if (string.Compare("BEF", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 0;

                case 'D':
                    if (string.Compare("DEM", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'E':
                    if (string.Compare("ESP", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("EUR", 0, str, 0, 3) != 0)
                        {
                            break;
                        }
                        return 2;
                    }
                    return 0;

                case 'F':
                    if (string.Compare("FIM", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("FRF", 0, str, 0, 3) != 0)
                        {
                            break;
                        }
                        return 2;
                    }
                    return 2;

                case 'G':
                    if (string.Compare("GRD", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'I':
                    if (string.Compare("IEP", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("ITL", 0, str, 0, 3) == 0)
                        {
                            return 0;
                        }
                        break;
                    }
                    return 2;

                case 'L':
                    if ((string.Compare("LUX", 0, str, 0, 3) != 0) && (string.Compare("LUF", 0, str, 0, 3) != 0))
                    {
                        break;
                    }
                    return 0;

                case 'N':
                    if (string.Compare("NLG", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'P':
                    if (string.Compare("PTE", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 1;
            }
            return 2;
        }

        internal static double calculate_fvifa(double rate, double nper)
        {
            if (rate == 0.0)
            {
                return nper;
            }
            double num2 = Math.Pow(1.0 + rate, nper) - 1.0;
            double d = num2 / rate;
            if (!double.IsNaN(d) && !double.IsInfinity(d))
            {
                return d;
            }
            return double.NaN;
        }

        internal static double calculate_interest_part(double pv, double pmt, double rate, double per)
        {
            double num = Math.Pow(1.0 + rate, per);
            double num2 = num - 1.0;
            double d = -(((pv * num) * rate) + (pmt * num2));
            if (!double.IsNaN(d) && !double.IsInfinity(d))
            {
                return d;
            }
            return double.NaN;
        }

        internal static double calculate_pmt(double rate, double nper, double pv, double fv, int type)
        {
            double num = calculate_pvif(rate, nper);
            double num2 = calculate_fvifa(rate, nper);
            double num3 = (-pv * num) - fv;
            double num4 = 1.0 + (rate * type);
            double num5 = num4 * num2;
            double d = num3 / num5;
            if (!double.IsNaN(d) && !double.IsInfinity(d))
            {
                return d;
            }
            return double.NaN;
        }

        internal static double calculate_pvif(double rate, double nper)
        {
            double d = Math.Pow(1.0 + rate, nper);
            if (!double.IsNaN(d) && !double.IsInfinity(d))
            {
                return d;
            }
            return double.NaN;
        }

        internal static DateTime coup_cd(DateTime settlement, DateTime maturity, int freq, bool next)
        {
            DateTime time = new DateTime(1, 1, 1);
            bool flag = maturity.Day == DateTime.DaysInMonth(maturity.Year, maturity.Month);
            int num = 12 / freq;
            int num2 = maturity.Year - settlement.Year;
            if (num2 > 0)
            {
                num2 = (num2 - 1) * freq;
            }
            do
            {
                time = new DateTime(maturity.Year, maturity.Month, maturity.Day);
                num2++;
                time = time.AddMonths(-(num2 * num));
                if (flag)
                {
                    int day = DateTime.DaysInMonth(time.Year, time.Month);
                    time = new DateTime(time.Year, time.Month, day);
                }
            }
            while (DateTime.Compare(settlement, time) < 0);
            if (next)
            {
                time = new DateTime(maturity.Year, maturity.Month, maturity.Day);
                num2--;
                time = time.AddMonths(-(num2 * num));
                if (flag)
                {
                    int num4 = DateTime.DaysInMonth(time.Year, time.Month);
                    time = new DateTime(time.Year, time.Month, num4);
                }
            }
            return time;
        }

        internal static double coupdaybs(DateTime settlement, DateTime maturity, int freq, int basis)
        {
            return (double) days_between_basis(coup_cd(settlement, maturity, freq, false), settlement, basis);
        }

        internal static double coupdays(DateTime settlement, DateTime maturity, int freq, int basis)
        {
            switch (basis)
            {
                case 0:
                case 2:
                case 4:
                case 5:
                    return (double) (360 / freq);

                case 3:
                    return (365.0 / ((double) freq));
            }
            DateTime to = coup_cd(settlement, maturity, freq, true);
            return (double) days_between_basis(coup_cd(settlement, maturity, freq, false), to, 1);
        }

        internal static double coupdaysnc(DateTime settlement, DateTime maturity, int freq, int basis)
        {
            DateTime to = coup_cd(settlement, maturity, freq, true);
            return (double) days_between_basis(settlement, to, basis);
        }

        internal static double coupncd(DateTime settlement, DateTime maturity, int freq)
        {
            return coup_cd(settlement, maturity, freq, true).ToOADate();
        }

        internal static int coupnum(DateTime settlement, DateTime maturity, int freq)
        {
            DateTime time = new DateTime(maturity.Year, maturity.Month, maturity.Day);
            int num = (maturity.Month - settlement.Month) + (12 * (maturity.Year - settlement.Year));
            time = time.AddMonths(-num);
            if (maturity.Day == DateTime.DaysInMonth(maturity.Year, maturity.Month))
            {
                while (time.Day != DateTime.DaysInMonth(time.Year, time.Month))
                {
                    time = time.AddDays(1.0);
                }
            }
            if (settlement.Day >= time.Day)
            {
                num--;
            }
            return (1 + (num / (12 / freq)));
        }

        internal static double couppcd(DateTime settlement, DateTime maturity, int freq)
        {
            return coup_cd(settlement, maturity, freq, false).ToOADate();
        }

        internal static double date_ratio(DateTime d1, DateTime d2, DateTime d3, int freq, int basis)
        {
            DateTime time = coup_cd(d1, d3, freq, true);
            DateTime settlement = coup_cd(d1, d3, freq, false);
            if (DateTime.Compare(time, d2) >= 0)
            {
                return (((double) days_between_basis(d1, d2, basis)) / coupdays(settlement, time, freq, basis));
            }
            double num = ((double) days_between_basis(d1, time, basis)) / coupdays(settlement, time, freq, basis);
            while (true)
            {
                settlement = new DateTime(time.Year, time.Month, time.Day);
                time = time.AddMonths(12 / freq);
                if (DateTime.Compare(time, d2) >= 0)
                {
                    return (num + (((double) days_between_basis(settlement, d2, basis)) / coupdays(settlement, time, freq, basis)));
                }
                num++;
            }
        }

        internal static int days_between_basis(DateTime from, DateTime to, int basis)
        {
            return Days_Between_Basis(from, to, basis);
        }

        internal static int Days_Between_Basis(DateTime from, DateTime to, int basis)
        {
            int num = 1;
            if (DateTime.Compare(from, to) > 0)
            {
                DateTime time = from;
                from = to;
                to = time;
                num = -1;
            }
            switch (basis)
            {
                case 1:
                case 2:
                case 3:
                    return (num * Convert.ToInt32((double) (to.ToOADate() - from.ToOADate())));

                case 4:
                    return (num * Days_Between_BASIS_30E_360(from, to));

                case 5:
                    return (num * Days_Between_BASIS_30Ep_360(from, to));

                case 6:
                    return (num * Days_Between_BASIS_MSRB_30_360_SYM(from, to));
            }
            return (num * Days_Between_BASIS_MSRB_30_360(from, to));
        }

        internal static int days_between_BASIS_30E_360(DateTime from, DateTime to)
        {
            return Days_Between_BASIS_30E_360(from, to);
        }

        internal static int Days_Between_BASIS_30E_360(DateTime from, DateTime to)
        {
            int year = from.Year;
            int month = from.Month;
            int day = from.Day;
            int num4 = to.Year;
            int num5 = to.Month;
            int num6 = to.Day;
            if (day == 0x1f)
            {
                day = 30;
            }
            if (num6 == 0x1f)
            {
                num6 = 30;
            }
            return ((((num4 - year) * 360) + ((num5 - month) * 30)) + (num6 - day));
        }

        internal static int days_between_BASIS_30Ep_360(DateTime from, DateTime to)
        {
            return Days_Between_BASIS_30Ep_360(from, to);
        }

        internal static int Days_Between_BASIS_30Ep_360(DateTime from, DateTime to)
        {
            int year = from.Year;
            int month = from.Month;
            int day = from.Day;
            int num4 = to.Year;
            int num5 = to.Month;
            int num6 = to.Day;
            if (day == 0x1f)
            {
                day = 30;
            }
            if (num6 == 0x1f)
            {
                num6 = 1;
                num5++;
            }
            return ((((num4 - year) * 360) + ((num5 - month) * 30)) + (num6 - day));
        }

        internal static int days_between_BASIS_MSRB_30_360(DateTime from, DateTime to)
        {
            return Days_Between_BASIS_MSRB_30_360(from, to);
        }

        internal static int Days_Between_BASIS_MSRB_30_360(DateTime from, DateTime to)
        {
            int year = from.Year;
            int month = from.Month;
            int day = from.Day;
            int num4 = to.Year;
            int num5 = to.Month;
            int num6 = to.Day;
            if (((month == 2) && (DateTime.DaysInMonth(from.Year, from.Month) == from.Day)) && ((num5 == 2) && (DateTime.DaysInMonth(to.Year, to.Month) == to.Day)))
            {
                day = 30;
                num6 = 30;
            }
            if ((num6 == 0x1f) && (day >= 30))
            {
                num6 = 30;
            }
            if (day == 0x1f)
            {
                day = 30;
            }
            return ((((num4 - year) * 360) + ((num5 - month) * 30)) + (num6 - day));
        }

        internal static int days_between_BASIS_MSRB_30_360_SYM(DateTime from, DateTime to)
        {
            return Days_Between_BASIS_MSRB_30_360_SYM(from, to);
        }

        internal static int Days_Between_BASIS_MSRB_30_360_SYM(DateTime from, DateTime to)
        {
            int year = from.Year;
            int month = from.Month;
            int day = from.Day;
            int num4 = to.Year;
            int num5 = to.Month;
            int num6 = to.Day;
            if ((month == 2) && (DateTime.DaysInMonth(from.Year, from.Month) == from.Day))
            {
                day = 30;
            }
            if ((num5 == 2) && (DateTime.DaysInMonth(to.Year, to.Month) == to.Day))
            {
                num6 = 30;
            }
            if ((num6 == 0x1f) && (day >= 30))
            {
                num6 = 30;
            }
            if (day == 0x1f)
            {
                day = 30;
            }
            return ((((num4 - year) * 360) + ((num5 - month) * 30)) + (num6 - day));
        }

        internal static int days_monthly_basis(DateTime date_i, DateTime date_m, int basis)
        {
            int year = date_i.Year;
            int month = date_i.Month;
            int day = date_i.Day;
            int num6 = date_m.Year;
            int num5 = date_m.Month;
            int num4 = date_m.Day;
            int num9 = num6 - year;
            int num7 = num5 - month;
            int num8 = num4 - day;
            num7 = (num9 * 12) + num7;
            DateTime.IsLeapYear(year);
            switch (basis)
            {
                case 0:
                {
                    CalcDays360Function function = new CalcDays360Function();
                    object[] args = new object[] { date_i, date_m };
                    return (int) ((int) function.Evaluate(args));
                }
                case 1:
                case 2:
                case 3:
                {
                    int num11 = (int) date_i.ToOADate();
                    int num10 = (int) date_m.ToOADate();
                    return (num10 - num11);
                }
                case 4:
                    return ((num7 * 30) + num8);
            }
            return -1;
        }

        internal static int displayPrecision(string str)
        {
            switch (str[0])
            {
                case 'A':
                    if (string.Compare("ATS", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'B':
                    if (string.Compare("BEF", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 0;

                case 'D':
                    if (string.Compare("DEM", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'E':
                    if (string.Compare("ESP", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("EUR", 0, str, 0, 3) != 0)
                        {
                            break;
                        }
                        return 2;
                    }
                    return 0;

                case 'F':
                    if (string.Compare("FIM", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("FRF", 0, str, 0, 3) != 0)
                        {
                            break;
                        }
                        return 2;
                    }
                    return 2;

                case 'G':
                    if (string.Compare("GRD", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'I':
                    if (string.Compare("IEP", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("ITL", 0, str, 0, 3) == 0)
                        {
                            return 0;
                        }
                        break;
                    }
                    return 2;

                case 'L':
                    if ((string.Compare("LUX", 0, str, 0, 3) != 0) && (string.Compare("LUF", 0, str, 0, 3) != 0))
                    {
                        break;
                    }
                    return 0;

                case 'N':
                    if (string.Compare("NLG", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;

                case 'P':
                    if (string.Compare("PTE", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return 2;
            }
            return 2;
        }

        internal static double duration(DateTime nSettle, DateTime nMat, double fCoup, double fYield, int nFreq, int nBase, double fNumOfCoups)
        {
            double num2;
            double num = 0.0;
            double num3 = 0.0;
            fCoup *= 100.0 / ((double) nFreq);
            fYield /= (double) nFreq;
            fYield++;
            for (num2 = 1.0; num2 < fNumOfCoups; num2++)
            {
                num += (num2 * fCoup) / Math.Pow(fYield, num2);
            }
            num += (fNumOfCoups * (fCoup + 100.0)) / Math.Pow(fYield, fNumOfCoups);
            for (num2 = 1.0; num2 < fNumOfCoups; num2++)
            {
                num3 += fCoup / Math.Pow(fYield, num2);
            }
            num3 += (fCoup + 100.0) / Math.Pow(fYield, fNumOfCoups);
            num /= num3;
            return (num / ((double) nFreq));
        }

        internal static double GetRmz(double fZins, double fZzr, double fBw, double fZw, int nF)
        {
            double num;
            if (fZins == 0.0)
            {
                num = (fBw + fZw) / fZzr;
            }
            else
            {
                double num2 = Math.Pow(1.0 + fZins, fZzr);
                if (nF > 0)
                {
                    num = (((fZw * fZins) / (num2 - 1.0)) + ((fBw * fZins) / (1.0 - (1.0 / num2)))) / (1.0 + fZins);
                }
                else
                {
                    num = ((fZw * fZins) / (num2 - 1.0)) + ((fBw * fZins) / (1.0 - (1.0 / num2)));
                }
            }
            return -num;
        }

        internal static double GetZw(double fZins, double fZzr, double fRmz, double fBw, int nF)
        {
            double num;
            if (fZins == 0.0)
            {
                num = fBw + (fRmz * fZzr);
            }
            else
            {
                double num2 = Math.Pow(1.0 + fZins, fZzr);
                if (nF > 0)
                {
                    num = (fBw * num2) + (((fRmz * (1.0 + fZins)) * (num2 - 1.0)) / fZins);
                }
                else
                {
                    num = (fBw * num2) + ((fRmz * (num2 - 1.0)) / fZins);
                }
            }
            return -num;
        }

        internal static void goal_seek_initialise(ref GoalSeekData data)
        {
            data.havexpos = data.havexneg = false;
            data.xmin = -10000000000;
            data.xmax = 10000000000;
            data.precision = 1E-10;
        }

        internal static double one_euro(string str, int prec)
        {
            switch (str[0])
            {
                case 'A':
                    if (string.Compare("ATS", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return Math.Round((double) 13.7603, prec);

                case 'B':
                    if (string.Compare("BEF", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return Math.Round((double) 40.3399, prec);

                case 'D':
                    if (string.Compare("DEM", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return Math.Round((double) 1.95583, prec);

                case 'E':
                    if (string.Compare("ESP", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("EUR", 0, str, 0, 3) != 0)
                        {
                            break;
                        }
                        return Math.Round((double) 1.0, prec);
                    }
                    return Math.Round((double) 166.386, prec);

                case 'F':
                    if (string.Compare("FIM", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("FRF", 0, str, 0, 3) != 0)
                        {
                            break;
                        }
                        return Math.Round((double) 6.55957, prec);
                    }
                    return Math.Round((double) 5.94573, prec);

                case 'G':
                    if (string.Compare("GRD", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return Math.Round((double) 340.75, prec);

                case 'I':
                    if (string.Compare("IEP", 0, str, 0, 3) != 0)
                    {
                        if (string.Compare("ITL", 0, str, 0, 3) == 0)
                        {
                            return Math.Round((double) 1936.27, prec);
                        }
                        break;
                    }
                    return Math.Round((double) 0.787564, prec);

                case 'L':
                    if ((string.Compare("LUX", 0, str, 0, 3) != 0) && (string.Compare("LUF", 0, str, 0, 3) != 0))
                    {
                        break;
                    }
                    return Math.Round((double) 40.3399, prec);

                case 'N':
                    if (string.Compare("NLG", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return Math.Round((double) 2.20371, prec);

                case 'P':
                    if (string.Compare("PTE", 0, str, 0, 3) != 0)
                    {
                        break;
                    }
                    return Math.Round((double) 200.482, prec);
            }
            return -1.0;
        }

        internal static double price(DateTime settlement, DateTime maturity, double rate, double yield, double redemption, int freq, int basis)
        {
            double num = coupdaybs(settlement, maturity, freq, basis);
            double num2 = coupdaysnc(settlement, maturity, freq, basis);
            double num3 = coupdays(settlement, maturity, freq, basis);
            int num11 = coupnum(settlement, maturity, freq);
            double num4 = 0.0;
            double num5 = (100.0 * rate) / ((double) freq);
            double x = 1.0 + (yield / ((double) freq));
            double num7 = num2 / num3;
            for (int i = 0; i < num11; i++)
            {
                num4 += num5 / Math.Pow(x, num7 + i);
            }
            double num8 = redemption / Math.Pow(x, (num11 - 1.0) + (num2 / num3));
            double num9 = (num / num3) * num5;
            return ((num8 + num4) - num9);
        }

        internal static bool update_data(double x, double y, ref GoalSeekData data)
        {
            if (y > 0.0)
            {
                if (data.havexpos)
                {
                    if (data.havexneg)
                    {
                        if (Math.Abs((double) (x - data.xneg)) < Math.Abs((double) (data.xpos - data.xneg)))
                        {
                            data.xpos = x;
                            data.ypos = y;
                        }
                    }
                    else if (y < data.ypos)
                    {
                        data.xpos = x;
                        data.ypos = y;
                    }
                }
                else
                {
                    data.xpos = x;
                    data.ypos = y;
                    data.havexpos = true;
                }
                return false;
            }
            if (y < 0.0)
            {
                if (data.havexneg)
                {
                    if (data.havexpos)
                    {
                        if (Math.Abs((double) (x - data.xpos)) < Math.Abs((double) (data.xpos - data.xneg)))
                        {
                            data.xneg = x;
                            data.yneg = y;
                        }
                    }
                    else if (-y < -data.yneg)
                    {
                        data.xneg = x;
                        data.yneg = y;
                    }
                }
                else
                {
                    data.xneg = x;
                    data.yneg = y;
                    data.havexneg = true;
                }
                return false;
            }
            data.root = x;
            return true;
        }
    }
}

