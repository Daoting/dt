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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    internal static class ColorExtension
    {
        private static Dictionary<int, uint> _palette = new Dictionary<int, uint>();
        private static Dictionary<string, uint> _preSetColors = new Dictionary<string, uint>();

        static ColorExtension()
        {
            _preSetColors.Add("aliceBlue", 0xfff0f8ff);
            _preSetColors.Add("antiqueWhite", 0xfffaebd7);
            _preSetColors.Add("aqua", 0xff0ffff);
            _preSetColors.Add("aquamarine", 0xff7fffd4);
            _preSetColors.Add("azure", 0xfff0ffff);
            _preSetColors.Add("beige", 0xfff5f5dc);
            _preSetColors.Add("bisque", 0xffffe4c4);
            _preSetColors.Add("black", 0xff000);
            _preSetColors.Add("blanchedAlmond", 0xffffebcd);
            _preSetColors.Add("blue", 0xff00ff);
            _preSetColors.Add("blueViolet", 0xff8a2be2);
            _preSetColors.Add("brown", 0xffa52a2a);
            _preSetColors.Add("burlyWood", 0xffdeb887);
            _preSetColors.Add("cadetBlue", 0xff5f9ea0);
            _preSetColors.Add("chartreuse", 0xff7fff0);
            _preSetColors.Add("chocolate", 0xffd2691e);
            _preSetColors.Add("coral", 0xffff7f50);
            _preSetColors.Add("cornflowerBlue", 0xff6495ed);
            _preSetColors.Add("cornsilk", 0xfffff8dc);
            _preSetColors.Add("crimson", 0xffdc143c);
            _preSetColors.Add("cyan", 0xff0ffff);
            _preSetColors.Add("darkBlue", 0xff008b);
            _preSetColors.Add("darkCyan", 0xff08b8b);
            _preSetColors.Add("darkGoldenrod", 0xffb886b);
            _preSetColors.Add("darkGray", 0xffa9a9a9);
            _preSetColors.Add("darkGreen", 0xff0640);
            _preSetColors.Add("darkGrey", 0xffa9a9a9);
            _preSetColors.Add("darkKhaki", 0xffbdb76b);
            _preSetColors.Add("darkMagenta", 0xff8b08b);
            _preSetColors.Add("darkOliveGreen", 0xff556b2f);
            _preSetColors.Add("darkOrange", 0xffff8c0);
            _preSetColors.Add("darkOrchid", 0xff9932cc);
            _preSetColors.Add("darkRed", 0xff8b00);
            _preSetColors.Add("darkSalmon", 0xffe9967a);
            _preSetColors.Add("darkSeaGreen", 0xff8fbc8f);
            _preSetColors.Add("darkSlateBlue", 0xff483d8b);
            _preSetColors.Add("darkSlateGray", 0xff2f4f4f);
            _preSetColors.Add("darkSlateGrey", 0xff2f4f4f);
            _preSetColors.Add("darkTurquoise", 0xff0ced1);
            _preSetColors.Add("darkViolet", 0xff940d3);
            _preSetColors.Add("deepPink", 0xffff1493);
            _preSetColors.Add("deepSkyBlue", 0xff0bfff);
            _preSetColors.Add("dimGray", 0xff696969);
            _preSetColors.Add("dimGrey", 0xff696969);
            _preSetColors.Add("dkBlue", 0xff008b);
            _preSetColors.Add("dkCyan", 0xff08b8b);
            _preSetColors.Add("dkGoldenrod", 0xffb886b);
            _preSetColors.Add("dkGray", 0xffa9a9a9);
            _preSetColors.Add("dkGreen", 0xff0640);
            _preSetColors.Add("dkGrey", 0xffa9a9a9);
            _preSetColors.Add("dkKhaki", 0xffbdb76b);
            _preSetColors.Add("dkMagenta", 0xff8b08b);
            _preSetColors.Add("dkOliveGreen", 0xff556b2f);
            _preSetColors.Add("dkOrange", 0xffff8c0);
            _preSetColors.Add("dkOrchid", 0xff9932cc);
            _preSetColors.Add("dkRed", 0xff8b00);
            _preSetColors.Add("dkSalmon", 0xffe9967a);
            _preSetColors.Add("dkSeaGreen", 0xff8fbc8b);
            _preSetColors.Add("dkSlateBlue", 0xff483d8b);
            _preSetColors.Add("dkSlateGray", 0xff2f4f4f);
            _preSetColors.Add("dkSlateGrey", 0xff2f4f4f);
            _preSetColors.Add("dkTurquoise", 0xff0ced1);
            _preSetColors.Add("dkViolet", 0xff940d3);
            _preSetColors.Add("dodgerBlue", 0xff1e90ff);
            _preSetColors.Add("firebrick", 0xffb22222);
            _preSetColors.Add("floralWhite", 0xfffffaf0);
            _preSetColors.Add("forestGreen", 0xff228b22);
            _preSetColors.Add("fuchsia", 0xffff0ff);
            _preSetColors.Add("gainsboro", 0xffdcdcdc);
            _preSetColors.Add("ghostWhite", 0xfff8f8ff);
            _preSetColors.Add("gold", 0xffffd70);
            _preSetColors.Add("goldenrod", 0xffdaa520);
            _preSetColors.Add("gray", 0xff808080);
            _preSetColors.Add("green", 0xff0800);
            _preSetColors.Add("greenYellow", 0xffadff2f);
            _preSetColors.Add("grey", 0xff808080);
            _preSetColors.Add("honeydew", 0xfff0fff0);
            _preSetColors.Add("hotPink", 0xffff69b4);
            _preSetColors.Add("indianRed", 0xffcd5c5c);
            _preSetColors.Add("indigo", 0xff4b082);
            _preSetColors.Add("ivory", 0xfffffff0);
            _preSetColors.Add("khaki", 0xfff0e68c);
            _preSetColors.Add("lavender", 0xffe6e6fa);
            _preSetColors.Add("lavenderBlush", 0xfffff0f5);
            _preSetColors.Add("lawnGreen", 0xff7cfc0);
            _preSetColors.Add("lemonChiffon", 0xfffffacd);
            _preSetColors.Add("lightBlue", 0xffadd8e6);
            _preSetColors.Add("lightCoral", 0xfff08080);
            _preSetColors.Add("lightCyan", 0xffe0ffff);
            _preSetColors.Add("lightGoldenrodYellow", 0xfffafad2);
            _preSetColors.Add("lightGray", 0xffd3d3d3);
            _preSetColors.Add("lightGreen", 0xff90ee90);
            _preSetColors.Add("lightGrey", 0xffd3d3d3);
            _preSetColors.Add("lightPink", 0xffffb6c1);
            _preSetColors.Add("lightSalmon", 0xffffa07a);
            _preSetColors.Add("lightSeaGreen", 0xff20b2aa);
            _preSetColors.Add("lightSkyBlue", 0xff87cefa);
            _preSetColors.Add("lightSlateGray", 0xff778899);
            _preSetColors.Add("lightSlateGrey", 0xff778899);
            _preSetColors.Add("lightSteelBlue", 0xffb0c4de);
            _preSetColors.Add("lightYellow", 0xffffffe0);
            _preSetColors.Add("lime", 0xff0ff0);
            _preSetColors.Add("limeGreen", 0xff32cd32);
            _preSetColors.Add("linen", 0xfffaf0e6);
            _preSetColors.Add("ltBlue", 0xffadd8e6);
            _preSetColors.Add("ltCoral", 0xfff08080);
            _preSetColors.Add("ltCyan", 0xffe0ffff);
            _preSetColors.Add("ltGoldenrodYellow", 0xfffafa78);
            _preSetColors.Add("ltGray", 0xffd3d3d3);
            _preSetColors.Add("ltGreen", 0xff90ee90);
            _preSetColors.Add("ltGrey", 0xffd3d3d3);
            _preSetColors.Add("ltPink", 0xffffb6c1);
            _preSetColors.Add("ltSalmon", 0xffffa07a);
            _preSetColors.Add("ltSeaGreen", 0xff20b2aa);
            _preSetColors.Add("ltSkyBlue", 0xff87cefa);
            _preSetColors.Add("ltSlateGray", 0xff778899);
            _preSetColors.Add("ltSlateGrey", 0xff778899);
            _preSetColors.Add("ltSteelBlue", 0xffb0c4de);
            _preSetColors.Add("ltYellow", 0xffffffe0);
            _preSetColors.Add("magenta", 0xffff0ff);
            _preSetColors.Add("maroon", 0xff8000);
            _preSetColors.Add("medAquamarine", 0xff66cdaa);
            _preSetColors.Add("medBlue", 0xff00cd);
            _preSetColors.Add("mediumAquamarine", 0xff66cdaa);
            _preSetColors.Add("mediumBlue", 0xff00cd);
            _preSetColors.Add("mediumOrchid", 0xffba55d3);
            _preSetColors.Add("mediumPurple", 0xff9370db);
            _preSetColors.Add("mediumSeaGreen", 0xff3cb371);
            _preSetColors.Add("mediumSlateBlue", 0xff7b68ee);
            _preSetColors.Add("mediumSpringGreen", 0xff0fa9a);
            _preSetColors.Add("mediumTurquoise", 0xff48d1cc);
            _preSetColors.Add("mediumVioletRed", 0xffc71585);
            _preSetColors.Add("medOrchid", 0xffba55d3);
            _preSetColors.Add("medPurple", 0xff9370db);
            _preSetColors.Add("medSeaGreen", 0xff3cb371);
            _preSetColors.Add("medSlateBlue", 0xff7b68ee);
            _preSetColors.Add("medSpringGreen", 0xff0fa9a);
            _preSetColors.Add("medTurquoise", 0xff48d1cc);
            _preSetColors.Add("medVioletRed", 0xffc71585);
            _preSetColors.Add("midnightBlue", 0xff191970);
            _preSetColors.Add("mintCream", 0xfff5fffa);
            _preSetColors.Add("mistyRose", 0xffffe4e1);
            _preSetColors.Add("moccasin", 0xffffe4b5);
            _preSetColors.Add("navajoWhite", 0xffffdead);
            _preSetColors.Add("navy", 0xff0080);
            _preSetColors.Add("oldLace", 0xfffdf5e6);
            _preSetColors.Add("olive", 0xff80800);
            _preSetColors.Add("oliveDrab", 0xff6b8e23);
            _preSetColors.Add("orange", 0xffffa50);
            _preSetColors.Add("orangeRed", 0xffff450);
            _preSetColors.Add("orchid", 0xffda70d6);
            _preSetColors.Add("paleGoldenrod", 0xffeee8aa);
            _preSetColors.Add("paleGreen", 0xff98fb98);
            _preSetColors.Add("paleTurquoise", 0xffafeeee);
            _preSetColors.Add("paleVioletRed", 0xffdb7093);
            _preSetColors.Add("papayaWhip", 0xffffefd5);
            _preSetColors.Add("peachPuff", 0xffffdab9);
            _preSetColors.Add("peru", 0xffcd853f);
            _preSetColors.Add("pink", 0xffffc0cb);
            _preSetColors.Add("plum", 0xffdda0dd);
            _preSetColors.Add("powderBlue", 0xffb0e0e6);
            _preSetColors.Add("purple", 0xff80080);
            _preSetColors.Add("red", 0xffff00);
            _preSetColors.Add("rosyBrown", 0xffbc8f8f);
            _preSetColors.Add("royalBlue", 0xff4169e1);
            _preSetColors.Add("saddleBrown", 0xff8b4513);
            _preSetColors.Add("salmon", 0xfffa8072);
            _preSetColors.Add("sandyBrown", 0xfff4a460);
            _preSetColors.Add("seaGreen", 0xff2e8b57);
            _preSetColors.Add("seaShell", 0xfffff5ee);
            _preSetColors.Add("sienna", 0xffa0522d);
            _preSetColors.Add("silver", 0xffc0c0c0);
            _preSetColors.Add("skyBlue", 0xff87ceeb);
            _preSetColors.Add("slateBlue", 0xff6a5acd);
            _preSetColors.Add("slateGray", 0xff708090);
            _preSetColors.Add("slateGrey", 0xff708090);
            _preSetColors.Add("snow", 0xfffffafa);
            _preSetColors.Add("springGreen", 0xff0ff7f);
            _preSetColors.Add("steelBlue", 0xff4682b4);
            _preSetColors.Add("tan", 0xffd2b48c);
            _preSetColors.Add("teal", 0xff08080);
            _preSetColors.Add("thistle", 0xffd8bfd8);
            _preSetColors.Add("tomato", 0xffff6347);
            _preSetColors.Add("turquoise", 0xff40e0d0);
            _preSetColors.Add("violet", 0xffee82ee);
            _preSetColors.Add("wheat", 0xfff5deb3);
            _preSetColors.Add("white", uint.MaxValue);
            _preSetColors.Add("whiteSmoke", 0xfff5f5f5);
            _preSetColors.Add("yellow", 0xffffff0);
            _preSetColors.Add("yellowGreen", 0xff9acd32);
        }

        internal static GcColor ApplyTintToColor(GcColor baseColor, double tint)
        {
            if (tint == 0.0)
            {
                return baseColor;
            }
            HLSColor color = new HLSColor(baseColor);
            if (tint > 0.0)
            {
                int num = 240 - color.Luminosity;
                double num2 = num * tint;
                return HLSColor.ColorFromHLS(color.Hue, color.Luminosity + ((int) num2), color.Saturation);
            }
            double num3 = color.Luminosity * -tint;
            return HLSColor.ColorFromHLS(color.Hue, color.Luminosity - ((int) num3), color.Saturation);
        }

        internal static GcColor ConvertHLSToRGB(double hue, double luminance, double saturation)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            if (saturation == 0.0)
            {
                num = (int)((luminance * 255.0) / 255.0);
                num2 = num;
                num3 = num;
            }
            else
            {
                double num4;
                if (luminance <= 127.0)
                {
                    num4 = ((luminance * (255.0 + saturation)) + 127.0) / 255.0;
                }
                else
                {
                    num4 = (luminance + saturation) - (((luminance * saturation) + 127.0) / 255.0);
                }
                double num5 = (2.0 * luminance) - num4;
                num = (int)(((HueToRGB(num5, num4, hue + 85.0) * 255.0) + 127.0) / 255.0);
                num2 = (int)(((HueToRGB(num5, num4, hue) * 255.0) + 127.0) / 255.0);
                num3 = (int)(((HueToRGB(num5, num4, hue - 85.0) * 255.0) + 127.0) / 255.0);
            }
            num = (num > 0) ? num : 0;
            num2 = (num2 > 0) ? num2 : 0;
            num3 = (num3 > 0) ? num3 : 0;
            num = (num < 0xff) ? num : 0xff;
            num2 = (num2 < 0xff) ? num2 : 0xff;
            num3 = (num3 > 0xff) ? num3 : 0xff;
            return GcColor.FromArgb(0, (byte) num, (byte) num2, (byte) num3);
        }

        internal static GcColor FromArgb(uint value)
        {
            return GcColor.FromArgb((byte) ((value >> 0x18) & 0xff), (byte) ((value >> 0x10) & 0xff), (byte) ((value >> 8) & 0xff), (byte) (value & 0xff));
        }

        internal static GcColor FromPresetColorVal(string preSetValue)
        {
            if (_preSetColors.ContainsKey(preSetValue))
            {
                return FromArgb(_preSetColors[preSetValue]);
            }
            return new GcColor();
        }

        internal static ExcelPaletteColor GetCloestColorIndex(GcColor color)
        {
            int num = -1;
            double maxValue = double.MaxValue;
            for (int i = 8; i < 0x40; i++)
            {
                uint num4 = DefaultPalette[i];
                int num5 = (int)((num4 & 0xff0000) >> 0x10);
                int num6 = (int)((num4 & 0xff00) >> 8);
                int num7 = (int) (num4 & 0xff);
                double num8 = (Math.Abs((double) ((num5 - color.R) * 0.3)) + Math.Abs((double) ((num6 - color.G) * 0.59))) + Math.Abs((double) ((num7 - color.B) * 0.11));
                if (num8 < maxValue)
                {
                    maxValue = num8;
                    num = i;
                }
            }
            return (ExcelPaletteColor) num;
        }

        internal static GcColor GetPaletteColor(int index)
        {
            if (index == 0x7fff)
            {
                return new GcColor();
            }
            return FromArgb(DefaultPalette[index]);
        }

        internal static double HueToRGB(double n1, double n2, double hue)
        {
            if (hue < 0.0)
            {
                hue += 255.0;
            }
            if (hue > 255.0)
            {
                hue -= 255.0;
            }
            if (hue < 42.0)
            {
                return (n1 + ((((n2 - n1) * hue) + 21.0) / 42.0));
            }
            if (hue < 127.0)
            {
                return n2;
            }
            if (hue < 170.0)
            {
                return (n1 + ((((n2 - n1) * (170.0 - hue)) + 21.0) / 42.0));
            }
            return n1;
        }

        internal static byte ScRgbTosRgb(float val)
        {
            if (val <= 0.0)
            {
                return 0;
            }
            if (val <= 0.0031308)
            {
                return (byte) (((255f * val) * 12.92f) + 0.5f);
            }
            if (val < 1.0)
            {
                return (byte) ((255f * ((1.055f * ((float) Math.Pow((double) val, 0.41666666666666669))) - 0.055f)) + 0.5f);
            }
            return 0xff;
        }

        internal static uint ToArgb(this GcColor color)
        {
            return uint.Parse(((byte) color.A).ToString("X2") + ((byte) color.R).ToString("X2") + ((byte) color.G).ToString("X2") + ((byte) color.B).ToString("X2"), (NumberStyles) NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
        }

        public static Dictionary<int, uint> DefaultPalette
        {
            get
            {
                if (_palette.Count == 0)
                {
                    _palette.Add(0, 0);
                    _palette.Add(1, 0xffffff);
                    _palette.Add(2, 0xff0000);
                    _palette.Add(3, 0xff00);
                    _palette.Add(4, 0xff);
                    _palette.Add(5, 0xffff00);
                    _palette.Add(6, 0xff00ff);
                    _palette.Add(7, 0xffff);
                    _palette.Add(8, 0);
                    _palette.Add(9, 0xffffff);
                    _palette.Add(10, 0xff0000);
                    _palette.Add(11, 0xff00);
                    _palette.Add(12, 0xff);
                    _palette.Add(13, 0xffff00);
                    _palette.Add(14, 0xff00ff);
                    _palette.Add(15, 0xffff);
                    _palette.Add(0x10, 0x800000);
                    _palette.Add(0x11, 0x8000);
                    _palette.Add(0x12, 0x80);
                    _palette.Add(0x13, 0x808000);
                    _palette.Add(20, 0x800080);
                    _palette.Add(0x15, 0x8080);
                    _palette.Add(0x16, 0xc0c0c0);
                    _palette.Add(0x17, 0x808080);
                    _palette.Add(0x18, 0x9999ff);
                    _palette.Add(0x19, 0x993366);
                    _palette.Add(0x1a, 0xffffcc);
                    _palette.Add(0x1b, 0xccffff);
                    _palette.Add(0x1c, 0x660066);
                    _palette.Add(0x1d, 0xff8080);
                    _palette.Add(30, 0x66cc);
                    _palette.Add(0x1f, 0xccccff);
                    _palette.Add(0x20, 0x80);
                    _palette.Add(0x21, 0xff00ff);
                    _palette.Add(0x22, 0xffff00);
                    _palette.Add(0x23, 0xffff);
                    _palette.Add(0x24, 0x800080);
                    _palette.Add(0x25, 0x800000);
                    _palette.Add(0x26, 0x8080);
                    _palette.Add(0x27, 0xff);
                    _palette.Add(40, 0xccff);
                    _palette.Add(0x29, 0xccffff);
                    _palette.Add(0x2a, 0xccffcc);
                    _palette.Add(0x2b, 0xffff99);
                    _palette.Add(0x2c, 0x99ccff);
                    _palette.Add(0x2d, 0xff99cc);
                    _palette.Add(0x2e, 0xcc99ff);
                    _palette.Add(0x2f, 0xffcc99);
                    _palette.Add(0x30, 0x3366ff);
                    _palette.Add(0x31, 0x33cccc);
                    _palette.Add(50, 0x99cc00);
                    _palette.Add(0x33, 0xffcc00);
                    _palette.Add(0x34, 0xff9900);
                    _palette.Add(0x35, 0xff6600);
                    _palette.Add(0x36, 0x666699);
                    _palette.Add(0x37, 0x969696);
                    _palette.Add(0x38, 0x3366);
                    _palette.Add(0x39, 0x339966);
                    _palette.Add(0x3a, 0x3300);
                    _palette.Add(0x3b, 0x333300);
                    _palette.Add(60, 0x993300);
                    _palette.Add(0x3d, 0x993366);
                    _palette.Add(0x3e, 0x333399);
                    _palette.Add(0x3f, 0x333333);
                    _palette.Add(0x40, GcSystemColors.GetSystemColor(GcSystemColorIndex.WindowText).ToArgb() & 0xffffff);
                    _palette.Add(0x41, GcSystemColors.GetSystemColor(GcSystemColorIndex.Window).ToArgb() & 0xffffff);
                }
                return _palette;
            }
        }
    }
}

