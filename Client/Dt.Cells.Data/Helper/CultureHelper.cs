#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// The CultureHelper Class 
    /// </summary>
    internal static class CultureHelper
    {
        static readonly string ChineseHongKongSarPRC = "zh-HK";
        static readonly string ChineseMacaoSAR = "zh-MO";
        static readonly string ChinesePRC = "zh-CN";
        static readonly string ChineseSimplified = "zh-Hans";
        static readonly string ChineseSingapore = "zh-SG";
        static readonly string ChineseTaiwan = "zh-TW";
        static readonly string ChineseTraditional = "zh-Hant";
        static readonly string EnglishCanada = "en-CA";
        static readonly string EnglishCaribbean = "en-029";
        static readonly string EnglishIreland = "en-IE";
        static readonly string EnglishJamaica = "en-JM";
        static readonly string EnglishNewZealand = "en-NZ";
        static readonly string EnglishPhilippines = "en-PH";
        static readonly string EnglishSouthAfrica = "en-ZA";
        static readonly string EnglishTrinidadandTobago = "en-TT";
        static readonly string EnglishUnitedKingdom = "en-GB";
        static readonly string EnglishUnitedStates = "en-US";
        static readonly string EnglishZimbabwe = "en-ZW";
        static readonly string Japanese = "ja";
        static readonly string JapaneseJapan = "ja-JP";

        public static bool AllowScience(CultureInfo culture)
        {
            return (((culture != null) && !culture.Name.StartsWith("zh")) && !culture.Name.StartsWith("ja"));
        }

        /// <summary>
        /// Creates the calendar.
        /// </summary>
        /// <param name="cultureID">The culture ID.</param>
        /// <returns>Returns the calendar.</returns>
        public static Calendar CreateCalendar(int cultureID)
        {
            int num = cultureID & 0xff;
            if (num == 0x11)
            {
                return new JapaneseCalendar();
            }
            return null;
        }

        /// <summary>
        /// Creates the calendar.
        /// </summary>
        /// <param name="cultureID">The culture ID.</param>
        /// <returns>Returns the calendar.</returns>
        public static Calendar CreateCalendar(string cultureID)
        {
            try
            {
                return CreateCalendar(NumberHelper.ParseHexString(cultureID));
            }
            catch
            {
                return new GregorianCalendar();
            }
        }

        /// <summary>
        /// Creates the culture information.
        /// </summary>
        /// <param name="cultureID">The culture ID.</param>
        /// <returns>Returns the culture information.</returns>
        public static CultureInfo CreateCultureInfo(int cultureID)
        {
            switch (cultureID)
            {
                case 0x404:
                    return new CultureInfo(ChineseTaiwan);

                case 0x409:
                    return new CultureInfo(EnglishUnitedStates);

                case 0x411:
                    return new CultureInfo(JapaneseJapan);

                case 4:
                    return new CultureInfo(ChineseSimplified);

                case 0x11:
                    return new CultureInfo(Japanese);

                case 0x804:
                    return new CultureInfo(ChinesePRC);

                case 0x809:
                    return new CultureInfo(EnglishUnitedKingdom);

                case 0xc04:
                    return new CultureInfo(ChineseHongKongSarPRC);

                case 0x1004:
                    return new CultureInfo(ChineseSingapore);

                case 0x1009:
                    return new CultureInfo(EnglishCanada);

                case 0x1809:
                    return new CultureInfo(EnglishIreland);

                case 0x1c09:
                    return new CultureInfo(EnglishSouthAfrica);

                case 0x2009:
                    return new CultureInfo(EnglishJamaica);

                case 0x1404:
                    return new CultureInfo(ChineseMacaoSAR);

                case 0x1409:
                    return new CultureInfo(EnglishNewZealand);

                case 0x2409:
                    return new CultureInfo(EnglishCaribbean);

                case 0x2c09:
                    return new CultureInfo(EnglishTrinidadandTobago);

                case 0x3009:
                    return new CultureInfo(EnglishZimbabwe);

                case 0x3409:
                    return new CultureInfo(EnglishPhilippines);

                case 0x7c04:
                    return new CultureInfo(ChineseTraditional);
            }
            return (CultureInfo.CurrentCulture.Clone() as CultureInfo);
        }

        /// <summary>
        /// Creates the culture information.
        /// </summary>
        /// <param name="cultureID">The culture ID.</param>
        /// <returns>Returns the culture information.</returns>
        public static CultureInfo CreateCultureInfo(string cultureID)
        {
            try
            {
                return CreateCultureInfo(NumberHelper.ParseHexString(cultureID));
            }
            catch
            {
                return CultureInfo.CurrentCulture;
            }
        }
    }
}

