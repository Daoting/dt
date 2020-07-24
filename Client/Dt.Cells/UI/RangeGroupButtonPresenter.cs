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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> range group button
    /// that is used to expand or collapse the group.
    /// </summary>
    [TemplateVisualState(GroupName="GroupState", Name="Expanded"), TemplateVisualState(GroupName="GroupState", Name="Collapsed")]
    public partial class RangeGroupButtonPresenter : Button
    {
        /// <summary>
        /// Indicates IsExpanded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", (Type) typeof(bool), (Type) typeof(RangeGroupButtonPresenter), new PropertyMetadata((bool) false, new PropertyChangedCallback(RangeGroupButtonPresenter.OnIsExpandedPropertyChanged)));

        /// <summary>
        /// Creates a new instance of the control.
        /// </summary>
        public RangeGroupButtonPresenter()
        {
            base.DefaultStyleKey = typeof(RangeGroupButtonPresenter);
            base.IsTabStop = false;
            Index = -1;
            Level = -1;
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="M:FrameworkElement.ApplyTemplate" /> when overridden in a derived class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState(false);
        }

        static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeGroupButtonPresenter presenter = d as RangeGroupButtonPresenter;
            if (presenter != null)
            {
                presenter.UpdateVisualState(true);
            }
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

        void UpdateVisualState(bool useTransitions)
        {
            if (IsExpanded)
            {
                VisualStateManager.GoToState(this, "Expanded", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Collapsed", useTransitions);
            }
        }

        internal int Index { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the range group is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return  (bool) ((bool) base.GetValue(IsExpandedProperty)); }
            internal set { base.SetValue(IsExpandedProperty, (bool) value); }
        }

        internal int Level { get; set; }
    }
}

