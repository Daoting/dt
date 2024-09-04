#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
#endregion

namespace Dt.Xls.Biff
{
    internal class ConverterFactory
    {
        internal static ExcelFont GetExcelFont(FONTRecord record)
        {
            return new ExcelFont { CharSetIndex = record.CharacterSet, FontColor = new ExcelColor(ExcelColorType.Indexed, record.ColorIndex, 0.0), FontFamily = (ExcelFontFamily)record.FontFamily, FontName = record.FontName, FontSize = record.FontHeight, IsBold = record.IsBold, IsItalic = record.IsItalic, IsOutlineStyle = record.IsOutline, IsShadowStyle = record.IsShadow, IsStrikeOut = record.IsStrikeOut, UnderLineStyle = (UnderLineStyle)record.UnderlineStyle, VerticalAlignRun = (VerticalAlignRun)(byte)record.SuperScript };
        }

        internal static ExtendedFormat GetExcelStyle(XFRecrod xf, ExcelFont font)
        {
            ExcelBorder border2 = new ExcelBorder();
            ExcelBorderSide side = new ExcelBorderSide {
                Color = new ExcelColor(ExcelColorType.Indexed, xf.LeftBorderColorIndex, 0.0),
                LineStyle = (ExcelBorderStyle) xf.LeftBorderLine
            };
            border2.Left = side;
            ExcelBorderSide side2 = new ExcelBorderSide {
                Color = new ExcelColor(ExcelColorType.Indexed, xf.RightBorderColorIndex, 0.0),
                LineStyle = (ExcelBorderStyle) xf.RightBorderLine
            };
            border2.Right = side2;
            ExcelBorderSide side3 = new ExcelBorderSide {
                Color = new ExcelColor(ExcelColorType.Indexed, xf.TopBorderColorIndex, 0.0),
                LineStyle = (ExcelBorderStyle) xf.TopBorderLine
            };
            border2.Top = side3;
            ExcelBorderSide side4 = new ExcelBorderSide {
                Color = new ExcelColor(ExcelColorType.Indexed, xf.BottomBorderColorIndex, 0.0),
                LineStyle = (ExcelBorderStyle) xf.BottomBorderLine
            };
            border2.Bottom = side4;
            ExcelBorder border = border2;
            ExtendedFormat format = new ExtendedFormat {
                Font = font,
                Border = border,
                PatternBackgroundColor = new ExcelColor(ExcelColorType.Indexed, xf.FillPatternBackgroundColor, 0.0),
                PatternColor = new ExcelColor(ExcelColorType.Indexed, xf.FillPatternColor, 0.0),
                FillPattern = (FillPatternType) xf.FillPatternIndex,
                HorizontalAlign = (ExcelHorizontalAlignment) xf.HAlignment,
                VerticalAlign = (ExcelVerticalAlignment) xf.VAlignment,
                Indent = xf.IndentLevel,
                IsHidden = xf.IsHidden,
                ApplyAlignment = xf.ApplyAlignment,
                ApplyBorder = xf.ApplyBorder,
                ApplyFont = xf.ApplyFont,
                ApplyFill = xf.ApplyFill,
                ApplyNumberFormat = xf.ApplyNumberFormat,
                ApplyProtection = xf.ApplyProtection,
                IsStyleFormat = xf.IsStyleXF,
                IsJustfyLastLine = xf.JustifyLastCharacter,
                IsLocked = xf.IsLocked,
                IsShrinkToFit = xf.IsShrinkContent,
                IsWordWrap = xf.IsWordWrap,
                ReadingOrder = (TextDirection) xf.Direction,
                Rotation = xf.TextRotation,
                IsFirstSymbolApostrophe = xf.IsF123Prefix
            };
            if (!format.IsStyleFormat)
            {
                format.ParentFormatID = new int?(xf.ParentXFIndex);
            }
            return format;
        }

        internal static FONTRecord GetFontBiffRecord(IExcelFont font, IExcelWriter writer)
        {
            return new FONTRecord { CharacterSet = font.CharSetIndex, ColorIndex = (ushort) writer.GetPaletteColor(font.FontColor), FontFamily = (byte) font.FontFamily, FontName = font.FontName, FontHeight = (ushort) (font.FontSize * 20.0), IsBold = font.IsBold, IsItalic = font.IsItalic, IsOutline = font.IsOutlineStyle, IsShadow = font.IsShadowStyle, IsStrikeOut = font.IsStrikeOut, UnderlineStyle = (byte) font.UnderLineStyle, SuperScript = (ushort) font.VerticalAlignRun };
        }

        internal static XFRecrod GetXFBiffRecord(IExtendedFormat style, ushort fontID, IExcelWriter writer)
        {
            XFRecrod recrod = new XFRecrod {
                FontIndex = fontID,
                FillPatternBackgroundColor = (byte) writer.GetPaletteColor(style.PatternBackgroundColor),
                FillPatternColor = (byte) writer.GetPaletteColor(style.PatternColor),
                FillPatternIndex = (byte) style.FillPattern,
                HAlignment = (byte) style.HorizontalAlign,
                VAlignment = (byte) style.VerticalAlign,
                IndentLevel = style.Indent,
                IsF123Prefix = style.IsFirstSymbolApostrophe,
                IsHidden = style.IsHidden,
                JustifyLastCharacter = style.IsJustfyLastLine,
                IsLocked = style.IsLocked,
                IsShrinkContent = style.IsShrinkToFit,
                IsWordWrap = style.IsWordWrap,
                Direction = (byte) style.ReadingOrder,
                TextRotation = (byte) style.Rotation,
                LeftBorderColorIndex = (byte) writer.GetPaletteColor(style.Border.Left.Color),
                LeftBorderLine = (byte) style.Border.Left.LineStyle,
                RightBorderColorIndex = (byte) writer.GetPaletteColor(style.Border.Right.Color),
                RightBorderLine = (byte) style.Border.Right.LineStyle,
                TopBorderColorIndex = (byte) writer.GetPaletteColor(style.Border.Top.Color),
                TopBorderLine = (byte) style.Border.Top.LineStyle,
                BottomBorderColorIndex = (byte) writer.GetPaletteColor(style.Border.Bottom.Color),
                BottomBorderLine = (byte) style.Border.Bottom.LineStyle,
                ApplyAlignment = style.ApplyAlignment,
                ApplyBorder = style.ApplyBorder,
                ApplyFont = style.ApplyFont,
                ApplyFill = style.ApplyFill,
                ApplyNumberFormat = style.ApplyNumberFormat,
                ApplyProtection = style.ApplyProtection
            };
            if ((style is ExtendedFormat) && (style as ExtendedFormat).IsStyleFormat)
            {
                recrod.IsStyleXF = true;
                recrod.ParentXFIndex = 0xfff;
            }
            else
            {
                recrod.IsStyleXF = false;
                if (style.ParentFormatID.HasValue)
                {
                    recrod.ParentXFIndex = (ushort) style.ParentFormatID.Value;
                }
                else
                {
                    recrod.ParentXFIndex = 15;
                }
            }
            if (style.NumberFormat != null)
            {
                recrod.FormatIndex = (ushort) style.NumberFormat.NumberFormatId;
                return recrod;
            }
            recrod.FormatIndex = (ushort) style.NumberFormatIndex;
            return recrod;
        }
    }
}

