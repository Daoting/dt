#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
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
#if !UWP
            Loaded += OnLoaded;
#endif
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 将指定Tab转为自动隐藏
        /// </summary>
        /// <param name="p_tab"></param>
        internal void Unpin(Tab p_tab)
        {
            if (p_tab.OwnTabs != null)
            {
                if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                {
                    if (p_tab.ReadLocalValue(TabItem.PopWidthProperty) == DependencyProperty.UnsetValue)
                        p_tab.PopWidth = p_tab.OwnTabs.ActualWidth;
                }
                else
                {
                    if (p_tab.ReadLocalValue(TabItem.PopHeightProperty) == DependencyProperty.UnsetValue)
                        p_tab.PopHeight = p_tab.OwnTabs.ActualHeight;
                }
                p_tab.RemoveFromParent();
            }
            else
            {
                if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                {
                    if (p_tab.ReadLocalValue(TabItem.PopWidthProperty) == DependencyProperty.UnsetValue)
                        p_tab.PopWidth = _defaultSize;
                }
                else
                {
                    if (p_tab.ReadLocalValue(TabItem.PopHeightProperty) == DependencyProperty.UnsetValue)
                        p_tab.PopHeight = _defaultSize;
                }
            }
            p_tab.IsPinned = false;
            Items.Add(p_tab);
        }

        /// <summary>
        /// 将指定Tab转为固定停靠
        /// </summary>
        /// <param name="p_tab"></param>
        internal void Pin(Tab p_tab)
        {
            Items.Remove(p_tab);
            p_tab.ClearValue(TabItem.PopWidthProperty);
            p_tab.ClearValue(TabItem.PopHeightProperty);
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
        /// <summary>
        /// 增删子项
        /// </summary>
        protected override void OnItemsChanged()
        {
            ResetMargin();
        }

        protected override object LoadDlgContent()
        {
            StackPanel sp = new StackPanel();

            Grid g = new Grid
            { 
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, new ColumnDefinition { Width = GridLength.Auto } },
                BorderBrush = Res.浅灰2,
                BorderThickness = new Thickness(1),
                Background = Res.浅灰1
            };
            TabHeader header = new TabHeader { BorderThickness = new Thickness(0), Height = Res.RowInnerHeight, Owner = this };
            g.Children.Add(header);
            Button btn = new Button { Content = "\uE027", Style = Res.字符按钮, Foreground = Res.深灰1 };
            btn.Click += OnPinClick;
            Grid.SetColumn(btn, 1);
            g.Children.Add(btn);
            sp.Children.Add(g);

            ContentPresenter content = new ContentPresenter()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            // 内容切换动画
            var ls = new TransitionCollection();
            ls.Add(new ContentThemeTransition { VerticalOffset = 60 });
            content.ContentTransitions = ls;
            Binding contentBinding = new Binding() { Path = new PropertyPath("SelectedContent"), Source = this };
            content.SetBinding(ContentPresenter.ContentProperty, contentBinding);
            sp.Children.Add(content);
            return sp;
        }

        void OnPinClick(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is Tab tab)
                tab.IsPinned = true;
        }
        #endregion

        #region 加载过程
#if UWP
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitTemplate();
        }
#else
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            InitTemplate();
        }
#endif

        void InitTemplate()
        {
            TabHeader header = GetTemplateChild("HeaderElement") as TabHeader;
            if (header != null)
                header.Owner = this;
            ResetMargin();
        }
        #endregion

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

