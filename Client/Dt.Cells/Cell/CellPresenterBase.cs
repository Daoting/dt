#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.CellTypes;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents an individual <see cref="T:GcSpreadSheet" /> base cell.
    /// </summary>
    public abstract partial class CellPresenterBase : Control
    {
        Cell _bindingCell;
        CellLayout _cellLayout;
        int _column = -1;
        TextBlock _content;
        InvalidDataPresenterInfo _dataValidationInvalidPresenterInfo;
        FilterButton _filterButton;
        FilterButtonInfo _filterButtonInfo;
        bool _isHiddenForEditing;
        bool _isInvalidating;
        CellOverflowLayout _overflowLayout;
        protected CellBackgroundPanel _rootPanel;

        internal virtual void ApplyState()
        {
            if (BindingCell != null && _rootPanel != null)
            {
                _rootPanel.Background = BindingCell.ActualBackground;
            }
                
            if (_filterButton != null)
            {
                _filterButton.ApplyState();
            }
        }

        internal virtual void CleanUpBeforeDiscard()
        {
            DetachEvents();
        }

        internal virtual void DetachEvents()
        {
        }

        public bool HasEditingElement()
        {
            return false;
        }

        public void SetEditingElement(FrameworkElement editingElement)
        {
            
        }

        internal Cell GetBindingCell()
        {
            if ((OwningRow == null) || (OwningRow.OwningPresenter == null))
            {
                return null;
            }
            int row = Row;
            int column = Column;
            if (_cellLayout != null)
            {
                row = _cellLayout.Row;
                column = _cellLayout.Column;
            }
            return OwningRow.OwningPresenter.CellCache.GetCachedCell(row, column);
        }

        FrameworkElement _editingElement;
        internal FrameworkElement GetEditingElement()
        {
            CanUserEditFormula = SheetView.CanUserEditFormula;
            if (_editingElement == null)
                _editingElement = new EditingElement();

        //    IFormatter formatter2;
        //    IFormatter preferredEditingFormatter = null;
        //    TextBox tbElement = GetEditingElement() as TextBox;
        //    Cell bindingCell = DataContext;
        //    StyleInfo info = bindingCell.Worksheet.GetActualStyleInfo(bindingCell.Row.Index, bindingCell.Column.Index, bindingCell.SheetArea, true);
        //    if ((tbElement == null) || (info == null))
        //    {
        //        return;
        //    }
        //    bool flag = false;
        //    string text = string.Empty;
        //    string formula = "";
        //    using (((IUIActionExecuter)bindingCell.Worksheet).BeginUIAction())
        //    {
        //        int index = bindingCell.Row.Index;
        //        int column = bindingCell.Column.Index;
        //        formula = bindingCell.Formula;
        //        if (formula == null)
        //        {
        //            object[,] objArray = bindingCell.Worksheet.FindFormulas(index, column, 1, 1);
        //            if (objArray.GetLength(0) > 0)
        //            {
        //                string str3 = objArray[0, 1].ToString();
        //                int length = str3.Length;
        //                if (((length > 2) && str3.StartsWith("{")) && str3.EndsWith("}"))
        //                {
        //                    formula = str3.Substring(1, length - 2);
        //                }
        //            }
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(formula))
        //    {
        //        text = "=" + formula;
        //        flag = true;
        //        goto Label_0293;
        //    }
        //    if (bindingCell.Value == null)
        //    {
        //        if (bindingCell.Value == null)
        //        {
        //            _cachedValueType = null;
        //        }
        //        goto Label_0293;
        //    }
        //    _cachedValueType = bindingCell.Value.GetType();
        //    preferredEditingFormatter = new GeneralFormatter().GetPreferredEditingFormatter(bindingCell.Value);
        //    if ((preferredEditingFormatter != null) && (info.Formatter is AutoFormatter))
        //    {
        //        try
        //        {
        //            text = preferredEditingFormatter.Format(bindingCell.Value);
        //            goto Label_01F7;
        //        }
        //        catch
        //        {
        //            text = bindingCell.Text;
        //            goto Label_01F7;
        //        }
        //    }
        //    text = bindingCell.Text;
        //Label_01F7:
        //    formatter2 = info.Formatter;
        //    if (formatter2 is GeneralFormatter)
        //    {
        //        GeneralFormatter formatter3 = formatter2 as GeneralFormatter;
        //        switch (formatter3.GetFormatType(bindingCell.Value))
        //        {
        //            case NumberFormatType.Number:
        //            case NumberFormatType.Text:
        //                formatter2 = new GeneralFormatter();
        //                break;
        //        }
        //    }
        //    if ((formatter2 != null) && !(formatter2 is AutoFormatter))
        //    {
        //        text = formatter2.Format(bindingCell.Value);
        //    }
        //    if (((text != null) && text.StartsWith("=")) && CanUserEditFormula)
        //    {
        //        text = "'" + text;
        //    }
        //Label_0293:
        //    tbElement.Text = text;
        //    if (info.FontSize > 0.0)
        //    {
        //        tbElement.FontSize = info.FontSize * ZoomFactor;
        //    }
        //    else
        //    {
        //        tbElement.ClearValue(TextBlock.FontSizeProperty);
        //    }
        //    tbElement.FontStyle = info.FontStyle;
        //    tbElement.FontWeight = info.FontWeight;
        //    tbElement.FontStretch = info.FontStretch;
        //    if (info.IsFontFamilySet() && (info.FontFamily != null))
        //    {
        //        tbElement.FontFamily = info.FontFamily;
        //    }
        //    else if (info.IsFontThemeSet())
        //    {
        //        string fontTheme = info.FontTheme;
        //        IThemeSupport worksheet = bindingCell.Worksheet;
        //        if (worksheet != null)
        //        {
        //            tbElement.FontFamily = worksheet.GetThemeFont(fontTheme);
        //        }
        //    }
        //    else
        //    {
        //        tbElement.ClearValue(Control.FontFamilyProperty);
        //    }
        //    Brush foreground = null;
        //    if (info.IsForegroundSet())
        //    {
        //        foreground = info.Foreground;
        //    }
        //    else if (info.IsForegroundThemeColorSet())
        //    {
        //        string fname = info.ForegroundThemeColor;
        //        if ((!string.IsNullOrEmpty(fname) && (bindingCell.Worksheet != null)) && (bindingCell.Worksheet.Workbook != null))
        //        {
        //            foreground = new SolidColorBrush(bindingCell.Worksheet.Workbook.GetThemeColor(fname));
        //        }
        //    }
        //    if (foreground != null)
        //    {
        //        tbElement.Foreground = foreground;
        //    }
        //    else
        //    {
        //        tbElement.Foreground = new SolidColorBrush(Colors.Black);
        //    }
        //    HorizontalAlignment alignment = bindingCell.ToHorizontalAlignment();
        //    VerticalAlignment alignment2 = info.VerticalAlignment.ToVerticalAlignment();
        //    tbElement.VerticalContentAlignment = alignment2;
        //    if (flag)
        //    {
        //        tbElement.TextAlignment = TextAlignment.Left;
        //    }
        //    else if (!bindingCell.ActualWordWrap)
        //    {
        //        switch (alignment)
        //        {
        //            case HorizontalAlignment.Left:
        //            case HorizontalAlignment.Stretch:
        //                tbElement.TextAlignment = TextAlignment.Left;
        //                break;

        //            case HorizontalAlignment.Center:
        //                tbElement.TextAlignment = TextAlignment.Center;
        //                break;

        //            case HorizontalAlignment.Right:
        //                tbElement.TextAlignment = TextAlignment.Right;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        tbElement.TextAlignment = TextAlignment.Left;
        //    }
        //    Thickness defaultPaddingForEdit = GetDefaultPaddingForEdit(tbElement.FontSize);
        //    tbElement.Margin = defaultPaddingForEdit;
        //    tbElement.TextWrapping = TextWrapping.Wrap;
            return _editingElement;
        }

        public bool CanUserEditFormula { get; set; }
        
        internal void HideForEditing()
        {
            if (!_isHiddenForEditing)
            {
                _isHiddenForEditing = true;
                _rootPanel.Visibility = Visibility.Collapsed;
            }
        }


