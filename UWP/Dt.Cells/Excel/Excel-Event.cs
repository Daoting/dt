#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-24 创建
******************************************************************************/
#endregion

#region 名称空间
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.UI.Core;
#endregion

namespace Dt.Base
{
    public partial class Excel
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
        public event EventHandler<UserErrorEventArgs> Error;

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
        public event EventHandler<TouchToolbarOpeningEventArgs> TouchToolbarOpening;

        /// <summary>
        /// Occurs when the user types a formula.
        /// </summary>
        public event EventHandler<UserFormulaEnteredEventArgs> UserFormulaEntered;

        /// <summary>
        /// Occurs when the user zooms.
        /// </summary>
        public event EventHandler<ZoomEventArgs> UserZooming;

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

        /// <summary>
        /// 报表项开始拖放事件
        /// </summary>
        public event EventHandler ItemStartDrag;

        /// <summary>
        /// 报表项拖放结束事件
        /// </summary>
        public event EventHandler<CellEventArgs> ItemDropped;

        void OnColumnHeaderSpanModelChanged(object sender, SheetSpanModelChangedEventArgs e)
        {
            OnSpanModelChanged(e.Row, e.Column, e.RowCount, e.ColumnCount, SheetArea.ColumnHeader);
        }

        void OnPictureChanged(object sender, PictureChangedEventArgs e)
        {
            HandleFloatingObjectChanged(e.Picture, e.Property, AutoRefresh);
        }

        void OnRowHeaderSpanModelChanged(object sender, SheetSpanModelChangedEventArgs e)
        {
            OnSpanModelChanged(e.Row, e.Column, e.RowCount, e.ColumnCount, SheetArea.CornerHeader | SheetArea.RowHeader);
        }

