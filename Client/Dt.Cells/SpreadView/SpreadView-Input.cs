using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Dt.Cells.UI
{
    public partial class SpreadView : SheetView
    {
        #region 鼠标按下
        protected override bool StartMouseClick(PointerRoutedEventArgs e)
        {
            if (IsMouseInScrollBar())
            {
                return false;
            }

            if (IsEditing && ((IsMouseInSplitBar() || IsMouseInSplitBox()) || IsMouseInTabSplitBox()))
            {
                return false;
            }

            Point position = e.GetCurrentPoint(this).Position;
            if (!GetTabStripRectangle().Contains(position))
            {
                return base.StartMouseClick(e);
            }

            if (CanSelectFormula)
            {
                IsSwitchingSheet = true;
                EditorConnector.ClearFlickingItems();
                if (!EditorConnector.IsInOtherSheet)
                {
                    EditorConnector.IsInOtherSheet = true;
                    EditorConnector.SheetIndex = Worksheet.Workbook.ActiveSheetIndex;
                    EditorConnector.RowIndex = Worksheet.ActiveRowIndex;
                    EditorConnector.ColumnIndex = Worksheet.ActiveColumnIndex;
                }
            }

            StopCellEditing(CanSelectFormula);
            if (_tabStrip != null)
            {
                _tabStrip.StopTabEditing(false);
            }
            _lastClickPoint = position;
            _routedEventArgs = e;
            return true;
        }

        protected override void ProcessMouseLeftButtonDown(HitTestInformation p_hitInfo)
        {
            GetSpreadLayout();
            if (!_isDoubleClick
                || (p_hitInfo.HitTestType != HitTestType.ColumnSplitBar && p_hitInfo.HitTestType != HitTestType.RowSplitBar))
            {
                switch (p_hitInfo.HitTestType)
                {
                    case HitTestType.TabStrip:
                        if (_routedEventArgs != null)
                        {
                            _tabStrip.ProcessMouseClickSheetTab(_routedEventArgs);
                        }
                        return;

                    case HitTestType.RowSplitBar:
                        StartRowSplitting();
                        if (p_hitInfo.ColumnViewportIndex >= 0)
                        {
                            StartColumnSplitting();
                        }
                        return;

                    case HitTestType.ColumnSplitBar:
                        StartColumnSplitting();
                        if (p_hitInfo.RowViewportIndex >= 0)
                        {
                            StartRowSplitting();
                        }
                        return;

                    case HitTestType.RowSplitBox:
                        StartRowSplitting();
                        return;

                    case HitTestType.ColumnSplitBox:
                        StartColumnSplitting();
                        return;

                    case HitTestType.TabSplitBox:
                        StartTabStripResizing();
                        return;
                }
                base.ProcessMouseLeftButtonDown(p_hitInfo);
            }
        }
        #endregion

        #region 鼠标移动
        protected override void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                UpdateScrollBarIndicatorMode(ScrollingIndicatorMode.TouchIndicator);
            }
            base.OnPointerMoved(sender, e);
        }

        protected override void ProcessMouseMove(HitTestInformation p_hitInfo)
        {
            // 不知何用？hdt
            //GetSpreadLayout();

            bool flag = false;
            switch (p_hitInfo.HitTestType)
            {
                case HitTestType.RowSplitBar:
                    if ((!IsWorking && !IsEditing) && !SpreadSheet.Workbook.Protect)
                    {
                        if (InputDeviceType != InputDeviceType.Touch)
                        {
                            if (p_hitInfo.ColumnViewportIndex < 0)
                            {
                                SetBuiltInCursor((CoreCursorType)8);
                            }
                            else
                            {
                                SetBuiltInCursor((CoreCursorType)3);
                            }
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.ColumnSplitBar:
                    if ((IsWorking || IsEditing) || SpreadSheet.Workbook.Protect)
                    {
                        goto Label_01B3;
                    }
                    if (InputDeviceType != InputDeviceType.Touch)
                    {
                        if (p_hitInfo.RowViewportIndex < 0)
                        {
                            SetBuiltInCursor((CoreCursorType)10);
                            break;
                        }
                        SetBuiltInCursor((CoreCursorType)3);
                    }
                    break;

                case HitTestType.RowSplitBox:
                    if ((!IsWorking && !IsEditing) && !SpreadSheet.Workbook.Protect)
                    {
                        if (InputDeviceType != InputDeviceType.Touch)
                        {
                            SetBuiltInCursor((CoreCursorType)8);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.ColumnSplitBox:
                    if ((!IsWorking && !IsEditing) && !SpreadSheet.Workbook.Protect)
                    {
                        if (InputDeviceType != InputDeviceType.Touch)
                        {
                            SetBuiltInCursor((CoreCursorType)10);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.TabSplitBox:
                    if (!IsWorking && !IsEditing)
                    {
                        if (InputDeviceType != InputDeviceType.Touch)
                        {
                            SetBuiltInCursor((CoreCursorType)10);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                default:
                    goto Label_01B3;
            }
            flag = true;
        Label_01B3:
            if (IsColumnSplitting)
            {
                ContinueColumnSplitting();
            }
            if (IsRowSplitting)
            {
                ContinueRowSplitting();
            }
            if (IsTabStripResizing)
            {
                ContinueTabStripResizing();
            }

            if (flag)
            {
                if (!IsWorking)
                {
                    _hoverManager.DoHover(p_hitInfo);
                }
            }
            else
            {
                base.ProcessMouseMove(p_hitInfo);
            }
            UpdateScrollBarIndicatorMode(ScrollingIndicatorMode.MouseIndicator);
        }
        #endregion

        #region 鼠标释放
        protected override void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
                if (_primaryTouchDeviceId.HasValue
                    && intermediatePoints != null
                    && intermediatePoints.Count > 0)
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

            if (TabStrip != null && TabStripEditable)
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

        protected override bool EndMouseClick(PointerRoutedEventArgs e)
        {
            if (GetHitInfo().HitTestType == HitTestType.TabStrip)
            {
                return false;
            }
            return base.EndMouseClick(e);
        }

        protected override void ProcessMouseLeftButtonUp(HitTestInformation p_hitInfo)
        {
            if ((IsColumnSplitting || IsRowSplitting) || IsTabStripResizing)
            {
                ClearMouseLeftButtonDownStates();
                if (!IsEditing)
                {
                    FocusInternal();
                }
            }
            else
            {
                base.ProcessMouseLeftButtonUp(p_hitInfo);
            }
        }
        #endregion

        #region 鼠标离开
        protected override void ResetTouchStates(IList<PointerPoint> ps)
        {
            if (IsTouchColumnSplitting)
            {
                EndColumnSplitting();
            }
            if (IsTouchRowSplitting)
            {
                EndRowSplitting();
            }
            if (IsTouchTabStripResizing)
            {
                EndTabStripResizing();
            }
            base.ResetTouchStates(ps);
        }
        #endregion

        #region 鼠标双击
        protected override void ProcessMouseLeftButtonDoubleClick(DoubleTappedRoutedEventArgs e)
        {
            HitTestInformation savedHitTestInformation = base.GetHitInfo();
            if ((!IsEditing || ((savedHitTestInformation.HitTestType != HitTestType.RowSplitBar) && (savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBar))) && ((savedHitTestInformation.HitTestType != HitTestType.Viewport) || !savedHitTestInformation.ViewportInfo.InSelectionDrag))
            {
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        ProcessSplitBarDoubleClick(savedHitTestInformation);
                        return;

                    case HitTestType.TabStrip:
                        {
                            if (((_tabStrip == null) || !TabStripEditable) || (SpreadSheet.Workbook.Protect || (base._routedEventArgs == null)))
                            {
                                break;
                            }
                            _tabStrip.StartTabEditing(_routedEventArgs);
                            if (!_tabStrip.IsEditing)
                            {
                                break;
                            }
                            int sheetTabIndex = -1;
                            if (_tabStrip.ActiveTab != null)
                            {
                                sheetTabIndex = _tabStrip.ActiveTab.SheetIndex;
                            }
                            base.RaiseSheetTabDoubleClick(sheetTabIndex);
                            return;
                        }
                    default:
                        base.ProcessMouseLeftButtonDoubleClick(e);
                        break;
                }
            }
        }
        #endregion

        #region 触摸开始
        protected override void OnManipulationStarted()
        {
            UpdateHitInfo(_touchStartPoint);
            _touchStartHitTestInfo = GetHitInfo();
            _touchZoomNewFactor = Worksheet.ZoomFactor;
            if ((_touchStartHitTestInfo != null) && (_touchStartHitTestInfo.HitTestType != HitTestType.Empty))
            {
                InitTouchCacheInfomation();
            }
            base.OnManipulationStarted();
        }
        #endregion

        #region 触摸移动
        protected override void ProcessGestrueRecognizerManipulationUpdated(ManipulationUpdatedEventArgs e)
        {
            Point curPos = GetPanelPosition(e.Position);
            if (IsZero((double)(e.Cumulative.Scale - 1f)) || (_touchProcessedPointIds.Count == 1))
            {
                ProcessTouchFreeDrag(curPos, new Point(-e.Delta.Translation.X, -e.Delta.Translation.Y));
            }
            else if ((!IsZero((double)(e.Cumulative.Scale - 1f)) && !IsTouchZooming) && CanUserZoom)
            {
                IsContinueTouchOperation = true;
                _touchZoomInitFactor = Worksheet.ZoomFactor;
                IsTouchZooming = true;
                _touchZoomOrigin = curPos;
                CloseTouchToolbar();
                if (_touchStartHitTestInfo == null)
                {
                    _touchStartHitTestInfo = HitTest(curPos.X, curPos.Y);
                }
                if (_zoomOriginHitTestInfo == null)
                {
                    _zoomOriginHitTestInfo = HitTest(curPos.X, curPos.Y);
                }
                _touchZoomOrigin = curPos;
                InitCachedTransform();
            }
            else if (IsTouchZooming
                && !IsTouchDragFilling
                && !IsTouchDrapDropping
                && !IsEditing
                && !IsTouchSelectingCells
                && !IsTouchSelectingColumns
                && !IsTouchSelectingRows
                && !IsTouchResizingColumns
                && !IsTouchResizingRows
                && CanUserZoom)
            {
                double num = ((_touchZoomInitFactor * ((float)e.Cumulative.Scale)) * 100.0) / 100.0;
                float scale = e.Delta.Scale;
                if ((num < 0.5) || (num > 4.0))
                {
                    scale = 1f;
                }
                UpdateCachedImageTransform(curPos, e.Delta.Translation, (double)scale);
                if (num < 0.5)
                {
                    num = 0.5;
                }
                if (num > 4.0)
                {
                    num = 4.0;
                }
                _touchZoomNewFactor = num;
                InvalidateMeasure();
            }
        }

        protected override void ProcessTouchFreeDrag(Point p_curPos, Point p_offset)
        {
            if (Math.Abs(p_offset.X) < 1 && Math.Abs(p_offset.Y) < 1)
                return;

            // 滑动方向
            DragOrientation orientation;
            if (p_offset.X == 0)
            {
                orientation = DragOrientation.Vertical;
            }
            else if (p_offset.Y == 0)
            {
                orientation = DragOrientation.Horizontal;
            }
            else
            {
                double num = Math.Atan(Math.Abs(p_offset.Y) / Math.Abs(p_offset.X)) * 57.295779513082323;
                if (num > 55.0)
                {
                    orientation = DragOrientation.Vertical;
                }
                else if (num > 35.0)
                {
                    orientation = DragOrientation.Horizontal | DragOrientation.Vertical;
                }
                else
                {
                    orientation = DragOrientation.Horizontal;
                }
            }

            MousePosition = p_curPos;
            if (!IsWorking)
            {
                UpdateHitInfo(p_curPos);
            }
            HitTestInformation savedHitTestInformation = GetHitInfo();
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

                if (p_offset.X != 0.0 && (orientation & DragOrientation.Horizontal) == DragOrientation.Horizontal)
                {
                    if ((orientation & DragOrientation.Vertical) == DragOrientation.None)
                    {
                        _translateOffsetY = 0.0;
                    }

                    if (p_offset.X > 0.0)
                    {
                        _isTouchScrolling = true;
                        TouchScrollLeft(p_curPos, p_offset);
                    }
                    else
                    {
                        _isTouchScrolling = true;
                        TouchScrollRight(p_curPos, p_offset);
                    }
                }

                if (p_offset.Y != 0.0 && (orientation & DragOrientation.Vertical) == DragOrientation.Vertical)
                {
                    if ((orientation & DragOrientation.Horizontal) == DragOrientation.None)
                    {
                        _translateOffsetX = 0.0;
                    }

                    if (p_offset.Y > 0.0)
                    {
                        _isTouchScrolling = true;
                        TouchScrollUp(p_curPos, p_offset);
                    }
                    else
                    {
                        _isTouchScrolling = true;
                        TouchScrollBottom(p_curPos, p_offset);
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
                if ((p_offset.X != 0.0) && ((orientation & DragOrientation.Horizontal) == DragOrientation.Horizontal))
                {
                    if (p_offset.X > 0.0)
                    {
                        TouchTabStripScrollLeft(p_curPos, p_offset);
                    }
                    if (p_offset.X < 0.0)
                    {
                        TouchTabStripScrollRight(p_curPos, p_offset);
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
                base.ProcessTouchFreeDrag(p_curPos, p_offset);
            }
        }
        #endregion

        #region 触摸结束
        protected override void OnManipulationComplete()
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
            base.OnManipulationComplete();
        }
        #endregion

        #region 触摸单/双击
        protected override void ProcessTap(HitTestInformation p_hitInfo)
        {
            switch (p_hitInfo.HitTestType)
            {
                case HitTestType.HorizontalScrollBar:
                    {
                        int viewportLeftColumn = GetViewportLeftColumn(p_hitInfo.ColumnViewportIndex);
                        HorizontalScrollBarTouchSmallDecrement(p_hitInfo.ColumnViewportIndex, viewportLeftColumn - 1);
                        break;
                    }
                case HitTestType.VerticalScrollBar:
                    {
                        int viewportTopRow = GetViewportTopRow(p_hitInfo.RowViewportIndex);
                        VerticalScrollBarTouchSmallDecrement(p_hitInfo.RowViewportIndex, viewportTopRow - 1);
                        break;
                    }
                case HitTestType.TabStrip:
                    _tabStrip.ProcessTap(p_hitInfo.HitPoint);
                    break;
            }
            base.ProcessTap(p_hitInfo);
        }

        protected override void ProcessDoubleTap(HitTestInformation p_hitInfo)
        {
            HitTestInformation hi = TouchHitTest(p_hitInfo.HitPoint.X, p_hitInfo.HitPoint.Y);
            if (!IsEditing || ((hi.HitTestType != HitTestType.RowSplitBar) && (hi.HitTestType != HitTestType.ColumnSplitBar)))
            {
                if ((hi.HitTestType == HitTestType.RowSplitBar) || (hi.HitTestType == HitTestType.ColumnSplitBar))
                {
                    ProcessSplitBarDoubleTap(hi);
                }
                else if ((p_hitInfo.HitTestType != HitTestType.Viewport) || !p_hitInfo.ViewportInfo.InSelectionDrag)
                {
                    if (((p_hitInfo.HitTestType == HitTestType.TabStrip) && (_tabStrip != null)) && (TabStripEditable && !SpreadSheet.Workbook.Protect))
                    {
                        _tabStrip.StartTabTouchEditing(p_hitInfo.HitPoint);
                        if (_tabStrip.IsEditing)
                        {
                            int sheetTabIndex = (_tabStrip.ActiveTab != null) ? _tabStrip.ActiveTab.SheetIndex : -1;
                            base.RaiseSheetTabDoubleClick(sheetTabIndex);
                        }
                    }
                    else
                    {
                        base.ProcessDoubleTap(p_hitInfo);
                    }
                }
            }
        }
        #endregion

        #region HitTest
        internal override HitTestInformation HitTest(double x, double y)
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
            if (GetTabStripRectangle().Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabStrip;
                return information;
            }
            if (GetTabSplitBoxRectangle().Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabSplitBox;
                return information;
            }
            for (int i = 0; i < spreadLayout.ColumnPaneCount; i++)
            {
                if (GetHorizontalScrollBarRectangle(i).Contains(point))
                {
                    information.ColumnViewportIndex = i;
                    information.HitTestType = HitTestType.HorizontalScrollBar;
                    return information;
                }
            }
            for (int j = 0; j < spreadLayout.RowPaneCount; j++)
            {
                if (GetVerticalScrollBarRectangle(j).Contains(point))
                {
                    information.HitTestType = HitTestType.VerticalScrollBar;
                    information.RowViewportIndex = j;
                    return information;
                }
            }
            for (int k = 0; k < spreadLayout.ColumnPaneCount; k++)
            {
                if (GetHorizontalSplitBoxRectangle(k).Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBox;
                    information.ColumnViewportIndex = k;
                }
            }
            for (int m = 0; m < spreadLayout.RowPaneCount; m++)
            {
                if (GetVerticalSplitBoxRectangle(m).Contains(point))
                {
                    information.HitTestType = HitTestType.RowSplitBox;
                    information.RowViewportIndex = m;
                }
            }
            for (int n = 0; n < (spreadLayout.ColumnPaneCount - 1); n++)
            {
                if (GetHorizontalSplitBarRectangle(n).Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBar;
                    information.ColumnViewportIndex = n;
                }
            }
            for (int num6 = 0; num6 < (spreadLayout.RowPaneCount - 1); num6++)
            {
                if (GetVerticalSplitBarRectangle(num6).Contains(point))
                {
                    information.HitTestType = HitTestType.RowSplitBar;
                    information.RowViewportIndex = num6;
                }
            }
            if (information.HitTestType == HitTestType.Empty)
            {
                information = base.HitTest(x, y);
            }
            return information;
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
        #endregion

        #region 实现触摸滚动
        //****************************************************
        // 触摸时滚动单元格区通过改变 _translateOffsetX _translateOffsetY 的值在 ArrangeOverride 中实现
        // TabStrip 通过改变 Offset 值实现标签滚动
        //****************************************************

        void TouchScrollLeft(Point currentPoint, Point deltaPoint)
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

        void TouchScrollRight(Point currentPoint, Point deltaPoint)
        {
            int maxLeftScrollableColumn = base.GetMaxLeftScrollableColumn();
            int maxRightScrollableColumn = base.GetMaxRightScrollableColumn();
            ColumnLayoutModel columnLayoutModel = base.GetColumnLayoutModel(base._touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.X);
            int viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn <= maxLeftScrollableColumn)
            {
                Point point = currentPoint.Delta(_touchStartPoint);
                base._translateOffsetX = -1.0 * point.X;
                base._translateOffsetX = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetX), 120.0) * Math.Sign(base._translateOffsetX);
                return;
            }
            if ((viewportLeftColumn >= maxRightScrollableColumn) && (currentPoint.X < _touchStartPoint.X))
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
                Point point2 = currentPoint.Delta(_touchStartPoint);
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

        void TouchScrollUp(Point currentPoint, Point deltaPoint)
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

        void TouchScrollBottom(Point currentPoint, Point deltaPoint)
        {
            int maxTopScrollableRow = base.GetMaxTopScrollableRow();
            int maxBottomScrollableRow = base.GetMaxBottomScrollableRow();
            RowLayoutModel rowLayoutModel = base.GetRowLayoutModel(base._touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.Y);
            int viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= maxTopScrollableRow)
            {
                Point point = currentPoint.Delta(_touchStartPoint);
                base._translateOffsetY = -1.0 * point.Y;
                base._translateOffsetY = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetY), 80.0) * Math.Sign(base._translateOffsetY);
                return;
            }
            if ((viewportTopRow >= maxBottomScrollableRow) && (currentPoint.Y < _touchStartPoint.Y))
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
                Point point2 = currentPoint.Delta(_touchStartPoint);
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

        void TouchTabStripScrollLeft(Point currentPoint, Point deltaPoint)
        {
            TabsPresenter tabsPresenter = TabStrip.TabsPresenter;
            int firstScrollableSheetIndex = tabsPresenter.FirstScrollableSheetIndex;
            int lastScrollableSheetIndex = tabsPresenter.LastScrollableSheetIndex;
            int startIndex = tabsPresenter.StartIndex;
            double num2 = Math.Abs(deltaPoint.X);
            if (tabsPresenter.IsLastSheetVisible)
            {
                double num3 = -1.0 * currentPoint.Delta(_touchStartPoint).X;
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

        void TouchTabStripScrollRight(Point currentPoint, Point deltaPoint)
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
                Point point = currentPoint.Delta(_touchStartPoint);
                double num5 = -1.0 * point.X;
                num5 = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(num5), 120.0) * Math.Sign(num5);
                tabsPresenter.Offset = num5;
                return;
            }
            if ((startIndex < lastScrollableSheetIndex) || (currentPoint.X >= _touchStartPoint.X))
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
                        Point point2 = currentPoint.Delta(_touchStartPoint);
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
