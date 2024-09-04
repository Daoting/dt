#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct HLSColor
    {
        private const int ShadowAdj = -333;
        private const int HilightAdj = 500;
        private const int WatermarkAdj = -50;
        private const int Range = 240;
        private const int HLSMax = 240;
        private const int RGBMax = 0xff;
        private const int Undefined = 160;
        private int hue;
        private int saturation;
        private int luminosity;
        public HLSColor(GcColor color)
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
        public GcColor Darker(float percDarker)
        {
            int num = 0;
            int num2 = this.NewLuma(-333, true);
            return ColorFromHLS(this.hue,(int) (num2 - ((num2 - num) * percDarker)), this.saturation);
        }

        public override bool Equals(object o)
        {
            if (!(o is HLSColor))
            {
                return false;
            }
            HLSColor color = (HLSColor) o;
            return (((this.hue == color.hue) && (this.saturation == color.saturation)) && (this.luminosity == color.luminosity));
        }

        public static bool operator ==(HLSColor a, HLSColor b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(HLSColor a, HLSColor b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return (((this.hue << 6) | (this.saturation << 2)) | this.luminosity);
        }

        public GcColor Lighter(float percLighter)
        {
            int luminosity = this.luminosity;
            int num2 = this.NewLuma(500, true);
            return ColorFromHLS(this.hue, (int)(luminosity + ((num2 - luminosity) * percLighter)), this.saturation);
        }

        private int NewLuma(int n, bool scale)
        {
            return this.NewLuma(this.luminosity, n, scale);
        }

        private int NewLuma(int luminosity, int n, bool scale)
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

        public static GcColor ColorFromHLS(int hue, int luminosity, int saturation)
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
            return GcColor.FromArgb(0xff, num, num2, num3);
        }

        private static int HueToRGB(int n1, int n2, int hue)
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

