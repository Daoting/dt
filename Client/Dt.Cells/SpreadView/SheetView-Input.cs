#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    public partial class SheetView : Panel, IXmlSerializable
    {
        void InitInput()
        {
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            PointerExited += OnPointerExited;
            PointerWheelChanged += OnPointerWheelChanged;
            PointerCaptureLost += OnPointerCaptureLost;
            AddHandler(DoubleTappedEvent, new DoubleTappedEventHandler(OnDoubleTapped), true);

            IsTouchPromotedMouseMessage = false;
            _gestrueRecognizer = new GestureRecognizer();
            _gestrueRecognizer.GestureSettings =
                GestureSettings.DoubleTap
                | GestureSettings.Hold
                | GestureSettings.ManipulationScale
                | GestureSettings.ManipulationTranslateInertia
                | GestureSettings.ManipulationTranslateX
                | GestureSettings.ManipulationTranslateY
                | GestureSettings.Tap;
            _gestrueRecognizer.ShowGestureFeedback = false;
            _gestrueRecognizer.Tapped += OnGestureRecognizerTapped;
            _gestrueRecognizer.ManipulationStarted += OnGestrueRecognizerManipulationStarted;
            _gestrueRecognizer.ManipulationUpdated += OnGestrueRecognizerManipulationUpdated;
            _gestrueRecognizer.ManipulationCompleted += OnGestrueRecognizerManipulationCompleted;

            // 标志已处理，屏蔽外部的左右滑动
            ManipulationMode = ManipulationModes.None | ManipulationModes.TranslateX;
            ManipulationStarted += (s, e) => e.Handled = true;
        }

        #region 鼠标按下
        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                // 触摸模式
                IsTouching = false;
                _isTouchScrolling = false;
                IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
                if ((intermediatePoints != null) && (intermediatePoints.Count > 0))
                {
                    PointerPoint point = intermediatePoints[0];
                    InputDeviceType = Dt.Cells.UI.InputDeviceType.Touch;
                    if (CanTouchManipulate(point.Position))
                    {
                        // 可以看作手势
                        IsTouching = true;
                        _isTouchScrolling = false;
                        if (!_touchProcessedPointIds.Contains(point.PointerId))
                        {
                            if (!_primaryTouchDeviceId.HasValue)
                            {
                                _mouseDownPosition = e.GetCurrentPoint(this).Position;
                                CapturePointer(e.Pointer);
                                _primaryTouchDeviceId = new uint?(point.PointerId);
                                _primaryTouchDeviceReleased = false;
                                if ((Worksheet != null) && (Worksheet.Selections != null))
                                {
                                    CellRange[] rangeArray = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)Worksheet.Selections);
                                    SavedOldSelections = rangeArray;
                                }
                            }
                            _touchProcessedPointIds.Add(point.PointerId);
#if !UWP
                            StopInertialTimer();
#endif

                            try
                            {
                                // 开始手势，不触发事件
                                _gestrueRecognizer.ProcessDownEvent(point);
                                e.Handled = true;
                            }
                            catch
                            {
                                ResetTouchWhenError();
                            }
                        }
                    }
                    else
                    {
                        // 模拟鼠标
                        if (StartMouseClick(e))
                        {
                            _mouseDownPosition = e.GetCurrentPoint(this).Position;
                            var hitInfo = HitTest(_mouseDownPosition.X, _mouseDownPosition.Y);
                            SaveHitInfo(hitInfo);
                            ProcessMouseLeftButtonDown(hitInfo);
                            e.Handled = true;
                        }
                    }
                }
            }
            else
            {
                // 鼠标模式
                InputDeviceType = Dt.Cells.UI.InputDeviceType.Mouse;
                PointerPoint currentPoint = e.GetCurrentPoint(this);
                if (currentPoint.Properties.IsLeftButtonPressed)
                {
                    // 左键
                    Point pos = currentPoint.Position;
                    IsTouching = false;

                    // 无sheet时只底部标签有效
                    if (Worksheet == null && HitTest(pos.X, pos.Y).HitTestType != HitTestType.TabStrip)
                    {
                        return;
                    }

                    if (StartMouseClick(e))
                    {
                        _mouseDownPosition = pos;
                        var hitInfo = HitTest(_mouseDownPosition.X, _mouseDownPosition.Y);
                        SaveHitInfo(hitInfo);
                        ProcessMouseLeftButtonDown(hitInfo);
                        e.Handled = true;
                    }
                }
                IsMouseRightButtonPressed = currentPoint.Properties.IsRightButtonPressed;
            }
        }

        protected virtual bool StartMouseClick(PointerRoutedEventArgs e)
        {
            if (IsMouseInEditor() || IsMouseInRangeGroup())
            {
                return false;
            }

            if (!CapturePointer(e.Pointer))
            {
                return false;
            }

            IsMouseLeftButtonPressed = true;
            if (!_formulaSelectionFeature.IsSelectionBegined)
            {
                FocusInternal();
            }
            _lastClickPoint = e.GetCurrentPoint(this).Position;
            _routedEventArgs = e;
            return true;
        }

        protected virtual void ProcessMouseLeftButtonDown(HitTestInformation p_hitInfo)
        {
            if (!IsEditing
                || _formulaSelectionFeature.CanSelectFormula
                || p_hitInfo.HitTestType == HitTestType.FormulaSelection
                || StopCellEditing(false))
            {
                UpdateLastClickLocation(p_hitInfo);
                _hitFilterInfo = GetMouseDownFilterButton(p_hitInfo, false);
                DataValidationListButtonInfo mouseDownDataValidationButton = GetMouseDownDataValidationButton(p_hitInfo, false);
                if ((_hitFilterInfo != null) || (mouseDownDataValidationButton != null))
                {
                    if (mouseDownDataValidationButton != null)
                    {
                        ProcessMouseDownDataValidationListButton(mouseDownDataValidationButton);
                    }
                    else if (_hitFilterInfo != null)
                    {
                        ProcessMouseDownFilterButton(_hitFilterInfo);
                    }
                }
                else
                {
                    switch (p_hitInfo.HitTestType)
                    {
                        case HitTestType.Corner:
                            {
                                bool flag3 = false;
                                if (_formulaSelectionFeature.IsSelectionBegined)
                                {
                                    flag3 = _formulaSelectionFeature.StartSelecting(SheetArea.CornerHeader);
                                }
                                if (!flag3)
                                {
                                    if (p_hitInfo.HeaderInfo.InRowResize)
                                    {
                                        StartRowResizing();
                                        return;
                                    }
                                    if (p_hitInfo.HeaderInfo.InColumnResize)
                                    {
                                        StartColumnResizing();
                                        return;
                                    }
                                    StartSheetSelecting();
                                }
                                return;
                            }
                        case HitTestType.TabStrip:
                        case HitTestType.RowRangeGroup:
                        case HitTestType.ColumnRangeGroup:
                            return;

                        case HitTestType.RowHeader:
                            {
                                bool flag5 = false;
                                if (_formulaSelectionFeature.IsSelectionBegined)
                                {
                                    flag5 = _formulaSelectionFeature.StartSelecting(SheetArea.CornerHeader | SheetArea.RowHeader);
                                }
                                if (!flag5)
                                {
                                    if (p_hitInfo.HeaderInfo.InRowResize)
                                    {
                                        StartRowResizing();
                                        return;
                                    }
                                    UnSelectedAllFloatingObjects();
                                    StartRowsSelecting();
                                }
                                return;
                            }
                        case HitTestType.ColumnHeader:
                            {
                                bool flag4 = false;
                                if (_formulaSelectionFeature.IsSelectionBegined)
                                {
                                    flag4 = _formulaSelectionFeature.StartSelecting(SheetArea.ColumnHeader);
                                }
                                if (!flag4)
                                {
                                    if (p_hitInfo.HeaderInfo.InColumnResize)
                                    {
                                        StartColumnResizing();
                                        return;
                                    }
                                    UnSelectedAllFloatingObjects();
                                    StartColumnSelecting();
                                }
                                return;
                            }
                        case HitTestType.Viewport:
                            {
                                bool flag6 = false;
                                if (_formulaSelectionFeature.IsSelectionBegined)
                                {
                                    flag6 = _formulaSelectionFeature.StartSelecting(SheetArea.Cells);
                                }
                                if (!flag6)
                                {
                                    if (!_allowDragFill || !p_hitInfo.ViewportInfo.InDragFillIndicator)
                                    {
                                        if (_allowDragDrop && p_hitInfo.ViewportInfo.InSelectionDrag)
                                        {
                                            if (IsEditing)
                                            {
                                                StopCellEditing(false);
                                            }
                                            StartDragDropping();
                                        }
                                        else if (!IsEditing)
                                        {
                                            StartCellSelecting();
                                        }
                                    }
                                    else
                                    {
                                        if (IsEditing)
                                        {
                                            StopCellEditing(false);
                                        }
                                        StartDragFill();
                                    }
                                }
                                UnSelectedAllFloatingObjects();
                                return;
                            }
                        case HitTestType.CornerRangeGroup:
                            {
                                bool flag2 = false;
                                if (_formulaSelectionFeature.IsSelectionBegined)
                                {
                                    flag2 = _formulaSelectionFeature.StartSelecting(SheetArea.CornerHeader);
                                }
                                if (!flag2)
                                {
                                    StartSheetSelecting();
                                }
                                return;
                            }
                        case HitTestType.FloatingObject:
                            if (!IsWorking)
                            {
                                // hdt
                                bool shift;
                                bool ctrl;
                                bool flag9 = false;
                                KeyboardHelper.GetMetaKeyState(out shift, out ctrl);
                                FloatingObject obj = p_hitInfo.FloatingObjectInfo.FloatingObject;
                                if (obj != null)
                                {
                                    // hdt 此处原来为 &&
                                    flag9 = obj.Locked || Worksheet.Protect;
                                    if (!ctrl && !shift)
                                    {
                                        if (!obj.IsSelected && !flag9)
                                        {
                                            UnSelectedAllFloatingObjects();
                                            obj.IsSelected = true;
                                        }
                                    }
                                    else if (!flag9 && !obj.IsSelected)
                                    {
                                        obj.IsSelected = true;
                                        _isMouseDownFloatingObject = true;
                                    }
                                    if (!flag9)
                                    {
                                        if (p_hitInfo.FloatingObjectInfo.InMoving)
                                        {
                                            StartFloatingObjectsMoving();
                                            return;
                                        }
                                        if (((p_hitInfo.FloatingObjectInfo.InBottomNESWResize || p_hitInfo.FloatingObjectInfo.InBottomNSResize) || (p_hitInfo.FloatingObjectInfo.InBottomNWSEResize || p_hitInfo.FloatingObjectInfo.InLeftWEResize)) || ((p_hitInfo.FloatingObjectInfo.InRightWEResize || p_hitInfo.FloatingObjectInfo.InTopNESWResize) || (p_hitInfo.FloatingObjectInfo.InTopNSResize || p_hitInfo.FloatingObjectInfo.InTopNWSEResize)))
                                        {
                                            StartFloatingObjectsResizing();
                                        }
                                    }
                                }
                            }
                            return;

                        case HitTestType.FormulaSelection:
                            if (!p_hitInfo.FormulaSelectionInfo.CanMove)
                            {
                                _formulaSelectionFeature.StartDragResizing();
                                return;
                            }
                            _formulaSelectionFeature.StartDragDropping();
                            return;
                    }
                }
            }
        }
        #endregion

        #region 鼠标移动
        protected virtual void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (IsTouching && e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                try
                {
                    // 触发 ManipulationStarted ManipulationUpdated，uno中并非每次都触发ManipulationUpdated！
                    _gestrueRecognizer.ProcessMoveEvents(e.GetIntermediatePoints(this));
                }
                catch
                {
                    ResetTouchWhenError();
                }
                e.Handled = true;
            }
            else if (Worksheet != null)
            {
                MousePosition = e.GetCurrentPoint(this).Position;
                HitTestInformation hi = HitTest(MousePosition.X, MousePosition.Y);
                if (!IsWorking)
                {
                    ResetMouseCursor();
                }
                ProcessMouseMove(hi);
            }
        }

        protected virtual void ProcessMouseMove(HitTestInformation p_hitInfo)
        {
            switch (p_hitInfo.HitTestType)
            {
                case HitTestType.Corner:
                    if (!IsWorking)
                    {
                        if (p_hitInfo.HeaderInfo.InColumnResize)
                        {
                            SetBuiltInCursor(CoreCursorType.SizeWestEast);
                        }
                        if (p_hitInfo.HeaderInfo.InRowResize)
                        {
                            SetBuiltInCursor(CoreCursorType.SizeNorthSouth);
                        }
                    }
                    break;

                case HitTestType.RowHeader:
                    if (p_hitInfo.HeaderInfo.InRowResize && !IsWorking)
                    {
                        if (Worksheet.GetActualRowHeight(p_hitInfo.HeaderInfo.ResizingRow, SheetArea.Cells) != 0.0)
                        {
                            SetMouseCursor(CursorType.Resize_VerticalCursor);
                            break;
                        }
                        SetMouseCursor(CursorType.Resize_VerticalSplitCursor);
                    }
                    break;

                case HitTestType.ColumnHeader:
                    if (p_hitInfo.HeaderInfo.InColumnResize && !IsWorking)
                    {
                        if (Worksheet.GetActualColumnWidth(p_hitInfo.HeaderInfo.ResizingColumn, SheetArea.Cells) != 0.0)
                        {
                            SetMouseCursor(CursorType.Resize_HorizontalCursor);
                            break;
                        }
                        SetMouseCursor(CursorType.Resize_HorizontalSplitCursor);
                    }
                    break;

                case HitTestType.Viewport:
                    if (IsWorking)
                    {
                        if (IsMovingFloatingOjects)
                        {
                            SetMouseCursor(CursorType.DragCell_DragCursor);
                        }
                        break;
                    }
                    SetCursor(p_hitInfo);
                    break;

                case HitTestType.FloatingObject:
                    if (!IsTouching)
                    {
                        if (IsWorking)
                        {
                            if (IsMovingFloatingOjects)
                            {
                                SetMouseCursor(CursorType.DragCell_DragCursor);
                            }
                            break;
                        }
                        SetCursorForFloatingObject(p_hitInfo.FloatingObjectInfo);
                    }
                    break;

                case HitTestType.FormulaSelection:
                    if (!IsWorking)
                    {
                        _formulaSelectionFeature.SetCursor(p_hitInfo.FormulaSelectionInfo);
                    }
                    break;
            }
            if (_formulaSelectionFeature.IsDragging)
            {
                _formulaSelectionFeature.ContinueDragging();
            }
            if (IsResizingColumns)
            {
                SetMouseCursor(CursorType.Resize_HorizontalCursor);
                ContinueColumnResizing();
            }
            if (IsResizingRows)
            {
                SetMouseCursor(CursorType.Resize_VerticalCursor);
                ContinueRowResizing();
            }
            if (IsSelectingCells)
            {
                ContinueCellSelecting();
            }
            if (IsSelectingColumns)
            {
                ContinueColumnSelecting();
            }
            if (IsSelectingRows)
            {
                ContinueRowSelecting();
            }
            if (IsDragDropping)
            {
                ContinueDragDropping();
            }
            if (IsDraggingFill)
            {
                ContinueDragFill();
            }
            if (IsMovingFloatingOjects)
            {
                ContinueFloatingObjectsMoving();
            }
            if (IsResizingFloatingObjects)
            {
                ContinueFloatingObjectsResizing();
            }
            if (!IsWorking)
            {
                SaveHitInfo(p_hitInfo);
                _hoverManager.DoHover(p_hitInfo);
            }
        }
        #endregion

        #region 鼠标释放
        protected virtual void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
                ResetTouchStates(intermediatePoints);
                if ((intermediatePoints != null) && (intermediatePoints.Count > 0))
                {
                    try
                    {
                        // 触发 ManipulationComplete
                        _gestrueRecognizer.ProcessUpEvent(intermediatePoints[0]);
                        e.Handled = true;
                    }
                    catch
                    {
                        ResetTouchWhenError();
                    }

                    if (!_isTouchScrolling || IsTouchZooming)
                    {
                        try
                        {
                            if (!_primaryTouchDeviceId.HasValue)
                            {
                                _gestrueRecognizer.CompleteGesture();
                            }
                        }
                        catch
                        {
                            ResetTouchWhenError();
                        }
                        IsTouching = false;
                    }
                }
            }
            else if (Worksheet != null)
            {
                bool flag = IsResizingColumns || IsResizingRows || IsDragDropping || IsDraggingFill;
                HitTestInformation hitInfo = HitTest(MousePosition.X, MousePosition.Y);
                if (IsMouseLeftButtonPressed)
                {
                    if (EndMouseClick(e))
                    {
                        ProcessMouseLeftButtonUp(hitInfo);
                    }
                    if (!flag)
                    {
                        RaiseCellClick(MouseButtonType.Left, hitInfo);
                    }
                }
                else if (IsMouseRightButtonPressed)
                {
                    IsMouseRightButtonPressed = false;
                    if (!flag)
                    {
                        RaiseCellClick(MouseButtonType.Right, hitInfo);
                    }
                }

                e.Handled = true;
                if (!IsTouching
                    && e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                {
                    ProcessMouseLeftButtonUp(hitInfo);
                }
            }
        }

        protected virtual bool EndMouseClick(PointerRoutedEventArgs e)
        {
            IsMouseLeftButtonPressed = false;
            ReleasePointerCapture(e.Pointer);
            return true;
        }

        protected virtual void ProcessMouseLeftButtonUp(HitTestInformation p_hitInfo)
        {
            ClearMouseLeftButtonDownStates();
            if (!IsEditing && !_formulaSelectionFeature.IsSelectionBegined)
            {
                FocusInternal();
            }
            if (!IsWorking)
            {
                if ((_allowDragDrop && (p_hitInfo.HitTestType == HitTestType.Viewport)) && p_hitInfo.ViewportInfo.InSelectionDrag)
                {
                    SetBuiltInCursor(CoreCursorType.Hand);
                }
                if (((_lastClickPoint == MousePosition) && (p_hitInfo.HitTestType == HitTestType.FloatingObject)) && (p_hitInfo.FloatingObjectInfo.FloatingObject != null))
                {
                    UnSelectFloatingObject(p_hitInfo.FloatingObjectInfo.FloatingObject);
                }
            }
        }

        void RaiseCellClick(MouseButtonType p_btnType, HitTestInformation p_hitInfo)
        {
            if ((CellClick == null || _eventSuspended == 0))
                return;

            CellClickEventArgs args = null;
            Windows.Foundation.Point point2 = new Windows.Foundation.Point(-1.0, -1.0);
            if (p_hitInfo.HitTestType == HitTestType.Viewport)
            {
                args = CreateCellClickEventArgs(p_hitInfo.ViewportInfo.Row, p_hitInfo.ViewportInfo.Column, Worksheet.SpanModel, SheetArea.Cells, p_btnType);
                point2 = new Windows.Foundation.Point((double)p_hitInfo.ViewportInfo.Row, (double)p_hitInfo.ViewportInfo.Column);
            }
            else if (p_hitInfo.HitTestType == HitTestType.RowHeader)
            {
                args = CreateCellClickEventArgs(p_hitInfo.HeaderInfo.Row, p_hitInfo.HeaderInfo.Column, Worksheet.RowHeaderSpanModel, SheetArea.CornerHeader | SheetArea.RowHeader, p_btnType);
                point2 = new Windows.Foundation.Point((double)p_hitInfo.HeaderInfo.Row, (double)p_hitInfo.HeaderInfo.Column);
            }
            else if (p_hitInfo.HitTestType == HitTestType.ColumnHeader)
            {
                args = CreateCellClickEventArgs(p_hitInfo.HeaderInfo.Row, p_hitInfo.HeaderInfo.Column, Worksheet.ColumnHeaderSpanModel, SheetArea.ColumnHeader, p_btnType);
                point2 = new Windows.Foundation.Point((double)p_hitInfo.HeaderInfo.Row, (double)p_hitInfo.HeaderInfo.Column);
            }

            if (((args != null) && (point2.X != -1.0)) && ((point2.Y != -1.0) && point2.Equals(_lastClickLocation)))
            {
                CellClick(this, args);
            }
        }
        #endregion

        #region 鼠标离开
        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                if (!IsWorking)
                {
                    ResetMouseCursor();
                }
                return;
            }

            IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
            if (!IsTouching && (e.Pointer.PointerDeviceType == PointerDeviceType.Touch))
            {
                if (Worksheet != null)
                {
                    var pt = e.GetCurrentPoint(this).Position;
                    HitTestInformation hitInfo = HitTest(pt.X, pt.Y);
                    ProcessMouseLeftButtonUp(hitInfo);
                }
                return;
            }

            // uno未实现 IsInertial
            if (_gestrueRecognizer.IsActive
                //&& !_gestrueRecognizer.IsInertial
                && !_isTouchScrolling
                && !IsTouchDragFilling
                && !IsTouchDrapDropping
                && !IsTouchSelectingCells
                && !IsTouchSelectingColumns
                && !IsTouchSelectingRows)
            {
                try
                {
                    if (_primaryTouchDeviceId.HasValue)
                    {
                        using (IEnumerator<PointerPoint> enumerator = intermediatePoints.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current.PointerId == _primaryTouchDeviceId.Value)
                                {
                                    _gestrueRecognizer.CompleteGesture();
                                    goto Label_00F2;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    ResetTouchWhenError();
                }
            }
        Label_00F2:
            ResetTouchStates(intermediatePoints);
        }

        protected virtual void ResetTouchStates(IList<PointerPoint> ps)
        {
            if (((ps != null) && (ps.Count > 0)) && _primaryTouchDeviceId.HasValue)
            {
                foreach (PointerPoint point in ps)
                {
                    if (point.PointerId == _primaryTouchDeviceId.Value)
                    {
                        if (_touchStartHitTestInfo == null)
                        {
                            _touchStartHitTestInfo = HitTest(point.Position.X, point.Position.Y);
                        }
                        if (IsTouchResizingColumns)
                        {
                            EndTouchColumnResizing();
                        }
                        if (IsTouchResizingRows)
                        {
                            EndTouchRowResizing();
                        }
                        ResetSelectionFrameStroke();
                    }
                }
            }
            foreach (PointerPoint point2 in ps)
            {
                if (_touchProcessedPointIds.Contains(point2.PointerId))
                {
                    if (_primaryTouchDeviceId.HasValue && (_primaryTouchDeviceId.Value == point2.PointerId))
                    {
                        _primaryTouchDeviceId = null;
                        _primaryTouchDeviceReleased = true;
                    }
                    _touchProcessedPointIds.Remove(point2.PointerId);
                }
            }
            if (!_primaryTouchDeviceId.HasValue)
            {
                _touchProcessedPointIds.Clear();
            }
        }
        #endregion

        #region 鼠标滚轮
        void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if ((Worksheet == null) || (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse))
                return;

            bool flag;
            bool flag2;
            GetSheetLayout();
            int mouseWheelDelta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
            int num2 = (-mouseWheelDelta * 3) / 120;
            int activeRowViewportIndex = GetActiveRowViewportIndex();
            int rowViewportCount = GetViewportInfo().RowViewportCount;
            if (activeRowViewportIndex < 0)
            {
                activeRowViewportIndex = 0;
            }
            else if (activeRowViewportIndex >= rowViewportCount)
            {
                activeRowViewportIndex = rowViewportCount - 1;
            }
            KeyboardHelper.GetMetaKeyState(out flag2, out flag);
            if (flag)
            {
                if (CanUserZoom)
                {
                    float newZoomFactor = ZoomFactor + ((mouseWheelDelta > 0) ? 0.1f : -0.1f);
                    if (newZoomFactor < 0.1f)
                    {
                        newZoomFactor = 0.1f;
                    }
                    if (4f < newZoomFactor)
                    {
                        newZoomFactor = 4f;
                    }
                    float zoomFactor = ZoomFactor;
                    ZoomUndoAction command = new ZoomUndoAction(Worksheet, newZoomFactor);
                    DoCommand(command);
                    PrepareCellEditing();
                    RaiseUserZooming(zoomFactor, newZoomFactor);
                }
            }
            else if ((VerticalScrollable && (Worksheet != null)) && ((activeRowViewportIndex >= 0) && (activeRowViewportIndex < rowViewportCount)))
            {
                int viewportTopRow = GetViewportTopRow(activeRowViewportIndex);
                int num8 = num2 + viewportTopRow;
                num8 = Math.Max(TryGetNextScrollableRow(Worksheet.FrozenRowCount), num8);
                num8 = Math.Min(TryGetPreviousScrollableRow((Worksheet.RowCount - Worksheet.FrozenTrailingRowCount) - 1), num8);
                if (mouseWheelDelta <= 0)
                {
                    num8 = TryGetNextScrollableRow(num8);
                    if (num8 == -1)
                    {
                        num8 = TryGetNextScrollableRow(viewportTopRow + 1);
                    }
                }
                else
                {
                    num8 = TryGetPreviousScrollableRow(num8);
                    if (num8 == -1)
                    {
                        num8 = TryGetPreviousScrollableRow(viewportTopRow - 1);
                    }
                }
                if (num8 != -1)
                {
                    SetViewportTopRow(activeRowViewportIndex, num8);
                }
            }
            e.Handled = true;
        }
        #endregion

        #region 失去捕获
        void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
                if (((intermediatePoints != null) && (intermediatePoints.Count > 0)) && _primaryTouchDeviceId.HasValue)
                {
                    foreach (PointerPoint point in intermediatePoints)
                    {
                        if (_primaryTouchDeviceId.Value == point.PointerId)
                        {
                            _primaryTouchDeviceId = null;
                            _primaryTouchDeviceReleased = true;
                            _touchProcessedPointIds.Clear();
                            break;
                        }
                    }
                }
            }
            if (!IsTouching && (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse))
            {
                ClearMouseLeftButtonDownStates();
                IsWorking = false;
            }
        }
        #endregion

        #region 鼠标双击
        void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (Worksheet == null)
                return;

            HitTestInformation hitInfo = GetHitInfo();
            if (hitInfo.HitTestType != HitTestType.Viewport || IsMouseLeftButtonPressed)
            {
                HitTestInformation information2 = GetHitInfo();
                _isDoubleClick = true;
                EndMouseClick(e);
                ProcessMouseLeftButtonDoubleClick(e);
                _isDoubleClick = false;
                ClearMouseLeftButtonDownStates();
                bool flag = ((information2.HitTestType == HitTestType.Viewport) || (information2.HitTestType == HitTestType.RowHeader)) || (information2.HitTestType == HitTestType.ColumnHeader);
                bool flag2 = ((information2.HitTestType == HitTestType.ColumnHeader) && information2.HeaderInfo.InColumnResize) || ((information2.HitTestType == HitTestType.RowHeader) && information2.HeaderInfo.InRowResize);
                if (flag && !flag2)
                {
                    RaiseCellDoubleClick(e.GetPosition(this));
                }
            }
        }

        protected virtual void ProcessMouseLeftButtonDoubleClick(DoubleTappedRoutedEventArgs e)
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (((savedHitTestInformation.HitTestType == HitTestType.Viewport) && (savedHitTestInformation.ViewportInfo.Row > -1)) && (savedHitTestInformation.ViewportInfo.Column > -1))
            {
                DoubleClickStartCellEditing(savedHitTestInformation.ViewportInfo.Row, savedHitTestInformation.ViewportInfo.Column);
            }
            else if ((savedHitTestInformation.HitTestType == HitTestType.ColumnHeader) && savedHitTestInformation.HeaderInfo.InColumnResize)
            {
                AutoFitColumn();
            }
            else if ((savedHitTestInformation.HitTestType == HitTestType.RowHeader) && savedHitTestInformation.HeaderInfo.InRowResize)
            {
                AutoFitRow();
            }
            else if (savedHitTestInformation.HitTestType == HitTestType.Corner)
            {
                if (savedHitTestInformation.HeaderInfo.InColumnResize)
                {
                    AutoFitColumn();
                }
                else if (savedHitTestInformation.HeaderInfo.InRowResize)
                {
                    AutoFitRow();
                }
            }
        }

        void RaiseCellDoubleClick(Windows.Foundation.Point point)
        {
            if ((CellDoubleClick != null) && (_eventSuspended == 0))
            {
                HitTestInformation information = HitTest(point.X, point.Y);
                CellDoubleClickEventArgs args = null;
                if (information.HitTestType == HitTestType.Viewport)
                {
                    args = new CellDoubleClickEventArgs(SheetArea.Cells, information.ViewportInfo.Row, information.ViewportInfo.Column);
                }
                else if (information.HitTestType == HitTestType.RowHeader)
                {
                    args = new CellDoubleClickEventArgs(SheetArea.CornerHeader | SheetArea.RowHeader, information.HeaderInfo.Row, information.HeaderInfo.Column);
                }
                else if (information.HitTestType == HitTestType.ColumnHeader)
                {
                    args = new CellDoubleClickEventArgs(SheetArea.ColumnHeader, information.HeaderInfo.Row, information.HeaderInfo.Column);
                }
                if (args != null)
                {
                    CellDoubleClick(this, args);
                }
            }
        }
        #endregion

        #region 触摸开始
        void OnGestrueRecognizerManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {
            _touchStartPoint = GetPanelPosition(args.Position);
            OnManipulationStarted();

#if !UWP
            _currentPos = _touchStartPoint;
            _currentTransTime = DateTime.Now;
#endif
        }

        protected virtual void OnManipulationStarted()
        {
            InputDeviceType = Dt.Cells.UI.InputDeviceType.Touch;
            IsTouchingMovingFloatingObjects = false;
            IsTouchingResizingFloatingObjects = false;
            _translateOffsetX = 0.0;
            _translateOffsetY = 0.0;

            if (((_dragFillSmartTag != null) && (_dragFillPopup != null)) && !HitTestPopup(_dragFillPopup, _touchStartPoint))
            {
                CloseDragFillPopup();
            }

            if (_formulaSelectionFeature.TouchHitTest(_touchStartPoint.X, _touchStartPoint.Y, _touchStartHitTestInfo))
            {
                FSelectionFeature.StartDragResizing();
            }
            else
            {
                if ((_touchStartHitTestInfo.HitTestType == HitTestType.Viewport) && (Worksheet.Selections != null))
                {
                    IsTouchSelectingCells = false;
                    if ((!IsEditing && !IsTouchDrapDropping) && !IsTouchDragFilling)
                    {
                        if (AutoFillIndicatorRec.HasValue && AutoFillIndicatorRec.Value.Contains(_touchStartPoint))
                        {
                            StartTouchDragFill();
                        }
                        else if (CachedGripperLocation != null)
                        {
                            if (CachedGripperLocation.TopLeft.Expand(5, 5).Contains(_touchStartPoint))
                            {
                                MoveActiveCellToBottom();
                                StartTouchingSelecting();
                            }
                            else if (CachedGripperLocation.BottomRight.Expand(5, 5).Contains(_touchStartPoint))
                            {
                                StartTouchingSelecting();
                            }
                        }
                    }
                }
                if (((_touchStartHitTestInfo.HitTestType == HitTestType.RowHeader) && !_formulaSelectionFeature.IsSelectionBegined) && !AutoFillIndicatorRec.HasValue)
                {
                    if ((CachedGripperLocation != null) && CachedGripperLocation.TopLeft.Expand(5, 5).Contains(_touchStartPoint))
                    {
                        MoveActiveCellToBottom();
                        StartTouchingSelecting();
                    }
                    else if (ResizerGripperRect.HasValue && ResizerGripperRect.Value.Contains(_touchStartPoint))
                    {
                        StartTouchRowResizing();
                    }
                    else
                    {
                        _IsTouchStartRowSelecting = true;
                    }
                }
                if (((_touchStartHitTestInfo.HitTestType == HitTestType.ColumnHeader) && (GetMouseDownFilterButton(_touchStartHitTestInfo, false) == null)) && (!_formulaSelectionFeature.IsSelectionBegined && !AutoFillIndicatorRec.HasValue))
                {
                    if ((CachedGripperLocation != null) && CachedGripperLocation.TopLeft.Expand(5, 5).Contains(_touchStartPoint))
                    {
                        MoveActiveCellToBottom();
                        StartTouchingSelecting();
                    }
                    else if (ResizerGripperRect.HasValue && ResizerGripperRect.Value.Contains(_touchStartPoint))
                    {
                        StartTouchColumnResizing();
                    }
                    else
                    {
                        _IsTouchStartColumnSelecting = true;
                    }
                }
            }
        }
        #endregion

        #region 触摸移动
        void OnGestrueRecognizerManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
