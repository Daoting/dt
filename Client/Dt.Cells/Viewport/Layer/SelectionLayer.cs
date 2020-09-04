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
    internal partial class SelectionLayer : Panel
    {
        static Size _szEmpty = new Size();
        static Rect _rcEmpty = new Rect();
        readonly CellsPanel _owner;
        readonly List<Rect> _activeSelectionLayouts;
        readonly List<Rectangle> _activeSelectionRectangles;
        Path _selectionPath;
        const int ACTIVE_SELECTION_RECTANGLE_NUMBER = 4;

        public SelectionLayer(CellsPanel viewport)
        {
            _owner = viewport;
            _activeSelectionRectangles = new List<Rectangle>();
            _activeSelectionLayouts = new List<Rect>();

            _selectionPath = new Path();
            var back = _owner.Excel.ActiveSheet.SelectionBackground;
            if (back != null)
                _selectionPath.Fill = back;
            else
                _selectionPath.Fill = new SolidColorBrush(Color.FromArgb(60, 180, 180, 200));

            GeometryGroup group = new GeometryGroup();
            group.FillRule = FillRule.Nonzero;
            _selectionPath.Data = group;
            Children.Add(_selectionPath);

            for (int i = 0; i < 4; i++)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = back;
                _activeSelectionRectangles.Add(rectangle);
                Children.Add(rectangle);
            }

            FocusIndicator = new SelectionFrame(_owner);
            Children.Add(FocusIndicator);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner.Excel.HideSelectionWhenPrinting)
            {
                return availableSize;
            }

            UpdateActiveSelectionLayouts();
            for (int i = 0; i < _activeSelectionRectangles.Count; i++)
            {
                Rectangle rectangle = _activeSelectionRectangles[i];
                Rect rect = _activeSelectionLayouts[i];
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    rectangle.Measure(new Size(rect.Width, rect.Height));
                }
                else
                {
                    rectangle.Measure(_szEmpty);
                }
            }

            //GeometryGroup data = _selectionPath.Data as GeometryGroup;
            //if (data != null)
            //{
            //    data.Children.Clear();
            //    for (int j = 0; j < _owner._cachedSelectionLayout.Count; j++)
            //    {
            //        Rect rect2 = _owner._cachedSelectionLayout[j];
            //        if (_owner.IsActived)
            //        {
            //            Rect rect3 = _owner._cachedActiveSelectionLayout;
            //            if (!rect2.IsEmpty)
            //            {
            //                if (rect3.IsEmpty)
            //                {
            //                    RectangleGeometry geometry = new RectangleGeometry();
            //                    geometry.Rect = rect2;
            //                    data.Children.Add(geometry);
            //                }
            //                else if (!ContainsRect(rect2, rect3) && !ContainsRect(rect3, rect2))
            //                {
            //                    RectangleGeometry geometry2 = new RectangleGeometry();
            //                    geometry2.Rect = rect2;
            //                    data.Children.Add(geometry2);
            //                }
            //            }
            //        }
            //        else if (!rect2.IsEmpty)
            //        {
            //            RectangleGeometry geometry3 = new RectangleGeometry();
            //            geometry3.Rect = rect2;
            //            data.Children.Add(geometry3);
            //        }
            //    }
            //}
            //_selectionPath.Measure(availableSize);

            Rect rect4 = _owner._cachedSelectionFrameLayout;
            if (!IsAnchorCellInSelection)
            {
                rect4 = _owner._cachedFocusCellLayout;
            }
            if ((FocusIndicator.Visibility == Visibility.Visible) && (_owner.IsActived || (_owner._cachedSelectionLayout.Count <= 1)))
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
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_owner.Excel.HideSelectionWhenPrinting)
                return finalSize;

            Rect rect = new Rect(new Point(), finalSize);
            for (int i = 0; i < _activeSelectionRectangles.Count; i++)
            {
                if ((_activeSelectionLayouts[i].Width > 0.0) && (_activeSelectionLayouts[i].Height > 0.0))
                {
                    _activeSelectionRectangles[i].Arrange(_activeSelectionLayouts[i]);
                }
                else
                {
                    _activeSelectionRectangles[i].Arrange(_rcEmpty);
                }
            }

            //_selectionPath.Arrange(rect);

            Rect rect2 = _owner._cachedSelectionFrameLayout;
            if (!IsAnchorCellInSelection)
            {
                rect2 = _owner._cachedFocusCellLayout;
            }
            if ((FocusIndicator.Visibility == Visibility.Visible) && (_owner.IsActived || (_owner._cachedSelectionLayout.Count <= 1)))
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
            return finalSize;
        }

        bool ContainsRect(Rect rect1, Rect rect2)
        {
            return ((((rect2.Left >= rect1.Left) && (rect2.Right <= rect1.Right)) && (rect2.Top >= rect1.Top)) && (rect2.Bottom <= rect1.Bottom));
        }

        CellRange GetViewportRange()
        {
            int viewportTopRow = _owner.Excel.GetViewportTopRow(_owner.RowViewportIndex);
            int viewportLeftColumn = _owner.Excel.GetViewportLeftColumn(_owner.ColumnViewportIndex);
            int viewportBottomRow = _owner.Excel.GetViewportBottomRow(_owner.RowViewportIndex);
            int viewportRightColumn = _owner.Excel.GetViewportRightColumn(_owner.ColumnViewportIndex);
            return new CellRange(viewportTopRow, viewportLeftColumn, (viewportBottomRow - viewportTopRow) + 1, (viewportRightColumn - viewportLeftColumn) + 1);
        }

        internal void ResetSelectionFrameStroke()
        {
            FocusIndicator.ResetSelectionFrameStoke();
        }

        internal void SetSelectionFrameStroke(Brush brush)
        {
            FocusIndicator.SetSelectionFrameStroke(brush);
        }

        void UpdateActiveSelectionLayouts()
        {
            Rect rect = _owner._cachedActiveSelectionLayout;
            Rect rect2 = _owner._cachedFocusCellLayout;
            CellRange range = null;
            _activeSelectionLayouts.Clear();
            CellRange viewportRange = GetViewportRange();

            if (_owner.IsActived && (_owner.Excel.ActiveSheet.ActiveCell != null))
            {
                Worksheet ws = _owner.Excel.ActiveSheet;
                range = new CellRange(ws.ActiveRowIndex, ws.ActiveColumnIndex, 1, 1);
                CellRange range3 = ws.SpanModel.Find(range.Row, range.Column);
                if ((range3 != null) && viewportRange.Intersects(range3.Row, range3.Column, range3.RowCount, range3.ColumnCount))
                {
                    range = CellRange.GetIntersect(viewportRange, range3, viewportRange.RowCount, viewportRange.ColumnCount);
                }
            }

            if (_owner.IsActived)
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
                else if (_owner._cachedActiveSelection != null
                    && range != null
                    && IsActiveCellBoundsValid
                    && _owner._cachedActiveSelection != range
                    && _owner._cachedActiveSelection.Contains(range))
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

        internal SelectionFrame FocusIndicator { get; }

        bool IsActiveCellBoundsValid
        {
            get { return ((!_owner._cachedFocusCellLayout.IsEmpty && (_owner._cachedFocusCellLayout.Width > 0.0)) && (_owner._cachedFocusCellLayout.Height > 0.0)); }
        }

        public bool IsAnchorCellInSelection { get; set; }
    }
}

