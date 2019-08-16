#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 选项卡控件
    /// </summary>
    public partial class TabControl : ItemsControl
    {
        #region 静态内容
        /// <summary>
        /// TabItem 标题相对于内容的对齐方式
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register(
            "TabStripPlacement",
            typeof(ItemPlacement),
            typeof(TabControl),
            new PropertyMetadata(ItemPlacement.Bottom, new PropertyChangedCallback(OnTabStripPlacementChanged)));

        /// <summary>
        /// TabItem是否可拖拽调整位置
        /// </summary>
        public readonly static DependencyProperty AllowSwapItemProperty = DependencyProperty.Register(
            "AllowSwapItem",
            typeof(bool),
            typeof(TabControl),
            new PropertyMetadata(true));

        /// <summary>
        /// 点击标签时是否以弹出方式显示内容，平时自动隐藏
        /// </summary>
        public readonly static DependencyProperty IsAutoHideProperty = DependencyProperty.Register(
            "IsAutoHide",
            typeof(bool),
            typeof(TabControl),
            new PropertyMetadata(false, OnIsAutoHideChanged));

        /// <summary>
        /// 是否采用outlook导航样式
        /// </summary>
        public readonly static DependencyProperty IsOutlookStyleProperty = DependencyProperty.Register(
            "IsOutlookStyle",
            typeof(bool),
            typeof(TabControl),
            new PropertyMetadata(false, OnTabStripPlacementChanged));

        /// <summary>
        /// 当前选定的 TabItem 的索引
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            "SelectedIndex",
            typeof(int),
            typeof(TabControl),
            new CoercePropertyMetadata(-1, OnSelectedIndexChanged, CoerceSelectedIndex));

        /// <summary>
        /// 当前选定的 TabItem
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(TabControl),
            new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// 面板内容的宽度
        /// </summary>
        public static readonly DependencyProperty PopWidthProperty = DependencyProperty.Register(
            "PopWidth",
            typeof(double),
            typeof(TabControl),
            new PropertyMetadata(double.NaN, OnPopSizeChanged));

        /// <summary>
        /// 面板内容的高度
        /// </summary>
        public static readonly DependencyProperty PopHeightProperty = DependencyProperty.Register(
            "PopHeight",
            typeof(double),
            typeof(TabControl),
            new PropertyMetadata(double.NaN, OnPopSizeChanged));

        /// <summary>
        /// 内容模板
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
            "ContentTemplate",
            typeof(object),
            typeof(TabControl),
            new PropertyMetadata(null));

        /// <summary>
        /// 切换内容时的转换
        /// </summary>
        public static readonly DependencyProperty ContentTransitionsProperty = DependencyProperty.Register(
            "ContentTransitions",
            typeof(TransitionCollection),
            typeof(TabControl),
            new PropertyMetadata(null));

        /// <summary>
        /// 当前选择的 TabItem 的内容
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register(
            "SelectedContent",
            typeof(object),
            typeof(TabControl),
            new PropertyMetadata(null));

        /// <summary>
        /// 默认控件模板
        /// </summary>
        public static readonly DependencyProperty DefaultTemplateProperty = DependencyProperty.Register(
            "DefaultTemplate",
            typeof(ControlTemplate),
            typeof(TabControl),
            new PropertyMetadata(null));

        /// <summary>
        /// 弹出式模板
        /// </summary>
        public static readonly DependencyProperty PopTemplateProperty = DependencyProperty.Register(
            "PopTemplate",
            typeof(ControlTemplate),
            typeof(TabControl),
            new PropertyMetadata(null, OnPopTemplateChanged));

        static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            if (!tab._selector.Locked && tab._isLoaded)
            {
                int newValue = (int)e.NewValue;
                if (newValue == -1)
                {
                    tab._selector.Select(null);
                }
                else if (newValue > -1 && newValue < tab.Items.Count)
                {
                    tab._selector.Select(tab.ContainerFromIndex(newValue));
                }
            }
        }

        static object CoerceSelectedIndex(DependencyObject d, object value)
        {
            TabControl tab = (TabControl)d;
            if (!tab._updatingSelection
                && (value is int)
                && (((int)value) >= tab.Items.Count || ((int)value) < -1))
            {
                return -1;
            }
            return value;
        }

        static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            if (!tab._selector.Locked && tab._isLoaded)
            {
                if (e.NewValue != null)
                {
                    tab._selector.Select(tab.ContainerFromItem(e.NewValue));
                }
                else
                {
                    tab._selector.Select(null);
                }
            }
        }

        static void OnTabStripPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            tab.OnPlacementChanged();
            if (tab._isLoaded)
                tab.AffirmOutLook();
        }

        static void OnIsAutoHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            tab.OnIsAutoHideChanged();
        }

        static void OnPopSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            tab.ApplyPopStyle();
        }

        static void OnPopTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            ControlTemplate temp = (ControlTemplate)e.NewValue;
            if (tab.IsAutoHide && temp != null)
                tab.Template = temp;
        }
        #endregion

        #region 成员变量
        SelectionChanger _selector;

        StackPanel _panel;
        Grid _mainGrid;
        ItemsPresenter _strip;
        Grid _contentGrid;
        Dlg _dlg;

        protected bool _isLoaded;
        bool _updatingSelection;
        bool _swapping;
        #endregion

        #region 事件
        /// <summary>
        /// 当选定的 TabItem 更改时发生
        /// </summary>
        public event EventHandler<SelectedChangedEventArgs> SelectedChanged;

        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public TabControl()
        {
            DefaultStyleKey = typeof(TabControl);
            _selector = new SelectionChanger(this);
            Loaded += OnLoaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置 TabItem 标题相对于内容的对齐方式
        /// </summary>
        public ItemPlacement TabStripPlacement
        {
            get { return (ItemPlacement)GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        /// <summary>
        /// 获取设置TabItem是否可拖拽调整位置
        /// </summary>
        public bool AllowSwapItem
        {
            get { return (bool)GetValue(AllowSwapItemProperty); }
            set { SetValue(AllowSwapItemProperty, value); }
        }

        /// <summary>
        /// 获取设置点击标签时是否以弹出方式显示内容，平时自动隐藏
        /// </summary>
        public bool IsAutoHide
        {
            get { return (bool)GetValue(IsAutoHideProperty); }
            set { SetValue(IsAutoHideProperty, value); }
        }

        /// <summary>
        /// 获取设置是否采用outlook导航样式，只在标签在上下侧时有效
        /// </summary>
        public bool IsOutlookStyle
        {
            get { return (bool)GetValue(IsOutlookStyleProperty); }
            set { SetValue(IsOutlookStyleProperty, value); }
        }

        /// <summary>
        /// 获取设置弹出面板的宽度
        /// </summary>
        public double PopWidth
        {
            get { return (double)GetValue(PopWidthProperty); }
            set { SetValue(PopWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置弹出面板的高度
        /// </summary>
        public double PopHeight
        {
            get { return (double)GetValue(PopHeightProperty); }
            set { SetValue(PopHeightProperty, value); }
        }

        /// <summary>
        /// 获取或设置当前选定的 TabItem 的索引
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)((int)GetValue(SelectedIndexProperty)); }
            set { SetValue(SelectedIndexProperty, (int)value); }
        }

        /// <summary>
        /// 获取或设置当前选定的 TabItem
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 获取设置当前选择的 TabItem 的内容
        /// </summary>
        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }
            internal set { SetValue(SelectedContentProperty, value); }
        }

        /// <summary>
        /// 获取或设置内容模板
        /// </summary>
        public object ContentTemplate
        {
            get { return GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// 获取或设置切换内容时的转换
        /// </summary>
        public TransitionCollection ContentTransitions
        {
            get { return (TransitionCollection)GetValue(ContentTransitionsProperty); }
            set { SetValue(ContentTransitionsProperty, value); }
        }

        /// <summary>
        /// 获取设置默认控件模板
        /// </summary>
        public ControlTemplate DefaultTemplate
        {
            get { return (ControlTemplate)GetValue(DefaultTemplateProperty); }
            set { SetValue(DefaultTemplateProperty, value); }
        }

        /// <summary>
        /// 获取设置弹出式模板
        /// </summary>
        public ControlTemplate PopTemplate
        {
            get { return (ControlTemplate)GetValue(PopTemplateProperty); }
            set { SetValue(PopTemplateProperty, value); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 内部元素拖拽过程
        /// </summary>
        /// <param name="p_src">被拖拽元素</param>
        /// <param name="p_pt">鼠标位置</param>
        /// <returns>false 表示不在有效区域</returns>
        internal bool DoSwap(TabItem p_src, Point p_pt)
        {
            TabItem target = null;
            foreach (object item in Items)
            {
                TabItem tabItem = ContainerFromItem(item) as TabItem;
                if (tabItem != null && tabItem.ContainPoint(p_pt))
                {
                    target = tabItem;
                    break;
                }
            }

            // 交换位置
            if (target != null && target != p_src)
            {
                //防止两元素宽度不一致交换时的抖动问题
                if (p_src.ActualWidth < target.ActualWidth)
                {
                    if (IsCriticalPoint(target, p_src, p_pt))
                        return true;
                }

                _swapping = true;
                SetupTransitions();
                int srcIndex = IndexFromContainer(p_src);
                int tgtIndex = IndexFromContainer(target);
                Items.RemoveAt(tgtIndex);
                Items.Insert(srcIndex, target);
                _swapping = false;
            }
            return target != null;
        }

        /// <summary>
        /// 当前鼠标是否在两个待交换标签的临界处
        /// </summary>
        /// <param name="p_tgt">目标</param>
        /// <param name="p_src">源</param>
        /// <param name="p_pt"></param>
        /// <returns></returns>
        public static bool IsCriticalPoint(Control p_tgt, Control p_src, Point p_pt)
        {
            bool critical = false;
            double width = p_tgt.ActualWidth - p_src.ActualWidth;
            MatrixTransform mtTgt = p_tgt.TransformToVisual(null) as MatrixTransform;
            MatrixTransform mtSrc = p_src.TransformToVisual(null) as MatrixTransform;
            if (mtTgt != null && mtSrc != null)
            {
                //源在目标的前面
                if (mtSrc.Matrix.OffsetX < mtTgt.Matrix.OffsetX)
                {
                    if ((mtSrc.Matrix.OffsetX + width) > p_pt.X)
                        critical = true;
                }
                else
                {
                    if ((mtSrc.Matrix.OffsetX - width) < p_pt.X)
                        critical = true;
                }
            }
            return critical;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mainGrid = GetTemplateChild("MainGrid") as Grid;
            _strip = (ItemsPresenter)GetTemplateChild("ItemsPresenter");
            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);

            if (_isLoaded && !_swapping)
            {
                IVectorChangedEventArgs args = (IVectorChangedEventArgs)e;
                if (IsAutoHide)
                {
                    _selector.Select(null);
                }
                else
                {
                    int index = (int)args.Index;
                    if (args.CollectionChange == CollectionChange.ItemInserted)
                    {
                        _selector.Select(ContainerFromIndex(index));
                    }
                    else
                    {
                        _selector.ResetSelection();
                    }
                }
                AffirmOutLook();
            }
        }

        /// <summary>
        /// 准备指定元素以显示指定项
        /// </summary>
        /// <param name="p_element"></param>
        /// <param name="p_item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject p_element, object p_item)
        {
            TabItem tabItem = p_element as TabItem;
            if (tabItem != null)
            {
                tabItem.TabStripPlacement = TabStripPlacement;
                Binding bind = new Binding();
                bind.Path = new PropertyPath("BorderBrush");
                bind.Source = this;
                tabItem.SetBinding(Control.BorderBrushProperty, bind);
            }

            if (p_item is TabItem)
            {
                base.PrepareContainerForItemOverride(tabItem, p_item);
            }
            else
            {
                if (tabItem != null)
                {
                    if (!string.IsNullOrEmpty(DisplayMemberPath))
                    {
                        if ((ItemTemplate == null) && (ItemTemplateSelector == null))
                        {
                            PropertyInfo runtimeProperty = RuntimeReflectionExtensions.GetRuntimeProperty(p_item.GetType(), DisplayMemberPath);
                            if (runtimeProperty != null)
                            {
                                object obj = runtimeProperty.GetValue(p_item);
                                if (obj != null)
                                {
                                    tabItem.Title = obj.ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        tabItem.Title = p_item.ToString();
                    }
                    tabItem.Content = p_item;
                }
                base.PrepareContainerForItemOverride(tabItem, tabItem);
            }
        }

        /// <summary>
        /// 创建或标识用于显示给定项的元素
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItem();
        }

        /// <summary>
        /// 确定指定项是否是其自己的容器
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is TabItem);
        }

        /// <summary>
        /// 清除子项
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            TabItem tabItem = element as TabItem;
            if (tabItem != null)
            {
                tabItem.IsSelected = false;
            }
        }
        #endregion

        #region 虚方法
        /// <summary>
        /// 触发选择变化事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(SelectedChangedEventArgs e)
        {

            SelectedChanged?.Invoke(this, e);
        }

        /// <summary>
        /// IsAutoHide变化时切换样式
        /// </summary>
        protected virtual void OnIsAutoHideChanged()
        {
            if (IsAutoHide && PopTemplate != null)
                Template = PopTemplate;
            else if (!IsAutoHide && DefaultTemplate != null)
                Template = DefaultTemplate;

            if (_isLoaded)
            {
                SelectedIndex = -1;
                Loaded += OnLoaded;
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded += OnTabUnloaded;
            SizeChanged += OnSizeChanged;
            Loaded += (s, args) => _isLoaded = true;
            _isLoaded = true;

            AffirmOutLook();
            _panel = this.FindChildByType<StackPanel>();
            OnPlacementChanged();

            // 初始选择状态，优先级：SelectedIndex > SelectedItem > TabItem.IsSelected
            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                _selector.Select(ContainerFromIndex(SelectedIndex));
            }
            else if (SelectedItem != null)
            {
                _selector.Select(ContainerFromItem(SelectedItem));
            }
            else if (!IsAutoHide && Items.Count > 0)
            {
                TabItem item = null;
                for (int i = 0; i < Items.Count; i++)
                {
                    item = ContainerFromIndex(i) as TabItem;
                    if (item != null && item.IsSelected)
                        break;
                    item = null;
                }
                if (item != null)
                    _selector.Select(item);
                else
                    _selector.Select(ContainerFromIndex(0));
            }
        }

        /// <summary>
        /// 卸载时移除动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabUnloaded(object sender, RoutedEventArgs e)
        {
            if (ItemContainerTransitions != null)
                ItemContainerTransitions = null;
            _isLoaded = false;
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isLoaded)
                AffirmOutLook();
        }

        /// <summary>
        /// 弹出面板关闭时清除选择项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPopClosed(object sender, object e)
        {
            SelectedIndex = -1;
        }

        /// <summary>
        /// 切换标签相对于内容的对齐方式
        /// </summary>
        void OnPlacementChanged()
        {
            if (!_isLoaded)
                return;

            if (TabStripPlacement == ItemPlacement.Left)
            {
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Star);

                Grid.SetColumn(_strip, 0);
                Grid.SetColumn(_contentGrid, 1);
                Grid.SetColumnSpan(_strip, 1);
                Grid.SetColumnSpan(_contentGrid, 1);

                Grid.SetRow(_strip, 0);
                Grid.SetRow(_contentGrid, 0);
                Grid.SetRowSpan(_strip, 2);
                Grid.SetRowSpan(_contentGrid, 2);
            }
            else if (TabStripPlacement == ItemPlacement.Right)
            {
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Star);

                Grid.SetColumn(_strip, 1);
                Grid.SetColumn(_contentGrid, 0);
                Grid.SetColumnSpan(_strip, 1);
                Grid.SetColumnSpan(_contentGrid, 1);

                Grid.SetRow(_strip, 0);
                Grid.SetRow(_contentGrid, 0);
                Grid.SetRowSpan(_strip, 2);
                Grid.SetRowSpan(_contentGrid, 2);
            }
            else if (TabStripPlacement == ItemPlacement.Bottom)
            {
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);

                Grid.SetRow(_strip, 1);
                Grid.SetRow(_contentGrid, 0);
                Grid.SetRowSpan(_strip, 1);
                Grid.SetRowSpan(_contentGrid, 1);

                Grid.SetColumn(_strip, 0);
                Grid.SetColumn(_contentGrid, 0);
                Grid.SetColumnSpan(_strip, 2);
                Grid.SetColumnSpan(_contentGrid, 2);
            }
            else if (TabStripPlacement == ItemPlacement.Top)
            {
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);

                Grid.SetRow(_strip, 0);
                Grid.SetRow(_contentGrid, 1);
                Grid.SetRowSpan(_strip, 1);
                Grid.SetRowSpan(_contentGrid, 1);

                Grid.SetColumn(_strip, 0);
                Grid.SetColumn(_contentGrid, 0);
                Grid.SetColumnSpan(_strip, 2);
                Grid.SetColumnSpan(_contentGrid, 2);
            }

            ApplyPopStyle();
            if (_panel != null)
            {
                if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                    _panel.Orientation = Orientation.Vertical;
                else
                    _panel.Orientation = IsOutlookStyle ? Orientation.Vertical : Orientation.Horizontal;
            }

            for (int i = 0; i < Items.Count; i++)
            {
                TabItem item = ContainerFromIndex(i) as TabItem;
                if (item != null)
                {
                    item.TabStripPlacement = TabStripPlacement;
                }
            }
        }

        /// <summary>
        /// 弹出式标签属性设置
        /// </summary>
        internal void ApplyPopStyle()
        {
            if (!IsAutoHide || _dlg == null)
                return;

            if (TabStripPlacement == ItemPlacement.Left)
            {
                _dlg.PhonePlacement = DlgPlacement.FromRight;
                _dlg.WinPlacement = DlgPlacement.FromRight;
                _dlg.Height = double.NaN;
                _dlg.Width = PopWidth;
            }
            else if (TabStripPlacement == ItemPlacement.Right)
            {
                _dlg.PhonePlacement = DlgPlacement.FromLeft;
                _dlg.WinPlacement = DlgPlacement.FromLeft;
                _dlg.Height = double.NaN;
                _dlg.Width = PopWidth;
            }
            else if (TabStripPlacement == ItemPlacement.Bottom)
            {
                _dlg.PhonePlacement = DlgPlacement.FromTop;
                _dlg.WinPlacement = DlgPlacement.FromTop;
                _dlg.Height = PopHeight;
                _dlg.Width = double.NaN;
            }
            else if (TabStripPlacement == ItemPlacement.Top)
            {
                _dlg.PhonePlacement = DlgPlacement.FromBottom;
                _dlg.WinPlacement = DlgPlacement.FromBottom;
                _dlg.Height = PopHeight;
                _dlg.Width = double.NaN;
            }
        }

        /// <summary>
        /// 添加ReorderThemeTransition
        /// </summary>
        void SetupTransitions()
        {
            if (ItemContainerTransitions == null)
            {
                ItemContainerTransitions = new TransitionCollection();
                ItemContainerTransitions.Add(new ReorderThemeTransition());
            }
        }

        /// <summary>
        /// 自适应outlook模式
        /// </summary>
        void AffirmOutLook()
        {
            // 20 是默认stripborder的留白
            double max = 20;
            Size size = this.GetSize();

            if (TabStripPlacement == ItemPlacement.Bottom || TabStripPlacement == ItemPlacement.Top)
            {
                foreach (TabItem item in Items)
                {
                    if (item.ActualWidth <= 0)
                        item.Measure(size);
                    max += item.DesiredSize.Width;
                }
                IsOutlookStyle = ActualWidth < max;
            }
            else
            {
                foreach (TabItem item in Items)
                {
                    if (item.ActualHeight <= 0)
                        item.Measure(size);
                    max += item.DesiredSize.Height;
                }
                IsOutlookStyle = ActualHeight < max;
            }
        }
        #endregion

        #region 选择相关
        /// <summary>
        /// 处理给定项容器的选择状态
        /// </summary>
        /// <param name="p_container"></param>
        /// <param name="p_selected"></param>
        internal void NotifyIsSelectedChanged(object p_container, bool p_selected)
        {
            if (_isLoaded && !_selector.Locked && p_container != null)
            {
                if (p_selected)
                {
                    _selector.Select(p_container);
                }
                else
                {
                    _selector.Unselect(p_container);
                }
            }
        }

        /// <summary>
        /// 重置SelectedIndex, SelectedItem
        /// </summary>
        void UpdateSelectionProperties()
        {
            try
            {
                _updatingSelection = true;

                // 内部实际的选项索引
                int curIndex = -1;
                if (_selector.Selection != null)
                    curIndex = Items.IndexOf(_selector.Selection);
                if (SelectedIndex != curIndex)
                    SetValue(SelectedIndexProperty, curIndex);

                if (SelectedItem != _selector.Selection)
                    SetValue(SelectedItemProperty, _selector.Selection);
            }
            finally
            {
                _updatingSelection = false;
            }
        }

        /// <summary>
        /// 设置指定项的IsSelected值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        void SetItemIsSelected(object item, bool value)
        {
            if (item != null)
            {
                TabItem element = ContainerFromItem(item) as TabItem;
                if (element != null && element.IsSelected != value)
                    element.IsSelected = value;
            }
        }

        void CreateDlg()
        {
            _dlg = new Dlg() { ClipElement = _strip, PlacementTarget = _strip, Resizeable = false, HideTitleBar = true };
            _dlg.Content = LoadDlgContent();
            switch (TabStripPlacement)
            {
                case ItemPlacement.Left:
                    _dlg.WinPlacement = DlgPlacement.TargetTopRight;
                    break;
                case ItemPlacement.Top:
                    _dlg.WinPlacement = DlgPlacement.TargetBottomLeft;
                    break;
                case ItemPlacement.Right:
                    _dlg.WinPlacement = DlgPlacement.TargetOuterLeftTop;
                    break;
                case ItemPlacement.Bottom:
                    _dlg.WinPlacement = DlgPlacement.TargetOuterTop;
                    break;
                default:
                    break;
            }
            _dlg.Closed += OnPopClosed;
        }

        protected virtual object LoadDlgContent()
        {
            ContentPresenter content = new ContentPresenter()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            Binding contentBinding = new Binding() { Path = new PropertyPath("SelectedContent"), Source = this };
            content.SetBinding(ContentPresenter.ContentProperty, contentBinding);
            return content;
        }

        /// <summary>
        /// 选择变化后的处理
        /// </summary>
        /// <param name="p_unselectItem"></param>
        /// <param name="p_selectItem"></param>
        void AfterSelectionChanged(object p_unselectItem, object p_selectItem)
        {
            // 设置内容
            TabItem item = ContainerFromItem(SelectedItem) as TabItem;
            if (item != null)
            {
                SelectedContent = item.Content;
                // 弹出式
                if (IsAutoHide)
                {
                    if (_dlg == null)
                        CreateDlg();

                    if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                    {
                        if (item.ReadLocalValue(TabItem.PopWidthProperty) != DependencyProperty.UnsetValue)
                        {
                            _dlg.Width = item.PopWidth;
                            _dlg.Height = _strip.ActualHeight;
                        }
                    }
                    else
                    {
                        if (item.ReadLocalValue(TabItem.PopHeightProperty) != DependencyProperty.UnsetValue)
                        {
                            _dlg.Height = item.PopHeight;
                            _dlg.Width = _strip.ActualWidth;
                        }
                    }

                    _dlg.Show();
                }
            }
            else
            {
                SelectedContent = null;
                if (IsAutoHide && _dlg != null)
                    _dlg.Close();
            }

            OnSelectionChanged(new SelectedChangedEventArgs(p_unselectItem, p_selectItem));
        }

        /// <summary>
        /// 管理选择项变化
        /// </summary>
        internal class SelectionChanger
        {
            TabControl _tab;
            bool _locked;
            object _internalSelection;
            object _itemToSelect;
            object _itemToUnselect;

            public SelectionChanger(TabControl p_tab)
            {
                _tab = p_tab;
                _itemToSelect = null;
                _itemToUnselect = null;
                _internalSelection = null;
                _locked = false;
            }

            /// <summary>
            /// 选择指定项
            /// </summary>
            /// <param name="p_item"></param>
            internal void Select(object p_item)
            {
                if (_internalSelection != p_item)
                {
                    Begin();
                    _itemToSelect = p_item;
                    _itemToUnselect = _internalSelection;
                    End();
                }
            }

            /// <summary>
            /// 取消指定项的选择状态
            /// </summary>
            /// <param name="p_item"></param>
            internal void Unselect(object p_item)
            {
                if (p_item != null && _internalSelection == p_item)
                {
                    Begin();
                    _itemToUnselect = _internalSelection;
                    foreach (object item in _tab.Items)
                    {
                        DependencyObject con = _tab.ContainerFromItem(item);
                        if (con != _internalSelection)
                        {
                            _itemToSelect = con;
                            break;
                        }
                    }
                    End();
                }
            }

            /// <summary>
            /// 重置有效的标签
            /// </summary>
            internal void ResetSelection()
            {
                if (_tab.Items.Count == 0)
                    return;

                bool exist = (from item in _tab.Items
                              let con = _tab.ContainerFromItem(item)
                              where con == _internalSelection
                              select item).Any();
                if (!exist)
                {
                    Begin();
                    _itemToSelect = _tab.ContainerFromIndex(0);
                    _itemToUnselect = _internalSelection;
                    End();
                }
            }

            void Begin()
            {
                _locked = true;
                _itemToSelect = null;
                _itemToUnselect = null;
            }

            void End()
            {
                try
                {
                    SynchronizeSelection();
                    _tab.UpdateSelectionProperties();
                }
                finally
                {
                    _locked = false;
                    if (_itemToUnselect != null || _itemToSelect != null)
                    {
                        _tab.AfterSelectionChanged(_itemToUnselect, _itemToSelect);
                    }
                }
            }

            /// <summary>
            /// 同步现有的选择项
            /// </summary>
            void SynchronizeSelection()
            {
                if (_itemToUnselect != null)
                    _tab.SetItemIsSelected(_itemToUnselect, false);
                if (_itemToSelect != null)
                    _tab.SetItemIsSelected(_itemToSelect, true);
                _internalSelection = _itemToSelect;
            }

            internal bool Locked
            {
                get { return _locked; }
            }

            internal object Selection
            {
                get { return _internalSelection; }
            }
        }
        #endregion
    }

    /// <summary>
    /// 选择项变化事件参数
    /// </summary>
    public class SelectedChangedEventArgs : EventArgs
    {
        object _unselectItem;
        object _selectItem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_unselect"></param>
        /// <param name="p_select"></param>
        public SelectedChangedEventArgs(object p_unselect, object p_select)
        {
            _unselectItem = p_unselect;
            _selectItem = p_select;
        }

        /// <summary>
        /// 获取要取消的选择项
        /// </summary>
        public object UnselectItem
        {
            get { return _unselectItem; }
        }

        /// <summary>
        /// 获取新的选择项
        /// </summary>
        public object SelectItem
        {
            get { return _selectItem; }
        }
    }
}
