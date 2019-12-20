#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal static class MeasureHelper
    {
        private static bool _cachedAllowWrap = false;
        private static FontFamily _cachedFontFamily = null;
        private static double _cachedFontSize = 0.0;
        private static FontStretch _cachedFontStretch = FontStretch.Undefined;
        private static FontStyle _cachedFontStyle = FontStyle.Normal;
        private static FontWeight _cachedFontWeight = new FontWeight();
        private static bool _cachedUseLayoutRounding = false;
        [ThreadStatic]
        private static TextBlock _measure = null;
        private static Windows.UI.Xaml.Thickness ExcelCellBlankThickness = new Windows.UI.Xaml.Thickness(3.0, 3.3, 3.0, 3.0);
        private static Windows.UI.Xaml.Thickness TextBlockBlankThickness = new Windows.UI.Xaml.Thickness(1.0, 3.0, 1.0, 3.0);
        private static Windows.UI.Xaml.Thickness TextBoxBlankThickness = new Windows.UI.Xaml.Thickness(4.0, 6.0, 5.0, 7.0);

        public static Windows.Foundation.Size ConvertExcelCellSizeToTextSize(Windows.Foundation.Size excelCellSize, double zoomFactor)
        {
            double width = excelCellSize.Width;
            double height = excelCellSize.Height;
            Windows.UI.Xaml.Thickness excelBlank = GetExcelBlank();
            double num3 = excelBlank.Left + excelBlank.Right;
            if (width >= num3)
            {
                width -= num3;
            }
            else
            {
                width = 0.0;
            }
            double num4 = excelBlank.Top + excelBlank.Bottom;
            if (height >= num4)
            {
                height -= num4;
            }
            else
            {
                height = 0.0;
            }
            return new Windows.Foundation.Size(width, height);
        }

        public static Windows.Foundation.Size ConvertTextBlockSizeToTextSize(Windows.Foundation.Size textBlockSize, double zoomFactor, double fontSize)
        {
            double width = textBlockSize.Width;
            double height = textBlockSize.Height;
            Windows.UI.Xaml.Thickness textBlockBlank = GetTextBlockBlank(fontSize);
            double num3 = textBlockBlank.Left + textBlockBlank.Right;
            if (width >= num3)
            {
                width -= num3;
            }
            else
            {
                width = 0.0;
            }
            double num4 = textBlockBlank.Top + textBlockBlank.Bottom;
            num4 *= zoomFactor;
            if (height >= num4)
            {
                height -= num4;
            }
            else
            {
                height = 0.0;
            }
            return new Windows.Foundation.Size(width, height);
        }

        public static Windows.Foundation.Size ConvertTextSizeToExcelCellSize(Windows.Foundation.Size textSize, double zoomFactor)
        {
            double width = textSize.Width;
            double height = textSize.Height;
            Windows.UI.Xaml.Thickness excelBlank = GetExcelBlank();
            width += excelBlank.Left + excelBlank.Right;
            return new Windows.Foundation.Size(width, height + (excelBlank.Top + excelBlank.Bottom));
        }

        internal static Windows.UI.Xaml.Thickness GetExcelBlank()
        {
            double left = ExcelCellBlankThickness.Left;
            double right = ExcelCellBlankThickness.Right;
            double top = ExcelCellBlankThickness.Top;
            return new Windows.UI.Xaml.Thickness(left, top, right, ExcelCellBlankThickness.Bottom);
        }

        private static TextBlock GetTextBlock()
        {
            if (_measure == null)
            {
                _measure = new TextBlock();
            }
            return _measure;
        }

        internal static Windows.UI.Xaml.Thickness GetTextBlockBlank(double fontSize)
        {
            return TextBlockBlankThickness;
        }

        internal static Windows.UI.Xaml.Thickness GetTextBoxBlank(double fontSize)
        {
            return TextBoxBlankThickness;
        }

        internal static Windows.Foundation.Size MeasureCustomerTextInCell(Cell cell, string text, Windows.Foundation.Size maxSize, double zoomFactor, FontFamily unknownFontfamily, object textFormattingMode, bool useLayoutRounding)
        {
            FontFamily actualFontFamily = cell.ActualFontFamily;
            if (actualFontFamily == null)
            {
                if (unknownFontfamily != null)
                {
                    actualFontFamily = unknownFontfamily;
                }
                else
                {
                    actualFontFamily = GetTextBlock().FontFamily;
                }
            }
            bool cache = cell.CacheStyleObject(true);
            Windows.Foundation.Size size = MeasureText(text, actualFontFamily, cell.ActualFontSize * zoomFactor, cell.ActualFontStretch, cell.ActualFontStyle, cell.ActualFontWeight, maxSize, cell.ActualWordWrap, textFormattingMode, useLayoutRounding, zoomFactor);
            cell.CacheStyleObject(cache);
            size.Width += cell.ActualTextIndent * zoomFactor;
            return size;
        }

        public static Windows.Foundation.Size MeasureText(string text, FontFamily fontFamily, double fontSize, FontStretch fontStretch, FontStyle fontStyle, FontWeight fontWeight, Windows.Foundation.Size maxSize, bool allowWrap, object textFormattingMode, bool useLayoutRounding, double zoomFactor)
        {
            return ConvertTextBlockSizeToTextSize(MeasureTextBlock(text, fontFamily, fontSize, fontStretch, fontStyle, fontWeight, maxSize, allowWrap, textFormattingMode, useLayoutRounding, zoomFactor), zoomFactor, fontSize);
        }

        public static Windows.Foundation.Size MeasureTextBlock(string text, FontFamily fontFamily, double fontSize, FontStretch fontStretch, FontStyle fontStyle, FontWeight fontWeight, Windows.Foundation.Size maxSize, bool allowWrap, object textFormattingMode, bool useLayoutRounding, double zoomFactor)
        {
            Windows.Foundation.Size size = new Windows.Foundation.Size(0.0, 0.0);
            if (string.IsNullOrEmpty(text))
            {
                return size;
            }
            TextBlock textBlock = GetTextBlock();
            if (fontFamily != _cachedFontFamily)
            {
                _cachedFontFamily = fontFamily;
                textBlock.FontFamily = fontFamily;
            }
            if (fontSize != _cachedFontSize)
            {
                if (fontSize > 0.0)
                {
                    _cachedFontSize = fontSize;
                    textBlock.FontSize = fontSize;
                }
                else
                {
                    if (fontSize == 0.0)
                    {
                        return new Windows.Foundation.Size(0.0, 0.0);
                    }
                    _cachedFontSize = 0.0;
                    textBlock.ClearValue(TextBlock.FontSizeProperty);
                }
            }
            if (fontStretch != _cachedFontStretch)
            {
                _cachedFontStretch = fontStretch;
                textBlock.FontStretch = fontStretch;
            }
            if (fontStyle != _cachedFontStyle)
            {
                _cachedFontStyle = fontStyle;
                textBlock.FontStyle = fontStyle;
            }
            if (fontWeight.Weight != _cachedFontWeight.Weight)
            {
                _cachedFontWeight = fontWeight;
                textBlock.FontWeight = fontWeight;
            }
            if (allowWrap != _cachedAllowWrap)
            {
                _cachedAllowWrap = allowWrap;
                if (allowWrap)
                {
                    textBlock.TextWrapping = TextWrapping.Wrap;
                }
                else
                {
                    textBlock.TextWrapping = TextWrapping.NoWrap;
                }
            }
            if (_cachedUseLayoutRounding != useLayoutRounding)
            {
                _cachedUseLayoutRounding = useLayoutRounding;
                textBlock.UseLayoutRounding = useLayoutRounding;
            }
            if (text.EndsWith(" "))
            {
                text = text.Substring(0, text.Length - 1) + "\x00a0";
            }
            textBlock.Text = text;
            textBlock.Measure(maxSize);
            return textBlock.DesiredSize;
        }

        public static Windows.Foundation.Size MeasureTextInCell(Cell cell, Windows.Foundation.Size maxSize, double zoomFactor, FontFamily unknownFontfamily, object textFormattingMode, bool useLayoutRounding)
        {
            FontFamily actualFontFamily = cell.ActualFontFamily;
            if (actualFontFamily == null)
            {
                if (unknownFontfamily != null)
                {
                    actualFontFamily = unknownFontfamily;
                }
                else
                {
                    actualFontFamily = GetTextBlock().FontFamily;
                }
            }
            bool cache = cell.CacheStyleObject(true);
            Windows.Foundation.Size size = MeasureText(cell.Text, actualFontFamily, cell.ActualFontSize * zoomFactor, cell.ActualFontStretch, cell.ActualFontStyle, cell.ActualFontWeight, maxSize, cell.ActualWordWrap, textFormattingMode, useLayoutRounding, zoomFactor);
            cell.CacheStyleObject(cache);
            size.Width += cell.ActualTextIndent * zoomFactor;
            return size;
        }
    }
}