#if UWP
            double num = args.Velocities.Linear.Y * 1000.0;
            double num2 = args.Velocities.Linear.X * 1000.0;
            if ((Math.Abs(num) > 1200.0) || (Math.Abs(num2) > 1700.0))
            {
                _fastScroll = true;
            }
            else
            {
                _fastScroll = false;
            }

            if (sender.IsInertial)
                OnInertialManipulationUpdated(args);
            else
                ProcessGestrueRecognizerManipulationUpdated(args);
#else
            // uno未实现 Velocities IsInertial等，无_fastScroll情况
            if (IsZero(args.Cumulative.Scale - 1f) || (_touchProcessedPointIds.Count == 1))
            {
                _lastPos = _currentPos;
                _currentPos = GetPanelPosition(args.Position);
                _lastTransTime = _currentTransTime;
                _currentTransTime = DateTime.Now;
                ProcessTouchFreeDrag(_currentPos, new Point(-args.Delta.Translation.X, -args.Delta.Translation.Y));
            }
            else
            {
                ProcessGestrueRecognizerManipulationUpdated(args);
            }
#endif
        }

        void OnInertialManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            int viewportTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
            if ((viewportTopRow <= GetMaxTopScrollableRow()) && (_translateOffsetY > 0.0))
            {
                _translateOffsetY = 0.0;
                try
                {
                    _gestrueRecognizer.CompleteGesture();
                }
                catch
                {
                    ResetTouchWhenError();
                }
            }
            else if ((viewportTopRow >= GetMaxBottomScrollableRow()) && ((_translateOffsetY + Worksheet.Rows[GetMaxBottomScrollableRow()].ActualHeight) < 0.0))
            {
                try
                {
                    _gestrueRecognizer.CompleteGesture();
                }
                catch
                {
                    ResetTouchWhenError();
                }
            }
            else
            {
                int viewportLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
                if ((viewportLeftColumn <= GetMaxLeftScrollableColumn()) && (_translateOffsetX > 0.0))
                {
                    _translateOffsetX = 0.0;
                    try
                    {
                        _gestrueRecognizer.CompleteGesture();
                    }
                    catch
                    {
                        ResetTouchWhenError();
                    }
                }
                else
                {
                    if ((viewportLeftColumn >= GetMaxRightScrollableColumn()) && ((_translateOffsetX + Worksheet.Columns[GetMaxRightScrollableColumn()].ActualWidth) < 0.0))
                    {
                        try
                        {
                            _gestrueRecognizer.CompleteGesture();
                        }
                        catch
                        {
                            ResetTouchWhenError();
                        }
                    }
                    ProcessGestrueRecognizerManipulationUpdated(args);
                }
            }
        }

        protected virtual void ProcessGestrueRecognizerManipulationUpdated(ManipulationUpdatedEventArgs e)
        {
        }

        protected virtual void ProcessTouchFreeDrag(Point p_curPos, Point p_offset)
        {
            if (!IsTouching)
                return;

            HideMouseCursor();
            if (IsTouchDragFilling)
            {
                ContinueTouchDragFill();
            }
            else if (IsTouchDrapDropping)
            {
                ContinueTouchDragDropping();
            }
            else if (FSelectionFeature.IsDragging)
            {
                FSelectionFeature.ContinueDragging();
            }
            else if (IsTouchSelectingCells)
            {
                ContinueTouchSelectingCells(MousePosition);
            }
            else if (_IsTouchStartColumnSelecting)
            {
                if (!StopCellEditing(false))
                {
                    return;
                }
                UnSelectedAllFloatingObjects();
                StartColumnSelecting();
                _IsTouchStartColumnSelecting = false;
            }
            else if (IsTouchResizingColumns)
            {
                ContinueTouchColumnResizing();
            }
            else if (_IsTouchStartRowSelecting)
            {
                if (!StopCellEditing(false))
                {
                    return;
                }
                UnSelectedAllFloatingObjects();
                StartRowsSelecting();
                _IsTouchStartRowSelecting = false;
            }
            else if (IsTouchSelectingColumns)
            {
                ContinueColumnSelecting();
            }
            else if (IsTouchResizingRows)
            {
                ContinueTouchRowResizing();
            }
            else if (IsTouchSelectingRows)
            {
                ContinueRowSelecting();
            }

            HitTestInformation information = _touchStartHitTestInfo;
            if (((information.HitTestType == HitTestType.FloatingObject) && (information.FloatingObjectInfo != null)) && ((information.FloatingObjectInfo.FloatingObject != null) && information.FloatingObjectInfo.FloatingObject.IsSelected))
            {
                if (IsTouchingMovingFloatingObjects)
                {
                    ContinueFloatingObjectsMoving();
                }
                else if (IsTouchingResizingFloatingObjects)
                {
                    ContinueFloatingObjectsResizing();
                }
                else if (information.FloatingObjectInfo.InMoving)
                {
                    StartFloatingObjectsMoving();
                }
                else if (((information.FloatingObjectInfo.InBottomNESWResize || information.FloatingObjectInfo.InBottomNSResize) || (information.FloatingObjectInfo.InBottomNWSEResize || information.FloatingObjectInfo.InLeftWEResize)) || ((information.FloatingObjectInfo.InRightWEResize || information.FloatingObjectInfo.InTopNESWResize) || (information.FloatingObjectInfo.InTopNSResize || information.FloatingObjectInfo.InTopNWSEResize)))
                {
                    StartFloatingObjectsResizing();
                }
            }
        }
        #endregion

        #region 触摸结束
        void OnGestrueRecognizerManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
