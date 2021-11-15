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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents auto filter check box to indicates the filter item take effect or not.
    /// </summary>
    public partial class AutoFilterCheckBox : CheckBox
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:AutoFilterCheckBox" /> class.
        /// </summary>
        public AutoFilterCheckBox()
        {
            base.DefaultStyleKey = typeof(AutoFilterCheckBox);
            AutoFilterCheckBox box = this;
            box.Checked += AutoFilterCheckBox_Checked;
            AutoFilterCheckBox box2 = this;
            box2.Unchecked += AutoFilterCheckBox_Unchecked;
            AutoFilterCheckBox box3 = this;
            box.Indeterminate += AutoFilterCheckBox_Indeterminate;
            AutoFilterCheckBox box4 = this;
            box4.Loaded += AutoFilterCheckBox_Loaded;
        }

        void AutoFilterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateVisualState();
        }

        void AutoFilterCheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            UpdateVisualState();
        }

        void AutoFilterCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateVisualState();
        }

        void AutoFilterCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateVisualState();
        }

        void UpdateVisualState()
        {
            AutoFilterItem dataContext = base.DataContext as AutoFilterItem;
            if (dataContext == null)
            {
                VisualStateManager.GoToState(this, "Checked", false);
            }
            else if (!dataContext.IsChecked.HasValue)
            {
                VisualStateManager.GoToState(this, "Indeterminate", false);
            }
            else if (dataContext.IsChecked.Value)
            {
                VisualStateManager.GoToState(this, "Checked", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "UnChecked", false);
            }
        }
    }
}

