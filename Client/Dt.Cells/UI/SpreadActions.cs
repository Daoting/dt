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
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the actions available for input maps.
    /// </summary>
    public static class SpreadActions
    {
        /// <summary>
        /// Stops cell editing and cancels input.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void CancelInput(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.IsEditing)
            {
                view.StopCellEditing(true);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Clears the cell value.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void Clear(object sender, ActionEventArgs e)
        {
            Excel parameter = sender as Excel;
            if ((parameter != null) && !parameter.IsEditing)
            {
                var sheet = parameter.ActiveSheet;
                foreach (CellRange range in sheet.Selections)
                {
                    if (sheet.Protect && Excel.IsAnyCellInRangeLocked(sheet, range.Row, range.Column, range.RowCount, range.ColumnCount))
                    {
                        return;
                    }
                }
                List<CellRange> list = new List<CellRange>((IEnumerable<CellRange>) sheet.Selections);
                ClearValueUndoAction command = new ClearValueUndoAction(sheet, list.ToArray());
                if (command.CanExecute(parameter))
                {
                    parameter.DoCommand(command);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Clears the active cell value and enters edit mode.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void ClearAndEditing(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && !view.IsEditing)
            {
                view.StartCellEditing(false, "");
                e.Handled = true;
            }
        }

        /// <summary>
        /// Copies the floating objects to the Clipboard.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void ClipboardCopyFloatingObjects(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                var allSelectedFloatingObjects = view.GetAllSelectedFloatingObjects();
                if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Count > 0))
                {
                    List<FloatingObject> list = new List<FloatingObject>();
                    foreach (FloatingObject obj2 in allSelectedFloatingObjects)
                    {
                        list.Add(obj2.Clone() as FloatingObject);
                    }
                    SpreadXClipboard.FloatingObjects = list.ToArray();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Cuts the floating objects to the Clipboard.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void ClipboardCutFloatingObjects(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                var allSelectedFloatingObjects = view.GetAllSelectedFloatingObjects();
                if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Count > 0))
                {
                    List<FloatingObject> list = new List<FloatingObject>();
                    foreach (FloatingObject obj2 in allSelectedFloatingObjects)
                    {
                        list.Add(obj2.Clone() as FloatingObject);
                    }
                    SpreadXClipboard.FloatingObjects = list.ToArray();
                    DeleteFloatingObject(sender, e);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Pastes the floating objects from the Clipboard.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void ClipboardPasteFloatingObjects(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                FloatingObject[] floatingObjects = SpreadXClipboard.FloatingObjects;
                if ((floatingObjects != null) && (floatingObjects.Length > 0))
                {
                    view.DoCommand(new ClipboardPasteFloatingObjectUndoAction(view.ActiveSheet, floatingObjects));
                    SpreadXClipboard.Worksheet = null;
                    SpreadXClipboard.FloatingObjects = null;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Stops cell editing and moves the active cell to the next row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance contains the action event data.</param>
        public static void CommitInputNavigationDown(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                bool flag = true;
                if (view.IsEditing)
                {
                    flag = view.StopCellEditing(false);
                }
                if (flag)
                {
                    view.Navigation.ProcessNavigation((NavigationDirection)3);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Stops cell editing and moves the active cell to the next cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void CommitInputNavigationTabNext(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                bool flag = true;
                if (view.IsEditing)
                {
                    flag = view.StopCellEditing(false);
                }
                if (flag)
                {
                    if (view.Navigation.ShouldNavInSelection())
                    {
                        view.Navigation.ProcessNavigation((NavigationDirection)7);
                    }
                    else
                    {
                        view.Navigation.ProcessNavigation((NavigationDirection)5);
                    }
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Stops cell editing and moves the active cell to the previous cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void CommitInputNavigationTabPrevious(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                bool flag = true;
                if (view.IsEditing)
                {
                    flag = view.StopCellEditing(false);
                }
                if (flag)
                {
                    if (view.Navigation.ShouldNavInSelection())
                    {
                        view.Navigation.ProcessNavigation((NavigationDirection)6);
                    }
                    else
                    {
                        view.Navigation.ProcessNavigation((NavigationDirection)4);
                    }
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Stops cell editing and moves the active cell to the previous row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void CommitInputNavigationUp(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                bool flag = true;
                if (view.IsEditing)
                {
                    flag = view.StopCellEditing(false);
                }
                if (flag)
                {
                    view.Navigation.ProcessNavigation((NavigationDirection)2);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Copies the selected item text to the Clipboard.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void Copy(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.AutoClipboard)
            {
                if (view.IsEditing)
                {
                    ClipboardHelper.ClearSpreadXClipboard();
                }
                else
                {
                    CellRange spanCell = null;
                    if (view.ActiveSheet.Selections.Count > 1)
                    {
                        view.RaiseInvalidOperation(ResourceStrings.spreadActionCopyMultiplySelection, null, null);
                    }
                    else
                    {
                        if (view.ActiveSheet.Selections.Count == 1)
                        {
                            spanCell = view.ActiveSheet.Selections[0];
                        }
                        else
                        {
                            spanCell = view.ActiveSheet.GetSpanCell(view.ActiveSheet.ActiveRowIndex, view.ActiveSheet.ActiveColumnIndex);
                            if (spanCell == null)
                            {
                                spanCell = new CellRange(view.ActiveSheet.ActiveRowIndex, view.ActiveSheet.ActiveColumnIndex, 1, 1);
                            }
                        }
                        view.RaiseClipboardChanging();
                        view.ClipboardCopy(spanCell);
                        view.RaiseClipboardChanged();
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Cuts the selected item text to the Clipboard.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void Cut(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((((view != null) && view.AutoClipboard) && (view.ActiveSheet.Selections.Count == 1)) && !view.IsEditing)
            {
                CellRange spanCell = null;
                if (view.ActiveSheet.Selections.Count > 1)
                {
                    view.RaiseInvalidOperation(ResourceStrings.spreadActionCutMultipleSelections, null, null);
                }
                else
                {
                    if (view.ActiveSheet.Selections.Count > 0)
                    {
                        spanCell = view.ActiveSheet.Selections[0];
                    }
                    else
                    {
                        spanCell = view.ActiveSheet.GetSpanCell(view.ActiveSheet.ActiveRowIndex, view.ActiveSheet.ActiveColumnIndex);
                        if (spanCell == null)
                        {
                            spanCell = new CellRange(view.ActiveSheet.ActiveRowIndex, view.ActiveSheet.ActiveColumnIndex, 1, 1);
                        }
                    }
                    view.RaiseClipboardChanging();
                    view.ClipboardCut(spanCell);
                    view.RaiseClipboardChanged();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Deletes the floating object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void DeleteFloatingObject(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<string> list = new List<string>();
                foreach (FloatingObject obj2 in view.GetAllSelectedFloatingObjects())
                {
                    list.Add(obj2.Name);
                }
                FloatingObjectExtent extent = new FloatingObjectExtent(list.ToArray());
                DeleteFloatingObjectUndoAction command = new DeleteFloatingObjectUndoAction(view.ActiveSheet, extent);
                view.DoCommand(command);
                e.Handled = true;
            }
        }

        static void ExecutePaste(ActionEventArgs e, Excel gcSheetView, string clipboardText)
        {
            CellRange range2;
            var sheet = SpreadXClipboard.Worksheet;
            CellRange fromRange = SpreadXClipboard.Range;
            bool isCutting = SpreadXClipboard.IsCutting;
            if (((isCutting && (sheet != null)) && ((fromRange != null) && sheet.Protect)) && Excel.IsAnyCellInRangeLocked(sheet, fromRange.Row, fromRange.Column, fromRange.RowCount, fromRange.ColumnCount))
            {
                isCutting = false;
            }
            bool pasteInternal = false;
            List<CellRange> list = new List<CellRange>();
            if (gcSheetView.ActiveSheet.Selections.Count > 1)
            {
                foreach (CellRange range3 in gcSheetView.ActiveSheet.Selections)
                {
                    if (!gcSheetView.CheckPastedRange(sheet, fromRange, range3, isCutting, clipboardText, out range2, out pasteInternal))
                    {
                        return;
                    }
                    if (range3.Contains(range2) && !range3.Equals(range2))
                    {
                        gcSheetView.RaiseInvalidOperation(ResourceStrings.spreadActionPasteSizeDifferent, null, null);
                        return;
                    }
                    list.Add(range2);
                }
            }
            else if (gcSheetView.ActiveSheet.Selections.Count <= 0)
            {
                CellRange spanCell = gcSheetView.ActiveSheet.GetSpanCell(gcSheetView.ActiveSheet.ActiveRowIndex, gcSheetView.ActiveSheet.ActiveColumnIndex);
                if (spanCell == null)
                {
                    spanCell = new CellRange(gcSheetView.ActiveSheet.ActiveRowIndex, gcSheetView.ActiveSheet.ActiveColumnIndex, 1, 1);
                }
                if (!gcSheetView.CheckPastedRange(sheet, fromRange, spanCell, isCutting, clipboardText, out range2, out pasteInternal))
                {
                    return;
                }
                list.Add(range2);
            }
            else
            {
                CellRange toRange = gcSheetView.ActiveSheet.Selections[0];
                if (gcSheetView.CheckPastedRange(sheet, fromRange, toRange, isCutting, clipboardText, out range2, out pasteInternal))
                {
                    list.Add(range2);
                }
                else
                {
                    return;
                }
            }
            if (list.Count > 0)
            {
                if (!pasteInternal)
                {
                    sheet = null;
                    fromRange = null;
                }
                ClipboardPasteOptions clipBoardOptions = gcSheetView.ClipBoardOptions;
                if (isCutting)
                {
                    clipBoardOptions = ClipboardPasteOptions.All;
                }
                ClipboardPasteExtent pasteExtent = new ClipboardPasteExtent(fromRange, list.ToArray(), isCutting, clipboardText);
                ClipboardPasteUndoAction command = new ClipboardPasteUndoAction(sheet, gcSheetView.ActiveSheet, pasteExtent, clipBoardOptions);
                gcSheetView.DoCommand(command);
                e.Handled = true;
            }
        }

        static string[] GetSelectedFloatingObjectNames(Excel gcSheetView)
        {
            List<string> list = new List<string>();
            foreach (FloatingObject obj2 in gcSheetView.GetAllSelectedFloatingObjects())
            {
                list.Add(obj2.Name);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Sets the array formula on the current active range.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void InputArrayFormula(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && (view.EditingViewport != null))
            {
                string editorValue = (string) (view.EditingViewport.GetEditorValue() as string);
                SetArrayFormulaUndoAction command = new SetArrayFormulaUndoAction(view.ActiveSheet, editorValue);
                view.DoCommand(command);
            }
        }

        /// <summary>
        /// Inputs a new line in the current active cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void InputNewLine(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.IsEditing)
            {
                view.ProcessTextInput("\r\n", false, true);
                view.ActiveSheet.ActiveCell.WordWrap = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MoveFloatingObjectDown(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<string> list = new List<string>();
                foreach (FloatingObject obj2 in view.GetAllSelectedFloatingObjects())
                {
                    list.Add(obj2.Name);
                }
                MoveFloatingObjectExtent extent = new MoveFloatingObjectExtent(list.ToArray(), 0.0, 1.0);
                MoveFloatingObjectUndoAction command = new MoveFloatingObjectUndoAction(view.ActiveSheet, extent);
                view.DoCommand(command);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MoveFloatingObjectLeft(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<string> list = new List<string>();
                foreach (FloatingObject obj2 in view.GetAllSelectedFloatingObjects())
                {
                    list.Add(obj2.Name);
                }
                MoveFloatingObjectExtent extent = new MoveFloatingObjectExtent(list.ToArray(), -1.0, 0.0);
                MoveFloatingObjectUndoAction command = new MoveFloatingObjectUndoAction(view.ActiveSheet, extent);
                view.DoCommand(command);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MoveFloatingObjectRight(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<string> list = new List<string>();
                foreach (FloatingObject obj2 in view.GetAllSelectedFloatingObjects())
                {
                    list.Add(obj2.Name);
                }
                MoveFloatingObjectExtent extent = new MoveFloatingObjectExtent(list.ToArray(), 1.0, 0.0);
                MoveFloatingObjectUndoAction command = new MoveFloatingObjectUndoAction(view.ActiveSheet, extent);
                view.DoCommand(command);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void MoveFloatingObjectTop(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<string> list = new List<string>();
                foreach (FloatingObject obj2 in view.GetAllSelectedFloatingObjects())
                {
                    list.Add(obj2.Name);
                }
                MoveFloatingObjectExtent extent = new MoveFloatingObjectExtent(list.ToArray(), 0.0, -1.0);
                MoveFloatingObjectUndoAction command = new MoveFloatingObjectUndoAction(view.ActiveSheet, extent);
                view.DoCommand(command);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the last row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationBottom(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)15);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the next row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationDown(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)3);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the last column.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationEnd(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)13);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the first cell in the sheetview.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationFirst(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)0x10);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the first column.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationHome(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)12);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the last cell in the sheetview.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationLast(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)0x11);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the previous column.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationLeft(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation(0);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the next cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationNext(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && !view.IsEditing)
            {
                view.Navigation.ProcessNavigation((NavigationDirection)5);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Navigates to the next floating object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void NavigationNextFloatingObject(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<FloatingObject> list = view.GetAllFloatingObjects();
                List<FloatingObject> list2 = view.GetAllSelectedFloatingObjects();
                FloatingObject obj2 = list2[list2.Count - 1];
                view.UnSelectedAllFloatingObjects();
                int num2 = list.IndexOf(obj2) + 1;
                if (num2 > (list.Count - 1))
                {
                    num2 = 0;
                }
                list[num2].IsSelected = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active sheet to the next sheet.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationNextSheet(object sender, ActionEventArgs e)
        {
            var view = sender as Excel;
            if ((view != null) && view.NavigationNextSheet())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell down one page of rows.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationPageDown(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)9);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell up one page of rows.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationPageUp(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)8);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the previous cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationPrevious(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && !view.IsEditing)
            {
                view.Navigation.ProcessNavigation((NavigationDirection)4);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Navigates to the previous floating object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void NavigationPreviousFloatingObject(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<FloatingObject> list = view.GetAllFloatingObjects();
                List<FloatingObject> list2 = view.GetAllSelectedFloatingObjects();
                FloatingObject obj2 = list2[list2.Count - 1];
                view.UnSelectedAllFloatingObjects();
                int num2 = list.IndexOf(obj2) - 1;
                if (num2 < 0)
                {
                    num2 = list.Count - 1;
                }
                list[num2].IsSelected = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active sheet to the previous sheet.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationPreviousSheet(object sender, ActionEventArgs e)
        {
            var view = sender as Excel;
            if ((view != null) && view.NavigationPreviousSheet())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the next column.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationRight(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)1);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the first row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationTop(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)14);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Moves the active cell to the previous row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void NavigationUp(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Navigation.ProcessNavigation((NavigationDirection)2);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Pastes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance containing the event data.</param>
        public static void Paste(object sender, ActionEventArgs e)
        {
            AsyncOperationCompletedHandler<string> handler = null;
            Excel gcSheetView = sender as Excel;
            if ((((gcSheetView != null) && gcSheetView.AutoClipboard) && (gcSheetView.ActiveSheet != null)) && !gcSheetView.IsEditing)
            {
                DataPackageView content = Clipboard.GetContent();
                if ((content != null) && content.Contains(StandardDataFormats.Text))
                {
                    IAsyncOperation<string> textAsync = content.GetTextAsync();
                    if (handler == null)
                    {
                        handler = delegate (IAsyncOperation<string> asyncInfo, AsyncStatus asyncStatus) {
                            if (asyncStatus == AsyncStatus.Completed)
                            {
                                string clipboardText = asyncInfo.GetResults();
                                ExecutePaste(e, gcSheetView, clipboardText);
                            }
                        };
                    }
                    textAsync.Completed = (AsyncOperationCompletedHandler<string>) Delegate.Combine((Delegate) textAsync.Completed, (Delegate) handler);
                }
            }
        }

        /// <summary>
        /// Performs a redo of the most recently undone edit or action.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void Redo(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (((view != null) && !view.IsEditing) && view.UndoManager.CanRedo)
            {
                view.UndoManager.Redo();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Selects all objects.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void SelectionAll(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                List<FloatingObject> list = new List<FloatingObject>(view.GetAllFloatingObjects());
                foreach (FloatingObject obj2 in list)
                {
                    if (!obj2.IsSelected)
                    {
                        obj2.IsSelected = true;
                    }
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection to the last row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionBottom(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Bottom);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection down one row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionDown(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Down);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection to the last column.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionEnd(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.End);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection to the first cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionFirst(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.First);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection to the first column.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionHome(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Home);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection to the last cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionLast(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Last);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection one column to the left.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionLeft(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Left);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection down to include one page of rows.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionPageDown(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.PageDown);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection up to include one page of rows.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionPageUp(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.PageUp);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection one column to the right.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionRight(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Right);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection to the first row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionTop(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Top);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extends the selection up one row.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void SelectionUp(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (view != null)
            {
                if (view.IsEditing && (!view.CanCommitAndNavigate() || !view.StopCellEditing(false)))
                {
                    return;
                }
                view.Selection.KeyboardSelect(NavigationDirection.Up);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Starts to edit the current active cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void StartEditing(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && !view.IsEditing)
            {
                view.StartCellEditing(false, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Starts to edit formula on the current active cell.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void StartEditingFormula(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (((view != null) && view.CanUserEditFormula) && (!view.IsEditing && !string.IsNullOrEmpty(view.ActiveSheet.GetFormula(view.ActiveSheet.ActiveRowIndex, view.ActiveSheet.ActiveColumnIndex))))
            {
                view.StartCellEditing(false, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Performs an undo of the most recent edit or action.
        /// </summary>
        /// <param name="sender">The object to do the action on.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
        public static void Undo(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if (((view != null) && !view.IsEditing) && view.UndoManager.CanUndo)
            {
                view.UndoManager.Undo();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Unselects all floating objects.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the event data.</param>
        public static void UnSelectAllFloatingObjects(object sender, ActionEventArgs e)
        {
            Excel view = sender as Excel;
            if ((view != null) && view.HasSelectedFloatingObject())
            {
                view.UnSelectedAllFloatingObjects();
                e.Handled = true;
            }
        }
    }
}

