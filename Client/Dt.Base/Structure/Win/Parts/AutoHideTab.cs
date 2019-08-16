#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 自动隐藏区域
    /// </summary>
    public partial class AutoHideTab : TabControl
    {
        #region 成员变量
        const double _margin = 10.0;
        const double _defaultSize = 350;
        #endregion

        #region 构造方法
        public AutoHideTab()
        {
            DefaultStyleKey = typeof(AutoHideTab);
            Loaded += OnLoaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取是否显示切换Outlook样式的按钮
        /// </summary>
        public Visibility OutlookButtonVisible
        {
            get { return Visibility.Collapsed; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 将指定Tab转为自动隐藏
        /// </summary>
        /// <param name="p_sectItem"></param>
        internal void Unpin(Tab p_sectItem)
        {
            if (p_sectItem.Container != null)
            {
                if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                {
                    if (p_sectItem.ReadLocalValue(TabItem.PopWidthProperty) == DependencyProperty.UnsetValue)
                        p_sectItem.PopWidth = p_sectItem.Container.ActualWidth;
                }
                else
                {
                    if (p_sectItem.ReadLocalValue(TabItem.PopHeightProperty) == DependencyProperty.UnsetValue)
                        p_sectItem.PopHeight = p_sectItem.Container.ActualHeight;
                }
                p_sectItem.RemoveFromParent();
            }
            else
            {
                if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                {
                    if (p_sectItem.ReadLocalValue(TabItem.PopWidthProperty) == DependencyProperty.UnsetValue)
                        p_sectItem.PopWidth = _defaultSize;
                }
                else
                {
                    if (p_sectItem.ReadLocalValue(TabItem.PopHeightProperty) == DependencyProperty.UnsetValue)
                        p_sectItem.PopHeight = _defaultSize;
                }
            }

            Items.Add(p_sectItem);
        }

        /// <summary>
        /// 将指定Tab转为固定停靠
        /// </summary>
        /// <param name="p_sectItem"></param>
        internal void Pin(Tab p_sectItem)
        {
            Items.Remove(p_sectItem);
            p_sectItem.ClearValue(TabItem.PopWidthProperty);
            p_sectItem.ClearValue(TabItem.PopHeightProperty);
        }

        /// <summary>
        /// 清空子项
        /// </summary>
        internal void Clear()
        {
            while (Items.Count > 0)
            {
                Pin(Items[0] as Tab);
            }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TabHeader header = GetTemplateChild("HeaderElement") as TabHeader;
            if (header != null)
                header.Owner = this;
        }

        /// <summary>
        /// 增删子项
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            if (_isLoaded)
                ResetMargin();
        }

        /// <summary>
        /// IsAutoHide变化时不做操作
        /// </summary>
        protected override void OnIsAutoHideChanged()
        {
        }

        protected override object LoadDlgContent()
        {
            StackPanel sp = new StackPanel();
            TabHeader header = new TabHeader();
            header.Owner = this;
            sp.Children.Add(header);
            ContentPresenter content = new ContentPresenter()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            Binding contentBinding = new Binding() { Path = new PropertyPath("SelectedContent"), Source = this };
            content.SetBinding(ContentPresenter.ContentProperty, contentBinding);
            sp.Children.Add(content);
            return sp;
        }
        #endregion

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            ResetMargin();
        }

        void ResetMargin()
        {
            if (Items.Count == 0)
            {
                Margin = new Thickness(0);
            }
            else
            {
                if (TabStripPlacement == ItemPlacement.Left)
                    Margin = new Thickness(0, 0, _margin, 0);
                else if (TabStripPlacement == ItemPlacement.Right)
                    Margin = new Thickness(_margin, 0, 0, 0);
                else if (TabStripPlacement == ItemPlacement.Bottom)
                    Margin = new Thickness(0, _margin, 0, 0);
                else if (TabStripPlacement == ItemPlacement.Top)
                    Margin = new Thickness(0, 0, 0, _margin);
            }
        }
    }
}

