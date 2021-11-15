#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    internal class ColorHelper
    {
        static Dictionary<int, uint> _palette = new Dictionary<int, uint>();
        public static readonly Windows.UI.Color Black = Windows.UI.Color.FromArgb(0xff, 0, 0, 0);
        internal static Dictionary<int, Windows.UI.Color> CustomPalette = null;
        public static readonly Windows.UI.Color EmptyColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
        public static readonly Windows.UI.Color TransparentColor = Windows.UI.Color.FromArgb(0, 0xff, 0xff, 0xff);
        internal static bool UseCustomPalette = false;
        public static readonly Windows.UI.Color White = Windows.UI.Color.FromArgb(0xff, 0xff, 0xff, 0xff);

        static ColorHelper()
        {
            _palette.Add(0, 0xff000000);
            _palette.Add(1, uint.MaxValue);
            _palette.Add(2, 0xffff0000);
            _palette.Add(3, 0xff00ff00);
            _palette.Add(4, 0xff0000ff);
            _palette.Add(5, 0xffffff00);
            _palette.Add(6, 0xffff00ff);
            _palette.Add(7, 0xff00ffff);
            _palette.Add(8, 0xff000000);
            _palette.Add(9, uint.MaxValue);
            _palette.Add(10, 0xffff0000);
            _palette.Add(11, 0xff00ff00);
            _palette.Add(12, 0xff0000ff);
            _palette.Add(13, 0xffffff00);
            _palette.Add(14, 0xffff00ff);
            _palette.Add(15, 0xff00ffff);
            _palette.Add(0x10, 0xff800000);
            _palette.Add(0x11, 0xff008000);
            _palette.Add(0x12, 0xff000080);
            _palette.Add(0x13, 0xff808000);
            _palette.Add(20, 0xff800080);
            _palette.Add(0x15, 0xff008080);
            _palette.Add(0x16, 0xffc0c0c0);
            _palette.Add(0x17, 0xff808080);
            _palette.Add(0x18, 0xff9999ff);
            _palette.Add(0x19, 0xff993366);
            _palette.Add(0x1a, 0xffffffcc);
            _palette.Add(0x1b, 0xffccffff);
            _palette.Add(0x1c, 0xff660066);
            _palette.Add(0x1d, 0xffff8080);
            _palette.Add(30, 0xff0066cc);
            _palette.Add(0x1f, 0xffccccff);
            _palette.Add(0x20, 0xff000080);
            _palette.Add(0x21, 0xffff00ff);
            _palette.Add(0x22, 0xffffff00);
            _palette.Add(0x23, 0xff00ffff);
            _palette.Add(0x24, 0xff800080);
            _palette.Add(0x25, 0xff800000);
            _palette.Add(0x26, 0xff008080);
            _palette.Add(0x27, 0xff0000ff);
            _palette.Add(40, 0xff00ccff);
            _palette.Add(0x29, 0xffccffff);
            _palette.Add(0x2a, 0xffccffcc);
            _palette.Add(0x2b, 0xffffff99);
            _palette.Add(0x2c, 0xff99ccff);
            _palette.Add(0x2d, 0xffff99cc);
            _palette.Add(0x2e, 0xffcc99ff);
            _palette.Add(0x2f, 0xffffcc99);
            _palette.Add(0x30, 0xff3366ff);
            _palette.Add(0x31, 0xff33cccc);
            _palette.Add(50, 0xff99cc00);
            _palette.Add(0x33, 0xffffcc00);
            _palette.Add(0x34, 0xffff9900);
            _palette.Add(0x35, 0xffff6600);
            _palette.Add(0x36, 0xff666699);
            _palette.Add(0x37, 0xff969696);
            _palette.Add(0x38, 0xff003366);
            _palette.Add(0x39, 0xff339966);
            _palette.Add(0x3a, 0xff003300);
            _palette.Add(0x3b, 0xff333300);
            _palette.Add(60, 0xff993300);
            _palette.Add(0x3d, 0xff993366);
            _palette.Add(0x3e, 0xff333399);
            _palette.Add(0x3f, 0xff333333);
            _palette.Add(0x40, (uint) ToArgb(SystemColors.WindowTextColor));
            _palette.Add(0x41, (uint) ToArgb(SystemColors.WindowColor));
        }

        public static Windows.UI.Color ColorFromHLS(int hue, int luminosity, int saturation)
        {
            return HLSColor.ColorFromHLS(hue, luminosity, saturation);
        }

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
        /// Converts a color from an index.
        /// </summary>
        /// <param name="index">The index from which to convert.</param>
        /// <param name="alpha">The alpha value.</param>
        /// <returns>Returns the color.</returns>
        public static Windows.UI.Color ColorFromIndex(int index, byte alpha)
        {
            return ColorFromIndex(index, alpha, null);
        }

        /// <summary>
        /// Paints from the index using the specified palette.
        /// </summary>
        /// <param name="index">The index from which to convert.</param>
        /// <param name="alpha">The alpha value.</param>
        /// <param name="palette">The palette to use.</param>
        /// <returns>Returns the color.</returns>
        public static Windows.UI.Color ColorFromIndex(int index, byte alpha, byte[,] palette)
        {
            if (index == 0x7fff)
            {
                return new Windows.UI.Color();
            }
            if ((index >= 0) && (index < _palette.Count))
            {
                if (!UseCustomPalette)
                {
                    uint num = _palette[index];
                    byte r = (byte) ((num >> 0x10) & 0xff);
                    byte g = (byte) ((num >> 8) & 0xff);
                    byte b = (byte) (num & 0xff);
                    return Windows.UI.Color.FromArgb(alpha, r, g, b);
                }
                if (CustomPalette.ContainsKey(index))
                {
                    return CustomPalette[index];
                }
                if (index == 0x40)
                {
                    return SystemColors.WindowTextColor;
                }
                if (index == 0x41)
                {
                    return SystemColors.WindowColor;
                }
            }
            return Colors.Transparent;
        }

        public static Windows.UI.Color Dark(Windows.UI.Color baseColor, float percOfDarkDark)
        {
            HLSColor color = new HLSColor(baseColor);
            return color.Darker(percOfDarkDark);
        }

        public static Windows.UI.Color FromArgb(uint value)
        {
            return Windows.UI.Color.FromArgb((byte) ((value >> 0x18) & 0xff), (byte) ((value >> 0x10) & 0xff), (byte) ((value >> 8) & 0xff), (byte) (value & 0xff));
        }

        /// <summary>
        /// Converts a color from a string value.
        /// </summary>
        /// <param name="color">The color (#ff223344).</param>
        /// <returns>Returns the color.</returns>
        public static Windows.UI.Color? FromStringValue(string color)
        {
            if ((color != null) && color.StartsWith(((char) DefaultTokens.Sharp).ToString()))
            {
                try
                {
                    int num = NumberHelper.ParseHexString(color.TrimStart(new char[] { DefaultTokens.Sharp }));
                    return new Windows.UI.Color?(Windows.UI.Color.FromArgb((byte) ((num & 0xff000000L) >> 0x18), (byte) ((num & 0xff0000) >> 0x10), (byte) ((num & 0xff00) >> 8), (byte) (num & 0xff)));
                }
                catch
                {
                }
            }
            return null;
        }

        static int GetCloestColorIndex(Windows.UI.Color color, int startIndex = 0x40)
        {
            int num = -1;
            double maxValue = double.MaxValue;
            for (int i = Math.Min(_palette.Count - 1, startIndex); i >= 0; i--)
            {
                uint num4 = _palette[i];
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
            return num;
        }

        static byte GetMaxInt(int baseColorAlpha, int colorAlpha, int baseColorValue, int colorValue, double resultColorAlpha)
        {
            if (resultColorAlpha == 0.0)
            {
                return 0;
            }
            double num = ((colorAlpha * colorValue) + (((double) (((0xff - colorAlpha) * baseColorAlpha) * baseColorValue)) / 255.0)) / resultColorAlpha;
            return (byte) num;
        }

        internal static int GetPaletteColorIndex(IThemeSupport themeSupport, IExcelColor excelColor, int startIndex = 0x40)
        {
            if (excelColor.IsThemeColor)
            {
                return GetCloestColorIndex(GetRGBColor(themeSupport, new ExcelColor(ExcelColorType.Theme, excelColor.Value, 0.0)), 0x40);
            }
            return GetCloestColorIndex(GetRGBColor(themeSupport, excelColor), startIndex);
        }

        internal static Windows.UI.Color GetRGBColor(IThemeSupport themeSupport, IExcelColor excelColor)
        {
            if (excelColor != null)
            {
                if (excelColor.IsAutoColor)
                {
                    return SystemColors.WindowTextColor;
                }
                if (excelColor.IsIndexedColor)
                {
                    return UpdateTint(ColorFromIndex((int) excelColor.Value), (float) excelColor.Tint);
                }
                if (excelColor.IsRGBColor)
                {
                    uint num = excelColor.Value | 0xff000000;
                    return UpdateTint(FromArgb(num), (float) excelColor.Tint);
                }
                if (excelColor.IsThemeColor && (themeSupport != null))
                {
                    return UpdateTint(themeSupport.GetThemeColor(excelColor.GetThemeColorName()), (float) excelColor.Tint);
                }
            }
            return Colors.Transparent;
        }

        public static bool IsEmpty(Windows.UI.Color color)
        {
            return (color == EmptyColor);
        }

        public static bool IsTransparent(Windows.UI.Color color)
        {
            return (color == TransparentColor);
        }

        public static Windows.UI.Color Light(Windows.UI.Color baseColor, float percOfLightLight)
        {
            HLSColor color = new HLSColor(baseColor);
            return color.Lighter(percOfLightLight);
        }

        static int MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            int num = 0;
            num |= alpha << 0x18;
            num |= red << 0x10;
            num |= green << 8;
            return (num | blue);
        }

        internal static Windows.UI.Color MixTranslucentColor(Windows.UI.Color baseColor, Windows.UI.Color color)
        {
            if (color.A == 0xff)
            {
                return color;
            }
            if (color.A == 0)
            {
                return baseColor;
            }
            byte a = baseColor.A;
            double num = (int) Math.Round((double) (((double) ((0xff - baseColor.A) * color.A)) / 255.0));
            byte r = GetMaxInt(baseColor.A, color.A, baseColor.R, color.R, num + baseColor.A);
            byte g = GetMaxInt(baseColor.A, color.A, baseColor.G, color.G, num + baseColor.A);
            byte b = GetMaxInt(baseColor.A, color.A, baseColor.B, color.B, num + baseColor.A);
            return Windows.UI.Color.FromArgb((byte) Math.Round((double) (num + baseColor.A)), r, g, b);
        }

        public static int ToArgb(Windows.UI.Color color)
        {
            return MakeArgb(color.A, color.R, color.G, color.B);
        }

        internal static Windows.UI.Color UpdateColor(Color color, SpreadDrawingColorSettings drawingColorSettings, bool updateTint)
        {
            if (drawingColorSettings != null)
            {
                double? alpha = drawingColorSettings.alpha;
                if (alpha.HasValue)
                {
                    color = Windows.UI.Color.FromArgb((byte)((alpha.Value / 100000.0) * 255.0), color.R, color.G, color.B);
                }
                double? shade = drawingColorSettings.shade;
                if (shade.HasValue)
                {
                    float num = ((float)shade.Value) / 100000f;
                    byte r = (byte)(num * color.R);
                    byte g = (byte)(num * color.G);
                    byte b = (byte)(num * color.B);
                    color = Windows.UI.Color.FromArgb(color.A, r, g, b);
                }
                if (updateTint)
                {
                    double? tint = drawingColorSettings.tint;
                    if (tint.HasValue)
                    {
                        float num5 = ((float)tint.Value) / 100000f;
                        byte num6 = (byte)((num5 * color.R) + ((1f - num5) * 255f));
                        byte num7 = (byte)((num5 * color.G) + ((1f - num5) * 255f));
                        byte num8 = (byte)((num5 * color.B) + ((1f - num5) * 255f));
                        color = Windows.UI.Color.FromArgb(color.A, num6, num7, num8);
                    }
                }
                double? hue = drawingColorSettings.hue;
                double? hueOff = drawingColorSettings.hueOff;
                double? hueMod = drawingColorSettings.hueMod;
                double? sat = drawingColorSettings.sat;
                double? satOff = drawingColorSettings.satOff;
                double? satMod = drawingColorSettings.satMod;
                double? lum = drawingColorSettings.lum;
                double? lumOff = drawingColorSettings.lumOff;
                double? lumMod = drawingColorSettings.lumMod;
                if (((!hue.HasValue && !hueOff.HasValue) && (!hueMod.HasValue && !sat.HasValue)) && ((!satOff.HasValue && !satMod.HasValue) && ((!lum.HasValue && !lumOff.HasValue) && !lumMod.HasValue)))
                {
                    return color;
                }
                HLSColor color2 = new HLSColor(color);
                int num9 = color2.Hue;
                int saturation = color2.Saturation;
                int luminosity = color2.Luminosity;
                int num12 = 0xff;
                if (hue.HasValue)
                {
                    num9 = (int)((hue.Value * num12) / 36000000.0);
                }
                if (hueMod.HasValue)
                {
                    num9 = (int)((num9 * hueMod.Value) / 100000.0);
                    if (num9 > num12)
                    {
                        num9 = num12;
                    }
                }
                if (hueOff.HasValue)
                {
                    num9 += (int)((hueOff.Value * num12) / 21600000.0);
                    if (num9 > num12)
                    {
                        num9 = num12;
                    }
                    else if (num9 < 0)
                    {
                        num9 = 0;
                    }
                }
                if (sat.HasValue)
                {
                    saturation = (int)((sat.Value * num12) / 1000000.0);
                }
                if (satMod.HasValue)
                {
                    saturation = (int)((saturation * satMod.Value) / 100000.0);
                    if (saturation > num12)
                    {
                        saturation = num12;
                    }
                }
                if (satOff.HasValue)
                {
                    saturation += (int)((satOff.Value * num12) / 100000.0);
                    if (saturation > num12)
                    {
                        saturation = num12;
                    }
                    else if (saturation < 0)
                    {
                        saturation = 0;
                    }
                }
                if (lum.HasValue)
                {
                    luminosity = (int)((lum.Value * num12) / 1000000.0);
                }
                if (lumMod.HasValue)
                {
                    luminosity = (int)((luminosity * lumMod.Value) / 100000.0);
                    if (luminosity > num12)
                    {
                        luminosity = num12;
                    }
                }
                if (lumOff.HasValue)
                {
                    luminosity += (int)((lumOff.Value * num12) / 100000.0);
                    if (luminosity > num12)
                    {
                        luminosity = num12;
                    }
                    else if (luminosity < 0)
                    {
                        luminosity = 0;
                    }
                }
                color = ColorFromHLS(num9, luminosity, saturation);
            }
            return color;
        }

        public static Windows.UI.Color UpdateTint(Windows.UI.Color baseColor, float tint)
        {
            if (tint == 0.0)
            {
                return baseColor;
            }
            HLSColor color = new HLSColor(baseColor);
            if (tint > 0f)
            {
                int num = 240 - color.Luminosity;
                float num2 = num * tint;
                return ColorFromHLS(color.Hue, color.Luminosity + ((int) num2), color.Saturation);
            }
            float num3 = color.Luminosity * -tint;
            return ColorFromHLS(color.Hue, color.Luminosity - ((int) num3), color.Saturation);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HLSColor
        {
            const int ShadowAdj = -333;
            const int HilightAdj = 500;
            const int WatermarkAdj = -50;
            const int Range = 240;
            const int HLSMax = 240;
            const int RGBMax = 0xff;
            const int Undefined = 160;
            int hue;
            int saturation;
            int luminosity;
            public HLSColor(Windows.UI.Color color)
            {
                int r = color.R;
                int g = color.G;
                int b = color.B;
                int num4 = Math.Max(Math.Max(r, g), b);
                int num5 = Math.Min(Math.Min(r, g), b);
                int num6 = num4 + num5;
                this.luminosity = ((num6 * 240) + 0xff) / 510;
                int num7 = num4 - num5;
                if (num7 == 0)
                {
                    this.saturation = 0;
                    this.hue = 160;
                }
                else
                {
                    if (this.luminosity <= 120)
                    {
                        this.saturation = ((num7 * 240) + (num6 / 2)) / num6;
                    }
                    else
                    {
                        this.saturation = ((num7 * 240) + ((510 - num6) / 2)) / (510 - num6);
                    }
                    int num8 = (((num4 - r) * 40) + (num7 / 2)) / num7;
                    int num9 = (((num4 - g) * 40) + (num7 / 2)) / num7;
                    int num10 = (((num4 - b) * 40) + (num7 / 2)) / num7;
                    if (r == num4)
                    {
                        this.hue = num10 - num9;
                    }
                    else if (g == num4)
                    {
                        this.hue = (80 + num8) - num10;
                    }
                    else
                    {
                        this.hue = (160 + num9) - num8;
                    }
                    if (this.hue < 0)
                    {
                        this.hue += 240;
                    }
                    if (this.hue > 240)
                    {
                        this.hue -= 240;
                    }
                }
            }

            public int Luminosity
            {
                get { return  this.luminosity; }
            }
            public int Hue
            {
                get { return  this.hue; }
            }
            public int Saturation
            {
                get { return  this.saturation; }
            }
            public Windows.UI.Color Darker(float percDarker)
            {
                int num = 0;
                int num2 = this.NewLuma(-333, true);
                return ColorFromHLS(this.hue, (int)(num2 - ((num2 - num) * percDarker)), this.saturation);
            }

            public override bool Equals(object o)
            {
                if (!(o is ColorHelper.HLSColor))
                {
                    return false;
                }
                ColorHelper.HLSColor color = (ColorHelper.HLSColor) o;
                return (((this.hue == color.hue) && (this.saturation == color.saturation)) && (this.luminosity == color.luminosity));
            }

            public static bool operator ==(ColorHelper.HLSColor a, ColorHelper.HLSColor b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(ColorHelper.HLSColor a, ColorHelper.HLSColor b)
            {
                return !a.Equals(b);
            }

            public override int GetHashCode()
            {
                return (((this.hue << 6) | (this.saturation << 2)) | this.luminosity);
            }

            public Windows.UI.Color Lighter(float percLighter)
            {
                int luminosity = this.luminosity;
                int num2 = this.NewLuma(500, true);
                return ColorFromHLS(this.hue, (int)(luminosity + ((num2 - luminosity) * percLighter)), this.saturation);
            }

            int NewLuma(int n, bool scale)
            {
                return this.NewLuma(this.luminosity, n, scale);
            }

            int NewLuma(int luminosity, int n, bool scale)
            {
                if (n == 0)
                {
                    return luminosity;
                }
                if (scale)
                {
                    if (n > 0)
                    {
                        return (int) (((luminosity * (0x3e8 - n)) + (0xf1L * n)) / 0x3e8L);
                    }
                    return ((luminosity * (n + 0x3e8)) / 0x3e8);
                }
                int num = luminosity;
                num += (int)((n * 240L) / 0x3e8L);
                if (num < 0)
                {
                    num = 0;
                }
                if (num > 240)
                {
                    num = 240;
                }
                return num;
            }

            public static Windows.UI.Color ColorFromHLS(int hue, int luminosity, int saturation)
            {
                byte num;
                byte num2;
                byte num3;
                if (saturation == 0)
                {
                    num = num2 = num3 = (byte)((luminosity * 0xff) / 240);
                    if (hue != 160)
                    {
                    }
                }
                else
                {
                    int num4;
                    if (luminosity <= 120)
                    {
                        num4 = ((luminosity * (240 + saturation)) + 120) / 240;
                    }
                    else
                    {
                        num4 = (luminosity + saturation) - (((luminosity * saturation) + 120) / 240);
                    }
                    int num5 = (2 * luminosity) - num4;
                    num = (byte)(((HueToRGB(num5, num4, hue + 80) * 0xff) + 120) / 240);
                    num2 = (byte)(((HueToRGB(num5, num4, hue) * 0xff) + 120) / 240);
                    num3 = (byte)(((HueToRGB(num5, num4, hue - 80) * 0xff) + 120) / 240);
                }
                return Windows.UI.Color.FromArgb(0xff, num, num2, num3);
            }

            static int HueToRGB(int n1, int n2, int hue)
            {
                if (hue < 0)
                {
                    hue += 240;
                }
                if (hue > 240)
                {
                    hue -= 240;
                }
                if (hue < 40)
                {
                    return (n1 + ((((n2 - n1) * hue) + 20) / 40));
                }
                if (hue < 120)
                {
                    return n2;
                }
                if (hue < 160)
                {
                    return (n1 + ((((n2 - n1) * (160 - hue)) + 20) / 40));
                }
                return n1;
            }
        }
    }
}

