#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.Xls;
using Dt.Xls.Chart;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    internal class ExcelWriter : IExcelWriter, IExcelWriter2, IExcelSparklineWriter, IExcelTableWriter, IExcelChartWriter, IExcelLosslessWriter
    {
        Dictionary<int, DataValidator[,]> _allValidators;
        Dictionary<int, Dictionary<int, double>> _autoRowHeight;
        static Dictionary<string, int> _builtInStyle;
        HashSet<string> _builtInTableStyleNames;
        HashSet<string> _cellRangeNames;
        Dictionary<int, Dictionary<long, int>> _cellStyleTable;
        Dictionary<int, List<IExcelFilterColumn>> _colorFilters;
        Dictionary<int, Dictionary<int, int>> _columnStyleTable;
        Dictionary<int, List<IExcelConditionalFormat>> _conditinalFormats;
        Dictionary<int, double> _defaultRowHeights;
        List<IDifferentialFormatting> _differenceFormats;
        Dictionary<int, Dictionary<long, Tuple<string, CellRange>>> _formulas;
        double _maxiumDigitWidth;
        List<StyleInfo> _namedStyles;
        Dictionary<string, int> _namedStyleTable;
        StyleInfo _normalStyleInfo;
        Dictionary<int, List<int>> _notEmptyRows;
        Dictionary<int, Dictionary<int, int>> _rowStyleTable;
        ExcelSaveFlags _saveFlags;
        int[] _sheetscolumnOffsets;
        int[] _sheetsRowOffsets;
        Dictionary<int, List<SparklineGroup>> _sparklineGroups;
        List<StyleInfo> _styles;
        Dictionary<string, List<IExcelFilterColumn>> _tableColorFilters;
        List<IExcelTableStyle> _tableStyles;
        Dictionary<string, int> _uniqueTableID;
        Workbook _workbook;
        Dictionary<int, Dictionary<int, List<IExcelCell>>> _workbookCells;
        static StyleInfo DefaultDateTimeStyle;
        static StyleInfo DefaultTimeSpanStyle;
        static string MeasureItem = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static GeneralFormatter ShortDateFormatter;
        static GeneralFormatter ShortTimeFormatter;

        static ExcelWriter()
        {
            InitExcelBuiltInStyles();
            ExtendedNumberFormatHelper.Reset();
        }

        internal ExcelWriter(Workbook owner)
            : this(owner, ExcelSaveFlags.NoFlagsSet)
        {
        }

        internal ExcelWriter(Workbook owner, ExcelSaveFlags saveFlags)
        {
            this._defaultRowHeights = new Dictionary<int, double>();
            this._autoRowHeight = new Dictionary<int, Dictionary<int, double>>();
            this._conditinalFormats = new Dictionary<int, List<IExcelConditionalFormat>>();
            this._styles = new List<StyleInfo>();
            this._namedStyles = new List<StyleInfo>();
            this._namedStyleTable = new Dictionary<string, int>();
            this._cellStyleTable = new Dictionary<int, Dictionary<long, int>>();
            this._rowStyleTable = new Dictionary<int, Dictionary<int, int>>();
            this._columnStyleTable = new Dictionary<int, Dictionary<int, int>>();
            this._differenceFormats = new List<IDifferentialFormatting>();
            this._colorFilters = new Dictionary<int, List<IExcelFilterColumn>>();
            this._tableColorFilters = new Dictionary<string, List<IExcelFilterColumn>>();
            this._notEmptyRows = new Dictionary<int, List<int>>();
            this._workbookCells = new Dictionary<int, Dictionary<int, List<IExcelCell>>>();
            this._allValidators = new Dictionary<int, DataValidator[,]>();
            this._formulas = new Dictionary<int, Dictionary<long, Tuple<string, CellRange>>>();
            this._maxiumDigitWidth = double.NaN;
            this._sparklineGroups = new Dictionary<int, List<SparklineGroup>>();
            this._uniqueTableID = new Dictionary<string, int>();
            this._tableStyles = new List<IExcelTableStyle>();
            this._workbook = owner;
            this._saveFlags = saveFlags;
            this.InitDefaultDateFormatter();
            this.PreProcessWorkbook();
        }

        void AddCellSparklineToCache(List<SparklineGroup> sheetSparklineGroups, Sparkline sparkline)
        {
            if ((sparkline != null) && (sheetSparklineGroups.IndexOf(sparkline.Group) == -1))
            {
                sheetSparklineGroups.Add(sparkline.Group);
            }
        }

        void AddCellToCache(int sheetIndex, int row, Dt.Cells.Data.ExcelCell cell)
        {
            int num = row;
            if (!this._workbookCells[sheetIndex].ContainsKey(num))
            {
                this._workbookCells[sheetIndex].Add(num, new List<IExcelCell>());
            }
            if (this._workbookCells[sheetIndex][num] == null)
            {
                this._workbookCells[sheetIndex][num] = new List<IExcelCell>();
            }
            this._workbookCells[sheetIndex][num].Add(cell);
        }

        void AddCellValidatorToCache(int row, int column, StyleInfo styleInfo, DataValidator[,] sheetValidators)
        {
            if ((!this._saveFlags.SaveDataOnly() && (styleInfo != null)) && (styleInfo.DataValidator != null))
            {
                sheetValidators[row, column] = styleInfo.DataValidator;
            }
        }

        void AddColorFilterSettingToDifferentConditionalFormats()
        {
            for (int i = 0; i < this._workbook.SheetCount; i++)
            {
                Worksheet local1 = this._workbook.Sheets[i];
                this.GetSheetColorFilter(i);
                this.GetSheetTableColorFilter(i);
            }
        }

        void AddDateTimeFormatterIfValueIsDate(object value, StyleInfo styleInfo)
        {
            if ((styleInfo != null) && (styleInfo.Formatter == null))
            {
                if ((value is DateTime) || (value is DateTimeOffset))
                {
                    styleInfo.Formatter = ShortDateFormatter;
                }
                if (value is TimeSpan)
                {
                    styleInfo.Formatter = ShortTimeFormatter;
                }
            }
        }

        void AddFilterToFilterColumn(IExcelFilterColumn filterColumn, IExcelFilter filter)
        {
            if ((filterColumn != null) && (filter != null))
            {
                if (filter is IExcelTop10Filter)
                {
                    filterColumn.Top10 = filter as IExcelTop10Filter;
                }
                if (filter is IExcelDynamicFilter)
                {
                    filterColumn.DynamicFilter = filter as IExcelDynamicFilter;
                }
                if (filter is IExcelFilters)
                {
                    filterColumn.Filters = filter as IExcelFilters;
                }
                if (filter is IExcelCustomFilters)
                {
                    filterColumn.CustomFilters = filter as IExcelCustomFilters;
                }
            }
        }

        public bool AllEqual(IEnumerable<int> items, int expected)
        {
            if (items == null)
            {
                return false;
            }
            using (IEnumerator<int> enumerator = items.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != expected)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        string AppendA1Letter(int coord)
        {
            StringBuilder builder = new StringBuilder();
            if (coord >= 0)
            {
                int length = builder.Length;
                coord++;
                while (coord > 0)
                {
                    builder.Insert(length, (char[])new char[] { ((char)(0x41 + ((coord - 1) % 0x1a))) });
                    coord = (coord - 1) / 0x1a;
                }
            }
            return builder.ToString();
        }

        bool AreSameCondition(ConditionBase condition1, ConditionBase condition2)
        {
            if (!object.ReferenceEquals(condition1, condition2))
            {
                if ((condition1 == null) || (condition2 == null))
                {
                    return false;
                }
                FieldInfo[] infoArray = Enumerable.ToArray<FieldInfo>((IEnumerable<FieldInfo>)(from f1 in IntrospectionExtensions.GetTypeInfo(condition1.GetType()).DeclaredFields select f1));
                FieldInfo[] infoArray2 = Enumerable.ToArray<FieldInfo>((IEnumerable<FieldInfo>)(from f2 in IntrospectionExtensions.GetTypeInfo(condition2.GetType()).DeclaredFields select f2));
                if (Enumerable.Count<FieldInfo>(infoArray) != Enumerable.Count<FieldInfo>(infoArray2))
                {
                    return false;
                }
                for (int i = 0; i < Enumerable.Count<FieldInfo>(infoArray); i++)
                {
                    if (infoArray[i].Name != infoArray2[i].Name)
                    {
                        return false;
                    }
                    if (infoArray[i].GetValue(condition1) != infoArray2[i].GetValue(condition2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        bool AreSameValidator(DataValidator validator1, DataValidator validator2)
        {
            if (object.ReferenceEquals(validator1, validator2))
            {
                return true;
            }
            if ((validator1 == null) || (validator2 == null))
            {
                return false;
            }
            return ((((((validator1.ErrorMessage == validator2.ErrorMessage) && (validator1.ErrorStyle == validator2.ErrorStyle)) && ((validator1.ErrorTitle == validator2.ErrorTitle) && (validator1.IgnoreBlank == validator2.IgnoreBlank))) && (((validator1.InCellDropdown == validator2.InCellDropdown) && (validator1.InputMessage == validator2.InputMessage)) && ((validator1.InputTitle == validator2.InputTitle) && (validator1.ShowErrorMessage == validator2.ShowErrorMessage)))) && ((validator1.ShowInputMessage == validator2.ShowInputMessage) && (validator1.Type == validator2.Type))) && this.AreSameCondition(validator1.Condition, validator2.Condition));
        }

        static string BuildExcelPrintHeader(string left, string center, string right)
        {
            return string.Format("&L{0}&C{1}&R{2}", (object[])new object[] { left, center, right });
        }

        double CalcColumnWidth(double actualWidth)
        {
            if (actualWidth == 0.0)
            {
                return 0.0;
            }
            double maxiumDigitWidth = this.GetMaxiumDigitWidth();
            return (Math.Floor((double)(((actualWidth / maxiumDigitWidth) * 256.0) + 0.5)) / 256.0);
        }

        double CalcRowHeight(string text, FontFamily fontFamily, double fontSize, bool wordWrap, FontWeight fontWeight, FontStyle fontStyle, double width)
        {
            bool bold = false;
            if ((fontWeight.Equals(FontWeights.Bold) || fontWeight.Equals(FontWeights.ExtraBold)) || fontWeight.Equals(FontWeights.SemiBold))
            {
                bold = true;
            }
            string source = NameConstans.DEFAULT_FONT_FAMILY;
            if (fontFamily != null)
            {
                source = fontFamily.Source;
            }
            return ((1.0 + this.MeasureStringHeight(text, source, fontSize, wordWrap, bold, fontStyle == FontStyle.Italic, width)) * 72.0) / UnitHelper.GetDPI();
        }

        double CalculateDefaultRowHeight(int sheet)
        {
            double result = 0.0;
            if (!this._defaultRowHeights.TryGetValue(sheet, out result))
            {
                FontFamily fontFamily = this.GetFontFamily(sheet);
                double fontSize = UnitHelper.PixelToPoint(this.GetFontSize(sheet));
                BorderLine borderTop = this._workbook.Sheets[sheet].DefaultStyle.BorderTop;
                BorderLine borderBottom = this._workbook.Sheets[sheet].DefaultStyle.BorderBottom;
                double num = (borderTop == null) ? 0.0 : this.GetBorderLineHeight(borderTop.Style);
                double num2 = (borderBottom == null) ? 0.0 : this.GetBorderLineHeight(borderBottom.Style);
                double extraHeight = Math.Max(num, num2);
                if (extraHeight == 0.0)
                {
                    extraHeight = 1.0;
                }
                result = UnitHelper.PixelToPoint(extraHeight + this.MeasureStringHeight(MeasureItem, fontFamily.Source, fontSize, false, false, false, this._workbook.Sheets[sheet].DefaultColumnWidth));
                this._defaultRowHeights.Add(sheet, result);
            }
            return result;
        }

        bool CanMove(DataValidator expected, Worksheet sheet, int row, int column, int rowStep, int columnStep, bool[,] visited)
        {
            if (((row + rowStep) >= sheet.RowCount) || ((column + columnStep) >= sheet.ColumnCount))
            {
                return false;
            }
            if (visited[row + rowStep, column + columnStep])
            {
                return false;
            }
            return this.AreSameValidator(expected, sheet.Cells[row + rowStep, column + columnStep].DataValidator);
        }

        bool CanMoveToLower(DataValidator expected, Worksheet sheet, int row, int column, bool[,] visited)
        {
            return this.CanMove(expected, sheet, row, column, 1, 0, visited);
        }

        bool CanMoveToLowerRight(DataValidator expected, Worksheet sheet, int row, int column, bool[,] visited)
        {
            return this.CanMove(expected, sheet, row, column, 1, 1, visited);
        }

        bool CanMoveToRight(DataValidator expected, Worksheet sheet, int row, int column, bool[,] visited)
        {
            return this.CanMove(expected, sheet, row, column, 0, 1, visited);
        }

        void Clear()
        {
            this._sheetsRowOffsets = null;
            this._sheetscolumnOffsets = null;
            this._defaultRowHeights.Clear();
            this._workbookCells.Clear();
            this._conditinalFormats.Clear();
            this._styles.Clear();
            this._namedStyles.Clear();
            this._namedStyleTable.Clear();
            this._cellStyleTable.Clear();
            this._rowStyleTable.Clear();
            this._columnStyleTable.Clear();
            this._notEmptyRows.Clear();
            this._differenceFormats.Clear();
            this._colorFilters.Clear();
            this._tableColorFilters.Clear();
            this._formulas.Clear();
            this._allValidators.Clear();
            this._maxiumDigitWidth = double.NaN;
            this._normalStyleInfo = null;
            this._sparklineGroups.Clear();
            this._uniqueTableID.Clear();
            this._tableStyles.Clear();
            this._cellRangeNames = null;
        }

        ExcelCellRange CreateCellRange(int row, int column, int rowCount, int columnColunt)
        {
            return new ExcelCellRange { Row = row, Column = column, RowSpan = rowCount, ColumnSpan = columnColunt };
        }

        string CreateConditionalFormatFormula(ExcelGeneralRule rule, IRange range)
        {
            if ((rule != null) && (range != null))
            {
                if ((range.Row < 0) || (range.Column < 0))
                {
                    return null;
                }
                string str = this.AppendA1Letter(range.Column) + ((int)(range.Row + 1));
                string str2 = string.Empty;
                if (range.RowSpan == 0x100000)
                {
                    str2 = string.Format("${0}:${1}", (object[])new object[] { this.AppendA1Letter(range.Column), this.AppendA1Letter((range.Column + range.ColumnSpan) - 1) });
                }
                else if (range.ColumnSpan == 0x4000)
                {
                    str2 = string.Format("${0}:${1}", (object[])new object[] { ((int)(range.Row + 1)), ((int)(range.Row + range.RowSpan)) });
                }
                else
                {
                    str2 = string.Format("${0}${1}:${2}${3}", (object[])new object[] { this.AppendA1Letter(range.Column), ((int)(range.Row + 1)), this.AppendA1Letter((range.Column + range.ColumnSpan) - 1), ((int)(range.Row + range.RowSpan)) });
                }
                if (rule.Type == ExcelConditionalFormatType.Top10)
                {
                    if (rule.Bottom.HasValue && rule.Bottom.Value)
                    {
                        if (rule.Percent.HasValue && rule.Percent.Value)
                        {
                            return string.Format("IF(INT(COUNT({0})*{1}%)>0,SMALL({0},INT(COUNT({0})*{1}%)),MIN({0}))>={2}", (object[])new object[] { str2, ((int)rule.Rank.Value), str });
                        }
                        return string.Format("SMALL(({0}),MIN({1},COUNT({0})))>={2}", (object[])new object[] { str2, ((int)rule.Rank.Value), str });
                    }
                    if (rule.Percent.HasValue && rule.Percent.Value)
                    {
                        return string.Format("IF(INT(COUNT({0})*{1}%)>0,LARGE({0},INT(COUNT({0})*{1}%)),MIN({0}))<={2}", (object[])new object[] { str2, ((int)rule.Rank.Value), str });
                    }
                    return string.Format("LARGE(({0}),MIN({1},COUNT({0})))<={2}", (object[])new object[] { str2, ((int)rule.Rank.Value), str });
                }
                if (rule.Type == ExcelConditionalFormatType.ContainsText)
                {
                    return string.Format("NOT(ISERROR(SEARCH(\"{0}\",{1})))", (object[])new object[] { rule.Text, str });
                }
                if (rule.Type == ExcelConditionalFormatType.NotContainsText)
                {
                    return string.Format("ISERROR(SEARCH(\"{0}\",{1}))", (object[])new object[] { rule.Text, str });
                }
                if (rule.Type == ExcelConditionalFormatType.BeginsWith)
                {
                    return string.Format("LEFT({0},{1}) = \"{2}\"", (object[])new object[] { str, ((int)rule.Text.Length), rule.Text });
                }
                if (rule.Type == ExcelConditionalFormatType.EndsWith)
                {
                    return string.Format("RIGHT({0},{1}) = \"{2}\"", (object[])new object[] { str, ((int)rule.Text.Length), rule.Text });
                }
                if (rule.Type == ExcelConditionalFormatType.DuplicateValues)
                {
                    return string.Format("AND(COUNTIF({0},{1})>1,NOT(ISBLANK({1})))", (object[])new object[] { str2, str });
                }
                if (rule.Type == ExcelConditionalFormatType.UniqueValues)
                {
                    return string.Format("AND(COUNTIF({0},{1})=1,NOT(ISBLANK({1})))", (object[])new object[] { str2, str });
                }
                if (((rule.Type == ExcelConditionalFormatType.AboveAverage) || (rule.Type == ExcelConditionalFormatType.BelowAverage)) || ((rule.Type == ExcelConditionalFormatType.AboveOrEqualToAverage) || (rule.Type == ExcelConditionalFormatType.BelowOrEqualToAverage)))
                {
                    if ((rule.Type == ExcelConditionalFormatType.AboveAverage) && !rule.StdDev.HasValue)
                    {
                        return string.Format("{0}>AVERAGE({1})", (object[])new object[] { str, str2 });
                    }
                    if (rule.AboveAverage.HasValue)
                    {
                        if (((rule.EqualAverage.HasValue && !rule.AboveAverage.Value) && (rule.EqualAverage.HasValue && rule.EqualAverage.Value)) && !rule.StdDev.HasValue)
                        {
                            return string.Format("{0}<=AVERAGE({1})", (object[])new object[] { str, str2 });
                        }
                        if (!rule.AboveAverage.Value && !rule.StdDev.HasValue)
                        {
                            return string.Format("{0}<AVERAGE({1})", (object[])new object[] { str, str2 });
                        }
                    }
                    if ((rule.EqualAverage.HasValue && rule.EqualAverage.Value) && !rule.StdDev.HasValue)
                    {
                        return string.Format("{0}>=AVERAGE({1})", (object[])new object[] { str, str2 });
                    }
                    if (rule.StdDev.HasValue)
                    {
                        if (rule.AboveAverage.HasValue && !rule.AboveAverage.Value)
                        {
                            return string.Format("({0}-AVERAGE({1}))<=STDEVP({1})*(-{2})", (object[])new object[] { str, str2, ((int)rule.StdDev.Value) });
                        }
                        return string.Format("({0}-AVERAGE({1}))>=STDEVP({1})*({2})", (object[])new object[] { str, str2, ((int)rule.StdDev.Value) });
                    }
                }
            }
            return null;
        }

        IRange ExpandRange(CellRange range, int rowOffset, int columnOffset)
        {
            if (range == null)
            {
                return null;
            }
            int row = range.Row;
            int column = range.Column;
            int rowCount = range.RowCount;
            int columnCount = range.ColumnCount;
            if (row == -1)
            {
                row = 0;
                rowCount = 0x100000;
            }
            if (column == -1)
            {
                column = 0;
                columnCount = 0x4000;
            }
            if (rowOffset < 0)
            {
                row = Math.Max(0, row + rowOffset);
                rowCount += range.Row - row;
            }
            else
            {
                rowCount += rowOffset;
            }
            if (columnOffset < 0)
            {
                column = Math.Max(0, column + columnOffset);
                columnCount += range.Column - column;
            }
            else
            {
                columnCount += columnOffset;
            }
            return new ExcelCellRange { Row = row, Column = column, RowSpan = rowCount, ColumnSpan = columnCount };
        }

        public void Finish()
        {
            this.PostProcessWorkbook();
            ExtendedNumberFormatHelper.Reset();
        }

        public IExcelAutoFilter GetAutoFilter(short sheet)
        {
            if (this._saveFlags.SaveDataOnly())
            {
                return null;
            }
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if ((worksheet.RowFilter == null) || (worksheet.RowFilter.Range == null))
            {
                return null;
            }
            ExcelAutoFilter filter = new ExcelAutoFilter
            {
                Range = this.ExpandRange(worksheet.RowFilter.Range, -1, 0)
            };
            SheetTable table = worksheet.FindTable(worksheet.RowFilter.Range.Row, worksheet.RowFilter.Range.Column);
            if ((((table != null) && (table.RowFilter != null)) && ((table.RowFilter.Range.Row == worksheet.RowFilter.Range.Row) && (table.RowFilter.Range.Column == worksheet.RowFilter.Range.Column))) && ((table.RowFilter.Range.RowCount == worksheet.RowFilter.Range.RowCount) && (table.RowFilter.Range.ColumnCount == worksheet.RowFilter.Range.ColumnCount)))
            {
                return null;
            }
            if (this._colorFilters.ContainsKey(sheet) && (this._colorFilters[sheet].Count > 0))
            {
                if (filter.FilterColumns == null)
                {
                    filter.FilterColumns = new List<IExcelFilterColumn>();
                }
                filter.FilterColumns.AddRange(this._colorFilters[sheet]);
            }
            for (int i = 0; i < worksheet.RowFilter.Range.ColumnCount; i++)
            {
                ReadOnlyCollection<ConditionBase> filterItems = worksheet.RowFilter.GetFilterItems(worksheet.RowFilter.Range.Column + i);
                if ((filterItems != null) && (filterItems.Count > 0))
                {
                    ConditionBase base2 = null;
                    if (filterItems.Count > 0)
                    {
                        base2 = filterItems[0];
                        for (int j = 1; j < filterItems.Count; j++)
                        {
                            base2 = new RelationCondition(RelationCompareType.Or, base2, filterItems[j]);
                        }
                    }
                    else
                    {
                        base2 = filterItems[0];
                    }
                    if (base2 != null)
                    {
                        ExcelFilterColumn filterColumn = new ExcelFilterColumn
                        {
                            AutoFilterColumnId = (uint)i
                        };
                        IExcelFilter filter2 = base2.ToExcelFilter(worksheet, new CellRange(filter.Range.Row, filter.Range.Column + i, filter.Range.RowSpan, 1));
                        if (filter2 != null)
                        {
                            this.AddFilterToFilterColumn(filterColumn, filter2);
                            filter.FilterColumns.Add(filterColumn);
                        }
                    }
                }
            }
            return filter;
        }

        double GetBorderLineHeight(BorderLineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case BorderLineStyle.None:
                    return 0.0;

                case BorderLineStyle.Thin:
                case BorderLineStyle.Dashed:
                case BorderLineStyle.Dotted:
                case BorderLineStyle.Hair:
                case BorderLineStyle.DashDot:
                case BorderLineStyle.DashDotDot:
                case BorderLineStyle.SlantedDashDot:
                    return 1.0;

                case BorderLineStyle.Medium:
                case BorderLineStyle.MediumDashed:
                case BorderLineStyle.MediumDashDot:
                case BorderLineStyle.MediumDashDotDot:
                    return 2.0;

                case BorderLineStyle.Thick:
                case BorderLineStyle.Double:
                    return 3.0;
            }
            return 0.0;
        }

        public List<IBuiltInName> GetBuiltInNameList()
        {
            return null;
        }

        public ICalculationProperty GetCalculationProperty()
        {
            ExcelReferenceStyle style = ExcelReferenceStyle.A1;
            if (this._workbook.ReferenceStyle == ReferenceStyle.R1C1)
            {
                style = ExcelReferenceStyle.R1C1;
            }
            return new CalculationProperty { CalculationMode = ExcelCalculationMode.Automatic, IsFullPrecision = true, IsIterateCalculate = false, MaximunChange = 0.001, MaxIterationCount = 100, ReCalculationBeforeSave = true, RefMode = style };
        }

        void GetCellDataValidation(short sheet, List<IExcelDataValidation> result)
        {
            DataValidator[,] validatorArray;
            Worksheet worksheet = this._workbook.Sheets[sheet];
            this._allValidators.TryGetValue(sheet, out validatorArray);
            List<KeyValuePair<DataValidator, ExcelCellRange>> list = new List<KeyValuePair<DataValidator, ExcelCellRange>>();
            int rowCount = worksheet.RowCount;
            int columnCount = worksheet.ColumnCount;
            int num = worksheet.RowCount;
            int num2 = worksheet.ColumnCount;
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    if (validatorArray[i, j] != null)
                    {
                        list.Add(new KeyValuePair<DataValidator, ExcelCellRange>(validatorArray[i, j], this.CreateCellRange(i, j, 1, 1)));
                    }
                }
            }
            new CalcParserContext(this._workbook.ReferenceStyle == ReferenceStyle.R1C1, 0, 0, null);
            foreach (KeyValuePair<DataValidator, ExcelCellRange> pair in list)
            {
                IExcelDataValidation validation = pair.Key.ToExcelDataValidation();
                if (!string.IsNullOrWhiteSpace(validation.FirstFormula))
                {
                    validation.FirstFormula = validation.FirstFormula;
                }
                if (!string.IsNullOrWhiteSpace(validation.SecondFormula))
                {
                    validation.SecondFormula = validation.SecondFormula;
                }
                validation.Ranges.Add(pair.Value);
                result.Add(validation);
            }
        }

        Tuple<string, CellRange> GetCellFormulaStruct(int sheetIndex, int row, int column)
        {
            Tuple<string, CellRange> tuple = null;
            if (!this._saveFlags.SaveDataOnly() && !this._saveFlags.NoFormulas())
            {
                tuple = this.GetFormula(sheetIndex, row, column);
            }
            return tuple;
        }

        public bool GetCells(short sheet, List<IExcelCell> cells)
        {
            if (!this._workbookCells.ContainsKey(sheet))
            {
                return false;
            }
            Dictionary<int, List<IExcelCell>> dictionary = this._workbookCells[sheet];
            if (dictionary != null)
            {
                foreach (int num in dictionary.Keys)
                {
                    cells.AddRange(dictionary[num]);
                }
            }
            return true;
        }

        void GetCellStyle(Worksheet worksheet, int row, int column, Dictionary<string, StyleInfo> allNamedStyles, Dictionary<long, int> cellStyleTable, string currentColumnNamedStyle, string currentRowNamedStyle, StyleInfo currentRowStyle, StyleInfo currentColumnStyle, int styleKey, object value, ref string styleName, ref StyleInfo styleInfo, bool isInTable)
        {
            object obj2 = worksheet.GetStyleObject(row, column, SheetArea.Cells);
            if (isInTable)
            {
                StyleInfo info = worksheet.GetStyleInfo(row, column);
                if (info != null)
                {
                    StyleInfo info2 = info.Clone() as StyleInfo;
                    if ((!info2.IsBackgroundSet() && !info2.IsBackgroundThemeColorSet()) && this._workbook.DefaultStyle.IsBackgroundSet())
                    {
                        info2.Background = this._workbook.DefaultStyle.Background;
                    }
                    if ((!info2.IsBackgroundSet() && !info2.IsBackgroundThemeColorSet()) && this._workbook.DefaultStyle.IsBackgroundThemeColorSet())
                    {
                        info2.BackgroundThemeColor = this._workbook.DefaultStyle.BackgroundThemeColor;
                    }
                    if (!info2.IsFontSizeSet() && this._workbook.DefaultStyle.IsFontSizeSet())
                    {
                        info2.FontSize = this._workbook.DefaultStyle.FontSize;
                    }
                    if (!info2.IsFontFamilySet() && this._workbook.DefaultStyle.IsFontFamilySet())
                    {
                        info2.FontFamily = this._workbook.DefaultStyle.FontFamily;
                    }
                    if (!info2.IsFontWeightSet() && this._workbook.DefaultStyle.IsFontWeightSet())
                    {
                        info2.FontWeight = this._workbook.DefaultStyle.FontWeight;
                    }
                    if (!info2.IsFontStyleSet() && this._workbook.DefaultStyle.IsFontStyleSet())
                    {
                        info2.FontStyle = this._workbook.DefaultStyle.FontStyle;
                    }
                    if (!info2.IsFontStretchSet() && this._workbook.DefaultStyle.IsFontStretchSet())
                    {
                        info2.FontStretch = this._workbook.DefaultStyle.FontStretch;
                    }
                    if (!info2.IsBorderBottomSet() && this._workbook.DefaultStyle.IsBorderBottomSet())
                    {
                        info2.BorderBottom = this._workbook.DefaultStyle.BorderBottom;
                    }
                    if (!info2.IsBorderTopSet() && this._workbook.DefaultStyle.IsBorderTopSet())
                    {
                        info2.BorderTop = this._workbook.DefaultStyle.BorderTop;
                    }
                    if (!info2.IsBorderLeftSet() && this._workbook.DefaultStyle.IsBorderLeftSet())
                    {
                        info2.BorderLeft = this._workbook.DefaultStyle.BorderLeft;
                    }
                    if (!info2.IsBorderRightSet() && this._workbook.DefaultStyle.IsBorderRightSet())
                    {
                        info2.BorderRight = this._workbook.DefaultStyle.BorderRight;
                    }
                    if (!info2.IsLockedSet() && this._workbook.DefaultStyle.IsLockedSet())
                    {
                        info2.Locked = this._workbook.DefaultStyle.Locked;
                    }
                    if (!info2.IsShrinkToFitSet() && this._workbook.DefaultStyle.IsShrinkToFitSet())
                    {
                        info2.ShrinkToFit = this._workbook.DefaultStyle.ShrinkToFit;
                    }
                    if (!info2.IsTextIndentSet() && this._workbook.DefaultStyle.IsTextIndentSet())
                    {
                        info2.TextIndent = this._workbook.DefaultStyle.TextIndent;
                    }
                    if (!info2.IsVerticalAlignmentSet() && this._workbook.DefaultStyle.IsVerticalAlignmentSet())
                    {
                        info2.VerticalAlignment = this._workbook.DefaultStyle.VerticalAlignment;
                    }
                    if (!info2.IsHorizontalAlignmentSet() && this._workbook.DefaultStyle.IsHorizontalAlignmentSet())
                    {
                        info2.HorizontalAlignment = this._workbook.DefaultStyle.HorizontalAlignment;
                    }
                    if ((!info2.IsForegroundSet() && !info2.IsForegroundThemeColorSet()) && this._workbook.DefaultStyle.IsForegroundSet())
                    {
                        info2.Foreground = this._workbook.DefaultStyle.Foreground;
                    }
                    if ((!info2.IsForegroundSet() && !info2.IsForegroundThemeColorSet()) && this._workbook.DefaultStyle.IsForegroundThemeColorSet())
                    {
                        info2.ForegroundThemeColor = this._workbook.DefaultStyle.ForegroundThemeColor;
                    }
                    info = info2;
                }
                obj2 = info;
            }
            if (!this._saveFlags.SaveDataOnly())
            {
                if ((obj2 != null) && (obj2 is string))
                {
                    styleName = (string)(obj2 as string);
                }
                if (this._saveFlags.SaveAsViewed())
                {
                    styleName = null;
                    styleInfo = worksheet.GetActualStyleInfo(row, column, SheetArea.Cells);
                    if (styleInfo != null)
                    {
                        cellStyleTable.Add((long)styleKey, this.GetStyleIndex(styleInfo));
                    }
                }
                int num = 0;
                if ((!string.IsNullOrEmpty(styleName) && (styleName != currentRowNamedStyle)) && ((styleName != currentColumnNamedStyle) && this._namedStyleTable.TryGetValue(styleName, out num)))
                {
                    cellStyleTable.Add((long)styleKey, num);
                    allNamedStyles.TryGetValue(styleName, out styleInfo);
                }
                else if (styleInfo == null)
                {
                    if ((obj2 != null) && (obj2 is StyleInfo))
                    {
                        styleInfo = obj2 as StyleInfo;
                    }
                    if (styleInfo != null)
                    {
                        if (this.IsEmptyStyleInfo(styleInfo) && styleInfo.IsParentSet())
                        {
                            StyleInfo info3 = this._workbook.NamedStyles.Find(styleInfo.Parent);
                            if (info3 != null)
                            {
                                string parent = styleInfo.Parent;
                                styleInfo = info3.Clone() as StyleInfo;
                                styleInfo.ResetName();
                                styleInfo.Parent = parent;
                            }
                        }
                        if ((styleInfo.IsDataValidatorSet() && !string.IsNullOrWhiteSpace(styleInfo.Name)) && this._namedStyleTable.TryGetValue(styleInfo.Name, out num))
                        {
                            cellStyleTable.Add((long)styleKey, num);
                        }
                        else
                        {
                            if (this.IsEmptyStyleInfo(styleInfo) && (((currentColumnNamedStyle != null) || (currentRowNamedStyle != null)) || ((currentColumnStyle != null) || (currentRowStyle != null))))
                            {
                                styleInfo = worksheet.GetActualStyleInfo(row, column, SheetArea.Cells);
                            }
                            this.AddDateTimeFormatterIfValueIsDate(value, styleInfo);
                            cellStyleTable.Add((long)styleKey, this.GetStyleIndex(styleInfo));
                        }
                    }
                    else if (((styleInfo == null) && (value != null)) && (value is DateTime))
                    {
                        styleInfo = worksheet.GetActualStyleInfo(row, column, SheetArea.Cells);
                        this.AddDateTimeFormatterIfValueIsDate(value, styleInfo);
                        cellStyleTable.Add((long)styleKey, this.GetStyleIndex(styleInfo));
                    }
                    else if (!string.IsNullOrWhiteSpace(currentRowNamedStyle) && this._namedStyleTable.TryGetValue(currentRowNamedStyle, out num))
                    {
                        cellStyleTable.Add((long)styleKey, num);
                        allNamedStyles.TryGetValue(currentRowNamedStyle, out styleInfo);
                    }
                    else if ((currentRowStyle != null) && !this.IsEmptyStyleInfo(currentRowStyle))
                    {
                        styleInfo = currentRowStyle;
                        cellStyleTable.Add((long)styleKey, this.GetStyleIndex(styleInfo));
                    }
                    else if (!string.IsNullOrWhiteSpace(currentColumnNamedStyle) && this._namedStyleTable.TryGetValue(currentColumnNamedStyle, out num))
                    {
                        cellStyleTable.Add((long)styleKey, num);
                        allNamedStyles.TryGetValue(currentColumnNamedStyle, out styleInfo);
                    }
                    else if ((currentColumnStyle != null) && !this.IsEmptyStyleInfo(currentColumnStyle))
                    {
                        styleInfo = currentColumnStyle;
                        cellStyleTable.Add((long)styleKey, this.GetStyleIndex(styleInfo));
                    }
                    else
                    {
                        styleInfo = null;
                    }
                }
            }
            else
            {
                if ((value is DateTime) || (value is DateTimeOffset))
                {
                    styleInfo = DefaultDateTimeStyle;
                }
                if (value is TimeSpan)
                {
                    styleInfo = DefaultTimeSpanStyle;
                }
                if ((styleInfo != null) && !styleInfo.Equals(currentRowStyle))
                {
                    cellStyleTable.Add((long)styleKey, this.GetStyleIndex(styleInfo));
                }
            }
        }

        public string GetCodeName(int sheetIndex)
        {
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            return worksheet.CodeName;
        }

        public Dictionary<int, GcColor> GetColorPalette()
        {
            if (!Dt.Cells.Data.ColorHelper.UseCustomPalette)
            {
                return null;
            }
            Dictionary<int, GcColor> dictionary = new Dictionary<int, GcColor>();
            foreach (KeyValuePair<int, Windows.UI.Color> pair in Dt.Cells.Data.ColorHelper.CustomPalette)
            {
                int introduced6 = pair.Key;
                dictionary[introduced6] = GcColor.FromArgb(pair.Value.R, pair.Value.G, pair.Value.B);
            }
            return dictionary;
        }

        IExcelColumn GetColumn(int sheet, Worksheet worksheet, int column, int columnRangeGroupMaxLevel)
        {
            Dt.Cells.Data.ExcelColumn column2 = new Dt.Cells.Data.ExcelColumn(column)
            {
                Width = this.CalcColumnWidth(worksheet.GetColumnWidth(column, SheetArea.Cells))
            };
            if ((columnRangeGroupMaxLevel >= 0) && (column < worksheet.ColumnCount))
            {
                int levelInternal = worksheet.ColumnRangeGroup.GetLevelInternal(column);
                if (worksheet.ColumnRangeGroup.Direction == RangeGroupDirection.Forward)
                {
                    int num2 = 0;
                    if (column > 0)
                    {
                        num2 = worksheet.ColumnRangeGroup.GetLevelInternal(column - 1);
                    }
                    if (((column > 0) && (levelInternal >= 0)) && (num2 > levelInternal))
                    {
                        column2.Collapsed = worksheet.ColumnRangeGroup.GetCollapsed(column);
                    }
                }
                else
                {
                    int num3 = 0;
                    if ((column + 1) < worksheet.ColumnCount)
                    {
                        num3 = worksheet.ColumnRangeGroup.GetLevelInternal(column + 1);
                    }
                    if (((column >= 0) && (levelInternal == 0)) && (num3 > levelInternal))
                    {
                        column2.Collapsed = worksheet.ColumnRangeGroup.GetCollapsed(column);
                    }
                }
                int num4 = worksheet.ColumnRangeGroup.GetLevelInternal(column);
                if (!this._saveFlags.SaveDataOnly())
                {
                    column2.OutLineLevel = (num4 == -1) ? ((byte)0) : ((byte)num4);
                }
            }
            if (column < worksheet.ColumnCount)
            {
                column2.Visible = worksheet.GetActualColumnVisible(column, SheetArea.Cells);
                if (this._saveFlags.SaveAsViewed() && column2.Collapsed)
                {
                    column2.Visible = false;
                }
            }
            int id = 0;
            if (!this._saveFlags.SaveDataOnly())
            {
                this._columnStyleTable[sheet].TryGetValue(column, out id);
                column2.SetFormatId(id);
                return column2;
            }
            column2.SetFormatId(-1);
            return column2;
        }

        void GetColumnDataValidation(short sheet, List<IExcelDataValidation> result)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            int count = worksheet.Columns.Count;
            for (int i = 0; i < count; i++)
            {
                Column column = worksheet.Columns[i];
                if (column != null)
                {
                    DataValidator dataValidator = column.DataValidator;
                    if (dataValidator != null)
                    {
                        IExcelDataValidation validation = dataValidator.ToExcelDataValidation();
                        validation.Ranges.Add(this.CreateCellRange(0, i, 0x100000, 1));
                        result.Add(validation);
                    }
                }
            }
        }

        List<string> GetColumnNamedStyleInfos(Worksheet worksheet)
        {
            List<string> list = new List<string>();
            int num = Math.Max(worksheet.GetLastDirtyColumn(StorageType.Data), Math.Max(worksheet.GetLastDirtyColumn(StorageType.Sparkline), worksheet.GetLastDirtyColumn(StorageType.Style))) + 1;
            for (int i = 0; i < num; i++)
            {
                if (i < worksheet.ColumnCount)
                {
                    object obj2 = worksheet.GetStyleObject(-1, i, SheetArea.Cells);
                    if (obj2 is string)
                    {
                        list.Add((string)(obj2 as string));
                    }
                    else
                    {
                        list.Add(string.Empty);
                    }
                }
                else
                {
                    list.Add(string.Empty);
                }
            }
            return list;
        }

        List<StyleInfo> GetColumnStyleInfos(Worksheet worksheet)
        {
            List<StyleInfo> columnStyles = new List<StyleInfo>();
            int columnCount = Math.Max(worksheet.GetLastDirtyColumn(StorageType.Data), Math.Max(worksheet.GetLastDirtyColumn(StorageType.Sparkline), worksheet.GetLastDirtyColumn(StorageType.Style))) + 1;
            for (int j = 0; j < columnCount; j++)
            {
                if (j < worksheet.ColumnCount)
                {
                    columnStyles.Add(worksheet.GetStyleInfo(-1, j, SheetArea.Cells));
                }
                else
                {
                    columnStyles.Add(null);
                }
            }
            return columnStyles;
        }

        List<double> GetColumnWidthInfos(Worksheet worksheet)
        {
            List<double> columnsWidthCollection = new List<double>();
            int columnCount = worksheet.ColumnCount;
            for (int j = 0; j < columnCount; j++)
            {
                columnsWidthCollection.Add(worksheet.Columns[j].Width);
            }
            return columnsWidthCollection;
        }

        public List<IExcelConditionalFormat> GetConditionalFormat(int sheet)
        {
            if (!this._saveFlags.SaveDataOnly() && this._conditinalFormats.ContainsKey(sheet))
            {
                return this._conditinalFormats[sheet];
            }
            return null;
        }

        int GetCurrentSheetMaxUsedRowIndex(int sheetIndex)
        {
            List<int> notEmptyRows = this.GetNotEmptyRows(sheetIndex);
            return Math.Min(this._workbook.Sheets[sheetIndex].RowCount - 1, (notEmptyRows.Count > 0) ? Enumerable.Max((IEnumerable<int>)notEmptyRows) : 0);
        }

        public List<IFunction> GetCustomOrFunctionNameList()
        {
            return null;
        }

        public double GetDefaultColumnWidth(short sheet)
        {
            return this.CalcColumnWidth(this._workbook.Sheets[sheet].DefaultColumnWidth);
        }

        public string GetDefaultPivotTableStyleName()
        {
            return "PivotStyleMedium9";
        }

        public double GetDefaultRowHeight(short sheet, ref bool customHeight)
        {
            if (this._saveFlags.AutoRowHeight())
            {
                customHeight = false;
                return this.CalculateDefaultRowHeight(sheet);
            }
            double num = UnitHelper.PixelToPoint(this._workbook.Sheets[sheet].DefaultRowHeight);
            if (Math.Abs((double)(this.CalculateDefaultRowHeight(sheet) - num)) < 2.0)
            {
                customHeight = false;
                return num;
            }
            customHeight = true;
            return num;
        }

        public string GetDefaultTableStyleName()
        {
            string name = this._workbook.DefaultTableStyle.Name;
            if (this.BuiltInTableStyleNames.Contains(name.ToUpperInvariant()))
            {
                return ("TableStyle" + name);
            }
            return name;
        }

        void GetDifferentConditionalFormatFormConditionalFormat(List<StyleInfo> styles)
        {
            for (int i = 0; i < this._workbook.SheetCount; i++)
            {
                Worksheet worksheet = this._workbook.Sheets[i];
                short identfier = 0;
                if ((worksheet.ConditionalFormats != null) && (worksheet.ConditionalFormats.RuleCount != 0))
                {
                    if (!this._conditinalFormats.ContainsKey(i))
                    {
                        this._conditinalFormats.Add(i, new List<IExcelConditionalFormat>());
                    }
                    else
                    {
                        this._conditinalFormats[i].Clear();
                    }
                    for (int j = 0; j < worksheet.ConditionalFormats.RuleCount; j++)
                    {
                        FormattingRuleBase item = worksheet.ConditionalFormats[j];
                        if ((item.Ranges != null) && (item.Ranges.Length != 0))
                        {
                            try
                            {
                                if (item is CellValueRule)
                                {
                                    this.ProcessConditionalFormatCellValueRule(styles, i, ref identfier, item);
                                }
                                else if (item is FormulaRule)
                                {
                                    this.ProcessConditionalFormatFormulaRule(styles, i, ref identfier, item);
                                }
                                else if (item is Top10Rule)
                                {
                                    this.ProcessConditionalFormatTop10Rule(styles, i, ref identfier, item);
                                }
                                else if ((item is UniqueRule) || (item is DuplicateRule))
                                {
                                    this.ProcessConditionalFormatUniqueRule(styles, i, ref identfier, item);
                                }
                                else if (item is SpecificTextRule)
                                {
                                    this.ProcessConditionalFormatSpecificTextRule(styles, i, ref identfier, item);
                                }
                                else if (item is AverageRule)
                                {
                                    this.ProcessConditionalFormatAverageRule(styles, i, ref identfier, item);
                                }
                                else if (item is DateOccurringRule)
                                {
                                    this.ProcessConditionalFormatDateOccurringRule(styles, i, ref identfier, item);
                                }
                                else if (item is ScaleRule)
                                {
                                    this.ProcessConditionalFormatScaleRule(i, ref identfier, item);
                                }
                                else if (item is IconSetRule)
                                {
                                    this.ProcessConditionalFormatIconSetRule(i, ref identfier, item);
                                }
                            }
                            catch (Exception exception)
                            {
                                this.OnExcelSaveError(new ExcelWarning(ResourceStrings.ExcelWriterWriteConditionalFormatError, ExcelWarningCode.General, i, item.Ranges[0].Row, item.Ranges[0].Column, exception));
                            }
                        }
                    }
                }
            }
        }

        void GetDifferentConditionalFormatFromTable(List<StyleInfo> styles)
        {
            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < this._workbook.SheetCount; i++)
            {
                SheetTable[] tables = this._workbook.Sheets[i].GetTables();
                if ((tables != null) && (tables.Length > 0))
                {
                    foreach (SheetTable table in tables)
                    {
                        if (table.Style != null)
                        {
                            string str = table.Style.Name.ToUpperInvariant();
                            if (!this.BuiltInTableStyleNames.Contains(str) && !set.Contains(str))
                            {
                                set.Add(table.Style.Name.ToUpperInvariant());
                                ExcelTableStyle excelTableStyle = this.GetExcelTableStyle(styles, table.Style);
                                this._tableStyles.Add(excelTableStyle);
                            }
                        }
                    }
                }
            }
            if (TableStyles.CustomStyles != null)
            {
                foreach (TableStyle style2 in TableStyles.CustomStyles)
                {
                    if (!set.Contains(style2.Name.ToUpperInvariant()))
                    {
                        set.Add(style2.Name.ToUpperInvariant());
                        ExcelTableStyle style3 = this.GetExcelTableStyle(styles, style2);
                        this._tableStyles.Add(style3);
                    }
                }
            }
        }

        public List<IDifferentialFormatting> GetDifferentialFormattingRecords()
        {
            if ((this._differenceFormats != null) && (this._differenceFormats.Count > 0))
            {
                return this._differenceFormats;
            }
            return null;
        }

        public void GetDimensions(short sheet, ref int row, ref int column)
        {
            int rowCount = 0;
            int columnCount = 0;
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if (this._saveFlags.SaveCustomColumnHeaders())
            {
                rowCount = worksheet.ColumnHeader.RowCount;
            }
            if (this._saveFlags.SaveCustomRowHeaders())
            {
                columnCount = worksheet.RowHeader.ColumnCount;
            }
            row = worksheet.RowCount + rowCount;
            column = worksheet.ColumnCount + columnCount;
        }

        public void GetDisplayElements(short sheet, ref bool showFormula, ref bool showZeros, ref bool showGridLine, ref bool showRowColumnHeader, ref bool rightToLeftColumns)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            showFormula = false;
            showGridLine = worksheet.ShowGridLine;
            showRowColumnHeader = true;
            if (!worksheet.RowHeader.IsVisible && !worksheet.ColumnHeader.IsVisible)
            {
                showRowColumnHeader = false;
            }
            showZeros = true;
            rightToLeftColumns = false;
        }

        IAnchor GetExcelAnchor(FloatingObject floatingObject, ExcelSheetType sheetType)
        {
            if (sheetType == ExcelSheetType.ChartSheet)
            {
                return new AbsoluteAnchor { X = 0.0, Y = 0.0, Height = floatingObject.Size.Height, Width = floatingObject.Size.Width };
            }
            return new TwoCellAnchor { FromRow = floatingObject.StartRow, FromColumn = floatingObject.StartColumn, ToRow = floatingObject.EndRow, ToColumn = floatingObject.EndColumn, FromRowOffset = floatingObject.StartRowOffset, FromColumnOffset = floatingObject.StartColumnOffset, ToRowOffset = floatingObject.EndRowOffset, ToColumnOffset = floatingObject.EndColumnOffset };
        }

        public List<IExtendedFormat> GetExcelCellFormats()
        {
            List<IExtendedFormat> list = new List<IExtendedFormat>();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            string str = "__builtInStyle";
            foreach (StyleInfo info in this._styles)
            {
                IExtendedFormat format = info.ToExtendedFormat(this._workbook);
                if ((info.Name != "") && !info.Name.StartsWith(str))
                {
                    if (!dictionary.ContainsKey(info.Name))
                    {
                        dictionary.Add(info.Name, dictionary.Count);
                    }
                    format.IsStyleFormat = true;
                }
                else
                {
                    int num = 0;
                    dictionary.TryGetValue(info.Parent, out num);
                    format.ParentFormatID = new int?(num);
                }
                list.Add(format);
            }
            return list;
        }

        IExcelColor GetExcelColor(Windows.UI.Color color)
        {
            color = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, color);
            return new ExcelColor(GcColor.FromArgb(color.A, color.R, color.G, color.B));
        }

        public IExtendedFormat GetExcelDefaultCellFormat()
        {
            StyleInfo defaultStyle = this._workbook.DefaultStyle.Clone() as StyleInfo;
            if (string.IsNullOrWhiteSpace(defaultStyle.FontTheme) && (defaultStyle.FontFamily == null))
            {
                defaultStyle.FontFamily = this._workbook.CurrentTheme.BodyFontFamily;
            }
            IExtendedFormat result = null;
            if ((defaultStyle.Background is SolidColorBrush)
                && ((defaultStyle.Background as SolidColorBrush).Color == Windows.UI.Color.FromArgb(0, 0xff, 0xff, 0xff))
                && (((defaultStyle.FontFamily != null) ? defaultStyle.FontFamily.Source : NameConstans.DEFAULT_FONT_FAMILY) == ((this._workbook.CurrentTheme.BodyFontFamily != null) ? this._workbook.CurrentTheme.BodyFontFamily.Source : NameConstans.DEFAULT_FONT_FAMILY))
                && (defaultStyle.FontSize == DefaultStyleCollection.DefaultFontSize))
            {
                defaultStyle.Background = null;
            }
            result = defaultStyle.ToExtendedFormat(this._workbook);
            result.ParentFormatID = 0;
            return result;
        }

        ExcelIconSetType GetExcelIconSetType(IconSetType setType)
        {
            switch (setType)
            {
                case IconSetType.ThreeArrowsColored:
                    return ExcelIconSetType.Icon_3Arrows;

                case IconSetType.ThreeArrowsGray:
                    return ExcelIconSetType.Icon_3ArrowsGray;

                case IconSetType.ThreeTriangles:
                    return ExcelIconSetType.Icon_3Triangles;

                case IconSetType.ThreeStars:
                    return ExcelIconSetType.Icon_3Stars;

                case IconSetType.ThreeFlags:
                    return ExcelIconSetType.Icon_3Flags;

                case IconSetType.ThreeTrafficLightsUnrimmed:
                    return ExcelIconSetType.Icon_3TrafficLights1;

                case IconSetType.ThreeTrafficLightsRimmed:
                    return ExcelIconSetType.Icon_3TrafficLights2;

                case IconSetType.ThreeSigns:
                    return ExcelIconSetType.Icon_3Signs;

                case IconSetType.ThreeSymbolsCircled:
                    return ExcelIconSetType.Icon_3Symbols;

                case IconSetType.ThreeSymbolsUncircled:
                    return ExcelIconSetType.Icon_3Symbols2;

                case IconSetType.FourArrowsColored:
                    return ExcelIconSetType.Icon_4Arrows;

                case IconSetType.FourArrowsGray:
                    return ExcelIconSetType.Icon_4ArrowsGray;

                case IconSetType.FourRedToBlack:
                    return ExcelIconSetType.Icon_4RedToBlack;

                case IconSetType.FourRatings:
                    return ExcelIconSetType.Icon_4Rating;

                case IconSetType.FourTrafficLights:
                    return ExcelIconSetType.Icon_4TrafficLights;

                case IconSetType.FiveArrowsColored:
                    return ExcelIconSetType.Icon_5Arrows;

                case IconSetType.FiveArrowsGray:
                    return ExcelIconSetType.Icon_5ArrowsGray;

                case IconSetType.FiveRatings:
                    return ExcelIconSetType.Icon_5Rating;

                case IconSetType.FiveQuarters:
                    return ExcelIconSetType.Icon_5Quarters;

                case IconSetType.FiveBoxes:
                    return ExcelIconSetType.Icon_5Boxes;
            }
            return ExcelIconSetType.Icon_NIL;
        }

        IExcelColor GetExcelRGBColor(Windows.UI.Color color)
        {
            Windows.UI.Color color2 = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, color);
            return new ExcelColor(GcColor.FromArgb(color2.A, color2.R, color2.G, color2.B));
        }

        IExcelSparklineGroup GetExcelSparkLineGroup(int sheet, SparklineGroup sparklineGroup)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            ExcelSparklineGroup group = new ExcelSparklineGroup
            {
                ShowXAxis = sparklineGroup.Setting.DisplayXAxis,
                ShowFirstDifferently = sparklineGroup.Setting.ShowFirst,
                ShowLastDifferently = sparklineGroup.Setting.ShowLast,
                ShowHighestDifferently = sparklineGroup.Setting.ShowHigh,
                ShowLowestDifferently = sparklineGroup.Setting.ShowLow,
                ShowMarkers = sparklineGroup.Setting.ShowMarkers,
                ShowNegativeDifferently = sparklineGroup.Setting.ShowNegative,
                ShowHidden = sparklineGroup.Setting.DisplayHidden,
                LineWeight = (sparklineGroup.Setting.LineWeight * 3.0) / 4.0,
                DisplayEmptyCellAs = (ExcelSparklineEmptyCellDisplayAs)sparklineGroup.Setting.DisplayEmptyCellsAs,
                RightToLeft = sparklineGroup.Setting.RightToLeft
            };
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.AxisThemeColor))
            {
                group.AxisColor = sparklineGroup.Setting.AxisThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color axisColor = sparklineGroup.Setting.AxisColor;
                group.AxisColor = this.GetExcelColor(sparklineGroup.Setting.AxisColor);
            }
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.FirstMarkerThemeColor))
            {
                group.FirstColor = sparklineGroup.Setting.FirstMarkerThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color firstMarkerColor = sparklineGroup.Setting.FirstMarkerColor;
                group.FirstColor = this.GetExcelColor(sparklineGroup.Setting.FirstMarkerColor);
            }
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.HighMarkerThemeColor))
            {
                group.HighColor = sparklineGroup.Setting.HighMarkerThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color highMarkerColor = sparklineGroup.Setting.HighMarkerColor;
                group.HighColor = this.GetExcelColor(sparklineGroup.Setting.HighMarkerColor);
            }
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.LastMarkerThemeColor))
            {
                group.LastColor = sparklineGroup.Setting.LastMarkerThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color lastMarkerColor = sparklineGroup.Setting.LastMarkerColor;
                group.LastColor = this.GetExcelColor(sparklineGroup.Setting.LastMarkerColor);
            }
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.LowMarkerThemeColor))
            {
                group.LowColor = sparklineGroup.Setting.LowMarkerThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color lowMarkerColor = sparklineGroup.Setting.LowMarkerColor;
                group.LowColor = this.GetExcelColor(sparklineGroup.Setting.LowMarkerColor);
            }
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.MarkersThemeColor))
            {
                group.MarkersColor = sparklineGroup.Setting.MarkersThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color markersColor = sparklineGroup.Setting.MarkersColor;
                group.MarkersColor = this.GetExcelColor(sparklineGroup.Setting.MarkersColor);
            }
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.NegativeThemeColor))
            {
                group.NegativeColor = sparklineGroup.Setting.NegativeThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color negativeColor = sparklineGroup.Setting.NegativeColor;
                group.NegativeColor = this.GetExcelColor(sparklineGroup.Setting.NegativeColor);
            }
            if (!string.IsNullOrEmpty(sparklineGroup.Setting.SeriesThemeColor))
            {
                group.SeriesColor = sparklineGroup.Setting.SeriesThemeColor.GetExcelThemeColor();
            }
            else
            {
                Windows.UI.Color seriesColor = sparklineGroup.Setting.SeriesColor;
                group.SeriesColor = this.GetExcelColor(sparklineGroup.Setting.SeriesColor);
            }
            group.MaxAxisType = (ExcelSparklineAxisMinMax)sparklineGroup.Setting.MaxAxisType;
            group.MinAxisType = (ExcelSparklineAxisMinMax)sparklineGroup.Setting.MinAxisType;
            if (group.MaxAxisType == ExcelSparklineAxisMinMax.Custom)
            {
                group.ManualMaxValue = sparklineGroup.Setting.ManualMax;
            }
            if (group.MinAxisType == ExcelSparklineAxisMinMax.Custom)
            {
                group.ManualMinValue = sparklineGroup.Setting.ManualMin;
            }
            if (sparklineGroup.Count > 0)
            {
                IEnumerator<Sparkline> enumerator = sparklineGroup.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Sparkline sparkline = enumerator.Current;
                    group.SparklineType = (ExcelSparklineType)sparkline.SparklineType;
                    ExcelSparkline sparkline2 = new ExcelSparkline();
                    ExcelCellRange range5 = new ExcelCellRange
                    {
                        Row = sparkline.Row,
                        Column = sparkline.Column,
                        RowSpan = 1,
                        ColumnSpan = 1
                    };
                    sparkline2.Location = range5;
                    CellRange sparklineDataRange = worksheet.Cells[sparkline.Row, sparkline.Column].SparklineDataRange;
                    CellRange sparklineDateAxisRange = worksheet.Cells[sparkline.Row, sparkline.Column].SparklineDateAxisRange;
                    if (sparklineDateAxisRange != null)
                    {
                        group.IsDateAxis = true;
                        ExternalCellRange range3 = new ExternalCellRange
                        {
                            Row = sparklineDateAxisRange.Row,
                            Column = sparklineDateAxisRange.Column,
                            RowSpan = sparklineDateAxisRange.RowCount,
                            ColumnSpan = sparklineDateAxisRange.ColumnCount,
                            WorksheetName = this._workbook.Sheets[sheet].Name
                        };
                        group.DateAxisRange = range3;
                    }
                    ExternalCellRange range4 = new ExternalCellRange
                    {
                        WorksheetName = worksheet.Name,
                        Row = sparklineDataRange.Row,
                        Column = sparklineDataRange.Column,
                        RowSpan = sparklineDataRange.RowCount,
                        ColumnSpan = sparklineDataRange.ColumnCount
                    };
                    sparkline2.DataRange = range4;
                    group.Sparklines.Add(sparkline2);
                }
            }
            return group;
        }

        public List<IExcelSparklineGroup> GetExcelSparkLineGroups(int sheet)
        {
            if (this._saveFlags.SaveDataOnly())
            {
                return null;
            }
            if (!this._sparklineGroups.ContainsKey(sheet))
            {
                return null;
            }
            List<SparklineGroup> list = this._sparklineGroups[sheet];
            if (list.Count == 0)
            {
                return null;
            }
            List<IExcelSparklineGroup> list2 = new List<IExcelSparklineGroup>();
            foreach (SparklineGroup group in list)
            {
                list2.Add(this.GetExcelSparkLineGroup(sheet, group));
            }
            return list2;
        }

        /// <summary>
        /// hdt 唐忠宝修改
        /// </summary>
        /// <returns></returns>
        public List<IExcelStyle> GetExcelStyles()
        {
            List<IExcelStyle> list = new List<IExcelStyle>();
            IExcelStyle style = null;
            if (this._namedStyles != null)
            {
                for (int i = 0; i < this._namedStyles.Count; i++)
                {
                    if (this._namedStyles[i].Name == "Normal")
                    {
                        ExcelStyle style2 = new ExcelStyle
                        {
                            Name = "Normal",
                            Format = this._namedStyles[i].ToExtendedFormat(this._workbook)
                        };
                        style2.Format.IsStyleFormat = true;
                        style2.BuiltInStyle = BuiltInStyleIndex.Normal;
                        style = style2;
                        this._normalStyleInfo = this._namedStyles[i];
                        break;
                    }
                }
            }
            if (style == null)
            {
                ExcelStyle style3 = new ExcelStyle
                {
                    Name = "Normal"
                };
                StyleInfo styleInfo = this._workbook.DefaultStyle.Clone() as StyleInfo;
                styleInfo.FontFamily = this._workbook.CurrentTheme.BodyFontFamily;
                style3.Format = styleInfo.ToExtendedFormat(this._workbook);
                style3.Format.Font.FontSize = DefaultStyleCollection.DefaultFontSize;
                style3.Format.IsStyleFormat = true;
                style3.BuiltInStyle = BuiltInStyleIndex.Normal;
                list.Add(style3);
            }
            if (this._namedStyles != null)
            {
                foreach (StyleInfo info2 in this._namedStyles)
                {
                    if ((info2.Name != null) && !info2.Name.StartsWith("__builtInStyle"))
                    {
                        if (_builtInStyle.ContainsKey(info2.Name))
                        {
                            ExcelStyle style4 = new ExcelStyle
                            {
                                Name = info2.Name,
                                Format = info2.ToExtendedFormat(this._workbook)
                            };
                            style4.Format.IsStyleFormat = true;
                            style4.BuiltInStyle = (BuiltInStyleIndex)_builtInStyle[info2.Name];
                            if ((info2.Name.StartsWith("RowLevel_") || info2.Name.StartsWith("ColLevel_")) && (info2.Name.Length > 9))
                            {
                                style4.OutLineLevel = (byte)(int.Parse(info2.Name.Substring(9)) - 1);
                            }
                            list.Add(style4);
                        }
                        else
                        {
                            CustomExcelStyle style5 = new CustomExcelStyle
                            {
                                Name = info2.Name,
                                Format = info2.ToExtendedFormat(this._workbook)
                            };
                            style5.Format.IsStyleFormat = true;
                            list.Add(style5);
                        }
                    }
                }
            }
            return list;
        }

        IExcelTable GetExcelTable(int sheet, SheetTable table)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            ExcelTable table2 = new ExcelTable
            {
                Name = table.Name.Replace(' ', '_')
            };
            string name = table2.Name;
            if (this.CellRangeNames.Contains(name.ToUpperInvariant()))
            {
                int num = 1;
                while (true)
                {
                    name = name + "_" + ((int)num);
                    if (!this.CellRangeNames.Contains(name.ToUpperInvariant()))
                    {
                        this.CellRangeNames.Add(name.ToUpperInvariant());
                        break;
                    }
                    num++;
                }
            }
            table2.DisplayName = name;
            table2.Id = this.GetUniqueTableID(table.Name);
            table2.ShowHeaderRow = table.ShowHeader;
            table2.ShowTotalsRow = table.ShowFooter;
            CellRange range = table.Range;
            table2.Range = this.CreateCellRange(range.Row, range.Column, range.RowCount, range.ColumnCount);
            for (int i = 1; i <= range.ColumnCount; i++)
            {
                ExcelTableColumn column = new ExcelTableColumn();
                column.Id = i;
                column.Name = table.GetColumnName(i - 1);

                SheetTable.TableColumn column2 = table.GetColumn(i - 1);
                if (column2 != null)
                {
                    string totalsRowCustomFunction = null;
                    if (!string.IsNullOrWhiteSpace(column2.TotalsRowCustomFunction))
                    {
                        totalsRowCustomFunction = column2.TotalsRowCustomFunction;
                    }
                    else if (!string.IsNullOrWhiteSpace(column2.Formula))
                    {
                        totalsRowCustomFunction = column2.Formula;
                    }
                    if (!string.IsNullOrWhiteSpace(totalsRowCustomFunction))
                    {
                        if (totalsRowCustomFunction.StartsWith("{") && totalsRowCustomFunction.EndsWith("}"))
                        {
                            totalsRowCustomFunction = totalsRowCustomFunction.Substring(1, totalsRowCustomFunction.Length - 2);
                            column.TotalsRowFunctionIsArrayFormula = true;
                        }
                        column.TotalsRowFunction = this.GetExcelTableTotalsRowFunction(totalsRowCustomFunction);
                        if (column.TotalsRowFunction == ExcelTableTotalsRowFunction.Custom)
                        {
                            column.TotalsRowCustomFunction = totalsRowCustomFunction;
                        }
                    }
                    column.TotalsRowFunctionIsArrayFormula = column2.TotalsRowFunctionIsArrayFormula;
                    if (!string.IsNullOrWhiteSpace(column2.TotalsRowFunction))
                    {
                        ExcelTableTotalsRowFunction none = ExcelTableTotalsRowFunction.None;
                        Enum.TryParse<ExcelTableTotalsRowFunction>(column2.TotalsRowFunction, true, out none);
                        column.TotalsRowFunction = none;
                        if (column.TotalsRowFunction == ExcelTableTotalsRowFunction.Custom)
                        {
                            column.TotalsRowCustomFunction = totalsRowCustomFunction;
                        }
                    }
                    column.TotalsRowLabel = column2.TotalsRowLabel;
                    column.CalculatedColumnFormula = column2.CalculatedColumnFormula;
                    column.CalculatedColumnFormulaIsArrayFormula = column2.CalculatedColumnFormulaIsArrayFormula;
                }
                
                table2.Columns.Add(column);
            }
            ExcelTableStyleInfo info = new ExcelTableStyleInfo
            {
                ShowColumnStripes = table.BandedColumns,
                ShowRowStripes = table.BandedRows,
                ShowFirstColumn = table.HighlightFirstColumn,
                ShowLastColumn = table.HighlightLastColumn
            };
            if (table.Style != null)
            {
                if (this.BuiltInTableStyleNames.Contains(table.Style.Name.ToUpperInvariant()))
                {
                    info.Name = "TableStyle" + table.Style.Name;
                }
                else
                {
                    info.Name = table.Style.Name;
                }
            }
            table2.TableStyleInfo = info;
            if ((table.ShowHeader && (table.RowFilter != null)) && table.RowFilter.ShowFilterButton)
            {
                ExcelAutoFilter filter = new ExcelAutoFilter
                {
                    Range = this.ExpandRange(table.RowFilter.Range, -1, 0)
                };
                if (this._tableColorFilters.ContainsKey(table.Name) && (this._tableColorFilters[table.Name].Count > 0))
                {
                    if (filter.FilterColumns == null)
                    {
                        filter.FilterColumns = new List<IExcelFilterColumn>();
                    }
                    filter.FilterColumns.AddRange(this._tableColorFilters[table.Name]);
                }
                for (int j = 0; j < range.ColumnCount; j++)
                {
                    ReadOnlyCollection<ConditionBase> filterItems = table.RowFilter.GetFilterItems(range.Column + j);
                    if ((filterItems != null) && (filterItems.Count > 0))
                    {
                        ConditionBase base2 = null;
                        if (filterItems.Count > 0)
                        {
                            base2 = filterItems[0];
                            for (int k = 1; k < filterItems.Count; k++)
                            {
                                base2 = new RelationCondition(RelationCompareType.Or, base2, filterItems[k]);
                            }
                        }
                        else
                        {
                            base2 = filterItems[0];
                        }
                        if (base2 != null)
                        {
                            ExcelFilterColumn filterColumn = new ExcelFilterColumn
                            {
                                AutoFilterColumnId = (uint)j
                            };
                            this.AddFilterToFilterColumn(filterColumn, base2.ToExcelFilter(worksheet, new CellRange(table2.Range.Row, table2.Range.Column + j, table2.Range.RowSpan, 1)));
                            filter.FilterColumns.Add(filterColumn);
                        }
                    }
                }
                table2.AutoFilter = filter;
            }
            return table2;
        }

        ExcelTableStyle GetExcelTableStyle(List<StyleInfo> styles, TableStyle tableStyle)
        {
            ExcelTableStyle style = new ExcelTableStyle
            {
                Name = tableStyle.Name,
                IsTableStyle = true,
                IsPivotStyle = false
            };
            if (tableStyle.WholeTableStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.WholeTable, this.InsertStyleToUniqueCollection(styles, tableStyle.WholeTableStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.HeaderRowStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.HeaderRow, this.InsertStyleToUniqueCollection(styles, tableStyle.HeaderRowStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.FooterRowStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.TotalRow, this.InsertStyleToUniqueCollection(styles, tableStyle.FooterRowStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.HighlightFirstColumnStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.FirstColumn, this.InsertStyleToUniqueCollection(styles, tableStyle.HighlightFirstColumnStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.HighlightLastColumnStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.LastColumn, this.InsertStyleToUniqueCollection(styles, tableStyle.HighlightLastColumnStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.FirstRowStripStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.FirstRowStripe, this.InsertStyleToUniqueCollection(styles, tableStyle.FirstRowStripStyle.ToStyleInfo(this._workbook)), Math.Min(9, tableStyle.FirstRowStripSize)));
            }
            if (tableStyle.SecondRowStripStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.SecondRowStripe, this.InsertStyleToUniqueCollection(styles, tableStyle.SecondRowStripStyle.ToStyleInfo(this._workbook)), Math.Min(9, tableStyle.SecondRowStripSize)));
            }
            if (tableStyle.FirstColumnStripStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.FirstColumnStripe, this.InsertStyleToUniqueCollection(styles, tableStyle.FirstColumnStripStyle.ToStyleInfo(this._workbook)), Math.Min(9, tableStyle.FirstColumnStripSize)));
            }
            if (tableStyle.SecondColumnStripStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.SecondColumnStripe, this.InsertStyleToUniqueCollection(styles, tableStyle.SecondColumnStripStyle.ToStyleInfo(this._workbook)), Math.Min(9, tableStyle.SecondColumnStripSize)));
            }
            if (tableStyle.FirstHeaderCellStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.FirstHeaderCell, this.InsertStyleToUniqueCollection(styles, tableStyle.FirstHeaderCellStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.LastHeaderCellStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.LastHeaderCell, this.InsertStyleToUniqueCollection(styles, tableStyle.LastHeaderCellStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.FirstFooterCellStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.FirstTotalCell, this.InsertStyleToUniqueCollection(styles, tableStyle.FirstFooterCellStyle.ToStyleInfo(this._workbook)), 1));
            }
            if (tableStyle.LastFooterCellStyle != null)
            {
                style.TableStyleElements.Add(this.GetTableStyleElement(ExcelTableElementType.LastTotalCell, this.InsertStyleToUniqueCollection(styles, tableStyle.LastFooterCellStyle.ToStyleInfo(this._workbook)), 1));
            }
            return style;
        }

        ExcelTableTotalsRowFunction GetExcelTableTotalsRowFunction(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                return ExcelTableTotalsRowFunction.None;
            }
            formula = formula.Trim().ToUpperInvariant();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < formula.Length; i++)
            {
                if (formula[i] != ' ')
                {
                    builder.Append(formula[i]);
                }
            }
            formula = builder.ToString();
            if (formula.StartsWith("SUBTOTAL("))
            {
                formula = formula.Substring(9);
                string listSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                string[] strArray = new string[] { "(", listSeparator, ")" };
                string[] strArray2 = formula.Split(strArray, (StringSplitOptions)StringSplitOptions.None);
                if (strArray2.Length < 2)
                {
                    return ExcelTableTotalsRowFunction.Custom;
                }
                switch (strArray2[0].Trim())
                {
                    case "101":
                        return ExcelTableTotalsRowFunction.Average;

                    case "103":
                        return ExcelTableTotalsRowFunction.Count;

                    case "102":
                        return ExcelTableTotalsRowFunction.CountNums;

                    case "104":
                        return ExcelTableTotalsRowFunction.Max;

                    case "105":
                        return ExcelTableTotalsRowFunction.Min;

                    case "109":
                        return ExcelTableTotalsRowFunction.Sum;

                    case "107":
                        return ExcelTableTotalsRowFunction.StdDev;

                    case "110":
                        return ExcelTableTotalsRowFunction.Var;
                }
            }
            return ExcelTableTotalsRowFunction.Custom;
        }

        public List<IExternalWorkbookInfo> GetExternWorkbookInfo()
        {
            return null;
        }

        FontFamily GetFontFamily(int sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            FontFamily fontFamily = null;
            if (worksheet.DefaultStyle.IsFontFamilySet())
            {
                fontFamily = worksheet.DefaultStyle.FontFamily;
            }
            else if (this._workbook.DefaultStyle.IsFontFamilySet())
            {
                fontFamily = this._workbook.DefaultStyle.FontFamily;
            }
            else
            {
                fontFamily = DefaultStyleCollection.DefaultFontFamily;
            }
            return fontFamily;
        }

        IFontScheme GetFontScheme(SpreadTheme spreadTheme)
        {
            IThemeFonts themeFonts = this.GetThemeFonts(spreadTheme.HeadingFontName);
            return new FontScheme(spreadTheme.Name, themeFonts, this.GetThemeFonts(spreadTheme.BodyFontName));
        }

        double GetFontSize(int sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if (worksheet.DefaultStyle.IsFontSizeSet())
            {
                return worksheet.DefaultStyle.FontSize;
            }
            if (this._workbook.DefaultStyle.IsFontSizeSet())
            {
                return this._workbook.DefaultStyle.FontSize;
            }
            return DefaultStyleCollection.DefaultFontSize;
        }

        Tuple<string, CellRange> GetFormula(int sheet, int row, int column)
        {
            Dictionary<long, Tuple<string, CellRange>> dictionary = null;
            int columnCount = this._workbook.Sheets[sheet].ColumnCount;
            int rowCount = this._workbook.Sheets[sheet].RowCount;
            if (!this._formulas.TryGetValue(sheet, out dictionary))
            {
                object[,] objArray = this._workbook.Sheets[sheet].FindFormulas(-1, -1, 1, 1);
                if ((objArray == null) || (objArray.Length == 0))
                {
                    this._formulas.Add(sheet, null);
                }
                else
                {
                    this._formulas.Add(sheet, new Dictionary<long, Tuple<string, CellRange>>());
                    int num3 = objArray.Length / objArray.Rank;
                    for (int i = 0; i < num3; i++)
                    {
                        string str = objArray[i, 1].ToString();
                        CellRange range = objArray[i, 0] as CellRange;
                        if ((range.RowCount == 1) && (range.ColumnCount == 1))
                        {
                            int num5 = (range.Row * columnCount) + range.Column;
                            if (!this._formulas[sheet].ContainsKey((long)num5))
                            {
                                this._formulas[sheet].Add((long)num5, new Tuple<string, CellRange>(str, range));
                            }
                        }
                        else if ((range.RowCount == -1) && (range.ColumnCount >= 0))
                        {
                            for (int j = 0; j < rowCount; j++)
                            {
                                int num7 = (j * columnCount) + range.Column;
                                if (!this._formulas[sheet].ContainsKey((long)num7))
                                {
                                    this._formulas[sheet].Add((long)num7, new Tuple<string, CellRange>(str, new CellRange(j, range.Column, 1, 1)));
                                }
                            }
                        }
                        else if ((range.ColumnCount == -1) && (range.RowCount >= 0))
                        {
                            for (int k = 0; k < columnCount; k++)
                            {
                                int num9 = (range.Row * columnCount) + k;
                                if (!this._formulas[sheet].ContainsKey((long)num9))
                                {
                                    this._formulas[sheet].Add((long)num9, new Tuple<string, CellRange>(str, new CellRange(range.Row, k, 1, 1)));
                                }
                            }
                        }
                        else
                        {
                            for (int m = 0; m < range.RowCount; m++)
                            {
                                for (int n = 0; n < range.ColumnCount; n++)
                                {
                                    int num12 = (columnCount * (range.Row + m)) + (range.Column + n);
                                    if (!this._formulas[sheet].ContainsKey((long)num12))
                                    {
                                        this._formulas[sheet].Add((long)num12, new Tuple<string, CellRange>(str, range));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this._formulas.TryGetValue(sheet, out dictionary);
            if (dictionary == null)
            {
                return null;
            }
            Tuple<string, CellRange> tuple = null;
            dictionary.TryGetValue((long)((columnCount * row) + column), out tuple);
            return tuple;
        }

        public void GetFrozen(short sheet, ref int frozenRowCount, ref int frozenColumnCount, ref int frozenTrailingRowCount, ref int frozenTrailingColumnCount)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if (this._saveFlags.SaveDataOnly())
            {
                frozenRowCount = 0;
                frozenColumnCount = 0;
                frozenTrailingColumnCount = 0;
                frozenTrailingRowCount = 0;
            }
            else
            {
                if (this._saveFlags.SaveCustomColumnHeaders())
                {
                    frozenRowCount = worksheet.ColumnHeader.RowCount + worksheet.FrozenRowCount;
                }
                else
                {
                    frozenRowCount = worksheet.FrozenRowCount;
                }
                if (this._saveFlags.SaveCustomRowHeaders())
                {
                    frozenColumnCount = worksheet.RowHeader.ColumnCount + worksheet.FrozenColumnCount;
                }
                else
                {
                    frozenColumnCount = worksheet.FrozenColumnCount;
                }
                frozenTrailingColumnCount = worksheet.FrozenTrailingColumnCount;
                frozenTrailingRowCount = worksheet.FrozenTrailingRowCount;
            }
        }

        public IExcelColor GetGridlineColor(short sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if (worksheet.GridLineColor != Worksheet.DefaultGridLineColor)
            {
                int num = Dt.Cells.Data.ColorHelper.GetPaletteColorIndex(this._workbook, this._workbook.Sheets[sheet].GridLineColor.ToExcelColor(), 0x3f);
                return new ExcelColor(ExcelColorType.Indexed, (uint)num, 0.0);
            }
            return null;
        }

        public void GetGutters(short sheet, ref int rowMaxOutLineLevel, ref int columnMaxOutLineLevel)
        {
            if (this._saveFlags.SaveDataOnly())
            {
                rowMaxOutLineLevel = 0;
                columnMaxOutLineLevel = 0;
            }
            else
            {
                int maxLevel = this._workbook.Sheets[sheet].RowRangeGroup.GetMaxLevel();
                if (maxLevel == -1)
                {
                    rowMaxOutLineLevel = 0;
                }
                else
                {
                    rowMaxOutLineLevel = maxLevel + 2;
                }
                int num2 = this._workbook.Sheets[sheet].ColumnRangeGroup.GetMaxLevel();
                if (num2 == -1)
                {
                    columnMaxOutLineLevel = 0;
                }
                else
                {
                    columnMaxOutLineLevel = num2 + 2;
                }
            }
        }

        public List<IName> GetInternalDefinedNames()
        {
            List<IName> list = new List<IName>();
            bool flag = this._workbook.ReferenceStyle == ReferenceStyle.A1;
            CalcParser parser = new CalcParser();
            if (this._workbook.Names != null)
            {
                WorkbookParserContext context = new WorkbookParserContext(this._workbook, !flag, 0, 0, null);
                foreach (NameInfo info in (IEnumerable<NameInfo>)this._workbook.Names)
                {
                    if (!string.IsNullOrWhiteSpace(info.Name) && (info.Expression != null))
                    {
                        ExcelNamedRange range = new ExcelNamedRange(info.Name, -1);
                        string str = parser.Unparse(info.Expression, context);
                        if (str.StartsWith("!") && (((info.Expression is CalcBangCellExpression) || (info.Expression is CalcBangErrorExpression)) || ((info.Expression is CalcBangNameExpression) || (info.Expression is CalcBangRangeExpression))))
                        {
                            str = "''" + str;
                        }
                        if (flag)
                        {
                            range.RefersTo = str;
                        }
                        else
                        {
                            range.RefersToR1C1 = str;
                        }
                        list.Add(range);
                    }
                }
            }
            for (int i = 0; i < this._workbook.SheetCount; i++)
            {
                Worksheet worksheet = this._workbook.Sheets[i];
                SpreadCalcParserContext context2 = new SpreadCalcParserContext(worksheet, !flag, 0, 0, null);
                foreach (NameInfo info2 in (IEnumerable<NameInfo>)worksheet.Names)
                {
                    if (!string.IsNullOrWhiteSpace(info2.Name) && (info2.Expression != null))
                    {
                        ExcelNamedRange range2 = new ExcelNamedRange(info2.Name, i);
                        string str2 = parser.Unparse(info2.Expression, context2);
                        if (flag)
                        {
                            range2.RefersTo = str2;
                        }
                        else
                        {
                            range2.RefersToR1C1 = str2;
                        }
                        list.Add(range2);
                    }
                }
            }
            return list;
        }

        double GetMaxiumDigitWidth()
        {
            return this.GetMaxiumDigitWidthCore();
        }

        public double GetMaxiumDigitWidthCore()
        {
            if (double.IsNaN(this._maxiumDigitWidth))
            {
                if (this._normalStyleInfo == null)
                {
                    this._normalStyleInfo = this._workbook.DefaultStyle;
                }
                TextBlock block = new TextBlock();
                if (!string.IsNullOrWhiteSpace(this._normalStyleInfo.FontTheme))
                {
                    if (this._normalStyleInfo.FontTheme == "Headings")
                    {
                        block.FontFamily = this._workbook.CurrentTheme.HeadingFontFamily;
                    }
                    else if (this._normalStyleInfo.FontTheme == "Body")
                    {
                        block.FontFamily = this._workbook.CurrentTheme.BodyFontFamily;
                    }
                    else
                    {
                        block.FontFamily = new FontFamily(NameConstans.DEFAULT_FONT_FAMILY);
                    }
                }
                else if (this._normalStyleInfo.FontFamily != null)
                {
                    block.FontFamily = this._normalStyleInfo.FontFamily;
                }
                else
                {
                    block.FontFamily = new FontFamily(NameConstans.DEFAULT_FONT_FAMILY);
                }
                block.FontSize = this._normalStyleInfo.FontSize;
                block.FontWeight = this._normalStyleInfo.FontWeight;
                block.FontStyle = this._normalStyleInfo.FontStyle;
                double num = 0.0;
                for (int j = 0; j < 10; j++)
                {
                    char c = (char)(0x30 + j);
                    string str = (string)new string(c, 1);
                    block.Text = str;
                    block.Measure(new Windows.Foundation.Size(double.MaxValue, double.MaxValue));
                    double num3 = Math.Ceiling(block.ActualWidth);
                    if (num3 > num)
                    {
                        num = num3;
                    }
                }
                this._maxiumDigitWidth = num;
            }
            return this._maxiumDigitWidth;
        }

        public List<IRange> GetMergedCells(short sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            List<IRange> list = new List<IRange>();
            if ((worksheet.SpanModel != null) && !this._saveFlags.SaveDataOnly())
            {
                if (worksheet.SpanModel == null)
                {
                    return list;
                }
                foreach (CellRange range in worksheet.SpanModel)
                {
                    ExcelCellRange range2 = new ExcelCellRange
                    {
                        Row = range.Row,
                        Column = range.Column,
                        RowSpan = range.RowCount,
                        ColumnSpan = range.ColumnCount
                    };
                    list.Add(range2);
                }
            }
            return list;
        }

        public List<IExcelColumn> GetNonEmptyColumns(short sheet)
        {
            List<IExcelColumn> list = new List<IExcelColumn>();
            Worksheet worksheet = this._workbook.Sheets[sheet];
            int maxLevel = worksheet.ColumnRangeGroup.GetMaxLevel();
            int columnCount = worksheet.ColumnCount;
            for (int i = 0; i < columnCount; i++)
            {
                IExcelColumn column = this.GetColumn(sheet, worksheet, i, maxLevel);
                if (column != null)
                {
                    list.Add(column);
                }
            }
            return list;
        }

        public List<IExcelRow> GetNonEmptyRows(short sheet)
        {
            List<IExcelRow> list = new List<IExcelRow>();
            Worksheet worksheet = this._workbook.Sheets[sheet];
            List<int> notEmptyRows = this.GetNotEmptyRows(sheet);
            int num = Math.Min(worksheet.RowCount - 1, (notEmptyRows.Count > 0) ? Enumerable.Max((IEnumerable<int>)notEmptyRows) : 0);
            Dictionary<int, double> dictionary = this._autoRowHeight[sheet];
            int maxRowRangeLevel = worksheet.RowRangeGroup.GetMaxLevel();
            for (int row = 0; row <= num; row++)
            {
                IExcelRow excelRow = this.GetRow(sheet, row, maxRowRangeLevel);
                if (excelRow != null)
                {
                    if (this._saveFlags.AutoRowHeight())
                    {
                        excelRow.Height = dictionary[row];
                        excelRow.CustomHeight = false;
                    }
                    else
                    {
                        excelRow.CustomHeight = true;
                    }
                    list.Add(excelRow);
                }
            }
            return list;
        }

        List<int> GetNotEmptyRows(int sheet)
        {
            List<int> list = null;
            if (this._notEmptyRows.TryGetValue(sheet, out list))
            {
                if (list == null)
                {
                    return new List<int>();
                }
                return list;
            }
            Worksheet worksheet = this._workbook.Sheets[sheet];
            list = Enumerable.ToList<int>(Enumerable.Distinct<int>((IEnumerable<int>)worksheet.GetNonEmptyRows()));
            if (list.Count > 0)
            {
                int row = Enumerable.Max((IEnumerable<int>)list);
                for (int j = 0; j < worksheet.ColumnCount; j++)
                {
                    CellRange range = worksheet.SpanModel.Find(row, j);
                    if (((range != null) && (range.Row == row)) && (range.RowCount > 1))
                    {
                        int num3 = Math.Min((int)(worksheet.RowCount - 1), (int)((row + range.RowCount) - 1));
                        list.AddRange((IEnumerable<int>)Enumerable.ToList<int>(Enumerable.Range(row, (num3 - row) + 1)));
                    }
                }
            }
            RangeGroup rowRangeGroup = worksheet.RowRangeGroup;
            for (int i = 0; i < rowRangeGroup.Count; i++)
            {
                if (rowRangeGroup.GetLevelInternal(i) > 0)
                {
                    list.Add(i);
                }
            }
            list = Enumerable.ToList<int>(Enumerable.Distinct<int>((IEnumerable<int>)list));
            list.Sort();
            this._notEmptyRows.Add(sheet, list);
            return list;
        }

        public void GetOutlineDirection(int sheet, ref bool summaryColumnsRightToDetail, ref bool summaryRowsBelowDetail)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            summaryColumnsRightToDetail = worksheet.ColumnRangeGroup.Direction == RangeGroupDirection.Forward;
            summaryRowsBelowDetail = worksheet.RowRangeGroup.Direction == RangeGroupDirection.Forward;
        }

        double GetPageMarginInExcel(double margin)
        {
            return (margin / 100.0);
        }

        public ExcelPaletteColor GetPaletteColor(IExcelColor color)
        {
            if (color == null)
            {
                return ExcelPaletteColor.Black;
            }
            return (ExcelPaletteColor)Dt.Cells.Data.ColorHelper.GetPaletteColorIndex(this._workbook, color, 0x40);
        }

        public void GetPane(short sheet, ref int horizontalPosition, ref int verticalPosition, ref int topVisibleRow, ref int leftmostVisibleColumn, ref int paneIndex)
        {
            int frozenRowCount = 0;
            int frozenColumnCount = 0;
            int frozenTrailingRowCount = 0;
            int frozenTrailingColumnCount = 0;
            this.GetFrozen(sheet, ref frozenRowCount, ref frozenColumnCount, ref frozenTrailingRowCount, ref frozenTrailingColumnCount);
            if ((frozenRowCount > 0) || (frozenColumnCount > 0))
            {
                horizontalPosition = 0;
                verticalPosition = 0;
                topVisibleRow = 0;
                leftmostVisibleColumn = 0;
            }
            else if (!this._saveFlags.SaveCustomColumnHeaders() && !this._saveFlags.SaveCustomRowHeaders())
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                ViewportInfo viewportInfo = worksheet.GetViewportInfo();
                float zoomFactor = worksheet.ZoomFactor;
                int num6 = 0x1a;
                int num7 = 20;
                if (viewportInfo != null)
                {
                    int rowViewportCount = viewportInfo.RowViewportCount;
                    int columnViewportCount = viewportInfo.ColumnViewportCount;
                    if ((rowViewportCount >= 2) || (columnViewportCount >= 2))
                    {
                        bool flag = true;
                        bool flag2 = true;
                        if (rowViewportCount >= 2)
                        {
                            if (viewportInfo.ActiveRowViewport == 1)
                            {
                                flag = false;
                            }
                            if (viewportInfo.TopRows[0] == -1)
                            {
                                verticalPosition = 0x12;
                            }
                            else
                            {
                                verticalPosition = ((int)viewportInfo.ViewportHeight[0]) + num7;
                            }
                            verticalPosition = (int)((UnitHelper.PixelToPoint((double)verticalPosition) * 20.0) * zoomFactor);
                            if (viewportInfo.TopRows.Length >= 2)
                            {
                                topVisibleRow = viewportInfo.TopRows[1];
                            }
                        }
                        else
                        {
                            verticalPosition = 0;
                        }
                        if (columnViewportCount >= 2)
                        {
                            if (viewportInfo.ActiveColumnViewport == 1)
                            {
                                flag2 = false;
                            }
                            if (viewportInfo.ViewportWidth[0] == -1.0)
                            {
                                horizontalPosition = 0x22;
                            }
                            else
                            {
                                horizontalPosition = (int)(viewportInfo.ViewportWidth[0] + num6);
                            }
                            horizontalPosition = (int)((UnitHelper.PixelToPoint((double)horizontalPosition) * 20.0) * zoomFactor);
                            if (viewportInfo.LeftColumns.Length >= 2)
                            {
                                leftmostVisibleColumn = viewportInfo.LeftColumns[1];
                            }
                        }
                        else
                        {
                            horizontalPosition = 0;
                        }
                        if (flag)
                        {
                            if (flag2)
                            {
                                paneIndex = 3;
                            }
                            else
                            {
                                paneIndex = 1;
                            }
                        }
                        else if (flag2)
                        {
                            paneIndex = 2;
                        }
                        else
                        {
                            paneIndex = 0;
                        }
                    }
                }
            }
        }

        public string GetPrintArea(int sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            PrintInfo printInfo = worksheet.PrintInfo;
            if (((printInfo.RowStart == -1) && (printInfo.RowEnd == -1)) && ((printInfo.ColumnStart == -1) && (printInfo.ColumnEnd == -1)))
            {
                return null;
            }
            string str = CellRangUtility.FormatCellRange(worksheet, new CellRange(printInfo.RowStart, printInfo.ColumnStart, (printInfo.RowEnd - printInfo.RowStart) + 1, (printInfo.ColumnEnd - printInfo.ColumnStart) + 1));
            string name = worksheet.Name;
            if (name.IndexOf(' ') != -1)
            {
                name = "'" + name + "'";
            }
            return (name + "!" + str);
        }

        public IExcelPrintOptions GetPrintOptions(short sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            PrintInfo printInfo = worksheet.PrintInfo;
            return new ExcelPrintOptions { HorizontalCentered = (printInfo.Centering == Centering.Horizontal) || (printInfo.Centering == Centering.Both), VerticalCentered = printInfo.Centering == Centering.Vertical, PrintGridLine = printInfo.ShowGridLine, PrintRowColumnsHeaders = (printInfo.ShowRowHeader == VisibilityType.Show) && (printInfo.ShowColumnHeader == VisibilityType.Show) };
        }

        public IExcelPrintPageMargin GetPrintPageMargin(short sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            PrintInfo printInfo = worksheet.PrintInfo;
            return new ExcelPrintPageMargin { Bottom = this.GetPageMarginInExcel((double)printInfo.Margin.Bottom), Top = this.GetPageMarginInExcel((double)printInfo.Margin.Top), Left = this.GetPageMarginInExcel((double)printInfo.Margin.Left), Right = this.GetPageMarginInExcel((double)printInfo.Margin.Right), Header = this.GetPageMarginInExcel((double)printInfo.Margin.Header), Footer = this.GetPageMarginInExcel((double)printInfo.Margin.Footer) };
        }

        public IExcelPrintPageSetting GetPrintPageSetting(short sheet)
        {
            Worksheet worksheet = this._workbook[sheet];
            PrintInfo printInfo = worksheet.PrintInfo;
            ExcelPrintPageSetting setting = new ExcelPrintPageSetting
            {
                Copies = 1
            };
            if (printInfo.FirstPageNumber != 1)
            {
                setting.UseCustomStartingPage = true;
                setting.FirstPageNumber = (short)printInfo.FirstPageNumber;
            }
            setting.Orientation = (printInfo.Orientation == PrintPageOrientation.Landscape) ? ExcelPrintOrientation.Landscape : ExcelPrintOrientation.Portrait;
            setting.PageOrder = (printInfo.PageOrder == PrintPageOrder.DownThenOver) ? ExcelPrintPageOrder.DownThenOver : ExcelPrintPageOrder.OverThenDown;
            setting.ZoomFactor = (float)printInfo.ZoomFactor;
            setting.ShowColor = !printInfo.BlackAndWhite;
            if ((printInfo.FitPagesTall != -1) || (printInfo.FitPagesWide != -1))
            {
                setting.UseSmartPrint = true;
                setting.SmartPrintPagesWidth = Math.Max(1, printInfo.FitPagesWide);
                setting.SmartPrintPagesHeight = Math.Max(1, printInfo.FitPagesTall);
            }
            string str = BuildExcelPrintHeader(printInfo.HeaderLeft, printInfo.HeaderCenter, printInfo.HeaderRight);
            setting.AdvancedHeadFooterSetting.HeaderEvenPage = str;
            setting.AdvancedHeadFooterSetting.HeaderOddPage = str;
            string str2 = BuildExcelPrintHeader(printInfo.FooterLeft, printInfo.FooterCenter, printInfo.FooterRight);
            setting.AdvancedHeadFooterSetting.FooterOddPage = str2;
            setting.AdvancedHeadFooterSetting.FooterEvenPage = str2;
            return setting;
        }

        public string GetPrintTitle(int sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            PrintInfo printInfo = worksheet.PrintInfo;
            if (((printInfo.RepeatRowStart == -1) && (printInfo.RepeatRowEnd == -1)) && ((printInfo.RepeatColumnStart == -1) && (printInfo.RepeatColumnEnd == -1)))
            {
                return null;
            }
            string str = CellRangUtility.FormatCellRange(worksheet, new CellRange(printInfo.RepeatRowStart, -1, (printInfo.RepeatRowEnd - printInfo.RepeatRowStart) + 1, -1));
            string str2 = CellRangUtility.FormatCellRange(worksheet, new CellRange(-1, printInfo.RepeatColumnStart, -1, (printInfo.RepeatColumnEnd - printInfo.RepeatColumnStart) + 1));
            string name = worksheet.Name;
            bool flag = !char.IsLetter(name[0]) && (name[0] != '_');
            for (int i = 1; !flag && (i < name.Length); i++)
            {
                flag = !char.IsLetterOrDigit(name[i]) && (name[i] != '_');
            }
            name.Replace("'", "''");
            if (flag)
            {
                name = "'" + name + "'";
            }
            StringBuilder builder = new StringBuilder();
            if ((printInfo.RepeatRowStart == -1) && (printInfo.RepeatRowEnd == -1))
            {
                builder.Append(name).Append("!").Append(str2);
            }
            else if ((printInfo.RepeatColumnStart == -1) && (printInfo.RepeatColumnEnd == -1))
            {
                builder.Append(name).Append("!").Append(str);
            }
            else
            {
                builder.Append(name).Append("!").Append(str2).Append(",").Append(name).Append(str);
            }
            return builder.ToString();
        }

        public void GetProtect(short sheet, ref bool isProtect)
        {
            if (sheet != -1)
            {
                isProtect = this._workbook[sheet].Protect;
            }
        }

        object GetRealCellValueForExcel(object value, out CellType cellType)
        {
            if (value == null)
            {
                cellType = CellType.Unknown;
                return null;
            }
            if (value is DateTimeOffset)
            {
                cellType = CellType.Datetime;
                value = ((DateTimeOffset)value).DateTime;
            }
            if (value is DateTime)
            {
                cellType = CellType.Datetime;
                return (double)ConverterHelper.ToExcelOADate((DateTime)value);
            }
            if (value is TimeSpan)
            {
                cellType = CellType.Datetime;
                DateTime time = new DateTime(0x76b, 12, 30);
                TimeSpan span = (TimeSpan)value;
                if (span.Ticks >= 0L)
                {
                    return (double)ConverterHelper.ToExcelOADate(time.Add(span));
                }
                return (double)0.0;
            }
            if ((value is double) && double.IsNaN((double)((double)value)))
            {
                cellType = CellType.Numeric;
                return (double)0.0;
            }
            if (value is CalcError)
            {
                cellType = CellType.Error;
                return value.ToString();
            }
            if (value is bool)
            {
                cellType = CellType.Boolean;
                return value;
            }
            if (this.IsNumber(value))
            {
                cellType = CellType.Numeric;
                return value;
            }
            if (value is string)
            {
                cellType = CellType.String;
                return value;
            }
            if (value is char)
            {
                cellType = CellType.String;
                value = value.ToString();
                return value;
            }
            cellType = CellType.String;
            value = value.ToString();
            return value;
        }

        IExcelRow GetRow(short sheet, int row, int rowRangeGroupMaxLevel)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            Dt.Cells.Data.ExcelRow row2 = new Dt.Cells.Data.ExcelRow(row)
            {
                Height = UnitHelper.PixelToPoint(worksheet.GetRowHeight(row))
            };
            if (rowRangeGroupMaxLevel >= 0)
            {
                int levelInternal = worksheet.RowRangeGroup.GetLevelInternal(row);
                if (worksheet.RowRangeGroup.Direction == RangeGroupDirection.Forward)
                {
                    int num2 = 0;
                    if (row > 0)
                    {
                        num2 = worksheet.RowRangeGroup.GetLevelInternal(row - 1);
                    }
                    if (((row > 0) && (levelInternal >= 0)) && (num2 > levelInternal))
                    {
                        row2.Collapsed = worksheet.RowRangeGroup.GetCollapsed(row);
                    }
                }
                else
                {
                    int num3 = 0;
                    if ((row + 1) < worksheet.RowCount)
                    {
                        num3 = worksheet.RowRangeGroup.GetLevelInternal(row + 1);
                    }
                    if (((row >= 0) && (levelInternal == 0)) && (num3 > 0))
                    {
                        row2.Collapsed = worksheet.RowRangeGroup.GetCollapsed(row);
                    }
                }
                int num4 = worksheet.RowRangeGroup.GetLevelInternal(row);
                if (!this._saveFlags.SaveDataOnly())
                {
                    row2.OutLineLevel = (num4 == -1) ? ((byte)0) : ((byte)num4);
                }
            }
            row2.Visible = worksheet.GetActualRowVisible(row, SheetArea.Cells);
            if (this._saveFlags.SaveAsFiltered() || this._saveFlags.SaveAsViewed())
            {
                if (((row < worksheet.Rows.Count) && row2.Visible) && (!this._saveFlags.SaveAsFiltered() && worksheet.Rows[row].IsFilteredOut))
                {
                    row2.Collapsed = true;
                    row2.Visible = false;
                }
            }
            else if ((!this._saveFlags.SaveAsFiltered() && worksheet.Rows[row].IsFilteredOut) && !row2.Visible)
            {
                row2.Visible = true;
            }
            int id = 0;
            if (!this._saveFlags.SaveDataOnly())
            {
                this._rowStyleTable[sheet].TryGetValue(row, out id);
                row2.SetFormatId(id);
                return row2;
            }
            row2.SetFormatId(-1);
            return row2;
        }

        void GetRowDataValidation(short sheet, List<IExcelDataValidation> result)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            int count = worksheet.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                Row row = worksheet.Rows[i];
                if (row != null)
                {
                    DataValidator dataValidator = row.DataValidator;
                    if (dataValidator != null)
                    {
                        IExcelDataValidation validation = dataValidator.ToExcelDataValidation();
                        validation.Ranges.Add(this.CreateCellRange(i, 0, 1, 0x4000));
                        result.Add(validation);
                    }
                }
            }
        }

        ExcelConditionalFormatValueObjectType GetScaleValueType(ScaleValueType scaleValueType)
        {
            switch (scaleValueType)
            {
                case ScaleValueType.Number:
                    return ExcelConditionalFormatValueObjectType.Num;

                case ScaleValueType.LowestValue:
                    return ExcelConditionalFormatValueObjectType.Min;

                case ScaleValueType.HighestValue:
                    return ExcelConditionalFormatValueObjectType.Max;

                case ScaleValueType.Percent:
                    return ExcelConditionalFormatValueObjectType.Percent;

                case ScaleValueType.Percentile:
                    return ExcelConditionalFormatValueObjectType.Percentile;

                case ScaleValueType.Automin:
                    return ExcelConditionalFormatValueObjectType.AutoMin;

                case ScaleValueType.Formula:
                    return ExcelConditionalFormatValueObjectType.Formula;

                case ScaleValueType.Automax:
                    return ExcelConditionalFormatValueObjectType.AutoMax;
            }
            return ExcelConditionalFormatValueObjectType.Num;
        }

        public void GetScroll(ref bool horizontalScroll, ref bool verticalScroll)
        {
            if ((this._workbook.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) || (this._workbook.VerticalScrollBarVisibility == ScrollBarVisibility.Visible))
            {
                verticalScroll = true;
            }
            else
            {
                verticalScroll = false;
            }
            if ((this._workbook.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) || (this._workbook.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible))
            {
                horizontalScroll = true;
            }
            else
            {
                horizontalScroll = false;
            }
        }

        public bool GetSelectionList(short sheet, List<GcRect> selectionList, ref GcPoint activeCell, ref PaneType paneType)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            int frozenRowCount = worksheet.FrozenRowCount;
            int frozenColumnCount = worksheet.FrozenColumnCount;
            if (this._saveFlags.SaveDataOnly())
            {
                frozenColumnCount = frozenRowCount = 0;
            }
            activeCell.X = (worksheet.ActiveColumnIndex >= 0) ? ((double)worksheet.ActiveColumnIndex) : ((double)0);
            activeCell.Y = (worksheet.ActiveRowIndex >= 0) ? ((double)worksheet.ActiveRowIndex) : ((double)0);
            ViewportInfo viewportInfo = worksheet.GetViewportInfo();
            if (viewportInfo != null)
            {
                int rowViewportCount = viewportInfo.RowViewportCount;
                int columnViewportCount = viewportInfo.ColumnViewportCount;
                if (((frozenColumnCount == 0) && (frozenRowCount == 0)) && (this._saveFlags.SaveCustomRowHeaders() || this._saveFlags.SaveCustomColumnHeaders()))
                {
                    rowViewportCount = 1;
                    columnViewportCount = 1;
                }
                if ((paneType != PaneType.TopLeft) && ((rowViewportCount > 2) || (columnViewportCount > 2)))
                {
                    return false;
                }
                if (paneType == PaneType.BottomRight)
                {
                    if (((rowViewportCount <= 1) || (columnViewportCount <= 1)) && ((frozenRowCount <= 0) || (frozenColumnCount <= 0)))
                    {
                        return false;
                    }
                }
                else if (paneType == PaneType.TopRight)
                {
                    if ((columnViewportCount <= 1) && (frozenColumnCount <= 0))
                    {
                        return false;
                    }
                }
                else if (paneType == PaneType.BottomLeft)
                {
                    if ((rowViewportCount <= 1) && (frozenRowCount <= 0))
                    {
                        return false;
                    }
                }
                else
                {
                    int num1 = (int)paneType;
                }
                if (selectionList != null)
                {
                    if ((worksheet.Selections != null) && (worksheet.Selections.Count > 0))
                    {
                        foreach (CellRange range in worksheet.Selections)
                        {
                            GcRect rect = new GcRect((range.Column > 0) ? ((double)range.Column) : ((double)0), (range.Row > 0) ? ((double)range.Row) : ((double)0), (range.ColumnCount > 0) ? ((double)range.ColumnCount) : ((double)0x4000), (range.RowCount > 0) ? ((double)range.RowCount) : ((double)0x100000));
                            selectionList.Add(rect);
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

        void GetSheetColorFilter(int sheetIndex)
        {
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            if (worksheet.RowFilter != null)
            {
                for (int i = 0; i < worksheet.RowFilter.Range.ColumnCount; i++)
                {
                    foreach (ConditionBase base2 in worksheet.RowFilter.GetFilterItems(worksheet.RowFilter.Range.Column + i))
                    {
                        if (base2 is ColorCondition)
                        {
                            try
                            {
                                ColorCondition condition = base2 as ColorCondition;
                                IExcelColorFilter filter = new ExcelColorFilter();
                                if (condition.CompareType == ColorCompareType.BackgroundColor)
                                {
                                    filter.CellColor = true;
                                }
                                else
                                {
                                    filter.CellColor = false;
                                }
                                filter.DxfId = (uint)this._differenceFormats.Count;
                                DifferentialFormatting formatting = new DifferentialFormatting
                                {
                                    Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(FillPatternType.Solid, ((Windows.UI.Color)condition.ExpectedValue).ToExcelColor(), new ExcelColor(GcColor.FromArgb(0xff, 0xff, 0xff, 0xff)))
                                };
                                this._differenceFormats.Add(formatting);
                                if (!this._colorFilters.ContainsKey(sheetIndex))
                                {
                                    this._colorFilters.Add(sheetIndex, new List<IExcelFilterColumn>());
                                }
                                ExcelFilterColumn column = new ExcelFilterColumn
                                {
                                    AutoFilterColumnId = (uint)i,
                                    ColorFilter = filter
                                };
                                this._colorFilters[sheetIndex].Add(column);
                            }
                            catch (Exception exception)
                            {
                                this.OnExcelSaveError(new ExcelWarning("Failed to process Filter", ExcelWarningCode.General, sheetIndex, -1, i, exception));
                            }
                        }
                    }
                }
            }
        }

        public int GetSheetCount()
        {
            return this._workbook.Sheets.Count;
        }

        public string GetSheetName(int sheet)
        {
            return this._workbook.Sheets[sheet].Name;
        }

        public IExcelColor GetSheetTabColor(int sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if (!string.IsNullOrWhiteSpace(worksheet.SheetTabThemeColor))
            {
                return worksheet.SheetTabThemeColor.GetExcelThemeColor();
            }
            if (worksheet.SheetTabColor != Colors.Transparent)
            {
                return worksheet.SheetTabColor.ToExcelColor();
            }
            return null;
        }

        void GetSheetTableColorFilter(int sheetIndex)
        {
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            foreach (SheetTable table in worksheet.GetTables())
            {
                CellRange range = table.Range;
                for (int i = 0; i < range.ColumnCount; i++)
                {
                    foreach (ConditionBase base2 in table.RowFilter.GetFilterItems(range.Column + i))
                    {
                        if (base2 is ColorCondition)
                        {
                            try
                            {
                                ColorCondition condition = base2 as ColorCondition;
                                IExcelColorFilter filter = new ExcelColorFilter();
                                if (condition.CompareType == ColorCompareType.BackgroundColor)
                                {
                                    filter.CellColor = true;
                                }
                                else
                                {
                                    filter.CellColor = false;
                                }
                                filter.DxfId = (uint)this._differenceFormats.Count;
                                DifferentialFormatting formatting = new DifferentialFormatting
                                {
                                    Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(FillPatternType.Solid, ((Windows.UI.Color)condition.ExpectedValue).ToExcelColor(), new ExcelColor(GcColor.FromArgb(0xff, 0xff, 0xff, 0xff)))
                                };
                                this._differenceFormats.Add(formatting);
                                if (!this._tableColorFilters.ContainsKey(table.Name))
                                {
                                    this._tableColorFilters.Add(table.Name, new List<IExcelFilterColumn>());
                                }
                                ExcelFilterColumn column = new ExcelFilterColumn
                                {
                                    AutoFilterColumnId = (uint)i,
                                    ColorFilter = filter
                                };
                                this._tableColorFilters[table.Name].Add(column);
                            }
                            catch (Exception exception)
                            {
                                this.OnExcelSaveError(new ExcelWarning("Failed to process Filter", ExcelWarningCode.General, sheetIndex, -1, i, exception));
                            }
                        }
                    }
                }
            }
        }

        public List<IExcelTable> GetSheetTables(int sheetIndex)
        {
            if (this._saveFlags.SaveDataOnly())
            {
                return null;
            }
            SheetTable[] tables = this._workbook.Sheets[sheetIndex].GetTables();
            if ((tables == null) || (tables.Length == 0))
            {
                return null;
            }
            List<IExcelTable> list = new List<IExcelTable>();
            foreach (SheetTable table in tables)
            {
                list.Add(this.GetExcelTable(sheetIndex, table));
            }
            return list;
        }

        public ExcelSheetType GetSheetType(int sheetIndex)
        {
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            return worksheet.ExcelSheetType;
        }

        List<SparklineGroup> GetSparklineGroup(int sheetIndex)
        {
            if (!this._sparklineGroups.ContainsKey(sheetIndex))
            {
                this._sparklineGroups.Add(sheetIndex, new List<SparklineGroup>());
            }
            else if (this._sparklineGroups[sheetIndex] == null)
            {
                this._sparklineGroups[sheetIndex] = new List<SparklineGroup>();
            }
            return this._sparklineGroups[sheetIndex];
        }

        int GetStyleIndex(StyleInfo style)
        {
            if (style == null)
            {
                return 0;
            }
            if (!string.IsNullOrWhiteSpace(style.Name))
            {
                StyleInfo info = style.Clone() as StyleInfo;
                info.Name = null;
                info.Parent = style.Name;
                int index = this._styles.IndexOf(info);
                if (index == -1)
                {
                    index = this._styles.Count;
                    this._styles.Add(info);
                }
                return index;
            }
            for (int i = this._styles.Count - 1; i >= 0; i--)
            {
                if (this._styles[i].Equals(style))
                {
                    return i;
                }
            }
            this._styles.Add(style);
            return (this._styles.Count - 1);
        }

        TableStyleElement GetTableStyleElement(ExcelTableElementType type, int dxfIndex, int size)
        {
            return new TableStyleElement { Type = type, DifferentFormattingIndex = dxfIndex, Size = size };
        }

        public List<IExcelTableStyle> GetTableStyles()
        {
            if (this._saveFlags.SaveDataOnly())
            {
                return null;
            }
            return this._tableStyles;
        }

        public bool GetTabs(ref bool showTabs, ref int selectedTabIndex, ref int firstDisplayedTabIndex, ref int selectedTabCount, ref int tabRatio)
        {
            showTabs = true;
            selectedTabIndex = this._workbook.ActiveSheetIndex;
            firstDisplayedTabIndex = this._workbook.StartSheetIndex;
            tabRatio = 600;
            selectedTabCount = 1;
            return true;
        }

        public IExcelTheme GetTheme()
        {
            if (this._saveFlags.SaveDataOnly())
            {
                return null;
            }
            if (this._workbook.CurrentTheme == null)
            {
                return null;
            }
            SpreadTheme currentTheme = this._workbook.CurrentTheme;
            if (currentTheme == null)
            {
                return null;
            }
            string currentThemeName = this._workbook.CurrentThemeName;
            if (string.IsNullOrWhiteSpace(currentThemeName))
            {
                return null;
            }
            if (currentThemeName.ToUpperInvariant() == "OFFICE")
            {
                currentThemeName = "Office Theme";
            }
            return new ExcelTheme(currentThemeName, this.GetThemeColorScheme(currentTheme), this.GetFontScheme(currentTheme));
        }

        IColorScheme GetThemeColorScheme(SpreadTheme spreadTheme)
        {
            return new ColorScheme(spreadTheme.Colors.Name, new List<IExcelColor> { spreadTheme.Colors.BackgroundColor1.ToExcelColor(), spreadTheme.Colors.TextColor1.ToExcelColor(), spreadTheme.Colors.BackgroundColor2.ToExcelColor(), spreadTheme.Colors.TextColor2.ToExcelColor(), spreadTheme.Colors.Accent1.ToExcelColor(), spreadTheme.Colors.Accent2.ToExcelColor(), spreadTheme.Colors.Accent3.ToExcelColor(), spreadTheme.Colors.Accent4.ToExcelColor(), spreadTheme.Colors.Accent5.ToExcelColor(), spreadTheme.Colors.Accent6.ToExcelColor(), spreadTheme.Colors.Hyperlink.ToExcelColor(), spreadTheme.Colors.FollowedHyperlink.ToExcelColor() });
        }

        IThemeFonts GetThemeFonts(string name)
        {
            return new ThemeFonts(new List<IRunFormatting> { new RunFormating(0, name) }, new List<IThemeFont>());
        }

        public bool GetTopLeft(short sheet, ref int topRow, ref int leftColumn)
        {
            if (this._saveFlags.SaveDataOnly())
            {
                leftColumn = topRow = 0;
                return true;
            }
            ViewportInfo viewportInfo = this._workbook.Sheets[sheet].GetViewportInfo();
            if (viewportInfo != null)
            {
                if ((viewportInfo.TopRows != null) && (viewportInfo.TopRows.Length > 0))
                {
                    topRow = viewportInfo.TopRows[0];
                }
                if ((viewportInfo.LeftColumns != null) && (viewportInfo.LeftColumns.Length > 0))
                {
                    leftColumn = viewportInfo.LeftColumns[0];
                }
            }
            return true;
        }

        int GetUniqueTableID(string name)
        {
            int num = -1;
            if (!this._uniqueTableID.TryGetValue(name, out num))
            {
                num = this._uniqueTableID.Count + 1;
                this._uniqueTableID.Add(name, num);
            }
            return num;
        }

        public List<IUnsupportRecord> GetUnsupportItems(int sheetIndex)
        {
            if (sheetIndex < 0)
            {
                return this._workbook.UnSupportExcelRecrods;
            }
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            return worksheet.UnSupportExcelRecrods;
        }

        public List<IExcelDataValidation> GetValidationData(short sheet)
        {
            if (!this._saveFlags.SaveDataOnly())
            {
                Worksheet local1 = this._workbook.Sheets[sheet];
                List<IExcelDataValidation> result = new List<IExcelDataValidation>();
                this.GetRowDataValidation(sheet, result);
                this.GetColumnDataValidation(sheet, result);
                this.GetCellDataValidation(sheet, result);
                if (result.Count != 0)
                {
                    return result;
                }
            }
            return null;
        }

        public IExcelRect GetWindow(ref bool hidden, ref bool iconic)
        {
            return null;
        }

        public IExcelWorkbookPropery GetWorkbookProperty()
        {
            return new ExcelWorkbookPropery { IsDate1904 = false, SaveExternalLinks = true };
        }

        public List<IExcelChart> GetWorksheetCharts(int sheet)
        {
            Worksheet worksheet = this._workbook.Sheets[sheet];
            List<IExcelChart> list = new List<IExcelChart>();
            using (IEnumerator<SpreadChart> enumerator = worksheet.Charts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    SpreadChart chart = enumerator.Current;
                    ExcelChart excelChart = new ExcelChart();
                    DataSeriesCounter.Index = 0;
                    InitAxisIDs(chart);
                    excelChart.BarChart = chart.ToExcelBarChart();
                    excelChart.LineChart = chart.ToExcelLineChart();
                    excelChart.StockChart = chart.ToExcelStokeChart();
                    excelChart.PieChart = chart.ToExcelPieChart();
                    excelChart.DoughnutChart = chart.ToExcelDoughnutChart();
                    excelChart.BubbleChart = chart.ToExcelBubbleChart();
                    excelChart.AreaChart = chart.ToExcelAreaChart();
                    excelChart.ScatterChart = chart.ToExcelScatterChart();
                    excelChart.RadarChart = chart.ToExcelRadarChart();
                    excelChart.DataTable = chart.DataTableSettings.ToExcelChartDataTable();
                    excelChart.Anchor = this.GetExcelAnchor(chart, worksheet.ExcelSheetType);
                    excelChart.Legend = chart.Legend.ToExcelLegend();
                    excelChart.ChartTitle = chart.ChartTitle.ToExcelSpreadChartTitle();
                    excelChart.Name = chart.Name;
                    excelChart.FloorWall = chart.FloorWall.ToExcelWall();
                    excelChart.SideWall = chart.SideWall.ToExcelWall();
                    excelChart.BackWall = chart.BackWall.ToExcelWall();
                    excelChart.PlotAreaFormat = chart.PlotArea.GetExcelChartForamt();
                    excelChart.PlotAreaLayout = chart.PlotAreaLayout.ToExcelLayout();
                    excelChart.ChartFormat = ExcelChartExtension.CreateExcelChartFormat(chart.FillThemeColor, chart.Fill, chart.StrokeThemeColor, chart.Stroke, chart.StrokeThickness, chart.IsAutomaticFill, chart.IsAutomaticStroke, chart.StrokeDashType, 1.0, (ExcelDrawingColorSettings)chart.FillDrawingColorSettings, (ExcelDrawingColorSettings)chart.StrokeDrawingColorSettings);
                    if ((((chart.FontFamily != null) || (chart.Foreground != null)) || (!string.IsNullOrWhiteSpace(chart.ForegroundThemeColor) || !(chart.FontSize + 1.0).IsZero())) || (ExcelChartExtension.FontWeightsIsBold(chart.FontWeight) || (chart.FontStyle != FontStyle.Normal)))
                    {
                        if (excelChart.TextFormat == null)
                        {
                            excelChart.TextFormat = new ExcelTextFormat();
                            excelChart.TextFormat.TextParagraphs.Add(new TextParagraph());
                        }
                        if (chart.FontFamily != null)
                        {
                            RichTextUtility.SetFontFamily(excelChart.TextFormat.TextParagraphs, chart.FontFamily.GetFontName());
                        }
                        RichTextUtility.SetRichTextFill(excelChart.TextFormat.TextParagraphs, chart.ActualForeground);
                        RichTextUtility.SetRichTextFontSize(excelChart.TextFormat.TextParagraphs, chart.ActualFontSize);
                        if (chart.FontStyle == FontStyle.Italic)
                        {
                            RichTextUtility.SetRichTextFontStyle(excelChart.TextFormat.TextParagraphs, true);
                        }
                        if (ExcelChartExtension.FontWeightsIsBold(chart.FontWeight))
                        {
                            RichTextUtility.SetRichtTextFontWeight(excelChart.TextFormat.TextParagraphs, true);
                        }
                        else
                        {
                            RichTextUtility.SetRichtTextFontWeight(excelChart.TextFormat.TextParagraphs, false);
                        }
                    }

                    switch (chart.DisplayEmptyCellsAs)
                    {
                        case EmptyValueStyle.Gaps:
                            excelChart.DisplayBlanksAs = DisplayBlankAs.Gap;
                            break;

                        case EmptyValueStyle.Zero:
                            excelChart.DisplayBlanksAs = DisplayBlankAs.Zero;
                            break;

                        case EmptyValueStyle.Connect:
                            excelChart.DisplayBlanksAs = DisplayBlankAs.Span;
                            break;
                    }
                    excelChart.PlotVisibleOnly = !chart.DisplayHidden;
                    excelChart.ShowAutoTitle = chart.ShowAutoTitle;
                    excelChart.ShowDataLabelsOverMaximun = chart.ShowDataLabelOverMax;
                    excelChart.AlternateContentChoiceStyleList = chart.AlternateContentChoiceList;
                    excelChart.AlternateFallbackStyleList = chart.AlternateFallbackStyleList;
                    excelChart.IsDate1904 = chart.IsDate1904;
                    if (chart.CornerRadius > 0.0)
                    {
                        excelChart.RoundedCorners = true;
                    }
                    else
                    {
                        excelChart.RoundedCorners = false;
                    }
                    if ((chart.DefaultStyleIndex >= 1) && (chart.DefaultStyleIndex <= 0x30))
                    {
                        excelChart.DefaultStyleIndex = chart.DefaultStyleIndex;
                    }
                    excelChart.SecondaryChart = chart.SecondaryChart;
                    if (excelChart.SecondaryChart != null)
                    {
                        this.UpdateChartSeriesIndex(excelChart.SecondaryChart);
                    }
                    excelChart.Hidden = !chart.Visible;
                    excelChart.Locked = chart.Locked;
                    list.Add(excelChart);
                }
            }
            foreach (IExcelChart chart in worksheet.UnSupportedCharts)
            {
                list.Add(chart);
            }
            return list;
        }

        public List<IExcelImage> GetWorkSheetImages(int sheetIndex)
        {
            RenderTargetBitmap bmp;
            List<IExcelImage> list = new List<IExcelImage>();
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            using (IEnumerator<Picture> enumerator = worksheet.Pictures.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Picture picture = enumerator.Current;
                    if (picture != null)
                    {
                        if (!string.IsNullOrEmpty(picture.ImageByteArrayBase64String))
                        {
                            try
                            {
                                byte[] sourceArray = Convert.FromBase64String(picture.ImageByteArrayBase64String);
                                ExcelImage image = new ExcelImage(picture.Name, ImageType.PNG, sourceArray)
                                {
                                    Anchor = this.GetExcelAnchor(picture, worksheet.ExcelSheetType),
                                    Hidden = !picture.Visible
                                };
                                list.Add(image);
                            }
                            catch
                            { }
                        }
                        else if (picture.UriSource != null)
                        {
                            try
                            {
                                BitmapImage imageSource = new BitmapImage();
                                imageSource.UriSource = picture.UriSource;
                                Stream stream = Utility.GetImageStream(imageSource, ImageFormat.Png, PictureSerializationMode.Normal);
                                byte[] buffer = new byte[stream.Length];
                                stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                                stream.Read(buffer, 0, (int)stream.Length);
                                ExcelImage image2 = new ExcelImage(picture.Name, ImageType.PNG, buffer)
                                {
                                    Anchor = this.GetExcelAnchor(picture, worksheet.ExcelSheetType),
                                    Hidden = !picture.Visible
                                };
                                list.Add(image2);
                            }
                            catch
                            { }
                        }
                        else if ((bmp = picture.ImageSource as RenderTargetBitmap) != null)
                        {
                            // hdt 新增，提供报表中图表的导出
                            try
                            {
                                Stream stream = Utility.GetBmpStream(bmp);
                                byte[] buffer = new byte[stream.Length];
                                stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                                stream.Read(buffer, 0, (int)stream.Length);
                                ExcelImage image = new ExcelImage(picture.Name, ImageType.PNG, buffer)
                                {
                                    Anchor = this.GetExcelAnchor(picture, worksheet.ExcelSheetType),
                                    Hidden = !picture.Visible
                                };
                                list.Add(image);
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            list.AddRange(Enumerable.Cast<IExcelImage>((IEnumerable)worksheet.UnsupportImages));
            return list;
        }

        public void GetZoom(short sheet, ref float zoom)
        {
            zoom = this._workbook.Sheets[sheet].ZoomFactor;
        }

        static void InitAxisIDs(SpreadChart chart)
        {
            ExcelChartExtension._axisCollection.Clear();
            if ((chart.AxisX != null) && (chart.AxisX.Id > 0))
            {
                ExcelChartExtension._axisCollection.Add(chart.AxisX.Id);
            }
            if ((chart.AxisY != null) && (chart.AxisY.Id > 0))
            {
                ExcelChartExtension._axisCollection.Add(chart.AxisY.Id);
            }
            if ((chart.AxisZ != null) && (chart.AxisZ.Id > 0))
            {
                ExcelChartExtension._axisCollection.Add(chart.AxisZ.Id);
            }
        }

        void InitCacheCollections(int sheetIndex)
        {
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            this.InitWorkbookCellsCollection(sheetIndex);
            this.InitValidatorCollection(sheetIndex, worksheet.RowCount, worksheet.ColumnCount);
            this.InitStyleTables(sheetIndex);
            if (!this._autoRowHeight.ContainsKey(sheetIndex) || (this._autoRowHeight[sheetIndex] == null))
            {
                this._autoRowHeight[sheetIndex] = new Dictionary<int, double>();
            }
        }

        void InitDefaultDateFormatter()
        {
            ShortDateFormatter = new GeneralFormatter(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            ShortTimeFormatter = new GeneralFormatter("h:mm:ss");
            StyleInfo info = new StyleInfo
            {
                Formatter = ShortDateFormatter
            };
            DefaultDateTimeStyle = info;
            StyleInfo info2 = new StyleInfo
            {
                Formatter = ShortTimeFormatter
            };
            DefaultTimeSpanStyle = info2;
        }

        static void InitExcelBuiltInStyles()
        {
            _builtInStyle = new Dictionary<string, int>();
            _builtInStyle.Add("20% - Accent1", 30);
            _builtInStyle.Add("20% - Accent2", 0x22);
            _builtInStyle.Add("20% - Accent3", 0x26);
            _builtInStyle.Add("20% - Accent4", 0x2a);
            _builtInStyle.Add("20% - Accent5", 0x2e);
            _builtInStyle.Add("20% - Accent6", 50);
            _builtInStyle.Add("40% - Accent1", 0x1f);
            _builtInStyle.Add("40% - Accent2", 0x23);
            _builtInStyle.Add("40% - Accent3", 0x27);
            _builtInStyle.Add("40% - Accent4", 0x2b);
            _builtInStyle.Add("40% - Accent5", 0x2f);
            _builtInStyle.Add("40% - Accent6", 0x33);
            _builtInStyle.Add("60% - Accent1", 0x20);
            _builtInStyle.Add("60% - Accent2", 0x24);
            _builtInStyle.Add("60% - Accent3", 40);
            _builtInStyle.Add("60% - Accent4", 0x2c);
            _builtInStyle.Add("60% - Accent5", 0x30);
            _builtInStyle.Add("60% - Accent6", 0x34);
            _builtInStyle.Add("Accent1", 0x1d);
            _builtInStyle.Add("Accent2", 0x21);
            _builtInStyle.Add("Accent3", 0x25);
            _builtInStyle.Add("Accent4", 0x29);
            _builtInStyle.Add("Accent5", 0x2d);
            _builtInStyle.Add("Accent6", 0x31);
            _builtInStyle.Add("Bad", 0x1b);
            _builtInStyle.Add("Calculation", 0x16);
            _builtInStyle.Add("Check Cell", 0x17);
            _builtInStyle.Add("Comma", 3);
            _builtInStyle.Add("Comma [0]", 6);
            _builtInStyle.Add("Currency", 4);
            _builtInStyle.Add("Currency [0]", 7);
            _builtInStyle.Add("Explanatory Text", 0x35);
            _builtInStyle.Add("Good", 0x1a);
            _builtInStyle.Add("Heading 1", 0x10);
            _builtInStyle.Add("Heading 2", 0x11);
            _builtInStyle.Add("Heading 3", 0x12);
            _builtInStyle.Add("Heading 4", 0x13);
            _builtInStyle.Add("Input", 20);
            _builtInStyle.Add("Linked Cell", 0x18);
            _builtInStyle.Add("Neutral", 0x1c);
            _builtInStyle.Add("Normal", 0);
            _builtInStyle.Add("Note", 10);
            _builtInStyle.Add("Output", 0x15);
            _builtInStyle.Add("Percent", 5);
            _builtInStyle.Add("Title", 15);
            _builtInStyle.Add("Total", 0x19);
            _builtInStyle.Add("Warning Text", 11);
            _builtInStyle.Add("Hyperlink", 8);
            _builtInStyle.Add("FollowedHyperLink", 9);
            _builtInStyle.Add("RowLevel_1", 1);
            _builtInStyle.Add("RowLevel_2", 1);
            _builtInStyle.Add("RowLevel_3", 1);
            _builtInStyle.Add("RowLevel_4", 1);
            _builtInStyle.Add("RowLevel_5", 1);
            _builtInStyle.Add("RowLevel_6", 1);
            _builtInStyle.Add("RowLevel_7", 1);
            _builtInStyle.Add("ColLevel_1", 2);
            _builtInStyle.Add("ColLevel_2", 2);
            _builtInStyle.Add("ColLevel_3", 2);
            _builtInStyle.Add("ColLevel_4", 2);
            _builtInStyle.Add("ColLevel_5", 2);
            _builtInStyle.Add("ColLevel_6", 2);
            _builtInStyle.Add("ColLevel_7", 2);
        }

        void InitStyle()
        {
            StyleInfo normalStyle = this._workbook.DefaultStyle.Clone() as StyleInfo;
            if (string.IsNullOrWhiteSpace(normalStyle.FontTheme) && (normalStyle.FontFamily == null))
            {
                normalStyle.FontFamily = this._workbook.CurrentTheme.BodyFontFamily;
            }
            if ((((normalStyle.Background is SolidColorBrush) && ((normalStyle.Background as SolidColorBrush).Color == Windows.UI.Color.FromArgb(0, 0xff, 0xff, 0xff))) && (((normalStyle.FontFamily != null) ? normalStyle.FontFamily.Source : NameConstans.DEFAULT_FONT_FAMILY) == ((this._workbook.CurrentTheme.BodyFontFamily != null) ? this._workbook.CurrentTheme.BodyFontFamily.Source : NameConstans.DEFAULT_FONT_FAMILY))) && (normalStyle.FontSize == DefaultStyleCollection.DefaultFontSize))
            {
                normalStyle.Background = null;
            }
            normalStyle.Name = "Normal";
            this._namedStyleTable.Add("Normal", 0);
            this._styles.Add(normalStyle);
            this._namedStyles.Add(normalStyle);
        }

        void InitStyleTables(int i)
        {
            if (!this._cellStyleTable.ContainsKey(i) || (this._cellStyleTable[i] == null))
            {
                this._cellStyleTable[i] = new Dictionary<long, int>();
            }
            if (!this._rowStyleTable.ContainsKey(i) || (this._rowStyleTable[i] == null))
            {
                this._rowStyleTable[i] = new Dictionary<int, int>();
            }
            if (!this._columnStyleTable.ContainsKey(i) || (this._columnStyleTable[i] == null))
            {
                this._columnStyleTable[i] = new Dictionary<int, int>();
            }
        }

        void InitValidatorCollection(int sheetIndex, int rowCount, int columnCount)
        {
            Worksheet local1 = this._workbook.Sheets[sheetIndex];
            if (!this._allValidators.ContainsKey(sheetIndex))
            {
                this._allValidators.Add(sheetIndex, new DataValidator[rowCount, columnCount]);
            }
            else if (this._allValidators[sheetIndex] != null)
            {
                this._allValidators[sheetIndex] = new DataValidator[rowCount, columnCount];
            }
        }

        void InitWorkbookCellsCollection(int sheetIndex)
        {
            if (!this._workbookCells.ContainsKey(sheetIndex))
            {
                this._workbookCells.Add(sheetIndex, new Dictionary<int, List<IExcelCell>>());
            }
            else if (this._workbookCells[sheetIndex] == null)
            {
                this._workbookCells[sheetIndex] = new Dictionary<int, List<IExcelCell>>();
            }
        }

        int InsertStyleToUniqueCollection(List<StyleInfo> styles, StyleInfo style)
        {
            int index = styles.IndexOf(style);
            if (index == -1)
            {
                index = styles.Count;
                styles.Add(style);
            }
            return index;
        }

        public bool IsCalculationError(object value, ref short errorVal)
        {
            if (value != null)
            {
                errorVal = 0xff;
                switch (value.ToString())
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
                        errorVal = 0xff;
                        break;
                }
                if (errorVal != 0xff)
                {
                    return true;
                }
            }
            return false;
        }

        bool IsEmptyStyleInfo(StyleInfo styleInfo)
        {
            if ((((((styleInfo.IsNameSet() || styleInfo.IsLockedSet()) || (styleInfo.IsTabStopSet() || styleInfo.IsFontThemeSet())) || ((styleInfo.IsFontFamilySet() || styleInfo.IsFontSizeSet()) || (styleInfo.IsFontStretchSet() || styleInfo.IsFontStyleSet()))) || (((styleInfo.IsFontWeightSet() || styleInfo.IsHorizontalAlignmentSet()) || (styleInfo.IsVerticalAlignmentSet() || styleInfo.IsTextIndentSet())) || ((styleInfo.IsWordWrapSet() || styleInfo.IsShrinkToFitSet()) || (styleInfo.IsBorderLeftSet() || styleInfo.IsBorderTopSet())))) || (((styleInfo.IsBorderBottomSet() || styleInfo.IsBorderRightSet()) || (styleInfo.IsBackgroundSet() || styleInfo.IsForegroundSet())) || styleInfo.IsBackgroundThemeColorSet())) || styleInfo.IsForegroundThemeColorSet())
            {
                return false;
            }
            if (styleInfo.IsFormatterSet() && ((styleInfo.IsFormatterSet() && (styleInfo.Formatter is GeneralFormatter)) && (styleInfo.Formatter.FormatString.ToUpperInvariant() != "GENERAL")))
            {
                return false;
            }
            return true;
        }

        bool IsNumber(object value)
        {
            if (value is string)
            {
                return false;
            }
            return ((((((value is double) || (value is float)) || ((value is int) || (value is long))) || (((value is short) || (value is decimal)) || ((value is sbyte) || (value is ulong)))) || ((value is uint) || (value is ushort))) || (value is byte));
        }

        public bool IsSheetHidden(int sheet)
        {
            return !this._workbook.Sheets[sheet].Visible;
        }

        double MeasureStringHeight(string text, string fontName, double fontSize, bool wordWrap, bool bold, bool italic, double width)
        {
            return this.MeasureStringHeightCore(text, fontName, fontSize, wordWrap, bold, italic, width);
        }

        public double MeasureStringHeightCore(string text, string fontName, double fontSize, bool wordWrap, bool bold, bool italic, double width)
        {
            TextBlock block = new TextBlock();
            block.FontFamily = new FontFamily(fontName);
            block.FontSize = fontSize;
            if (bold)
            {
                block.FontWeight = FontWeights.Bold;
            }
            if (italic)
            {
                block.FontStyle = FontStyle.Italic;
            }
            block.Margin = new Windows.UI.Xaml.Thickness(4.0);
            block.TextWrapping = wordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            block.Text = text;
            block.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            block.Measure(new Windows.Foundation.Size((width <= 0.0) ? double.MaxValue : width, double.MaxValue));
            block.Width = (width < 0.0) ? double.MaxValue : width;
            return Math.Floor((double)(block.ActualHeight + 8.0));
        }

        public void OnExcelSaveError(ExcelWarning excelWarning)
        {
            if (excelWarning != null)
            {
                this._workbook.OnExcelError(excelWarning);
            }
        }

        void PostProcessWorkbook()
        {
            this._workbook.SuspendCalcService();
            for (int i = 0; i < this._workbook.SheetCount; i++)
            {
                Worksheet worksheet = this._workbook.Sheets[i];
                int count = this._sheetsRowOffsets[i];
                if (count > 0)
                {
                    worksheet.RemoveRows(0, count);
                }
                int num3 = this._sheetscolumnOffsets[i];
                if (num3 > 0)
                {
                    worksheet.RemoveColumns(0, num3);
                }
            }
            this.Clear();
            this._workbook.ResumeCalcService();
            this._workbook.ResumeEvent();
        }

        void PreProcessCells(int sheetIndex)
        {
            Dictionary<long, int> cellStyleTable;
            Dictionary<string, StyleInfo> allNamedStyles;
            List<StyleInfo> columnStyles;
            List<string> columnNamedStyles;
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            if (worksheet.Rows.Count != 0)
            {
                int num = Math.Max(worksheet.GetLastDirtyColumn(StorageType.Data), Math.Max(worksheet.GetLastDirtyColumn(StorageType.Sparkline), worksheet.GetLastDirtyColumn(StorageType.Style))) + 1;
                cellStyleTable = this._cellStyleTable[sheetIndex];
                Dictionary<int, int> rowStyleTable = this._rowStyleTable[sheetIndex];
                DataValidator[,] sheetValidators = this._allValidators[sheetIndex];
                allNamedStyles = new Dictionary<string, StyleInfo>();
                foreach (StyleInfo info in worksheet.NamedStyles)
                {
                    allNamedStyles[info.Name] = info;
                }
                columnStyles = this.GetColumnStyleInfos(worksheet);
                columnNamedStyles = this.GetColumnNamedStyleInfos(worksheet);
                List<double> columnWidthInfos = this.GetColumnWidthInfos(worksheet);
                Dictionary<int, double> dictionary2 = this._autoRowHeight[sheetIndex];
                int currentSheetMaxUsedRowIndex = this.GetCurrentSheetMaxUsedRowIndex(sheetIndex);
                double num3 = this.CalculateDefaultRowHeight(sheetIndex);
                for (int row = 0; row <= currentSheetMaxUsedRowIndex; row++)
                {
                    string currentRowStyleName = null;
                    StyleInfo currentRowStyle = null;
                    double maxRowHeight = num3;
                    object obj2 = this.ProcessRowStyle(worksheet, rowStyleTable, row);
                    if ((obj2 != null) && (obj2 is string))
                    {
                        currentRowStyleName = (string)((string)obj2);
                    }
                    else if ((obj2 != null) && (obj2 is StyleInfo))
                    {
                        currentRowStyle = obj2 as StyleInfo;
                    }
                    List<SparklineGroup> sparklineGroup = this.GetSparklineGroup(sheetIndex);
                    for (int column = 0; column < num; column++)
                    {
                        int styleKey = (row * num) + column;
                        StyleInfo styleInfo = null;
                        string styleName = null;
                        object value = worksheet.GetValue(row, column);
                        Tuple<string, CellRange> formulaStruct = this.GetCellFormulaStruct(sheetIndex, row, column);
                        Sparkline sparkline = worksheet.GetSparkline(row, column);
                        this.AddCellSparklineToCache(sparklineGroup, sparkline);
                        SheetTable table = worksheet.FindTable(row, column);
                        bool isInTable = table != null;
                        this.GetCellStyle(worksheet, row, column, allNamedStyles, cellStyleTable, columnNamedStyles[column], currentRowStyleName, currentRowStyle, columnStyles[column], styleKey, value, ref styleName, ref styleInfo, isInTable);
                        if ((isInTable && ((row == table.HeaderIndex) || (row == table.FooterIndex))) && (value != null))
                        {
                            value = value.ToString();
                        }
                        CellType unknown = CellType.Unknown;
                        object realCellValueForExcel = this.GetRealCellValueForExcel(value, out unknown);
                        if (((styleInfo != null) || (formulaStruct != null)) || ((realCellValueForExcel != null) || !string.IsNullOrEmpty(styleName)))
                        {
                            Dt.Cells.Data.ExcelCell cell = new Dt.Cells.Data.ExcelCell(row, column)
                            {
                                CellType = unknown,
                                Value = realCellValueForExcel
                            };
                            this.SetCellStyle(cellStyleTable, styleKey, styleInfo, styleName, cell);
                            this.SetCellFormula(formulaStruct, cell);
                            this.AddCellToCache(sheetIndex, row, cell);
                            this.AddCellValidatorToCache(row, column, styleInfo, sheetValidators);
                            maxRowHeight = this.UpdateMaxRowHeightIfNeeded(this._workbook, columnWidthInfos, maxRowHeight, column, styleInfo, cell);
                        }
                    }
                    dictionary2.Add(row, maxRowHeight);
                }
            }
        }

        void PreProcessColumnStyles(int sheetIndex)
        {
            Worksheet worksheet = this._workbook.Sheets[sheetIndex];
            Dictionary<int, int> dictionary = this._columnStyleTable[sheetIndex];
            int columnCount = worksheet.ColumnCount;
            for (int i = 0; i < columnCount; i++)
            {
                if (!this._saveFlags.SaveDataOnly())
                {
                    string str = null;
                    object obj2 = worksheet.GetStyleObject(-1, i, SheetArea.Cells);
                    if (obj2 is string)
                    {
                        str = (string)(obj2 as string);
                    }
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (this._namedStyleTable.ContainsKey(str))
                        {
                            dictionary.Add(i, this._namedStyleTable[str]);
                        }
                        else
                        {
                            dictionary.Add(i, -1);
                        }
                    }
                    else if (obj2 is StyleInfo)
                    {
                        StyleInfo styleInfo = obj2 as StyleInfo;
                        if (styleInfo != null)
                        {
                            if (this.IsEmptyStyleInfo(styleInfo) && styleInfo.IsParentSet())
                            {
                                StyleInfo info2 = this._workbook.NamedStyles.Find(styleInfo.Parent);
                                if (info2 != null)
                                {
                                    string parent = styleInfo.Parent;
                                    styleInfo = info2.Clone() as StyleInfo;
                                    styleInfo.ResetName();
                                    styleInfo.Parent = parent;
                                }
                            }
                            int styleIndex = this.GetStyleIndex(styleInfo);
                            dictionary.Add(i, styleIndex);
                        }
                        else
                        {
                            dictionary.Add(i, 0);
                        }
                    }
                }
            }
        }

        void PreProcessDifferentConditonalFormats()
        {
            List<StyleInfo> styles = new List<StyleInfo>();
            this.GetDifferentConditionalFormatFromTable(styles);
            this.GetDifferentConditionalFormatFormConditionalFormat(styles);
            List<IDifferentialFormatting> result = new List<IDifferentialFormatting>();
            foreach (StyleInfo info in styles)
            {
                result.Add(info.ToDifferentialFormatting(this._workbook));
            }
            if (this._differenceFormats == null)
            {
                this._differenceFormats = new List<IDifferentialFormatting>();
            }
            this._differenceFormats = (result.Count > 0) ? result : new List<IDifferentialFormatting>();
            this.AddColorFilterSettingToDifferentConditionalFormats();
        }

        void PreProcessNamedStyle()
        {
            HashSet<string> processedNamesStyle = new HashSet<string> { "Normal" };
            List<StyleInfo> fakeNamedStyles = new List<StyleInfo>();
            this.PreProcessNamedStyles(this._workbook.NamedStyles, processedNamesStyle, fakeNamedStyles);
            for (int i = 0; i < this._workbook.SheetCount; i++)
            {
                this.PreProcessNamedStyles(this._workbook.Sheets[i].NamedStyles, processedNamesStyle, fakeNamedStyles);
            }
            BuiltInExcelStyles builtInExcelStyleCollection = new BuiltInExcelStyles();
            List<IExcelStyle> builtInStyles = new List<IExcelStyle>();
            Task.Factory.StartNew(delegate
            {
                builtInStyles = builtInExcelStyleCollection.GetBuiltInStyls();
            }).Wait();
            StyleInfoCollection builtInStyleCollection = new StyleInfoCollection();
            foreach (IExcelStyle style in builtInStyles)
            {
                StyleInfo info = style.Format.ToCellStyleInfo(this._workbook);
                info.Name = style.Name;
                builtInStyleCollection.Add(info);
            }
            this.PreProcessNamedStyles(builtInStyleCollection, processedNamesStyle, fakeNamedStyles);
            int num2 = this._styles.Count;
            for (int j = 0; j < num2; j++)
            {
                StyleInfo info = this._styles[j].Clone() as StyleInfo;
                info.Parent = this._styles[j].Name;
                info.Name = null;
                this._namedStyleTable[this._styles[j].Name] = this._styles.Count;
                this._styles.Add(info);
            }
            for (int k = 0; k < fakeNamedStyles.Count; k++)
            {
                this._namedStyleTable[fakeNamedStyles[k].Name] = this._styles.Count;
                this._styles.Add(fakeNamedStyles[k]);
            }
        }

        void PreProcessNamedStyles(StyleInfoCollection namedStyles, HashSet<string> processedNamesStyle, List<StyleInfo> fakeNamedStyles)
        {
            if (!this._saveFlags.SaveDataOnly() && (namedStyles != null))
            {
                foreach (StyleInfo info in namedStyles)
                {
                    if (!processedNamesStyle.Contains(info.Name))
                    {
                        processedNamesStyle.Add(info.Name);
                        if (!info.Name.StartsWith("__builtInStyle"))
                        {
                            this._styles.Add(info);
                            this._namedStyles.Add(info);
                        }
                        else
                        {
                            fakeNamedStyles.Add(info);
                        }
                    }
                }
            }
        }

        void PreProcessWorkbook()
        {
            this._workbook.SuspendCalcService();
            this._workbook.SuspendEvent();
            this.Clear();
            this.ShiftWorkbook();
            this.InitStyle();
            this.PreProcessNamedStyle();
            this.PreProcessDifferentConditonalFormats();
            for (int sheetIndex = 0; sheetIndex < this._workbook.Sheets.Count; sheetIndex++)
            {
                if (this._workbook.Sheets[sheetIndex] != null)
                {
                    this.InitCacheCollections(sheetIndex);
                    this.ShiftRowHeader(sheetIndex);
                    this.ShiftColumnHeader(sheetIndex);
                    this.PreProcessCells(sheetIndex);
                    this.PreProcessColumnStyles(sheetIndex);
                    if (this._formulas.ContainsKey(sheetIndex))
                    {
                        this._formulas.Remove(sheetIndex);
                    }
                }
            }
            this._formulas.Clear();
            try
            {
                if (!this.AllEqual(this._sheetsRowOffsets, 0) || !this.AllEqual(this._sheetscolumnOffsets, 0))
                {
                    this._workbook.ResumeCalcService();
                }
            }
            catch
            { }
        }

        void ProcessConditionalFormatAverageRule(List<StyleInfo> styles, int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            AverageRule rule = item as AverageRule;
            if (rule != null)
            {
                short num3;
                ExcelConditionalFormat format = new ExcelConditionalFormat();
                identfier = (short)((num3 = identfier) + 1);
                format.Identifier = num3;
                format.Ranges = this.ToRanges(rule.Ranges);
                if (format.Ranges != null)
                {
                    int num = 0;
                    ExcelConditionalFormatType aboveAverage = ExcelConditionalFormatType.AboveAverage;
                    switch (rule.Operator)
                    {
                        case AverageConditionType.Above:
                            aboveAverage = ExcelConditionalFormatType.AboveAverage;
                            break;

                        case AverageConditionType.Below:
                            aboveAverage = ExcelConditionalFormatType.BelowAverage;
                            break;

                        case AverageConditionType.EqualOrAbove:
                            aboveAverage = ExcelConditionalFormatType.AboveOrEqualToAverage;
                            break;

                        case AverageConditionType.EqualOrBelow:
                            aboveAverage = ExcelConditionalFormatType.BelowOrEqualToAverage;
                            break;

                        case AverageConditionType.Above1StdDev:
                            aboveAverage = ExcelConditionalFormatType.AboveAverage;
                            num = 1;
                            break;

                        case AverageConditionType.Below1StdDev:
                            aboveAverage = ExcelConditionalFormatType.BelowAverage;
                            num = 1;
                            break;

                        case AverageConditionType.Above2StdDev:
                            aboveAverage = ExcelConditionalFormatType.AboveAverage;
                            num = 2;
                            break;

                        case AverageConditionType.Below2StdDev:
                            aboveAverage = ExcelConditionalFormatType.BelowAverage;
                            num = 2;
                            break;

                        case AverageConditionType.Above3StdDev:
                            aboveAverage = ExcelConditionalFormatType.AboveAverage;
                            num = 3;
                            break;

                        case AverageConditionType.Below3StdDev:
                            aboveAverage = ExcelConditionalFormatType.BelowAverage;
                            num = 3;
                            break;
                    }
                    ExcelGeneralRule rule2 = new ExcelGeneralRule(aboveAverage)
                    {
                        Priority = item.Priority,
                        StopIfTrue = item.StopIfTrue
                    };
                    if (num != 0)
                    {
                        rule2.StdDev = new int?(num);
                    }
                    if (rule.Style == null)
                    {
                        rule2.DifferentialFormattingId = -1;
                    }
                    else
                    {
                        int index = styles.IndexOf(rule.Style);
                        if (index == -1)
                        {
                            index = styles.Count;
                            styles.Add(rule.Style);
                        }
                        rule2.DifferentialFormattingId = index;
                    }
                    if (rule2.Formulas.Count == 0)
                    {
                        string str = this.CreateConditionalFormatFormula(rule2, format.Ranges[0]);
                        if (str != null)
                        {
                            rule2.Formulas.Add(str);
                        }
                    }
                    format.ConditionalFormattingRules.Add(rule2);
                    this._conditinalFormats[sheetIndex].Add(format);
                }
            }
        }

        void ProcessConditionalFormatCellValueRule(List<StyleInfo> styles, int sheet, ref short identfier, FormattingRuleBase item)
        {
            CellValueRule rule = item as CellValueRule;
            if (rule != null)
            {
                short num2;
                ExcelConditionalFormat format = new ExcelConditionalFormat();
                identfier = (short)((num2 = identfier) + 1);
                format.Identifier = num2;
                format.Ranges = this.ToRanges(rule.Ranges);
                if (format.Ranges != null)
                {
                    ExcelHighlightingRule rule2 = new ExcelHighlightingRule
                    {
                        Priority = item.Priority,
                        StopIfTrue = item.StopIfTrue,
                        ComparisonOperator = (ExcelConditionalFormattingOperator)rule.Operator
                    };
                    if (rule.Value1 != null)
                    {
                        string str = Convert.ToString(rule.Value1, (IFormatProvider)CultureInfo.InvariantCulture).RemoveFormulaSymbolIfExists();
                        rule2.Formulas.Add(str);
                    }
                    if (rule.Value2 != null)
                    {
                        string str2 = Convert.ToString(rule.Value2, (IFormatProvider)CultureInfo.InvariantCulture).RemoveFormulaSymbolIfExists();
                        rule2.Formulas.Add(str2);
                    }
                    if (rule.Style == null)
                    {
                        rule2.DifferentialFormattingId = -1;
                    }
                    else
                    {
                        int index = styles.IndexOf(rule.Style);
                        if (index == -1)
                        {
                            index = styles.Count;
                            styles.Add(rule.Style);
                        }
                        rule2.DifferentialFormattingId = index;
                    }
                    format.ConditionalFormattingRules.Add(rule2);
                    this._conditinalFormats[sheet].Add(format);
                }
            }
        }

        void ProcessConditionalFormatColorScaleRule(int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            int index = 0;
            ScaleRule rule = item as ScaleRule;
            if ((rule.Scales.Length == 2) || (rule.Scales.Length == 3))
            {
                bool flag = false;
                foreach (object obj2 in rule.Expected)
                {
                    if ((obj2 != null) && !(obj2 is Windows.UI.Color))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    ExcelConditionalFormat format = new ExcelConditionalFormat
                    {
                        Ranges = this.ToRanges(rule.Ranges)
                    };
                    if (format.Ranges != null)
                    {
                        short num3;
                        identfier = (short)((num3 = identfier) + 1);
                        format.Identifier = num3;
                        ExcelColorScaleRule rule2 = new ExcelColorScaleRule
                        {
                            Priority = item.Priority,
                            StopIfTrue = item.StopIfTrue,
                            Minimum = new ExcelConditionalFormatValueObject()
                        };
                        rule2.Minimum.Type = this.GetScaleValueType(rule.Scales[index].Type);
                        if (rule.Scales[index].Value != null)
                        {
                            rule2.Minimum.Value = Convert.ToString(rule.Scales[index].Value, (IFormatProvider)CultureInfo.InvariantCulture).RemoveFormulaSymbolIfExists();
                        }
                        Windows.UI.Color color = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, (Windows.UI.Color)rule.Expected[index]);
                        rule2.MinimumColor = new ExcelColor(GcColor.FromArgb(color.A, color.R, color.G, color.B));
                        if (rule.Expected.Length >= 3)
                        {
                            index++;
                            if (rule.scales[index] != null)
                            {
                                rule2.HasMiddleNode = true;
                                rule2.Middle = new ExcelConditionalFormatValueObject();
                                rule2.Middle.Type = this.GetScaleValueType(rule.Scales[index].Type);
                                if (rule.Scales[index].Value != null)
                                {
                                    rule2.Middle.Value = Convert.ToString(rule.Scales[index].Value, (IFormatProvider)CultureInfo.InvariantCulture).RemoveFormulaSymbolIfExists();
                                }
                                Windows.UI.Color color2 = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, (Windows.UI.Color)rule.Expected[index]);
                                rule2.MiddleColor = new ExcelColor(GcColor.FromArgb(color2.A, color2.R, color2.G, color2.B));
                            }
                        }
                        index++;
                        rule2.Maximum = new ExcelConditionalFormatValueObject();
                        rule2.Maximum.Type = this.GetScaleValueType(rule.Scales[index].Type);
                        if (rule.Scales[index].Value != null)
                        {
                            rule2.Maximum.Value = Convert.ToString(rule.Scales[index].Value, (IFormatProvider)CultureInfo.InvariantCulture).RemoveFormulaSymbolIfExists();
                        }
                        Windows.UI.Color color3 = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, (Windows.UI.Color)rule.Expected[index]);
                        rule2.MaximumColor = new ExcelColor(GcColor.FromArgb(color3.A, color3.R, color3.G, color3.B));
                        format.IsOffice2007ConditionalFormat = true;
                        format.ConditionalFormattingRules.Add(rule2);
                        this._conditinalFormats[sheetIndex].Add(format);
                    }
                }
            }
        }

        void ProcessConditionalFormatDataBarSetRule(int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            DataBarRule rule = item as DataBarRule;
            ExcelConditionalFormat format = new ExcelConditionalFormat
            {
                Ranges = this.ToRanges(rule.Ranges)
            };
            if (format.Ranges != null)
            {
                short num;
                identfier = (short)((num = identfier) + 1);
                format.Identifier = num;
                Excel2010DataBarRule rule2 = new Excel2010DataBarRule
                {
                    Priority = item.Priority,
                    StopIfTrue = item.StopIfTrue,
                    Color = this.GetExcelRGBColor(rule.Color),
                    ShowValue = !rule.ShowBarOnly,
                    Minimum = new ExcelConditionalFormatValueObject()
                };
                rule2.Minimum.Type = this.GetScaleValueType(rule.MinimumType);
                if (rule.MinimumValue != null)
                {
                    rule2.Minimum.Value = Convert.ToString(rule.MinimumValue, (IFormatProvider)CultureInfo.InvariantCulture).RemoveFormulaSymbolIfExists();
                }
                rule2.Maximum = new ExcelConditionalFormatValueObject();
                rule2.Maximum.Type = this.GetScaleValueType(rule.MaximumType);
                if (rule.MaximumValue != null)
                {
                    rule2.Maximum.Value = Convert.ToString(rule.MaximumValue, (IFormatProvider)CultureInfo.InvariantCulture).RemoveFormulaSymbolIfExists();
                }
                rule2.MinimumDataBarLength = 0;
                rule2.MaximumDataBarLength = 100;
                format.IsOffice2007ConditionalFormat = true;
                if (rule.ShowBorder)
                {
                    rule2.ShowBorder = true;
                    rule2.BorderColor = this.GetExcelRGBColor(rule.BorderColor);
                }
                if (rule.UseNegativeFillColor)
                {
                    rule2.NegativeBarColorAsPositive = false;
                    rule2.NegativeFillColor = this.GetExcelRGBColor(rule.NegativeFillColor);
                }
                else
                {
                    rule2.NegativeBarColorAsPositive = true;
                }
                if (rule.ShowBorder && rule.UseNegativeBorderColor)
                {
                    rule2.NegativeBorderColorSameAsPositive = false;
                    rule2.NegativeBorderColor = this.GetExcelRGBColor(rule.NegativeBorderColor);
                }
                else
                {
                    rule2.NegativeBorderColorSameAsPositive = true;
                }
                switch (rule.AxisPosition)
                {
                    case Dt.Cells.Data.DataBarAxisPosition.Automatic:
                        rule2.AxisPosition = Dt.Xls.DataBarAxisPosition.Automatic;
                        break;

                    case Dt.Cells.Data.DataBarAxisPosition.CellMidPoint:
                        rule2.AxisPosition = Dt.Xls.DataBarAxisPosition.Middle;
                        break;

                    case Dt.Cells.Data.DataBarAxisPosition.None:
                        rule2.AxisPosition = Dt.Xls.DataBarAxisPosition.None;
                        break;
                }
                if (rule2.AxisPosition != Dt.Xls.DataBarAxisPosition.None)
                {
                    rule2.AxisColor = this.GetExcelRGBColor(rule.AxisColor);
                }
                if (rule.Gradient)
                {
                    rule2.IsGradientDatabar = true;
                }
                else
                {
                    rule2.IsGradientDatabar = false;
                }
                switch (rule.DataBarDirection)
                {
                    case BarDirection.LeftToRight:
                        rule2.Direction = DataBarDirection.Context;
                        break;

                    case BarDirection.RightToLeft:
                        rule2.Direction = DataBarDirection.RightToLeft;
                        break;
                }
                format.ConditionalFormattingRules.Add(rule2);
                this._conditinalFormats[sheetIndex].Add(format);
            }
        }

        void ProcessConditionalFormatDateOccurringRule(List<StyleInfo> styles, int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            DateOccurringRule rule = item as DateOccurringRule;
            if (rule != null)
            {
                short num2;
                ExcelConditionalFormat format = new ExcelConditionalFormat();
                identfier = (short)((num2 = identfier) + 1);
                format.Identifier = num2;
                format.Ranges = this.ToRanges(rule.Ranges);
                if (format.Ranges != null)
                {
                    ExcelConditionalFormatType today = ExcelConditionalFormatType.Today;
                    switch (rule.Operator)
                    {
                        case DateOccurringType.Today:
                            today = ExcelConditionalFormatType.Today;
                            break;

                        case DateOccurringType.Yesterday:
                            today = ExcelConditionalFormatType.Yesterday;
                            break;

                        case DateOccurringType.Tomorrow:
                            today = ExcelConditionalFormatType.Tomorrow;
                            break;

                        case DateOccurringType.Last7Days:
                            today = ExcelConditionalFormatType.Last7Days;
                            break;

                        case DateOccurringType.ThisMonth:
                            today = ExcelConditionalFormatType.ThisMonth;
                            break;

                        case DateOccurringType.LastMonth:
                            today = ExcelConditionalFormatType.LastMonth;
                            break;

                        case DateOccurringType.NextMonth:
                            today = ExcelConditionalFormatType.NextMonth;
                            break;

                        case DateOccurringType.ThisWeek:
                            today = ExcelConditionalFormatType.ThisWeek;
                            break;

                        case DateOccurringType.LastWeek:
                            today = ExcelConditionalFormatType.LastWeek;
                            break;

                        case DateOccurringType.NextWeek:
                            today = ExcelConditionalFormatType.NextWeek;
                            break;
                    }
                    ExcelGeneralRule rule2 = new ExcelGeneralRule(today)
                    {
                        Priority = item.Priority,
                        StopIfTrue = item.StopIfTrue
                    };
                    if (rule.Style == null)
                    {
                        rule2.DifferentialFormattingId = -1;
                    }
                    else
                    {
                        int index = styles.IndexOf(rule.Style);
                        if (index == -1)
                        {
                            index = styles.Count;
                            styles.Add(rule.Style);
                        }
                        rule2.DifferentialFormattingId = index;
                    }
                    if (rule2.Formulas.Count == 0)
                    {
                        string str = this.CreateConditionalFormatFormula(rule2, format.Ranges[0]);
                        if (str != null)
                        {
                            rule2.Formulas.Add(str);
                        }
                    }
                    format.ConditionalFormattingRules.Add(rule2);
                    this._conditinalFormats[sheetIndex].Add(format);
                }
            }
        }

        void ProcessConditionalFormatFormulaRule(List<StyleInfo> styles, int sheet, ref short identfier, FormattingRuleBase item)
        {
            FormulaRule rule = item as FormulaRule;
            if (rule != null)
            {
                short num2;
                ExcelConditionalFormat format = new ExcelConditionalFormat();
                identfier = (short)((num2 = identfier) + 1);
                format.Identifier = num2;
                format.Ranges = this.ToRanges(rule.Ranges);
                if ((format.Ranges != null) && !string.IsNullOrWhiteSpace(rule.Formula))
                {
                    ExcelConditionalFormatType expression = ExcelConditionalFormatType.Expression;
                    ExcelGeneralRule rule2 = new ExcelGeneralRule(expression)
                    {
                        StopIfTrue = item.StopIfTrue,
                        Priority = item.Priority
                    };
                    string str = rule.Formula.RemoveFormulaSymbolIfExists();
                    rule2.Formulas.Add(str);
                    if (rule.Style == null)
                    {
                        rule2.DifferentialFormattingId = -1;
                    }
                    else
                    {
                        int index = styles.IndexOf(rule.Style);
                        if (index == -1)
                        {
                            index = styles.Count;
                            styles.Add(rule.Style);
                        }
                        rule2.DifferentialFormattingId = index;
                    }
                    format.ConditionalFormattingRules.Add(rule2);
                    this._conditinalFormats[sheet].Add(format);
                }
            }
        }

        void ProcessConditionalFormatIconSetRule(int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            IconSetRule rule = item as IconSetRule;
            ExcelConditionalFormat format = new ExcelConditionalFormat
            {
                Ranges = this.ToRanges(rule.Ranges)
            };
            if (format.Ranges != null)
            {
                short num;
                identfier = (short)((num = identfier) + 1);
                format.Identifier = num;
                ExcelIconSetsRule rule2 = new ExcelIconSetsRule
                {
                    Thresholds = new List<IExcelConditionalFormatValueObject>(),
                    NotPassTheThresholdsWhenEquals = new List<bool>()
                };
                ExcelConditionalFormatValueObject obj2 = new ExcelConditionalFormatValueObject
                {
                    Type = ExcelConditionalFormatValueObjectType.Percent,
                    Value = "0"
                };
                rule2.Thresholds.Add(obj2);
                rule2.NotPassTheThresholdsWhenEquals.Add(false);
                rule2.IconOnly = rule.ShowIconOnly;
                rule2.ReversedOrder = rule.ReverseIconOrder;
                rule2.Priority = item.Priority;
                rule2.StopIfTrue = item.StopIfTrue;
                foreach (IconCriterion criterion in rule.IconCriteria)
                {
                    rule2.Thresholds.Add(criterion.ToIconditionalFormatValueObject());
                    rule2.NotPassTheThresholdsWhenEquals.Add(!criterion.IsGreaterThanOrEqualTo);
                }
                rule2.IconSet = this.GetExcelIconSetType(rule.IconSetType);
                format.IsOffice2007ConditionalFormat = true;
                format.ConditionalFormattingRules.Add(rule2);
                this._conditinalFormats[sheetIndex].Add(format);
            }
        }

        void ProcessConditionalFormatScaleRule(int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            if (item is DataBarRule)
            {
                this.ProcessConditionalFormatDataBarSetRule(sheetIndex, ref identfier, item);
            }
            else
            {
                this.ProcessConditionalFormatColorScaleRule(sheetIndex, ref identfier, item);
            }
        }

        void ProcessConditionalFormatSpecificTextRule(List<StyleInfo> styles, int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            SpecificTextRule rule = item as SpecificTextRule;
            if (rule != null)
            {
                short num2;
                ExcelConditionalFormat format = new ExcelConditionalFormat();
                identfier = (short)((num2 = identfier) + 1);
                format.Identifier = num2;
                format.Ranges = this.ToRanges(rule.Ranges);
                if (format.Ranges != null)
                {
                    ExcelConditionalFormatType containsText = ExcelConditionalFormatType.ContainsText;
                    ExcelConditionalFormattingOperator notContains = ExcelConditionalFormattingOperator.ContainsText;
                    if (rule.Operator == TextComparisonOperator.DoesNotContain)
                    {
                        containsText = ExcelConditionalFormatType.NotContainsText;
                        notContains = ExcelConditionalFormattingOperator.NotContains;
                    }
                    else if (rule.Operator == TextComparisonOperator.BeginsWith)
                    {
                        containsText = ExcelConditionalFormatType.BeginsWith;
                        notContains = ExcelConditionalFormattingOperator.BeginsWith;
                    }
                    else if (rule.Operator == TextComparisonOperator.EndsWith)
                    {
                        containsText = ExcelConditionalFormatType.EndsWith;
                        notContains = ExcelConditionalFormattingOperator.EndsWith;
                    }
                    ExcelGeneralRule rule2 = new ExcelGeneralRule(containsText)
                    {
                        Priority = item.Priority,
                        Operator = new ExcelConditionalFormattingOperator?(notContains),
                        Text = rule.Text,
                        StopIfTrue = item.StopIfTrue
                    };
                    if (rule.Style == null)
                    {
                        rule2.DifferentialFormattingId = -1;
                    }
                    else
                    {
                        int index = styles.IndexOf(rule.Style);
                        if (index == -1)
                        {
                            index = styles.Count;
                            styles.Add(rule.Style);
                        }
                        rule2.DifferentialFormattingId = index;
                    }
                    if (rule2.Formulas.Count == 0)
                    {
                        string str = this.CreateConditionalFormatFormula(rule2, format.Ranges[0]);
                        if (str != null)
                        {
                            rule2.Formulas.Add(str);
                        }
                    }
                    format.ConditionalFormattingRules.Add(rule2);
                    this._conditinalFormats[sheetIndex].Add(format);
                }
            }
        }

        void ProcessConditionalFormatTop10Rule(List<StyleInfo> styles, int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            Top10Rule rule = item as Top10Rule;
            if (rule != null)
            {
                short num2;
                ExcelConditionalFormat format = new ExcelConditionalFormat();
                identfier = (short)((num2 = identfier) + 1);
                format.Identifier = num2;
                format.Ranges = this.ToRanges(rule.Ranges);
                if (format.Ranges != null)
                {
                    ExcelConditionalFormatType cfType = ExcelConditionalFormatType.Top10;
                    ExcelGeneralRule rule2 = new ExcelGeneralRule(cfType)
                    {
                        StopIfTrue = item.StopIfTrue
                    };
                    if (rule.Operator == Top10ConditionType.Bottom)
                    {
                        rule2.Bottom = true;
                    }
                    rule2.Percent = false;
                    rule2.Priority = item.Priority;
                    rule2.Rank = new int?(rule.Rank);
                    if (rule.Style == null)
                    {
                        rule2.DifferentialFormattingId = -1;
                    }
                    else
                    {
                        int index = styles.IndexOf(rule.Style);
                        if (index == -1)
                        {
                            index = styles.Count;
                            styles.Add(rule.Style);
                        }
                        rule2.DifferentialFormattingId = index;
                    }
                    if (rule2.Formulas.Count == 0)
                    {
                        string str = this.CreateConditionalFormatFormula(rule2, format.Ranges[0]);
                        if (str != null)
                        {
                            rule2.Formulas.Add(str);
                        }
                    }
                    format.ConditionalFormattingRules.Add(rule2);
                    this._conditinalFormats[sheetIndex].Add(format);
                }
            }
        }

        void ProcessConditionalFormatUniqueRule(List<StyleInfo> styles, int sheetIndex, ref short identfier, FormattingRuleBase item)
        {
            FormattingRuleBase base2 = item;
            if (base2 != null)
            {
                short num2;
                ExcelConditionalFormat format = new ExcelConditionalFormat();
                identfier = (short)((num2 = identfier) + 1);
                format.Identifier = num2;
                format.Ranges = this.ToRanges(base2.Ranges);
                if (format.Ranges != null)
                {
                    ExcelConditionalFormatType uniqueValues = ExcelConditionalFormatType.UniqueValues;
                    if (base2 is DuplicateRule)
                    {
                        uniqueValues = ExcelConditionalFormatType.DuplicateValues;
                    }
                    ExcelGeneralRule rule = new ExcelGeneralRule(uniqueValues)
                    {
                        Priority = item.Priority,
                        StopIfTrue = item.StopIfTrue
                    };
                    if (base2.Style == null)
                    {
                        rule.DifferentialFormattingId = -1;
                    }
                    else
                    {
                        int index = styles.IndexOf(base2.Style);
                        if (index == -1)
                        {
                            index = styles.Count;
                            styles.Add(base2.Style);
                        }
                        rule.DifferentialFormattingId = index;
                    }
                    if (rule.Formulas.Count == 0)
                    {
                        string str = this.CreateConditionalFormatFormula(rule, format.Ranges[0]);
                        if (str != null)
                        {
                            rule.Formulas.Add(str);
                        }
                    }
                    format.ConditionalFormattingRules.Add(rule);
                    this._conditinalFormats[sheetIndex].Add(format);
                }
            }
        }

        object ProcessRowStyle(Worksheet worksheet, Dictionary<int, int> rowStyleTable, int row)
        {
            object obj2 = null;
            if (!this._saveFlags.SaveDataOnly())
            {
                string str = null;
                object obj3 = worksheet.GetStyleObject(row, -1, SheetArea.Cells);
                if (obj3 is string)
                {
                    str = (string)(obj3 as string);
                    obj2 = str;
                }
                if (this._saveFlags.SaveAsViewed())
                {
                    FormattingRuleBase[] rules = worksheet.ConditionalFormats.GetRules(row, -1);
                    if (rules.Length != 0)
                    {
                        bool flag = false;
                        foreach (FormattingRuleBase base2 in rules)
                        {
                            if (base2.Contains(row, -1))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            str = null;
                            StyleInfo style = worksheet.GetActualStyleInfo(row, -1, SheetArea.Cells);
                            if (style != null)
                            {
                                rowStyleTable.Add(row, this.GetStyleIndex(style));
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(str))
                {
                    if (this._namedStyleTable.ContainsKey(str))
                    {
                        rowStyleTable.Add(row, this._namedStyleTable[str]);
                        return obj2;
                    }
                    rowStyleTable.Add(row, -1);
                    return obj2;
                }
                StyleInfo styleInfo = worksheet.GetStyleInfo(row, -1, SheetArea.Cells);
                obj2 = styleInfo;
                if (styleInfo != null)
                {
                    if (this.IsEmptyStyleInfo(styleInfo) && styleInfo.IsParentSet())
                    {
                        StyleInfo info3 = this._workbook.NamedStyles.Find(styleInfo.Parent);
                        if (info3 != null)
                        {
                            string parent = styleInfo.Parent;
                            styleInfo = info3.Clone() as StyleInfo;
                            styleInfo.ResetName();
                            styleInfo.Parent = parent;
                            obj2 = styleInfo;
                        }
                    }
                    rowStyleTable.Add(row, this.GetStyleIndex(styleInfo));
                    return obj2;
                }
                rowStyleTable.Add(row, -1);
            }
            return obj2;
        }

        void SetCellFormula(Tuple<string, CellRange> formulaStruct, Dt.Cells.Data.ExcelCell cell)
        {
            bool flag = this._workbook.ReferenceStyle == ReferenceStyle.A1;
            if (((formulaStruct != null) && !string.IsNullOrWhiteSpace(formulaStruct.Item1)) && (!this._saveFlags.SaveDataOnly() && !this._saveFlags.NoFormulas()))
            {
                cell.CellFormula = new ExcelFormula();
                string str = formulaStruct.Item1;
                bool flag2 = false;
                if (str.StartsWith("{") && str.EndsWith("}"))
                {
                    str = str.Substring(1, str.Length - 2);
                    flag2 = true;
                }
                if (flag2)
                {
                    cell.CellType = CellType.Array;
                }
                else
                {
                    cell.CellType = CellType.FormulaString;
                }
                if (flag)
                {
                    if (!flag2)
                    {
                        cell.CellFormula.Formula = str;
                        cell.Formula = str;
                    }
                    else
                    {
                        cell.FormulaArray = str;
                        cell.IsArrayFormula = true;
                        cell.CellFormula.Formula = str;
                        cell.CellFormula.IsArrayFormula = true;
                        CellRange range = formulaStruct.Item2;
                        ExcelCellRange range2 = new ExcelCellRange
                        {
                            Row = range.Row,
                            Column = range.Column,
                            RowSpan = range.RowCount,
                            ColumnSpan = range.ColumnCount
                        };
                        cell.CellFormula.ArrayFormulaRange = range2;
                    }
                }
                else if (!flag2)
                {
                    cell.CellFormula.FormulaR1C1 = str;
                    cell.FormulaR1C1 = str;
                }
                else
                {
                    cell.FormulaArrayR1C1 = str;
                    cell.IsArrayFormula = true;
                    cell.CellFormula.FormulaR1C1 = str;
                    cell.CellFormula.IsArrayFormula = true;
                    CellRange range3 = formulaStruct.Item2;
                    ExcelCellRange range4 = new ExcelCellRange
                    {
                        Row = range3.Row,
                        Column = range3.Column,
                        RowSpan = range3.RowCount,
                        ColumnSpan = range3.ColumnCount
                    };
                    cell.CellFormula.ArrayFormulaRange = range4;
                }
            }
        }

        void SetCellStyle(Dictionary<long, int> cellStyleTable, int styleKey, StyleInfo styleInfo, string styleName, Dt.Cells.Data.ExcelCell cell)
        {
            if (!string.IsNullOrEmpty(styleName))
            {
                int id = -1;
                if (this._namedStyleTable.TryGetValue(styleName, out id))
                {
                    cell.SetFormatId(id);
                }
                else
                {
                    cell.SetFormatId(-1);
                }
            }
            else if (styleInfo != null)
            {
                int num2 = -1;
                if (cellStyleTable.TryGetValue((long)styleKey, out num2))
                {
                    cell.SetFormatId(num2);
                }
                else
                {
                    cell.SetFormatId(-1);
                }
            }
            else
            {
                cell.SetFormatId(-1);
            }
        }

        void ShiftColumnHeader(int sheetIndex)
        {
            if (this._saveFlags.SaveCustomColumnHeaders())
            {
                Worksheet worksheet = this._workbook.Sheets[sheetIndex];
                for (int i = 0; i < worksheet.ColumnHeader.RowCount; i++)
                {
                    for (int k = 0; k < worksheet.ColumnCount; k++)
                    {
                        Cell cell = worksheet.ColumnHeader.Cells[i, k];
                        Cell cell2 = worksheet.Cells[i, k];
                        cell2.Value = cell.Value;
                        cell2.RowSpan = cell.RowSpan;
                        cell2.ColumnSpan = cell.ColumnSpan;
                        if (!string.IsNullOrWhiteSpace(cell.StyleName))
                        {
                            worksheet.SetStyleName(i, k, cell.StyleName);
                        }
                        else
                        {
                            worksheet.SetStyleInfo(i, k, worksheet.GetActualStyleInfo(i, k, SheetArea.ColumnHeader));
                        }
                    }
                }
                for (int j = 0; j < worksheet.ColumnHeader.RowCount; j++)
                {
                    worksheet.SetRowHeight(j, SheetArea.Cells, worksheet.GetRowHeight(j, SheetArea.ColumnHeader));
                    worksheet.SetColumnVisible(j, SheetArea.Cells, worksheet.GetColumnVisible(j, SheetArea.ColumnHeader));
                }
            }
        }

        void ShiftRowHeader(int sheetIndex)
        {
            if (this._saveFlags.SaveCustomRowHeaders())
            {
                Worksheet worksheet = this._workbook.Sheets[sheetIndex];
                for (int i = 0; i < worksheet.RowCount; i++)
                {
                    for (int k = 0; k < worksheet.RowHeader.ColumnCount; k++)
                    {
                        Cell cell = worksheet.RowHeader.Cells[i, k];
                        Cell cell2 = worksheet.Cells[i, k];
                        cell2.Value = cell.Value;
                        cell2.RowSpan = cell.RowSpan;
                        cell2.ColumnSpan = cell.ColumnSpan;
                        if (!string.IsNullOrWhiteSpace(cell.StyleName))
                        {
                            worksheet.SetStyleName(i, k, cell.StyleName);
                        }
                        else
                        {
                            worksheet.SetStyleInfo(i, k, worksheet.GetActualStyleInfo(i, k, SheetArea.CornerHeader | SheetArea.RowHeader));
                        }
                    }
                }
                for (int j = 0; j < worksheet.RowHeader.ColumnCount; j++)
                {
                    worksheet.SetColumnWidth(j, SheetArea.Cells, worksheet.GetColumnWidth(j, SheetArea.CornerHeader | SheetArea.RowHeader));
                    worksheet.SetColumnVisible(j, SheetArea.Cells, worksheet.GetColumnVisible(j, SheetArea.CornerHeader | SheetArea.RowHeader));
                }
            }
        }

        void ShiftWorkbook()
        {
            this._sheetsRowOffsets = new int[this._workbook.SheetCount];
            this._sheetscolumnOffsets = new int[this._workbook.SheetCount];
            for (int j = 0; j < this._workbook.SheetCount; j++)
            {
                Worksheet worksheet = this._workbook.Sheets[j];
                if (this._saveFlags.SaveCustomRowHeaders())
                {
                    this._sheetscolumnOffsets[j] = worksheet.RowHeader.ColumnCount;
                    worksheet.AddColumns(0, worksheet.RowHeader.ColumnCount);
                }
                if (this._saveFlags.SaveCustomColumnHeaders())
                {
                    this._sheetsRowOffsets[j] = worksheet.ColumnHeader.RowCount;
                    worksheet.AddRows(0, worksheet.ColumnHeader.RowCount);
                }
            }
        }

        IRange ToRange(CellRange range)
        {
            if (range == null)
            {
                return null;
            }
            int row = range.Row;
            int column = range.Column;
            int rowCount = range.RowCount;
            int columnCount = range.ColumnCount;
            if ((row == -1) && (rowCount == -1))
            {
                row = 0;
                rowCount = 0x100000;
            }
            if ((column == -1) && (columnCount == -1))
            {
                column = 0;
                columnCount = 0x4000;
            }
            return new ExcelCellRange { Row = row, RowSpan = rowCount, Column = column, ColumnSpan = columnCount };
        }

        List<IRange> ToRanges(CellRange[] cellRanges)
        {
            if ((cellRanges == null) || (cellRanges.Length == 0))
            {
                return null;
            }
            List<IRange> list = new List<IRange>();
            foreach (CellRange range in cellRanges)
            {
                if (range != null)
                {
                    list.Add(this.ToRange(range));
                }
            }
            return list;
        }

        void UpdateChartSeriesIndex(IExcelChart excelChart)
        {
            CovarianceList<IExcelChartSeriesBase> list = new CovarianceList<IExcelChartSeriesBase>();
            if (excelChart.BarChart != null)
            {
                list.AddRange<IExcelBarSeries>((IEnumerable<IExcelBarSeries>)excelChart.BarChart.BarSeries);
            }
            if (excelChart.StockChart != null)
            {
                list.AddRange<IExcelLineSeries>((IEnumerable<IExcelLineSeries>)excelChart.StockChart.LineSeries);
            }
            if (excelChart.PieChart != null)
            {
                list.AddRange<IExcelPieSeries>((IEnumerable<IExcelPieSeries>)excelChart.PieChart.PieSeries);
            }
            if (excelChart.DoughnutChart != null)
            {
                list.AddRange<IExcelPieSeries>((IEnumerable<IExcelPieSeries>)excelChart.DoughnutChart.PieSeries);
            }
            if (excelChart.LineChart != null)
            {
                list.AddRange<IExcelLineSeries>((IEnumerable<IExcelLineSeries>)excelChart.LineChart.LineSeries);
            }
            if (excelChart.BubbleChart != null)
            {
                list.AddRange<ExcelBubbleSeries>((IEnumerable<ExcelBubbleSeries>)excelChart.BubbleChart.BubbleSeries);
            }
            if (excelChart.AreaChart != null)
            {
                list.AddRange<IExcelAreaSeries>((IEnumerable<IExcelAreaSeries>)excelChart.AreaChart.AreaSeries);
            }
            if (excelChart.ScatterChart != null)
            {
                list.AddRange<IExcelScatterSeries>((IEnumerable<IExcelScatterSeries>)excelChart.ScatterChart.ScatterSeries);
            }
            if (excelChart.RadarChart != null)
            {
                list.AddRange<IExcelRadarSeries>((IEnumerable<IExcelRadarSeries>)excelChart.RadarChart.RadarSeries);
            }
            if (list.Count > 0)
            {
                foreach (IExcelChartSeriesBase base2 in list)
                {
                    base2.Index = DataSeriesCounter.Index;
                    base2.Order = DataSeriesCounter.Index;
                    DataSeriesCounter.Index++;
                }
            }
        }

        double UpdateMaxRowHeightIfNeeded(Workbook workbook, List<double> columnsWidthCollection, double maxRowHeight, int column, StyleInfo styleInfo, Dt.Cells.Data.ExcelCell cell)
        {
            if ((this._saveFlags.AutoRowHeight() && (cell.Value != null)) && (styleInfo != null))
            {
                FontFamily bodyFontFamily = null;
                if (!string.IsNullOrWhiteSpace(styleInfo.FontTheme))
                {
                    if (styleInfo.FontTheme.ToUpperInvariant() == "Body")
                    {
                        bodyFontFamily = workbook.CurrentTheme.BodyFontFamily;
                    }
                    else if (styleInfo.FontTheme.ToUpperInvariant() == "Headings")
                    {
                        bodyFontFamily = workbook.CurrentTheme.HeadingFontFamily;
                    }
                }
                else if (styleInfo.FontFamily != null)
                {
                    bodyFontFamily = styleInfo.FontFamily;
                }
                if (styleInfo.FontFamily == null)
                {
                    styleInfo.FontFamily = (workbook.DefaultStyle.FontFamily != null) ? workbook.DefaultStyle.FontFamily : new FontFamily(NameConstans.DEFAULT_FONT_FAMILY);
                }
                double fontSize = (styleInfo.FontSize >= 0.0) ? styleInfo.FontSize : ((workbook.DefaultStyle.FontSize * 96.0) / 72.0);
                double num2 = this.CalcRowHeight(cell.Value.ToString(), bodyFontFamily, fontSize, styleInfo.WordWrap, styleInfo.FontWeight, styleInfo.FontStyle, columnsWidthCollection[column]);
                if (num2 > maxRowHeight)
                {
                    maxRowHeight = num2;
                }
            }
            return maxRowHeight;
        }

        HashSet<string> BuiltInTableStyleNames
        {
            get
            {
                if (this._builtInTableStyleNames == null)
                {
                    this._builtInTableStyleNames = new HashSet<string>();
                    for (int i = 1; i < 0x1d; i++)
                    {
                        this._builtInTableStyleNames.Add("LIGHT" + ((int)i).ToString());
                        this._builtInTableStyleNames.Add("MEDIUM" + ((int)i).ToString());
                        if (i < 12)
                        {
                            this._builtInTableStyleNames.Add("DARK" + ((int)i).ToString());
                        }
                    }
                }
                return this._builtInTableStyleNames;
            }
        }

        HashSet<string> CellRangeNames
        {
            get
            {
                if (this._cellRangeNames == null)
                {
                    this._cellRangeNames = new HashSet<string>();
                    foreach (IName name in this.GetInternalDefinedNames())
                    {
                        this._cellRangeNames.Add(name.Name.ToUpperInvariant());
                    }
                }
                return this._cellRangeNames;
            }
        }
    }
}

