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
    /// Represents an individual <see cref="T:Dt.Cells.UI.GcSpreadSheet" /> base cell.
    /// </summary>
    [TemplateVisualState(Name="Unselected", GroupName="SelectionStates"), TemplatePart(Name="Root", Type=typeof(CellBackgroundPanel)), TemplateVisualState(Name="Selected", GroupName="SelectionStates")]
    public abstract partial class CellPresenterBase : Control
    {
        Cell _bindingCell;
        Brush _cachedBackground;
        Dt.Cells.UI.CellLayout _cellLayout;
        ICellType _cellType;
        int _column = -1;
        FrameworkElement _content;
        InvalidDataPresenterInfo _dataValidationInvalidPresenterInfo;
        FilterButton _filterButton;
        Dt.Cells.UI.FilterButtonInfo _filterButtonInfo;
        bool _isContentAddedToPanel;
        bool _isHiddenForEditing;
        bool _isInvalidating;
        Dt.Cells.UI.CellOverflowLayout _overflowLayout;
        internal CellBackgroundPanel _rootPanel;
        internal const int ConditionalZIndex = 500;
        internal const int ContentZIndex = 0x7d0;
        internal const int CustomDrawingObjectZIndex = 0x5dc;
        internal const int FilterButtonZIndex = 0xbb8;
        internal const string GCCELL_elementRoot = "Root";
        internal const int SparkLineZIndex = 0x3e8;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public CellPresenterBase()
        {
            DefaultStyleKey = typeof(CellPresenterBase);
            Padding = new Windows.UI.Xaml.Thickness(2.0, 0.0, 2.0, 0.0);
            _cellType = new BaseCellType();
            ShowContent = true;
        }

        internal virtual void ApplyState()
        {
            SetBackground();
            if (_filterButton != null)
            {
                _filterButton.ApplyState();
            }
        }

        Windows.Foundation.Size CalcStringSize(Windows.Foundation.Size maxSize, bool allowWrap, string text = null)
        {
            if (_cellType.HasEditingElement())
            {
                TextBox editingElement = _cellType.GetEditingElement() as TextBox;
                if ((editingElement != null) && !string.IsNullOrEmpty(editingElement.Text))
                {
                    Cell bindingCell = BindingCell;
                    if (bindingCell != null)
                    {
                        FontFamily actualFontFamily = bindingCell.ActualFontFamily;
                        if (actualFontFamily == null)
                        {
                            actualFontFamily = editingElement.FontFamily;
                        }
                        object textFormattingMode = null;
                        double fontSize = bindingCell.ActualFontSize * ZoomFactor;
                        if (fontSize < 0.0)
                        {
                            fontSize = editingElement.FontSize;
                        }
                        return MeasureHelper.MeasureText((text == null) ? editingElement.Text : text, actualFontFamily, fontSize, bindingCell.ActualFontStretch, bindingCell.ActualFontStyle, bindingCell.ActualFontWeight, maxSize, allowWrap, textFormattingMode, SheetView.UseLayoutRounding, ZoomFactor);
                    }
                }
            }
            return new Windows.Foundation.Size();
        }

        internal virtual void CleanUpBeforeDiscard()
        {
            DetachEvents();
        }

        internal virtual FrameworkElement CreateContent()
        {
            FrameworkElement displayElement = _cellType.GetDisplayElement();
            if (displayElement != null)
            {
                Canvas.SetZIndex(displayElement, 0x7d0);
            }
            return displayElement;
        }

        internal virtual void DetachEvents()
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

        internal virtual FrameworkElement GetEditingElement()
        {
            if (_cellType == null)
            {
                return null;
            }
            if (_cellType is IFormulaEditingSupport)
            {
                ((IFormulaEditingSupport) _cellType).CanUserEditFormula = SheetView.CanUserEditFormula;
            }
            _cellType.InitEditingElement();
            return _cellType.GetEditingElement();
        }

        internal Windows.Foundation.Size GetPreferredEditorSize(Windows.Foundation.Size maxSize, Windows.Foundation.Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            if (((OwningRow == null) ? null : OwningRow.OwningPresenter) == null)
            {
                return new Windows.Foundation.Size();
            }
            if (!OwningRow.OwningPresenter.Sheet.CanEditOverflow || (_cellType == null))
            {
                return new Windows.Foundation.Size(cellContentSize.Width, cellContentSize.Height);
            }
            double num = Math.Min(maxSize.Width, cellContentSize.Width);
            Windows.Foundation.Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(CalcStringSize(maxSize, true, null), ZoomFactor);
            size.Width += 2.0;
            string text = "T";
            Windows.Foundation.Size size2 = CalcStringSize(new Windows.Foundation.Size(2147483647.0, 2147483647.0), false, text);
            size.Width += size2.Width;
            if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (num < (size.Width + indent)))
            {
                size.Width += indent;
            }
            return new Windows.Foundation.Size(Math.Max(num, size.Width), Math.Max(cellContentSize.Height, size.Height));
        }

        internal void HideForEditing()
        {
            if (!_isHiddenForEditing)
            {
                _isHiddenForEditing = true;
                RootPanel.Visibility = Visibility.Collapsed;
            }
        }

        void InitCellType()
        {
            if (_cellType != null)
            {
                _cellType.DataContext = BindingCell;
                IZoomSupport support = _cellType as IZoomSupport;
                if (support != null)
                {
                    support.ZoomFactor = ZoomFactor;
                }
            }
        }

