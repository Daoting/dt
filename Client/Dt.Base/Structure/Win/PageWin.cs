#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单页面窗口
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class PageWin : Control, IWin
    {
        #region 静态成员
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(PageWin),
           new PropertyMetadata("无标题"));

        public readonly static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(PageWin),
            new PropertyMetadata(Icons.文件));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu",
            typeof(Menu),
            typeof(PageWin),
            new PropertyMetadata(null));

        public readonly static DependencyProperty HideTitleBarProperty = DependencyProperty.Register(
            "HideTitleBar",
            typeof(bool),
            typeof(PageWin),
            new PropertyMetadata(false));

        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(PageWin),
            new PropertyMetadata(null));

        public static readonly DependencyProperty BackButtonVisibilityProperty = DependencyProperty.Register(
            "BackButtonVisibility",
            typeof(Visibility),
            typeof(PageWin),
            new PropertyMetadata(Visibility.Visible));
        #endregion

        #region 构造方法
        public PageWin()
        {
            if (AtSys.IsPhoneUI)
                Style = (Style)Application.Current.Resources["PhonePageWin"];
            else
                DefaultStyleKey = typeof(PageWin);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 关闭前事件，可以取消关闭
        /// </summary>
        public event EventHandler<AsyncCancelEventArgs> Closing;

        /// <summary>
        /// 关闭后事件
        /// </summary>
        public event EventHandler Closed;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置标题文字
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置图标名称
        /// </summary>
        public Icons Icon
        {
            get { return (Icons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置菜单
        /// </summary>
        public Menu Menu
        {
            get { return (Menu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
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
        /// 获取设置窗口内容
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 获取设置初始参数
        /// </summary>
        public virtual string Params
        {
            get { return null; }
        }

        /// <summary>
        /// 获取设置是否显示返回按钮
        /// </summary>
        internal Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (AtSys.IsPhoneUI && BackButtonVisibility == Visibility.Visible)
            {
                AtUI.OnPhoneTitleTapped((Grid)GetTemplateChild("HeaderGrid"), this);
                Button btn = GetTemplateChild("BackButton") as Button;
                if (btn != null)
                    btn.Click += InputManager.OnBackClick;
            }
        }
        #endregion

        #region 实现接口
        /// <summary>
        /// 导航到窗口主页
        /// </summary>
        void IWin.NaviToHome()
        {
            if (AtApp.Frame.Content == null)
                BackButtonVisibility = Visibility.Collapsed;
            PhonePage.Show(this);
        }

        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        async Task<bool> IPhonePage.OnClosing()
        {
            if (Closing != null)
            {
                var args = new AsyncCancelEventArgs();
                Closing(this, args);
                await args.EnsureAllCompleted();
                return !args.Cancel;
            }
            return true;
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void IPhonePage.OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}