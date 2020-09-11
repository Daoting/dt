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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
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
        readonly List<Line> _horLines;
        readonly List<Line> _verLines;
        readonly Rectangle _rect;
        Rect _bounds;
        int _startRow;
        int _startCol;
        int _toRow;
        int _toCol;
        bool _moving;
        Rectangle _dragRect;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_viewport"></param>
        public DecorationLayer(CellsPanel p_viewport)
        {
            _viewport = p_viewport;
            _horLines = new List<Line>();
            _verLines = new List<Line>();

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

            double width = availableSize.Width;
            double height = availableSize.Height;
            if (double.IsInfinity(width))
                width = Windows.UI.Xaml.Window.Current.Bounds.Width;
            if (double.IsInfinity(height))
                height = Windows.UI.Xaml.Window.Current.Bounds.Height;

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

            // 水平垂直分页线
            Size paperSize = _viewport.Excel.PaperSize;
            if (paperSize.Width > 0 && paperSize.Height > 0)
            {
                PrepareLines(width, height, true, paperSize);
                PrepareLines(width, height, false, paperSize);
            }
            else
            {
                ClearLines(true);
                ClearLines(false);
            }
            return _viewport.GetViewportSize(availableSize);
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
            Size paperSize = _viewport.Excel.PaperSize;
            if (paperSize.Width > 0 && paperSize.Height > 0)
            {
                Worksheet ws = _viewport.Excel.ActiveSheet;
                Point topLeft = ws.GetTopLeftLocation(_viewport.RowViewportIndex, _viewport.ColumnViewportIndex);
                double offsetY = topLeft.Y % paperSize.Height;
                double offsetX = topLeft.X % paperSize.Width;

                for (int i = 0; i < _horLines.Count; i++)
                {
                    double top = (i + 1) * paperSize.Height - offsetY;
                    Line line = _horLines[i];
                    line.Y1 = line.Y2 = top;
                    line.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }

                for (int i = 0; i < _verLines.Count; i++)
                {
                    double left = (i + 1) * paperSize.Width - offsetX;
                    Line line = _verLines[i];
                    line.X1 = line.X2 = left;
                    line.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
            }
            return finalSize;
        }

        /// <summary>
        /// 准备分页线
        /// </summary>
        /// <param name="p_width"></param>
        /// <param name="p_height"></param>
        /// <param name="p_isHor"></param>
        /// <param name="p_paper"></param>
        void PrepareLines(double p_width, double p_height, bool p_isHor, Size p_paper)
        {
            int count;
            List<Line> lines;
            if (p_isHor)
            {
                lines = _horLines;
                count = (int)Math.Ceiling(p_height / p_paper.Height);
            }
            else
            {
                lines = _verLines;
                count = (int)Math.Ceiling(p_width / p_paper.Width);
            }
            int newCount = count - lines.Count;

            if (newCount > 0)
            {
                // 原有子元素不够
                for (int i = 0; i < newCount; i++)
                {
                    Line line = CreateLine();
                    lines.Add(line);
                    Children.Add(line);
                }
            }
            else if (newCount < 0)
            {
                // 移除多余的子元素
                for (int i = 0; i < -newCount; i++)
                {
                    int index = lines.Count - 1;
                    Line line = lines[index];
                    Children.Remove(line);
                    lines.RemoveAt(index);
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                Line line = lines[i];
                if (p_isHor)
                    line.X2 = p_width;
                else
                    line.Y2 = p_height;
                line.Measure(new Size(p_width, p_height));
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

        void ClearLines(bool p_isHor)
        {
            List<Line> lines = p_isHor ? _horLines : _verLines;
            while (lines.Count > 0)
            {
                Line line = lines[0];
                Children.Remove(line);
                lines.RemoveAt(0);
            }
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
    }
}

