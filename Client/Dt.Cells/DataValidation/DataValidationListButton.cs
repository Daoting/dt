#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:Dt.Cells.UI.GcSpreadSheet" /> DataValidation List button. 
    /// </summary>
    public partial class DataValidationListButton : Button
    {
        /// <summary>
        /// Creates a new instance of the control.
        /// </summary>
        public DataValidationListButton()
        {
            base.DefaultStyleKey = typeof(DataValidationListButton);
            base.IsHitTestVisible = false;
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            e.Handled = false;
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            e.Handled = false;
        }
    }
}

