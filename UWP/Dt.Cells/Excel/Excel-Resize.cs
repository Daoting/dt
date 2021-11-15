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
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        void AutoFitColumn()
        {
            ColumnLayout viewportResizingColumnLayoutFromX;
            if (IsResizingColumns)
            {
                EndColumnResizing();
            }
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (savedHitTestInformation.HitTestType == HitTestType.ColumnHeader)
            {
                viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(savedHitTestInformation.ColumnViewportIndex, savedHitTestInformation.HitPoint.X);
                bool flag = false;
                if (viewportResizingColumnLayoutFromX == null)
                {
                    if (savedHitTestInformation.ColumnViewportIndex == 0)
                    {
                        viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(-1, savedHitTestInformation.HitPoint.X);
                    }
                    if ((viewportResizingColumnLayoutFromX == null) && ((savedHitTestInformation.ColumnViewportIndex == 0) || (savedHitTestInformation.ColumnViewportIndex == -1)))
                    {
                        viewportResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(savedHitTestInformation.HitPoint.X);
                        flag = true;
                    }
                }
                if (viewportResizingColumnLayoutFromX != null)
                {
                    int column = viewportResizingColumnLayoutFromX.Column;
                    if (!flag)
                    {
                        AutoFitColumnInternal(column, true, false);
                    }
                    else
                    {
                        ColumnAutoFitUndoAction command = new ColumnAutoFitUndoAction(ActiveSheet, new ColumnAutoFitExtent[] { new ColumnAutoFitExtent(column) }, true);
                        DoCommand(command);
                    }
                }
            }
            else if (savedHitTestInformation.HitTestType == HitTestType.Corner)
            {
                viewportResizingColumnLayoutFromX = GetRowHeaderColumnLayoutModel().FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                if (viewportResizingColumnLayoutFromX != null)
                {
                    int num2 = viewportResizingColumnLayoutFromX.Column;
                    ColumnAutoFitUndoAction action2 = new ColumnAutoFitUndoAction(ActiveSheet, new ColumnAutoFitExtent[] { new ColumnAutoFitExtent(num2) }, true);
                    DoCommand(action2);
                }
            }
        }

        void AutoFitColumnForTouch(HitTestInformation hi)
        {
            ColumnLayout viewportResizingColumnLayoutFromXForTouch;
            if (IsTouchResizingColumns)
            {
                EndTouchColumnResizing();
            }
            if (hi.HitTestType == HitTestType.ColumnHeader)
            {
                viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(hi.ColumnViewportIndex, hi.HitPoint.X);
                bool flag = false;
                if (viewportResizingColumnLayoutFromXForTouch == null)
                {
                    if (hi.ColumnViewportIndex == 0)
                    {
                        viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(-1, hi.HitPoint.X);
                    }
                    if ((viewportResizingColumnLayoutFromXForTouch == null) && ((hi.ColumnViewportIndex == 0) || (hi.ColumnViewportIndex == -1)))
                    {
                        viewportResizingColumnLayoutFromXForTouch = GetRowHeaderResizingColumnLayoutFromXForTouch(hi.HitPoint.X);
                        flag = true;
                    }
                }
                if (viewportResizingColumnLayoutFromXForTouch != null)
                {
                    int column = viewportResizingColumnLayoutFromXForTouch.Column;
                    if (!flag)
                    {
                        AutoFitColumnInternal(column, true, false);
                    }
                    else
                    {
                        ColumnAutoFitUndoAction command = new ColumnAutoFitUndoAction(ActiveSheet, new ColumnAutoFitExtent[] { new ColumnAutoFitExtent(column) }, true);
                        DoCommand(command);
                    }
                }
            }
            else if (hi.HitTestType == HitTestType.Corner)
            {
                viewportResizingColumnLayoutFromXForTouch = GetRowHeaderColumnLayoutModel().FindColumn(hi.HeaderInfo.ResizingColumn);
                if (viewportResizingColumnLayoutFromXForTouch != null)
                {
                    int num2 = viewportResizingColumnLayoutFromXForTouch.Column;
                    ColumnAutoFitUndoAction action2 = new ColumnAutoFitUndoAction(ActiveSheet, new ColumnAutoFitExtent[] { new ColumnAutoFitExtent(num2) }, true);
                    DoCommand(action2);
                }
            }
        }

        void AutoFitColumnInternal(int columnIndex, bool supportUndo, bool isRowHeader)
        {
            List<ColumnAutoFitExtent> list = new List<ColumnAutoFitExtent>();
            if (ActiveSheet.IsSelected(-1, columnIndex))
            {
                foreach (CellRange range in ActiveSheet.Selections)
                {
                    if (range.Row == -1)
                    {
                        int num = (range.Column == -1) ? 0 : range.Column;
                        int num2 = (range.Column == -1) ? ActiveSheet.ColumnCount : range.ColumnCount;
                        for (int i = num; i < (num + num2); i++)
                        {
                            list.Add(new ColumnAutoFitExtent(i));
                        }
                    }
                }
            }
            else
            {
                list.Add(new ColumnAutoFitExtent(columnIndex));
            }
            ColumnAutoFitExtent[] columns = new ColumnAutoFitExtent[list.Count];
            list.CopyTo(columns);
            ColumnAutoFitUndoAction command = new ColumnAutoFitUndoAction(ActiveSheet, columns, isRowHeader);
            if (supportUndo)
            {
                DoCommand(command);
            }
            else
            {
                command.Execute(this);
            }
        }

        void AutoFitRow()
        {
            RowLayout viewportResizingRowLayoutFromY;
            if (IsResizingRows)
            {
                EndRowResizing();
            }
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (savedHitTestInformation.HitTestType == HitTestType.RowHeader)
            {
                bool flag = false;
                viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.HitPoint.Y);
                if (viewportResizingRowLayoutFromY == null)
                {
                    if (savedHitTestInformation.RowViewportIndex == 0)
                    {
                        viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(-1, savedHitTestInformation.HitPoint.Y);
                    }
                    if ((viewportResizingRowLayoutFromY == null) && ((savedHitTestInformation.RowViewportIndex == -1) || (savedHitTestInformation.RowViewportIndex == 0)))
                    {
                        viewportResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(savedHitTestInformation.HitPoint.Y);
                        flag = true;
                    }
                }
                if (viewportResizingRowLayoutFromY != null)
                {
                    int row = viewportResizingRowLayoutFromY.Row;
                    if (!flag)
                    {
                        AutoFitRowInternal(row, true, false);
                    }
                    else
                    {
                        RowAutoFitUndoAction command = new RowAutoFitUndoAction(ActiveSheet, new RowAutoFitExtent[] { new RowAutoFitExtent(row) }, true);
                        DoCommand(command);
                    }
                }
            }
            else if (savedHitTestInformation.HitTestType == HitTestType.Corner)
            {
                viewportResizingRowLayoutFromY = GetColumnHeaderRowLayoutModel().FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                if (viewportResizingRowLayoutFromY != null)
                {
                    int num2 = viewportResizingRowLayoutFromY.Row;
                    RowAutoFitUndoAction action2 = new RowAutoFitUndoAction(ActiveSheet, new RowAutoFitExtent[] { new RowAutoFitExtent(num2) }, true);
                    DoCommand(action2);
                }
            }
        }

        void AutoFitRowForTouch(HitTestInformation hi)
        {
            RowLayout viewportResizingRowLayoutFromYForTouch;
            if (IsTouchResizingRows)
            {
                EndTouchRowResizing();
            }
            if (hi.HitTestType == HitTestType.RowHeader)
            {
                bool flag = false;
                viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(hi.RowViewportIndex, hi.HitPoint.Y);
                if (viewportResizingRowLayoutFromYForTouch == null)
                {
                    if (hi.RowViewportIndex == 0)
                    {
                        viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(-1, hi.HitPoint.Y);
                    }
                    if ((viewportResizingRowLayoutFromYForTouch == null) && ((hi.RowViewportIndex == -1) || (hi.RowViewportIndex == 0)))
                    {
                        viewportResizingRowLayoutFromYForTouch = GetColumnHeaderResizingRowLayoutFromYForTouch(hi.HitPoint.Y);
                        flag = true;
                    }
                }
                if (viewportResizingRowLayoutFromYForTouch != null)
                {
                    int row = viewportResizingRowLayoutFromYForTouch.Row;
                    if (!flag)
                    {
                        AutoFitRowInternal(row, true, false);
                    }
                    else
                    {
                        RowAutoFitUndoAction command = new RowAutoFitUndoAction(ActiveSheet, new RowAutoFitExtent[] { new RowAutoFitExtent(row) }, true);
                        DoCommand(command);
                    }
                }
            }
            else if (hi.HitTestType == HitTestType.Corner)
            {
                viewportResizingRowLayoutFromYForTouch = GetColumnHeaderRowLayoutModel().FindRow(hi.HeaderInfo.ResizingRow);
                if (viewportResizingRowLayoutFromYForTouch != null)
                {
                    int num2 = viewportResizingRowLayoutFromYForTouch.Row;
                    RowAutoFitUndoAction action2 = new RowAutoFitUndoAction(ActiveSheet, new RowAutoFitExtent[] { new RowAutoFitExtent(num2) }, true);
                    DoCommand(action2);
                }
            }
        }

        void AutoFitRowInternal(int rowIndex, bool supportUndo, bool isColumnHeader)
        {
            List<RowAutoFitExtent> list = new List<RowAutoFitExtent>();
            if (ActiveSheet.IsSelected(rowIndex, -1))
            {
                foreach (CellRange range in ActiveSheet.Selections)
                {
                    if (range.Column == -1)
                    {
                        int num = (range.Row == -1) ? 0 : range.Row;
                        int num2 = (range.Row == -1) ? ActiveSheet.RowCount : range.RowCount;
                        for (int i = num; i < (num + num2); i++)
                        {
                            list.Add(new RowAutoFitExtent(i));
                        }
                    }
                }
            }
            else
            {
                list.Add(new RowAutoFitExtent(rowIndex));
            }
            RowAutoFitExtent[] rows = new RowAutoFitExtent[list.Count];
            list.CopyTo(rows);
            RowAutoFitUndoAction command = new RowAutoFitUndoAction(ActiveSheet, rows, isColumnHeader);
            if (supportUndo)
            {
                DoCommand(command);
            }
            else
            {
                command.Execute(this);
            }
        }

        void ContinueColumnResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            ColumnLayout viewportResizingColumnLayoutFromX = null;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.Corner:
                    viewportResizingColumnLayoutFromX = GetRowHeaderColumnLayoutModel().FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                    break;

                case HitTestType.ColumnHeader:
                    viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(savedHitTestInformation.ColumnViewportIndex, savedHitTestInformation.HitPoint.X);
                    if (viewportResizingColumnLayoutFromX == null)
                    {
                        viewportResizingColumnLayoutFromX = GetViewportColumnLayoutModel(savedHitTestInformation.ColumnViewportIndex).FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                        if (viewportResizingColumnLayoutFromX == null)
                        {
                            if (savedHitTestInformation.ColumnViewportIndex == 0)
                            {
                                viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(-1, savedHitTestInformation.HitPoint.X);
                            }
                            if ((viewportResizingColumnLayoutFromX == null) && ((savedHitTestInformation.ColumnViewportIndex == 0) || (savedHitTestInformation.ColumnViewportIndex == -1)))
                            {
                                viewportResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(savedHitTestInformation.HitPoint.X);
                            }
                        }
                    }
                    break;
            }
            if (viewportResizingColumnLayoutFromX != null)
            {
                double x = viewportResizingColumnLayoutFromX.X;
                if (MousePosition.X > _resizingTracker.X1)
                {
                    _resizingTracker.X1 = Math.Min(base.ActualWidth, MousePosition.X) - 0.5;
                }
                else
                {
                    _resizingTracker.X1 = Math.Max(x, MousePosition.X) - 0.5;
                }
                _resizingTracker.X2 = _resizingTracker.X1;
                if ((InputDeviceType != InputDeviceType.Touch) && ((ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Column)))
                {
                    UpdateResizeToolTip(GetHorizontalResizeTip(Math.Max((double)0.0, (double)(MousePosition.X - x))), true);
                }
            }
        }

        void ContinueRowResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            RowLayout viewportResizingRowLayoutFromY = null;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.Corner:
                    viewportResizingRowLayoutFromY = GetColumnHeaderRowLayoutModel().FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                    break;

                case HitTestType.RowHeader:
                    viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.HitPoint.Y);
                    if (((viewportResizingRowLayoutFromY == null) && (savedHitTestInformation.HeaderInfo != null)) && (savedHitTestInformation.HeaderInfo.ResizingRow >= 0))
                    {
                        viewportResizingRowLayoutFromY = GetViewportRowLayoutModel(savedHitTestInformation.RowViewportIndex).FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                    }
                    if ((viewportResizingRowLayoutFromY == null) && (savedHitTestInformation.RowViewportIndex == 0))
                    {
                        viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(-1, savedHitTestInformation.HitPoint.Y);
                    }
                    if ((viewportResizingRowLayoutFromY == null) && ((savedHitTestInformation.RowViewportIndex == -1) || (savedHitTestInformation.RowViewportIndex == 0)))
                    {
                        viewportResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(savedHitTestInformation.HitPoint.Y);
                    }
                    break;
            }
            if (viewportResizingRowLayoutFromY != null)
            {
                double y = viewportResizingRowLayoutFromY.Y;
                if (MousePosition.Y > _resizingTracker.Y1)
                {
                    _resizingTracker.Y1 = Math.Min(base.ActualHeight, MousePosition.Y) - 0.5;
                }
                else
                {
                    _resizingTracker.Y1 = Math.Max(y, MousePosition.Y) - 0.5;
                }
                _resizingTracker.Y2 = _resizingTracker.Y1;
                if ((InputDeviceType != InputDeviceType.Touch) && ((ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Row)))
                {
                    UpdateResizeToolTip(GetVerticalResizeTip(Math.Max((double)0.0, (double)(MousePosition.Y - y))), false);
                }
            }
        }

        void ContinueTouchColumnResizing()
        {
            _DoTouchResizing = true;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            ColumnLayout viewportResizingColumnLayoutFromXForTouch = null;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.Corner:
                    viewportResizingColumnLayoutFromXForTouch = GetRowHeaderColumnLayoutModel().FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                    break;

                case HitTestType.ColumnHeader:
                    viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(savedHitTestInformation.ColumnViewportIndex, savedHitTestInformation.HitPoint.X);
                    if (viewportResizingColumnLayoutFromXForTouch == null)
                    {
                        viewportResizingColumnLayoutFromXForTouch = GetViewportColumnLayoutModel(savedHitTestInformation.ColumnViewportIndex).FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                        if ((viewportResizingColumnLayoutFromXForTouch == null) && (savedHitTestInformation.ColumnViewportIndex == 0))
                        {
                            viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(-1, savedHitTestInformation.HitPoint.X);
                        }
                        if ((viewportResizingColumnLayoutFromXForTouch == null) && ((savedHitTestInformation.ColumnViewportIndex == 0) || (savedHitTestInformation.ColumnViewportIndex == -1)))
                        {
                            viewportResizingColumnLayoutFromXForTouch = GetRowHeaderResizingColumnLayoutFromXForTouch(savedHitTestInformation.HitPoint.X);
                        }
                    }
                    break;
            }
            if (viewportResizingColumnLayoutFromXForTouch != null)
            {
                double x = viewportResizingColumnLayoutFromXForTouch.X;
                if (MousePosition.X > _resizingTracker.X1)
                {
                    _resizingTracker.X1 = Math.Min(base.ActualWidth, MousePosition.X) - 0.5;
                }
                else
                {
                    _resizingTracker.X1 = Math.Max(x, MousePosition.X) - 0.5;
                }
                _resizingTracker.X2 = _resizingTracker.X1;
                if ((InputDeviceType != InputDeviceType.Touch) && ((ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Column)))
                {
                    UpdateResizeToolTip(GetHorizontalResizeTip(Math.Max((double)0.0, (double)(MousePosition.X - x))), true);
                }
            }
        }

        void ContinueTouchRowResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            RowLayout viewportResizingRowLayoutFromYForTouch = null;
            _DoTouchResizing = true;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.Corner:
                    viewportResizingRowLayoutFromYForTouch = GetColumnHeaderRowLayoutModel().FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                    break;

                case HitTestType.RowHeader:
                    viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.HitPoint.Y);
                    if (((viewportResizingRowLayoutFromYForTouch == null) && (savedHitTestInformation.HeaderInfo != null)) && (savedHitTestInformation.HeaderInfo.ResizingRow >= 0))
                    {
                        viewportResizingRowLayoutFromYForTouch = GetViewportRowLayoutModel(savedHitTestInformation.RowViewportIndex).FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                    }
                    if ((viewportResizingRowLayoutFromYForTouch == null) && (savedHitTestInformation.RowViewportIndex == 0))
                    {
                        viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(-1, savedHitTestInformation.HitPoint.Y);
                    }
                    if ((viewportResizingRowLayoutFromYForTouch == null) && ((savedHitTestInformation.RowViewportIndex == -1) || (savedHitTestInformation.RowViewportIndex == 0)))
                    {
                        viewportResizingRowLayoutFromYForTouch = GetColumnHeaderResizingRowLayoutFromYForTouch(savedHitTestInformation.HitPoint.Y);
                    }
                    break;
            }
            if (viewportResizingRowLayoutFromYForTouch != null)
            {
                double y = viewportResizingRowLayoutFromYForTouch.Y;
                if (MousePosition.Y > _resizingTracker.Y1)
                {
                    _resizingTracker.Y1 = Math.Min(base.ActualHeight, MousePosition.Y) - 0.5;
                }
                else
                {
                    _resizingTracker.Y1 = Math.Max(y, MousePosition.Y) - 0.5;
                }
                _resizingTracker.Y2 = _resizingTracker.Y1;
                if ((InputDeviceType != InputDeviceType.Touch) && ((ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Row)))
                {
                    UpdateResizeToolTip(GetVerticalResizeTip(Math.Max((double)0.0, (double)(MousePosition.Y - y))), false);
                }
            }
        }

        void EndColumnResizing()
        {
            IsWorking = false;
            IsResizingColumns = false;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (savedHitTestInformation.HitPoint.X == MousePosition.X)
            {
                _resizingTracker.Visibility = Visibility.Collapsed;
                CloseTooltip();
            }
            else
            {
                ColumnLayout viewportResizingColumnLayoutFromX;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.Corner:
                        viewportResizingColumnLayoutFromX = GetRowHeaderColumnLayoutModel().FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                        if (viewportResizingColumnLayoutFromX != null)
                        {
                            double num6 = (_resizingTracker.X1 - viewportResizingColumnLayoutFromX.X) - viewportResizingColumnLayoutFromX.Width;
                            int column = viewportResizingColumnLayoutFromX.Column;
                            double size = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.RowHeader.Columns[column].ActualWidth + (num6 / ((double)ZoomFactor)))));
                            ColumnResizeExtent[] columns = new ColumnResizeExtent[] { new ColumnResizeExtent(column, column) };
                            ColumnResizeUndoAction command = new ColumnResizeUndoAction(ActiveSheet, columns, size, true);
                            DoCommand(command);
                        }
                        break;

                    case HitTestType.ColumnHeader:
                        {
                            viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(savedHitTestInformation.ColumnViewportIndex, savedHitTestInformation.HitPoint.X);
                            bool flag = false;
                            if (viewportResizingColumnLayoutFromX == null)
                            {
                                viewportResizingColumnLayoutFromX = GetViewportColumnLayoutModel(savedHitTestInformation.ColumnViewportIndex).FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                                if (viewportResizingColumnLayoutFromX == null)
                                {
                                    if (savedHitTestInformation.ColumnViewportIndex == 0)
                                    {
                                        viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(-1, savedHitTestInformation.HitPoint.X);
                                    }
                                    if ((viewportResizingColumnLayoutFromX == null) && ((savedHitTestInformation.ColumnViewportIndex == 0) || (savedHitTestInformation.ColumnViewportIndex == -1)))
                                    {
                                        viewportResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(savedHitTestInformation.HitPoint.X);
                                        flag = true;
                                    }
                                }
                            }
                            if (viewportResizingColumnLayoutFromX != null)
                            {
                                double num = (_resizingTracker.X1 - viewportResizingColumnLayoutFromX.X) - viewportResizingColumnLayoutFromX.Width;
                                int num2 = viewportResizingColumnLayoutFromX.Column;
                                double num3 = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.Columns[num2].ActualWidth + (num / ((double)ZoomFactor)))));
                                if (!flag)
                                {
                                    List<ColumnResizeExtent> list = new List<ColumnResizeExtent>();
                                    if (ActiveSheet.IsSelected(-1, num2))
                                    {
                                        foreach (CellRange range in ActiveSheet.Selections)
                                        {
                                            if (range.Row == -1)
                                            {
                                                int firstColumn = (range.Column == -1) ? 0 : range.Column;
                                                int num5 = ((range.Column == -1) && (range.ColumnCount == -1)) ? ActiveSheet.ColumnCount : range.ColumnCount;
                                                list.Add(new ColumnResizeExtent(firstColumn, (firstColumn + num5) - 1));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new ColumnResizeExtent(num2, num2));
                                    }
                                    ColumnResizeExtent[] extentArray = new ColumnResizeExtent[list.Count];
                                    list.CopyTo(extentArray);
                                    ColumnResizeUndoAction action = new ColumnResizeUndoAction(ActiveSheet, extentArray, num3, false);
                                    DoCommand(action);
                                }
                                else
                                {
                                    ColumnResizeExtent[] extentArray2 = new ColumnResizeExtent[] { new ColumnResizeExtent(num2, num2) };
                                    ColumnResizeUndoAction action2 = new ColumnResizeUndoAction(ActiveSheet, extentArray2, num3, true);
                                    DoCommand(action2);
                                }
                            }
                            break;
                        }
                }
                _resizingTracker.Visibility = Visibility.Collapsed;
                CloseTooltip();
            }
        }

        void EndRowResizing()
        {
            IsWorking = false;
            IsResizingRows = false;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (savedHitTestInformation.HitPoint.Y == MousePosition.Y)
            {
                TooltipHelper.CloseTooltip();
                _resizingTracker.Visibility = Visibility.Collapsed;
            }
            else
            {
                RowLayout viewportResizingRowLayoutFromY = null;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.Corner:
                        viewportResizingRowLayoutFromY = GetColumnHeaderRowLayoutModel().FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                        if (viewportResizingRowLayoutFromY != null)
                        {
                            double num6 = (_resizingTracker.Y1 - viewportResizingRowLayoutFromY.Y) - viewportResizingRowLayoutFromY.Height;
                            int row = viewportResizingRowLayoutFromY.Row;
                            double size = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.ColumnHeader.Rows[row].ActualHeight + (num6 / ((double)ZoomFactor)))));
                            RowResizeExtent[] rows = new RowResizeExtent[] { new RowResizeExtent(row, row) };
                            RowResizeUndoAction command = new RowResizeUndoAction(ActiveSheet, rows, size, true);
                            DoCommand(command);
                        }
                        break;

                    case HitTestType.RowHeader:
                        {
                            viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.HitPoint.Y);
                            bool flag = false;
                            if (((viewportResizingRowLayoutFromY == null) && (savedHitTestInformation.HeaderInfo != null)) && (savedHitTestInformation.HeaderInfo.ResizingRow >= 0))
                            {
                                viewportResizingRowLayoutFromY = GetViewportRowLayoutModel(savedHitTestInformation.RowViewportIndex).FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                            }
                            if ((viewportResizingRowLayoutFromY == null) && (savedHitTestInformation.RowViewportIndex == 0))
                            {
                                viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(-1, savedHitTestInformation.HitPoint.Y);
                            }
                            if ((viewportResizingRowLayoutFromY == null) && ((savedHitTestInformation.RowViewportIndex == -1) || (savedHitTestInformation.RowViewportIndex == 0)))
                            {
                                viewportResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(savedHitTestInformation.HitPoint.Y);
                                flag = true;
                            }
                            if (viewportResizingRowLayoutFromY != null)
                            {
                                double num = (_resizingTracker.Y1 - viewportResizingRowLayoutFromY.Y) - viewportResizingRowLayoutFromY.Height;
                                int firstRow = viewportResizingRowLayoutFromY.Row;
                                double num3 = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.Rows[firstRow].ActualHeight + (num / ((double)ZoomFactor)))));
                                if (flag)
                                {
                                    RowResizeExtent[] extentArray2 = new RowResizeExtent[] { new RowResizeExtent(firstRow, firstRow) };
                                    RowResizeUndoAction action2 = new RowResizeUndoAction(ActiveSheet, extentArray2, num3, true);
                                    DoCommand(action2);
                                    break;
                                }
                                List<RowResizeExtent> list = new List<RowResizeExtent>();
                                if (ActiveSheet.IsSelected(firstRow, -1))
                                {
                                    foreach (CellRange range in ActiveSheet.Selections)
                                    {
                                        if (range.Column == -1)
                                        {
                                            int num4 = (range.Row == -1) ? 0 : range.Row;
                                            int num5 = ((range.Row == -1) && (range.RowCount == -1)) ? ActiveSheet.RowCount : range.RowCount;
                                            list.Add(new RowResizeExtent(num4, (num4 + num5) - 1));
                                        }
                                    }
                                }
                                else
                                {
                                    list.Add(new RowResizeExtent(firstRow, firstRow));
                                }
                                RowResizeExtent[] extentArray = new RowResizeExtent[list.Count];
                                list.CopyTo(extentArray);
                                RowResizeUndoAction action = new RowResizeUndoAction(ActiveSheet, extentArray, num3, false);
                                DoCommand(action);
                            }
                            break;
                        }
                }
                TooltipHelper.CloseTooltip();
                _resizingTracker.Visibility = Visibility.Collapsed;
            }
        }

        void EndTouchColumnResizing()
        {
            IsWorking = false;
            IsTouchResizingColumns = false;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitPoint.X == MousePosition.X) || !_DoTouchResizing)
            {
                TooltipHelper.CloseTooltip();
                _resizingTracker.Visibility = Visibility.Collapsed;
            }
            else
            {
                ColumnLayout viewportResizingColumnLayoutFromXForTouch;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.Corner:
                        viewportResizingColumnLayoutFromXForTouch = GetRowHeaderColumnLayoutModel().FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                        if (viewportResizingColumnLayoutFromXForTouch != null)
                        {
                            double num6 = (_resizingTracker.X1 - viewportResizingColumnLayoutFromXForTouch.X) - viewportResizingColumnLayoutFromXForTouch.Width;
                            int column = viewportResizingColumnLayoutFromXForTouch.Column;
                            double size = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.RowHeader.Columns[column].ActualWidth + (num6 / ((double)ZoomFactor)))));
                            ColumnResizeExtent[] columns = new ColumnResizeExtent[] { new ColumnResizeExtent(column, column) };
                            ColumnResizeUndoAction command = new ColumnResizeUndoAction(ActiveSheet, columns, size, true);
                            DoCommand(command);
                        }
                        break;

                    case HitTestType.ColumnHeader:
                        {
                            viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(savedHitTestInformation.ColumnViewportIndex, savedHitTestInformation.HitPoint.X);
                            bool flag = false;
                            if (viewportResizingColumnLayoutFromXForTouch == null)
                            {
                                viewportResizingColumnLayoutFromXForTouch = GetViewportColumnLayoutModel(savedHitTestInformation.ColumnViewportIndex).FindColumn(savedHitTestInformation.HeaderInfo.ResizingColumn);
                                if ((viewportResizingColumnLayoutFromXForTouch == null) && (savedHitTestInformation.ColumnViewportIndex == 0))
                                {
                                    viewportResizingColumnLayoutFromXForTouch = GetViewportResizingColumnLayoutFromXForTouch(-1, savedHitTestInformation.HitPoint.X);
                                }
                                if ((viewportResizingColumnLayoutFromXForTouch == null) && ((savedHitTestInformation.ColumnViewportIndex == 0) || (savedHitTestInformation.ColumnViewportIndex == -1)))
                                {
                                    viewportResizingColumnLayoutFromXForTouch = GetRowHeaderResizingColumnLayoutFromXForTouch(savedHitTestInformation.HitPoint.X);
                                    flag = true;
                                }
                            }
                            if (viewportResizingColumnLayoutFromXForTouch != null)
                            {
                                double num = (_resizingTracker.X1 - viewportResizingColumnLayoutFromXForTouch.X) - viewportResizingColumnLayoutFromXForTouch.Width;
                                int num2 = viewportResizingColumnLayoutFromXForTouch.Column;
                                double num3 = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.Columns[num2].ActualWidth + (num / ((double)ZoomFactor)))));
                                if (!flag)
                                {
                                    List<ColumnResizeExtent> list = new List<ColumnResizeExtent>();
                                    if (ActiveSheet.IsSelected(-1, num2))
                                    {
                                        foreach (CellRange range in ActiveSheet.Selections)
                                        {
                                            if (range.Row == -1)
                                            {
                                                int firstColumn = (range.Column == -1) ? 0 : range.Column;
                                                int num5 = ((range.Column == -1) && (range.ColumnCount == -1)) ? ActiveSheet.ColumnCount : range.ColumnCount;
                                                list.Add(new ColumnResizeExtent(firstColumn, (firstColumn + num5) - 1));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new ColumnResizeExtent(num2, num2));
                                    }
                                    ColumnResizeExtent[] extentArray = new ColumnResizeExtent[list.Count];
                                    list.CopyTo(extentArray);
                                    ColumnResizeUndoAction action = new ColumnResizeUndoAction(ActiveSheet, extentArray, num3, false);
                                    DoCommand(action);
                                }
                                else
                                {
                                    ColumnResizeExtent[] extentArray2 = new ColumnResizeExtent[] { new ColumnResizeExtent(num2, num2) };
                                    ColumnResizeUndoAction action2 = new ColumnResizeUndoAction(ActiveSheet, extentArray2, num3, true);
                                    DoCommand(action2);
                                }
                            }
                            break;
                        }
                }
                TooltipHelper.CloseTooltip();
                _resizingTracker.Visibility = Visibility.Collapsed;
                _DoTouchResizing = false;
            }
        }

        void EndTouchRowResizing()
        {
            IsWorking = false;
            IsTouchResizingRows = false;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitPoint.Y == MousePosition.Y) || !_DoTouchResizing)
            {
                TooltipHelper.CloseTooltip();
                _resizingTracker.Visibility = Visibility.Collapsed;
            }
            else
            {
                RowLayout viewportResizingRowLayoutFromYForTouch = null;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.Corner:
                        viewportResizingRowLayoutFromYForTouch = GetColumnHeaderRowLayoutModel().FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                        if (viewportResizingRowLayoutFromYForTouch != null)
                        {
                            double num6 = (_resizingTracker.Y1 - viewportResizingRowLayoutFromYForTouch.Y) - viewportResizingRowLayoutFromYForTouch.Height;
                            int row = viewportResizingRowLayoutFromYForTouch.Row;
                            double size = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.ColumnHeader.Rows[row].ActualHeight + (num6 / ((double)ZoomFactor)))));
                            RowResizeExtent[] rows = new RowResizeExtent[] { new RowResizeExtent(row, row) };
                            RowResizeUndoAction command = new RowResizeUndoAction(ActiveSheet, rows, size, true);
                            DoCommand(command);
                        }
                        break;

                    case HitTestType.RowHeader:
                        {
                            viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.HitPoint.Y);
                            bool flag = false;
                            if ((viewportResizingRowLayoutFromYForTouch == null) && (savedHitTestInformation.RowViewportIndex == 0))
                            {
                                viewportResizingRowLayoutFromYForTouch = GetViewportResizingRowLayoutFromYForTouch(-1, savedHitTestInformation.HitPoint.Y);
                            }
                            if (((viewportResizingRowLayoutFromYForTouch == null) && (savedHitTestInformation.HeaderInfo != null)) && (savedHitTestInformation.HeaderInfo.ResizingRow >= 0))
                            {
                                viewportResizingRowLayoutFromYForTouch = GetViewportRowLayoutModel(savedHitTestInformation.RowViewportIndex).FindRow(savedHitTestInformation.HeaderInfo.ResizingRow);
                            }
                            if ((viewportResizingRowLayoutFromYForTouch == null) && ((savedHitTestInformation.RowViewportIndex == -1) || (savedHitTestInformation.RowViewportIndex == 0)))
                            {
                                viewportResizingRowLayoutFromYForTouch = GetColumnHeaderResizingRowLayoutFromYForTouch(savedHitTestInformation.HitPoint.Y);
                                flag = true;
                            }
                            if (viewportResizingRowLayoutFromYForTouch != null)
                            {
                                double num = (_resizingTracker.Y1 - viewportResizingRowLayoutFromYForTouch.Y) - viewportResizingRowLayoutFromYForTouch.Height;
                                int firstRow = viewportResizingRowLayoutFromYForTouch.Row;
                                double num3 = Math.Ceiling(Math.Max((double)0.0, (double)(ActiveSheet.Rows[firstRow].ActualHeight + (num / ((double)ZoomFactor)))));
                                if (flag)
                                {
                                    RowResizeExtent[] extentArray2 = new RowResizeExtent[] { new RowResizeExtent(firstRow, firstRow) };
                                    RowResizeUndoAction action2 = new RowResizeUndoAction(ActiveSheet, extentArray2, num3, true);
                                    DoCommand(action2);
                                    break;
                                }
                                List<RowResizeExtent> list = new List<RowResizeExtent>();
                                if (ActiveSheet.IsSelected(firstRow, -1))
                                {
                                    foreach (CellRange range in ActiveSheet.Selections)
                                    {
                                        if (range.Column == -1)
                                        {
                                            int num4 = (range.Row == -1) ? 0 : range.Row;
                                            int num5 = ((range.Row == -1) && (range.RowCount == -1)) ? ActiveSheet.RowCount : range.RowCount;
                                            list.Add(new RowResizeExtent(num4, (num4 + num5) - 1));
                                        }
                                    }
                                }
                                else
                                {
                                    list.Add(new RowResizeExtent(firstRow, firstRow));
                                }
                                RowResizeExtent[] extentArray = new RowResizeExtent[list.Count];
                                list.CopyTo(extentArray);
                                RowResizeUndoAction action = new RowResizeUndoAction(ActiveSheet, extentArray, num3, false);
                                DoCommand(action);
                            }
                            break;
                        }
                }
                TooltipHelper.CloseTooltip();
                _resizingTracker.Visibility = Visibility.Collapsed;
                _DoTouchResizing = false;
            }
        }

        string GetHorizontalResizeTip(double size)
        {
            object[] args = new object[1];
            double num = size / ((double)ZoomFactor);
            args[0] = ((double)num).ToString("0");
            return string.Format(ResourceStrings.ColumnResize, args);
        }

        string GetVerticalResizeTip(double size)
        {
            object[] args = new object[1];
            double num = size / ((double)ZoomFactor);
            args[0] = ((double)num).ToString("0");
            return string.Format(ResourceStrings.RowResize, args);
        }

    }
}

