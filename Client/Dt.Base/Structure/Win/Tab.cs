﻿#region 文件描述
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
    public partial class Tab : TabItem, IPhonePage, INaviHost
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

        public readonly static DependencyProperty PhoneBarProperty = DependencyProperty.Register(
            "PhoneBar",
            typeof(object),
            typeof(Tab),
            new PropertyMetadata(null, OnPhoneBarChanged));

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

        static void OnPhoneBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (AtSys.IsPhoneUI
                && e.NewValue is Button btn
                && btn.Style == null)
            {
                // 默认搜索按钮样式
                btn.Style = AtRes.PhonSearchButton;
            }
        }
        #endregion

        #region 构造方法
        public Tab()
        {
            // 两种UI模式样式不同，PhoneUI模式为缺省样式
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
        /// 获取设置Phone模式下的工具栏，一般为搜索栏
        /// </summary>
        public object PhoneBar
        {
            get { return GetValue(PhoneBarProperty); }
            set { SetValue(PhoneBarProperty, value); }
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
        /// 获取所属的Tabs
        /// </summary>
        public Tabs OwnTabs
        {
            get { return (Owner as Tabs); }
        }

        /// <summary>
        /// 所属Win
        /// </summary>
        internal Win OwnWin { get; set; }
        #endregion

        #region INaviHost
        Stack<INaviContent> _naviCache;

        /// <summary>
        /// 向前导航到新内容
        /// </summary>
        /// <param name="p_content"></param>
        void INaviHost.NaviTo(INaviContent p_content)
        {
            INaviContent current;
            if (p_content == null || (current = Content as INaviContent) == null)
                return;

            if (AtSys.IsPhoneUI)
            {
                Tab tab = new Tab { OwnWin = OwnWin, Content = p_content };
                PhonePage.Show(tab);
                return;
            }

            if (_naviCache == null)
            {
                _naviCache = new Stack<INaviContent>();
                // 内容切换动画
                var ls = new TransitionCollection();
                ls.Add(new ContentThemeTransition { VerticalOffset = 60 });
                OwnTabs.ContentTransitions = ls;
            }
            _naviCache.Push(current);
            Content = p_content;
        }

        /// <summary>
        /// 返回上一内容
        /// </summary>
        void INaviHost.GoBack()
        {
            if (AtSys.IsPhoneUI)
                InputManager.GoBack();
            else if (_naviCache != null && _naviCache.Count > 0)
                Content = _naviCache.Pop();
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
            if (OwnWin != null && OwnWin.Home == Title)
                return OwnWin.AllowClose();
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void IPhonePage.OnClosed()
        {
            // 只在首页时处理
            if (OwnWin != null && OwnWin.Home == Title)
                OwnWin.AfterClosed();
        }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            if (AtSys.IsPhoneUI)
                InitPhoneUITemplate();
            else
                base.OnLoadTemplate();
        }

        void InitPhoneUITemplate()
        {
            if (PinButtonVisibility == Visibility.Visible)
            {
                WinKit.OnPhoneTitleTapped((Grid)GetTemplateChild("HeaderGrid"), OwnWin);
                Button btn = GetTemplateChild("BackButton") as Button;
                if (btn != null)
                    btn.Click += InputManager.OnBackClick;
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 切换内容
        /// </summary>
        protected override void OnContentChanged()
        {
            if (!AtSys.IsPhoneUI)
                base.OnContentChanged();

            var navi = Content as INaviContent;
            if (navi == null)
                return;

            if (_naviCache != null)
            {
                if (_naviCache.Count == 1)
                    HeaderButtonText = "\uE085";
                else if (_naviCache.Count == 0)
                    HeaderButtonText = IsPinned ? "\uE022" : "\uE021";
            }

            // 外部可设置Menu和Title
            navi.AddToHost(this);
            if (string.IsNullOrEmpty(Title))
                Title = "无标题";
        }

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

        internal void OnHeaderButtonClick()
        {
            if (_naviCache != null)
            {
                // 避免连续返回时造成停靠的误操作，使第一次点击停靠时无效！
                if (_naviCache.Count > 0)
                    Content = _naviCache.Pop();
                else
                    _naviCache = null;
            }
            else
            {
                IsPinned = !IsPinned;
            }
        }

        void OnIsPinnedChanged()
        {
            HeaderButtonText = IsPinned ? "\uE022" : "\uE021";
            if (_isLoaded)
            {
                if (!IsPinned && (IsFloating || IsInCenter))
                    throw new InvalidOperationException("浮动或在中部区域时无法自动隐藏！");
                OwnWin.OnPinChange(this);
            }
        }

        void UpdatePinButton()
        {
            PinButtonVisibility = (CanUserPin && !IsInCenter && !IsFloating) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}
