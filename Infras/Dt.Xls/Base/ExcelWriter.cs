#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Dt.Xls
{
    internal class ExcelWriter : IExcelWriter, IExcelSparklineWriter, IExcelWriter2, IExcelTableWriter, IExcelChartWriter
    {
        private IExcelWorkbook _workbook;

        public ExcelWriter(IExcelWorkbook workbook)
        {
            if (workbook == null)
            {
                throw new ArgumentNullException("workbook");
            }
            this._workbook = workbook;
        }

        public void Finish()
        {
        }

        public IExcelAutoFilter GetAutoFilter(short sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                return this._workbook.Worksheets[sheet].AutoFilter;
            }
            return null;
        }

        public List<IBuiltInName> GetBuiltInNameList()
        {
            return this._workbook.BuiltInNameList;
        }

        public bool GetCalculationMode(short sheet, ref short autoRecalc)
        {
            autoRecalc = (short) this._workbook.CalculationMode;
            return true;
        }

        public ICalculationProperty GetCalculationProperty()
        {
            return new CalculationProperty { CalculationMode = this._workbook.CalculationMode, IsFullPrecision = this._workbook.PrecisionAsDisplay, IsIterateCalculate = this._workbook.IsIterataCalculate, MaximunChange = this._workbook.MaximumChange, MaxIterationCount = this._workbook.MaximumIterations, ReCalculationBeforeSave = this._workbook.RecalculateBeforeSave, RefMode = this._workbook.ReferenceStyle };
        }

        public bool GetCells(short sheet, List<IExcelCell> cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException("cells");
            }
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet == null)
            {
                return false;
            }
            List<IExcelRow> nonEmptyRows = worksheet.GetNonEmptyRows();
            for (int i = 0; i < nonEmptyRows.Count; i++)
            {
                IExcelRow row = nonEmptyRows[i];
                List<IExcelCell> nonEmptyCells = worksheet.GetNonEmptyCells(row.Index);
                if (nonEmptyCells.Count > 0)
                {
                    cells.AddRange((IEnumerable<IExcelCell>) nonEmptyCells);
                }
            }
            return true;
        }

        public object GetCellValue(short sheet, int row, int column)
        {
            if (this.ValidateWorkbook(sheet))
            {
                IExcelCell cell = this._workbook.Worksheets[sheet].GetCell(row, column, false);
                if (cell != null)
                {
                    return cell.Value;
                }
            }
            return null;
        }

        public Dictionary<int, GcColor> GetColorPalette()
        {
            return this._workbook.ColorPalette;
        }

        public bool GetColumnPageBreaks(short sheet, List<short> pageBreaks)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            ushort num = 0;
            if (this._workbook.Operator.ExportedExcelVersion == ExcelVersion.Excel2007)
            {
                num = (ushort) Math.Min(0x4000, worksheet.ColumnCount);
            }
            else
            {
                num = (ushort) Math.Min(0x100, worksheet.ColumnCount);
            }
            for (short i = 0; i < num; i++)
            {
                IExcelColumn column = worksheet.GetColumn(i, false);
                if ((column != null) && column.PageBreak)
                {
                    pageBreaks.Add(i);
                }
            }
            return true;
        }

        public List<IExcelConditionalFormat> GetConditionalFormat(int sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                return this._workbook.Worksheets[sheet].ConditionalFormat;
            }
            return null;
        }

        public List<IFunction> GetCustomOrFunctionNameList()
        {
            return this._workbook.CustomOrFunctionNameList;
        }

        public double GetDefaultColumnWidth(short sheet)
        {
            return this._workbook.Worksheets[sheet].DefaultColumnWidth;
        }

        public string GetDefaultPivotTableStyleName()
        {
            return this._workbook.DefaultPivotTableStyleName;
        }

        public double GetDefaultRowHeight(short sheet, ref bool customHeight)
        {
            customHeight = false;
            return this._workbook.Worksheets[sheet].DefaultRowHeight;
        }

        public string GetDefaultTableStyleName()
        {
            return this._workbook.DefaultTableStyleName;
        }

        public List<IDifferentialFormatting> GetDifferentialFormattingRecords()
        {
            return (this._workbook.DifferentialFormattings ?? new List<IDifferentialFormatting>());
        }

        public void GetDimensions(short sheet, ref int row, ref int column)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (this._workbook.Operator.ExportedExcelVersion == ExcelVersion.Excel2003)
            {
                column = worksheet.ColumnCount;
                row = worksheet.RowCount;
            }
            else if (worksheet is ExcelWorksheet)
            {
                column = (worksheet as ExcelWorksheet).ActualColumnCount;
                row = (worksheet as ExcelWorksheet).ActualRowCount;
            }
            else
            {
                column = worksheet.ColumnCount;
                row = worksheet.RowCount;
            }
        }

        public void GetDisplayElements(short sheet, ref bool dispFormula, ref bool dispZeros, ref bool dispGrid, ref bool dispRowColHdr, ref bool rightToLeftColumns)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            dispGrid = worksheet.ShowGridLines;
            dispRowColHdr = worksheet.ShowHeaders;
            dispFormula = worksheet.ShowFormulas;
            dispZeros = worksheet.ShowZeros;
            rightToLeftColumns = worksheet.RightToLeftColumns;
        }

        public List<IExtendedFormat> GetExcelCellFormats()
        {
            List<IExtendedFormat> list = new List<IExtendedFormat>();
            foreach (KeyValuePair<int, IExtendedFormat> pair in this._workbook.ExcelCellFormats)
            {
                list.Add(pair.Value);
            }
            return list;
        }

        public IExtendedFormat GetExcelDefaultCellFormat()
        {
            return this._workbook.DefaultCellFormat;
        }

        public List<IExcelSparklineGroup> GetExcelSparkLineGroups(int sheet)
        {
            return this._workbook.Worksheets[sheet].SparklineGroups;
        }

        public List<IExcelStyle> GetExcelStyles()
        {
            List<IExcelStyle> list = new List<IExcelStyle>();
            foreach (IExcelStyle style in this._workbook.ExcelStyles)
            {
                list.Add(style);
            }
            return list;
        }

        public List<IExternalWorkbookInfo> GetExternWorkbookInfo()
        {
            return this._workbook.ExternWorkbooks;
        }

        public void GetFrozen(short sheet, ref int frozenRowCount, ref int frozenColumnCount, ref int frozenTrailingRowCount, ref int frozenTrailingColumnCount)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            frozenRowCount = worksheet.FrozenRowCount;
            frozenColumnCount = worksheet.FrozenColumnCount;
        }

        public IExcelColor GetGridlineColor(short sheet)
        {
            if (!this.ValidateWorkbook(sheet))
            {
                return null;
            }
            IExcelColor gridLineColor = this._workbook.Worksheets[sheet].GridLineColor;
            if (gridLineColor == null)
            {
                return null;
            }
            if (gridLineColor.ColorType == ExcelColorType.Indexed)
            {
                return gridLineColor;
            }
            return new ExcelColor(ExcelColorType.Indexed, (uint) this._workbook.GetPaletteColor(gridLineColor), 0.0);
        }

        public void GetGutters(short sheet, ref int iLevelRwMac, ref int iLevelColMac)
        {
            if (this.ValidateWorkbook(sheet))
            {
                IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
                iLevelRwMac = worksheet.RowMaxOutlineLevel;
                iLevelColMac = worksheet.ColumnMaxOutlineLevel;
            }
        }

        public bool GetHorizontalCenter(short sheet, ref bool center)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet.PrintSettings != null)
            {
                center = worksheet.PrintSettings.Options.HorizontalCentered;
            }
            return true;
        }

        public List<IName> GetInternalDefinedNames()
        {
            List<IName> list = new List<IName>();
            list.AddRange((IEnumerable<IName>) this._workbook.NamedCellRanges.Values);
            for (int i = 0; i < this._workbook.Worksheets.Count; i++)
            {
                list.AddRange((IEnumerable<IName>) this._workbook.Worksheets[i].NamedCellRanges.Values);
            }
            return list;
        }

        public List<IRange> GetMergedCells(short sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                return this._workbook.Worksheets[sheet].MergedCells;
            }
            return null;
        }

        public List<IExcelColumn> GetNonEmptyColumns(short sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                return this._workbook.Worksheets[sheet].GetNonEmptyColumns();
            }
            return null;
        }

        public List<IExcelRow> GetNonEmptyRows(short sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
                return worksheet.GetNonEmptyRows();
            }
            return null;
        }

        public void GetOutlineDirection(int sheet, ref bool summaryColumnsRightToDetail, ref bool summaryRowsBelowDetail)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            summaryColumnsRightToDetail = worksheet.SummaryColumnsToRightOfDetail;
            summaryRowsBelowDetail = worksheet.SummaryRowsBelowDetail;
        }

        public ExcelPaletteColor GetPaletteColor(IExcelColor color)
        {
            if (color == null)
            {
                return ExcelPaletteColor.SystemWindowTextColor;
            }
            return this._workbook.GetPaletteColor(color);
        }

        public void GetPane(short sheet, ref int x, ref int y, ref int rwTop, ref int columnLeft, ref int activePane)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            ExcelWorksheet.ExcelViewportInfo defaultViewport = worksheet.DefaultViewport;
            if (defaultViewport != null)
            {
                double num = 0.0;
                double num2 = 0.0;
                int rowPaneCount = defaultViewport.RowPaneCount;
                int columnPaneCount = defaultViewport.ColumnPaneCount;
                if ((rowPaneCount == 2) || (columnPaneCount == 2))
                {
                    bool flag = true;
                    bool flag2 = true;
                    if (rowPaneCount == 2)
                    {
                        if (defaultViewport.ActiveRowPane == 1)
                        {
                            flag = false;
                        }
                        if (defaultViewport.PreferredHeight[0] == -1)
                        {
                            y = 0x12;
                        }
                        else
                        {
                            y = defaultViewport.PreferredHeight[0] + 0x12;
                        }
                        num = (Convert.ToDouble((int) y, (IFormatProvider) CultureInfo.InvariantCulture) * 1440.0) / 96.0;
                        y = Convert.ToInt32((double) num, (IFormatProvider) CultureInfo.InvariantCulture);
                        rwTop = defaultViewport.TopRow[1];
                    }
                    else
                    {
                        y = 0;
                    }
                    if (columnPaneCount == 2)
                    {
                        if (defaultViewport.ActiveColumnPane == 1)
                        {
                            flag2 = false;
                        }
                        if (defaultViewport.PreferredWidth[0] == -1)
                        {
                            x = 0x22;
                        }
                        else
                        {
                            x = defaultViewport.PreferredWidth[0] + 0x22;
                        }
                        num2 = (Convert.ToDouble((int) x, (IFormatProvider) CultureInfo.InvariantCulture) * 1440.0) / 96.0;
                        x = Convert.ToInt32((double) num2, (IFormatProvider) CultureInfo.InvariantCulture);
                        columnLeft = defaultViewport.LeftColumn[1];
                    }
                    else
                    {
                        x = 0;
                    }
                    if (flag)
                    {
                        if (flag2)
                        {
                            activePane = 3;
                        }
                        else
                        {
                            activePane = 1;
                        }
                    }
                    else if (flag2)
                    {
                        activePane = 2;
                    }
                    else
                    {
                        activePane = 0;
                    }
                }
            }
        }

        public string GetPrintArea(int sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                return this._workbook.Worksheets[sheet].PrintArea;
            }
            return null;
        }

        public bool GetPrintHeaders(short sheet, ref bool print)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet.PrintSettings != null)
            {
                return false;
            }
            return true;
        }

        public IExcelPrintOptions GetPrintOptions(short sheet)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            return worksheet.PrintSettings.Options;
        }

        public IExcelPrintPageMargin GetPrintPageMargin(short sheet)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            return worksheet.PrintSettings.Margin;
        }

        public IExcelPrintPageSetting GetPrintPageSetting(short sheet)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            return worksheet.PrintSettings.PageSetting;
        }

        public string GetPrintTitle(int sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                return this._workbook.Worksheets[sheet].PrintTitle;
            }
            return null;
        }

        public void GetProtect(short sheet, ref bool isProtect)
        {
            if (sheet == -1)
            {
                isProtect = this._workbook.Locked;
            }
            else
            {
                isProtect = this._workbook.Worksheets[sheet].IsLocked;
            }
        }

        public bool GetRowPageBreaks(short sheet, List<short> pageBreaks)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            ushort num = (ushort) Math.Min(0xffff, worksheet.RowCount);
            for (short i = 0; i < num; i++)
            {
                IExcelRow row = worksheet.GetRow(i, false);
                if ((row != null) && row.PageBreak)
                {
                    pageBreaks.Add(i);
                }
            }
            return true;
        }

        public void GetScroll(ref bool horizontalScroll, ref bool verticalScroll)
        {
            horizontalScroll = this.NeedScroll(this._workbook.HorizontalScrollBarPolicy);
            verticalScroll = this.NeedScroll(this._workbook.VerticalScrollBarPolicy);
        }

        public bool GetSelectionList(short sheet, List<GcRect> selectionList, ref GcPoint activeCell, ref PaneType paneIndex)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            int frozenRowCount = worksheet.FrozenRowCount;
            int frozenColumnCount = worksheet.FrozenColumnCount;
            activeCell.X = worksheet.ActiveColumnIndex;
            activeCell.Y = worksheet.ActiveRowIndex;
            ExcelWorksheet.ExcelViewportInfo defaultViewport = worksheet.DefaultViewport;
            if (defaultViewport != null)
            {
                int rowPaneCount = defaultViewport.RowPaneCount;
                int columnPaneCount = defaultViewport.ColumnPaneCount;
                if ((paneIndex != PaneType.TopLeft) && ((rowPaneCount > 2) || (columnPaneCount > 2)))
                {
                    return false;
                }
                if (paneIndex == PaneType.BottomRight)
                {
                    if (((rowPaneCount <= 1) || (columnPaneCount <= 1)) && ((frozenRowCount <= 0) || (frozenColumnCount <= 0)))
                    {
                        return false;
                    }
                }
                else if (paneIndex == PaneType.TopRight)
                {
                    if ((columnPaneCount <= 1) && (frozenColumnCount <= 0))
                    {
                        return false;
                    }
                }
                else if (paneIndex == PaneType.BottomLeft)
                {
                    if ((rowPaneCount <= 1) && (frozenRowCount <= 0))
                    {
                        return false;
                    }
                }
                else
                {
                    int num1 = (int) paneIndex;
                }
                if (selectionList != null)
                {
                    if ((worksheet.Selections != null) && (worksheet.Selections.Count > 0))
                    {
                        foreach (ISelectionRange range in worksheet.Selections)
                        {
                            if (range.activePaneType == paneIndex)
                            {
                                GcRect rect = new GcRect((range.Column > 0) ? ((double) range.Column) : ((double) 0), (range.Row > 0) ? ((double) range.Row) : ((double) 0), (range.ColumnSpan > 0) ? ((double) range.ColumnSpan) : ((double) 0x4000), (range.RowSpan > 0) ? ((double) range.RowSpan) : ((double) 0x100000));
                                selectionList.Add(rect);
                            }
                        }
                    }
                    else
                    {
                        selectionList.Add(new GcRect(activeCell.X, activeCell.Y, 1.0, 1.0));
                    }
                }
            }
            return true;
        }

        public int GetSheetCount()
        {
            if (this._workbook.Worksheets != null)
            {
                return this._workbook.Worksheets.Count;
            }
            return 0;
        }

        public string GetSheetName(int sheetIndex)
        {
            if (this.ValidateWorkbook(sheetIndex))
            {
                return this._workbook.Worksheets[sheetIndex].Name;
            }
            return null;
        }

        public IExcelColor GetSheetTabColor(int sheet)
        {
            return this._workbook.Worksheets[sheet].SheetTabColor;
        }

        public List<IExcelTable> GetSheetTables(int sheetIndex)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheetIndex];
            return worksheet.GetSheetTables();
        }

        public List<IExcelTableStyle> GetTableStyles()
        {
            return this._workbook.TableStyles;
        }

        public bool GetTabs(ref bool dispTabs, ref int tabCur, ref int tabFirst, ref int tabSel, ref int tabRatio)
        {
            switch (this._workbook.TabStripPolicy)
            {
                case ExcelTabStripPolicy.Always:
                case ExcelTabStripPolicy.AsNeeded:
                    dispTabs = true;
                    break;

                default:
                    dispTabs = false;
                    break;
            }
            tabRatio = Convert.ToInt32((double) (this._workbook.TabStripRatio * 1000.0));
            tabCur = this._workbook.ActiveSheetIndex;
            tabFirst = this._workbook.FirstDisplayedTabIndex;
            tabSel = this._workbook.SelectedTabCount = 1;
            return true;
        }

        public IExcelTheme GetTheme()
        {
            return this._workbook.Theme;
        }

        public bool GetTopLeft(short sheet, ref int row, ref int column)
        {
            this._workbook.Worksheets[sheet].GetTopLeft(ref row, ref column);
            return true;
        }

        public List<IExcelDataValidation> GetValidationData(short sheet)
        {
            if (this.ValidateWorkbook(sheet))
            {
                return this._workbook.Worksheets[sheet].GetDataValidations();
            }
            return null;
        }

        public bool GetVerticalCenter(short sheet, ref bool center)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet.PrintSettings != null)
            {
                center = worksheet.PrintSettings.Options.VerticalCentered;
            }
            return true;
        }

        public IExcelRect GetWindow(ref bool hidden, ref bool iconic)
        {
            ExcelRect rect = null;
            if (this._workbook.ExcelRect != null)
            {
                rect = new ExcelRect(this._workbook.ExcelRect.Left, this._workbook.ExcelRect.Top, this._workbook.ExcelRect.Width, this._workbook.ExcelRect.Height);
            }
            hidden = this._workbook.IsWindowHidden;
            iconic = this._workbook.IsWindowDisplayAsIcon;
            return rect;
        }

        public IExcelWorkbookPropery GetWorkbookProperty()
        {
            return new ExcelWorkbookPropery { IsDate1904 = this._workbook.Is1904Date, SaveExternalLinks = this._workbook.SaveExternalLinks };
        }

        public List<IExcelChart> GetWorksheetCharts(int sheetIndex)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheetIndex];
            return worksheet.GetSheetCharts();
        }

        public List<IExcelImage> GetWorkSheetImages(int sheetIndex)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheetIndex];
            return worksheet.GetSheetImages();
        }

        public void GetZoom(short sheet, ref float zoom)
        {
            if (this.ValidateWorkbook(sheet))
            {
                zoom = this._workbook.Worksheets[sheet].Zoom;
            }
        }

        public bool IsCalculationError(object value, ref short errorVal)
        {
            ExcelCalcError error = value as ExcelCalcError;
            if (error != null)
            {
                errorVal = 0xff;
                switch (error.ToString().ToUpperInvariant())
                {
                    case "#CIRCLE!":
                        errorVal = 0;
                        break;

                    case "#DIV/0!":
                        errorVal = 7;
                        break;

                    case "#NAME?":
                        errorVal = 0x1d;
                        break;

                    case "#N/A":
                        errorVal = 0x2a;
                        break;

                    case "#NULL!":
                        errorVal = 0;
                        break;

                    case "#NUM!":
                        errorVal = 0x24;
                        break;

                    case "#REF!":
                        errorVal = 0x17;
                        break;

                    case "#VALUE!":
                        errorVal = 15;
                        break;

                    default:
                        throw new ExcelException(ResourceHelper.GetResourceString("incorrectErrorCodeError"), ExcelExceptionCode.IncorrectErrorCode);
                }
                if (errorVal != 0xff)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSheetHidden(int sheetIndex)
        {
            return (this.ValidateWorkbook(sheetIndex) && !this._workbook.Worksheets[sheetIndex].IsVisible);
        }

        private bool NeedScroll(ExcelScrollBarPolicy policy)
        {
            if ((policy != ExcelScrollBarPolicy.AsNeeded) && (policy != ExcelScrollBarPolicy.Always))
            {
                return false;
            }
            return true;
        }

        public void OnExcelSaveError(ExcelWarning excelWarning)
        {
        }

        private bool ValidateWorkbook(int sheet)
        {
            if (this._workbook.Worksheets == null)
            {
                return false;
            }
            return ((sheet >= 0) && (sheet < this._workbook.Worksheets.Count));
        }
    }
}

