#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        /// <summary>
        /// Adds a cell or cells to the selection.
        /// </summary>
        /// <param name="row">The row index of the first cell to add.</param>
        /// <param name="column">The column index of the first cell to add.</param>
        /// <param name="rowCount">The number of rows to add.</param>
        /// <param name="columnCount">The number of columns to add.</param>
        public void AddSelection(int row, int column, int rowCount, int columnCount)
        {
            ActiveSheet.AddSelection(row, column, rowCount, columnCount);
        }

        /// <summary>
        /// Specifies the last cell in the cell selection. 
        /// </summary>  
        /// <param name="row">The row index of the extended selection.</param>
        /// <param name="column">The column index of the extended selection.</param>
        public void ExtendSelection(int row, int column)
        {
            ActiveSheet.ExtendSelection(row, column);
        }

        /// <summary>
        /// Selects the specified cells.
        /// </summary>
        /// <param name="row">The row index of the first cell.</param>
        /// <param name="column">The column index of the first cell.</param>
        /// <param name="rowCount">The number of rows in the selection.</param>
        /// <param name="columnCount">The number of columns in the selection.</param>
        public void SetSelection(int row, int column, int rowCount, int columnCount)
        {
            if (ActiveSheet.GetActualStyleInfo(row, column, SheetArea.Cells).Focusable)
            {
                ActiveSheet.SetSelection(row, column, rowCount, columnCount);
            }
        }

        public CellRange GetActiveSelection()
        {
            CellRange activeCell = GetActiveCell();
            ReadOnlyCollection<CellRange> selections = ActiveSheet.Selections;
            int num = selections.Count;
            if (num > 0)
            {
                for (int i = num - 1; i >= 0; i--)
                {
                    CellRange range2 = selections[i];
                    if (range2.Contains(activeCell))
                    {
                        return range2;
                    }
                }
            }
            return null;
        }

        void ContinueRowSelecting()
        {
            if ((IsWorking && (IsSelectingRows || IsTouchSelectingRows)) && (MousePosition != _lastClickPoint))
            {
                int activeRowViewportIndex = GetActiveRowViewportIndex();
                RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(activeRowViewportIndex, MousePosition.Y);
                if (viewportRowLayoutNearY != null)
                {
                    CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                    if (InputDeviceType == InputDeviceType.Touch)
                    {
                        IsContinueTouchOperation = true;
                    }
                    ExtendSelection(viewportRowLayoutNearY.Row, -1);
                    RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
                    ProcessScrollTimer();
                }
            }
        }

        void EndRowSelecting()
        {
            IsWorking = false;
            IsSelectingRows = false;
            IsTouchSelectingRows = false;
            StopScrollTimer();
            if (InputDeviceType == InputDeviceType.Touch)
            {
                CellRange activeSelection = GetActiveSelection();
                if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
                {
                    activeSelection = ActiveSheet.Selections[0];
                }
                GetHitInfo();
                int viewportLeftColumn = GetViewportLeftColumn(GetActiveColumnViewportIndex());
                if ((ActiveSheet.ActiveColumnIndex != viewportLeftColumn) || (activeSelection.Row != ActiveSheet.ActiveRowIndex))
                {
                    ActiveSheet.SetActiveCell(activeSelection.Row, viewportLeftColumn, false);
                }
                RefreshSelection();
            }
            if (SavedOldSelections != null)
            {
                if (!IsRangesEqual(SavedOldSelections, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections)))
                {
                    RaiseSelectionChanged();
                }
                SavedOldSelections = null;
            }
        }

        void ContinueColumnSelecting()
        {
            if ((IsWorking && (IsSelectingColumns || IsTouchSelectingColumns)) && (MousePosition != _lastClickPoint))
            {
                int activeColumnViewportIndex = GetActiveColumnViewportIndex();
                ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(activeColumnViewportIndex, MousePosition.X);
                if (viewportColumnLayoutNearX != null)
                {
                    CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                    if (InputDeviceType == InputDeviceType.Touch)
                    {
                        IsContinueTouchOperation = true;
                    }
                    ExtendSelection(-1, viewportColumnLayoutNearX.Column);
                    RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
                    ProcessScrollTimer();
                }
            }
        }

        void EndColumnSelecting()
        {
            IsWorking = false;
            IsTouchSelectingColumns = false;
            IsSelectingColumns = false;
            StopScrollTimer();
            if (InputDeviceType == InputDeviceType.Touch)
            {
                CellRange activeSelection = GetActiveSelection();
                if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
                {
                    activeSelection = ActiveSheet.Selections[0];
                }
                GetHitInfo();
                int viewportTopRow = GetViewportTopRow(GetActiveRowViewportIndex());
                if ((ActiveSheet.ActiveRowIndex != viewportTopRow) || (activeSelection.Column != ActiveSheet.ActiveColumnIndex))
                {
                    ActiveSheet.SetActiveCell(viewportTopRow, activeSelection.Column, false);
                }
                RefreshSelection();
            }
            if (SavedOldSelections != null)
            {
                if (!IsRangesEqual(SavedOldSelections, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections)))
                {
                    RaiseSelectionChanged();
                }
                SavedOldSelections = null;
            }
        }

        void ContinueCellSelecting()
        {
            if ((IsWorking && IsSelectingCells) && (MousePosition != _lastClickPoint))
            {
                int activeColumnViewportIndex = GetActiveColumnViewportIndex();
                int activeRowViewportIndex = GetActiveRowViewportIndex();
                ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(activeColumnViewportIndex, MousePosition.X);
                RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(activeRowViewportIndex, MousePosition.Y);
                CellLayout layout3 = GetViewportCellLayoutModel(activeRowViewportIndex, activeColumnViewportIndex).FindPoint(MousePosition.X, MousePosition.Y);
                CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                if (layout3 != null)
                {
                    ExtendSelection(layout3.Row, layout3.Column);
                }
                else if ((viewportColumnLayoutNearX != null) && (viewportRowLayoutNearY != null))
                {
                    ExtendSelection(viewportRowLayoutNearY.Row, viewportColumnLayoutNearX.Column);
                }
                RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
                ProcessScrollTimer();
            }
        }

        void EndCellSelecting()
        {
            IsWorking = false;
            IsSelectingCells = false;
            StopScrollTimer();
            if (SavedOldSelections != null)
            {
                if (!IsRangesEqual(SavedOldSelections, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections)))
                {
                    RaiseSelectionChanged();
                }
                SavedOldSelections = null;
            }
        }

        void ContinueTouchSelectingCells(Point touchPoint)
        {
            IsContinueTouchOperation = true;
            int activeColumnViewportIndex = GetActiveColumnViewportIndex();
            int activeRowViewportIndex = GetActiveRowViewportIndex();
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(activeColumnViewportIndex, touchPoint.X);
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(activeRowViewportIndex, touchPoint.Y);
            CellLayout layout3 = GetViewportCellLayoutModel(activeRowViewportIndex, activeColumnViewportIndex).FindPoint(touchPoint.X, touchPoint.Y);
            if ((CachedGripperLocation != null) && CachedGripperLocation.TopLeft.Expand(10, 10).Contains(touchPoint))
            {
                CellRange range = ActiveSheet.Selections[0];
                if ((ActiveSheet.ActiveRowIndex != ((range.Row + range.RowCount) - 1)) || (ActiveSheet.ActiveColumnIndex != ((range.Column + range.ColumnCount) - 1)))
                {
                    ActiveSheet.Workbook.SuspendEvent();
                    ActiveSheet.SetActiveCell((range.Row + range.RowCount) - 1, (range.Column + range.ColumnCount) - 1, false);
                    ActiveSheet.Workbook.ResumeEvent();
                }
            }
            CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
            if (layout3 != null)
            {
                ExtendSelection(layout3.Row, layout3.Column);
            }
            else if ((viewportColumnLayoutNearX != null) && (viewportRowLayoutNearY != null))
            {
                ExtendSelection(viewportRowLayoutNearY.Row, viewportColumnLayoutNearX.Column);
            }
            RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
            ProcessScrollTimer();
        }

        void EndTouchSelectingCells()
        {
            IsWorking = false;
            IsTouchSelectingCells = false;
            StopScrollTimer();
            if (SavedOldSelections != null)
            {
                if (!IsRangesEqual(SavedOldSelections, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections)))
                {
                    RaiseSelectionChanged();
                }
                SavedOldSelections = null;
            }
            CellRange activeSelection = GetActiveSelection();
            if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
            {
                activeSelection = ActiveSheet.Selections[0];
            }
            if ((activeSelection != null) && ((ActiveSheet.ActiveColumnIndex != activeSelection.Column) || (ActiveSheet.ActiveRowIndex != activeSelection.Row)))
            {
                ActiveSheet.SetActiveCell(activeSelection.Row, activeSelection.Column, false);
            }
        }

        

        Rect GetActiveSelectionBounds()
        {
            CellRange activeSelection = GetActiveSelection();
            if (activeSelection == null)
            {
                activeSelection = GetActiveCell();
            }
            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
            SheetLayout sheetLayout = GetSheetLayout();
            if (viewportRowsPresenter != null)
            {
                double viewportX = sheetLayout.GetViewportX(GetActiveColumnViewportIndex());
                double viewportY = sheetLayout.GetViewportY(GetActiveRowViewportIndex());
                Rect rangeBounds = viewportRowsPresenter.GetRangeBounds(activeSelection);
                if (!double.IsInfinity(rangeBounds.Width) && !double.IsInfinity(rangeBounds.Height))
                {
                    return new Rect(viewportX + rangeBounds.X, viewportY + rangeBounds.Y, rangeBounds.Width, rangeBounds.Height);
                }
            }
            return Rect.Empty;
        }

        void RefreshSelection()
        {
            if (_cellsPanels != null)
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
                            viewport.RefreshSelection();
                        }
                    }
                }
            }
        }

        void RefreshSelectionBorder()
        {
            if (_cellsPanels != null)
            {
                CellsPanel[,] viewportArray = _cellsPanels;
                int upperBound = viewportArray.GetUpperBound(0);
                int num4 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num4; j++)
                    {
                        CellsPanel viewport = viewportArray[i, j];
                        if (viewport != null)
                        {
                            int rowViewportIndex = viewport.RowViewportIndex;
                            int columnViewportIndex = viewport.ColumnViewportIndex;
                            if (NeedRefresh(rowViewportIndex, columnViewportIndex))
                            {
                                viewport.SelectionContainer.FocusIndicator.IsTopVisible = false;
                                viewport.SelectionContainer.FocusIndicator.IsLeftVisible = false;
                                viewport.SelectionContainer.FocusIndicator.IsRightVisible = false;
                                viewport.SelectionContainer.FocusIndicator.IsBottomVisible = false;
                                if (IsVerticalDragFill)
                                {
                                    if (_currentFillDirection == DragFillDirection.Down)
                                    {
                                        if (rowViewportIndex == _dragFillStartBottomRowViewport)
                                        {
                                            viewport.SelectionContainer.FocusIndicator.IsBottomVisible = true;
                                        }
                                    }
                                    else if (_currentFillDirection == DragFillDirection.Up)
                                    {
                                        if (rowViewportIndex == _dragFillStartTopRowViewport)
                                        {
                                            viewport.SelectionContainer.FocusIndicator.IsTopVisible = true;
                                        }
                                    }
                                    else if (_currentFillDirection == DragFillDirection.UpClear)
                                    {
                                    }
                                }
                                else if (_currentFillDirection == DragFillDirection.Right)
                                {
                                    if (columnViewportIndex == _dragFillStartRightColumnViewport)
                                    {
                                        viewport.SelectionContainer.FocusIndicator.IsRightVisible = true;
                                    }
                                }
                                else if (_currentFillDirection == DragFillDirection.Left)
                                {
                                    if (columnViewportIndex == _dragFillStartLeftColumnViewport)
                                    {
                                        viewport.SelectionContainer.FocusIndicator.IsLeftVisible = true;
                                    }
                                }
                                viewport.SelectionContainer.FocusIndicator.InvalidateMeasure();
                                viewport.SelectionContainer.FocusIndicator.InvalidateArrange();
                            }
                        }
                    }
                }
            }
        }

        void ResetSelectionFrameStroke()
        {
            if (_resetSelectionFrameStroke)
            {
                ViewportInfo viewportInfo = GetViewportInfo();
                int rowViewportCount = viewportInfo.RowViewportCount;
                int columnViewportCount = viewportInfo.ColumnViewportCount;
                for (int i = -1; i <= rowViewportCount; i++)
                {
                    for (int j = -1; j <= columnViewportCount; j++)
                    {
                        CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(i, j);
                        if (viewportRowsPresenter != null)
                        {
                            viewportRowsPresenter.SelectionContainer.ResetSelectionFrameStroke();
                        }
                    }
                }
            }
            _resetSelectionFrameStroke = false;
        }

        void StartCellSelecting()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (savedHitTestInformation.ViewportInfo != null)
            {
                int row = savedHitTestInformation.ViewportInfo.Row;
                int column = savedHitTestInformation.ViewportInfo.Column;
                int rowCount = 1;
                int columnCount = 1;
                if ((savedHitTestInformation.ViewportInfo.Row > -1) && (savedHitTestInformation.ViewportInfo.Column > -1))
                {
                    bool flag;
                    bool flag2;
                    CellLayout layout = GetViewportCellLayoutModel(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.ColumnViewportIndex).FindCell(savedHitTestInformation.ViewportInfo.Row, savedHitTestInformation.ViewportInfo.Column);
                    KeyboardHelper.GetMetaKeyState(out flag2, out flag);
                    if (layout != null)
                    {
                        row = layout.Row;
                        column = layout.Column;
                        rowCount = layout.RowCount;
                        columnCount = layout.ColumnCount;
                    }
                    if (ActiveSheet.GetActualStyleInfo(row, column, SheetArea.Cells).Focusable)
                    {
                        IsWorking = true;
                        if (PreviewLeaveCell(row, column))
                        {
                            IsWorking = false;
                        }
                        else
                        {
                            IsSelectingCells = true;
                            IsTouchSelectingCells = false;
                            SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
                            SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
                            CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                            SavedOldSelections = oldSelection;
                            if (!flag2)
                            {
                                SetActiveCellInternal(row, column, false);
                            }
                            if (flag)
                            {
                                AddSelection(row, column, 1, 1);
                            }
                            else if (flag2)
                            {
                                ExtendSelection((row + rowCount) - 1, (column + columnCount) - 1);
                            }
                            else
                            {
                                ActiveSheet.SetSelection(row, column, 1, 1);
                            }
                            RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
                            if (!IsWorking || !IsSelectingCells)
                            {
                                EndCellSelecting();
                            }
                            StartScrollTimer();
                        }
                    }
                }
            }
        }

        void StartColumnSelecting()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitTestType == HitTestType.Empty) || (savedHitTestInformation.HeaderInfo == null))
            {
                savedHitTestInformation = HitTest(_touchStartPoint.X, _touchStartPoint.Y);
            }
            if (savedHitTestInformation.HeaderInfo != null)
            {
                SheetLayout sheetLayout = GetSheetLayout();
                int viewportTopRow = GetViewportTopRow((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                int column = savedHitTestInformation.HeaderInfo.Column;
                Cell canSelectedCellInColumn = GetCanSelectedCellInColumn(viewportTopRow, column);
                if (canSelectedCellInColumn != null)
                {
                    viewportTopRow = canSelectedCellInColumn.Row.Index;
                    IsWorking = true;
                    if (PreviewLeaveCell(viewportTopRow, column))
                    {
                        IsWorking = false;
                    }
                    else
                    {
                        if (IsTouching)
                        {
                            IsTouchSelectingColumns = true;
                        }
                        else
                        {
                            IsSelectingColumns = true;
                        }
                        SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
                        SetActiveRowViewportIndex((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                        if (savedHitTestInformation.HeaderInfo.Column > -1)
                        {
                            CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                            if (InputDeviceType != InputDeviceType.Touch)
                            {
                                bool flag2;
                                bool flag3;
                                KeyboardHelper.GetMetaKeyState(out flag3, out flag2);
                                SavedOldSelections = oldSelection;
                                if (!flag3)
                                {
                                    SetActiveCellInternal(viewportTopRow, column, false);
                                }
                                if (flag2)
                                {
                                    AddSelection(-1, savedHitTestInformation.HeaderInfo.Column, -1, 1);
                                }
                                else if (flag3)
                                {
                                    ExtendSelection(-1, savedHitTestInformation.HeaderInfo.Column);
                                }
                                else
                                {
                                    ActiveSheet.SetSelection(-1, savedHitTestInformation.HeaderInfo.Column, -1, 1);
                                }
                                if (!flag3)
                                {
                                    ActiveSheet.SetActiveCell(viewportTopRow, column, false);
                                }
                            }
                            else
                            {
                                if ((ActiveSheet.SelectionPolicy == SelectionPolicy.MultiRange) && CanTouchMultiSelect)
                                {
                                    ActiveSheet.AddSelection(-1, savedHitTestInformation.HeaderInfo.Column, -1, 1);
                                }
                                else
                                {
                                    ActiveSheet.SetSelection(-1, savedHitTestInformation.HeaderInfo.Column, -1, 1);
                                }
                                ActiveSheet.SetActiveCell(viewportTopRow, column, false);
                            }
                            RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
                            if (!IsWorking || (!IsSelectingColumns && !IsTouchSelectingColumns))
                            {
                                EndColumnSelecting();
                            }
                            StartScrollTimer();
                        }
                    }
                }
            }
        }

        void StartRowsSelecting()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitTestType == HitTestType.Empty) || (savedHitTestInformation.HeaderInfo == null))
            {
                savedHitTestInformation = HitTest(_touchStartPoint.X, _touchStartPoint.Y);
            }
            if (savedHitTestInformation.HeaderInfo != null)
            {
                SheetLayout sheetLayout = GetSheetLayout();
                int row = savedHitTestInformation.HeaderInfo.Row;
                int viewportLeftColumn = GetViewportLeftColumn((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                Cell canSelectedCellInRow = GetCanSelectedCellInRow(row, viewportLeftColumn);
                if (canSelectedCellInRow != null)
                {
                    viewportLeftColumn = canSelectedCellInRow.Column.Index;
                    IsWorking = true;
                    if (PreviewLeaveCell(row, viewportLeftColumn))
                    {
                        IsWorking = false;
                    }
                    else
                    {
                        if (!IsTouching)
                        {
                            IsSelectingRows = true;
                        }
                        else
                        {
                            IsTouchSelectingRows = true;
                        }
                        SetActiveColumnViewportIndex((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                        SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
                        if (savedHitTestInformation.HeaderInfo.Row > -1)
                        {
                            CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                            SavedOldSelections = oldSelection;
                            if (InputDeviceType != InputDeviceType.Touch)
                            {
                                bool flag2;
                                bool flag3;
                                KeyboardHelper.GetMetaKeyState(out flag2, out flag3);
                                if (!flag2)
                                {
                                    SetActiveCellInternal(row, viewportLeftColumn, false);
                                }
                                if (flag3)
                                {
                                    AddSelection(savedHitTestInformation.HeaderInfo.Row, -1, 1, -1);
                                }
                                else if (flag2)
                                {
                                    ExtendSelection(savedHitTestInformation.HeaderInfo.Row, -1);
                                }
                                else
                                {
                                    ActiveSheet.SetSelection(savedHitTestInformation.HeaderInfo.Row, -1, 1, -1);
                                }
                                if (!flag2)
                                {
                                    ActiveSheet.SetActiveCell(row, viewportLeftColumn, false);
                                }
                            }
                            else
                            {
                                if ((ActiveSheet.SelectionPolicy == SelectionPolicy.MultiRange) && CanTouchMultiSelect)
                                {
                                    ActiveSheet.AddSelection(savedHitTestInformation.HeaderInfo.Row, -1, 1, -1);
                                }
                                else
                                {
                                    ActiveSheet.SetSelection(savedHitTestInformation.HeaderInfo.Row, -1, 1, -1);
                                }
                                ActiveSheet.SetActiveCell(savedHitTestInformation.HeaderInfo.Row, viewportLeftColumn, false);
                            }
                            RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
                            if (!IsWorking || (!IsSelectingRows && !IsTouchSelectingRows))
                            {
                                EndRowSelecting();
                            }
                            StartScrollTimer();
                        }
                    }
                }
            }
        }

        void StartSheetSelecting()
        {
            SheetLayout sheetLayout = GetSheetLayout();
            int viewportTopRow = GetViewportTopRow((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
            int viewportLeftColumn = GetViewportLeftColumn((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
            Cell cell = GetCanSelectedCell(viewportTopRow, viewportLeftColumn, (viewportTopRow < 0) ? -1 : (ActiveSheet.RowCount - viewportTopRow), (viewportLeftColumn < 0) ? -1 : (ActiveSheet.ColumnCount - viewportLeftColumn));
            if (cell != null)
            {
                viewportTopRow = cell.Row.Index;
                viewportLeftColumn = cell.Column.Index;
                if (((ActiveSheet.ColumnCount <= 0) || (ActiveSheet.RowCount <= 0)) || !PreviewLeaveCell(viewportTopRow, viewportLeftColumn))
                {
                    SetActiveColumnViewportIndex((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                    SetActiveRowViewportIndex((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                    CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                    if ((ActiveSheet.ColumnCount > 0) && (ActiveSheet.RowCount > 0))
                    {
                        SetActiveCellInternal(viewportTopRow, viewportLeftColumn, true);
                    }
                    ActiveSheet.ClearSelections();
                    ActiveSheet.AddSelection(-1, -1, -1, -1, false);
                    if (RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections)))
                    {
                        RaiseSelectionChanged();
                    }
                }
            }
        }

        void StartTapSelectCells()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            int row = savedHitTestInformation.ViewportInfo.Row;
            int column = savedHitTestInformation.ViewportInfo.Column;
            CloseTouchToolbar();
            if ((savedHitTestInformation.ViewportInfo.Row > -1) && (savedHitTestInformation.ViewportInfo.Column > -1))
            {
                CellLayout layout = GetViewportCellLayoutModel(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.ColumnViewportIndex).FindCell(savedHitTestInformation.ViewportInfo.Row, savedHitTestInformation.ViewportInfo.Column);
                if (layout != null)
                {
                    row = layout.Row;
                    column = layout.Column;
                    int rowCount = layout.RowCount;
                    int columnCount = layout.ColumnCount;
                }
                if (ActiveSheet.GetActualStyleInfo(row, column, SheetArea.Cells).Focusable)
                {
                    IsWorking = true;
                    if (PreviewLeaveCell(row, column))
                    {
                        IsWorking = false;
                    }
                    else
                    {
                        IsSelectingCells = false;
                        IsTouchSelectingCells = true;
                        SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
                        SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
                        SetActiveCellInternal(row, column, false);
                        CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
                        SavedOldSelections = oldSelection;
                        if ((ActiveSheet.SelectionPolicy == SelectionPolicy.MultiRange) && CanTouchMultiSelect)
                        {
                            ActiveSheet.AddSelection(row, column, 1, 1);
                        }
                        else
                        {
                            ActiveSheet.SetSelection(row, column, 1, 1);
                            RefreshSelection();
                        }
                        RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections));
                        if (!IsWorking)
                        {
                            EndCellSelecting();
                        }
                    }
                }
            }
        }

        void StartTouchingSelecting()
        {
            if (!IsEntrieSheetSelection() && IsEntrieRowSelection())
            {
                IsTouchSelectingRows = true;
            }
            else if (!IsEntrieSheetSelection() && IsEntrieColumnSelection())
            {
                IsTouchSelectingColumns = true;
            }
            else
            {
                IsTouchSelectingCells = true;
            }
            CloseTouchToolbar();
            CellRange[] rangeArray = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>)ActiveSheet.Selections);
            SavedOldSelections = rangeArray;
            IsWorking = true;
            StartScrollTimer();
        }

        void UpdateSelectState(ChartChangedBaseEventArgs e)
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
                            viewport.RefreshFloatingObjectContainerIsSelected();
                        }
                        else
                        {
                            viewport.RefreshFloatingObjectContainerIsSelected(e.Chart);
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

        internal void UpdateTouchSelectionGripper()
        {
            Rect? autoFillIndicatorRec;
            if (((InputDeviceType != InputDeviceType.Touch) || IsTouchPromotedMouseMessage))
            {
                Rect rect16 = new Rect(0.0, 0.0, 0.0, 0.0);
                _gripperLocations = null;
                ResizerGripperRect = null;
                _topLeftGripper.Arrange(rect16);
                _bottomRightGripper.Arrange(rect16);
                _resizerGripperContainer.Arrange(rect16);
                autoFillIndicatorRec = _autoFillIndicatorRec;
                if (autoFillIndicatorRec.HasValue)
                {
                    _autoFillIndicatorContainer.Arrange(rect16);
                    _autoFillIndicatorRec = null;
                }
                if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
                {
                    _touchToolbarPopup.IsOpen = false;
                }
                return;
            }
            Rect rect = new Rect(0.0, 0.0, 0.0, 0.0);
            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
            if (viewportRowsPresenter == null)
            {
                return;
            }
            if ((IsContinueTouchOperation || IsEditing) || (ActiveSheet.SelectionPolicy == SelectionPolicy.Single))
            {
                if (_gripperLocations != null)
                {
                    CachedGripperLocation = _gripperLocations;
                }
                _gripperLocations = null;
                _topLeftGripper.Arrange(rect);
                _bottomRightGripper.Arrange(rect);
                _resizerGripperContainer.Arrange(rect);
                return;
            }
            FloatingObject[] allSelectedFloatingObjects = GetAllSelectedFloatingObjects();
            if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Length > 0))
            {
                if (_gripperLocations != null)
                {
                    CachedGripperLocation = _gripperLocations;
                }
                _gripperLocations = null;
                _topLeftGripper.Arrange(rect);
                _bottomRightGripper.Arrange(rect);
                _resizerGripperContainer.Arrange(rect);
                return;
            }
            CellRange activeSelection = GetActiveSelection();
            if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
            {
                activeSelection = ActiveSheet.Selections[0];
            }
            if (activeSelection == null)
            {
                if (_gripperLocations != null)
                {
                    CachedGripperLocation = _gripperLocations;
                }
                _gripperLocations = null;
                _topLeftGripper.Arrange(rect);
                _bottomRightGripper.Arrange(rect);
                _resizerGripperContainer.Arrange(rect);
                return;
            }
            autoFillIndicatorRec = _autoFillIndicatorRec;
            if (autoFillIndicatorRec.HasValue)
            {
                if (_gripperLocations != null)
                {
                    CachedGripperLocation = _gripperLocations;
                }
                _gripperLocations = null;
                _topLeftGripper.Arrange(rect);
                _bottomRightGripper.Arrange(rect);
                _resizerGripperContainer.Arrange(rect);
                Rect autoFillIndicatorRect = GetAutoFillIndicatorRect(viewportRowsPresenter, activeSelection);
                _autoFillIndicatorContainer.Arrange(autoFillIndicatorRect);
                _autoFillIndicatorRec = new Rect?(autoFillIndicatorRect);
                return;
            }
            if (viewportRowsPresenter.Excel.ActiveSheet.Selections.Count <= 0)
            {
                return;
            }
            SheetLayout sheetLayout = GetSheetLayout();
            Rect rangeBounds = viewportRowsPresenter._cachedSelectionFrameLayout;
            if (!viewportRowsPresenter.SelectionContainer.IsAnchorCellInSelection)
            {
                rangeBounds = viewportRowsPresenter._cachedFocusCellLayout;
            }
            if (viewportRowsPresenter.Excel.ActiveSheet.Selections.Count > 0)
            {
                rangeBounds = viewportRowsPresenter.GetRangeBounds(activeSelection);
            }
            List<Tuple<Point, double>> list = new List<Tuple<Point, double>>();
            if (IsEntrieSheetSelection())
            {
                _gripperLocations = null;
                _topLeftGripper.Arrange(rect);
                _bottomRightGripper.Arrange(rect);
                _resizerGripperContainer.Arrange(rect);
                autoFillIndicatorRec = null;
                ResizerGripperRect = autoFillIndicatorRec;
            }
            else
            {
                double viewportY;
                bool flag2;
                if (!IsEntrieColumnSelection())
                {
                    if (!IsEntrieRowSelection())
                    {
                        double num27 = sheetLayout.GetViewportX(viewportRowsPresenter.ColumnViewportIndex);
                        double num28 = sheetLayout.GetViewportY(viewportRowsPresenter.RowViewportIndex);
                        int num29 = GetActiveRowViewportIndex();
                        int activeColumnViewportIndex = GetActiveColumnViewportIndex();
                        int viewportLeftColumn = GetViewportLeftColumn(activeColumnViewportIndex);
                        int num32 = GetViewportTopRow(num29);
                        int num33 = GetViewportBottomRow(num29);
                        int viewportRightColumn = GetViewportRightColumn(activeColumnViewportIndex);
                        int num35 = -7;
                        int num36 = -7;
                        if ((activeSelection.Column < viewportLeftColumn) || (activeSelection.Row < num32))
                        {
                            list.Add(Tuple.Create<Point, double>(new Point(-2147483648.0, -2147483648.0), 0.0));
                        }
                        else
                        {
                            list.Add(Tuple.Create<Point, double>(new Point((num27 + rangeBounds.X) + num35, (num28 + rangeBounds.Y) + num36), 16.0));
                        }
                        num35 = (int)(rangeBounds.Width - 9.0);
                        num36 = (int)(rangeBounds.Height - 9.0);
                        int num37 = (activeSelection.Row + activeSelection.RowCount) - 1;
                        int num38 = (activeSelection.Column + activeSelection.ColumnCount) - 1;
                        if (num37 > num33)
                        {
                            num36 = 0x7fffffff;
                        }
                        if (num38 > viewportRightColumn)
                        {
                            num35 = 0x7fffffff;
                        }
                        int num39 = GetActiveRowViewportIndex();
                        int num40 = GetActiveColumnViewportIndex();
                        ActiveSheet.GetViewportInfo();
                        if ((num35 == 0x7fffffff) || (num36 == 0x7fffffff))
                        {
                            for (int i = num39; i <= GetViewportInfo(ActiveSheet).RowViewportCount; i++)
                            {
                                for (int j = num40; j <= GetViewportInfo(ActiveSheet).ColumnViewportCount; j++)
                                {
                                    num33 = GetViewportBottomRow(i);
                                    viewportRightColumn = GetViewportRightColumn(j);
                                    if ((num33 >= num37) && (viewportRightColumn >= num38))
                                    {
                                        CellsPanel viewport8 = _cellsPanels[i + 1, j + 1];
                                        if (viewport8 != null)
                                        {
                                            Rect rect13 = viewport8._cachedSelectionFrameLayout;
                                            if (!viewport8.SelectionContainer.IsAnchorCellInSelection)
                                            {
                                                rect13 = viewport8._cachedFocusCellLayout;
                                            }
                                            num35 = (int)(((sheetLayout.GetViewportX(j) + rect13.X) + rect13.Width) - 9.0);
                                            num36 = (int)(((sheetLayout.GetViewportY(i) + rect13.Y) + rect13.Height) - 9.0);
                                            if (list.Count == 1)
                                            {
                                                if ((num35 > (sheetLayout.GetViewportX(j) + sheetLayout.GetViewportWidth(j))) || (num36 > (sheetLayout.GetViewportY(i) + sheetLayout.GetViewportHeight(i))))
                                                {
                                                    list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, 2147483647.0), 0.0));
                                                }
                                                else
                                                {
                                                    list.Add(Tuple.Create<Point, double>(new Point((double)num35, (double)num36), 16.0));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (list.Count == 1)
                            {
                                list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, 2147483647.0), 0.0));
                            }
                        }
                        else
                        {
                            num35 = (int)((num35 + num27) + rangeBounds.X);
                            num36 = (int)((num36 + num28) + rangeBounds.Y);
                            if ((num35 > (sheetLayout.GetViewportX(activeColumnViewportIndex) + sheetLayout.GetViewportWidth(activeColumnViewportIndex))) || (num36 > (sheetLayout.GetViewportY(num29) + sheetLayout.GetViewportHeight(num29))))
                            {
                                list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, 2147483647.0), 0.0));
                            }
                            else
                            {
                                list.Add(Tuple.Create<Point, double>(new Point((double)num35, (double)num36), 16.0));
                            }
                        }
                        goto Label_10BF;
                    }
                    double viewportX = sheetLayout.GetViewportX(viewportRowsPresenter.ColumnViewportIndex);
                    viewportY = sheetLayout.GetViewportY(viewportRowsPresenter.RowViewportIndex);
                    int viewportTopRow = GetViewportTopRow(viewportRowsPresenter.RowViewportIndex);
                    int viewportBottomRow = GetViewportBottomRow(viewportRowsPresenter.RowViewportIndex);
                    if (ActiveSheet.FrozenColumnCount > 0)
                    {
                        CellsPanel viewport5 = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex() + 1);
                        Rect rect9 = viewport5._cachedSelectionFrameLayout;
                        if (!viewport5.SelectionContainer.IsAnchorCellInSelection)
                        {
                            rect9 = viewportRowsPresenter._cachedFocusCellLayout;
                        }
                        rangeBounds = new Rect(rangeBounds.X, rangeBounds.Y, rangeBounds.Width + rect9.Width, rangeBounds.Height);
                    }
                    if (activeSelection.Row >= viewportTopRow)
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, (viewportY + rangeBounds.Y) - 16.0), 16.0));
                    }
                    else
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, -2147483648.0), 0.0));
                    }
                    int num18 = (int)(rangeBounds.Height - 9.0);
                    int num19 = (activeSelection.Row + activeSelection.RowCount) - 1;
                    if (num19 > viewportBottomRow)
                    {
                        num18 = 0x7fffffff;
                    }
                    int activeRowViewportIndex = GetActiveRowViewportIndex();
                    ActiveSheet.GetViewportInfo();
                    flag2 = true;
                    int rowViewportIndex = activeRowViewportIndex;
                    if (num18 == 0x7fffffff)
                    {
                        while (rowViewportIndex <= GetViewportInfo(ActiveSheet).RowViewportCount)
                        {
                            if (GetViewportBottomRow(rowViewportIndex) >= num19)
                            {
                                CellsPanel viewport6 = _cellsPanels[rowViewportIndex + 1, viewportRowsPresenter.ColumnViewportIndex + 1];
                                if (viewport6 != null)
                                {
                                    Rect rect10 = viewport6._cachedSelectionFrameLayout;
                                    if (!viewport6.SelectionContainer.IsAnchorCellInSelection)
                                    {
                                        rect10 = viewport6._cachedFocusCellLayout;
                                    }
                                    num18 = (int)((sheetLayout.GetViewportY(rowViewportIndex) + rect10.Y) + rect10.Height);
                                    if (list.Count == 1)
                                    {
                                        if (num18 <= (sheetLayout.GetViewportY(rowViewportIndex) + sheetLayout.GetViewportHeight(rowViewportIndex)))
                                        {
                                            list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, (double)num18), 16.0));
                                        }
                                        else
                                        {
                                            list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, 2147483647.0), 0.0));
                                            flag2 = false;
                                        }
                                        break;
                                    }
                                }
                            }
                            rowViewportIndex++;
                        }
                    }
                    else
                    {
                        double viewportHeight = sheetLayout.GetViewportHeight(viewportRowsPresenter.RowViewportIndex);
                        double y = (viewportY + rangeBounds.Y) + rangeBounds.Height;
                        if (y <= (viewportY + viewportHeight))
                        {
                            list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, y), 16.0));
                        }
                        else
                        {
                            list.Add(Tuple.Create<Point, double>(new Point(viewportX, 2147483647.0), 0.0));
                            flag2 = false;
                        }
                    }
                }
                else
                {
                    double num = sheetLayout.GetViewportX(viewportRowsPresenter.ColumnViewportIndex);
                    double num2 = sheetLayout.GetViewportY(viewportRowsPresenter.RowViewportIndex);
                    int num3 = GetViewportLeftColumn(viewportRowsPresenter.ColumnViewportIndex);
                    int num4 = GetViewportRightColumn(viewportRowsPresenter.ColumnViewportIndex);
                    if (ActiveSheet.FrozenRowCount > 0)
                    {
                        CellsPanel viewport2 = GetViewportRowsPresenter(GetActiveRowViewportIndex() + 1, GetActiveColumnViewportIndex());
                        Rect rect5 = viewport2._cachedSelectionFrameLayout;
                        if (!viewport2.SelectionContainer.IsAnchorCellInSelection)
                        {
                            rect5 = viewport2._cachedFocusCellLayout;
                        }
                        rangeBounds = new Rect(rangeBounds.X, rangeBounds.Y, rangeBounds.Width, rangeBounds.Height + rect5.Height);
                    }
                    if (activeSelection.Column >= num3)
                    {
                        list.Add(Tuple.Create<Point, double>(new Point((num + rangeBounds.X) - 16.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 16.0));
                    }
                    else
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(-2147483648.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 0.0));
                    }
                    int num5 = (int)(rangeBounds.Width - 9.0);
                    int num6 = (activeSelection.Column + activeSelection.ColumnCount) - 1;
                    if (num6 > num4)
                    {
                        num5 = 0x7fffffff;
                    }
                    int num7 = GetActiveColumnViewportIndex();
                    ActiveSheet.GetViewportInfo();
                    bool flag = true;
                    int columnViewportIndex = num7;
                    if (num5 == 0x7fffffff)
                    {
                        while (columnViewportIndex <= GetViewportInfo(ActiveSheet).ColumnViewportCount)
                        {
                            if (GetViewportRightColumn(columnViewportIndex) >= num6)
                            {
                                CellsPanel viewport3 = _cellsPanels[viewportRowsPresenter.RowViewportIndex + 1, columnViewportIndex + 1];
                                if (viewport3 != null)
                                {
                                    Rect rect6 = viewport3._cachedSelectionFrameLayout;
                                    if (!viewport3.SelectionContainer.IsAnchorCellInSelection)
                                    {
                                        rect6 = viewport3._cachedFocusCellLayout;
                                    }
                                    num5 = (int)((sheetLayout.GetViewportX(columnViewportIndex) + rect6.X) + rect6.Width);
                                    if (list.Count == 1)
                                    {
                                        if (num5 <= (sheetLayout.GetViewportX(columnViewportIndex) + sheetLayout.GetViewportWidth(columnViewportIndex)))
                                        {
                                            list.Add(Tuple.Create<Point, double>(new Point((double)num5, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 16.0));
                                        }
                                        else
                                        {
                                            list.Add(Tuple.Create<Point, double>(new Point(-2147483648.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 0.0));
                                            flag = false;
                                        }
                                        break;
                                    }
                                }
                            }
                            columnViewportIndex++;
                        }
                    }
                    else
                    {
                        double viewportWidth = sheetLayout.GetViewportWidth(viewportRowsPresenter.ColumnViewportIndex);
                        double x = (num + rangeBounds.X) + rangeBounds.Width;
                        if (x <= (num + viewportWidth))
                        {
                            list.Add(Tuple.Create<Point, double>(new Point(x, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 16.0));
                        }
                        else
                        {
                            list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 0.0));
                            flag = false;
                        }
                    }
                    var colHeader = _colHeaders[viewportRowsPresenter.ColumnViewportIndex + 1];
                    CellRange range2 = new CellRange(ActiveSheet.ColumnHeader.RowCount - 1, (activeSelection.Column + activeSelection.ColumnCount) - 1, 1, 1);
                    Rect rect7 = colHeader.GetRangeBounds(range2, SheetArea.ColumnHeader);
                    int column = (activeSelection.Column + activeSelection.ColumnCount) - 1;
                    if ((ActiveSheet.GetColumnResizable(column) && !rect7.IsEmpty) && flag)
                    {
                        double num12 = 0.0;
                        for (int k = 0; k < ActiveSheet.ColumnHeader.RowCount; k++)
                        {
                            num12 += ActiveSheet.GetActualRowHeight(k, SheetArea.ColumnHeader) * ActiveSheet.ZoomFactor;
                        }
                        Rect rect8 = new Rect(((num + rect7.X) + rect7.Width) - 8.0, (colHeader.Location.Y + num12) - 16.0, 16.0, 16.0);
                        _resizerGripperContainer.Child = _cachedColumnResizerGripperImage;
                        _resizerGripperContainer.Arrange(rect8);
                        ResizerGripperRect = new Rect?(rect8);
                    }
                    else
                    {
                        _resizerGripperContainer.Arrange(rect);
                        autoFillIndicatorRec = null;
                        ResizerGripperRect = autoFillIndicatorRec;
                    }
                    goto Label_10BF;
                }

                var header = _rowHeaders[viewportRowsPresenter.RowViewportIndex + 1];
                CellRange range = new CellRange((activeSelection.Row + activeSelection.RowCount) - 1, ActiveSheet.RowHeader.ColumnCount - 1, 1, 1);
                Rect rect11 = header.GetRangeBounds(range, SheetArea.CornerHeader | SheetArea.RowHeader);
                int row = (activeSelection.Row + activeSelection.RowCount) - 1;
                if ((ActiveSheet.GetRowResizable(row) && !rect11.IsEmpty) && flag2)
                {
                    double num25 = 0.0;
                    for (int m = 0; m < ActiveSheet.RowHeader.ColumnCount; m++)
                    {
                        num25 += ActiveSheet.GetActualColumnWidth(m, SheetArea.CornerHeader | SheetArea.RowHeader) * ActiveSheet.ZoomFactor;
                    }
                    Rect rect12 = new Rect((header.Location.X + num25) - 16.0, ((viewportY + rect11.Y) + rect11.Height) - 8.0, 16.0, 16.0);
                    _resizerGripperContainer.Child = _cachedRowResizerGripperImage;
                    _resizerGripperContainer.Arrange(rect12);
                    ResizerGripperRect = new Rect?(rect12);
                }
                else
                {
                    _resizerGripperContainer.Arrange(rect);
                    autoFillIndicatorRec = null;
                    ResizerGripperRect = autoFillIndicatorRec;
                }
            }
        Label_10BF:
            if (list.Count == 2)
            {
                Point point = list[0].Item1;
                double width = list[0].Item2;
                Rect rect14 = new Rect((double)((int)point.X), (double)((int)point.Y), width, width);
                _topLeftGripper.Arrange(rect14);
                point = list[1].Item1;
                width = list[1].Item2;
                Rect rect15 = new Rect((double)((int)point.X), (double)((int)point.Y), width, width);
                _bottomRightGripper.Arrange(rect15);
                GripperLocationsStruct struct2 = new GripperLocationsStruct
                {
                    TopLeft = rect14,
                    BottomRight = rect15
                };
                _gripperLocations = struct2;
                CachedGripperLocation = _gripperLocations;
                if (IsEntrieSheetSelection() || (!IsEntrieRowSelection() && !IsEntrieColumnSelection()))
                {
                    _resizerGripperContainer.Arrange(rect);
                    ResizerGripperRect = null;
                    return;
                }
            }
            else
            {
                _gripperLocations = null;
                _topLeftGripper.Arrange(rect);
                _bottomRightGripper.Arrange(rect);
                _resizerGripperContainer.Arrange(rect);
                ResizerGripperRect = null;
            }
        }

        bool IsEntrieColumnSelection()
        {
            if (ActiveSheet.Selections.Count != 1)
            {
                return false;
            }
            CellRange range = ActiveSheet.Selections[0];
            return ((range.Row == -1) && (range.RowCount == -1));
        }

        bool IsEntrieRowSelection()
        {
            if (ActiveSheet.Selections.Count != 1)
            {
                return false;
            }
            CellRange range = ActiveSheet.Selections[0];
            return ((range.Column == -1) && (range.ColumnCount == -1));
        }

        bool IsEntrieSheetSelection()
        {
            return (IsEntrieRowSelection() && IsEntrieColumnSelection());
        }

        bool IsInSelectionGripper(Point point)
        {
            if (CachedGripperLocation == null)
            {
                return false;
            }
            Rect topLeft = CachedGripperLocation.TopLeft;
            if (CachedGripperLocation.TopLeft.Expand(10, 10).Contains(point))
            {
                return true;
            }
            Rect bottomRight = CachedGripperLocation.BottomRight;
            return CachedGripperLocation.BottomRight.Expand(10, 10).Contains(point);
        }

        void StartScrollTimer()
        {
            if (IsWorking)
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                if (((savedHitTestInformation.HitTestType == HitTestType.Viewport) || (savedHitTestInformation.HitTestType == HitTestType.RowHeader)) || ((savedHitTestInformation.HitTestType == HitTestType.FloatingObject) || (savedHitTestInformation.HitTestType == HitTestType.FormulaSelection)))
                {
                    double viewportHeight = GetViewportHeight(savedHitTestInformation.RowViewportIndex);
                    _verticalSelectionMgr = new ScrollSelectionManager(0.0, viewportHeight, new Action<bool>(OnVerticalSelectionTick));
                }
                if (((savedHitTestInformation.HitTestType == HitTestType.Viewport) || (savedHitTestInformation.HitTestType == HitTestType.ColumnHeader)) || ((savedHitTestInformation.HitTestType == HitTestType.FloatingObject) || (savedHitTestInformation.HitTestType == HitTestType.FormulaSelection)))
                {
                    double viewportWidth = GetViewportWidth(savedHitTestInformation.ColumnViewportIndex);
                    _horizontalSelectionMgr = new ScrollSelectionManager(0.0, viewportWidth, new Action<bool>(OnHorizontalSelectionTick));
                }
            }
        }

        void StopScrollTimer()
        {
            if (_verticalSelectionMgr != null)
            {
                _verticalSelectionMgr.Dispose();
                _verticalSelectionMgr = null;
            }
            if (_horizontalSelectionMgr != null)
            {
                _horizontalSelectionMgr.Dispose();
                _horizontalSelectionMgr = null;
            }
        }

        void ProcessScrollTimer()
        {
            if (IsWorking)
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                SheetLayout sheetLayout = GetSheetLayout();
                ViewportInfo viewportInfo = GetViewportInfo();
                double viewportX = sheetLayout.GetViewportX(savedHitTestInformation.ColumnViewportIndex);
                double viewportY = sheetLayout.GetViewportY(savedHitTestInformation.RowViewportIndex);
                if (_verticalSelectionMgr != null)
                {
                    int rowViewportCount = viewportInfo.RowViewportCount;
                    if (savedHitTestInformation.RowViewportIndex == -1)
                    {
                        RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(0);
                        if ((viewportRowLayoutModel != null) && (viewportRowLayoutModel.Count > 0))
                        {
                            RowLayout layout2 = viewportRowLayoutModel[0];
                            if (((MousePosition.Y >= sheetLayout.GetViewportY(0)) && ((_verticalSelectionMgr.MousePosition + viewportY) < sheetLayout.GetViewportY(0))) && (layout2.Row > ActiveSheet.FrozenRowCount))
                            {
                                SetViewportTopRow(0, ActiveSheet.FrozenRowCount);
                            }
                        }
                    }
                    else if (savedHitTestInformation.RowViewportIndex == rowViewportCount)
                    {
                        RowLayoutModel model2 = GetViewportRowLayoutModel(rowViewportCount - 1);
                        if ((model2 != null) && (model2.Count > 0))
                        {
                            RowLayout layout3 = model2[model2.Count - 1];
                            if (((MousePosition.Y < sheetLayout.GetViewportY(rowViewportCount)) && ((_verticalSelectionMgr.MousePosition + viewportY) >= sheetLayout.GetViewportY(rowViewportCount))) && ((layout3.Y + layout3.Height) > sheetLayout.GetViewportY(rowViewportCount)))
                            {
                                double viewportHeight = sheetLayout.GetViewportHeight(rowViewportCount - 1);
                                double num5 = 0.0;
                                int num6 = (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1;
                                for (int i = num6; i >= ActiveSheet.FrozenRowCount; i--)
                                {
                                    num5 += ActiveSheet.GetActualRowHeight(i, SheetArea.Cells) * ActiveSheet.ZoomFactor;
                                    if (num5 > viewportHeight)
                                    {
                                        num6 = Math.Min(i + 1, num6);
                                        break;
                                    }
                                }
                                SetViewportTopRow(rowViewportCount - 1, num6);
                            }
                        }
                    }
                    _verticalSelectionMgr.MousePosition = MousePosition.Y - viewportY;
                }
                if (_horizontalSelectionMgr != null)
                {
                    int columnViewportCount = viewportInfo.ColumnViewportCount;
                    if (savedHitTestInformation.ColumnViewportIndex == -1)
                    {
                        ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(0);
                        if ((viewportColumnLayoutModel != null) && (viewportColumnLayoutModel.Count > 0))
                        {
                            ColumnLayout layout4 = viewportColumnLayoutModel[0];
                            if (((MousePosition.X >= sheetLayout.GetViewportX(0)) && ((_horizontalSelectionMgr.MousePosition + viewportX) < sheetLayout.GetViewportX(0))) && (layout4.Column > ActiveSheet.FrozenColumnCount))
                            {
                                SetViewportLeftColumn(0, ActiveSheet.FrozenColumnCount);
                            }
                        }
                    }
                    else if (savedHitTestInformation.ColumnViewportIndex == columnViewportCount)
                    {
                        ColumnLayoutModel model4 = GetViewportColumnLayoutModel(columnViewportCount - 1);
                        if ((model4 != null) && (model4.Count > 0))
                        {
                            ColumnLayout layout5 = model4[model4.Count - 1];
                            if (((MousePosition.X < sheetLayout.GetViewportX(columnViewportCount)) && ((_horizontalSelectionMgr.MousePosition + viewportX) >= sheetLayout.GetViewportX(columnViewportCount))) && ((layout5.X + layout5.Width) > sheetLayout.GetViewportX(columnViewportCount)))
                            {
                                double viewportWidth = sheetLayout.GetViewportWidth(columnViewportCount - 1);
                                double num10 = 0.0;
                                int num11 = (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1;
                                for (int j = num11; j >= ActiveSheet.FrozenColumnCount; j--)
                                {
                                    num10 += ActiveSheet.GetActualColumnWidth(j, SheetArea.Cells) * ActiveSheet.ZoomFactor;
                                    if (num10 > viewportWidth)
                                    {
                                        num11 = Math.Min(j + 1, num11);
                                        break;
                                    }
                                }
                                SetViewportLeftColumn(columnViewportCount - 1, num11);
                            }
                        }
                    }
                    _horizontalSelectionMgr.MousePosition = MousePosition.X - viewportX;
                }
            }
        }

        void OnHorizontalSelectionTick(bool needIncrease)
        {
            if (HorizontalScrollable)
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                int viewportLeftColumn = GetViewportLeftColumn(savedHitTestInformation.ColumnViewportIndex);
                int viewportRightColumn = GetViewportRightColumn(savedHitTestInformation.ColumnViewportIndex);
                Worksheet worksheet = ActiveSheet;
                if (needIncrease)
                {
                    if (viewportRightColumn < ((worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount) - 1))
                    {
                        SetViewportLeftColumn(savedHitTestInformation.ColumnViewportIndex, viewportLeftColumn + 1);
                        base.InvalidateMeasure();
                    }
                    else
                    {
                        ColumnLayout layout = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>)GetViewportColumnLayoutModel(savedHitTestInformation.ColumnViewportIndex));
                        if (layout != null)
                        {
                            SheetLayout sheetLayout = GetSheetLayout();
                            double num3 = sheetLayout.GetViewportX(savedHitTestInformation.ColumnViewportIndex) + sheetLayout.GetViewportWidth(savedHitTestInformation.ColumnViewportIndex);
                            if ((layout.X + layout.Width) >= num3)
                            {
                                SetViewportLeftColumn(savedHitTestInformation.ColumnViewportIndex, viewportLeftColumn + 1);
                                base.InvalidateMeasure();
                            }
                        }
                    }
                }
                else if ((viewportLeftColumn - 1) >= 0)
                {
                    SetViewportLeftColumn(savedHitTestInformation.ColumnViewportIndex, viewportLeftColumn - 1);
                    base.InvalidateMeasure();
                }
                if (IsSelectingCells)
                {
                    ContinueCellSelecting();
                }
                if (IsSelectingColumns)
                {
                    ContinueColumnSelecting();
                }
                if (IsTouchSelectingCells)
                {
                    ContinueTouchSelectingCells(MousePosition);
                }
                if (IsTouchSelectingColumns)
                {
                    ContinueColumnSelecting();
                }
                if (IsTouchDragFilling)
                {
                    ContinueTouchDragFill();
                }
                if (IsTouchDrapDropping)
                {
                    ContinueTouchDragDropping();
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
            }
        }

        void OnVerticalSelectionTick(bool needIncrease)
        {
            if (VerticalScrollable)
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                int viewportTopRow = GetViewportTopRow(savedHitTestInformation.RowViewportIndex);
                int viewportBottomRow = GetViewportBottomRow(savedHitTestInformation.RowViewportIndex);
                Worksheet worksheet = ActiveSheet;
                if (needIncrease)
                {
                    if (viewportBottomRow < ((ActiveSheet.RowCount - worksheet.FrozenTrailingRowCount) - 1))
                    {
                        SetViewportTopRow(savedHitTestInformation.RowViewportIndex, viewportTopRow + 1);
                        base.InvalidateMeasure();
                    }
                    else
                    {
                        RowLayout layout = Enumerable.Last<RowLayout>((IEnumerable<RowLayout>)GetViewportRowLayoutModel(savedHitTestInformation.RowViewportIndex));
                        if (layout != null)
                        {
                            SheetLayout sheetLayout = GetSheetLayout();
                            double num3 = sheetLayout.GetViewportY(savedHitTestInformation.RowViewportIndex) + sheetLayout.GetViewportHeight(savedHitTestInformation.RowViewportIndex);
                            if ((layout.Y + layout.Height) >= num3)
                            {
                                SetViewportTopRow(savedHitTestInformation.RowViewportIndex, viewportTopRow + 1);
                                base.InvalidateMeasure();
                            }
                        }
                    }
                }
                else if ((viewportTopRow - 1) >= 0)
                {
                    SetViewportTopRow(savedHitTestInformation.RowViewportIndex, viewportTopRow - 1);
                    base.InvalidateMeasure();
                }
                if (IsSelectingCells)
                {
                    ContinueCellSelecting();
                }
                if (IsSelectingRows)
                {
                    ContinueRowSelecting();
                }
                if (IsTouchSelectingCells)
                {
                    ContinueTouchSelectingCells(MousePosition);
                }
                if (IsTouchSelectingRows)
                {
                    ContinueRowSelecting();
                }
                if (IsTouchDragFilling)
                {
                    ContinueTouchDragFill();
                }
                if (IsDragDropping)
                {
                    ContinueDragDropping();
                }
                if (IsTouchDrapDropping)
                {
                    ContinueTouchDragDropping();
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
            }
        }

        internal void RaiseSelectionChanged()
        {
            if ((SelectionChanged != null) && (_eventSuspended == 0))
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        internal bool RaiseSelectionChanging(CellRange[] oldSelection, CellRange[] newSelection)
        {
            if (((SelectionChanging != null) && (_eventSuspended == 0)) && !IsRangesEqual(oldSelection, newSelection))
            {
                SelectionChanging(this, new SelectionChangingEventArgs(oldSelection, newSelection));
                return true;
            }
            return false;
        }
    }
}

