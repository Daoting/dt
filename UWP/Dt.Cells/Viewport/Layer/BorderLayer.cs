#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 调整为只用在内容区域，行/列头不再使用
    /// 因原代码在iOS中布局时造成死循环，已大改
    /// </summary>
    internal sealed partial class BorderLayer : Panel
    {
        readonly CellsPanel _owner;
        readonly List<int> _visibleRows = new List<int>();
        readonly List<int> _visibleCols = new List<int>();
        readonly List<CombinLine> _lines = new List<CombinLine>();
        int _recycledStart;
        int _rowStart;
        int _rowEnd;
        int _columnEnd;
        int _columnStart;
        BorderLine _gridLine;
        int _viewportBottomRow = -1;
        int _viewportRightColumn = -1;
        Point _location;

        readonly Dictionary<ulong, BorderLine> _hBorderLineCache = new Dictionary<ulong, BorderLine>();
        readonly Dictionary<ulong, BorderLine> _vBorderLineCache = new Dictionary<ulong, BorderLine>();
        readonly Dictionary<ulong, Rect> _cellBoundsCache = new Dictionary<ulong, Rect>();
        static Dictionary<Windows.UI.Color, SolidColorBrush> _brushCache = new Dictionary<Windows.UI.Color, SolidColorBrush>();

        public BorderLayer(CellsPanel viewport)
        {
            _owner = viewport;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner.Excel._fastScroll)
                return availableSize;

            var sheet = _owner.Excel.ActiveSheet;
            _gridLine = sheet.GetGridLine(SheetArea.Cells);
            _recycledStart = 0;
            _location = _owner.PointToClient(new Point());
            _lines.Clear();
            _vBorderLineCache.Clear();
            _hBorderLineCache.Clear();
            _cellBoundsCache.Clear();

            CalcVisibleRowColumnIndexes();
            BuildHorizontalBorders();
            BuildVerticalBorders();
            LinkBorders(availableSize);
            RecycleBorders();

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rc = new Rect(new Point(), finalSize);
            foreach (UIElement elem in Children)
            {
                elem.Arrange(rc);
            }
            return finalSize;
        }

        void CalcVisibleRowColumnIndexes()
        {
            _rowStart = _owner.Excel.GetViewportTopRow(_owner.RowViewportIndex);
            _columnStart = _owner.Excel.GetViewportLeftColumn(_owner.ColumnViewportIndex);
            _viewportBottomRow = _owner.Excel.GetViewportBottomRow(_owner.RowViewportIndex);
            _rowEnd = _viewportBottomRow;
            _viewportRightColumn = _owner.Excel.GetViewportRightColumn(_owner.ColumnViewportIndex);
            _columnEnd = _viewportRightColumn;

            var sheet = _owner.Excel.ActiveSheet;
            _visibleRows.Clear();
            _visibleCols.Clear();

            if ((_rowStart <= _rowEnd) && (_columnStart <= _columnEnd))
            {
                int num = -1;
                for (int i = _rowStart - 1; i > -1; i--)
                {
                    if (sheet.GetActualRowVisible(i, SheetArea.Cells))
                    {
                        num = i;
                        break;
                    }
                }
                _rowStart = num;

                num = -1;
                for (int i = _columnStart - 1; i > -1; i--)
                {
                    if (sheet.GetActualColumnVisible(i, SheetArea.Cells))
                    {
                        num = i;
                        break;
                    }
                }
                _columnStart = num;

                int count = _owner.GetDataContext().Rows.Count;
                for (int i = _rowEnd + 1; i < count; i++)
                {
                    if (sheet.GetActualRowVisible(i, SheetArea.Cells))
                    {
                        _rowEnd = i;
                        break;
                    }
                }

                count = _owner.GetDataContext().Columns.Count;
                for (int i = _columnEnd + 1; i < count; i++)
                {
                    if (sheet.GetActualColumnVisible(i, SheetArea.Cells))
                    {
                        _columnEnd = i;
                        break;
                    }
                }

                for (int i = _rowStart; i <= _rowEnd; i++)
                {
                    if (sheet.GetActualRowVisible(i, SheetArea.Cells))
                    {
                        _visibleRows.Add(i);
                    }
                }

                for (int i = _columnStart; i <= _columnEnd; i++)
                {
                    if (sheet.GetActualColumnVisible(i, SheetArea.Cells))
                    {
                        _visibleCols.Add(i);
                    }
                }
            }
        }

        void BuildHorizontalBorders()
        {
            if (_visibleRows.Count == 0)
                return;

            for (int i = 0; i < _visibleRows.Count; i++)
            {
                int row = _visibleRows[i];
                if (row > _viewportBottomRow)
                    return;

                LineItem previousLineItem = null;
                BorderLine previousLine = null;
                BorderLine line2 = null;
                BorderLine line3 = null;
                int length = _visibleCols.Count;
                for (int j = 0; j < length; j++)
                {
                    int column = _visibleCols[j];
                    if ((j == (length - 1)) && (column > _viewportRightColumn))
                        break;

                    if ((row != -1) || (column != -1))
                        BuildBordersInternal(ref i, ref j, row, column, 0, ref previousLineItem, ref previousLine, ref line2, ref line3);
                }
            }
        }

        void BuildVerticalBorders()
        {
            if (_visibleCols.Count == 0)
                return;

            for (int i = 0; i < _visibleCols.Count; i++)
            {
                int column = _visibleCols[i];
                if (column > _viewportRightColumn)
                    return;

                LineItem previousLineItem = null;
                BorderLine previousLine = null;
                BorderLine line2 = null;
                BorderLine line3 = null;
                int length = _visibleRows.Count;
                for (int j = 0; j < length; j++)
                {
                    int row = _visibleRows[j];
                    if ((j == (length - 1)) && (row > _viewportBottomRow))
                        break;

                    if ((row != -1) || (column != -1))
                        BuildBordersInternal(ref j, ref i, row, column, 1, ref previousLineItem, ref previousLine, ref line2, ref line3);
                }
            }
        }

        void LinkBorders(Size availableSize)
        {
            foreach (var l in _lines)
            {
                var borderLine = l.Item.Line;
                if (borderLine.Style == BorderLineStyle.Double)
                {
                    if (l.Item.Direction == 0)
                    {
                        // 水平线
                        BorderLineLayoutEngine.CalcDoubleLayout(l.Item, -_location.X, -_location.Y, 0, out double num, out double num2, out double num3, out double num4, out double num5, out double num6, out double num7, out double num8, out double num9, out double num10, out double num11, out double num12);
                        l.Line1.X1 = num5;
                        l.Line1.X2 = num7;
                        l.Line2.X1 = num6;
                        l.Line2.X2 = num8;
                        l.Line1.Y1 = num3 - 1.0;
                        l.Line1.Y2 = num4 - 1.0;
                        l.Line2.Y1 = num3 + 1.0;
                        l.Line2.Y2 = num4 + 1.0;
                    }
                    else
                    {
                        BorderLineLayoutEngine.CalcDoubleLayout(l.Item, -_location.X, -_location.Y, 1, out double num, out double num2, out double num3, out double num4, out double num5, out double num6, out double num7, out double num8, out double num9, out double num10, out double num11, out double num12);
                        l.Line1.Y1 = num9;
                        l.Line1.Y2 = num11;
                        l.Line2.Y1 = num10;
                        l.Line2.Y2 = num12;
                        l.Line1.X1 = num - 1.0;
                        l.Line1.X2 = num2 - 1.0;
                        l.Line2.X1 = num + 1.0;
                        l.Line2.X2 = num2 + 1.0;
                    }
                }
                else if (borderLine.Style == BorderLineStyle.SlantedDashDot)
                {
                    if (l.Item.Direction == 0)
                    {
                        // 水平线
                        BorderLineLayoutEngine.CalcNormalLayout(l.Item, -_location.X, -_location.Y, 0, out double num, out double num2, out double num3, out double num4);
                        l.Line1.X1 = num;
                        l.Line1.X2 = num2;
                        l.Line1.Y1 = num3 - 1.0;
                        l.Line1.Y2 = num4 - 1.0;
                        l.Line2.X1 = num;
                        l.Line2.X2 = num2;
                        l.Line2.Y1 = num3;
                        l.Line2.Y2 = num4;
                        l.Line1.StrokeDashOffset = ((borderLine.StyleData.StrokeDashOffset + l.Line1.StrokeThickness) == 0.0) ? 0.0 : (((num - _location.X) / l.Line1.StrokeThickness) - 1.0);
                        l.Line2.StrokeDashOffset = ((borderLine.StyleData.StrokeDashOffset + l.Line2.StrokeThickness) == 0.0) ? 0.0 : ((num - _location.X) / l.Line2.StrokeThickness);
                    }
                    else
                    {
                        BorderLineLayoutEngine.CalcNormalLayout(l.Item, -_location.X, -_location.Y, 1, out double num, out double num2, out double num3, out double num4);
                        l.Line1.X1 = num - 1.0;
                        l.Line1.X2 = num2 - 1.0;
                        l.Line1.Y1 = num3;
                        l.Line1.Y2 = num4;
                        l.Line2.X1 = num;
                        l.Line2.X2 = num2;
                        l.Line2.Y1 = num3;
                        l.Line2.Y2 = num4;
                        l.Line1.StrokeDashOffset = ((borderLine.StyleData.StrokeDashOffset + l.Line1.StrokeThickness) == 0.0) ? 0.0 : (((num3 - _location.Y) / l.Line1.StrokeThickness) - 1.0);
                        l.Line2.StrokeDashOffset = ((borderLine.StyleData.StrokeDashOffset + l.Line2.StrokeThickness) == 0.0) ? 0.0 : ((num3 - _location.Y) / l.Line2.StrokeThickness);
                    }
                }
                else
                {
                    if (l.Item.Direction == 0)
                    {
                        // 水平线
                        BorderLineLayoutEngine.CalcNormalLayout(l.Item, -_location.X, -_location.Y, 0, out double num, out double num2, out double num3, out double num4);
                        l.Line1.X1 = num;
                        l.Line1.X2 = num2;
                        l.Line1.Y1 = num3;
                        l.Line1.Y2 = num4;
                        l.Line1.StrokeDashOffset = ((borderLine.StyleData.StrokeDashOffset + l.Line1.StrokeThickness) == 0.0) ? 0.0 : ((num - _location.X) / l.Line1.StrokeThickness);
                    }
                    else
                    {
                        BorderLineLayoutEngine.CalcNormalLayout(l.Item, -_location.X, -_location.Y, 1, out double num, out double num2, out double num3, out double num4);
                        l.Line1.X1 = num;
                        l.Line1.X2 = num2;
                        num3 += 0.0001;
                        l.Line1.Y1 = num3;
                        l.Line1.Y2 = num4;
                        l.Line1.StrokeDashOffset = ((borderLine.StyleData.StrokeDashOffset + l.Line1.StrokeThickness) == 0.0) ? 0.0 : ((num3 - _location.Y) / l.Line1.StrokeThickness);
                    }
                }

                l.Line1.Measure(availableSize);
                l.Line2?.Measure(availableSize);
            }
        }

        void RecycleBorders()
        {
            // 频繁增删Children子元素会出现卡顿现象！
            // 将多余的线画在外部
            for (int i = _recycledStart; i < Children.Count; i++)
            {
                Line line = (Line)Children[i];
                line.X1 = -1;
                line.X2 = -1;
                line.Y1 = -1;
                line.Y2 = -1;
            }
        }

        void BuildBordersInternal(ref int rIndex, ref int cIndex, int row, int column, int lineDirection, ref LineItem previousLineItem, ref BorderLine previousLine, ref BorderLine previousBreaker1, ref BorderLine previousBreaker2)
        {
            if ((row == -1) && (lineDirection == 1))
            {
                // 垂直线
                previousBreaker1 = GetBorderLine(rIndex, cIndex, 0, column, Borders.TOP);
                previousBreaker2 = GetBorderLine(rIndex, cIndex, 0, NextColumn(cIndex), Borders.TOP);
            }
            else if ((column != -1) || (lineDirection != 0))
            {
                BorderLine line;
                if (column == -1)
                {
                    line = GetBorderLine(rIndex, cIndex, row, 0, Borders.LEFT);
                }
                else if (row == -1)
                {
                    line = GetBorderLine(rIndex, cIndex, 0, column, Borders.TOP);
                }
                else
                {
                    Borders borderIndex = (lineDirection == 0) ? Borders.BOTTOM : Borders.RIGHT;
                    line = GetBorderLine(rIndex, cIndex, row, column, borderIndex);
                }

                bool flag = !BorderLineLayoutEngine.IsDoubleLine(line) && object.Equals(line, previousLine);
                if (flag)
                {
                    flag = BorderLine.Max(previousBreaker1, previousBreaker2) < line
                        || BorderLine.Max(previousBreaker1, previousBreaker2) == line;
                }

                if (flag && (IsDoubleLine(previousBreaker1) || IsDoubleLine(previousBreaker2)))
                {
                    flag = false;
                }

                LineItem item;
                if (flag)
                {
                    // 等同上一线
                    item = previousLineItem;
                    switch (lineDirection)
                    {
                        case 0:
                            previousLineItem.ColumnEnd = column;
                            break;

                        case 1:
                            previousLineItem.RowEnd = row;
                            break;
                    }
                }
                else
                {
                    item = new LineItem
                    {
                        Direction = lineDirection,
                        RowFrom = row,
                        RowEnd = row,
                        ColumnFrom = column,
                        ColumnEnd = column,
                        Line = line,
                        PreviousLine = previousLine,
                        PreviousBreaker1 = previousBreaker1,
                        PreviousBreaker2 = previousBreaker2
                    };

                    if (((item.Line != BorderLine.Empty)
                        && (item.Line != BorderLine.NoBorder))
                        && (!item.IsGridLine || (line.Color.A != 0)))
                    {
                        ((IThemeContextSupport)line).SetContext(_owner.Excel.ActiveSheet);
                        CreateLine(item);
                        ((IThemeContextSupport)line).SetContext(null);
                    }
                }

                switch (lineDirection)
                {
                    case 0:
                        if (row != -1)
                        {
                            item.NextLine = GetBorderLine(rIndex, cIndex, row, NextColumn(cIndex), Borders.BOTTOM);
                            item.NextBreaker1 = GetBorderLine(rIndex, cIndex, row, column, Borders.RIGHT);
                            item.NextBreaker2 = GetBorderLine(rIndex, cIndex, NextRow(rIndex), column, Borders.RIGHT);
                            break;
                        }
                        item.NextLine = GetBorderLine(rIndex, cIndex, 0, NextColumn(cIndex), Borders.TOP);
                        item.NextBreaker1 = _gridLine;
                        item.NextBreaker2 = GetBorderLine(rIndex, cIndex, 0, column, Borders.RIGHT);
                        break;

                    case 1:
                        if (column != -1)
                        {
                            item.NextLine = GetBorderLine(rIndex, cIndex, NextRow(rIndex), column, Borders.RIGHT);
                            item.NextBreaker1 = GetBorderLine(rIndex, cIndex, row, column, Borders.BOTTOM);
                            item.NextBreaker2 = GetBorderLine(rIndex, cIndex, row, NextColumn(cIndex), Borders.BOTTOM);
                            break;
                        }
                        item.NextLine = GetBorderLine(rIndex, cIndex, NextRow(rIndex), column, Borders.LEFT);
                        item.NextBreaker1 = _gridLine;
                        item.NextBreaker2 = GetBorderLine(rIndex, cIndex, row, 0, Borders.BOTTOM);
                        break;
                }

                ulong num = (ulong)row;
                num = num << 0x20;
                num |= (uint)column;

                Rect empty;
                if (!_cellBoundsCache.TryGetValue(num, out empty))
                {
                    empty = GetCellBounds(row, column);
                    _cellBoundsCache.Add(num, empty);
                }
                item.Bounds.Add(empty);

                previousLine = line;
                previousLineItem = item;
                previousBreaker1 = item.NextBreaker1;
                previousBreaker2 = item.NextBreaker2;
            }
        }

        void CreateLine(LineItem p_lineItem)
        {
            var borderLine = p_lineItem.Line;
            if (borderLine.Style == BorderLineStyle.Double)
            {
                Line line1 = PopLine();
                line1.StrokeThickness = 1.0;
                line1.Stroke = GetSolidBrush(borderLine.Color);
                ApplyDashArray(line1, borderLine.StyleData.FarDash, borderLine.StyleData.StrokeDashOffset);

                Line line2 = PopLine();
                line2.StrokeThickness = 1.0;
                line2.Stroke = GetSolidBrush(borderLine.Color);
                ApplyDashArray(line2, borderLine.StyleData.NearDash, borderLine.StyleData.StrokeDashOffset);
                _lines.Add(new CombinLine { Item = p_lineItem, Line1 = line1, Line2 = line2 });
            }
            else if (borderLine.Style == BorderLineStyle.SlantedDashDot)
            {
                Line line1 = PopLine();
                line1.StrokeThickness = 1.0;
                line1.Stroke = GetSolidBrush(borderLine.Color);
                ApplyDashArray(line1, borderLine.StyleData.FarDash, borderLine.StyleData.StrokeDashOffset - 1);

                Line line2 = PopLine();
                line2.StrokeThickness = 1.0;
                line2.Stroke = GetSolidBrush(borderLine.Color);
                ApplyDashArray(line2, borderLine.StyleData.MiddleDash, borderLine.StyleData.StrokeDashOffset);
                _lines.Add(new CombinLine { Item = p_lineItem, Line1 = line1, Line2 = line2 });
            }
            else
            {
                Line line = PopLine();
                line.StrokeThickness = borderLine.StyleData.DrawingThickness;
                line.Stroke = GetSolidBrush(borderLine.Color);
                ApplyDashArray(line, borderLine.StyleData.MiddleDash, borderLine.StyleData.StrokeDashOffset);
                _lines.Add(new CombinLine { Item = p_lineItem, Line1 = line });
            }
        }

        void ApplyDashArray(Line p_line, DoubleCollection p_value, int p_offset)
        {
            p_line.StrokeDashArray?.Clear();
            if (p_value != null)
            {
                if (p_line.StrokeDashArray == null)
                    p_line.StrokeDashArray = new DoubleCollection();
                foreach (double num in p_value)
                {
                    p_line.StrokeDashArray.Add(num);
                }
                p_line.StrokeDashOffset = p_offset;
            }
        }

        SolidColorBrush GetSolidBrush(Windows.UI.Color color)
        {
            SolidColorBrush brush;
            if (!_brushCache.TryGetValue(color, out brush))
            {
                brush = new SolidColorBrush(color);
                _brushCache.Add(color, brush);
            }
            return brush;
        }

        Line PopLine()
        {
            Line line;
            if (_recycledStart >= Children.Count)
            {
                line = new Line();
                Children.Add(line);
            }
            else
            {
                line = (Line)Children[_recycledStart];
            }
            _recycledStart++;
            return line;
        }

        BorderLine GetBorderLine(int rIndex, int cIndex, int row, int column, Borders borderIndex)
        {
            if ((row == -1) && (borderIndex != Borders.BOTTOM))
                return null;

            if ((column == -1) && (borderIndex != Borders.RIGHT))
                return null;

            ulong num = ConverIndexToKey(rIndex, cIndex, row, column, borderIndex);
            BorderLine noBorder;
            if ((borderIndex == Borders.LEFT) || (borderIndex == Borders.RIGHT))
            {
                if (_vBorderLineCache.TryGetValue(num, out noBorder))
                {
                    return noBorder;
                }
            }
            else if (_hBorderLineCache.TryGetValue(num, out noBorder))
            {
                return noBorder;
            }

            bool isInCellflow = false;
            noBorder = GetCellActualBorderLine(row, column, borderIndex, out isInCellflow);

            if (!isInCellflow)
            {
                BorderLine line2 = null;
                Borders borders = (Borders)(((int)borderIndex + 2) % 4);
                if (borderIndex == Borders.LEFT)
                {
                    line2 = GetCellActualBorderLine(row, PreviousColumn(cIndex), borders, out isInCellflow);
                }
                else if (borderIndex == Borders.TOP)
                {
                    line2 = GetCellActualBorderLine(PreviousRow(rIndex), column, borders, out isInCellflow);
                }
                else if (borderIndex == Borders.RIGHT)
                {
                    line2 = GetCellActualBorderLine(row, NextColumn(cIndex), borders, out isInCellflow);
                }
                else
                {
                    line2 = GetCellActualBorderLine(NextRow(rIndex), column, borders, out isInCellflow);
                }

                if (!IsDoubleLine(noBorder) && IsDoubleLine(line2))
                {
                    noBorder = line2;
                }
                else if ((!IsDoubleLine(noBorder) || IsDoubleLine(line2)) && (line2 > noBorder))
                {
                    noBorder = line2;
                }
            }

            if (noBorder == null)
            {
                noBorder = BorderLine.NoBorder;
            }

            if ((borderIndex == Borders.LEFT) || (borderIndex == Borders.RIGHT))
            {
                _vBorderLineCache.Add(num, noBorder);
            }
            else
            {
                _hBorderLineCache.Add(num, noBorder);
            }
            return noBorder;
        }

        BorderLine GetCellActualBorderLine(int row, int column, Borders borderIndex, out bool isInCellflow)
        {
            isInCellflow = false;
            bool flag = false;
            BorderLine empty = null;
            Cell cachedCell = null;
            byte state = _owner.CachedSpanGraph.GetState(row, column);

            switch (borderIndex)
            {
                case Borders.LEFT:
                    if (state <= 0)
                    {
                        CellOverflowLayoutModel model = _owner.CellOverflowLayoutBuildEngine.GetModel(row);
                        if (model != null)
                        {
                            CellOverflowLayout cellOverflowLayout = model.GetCellOverflowLayout(column);
                            if (cellOverflowLayout != null)
                            {
                                if (cellOverflowLayout.StartingColumn > -1)
                                {
                                    if (cellOverflowLayout.StartingColumn == column)
                                    {
                                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                    }
                                    else
                                    {
                                        empty = null;
                                        isInCellflow = true;
                                    }
                                }
                                else if (cellOverflowLayout.Column == column)
                                {
                                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                }
                                else
                                {
                                    empty = null;
                                    isInCellflow = true;
                                }
                            }
                            else
                            {
                                empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                            }
                        }
                        else
                        {
                            empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        }
                        break;
                    }
                    if ((state & 1) != 1)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.TOP:
                    if (state <= 0)
                    {
                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        break;
                    }
                    if ((state & 2) != 2)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.RIGHT:
                    if (state <= 0)
                    {
                        CellOverflowLayoutModel model2 = _owner.CellOverflowLayoutBuildEngine.GetModel(row);
                        if (model2 != null)
                        {
                            CellOverflowLayout layout2 = model2.GetCellOverflowLayout(column);
                            if (layout2 != null)
                            {
                                if (layout2.EndingColumn > -1)
                                {
                                    if (layout2.EndingColumn == column)
                                    {
                                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                    }
                                    else
                                    {
                                        empty = null;
                                        isInCellflow = true;
                                    }
                                }
                                else if (layout2.Column == column)
                                {
                                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                }
                                else
                                {
                                    empty = null;
                                    isInCellflow = true;
                                }
                            }
                            else
                            {
                                empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                            }
                        }
                        else
                        {
                            empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        }
                        break;
                    }
                    if ((state & 4) != 4)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.BOTTOM:
                    if (state <= 0)
                    {
                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        break;
                    }
                    if ((state & 8) != 8)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;
            }

            if ((!flag && !isInCellflow) && (empty == null))
            {
                if (_owner.Excel.ZoomFactor < 0.4f)
                {
                    return BorderLine.Empty;
                }

                empty = _gridLine;
                if ((state & 0x20) == 0x20)
                {
                    return BorderLine.Empty;
                }

                if (state != 0)
                {
                    return empty;
                }

                if (cachedCell == null)
                {
                    cachedCell = _owner.CellCache.GetCachedCell(row, column);
                }

                if ((cachedCell != null) && (cachedCell.ActualBackground != null))
                {
                    empty = BorderLine.Empty;
                }
            }
            return empty;
        }

        ulong ConverIndexToKey(int rIndex, int cIndex, int row, int column, Borders borderIndex)
        {
            ulong num = 0L;
            if (borderIndex == Borders.RIGHT)
            {
                num = (ulong)row;
                num = num << 0x20;
                return (num | ((uint)column));
            }
            if (borderIndex == Borders.BOTTOM)
            {
                num = (ulong)row;
                num = num << 0x20;
                return (num | ((uint)column));
            }
            if (borderIndex == Borders.LEFT)
            {
                int num2 = PreviousColumn(cIndex);
                num = (ulong)row;
                num = num << 0x20;
                return (num | ((uint)num2));
            }
            if (borderIndex == Borders.TOP)
            {
                num = (ulong)PreviousRow(rIndex);
                num = num << 0x20;
                num |= (uint)column;
            }
            return num;
        }

        BorderLine GetCellBorderByBorderIndex(int row, int column, Borders borderIndex, ref Cell cell)
        {
            if (cell == null)
            {
                cell = _owner.CellCache.GetCachedCell(row, column);
                if (cell == null)
                {
                    return null;
                }
            }

            switch (borderIndex)
            {
                case Borders.LEFT:
                    return cell.ActualBorderLeft;

                case Borders.TOP:
                    return cell.ActualBorderTop;

                case Borders.RIGHT:
                    return cell.ActualBorderRight;

                case Borders.BOTTOM:
                    return cell.ActualBorderBottom;
            }
            return null;
        }

        Rect GetCellBounds(int row, int column)
        {
            Rect rect = _owner.GetCellBounds(row, column, true);
            if (rect.X == -1.0)
            {
                rect.X = _owner.Location.X;
            }
            if (rect.Y == -1.0)
            {
                rect.Y = _owner.Location.Y;
            }
            return rect;
        }

        static bool IsDoubleLine(BorderLine line)
        {
            return BorderLineLayoutEngine.IsDoubleLine(line);
        }

        int PreviousRow(int rPos)
        {
            if ((rPos >= 1) && (rPos <= _visibleRows.Count))
            {
                return _visibleRows[rPos - 1];
            }
            return -1;
        }

        int PreviousColumn(int cPos)
        {
            if ((cPos >= 1) && (cPos <= _visibleCols.Count))
            {
                return _visibleCols[cPos - 1];
            }
            return -1;
        }

        int NextRow(int rPos)
        {
            if ((rPos >= -1) && (rPos < (_visibleRows.Count - 1)))
            {
                return _visibleRows[rPos + 1];
            }
            return -1;
        }

        int NextColumn(int cPos)
        {
            if ((cPos >= -1) && (cPos < (_visibleCols.Count - 1)))
            {
                return _visibleCols[cPos + 1];
            }
            return -1;
        }

        class CombinLine
        {
            public LineItem Item { get; set; }

            public Line Line1 { get; set; }

            public Line Line2 { get; set; }
        }
    }
}

