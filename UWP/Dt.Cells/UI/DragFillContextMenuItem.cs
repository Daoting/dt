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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a drag fill context menu item.
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CheckHoverStates")]
    public partial class DragFillContextMenuItem : ContentControl
    {
        bool _isMouseOver;
        /// <summary>
        /// Defines the IsChecked dependency property which indicates what automatic fill type is applied.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", (Type)typeof(bool), (Type)typeof(DragFillContextMenuItem), new PropertyMetadata((bool)false));
        /// <summary>
        /// Indicates a text dependency property of the context menu item.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", (Type)typeof(string), (Type)typeof(DragFillContextMenuItem), new PropertyMetadata(""));

        /// <summary>
        /// Occurs when the IsChecked property has changed.
        /// </summary>
        public event EventHandler Checked;

        /// <summary>
        /// Creates a new instance of the <see cref="T:DragFillContextMenuItem" /> class.
        /// </summary>
        /// <param name="text">The automatic drag fill context menu item text.</param>
        /// <param name="fillType">The automatic drag fill type that the menu item represents.</param>
        public DragFillContextMenuItem(string text, AutoFillType fillType)
        {
            base.DefaultStyleKey = typeof(DragFillContextMenuItem);
            PointerEntered += OnDragFillContextMenuItemPointerEntered;
            PointerExited += OnDragFillContextMenuItemPointerExited;
            PointerPressed += OnDragFillContextMenuItemPointerPressed;
            PointerReleased += OnDragFillContextMenuItemPointerReleased;
            Text = text;
            FillType = fillType;
            Loaded += DragFillContextMenuItem_Loaded;
        }

        /// <summary>
        /// XamlTyp用，hdt
        /// </summary>
        public DragFillContextMenuItem()
        { }

#if ANDROID
        new
#endif
        internal void Click()
        {
            IsChecked = true;
            UpdateVisualState();
            OnChecked();
        }

        void DragFillContextMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            _isMouseOver = false;
            UpdateVisualState();
        }

        void OnChecked()
        {
            if (Checked != null)
            {
                Checked(this, EventArgs.Empty);
            }
        }

        void OnDragFillContextMenuItemPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isMouseOver = true;
            UpdateVisualState();
        }

        void OnDragFillContextMenuItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isMouseOver = false;
            UpdateVisualState();
        }

        void OnDragFillContextMenuItemPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void OnDragFillContextMenuItemPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            IsChecked = true;
            UpdateVisualState();
            OnChecked();
        }

        internal void SetIsChecked(bool isChecked)
        {
            IsChecked = isChecked;
            UpdateVisualState();
        }

        void UpdateVisualState()
        {
            if (IsChecked && _isMouseOver)
            {
                VisualStateManager.GoToState(this, "CheckedAndMouseOver", true);
            }
            else if (_isMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
            else if (IsChecked)
            {
                VisualStateManager.GoToState(this, "Checked", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        /// <summary>
        /// Gets or sets the automatic fill type that the context menu item represents.
        /// </summary>
        /// <value>
        /// The automatic fill type.
        /// </value>
        public AutoFillType FillType { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this context menu item is checked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this context menu item is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get { return (bool)((bool)base.GetValue(IsCheckedProperty)); }
            set { base.SetValue(IsCheckedProperty, (bool)value); }
        }

        /// <summary>
        /// Gets or sets the context menu item text.
        /// </summary>
        /// <value>
        /// The  context menu item text.
        /// </value>
        public string Text
        {
            get { return (string)((string)base.GetValue(TextProperty)); }
            set { base.SetValue(TextProperty, value); }
        }
    }
}

