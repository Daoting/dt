#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
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
        ColumnGroupExpandExtent _columnExpandExtent;
        Worksheet _sheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnGroupExpandUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="columnExpandExtent">The column expand extent information.</param>
        public ColumnGroupExpandUndoAction(Worksheet sheet, ColumnGroupExpandExtent columnExpandExtent)
        {
            _sheet = sheet;
            _columnExpandExtent = columnExpandExtent;
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
            if (((_sheet != null) && (_columnExpandExtent != null)) && (_sheet.ColumnRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _columnExpandExtent.Index;
                    bool collapsed = _columnExpandExtent.Collapsed;
                    _sheet.ColumnRangeGroup.Data.SetCollapsed(index, collapsed);
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                Excel excel = sender as Excel;
                if (excel != null)
                {
                    excel.InvalidateLayout();
                    excel.InvalidateViewportHorizontalArrangement(-2);
                    excel.InvalidateHeaderHorizontalArrangement();
                    excel.InvalidateMeasure();
                    excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                    IList<SpreadChartBase> chartShapeAffectedByColumnRangeGroup = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedByColumnRangeGroup(excel.ActiveSheet);
                    if (chartShapeAffectedByColumnRangeGroup.Count > 0)
                    {
                        excel.RefreshFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) chartShapeAffectedByColumnRangeGroup));
                    }
                    ShowColumnRangeGroup(excel);
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
        }

        void ShowColumnRangeGroup(Excel excel)
        {
            int index = _columnExpandExtent.Index;
            if ((index >= 0) && (index < _sheet.ColumnCount))
            {
                ViewportInfo viewportInfo = excel.GetViewportInfo();
                if (excel.ActiveSheet.ColumnRangeGroup.Direction == RangeGroupDirection.Forward)
                {
                    RangeGroupInfo info2 = _sheet.ColumnRangeGroup.Find(index - 1, _columnExpandExtent.Level);
                    if (info2 != null)
                    {
                        int start = info2.Start;
                        int rightColumn = index;
                        int viewportIndex = _columnExpandExtent.ViewportIndex;
                        if (_sheet.ColumnRangeGroup.Data.GetCollapsed(index))
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
                                if (start < excel.ActiveSheet.FrozenColumnCount)
                                {
                                    start = excel.ActiveSheet.FrozenColumnCount;
                                }
                            }
                            else if (viewportIndex >= viewportInfo.ColumnViewportCount)
                            {
                                if (start >= (excel.ActiveSheet.ColumnCount - excel.ActiveSheet.FrozenTrailingColumnCount))
                                {
                                    return;
                                }
                                start = Math.Max(start, excel.ActiveSheet.FrozenColumnCount);
                                rightColumn = (excel.ActiveSheet.ColumnCount - excel.ActiveSheet.FrozenTrailingColumnCount) - 1;
                                viewportIndex--;
                            }
                        }
                        int viewportLeftColumn = excel.GetViewportLeftColumn(viewportIndex);
                        if (start < viewportLeftColumn)
                        {
                            viewportLeftColumn = start;
                        }
                        double viewportWidth = excel.GetViewportWidth(viewportIndex);
                        double num7 = NavigatorHelper.GetColumnWidth(excel.ActiveSheet, viewportLeftColumn, rightColumn);
                        if (num7 > viewportWidth)
                        {
                            viewportLeftColumn = NavigatorHelper.GetNewLeftColumn(excel.ActiveSheet, viewportLeftColumn, num7 - viewportWidth);
                        }
                        excel.SetViewportLeftColumn(viewportIndex, viewportLeftColumn);
                    }
                }
                else if (_sheet.ColumnRangeGroup.Direction == RangeGroupDirection.Backward)
                {
                    RangeGroupInfo info3 = _sheet.ColumnRangeGroup.Find(index + 1, _columnExpandExtent.Level);
                    if (info3 != null)
                    {
                        int frozenColumnCount = index;
                        int end = info3.End;
                        int columnViewportIndex = _columnExpandExtent.ViewportIndex;
                        if (_sheet.ColumnRangeGroup.Data.GetCollapsed(index))
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
                                if (end >= (excel.ActiveSheet.ColumnCount - excel.ActiveSheet.FrozenTrailingColumnCount))
                                {
                                    end = (excel.ActiveSheet.ColumnCount - excel.ActiveSheet.FrozenTrailingColumnCount) - 1;
                                }
                            }
                            else if (columnViewportIndex < 0)
                            {
                                if (end < excel.ActiveSheet.FrozenColumnCount)
                                {
                                    return;
                                }
                                frozenColumnCount = excel.ActiveSheet.FrozenColumnCount;
                                end = Math.Min(end, (excel.ActiveSheet.ColumnCount - excel.ActiveSheet.FrozenTrailingColumnCount) - 1);
                                columnViewportIndex++;
                            }
                        }
                        int leftColumn = excel.GetViewportLeftColumn(columnViewportIndex);
                        if (frozenColumnCount < leftColumn)
                        {
                            excel.SetViewportLeftColumn(columnViewportIndex, frozenColumnCount);
                        }
                        else
                        {
                            double num12 = excel.GetViewportWidth(columnViewportIndex);
                            double num13 = NavigatorHelper.GetColumnWidth(excel.ActiveSheet, leftColumn, end);
                            if (num13 > num12)
                            {
                                leftColumn = NavigatorHelper.GetNewLeftColumn(excel.ActiveSheet, leftColumn, num13 - num12);
                                excel.SetViewportLeftColumn(columnViewportIndex, Math.Min(frozenColumnCount, leftColumn));
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
            if (((_sheet != null) && (_columnExpandExtent != null)) && (_sheet.ColumnRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _columnExpandExtent.Index;
                    bool collapsed = _columnExpandExtent.Collapsed;
                    _sheet.ColumnRangeGroup.Data.SetCollapsed(index, !collapsed);
                    flag = true;
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                Excel excel = sender as Excel;
                if (excel == null)
                {
                    return flag;
                }
                excel.InvalidateLayout();
                excel.InvalidateViewportHorizontalArrangement(-2);
                excel.InvalidateHeaderHorizontalArrangement();
                excel.InvalidateMeasure();
                excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                IList<SpreadChartBase> chartShapeAffectedByColumnRangeGroup = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedByColumnRangeGroup(excel.ActiveSheet);
                if (chartShapeAffectedByColumnRangeGroup.Count > 0)
                {
                    excel.RefreshFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) chartShapeAffectedByColumnRangeGroup));
                }
                ShowColumnRangeGroup(excel);
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

