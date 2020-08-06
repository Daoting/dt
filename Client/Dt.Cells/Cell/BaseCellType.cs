#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.CellTypes
{
    internal class BaseCellType : ICellType, IZoomSupport, IFormulaEditingSupport
    {
        Cell _dataContext;
        protected FrameworkElement _displayElement;
        protected FrameworkElement _editingElement;
        double _zoomFactor = 1.0;
        double? _appliedShrinkFactor = null;
        Type _cachedValueType;

        public virtual bool ApplyEditing(SheetView sheetView, bool allowFormula)
        {
            TextBox box = _editingElement as TextBox;
            if (box != null)
            {
                bool isFormulaApplied = false;
                string appliedFormula = null;
                bool flag2 = ApplyValueToCell(sheetView, DataContext, allowFormula, box.Text, _cachedValueType, out isFormulaApplied, out appliedFormula);
                _cachedValueType = null;
                return flag2;
            }
            return false;
        }

        internal static bool ApplyValueToCell(SheetView sheetView, Cell bindingCell, bool allowFormula, object editorValue, Type valueType, out bool isFormulaApplied, out string appliedFormula)
        {
            isFormulaApplied = false;
            appliedFormula = null;
            if (bindingCell != null)
            {
                if (ContainsArrayFormula(bindingCell.Worksheet.FindFormulas(bindingCell.Row.Index, bindingCell.Column.Index, 1, 1)))
                {
                    return false;
                }
                string str = (string) (editorValue as string);
                if ((allowFormula && (str != null)) && (str.StartsWith("=") && (str.Length > 1)))
                {
                    appliedFormula = str.TrimStart(new char[] { '=' });
                    try
                    {
                        isFormulaApplied = true;
                        bindingCell.Formula = appliedFormula;
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
                if (!string.IsNullOrEmpty(bindingCell.Formula))
                {
                    bindingCell.Formula = null;
                }
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        if (str.StartsWith("'="))
                        {
                            str = str.Substring(1);
                        }
                        IFormatter actualFormatter = bindingCell.ActualFormatter;
                        if ((actualFormatter != null) && !(actualFormatter is AutoFormatter))
                        {
                            object obj2 = actualFormatter.Parse(str);
                            object obj3 = null;
                            if (obj2 == null)
                            {
                                obj3 = str;
                            }
                            else
                            {
                                obj3 = obj2;
                            }
                            obj3 = sheetView.RaiseCellValueApplying(bindingCell.Row.Index, bindingCell.Column.Index, obj3);
                            bindingCell.Value = obj3;
                        }
                        else
                        {
                            UpdateFormatter(str, bindingCell, valueType);
                        }
                        goto Label_0139;
                    }
                    catch (InvalidCastException)
                    {
                        bindingCell.Value = editorValue as string;
                        goto Label_0139;
                    }
                }
                bindingCell.Value = null;
            }
        Label_0139:
            return true;
        }

        internal static bool ContainsArrayFormula(object[,] formulas)
        {
            if (formulas != null)
            {
                for (int i = 0; i < formulas.GetLength(0); i++)
                {
                    CellRange range = formulas[i, 0] as CellRange;
                    if ((range != null) && ((range.RowCount > 1) || (range.ColumnCount > 1)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual FrameworkElement CreateDisplayElement()
        {
            return new TextBlock();
        }

        protected virtual FrameworkElement CreateEditingElement()
        {
            return new EditingElement();
        }

        Windows.UI.Xaml.Thickness GetDefaultMarginForDisplay(double fontSize)
        {
            double zoomFactor = ZoomFactor;
            Windows.UI.Xaml.Thickness excelBlank = MeasureHelper.GetExcelBlank();
            Windows.UI.Xaml.Thickness textBlockBlank = MeasureHelper.GetTextBlockBlank(fontSize);
            double left = excelBlank.Left - textBlockBlank.Left;
            double right = excelBlank.Right - textBlockBlank.Right;
            double top = excelBlank.Top - textBlockBlank.Top;
            return new Windows.UI.Xaml.Thickness(left, top, right, excelBlank.Bottom - textBlockBlank.Bottom);
        }

        Windows.UI.Xaml.Thickness GetDefaultPaddingForEdit(double fontSize)
        {
            double zoomFactor = ZoomFactor;
            Windows.UI.Xaml.Thickness excelBlank = MeasureHelper.GetExcelBlank();
            Windows.UI.Xaml.Thickness textBoxBlank = MeasureHelper.GetTextBoxBlank(fontSize);
            double left = excelBlank.Left - textBoxBlank.Left;
            double right = excelBlank.Right - textBoxBlank.Right;
            double top = excelBlank.Top - textBoxBlank.Top;
            return new Windows.UI.Xaml.Thickness(left, top, right, excelBlank.Bottom - textBoxBlank.Bottom);
        }

        public virtual FrameworkElement GetDisplayElement()
        {
            if (_displayElement == null)
            {
                _displayElement = CreateDisplayElement();
            }
            return _displayElement;
        }

        public virtual FrameworkElement GetEditingElement()
        {
            if (_editingElement == null)
            {
                _editingElement = CreateEditingElement();
            }
            return _editingElement;
        }

        public virtual bool HasEditingElement()
        {
            return (_editingElement != null);
        }

        public virtual void InitDisplayElement(string p_text)
        {
            TextBlock tb = GetDisplayElement() as TextBlock;
            Cell cell = DataContext;
            if (cell == null || tb == null)
                return;

            tb.Text = p_text;
            tb.HorizontalAlignment = cell.ToHorizontalAlignment();
            //switch (tb.HorizontalAlignment)
            //{
            //    case HorizontalAlignment.Center:
            //        tb.TextAlignment = TextAlignment.Center;
            //        break;

            //    case HorizontalAlignment.Right:
            //        tb.TextAlignment = TextAlignment.Right;
            //        break;

            //    default:
            //        tb.TextAlignment = TextAlignment.Left;
            //        break;
            //}
            //tb.VerticalAlignment = cell.ActualVerticalAlignment.ToVerticalAlignment();
            //if (string.IsNullOrWhiteSpace(p_text))
            //    return;

            ////tb.Foreground = cell.ActualForeground;

            //tb.FontStyle = cell.ActualFontStyle;
            //tb.FontWeight = cell.ActualFontWeight;
            //tb.FontStretch = cell.ActualFontStretch;
            //tb.FontFamily = cell.ActualFontFamily;
            //bool actualWordWrap = cell.ActualWordWrap;
            //tb.TextWrapping = actualWordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;

            //// 自动缩放到格
            //if (!actualWordWrap && cell.ActualShrinkToFit)
            //{
            //    double fontSize = cell.ActualFontSize * ZoomFactor;
            //    if (fontSize > 0.0)
            //    {
            //        double width = MeasureHelper.MeasureText(
            //            p_text,
            //            tb.FontFamily,
            //            fontSize,
            //            tb.FontStretch,
            //            tb.FontStyle,
            //            tb.FontWeight,
            //            new Size(double.PositiveInfinity, double.PositiveInfinity),
            //            false,
            //            null,
            //            tb.UseLayoutRounding,
            //            ZoomFactor).Width;
            //        double num4 = cell.ActualTextIndent * ZoomFactor;
            //        double num5 = cell.Worksheet.GetActualColumnWidth(cell.Column.Index, cell.ColumnSpan, cell.SheetArea) * ZoomFactor;
            //        num5 = MeasureHelper.ConvertExcelCellSizeToTextSize(new Size(num5, double.PositiveInfinity), ZoomFactor).Width;
            //        num5 = Math.Max((double) 0.0, (double) (num5 - num4));
            //        double num2 = num5 / width;
            //        if (num5 < width)
            //        {
            //            _appliedShrinkFactor = new double?(num2);
            //        }
            //        else
            //        {
            //            _appliedShrinkFactor = null;
            //        }
            //    }
            //}
            //else
            //{
            //    _appliedShrinkFactor = null;
            //}
            //double num6 = cell.ActualFontSize * ZoomFactor;
            //if (_appliedShrinkFactor.HasValue)
            //{
            //    num6 *= _appliedShrinkFactor.Value;
            //}
            //if (num6 > 0.0)
            //{
            //    tb.FontSize = num6;
            //}
            //else if (num6 == 0.0)
            //{
            //    tb.Text = "";
            //}
            //else
            //{
            //    tb.ClearValue(TextBlock.FontSizeProperty);
            //}

            //var thickness = GetDefaultMarginForDisplay(num6);
            //double num7 = cell.ActualTextIndent * ZoomFactor;
            //switch (tb.HorizontalAlignment)
            //{
            //    case HorizontalAlignment.Center:
            //        num7 = 0.0;
            //        break;

            //    case HorizontalAlignment.Left:
            //        thickness.Left += num7;
            //        break;

            //    case HorizontalAlignment.Right:
            //        thickness.Right += num7;
            //        break;
            //}
            ////tb.Margin = thickness;

            //if (cell.ActualUnderline)
            //{
            //    Underline underline = new Underline();
            //    Run run = new Run();
            //    run.Text = tb.Text;
            //    underline.Inlines.Add(run);
            //    tb.Inlines.Clear();
            //    tb.Inlines.Add(underline);
            //}
            //else
            //{
            //    string str = tb.Text;
            //    tb.Inlines.Clear();
            //    tb.Text = str;
            //}
            //InitStrikethroughforDisplayElement(tb, cell);
        }

        public virtual void InitEditingElement()
        {
            IFormatter formatter2;
            IFormatter preferredEditingFormatter = null;
            TextBox tbElement = GetEditingElement() as TextBox;
            Cell bindingCell = DataContext;
            StyleInfo info = bindingCell.Worksheet.GetActualStyleInfo(bindingCell.Row.Index, bindingCell.Column.Index, bindingCell.SheetArea, true);
            if ((tbElement == null) || (info == null))
            {
                return;
            }
            bool flag = false;
            string text = string.Empty;
            string formula = "";
            using (((IUIActionExecuter) bindingCell.Worksheet).BeginUIAction())
            {
                int index = bindingCell.Row.Index;
                int column = bindingCell.Column.Index;
                formula = bindingCell.Formula;
                if (formula == null)
                {
                    object[,] objArray = bindingCell.Worksheet.FindFormulas(index, column, 1, 1);
                    if (objArray.GetLength(0) > 0)
                    {
                        string str3 = objArray[0, 1].ToString();
                        int length = str3.Length;
                        if (((length > 2) && str3.StartsWith("{")) && str3.EndsWith("}"))
                        {
                            formula = str3.Substring(1, length - 2);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(formula))
            {
                text = "=" + formula;
                flag = true;
                goto Label_0293;
            }
            if (bindingCell.Value == null)
            {
                if (bindingCell.Value == null)
                {
                    _cachedValueType = null;
                }
                goto Label_0293;
            }
            _cachedValueType = bindingCell.Value.GetType();
            preferredEditingFormatter = new GeneralFormatter().GetPreferredEditingFormatter(bindingCell.Value);
            if ((preferredEditingFormatter != null) && (info.Formatter is AutoFormatter))
            {
                try
                {
                    text = preferredEditingFormatter.Format(bindingCell.Value);
                    goto Label_01F7;
                }
                catch
                {
                    text = bindingCell.Text;
                    goto Label_01F7;
                }
            }
            text = bindingCell.Text;
        Label_01F7:
            formatter2 = info.Formatter;
            if (formatter2 is GeneralFormatter)
            {
                GeneralFormatter formatter3 = formatter2 as GeneralFormatter;
                switch (formatter3.GetFormatType(bindingCell.Value))
                {
                    case NumberFormatType.Number:
                    case NumberFormatType.Text:
                        formatter2 = new GeneralFormatter();
                        break;
                }
            }
            if ((formatter2 != null) && !(formatter2 is AutoFormatter))
            {
                text = formatter2.Format(bindingCell.Value);
            }
            if (((text != null) && text.StartsWith("=")) && CanUserEditFormula)
            {
                text = "'" + text;
            }
        Label_0293:
            tbElement.Text = text;
            if (info.FontSize > 0.0)
            {
                tbElement.FontSize = info.FontSize * ZoomFactor;
            }
            else
            {
                tbElement.ClearValue(TextBlock.FontSizeProperty);
            }
            tbElement.FontStyle = info.FontStyle;
            tbElement.FontWeight = info.FontWeight;
            tbElement.FontStretch = info.FontStretch;
            if (info.IsFontFamilySet() && (info.FontFamily != null))
            {
                tbElement.FontFamily = info.FontFamily;
            }
            else if (info.IsFontThemeSet())
            {
                string fontTheme = info.FontTheme;
                IThemeSupport worksheet = bindingCell.Worksheet;
                if (worksheet != null)
                {
                    tbElement.FontFamily = worksheet.GetThemeFont(fontTheme);
                }
            }
            else
            {
                tbElement.ClearValue(Control.FontFamilyProperty);
            }
            Brush foreground = null;
            if (info.IsForegroundSet())
            {
                foreground = info.Foreground;
            }
            else if (info.IsForegroundThemeColorSet())
            {
                string fname = info.ForegroundThemeColor;
                if ((!string.IsNullOrEmpty(fname) && (bindingCell.Worksheet != null)) && (bindingCell.Worksheet.Workbook != null))
                {
                    foreground = new SolidColorBrush(bindingCell.Worksheet.Workbook.GetThemeColor(fname));
                }
            }
            if (foreground != null)
            {
                tbElement.Foreground = foreground;
            }
            else
            {
                tbElement.Foreground = new SolidColorBrush(Colors.Black);
            }
            HorizontalAlignment alignment = bindingCell.ToHorizontalAlignment();
            VerticalAlignment alignment2 = info.VerticalAlignment.ToVerticalAlignment();
            tbElement.VerticalContentAlignment = alignment2;
            if (flag)
            {
                tbElement.TextAlignment = TextAlignment.Left;
            }
            else if (!bindingCell.ActualWordWrap)
            {
                switch (alignment)
                {
                    case HorizontalAlignment.Left:
                    case HorizontalAlignment.Stretch:
                        tbElement.TextAlignment = TextAlignment.Left;
                        break;

                    case HorizontalAlignment.Center:
                        tbElement.TextAlignment = TextAlignment.Center;
                        break;

                    case HorizontalAlignment.Right:
                        tbElement.TextAlignment = TextAlignment.Right;
                        break;
                }
            }
            else
            {
                tbElement.TextAlignment = TextAlignment.Left;
            }
            Windows.UI.Xaml.Thickness defaultPaddingForEdit = GetDefaultPaddingForEdit(tbElement.FontSize);
            tbElement.Margin = defaultPaddingForEdit;
            tbElement.TextWrapping = TextWrapping.Wrap;
            bool actualUnderline = bindingCell.ActualUnderline;
            bool actualStrikethrough = bindingCell.ActualStrikethrough;
        }

        static void InitStrikethroughforDisplayElement(TextBlock tbElement, Cell cell)
        {
            if (cell.ActualStrikethrough)
            {
                foreach (UIElement element in (tbElement.Parent as Panel).Children)
                {
                    if (element is StrikethroughView)
                    {
                        StrikethroughView view = element as StrikethroughView;
                        if (view.LineContainer != null)
                        {
                            foreach (UIElement element2 in view.LineContainer.Children)
                            {
                                if (element2 is Line)
                                {
                                    (element2 as Line).Stroke = tbElement.Foreground;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void SetEditingElement(FrameworkElement editingElement)
        {
            if (!object.ReferenceEquals(_editingElement, editingElement))
            {
                _editingElement = editingElement;
            }
        }

        static void UpdateFormatter(string text, Cell cell, Type cacheValueType)
        {
            object obj2 = null;
            GeneralFormatter preferredDisplayFormatter = new GeneralFormatter().GetPreferredDisplayFormatter(text, out obj2) as GeneralFormatter;
            object obj3 = obj2;
            if (((cell.ActualFormatter != null) && (obj2 != null)) && ((cell.ActualFormatter is AutoFormatter) && !preferredDisplayFormatter.FormatString.Equals("General")))
            {
                cell.Formatter = new AutoFormatter(preferredDisplayFormatter);
            }
            else if (cell.ActualFormatter == null)
            {
                cell.Formatter = new AutoFormatter(preferredDisplayFormatter);
            }
            cell.Value = obj3;
        }

        public bool CanUserEditFormula { get; set; }

        public Cell DataContext
        {
            get { return  _dataContext; }
            set { _dataContext = value; }
        }

        public double ZoomFactor
        {
            get { return  _zoomFactor; }
            set { _zoomFactor = value; }
        }
    }
}

