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
    /// <summary>
    /// Represents the GcSpreadSheet worksheet viewer used to present and handle worksheet operations.
    /// </summary>
    public partial class SheetView : Panel, IXmlSerializable
    {
        internal SheetView(Excel p_owner)
        {
            Excel = p_owner;
            _invisibleRows = new HashSet<int>();
            _invisibleColumns = new HashSet<int>();
            _formulaSelectionFeature = new FormulaSelectionFeature(this);
            InitInput();
            Init();
        }

        /// <summary>
        /// Adjusts the adjacent column viewport's width.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index to adjust, it adjusts the column viewport and its next column viewport.</param>
        /// <param name="deltaViewportWidth">The column width adjusted offset.</param>
        public void AdjustColumnViewport(int columnViewportIndex, double deltaViewportWidth)
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            if ((columnViewportIndex < 0) || (columnViewportIndex > (viewportInfo.ColumnViewportCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            if ((viewportInfo.ColumnViewportCount > 1) && (columnViewportIndex != (viewportInfo.ColumnViewportCount - 1)))
            {
                int index = columnViewportIndex + 1;
                viewportInfo.ViewportWidth[columnViewportIndex] = DoubleUtil.Formalize(GetViewportWidth(columnViewportIndex) + deltaViewportWidth) / ((double)ZoomFactor);
                viewportInfo.ViewportWidth[index] = DoubleUtil.Formalize(GetViewportWidth(index) - deltaViewportWidth) / ((double)ZoomFactor);
                if (viewportInfo.ViewportWidth[index] == 0.0)
                {
                    ActiveSheet.RemoveColumnViewport(index);
                }
                if (viewportInfo.ViewportWidth[columnViewportIndex] == 0.0)
                {
                    ActiveSheet.RemoveColumnViewport(columnViewportIndex);
                }
                viewportInfo = GetViewportInfo();
                viewportInfo.ViewportWidth[viewportInfo.ColumnViewportCount - 1] = -1.0;
                ActiveSheet.SetViewportInfo(viewportInfo);
                InvalidateLayout();
                InvalidateMeasure();
            }
        }

        CellRange AdjustFillRange(CellRange fillRange)
        {
            int row = (fillRange.Row != -1) ? fillRange.Row : 0;
            int column = (fillRange.Column != -1) ? fillRange.Column : 0;
            int rowCount = (fillRange.RowCount != -1) ? fillRange.RowCount : ActiveSheet.RowCount;
            return new CellRange(row, column, rowCount, (fillRange.ColumnCount != -1) ? fillRange.ColumnCount : ActiveSheet.ColumnCount);
        }

        /// <summary>
        /// Adjusts the adjacent row viewport's height.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index to adjust, it adjusts the row viewport and its next row viewport.</param>
        /// <param name="deltaViewportHeight">The row height adjusted offset.</param>
        public void AdjustRowViewport(int rowViewportIndex, double deltaViewportHeight)
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            if ((rowViewportIndex < 0) || (rowViewportIndex > (viewportInfo.RowViewportCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            if ((viewportInfo.RowViewportCount > 1) && (rowViewportIndex != (viewportInfo.RowViewportCount - 1)))
            {
                int index = rowViewportIndex + 1;
                viewportInfo.ViewportHeight[rowViewportIndex] = DoubleUtil.Formalize(GetViewportHeight(rowViewportIndex) + deltaViewportHeight) / ((double)ZoomFactor);
                viewportInfo.ViewportHeight[index] = DoubleUtil.Formalize(GetViewportHeight(index) - deltaViewportHeight) / ((double)ZoomFactor);
                if (viewportInfo.ViewportHeight[index] == 0.0)
                {
                    ActiveSheet.RemoveRowViewport(rowViewportIndex + 1);
                }
                if (viewportInfo.ViewportHeight[rowViewportIndex] == 0.0)
                {
                    ActiveSheet.RemoveRowViewport(rowViewportIndex);
                }
                viewportInfo = GetViewportInfo();
                viewportInfo.ViewportHeight[viewportInfo.RowViewportCount - 1] = -1.0;
                ActiveSheet.SetViewportInfo(viewportInfo);
                InvalidateLayout();
                InvalidateMeasure();
            }
        }

        CellRange AdjustViewportRange(int rowViewport, int columnViewport, CellRange range)
        {
            int row = (range.Row != -1) ? range.Row : GetViewportTopRow(rowViewport);
            int column = (range.Column != -1) ? range.Column : GetViewportLeftColumn(columnViewport);
            int rowCount = (range.RowCount != -1) ? range.RowCount : ActiveSheet.RowCount;
            return new CellRange(row, column, rowCount, (range.ColumnCount != -1) ? range.ColumnCount : ActiveSheet.ColumnCount);
        }

        internal DataValidationResult ApplyEditingValue(bool cancel = false)
        {
            if (IsEditing && EditorDirty)
            {
                CellsPanel editingViewport = EditingViewport;
                if (((editingViewport != null) && editingViewport.IsEditing()) && !cancel)
                {
                    int editingRowIndex = editingViewport.EditingContainer.EditingRowIndex;
                    int editingColumnIndex = editingViewport.EditingContainer.EditingColumnIndex;
                    string editorValue = (string)(editingViewport.GetEditorValue() as string);
                    CellEditExtent extent = new CellEditExtent(editingRowIndex, editingColumnIndex, editorValue);
                    CellEditUndoAction command = new CellEditUndoAction(ActiveSheet, extent);
                    DoCommand(command);
                    return command.ApplyResult;
                }
            }
            return DataValidationResult.ForceApply;
        }

        Point ArrangeDragFillTooltip(CellRange range, FillDirection direction)
        {
            int row = -1;
            int column = -1;
            switch (direction)
            {
                case FillDirection.Left:
                    row = (range.Row + range.RowCount) - 1;
                    column = range.Column;
                    break;

                case FillDirection.Right:
                case FillDirection.Down:
                    row = (range.Row + range.RowCount) - 1;
                    column = (range.Column + range.ColumnCount) - 1;
                    break;

                case FillDirection.Up:
                    row = range.Row;
                    column = (range.Column + range.ColumnCount) - 1;
                    break;
            }
            RowLayout layout = GetViewportRowLayoutModel(_dragToRowViewport).FindRow(row);
            ColumnLayout layout2 = GetViewportColumnLayoutModel(_dragToColumnViewport).FindColumn(column);
            if ((layout != null) && (layout2 != null))
            {
                switch (direction)
                {
                    case FillDirection.Left:
                        return new Point(layout2.X + 2.0, (layout.Y + layout.Height) + 2.0);

                    case FillDirection.Right:
                    case FillDirection.Down:
                        return new Point((layout2.X + layout2.Width) + 2.0, (layout.Y + layout.Height) + 2.0);

                    case FillDirection.Up:
                        return new Point((layout2.X + layout2.Width) + 2.0, layout.Y + 2.0);
                }
            }
            return new Point();
        }

        internal void ArrangeRangeGroup(int rowPaneCount, int columnPaneCount, SheetLayout layout)
        {
            double x;
            double y;
            GroupLayout groupLayout = GetGroupLayout();
            if ((_groupCornerPresenter != null) && (_groupCornerPresenter.Parent != null))
            {
                x = groupLayout.X;
                y = groupLayout.Y;
                if ((_groupCornerPresenter.Width != groupLayout.Width) || (_groupCornerPresenter.Height != groupLayout.Height))
                {
                    _groupCornerPresenter.Arrange(new Rect(x, y, groupLayout.Width, groupLayout.Height));
                }
            }
            if ((_rowGroupHeaderPresenter != null) && (_rowGroupHeaderPresenter.Parent != null))
            {
                x = groupLayout.X;
                y = groupLayout.Y + groupLayout.Height;
                double width = groupLayout.Width;
                double headerHeight = layout.HeaderHeight;
                _rowGroupHeaderPresenter.Arrange(new Rect(x, y, width, headerHeight));
            }
            if ((_columnGroupHeaderPresenter != null) && (_columnGroupHeaderPresenter.Parent != null))
            {
                x = groupLayout.X + groupLayout.Width;
                y = groupLayout.Y;
                double headerWidth = layout.HeaderWidth;
                double height = groupLayout.Height;
                _columnGroupHeaderPresenter.Arrange(new Rect(x, y, headerWidth, height));
            }
            if (_rowGroupPresenters != null)
            {
                for (int i = -1; i <= rowPaneCount; i++)
                {
                    GcRangeGroup group = _rowGroupPresenters[i + 1];
                    if (group != null)
                    {
                        x = groupLayout.X;
                        y = layout.GetViewportY(i);
                        double num8 = groupLayout.Width;
                        double viewportHeight = layout.GetViewportHeight(i);
                        if (!IsTouching || (i != _touchStartHitTestInfo.RowViewportIndex))
                        {
                            group.Arrange(new Rect(x, y, num8, viewportHeight));
                            group.Clip = null;
                        }
                        else
                        {
                            group.Arrange(new Rect(x, y + _translateOffsetY, num8, viewportHeight));
                            if (_translateOffsetY < 0.0)
                            {
                                RectangleGeometry geometry = new RectangleGeometry();
                                geometry.Rect = new Rect(x, Math.Abs(_translateOffsetY), num8, viewportHeight);
                                group.Clip = geometry;
                            }
                            else if (_translateOffsetY > 0.0)
                            {
                                RectangleGeometry geometry2 = new RectangleGeometry();
                                geometry2.Rect = new Rect(x, 0.0, num8, Math.Max((double)0.0, (double)(viewportHeight - Math.Abs(_translateOffsetY))));
                                group.Clip = geometry2;
                            }
                        }
                    }
                }
            }
            if (_columnGroupPresenters != null)
            {
                for (int j = -1; j <= columnPaneCount; j++)
                {
                    GcRangeGroup group2 = _columnGroupPresenters[j + 1];
                    if (group2 != null)
                    {
                        x = layout.GetViewportX(j);
                        y = groupLayout.Y;
                        double viewportWidth = layout.GetViewportWidth(j);
                        double num12 = groupLayout.Height;
                        if (!IsTouching || (j != _touchStartHitTestInfo.ColumnViewportIndex))
                        {
                            group2.Arrange(new Rect(x, y, viewportWidth, num12));
                            group2.Clip = null;
                        }
                        else
                        {
                            group2.Arrange(new Rect(x + _translateOffsetX, y, viewportWidth, num12));
                            if (_translateOffsetX < 0.0)
                            {
                                RectangleGeometry geometry3 = new RectangleGeometry();
                                geometry3.Rect = new Rect(Math.Abs(_translateOffsetX), y, viewportWidth, num12);
                                group2.Clip = geometry3;
                            }
                            else if (_translateOffsetX > 0.0)
                            {
                                RectangleGeometry geometry4 = new RectangleGeometry();
                                geometry4.Rect = new Rect(0.0, y, Math.Max((double)0.0, (double)(viewportWidth - Math.Abs(_translateOffsetX))), num12);
                                group2.Clip = geometry4;
                            }
                        }
                    }
                }
            }
        }

        Point CalcMoveOffset(int moveStartRowViewport, int moveStartColumnViewport, int moveStartRow, int moveStartColumn, Point startPoint, int moveEndRowViewport, int moveEndColumnViewport, int moveEndRow, int moveEndColumn, Point endPoint)
        {
            RowLayout layout = GetViewportRowLayoutModel(moveEndRowViewport).FindRow(moveEndRow);
            ColumnLayout layout2 = GetViewportColumnLayoutModel(moveEndColumnViewport).FindColumn(moveEndColumn);
            if ((layout == null) || (layout2 == null))
            {
                return new Point(0.0, 0.0);
            }
            Rect rect = _floatingObjectsMovingResizingStartPointCellBounds;
            Rect rect2 = new Rect(layout2.X, layout.Y, layout2.Width, layout.Height);
            bool flag = true;
            if (moveEndRow < moveStartRow)
            {
                flag = false;
                int num = moveStartRow;
                moveStartRow = moveEndRow;
                moveEndRow = num;
                double y = startPoint.Y;
                startPoint.Y = endPoint.Y;
                endPoint.Y = y;
                y = rect.Y;
                rect.Y = rect2.Y;
                rect2.Y = y;
                y = rect.Height;
                rect.Height = rect2.Height;
                rect2.Height = y;
            }
            double num3 = 0.0;
            for (int i = moveStartRow; i <= moveEndRow; i++)
            {
                num3 += Math.Ceiling((double)(ActiveSheet.GetActualRowHeight(i, SheetArea.Cells) * ZoomFactor));
            }
            num3 -= startPoint.Y - rect.Y;
            num3 -= (rect2.Y + rect2.Height) - endPoint.Y;
            if (!flag)
            {
                num3 = -num3;
            }
            bool flag2 = true;
            if (moveEndColumn < moveStartColumn)
            {
                flag2 = false;
                int num5 = moveStartColumn;
                moveStartColumn = moveEndColumn;
                moveEndColumn = num5;
                double width = startPoint.X;
                startPoint.X = endPoint.X;
                endPoint.X = width;
                width = rect.X;
                rect.X = rect2.X;
                rect2.X = width;
                width = rect.Width;
                rect.Width = rect2.Width;
                rect2.Width = width;
            }
            double x = 0.0;
            for (int j = moveStartColumn; j <= moveEndColumn; j++)
            {
                x += Math.Ceiling((double)(ActiveSheet.GetActualColumnWidth(j, SheetArea.Cells) * ZoomFactor));
            }
            x -= startPoint.X - rect.X;
            x -= (rect2.X + rect2.Width) - endPoint.X;
            if (!flag2)
            {
                x = -x;
            }
            x = Math.Floor((double)(x / ((double)ZoomFactor)));
            return new Point(x, Math.Floor((double)(num3 / ((double)ZoomFactor))));
        }

        internal bool CanCommitAndNavigate()
        {
            if (!IsEditing)
            {
                return false;
            }
            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
            if ((viewportRowsPresenter != null) && (((viewportRowsPresenter.EditingContainer != null) && (viewportRowsPresenter.EditingContainer.Editor != null)) && (viewportRowsPresenter.EditingContainer.EditorStatus == EditorStatus.Edit)))
            {
                return false;
            }
            return true;
        }

        internal bool CheckPastedRange(Worksheet fromSheet, CellRange fromRange, CellRange toRange, bool isCutting, string clipboardText, out CellRange pastedRange, out bool pasteInternal)
        {
            pasteInternal = false;
            pastedRange = null;
            CellRange exceptedRange = isCutting ? fromRange : null;
            if ((fromSheet == null) && string.IsNullOrEmpty(clipboardText))
            {
                return false;
            }
            pasteInternal = IsPastedInternal(fromSheet, fromRange, ActiveSheet, clipboardText);
            Worksheet toSheet = ActiveSheet;
            if (pasteInternal)
            {
                bool flag;
                string str;
                if ((isCutting && fromSheet.Protect) && IsAnyCellInRangeLocked(fromSheet, fromRange.Row, fromRange.Column, fromRange.RowCount, fromRange.ColumnCount))
                {
                    RaiseInvalidOperation(ResourceStrings.SheetViewPasteSouceSheetCellsAreLocked, null, null);
                    return false;
                }
                pastedRange = GetPastedRange(fromSheet, fromRange, toSheet, toRange, isCutting);
                if (RaiseValidationPasting(fromSheet, fromRange, ActiveSheet, toRange, pastedRange, isCutting, out flag, out str))
                {
                    pastedRange = GetPastedRange(fromSheet, fromRange, toSheet, toRange, isCutting);
                    return !flag;
                }
            }
            else
            {
                bool flag3;
                string str2;
                pastedRange = GetPastedRange(toRange, clipboardText);
                if (RaiseValidationPasting(null, null, ActiveSheet, toRange, pastedRange, isCutting, out flag3, out str2))
                {
                    return !flag3;
                }
            }
            if (pastedRange == null)
            {
                RaiseInvalidOperation(ResourceStrings.SheetViewTheCopyAreaAndPasteAreaAreNotTheSameSize, null, null);
                return false;
            }
            if (toSheet.Protect && IsAnyCellInRangeLocked(toSheet, pastedRange.Row, pastedRange.Column, pastedRange.RowCount, pastedRange.ColumnCount))
            {
                RaiseInvalidOperation(ResourceStrings.SheetViewPasteDestinationSheetCellsAreLocked, null, null);
                return false;
            }
            if (pasteInternal)
            {
                if (HasPartSpans(fromSheet, fromRange.Row, fromRange.Column, fromRange.RowCount, fromRange.ColumnCount))
                {
                    RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangeMergeCell, "Paste", new ClipboardPastingEventArgs(fromSheet, fromRange, toSheet, pastedRange, ClipBoardOptions, isCutting));
                    return false;
                }
                if (HasPartArrayFormulas(fromSheet, fromRange.Row, fromRange.Column, fromRange.RowCount, fromRange.ColumnCount, exceptedRange))
                {
                    RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangePartOfArrayFormula, null, null);
                    return false;
                }
                int rowCount = (pastedRange.Row < 0) ? toSheet.RowCount : pastedRange.RowCount;
                int columnCount = (pastedRange.Column < 0) ? toSheet.ColumnCount : pastedRange.ColumnCount;
                int num3 = (fromRange.Row < 0) ? fromSheet.RowCount : fromRange.RowCount;
                int num4 = (fromRange.Column < 0) ? fromSheet.ColumnCount : fromRange.ColumnCount;
                if ((rowCount <= num3) && (columnCount <= num4))
                {
                    if (HasPartSpans(toSheet, pastedRange.Row, pastedRange.Column, pastedRange.RowCount, pastedRange.ColumnCount))
                    {
                        RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangeMergeCell, "Paste", new ClipboardPastingEventArgs(fromSheet, fromRange, toSheet, pastedRange, ClipBoardOptions, isCutting));
                        return false;
                    }
                    if (HasPartArrayFormulas(toSheet, pastedRange.Row, pastedRange.Column, pastedRange.RowCount, pastedRange.ColumnCount, exceptedRange))
                    {
                        RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangePartOfArrayFormula, null, null);
                        return false;
                    }
                }
                else
                {
                    int row = toRange.Row;
                    int column = toRange.Column;
                    if ((toRange.Row < 0) && (num3 < toSheet.RowCount))
                    {
                        row = 0;
                    }
                    if ((toRange.Column < 0) && (num4 < toSheet.ColumnCount))
                    {
                        column = 0;
                    }
                    if (((rowCount % num3) != 0) || ((columnCount % num4) != 0))
                    {
                        rowCount = num3;
                        columnCount = num4;
                        pastedRange = new CellRange(row, column, rowCount, columnCount);
                    }
                    int num7 = rowCount / num3;
                    int num8 = columnCount / num4;
                    for (int i = 0; i < num7; i++)
                    {
                        for (int j = 0; j < num8; j++)
                        {
                            if (HasPartSpans(toSheet, (row < 0) ? -1 : (row + (i * num3)), (column < 0) ? -1 : (column + (j * num4)), (row < 0) ? -1 : num3, (column < 0) ? -1 : num4))
                            {
                                RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangeMergeCell, "Paste", new ClipboardPastingEventArgs(fromSheet, fromRange, toSheet, pastedRange, ClipBoardOptions, isCutting));
                                return false;
                            }
                            if (HasPartArrayFormulas(toSheet, (row < 0) ? -1 : (row + (i * num3)), (column < 0) ? -1 : (column + (j * num4)), (row < 0) ? -1 : num3, (column < 0) ? -1 : num4, exceptedRange))
                            {
                                RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangePartOfArrayFormula, null, null);
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (HasPartSpans(toSheet, pastedRange.Row, pastedRange.Column, pastedRange.RowCount, pastedRange.ColumnCount))
                {
                    RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangeMergeCell, "Paste", new ClipboardPastingEventArgs(fromSheet, fromRange, toSheet, pastedRange, ClipBoardOptions, isCutting));
                    return false;
                }
                if (HasPartArrayFormulas(toSheet, pastedRange.Row, pastedRange.Column, pastedRange.RowCount, pastedRange.ColumnCount, exceptedRange))
                {
                    RaiseInvalidOperation(ResourceStrings.SheetViewPasteChangePartOfArrayFormula, null, null);
                    return false;
                }
                if (((pastedRange.Row + pastedRange.RowCount) > toSheet.RowCount) || ((pastedRange.Column + pastedRange.ColumnCount) > toSheet.ColumnCount))
                {
                    RaiseInvalidOperation(ResourceStrings.SheetViewTheCopyAreaAndPasteAreaAreNotTheSameSize, null, null);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Clears all undo and redo actions in the current UndoManager. 
        /// </summary>
        public void ClearUndoManager()
        {
            if (_undoManager != null)
            {
                _undoManager.UndoList.Clear();
                _undoManager.RedoList.Clear();
            }
        }

        /// <summary>
        /// Copies the text of a cell range to the Clipboard.
        /// </summary>
        /// <param name="range">The copied cell range.</param>
        public void ClipboardCopy(CellRange range)
        {
            if (ActiveSheet != null)
            {
                if (range == null)
                {
                    throw new ArgumentNullException("range");
                }
                if (!IsValidRange(range.Row, range.Column, range.RowCount, range.ColumnCount, ActiveSheet.RowCount, ActiveSheet.ColumnCount))
                {
                    throw new ArgumentException(ResourceStrings.SheetViewClipboardArgumentException);
                }
                CopyToClipboard(range, false);
            }
        }

        /// <summary>
        /// Cuts the text of a cell range to the Clipboard.
        /// </summary>
        /// <param name="range">The cut cell range.</param>
        public void ClipboardCut(CellRange range)
        {
            if (ActiveSheet != null)
            {
                if (range == null)
                {
                    throw new ArgumentNullException("range");
                }
                if (!IsValidRange(range.Row, range.Column, range.RowCount, range.ColumnCount, ActiveSheet.RowCount, ActiveSheet.ColumnCount))
                {
                    throw new ArgumentException(ResourceStrings.SheetViewClipboardArgumentException);
                }
                CopyToClipboard(range, true);
            }
        }

        /// <summary>
        /// Pastes content from the Clipboard to a cell range on the sheet.
        /// </summary>
        /// <param name="range">The pasted cell range on the sheet.</param>
        public void ClipboardPaste(CellRange range)
        {
            ClipboardPaste(range, ClipboardPasteOptions.All);
        }

        /// <summary>
        /// Pastes content from the Clipboard to a cell range on the sheet.
        /// </summary>
        /// <param name="range">The pasted cell range.</param>
        /// <param name="option">The Clipboard paste option that indicates which content type to paste.</param>
        public void ClipboardPaste(CellRange range, ClipboardPasteOptions option)
        {
            if (ActiveSheet != null)
            {
                CellRange range1;
                bool flag2;
                if (range == null)
                {
                    throw new ArgumentNullException("range");
                }
                if (!IsValidRange(range.Row, range.Column, range.RowCount, range.ColumnCount, ActiveSheet.RowCount, ActiveSheet.ColumnCount))
                {
                    throw new ArgumentException(ResourceStrings.SheetViewClipboardArgumentException);
                }
                var fromSheet = SpreadXClipboard.Worksheet;
                CellRange fromRange = SpreadXClipboard.Range;
                string clipboardText = ClipboardHelper.GetClipboardData();
                bool isCutting = SpreadXClipboard.IsCutting;
                if (((isCutting && (fromSheet != null)) && ((fromRange != null) && fromSheet.Protect)) && IsAnyCellInRangeLocked(fromSheet, fromRange.Row, fromRange.Column, fromRange.RowCount, fromRange.ColumnCount))
                {
                    isCutting = false;
                }
                if (CheckPastedRange(fromSheet, fromRange, range, isCutting, clipboardText, out range1, out flag2))
                {
                    if (isCutting)
                    {
                        option = ClipboardPasteOptions.All;
                    }
                    if (flag2)
                    {
                        ClipboardPaste(fromSheet, fromRange, ActiveSheet, range1, isCutting, clipboardText, option);
                    }
                    else
                    {
                        ClipboardPaste(null, null, ActiveSheet, range1, isCutting, clipboardText, option);
                    }
                    SetSelection(range1.Row, range1.Column, range1.RowCount, range1.ColumnCount);
                    SetActiveCell((range.Row < 0) ? 0 : range.Row, (range.Column < 0) ? 0 : range.Column, false);
                    InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                }
            }
        }

        internal static void ClipboardPaste(Worksheet fromSheet, CellRange fromRange, Worksheet toSheet, CellRange toRange, bool isCutting, string clipboardText, ClipboardPasteOptions option)
        {
            if (((fromSheet != null) && (fromSheet.Workbook != null)) && (object.ReferenceEquals(toSheet.Workbook, fromSheet.Workbook) && !toSheet.Workbook.Sheets.Contains(fromSheet)))
            {
                ClipboardHelper.ClearClipboard();
            }
            else if ((fromSheet != null) && (fromRange != null))
            {
                if (isCutting)
                {
                    Workbook.MoveTo(fromSheet, fromRange.Row, fromRange.Column, toSheet, toRange.Row, toRange.Column, fromRange.RowCount, fromRange.ColumnCount, ConvertPasteOption(option));
                    ClipboardHelper.ClearClipboard();
                }
                else
                {
                    int num = (toRange.Row < 0) ? toSheet.RowCount : toRange.RowCount;
                    int num2 = (toRange.Column < 0) ? toSheet.ColumnCount : toRange.ColumnCount;
                    int num3 = (fromRange.Row < 0) ? fromSheet.RowCount : fromRange.RowCount;
                    int num4 = (fromRange.Column < 0) ? fromSheet.ColumnCount : fromRange.ColumnCount;
                    if ((num > num3) || (num2 > num4))
                    {
                        int row = toRange.Row;
                        int column = toRange.Column;
                        if ((toRange.Row < 0) && (num3 < toSheet.RowCount))
                        {
                            row = 0;
                        }
                        if ((toRange.Column < 0) && (num4 < toSheet.ColumnCount))
                        {
                            column = 0;
                        }
                        if (((num % num3) != 0) || ((num2 % num4) != 0))
                        {
                            num = num3;
                            num2 = num4;
                        }
                        int num7 = num / num3;
                        int num8 = num2 / num4;
                        fromSheet.SuspendCalcService();
                        toSheet.SuspendCalcService();
                        try
                        {
                            for (int i = 0; i < num7; i++)
                            {
                                for (int j = 0; j < num8; j++)
                                {
                                    Workbook.CopyTo(fromSheet, fromRange.Row, fromRange.Column, toSheet, (row < 0) ? -1 : (row + (i * num3)), (column < 0) ? -1 : (column + (j * num4)), (row < 0) ? -1 : num3, (column < 0) ? -1 : num4, ConvertPasteOption(option));
                                }
                            }
                            return;
                        }
                        finally
                        {
                            fromSheet.ResumeCalcService();
                            toSheet.ResumeCalcService();
                        }
                    }
                    Workbook.CopyTo(fromSheet, fromRange.Row, fromRange.Column, toSheet, toRange.Row, toRange.Column, fromRange.RowCount, fromRange.ColumnCount, ConvertPasteOption(option));
                }
            }
            else
            {
                int num11 = toRange.Row;
                int num12 = toRange.Column;
                int rowCount = toRange.RowCount;
                int columnCount = toRange.ColumnCount;
                IEnumerator enumerator = toSheet.SpanModel.GetEnumerator(num11, num12, rowCount, columnCount);
                while (enumerator.MoveNext())
                {
                    CellRange current = enumerator.Current as CellRange;
                    if (current != null)
                    {
                        toSheet.SpanModel.Remove(current.Row, current.Column);
                    }
                }
                if (string.IsNullOrEmpty(clipboardText))
                {
                    for (int k = 0; k < rowCount; k++)
                    {
                        for (int m = 0; m < columnCount; m++)
                        {
                            toSheet.SetValue(num11 + k, num12 + m, null);
                        }
                    }
                }
                else
                {
                    toSheet.SetCsv(num11, num12, clipboardText, "\r\n", "\t", "\"", TextFileOpenFlags.ImportFormula);
                }
            }
        }

        internal void CloseAutoFilterIndicator()
        {
            _autoFillIndicatorContainer.Width = 0.0;
            _autoFillIndicatorContainer.Height = 0.0;
            _autoFillIndicatorContainer.Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
            _autoFillIndicatorContainer.InvalidateMeasure();
            _autoFillIndicatorRec = null;
        }

        internal void CloseDragFillPopup()
        {
            if (_dragFillPopup != null)
            {
                _dragFillPopup.Close();
            }
            if (_dragFillSmartTag != null)
            {
                _dragFillSmartTag.AutoFilterTypeChanged -= new EventHandler(DragFillSmartTag_AutoFilterTypeChanged);
                _dragFillSmartTag.CloseDragFillSmartTagPopup();
                _dragFillSmartTag = null;
            }
        }

        internal void CloseTooltip()
        {
            TooltipHelper.CloseTooltip();
        }

        internal void CloseTouchToolbar()
        {
            if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
            {
                _touchToolbarPopup.IsOpen = false;
            }
        }

        internal bool ContainsFilterButton(int row, int column, SheetArea sheetArea)
        {
            return (GetFilterButtonInfo(row, column, sheetArea) != null);
        }

        void ContinueDragDropping()
        {
            if (IsDragDropping && (_dragDropFromRange != null))
            {
                DoContinueDragDropping();
            }
        }

        void ContinueDragFill()
        {
            if (IsDraggingFill && (_dragFillStartRange != null))
            {
                DoContinueDragFill();
            }
        }

        void ContinueTouchDragDropping()
        {
            if (IsTouchDrapDropping && (_dragDropFromRange != null))
            {
                DoContinueDragDropping();
            }
        }

        void ContinueTouchDragFill()
        {
            if (IsTouchDragFilling && (_dragFillStartRange != null))
            {
                DoContinueDragFill();
            }
        }

        internal static CopyToOption ConvertPasteOption(ClipboardPasteOptions pasteOption)
        {
            CopyToOption option = 0;
            if ((pasteOption & ClipboardPasteOptions.Values) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.Value;
            }
            if ((pasteOption & ClipboardPasteOptions.Formatting) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.Style;
            }
            if ((pasteOption & ClipboardPasteOptions.Formulas) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.Formula;
            }
            if ((pasteOption & ClipboardPasteOptions.FloatingObjects) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.FloatingObject;
            }
            if ((pasteOption & ClipboardPasteOptions.RangeGroup) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.RangeGroup;
            }
            if ((pasteOption & ClipboardPasteOptions.Sparkline) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.Sparkline;
            }
            if ((pasteOption & ClipboardPasteOptions.Span) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.Span;
            }
            if ((pasteOption & ClipboardPasteOptions.Tags) > ((ClipboardPasteOptions)0))
            {
                option |= CopyToOption.Tag;
            }
            return option;
        }

        void CopyToClipboard(CellRange range, bool isCutting)
        {
            SpreadXClipboard.Range = range;
            SpreadXClipboard.FloatingObjects = null;
            SpreadXClipboard.IsCutting = isCutting;
            SpreadXClipboard.Worksheet = ActiveSheet;
            ClipboardHelper.SetClipboardData(ActiveSheet.GetCsv(range.Row, range.Column, range.RowCount, range.ColumnCount, "\r\n", "\t", "\"", false));
        }

        AutoFilterDropDownItemControl CreateAutoFilter(FilterButtonInfo info)
        {
            HideRowFilter rowFilter = info.RowFilter;
            int column = info.Column;
            AutoFilterDropDownItemControl depObj = new AutoFilterDropDownItemControl();
            depObj.SuspendAllHandlers();
            AutoFilterItem item = new AutoFilterItem
            {
                IsChecked = null,
                Criterion = ResourceStrings.Filter_SelectAll
            };
            depObj.FilterItems.Add(item);
            ReadOnlyCollection<object> filterableDataItems = rowFilter.GetFilterableDataItems(column);
            bool flag = false;
            if ((filterableDataItems != null) && (filterableDataItems.Count > 0))
            {
                flag = filterableDataItems[filterableDataItems.Count - 1] == RowFilterBase.BlankItem;
            }
            List<object> filteredInDateItems = new List<object>();
            if (rowFilter.IsColumnFiltered(column))
            {
                filteredInDateItems = GetFilteredInDateItems(column, rowFilter);
            }
            else
            {
                filteredInDateItems = Enumerable.ToList<object>((IEnumerable<object>)filterableDataItems);
            }
            HashSet<object> set = new HashSet<object>();
            foreach (object obj2 in filteredInDateItems)
            {
                set.Add(obj2);
            }
            bool flag2 = true;
            bool flag3 = true;
            AutoFilterItem item2 = null;
            for (int i = 0; i < filterableDataItems.Count; i++)
            {
                object obj3 = filterableDataItems[i];
                bool flag4 = set.Contains(obj3);
                if ((obj3 == null) || string.IsNullOrEmpty(obj3.ToString()))
                {
                    if (item2 == null)
                    {
                        item2 = new AutoFilterItem
                        {
                            IsChecked = new bool?(flag4),
                            Criterion = BlankFilterItem.Blank
                        };
                    }
                }
                else
                {
                    AutoFilterItem item4 = new AutoFilterItem
                    {
                        IsChecked = new bool?(flag4),
                        Criterion = obj3
                    };
                    depObj.FilterItems.Add(item4);
                }
                flag2 = flag2 && flag4;
                flag3 = flag3 && !flag4;
            }
            if (flag && (item2 == null))
            {
                bool flag5 = false;
                if (rowFilter.IsColumnFiltered(column))
                {
                    foreach (object obj4 in filteredInDateItems)
                    {
                        if ((obj4 == null) || string.IsNullOrEmpty(obj4.ToString()))
                        {
                            flag5 = true;
                            break;
                        }
                    }
                }
                else
                {
                    flag5 = true;
                }
                item2 = new AutoFilterItem
                {
                    IsChecked = new bool?(flag5),
                    Criterion = BlankFilterItem.Blank
                };
                flag2 = flag2 && flag5;
                flag3 = flag3 && !flag5;
            }
            if (item2 != null)
            {
                depObj.FilterItems.Add(item2);
            }
            if (flag2)
            {
                item.IsChecked = true;
            }
            else if (flag3)
            {
                item.IsChecked = false;
            }
            else
            {
                item.IsChecked = null;
            }
            depObj.Command = new FilterCommand(this, info, column);
            depObj.ResumeAllHandlers();
            return depObj;
        }

        CellClickEventArgs CreateCellClickEventArgs(int row, int column, SheetSpanModel spanModel, SheetArea area, MouseButtonType button)
        {
            CellRange range = spanModel.Find(row, column);
            if (range != null)
            {
                row = range.Row;
                column = range.Column;
            }
            return new CellClickEventArgs(area, row, column, button);
        }

        internal CellLayoutModel CreateColumnHeaderCellLayoutModel(int columnViewportIndex)
        {
            ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(columnViewportIndex);
            RowLayoutModel columnHeaderRowLayoutModel = GetColumnHeaderRowLayoutModel();
            CellLayoutModel model3 = new CellLayoutModel();
            if ((viewportColumnLayoutModel.Count > 0) && (columnHeaderRowLayoutModel.Count > 0))
            {
                Worksheet worksheet = ActiveSheet;
                int row = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)columnHeaderRowLayoutModel, 0).Row;
                int column = viewportColumnLayoutModel[0].Column;
                int num3 = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)columnHeaderRowLayoutModel, columnHeaderRowLayoutModel.Count - 1).Row;
                int num4 = viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column;
                IEnumerator enumerator = ActiveSheet.ColumnHeaderSpanModel.GetEnumerator(row, column, (num3 - row) + 1, (num4 - column) + 1);
                float zoomFactor = ZoomFactor;
                while (enumerator.MoveNext())
                {
                    double num6 = 0.0;
                    double num7 = 0.0;
                    double width = 0.0;
                    double height = 0.0;
                    CellRange current = (CellRange)enumerator.Current;
                    for (int i = current.Row; i < row; i++)
                    {
                        num7 -= Math.Ceiling((double)(worksheet.GetActualRowHeight(i, SheetArea.ColumnHeader) * zoomFactor));
                    }
                    for (int j = row; j < current.Row; j++)
                    {
                        num7 += Math.Ceiling((double)(worksheet.GetActualRowHeight(j, SheetArea.ColumnHeader) * zoomFactor));
                    }
                    for (int k = current.Column; k < column; k++)
                    {
                        num6 -= Math.Ceiling((double)(worksheet.GetActualColumnWidth(k, SheetArea.Cells) * zoomFactor));
                    }
                    for (int m = column; m < current.Column; m++)
                    {
                        num6 += Math.Ceiling((double)(worksheet.GetActualColumnWidth(m, SheetArea.Cells) * zoomFactor));
                    }
                    for (int n = current.Row; n < (current.Row + current.RowCount); n++)
                    {
                        if (n < worksheet.ColumnHeader.RowCount)
                        {
                            height += Math.Ceiling((double)(worksheet.GetActualRowHeight(n, SheetArea.ColumnHeader) * zoomFactor));
                        }
                    }
                    for (int num15 = current.Column; num15 < (current.Column + current.ColumnCount); num15++)
                    {
                        if (num15 < worksheet.ColumnCount)
                        {
                            width += Math.Ceiling((double)(worksheet.GetActualColumnWidth(num15, SheetArea.Cells) * zoomFactor));
                        }
                    }
                    model3.Add(new CellLayout(current.Row, current.Column, current.RowCount, current.ColumnCount, viewportColumnLayoutModel[0].X + num6, columnHeaderRowLayoutModel[0].Y + num7, width, height));
                }
            }
            return model3;
        }

        RowLayoutModel CreateColumnHeaderRowLayoutModel()
        {
            RowLayoutModel model = new RowLayoutModel();
            SheetLayout layout = GetSheetLayout();
            var activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = ZoomFactor;
                double headerY = layout.HeaderY;
                for (int i = 0; i < activeSheet.ColumnHeader.RowCount; i++)
                {
                    double height = Math.Ceiling((double)(activeSheet.GetActualRowHeight(i, SheetArea.ColumnHeader) * zoomFactor));
                    model.Add(new RowLayout(i, headerY, height));
                    headerY += height;
                }
            }
            return model;
        }

        ColumnLayoutModel CreateEnhancedResizeToZeroColumnHeaderViewportColumnLayoutModel(int columnViewportIndex)
        {
            if (ResizeZeroIndicator == ResizeZeroIndicator.Default)
            {
                return CreateViewportColumnLayoutModel(columnViewportIndex);
            }
            SheetLayout layout = GetSheetLayout();
            ColumnLayoutModel model = new ColumnLayoutModel();
            var activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = ZoomFactor;
                if (columnViewportIndex == -1)
                {
                    int frozenColumnCount = activeSheet.FrozenColumnCount;
                    if (frozenColumnCount > activeSheet.ColumnCount)
                    {
                        frozenColumnCount = activeSheet.ColumnCount;
                    }
                    double x = layout.HeaderX + layout.HeaderWidth;
                    for (int i = 0; i < frozenColumnCount; i++)
                    {
                        double width = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(i, x, width));
                        x += width;
                    }
                    return model;
                }
                if ((columnViewportIndex >= 0) && (columnViewportIndex < layout.ColumnPaneCount))
                {
                    double viewportWidth = layout.GetViewportWidth(columnViewportIndex);
                    int viewportLeftColumn = GetViewportLeftColumn(columnViewportIndex);
                    if ((viewportLeftColumn > 0) && activeSheet.GetActualColumnWidth(viewportLeftColumn - 1, SheetArea.Cells).IsZero())
                    {
                        viewportLeftColumn--;
                    }
                    int num8 = (activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount) - 1;
                    int num9 = (num8 - viewportLeftColumn) + 1;
                    HashSet<int> set = new HashSet<int>();
                    HashSet<int> set2 = new HashSet<int>();
                    Dictionary<int, double> dictionary = new Dictionary<int, double>();
                    for (int j = viewportLeftColumn; ((viewportWidth > 0.0) && (j != -1)) && (j <= num8); j++)
                    {
                        dictionary.Add(j, Math.Ceiling((double)(activeSheet.GetActualColumnWidth(j, SheetArea.Cells) * zoomFactor)));
                    }
                    for (int k = viewportLeftColumn; ((viewportWidth > 0.0) && (k != -1)) && (k <= num8); k++)
                    {
                        int num12 = -1;
                        double minValue = dictionary[k];
                        while (minValue.IsZero())
                        {
                            set2.Add(k);
                            num12 = k;
                            k++;
                            if (k <= num8)
                            {
                                minValue = dictionary[k];
                            }
                            else
                            {
                                minValue = double.MinValue;
                            }
                        }
                        if (num12 != -1)
                        {
                            if (num9 != set2.Count)
                            {
                                set.Add(num12);
                            }
                            k--;
                        }
                    }
                    for (int m = viewportLeftColumn; ((viewportWidth > 0.0) && (m != -1)) && (m <= num8); m++)
                    {
                        double num15 = dictionary[m];
                        if (set.Contains(m))
                        {
                            int num16 = m - 1;
                            int num17 = m + 1;
                            while (true)
                            {
                                if (!set2.Contains(num16))
                                {
                                    break;
                                }
                                num16--;
                            }
                            while (set2.Contains(num17))
                            {
                                num17++;
                            }
                            if ((num16 >= viewportLeftColumn) && (num17 <= num8))
                            {
                                double num18 = dictionary[num16];
                                double num19 = dictionary[num17];
                                num15 = Math.Min(num18, 3.0) + Math.Min(num19, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num18 - 3.0));
                                dictionary[num17] = Math.Max((double)0.0, (double)(num19 - 3.0));
                            }
                            else if ((num16 < viewportLeftColumn) && (num17 <= num8))
                            {
                                double num20 = dictionary[num17];
                                num15 = Math.Min(num20, 3.0);
                                dictionary[num17] = Math.Max((double)0.0, (double)(num20 - 3.0));
                            }
                            else if ((num16 >= viewportLeftColumn) && (num17 > num8))
                            {
                                double num21 = dictionary[num16];
                                num15 = Math.Min(num21, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num21 - 3.0));
                            }
                            dictionary[m] = num15;
                        }
                        viewportWidth -= num15;
                    }
                    viewportWidth = layout.GetViewportWidth(columnViewportIndex);
                    double viewportX = layout.GetViewportX(columnViewportIndex);
                    for (int n = viewportLeftColumn; ((viewportWidth > 0.0) && (n != -1)) && (n <= num8); n++)
                    {
                        double num24 = dictionary[n];
                        model.Add(new ColumnLayout(n, viewportX, num24));
                        viewportX += num24;
                        viewportWidth -= num24;
                    }
                    return model;
                }
                if (columnViewportIndex == layout.ColumnPaneCount)
                {
                    double num25 = layout.GetViewportX(layout.ColumnPaneCount - 1) + layout.GetViewportWidth(layout.ColumnPaneCount - 1);
                    if ((IsTouching && (ActiveSheet.FrozenTrailingColumnCount > 0)) && ((_touchStartHitTestInfo.ColumnViewportIndex == (layout.ColumnPaneCount - 1)) && (_translateOffsetX < 0.0)))
                    {
                        num25 += _translateOffsetX;
                    }
                    for (int num26 = Math.Max(activeSheet.FrozenColumnCount, activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount); num26 < activeSheet.ColumnCount; num26++)
                    {
                        double num27 = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(num26, SheetArea.Cells) * zoomFactor));
                        if ((num27 == 0.0) && (ResizeZeroIndicator == ResizeZeroIndicator.Enhanced))
                        {
                            num27 = 4.0;
                        }
                        model.Add(new ColumnLayout(num26, num25, num27));
                        num25 += num27;
                    }
                }
            }
            return model;
        }

        RowLayoutModel CreateEnhancedResizeToZeroRowHeaderViewportRowLayoutModel(int rowViewportIndex)
        {
            if (ResizeZeroIndicator == ResizeZeroIndicator.Default)
            {
                return CreateViewportRowLayoutModel(rowViewportIndex);
            }
            RowLayoutModel model = new RowLayoutModel();
            SheetLayout layout = GetSheetLayout();
            var activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = ZoomFactor;
                if (rowViewportIndex == -1)
                {
                    double y = layout.HeaderY + layout.HeaderHeight;
                    int frozenRowCount = ActiveSheet.FrozenRowCount;
                    if (ActiveSheet.RowCount < frozenRowCount)
                    {
                        frozenRowCount = ActiveSheet.RowCount;
                    }
                    for (int i = 0; i < frozenRowCount; i++)
                    {
                        double height = Math.Ceiling((double)(activeSheet.GetActualRowHeight(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(i, y, height));
                        y += height;
                    }
                    return model;
                }
                if ((rowViewportIndex >= 0) && (rowViewportIndex < layout.RowPaneCount))
                {
                    double viewportHeight = layout.GetViewportHeight(rowViewportIndex);
                    int viewportTopRow = GetViewportTopRow(rowViewportIndex);
                    if ((viewportTopRow > 0) && activeSheet.GetActualRowHeight(viewportTopRow - 1, SheetArea.Cells).IsZero())
                    {
                        viewportTopRow--;
                    }
                    int num8 = (activeSheet.RowCount - activeSheet.FrozenTrailingRowCount) - 1;
                    int num9 = (num8 - viewportTopRow) + 1;
                    HashSet<int> set = new HashSet<int>();
                    HashSet<int> set2 = new HashSet<int>();
                    Dictionary<int, double> dictionary = new Dictionary<int, double>();
                    for (int j = viewportTopRow; ((viewportHeight > 0.0) && (j != -1)) && (j <= num8); j++)
                    {
                        dictionary.Add(j, Math.Ceiling((double)(activeSheet.GetActualRowHeight(j, SheetArea.Cells) * zoomFactor)));
                    }
                    for (int k = viewportTopRow; ((viewportHeight > 0.0) && (k != -1)) && (k <= num8); k++)
                    {
                        int num12 = -1;
                        double minValue = dictionary[k];
                        while (minValue.IsZero())
                        {
                            set2.Add(k);
                            num12 = k;
                            k++;
                            if (k <= num8)
                            {
                                minValue = dictionary[k];
                            }
                            else
                            {
                                minValue = double.MinValue;
                            }
                        }
                        if (num12 != -1)
                        {
                            if (num9 != set2.Count)
                            {
                                set.Add(num12);
                            }
                            k--;
                        }
                    }
                    for (int m = viewportTopRow; ((viewportHeight > 0.0) && (m != -1)) && (m <= num8); m++)
                    {
                        double num15 = dictionary[m];
                        if (set.Contains(m))
                        {
                            int num16 = m - 1;
                            int num17 = m + 1;
                            while (true)
                            {
                                if (!set2.Contains(num16))
                                {
                                    break;
                                }
                                num16--;
                            }
                            while (set2.Contains(num17))
                            {
                                num17++;
                            }
                            if ((num16 >= viewportTopRow) && (num17 <= num8))
                            {
                                double num18 = dictionary[num16];
                                double num19 = dictionary[num17];
                                num15 = Math.Min(num18, 3.0) + Math.Min(num19, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num18 - 3.0));
                                dictionary[num17] = Math.Max((double)0.0, (double)(num19 - 3.0));
                            }
                            else if ((num16 < viewportTopRow) && (num17 <= num8))
                            {
                                double num20 = dictionary[num17];
                                num15 = Math.Min(num20, 3.0);
                                dictionary[num17] = Math.Max((double)0.0, (double)(num20 - 3.0));
                            }
                            else if ((num16 >= viewportTopRow) && (num17 > num8))
                            {
                                double num21 = dictionary[num16];
                                num15 = Math.Min(num21, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num21 - 3.0));
                            }
                            dictionary[m] = num15;
                        }
                        viewportHeight -= num15;
                    }
                    double viewportY = layout.GetViewportY(rowViewportIndex);
                    viewportHeight = layout.GetViewportHeight(rowViewportIndex);
                    for (int n = viewportTopRow; ((viewportHeight > 0.0) && (n != -1)) && (n <= num8); n++)
                    {
                        double num24 = dictionary[n];
                        model.Add(new RowLayout(n, viewportY, num24));
                        viewportY += num24;
                        viewportHeight -= num24;
                    }
                    return model;
                }
                if (rowViewportIndex == layout.RowPaneCount)
                {
                    double num25 = layout.GetViewportY(layout.RowPaneCount - 1) + layout.GetViewportHeight(layout.RowPaneCount - 1);
                    if ((IsTouching && (ActiveSheet.FrozenTrailingColumnCount > 0)) && ((_touchStartHitTestInfo.RowViewportIndex == (layout.RowPaneCount - 1)) && (_translateOffsetY < 0.0)))
                    {
                        num25 += _translateOffsetY;
                    }
                    for (int num26 = Math.Max(activeSheet.FrozenRowCount, activeSheet.RowCount - activeSheet.FrozenTrailingRowCount); num26 < activeSheet.RowCount; num26++)
                    {
                        double num27 = Math.Ceiling((double)(activeSheet.GetActualRowHeight(num26, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(num26, num25, num27));
                        num25 += num27;
                    }
                }
            }
            return model;
        }


        FilterButtonInfoModel CreateFilterButtonInfoModel()
        {
            FilterButtonInfoModel model = new FilterButtonInfoModel();
            Worksheet worksheet = ActiveSheet;
            if (worksheet != null)
            {
                HideRowFilter rowFilter = worksheet.RowFilter as HideRowFilter;
                if (((rowFilter != null) && (rowFilter.Range != null)) && rowFilter.ShowFilterButton)
                {
                    CellRange range = rowFilter.Range;
                    if (range.Row < 1)
                    {
                        int num = (range.Column < 0) ? 0 : range.Column;
                        int num2 = (range.Column < 0) ? (worksheet.ColumnCount - 1) : ((range.Column + range.ColumnCount) - 1);
                        int row = worksheet.ColumnHeader.RowCount - 1;
                        if (row >= 0)
                        {
                            int column = num;
                            while (column <= num2)
                            {
                                FilterButtonInfo info = new FilterButtonInfo(rowFilter)
                                {
                                    SheetArea = SheetArea.ColumnHeader,
                                    Row = row
                                };
                                CellRange range2 = worksheet.GetSpanCell(row, column, SheetArea.ColumnHeader);
                                if (range2 != null)
                                {
                                    info.Row = range2.Row;
                                    info.Column = range2.Column;
                                    column += range2.ColumnCount;
                                }
                                else
                                {
                                    info.Column = column;
                                    column++;
                                }
                                model.Add(info);
                            }
                        }
                    }
                    else
                    {
                        int num5 = (range.Column < 0) ? 0 : range.Column;
                        int num6 = (range.Column < 0) ? (worksheet.ColumnCount - 1) : ((range.Column + range.ColumnCount) - 1);
                        int num7 = range.Row - 1;
                        int num8 = num5;
                        while (num8 <= num6)
                        {
                            FilterButtonInfo info2 = new FilterButtonInfo(rowFilter)
                            {
                                SheetArea = SheetArea.Cells,
                                Row = num7
                            };
                            CellRange range3 = worksheet.GetSpanCell(num7, num8, SheetArea.Cells);
                            if (range3 != null)
                            {
                                info2.Row = range3.Row;
                                info2.Column = range3.Column;
                                num8 += range3.ColumnCount;
                            }
                            else
                            {
                                info2.Column = num8;
                                num8++;
                            }
                            model.Add(info2);
                        }
                    }
                }
                foreach (SheetTable table in ActiveSheet.GetTables())
                {
                    if (((table != null) && table.ShowHeader) && table.RowFilter.ShowFilterButton)
                    {
                        int headerIndex = table.HeaderIndex;
                        for (int i = 0; i < table.Range.ColumnCount; i++)
                        {
                            int num11 = table.Range.Column + i;
                            FilterButtonInfo info3 = new FilterButtonInfo(table.RowFilter as HideRowFilter, headerIndex, num11, SheetArea.Cells);
                            model.Add(info3);
                        }
                    }
                }
            }
            return model;
        }

        internal Line CreateFreezeLine()
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            Line line2 = new Line();
            line2.StrokeThickness = 1.0;
            line2.Stroke = brush;
            Line element = line2;
            element.TypeSafeSetStyle(FreezeLineStyle);
            return element;
        }

        internal GroupLayout CreateGroupLayout()
        {
            Worksheet worksheet = ActiveSheet;
            GroupLayout layout = new GroupLayout
            {
                X = 0.0,
                Y = 0.0
            };
            if (worksheet != null)
            {
                if (ShowRowRangeGroup && (worksheet.RowRangeGroup != null))
                {
                    int maxLevel = worksheet.RowRangeGroup.GetMaxLevel();
                    if (maxLevel >= 0)
                    {
                        double num2 = Math.Min((double)16.0, (double)(16.0 * ZoomFactor));
                        layout.Width = (num2 * (maxLevel + 2)) + 4.0;
                    }
                }
                if (ShowColumnRangeGroup && (worksheet.ColumnRangeGroup != null))
                {
                    int num3 = worksheet.ColumnRangeGroup.GetMaxLevel();
                    if (num3 >= 0)
                    {
                        double num4 = Math.Min((double)16.0, (double)(16.0 * ZoomFactor));
                        layout.Height = (num4 * (num3 + 2)) + 4.0;
                    }
                }
            }
            return layout;
        }

        internal CellLayoutModel CreateRowHeaderCellLayoutModel(int rowViewportIndex)
        {
            ColumnLayoutModel rowHeaderColumnLayoutModel = GetRowHeaderColumnLayoutModel();
            RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(rowViewportIndex);
            CellLayoutModel model3 = new CellLayoutModel();
            if ((rowHeaderColumnLayoutModel.Count > 0) && (viewportRowLayoutModel.Count > 0))
            {
                Worksheet worksheet = ActiveSheet;
                int row = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)viewportRowLayoutModel, 0).Row;
                int column = rowHeaderColumnLayoutModel[0].Column;
                int num3 = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)viewportRowLayoutModel, viewportRowLayoutModel.Count - 1).Row;
                int num4 = rowHeaderColumnLayoutModel[rowHeaderColumnLayoutModel.Count - 1].Column;
                IEnumerator enumerator = ActiveSheet.RowHeaderSpanModel.GetEnumerator(row, column, (num3 - row) + 1, (num4 - column) + 1);
                float zoomFactor = ZoomFactor;
                while (enumerator.MoveNext())
                {
                    double num6 = 0.0;
                    double num7 = 0.0;
                    double width = 0.0;
                    double height = 0.0;
                    CellRange current = (CellRange)enumerator.Current;
                    for (int i = current.Row; i < row; i++)
                    {
                        num7 -= Math.Ceiling((double)(worksheet.GetActualRowHeight(i, SheetArea.Cells) * zoomFactor));
                    }
                    for (int j = row; j < current.Row; j++)
                    {
                        num7 += Math.Ceiling((double)(worksheet.GetActualRowHeight(j, SheetArea.Cells) * zoomFactor));
                    }
                    for (int k = current.Column; k < column; k++)
                    {
                        num6 -= Math.Ceiling((double)(worksheet.GetActualColumnWidth(k, SheetArea.CornerHeader | SheetArea.RowHeader) * zoomFactor));
                    }
                    for (int m = column; m < current.Column; m++)
                    {
                        num6 += Math.Ceiling((double)(worksheet.GetActualColumnWidth(m, SheetArea.CornerHeader | SheetArea.RowHeader) * zoomFactor));
                    }
                    for (int n = current.Row; n < (current.Row + current.RowCount); n++)
                    {
                        if (n < worksheet.RowCount)
                        {
                            height += Math.Ceiling((double)(worksheet.GetActualRowHeight(n, SheetArea.Cells) * zoomFactor));
                        }
                    }
                    for (int num15 = current.Column; num15 < (current.Column + current.ColumnCount); num15++)
                    {
                        if (num15 < worksheet.RowHeader.ColumnCount)
                        {
                            width += Math.Ceiling((double)(worksheet.GetActualColumnWidth(num15, SheetArea.CornerHeader | SheetArea.RowHeader) * zoomFactor));
                        }
                    }
                    model3.Add(new CellLayout(current.Row, current.Column, current.RowCount, current.ColumnCount, rowHeaderColumnLayoutModel[0].X + num6, viewportRowLayoutModel[0].Y + num7, width, height));
                }
            }
            return model3;
        }

        ColumnLayoutModel CreateRowHeaderColumnLayoutModel()
        {
            ColumnLayoutModel model = new ColumnLayoutModel();
            SheetLayout layout = GetSheetLayout();
            var activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = ZoomFactor;
                double headerX = layout.HeaderX;
                for (int i = 0; i < activeSheet.RowHeader.ColumnCount; i++)
                {
                    double width = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(i, SheetArea.CornerHeader | SheetArea.RowHeader) * zoomFactor));
                    model.Add(new ColumnLayout(i, headerX, width));
                    headerX += width;
                }
            }
            return model;
        }

        internal CellLayoutModel CreateViewportCellLayoutModel(int rowViewportIndex, int columnViewportIndex)
        {
            ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(columnViewportIndex);
            RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(rowViewportIndex);
            CellLayoutModel model3 = new CellLayoutModel();
            if ((viewportColumnLayoutModel.Count > 0) && (viewportRowLayoutModel.Count > 0))
            {
                int row = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)viewportRowLayoutModel, 0).Row;
                int column = viewportColumnLayoutModel[0].Column;
                int num3 = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)viewportRowLayoutModel, viewportRowLayoutModel.Count - 1).Row;
                int num4 = viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column;
                IEnumerator enumerator = ActiveSheet.SpanModel.GetEnumerator(row, column, (num3 - row) + 1, (num4 - column) + 1);
                Worksheet worksheet = ActiveSheet;
                SheetArea cells = SheetArea.Cells;
                float zoomFactor = ZoomFactor;
                Dictionary<int, double> dictionary = new Dictionary<int, double>();
                Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
                while (enumerator.MoveNext() && (worksheet != null))
                {
                    double num6 = 0.0;
                    double num7 = 0.0;
                    double width = 0.0;
                    double height = 0.0;
                    CellRange current = (CellRange)enumerator.Current;
                    for (int i = current.Row; i < row; i++)
                    {
                        double num11 = 0.0;
                        if (!dictionary2.TryGetValue(i, out num11))
                        {
                            num11 = Math.Ceiling((double)(worksheet.GetActualRowHeight(i, cells) * zoomFactor));
                            dictionary2[i] = num11;
                        }
                        num7 -= num11;
                    }
                    for (int j = row; j < current.Row; j++)
                    {
                        double num13 = 0.0;
                        if (!dictionary2.TryGetValue(j, out num13))
                        {
                            num13 = Math.Ceiling((double)(worksheet.GetActualRowHeight(j, cells) * zoomFactor));
                            dictionary2[j] = num13;
                        }
                        num7 += num13;
                    }
                    for (int k = current.Column; k < column; k++)
                    {
                        double num15 = 0.0;
                        if (!dictionary.TryGetValue(k, out num15))
                        {
                            num15 = Math.Ceiling((double)(worksheet.GetActualColumnWidth(k, cells) * zoomFactor));
                            dictionary.Add(k, num15);
                        }
                        num6 -= num15;
                    }
                    for (int m = column; m < current.Column; m++)
                    {
                        double num17 = 0.0;
                        if (!dictionary.TryGetValue(m, out num17))
                        {
                            num17 = Math.Ceiling((double)(worksheet.GetActualColumnWidth(m, cells) * zoomFactor));
                            dictionary.Add(m, num17);
                        }
                        num6 += num17;
                    }
                    for (int n = current.Row; n < (current.Row + current.RowCount); n++)
                    {
                        if (n < worksheet.RowCount)
                        {
                            double num19 = 0.0;
                            if (!dictionary2.TryGetValue(n, out num19))
                            {
                                num19 = Math.Ceiling((double)(worksheet.GetActualRowHeight(n, cells) * zoomFactor));
                                dictionary2[n] = num19;
                            }
                            height += num19;
                        }
                    }
                    for (int num20 = current.Column; num20 < (current.Column + current.ColumnCount); num20++)
                    {
                        if (num20 < worksheet.ColumnCount)
                        {
                            double num21 = 0.0;
                            if (!dictionary.TryGetValue(num20, out num21))
                            {
                                num21 = Math.Ceiling((double)(worksheet.GetActualColumnWidth(num20, cells) * zoomFactor));
                                dictionary.Add(num20, num21);
                            }
                            width += num21;
                        }
                    }
                    model3.Add(new CellLayout(current.Row, current.Column, current.RowCount, current.ColumnCount, viewportColumnLayoutModel[0].X + num6, viewportRowLayoutModel[0].Y + num7, width, height));
                }
            }
            return model3;
        }

        FloatingObjectLayoutModel CreateViewportChartShapeLayoutMode(int rowViewportIndex, int columnViewportIndex)
        {
            FloatingObjectLayoutModel model = new FloatingObjectLayoutModel();
            FloatingObject[] allFloatingObjects = GetAllFloatingObjects();
            if (allFloatingObjects.Length != 0)
            {
                SheetLayout sheetLayout = GetSheetLayout();
                double viewportX = sheetLayout.GetViewportX(columnViewportIndex);
                double viewportY = sheetLayout.GetViewportY(rowViewportIndex);
                Point viewportTopLeftCoordinates = GetViewportTopLeftCoordinates(rowViewportIndex, columnViewportIndex);
                for (int i = 0; i < allFloatingObjects.Length; i++)
                {
                    FloatingObject obj2 = allFloatingObjects[i];
                    double x = 0.0;
                    for (int j = 0; j < obj2.StartColumn; j++)
                    {
                        double num6 = Math.Ceiling((double)(ActiveSheet.GetActualColumnWidth(j, SheetArea.Cells) * ZoomFactor));
                        x += num6;
                    }
                    x += obj2.StartColumnOffset * ZoomFactor;
                    double y = 0.0;
                    for (int k = 0; k < obj2.StartRow; k++)
                    {
                        double num9 = Math.Ceiling((double)(ActiveSheet.GetActualRowHeight(k, SheetArea.Cells) * ZoomFactor));
                        y += num9;
                    }
                    y += obj2.StartRowOffset * ZoomFactor;
                    double with = Math.Ceiling((double)(obj2.Size.Width * ZoomFactor));
                    double height = Math.Ceiling((double)(obj2.Size.Height * ZoomFactor));
                    x -= viewportTopLeftCoordinates.X;
                    y -= viewportTopLeftCoordinates.Y;
                    x += viewportX;
                    y += viewportY;
                    model.Add(new FloatingObjectLayout(obj2.Name, x, y, with, height));
                }
            }
            return model;
        }

        ColumnLayoutModel CreateViewportColumnLayoutModel(int columnViewportIndex)
        {
            SheetLayout layout = GetSheetLayout();
            ColumnLayoutModel model = new ColumnLayoutModel();
            var activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = ZoomFactor;
                if (columnViewportIndex == -1)
                {
                    int frozenColumnCount = activeSheet.FrozenColumnCount;
                    if (frozenColumnCount > activeSheet.ColumnCount)
                    {
                        frozenColumnCount = activeSheet.ColumnCount;
                    }
                    double x = layout.HeaderX + layout.HeaderWidth;
                    for (int i = 0; i < frozenColumnCount; i++)
                    {
                        double width = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(i, x, width));
                        x += width;
                    }
                    return model;
                }
                if ((columnViewportIndex >= 0) && (columnViewportIndex < layout.ColumnPaneCount))
                {
                    double viewportX = layout.GetViewportX(columnViewportIndex);
                    double viewportWidth = layout.GetViewportWidth(columnViewportIndex);
                    for (int j = GetViewportLeftColumn(columnViewportIndex); ((viewportWidth > 0.0) && (j != -1)) && (j < (activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount)); j++)
                    {
                        double num9 = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(j, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(j, viewportX, num9));
                        viewportX += num9;
                        viewportWidth -= num9;
                    }
                    return model;
                }
                if (columnViewportIndex == layout.ColumnPaneCount)
                {
                    double num10 = layout.GetViewportX(layout.ColumnPaneCount - 1) + layout.GetViewportWidth(layout.ColumnPaneCount - 1);
                    if ((IsTouching && (ActiveSheet.FrozenTrailingColumnCount > 0)) && ((_touchStartHitTestInfo.ColumnViewportIndex == (layout.ColumnPaneCount - 1)) && (_translateOffsetX < 0.0)))
                    {
                        num10 += _translateOffsetX;
                    }
                    for (int k = Math.Max(activeSheet.FrozenColumnCount, activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount); k < activeSheet.ColumnCount; k++)
                    {
                        double num12 = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(k, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(k, num10, num12));
                        num10 += num12;
                    }
                }
            }
            return model;
        }

        RowLayoutModel CreateViewportRowLayoutModel(int rowViewportIndex)
        {
            RowLayoutModel model = new RowLayoutModel();
            SheetLayout layout = GetSheetLayout();
            var activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = ZoomFactor;
                if (rowViewportIndex == -1)
                {
                    double y = layout.HeaderY + layout.HeaderHeight;
                    int frozenRowCount = ActiveSheet.FrozenRowCount;
                    if (ActiveSheet.RowCount < frozenRowCount)
                    {
                        frozenRowCount = ActiveSheet.RowCount;
                    }
                    for (int i = 0; i < frozenRowCount; i++)
                    {
                        double height = Math.Ceiling((double)(activeSheet.GetActualRowHeight(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(i, y, height));
                        y += height;
                    }
                    return model;
                }
                if ((rowViewportIndex >= 0) && (rowViewportIndex < layout.RowPaneCount))
                {
                    double viewportY = layout.GetViewportY(rowViewportIndex);
                    double viewportHeight = layout.GetViewportHeight(rowViewportIndex);
                    int rowCount = activeSheet.RowCount;
                    for (int j = GetViewportTopRow(rowViewportIndex); ((viewportHeight > 0.0) && (j != -1)) && (j < (rowCount - activeSheet.FrozenTrailingRowCount)); j++)
                    {
                        double num10 = Math.Ceiling((double)(activeSheet.GetActualRowHeight(j, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(j, viewportY, num10));
                        viewportY += num10;
                        viewportHeight -= num10;
                    }
                    return model;
                }
                if (rowViewportIndex == layout.RowPaneCount)
                {
                    double num11 = layout.GetViewportY(layout.RowPaneCount - 1) + layout.GetViewportHeight(layout.RowPaneCount - 1);
                    if ((IsTouching && (ActiveSheet.FrozenTrailingColumnCount > 0)) && ((_touchStartHitTestInfo.RowViewportIndex == (layout.RowPaneCount - 1)) && (_translateOffsetY < 0.0)))
                    {
                        num11 += _translateOffsetY;
                    }
                    for (int k = Math.Max(activeSheet.FrozenRowCount, activeSheet.RowCount - activeSheet.FrozenTrailingRowCount); k < activeSheet.RowCount; k++)
                    {
                        double num13 = Math.Ceiling((double)(activeSheet.GetActualRowHeight(k, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(k, num11, num13));
                        num11 += num13;
                    }
                }
            }
            return model;
        }

        internal bool DoCommand(ICommand command)
        {
            return UndoManager.Do(command);
        }

        void DoContinueDragDropping()
        {
            UpdateMouseCursorLocation();
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(_dragStartRowViewport, MousePosition.Y);
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(_dragStartColumnViewport, MousePosition.X);
            if (((viewportRowLayoutNearY != null) && (viewportColumnLayoutNearX != null)) && ((viewportRowLayoutNearY.Height > 0.0) && (viewportColumnLayoutNearX.Width > 0.0)))
            {
                bool flag;
                bool flag2;
                int row = viewportRowLayoutNearY.Row;
                int column = viewportColumnLayoutNearX.Column;
                int rowViewportIndex = _dragStartRowViewport;
                int columnViewportIndex = _dragStartColumnViewport;
                if (GetViewportRowLayoutModel(rowViewportIndex).FindRow(row) == null)
                {
                    double y = GetHitInfo().HitPoint.Y;
                    int rowViewportCount = GetViewportInfo().RowViewportCount;
                    if (MousePosition.Y < y)
                    {
                        if ((_dragStartRowViewport == 0) && (row < ActiveSheet.FrozenRowCount))
                        {
                            rowViewportIndex = -1;
                        }
                        else if ((_dragStartRowViewport == rowViewportCount) && (row < (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
                        {
                            rowViewportIndex = rowViewportCount - 1;
                        }
                    }
                    else if ((_dragStartRowViewport == -1) && (row >= ActiveSheet.FrozenRowCount))
                    {
                        rowViewportIndex = 0;
                    }
                    else if ((_dragStartRowViewport == (rowViewportCount - 1)) && (row >= (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
                    {
                        rowViewportIndex = rowViewportCount;
                    }
                }
                if (GetViewportColumnLayoutModel(columnViewportIndex).FindColumn(column) == null)
                {
                    double x = GetHitInfo().HitPoint.X;
                    int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                    if (MousePosition.X < x)
                    {
                        if ((_dragStartColumnViewport == 0) && (column < ActiveSheet.FrozenColumnCount))
                        {
                            columnViewportIndex = -1;
                        }
                        else if ((_dragStartColumnViewport == columnViewportCount) && (column < (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
                        {
                            columnViewportIndex = columnViewportCount - 1;
                        }
                    }
                    else if ((_dragStartColumnViewport == -1) && (column >= ActiveSheet.FrozenColumnCount))
                    {
                        columnViewportIndex = 0;
                    }
                    else if ((_dragStartColumnViewport == (columnViewportCount - 1)) && (column >= (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
                    {
                        columnViewportIndex = columnViewportCount;
                    }
                }
                _dragToRowViewport = rowViewportIndex;
                _dragToColumnViewport = columnViewportIndex;
                _dragToRow = row;
                _dragToColumn = column;
                KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                _isDragInsert = flag;
                _isDragCopy = flag2;
                if (_isDragInsert && ((_dragDropFromRange.Row == -1) || (_dragDropFromRange.Column == -1)))
                {
                    RefreshDragDropInsertIndicator(rowViewportIndex, columnViewportIndex, row, column);
                }
                else
                {
                    RefreshDragDropIndicator(rowViewportIndex, columnViewportIndex, row, column);
                }
            }
            ProcessScrollTimer();
        }

        void DoContinueDragFill()
        {
            UpdateDragToViewports();
            UpdateDragToCoordicates();
            if (((_dragToRow >= 0) || (_dragToColumn >= 0)) && !IsMouseInDragFillIndicator(MousePosition.X, MousePosition.Y, _dragStartRowViewport, _dragStartColumnViewport, false))
            {
                UpdateMouseCursorLocation();
                UpdateCurrentFillSettings();
                UpdateCurrentFillRange();
                RefreshDragFill();
                RefreshSelectionBorder();
                ProcessScrollTimer();
                int row = (_currentFillRange.Row + _currentFillRange.RowCount) - 1;
                int column = (_currentFillRange.Column + _currentFillRange.ColumnCount) - 1;
                FillDirection currentFillDirection = GetCurrentFillDirection();
                switch (currentFillDirection)
                {
                    case FillDirection.Left:
                    case FillDirection.Up:
                        row = _currentFillRange.Row;
                        column = _currentFillRange.Column;
                        break;
                }
                string str = ActiveSheet.GetFillText(row, column, GetDragAutoFillType(), currentFillDirection);
                if (str == null)
                {
                    TooltipHelper.CloseTooltip();
                }
                if (!string.IsNullOrWhiteSpace(str))
                {
                    Point point = ArrangeDragFillTooltip(_currentFillRange, currentFillDirection);
                    if (IsTouchDragFilling)
                    {
                        if (Excel.ShowDragFillTip)
                        {
                            TooltipHelper.ShowTooltip(str, point.X + 40.0, point.Y);
                        }
                    }
                    else if (Excel.ShowDragFillTip)
                    {
                        TooltipHelper.ShowTooltip(str, point.X, point.Y);
                    }
                }
            }
        }

        void DoEndDragDropping(ref bool isInvalid, ref string invalidMessage, ref bool doCommand)
        {
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(_dragStartRowViewport, MousePosition.Y);
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(_dragStartColumnViewport, MousePosition.X);
            if ((viewportRowLayoutNearY != null) && (viewportColumnLayoutNearX != null))
            {
                int row = _dragDropFromRange.Row;
                int column = _dragDropFromRange.Column;
                int rowCount = _dragDropFromRange.RowCount;
                int columnCount = _dragDropFromRange.ColumnCount;
                int toRow = (viewportRowLayoutNearY.Height > 0.0) ? viewportRowLayoutNearY.Row : _dragToRow;
                int toColumn = (viewportColumnLayoutNearX.Width > 0.0) ? viewportColumnLayoutNearX.Column : _dragToColumn;
                CellRange exceptedRange = _isDragCopy ? null : _dragDropFromRange;
                if (_isDragInsert && ((row == -1) || (column == -1)))
                {
                    if ((row < 0) || (column < 0))
                    {
                        if (column < 0)
                        {
                            if (row >= 0)
                            {
                                if (MousePosition.Y > (viewportRowLayoutNearY.Y + (viewportRowLayoutNearY.Height / 2.0)))
                                {
                                    toRow = Math.Min(ActiveSheet.RowCount, toRow + 1);
                                }
                                if ((_isDragCopy && ((toRow <= row) || (toRow >= (row + rowCount)))) || (!_isDragCopy && ((toRow < row) || (toRow > (row + rowCount)))))
                                {
                                    if (!RaiseValidationDragDropBlock(row, column, toRow, toColumn, rowCount, columnCount, _isDragCopy, true, out isInvalid, out invalidMessage))
                                    {
                                        if (HasPartSpans(ActiveSheet, row, -1, rowCount, -1) || HasPartSpans(ActiveSheet, toRow, -1, 0, -1))
                                        {
                                            isInvalid = true;
                                            invalidMessage = ResourceStrings.SheetViewDragDropChangePartOfMergedCell;
                                        }
                                        if (!isInvalid && (HasPartArrayFormulas(ActiveSheet, row, -1, rowCount, -1, exceptedRange) || HasPartArrayFormulas(ActiveSheet, toRow, -1, 0, -1, exceptedRange)))
                                        {
                                            isInvalid = true;
                                            invalidMessage = ResourceStrings.SheetViewPasteChangePartOfArrayFormula;
                                        }
                                        if (!isInvalid && ActiveSheet.Protect)
                                        {
                                            isInvalid = true;
                                            invalidMessage = ResourceStrings.SheetViewDragDropChangeProtectRow;
                                        }
                                        if ((!isInvalid && !_isDragCopy) && HasTable(ActiveSheet, row, -1, rowCount, -1, true))
                                        {
                                            isInvalid = true;
                                            invalidMessage = ResourceStrings.SheetViewDragDropChangePartOfTable;
                                        }
                                    }
                                    if (!isInvalid)
                                    {
                                        DragDropExtent dragMoveExtent = new DragDropExtent(row, -1, toRow, -1, rowCount, -1);
                                        CopyToOption all = CopyToOption.All;
                                        if (!RaiseDragDropBlock(dragMoveExtent.FromRow, dragMoveExtent.FromColumn, dragMoveExtent.ToRow, dragMoveExtent.ToColumn, dragMoveExtent.RowCount, dragMoveExtent.ColumnCount, _isDragCopy, true, CopyToOption.All, out all))
                                        {
                                            DragDropUndoAction command = new DragDropUndoAction(ActiveSheet, dragMoveExtent, _isDragCopy, true, all);
                                            DoCommand(command);
                                            RaiseDragDropBlockCompleted(dragMoveExtent.FromRow, dragMoveExtent.FromColumn, dragMoveExtent.ToRow, dragMoveExtent.ToColumn, dragMoveExtent.RowCount, dragMoveExtent.ColumnCount, _isDragCopy, true, all);
                                            doCommand = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (MousePosition.X > (viewportColumnLayoutNearX.X + (viewportColumnLayoutNearX.Width / 2.0)))
                            {
                                toColumn = Math.Min(ActiveSheet.ColumnCount, toColumn + 1);
                            }
                            if ((_isDragCopy && ((toColumn <= column) || (toColumn >= (column + columnCount)))) || (!_isDragCopy && ((toColumn < column) || (toColumn > (column + columnCount)))))
                            {
                                if (!RaiseValidationDragDropBlock(row, column, toRow, toColumn, rowCount, columnCount, _isDragCopy, true, out isInvalid, out invalidMessage))
                                {
                                    if (HasPartSpans(ActiveSheet, -1, column, -1, columnCount) || HasPartSpans(ActiveSheet, -1, toColumn, -1, 0))
                                    {
                                        isInvalid = true;
                                        invalidMessage = ResourceStrings.SheetViewDragDropChangePartOfMergedCell;
                                    }
                                    if (!isInvalid && (HasPartArrayFormulas(ActiveSheet, -1, column, -1, columnCount, exceptedRange) || HasPartArrayFormulas(ActiveSheet, -1, toColumn, -1, 0, exceptedRange)))
                                    {
                                        isInvalid = true;
                                        invalidMessage = ResourceStrings.SheetViewDragDropChangePartOChangePartOfAnArray;
                                    }
                                    if (!isInvalid && ActiveSheet.Protect)
                                    {
                                        isInvalid = true;
                                        invalidMessage = ResourceStrings.SheetViewDragDropChangeProtectColumn;
                                    }
                                    if (!isInvalid && HasTable(ActiveSheet, -1, toColumn, -1, 1, true))
                                    {
                                        isInvalid = true;
                                        invalidMessage = ResourceStrings.SheetViewDragDropShiftTableCell;
                                    }
                                    if ((!isInvalid && !_isDragCopy) && HasTable(ActiveSheet, -1, column, -1, columnCount, true))
                                    {
                                        isInvalid = true;
                                        invalidMessage = ResourceStrings.SheetViewDragDropChangePartOfTable;
                                    }
                                }
                                if (!isInvalid)
                                {
                                    DragDropExtent extent = new DragDropExtent(-1, column, -1, toColumn, -1, columnCount);
                                    CopyToOption newCopyOption = CopyToOption.All;
                                    if (!RaiseDragDropBlock(extent.FromRow, extent.FromColumn, extent.ToRow, extent.ToColumn, extent.RowCount, extent.ColumnCount, _isDragCopy, true, CopyToOption.All, out newCopyOption))
                                    {
                                        DragDropUndoAction action = new DragDropUndoAction(ActiveSheet, extent, _isDragCopy, true, newCopyOption);
                                        DoCommand(action);
                                        RaiseDragDropBlockCompleted(extent.FromRow, extent.FromColumn, extent.ToRow, extent.ToColumn, extent.RowCount, extent.ColumnCount, _isDragCopy, true, newCopyOption);
                                        doCommand = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    toRow = (_dragDropFromRange.Row < 0) ? -1 : Math.Max(0, Math.Min((int)(ActiveSheet.RowCount - rowCount), (int)(toRow - _dragDropRowOffset)));
                    toColumn = (_dragDropFromRange.Column < 0) ? -1 : Math.Max(0, Math.Min((int)(ActiveSheet.ColumnCount - columnCount), (int)(toColumn - _dragDropColumnOffset)));
                    if ((toRow != row) || (toColumn != column))
                    {
                        if (!RaiseValidationDragDropBlock(row, column, toRow, toColumn, rowCount, columnCount, _isDragCopy, true, out isInvalid, out invalidMessage))
                        {
                            if (HasPartSpans(ActiveSheet, row, column, rowCount, columnCount) || HasPartSpans(ActiveSheet, toRow, toColumn, rowCount, columnCount))
                            {
                                isInvalid = true;
                                invalidMessage = ResourceStrings.SheetViewDragDropChangePartOfMergedCell;
                            }
                            if (!isInvalid && (HasPartArrayFormulas(ActiveSheet, row, column, rowCount, columnCount, exceptedRange) || HasPartArrayFormulas(ActiveSheet, toRow, toColumn, rowCount, columnCount, exceptedRange)))
                            {
                                isInvalid = true;
                                invalidMessage = ResourceStrings.SheetViewPasteChangePartOfArrayFormula;
                            }
                            if ((!isInvalid && ActiveSheet.Protect) && ((!_isDragCopy && IsAnyCellInRangeLocked(ActiveSheet, row, column, rowCount, columnCount)) || IsAnyCellInRangeLocked(ActiveSheet, toRow, toColumn, rowCount, columnCount)))
                            {
                                isInvalid = true;
                                invalidMessage = ResourceStrings.SheetViewDragDropChangeProtectCell;
                            }
                        }
                        if (!isInvalid)
                        {
                            DragDropExtent extent3 = new DragDropExtent(row, column, toRow, toColumn, rowCount, columnCount);
                            CopyToOption option3 = CopyToOption.All;
                            if (!RaiseDragDropBlock(extent3.FromRow, extent3.FromColumn, extent3.ToRow, extent3.ToColumn, extent3.RowCount, extent3.ColumnCount, _isDragCopy, false, CopyToOption.All, out option3))
                            {
                                DragDropUndoAction action3 = new DragDropUndoAction(ActiveSheet, extent3, _isDragCopy, false, option3);
                                DoCommand(action3);
                                RaiseDragDropBlockCompleted(extent3.FromRow, extent3.FromColumn, extent3.ToRow, extent3.ToColumn, extent3.RowCount, extent3.ColumnCount, _isDragCopy, false, option3);
                                doCommand = true;
                            }
                        }
                    }
                }
            }
        }

        void DoubleClickStartCellEditing(int row, int column)
        {
            CellRange spanCell = ActiveSheet.GetSpanCell(row, column);
            if (spanCell != null)
            {
                row = spanCell.Row;
                column = spanCell.Column;
            }
            if ((row == _currentActiveRowIndex) && (column == _currentActiveColumnIndex))
            {
                object formula = ActiveSheet.GetValue(row, column);
                if (formula == null)
                {
                    formula = ActiveSheet.GetFormula(row, column);
                }
                EditorStatus enter = EditorStatus.Enter;
                if ((formula != null) && (formula.ToString() != ""))
                {
                    enter = EditorStatus.Edit;
                }
                StartCellEditing(false, null, enter);
            }
        }

        void DoubleTapStartCellEediting(int row, int column)
        {
            DoubleClickStartCellEditing(row, column);
        }

        void DragFillSmartTag_AutoFilterTypeChanged(object sender, EventArgs e)
        {
            if (IsDragFill)
            {
                DragFillSmartTag tag = sender as DragFillSmartTag;
                AutoFillType autoFilterType = tag.AutoFilterType;
                if (_preFillCellsInfo != null)
                {
                    try
                    {
                        SuspendFloatingObjectsInvalidate();
                        CellRange range = AdjustFillRange(_currentFillRange);
                        CopyMoveHelper.UndoCellsInfo(ActiveSheet, _preFillCellsInfo, range.Row, range.Column, SheetArea.Cells);
                    }
                    finally
                    {
                        ResumeFloatingObjectsInvalidate();
                    }
                }
                FillDirection currentFillDirection = GetCurrentFillDirection();
                if (!RaiseDragFillBlock(_currentFillRange, currentFillDirection, autoFilterType))
                {
                    DragFillExtent dragFillExtent = new DragFillExtent(_dragFillStartRange, _currentFillRange, autoFilterType, currentFillDirection);
                    DragFillUndoAction command = new DragFillUndoAction(ActiveSheet, dragFillExtent);
                    DoCommand(command);
                    RaiseDragFillBlockCompleted(dragFillExtent.FillRange, dragFillExtent.FillDirection, dragFillExtent.AutoFillType);
                }
            }
        }

        void DragFillSmartTagPopup_Closed(object sender, object e)
        {
            if (!IsDraggingFill)
            {
                _dragFillStartRange = null;
                _preFillCellsInfo = null;
                _currentFillDirection = DragFillDirection.Down;
                _currentFillRange = null;
                _dragFillPopup = null;
            }
        }

        void DragFillSmartTagPopup_Opened(object sender, EventArgs e)
        {
        }

        void EndDragDropping()
        {
            ResetMouseCursor();
            bool isInvalid = false;
            string invalidMessage = string.Empty;
            bool doCommand = false;
            if (IsDragDropping && (GetHitInfo().HitPoint != MousePosition))
            {
                DoEndDragDropping(ref isInvalid, ref invalidMessage, ref doCommand);
            }
            if (doCommand)
            {
                SetActiveportIndexAfterDragDrop();
            }
            IsDragDropping = false;
            ResetFlagasAfterDragDropping();
            StopScrollTimer();
            TooltipHelper.CloseTooltip();
            if (isInvalid)
            {
                RaiseInvalidOperation(invalidMessage, null, null);
            }
        }

        void EndDragFill()
        {
            if (_currentFillRange == null)
            {
                IsDraggingFill = false;
            }
            else if (!IsDraggingFill)
            {
                ResetDragFill();
            }
            else
            {
                IsDraggingFill = false;
                if (IsMouseInDragFillIndicator(MousePosition.X, MousePosition.Y, _dragStartRowViewport, _dragStartColumnViewport, false))
                {
                    ResetDragFill();
                }
                else
                {
                    CellRange dragFillFrameRange = GetDragFillFrameRange();
                    if (!ValidateFillRange(dragFillFrameRange))
                    {
                        ResetDragFill();
                        RefreshSelection();
                    }
                    else
                    {
                        AutoFillType dragAutoFillType = GetDragAutoFillType();
                        bool flag3 = ExecuteDragFillAction(_currentFillRange, dragAutoFillType);
                        if (flag3)
                        {
                            ResetDragFill();
                            string sheetViewDragFillInvalidOperation = ResourceStrings.SheetViewDragFillInvalidOperation;
                            RaiseInvalidOperation(sheetViewDragFillInvalidOperation, "DragFill", null);
                        }
                        if (!flag3 && IsDragFill)
                        {
                            ShowDragFillSmartTag(_currentFillRange, dragAutoFillType);
                            ResetDragFill();
                        }
                        else
                        {
                            ResetDragFill();
                        }
                        RefreshSelection();
                    }
                }
            }
        }

        void EndTouchDragDropping()
        {
            ResetMouseCursor();
            bool isInvalid = false;
            string invalidMessage = string.Empty;
            bool doCommand = false;
            if (IsTouchDrapDropping && (GetHitInfo().HitPoint != MousePosition))
            {
                DoEndDragDropping(ref isInvalid, ref invalidMessage, ref doCommand);
            }
            if (doCommand)
            {
                SetActiveportIndexAfterDragDrop();
            }
            ResetFlagasAfterDragDropping();
            StopScrollTimer();
            if (isInvalid)
            {
                RaiseInvalidOperation(invalidMessage, null, null);
            }
            TooltipHelper.CloseTooltip();
        }

        void EndTouchDragFill()
        {
            if (_currentFillRange != null)
            {
                if (!IsTouchDragFilling)
                {
                    ResetTouchDragFill();
                }
                else if (IsMouseInDragFillIndicator(MousePosition.X, MousePosition.Y, _dragStartRowViewport, _dragStartColumnViewport, false))
                {
                    ResetTouchDragFill();
                }
                else
                {
                    CellRange dragFillFrameRange = GetDragFillFrameRange();
                    if (!ValidateFillRange(dragFillFrameRange))
                    {
                        ResetTouchDragFill();
                        RefreshSelection();
                    }
                    else
                    {
                        AutoFillType dragAutoFillType = GetDragAutoFillType();
                        if (!ExecuteDragFillAction(_currentFillRange, dragAutoFillType) && IsTouchDragFilling)
                        {
                            ShowDragFillSmartTag(_currentFillRange, dragAutoFillType);
                        }
                        ResetTouchDragFill();
                        RefreshSelection();
                    }
                }
            }
        }

        bool ExecuteDragFillAction(CellRange fillRange, AutoFillType autoFillType)
        {
            CellRange range = AdjustFillRange(fillRange);
            object[,] objArray = ActiveSheet.FindFormulas(range.Row, range.Column, range.RowCount, range.ColumnCount);
            for (int i = 0; i < objArray.GetLength(0); i++)
            {
                string str = (string)(objArray[i, 1] as string);
                if (str.StartsWith("{"))
                {
                    return true;
                }
            }
            _preFillCellsInfo = new CopyMoveCellsInfo(range.RowCount, range.ColumnCount);
            CopyMoveHelper.SaveViewportInfo(ActiveSheet, _preFillCellsInfo, range.Row, range.Column, CopyToOption.All);
            FillDirection currentFillDirection = GetCurrentFillDirection();
            if (RaiseDragFillBlock(fillRange, currentFillDirection, autoFillType))
            {
                return true;
            }
            DragFillExtent dragFillExtent = new DragFillExtent(_dragFillStartRange, fillRange, autoFillType, currentFillDirection);
            DragFillUndoAction command = new DragFillUndoAction(ActiveSheet, dragFillExtent);
            if (!DoCommand(command))
            {
                command.Undo(this);
                return true;
            }
            RaiseDragFillBlockCompleted(dragFillExtent.FillRange, dragFillExtent.FillDirection, dragFillExtent.AutoFillType);
            return false;
        }

        void FilterPopup_Closed(object sender, object e)
        {
            FocusInternal();
            if (_hitFilterInfo != null)
            {
                UpdateHitFilterCellState();
                _hitFilterInfo.RowViewportIndex = -2;
                _hitFilterInfo.ColumnViewportIndex = -2;
                _hitFilterInfo = null;
            }
        }

        void FilterPopup_Opened(object sender, object e)
        {
            if (_hitFilterInfo != null)
            {
                UpdateHitFilterCellState();
            }
        }

        internal void FocusInternal()
        {
#if UWP || WASM
            // 手机上不设置输入焦点
            Excel.IsTabStop = true;

            if (_cellsPanels != null)
            {
                CellsPanel viewport = _cellsPanels[1, 1];
                if ((viewport != null) && !viewport.FocusContent())
                {
                    Excel.Focus(FocusState.Programmatic);
                }
            }
#endif
        }

        internal CellRange GetActiveCell()
        {
            int activeRowIndex = ActiveSheet.ActiveRowIndex;
            int activeColumnIndex = ActiveSheet.ActiveColumnIndex;
            CellRange range = new CellRange(activeRowIndex, activeColumnIndex, 1, 1);
            CellRange range2 = ActiveSheet.SpanModel.Find(activeRowIndex, activeColumnIndex);
            if (range2 != null)
            {
                range = range2;
            }
            return range;
        }

        internal int GetActiveColumnViewportIndex()
        {
            return ActiveSheet.GetActiveColumnViewportIndex();
        }

        internal int GetActiveRowViewportIndex()
        {
            return ActiveSheet.GetActiveRowViewportIndex();
        }

        internal FloatingObject[] GetAllFloatingObjects()
        {
            List<FloatingObject> list = new List<FloatingObject>();
            if (ActiveSheet != null)
            {
                if (ActiveSheet.FloatingObjects.Count > 0)
                {
                    list.AddRange((IEnumerable<FloatingObject>)ActiveSheet.FloatingObjects);
                }
                if (ActiveSheet.Pictures.Count > 0)
                {
                    foreach (Picture picture in ActiveSheet.Pictures)
                    {
                        list.Add(picture);
                    }
                }
                if (ActiveSheet.Charts.Count > 0)
                {
                    foreach (FloatingObject obj2 in ActiveSheet.Charts)
                    {
                        list.Add(obj2);
                    }
                }
            }
            return list.ToArray();
        }

        internal FloatingObject[] GetAllSelectedFloatingObjects()
        {
            List<FloatingObject> list = new List<FloatingObject>();
            foreach (FloatingObject obj2 in GetAllFloatingObjects())
            {
                if (obj2.IsSelected)
                {
                    list.Add(obj2);
                }
            }
            return list.ToArray();
        }

        Rect GetAutoFillIndicatorRect(CellsPanel vp, CellRange activeSelection)
        {
            SheetLayout sheetLayout = GetSheetLayout();
            double viewportX = sheetLayout.GetViewportX(vp.ColumnViewportIndex);
            double viewportY = sheetLayout.GetViewportY(vp.RowViewportIndex);
            Rect rangeBounds = vp._cachedSelectionFrameLayout;
            if (!vp.SelectionContainer.IsAnchorCellInSelection)
            {
                rangeBounds = vp._cachedFocusCellLayout;
            }
            if (vp.Sheet.ActiveSheet.Selections.Count > 0)
            {
                rangeBounds = vp.GetRangeBounds(activeSelection);
            }
            Rect rect3 = rangeBounds;
            return new Rect(((rect3.Width + viewportX) + rangeBounds.X) - 16.0, (rect3.Height + viewportY) + rangeBounds.Y, 16.0, 16.0);
        }

        FloatingObjectLayoutModel GetCacheFloatingObjectsMovingResizingLayoutModels(int rowViewport, int columnViewport)
        {
            return _cachedFloatingObjectMovingResizingLayoutModel[rowViewport + 1, columnViewport + 1];
        }

        Cell GetCanSelectedCell(int row, int column, int rowCount, int columnCount)
        {
            Worksheet worksheet = ActiveSheet;
            int num = (row < 0) ? 0 : row;
            int num2 = (column < 0) ? 0 : column;
            int num3 = (row < 0) ? worksheet.RowCount : (row + rowCount);
            int num4 = (column < 0) ? worksheet.ColumnCount : (column + columnCount);
            for (int i = num; i < num3; i++)
            {
                if (worksheet.GetActualRowVisible(i, SheetArea.Cells))
                {
                    for (int j = num2; j < num4; j++)
                    {
                        CellRange spanCell = worksheet.GetSpanCell(i, j);
                        if (spanCell == null)
                        {
                            if (worksheet.GetActualStyleInfo(i, j, SheetArea.Cells).Focusable && worksheet.GetActualColumnVisible(j, SheetArea.Cells))
                            {
                                return worksheet.Cells[i, j];
                            }
                            j++;
                        }
                        else
                        {
                            if (worksheet.GetActualStyleInfo(spanCell.Row, spanCell.Column, SheetArea.Cells).Focusable && (worksheet.GetActualColumnWidth(spanCell.Column, spanCell.ColumnCount, SheetArea.Cells) > 0.0))
                            {
                                return worksheet.Cells[spanCell.Row, spanCell.Column];
                            }
                            j = spanCell.Column + spanCell.ColumnCount;
                        }
                    }
                }
                i++;
            }
            return null;
        }

        Cell GetCanSelectedCellInColumn(int startRow, int column)
        {
            Worksheet worksheet = ActiveSheet;
            int row = startRow;
            while (row < worksheet.RowCount)
            {
                CellRange spanCell = worksheet.GetSpanCell(row, column);
                if (spanCell == null)
                {
                    if (worksheet.GetActualStyleInfo(row, column, SheetArea.Cells).Focusable && worksheet.GetActualRowVisible(row, SheetArea.Cells))
                    {
                        return worksheet.Cells[row, column];
                    }
                    row++;
                }
                else
                {
                    if (((spanCell.ColumnCount == 1) || ((spanCell.Row + spanCell.RowCount) == worksheet.RowCount)) && (worksheet.GetActualStyleInfo(spanCell.Row, column, SheetArea.Cells).Focusable && worksheet.GetActualRowVisible(spanCell.Row, SheetArea.Cells)))
                    {
                        return worksheet.Cells[spanCell.Row, column];
                    }
                    row = spanCell.Row + spanCell.RowCount;
                }
            }
            return null;
        }

        Cell GetCanSelectedCellInRow(int row, int startColumn)
        {
            Worksheet worksheet = ActiveSheet;
            int column = startColumn;
            while (column < worksheet.ColumnCount)
            {
                CellRange spanCell = worksheet.GetSpanCell(row, column);
                if (spanCell == null)
                {
                    if (worksheet.GetActualStyleInfo(row, column, SheetArea.Cells).Focusable && worksheet.GetActualColumnVisible(column, SheetArea.Cells))
                    {
                        return worksheet.Cells[row, column];
                    }
                    column++;
                }
                else
                {
                    if (((spanCell.RowCount == 1) || ((spanCell.Column + spanCell.ColumnCount) == worksheet.ColumnCount)) && (worksheet.GetActualStyleInfo(row, spanCell.Column, SheetArea.Cells).Focusable && worksheet.GetActualColumnVisible(spanCell.Column, SheetArea.Cells)))
                    {
                        return worksheet.Cells[row, spanCell.Column];
                    }
                    column = spanCell.Column + spanCell.ColumnCount;
                }
            }
            return null;
        }

        internal CellLayoutModel GetCellLayoutModel(int rowViewportIndex, int columnViewportIndex, SheetArea sheetArea)
        {
            switch (sheetArea)
            {
                case SheetArea.Cells:
                    return GetViewportCellLayoutModel(rowViewportIndex, columnViewportIndex);

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return GetRowHeaderCellLayoutModel(rowViewportIndex);

                case SheetArea.ColumnHeader:
                    return GetColumnHeaderCellLayoutModel(columnViewportIndex);
            }
            return null;
        }

        CellRange GetCellRangeEx(CellRange cellRange, ICellsSupport dataContext)
        {
            return new CellRange((cellRange.Row < 0) ? 0 : cellRange.Row, (cellRange.Column < 0) ? 0 : cellRange.Column, (cellRange.RowCount < 0) ? dataContext.Rows.Count : cellRange.RowCount, (cellRange.ColumnCount < 0) ? dataContext.Columns.Count : cellRange.ColumnCount);
        }

        internal double GetColumnAutoFitValue(int column, bool rowHeader)
        {
            string str = string.Empty;
            double num = -1.0;
            Worksheet worksheet = ActiveSheet;
            int rowCount = worksheet.RowCount;
            Cell cell = null;
            FontFamily fontFamily = Excel.FontFamily;
            object textFormattingMode = null;
            SheetArea sheetArea = rowHeader ? (SheetArea.CornerHeader | SheetArea.RowHeader) : SheetArea.Cells;
            IDictionary<MeasureInfo, Dictionary<string, object>> dictionary = (IDictionary<MeasureInfo, Dictionary<string, object>>)new Dictionary<MeasureInfo, Dictionary<string, object>>();
            int activeRowViewportIndex = GetActiveRowViewportIndex();
            int viewportTopRow = GetViewportTopRow(activeRowViewportIndex);
            if (viewportTopRow < 0)
            {
                viewportTopRow = 0;
            }
            int num5 = 500;
            for (int i = 0; i < rowCount; i++)
            {
                cell = rowHeader ? worksheet.RowHeader.Cells[i, column] : worksheet.Cells[i, column];
                string str2 = ActiveSheet.GetText(i, column, sheetArea);
                if (!string.IsNullOrEmpty(str2))
                {
                    CellRange range = worksheet.GetSpanCell(i, column, sheetArea);
                    if ((range == null) || ((range.Column >= column) && (range.ColumnCount <= 1)))
                    {
                        double height = 0.0;
                        if (range == null)
                        {
                            height = worksheet.GetRowHeight(i, sheetArea);
                        }
                        else
                        {
                            for (int j = 0; j < range.RowCount; j++)
                            {
                                height += worksheet.GetRowHeight(i, sheetArea);
                            }
                        }
                        Size maxSize = MeasureHelper.ConvertExcelCellSizeToTextSize(new Size(double.PositiveInfinity, height), 1.0);
                        if ((viewportTopRow <= i) && (i < (viewportTopRow + num5)))
                        {
                            num = Math.Max(num, MeasureCellText(cell, i, column, maxSize, fontFamily, textFormattingMode, base.UseLayoutRounding));
                        }
                        else
                        {
                            MeasureInfo info = new MeasureInfo(cell, maxSize);
                            if (dictionary.Keys.Contains(info))
                            {
                                str = (string)(dictionary[info]["t"] as string);
                                if (str2.Length > str.Length)
                                {
                                    dictionary[info]["t"] = str2;
                                    dictionary[info]["r"] = (int)i;
                                }
                            }
                            else
                            {
                                Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                                dictionary2.Add("t", str2);
                                dictionary2.Add("r", (int)i);
                                dictionary.Add(info, dictionary2);
                            }
                        }
                        if (range != null)
                        {
                            i += range.RowCount - 1;
                        }
                    }
                }
            }
            foreach (MeasureInfo info2 in dictionary.Keys)
            {
                int row = (int)((int)dictionary[info2]["r"]);
                double rowHeight = 0.0;
                cell = rowHeader ? worksheet.RowHeader.Cells[row, column] : worksheet.Cells[row, column];
                CellRange range2 = worksheet.GetSpanCell(row, column, sheetArea);
                if (range2 == null)
                {
                    rowHeight = worksheet.GetRowHeight(row, sheetArea);
                }
                else
                {
                    for (int k = 0; k < range2.RowCount; k++)
                    {
                        rowHeight += worksheet.GetRowHeight(row, sheetArea);
                    }
                }
                Size size2 = MeasureHelper.ConvertExcelCellSizeToTextSize(new Size(double.PositiveInfinity, rowHeight), 1.0);
                num = Math.Max(num, MeasureCellText(cell, row, column, size2, fontFamily, textFormattingMode, base.UseLayoutRounding));
            }
            return num;
        }

        internal CellLayoutModel GetColumnHeaderCellLayoutModel(int columnViewportIndex)
        {
            int columnViewportCount = GetViewportInfo().ColumnViewportCount;
            if (_cachedColumnHeaderCellLayoutModel == null)
            {
                _cachedColumnHeaderCellLayoutModel = new CellLayoutModel[columnViewportCount + 2];
            }
            if (_cachedColumnHeaderCellLayoutModel[columnViewportIndex + 1] == null)
            {
                _cachedColumnHeaderCellLayoutModel[columnViewportIndex + 1] = CreateColumnHeaderCellLayoutModel(columnViewportIndex);
            }
            return _cachedColumnHeaderCellLayoutModel[columnViewportIndex + 1];
        }

        Rect GetColumnHeaderRectangle(int columnViewportIndex)
        {
            SheetLayout sheetLayout = GetSheetLayout();
            double viewportX = sheetLayout.GetViewportX(columnViewportIndex);
            double headerY = sheetLayout.HeaderY;
            double width = sheetLayout.GetViewportWidth(columnViewportIndex) - 1.0;
            double height = sheetLayout.HeaderHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(viewportX, headerY, width, height);
            }
            return Rect.Empty;
        }

        RowLayout GetColumnHeaderResizingRowLayoutFromY(double y)
        {
            Worksheet worksheet = ActiveSheet;
            if (worksheet.ColumnCount > 0)
            {
                RowLayoutModel columnHeaderRowLayoutModel = GetColumnHeaderRowLayoutModel();
                for (int i = columnHeaderRowLayoutModel.Count - 1; i >= 0; i--)
                {
                    RowLayout layout = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)columnHeaderRowLayoutModel, i);
                    if (((y >= Math.Max((layout.Y + layout.Height) - 4.0, layout.Y)) && (y < ((layout.Y + layout.Height) + 4.0))) && worksheet.ColumnHeader.Rows[layout.Row].CanUserResize)
                    {
                        return layout;
                    }
                }
            }
            return null;
        }

        RowLayout GetColumnHeaderRowLayout(int row)
        {
            return GetColumnHeaderRowLayoutModel().FindRow(row);
        }

        RowLayout GetColumnHeaderRowLayoutFromY(double y)
        {
            return GetColumnHeaderRowLayoutModel().FindY(y);
        }

        internal RowLayoutModel GetColumnHeaderRowLayoutModel()
        {
            if (_cachedColumnHeaderRowLayoutModel == null)
            {
                _cachedColumnHeaderRowLayoutModel = CreateColumnHeaderRowLayoutModel();
            }
            return _cachedColumnHeaderRowLayoutModel;
        }

        ColHeaderPanel GetColumnHeaderRowsPresenter(int columnViewportIndex)
        {
            if (_colHeaders == null)
                return null;
            return _colHeaders[columnViewportIndex + 1];
        }

        internal ColumnLayoutModel GetColumnHeaderViewportColumnLayoutModel(int columnViewportIndex)
        {
            if (_cachedColumnHeaderViewportColumnLayoutModel == null)
            {
                int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                _cachedColumnHeaderViewportColumnLayoutModel = new ColumnLayoutModel[columnViewportCount + 2];
            }
            if (_cachedColumnHeaderViewportColumnLayoutModel[columnViewportIndex + 1] == null)
            {
                if (ResizeZeroIndicator == ResizeZeroIndicator.Enhanced)
                {
                    _cachedColumnHeaderViewportColumnLayoutModel[columnViewportIndex + 1] = CreateEnhancedResizeToZeroColumnHeaderViewportColumnLayoutModel(columnViewportIndex);
                }
                else
                {
                    _cachedColumnHeaderViewportColumnLayoutModel[columnViewportIndex + 1] = CreateViewportColumnLayoutModel(columnViewportIndex);
                }
            }
            return _cachedColumnHeaderViewportColumnLayoutModel[columnViewportIndex + 1];
        }

        internal ColumnLayoutModel GetColumnLayoutModel(int columnViewportIndex, SheetArea sheetArea)
        {
            switch (sheetArea)
            {
                case SheetArea.Cells:
                    return GetViewportColumnLayoutModel(columnViewportIndex);

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return GetRowHeaderColumnLayoutModel();

                case SheetArea.ColumnHeader:
                    if (ResizeZeroIndicator != ResizeZeroIndicator.Enhanced)
                    {
                        return GetViewportColumnLayoutModel(columnViewportIndex);
                    }
                    return GetColumnHeaderViewportColumnLayoutModel(columnViewportIndex);
            }
            return null;
        }

        Rect GetCornerRectangle()
        {
            SheetLayout sheetLayout = GetSheetLayout();
            double headerX = sheetLayout.HeaderX;
            double headerY = sheetLayout.HeaderY;
            double width = sheetLayout.HeaderWidth - 1.0;
            double height = sheetLayout.HeaderHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(headerX, headerY, width, height);
            }
            return Rect.Empty;
        }

        ColumnLayout GetCurrentDragToColumnLayout()
        {
            return GetViewportColumnLayoutModel(_dragToColumnViewport).FindColumn(_dragToColumn);
        }

        RowLayout GetCurrentDragToRowLayout()
        {
            return GetViewportRowLayoutModel(_dragToRowViewport).FindRow(_dragToRow);
        }

        FillDirection GetCurrentFillDirection()
        {
            switch (_currentFillDirection)
            {
                case DragFillDirection.Left:
                    return FillDirection.Left;

                case DragFillDirection.Right:
                    return FillDirection.Right;

                case DragFillDirection.Up:
                    return FillDirection.Up;

                case DragFillDirection.Down:
                    return FillDirection.Down;

                case DragFillDirection.LeftClear:
                    return FillDirection.Left;

                case DragFillDirection.UpClear:
                    return FillDirection.Up;
            }
            return FillDirection.Down;
        }

        CellRange GetCurrentFillRange()
        {
            int row = -1;
            int column = -1;
            int rowCount = -1;
            int columnCount = -1;
            switch (_currentFillDirection)
            {
                case DragFillDirection.Left:
                    if (!IsDragFillWholeColumns)
                    {
                        row = DragFillStartTopRow;
                        rowCount = _dragFillStartRange.RowCount;
                        break;
                    }
                    row = -1;
                    rowCount = -1;
                    break;

                case DragFillDirection.Right:
                    if (!IsDragFillWholeColumns)
                    {
                        row = DragFillStartTopRow;
                        rowCount = _dragFillStartRange.RowCount;
                    }
                    else
                    {
                        row = -1;
                        rowCount = -1;
                    }
                    column = DragFillStartRightColumn + 1;
                    columnCount = (_dragToColumn - column) + 1;
                    goto Label_0184;

                case DragFillDirection.Up:
                    row = _dragToRow;
                    rowCount = DragFillStartTopRow - row;
                    if (!IsDragFillWholeRows)
                    {
                        column = DragFillStartLeftColumn;
                        columnCount = _dragFillStartRange.ColumnCount;
                    }
                    else
                    {
                        column = -1;
                        columnCount = -1;
                    }
                    goto Label_0184;

                case DragFillDirection.Down:
                    row = DragFillStartBottomRow + 1;
                    rowCount = (_dragToRow - row) + 1;
                    if (!IsDragFillWholeRows)
                    {
                        column = DragFillStartLeftColumn;
                        columnCount = _dragFillStartRange.ColumnCount;
                    }
                    else
                    {
                        column = -1;
                        columnCount = -1;
                    }
                    goto Label_0184;

                case DragFillDirection.LeftClear:
                    if (!IsDragFillWholeColumns)
                    {
                        row = _dragFillStartRange.Row;
                        rowCount = _dragFillStartRange.RowCount;
                    }
                    else
                    {
                        row = -1;
                        rowCount = -1;
                    }
                    column = _dragToColumn;
                    columnCount = (DragFillStartRightColumn - column) + 1;
                    goto Label_0184;

                case DragFillDirection.UpClear:
                    row = _dragToRow;
                    rowCount = (DragFillStartBottomRow - row) + 1;
                    if (!IsDragFillWholeRows)
                    {
                        column = DragFillStartLeftColumn;
                        columnCount = _dragFillStartRange.ColumnCount;
                    }
                    else
                    {
                        column = -1;
                        columnCount = -1;
                    }
                    goto Label_0184;

                default:
                    goto Label_0184;
            }
            column = _dragToColumn;
            columnCount = DragFillStartLeftColumn - column;
        Label_0184:
            return new CellRange(row, column, rowCount, columnCount);
        }

        AutoFillType GetDragAutoFillType()
        {
            bool flag;
            bool flag2;
            if (DefaultAutoFillType.HasValue)
            {
                return DefaultAutoFillType.Value;
            }
            if (IsDragClear)
            {
                return AutoFillType.ClearValues;
            }
            KeyboardHelper.GetMetaKeyState(out flag, out flag2);
            if ((((_dragFillStartRange.RowCount == 1) && (_dragFillStartRange.ColumnCount == 1)) && !IsDragFillWholeColumns) && !IsDragFillWholeRows)
            {
                if (flag2)
                {
                    return AutoFillType.FillSeries;
                }
                return AutoFillType.CopyCells;
            }
            if (flag2)
            {
                return AutoFillType.CopyCells;
            }
            return AutoFillType.FillSeries;
        }

        internal CellRange GetDragClearRange()
        {
            if (IsDragClear)
            {
                return _currentFillRange;
            }
            return null;
        }

        internal CellRange GetDragFillFrameRange()
        {
            if (IsDragClear)
            {
                return _dragFillStartRange;
            }
            int row = 0;
            int rowCount = 0;
            int column = 0;
            int columnCount = 0;
            if (IsVerticalDragFill)
            {
                row = (_currentFillDirection == DragFillDirection.Up) ? _currentFillRange.Row : _dragFillStartRange.Row;
                rowCount = _dragFillStartRange.RowCount + _currentFillRange.RowCount;
                column = _dragFillStartRange.Column;
                columnCount = _dragFillStartRange.ColumnCount;
            }
            else
            {
                row = _dragFillStartRange.Row;
                rowCount = _dragFillStartRange.RowCount;
                column = (_currentFillDirection == DragFillDirection.Left) ? _currentFillRange.Column : _dragFillStartRange.Column;
                columnCount = _dragFillStartRange.ColumnCount + _currentFillRange.ColumnCount;
            }
            return new CellRange(row, column, rowCount, columnCount);
        }

        internal FilterButtonInfo GetFilterButtonInfo(int row, int column, SheetArea sheetArea)
        {
            return GetFilterButtonInfoModel().Find(row, column, sheetArea);
        }

        FilterButtonInfoModel GetFilterButtonInfoModel()
        {
            if (_cachedFilterButtonInfoModel == null)
            {
                _cachedFilterButtonInfoModel = CreateFilterButtonInfoModel();
            }
            return _cachedFilterButtonInfoModel;
        }

        List<object> GetFilteredInDateItems(int columnIndex, RowFilterBase filter)
        {
            List<object> list = new List<object>();
            if ((filter != null) && filter.IsColumnFiltered(columnIndex))
            {
                int num = (filter.Range.Row == -1) ? 0 : filter.Range.Row;
                int num2 = (filter.Range.RowCount == -1) ? filter.Sheet.RowCount : filter.Range.RowCount;
                for (int i = num; i < (num + num2); i++)
                {
                    if (!filter.IsRowFilteredOut(i))
                    {
                        object obj2 = filter.Sheet.GetValue(i, columnIndex);
                        object text = null;
                        if ((obj2 is DateTime) || (obj2 is TimeSpan))
                        {
                            text = obj2;
                        }
                        else
                        {
                            text = filter.Sheet.GetText(i, columnIndex);
                        }
                        if (!list.Contains(text))
                        {
                            list.Add(text);
                        }
                    }
                }
            }
            return list;
        }

        CellRange GetFromRange()
        {
            CellRange range = null;
            if (ActiveSheet.Selections.Count > 1)
            {
                return range;
            }
            if (ActiveSheet.Selections.Count == 1)
            {
                return ActiveSheet.Selections[0];
            }
            CellRange spanCell = ActiveSheet.GetSpanCell(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
            if (spanCell != null)
            {
                return spanCell;
            }
            return new CellRange(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, 1, 1);
        }

        internal Windows.UI.Color GetGripperFillColor()
        {
            if (ActiveSheet == null)
            {
                return Colors.White;
            }
            if (!string.IsNullOrWhiteSpace(ActiveSheet.TouchSelectionGripperBackgroundThemeColor))
            {
                return ActiveSheet.Workbook.GetThemeColor(ActiveSheet.TouchSelectionGripperBackgroundThemeColor);
            }
            return ActiveSheet.TouchSelectionGripperBackgroundColor;
        }

        internal Windows.UI.Color GetGripperStrokeColor()
        {
            if (ActiveSheet == null)
            {
                return Windows.UI.Color.FromArgb(220, 0, 0, 0);
            }
            if (!string.IsNullOrWhiteSpace(ActiveSheet.SelectionBorderThemeColor))
            {
                return ActiveSheet.Workbook.GetThemeColor(ActiveSheet.SelectionBorderThemeColor);
            }
            return ActiveSheet.SelectionBorderColor;
        }

        internal GroupLayout GetGroupLayout()
        {
            if (_cachedGroupLayout == null)
            {
                _cachedGroupLayout = CreateGroupLayout();
            }
            return _cachedGroupLayout;
        }

        string GetHorizentalScrollTip(int column)
        {
            return string.Format(ResourceStrings.HorizentalScroll, (object[])new object[] { ((ActiveSheet.ColumnHeader.AutoText == HeaderAutoText.Numbers) ? ((int)column).ToString() : IndexToLetter(column)) });
        }

        internal ImageSource GetImageSource(string image)
        {
            if (_cachedToolbarImageSources.ContainsKey(image))
            {
                return _cachedToolbarImageSources[image];
            }
            string name = IntrospectionExtensions.GetTypeInfo((Type)typeof(SheetView)).Assembly.GetName().Name;
            Uri uri = new Uri(string.Format("ms-appx:///{0}/Icons/{1}", (object[])new object[] { name, image }), (UriKind)UriKind.RelativeOrAbsolute);
            BitmapImage image2 = new BitmapImage(uri);
            _cachedResizerGipper[image] = image2;
            return image2;
        }

        internal int GetMaxBottomScrollableRow()
        {
            int frozenRowCount = ActiveSheet.FrozenRowCount;
            int num2 = (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1;
            while (num2 > frozenRowCount)
            {
                if (ActiveSheet.Rows[num2].ActualVisible)
                {
                    return num2;
                }
                num2--;
            }
            return num2;
        }

        internal int GetMaxLeftScrollableColumn()
        {
            int frozenColumnCount = ActiveSheet.FrozenColumnCount;
            int num2 = (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1;
            while (frozenColumnCount < num2)
            {
                if (ActiveSheet.Columns[frozenColumnCount].ActualVisible)
                {
                    return frozenColumnCount;
                }
                frozenColumnCount++;
            }
            return frozenColumnCount;
        }

        internal int GetMaxRightScrollableColumn()
        {
            int frozenColumnCount = ActiveSheet.FrozenColumnCount;
            int num2 = (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1;
            while (num2 > frozenColumnCount)
            {
                if (ActiveSheet.Columns[num2].ActualVisible)
                {
                    return num2;
                }
                num2--;
            }
            return num2;
        }

        internal int GetMaxTopScrollableRow()
        {
            int frozenRowCount = ActiveSheet.FrozenRowCount;
            int num2 = (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1;
            while (frozenRowCount < num2)
            {
                if (ActiveSheet.Rows[frozenRowCount].ActualVisible)
                {
                    return frozenRowCount;
                }
                frozenRowCount++;
            }
            return frozenRowCount;
        }

        internal FilterButtonInfo GetMouseDownFilterButton()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (savedHitTestInformation != null)
            {
                return GetMouseDownFilterButton(savedHitTestInformation, false);
            }
            return null;
        }

        FilterButtonInfo GetMouseDownFilterButton(HitTestInformation hi, bool touching = false)
        {
            FilterButtonInfo info = null;
            RowLayout columnHeaderRowLayoutFromY = null;
            ColumnLayout viewportColumnLayoutFromX = null;
            SheetArea cells = SheetArea.Cells;
            if (hi.HitTestType == HitTestType.ColumnHeader)
            {
                columnHeaderRowLayoutFromY = GetColumnHeaderRowLayoutFromY(hi.HitPoint.Y);
                viewportColumnLayoutFromX = GetViewportColumnLayoutFromX(hi.ColumnViewportIndex, hi.HitPoint.X);
                cells = SheetArea.ColumnHeader;
            }
            else if (hi.HitTestType == HitTestType.Viewport)
            {
                columnHeaderRowLayoutFromY = GetViewportRowLayoutFromY(hi.RowViewportIndex, hi.HitPoint.Y);
                viewportColumnLayoutFromX = GetViewportColumnLayoutFromX(hi.ColumnViewportIndex, hi.HitPoint.X);
                cells = SheetArea.Cells;
            }
            if ((columnHeaderRowLayoutFromY != null) && (viewportColumnLayoutFromX != null))
            {
                int row = columnHeaderRowLayoutFromY.Row;
                int column = viewportColumnLayoutFromX.Column;
                CellRange range = ActiveSheet.GetSpanCell(columnHeaderRowLayoutFromY.Row, viewportColumnLayoutFromX.Column, cells);
                if (range != null)
                {
                    if ((columnHeaderRowLayoutFromY.Row != ((range.Row + range.RowCount) - 1)) || (viewportColumnLayoutFromX.Column != ((range.Column + range.ColumnCount) - 1)))
                    {
                        return null;
                    }
                    row = range.Row;
                    column = range.Column;
                }
                info = GetFilterButtonInfo(row, column, cells);
                if (info != null)
                {
                    double x = hi.HitPoint.X;
                    double y = hi.HitPoint.Y;
                    double num5 = Math.Min(16.0, viewportColumnLayoutFromX.Width);
                    double num6 = Math.Min(16.0, columnHeaderRowLayoutFromY.Height);
                    if (touching)
                    {
                        double num7 = ((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) - num5) - 6.0;
                        double num8 = ((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - num6) - 6.0;
                        double num9 = (viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) + 4.0;
                        double num10 = (columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) + 4.0;
                        if (((x >= num7) && (x < num9)) && ((y >= num8) && (y < num10)))
                        {
                            info.RowViewportIndex = hi.RowViewportIndex;
                            info.ColumnViewportIndex = hi.ColumnViewportIndex;
                            return info;
                        }
                    }
                    else if (((x >= (((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) - num5) - 2.0)) && (x < ((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) - 2.0))) && ((y >= (((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - num6) - 2.0)) && (y < ((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - 2.0))))
                    {
                        info.RowViewportIndex = hi.RowViewportIndex;
                        info.ColumnViewportIndex = hi.ColumnViewportIndex;
                        return info;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the column count when scrolling right one page.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index one page to the right.</param>
        /// <returns>The column count when scrolling right one page.</returns>
        public int GetNextPageColumnCount(int columnViewportIndex)
        {
            return GetNextPageColumnCount(ActiveSheet, columnViewportIndex);
        }

        int GetNextPageColumnCount(Worksheet sheet, int columnViewportIndex)
        {
            if (sheet == null)
            {
                return 0;
            }
            float zoomFactor = ZoomFactor;
            int viewportLeftColumn = sheet.GetViewportLeftColumn(columnViewportIndex);
            double viewportWidth = GetViewportWidth(sheet, columnViewportIndex);
            if ((viewportLeftColumn < ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1)) && ((sheet.Columns[viewportLeftColumn].ActualWidth * zoomFactor) >= viewportWidth))
            {
                return 1;
            }
            int num4 = 0;
            double num5 = 0.0;
            int column = viewportLeftColumn;
            while (column < (sheet.ColumnCount - sheet.FrozenTrailingColumnCount))
            {
                double num7 = sheet.GetActualColumnWidth(column, SheetArea.Cells) * zoomFactor;
                if ((num5 + num7) > viewportWidth)
                {
                    break;
                }
                num5 += num7;
                num4++;
                column++;
            }
            if (column == (sheet.ColumnCount - sheet.FrozenTrailingColumnCount))
            {
                num4 = 0;
            }
            return num4;
        }

        /// <summary>
        /// Gets the row count when scrolling down one page.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index one page down.</param>
        /// <returns>The row count when scrolling down one page.</returns>
        public int GetNextPageRowCount(int rowViewportIndex)
        {
            return GetNextPageRowCount(ActiveSheet, rowViewportIndex);
        }

        int GetNextPageRowCount(Worksheet sheet, int rowViewportIndex)
        {
            if (sheet == null)
            {
                return 0;
            }
            float zoomFactor = ZoomFactor;
            int viewportTopRow = GetViewportTopRow(sheet, rowViewportIndex);
            double viewportHeight = GetViewportHeight(sheet, rowViewportIndex);
            if ((viewportTopRow < ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1)) && ((sheet.Rows[viewportTopRow].ActualHeight * zoomFactor) >= viewportHeight))
            {
                return 1;
            }
            int num4 = 0;
            double num5 = 0.0;
            int row = viewportTopRow;
            while (row < (sheet.RowCount - sheet.FrozenTrailingRowCount))
            {
                double num7 = sheet.GetActualRowHeight(row, SheetArea.Cells) * zoomFactor;
                if ((num5 + num7) > viewportHeight)
                {
                    break;
                }
                num5 += num7;
                num4++;
                row++;
            }
            if (row == (sheet.RowCount - sheet.FrozenTrailingRowCount))
            {
                num4 = 0;
            }
            return num4;
        }

        internal int GetNextScrollableColumn(int startColumn)
        {
            int num = startColumn + 1;
            int num2 = ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount;
            while (num < num2)
            {
                if (ActiveSheet.Columns[num].ActualVisible)
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        internal int GetNextScrollableRow(int startRow)
        {
            int num = startRow + 1;
            int num2 = ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount;
            while (num < num2)
            {
                if (ActiveSheet.Rows[num].ActualVisible)
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        static CellRange GetPastedRange(CellRange toRange, string clipboadText)
        {
            CellRange range = null;
            string[,] strArray = Worksheet.ParseCsv(clipboadText, "\r\n", "\t", "\"");
            if (strArray != null)
            {
                int row = (toRange.Row < 0) ? 0 : toRange.Row;
                int column = (toRange.Column < 0) ? 0 : toRange.Column;
                int length = strArray.GetLength(0);
                int columnCount = strArray.GetLength(1);
                range = new CellRange(row, column, length, columnCount);
            }
            return range;
        }

        static CellRange GetPastedRange(Worksheet fromSheet, CellRange fromRange, Worksheet toSheet, CellRange toRange, bool isCutting)
        {
            int row = (fromRange.Row < 0) ? 0 : fromRange.Row;
            int column = (fromRange.Column < 0) ? 0 : fromRange.Column;
            int rowCount = (fromRange.Row < 0) ? fromSheet.RowCount : fromRange.RowCount;
            int columnCount = (fromRange.Column < 0) ? fromSheet.ColumnCount : fromRange.ColumnCount;
            int num5 = (toRange.Row < 0) ? 0 : toRange.Row;
            int num6 = (toRange.Column < 0) ? 0 : toRange.Column;
            int num7 = (toRange.Row < 0) ? toSheet.RowCount : toRange.RowCount;
            int num8 = (toRange.Column < 0) ? toSheet.ColumnCount : toRange.ColumnCount;
            if ((isCutting || ((num7 % rowCount) != 0)) || ((num8 % columnCount) != 0))
            {
                num7 = rowCount;
                num8 = columnCount;
            }
            if (!IsValidRange(row, column, rowCount, columnCount, fromSheet.RowCount, fromSheet.ColumnCount))
            {
                return null;
            }
            if (!IsValidRange(num5, num6, num7, num8, toSheet.RowCount, toSheet.ColumnCount))
            {
                return null;
            }
            CellRange range = new CellRange(num5, num6, num7, num8);
            if (!isCutting && object.ReferenceEquals(fromSheet, toSheet))
            {
                if (range.Contains(row, column, rowCount, columnCount))
                {
                    if ((((row - num5) % rowCount) != 0) || (((column - num6) % columnCount) != 0))
                    {
                        return null;
                    }
                }
                else if (range.Intersects(row, column, rowCount, columnCount) && ((num7 > rowCount) || (num8 > columnCount)))
                {
                    return null;
                }
            }
            if (toRange.Row == -1)
            {
                num5 = -1;
                num7 = -1;
            }
            if (toRange.Column == -1)
            {
                num6 = -1;
                num8 = -1;
            }
            return new CellRange(num5, num6, num7, num8);
        }

        /// <summary>
        /// Gets the column count when scrolling left one page.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index one page to the left.</param>
        /// <returns>The column count when scrolling left one page.</returns>
        public int GetPrePageColumnCount(int columnViewportIndex)
        {
            return GetPrePageColumnCount(ActiveSheet, columnViewportIndex);
        }

        int GetPrePageColumnCount(Worksheet sheet, int columnViewportIndex)
        {
            if (sheet == null)
            {
                return 0;
            }
            float zoomFactor = ZoomFactor;
            int viewportLeftColumn = sheet.GetViewportLeftColumn(columnViewportIndex);
            double viewportWidth = GetViewportWidth(sheet, columnViewportIndex);
            int column = viewportLeftColumn - 1;
            if ((column > sheet.FrozenColumnCount) && ((sheet.Columns[column].ActualWidth * zoomFactor) >= viewportWidth))
            {
                return 1;
            }
            double num5 = 0.0;
            int num6 = 0;
            while ((column >= sheet.FrozenColumnCount) && (num5 < viewportWidth))
            {
                double num7 = sheet.GetActualColumnWidth(column, SheetArea.Cells) * zoomFactor;
                if ((num5 + num7) > viewportWidth)
                {
                    return num6;
                }
                num5 += num7;
                num6++;
                column--;
            }
            return num6;
        }

        /// <summary>
        /// Gets the row count when scrolling up one page.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index one page up.</param>
        /// <returns>The row count when scrolling up one page.</returns>
        public int GetPrePageRowCount(int rowViewportIndex)
        {
            return GetPrePageRowCount(ActiveSheet, rowViewportIndex);
        }

        int GetPrePageRowCount(Worksheet sheet, int rowViewportIndex)
        {
            if (sheet == null)
            {
                return 0;
            }
            float zoomFactor = ZoomFactor;
            int viewportTopRow = GetViewportTopRow(sheet, rowViewportIndex);
            double viewportHeight = GetViewportHeight(sheet, rowViewportIndex);
            int row = viewportTopRow - 1;
            if ((row > sheet.FrozenRowCount) && ((sheet.Rows[row].ActualHeight * zoomFactor) >= viewportHeight))
            {
                return 1;
            }
            double num5 = 0.0;
            int num6 = 0;
            while (row >= sheet.FrozenRowCount)
            {
                double num7 = sheet.GetActualRowHeight(row, SheetArea.Cells) * zoomFactor;
                if ((num5 + num7) > viewportHeight)
                {
                    return num6;
                }
                num5 += num7;
                num6++;
                row--;
            }
            return num6;
        }

        string GetRangeString(CellRange range)
        {
            CalcExpression expression;
            int row = range.Row;
            int column = range.Column;
            int rowCount = range.RowCount;
            int columnCount = range.ColumnCount;
            CalcParser parser = new CalcParser();
            if ((range.RowCount == 1) && (range.ColumnCount == 1))
            {
                expression = new CalcCellExpression(row, column, true, true);
            }
            else
            {
                new CalcCellIdentity(row, column);
                if (((rowCount == -1) && (columnCount == -1)) || ((row == -1) && (column == -1)))
                {
                    expression = new CalcRangeExpression();
                }
                else if ((columnCount == -1) || (column == -1))
                {
                    expression = new CalcRangeExpression(row, (row + rowCount) - 1, true, true, true);
                }
                else if ((rowCount == -1) || (row == -1))
                {
                    expression = new CalcRangeExpression(column, (column + columnCount) - 1, true, true, false);
                }
                else
                {
                    expression = new CalcRangeExpression(row, column, (row + rowCount) - 1, (column + columnCount) - 1, true, true, true, true);
                }
            }
            CalcParserContext context = new CalcParserContext(ActiveSheet.ReferenceStyle == ReferenceStyle.R1C1, 0, 0, null);
            return parser.Unparse(expression, context);
        }

        internal BitmapImage GetResizerBitmapImage(bool rowHeaderResizer)
        {
            string str = "";
            if (rowHeaderResizer)
            {
                str = "ResizeGripperVer.png";
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    str = "ResizeGripperVer_dark.png";
                }
            }
            else
            {
                str = "ResizeGripperHor.png";
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    str = "ResizeGripperHor_dark.png";
                }
            }
            if (_cachedResizerGipper.ContainsKey(str))
            {
                return _cachedResizerGipper[str];
            }
            string name = IntrospectionExtensions.GetTypeInfo((Type)typeof(SheetView)).Assembly.GetName().Name;
            Uri uri = new Uri(string.Format("ms-appx:///{0}/Icons/{1}", (object[])new object[] { name, str }), (UriKind)UriKind.RelativeOrAbsolute);
            BitmapImage image = new BitmapImage(uri);
            _cachedResizerGipper[str] = image;
            return image;
        }

        internal double GetRowAutoFitValue(int row, bool columnHeader)
        {
            double num = -1.0;
            Worksheet worksheet = ActiveSheet;
            int columnCount = worksheet.ColumnCount;
            Cell cell = null;
            FontFamily unknownFontfamily = Excel.FontFamily;
            object textFormattingMode = null;
            for (int i = 0; i < columnCount; i++)
            {
                cell = columnHeader ? worksheet.ColumnHeader.Cells[row, i] : worksheet.Cells[row, i];
                if (!string.IsNullOrEmpty(cell.Text))
                {
                    CellRange range = worksheet.GetSpanCell(row, i, cell.SheetArea);
                    if ((range == null) || ((range.Row >= row) && (range.RowCount <= 1)))
                    {
                        double width = 0.0;
                        if (range == null)
                        {
                            width = worksheet.GetColumnWidth(i, cell.SheetArea);
                        }
                        else
                        {
                            for (int j = 0; j < range.ColumnCount; j++)
                            {
                                width += worksheet.GetColumnWidth(i + j, cell.SheetArea);
                            }
                        }
                        Size maxSize = MeasureHelper.ConvertExcelCellSizeToTextSize(new Size(width, double.PositiveInfinity), 1.0);
                        Size size3 = MeasureHelper.ConvertTextSizeToExcelCellSize(MeasureHelper.MeasureTextInCell(cell, maxSize, 1.0, unknownFontfamily, textFormattingMode, base.UseLayoutRounding), 1.0);
                        num = Math.Max(num, size3.Height);
                        if (range != null)
                        {
                            i += range.ColumnCount - 1;
                        }
                    }
                }
            }
            return num;
        }

        internal CellLayoutModel GetRowHeaderCellLayoutModel(int rowViewportIndex)
        {
            int rowViewportCount = GetViewportInfo().RowViewportCount;
            if (_cachedRowHeaderCellLayoutModel == null)
            {
                _cachedRowHeaderCellLayoutModel = new CellLayoutModel[rowViewportCount + 2];
            }
            if (_cachedRowHeaderCellLayoutModel[rowViewportIndex + 1] == null)
            {
                _cachedRowHeaderCellLayoutModel[rowViewportIndex + 1] = CreateRowHeaderCellLayoutModel(rowViewportIndex);
            }
            return _cachedRowHeaderCellLayoutModel[rowViewportIndex + 1];
        }

        ColumnLayout GetRowHeaderColumnLayout(int column)
        {
            return GetRowHeaderColumnLayoutModel().FindColumn(column);
        }

        ColumnLayout GetRowHeaderColumnLayoutFromX(double x)
        {
            return GetRowHeaderColumnLayoutModel().FindX(x);
        }

        internal ColumnLayoutModel GetRowHeaderColumnLayoutModel()
        {
            if (_cachedRowHeaderColumnLayoutModel == null)
            {
                _cachedRowHeaderColumnLayoutModel = CreateRowHeaderColumnLayoutModel();
            }
            return _cachedRowHeaderColumnLayoutModel;
        }

        Rect GetRowHeaderRectangle(int rowViewportIndex)
        {
            SheetLayout sheetLayout = GetSheetLayout();
            double headerX = sheetLayout.HeaderX;
            double viewportY = sheetLayout.GetViewportY(rowViewportIndex);
            double width = sheetLayout.HeaderWidth - 1.0;
            double height = sheetLayout.GetViewportHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(headerX, viewportY, width, height);
            }
            return Rect.Empty;
        }

        ColumnLayout GetRowHeaderResizingColumnLayoutFromX(double x)
        {
            Worksheet worksheet = ActiveSheet;
            ColumnLayoutModel rowHeaderColumnLayoutModel = GetRowHeaderColumnLayoutModel();
            if (worksheet.RowCount > 0)
            {
                for (int i = rowHeaderColumnLayoutModel.Count - 1; i >= 0; i--)
                {
                    ColumnLayout layout = Enumerable.ElementAt<ColumnLayout>((IEnumerable<ColumnLayout>)rowHeaderColumnLayoutModel, i);
                    if (((x >= Math.Max(layout.X, (layout.X + layout.Width) - 4.0)) && (x < ((layout.X + layout.Width) + 4.0))) && worksheet.RowHeader.Columns[layout.Column].CanUserResize)
                    {
                        return layout;
                    }
                }
            }
            return null;
        }

        ColumnLayout GetRowHeaderResizingColumnLayoutFromXForTouch(double x)
        {
            ColumnLayout rowHeaderResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(x);
            if (rowHeaderResizingColumnLayoutFromX == null)
            {
                for (int i = -5; i < 5; i++)
                {
                    rowHeaderResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(x + i);
                    if (rowHeaderResizingColumnLayoutFromX != null)
                    {
                        return rowHeaderResizingColumnLayoutFromX;
                    }
                }
            }
            return rowHeaderResizingColumnLayoutFromX;
        }

        internal RowHeaderPanel GetRowHeaderRowsPresenter(int rowViewportIndex)
        {
            if (_rowHeaders == null)
                return null;
            return _rowHeaders[rowViewportIndex + 1];
        }

        internal RowLayoutModel GetRowHeaderViewportRowLayoutModel(int rowViewportIndex)
        {
            if (_cachedRowHeaderViewportRowLayoutModel == null)
            {
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                _cachedRowHeaderViewportRowLayoutModel = new RowLayoutModel[rowViewportCount + 2];
            }
            if (_cachedRowHeaderViewportRowLayoutModel[rowViewportIndex + 1] == null)
            {
                if (ResizeZeroIndicator == ResizeZeroIndicator.Enhanced)
                {
                    _cachedRowHeaderViewportRowLayoutModel[rowViewportIndex + 1] = CreateEnhancedResizeToZeroRowHeaderViewportRowLayoutModel(rowViewportIndex);
                }
                else
                {
                    _cachedRowHeaderViewportRowLayoutModel[rowViewportIndex + 1] = CreateViewportRowLayoutModel(rowViewportIndex);
                }
            }
            return _cachedRowHeaderViewportRowLayoutModel[rowViewportIndex + 1];
        }

        internal RowLayoutModel GetRowLayoutModel(int rowViewportIndex, SheetArea sheetArea)
        {
            switch (sheetArea)
            {
                case SheetArea.Cells:
                    return GetViewportRowLayoutModel(rowViewportIndex);

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    if (ResizeZeroIndicator != ResizeZeroIndicator.Enhanced)
                    {
                        return GetViewportRowLayoutModel(rowViewportIndex);
                    }
                    return GetRowHeaderViewportRowLayoutModel(rowViewportIndex);

                case SheetArea.ColumnHeader:
                    return GetColumnHeaderRowLayoutModel();
            }
            return null;
        }

        internal static object[,] GetsArrayFormulas(Worksheet sheet, int row, int column, int rowCount, int columnCount)
        {
            object[,] objArray = sheet.FindFormulas(row, column, rowCount, columnCount);
            if ((objArray != null) && (objArray.Length > 0))
            {
                List<string> list = new List<string>();
                List<CellRange> list2 = new List<CellRange>();
                int length = objArray.GetLength(0);
                for (int i = 0; i < length; i++)
                {
                    string str = (string)(objArray[i, 1] as string);
                    if ((!string.IsNullOrEmpty(str) && str.StartsWith("{")) && str.EndsWith("}"))
                    {
                        list2.Add((CellRange)objArray[i, 0]);
                        list.Add(str);
                    }
                }
                if (list.Count > 0)
                {
                    object[,] objArray2 = new object[list.Count, 2];
                    for (int j = 0; j < list.Count; j++)
                    {
                        objArray2[j, 0] = list2[j];
                        objArray2[j, 1] = list[j];
                    }
                    return objArray2;
                }
            }
            return null;
        }

        /// <summary>
        /// Ges the spread chart view.
        /// </summary>
        /// <param name="chartName">Name of the chart.</param>
        /// <returns></returns>
        public SpreadChartView GetSpreadChartView(string chartName)
        {
            int activeRowViewportIndex = GetActiveRowViewportIndex();
            int activeColumnViewportIndex = GetActiveColumnViewportIndex();
            CellsPanel viewport = _cellsPanels[activeRowViewportIndex + 1, activeColumnViewportIndex + 1];
            if (viewport != null)
            {
                return viewport.GetSpreadChartView(chartName);
            }
            return null;
        }

        ColumnLayout GetValidHorDragToColumnLayout()
        {
            if (IsIncreaseFill)
            {
                if (IsDragToColumnInView)
                {
                    return GetCurrentDragToColumnLayout();
                }
                return DragFillToViewportRightColumnLayout;
            }
            if (IsDragFillStartRightColumnInView)
            {
                return DragFillStartRightColumnLayout;
            }
            return DragFillStartViewportRightColumnLayout;
        }

        RowLayout GetValidVerDragToRowLayout()
        {
            if (IsIncreaseFill)
            {
                if (IsDragToRowInView)
                {
                    return GetCurrentDragToRowLayout();
                }
                return DragFillToViewportBottomRowLayout;
            }
            if (IsDragFillStartBottomRowInView)
            {
                return DragFillStartBottomRowLayout;
            }
            return DragFillStartViewportBottomRowLayout;
        }

        string GetVericalScrollTip(int row)
        {
            return string.Format(ResourceStrings.VerticalScroll, (object[])new object[] { ((int)row) });
        }

        /// <summary>
        /// Gets the row viewport's bottom row index.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <returns>The bottom row index in the row viewport.</returns>
        public int GetViewportBottomRow(int rowViewportIndex)
        {
            return GetViewportBottomRow(ActiveSheet, rowViewportIndex);
        }

        int GetViewportBottomRow(Worksheet sheet, int rowViewportIndex)
        {
            if (rowViewportIndex == GetViewportInfo(sheet).RowViewportCount)
            {
                return (sheet.RowCount - 1);
            }
            int viewportTopRow = GetViewportTopRow(rowViewportIndex);
            double viewportHeight = GetViewportHeight(sheet, rowViewportIndex);
            double num3 = 0.0;
            int num4 = 0;
            float zoomFactor = ZoomFactor;
            int row = viewportTopRow;
            while ((row < (sheet.RowCount - sheet.FrozenTrailingRowCount)) && (num3 < viewportHeight))
            {
                num3 += Math.Ceiling((double)(sheet.GetActualRowHeight(row, SheetArea.Cells) * zoomFactor));
                row++;
                num4++;
            }
            return ((viewportTopRow + num4) - 1);
        }

        CellItem GetViewportCell(int rowViewportIndex, int columnViewportIndex, int rowIndex, int columnIndex)
        {
            CellItem cell = null;
            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(rowViewportIndex, columnViewportIndex);
            if (viewportRowsPresenter != null)
            {
                RowItem row = viewportRowsPresenter.GetRow(rowIndex);
                if (row != null)
                {
                    cell = row.GetCell(columnIndex);
                }
            }
            return cell;
        }

        internal CellLayoutModel GetViewportCellLayoutModel(int rowViewportIndex, int columnViewportIndex)
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            int columnViewportCount = viewportInfo.ColumnViewportCount;
            int rowViewportCount = viewportInfo.RowViewportCount;
            if (_cachedViewportCellLayoutModel == null)
            {
                _cachedViewportCellLayoutModel = new CellLayoutModel[rowViewportCount + 2, columnViewportCount + 2];
            }
            if (_cachedViewportCellLayoutModel[rowViewportIndex + 1, columnViewportIndex + 1] == null)
            {
                _cachedViewportCellLayoutModel[rowViewportIndex + 1, columnViewportIndex + 1] = CreateViewportCellLayoutModel(rowViewportIndex, columnViewportIndex);
            }
            return _cachedViewportCellLayoutModel[rowViewportIndex + 1, columnViewportIndex + 1];
        }

        ColumnLayout GetViewportColumnLayoutFromX(int columnViewportIndex, double x)
        {
            if (ResizeZeroIndicator != ResizeZeroIndicator.Enhanced)
            {
                return GetViewportColumnLayoutModel(columnViewportIndex).FindX(x);
            }
            ColumnLayoutModel columnHeaderViewportColumnLayoutModel = GetColumnHeaderViewportColumnLayoutModel(columnViewportIndex);
            ColumnLayout layout = columnHeaderViewportColumnLayoutModel.FindX(x);
            if ((InputDeviceType == InputDeviceType.Touch) && (layout != null))
            {
                if (ActiveSheet.GetActualColumnWidth(layout.Column, SheetArea.Cells).IsZero())
                {
                    return layout;
                }
                if ((layout.Column <= 0) || !ActiveSheet.GetActualColumnWidth(layout.Column - 1, SheetArea.Cells).IsZero())
                {
                    return layout;
                }
                ColumnLayout layout2 = columnHeaderViewportColumnLayoutModel.FindColumn(layout.Column - 1);
                if ((layout2 != null) && (((layout2.X + layout2.Width) + 3.0) >= x))
                {
                    return layout2;
                }
            }
            return layout;
        }

        internal ColumnLayoutModel GetViewportColumnLayoutModel(int columnViewportIndex)
        {
            if (_cachedViewportColumnLayoutModel == null)
            {
                int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                _cachedViewportColumnLayoutModel = new ColumnLayoutModel[columnViewportCount + 2];
            }
            if (_cachedViewportColumnLayoutModel[columnViewportIndex + 1] == null)
            {
                _cachedViewportColumnLayoutModel[columnViewportIndex + 1] = CreateViewportColumnLayoutModel(columnViewportIndex);
            }
            return _cachedViewportColumnLayoutModel[columnViewportIndex + 1];
        }

        internal ColumnLayout GetViewportColumnLayoutNearX(int columnViewportIndex, double x)
        {
            SheetLayout sheetLayout = GetSheetLayout();
            ColumnLayout layout2 = null;
            int columnViewportCount = GetViewportInfo().ColumnViewportCount;
            if ((columnViewportIndex == -1) && (x > (sheetLayout.GetViewportX(-1) + sheetLayout.GetViewportWidth(-1))))
            {
                layout2 = GetViewportColumnLayoutModel(0).FindNearX(x);
            }
            else if (((columnViewportIndex == 0) && (x < sheetLayout.GetViewportX(0))) && (GetViewportLeftColumn(0) == ActiveSheet.FrozenColumnCount))
            {
                layout2 = GetViewportColumnLayoutModel(-1).FindNearX(x);
            }
            else if (((columnViewportIndex == (columnViewportCount - 1)) && (x > sheetLayout.GetViewportX(columnViewportCount))) && (GetViewportRightColumn(columnViewportCount - 1) == ((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1)))
            {
                layout2 = GetViewportColumnLayoutModel(columnViewportCount).FindNearX(x);
            }
            else if ((columnViewportIndex == columnViewportCount) && (x < sheetLayout.GetViewportX(columnViewportCount)))
            {
                layout2 = GetViewportColumnLayoutModel(columnViewportCount - 1).FindNearX(x);
            }
            if (layout2 == null)
            {
                layout2 = GetViewportColumnLayoutModel(columnViewportIndex).FindNearX(x);
            }
            return layout2;
        }

        internal FloatingObjectLayoutModel GetViewportFloatingObjectLayoutModel(int rowViewportIndex, int columnViewportIndex)
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            int columnViewportCount = viewportInfo.ColumnViewportCount;
            int rowViewportCount = viewportInfo.RowViewportCount;
            if (_cachedFloatingObjectLayoutModel == null)
            {
                _cachedFloatingObjectLayoutModel = new FloatingObjectLayoutModel[rowViewportCount + 2, columnViewportCount + 2];
            }
            if (_cachedFloatingObjectLayoutModel[rowViewportIndex + 1, columnViewportIndex + 1] == null)
            {
                _cachedFloatingObjectLayoutModel[rowViewportIndex + 1, columnViewportIndex + 1] = CreateViewportChartShapeLayoutMode(rowViewportIndex, columnViewportIndex);
            }
            return _cachedFloatingObjectLayoutModel[rowViewportIndex + 1, columnViewportIndex + 1];
        }

        internal double GetViewportHeight(int rowViewportIndex)
        {
            return GetViewportHeight(ActiveSheet, rowViewportIndex);
        }

        double GetViewportHeight(Worksheet sheet, int rowViewportIndex)
        {
            return GetSheetLayout().GetViewportHeight(rowViewportIndex);
        }

        /// <summary>
        /// Gets the column viewport's left column index.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <returns>The left column index in the column viewport.</returns>
        public int GetViewportLeftColumn(int columnViewportIndex)
        {
            return ActiveSheet.GetViewportLeftColumn(columnViewportIndex);
        }

        Rect GetViewportRectangle(int rowViewportIndex, int columnViewportIndex)
        {
            SheetLayout sheetLayout = GetSheetLayout();
            double viewportX = sheetLayout.GetViewportX(columnViewportIndex);
            double viewportY = sheetLayout.GetViewportY(rowViewportIndex);
            double width = sheetLayout.GetViewportWidth(columnViewportIndex) - 1.0;
            double height = sheetLayout.GetViewportHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(viewportX, viewportY, width, height);
            }
            return Rect.Empty;
        }

        ColumnLayout GetViewportResizingColumnLayoutFromX(int columnViewportIndex, double x)
        {
            Worksheet worksheet = ActiveSheet;
            ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(columnViewportIndex);
            for (int i = viewportColumnLayoutModel.Count - 1; i >= 0; i--)
            {
                ColumnLayout layout = Enumerable.ElementAt<ColumnLayout>((IEnumerable<ColumnLayout>)viewportColumnLayoutModel, i);
                if (((layout != null) && (x >= Math.Max(layout.X, (layout.X + layout.Width) - 4.0))) && ((x < ((layout.X + layout.Width) + 4.0)) && worksheet.Columns[layout.Column].CanUserResize))
                {
                    return layout;
                }
            }
            if (((columnViewportIndex >= 0) && (columnViewportIndex < GetViewportInfo().ColumnViewportCount)) && (viewportColumnLayoutModel.Count > 0))
            {
                ColumnLayout layout2 = viewportColumnLayoutModel[0];
                if (((x >= Math.Max((double)0.0, (double)(layout2.X - 4.0))) && (x < (layout2.X + 4.0))) && ((columnViewportIndex - 1) >= -1))
                {
                    ColumnLayoutModel model2 = GetViewportColumnLayoutModel(Math.Max(-1, columnViewportIndex - 1));
                    for (int j = layout2.Column - 1; j >= worksheet.FrozenColumnCount; j--)
                    {
                        if (model2.Find(j) != null)
                        {
                            break;
                        }
                        if ((worksheet.GetActualColumnWidth(j, SheetArea.Cells) == 0.0) && worksheet.Columns[j].CanUserResize)
                        {
                            return new ColumnLayout(j, layout2.X, 0.0);
                        }
                    }
                }
            }
            return null;
        }

        ColumnLayout GetViewportResizingColumnLayoutFromXForTouch(int columnViewportIndex, double x)
        {
            Worksheet worksheet = ActiveSheet;
            ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(columnViewportIndex);
            for (int i = viewportColumnLayoutModel.Count - 1; i >= 0; i--)
            {
                ColumnLayout layout = Enumerable.ElementAt<ColumnLayout>((IEnumerable<ColumnLayout>)viewportColumnLayoutModel, i);
                if (((layout != null) && (x >= Math.Max(layout.X, (layout.X + layout.Width) - 8.0))) && ((x < ((layout.X + layout.Width) + 8.0)) && worksheet.Columns[layout.Column].CanUserResize))
                {
                    return layout;
                }
            }
            if (((columnViewportIndex >= 0) && (columnViewportIndex < GetViewportInfo().ColumnViewportCount)) && (viewportColumnLayoutModel.Count > 0))
            {
                ColumnLayout layout2 = viewportColumnLayoutModel[0];
                if (((x >= Math.Max((double)0.0, (double)(layout2.X - 8.0))) && (x < (layout2.X + 8.0))) && ((columnViewportIndex - 1) >= -1))
                {
                    ColumnLayoutModel model2 = GetViewportColumnLayoutModel(Math.Max(-1, columnViewportIndex - 1));
                    for (int j = layout2.Column - 1; j >= worksheet.FrozenColumnCount; j--)
                    {
                        if (model2.Find(j) != null)
                        {
                            break;
                        }
                        if ((worksheet.GetActualColumnWidth(j, SheetArea.Cells) == 0.0) && worksheet.Columns[j].CanUserResize)
                        {
                            return new ColumnLayout(j, layout2.X, 0.0);
                        }
                    }
                }
            }
            return null;
        }

        RowLayout GetViewportResizingRowLayoutFromY(int rowViewportIndex, double y)
        {
            Worksheet worksheet = ActiveSheet;
            RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(rowViewportIndex);
            for (int i = viewportRowLayoutModel.Count - 1; i >= 0; i--)
            {
                RowLayout layout = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)viewportRowLayoutModel, i);
                if (((layout != null) && (y >= Math.Max(layout.Y, (layout.Y + layout.Height) - 4.0))) && ((y < ((layout.Y + layout.Height) + 4.0)) && worksheet.Rows[layout.Row].CanUserResize))
                {
                    return layout;
                }
            }
            if (((rowViewportIndex >= 0) && (rowViewportIndex < GetViewportInfo().RowViewportCount)) && (viewportRowLayoutModel.Count > 0))
            {
                RowLayout layout2 = viewportRowLayoutModel[0];
                if (((y >= Math.Max((double)0.0, (double)(layout2.Y - 4.0))) && (y < (layout2.Y + 4.0))) && ((rowViewportIndex - 1) >= -1))
                {
                    RowLayoutModel model2 = GetViewportRowLayoutModel(Math.Max(-1, rowViewportIndex - 1));
                    for (int j = layout2.Row - 1; j >= worksheet.FrozenRowCount; j--)
                    {
                        if (model2.Find(j) != null)
                        {
                            break;
                        }
                        if ((worksheet.GetActualRowHeight(j, SheetArea.Cells) == 0.0) && worksheet.Rows[j].CanUserResize)
                        {
                            return new RowLayout(j, layout2.Y, 0.0);
                        }
                    }
                }
            }
            return null;
        }

        RowLayout GetViewportResizingRowLayoutFromYForTouch(int rowViewportIndex, double y)
        {
            Worksheet worksheet = ActiveSheet;
            RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(rowViewportIndex);
            for (int i = viewportRowLayoutModel.Count - 1; i >= 0; i--)
            {
                RowLayout layout = Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>)viewportRowLayoutModel, i);
                if (((layout != null) && (y >= Math.Max(layout.Y, (layout.Y + layout.Height) - 8.0))) && ((y < ((layout.Y + layout.Height) + 8.0)) && worksheet.Rows[layout.Row].CanUserResize))
                {
                    return layout;
                }
            }
            if (((rowViewportIndex >= 0) && (rowViewportIndex < GetViewportInfo().RowViewportCount)) && (viewportRowLayoutModel.Count > 0))
            {
                RowLayout layout2 = viewportRowLayoutModel[0];
                if (((y >= Math.Max((double)0.0, (double)(layout2.Y - 8.0))) && (y < (layout2.Y + 8.0))) && ((rowViewportIndex - 1) >= -1))
                {
                    RowLayoutModel model2 = GetViewportRowLayoutModel(Math.Max(-1, rowViewportIndex - 1));
                    for (int j = layout2.Row - 1; j >= worksheet.FrozenRowCount; j--)
                    {
                        if (model2.Find(j) != null)
                        {
                            break;
                        }
                        if ((worksheet.GetActualRowHeight(j, SheetArea.Cells) == 0.0) && worksheet.Rows[j].CanUserResize)
                        {
                            return new RowLayout(j, layout2.Y, 0.0);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the column viewport's right column index.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <returns>The right column index in the column viewport.</returns>
        public int GetViewportRightColumn(int columnViewportIndex)
        {
            return GetViewportRightColumn(ActiveSheet, columnViewportIndex);
        }

        int GetViewportRightColumn(Worksheet sheet, int columnViewportIndex)
        {
            if (columnViewportIndex == GetViewportInfo(sheet).ColumnViewportCount)
            {
                return (sheet.ColumnCount - 1);
            }
            int viewportLeftColumn = sheet.GetViewportLeftColumn(columnViewportIndex);
            double viewportWidth = GetViewportWidth(sheet, columnViewportIndex);
            int num3 = 0;
            double num4 = 0.0;
            float zoomFactor = ZoomFactor;
            int column = viewportLeftColumn;
            while ((column < (sheet.ColumnCount - sheet.FrozenTrailingColumnCount)) && (num4 < viewportWidth))
            {
                num4 += Math.Ceiling((double)(sheet.GetActualColumnWidth(column, SheetArea.Cells) * zoomFactor));
                column++;
                num3++;
            }
            return ((viewportLeftColumn + num3) - 1);
        }

        RowLayout GetViewportRowLayoutFromY(int rowViewportIndex, double y)
        {
            if (ResizeZeroIndicator != ResizeZeroIndicator.Enhanced)
            {
                return GetViewportRowLayoutModel(rowViewportIndex).FindY(y);
            }
            RowLayoutModel rowHeaderViewportRowLayoutModel = GetRowHeaderViewportRowLayoutModel(rowViewportIndex);
            RowLayout layout = rowHeaderViewportRowLayoutModel.FindY(y);
            if ((InputDeviceType == InputDeviceType.Touch) && (layout != null))
            {
                if (ActiveSheet.GetActualRowHeight(layout.Row, SheetArea.Cells).IsZero())
                {
                    return layout;
                }
                if ((layout.Row <= 0) || !ActiveSheet.GetActualRowHeight(layout.Row - 1, SheetArea.Cells).IsZero())
                {
                    return layout;
                }
                RowLayout layout2 = rowHeaderViewportRowLayoutModel.FindRow(layout.Row - 1);
                if ((layout2 != null) && (((layout2.Y + layout2.Height) + 3.0) >= y))
                {
                    return layout2;
                }
            }
            return layout;
        }

        internal RowLayoutModel GetViewportRowLayoutModel(int rowViewportIndex)
        {
            if (_cachedViewportRowLayoutModel == null)
            {
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                _cachedViewportRowLayoutModel = new RowLayoutModel[rowViewportCount + 2];
            }
            if (_cachedViewportRowLayoutModel[rowViewportIndex + 1] == null)
            {
                _cachedViewportRowLayoutModel[rowViewportIndex + 1] = CreateViewportRowLayoutModel(rowViewportIndex);
            }
            return _cachedViewportRowLayoutModel[rowViewportIndex + 1];
        }

        internal RowLayout GetViewportRowLayoutNearY(int rowViewportIndex, double y)
        {
            SheetLayout sheetLayout = GetSheetLayout();
            RowLayout layout2 = null;
            int rowViewportCount = GetViewportInfo().RowViewportCount;
            if ((rowViewportIndex == -1) && (sheetLayout.GetViewportY(0) < y))
            {
                layout2 = GetViewportRowLayoutModel(0).FindNearY(y);
            }
            else if (((rowViewportIndex == 0) && (y < sheetLayout.GetViewportY(0))) && (GetViewportTopRow(0) == ActiveSheet.FrozenRowCount))
            {
                layout2 = GetViewportRowLayoutModel(-1).FindNearY(y);
            }
            else if (((rowViewportIndex == (rowViewportCount - 1)) && (y > sheetLayout.GetViewportY(rowViewportCount))) && (GetViewportBottomRow(rowViewportCount - 1) == ((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1)))
            {
                layout2 = GetViewportRowLayoutModel(rowViewportCount).FindNearY(y);
            }
            else if ((rowViewportIndex == rowViewportCount) && (y < sheetLayout.GetViewportY(rowViewportCount)))
            {
                layout2 = GetViewportRowLayoutModel(rowViewportCount - 1).FindNearY(y);
            }
            if (layout2 == null)
            {
                layout2 = GetViewportRowLayoutModel(rowViewportIndex).FindNearY(y);
            }
            return layout2;
        }

        internal CellsPanel GetViewportRowsPresenter(int rowViewportIndex, int columnViewportIndex)
        {
            if (_cellsPanels != null)
            {
                int length = _cellsPanels.GetLength(0);
                if ((rowViewportIndex >= -1) && (rowViewportIndex < (length - 1)))
                {
                    int num2 = _cellsPanels.GetLength(1);
                    if ((columnViewportIndex >= -1) && (columnViewportIndex < (num2 - 1)))
                    {
                        return _cellsPanels[rowViewportIndex + 1, columnViewportIndex + 1];
                    }
                }
            }
            return null;
        }

        Point GetViewportTopLeftCoordinates(int rowViewportIndex, int columnViewportIndex)
        {
            int viewportTopRow = GetViewportTopRow(rowViewportIndex);
            double y = 0.0;
            for (int i = 0; i < viewportTopRow; i++)
            {
                double num4 = Math.Ceiling((double)(ActiveSheet.GetActualRowHeight(i, SheetArea.Cells) * ZoomFactor));
                y += num4;
            }
            int viewportLeftColumn = GetViewportLeftColumn(columnViewportIndex);
            double x = 0.0;
            for (int j = 0; j < viewportLeftColumn; j++)
            {
                double num8 = Math.Ceiling((double)(ActiveSheet.GetActualColumnWidth(j, SheetArea.Cells) * ZoomFactor));
                x += num8;
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Gets the row viewport's top row index.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <returns>The top row index in the row viewport.</returns>
        public int GetViewportTopRow(int rowViewportIndex)
        {
            return GetViewportTopRow(ActiveSheet, rowViewportIndex);
        }

        int GetViewportTopRow(Worksheet sheet, int rowViewportIndex)
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            if ((viewportInfo.RowViewportCount > 0) && (viewportInfo.ColumnViewportCount > 0))
            {
                if (rowViewportIndex == -1)
                {
                    return 0;
                }
                if ((rowViewportIndex >= 0) && (rowViewportIndex < viewportInfo.RowViewportCount))
                {
                    return viewportInfo.TopRows[rowViewportIndex];
                }
                if (rowViewportIndex == viewportInfo.RowViewportCount)
                {
                    return Math.Max(sheet.FrozenRowCount, sheet.RowCount - sheet.FrozenTrailingRowCount);
                }
            }
            return -1;
        }

        internal double GetViewportWidth(int columnViewportIndex)
        {
            return GetViewportWidth(ActiveSheet, columnViewportIndex);
        }

        double GetViewportWidth(Worksheet sheet, int columnViewportIndex)
        {
            return GetSheetLayout().GetViewportWidth(columnViewportIndex);
        }

        internal int GetVisibleColumnCount()
        {
            return GetVisibleColumnCount(ActiveSheet);
        }

        int GetVisibleColumnCount(Worksheet worksheet)
        {
            if (worksheet == null)
            {
                return -1;
            }
            int num = 0;
            for (int i = 0; i < worksheet.ColumnCount; i++)
            {
                if (worksheet.GetActualColumnVisible(i, SheetArea.Cells))
                {
                    num++;
                }
            }
            return num;
        }

        internal int GetVisibleRowCount()
        {
            return GetVisibleRowCount(ActiveSheet);
        }

        int GetVisibleRowCount(Worksheet worksheet)
        {
            if (worksheet == null)
            {
                return -1;
            }
            int num = 0;
            for (int i = 0; i < worksheet.RowCount; i++)
            {
                if (worksheet.GetActualRowVisible(i, SheetArea.Cells))
                {
                    num++;
                }
            }
            return num;
        }

        internal void HandleCellChanged(object sender, CellChangedEventArgs e)
        {
            if (sender == ActiveSheet)
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
                            InvalidateRange(e.Row, e.Column, e.RowCount, e.ColumnCount, e.SheetArea);
                            return;
                        }
                        InvalidateRange(-1, -1, -1, -1, SheetArea.Cells);
                        return;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    case SheetArea.ColumnHeader:
                        if (e.PropertyName == "Axis")
                        {
                            InvalidateLayout();
                        }
                        InvalidateRange(e.Row, e.Column, e.RowCount, e.ColumnCount, e.SheetArea);
                        return;
                }
            }
        }

        internal void HandleChartChanged(object sender, ChartChangedBaseEventArgs e, bool autoRefresh)
        {
            if (_cellsPanels != null)
            {
                if (e.Property == "IsSelected")
                {
                    UpdateSelectState(e);
                }
                else if (autoRefresh)
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
        }

        internal void HandleFloatingObjectChanged(FloatingObject floatingObject, string property, bool autoRefresh)
        {
            if (_cellsPanels != null)
            {
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
        }

        internal void HandleFloatingObjectChanged(object sender, FloatingObjectChangedEventArgs e, bool autoRefresh)
        {
            HandleFloatingObjectChanged(e.FloatingObject, e.Property, autoRefresh);
        }

        internal void HandlePictureChanged(object sender, PictureChangedEventArgs e, bool autoRefresh)
        {
            HandleFloatingObjectChanged(e.Picture, e.Property, autoRefresh);
        }

        internal void HandleSheetColumnHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == ActiveSheet.ColumnHeader)
            {
                switch (e.PropertyName)
                {
                    case "DefaultStyle":
                    case "AutoText":
                    case "AutoTextIndex":
                    case "IsVisible":
                    case "RowCount":
                        Invalidate();
                        return;

                    case "DefaultRowHeight":
                        InvalidateRows(0, ActiveSheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);
                        return;
                }
            }
        }

        internal void HandleSheetPropertyChanged(object sender, PropertyChangedEventArgs e, bool autoRefresh)
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
                            Invalidate();
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
                            UpdateDataValidationUI(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
                            return;

                        case "FrozenRowCount":
                            SetViewportTopRow(0, ActiveSheet.FrozenRowCount);
                            if (autoRefresh)
                            {
                                InvalidateRows(0, ActiveSheet.FrozenRowCount, SheetArea.Cells | SheetArea.RowHeader);
                            }
                            return;

                        case "FrozenColumnCount":
                            SetViewportLeftColumn(0, ActiveSheet.FrozenColumnCount);
                            if (autoRefresh)
                            {
                                InvalidateColumns(0, ActiveSheet.FrozenColumnCount, SheetArea.Cells | SheetArea.ColumnHeader);
                            }
                            return;

                        case "FrozenTrailingRowCount":
                            if (autoRefresh)
                            {
                                InvalidateRows(Math.Max(0, ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount), ActiveSheet.FrozenTrailingRowCount, SheetArea.Cells | SheetArea.RowHeader);
                            }
                            return;

                        case "FrozenTrailingColumnCount":
                            if (autoRefresh)
                            {
                                InvalidateRows(Math.Max(0, ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount), ActiveSheet.FrozenTrailingColumnCount, SheetArea.Cells | SheetArea.ColumnHeader);
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
                                InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
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
                                InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            }
                            return;

                        case "DataSource":
                            if (autoRefresh)
                            {
                                Invalidate();
                            }
                            return;

                        case "[ViewportInfo]":
                            return;

                        case "RowCount":
                        case "RowRangeGroup":
                            if (autoRefresh)
                            {
                                InvalidateRows(0, ActiveSheet.RowCount, SheetArea.Cells | SheetArea.RowHeader);
                            }
                            return;

                        case "ColumnCount":
                        case "ColumnRangeGroup":
                            if (autoRefresh)
                            {
                                InvalidateColumns(0, ActiveSheet.ColumnCount, SheetArea.Cells | SheetArea.ColumnHeader);
                            }
                            return;

                        case "StartingRowNumber":
                        case "RowHeaderColumnCount":
                            if (autoRefresh)
                            {
                                InvalidateColumns(0, ActiveSheet.RowHeader.ColumnCount, SheetArea.CornerHeader | SheetArea.RowHeader);
                            }
                            return;

                        case "StartingColumnNumber":
                        case "ColumnHeaderRowCount":
                            if (autoRefresh)
                            {
                                InvalidateRows(0, ActiveSheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);
                            }
                            return;

                        case "RowHeaderDefaultStyle":
                            if (autoRefresh)
                            {
                                InvalidateRange(-1, -1, -1, -1, SheetArea.CornerHeader | SheetArea.RowHeader);
                            }
                            return;

                        case "ColumnHeaderDefaultStyle":
                            if (autoRefresh)
                            {
                                InvalidateRange(-1, -1, -1, -1, SheetArea.ColumnHeader);
                            }
                            return;

                        case "ReferenceStyle":
                        case "Names":
                            if (autoRefresh)
                            {
                                InvalidateRange(-1, -1, -1, -1, SheetArea.Cells);
                            }
                            return;

                        case "[ImportFile]":
                            if (autoRefresh)
                            {
                                InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            }
                            Excel.HideProgressRingOnOpenCSVCompleted();
                            return;

                        case "[OpenXml]":
                            InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
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

        internal void HandleSheetRowHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == ActiveSheet.RowHeader)
            {
                switch (e.PropertyName)
                {
                    case "DefaultStyle":
                    case "AutoText":
                    case "AutoTextIndex":
                    case "IsVisible":
                    case "ColumnCount":
                        Invalidate();
                        return;

                    case "DefaultColumnWidth":
                        InvalidateColumns(0, ActiveSheet.RowHeader.ColumnCount, SheetArea.CornerHeader | SheetArea.RowHeader);
                        return;
                }
            }
        }

        void HandleVisibleChanged(Worksheet sheet)
        {
            if ((sheet != null) && (sheet.Workbook != null))
            {
                if (sheet.Visible)
                {
                    if (sheet.Workbook.ActiveSheetIndex < 0)
                    {
                        sheet.Workbook.ActiveSheet = sheet;
                    }
                }
                else if (sheet.Workbook.Sheets != null)
                {
                    int index = sheet.Workbook.Sheets.IndexOf(sheet);
                    if ((index != -1) && (index == sheet.Workbook.ActiveSheetIndex))
                    {
                        int count = sheet.Workbook.Sheets.Count;
                        int num3 = index + 1;
                        while ((num3 < count) && !sheet.Workbook.Sheets[num3].Visible)
                        {
                            num3++;
                        }
                        if (num3 >= count)
                        {
                            num3 = index - 1;
                            while ((num3 >= 0) && !sheet.Workbook.Sheets[num3].Visible)
                            {
                                num3--;
                            }
                        }
                        sheet.Workbook.ActiveSheetIndex = num3;
                    }
                }
            }
        }

        internal void HandleWorkbookPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Sheets":
                case "ActiveSheetIndex":
                case "ActiveSheet":
                    Invalidate();
                    return;

                case "StartSheetIndex":
                    ProcessStartSheetIndexChanged();
                    return;

                case "CurrentThemeName":
                case "CurrentTheme":
                    InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                    InvalidateFloatingObjects();
                    return;

                case "HorizontalScrollBarVisibility":
                case "VerticalScrollBarVisibility":
                case "ReferenceStyle":
                case "Names":
                case "CanCellOverflow":
                case "AutoRefresh":
                case "[OpenXml]":
                    InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                    return;

                case "[OpenExcel]":
                case "[DataCalculated]":
                    Excel.HideOpeningStatusOnOpenExcelCompleted();
                    InvalidateLayout();
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                    Invalidate();
                    return;
            }
        }

        internal static bool HasArrayFormulas(Worksheet sheet, int row, int column, int rowCount, int columnCount)
        {
            object[,] objArray = GetsArrayFormulas(sheet, row, column, rowCount, columnCount);
            return ((objArray != null) && (objArray.Length > 0));
        }

        static bool HasPartArrayFormulas(Worksheet sheet, int row, int column, int rowCount, int columnCount, CellRange exceptedRange)
        {
            object[,] objArray = GetsArrayFormulas(sheet, row, column, rowCount, columnCount);
            if ((objArray != null) && (objArray.Length > 0))
            {
                int length = objArray.GetLength(0);
                for (int i = 0; i < length; i++)
                {
                    CellRange range = (CellRange)objArray[i, 0];
                    if ((exceptedRange == null) || !exceptedRange.Equals(range))
                    {
                        if ((row != -1) && ((range.Row < row) || ((range.Row + range.RowCount) > (row + rowCount))))
                        {
                            return true;
                        }
                        if ((column != -1) && ((range.Column < column) || ((range.Column + range.ColumnCount) > (column + columnCount))))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        static bool HasPartSpans(Worksheet sheet, int row, int column, int rowCount, int columnCount)
        {
            if ((row >= 0) || (column >= 0))
            {
                if (row < 0)
                {
                    SheetSpanModel columnHeaderSpanModel = sheet.ColumnHeaderSpanModel;
                    if ((columnHeaderSpanModel != null) && !columnHeaderSpanModel.IsEmpty())
                    {
                        IEnumerator enumerator = columnHeaderSpanModel.GetEnumerator(-1, column, -1, columnCount);
                        CellRange current = null;
                        while (enumerator.MoveNext())
                        {
                            current = (CellRange)enumerator.Current;
                            if ((current.Column < column) || ((current.Column + current.ColumnCount) > (column + columnCount)))
                            {
                                return true;
                            }
                        }
                    }
                }
                else if (column < 0)
                {
                    SheetSpanModel rowHeaderSpanModel = sheet.RowHeaderSpanModel;
                    if ((rowHeaderSpanModel != null) && !rowHeaderSpanModel.IsEmpty())
                    {
                        IEnumerator enumerator2 = rowHeaderSpanModel.GetEnumerator(row, -1, rowCount, -1);
                        CellRange range2 = null;
                        while (enumerator2.MoveNext())
                        {
                            range2 = (CellRange)enumerator2.Current;
                            if ((range2.Row < row) || ((range2.Row + range2.RowCount) > (row + rowCount)))
                            {
                                return true;
                            }
                        }
                    }
                }
                SheetSpanModel spanModel = sheet.SpanModel;
                if ((spanModel != null) && !spanModel.IsEmpty())
                {
                    IEnumerator enumerator3 = spanModel.GetEnumerator(row, column, rowCount, columnCount);
                    CellRange range3 = null;
                    while (enumerator3.MoveNext())
                    {
                        range3 = (CellRange)enumerator3.Current;
                        if ((row != -1) && ((range3.Row < row) || ((range3.Row + range3.RowCount) > (row + rowCount))))
                        {
                            return true;
                        }
                        if ((column != -1) && ((range3.Column < column) || ((range3.Column + range3.ColumnCount) > (column + columnCount))))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool HasSelectedFloatingObject()
        {
            foreach (IFloatingObject obj2 in GetAllFloatingObjects())
            {
                if (obj2.IsSelected)
                {
                    return true;
                }
            }
            return false;
        }

        bool HasSpans(int row, int column, int rowCount, int columnCount)
        {
            IEnumerable spanModel = ActiveSheet.SpanModel;
            if (spanModel != null)
            {
                foreach (CellRange range in spanModel)
                {
                    if (range.Intersects(row, column, rowCount, columnCount))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool HasTable(Worksheet sheet, int row, int column, int rowCount, int columnCount, bool isInsert)
        {
            int num = (row < 0) ? 0 : row;
            int num2 = (column < 0) ? 0 : column;
            foreach (SheetTable table in sheet.GetTables())
            {
                if (table.Range.Intersects(row, column, rowCount, columnCount))
                {
                    if (!isInsert)
                    {
                        return true;
                    }
                    if ((num > table.Range.Row) || (num2 > table.Range.Column))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool HitTestFloatingObject(int rowViewportIndex, int columnViewportIndex, double mouseX, double mouseY, HitTestInformation hi)
        {
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewportIndex, columnViewportIndex);
            if ((viewportFloatingObjectLayoutModel != null) && (viewportFloatingObjectLayoutModel.Count != 0))
            {
                FloatingObject[] allFloatingObjects = GetAllFloatingObjects();
                foreach (FloatingObject obj2 in SortFloatingObjectByZIndex(allFloatingObjects))
                {
                    FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                    if ((layout != null) && obj2.Visible)
                    {
                        bool isSelected = obj2.IsSelected;
                        double x = layout.X;
                        double y = layout.Y;
                        double width = layout.Width;
                        double height = layout.Height;
                        if (isSelected)
                        {
                            double num5 = 7.0;
                            x -= num5;
                            y -= num5;
                            width += 2.0 * num5;
                            height += 2.0 * num5;
                        }
                        Rect rect = new Rect(x, y, width, height);
                        if (rect.Contains(new Point(mouseX, mouseY)))
                        {
                            ViewportFloatingObjectHitTestInformation information = new ViewportFloatingObjectHitTestInformation();
                            hi.HitTestType = HitTestType.FloatingObject;
                            hi.FloatingObjectInfo = information;
                            information.FloatingObject = obj2;
                            if (!isSelected)
                            {
                                information.InMoving = true;
                                return true;
                            }
                            double num6 = 7.0;
                            double size = 10.0;
                            Rect rect2 = new Rect(x, y, num6, num6);
                            if (InflateRect(rect2, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InTopNWSEResize = true;
                                return true;
                            }
                            Rect rect3 = new Rect((x + (width / 2.0)) - num6, y, 2.0 * num6, num6);
                            if (InflateRect(rect3, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InTopNSResize = true;
                                return true;
                            }
                            Rect rect4 = new Rect((x + width) - num6, y, num6, num6);
                            if (InflateRect(rect4, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InTopNESWResize = true;
                                return true;
                            }
                            Rect rect5 = new Rect(x, (y + (height / 2.0)) - num6, num6, 2.0 * num6);
                            if (InflateRect(rect5, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InLeftWEResize = true;
                                return true;
                            }
                            Rect rect6 = new Rect((x + width) - num6, (y + (height / 2.0)) - num6, num6, 2.0 * num6);
                            if (InflateRect(rect6, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InRightWEResize = true;
                                return true;
                            }
                            Rect rect7 = new Rect(x, (y + height) - num6, num6, num6);
                            if (InflateRect(rect7, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InBottomNESWResize = true;
                                return true;
                            }
                            Rect rect8 = new Rect((x + (width / 2.0)) - num6, (y + height) - num6, 2.0 * num6, num6);
                            if (InflateRect(rect8, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InBottomNSResize = true;
                                return true;
                            }
                            Rect rect9 = new Rect((x + width) - num6, (y + height) - num6, num6, num6);
                            if (InflateRect(rect9, size).Contains(new Point(mouseX, mouseY)))
                            {
                                information.InBottomNWSEResize = true;
                                return true;
                            }
                            information.InMoving = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        bool HitTestPopup(PopupHelper popUpHelper, Point point)
        {
            if (popUpHelper == null)
            {
                return false;
            }
            Rect rect = new Rect(popUpHelper.Location.X, popUpHelper.Location.Y, popUpHelper.Size.Width, popUpHelper.Size.Height);
            return rect.Expand(10, 5).Contains(point);
        }

        string IndexToLetter(int index)
        {
            StringBuilder builder = new StringBuilder();
            while (index > 0)
            {
                builder.Append((char)(0x41 + ((index - 1) % 0x1a)));
                index = (index - 1) / 0x1a;
            }
            for (int i = 0; i < (builder.Length / 2); i++)
            {
                char ch = builder[i];
                builder[i] = builder[(builder.Length - i) - 1];
                builder[(builder.Length - i) - 1] = ch;
            }
            return builder.ToString();
        }

        Rect InflateRect(Rect rect, double size)
        {
            double x = rect.X - size;
            double y = rect.Y - size;
            double width = rect.Width + (2.0 * size);
            double height = rect.Height + (2.0 * size);
            if (width < 0.0)
            {
                width = 0.0;
            }
            if (height < 0.0)
            {
                height = 0.0;
            }
            return new Rect(x, y, width, height);
        }

        bool InitFloatingObjectsMovingResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (IsTouching)
            {
                savedHitTestInformation = _touchStartHitTestInfo;
            }
            if (((savedHitTestInformation.ViewportInfo == null) || (savedHitTestInformation.RowViewportIndex == -2)) || (savedHitTestInformation.ColumnViewportIndex == 2))
            {
                return false;
            }
            _floatingObjectsMovingResizingStartRow = savedHitTestInformation.ViewportInfo.Row;
            _floatingObjectsMovingResizingStartColumn = savedHitTestInformation.ViewportInfo.Column;
            _dragStartRowViewport = savedHitTestInformation.RowViewportIndex;
            _dragStartColumnViewport = savedHitTestInformation.ColumnViewportIndex;
            _dragToRowViewport = savedHitTestInformation.RowViewportIndex;
            _dragToColumnViewport = savedHitTestInformation.ColumnViewportIndex;
            _floatingObjectsMovingResizingStartPoint = savedHitTestInformation.HitPoint;
            SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
            SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
            CachFloatingObjectsMovingResizingLayoutModels();
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(_dragStartRowViewport, _floatingObjectsMovingResizingStartPoint.Y);
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(_dragToColumnViewport, _floatingObjectsMovingResizingStartPoint.X);
            _floatingObjectsMovingResizingStartPointCellBounds = new Rect(viewportColumnLayoutNearX.X, viewportRowLayoutNearY.Y, viewportColumnLayoutNearX.Width, viewportRowLayoutNearY.Height);
            _floatingObjectsMovingStartLocations = new Dictionary<string, Point>();
            FloatingObject[] objArray = _movingResizingFloatingObjects;
            for (int i = 0; i < objArray.Length; i++)
            {
                IFloatingObject obj2 = objArray[i];
                _floatingObjectsMovingStartLocations.Add(obj2.Name, obj2.Location);
            }
            return true;
        }

        void InvaidateViewportHorizontalArrangementInternal(int columnViewportIndex)
        {
            int rowViewportCount = GetViewportInfo().RowViewportCount;
            for (int i = -1; i <= rowViewportCount; i++)
            {
                CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(i, columnViewportIndex);
                if (viewportRowsPresenter != null)
                {
                    viewportRowsPresenter.InvalidateRowsMeasureState(true);
                    viewportRowsPresenter.InvalidateBordersMeasureState();
                    viewportRowsPresenter.InvalidateSelectionMeasureState();
                    viewportRowsPresenter.InvalidateFloatingObjectsMeasureState();
                    viewportRowsPresenter.InvalidateMeasure();
                }
            }
            var columnHeaderRowsPresenter = GetColumnHeaderRowsPresenter(columnViewportIndex);
            if (columnHeaderRowsPresenter != null)
            {
                columnHeaderRowsPresenter.InvalidateRowsMeasureState(true);
                columnHeaderRowsPresenter.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Invalidates the measurement state (layout) and the arranged state (layout) for the control.
        /// The view layout and data is updated after the invalidation.
        /// </summary>
#if ANDROID
        new
#endif
        public void Invalidate()
        {
            if (!IsSuspendInvalidate())
            {
                if (IsEditing)
                {
                    StopCellEditing(true);
                }
                InvalidateLayout();
                Children.Clear();
                _cornerPanel = null;
                _rowHeaders = null;
                _colHeaders = null;
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
                                viewport.RemoveDataValidationUI();
                            }
                        }
                    }
                }
                _cellsPanels = null;
                _groupCornerPresenter = null;
                _rowGroupHeaderPresenter = null;
                _columnGroupHeaderPresenter = null;
                _rowGroupPresenters = null;
                _columnGroupPresenters = null;
                _tooltipHelper = null;
                _currentActiveColumnIndex = (ActiveSheet == null) ? -1 : ActiveSheet.ActiveColumnIndex;
                _currentActiveRowIndex = (ActiveSheet == null) ? -1 : ActiveSheet.ActiveRowIndex;
                Navigation.UpdateStartPosition(_currentActiveRowIndex, _currentActiveColumnIndex);
            }
        }

        /// <summary>
        /// Invalidates the charts.
        /// </summary>
        public void InvalidateCharts()
        {
            if ((ActiveSheet != null) && (ActiveSheet.Charts.Count > 0))
            {
                InvalidateCharts(ActiveSheet.Charts.ToArray());
            }
        }

        /// <summary>
        /// Invalidates the charts.
        /// </summary>
        /// <param name="charts">The charts.</param>
        public void InvalidateCharts(params SpreadChart[] charts)
        {
            InvalidateFloatingObjectLayout();
            foreach (SpreadChart chart in charts)
            {
                RefreshViewportFloatingObjects(chart);
            }
        }

        /// <summary>
        /// Invalidates the column state in the control; the column layout and data is updated after the invalidation.
        /// </summary>
        /// <param name="column">The start column index.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="sheetArea">The invalidated sheet area.</param>
        public void InvalidateColumns(int column, int columnCount, SheetArea sheetArea)
        {
            if (!IsSuspendInvalidate())
            {
                InvalidateRange(-1, column, -1, columnCount, sheetArea);
            }
        }

        /// <summary>
        /// Invalidates the custom floating objects.
        /// </summary>
        public void InvalidateCustomFloatingObjects()
        {
            if ((ActiveSheet != null) && (ActiveSheet.FloatingObjects.Count > 0))
            {
                List<CustomFloatingObject> list = new List<CustomFloatingObject>();
                foreach (FloatingObject obj2 in ActiveSheet.FloatingObjects)
                {
                    if (obj2 is CustomFloatingObject)
                    {
                        list.Add(obj2 as CustomFloatingObject);
                    }
                }
                InvalidateCustomFloatingObjects(list.ToArray());
            }
        }

        /// <summary>
        /// Invalidates the custom floating objects.
        /// </summary>
        /// <param name="floatingObjects">The floating objects.</param>
        public void InvalidateCustomFloatingObjects(params CustomFloatingObject[] floatingObjects)
        {
            InvalidateFloatingObjectLayout();
            foreach (CustomFloatingObject obj2 in floatingObjects)
            {
                RefreshViewportFloatingObjects(obj2);
            }
        }

        internal void InvalidateFloatingObjectLayout()
        {
            InvalidateFloatingObjectsLayoutModel();
            RefreshViewportFloatingObjectsLayout();
        }

        /// <summary>
        /// Invalidates the charts.
        /// </summary>
        public void InvalidateFloatingObjects()
        {
            InvalidateFloatingObjectLayout();
            RefreshViewportFloatingObjects();
        }

        /// <summary>
        /// Invalidates the floating object.
        /// </summary>
        /// <param name="floatingObjects">The floating objects.</param>
        public void InvalidateFloatingObjects(params FloatingObject[] floatingObjects)
        {
            InvalidateFloatingObjectLayout();
            foreach (FloatingObject obj2 in floatingObjects)
            {
                RefreshViewportFloatingObjects(obj2);
            }
        }

        void InvalidateFloatingObjectsLayoutModel()
        {
            _cachedFloatingObjectLayoutModel = null;
        }

        internal void InvalidateHeaderHorizontalArrangement()
        {
            if (!IsSuspendInvalidate())
            {
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                for (int i = -1; i <= rowViewportCount; i++)
                {
                    var rowHeaderRowsPresenter = GetRowHeaderRowsPresenter(i);
                    if (rowHeaderRowsPresenter != null)
                    {
                        rowHeaderRowsPresenter.InvalidateRowsMeasureState(true);
                        rowHeaderRowsPresenter.InvalidateMeasure();
                    }
                }
                if (_cornerPanel != null)
                    _cornerPanel.InvalidateMeasure();
            }
        }

        void InvalidateHeaderRowMeasure(int rowIndex)
        {
            int columnViewportCount = GetViewportInfo().ColumnViewportCount;
            for (int i = -1; i <= columnViewportCount; i++)
            {
                Action<CellLayout> action = null;
                var columnHeaderViewport = GetColumnHeaderRowsPresenter(i);
                if (columnHeaderViewport != null)
                {
                    var objRow = columnHeaderViewport.GetRow(rowIndex);
                    if (objRow != null)
                    {
                        objRow.InvalidateMeasure();
                    }
                    if (action == null)
                    {
                        action = delegate (CellLayout cellLayout)
                        {
                            if ((rowIndex >= cellLayout.Row) && (rowIndex < (cellLayout.Row + cellLayout.RowCount)))
                            {
                                objRow = columnHeaderViewport.GetRow(cellLayout.Row);
                                if (objRow != null)
                                {
                                    objRow.InvalidateMeasure();
                                }
                            }
                        };
                    }
                    Enumerable.ToList<CellLayout>(GetColumnHeaderCellLayoutModel(i)).ForEach<CellLayout>(action);
                }
            }
            if (_cornerPanel != null)
                _cornerPanel.InvalidateMeasure();
        }

        void InvalidateHeaderRowsPresenterMeasure(bool invalidateRowMeasure)
        {
            int columnViewportCount = GetViewportInfo().ColumnViewportCount;
            for (int i = -1; i <= columnViewportCount; i++)
            {
                var columnHeaderRowsPresenter = GetColumnHeaderRowsPresenter(i);
                if (columnHeaderRowsPresenter != null)
                {
                    if (invalidateRowMeasure)
                        columnHeaderRowsPresenter.InvalidateRowsMeasureState(true);
                    columnHeaderRowsPresenter.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Invalidates the pictures.
        /// </summary>
        public void InvalidatePictures()
        {
            if ((ActiveSheet != null) && (ActiveSheet.Pictures.Count > 0))
            {
                InvalidatePictures(ActiveSheet.Pictures.ToArray());
            }
        }

        /// <summary>
        /// Invalidates the pictures.
        /// </summary>
        /// <param name="pictures">The pictures.</param>
        public void InvalidatePictures(params Picture[] pictures)
        {
            InvalidateFloatingObjectLayout();
            foreach (Picture picture in pictures)
            {
                RefreshViewportFloatingObjects(picture);
            }
        }

        /// <summary>
        /// Invalidates a range state in the control; the range layout and data is updated after the invalidation.
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="column">The start column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="sheetArea">The invalidated sheet area.</param>
        public void InvalidateRange(int row, int column, int rowCount, int columnCount, SheetArea sheetArea)
        {
            if (!IsSuspendInvalidate())
            {
                if ((row < 0) || (column < 0))
                {
                    InvalidateLayout();
                }
                _cachedFilterButtonInfoModel = null;
                InvalidateMeasure();
                Worksheet worksheet = ActiveSheet;
                if (((byte)(sheetArea & SheetArea.Cells)) == 1)
                {
                    if (row < 0)
                    {
                        row = 0;
                        rowCount = (worksheet == null) ? 0 : worksheet.RowCount;
                    }
                    if (column < 0)
                    {
                        column = 0;
                        columnCount = (worksheet == null) ? 0 : worksheet.ColumnCount;
                    }
                    _cachedViewportCellLayoutModel = null;
                    RefreshViewportCells(_cellsPanels, row, column, rowCount, columnCount);
                }
                if (((byte)(sheetArea & SheetArea.ColumnHeader)) == 4)
                {
                    if (row < 0)
                    {
                        row = 0;
                        rowCount = (worksheet == null) ? 0 : worksheet.RowCount;
                    }
                    if (column < 0)
                    {
                        column = 0;
                        columnCount = (worksheet == null) ? 0 : worksheet.ColumnCount;
                    }
                    _cachedColumnHeaderCellLayoutModel = null;
                    RefreshHeaderCells(_colHeaders, row, column, rowCount, columnCount);
                }
                if (((byte)(sheetArea & (SheetArea.CornerHeader | SheetArea.RowHeader))) == 2)
                {
                    if (row < 0)
                    {
                        row = 0;
                        rowCount = (worksheet == null) ? 0 : worksheet.RowCount;
                    }
                    if (column < 0)
                    {
                        column = 0;
                        columnCount = (worksheet == null) ? 0 : worksheet.ColumnCount;
                    }
                    _cachedRowHeaderCellLayoutModel = null;
                    RefreshHeaderCells(_rowHeaders, row, column, rowCount, columnCount);
                }
            }
        }

        /// <summary>
        /// Invalidates the row state in the control; the row layout and data is updated after the invalidation.
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="sheetArea">The invalidated sheet area.</param>
        public void InvalidateRows(int row, int rowCount, SheetArea sheetArea)
        {
            if (!IsSuspendInvalidate())
            {
                InvalidateRange(row, -1, rowCount, -1, sheetArea);
            }
        }

        internal void InvalidateViewportColumnsLayout()
        {
            if (!IsSuspendInvalidate())
            {
                _cachedViewportColumnLayoutModel = null;
                _cachedColumnHeaderViewportColumnLayoutModel = null;
                _cachedViewportCellLayoutModel = null;
                _cachedColumnHeaderCellLayoutModel = null;
                _cachedFloatingObjectLayoutModel = null;
            }
        }

        internal void InvalidateViewportHorizontalArrangement(int columnViewportIndex)
        {
            if (!IsSuspendInvalidate())
            {
                int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                if (columnViewportIndex < -1)
                {
                    for (int i = -1; i <= columnViewportCount; i++)
                    {
                        InvaidateViewportHorizontalArrangementInternal(i);
                    }
                }
                else
                {
                    InvaidateViewportHorizontalArrangementInternal(columnViewportIndex);
                }
            }
        }

        void InvalidateViewportRowMeasure(int rowViewportIndex, int rowIndex)
        {
            if (rowViewportIndex < -1)
            {
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                for (int i = -1; i <= rowViewportCount; i++)
                {
                    InvalidateViewportRowMeasureInternal(i, rowIndex);
                }
            }
            else
            {
                InvalidateViewportRowMeasureInternal(rowViewportIndex, rowIndex);
            }
        }

        void InvalidateViewportRowMeasureInternal(int rowViewportIndex, int rowIndex)
        {
            Action<CellLayout> action2 = null;
            int columnViewportCount = GetViewportInfo().ColumnViewportCount;
            for (int i = -1; i <= columnViewportCount; i++)
            {
                Action<CellLayout> action = null;
                CellsPanel viewport = GetViewportRowsPresenter(rowViewportIndex, i);
                if (viewport != null)
                {
                    viewport.InvalidateMeasure();
                    var objRow = viewport.GetRow(rowIndex);
                    if (objRow != null)
                    {
                        objRow.InvalidateMeasure();
                    }
                    if (action == null)
                    {
                        action = delegate (CellLayout cellLayout)
                        {
                            if ((rowIndex >= cellLayout.Row) && (rowIndex < (cellLayout.Row + cellLayout.RowCount)))
                            {
                                objRow = viewport.GetRow(cellLayout.Row);
                                if (objRow != null)
                                {
                                    objRow.InvalidateMeasure();
                                }
                            }
                        };
                    }
                    Enumerable.ToList<CellLayout>(GetViewportCellLayoutModel(rowViewportIndex, i)).ForEach<CellLayout>(action);
                }
            }

            var rowHeaderViewport = GetRowHeaderRowsPresenter(rowViewportIndex);
            if (rowHeaderViewport != null)
            {
                rowHeaderViewport.InvalidateMeasure();
                var row = rowHeaderViewport.GetRow(rowIndex);
                if (row != null)
                {
                    row.InvalidateMeasure();
                }
                if (action2 == null)
                {
                    action2 = delegate (CellLayout cellLayout)
                    {
                        if ((rowIndex >= cellLayout.Row) && (rowIndex < (cellLayout.Row + cellLayout.RowCount)))
                        {
                            row = rowHeaderViewport.GetRow(cellLayout.Row);
                            if (row != null)
                            {
                                row.InvalidateMeasure();
                            }
                        }
                    };
                }
                Enumerable.ToList<CellLayout>(GetRowHeaderCellLayoutModel(rowViewportIndex)).ForEach<CellLayout>(action2);
            }
        }

        internal void InvalidateViewportRowsLayout()
        {
            if (!IsSuspendInvalidate())
            {
                _cachedViewportRowLayoutModel = null;
                _cachedRowHeaderViewportRowLayoutModel = null;
                _cachedViewportCellLayoutModel = null;
                _cachedRowHeaderCellLayoutModel = null;
                _cachedFloatingObjectLayoutModel = null;
            }
        }

        /// <summary>
        /// Invalidate the specified RowsPresenter.
        /// </summary>
        internal void InvalidateViewportRowsPresenterMeasure(int rowViewportIndex, bool invalidateRowsMeasure)
        {
            if (!IsSuspendInvalidate())
            {
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                if (rowViewportIndex < -1)
                {
                    for (int i = -1; i <= rowViewportCount; i++)
                    {
                        InvalidateViewportRowsPresenterMeasureInternal(i, invalidateRowsMeasure);
                    }
                }
                else
                {
                    InvalidateViewportRowsPresenterMeasureInternal(rowViewportIndex, invalidateRowsMeasure);
                }
            }
        }

        void InvalidateViewportRowsPresenterMeasureInternal(int rowViewportIndex, bool invalidateRowsMeasure)
        {
            int columnViewportCount = GetViewportInfo().ColumnViewportCount;
            for (int i = -1; i <= columnViewportCount; i++)
            {
                CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(rowViewportIndex, i);
                if (viewportRowsPresenter != null)
                {
                    viewportRowsPresenter.InvalidateBordersMeasureState();
                    viewportRowsPresenter.InvalidateSelectionMeasureState();
                    viewportRowsPresenter.InvalidateRowsMeasureState(false);
                    viewportRowsPresenter.InvalidateMeasure();
                }
            }
            var rowHeaderRowsPresenter = GetRowHeaderRowsPresenter(rowViewportIndex);
            if (rowHeaderRowsPresenter != null)
            {
                rowHeaderRowsPresenter.InvalidateMeasure();
                rowHeaderRowsPresenter.InvalidateRowsMeasureState(false);
            }
        }

        internal static bool IsAnyCellInRangeLocked(Worksheet sheet, int row, int column, int rowCount, int columnCount)
        {
            if (sheet != null)
            {
                int num = (row < 0) ? 0 : row;
                int num2 = (column < 0) ? 0 : column;
                int num3 = (row < 0) ? sheet.RowCount : rowCount;
                int num4 = (column < 0) ? sheet.ColumnCount : columnCount;
                for (int i = 0; i < num3; i++)
                {
                    for (int j = 0; j < num4; j++)
                    {
                        if (sheet.Cells[num + i, num2 + j].ActualLocked)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        bool IsCellEditable(int rowIndex, int columnIndex)
        {
            // hdt 在报表预览中实现单元格不可编辑且图表可拖动
            if (ActiveSheet != null)
            {
                if (ActiveSheet.LockCell)
                    return false;
                if (ActiveSheet.Protect)
                    return !ActiveSheet.Cells[rowIndex, columnIndex].ActualLocked;
            }
            return true;
        }

        bool IsColumnInViewport(int columnViewport, int column)
        {
            int viewportLeftColumn = GetViewportLeftColumn(columnViewport);
            int viewportRightColumn = GetViewportRightColumn(columnViewport);
            return ((column >= viewportLeftColumn) && (column <= viewportRightColumn));
        }

        bool IsColumnRangeGroupHitTest(Point hitPoint)
        {
            GroupLayout groupLayout = GetGroupLayout();
            if ((ActiveSheet != null) && (groupLayout.Height > 0.0))
            {
                SheetLayout sheetLayout = GetSheetLayout();
                double headerX = sheetLayout.HeaderX;
                double y = groupLayout.Y;
                double width = sheetLayout.HeaderWidth - 1.0;
                double height = groupLayout.Height - 1.0;
                Rect empty = Rect.Empty;
                if ((width >= 0.0) && (height >= 0.0))
                {
                    empty = new Rect(headerX, y, width, height);
                }
                if (empty.Contains(hitPoint))
                {
                    return true;
                }
                int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                for (int i = -1; i <= columnViewportCount; i++)
                {
                    double viewportX = sheetLayout.GetViewportX(i);
                    double num8 = groupLayout.Y;
                    double num9 = groupLayout.Height - 1.0;
                    double num10 = sheetLayout.GetViewportWidth(i) - 1.0;
                    Rect rect2 = Rect.Empty;
                    if ((num9 >= 0.0) && (num10 >= 0.0))
                    {
                        rect2 = new Rect(viewportX, num8, num10, num9);
                    }
                    if (rect2.Contains(hitPoint))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool IsCornerRangeGroupHitTest(Point hitPoint)
        {
            GroupLayout groupLayout = GetGroupLayout();
            if ((groupLayout.Width > 0.0) && (groupLayout.Height > 0.0))
            {
                double x = groupLayout.X;
                double y = groupLayout.Y;
                double width = groupLayout.Width - 1.0;
                double height = groupLayout.Height - 1.0;
                Rect empty = Rect.Empty;
                if ((width >= 0.0) && (height >= 0.0))
                {
                    empty = new Rect(x, y, width, height);
                }
                if (empty.Contains(hitPoint))
                {
                    return true;
                }
            }
            return false;
        }

        bool IsMouseInDragDropLocation(double mouseX, double mouseY, int rowViewportIndex, int columnViewportIndex, bool isTouching = false)
        {
            Worksheet worksheet = ActiveSheet;
            if (worksheet != null)
            {
                int row;
                int column;
                int rowCount;
                int columnCount;
                CellRange spanCell = worksheet.GetSpanCell(worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex);
                if (spanCell == null)
                {
                    spanCell = new CellRange(worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex, 1, 1);
                }
                if (worksheet.Selections.Count > 1)
                {
                    return false;
                }
                if (worksheet.Selections.Count == 1)
                {
                    CellRange range2 = worksheet.Selections[0];
                    row = range2.Row;
                    column = range2.Column;
                    rowCount = range2.RowCount;
                    columnCount = range2.ColumnCount;
                }
                else
                {
                    row = spanCell.Row;
                    column = spanCell.Column;
                    rowCount = spanCell.RowCount;
                    columnCount = spanCell.ColumnCount;
                }
                if ((row == -1) && (column == -1))
                {
                    return false;
                }
                if (row == -1)
                {
                    row = 0;
                    rowCount = worksheet.RowCount;
                }
                if (column == -1)
                {
                    column = 0;
                    columnCount = worksheet.ColumnCount;
                }
                SheetLayout sheetLayout = GetSheetLayout();
                RowLayout layout2 = GetViewportRowLayoutModel(rowViewportIndex).Find(row);
                RowLayout layout3 = GetViewportRowLayoutModel(rowViewportIndex).Find((row + rowCount) - 1);
                ColumnLayout layout4 = GetViewportColumnLayoutModel(columnViewportIndex).Find(column);
                ColumnLayout layout5 = GetViewportColumnLayoutModel(columnViewportIndex).Find((column + columnCount) - 1);
                if (((rowCount < worksheet.RowCount) && (layout2 == null)) && (layout3 == null))
                {
                    return false;
                }
                if (((columnCount < worksheet.ColumnCount) && (layout4 == null)) && (layout5 == null))
                {
                    return false;
                }
                double num5 = Math.Ceiling((layout4 == null) ? sheetLayout.GetViewportX(columnViewportIndex) : layout4.X);
                double num6 = Math.Ceiling((layout5 == null) ? ((double)((sheetLayout.GetViewportX(columnViewportIndex) + sheetLayout.GetViewportWidth(columnViewportIndex)) - 1.0)) : ((double)((layout5.X + layout5.Width) - 1.0)));
                double num7 = Math.Ceiling((layout2 == null) ? sheetLayout.GetViewportY(rowViewportIndex) : layout2.Y);
                double num8 = Math.Ceiling((layout3 == null) ? ((double)((sheetLayout.GetViewportY(rowViewportIndex) + sheetLayout.GetViewportHeight(rowViewportIndex)) - 1.0)) : ((double)((layout3.Y + layout3.Height) - 1.0)));
                double num9 = 2.0;
                double num10 = 1.0;
                if (isTouching)
                {
                    num9 = 10.0;
                    num10 = 5.0;
                }
                if (IsEditing && spanCell.Equals(row, column, rowCount, columnCount))
                {
                    if ((mouseY >= (num7 - num9)) && (mouseY <= (num8 + num10)))
                    {
                        if (((layout4 != null) && (mouseX >= (num5 - num9))) && (mouseX <= (num5 - num10)))
                        {
                            return true;
                        }
                        if (((layout5 != null) && (mouseX >= (num6 + num10))) && (mouseX <= (num6 + num10)))
                        {
                            return true;
                        }
                    }
                    if (((mouseX >= (num5 - num9)) && (mouseX <= (num6 + num10))) && ((((layout2 != null) && (mouseY >= (num7 - num9))) && (mouseY <= (num7 - num10))) || (((layout3 != null) && (mouseY >= (num8 + num10))) && (mouseY <= (num8 + num10)))))
                    {
                        return true;
                    }
                }
                else
                {
                    if ((mouseY >= (num7 - num9)) && (mouseY <= (num8 + num10)))
                    {
                        if (((layout4 != null) && (mouseX >= (num5 - num9))) && (mouseX <= num5))
                        {
                            return true;
                        }
                        if (((layout5 != null) && (mouseX >= (num6 - num10))) && (mouseX <= (num6 + num10)))
                        {
                            return true;
                        }
                    }
                    if ((mouseX >= (num5 - num9)) && (mouseX <= (num6 + num10)))
                    {
                        if (((layout2 != null) && (mouseY >= (num7 - num9))) && (mouseY <= num7))
                        {
                            return true;
                        }
                        if (((layout3 != null) && (mouseY >= (num8 - num10))) && (mouseY <= (num8 + num10)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        bool IsMouseInDragFillIndicator(double mouseX, double mouseY, int rowViewportIndex, int columnViewportIndex, bool isTouching = false)
        {
            int row;
            int column;
            CellRange spanCell;
            double num7;
            double num8;
            Worksheet worksheet = ActiveSheet;
            if (worksheet == null)
            {
                return false;
            }
            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(rowViewportIndex, columnViewportIndex);
            if ((viewportRowsPresenter == null) || !viewportRowsPresenter.SelectionContainer.FocusIndicator.IsFillIndicatorVisible)
            {
                return false;
            }
            FillIndicatorPosition fillIndicatorPosition = viewportRowsPresenter.SelectionContainer.FocusIndicator.FillIndicatorPosition;
            if (worksheet.Selections.Count > 1)
            {
                return false;
            }
            if (worksheet.Selections.Count == 1)
            {
                spanCell = worksheet.Selections[0];
            }
            else
            {
                spanCell = worksheet.GetSpanCell(worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex);
                if (spanCell == null)
                {
                    spanCell = new CellRange(worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex, 1, 1);
                }
            }
            spanCell = AdjustViewportRange(rowViewportIndex, columnViewportIndex, spanCell);
            switch (fillIndicatorPosition)
            {
                case FillIndicatorPosition.BottomRight:
                    row = (spanCell.Row + spanCell.RowCount) - 1;
                    column = (spanCell.Column + spanCell.ColumnCount) - 1;
                    break;

                case FillIndicatorPosition.BottomLeft:
                    row = (spanCell.Row + spanCell.RowCount) - 1;
                    column = spanCell.Column;
                    break;

                default:
                    row = spanCell.Row;
                    column = (spanCell.Column + spanCell.ColumnCount) - 1;
                    break;
            }
            SheetLayout sheetLayout = GetSheetLayout();
            double viewportX = sheetLayout.GetViewportX(columnViewportIndex);
            double viewportY = sheetLayout.GetViewportY(rowViewportIndex);
            double viewportWidth = sheetLayout.GetViewportWidth(columnViewportIndex);
            double viewportHeight = sheetLayout.GetViewportHeight(rowViewportIndex);
            Rect rect = new Rect(viewportX, viewportY, viewportWidth, viewportHeight);
            if (!rect.Contains(new Point(mouseX, mouseY)))
            {
                return false;
            }
            RowLayout layout2 = GetViewportRowLayoutModel(rowViewportIndex).Find(row);
            ColumnLayout layout3 = GetViewportColumnLayoutModel(columnViewportIndex).FindColumn(column);
            if ((layout2 == null) || (layout3 == null))
            {
                return false;
            }
            int num9 = 5;
            double num10 = 3.0;
            switch (fillIndicatorPosition)
            {
                case FillIndicatorPosition.BottomRight:
                    num7 = (layout3.X + layout3.Width) - num10;
                    num8 = (layout2.Y + layout2.Height) - num10;
                    break;

                case FillIndicatorPosition.BottomLeft:
                    num7 = layout3.X + 1.0;
                    num8 = (layout2.Y + layout2.Height) - num10;
                    break;

                default:
                    num7 = (layout3.X + layout3.Width) - num10;
                    num8 = layout2.Y + 1.0;
                    break;
            }
            Point point = new Point(mouseX, mouseY);
            if (IsTouching)
            {
                if (IsEditing)
                {
                    return false;
                }
                double x = Math.Max((double)0.0, (double)(num7 - 15.0));
                double y = Math.Max((double)0.0, (double)(num8 - 5.0));
                Rect rect2 = new Rect(x, y, 30.0, 25.0);
                return rect2.Contains(point);
            }
            Rect rect3 = new Rect(num7, num8, (double)num9, (double)num9);
            if (!IsEditing)
            {
                return rect3.Contains(point);
            }
            Rect empty = Rect.Empty;
            switch (fillIndicatorPosition)
            {
                case FillIndicatorPosition.BottomRight:
                    empty = new Rect(num7, num8, 2.0, 2.0);
                    break;

                case FillIndicatorPosition.TopRight:
                    empty = new Rect(num7, num8, 2.0, (double)num9);
                    break;

                case FillIndicatorPosition.BottomLeft:
                    empty = new Rect(num7, num8, (double)num9, 2.0);
                    break;
            }
            return (rect3.Contains(point) && !empty.Contains(point));
        }


        bool IsNeedRefreshFloatingObjectsMovingResizingContainer(int rowViewport, int columnViewport)
        {
            return true;
        }

        static bool IsPastedInternal(Worksheet srcSheet, CellRange srcRange, Worksheet destSheet, string clipboadText)
        {
            string str = null;
            if ((srcSheet != null) && (srcRange != null))
            {
                str = srcSheet.GetCsv(srcRange.Row, srcRange.Column, srcRange.RowCount, srcRange.ColumnCount, "\r\n", "\t", "\"", false);
                if (str == string.Empty)
                {
                    str = null;
                }
            }
            return ((((srcSheet != null) && (srcRange != null)) && ((destSheet.Workbook != null) && (destSheet.Workbook == srcSheet.Workbook))) && (str == clipboadText));
        }

        static bool IsRangesEqual(CellRange[] oldSelection, CellRange[] newSelection)
        {
            int num = (oldSelection == null) ? 0 : oldSelection.Length;
            int num2 = (newSelection == null) ? 0 : newSelection.Length;
            bool flag = true;
            if (num == num2)
            {
                for (int i = 0; i < num; i++)
                {
                    if (!object.Equals(oldSelection[i], newSelection[i]))
                    {
                        return false;
                    }
                }
                return flag;
            }
            return false;
        }

        bool IsRowInViewport(int rowViewport, int row)
        {
            int viewportTopRow = GetViewportTopRow(rowViewport);
            int viewportBottomRow = GetViewportBottomRow(rowViewport);
            return ((row >= viewportTopRow) && (row <= viewportBottomRow));
        }

        bool IsRowRangeGroupHitTest(Point hitPoint)
        {
            GroupLayout groupLayout = GetGroupLayout();
            if ((ActiveSheet != null) && (groupLayout.Width > 0.0))
            {
                SheetLayout sheetLayout = GetSheetLayout();
                double x = groupLayout.X;
                double headerY = sheetLayout.HeaderY;
                double width = groupLayout.Width - 1.0;
                double height = sheetLayout.HeaderHeight - 1.0;
                Rect empty = Rect.Empty;
                if ((width >= 0.0) && (height >= 0.0))
                {
                    empty = new Rect(x, headerY, width, height);
                }
                if (empty.Contains(hitPoint))
                {
                    return true;
                }
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                for (int i = -1; i <= rowViewportCount; i++)
                {
                    double num7 = groupLayout.X;
                    double viewportY = sheetLayout.GetViewportY(i);
                    double num9 = groupLayout.Width - 1.0;
                    double num10 = sheetLayout.GetViewportHeight(i) - 1.0;
                    Rect rect2 = Rect.Empty;
                    if ((num9 >= 0.0) && (num10 >= 0.0))
                    {
                        rect2 = new Rect(num7, viewportY, num9, num10);
                    }
                    if (rect2.Contains(hitPoint))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool IsValidRange(int row, int column, int rowCount, int columnCount, int maxRowCount, int maxColumnCount)
        {
            if (((-1 <= row) && (row < maxRowCount)) && ((-1 <= column) && (column < maxColumnCount)))
            {
                if ((row == -1) && (column == -1))
                {
                    return true;
                }
                if (row == -1)
                {
                    if ((columnCount != 0) && ((column + columnCount) <= maxColumnCount))
                    {
                        return true;
                    }
                }
                else if (column == -1)
                {
                    if ((rowCount != 0) && ((row + rowCount) <= maxRowCount))
                    {
                        return true;
                    }
                }
                else if (((columnCount != 0) && ((column + columnCount) <= maxColumnCount)) && ((rowCount != 0) && ((row + rowCount) <= maxRowCount)))
                {
                    return true;
                }
            }
            return false;
        }

        double MeasureCellText(Cell cell, int row, int column, Size maxSize, FontFamily fontFamily, object textFormattingMode, bool useLayoutRounding)
        {
            double num = 0.0;
            Size size2 = MeasureHelper.ConvertTextSizeToExcelCellSize(MeasureHelper.MeasureTextInCell(cell, maxSize, 1.0, fontFamily, textFormattingMode, useLayoutRounding), 1.0);
            num = Math.Max(num, size2.Width);
            if (!ContainsFilterButton(row, column, cell.SheetArea))
            {
                return num;
            }
            switch (cell.ToHorizontalAlignment())
            {
                case HorizontalAlignment.Right:
                    return num;

                case HorizontalAlignment.Center:
                    return Math.Max(num, size2.Width + 36.0);
            }
            return Math.Max(num, size2.Width + 16.0);
        }

        internal void MeasureRangeGroup(int rowPaneCount, int columnPaneCount, SheetLayout layout)
        {
            GroupLayout groupLayout = GetGroupLayout();
            if ((_rowGroupPresenters != null) && ((ActiveSheet == null) || (_rowGroupPresenters.Length != rowPaneCount)))
            {
                foreach (GcRangeGroup group in _rowGroupPresenters)
                {
                    base.Children.Remove(group);
                }
                _rowGroupPresenters = null;
            }
            if (_rowGroupPresenters == null)
            {
                _rowGroupPresenters = new GcRangeGroup[rowPaneCount + 2];
            }
            if (groupLayout.Width > 0.0)
            {
                for (int i = -1; i <= rowPaneCount; i++)
                {
                    double viewportY = layout.GetViewportY(i);
                    double viewportHeight = layout.GetViewportHeight(i);
                    if (_rowGroupPresenters[i + 1] == null)
                    {
                        GcRangeGroup group2 = new GcRangeGroup(this);
                        _rowGroupPresenters[i + 1] = group2;
                    }
                    GcRangeGroup group3 = _rowGroupPresenters[i + 1];
                    group3.Orientation = Orientation.Horizontal;
                    group3.ViewportIndex = i;
                    group3.Location = new Point(groupLayout.X, viewportY);
                    if (viewportHeight > 0.0)
                    {
                        if (!base.Children.Contains(group3))
                        {
                            base.Children.Add(group3);
                        }
                        group3.InvalidateMeasure();
                        group3.Measure(new Size(groupLayout.Width, viewportHeight));
                    }
                    else
                    {
                        base.Children.Remove(group3);
                        _rowGroupPresenters[i + 1] = null;
                    }
                }
            }
            else
            {
                GcRangeGroup[] groupArray2 = _rowGroupPresenters;
                for (int j = 0; j < groupArray2.Length; j++)
                {
                    GcGroupBase base2 = groupArray2[j];
                    base.Children.Remove(base2);
                }
            }
            if ((_columnGroupPresenters != null) && ((ActiveSheet == null) || (_columnGroupPresenters.Length != columnPaneCount)))
            {
                foreach (GcRangeGroup group4 in _columnGroupPresenters)
                {
                    base.Children.Remove(group4);
                }
                _columnGroupPresenters = null;
            }
            if (_columnGroupPresenters == null)
            {
                _columnGroupPresenters = new GcRangeGroup[columnPaneCount + 2];
            }
            if (groupLayout.Height > 0.0)
            {
                for (int k = -1; k <= columnPaneCount; k++)
                {
                    double viewportX = layout.GetViewportX(k);
                    double viewportWidth = layout.GetViewportWidth(k);
                    if (_columnGroupPresenters[k + 1] == null)
                    {
                        GcRangeGroup group5 = new GcRangeGroup(this);
                        _columnGroupPresenters[k + 1] = group5;
                    }
                    GcRangeGroup group6 = _columnGroupPresenters[k + 1];
                    group6.Orientation = Orientation.Vertical;
                    group6.ViewportIndex = k;
                    group6.Location = new Point(viewportX, groupLayout.Y);
                    if (viewportWidth > 0.0)
                    {
                        if (!base.Children.Contains(group6))
                        {
                            base.Children.Add(group6);
                        }
                        group6.InvalidateMeasure();
                        group6.Measure(new Size(viewportWidth, groupLayout.Height));
                    }
                    else
                    {
                        base.Children.Remove(group6);
                        _columnGroupPresenters[k + 1] = null;
                    }
                }
            }
            else
            {
                GcRangeGroup[] groupArray4 = _columnGroupPresenters;
                for (int m = 0; m < groupArray4.Length; m++)
                {
                    GcGroupBase base3 = groupArray4[m];
                    base.Children.Remove(base3);
                }
            }
            if (_rowGroupHeaderPresenter == null)
            {
                _rowGroupHeaderPresenter = new GcRangeGroupHeader(this);
            }
            _rowGroupHeaderPresenter.Orientation = Orientation.Horizontal;
            _rowGroupHeaderPresenter.Location = new Point(groupLayout.X, groupLayout.Y + groupLayout.Height);
            if (groupLayout.Width > 0.0)
            {
                if (!base.Children.Contains(_rowGroupHeaderPresenter))
                {
                    base.Children.Add(_rowGroupHeaderPresenter);
                }
                _rowGroupHeaderPresenter.InvalidateMeasure();
                _rowGroupHeaderPresenter.Measure(new Size(groupLayout.Width, layout.HeaderHeight));
            }
            else
            {
                base.Children.Remove(_rowGroupHeaderPresenter);
                _rowGroupHeaderPresenter = null;
            }
            if (_columnGroupHeaderPresenter == null)
            {
                _columnGroupHeaderPresenter = new GcRangeGroupHeader(this);
            }
            _columnGroupHeaderPresenter.Orientation = Orientation.Vertical;
            _columnGroupHeaderPresenter.Location = new Point(groupLayout.X + groupLayout.Width, groupLayout.Y);
            if (groupLayout.Height > 0.0)
            {
                if (!base.Children.Contains(_columnGroupHeaderPresenter))
                {
                    base.Children.Add(_columnGroupHeaderPresenter);
                }
                _columnGroupHeaderPresenter.InvalidateMeasure();
                _columnGroupHeaderPresenter.Measure(new Size(layout.HeaderWidth, groupLayout.Height));
            }
            else
            {
                base.Children.Remove(_columnGroupHeaderPresenter);
                _columnGroupHeaderPresenter = null;
            }
            if (_groupCornerPresenter == null)
            {
                _groupCornerPresenter = new GcRangeGroupCorner(this);
            }
            _groupCornerPresenter.Location = new Point(groupLayout.X, groupLayout.Y);
            if ((groupLayout.Width > 0.0) && (groupLayout.Height > 0.0))
            {
                if (!base.Children.Contains(_groupCornerPresenter))
                {
                    base.Children.Add(_groupCornerPresenter);
                }
                _groupCornerPresenter.InvalidateMeasure();
                _groupCornerPresenter.Measure(new Size(groupLayout.Width, groupLayout.Height));
            }
            else
            {
                base.Children.Remove(_groupCornerPresenter);
                _groupCornerPresenter = null;
            }
        }

        void MoveActiveCellToBottom()
        {
            CellRange activeSelection = GetActiveSelection();
            if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
            {
                activeSelection = ActiveSheet.Selections[0];
            }
            if ((ActiveSheet.ActiveRowIndex != ((activeSelection.Row + activeSelection.RowCount) - 1)) || (ActiveSheet.ActiveColumnIndex != ((activeSelection.Column + activeSelection.ColumnCount) - 1)))
            {
                ActiveSheet.Workbook.SuspendEvent();
                ActiveSheet.SetActiveCell((activeSelection.Row + activeSelection.RowCount) - 1, (activeSelection.Column + activeSelection.ColumnCount) - 1, false);
                ActiveSheet.Workbook.ResumeEvent();
            }
        }

        bool NeedRefresh(int rowViewport, int columnViewport)
        {
            bool flag = false;
            bool flag2 = false;
            ViewportInfo viewportInfo = GetViewportInfo();
            if (IsDragFillWholeColumns)
            {
                if (ActiveSheet.FrozenRowCount == 0)
                {
                    flag = (rowViewport == _dragToRowViewport) || (rowViewport == viewportInfo.RowViewportCount);
                }
                else if (_dragToRowViewport >= 1)
                {
                    flag = ((rowViewport == -1) || (rowViewport == viewportInfo.RowViewportCount)) || (rowViewport == _dragToRowViewport);
                }
                else
                {
                    flag = ((rowViewport == -1) || (rowViewport == viewportInfo.RowViewportCount)) || (rowViewport == 0);
                }
                flag2 = ((columnViewport == _dragFillStartLeftColumnViewport) || (columnViewport == _dragFillStartRightColumnViewport)) || (columnViewport == _dragToColumnViewport);
            }
            else if (IsDragFillWholeRows)
            {
                if (ActiveSheet.FrozenColumnCount == 0)
                {
                    flag2 = (columnViewport == _dragToColumnViewport) || (columnViewport == viewportInfo.ColumnViewportCount);
                }
                else if (_dragToColumnViewport >= 1)
                {
                    flag2 = ((columnViewport == -1) || (columnViewport == viewportInfo.ColumnViewportCount)) || (columnViewport == _dragToColumnViewport);
                }
                else
                {
                    flag2 = ((columnViewport == -1) || (columnViewport == viewportInfo.ColumnViewportCount)) || (columnViewport == 0);
                }
                flag = ((rowViewport == _dragFillStartTopRowViewport) || (rowViewport == _dragFillStartBottomRowViewport)) || (rowViewport == _dragToRowViewport);
            }
            else
            {
                flag = ((rowViewport >= _dragFillStartTopRowViewport) && (rowViewport <= _dragFillStartBottomRowViewport)) || (rowViewport == _dragToRowViewport);
                flag2 = ((columnViewport >= _dragFillStartLeftColumnViewport) && (columnViewport <= _dragFillStartRightColumnViewport)) || (columnViewport == _dragToColumnViewport);
            }
            return (flag && flag2);
        }

        internal void OnActiveSheetChanged()
        {
            if (EditorConnector.IsFormulaSelectionBegined)
            {
                EditorConnector.UpdateSelectionItemsForCurrentSheet();
                EditorConnector.ActivateEditor = true;
            }
            Invalidate();
        }

        void OnEditedCellChanged(object sender, CellChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "Value"))
            {
                RaiseValueChanged(e.Row, e.Column);
            }
        }

        void PrepareCellEditing()
        {
            if (IsCellEditable(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex))
            {
                CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
                if (viewportRowsPresenter != null)
                {
                    viewportRowsPresenter.PrepareCellEditing(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
                }
            }
        }

        bool PreviewLeaveCell(int row, int column)
        {
            return (((row != ActiveSheet.ActiveRowIndex) || (column != ActiveSheet.ActiveColumnIndex)) && RaiseLeaveCell(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, row, column));
        }

        internal void ProcessStartSheetIndexChanged()
        {
            if (((ActiveSheet != null) && (ActiveSheet.Workbook != null)) && (_tabStrip != null))
            {
                _tabStrip.SetStartSheet(ActiveSheet.Workbook.StartSheetIndex);
            }
        }

        internal void ProcessTextInput(string c, bool replace, bool justInputText = false)
        {
            if (!justInputText)
            {
                bool flag;
                bool flag2;
                KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                if (flag2)
                {
                    return;
                }
            }
            if (IsEditing)
            {
                CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
                if (viewportRowsPresenter != null)
                {
                    viewportRowsPresenter.SendFirstKey(c, replace);
                }
            }
        }

        internal string RaiseCellTextRendering(int row, int column, string text)
        {
            if (CellTextRendering != null)
            {
                CellTextRenderingEventArgs args = new CellTextRenderingEventArgs(row, column, text);
                CellTextRendering(this, args);
                return args.CellText;
            }
            return text;
        }

        internal object RaiseCellValueApplying(int row, int column, object value)
        {
            if (CellValueApplying != null)
            {
                CellValueApplyingEventArgs args = new CellValueApplyingEventArgs(row, column, value);
                CellValueApplying(this, args);
                return args.CellValue;
            }
            return value;
        }

        internal void RaiseClipboardChanged()
        {
            if ((ClipboardChanged != null) && (_eventSuspended == 0))
            {
                ClipboardChanged(this, EventArgs.Empty);
            }
        }

        internal void RaiseClipboardChanging()
        {
            if ((ClipboardChanging != null) && (_eventSuspended == 0))
            {
                ClipboardChanging(this, EventArgs.Empty);
            }
        }

        internal void RaiseClipboardPasted(Worksheet sourceSheet, CellRange sourceRange, Worksheet worksheet, CellRange cellRange, ClipboardPasteOptions pastOption)
        {
            if ((ClipboardPasted != null) && (_eventSuspended == 0))
            {
                ClipboardPasted(this, new ClipboardPastedEventArgs(sourceSheet, sourceRange, worksheet, cellRange, pastOption));
            }
        }

        internal bool RaiseClipboardPasting(Worksheet sourceSheet, CellRange sourceRange, Worksheet worksheet, CellRange cellRange, ClipboardPasteOptions pastOption, bool isCutting, out ClipboardPasteOptions newPastOption)
        {
            newPastOption = pastOption;
            if ((ClipboardPasting != null) && (_eventSuspended == 0))
            {
                ClipboardPastingEventArgs args = new ClipboardPastingEventArgs(sourceSheet, sourceRange, worksheet, cellRange, pastOption, isCutting);
                ClipboardPasting(this, args);
                newPastOption = args.PasteOption;
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseColumnWidthChanged(int[] columnList, bool header)
        {
            if ((ColumnWidthChanged != null) && (_eventSuspended == 0))
            {
                ColumnWidthChanged(this, new ColumnWidthChangedEventArgs(columnList, header));
            }
        }

        internal bool RaiseColumnWidthChanging(int[] columnList, bool header)
        {
            if ((ColumnWidthChanging != null) && (_eventSuspended == 0))
            {
                ColumnWidthChangingEventArgs args = new ColumnWidthChangingEventArgs(columnList, header);
                ColumnWidthChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool RaiseDragDropBlock(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, bool copy, bool insert, CopyToOption copyOption, out CopyToOption newCopyOption)
        {
            newCopyOption = copyOption;
            if ((DragDropBlock != null) && (_eventSuspended == 0))
            {
                DragDropBlockEventArgs args = new DragDropBlockEventArgs(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, copy, insert, copyOption);
                DragDropBlock(this, args);
                newCopyOption = args.CopyOption;
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseDragDropBlockCompleted(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, bool copy, bool insert, CopyToOption copyOption)
        {
            if ((DragDropBlockCompleted != null) && (_eventSuspended == 0))
            {
                DragDropBlockCompleted(this, new DragDropBlockCompletedEventArgs(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, copy, insert, copyOption));
            }
        }

        internal bool RaiseDragFillBlock(CellRange fillRange, FillDirection fillDirection, AutoFillType fillType)
        {
            if ((DragFillBlock != null) && (_eventSuspended == 0))
            {
                DragFillBlockEventArgs args = new DragFillBlockEventArgs(fillRange, fillDirection, fillType);
                DragFillBlock(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseDragFillBlockCompleted(CellRange fillRange, FillDirection fillDirection, AutoFillType fillType)
        {
            if ((DragFillBlockCompleted != null) && (_eventSuspended == 0))
            {
                DragFillBlockCompleted(this, new DragFillBlockCompletedEventArgs(fillRange, fillDirection, fillType));
            }
        }

        internal void RaiseEditChange(int row, int column)
        {
            if ((EditChange != null) && (_eventSuspended == 0))
            {
                EditChange(this, new EditCellEventArgs(row, column));
            }
        }

        internal void RaiseEditEnd(int row, int column)
        {
            if ((EditEnd != null) && (_eventSuspended == 0))
            {
                EditEnd(this, new EditCellEventArgs(row, column));
            }
        }

        internal bool RaiseEditStarting(int row, int column)
        {
            if ((EditStarting != null) && (_eventSuspended == 0))
            {
                EditCellStartingEventArgs args = new EditCellStartingEventArgs(row, column);
                EditStarting(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseEnterCell(int row, int column)
        {
            if ((EnterCell != null) && (_eventSuspended == 0))
            {
                EnterCellEventArgs args = new EnterCellEventArgs(row, column);
                EnterCell(this, args);
            }
        }

        /// <summary>
        /// Raises the error.
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="column">The column</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="exception">The exception</param>
        /// <returns>Return if ignore the error</returns>
        internal bool RaiseError(int row, int column, string errorMessage, Exception exception)
        {
            if ((Error != null) && (_eventSuspended == 0))
            {
                UserErrorEventArgs args = new UserErrorEventArgs(this, row, column, errorMessage, exception);
                Error(this, args);
                return args.Cancel;
            }
            return false;
        }

        internal bool RaiseFilterPopupOpening(int row, int column)
        {
            if (FilterPopupOpening != null)
            {
                CellCancelEventArgs args = new CellCancelEventArgs(row, column);
                FilterPopupOpening(this, args);
                return args.Cancel;
            }
            return false;
        }

        internal void RaiseFloatingObjectPasted(Worksheet worksheet, FloatingObject pastedObject)
        {
            if ((FloatingObjectPasted != null) && (_eventSuspended == 0))
            {
                FloatingObjectPasted(this, new FloatingObjectPastedEventArgs(worksheet, pastedObject));
            }
        }

        internal void RaiseInvalidOperation(string message, string operation = null, object context = null)
        {
            if ((InvalidOperation != null) && (_eventSuspended == 0))
            {
                InvalidOperationEventArgs args = new InvalidOperationEventArgs(message, operation, context);
                InvalidOperation(this, args);
            }
        }

        internal bool RaiseLeaveCell(int row, int column, int toRow, int toColumn)
        {
            if ((LeaveCell != null) && (_eventSuspended == 0))
            {
                LeaveCellEventArgs args = new LeaveCellEventArgs(row, column, toRow, toColumn);
                LeaveCell(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        void RaiseLeftChanged(int oldIndex, int newIndex, int viewportIndex)
        {
            if ((LeftColumnChanged != null) && (_eventSuspended == 0))
            {
                LeftColumnChanged(this, new ViewportEventArgs(oldIndex, newIndex, viewportIndex));
            }
        }

        internal void RaiseRangeFiltered(int column, object[] filterValues)
        {
            if ((RangeFiltered != null) && (_eventSuspended == 0))
            {
                RangeFiltered(this, new RangeFilteredEventArgs(column, filterValues));
            }
        }

        internal bool RaiseRangeFiltering(int column, object[] filterValues)
        {
            if ((RangeFiltering != null) && (_eventSuspended == 0))
            {
                RangeFilteringEventArgs args = new RangeFilteringEventArgs(column, filterValues);
                RangeFiltering(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseRangeGroupStateChanged(bool isRowGroup, int index, int level)
        {
            if ((RangeGroupStateChanged != null) && (_eventSuspended == 0))
            {
                RangeGroupStateChanged(this, new RangeGroupStateChangedEventArgs(isRowGroup, index, level));
            }
        }

        internal bool RaiseRangeGroupStateChanging(bool isRowGroup, int index, int level)
        {
            if ((RangeGroupStateChanging != null) && (_eventSuspended == 0))
            {
                RangeGroupStateChangingEventArgs args = new RangeGroupStateChangingEventArgs(isRowGroup, index, level);
                RangeGroupStateChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseRangeSorted(int column, bool isAscending)
        {
            if ((RangeSorted != null) && (_eventSuspended == 0))
            {
                RangeSorted(this, new RangeSortedEventArgs(column, isAscending));
            }
        }

        internal bool RaiseRangeSorting(int column, bool isAscending)
        {
            if ((RangeSorting != null) && (_eventSuspended == 0))
            {
                RangeSortingEventArgs args = new RangeSortingEventArgs(column, isAscending);
                RangeSorting(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseRowHeightChanged(int[] rowList, bool header)
        {
            if ((RowHeightChanged != null) && (_eventSuspended == 0))
            {
                RowHeightChanged(this, new RowHeightChangedEventArgs(rowList, header));
            }
        }

        internal bool RaiseRowHeightChanging(int[] rowList, bool header)
        {
            if ((RowHeightChanging != null) && (_eventSuspended == 0))
            {
                RowHeightChangingEventArgs args = new RowHeightChangingEventArgs(rowList, header);
                RowHeightChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        internal void RaiseSheetTabClick(int sheetTabIndex)
        {
            if ((SheetTabClick != null) && (_eventSuspended == 0))
            {
                SheetTabClick(this, new SheetTabClickEventArgs(sheetTabIndex));
            }
        }

        internal void RaiseSheetTabDoubleClick(int sheetTabIndex)
        {
            if ((SheetTabDoubleClick != null) && (_eventSuspended == 0))
            {
                SheetTabDoubleClick(this, new SheetTabDoubleClickEventArgs(sheetTabIndex));
            }
        }

        void RaiseTopChanged(int oldIndex, int newIndex, int viewportIndex)
        {
            if ((TopRowChanged != null) && (_eventSuspended == 0))
            {
                TopRowChanged(this, new ViewportEventArgs(oldIndex, newIndex, viewportIndex));
            }
        }

        internal void RaiseTouchCellClick(HitTestInformation hi)
        {
            if ((CellClick != null) && (_eventSuspended == 0))
            {
                CellClickEventArgs args = null;
                Point point = new Point(-1.0, -1.0);
                if (hi.HitTestType == HitTestType.Viewport)
                {
                    args = CreateCellClickEventArgs(hi.ViewportInfo.Row, hi.ViewportInfo.Column, ActiveSheet.SpanModel, SheetArea.Cells, MouseButtonType.Left);
                    point = new Point((double)hi.ViewportInfo.Row, (double)hi.ViewportInfo.Column);
                }
                else if (hi.HitTestType == HitTestType.RowHeader)
                {
                    args = CreateCellClickEventArgs(hi.ViewportInfo.Row, hi.ViewportInfo.Column, ActiveSheet.SpanModel, SheetArea.CornerHeader | SheetArea.RowHeader, MouseButtonType.Left);
                    point = new Point((double)hi.HeaderInfo.Row, (double)hi.HeaderInfo.Column);
                }
                else if (hi.HitTestType == HitTestType.ColumnHeader)
                {
                    args = CreateCellClickEventArgs(hi.ViewportInfo.Row, hi.ViewportInfo.Column, ActiveSheet.SpanModel, SheetArea.ColumnHeader, MouseButtonType.Left);
                    point = new Point((double)hi.HeaderInfo.Row, (double)hi.HeaderInfo.Column);
                }
                if (((args != null) && (point.X != -1.0)) && (point.Y != -1.0))
                {
                    CellClick(this, args);
                }
            }
        }

        internal bool RaiseTouchToolbarOpeningEvent(Point touchPoint, TouchToolbarShowingArea area)
        {
            if ((TouchToolbarOpening != null) && (_eventSuspended == 0))
            {
                TouchToolbarOpeningEventArgs args = new TouchToolbarOpeningEventArgs((int)touchPoint.X, (int)touchPoint.Y, area);
                TouchToolbarOpening(this, args);
                return false;
            }
            return true;
        }

        internal void RaiseUserFormulaEntered(int row, int column, string formula)
        {
            if ((UserFormulaEntered != null) && (_eventSuspended == 0))
            {
                if (formula != null)
                {
                    formula = formula.ToUpperInvariant();
                }
                else
                {
                    formula = "";
                }
                UserFormulaEntered(this, new UserFormulaEnteredEventArgs(row, column, formula));
            }
        }

        internal void RaiseUserZooming(float oldZoomFactor, float newZoomFactor)
        {
            if ((UserZooming != null) && (_eventSuspended == 0))
            {
                UserZooming(this, new ZoomEventArgs(oldZoomFactor, newZoomFactor));
            }
        }

        internal void RaiseValueChanged(int row, int column)
        {
            if ((ValueChanged != null) && (_eventSuspended == 0))
            {
                ValueChanged(this, new CellEventArgs(row, column));
            }
        }

        internal void RefreshCellAreaViewport(int row, int column, int rowCount, int columnCount)
        {
            RefreshViewportCells(_cellsPanels, 0, 0, rowCount, columnCount);
        }

        void RefreshDragDropIndicator(int dragToRowViewportIndex, int dragToColumnViewportIndex, int dragToRow, int dragToColumn)
        {
            RowLayout layout = GetViewportRowLayoutModel(dragToRowViewportIndex).FindRow(dragToRow);
            ColumnLayout layout2 = GetViewportColumnLayoutModel(dragToColumnViewportIndex).FindColumn(dragToColumn);
            if ((layout != null) && (layout2 != null))
            {
                _dragDropInsertIndicator.Visibility = Visibility.Collapsed;
                int row = _dragDropFromRange.Row;
                int column = _dragDropFromRange.Column;
                int rowCount = _dragDropFromRange.RowCount;
                int columnCount = _dragDropFromRange.ColumnCount;
                int num5 = (row < 0) ? -1 : Math.Max(0, Math.Min((int)(ActiveSheet.RowCount - rowCount), (int)(dragToRow - _dragDropRowOffset)));
                int num6 = (column < 0) ? -1 : Math.Max(0, Math.Min((int)(ActiveSheet.ColumnCount - columnCount), (int)(dragToColumn - _dragDropColumnOffset)));
                int index = (num6 < 0) ? 0 : num6;
                int num8 = (num6 < 0) ? (ActiveSheet.ColumnCount - 1) : ((index + columnCount) - 1);
                int num9 = (num5 < 0) ? 0 : num5;
                int num10 = (num5 < 0) ? (ActiveSheet.RowCount - 1) : ((num9 + rowCount) - 1);
                int columnViewportIndex = dragToColumnViewportIndex;
                int num12 = dragToColumnViewportIndex;
                int rowViewportIndex = dragToRowViewportIndex;
                int num14 = dragToRowViewportIndex;
                int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                if ((ActiveSheet.FrozenColumnCount > 0) && ((dragToColumnViewportIndex == -1) || (dragToColumnViewportIndex == 0)))
                {
                    if (index < ActiveSheet.FrozenColumnCount)
                    {
                        columnViewportIndex = -1;
                    }
                    if (num8 < ActiveSheet.FrozenColumnCount)
                    {
                        num12 = -1;
                    }
                    else if (((columnViewportCount == 1) && (ActiveSheet.FrozenTrailingColumnCount > 0)) && (num8 >= (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
                    {
                        num12 = 1;
                    }
                    else
                    {
                        num12 = 0;
                    }
                }
                else if ((ActiveSheet.FrozenTrailingColumnCount > 0) && ((dragToColumnViewportIndex == (columnViewportCount - 1)) || (dragToColumnViewportIndex == columnViewportCount)))
                {
                    if (index < (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount))
                    {
                        if (((columnViewportCount == 1) && (ActiveSheet.FrozenColumnCount > 0)) && (index < ActiveSheet.FrozenColumnCount))
                        {
                            columnViewportIndex = -1;
                        }
                        else
                        {
                            columnViewportIndex = columnViewportCount - 1;
                        }
                        if (num8 < (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount))
                        {
                            num12 = columnViewportCount - 1;
                        }
                        else
                        {
                            num12 = columnViewportCount;
                        }
                    }
                    else
                    {
                        columnViewportIndex = columnViewportCount;
                        num12 = columnViewportCount;
                    }
                }
                if ((ActiveSheet.FrozenRowCount > 0) && ((dragToRowViewportIndex == -1) || (dragToRowViewportIndex == 0)))
                {
                    if (num5 < ActiveSheet.FrozenRowCount)
                    {
                        rowViewportIndex = -1;
                    }
                    if (num10 < ActiveSheet.FrozenRowCount)
                    {
                        num14 = -1;
                    }
                    else if (((rowViewportCount == 1) && (ActiveSheet.FrozenTrailingRowCount > 0)) && (num10 >= (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
                    {
                        num14 = 1;
                    }
                    else
                    {
                        num14 = 0;
                    }
                }
                else if ((ActiveSheet.FrozenTrailingRowCount > 0) && ((dragToRowViewportIndex == (rowViewportCount - 1)) || (dragToRowViewportIndex == rowViewportCount)))
                {
                    if (num9 < (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount))
                    {
                        if (((rowViewportCount == 1) && (ActiveSheet.FrozenRowCount > 0)) && (num9 < ActiveSheet.FrozenRowCount))
                        {
                            rowViewportIndex = -1;
                        }
                        else
                        {
                            rowViewportIndex = rowViewportCount - 1;
                        }
                        if (num10 < (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount))
                        {
                            num14 = rowViewportCount - 1;
                        }
                        else
                        {
                            num14 = rowViewportCount;
                        }
                    }
                    else
                    {
                        rowViewportIndex = rowViewportCount;
                        num14 = rowViewportCount;
                    }
                }
                ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(columnViewportIndex);
                ColumnLayoutModel model2 = viewportColumnLayoutModel;
                if (num12 != columnViewportIndex)
                {
                    model2 = GetViewportColumnLayoutModel(num12);
                }
                RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(rowViewportIndex);
                RowLayoutModel model4 = viewportRowLayoutModel;
                if (num14 != rowViewportIndex)
                {
                    model4 = GetViewportRowLayoutModel(num14);
                }
                if ((((viewportRowLayoutModel != null) && (viewportRowLayoutModel.Count > 0)) && ((model4 != null) && (model4.Count > 0))) && (((viewportColumnLayoutModel != null) && (viewportColumnLayoutModel.Count > 0)) && ((model2 != null) && (model2.Count > 0))))
                {
                    double d = -1.0;
                    double num18 = -1.0;
                    double num19 = -1.0;
                    double num20 = -1.0;
                    ColumnLayout layout3 = viewportColumnLayoutModel.Find(index);
                    ColumnLayout layout4 = model2.Find(num8);
                    if (layout3 != null)
                    {
                        d = layout3.X;
                    }
                    else
                    {
                        d = viewportColumnLayoutModel[0].X;
                    }
                    if (layout4 != null)
                    {
                        num19 = layout4.X + layout4.Width;
                    }
                    else
                    {
                        num19 = model2[model2.Count - 1].X + model2[model2.Count - 1].Width;
                    }
                    RowLayout layout5 = viewportRowLayoutModel.Find(num9);
                    RowLayout layout6 = model4.Find(num10);
                    if (layout5 != null)
                    {
                        num18 = layout5.Y;
                    }
                    else
                    {
                        num18 = viewportRowLayoutModel[0].Y;
                    }
                    if (layout6 != null)
                    {
                        num20 = layout6.Y + layout6.Height;
                    }
                    else
                    {
                        num20 = model4[model4.Count - 1].Y + model4[model4.Count - 1].Height;
                    }
                    SheetLayout sheetLayout = GetSheetLayout();
                    bool flag = ((index >= viewportColumnLayoutModel[0].Column) && (index <= viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column)) && ActiveSheet.GetActualColumnVisible(index, SheetArea.Cells);
                    bool flag2 = ((num8 >= model2[0].Column) && (num8 <= model2[model2.Count - 1].Column)) && ActiveSheet.GetActualColumnVisible(num8, SheetArea.Cells);
                    bool flag3 = ((num9 >= viewportRowLayoutModel[0].Row) && (num9 <= viewportRowLayoutModel[viewportRowLayoutModel.Count - 1].Row)) && ActiveSheet.GetActualRowVisible(num9, SheetArea.Cells);
                    bool flag4 = ((num10 >= model4[0].Row) && (num10 <= model4[model4.Count - 1].Row)) && ActiveSheet.GetActualRowVisible(num10, SheetArea.Cells);
                    double num21 = sheetLayout.GetViewportX(num12) + sheetLayout.GetViewportWidth(num12);
                    double num22 = sheetLayout.GetViewportY(num14) + sheetLayout.GetViewportHeight(num14);
                    if (flag2 && (num21 < num19))
                    {
                        flag2 = false;
                    }
                    if (flag4 && (num22 < num20))
                    {
                        flag4 = false;
                    }
                    double num23 = Math.Floor((double)((Math.Min(num21, num19) - d) + 3.0));
                    double num24 = Math.Floor((double)((Math.Min(num22, num20) - num18) + 3.0));
                    d -= 2.0;
                    num18 -= 2.0;
                    Canvas.SetLeft(_dragDropIndicator, Math.Floor(d));
                    Canvas.SetTop(_dragDropIndicator, Math.Floor(num18));
                    _dragDropIndicator.Visibility = Visibility.Visible;
                    _dragDropIndicator.Height = num24;
                    _dragDropIndicator.Width = num23;
                    double x = (index <= viewportColumnLayoutModel[0].Column) ? 2.0 : 0.0;
                    double y = (num9 <= viewportRowLayoutModel[0].Row) ? 2.0 : 0.0;
                    double width = 3.0;
                    Rect empty = Rect.Empty;
                    Rect rect2 = Rect.Empty;
                    Rect rect3 = Rect.Empty;
                    Rect rect4 = Rect.Empty;
                    if (flag)
                    {
                        empty = new Rect(x, y, width - x, num24 - y);
                    }
                    if (flag3)
                    {
                        rect2 = new Rect(x, y, num23 - x, width - y);
                    }
                    if (flag2)
                    {
                        rect3 = new Rect(num23 - width, y, width, num24 - y);
                    }
                    if (flag4)
                    {
                        rect4 = new Rect(x, num24 - width, num23 - x, width);
                    }
                    if (_dragDropIndicator.Children.Count >= 8)
                    {
                        if (flag)
                        {
                            RectangleGeometry geometry = new RectangleGeometry();
                            geometry.Rect = empty;
                            ((UIElement)_dragDropIndicator.Children[0]).Clip = geometry;
                            RectangleGeometry geometry2 = new RectangleGeometry();
                            geometry2.Rect = empty;
                            ((UIElement)_dragDropIndicator.Children[4]).Clip = geometry2;
                        }
                        if (flag3)
                        {
                            RectangleGeometry geometry3 = new RectangleGeometry();
                            geometry3.Rect = rect2;
                            ((UIElement)_dragDropIndicator.Children[1]).Clip = geometry3;
                            RectangleGeometry geometry4 = new RectangleGeometry();
                            geometry4.Rect = rect2;
                            ((UIElement)_dragDropIndicator.Children[5]).Clip = geometry4;
                        }
                        if (flag2)
                        {
                            RectangleGeometry geometry5 = new RectangleGeometry();
                            geometry5.Rect = rect3;
                            ((UIElement)_dragDropIndicator.Children[2]).Clip = geometry5;
                            RectangleGeometry geometry6 = new RectangleGeometry();
                            geometry6.Rect = rect3;
                            ((UIElement)_dragDropIndicator.Children[6]).Clip = geometry6;
                        }
                        if (flag4)
                        {
                            RectangleGeometry geometry7 = new RectangleGeometry();
                            geometry7.Rect = rect4;
                            ((UIElement)_dragDropIndicator.Children[3]).Clip = geometry7;
                            RectangleGeometry geometry8 = new RectangleGeometry();
                            geometry8.Rect = rect4;
                            ((UIElement)_dragDropIndicator.Children[7]).Clip = geometry8;
                        }
                    }
                    if (Excel.ShowDragDropTip)
                    {
                        TooltipHelper.ShowTooltip(GetRangeString(new CellRange(num5, num6, rowCount, columnCount)), num19 + 2.0, num20 + 5.0);
                    }
                }
            }
        }

        void RefreshDragDropInsertIndicator(int dragToRowViewportIndex, int dragToColumnViewportIndex, int dragToRow, int dragToColumn)
        {
            RowLayout layout = GetViewportRowLayoutModel(dragToRowViewportIndex).FindRow(dragToRow);
            ColumnLayout layout2 = GetViewportColumnLayoutModel(dragToColumnViewportIndex).FindColumn(dragToColumn);
            if ((layout != null) && (layout2 != null))
            {
                _dragDropIndicator.Visibility = Visibility.Collapsed;
                SheetLayout sheetLayout = GetSheetLayout();
                int row = _dragDropFromRange.Row;
                int column = _dragDropFromRange.Column;
                int rowCount = _dragDropFromRange.RowCount;
                int columnCount = _dragDropFromRange.ColumnCount;
                double width = 3.0;
                if ((row < 0) || (column < 0))
                {
                    if (column >= 0)
                    {
                        int num6 = (column < 0) ? 0 : column;
                        int num7 = (column < 0) ? ActiveSheet.ColumnCount : columnCount;
                        double d = layout2.X - (width / 2.0);
                        if (MousePosition.X > (layout2.X + (layout2.Width / 2.0)))
                        {
                            d = (layout2.X + layout2.Width) - (width / 2.0);
                            dragToColumn++;
                        }
                        if (d > (sheetLayout.GetViewportX(dragToColumnViewportIndex) + sheetLayout.GetViewportWidth(dragToColumnViewportIndex)))
                        {
                            _dragDropInsertIndicator.Visibility = Visibility.Collapsed;
                        }
                        else if (((_isDragCopy && (dragToColumn > num6)) && (dragToColumn < (num6 + num7))) || ((!_isDragCopy && (dragToColumn >= num6)) && (dragToColumn < (num6 + num7))))
                        {
                            _dragDropInsertIndicator.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            _dragDropInsertIndicator.Visibility = Visibility.Visible;
                            int rowViewportCount = GetViewportInfo().RowViewportCount;
                            double viewportY = 0.0;
                            double num11 = 0.0;
                            if ((ActiveSheet.FrozenRowCount > 0) && ((dragToRowViewportIndex == -1) || (dragToRowViewportIndex == 0)))
                            {
                                viewportY = sheetLayout.GetViewportY(-1);
                                if ((rowViewportCount == 1) && (ActiveSheet.FrozenTrailingRowCount > 0))
                                {
                                    num11 = sheetLayout.GetViewportY(1) + sheetLayout.GetViewportHeight(1);
                                }
                                else
                                {
                                    RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(0);
                                    if ((viewportRowLayoutModel != null) && (viewportRowLayoutModel.Count > 0))
                                    {
                                        RowLayout layout4 = viewportRowLayoutModel[viewportRowLayoutModel.Count - 1];
                                        num11 = Math.Min((double)(layout4.Y + layout4.Height), (double)(sheetLayout.GetViewportY(0) + sheetLayout.GetViewportHeight(0)));
                                    }
                                    else
                                    {
                                        num11 = sheetLayout.GetViewportY(-1) + sheetLayout.GetViewportHeight(-1);
                                    }
                                }
                            }
                            else if ((ActiveSheet.FrozenTrailingRowCount > 0) && ((dragToRowViewportIndex == rowViewportCount) || (dragToRowViewportIndex == (rowViewportCount - 1))))
                            {
                                if ((rowViewportCount == 1) && (ActiveSheet.FrozenRowCount > 0))
                                {
                                    viewportY = sheetLayout.GetViewportY(-1);
                                }
                                else
                                {
                                    viewportY = sheetLayout.GetViewportY(rowViewportCount - 1);
                                }
                                num11 = sheetLayout.GetViewportY(rowViewportCount) + sheetLayout.GetViewportHeight(rowViewportCount);
                            }
                            else
                            {
                                viewportY = sheetLayout.GetViewportY(dragToRowViewportIndex);
                                RowLayoutModel model2 = GetViewportRowLayoutModel(dragToRowViewportIndex);
                                if ((model2 != null) && (model2.Count > 0))
                                {
                                    RowLayout layout5 = model2[model2.Count - 1];
                                    num11 = Math.Min((double)(layout5.Y + layout5.Height), (double)(sheetLayout.GetViewportY(dragToRowViewportIndex) + sheetLayout.GetViewportHeight(dragToRowViewportIndex)));
                                }
                                else
                                {
                                    num11 = sheetLayout.GetViewportY(dragToRowViewportIndex) + sheetLayout.GetViewportHeight(dragToRowViewportIndex);
                                }
                            }
                            Canvas.SetLeft(_dragDropInsertIndicator, Math.Floor(d));
                            Canvas.SetTop(_dragDropInsertIndicator, Math.Floor(viewportY));
                            double num12 = width * 2.0;
                            double num13 = Math.Floor((double)(num11 - viewportY));
                            _dragDropInsertIndicator.Width = num12;
                            _dragDropInsertIndicator.Height = num13;
                            RectangleGeometry geometry = new RectangleGeometry();
                            geometry.Rect = new Rect(0.0, 0.0, width, num13);
                            _dragDropInsertIndicator.Clip = geometry;
                            if (Excel.ShowDragDropTip)
                            {
                                TooltipHelper.ShowTooltip(GetRangeString(new CellRange(-1, dragToColumn, -1, num7)), MousePosition.X + 10.0, _mouseDownPosition.Y + 10.0);
                            }
                        }
                    }
                    else if (row >= 0)
                    {
                        int num14 = (row < 0) ? 0 : row;
                        int num15 = (row < 0) ? ActiveSheet.RowCount : rowCount;
                        double num16 = layout.Y - (width / 2.0);
                        if (MousePosition.Y > (layout.Y + (layout.Height / 2.0)))
                        {
                            num16 = (layout.Y + layout.Height) - (width / 2.0);
                            dragToRow++;
                        }
                        if (num16 > (sheetLayout.GetViewportY(dragToRowViewportIndex) + sheetLayout.GetViewportHeight(dragToRowViewportIndex)))
                        {
                            _dragDropInsertIndicator.Visibility = Visibility.Collapsed;
                        }
                        else if (((_isDragCopy && (dragToRow > num14)) && (dragToRow < (num14 + num15))) || ((!_isDragCopy && (dragToRow >= num14)) && (dragToRow < (num14 + num15))))
                        {
                            _dragDropInsertIndicator.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            _dragDropInsertIndicator.Visibility = Visibility.Visible;
                            int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                            double viewportX = 0.0;
                            double num19 = 0.0;
                            if ((ActiveSheet.FrozenColumnCount > 0) && ((dragToColumnViewportIndex == -1) || (dragToColumnViewportIndex == 0)))
                            {
                                viewportX = sheetLayout.GetViewportX(-1);
                                if ((columnViewportCount == 1) && (ActiveSheet.FrozenTrailingColumnCount > 0))
                                {
                                    num19 = sheetLayout.GetViewportX(1) + sheetLayout.GetViewportWidth(1);
                                }
                                else
                                {
                                    ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(0);
                                    if ((viewportColumnLayoutModel != null) && (viewportColumnLayoutModel.Count > 0))
                                    {
                                        ColumnLayout layout6 = viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1];
                                        num19 = Math.Min((double)(layout6.X + layout6.Width), (double)(sheetLayout.GetViewportX(0) + sheetLayout.GetViewportWidth(0)));
                                    }
                                    else
                                    {
                                        num19 = sheetLayout.GetViewportX(-1) + sheetLayout.GetViewportWidth(-1);
                                    }
                                }
                            }
                            else if ((ActiveSheet.FrozenTrailingColumnCount > 0) && ((dragToColumnViewportIndex == columnViewportCount) || (dragToColumnViewportIndex == (columnViewportCount - 1))))
                            {
                                if ((columnViewportCount == 1) && (ActiveSheet.FrozenColumnCount > 0))
                                {
                                    viewportX = sheetLayout.GetViewportX(-1);
                                }
                                else
                                {
                                    viewportX = sheetLayout.GetViewportX(columnViewportCount - 1);
                                }
                                num19 = sheetLayout.GetViewportX(columnViewportCount) + sheetLayout.GetViewportWidth(columnViewportCount);
                            }
                            else
                            {
                                viewportX = sheetLayout.GetViewportX(dragToColumnViewportIndex);
                                ColumnLayoutModel model4 = GetViewportColumnLayoutModel(dragToColumnViewportIndex);
                                if ((model4 != null) && (model4.Count > 0))
                                {
                                    ColumnLayout layout7 = model4[model4.Count - 1];
                                    num19 = Math.Min((double)(layout7.X + layout7.Width), (double)(sheetLayout.GetViewportX(dragToColumnViewportIndex) + sheetLayout.GetViewportWidth(dragToColumnViewportIndex)));
                                }
                                else
                                {
                                    num19 = sheetLayout.GetViewportX(dragToColumnViewportIndex) + sheetLayout.GetViewportWidth(dragToColumnViewportIndex);
                                }
                            }
                            Canvas.SetLeft(_dragDropInsertIndicator, Math.Floor(viewportX));
                            Canvas.SetTop(_dragDropInsertIndicator, Math.Floor(num16));
                            double num20 = Math.Floor((double)(num19 - viewportX));
                            double num21 = width * 2.0;
                            _dragDropInsertIndicator.Width = num20;
                            _dragDropInsertIndicator.Height = num21;
                            RectangleGeometry geometry2 = new RectangleGeometry();
                            geometry2.Rect = new Rect(0.0, 0.0, num20, width);
                            _dragDropInsertIndicator.Clip = geometry2;
                            if (Excel.ShowDragDropTip)
                            {
                                TooltipHelper.ShowTooltip(GetRangeString(new CellRange(dragToRow, -1, num15, -1)), _mouseDownPosition.X + 10.0, MousePosition.Y + 10.0);
                            }
                        }
                    }
                }
            }
        }

        void RefreshDragFill()
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
                            if (NeedRefresh(viewport.RowViewportIndex, viewport.ColumnViewportIndex))
                            {
                                viewport.RefreshDragFill();
                            }
                            else
                            {
                                viewport.ResetDragFill();
                            }
                        }
                    }
                }
            }
        }

        internal void RefreshHeaderCells(HeaderPanel[] headers, int row, int column, int rowCount, int columnCount)
        {
            if (!IsSuspendInvalidate() && (headers != null))
            {
                foreach (var viewport in headers)
                {
                    if (viewport == null)
                        continue;

                    foreach (RowLayout layout in viewport.GetRowLayoutModel())
                    {
                        if ((row <= layout.Row) && (layout.Row < (row + rowCount)))
                        {
                            var presenter = viewport.GetRow(layout.Row);
                            if (presenter != null)
                            {
                                foreach (var base2 in presenter.Children.OfType<HeaderCellItem>())
                                {
                                    if ((column <= base2.Column) && (base2.Column < (column + columnCount)))
                                    {
                                        base2.UpdateChildren();
                                    }
                                }
                            }
                        }
                    }
                    viewport.InvalidateRowsMeasureState(true);
                }
            }
        }

        internal void RefreshViewportCells(CellsPanel[,] viewports, int row, int column, int rowCount, int columnCount)
        {
            if (viewports == null)
                return;

            CellsPanel[,] viewportArray = viewports;
            int upperBound = viewportArray.GetUpperBound(0);
            int num2 = viewportArray.GetUpperBound(1);
            for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
            {
                for (int j = viewportArray.GetLowerBound(1); j <= num2; j++)
                {
                    CellsPanel viewport = viewportArray[i, j];
                    if (viewport != null)
                    {
                        if (((IsEditing && (viewport.EditingContainer != null)) && viewport.IsActived) && ((viewport.EditingContainer.EditingRowIndex != ActiveSheet.ActiveRowIndex) || (ActiveSheet.ActiveColumnIndex != viewport.EditingContainer.EditingColumnIndex)))
                        {
                            StopCellEditing(true);
                        }

                        foreach (RowLayout layout in viewport.GetRowLayoutModel())
                        {
                            if ((row <= layout.Row) && (layout.Row < (row + rowCount)))
                            {
                                RowItem presenter = viewport.GetRow(layout.Row);
                                if (presenter != null)
                                {
                                    foreach (CellItem cell in presenter.Children)
                                    {
                                        if ((column <= cell.Column) && (cell.Column < (column + columnCount)))
                                        {
                                            cell.Refresh();
                                        }
                                    }
                                }
                            }
                        }

                        viewport.InvalidateBordersMeasureState();
                        viewport.InvalidateSelectionMeasureState();
                        viewport.InvalidateRowsMeasureState(true);
                    }
                }
            }
        }

        void RefreshViewportFloatingObjects()
        {
            if ((_cellsPanels != null) && (_cellsPanels != null))
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
                            viewport.RefreshFloatingObjects();
                        }
                    }
                }
            }
        }

        void RefreshViewportFloatingObjects(FloatingObject floatingObject)
        {
            if ((_cellsPanels != null) && (_cellsPanels != null))
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
                            if (floatingObject is SpreadChart)
                            {
                                viewport.RefreshFloatingObject(new ChartChangedEventArgs(floatingObject as SpreadChart, ChartArea.All, null));
                            }
                            else if (floatingObject is Picture)
                            {
                                viewport.RefreshFloatingObject(new PictureChangedEventArgs(floatingObject as Picture, null));
                            }
                            else if (floatingObject != null)
                            {
                                viewport.RefreshFloatingObject(new FloatingObjectChangedEventArgs(floatingObject, null));
                            }
                        }
                    }
                }
            }
        }

        void RefreshViewportFloatingObjectsContainerMoving()
        {
            if ((_cellsPanels != null) && (_cellsPanels != null))
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
                            if (IsNeedRefreshFloatingObjectsMovingResizingContainer(viewport.RowViewportIndex, viewport.ColumnViewportIndex))
                            {
                                viewport.RefreshFloatingObjectMovingFrames();
                            }
                            else
                            {
                                viewport.ResetFloatingObjectovingFrames();
                            }
                        }
                    }
                }
            }
        }

        void RefreshViewportFloatingObjectsContainerResizing()
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
                            if (IsNeedRefreshFloatingObjectsMovingResizingContainer(viewport.RowViewportIndex, viewport.ColumnViewportIndex))
                            {
                                viewport.RefreshFlaotingObjectResizingFrames();
                            }
                            else
                            {
                                viewport.ResetFloatingObjectResizingFrames();
                            }
                        }
                    }
                }
            }
        }

        void RefreshViewportFloatingObjectsLayout()
        {
            if ((_cellsPanels != null) && (_cellsPanels != null))
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
                            viewport.InvalidateFloatingObjectsMeasureState();
                        }
                    }
                }
            }
        }

        internal void Reset()
        {
            Init();
        }

        void ResetDragFill()
        {
            ResetMouseCursor();
            IsWorking = false;
            IsDraggingFill = false;
            ResetDragFillViewportInfo();
            StopScrollTimer();
            TooltipHelper.CloseTooltip();
        }

        void ResetDragFillViewportInfo()
        {
            _dragStartRowViewport = -2;
            _dragStartColumnViewport = -2;
            _dragToRowViewport = -2;
            _dragToColumnViewport = -2;
            _dragToRow = -2;
            _dragToColumn = -2;
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
                            viewport.ResetDragFill();
                        }
                    }
                }
            }
        }

        void ResetFlagasAfterDragDropping()
        {
            IsWorking = false;
            _dragDropIndicator.Clip = null;
            _dragDropIndicator.Visibility = Visibility.Collapsed;
            _dragDropInsertIndicator.Visibility = Visibility.Collapsed;
            _dragDropFromRange = null;
            _dragDropRowOffset = 0;
            _dragDropColumnOffset = 0;
            _isDragInsert = false;
            _isDragCopy = false;
            _dragStartRowViewport = -2;
            _dragStartColumnViewport = -2;
            _dragToRowViewport = -2;
            _dragToColumnViewport = -2;
            _dragToRow = -2;
            _dragToColumn = -2;
        }

        void ResetFloatingObjectsMovingResizing()
        {
            IsWorking = false;
            IsMovingFloatingOjects = false;
            IsResizingFloatingObjects = false;
            IsTouchingMovingFloatingObjects = false;
            IsTouchingResizingFloatingObjects = false;
            _movingResizingFloatingObjects = null;
            _dragStartRowViewport = -2;
            _dragStartColumnViewport = -2;
            _dragToRowViewport = -2;
            _dragToColumnViewport = -2;
            _dragToRow = -2;
            _dragToColumn = -2;
            _floatingObjectsMovingResizingStartRow = -2;
            _floatingObjectsMovingResizingStartColumn = -2;
            _floatingObjectsMovingResizingOffset = new Point(0.0, 0.0);
            _floatingObjectsMovingStartLocations = null;
            _floatingObjectsMovingResizingStartPointCellBounds = new Rect(0.0, 0.0, 0.0, 0.0);
            _cachedFloatingObjectMovingResizingLayoutModel = null;
            ResetViewportFloatingObjectsContainerMoving();
            ResetViewportFloatingObjectsContainerReSizing();
        }

        void ResetTouchDragFill()
        {
            ResetMouseCursor();
            IsWorking = false;
            ResetDragFillViewportInfo();
            StopScrollTimer();
            TooltipHelper.CloseTooltip();
        }

        void ResetViewportFloatingObjectsContainerMoving()
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
                            viewport.ResetFloatingObjectovingFrames();
                        }
                    }
                }
            }
        }

        void ResetViewportFloatingObjectsContainerReSizing()
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
                            viewport.ResetFloatingObjectResizingFrames();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resumes the events.
        /// </summary>
        public void ResumeEvent()
        {
            _eventSuspended--;
            if (_eventSuspended < 0)
            {
                _eventSuspended = 0;
            }
        }

        internal void ResumeFloatingObjectsInvalidate()
        {
            if ((_cellsPanels != null) && (_cellsPanels != null))
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
                            viewport.ResumeFloatingObjectsInvalidate(false);
                        }
                    }
                }
            }
        }

        double RoundToPoint(double value)
        {
            return Math.Floor(value);
        }

        /// <summary>
        /// Sets the active cell of the sheet.
        /// </summary>
        /// <param name="row">The active row index.</param>
        /// <param name="column">The active column index.</param>
        /// <param name="clearSelection"> if set to <c>true</c> clears the old selection.</param>
        public void SetActiveCell(int row, int column, bool clearSelection)
        {
            if (ActiveSheet.GetActualStyleInfo(row, column, SheetArea.Cells).Focusable)
            {
                SetActiveCellInternal(row, column, clearSelection);
            }
        }

        internal void SetActiveCellInternal(int row, int column, bool clearSelection)
        {
            if ((row != ActiveSheet.ActiveRowIndex) || (column != ActiveSheet.ActiveColumnIndex))
            {
                ActiveSheet.SetActiveCell(row, column, clearSelection);
                RaiseEnterCell(row, column);
            }
        }

        internal void SetActiveColumnViewportIndex(int value)
        {
            ViewportInfo viewportInfo = ActiveSheet.GetViewportInfo();
            if (viewportInfo.ActiveColumnViewport != value)
            {
                viewportInfo.ActiveColumnViewport = value;
                ActiveSheet.SetViewportInfo(viewportInfo);
                UpdateFocusIndicator();
                UpdateDataValidationUI(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
            }
        }

        void SetActiveportIndexAfterDragDrop()
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            if ((_dragToRowViewport != -2) && (_dragToColumnViewport != -2))
            {
                int num = _dragToRowViewport;
                int num2 = _dragToColumnViewport;
                int activeRowIndex = ActiveSheet.ActiveRowIndex;
                int activeColumnIndex = ActiveSheet.ActiveColumnIndex;
                if ((num == 0) && (activeRowIndex < ActiveSheet.FrozenRowCount))
                {
                    num = -1;
                }
                else if ((num == viewportInfo.RowViewportCount) && (activeRowIndex < (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
                {
                    num = viewportInfo.RowViewportCount - 1;
                }
                if ((num2 == 0) && (activeColumnIndex < ActiveSheet.FrozenColumnCount))
                {
                    num2 = -1;
                }
                else if ((num2 == viewportInfo.ColumnViewportCount) && (activeColumnIndex < (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
                {
                    num2 = viewportInfo.ColumnViewportCount - 1;
                }
                if (num != GetActiveRowViewportIndex())
                {
                    SetActiveRowViewportIndex(num);
                }
                if (num2 != GetActiveColumnViewportIndex())
                {
                    SetActiveColumnViewportIndex(num2);
                }
            }
        }

        internal void SetActiveRowViewportIndex(int value)
        {
            ViewportInfo viewportInfo = ActiveSheet.GetViewportInfo();
            if (viewportInfo.ActiveRowViewport != value)
            {
                viewportInfo.ActiveRowViewport = value;
                ActiveSheet.SetViewportInfo(viewportInfo);
                UpdateFocusIndicator();
                UpdateDataValidationUI(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
            }
        }

        /// <summary>
        /// Sets the index of the floating object Z.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="zIndex">Index of the z.</param>
        public void SetFloatingObjectZIndex(string name, int zIndex)
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
                            viewport.SetFlotingObjectZIndex(name, zIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the column viewport's left column.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <param name="value">The column index.</param>
        public void SetViewportLeftColumn(int columnViewportIndex, int value)
        {
            if ((ActiveSheet != null) && (_hScrollable || _isTouchScrolling))
            {
                value = Math.Max(ActiveSheet.FrozenColumnCount, value);
                value = Math.Min((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1, value);
                value = TryGetNextScrollableColumn(value);

                ViewportInfo viewportInfo = GetViewportInfo();
                value = Math.Max(ActiveSheet.FrozenColumnCount, value);
                value = Math.Min((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1, value);
                value = TryGetNextScrollableColumn(value);
                if (((columnViewportIndex >= 0) && (columnViewportIndex < viewportInfo.ColumnViewportCount)) && (viewportInfo.LeftColumns[columnViewportIndex] != value))
                {
                    int oldIndex = viewportInfo.LeftColumns[columnViewportIndex];
                    viewportInfo.LeftColumns[columnViewportIndex] = value;
                    InvalidateViewportColumnsLayout();
                    InvalidateViewportHorizontalArrangement(columnViewportIndex);
                    if (_columnGroupPresenters != null)
                    {
                        GcRangeGroup group = _columnGroupPresenters[columnViewportIndex + 1];
                        if (group != null)
                        {
                            group.InvalidateMeasure();
                        }
                    }
                    RaiseLeftChanged(oldIndex, value, columnViewportIndex);
                }
                if (!IsWorking)
                {
                    SaveHitInfo(null);
                }

                if (_horizontalScrollBar != null)
                {
                    GetSheetLayout();
                    if (((columnViewportIndex > -1) && (columnViewportIndex < _horizontalScrollBar.Length)) && (_horizontalScrollBar[columnViewportIndex].Value != value))
                    {
                        int invisibleColumnsBeforeColumn = GetInvisibleColumnsBeforeColumn(ActiveSheet, value);
                        int num2 = value - invisibleColumnsBeforeColumn;
                        _horizontalScrollBar[columnViewportIndex].Value = (double)num2;
                        _horizontalScrollBar[columnViewportIndex].InvalidateArrange();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the row viewport's top row.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <param name="value">The row index.</param>
        public void SetViewportTopRow(int rowViewportIndex, int value)
        {
            if ((ActiveSheet != null) && (_vScrollable || _isTouchScrolling))
            {
                value = Math.Max(ActiveSheet.FrozenRowCount, value);
                value = Math.Min((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1, value);
                value = TryGetNextScrollableRow(value);
                if (_verticalScrollBar != null)
                {
                    GetSheetLayout();
                    if (((rowViewportIndex > -1) && (rowViewportIndex < _verticalScrollBar.Length)) && (value != _verticalScrollBar[rowViewportIndex].Value))
                    {
                        int invisibleRowsBeforeRow = GetInvisibleRowsBeforeRow(ActiveSheet, value);
                        int num2 = value - invisibleRowsBeforeRow;
                        _verticalScrollBar[rowViewportIndex].Value = (double)num2;
                        _verticalScrollBar[rowViewportIndex].InvalidateArrange();
                    }
                }

                ViewportInfo viewportInfo = GetViewportInfo();
                value = Math.Max(ActiveSheet.FrozenRowCount, value);
                value = Math.Min((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1, value);
                value = TryGetNextScrollableRow(value);
                if (((rowViewportIndex >= 0) && (rowViewportIndex < viewportInfo.RowViewportCount)) && (viewportInfo.TopRows[rowViewportIndex] != value))
                {
                    int oldIndex = viewportInfo.TopRows[rowViewportIndex];
                    viewportInfo.TopRows[rowViewportIndex] = value;
                    InvalidateViewportRowsLayout();
                    InvalidateViewportRowsPresenterMeasure(rowViewportIndex, false);
                    for (int i = -1; i < viewportInfo.ColumnViewportCount; i++)
                    {
                        CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(rowViewportIndex, i);
                        if (viewportRowsPresenter != null)
                        {
                            if ((viewportRowsPresenter.RowViewportIndex == GetActiveRowViewportIndex()) && (viewportRowsPresenter.ColumnViewportIndex == GetActiveColumnViewportIndex()))
                            {
                                viewportRowsPresenter.UpdateDataValidationUI(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex);
                            }
                            viewportRowsPresenter.InvalidateMeasure();
                            viewportRowsPresenter.InvalidateBordersMeasureState();
                            viewportRowsPresenter.InvalidateSelectionMeasureState();
                            viewportRowsPresenter.InvalidateFloatingObjectsMeasureState();
                        }
                    }
                    var rowHeaderRowsPresenter = GetRowHeaderRowsPresenter(rowViewportIndex);
                    if (rowHeaderRowsPresenter != null)
                    {
                        rowHeaderRowsPresenter.InvalidateMeasure();
                    }
                    if (_rowGroupPresenters != null)
                    {
                        GcRangeGroup group = _rowGroupPresenters[rowViewportIndex + 1];
                        if (group != null)
                        {
                            group.InvalidateMeasure();
                        }
                    }
                    RaiseTopChanged(oldIndex, value, rowViewportIndex);
                }
                if (!IsWorking)
                {
                    SaveHitInfo(null);
                }
            }
        }

        /// <summary>
        /// Displays the automatic fill indicator.
        /// </summary>
        public void ShowAutoFillIndicator()
        {
            if (CanUserDragFill)
            {
                CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
                if (viewportRowsPresenter != null)
                {
                    CellRange activeSelection = GetActiveSelection();
                    if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
                    {
                        activeSelection = ActiveSheet.Selections[0];
                    }
                    if (activeSelection != null)
                    {
                        _autoFillIndicatorContainer.Width = 16.0;
                        _autoFillIndicatorContainer.Height = 16.0;
                        _autoFillIndicatorRec = new Rect?(GetAutoFillIndicatorRect(viewportRowsPresenter, activeSelection));
                        base.InvalidateArrange();
                        CachedGripperLocation = null;
                    }
                }
            }
        }

        internal void ShowCell(int rowViewportIndex, int columnViewportIndex, int row, int column, VerticalPosition verticalPosition, HorizontalPosition horizontalPosition)
        {
            Worksheet worksheet = ActiveSheet;
            if (((worksheet != null) && (row <= worksheet.RowCount)) && (column <= worksheet.ColumnCount))
            {
                int viewportTopRow = GetViewportTopRow(rowViewportIndex);
                int viewportLeftColumn = GetViewportLeftColumn(columnViewportIndex);
                switch (horizontalPosition)
                {
                    case HorizontalPosition.Center:
                        {
                            double num3 = RoundToPoint((GetViewportWidth(columnViewportIndex) - RoundToPoint(worksheet.Columns[column].ActualWidth * ZoomFactor)) / 2.0);
                            while (0 < column)
                            {
                                num3 -= RoundToPoint(worksheet.Columns[column - 1].ActualWidth * ZoomFactor);
                                if (num3 < 0.0)
                                {
                                    break;
                                }
                                column--;
                            }
                            break;
                        }
                    case HorizontalPosition.Right:
                        {
                            double num4 = GetViewportWidth(columnViewportIndex) - RoundToPoint(worksheet.Columns[column].ActualWidth * ZoomFactor);
                            while (0 < column)
                            {
                                num4 -= RoundToPoint(worksheet.Columns[column - 1].ActualWidth * ZoomFactor);
                                if (num4 < 0.0)
                                {
                                    break;
                                }
                                column--;
                            }
                            break;
                        }
                    case HorizontalPosition.Nearest:
                        if (column >= viewportLeftColumn)
                        {
                            double num5 = GetViewportWidth(columnViewportIndex) - RoundToPoint(worksheet.Columns[column].Width * ZoomFactor);
                            while (viewportLeftColumn < column)
                            {
                                num5 -= RoundToPoint(worksheet.Columns[column - 1].ActualWidth * ZoomFactor);
                                if (num5 < 0.0)
                                {
                                    break;
                                }
                                column--;
                            }
                        }
                        break;
                }
                switch (verticalPosition)
                {
                    case VerticalPosition.Center:
                        {
                            double num6 = RoundToPoint((GetViewportHeight(rowViewportIndex) - RoundToPoint(worksheet.Rows[row].ActualHeight * ZoomFactor)) / 2.0);
                            while (0 < row)
                            {
                                num6 -= RoundToPoint(worksheet.Rows[row - 1].ActualHeight * ZoomFactor);
                                if (num6 < 0.0)
                                {
                                    break;
                                }
                                row--;
                            }
                            break;
                        }
                    case VerticalPosition.Bottom:
                        {
                            double num7 = GetViewportHeight(rowViewportIndex) - RoundToPoint(worksheet.Rows[row].ActualHeight * ZoomFactor);
                            while (0 < row)
                            {
                                num7 -= RoundToPoint(worksheet.Rows[row - 1].ActualHeight * ZoomFactor);
                                if (num7 < 0.0)
                                {
                                    break;
                                }
                                row--;
                            }
                            break;
                        }
                    case VerticalPosition.Nearest:
                        if ((row >= viewportTopRow) && (viewportTopRow != -1))
                        {
                            double num8 = GetViewportHeight(rowViewportIndex) - RoundToPoint(worksheet.Rows[row].ActualHeight * ZoomFactor);
                            while (viewportTopRow < row)
                            {
                                num8 -= RoundToPoint(worksheet.Rows[row - 1].ActualHeight * ZoomFactor);
                                if (num8 < 0.0)
                                {
                                    break;
                                }
                                row--;
                            }
                        }
                        break;
                }
                if (row != viewportTopRow)
                {
                    SetViewportTopRow(rowViewportIndex, row);
                }
                if (column != viewportLeftColumn)
                {
                    SetViewportLeftColumn(columnViewportIndex, column);
                }
            }
        }

        internal void ShowColumn(int columnViewportIndex, int column, HorizontalPosition horizontalPosition)
        {
            int viewportTopRow = GetViewportTopRow(0);
            ShowCell(0, columnViewportIndex, viewportTopRow, column, VerticalPosition.Top, horizontalPosition);
        }

        void ShowDragFillSmartTag(CellRange fillRange, AutoFillType initFillType)
        {
            double x = 0.0;
            double y = 0.0;
            if (!IsDragFillWholeColumns && !IsDragFillWholeRows)
            {
                int index = (fillRange.Row + fillRange.RowCount) - 1;
                int num4 = (fillRange.Column + fillRange.ColumnCount) - 1;
                if (IsVerticalDragFill)
                {
                    ColumnLayout layout = GetViewportColumnLayoutModel(_dragFillStartRightColumnViewport).Find(num4);
                    if (layout == null)
                    {
                        int viewportRightColumn = GetViewportRightColumn(_dragFillStartLeftColumnViewport);
                        layout = GetViewportColumnLayoutModel(_dragFillStartLeftColumnViewport).FindColumn(viewportRightColumn);
                    }
                    x = layout.X + layout.Width;
                    RowLayout validVerDragToRowLayout = GetValidVerDragToRowLayout();
                    y = validVerDragToRowLayout.Y + validVerDragToRowLayout.Height;
                }
                else
                {
                    RowLayout layout3 = GetViewportRowLayoutModel(_dragFillStartBottomRowViewport).Find(index);
                    if (layout3 == null)
                    {
                        int viewportBottomRow = GetViewportBottomRow(_dragFillStartTopRowViewport);
                        layout3 = GetViewportRowLayoutModel(_dragFillStartTopRowViewport).FindRow(viewportBottomRow);
                    }
                    y = layout3.Y + layout3.Height;
                    ColumnLayout validHorDragToColumnLayout = GetValidHorDragToColumnLayout();
                    x = validHorDragToColumnLayout.X + validHorDragToColumnLayout.Width;
                }
            }
            else if (IsDragFillWholeColumns && !IsDragFillWholeRows)
            {
                int column = fillRange.Column;
                int columnCount = fillRange.ColumnCount;
                ColumnLayout layout5 = GetValidHorDragToColumnLayout();
                x = layout5.X + layout5.Width;
                y = DragFillStartViewportTopRowLayout.Y;
            }
            else if (IsDragFillWholeRows && !IsDragFillWholeColumns)
            {
                int row = fillRange.Row;
                int rowCount = fillRange.RowCount;
                RowLayout layout6 = GetValidVerDragToRowLayout();
                y = layout6.Y + layout6.Height;
                x = DragFillStartViewportLeftColumnLayout.X;
                y = layout6.Y + layout6.Height;
            }
            if ((x != 0.0) && (y != 0.0))
            {
                x -= 4.0;
                y++;
                Windows.UI.Xaml.Controls.Primitives.Popup popup = new Windows.UI.Xaml.Controls.Primitives.Popup();
                _dragFillPopup = new PopupHelper(popup);
                base.Children.Add(popup);
                popup.Closed += DragFillSmartTagPopup_Closed;
                _dragFillSmartTag = new DragFillSmartTag(this);
                _dragFillSmartTag.AutoFilterType = initFillType;
                _dragFillSmartTag.AutoFilterTypeChanged += new EventHandler(DragFillSmartTag_AutoFilterTypeChanged);
                if (InputDeviceType == InputDeviceType.Touch)
                {
                    x += 4.0;
                    y += 4.0;
                }
                _dragFillPopup.ShowAsModal(this, _dragFillSmartTag, new Point(x, y), PopupDirection.BottomRight, false, false);
            }
        }

        internal void ShowFormulaSelectionTouchGrippers()
        {
            if (_formulaSelectionGripperPanel != null)
            {
                _formulaSelectionGripperPanel.Visibility = Visibility.Visible;
            }
        }

        internal void ShowRow(int rowViewportIndex, int row, VerticalPosition verticalPosition)
        {
            int viewportLeftColumn = GetViewportLeftColumn(0);
            ShowCell(rowViewportIndex, 0, row, viewportLeftColumn, verticalPosition, HorizontalPosition.Left);
        }

        IEnumerable<FloatingObject> SortFloatingObjectByZIndex(FloatingObject[] floatingObjects)
        {
            Dictionary<int, List<FloatingObject>> dictionary = new Dictionary<int, List<FloatingObject>>();
            foreach (FloatingObject obj2 in floatingObjects)
            {
                int floatingObjectZIndex = GetFloatingObjectZIndex(obj2.Name);
                List<FloatingObject> list = null;
                dictionary.TryGetValue(floatingObjectZIndex, out list);
                if (list == null)
                {
                    list = new List<FloatingObject> {
                        obj2
                    };
                    dictionary.Add(floatingObjectZIndex, list);
                }
                else
                {
                    list.Add(obj2);
                }
            }
            IOrderedEnumerable<KeyValuePair<int, List<FloatingObject>>> enumerable = Enumerable.OrderBy<KeyValuePair<int, List<FloatingObject>>, int>((IEnumerable<KeyValuePair<int, List<FloatingObject>>>)dictionary, delegate (KeyValuePair<int, List<FloatingObject>> p)
            {
                return p.Key;
            });
            List<FloatingObject> list2 = new List<FloatingObject>();
            foreach (KeyValuePair<int, List<FloatingObject>> pair in enumerable)
            {
                list2.AddRange(pair.Value);
            }
            list2.Reverse();
            return (IEnumerable<FloatingObject>)list2;
        }

        /// <summary>
        /// Starts to edit the active cell.
        /// </summary>
        /// <param name="selectAll">if set to <c>true</c> selects all the text when the text is changed during editing.</param>
        /// <param name="defaultText">if set to <c>true</c> [default text].</param>
        public void StartCellEditing(bool selectAll = false, string defaultText = null)
        {
            StartCellEditing(selectAll, defaultText, EditorStatus.Edit);
        }

        /// <summary>
        /// Starts to edit the active cell.
        /// </summary>
        /// <param name="selectAll">if set to <c>true</c> will select all the text when text changed during editing.</param>
        /// <param name="defaultText">The default text of editor.</param>
        /// <param name="status">The status of the editor</param>
        internal void StartCellEditing(bool selectAll = false, string defaultText = null, EditorStatus status = EditorStatus.Edit)
        {
            StartTextInputInternal(selectAll, defaultText, status, false);
        }

        void StartColumnResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout sheetLayout = GetSheetLayout();
            ColumnLayout viewportResizingColumnLayoutFromX = null;
            IsWorking = true;
            IsResizingColumns = true;
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            if (_resizingTracker == null)
            {
                Line line = new Line();
                line.Stroke = brush;
                line.StrokeThickness = 1.0;
                line.StrokeDashArray = new DoubleCollection { 1.0 };
                _resizingTracker = line;
                TrackersContainer.Children.Add(_resizingTracker);
            }
            _resizingTracker.Visibility = Visibility.Visible;
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
                _resizingTracker.X1 = (viewportResizingColumnLayoutFromX.X + viewportResizingColumnLayoutFromX.Width) - 0.5;
                _resizingTracker.Y1 = sheetLayout.HeaderY;
                _resizingTracker.X2 = _resizingTracker.X1;
                _resizingTracker.Y2 = _resizingTracker.Y1 + _availableSize.Height;
                if (((InputDeviceType != InputDeviceType.Touch) && ((Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Column))) && ((savedHitTestInformation.ColumnViewportIndex > -2) && (_colHeaders[savedHitTestInformation.ColumnViewportIndex + 1].GetViewportCell(savedHitTestInformation.HeaderInfo.Row, savedHitTestInformation.HeaderInfo.Column, true) != null)))
                {
                    UpdateResizeToolTip(GetHorizontalResizeTip(viewportResizingColumnLayoutFromX.Width), true);
                }
            }
        }

        void StartDragDropping()
        {
            if (!IsDragDropping)
            {
                CellRange fromRange = GetFromRange();
                if (fromRange != null)
                {
                    IsDragDropping = true;
                    IsWorking = true;
                    UpdateDragIndicatorAndStartTimer(fromRange);
                }
            }
        }

        void StartDragFill()
        {
            if (!IsDraggingFill)
            {
                UpdateDragFillStartRange();
                if (_dragFillStartRange != null)
                {
                    IsDraggingFill = true;
                    IsWorking = true;
                    UpdateDragFillViewportInfoAndStartTimer();
                }
            }
        }

        void StartFloatingObjectsMoving()
        {
            _movingResizingFloatingObjects = GetAllSelectedFloatingObjects();
            if (((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Length != 0)) && InitFloatingObjectsMovingResizing())
            {
                if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
                {
                    _touchToolbarPopup.IsOpen = false;
                }
                IsWorking = true;
                if (IsTouching)
                {
                    IsTouchingMovingFloatingObjects = true;
                }
                else
                {
                    IsMovingFloatingOjects = true;
                }
                StartScrollTimer();
            }
        }

        void StartFloatingObjectsResizing()
        {
            _movingResizingFloatingObjects = GetAllSelectedFloatingObjects();
            if (((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Length != 0)) && InitFloatingObjectsMovingResizing())
            {
                if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
                {
                    _touchToolbarPopup.IsOpen = false;
                }
                IsWorking = true;
                if (IsTouching)
                {
                    IsTouchingResizingFloatingObjects = true;
                }
                else
                {
                    IsResizingFloatingObjects = true;
                }
                StartScrollTimer();
            }
        }

        void StartRowResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout sheetLayout = GetSheetLayout();
            RowLayout viewportResizingRowLayoutFromY = null;
            IsResizingRows = true;
            IsWorking = true;
            if (_resizingTracker == null)
            {
                _resizingTracker = new Line();
                _resizingTracker.Stroke = new SolidColorBrush(Colors.Black);
                _resizingTracker.StrokeThickness = 1.0;
                _resizingTracker.StrokeDashArray = new DoubleCollection { 1.0 };
                TrackersContainer.Children.Add(_resizingTracker);
            }
            _resizingTracker.Visibility = Visibility.Visible;
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
                    if ((viewportResizingRowLayoutFromY == null) && ((savedHitTestInformation.RowViewportIndex == 0) || (savedHitTestInformation.RowViewportIndex == -1)))
                    {
                        viewportResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(savedHitTestInformation.HitPoint.Y);
                    }
                    break;
            }
            if (viewportResizingRowLayoutFromY != null)
            {
                _resizingTracker.X1 = sheetLayout.HeaderX;
                _resizingTracker.X2 = sheetLayout.HeaderX + _availableSize.Width;
                _resizingTracker.Y1 = (viewportResizingRowLayoutFromY.Y + viewportResizingRowLayoutFromY.Height) - 0.5;
                _resizingTracker.Y2 = _resizingTracker.Y1;
                if (((InputDeviceType != InputDeviceType.Touch) && ((Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Row)))
                    && ((savedHitTestInformation.RowViewportIndex > -2) && (_rowHeaders[savedHitTestInformation.RowViewportIndex + 1].GetViewportCell(savedHitTestInformation.HeaderInfo.Row, savedHitTestInformation.HeaderInfo.Column, true) != null)))
                {
                    UpdateResizeToolTip(GetVerticalResizeTip(viewportResizingRowLayoutFromY.Height), false);
                }
            }
        }

        internal void StartTextInput(EditorStatus status = EditorStatus.Edit)
        {
            StartTextInputInternal(false, null, status, true);
        }

        void StartTextInputInternal(bool selectAll = false, string defaultText = null, EditorStatus status = (EditorStatus)2, bool fromTextInputService = false)
        {
            if ((!IsEditing || StopCellEditing(false)) && (ActiveSheet != null))
            {
                EditingViewport = null;
                if (!IsEditing && IsCellEditable(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex))
                {
                    CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
                    if (viewportRowsPresenter != null)
                    {
                        CoreWindow.GetForCurrentThread().ReleasePointerCapture();
                        EditingViewport = viewportRowsPresenter;
                        bool flag = false;
                        if (fromTextInputService)
                        {
                            flag = viewportRowsPresenter.StartTextInput(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, status);
                        }
                        else
                        {
                            flag = viewportRowsPresenter.StartCellEditing(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, selectAll, defaultText, status);
                        }
                        IsEditing = flag;
                        Excel.IsTabStop = !flag;
                        if (!flag)
                        {
                            EditingViewport = null;
                        }
                    }
                }
            }
        }

        void StartTouchColumnResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout sheetLayout = GetSheetLayout();
            ColumnLayout viewportResizingColumnLayoutFromXForTouch = null;
            IsWorking = true;
            IsTouchResizingColumns = true;
            _DoTouchResizing = false;
            CloseTouchToolbar();
            if (_resizingTracker == null)
            {
                SolidColorBrush brush = new SolidColorBrush(Colors.Black);
                Line line = new Line();
                line.Stroke = brush;
                line.StrokeThickness = 1.0;
                line.StrokeDashArray = new DoubleCollection { 1.0 };
                _resizingTracker = line;
                TrackersContainer.Children.Add(_resizingTracker);
            }
            _resizingTracker.Visibility = Visibility.Visible;
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
                _resizingTracker.X1 = (viewportResizingColumnLayoutFromXForTouch.X + viewportResizingColumnLayoutFromXForTouch.Width) - 0.5;
                _resizingTracker.Y1 = sheetLayout.HeaderY;
                _resizingTracker.X2 = _resizingTracker.X1;
                _resizingTracker.Y2 = _resizingTracker.Y1 + _availableSize.Height;
                if (((InputDeviceType != InputDeviceType.Touch) && ((Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Column))) && ((savedHitTestInformation.ColumnViewportIndex > -2) && (_colHeaders[savedHitTestInformation.ColumnViewportIndex + 1].GetViewportCell(savedHitTestInformation.HeaderInfo.Row, savedHitTestInformation.HeaderInfo.Column, true) != null)))
                {
                    UpdateResizeToolTip(GetHorizontalResizeTip(viewportResizingColumnLayoutFromXForTouch.Width), true);
                }
            }
        }

        void StartTouchDragFill()
        {
            if (!IsTouchDragFilling)
            {
                UpdateDragFillStartRange();
                if (_dragFillStartRange != null)
                {
                    IsTouchDragFilling = true;
                    IsWorking = true;
                    UpdateDragFillViewportInfoAndStartTimer();
                }
            }
        }

        void StartTouchRowResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout sheetLayout = GetSheetLayout();
            RowLayout viewportResizingRowLayoutFromYForTouch = null;
            _DoTouchResizing = false;
            IsTouchResizingRows = true;
            IsWorking = true;
            CloseTouchToolbar();
            if (_resizingTracker == null)
            {
                _resizingTracker = new Line();
                _resizingTracker.Stroke = new SolidColorBrush(Colors.Black);
                _resizingTracker.StrokeThickness = 1.0;
                _resizingTracker.StrokeDashArray = new DoubleCollection { 1.0 };
                TrackersContainer.Children.Add(_resizingTracker);
            }
            _resizingTracker.Visibility = Visibility.Visible;
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
                    if ((viewportResizingRowLayoutFromYForTouch == null) && ((savedHitTestInformation.RowViewportIndex == 0) || (savedHitTestInformation.RowViewportIndex == -1)))
                    {
                        viewportResizingRowLayoutFromYForTouch = GetColumnHeaderResizingRowLayoutFromYForTouch(savedHitTestInformation.HitPoint.Y);
                    }
                    break;
            }
            if (viewportResizingRowLayoutFromYForTouch != null)
            {
                _resizingTracker.X1 = sheetLayout.HeaderX;
                _resizingTracker.X2 = sheetLayout.HeaderX + _availableSize.Width;
                _resizingTracker.Y1 = (viewportResizingRowLayoutFromYForTouch.Y + viewportResizingRowLayoutFromYForTouch.Height) - 0.5;
                _resizingTracker.Y2 = _resizingTracker.Y1;
                if (((InputDeviceType != InputDeviceType.Touch) && ((Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both) || (Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Row))) && ((savedHitTestInformation.RowViewportIndex > -2) && (_rowHeaders[savedHitTestInformation.RowViewportIndex + 1].GetViewportCell(savedHitTestInformation.HeaderInfo.Row, savedHitTestInformation.HeaderInfo.Column, true) != null)))
                {
                    UpdateResizeToolTip(GetVerticalResizeTip(viewportResizingRowLayoutFromYForTouch.Height), false);
                }
            }
        }

        /// <summary>
        /// Stops editing the active cell.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> does not apply the edited text to the cell.</param>
        /// <returns><c>true</c> when able to stop cell editing successfully; otherwise, <c>false</c>.</returns>
        public bool StopCellEditing(bool cancel = false)
        {
            if (IsEditing && (ActiveSheet != null))
            {
                CellsPanel editingViewport = EditingViewport;
                if (editingViewport != null)
                {
                    if (!cancel && (ApplyEditingValue(cancel) == DataValidationResult.Retry))
                    {
                        editingViewport.RetryEditing();
                    }
                    else
                    {
                        bool editorDirty = editingViewport.EditorDirty;
                        editingViewport.StopCellEditing(cancel);
                        if (editorDirty && !cancel)
                        {
                            RefreshViewportCells(_cellsPanels, 0, 0, ActiveSheet.RowCount, ActiveSheet.ColumnCount);
                        }
                    }
                    if (editingViewport.IsEditing())
                    {
                        return false;
                    }
                    EditingViewport = null;
                }
            }
            IsEditing = false;
            return true;
        }

        /// <summary>
        /// Suspends all events.
        /// </summary>
        public void SuspendEvent()
        {
            _eventSuspended++;
        }

        internal void SuspendFloatingObjectsInvalidate()
        {
            if ((_cellsPanels != null) && (_cellsPanels != null))
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
                            viewport.SuspendFloatingObjectsInvalidate();
                        }
                    }
                }
            }
        }

        void SwitchDragDropIndicator()
        {
            bool flag;
            bool flag2;
            KeyboardHelper.GetMetaKeyState(out flag, out flag2);
            if (((_dragToRowViewport != -2) && (_dragToColumnViewport != -2)) && ((_dragToRow != -2) && (_dragToColumn != -2)))
            {
                bool flag3 = _isDragInsert;
                if (flag)
                {
                    if (!flag3 && ((_dragDropFromRange.Row == -1) || (_dragDropFromRange.Column == -1)))
                    {
                        RefreshDragDropInsertIndicator(_dragToRowViewport, _dragToColumnViewport, _dragToRow, _dragToColumn);
                    }
                }
                else if (flag3)
                {
                    RefreshDragDropIndicator(_dragToRowViewport, _dragToColumnViewport, _dragToRow, _dragToColumn);
                }
            }
            _isDragInsert = flag;
            _isDragCopy = flag2;
        }

        void SynViewportChartShapeThemes()
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
                            viewport.SynChartShapeThemes();
                        }
                    }
                }
            }
        }



        internal int TryGetNextScrollableColumn(int startColumn)
        {
            int frozenColumnCount = ActiveSheet.FrozenColumnCount;
            int num2 = (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1;
            if (startColumn < frozenColumnCount)
            {
                return frozenColumnCount;
            }
            if (startColumn > num2)
            {
                return num2;
            }
            for (int i = startColumn; i <= num2; i++)
            {
                if (ActiveSheet.GetActualColumnWidth(i, SheetArea.Cells) > 0.0)
                {
                    return i;
                }
            }
            return -1;
        }

        internal int TryGetNextScrollableRow(int startRow)
        {
            int frozenRowCount = ActiveSheet.FrozenRowCount;
            int num2 = (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1;
            if (startRow < frozenRowCount)
            {
                return frozenRowCount;
            }
            if (startRow > num2)
            {
                return num2;
            }
            for (int i = startRow; i <= num2; i++)
            {
                if (ActiveSheet.GetActualRowHeight(i, SheetArea.Cells) > 0.0)
                {
                    return i;
                }
            }
            return -1;
        }

        internal int TryGetPreviousScrollableColumn(int startColumn)
        {
            int frozenColumnCount = ActiveSheet.FrozenColumnCount;
            int num2 = (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1;
            if (startColumn < frozenColumnCount)
            {
                return frozenColumnCount;
            }
            if (startColumn > num2)
            {
                return num2;
            }
            for (int i = startColumn; i >= frozenColumnCount; i--)
            {
                if (ActiveSheet.GetActualColumnWidth(i, SheetArea.Cells) > 0.0)
                {
                    return i;
                }
            }
            return -1;
        }

        internal int TryGetPreviousScrollableRow(int startRow)
        {
            int frozenRowCount = ActiveSheet.FrozenRowCount;
            int num2 = (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1;
            if (startRow < frozenRowCount)
            {
                return frozenRowCount;
            }
            if (startRow > num2)
            {
                return num2;
            }
            for (int i = startRow; i >= frozenRowCount; i--)
            {
                if (ActiveSheet.GetActualRowHeight(i, SheetArea.Cells) > 0.0)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void UnSelectedAllFloatingObjects()
        {
            FloatingObject[] allFloatingObjects = GetAllFloatingObjects();
            if (allFloatingObjects.Length > 0)
            {
                FloatingObject[] objArray2 = allFloatingObjects;
                for (int i = 0; i < objArray2.Length; i++)
                {
                    IFloatingObject obj2 = objArray2[i];
                    obj2.IsSelected = false;
                }
            }
        }

        void UnSelectFloatingObject(FloatingObject floatingObject)
        {
            try
            {
                if (!_isMouseDownFloatingObject)
                {
                    bool flag;
                    bool flag2;
                    KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                    if (((flag2 || flag) && !(floatingObject.Locked && ActiveSheet.Protect)) && floatingObject.IsSelected)
                    {
                        floatingObject.IsSelected = false;
                    }
                }
            }
            finally
            {
                _isMouseDownFloatingObject = false;
            }
        }

        internal void UpdateColumnHeaderCellsState(int row, int column, int rowCount, int columnCount)
        {
            if (_colHeaders != null)
            {
                rowCount = ((rowCount < 0) || (row < 0)) ? ActiveSheet.ColumnHeader.RowCount : rowCount;
                columnCount = ((columnCount < 0) || (column < 0)) ? ActiveSheet.ColumnCount : columnCount;
                row = (row < 0) ? 0 : row;
                column = (column < 0) ? 0 : column;
                new CellRange(row, column, rowCount, columnCount);
                foreach (var viewport in _colHeaders)
                {
                    if (viewport != null)
                    {
                        ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(viewport.ColumnViewportIndex);
                        GetColumnHeaderRowLayoutModel();
                        if ((viewportColumnLayoutModel != null) && (viewportColumnLayoutModel.Count > 0))
                        {
                            for (int i = row; i < (row + rowCount); i++)
                            {
                                var presenter = viewport.GetRow(i);
                                if (presenter != null)
                                {
                                    for (int j = Math.Max(column, viewportColumnLayoutModel[0].Column); j < (column + columnCount); j++)
                                    {
                                        if (j > viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column)
                                        {
                                            break;
                                        }
                                        var cell = presenter.GetCell(j);
                                        if (cell != null)
                                        {
                                            cell.ApplyState();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void UpdateCornerHeaderCellState()
        {
            if (_cornerPanel != null)
            {
                _cornerPanel.ApplyState();
            }
        }

        void UpdateCurrentFillRange()
        {
            _currentFillRange = GetCurrentFillRange();
        }

        void UpdateCurrentFillSettings()
        {
            if (!IsDragFillWholeRows && !IsDragFillWholeColumns)
            {
                if ((_dragToRow < DragFillStartTopRow) || (_dragToRow > DragFillStartBottomRow))
                {
                    if (_dragToRow >= DragFillStartTopRow)
                    {
                        if (_dragToRow > DragFillStartBottomRow)
                        {
                            if ((_dragToColumn >= DragFillStartLeftColumn) && (_dragToColumn <= DragFillStartRightColumn))
                            {
                                _currentFillDirection = DragFillDirection.Down;
                            }
                            else if (_dragToColumn < DragFillStartLeftColumn)
                            {
                                int num9 = Math.Abs((int)(_dragToColumn - DragFillStartLeftColumn));
                                if (Math.Abs((int)(_dragToRow - DragFillStartBottomRow)) >= num9)
                                {
                                    _currentFillDirection = DragFillDirection.Down;
                                }
                                else
                                {
                                    _currentFillDirection = DragFillDirection.Left;
                                }
                            }
                            else if (_dragToColumn > DragFillStartRightColumn)
                            {
                                int num11 = Math.Abs((int)(_dragToColumn - DragFillStartRightColumn));
                                if (Math.Abs((int)(_dragToRow - DragFillStartBottomRow)) >= num11)
                                {
                                    _currentFillDirection = DragFillDirection.Down;
                                }
                                else
                                {
                                    _currentFillDirection = DragFillDirection.Right;
                                }
                            }
                        }
                    }
                    else if ((_dragToColumn >= DragFillStartLeftColumn) && (_dragToColumn <= DragFillStartRightColumn))
                    {
                        _currentFillDirection = DragFillDirection.Up;
                    }
                    else if (_dragToColumn < DragFillStartLeftColumn)
                    {
                        int num5 = Math.Abs((int)(_dragToColumn - DragFillStartLeftColumn));
                        if (Math.Abs((int)(_dragToRow - DragFillStartTopRow)) >= num5)
                        {
                            _currentFillDirection = DragFillDirection.Up;
                        }
                        else
                        {
                            _currentFillDirection = DragFillDirection.Left;
                        }
                    }
                    else if (_dragToColumn > DragFillStartRightColumn)
                    {
                        int num7 = Math.Abs((int)(_dragToColumn - DragFillStartRightColumn));
                        if (Math.Abs((int)(_dragToRow - DragFillStartTopRow)) >= num7)
                        {
                            _currentFillDirection = DragFillDirection.Up;
                        }
                        else
                        {
                            _currentFillDirection = DragFillDirection.Right;
                        }
                    }
                }
                else if ((_dragToColumn >= DragFillStartLeftColumn) && (_dragToColumn <= DragFillStartRightColumn))
                {
                    int num = Math.Abs((int)(_dragToColumn - DragFillStartRightColumn));
                    int num2 = Math.Abs((int)(_dragToRow - DragFillStartBottomRow));
                    if (num2 > num)
                    {
                        _currentFillDirection = DragFillDirection.UpClear;
                    }
                    else if (num2 < num)
                    {
                        _currentFillDirection = DragFillDirection.LeftClear;
                    }
                    else
                    {
                        RowLayout dragFillStartBottomRowLayout = DragFillStartBottomRowLayout;
                        if (dragFillStartBottomRowLayout == null)
                        {
                            dragFillStartBottomRowLayout = DragFillToViewportBottomRowLayout;
                        }
                        if (MousePosition.Y > (dragFillStartBottomRowLayout.Y + dragFillStartBottomRowLayout.Height))
                        {
                            _currentFillDirection = DragFillDirection.Down;
                        }
                        else
                        {
                            ColumnLayout dragFillStartRightColumnLayout = DragFillStartRightColumnLayout;
                            if (dragFillStartRightColumnLayout == null)
                            {
                                dragFillStartRightColumnLayout = DragFillToViewportRightColumnLayout;
                            }
                            double num3 = (dragFillStartRightColumnLayout.X + dragFillStartRightColumnLayout.Width) - MousePosition.X;
                            double num4 = (dragFillStartBottomRowLayout.Y + dragFillStartBottomRowLayout.Height) - MousePosition.Y;
                            if (num3 >= num4)
                            {
                                _currentFillDirection = DragFillDirection.LeftClear;
                            }
                            else
                            {
                                _currentFillDirection = DragFillDirection.UpClear;
                            }
                        }
                    }
                }
                else if (_dragToColumn < DragFillStartLeftColumn)
                {
                    _currentFillDirection = DragFillDirection.Left;
                }
                else if (_dragToColumn > DragFillStartRightColumn)
                {
                    _currentFillDirection = DragFillDirection.Right;
                }
            }
            else if (IsDragFillWholeColumns)
            {
                if ((_dragToColumn >= DragFillStartLeftColumn) && (_dragToColumn <= DragFillStartRightColumn))
                {
                    _currentFillDirection = DragFillDirection.LeftClear;
                }
                else if (_dragToColumn < DragFillStartLeftColumn)
                {
                    _currentFillDirection = DragFillDirection.Left;
                }
                else if (_dragToColumn > DragFillStartRightColumn)
                {
                    _currentFillDirection = DragFillDirection.Right;
                }
            }
            else if (IsDragFillWholeRows)
            {
                if ((_dragToRow >= DragFillStartTopRow) && (_dragToRow <= DragFillStartBottomRow))
                {
                    _currentFillDirection = DragFillDirection.UpClear;
                }
                else if (_dragToRow < DragFillStartTopRow)
                {
                    _currentFillDirection = DragFillDirection.Up;
                }
                else if (_dragToRow > DragFillStartBottomRow)
                {
                    _currentFillDirection = DragFillDirection.Down;
                }
            }
        }


        void UpdateDragFillStartRange()
        {
            if (ActiveSheet.Selections.Count == 1)
            {
                _dragFillStartRange = ActiveSheet.Selections[0];
            }
            else if (ActiveSheet.ActiveCell != null)
            {
                _dragFillStartRange = new CellRange(ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, 1, 1);
            }
        }

        void UpdateDragFillViewportInfoAndStartTimer()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            _dragStartRowViewport = savedHitTestInformation.RowViewportIndex;
            _dragStartColumnViewport = savedHitTestInformation.ColumnViewportIndex;
            _dragToRowViewport = savedHitTestInformation.RowViewportIndex;
            _dragToColumnViewport = savedHitTestInformation.ColumnViewportIndex;
            UpdateDragStartRangeViewports();
            StartScrollTimer();
        }

        void UpdateDragIndicatorAndStartTimer(CellRange fromRange)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            if (_dragDropInsertIndicator == null)
            {
                _dragDropInsertIndicator = new Grid();
                _dragDropInsertIndicator.Visibility = Visibility.Collapsed;
                Rectangle rectangle = new Rectangle();
                rectangle.Stroke = brush;
                rectangle.StrokeThickness = 1.0;
                rectangle.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle.StrokeDashOffset = 0.5;
                _dragDropInsertIndicator.Children.Add(rectangle);
                Rectangle rectangle2 = new Rectangle();
                rectangle2.Stroke = brush;
                rectangle2.StrokeThickness = 1.0;
                rectangle2.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle2.StrokeDashOffset = 0.5;
                rectangle2.Margin = new Thickness(1.0);
                _dragDropInsertIndicator.Children.Add(rectangle2);
                Rectangle rectangle3 = new Rectangle();
                rectangle3.Stroke = brush;
                rectangle3.StrokeThickness = 1.0;
                rectangle3.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle3.StrokeDashOffset = 0.5;
                rectangle3.Margin = new Thickness(2.0);
                _dragDropInsertIndicator.Children.Add(rectangle3);
                TrackersContainer.Children.Add(_dragDropInsertIndicator);
            }
            if (_dragDropIndicator == null)
            {
                _dragDropIndicator = new Grid();
                _dragDropIndicator.Visibility = Visibility.Collapsed;
                Rectangle rectangle4 = new Rectangle();
                rectangle4.Stroke = brush;
                rectangle4.StrokeThickness = 1.0;
                rectangle4.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle4.StrokeDashOffset = 0.5;
                _dragDropIndicator.Children.Add(rectangle4);
                Rectangle rectangle5 = new Rectangle();
                rectangle5.Stroke = brush;
                rectangle5.StrokeThickness = 1.0;
                rectangle5.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle5.StrokeDashOffset = 0.5;
                _dragDropIndicator.Children.Add(rectangle5);
                Rectangle rectangle6 = new Rectangle();
                rectangle6.Stroke = brush;
                rectangle6.StrokeThickness = 1.0;
                rectangle6.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle6.StrokeDashOffset = 0.5;
                _dragDropIndicator.Children.Add(rectangle6);
                Rectangle rectangle7 = new Rectangle();
                rectangle7.Stroke = brush;
                rectangle7.StrokeThickness = 1.0;
                rectangle7.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle7.StrokeDashOffset = 0.5;
                _dragDropIndicator.Children.Add(rectangle7);
                Rectangle rectangle8 = new Rectangle();
                rectangle8.Stroke = brush;
                rectangle8.StrokeThickness = 1.0;
                rectangle8.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle8.StrokeDashOffset = 0.5;
                rectangle8.Margin = new Thickness(1.0);
                _dragDropIndicator.Children.Add(rectangle8);
                Rectangle rectangle9 = new Rectangle();
                rectangle9.Stroke = brush;
                rectangle9.StrokeThickness = 1.0;
                rectangle9.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle9.StrokeDashOffset = 0.5;
                rectangle9.Margin = new Thickness(1.0);
                _dragDropIndicator.Children.Add(rectangle9);
                Rectangle rectangle10 = new Rectangle();
                rectangle10.Stroke = brush;
                rectangle10.StrokeThickness = 1.0;
                rectangle10.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle10.StrokeDashOffset = 0.5;
                rectangle10.Margin = new Thickness(1.0);
                _dragDropIndicator.Children.Add(rectangle10);
                Rectangle rectangle11 = new Rectangle();
                rectangle11.Stroke = brush;
                rectangle11.StrokeThickness = 1.0;
                rectangle11.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
                rectangle11.StrokeDashOffset = 0.5;
                rectangle11.Margin = new Thickness(1.0);
                _dragDropIndicator.Children.Add(rectangle11);
                TrackersContainer.Children.Add(_dragDropIndicator);
            }
            _dragDropFromRange = fromRange;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation != null) && (savedHitTestInformation.ViewportInfo != null))
            {
                int row = savedHitTestInformation.ViewportInfo.Row;
                int column = savedHitTestInformation.ViewportInfo.Column;
                int num3 = (fromRange.Row < 0) ? 0 : fromRange.Row;
                int num4 = (fromRange.Column < 0) ? 0 : fromRange.Column;
                int num5 = (fromRange.Row < 0) ? (ActiveSheet.RowCount - 1) : ((fromRange.Row + fromRange.RowCount) - 1);
                int num6 = (fromRange.Column < 0) ? (ActiveSheet.ColumnCount - 1) : ((fromRange.Column + fromRange.ColumnCount) - 1);
                if (row < num3)
                {
                    row = num3;
                }
                if (row > num5)
                {
                    row = num5;
                }
                if (column < num4)
                {
                    column = num4;
                }
                if (column > num6)
                {
                    column = num6;
                }
                _dragDropRowOffset = row - num3;
                _dragDropColumnOffset = column - num4;
                int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                _dragStartColumnViewport = savedHitTestInformation.ColumnViewportIndex;
                if ((savedHitTestInformation.ColumnViewportIndex == -1) && (column == ActiveSheet.FrozenColumnCount))
                {
                    _dragStartColumnViewport = 0;
                }
                else if ((savedHitTestInformation.ColumnViewportIndex == 0) && (column == (ActiveSheet.FrozenColumnCount - 1)))
                {
                    _dragStartColumnViewport = -1;
                }
                else if ((savedHitTestInformation.ColumnViewportIndex == columnViewportCount) && (column == ((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1)))
                {
                    _dragStartColumnViewport = columnViewportCount - 1;
                }
                else if ((savedHitTestInformation.ColumnViewportIndex == (columnViewportCount - 1)) && (column == (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
                {
                    _dragStartColumnViewport = columnViewportCount;
                }
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                _dragStartRowViewport = savedHitTestInformation.RowViewportIndex;
                if ((savedHitTestInformation.RowViewportIndex == -1) && (row == ActiveSheet.FrozenRowCount))
                {
                    _dragStartRowViewport = 0;
                }
                else if ((savedHitTestInformation.RowViewportIndex == 0) && (row == (ActiveSheet.FrozenRowCount - 1)))
                {
                    _dragStartRowViewport = -1;
                }
                else if ((savedHitTestInformation.RowViewportIndex == rowViewportCount) && (row == ((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1)))
                {
                    _dragStartRowViewport = rowViewportCount - 1;
                }
                else if ((savedHitTestInformation.RowViewportIndex == (rowViewportCount - 1)) && (row == (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
                {
                    _dragStartRowViewport = rowViewportCount;
                }
                StartScrollTimer();
            }
        }

        void UpdateDragStartRangeViewports()
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            int dragFillStartTopRow = DragFillStartTopRow;
            if ((dragFillStartTopRow >= 0) && (dragFillStartTopRow < ActiveSheet.FrozenRowCount))
            {
                _dragFillStartTopRowViewport = -1;
            }
            else if ((dragFillStartTopRow >= ActiveSheet.FrozenRowCount) && (dragFillStartTopRow < (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
            {
                if (DragFillStartBottomRow >= (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount))
                {
                    _dragFillStartTopRowViewport = viewportInfo.RowViewportCount - 1;
                }
                else
                {
                    _dragFillStartTopRowViewport = _dragStartRowViewport;
                }
            }
            else if (dragFillStartTopRow >= (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount))
            {
                _dragFillStartTopRowViewport = viewportInfo.RowViewportCount;
            }
            if (IsDragFillWholeColumns)
            {
                if (ActiveSheet.FrozenTrailingColumnCount == 0)
                {
                    _dragFillStartBottomRowViewport = viewportInfo.RowViewportCount - 1;
                }
                else
                {
                    _dragFillStartBottomRowViewport = viewportInfo.RowViewportCount;
                }
            }
            else
            {
                _dragFillStartBottomRowViewport = _dragStartRowViewport;
            }
            int dragFillStartLeftColumn = DragFillStartLeftColumn;
            if ((dragFillStartLeftColumn >= 0) && (dragFillStartLeftColumn < ActiveSheet.FrozenColumnCount))
            {
                _dragFillStartLeftColumnViewport = -1;
            }
            else if ((dragFillStartLeftColumn >= ActiveSheet.FrozenColumnCount) && (dragFillStartLeftColumn < (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
            {
                if (DragFillStartRightColumn >= (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount))
                {
                    _dragFillStartLeftColumnViewport = viewportInfo.ColumnViewportCount - 1;
                }
                else
                {
                    _dragFillStartLeftColumnViewport = _dragStartColumnViewport;
                }
            }
            else if (dragFillStartLeftColumn >= (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount))
            {
                _dragFillStartLeftColumnViewport = viewportInfo.ColumnViewportCount;
            }
            if (IsDragFillWholeRows)
            {
                if (ActiveSheet.FrozenTrailingRowCount == 0)
                {
                    _dragFillStartRightColumnViewport = viewportInfo.ColumnViewportCount - 1;
                }
                else
                {
                    _dragFillStartRightColumnViewport = viewportInfo.ColumnViewportCount;
                }
            }
            else
            {
                _dragFillStartRightColumnViewport = _dragStartColumnViewport;
            }
        }

        void UpdateDragToColumn()
        {
            double maxValue;
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(_dragToColumnViewport, MousePosition.X);
            if (viewportColumnLayoutNearX != null)
            {
                _dragToColumn = viewportColumnLayoutNearX.Column;
                maxValue = (viewportColumnLayoutNearX.X + viewportColumnLayoutNearX.Width) - 1.0;
            }
            else
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                if (MousePosition.X > savedHitTestInformation.HitPoint.X)
                {
                    _dragToColumn = DragFillStartViewportRightColumn;
                    maxValue = (DragFillStartViewportRightColumnLayout.X + DragFillStartViewportRightColumnLayout.Width) - 1.0;
                }
                else
                {
                    _dragToColumn = DragFillStartViewportLeftColumn;
                    maxValue = double.MaxValue;
                }
            }
            if (_dragToColumn == DragFillToViewportRightColumn)
            {
                double width = 0.0;
                Rect rowHeaderRectangle = GetRowHeaderRectangle(_dragStartRowViewport);
                if (!rowHeaderRectangle.IsEmpty)
                {
                    width = rowHeaderRectangle.Width;
                }
                for (int i = -1; i <= _dragToColumnViewport; i++)
                {
                    width += GetViewportWidth(i);
                }
                if (maxValue > width)
                {
                    _dragToColumn = DragFillToViewportRightColumn - 1;
                    if (_dragToColumn < 0)
                    {
                        _dragToColumn = 0;
                    }
                }
            }
        }

        void UpdateDragToColumnViewport()
        {
            _dragToColumnViewport = _dragStartColumnViewport;
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(_dragToColumnViewport, MousePosition.X);
            if ((viewportColumnLayoutNearX == null) || (GetViewportColumnLayoutModel(_dragToColumnViewport).FindColumn(viewportColumnLayoutNearX.Column) == null))
            {
                double x = GetHitInfo().HitPoint.X;
                int columnViewportCount = GetViewportInfo().ColumnViewportCount;
                if (MousePosition.X < x)
                {
                    if ((_dragStartColumnViewport == 0) && (_dragToColumn <= ActiveSheet.FrozenColumnCount))
                    {
                        _dragToColumnViewport = -1;
                    }
                    else if ((_dragStartColumnViewport == columnViewportCount) && (_dragToColumn <= (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
                    {
                        _dragToColumnViewport = columnViewportCount - 1;
                    }
                }
                else if ((_dragStartColumnViewport == -1) && (_dragToColumn >= ActiveSheet.FrozenColumnCount))
                {
                    _dragToColumnViewport = 0;
                }
                else if ((_dragStartColumnViewport == (columnViewportCount - 1)) && (_dragToColumn >= (ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount)))
                {
                    _dragToColumnViewport = columnViewportCount;
                }
            }
        }

        void UpdateDragToCoordicates()
        {
            UpdateDragToRow();
            UpdateDragToColumn();
        }

        void UpdateDragToRow()
        {
            double maxValue;
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(_dragToRowViewport, MousePosition.Y);
            if (viewportRowLayoutNearY != null)
            {
                _dragToRow = viewportRowLayoutNearY.Row;
                maxValue = (viewportRowLayoutNearY.Y + viewportRowLayoutNearY.Height) - 1.0;
            }
            else
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                if (MousePosition.Y > savedHitTestInformation.HitPoint.Y)
                {
                    _dragToRow = DragFillStartViewportBottomRow;
                    maxValue = (DragFillStartViewportBottomRowLayout.Y + DragFillStartViewportBottomRowLayout.Height) - 1.0;
                }
                else
                {
                    _dragToRow = DragFillStartViewportTopRow;
                    maxValue = double.MaxValue;
                }
            }
            if (_dragToRow == DragFillToViewportBottomRow)
            {
                double height = 0.0;
                Rect columnHeaderRectangle = GetColumnHeaderRectangle(_dragStartColumnViewport);
                if (!columnHeaderRectangle.IsEmpty)
                {
                    height = columnHeaderRectangle.Height;
                }
                for (int i = -1; i <= _dragToRowViewport; i++)
                {
                    height += GetViewportHeight(i);
                }
                if (maxValue > height)
                {
                    _dragToRow = DragFillToViewportBottomRow - 1;
                    if (_dragToRow < 0)
                    {
                        _dragToRow = 0;
                    }
                }
            }
        }

        void UpdateDragToRowViewport()
        {
            _dragToRowViewport = _dragStartRowViewport;
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(_dragToRowViewport, MousePosition.Y);
            if ((viewportRowLayoutNearY == null) || (GetViewportRowLayoutModel(_dragToRowViewport).FindRow(viewportRowLayoutNearY.Row) == null))
            {
                double y = GetHitInfo().HitPoint.Y;
                int rowViewportCount = GetViewportInfo().RowViewportCount;
                if (MousePosition.Y < y)
                {
                    if ((_dragStartRowViewport == 0) && (_dragToRow <= ActiveSheet.FrozenRowCount))
                    {
                        _dragToRowViewport = -1;
                    }
                    else if ((_dragStartRowViewport == rowViewportCount) && (_dragToRow <= (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
                    {
                        _dragToRowViewport = rowViewportCount - 1;
                    }
                }
                else if ((_dragStartRowViewport == -1) && (_dragToRow >= ActiveSheet.FrozenRowCount))
                {
                    _dragToRowViewport = 0;
                }
                else if ((_dragStartRowViewport == (rowViewportCount - 1)) && (_dragToRow >= (ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount)))
                {
                    _dragToRowViewport = rowViewportCount;
                }
            }
        }

        void UpdateDragToViewports()
        {
            UpdateDragToRowViewport();
            UpdateDragToColumnViewport();
        }

        void UpdateFloatingObjectsMovingResizingToColumn()
        {
            double maxValue;
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(_dragToColumnViewport, MousePosition.X);
            if (viewportColumnLayoutNearX != null)
            {
                _dragToColumn = viewportColumnLayoutNearX.Column;
                maxValue = (viewportColumnLayoutNearX.X + viewportColumnLayoutNearX.Width) - 1.0;
            }
            else
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                if (MousePosition.X > savedHitTestInformation.HitPoint.X)
                {
                    _dragToColumn = GetViewportRightColumn(_dragStartColumnViewport);
                    ColumnLayout layout2 = GetViewportColumnLayoutModel(_dragStartColumnViewport).FindColumn(_dragToColumn);
                    maxValue = (layout2.X + layout2.Width) - 1.0;
                }
                else
                {
                    _dragToColumn = GetViewportLeftColumn(_dragToColumnViewport);
                    maxValue = double.MaxValue;
                }
            }
            int viewportRightColumn = GetViewportRightColumn(_dragToColumnViewport);
            if (_dragToColumn == viewportRightColumn)
            {
                SheetLayout sheetLayout = GetSheetLayout();
                double num3 = sheetLayout.GetViewportX(_dragToColumnViewport) + sheetLayout.GetViewportWidth(_dragToColumnViewport);
                if (maxValue > num3)
                {
                    _dragToColumn = GetViewportRightColumn(_dragToColumnViewport) - 1;
                    if (_dragToColumn < 0)
                    {
                        _dragToColumn = 0;
                    }
                }
            }
        }

        void UpdateFloatingObjectsMovingResizingToCoordicates()
        {
            UpdateFloatingObjectsMovingResizingToRow();
            UpdateFloatingObjectsMovingResizingToColumn();
        }

        void UpdateFloatingObjectsMovingResizingToRow()
        {
            double maxValue;
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(_dragToRowViewport, MousePosition.Y);
            if (viewportRowLayoutNearY != null)
            {
                _dragToRow = viewportRowLayoutNearY.Row;
                maxValue = (viewportRowLayoutNearY.Y + viewportRowLayoutNearY.Height) - 1.0;
            }
            else
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                if (MousePosition.Y > savedHitTestInformation.HitPoint.Y)
                {
                    _dragToRow = GetViewportBottomRow(_dragStartRowViewport);
                    RowLayout layout2 = GetViewportRowLayoutModel(_dragStartRowViewport).FindRow(_dragToRow);
                    maxValue = (layout2.Y + layout2.Height) - 1.0;
                }
                else
                {
                    _dragToRow = GetViewportTopRow(_dragStartRowViewport);
                    maxValue = double.MaxValue;
                }
            }
            int viewportBottomRow = GetViewportBottomRow(_dragToRowViewport);
            if (_dragToRow == viewportBottomRow)
            {
                SheetLayout sheetLayout = GetSheetLayout();
                double num3 = sheetLayout.GetViewportY(_dragToRowViewport) + sheetLayout.GetViewportHeight(_dragToRowViewport);
                if (maxValue > num3)
                {
                    _dragToRow = GetViewportBottomRow(_dragToRowViewport) - 1;
                    if (_dragToRow < 0)
                    {
                        _dragToRow = 0;
                    }
                }
            }
        }

        void UpdateFloatingObjectsMovingResizingToViewports()
        {
            UpdateDragToRowViewport();
            UpdateDragToColumnViewport();
        }

        void UpdateFocusIndicator()
        {
            UpdateColumnHeaderCellsState(-1, _currentActiveColumnIndex, -1, 1);
            UpdateRowHeaderCellsState(_currentActiveRowIndex, -1, 1, -1);
            RefreshSelection();
            _currentActiveRowIndex = ActiveSheet.ActiveRowIndex;
            _currentActiveColumnIndex = ActiveSheet.ActiveColumnIndex;
            UpdateColumnHeaderCellsState(-1, _currentActiveColumnIndex, -1, 1);
            UpdateRowHeaderCellsState(_currentActiveRowIndex, -1, 1, -1);
        }

        internal void UpdateFreezeLines()
        {
            if (!IsTouchZooming)
            {
                SheetLayout sheetLayout = GetSheetLayout();
                ViewportInfo viewportInfo = GetViewportInfo();
                int columnViewportCount = viewportInfo.ColumnViewportCount;
                int rowViewportCount = viewportInfo.RowViewportCount;
                if (_columnFreezeLine == null)
                {
                    _columnFreezeLine = CreateFreezeLine();
                }
                if ((sheetLayout.FrozenWidth > 0.0) && ShowFreezeLine)
                {
                    if (!TrackersContainer.Children.Contains(_columnFreezeLine))
                    {
                        TrackersContainer.Children.Add(_columnFreezeLine);
                    }
                    int frozenColumnCount = ActiveSheet.FrozenColumnCount;
                    if (frozenColumnCount > ActiveSheet.ColumnCount)
                    {
                        frozenColumnCount = ActiveSheet.ColumnCount;
                    }
                    ColumnLayout layout2 = GetViewportColumnLayoutModel(-1).FindColumn(frozenColumnCount - 1);
                    if (layout2 != null)
                    {
                        _columnFreezeLine.X1 = layout2.X + layout2.Width;
                        _columnFreezeLine.X2 = _columnFreezeLine.X1;
                        _columnFreezeLine.Y1 = 0.0;
                        _columnFreezeLine.Y2 = sheetLayout.FrozenTrailingY + sheetLayout.FrozenTrailingHeight;
                    }
                    else
                    {
                        TrackersContainer.Children.Remove(_columnFreezeLine);
                    }
                }
                else
                {
                    TrackersContainer.Children.Remove(_columnFreezeLine);
                }
                if (_columnTrailingFreezeLine == null)
                {
                    _columnTrailingFreezeLine = CreateFreezeLine();
                }
                if ((sheetLayout.FrozenTrailingWidth > 0.0) && ShowFreezeLine)
                {
                    if (!TrackersContainer.Children.Contains(_columnTrailingFreezeLine))
                    {
                        TrackersContainer.Children.Add(_columnTrailingFreezeLine);
                    }
                    ColumnLayout layout3 = GetViewportColumnLayoutModel(columnViewportCount).FindColumn(Math.Max(ActiveSheet.FrozenColumnCount, ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount));
                    if (layout3 != null)
                    {
                        _columnTrailingFreezeLine.X1 = layout3.X;
                        _columnTrailingFreezeLine.X2 = _columnTrailingFreezeLine.X1;
                        _columnTrailingFreezeLine.Y1 = 0.0;
                        _columnTrailingFreezeLine.Y2 = sheetLayout.FrozenTrailingY + sheetLayout.FrozenTrailingHeight;
                    }
                    else
                    {
                        TrackersContainer.Children.Remove(_columnTrailingFreezeLine);
                    }
                }
                else
                {
                    TrackersContainer.Children.Remove(_columnTrailingFreezeLine);
                }
                if (_rowFreezeLine == null)
                {
                    _rowFreezeLine = CreateFreezeLine();
                }
                if ((sheetLayout.FrozenHeight > 0.0) && ShowFreezeLine)
                {
                    if (!TrackersContainer.Children.Contains(_rowFreezeLine))
                    {
                        TrackersContainer.Children.Add(_rowFreezeLine);
                    }
                    int frozenRowCount = ActiveSheet.FrozenRowCount;
                    if (ActiveSheet.RowCount < frozenRowCount)
                    {
                        frozenRowCount = ActiveSheet.RowCount;
                    }
                    RowLayout layout4 = GetViewportRowLayoutModel(-1).FindRow(frozenRowCount - 1);
                    if (layout4 != null)
                    {
                        _rowFreezeLine.X1 = 0.0;
                        if (_translateOffsetX >= 0.0)
                        {
                            _rowFreezeLine.X2 = sheetLayout.FrozenTrailingX + sheetLayout.FrozenTrailingWidth;
                        }
                        else
                        {
                            _rowFreezeLine.X2 = (sheetLayout.FrozenTrailingX + _translateOffsetX) + sheetLayout.FrozenTrailingWidth;
                        }
                        _rowFreezeLine.Y1 = layout4.Y + layout4.Height;
                        _rowFreezeLine.Y2 = _rowFreezeLine.Y1;
                    }
                    else
                    {
                        TrackersContainer.Children.Remove(_rowFreezeLine);
                    }
                }
                else
                {
                    TrackersContainer.Children.Remove(_rowFreezeLine);
                }
                if (_rowTrailingFreezeLine == null)
                {
                    _rowTrailingFreezeLine = CreateFreezeLine();
                }
                if ((sheetLayout.FrozenTrailingHeight > 0.0) && ShowFreezeLine)
                {
                    if (!TrackersContainer.Children.Contains(_rowTrailingFreezeLine))
                    {
                        TrackersContainer.Children.Add(_rowTrailingFreezeLine);
                    }
                    RowLayout layout5 = GetViewportRowLayoutModel(rowViewportCount).FindRow(Math.Max(ActiveSheet.FrozenRowCount, ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount));
                    if (layout5 != null)
                    {
                        _rowTrailingFreezeLine.X1 = 0.0;
                        _rowTrailingFreezeLine.X2 = sheetLayout.FrozenTrailingX + sheetLayout.FrozenTrailingWidth;
                        _rowTrailingFreezeLine.Y1 = layout5.Y + ((_translateOffsetY < 0.0) ? _translateOffsetY : 0.0);
                        _rowTrailingFreezeLine.Y2 = _rowTrailingFreezeLine.Y1;
                    }
                    else
                    {
                        TrackersContainer.Children.Remove(_rowTrailingFreezeLine);
                    }
                }
                else
                {
                    TrackersContainer.Children.Remove(_rowTrailingFreezeLine);
                }
            }
        }

        internal void UpdateHeaderCellsState(int row, int rowCount, int column, int columnCount)
        {
            UpdateColumnHeaderCellsState(-1, column, -1, columnCount);
            UpdateRowHeaderCellsState(row, -1, rowCount, -1);
            UpdateHeaderCellsStateInSpanArea();
            UpdateFocusIndicator();
            UpdateHeaderCellsStateInSpanArea();
            UpdateCornerHeaderCellState();
        }

        void UpdateHeaderCellsStateInSpanArea()
        {
            Enumerable.ToList<CellLayout>((IEnumerable<CellLayout>)(from cellLayout in GetViewportCellLayoutModel(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex()) select cellLayout)).ForEach<CellLayout>(delegate (CellLayout cellLayout)
            {
                UpdateRowHeaderCellsState(cellLayout.Row, -1, cellLayout.RowCount, -1);
                UpdateColumnHeaderCellsState(-1, cellLayout.Column, -1, cellLayout.ColumnCount);
            });
        }

        void UpdateHitFilterCellState()
        {
            if (_hitFilterInfo != null)
            {
                if (_hitFilterInfo.SheetArea == SheetArea.ColumnHeader)
                {
                    var columnHeaderRowsPresenter = GetColumnHeaderRowsPresenter(_hitFilterInfo.ColumnViewportIndex);
                    if (columnHeaderRowsPresenter != null)
                    {
                        var row = columnHeaderRowsPresenter.GetRow(_hitFilterInfo.Row);
                        if (row != null)
                        {
                            var cell = row.GetCell(_hitFilterInfo.Column);
                            if (cell != null)
                            {
                                cell.ApplyState();
                            }
                        }
                    }
                }
                else if (_hitFilterInfo.SheetArea == SheetArea.Cells)
                {
                    CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(_hitFilterInfo.RowViewportIndex, _hitFilterInfo.ColumnViewportIndex);
                    if (viewportRowsPresenter != null)
                    {
                        RowItem presenter2 = viewportRowsPresenter.GetRow(_hitFilterInfo.Row);
                        if (presenter2 != null)
                        {
                            CellItem base3 = presenter2.GetCell(_hitFilterInfo.Column);
                            if (base3 != null)
                            {
                                base3.ApplyState();
                            }
                        }
                    }
                }
            }
        }

        void UpdateLastClickLocation(HitTestInformation hi)
        {
            if ((hi.HitTestType == HitTestType.Viewport) && (hi.ViewportInfo != null))
            {
                _lastClickLocation = new Point((double)hi.ViewportInfo.Row, (double)hi.ViewportInfo.Column);
            }
            else if ((hi.HitTestType == HitTestType.ColumnHeader) && (hi.HeaderInfo != null))
            {
                _lastClickLocation = new Point((double)hi.HeaderInfo.Row, (double)hi.HeaderInfo.Column);
            }
            else if ((hi.HitTestType == HitTestType.RowHeader) && (hi.HeaderInfo != null))
            {
                _lastClickLocation = new Point((double)hi.HeaderInfo.Row, (double)hi.HeaderInfo.Column);
            }
            else
            {
                _lastClickLocation = new Point(-1.0, -1.0);
            }
        }

        void UpdateResizeToolTip(string text, bool resizeColumn)
        {
            if (resizeColumn && ((Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Column) || (Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both)))
            {
                double x = _mouseDownPosition.X;
                double offsetY = _mouseDownPosition.Y - 40.0;
                TooltipHelper.ShowTooltip(text, x, offsetY);
            }
            else if ((Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Row) || (Excel.ShowResizeTip == Dt.Cells.Data.ShowResizeTip.Both))
            {
                double offsetX = _mouseDownPosition.X;
                double num4 = _mouseDownPosition.Y - 38.0;
                TooltipHelper.ShowTooltip(text, offsetX, num4);
            }
        }

        internal void UpdateRowHeaderCellsState(int row, int column, int rowCount, int columnCount)
        {
            if (_rowHeaders != null)
            {
                rowCount = ((rowCount < 0) || (row < 0)) ? ActiveSheet.RowCount : rowCount;
                columnCount = ((columnCount < 0) || (column < 0)) ? ActiveSheet.RowHeader.ColumnCount : columnCount;
                row = (row < 0) ? 0 : row;
                column = (column < 0) ? 0 : column;
                new CellRange(row, column, rowCount, columnCount);
                foreach (var viewport in _rowHeaders)
                {
                    if (viewport != null)
                    {
                        RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(viewport.RowViewportIndex);
                        if ((viewportRowLayoutModel != null) && (viewportRowLayoutModel.Count > 0))
                        {
                            for (int i = Math.Max(row, viewportRowLayoutModel[0].Row); i < (row + rowCount); i++)
                            {
                                if (i > viewportRowLayoutModel[viewportRowLayoutModel.Count - 1].Row)
                                {
                                    break;
                                }
                                var presenter = viewport.GetRow(i);
                                if (presenter != null)
                                {
                                    for (int j = column; j < (column + columnCount); j++)
                                    {
                                        var cell = presenter.GetCell(j);
                                        if (cell != null)
                                        {
                                            cell.ApplyState();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void UpdateScrollToolTip(bool verticalScroll, int scrollTo = -1)
        {
            if (verticalScroll && ((Excel.ShowScrollTip == Dt.Cells.Data.ShowScrollTip.Vertical) || (Excel.ShowScrollTip == Dt.Cells.Data.ShowScrollTip.Both)))
            {
                double offsetX = _mouseDownPosition.X - 100.0;
                double offsetY = _mouseDownPosition.Y - 10.0;
                if (scrollTo == -1)
                {
                    scrollTo = GetViewportTopRow(GetHitInfo().RowViewportIndex) + 1;
                }
                TooltipHelper.ShowTooltip(GetVericalScrollTip(scrollTo), offsetX, offsetY);
            }
            else if ((Excel.ShowScrollTip == Dt.Cells.Data.ShowScrollTip.Horizontal) || (Excel.ShowScrollTip == Dt.Cells.Data.ShowScrollTip.Both))
            {
                double num3 = _mouseDownPosition.X - 20.0;
                double num4 = _mouseDownPosition.Y - 40.0;
                if (scrollTo == -1)
                {
                    scrollTo = GetViewportLeftColumn(GetHitInfo().ColumnViewportIndex) + 1;
                }
                TooltipHelper.ShowTooltip(GetHorizentalScrollTip(scrollTo), num3, num4);
            }
        }

        void UpdateTabStrip()
        {
            if ((_tabStrip != null) && (_tabStrip.TabsPresenter != null))
            {
                _tabStrip.TabsPresenter.InvalidateMeasure();
                _tabStrip.TabsPresenter.InvalidateArrange();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected void UpdateTouchHitTestInfoForHold(Point point)
        {
            GetHitInfo();
            Point point2 = point;
            SaveHitInfo(TouchHitTest(point2.X, point2.Y));
            _lastClickPoint = new Point(point2.X, point2.Y);
        }

        bool ValidateFillRange(CellRange fillRange)
        {
            bool flag = true;
            string message = string.Empty;
            if (HasSpans(fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount))
            {
                flag = false;
                message = ResourceStrings.SheetViewDragFillChangePartOfMergeCell;
            }
            if ((flag && ActiveSheet.Protect) && IsAnyCellInRangeLocked(ActiveSheet, fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount))
            {
                flag = false;
                message = ResourceStrings.SheetViewDragFillChangeProtectCell;
            }
            if (!flag)
            {
                RaiseInvalidOperation(message, null, null);
            }
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <param name="viewportWidth"></param>
        internal void AddColumnViewport(int columnViewportIndex, double viewportWidth)
        {
            ActiveSheet.AddColumnViewport(columnViewportIndex, viewportWidth / ((double)ZoomFactor));
            InvalidateLayout();
            InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <param name="viewportHeight"></param>
        internal void AddRowViewport(int rowViewportIndex, double viewportHeight)
        {
            ActiveSheet.AddRowViewport(rowViewportIndex, viewportHeight / ((double)ZoomFactor));
            InvalidateLayout();
            InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        void AdjustViewportLeftColumn()
        {
            if (_translateOffsetX != 0.0)
            {
                int viewportLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
                if (viewportLeftColumn >= ActiveSheet.FrozenColumnCount)
                {
                    ColumnLayout layout = GetColumnLayoutModel(_touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells).FindColumn(viewportLeftColumn);
                    if (layout != null)
                    {
                        double width = layout.Width;
                        int maxLeftScrollableColumn = GetMaxLeftScrollableColumn();
                        if (viewportLeftColumn <= maxLeftScrollableColumn)
                        {
                            if ((_translateOffsetX < 0.0) && (Math.Abs(_translateOffsetX) >= (width / 2.0)))
                            {
                                int nextScrollableColumn = GetNextScrollableColumn(viewportLeftColumn);
                                if (nextScrollableColumn != -1)
                                {
                                    SetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex, nextScrollableColumn);
                                }
                            }
                        }
                        else if (Math.Abs(_translateOffsetX) >= (width / 2.0))
                        {
                            int num5 = GetNextScrollableColumn(viewportLeftColumn);
                            if (num5 != -1)
                            {
                                SetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex, num5);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void AdjustViewportSize()
        {
            SheetLayout layout = GetSheetLayout();
            if (layout != null)
            {
                if (((_cachedViewportWidths != null) && (_touchStartHitTestInfo.ColumnViewportIndex != -1)) && (_touchStartHitTestInfo.ColumnViewportIndex < layout.ColumnPaneCount))
                {
                    layout.SetViewportWidth(_touchStartHitTestInfo.ColumnViewportIndex, _cachedViewportWidths[_touchStartHitTestInfo.ColumnViewportIndex + 1]);
                }
                if (((_cachedViewportHeights != null) && (_touchStartHitTestInfo.RowViewportIndex != -1)) && (_touchStartHitTestInfo.RowViewportIndex < layout.RowPaneCount))
                {
                    layout.SetViewportHeight(_touchStartHitTestInfo.RowViewportIndex, _cachedViewportHeights[_touchStartHitTestInfo.RowViewportIndex + 1]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void AdjustViewportTopRow()
        {
            if (_translateOffsetY != 0.0)
            {
                int viewportTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
                if (viewportTopRow >= ActiveSheet.FrozenRowCount)
                {
                    RowLayout layout = GetRowLayoutModel(_touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells).FindRow(viewportTopRow);
                    if (layout != null)
                    {
                        double height = layout.Height;
                        int maxTopScrollableRow = GetMaxTopScrollableRow();
                        if (viewportTopRow <= maxTopScrollableRow)
                        {
                            if ((_translateOffsetY < 0.0) && (Math.Abs(_translateOffsetY) >= (height / 2.0)))
                            {
                                int nextScrollableRow = GetNextScrollableRow(viewportTopRow);
                                if (nextScrollableRow != -1)
                                {
                                    SetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex, nextScrollableRow);
                                }
                            }
                        }
                        else if (Math.Abs(_translateOffsetY) >= (height / 2.0))
                        {
                            int num5 = GetNextScrollableRow(viewportTopRow);
                            if (num5 != -1)
                            {
                                SetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex, num5);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the top row index async, for performance optimization 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        async void AsynSetViewportTopRow(int rowViewportIndex)
        {
            if (!_pendinging)
            {
                _pendinging = true;
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate
                {
                    _pendinging = false;
                    if (GetViewportTopRow(rowViewportIndex) != _scrollTo)
                    {
                        SetViewportTopRow(rowViewportIndex, _scrollTo);
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ClearViewportsClip()
        {
            SheetLayout layout = GetSheetLayout();
            if (layout != null)
            {
                if (_cellsPanels != null)
                {
                    for (int i = -1; i <= layout.ColumnPaneCount; i++)
                    {
                        for (int j = -1; j <= layout.RowPaneCount; j++)
                        {
                            CellsPanel viewport = _cellsPanels[j + 1, i + 1];
                            if ((viewport != null) && (viewport.Clip != null))
                            {
                                viewport.Clip = null;
                            }
                        }
                    }
                }
                if (_colHeaders != null)
                {
                    for (int k = -1; k <= layout.ColumnPaneCount; k++)
                    {
                        var viewport2 = _colHeaders[k + 1];
                        if ((viewport2 != null) && (viewport2.Clip != null))
                        {
                            viewport2.Clip = null;
                        }
                    }
                }
                if (_rowHeaders != null)
                {
                    for (int m = -1; m <= layout.RowPaneCount; m++)
                    {
                        var viewport3 = _rowHeaders[m + 1];
                        if ((viewport3 != null) && (viewport3.Clip != null))
                        {
                            viewport3.Clip = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ContinueColumnSplitting()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.X <= _columnSplittingTracker.X1)
                    {
                        _columnSplittingTracker.X1 = Math.Max(MousePosition.X, layout.GetViewportX(columnViewportIndex) + (layout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                        break;
                    }
                    _columnSplittingTracker.X1 = Math.Min(MousePosition.X, (layout.GetViewportX(columnViewportIndex + 1) + layout.GetViewportWidth(columnViewportIndex + 1)) - (layout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                    break;

                case HitTestType.ColumnSplitBox:
                    if (MousePosition.X <= _columnSplittingTracker.X1)
                    {
                        _columnSplittingTracker.X1 = Math.Max(MousePosition.X, layout.GetViewportX(columnViewportIndex) + (layout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0));
                        break;
                    }
                    _columnSplittingTracker.X1 = Math.Min(MousePosition.X, (layout.GetViewportX(columnViewportIndex) + layout.GetViewportWidth(columnViewportIndex)) - (layout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0));
                    break;
            }
            _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
        }

        /// <summary>
        /// 
        /// </summary>
        void ContinueRowSplitting()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.Y <= _rowSplittingTracker.Y1)
                    {
                        _rowSplittingTracker.Y1 = Math.Max(MousePosition.Y, layout.GetViewportY(rowViewportIndex) + (layout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                        break;
                    }
                    _rowSplittingTracker.Y1 = Math.Min(MousePosition.Y, (layout.GetViewportY(rowViewportIndex + 1) + layout.GetViewportHeight(rowViewportIndex + 1)) - (layout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                    break;

                case HitTestType.RowSplitBox:
                    if (MousePosition.Y <= _rowSplittingTracker.Y1)
                    {
                        _rowSplittingTracker.Y1 = Math.Max(MousePosition.Y, layout.GetViewportY(rowViewportIndex) + (layout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0));
                        break;
                    }
                    _rowSplittingTracker.Y1 = Math.Min(MousePosition.Y, (layout.GetViewportY(rowViewportIndex) + layout.GetViewportHeight(rowViewportIndex)) - (layout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0));
                    break;
            }
            _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
        }

        /// <summary>
        /// 
        /// </summary>
        void ContinueTabStripResizing()
        {
            SheetLayout layout = GetSheetLayout();
            double tabStripX = layout.TabStripX;
            double tabStripHeight = layout.TabStripHeight;
            double num2 = layout.GetHorizontalScrollBarWidth(0) + layout.TabStripWidth;
            double num3 = Math.Min(Math.Max((double)0.0, (double)(MousePosition.X - tabStripX)), num2);
            _tabStripRatio = num3 / num2;
            InvalidateLayout();
            InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="renderSize"></param>
        /// <returns></returns>
        BitmapSource CreateCachedIamge(UIElement child, Size renderSize)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        Image CreateCachedVisual(ImageSource image, Rect bounds)
        {
#if UWP
            if (!IsZero(bounds.Width) && !IsZero(bounds.Height))
            {
                WriteableBitmap bitmap = image as WriteableBitmap;
                if (bitmap != null)
                {
                    Image image2 = new Image();
                    image2.Width = (double)bitmap.PixelWidth;
                    image2.Height = (double)bitmap.PixelHeight;
                    image2.HorizontalAlignment = (HorizontalAlignment)3;
                    image2.VerticalAlignment = (VerticalAlignment)3;
                    image2.IsHitTestVisible = false;
                    image2.Source = image;
                    image2.Stretch = 0;
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = bounds;
                    image2.Clip = geometry;
                    image2.Margin = new Thickness(-bounds.X, -bounds.Y, bounds.Right - bitmap.PixelWidth, bounds.Bottom - bitmap.PixelHeight);
                    return image2;
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnX"></param>
        /// <param name="rowY"></param>
        /// <param name="viewportWidth"></param>
        /// <param name="viewportHeight"></param>
        /// <param name="viewportColumnIndex"></param>
        /// <param name="viewportRowIndex"></param>
        /// <returns></returns>
        Rect CreateClipRect(double columnX, double rowY, double viewportWidth, double viewportHeight, int viewportColumnIndex, int viewportRowIndex)
        {
            SheetLayout layout = GetSheetLayout();
            double num = columnX;
            double num2 = rowY;
            double num3 = viewportWidth;
            double num4 = viewportHeight;
            if ((ActiveSheet.FrozenColumnCount > 0) && ShowFreezeLine)
            {
                if (viewportColumnIndex == 0)
                {
                    num++;
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
                else if (viewportColumnIndex == -1)
                {
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
            }
            if ((ActiveSheet.FrozenRowCount > 0) && ShowFreezeLine)
            {
                if (viewportRowIndex == 0)
                {
                    num2++;
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
                else if (viewportRowIndex == -1)
                {
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
            }
            if ((ActiveSheet.FrozenTrailingColumnCount > 0) && ShowFreezeLine)
            {
                if (viewportColumnIndex == (layout.ColumnPaneCount - 1))
                {
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
                else if (viewportColumnIndex == layout.ColumnPaneCount)
                {
                    num++;
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
            }
            if ((ActiveSheet.FrozenTrailingRowCount > 0) && ShowFreezeLine)
            {
                if (viewportRowIndex == (layout.RowPaneCount - 1))
                {
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
                else if (viewportRowIndex == layout.RowPaneCount)
                {
                    num2++;
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
            }
            double num5 = 0.0;
            int viewportTopRow = GetViewportTopRow(viewportRowIndex);
            int viewportBottomRow = GetViewportBottomRow(viewportRowIndex);
            if (((viewportTopRow >= 0) && (viewportBottomRow < ActiveSheet.RowCount)) && (viewportTopRow <= viewportBottomRow))
            {
                RowLayoutModel rowLayoutModel = GetRowLayoutModel(viewportRowIndex, SheetArea.Cells);
                if (rowLayoutModel != null)
                {
                    for (int i = viewportTopRow; i <= viewportBottomRow; i++)
                    {
                        RowLayout layout2 = rowLayoutModel.FindRow(i);
                        if (layout2 != null)
                        {
                            num5 += layout2.Height;
                        }
                        if (num5 >= viewportHeight)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    num5 = num4;
                }
            }
            else
            {
                num5 = num4;
            }
            double num9 = 0.0;
            int viewportLeftColumn = GetViewportLeftColumn(viewportColumnIndex);
            int viewportRightColumn = GetViewportRightColumn(viewportColumnIndex);
            if (((viewportLeftColumn >= 0) && (viewportRightColumn < ActiveSheet.ColumnCount)) && (viewportLeftColumn <= viewportRightColumn))
            {
                ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel(viewportColumnIndex, SheetArea.Cells);
                if (columnLayoutModel != null)
                {
                    for (int j = viewportLeftColumn; j <= viewportRightColumn; j++)
                    {
                        ColumnLayout layout3 = columnLayoutModel.FindColumn(j);
                        if (layout3 != null)
                        {
                            num9 += layout3.Width;
                        }
                        if (num9 >= viewportWidth)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    num9 = num3;
                }
            }
            else
            {
                num9 = num3;
            }
            return new Rect { X = num, Y = num2, Width = Math.Min(num9, num3), Height = Math.Min(num5, num4) };
        }

        /// <summary>
        /// 
        /// </summary>
        void EndColumnSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            IsWorking = false;
            IsTouchColumnSplitting = false;
            IsColumnSplitting = false;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.X <= layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex))
                    {
                        num2 = layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex) - MousePosition.X;
                    }
                    else
                    {
                        num2 = Math.Max((double)0.0, (double)((MousePosition.X - layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex)) - layout.GetHorizontalSplitBarWidth(savedHitTestInformation.ColumnViewportIndex)));
                    }
                    if (num2 != 0.0)
                    {
                        double deltaViewportWidth = (_columnSplittingTracker.X1 - layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex)) - (layout.GetHorizontalSplitBarWidth(savedHitTestInformation.ColumnViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.ColumnViewportIndex;
                        if (!RaiseColumnViewportWidthChanging(viewportIndex, deltaViewportWidth))
                        {
                            AdjustColumnViewport(columnViewportIndex, deltaViewportWidth);
                            RaiseColumnViewportWidthChanged(viewportIndex, deltaViewportWidth);
                        }
                    }
                    goto Label_0258;

                case HitTestType.ColumnSplitBox:
                    if (ColumnSplitBoxAlignment != SplitBoxAlignment.Leading)
                    {
                        num2 = Math.Max((double)0.0, (double)(((layout.GetViewportX(savedHitTestInformation.ColumnViewportIndex) + layout.GetViewportWidth(savedHitTestInformation.ColumnViewportIndex)) - MousePosition.X) - layout.GetHorizontalSplitBoxWidth(savedHitTestInformation.ColumnViewportIndex)));
                        break;
                    }
                    num2 = Math.Max((double)0.0, (double)((MousePosition.X - layout.GetViewportX(savedHitTestInformation.ColumnViewportIndex)) - layout.GetHorizontalSplitBoxWidth(savedHitTestInformation.ColumnViewportIndex)));
                    break;

                default:
                    goto Label_0258;
            }
            if (num2 > 0.0)
            {
                double num3 = (_columnSplittingTracker.X1 - layout.GetViewportX(columnViewportIndex)) - (layout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                int num4 = (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (GetViewportInfo().ColumnViewportCount - 1);
                if (!RaiseColumnViewportWidthChanging(num4, num3))
                {
                    AddColumnViewport(columnViewportIndex, num3);
                    RaiseColumnViewportWidthChanged(num4, num3);
                    ShowCell(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex(), ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                }
            }
        Label_0258:
            _columnSplittingTracker.Opacity = 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <returns></returns>
        internal int GetActiveRowViewportIndex(int sheetIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= Excel.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            var sheet = Excel.Sheets[sheetIndex];
            return GetViewportInfo(sheet).ActiveRowViewport;
        }

        /// <summary>
        /// 
        /// </summary>
        void EndRowSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            IsWorking = false;
            IsRowSplitting = false;
            IsTouchRowSplitting = false;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.Y <= layout.GetVerticalSplitBarY(rowViewportIndex))
                    {
                        num2 = layout.GetVerticalSplitBarY(rowViewportIndex) - MousePosition.Y;
                    }
                    else
                    {
                        num2 = Math.Max((double)0.0, (double)((MousePosition.Y - layout.GetVerticalSplitBarY(rowViewportIndex)) - layout.GetVerticalSplitBarHeight(rowViewportIndex)));
                    }
                    if (num2 != 0.0)
                    {
                        double deltaViewportHeight = (_rowSplittingTracker.Y1 - layout.GetVerticalSplitBarY(rowViewportIndex)) - (layout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.RowViewportIndex;
                        if (!RaiseRowViewportHeightChanging(viewportIndex, deltaViewportHeight))
                        {
                            AdjustRowViewport(rowViewportIndex, deltaViewportHeight);
                            RaiseRowViewportHeightChanged(viewportIndex, deltaViewportHeight);
                        }
                    }
                    goto Label_021D;

                case HitTestType.RowSplitBox:
                    if (RowSplitBoxAlignment != SplitBoxAlignment.Leading)
                    {
                        num2 = Math.Max((double)0.0, (double)(((layout.GetViewportY(rowViewportIndex) + layout.GetViewportHeight(rowViewportIndex)) - MousePosition.Y) - layout.GetVerticalSplitBoxHeight(rowViewportIndex)));
                        break;
                    }
                    num2 = Math.Max((double)0.0, (double)((MousePosition.Y - layout.GetViewportY(rowViewportIndex)) - layout.GetVerticalSplitBoxHeight(rowViewportIndex)));
                    break;

                default:
                    goto Label_021D;
            }
            if (num2 > 0.0)
            {
                double num3 = (_rowSplittingTracker.Y1 - layout.GetViewportY(rowViewportIndex)) - (layout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                int num4 = (RowSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (GetViewportInfo().RowViewportCount - 1);
                if (!RaiseRowViewportHeightChanging(num4, num3))
                {
                    AddRowViewport(rowViewportIndex, num3);
                    RaiseRowViewportHeightChanged(num4, num3);
                    ShowCell(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex(), ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                }
            }
        Label_021D:
            _rowSplittingTracker.Opacity = 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        void EndTabStripResizing()
        {
            IsTabStripResizing = false;
            IsTouchTabStripResizing = false;
            IsWorking = false;
        }

        internal int GetActiveColumnViewportIndex(int sheetIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= Excel.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            var sheet = Excel.Sheets[sheetIndex];
            return GetViewportInfo(sheet).ActiveColumnViewport;
        }

        internal int GetColumnPaneCount()
        {
            return GetViewportInfo().ColumnViewportCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnPaneCount"></param>
        /// <returns></returns>
        double GetColumnSplitBoxesWidth(int columnPaneCount)
        {
            if (_columnSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                if (_columnSplitBoxPolicy == SplitBoxPolicy.AsNeeded)
                {
                    if (columnPaneCount == 1)
                    {
                        return 6.0;
                    }
                    return 0.0;
                }
                if (_columnSplitBoxPolicy == SplitBoxPolicy.Never)
                {
                    return 0.0;
                }
            }
            return 6.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        Rect GetHorizontalScrollBarRectangle(int columnViewportIndex)
        {
            SheetLayout layout = GetSheetLayout();
            double horizontalScrollBarX = layout.GetHorizontalScrollBarX(columnViewportIndex);
            double ornamentY = layout.OrnamentY;
            double width = layout.GetHorizontalScrollBarWidth(columnViewportIndex) - 1.0;
            double height = layout.OrnamentHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(horizontalScrollBarX, ornamentY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        Rect GetHorizontalSplitBarRectangle(int columnViewportIndex)
        {
            SheetLayout layout = GetSheetLayout();
            double horizontalSplitBarX = layout.GetHorizontalSplitBarX(columnViewportIndex);
            double headerY = layout.HeaderY;
            double width = layout.GetHorizontalSplitBarWidth(columnViewportIndex) - 1.0;
            double height = _availableSize.Height - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(horizontalSplitBarX, headerY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        Rect GetHorizontalSplitBoxRectangle(int columnViewportIndex)
        {
            SheetLayout layout = GetSheetLayout();
            double horizontalSplitBoxX = layout.GetHorizontalSplitBoxX(columnViewportIndex);
            double ornamentY = layout.OrnamentY;
            double width = layout.GetHorizontalSplitBoxWidth(columnViewportIndex) - 1.0;
            double height = layout.OrnamentHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(horizontalSplitBoxX, ornamentY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="beforeColumn"></param>
        /// <returns></returns>
        int GetInvisibleColumnsBeforeColumn(Worksheet sheet, int beforeColumn)
        {
            int num = 0;
            using (HashSet<int>.Enumerator enumerator = _invisibleColumns.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current >= beforeColumn)
                    {
                        return num;
                    }
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="beforeRow"></param>
        /// <returns></returns>
        int GetInvisibleRowsBeforeRow(Worksheet sheet, int beforeRow)
        {
            int num = 0;
            using (HashSet<int>.Enumerator enumerator = _invisibleRows.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current >= beforeRow)
                    {
                        return num;
                    }
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal int GetRowPaneCount()
        {
            return GetViewportInfo().RowViewportCount;
        }

        double GetRowSplitBoxesHeight(int rowPaneCount)
        {
            if (_rowSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                if (_rowSplitBoxPolicy == SplitBoxPolicy.AsNeeded)
                {
                    if (rowPaneCount == 1)
                    {
                        return 6.0;
                    }
                    return 0.0;
                }
                if (_rowSplitBoxPolicy == SplitBoxPolicy.Never)
                {
                    return 0.0;
                }
            }
            return 6.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        int GetSheetInvisibleColumns(Worksheet sheet)
        {
            int num = 0;
            _invisibleColumns.Clear();
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
                if (!sheet.GetActualColumnVisible(i, SheetArea.Cells))
                {
                    _invisibleColumns.Add(i);
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        int GetSheetInvisibleRows(Worksheet sheet)
        {
            int num = 0;
            _invisibleRows.Clear();
            for (int i = 0; i < sheet.RowCount; i++)
            {
                if (!sheet.GetActualRowVisible(i, SheetArea.Cells))
                {
                    _invisibleRows.Add(i);
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        double GetSmoothingScale(double scale)
        {
            if ((scale > 0.99) && (scale < 1.01))
            {
                return 1.0;
            }
            return scale;
        }

        /// <summary>
        /// Calculates the start index to bring the tab into view. 
        /// </summary>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns></returns>
        public int GetStartIndexToBringTabIntoView(int tabIndex)
        {
            if (TabStrip != null)
            {
                return TabStrip.GetStartIndexToBringTabIntoView(tabIndex);
            }
            return Excel.StartSheetIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Rect GetTabSplitBoxRectangle()
        {
            SheetLayout layout = GetSheetLayout();
            double tabSplitBoxX = layout.TabSplitBoxX;
            double ornamentY = layout.OrnamentY;
            double width = layout.TabSplitBoxWidth - 1.0;
            double height = layout.OrnamentHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(tabSplitBoxX, ornamentY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Rect GetTabStripRectangle()
        {
            SheetLayout layout = GetSheetLayout();
            double tabStripX = layout.TabStripX;
            double tabStripY = layout.TabStripY;
            double width = layout.TabStripWidth - 1.0;
            double height = layout.TabStripHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(tabStripX, tabStripY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        Rect GetVerticalScrollBarRectangle(int rowViewportIndex)
        {
            SheetLayout layout = GetSheetLayout();
            double ornamentX = layout.OrnamentX;
            double verticalScrollBarY = layout.GetVerticalScrollBarY(rowViewportIndex);
            double width = layout.OrnamentWidth - 1.0;
            double height = layout.GetVerticalScrollBarHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(ornamentX, verticalScrollBarY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        Rect GetVerticalSplitBarRectangle(int rowViewportIndex)
        {
            SheetLayout layout = GetSheetLayout();
            double headerX = layout.HeaderX;
            double verticalSplitBarY = layout.GetVerticalSplitBarY(rowViewportIndex);
            double width = _availableSize.Width - 1.0;
            double height = layout.GetVerticalSplitBarHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(headerX, verticalSplitBarY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        Rect GetVerticalSplitBoxRectangle(int rowViewportIndex)
        {
            SheetLayout layout = GetSheetLayout();
            double ornamentX = layout.OrnamentX;
            double verticalSplitBoxY = layout.GetVerticalSplitBoxY(rowViewportIndex);
            double width = layout.OrnamentWidth - 1.0;
            double height = layout.GetVerticalSplitBoxHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(ornamentX, verticalSplitBoxY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        int GetViewportInvisibleColumns(int columnViewportIndex)
        {
            ColumnLayoutModel viewportColumnLayoutModel = GetViewportColumnLayoutModel(columnViewportIndex);
            int num = 0;
            foreach (ColumnLayout layout in viewportColumnLayoutModel)
            {
                if (!ActiveSheet.GetActualColumnVisible(layout.Column, SheetArea.Cells))
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        int GetViewportInvisibleRows(int rowViewportIndex)
        {
            RowLayoutModel viewportRowLayoutModel = GetViewportRowLayoutModel(rowViewportIndex);
            int num = 0;
            foreach (RowLayout layout in viewportRowLayoutModel)
            {
                if (!ActiveSheet.GetActualRowVisible(layout.Row, SheetArea.Cells))
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HideOpeningProgressRing()
        {
            if (_progressRing != null)
            {
                _progressRing.Visibility = (Visibility)1;
                _progressRing.IsActive = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HideOpeningStatus()
        {
            HideOpeningProgressRing();
            if (TabStrip != null)
            {
                TabStrip.Visibility = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HorizontalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
            {
                _touchToolbarPopup.IsOpen = false;
            }
            if (((ScrollBarTrackPolicy == ScrollBarTrackPolicy.Both) || (ScrollBarTrackPolicy == ScrollBarTrackPolicy.Horizontal)) || (_isTouchScrolling || (e.ScrollEventType != (ScrollEventType)5)))
            {
                for (int i = 0; i < _horizontalScrollBar.Length; i++)
                {
                    if (sender == _horizontalScrollBar[i])
                    {
                        if (HorizontalScrollable)
                        {
                            ProcessHorizontalScroll(i, e);
                            return;
                        }
                        _horizontalScrollBar[i].Value = (double)GetViewportLeftColumn(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <param name="newValue"></param>
        void HorizontalScrollBarTouchSmallDecrement(int columnViewportIndex, int newValue)
        {
            int viewportLeftColumn = GetViewportLeftColumn(columnViewportIndex);
            int num2 = TryGetPreviousScrollableColumn(newValue);
            if ((viewportLeftColumn != num2) && (num2 != -1))
            {
                SetViewportLeftColumn(columnViewportIndex, num2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void InitCachedTransform()
        {
            SheetLayout layout = GetSheetLayout();
            if (_cachedViewportTransform == null)
            {
                _cachedViewportTransform = new TransformGroup[layout.RowPaneCount + 2, layout.ColumnPaneCount + 2];
                for (int i = -1; i <= layout.ColumnPaneCount; i++)
                {
                    for (int j = -1; j <= layout.RowPaneCount; j++)
                    {
                        _cachedViewportTransform[j + 1, i + 1] = InitManipulationTransforms();
                    }
                }
            }
            if (_cachedColumnHeaderViewportTransform == null)
            {
                _cachedColumnHeaderViewportTransform = new TransformGroup[layout.ColumnPaneCount + 2];
                for (int k = -1; k <= layout.ColumnPaneCount; k++)
                {
                    _cachedColumnHeaderViewportTransform[k + 1] = InitManipulationTransforms();
                }
            }
            if (_cachedRowHeaderViewportTransform == null)
            {
                _cachedRowHeaderViewportTransform = new TransformGroup[layout.RowPaneCount + 2];
                for (int m = -1; m <= layout.RowPaneCount; m++)
                {
                    _cachedRowHeaderViewportTransform[m + 1] = InitManipulationTransforms();
                }
            }
            if (_cachedCornerViewportTransform == null)
            {
                _cachedCornerViewportTransform = InitManipulationTransforms();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void InitCachedVisual()
        {
            SheetLayout layout = GetSheetLayout();
            BitmapSource image = CreateCachedIamge(this, RenderSize);
            if (_cachedViewportVisual == null)
            {
                _cachedViewportVisual = new Image[layout.RowPaneCount + 2, layout.ColumnPaneCount + 2];
                for (int i = -1; i <= layout.ColumnPaneCount; i++)
                {
                    double viewportX = layout.GetViewportX(i);
                    double viewportWidth = layout.GetViewportWidth(i);
                    for (int j = -1; j <= layout.RowPaneCount; j++)
                    {
                        double viewportY = layout.GetViewportY(j);
                        double viewportHeight = layout.GetViewportHeight(j);
                        _cachedViewportVisual[j + 1, i + 1] = CreateCachedVisual(image, CreateClipRect(viewportX, viewportY, viewportWidth, viewportHeight, i, j));
                    }
                }
            }
            if (_cachedColumnHeaderViewportVisual == null)
            {
                _cachedColumnHeaderViewportVisual = new Image[layout.ColumnPaneCount + 2];
                for (int k = -1; k <= layout.ColumnPaneCount; k++)
                {
                    double columnX = layout.GetViewportX(k);
                    double headerY = layout.HeaderY;
                    double num10 = layout.GetViewportWidth(k);
                    double headerHeight = layout.HeaderHeight;
                    _cachedColumnHeaderViewportVisual[k + 1] = CreateCachedVisual(image, CreateClipRect(columnX, headerY, num10, headerHeight, k, -2));
                }
            }
            if (_cachedRowHeaderViewportVisual == null)
            {
                _cachedRowHeaderViewportVisual = new Image[layout.RowPaneCount + 2];
                for (int m = -1; m <= layout.RowPaneCount; m++)
                {
                    double headerX = layout.HeaderX;
                    double rowY = layout.GetViewportY(m);
                    double headerWidth = layout.HeaderWidth;
                    double num16 = layout.GetViewportHeight(m);
                    _cachedRowHeaderViewportVisual[m + 1] = CreateCachedVisual(image, CreateClipRect(headerX, rowY, headerWidth, num16, -2, m));
                }
            }
            if (_cachedCornerViewportVisual == null)
            {
                _cachedCornerViewportVisual = CreateCachedVisual(image, new Rect(layout.HeaderX, layout.HeaderY, layout.HeaderWidth, layout.HeaderHeight));
            }
            if (_cachedBottomRightACornerVisual == null)
            {
                _cachedBottomRightACornerVisual = CreateCachedVisual(image, new Rect(layout.OrnamentX, layout.OrnamentY, layout.OrnamentWidth, layout.OrnamentHeight));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TransformGroup InitManipulationTransforms()
        {
            TransformGroup group = new TransformGroup();
            group.Children.Add(new CompositeTransform());
            MatrixTransform transform = new MatrixTransform();
            transform.Matrix = Windows.UI.Xaml.Media.Matrix.Identity;
            group.Children.Add(transform);
            return group;
        }

        /// <summary>
        /// 
        /// </summary>
        void InitTouchCacheInfomation()
        {
            SheetLayout layout = GetSheetLayout();
            if (layout != null)
            {
                if (_cachedViewportHeights == null)
                {
                    _cachedViewportHeights = new double[layout.RowPaneCount + 2];
                }
                if (_cachedViewportWidths == null)
                {
                    _cachedViewportWidths = new double[layout.ColumnPaneCount + 2];
                }
                if (_cachedViewportSplitBarX == null)
                {
                    _cachedViewportSplitBarX = new double[layout.ColumnPaneCount - 1];
                }
                if (_cachedViewportSplitBarY == null)
                {
                    _cachedViewportSplitBarY = new double[layout.RowPaneCount - 1];
                }
                for (int i = -1; i <= layout.RowPaneCount; i++)
                {
                    _cachedViewportHeights[i + 1] = layout.GetViewportHeight(i);
                }
                for (int j = -1; j <= layout.ColumnPaneCount; j++)
                {
                    _cachedViewportWidths[j + 1] = layout.GetViewportWidth(j);
                }
                for (int k = 0; k < (layout.ColumnPaneCount - 1); k++)
                {
                    _cachedViewportSplitBarX[k] = layout.GetHorizontalSplitBarX(k);
                }
                for (int m = 0; m < (layout.RowPaneCount - 1); m++)
                {
                    _cachedViewportSplitBarY[m] = layout.GetVerticalSplitBarY(m);
                }
                _touchStartLeftColumn = GetViewportLeftColumn(_touchStartHitTestInfo.ColumnViewportIndex);
                _touchStartTopRow = GetViewportTopRow(_touchStartHitTestInfo.RowViewportIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void InvalidateSheetLayout()
        {
            if (!IsSuspendInvalidate())
            {
                Children.Clear();
                _cornerPanel = null;
                _rowHeaders = null;
                _colHeaders = null;
                _cellsPanels = null;
                InvalidateLayout();
                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void InvalidTabStrip()
        {
            if (!IsSuspendInvalidate())
            {
                RefreshTabStrip();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInScrollBar()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitTestType != HitTestType.HorizontalScrollBar) && (savedHitTestInformation.HitTestType != HitTestType.VerticalScrollBar))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInSplitBar()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitTestType != HitTestType.RowSplitBar) && (savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBar))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInSplitBox()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if ((savedHitTestInformation.HitTestType != HitTestType.RowSplitBox) && (savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBox))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInTabSplitBox()
        {
            return (GetHitInfo().HitTestType == HitTestType.TabSplitBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInTabStrip()
        {
            return (GetHitInfo().HitTestType == HitTestType.TabStrip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsZero(double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="scrollValue"></param>
        /// <returns></returns>
        int MapScrollValueToColumnIndex(Worksheet sheet, int scrollValue)
        {
            int num = 0;
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
                if (!_invisibleColumns.Contains(i))
                {
                    num++;
                }
                if (num == scrollValue)
                {
                    return (i + 1);
                }
            }
            return scrollValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="scrollValue"></param>
        /// <returns></returns>
        int MapScrollValueToRowIndex(Worksheet sheet, int scrollValue)
        {
            int num = 0;
            for (int i = 0; i < sheet.RowCount; i++)
            {
                if (!_invisibleRows.Contains(i))
                {
                    num++;
                }
                if (num == scrollValue)
                {
                    return (i + 1);
                }
            }
            return scrollValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool NavigationNextSheet()
        {
            SaveDataForFormulaSelection();
            if (!StopCellEditing(CanSelectFormula))
            {
                return false;
            }
            TabStrip tabStrip = TabStrip;
            if (tabStrip != null)
            {
                tabStrip.ActiveNextTab();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool NavigationPreviousSheet()
        {
            SaveDataForFormulaSelection();
            if (!StopCellEditing(CanSelectFormula))
            {
                return false;
            }
            TabStrip tabStrip = TabStrip;
            if (tabStrip != null)
            {
                tabStrip.ActivePreviousTab();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHorizontalScrollBarPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            _mouseDownPosition = e.GetCurrentPoint(this).Position;
            CloseTooltip();
        }

        void OnHorizontalScrollBarPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                InputDeviceType = InputDeviceType.Touch;
            }
            else
            {
                InputDeviceType = InputDeviceType.Mouse;
            }
            if ((ElementTreeHelper.GetParentOrSelf<Thumb>(e.OriginalSource as DependencyObject) != null) && ((Excel.ShowScrollTip == ShowScrollTip.Horizontal) || (Excel.ShowScrollTip == ShowScrollTip.Both)))
            {
                _showScrollTip = true;
                _mouseDownPosition = e.GetCurrentPoint(this).Position;
                UpdateScrollToolTip(false, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHorizontalScrollBarPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            _mouseDownPosition = e.GetCurrentPoint(this).Position;
            CloseTooltip();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabStripActiveTabChanged(object sender, EventArgs e)
        {
            TabStrip strip = sender as TabStrip;
            int sheetIndex = -1;
            if ((strip != null) && (strip.ActiveTab != null))
            {
                sheetIndex = strip.ActiveTab.SheetIndex;
            }
            if ((sheetIndex >= 0) && (sheetIndex < Excel.Sheets.Count))
            {
                StopCellEditing(false);
                if (sheetIndex != Excel.ActiveSheetIndex)
                {
                    Excel.Workbook.ActiveSheetIndex = sheetIndex;
                    RaiseActiveSheetIndexChanged();
                    _currentActiveRowIndex = Excel.ActiveSheet.ActiveRowIndex;
                    _currentActiveColumnIndex = Excel.ActiveSheet.ActiveColumnIndex;
                    Navigation.UpdateStartPosition(_currentActiveRowIndex, _currentActiveColumnIndex);
                    Invalidate();
                }
            }
            if (!IsEditing)
            {
                Excel.Focus(FocusState.Programmatic);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabStripActiveTabChanging(object sender, EventArgs e)
        {
            if ((sender is TabStrip) && (e is CancelEventArgs))
            {
                ((CancelEventArgs)e).Cancel = RaiseActiveSheetIndexChanging();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabStripNewTabNeeded(object sender, EventArgs e)
        {
            StopCellEditing(false);
            var item = new Worksheet();
            Excel.Sheets.Add(item);
            item.ReferenceStyle = Excel.Workbook.ReferenceStyle;
            if (item.ReferenceStyle == ReferenceStyle.R1C1)
            {
                item.ColumnHeader.AutoText = HeaderAutoText.Numbers;
            }
            else
            {
                item.ColumnHeader.AutoText = HeaderAutoText.Letters;
            }
            _currentActiveRowIndex = item.ActiveRowIndex;
            _currentActiveColumnIndex = item.ActiveColumnIndex;
            Navigation.UpdateStartPosition(_currentActiveRowIndex, _currentActiveColumnIndex);
            (sender as TabStrip).NewTab(Excel.Sheets.Count - 1);
            InvalidateSheetLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerticalScrollbarPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            _mouseDownPosition = e.GetCurrentPoint(this).Position;
            CloseTooltip();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerticalScrollbarPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                InputDeviceType = InputDeviceType.Touch;
            }
            else
            {
                InputDeviceType = InputDeviceType.Mouse;
            }
            if ((ElementTreeHelper.GetParentOrSelf<Thumb>(e.OriginalSource as DependencyObject) != null) && ((Excel.ShowScrollTip == ShowScrollTip.Vertical) || (Excel.ShowScrollTip == ShowScrollTip.Both)))
            {
                _showScrollTip = true;
                _mouseDownPosition = e.GetCurrentPoint(this).Position;
                UpdateScrollToolTip(true, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerticalScrollbarPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            _mouseDownPosition = e.GetCurrentPoint(this).Position;
            CloseTooltip();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <param name="e"></param>
        void ProcessHorizontalScroll(int columnViewportIndex, ScrollEventArgs e)
        {
            int viewportLeftColumn = GetViewportLeftColumn(columnViewportIndex);
            int scrollValue = (int)Math.Round(e.NewValue);
            scrollValue = MapScrollValueToColumnIndex(ActiveSheet, scrollValue);
            int num3 = scrollValue;
            if (e.ScrollEventType == (ScrollEventType)3)
            {
                if (NavigatorHelper.ScrollToNextPageOfColumns(this, columnViewportIndex))
                {
                    num3 = GetViewportLeftColumn(columnViewportIndex);
                }
                else
                {
                    num3 = TryGetNextScrollableColumn(scrollValue);
                }
            }
            else if (e.ScrollEventType == (ScrollEventType)1)
            {
                num3 = TryGetNextScrollableColumn(scrollValue);
            }
            else if (e.ScrollEventType == (ScrollEventType)2)
            {
                if (NavigatorHelper.ScrollToPreviousPageOfColumns(this, columnViewportIndex))
                {
                    num3 = GetViewportLeftColumn(columnViewportIndex);
                }
                else
                {
                    num3 = TryGetPreviousScrollableColumn(scrollValue);
                }
            }
            else if (e.ScrollEventType == 0)
            {
                num3 = TryGetPreviousScrollableColumn(scrollValue);
            }
            if ((e.ScrollEventType == (ScrollEventType)5) || (e.ScrollEventType == (ScrollEventType)8))
            {
                num3 = TryGetNextScrollableColumn(scrollValue);
            }
            if ((viewportLeftColumn != num3) && (num3 != -1))
            {
                SetViewportLeftColumn(columnViewportIndex, num3);
            }
            if (((e.ScrollEventType != (ScrollEventType)5) && (num3 != e.NewValue)) && (_horizontalScrollBar != null))
            {
                GetSheetLayout();
                if (((columnViewportIndex > -1) && (columnViewportIndex < _horizontalScrollBar.Length)) && (_horizontalScrollBar[columnViewportIndex].Value != num3))
                {
                    int invisibleColumnsBeforeColumn = GetInvisibleColumnsBeforeColumn(ActiveSheet, num3);
                    num3 -= invisibleColumnsBeforeColumn;
                    _horizontalScrollBar[columnViewportIndex].Value = (num3 != -1) ? ((double)num3) : ((double)viewportLeftColumn);
                    _horizontalScrollBar[columnViewportIndex].InvalidateArrange();
                }
            }
            if (_showScrollTip && ((Excel.ShowScrollTip == ShowScrollTip.Both) || (Excel.ShowScrollTip == ShowScrollTip.Horizontal)))
            {
                UpdateScrollToolTip(false, num3 + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hi"></param>
        void ProcessSplitBarDoubleClick(HitTestInformation hi)
        {
            if (!Excel.Workbook.Protect)
            {
                int rowViewportIndex = hi.RowViewportIndex;
                int columnViewportIndex = hi.ColumnViewportIndex;
                if (rowViewportIndex >= 0)
                {
                    double viewportHeight = GetViewportHeight(rowViewportIndex + 1);
                    if (!RaiseRowViewportHeightChanging(rowViewportIndex, viewportHeight))
                    {
                        ActiveSheet.RemoveRowViewport(rowViewportIndex);
                        RaiseRowViewportHeightChanged(rowViewportIndex, viewportHeight);
                    }
                }
                if (columnViewportIndex >= 0)
                {
                    double viewportWidth = GetViewportWidth(columnViewportIndex + 1);
                    if (!RaiseColumnViewportWidthChanging(columnViewportIndex, viewportWidth))
                    {
                        ActiveSheet.RemoveColumnViewport(columnViewportIndex);
                        RaiseColumnViewportWidthChanged(columnViewportIndex, viewportWidth);
                    }
                }
                InvalidateLayout();
                _positionInfo = null;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hi"></param>
        void ProcessSplitBarDoubleTap(HitTestInformation hi)
        {
            ProcessSplitBarDoubleClick(hi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <param name="e"></param>
        void ProcessVerticalScroll(int rowViewportIndex, ScrollEventArgs e)
        {
            int viewportTopRow = GetViewportTopRow(rowViewportIndex);
            int scrollValue = (int)Math.Round(e.NewValue);
            scrollValue = MapScrollValueToRowIndex(ActiveSheet, scrollValue);
            int beforeRow = scrollValue;
            if (e.ScrollEventType == (ScrollEventType)1)
            {
                beforeRow = TryGetNextScrollableRow(scrollValue);
            }
            else if (e.ScrollEventType == (ScrollEventType)3)
            {
                if (NavigatorHelper.ScrollToNextPageOfRows(this, rowViewportIndex))
                {
                    beforeRow = GetViewportTopRow(rowViewportIndex);
                }
                else
                {
                    beforeRow = TryGetNextScrollableRow(scrollValue);
                }
            }
            else if (e.ScrollEventType == (ScrollEventType)2)
            {
                if (NavigatorHelper.ScrollToPreviousPageOfRows(this, rowViewportIndex))
                {
                    beforeRow = GetViewportTopRow(rowViewportIndex);
                }
                else
                {
                    beforeRow = TryGetPreviousScrollableRow(scrollValue);
                }
            }
            else if (e.ScrollEventType == 0)
            {
                beforeRow = TryGetPreviousScrollableRow(scrollValue);
            }
            if ((e.ScrollEventType == (ScrollEventType)5) || (e.ScrollEventType == (ScrollEventType)8))
            {
                beforeRow = TryGetNextScrollableRow(scrollValue);
            }
            if ((viewportTopRow != beforeRow) && (beforeRow != -1))
            {
                _scrollTo = beforeRow;
                AsynSetViewportTopRow(rowViewportIndex);
            }
            if (((e.ScrollEventType != (ScrollEventType)5) && (beforeRow != e.NewValue)) && (_verticalScrollBar != null))
            {
                GetSheetLayout();
                if (((rowViewportIndex > -1) && (rowViewportIndex < _verticalScrollBar.Length)) && (beforeRow != _verticalScrollBar[rowViewportIndex].Value))
                {
                    int invisibleRowsBeforeRow = GetInvisibleRowsBeforeRow(ActiveSheet, beforeRow);
                    beforeRow -= invisibleRowsBeforeRow;
                    _verticalScrollBar[rowViewportIndex].Value = (beforeRow != -1) ? ((double)beforeRow) : ((double)viewportTopRow);
                    _verticalScrollBar[rowViewportIndex].InvalidateMeasure();
                    _verticalScrollBar[rowViewportIndex].InvalidateArrange();
                }
            }
            if (_showScrollTip && ((Excel.ShowScrollTip == ShowScrollTip.Both) || (Excel.ShowScrollTip == ShowScrollTip.Vertical)))
            {
                UpdateScrollToolTip(true, _scrollTo + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RaiseActiveSheetIndexChanged()
        {
            if ((ActiveSheetChanged != null) && (_eventSuspended == 0))
            {
                ActiveSheetChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool RaiseActiveSheetIndexChanging()
        {
            if ((ActiveSheetChanging != null) && (_eventSuspended == 0))
            {
                CancelEventArgs args = new CancelEventArgs();
                ActiveSheetChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportWidth"></param>
        internal void RaiseColumnViewportWidthChanged(int viewportIndex, double deltaViewportWidth)
        {
            if ((ColumnViewportWidthChanged != null) && (_eventSuspended == 0))
            {
                ColumnViewportWidthChanged(this, new ColumnViewportWidthChangedEventArgs(viewportIndex, deltaViewportWidth));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportWidth"></param>
        /// <returns></returns>
        internal bool RaiseColumnViewportWidthChanging(int viewportIndex, double deltaViewportWidth)
        {
            if ((ColumnViewportWidthChanging != null) && (_eventSuspended == 0))
            {
                ColumnViewportWidthChangingEventArgs args = new ColumnViewportWidthChangingEventArgs(viewportIndex, deltaViewportWidth);
                ColumnViewportWidthChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportHeight"></param>
        internal void RaiseRowViewportHeightChanged(int viewportIndex, double deltaViewportHeight)
        {
            if ((RowViewportHeightChanged != null) && (_eventSuspended == 0))
            {
                RowViewportHeightChanged(this, new RowViewportHeightChangedEventArgs(viewportIndex, deltaViewportHeight));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportHeight"></param>
        /// <returns></returns>
        internal bool RaiseRowViewportHeightChanging(int viewportIndex, double deltaViewportHeight)
        {
            if ((RowViewportHeightChanging != null) && (_eventSuspended == 0))
            {
                RowViewportHeightChangingEventArgs args = new RowViewportHeightChangingEventArgs(viewportIndex, deltaViewportHeight);
                RowViewportHeightChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RefreshTabStrip()
        {
            if (_tabStrip != null)
            {
                _tabStrip.Refresh();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        internal void RemoveColumnViewport(int columnViewportIndex)
        {
            ActiveSheet.RemoveColumnViewport(columnViewportIndex);
            InvalidateLayout();
            InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        internal void RemoveRowViewport(int rowViewportIndex)
        {
            ActiveSheet.RemoveRowViewport(rowViewportIndex);
            InvalidateLayout();
            InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SaveDataForFormulaSelection()
        {
            if (CanSelectFormula)
            {
                IsSwitchingSheet = true;
                EditorConnector.ClearFlickingItems();
                if (!EditorConnector.IsInOtherSheet)
                {
                    EditorConnector.IsInOtherSheet = true;
                    EditorConnector.SheetIndex = ActiveSheet.Workbook.ActiveSheetIndex;
                    EditorConnector.RowIndex = ActiveSheet.ActiveRowIndex;
                    EditorConnector.ColumnIndex = ActiveSheet.ActiveColumnIndex;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ShowOpeningProgressRing()
        {
            if (_progressRing != null)
            {
                _progressRing.Visibility = 0;
                _progressRing.IsActive = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ShowOpeningStatus()
        {
            ShowOpeningProgressRing();
            if (TabStrip != null)
            {
                TabStrip.Visibility = (Visibility)1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void StartColumnSplitting()
        {
            if (!Excel.Workbook.Protect)
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                SheetLayout layout = GetSheetLayout();
                if (!IsTouching)
                {
                    IsColumnSplitting = true;
                }
                else
                {
                    IsTouchColumnSplitting = true;
                }
                IsWorking = true;
                if (_columnSplittingTracker == null)
                {
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.Opacity = 0.5;
                    _columnSplittingTracker = line;
                    SplittingTrackerContainer.Children.Add(_columnSplittingTracker);
                }
                int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
                int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
                _columnSplittingTracker.Opacity = 0.5;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        _columnSplittingTracker.StrokeThickness = layout.GetHorizontalSplitBarWidth(columnViewportIndex);
                        _columnSplittingTracker.X1 = layout.GetHorizontalSplitBarX(columnViewportIndex) + (layout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0);
                        _columnSplittingTracker.Y1 = layout.Y;
                        _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
                        _columnSplittingTracker.Y2 = layout.HeaderY + _availableSize.Height;
                        return;

                    case HitTestType.RowSplitBox:
                        return;

                    case HitTestType.ColumnSplitBox:
                        _columnSplittingTracker.StrokeThickness = layout.GetHorizontalSplitBoxWidth(columnViewportIndex);
                        if (ColumnSplitBoxAlignment != SplitBoxAlignment.Leading)
                        {
                            _columnSplittingTracker.X1 = (layout.GetViewportX(columnViewportIndex) + layout.GetViewportWidth(columnViewportIndex)) - (layout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                        }
                        else
                        {
                            _columnSplittingTracker.X1 = layout.GetViewportX(columnViewportIndex) + (layout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                        }
                        _columnSplittingTracker.Y1 = layout.Y;
                        _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
                        _columnSplittingTracker.Y2 = layout.HeaderY + _availableSize.Height;
                        return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void StartRowSplitting()
        {
            if (!Excel.Workbook.Protect)
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                SheetLayout layout = GetSheetLayout();
                if (!IsTouching)
                {
                    IsRowSplitting = true;
                }
                else
                {
                    IsTouchRowSplitting = true;
                }
                IsWorking = true;
                if (_rowSplittingTracker == null)
                {
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.Opacity = 0.5;
                    _rowSplittingTracker = line;
                    SplittingTrackerContainer.Children.Add(_rowSplittingTracker);
                }
                int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
                int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
                _rowSplittingTracker.Opacity = 0.5;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        _rowSplittingTracker.StrokeThickness = layout.GetVerticalSplitBarHeight(rowViewportIndex);
                        _rowSplittingTracker.Y1 = layout.GetVerticalSplitBarY(rowViewportIndex) + (layout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0);
                        _rowSplittingTracker.X1 = layout.X;
                        _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
                        _rowSplittingTracker.X2 = layout.X + _availableSize.Width;
                        return;

                    case HitTestType.RowSplitBox:
                        _rowSplittingTracker.StrokeThickness = layout.GetVerticalSplitBoxHeight(rowViewportIndex);
                        if (RowSplitBoxAlignment != SplitBoxAlignment.Leading)
                        {
                            _rowSplittingTracker.Y1 = (layout.GetViewportY(rowViewportIndex) + layout.GetViewportHeight(rowViewportIndex)) - (layout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                        }
                        else
                        {
                            _rowSplittingTracker.Y1 = layout.GetViewportY(rowViewportIndex) + (layout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                        }
                        _rowSplittingTracker.X1 = layout.X;
                        _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
                        _rowSplittingTracker.X2 = layout.X + _availableSize.Width;
                        return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void StartTabStripResizing()
        {
            if (!IsTouching)
            {
                IsTabStripResizing = true;
            }
            else
            {
                IsTouchTabStripResizing = true;
            }
            IsWorking = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="translate"></param>
        /// <param name="scale"></param>
        void UpdateCachedImageTransform(Point currentPoint, Point translate, double scale)
        {
            SheetLayout layout = GetSheetLayout();
            if (_cachedCornerViewportTransform != null)
            {
                UpdateCachedImageTransform(_cachedCornerViewportTransform, currentPoint, translate, scale, -1, -1, layout.HeaderX, layout.HeaderY);
            }
            if (_cachedColumnHeaderViewportTransform != null)
            {
                for (int i = -1; i <= layout.ColumnPaneCount; i++)
                {
                    double viewportX = layout.GetViewportX(i);
                    double headerY = layout.HeaderY;
                    UpdateCachedImageTransform(_cachedColumnHeaderViewportTransform[i + 1], currentPoint, translate, scale, i, -1, viewportX, headerY);
                }
            }
            if (_cachedRowHeaderViewportTransform != null)
            {
                for (int j = -1; j <= layout.RowPaneCount; j++)
                {
                    double headerX = layout.HeaderX;
                    double viewportY = layout.GetViewportY(j);
                    UpdateCachedImageTransform(_cachedRowHeaderViewportTransform[j + 1], currentPoint, translate, scale, -1, j, headerX, viewportY);
                }
            }
            if (_cachedViewportTransform != null)
            {
                for (int k = -1; k <= layout.ColumnPaneCount; k++)
                {
                    double columnX = layout.GetViewportX(k);
                    for (int m = -1; m <= layout.RowPaneCount; m++)
                    {
                        double rowY = layout.GetViewportY(m);
                        UpdateCachedImageTransform(_cachedViewportTransform[m + 1, k + 1], currentPoint, translate, scale, k, m, columnX, rowY);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformGruop"></param>
        /// <param name="currentPoint"></param>
        /// <param name="translate"></param>
        /// <param name="scale"></param>
        /// <param name="columnViewportIndex"></param>
        /// <param name="rowViewportIndex"></param>
        /// <param name="columnX"></param>
        /// <param name="rowY"></param>
        void UpdateCachedImageTransform(TransformGroup transformGruop, Point currentPoint, Point translate, double scale, int columnViewportIndex, int rowViewportIndex, double columnX, double rowY)
        {
            MatrixTransform transform = null;
            CompositeTransform transform2 = null;
            double num5;
            foreach (Transform transform3 in transformGruop.Children)
            {
                if (transform3 is MatrixTransform)
                {
                    transform = transform3 as MatrixTransform;
                }
                else if (transform3 is CompositeTransform)
                {
                    transform2 = transform3 as CompositeTransform;
                }
            }
            transform.Matrix = transformGruop.Value;
            double x = currentPoint.X;
            double y = currentPoint.Y;
            double num3 = translate.X;
            double num4 = translate.Y;
            SheetLayout layout = GetSheetLayout();
            if ((columnViewportIndex < 0) || (columnViewportIndex < _touchStartHitTestInfo.ColumnViewportIndex))
            {
                x = 0.0;
                num3 = 0.0;
            }
            else if (columnViewportIndex > _touchStartHitTestInfo.ColumnViewportIndex)
            {
                x = layout.GetViewportWidth(columnViewportIndex);
            }
            if ((rowViewportIndex < 0) || (rowViewportIndex < _touchStartHitTestInfo.RowViewportIndex))
            {
                y = 0.0;
                num4 = 0.0;
            }
            else if (rowViewportIndex > _touchStartHitTestInfo.RowViewportIndex)
            {
                y = layout.GetViewportHeight(rowViewportIndex);
            }
            Point point = transform.TransformPoint(new Point(x, y));
            transform2.CenterX = point.X;
            transform2.CenterY = point.Y;
            transform2.ScaleY = num5 = scale;
            transform2.ScaleX = num5;
            transform2.TranslateX = num3;
            transform2.TranslateY = num4;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateCrossSplitBars()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_crossSplitBar != null) && (((ActiveSheet == null) || (_crossSplitBar.GetLength(0) != (layout.RowPaneCount - 1))) || (_crossSplitBar.GetLength(1) != (layout.ColumnPaneCount - 1))))
            {
                for (int i = 0; i < _crossSplitBar.GetLength(0); i++)
                {
                    for (int j = 0; j < _crossSplitBar.GetLength(1); j++)
                    {
                        Children.Remove(_crossSplitBar[i, j]);
                    }
                }
                _crossSplitBar = null;
            }
            if (((ActiveSheet != null) && (_crossSplitBar == null)) && ((layout == null) || ((layout.RowPaneCount >= 1) && (layout.ColumnPaneCount >= 1))))
            {
                _crossSplitBar = new CrossSplitBar[layout.RowPaneCount - 1, layout.ColumnPaneCount - 1];
                for (int k = 0; k < _crossSplitBar.GetLength(0); k++)
                {
                    for (int m = 0; m < _crossSplitBar.GetLength(1); m++)
                    {
                        _crossSplitBar[k, m] = new CrossSplitBar();
                        Canvas.SetZIndex(_crossSplitBar[k, m], 2);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalScrollBars()
        {
            if (ActiveSheet != null)
            {
                SheetLayout layout = GetSheetLayout();
                if ((_horizontalScrollBar != null) && ((ActiveSheet == null) || (_horizontalScrollBar.Length != layout.ColumnPaneCount)))
                {
                    for (int j = 0; j < _horizontalScrollBar.Length; j++)
                    {
                        _horizontalScrollBar[j].Scroll -= HorizontalScrollbar_Scroll;
                        _horizontalScrollBar[j].PointerPressed -= OnHorizontalScrollBarPointerPressed;
                        _horizontalScrollBar[j].PointerReleased -= OnHorizontalScrollBarPointerReleased;
                        _horizontalScrollBar[j].PointerExited -= OnHorizontalScrollBarPointerExited;
                        Children.Remove(_horizontalScrollBar[j]);
                    }
                    _horizontalScrollBar = null;
                }
                if (_horizontalScrollBar == null)
                {
                    _horizontalScrollBar = new ScrollBar[layout.ColumnPaneCount];
                    for (int k = 0; k < layout.ColumnPaneCount; k++)
                    {
                        _horizontalScrollBar[k] = new ScrollBar();
                        _horizontalScrollBar[k].Orientation = (Orientation)1;
                        _horizontalScrollBar[k].IsTabStop = false;
                        _horizontalScrollBar[k].TypeSafeSetStyle(Excel.HorizontalScrollBarStyle);
                        _horizontalScrollBar[k].Scroll += HorizontalScrollbar_Scroll;
                        _horizontalScrollBar[k].PointerPressed += OnHorizontalScrollBarPointerPressed;
                        _horizontalScrollBar[k].PointerReleased += OnHorizontalScrollBarPointerReleased;
                        _horizontalScrollBar[k].PointerExited += OnHorizontalScrollBarPointerExited;
                        Canvas.SetZIndex(_horizontalScrollBar[k], 0x62);
                    }
                }
                int sheetInvisibleColumns = GetSheetInvisibleColumns(ActiveSheet);
                for (int i = 0; i < layout.ColumnPaneCount; i++)
                {
                    double num8;
                    int viewportInvisibleColumns = GetViewportInvisibleColumns(i);
                    _horizontalScrollBar[i].Minimum = (double)ActiveSheet.FrozenColumnCount;
                    _horizontalScrollBar[i].Maximum = (double)Math.Max(ActiveSheet.FrozenColumnCount, ((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - sheetInvisibleColumns) - 1);
                    _horizontalScrollBar[i].ViewportSize = num8 = GetViewportColumnLayoutModel(i).Count - viewportInvisibleColumns;
                    _horizontalScrollBar[i].LargeChange = num8;
                    _horizontalScrollBar[i].SmallChange = 1.0;
                    int viewportLeftColumn = GetViewportLeftColumn(i);
                    viewportLeftColumn = TryGetNextScrollableColumn(viewportLeftColumn);
                    int invisibleColumnsBeforeColumn = GetInvisibleColumnsBeforeColumn(ActiveSheet, viewportLeftColumn);
                    viewportLeftColumn -= invisibleColumnsBeforeColumn;
                    _horizontalScrollBar[i].Value = (double)viewportLeftColumn;
                    _horizontalScrollBar[i].InvalidateArrange();
                    _horizontalScrollBar[i].IsEnabled = HorizontalScrollBarPolicy != 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalSplitBars()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_horizontalSplitBar != null) && ((ActiveSheet == null) || (_horizontalSplitBar.Length != (layout.ColumnPaneCount - 1))))
            {
                foreach (HorizontalSplitBar bar in _horizontalSplitBar)
                {
                    Children.Remove(bar);
                }
                _horizontalSplitBar = null;
            }
            if (((ActiveSheet != null) && (_horizontalSplitBar == null)) && (layout.ColumnPaneCount >= 1))
            {
                _horizontalSplitBar = new HorizontalSplitBar[layout.ColumnPaneCount - 1];
                for (int i = 0; i < _horizontalSplitBar.Length; i++)
                {
                    _horizontalSplitBar[i] = new HorizontalSplitBar();
                    Canvas.SetZIndex(_horizontalSplitBar[i], 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalSplitBoxes()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_horizontalSplitBox != null) && ((ActiveSheet == null) || (_horizontalSplitBox.Length != layout.ColumnPaneCount)))
            {
                for (int i = 0; i < _horizontalSplitBox.Length; i++)
                {
                    Children.Remove(_horizontalSplitBox[i]);
                }
                _horizontalSplitBox = null;
            }
            if ((ActiveSheet != null) && (_horizontalSplitBox == null))
            {
                _horizontalSplitBox = new HorizontalSplitBox[layout.ColumnPaneCount];
                for (int j = 0; j < layout.ColumnPaneCount; j++)
                {
                    _horizontalSplitBox[j] = new HorizontalSplitBox();
                    Canvas.SetZIndex(_horizontalSplitBox[j], 0x62);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScrollingIndicatorMode"></param>
        void UpdateScrollBarIndicatorMode(ScrollingIndicatorMode ScrollingIndicatorMode)
        {
            if (_horizontalScrollBar != null)
            {
                for (int i = 0; i < _horizontalScrollBar.Length; i++)
                {
                    if (_horizontalScrollBar[i] != null)
                    {
                        _horizontalScrollBar[i].IndicatorMode = ScrollingIndicatorMode;
                    }
                }
            }
            if (_verticalScrollBar != null)
            {
                for (int j = 0; j < _verticalScrollBar.Length; j++)
                {
                    if (_verticalScrollBar[j] != null)
                    {
                        _verticalScrollBar[j].IndicatorMode = ScrollingIndicatorMode;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalScrollBars()
        {
            if (ActiveSheet != null)
            {
                SheetLayout layout = GetSheetLayout();
                if ((_verticalScrollBar != null) && ((ActiveSheet == null) || (_verticalScrollBar.Length != layout.RowPaneCount)))
                {
                    for (int j = 0; j < _verticalScrollBar.Length; j++)
                    {
                        _verticalScrollBar[j].Scroll -= VerticalScrollbar_Scroll;
                        _verticalScrollBar[j].PointerPressed -= OnVerticalScrollbarPointerPressed;
                        _verticalScrollBar[j].PointerReleased -= OnVerticalScrollbarPointerReleased;
                        _verticalScrollBar[j].PointerExited -= OnVerticalScrollbarPointerExited;
                        Children.Remove(_verticalScrollBar[j]);
                    }
                    _verticalScrollBar = null;
                }
                if ((ActiveSheet != null) && (_verticalScrollBar == null))
                {
                    _verticalScrollBar = new ScrollBar[layout.RowPaneCount];
                    for (int k = 0; k < _verticalScrollBar.Length; k++)
                    {
                        _verticalScrollBar[k] = new ScrollBar();
                        _verticalScrollBar[k].IsEnabled = true;
                        _verticalScrollBar[k].Orientation = 0;
                        _verticalScrollBar[k].ViewportSize = 25.0;
                        _verticalScrollBar[k].IsTabStop = false;
                        _verticalScrollBar[k].TypeSafeSetStyle(Excel.VerticalScrollBarStyle);
                        _verticalScrollBar[k].Scroll += VerticalScrollbar_Scroll;
                        _verticalScrollBar[k].PointerPressed += OnVerticalScrollbarPointerPressed;
                        _verticalScrollBar[k].PointerReleased += OnVerticalScrollbarPointerReleased;
                        _verticalScrollBar[k].PointerExited += OnVerticalScrollbarPointerExited;
                        Canvas.SetZIndex(_verticalScrollBar[k], 0x62);
                    }
                }
                int sheetInvisibleRows = GetSheetInvisibleRows(ActiveSheet);
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    double num8;
                    int viewportInvisibleRows = GetViewportInvisibleRows(i);
                    _verticalScrollBar[i].Minimum = (double)ActiveSheet.FrozenRowCount;
                    _verticalScrollBar[i].Maximum = (double)Math.Max(ActiveSheet.FrozenRowCount, ((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - sheetInvisibleRows) - 1);
                    _verticalScrollBar[i].ViewportSize = num8 = GetViewportRowLayoutModel(i).Count - viewportInvisibleRows;
                    _verticalScrollBar[i].LargeChange = num8;
                    _verticalScrollBar[i].SmallChange = 1.0;
                    int viewportTopRow = GetViewportTopRow(i);
                    viewportTopRow = TryGetNextScrollableRow(viewportTopRow);
                    int invisibleRowsBeforeRow = GetInvisibleRowsBeforeRow(ActiveSheet, viewportTopRow);
                    viewportTopRow -= invisibleRowsBeforeRow;
                    _verticalScrollBar[i].Value = (double)viewportTopRow;
                    _verticalScrollBar[i].InvalidateArrange();
                    _verticalScrollBar[i].IsEnabled = VerticalScrollBarPolicy != 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalSplitBars()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_verticalSplitBar != null) && ((ActiveSheet == null) || (_verticalSplitBar.Length != (layout.RowPaneCount - 1))))
            {
                foreach (VerticalSplitBar bar in _verticalSplitBar)
                {
                    Children.Remove(bar);
                }
                _verticalSplitBar = null;
            }
            if (((ActiveSheet != null) && (_verticalSplitBar == null)) && (layout.RowPaneCount >= 1))
            {
                _verticalSplitBar = new VerticalSplitBar[layout.RowPaneCount - 1];
                for (int i = 0; i < _verticalSplitBar.Length; i++)
                {
                    _verticalSplitBar[i] = new VerticalSplitBar();
                    Canvas.SetZIndex(_verticalSplitBar[i], 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalSplitBoxes()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_verticalSplitBox != null) && ((ActiveSheet == null) || (_verticalSplitBox.Length != layout.RowPaneCount)))
            {
                foreach (VerticalSplitBox box in _verticalSplitBox)
                {
                    Children.Remove(box);
                }
                _verticalSplitBox = null;
            }
            if ((ActiveSheet != null) && (_verticalSplitBox == null))
            {
                _verticalSplitBox = new VerticalSplitBox[layout.RowPaneCount];
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    _verticalSplitBox[i] = new VerticalSplitBox();
                    Canvas.SetZIndex(_verticalSplitBox[i], 0x62);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateViewport()
        {
            bool flag = ((_touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && (_touchStartHitTestInfo.FloatingObjectInfo.FloatingObject != null)) && _touchStartHitTestInfo.FloatingObjectInfo.FloatingObject.IsSelected;
            if (((_touchStartHitTestInfo != null) && (_touchStartHitTestInfo.HitTestType == HitTestType.Viewport)) || ((_touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && !flag))
            {
                AdjustViewportLeftColumn();
                AdjustViewportTopRow();
                AdjustViewportSize();
                _translateOffsetY = 0.0;
                _translateOffsetX = 0.0;
                if (_updateViewportAfterTouch)
                {
                    InvalidateLayout();
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void VerticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
            {
                _touchToolbarPopup.IsOpen = false;
            }
            if (((ScrollBarTrackPolicy == ScrollBarTrackPolicy.Both) || (ScrollBarTrackPolicy == ScrollBarTrackPolicy.Vertical)) || (_isTouchScrolling || (e.ScrollEventType != (ScrollEventType)5)))
            {
                for (int i = 0; i < _verticalScrollBar.Length; i++)
                {
                    if (sender == _verticalScrollBar[i])
                    {
                        if (VerticalScrollable)
                        {
                            ProcessVerticalScroll(i, e);
                            return;
                        }
                        _verticalScrollBar[i].Value = (double)GetViewportTopRow(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <param name="newValue"></param>
        void VerticalScrollBarTouchSmallDecrement(int rowViewportIndex, int newValue)
        {
            int viewportTopRow = GetViewportTopRow(rowViewportIndex);
            int num2 = TryGetPreviousScrollableRow(newValue);
            if ((viewportTopRow != num2) && (num2 != -1))
            {
                SetViewportTopRow(rowViewportIndex, num2);
            }
        }
    }
}

