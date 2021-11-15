#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal partial class EditingLayer : Panel
    {
        static Rect _rcEmpty = new Rect();
        CellsPanel _ownPanel;
        CellItem _editingCell;

        public event EventHandler EditingChanged;

        public EditingLayer(CellsPanel parent)
        {
            _ownPanel = parent;

            EditorStatus = EditorStatus.Ready;
            Editor = new TextBox
            {
                BorderThickness = new Thickness(0.0),
#if IOS
                Padding = new Thickness(4, -1, 0, 0),
#else
                Padding = new Thickness(4, 5, 4, 4),
#endif
                TextWrapping = TextWrapping.Wrap,
                Background = BrushRes.WhiteBrush,
                IsHitTestVisible = false,
                Opacity = 0d,
            };
            Editor.TextChanged += EditorTextChanged;
            Children.Add(Editor);
        }

        public void PrepareEditor(CellItem p_cell)
        {
            ResetEditorCell(p_cell, EditorStatus.Ready);

            // false可控制光标不显示
            Editor.IsHitTestVisible = false;
            Editor.Opacity = 0d;
            Editor.Text = "";

            InvalidateMeasure();
            InvalidateArrange();
        }

        public void ShowEditor(CellItem p_cell, EditorStatus p_status)
        {
            ResetEditorCell(p_cell, p_status);

            // 双击显示光标，IsHitTestVisible为false可控制光标不显示
            // 保证Editor再次点击时不失去焦点
            Editor.IsHitTestVisible = true;
            Editor.Opacity = 1.0;

            var cell = _editingCell.BindingCell;
            StyleInfo info = cell.Worksheet.GetActualStyleInfo(cell.Row.Index, cell.Column.Index, cell.SheetArea, true);
            if (info == null)
                return;

            // Enter状态表示因键盘输入触发，不需要给Editor设置Cell原有的Text
            // Edit状态表示双击触发
            bool isFormula = false;
            if (p_status != EditorStatus.Enter)
                isFormula = ApplyEditorText(info);
            ApplyEditorStyle(info, isFormula);
            Editor.Focus(FocusState.Programmatic);

            InvalidateMeasure();
            InvalidateArrange();
        }

        public void HideEditor()
        {
            _editingCell = null;
            EditorDirty = false;
            EditorStatus = EditorStatus.Ready;
            EditingColumnIndex = -2;
            EditingRowIndex = -2;

            Editor.IsHitTestVisible = false;
            Editor.Opacity = 0d;
            Editor.Text = "";

            InvalidateMeasure();
            InvalidateArrange();
        }

        public int EditingColumnIndex { get; private set; }

        public int EditingRowIndex { get; private set; }

        public TextBox Editor { get; }

        public bool EditorDirty { get; set; }

        public EditorStatus EditorStatus { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            Editor.Measure(availableSize);
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_editingCell == null || _editingCell.BindingCell == null)
            {
                Editor.Arrange(_rcEmpty);
                return finalSize;
            }

            Rect rect = CalcEditorBounds(finalSize);
            double left = rect.Left;
            if (_editingCell.BindingCell.ActualTextIndent > 0)
            {
                float indent = _editingCell.BindingCell.ActualTextIndent * _ownPanel.Excel.ZoomFactor;
                var alignment = _editingCell.BindingCell.ToHorizontalAlignment();
                switch (alignment)
                {
                    case HorizontalAlignment.Left:
                    case HorizontalAlignment.Stretch:
                        left += indent;
                        break;

                    case HorizontalAlignment.Right:
                        left -= indent;
                        break;
                }
            }
            Editor.Arrange(new Rect(left, rect.Top, rect.Width, rect.Height));
            Clip = new RectangleGeometry { Rect = rect };
            return finalSize;
        }

        void ResetEditorCell(CellItem p_cell, EditorStatus p_status)
        {
            _editingCell = p_cell;
            EditorDirty = false;
            int row = p_cell.Row;
            int column = p_cell.Column;
            if (p_cell.CellLayout != null)
            {
                row = p_cell.CellLayout.Row;
                column = p_cell.CellLayout.Column;
            }
            EditingColumnIndex = column;
            EditingRowIndex = row;
            EditorStatus = p_status;
        }

        bool ApplyEditorText(StyleInfo p_info)
        {
            var cell = _editingCell.BindingCell;
            string formula = string.Empty;
            using (((IUIActionExecuter)cell.Worksheet).BeginUIAction())
            {
                int index = cell.Row.Index;
                int column = cell.Column.Index;
                formula = cell.Formula;
                if (formula == null)
                {
                    object[,] objArray = cell.Worksheet.FindFormulas(index, column, 1, 1);
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

            // 存在公式
            if (!string.IsNullOrEmpty(formula))
            {
                Editor.Text = "=" + formula;
                return true;
            }

            if (cell.Value == null)
            {
                Editor.Text = string.Empty;
                return false;
            }

            string text;
            // 存在格式化
            var preferredEditingFormatter = new GeneralFormatter().GetPreferredEditingFormatter(cell.Value);
            if ((preferredEditingFormatter != null) && (p_info.Formatter is AutoFormatter))
            {
                try
                {
                    text = preferredEditingFormatter.Format(cell.Value);
                }
                catch
                {
                    text = cell.Text;
                }
            }
            else
            {
                text = cell.Text;
            }

            var formatter2 = p_info.Formatter;
            if (formatter2 is GeneralFormatter formatter3)
            {
                switch (formatter3.GetFormatType(cell.Value))
                {
                    case NumberFormatType.Number:
                    case NumberFormatType.Text:
                        formatter2 = new GeneralFormatter();
                        break;
                }
            }
            if ((formatter2 != null) && !(formatter2 is AutoFormatter))
            {
                text = formatter2.Format(cell.Value);
            }
            if (text != null && text.StartsWith("=") && _ownPanel.Excel.CanUserEditFormula)
            {
                text = "'" + text;
            }
            Editor.Text = text;
            return false;
        }

        void ApplyEditorStyle(StyleInfo p_info, bool p_isFormula)
        {
            var cell = _editingCell.BindingCell;
            if (p_info.FontSize > 0.0)
            {
                Editor.FontSize = p_info.FontSize * _ownPanel.Excel.ZoomFactor;
            }
            else
            {
                Editor.ClearValue(TextBlock.FontSizeProperty);
            }

            Editor.FontStyle = p_info.FontStyle;
            Editor.FontWeight = p_info.FontWeight;
            Editor.FontStretch = p_info.FontStretch;

            if (p_info.IsFontFamilySet() && (p_info.FontFamily != null))
            {
                Editor.FontFamily = p_info.FontFamily;
            }
            else if (p_info.IsFontThemeSet())
            {
                string fontTheme = p_info.FontTheme;
                IThemeSupport worksheet = cell.Worksheet;
                if (worksheet != null)
                {
                    Editor.FontFamily = worksheet.GetThemeFont(fontTheme);
                }
            }
            else
            {
                Editor.ClearValue(Control.FontFamilyProperty);
            }

            Brush foreground = null;
            if (p_info.IsForegroundSet())
            {
                foreground = p_info.Foreground;
            }
            else if (p_info.IsForegroundThemeColorSet())
            {
                string fname = p_info.ForegroundThemeColor;
                if ((!string.IsNullOrEmpty(fname) && (cell.Worksheet != null)) && (cell.Worksheet.Workbook != null))
                {
                    foreground = new SolidColorBrush(cell.Worksheet.Workbook.GetThemeColor(fname));
                }
            }
            if (foreground != null)
            {
                Editor.Foreground = foreground;
            }
            else
            {
                Editor.Foreground = BrushRes.BlackBrush;
            }

            Editor.VerticalContentAlignment = p_info.VerticalAlignment.ToVerticalAlignment();
            if (p_isFormula)
            {
                Editor.TextAlignment = Windows.UI.Xaml.TextAlignment.Left;
            }
            else if (!cell.ActualWordWrap)
            {
                switch (cell.ToHorizontalAlignment())
                {
                    case HorizontalAlignment.Left:
                    case HorizontalAlignment.Stretch:
                        Editor.TextAlignment = Windows.UI.Xaml.TextAlignment.Left;
                        break;

                    case HorizontalAlignment.Center:
                        Editor.TextAlignment = Windows.UI.Xaml.TextAlignment.Center;
                        break;

                    case HorizontalAlignment.Right:
                        Editor.TextAlignment = Windows.UI.Xaml.TextAlignment.Right;
                        break;
                }
            }
            else
            {
                Editor.TextAlignment = Windows.UI.Xaml.TextAlignment.Left;
            }

            Editor.SelectionStart = Editor.Text.Length;
            Editor.SelectAll();
        }

        Rect CalcEditorBounds(Size viewportSize)
        {
            Rect bounds = _ownPanel.GetCellBounds(EditingRowIndex, EditingColumnIndex, false);
            // 不在可视区
            if (bounds.Width <= 1 || bounds.Height <= 1)
                return new Rect();

            bounds.Width--;
            bounds.Height--;
            Rect rcPanel = new Rect(_ownPanel.Location, viewportSize);
            bounds.Intersect(rcPanel);
            if (bounds.IsEmpty || bounds.Width == 0.0 || bounds.Height == 0.0)
                return new Rect();

            Size cellContentSize = new Size(bounds.Width, bounds.Height);
            double x = bounds.X;
            double height = viewportSize.Height - (bounds.Top - _ownPanel.Location.Y);

            Cell cachedCell = _ownPanel.CellCache.GetCachedCell(EditingRowIndex, EditingColumnIndex);
            HorizontalAlignment alignment = cachedCell.ToHorizontalAlignment();
            switch (alignment)
            {
                case HorizontalAlignment.Left:
                case HorizontalAlignment.Stretch:
                    {
                        float indent = cachedCell.ActualTextIndent * _ownPanel.Excel.ZoomFactor;
                        double num4 = (viewportSize.Width - bounds.Left) + _ownPanel.Location.X;
                        num4 = Math.Max(Math.Min(num4, viewportSize.Width), 0.0);
                        Size maxSize = new Size(num4, height);
                        return new Rect(_ownPanel.PointToClient(new Point(x, bounds.Y)), GetPreferredEditorSize(maxSize, cellContentSize, alignment, indent));
                    }
                case HorizontalAlignment.Right:
                    {
                        float num5 = cachedCell.ActualTextIndent * _ownPanel.Excel.ZoomFactor;
                        double num6 = bounds.Right - _ownPanel.Location.X;
                        num6 = Math.Max(Math.Min(num6, viewportSize.Width), 0.0);
                        Size size4 = new Size(num6, height);
                        Size size = GetPreferredEditorSize(size4, cellContentSize, alignment, num5);
                        Point point = new Point(bounds.Right - size.Width, bounds.Top);
                        return new Rect(_ownPanel.PointToClient(point), size);
                    }
                case HorizontalAlignment.Center:
                    {
                        double num7 = (bounds.Left - _ownPanel.Location.X) + (bounds.Width / 2.0);
                        if (num7 < 0.0)
                        {
                            num7 = 0.0;
                        }
                        double num8 = viewportSize.Width - num7;
                        if (num8 < 0.0)
                        {
                            num8 = 0.0;
                        }
                        double width = 2.0 * Math.Min(num7, num8);
                        Size size6 = new Size(width, height);
                        Size size7 = GetPreferredEditorSize(size6, cellContentSize, alignment, 0f);
                        if (size7.Width > bounds.Width)
                        {
                            x -= (size7.Width - bounds.Width) / 2.0;
                        }
                        return new Rect(_ownPanel.PointToClient(new Point(x, bounds.Y)), size7);
                    }
            }
            Point location = _ownPanel.PointToClient(new Point(bounds.X, bounds.Y));
            return new Rect(location, new Size(bounds.Width, bounds.Height));
        }

        Size GetPreferredEditorSize(Size maxSize, Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            if (!_ownPanel.Excel.CanEditOverflow || string.IsNullOrEmpty(Editor.Text))
                return cellContentSize;

            // 支持文本溢出单元格
            Size realSize = MeasureHelper.MeasureText(
                    Editor.Text,
                    Editor.FontFamily,
                    Editor.FontSize,
                    Editor.FontStretch,
                    Editor.FontStyle,
                    Editor.FontWeight,
                    maxSize,
                    true,
                    null,
                    _ownPanel.Excel.UseLayoutRounding,
                    _ownPanel.Excel.ZoomFactor);
            Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(realSize, _ownPanel.Excel.ZoomFactor);
            // 多出字符'T'的宽度，不再测量
            size.Width += Editor.FontSize;
            //string text = "T";
            //Size size2 = CalcStringSize(new Size(2147483647.0, 2147483647.0), false, text);
            //size.Width += size2.Width;

            double width = Math.Min(maxSize.Width, cellContentSize.Width);
            if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (width < (size.Width + indent)))
            {
                size.Width += indent;
            }
            return new Size(Math.Max(width, size.Width), Math.Max(cellContentSize.Height, size.Height));
        }

        void EditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_ownPanel.IsEditing())
            {
                EditorDirty = true;
                EditingChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

