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
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the <see cref="T:System.Double" /> equivalent number when converted to another measurement system.
    /// </summary>
    public class CalcConvertFunction : CalcBuiltinFunction
    {
        private static readonly string[] _fixPrefixesList = new string[] { "cup", "mmHg", "J", "sec", "cel", "kel", "hh", "Wh", "wh", "flb", "BTU" };
        internal static double atto = 1E-18;
        internal static double C_K_offset = 273.15;
        internal static double centi = 0.01;
        internal static double deci = 0.1;
        internal static double deka = 10.0;
        internal static eng_convert_unit_t[] distance_units = new eng_convert_unit_t[] { new eng_convert_unit_t("m", 1.0), new eng_convert_unit_t("mi", one_m_to_mi), new eng_convert_unit_t("Nmi", one_m_to_Nmi), new eng_convert_unit_t("in", one_m_to_in), new eng_convert_unit_t("ft", one_m_to_ft), new eng_convert_unit_t("yd", one_m_to_yd), new eng_convert_unit_t("ang", one_m_to_ang), new eng_convert_unit_t("Pica", one_m_to_Pica), new eng_convert_unit_t("km", one_m_to_km), new eng_convert_unit_t(null, 0.0) };
        internal static eng_convert_unit_t[] energy_units = new eng_convert_unit_t[] { new eng_convert_unit_t("J", 1.0), new eng_convert_unit_t("e", one_J_to_e), new eng_convert_unit_t("c", one_J_to_c), new eng_convert_unit_t("cal", one_J_to_cal), new eng_convert_unit_t("eV", one_J_to_eV), new eng_convert_unit_t("HPh", one_J_to_HPh), new eng_convert_unit_t("Wh", one_J_to_Wh), new eng_convert_unit_t("flb", one_J_to_flb), new eng_convert_unit_t("BTU", one_J_to_BTU), new eng_convert_unit_t("ev", one_J_to_eV), new eng_convert_unit_t("hh", one_J_to_HPh), new eng_convert_unit_t("wh", one_J_to_Wh), new eng_convert_unit_t("btu", one_J_to_BTU), new eng_convert_unit_t(null, 0.0) };
        internal static double exa = 1E+18;
        internal static double femto = 1E-15;
        internal static eng_convert_unit_t[] force_units = new eng_convert_unit_t[] { new eng_convert_unit_t("N", 1.0), new eng_convert_unit_t("dyn", one_N_to_dyn), new eng_convert_unit_t("lbf", one_N_to_lbf), new eng_convert_unit_t("dy", one_N_to_dyn), new eng_convert_unit_t(null, 0.0) };
        internal static double giga = 1000000000.0;
        internal static double hecto = 100.0;
        internal static double kilo = 1000.0;
        internal static eng_convert_unit_t[] liquid_units = new eng_convert_unit_t[] { new eng_convert_unit_t("tsp", 1.0), new eng_convert_unit_t("tbs", one_tsp_to_tbs), new eng_convert_unit_t("oz", one_tsp_to_oz), new eng_convert_unit_t("cup", one_tsp_to_cup), new eng_convert_unit_t("pt", one_tsp_to_pt), new eng_convert_unit_t("qt", one_tsp_to_qt), new eng_convert_unit_t("gal", one_tsp_to_gal), new eng_convert_unit_t("l", one_tsp_to_l), new eng_convert_unit_t("uk_pt", one_tsp_to_ukpt), new eng_convert_unit_t("us_pt", one_tsp_to_pt), new eng_convert_unit_t("lt", one_tsp_to_l), new eng_convert_unit_t(null, 0.0) };
        internal static eng_convert_unit_t[] magnetism_units = new eng_convert_unit_t[] { new eng_convert_unit_t("T", 1.0), new eng_convert_unit_t("ga", one_T_to_ga), new eng_convert_unit_t(null, 0.0) };
        internal static double mega = 1000000.0;
        internal static double micro = 1E-06;
        internal static double milli = 0.001;
        internal static double nano = 1E-09;
        internal static double one_g_to_lbm = 0.002204622915;
        internal static double one_g_to_ozm = 0.035273972;
        internal static double one_g_to_sg = 6.852205001E-05;
        internal static double one_g_to_u = 6.02217E+23;
        internal static double one_HP_to_W = 745.701;
        internal static double one_J_to_BTU = 0.000947815;
        internal static double one_J_to_c = 0.239006249;
        internal static double one_J_to_cal = 0.238846191;
        internal static double one_J_to_e = 9999995.193;
        internal static double one_J_to_eV = 6.2146E+18;
        internal static double one_J_to_flb = 23.73042222;
        internal static double one_J_to_HPh = (1.0 / (3600.0 * one_HP_to_W));
        internal static double one_J_to_Wh = 0.00027777777777777778;
        internal static double one_m_to_ang = 10000000000;
        internal static double one_m_to_ft = (one_m_to_in / 12.0);
        internal static double one_m_to_in = 39.370078740157481;
        internal static double one_m_to_km = 0.001;
        internal static double one_m_to_mi = (one_m_to_yd / 1760.0);
        internal static double one_m_to_Nmi = 0.00053995680345572358;
        internal static double one_m_to_Pica = 2834.645669;
        internal static double one_m_to_yd = (one_m_to_ft / 3.0);
        internal static double one_N_to_dyn = 100000.0;
        internal static double one_N_to_lbf = 0.224808924;
        internal static double one_Pa_to_atm = 9.869233E-06;
        internal static double one_Pa_to_mmHg = 0.00750061708;
        internal static double one_T_to_ga = 10000.0;
        internal static double one_tsp_to_cup = 0.020833333333333332;
        internal static double one_tsp_to_gal = 0.0013020833333333333;
        internal static double one_tsp_to_l = 0.004929994;
        internal static double one_tsp_to_oz = 0.16666666666666666;
        internal static double one_tsp_to_pt = 0.010416666666666666;
        internal static double one_tsp_to_qt = 0.005208333333333333;
        internal static double one_tsp_to_tbs = 0.33333333333333331;
        internal static double one_tsp_to_ukpt = 0.008675585;
        internal static double one_yr_to_day = 365.25;
        internal static double one_yr_to_hr = (24.0 * one_yr_to_day);
        internal static double one_yr_to_mn = (60.0 * one_yr_to_hr);
        internal static double one_yr_to_sec = (60.0 * one_yr_to_mn);
        internal static double peta = 1E+15;
        internal static double pico = 1E-12;
        internal static eng_convert_unit_t[] power_units = new eng_convert_unit_t[] { new eng_convert_unit_t("HP", 1.0), new eng_convert_unit_t("W", one_HP_to_W), new eng_convert_unit_t("h", 1.0), new eng_convert_unit_t("w", one_HP_to_W), new eng_convert_unit_t(null, 0.0) };
        internal static eng_convert_unit_t[] prefixes = new eng_convert_unit_t[] { 
            new eng_convert_unit_t("Y", yotta), new eng_convert_unit_t("Z", zetta), new eng_convert_unit_t("E", exa), new eng_convert_unit_t("P", peta), new eng_convert_unit_t("T", tera), new eng_convert_unit_t("G", giga), new eng_convert_unit_t("M", mega), new eng_convert_unit_t("k", kilo), new eng_convert_unit_t("h", hecto), new eng_convert_unit_t("e", deka), new eng_convert_unit_t("d", deci), new eng_convert_unit_t("c", centi), new eng_convert_unit_t("m", milli), new eng_convert_unit_t("u", micro), new eng_convert_unit_t("n", nano), new eng_convert_unit_t("p", pico), 
            new eng_convert_unit_t("f", femto), new eng_convert_unit_t("a", atto), new eng_convert_unit_t("z", zepto), new eng_convert_unit_t("y", yocto), new eng_convert_unit_t(null, 0.0)
         };
        internal static eng_convert_unit_t[] pressure_units = new eng_convert_unit_t[] { new eng_convert_unit_t("Pa", 1.0), new eng_convert_unit_t("atm", one_Pa_to_atm), new eng_convert_unit_t("mmHg", one_Pa_to_mmHg), new eng_convert_unit_t("p", 1.0), new eng_convert_unit_t("at", one_Pa_to_atm), new eng_convert_unit_t(null, 0.0) };
        internal static double tera = 1000000000000;
        internal static eng_convert_unit_t[] time_units = new eng_convert_unit_t[] { new eng_convert_unit_t("yr", 1.0), new eng_convert_unit_t("day", one_yr_to_day), new eng_convert_unit_t("hr", one_yr_to_hr), new eng_convert_unit_t("mn", one_yr_to_mn), new eng_convert_unit_t("sec", one_yr_to_sec), new eng_convert_unit_t(null, 0.0) };
        internal static eng_convert_unit_t[] weight_units = new eng_convert_unit_t[] { new eng_convert_unit_t("g", 1.0), new eng_convert_unit_t("sg", one_g_to_sg), new eng_convert_unit_t("lbm", one_g_to_lbm), new eng_convert_unit_t("u", one_g_to_u), new eng_convert_unit_t("ozm", one_g_to_ozm), new eng_convert_unit_t(null, 0.0) };
        internal static double yocto = 1E-24;
        internal static double yotta = 1E+24;
        internal static double zepto = 1E-21;
        internal static double zetta = 1E+21;

        private bool convert(eng_convert_unit_t[] units, eng_convert_unit_t[] prefixes, string from_unit, string to_unit, double n, ref double v)
        {
            double c = 0.0;
            double prefix = 0.0;
            double num4 = 0.0;
            double num3 = 0.0;
            if (!this.get_constant_of_unit(units, prefixes, from_unit, ref c, ref prefix))
            {
                return false;
            }
            if (!this.get_constant_of_unit(units, prefixes, to_unit, ref num3, ref num4))
            {
                return false;
            }
            if ((c == 0.0) || (num4 == 0.0))
            {
                return false;
            }
            v = (((n * prefix) / c) * num3) / num4;
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> equivalent number when converted to another measurement system.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: number, from_unit, to_unit.
        /// </para>
        /// <para>
        /// Number is the value in from_units to convert.
        /// </para>
        /// <para>
        /// From_unit is the units for number.
        /// </para>
        /// <para>
        /// To_unit is the units for the result.
        /// CONVERT accepts the following text values (in quotation marks) for from_unit and to_unit.
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
            string str = CalcConvert.ToString(args[1]);
            string str2 = CalcConvert.ToString(args[2]);
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str2))
            {
                if (((string.Compare(str, "C") == 0) || (string.Compare(str, "cel") == 0)) && ((string.Compare(str2, "C") == 0) || (string.Compare(str2, "cel") == 0)))
                {
                    return (double) num;
                }
                if (((string.Compare(str, "F") == 0) || (string.Compare(str, "fah") == 0)) && ((string.Compare(str2, "F") == 0) || (string.Compare(str2, "fah") == 0)))
                {
                    return (double) num;
                }
                if (((string.Compare(str, "K") == 0) || (string.Compare(str, "kel") == 0)) && ((string.Compare(str2, "K") == 0) || (string.Compare(str2, "kel") == 0)))
                {
                    return (double) num;
                }
                if (((string.Compare(str, "C") == 0) || (string.Compare(str, "cel") == 0)) && ((string.Compare(str2, "F") == 0) || (string.Compare(str2, "fah") == 0)))
                {
                    return (double) (((num * 9.0) / 5.0) + 32.0);
                }
                if (((string.Compare(str, "F") == 0) || (string.Compare(str, "fah") == 0)) && ((string.Compare(str2, "C") == 0) || (string.Compare(str2, "cel") == 0)))
                {
                    return (double) (((num - 32.0) * 5.0) / 9.0);
                }
                if (((string.Compare(str, "F") == 0) || (string.Compare(str, "fah") == 0)) && ((string.Compare(str2, "F") == 0) || (string.Compare(str2, "fah") == 0)))
                {
                    return (double) num;
                }
                if (((string.Compare(str, "F") == 0) || (string.Compare(str, "fah") == 0)) && ((string.Compare(str2, "K") == 0) || (string.Compare(str2, "kel") == 0)))
                {
                    return (double) ((((num - 32.0) * 5.0) / 9.0) + C_K_offset);
                }
                if (((string.Compare(str, "K") == 0) || (string.Compare(str, "kel") == 0)) && ((string.Compare(str2, "F") == 0) || (string.Compare(str2, "fah") == 0)))
                {
                    return (double) ((((num - C_K_offset) * 9.0) / 5.0) + 32.0);
                }
                if (((string.Compare(str, "C") == 0) || (string.Compare(str, "cel") == 0)) && ((string.Compare(str2, "K") == 0) || (string.Compare(str2, "kel") == 0)))
                {
                    return (double) (num + C_K_offset);
                }
                if (((string.Compare(str, "K") == 0) || (string.Compare(str, "kel") == 0)) && ((string.Compare(str2, "C") == 0) || (string.Compare(str2, "cel") == 0)))
                {
                    return (double) (num - C_K_offset);
                }
                double v = 0.0;
                if (this.convert(weight_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(distance_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(time_units, null, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(pressure_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(force_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(energy_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(power_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(magnetism_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(liquid_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
                if (this.convert(magnetism_units, prefixes, str, str2, num, ref v))
                {
                    return (double) v;
                }
            }
            return CalcErrors.NotAvailable;
        }

        private static bool FixPrefixes(string unitName)
        {
            foreach (string str in _fixPrefixesList)
            {
                if (string.Compare(unitName, str, StringComparison.CurrentCulture) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool get_constant_of_unit(eng_convert_unit_t[] units, eng_convert_unit_t[] prefixes, string unit_name, ref double c, ref double prefix)
        {
            int num;
            prefix = 1.0;
            for (num = 0; units[num].str != null; num++)
            {
                if (string.Compare(unit_name, units[num].str) == 0)
                {
                    c = units[num].c;
                    return true;
                }
            }
            int num2 = 0;
            if (prefixes != null)
            {
                for (num = 0; prefixes[num].str != null; num++)
                {
                    if ((string.Compare(unit_name, 0, prefixes[num].str, 0, 1, StringComparison.CurrentCultureIgnoreCase) == 0) && FixPrefixes(unit_name))
                    {
                        prefix = prefixes[num].c;
                        num2++;
                    }
                }
            }
            for (num = 0; units[num].str != null; num++)
            {
                if ((string.Compare(unit_name, 1, units[num].str, 0, units[num].str.Length) == 0) && FixPrefixes(unit_name))
                {
                    c = units[num].c;
                    return true;
                }
            }
            return false;
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
                return "CONVERT";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct eng_convert_unit_t
        {
            public string str;
            public double c;
            public eng_convert_unit_t(string str, double c)
            {
                this.str = str;
                this.c = c;
            }
        }
    }
}

