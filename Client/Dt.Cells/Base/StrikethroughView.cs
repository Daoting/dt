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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class StrikethroughView : Panel
    {
        Cell _bindingCell;
        Border _border;
        CellBackgroundPanel _cellBackgroundPanel;
        Canvas _lineContainer;
        const int _ViewMargin = 1;

        public StrikethroughView(Cell bindingCell, CellBackgroundPanel backPanel)
        {
            base.Margin = new Windows.UI.Xaml.Thickness(1.0);
            _bindingCell = bindingCell;
            _border = new Border();
            _lineContainer = new Canvas();
            _cellBackgroundPanel = backPanel;
            base.Children.Add(_border);
        }

        protected override Size ArrangeOverride(Size availableSize)
        {
            Size size = new Size(availableSize.Width, availableSize.Height);
            foreach (UIElement element in _cellBackgroundPanel.Children)
            {
                if (element is TextBlock)
                {
                    size.Width = (element as TextBlock).DesiredSize.Width;
                    foreach (Line line in _lineContainer.Children)
                    {
                        line.Stroke = (element as TextBlock).Foreground;
                    }
                    _border.HorizontalAlignment = (element as TextBlock).HorizontalAlignment;
                    break;
                }
            }
            _border.Arrange(new Rect(new Point(0.0, 0.0), availableSize));
            if (_bindingCell.Worksheet.Workbook.CanCellOverflow)
            {
                double width = Math.Max(availableSize.Width, size.Width);
                RectangleGeometry geometry = new RectangleGeometry();
                geometry.Rect = new Rect(new Point(0.0, 0.0), new Size(width, availableSize.Height));
                base.Clip = geometry;
            }
            else
            {
                RectangleGeometry geometry2 = new RectangleGeometry();
                geometry2.Rect = new Rect(new Point(0.0, 0.0), availableSize);
                base.Clip = geometry2;
            }
            return base.ArrangeOverride(availableSize);
        }

        double GetLineHeight(Cell cell)
        {
            TextBlock block = new TextBlock();
            block.TextWrapping = TextWrapping.NoWrap;
            block.Text = Regex.Replace(cell.Text, @"[\n\r]", "");
            return MeasureHelper.MeasureText(block.Text, cell.ActualFontFamily, cell.ActualFontSize * cell.Worksheet.ZoomFactor, cell.ActualFontStretch, cell.ActualFontStyle, cell.ActualFontWeight, new Size(double.PositiveInfinity, double.PositiveInfinity), cell.ActualWordWrap, null, false, (double) cell.Worksheet.ZoomFactor).Height;
        }

        double GetLineSpacing(Cell cell)
        {
            string text = "ABCabABC";
            string str2 = "ABC\r\nAB";
            Size size = MeasureHelper.MeasureText(text, cell.ActualFontFamily, cell.ActualFontSize * cell.Worksheet.ZoomFactor, cell.ActualFontStretch, cell.ActualFontStyle, cell.ActualFontWeight, new Size(double.PositiveInfinity, double.PositiveInfinity), cell.ActualWordWrap, null, false, (double) cell.Worksheet.ZoomFactor);
            return (MeasureHelper.MeasureText(str2, cell.ActualFontFamily, cell.ActualFontSize * cell.Worksheet.ZoomFactor, cell.ActualFontStretch, cell.ActualFontStyle, cell.ActualFontWeight, new Size(double.PositiveInfinity, double.PositiveInfinity), cell.ActualWordWrap, null, false, (double) cell.Worksheet.ZoomFactor).Height - (size.Height * 2.0));
        }

        static Size GetTextSize(Cell cell)
        {
            Size maxSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            if (cell.ActualWordWrap)
            {
                CellRange range = cell.Worksheet.GetSpanCell(cell.Row.Index, cell.Column.Index, cell.SheetArea);
                if ((range != null) && ((range.Row < cell.Row.Index) || (range.RowCount > 1)))
                {
                    return new Size(0.0, 0.0);
                }
                double width = 0.0;
                if (range == null)
                {
                    width = cell.Worksheet.GetColumnWidth(cell.Column.Index, cell.SheetArea);
                }
                else
                {
                    for (int i = 0; i < range.ColumnCount; i++)
                    {
                        width += cell.Worksheet.GetColumnWidth(cell.Column.Index + i, cell.SheetArea);
                    }
                }
                width *= cell.Worksheet.ZoomFactor;
                maxSize = MeasureHelper.ConvertExcelCellSizeToTextSize(new Size(width, double.PositiveInfinity), 1.0);
            }
            return MeasureHelper.MeasureTextInCell(cell, maxSize, (double) cell.Worksheet.ZoomFactor, cell.ActualFontFamily, null, false);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _border.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        public void SetLines(float zoomFactor, Cell cell)
        {
            if (!cell.ActualStrikethrough || string.IsNullOrEmpty(cell.Text))
            {
                List<Line> list = new List<Line>();
                foreach (UIElement element in base.Children)
                {
                    if (element is Line)
                    {
                        list.Add(element as Line);
                    }
                }
                foreach (Line line in list)
                {
                    base.Children.Remove(line);
                }
            }
            else
            {
                double lineSpacing = GetLineSpacing(cell);
                double lineHeight = GetLineHeight(cell);
                Size textSize = GetTextSize(cell);
                double a = (textSize.Height + lineSpacing) / (lineHeight + lineSpacing);
                int num4 = (int) Math.Round(a);
                double width = textSize.Width;
                Windows.UI.Xaml.Thickness excelBlank = MeasureHelper.GetExcelBlank();
                _lineContainer.Height = textSize.Height + ((excelBlank.Top + excelBlank.Bottom) * cell.Worksheet.ZoomFactor);
                _lineContainer.Width = textSize.Width + ((excelBlank.Left + excelBlank.Right) * cell.Worksheet.ZoomFactor);
                if (num4 > 0)
                {
                    for (int i = 0; i < num4; i++)
                    {
                        Line line2 = new Line();
                        line2.StrokeThickness = 1.0;
                        line2.Stroke = cell.ActualForeground;
                        line2.X1 = excelBlank.Left * cell.Worksheet.ZoomFactor;
                        line2.Y1 = ((((lineHeight + lineSpacing) * i) + (lineHeight / 2.0)) + (excelBlank.Top * cell.Worksheet.ZoomFactor)) + 2.0;
                        line2.X2 = excelBlank.Left + width;
                        line2.Y2 = line2.Y1;
                        _lineContainer.Children.Add(line2);
                        line2.Stroke = cell.ActualForeground;
                    }
                }
                _border.Child = _lineContainer;
                HorizontalAlignment left = HorizontalAlignment.Left;
                switch (cell.ActualHorizontalAlignment)
                {
                    case CellHorizontalAlignment.Center:
                        left = HorizontalAlignment.Center;
                        break;

                    case CellHorizontalAlignment.Right:
                        left = HorizontalAlignment.Right;
                        break;
                }
                _lineContainer.HorizontalAlignment = left;
                _border.HorizontalAlignment = left;
                VerticalAlignment alignment2 = cell.ActualVerticalAlignment.ToVerticalAlignment();
                _lineContainer.VerticalAlignment = alignment2;
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        public Canvas LineContainer
        {
            get { return  _lineContainer; }
        }
    }
}

