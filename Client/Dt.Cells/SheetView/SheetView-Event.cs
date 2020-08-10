#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    public partial class SheetView : Panel, IXmlSerializable
    {
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


        /// <summary>
        /// Occurs when the user has changed the active sheet. 
        /// </summary>
        public event EventHandler ActiveSheetChanged;

        /// <summary>
        /// Occurs when the user changes the active sheet. 
        /// </summary>
        public event EventHandler<CancelEventArgs> ActiveSheetChanging;

        /// <summary>
        /// Occurs when the user has changed a viewport column width. 
        /// </summary>
        public event EventHandler<ColumnViewportWidthChangedEventArgs> ColumnViewportWidthChanged;

        /// <summary>
        /// Occurs when the user changes a viewport column width. 
        /// </summary>
        public event EventHandler<ColumnViewportWidthChangingEventArgs> ColumnViewportWidthChanging;

        /// <summary>
        /// Occurs when the user has changed a viewport row height. 
        /// </summary>
        public event EventHandler<RowViewportHeightChangedEventArgs> RowViewportHeightChanged;

        /// <summary>
        /// Occurs when the user changes a viewport row height. 
        /// </summary>
        public event EventHandler<RowViewportHeightChangingEventArgs> RowViewportHeightChanging;

    }
}

