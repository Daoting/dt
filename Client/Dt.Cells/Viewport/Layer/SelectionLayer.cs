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
        int _recycledStart;

        public SelectionLayer(CellsPanel viewport)
        {
            _owner = viewport;
            _activeSelectionRectangles = new List<Rectangle>();
            _activeSelectionLayouts = new List<Rect>();

            // 当前焦点区域的背景，由4个矩形组成，一般使用2个
            var back = _owner.Excel.ActiveSheet.SelectionBackground;
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
            if (!_owner.Excel.ShowSelection)
            {
                return availableSize;
            }

            // 选择多区域时，非当前焦点区域，原来用Path实现，但在iOS上造成死循环
            _recycledStart = 0;
            for (int i = 0; i < _owner._cachedSelectionLayout.Count; i++)
            {
                Rect curRect = _owner._cachedSelectionLayout[i];
                if (curRect.IsEmpty)
                    continue;

                bool show = false;
                if (_owner.IsActived)
                {
                    Rect activeRect = _owner._cachedActiveSelectionLayout;
                    if (activeRect.IsEmpty
                        || (!ContainsRect(curRect, activeRect) && !ContainsRect(activeRect, curRect)))
                    {
                        show = true;
                    }
                }
                else
                {
                    show = true;
                }

                if (show)
                {
                    Rectangle rect = PopCachedSelection();
                    rect.Tag = curRect;
                    rect.Measure(new Size(curRect.Width, curRect.Height));
                }
            }
            int recycled = Children.Count - 5 - _recycledStart;
            if (recycled > 0)
            {
                // 多余的区域，为避免频繁增删Children子元素，区域矩形只增不删
                for (int i = 0; i < recycled; i++)
                {
                    ((Rectangle)Children[i + _recycledStart]).Tag = _rcEmpty;
                }
            }

            // 当前焦点区域背景
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

            // 当前焦点区域的外框，选择多区域时不可见
            if (_owner.IsActived || (_owner._cachedSelectionLayout.Count <= 1))
            {
                Rect rcFrame = IsAnchorCellInSelection ? _owner._cachedSelectionFrameLayout : _owner._cachedFocusCellLayout;
                FocusIndicator.Measure(new Size(rcFrame.Width, rcFrame.Height));
            }
            else
            {
                FocusIndicator.Measure(_szEmpty);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!_owner.Excel.ShowSelection)
                return finalSize;

            // 选择多区域时，非当前焦点区域
            if (Children.Count - 5 > 0)
            {
                for (int i = 0; i < Children.Count - 5; i++)
                {
                    var rc = (Rectangle)Children[i];
                    rc.Arrange((Rect)rc.Tag);
                }
            }

            // 当前焦点区域背景
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

            // 当前焦点区域的外框，选择多区域时不可见
            if (_owner.IsActived || (_owner._cachedSelectionLayout.Count <= 1))
            {
                Rect rcFrame = IsAnchorCellInSelection ? _owner._cachedSelectionFrameLayout : _owner._cachedFocusCellLayout;
                FocusIndicator.Arrange(rcFrame);
            }
            else
            {
                FocusIndicator.Arrange(_rcEmpty);
            }
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

        Rectangle PopCachedSelection()
        {
            Rectangle rect;
            if (_recycledStart + 5 >= Children.Count)
            {
                rect = new Rectangle { Fill = _owner.Excel.ActiveSheet.SelectionBackground };
                Children.Insert(_recycledStart, rect);
            }
            else
            {
                rect = (Rectangle)Children[_recycledStart];
            }
            _recycledStart++;
            return rect;
        }

        void UpdateActiveSelectionLayouts()
        {
            Rect rcSelection = _owner._cachedActiveSelectionLayout;
            Rect rcFocus = _owner._cachedFocusCellLayout;
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
                    _activeSelectionLayouts.Add(rcSelection);
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
                    Rect rect3 = new Rect(rcSelection.X, rcSelection.Y, rcSelection.Width, rcFocus.Y - rcSelection.Y);
                    Rect rect4 = new Rect(rcSelection.X, rcFocus.Y, rcFocus.X - rcSelection.X, rcFocus.Height);
                    double width = rcSelection.Right - rcFocus.Right;
                    if (width < 0.0)
                    {
                        width = 0.0;
                    }
                    Rect rect5 = new Rect(rcFocus.Right, rcFocus.Y, width, rcFocus.Height);
                    double height = rcSelection.Bottom - rcFocus.Bottom;
                    if (height < 0.0)
                    {
                        height = 0.0;
                    }
                    Rect rect6 = new Rect(rcSelection.X, rcFocus.Bottom, rcSelection.Width, height);
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
                _activeSelectionLayouts.Add(rcSelection);
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

