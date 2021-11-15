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
using Windows.UI.Xaml.Data;
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
            new PropertyMetadata(true));

        public static readonly DependencyProperty CanDockProperty = DependencyProperty.Register(
            "CanDock",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(true));

        public static readonly DependencyProperty IsFloatingProperty = DependencyProperty.Register(
            "IsFloating",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsInCenterProperty = DependencyProperty.Register(
            "IsInCenter",
            typeof(bool),
            typeof(Tab),
            new PropertyMetadata(false));

        public static readonly DependencyProperty BackButtonVisibilityProperty = DependencyProperty.Register(
            "BackButtonVisibility",
            typeof(Visibility),
            typeof(Tab),
            new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu",
            typeof(Menu),
            typeof(Tab),
            new PropertyMetadata(null));

        public static readonly DependencyProperty OrderProperty = DependencyProperty.Register(
            "Order",
            typeof(int),
            typeof(Tab),
            new PropertyMetadata(0));

        public static readonly DependencyProperty ContentTransitionsProperty = DependencyProperty.Register(
            "ContentTransitions",
            typeof(TransitionCollection),
            typeof(Tab),
            new PropertyMetadata(null));

        static void OnIsPinnedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Tab)d).OnIsPinnedChanged();
        }
        #endregion

        #region 构造方法
        public Tab()
        {
            // 两种UI模式样式不同，PhoneUI模式为缺省样式
            if (Kit.IsPhoneUI)
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
        /// 获取设置Phone模式时 是否在首页显示 以及 显示次序
        /// <para>默认0表示不在首页显示</para>
        /// <para>大于0时在首页显示</para>
        /// <para>当有多个Tab显示在首页时，底部标签次序按值从小到大排列</para>
        /// </summary>
        public int Order
        {
            get { return (int)GetValue(OrderProperty); }
            set { SetValue(OrderProperty, value); }
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
        /// 获取设置是否隐藏标题栏，只在PhoneUI模式下有效
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
        /// 获取设置当前Tab是否允许其他浮动Tab停靠在中部
        /// </summary>
        public bool CanDockInCenter
        {
            get { return (bool)GetValue(CanDockInCenterProperty); }
            set { SetValue(CanDockInCenterProperty, value); }
        }

        /// <summary>
        /// 当前Tab是否允许停靠
        /// </summary>
        public bool CanDock
        {
            get { return (bool)GetValue(CanDockProperty); }
            set { SetValue(CanDockProperty, value); }
        }

        /// <summary>
        /// 获取设置当前Tab是否可以浮动
        /// </summary>
        public bool CanFloat
        {
            get { return (bool)GetValue(CanFloatProperty); }
            set { SetValue(CanFloatProperty, value); }
        }

        /// <summary>
        /// 获取设置是否允许在固定和自动隐藏之间切换
        /// </summary>
        public bool CanUserPin
        {
            get { return (bool)GetValue(CanUserPinProperty); }
            set { SetValue(CanUserPinProperty, value); }
        }

        /// <summary>
        /// 获取是否为浮动状态，浮动时父容器为ToolWindow
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
        /// 获取是否显示返回按钮，默认Visible，内部绑定用
        /// </summary>
        public Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            internal set { SetValue(BackButtonVisibilityProperty, value); }
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
        /// 获取所属的Tabs
        /// </summary>
        public Tabs OwnTabs
        {
            get { return (Owner as Tabs); }
        }

        /// <summary>
        /// 所属Win
        /// </summary>
        public Win OwnWin { get; internal set; }
        #endregion

        #region Mv
        Stack<Mv> _navCache;

        /// <summary>
        /// 向前导航到新内容
        /// </summary>
        /// <param name="p_content"></param>
        internal void Forward(Mv p_content)
        {
            Mv current;
            if (p_content == null || (current = Content as Mv) == null)
                return;

            if (Kit.IsPhoneUI && p_content.OwnDlg == null)
            {
                Tab tab = new Tab { OwnWin = OwnWin, Content = p_content };
                PhonePage.Show(tab);
                return;
            }

            if (_navCache == null)
            {
                _navCache = new Stack<Mv>();

                // 内容切换动画
                var ls = new TransitionCollection();
                ls.Add(new ContentThemeTransition { VerticalOffset = 60 });
                if (OwnTabs != null)
                    OwnTabs.ContentTransitions = ls;
                else if (Kit.IsPhoneUI)
                    ContentTransitions = ls;
            }
            _navCache.Push(current);
            Content = p_content;
        }

        /// <summary>
        /// 向后导航到上一内容
        /// </summary>
        internal async void Backward()
        {
            var mv = Content as Mv;
            if (mv == null)
            {
                // 普通内容
                if (Kit.IsPhoneUI)
                    InputManager.GoBack();
                return;
            }

            if (Kit.IsPhoneUI && mv.OwnDlg == null)
            {
                // 允许返回
                if (await mv.BeforeClose())
                {
                    InputManager.GoBack();
                    mv.AfterClosed();
                }
            }
            else if (_navCache != null && _navCache.Count > 0)
            {
                if (await mv.BeforeClose())
                {
                    Content = _navCache.Pop();
                    mv.AfterClosed();
                }
            }
            else if (mv.OwnDlg != null)
            {
                // 带遮罩的Mv
                if (await mv.BeforeClose())
                {
                    mv.OwnDlg.Close();
                    mv.AfterClosed();
                }
            }
        }

        /// <summary>
        /// 切换内容
        /// </summary>
        protected override void OnContentChanged()
        {
            if (!Kit.IsPhoneUI)
                base.OnContentChanged();

            var mv = Content as Mv;
            if (mv == null)
            {
                if (!Kit.IsPhoneUI)
                    BackButtonVisibility = Visibility.Collapsed;
                return;
            }

            if (mv.OwnDlg != null)
            {
                if (mv.OwnDlg.HideTitleBar
                    || (_navCache != null && _navCache.Count > 0))
                {
                    BackButtonVisibility = Visibility.Visible;
                }
                else
                {
                    BackButtonVisibility = Visibility.Collapsed;
                }
            }
            else if (!Kit.IsPhoneUI)
            {
                BackButtonVisibility = (_navCache != null && _navCache.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            }

            // 绑定Mv的依赖属性
            SetBinding(TitleProperty, new Binding { Path = new PropertyPath("Title"), Source = mv });
            SetBinding(MenuProperty, new Binding { Path = new PropertyPath("Menu"), Source = mv });
            if (Kit.IsPhoneUI)
                SetBinding(HideTitleBarProperty, new Binding { Path = new PropertyPath("HideTitleBar"), Source = mv });

            mv.AddToHost(this);
        }
        #endregion

        #region IPhonePage
        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        Task<bool> IPhonePage.OnClosing()
        {
            // 只在首页时处理
            if (Order > 0 && OwnWin != null)
                return OwnWin.AllowClose();
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void IPhonePage.OnClosed()
        {
            // 只在首页时处理
            if (Order > 0 && OwnWin != null)
                OwnWin.AfterClosed();
        }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            if (Kit.IsPhoneUI)
                InitPhoneUITemplate();
            else
                base.OnLoadTemplate();
        }

        void InitPhoneUITemplate()
        {
            if (!HideTitleBar)
            {
                if (OwnWin != null && BackButtonVisibility == Visibility.Visible)
                {
                    // 首页无标题右键菜单
                    WinKit.OnPhoneTitleTapped((Grid)GetTemplateChild("HeaderGrid"), OwnWin);
                }

                Button btn = GetTemplateChild("BackButton") as Button;
                if (btn != null)
                    btn.Click += (s, e) => Backward();
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 开始拖动标签
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartDrag(PointerRoutedEventArgs e)
        {
            if (IsPinned && CanFloat)
                OwnWin.OnDragStarted(this, e);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 从父容器中移除当前Tab
        /// </summary>
        internal void RemoveFromParent()
        {
            if (Owner != null)
            {
                ClearValue(TabItemPanel.SplitterChangeProperty);
                Owner.Items.Remove(this);
            }
        }

        void OnIsPinnedChanged()
        {
            if (_isLoaded)
            {
                if (!IsPinned && (IsFloating || IsInCenter))
                    throw new InvalidOperationException("浮动或在中部区域时无法自动隐藏！");
                OwnWin.OnPinChange(this);
            }
        }
        #endregion
    }
}
