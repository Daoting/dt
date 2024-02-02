#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RegExpNamedMasks
    {
        #region 静态内容
        /// <summary>
        /// 
        /// </summary>
        public static CultureInfo DefaultCulture = CultureInfo.CurrentCulture;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Escape(string input)
        {
            string str = string.Empty;
            foreach (char ch in input)
            {
                str = str + '[';
                switch (ch)
                {
                    case '^':
                    case '\\':
                        str = str + '\\';
                        break;
                }
                str = str + ch + ']';
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStrings"></param>
        /// <param name="ignoreZeros"></param>
        /// <returns></returns>
        public static string Escape(string[] inputStrings, bool ignoreZeros)
        {
            bool flag = false;
            string str = string.Empty;
            foreach (string str2 in inputStrings)
            {
                if (str2.Length == 0)
                {
                    if (!ignoreZeros)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (str.Length != 0)
                    {
                        str = str + '|';
                    }
                    str = str + Escape(str2);
                }
            }
            if (str.Length > 0)
            {
                str = '(' + str + ')';
                if (flag)
                {
                    str = str + '?';
                }
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetAbbreviatedDayNames(CultureInfo culture)
        {
            return Escape(culture.DateTimeFormat.AbbreviatedDayNames, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetAbbreviatedMonthNames(CultureInfo culture)
        {
            return Escape(culture.DateTimeFormat.AbbreviatedMonthNames, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetAMDesignator(CultureInfo culture)
        {
            return Escape(culture.DateTimeFormat.AMDesignator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetCurrencyDecimalSeparator(CultureInfo culture)
        {
            return Escape(culture.NumberFormat.CurrencyDecimalSeparator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetCurrencyPattern(CultureInfo culture)
        {
            int currencyDecimalDigits = culture.NumberFormat.CurrencyDecimalDigits;
            if (currencyDecimalDigits <= 0)
            {
                return @"\d+";
            }
            return (@"\d*\R{CurrencyDecimalSeparator}\d{" + currencyDecimalDigits.ToString() + "}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetCurrencySymbol(CultureInfo culture)
        {
            return Escape(culture.NumberFormat.CurrencySymbol);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetDateSeparator(CultureInfo culture)
        {
            return ""; // Escape(DateTimeFormatHelper.GetDateSeparator(culture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetDayNames(CultureInfo culture)
        {
            return Escape(culture.DateTimeFormat.DayNames, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetMonthNames(CultureInfo culture)
        {
            return Escape(culture.DateTimeFormat.MonthNames, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskName"></param>
        /// <returns></returns>
        public static string GetNamedMask(string maskName)
        {
            return GetNamedMask(maskName, DefaultCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskName"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetNamedMask(string maskName, CultureInfo culture)
        {
            switch (maskName)
            {
                case "DateSeparator":
                    return GetDateSeparator(culture);

                case "TimeSeparator":
                    return GetTimeSeparator(culture);

                case "AbbreviatedDayNames":
                    return GetAbbreviatedDayNames(culture);

                case "AbbreviatedMonthNames":
                    return GetAbbreviatedMonthNames(culture);

                case "AMDesignator":
                    return GetAMDesignator(culture);

                case "DayNames":
                    return GetDayNames(culture);

                case "MonthNames":
                    return GetMonthNames(culture);

                case "PMDesignator":
                    return GetPMDesignator(culture);

                case "NumberDecimalSeparator":
                    return GetNumberDecimalSeparator(culture);

                case "CurrencyDecimalSeparator":
                    return GetCurrencyDecimalSeparator(culture);

                case "CurrencySymbol":
                    return GetCurrencySymbol(culture);

                case "NumberPattern":
                    return GetNumberPattern(culture);

                case "CurrencyPattern":
                    return GetCurrencyPattern(culture);
            }
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "IncorrectMask: unknown named mask '{0}'", new object[] { maskName }), "maskName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetNumberDecimalSeparator(CultureInfo culture)
        {
            return Escape(culture.NumberFormat.NumberDecimalSeparator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetNumberPattern(CultureInfo culture)
        {
            int numberDecimalDigits = culture.NumberFormat.NumberDecimalDigits;
            if (numberDecimalDigits <= 0)
            {
                return @"\d+";
            }
            return (@"\d*\R{NumberDecimalSeparator}\d{" + numberDecimalDigits.ToString() + "}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetPMDesignator(CultureInfo culture)
        {
            return Escape(culture.DateTimeFormat.PMDesignator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string GetTimeSeparator(CultureInfo culture)
        {
            return ""; // Escape(DateTimeFormatHelper.GetTimeSeparator(culture));
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        protected RegExpNamedMasks() { }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public static string AbbreviatedDayNames
        {
            get { return GetAbbreviatedDayNames(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string AbbreviatedMonthNames
        {
            get { return GetAbbreviatedMonthNames(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string AMDesignator
        {
            get { return GetAMDesignator(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CurrencyDecimalSeparator
        {
            get { return GetCurrencyDecimalSeparator(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CurrencyPattern
        {
            get { return GetCurrencyPattern(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CurrencySymbol
        {
            get { return GetCurrencySymbol(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DateSeparator
        {
            get { return GetDateSeparator(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DayNames
        {
            get { return GetDayNames(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string MonthNames
        {
            get { return GetMonthNames(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string NumberDecimalSeparator
        {
            get { return GetNumberDecimalSeparator(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string NumberPattern
        {
            get { return GetNumberPattern(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string PMDesignator
        {
            get { return GetPMDesignator(DefaultCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string TimeSeparator
        {
            get { return GetTimeSeparator(DefaultCulture); }
        }
        #endregion 
    }
}

