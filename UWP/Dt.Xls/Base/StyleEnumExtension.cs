#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    internal static class StyleEnumExtension
    {
        public static ColorSchemeIndex ToColorSchmeIndex(this string val)
        {
            if (val == "ACCENT1")
            {
                return ColorSchemeIndex.Accent1;
            }
            if (val == "ACCENT2")
            {
                return ColorSchemeIndex.Accent2;
            }
            if (val == "ACCENT3")
            {
                return ColorSchemeIndex.Accent3;
            }
            if (val == "ACCENT4")
            {
                return ColorSchemeIndex.Accent4;
            }
            if (val == "ACCENT5")
            {
                return ColorSchemeIndex.Accent5;
            }
            if (val == "ACCENT6")
            {
                return ColorSchemeIndex.Accent6;
            }
            if (val == "DK1")
            {
                return ColorSchemeIndex.TextDark1;
            }
            if (val == "DK2")
            {
                return ColorSchemeIndex.TextDark2;
            }
            if (val == "LT1")
            {
                return ColorSchemeIndex.TextLight1;
            }
            if (val == "LT2")
            {
                return ColorSchemeIndex.TextLight2;
            }
            if (val == "HLIK")
            {
                return ColorSchemeIndex.Hyperlink;
            }
            if (val == "FOLHLINK")
            {
                return ColorSchemeIndex.FollowedHyperlink;
            }
            if (val == "PHCLR")
            {
                return ColorSchemeIndex._PlaceHolderColor;
            }
            if (val == "BG1")
            {
                return ColorSchemeIndex._BackgroundColor1;
            }
            if (val == "BG2")
            {
                return ColorSchemeIndex._BackgroudnColor2;
            }
            if (val == "TX1")
            {
                return ColorSchemeIndex._TextColor1;
            }
            if (val == "TX2")
            {
                return ColorSchemeIndex._TextColor2;
            }
            return ColorSchemeIndex.None;
        }

        public static FontLanguage ToFontLanguage(this string value)
        {
            if (value != "LATIN")
            {
                if (value == "CS")
                {
                    return FontLanguage.ComplexScriptFont;
                }
                if (value == "EA")
                {
                    return FontLanguage.EastAsianFont;
                }
                if (value == "SYM")
                {
                    return FontLanguage.SymbolFont;
                }
            }
            return FontLanguage.LatinFont;
        }

        public static string ToRunFormattingValue(this FontLanguage fontLanguage)
        {
            switch (fontLanguage)
            {
                case FontLanguage.LatinFont:
                    return "latin";

                case FontLanguage.ComplexScriptFont:
                    return "cs";

                case FontLanguage.EastAsianFont:
                    return "ea";

                case FontLanguage.SymbolFont:
                    return "sym";
            }
            return "";
        }

        public static string ToSchemeClrValue(this ColorSchemeIndex scheme)
        {
            ColorSchemeIndex index = scheme;
            switch (index)
            {
                case ColorSchemeIndex.TextLight1:
                    return "lt1";

                case ColorSchemeIndex.TextDark1:
                    return "dk1";

                case ColorSchemeIndex.TextLight2:
                    return "lt2";

                case ColorSchemeIndex.TextDark2:
                    return "dk2";

                case ColorSchemeIndex.Accent1:
                    return "accent1";

                case ColorSchemeIndex.Accent2:
                    return "accent2";

                case ColorSchemeIndex.Accent3:
                    return "accent3";

                case ColorSchemeIndex.Accent4:
                    return "accent4";

                case ColorSchemeIndex.Accent5:
                    return "accent5";

                case ColorSchemeIndex.Accent6:
                    return "accent6";

                case ColorSchemeIndex.Hyperlink:
                    return "hlink";

                case ColorSchemeIndex.FollowedHyperlink:
                    return "folHlink";

                case ColorSchemeIndex._PlaceHolderColor:
                    return "phClr";

                case ColorSchemeIndex._TextColor1:
                    return "tx1";

                case ColorSchemeIndex._TextColor2:
                    return "tx2";

                case ColorSchemeIndex._BackgroundColor1:
                    return "bg1";

                case ColorSchemeIndex._BackgroudnColor2:
                    return "bg2";
            }
            if (index != ColorSchemeIndex.None)
            {
                return "";
            }
            return "";
        }
    }
}

