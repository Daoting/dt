#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
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
    public partial class SheetView : Panel, IXmlSerializable
    {
        bool _allowDragDrop;
        bool _allowDragFill = true;
        bool _allowEditOverflow;
        bool _allowUndo;
        bool _allowUserFormula;
        bool _allowUserZoom;
        bool _autoClipboard;
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
        internal bool _canTouchMultiSelect;
        ClipboardPasteOptions _clipBoardOptions;
        internal Line _columnFreezeLine;
        GcRangeGroupHeader _columnGroupHeaderPresenter;
        GcRangeGroup[] _columnGroupPresenters;
        internal GcViewport[] _columnHeaderPresenters;
        int _columnOffset;
        internal Line _columnTrailingFreezeLine;
        internal GcHeaderCornerViewport _cornerPresenter;
        internal int _currentActiveColumnIndex;
        internal int _currentActiveRowIndex;
        DragFillDirection _currentFillDirection = DragFillDirection.Down;
        CellRange _currentFillRange;
        Canvas _cursorsContainer;
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
        GcViewport _editinViewport;
        EditorInfo _editorInfo;
        internal short _eventSuspended;
        Windows.UI.Xaml.Controls.Primitives.Popup _filterPopup;
        PopupHelper _filterPopupHelper;
        Dictionary<KeyStroke, SpreadAction> _floatingObjectsKeyMap = new Dictionary<KeyStroke, SpreadAction>();
        Point _floatingObjectsMovingResizingOffset = new Point(0.0, 0.0);
        int _floatingObjectsMovingResizingStartColumn = -2;
        Point _floatingObjectsMovingResizingStartPoint = new Point(0.0, 0.0);
        Rect _floatingObjectsMovingResizingStartPointCellBounds = new Rect(0.0, 0.0, 0.0, 0.0);
        int _floatingObjectsMovingResizingStartRow = -2;
        Dictionary<string, Point> _floatingObjectsMovingStartLocations = new Dictionary<string, Point>();
        FormulaSelectionFeature _formulaSelectionFeature;
        internal FormulaSelectionGripperContainerPanel _formulaSelectionGripperPanel;
        Style _freezeLineStyle;
        internal GestureRecognizer _gestrueRecognizer;
        GcRangeGroupCorner _groupCornerPresenter;
        bool _hideSelectionWhenPrinting;
        bool _highlightDataValidationInvalidData;
        FilterButtonInfo _hitFilterInfo;
        ScrollSelectionManager _horizontalSelectionMgr;
        internal Control _host;
        internal HoverManager _hoverManager;
        bool _hScrollable;
        FontFamily _inheritedControlFontFamily;
        internal InputDeviceType _inputDeviceType;
        internal bool _isDoubleClick;
        bool _isDragCopy;
        bool _isDragInsert;
        bool _isEditing;
        internal bool _isIMEEnterEditing;
        bool _isMouseDownFloatingObject;
        internal bool _isTouchScrolling;
        Dictionary<KeyStroke, SpreadAction> _keyMap;
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
        internal GcViewport[] _rowHeaderPresenters;
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
        internal GcViewport[,] _viewportPresenters;
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
        Dt.Cells.Data.Worksheet _sheet;
        internal const double SPLITBOXWIDTH = 20.0;
        const double _TOOLTIP_OFFSET = 4.0;
        internal const double VERTICALSPLITBOX_HEIGHT = 6.0;
        const float _ZOOM_MAX = 4f;
        const float _ZOOM_MIN = 0.1f;

        /// <summary>
        /// Occurs when the user presses down the left mouse button in a cell. 
        /// </summary>
        public event EventHandler<CellClickEventArgs> CellClick;

        /// <summary>
        /// Occurs when the user presses down the left mouse button twice (double-clicks) in a cell. 
        /// </summary>
        public event EventHandler<CellDoubleClickEventArgs> CellDoubleClick;

        /// <summary>
        /// Occurs when [cell text rendering].
        /// </summary>
        public event EventHandler<CellTextRenderingEventArgs> CellTextRendering;

        /// <summary>
        /// Occurs when [cell value applying].
        /// </summary>
        public event EventHandler<CellValueApplyingEventArgs> CellValueApplying;

        /// <summary>
        /// Occurs when a Clipboard change occurs that affects GcSpreadSheet.
        /// </summary>
        public event EventHandler<EventArgs> ClipboardChanged;

        /// <summary>
        /// Occurs when the Clipboard is changing due to a GcSpreadSheet action.
        /// </summary>
        public event EventHandler<EventArgs> ClipboardChanging;

        /// <summary>
        /// Occurs when the user has pasted from the Clipboard.
        /// </summary>
        public event EventHandler<ClipboardPastedEventArgs> ClipboardPasted;

        /// <summary>
        /// Occurs when the user pastes from the Clipboard.
        /// </summary>
        public event EventHandler<ClipboardPastingEventArgs> ClipboardPasting;

        /// <summary>
        /// Occurs when the column width has changed.
        /// </summary>
        public event EventHandler<ColumnWidthChangedEventArgs> ColumnWidthChanged;

        /// <summary>
        /// Occurs when the column width is changing.
        /// </summary>
        public event EventHandler<ColumnWidthChangingEventArgs> ColumnWidthChanging;

        /// <summary>
        /// Occurs when [data validation list popup opening].
        /// </summary>
        public event EventHandler<CellCancelEventArgs> DataValidationListPopupOpening;

        /// <summary>
        /// Occurs when the user drags and drops a range of cells.
        /// </summary>
        public event EventHandler<DragDropBlockEventArgs> DragDropBlock;

        /// <summary>
        /// Occurs at the completion of the user dragging and dropping a range of cells.
        /// </summary>
        public event EventHandler<DragDropBlockCompletedEventArgs> DragDropBlockCompleted;

        /// <summary>
        /// Occurs when the user drags to fill a range of cells.
        /// </summary>
        public event EventHandler<DragFillBlockEventArgs> DragFillBlock;

        /// <summary>
        /// Occurs at the completion of the user dragging to fill a range of cells.
        /// </summary>
        public event EventHandler<DragFillBlockCompletedEventArgs> DragFillBlockCompleted;

        /// <summary>
        /// Occurs when a cell is in edit mode and the text is changed.
        /// </summary>
        public event EventHandler<EditCellEventArgs> EditChange;

        /// <summary>
        /// Occurs when a cell leaves edit mode.
        /// </summary>
        public event EventHandler<EditCellEventArgs> EditEnd;

        /// <summary>
        /// Occurs when a cell is going in to edit mode.
        /// </summary>
        public event EventHandler<EditCellStartingEventArgs> EditStarting;

        /// <summary>
        /// Occurs when the user enters a cell. 
        /// </summary>
        public event EventHandler<EnterCellEventArgs> EnterCell;

        /// <summary>
        /// Occurs when the user performs an invalid operation.
        /// </summary>
        internal event EventHandler<UserErrorEventArgs> Error;

        /// <summary>
        /// Occurs when [filter popup opening].
        /// </summary>
        public event EventHandler<CellCancelEventArgs> FilterPopupOpening;

        /// <summary>
        /// Occurs when [floating object pasted].
        /// </summary>
        public event EventHandler<FloatingObjectPastedEventArgs> FloatingObjectPasted;

        /// <summary>
        /// Occurs when an invalid operation is performed.
        /// </summary>
        public event EventHandler<InvalidOperationEventArgs> InvalidOperation;

        /// <summary>
        /// Occurs when the user leaves a cell. 
        /// </summary>
        public event EventHandler<LeaveCellEventArgs> LeaveCell;

        /// <summary>
        /// Occurs when the left column changes.
        /// </summary>
        public event EventHandler<ViewportEventArgs> LeftColumnChanged;

        /// <summary>
        /// Occurs when a column has just been automatically sorted.
        /// </summary>
        public event EventHandler<RangeFilteredEventArgs> RangeFiltered;

        /// <summary>
        /// Occurs when a column is about to be automatically filtered.
        /// </summary>
        public event EventHandler<RangeFilteringEventArgs> RangeFiltering;

        /// <summary>
        /// Occurs when the user has changed the state of outline (range group) rows
        /// or columns.
        /// </summary>
        public event EventHandler<RangeGroupStateChangedEventArgs> RangeGroupStateChanged;

        /// <summary>
        /// Occurs before the user changes the state of outline (range group) rows
        /// or columns.
        /// </summary>
        public event EventHandler<RangeGroupStateChangingEventArgs> RangeGroupStateChanging;

        /// <summary>
        /// Occurs when a column has just been automatically sorted.
        /// </summary>
        public event EventHandler<RangeSortedEventArgs> RangeSorted;

        /// <summary>
        /// Occurs when a column is about to be automatically sorted.
        /// </summary>
        public event EventHandler<RangeSortingEventArgs> RangeSorting;

        /// <summary>
        /// Occurs when the row height has changed.
        /// </summary>
        public event EventHandler<RowHeightChangedEventArgs> RowHeightChanged;

        /// <summary>
        /// Occurs when the row height is changing.
        /// </summary>
        public event EventHandler<RowHeightChangingEventArgs> RowHeightChanging;

        /// <summary>
        /// Occurs when the selection of cells on the sheet has changed. 
        /// </summary>
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Occurs when the selection of cells on the sheet is changing.
        /// </summary>
        public event EventHandler<SelectionChangingEventArgs> SelectionChanging;

        /// <summary>
        /// Occurs when the user clicks the sheet tab.
        /// </summary>
        public event EventHandler<SheetTabClickEventArgs> SheetTabClick;

        /// <summary>
        /// Occurs when the user double-clicks the sheet tab.
        /// </summary>
        public event EventHandler<SheetTabDoubleClickEventArgs> SheetTabDoubleClick;

        /// <summary>
        /// Occurs when the top row changes.
        /// </summary>
        public event EventHandler<ViewportEventArgs> TopRowChanged;

        /// <summary>
        /// Occurs before GcSpreadSheet show toolstrip menu bar
        /// </summary>
        internal event EventHandler<TouchToolbarOpeningEventArgs> TouchToolbarOpening;

        /// <summary>
        /// Occurs when the user types a formula.
        /// </summary>
        public event EventHandler<UserFormulaEnteredEventArgs> UserFormulaEntered;

        /// <summary>
        /// Occurs when the user zooms.
        /// </summary>
        public event EventHandler<ZoomEventArgs> UserZooming;

        /// <summary>
        /// Occurs when the user drags and drops a range of cells.
        /// </summary>
        public event EventHandler<ValidationDragDropBlockEventArgs> ValidationDragDropBlock;

        /// <summary>
        /// Occurs when the applied cell value is invalid.
        /// </summary>
        public event EventHandler<ValidationErrorEventArgs> ValidationError;

        /// <summary>
        /// Occurs when validation the whether the pasting is validate.
        /// </summary>
        public event EventHandler<ValidationPastingEventArgs> ValidationPasting;

        /// <summary>
        /// Occurs when the value in the subeditor changes. 
        /// </summary>
        public event EventHandler<CellEventArgs> ValueChanged;

    }
}

