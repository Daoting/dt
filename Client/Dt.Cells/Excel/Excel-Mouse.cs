#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        void InitPointer()
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
                    InputDeviceType = InputDeviceType.Touch;
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
                                if ((ActiveSheet != null) && (ActiveSheet.Selections != null))
                                {
                                    CellRange[] rangeArray = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
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
                InputDeviceType = InputDeviceType.Mouse;
                PointerPoint currentPoint = e.GetCurrentPoint(this);
                if (currentPoint.Properties.IsLeftButtonPressed)
                {
                    // 左键
                    Point pos = currentPoint.Position;
                    IsTouching = false;

                    // 无sheet时只底部标签有效
                    if (ActiveSheet == null && HitTest(pos.X, pos.Y).HitTestType != HitTestType.TabStrip)
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

        bool StartMouseClick(PointerRoutedEventArgs e)
        {
            if (IsMouseInScrollBar())
            {
                return false;
            }

            if (IsEditing && (IsMouseInSplitBar() || IsMouseInSplitBox()))
            {
                return false;
            }

            Point position = e.GetCurrentPoint(this).Position;
            if (!GetTabStripRectangle().Contains(position))
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
                FocusInternal();
                _lastClickPoint = e.GetCurrentPoint(this).Position;
                _routedEventArgs = e;
                return true;
            }

            StopCellEditing(true);
            if (_tabStrip != null)
            {
                _tabStrip.StopTabEditing(false);
            }
            _lastClickPoint = position;
            _routedEventArgs = e;
            return true;
        }

        void ProcessMouseLeftButtonDown(HitTestInformation p_hitInfo)
        {
            // 双击行/列头自动宽高
            if (_isDoubleClick
                && (p_hitInfo.HitTestType == HitTestType.ColumnSplitBar || p_hitInfo.HitTestType == HitTestType.RowSplitBar))
                return;

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
            }

            if (!IsEditing || StopCellEditing(false))
            {
                UpdateLastClickLocation(p_hitInfo);
                _hitFilterInfo = GetMouseDownFilterButton(p_hitInfo, false);
                if (_hitFilterInfo != null)
                {
                    ProcessMouseDownFilterButton(_hitFilterInfo);
                }
                else
                {
                    switch (p_hitInfo.HitTestType)
                    {
                        case HitTestType.Corner:
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
                                return;
                            }
                        case HitTestType.TabStrip:
                        case HitTestType.RowRangeGroup:
                        case HitTestType.ColumnRangeGroup:
                            return;

                        case HitTestType.RowHeader:
                            {
                                if (p_hitInfo.HeaderInfo.InRowResize)
                                {
                                    StartRowResizing();
                                    return;
                                }
                                UnSelectedAllFloatingObjects();
                                StartRowsSelecting();
                                return;
                            }
                        case HitTestType.ColumnHeader:
                            {
                                if (p_hitInfo.HeaderInfo.InColumnResize)
                                {
                                    StartColumnResizing();
                                    return;
                                }
                                UnSelectedAllFloatingObjects();
                                StartColumnSelecting();
                                return;
                            }
                        case HitTestType.Viewport:
                            {
                                if (!CanUserDragFill || !p_hitInfo.ViewportInfo.InDragFillIndicator)
                                {
                                    if (CanUserDragDrop && p_hitInfo.ViewportInfo.InSelectionDrag)
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
                                UnSelectedAllFloatingObjects();
                                return;
                            }
                        case HitTestType.CornerRangeGroup:
                            {
                                StartSheetSelecting();
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
                                    flag9 = obj.Locked || ActiveSheet.Protect;
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
                    }
                }
            }
        }

        void ProcessMouseDownFilterButton(FilterButtonInfo filterBtnInfo)
        {
            if (!RaiseFilterPopupOpening(filterBtnInfo.Row, filterBtnInfo.Column) && (filterBtnInfo != null))
            {
                _filterPopupHelper = new PopupHelper(FilterPopup);
                ColumnDropDownList dropdown = new ColumnDropDownList();
                AddSortItems(dropdown, filterBtnInfo);
                dropdown.Items.Add(new SeparatorDropDownItemControl());
                AutoFilterDropDownItemControl control = CreateAutoFilter(filterBtnInfo);
                dropdown.Items.Add(control);
                dropdown.Popup = _filterPopupHelper;
                int row = filterBtnInfo.Row;
                int column = filterBtnInfo.Column;
                CellRange range = ActiveSheet.GetSpanCell(row, column, filterBtnInfo.SheetArea);
                if (range != null)
                {
                    row = (range.Row + range.RowCount) - 1;
                    column = (range.Column + range.ColumnCount) - 1;
                }
                RowLayout columnHeaderRowLayout = null;
                ColumnLayout layout2 = GetViewportColumnLayoutModel(filterBtnInfo.ColumnViewportIndex).Find(column);
                if (filterBtnInfo.SheetArea == SheetArea.ColumnHeader)
                {
                    columnHeaderRowLayout = GetColumnHeaderRowLayout(row);
                }
                else if (filterBtnInfo.SheetArea == SheetArea.Cells)
                {
                    columnHeaderRowLayout = GetViewportRowLayoutModel(filterBtnInfo.RowViewportIndex).Find(row);
                }
                if ((columnHeaderRowLayout != null) && (layout2 != null))
                {
                    _filterPopupHelper.ShowAsModal(this, dropdown, new Point(layout2.X + layout2.Width, columnHeaderRowLayout.Y + columnHeaderRowLayout.Height));
                }
            }
        }

        async void AddSortItems(ColumnDropDownList dropdown, FilterButtonInfo info)
        {
            DropDownItemControl control = new DropDownItemControl();
            control.Content = ResourceStrings.SortDropdownItemSortAscend;
            control.Icon = await SR.GetImage("SortAscending.png");
            control.Command = new SortCommand(this, info, true);
            dropdown.Items.Add(control);
            DropDownItemControl control2 = new DropDownItemControl();
            control2.Content = ResourceStrings.SortDropdownItemSortDescend;
            control2.Icon = await SR.GetImage("SortDescending.png");
            control2.Command = new SortCommand(this, info, false);
            dropdown.Items.Add(control2);
        }
        #endregion

        #region 鼠标移动
        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // 构造ScrollBar时设置
            //if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            //{
            //    UpdateScrollBarIndicatorMode(ScrollingIndicatorMode.TouchIndicator);
            //}

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
            else if (ActiveSheet != null)
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

        void ProcessMouseMove(HitTestInformation p_hitInfo)
        {
            bool flag = false;
            switch (p_hitInfo.HitTestType)
            {
                case HitTestType.RowSplitBar:
                    if ((!IsWorking && !IsEditing) && !Workbook.Protect)
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
                    if ((IsWorking || IsEditing) || Workbook.Protect)
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
                    if ((!IsWorking && !IsEditing) && !Workbook.Protect)
                    {
                        if (InputDeviceType != InputDeviceType.Touch)
                        {
                            SetBuiltInCursor((CoreCursorType)8);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.ColumnSplitBox:
                    if ((!IsWorking && !IsEditing) && !Workbook.Protect)
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

            if (flag)
            {
                if (!IsWorking)
                {
                    _hoverManager.DoHover(p_hitInfo);
                }
            }
            else
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
                            if (ActiveSheet.GetActualRowHeight(p_hitInfo.HeaderInfo.ResizingRow, SheetArea.Cells) != 0.0)
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
                            if (ActiveSheet.GetActualColumnWidth(p_hitInfo.HeaderInfo.ResizingColumn, SheetArea.Cells) != 0.0)
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

            // 构造ScrollBar时设置
            //UpdateScrollBarIndicatorMode(ScrollingIndicatorMode.MouseIndicator);
        }
        #endregion

        #region 鼠标释放
        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
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

            if (_tabStrip != null && TabStripEditable)
            {
                Point point = e.GetCurrentPoint(this).Position;
                if ((HitTest(point.X, point.Y).HitTestType == HitTestType.TabStrip) && _tabStrip.StayInEditing(point))
                {
                    e.Handled = true;
                    return;
                }
            }

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
            else if (ActiveSheet != null)
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

        bool EndMouseClick(PointerRoutedEventArgs e)
        {
            if (GetHitInfo().HitTestType == HitTestType.TabStrip)
                return false;

            IsMouseLeftButtonPressed = false;
            ReleasePointerCapture(e.Pointer);
            return true;
        }

        void ProcessMouseLeftButtonUp(HitTestInformation p_hitInfo)
        {
            ClearMouseLeftButtonDownStates();
            if (IsColumnSplitting || IsRowSplitting)
            {
                if (!IsEditing)
                {
                    FocusInternal();
                }
            }
            else
            {
                if (!IsEditing)
                {
                    FocusInternal();
                }
                if (!IsWorking)
                {
                    if ((CanUserDragDrop && (p_hitInfo.HitTestType == HitTestType.Viewport)) && p_hitInfo.ViewportInfo.InSelectionDrag)
                    {
                        SetBuiltInCursor(CoreCursorType.Hand);
                    }
                    if (((_lastClickPoint == MousePosition) && (p_hitInfo.HitTestType == HitTestType.FloatingObject)) && (p_hitInfo.FloatingObjectInfo.FloatingObject != null))
                    {
                        UnSelectFloatingObject(p_hitInfo.FloatingObjectInfo.FloatingObject);
                    }
                }
            }
        }

        void RaiseCellClick(MouseButtonType p_btnType, HitTestInformation p_hitInfo)
        {
            if (CellClick == null || _eventSuspended != 0)
                return;

            CellClickEventArgs args = null;
            Point point2 = new Point(-1.0, -1.0);
            if (p_hitInfo.HitTestType == HitTestType.Viewport)
            {
                args = CreateCellClickEventArgs(p_hitInfo.ViewportInfo.Row, p_hitInfo.ViewportInfo.Column, ActiveSheet.SpanModel, SheetArea.Cells, p_btnType);
                point2 = new Point((double)p_hitInfo.ViewportInfo.Row, (double)p_hitInfo.ViewportInfo.Column);
            }
            else if (p_hitInfo.HitTestType == HitTestType.RowHeader)
            {
                args = CreateCellClickEventArgs(p_hitInfo.HeaderInfo.Row, p_hitInfo.HeaderInfo.Column, ActiveSheet.RowHeaderSpanModel, SheetArea.CornerHeader | SheetArea.RowHeader, p_btnType);
                point2 = new Point((double)p_hitInfo.HeaderInfo.Row, (double)p_hitInfo.HeaderInfo.Column);
            }
            else if (p_hitInfo.HitTestType == HitTestType.ColumnHeader)
            {
                args = CreateCellClickEventArgs(p_hitInfo.HeaderInfo.Row, p_hitInfo.HeaderInfo.Column, ActiveSheet.ColumnHeaderSpanModel, SheetArea.ColumnHeader, p_btnType);
                point2 = new Point((double)p_hitInfo.HeaderInfo.Row, (double)p_hitInfo.HeaderInfo.Column);
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
                if (ActiveSheet != null)
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

        void ResetTouchStates(IList<PointerPoint> ps)
        {
            if (IsTouchColumnSplitting)
            {
                EndColumnSplitting();
            }
            if (IsTouchRowSplitting)
            {
                EndRowSplitting();
            }

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
            if ((ActiveSheet == null) || (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse))
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
                    ZoomUndoAction command = new ZoomUndoAction(ActiveSheet, newZoomFactor);
                    DoCommand(command);
                    PrepareCellEditing();
                    RaiseUserZooming(zoomFactor, newZoomFactor);
                }
            }
            else if ((VerticalScrollable && (ActiveSheet != null)) && ((activeRowViewportIndex >= 0) && (activeRowViewportIndex < rowViewportCount)))
            {
                int viewportTopRow = GetViewportTopRow(activeRowViewportIndex);
                int num8 = num2 + viewportTopRow;
                num8 = Math.Max(TryGetNextScrollableRow(ActiveSheet.FrozenRowCount), num8);
                num8 = Math.Min(TryGetPreviousScrollableRow((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1), num8);
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
            if (ActiveSheet == null)
                return;

            HitTestInformation hitInfo = GetHitInfo();
            if (hitInfo.HitTestType != HitTestType.Viewport || IsMouseLeftButtonPressed)
            {
                HitTestInformation information2 = GetHitInfo();
                _isDoubleClick = true;
                IsMouseLeftButtonPressed = false;
                ReleasePointerCaptures();
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

        void ProcessMouseLeftButtonDoubleClick(DoubleTappedRoutedEventArgs e)
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
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
                            if (((_tabStrip == null) || !TabStripEditable) || (Workbook.Protect || (_routedEventArgs == null)))
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
                            RaiseSheetTabDoubleClick(sheetTabIndex);
                            return;
                        }
                    default:
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
                        break;
                }
            }
        }

        void RaiseCellDoubleClick(Point point)
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

        void OnManipulationStarted()
        {
            UpdateTouchHitInfo(_touchStartPoint);
            _touchStartHitTestInfo = GetHitInfo();
            _touchZoomNewFactor = ActiveSheet.ZoomFactor;
            if ((_touchStartHitTestInfo != null) && (_touchStartHitTestInfo.HitTestType != HitTestType.Empty))
            {
                InitTouchCacheInfomation();
            }

            InputDeviceType = InputDeviceType.Touch;
            IsTouchingMovingFloatingObjects = false;
            IsTouchingResizingFloatingObjects = false;
            _translateOffsetX = 0.0;
            _translateOffsetY = 0.0;

            if (((_dragFillSmartTag != null) && (_dragFillPopup != null)) && !HitTestPopup(_dragFillPopup, _touchStartPoint))
            {
                CloseDragFillPopup();
            }

            if ((_touchStartHitTestInfo.HitTestType == HitTestType.Viewport) && (ActiveSheet.Selections != null))
            {
                IsTouchSelectingCells = false;
                if ((!IsEditing && !IsTouchDrapDropping) && !IsTouchDragFilling)
                {
                    if (_autoFillIndicatorRect.HasValue && _autoFillIndicatorRect.Value.Contains(_touchStartPoint))
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
            if (_touchStartHitTestInfo.HitTestType == HitTestType.RowHeader && !_autoFillIndicatorRect.HasValue)
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
            if (_touchStartHitTestInfo.HitTestType == HitTestType.ColumnHeader
                && GetMouseDownFilterButton(_touchStartHitTestInfo, false) == null
                && !_autoFillIndicatorRect.HasValue)
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
            else if ((viewportTopRow >= GetMaxBottomScrollableRow()) && ((_translateOffsetY + ActiveSheet.Rows[GetMaxBottomScrollableRow()].ActualHeight) < 0.0))
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
                    if ((viewportLeftColumn >= GetMaxRightScrollableColumn()) && ((_translateOffsetX + ActiveSheet.Columns[GetMaxRightScrollableColumn()].ActualWidth) < 0.0))
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

        void ProcessGestrueRecognizerManipulationUpdated(ManipulationUpdatedEventArgs e)
        {
            Point curPos = GetPanelPosition(e.Position);
            if (IsZero((double)(e.Cumulative.Scale - 1f)) || (_touchProcessedPointIds.Count == 1))
            {
                ProcessTouchFreeDrag(curPos, new Point(-e.Delta.Translation.X, -e.Delta.Translation.Y));
            }
            else if ((!IsZero((double)(e.Cumulative.Scale - 1f)) && !IsTouchZooming) && CanUserZoom)
            {
                IsContinueTouchOperation = true;
                _touchZoomInitFactor = ActiveSheet.ZoomFactor;
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

        protected virtual void ProcessTouchFreeDrag(Point p_curPos, Point p_offset)
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
                UpdateTouchHitInfo(p_curPos);
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

                SheetLayout layout = GetSheetLayout();
                if (_translateOffsetX < 0.0)
                {
                    if ((_touchStartHitTestInfo.ColumnViewportIndex == -1) || (_touchStartHitTestInfo.ColumnViewportIndex == layout.ColumnPaneCount))
                    {
                        _translateOffsetX = 0.0;
                    }
                    else
                    {
                        layout.SetViewportWidth(_touchStartHitTestInfo.ColumnViewportIndex, _cachedViewportWidths[_touchStartHitTestInfo.ColumnViewportIndex + 1] + Math.Abs(_translateOffsetX));
                        InvalidateViewportColumnsLayout();
                    }
                }
                else if ((_touchStartHitTestInfo.ColumnViewportIndex == -1) || (_touchStartHitTestInfo.ColumnViewportIndex == layout.ColumnPaneCount))
                {
                    _translateOffsetX = 0.0;
                }
                else
                {
                    layout.SetViewportWidth(_touchStartHitTestInfo.ColumnViewportIndex, _cachedViewportWidths[_touchStartHitTestInfo.ColumnViewportIndex + 1]);
                    InvalidateViewportColumnsLayout();
                }

                if (_translateOffsetY < 0.0)
                {
                    if ((_touchStartHitTestInfo.RowViewportIndex == -1) || (_touchStartHitTestInfo.RowViewportIndex == layout.RowPaneCount))
                    {
                        _translateOffsetY = 0.0;
                    }
                    else
                    {
                        layout.SetViewportHeight(_touchStartHitTestInfo.RowViewportIndex, _cachedViewportHeights[_touchStartHitTestInfo.RowViewportIndex + 1] + Math.Abs(_translateOffsetY));
                        InvalidateViewportRowsLayout();
                    }
                }
                else if ((_touchStartHitTestInfo.RowViewportIndex == -1) || (_touchStartHitTestInfo.RowViewportIndex == layout.RowPaneCount))
                {
                    _translateOffsetY = 0.0;
                }
                else
                {
                    layout.SetViewportHeight(_touchStartHitTestInfo.RowViewportIndex, _cachedViewportHeights[_touchStartHitTestInfo.RowViewportIndex + 1]);
                    InvalidateViewportColumnsLayout();
                }
                InvalidateMeasure();
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
                    _tabStrip.TabsPresenter.InvalidateMeasure();
                    _tabStrip.TabsPresenter.InvalidateArrange();
                }
            }
            else
            {
                HideMouseCursor();
                if (IsTouchDragFilling)
                {
                    ContinueTouchDragFill();
                }
                else if (IsTouchDrapDropping)
                {
                    ContinueTouchDragDropping();
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

        void OnManipulationComplete()
        {
            IsContinueTouchOperation = false;
            CachedGripperLocation = null;
            ClearViewportsClip();
            UpdateViewport();

            if (IsTouchTabStripScrolling)
            {
                IsTouchTabStripScrolling = false;
                _tabStrip.TabsPresenter.Offset = 0.0;
                _tabStrip.TabsPresenter.InvalidateMeasure();
                _tabStrip.TabsPresenter.InvalidateArrange();
            }

            if (IsTouchZooming)
            {
                IsTouchZooming = false;
                if ((_zoomOriginHitTestInfo != null) && (_zoomOriginHitTestInfo.HitTestType == HitTestType.Viewport))
                {
                    TransformGroup group = _cachedViewportTransform[_zoomOriginHitTestInfo.RowViewportIndex + 1, _zoomOriginHitTestInfo.ColumnViewportIndex + 1];
                    if (group != null)
                    {
                        SheetLayout layout = GetSheetLayout();
                        Point newLocation = new Point(layout.HeaderWidth, layout.HeaderHeight);
                        Point reference = group.TransformPoint(newLocation);
                        int viewportLeftColumn = GetViewportLeftColumn(_zoomOriginHitTestInfo.ColumnViewportIndex);
                        Point point3 = reference.Delta(newLocation);
                        int num2 = viewportLeftColumn;
                        double x = point3.X;
                        if (x > 0.0)
                        {
                            while ((x > 0.0) && (num2 < ActiveSheet.ColumnCount))
                            {
                                x -= Math.Floor((double)(ActiveSheet.Columns[num2].ActualWidth * _touchZoomNewFactor));
                                num2++;
                            }
                        }
                        else if (x < 0.0)
                        {
                            while (((x < 0.0) && (num2 > 0)) && (num2 < ActiveSheet.ColumnCount))
                            {
                                x += Math.Floor((double)(ActiveSheet.Columns[num2].ActualWidth * _touchZoomNewFactor));
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
                            while ((y > 0.0) && (num5 < ActiveSheet.RowCount))
                            {
                                y -= Math.Floor((double)(ActiveSheet.Rows[num5].ActualHeight * _touchZoomNewFactor));
                                num5++;
                            }
                        }
                        else if (y < 0.0)
                        {
                            while (((y < 0.0) && (num5 > 0)) && (num5 < ActiveSheet.RowCount))
                            {
                                y += Math.Floor((double)(ActiveSheet.Rows[num5].ActualHeight * _touchZoomNewFactor));
                                num5--;
                            }
                        }
                        if (num5 != viewportTopRow)
                        {
                            SetViewportTopRow(_zoomOriginHitTestInfo.RowViewportIndex, num5);
                        }
                    }
                }

                _cachedViewportTransform = null;
                _cachedRowHeaderViewportTransform = null;
                _cachedColumnHeaderViewportTransform = null;
                _cachedCornerViewportTransform = null;
                float zoomFactor = ActiveSheet.ZoomFactor;
                ActiveSheet.ZoomFactor = (float)_touchZoomNewFactor;
                RaiseUserZooming(zoomFactor, ActiveSheet.ZoomFactor);
                InvalidateViewportColumnsLayout();
                InvalidateViewportRowsLayout();
                RefreshFloatingObjects();
                InvalidateMeasure();
                if (((_touchStartTopRow >= 0) && (_touchStartTopRow < ActiveSheet.RowCount)) && (_touchStartLeftColumn >= 0))
                {
                    int columnCount = ActiveSheet.ColumnCount;
                    int num14 = _touchStartLeftColumn;
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

            _fastScroll = false;
            GetViewportInfo();
            if (_cellsPanels != null)
            {
                CellsPanel[,] viewportArray = _cellsPanels;
                int upperBound = viewportArray.GetUpperBound(0);
                int num9 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num9; j++)
                    {
                        CellsPanel viewport = viewportArray[i, j];
                        if (viewport != null)
                        {
                            viewport.InvalidateBordersMeasureState();
                        }
                    }
                }
            }

            IsTouching = false;
            IsTouchPromotedMouseMessage = false;

            if (IsTouchSelectingCells)
            {
                EndTouchSelectingCells();
                if (InputDeviceType == InputDeviceType.Touch)
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
                    CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(_touchStartHitTestInfo.RowViewportIndex, _touchStartHitTestInfo.ColumnViewportIndex);
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
                    CellsPanel viewport2 = GetViewportRowsPresenter(_touchStartHitTestInfo.RowViewportIndex, _touchStartHitTestInfo.ColumnViewportIndex);
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
            else if ((viewportTopRow >= GetMaxBottomScrollableRow()) && ((_translateOffsetY + ActiveSheet.Rows[GetMaxBottomScrollableRow()].ActualHeight) < 0.0))
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
                else if ((viewportLeftColumn >= GetMaxRightScrollableColumn()) && ((_translateOffsetX + ActiveSheet.Columns[GetMaxRightScrollableColumn()].ActualWidth) < 0.0))
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
            UpdateTouchHitInfo(args.Position);
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

        void ProcessTap(HitTestInformation p_hitInfo)
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

            if ((GetActiveSelection() == null) && (ActiveSheet.Selections.Count > 0))
            {
                CellRange range = ActiveSheet.Selections[0];
            }
            if (IsEditing && !StopCellEditing(false))
            {
                return;
            }

            CloseAutoFilterIndicator();
            _hitFilterInfo = GetMouseDownFilterButton(p_hitInfo, true);
            if (_hitFilterInfo != null)
            {
                ProcessMouseDownFilterButton(_hitFilterInfo);
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
                    }
                    else if (!TapInSelectionRow(p_hitInfo.HeaderInfo.Row))
                    {
                        if (p_hitInfo.HeaderInfo.Row > -1)
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
                        }
                        else if (TapInSelectionColumn(p_hitInfo.HeaderInfo.Column))
                        {
                            TapInColumnHeaderSelection(p_hitInfo.HitPoint, p_hitInfo);
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
                            bool flag9 = obj.Locked || ActiveSheet.Protect;
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
                    var allSelectedFloatingObjects = GetAllSelectedFloatingObjects();
                    if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Count > 0))
                    {
                        UnSelectedAllFloatingObjects();
                        StartTapSelectCells();
                        EndTouchSelectingCells();
                        RaiseTouchCellClick(p_hitInfo);
                        ArrangeSelectionGripper();
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
                            ArrangeSelectionGripper();
                        }
                        else
                        {
                            RaiseTouchToolbarOpeningEvent(p_hitInfo.HitPoint, cells);
                        }
                    }
                }
                else
                {
                    StartTapSelectCells();
                    EndTouchSelectingCells();
                    RaiseTouchCellClick(p_hitInfo);
                }
            }
            UnSelectedAllFloatingObjects();
        Label_0568:
            FocusInternal();
        }

        Point TapInColumnHeaderSelection(Point point, HitTestInformation hi)
        {
            UnSelectedAllFloatingObjects();
            StartColumnSelecting();
            EndColumnSelecting();
            RaiseTouchCellClick(hi);
            return point;
        }

        Point TapInRowHeaderSelection(Point point, HitTestInformation hi)
        {
            UnSelectedAllFloatingObjects();
            StartRowsSelecting();
            EndRowSelecting();
            RaiseTouchCellClick(hi);
            return point;
        }

        bool TapInSelection(Point point)
        {
            return GetActiveSelectionBounds().Contains(point);
        }

        bool TapInSelectionColumn(int column)
        {
            ReadOnlyCollection<CellRange> selections = ActiveSheet.Selections;
            if (selections != null)
            {
                foreach (CellRange range in selections)
                {
                    if (((range.Row == -1) && (range.RowCount == -1)) && range.IntersectColumn(column))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool TapInSelectionRow(int row)
        {
            ReadOnlyCollection<CellRange> selections = ActiveSheet.Selections;
            if (selections != null)
            {
                foreach (CellRange range in selections)
                {
                    if (((range.Column == -1) && (range.ColumnCount == -1)) && range.IntersectRow(row))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void ProcessDoubleTap(HitTestInformation p_hitInfo)
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
                    if (((p_hitInfo.HitTestType == HitTestType.TabStrip) && (_tabStrip != null)) && (TabStripEditable && !Workbook.Protect))
                    {
                        _tabStrip.StartTabTouchEditing(p_hitInfo.HitPoint);
                        if (_tabStrip.IsEditing)
                        {
                            int sheetTabIndex = (_tabStrip.ActiveTab != null) ? _tabStrip.ActiveTab.SheetIndex : -1;
                            RaiseSheetTabDoubleClick(sheetTabIndex);
                        }
                    }
                    else
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
                }
            }
        }
        #endregion

        #region HitTest
        HitTestInformation GetHitInfo()
        {
            if (_positionInfo == null)
            {
                _positionInfo = new HitTestInformation();
            }
            return _positionInfo;
        }

        void SaveHitInfo(HitTestInformation hitTestInfo)
        {
            _positionInfo = hitTestInfo;
        }

        void UpdateTouchHitInfo(Point point)
        {
            HitTestInformation hitInfo = GetHitInfo();
            if (point != hitInfo.HitPoint)
            {
                hitInfo = TouchHitTest(point.X, point.Y);
                SaveHitInfo(hitInfo);
            }
            _lastClickPoint = point;
        }

        internal HitTestInformation TouchHitTest(double x, double y)
        {
            Point hitPoint = new Point(x, y);
            SheetLayout layout = GetSheetLayout();
            HitTestInformation hi = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = hitPoint
            };
            bool flag = (RowSplitBoxAlignment == SplitBoxAlignment.Trailing) && (ColumnSplitBoxAlignment == SplitBoxAlignment.Trailing);
            for (int i = 0; i < layout.ColumnPaneCount; i++)
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
                if (horizontalSplitBoxRectangle.Contains(hitPoint))
                {
                    hi.HitTestType = HitTestType.ColumnSplitBox;
                    hi.ColumnViewportIndex = i;
                    return hi;
                }
            }
            for (int j = 0; j < layout.RowPaneCount; j++)
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
                if (verticalSplitBoxRectangle.Expand(15, 30).Contains(hitPoint))
                {
                    hi.HitTestType = HitTestType.RowSplitBox;
                    hi.RowViewportIndex = j;
                    return hi;
                }
            }
            for (int k = 0; k < (layout.ColumnPaneCount - 1); k++)
            {
                if (GetHorizontalSplitBarRectangle(k).Expand(10, 10).Contains(hitPoint) && (hi.HitTestType != HitTestType.ColumnSplitBox))
                {
                    hi.HitTestType = HitTestType.ColumnSplitBar;
                    hi.ColumnViewportIndex = k;
                }
            }
            for (int m = 0; m < (layout.RowPaneCount - 1); m++)
            {
                if (GetVerticalSplitBarRectangle(m).Expand(10, 10).Contains(hitPoint) && (hi.HitTestType != HitTestType.RowSplitBox))
                {
                    hi.HitTestType = HitTestType.RowSplitBar;
                    hi.RowViewportIndex = m;
                }
            }

            if (hi.HitTestType == HitTestType.Empty)
            {
                if (GetTabStripRectangle().Contains(hitPoint))
                {
                    hi.ColumnViewportIndex = 0;
                    hi.HitTestType = HitTestType.TabStrip;
                    return hi;
                }
                for (int n = 0; n < layout.ColumnPaneCount; n++)
                {
                    if (GetHorizontalScrollBarRectangle(n).Contains(hitPoint))
                    {
                        hi.ColumnViewportIndex = n;
                        hi.HitTestType = HitTestType.HorizontalScrollBar;
                        return hi;
                    }
                }
                for (int num6 = 0; num6 < layout.RowPaneCount; num6++)
                {
                    if (GetVerticalScrollBarRectangle(num6).Contains(hitPoint))
                    {
                        hi.HitTestType = HitTestType.VerticalScrollBar;
                        hi.RowViewportIndex = num6;
                        return hi;
                    }
                }
            }

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
                    if ((viewportResizingRowLayoutFromYForTouch != null) && (((viewportResizingRowLayoutFromYForTouch.Height > 0.0) || (viewportResizingRowLayoutFromYForTouch.Row >= ActiveSheet.RowCount)) || !ActiveSheet.RowRangeGroup.IsCollapsed(viewportResizingRowLayoutFromYForTouch.Row)))
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
                    if ((viewportResizingColumnLayoutFromXForTouch != null) && (((viewportResizingColumnLayoutFromXForTouch.Width > 0.0) || (viewportResizingColumnLayoutFromXForTouch.Column >= ActiveSheet.ColumnCount)) || !ActiveSheet.ColumnRangeGroup.IsCollapsed(viewportResizingColumnLayoutFromXForTouch.Column)))
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
                            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(k, m);
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
                            if (((IsEditing && !hi.ViewportInfo.InSelectionDrag) && (!hi.ViewportInfo.InDragFillIndicator && (viewportRowsPresenter != null))) && viewportRowsPresenter.EditorBounds.Contains(new Point(x - viewportRowsPresenter.Location.X, y - viewportRowsPresenter.Location.Y)))
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

        RowLayout GetColumnHeaderResizingRowLayoutFromYForTouch(double y)
        {
            RowLayout columnHeaderResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(y);
            if (columnHeaderResizingRowLayoutFromY == null)
            {
                for (int i = -5; i < 5; i++)
                {
                    columnHeaderResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(y);
                    if (columnHeaderResizingRowLayoutFromY != null)
                    {
                        return columnHeaderResizingRowLayoutFromY;
                    }
                }
            }
            return columnHeaderResizingRowLayoutFromY;
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

        void ClearMouseLeftButtonDownStates()
        {
            if (IsColumnSplitting)
            {
                EndColumnSplitting();
            }
            if (IsRowSplitting)
            {
                EndRowSplitting();
            }

            if (IsResizingColumns)
            {
                EndColumnResizing();
            }
            if (IsResizingRows)
            {
                EndRowResizing();
            }
            if (IsSelectingCells)
            {
                EndCellSelecting();
            }
            if (IsSelectingColumns)
            {
                EndColumnSelecting();
                HitTestInformation savedHitTestInformation = GetHitInfo();
                if ((savedHitTestInformation != null) && (savedHitTestInformation.HitTestType == HitTestType.ColumnHeader))
                {
                    var columnHeaderRowsPresenter = GetColumnHeaderRowsPresenter(savedHitTestInformation.ColumnViewportIndex);
                    if (columnHeaderRowsPresenter != null)
                    {
                        var row = columnHeaderRowsPresenter.GetRow(savedHitTestInformation.HeaderInfo.Row);
                        if (row != null)
                        {
                            var cell = row.GetCell(savedHitTestInformation.HeaderInfo.Column);
                            if (cell != null)
                            {
                                cell.ApplyState();
                            }
                        }
                    }
                }
            }
            if (IsSelectingRows)
            {
                EndRowSelecting();
                HitTestInformation information2 = GetHitInfo();
                if ((information2 != null) && (information2.HitTestType == HitTestType.RowHeader))
                {
                    var rowHeaderRowsPresenter = GetRowHeaderRowsPresenter(information2.RowViewportIndex);
                    if (rowHeaderRowsPresenter != null)
                    {
                        var presenter2 = rowHeaderRowsPresenter.GetRow(information2.HeaderInfo.Row);
                        if (presenter2 != null)
                        {
                            var base3 = presenter2.GetCell(information2.HeaderInfo.Column);
                            if (base3 != null)
                            {
                                base3.ApplyState();
                            }
                        }
                    }
                }
            }
            if (IsDragDropping)
            {
                EndDragDropping();
            }
            if (IsDraggingFill)
            {
                EndDragFill();
            }
            if (IsMovingFloatingOjects)
            {
                EndFloatingObjectsMoving();
            }
            if (IsResizingFloatingObjects)
            {
                EndFloatingObjectResizing();
            }
        }

        bool CanTouchManipulate(Point point)
        {
            IsTouchPromotedMouseMessage = false;
            HitTestInformation information = TouchHitTest(point.X, point.Y);

            // 触摸位置不是底部的sheet标签
            if (information.HitTestType != HitTestType.TabStrip)
            {
                if (ActiveSheet == null)
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
                    || information.HitTestType == HitTestType.RowSplitBox)
                {
                    // 触摸升级为鼠标消息
                    IsTouchPromotedMouseMessage = true;
                    return false;
                }

                if (information.HitTestType == HitTestType.Viewport)
                {
                    if (IsEditing
                        && ActiveSheet.ActiveRowIndex == information.ViewportInfo.Row
                        && ActiveSheet.ActiveColumnIndex == information.ViewportInfo.Column)
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
            IsTouching = false;
            _isTouchScrolling = false;
            IsTouchZooming = false;
            _touchProcessedPointIds.Clear();
        }
        #endregion

        #region 实现触摸滚动
        //****************************************************
        // 触摸时滚动单元格区通过改变 _translateOffsetX _translateOffsetY 的值在 ArrangeOverride 中实现
        // TabStrip 通过改变 Offset 值实现标签滚动
        //****************************************************

        void TouchScrollLeft(Point currentPoint, Point deltaPoint)
        {
            int maxRightScrollableColumn = GetMaxRightScrollableColumn();
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel(_touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells);
            double num2 = Math.Abs(deltaPoint.X);
            int viewportLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn > maxRightScrollableColumn)
            {
                return;
            }
            double num4 = 0.0;
            if (columnLayoutModel.FindColumn(viewportLeftColumn) != null)
            {
                num4 = columnLayoutModel.FindColumn(viewportLeftColumn).Width + _translateOffsetX;
            }
            if (num4 > num2)
            {
                _translateOffsetX += -1.0 * num2;
                return;
            }
            num2 -= num4;
            int num5 = viewportLeftColumn + 1;
            while (true)
            {
                if ((num5 > maxRightScrollableColumn) || ActiveSheet.Columns[num5].ActualVisible)
                {
                    break;
                }
                num5++;
            }
            if (num5 > maxRightScrollableColumn)
            {
                _translateOffsetX += -1.0 * Math.Abs(deltaPoint.X);
                return;
            }
            SetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex, num5);
            viewportLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn >= maxRightScrollableColumn)
            {
                return;
            }
            int num6 = viewportLeftColumn;
        Label_010B:
            if (num6 >= ActiveSheet.ColumnCount)
            {
                return;
            }
            if (num6 >= 0)
            {
                Column column = ActiveSheet.Columns[num6];
                if (column.ActualVisible)
                {
                    double num7 = column.Width * ZoomFactor;
                    if (num7 > num2)
                    {
                        _translateOffsetX = -1.0 * num2;
                        return;
                    }
                    num2 -= num7;
                }
                num6++;
                if (num6 <= maxRightScrollableColumn)
                {
                    if (column.ActualVisible)
                    {
                        SetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex, num6);
                    }
                    goto Label_010B;
                }
            }
        }

        void TouchScrollRight(Point currentPoint, Point deltaPoint)
        {
            int maxLeftScrollableColumn = GetMaxLeftScrollableColumn();
            int maxRightScrollableColumn = GetMaxRightScrollableColumn();
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel(_touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.X);
            int viewportLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn <= maxLeftScrollableColumn)
            {
                Point point = currentPoint.Delta(_touchStartPoint);
                _translateOffsetX = -1.0 * point.X;
                _translateOffsetX = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(_translateOffsetX), 120.0) * Math.Sign(_translateOffsetX);
                return;
            }
            if ((viewportLeftColumn >= maxRightScrollableColumn) && (currentPoint.X < _touchStartPoint.X))
            {
                return;
            }
            double num5 = 0.0;
            if (columnLayoutModel.FindColumn(viewportLeftColumn) != null)
            {
                if ((_translateOffsetX + Math.Abs(deltaPoint.X)) < 0.0)
                {
                    num5 = (columnLayoutModel.FindColumn(viewportLeftColumn).Width + _translateOffsetX) + Math.Abs(deltaPoint.X);
                }
                else
                {
                    num5 = 0.0;
                }
            }
            if (num5 >= num3)
            {
                _translateOffsetX += num3;
                return;
            }
            num3 -= num5;
            if ((num3 + _translateOffsetX) >= 0.0)
            {
                num3 += _translateOffsetX;
                _translateOffsetX = 0.0;
            }
            else
            {
                return;
            }
            int num6 = viewportLeftColumn - 1;
            while (true)
            {
                if ((num6 < maxLeftScrollableColumn) || ActiveSheet.Columns[num6].ActualVisible)
                {
                    break;
                }
                num6--;
            }
            if (num6 < maxLeftScrollableColumn)
            {
                Point point2 = currentPoint.Delta(_touchStartPoint);
                _translateOffsetX = -1.0 * point2.X;
                _translateOffsetX = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(_translateOffsetX), 120.0) * Math.Sign(_translateOffsetX);
                return;
            }
            SetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex, num6);
            viewportLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn <= maxLeftScrollableColumn)
            {
                return;
            }
            int num7 = viewportLeftColumn;
        Label_0201:
            if (num7 >= ActiveSheet.ColumnCount)
            {
                return;
            }
            if (num7 >= 0)
            {
                Column column = ActiveSheet.Columns[num7];
                if (column.ActualVisible)
                {
                    double num8 = column.Width * ZoomFactor;
                    if (num8 >= num3)
                    {
                        _translateOffsetX = num3 - num8;
                        GetSheetLayout();
                        return;
                    }
                    num3 -= num8;
                }
                num7--;
                if (num7 < maxLeftScrollableColumn)
                {
                    _translateOffsetX = num3;
                }
                else
                {
                    if (column.ActualVisible)
                    {
                        SetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex, num7);
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
                if ((num5 > maxBottomScrollableRow) || ActiveSheet.Rows[num5].ActualVisible)
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

            SetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex, num5);
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
            if (num6 < ActiveSheet.RowCount)
            {
                Row row = ActiveSheet.Rows[num6];
                if (row.ActualVisible)
                {
                    double num7 = row.ActualHeight * ZoomFactor;
                    if (num7 > num2)
                    {
                        _translateOffsetY = -1.0 * num2;
                        return;
                    }
                    num2 -= num7;
                }
                num6++;
                if (num6 < maxBottomScrollableRow)
                {
                    if (row.ActualVisible)
                    {
                        SetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex, num6);
                    }
                    goto Label_0112;
                }
            }
        }

        void TouchScrollBottom(Point currentPoint, Point deltaPoint)
        {
            int maxTopScrollableRow = GetMaxTopScrollableRow();
            int maxBottomScrollableRow = GetMaxBottomScrollableRow();
            RowLayoutModel rowLayoutModel = GetRowLayoutModel(_touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.Y);
            int viewportTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= maxTopScrollableRow)
            {
                Point point = currentPoint.Delta(_touchStartPoint);
                _translateOffsetY = -1.0 * point.Y;
                _translateOffsetY = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(_translateOffsetY), 80.0) * Math.Sign(_translateOffsetY);
                return;
            }
            if ((viewportTopRow >= maxBottomScrollableRow) && (currentPoint.Y < _touchStartPoint.Y))
            {
                return;
            }
            double num5 = 0.0;
            if (rowLayoutModel.FindRow(viewportTopRow) != null)
            {
                if ((_translateOffsetY + Math.Abs(deltaPoint.Y)) < 0.0)
                {
                    num5 = (rowLayoutModel.FindRow(viewportTopRow).Height + _translateOffsetY) + Math.Abs(deltaPoint.Y);
                }
                else
                {
                    num5 = 0.0;
                }
            }
            if (num5 >= num3)
            {
                _translateOffsetY += num3;
                return;
            }
            num3 -= num5;
            if ((num3 + _translateOffsetY) >= 0.0)
            {
                num3 += _translateOffsetY;
                _translateOffsetY = 0.0;
            }
            else
            {
                return;
            }
            int num6 = viewportTopRow - 1;
            while (true)
            {
                if ((num6 < maxTopScrollableRow) || ActiveSheet.Rows[num6].ActualVisible)
                {
                    break;
                }
                num6--;
            }
            if (num6 < maxTopScrollableRow)
            {
                Point point2 = currentPoint.Delta(_touchStartPoint);
                _translateOffsetY = -1.0 * point2.Y;
                _translateOffsetY = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(_translateOffsetY), 80.0) * Math.Sign(_translateOffsetY);
                return;
            }
            SetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex, num6);
            viewportTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= maxTopScrollableRow)
            {
                return;
            }
            int num7 = viewportTopRow;
        Label_0201:
            if (!ActiveSheet.Rows[num7].ActualVisible)
            {
                num7--;
            }
            if (((num7 >= maxTopScrollableRow) && (num7 >= 0)) && (num7 < ActiveSheet.RowCount))
            {
                Row row = ActiveSheet.Rows[num7];
                if (row.ActualVisible)
                {
                    double num8 = row.Height * ZoomFactor;
                    if (num8 >= num3)
                    {
                        _translateOffsetY = num3 - num8;
                        GetSheetLayout();
                        return;
                    }
                    num3 -= num8;
                }
                num7--;
                if (num7 >= maxTopScrollableRow)
                {
                    if (row.ActualVisible)
                    {
                        SetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex, num7);
                    }
                    goto Label_0201;
                }
            }
        }

        void TouchTabStripScrollLeft(Point currentPoint, Point deltaPoint)
        {
            TabsPresenter tabsPresenter = _tabStrip.TabsPresenter;
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
            StartSheetIndex = num5;
        Label_00F0:
            if (num5 >= tabsPresenter.LastScrollableSheetIndex)
            {
                return;
            }
            if (num5 >= 0)
            {
                double num6 = tabsPresenter.FirstSheetTabWidth * ZoomFactor;
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
                        StartSheetIndex = num5;
                        goto Label_00F0;
                    }
                }
            }
        }

        void TouchTabStripScrollRight(Point currentPoint, Point deltaPoint)
        {
            int num7;
            double num9;
            TabsPresenter tabsPresenter = _tabStrip.TabsPresenter;
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
                    StartSheetIndex = num7;
                    goto Label_0154;
                }
            }
            return;
        Label_0154:
            num9 = tabsPresenter.FirstSheetTabWidth * ZoomFactor;
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
                    StartSheetIndex = num7;
                    goto Label_0154;
                }
            }
        }
        #endregion
    }
}