#if UWP
            OnManipulationComplete();
#else
            // uno中无惯性效果，模拟
            Point curPos = GetPanelPosition(args.Position);
            double totalTime = (DateTime.Now - _lastTransTime).TotalMilliseconds;
            double speedX = (curPos.X - _lastPos.X) / totalTime * _inertialInterval;
            double speedY = (curPos.Y - _lastPos.Y) / totalTime * _inertialInterval;
            if (IsZero(args.Cumulative.Scale - 1f) && (Math.Abs(speedX) > _acceleration || Math.Abs(speedY) > _acceleration))
            {
                // 利用瞬时加速度
                _inertialSpeed = new Point(Math.Abs(speedX) > _acceleration ? speedX : 0, Math.Abs(speedY) > _acceleration ? speedY : 0.0);
                _inertialPos = curPos;
                StartInertialTimer();
            }
            else
            {
                OnManipulationComplete();
            }
#endif
        }

        protected virtual void OnManipulationComplete()
        {
            IsTouching = false;
            IsTouchPromotedMouseMessage = false;
            if (FSelectionFeature.IsDragging)
            {
                FSelectionFeature.EndDragging();
            }

            if (IsTouchSelectingCells)
            {
                EndTouchSelectingCells();
                if (InputDeviceType == Dt.Cells.UI.InputDeviceType.Touch)
                {
                    RefreshSelection();
                }
            }

            if (IsTouchDragFilling)
            {
                EndTouchDragFill();
                CloseAutoFilterIndicator();
                if (_resetSelectionFrameStroke)
                {
                    GcViewport viewportRowsPresenter = GetViewportRowsPresenter(_touchStartHitTestInfo.RowViewportIndex, _touchStartHitTestInfo.ColumnViewportIndex);
                    if (viewportRowsPresenter != null)
                    {
                        viewportRowsPresenter.SelectionContainer.ResetSelectionFrameStroke();
                    }
                    _resetSelectionFrameStroke = false;
                }
            }

            if (IsTouchDrapDropping)
            {
                EndTouchDragDropping();
                if (_resetSelectionFrameStroke)
                {
                    GcViewport viewport2 = GetViewportRowsPresenter(_touchStartHitTestInfo.RowViewportIndex, _touchStartHitTestInfo.ColumnViewportIndex);
                    if (viewport2 != null)
                    {
                        viewport2.SelectionContainer.ResetSelectionFrameStroke();
                    }
                    _resetSelectionFrameStroke = false;
                }
            }

            if (IsTouchSelectingColumns)
            {
                EndColumnSelecting();
            }
            if (IsTouchResizingColumns)
            {
                EndTouchColumnResizing();
            }
            if (IsTouchResizingRows)
            {
                EndTouchRowResizing();
            }
            if (IsTouchSelectingRows)
            {
                EndRowSelecting();
            }
            if (IsTouchingMovingFloatingObjects)
            {
                EndFloatingObjectsMoving();
            }
            if (IsTouchingResizingFloatingObjects)
            {
                EndFloatingObjectResizing();
            }
            _cachedViewportSplitBarY = null;
            _cachedViewportSplitBarX = null;
            _cachedViewportWidths = null;
            _cachedViewportHeights = null;
            _updateViewportAfterTouch = false;
            _touchStartLeftColumn = -1;
            _touchStartTopRow = -1;
            _IsTouchStartColumnSelecting = false;
            _IsTouchStartRowSelecting = false;
            _isTouchScrolling = false;
            IsTouchDragFilling = false;
            IsTouchDrapDropping = false;
            IsTouchingMovingFloatingObjects = false;
            IsTouchingResizingFloatingObjects = false;
        }

