#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 增加属性控制的TabItem
    /// </summary>
    public partial class Tab : TabItem, IPhonePage
    {
        #region 系统事件
        /// <summary>
        /// 开始拖动事件
        /// </summary>
        internal static BaseRoutedEvent DragStartedEvent;

        /// <summary>
        /// 拖拽移动事件
        /// </summary>
        internal static BaseRoutedEvent DragDeltaEvent;

        /// <summary>
        /// 拖动结束事件
        /// </summary>
        internal static BaseRoutedEvent DragCompletedEvent;

        /// <summary>
        /// Pin状态变化事件
        /// </summary>
        internal static BaseRoutedEvent PinChangeEvent;

#if UWP
        static Tab()
        {
            DragStartedEvent = EventManager.RegisterRoutedEvent(
                "DragStarted",
                RoutingStrategy.Bubble,
                typeof(DragInfoEventHandler),
                typeof(Tab));

            DragDeltaEvent = EventManager.RegisterRoutedEvent(
                "DragDelta",
                RoutingStrategy.Bubble,
                typeof(DragInfoEventHandler),
                typeof(Tab));

            DragCompletedEvent = EventManager.RegisterRoutedEvent(
                "DragCompleted",
                RoutingStrategy.Bubble,
                typeof(DragInfoEventHandler),
                typeof(Tab));

            PinChangeEvent = EventManager.RegisterRoutedEvent(
                "PinChange",
                RoutingStrategy.Tunnel,
                typeof(EventHandler<PinChangeEventArgs>),
                typeof(Tab));
        }
#endif
        #endregion

        #region 静态内容
        public readonly static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(Tab),
            new PropertyMetadata(Icons.个人信息));

        public readonly static DependencyProperty HideTitleBarProperty = DependencyProperty.Register(
            "HideTitleBar",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(
            "IsPinned",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(true, OnIsPinnedChanged));

        public static readonly DependencyProperty CanDockInCenterProperty = DependencyProperty.Register(
            "CanDockInCenter",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanFloatProperty = DependencyProperty.Register(
            "CanFloat",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanUserPinProperty = DependencyProperty.Register(
            "CanUserPin",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(true, OnUpdatePinButton));

        public static readonly DependencyProperty CanDockProperty = DependencyProperty.Register(
            "CanDock",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(true));

        public static readonly DependencyProperty IsFloatingProperty = DependencyProperty.Register(
            "IsFloating",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(false, OnUpdatePinButton));

        public static readonly DependencyProperty IsInCenterProperty = DependencyProperty.Register(
            "IsInCenter",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(false, OnUpdatePinButton));

        public static readonly DependencyProperty PinButtonVisibilityProperty = DependencyProperty.Register(
            "PinButtonVisibility",
            typeof(Visibility),
            typeof(Tab),
            new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty HeaderButtonTextProperty = DependencyProperty.Register(
            "HeaderButtonText",
            typeof(string),
            typeof(Tab),
            new PropertyMetadata("\uE022"));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu",
            typeof(Menu),
            typeof(Tab),
            new PropertyMetadata(null));

        static void OnIsPinnedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Tab)d).OnIsPinnedChanged();
        }

        static void OnUpdatePinButton(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Tab)d).UpdatePinButton();
        }
        #endregion

        #region 成员变量
        Grid _root;
        Stack<ITabContent> _naviCache;
        #endregion

        #region 构造方法
        public Tab()
        {
            // 和NaviWin, PageWin写法不同！uno设置Style同时OnApplyTemplate，Tab只在显示时才！
            if (AtSys.IsPhoneUI)
                DefaultStyleKey = typeof(Tab);
            else
                Style = (Style)Application.Current.Resources["WinTab"];
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置菜单
        /// </summary>
        public Menu Menu
        {
            get { return (Menu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// 获取设置图标名称，只在PhoneUI模式下的Tab页显示
        /// </summary>
        public Icons Icon
        {
            get { return (Icons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置是否隐藏标题栏
        /// </summary>
        public bool HideTitleBar
        {
            get { return (bool)GetValue(HideTitleBarProperty); }
            set { SetValue(HideTitleBarProperty, value); }
        }

        /// <summary>
        /// 获取设置是否已固定
        /// </summary>
        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        /// <summary>
        /// 获取设置是否可以停靠在中部
        /// </summary>
        public bool CanDockInCenter
        {
            get { return (bool)GetValue(CanDockInCenterProperty); }
            set { SetValue(CanDockInCenterProperty, value); }
        }

        /// <summary>
        /// 获取是否允许停靠
        /// </summary>
        public bool CanDock
        {
            get { return (bool)GetValue(CanDockProperty); }
            set { SetValue(CanDockProperty, value); }
        }

        /// <summary>
        /// 获取设置是否可以浮动
        /// </summary>
        public bool CanFloat
        {
            get { return (bool)GetValue(CanFloatProperty); }
            set { SetValue(CanFloatProperty, value); }
        }

        /// <summary>
        /// 获取设置是否允许固定
        /// </summary>
        public bool CanUserPin
        {
            get { return (bool)GetValue(CanUserPinProperty); }
            set { SetValue(CanUserPinProperty, value); }
        }

        /// <summary>
        /// 获取是否为浮动状态
        /// </summary>
        public bool IsFloating
        {
            get { return (bool)GetValue(IsFloatingProperty); }
            internal set { SetValue(IsFloatingProperty, value); }
        }

        /// <summary>
        /// 获取设置是否停靠在中部
        /// </summary>
        public bool IsInCenter
        {
            get { return (bool)GetValue(IsInCenterProperty); }
            internal set { SetValue(IsInCenterProperty, value); }
        }

        /// <summary>
        /// 获取设置父容器是否为ToolWindow
        /// </summary>
        public bool IsInWindow
        {
            get { return IsFloating; }
            internal set
            {
                if (IsFloating != value)
                {
                    IsFloating = value;
                }
            }
        }

        /// <summary>
        /// 获取是否显示Pin按钮，手机上为返回按钮，windows上为自动隐藏按钮
        /// </summary>
        public Visibility PinButtonVisibility
        {
            get { return (Visibility)GetValue(PinButtonVisibilityProperty); }
            set { SetValue(PinButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// 获取设置标题栏按钮字符
        /// </summary>
        public string HeaderButtonText
        {
            get { return (string)GetValue(HeaderButtonTextProperty); }
            set { SetValue(HeaderButtonTextProperty, value); }
        }

        /// <summary>
        /// 获取父容器
        /// </summary>
        public Tabs Container
        {
            get { return (Parent as Tabs); }
        }

        /// <summary>
        /// 所属窗口，PhoneUI时用
        /// </summary>
        public Win OwnerWin { get; internal set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 向前导航到
        /// </summary>
        /// <param name="p_tabContent"></param>
        public void NaviTo(ITabContent p_tabContent)
        {
            ITabContent current;
            if (p_tabContent == null || (current = Content as ITabContent) == null)
                return;

            if (AtSys.IsPhoneUI)
            {
                Tab tab = new Tab { OwnerWin = OwnerWin, Content = p_tabContent };
                PhonePage.Show(tab);
                return;
            }

            if (_naviCache == null)
            {
                _naviCache = new Stack<ITabContent>();
                // 内容切换动画
                var ls = new TransitionCollection();
                ls.Add(new ContentThemeTransition { VerticalOffset = 60 });
                Container.ContentTransitions = ls;
            }
            _naviCache.Push(current);
            Content = p_tabContent;
        }

        /// <summary>
        /// 返回
        /// </summary>
        public void GoBack()
        {
            if (AtSys.IsPhoneUI)
                InputManager.GoBack();
            else if (_naviCache != null && _naviCache.Count > 0)
                Content = _naviCache.Pop();
        }

        /// <summary>
        /// 从父容器中移除当前Tab
        /// </summary>
        public void RemoveFromParent()
        {
            if (Container != null)
            {
                ClearValue(TabItemPanel.SplitterChangeProperty);
                Container.RemoveItem(this);
            }
        }
        #endregion

        #region 实现接口
        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        Task<bool> IPhonePage.OnClosing()
        {
            // 只在首页时处理
            if (OwnerWin != null && OwnerWin.Home == Title)
                return ((IWin)OwnerWin).OnClosing();
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void IPhonePage.OnClosed()
        {
            // 只在首页时处理
            if (OwnerWin != null && OwnerWin.Home == Title)
                ((IWin)OwnerWin).OnClosed();
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            if (!AtSys.IsPhoneUI)
            {
                base.OnApplyTemplate();
                return;
            }

            if (PinButtonVisibility == Visibility.Visible)
            {
                AtUI.OnPhoneTitleTapped((Grid)GetTemplateChild("HeaderGrid"), OwnerWin);
                Button btn = GetTemplateChild("BackButton") as Button;
                if (btn != null)
                    btn.Tapped += InputManager.OnBackClick;
            }

            _root = (Grid)GetTemplateChild("RootGrid");
            OnContentChanged();
        }

        /// <summary>
        /// 切换内容
        /// </summary>
        protected override void OnContentChanged()
        {
            if (AtSys.IsPhoneUI)
            {
                // 为uno节省一级ContentPresenter！
                if (_root != null)
                {
                    if (_root.Children.Count > 1)
                        _root.Children.RemoveAt(1);
                    FrameworkElement con = Content as FrameworkElement;
                    if (con != null)
                    {
                        Grid.SetRow(con, 1);
                        _root.Children.Add(con);
                    }
                }
                // 自定义Tab内容
                if (Content is ITabContent tc)
                {
                    // 有独立的Menu和Title
                    tc.Tab = this;
                    Menu = tc.Menu;
                    if (!string.IsNullOrEmpty(tc.Title))
                        Title = tc.Title;
                    else if (string.IsNullOrEmpty(Title))
                        Title = "无标题";
                }
            }
            else
            {
                base.OnContentChanged();
                // 自定义Tab内容
                if (Content is ITabContent tc)
                {
                    if (_naviCache != null)
                    {
                        if (_naviCache.Count == 1)
                            HeaderButtonText = "\uE085";
                        else if (_naviCache.Count == 0)
                            HeaderButtonText = IsPinned ? "\uE022" : "\uE021";
                    }

                    // 有独立的Menu和Title
                    tc.Tab = this;
                    Menu = tc.Menu;
                    if (!string.IsNullOrEmpty(tc.Title))
                        Title = tc.Title;
                }
            }
        }

        /// <summary>
        /// 开始拖动标签
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartDrag(PointerRoutedEventArgs e)
        {
            if (IsPinned && CanFloat)
                this.StartDrag(e);
        }
        #endregion

        #region 内部方法
        internal void OnHeaderButtonClick()
        {
            if (_naviCache != null && _naviCache.Count > 0)
                Content = _naviCache.Pop();
            else
                IsPinned = !IsPinned;
        }

        void OnIsPinnedChanged()
        {
            HeaderButtonText = IsPinned ? "\uE022" : "\uE021";
            if (_isLoaded)
            {
                if (!IsPinned && (IsFloating || IsInCenter))
                    throw new InvalidOperationException("浮动或在中部区域时无法自动隐藏！");
                this.RaiseEvent(new PinChangeEventArgs(PinChangeEvent, this, IsPinned));
            }
        }

        void UpdatePinButton()
        {
            PinButtonVisibility = (CanUserPin && !IsInCenter && !IsFloating) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }

    /// <summary>
    /// 自定义Tab内容
    /// </summary>
    public interface ITabContent
    {
        Tab Tab { get; set; }

        Menu Menu { get; }

        string Title { get; }
    }
}
