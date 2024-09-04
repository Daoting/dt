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
    internal class ExcelReader : IExcelReader, IExcelReader2, IExcelSparklineReader, IExcelTableReader, IExcelChartReader
    {
        private IExcelWorkbook _workbook;

        public ExcelReader(IExcelWorkbook workbook)
        {
            if (workbook == null)
            {
                throw new ArgumentNullException("workbook");
            }
            this._workbook = workbook;
        }

        public void AddSheet(string sheetName, byte hiddenState, ExcelSheetType sheetType)
        {
            if (sheetName == null)
            {
                throw new ArgumentNullException("sheetName");
            }
            ExcelWorksheet worksheet = new ExcelWorksheet(sheetName, 0x10000, 0x100) {
                IsVisible = hiddenState == 0
            };
            this._workbook.Worksheets.Add(worksheet);
        }

        public void Bof(short sheet)
        {
            this._workbook.ActivePaneIndex = 3;
        }

        public void Finish()
        {
        }

        public void OnExcelLoadError(ExcelWarning excelWarning)
        {
        }

        public void SetArrayFormula(int sheet, int rowFirst, int rowLast, short colFirst, short colLast, IExcelFormula arrayFormula)
        {
            if (arrayFormula == null)
            {
                throw new ArgumentNullException("arrayFormula");
            }
            if (!arrayFormula.IsArrayFormula)
            {
                throw new ArgumentException(ResourceHelper.GetResourceString("arrayFormulaError"));
            }
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            ExcelCellRange range = new ExcelCellRange {
                Row = rowFirst,
                RowSpan = (rowLast - rowFirst) + 1,
                Column = colFirst,
                ColumnSpan = (colLast - colFirst) + 1
            };
            for (int i = rowFirst; i <= rowLast; i++)
            {
                for (int j = colFirst; j <= colLast; j++)
                {
                    IExcelCell cell = worksheet.GetCell(i, j, true);
                    cell.CellFormula = arrayFormula;
                    cell.CellFormula.ArrayFormulaRange = range;
                    cell.IsArrayFormula = true;
                    cell.CellType = CellType.Array;
                }
            }
        }

        public void SetAutoFilter(short sheet, IExcelAutoFilter autoFilter)
        {
            if ((autoFilter.FilterColumns != null) && (autoFilter.FilterColumns.Count != 0))
            {
                this._workbook.Worksheets[sheet].AutoFilter = autoFilter;
            }
        }

        public void SetBuiltInNameList(List<IBuiltInName> builtInNames)
        {
            this._workbook.BuiltInNameList = new List<IBuiltInName>((IEnumerable<IBuiltInName>) builtInNames);
        }

        public void SetCalculationProperty(ICalculationProperty calcProperty)
        {
            if (calcProperty == null)
            {
                throw new ArgumentNullException("calProperty");
            }
            this._workbook.ReferenceStyle = calcProperty.RefMode;
            this._workbook.CalculationMode = calcProperty.CalculationMode;
            this._workbook.MaximumChange = calcProperty.MaximunChange;
            this._workbook.IsIterataCalculate = calcProperty.IsIterateCalculate;
            this._workbook.MaximumIterations = calcProperty.MaxIterationCount;
            this._workbook.PrecisionAsDisplay = calcProperty.IsFullPrecision;
            this._workbook.RecalculateBeforeSave = calcProperty.ReCalculationBeforeSave;
        }

        public bool SetCell(short sheet, int row, int column, object value, CellType type, int ixf, IExcelFormula cellFormula)
        {
            IExcelCell cell = this._workbook.Worksheets[sheet].GetCell(row, column, true);
            if (cell == null)
            {
                return false;
            }
            if (value != null)
            {
                cell.Value = value;
            }
            cell.SetFormatId(ixf);
            cell.CellType = type;
            if ((cellFormula != null) && ((cell.CellFormula == null) || !cell.CellFormula.IsArrayFormula))
            {
                cell.CellFormula = cellFormula;
            }
            return true;
        }

        public bool SetCellFormula(short sheet, int row, int column, IExcelFormula cellFormula)
        {
            if (cellFormula == null)
            {
                throw new ArgumentNullException("cellFormula");
            }
            IExcelCell cell = this._workbook.Worksheets[sheet].GetCell(row, column, true);
            if (cell != null)
            {
                cell.CellFormula = cellFormula;
                return true;
            }
            return false;
        }

        public bool SetCellHyperLink(short sheet, int row, int column, IExcelHyperLink hyperLink)
        {
            if (hyperLink == null)
            {
                throw new ArgumentNullException("hyperLink");
            }
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            IExcelCell cell = worksheet[row, column];
            if (cell == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(hyperLink.Description))
            {
                hyperLink.Description = cell.Value.ToString();
            }
            cell.Hyperlink = hyperLink;
            return true;
        }

        public void SetCellNote(short sheet, int row, int column, bool stickyNote, string note)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            worksheet[row, column].Note = note;
            if (stickyNote)
            {
                worksheet[row, column].NoteStyle = ExcelNoteStyle.StickyNote;
            }
        }

        public bool SetCellStyle(short sheet, int row, int column, int ixf, CellType type)
        {
            IExcelCell cell = this._workbook.Worksheets[sheet].GetCell(row, column, true);
            if (cell != null)
            {
                cell.CellType = type;
                cell.SetFormatId(ixf);
                return true;
            }
            return false;
        }

        public bool SetCellValue(short sheet, int row, int column, object value)
        {
            IExcelCell cell = this._workbook.Worksheets[sheet].GetCell(row, column, true);
            if (cell != null)
            {
                cell.Value = value;
                return true;
            }
            return false;
        }

        public void SetColorPalette(Dictionary<int, GcColor> palette)
        {
            if (palette == null)
            {
                throw new ArgumentNullException("palette");
            }
            if (palette.Count != 0x40)
            {
                throw new ArgumentException(ResourceHelper.GetResourceString("colorPaletteCountError"));
            }
            this._workbook.ColorPalette = palette;
        }

        public void SetColumnInfo(short sheet, short colFirst, short colLast, short ixf, double width, bool hidden, byte outlineLevel, bool collapsed)
        {
            if (colFirst < 0)
            {
                throw new ArgumentOutOfRangeException("");
            }
            if (colLast < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (width < 0.0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (colLast < colFirst)
            {
                throw new ArgumentOutOfRangeException();
            }
            IExcelWorksheet workSheet = this._workbook.Worksheets[sheet];
            workSheet.ColumnCount = Math.Max(workSheet.ColumnCount, colLast);
            for (int i = colFirst; i <= colLast; i++)
            {
                ExcelColumn column = new ExcelColumn(workSheet) {
                    Index = i,
                    Width = width
                };
                column.SetFormatId(ixf);
                column.Visible = !hidden;
                column.Collapsed = collapsed;
                column.OutLineLevel = outlineLevel;
                workSheet.SetColumn(i, column);
            }
        }

        public void SetConditionalFormatting(short sheet, IExcelConditionalFormat conditionalFormat)
        {
            if (conditionalFormat == null)
            {
                throw new ArgumentNullException("conditionalFormat");
            }
            this._workbook.Worksheets[sheet].ConditionalFormat.Add(conditionalFormat);
        }

        public void SetCustomOrFunctionNameList(List<IFunction> customOrFunctionNames)
        {
            this._workbook.CustomOrFunctionNameList = new List<IFunction>((IEnumerable<IFunction>) customOrFunctionNames);
        }

        public void SetDefaultColumnWidth(short sheet, double baseColumnWidth, double? defaultColumnWidth)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (defaultColumnWidth.HasValue && defaultColumnWidth.HasValue)
            {
                worksheet.DefaultColumnWidth = defaultColumnWidth.Value;
            }
            else
            {
                worksheet.DefaultColumnWidth = baseColumnWidth;
            }
        }

        public void SetDefaultRowHeight(short sheet, double height)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            worksheet.DefaultRowHeight = height;
        }

        public void SetDifferentialFormattingRecord(List<IDifferentialFormatting> dxfRecords)
        {
            this._workbook.DifferentialFormattings = new List<IDifferentialFormatting>((IEnumerable<IDifferentialFormatting>) dxfRecords);
        }

        public void SetDimensions(short sheet, int rowFirst, int rowLast, short columnFirst, short columnLast)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            switch (this._workbook.Operator.ImportedExcelVersion)
            {
                case ExcelVersion.Excel2007:
                    worksheet.RowCount = Math.Max(rowLast, 0x100000);
                    worksheet.ColumnCount = Math.Max((int)columnLast, 0x4000);
                    if (worksheet is ExcelWorksheet)
                    {
                        (worksheet as ExcelWorksheet).ActualRowCount = columnLast;
                        (worksheet as ExcelWorksheet).ActualRowCount = rowLast;
                    }
                    return;
            }
            worksheet.RowCount = Math.Max(rowLast, 0x10000);
            worksheet.ColumnCount = Math.Max((int)columnLast, 0x100);
            if (worksheet is ExcelWorksheet)
            {
                (worksheet as ExcelWorksheet).ActualRowCount = rowLast;
                (worksheet as ExcelWorksheet).ActualRowCount = columnLast;
            }
        }

        public void SetDisplayElements(short sheet, bool dispFormula, bool dispZeros, bool showGridLine, bool dispRowColHdr, bool rightToLeftColumns)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            worksheet.ShowGridLines = showGridLine;
            worksheet.ShowHeaders = dispRowColHdr;
            worksheet.ShowFormulas = dispFormula;
            worksheet.ShowZeros = dispZeros;
            worksheet.RightToLeftColumns = rightToLeftColumns;
        }

        public void SetExcelCellFormat(IExtendedFormat style)
        {
            Dictionary<int, IExtendedFormat> excelCellFormats = this._workbook.ExcelCellFormats;
            excelCellFormats.Add(excelCellFormats.Keys.Count, style);
        }

        public void SetExcelChart(int sheetIndex, IExcelChart chart)
        {
            if (chart != null)
            {
                this._workbook.Worksheets[sheetIndex].SetSheetChart(chart);
            }
        }

        public void SetExcelDefaultCellFormat(IExtendedFormat style)
        {
            this._workbook.DefaultCellFormat = style;
        }

        public void SetExcelImage(int sheetIndex, IExcelImage excelImage)
        {
            if (excelImage != null)
            {
                this._workbook.Worksheets[sheetIndex].SetSheetImage(excelImage);
            }
        }

        public void SetExcelSheetTabColor(int sheet, IExcelColor color)
        {
            if (color != null)
            {
                IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
                worksheet.SheetTabColor = color;
            }
        }

        public void SetExcelSparklineGroups(int sheet, List<IExcelSparklineGroup> excelSparklineGroups)
        {
            if ((excelSparklineGroups != null) && (excelSparklineGroups.Count > 0))
            {
                this._workbook.Worksheets[sheet].SparklineGroups = excelSparklineGroups;
            }
        }

        public void SetExcelStyle(IExcelStyle style)
        {
            if (style.IsBuiltInStyle)
            {
                ExcelStyle style2 = style as ExcelStyle;
                for (int i = 0; i < this._workbook.ExcelStyles.Count; i++)
                {
                    if (this._workbook.ExcelStyles[i].IsBuiltInStyle)
                    {
                        ExcelStyle style3 = this._workbook.ExcelStyles[i] as ExcelStyle;
                        if (style3.BuiltInStyle == style2.BuiltInStyle)
                        {
                            style2.Name = style3.Name;
                            break;
                        }
                    }
                }
            }
            foreach (IExcelStyle style4 in this._workbook.ExcelStyles)
            {
                if (style4.Name == style.Name)
                {
                    if (style4.IsBuiltInStyle)
                    {
                        ExcelStyle style5 = style as ExcelStyle;
                        if (style5 != null)
                        {
                            style4.Format = style5.Format;
                            (style4 as ExcelStyle).Category = style5.Category;
                            (style4 as ExcelStyle).BuiltInStyle = style5.BuiltInStyle;
                            (style4 as ExcelStyle).OutLineLevel = style5.OutLineLevel;
                            (style4 as ExcelStyle).IsCustomBuiltIn = style5.IsCustomBuiltIn;
                        }
                    }
                    else if (style is CustomExcelStyle)
                    {
                        style4.Format = style.Format;
                    }
                    return;
                }
            }
            this._workbook.ExcelStyles.Add(style);
        }

        public void SetExcelWorkbookProperty(IExcelWorkbookPropery property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            this._workbook.Is1904Date = property.IsDate1904;
            this._workbook.SaveExternalLinks = property.SaveExternalLinks;
        }

        public void SetExternalReferencedWorkbookInfo(IExternalWorkbookInfo externWorkbookInfo)
        {
            this._workbook.ExternWorkbooks.Add(externWorkbookInfo);
        }

        public void SetGridlineColor(short sheet, IExcelColor color)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            if (color.ColorType != ExcelColorType.Indexed)
            {
                throw new ArgumentException(ResourceHelper.GetResourceString("gridlineColorTypeError"));
            }
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            worksheet.GridLineColor = color;
        }

        public bool SetMergeCells(short sheet, int rowStart, int rowEnd, int columnStart, int columnEnd)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet.RowCount <= rowEnd)
            {
                worksheet.RowCount = rowEnd + 1;
            }
            if (worksheet.ColumnCount <= columnEnd)
            {
                worksheet.ColumnCount = columnEnd + 1;
            }
            ExcelCellRange range = new ExcelCellRange {
                Row = rowStart,
                Column = columnStart,
                RowSpan = (rowEnd - rowStart) + 1,
                ColumnSpan = (columnEnd - columnStart) + 1
            };
            worksheet.MergedCells.Add(range);
            return true;
        }

        public void SetNamedCellRange(IName cellRange)
        {
            if (cellRange == null)
            {
                throw new ArgumentNullException("cellRange");
            }
            if (cellRange.Index == -1)
            {
                this._workbook.NamedCellRanges.Add(cellRange.Name, cellRange);
            }
            else if ((this._workbook.Worksheets.Count > cellRange.Index) && !this._workbook.Worksheets[cellRange.Index].NamedCellRanges.ContainsKey(cellRange.Name))
            {
                this._workbook.Worksheets[cellRange.Index].NamedCellRanges.Add(cellRange.Name, cellRange);
            }
        }

        public void SetOutlineDirection(int sheet, bool summaryColumnsRightToDetail, bool summaryRowsBelowDetail)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            worksheet.SummaryColumnsToRightOfDetail = summaryColumnsRightToDetail;
            worksheet.SummaryRowsBelowDetail = summaryRowsBelowDetail;
        }

        public void SetPane(short sheet, int x, int y, int rwTop, int columnLeft, int pnnAct, bool isPanesFrozen)
        {
            this._workbook.ActivePaneIndex = pnnAct;
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (isPanesFrozen)
            {
                worksheet.FrozenColumnCount = x;
                worksheet.FrozenRowCount = y;
            }
            else
            {
                int num = 0;
                int num2 = 0;
                if (x > 0)
                {
                    double num3 = (Convert.ToDouble((int) x, (IFormatProvider) CultureInfo.InvariantCulture) * 96.0) / 1440.0;
                    num3 -= 34.0;
                    num = Convert.ToInt32((double) num3, (IFormatProvider) CultureInfo.InvariantCulture);
                }
                if (y > 0)
                {
                    double num4 = (Convert.ToDouble((int) y, (IFormatProvider) CultureInfo.InvariantCulture) * 96.0) / 1440.0;
                    num4 -= 18.0;
                    num2 = Convert.ToInt32((double) num4, (IFormatProvider) CultureInfo.InvariantCulture);
                }
                ExcelWorksheet.ExcelViewportInfo defaultViewport = worksheet.DefaultViewport;
                switch (pnnAct)
                {
                    case 0:
                        defaultViewport.ActiveColumnPane = 1;
                        defaultViewport.ActiveRowPane = 1;
                        break;

                    case 1:
                        defaultViewport.ActiveColumnPane = 1;
                        defaultViewport.ActiveRowPane = 0;
                        break;

                    case 2:
                        defaultViewport.ActiveColumnPane = 0;
                        defaultViewport.ActiveRowPane = 1;
                        break;

                    case 3:
                        defaultViewport.ActiveColumnPane = 0;
                        defaultViewport.ActiveRowPane = 0;
                        break;
                }
                if (x > 0)
                {
                    defaultViewport.ColumnPaneCount++;
                    int[] numArray = new int[2];
                    numArray[1] = columnLeft;
                    defaultViewport.LeftColumn = numArray;
                    defaultViewport.PreferredWidth = new int[] { num, -1 };
                }
                if (y > 0)
                {
                    defaultViewport.RowPaneCount++;
                    int[] numArray3 = new int[2];
                    numArray3[1] = rwTop;
                    defaultViewport.TopRow = numArray3;
                    defaultViewport.PreferredHeight = new int[] { num2, -1 };
                }
            }
        }

        public void SetPrintArea(int index, string printArea)
        {
            if (!string.IsNullOrWhiteSpace(printArea))
            {
                if (index == -1)
                {
                    for (int i = 0; i < this._workbook.Worksheets.Count; i++)
                    {
                        this._workbook.Worksheets[i].PrintArea = printArea;
                    }
                }
                else
                {
                    this._workbook.Worksheets[index].PrintArea = printArea;
                }
            }
        }

        public void SetPrintOption(short sheet, IExcelPrintOptions printOption)
        {
            if (printOption == null)
            {
                throw new ArgumentNullException("printOption");
            }
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet.PrintSettings == null)
            {
                worksheet.PrintSettings = new ExcelPrintSettings();
            }
            worksheet.PrintSettings.Options = printOption;
        }

        public void SetPrintPageMargin(short sheet, IExcelPrintPageMargin printPageMagin)
        {
            if (printPageMagin == null)
            {
                throw new ArgumentNullException("printPageMagin");
            }
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet.PrintSettings == null)
            {
                worksheet.PrintSettings = new ExcelPrintSettings();
            }
            worksheet.PrintSettings.Margin = printPageMagin;
        }

        public void SetPrintPageSetting(short sheet, IExcelPrintPageSetting pageSetting)
        {
            if (pageSetting == null)
            {
                throw new ArgumentNullException("pageSetting");
            }
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            worksheet.PrintSettings.PageSetting = pageSetting;
        }

        public void SetPrintTitles(int index, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (index == -1)
                {
                    for (int i = 0; i < this._workbook.Worksheets.Count; i++)
                    {
                        this._workbook.Worksheets[i].PrintTitle = title;
                    }
                }
                else
                {
                    this._workbook.Worksheets[index].PrintTitle = title;
                }
            }
        }

        public void SetProtect(short sheet, bool isProtect)
        {
            if (sheet != -1)
            {
                IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
                worksheet.IsLocked = isProtect;
            }
            else
            {
                this._workbook.Locked = isProtect;
                using (IEnumerator<IExcelWorksheet> enumerator = this._workbook.Worksheets.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.IsLocked = isProtect;
                    }
                }
            }
        }

        public void SetRowColumnGutters(short sheetIndex, short dxRwGut, short dyColGut, short iLevelRwMac, short iLevelColMac)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheetIndex];
            worksheet.RowGutter = dxRwGut;
            worksheet.RowMaxOutlineLevel = iLevelRwMac;
            worksheet.ColumnGutter = dyColGut;
            worksheet.ColumnMaxOutlineLevel = iLevelColMac;
        }

        public bool SetRowInfo(short sheet, int row, int columnFirstDef, int columnLastDefPlus1, short ixf, double height, byte outlineLevel, bool collapsed, bool hidden, bool unSynced, bool ghostDirty)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            if (worksheet.RowCount <= row)
            {
                worksheet.RowCount = row;
            }
            IExcelRow row2 = worksheet.GetRow(row, true);
            if (row2 == null)
            {
                return false;
            }
            if (ghostDirty)
            {
                row2.SetFormatId(ixf);
            }
            row2.Collapsed = collapsed;
            row2.OutLineLevel = outlineLevel;
            if (!double.IsNaN(height))
            {
                row2.Height = height;
            }
            row2.Visible = !hidden;
            return true;
        }

        public void SetScroll(bool dispHScroll, bool dispVScroll)
        {
            this._workbook.VerticalScrollBarPolicy = dispVScroll ? ExcelScrollBarPolicy.AsNeeded : ExcelScrollBarPolicy.Never;
            this._workbook.HorizontalScrollBarPolicy = dispHScroll ? ExcelScrollBarPolicy.AsNeeded : ExcelScrollBarPolicy.Never;
        }

        public void SetSelection(short sheet, PaneType paneType, int rowActive, int columnActive, int refCount, List<int> rowFirst, List<int> rowLast, List<int> colFirst, List<int> colLast)
        {
            int num = rowFirst.Count;
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            for (int i = 0; i < num; i++)
            {
                int row = rowFirst[i];
                int num4 = rowLast[i];
                int column = colFirst[i];
                int num6 = colLast[i];
                SelectionRange range = new SelectionRange(row, column, (row == -1) ? 0 : ((num4 - row) + 1), (column == -1) ? 0 : ((num6 - column) + 1)) {
                    activePaneType = paneType
                };
                if ((worksheet.Selections.Count != 1) || (worksheet.Selections[0] != range))
                {
                    worksheet.Selections.Add(range);
                }
            }
            int num7 = Math.Max(0, Math.Min(rowActive, worksheet.RowCount - 1));
            int num8 = Math.Max(0, Math.Min(columnActive, worksheet.ColumnCount - 1));
            worksheet.ActiveRowIndex = (worksheet.RowCount > 0) ? num7 : -1;
            worksheet.ActiveColumnIndex = (worksheet.ColumnCount > 0) ? num8 : -1;
        }

        public void SetTable(int sheetIndex, IExcelTable table)
        {
            if (table != null)
            {
                this._workbook.Worksheets[sheetIndex].SetSheetTable(table);
            }
        }

        public void SetTableDefaultStyle(string defaultTableStyleName, string defaultPivotTableStyleName)
        {
            this._workbook.DefaultTableStyleName = defaultTableStyleName;
            this._workbook.DefaultPivotTableStyleName = defaultPivotTableStyleName;
        }

        public void SetTableStyle(IExcelTableStyle tableStyle)
        {
            if (tableStyle != null)
            {
                this._workbook.TableStyles.Add(tableStyle);
            }
        }

        public void SetTabs(bool dispTabs, int tabCur, int tabFirst, int tabSel, int tabRatio)
        {
            this._workbook.TabStripPolicy = dispTabs ? ExcelTabStripPolicy.Always : ExcelTabStripPolicy.Never;
            this._workbook.TabStripRatio = ((double) tabRatio) / 1000.0;
            this._workbook.ActiveSheetIndex = tabCur;
            this._workbook.FirstDisplayedTabIndex = tabFirst;
            this._workbook.SelectedTabCount = tabSel;
        }

        public void SetTheme(IExcelTheme theme)
        {
            if (theme == null)
            {
                throw new ArgumentNullException("theme");
            }
            this._workbook.Theme = theme;
        }

        public void SetTopLeft(short sheet, int topRow, int leftColumn)
        {
            this._workbook.Worksheets[sheet].SetTopLeft(topRow, leftColumn);
        }

        public void SetValidationData(short sheet, IExcelDataValidation dv)
        {
            if (dv == null)
            {
                throw new ArgumentNullException("dv");
            }
            this._workbook.Worksheets[sheet].SetDataValidations(dv);
        }

        public void SetWindow(IExcelRect rect, bool hidden, bool iconic)
        {
            this._workbook.ExcelRect = rect;
            this._workbook.IsWindowHidden = hidden;
            this._workbook.IsWindowDisplayAsIcon = iconic;
        }

        public void SetZoom(short sheet, float zoom)
        {
            IExcelWorksheet worksheet = this._workbook.Worksheets[sheet];
            worksheet.Zoom = zoom;
        }
    }
}

