#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    public partial class Excel : Control
    {
        /// <summary>
        /// Occurs when the user has changed the active sheet. 
        /// </summary>
        public event EventHandler ActiveSheetChanged
        {
            add { View.ActiveSheetChanged += value; }
            remove { View.ActiveSheetChanged -= value; }
        }

        /// <summary>
        /// Occurs when the user changes the active sheet. 
        /// </summary>
        public event EventHandler<CancelEventArgs> ActiveSheetChanging
        {
            add { View.ActiveSheetChanging += value; }
            remove { View.ActiveSheetChanging -= value; }
        }

        /// <summary>
        /// Occurs when the user presses down the left mouse button in a cell. 
        /// </summary>
        public event EventHandler<CellClickEventArgs> CellClick
        {
            add { View.CellClick += value; }
            remove { View.CellClick -= value; }
        }

        /// <summary>
        /// Occurs when the user presses down the left mouse button twice (double-clicks) in a cell. 
        /// </summary>
        public event EventHandler<CellDoubleClickEventArgs> CellDoubleClick
        {
            add { View.CellDoubleClick += value; }
            remove { View.CellDoubleClick -= value; }
        }

        /// <summary>
        /// Occurs when the cell text is rendering. 
        /// </summary>
        public event EventHandler<CellTextRenderingEventArgs> CellTextRendering
        {
            add { View.CellTextRendering += value; }
            remove { View.CellTextRendering -= value; }
        }

        /// <summary>
        /// Occurs when [cell value applying]. 
        /// </summary>
        public event EventHandler<CellValueApplyingEventArgs> CellValueApplying
        {
            add { View.CellValueApplying += value; }
            remove { View.CellValueApplying -= value; }
        }

        /// <summary>
        /// Occurs when a Clipboard change occurs that can effect the GcSpreadSheet control. 
        /// </summary>
        public event EventHandler<EventArgs> ClipboardChanged
        {
            add { View.ClipboardChanged += value; }
            remove { View.ClipboardChanged -= value; }
        }

        /// <summary>
        /// Occurs when the Clipboard is changing due to a GcSpreadSheet action. 
        /// </summary>
        public event EventHandler<EventArgs> ClipboardChanging
        {
            add { View.ClipboardChanging += value; }
            remove { View.ClipboardChanging -= value; }
        }

        /// <summary>
        /// Occurs when the user pastes from the Clipboard. 
        /// </summary>
        public event EventHandler<ClipboardPastedEventArgs> ClipboardPasted
        {
            add { View.ClipboardPasted += value; }
            remove { View.ClipboardPasted -= value; }
        }

        /// <summary>
        /// Occurs when the user pastes from the Clipboard. 
        /// </summary>
        public event EventHandler<ClipboardPastingEventArgs> ClipboardPasting
        {
            add { View.ClipboardPasting += value; }
            remove { View.ClipboardPasting -= value; }
        }

        /// <summary>
        /// Occurs when a viewport column width has changed. 
        /// </summary>
        public event EventHandler<ColumnViewportWidthChangedEventArgs> ColumnViewportWidthChanged
        {
            add { View.ColumnViewportWidthChanged += value; }
            remove { View.ColumnViewportWidthChanged -= value; }
        }

        /// <summary>
        /// Occurs when a viewport column is about to be changed. 
        /// </summary>
        public event EventHandler<ColumnViewportWidthChangingEventArgs> ColumnViewportWidthChanging
        {
            add { View.ColumnViewportWidthChanging += value; }
            remove { View.ColumnViewportWidthChanging -= value; }
        }

        /// <summary>
        /// Occurs when the column width has changed. 
        /// </summary>
        public event EventHandler<ColumnWidthChangedEventArgs> ColumnWidthChanged
        {
            add { View.ColumnWidthChanged += value; }
            remove { View.ColumnWidthChanged -= value; }
        }

        /// <summary>
        /// Occurs when the column width is changing. 
        /// </summary>
        public event EventHandler<ColumnWidthChangingEventArgs> ColumnWidthChanging
        {
            add { View.ColumnWidthChanging += value; }
            remove { View.ColumnWidthChanging -= value; }
        }

        /// <summary>
        /// Occurs when a data validation list popup is opening. 
        /// </summary>
        public event EventHandler<CellCancelEventArgs> DataValidationListPopupOpening
        {
            add { View.DataValidationListPopupOpening += value; }
            remove { View.DataValidationListPopupOpening -= value; }
        }

        /// <summary>
        /// Occurs when the user drags and drops a range of cells. 
        /// </summary>
        public event EventHandler<DragDropBlockEventArgs> DragDropBlock
        {
            add { View.DragDropBlock += value; }
            remove { View.DragDropBlock -= value; }
        }

        /// <summary>
        /// Occurs at the completion of the user dragging and dropping a range of cells. 
        /// </summary>
        public event EventHandler<DragDropBlockCompletedEventArgs> DragDropBlockCompleted
        {
            add { View.DragDropBlockCompleted += value; }
            remove { View.DragDropBlockCompleted -= value; }
        }

        /// <summary>
        /// Occurs when the user drags to fill a range of cells.
        /// </summary>
        public event EventHandler<DragFillBlockEventArgs> DragFillBlock
        {
            add { View.DragFillBlock += value; }
            remove { View.DragFillBlock -= value; }
        }

        /// <summary>
        /// Occurs at the completion of the user dragging to fill a range of cells. 
        /// </summary>
        public event EventHandler<DragFillBlockCompletedEventArgs> DragFillBlockCompleted
        {
            add { View.DragFillBlockCompleted += value; }
            remove { View.DragFillBlockCompleted -= value; }
        }

        /// <summary>
        /// Occurs when a cell is in edit mode and the text is changed. 
        /// </summary>
        public event EventHandler<EditCellEventArgs> EditChange
        {
            add { View.EditChange += value; }
            remove { View.EditChange -= value; }
        }

        /// <summary>
        /// Occurs when a cell leaves edit mode. 
        /// </summary>
        public event EventHandler<EditCellEventArgs> EditEnd
        {
            add { View.EditEnd += value; }
            remove { View.EditEnd -= value; }
        }

        /// <summary>
        /// Occurs when a cell goes in edit mode. 
        /// </summary>
        public event EventHandler<EditCellStartingEventArgs> EditStarting
        {
            add { View.EditStarting += value; }
            remove { View.EditStarting -= value; }
        }

        /// <summary>
        /// Occurs when the user enters a cell. 
        /// </summary>
        public event EventHandler<EnterCellEventArgs> EnterCell
        {
            add { View.EnterCell += value; }
            remove { View.EnterCell -= value; }
        }

        /// <summary>
        /// Occurs when an error occurs during loading or saving an Excel-formatted file.
        /// </summary>
        public event EventHandler<ExcelErrorEventArgs> ExcelError
        {
            add { Workbook.ExcelError += value; }
            remove { Workbook.ExcelError -= value; }
        }

        /// <summary>
        /// Occurs when the filter popup is opening. 
        /// </summary>
        public event EventHandler<CellCancelEventArgs> FilterPopupOpening
        {
            add { View.FilterPopupOpening += value; }
            remove { View.FilterPopupOpening -= value; }
        }

        /// <summary>
        /// Occurs when the floating object is pasted. 
        /// </summary>
        public event EventHandler<FloatingObjectPastedEventArgs> FloatingObjectPasted
        {
            add { View.FloatingObjectPasted += value; }
            remove { View.FloatingObjectPasted -= value; }
        }

        /// <summary>
        /// Occurs when an invalid operation is performed. 
        /// </summary>
        public event EventHandler<InvalidOperationEventArgs> InvalidOperation
        {
            add { View.InvalidOperation += value; }
            remove { View.InvalidOperation -= value; }
        }

        /// <summary>
        /// Occurs when the user leaves a cell. 
        /// </summary>
        public event EventHandler<LeaveCellEventArgs> LeaveCell
        {
            add { View.LeaveCell += value; }
            remove { View.LeaveCell -= value; }
        }

        /// <summary>
        /// Occurs when the left column changes. 
        /// </summary>
        public event EventHandler<ViewportEventArgs> LeftColumnChanged
        {
            add { View.LeftColumnChanged += value; }
            remove { View.LeftColumnChanged -= value; }
        }

        /// <summary>
        /// Occurs when a column has just been automatically sorted. 
        /// </summary>
        public event EventHandler<RangeFilteredEventArgs> RangeFiltered
        {
            add { View.RangeFiltered += value; }
            remove { View.RangeFiltered -= value; }
        }

        /// <summary>
        /// Occurs when a column is about to be automatically filtered. 
        /// </summary>
        public event EventHandler<RangeFilteringEventArgs> RangeFiltering
        {
            add { View.RangeFiltering += value; }
            remove { View.RangeFiltering -= value; }
        }

        /// <summary>
        /// Occurs after the user has changed the state of the outline (range group) rows or columns. 
        /// </summary>
        public event EventHandler<RangeGroupStateChangedEventArgs> RangeGroupStateChanged
        {
            add { View.RangeGroupStateChanged += value; }
            remove { View.RangeGroupStateChanged -= value; }
        }

        /// <summary>
        /// Occurs before the user changes the state of the outline (range group) rows or columns. 
        /// </summary>
        public event EventHandler<RangeGroupStateChangingEventArgs> RangeGroupStateChanging
        {
            add { View.RangeGroupStateChanging += value; }
            remove { View.RangeGroupStateChanging -= value; }
        }

        /// <summary>
        /// Occurs when a column has just been automatically sorted. 
        /// </summary>
        public event EventHandler<RangeSortedEventArgs> RangeSorted
        {
            add { View.RangeSorted += value; }
            remove { View.RangeSorted -= value; }
        }

        /// <summary>
        /// Occurs when a column is about to be automatically sorted. 
        /// </summary>
        public event EventHandler<RangeSortingEventArgs> RangeSorting
        {
            add { View.RangeSorting += value; }
            remove { View.RangeSorting -= value; }
        }

        /// <summary>
        /// Occurs when the row height has changed. 
        /// </summary>
        public event EventHandler<RowHeightChangedEventArgs> RowHeightChanged
        {
            add { View.RowHeightChanged += value; }
            remove { View.RowHeightChanged -= value; }
        }

        /// <summary>
        /// Occurs when the row height is changing. 
        /// </summary>
        public event EventHandler<RowHeightChangingEventArgs> RowHeightChanging
        {
            add { View.RowHeightChanging += value; }
            remove { View.RowHeightChanging -= value; }
        }

        /// <summary>
        /// Occurs when a viewport row height has changed. 
        /// </summary>
        public event EventHandler<RowViewportHeightChangedEventArgs> RowViewportHeightChanged
        {
            add { View.RowViewportHeightChanged += value; }
            remove { View.RowViewportHeightChanged -= value; }
        }

        /// <summary>
        /// Occurs when a viewport row height is about to be changed. 
        /// </summary>
        public event EventHandler<RowViewportHeightChangingEventArgs> RowViewportHeightChanging
        {
            add { View.RowViewportHeightChanging += value; }
            remove { View.RowViewportHeightChanging -= value; }
        }

        /// <summary>
        /// Occurs when the selection of cells on the sheet is changed. 
        /// </summary>
        public event EventHandler<EventArgs> SelectionChanged
        {
            add { View.SelectionChanged += value; }
            remove { View.SelectionChanged -= value; }
        }

        /// <summary>
        /// Occurs when the selection of cells on the sheet is changing. 
        /// </summary>
        public event EventHandler<SelectionChangingEventArgs> SelectionChanging
        {
            add { View.SelectionChanging += value; }
            remove { View.SelectionChanging -= value; }
        }

        /// <summary>
        /// Occurs when the user clicks the sheet tab. 
        /// </summary>
        public event EventHandler<SheetTabClickEventArgs> SheetTabClick
        {
            add { View.SheetTabClick += value; }
            remove { View.SheetTabClick -= value; }
        }

        /// <summary>
        /// Occurs when the user double-clicks the sheet tab. 
        /// </summary>
        public event EventHandler<SheetTabDoubleClickEventArgs> SheetTabDoubleClick
        {
            add { View.SheetTabDoubleClick += value; }
            remove { View.SheetTabDoubleClick -= value; }
        }

        /// <summary>
        /// Occurs when the top row changes. 
        /// </summary>
        public event EventHandler<ViewportEventArgs> TopRowChanged
        {
            add { View.TopRowChanged += value; }
            remove { View.TopRowChanged -= value; }
        }

        /// <summary>
        /// Occurs before GcSpreadSheet shows the touch strip menu bar. 
        /// </summary>
        public event EventHandler<TouchToolbarOpeningEventArgs> TouchToolbarOpening
        {
            add { View.TouchToolbarOpening += value; }
            remove { View.TouchToolbarOpening -= value; }
        }

        /// <summary>
        /// Occurs when the user types a formula. 
        /// </summary>
        public event EventHandler<UserFormulaEnteredEventArgs> UserFormulaEntered
        {
            add { View.UserFormulaEntered += value; }
            remove { View.UserFormulaEntered -= value; }
        }

        /// <summary>
        /// Occurs when the user zooms. 
        /// </summary>
        public event EventHandler<ZoomEventArgs> UserZooming
        {
            add { View.UserZooming += value; }
            remove { View.UserZooming -= value; }
        }

        /// <summary>
        /// Occurs when the user drags and drops a range of cells. 
        /// </summary>
        public event EventHandler<ValidationDragDropBlockEventArgs> ValidationDragDropBlock
        {
            add { View.ValidationDragDropBlock += value; }
            remove { View.ValidationDragDropBlock -= value; }
        }

        /// <summary>
        /// Occurs when the cell value is invalid. 
        /// </summary>
        public event EventHandler<ValidationErrorEventArgs> ValidationError
        {
            add { View.ValidationError += value; }
            remove { View.ValidationError -= value; }
        }

        /// <summary>
        /// Occurs when a validator is being pasted.
        /// </summary>
        public event EventHandler<ValidationPastingEventArgs> ValidationPasting
        {
            add { View.ValidationPasting += value; }
            remove { View.ValidationPasting -= value; }
        }

        /// <summary>
        /// Occurs when the value in the subeditor changes. 
        /// </summary>
        public event EventHandler<CellEventArgs> ValueChanged
        {
            add { View.ValueChanged += value; }
            remove { View.ValueChanged -= value; }
        }
    }
}
