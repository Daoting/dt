#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class SelectionContainerPanel : Panel
    {
        List<Rect> _activeSelectionLayouts;
        List<Rectangle> _activeSelectionRectangles;
        SelectionFrame _focusIndicator;
        Brush _selectionBackground;
        Path _selectionPath;
        const int ACTIVE_SELECTION_RECTANGLE_NUMBER = 4;

        public SelectionContainerPanel(GcViewport parentViewport)
        {
            _activeSelectionRectangles = new List<Rectangle>();
            _activeSelectionLayouts = new List<Rect>();
            ParentViewport = parentViewport;

            _selectionBackground = parentViewport.Sheet.Worksheet.SelectionBackground;
            _selectionPath = new Path();
            if (_selectionBackground != null)
                _selectionPath.Fill = _selectionBackground;
            else
                _selectionPath.Fill = new SolidColorBrush(Color.FromArgb(60, 180, 180, 200));

            GeometryGroup group = new GeometryGroup();
            group.FillRule = FillRule.Nonzero;
            _selectionPath.Data = group;
            Children.Add(_selectionPath);

            for (int i = 0; i < 4; i++)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = _selectionBackground;
                _activeSelectionRectangles.Add(rectangle);
                Children.Add(rectangle);
            }

            _focusIndicator = new SelectionFrame(parentViewport);
            Children.Add(_focusIndicator);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ParentViewport.Sheet.HideSelectionWhenPrinting)
            {
                return availableSize;
            }

            UpdateActiveSelectionLayouts();
            for (int i = 0; i < _activeSelectionRectangles.Count; i++)
            {
                Rect rect = _activeSelectionLayouts[i];
                _activeSelectionRectangles[i].InvalidateMeasure();
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    _activeSelectionRectangles[i].Measure(new Size(rect.Width, rect.Height));
                }
                else
                {
                    _activeSelectionRectangles[i].Measure(new Size(0.0, 0.0));
                }
            }

            GeometryGroup data = _selectionPath.Data as GeometryGroup;
            if (data != null)
            {
                data.Children.Clear();
                for (int j = 0; j < ParentViewport._cachedSelectionLayout.Count; j++)
                {
                    Rect rect2 = ParentViewport._cachedSelectionLayout[j];
                    if (ParentViewport.IsActived)
                    {
                        Rect rect3 = ParentViewport._cachedActiveSelectionLayout;
                        if (!rect2.IsEmpty)
                        {
                            if (rect3.IsEmpty)
                            {
                                RectangleGeometry geometry = new RectangleGeometry();
                                geometry.Rect = rect2;
                                data.Children.Add(geometry);
                            }
                            else if (!ContainsRect(rect2, rect3) && !ContainsRect(rect3, rect2))
                            {
                                RectangleGeometry geometry2 = new RectangleGeometry();
                                geometry2.Rect = rect2;
                                data.Children.Add(geometry2);
                            }
                        }
                    }
                    else if (!rect2.IsEmpty)
                    {
                        RectangleGeometry geometry3 = new RectangleGeometry();
                        geometry3.Rect = rect2;
                        data.Children.Add(geometry3);
                    }
                }
            }

            _selectionPath.InvalidateMeasure();
            if (ParentViewport.Sheet.Worksheet.SelectionBackground == null)
            {
                _selectionBackground = new SolidColorBrush(Color.FromArgb(60, 180, 180, 200));
            }
            else
            {
                _selectionBackground = ParentViewport.Sheet.Worksheet.SelectionBackground;
            }
            foreach (Rectangle item in _activeSelectionRectangles)
            {
                item.Fill = _selectionBackground; ;
            }
            _selectionPath.Measure(availableSize);

            if (FocusIndicator != null)
            {
                Rect rect4 = ParentViewport._cachedSelectionFrameLayout;
                if (!IsAnchorCellInSelection)
                {
                    rect4 = ParentViewport._cachedFocusCellLayout;
                }
                if ((FocusIndicator.Visibility == Visibility.Visible) && (ParentViewport.IsActived || (ParentViewport._cachedSelectionLayout.Count <= 1)))
                {
                    if ((rect4.Width > 0.0) && (rect4.Height > 0.0))
                    {
                        FocusIndicator.Measure(new Size(rect4.Width, rect4.Height));
                    }
                }
                else
                {
                    FocusIndicator.Visibility = Visibility.Collapsed;
                }
            }
            return ParentViewport.GetViewportSize(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!ParentViewport.Sheet.HideSelectionWhenPrinting)
            {
                Rect rect = new Rect(new Windows.Foundation.Point(), finalSize);
                for (int i = 0; i < _activeSelectionRectangles.Count; i++)
                {
                    _activeSelectionRectangles[i].InvalidateArrange();
                    if ((_activeSelectionLayouts[i].Width > 0.0) && (_activeSelectionLayouts[i].Height > 0.0))
                    {
                        _activeSelectionRectangles[i].Arrange(_activeSelectionLayouts[i]);
                    }
                    else
                    {
                        _activeSelectionRectangles[i].Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
                    }
                }
                _selectionPath.Arrange(rect);
                if (FocusIndicator == null)
                {
                    return finalSize;
                }
                Rect rect2 = ParentViewport._cachedSelectionFrameLayout;
                if (!IsAnchorCellInSelection)
                {
                    rect2 = ParentViewport._cachedFocusCellLayout;
                }
                if ((FocusIndicator.Visibility == Visibility.Visible) && (ParentViewport.IsActived || (ParentViewport._cachedSelectionLayout.Count <= 1)))
                {
                    if ((rect2.Width > 0.0) && (rect2.Height > 0.0))
                    {
                        FocusIndicator.Arrange(rect2);
                        return finalSize;
                    }
                    FocusIndicator.Visibility = Visibility.Collapsed;
                    return finalSize;
                }
                FocusIndicator.Visibility = Visibility.Collapsed;
            }
            return finalSize;
        }

        bool ContainsRect(Rect rect1, Rect rect2)
        {
            return ((((rect2.Left >= rect1.Left) && (rect2.Right <= rect1.Right)) && (rect2.Top >= rect1.Top)) && (rect2.Bottom <= rect1.Bottom));
        }

        CellRange GetViewportRange()
        {
            int viewportTopRow = ParentViewport.Sheet.GetViewportTopRow(ParentViewport.RowViewportIndex);
            int viewportLeftColumn = ParentViewport.Sheet.GetViewportLeftColumn(ParentViewport.ColumnViewportIndex);
            int viewportBottomRow = ParentViewport.Sheet.GetViewportBottomRow(ParentViewport.RowViewportIndex);
            int viewportRightColumn = ParentViewport.Sheet.GetViewportRightColumn(ParentViewport.ColumnViewportIndex);
            return new CellRange(viewportTopRow, viewportLeftColumn, (viewportBottomRow - viewportTopRow) + 1, (viewportRightColumn - viewportLeftColumn) + 1);
        }

        internal void ResetSelectionFrameStroke()
        {
            _focusIndicator.ResetSelectionFrameStoke();
        }

        internal void SetSelectionFrameStroke(Brush brush)
        {
            _focusIndicator.SetSelectionFrameStroke(brush);
        }

        void UpdateActiveSelectionLayouts()
        {
            Rect rect = ParentViewport._cachedActiveSelectionLayout;
            Rect rect2 = ParentViewport._cachedFocusCellLayout;
            CellRange range = null;
            _activeSelectionLayouts = new List<Rect>();
            CellRange viewportRange = GetViewportRange();

            if (ParentViewport.IsActived && (ParentViewport.Sheet.Worksheet.ActiveCell != null))
            {
                Worksheet ws = ParentViewport.Sheet.Worksheet;
                range = new CellRange(ws.ActiveRowIndex, ws.ActiveColumnIndex, 1, 1);
                CellRange range3 = ws.SpanModel.Find(range.Row, range.Column);
                if ((range3 != null) && viewportRange.Intersects(range3.Row, range3.Column, range3.RowCount, range3.ColumnCount))
                {
                    range = CellRange.GetIntersect(viewportRange, range3, viewportRange.RowCount, viewportRange.ColumnCount);
                }
            }

            if (ParentViewport.IsActived)
            {
                if ((viewportRange.RowCount == 0) || (viewportRange.ColumnCount == 0))
                {
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                }
                else if ((range != null) && !viewportRange.Contains(range))
                {
                    _activeSelectionLayouts.Add(rect);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                }
                else if (ParentViewport._cachedActiveSelection != null
                    && range != null
                    && IsActiveCellBoundsValid
                    && ParentViewport._cachedActiveSelection != range
                    && ParentViewport._cachedActiveSelection.Contains(range))
                {
                    Rect rect3 = new Rect(rect.X, rect.Y, rect.Width, rect2.Y - rect.Y);
                    Rect rect4 = new Rect(rect.X, rect2.Y, rect2.X - rect.X, rect2.Height);
                    double width = rect.Right - rect2.Right;
                    if (width < 0.0)
                    {
                        width = 0.0;
                    }
                    Rect rect5 = new Rect(rect2.Right, rect2.Y, width, rect2.Height);
                    double height = rect.Bottom - rect2.Bottom;
                    if (height < 0.0)
                    {
                        height = 0.0;
                    }
                    Rect rect6 = new Rect(rect.X, rect2.Bottom, rect.Width, height);
                    _activeSelectionLayouts.Add(rect3);
                    _activeSelectionLayouts.Add(rect4);
                    _activeSelectionLayouts.Add(rect5);
                    _activeSelectionLayouts.Add(rect6);
                }
                else
                {
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                }
            }
            else if ((viewportRange.RowCount == 0) || (viewportRange.ColumnCount == 0))
            {
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
            }
            else
            {
                _activeSelectionLayouts.Add(rect);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
            }
        }

        internal SelectionFrame FocusIndicator
        {
            get { return _focusIndicator; }
        }

        bool IsActiveCellBoundsValid
        {
            get { return ((!ParentViewport._cachedFocusCellLayout.IsEmpty && (ParentViewport._cachedFocusCellLayout.Width > 0.0)) && (ParentViewport._cachedFocusCellLayout.Height > 0.0)); }
        }

        public bool IsAnchorCellInSelection { get; set; }

        public GcViewport ParentViewport { get; set; }
    }
}

