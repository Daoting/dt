#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class SpreadDrawingColorSettings
    {
        public static implicit operator ExcelDrawingColorSettings(SpreadDrawingColorSettings colorSettings)
        {
            if (colorSettings == null)
            {
                return null;
            }
            return new ExcelDrawingColorSettings { Alpha = colorSettings.alpha, Shade = colorSettings.shade, Tint = colorSettings.tint, Hue = colorSettings.hue, HueOff = colorSettings.hueOff, HueMod = colorSettings.hueMod, Sat = colorSettings.sat, SatOff = colorSettings.satOff, SatMod = colorSettings.satMod, Lum = colorSettings.lum, LumOff = colorSettings.lumOff, LumMod = colorSettings.lumMod };
        }

        public double? alpha { get; set; }

        public double? hue { get; set; }

        public double? hueMod { get; set; }

        public double? hueOff { get; set; }

        public double? lum { get; set; }

        public double? lumMod { get; set; }

        public double? lumOff { get; set; }

        public double? sat { get; set; }

        public double? satMod { get; set; }

        public double? satOff { get; set; }

        public double? shade { get; set; }

        public double? tint { get; set; }
    }
}

