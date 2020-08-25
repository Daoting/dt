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
        ContentControl _content;
        TextBlock _displayElement;
        TextBox _editingElement;
        const string _inser_TabName = "新建...";
        bool _isEditing;
        int _sheetIndex;
        const double DEFAULT_FONTSIZE = 13.0;
        const double DEFAULT_HEIGHT = 18.0;
        const double DEFAULT_PADDING_LEFT = 12.0;
        const double DEFAULT_PADDING_RIGHT = 6.0;
        public static readonly DependencyProperty IsActiveProperty;

        //static string INSER_TabName
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_inser_TabName))
        //        {
        //            _inser_TabName = ResourceStrings.TabStrip_NewSheet;
        //        }
        //        return _inser_TabName;
        //    }
        //}

        public bool IsActive
        {
            get { return (bool)((bool)base.GetValue(IsActiveProperty)); }
            set { base.SetValue(IsActiveProperty, (bool)value); }
        }

        internal TabStrip OwningStrip { get; set; }

        public int SheetIndex
        {
            get { return _sheetIndex; }
            internal set
            {
                _sheetIndex = value;
                if (_isEditing)
                {
                    GetEditingElement().Text = SheetName;
                }
                else
                {
                    GetDisplayElement().Text = SheetName;
                }
            }
        }

        internal string SheetName
        {
            get
            {
                if (((OwningStrip != null) && (_sheetIndex != -1)) && (_sheetIndex < OwningStrip.Workbook.SheetCount))
                {
                    return OwningStrip.Workbook.Sheets[_sheetIndex].Name;
                }
                return _inser_TabName;
            }
        }

        internal bool Visible
        {
            get
            {
                if (OwningStrip == null)
                {
                    return false;
                }
                if (_sheetIndex == -1)
                {
                    return OwningStrip.HasInsertTab;
                }
                return ((_sheetIndex < OwningStrip.Workbook.SheetCount) && OwningStrip.Workbook.Sheets[_sheetIndex].Visible);
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
            if (_displayElement == null)
            {
                _displayElement = new TextBlock();
                _displayElement.Padding = new Thickness(0.0);
                _displayElement.VerticalAlignment = (VerticalAlignment)1;
                _displayElement.FontSize = DEFAULT_FONTSIZE;
                _displayElement.LineStackingStrategy = LineStackingStrategy.MaxHeight;
                _displayElement.LineHeight = 0;
            }
            return _displayElement;
        }

        internal TextBox GetEditingElement()
        {
            if (_editingElement == null)
            {
                _editingElement = new TextBox();
                _editingElement.Padding = new Thickness(0.0);
                _editingElement.VerticalAlignment = (VerticalAlignment)2;
                _editingElement.FontSize = DEFAULT_FONTSIZE;
            }
            return _editingElement;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _content = GetTemplateChild("PART_ContentPresenter") as ContentControl;
            PrepareForDisplay();
            UpdateActiveStates();
        }

        static void OnIsActiveChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SheetTab tab = sender as SheetTab;
            if (tab != null)
                tab.UpdateActiveStates();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            UpdateActiveStates();
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            UpdateActiveStates();
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
            if (_content != null)
            {
                TextBlock txtDisplay = GetDisplayElement();
                txtDisplay.Text = SheetName;
                _content.Content = txtDisplay;
                _isEditing = false;
            }
        }

        internal void PrepareForEditing()
        {
            if (_content != null)
            {
                TextBox editingElement = GetEditingElement();
                editingElement.Text = SheetName;
                _content.Content = editingElement;
                editingElement.KeyDown += txtEditor_KeyDown;
                _isEditing = true;
            }
        }

        void txtEditor_KeyDown(object sender, KeyRoutedEventArgs e)
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

        void UpdateActiveStates()
        {
            VisualStateManager.GoToState(this, IsActive ? "Active" : "Regular", true);
        }
    }
}