#if !UWP
        //*********************uno中模拟惯性效果
        // 1秒25帧
        const double _inertialInterval = 40;
        // 每帧的加速度
        const double _acceleration = 4.0;
        Point _currentPos;
        Point _lastPos;
        DateTime _lastTransTime;
        DateTime _currentTransTime;
        Point _inertialSpeed;
        Point _inertialPos;
        DispatcherTimer _inertialTimer;
        bool _isInertialing;

        void StartInertialTimer()
        {

            if (_inertialTimer == null)
            {
                _inertialTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_inertialInterval) };
                _inertialTimer.Tick += OnInertialTimerTick;
            }
            _isInertialing = true;
            _inertialTimer.Start();

        }

        void StopInertialTimer()
        {
            if (_isInertialing)
            {
                _isInertialing = false;
                _inertialTimer.Stop();
                OnManipulationComplete();
            }
        }

        void OnInertialTimerTick(object sender, object e)
        {
            // 是否已超出行列范围
            bool stop = false;
            int viewportTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= GetMaxTopScrollableRow() && _translateOffsetY > 0.0)
            {
                _translateOffsetY = 0.0;
                stop = true;
            }
            else if ((viewportTopRow >= GetMaxBottomScrollableRow()) && ((_translateOffsetY + Worksheet.Rows[GetMaxBottomScrollableRow()].ActualHeight) < 0.0))
            {
                stop = true;
            }
            else
            {
                int viewportLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
                if ((viewportLeftColumn <= GetMaxLeftScrollableColumn()) && (_translateOffsetX > 0.0))
                {
                    _translateOffsetX = 0.0;
                    stop = true;
                }
                else if ((viewportLeftColumn >= GetMaxRightScrollableColumn()) && ((_translateOffsetX + Worksheet.Columns[GetMaxRightScrollableColumn()].ActualWidth) < 0.0))
                {
                    stop = true;
                }
            }
            if (stop)
            {
                StopInertialTimer();
                return;
            }

            if (_inertialSpeed.Y != 0)
            {
                if (_inertialSpeed.Y > _acceleration / 2)
                {
                    _inertialSpeed.Y -= _acceleration;
                }
                else if (_inertialSpeed.Y < -_acceleration / 2)
                {
                    _inertialSpeed.Y += _acceleration;
                }
                else
                {
                    _inertialSpeed.Y = 0;
                }
                _inertialPos.Y += _inertialSpeed.Y;
            }

            if (_inertialSpeed.X != 0)
            {
                if (_inertialSpeed.X > _acceleration / 2)
                {
                    _inertialSpeed.X -= _acceleration;
                }
                else if (_inertialSpeed.X < -_acceleration / 2)
                {
                    _inertialSpeed.X += _acceleration;
                }
                else
                {
                    _inertialSpeed.X = 0;
                }
                _inertialPos.X += _inertialSpeed.X;
            }

            if (_inertialSpeed.X == 0 && _inertialSpeed.Y == 0)
            {
                StopInertialTimer();
            }
            else
            {
                // 后面的offset只用来做判断
                ProcessTouchFreeDrag(_inertialPos, new Point(-_inertialSpeed.X, -_inertialSpeed.Y));
            }
        }
