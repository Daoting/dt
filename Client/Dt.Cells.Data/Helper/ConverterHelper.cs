#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.Xls;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal static class ConverterHelper
    {
        static string AddFormulaPrefixSymbol(this string formulaExp)
        {
            if (string.IsNullOrWhiteSpace(formulaExp))
            {
                return formulaExp;
            }
            return ("=" + formulaExp);
        }

        static string AddStringPrefixSymbol(this string formulaExp)
        {
            if (!string.IsNullOrWhiteSpace(formulaExp) && (formulaExp.Length > 2))
            {
                return formulaExp.Substring(1, formulaExp.Length - 2);
            }
            return formulaExp;
        }

        static double ConvertToDouble(object value)
        {
            if (value == null)
            {
                return double.NaN;
            }
            double num = 0.0;
            double.TryParse(value.ToString(), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num);
            return num;
        }

        static string ConvertValueToFormulaIfNeeded(string value)
        {
            int num;
            float num2;
            double num3;
            decimal num4;
            if (int.TryParse(value, out num))
            {
                return value;
            }
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
            return ("=" + value);
        }

        static List<ConditionBase> ExpanedRalationCondtion(RelationCondition relationCondtion)
        {
            List<ConditionBase> list = new List<ConditionBase>();
            Queue<ConditionBase> queue = new Queue<ConditionBase>();
            queue.Enqueue(relationCondtion);
            while (queue.Count > 0)
            {
                ConditionBase base2 = queue.Dequeue();
                if (base2 is RelationCondition)
                {
                    RelationCondition condition = base2 as RelationCondition;
                    if (condition.Item1 != null)
                    {
                        queue.Enqueue(condition.Item1);
                    }
                    if (condition.Item2 != null)
                    {
                        queue.Enqueue(condition.Item2);
                    }
                }
                else
                {
                    list.Add(base2);
                }
            }
            return list;
        }

        public static bool FromExcelOADate(double d, out DateTime result)
        {
            if ((d > -657435.0) && (d < 2958466.0))
            {
                result = Dt.Cells.Data.DateTimeExtension.FromOADate(d);
                if ((result.Year == 0x76c) && (result.Month < 3))
                {
                    result = result.AddDays(1.0);
                }
                return true;
            }
            result = DateTime.MinValue;
            return false;
        }

        static ExcelFilterOperator GetCellValueConditonOperator(CellValueCondition cellValueCondtion)
        {
            ExcelFilterOperator none = ExcelFilterOperator.None;
            switch (cellValueCondtion.CompareType)
            {
                case Dt.Cells.Data.GeneralCompareType.EqualsTo:
                    return ExcelFilterOperator.Equal;

                case Dt.Cells.Data.GeneralCompareType.NotEqualsTo:
                    return ExcelFilterOperator.NotEqual;

                case Dt.Cells.Data.GeneralCompareType.GreaterThan:
                    return ExcelFilterOperator.GreaterThan;

                case Dt.Cells.Data.GeneralCompareType.GreaterThanOrEqualsTo:
                    return ExcelFilterOperator.GreaterThanOrEqual;

                case Dt.Cells.Data.GeneralCompareType.LessThan:
                    return ExcelFilterOperator.LessThan;

                case Dt.Cells.Data.GeneralCompareType.LessThanOrEqualsTo:
                    return ExcelFilterOperator.LessThanOrEqual;
            }
            return none;
        }

        static void GetDateExConditionRange(DateOccurringType dateOccurringType, ref double from, ref double to)
        {
            switch (dateOccurringType)
            {
                case DateOccurringType.Today:
                {
                    DateTime time4 = DateTime.Now;
                    from = ToExcelOADate(new DateTime(time4.Year, time4.Month, time4.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time4.Year, time4.Month, time4.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.Yesterday:
                {
                    DateTime time3 = DateTime.Now;
                    TimeSpan span2 = new TimeSpan(1, 0, 0, 0);
                    time3 -= span2;
                    from = ToExcelOADate(new DateTime(time3.Year, time3.Month, time3.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time3.Year, time3.Month, time3.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.Tomorrow:
                {
                    DateTime time5 = DateTime.Now;
                    TimeSpan span3 = new TimeSpan(1, 0, 0, 0);
                    time5 += span3;
                    from = ToExcelOADate(new DateTime(time5.Year, time5.Month, time5.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time5.Year, time5.Month, time5.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.Last7Days:
                {
                    DateTime time = DateTime.Now;
                    TimeSpan span = new TimeSpan(6, 0, 0, 0);
                    DateTime time2 = time - span;
                    from = ToExcelOADate(new DateTime(time2.Year, time2.Month, time2.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time.Year, time.Month, time.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.ThisMonth:
                {
                    DateTime time21 = DateTime.Now;
                    TimeSpan span8 = new TimeSpan(time21.Day - 1, 0, 0, 0);
                    DateTime time22 = time21 - span8;
                    DateTime time24 = time22.AddMonths(1).AddDays(-1.0);
                    from = ToExcelOADate(new DateTime(time22.Year, time22.Month, time22.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time24.Year, time24.Month, time24.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.LastMonth:
                {
                    DateTime time17 = DateTime.Now;
                    TimeSpan span7 = new TimeSpan(time17.Day - 1, 0, 0, 0);
                    DateTime time18 = time17 - span7;
                    DateTime time19 = time18.AddMonths(-1);
                    DateTime time20 = time18.AddDays(-1.0);
                    from = ToExcelOADate(new DateTime(time19.Year, time19.Month, time19.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time20.Year, time20.Month, time20.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.NextMonth:
                {
                    DateTime time25 = DateTime.Now;
                    TimeSpan span9 = new TimeSpan(time25.Day - 1, 0, 0, 0);
                    DateTime time27 = (time25 - span9).AddMonths(1);
                    DateTime time29 = time27.AddMonths(1).AddDays(-1.0);
                    from = ToExcelOADate(new DateTime(time27.Year, time27.Month, time27.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time29.Year, time29.Month, time29.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.ThisWeek:
                {
                    DateTime time10 = DateTime.Now;
                    TimeSpan span5 = new TimeSpan((int) time10.DayOfWeek, 0, 0, 0);
                    DateTime time11 = time10 - span5;
                    DateTime time12 = time11 + new TimeSpan(6, 0, 0, 0);
                    from = ToExcelOADate(new DateTime(time11.Year, time11.Month, time11.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time12.Year, time12.Month, time12.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.LastWeek:
                {
                    DateTime time6 = DateTime.Now;
                    TimeSpan span4 = new TimeSpan((int) time6.DayOfWeek, 0, 0, 0);
                    DateTime time7 = time6 - span4;
                    DateTime time8 = time7 - new TimeSpan(7, 0, 0, 0);
                    DateTime time9 = time8 + new TimeSpan(6, 0, 0, 0);
                    from = ToExcelOADate(new DateTime(time8.Year, time8.Month, time8.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time9.Year, time9.Month, time9.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
                case DateOccurringType.NextWeek:
                {
                    DateTime time13 = DateTime.Now;
                    TimeSpan span6 = new TimeSpan((int) time13.DayOfWeek, 0, 0, 0);
                    DateTime time14 = time13 - span6;
                    DateTime time15 = time14 + new TimeSpan(7, 0, 0, 0);
                    DateTime time16 = time15 + new TimeSpan(6, 0, 0, 0);
                    from = ToExcelOADate(new DateTime(time15.Year, time15.Month, time15.Day, 0, 0, 0));
                    to = ToExcelOADate(new DateTime(time16.Year, time16.Month, time16.Day, 0x17, 0x3b, 0x3b));
                    return;
                }
            }
        }

        internal static IExcelColor GetExcelChartThemeColor(this string themeColor)
        {
            if (string.IsNullOrEmpty(themeColor))
            {
                return ExcelColor.EmptyColor;
            }
            double result = 0.0;
            string[] strArray = themeColor.Split(new char[] { ' ' });
            if ((strArray.Length >= 3) && double.TryParse(strArray[2], out result))
            {
                result /= 100.0;
            }
            return new ExcelColor(ExcelColorType.Theme, themeColor.GetThemeColorIndex(true), result);
        }

        internal static IExcelColor GetExcelThemeColor(this string themeColor)
        {
            if (string.IsNullOrEmpty(themeColor))
            {
                return ExcelColor.EmptyColor;
            }
            double result = 0.0;
            string[] strArray = themeColor.Split(new char[] { ' ' });
            if ((strArray.Length >= 3) && double.TryParse(strArray[2], out result))
            {
                result /= 100.0;
            }
            return new ExcelColor(ExcelColorType.Theme, themeColor.GetThemeColorIndex(false), result);
        }

        static string GetFontFamilyName(IExcelFont font)
        {
            if (font.FontName == null)
            {
                return DefaultStyleCollection.DefaultFontFamily.Source;
            }
            if (string.IsNullOrWhiteSpace(font.FontName.Trim()))
            {
                return DefaultStyleCollection.DefaultFontFamily.Source;
            }
            return font.FontName;
        }

        internal static string GetFontName(this FontFamily fontfamily)
        {
            if ((fontfamily == null) || (fontfamily.Source == null))
            {
                return null;
            }
            string source = fontfamily.Source;
            string listSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            if (source.IndexOf(listSeparator) != -1)
            {
                source = Enumerable.FirstOrDefault<string>(source.Split(new string[] { listSeparator }, (StringSplitOptions) StringSplitOptions.None));
            }
            if (source != null)
            {
                int startIndex = source.Length - 1;
                while (startIndex >= 0)
                {
                    if (source[startIndex] == '/')
                    {
                        break;
                    }
                    startIndex--;
                }
                if (startIndex >= 0)
                {
                    source = source.Substring(startIndex);
                }
                if ((source != null) && source.StartsWith("/#"))
                {
                    source = source.Substring(2);
                }
            }
            return source;
        }

        static string GetFormula(object info)
        {
            if (info is Tuple<object, object, ComparisonOperator>)
            {
                Tuple<object, object, ComparisonOperator> tuple = info as Tuple<object, object, ComparisonOperator>;
                List<string> list = new List<string>();
                if (tuple.Item1 != null)
                {
                    list.Add(GetFormula(tuple.Item1));
                }
                if (tuple.Item2 != null)
                {
                    list.Add(GetFormula(tuple.Item2));
                }
                return string.Join(",", (IEnumerable<string>) list);
            }
            if (info == null)
            {
                return string.Empty;
            }
            string str = info.ToString().RemoveFormulaSymbolIfExists();
            if (info is DateTime)
            {
                str = ((double) ToExcelOADate((DateTime) info)).ToString();
            }
            return str;
        }

        static ExcelFilterOperator GetTextConditonOperator(TextCondition textCondtion)
        {
            switch (textCondtion.CompareType)
            {
                case TextCompareType.EqualsTo:
                case TextCompareType.BeginsWith:
                case TextCompareType.EndsWith:
                case TextCompareType.Contains:
                    return ExcelFilterOperator.Equal;

                case TextCompareType.NotEqualsTo:
                case TextCompareType.DoesNotBeginWith:
                case TextCompareType.DoesNotEndWith:
                case TextCompareType.DoesNotContain:
                    return ExcelFilterOperator.NotEqual;
            }
            return ExcelFilterOperator.Equal;
        }

        static string GetTextCondtionValue(string originalValue, TextCompareType textCompareType)
        {
            switch (textCompareType)
            {
                case TextCompareType.EqualsTo:
                case TextCompareType.NotEqualsTo:
                    return originalValue;

                case TextCompareType.BeginsWith:
                case TextCompareType.DoesNotBeginWith:
                    return (originalValue + "*");

                case TextCompareType.EndsWith:
                case TextCompareType.DoesNotEndWith:
                    return ("*" + originalValue);

                case TextCompareType.Contains:
                case TextCompareType.DoesNotContain:
                    return ("*" + originalValue + "*");
            }
            return originalValue;
        }

        internal static uint GetThemeColorIndex(this string themeColor, bool isChartTheme = false)
        {
            if (themeColor.StartsWith("Background 1"))
            {
                if (isChartTheme)
                {
                    return 0xf3;
                }
                return 0;
            }
            if (themeColor.StartsWith("Text 1"))
            {
                if (isChartTheme)
                {
                    return 0xf1;
                }
                return 1;
            }
            if (themeColor.StartsWith("Background 2"))
            {
                if (isChartTheme)
                {
                    return 0xf4;
                }
                return 2;
            }
            if (themeColor.StartsWith("Text 2"))
            {
                if (isChartTheme)
                {
                    return 0xf2;
                }
                return 3;
            }
            if (themeColor.StartsWith("Accent 1"))
            {
                return 4;
            }
            if (themeColor.StartsWith("Accent 2"))
            {
                return 5;
            }
            if (themeColor.StartsWith("Accent 3"))
            {
                return 6;
            }
            if (themeColor.StartsWith("Accent 4"))
            {
                return 7;
            }
            if (themeColor.StartsWith("Accent 5"))
            {
                return 8;
            }
            if (!themeColor.StartsWith("Accent 6"))
            {
                throw new InvalidOperationException(ResourceStrings.InvalidThemeColor);
            }
            return 9;
        }

        internal static string GetThemeColorName(this IExcelColor color)
        {
            if ((color != null) && (color.ColorType == ExcelColorType.Theme))
            {
                int num = (int)(color.Tint * 100.0);
                string str = ((int) num).ToString();
                if ((color.Value == 0) || (color.Value == 0xf3))
                {
                    return ("Background 1 " + str);
                }
                if ((color.Value == 1) || (color.Value == 0xf1))
                {
                    return ("Text 1 " + str);
                }
                if ((color.Value == 2) || (color.Value == 0xf4))
                {
                    return ("Background 2 " + str);
                }
                if ((color.Value == 3) || (color.Value == 0xf2))
                {
                    return ("Text 2 " + str);
                }
                if (color.Value == 4)
                {
                    return ("Accent 1 " + str);
                }
                if (color.Value == 5)
                {
                    return ("Accent 2 " + str);
                }
                if (color.Value == 6)
                {
                    return ("Accent 3 " + str);
                }
                if (color.Value == 7)
                {
                    return ("Accent 4 " + str);
                }
                if (color.Value == 8)
                {
                    return ("Accent 5 " + str);
                }
                if (color.Value == 9)
                {
                    return ("Accent 6 " + str);
                }
            }
            return null;
        }

        internal static bool HasSameSettingWith(this IExtendedFormat left, IExtendedFormat excelStyle)
        {
            bool? applyProtection = null;
            bool? nullable12 = null;
            if (object.ReferenceEquals(left, excelStyle))
            {
                return true;
            }
            if ((excelStyle == null) || (left == null))
            {
                return false;
            }
            if ((((left.Border.Equals(excelStyle.Border) && left.Font.Equals(excelStyle.Font)) && ((left.FillPattern == excelStyle.FillPattern) && (left.NumberFormatIndex == excelStyle.NumberFormatIndex))) && (((left.HorizontalAlign == excelStyle.HorizontalAlign) && (left.VerticalAlign == excelStyle.VerticalAlign)) && ((left.IsLocked == excelStyle.IsLocked) && (left.Rotation == excelStyle.Rotation)))) && ((((left.IsWordWrap == excelStyle.IsWordWrap) && (left.IsJustfyLastLine == excelStyle.IsJustfyLastLine)) && ((left.IsShrinkToFit == excelStyle.IsShrinkToFit) && (left.IsFirstSymbolApostrophe == excelStyle.IsFirstSymbolApostrophe))) && (((left.ReadingOrder == excelStyle.ReadingOrder) && (left.IsHidden == excelStyle.IsHidden)) && (left.Indent == excelStyle.Indent))))
            {
                bool? applyAlignment = left.ApplyAlignment;
                bool? nullable2 = excelStyle.ApplyAlignment;
                if ((applyAlignment.GetValueOrDefault() == nullable2.GetValueOrDefault()) && (applyAlignment.HasValue == nullable2.HasValue))
                {
                    bool? applyBorder = left.ApplyBorder;
                    bool? nullable4 = excelStyle.ApplyBorder;
                    if ((applyBorder.GetValueOrDefault() == nullable4.GetValueOrDefault()) && (applyBorder.HasValue == nullable4.HasValue))
                    {
                        bool? applyFill = left.ApplyFill;
                        bool? nullable6 = excelStyle.ApplyFill;
                        if ((applyFill.GetValueOrDefault() == nullable6.GetValueOrDefault()) && (applyFill.HasValue == nullable6.HasValue))
                        {
                            bool? applyFont = left.ApplyFont;
                            bool? nullable8 = excelStyle.ApplyFont;
                            if ((applyFont.GetValueOrDefault() == nullable8.GetValueOrDefault()) && (applyFont.HasValue == nullable8.HasValue))
                            {
                                bool? applyNumberFormat = left.ApplyNumberFormat;
                                bool? nullable10 = excelStyle.ApplyNumberFormat;
                                if ((applyNumberFormat.GetValueOrDefault() == nullable10.GetValueOrDefault()) && (applyNumberFormat.HasValue == nullable10.HasValue))
                                {
                                    applyProtection = left.ApplyProtection;
                                    nullable12 = excelStyle.ApplyProtection;
                                }
                            }
                        }
                    }
                }
            }
            bool flag = (applyProtection.GetValueOrDefault() == nullable12.GetValueOrDefault()) && (applyProtection.HasValue == nullable12.HasValue);
            if (!flag)
            {
                return false;
            }
            if ((left.NumberFormat == null) && (excelStyle.NumberFormat != null))
            {
                return false;
            }
            if (left.NumberFormat != null)
            {
                flag = flag && left.NumberFormat.Equals(excelStyle.NumberFormat);
            }
            if ((left.PatternColor == null) && (excelStyle.PatternColor != null))
            {
                return false;
            }
            if ((left.PatternBackgroundColor == null) && (excelStyle.PatternBackgroundColor != null))
            {
                return false;
            }
            if (left.PatternBackgroundColor != null)
            {
                flag = flag && left.PatternBackgroundColor.Equals(excelStyle.PatternBackgroundColor);
            }
            if (left.PatternColor != null)
            {
                flag = flag && left.PatternColor.Equals(excelStyle.PatternColor);
            }
            return flag;
        }

        static bool IsNullOrEmpty(this IExcelBorderSide borderSide)
        {
            return ((borderSide == null) || ((borderSide.LineStyle == ExcelBorderStyle.None) && (borderSide.Color == null)));
        }

        internal static string OffsetFormula(string formula, int rowOffset, int columnOffset)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                return formula;
            }
            if ((rowOffset == 0) && (columnOffset == 0))
            {
                return formula;
            }
            CalcParser parser = new CalcParser();
            CalcParserContext context = new CalcParserContext(false, 0, 0, null);
            return parser.Unparse(parser.Parse(formula, context).Offset(rowOffset, columnOffset, false, true), context);
        }

        internal static string RemoveFormulaSymbolIfExists(this string expression)
        {
            if (!string.IsNullOrWhiteSpace(expression))
            {
                if ((expression.Length >= 2) && (expression[0] == '='))
                {
                    return expression.Substring(1);
                }
                if (expression == "=")
                {
                    return "";
                }
            }
            return expression;
        }

        static void SetStyleInfoBorders(IExcelBorder border, StyleInfo styleInfo, Workbook workbook)
        {
            if (!border.Left.IsNullOrEmpty())
            {
                BorderLine line = border.Left.ToBorderLine(workbook);
                if (line != null)
                {
                    styleInfo.BorderLeft = line;
                }
            }
            if (!border.Right.IsNullOrEmpty())
            {
                BorderLine line2 = border.Right.ToBorderLine(workbook);
                if (line2 != null)
                {
                    styleInfo.BorderRight = line2;
                }
            }
            if (!border.Top.IsNullOrEmpty())
            {
                BorderLine line3 = border.Top.ToBorderLine(workbook);
                if (line3 != null)
                {
                    styleInfo.BorderTop = line3;
                }
            }
            if (!border.Bottom.IsNullOrEmpty())
            {
                BorderLine line4 = border.Bottom.ToBorderLine(workbook);
                if (line4 != null)
                {
                    styleInfo.BorderBottom = line4;
                }
            }
        }

        static void SetStyleInfoFill(FillPatternType fillPattern, IExcelColor patternColor, IExcelColor patternBackgroundColor, StyleInfo styleInfo, Workbook workbook)
        {
            if ((fillPattern == FillPatternType.Solid) && (patternColor != null))
            {
                if (patternColor.IsThemeColor)
                {
                    styleInfo.BackgroundThemeColor = patternColor.GetThemeColorName();
                }
                else if (((!patternColor.IsRGBColor || (patternColor.Value != 0)) && (!patternColor.IsIndexedColor || (patternColor.Value != 0x40))) && (!patternColor.IsIndexedColor || (patternColor.Value != 0x41)))
                {
                    styleInfo.Background = patternColor.ToBrush(workbook);
                }
                else if (patternBackgroundColor != null)
                {
                    if (patternBackgroundColor.IsThemeColor)
                    {
                        styleInfo.BackgroundThemeColor = patternBackgroundColor.GetThemeColorName();
                    }
                    else if (((!patternBackgroundColor.IsRGBColor || (patternBackgroundColor.Value != 0)) && (!patternBackgroundColor.IsIndexedColor || (patternBackgroundColor.Value != 0x40))) && (!patternBackgroundColor.IsIndexedColor || (patternBackgroundColor.Value != 0x41)))
                    {
                        styleInfo.Background = patternBackgroundColor.ToBrush(workbook);
                    }
                }
            }
            else if ((patternBackgroundColor != null) && (((patternBackgroundColor.IsRGBColor && (patternBackgroundColor.Value == 0)) || (patternBackgroundColor.IsIndexedColor && (patternBackgroundColor.Value == 0x40))) || (patternBackgroundColor.IsIndexedColor && (patternBackgroundColor.Value == 0x41))))
            {
                if (patternColor != null)
                {
                    if (patternColor.IsThemeColor)
                    {
                        styleInfo.BackgroundThemeColor = patternColor.GetThemeColorName();
                    }
                    else if (((!patternColor.IsRGBColor || (patternColor.Value != 0)) && (!patternColor.IsIndexedColor || (patternColor.Value != 0x40))) && (!patternColor.IsIndexedColor || (patternColor.Value != 0x41)))
                    {
                        styleInfo.Background = patternColor.ToBrush(workbook);
                    }
                }
            }
            else if (patternBackgroundColor != null)
            {
                if (patternBackgroundColor.IsThemeColor)
                {
                    styleInfo.BackgroundThemeColor = patternBackgroundColor.GetThemeColorName();
                }
                else
                {
                    styleInfo.Background = patternBackgroundColor.ToBrush(workbook);
                }
            }
            else if ((patternBackgroundColor == null) && (patternColor != null))
            {
                if (patternColor.IsThemeColor)
                {
                    styleInfo.BackgroundThemeColor = patternColor.GetThemeColorName();
                }
                else if (((!patternColor.IsRGBColor || (patternColor.Value != 0)) && (!patternColor.IsIndexedColor || (patternColor.Value != 0x40))) && (!patternColor.IsIndexedColor || (patternColor.Value != 0x41)))
                {
                    styleInfo.Background = patternColor.ToBrush(workbook);
                }
            }
        }

        static void SetStyleInfoFont(IExcelFont font, StyleInfo styleInfo, Workbook workbook)
        {
            string fontFamilyName = GetFontFamilyName(font);
            if (!string.IsNullOrWhiteSpace(fontFamilyName))
            {
                styleInfo.FontFamily = new FontFamily(fontFamilyName);
            }
            if (font.FontSize > 0.0)
            {
                styleInfo.FontSize = (float)UnitHelper.PointToPixel(font.FontSize);
            }
            if (font.IsItalic)
            {
                styleInfo.FontStyle = FontStyle.Italic;
            }
            else
            {
                styleInfo.FontStyle = FontStyle.Normal;
            }
            if (font.IsBold)
            {
                styleInfo.FontWeight = FontWeights.Bold;
            }
            else
            {
                styleInfo.FontWeight = FontWeights.Normal;
            }
            if (font.FontColor != null)
            {
                if (font.FontColor.IsThemeColor)
                {
                    styleInfo.ForegroundThemeColor = font.FontColor.GetThemeColorName();
                }
                else
                {
                    styleInfo.Foreground = font.FontColor.ToBrush(workbook);
                }
            }
            styleInfo.Strikethrough = font.IsStrikeOut;
            if (font.UnderLineStyle != UnderLineStyle.None)
            {
                styleInfo.Underline = true;
            }
            if (font.FontScheme == FontSchemeCategory.Major)
            {
                styleInfo.FontTheme = "Headings";
            }
            else if (font.FontScheme == FontSchemeCategory.Minor)
            {
                styleInfo.FontTheme = "Body";
            }
        }

        static BorderLine ToBorderLine(this IExcelBorderSide borderSide, Workbook workbook)
        {
            if (borderSide.LineStyle == ExcelBorderStyle.None)
            {
                return null;
            }
            if (borderSide.Color != null)
            {
                if (borderSide.Color.IsThemeColor)
                {
                    return new BorderLine(borderSide.Color.GetThemeColorName(), (BorderLineStyle) borderSide.LineStyle);
                }
                return new BorderLine(Dt.Cells.Data.ColorHelper.GetRGBColor(workbook, borderSide.Color), (BorderLineStyle) borderSide.LineStyle);
            }
            return new BorderLine(Colors.Transparent, (BorderLineStyle) borderSide.LineStyle);
        }

        internal static Brush ToBrush(this IExcelColor excelColor, Workbook workbook)
        {
            if (excelColor == null)
                return null;
            return new SolidColorBrush(Dt.Cells.Data.ColorHelper.GetRGBColor(workbook, excelColor));
        }

        internal static StyleInfo ToCellStyleInfo(this IDifferentialFormatting dxf, Workbook workbook)
        {
            if (dxf == null)
            {
                return null;
            }
            StyleInfo result = new StyleInfo();
            GeneralFormatter formatter = null;
            if ((dxf.NumberFormat != null) && (dxf.NumberFormat.NumberFormatCode != null))
            {
                result.Formatter = new GeneralFormatter(dxf.NumberFormat.NumberFormatCode);
            }
            if ((formatter != null) && (formatter.FormatString != dxf.NumberFormat.NumberFormatCode))
            {
                ExtendedNumberFormatHelper.UpdateFormatCodeTable(formatter.FormatString, dxf.NumberFormat.NumberFormatCode);
            }
            if (dxf.Font != null)
            {
                SetStyleInfoFont(dxf.Font, result, workbook);
                if ((result.FontFamily != null) && (result.FontFamily.Source == DefaultStyleCollection.DefaultFontFamily.Source))
                {
                    result.ResetFontFamily();
                }
            }
            if (dxf.Alignment != null)
            {
                result.TextIndent = dxf.Alignment.IndentationLevel;
                if (result.TextIndent > 0)
                {
                    result.TextIndent *= 10;
                }
                result.WordWrap = dxf.Alignment.IsTextWrapped;
                result.HorizontalAlignment = dxf.Alignment.HorizontalAlignment.ToHorizontalAlignment();
                result.VerticalAlignment = dxf.Alignment.VerticalAlignment.ToVerticalAlignment();
            }
            if (dxf.Border != null)
            {
                SetStyleInfoBorders(dxf.Border, result, workbook);
            }
            if (dxf.Fill != null)
            {
                SetStyleInfoFill(dxf.Fill.Item1, dxf.Fill.Item2, dxf.Fill.Item3, result, workbook);
            }
            return result;
        }

        internal static StyleInfo ToCellStyleInfo(this IExtendedFormat format, Workbook workbook)
        {
            StyleInfo result = null;
            if (format == null)
            {
                result = null;
            }
            result = new StyleInfo();
            if ((!format.IsStyleFormat || (format.IsStyleFormat && (!format.ApplyFont.HasValue || format.ApplyFont.Value))) && ((format.Font != null) && (format.Font != null)))
            {
                SetStyleInfoFont(format.Font, result, workbook);
            }
            if (!format.IsStyleFormat || (format.IsStyleFormat && (!format.ApplyProtection.HasValue || format.ApplyProtection.Value)))
            {
                result.Locked = format.IsLocked;
            }
            if (!format.IsStyleFormat || (format.IsStyleFormat && (!format.ApplyAlignment.HasValue || format.ApplyAlignment.Value)))
            {
                result.TextIndent = format.Indent;
                if (result.TextIndent > 0)
                {
                    result.TextIndent *= 10;
                }
                result.WordWrap = format.IsWordWrap;
                result.VerticalAlignment = format.VerticalAlign.ToVerticalAlignment();
                result.HorizontalAlignment = format.HorizontalAlign.ToHorizontalAlignment();
            }
            if (format.Border != null)
            {
                IExcelBorder border = format.Border;
                if ((format.IsStyleFormat && format.ApplyBorder.HasValue) && !format.ApplyBorder.Value)
                {
                    border = null;
                }
                if (border != null)
                {
                    SetStyleInfoBorders(border, result, workbook);
                }
            }
            if ((format.FillPattern != FillPatternType.None) && (!format.IsStyleFormat || (format.IsStyleFormat && (!format.ApplyFill.HasValue || format.ApplyFill.Value))))
            {
                SetStyleInfoFill(format.FillPattern, format.PatternColor, format.PatternBackgroundColor, result, workbook);
            }
            if (format.IsShrinkToFit)
            {
                result.ShrinkToFit = true;
            }
            string formatCode = ExtendedNumberFormatHelper.GetFormatCode(format);
            IFormatter formatter = null;
            if (formatCode == "@")
            {
                formatter = new GeneralFormatter(formatCode);
            }
            else
            {
                formatter = new AutoFormatter(new GeneralFormatter(formatCode));
            }
            if (formatter.FormatString != formatCode)
            {
                ExtendedNumberFormatHelper.UpdateFormatCodeTable(formatter.FormatString, formatCode);
            }
            if (!format.IsStyleFormat || (format.IsStyleFormat && (!format.ApplyNumberFormat.HasValue || format.ApplyNumberFormat.Value)))
            {
                result.Formatter = formatter;
            }
            return result;
        }

        internal static ComparisonOperator ToComparisonOperator(this ExcelDataValidationOperator op)
        {
            return (ComparisonOperator)((int)op + 1);
        }

        internal static ComparisonOperator ToComparisonOperator(this DateCompareType compare)
        {
            switch (compare)
            {
                case DateCompareType.EqualsTo:
                    return ComparisonOperator.EqualTo;

                case DateCompareType.NotEqualsTo:
                    return ComparisonOperator.NotEqualTo;

                case DateCompareType.Before:
                    return ComparisonOperator.LessThan;

                case DateCompareType.BeforeEqualsTo:
                    return ComparisonOperator.LessThanOrEqualTo;

                case DateCompareType.After:
                    return ComparisonOperator.GreaterThan;

                case DateCompareType.AfterEqualsTo:
                    return ComparisonOperator.GreaterThanOrEqualTo;
            }
            return ComparisonOperator.EqualTo;
        }

        internal static ComparisonOperator ToComparisonOperator(this Dt.Cells.Data.GeneralCompareType compare)
        {
            switch (compare)
            {
                case Dt.Cells.Data.GeneralCompareType.EqualsTo:
                    return ComparisonOperator.EqualTo;

                case Dt.Cells.Data.GeneralCompareType.NotEqualsTo:
                    return ComparisonOperator.NotEqualTo;

                case Dt.Cells.Data.GeneralCompareType.GreaterThan:
                    return ComparisonOperator.GreaterThan;

                case Dt.Cells.Data.GeneralCompareType.GreaterThanOrEqualsTo:
                    return ComparisonOperator.GreaterThanOrEqualTo;

                case Dt.Cells.Data.GeneralCompareType.LessThan:
                    return ComparisonOperator.LessThan;

                case Dt.Cells.Data.GeneralCompareType.LessThanOrEqualsTo:
                    return ComparisonOperator.LessThanOrEqualTo;
            }
            return ComparisonOperator.EqualTo;
        }

        internal static CriteriaType ToCriteriaType(this ExcelDataValidationType validationType)
        {
            return (CriteriaType) validationType;
        }

        internal static DataValidator ToDataValidator(this IExcelDataValidation dataValidation, int rowOffset, int columnOffset)
        {
            if (dataValidation == null)
            {
                return null;
            }
            CriteriaType type = dataValidation.Type.ToCriteriaType();
            ErrorStyle style = dataValidation.ErrorStyle.ToErrorStyle();
            ComparisonOperator typeOperator = dataValidation.CompareOperator.ToComparisonOperator();
            string formulaExp = OffsetFormula(dataValidation.FirstFormula, rowOffset, columnOffset);
            string str2 = OffsetFormula(dataValidation.SecondFormula, rowOffset, columnOffset);
            DataValidator validator = null;
            switch (type)
            {
                case CriteriaType.AnyValue:
                    validator = new DataValidator();
                    break;

                case CriteriaType.WholeNumber:
                    validator = DataValidator.CreateNumberValidator(typeOperator, formulaExp.AddFormulaPrefixSymbol(), str2.AddFormulaPrefixSymbol(), true);
                    break;

                case CriteriaType.DecimalValues:
                    validator = DataValidator.CreateNumberValidator(typeOperator, formulaExp.AddFormulaPrefixSymbol(), str2.AddFormulaPrefixSymbol(), false);
                    break;

                case CriteriaType.List:
                {
                    string str3 = formulaExp;
                    if (str3 == null)
                    {
                        str3 = str2;
                    }
                    if (str3.StartsWith("\"") && str3.EndsWith("\""))
                    {
                        validator = DataValidator.CreateListValidator(str3.AddStringPrefixSymbol());
                    }
                    else
                    {
                        validator = DataValidator.CreateFormulaListValidator(str3.AddFormulaPrefixSymbol());
                    }
                    break;
                }
                case CriteriaType.Date:
                {
                    double num;
                    DateTime time;
                    double num2;
                    DateTime time2;
                    string str5 = formulaExp;
                    string str6 = str2;
                    if (((str5 != null) && double.TryParse(str5.ToString(), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num)) && FromExcelOADate(num, out time))
                    {
                        str5 = ((double) time.ToOADate()).ToString();
                    }
                    if (((str6 != null) && double.TryParse(str6.ToString(), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num2)) && FromExcelOADate(num2, out time2))
                    {
                        str6 = ((double) time2.ToOADate()).ToString();
                    }
                    validator = DataValidator.CreateDateValidator(typeOperator, str5.AddFormulaPrefixSymbol(), str6.AddFormulaPrefixSymbol(), false);
                    break;
                }
                case CriteriaType.Time:
                    validator = DataValidator.CreateDateValidator(typeOperator, formulaExp.AddFormulaPrefixSymbol(), str2.AddFormulaPrefixSymbol(), true);
                    break;

                case CriteriaType.TextLength:
                    validator = DataValidator.CreateTextLengthValidator(typeOperator, formulaExp.AddFormulaPrefixSymbol(), str2.AddFormulaPrefixSymbol());
                    break;

                case CriteriaType.Custom:
                    validator = DataValidator.CreateFormulaValidator(formulaExp.AddFormulaPrefixSymbol());
                    break;
            }
            if (validator != null)
            {
                validator.ErrorStyle = style;
                validator.ErrorMessage = dataValidation.Error;
                validator.ErrorTitle = dataValidation.ErrorTitle;
                validator.IgnoreBlank = dataValidation.AllowBlank;
                validator.InCellDropdown = dataValidation.ShowPromptBox;
                validator.InputMessage = dataValidation.Prompt;
                validator.InputTitle = dataValidation.PromptTitle;
                validator.ShowErrorMessage = dataValidation.ShowErrorBox;
                validator.ShowInputMessage = dataValidation.ShowInputMessage;
            }
            return validator;
        }

        internal static IDifferentialFormatting ToDifferentialFormatting(this StyleInfo styleInfo, Workbook workbook)
        {
            DifferentialFormatting result = new DifferentialFormatting();
            if ((styleInfo.Formatter != null) && !string.IsNullOrWhiteSpace(styleInfo.Formatter.FormatString))
            {
                string excelFormatCode = ExtendedNumberFormatHelper.GetExcelFormatCode(styleInfo.Formatter.FormatString);
                bool isBuiltIn = true;
                int formatId = ExtendedNumberFormatHelper.GetFormatId(excelFormatCode, ref isBuiltIn);
                if (isBuiltIn)
                {
                    result.FormatId = formatId;
                }
                else
                {
                    result.NumberFormat = new ExcelNumberFormat(formatId, excelFormatCode);
                }
            }
            else
            {
                result.FormatId = -1;
            }
            if (styleInfo.IsBackgroundSet() || styleInfo.IsBackgroundThemeColorSet())
            {
                FillPatternType none = FillPatternType.None;
                IExcelColor excelThemeColor = null;
                if (styleInfo.IsBackgroundThemeColorSet())
                {
                    excelThemeColor = styleInfo.BackgroundThemeColor.GetExcelThemeColor();
                }
                else if ((styleInfo.Background is SolidColorBrush) && (Dt.Cells.Data.ColorHelper.ToArgb((styleInfo.Background as SolidColorBrush).Color) != 0xffffff))
                {
                    Windows.UI.Color color2 = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, (styleInfo.Background as SolidColorBrush).Color);
                    excelThemeColor = new ExcelColor(GcColor.FromArgb(color2.A, color2.R, color2.G, color2.B));
                }
                if (excelThemeColor != null)
                {
                    none = FillPatternType.Solid;
                }
                result.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(none, null, excelThemeColor);
            }
            if ((styleInfo.IsBorderLeftSet() || styleInfo.IsBorderRightSet()) || (styleInfo.IsBorderTopSet() || styleInfo.IsBorderBottomSet()))
            {
                IExcelBorder border = null;
                if ((styleInfo.BorderHorizontal != null) || (styleInfo.BorderVertical != null))
                {
                    border = new ExcelTableBorder();
                    if (styleInfo.BorderHorizontal != null)
                    {
                        (border as IExcelTableBorder).Horizontal = styleInfo.BorderHorizontal.ToExcelBorderSide(workbook);
                    }
                    if (styleInfo.BorderVertical != null)
                    {
                        (border as IExcelTableBorder).Vertical = styleInfo.BorderVertical.ToExcelBorderSide(workbook);
                    }
                }
                else
                {
                    border = new ExcelBorder();
                }
                if (styleInfo.IsBorderLeftSet())
                {
                    border.Left = styleInfo.BorderLeft.ToExcelBorderSide(workbook);
                }
                if (styleInfo.IsBorderRightSet())
                {
                    border.Right = styleInfo.BorderRight.ToExcelBorderSide(workbook);
                }
                if (styleInfo.IsBorderTopSet())
                {
                    border.Top = styleInfo.BorderTop.ToExcelBorderSide(workbook);
                }
                if (styleInfo.IsBorderBottomSet())
                {
                    border.Bottom = styleInfo.BorderBottom.ToExcelBorderSide(workbook);
                }
                result.Border = border;
            }
            if (((styleInfo.IsFontFamilySet() || styleInfo.IsFontSizeSet()) || (styleInfo.IsFontStyleSet() || styleInfo.IsFontWeightSet())) || ((styleInfo.IsForegroundSet() || styleInfo.IsForegroundThemeColorSet()) || (styleInfo.IsFontThemeSet() || styleInfo.IsUnderlineSet())))
            {
                result.Font = new ExcelFont(DefaultStyleCollection.DefaultFontFamily.Source, null, ExcelFontFamily.Auto);
                if (styleInfo.IsFontFamilySet())
                {
                    result.Font.FontName = styleInfo.FontFamily.GetFontName();
                }
                if (result.Font.FontName == workbook.CurrentThemeName)
                {
                    result.Font.FontName = null;
                }
                result.Font.FontFamily = ExcelFontFamily.Auto;
                if (styleInfo.IsFontSizeSet())
                {
                    result.Font.FontSize = UnitHelper.PixelToPoint(styleInfo.FontSize);
                }
                else
                {
                    result.Font.FontSize = 0.0;
                }
                if (styleInfo.IsFontStyleSet() && (styleInfo.FontStyle == FontStyle.Italic))
                {
                    result.Font.IsItalic = true;
                }
                if (styleInfo.IsFontWeightSet() && styleInfo.FontWeight.Equals(FontWeights.Bold))
                {
                    result.Font.IsBold = true;
                }
                if (styleInfo.IsForegroundThemeColorSet())
                {
                    result.Font.FontColor = styleInfo.ForegroundThemeColor.GetExcelThemeColor();
                }
                else if (styleInfo.IsForegroundSet() && (styleInfo.Foreground is SolidColorBrush))
                {
                    Windows.UI.Color color = (styleInfo.Foreground as SolidColorBrush).Color;
                    color = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, color);
                    result.Font.FontColor = new ExcelColor(GcColor.FromArgb(color.A, color.R, color.G, color.B));
                }
                if (styleInfo.IsFontThemeSet())
                {
                    if (styleInfo.FontTheme.ToUpperInvariant() == "Headings".ToUpperInvariant())
                    {
                        result.Font.FontScheme = FontSchemeCategory.Major;
                    }
                    else if (styleInfo.FontTheme.ToUpperInvariant() == "Body".ToUpperInvariant())
                    {
                        result.Font.FontScheme = FontSchemeCategory.Minor;
                    }
                }
                if (styleInfo.IsUnderlineSet() && styleInfo.Underline)
                {
                    result.Font.UnderLineStyle = UnderLineStyle.Single;
                }
                if (styleInfo.IsStrikethroughSet() && styleInfo.Strikethrough)
                {
                    result.Font.IsStrikeOut = true;
                }
            }
            if ((styleInfo.IsVerticalAlignmentSet() || styleInfo.IsHorizontalAlignmentSet()) || (styleInfo.IsWordWrapSet() || styleInfo.IsTextIndentSet()))
            {
                if (result.Alignment == null)
                {
                    result.Alignment = new AlignmentBlock();
                }
                if (styleInfo.IsWordWrapSet())
                {
                    result.Alignment.IsTextWrapped = styleInfo.WordWrap;
                }
                if (styleInfo.IsTextIndentSet())
                {
                    result.Alignment.IndentationLevel = (byte) Math.Ceiling((double) (((double) styleInfo.TextIndent) / 10.0));
                }
                result.Alignment.VerticalAlignment = styleInfo.VerticalAlignment.ToExcelVerticalAlignment();
                result.Alignment.HorizontalAlignment = styleInfo.HorizontalAlignment.ToExcelHorizontalAlignment();
            }
            return result;
        }

        internal static ExcelDynamicFilterType ToDynamicFilterType(this DateOccurringType dateType)
        {
            switch (dateType)
            {
                case DateOccurringType.Today:
                    return ExcelDynamicFilterType.Today;

                case DateOccurringType.Yesterday:
                    return ExcelDynamicFilterType.Yesterday;

                case DateOccurringType.Tomorrow:
                    return ExcelDynamicFilterType.Tomorrow;

                case DateOccurringType.Last7Days:
                    return ExcelDynamicFilterType.LastWeek;

                case DateOccurringType.ThisMonth:
                    return ExcelDynamicFilterType.ThisMonth;

                case DateOccurringType.LastMonth:
                    return ExcelDynamicFilterType.LastMonth;

                case DateOccurringType.NextMonth:
                    return ExcelDynamicFilterType.NextMonth;

                case DateOccurringType.ThisWeek:
                    return ExcelDynamicFilterType.ThisWeek;

                case DateOccurringType.LastWeek:
                    return ExcelDynamicFilterType.LastWeek;

                case DateOccurringType.NextWeek:
                    return ExcelDynamicFilterType.NextWeek;
            }
            return ExcelDynamicFilterType.Null;
        }

        internal static ExcelDynamicFilterType ToDynamicFilterType(this MonthOfYearType monthType)
        {
            switch (monthType)
            {
                case MonthOfYearType.January:
                    return ExcelDynamicFilterType.M1;

                case MonthOfYearType.February:
                    return ExcelDynamicFilterType.M2;

                case MonthOfYearType.March:
                    return ExcelDynamicFilterType.M3;

                case MonthOfYearType.April:
                    return ExcelDynamicFilterType.M4;

                case MonthOfYearType.May:
                    return ExcelDynamicFilterType.M5;

                case MonthOfYearType.June:
                    return ExcelDynamicFilterType.M6;

                case MonthOfYearType.July:
                    return ExcelDynamicFilterType.M7;

                case MonthOfYearType.August:
                    return ExcelDynamicFilterType.M8;

                case MonthOfYearType.September:
                    return ExcelDynamicFilterType.M9;

                case MonthOfYearType.October:
                    return ExcelDynamicFilterType.M10;

                case MonthOfYearType.November:
                    return ExcelDynamicFilterType.M11;

                case MonthOfYearType.December:
                    return ExcelDynamicFilterType.M12;
            }
            return ExcelDynamicFilterType.Null;
        }

        internal static ExcelDynamicFilterType ToDynamicFilterType(this QuarterType quarterType)
        {
            switch (quarterType)
            {
                case QuarterType.Quarter1:
                    return ExcelDynamicFilterType.Q1;

                case QuarterType.Quarter2:
                    return ExcelDynamicFilterType.Q2;

                case QuarterType.Quarter3:
                    return ExcelDynamicFilterType.Q3;

                case QuarterType.Quarter4:
                    return ExcelDynamicFilterType.Q4;
            }
            return ExcelDynamicFilterType.Null;
        }

        internal static ErrorStyle ToErrorStyle(this ExcelDataValidationErrorStyle errorStyle)
        {
            return (ErrorStyle) errorStyle;
        }

        internal static IExcelBorderSide ToExcelBorderSide(this BorderLine borderLine, Workbook workbook)
        {
            IExcelBorderSide side = new ExcelBorderSide();
            if (borderLine != null)
            {
                if (!string.IsNullOrWhiteSpace(borderLine.ThemeColor))
                {
                    side.Color = borderLine.ThemeColor.GetExcelThemeColor();
                }
                else
                {
                    Windows.UI.Color color = borderLine.Color;
                    side.Color = new ExcelColor(GcColor.FromArgb(color.A, color.R, color.G, color.B));
                }
                side.LineStyle = (ExcelBorderStyle)borderLine.Style;
            }
            return side;
        }

        internal static IExcelDataValidation ToExcelDataValidation(this DataValidator dataValidator)
        {
            if (dataValidator == null)
            {
                return null;
            }
            Tuple<object, object, ComparisonOperator> tuple = UnpackConditionBack(dataValidator.Condition);
            string formula = null;
            string str2 = null;
            if (tuple != null)
            {
                if (tuple.Item1 != null)
                {
                    formula = GetFormula(tuple.Item1);
                }
                if (tuple.Item2 != null)
                {
                    str2 = GetFormula(tuple.Item2);
                }
            }
            ExcelDataValidation validation = new ExcelDataValidation {
                Type = dataValidator.Type.ToExcelDataValidationType(),
                FirstFormula = formula,
                SecondFormula = str2
            };
            if (validation.Type == ExcelDataValidationType.Date)
            {
                double num;
                double num2;
                if (((validation.FirstFormula != null) && double.TryParse(validation.FirstFormula, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num)) && ((num > -657435.0) && (num < 2958466.0)))
                {
                    DateTime dt = Dt.Cells.Data.DateTimeExtension.FromOADate(num);
                    validation.FirstFormula = ((double) ToExcelOADate(dt)).ToString();
                }
                if (((validation.SecondFormula != null) && double.TryParse(validation.SecondFormula, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num2)) && ((num2 > -657435.0) && (num2 < 2958466.0)))
                {
                    DateTime time2 = Dt.Cells.Data.DateTimeExtension.FromOADate(num2);
                    validation.SecondFormula = ((double) ToExcelOADate(time2)).ToString();
                }
            }
            if (((validation.Type == ExcelDataValidationType.List) && (dataValidator.Value1 != null)) && !dataValidator.Value1.ToString().StartsWith("="))
            {
                validation.FirstFormula = "\"" + dataValidator.Value1.ToString() + "\"";
            }
            validation.CompareOperator = dataValidator.ComparisonOperator.ToExcelDataValidationOperator();
            validation.AllowBlank = dataValidator.IgnoreBlank;
            validation.Error = dataValidator.ErrorMessage;
            validation.ErrorTitle = dataValidator.ErrorTitle;
            validation.ErrorStyle = dataValidator.ErrorStyle.ToExcelDataValidatorErrorStyle();
            validation.Prompt = dataValidator.InputMessage;
            validation.ShowPromptBox = dataValidator.InCellDropdown;
            validation.PromptTitle = dataValidator.InputTitle;
            validation.ShowErrorBox = dataValidator.ShowErrorMessage;
            validation.ShowInputMessage = dataValidator.ShowInputMessage;
            return validation;
        }

        internal static ExcelDataValidationOperator ToExcelDataValidationOperator(this ComparisonOperator op)
        {
            if (op == ComparisonOperator.EqualTo)
            {
                return ExcelDataValidationOperator.Equal;
            }
            return (ExcelDataValidationOperator) (op - ((ComparisonOperator) 1));
        }

        internal static ExcelDataValidationType ToExcelDataValidationType(this CriteriaType criteriaType)
        {
            return (ExcelDataValidationType) criteriaType;
        }

        internal static ExcelDataValidationErrorStyle ToExcelDataValidatorErrorStyle(this ErrorStyle errorStyle)
        {
            return (ExcelDataValidationErrorStyle) errorStyle;
        }

        internal static IExcelFilter ToExcelFilter(this ConditionBase condition, Worksheet sheet, CellRange range)
        {
            DateCondition condition7;
            ExcelFilterOperator notEqual;
            if (condition == null)
            {
                return null;
            }
            if (sheet == null)
            {
                return null;
            }
            if (condition is ColorCondition)
            {
                goto Label_0B37;
            }
            if (condition is Top10Condition)
            {
                int num;
                ExcelTop10 top = new ExcelTop10();
                Top10Condition condition2 = condition as Top10Condition;
                if (condition2.Type == Top10ConditionType.Bottom)
                {
                    top.Top = false;
                }
                else
                {
                    top.Top = true;
                }
                if (condition2.IsPercent)
                {
                    top.Percent = true;
                }
                if ((condition2.ExpectedValue == null) || !int.TryParse(condition2.ExpectedValue.ToString(), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num))
                {
                    return null;
                }
                top.Value = num;
                if (top.Percent)
                {
                    num = (int) Math.Ceiling((double) (((double) (range.RowCount % num)) / 100.0));
                }
                if ((sheet != null) && (range != null))
                {
                    if (top.Top)
                    {
                        top.FilterValue = Enumerable.Min((IEnumerable<double>) Top10Condition.GetMaxValues(new ActualValue(sheet, 0, 0), num, new ICellRange[] { range }));
                        return top;
                    }
                    top.FilterValue = Enumerable.Max((IEnumerable<double>) Top10Condition.GetMinValues(new ActualValue(sheet, 0, 0), num, new CellRange[] { range }));
                    return top;
                }
                top.FilterValue = double.NaN;
                return top;
            }
            if (condition is AverageCondition)
            {
                AverageCondition condition3 = condition as AverageCondition;
                ExcelDynamicFilter filter = new ExcelDynamicFilter();
                if (condition3.Type == AverageConditionType.Above)
                {
                    filter.Type = ExcelDynamicFilterType.AboveAverage;
                }
                else if (condition3.Type == AverageConditionType.Below)
                {
                    filter.Type = ExcelDynamicFilterType.BelowAverage;
                }
                if (range == null)
                {
                    return null;
                }
                List<double> list = new List<double>();
                for (int i = 1; i < range.RowCount; i++)
                {
                    double d = ConvertToDouble(sheet.GetValue(range.Row + i, range.Column));
                    if (!double.IsNaN(d))
                    {
                        list.Add(d);
                    }
                }
                filter.Value = (double) Enumerable.Average((IEnumerable<double>) list);
                return filter;
            }
            if (condition is DateExCondition)
            {
                DateExCondition condition4 = condition as DateExCondition;
                ExcelDynamicFilter filter2 = new ExcelDynamicFilter();
                if (condition4.ExpectedValueType == typeof(DateOccurringType))
                {
                    filter2.Type = ((DateOccurringType) condition4.ExpectedValue).ToDynamicFilterType();
                    double naN = double.NaN;
                    double to = double.NaN;
                    GetDateExConditionRange((DateOccurringType) condition4.ExpectedValue, ref naN, ref to);
                    filter2.Value = (double) naN;
                    filter2.MaxValue = (double) to;
                    return filter2;
                }
                if (condition4.ExpectedValueType == typeof(QuarterType))
                {
                    filter2.Type = ((QuarterType) condition4.ExpectedValue).ToDynamicFilterType();
                    filter2.Value = 0;
                    filter2.MaxValue = 0;
                    return filter2;
                }
                if (condition4.ExpectedValueType == typeof(MonthOfYearType))
                {
                    filter2.Type = ((MonthOfYearType) condition4.ExpectedValue).ToDynamicFilterType();
                    filter2.Value = 0;
                    filter2.MaxValue = 0;
                    return filter2;
                }
                filter2.Type = ExcelDynamicFilterType.Null;
                return filter2;
            }
            if (condition is CellValueCondition)
            {
                CellValueCondition cellValueCondtion = condition as CellValueCondition;
                ExcelFilterOperator cellValueConditonOperator = GetCellValueConditonOperator(cellValueCondtion);
                if (cellValueConditonOperator != ExcelFilterOperator.None)
                {
                    ExcelCustomFilters filters = new ExcelCustomFilters {
                        Filter1 = new ExcelCustomFilter()
                    };
                    filters.Filter1.Operator = cellValueConditonOperator;
                    if (cellValueCondtion.ExpectedValue != null)
                    {
                        filters.Filter1.Value = cellValueCondtion.ExpectedValue;
                        return filters;
                    }
                    filters.Filter1.Value = cellValueCondtion.ExpectedFormula.RemoveFormulaSymbolIfExists();
                    return filters;
                }
                goto Label_0B37;
            }
            if (condition is TextCondition)
            {
                TextCondition condition6 = condition as TextCondition;
                string str = condition6.ExpectedValue.ToString();
                ExcelFilterOperator none = ExcelFilterOperator.None;
                switch (condition6.CompareType)
                {
                    case TextCompareType.EqualsTo:
                    {
                        ExcelFilters filters2 = new ExcelFilters();
                        if (filters2.Filter == null)
                        {
                            filters2.Filter = new List<string>();
                        }
                        filters2.Filter.Add(str);
                        return filters2;
                    }
                    case TextCompareType.NotEqualsTo:
                        none = ExcelFilterOperator.NotEqual;
                        break;

                    case TextCompareType.BeginsWith:
                        str = str + "*";
                        break;

                    case TextCompareType.DoesNotBeginWith:
                    case TextCompareType.DoesNotEndWith:
                        return null;

                    case TextCompareType.EndsWith:
                        str = "*" + str;
                        break;

                    case TextCompareType.Contains:
                        str = "*" + str + "*";
                        break;

                    case TextCompareType.DoesNotContain:
                        none = ExcelFilterOperator.NotEqual;
                        str = "*" + str + "*";
                        break;
                }
                ExcelCustomFilters filters3 = new ExcelCustomFilters {
                    Filter1 = new ExcelCustomFilter()
                };
                filters3.Filter1.Operator = none;
                filters3.Filter1.Value = str;
                return filters3;
            }
            else
            {
                if (!(condition is DateCondition))
                {
                    if (condition is RelationCondition)
                    {
                        RelationCondition relationCondtion = condition as RelationCondition;
                        if (relationCondtion.CompareType == RelationCompareType.Or)
                        {
                            List<ConditionBase> list2 = ExpanedRalationCondtion(relationCondtion);
                            if (Enumerable.All<ConditionBase>((IEnumerable<ConditionBase>) list2, delegate (ConditionBase item) {
                                return item is DateCondition;
                            }))
                            {
                                ExcelFilters filters6 = new ExcelFilters();
                                foreach (DateCondition condition9 in list2)
                                {
                                    DateTime expectedValue = (DateTime) condition9.ExpectedValue;
                                    if (filters6.DateGroupItem == null)
                                    {
                                        filters6.DateGroupItem = new List<IExcelDateGroupItem>();
                                    }
                                    ExcelDateGroupItem item2 = new ExcelDateGroupItem {
                                        Year = (ushort) expectedValue.Year,
                                        Month = (ushort) expectedValue.Month,
                                        Day = (ushort) expectedValue.Day,
                                        Hour = (ushort) expectedValue.Hour,
                                        Minute = (ushort) expectedValue.Minute,
                                        Second = (ushort) expectedValue.Second,
                                        DateTimeGrouping = ExcelDateTimeGrouping.Day
                                    };
                                    filters6.DateGroupItem.Add(item2);
                                }
                                return filters6;
                            }
                            if (Enumerable.All<ConditionBase>((IEnumerable<ConditionBase>) list2, delegate (ConditionBase item) {
                                return item is TextCondition;
                            }))
                            {
                                bool flag = false;
                                if (list2.Count > 2)
                                {
                                    flag = false;
                                }
                                else
                                {
                                    foreach (TextCondition condition10 in list2)
                                    {
                                        if (condition10.CompareType != TextCompareType.EqualsTo)
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                }
                                if (flag)
                                {
                                    ExcelCustomFilters filters7 = new ExcelCustomFilters();
                                    TextCondition textCondtion = list2[0] as TextCondition;
                                    ExcelFilterOperator textConditonOperator = GetTextConditonOperator(textCondtion);
                                    filters7.Filter1 = new ExcelCustomFilter();
                                    filters7.Filter1.Operator = textConditonOperator;
                                    if (textCondtion.ExpectedValue != null)
                                    {
                                        filters7.Filter1.Value = GetTextCondtionValue(textCondtion.ExpectedValue.ToString(), textCondtion.CompareType);
                                    }
                                    else
                                    {
                                        filters7.Filter1.Value = textCondtion.ExpectedFormula.RemoveFormulaSymbolIfExists();
                                    }
                                    if (list2.Count == 2)
                                    {
                                        textCondtion = list2[1] as TextCondition;
                                        textConditonOperator = GetTextConditonOperator(textCondtion);
                                        filters7.Filter2 = new ExcelCustomFilter();
                                        filters7.Filter2.Operator = textConditonOperator;
                                        if (textCondtion.ExpectedValue != null)
                                        {
                                            filters7.Filter2.Value = GetTextCondtionValue(textCondtion.ExpectedValue.ToString(), textCondtion.CompareType);
                                        }
                                        else
                                        {
                                            filters7.Filter2.Value = textCondtion.ExpectedFormula.RemoveFormulaSymbolIfExists();
                                        }
                                    }
                                    filters7.And = false;
                                    return filters7;
                                }
                                ExcelFilters filters8 = new ExcelFilters();
                                foreach (TextCondition condition12 in list2)
                                {
                                    if (filters8.Filter == null)
                                    {
                                        filters8.Filter = new List<string>();
                                    }
                                    if (condition12.ExpectedValue != null)
                                    {
                                        filters8.Filter.Add(condition12.ExpectedValue.ToString());
                                    }
                                }
                                return filters8;
                            }
                            if (Enumerable.All<ConditionBase>((IEnumerable<ConditionBase>) list2, delegate (ConditionBase item) {
                                return item is CellValueCondition;
                            }))
                            {
                                bool flag2 = true;
                                if (list2.Count <= 2)
                                {
                                    flag2 = false;
                                }
                                if (flag2)
                                {
                                    ExcelFilters filters9 = new ExcelFilters();
                                    foreach (CellValueCondition condition13 in list2)
                                    {
                                        if (filters9.Filter == null)
                                        {
                                            filters9.Filter = new List<string>();
                                        }
                                        if (condition13.ExpectedValue != null)
                                        {
                                            filters9.Filter.Add(condition13.ExpectedValue.ToString());
                                        }
                                    }
                                    return filters9;
                                }
                                if (list2.Count > 0)
                                {
                                    ExcelCustomFilters filters10 = new ExcelCustomFilters();
                                    CellValueCondition condition14 = list2[0] as CellValueCondition;
                                    ExcelFilterOperator operator5 = GetCellValueConditonOperator(condition14);
                                    filters10.Filter1 = new ExcelCustomFilter();
                                    filters10.Filter1.Operator = operator5;
                                    if (condition14.ExpectedValue != null)
                                    {
                                        filters10.Filter1.Value = condition14.ExpectedValue;
                                    }
                                    else
                                    {
                                        filters10.Filter1.Value = condition14.ExpectedFormula.RemoveFormulaSymbolIfExists();
                                    }
                                    if (list2.Count == 2)
                                    {
                                        condition14 = list2[1] as CellValueCondition;
                                        operator5 = GetCellValueConditonOperator(condition14);
                                        filters10.Filter2 = new ExcelCustomFilter();
                                        filters10.Filter2.Operator = operator5;
                                        if (condition14.ExpectedValue != null)
                                        {
                                            filters10.Filter2.Value = condition14.ExpectedValue;
                                        }
                                        else
                                        {
                                            filters10.Filter2.Value = condition14.ExpectedFormula.RemoveFormulaSymbolIfExists();
                                        }
                                    }
                                    filters10.And = false;
                                    return filters10;
                                }
                            }
                        }
                        else
                        {
                            List<ConditionBase> list3 = ExpanedRalationCondtion(relationCondtion);
                            if (list3.Count == 2)
                            {
                                ExcelCustomFilters filters11 = new ExcelCustomFilters {
                                    And = relationCondtion.CompareType == RelationCompareType.And
                                };
                                IExcelFilter filter3 = list3[0].ToExcelFilter(sheet, range);
                                if ((filter3 != null) && (filter3 is IExcelCustomFilters))
                                {
                                    filters11.Filter1 = (filter3 as IExcelCustomFilters).Filter1;
                                }
                                else
                                {
                                    return null;
                                }
                                IExcelFilter filter4 = list3[1].ToExcelFilter(sheet, range);
                                if ((filter4 != null) && (filter4 is IExcelCustomFilters))
                                {
                                    filters11.Filter2 = (filter4 as IExcelCustomFilters).Filter1;
                                    return filters11;
                                }
                                return null;
                            }
                        }
                    }
                    goto Label_0B37;
                }
                condition7 = condition as DateCondition;
                if (condition7.CompareType == DateCompareType.EqualsTo)
                {
                    ExcelFilters filters4 = new ExcelFilters();
                    DateTime time = (DateTime) condition7.ExpectedValue;
                    if (filters4.DateGroupItem == null)
                    {
                        filters4.DateGroupItem = new List<IExcelDateGroupItem>();
                    }
                    ExcelDateGroupItem item = new ExcelDateGroupItem {
                        Year = (ushort) time.Year,
                        Month = (ushort) time.Month,
                        Day = (ushort) time.Day,
                        Hour = (ushort) time.Hour,
                        Minute = (ushort) time.Minute,
                        Second = (ushort) time.Second,
                        DateTimeGrouping = ExcelDateTimeGrouping.Day
                    };
                    filters4.DateGroupItem.Add(item);
                    return filters4;
                }
                notEqual = ExcelFilterOperator.None;
                switch (condition7.CompareType)
                {
                    case DateCompareType.EqualsTo:
                        notEqual = ExcelFilterOperator.None;
                        goto Label_0582;

                    case DateCompareType.NotEqualsTo:
                        notEqual = ExcelFilterOperator.NotEqual;
                        goto Label_0582;

                    case DateCompareType.Before:
                        notEqual = ExcelFilterOperator.LessThan;
                        goto Label_0582;

                    case DateCompareType.BeforeEqualsTo:
                        notEqual = ExcelFilterOperator.LessThanOrEqual;
                        goto Label_0582;

                    case DateCompareType.After:
                        notEqual = ExcelFilterOperator.GreaterThan;
                        goto Label_0582;

                    case DateCompareType.AfterEqualsTo:
                        notEqual = ExcelFilterOperator.GreaterThanOrEqual;
                        goto Label_0582;
                }
                notEqual = ExcelFilterOperator.None;
            }
        Label_0582:
            if (notEqual != ExcelFilterOperator.None)
            {
                ExcelCustomFilters filters5 = new ExcelCustomFilters {
                    Filter1 = new ExcelCustomFilter()
                };
                filters5.Filter1.Operator = notEqual;
                filters5.Filter1.Value = condition7.ExpectedValue.ToString();
                return filters5;
            }
        Label_0B37:
            return null;
        }

        internal static ExcelHorizontalAlignment ToExcelHorizontalAlignment(this CellHorizontalAlignment horizontalAlignment)
        {
            switch (horizontalAlignment)
            {
                case CellHorizontalAlignment.Left:
                    return ExcelHorizontalAlignment.Left;

                case CellHorizontalAlignment.Center:
                    return ExcelHorizontalAlignment.Center;

                case CellHorizontalAlignment.Right:
                    return ExcelHorizontalAlignment.Right;

                case CellHorizontalAlignment.General:
                    return ExcelHorizontalAlignment.General;
            }
            return ExcelHorizontalAlignment.General;
        }

        public static double ToExcelOADate(DateTime dt)
        {
            try
            {
                if ((dt.Year == 0x76c) && (dt.Month < 3))
                {
                    dt = dt.AddDays(-1.0);
                }
                return dt.ToOADate();
            }
            catch
            {
            }
            if ((dt >= DateTime.MinValue) && (dt <= DateTime.MaxValue))
            {
                return dt.ToOADate();
            }
            return 0.0;
        }

        internal static ExcelVerticalAlignment ToExcelVerticalAlignment(this CellVerticalAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case CellVerticalAlignment.Top:
                    return ExcelVerticalAlignment.Top;

                case CellVerticalAlignment.Center:
                    return ExcelVerticalAlignment.Center;

                case CellVerticalAlignment.Bottom:
                    return ExcelVerticalAlignment.Bottom;
            }
            return ExcelVerticalAlignment.Bottom;
        }

        internal static IExtendedFormat ToExtendedFormat(this StyleInfo styleInfo, Workbook workbook)
        {
            IExtendedFormat extendedFormat = null;
            if (styleInfo == null)
            {
                extendedFormat = new ExtendedFormat();
            }
            else
            {
                ExtendedFormat format = new ExtendedFormat();
                if ((styleInfo.Formatter != null) && !string.IsNullOrWhiteSpace(styleInfo.Formatter.FormatString))
                {
                    string excelFormatCode = ExtendedNumberFormatHelper.GetExcelFormatCode(styleInfo.Formatter.FormatString);
                    bool isBuiltIn = true;
                    int id = ExtendedNumberFormatHelper.GetFormatId(excelFormatCode, ref isBuiltIn);
                    if (isBuiltIn)
                    {
                        format.NumberFormatIndex = id;
                    }
                    else
                    {
                        format.NumberFormat = new ExcelNumberFormat(id, excelFormatCode);
                    }
                    if (id > 0)
                    {
                        format.ApplyNumberFormat = true;
                    }
                }
                else
                {
                    format.NumberFormatIndex = 0;
                }
                if (styleInfo.IsBackgroundSet() || styleInfo.IsBackgroundThemeColorSet())
                {
                    if (styleInfo.IsBackgroundThemeColorSet())
                    {
                        format.PatternBackgroundColor = new ExcelColor(ExcelColorType.Indexed, 0x40, 0.0);
                        format.PatternColor = styleInfo.BackgroundThemeColor.GetExcelThemeColor();
                    }
                    else if ((styleInfo.Background is SolidColorBrush) && (Dt.Cells.Data.ColorHelper.ToArgb((styleInfo.Background as SolidColorBrush).Color) != 0xffffff))
                    {
                        Windows.UI.Color color = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, (styleInfo.Background as SolidColorBrush).Color);
                        format.PatternColor = new ExcelColor(GcColor.FromArgb(color.A, color.R, color.G, color.B));
                    }
                    if (format.PatternColor != null)
                    {
                        format.FillPattern = FillPatternType.Solid;
                        format.ApplyFill = true;
                    }
                }
                if ((styleInfo.IsBorderLeftSet() || styleInfo.IsBorderRightSet()) || (styleInfo.IsBorderTopSet() || styleInfo.IsBorderBottomSet()))
                {
                    format.Border = new ExcelBorder();
                    if (styleInfo.IsBorderLeftSet())
                    {
                        format.Border.Left = styleInfo.BorderLeft.ToExcelBorderSide(workbook);
                    }
                    if (styleInfo.IsBorderRightSet())
                    {
                        format.Border.Right = styleInfo.BorderRight.ToExcelBorderSide(workbook);
                    }
                    if (styleInfo.IsBorderTopSet())
                    {
                        format.Border.Top = styleInfo.BorderTop.ToExcelBorderSide(workbook);
                    }
                    if (styleInfo.IsBorderBottomSet())
                    {
                        format.Border.Bottom = styleInfo.BorderBottom.ToExcelBorderSide(workbook);
                    }
                    format.ApplyBorder = true;
                }
                if (((styleInfo.IsFontFamilySet() || styleInfo.IsFontSizeSet()) || (styleInfo.IsFontStyleSet() || styleInfo.IsFontWeightSet())) || ((styleInfo.IsForegroundSet() || styleInfo.IsForegroundThemeColorSet()) || (styleInfo.IsFontThemeSet() || styleInfo.IsUnderlineSet())))
                {
                    format.ApplyFont = true;
                    string fontName = workbook.CurrentTheme.BodyFontName;
                    if (styleInfo.FontFamily != null)
                    {
                        fontName = styleInfo.FontFamily.GetFontName();
                    }
                    if (string.IsNullOrEmpty(fontName))
                    {
                        fontName = DefaultStyleCollection.DefaultFontFamily.Source;
                    }
                    format.Font = new ExcelFont(fontName.Trim(), null, ExcelFontFamily.Auto);
                    if (styleInfo.IsFontSizeSet())
                    {
                        format.Font.FontSize = UnitHelper.PixelToPoint(styleInfo.FontSize);
                    }
                    if (styleInfo.IsFontStyleSet() && (styleInfo.FontStyle == FontStyle.Italic))
                    {
                        format.Font.IsItalic = true;
                    }
                    if (styleInfo.IsFontWeightSet() && styleInfo.FontWeight.Equals(FontWeights.Bold))
                    {
                        format.Font.IsBold = true;
                    }
                    if (styleInfo.Underline)
                    {
                        format.Font.UnderLineStyle = UnderLineStyle.Single;
                    }
                    if (styleInfo.IsForegroundThemeColorSet())
                    {
                        format.Font.FontColor = styleInfo.ForegroundThemeColor.GetExcelThemeColor();
                    }
                    else if (styleInfo.IsForegroundSet() && (styleInfo.Foreground is SolidColorBrush))
                    {
                        Windows.UI.Color color2 = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, (styleInfo.Foreground as SolidColorBrush).Color);
                        format.Font.FontColor = new ExcelColor(GcColor.FromArgb(color2.A, color2.R, color2.G, color2.B));
                    }
                    format.Font.IsStrikeOut = styleInfo.Strikethrough;
                    if (styleInfo.IsFontThemeSet())
                    {
                        if (styleInfo.FontTheme.ToUpperInvariant() == "Headings".ToUpperInvariant())
                        {
                            format.Font.FontScheme = FontSchemeCategory.Major;
                        }
                        else if (styleInfo.FontTheme.ToUpperInvariant() == "Body".ToUpperInvariant())
                        {
                            format.Font.FontScheme = FontSchemeCategory.Minor;
                        }
                    }
                }
                if (styleInfo.IsShrinkToFitSet() && styleInfo.ShrinkToFit)
                {
                    format.IsShrinkToFit = true;
                }
                if (styleInfo.IsLockedSet())
                {
                    format.IsLocked = styleInfo.Locked;
                }
                else
                {
                    format.IsLocked = true;
                }
                if (styleInfo.IsWordWrapSet())
                {
                    format.IsWordWrap = styleInfo.WordWrap;
                }
                if (styleInfo.IsTextIndentSet())
                {
                    format.Indent = (byte)Math.Ceiling((double)(((double)styleInfo.TextIndent) / 10.0));
                }
                format.VerticalAlign = styleInfo.VerticalAlignment.ToExcelVerticalAlignment();
                format.HorizontalAlign = styleInfo.HorizontalAlignment.ToExcelHorizontalAlignment();
                extendedFormat = format;
            }
            return extendedFormat;
        }

        internal static CellHorizontalAlignment ToHorizontalAlignment(this ExcelHorizontalAlignment horizontalAlignment)
        {
            switch (horizontalAlignment)
            {
                case ExcelHorizontalAlignment.General:
                    return CellHorizontalAlignment.General;

                case ExcelHorizontalAlignment.Left:
                case ExcelHorizontalAlignment.CenterContinuous:
                    return CellHorizontalAlignment.Left;

                case ExcelHorizontalAlignment.Center:
                    return CellHorizontalAlignment.Center;

                case ExcelHorizontalAlignment.Right:
                    return CellHorizontalAlignment.Right;

                case ExcelHorizontalAlignment.Fill:
                case ExcelHorizontalAlignment.Justify:
                case ExcelHorizontalAlignment.Distributed:
                    return CellHorizontalAlignment.General;
            }
            return CellHorizontalAlignment.General;
        }

        internal static IconCriterion ToIconCriterion(this IExcelConditionalFormatValueObject cfvo, bool applyOnWhenGreaterThan)
        {
            IconValueType number = IconValueType.Number;
            switch (cfvo.Type)
            {
                case ExcelConditionalFormatValueObjectType.Num:
                    number = IconValueType.Number;
                    break;

                case ExcelConditionalFormatValueObjectType.Min:
                    number = IconValueType.Number;
                    break;

                case ExcelConditionalFormatValueObjectType.Max:
                    number = IconValueType.Number;
                    break;

                case ExcelConditionalFormatValueObjectType.Percent:
                    number = IconValueType.Percent;
                    break;

                case ExcelConditionalFormatValueObjectType.Percentile:
                    number = IconValueType.Percentile;
                    break;

                case ExcelConditionalFormatValueObjectType.Stddev:
                    number = IconValueType.Number;
                    break;

                case ExcelConditionalFormatValueObjectType.Formula:
                    number = IconValueType.Formula;
                    break;
            }
            if (number == IconValueType.Formula)
            {
                return new IconCriterion(applyOnWhenGreaterThan, number, "=" + cfvo.Value);
            }
            return new IconCriterion(applyOnWhenGreaterThan, number, ConvertValueToFormulaIfNeeded(cfvo.Value));
        }

        internal static IExcelConditionalFormatValueObject ToIconditionalFormatValueObject(this IconCriterion criterion)
        {
            ExcelConditionalFormatValueObject obj2 = new ExcelConditionalFormatValueObject();
            switch (criterion.IconValueType)
            {
                case IconValueType.Number:
                    obj2.Type = ExcelConditionalFormatValueObjectType.Num;
                    break;

                case IconValueType.Percent:
                    obj2.Type = ExcelConditionalFormatValueObjectType.Percent;
                    break;

                case IconValueType.Percentile:
                    obj2.Type = ExcelConditionalFormatValueObjectType.Percentile;
                    break;

                case IconValueType.Formula:
                    obj2.Type = ExcelConditionalFormatValueObjectType.Formula;
                    break;
            }
            if (criterion.Value != null)
            {
                obj2.Value = criterion.Value.ToString();
                if (((obj2.Value != null) && obj2.Value.StartsWith("=")) && (obj2.Value.Length > 1))
                {
                    obj2.Value = obj2.Value.Substring(1);
                }
            }
            return obj2;
        }

        internal static StyleInfo ToStyleInfo(this TableStyleInfo tableStyleInfo, Workbook workbook)
        {
            StyleInfo result = new StyleInfo();
            if (tableStyleInfo.IsBackgroundSet())
            {
                result.Background = tableStyleInfo.Background;
            }
            if (tableStyleInfo.IsBackgroundThemeColorSet())
            {
                result.BackgroundThemeColor = tableStyleInfo.BackgroundThemeColor;
            }
            if (tableStyleInfo.IsBorderLeftSet())
            {
                result.BorderLeft = tableStyleInfo.BorderLeft;
            }
            if (tableStyleInfo.IsBorderRightSet())
            {
                result.BorderRight = tableStyleInfo.BorderRight;
            }
            if (tableStyleInfo.IsBorderTopSet())
            {
                result.BorderTop = tableStyleInfo.BorderTop;
            }
            if (tableStyleInfo.IsBorderBottomSet())
            {
                result.BorderBottom = tableStyleInfo.BorderBottom;
            }
            if (tableStyleInfo.IsBorderVerticalSet())
            {
                result.BorderVertical = tableStyleInfo.BorderVertical;
            }
            if (tableStyleInfo.IsBorderHorizontalSet())
            {
                result.BorderHorizontal = tableStyleInfo.BorderHorizontal;
            }
            if (tableStyleInfo.IsFontWeightSet())
            {
                result.FontWeight = tableStyleInfo.FontWeight;
            }
            if (tableStyleInfo.IsFontStyleSet())
            {
                result.FontStyle = tableStyleInfo.FontStyle;
            }
            if (tableStyleInfo.IsFontStretchSet())
            {
                result.FontStretch = tableStyleInfo.FontStretch;
            }
            if (tableStyleInfo.IsForegroundSet())
            {
                result.Foreground = tableStyleInfo.Foreground;
            }
            if (tableStyleInfo.IsForegroundThemeColorSet())
            {
                result.ForegroundThemeColor = tableStyleInfo.ForegroundThemeColor;
            }
            return result;
        }

        internal static TableStyleInfo ToTableStyleInfo(this IDifferentialFormatting dxf, Workbook workbook)
        {
            if (dxf == null)
            {
                return null;
            }
            TableStyleInfo result = new TableStyleInfo();
            if (dxf.Font != null)
            {
                if (dxf.Font.IsItalic)
                {
                    result.FontStyle = FontStyle.Italic;
                }
                else
                {
                    result.FontStyle = FontStyle.Normal;
                }
                if (dxf.Font.IsBold)
                {
                    result.FontWeight = FontWeights.Bold;
                }
                else
                {
                    result.FontWeight = FontWeights.Normal;
                }
                if (dxf.Font.FontColor != null)
                {
                    if (dxf.Font.FontColor.IsThemeColor)
                    {
                        result.ForegroundThemeColor = dxf.Font.FontColor.GetThemeColorName();
                    }
                    else
                    {
                        result.Foreground = dxf.Font.FontColor.ToBrush(workbook);
                    }
                }
            }
            if (dxf.Border != null)
            {
                IExcelBorder border = dxf.Border;
                if (!border.Left.IsNullOrEmpty())
                {
                    BorderLine line = border.Left.ToBorderLine(workbook);
                    if (line != null)
                    {
                        result.BorderLeft = line;
                    }
                }
                if (!border.Right.IsNullOrEmpty())
                {
                    BorderLine line2 = border.Right.ToBorderLine(workbook);
                    if (line2 != null)
                    {
                        result.BorderRight = line2;
                    }
                }
                if (!border.Top.IsNullOrEmpty())
                {
                    BorderLine line3 = border.Top.ToBorderLine(workbook);
                    if (line3 != null)
                    {
                        result.BorderTop = line3;
                    }
                }
                if (!border.Bottom.IsNullOrEmpty())
                {
                    BorderLine line4 = border.Bottom.ToBorderLine(workbook);
                    if (line4 != null)
                    {
                        result.BorderBottom = line4;
                    }
                }
                if (border is IExcelTableBorder)
                {
                    ExcelTableBorder border2 = border as ExcelTableBorder;
                    if (!border2.Vertical.IsNullOrEmpty())
                    {
                        BorderLine line5 = border2.Vertical.ToBorderLine(workbook);
                        if (line5 != null)
                        {
                            result.BorderVertical = line5;
                        }
                    }
                    if (!border2.Horizontal.IsNullOrEmpty())
                    {
                        BorderLine line6 = border2.Horizontal.ToBorderLine(workbook);
                        if (line6 != null)
                        {
                            result.BorderHorizontal = line6;
                        }
                    }
                }
            }
            if (dxf.Fill != null)
            {
                FillPatternType type = dxf.Fill.Item1;
                IExcelColor color = dxf.Fill.Item2;
                IExcelColor color2 = dxf.Fill.Item3;
                if ((type == FillPatternType.Solid) && (color != null))
                {
                    if (color.IsThemeColor)
                    {
                        result.BackgroundThemeColor = color.GetThemeColorName();
                    }
                    else if (((color.IsRGBColor && (color.Value == 0)) || (color.IsIndexedColor && (color.Value == 0x40))) || (color.IsIndexedColor && (color.Value == 0x41)))
                    {
                        if (color2 != null)
                        {
                            if (color2.IsThemeColor)
                            {
                                result.BackgroundThemeColor = color2.GetThemeColorName();
                            }
                            else if (color2.IsRGBColor && (color2.Value != 0))
                            {
                                result.Background = color2.ToBrush(workbook);
                            }
                        }
                    }
                    else
                    {
                        result.Background = color.ToBrush(workbook);
                    }
                }
                else if (((color2.IsRGBColor && (color2.Value == 0)) || (color2.IsIndexedColor && (color2.Value == 0x40))) || (color2.IsIndexedColor && (color2.Value == 0x41)))
                {
                    if (color != null)
                    {
                        if (color.IsThemeColor)
                        {
                            result.BackgroundThemeColor = color.GetThemeColorName();
                        }
                        else if (((!color.IsRGBColor || (color.Value != 0)) && (!color.IsIndexedColor || (color.Value != 0x40))) && (!color.IsIndexedColor || (color.Value != 0x41)))
                        {
                            result.Background = color.ToBrush(workbook);
                        }
                    }
                }
                else if (color2 != null)
                {
                    if (color2.IsThemeColor)
                    {
                        result.BackgroundThemeColor = color2.GetThemeColorName();
                    }
                    else
                    {
                        result.Background = color2.ToBrush(workbook);
                    }
                }
            }
            return result;
        }

        internal static CellVerticalAlignment ToVerticalAlignment(this ExcelVerticalAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case ExcelVerticalAlignment.Top:
                    return CellVerticalAlignment.Top;

                case ExcelVerticalAlignment.Center:
                    return CellVerticalAlignment.Center;

                case ExcelVerticalAlignment.Bottom:
                    return CellVerticalAlignment.Bottom;
            }
            return CellVerticalAlignment.Bottom;
        }

        internal static Tuple<object, object, ComparisonOperator> UnpackConditionBack(ConditionBase condition)
        {
            if (condition is RelationCondition)
            {
                return UnpackRelationCondition(condition as RelationCondition);
            }
            if (condition is NumberCondition)
            {
                NumberCondition condition2 = condition as NumberCondition;
                ComparisonOperator @operator = condition2.CompareType.ToComparisonOperator();
                object expectedValue = condition2.ExpectedFormula;
                if (expectedValue == null)
                {
                    expectedValue = condition2.ExpectedValue;
                }
                return new Tuple<object, object, ComparisonOperator>(expectedValue, null, @operator);
            }
            if (condition is TimeCondition)
            {
                TimeCondition condition3 = condition as TimeCondition;
                ComparisonOperator operator2 = condition3.CompareType.ToComparisonOperator();
                object obj3 = condition3.ExpectedFormula;
                if ((obj3 == null) && (condition3.ExpectedValue is DateTime))
                {
                    obj3 = (double) ToExcelOADate((DateTime) condition3.ExpectedValue);
                }
                return new Tuple<object, object, ComparisonOperator>(obj3, null, operator2);
            }
            if (condition is TextLengthCondition)
            {
                TextLengthCondition condition4 = condition as TextLengthCondition;
                ComparisonOperator operator3 = condition4.CompareType.ToComparisonOperator();
                object obj4 = condition4.ExpectedFormula;
                if (obj4 == null)
                {
                    obj4 = condition4.ExpectedValue;
                }
                return new Tuple<object, object, ComparisonOperator>(obj4, null, operator3);
            }
            if (condition is FormulaCondition)
            {
                FormulaCondition condition5 = condition as FormulaCondition;
                return new Tuple<object, object, ComparisonOperator>((condition5.ExpectedFormula != null) ? condition5.ExpectedFormula : condition5.ExpectedValue, null, ComparisonOperator.Between);
            }
            if (condition is AreaCondition)
            {
                AreaCondition condition6 = condition as AreaCondition;
                object obj5 = condition6.ExpectedFormula;
                if (obj5 == null)
                {
                    obj5 = condition6.ExpectedValue;
                    if (obj5 is string)
                    {
                        obj5 = "\"" + obj5 + "\"";
                    }
                }
                return new Tuple<object, object, ComparisonOperator>(obj5, null, ComparisonOperator.Between);
            }
            if (!(condition is DateCondition))
            {
                return null;
            }
            DateCondition condition7 = condition as DateCondition;
            object expectedFormula = condition7.ExpectedFormula;
            if (expectedFormula == null)
            {
                expectedFormula = condition7.ExpectedValue;
                if (expectedFormula is string)
                {
                    expectedFormula = "\"" + expectedFormula + "\"";
                }
            }
            return new Tuple<object, object, ComparisonOperator>(expectedFormula, null, ComparisonOperator.Between);
        }

        internal static Tuple<object, object, ComparisonOperator> UnpackRelationCondition(RelationCondition condition)
        {
            if (condition == null)
            {
                return null;
            }
            object obj2 = null;
            object obj3 = null;
            ComparisonOperator equalTo = ComparisonOperator.EqualTo;
            RelationCondition condition2 = condition;
            if (condition2.CompareType == RelationCompareType.And)
            {
                equalTo = ComparisonOperator.Between;
            }
            else
            {
                equalTo = ComparisonOperator.NotBetween;
            }
            if (condition.Item1 != null)
            {
                if (condition.Item1 is RelationCondition)
                {
                    obj2 = UnpackRelationCondition(condition.Item1 as RelationCondition);
                }
                else
                {
                    obj2 = UnpackConditionBack(condition.Item1);
                }
            }
            if (condition.Item2 != null)
            {
                if (condition.Item2 is RelationCondition)
                {
                    obj3 = UnpackRelationCondition(condition.Item2 as RelationCondition);
                }
                else
                {
                    obj3 = UnpackConditionBack(condition.Item2);
                }
            }
            return new Tuple<object, object, ComparisonOperator>(obj2, obj3, equalTo);
        }
    }
}

