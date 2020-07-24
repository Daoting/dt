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
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents an undo action to expand or collapse a row range group.
    /// </summary>
    public class RowGroupExpandUndoAction : ActionBase, IUndo
    {
        RowGroupExpandExtent _rowExpandExtent;
        Worksheet _sheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowGroupExpandUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="rowExpandExtent">The row expand extent information.</param>
        public RowGroupExpandUndoAction(Worksheet sheet, RowGroupExpandExtent rowExpandExtent)
        {
            _sheet = sheet;
            _rowExpandExtent = rowExpandExtent;
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed in, this object can be set to null.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((_sheet != null) && (_rowExpandExtent != null)) && (_sheet.RowRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _rowExpandExtent.Index;
                    bool collapsed = _rowExpandExtent.Collapsed;
                    _sheet.RowRangeGroup.Data.SetCollapsed(index, collapsed);
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                SheetView sheetView = sender as SheetView;
                if (sheetView != null)
                {
                    sheetView.InvalidateLayout();
                    sheetView.InvalidateViewportHorizontalArrangement(-2);
                    sheetView.InvalidateHeaderHorizontalArrangement();
                    sheetView.InvalidateMeasure();
                    sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.RowHeader);
                    IList<SpreadChartBase> chartShapeAffectedByRowRangeGroup = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedByRowRangeGroup(sheetView.Worksheet);
                    if (chartShapeAffectedByRowRangeGroup.Count > 0)
                    {
                        sheetView.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) chartShapeAffectedByRowRangeGroup));
                    }
                    ShowRowRangeGroup(sheetView);
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
        }

        void ShowRowRangeGroup(SheetView sheetView)
        {
            int index = _rowExpandExtent.Index;
            if ((index >= 0) && (index < _sheet.RowCount))
            {
                ViewportInfo viewportInfo = sheetView.GetViewportInfo();
                if (sheetView.Worksheet.RowRangeGroup.Direction == RangeGroupDirection.Forward)
                {
                    RangeGroupInfo info2 = _sheet.RowRangeGroup.Find(index - 1, _rowExpandExtent.Level);
                    if (info2 != null)
                    {
                        int start = info2.Start;
                        int bottonRow = index;
                        int viewportIndex = _rowExpandExtent.ViewportIndex;
                        if (_sheet.RowRangeGroup.Data.GetCollapsed(index))
                        {
                            if ((viewportIndex < 0) || (viewportIndex >= viewportInfo.RowViewportCount))
                            {
                                return;
                            }
                            start = index;
                        }
                        else
                        {
                            if (viewportIndex < 0)
                            {
                                return;
                            }
                            if (viewportIndex == 0)
                            {
                                if (start < sheetView.Worksheet.FrozenRowCount)
                                {
                                    start = sheetView.Worksheet.FrozenRowCount;
                                }
                            }
                            else if (viewportIndex >= viewportInfo.RowViewportCount)
                            {
                                if (start >= (sheetView.Worksheet.RowCount - sheetView.Worksheet.FrozenTrailingRowCount))
                                {
                                    return;
                                }
                                start = Math.Max(start, sheetView.Worksheet.FrozenRowCount);
                                bottonRow = (sheetView.Worksheet.RowCount - sheetView.Worksheet.FrozenTrailingRowCount) - 1;
                                viewportIndex--;
                            }
                        }
                        int viewportTopRow = sheetView.GetViewportTopRow(viewportIndex);
                        if (start < viewportTopRow)
                        {
                            viewportTopRow = start;
                        }
                        double viewportHeight = sheetView.GetViewportHeight(viewportIndex);
                        double num7 = NavigatorHelper.GetRowHeight(sheetView.Worksheet, viewportTopRow, bottonRow);
                        if (num7 > viewportHeight)
                        {
                            viewportTopRow = NavigatorHelper.GetNewTopRow(sheetView.Worksheet, viewportTopRow, num7 - viewportHeight);
                        }
                        sheetView.SetViewportTopRow(viewportIndex, viewportTopRow);
                    }
                }
                else if (_sheet.RowRangeGroup.Direction == RangeGroupDirection.Backward)
                {
                    RangeGroupInfo info3 = _sheet.RowRangeGroup.Find(index + 1, _rowExpandExtent.Level);
                    if (info3 != null)
                    {
                        int frozenRowCount = index;
                        int end = info3.End;
                        int rowViewportIndex = _rowExpandExtent.ViewportIndex;
                        if (_sheet.RowRangeGroup.Data.GetCollapsed(index))
                        {
                            if ((rowViewportIndex < 0) || (rowViewportIndex >= viewportInfo.RowViewportCount))
                            {
                                return;
                            }
                            end = index;
                        }
                        else
                        {
                            if (rowViewportIndex >= viewportInfo.RowViewportCount)
                            {
                                return;
                            }
                            if (rowViewportIndex == (viewportInfo.RowViewportCount - 1))
                            {
                                if (end >= (sheetView.Worksheet.RowCount - sheetView.Worksheet.FrozenTrailingRowCount))
                                {
                                    end = (sheetView.Worksheet.RowCount - sheetView.Worksheet.FrozenTrailingRowCount) - 1;
                                }
                            }
                            else if (rowViewportIndex < 0)
                            {
                                if (end < sheetView.Worksheet.FrozenRowCount)
                                {
                                    return;
                                }
                                frozenRowCount = sheetView.Worksheet.FrozenRowCount;
                                end = Math.Min(end, (sheetView.Worksheet.RowCount - sheetView.Worksheet.FrozenTrailingRowCount) - 1);
                                rowViewportIndex++;
                            }
                        }
                        int topRow = sheetView.GetViewportTopRow(rowViewportIndex);
                        if (frozenRowCount < topRow)
                        {
                            sheetView.SetViewportTopRow(rowViewportIndex, frozenRowCount);
                        }
                        else
                        {
                            double num12 = sheetView.GetViewportHeight(rowViewportIndex);
                            double num13 = NavigatorHelper.GetRowHeight(sheetView.Worksheet, topRow, end);
                            if (num13 > num12)
                            {
                                topRow = NavigatorHelper.GetNewTopRow(sheetView.Worksheet, topRow, num13 - num12);
                                sheetView.SetViewportTopRow(rowViewportIndex, Math.Min(frozenRowCount, topRow));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionRowGroupExpand;
        }

        /// <summary>
        /// Undoes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if undo is successful; otherwise <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((_sheet != null) && (_rowExpandExtent != null)) && (_sheet.RowRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _rowExpandExtent.Index;
                    bool collapsed = _rowExpandExtent.Collapsed;
                    _sheet.RowRangeGroup.Data.SetCollapsed(index, !collapsed);
                    flag = true;
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                SheetView sheetView = sender as SheetView;
                if (sheetView == null)
                {
                    return flag;
                }
                sheetView.InvalidateLayout();
                sheetView.InvalidateViewportHorizontalArrangement(-2);
                sheetView.InvalidateHeaderHorizontalArrangement();
                sheetView.InvalidateMeasure();
                sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.RowHeader);
                IList<SpreadChartBase> chartShapeAffectedByRowRangeGroup = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedByRowRangeGroup(sheetView.Worksheet);
                if (chartShapeAffectedByRowRangeGroup.Count > 0)
                {
                    sheetView.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) chartShapeAffectedByRowRangeGroup));
                }
                ShowRowRangeGroup(sheetView);
            }
            return flag;
        }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  true; }
        }
    }
}

