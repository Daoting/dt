using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Dt.Cells.UI
{
    /// <summary>
    /// hdt 大调整
    /// </summary>
    public partial class SheetTab : Button
    {
        private ContentControl _content;
        private TextBlock _displayElement;
        private TextBox _editingElement;
        private static string _inser_TabName;
        private bool _isEditing;
        private int _sheetIndex;
        private const double DEFAULT_FONTSIZE = 13.0;
        private const double DEFAULT_HEIGHT = 18.0;
        private const double DEFAULT_PADDING_LEFT = 12.0;
        private const double DEFAULT_PADDING_RIGHT = 6.0;
        public static readonly DependencyProperty IsActiveProperty;

        private static string INSER_TabName
        {
            get
            {
                if (string.IsNullOrEmpty(_inser_TabName))
                {
                    _inser_TabName = ResourceStrings.TabStrip_NewSheet;
                }
                return _inser_TabName;
            }
        }

        public bool IsActive
        {
            get { return (bool)((bool)base.GetValue(IsActiveProperty)); }
            set { base.SetValue(IsActiveProperty, (bool)value); }
        }

        internal TabStrip OwningStrip { get; set; }

        public int SheetIndex
        {
            get { return this._sheetIndex; }
            internal set
            {
                this._sheetIndex = value;
                if (this._isEditing)
                {
                    this.GetEditingElement().Text = this.SheetName;
                }
                else
                {
                    this.GetDisplayElement().Text = this.SheetName;
                }
            }
        }

        internal string SheetName
        {
            get
            {
                if (((this.OwningStrip != null) && (this._sheetIndex != -1)) && (this._sheetIndex < this.OwningStrip.Workbook.SheetCount))
                {
                    return this.OwningStrip.Workbook.Sheets[this._sheetIndex].Name;
                }
                return INSER_TabName;
            }
        }

        internal bool Visible
        {
            get
            {
                if (this.OwningStrip == null)
                {
                    return false;
                }
                if (this._sheetIndex == -1)
                {
                    return this.OwningStrip.HasInsertTab;
                }
                return ((this._sheetIndex < this.OwningStrip.Workbook.SheetCount) && this.OwningStrip.Workbook.Sheets[this._sheetIndex].Visible);
            }
        }

        static SheetTab()
        {
            IsActiveProperty = DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(SheetTab),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsActiveChanged)));
        }

        public SheetTab()
        {
            DefaultStyleKey = typeof(SheetTab);
            ClickMode = ClickMode.Press;
            _sheetIndex = -1;
        }

        internal TextBlock GetDisplayElement()
        {
            if (this._displayElement == null)
            {
                this._displayElement = new TextBlock();
                this._displayElement.Padding = new Thickness(0.0);
                this._displayElement.VerticalAlignment = (VerticalAlignment)1;
                _displayElement.FontSize = DEFAULT_FONTSIZE;
                _displayElement.LineStackingStrategy = LineStackingStrategy.MaxHeight;
                _displayElement.LineHeight = 0;
            }
            return this._displayElement;
        }

        internal TextBox GetEditingElement()
        {
            if (this._editingElement == null)
            {
                this._editingElement = new EditingElement();
                this._editingElement.Padding = new Thickness(0.0);
                this._editingElement.BorderThickness = new Thickness(0.0);
                this._editingElement.VerticalAlignment = (VerticalAlignment)2;
                this._editingElement.VerticalContentAlignment = (VerticalAlignment)1;
                this._editingElement.FontSize = DEFAULT_FONTSIZE;
            }
            return this._editingElement;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._content = base.GetTemplateChild("PART_ContentPresenter") as ContentControl;
            base.FontSize = 13.0;
            this.PrepareForDisplay();
            this.UpdateActiveStates();
        }

        private static void OnIsActiveChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SheetTab tab = sender as SheetTab;
            if (tab != null)
                tab.UpdateActiveStates();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            this.UpdateActiveStates();
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            this.UpdateActiveStates();
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            e.Handled = false;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            e.Handled = false;
        }

        internal void PrepareForDisplay()
        {
            if (this._content != null)
            {
                TextBlock txtDisplay = this.GetDisplayElement();
                txtDisplay.Text = this.SheetName;
                this._content.Content = txtDisplay;
                this._isEditing = false;
            }
        }

        internal void PrepareForEditing()
        {
            if (this._content != null)
            {
                TextBox editingElement = this.GetEditingElement();
                editingElement.Text = this.SheetName;
                this._content.Content = editingElement;
                editingElement.KeyDown += txtEditor_KeyDown;
                this._isEditing = true;
            }
        }

        private void txtEditor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == (VirtualKey)0x20)
            {
                TextBox box = sender as TextBox;
                int startIndex = box.SelectionStart;
                int count = box.SelectionLength;
                string str = box.Text.Remove(startIndex, count);
                box.Text = str.Insert(startIndex, " ");
                box.SelectionStart = startIndex + 1;
                e.Handled = true;
            }
        }

        private void UpdateActiveStates()
        {
            if (this.IsActive)
            {
                VisualStateManager.GoToState(this, "Active", true);
            }
#if UWP
            else if (base.IsPointerOver)
            {
                VisualStateManager.GoToState(this, "Hover", true);
            }
#endif
            else
            {
                VisualStateManager.GoToState(this, "Regular", true);
            }
        }
    }
}
