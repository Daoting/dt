#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> range group header button
    /// that is used to expand or collapse all the groups in the same level.
    /// </summary>
    public partial class RangeGroupHeaderButtonPresenter : Button
    {
        /// <summary>
        /// Indicates the level dependency property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", (Type) typeof(int), (Type) typeof(RangeGroupHeaderButtonPresenter), new PropertyMetadata((int) 0));

        /// <summary>
        /// Creates a new instance of the control.
        /// </summary>
        public RangeGroupHeaderButtonPresenter()
        {
            base.DefaultStyleKey = typeof(RangeGroupHeaderButtonPresenter);
            base.IsTabStop = false;
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

        /// <summary>
        /// Gets or sets a value that indicates the range group level.
        /// </summary>
        public int Level
        {
            get { return  (int) ((int) base.GetValue(LevelProperty)); }
            set { base.SetValue(LevelProperty, (int) value); }
        }
    }
}

