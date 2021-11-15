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
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents auto filter listbox to display the filter items.
    /// </summary>
    public partial class AutoFilterListBox : ListBox
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:AutoFilterListBox" /> class.
        /// </summary>
        public AutoFilterListBox()
        {
            base.DefaultStyleKey = typeof(AutoFilterListBox);
        }

        /// <summary>
        /// Creates or identifies the element used to display a specified item.
        /// </summary>
        /// <returns>
        /// An  AutoFilterListBoxItem.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AutoFilterListBoxItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own ItemContainer.
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <returns>
        /// <c>true</c> if the item is its own ItemContainer; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is AutoFilterListBoxItem);
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus" /> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            ProcessGotFocus(e);
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Responds to the <see cref="E:System.Windows.UIElement.KeyDown" /> event.
        /// </summary>
        /// <param name="e">Provides data for <see cref="T:System.Windows.Input.KeyEventArgs" />.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (((e.Key != VirtualKey.Up) || (base.SelectedIndex != 0)) && ((e.Key != VirtualKey.Down) || (base.SelectedIndex != (base.Items.Count - 1))))
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            AutoFilterListBoxItem item2 = element as AutoFilterListBoxItem;
            if (item2 != null)
            {
                item2.AutoFilterListBox = this;
            }
        }

        void ProcessGotFocus(RoutedEventArgs e)
        {
            if (base.SelectedIndex < 0)
            {
                base.SelectedIndex = 0;
            }
            ListBoxItem targetElement = base.ContainerFromItem(base.SelectedItem) as ListBoxItem;
            if ((targetElement != null) && !ElementTreeHelper.IsKeyboardFocusWithin(targetElement))
            {
                targetElement.Focus(FocusState.Programmatic);
            }
        }
    }
}

