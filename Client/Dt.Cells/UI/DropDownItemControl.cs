#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a filter dropdown dialog's item control.
    /// </summary>
    [TemplateVisualState(Name="Focused", GroupName="FocusStates"), TemplateVisualState(Name="Normal", GroupName="CommonStates"), TemplateVisualState(Name="Disabled", GroupName="CommonStates"), TemplateVisualState(Name="Unfocused", GroupName="FocusStates")]
    public partial class DropDownItemControl : DropDownItemBaseControl
    {
        /// <summary>
        /// Indicates the icon property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(BitmapImage),
            typeof(DropDownItemControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Occurs when mouse click the item.
        /// </summary>
#if ANDROID
        new
#endif
        public event RoutedEventHandler Click;

        /// <summary>
        /// Creates a new instance of the <see cref="T:DropDownItemControl" /> class.
        /// </summary>
        public DropDownItemControl()
        {
            DropDownItemControl control = this;
            control.PointerEntered += DropDownItemControl_PointerEntered;
            DropDownItemControl control2 = this;
            control2.PointerExited += DropDownItemControl_PointerExited;
            DropDownItemControl control3 = this;
            control3.PointerReleased += DropDownItemControl_PointerReleased;
            base.DefaultStyleKey = typeof(DropDownItemControl);
        }

        void DropDownItemControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            base.ParentDropDownList.SelectItem(this);
        }

        void DropDownItemControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            base.ParentDropDownList.DeselectItem(this);
        }

        void DropDownItemControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                OnClick();
                e.Handled = true;
            }
            base.OnPointerReleased(e);
        }

        /// <summary>
        /// Called when mouse click the control to raise click event and execute command.
        /// </summary>
        protected virtual void OnClick()
        {
            RoutedEventHandler click = Click;
            if (click != null)
            {
                click(this, new RoutedEventArgs());
            }
            if (base.ParentDropDownList != null)
            {
                base.ParentDropDownList.Close();
            }
            ExecuteCommand();
        }

        internal override void OnIsSelectedChanged()
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (!e.Handled && (e.Key == VirtualKey.Enter))
            {
                OnClick();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                VisualStateManager.GoToState(this, "Focused", true);
            }
        }

        /// <summary>
        /// Called before the PointerExited event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                VisualStateManager.GoToState(this, "Unfocused", true);
            }
        }

        /// <summary>
        /// Updates the state of the control.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> to transition between states; otherwise <c>false</c>.</param>
        protected override void UpdateVisualState(bool useTransitions)
        {
            base.IsEnabled = CanExecuteCommand();
            if (base.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
            }
            if (base.IsSelected && base.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            }
        }

        /// <summary>
        /// Gets or sets the item icon image.
        /// </summary>
        public BitmapImage Icon
        {
            get { return  (BitmapImage)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}

