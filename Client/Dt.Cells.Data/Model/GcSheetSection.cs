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
    internal class GcSheetSection : GcMultiplePageSection, IGcAllowAppendixSection
    {
        bool flagName;
        string sheetName;
        internal const string Suffix = "ETEN2J";
        Worksheet worksheet;

        /// <summary>
        /// Creates a new section for the worksheet.
        /// </summary>
        public GcSheetSection()
        {
            this.Init();
        }

        /// <summary>
        /// Creates a new section for the specified worksheet.
        /// </summary>
        /// <param name="worksheet">The <see cref="T:Dt.Cells.Data.Worksheet" /> object.</param>
        public GcSheetSection(Worksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            this.Init();
            this.worksheet = worksheet;
            this.sheetName = worksheet.Name;
        }

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
            Worksheet worksheet = this.worksheet;
            PrintInfo settings = worksheet.PrintInfo ?? new PrintInfo();
            SheetState state = new SheetState(settings);
            StorageType type = StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data;
            state.RowStartIndex = (settings.RowStart != -1) ? Math.Max(settings.RowStart, 0) : 0;
            state.RowEndIndex = (settings.RowEnd != -1) ? Math.Min(settings.RowEnd, worksheet.RowCount - 1) : (worksheet.RowCount - 1);
            if (settings.UseMax && (settings.RowEnd == -1))
            {
                state.RowEndIndex = Math.Max(state.RowStartIndex, worksheet.GetLastDirtyRow(type));
            }
            state.ColumnStartIndex = (settings.ColumnStart != -1) ? Math.Max(settings.ColumnStart, 0) : 0;
            state.ColumnEndIndex = (settings.ColumnEnd != -1) ? Math.Min(settings.ColumnEnd, worksheet.ColumnCount - 1) : (worksheet.ColumnCount - 1);
            if (settings.UseMax && (settings.ColumnEnd == -1))
            {
                state.ColumnEndIndex = Math.Max(state.ColumnStartIndex, worksheet.GetLastDirtyColumn(type));
            }
            state.RepeatRowStartIndex = (settings.RepeatRowStart != -1) ? Math.Max(0, settings.RepeatRowStart) : settings.RepeatRowStart;
            state.RepeatRowEndIndex = Math.Min(settings.RepeatRowEnd, worksheet.RowCount - 1);
            state.RepeatColumnStartIndex = (settings.RepeatColumnStart != -1) ? Math.Max(0, settings.RepeatColumnStart) : settings.RepeatColumnStart;
            state.RepeatColumnEndIndex = Math.Min(settings.RepeatColumnEnd, worksheet.ColumnCount - 1);
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
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[]) new object[] { "Row" }));
            }
            if (state.ColumnEndIndex < state.ColumnStartIndex)
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[]) new object[] { "Column" }));
            }
            if (((state.RepeatRowEndIndex != -1) && (state.RepeatRowStartIndex != -1)) && (state.RepeatRowEndIndex < state.RepeatRowStartIndex))
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionRepeatEndIndexError, (object[]) new object[] { "row" }));
            }
            if (((state.RepeatColumnEndIndex != -1) && (state.RepeatColumnStartIndex != -1)) && (state.RepeatColumnEndIndex < state.RepeatColumnStartIndex))
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionRepeatEndIndexError, (object[]) new object[] { "column" }));
            }
            state.ColumnWidths = new PartLayoutData(GetColumnWidths(worksheet, SheetArea.Cells, state.ColumnStartIndex, state.ColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, worksheet, settings), false, settings.ShowGridLine, worksheet.BorderCollapse), state.ColumnStartIndex, worksheet.ColumnCount);
            if (ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, worksheet, settings))
            {
                state.RowHeaderWidths = new PartLayoutData(GetColumnWidths(worksheet, SheetArea.CornerHeader | SheetArea.RowHeader, 0, worksheet.RowHeaderColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, worksheet, settings), false, settings.ShowGridLine, worksheet.BorderCollapse));
            }
            if ((worksheet.FrozenColumnCount > 0) && (state.ColumnEndIndex >= worksheet.FrozenColumnCount))
            {
                if (state.ColumnStartIndex < worksheet.FrozenColumnCount)
                {
                    state.ColumnStartIndex = worksheet.FrozenColumnCount;
                }
                state.ColumnWidths.AppendSizes(GetColumnWidths(worksheet, SheetArea.Cells, 0, worksheet.FrozenColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, worksheet, settings), false, settings.ShowGridLine, worksheet.BorderCollapse), 0);
                state.FrozenColumnWidths = new PartLayoutData(state.ColumnWidths.GetSizes(0, worksheet.FrozenColumnCount));
            }
            if (state.HasRepeatColumn)
            {
                state.ColumnWidths.AppendSizes(GetColumnWidths(worksheet, SheetArea.Cells, state.RepeatColumnStartIndex, state.RepeatColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, worksheet, settings), false, settings.ShowGridLine, worksheet.BorderCollapse), state.RepeatColumnStartIndex);
                state.RepeatColumnWidths = new PartLayoutData(state.ColumnWidths.GetSizes(state.RepeatColumnStartIndex, state.RepeatColumnCount));
            }
            if ((worksheet.FrozenTrailingColumnCount > 0) && (state.ColumnStartIndex < ((worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount) - 1)))
            {
                if (state.ColumnEndIndex > ((worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount) - 1))
                {
                    state.ColumnEndIndex = (worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount) - 1;
                }
                state.ColumnWidths.AppendSizes(GetColumnWidths(worksheet, SheetArea.Cells, worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount, worksheet.FrozenTrailingColumnCount, context, settings.BestFitColumns, state.RowStartIndex, state.RowEndIndex, state.RepeatColumnStartIndex, state.RepeatColumnEndIndex, ShowPart(SheetArea.ColumnHeader, worksheet, settings), false, settings.ShowGridLine, worksheet.BorderCollapse), worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount);
                state.FrozenTrailingColumnWidths = new PartLayoutData(state.ColumnWidths.GetSizes(worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount, worksheet.FrozenTrailingColumnCount));
            }
            state.RowHeights = new PartLayoutData(GetRowHeights(worksheet, SheetArea.Cells, state.RowStartIndex, state.RowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, worksheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, worksheet.BorderCollapse), state.RowStartIndex, worksheet.RowCount);
            if (ShowPart(SheetArea.ColumnHeader, worksheet, settings))
            {
                state.ColumnHeaderHeights = new PartLayoutData(GetRowHeights(worksheet, SheetArea.ColumnHeader, 0, worksheet.ColumnHeaderRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, worksheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, worksheet.BorderCollapse));
            }
            if ((worksheet.FrozenRowCount > 0) && (state.RowEndIndex >= worksheet.FrozenRowCount))
            {
                if (state.RowStartIndex < worksheet.FrozenRowCount)
                {
                    state.RowStartIndex = worksheet.FrozenRowCount;
                }
                state.RowHeights.AppendSizes(GetRowHeights(worksheet, SheetArea.Cells, 0, worksheet.FrozenRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, worksheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, worksheet.BorderCollapse), 0);
                state.FrozenRowHeights = new PartLayoutData(state.RowHeights.GetSizes(0, worksheet.FrozenRowCount));
            }
            if (state.HasRepeatRow)
            {
                state.RowHeights.AppendSizes(GetRowHeights(worksheet, SheetArea.Cells, state.RepeatRowStartIndex, state.RepeatRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, worksheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, worksheet.BorderCollapse), state.RepeatRowStartIndex);
                state.RepeatRowHeights = new PartLayoutData(state.RowHeights.GetSizes(state.RepeatRowStartIndex, state.RepeatRowCount));
            }
            if ((worksheet.FrozenTrailingRowCount > 0) && (state.RowStartIndex < ((worksheet.RowCount - worksheet.FrozenTrailingRowCount) - 1)))
            {
                if (state.RowEndIndex > ((worksheet.RowCount - worksheet.FrozenTrailingRowCount) - 1))
                {
                    state.RowEndIndex = (worksheet.RowCount - worksheet.FrozenTrailingRowCount) - 1;
                }
                state.RowHeights.AppendSizes(GetRowHeights(worksheet, SheetArea.Cells, worksheet.RowCount - worksheet.FrozenTrailingRowCount, worksheet.FrozenTrailingRowCount, context, settings.BestFitRows, state.ColumnStartIndex, state.ColumnEndIndex, state.RepeatRowStartIndex, state.RepeatRowEndIndex, ShowPart(SheetArea.CornerHeader | SheetArea.RowHeader, worksheet, settings), false, state.ColumnWidths, state.RowHeaderWidths, state.RowFooterWidths, settings.ShowGridLine, worksheet.BorderCollapse), worksheet.RowCount - worksheet.FrozenTrailingRowCount);
                state.FrozenTrailingRowHeights = new PartLayoutData(state.RowHeights.GetSizes(worksheet.RowCount - worksheet.FrozenTrailingRowCount, worksheet.FrozenTrailingRowCount));
            }
            if (state.RowEndIndex < state.RowStartIndex)
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[]) new object[] { "Row" }));
            }
            if (state.ColumnEndIndex < state.ColumnStartIndex)
            {
                throw new InvalidOperationException(string.Format(ResourceStrings.ReportingGcSheetSectionEndIndexError, (object[]) new object[] { "Column" }));
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
                state.FrozenTrailingRowHeights.OffsetIndex = worksheet.RowCount - worksheet.FrozenTrailingRowCount;
            }
            if (state.HasFrozenTrailingColumn)
            {
                state.FrozenTrailingColumnWidths.OffsetIndex = worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount;
            }
            int rowStartIndex = state.HasFrozenRow ? 0 : state.RowStartIndex;
            int rowEndIndex = state.HasFrozenTrailingRow ? (worksheet.RowCount - 1) : state.RowEndIndex;
            int columnStartIndex = state.HasFrozenColumn ? 0 : state.ColumnStartIndex;
            int columnEndIndex = state.HasFrozenTrailingColumn ? (worksheet.ColumnCount - 1) : state.ColumnEndIndex;
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
            state.CellSpans = Utilities.GetSpanLayoutData(worksheet, SheetArea.Cells, rowStartIndex, rowEndIndex, columnStartIndex, columnEndIndex, state.ColumnWidths, context, false, !settings.BestFitColumns, state);
            if (state.HasColumnHeader)
            {
                state.ColumnHeaderSpans = Utilities.GetSpanLayoutData(worksheet, SheetArea.ColumnHeader, 0, worksheet.ColumnHeaderRowCount - 1, columnStartIndex, columnEndIndex, state.ColumnWidths, context, false, !settings.BestFitColumns, state);
            }
            if (state.HasRowHeader)
            {
                state.RowHeaderSpans = Utilities.GetSpanLayoutData(worksheet, SheetArea.CornerHeader | SheetArea.RowHeader, rowStartIndex, rowEndIndex, 0, worksheet.RowHeaderColumnCount - 1, state.RowHeaderWidths, context, false, !settings.BestFitColumns, state);
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
                if (state.HasFrozenColumn && (state.ColumnStartIndex < worksheet.FrozenColumnCount))
                {
                    state.ColumnStartIndex = worksheet.FrozenColumnCount;
                }
                if (state.HasFrozenTrailingColumn && (state.ColumnEndIndex > ((worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount) - 1)))
                {
                    state.ColumnEndIndex = (worksheet.ColumnCount - worksheet.FrozenTrailingColumnCount) - 1;
                }
                if (state.HasFrozenRow && (state.RowStartIndex < worksheet.FrozenRowCount))
                {
                    state.RowStartIndex = worksheet.FrozenRowCount;
                }
                if (state.HasFrozenTrailingRow && (state.RowEndIndex > ((worksheet.RowCount - worksheet.FrozenTrailingRowCount) - 1)))
                {
                    state.RowEndIndex = (worksheet.RowCount - worksheet.FrozenTrailingRowCount) - 1;
                }
            }
            SheetSectionState state2 = new SheetSectionState();
            state2.SheetStates.Add(worksheet, state);
            SheetPrintState state3 = new SheetPrintState(worksheet, state) {
                CurrentRowIndex = state.RowStartIndex,
                CurrentColumnIndex = state.ColumnStartIndex
            };
            state2.CurrentPrintStates.Add(state3);
            return state2;
        }

        /// <summary>
        /// Gets the build in control range.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="buildInControlState">State of the build in control</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <param name="continuePage">If set to <c>true</c>, [continue page]</param>
        /// <returns></returns>
        internal override List<GcRangeBlock> GetBuildInControlRange(GcReportContext context, int x, int y, int width, int high, object buildInControlState, bool horizontal, bool continuePage)
        {
            SheetSectionState sheetSectionState = GetSheetSectionState(buildInControlState);
            if (sheetSectionState == null)
            {
                return null;
            }
            if (!this.HasMorePage(sheetSectionState, horizontal))
            {
                return null;
            }
            if (IsPrintNoteMode(sheetSectionState, horizontal))
            {
                return GetSheetNoteRange(context, x, y, width, high, sheetSectionState, horizontal);
            }
            PrintInfo info = this.worksheet.PrintInfo ?? new PrintInfo();
            if (!horizontal && (sheetSectionState.CurrentPrintStates.Count > 1))
            {
                SheetPrintState state2 = sheetSectionState.CurrentPrintStates[sheetSectionState.CurrentPrintStates.Count - 1];
                sheetSectionState.CurrentPrintStates.Clear();
                sheetSectionState.CurrentPrintStates.Add(state2);
            }
            double num = high;
            double num2 = y;
            List<GcRangeBlock> list = new List<GcRangeBlock>();
            for (int i = 0; i < sheetSectionState.CurrentPrintStates.Count; i++)
            {
                SheetPrintState state3 = sheetSectionState.CurrentPrintStates[i];
                SheetState sheetState = state3.SheetState;
                Worksheet sheet = state3.Sheet;
                if (!horizontal)
                {
                    state3.CurrentRowIndex += state3.LastRowCount;
                    state3.CurrentColumnIndex = state3.ColumnStartIndex;
                }
                bool hasRowHeader = sheetState.HasRowHeader;
                if (hasRowHeader && (info.ShowRowHeader == VisibilityType.ShowOnce))
                {
                    hasRowHeader = state3.CurrentColumnIndex == sheetState.ColumnStartIndex;
                }
                bool hasColumnHeader = sheetState.HasColumnHeader;
                if (hasColumnHeader && (info.ShowColumnHeader == VisibilityType.ShowOnce))
                {
                    hasColumnHeader = state3.CurrentRowIndex == sheetState.RowStartIndex;
                }
                bool hasRowFooter = sheetState.HasRowFooter;
                bool flag4 = false;
                if (hasRowFooter && (info.ShowRowFooter == VisibilityType.ShowOnce))
                {
                    flag4 = true;
                    hasRowFooter = false;
                }
                bool hasColumnFooter = sheetState.HasColumnFooter;
                bool flag6 = false;
                if (hasColumnFooter && (info.ShowColumnFooter == VisibilityType.ShowOnce))
                {
                    flag6 = true;
                    hasColumnFooter = false;
                }
                double num4 = width - state3.OffsetX;
                if (hasRowHeader)
                {
                    num4 -= sheetState.RowHeaderWidths.AllSize;
                }
                if (sheetState.HasFrozenColumn)
                {
                    num4 -= sheetState.FrozenColumnWidths.AllSize;
                }
                if (sheetState.NeedRepeatColumn(state3.CurrentColumnIndex))
                {
                    num4 -= sheetState.RepeatColumnWidths.AllSize;
                }
                if (sheetState.HasFrozenTrailingColumn)
                {
                    num4 -= sheetState.FrozenTrailingColumnWidths.AllSize;
                }
                if (hasRowFooter)
                {
                    num4 -= sheetState.RowFooterWidths.AllSize;
                }
                double num5 = num;
                if (hasColumnHeader)
                {
                    num5 -= sheetState.ColumnHeaderHeights.AllSize;
                }
                if (sheetState.HasFrozenRow)
                {
                    num5 -= sheetState.FrozenRowHeights.AllSize;
                }
                if (sheetState.NeedRepeatRow(state3.CurrentRowIndex))
                {
                    num5 -= sheetState.RepeatRowHeights.AllSize;
                }
                if (sheetState.HasFrozenTrailingRow)
                {
                    num5 -= sheetState.FrozenTrailingRowHeights.AllSize;
                }
                if (hasColumnFooter)
                {
                    num5 -= sheetState.ColumnFooterHeights.AllSize;
                }
                if (num4 < 0.0)
                {
                    num4 = 0.0;
                }
                if (num5 < 0.0)
                {
                    num5 = 0.0;
                }
                int num6 = 0;
                int currentColumnIndex = state3.CurrentColumnIndex;
                while (currentColumnIndex <= state3.ColumnEndIndex)
                {
                    double num8 = Utilities.GetColumnWidth(sheet, sheetState.ColumnWidths, SheetArea.Cells, currentColumnIndex);
                    if ((num4 < num8) || ((num6 >= 1) && sheet.GetColumnPageBreak(currentColumnIndex)))
                    {
                        break;
                    }
                    num4 -= num8;
                    currentColumnIndex++;
                    num6++;
                }
                if ((num6 == 0) && (state3.CurrentColumnIndex <= state3.ColumnEndIndex))
                {
                    sheetState.ColumnWidths.SetSize(state3.CurrentColumnIndex, num4);
                    num4 = 0.0;
                    num6 = 1;
                }
                if (flag4 && ((state3.CurrentColumnIndex + num6) > sheetState.ColumnEndIndex))
                {
                    if (sheetState.RowFooterWidths.AllSize < num4)
                    {
                        hasRowFooter = true;
                        num4 -= sheetState.RowFooterWidths.AllSize;
                    }
                    else if ((num6 == 1) && (num4 == 0.0))
                    {
                        hasRowFooter = true;
                    }
                    else
                    {
                        num6--;
                        num4 -= Utilities.GetColumnWidth(sheet, sheetState.ColumnWidths, SheetArea.Cells, (state3.CurrentColumnIndex + num6) - 1);
                    }
                }
                int num9 = 0;
                int currentRowIndex = state3.CurrentRowIndex;
                while (currentRowIndex <= state3.RowEndIndex)
                {
                    int acutalRowIndex = state3.GetAcutalRowIndex(currentRowIndex);
                    double num12 = Utilities.GetRowHeight(sheet, sheetState.RowHeights, SheetArea.Cells, acutalRowIndex);
                    if ((num5 < num12) || ((num9 >= 1) && sheet.GetRowPageBreak(state3.GetAcutalRowIndex(currentRowIndex))))
                    {
                        break;
                    }
                    num5 -= num12;
                    currentRowIndex++;
                    num9++;
                }
                if (num9 == 0)
                {
                    if (continuePage)
                    {
                        return list;
                    }
                    sheetState.RowHeights.SetSize(state3.CurrentRowIndex, num5);
                    num5 = 0.0;
                    num9 = 1;
                }
                if (flag6 && ((state3.CurrentRowIndex + num9) > sheetState.RowEndIndex))
                {
                    if (sheetState.ColumnFooterHeights.AllSize < num5)
                    {
                        hasColumnFooter = true;
                        num5 -= sheetState.ColumnFooterHeights.AllSize;
                    }
                    else if ((num9 == 1) && (num5 == 0.0))
                    {
                        hasColumnFooter = true;
                    }
                    else
                    {
                        num9--;
                        num5 -= Utilities.GetRowHeight(sheet, sheetState.RowHeights, SheetArea.Cells, state3.GetAcutalRowIndex((state3.CurrentRowIndex + num9) - 1));
                    }
                }
                GcRangeBlock block = new GcRangeBlock(x + state3.OffsetX, num2, (width - num4) - state3.OffsetX, num - num5);
                num2 += num - num5;
                num = num5;
                if ((num6 > 0) && (num9 > 0))
                {
                    double num13 = 0.0;
                    if (hasColumnHeader)
                    {
                        GcSheetBlock block2 = new GcSheetBlock(0.0, num13, width - num4, sheetState.ColumnHeaderHeights.AllSize, GcSheetBlock.PartType.Header, sheet, sheetState) {
                            RowStartIndex = 0,
                            RowEndIndex = sheet.ColumnHeaderRowCount - 1
                        };
                        block.Blocks.Add(block2);
                        num13 += sheetState.ColumnHeaderHeights.AllSize;
                    }
                    if (sheetState.HasFrozenRow)
                    {
                        GcSheetBlock block3 = new GcSheetBlock(0.0, num13, width - num4, sheetState.FrozenRowHeights.AllSize, GcSheetBlock.PartType.Frozen, sheet, sheetState) {
                            RowStartIndex = 0,
                            RowEndIndex = sheet.FrozenRowCount - 1
                        };
                        block.Blocks.Add(block3);
                        num13 += sheetState.FrozenRowHeights.AllSize;
                    }
                    if (sheetState.NeedRepeatRow(state3.CurrentRowIndex))
                    {
                        GcSheetBlock block4 = new GcSheetBlock(0.0, num13, width - num4, sheetState.RepeatRowHeights.AllSize, GcSheetBlock.PartType.Repeat, sheet, sheetState) {
                            RowStartIndex = sheetState.RepeatRowStartIndex,
                            RowEndIndex = sheetState.RepeatRowEndIndex
                        };
                        block.Blocks.Add(block4);
                        num13 += sheetState.RepeatRowHeights.AllSize;
                    }
                    double height = 0.0;
                    for (int j = state3.CurrentRowIndex; j < (state3.CurrentRowIndex + num9); j++)
                    {
                        height += Utilities.GetRowHeight(sheet, sheetState.RowHeights, SheetArea.Cells, state3.GetAcutalRowIndex(j));
                    }
                    GcSheetBlock block5 = new GcSheetBlock(0.0, num13, width - num4, height, GcSheetBlock.PartType.Cell, sheet, sheetState) {
                        RowStartIndex = state3.CurrentRowIndex,
                        RowEndIndex = (state3.CurrentRowIndex + num9) - 1
                    };
                    if (state3.RowIndexs != null)
                    {
                        block5.RowIndexs = new List<int>((IEnumerable<int>) state3.RowIndexs);
                    }
                    block.Blocks.Add(block5);
                    num13 += block5.Height;
                    if (sheetState.HasFrozenTrailingRow)
                    {
                        GcSheetBlock block6 = new GcSheetBlock(0.0, num13, width - num4, sheetState.FrozenTrailingRowHeights.AllSize, GcSheetBlock.PartType.FrozenTrailing, sheet, sheetState) {
                            RowStartIndex = sheet.RowCount - sheet.FrozenTrailingRowCount,
                            RowEndIndex = sheet.RowCount - 1
                        };
                        block.Blocks.Add(block6);
                        num13 += sheetState.FrozenTrailingRowHeights.AllSize;
                    }
                    foreach (GcSheetBlock block8 in block.Blocks)
                    {
                        if (block8 != null)
                        {
                            block8.ColumnStartIndex = state3.CurrentColumnIndex;
                            block8.ColumnEndIndex = (state3.CurrentColumnIndex + num6) - 1;
                            block8.Horizontal = true;
                            block8.Header = hasRowHeader;
                            block8.Frozen = sheetState.HasFrozenColumn;
                            block8.Repeat = sheetState.NeedRepeatColumn(state3.CurrentColumnIndex);
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
                    state3.CurrentColumnIndex += num6;
                    state3.LastRowCount = num9;
                    if ((!horizontal && (i == (sheetSectionState.CurrentPrintStates.Count - 1))) && ((sheetSectionState.PrintStateStack.Count > 0) && ((state3.CurrentRowIndex + state3.LastRowCount) > state3.RowEndIndex)))
                    {
                        sheetSectionState.CurrentPrintStates.Add(sheetSectionState.PrintStateStack.Pop());
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="area">The area</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="count">The count</param>
        /// <param name="context">The context.</param>
        /// <param name="autoFit">if set to <c>true</c> [auto fit].</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="repeatStartIndex">Start index of the repeat.</param>
        /// <param name="repeatEndIndex">End index of the repeat.</param>
        /// <param name="hasTopPart">if set to <c>true</c> [has top part].</param>
        /// <param name="hasBottomPart">if set to <c>true</c> [has bottom part].</param>
        /// <param name="showGridline">if set to <c>true</c> [show gridline].</param>
        /// <param name="borderCollapse">The border collapse.</param>
        /// <returns></returns>
        static List<double> GetColumnWidths(Worksheet worksheet, SheetArea area, int startIndex, int count, GcReportContext context, bool autoFit, int rowStartIndex, int rowEndIndex, int repeatStartIndex, int repeatEndIndex, bool hasTopPart, bool hasBottomPart, bool showGridline, BorderCollapse borderCollapse)
        {
            return Utilities.GetColumnWidths(worksheet, area, startIndex, count, context, autoFit, rowStartIndex, rowEndIndex, repeatStartIndex, repeatEndIndex, hasTopPart, hasBottomPart, UnitType.CentileInch, showGridline, borderCollapse);
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="area">The area</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="count">The count</param>
        /// <param name="context">The context</param>
        /// <param name="autoFit">If set to <c>true</c>, [auto fit]</param>
        /// <param name="columnStartIndex">Start index of the column</param>
        /// <param name="columnEndIndex">End index of the column</param>
        /// <param name="repeatStartIndex">Start index of the repeat</param>
        /// <param name="repeatEndIndex">End index of the repeat</param>
        /// <param name="hasLeftPart">if set to <c>true</c> [has left part].</param>
        /// <param name="hasRightPart">if set to <c>true</c> [has right part].</param>
        /// <param name="colWidths">The col widths.</param>
        /// <param name="lColWidths">The l col widths.</param>
        /// <param name="rColWidths">The r col widths.</param>
        /// <param name="showGridline">if set to <c>true</c> [show gridline].</param>
        /// <param name="borderCollapse">The border collapse.</param>
        /// <returns></returns>
        static List<double> GetRowHeights(Worksheet worksheet, SheetArea area, int startIndex, int count, GcReportContext context, bool autoFit, int columnStartIndex, int columnEndIndex, int repeatStartIndex, int repeatEndIndex, bool hasLeftPart, bool hasRightPart, PartLayoutData colWidths, PartLayoutData lColWidths, PartLayoutData rColWidths, bool showGridline, BorderCollapse borderCollapse)
        {
            return Utilities.GetRowHeights(worksheet, area, startIndex, count, context, autoFit, columnStartIndex, columnEndIndex, repeatStartIndex, repeatEndIndex, hasLeftPart, hasRightPart, colWidths, lColWidths, rColWidths, UnitType.CentileInch, showGridline, borderCollapse);
        }

        /// <summary>
        /// Gets the sheet note part.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="range">The range</param>
        /// <param name="nps">The NPS</param>
        /// <param name="lineNumber">The line number</param>
        /// <param name="offsetY">The offset Y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="titleLabel">The title label</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        static bool GetSheetNotePart(GcReportContext context, GcRangeBlock range, NotePrintState nps, int lineNumber, ref int offsetY, int width, int high, GcLabel titleLabel, string value)
        {
            int lastOffsetX = 0;
            int num2 = 0;
            if (nps.LastColumn <= 0)
            {
                if (nps.HasLastState)
                {
                    nps.ResetLastState();
                }
                GcBlock block = new GcBlock((double) lastOffsetX, (double) offsetY, (double) titleLabel.Width, (double) titleLabel.Height, titleLabel);
                num2 = Math.Max(num2, titleLabel.Height);
                if (high > (offsetY + num2))
                {
                    range.Blocks.Add(block);
                }
                else
                {
                    nps.LastLine = lineNumber;
                    nps.LastColumn = 0;
                    nps.LastOffsetX = lastOffsetX;
                    nps.LastOffsetY = 0;
                    return false;
                }
                lastOffsetX += titleLabel.Width;
            }
            if (nps.LastColumn <= 1)
            {
                if (nps.HasLastState)
                {
                    lastOffsetX = nps.LastOffsetX;
                    offsetY = nps.LastOffsetY;
                    nps.ResetLastState();
                }
                GcLabel label = new GcLabel(value) {
                    X = lastOffsetX,
                    Y = offsetY,
                    Width = width - lastOffsetX
                };
                label.Font = nps.NoteFont;
                label.AutoSize(context);
                num2 = Math.Max(num2, label.Height);
                range.Blocks.Add(label.GetBlock(context));
                if (high < (offsetY + num2))
                {
                    nps.LastLine = lineNumber;
                    nps.LastColumn = 1;
                    nps.LastOffsetX = lastOffsetX;
                    nps.LastOffsetY = offsetY - high;
                    return false;
                }
                offsetY += num2;
            }
            return true;
        }

        /// <summary>
        /// Gets the sheet note range.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns></returns>
        static List<GcRangeBlock> GetSheetNoteRange(GcReportContext context, int x, int y, int width, int high, SheetSectionState sectionState, bool horizontal)
        {
            if ((sectionState == null) || !SheetNotesHasMorePage(sectionState, horizontal))
            {
                return null;
            }
            List<GcRangeBlock> list = new List<GcRangeBlock>();
            GcRangeBlock range = new GcRangeBlock((double) x, (double) y, (double) width, (double) high);
            list.Add(range);
            int offsetY = 0;
            NotePrintState notePrintState = sectionState.NotePrintState;
            while (notePrintState.Index < sectionState.NoteStates.Count)
            {
                NoteState state2 = sectionState.NoteStates[notePrintState.Index];
                if (((notePrintState.LastLine <= 0) && notePrintState.IsMultiSheet) && (!string.IsNullOrEmpty(state2.Sheet.Name) && !GetSheetNotePart(context, range, notePrintState, 0, ref offsetY, width, high, notePrintState.SheetTitle, state2.Sheet.Name)))
                {
                    return list;
                }
                if ((notePrintState.LastLine <= 1) && !GetSheetNotePart(context, range, notePrintState, 1, ref offsetY, width, high, notePrintState.CellTitle, string.Format("{0} {1}", (object[]) new object[] { state2.Sheet.GetColumnLabel(state2.ColumnIndex), state2.Sheet.GetRowLabel(state2.RowIndex) })))
                {
                    return list;
                }
                notePrintState.Index++;
            }
            return list;
        }

        /// <summary>
        /// Gets the state of the sheet section.
        /// </summary>
        /// <param name="buildInControlState">State of the build in control</param>
        /// <returns></returns>
        static SheetSectionState GetSheetSectionState(object buildInControlState)
        {
            if (buildInControlState == null)
            {
                return null;
            }
            return (buildInControlState as SheetSectionState);
        }

        /// <summary>
        /// Determines whether [has more page] [the specified build in control state].
        /// </summary>
        /// <param name="buildInControlState">State of the built-in control.</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal].</param>
        /// <returns>
        /// <c>true</c> if [has more page] [the specified built-in control state]; otherwise, <c>false</c>.
        /// </returns>
        protected override bool HasMorePage(object buildInControlState, bool horizontal)
        {
            SheetSectionState sheetSectionState = GetSheetSectionState(buildInControlState);
            if (sheetSectionState == null)
            {
                return false;
            }
            if (!SheetHasMorePage(sheetSectionState, horizontal))
            {
                return SheetNotesHasMorePage(sheetSectionState, horizontal);
            }
            return true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.worksheet = null;
            this.sheetName = string.Empty;
            this.flagName = false;
        }

        /// <summary>
        /// Determines whether [is print note mode] [the specified section state].
        /// </summary>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns>
        /// <c>true</c> if [is print note mode] [the specified section state]; otherwise, <c>false</c>
        /// </returns>
        static bool IsPrintNoteMode(SheetSectionState sectionState, bool horizontal)
        {
            return (!SheetHasMorePage(sectionState, horizontal) && SheetNotesHasMorePage(sectionState, horizontal));
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void ReadXmlBase(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "SheetName":
                        this.sheetName = Serializer.ReadAttribute("value", reader);
                        return;

                    case "FlagName":
                        this.flagName = Serializer.ReadAttributeBoolean("value", false, reader);
                        return;
                }
                base.ReadXmlBase(reader);
            }
        }

        /// <summary>
        /// The sheet has more pages.
        /// </summary>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns></returns>
        static bool SheetHasMorePage(SheetSectionState sectionState, bool horizontal)
        {
            bool flag = false;
            if (horizontal)
            {
                foreach (SheetPrintState state in sectionState.CurrentPrintStates)
                {
                    if ((state.CurrentColumnIndex <= state.ColumnEndIndex) && (state.LastRowCount != 0))
                    {
                        return true;
                    }
                }
                return flag;
            }
            if (sectionState.PrintStateStack.Count > 0)
            {
                return true;
            }
            if (sectionState.CurrentPrintStates.Count > 0)
            {
                SheetPrintState state2 = sectionState.CurrentPrintStates[sectionState.CurrentPrintStates.Count - 1];
                flag = (state2.CurrentRowIndex + state2.LastRowCount) <= state2.RowEndIndex;
            }
            return flag;
        }

        /// <summary>
        /// The sheet notes have more pages.
        /// </summary>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns></returns>
        static bool SheetNotesHasMorePage(SheetSectionState sectionState, bool horizontal)
        {
            return ((!horizontal && (sectionState.NoteStates.Count > 0)) && (sectionState.NotePrintState.Index < sectionState.NoteStates.Count));
        }

        /// <summary>
        /// Shows the part.
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="worksheet">The sheet</param>
        /// <param name="settings">The settings</param>
        /// <returns></returns>
        static bool ShowPart(SheetArea area, Worksheet worksheet, PrintInfo settings)
        {
            VisibilityType showRowHeader;
            bool rowHeaderVisible;
            int rowHeaderColumnCount;
            switch (area)
            {
                case SheetArea.Cells:
                    return true;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    showRowHeader = settings.ShowRowHeader;
                    rowHeaderVisible = worksheet.RowHeaderVisible;
                    rowHeaderColumnCount = worksheet.RowHeaderColumnCount;
                    break;

                case SheetArea.ColumnHeader:
                    showRowHeader = settings.ShowColumnHeader;
                    rowHeaderVisible = worksheet.ColumnHeaderVisible;
                    rowHeaderColumnCount = worksheet.ColumnHeaderRowCount;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("area");
            }
            switch (showRowHeader)
            {
                case VisibilityType.Inherit:
                    if (!rowHeaderVisible)
                    {
                        return false;
                    }
                    return (rowHeaderColumnCount > 0);

                case VisibilityType.Hide:
                    return false;

                case VisibilityType.Show:
                    return (rowHeaderColumnCount > 0);

                case VisibilityType.ShowOnce:
                    return (rowHeaderColumnCount > 0);
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteXmlBase(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            base.WriteXmlBase(writer);
            if (!this.flagName)
            {
                this.sheetName = (this.worksheet == null) ? string.Empty : this.worksheet.Name;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets a value indicating whether [flag name].
        /// </summary>
        /// <value><c>true</c> if [flag name]; otherwise, <c>false</c></value>
        internal bool FlagName
        {
            get { return  this.flagName; }
            set { this.flagName = value; }
        }

        /// <summary>
        /// Gets or sets the worksheet.
        /// </summary>
        /// <value>The <see cref="T:Dt.Cells.Data.Worksheet" /> object.</value>
        public Worksheet Sheet
        {
            get { return  this.worksheet; }
            set
            {
                this.worksheet = value;
                this.sheetName = (this.worksheet != null) ? this.worksheet.Name : string.Empty;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the name of the sheet.
        /// </summary>
        /// <value>The name of the sheet</value>
        internal string SheetName
        {
            get { return  this.sheetName; }
            set { this.sheetName = value; }
        }

        /// <summary>
        /// Internal only.
        /// NotePrintState
        /// </summary>
        internal class NotePrintState
        {
            GcLabel cellTitle;
            GcLabel commentTitle;
            int index;
            bool isMultiSheet;
            int lastColumn = -1;
            int lastLine = -1;
            int lastOffsetX;
            int lastOffsetY;
            const string NoteCellTitle = "Cell: ";
            const string NoteCommentTitle = "Comment: ";
            Font noteFont;
            Font noteHeadFont;
            const string NoteSheetTitle = "Sheet: ";
            GcLabel sheetTitle;

            /// <summary>
            /// Creates all title controls.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="font">The font.</param>
            public void CreatAllTitleControls(GcReportContext context, Font font)
            {
                this.sheetTitle = new GcLabel("Sheet: ");
                this.sheetTitle.Font = font;
                this.sheetTitle.Alignment.WordWrap = false;
                this.sheetTitle.AutoSize(context);
                this.cellTitle = new GcLabel("Cell: ");
                this.cellTitle.Font = font;
                this.cellTitle.Alignment.WordWrap = false;
                this.cellTitle.AutoSize(context);
                this.commentTitle = new GcLabel("Comment: ");
                this.commentTitle.Font = font;
                this.commentTitle.Alignment.WordWrap = false;
                this.commentTitle.AutoSize(context);
            }

            /// <summary>
            /// Resets the last state.
            /// </summary>
            public void ResetLastState()
            {
                this.LastLine = -1;
                this.LastColumn = -1;
                this.LastOffsetX = 0;
                this.LastOffsetY = 0;
            }

            /// <summary>
            /// Gets the cell title.
            /// </summary>
            /// <value>The cell title.</value>
            public GcLabel CellTitle
            {
                get { return  this.cellTitle; }
            }

            /// <summary>
            /// Gets the comment title.
            /// </summary>
            /// <value>The comment title.</value>
            public GcLabel CommentTitle
            {
                get { return  this.commentTitle; }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a last state.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a last state; otherwise, <c>false</c>.
            /// </value>
            public bool HasLastState
            {
                get { return  (this.lastLine != -1); }
            }

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            /// <value>The index.</value>
            public int Index
            {
                get { return  this.index; }
                set { this.index = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance has multiple sheets.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has multiple sheets; otherwise, <c>false</c>.
            /// </value>
            public bool IsMultiSheet
            {
                get { return  this.isMultiSheet; }
                set { this.isMultiSheet = value; }
            }

            /// <summary>
            /// Gets or sets the last column.
            /// </summary>
            /// <value>The last column.</value>
            public int LastColumn
            {
                get { return  this.lastColumn; }
                set { this.lastColumn = value; }
            }

            /// <summary>
            /// Gets or sets the last line.
            /// </summary>
            /// <value>The last line.</value>
            public int LastLine
            {
                get { return  this.lastLine; }
                set { this.lastLine = value; }
            }

            /// <summary>
            /// Gets or sets the last X offset.
            /// </summary>
            /// <value>The last X offset.</value>
            public int LastOffsetX
            {
                get { return  this.lastOffsetX; }
                set { this.lastOffsetX = value; }
            }

            /// <summary>
            /// Gets or sets the last Y offset.
            /// </summary>
            /// <value>The last Y offset.</value>
            public int LastOffsetY
            {
                get { return  this.lastOffsetY; }
                set { this.lastOffsetY = value; }
            }

            /// <summary>
            /// Gets or sets the note font.
            /// </summary>
            /// <value>The note font.</value>
            public Font NoteFont
            {
                get { return  this.noteFont; }
                set { this.noteFont = value; }
            }

            /// <summary>
            /// Gets or sets the note head font.
            /// </summary>
            /// <value>The note head font.</value>
            public Font NoteHeadFont
            {
                get { return  this.noteHeadFont; }
                set { this.noteHeadFont = value; }
            }

            /// <summary>
            /// Gets the sheet title.
            /// </summary>
            /// <value>The sheet title.</value>
            public GcLabel SheetTitle
            {
                get { return  this.sheetTitle; }
            }
        }

        /// <summary>
        /// Internal only.
        /// NoteState
        /// </summary>
        internal class NoteState
        {
            readonly int columnIndex;
            readonly int rowIndex;
            readonly Worksheet worksheet;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetSection.NoteState" /> class.
            /// </summary>
            /// <param name="worksheet">The sheet.</param>
            /// <param name="rowIndex">The row index.</param>
            /// <param name="columnIndex">The column index.</param>
            public NoteState(Worksheet worksheet, int rowIndex, int columnIndex)
            {
                this.worksheet = worksheet;
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
            }

            /// <summary>
            /// Gets the column index.
            /// </summary>
            /// <value>The index of the column.</value>
            public int ColumnIndex
            {
                get { return  this.columnIndex; }
            }

            /// <summary>
            /// Gets the index of the row.
            /// </summary>
            /// <value>The index of the row.</value>
            public int RowIndex
            {
                get { return  this.rowIndex; }
            }

            /// <summary>
            /// Gets the sheet.
            /// </summary>
            /// <value>The sheet.</value>
            public Worksheet Sheet
            {
                get { return  this.worksheet; }
            }
        }

        /// <summary>
        /// Internal only.
        /// SheetPrintState
        /// </summary>
        internal class SheetPrintState
        {
            int columnEndIndex;
            int columnStartIndex;
            int currentColumnIndex;
            int currentRowIndex;
            int lastRowCount;
            double offsetX;
            int rowEndIndex;
            List<int> rowIndexs;
            int rowStartIndex;
            bool seekChild = true;
            readonly Dt.Cells.Data.GcSheetSection.SheetState sheetState;
            readonly Worksheet worksheet;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetSection.SheetPrintState" /> class.
            /// </summary>
            /// <param name="worksheet">The sheet.</param>
            /// <param name="sheetState">State of the sheet.</param>
            public SheetPrintState(Worksheet worksheet, Dt.Cells.Data.GcSheetSection.SheetState sheetState)
            {
                this.worksheet = worksheet;
                this.sheetState = sheetState;
                this.rowStartIndex = sheetState.RowStartIndex;
                this.rowEndIndex = sheetState.RowEndIndex;
                this.columnStartIndex = sheetState.ColumnStartIndex;
                this.columnEndIndex = sheetState.ColumnEndIndex;
                this.currentRowIndex = this.rowStartIndex;
                this.currentColumnIndex = this.columnStartIndex;
            }

            /// <summary>
            /// Gets the index of the displayed row.
            /// </summary>
            /// <param name="row">The row.</param>
            /// <returns></returns>
            public int GetAcutalRowIndex(int row)
            {
                if (this.RowIndexs != null)
                {
                    return this.RowIndexs[row];
                }
                return row;
            }

            /// <summary>
            /// Splits the row.
            /// </summary>
            /// <param name="row">The row.</param>
            /// <returns></returns>
            public GcSheetSection.SheetPrintState SplitRow(int row)
            {
                if (this.rowIndexs != null)
                {
                    row = this.rowIndexs.IndexOf(row);
                }
                if ((row >= this.RowEndIndex) || (row < 0))
                {
                    return null;
                }
                GcSheetSection.SheetPrintState state = new GcSheetSection.SheetPrintState(this.worksheet, this.sheetState);
                if (this.rowIndexs == null)
                {
                    state.rowStartIndex = row + 1;
                    state.currentRowIndex = state.rowStartIndex;
                    state.rowEndIndex = this.rowEndIndex;
                    this.rowEndIndex = row;
                }
                else
                {
                    state.rowStartIndex = 0;
                    state.currentRowIndex = 0;
                    state.rowIndexs = new List<int>();
                    state.rowIndexs.AddRange((IEnumerable<int>) this.rowIndexs.GetRange(row + 1, (this.rowIndexs.Count - row) - 1));
                    state.rowEndIndex = state.rowIndexs.Count - 1;
                    this.rowIndexs.RemoveRange(row + 1, (this.rowIndexs.Count - row) - 1);
                    this.rowEndIndex = this.rowIndexs.Count - 1;
                }
                state.offsetX = this.offsetX;
                state.seekChild = this.seekChild;
                return state;
            }

            /// <summary>
            /// Gets or sets the end index of the column.
            /// </summary>
            /// <value>The end index of the column.</value>
            public int ColumnEndIndex
            {
                get { return  this.columnEndIndex; }
                set { this.columnEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the column.
            /// </summary>
            /// <value>The start index of the column.</value>
            public int ColumnStartIndex
            {
                get { return  this.columnStartIndex; }
                set { this.columnStartIndex = value; }
            }

            /// <summary>
            /// Gets or sets the index of the current column.
            /// </summary>
            /// <value>The index of the current column.</value>
            public int CurrentColumnIndex
            {
                get { return  this.currentColumnIndex; }
                set { this.currentColumnIndex = value; }
            }

            /// <summary>
            /// Gets or sets the index of the current row.
            /// </summary>
            /// <value>The index of the current row.</value>
            public int CurrentRowIndex
            {
                get { return  this.currentRowIndex; }
                set { this.currentRowIndex = value; }
            }

            /// <summary>
            /// Gets or sets the last row count.
            /// </summary>
            /// <value>The last row count.</value>
            public int LastRowCount
            {
                get { return  this.lastRowCount; }
                set { this.lastRowCount = value; }
            }

            /// <summary>
            /// Gets or sets the X offset.
            /// </summary>
            /// <value>The X offset.</value>
            public double OffsetX
            {
                get { return  this.offsetX; }
                set { this.offsetX = value; }
            }

            /// <summary>
            /// Gets or sets the end index of the row.
            /// </summary>
            /// <value>The end index of the row.</value>
            public int RowEndIndex
            {
                get { return  this.rowEndIndex; }
                set { this.rowEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the row indexes.
            /// </summary>
            /// <value>The row indexes.</value>
            public List<int> RowIndexs
            {
                get { return  this.rowIndexs; }
                set { this.rowIndexs = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the row.
            /// </summary>
            /// <value>The start index of the row.</value>
            public int RowStartIndex
            {
                get { return  this.rowStartIndex; }
                set { this.rowStartIndex = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether to search for child values.
            /// </summary>
            /// <value><c>true</c> if [seek child]; otherwise, <c>false</c>.</value>
            public bool SeekChild
            {
                get { return  this.seekChild; }
                set { this.seekChild = value; }
            }

            /// <summary>
            /// Gets the sheet.
            /// </summary>
            /// <value>The sheet.</value>
            public Worksheet Sheet
            {
                get { return  this.worksheet; }
            }

            /// <summary>
            /// Gets the state of the sheet.
            /// </summary>
            /// <value>The state of the sheet.</value>
            public Dt.Cells.Data.GcSheetSection.SheetState SheetState
            {
                get { return  this.sheetState; }
            }
        }

        /// <summary>
        /// Internal only.
        /// SheetSectionState
        /// </summary>
        internal class SheetSectionState
        {
            readonly List<DataRelation> circleRelation = new List<DataRelation>();
            readonly List<GcSheetSection.SheetPrintState> currentPrintStates = new List<GcSheetSection.SheetPrintState>();
            Worksheet groupSheet;
            readonly Dt.Cells.Data.GcSheetSection.NotePrintState notePrintState = new Dt.Cells.Data.GcSheetSection.NotePrintState();
            readonly List<GcSheetSection.NoteState> noteStates = new List<GcSheetSection.NoteState>();
            readonly Stack<GcSheetSection.SheetPrintState> printStateStack = new Stack<GcSheetSection.SheetPrintState>();
            readonly Dictionary<Worksheet, GcSheetSection.SheetState> sheetStates = new Dictionary<Worksheet, GcSheetSection.SheetState>();

            /// <summary>
            /// Gets the circle relation.
            /// </summary>
            /// <value>The circle relation.</value>
            public List<DataRelation> CircleRelation
            {
                get { return  this.circleRelation; }
            }

            /// <summary>
            /// Gets the current print states.
            /// </summary>
            /// <value>The current print states.</value>
            public List<GcSheetSection.SheetPrintState> CurrentPrintStates
            {
                get { return  this.currentPrintStates; }
            }

            /// <summary>
            /// Gets or sets the group sheet.
            /// </summary>
            /// <value>The group sheet.</value>
            public Worksheet GroupSheet
            {
                get { return  this.groupSheet; }
                set { this.groupSheet = value; }
            }

            /// <summary>
            /// Gets the state of the print note.
            /// </summary>
            /// <value>The state of the print note.</value>
            public Dt.Cells.Data.GcSheetSection.NotePrintState NotePrintState
            {
                get { return  this.notePrintState; }
            }

            /// <summary>
            /// Gets the note states.
            /// </summary>
            /// <value>The note states.</value>
            public List<GcSheetSection.NoteState> NoteStates
            {
                get { return  this.noteStates; }
            }

            /// <summary>
            /// Gets the print state stack.
            /// </summary>
            /// <value>The print state stack.</value>
            public Stack<GcSheetSection.SheetPrintState> PrintStateStack
            {
                get { return  this.printStateStack; }
            }

            /// <summary>
            /// Gets the sheet states.
            /// </summary>
            /// <value>The sheet states.</value>
            public Dictionary<Worksheet, GcSheetSection.SheetState> SheetStates
            {
                get { return  this.sheetStates; }
            }
        }

        /// <summary>
        /// Internal only.
        /// SheetState
        /// </summary>
        internal class SheetState
        {
            SpanLayoutData bottomLeftCornerSpans;
            SpanLayoutData bottomRightCornerSpans;
            SpanLayoutData cellSpans;
            int columnEndIndex;
            PartLayoutData columnFooterHeights;
            SpanLayoutData columnFooterSpans;
            PartLayoutData columnHeaderHeights;
            SpanLayoutData columnHeaderSpans;
            int columnStartIndex;
            PartLayoutData columnWidths;
            PartLayoutData frozenColumnWidths;
            PartLayoutData frozenRowHeights;
            PartLayoutData frozenTrailingColumnWidths;
            PartLayoutData frozenTrailingRowHeights;
            bool hasHierarchy;
            bool isBookmarked;
            bool isHierarchy;
            int repeatColumnEndIndex = -1;
            int repeatColumnStartIndex = -1;
            PartLayoutData repeatColumnWidths;
            int repeatRowEndIndex = -1;
            PartLayoutData repeatRowHeights;
            int repeatRowStartIndex = -1;
            int rowEndIndex;
            SpanLayoutData rowFooterSpans;
            PartLayoutData rowFooterWidths;
            SpanLayoutData rowHeaderSpans;
            PartLayoutData rowHeaderWidths;
            PartLayoutData rowHeights;
            int rowStartIndex;
            readonly PrintInfo settings;
            readonly Dictionary<Rect, List<Shape>> shapes = new Dictionary<Rect, List<Shape>>();
            readonly Dictionary<Rect, List<StickyNote>> stickyNotes = new Dictionary<Rect, List<StickyNote>>();
            SpanLayoutData topLeftCornerSpans;
            SpanLayoutData topRightCornerSpans;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetSection.SheetState" /> class.
            /// </summary>
            /// <param name="settings">The settings.</param>
            public SheetState(PrintInfo settings)
            {
                this.settings = settings;
            }

            public bool NeedRepeatColumn(int columnIndex)
            {
                return (this.HasRepeatColumn && (columnIndex > this.RepeatColumnEndIndex));
            }

            public bool NeedRepeatRow(int rowIndex)
            {
                return (this.HasRepeatRow && (rowIndex > this.RepeatRowEndIndex));
            }

            /// <summary>
            /// Gets or sets the bottom left corner spans.
            /// </summary>
            /// <value>The bottom left corner spans.</value>
            public SpanLayoutData BottomLeftCornerSpans
            {
                get { return  this.bottomLeftCornerSpans; }
                set { this.bottomLeftCornerSpans = value; }
            }

            /// <summary>
            /// Gets or sets the bottom right corner spans.
            /// </summary>
            /// <value>The bottom right corner spans.</value>
            public SpanLayoutData BottomRightCornerSpans
            {
                get { return  this.bottomRightCornerSpans; }
                set { this.bottomRightCornerSpans = value; }
            }

            /// <summary>
            /// Gets or sets the cell spans.
            /// </summary>
            /// <value>The cell spans.</value>
            public SpanLayoutData CellSpans
            {
                get { return  this.cellSpans; }
                set { this.cellSpans = value; }
            }

            /// <summary>
            /// Gets the column count.
            /// </summary>
            /// <value>The column count.</value>
            public int ColumnCount
            {
                get { return  ((this.ColumnEndIndex - this.ColumnStartIndex) + 1); }
            }

            /// <summary>
            /// Gets or sets the end index of the column.
            /// </summary>
            /// <value>The end index of the column.</value>
            public int ColumnEndIndex
            {
                get { return  this.columnEndIndex; }
                set { this.columnEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the column footer heights.
            /// </summary>
            /// <value>The column footer heights.</value>
            public PartLayoutData ColumnFooterHeights
            {
                get { return  this.columnFooterHeights; }
                set { this.columnFooterHeights = value; }
            }

            /// <summary>
            /// Gets or sets the column footer spans.
            /// </summary>
            /// <value>The column footer spans.</value>
            public SpanLayoutData ColumnFooterSpans
            {
                get { return  this.columnFooterSpans; }
                set { this.columnFooterSpans = value; }
            }

            /// <summary>
            /// Gets or sets the column header heights.
            /// </summary>
            /// <value>The column header heights.</value>
            public PartLayoutData ColumnHeaderHeights
            {
                get { return  this.columnHeaderHeights; }
                set { this.columnHeaderHeights = value; }
            }

            /// <summary>
            /// Gets or sets the column header spans.
            /// </summary>
            /// <value>The column header spans.</value>
            public SpanLayoutData ColumnHeaderSpans
            {
                get { return  this.columnHeaderSpans; }
                set { this.columnHeaderSpans = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the column.
            /// </summary>
            /// <value>The start index of the column.</value>
            public int ColumnStartIndex
            {
                get { return  this.columnStartIndex; }
                set { this.columnStartIndex = value; }
            }

            /// <summary>
            /// Gets or sets the column widths.
            /// </summary>
            /// <value>The column widths.</value>
            public PartLayoutData ColumnWidths
            {
                get { return  this.columnWidths; }
                set { this.columnWidths = value; }
            }

            /// <summary>
            /// Gets or sets the frozen column widths.
            /// </summary>
            /// <value>The frozen column widths.</value>
            public PartLayoutData FrozenColumnWidths
            {
                get { return  this.frozenColumnWidths; }
                set { this.frozenColumnWidths = value; }
            }

            /// <summary>
            /// Gets or sets the frozen row heights.
            /// </summary>
            /// <value>The frozen row heights.</value>
            public PartLayoutData FrozenRowHeights
            {
                get { return  this.frozenRowHeights; }
                set { this.frozenRowHeights = value; }
            }

            /// <summary>
            /// Gets or sets the frozen trailing column widths.
            /// </summary>
            /// <value>The frozen trailing column widths.</value>
            public PartLayoutData FrozenTrailingColumnWidths
            {
                get { return  this.frozenTrailingColumnWidths; }
                set { this.frozenTrailingColumnWidths = value; }
            }

            /// <summary>
            /// Gets or sets the frozen trailing row heights.
            /// </summary>
            /// <value>The frozen trailing row heights.</value>
            public PartLayoutData FrozenTrailingRowHeights
            {
                get { return  this.frozenTrailingRowHeights; }
                set { this.frozenTrailingRowHeights = value; }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a bottom left corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a bottom left corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasBottomLeftCorner
            {
                get { return  (this.HasColumnFooter && this.HasRowHeader); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a bottom right corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a bottom right corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasBottomRightCorner
            {
                get { return  (this.HasColumnFooter && this.HasRowFooter); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a column footer.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a column footer; otherwise, <c>false</c>.
            /// </value>
            public bool HasColumnFooter
            {
                get { return  (this.ColumnFooterHeights != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a column header.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a column header; otherwise, <c>false</c>.
            /// </value>
            public bool HasColumnHeader
            {
                get { return  (this.ColumnHeaderHeights != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen columns.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen columns; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenColumn
            {
                get { return  (!this.IsHierarchy && (this.FrozenColumnWidths != null)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen rows.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen rows; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenRow
            {
                get { return  (!this.IsHierarchy && (this.FrozenRowHeights != null)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen trailing columns.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen trailing columns; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenTrailingColumn
            {
                get { return  (!this.IsHierarchy && (this.FrozenTrailingColumnWidths != null)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen trailing rows.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen trailing rows; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenTrailingRow
            {
                get { return  (!this.IsHierarchy && (this.FrozenTrailingRowHeights != null)); }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance has a hierarchy.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a hierarchy; otherwise, <c>false</c>.
            /// </value>
            public bool HasHierarchy
            {
                get { return  this.hasHierarchy; }
                set { this.hasHierarchy = value; }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has repeated columns.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has repeated columns; otherwise, <c>false</c>.
            /// </value>
            public bool HasRepeatColumn
            {
                get { return  (((!this.IsHierarchy && (this.RepeatColumnStartIndex >= 0)) && (this.RepeatColumnEndIndex >= 0)) && (this.RepeatColumnEndIndex >= this.RepeatColumnStartIndex)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has repeated rows.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has repeated rows; otherwise, <c>false</c>.
            /// </value>
            public bool HasRepeatRow
            {
                get { return  (((!this.IsHierarchy && (this.RepeatRowStartIndex >= 0)) && (this.RepeatRowEndIndex >= 0)) && (this.RepeatRowEndIndex >= this.RepeatRowStartIndex)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a row footer.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a row footer; otherwise, <c>false</c>.
            /// </value>
            public bool HasRowFooter
            {
                get { return  (this.RowFooterWidths != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a row header.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a row header; otherwise, <c>false</c>.
            /// </value>
            public bool HasRowHeader
            {
                get { return  (this.RowHeaderWidths != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a top left corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a top left corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasTopLeftCorner
            {
                get { return  (this.HasColumnHeader && this.HasRowHeader); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a top right corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a top right corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasTopRightCorner
            {
                get { return  (this.HasColumnHeader && this.HasRowFooter); }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance is bookmarked.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is bookmarked; otherwise, <c>false</c>.
            /// </value>
            public bool IsBookmarked
            {
                get { return  this.isBookmarked; }
                set { this.isBookmarked = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance is a hierarchy.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is a hierarchy; otherwise, <c>false</c>.
            /// </value>
            public bool IsHierarchy
            {
                get { return  this.isHierarchy; }
                set { this.isHierarchy = value; }
            }

            /// <summary>
            /// Gets the repeated column count.
            /// </summary>
            /// <value>The repeated column count.</value>
            public int RepeatColumnCount
            {
                get
                {
                    if (!this.HasRepeatColumn)
                    {
                        return 0;
                    }
                    return ((this.RepeatColumnEndIndex - this.RepeatColumnStartIndex) + 1);
                }
            }

            /// <summary>
            /// Gets or sets the end index of the repeated column.
            /// </summary>
            /// <value>The end index of the repeated column.</value>
            public int RepeatColumnEndIndex
            {
                get { return  this.repeatColumnEndIndex; }
                set { this.repeatColumnEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the repeated column.
            /// </summary>
            /// <value>The start index of the repeated column.</value>
            public int RepeatColumnStartIndex
            {
                get { return  this.repeatColumnStartIndex; }
                set { this.repeatColumnStartIndex = value; }
            }

            /// <summary>
            /// Gets or sets the repeated column widths.
            /// </summary>
            /// <value>The repeated column widths.</value>
            public PartLayoutData RepeatColumnWidths
            {
                get { return  this.repeatColumnWidths; }
                set { this.repeatColumnWidths = value; }
            }

            /// <summary>
            /// Gets the repeated row count.
            /// </summary>
            /// <value>The repeated row count.</value>
            public int RepeatRowCount
            {
                get
                {
                    if (!this.HasRepeatRow)
                    {
                        return 0;
                    }
                    return ((this.RepeatRowEndIndex - this.RepeatRowStartIndex) + 1);
                }
            }

            /// <summary>
            /// Gets or sets the end index of the repeated row.
            /// </summary>
            /// <value>The end index of the repeated row.</value>
            public int RepeatRowEndIndex
            {
                get { return  this.repeatRowEndIndex; }
                set { this.repeatRowEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the repeated row heights.
            /// </summary>
            /// <value>The repeated row heights.</value>
            public PartLayoutData RepeatRowHeights
            {
                get { return  this.repeatRowHeights; }
                set { this.repeatRowHeights = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the repeated row.
            /// </summary>
            /// <value>The start index of the repeated row.</value>
            public int RepeatRowStartIndex
            {
                get { return  this.repeatRowStartIndex; }
                set { this.repeatRowStartIndex = value; }
            }

            /// <summary>
            /// Gets the row count.
            /// </summary>
            /// <value>The row count.</value>
            public int RowCount
            {
                get { return  ((this.RowEndIndex - this.RowStartIndex) + 1); }
            }

            /// <summary>
            /// Gets or sets the end index of the row.
            /// </summary>
            /// <value>The end index of the row.</value>
            public int RowEndIndex
            {
                get { return  this.rowEndIndex; }
                set { this.rowEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the row footer spans.
            /// </summary>
            /// <value>The row footer spans.</value>
            public SpanLayoutData RowFooterSpans
            {
                get { return  this.rowFooterSpans; }
                set { this.rowFooterSpans = value; }
            }

            /// <summary>
            /// Gets or sets the row footer widths.
            /// </summary>
            /// <value>The row footer widths.</value>
            public PartLayoutData RowFooterWidths
            {
                get { return  this.rowFooterWidths; }
                set { this.rowFooterWidths = value; }
            }

            /// <summary>
            /// Gets or sets the row header spans.
            /// </summary>
            /// <value>The row header spans.</value>
            public SpanLayoutData RowHeaderSpans
            {
                get { return  this.rowHeaderSpans; }
                set { this.rowHeaderSpans = value; }
            }

            /// <summary>
            /// Gets or sets the row header widths.
            /// </summary>
            /// <value>The row header widths.</value>
            public PartLayoutData RowHeaderWidths
            {
                get { return  this.rowHeaderWidths; }
                set { this.rowHeaderWidths = value; }
            }

            /// <summary>
            /// Gets or sets the row heights.
            /// </summary>
            /// <value>The row heights.</value>
            public PartLayoutData RowHeights
            {
                get { return  this.rowHeights; }
                set { this.rowHeights = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the row.
            /// </summary>
            /// <value>The start index of the row</value>
            public int RowStartIndex
            {
                get { return  this.rowStartIndex; }
                set { this.rowStartIndex = value; }
            }

            /// <summary>
            /// Gets the shapes.
            /// </summary>
            /// <value>The shapes.</value>
            public Dictionary<Rect, List<Shape>> Shapes
            {
                get { return  this.shapes; }
            }

            /// <summary>
            /// Gets a value that indicates whether to show the border.
            /// </summary>
            /// <value><c>true</c> to show the border; otherwise, <c>false</c>.</value>
            public bool ShowBorder
            {
                get { return  this.settings.ShowBorder; }
            }

            /// <summary>
            /// Gets a value that indicates whether to show the grid line.
            /// </summary>
            /// <value><c>true</c> to show the grid line; otherwise, <c>false</c>.</value>
            public bool ShowGridLine
            {
                get { return  this.settings.ShowGridLine; }
            }

            /// <summary>
            /// Gets the sticky notes.
            /// </summary>
            /// <value>The sticky notes.</value>
            public Dictionary<Rect, List<StickyNote>> StickyNotes
            {
                get { return  this.stickyNotes; }
            }

            /// <summary>
            /// Gets or sets the top left corner spans.
            /// </summary>
            /// <value>The top left corner spans.</value>
            public SpanLayoutData TopLeftCornerSpans
            {
                get { return  this.topLeftCornerSpans; }
                set { this.topLeftCornerSpans = value; }
            }

            /// <summary>
            /// Gets or sets the top right corner spans.
            /// </summary>
            /// <value>The top right corner spans.</value>
            public SpanLayoutData TopRightCornerSpans
            {
                get { return  this.topRightCornerSpans; }
                set { this.topRightCornerSpans = value; }
            }
        }
    }
}

