#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.Generic;
using System.Xml;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a section that is used for display of the <see cref="T:Dt.Cells.Data.Worksheet" />.
    /// </summary>
    internal partial class GcSheetSection : GcMultiplePageSection, IGcAllowAppendixSection
    {
        /// <summary>
        /// Creates the state of the build in control.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal override object CreateBuildInControlState(GcReportContext context)
        {
            if (this.worksheet == null)
            {
                return null;
            }

            Worksheet sheet = this.worksheet;
            PrintInfo settings = sheet.PrintInfo ?? new PrintInfo();
            SheetState state = new SheetState(settings);
            StorageType type = StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data;
            state.RowStartIndex = (settings.RowStart != -1) ? Math.Max(settings.RowStart, 0) : 0;
            state.RowEndIndex = (settings.RowEnd != -1) ? Math.Min(settings.RowEnd, sheet.RowCount - 1) : (sheet.RowCount - 1);
            if (settings.UseMax && (settings.RowEnd == -1))
            {
                state.RowEndIndex = Math.Max(state.RowStartIndex, sheet.GetLastDirtyRow(type));
            }
            state.ColumnStartIndex = (settings.ColumnStart != -1) ? Math.Max(settings.ColumnStart, 0) : 0;
            state.ColumnEndIndex = (settings.ColumnEnd != -1) ? Math.Min(settings.ColumnEnd, sheet.ColumnCount - 1) : (sheet.ColumnCount - 1);
            if (settings.UseMax && (settings.ColumnEnd == -1))
            {
                state.ColumnEndIndex = Math.Max(state.ColumnStartIndex, sheet.GetLastDirtyColumn(type));
            }

            // hdt 浮动图片的区域，区域输出时不扩展范围
            if (sheet.Pictures.Count > 0
                && sheet.PrintInfo.RowStart == -1
                && sheet.PrintInfo.ColumnStart == -1)
            {
                // 图片位置超出格范围时无法绘制！！！
                // 因图片浮动，计算绘制范围时未计算
                // 若动态添加行列，会造成图片大小变化
                foreach (var p in sheet.Pictures)
                {
                    if (p.EndRow > state.RowEndIndex
                        && p.EndRow < sheet.RowCount)
                    {
                        state.RowEndIndex = p.EndRow;
                    }
                    if (p.EndColumn > state.ColumnEndIndex
                        && p.EndColumn < sheet.ColumnCount)
                    {
                        state.ColumnEndIndex = p.EndColumn;
                    }
                }
            }
            
            state.RepeatRowStartIndex = (settings.RepeatRowStart != -1) ? Math.Max(0, settings.RepeatRowStart) : settings.RepeatRowStart;
            state.RepeatRowEndIndex = Math.Min(settings.RepeatRowEnd, sheet.RowCount - 1);
            state.RepeatColumnStartIndex = (settings.RepeatColumnStart != -1) ? Math.Max(0, settings.RepeatColumnStart) : settings.RepeatColumnStart;
            state.RepeatColumnEndIndex = Math.Min(settings.RepeatColumnEnd, sheet.ColumnCount - 1);
            if ((state.HasRepeatRow && (state.RepeatRowStartIndex < state.RowStartIndex)) && (state.RepeatRowEndIndex >= state.RowStartIndex))
            {
                if (state.RepeatRowEndIndex < state.RowEndIndex)
                {
                    state.RowStartIndex = state.RepeatRowEndIndex + 1;
                }
                else
                {
                    state.RowStartIndex = state.RepeatRowStartIndex;
                    state.RepeatRowStartIndex = -1;
                }
            }
            if ((state.HasRepeatColumn && (state.RepeatColumnStartIndex < state.ColumnStartIndex)) && (state.RepeatColumnEndIndex >= state.ColumnStartIndex))
            {
                if (state.RepeatColumnEndIndex < state.ColumnEndIndex)
                {
                    state.ColumnStartIndex = state.RepeatColumnEndIndex + 1;
                }
                else
                {
                    state.ColumnStartIndex = state.RepeatColumnStartIndex;
                    state.RepeatColumnStartIndex = -1;
                }
            }

            if (state.RowEndIndex < state.RowStartIndex)
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[])new object[] { "Row" }));
            }
            if (state.ColumnEndIndex < state.ColumnStartIndex)
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[])new object[] { "Column" }));
            }
            if (((state.RepeatRowEndIndex != -1) && (state.RepeatRowStartIndex != -1)) && (state.RepeatRowEndIndex < state.RepeatRowStartIndex))
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionRepeatEndIndexError, (object[])new object[] { "row" }));
            }
            if (((state.RepeatColumnEndIndex != -1) && (state.RepeatColumnStartIndex != -1)) && (state.RepeatColumnEndIndex < state.RepeatColumnStartIndex))
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionRepeatEndIndexError, (object[])new object[] { "column" }));
            }

            state.ColumnWidths = new PartLayoutData(GetColumnWidths(sheet, SheetArea.Cells, state.ColumnStartIndex, state.ColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, sheet, settings), false, settings.ShowGridLine, sheet.BorderCollapse), state.ColumnStartIndex, sheet.ColumnCount);
            if (ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, sheet, settings))
            {
                state.RowHeaderWidths = new PartLayoutData(GetColumnWidths(sheet, SheetArea.CornerHeader | SheetArea.RowHeader, 0, sheet.RowHeaderColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, sheet, settings), false, settings.ShowGridLine, sheet.BorderCollapse));
            }
            if ((sheet.FrozenColumnCount > 0) && (state.ColumnEndIndex >= sheet.FrozenColumnCount))
            {
                if (state.ColumnStartIndex < sheet.FrozenColumnCount)
                {
                    state.ColumnStartIndex = sheet.FrozenColumnCount;
                }
                state.ColumnWidths.AppendSizes(GetColumnWidths(sheet, SheetArea.Cells, 0, sheet.FrozenColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, sheet, settings), false, settings.ShowGridLine, sheet.BorderCollapse), 0);
                state.FrozenColumnWidths = new PartLayoutData(state.ColumnWidths.GetSizes(0, sheet.FrozenColumnCount));
            }
            if (state.HasRepeatColumn)
            {
                state.ColumnWidths.AppendSizes(GetColumnWidths(sheet, SheetArea.Cells, state.RepeatColumnStartIndex, state.RepeatColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, sheet, settings), false, settings.ShowGridLine, sheet.BorderCollapse), state.RepeatColumnStartIndex);
                state.RepeatColumnWidths = new PartLayoutData(state.ColumnWidths.GetSizes(state.RepeatColumnStartIndex, state.RepeatColumnCount));
            }
            if ((sheet.FrozenTrailingColumnCount > 0) && (state.ColumnStartIndex < ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1)))
            {
                if (state.ColumnEndIndex > ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1))
                {
                    state.ColumnEndIndex = (sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1;
                }
                state.ColumnWidths.AppendSizes(GetColumnWidths(sheet, SheetArea.Cells, sheet.ColumnCount - sheet.FrozenTrailingColumnCount, sheet.FrozenTrailingColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, sheet, settings), false, settings.ShowGridLine, sheet.BorderCollapse), sheet.ColumnCount - sheet.FrozenTrailingColumnCount);
                state.FrozenTrailingColumnWidths = new PartLayoutData(state.ColumnWidths.GetSizes(sheet.ColumnCount - sheet.FrozenTrailingColumnCount, sheet.FrozenTrailingColumnCount));
            }

            state.RowHeights = new PartLayoutData(GetRowHeights(sheet, SheetArea.Cells, state.RowStartIndex, state.RowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, sheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, sheet.BorderCollapse), state.RowStartIndex, sheet.RowCount);
            if (ShowPart(SheetArea.ColumnHeader, sheet, settings))
            {
                state.ColumnHeaderHeights = new PartLayoutData(GetRowHeights(sheet, SheetArea.ColumnHeader, 0, sheet.ColumnHeaderRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, sheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, sheet.BorderCollapse));
            }
            if ((sheet.FrozenRowCount > 0) && (state.RowEndIndex >= sheet.FrozenRowCount))
            {
                if (state.RowStartIndex < sheet.FrozenRowCount)
                {
                    state.RowStartIndex = sheet.FrozenRowCount;
                }
                state.RowHeights.AppendSizes(GetRowHeights(sheet, SheetArea.Cells, 0, sheet.FrozenRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, sheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, sheet.BorderCollapse), 0);
                state.FrozenRowHeights = new PartLayoutData(state.RowHeights.GetSizes(0, sheet.FrozenRowCount));
            }
            if (state.HasRepeatRow)
            {
                state.RowHeights.AppendSizes(GetRowHeights(sheet, SheetArea.Cells, state.RepeatRowStartIndex, state.RepeatRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, sheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, sheet.BorderCollapse), state.RepeatRowStartIndex);
                state.RepeatRowHeights = new PartLayoutData(state.RowHeights.GetSizes(state.RepeatRowStartIndex, state.RepeatRowCount));
            }
            if ((sheet.FrozenTrailingRowCount > 0) && (state.RowStartIndex < ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1)))
            {
                if (state.RowEndIndex > ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1))
                {
                    state.RowEndIndex = (sheet.RowCount - sheet.FrozenTrailingRowCount) - 1;
                }
                state.RowHeights.AppendSizes(GetRowHeights(sheet, SheetArea.Cells, sheet.RowCount - sheet.FrozenTrailingRowCount, sheet.FrozenTrailingRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, sheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, sheet.BorderCollapse), sheet.RowCount - sheet.FrozenTrailingRowCount);
                state.FrozenTrailingRowHeights = new PartLayoutData(state.RowHeights.GetSizes(sheet.RowCount - sheet.FrozenTrailingRowCount, sheet.FrozenTrailingRowCount));
            }

            if (state.RowEndIndex < state.RowStartIndex)
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[])new object[] { "Row" }));
            }
            if (state.ColumnEndIndex < state.ColumnStartIndex)
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[])new object[] { "Column" }));
            }

            if (state.HasRepeatRow)
            {
                state.RepeatRowHeights.OffsetIndex = state.RepeatRowStartIndex;
            }
            if (state.HasRepeatColumn)
            {
                state.RepeatColumnWidths.OffsetIndex = state.RepeatColumnStartIndex;
            }
            if (state.HasFrozenTrailingRow)
            {
                state.FrozenTrailingRowHeights.OffsetIndex = sheet.RowCount - sheet.FrozenTrailingRowCount;
            }
            if (state.HasFrozenTrailingColumn)
            {
                state.FrozenTrailingColumnWidths.OffsetIndex = sheet.ColumnCount - sheet.FrozenTrailingColumnCount;
            }
            int rowStartIndex = state.HasFrozenRow ? 0 : state.RowStartIndex;
            int rowEndIndex = state.HasFrozenTrailingRow ? (sheet.RowCount - 1) : state.RowEndIndex;
            int columnStartIndex = state.HasFrozenColumn ? 0 : state.ColumnStartIndex;
            int columnEndIndex = state.HasFrozenTrailingColumn ? (sheet.ColumnCount - 1) : state.ColumnEndIndex;
            if (state.HasRepeatRow)
            {
                rowStartIndex = Math.Min(rowStartIndex, state.RepeatRowStartIndex);
                rowEndIndex = Math.Max(rowEndIndex, state.RepeatRowEndIndex);
            }
            if (state.HasRepeatColumn)
            {
                columnStartIndex = Math.Min(columnStartIndex, state.RepeatColumnStartIndex);
                columnEndIndex = Math.Max(columnEndIndex, state.RepeatColumnEndIndex);
            }
            state.CellSpans = Utilities.GetSpanLayoutData(sheet, SheetArea.Cells, rowStartIndex, rowEndIndex, columnStartIndex, columnEndIndex, state.ColumnWidths, context, false, !settings.BestFitColumns, state);
            if (state.HasColumnHeader)
            {
                state.ColumnHeaderSpans = Utilities.GetSpanLayoutData(sheet, SheetArea.ColumnHeader, 0, sheet.ColumnHeaderRowCount - 1, columnStartIndex, columnEndIndex, state.ColumnWidths, context, false, !settings.BestFitColumns, state);
            }
            if (state.HasRowHeader)
            {
                state.RowHeaderSpans = Utilities.GetSpanLayoutData(sheet, SheetArea.CornerHeader | SheetArea.RowHeader, rowStartIndex, rowEndIndex, 0, sheet.RowHeaderColumnCount - 1, state.RowHeaderWidths, context, false, !settings.BestFitColumns, state);
            }

            if (((settings.UseMax && (settings.RowStart == -1)) && ((settings.RowEnd == -1) && (settings.ColumnStart == -1))) && (settings.ColumnEnd == -1))
            {
                rowStartIndex = state.RowStartIndex;
                rowEndIndex = state.RowEndIndex;
                columnStartIndex = state.ColumnStartIndex;
                columnEndIndex = state.ColumnEndIndex;
                int num5 = 0;
                Utilities.ContainsCellRange(state.CellSpans.GetOutsideCellRage(), ref rowStartIndex, ref rowEndIndex, ref columnStartIndex, ref columnEndIndex);
                if (state.HasColumnHeader)
                {
                    Utilities.ContainsCellRange(state.ColumnHeaderSpans.GetOutsideCellRage(), ref num5, ref num5, ref columnStartIndex, ref columnEndIndex);
                }
                if (state.HasColumnFooter)
                {
                    Utilities.ContainsCellRange(state.ColumnFooterSpans.GetOutsideCellRage(), ref num5, ref num5, ref columnStartIndex, ref columnEndIndex);
                }
                if (state.HasRowHeader)
                {
                    Utilities.ContainsCellRange(state.RowHeaderSpans.GetOutsideCellRage(), ref rowStartIndex, ref rowEndIndex, ref num5, ref num5);
                }
                if (state.HasRowFooter)
                {
                    Utilities.ContainsCellRange(state.RowFooterSpans.GetOutsideCellRage(), ref rowStartIndex, ref rowEndIndex, ref num5, ref num5);
                }
                state.RowStartIndex = rowStartIndex;
                state.RowEndIndex = rowEndIndex;
                state.ColumnStartIndex = columnStartIndex;
                state.ColumnEndIndex = columnEndIndex;
                if (state.HasFrozenColumn && (state.ColumnStartIndex < sheet.FrozenColumnCount))
                {
                    state.ColumnStartIndex = sheet.FrozenColumnCount;
                }
                if (state.HasFrozenTrailingColumn && (state.ColumnEndIndex > ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1)))
                {
                    state.ColumnEndIndex = (sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1;
                }
                if (state.HasFrozenRow && (state.RowStartIndex < sheet.FrozenRowCount))
                {
                    state.RowStartIndex = sheet.FrozenRowCount;
                }
                if (state.HasFrozenTrailingRow && (state.RowEndIndex > ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1)))
                {
                    state.RowEndIndex = (sheet.RowCount - sheet.FrozenTrailingRowCount) - 1;
                }
            }

            SheetSectionState sectionState = new SheetSectionState();
            sectionState.SheetStates.Add(sheet, state);
            SheetPrintState printState = new SheetPrintState(sheet, state)
            {
                CurrentRowIndex = state.RowStartIndex,
                CurrentColumnIndex = state.ColumnStartIndex
            };
            sectionState.CurrentPrintStates.Add(printState);
            return sectionState;
        }

        /// <summary>
        /// Gets the build in control range.
        /// </summary>
        /// <param name="p_context">The context</param>
        /// <param name="p_x">The x</param>
        /// <param name="p_y">The y</param>
        /// <param name="p_width">The width</param>
        /// <param name="p_high">The height</param>
        /// <param name="p_buildInState">State of the build in control</param>
        /// <param name="p_horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <param name="p_continuePage">If set to <c>true</c>, [continue page]</param>
        /// <returns></returns>
        internal override List<GcRangeBlock> GetBuildInControlRange(
            GcReportContext p_context,
            int p_x,
            int p_y,
            int p_width,
            int p_high,
            object p_buildInState,
            bool p_horizontal,
            bool p_continuePage)
        {
            SheetSectionState sectionState = GetSheetSectionState(p_buildInState);
            if (sectionState == null)
            {
                return null;
            }
            if (!this.HasMorePage(sectionState, p_horizontal))
            {
                return null;
            }

            // 打印注释模式
            if (IsPrintNoteMode(sectionState, p_horizontal))
            {
                return GetSheetNoteRange(p_context, p_x, p_y, p_width, p_high, sectionState, p_horizontal);
            }

            PrintInfo info = this.worksheet.PrintInfo ?? new PrintInfo();
            if (!p_horizontal && (sectionState.CurrentPrintStates.Count > 1))
            {
                SheetPrintState state2 = sectionState.CurrentPrintStates[sectionState.CurrentPrintStates.Count - 1];
                sectionState.CurrentPrintStates.Clear();
                sectionState.CurrentPrintStates.Add(state2);
            }

            double high = p_high;
            double stateY = p_y;
            List<GcRangeBlock> list = new List<GcRangeBlock>();
            for (int i = 0; i < sectionState.CurrentPrintStates.Count; i++)
            {
                SheetPrintState printState = sectionState.CurrentPrintStates[i];
                SheetState sheetState = printState.SheetState;
                Worksheet sheet = printState.Sheet;

                if (!p_horizontal)
                {
                    printState.CurrentRowIndex += printState.LastRowCount;
                    printState.CurrentColumnIndex = printState.ColumnStartIndex;
                }
                bool hasRowHeader = sheetState.HasRowHeader;
                if (hasRowHeader && (info.ShowRowHeader == VisibilityType.ShowOnce))
                {
                    hasRowHeader = printState.CurrentColumnIndex == sheetState.ColumnStartIndex;
                }
                bool hasColumnHeader = sheetState.HasColumnHeader;
                if (hasColumnHeader && (info.ShowColumnHeader == VisibilityType.ShowOnce))
                {
                    hasColumnHeader = printState.CurrentRowIndex == sheetState.RowStartIndex;
                }
                bool hasRowFooter = sheetState.HasRowFooter;
                bool printRowFooter = false;
                if (hasRowFooter && (info.ShowRowFooter == VisibilityType.ShowOnce))
                {
                    printRowFooter = true;
                    hasRowFooter = false;
                }
                bool hasColumnFooter = sheetState.HasColumnFooter;
                bool printColFooter = false;
                if (hasColumnFooter && (info.ShowColumnFooter == VisibilityType.ShowOnce))
                {
                    printColFooter = true;
                    hasColumnFooter = false;
                }

                #region x y起始位置
                double startX = p_width - printState.OffsetX;
                if (hasRowHeader)
                {
                    startX -= sheetState.RowHeaderWidths.AllSize;
                }
                if (sheetState.HasFrozenColumn)
                {
                    startX -= sheetState.FrozenColumnWidths.AllSize;
                }
                if (sheetState.NeedRepeatColumn(printState.CurrentColumnIndex))
                {
                    startX -= sheetState.RepeatColumnWidths.AllSize;
                }
                if (sheetState.HasFrozenTrailingColumn)
                {
                    startX -= sheetState.FrozenTrailingColumnWidths.AllSize;
                }
                if (hasRowFooter)
                {
                    startX -= sheetState.RowFooterWidths.AllSize;
                }
                if (startX < 0.0)
                {
                    startX = 0.0;
                }

                double startY = high;
                if (hasColumnHeader)
                {
                    startY -= sheetState.ColumnHeaderHeights.AllSize;
                }
                if (sheetState.HasFrozenRow)
                {
                    startY -= sheetState.FrozenRowHeights.AllSize;
                }
                if (sheetState.NeedRepeatRow(printState.CurrentRowIndex))
                {
                    startY -= sheetState.RepeatRowHeights.AllSize;
                }
                if (sheetState.HasFrozenTrailingRow)
                {
                    startY -= sheetState.FrozenTrailingRowHeights.AllSize;
                }
                if (hasColumnFooter)
                {
                    startY -= sheetState.ColumnFooterHeights.AllSize;
                }
                if (startY < 0.0)
                {
                    startY = 0.0;
                }
                #endregion

                #region 计算可绘制的行数列数
                int printCols = 0;
                int colIndex = printState.CurrentColumnIndex;
                while (colIndex <= printState.ColumnEndIndex)
                {
                    double colWidth = Utilities.GetColumnWidth(sheet, sheetState.ColumnWidths, SheetArea.Cells, colIndex);

                    // hdt 此处单位0.01英寸，小数位造成提前分页、空白页现象，差别在1之内的不分页！！！
                    // colWidth = 1.04166 是报表的页面分割线
                    if (startX < colWidth
                        && (Math.Abs(startX - colWidth) > 1 || colWidth < 2))
                    {
                        break;
                    }

                    startX = Math.Max(startX - colWidth, 0);
                    colIndex++;
                    printCols++;
                }

                if ((printCols == 0) && (printState.CurrentColumnIndex <= printState.ColumnEndIndex))
                {
                    sheetState.ColumnWidths.SetSize(printState.CurrentColumnIndex, startX);
                    startX = 0.0;
                    printCols = 1;
                }

                if (printRowFooter && ((printState.CurrentColumnIndex + printCols) > sheetState.ColumnEndIndex))
                {
                    if (sheetState.RowFooterWidths.AllSize < startX)
                    {
                        hasRowFooter = true;
                        startX -= sheetState.RowFooterWidths.AllSize;
                    }
                    else if ((printCols == 1) && (startX == 0.0))
                    {
                        hasRowFooter = true;
                    }
                    else
                    {
                        printCols--;
                        startX -= Utilities.GetColumnWidth(sheet, sheetState.ColumnWidths, SheetArea.Cells, (printState.CurrentColumnIndex + printCols) - 1);
                    }
                }

                int printRows = 0;
                int rowIndex = printState.CurrentRowIndex;
                while (rowIndex <= printState.RowEndIndex)
                {
                    int acutalRowIndex = printState.GetAcutalRowIndex(rowIndex);
                    double rowHeight = Utilities.GetRowHeight(sheet, sheetState.RowHeights, SheetArea.Cells, acutalRowIndex);

                    // hdt 此处单位0.01英寸，小数位造成提前分页、空白页现象，差别在1之内的不分页！！！
                    // rowHeigh = 1.04166 是报表的页面分割线
                    if (startY < rowHeight
                        && (Math.Abs(startY - rowHeight) > 1 || rowHeight < 2))
                    {
                        break;
                    }

                    startY = Math.Max(startY - rowHeight, 0);
                    rowIndex++;
                    printRows++;
                }
                if (printRows == 0)
                {
                    if (p_continuePage)
                    {
                        return list;
                    }
                    sheetState.RowHeights.SetSize(printState.CurrentRowIndex, startY);
                    startY = 0.0;
                    printRows = 1;
                }
                if (printColFooter && ((printState.CurrentRowIndex + printRows) > sheetState.RowEndIndex))
                {
                    if (sheetState.ColumnFooterHeights.AllSize < startY)
                    {
                        hasColumnFooter = true;
                        startY -= sheetState.ColumnFooterHeights.AllSize;
                    }
                    else if ((printRows == 1) && (startY == 0.0))
                    {
                        hasColumnFooter = true;
                    }
                    else
                    {
                        printRows--;
                        startY -= Utilities.GetRowHeight(sheet, sheetState.RowHeights, SheetArea.Cells, printState.GetAcutalRowIndex((printState.CurrentRowIndex + printRows) - 1));
                    }
                }
                #endregion

                GcRangeBlock block = new GcRangeBlock(p_x + printState.OffsetX, stateY, (p_width - startX) - printState.OffsetX, high - startY);
                stateY += high - startY;
                high = startY;

                if (printCols <= 0 || printRows <= 0)
                    continue;

                // 有绘制区域
                double blockY = 0.0;
                if (hasColumnHeader)
                {
                    GcSheetBlock block2 = new GcSheetBlock(0.0, blockY, p_width - startX, sheetState.ColumnHeaderHeights.AllSize, GcSheetBlock.PartType.Header, sheet, sheetState)
                    {
                        RowStartIndex = 0,
                        RowEndIndex = sheet.ColumnHeaderRowCount - 1
                    };
                    block.Blocks.Add(block2);
                    blockY += sheetState.ColumnHeaderHeights.AllSize;
                }
                if (sheetState.HasFrozenRow)
                {
                    GcSheetBlock block3 = new GcSheetBlock(0.0, blockY, p_width - startX, sheetState.FrozenRowHeights.AllSize, GcSheetBlock.PartType.Frozen, sheet, sheetState)
                    {
                        RowStartIndex = 0,
                        RowEndIndex = sheet.FrozenRowCount - 1
                    };
                    block.Blocks.Add(block3);
                    blockY += sheetState.FrozenRowHeights.AllSize;
                }
                if (sheetState.NeedRepeatRow(printState.CurrentRowIndex))
                {
                    GcSheetBlock block4 = new GcSheetBlock(0.0, blockY, p_width - startX, sheetState.RepeatRowHeights.AllSize, GcSheetBlock.PartType.Repeat, sheet, sheetState)
                    {
                        RowStartIndex = sheetState.RepeatRowStartIndex,
                        RowEndIndex = sheetState.RepeatRowEndIndex
                    };
                    block.Blocks.Add(block4);
                    blockY += sheetState.RepeatRowHeights.AllSize;
                }
                double height = 0.0;
                for (int j = printState.CurrentRowIndex; j < (printState.CurrentRowIndex + printRows); j++)
                {
                    height += Utilities.GetRowHeight(sheet, sheetState.RowHeights, SheetArea.Cells, printState.GetAcutalRowIndex(j));
                }
                GcSheetBlock block5 = new GcSheetBlock(0.0, blockY, p_width - startX, height, GcSheetBlock.PartType.Cell, sheet, sheetState)
                {
                    RowStartIndex = printState.CurrentRowIndex,
                    RowEndIndex = (printState.CurrentRowIndex + printRows) - 1
                };
                if (printState.RowIndexs != null)
                {
                    block5.RowIndexs = new List<int>((IEnumerable<int>)printState.RowIndexs);
                }
                block.Blocks.Add(block5);
                blockY += block5.Height;
                if (sheetState.HasFrozenTrailingRow)
                {
                    GcSheetBlock block6 = new GcSheetBlock(0.0, blockY, p_width - startX, sheetState.FrozenTrailingRowHeights.AllSize, GcSheetBlock.PartType.FrozenTrailing, sheet, sheetState)
                    {
                        RowStartIndex = sheet.RowCount - sheet.FrozenTrailingRowCount,
                        RowEndIndex = sheet.RowCount - 1
                    };
                    block.Blocks.Add(block6);
                    blockY += sheetState.FrozenTrailingRowHeights.AllSize;
                }

                // hdt 导出pdf时包含图片，Chart在导出前若可视则已转为图片！！！
                if (sheet.Pictures.Count > 0)
                {
                    Rect printRc = new Rect(printState.CurrentColumnIndex, printState.CurrentRowIndex, printCols, printRows);
                    foreach (var p in sheet.Pictures)
                    {
                        // 判断是否在绘制范围
                        var rc = new Rect(p.StartColumn, p.StartRow, Math.Max(0, p.EndColumn - p.StartColumn), Math.Max(0, p.EndRow - p.StartRow));
                        rc.Intersect(printRc);
                        if (rc.IsEmpty || rc.Width == 0 || rc.Height == 0)
                            continue;

                        double leftX = 0;
                        for (int ci = 0; ci < printState.CurrentColumnIndex; ci++)
                        {
                            leftX += Utilities.GetColumnWidth(sheet, sheetState.ColumnWidths, SheetArea.Cells, ci);
                        }
                        double imgX = (p.Location.X * 100.0 / UnitManager.Dpi) - leftX;
                        if (sheetState.RowHeaderWidths != null)
                            imgX += sheetState.RowHeaderWidths.AllSize;

                        double topY = 0;
                        for (int ci = 0; ci < printState.CurrentRowIndex; ci++)
                        {
                            topY += Utilities.GetRowHeight(sheet, sheetState.RowHeights, SheetArea.Cells, ci);
                        }
                        double imgY = (p.Location.Y * 100.0 / UnitManager.Dpi) - topY;
                        if (sheetState.ColumnHeaderHeights != null)
                            imgY += sheetState.ColumnHeaderHeights.AllSize;

                        var img = new GcImage(p.GetImageData());

                        // 除等比缩放外，其他未处理
                        if (p.PictureStretch == Microsoft.UI.Xaml.Media.Stretch.Uniform)
                        {
                            double rate = Math.Min(p.Size.Width / img.Width, p.Size.Height / img.Height);
                            int iw = (int)(img.Width * rate);
                            int ih = (int)(img.Height * rate);
                            imgX += Math.Abs(p.Size.Width - iw) / 2;
                            imgY += Math.Abs(p.Size.Height - ih) / 2;
                            img.Width = (int)(iw * 100.0 / UnitManager.Dpi);
                            img.Height = (int)(ih * 100.0 / UnitManager.Dpi);
                        }
                        block.Blocks.Add(new GcBlock(imgX, imgY, img.Width, img.Height, img));
                    }
                }

                foreach (var blockx in block.Blocks)
                {
                    if (blockx is GcSheetBlock block8)
                    {
                        block8.ColumnStartIndex = printState.CurrentColumnIndex;
                        block8.ColumnEndIndex = (printState.CurrentColumnIndex + printCols) - 1;
                        block8.Horizontal = true;
                        block8.Header = hasRowHeader;
                        block8.Frozen = sheetState.HasFrozenColumn;
                        block8.Repeat = sheetState.NeedRepeatColumn(printState.CurrentColumnIndex);
                        block8.FrozenTrailing = sheetState.HasFrozenTrailingColumn;
                        block8.Footer = hasRowFooter;
                    }
                }

                if ((!sheetState.IsBookmarked && (block.Blocks.Count > 0)) && !string.IsNullOrEmpty(sheet.Name))
                {
                    GcSheetBlock block9 = block.Blocks[0] as GcSheetBlock;
                    if (block9 != null)
                    {
                        block9.Bookmark = sheet.Name;
                        sheetState.IsBookmarked = true;
                    }
                }
                list.Add(block);
                printState.CurrentColumnIndex += printCols;
                printState.LastRowCount = printRows;
                if ((!p_horizontal && (i == (sectionState.CurrentPrintStates.Count - 1))) && ((sectionState.PrintStateStack.Count > 0) && ((printState.CurrentRowIndex + printState.LastRowCount) > printState.RowEndIndex)))
                {
                    sectionState.CurrentPrintStates.Add(sectionState.PrintStateStack.Pop());
                }
            }
            return list;
        }
    }
}