#if ANDROID
        new
#endif
        internal virtual void Invalidate()
        {
            _isInvalidating = true;
            InvalidateMeasure();
        }

        #region 重写方法
        protected override Size MeasureOverride(Size constraint)
        {
            if (_isHiddenForEditing && _rootPanel != null)
            {
                if (!OwningRow.OwningPresenter.IsEditing())
                {
                    UnHideForEditing();
                }
                else
                {
                    int editingRowIndex = OwningRow.OwningPresenter.EditingContainer.EditingRowIndex;
                    int editingColumnIndex = OwningRow.OwningPresenter.EditingContainer.EditingColumnIndex;
                    if ((editingRowIndex != Row) || (editingColumnIndex != Column))
                    {
                        UnHideForEditing();
                    }
                }
            }
            if (_isInvalidating)
            {
                UpdataContent();
                _isInvalidating = false;
            }
            return base.MeasureOverride(constraint);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootPanel = GetTemplateChild("Root") as CellBackgroundPanel;
            if (_rootPanel != null)
                _rootPanel.OwneringCell = this;
            UpdataContent();
        }
        #endregion

        protected virtual void UpdataContent()
        {
            Cell cell = BindingCell;
            if (cell == null || _rootPanel == null)
                return;

            string text = cell.Text;
            if (cell.SheetArea == SheetArea.Cells)
            {
                text = SheetView.RaiseCellTextRendering(cell.Row.Index, cell.Column.Index, text);
            }

            if (string.IsNullOrEmpty(text) || !ShowContent)
            {
                if (_content != null)
                {
                    _rootPanel.Children.Remove(_content);
                    _content = null;
                }
            }
            else
            {
                if (_content == null)
                {
                    _content = new TextBlock();
                    Canvas.SetZIndex(_content, 0x7d0);
                    _rootPanel.Children.Add(_content);
                }
                _content.Text = text;
                ApplyStyle(_content);
            }

            if (_isHiddenForEditing)
            {
                UnHideForEditing();
            }
            ApplyState();
        }

        void ApplyStyle(TextBlock p_tb)
        {

        }

        internal void RemoveInvalidDataPresenter()
        {
            if (_dataValidationInvalidPresenterInfo != null)
            {
                OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                _dataValidationInvalidPresenterInfo = null;
            }
        }

        internal void SetContentPresenter(FrameworkElement content)
        {
            //if ((_rootPanel != null) && !object.Equals(_content, content))
            //{
            //    if (_content != null)
            //    {
            //        _rootPanel.Children.Remove(_content);
            //    }
            //    _content = content;
            //    if (content != null)
            //    {
            //        _rootPanel.Children.Add(content);
            //        if (_content != null)
            //        {
            //            _rootPanel.InvalidateArrange();
            //            _content.Arrange(new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight));
            //            _content.InvalidateArrange();
            //        }
            //    }
            //}
        }

        internal void SynFilterButton()
        {
            if (FilterButtonInfo != null)
            {
                if (_filterButton == null)
                {
                    FilterButton element = new FilterButton();
                    element.HorizontalAlignment = HorizontalAlignment.Right;
                    element.VerticalAlignment = VerticalAlignment.Bottom;
                    element.CellView = this;
                    element.Area = SheetArea.Cells;
                    _filterButton = element;
                    Canvas.SetZIndex(element, 0xbb8);
                    _rootPanel.Children.Add(element);
                }
                else
                {
                    _filterButton.ApplyState();
                }
            }
            else if (_filterButton != null)
            {
                _rootPanel.Children.Remove(_filterButton);
                _filterButton = null;
            }
        }

        internal virtual bool TryUpdateVisualTree()
        {
            SheetView sheetView = SheetView;
            Cell bindingCell = BindingCell;
            if (sheetView == null || bindingCell == null)
                return false;

            int row = Row;
            int column = Column;
            if (CellLayout != null)
            {
                row = CellLayout.Row;
                column = CellLayout.Column;
            }

            bool update = false;
            FilterButtonInfo info = sheetView.GetFilterButtonInfo(row, column, bindingCell.SheetArea);
            if (info != FilterButtonInfo)
            {
                FilterButtonInfo = info;
                SynFilterButton();
                update = true;
            }

            if (OwningRow.OwningPresenter.Sheet.HighlightInvalidData)
            {
                if (_dataValidationInvalidPresenterInfo == null)
                {
                    DataValidator actualDataValidator = BindingCell.ActualDataValidator;
                    if ((actualDataValidator != null) && !actualDataValidator.IsValid(sheetView.ActiveSheet, Row, Column, bindingCell.Value))
                    {
                        InvalidDataPresenterInfo info2 = new InvalidDataPresenterInfo
                        {
                            Row = Row,
                            Column = Column
                        };
                        _dataValidationInvalidPresenterInfo = info2;
                        OwningRow.OwningPresenter.AddDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                        update = true;
                    }
                }
                else if (_dataValidationInvalidPresenterInfo != null)
                {
                    DataValidator validator2 = BindingCell.ActualDataValidator;
                    if ((validator2 == null) || validator2.IsValid(sheetView.ActiveSheet, Row, Column, bindingCell.Value))
                    {
                        OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                        _dataValidationInvalidPresenterInfo = null;
                    }
                    update = true;
                }
            }
            else if (_dataValidationInvalidPresenterInfo != null)
            {
                OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                _dataValidationInvalidPresenterInfo = null;
                update = true;
            }
            return update;
        }

        internal void UnHideForEditing()
        {
            if (_isHiddenForEditing)
            {
                _isHiddenForEditing = false;
                _rootPanel.ClearValue(UIElement.VisibilityProperty);
            }
        }

        internal void UpdateBindingCell()
        {
            Cell bindingCell = GetBindingCell();
            _bindingCell = bindingCell;
        }

        void WalkTree(DependencyObject p)
        {
            if (p is Border)
            {
                ((Border)p).BorderThickness = new Thickness(0.0);
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(p);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(p, i);
                WalkTree(child);
            }
        }

        internal Cell BindingCell
        {
            get
            {
                if (_bindingCell == null)
                {
                    _bindingCell = GetBindingCell();
                }
                return _bindingCell;
            }
        }

        internal CellLayout CellLayout
        {
            get { return _cellLayout; }
            set
            {
                if (!object.Equals(_cellLayout, value))
                {
                    _cellLayout = value;
                    InvalidateMeasure();
                }
            }
        }

        internal CellOverflowLayout CellOverflowLayout
        {
            get { return _overflowLayout; }
            set
            {
                if (!object.Equals(_overflowLayout, value))
                {
                    _overflowLayout = value;
                    if (_rootPanel != null)
                    {
                        _rootPanel.InvalidateMeasure();
                    }
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates the column index.
        /// </summary>
        public int Column
        {
            get { return _column; }
            internal set
            {
                if (value != _column)
                {
                    _column = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal FilterButtonInfo FilterButtonInfo
        {
            get { return _filterButtonInfo; }
            set
            {
                if (_filterButtonInfo != value)
                {
                    _filterButtonInfo = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal bool HasFilterButton
        {
            get { return (_filterButton != null); }
        }

        /// <summary>
        /// Gets a value that indicates that the cell's viewport is active. 
        /// </summary>
        protected virtual bool IsActive
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is the active cell.
        /// </summary>
        protected virtual bool IsCurrent
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates that the mouse is over the cell.
        /// </summary>
        protected virtual bool IsMouseOver
        {
            get { return false; }
        }

        internal virtual bool IsRecylable
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is selected.
        /// </summary>
        protected virtual bool IsSelected
        {
            get { return false; }
        }

        internal RowItem OwningRow { get; set; }

        /// <summary>
        /// Gets a value that indicates the row index.
        /// </summary>
        public int Row
        {
            get { return OwningRow.Row; }
        }

        internal SheetView SheetView
        {
            get
            {
                if ((OwningRow != null) && (OwningRow.OwningPresenter != null))
                {
                    return OwningRow.OwningPresenter.Sheet;
                }
                return null;
            }
        }

        internal bool ShowContent { get; set; } = true;

        double ZoomFactor
        {
            get
            {
                SheetView sheetView = SheetView;
                if (sheetView != null)
                {
                    return (double)sheetView.ZoomFactor;
                }
                return 1.0;
            }
        }

        internal Size GetPreferredEditorSize(Size maxSize, Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            if (((OwningRow == null) ? null : OwningRow.OwningPresenter) == null)
            {
                return new Size();
            }
            //if (!OwningRow.OwningPresenter.Sheet.CanEditOverflow || (_cellType == null))
            //{
            //    return new Size(cellContentSize.Width, cellContentSize.Height);
            //}
            double num = Math.Min(maxSize.Width, cellContentSize.Width);
            Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(CalcStringSize(maxSize, true, null), ZoomFactor);
            size.Width += 2.0;
            string text = "T";
            Size size2 = CalcStringSize(new Size(2147483647.0, 2147483647.0), false, text);
            size.Width += size2.Width;
            if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (num < (size.Width + indent)))
            {
                size.Width += indent;
            }
            return new Size(Math.Max(num, size.Width), Math.Max(cellContentSize.Height, size.Height));
        }

        internal bool JudgeWordWrap(Size maxSize, Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            return false;
            //if (((((OwningRow == null) ? null : OwningRow.OwningPresenter) == null) || !OwningRow.OwningPresenter.Sheet.CanEditOverflow) || (_cellType == null))
            //{
            //    return false;
            //}
            //double num = Math.Min(maxSize.Width, cellContentSize.Width);
            //Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(CalcStringSize(new Size(2147483647.0, 2147483647.0), false, null), ZoomFactor);
            //size.Width += 2.0;
            //if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (num < (size.Width + indent)))
            //{
            //    size.Width += indent;
            //}
            //return (maxSize.Width < size.Width);
        }

        Size CalcStringSize(Size maxSize, bool allowWrap, string text = null)
        {
            //if (_cellType.HasEditingElement())
            //{
            //    TextBox editingElement = _cellType.GetEditingElement() as TextBox;
            //    if ((editingElement != null) && !string.IsNullOrEmpty(editingElement.Text))
            //    {
            //        Cell bindingCell = BindingCell;
            //        if (bindingCell != null)
            //        {
            //            FontFamily actualFontFamily = bindingCell.ActualFontFamily;
            //            if (actualFontFamily == null)
            //            {
            //                actualFontFamily = editingElement.FontFamily;
            //            }
            //            object textFormattingMode = null;
            //            double fontSize = bindingCell.ActualFontSize * ZoomFactor;
            //            if (fontSize < 0.0)
            //            {
            //                fontSize = editingElement.FontSize;
            //            }
            //            return MeasureHelper.MeasureText((text == null) ? editingElement.Text : text, actualFontFamily, fontSize, bindingCell.ActualFontStretch, bindingCell.ActualFontStyle, bindingCell.ActualFontWeight, maxSize, allowWrap, textFormattingMode, SheetView.UseLayoutRounding, ZoomFactor);
            //        }
            //    }
            //}
            return new Size();
        }

    }
}

