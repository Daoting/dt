#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    public partial class SheetView
    {
        internal bool RaiseValidationDragDropBlock(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, bool copy, bool insert, out bool isInValid, out string invalidMessage)
        {
            isInValid = false;
            invalidMessage = "";
            if ((ValidationDragDropBlock != null) && (_eventSuspended == 0))
            {
                ValidationDragDropBlockEventArgs args = new ValidationDragDropBlockEventArgs(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, copy, insert);
                ValidationDragDropBlock(this, args);
                if (args.Handle)
                {
                    isInValid = args.IsInvalid;
                    invalidMessage = args.InvalidMessage;
                    return true;
                }
            }
            return false;
        }

        internal void RaiseValidationError(int row, int column, ValidationErrorEventArgs eventArgs)
        {
            if ((ValidationError != null) && (_eventSuspended == 0))
            {
                ValidationError(this, eventArgs);
            }
        }

        internal bool RaiseValidationPasting(Worksheet sourceSheet, CellRange sourceRange, Worksheet worksheet, CellRange cellRange, CellRange pastingRange, bool isCutting, out bool isInvalid, out string invalidMessage)
        {
            isInvalid = false;
            invalidMessage = "";
            if ((ValidationPasting == null) || (_eventSuspended != 0))
            {
                return false;
            }
            ValidationPastingEventArgs args = new ValidationPastingEventArgs(sourceSheet, sourceRange, worksheet, cellRange, pastingRange, isCutting);
            ValidationPasting(this, args);
            if (args.Handle)
            {
                isInvalid = args.IsInvalid;
                invalidMessage = args.InvalidMessage;
            }
            return args.Handle;
        }

        internal void RefreshDataValidationInvalidCircles()
        {
            if (_viewportPresenters != null)
            {
                CellsPanel[,] viewportArray = _viewportPresenters;
                int upperBound = viewportArray.GetUpperBound(0);
                int num2 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num2; j++)
                    {
                        CellsPanel viewport = viewportArray[i, j];
                        if ((viewport != null) && (viewport.SheetArea == SheetArea.Cells))
                        {
                            viewport.RefreshDataValidationInvalidCircles();
                        }
                    }
                }
            }
        }

        internal DataValidationListButtonInfo GetDataValidationListButtonInfo(int row, int column, SheetArea sheetArea)
        {
            if (((sheetArea != SheetArea.Cells) || (ActiveSheet.ActiveColumnIndex != column)) || (ActiveSheet.ActiveRowIndex != row))
            {
                return null;
            }
            DataValidator actualDataValidator = ActiveSheet.ActiveCell.ActualDataValidator;
            if (((actualDataValidator == null) || (actualDataValidator.Type != CriteriaType.List)) || !actualDataValidator.InCellDropdown)
            {
                return null;
            }
            ViewportInfo viewportInfo = GetViewportInfo();
            List<int> list = Enumerable.ToList<int>(Enumerable.Distinct<int>(viewportInfo.LeftColumns));
            list.Add(ActiveSheet.ColumnCount);
            int num = column + 1;
            CellRange spanCell = ActiveSheet.GetSpanCell(row, column);
            if ((spanCell != null) && (spanCell.ColumnCount > 1))
            {
                num = column + spanCell.ColumnCount;
            }
            if (!list.Contains(num))
            {
                return new DataValidationListButtonInfo(actualDataValidator, row, column, SheetArea.Cells) { DisplayColumn = column + 1, ColumnViewportIndex = viewportInfo.ActiveColumnViewport, RowViewportIndex = viewportInfo.ActiveRowViewport };
            }
            return new DataValidationListButtonInfo(actualDataValidator, row, column, SheetArea.Cells) { DisplayColumn = column, ColumnViewportIndex = viewportInfo.ActiveColumnViewport, RowViewportIndex = viewportInfo.ActiveRowViewport };
        }

        internal void UpdateDataValidationUI(int row, int column)
        {
            if (_viewportPresenters != null)
            {
                CellsPanel[,] viewportArray = _viewportPresenters;
                int upperBound = viewportArray.GetUpperBound(0);
                int num2 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num2; j++)
                    {
                        CellsPanel viewport = viewportArray[i, j];
                        if ((viewport != null) && (viewport.SheetArea == SheetArea.Cells))
                        {
                            viewport.UpdateDataValidationUI(row, column);
                        }
                    }
                }
            }
        }

        void ProcessMouseDownDataValidationListButton(DataValidationListButtonInfo dataBtnInfo)
        {
            if (!RaiseDataValidationListPopupOpening(dataBtnInfo.Row, dataBtnInfo.Column) && (dataBtnInfo != null))
            {
                _dataValidationPopUpHelper = new PopupHelper(DataValidationListPopUp);
                DataValidationListBox dvListBox = new DataValidationListBox();
                dvListBox.Background = new SolidColorBrush(Colors.White);
                object[] array = dataBtnInfo.Validator.GetValidList(ActiveSheet, dataBtnInfo.Row, dataBtnInfo.Column);
                if ((dataBtnInfo.Validator.Type == CriteriaType.List) && (dataBtnInfo.Validator.Value1 != null))
                {
                    string str = (string)(dataBtnInfo.Validator.Value1 as string);
                    if (!str.StartsWith("="))
                    {
                        string listSeparator = CultureInfo.InvariantCulture.TextInfo.ListSeparator;
                        string[] strArray = new string[] { listSeparator, @"\0" };
                        string[] strArray2 = str.Split(strArray, (StringSplitOptions)StringSplitOptions.None);
                        if (strArray2 != null)
                        {
                            List<string> list = new List<string>();
                            foreach (string str3 in strArray2)
                            {
                                list.Add(str3.Trim(new char[] { ' ' }));
                            }
                            array = list.ToArray();
                        }
                    }
                }
                object obj2 = ActiveSheet.GetValue(dataBtnInfo.Row, dataBtnInfo.Column);
                if (array != null)
                {
                    int index = -1;
                    index = Array.IndexOf<object>(array, obj2);
                    float zoomFactor = ActiveSheet.ZoomFactor;
                    for (int i = 0; i < array.Length; i++)
                    {
                        object obj3 = array[i];
                        DataValidationListItem item = new DataValidationListItem
                        {
                            Value = obj3,
                            TextSize = 14f * zoomFactor
                        };
                        dvListBox.Items.Add(item);
                    }
                    dvListBox.SelectedIndex = index;
                    dvListBox.Command = new SetValueCommand(this, dataBtnInfo);
                }
                dvListBox.Popup = _dataValidationPopUpHelper;
                int row = dataBtnInfo.Row;
                int column = dataBtnInfo.Column;
                CellRange range = ActiveSheet.GetSpanCell(row, column, dataBtnInfo.SheetArea);
                if (range != null)
                {
                    row = (range.Row + range.RowCount) - 1;
                    column = (range.Column + range.ColumnCount) - 1;
                }
                RowLayout columnHeaderRowLayout = null;
                ColumnLayout layout2 = GetViewportColumnLayoutModel(dataBtnInfo.ColumnViewportIndex).Find(column);
                if (dataBtnInfo.SheetArea == SheetArea.ColumnHeader)
                {
                    columnHeaderRowLayout = GetColumnHeaderRowLayout(row);
                }
                else if (dataBtnInfo.SheetArea == SheetArea.Cells)
                {
                    columnHeaderRowLayout = GetViewportRowLayoutModel(dataBtnInfo.RowViewportIndex).Find(row);
                }
                if ((columnHeaderRowLayout != null) && (layout2 != null))
                {
                    double num6 = Math.Min(16.0, layout2.Width);
                    if (dataBtnInfo.Column == dataBtnInfo.DisplayColumn)
                    {
                        dvListBox.Width = Math.Max((double)60.0, (double)(GetDataValidationListDropdownWidth(row, dataBtnInfo.Column, dataBtnInfo.ColumnViewportIndex) + 5.0));
                        dvListBox.MaxHeight = 200.0;
                        _dataValidationPopUpHelper.ShowAsModal(this, dvListBox, new Point(layout2.X + layout2.Width, columnHeaderRowLayout.Y + columnHeaderRowLayout.Height));
                    }
                    else
                    {
                        dvListBox.Width = Math.Max((double)60.0, (double)((GetDataValidationListDropdownWidth(row, dataBtnInfo.Column, dataBtnInfo.ColumnViewportIndex) + 5.0) + 16.0));
                        dvListBox.MaxHeight = 200.0;
                        _dataValidationPopUpHelper.ShowAsModal(this, dvListBox, new Point((layout2.X + layout2.Width) + num6, columnHeaderRowLayout.Y + columnHeaderRowLayout.Height));
                    }
                }
            }
        }

        bool RaiseDataValidationListPopupOpening(int row, int column)
        {
            if (DataValidationListPopupOpening != null)
            {
                CellCancelEventArgs args = new CellCancelEventArgs(row, column);
                DataValidationListPopupOpening(this, args);
                return args.Cancel;
            }
            return false;
        }

        void _dataValidationListPopUp_Closed(object sender, object e)
        {
            FocusInternal();
        }

        void _dataValidationListPopUp_Opened(object sender, object e)
        {
        }

        double GetDataValidationListDropdownWidth(int row, int column, int columnViewportIndex)
        {
            double num = 0.0;
            CellRange range = ActiveSheet.GetSpanCell(row, column, SheetArea.Cells);
            if (range != null)
            {
                for (int i = 0; i < range.ColumnCount; i++)
                {
                    ColumnLayout layout = GetViewportColumnLayoutModel(columnViewportIndex).Find(column + i);
                    if (layout != null)
                    {
                        num += layout.Width;
                    }
                }
                return num;
            }
            ColumnLayout layout2 = GetViewportColumnLayoutModel(columnViewportIndex).Find(column);
            if (layout2 != null)
            {
                num += layout2.Width;
            }
            return num;
        }

        DataValidationListButtonInfo GetMouseDownDataValidationButton(HitTestInformation hi, bool touching = false)
        {
            DataValidationListButtonInfo info = null;
            RowLayout columnHeaderRowLayoutFromY = null;
            ColumnLayout viewportColumnLayoutFromX = null;
            SheetArea cells = SheetArea.Cells;
            if (hi.HitTestType == HitTestType.ColumnHeader)
            {
                columnHeaderRowLayoutFromY = GetColumnHeaderRowLayoutFromY(hi.HitPoint.Y);
                viewportColumnLayoutFromX = GetViewportColumnLayoutFromX(hi.ColumnViewportIndex, hi.HitPoint.X);
                cells = SheetArea.ColumnHeader;
                return null;
            }
            if (hi.HitTestType == HitTestType.Viewport)
            {
                ViewportInfo viewportInfo = GetViewportInfo();
                if ((hi.RowViewportIndex != viewportInfo.ActiveRowViewport) || (hi.ColumnViewportIndex != viewportInfo.ActiveColumnViewport))
                {
                    return null;
                }
                columnHeaderRowLayoutFromY = GetViewportRowLayoutFromY(hi.RowViewportIndex, hi.HitPoint.Y);
                viewportColumnLayoutFromX = GetViewportColumnLayoutFromX(hi.ColumnViewportIndex, hi.HitPoint.X);
                cells = SheetArea.Cells;
            }
            if ((columnHeaderRowLayoutFromY != null) && (viewportColumnLayoutFromX != null))
            {
                int row = columnHeaderRowLayoutFromY.Row;
                int column = viewportColumnLayoutFromX.Column - 1;
                while (column >= 0)
                {
                    CellRange range = ActiveSheet.GetSpanCell(row, column, cells);
                    if (range != null)
                    {
                        row = range.Row;
                        column = range.Column;
                    }
                    info = GetDataValidationListButtonInfo(row, column, cells);
                    if (info != null)
                    {
                        ColumnLayout layout3 = GetColumnLayoutModel(hi.ColumnViewportIndex, SheetArea.Cells).Find(column);
                        if ((layout3 != null) && (Math.Abs((double)(layout3.Width - 0.0)) >= 1E-06))
                        {
                            break;
                        }
                        info = null;
                        column--;
                    }
                    else
                    {
                        ColumnLayout layout4 = GetColumnLayoutModel(hi.ColumnViewportIndex, SheetArea.Cells).Find(column);
                        if ((layout4 == null) || ((Math.Abs((double)(layout4.Width - 0.0)) >= 1E-06) && (layout4.Width > 16.0)))
                        {
                            break;
                        }
                        column--;
                    }
                }
                if ((column >= 0) && (info == null))
                {
                    ColumnLayout layout5 = GetColumnLayoutModel(hi.ColumnViewportIndex, SheetArea.Cells).Find(column);
                    if (layout5 != null)
                    {
                        CellRange range2 = ActiveSheet.GetSpanCell(columnHeaderRowLayoutFromY.Row, layout5.Column - 1, cells);
                        if (range2 != null)
                        {
                            row = range2.Row;
                            column = range2.Column;
                        }
                        info = GetDataValidationListButtonInfo(row, column, cells);
                    }
                }
                if (info == null)
                {
                    row = columnHeaderRowLayoutFromY.Row;
                    column = viewportColumnLayoutFromX.Column;
                    CellRange range3 = ActiveSheet.GetSpanCell(columnHeaderRowLayoutFromY.Row, viewportColumnLayoutFromX.Column - 1, cells);
                    if (range3 != null)
                    {
                        row = range3.Row;
                        column = range3.Column;
                    }
                    info = GetDataValidationListButtonInfo(row, column, cells);
                }
                if (info != null)
                {
                    double x = hi.HitPoint.X;
                    double y = hi.HitPoint.Y;
                    double num5 = Math.Min(16.0, columnHeaderRowLayoutFromY.Height);
                    if (info.Column == info.DisplayColumn)
                    {
                        double num6 = Math.Min(16.0, viewportColumnLayoutFromX.Width);
                        if (!touching)
                        {
                            if (((x >= (((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) - num6) - 2.0)) && (x < ((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) - 2.0))) && ((y >= (((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - num5) - 2.0)) && (y < ((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - 2.0))))
                            {
                                info.RowViewportIndex = hi.RowViewportIndex;
                                info.ColumnViewportIndex = hi.ColumnViewportIndex;
                                return info;
                            }
                        }
                        else if (((x >= (((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) - num6) - 6.0)) && (x < ((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) + 4.0))) && ((y >= (((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - num5) - 6.0)) && (y < ((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) + 4.0))))
                        {
                            info.RowViewportIndex = hi.RowViewportIndex;
                            info.ColumnViewportIndex = hi.ColumnViewportIndex;
                            return info;
                        }
                    }
                    else
                    {
                        double num7 = 16.0;
                        double num8 = 0.0;
                        viewportColumnLayoutFromX = GetColumnLayoutModel(hi.ColumnViewportIndex, SheetArea.Cells).Find(info.Column);
                        if (viewportColumnLayoutFromX != null)
                        {
                            num8 += viewportColumnLayoutFromX.Width;
                        }
                        CellRange range4 = ActiveSheet.GetSpanCell(columnHeaderRowLayoutFromY.Row, info.Column, cells);
                        if ((range4 != null) && (range4.ColumnCount > 1))
                        {
                            for (int i = 1; i < range4.ColumnCount; i++)
                            {
                                viewportColumnLayoutFromX = GetColumnLayoutModel(hi.ColumnViewportIndex, SheetArea.Cells).Find(info.Column + i);
                                if (viewportColumnLayoutFromX != null)
                                {
                                    num8 += viewportColumnLayoutFromX.Width;
                                }
                            }
                        }
                        if (viewportColumnLayoutFromX != null)
                        {
                            if (touching)
                            {
                                if (((x >= ((viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width) - 4.0)) && (x < (((viewportColumnLayoutFromX.X + num8) + num7) + 4.0))) && ((y >= (((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - num5) - 6.0)) && (y < ((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) + 4.0))))
                                {
                                    info.RowViewportIndex = hi.RowViewportIndex;
                                    info.ColumnViewportIndex = hi.ColumnViewportIndex;
                                    return info;
                                }
                            }
                            else if (((x >= (viewportColumnLayoutFromX.X + viewportColumnLayoutFromX.Width)) && (x < ((viewportColumnLayoutFromX.X + num8) + num7))) && ((y >= (((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - num5) - 2.0)) && (y < ((columnHeaderRowLayoutFromY.Y + columnHeaderRowLayoutFromY.Height) - 2.0))))
                            {
                                info.RowViewportIndex = hi.RowViewportIndex;
                                info.ColumnViewportIndex = hi.ColumnViewportIndex;
                                return info;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}

