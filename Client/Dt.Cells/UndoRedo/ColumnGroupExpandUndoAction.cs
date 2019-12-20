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
    /// Represents an undo action to expand or collapse a column range group.
    /// </summary>
    public class ColumnGroupExpandUndoAction : ActionBase, IUndo
    {
        private ColumnGroupExpandExtent _columnExpandExtent;
        private Worksheet _sheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnGroupExpandUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="columnExpandExtent">The column expand extent information.</param>
        public ColumnGroupExpandUndoAction(Worksheet sheet, ColumnGroupExpandExtent columnExpandExtent)
        {
            this._sheet = sheet;
            this._columnExpandExtent = columnExpandExtent;
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
        /// Executes the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((this._sheet != null) && (this._columnExpandExtent != null)) && (this._sheet.ColumnRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = this._columnExpandExtent.Index;
                    bool collapsed = this._columnExpandExtent.Collapsed;
                    this._sheet.ColumnRangeGroup.Data.SetCollapsed(index, collapsed);
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
                    sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                    IList<SpreadChartBase> chartShapeAffectedByColumnRangeGroup = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedByColumnRangeGroup(sheetView.Worksheet);
                    if (chartShapeAffectedByColumnRangeGroup.Count > 0)
                    {
                        sheetView.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) chartShapeAffectedByColumnRangeGroup));
                    }
                    this.ShowColumnRangeGroup(sheetView);
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
        }

        private void ShowColumnRangeGroup(SheetView sheetView)
        {
            int index = this._columnExpandExtent.Index;
            if ((index >= 0) && (index < this._sheet.ColumnCount))
            {
                ViewportInfo viewportInfo = sheetView.GetViewportInfo();
                if (sheetView.Worksheet.ColumnRangeGroup.Direction == RangeGroupDirection.Forward)
                {
                    RangeGroupInfo info2 = this._sheet.ColumnRangeGroup.Find(index - 1, this._columnExpandExtent.Level);
                    if (info2 != null)
                    {
                        int start = info2.Start;
                        int rightColumn = index;
                        int viewportIndex = this._columnExpandExtent.ViewportIndex;
                        if (this._sheet.ColumnRangeGroup.Data.GetCollapsed(index))
                        {
                            if ((viewportIndex < 0) || (viewportIndex >= viewportInfo.ColumnViewportCount))
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
                                if (start < sheetView.Worksheet.FrozenColumnCount)
                                {
                                    start = sheetView.Worksheet.FrozenColumnCount;
                                }
                            }
                            else if (viewportIndex >= viewportInfo.ColumnViewportCount)
                            {
                                if (start >= (sheetView.Worksheet.ColumnCount - sheetView.Worksheet.FrozenTrailingColumnCount))
                                {
                                    return;
                                }
                                start = Math.Max(start, sheetView.Worksheet.FrozenColumnCount);
                                rightColumn = (sheetView.Worksheet.ColumnCount - sheetView.Worksheet.FrozenTrailingColumnCount) - 1;
                                viewportIndex--;
                            }
                        }
                        int viewportLeftColumn = sheetView.GetViewportLeftColumn(viewportIndex);
                        if (start < viewportLeftColumn)
                        {
                            viewportLeftColumn = start;
                        }
                        double viewportWidth = sheetView.GetViewportWidth(viewportIndex);
                        double num7 = NavigatorHelper.GetColumnWidth(sheetView.Worksheet, viewportLeftColumn, rightColumn);
                        if (num7 > viewportWidth)
                        {
                            viewportLeftColumn = NavigatorHelper.GetNewLeftColumn(sheetView.Worksheet, viewportLeftColumn, num7 - viewportWidth);
                        }
                        sheetView.SetViewportLeftColumn(viewportIndex, viewportLeftColumn);
                    }
                }
                else if (this._sheet.ColumnRangeGroup.Direction == RangeGroupDirection.Backward)
                {
                    RangeGroupInfo info3 = this._sheet.ColumnRangeGroup.Find(index + 1, this._columnExpandExtent.Level);
                    if (info3 != null)
                    {
                        int frozenColumnCount = index;
                        int end = info3.End;
                        int columnViewportIndex = this._columnExpandExtent.ViewportIndex;
                        if (this._sheet.ColumnRangeGroup.Data.GetCollapsed(index))
                        {
                            if ((columnViewportIndex < 0) || (columnViewportIndex >= viewportInfo.ColumnViewportCount))
                            {
                                return;
                            }
                            end = index;
                        }
                        else
                        {
                            if (columnViewportIndex >= viewportInfo.ColumnViewportCount)
                            {
                                return;
                            }
                            if (columnViewportIndex == (viewportInfo.ColumnViewportCount - 1))
                            {
                                if (end >= (sheetView.Worksheet.ColumnCount - sheetView.Worksheet.FrozenTrailingColumnCount))
                                {
                                    end = (sheetView.Worksheet.ColumnCount - sheetView.Worksheet.FrozenTrailingColumnCount) - 1;
                                }
                            }
                            else if (columnViewportIndex < 0)
                            {
                                if (end < sheetView.Worksheet.FrozenColumnCount)
                                {
                                    return;
                                }
                                frozenColumnCount = sheetView.Worksheet.FrozenColumnCount;
                                end = Math.Min(end, (sheetView.Worksheet.ColumnCount - sheetView.Worksheet.FrozenTrailingColumnCount) - 1);
                                columnViewportIndex++;
                            }
                        }
                        int leftColumn = sheetView.GetViewportLeftColumn(columnViewportIndex);
                        if (frozenColumnCount < leftColumn)
                        {
                            sheetView.SetViewportLeftColumn(columnViewportIndex, frozenColumnCount);
                        }
                        else
                        {
                            double num12 = sheetView.GetViewportWidth(columnViewportIndex);
                            double num13 = NavigatorHelper.GetColumnWidth(sheetView.Worksheet, leftColumn, end);
                            if (num13 > num12)
                            {
                                leftColumn = NavigatorHelper.GetNewLeftColumn(sheetView.Worksheet, leftColumn, num13 - num12);
                                sheetView.SetViewportLeftColumn(columnViewportIndex, Math.Min(frozenColumnCount, leftColumn));
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
            return ResourceStrings.undoActionColumnGroupExpand;
        }

        /// <summary>
        /// Undoes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if the undo is successful; otherwise, <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((this._sheet != null) && (this._columnExpandExtent != null)) && (this._sheet.ColumnRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = this._columnExpandExtent.Index;
                    bool collapsed = this._columnExpandExtent.Collapsed;
                    this._sheet.ColumnRangeGroup.Data.SetCollapsed(index, !collapsed);
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
                sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                IList<SpreadChartBase> chartShapeAffectedByColumnRangeGroup = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedByColumnRangeGroup(sheetView.Worksheet);
                if (chartShapeAffectedByColumnRangeGroup.Count > 0)
                {
                    sheetView.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) chartShapeAffectedByColumnRangeGroup));
                }
                this.ShowColumnRangeGroup(sheetView);
            }
            return flag;
        }

        /// <summary>
        /// Gets a value that indicates whether the command or operation can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  true; }
        }
    }
}

