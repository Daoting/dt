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
    internal partial class SheetTab : Control
    {
        #region 静态内容
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(SheetTab),
                new PropertyMetadata(false, OnIsActiveChanged));

        static void OnIsActiveChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            ((SheetTab)s).UpdateActiveStates();
        }
        #endregion

        ContentPresenter _presenter;
        readonly TextBlock _block;
        TextBox _textBox;
        const string _inser_TabName = "新建...";
        bool _isEditing;
        int _sheetIndex;

        public SheetTab(TabStrip p_owner)
        {
            DefaultStyleKey = typeof(SheetTab);
            Owner = p_owner;
            _sheetIndex = -1;
            _block = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public TabStrip Owner { get; }

        public int SheetIndex
        {
            get { return _sheetIndex; }
            internal set
            {
                if (_sheetIndex != value)
                {
                    _sheetIndex = value;
                    if (_isEditing)
                        _textBox.Text = SheetName;
                    else
                        _block.Text = SheetName;
                }
            }
        }

        public string SheetName
        {
            get
            {
                if (_sheetIndex != -1 && _sheetIndex < Owner.Workbook.SheetCount)
                    return Owner.Workbook.Sheets[_sheetIndex].Name;
                return _inser_TabName;
            }
        }

        public bool Visible
        {
            get
            {
                if (_sheetIndex == -1)
                    return Owner.Excel.TabStripInsertTab;
                return (_sheetIndex < Owner.Workbook.SheetCount) && Owner.Workbook.Sheets[_sheetIndex].Visible;
            }
        }

        public void PrepareForDisplay()
        {
            if (_presenter != null)
            {
                _block.Text = SheetName;
                _presenter.Content = _block;
                _isEditing = false;
            }
        }

        public void PrepareForEditing()
        {
            if (_presenter != null)
            {
                TextBox tb = GetEditor();
                tb.Text = SheetName;
                _presenter.Content = tb;
                _isEditing = true;
            }
        }

        public string GetEditText()
        {
            if (_textBox != null)
                return _textBox.Text;
            return null;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _presenter = GetTemplateChild("ContentPresenter") as ContentPresenter;
            PrepareForDisplay();
            UpdateActiveStates();
        }

        TextBox GetEditor()
        {
            if (_textBox == null)
            {
                _textBox = new TextBox
                {
                    BorderThickness = new Thickness(0.0),
                    Padding = new Thickness(0, 7, 0, 7),
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 15,
                };
                _textBox.Loaded += OnEditorLoaded;
                _textBox.LostFocus += OnEditorLostFocus;
                // uno上android只支持KeyUp，且只在Enter键时触发！
                _textBox.KeyUp += OnEditorKeyUp;
            }
            return _textBox;
        }

        void OnEditorLoaded(object sender, RoutedEventArgs e)
        {
            _textBox.Focus(FocusState.Programmatic);
            _textBox.SelectAll();
        }

        void OnEditorKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Owner.StopTabEditing(false);
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                Owner.StopTabEditing(true);
            }
        }

        void OnEditorLostFocus(object sender, RoutedEventArgs e)
        {
            Owner.StopTabEditing(false);
        }

        void UpdateActiveStates()
        {
            VisualStateManager.GoToState(this, IsActive ? "Active" : "Regular", true);
        }
    }
}