#if ANDROID
        new
#endif
        internal virtual void Invalidate()
        {
            _isInvalidating = true;
            base.InvalidateMeasure();
        }

        internal bool JudgeWordWrap(Windows.Foundation.Size maxSize, Windows.Foundation.Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            if (((((OwningRow == null) ? null : OwningRow.OwningPresenter) == null) || !OwningRow.OwningPresenter.Sheet.CanEditOverflow) || (_cellType == null))
            {
                return false;
            }
            double num = Math.Min(maxSize.Width, cellContentSize.Width);
            Windows.Foundation.Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(CalcStringSize(new Windows.Foundation.Size(2147483647.0, 2147483647.0), false, null), ZoomFactor);
            size.Width += 2.0;
            if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (num < (size.Width + indent)))
            {
                size.Width += indent;
            }
            return (maxSize.Width < size.Width);
        }

        /// <summary>
        /// Called to measure a control.
        /// </summary>
        /// <param name="constraint"> The maximum size that the method can return.</param>
        /// <returns> The size of the control, up to the maximum specified by the constraint.</returns>
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size constraint)
        {
            if (_isHiddenForEditing)
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
                Reset();
            }
            if (_content != null)
            {
                if (!ShowContent && (_content.Visibility == Visibility.Visible))
                {
                    _content.Visibility = Visibility.Collapsed;
                }
                else if (ShowContent && (_content.Visibility == Visibility.Collapsed))
                {
                    _content.Visibility = Visibility.Visible;
                }
            }
            _isInvalidating = false;
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="M:FrameworkElement.ApplyTemplate" /> when overridden in a derived class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            if (_rootPanel != null)
            {
                _rootPanel.Children.Clear();
                _isContentAddedToPanel = false;
            }
            _rootPanel = base.GetTemplateChild("Root") as CellBackgroundPanel;
            if (_rootPanel != null)
            {
                _rootPanel.OwneringCell = this;
            }
            PrepareCellForDisplay();
            base.OnApplyTemplate();
        }

        void OnBindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void PrepareCellForDisplay()
        {
            if (BindingCell != null)
            {
                string text = BindingCell.Text;
                if (BindingCell.SheetArea == SheetArea.Cells)
                {
                    text = SheetView.RaiseCellTextRendering(BindingCell.Row.Index, BindingCell.Column.Index, text);
                }
                if (string.IsNullOrEmpty(text))
                {
                    if (_isContentAddedToPanel)
                    {
                        if ((_rootPanel != null) && (_content != null))
                        {
                            _rootPanel.Children.Remove(_content);
                        }
                        _isContentAddedToPanel = false;
                    }
                }
                else if (!_isContentAddedToPanel && (_rootPanel != null))
                {
                    if (_content == null)
                    {
                        _content = CreateContent();
                        if (!ShowContent)
                        {
                            _content.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (_content != null)
                    {
                        _rootPanel.Children.Add(_content);
                        _isContentAddedToPanel = true;
                    }
                }
                if (_cellType != null)
                {
                    InitCellType();
                    if (_isContentAddedToPanel)
                    {
                        _cellType.InitDisplayElement(text);
                    }
                }
                if (_rootPanel != null)
                {
                    if (_isHiddenForEditing)
                    {
                        UnHideForEditing();
                    }
                    ApplyState();
                }
            }
        }

        internal void RemoveInvalidDataPresenter()
        {
            if (_dataValidationInvalidPresenterInfo != null)
            {
                OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                _dataValidationInvalidPresenterInfo = null;
            }
        }

        internal virtual void Reset()
        {
            if (_content == null)
            {
                base.ApplyTemplate();
            }
            if (_rootPanel != null)
            {
                _rootPanel.InvalidateMeasure();
            }
            PrepareCellForDisplay();
        }

        internal virtual void SetBackground()
        {
            if ((BindingCell != null) && (_rootPanel != null))
            {
                Brush actualBackground = BindingCell.ActualBackground;
                if (_cachedBackground != actualBackground)
                {
                    _rootPanel.Background = actualBackground;
                    _cachedBackground = actualBackground;
                }
            }
        }

        internal void SetContentPresenter(FrameworkElement content)
        {
            if ((RootPanel != null) && !object.Equals(_content, content))
            {
                if (_content != null)
                {
                    RootPanel.Children.Remove(_content);
                }
                _content = content;
                if (content != null)
                {
                    RootPanel.Children.Add(content);
                    if (_content != null)
                    {
                        RootPanel.InvalidateArrange();
                        _content.Arrange(new Windows.Foundation.Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight));
                        _content.InvalidateArrange();
                    }
                }
            }
        }

        internal void SetContentVisible(bool visible)
        {
            if (visible)
            {
                if (!ShowContent && (_content != null))
                {
                    _content.Visibility = Visibility.Visible;
                }
            }
            else if (ShowContent && (_content != null))
            {
                _content.Visibility = Visibility.Collapsed;
            }
            ShowContent = visible;
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
                    RootPanel.Children.Add(element);
                }
                else
                {
                    _filterButton.ApplyState();
                }
            }
            else if (_filterButton != null)
            {
                RootPanel.Children.Remove(_filterButton);
                _filterButton = null;
            }
        }

        internal virtual bool TryUpdateVisualTree()
        {
            Dt.Cells.UI.SheetView sheetView = SheetView;
            if (sheetView != null)
            {
                Cell bindingCell = BindingCell;
                if (bindingCell == null)
                {
                    return false;
                }
                int row = Row;
                int column = Column;
                if (CellLayout != null)
                {
                    row = CellLayout.Row;
                    column = CellLayout.Column;
                }
                bool flag = false;
                Dt.Cells.UI.FilterButtonInfo info = sheetView.GetFilterButtonInfo(row, column, bindingCell.SheetArea);
                if (info != FilterButtonInfo)
                {
                    FilterButtonInfo = info;
                    SynFilterButton();
                    flag = true;
                }
                if (OwningRow.OwningPresenter.Sheet.HighlightInvalidData)
                {
                    if (_dataValidationInvalidPresenterInfo == null)
                    {
                        DataValidator actualDataValidator = BindingCell.ActualDataValidator;
                        if ((actualDataValidator != null) && !actualDataValidator.IsValid(sheetView.Worksheet, Row, Column, bindingCell.Value))
                        {
                            InvalidDataPresenterInfo info2 = new InvalidDataPresenterInfo {
                                Row = Row,
                                Column = Column
                            };
                            _dataValidationInvalidPresenterInfo = info2;
                            OwningRow.OwningPresenter.AddDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                            flag = true;
                        }
                    }
                    else if (_dataValidationInvalidPresenterInfo != null)
                    {
                        DataValidator validator2 = BindingCell.ActualDataValidator;
                        if ((validator2 == null) || validator2.IsValid(sheetView.Worksheet, Row, Column, bindingCell.Value))
                        {
                            OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                            _dataValidationInvalidPresenterInfo = null;
                        }
                        flag = true;
                    }
                }
                else if (_dataValidationInvalidPresenterInfo != null)
                {
                    OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                    _dataValidationInvalidPresenterInfo = null;
                    flag = true;
                }
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        internal void UnHideForEditing()
        {
            if (_isHiddenForEditing)
            {
                _isHiddenForEditing = false;
                RootPanel.ClearValue(UIElement.VisibilityProperty);
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
                ((Border) p).BorderThickness = new Windows.UI.Xaml.Thickness(0.0);
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

        internal Dt.Cells.UI.CellLayout CellLayout
        {
            get { return  _cellLayout; }
            set
            {
                if (!object.Equals(_cellLayout, value))
                {
                    _cellLayout = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal Dt.Cells.UI.CellOverflowLayout CellOverflowLayout
        {
            get { return  _overflowLayout; }
            set
            {
                if (!object.Equals(_overflowLayout, value))
                {
                    _overflowLayout = value;
                    if (RootPanel != null)
                    {
                        RootPanel.InvalidateMeasure();
                    }
                    base.InvalidateMeasure();
                }
            }
        }

        internal ICellType CellType
        {
            get { return  _cellType; }
        }

        /// <summary>
        /// Gets a value that indicates the column index.
        /// </summary>
        public int Column
        {
            get { return  _column; }
            internal set
            {
                if (value != _column)
                {
                    _column = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal FrameworkElement Content
        {
            get { return  _content; }
            set { _content = value; }
        }

        internal Dt.Cells.UI.FilterButtonInfo FilterButtonInfo
        {
            get { return  _filterButtonInfo; }
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
            get { return  (_filterButton != null); }
        }

        /// <summary>
        /// Gets a value that indicates that the cell's viewport is active. 
        /// </summary>
        protected virtual bool IsActive
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is the active cell.
        /// </summary>
        protected virtual bool IsCurrent
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets a value that indicates that the mouse is over the cell.
        /// </summary>
        protected virtual bool IsMouseOver
        {
            get { return  false; }
        }

        internal virtual bool IsRecylable
        {
            get { return  true; }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is selected.
        /// </summary>
        protected virtual bool IsSelected
        {
            get { return  false; }
        }

        internal RowPresenter OwningRow { get; set; }

        internal CellBackgroundPanel RootPanel
        {
            get
            {
                if (_rootPanel == null)
                {
                    base.ApplyTemplate();
                }
                return _rootPanel;
            }
        }

        /// <summary>
        /// Gets a value that indicates the row index.
        /// </summary>
        public int Row
        {
            get { return  OwningRow.Row; }
        }

        internal Dt.Cells.UI.SheetView SheetView
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

        internal bool ShowContent { get; set; }

        double ZoomFactor
        {
            get
            {
                Dt.Cells.UI.SheetView sheetView = SheetView;
                if (sheetView != null)
                {
                    return (double) sheetView.ZoomFactor;
                }
                return 1.0;
            }
        }
    }
}

