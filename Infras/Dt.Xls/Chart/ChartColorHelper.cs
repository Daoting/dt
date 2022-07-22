#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Xls.Utils;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    internal class ChartColorHelper
    {
        internal static IExcelColor ReadColor(XElement node, ExcelDrawingColorSettings drawingColorSettings)
        {
            if (node.Name.LocalName == "schemeClr")
            {
                string str = node.GetAttributeValueOrDefaultOfStringType("val", null).ToUpperInvariant();
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "alpha")
                    {
                        drawingColorSettings.Alpha = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "shade")
                    {
                        drawingColorSettings.Shade = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "tint")
                    {
                        drawingColorSettings.Tint = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "hue")
                    {
                        drawingColorSettings.Hue = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "hueOff")
                    {
                        drawingColorSettings.HueOff = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "hueMod")
                    {
                        drawingColorSettings.HueMod = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "sat")
                    {
                        drawingColorSettings.Sat = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "satOff")
                    {
                        drawingColorSettings.SatOff = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "satMod")
                    {
                        drawingColorSettings.SatMod = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "lum")
                    {
                        drawingColorSettings.Lum = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "lumOff")
                    {
                        drawingColorSettings.LumOff = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                    else if (element.Name.LocalName == "lumMod")
                    {
                        drawingColorSettings.LumMod = new double?((double) element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                }
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return new ExcelColor(ExcelColorType.Theme, (uint) str.ToColorSchmeIndex(), 0.0);
                }
            }
            else
            {
                if (node.Name.LocalName == "hslClr")
                {
                    int num = node.GetAttributeValueOrDefaultOfInt32Type("hue", 0);
                    int num2 = node.GetAttributeValueOrDefaultOfInt32Type("sat", 0);
                    int num3 = node.GetAttributeValueOrDefaultOfInt32Type("lum", 0);
                    foreach (XElement element2 in node.Elements())
                    {
                        if (element2.Name.LocalName == "alpha")
                        {
                            drawingColorSettings.Alpha = new double?((double) element2.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                        }
                        else if (element2.Name.LocalName == "shade")
                        {
                            drawingColorSettings.Shade = new double?((double) element2.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                        }
                        else if (element2.Name.LocalName == "tint")
                        {
                            drawingColorSettings.Tint = new double?((double) element2.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                        }
                    }
                    return new ExcelColor(ExcelColorType.RGB, ColorExtension.ConvertHLSToRGB((double) num, (double) num3, (double) num2).ToArgb(), 0.0);
                }
                if (node.Name.LocalName == "prstClr")
                {
                    string str2 = node.GetAttributeValueOrDefaultOfStringType("val", null);
                    foreach (XElement element3 in node.Elements())
                    {
                        if (element3.Name.LocalName == "alpha")
                        {
                            drawingColorSettings.Alpha = new double?((double) element3.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                        }
                        else if (element3.Name.LocalName == "shade")
                        {
                            drawingColorSettings.Shade = new double?((double) element3.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                        }
                        else if (element3.Name.LocalName == "tint")
                        {
                            drawingColorSettings.Tint = new double?((double) element3.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        return new ExcelColor(ExcelColorType.RGB, ColorExtension.FromPresetColorVal(str2).ToArgb(), 0.0);
                    }
                }
                else
                {
                    if (node.Name.LocalName == "scrgbClr")
                    {
                        float val = node.GetAttributeValueOrDefaultOfFloatType("r", 0f);
                        float num5 = node.GetAttributeValueOrDefaultOfFloatType("g", 0f);
                        float num6 = node.GetAttributeValueOrDefaultOfFloatType("b", 0f);
                        foreach (XElement element4 in node.Elements())
                        {
                            if (element4.Name.LocalName == "alpha")
                            {
                                drawingColorSettings.Alpha = new double?((double) element4.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                            else if (element4.Name.LocalName == "shade")
                            {
                                drawingColorSettings.Shade = new double?((double) element4.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                            else if (element4.Name.LocalName == "tint")
                            {
                                drawingColorSettings.Tint = new double?((double) element4.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                        }
                        return new ExcelColor(ExcelColorType.RGB, GcColor.FromArgb(0xff, ColorExtension.ScRgbTosRgb(val), ColorExtension.ScRgbTosRgb(num5), ColorExtension.ScRgbTosRgb(num6)).ToArgb(), 0.0);
                    }
                    if (node.Name.LocalName == "srgbClr")
                    {
                        string str3 = node.GetAttributeValueOrDefaultOfStringType("val", null);
                        foreach (XElement element5 in node.Elements())
                        {
                            if (element5.Name.LocalName == "alpha")
                            {
                                drawingColorSettings.Alpha = new double?((double) element5.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                            else if (element5.Name.LocalName == "shade")
                            {
                                drawingColorSettings.Shade = new double?((double) element5.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                            else if (element5.Name.LocalName == "tint")
                            {
                                drawingColorSettings.Tint = new double?((double) element5.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                        }
                        if (!string.IsNullOrEmpty(str3))
                        {
                            uint num7 = 0;
                            if (uint.TryParse(str3, (NumberStyles) NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out num7))
                            {
                                return new ExcelColor(ExcelColorType.RGB, num7, 0.0);
                            }
                        }
                    }
                    else if (node.Name.LocalName == "sysClr")
                    {
                        GcColor color = new GcColor();
                        bool flag = ExcelSystemColor.TryGetSystemColor(node.GetAttributeValueOrDefaultOfStringType("val", null), out color);
                        foreach (XElement element6 in node.Elements())
                        {
                            if (element6.Name.LocalName == "alpha")
                            {
                                drawingColorSettings.Alpha = new double?((double) element6.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                            else if (element6.Name.LocalName == "shade")
                            {
                                drawingColorSettings.Shade = new double?((double) element6.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                            else if (element6.Name.LocalName == "tint")
                            {
                                drawingColorSettings.Tint = new double?((double) element6.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                            else if (element6.Name.LocalName == "lumMode")
                            {
                                drawingColorSettings.LumMod = new double?((double) element6.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                            }
                        }
                        if (flag)
                        {
                            return new ExcelColor(ExcelColorType.RGB, color.ToArgb(), 0.0);
                        }
                    }
                }
            }
            return null;
        }

        internal static void WriteColor(XmlWriter writer, IExcelColor color, ExcelDrawingColorSettings drawingColorSettings)
        {
            if (color != null)
            {
                if (drawingColorSettings == null)
                {
                    drawingColorSettings = new ExcelDrawingColorSettings();
                }
                if (color.ColorType == ExcelColorType.RGB)
                {
                    using (writer.WriteElement("srgbClr", null, "a"))
                    {
                        writer.WriteAttributeString("val", ((uint) color.Value).ToString("X8").Substring(2));
                        uint num = 0xff;
                        uint.TryParse(((uint) color.Value).ToString("X8").Substring(0, 2), (NumberStyles) NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out num);
                        double num2 = ((double) num) / 255.0;
                        if (!(num2 - 1.0).IsZero() && !num2.IsZero())
                        {
                            using (writer.WriteElement("alpha", null, "a"))
                            {
                                int num5 = (int)(num2 * 100000.0);
                                writer.WriteAttributeString("val", ((int) num5).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                goto Label_0138;
                            }
                        }
                        if ((drawingColorSettings != null) && drawingColorSettings.Alpha.HasValue)
                        {
                            writer.WriteLeafElementWithAttribute("alpha", null, "a", "val", ((double) drawingColorSettings.Alpha.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    Label_0138:
                        if (drawingColorSettings != null)
                        {
                            if (drawingColorSettings.Tint.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("tint", null, "a", "val", ((double) drawingColorSettings.Tint.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.Shade.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("shade", null, "a", "val", ((double) drawingColorSettings.Shade.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                        return;
                    }
                }
                if (color.ColorType == ExcelColorType.Theme)
                {
                    using (writer.WriteElement("schemeClr", null, "a"))
                    {
                        writer.WriteAttributeString("val", ((ColorSchemeIndex) color.Value).ToSchemeClrValue());
                        if (!color.Tint.IsZero())
                        {
                            if ((drawingColorSettings != null) && drawingColorSettings.LumMod.HasValue)
                            {
                                double num9 = color.Tint * 100000.0;
                                writer.WriteLeafElementWithAttribute("tint", null, "a", "val", ((double) num9).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            else if (color.Tint < 0.0)
                            {
                                double num10 = (1.0 + color.Tint) * 100000.0;
                                writer.WriteLeafElementWithAttribute("lumMod", null, "a", "val", ((double) num10).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                double num11 = color.Tint * 100000.0;
                                writer.WriteLeafElementWithAttribute("tint", null, "a", "val", ((double) num11).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                        else if ((drawingColorSettings != null) && drawingColorSettings.Tint.HasValue)
                        {
                            writer.WriteLeafElementWithAttribute("tint", null, "a", "val", ((double) drawingColorSettings.Tint.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (drawingColorSettings != null)
                        {
                            if (drawingColorSettings.Alpha.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("alpha", null, "a", "val", ((double) drawingColorSettings.Alpha.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.Shade.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("shade", null, "a", "val", ((double) drawingColorSettings.Shade.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.Sat.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("sat", null, "a", "val", ((double) drawingColorSettings.Sat.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.SatMod.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("satMod", null, "a", "val", ((double) drawingColorSettings.SatMod.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.SatOff.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("satOff", null, "a", "val", ((double) drawingColorSettings.SatOff.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.Hue.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("hue", null, "a", "val", ((double) drawingColorSettings.Hue.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.HueMod.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("hueMod", null, "a", "val", ((double) drawingColorSettings.HueMod.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.HueOff.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("hueOff", null, "a", "val", ((double) drawingColorSettings.HueOff.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.Lum.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("lum", null, "a", "val", ((double) drawingColorSettings.Lum.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.LumMod.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("lumMod", null, "a", "val", ((double) drawingColorSettings.LumMod.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (drawingColorSettings.LumOff.HasValue)
                            {
                                writer.WriteLeafElementWithAttribute("lumOff", null, "a", "val", ((double) drawingColorSettings.LumOff.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                        return;
                    }
                }
                ExcelColorType colorType = color.ColorType;
            }
        }
    }
}

