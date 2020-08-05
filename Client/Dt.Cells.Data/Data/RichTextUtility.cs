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
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal static class RichTextUtility
    {
        internal static TextParagraph GetFirstParagraph(List<TextParagraph> richText)
        {
            if ((richText != null) && (richText.Count > 0))
            {
                return richText[0];
            }
            return null;
        }

        internal static TextRun GetFirstRun(List<TextParagraph> richText)
        {
            if ((richText != null) && (richText.Count > 0))
            {
                TextParagraph paragraph = richText[0];
                if ((paragraph != null) && (paragraph.TextRuns.Count > 0))
                {
                    return paragraph.TextRuns[0];
                }
            }
            return null;
        }

        internal static FontFamily GetRichTextFamily(List<TextParagraph> richText)
        {
            GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            if ((firstParagraph != null) && !string.IsNullOrWhiteSpace(firstParagraph.LatinFontFamily))
            {
                return new FontFamily(firstParagraph.LatinFontFamily);
            }
            return DefaultStyleCollection.DefaultFontFamily;
        }

        internal static Brush GetRichTextFill(List<TextParagraph> richText, Workbook workbook)
        {
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            Brush brush = null;
            IFillFormat fillFormat = null;
            if (firstRun != null)
            {
                fillFormat = firstRun.FillFormat;
            }
            if ((fillFormat == null) && (firstParagraph != null))
            {
                fillFormat = firstParagraph.FillFormat;
            }
            if (fillFormat != null)
            {
                object fill = ExcelChartExtension.GetFill(fillFormat, workbook);
                SpreadDrawingColorSettings spreadDrawingColorSettings = ExcelChartExtension.GetSpreadDrawingColorSettings(fillFormat);
                if (fill is string)
                {
                    if (workbook != null)
                    {
                        Windows.UI.Color color = Dt.Cells.Data.ColorHelper.UpdateColor(workbook.GetThemeColor((string) (fill as string)), spreadDrawingColorSettings, false);
                        return new SolidColorBrush(color);
                    }
                    return brush;
                }
                if (fill is Brush)
                {
                    brush = fill as Brush;
                }
            }
            return brush;
        }

        internal static double? GetRichTextFontSize(List<TextParagraph> richText)
        {
            double? nullable = null;
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            if ((firstRun != null) && firstRun.FontSize.HasValue)
            {
                return new double?(UnitHelper.PointToPixel(firstRun.FontSize.Value));
            }
            if ((firstParagraph != null) && firstParagraph.FontSize.HasValue)
            {
                nullable = new double?(UnitHelper.PointToPixel(firstParagraph.FontSize.Value));
            }
            return nullable;
        }

        internal static FontStyle GetRichTextFontStyle(List<TextParagraph> richText)
        {
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            FontStyle normal = FontStyle.Normal;
            if (((firstRun != null) && firstRun.Italics.HasValue) && firstRun.Italics.Value)
            {
                return FontStyle.Italic;
            }
            if (((firstParagraph != null) && firstParagraph.Italics.HasValue) && firstParagraph.Italics.Value)
            {
                normal = FontStyle.Italic;
            }
            return normal;
        }

        internal static FontWeight GetRichTextFontWeight(List<TextParagraph> richText)
        {
            return GetRichTextFontWeight(richText, FontWeights.Normal);
        }

        internal static FontWeight GetRichTextFontWeight(List<TextParagraph> richText, FontWeight defaultFontWeight)
        {
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            FontWeight bold = defaultFontWeight;
            if (((firstRun != null) && firstRun.Bold.HasValue) && firstRun.Bold.Value)
            {
                return FontWeights.Bold;
            }
            if (((firstParagraph != null) && firstParagraph.Bold.HasValue) && firstParagraph.Bold.Value)
            {
                bold = FontWeights.Bold;
            }
            return bold;
        }

        internal static void SetFontFamily(List<TextParagraph> richText, string fontFamily)
        {
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            if ((firstParagraph != null) && !string.IsNullOrEmpty(fontFamily))
            {
                firstParagraph.LatinFontFamily = fontFamily;
            }
        }

        internal static void SetRichTextFill(List<TextParagraph> richText, Brush fillBrush)
        {
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            if ((firstRun != null) && (fillBrush != null))
            {
                firstRun.FillFormat = fillBrush.ToExcelFillFormat();
            }
            else if ((firstParagraph != null) && (fillBrush != null))
            {
                firstParagraph.FillFormat = fillBrush.ToExcelFillFormat();
            }
        }

        internal static void SetRichTextFontSize(List<TextParagraph> richText, double fontSize)
        {
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            if ((firstRun != null) && (fontSize > 0.0))
            {
                firstRun.FontSize = new double?(UnitHelper.PixelToPoint(fontSize));
            }
            else if ((firstParagraph != null) && (fontSize > 0.0))
            {
                firstParagraph.FontSize = new double?(UnitHelper.PixelToPoint(fontSize));
            }
        }

        internal static void SetRichTextFontStyle(List<TextParagraph> richText, bool italics)
        {
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            if ((firstRun != null) && italics)
            {
                firstRun.Italics = new bool?(italics);
            }
            else if ((firstParagraph != null) && italics)
            {
                firstParagraph.Italics = new bool?(italics);
            }
        }

        internal static void SetRichtTextFontWeight(List<TextParagraph> richText, bool bold)
        {
            TextRun firstRun = GetFirstRun(richText);
            TextParagraph firstParagraph = GetFirstParagraph(richText);
            FontWeight normal = FontWeights.Normal;
            if (firstRun != null)
            {
                firstRun.Bold = new bool?(bold);
            }
            else if (firstParagraph != null)
            {
                firstParagraph.Bold = new bool?(bold);
            }
        }
    }
}

