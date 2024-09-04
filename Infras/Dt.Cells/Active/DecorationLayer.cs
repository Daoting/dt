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
using Windows.Foundation;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 修饰层面板
    /// </summary>
    internal sealed partial class DecorationLayer : Panel
    {
        const double _thickness = 3;
        CellsPanel _viewport;
        PrintPaginator _paginator;
        readonly List<LineInfo> _horLines;
        readonly List<LineInfo> _verLines;
        readonly Rectangle _rect;
        Rect _bounds;
        int _startRow;
        int _startCol;
        int _toRow;
        int _toCol;
        bool _moving;
        Rectangle _dragRect;
        double _lastWidth;
        double _lastHeight;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_viewport"></param>
        public DecorationLayer(CellsPanel p_viewport)
        {
            _viewport = p_viewport;
            _horLines = new List<LineInfo>();
            _verLines = new List<LineInfo>();

            _bounds = new Rect();
            _rect = CreateRectangle();
            _rect.PointerPressed += OnPointerPressed;
            _rect.PointerMoved += OnPointerMoved;
            _rect.PointerExited += OnPointerExited;
            _rect.PointerReleased += OnPointerReleased;
            Children.Add(_rect);
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_viewport.Excel == null || _viewport.Excel.ActiveSheet == null)
                return _viewport.GetViewportSize(availableSize);

            // uno中尺寸有时容易多出小数，造成测量死循环！小数用Floor Ceiling Round取值都可能死循环！
            if (double.IsInfinity(availableSize.Width))
                _lastWidth = 5000;
            else if (Math.Abs(availableSize.Width - _lastWidth) > 1)
                _lastWidth = Math.Round(availableSize.Width);

            if (double.IsInfinity(availableSize.Height))
                _lastHeight = 5000;
            else if (Math.Abs(availableSize.Height - _lastHeight) > 1)
                _lastHeight = Math.Round(availableSize.Height);

            // 计算选择框位置区域
            bool isEmpty = true;
            CellRange range = _viewport.Excel.DecorationRange;
            if (range != null)
            {
                _bounds = _viewport.GetRangeBounds(range);
                if (!_bounds.IsEmpty)
                {
                    // 和原选择框重叠
                    _bounds.X -= _thickness - 1;
                    _bounds.Y -= _thickness - 1;
                    _bounds.Width += _thickness;
                    _bounds.Height += _thickness;
                    _rect.Measure(new Size(_bounds.Width, _bounds.Height));
                    isEmpty = false;
                }
            }
            if (isEmpty)
            {
                _bounds = new Rect();
                _rect.Measure(new Size(0.0, 0.0));
            }
            var size = new Size(_lastWidth, _lastHeight);

            // 水平垂直分页线
            if (_paginator == null)
            {
                var pi = _viewport.Excel.ActiveSheet.PrintInfo;
                _paginator = new PrintPaginator(
                    _viewport.Excel,
                    pi,
                    new Size(
                        pi.PaperSize.PxWidth - pi.Margin.PxLeft - pi.Margin.PxRight,
                        pi.PaperSize.PxHeight - pi.Margin.PxTop - pi.Margin.PxBottom));
            }
            _paginator.Paginate();
            PrepareLines(size);
            return _viewport.GetViewportSize(size);
        }

        /// <summary>
        /// 布局
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_viewport.Excel == null || _viewport.Excel.ActiveSheet == null)
                return finalSize;

            _rect.Arrange(_bounds);

            // 分页线
            Point topLeft = _viewport.Excel.ActiveSheet.GetTopLeftLocation(_viewport.RowViewportIndex, _viewport.ColumnViewportIndex);
            for (int i = 0; i < _horLines.Count; i++)
            {
                var info = _horLines[i];
                Line line = info.Line;
                line.Y1 = line.Y2 = info.Location.Y - topLeft.Y;
                line.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            for (int i = 0; i < _verLines.Count; i++)
            {
                var info = _verLines[i];
                Line line = info.Line;
                line.X1 = line.X2 = info.Location.X - topLeft.X;
                line.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            return finalSize;
        }

        void PrepareLines(Size p_size)
        {
            int newCount = _paginator.VerticalPageCount - _horLines.Count - 1;
            if (newCount != 0)
                CreateOrRemoveLines(_horLines, newCount);

            newCount = _paginator.HorizontalPageCount - _verLines.Count - 1;
            if (newCount != 0)
                CreateOrRemoveLines(_verLines, newCount);

            MeasureAllLines(p_size);
        }

        void CreateOrRemoveLines(List<LineInfo> p_lines, int p_newCount)
        {
            if (p_newCount > 0)
            {
                // 原有子元素不够
                for (int i = 0; i < p_newCount; i++)
                {
                    Line line = CreateLine();
                    p_lines.Add(new LineInfo { Line = line });
                    Children.Add(line);
                }
            }
            else if (p_newCount < 0)
            {
                // 移除多余的子元素
                for (int i = 0; i < -p_newCount; i++)
                {
                    int index = p_lines.Count - 1;
                    Children.Remove(p_lines[index].Line);
                    p_lines.RemoveAt(index);
                }
            }
        }

        void MeasureAllLines(Size p_size)
        {
            var sheet = _viewport.Excel.ActiveSheet;
            double rowHeaderWidth = 0.0;
            double colHeaderHeight = 0.0;
            if (sheet.RowHeader.IsVisible)
                rowHeaderWidth = ExcelPrinter.GetTotalWidth(sheet, sheet.RowHeader.ColumnCount, SheetArea.RowHeader);
            if (sheet.ColumnHeader.IsVisible)
                colHeaderHeight = ExcelPrinter.GetTotalHeight(sheet, sheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);

            if (_paginator.VerticalPageCount > 1)
            {
                for (int i = 0; i < _horLines.Count; i++)
                {
                    SheetPageInfo info = _paginator.GetPage(i, 0);
                    _horLines[i].Location = new Point(0, info.RowPage.YEnd + colHeaderHeight);
                    Line line = _horLines[i].Line;
                    line.X1 = 0;
                    line.X2 = p_size.Width;
                }
            }

            if (_paginator.HorizontalPageCount > 1)
            {
                for (int i = 0; i < _verLines.Count; i++)
                {
                    SheetPageInfo info = _paginator.GetPage(0, i);
                    _verLines[i].Location = new Point(info.ColumnPage.XEnd + rowHeaderWidth, 0);
                    Line line = _verLines[i].Line;
                    line.Y1 = 0;
                    line.Y2 = p_size.Height;
                }
            }
        }

        /// <summary>
        /// 创建分页线
        /// </summary>
        /// <returns></returns>
        Line CreateLine()
        {
            Line line = new Line();
            line.StrokeDashArray = new DoubleCollection() { 3, 1 };
            line.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xC3, 0xC3, 0xC3));
            line.StrokeThickness = 2;
            return line;
        }

        /// <summary>
        /// 拖拽报表项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_viewport.Excel == null
                || _viewport.Excel.DecorationRange == null
                || !_rect.CapturePointer(e.Pointer))
                return;

            e.Handled = true;
            _moving = true;
            var excel = _viewport.Excel;
            Point pos = e.GetCurrentPoint(excel).Position;
            HitTestInformation info = excel.HitTest(pos.X, pos.Y);
            if (info.ViewportInfo != null)
            {
                Worksheet ws = excel.ActiveSheet;
                CellRange range = excel.DecorationRange;
                int row = info.ViewportInfo.Row;
                int column = info.ViewportInfo.Column;
                int minRow = (range.Row < 0) ? 0 : range.Row;
                int minCol = (range.Column < 0) ? 0 : range.Column;
                int maxRow = (range.Row < 0) ? (ws.RowCount - 1) : ((range.Row + range.RowCount) - 1);
                int maxCol = (range.Column < 0) ? (ws.ColumnCount - 1) : ((range.Column + range.ColumnCount) - 1);
                if (row < minRow)
                {
                    row = minRow;
                }
                if (row > maxRow)
                {
                    row = maxRow;
                }
                if (column < minCol)
                {
                    column = minCol;
                }
                if (column > maxCol)
                {
                    column = maxCol;
                }
                _startRow = _toRow = row;
                _startCol = _toCol = column;

                ShowIndicator();
                _viewport.Excel.OnItemStartDrag();
            }
            else
            {
                ReleasePointer(e);
            }
        }

        /// <summary>
        /// 拖拽过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_viewport.Excel == null
                || _viewport.Excel.DecorationRange == null)
                return;

            e.Handled = true;
            var excel = _viewport.Excel;
            Point pos = e.GetCurrentPoint(excel).Position;
            excel.MousePosition = pos;
            if (_moving)
            {
                excel.UpdateMouseCursorLocation();
                HitTestInformation info = excel.HitTest(pos.X, pos.Y);
                if (info.ViewportInfo != null)
                {
                    // 位置不同时重绘
                    if (_toRow != info.ViewportInfo.Row || _toCol != info.ViewportInfo.Column)
                    {
                        _toRow = info.ViewportInfo.Row;
                        _toCol = info.ViewportInfo.Column;
                        RefreshIndicator();
                    }
                }
                else
                {
                    ReleasePointer(e);
                }
            }
            else
            {
                excel.SetMouseCursor(CursorType.DragCell_DragCursor);
            }
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            if (!_moving)
                _viewport.Excel.ResetCursor();
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_moving)
            {
                if (_startRow != _toRow || _startCol != _toCol)
                {
                    CellRange origin = _viewport.Excel.DecorationRange;
                    _viewport.Excel.OnItemDropped(new CellEventArgs(origin.Row + _toRow - _startRow, origin.Column + _toCol - _startCol));
                }
                ReleasePointer(e);
            }
        }

        void ReleasePointer(PointerRoutedEventArgs e)
        {
            _moving = false;
            _rect.ReleasePointerCapture(e.Pointer);
            _startRow = 0;
            _startCol = 0;
            _toRow = 0;
            _toCol = 0;
            if (_dragRect != null)
                _dragRect.Visibility = Visibility.Collapsed;
        }

        void ShowIndicator()
        {
            if (_dragRect == null && _viewport.Excel != null)
            {
                _dragRect = CreateRectangle();
                _viewport.Excel.TrackersPanel.Children.Add(_dragRect);
            }
        }

        void RefreshIndicator()
        {
            if (_viewport.Excel == null
                || _viewport.Excel.DecorationRange == null
                || _dragRect == null)
                return;

            CellRange origin = _viewport.Excel.DecorationRange;
            CellRange range = new CellRange(origin.Row + _toRow - _startRow, origin.Column + _toCol - _startCol, origin.RowCount, origin.ColumnCount);
            Rect rc = _viewport.GetRangeBounds(range);
            Canvas.SetLeft(_dragRect, Math.Floor(rc.Left + _viewport.Location.X - _thickness + 1));
            Canvas.SetTop(_dragRect, Math.Floor(rc.Top + _viewport.Location.Y - _thickness + 1));
            _dragRect.Visibility = Visibility.Visible;
            _dragRect.Height = rc.Height + _thickness;
            _dragRect.Width = rc.Width + _thickness;
        }

        Rectangle CreateRectangle()
        {
            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2));
            rect.StrokeThickness = _thickness;
            return rect;
        }

        class LineInfo
        {
            public Line Line { get; set; }

            public Point Location { get; set; }
        }
    }
}

