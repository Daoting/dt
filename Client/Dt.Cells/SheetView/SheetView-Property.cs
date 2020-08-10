#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.Cells.Data;
using Dt.Cells.UndoRedo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    public partial class SheetView
    {
        /// <summary>
        /// Gets the Excel control that is associated with the view. 
        /// </summary>
        public Excel Excel { get; }

        /// <summary>
        /// Gets the worksheet associated with the view.
        /// </summary>
        public Worksheet ActiveSheet
        {
            get { return Excel.ActiveSheet; }
        }

        /// <summary>
        /// Gets or sets whether the component handles the shortcut keys for Clipboard actions. 
        /// </summary>
        public bool AutoClipboard
        {
            get { return (bool)GetValue(AutoClipboardProperty); }
            set { SetValue(AutoClipboardProperty, value); }
        }

        /// <summary>
        /// Gets a value that indicates whether the user is editing a formula.
        /// </summary>
        public bool CanSelectFormula
        {
            get { return (_formulaSelectionFeature.IsSelectionBegined && _formulaSelectionFeature.CanSelectFormula); }
        }

        /// <summary>
        /// Indicates whether the user can select multiple ranges by touch.
        /// </summary>
        public bool CanTouchMultiSelect
        {
            get { return (bool)GetValue(CanTouchMultiSelectProperty); }
            set { SetValue(CanTouchMultiSelectProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow users to drag and drop a range.
        /// </summary>
        public bool CanUserDragDrop
        {
            get { return (bool)GetValue(CanUserDragDropProperty); }
            set { SetValue(CanUserDragDropProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow users to drag and fill a range.
        /// </summary>
        public bool CanUserDragFill
        {
            get { return (bool)GetValue(CanUserDragFillProperty); }
            set { SetValue(CanUserDragFillProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to enter formulas in a cell in the component.
        /// </summary>
        public bool CanUserEditFormula
        {
            get { return (bool)GetValue(CanUserEditFormulaProperty); }
            set { SetValue(CanUserEditFormulaProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to undo edit operations.
        /// </summary>
        public bool CanUserUndo
        {
            get { return (bool)GetValue(CanUserUndoProperty); }
            set { SetValue(CanUserUndoProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the user can scale the display of the component using the Ctrl key and the mouse wheel. 
        /// </summary>
        [DefaultValue(true)]
        public bool CanUserZoom
        {
            get { return (bool)GetValue(CanUserZoomProperty); }
            set { SetValue(CanUserZoomProperty, value); }
        }

        /// <summary>
        /// Gets the cell editor control on the editing viewport.
        /// </summary>
        Control CellEditor
        {
            get
            {
                if (((EditingViewport != null) && (EditingViewport.EditingContainer != null)) && ((EditingViewport.EditingContainer.EditingRowIndex == ActiveSheet.ActiveRowIndex) && (EditingViewport.EditingContainer.EditingColumnIndex == ActiveSheet.ActiveColumnIndex)))
                {
                    return (EditingViewport.EditingContainer.Editor as Control);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets whether the component handles the shortcut keys for Clipboard actions. 
        /// </summary>
        public ClipboardPasteOptions ClipBoardOptions
        {
            get { return (ClipboardPasteOptions)GetValue(ClipBoardOptionsProperty); }
            set { SetValue(ClipBoardOptionsProperty, value); }
        }

        Windows.UI.Xaml.Controls.Primitives.Popup DataValidationListPopUp
        {
            get
            {
                if (_dataValidationListPopUp == null)
                {
                    _dataValidationListPopUp = new Windows.UI.Xaml.Controls.Primitives.Popup();
                    _dataValidationListPopUp.Opened += _dataValidationListPopUp_Opened;
                    _dataValidationListPopUp.Closed += _dataValidationListPopUp_Closed;
                }
                return _dataValidationListPopUp;
            }
        }

        /// <summary>
        /// Gets or sets the default type of the automatic fill.
        /// </summary>
        /// <value>
        /// The default type of the automatic fill.
        /// </value>
        public AutoFillType? DefaultAutoFillType { get; set; }

        int DragFillStartBottomRow
        {
            get
            {
                if (_dragFillStartRange == null)
                {
                    return -1;
                }
                if (_dragFillStartRange.Row == -1)
                {
                    return (ActiveSheet.RowCount - 1);
                }
                return ((_dragFillStartRange.Row + _dragFillStartRange.RowCount) - 1);
            }
        }

        RowLayout DragFillStartBottomRowLayout
        {
            get
            {
                int dragFillStartBottomRow = DragFillStartBottomRow;
                if (dragFillStartBottomRow != -1)
                {
                    return GetViewportRowLayoutModel(_dragFillStartBottomRowViewport).FindRow(dragFillStartBottomRow);
                }
                return null;
            }
        }

        int DragFillStartLeftColumn
        {
            get
            {
                if (_dragFillStartRange == null)
                {
                    return -1;
                }
                if (_dragFillStartRange.Column == -1)
                {
                    return 0;
                }
                return _dragFillStartRange.Column;
            }
        }

        int DragFillStartRightColumn
        {
            get
            {
                if (_dragFillStartRange == null)
                {
                    return -1;
                }
                if (_dragFillStartRange.Column == -1)
                {
                    return (ActiveSheet.ColumnCount - 1);
                }
                return ((_dragFillStartRange.Column + _dragFillStartRange.ColumnCount) - 1);
            }
        }

        ColumnLayout DragFillStartRightColumnLayout
        {
            get
            {
                int dragFillStartRightColumn = DragFillStartRightColumn;
                if (dragFillStartRightColumn != -1)
                {
                    return GetViewportColumnLayoutModel(_dragFillStartRightColumnViewport).FindColumn(dragFillStartRightColumn);
                }
                return null;
            }
        }

        int DragFillStartTopRow
        {
            get
            {
                if (_dragFillStartRange == null)
                {
                    return -1;
                }
                if (_dragFillStartRange.Row == -1)
                {
                    return 0;
                }
                return _dragFillStartRange.Row;
            }
        }

        int DragFillStartViewportBottomRow
        {
            get { return GetViewportBottomRow(_dragStartRowViewport); }
        }

        RowLayout DragFillStartViewportBottomRowLayout
        {
            get { return GetViewportRowLayoutModel(_dragStartRowViewport).FindRow(DragFillStartViewportBottomRow); }
        }

        int DragFillStartViewportLeftColumn
        {
            get { return GetViewportLeftColumn(_dragStartColumnViewport); }
        }

        ColumnLayout DragFillStartViewportLeftColumnLayout
        {
            get { return GetViewportColumnLayoutModel(_dragStartColumnViewport).FindColumn(DragFillStartViewportLeftColumn); }
        }

        int DragFillStartViewportRightColumn
        {
            get { return GetViewportRightColumn(_dragStartColumnViewport); }
        }

        ColumnLayout DragFillStartViewportRightColumnLayout
        {
            get { return GetViewportColumnLayoutModel(_dragStartColumnViewport).FindColumn(DragFillStartViewportRightColumn); }
        }

        int DragFillStartViewportTopRow
        {
            get { return GetViewportTopRow(_dragStartRowViewport); }
        }

        RowLayout DragFillStartViewportTopRowLayout
        {
            get { return GetViewportRowLayoutModel(_dragStartRowViewport).FindRow(DragFillStartViewportTopRow); }
        }

        int DragFillToViewportBottomRow
        {
            get { return GetViewportBottomRow(_dragToRowViewport); }
        }

        RowLayout DragFillToViewportBottomRowLayout
        {
            get { return GetViewportRowLayoutModel(_dragToRowViewport).FindRow(DragFillToViewportBottomRow); }
        }

        int DragFillToViewportLeftColumn
        {
            get { return GetViewportLeftColumn(_dragToColumnViewport); }
        }

        int DragFillToViewportRightColumn
        {
            get { return GetViewportRightColumn(_dragToColumnViewport); }
        }

        ColumnLayout DragFillToViewportRightColumnLayout
        {
            get { return GetViewportColumnLayoutModel(_dragToColumnViewport).FindColumn(DragFillToViewportRightColumn); }
        }

        int DragFillToViewportTopRow
        {
            get { return GetViewportTopRow(_dragToRowViewport); }
        }

        internal CellsPanel EditingViewport
        {
            get { return _editinViewport; }
            set { _editinViewport = value; }
        }

        internal FormulaEditorConnector EditorConnector
        {
            get { return _formulaSelectionFeature.FormulaEditorConnector; }
        }

        internal bool EditorDirty
        {
            get { return ((_editinViewport != null) && _editinViewport.EditorDirty); }
        }

        /// <summary>
        /// Gets the information of the editor when the sheetview enters the formula selection mode.
        /// </summary>
        public EditorInfo EditorInfo
        {
            get
            {
                if (_editorInfo == null)
                {
                    _editorInfo = new EditorInfo(this);
                }
                return _editorInfo;
            }
        }

        Windows.UI.Xaml.Controls.Primitives.Popup FilterPopup
        {
            get
            {
                if (_filterPopup == null)
                {
                    _filterPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
                    _filterPopup.Opened += FilterPopup_Opened;
                    _filterPopup.Closed += FilterPopup_Closed;
                }
                return _filterPopup;
            }
        }

        internal SpreadXFormulaNavigation FormulaNavigation
        {
            get { return _formulaSelectionFeature.Navigation; }
        }

        internal SpreadXFormulaSelection FormulaSelection
        {
            get { return _formulaSelectionFeature.Selection; }
        }

        internal IList<FormulaSelectionItem> FormulaSelections
        {
            get { return _formulaSelectionFeature.Items; }
        }

        /// <summary>
        /// Gets or sets a value that indicates the freeze line style.
        /// </summary>
        internal Style FreezeLineStyle
        {
            get { return (Style)GetValue(FreezeLineStyleProperty); }
            set { SetValue(FreezeLineStyleProperty, value); }
        }

        internal bool HideSelectionWhenPrinting
        {
            get { return (bool)GetValue(HideSelectionWhenPrintingProperty); }
            set { SetValue(HideSelectionWhenPrintingProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to highlight invalid data.
        /// </summary>
        public bool HighlightInvalidData
        {
            get { return (bool)GetValue(HighlightInvalidDataProperty); }
            set { SetValue(HighlightInvalidDataProperty, value); }
        }





        // 以下未整理

        /// <summary>
        /// Gets or sets a value that indicates whether the horizontal scroll bar is scrollable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the horizontal scroll bar is scrollable; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool HorizontalScrollable
        {
            get { return _hScrollable; }
            set { _hScrollable = value; }
        }

        internal HoverManager HoverManager
        {
            get { return _hoverManager; }
        }

        /// <summary>
        /// Returns the last input device type.
        /// </summary>
        [DefaultValue(0)]
        public InputDeviceType InputDeviceType
        {
            get { return _inputDeviceType; }
            internal set
            {
                _inputDeviceType = value;
                if (_inputDeviceType == InputDeviceType.Touch)
                {
                    FormulaSelectionFeature.IsTouching = true;
                }
                else if (_inputDeviceType == InputDeviceType.Mouse)
                {
                    FormulaSelectionFeature.IsTouching = false;
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether there is a cell in edit mode.
        /// </summary>
        public bool IsCellEditing
        {
            get { return IsEditing; }
        }

        bool IsDecreaseFill
        {
            get
            {
                if (_currentFillDirection != DragFillDirection.Left)
                {
                    return (_currentFillDirection == DragFillDirection.Up);
                }
                return true;
            }
        }

        bool IsDragClear
        {
            get
            {
                if (_currentFillDirection != DragFillDirection.LeftClear)
                {
                    return (_currentFillDirection == DragFillDirection.UpClear);
                }
                return true;
            }
        }

        bool IsDragDropping { get; set; }

        bool IsDragFill
        {
            get
            {
                if (!IsIncreaseFill)
                {
                    return IsDecreaseFill;
                }
                return true;
            }
        }

        bool IsDragFillStartBottomRowInView
        {
            get { return IsRowInViewport(_dragFillStartBottomRowViewport, DragFillStartBottomRow); }
        }

        bool IsDragFillStartRightColumnInView
        {
            get { return IsColumnInViewport(_dragFillStartRightColumnViewport, DragFillStartRightColumn); }
        }

        bool IsDragFillWholeColumns
        {
            get { return ((_dragFillStartRange.Row == -1) && (_dragFillStartRange.Column != -1)); }
        }

        bool IsDragFillWholeRows
        {
            get { return ((_dragFillStartRange.Column == -1) && (_dragFillStartRange.Row != -1)); }
        }

        internal bool IsDraggingFill { get; set; }

        bool IsDragToColumnInView
        {
            get { return IsColumnInViewport(_dragToColumnViewport, _dragToColumn); }
        }

        bool IsDragToRowInView
        {
            get { return IsRowInViewport(_dragToRowViewport, _dragToRow); }
        }

        internal bool IsEditing
        {
            get
            {
                if ((_tabStrip != null) && _tabStrip.IsEditing)
                    return false;
                return _isEditing; 
            }
            set { _isEditing = value; }
        }

        internal bool IsFilterDropDownOpen
        {
            get { return ((_filterPopup != null) && _filterPopup.IsOpen); }
        }

        bool IsIncreaseFill
        {
            get
            {
                if (_currentFillDirection != DragFillDirection.Down)
                {
                    return (_currentFillDirection == DragFillDirection.Right);
                }
                return true;
            }
        }

        internal bool IsMouseLeftButtonPressed { get; set; }

        internal bool IsMouseRightButtonPressed { get; set; }

        bool IsMovingFloatingOjects { get; set; }

        bool IsResizingColumns { get; set; }

        bool IsResizingFloatingObjects { get; set; }

        bool IsResizingRows { get; set; }

        bool IsSelectingCells { get; set; }

        bool IsSelectingColumns { get; set; }

        bool IsSelectingRows { get; set; }

        internal bool IsSelectionBegined
        {
            get { return _formulaSelectionFeature.IsSelectionBegined; }
        }

        internal bool IsTouchingMovingFloatingObjects { get; set; }

        internal bool IsTouchingResizingFloatingObjects { get; set; }

        bool IsVerticalDragFill
        {
            get
            {
                if ((_currentFillDirection != DragFillDirection.Up) && (_currentFillDirection != DragFillDirection.Down))
                {
                    return (_currentFillDirection == DragFillDirection.UpClear);
                }
                return true;
            }
        }

        /// <summary>
        /// 是否开始执行某个操作，如调整行高列宽、拖拽、浮动窗口移动...
        /// </summary>
        internal bool IsWorking { get; set; }

        internal int MaxCellOverflowDistance
        {
            get { return 100; }
            set
            {
            }
        }

        internal int MouseOverColumnIndex { get; set; }

        internal int MouseOverRowIndex { get; set; }

        internal Point MousePosition { get; set; }

        internal SpreadXNavigation Navigation
        {
            get
            {
                if (_navigation == null)
                {
                    _navigation = new SpreadXNavigation(this);
                    if (ActiveSheet != null)
                    {
                        _navigation.UpdateStartPosition(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
                    }
                }
                return _navigation;
            }
        }

        /// <summary>
        /// Gets or sets the backgroud of the range group
        /// </summary>
        public Brush RangeGroupBackground
        {
            get { return _rangeGroupBackground; }
            set
            {
                _rangeGroupBackground = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the brush of the border of the range group
        /// </summary>
        public Brush RangeGroupBorderBrush
        {
            get { return _rangeGroupBorderBrush; }
            set
            {
                _rangeGroupBorderBrush = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the stroke of the group line
        /// </summary>
        public Brush RangeGroupLineStroke
        {
            get { return _rangeGroupLineStroke; }
            set
            {
                _rangeGroupLineStroke = value;
                Invalidate();
            }
        }

        internal Rect? ResizerGripperRect { get; set; }

        /// <summary>
        /// Specifies the drawing policy when the row or column is resized to zero.
        /// </summary>
        [DefaultValue(0)]
        public ResizeZeroIndicator ResizeZeroIndicator
        {
            get { return _resizeZeroIndicator; }
            set { _resizeZeroIndicator = value; }
        }

        CellRange[] SavedOldSelections { get; set; }

        internal SpreadXSelection Selection
        {
            get
            {
                if (_selection == null)
                {
                    _selection = new SpreadXSelection(this);
                }
                return _selection;
            }
        }

        internal Canvas ShapeDrawingContainer
        {
            get
            {
                if (_shapeDrawingContainer == null)
                {
                    _shapeDrawingContainer = new Canvas();
                }
                return _shapeDrawingContainer;
            }
        }

        /// <summary>
        /// Gets or sets whether the column range group is visible.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowColumnRangeGroup
        {
            get { return _showColumnRangeGroup; }
            set
            {
                if (value != _showColumnRangeGroup)
                {
                    _showColumnRangeGroup = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show the freeze line.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowFreezeLine
        {
            get { return _showFreezeLine; }
            set
            {
                _showFreezeLine = value;
                UpdateFreezeLines();
            }
        }

        /// <summary>
        /// Gets or sets whether the row range group is visible.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowRowRangeGroup
        {
            get { return _showRowRangeGroup; }
            set
            {
                if (value != _showRowRangeGroup)
                {
                    _showRowRangeGroup = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        TooltipPopupHelper TooltipHelper
        {
            get
            {
                if (_tooltipHelper == null)
                {
                    _tooltipHelper = new TooltipPopupHelper(this, -1.0);
                }
                return _tooltipHelper;
            }
            set { _tooltipHelper = value; }
        }

        internal Windows.UI.Xaml.Controls.Primitives.Popup ToolTipPopup
        {
            get
            {
                if (_tooltipPopup == null)
                {
                    _tooltipPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
                    _tooltipPopup.IsHitTestVisible = false;
                    base.Children.Add(_tooltipPopup);
                }
                return _tooltipPopup;
            }
        }

        internal Canvas TrackersContainer
        {
            get
            {
                if (_trackersContainer == null)
                {
                    _trackersContainer = new Canvas();
                    Canvas.SetZIndex(_trackersContainer, 2);
                }
                return _trackersContainer;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the trailing freeze line style.
        /// </summary>
        [DefaultValue((string)null)]
        internal Style TrailingFreezeLineStyle
        {
            get { return _trailingFreezeLineStyle; }
            set
            {
                _trailingFreezeLineStyle = value;
                _columnTrailingFreezeLine.TypeSafeSetStyle(value);
                _rowTrailingFreezeLine.TypeSafeSetStyle(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the undo manager for the control.
        /// </summary>
#if IOS
        new
#endif
        public UndoManager UndoManager
        {
            get
            {
                if (_undoManager == null)
                {
                    _undoManager = new UndoManager(this, -1, CanUserUndo);
                }
                return _undoManager;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the vertical scroll bar is scrollable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the vertical scroll bar is scrollable; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool VerticalScrollable
        {
            get { return _vScrollable; }
            set { _vScrollable = value; }
        }

        /// <summary>
        /// Gets or sets the scaling factor for displaying this sheet.
        /// </summary>
        /// <value>The scaling factor for displaying this sheet.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Specified scaling amount is out of range; must be between 0.5 (50%) and 4.0 (400%).
        /// </exception>
        [DefaultValue((float)1f)]
        public float ZoomFactor
        {
            get
            {
                if (ActiveSheet != null)
                {
                    return ActiveSheet.ZoomFactor;
                }
                return 1f;
            }
            set
            {
                if (ActiveSheet != null)
                {
                    ActiveSheet.ZoomFactor = value;
                    InvalidateRange(-1, -1, -1, -1, SheetArea.Cells);
                    InvalidateRange(-1, -1, -1, -1, SheetArea.ColumnHeader);
                    InvalidateRange(-1, -1, -1, -1, SheetArea.CornerHeader | SheetArea.RowHeader);
                    base.InvalidateMeasure();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        double ActualHorizontalScrollBarHeight
        {
            get
            {
                if (Excel.HorizontalScrollBarHeight >= 0.0)
                {
                    return Excel.HorizontalScrollBarHeight;
                }
                return 25.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        double ActualVerticalScrollBarWidth
        {
            get
            {
                if (Excel.VerticalScrollBarWidth >= 0.0)
                {
                    return Excel.VerticalScrollBarWidth;
                }
                return 25.0;
            }
        }

        /// <summary>
        /// Gets or sets the column split box alignment. 
        /// </summary>
        /// <value>
        /// The column split box alignment. 
        /// </value>
        [DefaultValue(0)]
        public SplitBoxAlignment ColumnSplitBoxAlignment
        {
            get { return _columnSplitBoxAlignment; }
            set
            {
                _columnSplitBoxAlignment = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets what conditions under which the GcSpreadSheet component permits column splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy ColumnSplitBoxPolicy
        {
            get { return _columnSplitBoxPolicy; }
            set
            {
                if (value != _columnSplitBoxPolicy)
                {
                    _columnSplitBoxPolicy = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the horizontal scroll bar. 
        /// </summary>
        /// <value>
        /// The height of the horizontal scroll bar. 
        /// </value>
        [DefaultValue((double)25.0)]
        public double HorizontalScrollBarHeight
        {
            get { return _horizontalScrollBarHeight; }
            set
            {
                _horizontalScrollBarHeight = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ScrollBarVisibility HorizontalScrollBarPolicy
        {
            get { return Excel.HorizontalScrollBarVisibility; }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll bar style. 
        /// </summary>
        /// <value>
        /// The horizontal scroll bar style. 
        /// </value>
        [DefaultValue((string)null)]
        public Style HorizontalScrollBarStyle
        {
            get { return _horizontalScrollBarStyle; }
            set
            {
                _horizontalScrollBarStyle = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsColumnSplitting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsRowSplitting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsTabStripResizing { get; set; }

        /// <summary>
        /// Gets or sets the row split box alignment. 
        /// </summary>
        /// <value>
        /// The row split box alignment. 
        /// </value>
        [DefaultValue(0)]
        public SplitBoxAlignment RowSplitBoxAlignment
        {
            get { return _rowSplitBoxAlignment; }
            set
            {
                _rowSplitBoxAlignment = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets what conditions under which the GcSpreadSheet component permits row splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy RowSplitBoxPolicy
        {
            get { return _rowSplitBoxPolicy; }
            set
            {
                if (value != _rowSplitBoxPolicy)
                {
                    _rowSplitBoxPolicy = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the sheet in the control scrolls when the user moves the scroll box. 
        /// </summary>
        ///<value>
        ///The scroll bar track policy. 
        ///</value>
        [DefaultValue(3)]
        public ScrollBarTrackPolicy ScrollBarTrackPolicy
        {
            get { return _scrollBarTrackPolicy; }
            set
            {
                _scrollBarTrackPolicy = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        Canvas SplittingTrackerContainer
        {
            get
            {
                if (_splittingTrackerContainer == null)
                {
                    _splittingTrackerContainer = new Canvas();
                    Canvas.SetZIndex(_splittingTrackerContainer, 0x63);
                }
                return _splittingTrackerContainer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TabStrip TabStrip
        {
            get { return _tabStrip; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the tab strip is editable.
        /// </summary>
        /// <value>
        /// true if the tab strip can be edited; otherwise, false. 
        /// </value>
        [DefaultValue(true)]
        public bool TabStripEditable
        {
            get { return _tabStripEditable; }
            set
            {
                _tabStripEditable = value;
                if ((!value && (_tabStrip != null)) && _tabStrip.IsEditing)
                {
                    _tabStrip.StopTabEditing(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether a special tab is displayed to allow the user to insert new sheets. 
        /// </summary>
        [DefaultValue(true)]
        public bool TabStripInsertTab
        {
            get { return _tabStripInsertTab; }
            set
            {
                if (value != _tabStripInsertTab)
                {
                    _tabStripInsertTab = value;
                    if ((_tabStrip != null) && (value != _tabStrip.HasInsertTab))
                    {
                        _tabStrip.HasInsertTab = value;
                        InvalidateLayout();
                        base.InvalidateMeasure();
                    }
                }
            }
        }

        /// <summary>
        ///Gets or sets the width of the tab strip for this component expressed as a percentage of the overall horizontal scroll bar width.  
        /// </summary>
        [DefaultValue((double)0.5)]
        public double TabStripRatio
        {
            get { return _tabStripRatio; }
            set
            {
                if (((value >= 0.0) && (value <= 1.0)) && (value != _tabStripRatio))
                {
                    _tabStripRatio = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        ///Gets or sets the display policy for the sheet tab strip for this component.  
        /// </summary>
        public Visibility TabStripVisibility
        {
            get { return _tabStripVisibility; }
            set
            {
                if (value != _tabStripVisibility)
                {
                    _tabStripVisibility = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ScrollBarVisibility VerticalScrollBarPolicy
        {
            get { return Excel.VerticalScrollBarVisibility; }
        }

        /// <summary>
        ///Gets or sets the vertical scroll bar style. 
        /// </summary>
        /// <value>
        /// The vertical scroll bar style. 
        /// </value>
        [DefaultValue((string)null)]
        public Style VerticalScrollBarStyle
        {
            get { return _verticalScrollBarStyle; }
            set
            {
                _verticalScrollBarStyle = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        ///Gets or sets the width of the vertical scroll bar.  
        /// </summary>
        /// <value>
        /// The width of the vertical scroll bar. 
        /// </value>
        [DefaultValue((double)25.0)]
        public double VerticalScrollBarWidth
        {
            get { return _verticalScrollBarWidth; }
            set
            {
                _verticalScrollBarWidth = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        #region 成员变量
        Rect? _autoFillIndicatorRec;
        GripperLocationsStruct _gripperLocations;

        internal Border _autoFillIndicatorContainer;
        Size _availableSize;
        internal Ellipse _bottomRightGripper;
        internal Image _cachedautoFillIndicatorImage;
        internal Image _cachedBottomRightACornerVisual;
        CellLayoutModel[] _cachedColumnHeaderCellLayoutModel;
        internal RowLayoutModel _cachedColumnHeaderRowLayoutModel;
        internal ColumnLayoutModel[] _cachedColumnHeaderViewportColumnLayoutModel;
        internal TransformGroup[] _cachedColumnHeaderViewportTransform;
        internal Image[] _cachedColumnHeaderViewportVisual;
        internal Image _cachedColumnResizerGripperImage;
        internal TransformGroup _cachedCornerViewportTransform;
        internal Image _cachedCornerViewportVisual;
        FilterButtonInfoModel _cachedFilterButtonInfoModel;
        FloatingObjectLayoutModel[,] _cachedFloatingObjectLayoutModel;
        FloatingObjectLayoutModel[,] _cachedFloatingObjectMovingResizingLayoutModel;
        GroupLayout _cachedGroupLayout;
        SheetLayout _cachedLayout;
        internal Dictionary<string, BitmapImage> _cachedResizerGipper;
        CellLayoutModel[] _cachedRowHeaderCellLayoutModel;
        internal ColumnLayoutModel _cachedRowHeaderColumnLayoutModel;
        internal RowLayoutModel[] _cachedRowHeaderViewportRowLayoutModel;
        internal TransformGroup[] _cachedRowHeaderViewportTransform;
        internal Image[] _cachedRowHeaderViewportVisual;
        internal Image _cachedRowResizerGripperImage;
        internal Dictionary<string, ImageSource> _cachedToolbarImageSources;
        CellLayoutModel[,] _cachedViewportCellLayoutModel;
        internal ColumnLayoutModel[] _cachedViewportColumnLayoutModel;
        internal double[] _cachedViewportHeights;
        internal RowLayoutModel[] _cachedViewportRowLayoutModel;
        internal double[] _cachedViewportSplitBarX;
        internal double[] _cachedViewportSplitBarY;
        internal TransformGroup[,] _cachedViewportTransform;
        internal Image[,] _cachedViewportVisual;
        internal double[] _cachedViewportWidths;
        internal Line _columnFreezeLine;
        GcRangeGroupHeader _columnGroupHeaderPresenter;
        GcRangeGroup[] _columnGroupPresenters;
        ColHeaderPanel[] _columnHeaderPresenters;
        int _columnOffset;
        internal Line _columnTrailingFreezeLine;
        CornerPanel _cornerPanel;
        internal int _currentActiveColumnIndex;
        internal int _currentActiveRowIndex;
        DragFillDirection _currentFillDirection = DragFillDirection.Down;
        CellRange _currentFillRange;
        Windows.UI.Xaml.Controls.Primitives.Popup _dataValidationListPopUp;
        PopupHelper _dataValidationPopUpHelper;
        int _dragDropColumnOffset;
        CellRange _dragDropFromRange;
        Grid _dragDropIndicator;
        Grid _dragDropInsertIndicator;
        int _dragDropRowOffset;
        PopupHelper _dragFillPopup;
        DragFillSmartTag _dragFillSmartTag;
        int _dragFillStartBottomRowViewport = -2;
        int _dragFillStartLeftColumnViewport = -2;
        CellRange _dragFillStartRange;
        int _dragFillStartRightColumnViewport = -2;
        int _dragFillStartTopRowViewport = -2;
        int _dragStartColumnViewport;
        int _dragStartRowViewport;
        int _dragToColumn;
        int _dragToColumnViewport;
        int _dragToRow;
        int _dragToRowViewport;
        CellsPanel _editinViewport;
        EditorInfo _editorInfo;
        internal short _eventSuspended;
        Windows.UI.Xaml.Controls.Primitives.Popup _filterPopup;
        PopupHelper _filterPopupHelper;

        Point _floatingObjectsMovingResizingOffset = new Point(0.0, 0.0);
        int _floatingObjectsMovingResizingStartColumn = -2;
        Point _floatingObjectsMovingResizingStartPoint = new Point(0.0, 0.0);
        Rect _floatingObjectsMovingResizingStartPointCellBounds = new Rect(0.0, 0.0, 0.0, 0.0);
        int _floatingObjectsMovingResizingStartRow = -2;
        Dictionary<string, Point> _floatingObjectsMovingStartLocations = new Dictionary<string, Point>();
        FormulaSelectionFeature _formulaSelectionFeature;
        internal FormulaSelectionGripperContainerPanel _formulaSelectionGripperPanel;
        internal GestureRecognizer _gestrueRecognizer;
        GcRangeGroupCorner _groupCornerPresenter;
        FilterButtonInfo _hitFilterInfo;
        ScrollSelectionManager _horizontalSelectionMgr;
        internal HoverManager _hoverManager;
        bool _hScrollable;
        internal InputDeviceType _inputDeviceType;
        internal bool _isDoubleClick;
        bool _isDragCopy;
        bool _isDragInsert;
        bool _isEditing;
        internal bool _isIMEEnterEditing;
        bool _isMouseDownFloatingObject;
        internal bool _isTouchScrolling;

        internal Point _lastClickLocation;
        internal Point _lastClickPoint;
        Image _mouseCursor;
        internal Point _mouseDownPosition;
        FloatingObject[] _movingResizingFloatingObjects;
        SpreadXNavigation _navigation;
        internal HitTestInformation _positionInfo;
        CopyMoveCellsInfo _preFillCellsInfo;
        internal uint? _primaryTouchDeviceId = null;
        internal bool _primaryTouchDeviceReleased;
        bool _protect;
        Brush _rangeGroupBackground;
        Brush _rangeGroupBorderBrush;
        Brush _rangeGroupLineStroke;
        internal bool _resetSelectionFrameStroke;
        internal Border _resizerGripperContainer;
        internal ResizeZeroIndicator _resizeZeroIndicator;
        internal Line _resizingTracker;
        internal PointerRoutedEventArgs _routedEventArgs;
        internal Line _rowFreezeLine;
        GcRangeGroupHeader _rowGroupHeaderPresenter;
        GcRangeGroup[] _rowGroupPresenters;
        internal CellsPanel[] _rowHeaderPresenters;
        int _rowOffset;
        internal Line _rowTrailingFreezeLine;
        SpreadXSelection _selection;
        Canvas _shapeDrawingContainer;
        bool _showColumnRangeGroup;
        bool _showFreezeLine;
        bool _showRowRangeGroup;
        int _suspendViewInvalidate;
        TooltipPopupHelper _tooltipHelper;
        Windows.UI.Xaml.Controls.Primitives.Popup _tooltipPopup;
        internal Ellipse _topLeftGripper;
        internal HashSet<uint> _touchProcessedPointIds = new HashSet<uint>();
        internal HitTestInformation _touchStartHitTestInfo;
        internal int _touchStartLeftColumn = -1;
        internal Point _touchStartPoint;
        internal int _touchStartTopRow = -1;
        internal Windows.UI.Xaml.Controls.Primitives.Popup _touchToolbarPopup = null;
        //internal Point _touchTranslatePoint;
        internal double _touchZoomInitFactor;
        internal double _touchZoomNewFactor;
        internal Point _touchZoomOrigin;
        Canvas _trackersContainer;
        Style _trailingFreezeLineStyle;
        internal double _translateOffsetX;
        internal double _translateOffsetY;
        UndoManager _undoManager;
        internal bool _updateViewportAfterTouch;
        ScrollSelectionManager _verticalSelectionMgr;
        internal CellsPanel[,] _viewportPresenters;
        bool _vScrollable;
        internal HitTestInformation _zoomOriginHitTestInfo;
        internal GripperLocationsStruct CachedGripperLocation;
        const string _CELL_DELIMITER = "\"";
        const string _COLUMN_DELIMITER = "\t";
        internal bool _DoTouchResizing;
        internal const double ENHANCED_ZERO_INDICATOR_WIDTH = 6.0;
        internal bool _fastScroll;
        internal const double FILTERBUTTON_HEIGHT = 16.0;
        internal const double FILTERBUTTON_WIDTH = 16.0;
        static readonly Size GCSPREAD_DefaultSize = new Size(500.0, 500.0);
        internal const int GRIPPERSIZE = 0x10;
        const double _GROUPBUTTON_HEIGHT = 16.0;
        const double _GROUPBUTTON_WIDTH = 16.0;
        internal const double HALF_ENHANCED_ZERO_INDICATOR_WIDTH = 3.0;
        internal const double HORIZONTALSPLITBOX_WIDTH = 6.0;
        const double _INDICATOR_THICKNESS = 3.0;
        internal bool IsContinueTouchOperation;
        internal bool IsTouchDragFilling;
        internal bool IsTouchDrapDropping;
        internal bool IsTouching;
        internal bool IsTouchPromotedMouseMessage;
        internal bool IsTouchResizingColumns;
        internal bool IsTouchResizingRows;
        internal bool IsTouchSelectingCells;
        internal bool IsTouchSelectingColumns;
        internal bool IsTouchSelectingRows;
        bool _IsTouchStartColumnSelecting;
        bool _IsTouchStartRowSelecting;
        internal bool IsTouchTabStripResizing;
        internal bool IsTouchZooming;
        internal const int _MAXSCROLLABLEHORIZONTALOFFSET = 120;
        internal const int _MAXSCROLLABLEVERTICALOFFSET = 80;
        const int _MOUSEWHEELSCROLLLINES = 3;
        internal const double RESIZE_HEIGHT = 4.0;
        internal const double RESIZE_WIDTH = 4.0;
        const string _ROW_DELIMITER = "\r\n";
        internal const double SPLITBOXWIDTH = 20.0;
        const double _TOOLTIP_OFFSET = 4.0;
        internal const double VERTICALSPLITBOX_HEIGHT = 6.0;
        const float _ZOOM_MAX = 4f;
        const float _ZOOM_MIN = 0.1f;


        Size _cachedLastAvailableSize;
        SplitBoxAlignment _columnSplitBoxAlignment;
        SplitBoxPolicy _columnSplitBoxPolicy;
        Line _columnSplittingTracker;
        CrossSplitBar[,] _crossSplitBar;
        ScrollBar[] _horizontalScrollBar;
        double _horizontalScrollBarHeight;
        Style _horizontalScrollBarStyle;
        HorizontalSplitBar[] _horizontalSplitBar;
        HorizontalSplitBox[] _horizontalSplitBox;
        HashSet<int> _invisibleColumns;
        HashSet<int> _invisibleRows;
        bool _pendinging;
        Grid _progressGrid;
        ProgressRing _progressRing;
        SplitBoxAlignment _rowSplitBoxAlignment;
        SplitBoxPolicy _rowSplitBoxPolicy;
        Line _rowSplittingTracker;
        ScrollBarTrackPolicy _scrollBarTrackPolicy;
        int _scrollTo;
        bool _showScrollTip;
        Canvas _splittingTrackerContainer;
        TabStrip _tabStrip;
        bool _tabStripEditable;
        bool _tabStripInsertTab;
        double _tabStripRatio;
        Visibility _tabStripVisibility;
        ScrollBar[] _verticalScrollBar;
        Style _verticalScrollBarStyle;
        double _verticalScrollBarWidth;
        VerticalSplitBar[] _verticalSplitBar;
        VerticalSplitBox[] _verticalSplitBox;
        internal const double GCSPREAD_HorizontalScrollBarDefaultHeight = 25.0;
        internal const ScrollBarTrackPolicy GCSPREAD_ScrollBarTrackPolicy = ScrollBarTrackPolicy.Both;
        internal const double GCSPREAD_TabStripRatio = 0.5;
        internal const double GCSPREAD_VerticalScrollBarDefaultWidth = 25.0;
        internal static bool IsSwitchingSheet;
        bool IsTouchColumnSplitting;
        bool IsTouchRowSplitting;
        bool IsTouchTabStripScrolling;
        HorizontalSplitBox tabStripSplitBox;
        const double TABSTRIPSPLITBOX_WIDTH = 16.0;
        #endregion

        #region 静态内容
        public static readonly DependencyProperty AutoClipboardProperty = DependencyProperty.Register(
            "AutoClipboard",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanTouchMultiSelectProperty = DependencyProperty.Register(
            "CanTouchMultiSelect",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(false));

        public static readonly DependencyProperty CanUserDragDropProperty = DependencyProperty.Register(
            "CanUserDragDrop",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanUserDragFillProperty = DependencyProperty.Register(
            "CanUserDragFill",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(true, OnCanUserDragFillChanged));

        public static readonly DependencyProperty CanUserEditFormulaProperty = DependencyProperty.Register(
            "CanUserEditFormula",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanUserUndoProperty = DependencyProperty.Register(
            "CanUserUndo",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(true, OnCanUserUndoChanged));

        public static readonly DependencyProperty CanUserZoomProperty = DependencyProperty.Register(
            "CanUserZoom",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ClipBoardOptionsProperty = DependencyProperty.Register(
            "ClipBoardOptions",
            typeof(ClipboardPasteOptions),
            typeof(SheetView),
            new PropertyMetadata(ClipboardPasteOptions.All));

        public static readonly DependencyProperty FreezeLineStyleProperty = DependencyProperty.Register(
            "FreezeLineStyle",
            typeof(Style),
            typeof(SheetView),
            new PropertyMetadata(null, OnFreezeLineStyleChanged));

        public static readonly DependencyProperty HideSelectionWhenPrintingProperty = DependencyProperty.Register(
            "HideSelectionWhenPrinting",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(false));

        public static readonly DependencyProperty HighlightInvalidDataProperty = DependencyProperty.Register(
            "HighlightInvalidData",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(false, OnHighlightInvalidDataChanged));

        static void OnHighlightInvalidDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var st = (SheetView)d;
            if ((bool)e.NewValue)
            {
                st.Invalidate();
            }
            else
            {
                st.RefreshDataValidationInvalidCircles();
            }
        }

        static void OnFreezeLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var st = (SheetView)d;
            Style style = (Style)e.NewValue;
            st._columnFreezeLine.TypeSafeSetStyle(style);
            st._rowFreezeLine.TypeSafeSetStyle(style);
            st.Invalidate();
        }

        static void OnCanUserUndoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var st = (SheetView)d;
            if (st._undoManager != null)
            {
                st._undoManager.AllowUndo = (bool)e.NewValue;
            }
        }

        static void OnCanUserDragFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SheetView)d).InvalidateRange(-1, -1, -1, -1, SheetArea.Cells);
        }
        #endregion
    }
}

