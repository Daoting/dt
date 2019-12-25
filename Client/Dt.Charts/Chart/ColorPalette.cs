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
using Windows.UI;
#endregion

namespace Dt.Charts
{
    internal class ColorPalette
    {
        int _accentIndex = 4;
        static Dictionary<Office2007Themes, ColorPalette> _builtIns;
        Windows.UI.Color[] _colors;
        const int _cycleCount = 6;
        const int _firstAccent = 4;
        string _name;

        public ColorPalette(string name, params Windows.UI.Color[] colors)
        {
            _name = name;
            _colors = colors;
        }

        static void BuildThemeTable()
        {
            _builtIns = new Dictionary<Office2007Themes, ColorPalette>();
            Windows.UI.Color[] colors = new Windows.UI.Color[] { Windows.UI.Color.FromArgb(0xff, 0xbd, 0, 0), Windows.UI.Color.FromArgb(0xff, 0xff, 0, 0), Windows.UI.Color.FromArgb(0xff, 0xff, 190, 0), Windows.UI.Color.FromArgb(0xff, 0xff, 0xff, 0), Windows.UI.Color.FromArgb(0xff, 0x94, 0xd7, 0x52), Windows.UI.Color.FromArgb(0xff, 0, 0xb6, 0x52), Windows.UI.Color.FromArgb(0xff, 0, 0xb6, 0xef), Windows.UI.Color.FromArgb(0xff, 0, 0x75, 0xc6), Windows.UI.Color.FromArgb(0xff, 0, 0x22, 0x63), Windows.UI.Color.FromArgb(0xff, 0x73, 0x35, 0x9c) };
            _builtIns[Office2007Themes.Standard] = new ColorPalette(string.Empty, colors);
            Windows.UI.Color[] colorArray2 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x15, 0x48, 0x7b), Windows.UI.Color.FromArgb(0xff, 0xef, 0xef, 0xe7), Windows.UI.Color.FromArgb(0xff, 0x4a, 130, 0xbd), Windows.UI.Color.FromArgb(0xff, 0xc6, 80, 0x4a), Windows.UI.Color.FromArgb(0xff, 0x9c, 0xba, 90), Windows.UI.Color.FromArgb(0xff, 0x84, 0x65, 0xa5), Windows.UI.Color.FromArgb(0xff, 0x4a, 0xae, 0xc6), Windows.UI.Color.FromArgb(0xff, 0xf7, 150, 0x42) };
            _builtIns[Office2007Themes.Office] = new ColorPalette("Office", colorArray2);
            Windows.UI.Color[] colorArray3 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0, 0, 0), Windows.UI.Color.FromArgb(0xff, 0xff, 0xff, 0xff), Windows.UI.Color.FromArgb(0xff, 0xde, 0xde, 0xde), Windows.UI.Color.FromArgb(0xff, 180, 180, 180), Windows.UI.Color.FromArgb(0xff, 150, 150, 150), Windows.UI.Color.FromArgb(0xff, 130, 130, 130), Windows.UI.Color.FromArgb(0xff, 90, 90, 90), Windows.UI.Color.FromArgb(0xff, 0x4b, 0x4b, 0x4b) };
            _builtIns[Office2007Themes.GrayScale] = new ColorPalette("GrayScale", colorArray3);
            Windows.UI.Color[] colorArray4 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x6b, 0x65, 0x6b), Windows.UI.Color.FromArgb(0xff, 0xce, 0xc3, 0xd6), Windows.UI.Color.FromArgb(0xff, 0xce, 0xba, 0x63), Windows.UI.Color.FromArgb(0xff, 0x9c, 0xb2, 0x84), Windows.UI.Color.FromArgb(0xff, 0x6b, 0xb2, 0xce), Windows.UI.Color.FromArgb(0xff, 0x63, 0x86, 0xce), Windows.UI.Color.FromArgb(0xff, 0x7b, 0x69, 0xce), Windows.UI.Color.FromArgb(0xff, 0xa5, 120, 0xbd) };
            _builtIns[Office2007Themes.Apex] = new ColorPalette("Apex", colorArray4);
            Windows.UI.Color[] colorArray5 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x33, 0x2e, 0x33), Windows.UI.Color.FromArgb(0xff, 0xe7, 0xdf, 0xd6), Windows.UI.Color.FromArgb(0xff, 0xf7, 0x7d, 0), Windows.UI.Color.FromArgb(0xff, 0x38, 0x27, 0x33), Windows.UI.Color.FromArgb(0xff, 0x15, 0x59, 0x7b), Windows.UI.Color.FromArgb(0xff, 0x4a, 0x86, 0x42), Windows.UI.Color.FromArgb(0xff, 0x63, 0x48, 0x7b), Windows.UI.Color.FromArgb(0xff, 0xc6, 0x9a, 90) };
            _builtIns[Office2007Themes.Aspect] = new ColorPalette("Aspect", colorArray5);
            Windows.UI.Color[] colorArray6 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x63, 0x69, 0x84), Windows.UI.Color.FromArgb(0xff, 0xc6, 0xd3, 0xd6), Windows.UI.Color.FromArgb(0xff, 0xd6, 0x60, 0x4a), Windows.UI.Color.FromArgb(0xff, 0xce, 0xb6, 0), Windows.UI.Color.FromArgb(0xff, 40, 0xae, 0xad), Windows.UI.Color.FromArgb(0xff, 140, 120, 0x73), Windows.UI.Color.FromArgb(0xff, 140, 0xb2, 140), Windows.UI.Color.FromArgb(0xff, 14, 0x92, 0x4a) };
            _builtIns[Office2007Themes.Civic] = new ColorPalette("Civic", colorArray6);
            Windows.UI.Color[] colorArray7 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x42, 0x44, 0x42), Windows.UI.Color.FromArgb(0xff, 0xde, 0xf7, 0xff), Windows.UI.Color.FromArgb(0xff, 0x2b, 0xa2, 0xbd), Windows.UI.Color.FromArgb(0xff, 0xde, 0x1c, 0x2b), Windows.UI.Color.FromArgb(0xff, 0xef, 0x65, 0x15), Windows.UI.Color.FromArgb(0xff, 0x38, 0x60, 0x9c), Windows.UI.Color.FromArgb(0xff, 0x42, 0x48, 0x7b), Windows.UI.Color.FromArgb(0xff, 0x7b, 0x3d, 0x4a) };
            _builtIns[Office2007Themes.Concourse] = new ColorPalette("Concourse", colorArray7);
            Windows.UI.Color[] colorArray8 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x6b, 0x65, 0x63), Windows.UI.Color.FromArgb(0xff, 0xef, 0xe7, 0xde), Windows.UI.Color.FromArgb(0xff, 0xd6, 0x48, 0x15), Windows.UI.Color.FromArgb(0xff, 0x9c, 0x2b, 0x15), Windows.UI.Color.FromArgb(0xff, 0xa5, 0x8e, 0x6b), Windows.UI.Color.FromArgb(0xff, 0x94, 0x60, 0x52), Windows.UI.Color.FromArgb(0xff, 0x94, 0x86, 0x84), Windows.UI.Color.FromArgb(0xff, 0x84, 0x5d, 90) };
            _builtIns[Office2007Themes.Equity] = new ColorPalette("Equity", colorArray8);
            Windows.UI.Color[] colorArray9 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0, 0x60, 0x7b), Windows.UI.Color.FromArgb(0xff, 0xde, 0xf7, 0xff), Windows.UI.Color.FromArgb(0xff, 0, 0x6d, 0xc6), Windows.UI.Color.FromArgb(0xff, 0, 0x9e, 0xde), Windows.UI.Color.FromArgb(0xff, 0, 0xd3, 0xde), Windows.UI.Color.FromArgb(0xff, 0x15, 0xcf, 0x9c), Windows.UI.Color.FromArgb(0xff, 0x7b, 0xcb, 0x63), Windows.UI.Color.FromArgb(0xff, 0xa5, 0xc3, 0x4a) };
            _builtIns[Office2007Themes.Flow] = new ColorPalette("Flow", colorArray9);
            Windows.UI.Color[] colorArray10 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x63, 0x69, 0x52), Windows.UI.Color.FromArgb(0xff, 0xef, 0xeb, 0xde), Windows.UI.Color.FromArgb(0xff, 0x73, 0xa2, 0x73), Windows.UI.Color.FromArgb(0xff, 0xb5, 0xcf, 0xb5), Windows.UI.Color.FromArgb(0xff, 0xad, 0xcf, 0xd6), Windows.UI.Color.FromArgb(0xff, 0xc6, 190, 0xad), Windows.UI.Color.FromArgb(0xff, 0xce, 0xc7, 0x94), Windows.UI.Color.FromArgb(0xff, 0xef, 0xb6, 0xb5) };
            _builtIns[Office2007Themes.Foundry] = new ColorPalette("Foundry", colorArray10);
            Windows.UI.Color[] colorArray11 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x73, 0x5d, 0x52), Windows.UI.Color.FromArgb(0xff, 0xef, 0xdf, 0xc6), Windows.UI.Color.FromArgb(0xff, 0x94, 0xb6, 0xd6), Windows.UI.Color.FromArgb(0xff, 0xde, 130, 0x42), Windows.UI.Color.FromArgb(0xff, 0xa5, 170, 0x84), Windows.UI.Color.FromArgb(0xff, 0xde, 0xb2, 90), Windows.UI.Color.FromArgb(0xff, 0x7b, 0xa6, 0x9c), Windows.UI.Color.FromArgb(0xff, 0x94, 0x8e, 140) };
            _builtIns[Office2007Themes.Median] = new ColorPalette("Median", colorArray11);
            Windows.UI.Color[] colorArray12 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x4a, 0x59, 0x6b), Windows.UI.Color.FromArgb(0xff, 0xd6, 0xef, 0xff), Windows.UI.Color.FromArgb(0xff, 0x7b, 0xd3, 0x38), Windows.UI.Color.FromArgb(0xff, 0xef, 0x15, 0x7b), Windows.UI.Color.FromArgb(0xff, 0xff, 0xba, 0), Windows.UI.Color.FromArgb(0xff, 0, 0xae, 0xde), Windows.UI.Color.FromArgb(0xff, 0x73, 0x8a, 0xce), Windows.UI.Color.FromArgb(0xff, 0x15, 0xb2, 0x9c) };
            _builtIns[Office2007Themes.Metro] = new ColorPalette("Metro", colorArray12);
            Windows.UI.Color[] colorArray13 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 90, 0x60, 0x7b), Windows.UI.Color.FromArgb(0xff, 0xd6, 0xd7, 0xd6), Windows.UI.Color.FromArgb(0xff, 0xf7, 0xae, 0), Windows.UI.Color.FromArgb(0xff, 0x63, 0xb6, 0xce), Windows.UI.Color.FromArgb(0xff, 0xe7, 0x6d, 0x7b), Windows.UI.Color.FromArgb(0xff, 0x6b, 0xb6, 0x6b), Windows.UI.Color.FromArgb(0xff, 0xef, 0x86, 0x52), Windows.UI.Color.FromArgb(0xff, 0xc6, 0x48, 0x42) };
            _builtIns[Office2007Themes.Module] = new ColorPalette("Module", colorArray13);
            Windows.UI.Color[] colorArray14 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0xb5, 0x3d, 0x9c), Windows.UI.Color.FromArgb(0xff, 0xf7, 0xe7, 0xef), Windows.UI.Color.FromArgb(0xff, 0xbd, 0x3d, 0x6b), Windows.UI.Color.FromArgb(0xff, 0xad, 0x65, 0xbd), Windows.UI.Color.FromArgb(0xff, 0xde, 0x6d, 0x33), Windows.UI.Color.FromArgb(0xff, 0xff, 0xb6, 0x38), Windows.UI.Color.FromArgb(0xff, 0xce, 0x6d, 0xa5), Windows.UI.Color.FromArgb(0xff, 0xff, 0x8e, 0x38) };
            _builtIns[Office2007Themes.Opulent] = new ColorPalette("Opulent", colorArray14);
            Windows.UI.Color[] colorArray15 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x52, 0x5d, 0x6b), Windows.UI.Color.FromArgb(0xff, 0xff, 0xf3, 0x9c), Windows.UI.Color.FromArgb(0xff, 0xff, 0x86, 0x33), Windows.UI.Color.FromArgb(0xff, 0x73, 0x9a, 0xde), Windows.UI.Color.FromArgb(0xff, 0xb5, 0x2b, 0x15), Windows.UI.Color.FromArgb(0xff, 0xf7, 0xcf, 0x2b), Windows.UI.Color.FromArgb(0xff, 0xad, 0xba, 0xd6), Windows.UI.Color.FromArgb(0xff, 0x73, 0x7d, 0x84) };
            _builtIns[Office2007Themes.Oriel] = new ColorPalette("Oriel", colorArray15);
            Windows.UI.Color[] colorArray16 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x42, 0x44, 0x52), Windows.UI.Color.FromArgb(0xff, 0xde, 0xeb, 0xef), Windows.UI.Color.FromArgb(0xff, 0x73, 0x7d, 0xa5), Windows.UI.Color.FromArgb(0xff, 0x9c, 0xba, 0xce), Windows.UI.Color.FromArgb(0xff, 0xd6, 0xdb, 0x7b), Windows.UI.Color.FromArgb(0xff, 0xff, 0xdb, 0x7b), Windows.UI.Color.FromArgb(0xff, 0xbd, 0x86, 0x73), Windows.UI.Color.FromArgb(0xff, 140, 0x72, 0x6b) };
            _builtIns[Office2007Themes.Origin] = new ColorPalette("Origin", colorArray16);
            Windows.UI.Color[] colorArray17 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x42, 0x4c, 0x22), Windows.UI.Color.FromArgb(0xff, 0xff, 0xfb, 0xce), Windows.UI.Color.FromArgb(0xff, 0xa5, 0xb6, 0x94), Windows.UI.Color.FromArgb(0xff, 0xf7, 0xa6, 0x42), Windows.UI.Color.FromArgb(0xff, 0xe7, 190, 0x2b), Windows.UI.Color.FromArgb(0xff, 0xd6, 0x92, 0xa5), Windows.UI.Color.FromArgb(0xff, 0x9c, 0x86, 0xc6), Windows.UI.Color.FromArgb(0xff, 0x84, 0x9e, 0xc6) };
            _builtIns[Office2007Themes.Paper] = new ColorPalette("Paper", colorArray17);
            Windows.UI.Color[] colorArray18 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x4a, 0x22, 0x15), Windows.UI.Color.FromArgb(0xff, 0xe7, 0xdf, 0xce), Windows.UI.Color.FromArgb(0xff, 0x38, 0x92, 0xa5), Windows.UI.Color.FromArgb(0xff, 0xff, 0xba, 0), Windows.UI.Color.FromArgb(0xff, 0xc6, 0x2b, 0x2b), Windows.UI.Color.FromArgb(0xff, 0x84, 170, 0x33), Windows.UI.Color.FromArgb(0xff, 0x94, 0x42, 0), Windows.UI.Color.FromArgb(0xff, 0x42, 0x59, 140) };
            _builtIns[Office2007Themes.Solstice] = new ColorPalette("Solstice", colorArray18);
            Windows.UI.Color[] colorArray19 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x38, 0x38, 0x38), Windows.UI.Color.FromArgb(0xff, 0xd6, 0xd3, 0xd6), Windows.UI.Color.FromArgb(0xff, 0x6b, 0xa2, 0xb5), Windows.UI.Color.FromArgb(0xff, 0xce, 0xae, 0), Windows.UI.Color.FromArgb(0xff, 140, 0x8a, 0xa5), Windows.UI.Color.FromArgb(0xff, 0x73, 0x86, 0x63), Windows.UI.Color.FromArgb(0xff, 0x9c, 0x92, 0x73), Windows.UI.Color.FromArgb(0xff, 0x7b, 0x86, 140) };
            _builtIns[Office2007Themes.Technic] = new ColorPalette("Technic", colorArray19);
            Windows.UI.Color[] colorArray20 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x4a, 0x38, 0x33), Windows.UI.Color.FromArgb(0xff, 0xff, 0xef, 0xce), Windows.UI.Color.FromArgb(0xff, 0xf7, 0xa2, 0x2b), Windows.UI.Color.FromArgb(0xff, 0xa5, 0x65, 0x4a), Windows.UI.Color.FromArgb(0xff, 0xb5, 0x8a, 0x84), Windows.UI.Color.FromArgb(0xff, 0xc6, 0x9a, 0x6b), Windows.UI.Color.FromArgb(0xff, 0xa5, 150, 0x73), Windows.UI.Color.FromArgb(0xff, 0xc6, 0x75, 0x2b) };
            _builtIns[Office2007Themes.Trek] = new ColorPalette("Trek", colorArray20);
            Windows.UI.Color[] colorArray21 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x42, 0x44, 0x52), Windows.UI.Color.FromArgb(0xff, 0xde, 0xdf, 0xde), Windows.UI.Color.FromArgb(0xff, 0x52, 0x55, 140), Windows.UI.Color.FromArgb(0xff, 0x42, 130, 0x84), Windows.UI.Color.FromArgb(0xff, 0xa5, 0x4c, 0xa5), Windows.UI.Color.FromArgb(0xff, 0xc6, 0x65, 0x2b), Windows.UI.Color.FromArgb(0xff, 140, 0x5d, 0x38), Windows.UI.Color.FromArgb(0xff, 90, 0x92, 0xb5) };
            _builtIns[Office2007Themes.Urban] = new ColorPalette("Urban", colorArray21);
            Windows.UI.Color[] colorArray22 = new Windows.UI.Color[] { Colors.White, Colors.Black, Windows.UI.Color.FromArgb(0xff, 0x63, 0x65, 0x63), Windows.UI.Color.FromArgb(0xff, 0xd6, 0xd3, 0xd6), Windows.UI.Color.FromArgb(0xff, 0xff, 0x38, 140), Windows.UI.Color.FromArgb(0xff, 0xe7, 0, 90), Windows.UI.Color.FromArgb(0xff, 0x9c, 0, 0x7b), Windows.UI.Color.FromArgb(0xff, 0x6b, 0, 0x7b), Windows.UI.Color.FromArgb(0xff, 0, 0x59, 0xd6), Windows.UI.Color.FromArgb(0xff, 0, 0x35, 0x9c) };
            _builtIns[Office2007Themes.Verve] = new ColorPalette("Verve", colorArray22);
        }

        public static Windows.UI.Color ColorFromHSL(double h, double s, double l)
        {
            if (l == 0.0)
            {
                return Colors.Black;
            }
            if (s == 0.0)
            {
                byte r = (byte)(255.0 * l);
                return Windows.UI.Color.FromArgb(0xff, r, r, r);
            }
            h /= 360.0;
            double num2 = (l <= 0.5) ? (l * (1.0 + s)) : ((l + s) - (l * s));
            double num3 = (2.0 * l) - num2;
            double[] numArray = new double[] { h + 0.33333333333333331, h, h - 0.33333333333333331 };
            double[] numArray2 = new double[3];
            for (int i = 0; i < 3; i++)
            {
                if (numArray[i] < 0.0)
                {
                    numArray[i]++;
                }
                if (numArray[i] > 1.0)
                {
                    numArray[i]--;
                }
                if ((6.0 * numArray[i]) < 1.0)
                {
                    numArray2[i] = num3 + (((num2 - num3) * numArray[i]) * 6.0);
                }
                else if ((2.0 * numArray[i]) < 1.0)
                {
                    numArray2[i] = num2;
                }
                else if ((3.0 * numArray[i]) < 2.0)
                {
                    numArray2[i] = num3 + (((num2 - num3) * (0.66666666666666663 - numArray[i])) * 6.0);
                }
                else
                {
                    numArray2[i] = num3;
                }
                numArray2[i] = Math.Round((double) (255.0 * numArray2[i]));
            }
            return Windows.UI.Color.FromArgb(0xff, (byte) numArray2[0], (byte) numArray2[1], (byte) numArray2[2]);
        }

        public static Windows.UI.Color Darken(Windows.UI.Color clr)
        {
            float hue = GetHue(clr);
            float saturation = GetSaturation(clr);
            float num3 = GetBrightness(clr) * 0.5f;
            return ColorFromHSL((double) hue, (double) saturation, (double) num3);
        }

        bool ExtendThemeColors(int maxAccentColors)
        {
            if ((_colors == null) || (_colors.Length < 10))
            {
                return false;
            }
            if ((maxAccentColors + 4) > _colors.Length)
            {
                maxAccentColors = (int) Math.Round((double) (Math.Ceiling((double) (((double) maxAccentColors) / 6.0)) * 6.0));
                int num = _colors.Length - 6;
                int num2 = maxAccentColors + 4;
                Windows.UI.Color[] colorArray = new Windows.UI.Color[num2];
                _colors.CopyTo(colorArray, 0);
                num2 -= 6;
                for (int i = num; i < num2; i++)
                {
                    Windows.UI.Color clr = colorArray[i];
                    float hue = GetHue(clr);
                    float saturation = GetSaturation(clr);
                    float num6 = (GetBrightness(clr) + 0.15f) % 1f;
                    if (num6 < 0.25f)
                    {
                        num6 += 0.25f;
                    }
                    clr = ColorFromHSL((double) hue, (double) saturation, (double) num6);
                    colorArray[i + 6] = clr;
                }
                _colors = colorArray;
            }
            return true;
        }

        public static float GetBrightness(Windows.UI.Color clr)
        {
            float num = ((float) clr.R) / 255f;
            float num2 = ((float) clr.G) / 255f;
            float num3 = ((float) clr.B) / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            return ((num4 + num5) / 2f);
        }

        public Windows.UI.Color GetColor(int index)
        {
            int num = index + 4;
            if (num < AllColors.Length)
            {
                return AllColors[num];
            }
            _accentIndex = num;
            return GetNextAccent();
        }

        public static float GetHue(Windows.UI.Color clr)
        {
            if ((clr.R == clr.G) && (clr.G == clr.B))
            {
                return 0f;
            }
            float num = ((float) clr.R) / 255f;
            float num2 = ((float) clr.G) / 255f;
            float num3 = ((float) clr.B) / 255f;
            float num4 = 0f;
            float num5 = num;
            float num6 = num;
            if (num2 > num5)
            {
                num5 = num2;
            }
            if (num3 > num5)
            {
                num5 = num3;
            }
            if (num2 < num6)
            {
                num6 = num2;
            }
            if (num3 < num6)
            {
                num6 = num3;
            }
            float num7 = num5 - num6;
            if (num == num5)
            {
                num4 = (num2 - num3) / num7;
            }
            else if (num2 == num5)
            {
                num4 = 2f + ((num3 - num) / num7);
            }
            else if (num3 == num5)
            {
                num4 = 4f + ((num - num2) / num7);
            }
            num4 *= 60f;
            if (num4 < 0f)
            {
                num4 += 360f;
            }
            return num4;
        }

        public Windows.UI.Color GetNextAccent()
        {
            if (_colors.Length <= _accentIndex)
            {
                ExtendThemeColors((_accentIndex - 4) + 1);
            }
            Windows.UI.Color color = _colors[_accentIndex];
            _accentIndex++;
            if (_accentIndex >= 40)
            {
                _accentIndex = 4;
            }
            return color;
        }

        public static ColorPalette GetOfficePalette(Office2007Themes themeColors)
        {
            if (_builtIns == null)
            {
                BuildThemeTable();
            }
            return _builtIns[themeColors];
        }

        public static ColorPalette GetOfficePalette(Palette colorGenTheme)
        {
            return GetOfficePalette((Office2007Themes) colorGenTheme);
        }

        public static float GetSaturation(Windows.UI.Color clr)
        {
            float num = ((float) clr.R) / 255f;
            float num2 = ((float) clr.G) / 255f;
            float num3 = ((float) clr.B) / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            if (num4 == num5)
            {
                return 0f;
            }
            float num6 = (num4 + num5) / 2f;
            if (num6 <= 0.5)
            {
                return ((num4 - num5) / (num4 + num5));
            }
            return ((num4 - num5) / ((2f - num4) - num5));
        }

        public Windows.UI.Color[] AllColors
        {
            get { return  _colors; }
        }
    }
}

