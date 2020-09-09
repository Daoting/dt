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
    public partial class GroupButton : Button
    {
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", (Type) typeof(bool), (Type) typeof(GroupButton), new PropertyMetadata((bool) false, new PropertyChangedCallback(GroupButton.OnIsExpandedPropertyChanged)));

        public GroupButton()
        {
            DefaultStyleKey = typeof(GroupButton);
            Index = -1;
            Level = -1;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState(false);
        }

        static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupButton presenter = d as GroupButton;
            if (presenter != null)
            {
                presenter.UpdateVisualState(true);
            }
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

        public bool IsExpanded
        {
            get { return  (bool) ((bool) base.GetValue(IsExpandedProperty)); }
            internal set { base.SetValue(IsExpandedProperty, (bool) value); }
        }

        internal int Level { get; set; }
    }
}

