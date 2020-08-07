#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents auto filter listbox item to display the filter item.
    /// </summary>
    public partial class AutoFilterListBoxItem : ListBoxItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:AutoFilterListBoxItem" /> class.
        /// </summary>
        public AutoFilterListBoxItem()
        {
            base.DefaultStyleKey = typeof(AutoFilterListBoxItem);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CheckBox = base.GetTemplateChild("CheckBox") as Windows.UI.Xaml.Controls.CheckBox;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Space)
            {
                PerformCheck();
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
                VisualStateManager.GoToState(this, "MouseOver", true);
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
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
            if ((intermediatePoints != null) && (intermediatePoints.Count == 1))
            {
                Point point = intermediatePoints[0].Position;
                if (((point.X >= 0.0) && (point.X < base.ActualWidth)) && ((point.Y >= 0.0) && (point.Y < base.ActualHeight)))
                {
                    PerformCheck();
                }
            }
            base.OnPointerReleased(e);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        void PerformCheck()
        {
            if (CheckBox != null)
            {
                CheckBox.IsChecked = new bool?(!CheckBox.IsChecked.HasValue ? true : !CheckBox.IsChecked.Value);
            }
        }

        internal AutoFilterListBox AutoFilterListBox { get; set; }

        Windows.UI.Xaml.Controls.CheckBox CheckBox { get; set; }
    }
}

