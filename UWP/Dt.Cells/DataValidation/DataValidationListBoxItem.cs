#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a DataValidation valid item.
    /// </summary>
    public partial class DataValidationListBoxItem : ListBoxItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:DataValidationListBoxItem" /> class.
        /// </summary>
        public DataValidationListBoxItem()
        {
            base.DefaultStyleKey = typeof(DataValidationListBoxItem);
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if ((base.Content == null) || ((base.Content is DataValidationListItem) && ((base.Content as DataValidationListItem).Value == null)))
            {
                DataValidationListBox.SelectedIndex = -1;
                DataValidationListBox.SelectedValue = null;
            }
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (DataValidationListBox != null)
            {
                DataValidationListBox.PerformSelectionChanged();
            }
        }

        internal DataValidationListBox DataValidationListBox { get; set; }
    }
}

