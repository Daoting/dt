﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        /// <summary>
        /// 是否开始执行某个操作，如调整行高列宽、拖拽、浮动窗口移动...
        /// </summary>
        internal bool IsWorking { get; set; }

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

        internal bool IsMouseLeftButtonPressed { get; set; }

        internal bool IsMouseRightButtonPressed { get; set; }

        internal bool IsSelectionBegined
        {
            get { return _formulaSelectionFeature.IsSelectionBegined; }
        }

        internal bool IsTouchingMovingFloatingObjects { get; set; }

        internal bool IsTouchingResizingFloatingObjects { get; set; }

        /// <summary>
        /// 设置打印时隐藏分页线
        /// </summary>
        internal bool HideDecorationWhenPrinting
        {
            set
            {
                if (_cellsPanels != null)
                {
                    int rowBound = _cellsPanels.GetUpperBound(0);
                    int colBound = _cellsPanels.GetUpperBound(1);
                    for (int i = _cellsPanels.GetLowerBound(0); i <= rowBound; i++)
                    {
                        for (int j = _cellsPanels.GetLowerBound(1); j <= colBound; j++)
                        {
                            CellsPanel viewport = _cellsPanels[i, j];
                            if (viewport != null)
                                viewport.HideDecorationWhenPrinting = value;
                        }
                    }
                }
            }
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

        internal CellsPanel EditingViewport { get; set; }

        internal FormulaEditorConnector EditorConnector
        {
            get { return _formulaSelectionFeature.FormulaEditorConnector; }
        }

        internal bool EditorDirty
        {
            get { return ((EditingViewport != null) && EditingViewport.EditorDirty); }
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

        internal HoverManager HoverManager
        {
            get { return _hoverManager; }
        }

        internal bool IsDraggingFill { get; set; }

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

        bool IsDragToColumnInView
        {
            get { return IsColumnInViewport(_dragToColumnViewport, _dragToColumn); }
        }

        bool IsDragToRowInView
        {
            get { return IsRowInViewport(_dragToRowViewport, _dragToRow); }
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

        bool IsMovingFloatingOjects { get; set; }

        bool IsResizingColumns { get; set; }

        bool IsResizingFloatingObjects { get; set; }

        bool IsResizingRows { get; set; }

        bool IsSelectingCells { get; set; }

        bool IsSelectingColumns { get; set; }

        bool IsSelectingRows { get; set; }

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

        internal Rect? ResizerGripperRect { get; set; }

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
                    Children.Add(_tooltipPopup);
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
        internal Style TrailingFreezeLineStyle
        {
            get { return (Style)GetValue(TrailingFreezeLineStyleProperty); }
            set { SetValue(TrailingFreezeLineStyleProperty, value); }
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


        // 以下未整理

        #region 成员变量
        Workbook _workbook;
        RowHeaderPanel[] _rowHeaders;
        ColHeaderPanel[] _colHeaders;
        CornerPanel _cornerPanel;
        CellsPanel[,] _cellsPanels;

        Size _paperSize;
        CellRange _decorationRange;
        Rect? _autoFillIndicatorRec;
        GripperLocationsStruct _gripperLocations;
        Border _autoFillIndicatorContainer;
        Size _availableSize;
        Ellipse _bottomRightGripper;
        Image _cachedautoFillIndicatorImage;
        Image _cachedBottomRightACornerVisual;
        CellLayoutModel[] _cachedColumnHeaderCellLayoutModel;
        RowLayoutModel _cachedColumnHeaderRowLayoutModel;
        ColumnLayoutModel[] _cachedColumnHeaderViewportColumnLayoutModel;
        TransformGroup[] _cachedColumnHeaderViewportTransform;
        Image[] _cachedColumnHeaderViewportVisual;
        Image _cachedColumnResizerGripperImage;
        TransformGroup _cachedCornerViewportTransform;
        Image _cachedCornerViewportVisual;
        FilterButtonInfoModel _cachedFilterButtonInfoModel;
        FloatingObjectLayoutModel[,] _cachedFloatingObjectLayoutModel;
        FloatingObjectLayoutModel[,] _cachedFloatingObjectMovingResizingLayoutModel;
        GroupLayout _cachedGroupLayout;
        SheetLayout _cachedLayout;
        Dictionary<string, BitmapImage> _cachedResizerGipper;
        CellLayoutModel[] _cachedRowHeaderCellLayoutModel;
        ColumnLayoutModel _cachedRowHeaderColumnLayoutModel;
        RowLayoutModel[] _cachedRowHeaderViewportRowLayoutModel;
        TransformGroup[] _cachedRowHeaderViewportTransform;
        Image[] _cachedRowHeaderViewportVisual;
        Image _cachedRowResizerGripperImage;
        Dictionary<string, ImageSource> _cachedToolbarImageSources;
        CellLayoutModel[,] _cachedViewportCellLayoutModel;
        ColumnLayoutModel[] _cachedViewportColumnLayoutModel;
        double[] _cachedViewportHeights;
        RowLayoutModel[] _cachedViewportRowLayoutModel;
        double[] _cachedViewportSplitBarX;
        double[] _cachedViewportSplitBarY;
        TransformGroup[,] _cachedViewportTransform;
        Image[,] _cachedViewportVisual;
        double[] _cachedViewportWidths;
        Line _columnFreezeLine;
        GcRangeGroupHeader _columnGroupHeaderPresenter;
        GcRangeGroup[] _columnGroupPresenters;

        int _columnOffset;
        Line _columnTrailingFreezeLine;
        int _currentActiveColumnIndex;
        int _currentActiveRowIndex;
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
        GestureRecognizer _gestrueRecognizer;
        GcRangeGroupCorner _groupCornerPresenter;
        FilterButtonInfo _hitFilterInfo;
        ScrollSelectionManager _horizontalSelectionMgr;
        HoverManager _hoverManager;
        InputDeviceType _inputDeviceType;
        bool _isDoubleClick;
        bool _isDragCopy;
        bool _isDragInsert;
        bool _isEditing;
        bool _isMouseDownFloatingObject;
        bool _isTouchScrolling;

        Point _lastClickLocation;
        Point _lastClickPoint;
        Image _mouseCursor;
        Point _mouseDownPosition;
        FloatingObject[] _movingResizingFloatingObjects;
        SpreadXNavigation _navigation;
        HitTestInformation _positionInfo;
        CopyMoveCellsInfo _preFillCellsInfo;
        uint? _primaryTouchDeviceId = null;
        bool _protect;
        bool _resetSelectionFrameStroke;
        Border _resizerGripperContainer;
        Line _resizingTracker;
        PointerRoutedEventArgs _routedEventArgs;
        Line _rowFreezeLine;
        GcRangeGroupHeader _rowGroupHeaderPresenter;
        GcRangeGroup[] _rowGroupPresenters;

        int _rowOffset;
        Line _rowTrailingFreezeLine;
        SpreadXSelection _selection;
        Canvas _shapeDrawingContainer;
        int _suspendViewInvalidate;
        TooltipPopupHelper _tooltipHelper;
        Windows.UI.Xaml.Controls.Primitives.Popup _tooltipPopup;
        Ellipse _topLeftGripper;
        HashSet<uint> _touchProcessedPointIds = new HashSet<uint>();
        HitTestInformation _touchStartHitTestInfo;
        int _touchStartLeftColumn = -1;
        Point _touchStartPoint;
        int _touchStartTopRow = -1;
        Windows.UI.Xaml.Controls.Primitives.Popup _touchToolbarPopup = null;
        //Point _touchTranslatePoint;
        double _touchZoomInitFactor;
        double _touchZoomNewFactor;
        Point _touchZoomOrigin;
        Canvas _trackersContainer;
        double _translateOffsetX;
        double _translateOffsetY;
        UndoManager _undoManager;
        bool _updateViewportAfterTouch;
        ScrollSelectionManager _verticalSelectionMgr;

        HitTestInformation _zoomOriginHitTestInfo;
        GripperLocationsStruct CachedGripperLocation;
        const string _CELL_DELIMITER = "\"";
        const string _COLUMN_DELIMITER = "\t";
        bool _DoTouchResizing;
        const double ENHANCED_ZERO_INDICATOR_WIDTH = 6.0;
        internal bool _fastScroll;
        const double FILTERBUTTON_HEIGHT = 16.0;
        const double FILTERBUTTON_WIDTH = 16.0;
        static readonly Size GCSPREAD_DefaultSize = new Size(500.0, 500.0);
        const int GRIPPERSIZE = 0x10;
        const double _GROUPBUTTON_HEIGHT = 16.0;
        const double _GROUPBUTTON_WIDTH = 16.0;
        const double HALF_ENHANCED_ZERO_INDICATOR_WIDTH = 3.0;
        const double HORIZONTALSPLITBOX_WIDTH = 6.0;
        const double _INDICATOR_THICKNESS = 3.0;
        bool IsContinueTouchOperation;
        bool IsTouchDragFilling;
        bool IsTouchDrapDropping;
        internal bool IsTouching;
        bool IsTouchPromotedMouseMessage;
        bool IsTouchResizingColumns;
        bool IsTouchResizingRows;
        bool IsTouchSelectingCells;
        bool IsTouchSelectingColumns;
        bool IsTouchSelectingRows;
        bool _IsTouchStartColumnSelecting;
        bool _IsTouchStartRowSelecting;
        bool IsTouchTabStripResizing;
        bool IsTouchZooming;
        const int _MAXSCROLLABLEHORIZONTALOFFSET = 120;
        const int _MAXSCROLLABLEVERTICALOFFSET = 80;
        const int _MOUSEWHEELSCROLLLINES = 3;
        const double RESIZE_HEIGHT = 4.0;
        const double RESIZE_WIDTH = 4.0;
        const string _ROW_DELIMITER = "\r\n";
        const double SPLITBOXWIDTH = 20.0;
        const double _TOOLTIP_OFFSET = 4.0;
        const double VERTICALSPLITBOX_HEIGHT = 6.0;
        const float _ZOOM_MAX = 4f;
        const float _ZOOM_MIN = 0.1f;

        Size _cachedLastAvailableSize;
        Line _columnSplittingTracker;
        CrossSplitBar[,] _crossSplitBar;
        ScrollBar[] _horizontalScrollBar;
        HorizontalSplitBar[] _horizontalSplitBar;
        HorizontalSplitBox[] _horizontalSplitBox;
        HashSet<int> _invisibleColumns;
        HashSet<int> _invisibleRows;
        bool _pendinging;
        Grid _progressGrid;
        ProgressRing _progressRing;
        Line _rowSplittingTracker;
        int _scrollTo;
        bool _showScrollTip;
        Canvas _splittingTrackerContainer;
        TabStrip _tabStrip;
        ScrollBar[] _verticalScrollBar;
        VerticalSplitBar[] _verticalSplitBar;
        VerticalSplitBox[] _verticalSplitBox;
        const double GCSPREAD_HorizontalScrollBarDefaultHeight = 25.0;
        const double GCSPREAD_TabStripRatio = 0.5;
        const double GCSPREAD_VerticalScrollBarDefaultWidth = 25.0;
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
            typeof(Excel),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanTouchMultiSelectProperty = DependencyProperty.Register(
            "CanTouchMultiSelect",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(false));

        public static readonly DependencyProperty CanUserDragDropProperty = DependencyProperty.Register(
            "CanUserDragDrop",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanUserDragFillProperty = DependencyProperty.Register(
            "CanUserDragFill",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true, OnCanUserDragFillChanged));

        public static readonly DependencyProperty CanUserEditFormulaProperty = DependencyProperty.Register(
            "CanUserEditFormula",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanUserUndoProperty = DependencyProperty.Register(
            "CanUserUndo",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true, OnCanUserUndoChanged));

        public static readonly DependencyProperty CanUserZoomProperty = DependencyProperty.Register(
            "CanUserZoom",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ClipBoardOptionsProperty = DependencyProperty.Register(
            "ClipBoardOptions",
            typeof(ClipboardPasteOptions),
            typeof(Excel),
            new PropertyMetadata(ClipboardPasteOptions.All));

        public static readonly DependencyProperty FreezeLineStyleProperty = DependencyProperty.Register(
            "FreezeLineStyle",
            typeof(Style),
            typeof(Excel),
            new PropertyMetadata(null, OnFreezeLineStyleChanged));

        public static readonly DependencyProperty HideSelectionWhenPrintingProperty = DependencyProperty.Register(
            "HideSelectionWhenPrinting",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(false));

        public static readonly DependencyProperty HighlightInvalidDataProperty = DependencyProperty.Register(
            "HighlightInvalidData",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(false, OnHighlightInvalidDataChanged));

        public static readonly DependencyProperty ColumnSplitBoxAlignmentProperty = DependencyProperty.Register(
            "ColumnSplitBoxAlignment",
            typeof(SplitBoxAlignment),
            typeof(Excel),
            new PropertyMetadata(SplitBoxAlignment.Leading, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty ColumnSplitBoxPolicyProperty = DependencyProperty.Register(
            "ColumnSplitBoxPolicy",
            typeof(SplitBoxPolicy),
            typeof(Excel),
            new PropertyMetadata(SplitBoxPolicy.Always, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty HorizontalScrollBarHeightProperty = DependencyProperty.Register(
            "HorizontalScrollBarHeight",
            typeof(double),
            typeof(Excel),
            new PropertyMetadata(25.0d, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty DefaultAutoFillTypeProperty = DependencyProperty.Register(
            "DefaultAutoFillType",
            typeof(AutoFillType?),
            typeof(Excel),
            new PropertyMetadata(null));

        public static readonly DependencyProperty DocumentUriProperty = DependencyProperty.Register(
            "DocumentUri",
            typeof(Uri),
            typeof(Excel),
            new PropertyMetadata(null, OnDocumentUriChanged));

        public static readonly DependencyProperty HorizontalScrollBarStyleProperty = DependencyProperty.Register(
            "HorizontalScrollBarStyle",
            typeof(Style),
            typeof(Excel),
            new PropertyMetadata(null, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty RangeGroupBackgroundProperty = DependencyProperty.Register(
            "RangeGroupBackground",
            typeof(Brush),
            typeof(Excel),
            new PropertyMetadata(null, OnInvalidateChanged));

        public static readonly DependencyProperty RangeGroupBorderBrushProperty = DependencyProperty.Register(
            "RangeGroupBorderBrush",
            typeof(Brush),
            typeof(Excel),
            new PropertyMetadata(null, OnInvalidateChanged));

        public static readonly DependencyProperty RangeGroupLineStrokeProperty = DependencyProperty.Register(
            "RangeGroupLineStroke",
            typeof(Brush),
            typeof(Excel),
            new PropertyMetadata(null, OnInvalidateChanged));

        public static readonly DependencyProperty ResizeZeroIndicatorProperty = DependencyProperty.Register(
            "ResizeZeroIndicator",
            typeof(ResizeZeroIndicator),
            typeof(Excel),
            new PropertyMetadata(Cells.UI.ResizeZeroIndicator.Default));

        public static readonly DependencyProperty RowSplitBoxAlignmentProperty = DependencyProperty.Register(
            "RowSplitBoxAlignment",
            typeof(SplitBoxAlignment),
            typeof(Excel),
            new PropertyMetadata(SplitBoxAlignment.Leading, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty RowSplitBoxPolicyProperty = DependencyProperty.Register(
            "RowSplitBoxPolicy",
            typeof(SplitBoxPolicy),
            typeof(Excel),
            new PropertyMetadata(SplitBoxPolicy.Always, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty ScrollBarTrackPolicyProperty = DependencyProperty.Register(
            "ScrollBarTrackPolicy",
            typeof(ScrollBarTrackPolicy),
            typeof(Excel),
            new PropertyMetadata(ScrollBarTrackPolicy.Both, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty ShowColumnRangeGroupProperty = DependencyProperty.Register(
            "ShowColumnRangeGroup",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty ShowFreezeLineProperty = DependencyProperty.Register(
            "ShowFreezeLine",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true, OnShowFreezeLineChanged));

        public static readonly DependencyProperty ShowRowRangeGroupProperty = DependencyProperty.Register(
            "ShowRowRangeGroup",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty TabStripEditableProperty = DependencyProperty.Register(
            "TabStripEditable",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true, OnTabStripEditableChanged));

        public static readonly DependencyProperty TabStripInsertTabProperty = DependencyProperty.Register(
            "TabStripInsertTab",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true, OnTabStripInsertTabChanged));

        public static readonly DependencyProperty TabStripRatioProperty = DependencyProperty.Register(
            "TabStripRatio",
            typeof(double),
            typeof(Excel),
            new PropertyMetadata(0.5d, OnTabStripRatioChanged));

        public static readonly DependencyProperty TabStripVisibilityProperty = DependencyProperty.Register(
            "TabStripVisibility",
            typeof(Visibility),
            typeof(Excel),
            new PropertyMetadata(Visibility.Visible, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty VerticalScrollBarStyleProperty = DependencyProperty.Register(
            "VerticalScrollBarStyle",
            typeof(Style),
            typeof(Excel),
            new PropertyMetadata(null, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty VerticalScrollBarWidthProperty = DependencyProperty.Register(
            "VerticalScrollBarWidth",
            typeof(double),
            typeof(Excel),
            new PropertyMetadata(25.0d, OnInvalidateLayoutChanged));

        public static readonly DependencyProperty HorizontalScrollableProperty = DependencyProperty.Register(
            "HorizontalScrollable",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true));

        public static readonly DependencyProperty VerticalScrollableProperty = DependencyProperty.Register(
            "VerticalScrollable",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true));

        public static readonly DependencyProperty TrailingFreezeLineStyleProperty = DependencyProperty.Register(
            "TrailingFreezeLineStyle",
            typeof(Style),
            typeof(Excel),
            new PropertyMetadata(null, OnTrailingFreezeLineStyleChanged));

        /// <summary>
        /// 是否显示修饰层
        /// </summary>
        public readonly static DependencyProperty ShowDecorationProperty = DependencyProperty.Register(
            "ShowDecoration",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(false));

        public readonly static DependencyProperty CanEditOverflowProperty = DependencyProperty.Register(
            "CanEditOverflow",
            typeof(bool),
            typeof(Excel),
            new PropertyMetadata(true));

        static void OnTrailingFreezeLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            var style = (Style)e.NewValue;
            excel._columnTrailingFreezeLine.TypeSafeSetStyle(style);
            excel._rowTrailingFreezeLine.TypeSafeSetStyle(style);
            excel.Invalidate();
        }

        static void OnTabStripRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            double val = (double)e.NewValue;
            if ((val >= 0.0) && (val <= 1.0))
            {
                excel.InvalidateLayout();
                excel.InvalidateMeasure();
            }
        }

        static void OnTabStripInsertTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            if ((excel._tabStrip != null) && ((bool)e.NewValue != excel._tabStrip.HasInsertTab))
            {
                excel._tabStrip.HasInsertTab = (bool)e.NewValue;
                excel.InvalidateLayout();
                excel.InvalidateMeasure();
            }
        }

        static void OnTabStripEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            if (!(bool)e.NewValue && excel._tabStrip != null && excel._tabStrip.IsEditing)
            {
                excel._tabStrip.StopTabEditing(false);
            }
        }

        static void OnShowFreezeLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Excel)d).UpdateFreezeLines();
        }

        static void OnInvalidateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Excel)d).Invalidate();
        }

        static void OnDocumentUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            Uri uri = (Uri)e.NewValue;
            if (uri == null)
                return;

            Action<Task<StorageFile>> action = null;
            Action<Task<StorageFile>> action2 = null;
            bool xmlFile = false;
            bool excelFile = false;
            string str = uri.IsAbsoluteUri ? uri.LocalPath : uri.OriginalString;
            if (!string.IsNullOrEmpty(str))
            {
                string extension = System.IO.Path.GetExtension(str);
                switch (extension)
                {
                    case ".xml":
                    case ".ssxml":
                        xmlFile = true;
                        break;
                }
                if ((extension == ".xls") || (extension == ".xlsx"))
                {
                    excelFile = true;
                }
            }

            if (uri.IsAbsoluteUri)
            {
                Workbook workbook = excel.Workbook;
                if (action == null)
                {
                    action = delegate (Task<StorageFile> fr)
                    {
                        Func<Task<IRandomAccessStreamWithContentType>, Task> func = null;
                        if ((fr.Result != null) && !fr.IsFaulted)
                        {
                            if (func == null)
                            {
                                func = async delegate (Task<IRandomAccessStreamWithContentType> r)
                                {
                                    if ((r.Result != null) && !r.IsFaulted)
                                    {
                                        using (Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(r.Result))
                                        {
                                            if (xmlFile)
                                            {
                                                await excel.OpenXmlAsync(stream);
                                            }
                                            if (excelFile)
                                            {
                                                await excel.OpenExcelAsync(stream);
                                            }
                                        }
                                    }
                                };
                            }
                            WindowsRuntimeSystemExtensions.AsTask<IRandomAccessStreamWithContentType>(fr.Result.OpenReadAsync()).ContinueWith<Task>(func);
                        }
                    };
                }
                WindowsRuntimeSystemExtensions.AsTask<StorageFile>(StorageFile.GetFileFromApplicationUriAsync(uri)).ContinueWith(action);
            }
            else
            {
                if (action2 == null)
                {
                    action2 = delegate (Task<StorageFile> fr)
                    {
                        Func<Task<IRandomAccessStreamWithContentType>, Task> func = null;
                        if (fr.Result != null)
                        {
                            if (func == null)
                            {
                                func = async delegate (Task<IRandomAccessStreamWithContentType> r)
                                {
                                    if (r.Result != null)
                                    {
                                        using (Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(r.Result))
                                        {
                                            if (xmlFile)
                                            {
                                                await excel.OpenXmlAsync(stream);
                                            }
                                            if (excelFile)
                                            {
                                                await excel.OpenExcelAsync(stream);
                                            }
                                        }
                                    }
                                };
                            }
                            WindowsRuntimeSystemExtensions.AsTask<IRandomAccessStreamWithContentType>(fr.Result.OpenReadAsync()).ContinueWith<Task>(func);
                        }
                    };
                }
                WindowsRuntimeSystemExtensions.AsTask<StorageFile>(StorageFile.GetFileFromApplicationUriAsync(uri)).ContinueWith(action2);
            }
        }

        static void OnInvalidateLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            excel.InvalidateLayout();
            excel.InvalidateMeasure();
        }

        static void OnHighlightInvalidDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            if ((bool)e.NewValue)
            {
                excel.Invalidate();
            }
            else
            {
                excel.RefreshDataValidationInvalidCircles();
            }
        }

        static void OnFreezeLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            Style style = (Style)e.NewValue;
            excel._columnFreezeLine.TypeSafeSetStyle(style);
            excel._rowFreezeLine.TypeSafeSetStyle(style);
            excel.Invalidate();
        }

        static void OnCanUserUndoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var excel = (Excel)d;
            if (excel._undoManager != null)
            {
                excel._undoManager.AllowUndo = (bool)e.NewValue;
            }
        }

        static void OnCanUserDragFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Excel)d).InvalidateRange(-1, -1, -1, -1, SheetArea.Cells);
        }
        #endregion
    }
}