using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Dt.Cells.UI
{
    public partial class SpreadView : SheetView
    {

        internal override void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                UpdateScrollBarIndicatorMode((ScrollingIndicatorMode)1);
            }
            base.OnPointerMoved(sender, e);
        }

        internal override void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
                if ((_primaryTouchDeviceId.HasValue && (intermediatePoints != null)) && (intermediatePoints.Count > 0))
                {
                    using (IEnumerator<PointerPoint> enumerator = intermediatePoints.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.PointerId == _primaryTouchDeviceId.Value)
                            {
                                if (IsTouchColumnSplitting)
                                {
                                    EndColumnSplitting();
                                }
                                if (IsTouchRowSplitting)
                                {
                                    EndRowSplitting();
                                }
                            }
                        }
                    }
                }
            }
            if ((TabStrip != null) && TabStripEditable)
            {
                Point point = e.GetCurrentPoint(this).Position;
                if ((HitTest(point.X, point.Y).HitTestType == HitTestType.TabStrip) && TabStrip.StayInEditing(point))
                {
                    e.Handled = true;
                    return;
                }
            }
            base.OnPointerReleased(sender, e);
        }

        protected override void OnManipulationStarted(Point point)
        {
            base.UpdateTouchHitTestInfo(new Point(point.X, point.Y));
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            base._touchStartHitTestInfo = savedHitTestInformation;
            base._touchZoomNewFactor = Worksheet.ZoomFactor;
            if ((savedHitTestInformation != null) && (savedHitTestInformation.HitTestType != HitTestType.Empty))
            {
                InitTouchCacheInfomation();
            }
            base.OnManipulationStarted(point);
        }

        protected override void ProcessGestrueRecognizerManipulationUpdated(ManipulationUpdatedEventArgs e)
        {
            if ((_currentGestureAction != null) && _currentGestureAction.IsValid)
            {
                if (IsZero((double)(e.Cumulative.Scale - 1f)) || (_touchProcessedPointIds.Count == 1))
                {
                    _currentGestureAction.HandleSingleManipulationDelta(e.Position, e.Cumulative.Translation);
                }
                else if ((!IsZero((double)(e.Cumulative.Scale - 1f)) && !base.IsTouchZooming) && base.CanUserZoom)
                {
                    base.IsContinueTouchOperation = true;
                    base._touchZoomInitFactor = Worksheet.ZoomFactor;
                    base.IsTouchZooming = true;
                    base._touchZoomOrigin = e.Position;
                    base.CloseTouchToolbar();
                    if (base._touchStartHitTestInfo == null)
                    {
                        base._touchStartHitTestInfo = HitTest(e.Position.X, e.Position.Y);
                    }
                    if (base._zoomOriginHitTestInfo == null)
                    {
                        base._zoomOriginHitTestInfo = HitTest(e.Position.X, e.Position.Y);
                    }
                    base._touchZoomOrigin = e.Position;
                    InitCachedTransform();
                }
                else if (base.IsTouchZooming && (((!base.IsTouchDragFilling && !base.IsTouchDrapDropping) && (!IsEditing && !base.IsTouchSelectingCells)) && ((!base.IsTouchSelectingColumns && !base.IsTouchSelectingRows) && ((!base.IsTouchResizingColumns && !base.IsTouchResizingRows) && base.CanUserZoom))))
                {
                    double num = ((base._touchZoomInitFactor * ((float)e.Cumulative.Scale)) * 100.0) / 100.0;
                    float scale = e.Delta.Scale;
                    if ((num < 0.5) || (num > 4.0))
                    {
                        scale = 1f;
                    }
                    UpdateCachedImageTransform(e.Position, e.Delta.Translation, (double)scale);
                    if (num < 0.5)
                    {
                        num = 0.5;
                    }
                    if (num > 4.0)
                    {
                        num = 4.0;
                    }
                    base._touchZoomNewFactor = num;
                    base.InvalidateMeasure();
                }
            }
        }

        protected override void ProcessManipulationTranslation(Point currentPosition, Point offsetFromOrigin)
        {
            if ((_currentGestureAction != null) && _currentGestureAction.IsValid)
            {
                _currentGestureAction.HandleSingleManipulationDelta(currentPosition, offsetFromOrigin);
            }
        }

        internal override void ProcessTouchFreeDrag(Point startPoint, Point currentPoint, Point deltaPoint, DragOrientation orientation)
        {
            if (!IsWorking)
            {
                UpdateTouchHitTestInfo(currentPoint);
            }
            HitTestInformation savedHitTestInformation = GetSavedHitTestInformation();
            MousePosition = currentPoint;
            if (!IsTouching)
                return;

            bool flag = ((_touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject)
                && (_touchStartHitTestInfo.FloatingObjectInfo.FloatingObject != null))
                && _touchStartHitTestInfo.FloatingObjectInfo.FloatingObject.IsSelected;
            if ((!IsTouchDragFilling
                    && !IsTouchDrapDropping
                    && !IsTouchSelectingCells
                    && !IsTouchTabStripResizing
                    && !IsRowSplitting
                    && !IsColumnSplitting
                    && !IsTouchSelectingColumns
                    && !IsTouchSelectingRows
                    && _touchStartHitTestInfo.HitTestType == HitTestType.Viewport)
                || ((_touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && !flag))
            {
                IsContinueTouchOperation = true;
                CloseTouchToolbar();
                _updateViewportAfterTouch = true;

                if (deltaPoint.X != 0.0 && (orientation & DragOrientation.Horizontal) == DragOrientation.Horizontal)
                {
                    if ((orientation & DragOrientation.Vertical) == DragOrientation.None)
                    {
                        _translateOffsetY = 0.0;
                    }

                    if (deltaPoint.X > 0.0)
                    {
                        _isTouchScrolling = true;
                        TouchScrollLeft(startPoint, currentPoint, deltaPoint);
                    }
                    else
                    {
                        _isTouchScrolling = true;
                        TouchScrollRight(startPoint, currentPoint, deltaPoint);
                    }
                }

                if (deltaPoint.Y != 0.0 && (orientation & DragOrientation.Vertical) == DragOrientation.Vertical)
                {
                    if ((orientation & DragOrientation.Horizontal) == DragOrientation.None)
                    {
                        _translateOffsetX = 0.0;
                    }

                    if (deltaPoint.Y > 0.0)
                    {
                        _isTouchScrolling = true;
                        TouchScrollUp(startPoint, currentPoint, deltaPoint);
                    }
                    else
                    {
                        _isTouchScrolling = true;
                        TouchScrollBottom(startPoint, currentPoint, deltaPoint);
                    }
                }

                SpreadLayout spreadLayout = GetSpreadLayout();
                if (_translateOffsetX < 0.0)
                {
                    if ((_touchStartHitTestInfo.ColumnViewportIndex == -1) || (_touchStartHitTestInfo.ColumnViewportIndex == spreadLayout.ColumnPaneCount))
                    {
                        _translateOffsetX = 0.0;
                    }
                    else
                    {
                        spreadLayout.SetViewportWidth(_touchStartHitTestInfo.ColumnViewportIndex, _cachedViewportWidths[_touchStartHitTestInfo.ColumnViewportIndex + 1] + Math.Abs(_translateOffsetX));
                        base.InvalidateViewportColumnsLayout();
                    }
                }
                else if ((_touchStartHitTestInfo.ColumnViewportIndex == -1) || (_touchStartHitTestInfo.ColumnViewportIndex == spreadLayout.ColumnPaneCount))
                {
                    _translateOffsetX = 0.0;
                }
                else
                {
                    spreadLayout.SetViewportWidth(_touchStartHitTestInfo.ColumnViewportIndex, _cachedViewportWidths[_touchStartHitTestInfo.ColumnViewportIndex + 1]);
                    base.InvalidateViewportColumnsLayout();
                }

                if (_translateOffsetY < 0.0)
                {
                    if ((base._touchStartHitTestInfo.RowViewportIndex == -1) || (base._touchStartHitTestInfo.RowViewportIndex == spreadLayout.RowPaneCount))
                    {
                        base._translateOffsetY = 0.0;
                    }
                    else
                    {
                        spreadLayout.SetViewportHeight(base._touchStartHitTestInfo.RowViewportIndex, base._cachedViewportHeights[base._touchStartHitTestInfo.RowViewportIndex + 1] + Math.Abs(base._translateOffsetY));
                        base.InvalidateViewportRowsLayout();
                    }
                }
                else if ((base._touchStartHitTestInfo.RowViewportIndex == -1) || (base._touchStartHitTestInfo.RowViewportIndex == spreadLayout.RowPaneCount))
                {
                    base._translateOffsetY = 0.0;
                }
                else
                {
                    spreadLayout.SetViewportHeight(base._touchStartHitTestInfo.RowViewportIndex, base._cachedViewportHeights[base._touchStartHitTestInfo.RowViewportIndex + 1]);
                    base.InvalidateViewportColumnsLayout();
                }
                base.InvalidateMeasure();
            }

            if ((savedHitTestInformation.HitTestType == HitTestType.ColumnSplitBox) && IsTouchColumnSplitting)
            {
                ContinueColumnSplitting();
            }
            else if ((savedHitTestInformation.HitTestType == HitTestType.RowSplitBox) && IsTouchRowSplitting)
            {
                ContinueRowSplitting();
            }
            else if ((savedHitTestInformation.HitTestType == HitTestType.RowSplitBar) || (savedHitTestInformation.HitTestType == HitTestType.ColumnSplitBar))
            {
                if (IsTouchRowSplitting)
                {
                    ContinueRowSplitting();
                }
                if (IsTouchColumnSplitting)
                {
                    ContinueColumnSplitting();
                }
            }
            else if (savedHitTestInformation.HitTestType == HitTestType.TabStrip)
            {
                IsTouchTabStripScrolling = true;
                if ((deltaPoint.X != 0.0) && ((orientation & DragOrientation.Horizontal) == DragOrientation.Horizontal))
                {
                    if (deltaPoint.X > 0.0)
                    {
                        TouchTabStripScrollLeft(startPoint, currentPoint, deltaPoint);
                    }
                    if (deltaPoint.X < 0.0)
                    {
                        TouchTabStripScrollRight(startPoint, currentPoint, deltaPoint);
                    }
                    TabStrip.TabsPresenter.InvalidateMeasure();
                    TabStrip.TabsPresenter.InvalidateArrange();
                }
            }
            else if ((savedHitTestInformation.HitTestType == HitTestType.TabSplitBox) && base.IsTouchTabStripResizing)
            {
                ContinueTabStripResizing();
            }
            else
            {
                base.ProcessTouchFreeDrag(startPoint, currentPoint, deltaPoint, orientation);
            }
        }

        protected override void OnManipulationComplete(Point currentPoint)
        {
            IsContinueTouchOperation = false;
            CachedGripperLocation = null;
            ClearViewportsClip();
            UpdateViewport();

            if (IsTouchTabStripScrolling)
            {
                IsTouchTabStripScrolling = false;
                TabStrip.TabsPresenter.Offset = 0.0;
                TabStrip.TabsPresenter.InvalidateMeasure();
                TabStrip.TabsPresenter.InvalidateArrange();
            }

            if (IsTouchZooming)
            {
                IsTouchZooming = false;
                _cachedViewportVisual = null;
                _cachedColumnHeaderViewportVisual = null;
                _cachedRowHeaderViewportVisual = null;
                _cachedCornerViewportVisual = null;
                _cachedBottomRightACornerVisual = null;
                if ((_zoomOriginHitTestInfo != null) && (_zoomOriginHitTestInfo.HitTestType == HitTestType.Viewport))
                {
                    TransformGroup group = _cachedViewportTransform[_zoomOriginHitTestInfo.RowViewportIndex + 1, _zoomOriginHitTestInfo.ColumnViewportIndex + 1];
                    if (group != null)
                    {
                        SpreadLayout spreadLayout = GetSpreadLayout();
                        Point newLocation = new Point(spreadLayout.HeaderWidth, spreadLayout.HeaderHeight);
                        Point reference = group.TransformPoint(newLocation);
                        int viewportLeftColumn = GetViewportLeftColumn(_zoomOriginHitTestInfo.ColumnViewportIndex);
                        Point point3 = reference.Delta(newLocation);
                        int num2 = viewportLeftColumn;
                        double x = point3.X;
                        if (x > 0.0)
                        {
                            while ((x > 0.0) && (num2 < Worksheet.ColumnCount))
                            {
                                x -= Math.Floor((double)(Worksheet.Columns[num2].ActualWidth * _touchZoomNewFactor));
                                num2++;
                            }
                        }
                        else if (x < 0.0)
                        {
                            while (((x < 0.0) && (num2 > 0)) && (num2 < Worksheet.ColumnCount))
                            {
                                x += Math.Floor((double)(Worksheet.Columns[num2].ActualWidth * _touchZoomNewFactor));
                                num2--;
                            }
                        }
                        if (num2 != viewportLeftColumn)
                        {
                            SetViewportLeftColumn(_zoomOriginHitTestInfo.ColumnViewportIndex, num2);
                        }
                        int viewportTopRow = GetViewportTopRow(_zoomOriginHitTestInfo.RowViewportIndex);
                        int num5 = viewportTopRow;
                        double y = point3.Y;
                        if (y > 0.0)
                        {
                            while ((y > 0.0) && (num5 < Worksheet.RowCount))
                            {
                                y -= Math.Floor((double)(Worksheet.Rows[num5].ActualHeight * base._touchZoomNewFactor));
                                num5++;
                            }
                        }
                        else if (y < 0.0)
                        {
                            while (((y < 0.0) && (num5 > 0)) && (num5 < Worksheet.RowCount))
                            {
                                y += Math.Floor((double)(Worksheet.Rows[num5].ActualHeight * base._touchZoomNewFactor));
                                num5--;
                            }
                        }
                        if (num5 != viewportTopRow)
                        {
                            SetViewportTopRow(base._zoomOriginHitTestInfo.RowViewportIndex, num5);
                        }
                    }
                }

                _cachedViewportTransform = null;
                _cachedRowHeaderViewportTransform = null;
                _cachedColumnHeaderViewportTransform = null;
                _cachedCornerViewportTransform = null;
                float zoomFactor = Worksheet.ZoomFactor;
                Worksheet.ZoomFactor = (float)base._touchZoomNewFactor;
                base.RaiseUserZooming(zoomFactor, Worksheet.ZoomFactor);
                base.InvalidateViewportColumnsLayout();
                base.InvalidateViewportRowsLayout();
                base.InvalidateFloatingObjects();
                base.InvalidateMeasure();
                if (((base._touchStartTopRow >= 0) && (base._touchStartTopRow < Worksheet.RowCount)) && (base._touchStartLeftColumn >= 0))
                {
                    int columnCount = Worksheet.ColumnCount;
                    int num14 = base._touchStartLeftColumn;
                }
            }

            if (IsTouchColumnSplitting)
            {
                EndColumnSplitting();
            }
            if (IsTouchRowSplitting)
            {
                EndRowSplitting();
            }
            if (base.IsTouchTabStripResizing)
            {
                EndTabStripResizing();
            }

            _fastScroll = false;
            GetViewportInfo();
            if (_viewportPresenters != null)
            {
                GcViewport[,] viewportArray = base._viewportPresenters;
                int upperBound = viewportArray.GetUpperBound(0);
                int num9 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num9; j++)
                    {
                        GcViewport viewport = viewportArray[i, j];
                        if (viewport != null)
                        {
                            viewport.InvalidateBordersMeasureState();
                        }
                    }
                }
            }
            if (base._rowHeaderPresenters != null)
            {
                foreach (GcViewport viewport2 in base._rowHeaderPresenters)
                {
                    if (viewport2 != null)
                    {
                        viewport2.InvalidateBordersMeasureState();
                    }
                }
            }
            if (base._columnHeaderPresenters != null)
            {
                foreach (GcViewport viewport3 in base._columnHeaderPresenters)
                {
                    if (viewport3 != null)
                    {
                        viewport3.InvalidateBordersMeasureState();
                    }
                }
            }
            base.OnManipulationComplete(currentPoint);
        }

        internal override void ResetTouchStates(IList<PointerPoint> ps)
        {
            if (IsTouchColumnSplitting)
            {
                EndColumnSplitting();
            }
            if (IsTouchRowSplitting)
            {
                EndRowSplitting();
            }
            if (base.IsTouchTabStripResizing)
            {
                EndTabStripResizing();
            }
            base.ResetTouchStates(ps);
        }

        internal override bool StartTouchTap(Point point)
        {
            if (IsEditing && ((IsMouseInSplitBar() || IsMouseInSplitBox()) || IsMouseInTabSplitBox()))
            {
                return false;
            }
            if (!GetTabStripRectangle().Contains(point) || base.CanSelectFormula)
            {
                return base.StartTouchTap(point);
            }
            base.StopCellEditing(false);
            if (_tabStrip != null)
            {
                _tabStrip.StopTabEditing(false);
            }
            base._lastClickPoint = point;
            return true;
        }

        internal override HitTestInformation TouchHitTest(double x, double y)
        {
            Point point = new Point(x, y);
            SpreadLayout spreadLayout = GetSpreadLayout();
            HitTestInformation information = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = point
            };
            bool flag = (RowSplitBoxAlignment == SplitBoxAlignment.Trailing) && (ColumnSplitBoxAlignment == SplitBoxAlignment.Trailing);
            for (int i = 0; i < spreadLayout.ColumnPaneCount; i++)
            {
                Rect horizontalSplitBoxRectangle = GetHorizontalSplitBoxRectangle(i);
                if (flag)
                {
                    horizontalSplitBoxRectangle = horizontalSplitBoxRectangle.Expand(30, 0);
                }
                else
                {
                    horizontalSplitBoxRectangle = horizontalSplitBoxRectangle.Expand(30, 15);
                }
                if (horizontalSplitBoxRectangle.Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBox;
                    information.ColumnViewportIndex = i;
                    return information;
                }
            }
            for (int j = 0; j < spreadLayout.RowPaneCount; j++)
            {
                Rect verticalSplitBoxRectangle = GetVerticalSplitBoxRectangle(j);
                if (flag)
                {
                    verticalSplitBoxRectangle = verticalSplitBoxRectangle.Expand(0, 30);
                }
                else
                {
                    verticalSplitBoxRectangle = verticalSplitBoxRectangle.Expand(15, 30);
                }
                if (verticalSplitBoxRectangle.Expand(15, 30).Contains(point))
                {
                    information.HitTestType = HitTestType.RowSplitBox;
                    information.RowViewportIndex = j;
                    return information;
                }
            }
            for (int k = 0; k < (spreadLayout.ColumnPaneCount - 1); k++)
            {
                if (GetHorizontalSplitBarRectangle(k).Expand(10, 10).Contains(point) && (information.HitTestType != HitTestType.ColumnSplitBox))
                {
                    information.HitTestType = HitTestType.ColumnSplitBar;
                    information.ColumnViewportIndex = k;
                }
            }
            for (int m = 0; m < (spreadLayout.RowPaneCount - 1); m++)
            {
                if (GetVerticalSplitBarRectangle(m).Expand(10, 10).Contains(point) && (information.HitTestType != HitTestType.RowSplitBox))
                {
                    information.HitTestType = HitTestType.RowSplitBar;
                    information.RowViewportIndex = m;
                }
            }
            if (GetTabSplitBoxRectangle().Expand(40, 10).Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabSplitBox;
                return information;
            }
            if (information.HitTestType == HitTestType.Empty)
            {
                if (GetTabStripRectangle().Contains(point))
                {
                    information.ColumnViewportIndex = 0;
                    information.HitTestType = HitTestType.TabStrip;
                    return information;
                }
                for (int n = 0; n < spreadLayout.ColumnPaneCount; n++)
                {
                    if (GetHorizontalScrollBarRectangle(n).Contains(point))
                    {
                        information.ColumnViewportIndex = n;
                        information.HitTestType = HitTestType.HorizontalScrollBar;
                        return information;
                    }
                }
                for (int num6 = 0; num6 < spreadLayout.RowPaneCount; num6++)
                {
                    if (GetVerticalScrollBarRectangle(num6).Contains(point))
                    {
                        information.HitTestType = HitTestType.VerticalScrollBar;
                        information.RowViewportIndex = num6;
                        return information;
                    }
                }
                if (information.HitTestType == HitTestType.Empty)
                {
                    information = base.TouchHitTest(x, y);
                }
            }
            return information;
        }

        #region 实现触摸滚动
        //****************************************************
        // 触摸时滚动单元格区通过改变 _translateOffsetX _translateOffsetY 的值在 ArrangeOverride 中实现
        // TabStrip 通过改变 Offset 值实现标签滚动
        //****************************************************

        void TouchScrollLeft(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int maxRightScrollableColumn = base.GetMaxRightScrollableColumn();
            ColumnLayoutModel columnLayoutModel = base.GetColumnLayoutModel(base._touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells);
            double num2 = Math.Abs(deltaPoint.X);
            int viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn > maxRightScrollableColumn)
            {
                return;
            }
            double num4 = 0.0;
            if (columnLayoutModel.FindColumn(viewportLeftColumn) != null)
            {
                num4 = columnLayoutModel.FindColumn(viewportLeftColumn).Width + base._translateOffsetX;
            }
            if (num4 > num2)
            {
                base._translateOffsetX += -1.0 * num2;
                return;
            }
            num2 -= num4;
            int num5 = viewportLeftColumn + 1;
            while (true)
            {
                if ((num5 > maxRightScrollableColumn) || Worksheet.Columns[num5].ActualVisible)
                {
                    break;
                }
                num5++;
            }
            if (num5 > maxRightScrollableColumn)
            {
                base._translateOffsetX += -1.0 * Math.Abs(deltaPoint.X);
                return;
            }
            base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num5);
            viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn >= maxRightScrollableColumn)
            {
                return;
            }
            int num6 = viewportLeftColumn;
        Label_010B:
            if (num6 >= Worksheet.ColumnCount)
            {
                return;
            }
            if (num6 >= 0)
            {
                Column column = Worksheet.Columns[num6];
                if (column.ActualVisible)
                {
                    double num7 = column.Width * base.ZoomFactor;
                    if (num7 > num2)
                    {
                        base._translateOffsetX = -1.0 * num2;
                        return;
                    }
                    num2 -= num7;
                }
                num6++;
                if (num6 <= maxRightScrollableColumn)
                {
                    if (column.ActualVisible)
                    {
                        base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num6);
                    }
                    goto Label_010B;
                }
            }
        }

        void TouchScrollRight(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int maxLeftScrollableColumn = base.GetMaxLeftScrollableColumn();
            int maxRightScrollableColumn = base.GetMaxRightScrollableColumn();
            ColumnLayoutModel columnLayoutModel = base.GetColumnLayoutModel(base._touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.X);
            int viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn <= maxLeftScrollableColumn)
            {
                Point point = currentPoint.Delta(startPoint);
                base._translateOffsetX = -1.0 * point.X;
                base._translateOffsetX = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetX), 120.0) * Math.Sign(base._translateOffsetX);
                return;
            }
            if ((viewportLeftColumn >= maxRightScrollableColumn) && (currentPoint.X < startPoint.X))
            {
                return;
            }
            double num5 = 0.0;
            if (columnLayoutModel.FindColumn(viewportLeftColumn) != null)
            {
                if ((base._translateOffsetX + Math.Abs(deltaPoint.X)) < 0.0)
                {
                    num5 = (columnLayoutModel.FindColumn(viewportLeftColumn).Width + base._translateOffsetX) + Math.Abs(deltaPoint.X);
                }
                else
                {
                    num5 = 0.0;
                }
            }
            if (num5 >= num3)
            {
                base._translateOffsetX += num3;
                return;
            }
            num3 -= num5;
            if ((num3 + base._translateOffsetX) >= 0.0)
            {
                num3 += base._translateOffsetX;
                base._translateOffsetX = 0.0;
            }
            else
            {
                return;
            }
            int num6 = viewportLeftColumn - 1;
            while (true)
            {
                if ((num6 < maxLeftScrollableColumn) || Worksheet.Columns[num6].ActualVisible)
                {
                    break;
                }
                num6--;
            }
            if (num6 < maxLeftScrollableColumn)
            {
                Point point2 = currentPoint.Delta(startPoint);
                base._translateOffsetX = -1.0 * point2.X;
                base._translateOffsetX = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetX), 120.0) * Math.Sign(base._translateOffsetX);
                return;
            }
            base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num6);
            viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn <= maxLeftScrollableColumn)
            {
                return;
            }
            int num7 = viewportLeftColumn;
        Label_0201:
            if (num7 >= Worksheet.ColumnCount)
            {
                return;
            }
            if (num7 >= 0)
            {
                Column column = Worksheet.Columns[num7];
                if (column.ActualVisible)
                {
                    double num8 = column.Width * base.ZoomFactor;
                    if (num8 >= num3)
                    {
                        base._translateOffsetX = num3 - num8;
                        GetSpreadLayout();
                        return;
                    }
                    num3 -= num8;
                }
                num7--;
                if (num7 < maxLeftScrollableColumn)
                {
                    base._translateOffsetX = num3;
                }
                else
                {
                    if (column.ActualVisible)
                    {
                        base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num7);
                    }
                    goto Label_0201;
                }
            }
        }

        void TouchScrollUp(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int maxBottomScrollableRow = GetMaxBottomScrollableRow();
            RowLayoutModel rowLayoutModel = GetRowLayoutModel(_touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells);
            int viewportTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow > maxBottomScrollableRow)
            {
                return;
            }

            double num2 = Math.Abs(deltaPoint.Y);
            double num4 = 0.0;
            var rowLayout = rowLayoutModel.FindRow(viewportTopRow);
            if (rowLayout != null)
            {
                num4 = rowLayout.Height + _translateOffsetY;
            }
            if (num4 > num2)
            {
                _translateOffsetY += -1.0 * num2;
                return;
            }

            num2 -= num4;
            int num5 = viewportTopRow + 1;
            while (true)
            {
                if ((num5 > maxBottomScrollableRow) || Worksheet.Rows[num5].ActualVisible)
                {
                    break;
                }
                num5++;
            }
            if (num5 > maxBottomScrollableRow)
            {
                _translateOffsetY += -1.0 * Math.Abs(deltaPoint.Y);
                return;
            }

            base.SetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex, num5);
            viewportTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow >= maxBottomScrollableRow)
            {
                return;
            }

            int num6 = viewportTopRow;
        Label_0112:
            if (num6 < 0)
            {
                return;
            }
            if (num6 < Worksheet.RowCount)
            {
                Row row = Worksheet.Rows[num6];
                if (row.ActualVisible)
                {
                    double num7 = row.ActualHeight * base.ZoomFactor;
                    if (num7 > num2)
                    {
                        base._translateOffsetY = -1.0 * num2;
                        return;
                    }
                    num2 -= num7;
                }
                num6++;
                if (num6 < maxBottomScrollableRow)
                {
                    if (row.ActualVisible)
                    {
                        base.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num6);
                    }
                    goto Label_0112;
                }
            }
        }

        void TouchScrollBottom(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int maxTopScrollableRow = base.GetMaxTopScrollableRow();
            int maxBottomScrollableRow = base.GetMaxBottomScrollableRow();
            RowLayoutModel rowLayoutModel = base.GetRowLayoutModel(base._touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.Y);
            int viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= maxTopScrollableRow)
            {
                Point point = currentPoint.Delta(startPoint);
                base._translateOffsetY = -1.0 * point.Y;
                base._translateOffsetY = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetY), 80.0) * Math.Sign(base._translateOffsetY);
                return;
            }
            if ((viewportTopRow >= maxBottomScrollableRow) && (currentPoint.Y < startPoint.Y))
            {
                return;
            }
            double num5 = 0.0;
            if (rowLayoutModel.FindRow(viewportTopRow) != null)
            {
                if ((base._translateOffsetY + Math.Abs(deltaPoint.Y)) < 0.0)
                {
                    num5 = (rowLayoutModel.FindRow(viewportTopRow).Height + base._translateOffsetY) + Math.Abs(deltaPoint.Y);
                }
                else
                {
                    num5 = 0.0;
                }
            }
            if (num5 >= num3)
            {
                base._translateOffsetY += num3;
                return;
            }
            num3 -= num5;
            if ((num3 + base._translateOffsetY) >= 0.0)
            {
                num3 += base._translateOffsetY;
                base._translateOffsetY = 0.0;
            }
            else
            {
                return;
            }
            int num6 = viewportTopRow - 1;
            while (true)
            {
                if ((num6 < maxTopScrollableRow) || Worksheet.Rows[num6].ActualVisible)
                {
                    break;
                }
                num6--;
            }
            if (num6 < maxTopScrollableRow)
            {
                Point point2 = currentPoint.Delta(startPoint);
                base._translateOffsetY = -1.0 * point2.Y;
                base._translateOffsetY = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetY), 80.0) * Math.Sign(base._translateOffsetY);
                return;
            }
            base.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num6);
            viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= maxTopScrollableRow)
            {
                return;
            }
            int num7 = viewportTopRow;
        Label_0201:
            if (!Worksheet.Rows[num7].ActualVisible)
            {
                num7--;
            }
            if (((num7 >= maxTopScrollableRow) && (num7 >= 0)) && (num7 < Worksheet.RowCount))
            {
                Row row = Worksheet.Rows[num7];
                if (row.ActualVisible)
                {
                    double num8 = row.Height * base.ZoomFactor;
                    if (num8 >= num3)
                    {
                        base._translateOffsetY = num3 - num8;
                        GetSpreadLayout();
                        return;
                    }
                    num3 -= num8;
                }
                num7--;
                if (num7 >= maxTopScrollableRow)
                {
                    if (row.ActualVisible)
                    {
                        base.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num7);
                    }
                    goto Label_0201;
                }
            }
        }

        void TouchTabStripScrollLeft(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            TabsPresenter tabsPresenter = TabStrip.TabsPresenter;
            int firstScrollableSheetIndex = tabsPresenter.FirstScrollableSheetIndex;
            int lastScrollableSheetIndex = tabsPresenter.LastScrollableSheetIndex;
            int startIndex = tabsPresenter.StartIndex;
            double num2 = Math.Abs(deltaPoint.X);
            if (tabsPresenter.IsLastSheetVisible)
            {
                double num3 = -1.0 * currentPoint.Delta(startPoint).X;
                num3 = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(num3), 120.0) * Math.Sign(num3);
                tabsPresenter.Offset = num3;
                return;
            }
            double num4 = tabsPresenter.FirstSheetTabWidth - Math.Abs(tabsPresenter.Offset);
            if (num4 >= num2)
            {
                tabsPresenter.Offset += -1.0 * num2;
                return;
            }
            num2 -= num4;
            int num5 = startIndex + 1;
            if (num5 > tabsPresenter.LastScrollableSheetIndex)
            {
                tabsPresenter.Offset = -1.0 * Math.Abs(deltaPoint.X);
            }
            if (num5 >= tabsPresenter.LastScrollableSheetIndex)
            {
                return;
            }
            SpreadSheet.StartSheetIndex = num5;
        Label_00F0:
            if (num5 >= tabsPresenter.LastScrollableSheetIndex)
            {
                return;
            }
            if (num5 >= 0)
            {
                double num6 = tabsPresenter.FirstSheetTabWidth * base.ZoomFactor;
                if (num6 > num2)
                {
                    tabsPresenter.Offset = -1.0 * num2;
                }
                else
                {
                    num2 -= num6;
                    num5++;
                    if (num5 <= tabsPresenter.LastScrollableSheetIndex)
                    {
                        SpreadSheet.StartSheetIndex = num5;
                        goto Label_00F0;
                    }
                }
            }
        }

        void TouchTabStripScrollRight(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int num7;
            double num9;
            TabsPresenter tabsPresenter = TabStrip.TabsPresenter;
            int firstScrollableSheetIndex = tabsPresenter.FirstScrollableSheetIndex;
            int lastScrollableSheetIndex = tabsPresenter.LastScrollableSheetIndex;
            int startIndex = tabsPresenter.StartIndex;
            double num4 = Math.Abs(deltaPoint.X);
            if (startIndex <= firstScrollableSheetIndex)
            {
                Point point = currentPoint.Delta(startPoint);
                double num5 = -1.0 * point.X;
                num5 = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(num5), 120.0) * Math.Sign(num5);
                tabsPresenter.Offset = num5;
                return;
            }
            if ((startIndex < lastScrollableSheetIndex) || (currentPoint.X >= startPoint.X))
            {
                double num6 = Math.Abs(tabsPresenter.Offset);
                if (num6 >= num4)
                {
                    tabsPresenter.Offset += num4;
                    return;
                }
                num4 -= num6;
                if ((num4 + tabsPresenter.Offset) >= 0.0)
                {
                    num4 += tabsPresenter.Offset;
                    tabsPresenter.Offset = 0.0;
                    num7 = startIndex - 1;
                    if (num7 < tabsPresenter.FirstScrollableSheetIndex)
                    {
                        Point point2 = currentPoint.Delta(startPoint);
                        double num8 = -1.0 * point2.X;
                        num8 = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(num8), 120.0) * Math.Sign(num8);
                        tabsPresenter.Offset = num8;
                        return;
                    }
                    if (num7 < tabsPresenter.FirstScrollableSheetIndex)
                    {
                        return;
                    }
                    SpreadSheet.StartSheetIndex = num7;
                    goto Label_0154;
                }
            }
            return;
        Label_0154:
            num9 = tabsPresenter.FirstSheetTabWidth * base.ZoomFactor;
            if (num9 >= num4)
            {
                tabsPresenter.Offset = num4 - num9;
            }
            else
            {
                num4 -= num9;
                num7--;
                if (num7 < tabsPresenter.FirstScrollableSheetIndex)
                {
                    tabsPresenter.Offset = num4;
                }
                else
                {
                    SpreadSheet.StartSheetIndex = num7;
                    goto Label_0154;
                }
            }
        }
        #endregion
    }
}
