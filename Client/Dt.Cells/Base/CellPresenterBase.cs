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
        private Cell _bindingCell;
        private Brush _cachedBackground;
        private Dt.Cells.UI.CellLayout _cellLayout;
        private ICellType _cellType;
        private int _column = -1;
        private FrameworkElement _content;
        private InvalidDataPresenterInfo _dataValidationInvalidPresenterInfo;
        private FilterButton _filterButton;
        private Dt.Cells.UI.FilterButtonInfo _filterButtonInfo;
        private bool _isContentAddedToPanel;
        private bool _isHiddenForEditing;
        private bool _isInvalidating;
        private Dt.Cells.UI.CellOverflowLayout _overflowLayout;
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
            base.DefaultStyleKey = typeof(CellPresenterBase);
            base.Padding = new Windows.UI.Xaml.Thickness(2.0, 0.0, 2.0, 0.0);
            this._cellType = new BaseCellType();
            this.ShowContent = true;
        }

        internal virtual void ApplyState()
        {
            this.SetBackground();
            if (this._filterButton != null)
            {
                this._filterButton.ApplyState();
            }
        }

        private Windows.Foundation.Size CalcStringSize(Windows.Foundation.Size maxSize, bool allowWrap, string text = null)
        {
            if (this._cellType.HasEditingElement())
            {
                TextBox editingElement = this._cellType.GetEditingElement() as TextBox;
                if ((editingElement != null) && !string.IsNullOrEmpty(editingElement.Text))
                {
                    Cell bindingCell = this.BindingCell;
                    if (bindingCell != null)
                    {
                        FontFamily actualFontFamily = bindingCell.ActualFontFamily;
                        if (actualFontFamily == null)
                        {
                            actualFontFamily = editingElement.FontFamily;
                        }
                        object textFormattingMode = null;
                        double fontSize = bindingCell.ActualFontSize * this.ZoomFactor;
                        if (fontSize < 0.0)
                        {
                            fontSize = editingElement.FontSize;
                        }
                        return MeasureHelper.MeasureText((text == null) ? editingElement.Text : text, actualFontFamily, fontSize, bindingCell.ActualFontStretch, bindingCell.ActualFontStyle, bindingCell.ActualFontWeight, maxSize, allowWrap, textFormattingMode, this.SheetView.UseLayoutRounding, this.ZoomFactor);
                    }
                }
            }
            return new Windows.Foundation.Size();
        }

        internal virtual void CleanUpBeforeDiscard()
        {
            this.DetachEvents();
        }

        internal virtual FrameworkElement CreateContent()
        {
            FrameworkElement displayElement = this._cellType.GetDisplayElement();
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
            if ((this.OwningRow == null) || (this.OwningRow.OwningPresenter == null))
            {
                return null;
            }
            int row = this.Row;
            int column = this.Column;
            if (this._cellLayout != null)
            {
                row = this._cellLayout.Row;
                column = this._cellLayout.Column;
            }
            return this.OwningRow.OwningPresenter.CellCache.GetCachedCell(row, column);
        }

        internal virtual FrameworkElement GetEditingElement()
        {
            if (this._cellType == null)
            {
                return null;
            }
            if (this._cellType is IFormulaEditingSupport)
            {
                ((IFormulaEditingSupport) this._cellType).CanUserEditFormula = this.SheetView.CanUserEditFormula;
            }
            this._cellType.InitEditingElement();
            FrameworkElement editingElement = this._cellType.GetEditingElement();
            if ((editingElement != null) && (editingElement is EditingElement))
            {
                (editingElement as EditingElement).Owner = this;
            }
            return editingElement;
        }

        internal Windows.Foundation.Size GetPreferredEditorSize(Windows.Foundation.Size maxSize, Windows.Foundation.Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            if (((this.OwningRow == null) ? null : this.OwningRow.OwningPresenter) == null)
            {
                return new Windows.Foundation.Size();
            }
            if (!this.OwningRow.OwningPresenter.Sheet.CanEditOverflow || (this._cellType == null))
            {
                return new Windows.Foundation.Size(cellContentSize.Width, cellContentSize.Height);
            }
            double num = Math.Min(maxSize.Width, cellContentSize.Width);
            Windows.Foundation.Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(this.CalcStringSize(maxSize, true, null), this.ZoomFactor);
            size.Width += 2.0;
            string text = "T";
            Windows.Foundation.Size size2 = this.CalcStringSize(new Windows.Foundation.Size(2147483647.0, 2147483647.0), false, text);
            size.Width += size2.Width;
            if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (num < (size.Width + indent)))
            {
                size.Width += indent;
            }
            return new Windows.Foundation.Size(Math.Max(num, size.Width), Math.Max(cellContentSize.Height, size.Height));
        }

        internal void HideForEditing()
        {
            if (!this._isHiddenForEditing)
            {
                this._isHiddenForEditing = true;
                this.RootPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void InitCellType()
        {
            if (this._cellType != null)
            {
                this._cellType.DataContext = this.BindingCell;
                IZoomSupport support = this._cellType as IZoomSupport;
                if (support != null)
                {
                    support.ZoomFactor = this.ZoomFactor;
                }
            }
        }

        internal virtual void Invalidate()
        {
            this._isInvalidating = true;
            base.InvalidateMeasure();
        }

        internal bool JudgeWordWrap(Windows.Foundation.Size maxSize, Windows.Foundation.Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            if (((((this.OwningRow == null) ? null : this.OwningRow.OwningPresenter) == null) || !this.OwningRow.OwningPresenter.Sheet.CanEditOverflow) || (this._cellType == null))
            {
                return false;
            }
            double num = Math.Min(maxSize.Width, cellContentSize.Width);
            Windows.Foundation.Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(this.CalcStringSize(new Windows.Foundation.Size(2147483647.0, 2147483647.0), false, null), this.ZoomFactor);
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
            if (this._isHiddenForEditing)
            {
                if (!this.OwningRow.OwningPresenter.IsEditing())
                {
                    this.UnHideForEditing();
                }
                else
                {
                    int editingRowIndex = this.OwningRow.OwningPresenter.EditingContainer.EditingRowIndex;
                    int editingColumnIndex = this.OwningRow.OwningPresenter.EditingContainer.EditingColumnIndex;
                    if ((editingRowIndex != this.Row) || (editingColumnIndex != this.Column))
                    {
                        this.UnHideForEditing();
                    }
                }
            }
            if (this._isInvalidating)
            {
                this.Reset();
            }
            if (this._content != null)
            {
                if (!this.ShowContent && (this._content.Visibility == Visibility.Visible))
                {
                    this._content.Visibility = Visibility.Collapsed;
                }
                else if (this.ShowContent && (this._content.Visibility == Visibility.Collapsed))
                {
                    this._content.Visibility = Visibility.Visible;
                }
            }
            this._isInvalidating = false;
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="M:FrameworkElement.ApplyTemplate" /> when overridden in a derived class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            if (this._rootPanel != null)
            {
                this._rootPanel.Children.Clear();
                this._isContentAddedToPanel = false;
            }
            this._rootPanel = base.GetTemplateChild("Root") as CellBackgroundPanel;
            if (this._rootPanel != null)
            {
                this._rootPanel.OwneringCell = this;
            }
            this.PrepareCellForDisplay();
            base.OnApplyTemplate();
        }

        private void OnBindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PrepareCellForDisplay()
        {
            if (this.BindingCell != null)
            {
                string text = this.BindingCell.Text;
                if (this.BindingCell.SheetArea == SheetArea.Cells)
                {
                    text = this.SheetView.RaiseCellTextRendering(this.BindingCell.Row.Index, this.BindingCell.Column.Index, text);
                }
                if (string.IsNullOrEmpty(text))
                {
                    if (this._isContentAddedToPanel)
                    {
                        if ((this._rootPanel != null) && (this._content != null))
                        {
                            this._rootPanel.Children.Remove(this._content);
                        }
                        this._isContentAddedToPanel = false;
                    }
                }
                else if (!this._isContentAddedToPanel && (this._rootPanel != null))
                {
                    if (this._content == null)
                    {
                        this._content = this.CreateContent();
                        if (!this.ShowContent)
                        {
                            this._content.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (this._content != null)
                    {
                        this._rootPanel.Children.Add(this._content);
                        this._isContentAddedToPanel = true;
                    }
                }
                if (this._cellType != null)
                {
                    this.InitCellType();
                    if (this._isContentAddedToPanel)
                    {
                        this._cellType.InitDisplayElement(text);
                    }
                }
                if (this._rootPanel != null)
                {
                    if (this._isHiddenForEditing)
                    {
                        this.UnHideForEditing();
                    }
                    this.ApplyState();
                }
            }
        }

        internal void RemoveInvalidDataPresenter()
        {
            if (this._dataValidationInvalidPresenterInfo != null)
            {
                this.OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(this._dataValidationInvalidPresenterInfo);
                this._dataValidationInvalidPresenterInfo = null;
            }
        }

        internal virtual void Reset()
        {
            if (this._content == null)
            {
                base.ApplyTemplate();
            }
            if (this._rootPanel != null)
            {
                this._rootPanel.InvalidateMeasure();
            }
            this.PrepareCellForDisplay();
        }

        internal virtual void SetBackground()
        {
            if ((this.BindingCell != null) && (this._rootPanel != null))
            {
                Brush actualBackground = this.BindingCell.ActualBackground;
                if (this._cachedBackground != actualBackground)
                {
                    this._rootPanel.Background = actualBackground;
                    this._cachedBackground = actualBackground;
                }
            }
        }

        internal void SetContentPresenter(FrameworkElement content)
        {
            if ((this.RootPanel != null) && !object.Equals(this._content, content))
            {
                if (this._content != null)
                {
                    this.RootPanel.Children.Remove(this._content);
                }
                this._content = content;
                if (content != null)
                {
                    this.RootPanel.Children.Add(content);
                    if (this._content != null)
                    {
                        this.RootPanel.InvalidateArrange();
                        this._content.Arrange(new Windows.Foundation.Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight));
                        this._content.InvalidateArrange();
                    }
                }
            }
        }

        internal void SetContentVisible(bool visible)
        {
            if (visible)
            {
                if (!this.ShowContent && (this._content != null))
                {
                    this._content.Visibility = Visibility.Visible;
                }
            }
            else if (this.ShowContent && (this._content != null))
            {
                this._content.Visibility = Visibility.Collapsed;
            }
            this.ShowContent = visible;
        }

        internal void SynFilterButton()
        {
            if (this.FilterButtonInfo != null)
            {
                if (this._filterButton == null)
                {
                    FilterButton element = new FilterButton();
                    element.HorizontalAlignment = HorizontalAlignment.Right;
                    element.VerticalAlignment = VerticalAlignment.Bottom;
                    element.CellView = this;
                    element.Area = SheetArea.Cells;
                    this._filterButton = element;
                    Canvas.SetZIndex(element, 0xbb8);
                    this.RootPanel.Children.Add(element);
                }
                else
                {
                    this._filterButton.ApplyState();
                }
            }
            else if (this._filterButton != null)
            {
                this.RootPanel.Children.Remove(this._filterButton);
                this._filterButton = null;
            }
        }

        internal virtual bool TryUpdateVisualTree()
        {
            Dt.Cells.UI.SheetView sheetView = this.SheetView;
            if (sheetView != null)
            {
                Cell bindingCell = this.BindingCell;
                if (bindingCell == null)
                {
                    return false;
                }
                int row = this.Row;
                int column = this.Column;
                if (this.CellLayout != null)
                {
                    row = this.CellLayout.Row;
                    column = this.CellLayout.Column;
                }
                bool flag = false;
                Dt.Cells.UI.FilterButtonInfo info = sheetView.GetFilterButtonInfo(row, column, bindingCell.SheetArea);
                if (info != this.FilterButtonInfo)
                {
                    this.FilterButtonInfo = info;
                    this.SynFilterButton();
                    flag = true;
                }
                if (this.OwningRow.OwningPresenter.Sheet.HighlightInvalidData)
                {
                    if (this._dataValidationInvalidPresenterInfo == null)
                    {
                        DataValidator actualDataValidator = this.BindingCell.ActualDataValidator;
                        if ((actualDataValidator != null) && !actualDataValidator.IsValid(sheetView.Worksheet, this.Row, this.Column, bindingCell.Value))
                        {
                            InvalidDataPresenterInfo info2 = new InvalidDataPresenterInfo {
                                Row = this.Row,
                                Column = this.Column
                            };
                            this._dataValidationInvalidPresenterInfo = info2;
                            this.OwningRow.OwningPresenter.AddDataValidationInvalidDataPresenterInfo(this._dataValidationInvalidPresenterInfo);
                            flag = true;
                        }
                    }
                    else if (this._dataValidationInvalidPresenterInfo != null)
                    {
                        DataValidator validator2 = this.BindingCell.ActualDataValidator;
                        if ((validator2 == null) || validator2.IsValid(sheetView.Worksheet, this.Row, this.Column, bindingCell.Value))
                        {
                            this.OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(this._dataValidationInvalidPresenterInfo);
                            this._dataValidationInvalidPresenterInfo = null;
                        }
                        flag = true;
                    }
                }
                else if (this._dataValidationInvalidPresenterInfo != null)
                {
                    this.OwningRow.OwningPresenter.RemoveDataValidationInvalidDataPresenterInfo(this._dataValidationInvalidPresenterInfo);
                    this._dataValidationInvalidPresenterInfo = null;
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
            if (this._isHiddenForEditing)
            {
                this._isHiddenForEditing = false;
                this.RootPanel.ClearValue(UIElement.VisibilityProperty);
            }
        }

        internal void UpdateBindingCell()
        {
            Cell bindingCell = this.GetBindingCell();
            this._bindingCell = bindingCell;
        }

        private void WalkTree(DependencyObject p)
        {
            if (p is Border)
            {
                ((Border) p).BorderThickness = new Windows.UI.Xaml.Thickness(0.0);
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(p);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(p, i);
                this.WalkTree(child);
            }
        }

        internal Cell BindingCell
        {
            get
            {
                if (this._bindingCell == null)
                {
                    this._bindingCell = this.GetBindingCell();
                }
                return this._bindingCell;
            }
        }

        internal Dt.Cells.UI.CellLayout CellLayout
        {
            get { return  this._cellLayout; }
            set
            {
                if (!object.Equals(this._cellLayout, value))
                {
                    this._cellLayout = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal Dt.Cells.UI.CellOverflowLayout CellOverflowLayout
        {
            get { return  this._overflowLayout; }
            set
            {
                if (!object.Equals(this._overflowLayout, value))
                {
                    this._overflowLayout = value;
                    if (this.RootPanel != null)
                    {
                        this.RootPanel.InvalidateMeasure();
                    }
                    base.InvalidateMeasure();
                }
            }
        }

        internal ICellType CellType
        {
            get { return  this._cellType; }
        }

        /// <summary>
        /// Gets a value that indicates the column index.
        /// </summary>
        public int Column
        {
            get { return  this._column; }
            internal set
            {
                if (value != this._column)
                {
                    this._column = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal FrameworkElement Content
        {
            get { return  this._content; }
            set { this._content = value; }
        }

        internal Dt.Cells.UI.FilterButtonInfo FilterButtonInfo
        {
            get { return  this._filterButtonInfo; }
            set
            {
                if (this._filterButtonInfo != value)
                {
                    this._filterButtonInfo = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal bool HasFilterButton
        {
            get { return  (this._filterButton != null); }
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
                if (this._rootPanel == null)
                {
                    base.ApplyTemplate();
                }
                return this._rootPanel;
            }
        }

        /// <summary>
        /// Gets a value that indicates the row index.
        /// </summary>
        public int Row
        {
            get { return  this.OwningRow.Row; }
        }

        internal Dt.Cells.UI.SheetView SheetView
        {
            get
            {
                if ((this.OwningRow != null) && (this.OwningRow.OwningPresenter != null))
                {
                    return this.OwningRow.OwningPresenter.Sheet;
                }
                return null;
            }
        }

        internal bool ShowContent { get; set; }

        private double ZoomFactor
        {
            get
            {
                Dt.Cells.UI.SheetView sheetView = this.SheetView;
                if (sheetView != null)
                {
                    return (double) sheetView.ZoomFactor;
                }
                return 1.0;
            }
        }
    }
}