        async void OnSelectionChanged(object sender, SheetSelectionChangedEventArgs e)
        {
            //hdt
            if (!this.AreHandlersSuspended())
            {
                if (Dispatcher.HasThreadAccess)
                {
                    HandleSheetSelectionChanged(sender, e);
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        HandleSheetSelectionChanged(sender, e);
                    });
                }
            }
        }

        void HandleSheetSelectionChanged(object sender, SheetSelectionChangedEventArgs e)
        {
            RefreshSelection();
            UpdateHeaderCellsState(e.Row, e.RowCount, e.Column, e.ColumnCount);
            Navigation.UpdateStartPosition(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
        }

        void OnSheetCellChanged(object sender, CellChangedEventArgs e)
        {
            if (AutoRefresh && sender == ActiveSheet)
            {
                switch (e.SheetArea)
                {
                    case SheetArea.CornerHeader:
                    case (SheetArea.Cells | SheetArea.RowHeader):
                        return;

                    case SheetArea.Cells:
                        if (e.PropertyName != "Formula")
                        {
                            if (e.PropertyName == "Axis")
                            {
                                InvalidateLayout();
                            }
                            RefreshRange(e.Row, e.Column, e.RowCount, e.ColumnCount, e.SheetArea);
                            return;
                        }
                        RefreshRange(-1, -1, -1, -1, SheetArea.Cells);
                        return;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    case SheetArea.ColumnHeader:
                        if (e.PropertyName == "Axis")
                        {
                            InvalidateLayout();
                        }
                        RefreshRange(e.Row, e.Column, e.RowCount, e.ColumnCount, e.SheetArea);
                        return;
                }
            }
        }

        void OnSheetChartChanged(object sender, ChartChangedEventArgs e)
        {
            if (_cellsPanels == null)
                return;

            if (e.Property == "IsSelected")
            {
                UpdateSelectState(e);
            }
            else if (AutoRefresh)
            {
                if (e.Chart == null)
                {
                    InvalidateFloatingObjectLayout();
                }
                else if (((e.ChartArea == ChartArea.AxisX) || (e.ChartArea == ChartArea.AxisY)) || (e.ChartArea == ChartArea.AxisZ))
                {
                    CellsPanel[,] viewportArray = _cellsPanels;
                    int upperBound = viewportArray.GetUpperBound(0);
                    int num2 = viewportArray.GetUpperBound(1);
                    for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                    {
                        for (int j = viewportArray.GetLowerBound(1); j <= num2; j++)
                        {
                            CellsPanel viewport = viewportArray[i, j];
                            if (viewport != null)
                            {
                                if (e.Chart == null)
                                {
                                    viewport.RefreshFloatingObjects();
                                }
                                else
                                {
                                    viewport.RefreshFloatingObject(e);
                                }
                            }
                        }
                    }
                }
                else
                {
                    bool displayHidden = false;
                    if (e.Chart is SpreadChart)
                    {
                        displayHidden = (e.Chart as SpreadChart).DisplayHidden;
                    }
                    if (((e.Property == "Location") || (e.Property == "Size")) || (((e.Property == "SheetRowChanged") || (e.Property == "SheetColumnChanged")) || (e.Property == "Name")))
                    {
                        CellsPanel[,] viewportArray2 = _cellsPanels;
                        int num5 = viewportArray2.GetUpperBound(0);
                        int num6 = viewportArray2.GetUpperBound(1);
                        for (int k = viewportArray2.GetLowerBound(0); k <= num5; k++)
                        {
                            for (int m = viewportArray2.GetLowerBound(1); m <= num6; m++)
                            {
                                if (viewportArray2[k, m] != null)
                                {
                                    InvalidateFloatingObjectLayout();
                                }
                            }
                        }
                    }
                    else if ((((e.Property == "RowFilter") || (e.Property == "RowRangeGroup")) || ((e.Property == "ColumnRangeGroup") || (e.Property == "TableFilter"))) || (((e.Property == "AxisX") || (e.Property == "AxisY")) || (e.Property == "AxisZ")))
                    {
                        CellsPanel[,] viewportArray3 = _cellsPanels;
                        int num9 = viewportArray3.GetUpperBound(0);
                        int num10 = viewportArray3.GetUpperBound(1);
                        for (int n = viewportArray3.GetLowerBound(0); n <= num9; n++)
                        {
                            for (int num12 = viewportArray3.GetLowerBound(1); num12 <= num10; num12++)
                            {
                                CellsPanel viewport3 = viewportArray3[n, num12];
                                if (viewport3 != null)
                                {
                                    viewport3.InvalidateFloatingObjectMeasureState(e.Chart);
                                    if (e.Chart == null)
                                    {
                                        viewport3.InvalidateFloatingObjectsMeasureState();
                                        foreach (SpreadChart chart in ActiveSheet.Charts)
                                        {
                                            if (!displayHidden)
                                            {
                                                viewport3.RefreshFloatingObject(e);
                                            }
                                        }
                                    }
                                    if ((e.Chart != null) && !displayHidden)
                                    {
                                        viewport3.RefreshFloatingObject(e);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        CellsPanel[,] viewportArray4 = _cellsPanels;
                        int num13 = viewportArray4.GetUpperBound(0);
                        int num14 = viewportArray4.GetUpperBound(1);
                        for (int num15 = viewportArray4.GetLowerBound(0); num15 <= num13; num15++)
                        {
                            for (int num16 = viewportArray4.GetLowerBound(1); num16 <= num14; num16++)
                            {
                                CellsPanel viewport4 = viewportArray4[num15, num16];
                                if (viewport4 != null)
                                {
                                    viewport4.InvalidateFloatingObjectsMeasureState();
                                    if (e.Chart == null)
                                    {
                                        foreach (SpreadChart chart in ActiveSheet.Charts)
                                        {
                                            viewport4.RefreshFloatingObject(e);
                                        }
                                    }
                                    if (e.Chart != null)
                                    {
                                        viewport4.RefreshFloatingObject(e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void OnSheetColumnChanged(object sender, SheetChangedEventArgs e)
        {
            AutoRefreshRowColumn(sender, e);
        }

        void OnSheetRowChanged(object sender, SheetChangedEventArgs e)
        {
            AutoRefreshRowColumn(sender, e);
        }

        void AutoRefreshRowColumn(object sender, SheetChangedEventArgs e)
        {
            if ((sender == ActiveSheet) && AutoRefresh)
            {
                if (((e.PropertyName == "Height") || (e.PropertyName == "Width")) || ((e.PropertyName == "IsVisible") || (e.PropertyName == "Axis")))
                {
                    RefreshAll();
                }
                else
                {
                    RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                }
            }
        }

        void OnSheetColumnHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AutoRefresh && sender == ActiveSheet.ColumnHeader)
            {
                switch (e.PropertyName)
                {
                    case "DefaultStyle":
                    case "AutoText":
                    case "AutoTextIndex":
                    case "IsVisible":
                    case "RowCount":
                        RefreshAll();
                        return;

                    case "DefaultRowHeight":
                        RefreshRows(0, ActiveSheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);
                        return;
                }
            }
        }

        void OnSheetFloatingObjectChanged(object sender, FloatingObjectChangedEventArgs e)
        {
            HandleFloatingObjectChanged(e.FloatingObject, e.Property, AutoRefresh);
        }

        void HandleFloatingObjectChanged(FloatingObject floatingObject, string property, bool autoRefresh)
        {
            if (_cellsPanels == null)
                return;

            if (floatingObject == null)
            {
                InvalidateFloatingObjectLayout();
            }
            else if (property == "IsSelected")
            {
                CellsPanel[,] viewportArray = _cellsPanels;
                int upperBound = viewportArray.GetUpperBound(0);
                int num2 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num2; j++)
                    {
                        CellsPanel viewport = viewportArray[i, j];
                        if (viewport != null)
                        {
                            if (floatingObject == null)
                            {
                                viewport.RefreshFloatingObjectContainerIsSelected();
                            }
                            else
                            {
                                viewport.RefreshFloatingObjectContainerIsSelected(floatingObject);
                            }
                        }
                    }
                }
                ReadOnlyCollection<CellRange> selections = ActiveSheet.Selections;
                if (selections.Count != 0)
                {
                    foreach (CellRange range in selections)
                    {
                        UpdateHeaderCellsState(range.Row, range.RowCount, range.Column, range.ColumnCount);
                    }
                }
            }
            else if (autoRefresh)
            {
                if ((((property == "Location") || (property == "Size")) || ((property == "SheetRowChanged") || (property == "SheetColumnChanged"))) || ((((property == "AxisX") || (property == "AxisY")) || ((property == "RowFilter") || (property == "RowRangeGroup"))) || ((property == "ColumnRangeGroup") || (property == "Name"))))
                {
                    CellsPanel[,] viewportArray2 = _cellsPanels;
                    int num5 = viewportArray2.GetUpperBound(0);
                    int num6 = viewportArray2.GetUpperBound(1);
                    for (int k = viewportArray2.GetLowerBound(0); k <= num5; k++)
                    {
                        for (int m = viewportArray2.GetLowerBound(1); m <= num6; m++)
                        {
                            CellsPanel viewport1 = viewportArray2[k, m];
                            InvalidateFloatingObjectLayout();
                        }
                    }
                }
                else
                {
                    CellsPanel[,] viewportArray3 = _cellsPanels;
                    int num9 = viewportArray3.GetUpperBound(0);
                    int num10 = viewportArray3.GetUpperBound(1);
                    for (int n = viewportArray3.GetLowerBound(0); n <= num9; n++)
                    {
                        for (int num12 = viewportArray3.GetLowerBound(1); num12 <= num10; num12++)
                        {
                            CellsPanel viewport2 = viewportArray3[n, num12];
                            if (viewport2 != null)
                            {
                                viewport2.InvalidateFloatingObjectMeasureState(floatingObject);
                                viewport2.RefreshFloatingObject(new FloatingObjectChangedEventArgs(floatingObject, null));
                            }
                        }
                    }
                }
            }
        }

        void OnSheetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (AutoRefresh && !IsSuspendInvalidate())
                {
                    RefreshTabStrip();
                }
            }
            else
            {
                HandleSheetPropertyChanged(sender, e, AutoRefresh);
            }
        }

        void HandleSheetPropertyChanged(object sender, PropertyChangedEventArgs e, bool autoRefresh)
        {
            if (ActiveSheet != null)
            {
                if (e.PropertyName == "Visible")
                {
                    Worksheet sheet = sender as Worksheet;
                    if (sheet != null)
                    {
                        HandleVisibleChanged(sheet);
                        if (autoRefresh)
                        {
                            RefreshAll();
                        }
                    }
                }
                if ((e.PropertyName == "SheetTabColor") || (e.PropertyName == "SheetTabThemeColor"))
                {
                    UpdateTabStrip();
                }
                if (sender == ActiveSheet)
                {
                    switch (e.PropertyName)
                    {
                        case "ActiveCell":
                        case "ActiveColumnIndex":
                        case "ActiveRowIndex":
                            Navigation.UpdateStartPosition(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
                            UpdateHeaderCellsStateInSpanArea();
                            UpdateFocusIndicator();
                            UpdateHeaderCellsStateInSpanArea();
                            PrepareCellEditing();
                            return;

                        case "FrozenRowCount":
                            SetViewportTopRow(0, ActiveSheet.FrozenRowCount);
                            if (autoRefresh)
                            {
                                RefreshRows(0, ActiveSheet.FrozenRowCount, SheetArea.Cells | SheetArea.RowHeader);
                            }
                            return;

                        case "FrozenColumnCount":
                            SetViewportLeftColumn(0, ActiveSheet.FrozenColumnCount);
                            if (autoRefresh)
                            {
                                RefreshColumns(0, ActiveSheet.FrozenColumnCount, SheetArea.Cells | SheetArea.ColumnHeader);
                            }
                            return;

                        case "FrozenTrailingRowCount":
                            if (autoRefresh)
                            {
                                RefreshRows(Math.Max(0, ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount), ActiveSheet.FrozenTrailingRowCount, SheetArea.Cells | SheetArea.RowHeader);
                            }
                            return;

                        case "FrozenTrailingColumnCount":
                            if (autoRefresh)
                            {
                                RefreshRows(Math.Max(0, ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount), ActiveSheet.FrozenTrailingColumnCount, SheetArea.Cells | SheetArea.ColumnHeader);
                            }
                            return;

                        case "RowFilter":
                            if (_cachedFilterButtonInfoModel != null)
                            {
                                _cachedFilterButtonInfoModel.Clear();
                                _cachedFilterButtonInfoModel = null;
                            }
                            if (autoRefresh)
                            {
                                RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            }
                            return;

                        case "ShowGridLine":
                        case "GridLineColor":
                        case "ZoomFactor":
                        case "DefaultColumnWidth":
                        case "DefaultRowHeight":
                        case "NamedStyles":
                        case "DefaultStyle":
                        case "[Sort]":
                        case "[MoveTo]":
                        case "[CopyTo]":
                        case "SelectionBorderColor":
                        case "SelectionBorderThemeColor":
                        case "SelectionBackground":
                            if (autoRefresh)
                            {
                                RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            }
                            return;

                        case "DataSource":
                            if (autoRefresh)
                            {
                                RefreshAll();
                            }
                            return;

                        case "[ViewportInfo]":
                            return;

                        case "RowCount":
                        case "RowRangeGroup":
                            if (autoRefresh)
                            {
                                RefreshRows(0, ActiveSheet.RowCount, SheetArea.Cells | SheetArea.RowHeader);
                            }
                            return;

                        case "ColumnCount":
                        case "ColumnRangeGroup":
                            if (autoRefresh)
                            {
                                RefreshColumns(0, ActiveSheet.ColumnCount, SheetArea.Cells | SheetArea.ColumnHeader);
                            }
                            return;

                        case "StartingRowNumber":
                        case "RowHeaderColumnCount":
                            if (autoRefresh)
                            {
                                RefreshColumns(0, ActiveSheet.RowHeader.ColumnCount, SheetArea.CornerHeader | SheetArea.RowHeader);
                            }
                            return;

                        case "StartingColumnNumber":
                        case "ColumnHeaderRowCount":
                            if (autoRefresh)
                            {
                                RefreshRows(0, ActiveSheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);
                            }
                            return;

                        case "RowHeaderDefaultStyle":
                            if (autoRefresh)
                            {
                                RefreshRange(-1, -1, -1, -1, SheetArea.CornerHeader | SheetArea.RowHeader);
                            }
                            return;

                        case "ColumnHeaderDefaultStyle":
                            if (autoRefresh)
                            {
                                RefreshRange(-1, -1, -1, -1, SheetArea.ColumnHeader);
                            }
                            return;

                        case "ReferenceStyle":
                        case "Names":
                            if (autoRefresh)
                            {
                                RefreshRange(-1, -1, -1, -1, SheetArea.Cells);
                            }
                            return;

                        case "[ImportFile]":
                            if (autoRefresh)
                            {
                                RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            }
                            HideProgressRing();
                            return;

                        case "[OpenXml]":
                            RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            return;

                        case "Charts":
                        case "SurfaceCharts":
                        case "FloatingObjects":
                        case "Pictures":
                            if (autoRefresh)
                            {
                                InvalidateFloatingObjectLayout();
                            }
                            return;
                    }
                }
            }
        }

        void OnSheetRowHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AutoRefresh && sender == ActiveSheet.RowHeader)
            {
                switch (e.PropertyName)
                {
                    case "DefaultStyle":
                    case "AutoText":
                    case "AutoTextIndex":
                    case "IsVisible":
                    case "ColumnCount":
                        RefreshAll();
                        return;

                    case "DefaultColumnWidth":
                        RefreshColumns(0, ActiveSheet.RowHeader.ColumnCount, SheetArea.CornerHeader | SheetArea.RowHeader);
                        return;
                }
            }
        }

        void OnSheetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable enumerable = (e.NewItems == null) ? ((IEnumerable)new Worksheet[0]) : ((IEnumerable)e.NewItems);
            IEnumerable enumerable2 = (e.OldItems == null) ? ((IEnumerable)new Worksheet[0]) : ((IEnumerable)e.OldItems);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    foreach (Worksheet worksheet in enumerable2)
                    {
                        DetachSheet(worksheet);
                    }
                    foreach (Worksheet worksheet2 in enumerable)
                    {
                        AttachSheet(worksheet2);
                    }
                    if ((SpreadXClipboard.Worksheet != null) && ((Workbook.Sheets == null) || !Workbook.Sheets.Contains(SpreadXClipboard.Worksheet)))
                    {
                        ClipboardHelper.ClearClipboard();
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    return;
            }
        }

        void OnSpanModelChanged(object sender, SheetSpanModelChangedEventArgs e)
        {
            OnSpanModelChanged(e.Row, e.Column, e.RowCount, e.ColumnCount, SheetArea.Cells);
        }

        void OnSpanModelChanged(int row, int column, int rowCount, int columnCount, SheetArea area)
        {
            if (AutoRefresh)
            {
                RefreshRange(row, column, rowCount, columnCount, area);
            }
        }

        void OnWorkbookPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AutoRefresh)
            {
                switch (e.PropertyName)
                {
                    case "Sheets":
                    case "ActiveSheetIndex":
                    case "ActiveSheet":
                    case "[OpenExcel]":
                    case "[DataCalculated]":
                    case "[OpenXml]":
                    case "AutoRefresh":
                        RefreshAll();
                        return;

                    case "StartSheetIndex":
                        ProcessStartSheetIndexChanged();
                        return;

                    case "CurrentThemeName":
                    case "CurrentTheme":
                        RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                        RefreshFloatingObjects();
                        return;

                    case "HorizontalScrollBarVisibility":
                    case "VerticalScrollBarVisibility":
                    case "ReferenceStyle":
                    case "Names":
                    case "CanCellOverflow":
                        RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                        return;
                }
            }

            if (e.PropertyName == "ActiveSheetIndex")
            {
                RefreshAll();
            }
        }

        /// <summary>
        /// 触发报表项开始拖放事件
        /// </summary>
        internal void OnItemStartDrag()
        {
            if (ItemStartDrag != null)
                ItemStartDrag(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发报表项拖放结束事件
        /// </summary>
        /// <param name="p_args"></param>
        internal void OnItemDropped(CellEventArgs p_args)
        {
            if (ItemDropped != null)
                ItemDropped(this, p_args);
        }

    }
}