#endif
        #endregion

        #region 触摸单/双击
        void OnGestureRecognizerTapped(GestureRecognizer sender, TappedEventArgs args)
        {
            // 鼠标操作时不调用！
            UpdateHitInfo(args.Position);
            HitTestInformation hitInfo = GetHitInfo();

            if (args.TapCount == 1)
            {
                ProcessTap(hitInfo);
                InvalidateMeasure();
            }
            else if (args.TapCount == 2)
            {
                ProcessDoubleTap(hitInfo);
            }
        }

        protected virtual void ProcessTap(HitTestInformation p_hitInfo)
        {
            if ((GetActiveSelection() == null) && (Worksheet.Selections.Count > 0))
            {
                CellRange range = Worksheet.Selections[0];
            }
            if ((IsEditing && !_formulaSelectionFeature.CanSelectFormula) && !StopCellEditing(false))
            {
                return;
            }

            CloseAutoFilterIndicator();
            _hitFilterInfo = GetMouseDownFilterButton(p_hitInfo, true);
            DataValidationListButtonInfo mouseDownDataValidationButton = GetMouseDownDataValidationButton(p_hitInfo, true);
            if ((_hitFilterInfo != null) || (mouseDownDataValidationButton != null))
            {
                if (mouseDownDataValidationButton != null)
                {
                    ProcessMouseDownDataValidationListButton(mouseDownDataValidationButton);
                    return;
                }
                if (_hitFilterInfo != null)
                {
                    ProcessMouseDownFilterButton(_hitFilterInfo);
                    return;
                }
                return;
            }

            switch (p_hitInfo.HitTestType)
            {
                case HitTestType.Corner:
                case HitTestType.CornerRangeGroup:
                    UnSelectedAllFloatingObjects();
                    StartSheetSelecting();
                    goto Label_0568;

                case HitTestType.RowHeader:
                    if (p_hitInfo.HeaderInfo.InRowResize)
                    {
                        if (TapInSelectionRow(p_hitInfo.HeaderInfo.Row) && !p_hitInfo.HeaderInfo.InRowResize)
                        {
                            TapInRowHeaderSelection(p_hitInfo.HitPoint, p_hitInfo);
                        }
                        else
                        {
                            UnSelectedAllFloatingObjects();
                            StartRowsSelecting();
                            EndRowSelecting();
                            RaiseTouchCellClick(p_hitInfo);
                        }
                    }
                    else if (IsEditing)
                    {
                        if (_formulaSelectionFeature.IsSelectionBegined)
                        {
                            _formulaSelectionFeature.TouchSelect(SheetArea.CornerHeader | SheetArea.RowHeader);
                        }
                    }
                    else if (!TapInSelectionRow(p_hitInfo.HeaderInfo.Row))
                    {
                        if (_formulaSelectionFeature.IsSelectionBegined)
                        {
                            _formulaSelectionFeature.TouchSelect(SheetArea.CornerHeader | SheetArea.RowHeader);
                        }
                        else if (p_hitInfo.HeaderInfo.Row > -1)
                        {
                            UnSelectedAllFloatingObjects();
                            StartRowsSelecting();
                            EndRowSelecting();
                            RaiseTouchCellClick(p_hitInfo);
                        }
                    }
                    else
                    {
                        TapInRowHeaderSelection(p_hitInfo.HitPoint, p_hitInfo);
                    }
                    goto Label_0568;

                case HitTestType.ColumnHeader:
                    if (_hitFilterInfo == null)
                    {
                        if (p_hitInfo.HeaderInfo.InColumnResize)
                        {
                            if (TapInSelectionColumn(p_hitInfo.HeaderInfo.Column) && !p_hitInfo.HeaderInfo.InColumnResize)
                            {
                                TapInColumnHeaderSelection(p_hitInfo.HitPoint, p_hitInfo);
                            }
                            else
                            {
                                UnSelectedAllFloatingObjects();
                                StartColumnSelecting();
                                EndColumnSelecting();
                                RaiseTouchCellClick(p_hitInfo);
                            }
                        }
                        else if (IsEditing)
                        {
                            if (_formulaSelectionFeature.IsSelectionBegined)
                            {
                                _formulaSelectionFeature.TouchSelect(SheetArea.ColumnHeader);
                            }
                        }
                        else if (TapInSelectionColumn(p_hitInfo.HeaderInfo.Column))
                        {
                            TapInColumnHeaderSelection(p_hitInfo.HitPoint, p_hitInfo);
                        }
                        else if (_formulaSelectionFeature.IsSelectionBegined)
                        {
                            _formulaSelectionFeature.TouchSelect(SheetArea.ColumnHeader);
                        }
                        else if (p_hitInfo.HeaderInfo.Column > -1)
                        {
                            UnSelectedAllFloatingObjects();
                            StartColumnSelecting();
                            EndColumnSelecting();
                            RaiseTouchCellClick(p_hitInfo);
                        }
                    }
                    goto Label_0568;

                case HitTestType.Viewport:
                    if (_dragFillSmartTag != null)
                    {
                        if ((_dragFillPopup == null) || !HitTestPopup(_dragFillPopup, p_hitInfo.HitPoint))
                        {
                            if (_dragFillSmartTag.IsContextMenuOpened && (_dragFillSmartTag.GetTappedDragFillContextMenu(p_hitInfo.HitPoint) != null))
                            {
                                DragFillContextMenuItem tappedDragFillContextMenu = _dragFillSmartTag.GetTappedDragFillContextMenu(p_hitInfo.HitPoint);
                                if (tappedDragFillContextMenu != null)
                                {
                                    tappedDragFillContextMenu.Click();
                                }
                            }
                            else
                            {
                                CloseDragFillPopup();
                                if (!IsEditing)
                                {
                                    UnSelectedAllFloatingObjects();
                                    StartTapSelectCells();
                                    EndTouchSelectingCells();
                                    RaiseTouchCellClick(p_hitInfo);
                                }
                            }
                            break;
                        }
                        _dragFillSmartTag.DragFillSmartTagTap(p_hitInfo.HitPoint);
                    }
                    break;

                case HitTestType.FloatingObject:
                    if (!IsWorking)
                    {
                        // hdt 添加判断
                        bool flag2 = false;
                        FloatingObject obj = p_hitInfo.FloatingObjectInfo.FloatingObject;
                        if (obj != null)
                        {
                            bool flag9 = obj.Locked || Worksheet.Protect;
                            if (!obj.IsSelected && !flag9)
                            {
                                UnSelectedAllFloatingObjects();
                                obj.IsSelected = true;
                                flag2 = true;
                            }
                        }

                        if (!flag2)
                        {
                            RaiseTouchToolbarOpeningEvent(p_hitInfo.HitPoint, TouchToolbarShowingArea.FloatingObjects);
                        }
                    }
                    goto Label_0568;

                default:
                    goto Label_0568;
            }
            if (((_dataValidationListPopUp != null) && _dataValidationListPopUp.IsOpen) && (_dataValidationPopUpHelper != null))
            {
                _dataValidationPopUpHelper.Close();
                if (!IsEditing)
                {
                    UnSelectedAllFloatingObjects();
                    StartTapSelectCells();
                    EndTouchSelectingCells();
                    RaiseTouchCellClick(p_hitInfo);
                }
            }
            if (((_filterPopup != null) && _filterPopup.IsOpen) && (_filterPopupHelper != null))
            {
                _filterPopupHelper.Close();
                if (!IsEditing)
                {
                    UnSelectedAllFloatingObjects();
                    StartTapSelectCells();
                    EndTouchSelectingCells();
                    RaiseTouchCellClick(p_hitInfo);
                }
            }
            else if (!IsEditing)
            {
                if (TapInSelection(p_hitInfo.HitPoint))
                {
                    FloatingObject[] allSelectedFloatingObjects = GetAllSelectedFloatingObjects();
                    if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Length > 0))
                    {
                        UnSelectedAllFloatingObjects();
                        StartTapSelectCells();
                        EndTouchSelectingCells();
                        RaiseTouchCellClick(p_hitInfo);
                        UpdateTouchSelectionGripper();
                    }
                    else
                    {
                        TouchToolbarShowingArea cells = TouchToolbarShowingArea.Cells;
                        if (IsEntrieColumnSelection() && IsEntrieRowSelection())
                        {
                            cells = TouchToolbarShowingArea.Cells;
                        }
                        else if (IsEntrieColumnSelection())
                        {
                            cells = TouchToolbarShowingArea.Columns;
                        }
                        else if (IsEntrieRowSelection())
                        {
                            cells = TouchToolbarShowingArea.Rows;
                        }
                        if (IsEntrieColumnSelection() && IsEntrieRowSelection())
                        {
                            StartTapSelectCells();
                            EndTouchSelectingCells();
                            RaiseTouchCellClick(p_hitInfo);
                            UpdateTouchSelectionGripper();
                        }
                        else
                        {
                            RaiseTouchToolbarOpeningEvent(p_hitInfo.HitPoint, cells);
                        }
                    }
                }
                else if (_formulaSelectionFeature.IsSelectionBegined)
                {
                    _formulaSelectionFeature.TouchSelect(SheetArea.Cells);
                }
                else
                {
                    StartTapSelectCells();
                    EndTouchSelectingCells();
                    RaiseTouchCellClick(p_hitInfo);
                }
            }
            else if (_formulaSelectionFeature.IsSelectionBegined)
            {
                _formulaSelectionFeature.TouchSelect(SheetArea.Cells);
            }
            UnSelectedAllFloatingObjects();
        Label_0568:
            if (!CanSelectFormula)
            {
                FocusInternal();
            }
        }

        protected virtual void ProcessDoubleTap(HitTestInformation p_hitInfo)
        {
            if (p_hitInfo.HitTestType == HitTestType.Viewport
                && p_hitInfo.ViewportInfo.Row > -1
                && p_hitInfo.ViewportInfo.Column > -1)
            {
                if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
                {
                    _touchToolbarPopup.IsOpen = false;
                }
                SetSelection(p_hitInfo.ViewportInfo.Row, p_hitInfo.ViewportInfo.Column, 1, 1);
                DoubleClickStartCellEditing(p_hitInfo.ViewportInfo.Row, p_hitInfo.ViewportInfo.Column);
                RaiseCellDoubleClick(p_hitInfo.HitPoint);
                RefreshSelection();
            }
            else
            {
                p_hitInfo = TouchHitTest(p_hitInfo.HitPoint.X, p_hitInfo.HitPoint.Y);
                if (p_hitInfo.HitTestType == HitTestType.ColumnHeader)
                {
                    AutoFitColumnForTouch(p_hitInfo);
                }
                else if (p_hitInfo.HitTestType == HitTestType.RowHeader)
                {
                    AutoFitRowForTouch(p_hitInfo);
                }
                else if (p_hitInfo.HitTestType == HitTestType.Corner)
                {
                    if (p_hitInfo.HeaderInfo.InColumnResize)
                    {
                        AutoFitColumnForTouch(p_hitInfo);
                    }
                    else if (p_hitInfo.HeaderInfo.InRowResize)
                    {
                        AutoFitRowForTouch(p_hitInfo);
                    }
                }
            }
        }
        #endregion

        #region HitTest
        protected HitTestInformation GetHitInfo()
        {
            if (_positionInfo == null)
            {
                _positionInfo = new HitTestInformation();
            }
            return _positionInfo;
        }

        protected void SaveHitInfo(HitTestInformation hitTestInfo)
        {
            _positionInfo = hitTestInfo;
        }

        protected void UpdateHitInfo(Point point)
        {
            HitTestInformation hitInfo = GetHitInfo();
            if (point != hitInfo.HitPoint)
            {
                hitInfo = HitTest(point.X, point.Y);
                SaveHitInfo(hitInfo);
            }
            _lastClickPoint = point;
        }

        internal virtual HitTestInformation HitTest(double x, double y)
        {
            Windows.Foundation.Point hitPoint = new Windows.Foundation.Point(x, y);
            HitTestInformation hi = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = hitPoint
            };
            ViewportInfo viewportInfo = GetViewportInfo();
            int columnViewportCount = viewportInfo.ColumnViewportCount;
            int rowViewportCount = viewportInfo.RowViewportCount;
            if (IsCornerRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.CornerRangeGroup;
                return hi;
            }
            if (IsRowRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.RowRangeGroup;
                return hi;
            }
            if (IsColumnRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.ColumnRangeGroup;
                return hi;
            }
            if (GetCornerRectangle().Contains(hitPoint))
            {
                HeaderHitTestInformation information2 = new HeaderHitTestInformation
                {
                    Column = -1,
                    Row = -1,
                    ResizingColumn = -1,
                    ResizingRow = -1
                };
                hi.HitTestType = HitTestType.Corner;
                hi.HeaderInfo = information2;
                ColumnLayout rowHeaderColumnLayoutFromX = GetRowHeaderColumnLayoutFromX(hitPoint.X);
                RowLayout columnHeaderRowLayoutFromY = GetColumnHeaderRowLayoutFromY(hitPoint.Y);
                ColumnLayout rowHeaderResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(hitPoint.X);
                RowLayout columnHeaderResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(hitPoint.Y);
                if (rowHeaderColumnLayoutFromX != null)
                {
                    information2.Column = rowHeaderColumnLayoutFromX.Column;
                }
                if (columnHeaderRowLayoutFromY != null)
                {
                    information2.Row = columnHeaderRowLayoutFromY.Row;
                }
                if (rowHeaderResizingColumnLayoutFromX != null)
                {
                    information2.InColumnResize = true;
                    information2.ResizingColumn = rowHeaderResizingColumnLayoutFromX.Column;
                }
                if (columnHeaderResizingRowLayoutFromY != null)
                {
                    information2.InRowResize = true;
                    information2.ResizingRow = columnHeaderResizingRowLayoutFromY.Row;
                }
                return hi;
            }
            for (int i = -1; i <= rowViewportCount; i++)
            {
                if (GetRowHeaderRectangle(i).Contains(hitPoint))
                {
                    HeaderHitTestInformation information4 = new HeaderHitTestInformation
                    {
                        Row = -1,
                        Column = -1,
                        ResizingColumn = -1,
                        ResizingRow = -1
                    };
                    hi.HitTestType = HitTestType.RowHeader;
                    hi.RowViewportIndex = i;
                    hi.HeaderInfo = information4;
                    if (((InputDeviceType == Dt.Cells.UI.InputDeviceType.Touch) && ResizerGripperRect.HasValue) && ResizerGripperRect.Value.Contains(hitPoint))
                    {
                        hi.HeaderInfo.InRowResize = true;
                        hi.HeaderInfo.ResizingRow = GetActiveCell().Row;
                        return hi;
                    }
                    ColumnLayout layout5 = GetRowHeaderColumnLayoutFromX(hitPoint.X);
                    RowLayout viewportRowLayoutFromY = GetViewportRowLayoutFromY(i, hitPoint.Y);
                    RowLayout viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(i, hitPoint.Y);
                    if ((viewportResizingRowLayoutFromY == null) && (hi.RowViewportIndex == 0))
                    {
                        viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(-1, hi.HitPoint.Y);
                    }
                    if ((viewportResizingRowLayoutFromY == null) && ((hi.RowViewportIndex == 0) || (hi.RowViewportIndex == -1)))
                    {
                        viewportResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(hi.HitPoint.Y);
                        if (viewportResizingRowLayoutFromY != null)
                        {
                            hi.HitTestType = HitTestType.Corner;
                        }
                    }
                    if (layout5 != null)
                    {
                        information4.Column = layout5.Column;
                    }
                    if (viewportRowLayoutFromY != null)
                    {
                        information4.Row = viewportRowLayoutFromY.Row;
                    }
                    if ((viewportResizingRowLayoutFromY != null) && (((viewportResizingRowLayoutFromY.Height > 0.0) || (viewportResizingRowLayoutFromY.Row >= Worksheet.RowCount)) || !Worksheet.RowRangeGroup.IsCollapsed(viewportResizingRowLayoutFromY.Row)))
                    {
                        information4.InRowResize = true;
                        information4.ResizingRow = viewportResizingRowLayoutFromY.Row;
                    }
                    return hi;
                }
            }
            for (int j = -1; j <= columnViewportCount; j++)
            {
                if (GetColumnHeaderRectangle(j).Contains(hitPoint))
                {
                    HeaderHitTestInformation information6 = new HeaderHitTestInformation
                    {
                        Row = -1,
                        Column = -1,
                        ResizingRow = -1,
                        ResizingColumn = -1
                    };
                    hi.HitTestType = HitTestType.ColumnHeader;
                    hi.HeaderInfo = information6;
                    hi.ColumnViewportIndex = j;
                    if (((InputDeviceType == Dt.Cells.UI.InputDeviceType.Touch) && ResizerGripperRect.HasValue) && ResizerGripperRect.Value.Contains(hitPoint))
                    {
                        hi.HeaderInfo.InColumnResize = true;
                        hi.HeaderInfo.ResizingColumn = GetActiveCell().Column;
                        return hi;
                    }
                    ColumnLayout viewportColumnLayoutFromX = GetViewportColumnLayoutFromX(j, hitPoint.X);
                    RowLayout layout9 = GetColumnHeaderRowLayoutFromY(hitPoint.Y);
                    ColumnLayout viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(j, hitPoint.X);
                    if (viewportResizingColumnLayoutFromX == null)
                    {
                        if (hi.ColumnViewportIndex == 0)
                        {
                            viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(-1, hitPoint.X);
                        }
                        if ((viewportResizingColumnLayoutFromX == null) && ((hi.ColumnViewportIndex == 0) || (hi.ColumnViewportIndex == -1)))
                        {
                            viewportResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(hitPoint.X);
                            if (viewportResizingColumnLayoutFromX != null)
                            {
                                hi.HitTestType = HitTestType.Corner;
                            }
                        }
                    }
                    if (viewportColumnLayoutFromX != null)
                    {
                        hi.HeaderInfo.Column = viewportColumnLayoutFromX.Column;
                    }
                    if (layout9 != null)
                    {
                        hi.HeaderInfo.Row = layout9.Row;
                    }
                    if ((viewportResizingColumnLayoutFromX != null) && (((viewportResizingColumnLayoutFromX.Width > 0.0) || (viewportResizingColumnLayoutFromX.Column >= Worksheet.ColumnCount)) || !Worksheet.ColumnRangeGroup.IsCollapsed(viewportResizingColumnLayoutFromX.Column)))
                    {
                        hi.HeaderInfo.InColumnResize = true;
                        hi.HeaderInfo.ResizingColumn = viewportResizingColumnLayoutFromX.Column;
                    }
                    return hi;
                }
            }
            for (int k = -1; k <= rowViewportCount; k++)
            {
                for (int m = -1; m <= columnViewportCount; m++)
                {
                    if (GetViewportRectangle(k, m).Contains(hitPoint))
                    {
                        hi.ColumnViewportIndex = m;
                        hi.RowViewportIndex = k;
                        ViewportHitTestInformation information8 = new ViewportHitTestInformation
                        {
                            Column = -1,
                            Row = -1
                        };
                        hi.HitTestType = HitTestType.Viewport;
                        hi.ViewportInfo = information8;
                        ColumnLayout layout11 = GetViewportColumnLayoutFromX(m, hitPoint.X);
                        RowLayout layout12 = GetViewportRowLayoutFromY(k, hitPoint.Y);
                        if (layout11 != null)
                        {
                            hi.ViewportInfo.Column = layout11.Column;
                        }
                        if (layout12 != null)
                        {
                            hi.ViewportInfo.Row = layout12.Row;
                        }
                        if (!_formulaSelectionFeature.HitTest(k, m, hitPoint.X, hitPoint.Y, hi) && (IsInSelectionGripper(new Windows.Foundation.Point(x, y)) || !HitTestFloatingObject(k, m, hitPoint.X, hitPoint.Y, hi)))
                        {
                            GcViewport viewportRowsPresenter = GetViewportRowsPresenter(k, m);
                            if ((layout11 != null) && (layout12 != null))
                            {
                                if (IsMouseInDragFillIndicator(hitPoint.X, hitPoint.Y, k, m, false))
                                {
                                    hi.ViewportInfo.InDragFillIndicator = true;
                                }
                                else if (IsMouseInDragDropLocation(hitPoint.X, hitPoint.Y, k, m, false))
                                {
                                    hi.ViewportInfo.InSelectionDrag = true;
                                }
                            }
                            if (((IsEditing && !hi.ViewportInfo.InSelectionDrag) && (!hi.ViewportInfo.InDragFillIndicator && (viewportRowsPresenter != null))) && viewportRowsPresenter.EditorBounds.Contains(new Windows.Foundation.Point(x - viewportRowsPresenter.Location.X, y - viewportRowsPresenter.Location.Y)))
                            {
                                hi.ViewportInfo.InEditor = true;
                            }
                            return hi;
                        }
                    }
                }
            }
            return hi;
        }

        internal virtual HitTestInformation TouchHitTest(double x, double y)
        {
            Windows.Foundation.Point hitPoint = new Windows.Foundation.Point(x, y);
            HitTestInformation hi = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = hitPoint
            };
            ViewportInfo viewportInfo = GetViewportInfo();
            int columnViewportCount = viewportInfo.ColumnViewportCount;
            int rowViewportCount = viewportInfo.RowViewportCount;
            if (IsCornerRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.CornerRangeGroup;
                return hi;
            }
            if (IsRowRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.RowRangeGroup;
                return hi;
            }
            if (IsColumnRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.ColumnRangeGroup;
                return hi;
            }
            if (GetCornerRectangle().Contains(hitPoint))
            {
                HeaderHitTestInformation information2 = new HeaderHitTestInformation
                {
                    Column = -1,
                    Row = -1,
                    ResizingColumn = -1,
                    ResizingRow = -1
                };
                hi.HitTestType = HitTestType.Corner;
                hi.HeaderInfo = information2;
                ColumnLayout rowHeaderColumnLayoutFromX = GetRowHeaderColumnLayoutFromX(hitPoint.X);
                RowLayout columnHeaderRowLayoutFromY = GetColumnHeaderRowLayoutFromY(hitPoint.Y);
                ColumnLayout rowHeaderResizingColumnLayoutFromXForTouch = GetRowHeaderResizingColumnLayoutFromXForTouch(hitPoint.X);
                RowLayout columnHeaderResizingRowLayoutFromYForTouch = GetColumnHeaderResizingRowLayoutFromYForTouch(hitPoint.Y);
                if (rowHeaderColumnLayoutFromX != null)
                {
                    information2.Column = rowHeaderColumnLayoutFromX.Column;
                }
                if (columnHeaderRowLayoutFromY != null)
                {
                    information2.Row = columnHeaderRowLayoutFromY.Row;
                }
                if (rowHeaderResizingColumnLayoutFromXForTouch != null)
                {
                    information2.InColumnResize = true;
                    information2.ResizingColumn = rowHeaderResizingColumnLayoutFromXForTouch.Column;
                }
                if (columnHeaderResizingRowLayoutFromYForTouch != null)
                {
                    information2.InRowResize = true;
                    information2.ResizingRow = columnHeaderResizingRowLayoutFromYForTouch.Row;
                }
                return hi;
            }
            for (int i = -1; i <= rowViewportCount; i++)
            {
                if (GetRowHeaderRectangle(i).Expand(2, 6).Contains(hitPoint))
                {
                    HeaderHitTestInformation information4 = new HeaderHitTestInformation
                    {
                        Row = -1,
                        Column = -1,
                        ResizingColumn = -1,
                        ResizingRow = -1
                    };
                    hi.HitTestType = HitTestType.RowHeader;
                    hi.RowViewportIndex = i;
                    hi.HeaderInfo = information4;
                    ColumnLayout layout5 = GetRowHeaderColumnLayoutFromX(hitPoint.X);
                    RowLayout viewportRowLayoutFromY = GetViewportRowLayoutFromY(i, hitPoint.Y);
                    RowLayout viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(i, hitPoint.Y);
                    if ((viewportResizingRowLayoutFromYForTouch == null) && (hi.RowViewportIndex == 0))
                    {
                        viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(-1, hi.HitPoint.Y);
                    }
                    if ((viewportResizingRowLayoutFromYForTouch == null) && ((hi.RowViewportIndex == 0) || (hi.RowViewportIndex == -1)))
                    {
                        viewportResizingRowLayoutFromYForTouch = GetColumnHeaderResizingRowLayoutFromYForTouch(hi.HitPoint.Y);
                        if (viewportResizingRowLayoutFromYForTouch != null)
                        {
                            hi.HitTestType = HitTestType.Corner;
                        }
                    }
                    if (layout5 != null)
                    {
                        information4.Column = layout5.Column;
                    }
                    if (viewportRowLayoutFromY != null)
                    {
                        information4.Row = viewportRowLayoutFromY.Row;
                    }
                    if ((viewportResizingRowLayoutFromYForTouch != null) && (((viewportResizingRowLayoutFromYForTouch.Height > 0.0) || (viewportResizingRowLayoutFromYForTouch.Row >= Worksheet.RowCount)) || !Worksheet.RowRangeGroup.IsCollapsed(viewportResizingRowLayoutFromYForTouch.Row)))
                    {
                        information4.InRowResize = true;
                        information4.ResizingRow = viewportResizingRowLayoutFromYForTouch.Row;
                    }
                    return hi;
                }
            }
            for (int j = -1; j <= columnViewportCount; j++)
            {
                if (GetColumnHeaderRectangle(j).Expand(3, 1).Contains(hitPoint))
                {
                    HeaderHitTestInformation information6 = new HeaderHitTestInformation
                    {
                        Row = -1,
                        Column = -1,
                        ResizingRow = -1,
                        ResizingColumn = -1
                    };
                    hi.HitTestType = HitTestType.ColumnHeader;
                    hi.HeaderInfo = information6;
                    hi.ColumnViewportIndex = j;
                    ColumnLayout viewportColumnLayoutFromX = GetViewportColumnLayoutFromX(j, hitPoint.X);
                    RowLayout layout9 = GetColumnHeaderRowLayoutFromY(hitPoint.Y);
                    ColumnLayout viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(j, hitPoint.X);
                    if (viewportResizingColumnLayoutFromXForTouch == null)
                    {
                        if (hi.ColumnViewportIndex == 0)
                        {
                            viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(-1, hitPoint.X);
                        }
                        if ((viewportResizingColumnLayoutFromXForTouch == null) && ((hi.ColumnViewportIndex == 0) || (hi.ColumnViewportIndex == -1)))
                        {
                            viewportResizingColumnLayoutFromXForTouch = GetRowHeaderResizingColumnLayoutFromXForTouch(hitPoint.X);
                            if (viewportResizingColumnLayoutFromXForTouch != null)
                            {
                                hi.HitTestType = HitTestType.Corner;
                            }
                        }
                    }
                    if (viewportResizingColumnLayoutFromXForTouch == null)
                    {
                        if (hi.ColumnViewportIndex == 0)
                        {
                            viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromX(-1, hitPoint.X);
                        }
                        if ((viewportResizingColumnLayoutFromXForTouch == null) && ((hi.ColumnViewportIndex == 0) || (hi.ColumnViewportIndex == -1)))
                        {
                            viewportResizingColumnLayoutFromXForTouch = GetRowHeaderResizingColumnLayoutFromXForTouch(hitPoint.X);
                            if (viewportResizingColumnLayoutFromXForTouch != null)
                            {
                                hi.HitTestType = HitTestType.Corner;
                            }
                        }
                    }
                    if (viewportColumnLayoutFromX != null)
                    {
                        hi.HeaderInfo.Column = viewportColumnLayoutFromX.Column;
                    }
                    if (layout9 != null)
                    {
                        hi.HeaderInfo.Row = layout9.Row;
                    }
                    if ((viewportResizingColumnLayoutFromXForTouch != null) && (((viewportResizingColumnLayoutFromXForTouch.Width > 0.0) || (viewportResizingColumnLayoutFromXForTouch.Column >= Worksheet.ColumnCount)) || !Worksheet.ColumnRangeGroup.IsCollapsed(viewportResizingColumnLayoutFromXForTouch.Column)))
                    {
                        hi.HeaderInfo.InColumnResize = true;
                        hi.HeaderInfo.ResizingColumn = viewportResizingColumnLayoutFromXForTouch.Column;
                    }
                    return hi;
                }
            }
            for (int k = -1; k <= rowViewportCount; k++)
            {
                for (int m = -1; m <= columnViewportCount; m++)
                {
                    if (GetViewportRectangle(k, m).Contains(hitPoint))
                    {
                        hi.ColumnViewportIndex = m;
                        hi.RowViewportIndex = k;
                        ViewportHitTestInformation information8 = new ViewportHitTestInformation
                        {
                            Column = -1,
                            Row = -1
                        };
                        hi.HitTestType = HitTestType.Viewport;
                        hi.ViewportInfo = information8;
                        ColumnLayout layout11 = GetViewportColumnLayoutFromX(m, hitPoint.X);
                        RowLayout layout12 = GetViewportRowLayoutFromY(k, hitPoint.Y);
                        if (layout11 != null)
                        {
                            hi.ViewportInfo.Column = layout11.Column;
                        }
                        if (layout12 != null)
                        {
                            hi.ViewportInfo.Row = layout12.Row;
                        }
                        if (!HitTestFloatingObject(k, m, hitPoint.X, hitPoint.Y, hi))
                        {
                            GcViewport viewportRowsPresenter = GetViewportRowsPresenter(k, m);
                            if ((layout11 != null) && (layout12 != null))
                            {
                                if (IsMouseInDragFillIndicator(hitPoint.X, hitPoint.Y, k, m, true))
                                {
                                    hi.ViewportInfo.InDragFillIndicator = true;
                                }
                                else if (IsMouseInDragDropLocation(hitPoint.X, hitPoint.Y, k, m, true))
                                {
                                    hi.ViewportInfo.InSelectionDrag = true;
                                }
                            }
                            if (((IsEditing && !hi.ViewportInfo.InSelectionDrag) && (!hi.ViewportInfo.InDragFillIndicator && (viewportRowsPresenter != null))) && viewportRowsPresenter.EditorBounds.Contains(new Windows.Foundation.Point(x - viewportRowsPresenter.Location.X, y - viewportRowsPresenter.Location.Y)))
                            {
                                hi.ViewportInfo.InEditor = true;
                            }
                            return hi;
                        }
                    }
                }
            }
            return hi;
        }
        #endregion

        #region 内部方法
        bool IsMouseInEditor()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            return ((IsEditing && (savedHitTestInformation.HitTestType == HitTestType.Viewport)) && savedHitTestInformation.ViewportInfo.InEditor);
        }

        bool IsMouseInRangeGroup()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitTestType != HitTestType.RowRangeGroup) && (savedHitTestInformation.HitTestType != HitTestType.ColumnRangeGroup))
            {
                return false;
            }
            return true;
        }

        bool CanTouchManipulate(Windows.Foundation.Point point)
        {
            IsTouchPromotedMouseMessage = false;
            HitTestInformation information = TouchHitTest(point.X, point.Y);

            // 触摸位置不是底部的sheet标签
            if (information.HitTestType != HitTestType.TabStrip)
            {
                if (Worksheet == null)
                {
                    return false;
                }
                if (information.HitTestType == HitTestType.HorizontalScrollBar
                    || information.HitTestType == HitTestType.VerticalScrollBar
                    || information.HitTestType == HitTestType.ColumnRangeGroup
                    || information.HitTestType == HitTestType.RowRangeGroup
                    || information.HitTestType == HitTestType.RowSplitBar
                    || information.HitTestType == HitTestType.ColumnSplitBar
                    || information.HitTestType == HitTestType.ColumnSplitBox
                    || information.HitTestType == HitTestType.RowSplitBox
                    || information.HitTestType == HitTestType.TabSplitBox)
                {
                    // 触摸升级为鼠标消息
                    IsTouchPromotedMouseMessage = true;
                    return false;
                }

                if (information.HitTestType == HitTestType.Viewport)
                {
                    if (IsEditing
                        && Worksheet.ActiveRowIndex == information.ViewportInfo.Row
                        && Worksheet.ActiveColumnIndex == information.ViewportInfo.Column)
                    {
                        return false;
                    }

                    if (_dataValidationListPopUp != null
                        && _dataValidationListPopUp.IsOpen
                        && HitTestPopup(_dataValidationPopUpHelper, point))
                    {
                        return false;
                    }

                    if (_filterPopup != null
                        && _filterPopup.IsOpen
                        && HitTestPopup(_filterPopupHelper, point))
                    {
                        return false;
                    }
                    return true;
                }

                if (information.HitTestType == HitTestType.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        protected Point GetPanelPosition(Point p_pos)
        {
            MatrixTransform mat = this.TransformToVisual(null) as MatrixTransform;
            if (mat != null)
                return new Point(p_pos.X - mat.Matrix.OffsetX, p_pos.Y - mat.Matrix.OffsetY);
            return new Point();
        }

        void ResetTouchWhenError()
        {
            _primaryTouchDeviceId = null;
            _primaryTouchDeviceReleased = false;
            IsTouching = false;
            _isTouchScrolling = false;
            IsTouchZooming = false;
            _touchProcessedPointIds.Clear();
        }

        bool IsZero(double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
        }
        #endregion
    }
}

