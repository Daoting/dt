#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    internal class FormatterColorHelper
    {
        static byte[,] Palette = new byte[,] { 
            { 0, 0, 0 }, { 0xff, 0xff, 0xff }, { 0xff, 0, 0 }, { 0, 0xff, 0 }, { 0, 0, 0xff }, { 0xff, 0xff, 0 }, { 0xff, 0, 0xff }, { 0, 0xff, 0xff }, { 0x80, 0, 0 }, { 0, 0x80, 0 }, { 0, 0, 0x80 }, { 0x80, 0x80, 0 }, { 0x80, 0, 0x80 }, { 0, 0x80, 0x80 }, { 0xc0, 0xc0, 0xc0 }, { 0x80, 0x80, 0x80 }, 
            { 0x99, 0x99, 0xff }, { 0x99, 0x33, 0x66 }, { 0xff, 0xff, 0xcc }, { 0xcc, 0xff, 0xff }, { 0x66, 0, 0x66 }, { 0xff, 0x80, 0x80 }, { 0, 0x66, 0xcc }, { 0xcc, 0xcc, 0xff }, { 0, 0, 0x80 }, { 0xff, 0, 0xff }, { 0xff, 0xff, 0 }, { 0, 0xff, 0xff }, { 0x80, 0, 0x80 }, { 0x80, 0, 0 }, { 0, 0x80, 0x80 }, { 0, 0, 0xff }, 
            { 0, 0xcc, 0xff }, { 0xcc, 0xff, 0xff }, { 0xcc, 0xff, 0xcc }, { 0xff, 0xff, 0x99 }, { 0x99, 0xcc, 0xff }, { 0xff, 0x99, 0xcc }, { 0xcc, 0x99, 0xff }, { 0xff, 0xcc, 0x99 }, { 0x33, 0x66, 0xff }, { 0x33, 0xcc, 0xcc }, { 0x99, 0xcc, 0 }, { 0xff, 0xcc, 0 }, { 0xff, 0x99, 0 }, { 0xff, 0x66, 0 }, { 0x66, 0x66, 0x99 }, { 150, 150, 150 }, 
            { 0, 0x33, 0x66 }, { 0x33, 0x99, 0x66 }, { 0, 0x33, 0 }, { 0x33, 0x33, 0 }, { 0x99, 0x33, 0 }, { 0x99, 0x33, 0x66 }, { 0x33, 0x33, 0x99 }, { 0x33, 0x33, 0x33 }
         };

        /// <summary>
        /// Converts from an Excel BIFF color index to an RGB value.
        /// </summary>
        /// <param name="index">Index from which to convert.</param>
        /// <returns>Returns the corresponding RGB value.</returns>
        public static Windows.UI.Color ColorFromIndex(int index)
        {
            return ColorFromIndex(index, 0xff, null);
        }

        /// <summary>
        /// Colors from index using the specified palette.
        /// </summary>
        /// <param name="index">Index from which to convert</param>
        /// <param name="alpha">The alpha</param>
        /// <param name="palette">Palette to use</param>
        /// <returns>Returns the color.</returns>
        static Windows.UI.Color ColorFromIndex(int index, byte alpha, byte[,] palette)
        {
            Windows.UI.Color color = new Windows.UI.Color();
            switch (index)
            {
                case 0:
                    return Windows.UI.Color.FromArgb(alpha, 0, 0, 0);

                case 1:
                    return Windows.UI.Color.FromArgb(alpha, 0xff, 0xff, 0xff);

                case 2:
                    return Windows.UI.Color.FromArgb(alpha, 0xff, 0, 0);

                case 3:
                    return Windows.UI.Color.FromArgb(alpha, 0, 0xff, 0);

                case 4:
                    return Windows.UI.Color.FromArgb(alpha, 0, 0, 0xff);

                case 5:
                    return Windows.UI.Color.FromArgb(alpha, 0xff, 0xff, 0);

                case 6:
                    return Windows.UI.Color.FromArgb(alpha, 0xff, 0, 0xff);

                case 7:
                    return Windows.UI.Color.FromArgb(alpha, 0, 0xff, 0xff);

                case 0x7fff:
                    return color;
            }
            if (index >= 0x40)
            {
                return color;
            }
            if (palette == null)
            {
                return Windows.UI.Color.FromArgb(alpha, Palette[index - 8, 0], Palette[index - 8, 1], Palette[index - 8, 2]);
            }
            return Windows.UI.Color.FromArgb(alpha, palette[index - 8, 0], palette[index - 8, 1], palette[index - 8, 2]);
        }

        /// <summary>
        /// From the name.
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns></returns>
        static Windows.UI.Color FromName(KnownColor color)
        {
            uint num = (uint) color;
            return Windows.UI.Color.FromArgb((byte) (num >> 0x18), (byte) (num >> 0x10), (byte) (num >> 8), (byte) num);
        }

        public static Windows.UI.Color FromName(string color)
        {
            KnownColor color2;
            try
            {
                color2 = (KnownColor) Enum.Parse((Type) typeof(KnownColor), color, true);
            }
            catch
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegalColorNameError);
            }
            return FromName(color2);
        }

        static Windows.UI.Color? FromNameFast(string color)
        {
            KnownColor color2;
            if (Enum.TryParse<KnownColor>(color, true, out color2))
            {
                return new Windows.UI.Color?(FromName(color2));
            }
            return null;
        }

        public static Windows.UI.Color? FromStringValue(string color)
        {
            if (color != null)
            {
                if (color.StartsWith(((char) DefaultTokens.Sharp).ToString()))
                {
                    try
                    {
                        int num = NumberHelper.ParseHexString(color.TrimStart(new char[] { DefaultTokens.Sharp }));
                        return new Windows.UI.Color?(Windows.UI.Color.FromArgb((byte) ((num & 0xff000000L) >> 0x18), (byte) ((num & 0xff0000) >> 0x10), (byte) ((num & 0xff00) >> 8), (byte) (num & 0xff)));
                    }
                    catch
                    {
                        return null;
                    }
                }
                Windows.UI.Color? nullable = FromNameFast(color);
                if (nullable.HasValue && nullable.HasValue)
                {
                    return new Windows.UI.Color?(nullable.Value);
                }
            }
            return null;
        }

        /// <summary>
        /// Lists the known colors.
        /// </summary>
        enum KnownColor : uint
        {
            AliceBlue = 0xfff0f8ff,
            AntiqueWhite = 0xfffaebd7,
            Aqua = 0xff00ffff,
            Aquamarine = 0xff7fffd4,
            Azure = 0xfff0ffff,
            Beige = 0xfff5f5dc,
            Bisque = 0xffffe4c4,
            Black = 0xff000000,
            BlanchedAlmond = 0xffffebcd,
            Blue = 0xff0000ff,
            BlueViolet = 0xff8a2be2,
            Brown = 0xffa52a2a,
            BurlyWood = 0xffdeb887,
            CadetBlue = 0xff5f9ea0,
            Chartreuse = 0xff7fff00,
            Chocolate = 0xffd2691e,
            Coral = 0xffff7f50,
            CornflowerBlue = 0xff6495ed,
            Cornsilk = 0xfffff8dc,
            Crimson = 0xffdc143c,
            Cyan = 0xff00ffff,
            DarkBlue = 0xff00008b,
            DarkCyan = 0xff008b8b,
            DarkGoldenrod = 0xffb8860b,
            DarkGray = 0xffa9a9a9,
            DarkGreen = 0xff006400,
            DarkKhaki = 0xffbdb76b,
            DarkMagenta = 0xff8b008b,
            DarkOliveGreen = 0xff556b2f,
            DarkOrange = 0xffff8c00,
            DarkOrchid = 0xff9932cc,
            DarkRed = 0xff8b0000,
            DarkSalmon = 0xffe9967a,
            DarkSeaGreen = 0xff8fbc8b,
            DarkSlateBlue = 0xff483d8b,
            DarkSlateGray = 0xff2f4f4f,
            DarkTurquoise = 0xff00ced1,
            DarkViolet = 0xff9400d3,
            DeepPink = 0xffff1493,
            DeepSkyBlue = 0xff00bfff,
            DimGray = 0xff696969,
            DodgerBlue = 0xff1e90ff,
            Firebrick = 0xffb22222,
            FloralWhite = 0xfffffaf0,
            ForestGreen = 0xff228b22,
            Fuchsia = 0xffff00ff,
            Gainsboro = 0xffdcdcdc,
            GhostWhite = 0xfff8f8ff,
            Gold = 0xffffd700,
            Goldenrod = 0xffdaa520,
            Gray = 0xff808080,
            Green = 0xff008000,
            GreenYellow = 0xffadff2f,
            Honeydew = 0xfff0fff0,
            HotPink = 0xffff69b4,
            IndianRed = 0xffcd5c5c,
            Indigo = 0xff4b0082,
            Ivory = 0xfffffff0,
            Khaki = 0xfff0e68c,
            Lavender = 0xffe6e6fa,
            LavenderBlush = 0xfffff0f5,
            LawnGreen = 0xff7cfc00,
            LemonChiffon = 0xfffffacd,
            LightBlue = 0xffadd8e6,
            LightCoral = 0xfff08080,
            LightCyan = 0xffe0ffff,
            LightGoldenrodYellow = 0xfffafad2,
            LightGray = 0xffd3d3d3,
            LightGreen = 0xff90ee90,
            LightPink = 0xffffb6c1,
            LightSalmon = 0xffffa07a,
            LightSeaGreen = 0xff20b2aa,
            LightSkyBlue = 0xff87cefa,
            LightSlateGray = 0xff778899,
            LightSteelBlue = 0xffb0c4de,
            LightYellow = 0xffffffe0,
            Lime = 0xff00ff00,
            LimeGreen = 0xff32cd32,
            Linen = 0xfffaf0e6,
            Magenta = 0xffff00ff,
            Maroon = 0xff800000,
            MediumAquamarine = 0xff66cdaa,
            MediumBlue = 0xff0000cd,
            MediumOrchid = 0xffba55d3,
            MediumPurple = 0xff9370db,
            MediumSeaGreen = 0xff3cb371,
            MediumSlateBlue = 0xff7b68ee,
            MediumSpringGreen = 0xff00fa9a,
            MediumTurquoise = 0xff48d1cc,
            MediumVioletRed = 0xffc71585,
            MidnightBlue = 0xff191970,
            MintCream = 0xfff5fffa,
            MistyRose = 0xffffe4e1,
            Moccasin = 0xffffe4b5,
            NavajoWhite = 0xffffdead,
            Navy = 0xff000080,
            OldLace = 0xfffdf5e6,
            Olive = 0xff808000,
            OliveDrab = 0xff6b8e23,
            Orange = 0xffffa500,
            OrangeRed = 0xffff4500,
            Orchid = 0xffda70d6,
            PaleGoldenrod = 0xffeee8aa,
            PaleGreen = 0xff98fb98,
            PaleTurquoise = 0xffafeeee,
            PaleVioletRed = 0xffdb7093,
            PapayaWhip = 0xffffefd5,
            PeachPuff = 0xffffdab9,
            Peru = 0xffcd853f,
            Pink = 0xffffc0cb,
            Plum = 0xffdda0dd,
            PowderBlue = 0xffb0e0e6,
            Purple = 0xff800080,
            Red = 0xffff0000,
            RosyBrown = 0xffbc8f8f,
            RoyalBlue = 0xff4169e1,
            SaddleBrown = 0xff8b4513,
            Salmon = 0xfffa8072,
            SandyBrown = 0xfff4a460,
            SeaGreen = 0xff2e8b57,
            SeaShell = 0xfffff5ee,
            Sienna = 0xffa0522d,
            Silver = 0xffc0c0c0,
            SkyBlue = 0xff87ceeb,
            SlateBlue = 0xff6a5acd,
            SlateGray = 0xff708090,
            Snow = 0xfffffafa,
            SpringGreen = 0xff00ff7f,
            SteelBlue = 0xff4682b4,
            Tan = 0xffd2b48c,
            Teal = 0xff008080,
            Thistle = 0xffd8bfd8,
            Tomato = 0xffff6347,
            Transparent = 0xffffff,
            Turquoise = 0xff40e0d0,
            Violet = 0xffee82ee,
            Wheat = 0xfff5deb3,
            White = 0xffffffff,
            WhiteSmoke = 0xfff5f5f5,
            Window = 0xffffffff,
            WindowText = 0xff000000,
            Yellow = 0xffffff00,
            YellowGreen = 0xff9acd32
        }
    }
}

