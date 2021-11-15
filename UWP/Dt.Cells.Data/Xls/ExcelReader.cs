#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.Xls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal class ExcelReader : IExcelReader, IExcelReader2, IExcelSparklineReader, IExcelTableReader
    {
        int _activeSheetIndex;
        Dictionary<int, string> _builtInStyleNames;
        List<IExcelStyle> _builtInStyles;
        Dictionary<string, TableStyle> _builtInTableStyles;
        Dictionary<int, Tuple<int, int>> _cachedActivePosition;
        List<StyleInfo> _cellStyleInfos;
        Dictionary<int, Dictionary<int, List<int>>> _columnCollapsedInfo;
        List<bool> _dateTimeFormatter;
        IExtendedFormat _defaultExcelExtendedFormat;
        string _defaultTableStyleName;
        List<StyleInfo> _differentCellStyles;
        List<IDifferentialFormatting> _differentFormats;
        List<IExtendedFormat> _extendedFormats;
        bool _initStyle;
        bool _isDate1904;
        double _maxiumDigitWidth;
        Dictionary<int, List<IName>> _namedCellRanges;
        Dictionary<string, IExtendedFormat> _namedStylesExtendedFormats;
        int _normalStleIndex;
        StyleInfo _normalStyleInfo;
        ExcelOpenFlags _openFlags;
        Dictionary<int, Dictionary<int, List<int>>> _rowCollaspedInfo;
        Dictionary<string, int> _sheetColumnDemensions;
        Dictionary<string, List<IExcelConditionalFormat>> _sheetsCondtionalForats;
        int _startSheetIndex;
        List<string> _styleNames;
        List<IExcelStyle> _styles;
        Dictionary<string, TableStyle> _tableStyleCollection;
        List<bool> _textFormatter;
        HashSet<string> _uniqueTableNames;
        Workbook _workbook;
        static Dictionary<char, int> DateAndTimeFormatElementDict = new Dictionary<char, int>();
        static string MeasureItem = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        static ExcelReader()
        {
            DateAndTimeFormatElementDict.Add('Y', 4);
            DateAndTimeFormatElementDict.Add('D', 4);
            DateAndTimeFormatElementDict.Add('M', 5);
            DateAndTimeFormatElementDict.Add('H', 2);
            DateAndTimeFormatElementDict.Add('S', 2);
        }

        internal ExcelReader(Workbook owner)
            : this(owner, ExcelOpenFlags.NoFlagsSet)
        {
        }

        internal ExcelReader(Workbook owner, ExcelOpenFlags openFlags)
        {
            this._extendedFormats = new List<IExtendedFormat>();
            this._styleNames = new List<string>();
            this._cellStyleInfos = new List<StyleInfo>();
            this._differentCellStyles = new List<StyleInfo>();
            this._differentFormats = new List<IDifferentialFormatting>();
            this._namedCellRanges = new Dictionary<int, List<IName>>();
            this._builtInStyleNames = new Dictionary<int, string>();
            this._maxiumDigitWidth = double.NaN;
            this._dateTimeFormatter = new List<bool>();
            this._textFormatter = new List<bool>();
            this._styles = new List<IExcelStyle>();
            this._namedStylesExtendedFormats = new Dictionary<string, IExtendedFormat>();
            this._initStyle = true;
            this._rowCollaspedInfo = new Dictionary<int, Dictionary<int, List<int>>>();
            this._columnCollapsedInfo = new Dictionary<int, Dictionary<int, List<int>>>();
            this._tableStyleCollection = new Dictionary<string, TableStyle>();
            this._uniqueTableNames = new HashSet<string>();
            this._cachedActivePosition = new Dictionary<int, Tuple<int, int>>();
            this._sheetColumnDemensions = new Dictionary<string, int>();
            this._sheetsCondtionalForats = new Dictionary<string, List<IExcelConditionalFormat>>();
            this._startSheetIndex = -1;
            this._activeSheetIndex = -1;
            ExcelImportExportGlobalVariables.Instance = new ExcelImportExportObjects();
            this._workbook = owner;
            this._openFlags = openFlags;
        }

        void AddAverageCondtionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelGeneralRule generalRule, ExcelConditionalFormatType type)
        {
            AverageConditionType above = AverageConditionType.Above;
            bool flag = true;
            if (type == ExcelConditionalFormatType.BelowAverage)
            {
                flag = false;
                above = AverageConditionType.Below;
            }
            if (type == ExcelConditionalFormatType.BelowOrEqualToAverage)
            {
                flag = false;
                above = AverageConditionType.EqualOrBelow;
            }
            if (type == ExcelConditionalFormatType.AboveOrEqualToAverage)
            {
                above = AverageConditionType.EqualOrAbove;
            }
            if (generalRule.StdDev.HasValue)
            {
                int num = generalRule.StdDev.Value;
                if (flag)
                {
                    switch (num)
                    {
                        case 1:
                            above = AverageConditionType.Above1StdDev;
                            break;

                        case 2:
                            above = AverageConditionType.Above2StdDev;
                            break;

                        case 3:
                            above = AverageConditionType.Above3StdDev;
                            break;
                    }
                }
                else
                {
                    switch (num)
                    {
                        case 1:
                            above = AverageConditionType.Below1StdDev;
                            break;

                        case 2:
                            above = AverageConditionType.Below2StdDev;
                            break;

                        case 3:
                            above = AverageConditionType.Below3StdDev;
                            break;
                    }
                }
            }
            worksheet.ConditionalFormats.AddAverageRule(above, (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
        }

        void AddColorScaleConditionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelConditionalFormatRule rule)
        {
            IExcelColorScaleRule rule2 = rule as IExcelColorScaleRule;
            if ((rule2 != null) && rule2.HasMiddleNode)
            {
                ScaleValueType scaleValueType = this.GetScaleValueType(rule2.Minimum.Type);
                ScaleValueType midType = this.GetScaleValueType(rule2.Middle.Type);
                ScaleValueType maxType = this.GetScaleValueType(rule2.Maximum.Type);
                worksheet.ConditionalFormats.AddThreeScaleRule(scaleValueType, (scaleValueType == ScaleValueType.Formula) ? this.AddFormulaSymbolsIfNeeded(rule2.Minimum.Value) : this.ConvertValueToFormulaIfNeeded(rule2.Minimum.Value), ColorHelper.GetRGBColor(this._workbook, rule2.MinimumColor), midType, (midType == ScaleValueType.Formula) ? this.AddFormulaSymbolsIfNeeded(rule2.Middle.Value) : this.ConvertValueToFormulaIfNeeded(rule2.Middle.Value), ColorHelper.GetRGBColor(this._workbook, rule2.MiddleColor), maxType, (maxType == ScaleValueType.Formula) ? this.AddFormulaSymbolsIfNeeded(rule2.Maximum.Value) : this.ConvertValueToFormulaIfNeeded(rule2.Maximum.Value), ColorHelper.GetRGBColor(this._workbook, rule2.MaximumColor), ranges.ToArray());
            }
            else
            {
                ScaleValueType minType = this.GetScaleValueType(rule2.Minimum.Type);
                ScaleValueType type5 = this.GetScaleValueType(rule2.Maximum.Type);
                worksheet.ConditionalFormats.AddTwoScaleRule(minType, (minType == ScaleValueType.Formula) ? this.AddFormulaSymbolsIfNeeded(rule2.Minimum.Value) : this.ConvertValueToFormulaIfNeeded(rule2.Minimum.Value), ColorHelper.GetRGBColor(this._workbook, rule2.MinimumColor), type5, (type5 == ScaleValueType.Formula) ? this.AddFormulaSymbolsIfNeeded(rule2.Maximum.Value) : this.ConvertValueToFormulaIfNeeded(rule2.Maximum.Value), ColorHelper.GetRGBColor(this._workbook, rule2.MaximumColor), ranges.ToArray());
            }
        }

        /// <summary>
        /// hdt 唐忠宝修改，从reflector 中重新拷贝代码替换原代码。
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="ranges"></param>
        /// <param name="rule"></param>
        void AddDataBarConditionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelConditionalFormatRule rule)
        {
            IExcelDataBarRule rule2 = rule as IExcelDataBarRule;
            DataBarRule rule3 = null;
            if (rule2 != null)
            {
                ScaleValueType scaleValueType = this.GetScaleValueType(rule2.Minimum.Type);
                string str = rule2.Minimum.Value;
                if (scaleValueType == ScaleValueType.Formula)
                {
                    str = this.AddFormulaSymbolsIfNeeded(str);
                }
                else
                {
                    str = this.ConvertValueToFormulaIfNeeded(str);
                }
                ScaleValueType maxType = this.GetScaleValueType(rule2.Maximum.Type);
                string str2 = rule2.Maximum.Value;
                if (maxType == ScaleValueType.Formula)
                {
                    str2 = this.AddFormulaSymbolsIfNeeded(str2);
                }
                else
                {
                    str2 = this.ConvertValueToFormulaIfNeeded(str2);
                }
                rule3 = new DataBarRule(scaleValueType, str, maxType, str2, ColorHelper.GetRGBColor(this._workbook, rule2.Color))
                {
                    Ranges = ranges.ToArray(),
                    ShowBarOnly = !rule2.ShowValue,
                    Priority = rule2.Priority
                };
                worksheet.ConditionalFormats.AddRule(rule3, false);
            }
            if ((rule3 != null) && (rule is IExcel2010DataBarRule))
            {
                IExcel2010DataBarRule rule4 = rule as IExcel2010DataBarRule;
                if (rule4.ShowBorder)
                {
                    rule3.ShowBorder = true;
                    rule3.BorderColor = ColorHelper.GetRGBColor(this._workbook, rule4.BorderColor);
                }
                if (!rule4.NegativeBarColorAsPositive)
                {
                    rule3.UseNegativeFillColor = true;
                    rule3.NegativeFillColor = ColorHelper.GetRGBColor(this._workbook, rule4.NegativeFillColor);
                }
                else
                {
                    rule3.UseNegativeFillColor = false;
                }
                if (rule4.ShowBorder && !rule4.NegativeBorderColorSameAsPositive)
                {
                    rule3.UseNegativeBorderColor = true;
                    rule3.NegativeBorderColor = ColorHelper.GetRGBColor(this._workbook, rule4.NegativeBorderColor);
                }
                switch (rule4.AxisPosition)
                {
                    case Dt.Xls.DataBarAxisPosition.Automatic:
                        rule3.AxisPosition = DataBarAxisPosition.Automatic;
                        break;

                    case Dt.Xls.DataBarAxisPosition.Middle:
                        rule3.AxisPosition = DataBarAxisPosition.CellMidPoint;
                        break;

                    case Dt.Xls.DataBarAxisPosition.None:
                        rule3.AxisPosition = DataBarAxisPosition.None;
                        break;
                }
                if (rule3.AxisPosition != DataBarAxisPosition.None)
                {
                    rule3.AxisColor = ColorHelper.GetRGBColor(this._workbook, rule4.AxisColor);
                }
                if (rule4.IsGradientDatabar)
                {
                    rule3.Gradient = true;
                }
                else
                {
                    rule3.Gradient = false;
                }
                switch (rule4.Direction)
                {
                    case DataBarDirection.Context:
                    case DataBarDirection.LeftToRight:
                        rule3.DataBarDirection = BarDirection.LeftToRight;
                        return;

                    case DataBarDirection.RightToLeft:
                        rule3.DataBarDirection = BarDirection.RightToLeft;
                        break;

                    default:
                        return;
                }
            }
        }

        void AddDatesOccuringConditionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelGeneralRule generalRule, ExcelConditionalFormatType type)
        {
            DateOccurringType today = DateOccurringType.Today;
            if (type == ExcelConditionalFormatType.Tomorrow)
            {
                today = DateOccurringType.Tomorrow;
            }
            else if (type == ExcelConditionalFormatType.Yesterday)
            {
                today = DateOccurringType.Yesterday;
            }
            else if (type == ExcelConditionalFormatType.Last7Days)
            {
                today = DateOccurringType.Last7Days;
            }
            else if (type == ExcelConditionalFormatType.LastMonth)
            {
                today = DateOccurringType.LastMonth;
            }
            else if (type == ExcelConditionalFormatType.LastWeek)
            {
                today = DateOccurringType.LastWeek;
            }
            else if (type == ExcelConditionalFormatType.ThisWeek)
            {
                today = DateOccurringType.ThisWeek;
            }
            else if (type == ExcelConditionalFormatType.ThisMonth)
            {
                today = DateOccurringType.ThisMonth;
            }
            else if (type == ExcelConditionalFormatType.NextMonth)
            {
                today = DateOccurringType.NextMonth;
            }
            else if (type == ExcelConditionalFormatType.NextWeek)
            {
                today = DateOccurringType.NextWeek;
            }
            worksheet.ConditionalFormats.AddDateOccurringRule(today, (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
        }

        string AddFormulaSymbolsIfNeeded(object value)
        {
            if (value == null)
            {
                return null;
            }
            string str = value.ToString();
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            return ("=" + str);
        }

        void AddGeneralRuleConditionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelConditionalFormatRule rule)
        {
            IExcelGeneralRule generalRule = rule as IExcelGeneralRule;
            if (generalRule != null)
            {
                ExcelConditionalFormatType type = generalRule.Type;
                switch (type)
                {
                    case ExcelConditionalFormatType.Expression:
                    case ExcelConditionalFormatType.ContainsBlanks:
                    case ExcelConditionalFormatType.NotContainsBlanks:
                    case ExcelConditionalFormatType.ContainsErrors:
                    case ExcelConditionalFormatType.NotContainsErrors:
                        worksheet.ConditionalFormats.AddFormulaRule(generalRule.Formulas[0], (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
                        return;

                    case ExcelConditionalFormatType.ColorScale:
                    case ExcelConditionalFormatType.DataBar:
                    case ExcelConditionalFormatType.IconSet:
                    case (ExcelConditionalFormatType.IconSet | ExcelConditionalFormatType.ColorScale):
                    case (ExcelConditionalFormatType.NotContainsErrors | ExcelConditionalFormatType.Expression):
                    case (ExcelConditionalFormatType.NotContainsErrors | ExcelConditionalFormatType.ColorScale):
                    case (ExcelConditionalFormatType.ThisMonth | ExcelConditionalFormatType.IconSet):
                        return;

                    case ExcelConditionalFormatType.Top10:
                        this.AddTop10ConditionalFormat(worksheet, ranges, generalRule);
                        return;

                    case ExcelConditionalFormatType.UniqueValues:
                        worksheet.ConditionalFormats.AddUniqueRule((generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
                        return;

                    case ExcelConditionalFormatType.ContainsText:
                        worksheet.ConditionalFormats.AddSpecificTextRule(TextComparisonOperator.Contains, generalRule.Text, (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
                        return;

                    case ExcelConditionalFormatType.Today:
                    case ExcelConditionalFormatType.Tomorrow:
                    case ExcelConditionalFormatType.Yesterday:
                    case ExcelConditionalFormatType.Last7Days:
                    case ExcelConditionalFormatType.LastMonth:
                    case ExcelConditionalFormatType.NextMonth:
                    case ExcelConditionalFormatType.ThisWeek:
                    case ExcelConditionalFormatType.NextWeek:
                    case ExcelConditionalFormatType.LastWeek:
                    case ExcelConditionalFormatType.ThisMonth:
                    case ExcelConditionalFormatType.TimePeriod:
                        this.AddDatesOccuringConditionalFormat(worksheet, ranges, generalRule, type);
                        return;

                    case ExcelConditionalFormatType.AboveAverage:
                    case ExcelConditionalFormatType.BelowAverage:
                    case ExcelConditionalFormatType.AboveOrEqualToAverage:
                    case ExcelConditionalFormatType.BelowOrEqualToAverage:
                        this.AddAverageCondtionalFormat(worksheet, ranges, generalRule, type);
                        return;

                    case ExcelConditionalFormatType.DuplicateValues:
                        worksheet.ConditionalFormats.AddDuplicateRule((generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
                        return;

                    case ExcelConditionalFormatType.BeginsWith:
                        worksheet.ConditionalFormats.AddSpecificTextRule(TextComparisonOperator.BeginsWith, generalRule.Text, (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
                        return;

                    case ExcelConditionalFormatType.EndsWith:
                        worksheet.ConditionalFormats.AddSpecificTextRule(TextComparisonOperator.EndsWith, generalRule.Text, (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
                        return;

                    case ExcelConditionalFormatType.NotContainsText:
                        worksheet.ConditionalFormats.AddSpecificTextRule(TextComparisonOperator.DoesNotContain, generalRule.Text, (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], ranges.ToArray());
                        return;
                }
            }
        }

        void AddHighlightConditionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelConditionalFormatRule rule)
        {
            IExcelHighlightingRule rule2 = rule as IExcelHighlightingRule;
            if (rule2 != null)
            {
                object obj2 = null;
                object obj3 = null;
                obj2 = rule2.Formulas[0];
                if (rule2.Formulas.Count == 2)
                {
                    obj3 = rule2.Formulas[1];
                }
                obj2 = this.AddFormulaSymbolsIfNeeded(obj2);
                obj3 = this.AddFormulaSymbolsIfNeeded(obj3);
                worksheet.ConditionalFormats.AddCellValueRule((ComparisonOperator)rule2.ComparisonOperator, obj2, obj3, (rule2.DifferentialFormattingId == -1) ? null : this._differentCellStyles[rule2.DifferentialFormattingId], ranges.ToArray());
            }
        }

        void AddIconSetCondtionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelConditionalFormatRule rule)
        {
            IExcelIconSetsRule rule2 = rule as IExcelIconSetsRule;
            if ((rule2 != null) && (rule2.IconSet != ExcelIconSetType.Icon_NIL))
            {
                IconSetRule rule3 = new IconSetRule(this.GetIconSetType(rule2.IconSet))
                {
                    Ranges = ranges.ToArray()
                };
                if ((rule2.Thresholds != null) && (rule2.Thresholds.Count > 0))
                {
                    if ((rule3.IconSetType >= IconSetType.ThreeArrowsColored) && (rule3.IconSetType <= IconSetType.ThreeSymbolsUncircled))
                    {
                        rule3.IconCriteria[0] = rule2.Thresholds[1].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[1]);
                        rule3.IconCriteria[1] = rule2.Thresholds[2].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[2]);
                    }
                    else if ((rule3.IconSetType >= IconSetType.FourArrowsColored) && (rule3.IconSetType <= IconSetType.FourTrafficLights))
                    {
                        rule3.IconCriteria[0] = rule2.Thresholds[1].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[1]);
                        rule3.IconCriteria[1] = rule2.Thresholds[2].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[2]);
                        rule3.IconCriteria[2] = rule2.Thresholds[3].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[3]);
                    }
                    else if ((rule3.IconSetType >= IconSetType.FiveArrowsColored) && (rule3.IconSetType <= IconSetType.FiveBoxes))
                    {
                        rule3.IconCriteria[0] = rule2.Thresholds[1].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[1]);
                        rule3.IconCriteria[1] = rule2.Thresholds[2].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[2]);
                        rule3.IconCriteria[2] = rule2.Thresholds[3].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[3]);
                        rule3.IconCriteria[3] = rule2.Thresholds[4].ToIconCriterion(!rule2.NotPassTheThresholdsWhenEquals[4]);
                    }
                }
                rule3.ShowIconOnly = rule2.IconOnly;
                rule3.ReverseIconOrder = rule2.ReversedOrder;
                worksheet.ConditionalFormats.AddRule(rule3);
            }
        }

        public void AddSheet(string name, byte hiddenState, ExcelSheetType sheetType)
        {
            Worksheet item = new Worksheet(name)
            {
                ShowGridLine = true
            };
            item.SuspendSpanModelEvent();
            item.SuspendCalcService();
            item.AutoRecalculation = false;
            item.Visible = hiddenState == 0;
            item.ReferenceStyle = this._workbook.ReferenceStyle;
            item.ColumnCount = 500;
            if (item.ReferenceStyle == ReferenceStyle.R1C1)
            {
                item.ColumnHeader.AutoText = HeaderAutoText.Numbers;
            }
            else
            {
                item.ColumnHeader.AutoText = HeaderAutoText.Letters;
            }
            this._workbook.Sheets.Add(item);
            item.FormulaService.SuspendCalcGraphs();
            item.CalcAxial.Suspend();
        }

        void AddStyleToWorkbookNameStyles(StyleInfo namedStyle)
        {
            if (this._workbook.NamedStyles == null)
            {
                this._workbook.NamedStyles = new StyleInfoCollection();
            }
            this._workbook.NamedStyles.Add(namedStyle);
        }

        void AddTop10ConditionalFormat(Worksheet worksheet, List<CellRange> ranges, IExcelGeneralRule generalRule)
        {
            Top10ConditionType top = Top10ConditionType.Top;
            if (generalRule.Bottom.HasValue && generalRule.Bottom.Value)
            {
                top = Top10ConditionType.Bottom;
            }
            int rank = 10;
            if (generalRule.Rank.HasValue)
            {
                rank = generalRule.Rank.Value;
            }
            foreach (CellRange range in ranges)
            {
                if (generalRule.Percent.HasValue && generalRule.Percent.Value)
                {
                    rank = (int)(((range.RowCount * range.ColumnCount) * rank) / 100L);
                    if (rank == 0)
                    {
                        rank = 1;
                    }
                }
                worksheet.ConditionalFormats.AddTop10Rule(top, rank, (generalRule.DifferentialFormattingId == -1) ? null : this._differentCellStyles[generalRule.DifferentialFormattingId], new CellRange[] { range });
            }
        }

        public void Bof(short sheet)
        {
        }

        double CalculateDefaultRowHeight()
        {
            FontFamily fontFamily = this.GetFontFamily();
            double fontSize = this._workbook.DefaultStyle.FontSize;
            double result = UnitHelper.PixelToPoint(1.0 + this.MeasureStringHeight(MeasureItem, fontFamily.Source, fontSize, false, false, false, -1.0));
            return result;
        }

        object ConvertExcelErrorToCalcEngineError(object value)
        {
            string str = value.ToString();
            switch (str)
            {
                case "#DIV/0!":
                    return new CalcDivideByZeroError();

                case "#N/A":
                    return new CalcNotAvailableError();

                case "#NAME?":
                    return new CalcNameError();

                case "#NULL!":
                    return new CalcNullError();

                case "#NUM!":
                    return new CalcNumberError();

                case "#REF!":
                    return new CalcReferenceError();

                case "#VALUE!":
                    return new CalcValueError();
            }
            return str;
        }

        string ConvertValueToFormulaIfNeeded(string value)
        {
            int num;
            if (!int.TryParse(value, out num))
            {
                float num2;
                double num3;
                decimal num4;
                if (float.TryParse(value, out num2))
                {
                    return value;
                }
                if (double.TryParse(value, out num3))
                {
                    return value;
                }
                if (decimal.TryParse(value, out num4))
                {
                    return value;
                }
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return ("=" + value);
                }
            }
            return value;
        }

        public void Finish()
        {
            if (this._startSheetIndex != -1)
            {
                this._workbook.StartSheetIndex = this._startSheetIndex;
            }
            if (this._activeSheetIndex != -1)
            {
                this._workbook.ActiveSheetIndex = this._activeSheetIndex;
            }
            if (!this._workbook.CanCellOverflow)
            {
                this._workbook.CanCellOverflow = true;
            }
            if (!string.IsNullOrWhiteSpace(this._defaultTableStyleName))
            {
                this._workbook.DefaultTableStyle = this.GetTableStyle(this._defaultTableStyleName);
            }
            foreach (Worksheet worksheet in this._workbook.Sheets)
            {
                int num = worksheet.GetLastDirtyColumn(StorageType.Data) + 1;
                if (this._sheetColumnDemensions.ContainsKey(worksheet.Name))
                {
                    int num2 = this._sheetColumnDemensions[worksheet.Name];
                    if (num2 > num)
                    {
                        num = num2;
                    }
                }
                worksheet.ColumnCount = num;
            }
            foreach (Worksheet worksheet2 in this._workbook.Sheets)
            {
                worksheet2.CalcAxial.Resume();
            }
            this.SetNamedCellRange();
            foreach (Worksheet worksheet3 in this._workbook.Sheets)
            {
                try
                {
                    worksheet3.FormulaService.ResumeCalcGraphs();
                }
                catch (Exception exception)
                {
                    this.OnExcelLoadError(new ExcelWarning(ResourceStrings.excelReaderFinish, ExcelWarningCode.General, -1, -1, -1, exception));
                }
            }
            this.SetRowColumnGroupCollapsedInfo();
            this.SetRowColumnHeaders();
            try
            {
                this.UpdateWorksheetsStatus();
                this.ReCalculateWorkbook(true);
            }
            catch (Exception exception2)
            {
                this.OnExcelLoadError(new ExcelWarning(ResourceStrings.excelReaderFinish, ExcelWarningCode.General, -1, -1, -1, exception2));
            }
            foreach (Worksheet worksheet4 in this._workbook.Sheets)
            {
                worksheet4.ResumeSpanModelEvent();
            }
            this.ResetExcelReaderToDefault();
            ExtendedNumberFormatHelper.Reset();
        }

        PaneType GetActivePane(short sheet)
        {
            ViewportInfo viewportInfo = this._workbook.Sheets[sheet].GetViewportInfo();
            if ((viewportInfo.ActiveRowViewport != 0) || (viewportInfo.ActiveColumnViewport != 0))
            {
                if ((viewportInfo.ActiveRowViewport == 1) && (viewportInfo.ActiveColumnViewport == 0))
                {
                    return PaneType.BottomLeft;
                }
                if ((viewportInfo.ActiveRowViewport == 1) && (viewportInfo.ActiveColumnViewport == 1))
                {
                    return PaneType.BottomRight;
                }
                if ((viewportInfo.ActiveRowViewport == 0) && (viewportInfo.ActiveColumnViewport == 1))
                {
                    return PaneType.TopRight;
                }
            }
            return PaneType.TopLeft;
        }

        TableStyle GetBuiltItTableStyle(string styleName)
        {
            TableStyle style = null;
            if (this.BuiltInTableStyles.TryGetValue(styleName, out style))
            {
                return style;
            }
            return this._workbook.DefaultTableStyle;
        }

        double GetColumnWidthInPixel(int sheet, double width)
        {
            if (width == 0.0)
            {
                return 0.0;
            }
            double maxiumDigitWidth = this.GetMaxiumDigitWidth();
            return Math.Floor((double)(((Math.Floor((double)(width * 256.0)) / 256.0) * maxiumDigitWidth) + 0.5));
        }

        ConditionBase GetCondition(IExcelCustomFilter customFilter)
        {
            if (customFilter == null)
            {
                return null;
            }
            string expected = customFilter.Value.ToString();
            int index = expected.IndexOf("*");
            int num2 = expected.LastIndexOf("*");
            if (index != -1)
            {
                if ((index == (expected.Length - 1)) && (index != 0))
                {
                    return TextCondition.FromString(TextCompareType.BeginsWith, expected.Substring(0, expected.Length - 1));
                }
                if ((index == 0) && (index == num2))
                {
                    return TextCondition.FromString(TextCompareType.EndsWith, expected.Substring(1));
                }
                TextCompareType contains = TextCompareType.Contains;
                if (customFilter.Operator == ExcelFilterOperator.NotEqual)
                {
                    contains = TextCompareType.DoesNotContain;
                }
                if ((index != -1) && (num2 != -1))
                {
                    expected = expected.Substring(1, expected.Length - 2);
                }
                return TextCondition.FromString(contains, expected);
            }
            GeneralCompareType equalsTo = GeneralCompareType.EqualsTo;
            switch (customFilter.Operator)
            {
                case ExcelFilterOperator.LessThan:
                    equalsTo = GeneralCompareType.LessThan;
                    break;

                case ExcelFilterOperator.LessThanOrEqual:
                    equalsTo = GeneralCompareType.LessThanOrEqualsTo;
                    break;

                case ExcelFilterOperator.GreaterThan:
                    equalsTo = GeneralCompareType.GreaterThan;
                    break;

                case ExcelFilterOperator.NotEqual:
                    equalsTo = GeneralCompareType.NotEqualsTo;
                    break;

                case ExcelFilterOperator.GreaterThanOrEqual:
                    equalsTo = GeneralCompareType.GreaterThanOrEqualsTo;
                    break;
            }
            return new CellValueCondition(equalsTo, customFilter.Value, null);
        }

        List<CellRange> GetConditionalFormatRanges(List<IRange> ranges)
        {
            List<CellRange> list = new List<CellRange>();
            for (int i = 0; i < ranges.Count; i++)
            {
                IRange range = ranges[i];
                bool flag = true;
                for (int j = 0; j < list.Count; j++)
                {
                    if (((list[j].Row == range.Row) && (list[j].RowCount == range.RowSpan)) && ((list[j].Column == range.Column) && (list[j].ColumnCount == range.ColumnSpan)))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    list.Add(new CellRange(range.Row, range.Column, range.RowSpan, range.ColumnSpan));
                }
            }
            return list;
        }

        double GetDefaultColumnWidthInPixel(int sheet, double width)
        {
            if (width == 0.0)
            {
                return 0.0;
            }
            double num2 = (Math.Ceiling((double)(this.GetMaxiumDigitWidth() / 4.0)) * 2.0) + 1.0;
            double num3 = this.GetColumnWidthInPixel(sheet, width) + num2;
            return (Math.Ceiling((double)(num3 / 8.0)) * 8.0);
        }

        object GetExcelRealUnderlyingCellValue(CellType type, object value)
        {
            double num2;
            System.DateTime time;
            if (value == null)
            {
                return null;
            }
            if (value is ExcelCalcError)
            {
                return this.ConvertExcelErrorToCalcEngineError(value);
            }
            if (((type == CellType.String) || (type == CellType.FormulaString)) || ((type == CellType.Blank) || (type == CellType.Array)))
            {
                return value;
            }
            if ((type == CellType.Unknown) || (type == CellType.Numeric))
            {
                double num;
                if (!this.IsNumber(value) && double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out num))
                {
                    return (double)num;
                }
                return value;
            }
            if (type == CellType.Error)
            {
                return this.ConvertExcelErrorToCalcEngineError(value);
            }
            if ((type != CellType.Datetime) || !double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out num2))
            {
                return value;
            }
            if (this._isDate1904)
            {
                num2 += 1462.0;
            }
            if (ConverterHelper.FromExcelOADate(num2, out time))
            {
                return time;
            }
            return (double)num2;
        }

        int GetExtendedFormatIndex(IExtendedFormat format)
        {
            for (int i = 0; i < this._extendedFormats.Count; i++)
            {
                if (this._extendedFormats[i].Equals(format))
                {
                    return i;
                }
            }
            return -1;
        }

        FontFamily GetFontFamily()
        {
            FontFamily fontFamily = null;
            if (this._workbook.DefaultStyle.IsFontFamilySet())
            {
                fontFamily = this._workbook.DefaultStyle.FontFamily;
            }
            else
            {
                fontFamily = DefaultStyleCollection.DefaultFontFamily;
            }
            if (fontFamily == null)
            {
                fontFamily = new FontFamily("Microsoft YaHei");
            }
            return fontFamily;
        }

        IconSetType GetIconSetType(ExcelIconSetType iconset)
        {
            switch (iconset)
            {
                case ExcelIconSetType.Icon_3Arrows:
                    return IconSetType.ThreeArrowsColored;

                case ExcelIconSetType.Icon_3ArrowsGray:
                    return IconSetType.ThreeArrowsGray;

                case ExcelIconSetType.Icon_3Flags:
                    return IconSetType.ThreeFlags;

                case ExcelIconSetType.Icon_3TrafficLights1:
                    return IconSetType.ThreeTrafficLightsUnrimmed;

                case ExcelIconSetType.Icon_3TrafficLights2:
                    return IconSetType.ThreeTrafficLightsRimmed;

                case ExcelIconSetType.Icon_3Signs:
                    return IconSetType.ThreeSigns;

                case ExcelIconSetType.Icon_3Symbols:
                    return IconSetType.ThreeSymbolsCircled;

                case ExcelIconSetType.Icon_3Symbols2:
                    return IconSetType.ThreeSymbolsUncircled;

                case ExcelIconSetType.Icon_4Arrows:
                    return IconSetType.FourArrowsColored;

                case ExcelIconSetType.Icon_4ArrowsGray:
                    return IconSetType.FourArrowsGray;

                case ExcelIconSetType.Icon_4RedToBlack:
                    return IconSetType.FourRedToBlack;

                case ExcelIconSetType.Icon_4Rating:
                    return IconSetType.FourRatings;

                case ExcelIconSetType.Icon_4TrafficLights:
                    return IconSetType.FourTrafficLights;

                case ExcelIconSetType.Icon_5Arrows:
                    return IconSetType.FiveArrowsColored;

                case ExcelIconSetType.Icon_5ArrowsGray:
                    return IconSetType.FiveArrowsGray;

                case ExcelIconSetType.Icon_5Rating:
                    return IconSetType.FiveRatings;

                case ExcelIconSetType.Icon_5Quarters:
                    return IconSetType.FiveQuarters;

                case ExcelIconSetType.Icon_3Stars:
                    return IconSetType.ThreeStars;

                case ExcelIconSetType.Icon_3Triangles:
                    return IconSetType.ThreeTriangles;

                case ExcelIconSetType.Icon_5Boxes:
                    return IconSetType.FiveBoxes;
            }
            return IconSetType.ThreeTrafficLightsUnrimmed;
        }

        double GetMaxiumDigitWidth()
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
                        block.FontFamily = new FontFamily("Microsoft YaHei");
                    }
                }
                else
                {
                    if (this._normalStyleInfo.FontFamily != null)
                    {
                        block.FontFamily = this._normalStyleInfo.FontFamily;
                    }
                    if (block.FontFamily == null)
                    {
                        block.FontFamily = new FontFamily("Microsoft YaHei");
                    }
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
                    double width = block.DesiredSize.Width;
                    if (width > num)
                    {
                        num = width;
                    }
                }
                this._maxiumDigitWidth = num;
            }
            return this._maxiumDigitWidth;
        }

        ScaleValueType GetScaleValueType(ExcelConditionalFormatValueObjectType cfvoType)
        {
            switch (cfvoType)
            {
                case ExcelConditionalFormatValueObjectType.Num:
                    return ScaleValueType.Number;

                case ExcelConditionalFormatValueObjectType.Min:
                    return ScaleValueType.LowestValue;

                case ExcelConditionalFormatValueObjectType.Max:
                    return ScaleValueType.HighestValue;

                case ExcelConditionalFormatValueObjectType.Percent:
                    return ScaleValueType.Percent;

                case ExcelConditionalFormatValueObjectType.Percentile:
                    return ScaleValueType.Percentile;

                case ExcelConditionalFormatValueObjectType.Formula:
                    return ScaleValueType.Formula;

                case ExcelConditionalFormatValueObjectType.AutoMin:
                    return ScaleValueType.Automin;

                case ExcelConditionalFormatValueObjectType.AutoMax:
                    return ScaleValueType.Automax;
            }
            return ScaleValueType.Number;
        }

        TableStyle GetTableStyle(string styleName)
        {
            if (string.IsNullOrWhiteSpace(styleName))
            {
                return null;
            }
            string baseBuiltInStyleName = null;
            if (!this.IsModifiedTableStyle(styleName, out baseBuiltInStyleName))
            {
                return this.GetBuiltItTableStyle(baseBuiltInStyleName);
            }
            if (this._tableStyleCollection.ContainsKey(styleName))
            {
                return this._tableStyleCollection[styleName];
            }
            return this._workbook.DefaultTableStyle;
        }

        string GetUniqueTableName(string name)
        {
            if (!this._uniqueTableNames.Contains(name))
            {
                this._uniqueTableNames.Add(name);
                return name;
            }
            int num = 1;
            while (true)
            {
                string item = name + "_" + ((int)num).ToString();
                if (!this._uniqueTableNames.Contains(item))
                {
                    this._uniqueTableNames.Add(item);
                    return item;
                }
                num++;
            }
        }

        void InitExcelDefaultBuiltInStyles()
        {
            if (this._initStyle)
            {
                this._workbook.NamedStyles.Clear();
                BuiltInExcelStyles builtInExcelStyleCollection = new BuiltInExcelStyles();
                this._builtInStyles = builtInExcelStyleCollection.GetBuiltInStyls();
                foreach (IExcelStyle style in this._builtInStyles)
                {
                    if ((style != null) && (style.Name == "Normal"))
                    {
                        this._normalStleIndex = this._extendedFormats.Count;
                    }
                    this._styles.Add(style);
                    this._extendedFormats.Add(style.Format);
                    this._styleNames.Add(style.Name);
                    this._namedStylesExtendedFormats[style.Name] = style.Format;
                    this.AddStyleToWorkbookNameStyles(new StyleInfo(style.Name, "", style.Format.ToCellStyleInfo(this._workbook)));
                }
            }
        }

        bool IsDatesOrTimesNumberFormat(string formatCode)
        {
            if (string.IsNullOrWhiteSpace(formatCode))
            {
                return false;
            }
            if (formatCode.Length == 0)
            {
                return false;
            }
            formatCode = formatCode.ToUpperInvariant().Trim();
            if (formatCode[0] == '[')
            {
                int index = formatCode.IndexOf(']');
                if ((index == -1) || (index == (formatCode.Length - 1)))
                {
                    return false;
                }
                formatCode = formatCode.Substring(index + 1);
            }
            int startIndex = -1;
            int length = 0;
            bool flag = false;
            bool? nullable = null;
            int num4 = 0;
            while (num4 < formatCode.Length)
            {
                char ch = formatCode[num4];
                switch (ch)
                {
                    case '[':
                        do
                        {
                            ch = formatCode[num4];
                            num4++;
                        }
                        while ((num4 < formatCode.Length) && (ch != ']'));
                        break;

                    case 'Y':
                    case 'D':
                    case 'M':
                    case 'H':
                    case 'S':
                        if (!flag)
                        {
                            flag = true;
                            startIndex = num4;
                            length = 1;
                        }
                        else
                        {
                            length++;
                        }
                        goto Label_0126;
                }
                flag = false;
                if (((ch == ' ') && nullable.HasValue) && nullable.Value)
                {
                    return true;
                }
                if (length != 0)
                {
                    string pattern = formatCode.Substring(startIndex, length);
                    startIndex = num4;
                    length = 0;
                    if (!nullable.HasValue)
                    {
                        nullable = new bool?(this.IsValidDateOrTimeFormatPattern(pattern));
                    }
                    else
                    {
                        nullable = new bool?(nullable.Value & this.IsValidDateOrTimeFormatPattern(pattern));
                    }
                }
            Label_0126:
                num4++;
            }
            if ((length != 0) && (num4 == formatCode.Length))
            {
                string str2 = formatCode.Substring(startIndex, length);
                if (!nullable.HasValue)
                {
                    nullable = new bool?(this.IsValidDateOrTimeFormatPattern(str2));
                }
                else
                {
                    nullable = new bool?(nullable.Value & this.IsValidDateOrTimeFormatPattern(str2));
                }
            }
            return (nullable.HasValue && nullable.Value);
        }

        bool IsEntireRow(IRange range)
        {
            return (range.RowSpan == 0x100000);
        }

        bool IsEntrieColumn(IRange range)
        {
            return (range.ColumnSpan == 0x4000);
        }

        bool IsModifiedTableStyle(string styleName, out string baseBuiltInStyleName)
        {
            string str = styleName.ToUpperInvariant();
            if (str.StartsWith("TABLESTYLE") && (str.Length > 9))
            {
                str = str.Substring(10);
            }
            else
            {
                baseBuiltInStyleName = null;
                return true;
            }
            baseBuiltInStyleName = str;
            int index = str.IndexOf(' ');
            if (index > 0)
            {
                baseBuiltInStyleName = str.Substring(0, index);
            }
            if (index == -1)
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
            return ((((((value is double) || (value is float)) || ((value is int) || (value is long))) || (((value is short) || (value is System.DateTime)) || ((value is System.TimeSpan) || (value is decimal)))) || (((value is sbyte) || (value is ulong)) || ((value is uint) || (value is ushort)))) || (value is byte));
        }

        bool IsValidDateOrTimeFormatPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return false;
            }
            foreach (var type in pattern.ToCharArray().GroupBy<char, char>(delegate(char c)
            {
                return c;
            }).Select(delegate(IGrouping<char, char> items)
            {
                return new { Key = items.Key, Count = items.Count<char>() };
            }))
            {
                int num = 0;
                if (!DateAndTimeFormatElementDict.TryGetValue(type.Key, out num))
                {
                    return false;
                }
                if (type.Count > num)
                {
                    return false;
                }
            }
            return true;
        }

        bool IsValidWorkSheet(int sheet)
        {
            return ((sheet >= 0) && (sheet < this._workbook.SheetCount));
        }

        double MeasureStringHeight(string text, string fontName, double fontSize, bool wordWrap, bool bold, bool italic, double width)
        {
            TextBlock block = new TextBlock();
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = "Microsoft YaHei";
            }
            block.FontFamily = new FontFamily(fontName);
            block.FontSize = fontSize;
            if (bold)
            {
                block.FontWeight = FontWeights.Bold;
            }
            if (italic)
            {
                block.FontStyle = Windows.UI.Text.FontStyle.Italic;
            }
            block.Margin = new Windows.UI.Xaml.Thickness(4.0);
            block.TextWrapping = wordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            block.Text = text;
            block.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            block.Measure(new Windows.Foundation.Size((width <= 0.0) ? double.MaxValue : width, double.MaxValue));
            return Math.Floor(block.ActualHeight);
        }

        public void OnExcelLoadError(ExcelWarning excelWarning)
        {
            if (excelWarning != null)
            {
                this._workbook.OnExcelError(excelWarning);
            }
        }

        void ReCalculateWorkbook(bool forceRecalculateAll = false)
        {
            if (!this._openFlags.DoNotRecalculateAfterLoad())
            {
                this._workbook.FormulaService.Recalculate(0xc350, forceRecalculateAll);
            }
        }

        void ResetCellStyleInfoOfTableRangeCell(Worksheet worksheet, int row, int column, StyleInfo defaultStyle)
        {
            StyleInfo styleInfo = worksheet.GetStyleInfo(row, column);
            if (styleInfo != null)
            {
                StyleInfo style = styleInfo.Clone() as StyleInfo;
                if (style.IsBackgroundSet() && defaultStyle.IsBackgroundSet())
                {
                    if (((style.Background is SolidColorBrush) && (defaultStyle.Background is SolidColorBrush)) && ((style.Background as SolidColorBrush).Color == (defaultStyle.Background as SolidColorBrush).Color))
                    {
                        style.ResetBackground();
                    }
                }
                else if ((style.IsBackgroundThemeColorSet() && defaultStyle.IsBackgroundThemeColorSet()) && (style.BackgroundThemeColor == defaultStyle.BackgroundThemeColor))
                {
                    style.ResetBackgroundThemeColor();
                }
                if (style.IsForegroundSet() && defaultStyle.IsForegroundSet())
                {
                    if (((style.Foreground is SolidColorBrush) && (defaultStyle.Foreground is SolidColorBrush)) && ((style.Foreground as SolidColorBrush).Color == (defaultStyle.Foreground as SolidColorBrush).Color))
                    {
                        style.ResetForeground();
                    }
                }
                else if ((style.IsForegroundThemeColorSet() && defaultStyle.IsForegroundThemeColorSet()) && (style.ForegroundThemeColor == defaultStyle.ForegroundThemeColor))
                {
                    style.ResetForegroundThemeColor();
                }
                if ((style.IsBorderBottomSet() && defaultStyle.IsBorderBottomSet()) && (style.BorderBottom == defaultStyle.BorderBottom))
                {
                    style.ResetBorderBottom();
                }
                if ((style.IsBorderLeftSet() && defaultStyle.IsBorderLeftSet()) && (style.BorderLeft == defaultStyle.BorderLeft))
                {
                    style.ResetBorderLeft();
                }
                if ((style.IsBorderRightSet() && defaultStyle.IsBorderRightSet()) && (style.BorderRight == defaultStyle.BorderRight))
                {
                    style.ResetBorderRight();
                }
                if ((style.IsBorderTopSet() && defaultStyle.IsBorderTopSet()) && (style.BorderTop == defaultStyle.BorderTop))
                {
                    style.ResetBorderTop();
                }
                if (((style.IsFontFamilySet() && defaultStyle.IsFontFamilySet()) && ((defaultStyle.FontFamily != null) && (style.FontFamily != null))) && (defaultStyle.FontFamily.Source == style.FontFamily.Source))
                {
                    style.ResetFontFamily();
                }
                if ((style.IsFontSizeSet() && defaultStyle.IsFontSizeSet()) && (style.FontSize == defaultStyle.FontSize))
                {
                    style.ResetFontSize();
                }
                if ((style.IsFontStyleSet() && defaultStyle.IsFontStyleSet()) && (style.FontStyle == defaultStyle.FontStyle))
                {
                    style.ResetFontStyle();
                }
                if (style.IsFormatterSet() && defaultStyle.IsFormatterSet())
                {
                    if ((style.Formatter == null) && (defaultStyle.Formatter == null))
                    {
                        style.ResetFormatter();
                    }
                    else if (((style.Formatter != null) && (defaultStyle.Formatter != null)) && ((style.Formatter.GetType() == defaultStyle.Formatter.GetType()) && (style.Formatter.FormatString == defaultStyle.Formatter.FormatString)))
                    {
                        style.ResetFormatter();
                    }
                }
                if ((style.IsHorizontalAlignmentSet() && defaultStyle.IsHorizontalAlignmentSet()) && (style.HorizontalAlignment == defaultStyle.HorizontalAlignment))
                {
                    style.ResetHorizontalAlignment();
                }
                if ((style.IsVerticalAlignmentSet() && defaultStyle.IsVerticalAlignmentSet()) && (style.VerticalAlignment == defaultStyle.VerticalAlignment))
                {
                    style.ResetVerticalAlignment();
                }
                if ((style.IsTextIndentSet() && defaultStyle.IsTextIndentSet()) && (style.TextIndent == defaultStyle.TextIndent))
                {
                    style.ResetTextIndent();
                }
                if ((style.IsFontWeightSet() && defaultStyle.IsFontWeightSet()) && style.FontWeight.Equals(defaultStyle.FontWeight))
                {
                    style.ResetFontWeight();
                }
                if ((style.IsWordWrapSet() && defaultStyle.IsWordWrapSet()) && (style.WordWrap == defaultStyle.WordWrap))
                {
                    style.ResetWordWrap();
                }
                if ((style.IsLockedSet() && defaultStyle.IsLockedSet()) && (style.Locked == defaultStyle.Locked))
                {
                    style.ResetLocked();
                }
                if ((style.IsFocusableSet() && defaultStyle.IsFocusableSet()) && (style.Focusable == defaultStyle.Focusable))
                {
                    style.ResetFocusable();
                }
                if ((style.IsTabStopSet() && defaultStyle.IsTabStopSet()) && (style.TabStop == defaultStyle.TabStop))
                {
                    style.ResetTabStop();
                }
                if ((style.IsShrinkToFitSet() && defaultStyle.IsShrinkToFitSet()) && (style.ShrinkToFit == defaultStyle.ShrinkToFit))
                {
                    style.ResetShrinkToFit();
                }
                string name = style.Name;
                style.ResetName();
                if (style.IsEmpty)
                {
                    worksheet.SetStyleInfo(row, column, null);
                }
                else
                {
                    style.Name = style.Name;
                    worksheet.SetStyleInfo(row, column, style);
                }
            }
        }

        void ResetExcelReaderToDefault()
        {
            this._extendedFormats.Clear();
            this._styleNames.Clear();
            this._styles.Clear();
            this._dateTimeFormatter.Clear();
            this._textFormatter.Clear();
            this._cellStyleInfos.Clear();
            this._differentCellStyles.Clear();
            this._differentFormats.Clear();
            this._normalStleIndex = 0;
            this._defaultExcelExtendedFormat = null;
            this._isDate1904 = false;
            this._namedCellRanges.Clear();
            this._builtInStyleNames.Clear();
            this._namedStylesExtendedFormats.Clear();
            this._openFlags = ExcelOpenFlags.NoFlagsSet;
            this._maxiumDigitWidth = double.NaN;
            this._normalStyleInfo = null;
            this._builtInStyles = null;
            this._rowCollaspedInfo.Clear();
            this._columnCollapsedInfo.Clear();
            ColorHelper.UseCustomPalette = false;
            ColorHelper.CustomPalette = null;
            this._tableStyleCollection.Clear();
            this._defaultTableStyleName = null;
            this._uniqueTableNames.Clear();
            this._cachedActivePosition.Clear();
            this._sheetColumnDemensions.Clear();
            this._sheetsCondtionalForats.Clear();
            this._activeSheetIndex = -1;
            this._startSheetIndex = 1;
        }

        public void SetArrayFormula(int sheet, int rowFirst, int rowLast, short columnFirst, short columnLast, IExcelFormula arrayFormula)
        {
            if (!this._openFlags.DataOnly() && (arrayFormula != null))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                rowLast = (rowLast < worksheet.RowCount) ? rowLast : (worksheet.RowCount - 1);
                columnLast = (columnLast < worksheet.ColumnCount) ? ((short)columnLast) : ((short)(worksheet.ColumnCount - 1));
                string str = (this._workbook.ReferenceStyle == ReferenceStyle.A1) ? arrayFormula.Formula : arrayFormula.FormulaR1C1;
                if (!string.IsNullOrWhiteSpace(str) && !string.IsNullOrWhiteSpace(str))
                {
                    this._workbook.Sheets[sheet].SetFormula(rowFirst, columnFirst, (rowLast - rowFirst) + 1, (columnLast - columnFirst) + 1, SheetArea.Cells, str, true);
                }
            }
        }

        public void SetAutoFilter(short sheet, IExcelAutoFilter autoFilter)
        {
            if (((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (((autoFilter != null) && (autoFilter.Range != null)) && (autoFilter.FilterColumns != null))) && this.IsValidWorkSheet(sheet))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                int num = (autoFilter.Range.Row + autoFilter.Range.RowSpan) - 1;
                int num2 = (autoFilter.Range.Column + autoFilter.Range.ColumnSpan) - 1;
                if (num >= worksheet.RowCount)
                {
                    worksheet.RowCount = num + 1;
                }
                if (num2 >= worksheet.ColumnCount)
                {
                    worksheet.ColumnCount = num2 + 1;
                }
                CellRange range = new CellRange(autoFilter.Range.Row + 1, autoFilter.Range.Column, (autoFilter.Range.RowSpan == 1) ? ((worksheet.RowCount - autoFilter.Range.Row) - 1) : (autoFilter.Range.RowSpan - 1), autoFilter.Range.ColumnSpan);
                if (worksheet.RowFilter == null)
                {
                    worksheet.RowFilter = new HideRowFilter(range);
                }
                foreach (IExcelFilterColumn column in autoFilter.FilterColumns)
                {
                    int num3 = (int)(column.AutoFilterColumnId + range.Column);
                    worksheet.RowFilter.RemoveFilterItems(num3);
                    this.SetForegroundBackgroundAutoFilter(worksheet.RowFilter, column, num3);
                    this.SetIconAutoFilter(column);
                    this.SetFilters(worksheet.RowFilter, column, num3);
                    this.SetTop10AutoFilter(worksheet.RowFilter, column, num3);
                    this.SetDynamicAutoFilter(worksheet.RowFilter, range, column, num3);
                    this.SetCustomAutoFilter(worksheet.RowFilter, column, num3);
                }
                worksheet.RowFilter.Filter();
            }
        }

        public void SetBuiltInNameList(List<IBuiltInName> builtInNames)
        {
        }

        public void SetCalculationProperty(ICalculationProperty calcProperty)
        {
            if (calcProperty != null)
            {
                if (calcProperty.RefMode == ExcelReferenceStyle.A1)
                {
                    this._workbook.ReferenceStyle = ReferenceStyle.A1;
                    for (int i = 0; i < this._workbook.SheetCount; i++)
                    {
                        this._workbook.Sheets[i].ColumnHeaderAutoText = HeaderAutoText.Letters;
                    }
                }
                else
                {
                    this._workbook.ReferenceStyle = ReferenceStyle.R1C1;
                    for (int j = 0; j < this._workbook.SheetCount; j++)
                    {
                        this._workbook.Sheets[j].ColumnHeaderAutoText = HeaderAutoText.Numbers;
                    }
                }
            }
        }

        public bool SetCell(short sheet, int row, int column, object value, CellType type, int formatIndex, IExcelFormula cellFormula)
        {
            if (!this.IsValidWorkSheet(sheet))
            {
                return false;
            }
            Worksheet worksheet = this._workbook.Sheets[sheet];
            this.SetCellStyle(worksheet, row, column, formatIndex, type);
            this.SetCellValueInternal(worksheet, row, column, formatIndex, type, value);
            this.SetCellFormula(worksheet, row, column, cellFormula);
            return true;
        }

        bool SetCellFormula(Worksheet worksheet, int row, int column, IExcelFormula cellFormula)
        {
            if (cellFormula == null)
            {
                return false;
            }
            string str = (this._workbook.ReferenceStyle == ReferenceStyle.A1) ? cellFormula.Formula : cellFormula.FormulaR1C1;
            if (!string.IsNullOrWhiteSpace(str) && (!this._openFlags.DataOnly() && !string.IsNullOrWhiteSpace(str)))
            {
                if (cellFormula.IsArrayFormula)
                {
                    CalcCellIdentity id = new CalcCellIdentity(row, column);
                    if (worksheet.CalcAxial.Manager.IsIsIntersectantWithArrayFormula(id))
                    {
                        return true;
                    }
                }
                worksheet.SetFormula(row, column, 1, 1, SheetArea.Cells, str, cellFormula.IsArrayFormula);
            }
            return true;
        }

        public bool SetCellFormula(short sheet, int row, int column, IExcelFormula cellFormula)
        {
            if (!this.IsValidWorkSheet(sheet))
            {
                return false;
            }
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if (cellFormula == null)
            {
                return false;
            }
            string str = (this._workbook.ReferenceStyle == ReferenceStyle.A1) ? cellFormula.Formula : cellFormula.FormulaR1C1;
            if (!string.IsNullOrWhiteSpace(str) && (!this._openFlags.DataOnly() && !string.IsNullOrWhiteSpace(str)))
            {
                worksheet.SetFormula(row, column, 1, 1, SheetArea.Cells, str, cellFormula.IsArrayFormula);
            }
            return true;
        }

        public bool SetCellHyperLink(short sheet, int row, int column, IExcelHyperLink hyperLink)
        {
            return true;
        }

        public void SetCellNote(short sheet, int row, int column, bool stickyNote, string note)
        {
        }

        bool SetCellStyle(Worksheet worksheet, int row, int column, int formatIndex, CellType cellType)
        {
            if (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly())
            {
                SheetArea sheetArea = SheetArea.Cells;
                string str = null;
                if (this._builtInStyleNames.TryGetValue(formatIndex, out str))
                {
                    worksheet.SetStyleName(row, column, sheetArea, str);
                }
                else if ((formatIndex > 0) && (formatIndex < this._cellStyleInfos.Count))
                {
                    worksheet.SetStyleInfo(row, column, sheetArea, this._cellStyleInfos[formatIndex]);
                }
            }
            return true;
        }

        public bool SetCellStyle(short sheet, int row, int column, int formatIndex, CellType type)
        {
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && this.IsValidWorkSheet(sheet))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                SheetArea cells = SheetArea.Cells;
                string str = null;
                if (this._builtInStyleNames.TryGetValue(formatIndex, out str))
                {
                    worksheet.SetStyleName(row, column, cells, str);
                }
                else if ((formatIndex > 0) && (formatIndex < this._cellStyleInfos.Count))
                {
                    worksheet.SetStyleInfo(row, column, cells, this._cellStyleInfos[formatIndex]);
                }
            }
            return true;
        }

        public bool SetCellValue(short sheet, int row, int column, object value)
        {
            if ((value != null) && this.IsValidWorkSheet(sheet))
            {
                this._workbook.Sheets[sheet].SetValue(row, column, SheetArea.Cells, this.GetExcelRealUnderlyingCellValue(CellType.Unknown, value));
            }
            return true;
        }

        void SetCellValueInternal(Worksheet workSheet, int row, int column, int formatIndex, CellType type, object value)
        {
            if (value != null)
            {
                double num;
                bool flag = false;
                if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (formatIndex < this._dateTimeFormatter.Count))
                {
                    flag = this._dateTimeFormatter[formatIndex];
                }
                if ((flag && !(value is System.DateTime)) && double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out num))
                {
                    System.DateTime time;
                    if (this._isDate1904)
                    {
                        num += 1462.0;
                    }
                    if (ConverterHelper.FromExcelOADate(num, out time))
                    {
                        value = time;
                    }
                    else
                    {
                        value = (double)num;
                    }
                }
                bool flag2 = false;
                if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (formatIndex < this._textFormatter.Count))
                {
                    flag2 = this._textFormatter[formatIndex];
                }
                if (flag2)
                {
                    workSheet.SetValue(row, column, SheetArea.Cells, (value == null) ? string.Empty : value.ToString());
                }
                else
                {
                    workSheet.SetValue(row, column, SheetArea.Cells, this.GetExcelRealUnderlyingCellValue(type, value));
                }
            }
        }

        public void SetColorPalette(Dictionary<int, GcColor> palette)
        {
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (palette != null))
            {
                ColorHelper.UseCustomPalette = true;
                Dictionary<int, Windows.UI.Color> dictionary = new Dictionary<int, Windows.UI.Color>();
                foreach (KeyValuePair<int, GcColor> pair in palette)
                {
                    dictionary.Add(pair.Key, Windows.UI.Color.FromArgb(pair.Value.A, pair.Value.R, pair.Value.G, pair.Value.B));
                }
                ColorHelper.CustomPalette = dictionary;
            }
        }

        void SetColumnInfo(short sheet, Worksheet worksheet, int column, short formatIndex, double width, bool hidden, byte outlineLevel, bool collapsed)
        {
            if (outlineLevel > 0)
            {
                if ((column + 2) > worksheet.ColumnCount)
                {
                    worksheet.ColumnCount = column + 2;
                }
                int num = 0;
                if (this._sheetColumnDemensions.TryGetValue(worksheet.Name, out num) && (num < (column + 2)))
                {
                    this._sheetColumnDemensions[worksheet.Name] = column + 2;
                }
                worksheet.ColumnRangeGroup.SetLevelInternal(column, outlineLevel);
            }
            if (collapsed)
            {
                if (!this._columnCollapsedInfo.ContainsKey(sheet))
                {
                    this._columnCollapsedInfo.Add(sheet, new Dictionary<int, List<int>>());
                }
                if (!this._columnCollapsedInfo[sheet].ContainsKey(outlineLevel + 1))
                {
                    this._columnCollapsedInfo[sheet].Add(outlineLevel + 1, new List<int>());
                }
                if (this._columnCollapsedInfo[sheet][outlineLevel + 1] == null)
                {
                    this._columnCollapsedInfo[sheet][outlineLevel + 1] = new List<int>();
                }
                this._columnCollapsedInfo[sheet][outlineLevel + 1].Add(column);
            }
            if (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly())
            {
                worksheet.SetColumnWidth(column, SheetArea.Cells, width);
            }
            if ((outlineLevel > 0) && (Math.Abs((double)(width - 0.0)) < 0.01))
            {
                worksheet.SetColumnWidth(column, SheetArea.Cells, worksheet.DefaultColumnWidth);
            }
            if (outlineLevel <= 0)
            {
                worksheet.SetColumnVisible(column, SheetArea.Cells, !hidden);
            }
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (formatIndex != -1))
            {
                string str = null;
                if (this._builtInStyleNames.TryGetValue(formatIndex, out str))
                {
                    worksheet.SetStyleObject(-1, column, SheetArea.Cells, str);
                }
                else if (formatIndex < this._cellStyleInfos.Count)
                {
                    worksheet.SetStyleObject(-1, column, SheetArea.Cells, this._cellStyleInfos[formatIndex]);
                }
            }
        }

        public void SetColumnInfo(short sheet, short columnFirst, short columnLast, short formatIndex, double width, bool hidden, byte outlineLevel, bool collapsed)
        {
            if (this.IsValidWorkSheet(sheet))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                ExcelImportExportGlobalVariables.Instance.MaxColumnCount = Math.Min(0x4000, Math.Max(worksheet.ColumnCount, columnLast + 1));
                double columnWidthInPixel = this.GetColumnWidthInPixel(sheet, width);
                if (columnLast == 0x4000)
                {
                    columnLast--;
                }
                for (int i = columnFirst; i <= columnLast; i++)
                {
                    this.SetColumnInfo(sheet, worksheet, i, formatIndex, columnWidthInPixel, hidden, outlineLevel, collapsed);
                }
            }
        }

        public void SetConditionalFormatting(short sheet, IExcelConditionalFormat format)
        {
            if (((((format != null) && (format.Ranges != null)) && ((format.ConditionalFormattingRules != null) && (format.ConditionalFormattingRules.Count != 0))) && this.IsValidWorkSheet(sheet)) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                if (!this._sheetsCondtionalForats.ContainsKey(worksheet.Name))
                {
                    this._sheetsCondtionalForats[worksheet.Name] = new List<IExcelConditionalFormat>();
                }
                List<IExcelConditionalFormat> list = null;
                if (!this._sheetsCondtionalForats.TryGetValue(worksheet.Name, out list))
                {
                    list = new List<IExcelConditionalFormat>();
                    this._sheetsCondtionalForats[worksheet.Name] = list;
                }
                if (list == null)
                {
                    list = new List<IExcelConditionalFormat>();
                    this._sheetsCondtionalForats[worksheet.Name] = list;
                }
                list.Add(format);
            }
        }

        void SetCustomAutoFilter(RowFilterBase rowFilter, IExcelFilterColumn filterColumn, int columId)
        {
            if (filterColumn.CustomFilters != null)
            {
                IExcelCustomFilters customFilters = filterColumn.CustomFilters;
                ConditionBase filterItem = null;
                if (customFilters.Filter1 != null)
                {
                    IExcelCustomFilter customFilter = customFilters.Filter1;
                    filterItem = this.GetCondition(customFilter);
                }
                if (customFilters.Filter2 != null)
                {
                    RelationCompareType and = RelationCompareType.And;
                    if (!customFilters.And)
                    {
                        and = RelationCompareType.Or;
                    }
                    filterItem = new RelationCondition(and, this.GetCondition(customFilters.Filter1), this.GetCondition(customFilters.Filter2));
                }
                if (filterItem != null)
                {
                    rowFilter.AddFilterItem(columId, filterItem);
                }
            }
        }

        public void SetCustomOrFunctionNameList(List<IFunction> customOrFunctionNames)
        {
        }

        public void SetDefaultColumnWidth(short sheet, double baseColumnWidth, double? defaultColumnWidth)
        {
            if (this.IsValidWorkSheet(sheet) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()))
            {
                if (defaultColumnWidth.HasValue && defaultColumnWidth.HasValue)
                {
                    this._workbook.Sheets[sheet].DefaultColumnWidth = this.GetColumnWidthInPixel(sheet, defaultColumnWidth.Value);
                }
                else
                {
                    this._workbook.Sheets[sheet].DefaultColumnWidth = this.GetDefaultColumnWidthInPixel(sheet, baseColumnWidth);
                }
            }
        }

        public void SetDefaultRowHeight(short sheet, double defaultHeight)
        {
            if (this.IsValidWorkSheet(sheet) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()))
            {
                double num = UnitHelper.PointToPixel(defaultHeight);
                this._workbook.Sheets[sheet].DefaultRowHeight = num;
            }
        }

        public void SetDifferentialFormattingRecord(List<IDifferentialFormatting> dxfList)
        {
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (dxfList != null))
            {
                this._differentFormats = new List<IDifferentialFormatting>(dxfList);
                foreach (IDifferentialFormatting formatting in dxfList)
                {
                    this._differentCellStyles.Add(formatting.ToCellStyleInfo(this._workbook));
                }
            }
        }

        public void SetDimensions(short sheet, int rowFirst, int rowLast, short columnFirst, short columnLast)
        {
            if (this.IsValidWorkSheet(sheet))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                worksheet.RowCount = (rowLast > 0) ? rowLast : 1;
                int num = (columnLast > 0) ? columnLast : 1;
                this._sheetColumnDemensions[worksheet.Name] = num;
                if (columnLast > worksheet.ColumnCount)
                {
                    worksheet.ColumnCount = num;
                }
            }
        }

        public void SetDisplayElements(short sheet, bool showFormula, bool showZeros, bool showGridLine, bool showRowColumnHeader, bool rightToLeftColumns)
        {
            if (this.IsValidWorkSheet(sheet) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                worksheet.ShowGridLine = showGridLine;
                worksheet.ColumnHeader.IsVisible = showRowColumnHeader;
                worksheet.RowHeader.IsVisible = showRowColumnHeader;
            }
        }

        void SetDynamicAutoFilter(RowFilterBase rowFilter, CellRange filterRange, IExcelFilterColumn filterColumn, int columId)
        {
            if (filterColumn.DynamicFilter != null)
            {
                IExcelDynamicFilter dynamicFilter = filterColumn.DynamicFilter;
                ConditionBase filterItem = null;
                switch (dynamicFilter.Type)
                {
                    case ExcelDynamicFilterType.AboveAverage:
                        {
                            CellRange range = new CellRange(filterRange.Row, columId, filterRange.RowCount, 1);
                            filterItem = AverageCondition.Create(AverageConditionType.Above, new CellRange[] { range });
                            break;
                        }
                    case ExcelDynamicFilterType.BelowAverage:
                        {
                            CellRange range2 = new CellRange(filterRange.Row, columId, filterRange.RowCount, 1);
                            filterItem = AverageCondition.Create(AverageConditionType.Below, new CellRange[] { range2 });
                            break;
                        }
                    case ExcelDynamicFilterType.Tomorrow:
                        filterItem = new DateExCondition(DateOccurringType.Tomorrow);
                        break;

                    case ExcelDynamicFilterType.Today:
                        filterItem = new DateExCondition(DateOccurringType.Today);
                        break;

                    case ExcelDynamicFilterType.Yesterday:
                        filterItem = new DateExCondition(DateOccurringType.Yesterday);
                        break;

                    case ExcelDynamicFilterType.NextWeek:
                        filterItem = new DateExCondition(DateOccurringType.NextWeek);
                        break;

                    case ExcelDynamicFilterType.ThisWeek:
                        filterItem = new DateExCondition(DateOccurringType.ThisWeek);
                        break;

                    case ExcelDynamicFilterType.LastWeek:
                        filterItem = new DateExCondition(DateOccurringType.LastWeek);
                        break;

                    case ExcelDynamicFilterType.NextMonth:
                        filterItem = new DateExCondition(DateOccurringType.NextWeek);
                        break;

                    case ExcelDynamicFilterType.ThisMonth:
                        filterItem = new DateExCondition(DateOccurringType.ThisWeek);
                        break;

                    case ExcelDynamicFilterType.LastMonth:
                        filterItem = new DateExCondition(DateOccurringType.LastMonth);
                        break;

                    case ExcelDynamicFilterType.Q1:
                        filterItem = DateExCondition.FromQuarter(QuarterType.Quarter1);
                        break;

                    case ExcelDynamicFilterType.Q2:
                        filterItem = DateExCondition.FromQuarter(QuarterType.Quarter2);
                        break;

                    case ExcelDynamicFilterType.Q3:
                        filterItem = DateExCondition.FromQuarter(QuarterType.Quarter3);
                        break;

                    case ExcelDynamicFilterType.Q4:
                        filterItem = DateExCondition.FromQuarter(QuarterType.Quarter4);
                        break;

                    case ExcelDynamicFilterType.M1:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.January);
                        break;

                    case ExcelDynamicFilterType.M2:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.February);
                        break;

                    case ExcelDynamicFilterType.M3:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.March);
                        break;

                    case ExcelDynamicFilterType.M4:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.April);
                        break;

                    case ExcelDynamicFilterType.M5:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.May);
                        break;

                    case ExcelDynamicFilterType.M6:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.June);
                        break;

                    case ExcelDynamicFilterType.M7:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.July);
                        break;

                    case ExcelDynamicFilterType.M8:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.August);
                        break;

                    case ExcelDynamicFilterType.M9:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.September);
                        break;

                    case ExcelDynamicFilterType.M10:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.October);
                        break;

                    case ExcelDynamicFilterType.M11:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.November);
                        break;

                    case ExcelDynamicFilterType.M12:
                        filterItem = DateExCondition.FromMonth(MonthOfYearType.December);
                        break;
                }
                if (filterItem != null)
                {
                    rowFilter.AddFilterItem(columId, filterItem);
                }
            }
        }

        public void SetExcelCellFormat(IExtendedFormat format)
        {
            StyleInfo styleInfo;
            if (format != null)
            {
                styleInfo = format.ToCellStyleInfo(this._workbook);
                this._cellStyleInfos.Add(styleInfo);
                string formatCode = ExtendedNumberFormatHelper.GetFormatCode(format);
                this._dateTimeFormatter.Add(this.IsDatesOrTimesNumberFormat(formatCode));
                if (formatCode == "@")
                {
                    this._textFormatter.Add(true);
                }
                else
                {
                    this._textFormatter.Add(false);
                }
                if (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly())
                {
                    if (format.IsStyleFormat)
                    {
                        int extendedFormatIndex = this.GetExtendedFormatIndex(format);
                        if (extendedFormatIndex == -1)
                        {
                            extendedFormatIndex = this._normalStleIndex;
                        }
                        if (this._styleNames.Count > extendedFormatIndex)
                        {
                            this._builtInStyleNames.Add(this._builtInStyleNames.Count, this._styleNames[extendedFormatIndex]);
                        }
                        else
                        {
                            this._builtInStyleNames.Add(this._builtInStyleNames.Count, "__builtInStyle" + ((int)this._builtInStyleNames.Count));
                        }
                    }
                    else
                    {
                        string str2 = "";
                        if (format.ParentFormatID.HasValue && (this._builtInStyleNames.Count > format.ParentFormatID.Value))
                        {
                            str2 = this._builtInStyleNames[format.ParentFormatID.Value];
                        }
                        if ((str2 == "Normal") || str2.StartsWith("__builtInStyle"))
                        {
                            str2 = "__builtInStyle" + ((int)this._builtInStyleNames.Count);
                        }
                        if (string.IsNullOrEmpty(str2))
                        {
                            str2 = "__builtInStyle" + ((int)this._builtInStyleNames.Count);
                        }
                        styleInfo.Name = str2;
                        IExtendedFormat format2 = null;
                        if ((!str2.StartsWith("__builtInStyle") && this._namedStylesExtendedFormats.TryGetValue(str2, out format2)) && !format2.HasSameSettingWith(format))
                        {
                            styleInfo.Parent = str2;
                            str2 = "__builtInStyle" + ((int)this._builtInStyleNames.Count);
                            styleInfo.Name = str2;
                        }
                        this._workbook.NamedStyles.Add(styleInfo);
                        this._builtInStyleNames.Add(this._builtInStyleNames.Count, str2);
                    }
                }
            }
        }

        public void SetExcelDefaultCellFormat(IExtendedFormat format)
        {
            if (format != null)
            {
                this._normalStyleInfo = format.ToCellStyleInfo(this._workbook);
                this._defaultExcelExtendedFormat = format;
                if (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly())
                {
                    this._workbook.DefaultStyle = this._normalStyleInfo;
                }
            }
        }

        public void SetExcelSheetTabColor(int sheet, IExcelColor color)
        {
            if (color != null)
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                if (color.ColorType == ExcelColorType.Theme)
                {
                    worksheet.SheetTabThemeColor = color.GetThemeColorName();
                }
                else
                {
                    worksheet.SheetTabColor = ColorHelper.GetRGBColor(this._workbook, color);
                }
            }
        }

        public void SetExcelSparklineGroups(int sheet, List<IExcelSparklineGroup> excelSparklineGroups)
        {
            if ((!this._openFlags.DataFormulaOnly() && !this._openFlags.DataOnly()) && ((excelSparklineGroups != null) && (excelSparklineGroups.Count != 0)))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                foreach (IExcelSparklineGroup group in excelSparklineGroups)
                {
                    if (group.Sparklines.Count > 0)
                    {
                        SparklineSetting setting = new SparklineSetting
                        {
                            RightToLeft = group.RightToLeft,
                            DisplayXAxis = group.ShowXAxis,
                            ShowFirst = group.ShowFirstDifferently,
                            ShowLast = group.ShowLastDifferently,
                            ShowHigh = group.ShowHighestDifferently,
                            ShowLow = group.ShowLowestDifferently,
                            ShowMarkers = group.ShowMarkers,
                            ShowNegative = group.ShowNegativeDifferently,
                            DisplayHidden = group.ShowHidden,
                            LineWeight = (group.LineWeight * 4.0) / 3.0,
                            DisplayEmptyCellsAs = (EmptyValueStyle)group.DisplayEmptyCellAs
                        };
                        if (group.AxisColor != null)
                        {
                            if (group.AxisColor.IsThemeColor)
                            {
                                setting.AxisThemeColor = group.AxisColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.AxisColor = ColorHelper.GetRGBColor(this._workbook, group.AxisColor);
                            }
                        }
                        if (group.FirstColor != null)
                        {
                            if (group.FirstColor.IsThemeColor)
                            {
                                setting.FirstMarkerThemeColor = group.FirstColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.FirstMarkerColor = ColorHelper.GetRGBColor(this._workbook, group.FirstColor);
                            }
                        }
                        if (group.HighColor != null)
                        {
                            if (group.HighColor.IsThemeColor)
                            {
                                setting.HighMarkerThemeColor = group.HighColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.HighMarkerColor = ColorHelper.GetRGBColor(this._workbook, group.HighColor);
                            }
                        }
                        if (group.LastColor != null)
                        {
                            if (group.LastColor.IsThemeColor)
                            {
                                setting.LastMarkerThemeColor = group.LastColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.LastMarkerColor = ColorHelper.GetRGBColor(this._workbook, group.LastColor);
                            }
                        }
                        if (group.LowColor != null)
                        {
                            if (group.LowColor.IsThemeColor)
                            {
                                setting.LowMarkerThemeColor = group.LowColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.LowMarkerColor = ColorHelper.GetRGBColor(this._workbook, group.LowColor);
                            }
                        }
                        if (group.MarkersColor != null)
                        {
                            if (group.MarkersColor.IsThemeColor)
                            {
                                setting.MarkersThemeColor = group.MarkersColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.MarkersColor = ColorHelper.GetRGBColor(this._workbook, group.MarkersColor);
                            }
                        }
                        if (group.NegativeColor != null)
                        {
                            if (group.NegativeColor.IsThemeColor)
                            {
                                setting.NegativeThemeColor = group.NegativeColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.NegativeColor = ColorHelper.GetRGBColor(this._workbook, group.NegativeColor);
                            }
                        }
                        if (group.SeriesColor != null)
                        {
                            if (group.SeriesColor.IsThemeColor)
                            {
                                setting.SeriesThemeColor = group.SeriesColor.GetThemeColorName();
                            }
                            else
                            {
                                setting.SeriesColor = ColorHelper.GetRGBColor(this._workbook, group.SeriesColor);
                            }
                        }
                        setting.MaxAxisType = (SparklineAxisMinMax)group.MaxAxisType;
                        if ((setting.MaxAxisType == SparklineAxisMinMax.Custom) && !double.IsNaN(group.ManualMaxValue))
                        {
                            setting.ManualMax = group.ManualMaxValue;
                        }
                        setting.MinAxisType = (SparklineAxisMinMax)group.MinAxisType;
                        if ((setting.MinAxisType == SparklineAxisMinMax.Custom) && !double.IsNaN(group.ManualMinValue))
                        {
                            setting.ManualMin = group.ManualMinValue;
                        }
                        List<Sparkline> source = new List<Sparkline>();
                        foreach (IExcelSparkline sparkline in group.Sparklines)
                        {
                            if (sparkline.Location.Column >= worksheet.ColumnCount)
                            {
                                worksheet.ColumnCount = sparkline.Location.Column + 1;
                            }
                            int num = 0;
                            if (this._sheetColumnDemensions.TryGetValue(worksheet.Name, out num) && (num < (sparkline.Location.Column + 1)))
                            {
                                this._sheetColumnDemensions[worksheet.Name] = sparkline.Location.Column + 1;
                            }
                            if (sparkline.Location.Row >= worksheet.RowCount)
                            {
                                worksheet.RowCount = sparkline.Location.Row + 1;
                            }
                            if ((sparkline.DataRange.WorkbookName == null) && (sparkline.DataRange.WorksheetName == worksheet.Name))
                            {
                                CellRange dataRange = new CellRange(sparkline.DataRange.Row, sparkline.DataRange.Column, sparkline.DataRange.RowSpan, sparkline.DataRange.ColumnSpan);
                                if (dataRange != null)
                                {
                                    DataOrientation dataOrientation = (dataRange.RowCount == 1) ? DataOrientation.Horizontal : DataOrientation.Vertical;
                                    Sparkline item = null;
                                    if (!group.IsDateAxis)
                                    {
                                        item = worksheet.SetSparkline(sparkline.Location.Row, sparkline.Location.Column, dataRange, dataOrientation, (SparklineType)group.SparklineType, setting);
                                    }
                                    else
                                    {
                                        CellRange dateAxisRange = new CellRange(group.DateAxisRange.Row, group.DateAxisRange.Column, group.DateAxisRange.RowSpan, group.DateAxisRange.ColumnSpan);
                                        item = worksheet.SetSparkline(sparkline.Location.Row, sparkline.Location.Column, dataRange, dataOrientation, (SparklineType)group.SparklineType, dateAxisRange, (dateAxisRange.RowCount == 1) ? DataOrientation.Horizontal : DataOrientation.Vertical, setting);
                                    }
                                    source.Add(item);
                                }
                            }
                        }
                        Sparkline[] sparklines = source.OrderBy<Sparkline, int>(delegate(Sparkline item)
                        {
                            return item.Row;
                        }).OrderBy<Sparkline, int>(delegate(Sparkline item)
                        {
                            return item.Column;
                        }).ToArray<Sparkline>();
                        worksheet.GroupSparkline(sparklines);
                    }
                }
            }
        }

        public void SetExcelStyle(IExcelStyle style)
        {
            string styleName;
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (style != null))
            {
                if (this._initStyle)
                {
                    this.InitExcelDefaultBuiltInStyles();
                    this._initStyle = false;
                }
                styleName = style.Name;
                if ((this._builtInStyles != null) && style.IsBuiltInStyle)
                {
                    foreach (ExcelStyle style2 in this._builtInStyles)
                    {
                        if (style2.BuiltInStyle == (style as ExcelStyle).BuiltInStyle)
                        {
                            styleName = style2.Name;
                            break;
                        }
                    }
                }
                if (this._workbook.NamedStyles.Find(styleName) == null)
                {
                    this._namedStylesExtendedFormats[styleName] = style.Format;
                    this._styles.Add(style);
                    this._extendedFormats.Add(style.Format);
                    this._styleNames.Add(styleName);
                    this.AddStyleToWorkbookNameStyles(new StyleInfo(styleName, "", style.Format.ToCellStyleInfo(this._workbook)));
                }
                else
                {
                    for (int j = 0; j < this._styles.Count; j++)
                    {
                        if (this._styles[j].Name == styleName)
                        {
                            this._styles[j] = style;
                            this._extendedFormats[j] = style.Format;
                            break;
                        }
                    }
                    this._namedStylesExtendedFormats[styleName] = style.Format;
                    this.AddStyleToWorkbookNameStyles(new StyleInfo(styleName, "", style.Format.ToCellStyleInfo(this._workbook)));
                }
            }
        }

        public void SetExcelWorkbookProperty(IExcelWorkbookPropery workbookPropety)
        {
            if (workbookPropety.IsDate1904)
            {
                this._isDate1904 = true;
            }
        }

        public void SetExternalReferencedWorkbookInfo(IExternalWorkbookInfo externWorkbookInfo)
        {
        }

        void SetFilters(RowFilterBase rowFilter, IExcelFilterColumn filterColumn, int columId)
        {
            if (filterColumn.Filters != null)
            {
                IExcelFilters filters = filterColumn.Filters;
                ConditionBase filterItem = null;
                if (filters.Filter != null)
                {
                    foreach (string str in filters.Filter)
                    {
                        filterItem = new TextCondition(TextCompareType.EqualsTo, str, null)
                        {
                            IgnoreBlank = !filters.Blank
                        };
                        rowFilter.AddFilterItem(columId, filterItem);
                        filterItem = null;
                    }
                }
                if (filters.DateGroupItem != null)
                {
                    foreach (IExcelDateGroupItem item in filters.DateGroupItem)
                    {
                        System.DateTime expected = new System.DateTime(item.Year, item.Month, item.Day, item.Hour, item.Minute, item.Second);
                        DateCondition condition = DateCondition.FromDateTime(DateCompareType.EqualsTo, expected);
                        condition.IgnoreBlank = !filters.Blank;
                        rowFilter.AddFilterItem(columId, condition);
                    }
                }
            }
        }

        void SetForegroundBackgroundAutoFilter(RowFilterBase rowFilter, IExcelFilterColumn filterColumn, int columId)
        {
            if (filterColumn.ColorFilter != null)
            {
                IExcelColorFilter colorFilter = filterColumn.ColorFilter;
                if ((colorFilter != null) && (colorFilter.DxfId < this._differentCellStyles.Count))
                {
                    IDifferentialFormatting formatting = this._differentFormats[(int)colorFilter.DxfId];
                    if (colorFilter.CellColor)
                    {
                        rowFilter.AddBackgroundFilter(columId, ColorHelper.GetRGBColor(this._workbook, formatting.Fill.Item2));
                    }
                    else
                    {
                        rowFilter.AddForegroundFilter(columId, ColorHelper.GetRGBColor(this._workbook, formatting.Fill.Item2));
                    }
                }
            }
        }

        public void SetGridlineColor(short sheet, IExcelColor color)
        {
            if ((((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && this.IsValidWorkSheet(sheet)) && (color != null)) && (!color.IsIndexedColor || (color.Value != 0x40)))
            {
                this._workbook.Sheets[sheet].GridLineColor = ColorHelper.GetRGBColor(this._workbook, color);
            }
        }

        void SetIconAutoFilter(IExcelFilterColumn filterColumn)
        {
            IExcelIconFilter iconFilter = filterColumn.IconFilter;
        }

        public bool SetMergeCells(short sheet, int rowStart, int rowEnd, int columnStart, int columnEnd)
        {
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && this.IsValidWorkSheet(sheet))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                if (rowEnd >= worksheet.RowCount)
                {
                    rowEnd = worksheet.RowCount - 1;
                }
                if (columnEnd >= worksheet.ColumnCount)
                {
                    columnEnd = worksheet.ColumnCount - 1;
                }
                worksheet.SpanModel.Add(rowStart, columnStart, (rowEnd - rowStart) + 1, (columnEnd - columnStart) + 1);
            }
            return true;
        }

        void SetNamedCellRange()
        {
            if ((this._namedCellRanges != null) && (this._namedCellRanges.Count != 0))
            {
                this._workbook.Names.Clear();
                foreach (Worksheet worksheet in this._workbook.Sheets)
                {
                    worksheet.Names.Clear();
                }
                bool flag = this._workbook.ReferenceStyle == ReferenceStyle.R1C1;
                this._openFlags.DataFormulaOnly();
                CalcParser parser = new CalcParser();
                this._workbook.SuspendCalcService();
                try
                {
                    foreach (KeyValuePair<int, List<IName>> pair in this._namedCellRanges)
                    {
                        if ((pair.Key == -1) && (pair.Value != null))
                        {
                            foreach (IName name in pair.Value)
                            {
                                try
                                {
                                    SpreadCalcParserContext context = new SpreadCalcParserContext(this._workbook.ActiveSheet, flag, 0, 0, null);
                                    CalcExpression expression = parser.Parse(flag ? name.RefersToR1C1 : name.RefersTo, context);
                                    this._workbook.SetCustomNameInternal(name.Name, 0, 0, expression, false);
                                }
                                catch (Exception exception)
                                {
                                    this._workbook.SetCustomNameInternal(name.Name, 0, 0, new CalcErrorExpression(CalcErrors.Reference), false);
                                    this.OnExcelLoadError(new ExcelWarning(string.Format(ResourceStrings.ExcelSetGlobalNameError, new object[] { name.Name }), ExcelWarningCode.DefinedOrCustomNameError, -1, -1, -1, exception));
                                }
                            }
                        }
                        else if (pair.Value != null)
                        {
                            Worksheet worksheet2 = this._workbook.Sheets[pair.Key];
                            foreach (IName name2 in pair.Value)
                            {
                                try
                                {
                                    SpreadCalcParserContext context2 = new SpreadCalcParserContext(worksheet2, flag, 0, 0, null);
                                    CalcExpression expression2 = parser.Parse(flag ? name2.RefersToR1C1 : name2.RefersTo, context2);
                                    worksheet2.SetCustomNameInternal(name2.Name, 0, 0, expression2, false);
                                }
                                catch (Exception exception2)
                                {
                                    worksheet2.SetCustomNameInternal(name2.Name, 0, 0, new CalcErrorExpression(CalcErrors.Reference), false);
                                    this.OnExcelLoadError(new ExcelWarning(string.Format(ResourceStrings.ExcelSetNamedCellRangeError, new object[] { name2.Name, (int)name2.Index }), ExcelWarningCode.DefinedOrCustomNameError, -1, -1, -1, exception2));
                                }
                            }
                        }
                    }
                }
                finally
                {
                    this._workbook.ResumeCalcService();
                }
            }
        }

        public void SetNamedCellRange(IName namedCellRange)
        {
            if ((!this._openFlags.DataOnly() && (namedCellRange != null)) && (!string.IsNullOrWhiteSpace(namedCellRange.Name) && !namedCellRange.Name.ToUpperInvariant().StartsWith("_XLNM.PRINT_")))
            {
                if (!this._namedCellRanges.ContainsKey(namedCellRange.Index))
                {
                    this._namedCellRanges.Add(namedCellRange.Index, new List<IName>());
                }
                this._namedCellRanges[namedCellRange.Index].Add(namedCellRange);
            }
        }

        public void SetOutlineDirection(int sheet, bool summaryColumnsRightToDetail, bool summaryRowsBelowDetail)
        {
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && this.IsValidWorkSheet(sheet))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                if (summaryColumnsRightToDetail)
                {
                    worksheet.ColumnRangeGroup.Direction = RangeGroupDirection.Forward;
                }
                else
                {
                    worksheet.ColumnRangeGroup.Direction = RangeGroupDirection.Backward;
                }
                if (summaryRowsBelowDetail)
                {
                    worksheet.RowRangeGroup.Direction = RangeGroupDirection.Forward;
                }
                else
                {
                    worksheet.RowRangeGroup.Direction = RangeGroupDirection.Backward;
                }
            }
        }

        public void SetPane(short sheet, int horizontalPosition, int verticalPosition, int topVisibleRow, int leftmostVisibleColumn, int paneIndex, bool isPanesFrozen)
        {
            if (this.IsValidWorkSheet(sheet) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                if (!isPanesFrozen)
                {
                    int num = 0;
                    int num2 = 0;
                    if (worksheet.RowCount <= topVisibleRow)
                    {
                        worksheet.RowCount = topVisibleRow + 1;
                    }
                    if (worksheet.ColumnCount <= leftmostVisibleColumn)
                    {
                        worksheet.ColumnCount = leftmostVisibleColumn + 1;
                    }
                    int num3 = 0;
                    if (this._sheetColumnDemensions.TryGetValue(worksheet.Name, out num3) && (num3 < (leftmostVisibleColumn + 1)))
                    {
                        this._sheetColumnDemensions[worksheet.Name] = leftmostVisibleColumn + 1;
                    }
                    int num4 = 0x1a;
                    int num5 = 20;
                    float zoomFactor = this._workbook.Sheets[sheet].ZoomFactor;
                    if (horizontalPosition > 0)
                    {
                        num = (int)(UnitHelper.PointToPixel((double)(((float)(horizontalPosition / 20)) / zoomFactor)) - num4);
                    }
                    if (verticalPosition > 0)
                    {
                        num2 = (int)(UnitHelper.PointToPixel((double)(((float)(verticalPosition / 20)) / zoomFactor)) - num5);
                    }
                    int columnViewportCount = 1;
                    int rowViewportCount = 1;
                    if (horizontalPosition > 0)
                    {
                        columnViewportCount++;
                    }
                    if (verticalPosition > 0)
                    {
                        rowViewportCount++;
                    }
                    ViewportInfo info2 = new ViewportInfo(worksheet, rowViewportCount, columnViewportCount);
                    switch (paneIndex)
                    {
                        case 0:
                            info2.ActiveColumnViewport = 1;
                            info2.ActiveRowViewport = 1;
                            break;

                        case 1:
                            info2.ActiveColumnViewport = 1;
                            info2.ActiveRowViewport = 0;
                            break;

                        case 2:
                            info2.ActiveColumnViewport = 0;
                            info2.ActiveRowViewport = 1;
                            break;

                        case 3:
                            info2.ActiveColumnViewport = 0;
                            info2.ActiveRowViewport = 0;
                            break;
                    }
                    if (horizontalPosition > 0)
                    {
                        info2.ViewportWidth[0] = num;
                        info2.LeftColumns[1] = leftmostVisibleColumn;
                    }
                    if (verticalPosition > 0)
                    {
                        info2.ViewportHeight[0] = num2;
                        info2.TopRows[1] = topVisibleRow;
                    }
                    worksheet.SetViewportInfo(info2);
                }
                else
                {
                    if (worksheet.RowCount <= verticalPosition)
                    {
                        worksheet.RowCount = verticalPosition + 1;
                    }
                    if (worksheet.ColumnCount <= horizontalPosition)
                    {
                        worksheet.ColumnCount = horizontalPosition + 1;
                    }
                    worksheet.FrozenColumnCount = horizontalPosition;
                    worksheet.FrozenRowCount = verticalPosition;
                    ViewportInfo viewportInfo = worksheet.GetViewportInfo();
                    switch (paneIndex)
                    {
                        case 0:
                            viewportInfo.ActiveColumnViewport = 1;
                            viewportInfo.ActiveRowViewport = 1;
                            break;

                        case 1:
                            viewportInfo.ActiveColumnViewport = 1;
                            viewportInfo.ActiveRowViewport = 0;
                            break;

                        case 2:
                            viewportInfo.ActiveColumnViewport = 0;
                            viewportInfo.ActiveRowViewport = 1;
                            break;

                        case 3:
                            viewportInfo.ActiveColumnViewport = 0;
                            viewportInfo.ActiveRowViewport = 0;
                            break;
                    }
                    worksheet.SetViewportInfo(viewportInfo);
                }
            }
        }

        public void SetPrintArea(int sheet, string printArea)
        {
        }

        public void SetPrintOption(short sheet, IExcelPrintOptions printOption)
        {
        }

        public void SetPrintPageMargin(short sheet, IExcelPrintPageMargin printMargin)
        {
        }

        public void SetPrintPageSetting(short sheet, IExcelPrintPageSetting pageSetting)
        {
        }

        public void SetPrintTitles(int sheet, string title)
        {
        }

        public void SetProtect(short sheet, bool isProtect)
        {
            if ((sheet != -1) && this.IsValidWorkSheet(sheet))
            {
                this._workbook.Sheets[sheet].Protect = isProtect;
            }
        }

        void SetRowColumnGroupCollapsedInfo()
        {
            foreach (KeyValuePair<int, Dictionary<int, List<int>>> pair in this._rowCollaspedInfo)
            {
                if (pair.Key < this._workbook.SheetCount)
                {
                    Worksheet worksheet = this._workbook.Sheets[pair.Key];
                    if ((pair.Value != null) && (pair.Value.Keys.Count > 0))
                    {
                        foreach (int num in pair.Value.Keys)
                        {
                            if ((pair.Value[num] != null) && (pair.Value[num].Count > 0))
                            {
                                foreach (int num2 in pair.Value[num])
                                {
                                    worksheet.RowRangeGroup.SetCollapsed(num2, true);
                                }
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<int, Dictionary<int, List<int>>> pair2 in this._columnCollapsedInfo)
            {
                if (pair2.Key < this._workbook.SheetCount)
                {
                    Worksheet worksheet2 = this._workbook.Sheets[pair2.Key];
                    if ((pair2.Value != null) && (pair2.Value.Keys.Count > 0))
                    {
                        foreach (int num3 in pair2.Value.Keys)
                        {
                            if ((pair2.Value[num3] != null) && (pair2.Value[num3].Count > 0))
                            {
                                foreach (int num4 in pair2.Value[num3])
                                {
                                    worksheet2.ColumnRangeGroup.SetCollapsed(num4, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetRowColumnGutters(short sheet, short rowGutter, short columnGutter, short rowMaxOutlineLevel, short columnMaxOutlineLevel)
        {
        }

        void SetRowColumnHeaders()
        {
            double defaultColumnHeaderRowHeight = this.CalculateDefaultRowHeight();
            foreach (Worksheet worksheet in this._workbook.Sheets)
            {
                worksheet.ColumnHeader.DefaultRowHeight = Math.Max(worksheet.ColumnHeader.DefaultRowHeight, defaultColumnHeaderRowHeight);
                worksheet.ColumnHeaderDefaultStyle.FontSize = this._workbook.DefaultStyle.FontSize;
                worksheet.RowHeaderDefaultStyle.FontSize = this._workbook.DefaultStyle.FontSize;
            }
            for (int k = 0; k < this._workbook.SheetCount; k++)
            {
                Worksheet worksheet = this._workbook.Sheets[k];
                int frozenRowCount = worksheet.FrozenRowCount;
                int frozenColumnCount = worksheet.FrozenColumnCount;
                int rowShift = 0;
                int columnShift = 0;
                if (this._openFlags.RowHeaders())
                {
                    columnShift = worksheet.FrozenColumnCount;
                }
                if (this._openFlags.ColumnHeaders())
                {
                    rowShift = worksheet.FrozenRowCount;
                }
                if (this._openFlags.RowHeaders() && (frozenColumnCount > 0))
                {
                    worksheet.RowHeader.ColumnCount = frozenColumnCount;
                    for (int m = rowShift; m < worksheet.RowCount; m++)
                    {
                        for (int n = 0; n < frozenColumnCount; n++)
                        {
                            worksheet.RowHeader.Columns[n].Width = worksheet.Columns[n].Width;
                            worksheet.RowHeader.Columns[n].IsVisible = worksheet.Columns[n].IsVisible;
                            Cell cell = worksheet.Cells[m, n];
                            worksheet.RowHeader.Cells[m, n].Value = cell.Value;
                            if (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly())
                            {
                                worksheet.SetStyleName(m, n, SheetArea.CornerHeader | SheetArea.RowHeader, worksheet.GetStyleName(m, n));
                                worksheet.RowHeader.Cells[m, n].RowSpan = cell.RowSpan;
                                worksheet.RowHeader.Cells[m, n].ColumnSpan = Math.Min(cell.ColumnSpan, worksheet.RowHeader.ColumnCount - n);
                            }
                        }
                    }
                }
                if (this._openFlags.ColumnHeaders() && (frozenRowCount > 0))
                {
                    worksheet.ColumnHeader.RowCount = frozenRowCount;
                    for (int i = columnShift; i < worksheet.ColumnCount; i++)
                    {
                        for (int j = 0; j < frozenRowCount; j++)
                        {
                            worksheet.ColumnHeader.Rows[j].Height = worksheet.Rows[j].Height;
                            worksheet.ColumnHeader.Rows[j].IsVisible = worksheet.Rows[j].IsVisible;
                            Cell cell = worksheet.Cells[j, i];
                            worksheet.ColumnHeader.Cells[j, i].Value = cell.Value;
                            if (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly())
                            {
                                worksheet.SetStyleName(j, i, SheetArea.ColumnHeader, worksheet.GetStyleName(j, i));
                                worksheet.ColumnHeader.Cells[j, i].RowSpan = Math.Min(cell.RowSpan, worksheet.ColumnHeader.RowCount - j);
                                worksheet.ColumnHeader.Cells[j, i].ColumnSpan = cell.ColumnSpan;
                            }
                        }
                    }
                }
                if (this._openFlags.RowHeaders())
                {
                    int num2 = worksheet.FrozenColumnCount;
                    ViewportInfo viewportInfo = worksheet.GetViewportInfo();
                    viewportInfo.LeftColumns[0] -= num2;
                    worksheet.SetViewportInfo(viewportInfo);
                }
                if (this._openFlags.ColumnHeaders())
                {
                    int num3 = worksheet.FrozenRowCount;
                    ViewportInfo info2 = worksheet.GetViewportInfo();
                    info2.TopRows[0] -= num3;
                    worksheet.SetViewportInfo(info2);
                }
                if (rowShift > 0)
                {
                    worksheet.RemoveRows(0, rowShift);
                }
                if (columnShift > 0)
                {
                    worksheet.RemoveColumns(0, columnShift);
                }
                if (this._openFlags.RowHeaders())
                {
                    worksheet.FrozenColumnCount = 0;
                }
                if (this._openFlags.ColumnHeaders())
                {
                    worksheet.FrozenRowCount = 0;
                }
            }
        }

        public bool SetRowInfo(short sheet, int row, int firstDefinedColumn, int lastDefinedColumn, short formatIndex, double height, byte outlineLevel, bool collapsed, bool zeroHeight, bool unSynced, bool ghostDirty)
        {
            if (!this.IsValidWorkSheet(sheet))
            {
                return false;
            }
            Worksheet worksheet = this._workbook.Sheets[sheet];
            if (outlineLevel > 0)
            {
                if ((row + 2) > worksheet.RowCount)
                {
                    worksheet.RowCount = row + 2;
                }
                worksheet.RowRangeGroup.SetLevelInternal(row, outlineLevel);
            }
            else if ((row + 1) > worksheet.RowCount)
            {
                worksheet.RowCount = row + 1;
            }
            if (((ghostDirty && (formatIndex != -1)) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly())) && (row < worksheet.RowCount))
            {
                string str = null;
                if (this._builtInStyleNames.TryGetValue(formatIndex, out str))
                {
                    worksheet.SetStyleObject(row, -1, SheetArea.Cells, str);
                }
                else
                {
                    worksheet.SetStyleObject(row, -1, SheetArea.Cells, this._cellStyleInfos[formatIndex]);
                }
            }
            if (collapsed)
            {
                if (!this._rowCollaspedInfo.ContainsKey(sheet))
                {
                    this._rowCollaspedInfo.Add(sheet, new Dictionary<int, List<int>>());
                }
                if (!this._rowCollaspedInfo[sheet].ContainsKey(outlineLevel + 1))
                {
                    this._rowCollaspedInfo[sheet].Add(outlineLevel + 1, new List<int>());
                }
                if (this._rowCollaspedInfo[sheet][outlineLevel + 1] == null)
                {
                    this._rowCollaspedInfo[sheet][outlineLevel + 1] = new List<int>();
                }
                this._rowCollaspedInfo[sheet][outlineLevel + 1].Add(row);
            }
            if (!double.IsNaN(height))
            {
                worksheet.SetRowHeight(row, SheetArea.Cells, UnitHelper.PointToPixel(height));
            }
            if (outlineLevel == 0)
            {
                worksheet.SetRowVisible(row, SheetArea.Cells, !zeroHeight);
            }
            return true;
        }

        public void SetScroll(bool showHorizontalScrollbarAsNeeded, bool showVerticalScrollBarAsNeeded)
        {
            this._workbook.HorizontalScrollBarVisibility = showHorizontalScrollbarAsNeeded ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            this._workbook.VerticalScrollBarVisibility = showVerticalScrollBarAsNeeded ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
        }

        public void SetSelection(short sheet, PaneType paneType, int rowActive, int columnActive, int selectionCount, List<int> rowFirst, List<int> rowLast, List<int> colFirst, List<int> colLast)
        {
            if ((this.IsValidWorkSheet(sheet) && (((rowFirst != null) && (rowLast != null)) && ((colFirst != null) && (colLast != null)))) && !this._openFlags.DataOnly())
            {
                if (this._openFlags.DataFormulaOnly())
                {
                    this._cachedActivePosition[sheet] = new Tuple<int, int>(rowActive, columnActive);
                }
                else
                {
                    Worksheet worksheet = this._workbook.Sheets[sheet];
                    PaneType activePane = this.GetActivePane(sheet);
                    if (paneType == activePane)
                    {
                        int count = rowFirst.Count;
                        if (worksheet.RowCount <= rowActive)
                        {
                            worksheet.RowCount = rowActive + 1;
                        }
                        if (worksheet.ColumnCount <= columnActive)
                        {
                            worksheet.ColumnCount = columnActive + 1;
                        }
                        int num2 = 0;
                        if (this._sheetColumnDemensions.TryGetValue(worksheet.Name, out num2) && (num2 < (columnActive + 1)))
                        {
                            this._sheetColumnDemensions[worksheet.Name] = columnActive + 1;
                        }
                        if (count > 1)
                        {
                            worksheet.SelectionPolicy = SelectionPolicy.MultiRange;
                        }
                        for (int i = 0; i < count; i++)
                        {
                            int num4 = rowFirst[i];
                            int num5 = rowLast[i];
                            int num6 = colFirst[i];
                            int num7 = colLast[i];
                            if (num5 == -1)
                            {
                                num5 = worksheet.RowCount - 1;
                            }
                            if (num7 == -1)
                            {
                                num7 = worksheet.ColumnCount - 1;
                            }
                            num4 = (num4 >= 0) ? num4 : 0;
                            num5 = (num5 >= 0) ? num5 : 0;
                            num6 = (num6 >= 0) ? num6 : 0;
                            num7 = (num7 >= 0) ? num7 : 0;
                            if ((num4 < worksheet.RowCount) && (num6 < worksheet.ColumnCount))
                            {
                                int rowCount = Math.Min((int)(worksheet.RowCount - num4), (int)((num5 - num4) + 1));
                                int columnCount = worksheet.ColumnCount;
                                this._sheetColumnDemensions.TryGetValue(worksheet.Name, out columnCount);
                                int num10 = Math.Min((int)(Math.Min(columnCount, worksheet.ColumnCount) - num6), (int)((num7 - num6) + 1));
                                if ((rowCount >= 1) && (num10 >= 1))
                                {
                                    worksheet.Selection.AddSelection(num4, num6, rowCount, num10);
                                }
                            }
                        }
                        int row = ((rowActive >= 0) && (rowActive < worksheet.RowCount)) ? rowActive : 0;
                        int column = ((columnActive >= 0) && (columnActive < worksheet.ColumnCount)) ? columnActive : 0;
                        worksheet.SetActiveCell(row, column);
                        worksheet.Selection.SetAnchor(row, column);
                        worksheet.UpdateActiveViewportByAnchor();
                    }
                }
            }
        }

        public void SetTable(int sheetIndex, IExcelTable table)
        {
            SheetTable sheetTable;
            if ((((table != null) && (table.Range != null)) && this.IsValidWorkSheet(sheetIndex)) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()))
            {
                Worksheet worksheet = this._workbook.Sheets[sheetIndex];
                IRange range = table.Range;
                if ((range.Row + range.RowSpan) > worksheet.RowCount)
                {
                    worksheet.RowCount = range.Row + range.RowSpan;
                }
                if ((range.Column + range.ColumnSpan) > worksheet.ColumnCount)
                {
                    worksheet.ColumnCount = range.Column + range.ColumnSpan;
                }
                int num = 0;
                if (this._sheetColumnDemensions.TryGetValue(worksheet.Name, out num) && ((range.Column + range.ColumnSpan) > num))
                {
                    this._sheetColumnDemensions[worksheet.Name] = range.Column + range.ColumnSpan;
                }
                TableStyle tableStyle = null;
                if (table.TableStyleInfo != null)
                {
                    tableStyle = this.GetTableStyle(table.TableStyleInfo.Name);
                }
                int rowSpan = range.RowSpan;
                int row = range.Row;
                sheetTable = null;
                if (table.ShowTotalsRow)
                {
                    rowSpan--;
                }
                if (!table.ShowHeaderRow)
                {
                    if ((row - 1) >= 0)
                    {
                        row--;
                        rowSpan++;
                        sheetTable = worksheet.AddTable(this.GetUniqueTableName(table.Name), row, range.Column, rowSpan, range.ColumnSpan, tableStyle);
                        sheetTable.ShowHeader = false;
                    }
                    else
                    {
                        rowSpan++;
                        sheetTable = worksheet.AddTable(this.GetUniqueTableName(table.Name), row, range.Column, rowSpan, range.ColumnSpan, tableStyle);
                        sheetTable.ShowHeader = false;
                        worksheet.MoveTable(sheetTable, 0, sheetTable.Range.Column);
                    }
                }
                else
                {
                    sheetTable = worksheet.AddTable(this.GetUniqueTableName(table.Name), row, range.Column, rowSpan, range.ColumnSpan, tableStyle);
                }
                sheetTable.ShowFooter = table.ShowTotalsRow;
                for (int i = 0; i < range.ColumnSpan; i++)
                {
                    IExcelTableColumn column = table.Columns[i];
                    sheetTable.SetColumnName(i, column.Name);
                }
                if (table.TableStyleInfo != null)
                {
                    sheetTable.HighlightFirstColumn = table.TableStyleInfo.ShowFirstColumn;
                    sheetTable.HighlightLastColumn = table.TableStyleInfo.ShowLastColumn;
                    sheetTable.BandedRows = table.TableStyleInfo.ShowRowStripes;
                    sheetTable.BandedColumns = table.TableStyleInfo.ShowColumnStripes;
                }
                CellRange range2 = sheetTable.Range;
                if (((((worksheet.RowFilter != null) && (worksheet.RowFilter.Range.Row == (sheetTable.ShowHeader ? (range2.Row + 1) : range2.Row))) && (worksheet.RowFilter.Range.Column == range2.Column)) && (worksheet.RowFilter.Range.RowCount == (sheetTable.ShowHeader ? (range2.RowCount - 1) : range2.RowCount))) && (worksheet.RowFilter.Range.ColumnCount == range2.ColumnCount))
                {
                    worksheet.RowFilter = null;
                }
                if ((table.AutoFilter != null) && (table.AutoFilter.Range != null))
                {
                    CellRange filterRange = new CellRange(table.AutoFilter.Range.Row + 1, table.AutoFilter.Range.Column, (table.AutoFilter.Range.RowSpan == 1) ? (worksheet.RowCount - table.AutoFilter.Range.Row) : (table.AutoFilter.Range.RowSpan - 1), table.AutoFilter.Range.ColumnSpan);
                    foreach (IExcelFilterColumn column2 in table.AutoFilter.FilterColumns)
                    {
                        int columId = (int)(column2.AutoFilterColumnId + table.Range.Column);
                        this.SetForegroundBackgroundAutoFilter(sheetTable.RowFilter, column2, columId);
                        this.SetIconAutoFilter(column2);
                        this.SetFilters(sheetTable.RowFilter, column2, columId);
                        this.SetTop10AutoFilter(sheetTable.RowFilter, column2, columId);
                        this.SetDynamicAutoFilter(sheetTable.RowFilter, filterRange, column2, columId);
                        this.SetCustomAutoFilter(sheetTable.RowFilter, column2, columId);
                    }
                    sheetTable.RowFilter.Filter();
                }
                else if ((table.AutoFilter == null) || (table.AutoFilter.Range == null))
                {
                    sheetTable.RowFilter.ShowFilterButton = false;
                }
                for (row = range2.Row; row < (range2.Row + range2.RowCount); row++)
                {
                    for (int j = range2.Column; j < (range2.Column + range2.ColumnCount); j++)
                    {
                        this.ResetCellStyleInfoOfTableRangeCell(worksheet, row, j, this._workbook.DefaultStyle);
                    }
                }
            }
        }

        public void SetTableDefaultStyle(string defaultTableStyleName, string defaultPivotTableStyleName)
        {
            this._defaultTableStyleName = defaultTableStyleName;
        }

        public void SetTableStyle(IExcelTableStyle tableStyle)
        {
            if ((((tableStyle != null) && (tableStyle.Name != null)) && tableStyle.IsTableStyle) && (!this._openFlags.DataFormulaOnly() && !this._openFlags.DataOnly()))
            {
                string baseBuiltInStyleName = null;
                this.IsModifiedTableStyle(tableStyle.Name, out baseBuiltInStyleName);
                TableStyle style = null;
                if (baseBuiltInStyleName != null)
                {
                    style = this.GetBuiltItTableStyle(baseBuiltInStyleName).Clone() as TableStyle;
                }
                else
                {
                    style = new TableStyle();
                }
                if (tableStyle.TableStyleElements != null)
                {
                    foreach (IExcelTableStyleElement element in tableStyle.TableStyleElements)
                    {
                        TableStyleInfo info = null;
                        if ((element.DifferentFormattingIndex >= 0) && (element.DifferentFormattingIndex < this._differentFormats.Count))
                        {
                            info = this._differentFormats[element.DifferentFormattingIndex].ToTableStyleInfo(this._workbook);
                        }
                        switch (element.Type)
                        {
                            case ExcelTableElementType.WholeTable:
                                if (info != null)
                                {
                                    style.WholeTableStyle = info;
                                }
                                break;

                            case ExcelTableElementType.HeaderRow:
                                if (info != null)
                                {
                                    style.HeaderRowStyle = info;
                                }
                                break;

                            case ExcelTableElementType.TotalRow:
                                if (info != null)
                                {
                                    style.FooterRowStyle = info;
                                }
                                break;

                            case ExcelTableElementType.FirstColumn:
                                if (info != null)
                                {
                                    style.HighlightFirstColumnStyle = info;
                                }
                                break;

                            case ExcelTableElementType.LastColumn:
                                if (info != null)
                                {
                                    style.HighlightLastColumnStyle = info;
                                }
                                break;

                            case ExcelTableElementType.FirstRowStripe:
                                if (info != null)
                                {
                                    style.FirstRowStripStyle = info;
                                }
                                style.FirstRowStripSize = element.Size;
                                break;

                            case ExcelTableElementType.SecondRowStripe:
                                if (info != null)
                                {
                                    style.SecondRowStripStyle = info;
                                }
                                style.SecondRowStripSize = element.Size;
                                break;

                            case ExcelTableElementType.FirstColumnStripe:
                                if (info != null)
                                {
                                    style.HighlightFirstColumnStyle = info;
                                }
                                style.FirstColumnStripSize = element.Size;
                                break;

                            case ExcelTableElementType.SecondColumnStripe:
                                if (info != null)
                                {
                                    style.SecondColumnStripStyle = info;
                                }
                                style.SecondColumnStripSize = element.Size;
                                break;

                            case ExcelTableElementType.FirstHeaderCell:
                                if (info != null)
                                {
                                    style.FirstHeaderCellStyle = info;
                                }
                                break;

                            case ExcelTableElementType.LastHeaderCell:
                                if (info != null)
                                {
                                    style.LastHeaderCellStyle = info;
                                }
                                break;

                            case ExcelTableElementType.FirstTotalCell:
                                if (info != null)
                                {
                                    style.FirstFooterCellStyle = info;
                                }
                                break;

                            case ExcelTableElementType.LastTotalCell:
                                if (info != null)
                                {
                                    style.LastFooterCellStyle = info;
                                }
                                break;
                        }
                    }
                }
                style.Name = tableStyle.Name;
                this._tableStyleCollection.Add(tableStyle.Name, style);
                TableStyles.AddCustomStyles(style);
            }
        }

        public void SetTabs(bool showTabs, int selectedTabIndex, int firstDisplayedTabIndex, int selectedTabCount, int tabRatio)
        {
            this._startSheetIndex = firstDisplayedTabIndex;
            this._activeSheetIndex = selectedTabIndex;
        }

        public void SetTheme(IExcelTheme theme)
        {
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && (theme != null))
            {
                this._workbook.ResetThemes();
                string name = theme.Name;
                if ((name == null) || name.ToUpperInvariant().StartsWith("OFFICE"))
                {
                    name = "Office";
                }
                ThemeColor colors = new ThemeColor(theme.ColorScheme.Name, ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[1]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[3]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[0]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[2]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[4]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[5]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[6]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[7]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[8]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[9]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[10]), ColorHelper.GetRGBColor(this._workbook, theme.ColorScheme.SchemeColors[11]));
                // hdt
                string headingFontName = (from item in theme.FontScheme.MajorFont.RunFormattings
                                          select item.Typeface).FirstOrDefault();
                string bodyFontName = (from item in theme.FontScheme.MinorFont.RunFormattings
                                       select item.Typeface).FirstOrDefault();
                if (this._workbook.Themes.Contains(name))
                {
                    SpreadTheme theme2 = this._workbook.Themes[name];
                    theme2.Colors = colors;
                    theme2.HeadingFontName = headingFontName;
                    theme2.BodyFontName = bodyFontName;
                    this._workbook.CurrentThemeName = name;
                }
                else
                {
                    SpreadTheme theme3 = new SpreadTheme(name, colors, headingFontName, bodyFontName);
                    this._workbook.Themes.Add(theme3);
                    this._workbook.CurrentThemeName = theme3.Name;
                }
            }
        }

        void SetTop10AutoFilter(RowFilterBase rowFilter, IExcelFilterColumn filterColumn, int columId)
        {
            if (filterColumn.Top10 != null)
            {
                IExcelTop10Filter filter = filterColumn.Top10;
                Top10ConditionType top = Top10ConditionType.Top;
                if (filter.Top)
                {
                    top = Top10ConditionType.Top;
                }
                else
                {
                    top = Top10ConditionType.Bottom;
                }
                Top10Condition filterItem = new Top10Condition
                {
                    IsPercent = filter.Percent,
                    Type = top,
                    ExpectedValue = (double)filter.Value
                };
                rowFilter.AddFilterItem(columId, filterItem);
            }
        }

        public void SetTopLeft(short sheet, int topRow, int leftColumn)
        {
            if (this.IsValidWorkSheet(sheet) && (!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()))
            {
                Worksheet worksheet = this._workbook.Sheets[sheet];
                worksheet.GetViewportInfo().TopRows[0] = (topRow < worksheet.RowCount) ? topRow : (worksheet.RowCount - 1);
                worksheet.GetViewportInfo().LeftColumns[0] = (leftColumn < worksheet.ColumnCount) ? leftColumn : (worksheet.ColumnCount - 1);
            }
        }

        public void SetValidationData(short sheet, IExcelDataValidation dataValidation)
        {
            Worksheet worksheet;
            int baseRow;
            int baseColumn;
            if (((!this._openFlags.DataFormulaOnly() && !this._openFlags.DataOnly()) && this.IsValidWorkSheet(sheet)) && ((dataValidation != null) && (dataValidation.Ranges != null)))
            {
                worksheet = this._workbook.Sheets[sheet];
                baseRow = 0x7fffffff;
                baseColumn = 0x7fffffff;
                foreach (IRange range in dataValidation.Ranges)
                {
                    if (range.Row < baseRow)
                    {
                        baseRow = range.Row;
                    }
                    if (range.Column < baseColumn)
                    {
                        baseColumn = range.Column;
                    }
                }
                using (List<IRange>.Enumerator enumerator2 = dataValidation.Ranges.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        IRange range = enumerator2.Current;
                        if (this.IsEntrieColumn(range))
                        {
                            worksheet.Columns[range.Column].DataValidator = dataValidation.ToDataValidator(range.Row, range.Column);
                        }
                        else
                        {
                            if (this.IsEntireRow(range))
                            {
                                worksheet.Rows[range.Row].DataValidator = dataValidation.ToDataValidator(range.Row, range.Column);
                                continue;
                            }
                            for (int k = 0; (k < range.RowSpan) && ((range.Row + k) < worksheet.RowCount); k++)
                            {
                                for (int i = 0; (i < range.ColumnSpan) && ((range.Column + i) < worksheet.ColumnCount); i++)
                                {
                                    if (worksheet.GetStyleObject(range.Row + k, range.Column + i, SheetArea.Cells) == null)
                                    {
                                        worksheet.SetStyleName(range.Row + k, range.Column + i, "Normal");
                                    }
                                    worksheet.Cells[range.Row + k, range.Column + i].DataValidator = dataValidation.ToDataValidator((k + range.Row) - baseRow, (i + range.Column) - baseColumn);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetWindow(IExcelRect rect, bool hidden, bool iconic)
        {
        }

        public void SetZoom(short sheet, float zoom)
        {
            if ((!this._openFlags.DataOnly() && !this._openFlags.DataFormulaOnly()) && this.IsValidWorkSheet(sheet))
            {
                this._workbook.Sheets[sheet].ZoomFactor = zoom;
            }
        }

        void UpdateWorksheetsStatus()
        {
            StyleInfo info = this._defaultExcelExtendedFormat.ToCellStyleInfo(this._workbook);
            for (int i = 0; i < this._workbook.Sheets.Count; i++)
            {
                Worksheet worksheet = this._workbook.Sheets[i];
                worksheet.DefaultStyle = info;
                worksheet.ColumnRangeGroup.Refresh();
                worksheet.RowRangeGroup.Refresh();
                List<IExcelConditionalFormat> list = null;
                if (this._sheetsCondtionalForats.TryGetValue(worksheet.Name, out list) && (list != null))
                {
                    foreach (IExcelConditionalFormat format in list)
                    {
                        if (((format == null) || (format.Ranges == null)) || ((format.ConditionalFormattingRules == null) || (format.ConditionalFormattingRules.Count == 0)))
                        {
                            break;
                        }
                        List<CellRange> conditionalFormatRanges = this.GetConditionalFormatRanges(format.Ranges);
                        foreach (IExcelConditionalFormatRule rule in format.ConditionalFormattingRules)
                        {
                            if (rule is IExcelColorScaleRule)
                            {
                                this.AddColorScaleConditionalFormat(worksheet, conditionalFormatRanges, rule);
                            }
                            else if (rule is IExcelDataBarRule)
                            {
                                this.AddDataBarConditionalFormat(worksheet, conditionalFormatRanges, rule);
                            }
                            else if (rule is IExcelHighlightingRule)
                            {
                                this.AddHighlightConditionalFormat(worksheet, conditionalFormatRanges, rule);
                            }
                            else if (rule is IExcelIconSetsRule)
                            {
                                this.AddIconSetCondtionalFormat(worksheet, conditionalFormatRanges, rule);
                            }
                            else
                            {
                                this.AddGeneralRuleConditionalFormat(worksheet, conditionalFormatRanges, rule);
                            }
                        }
                    }
                }
                if (!this._openFlags.DoNotRecalculateAfterLoad())
                {
                    worksheet.AutoRecalculation = true;
                    worksheet.ResumeCalcService(false);
                }
                else
                {
                    worksheet.AutoRecalculation = false;
                }
            }
        }

        Dictionary<string, TableStyle> BuiltInTableStyles
        {
            get
            {
                if (this._builtInTableStyles == null)
                {
                    this._builtInTableStyles = new Dictionary<string, TableStyle>();
                    // hdt
                    foreach (PropertyInfo info in typeof(TableStyles).GetTypeInfo().DeclaredProperties)
                    {
                        if (info.Name.ToUpperInvariant() != "CUSTOMSTYLES")
                        {
                            this._builtInTableStyles.Add(info.Name.ToUpperInvariant(), info.GetValue(null, null) as TableStyle);
                        }
                    }
                }
                return this._builtInTableStyles;
            }
        }
    }
}

