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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 导航窗口
    /// </summary>
    [ContentProperty(Name = "NaviContent")]
    public partial class NaviWin : Control, IWin
    {
        #region 静态成员
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(NaviWin),
           new PropertyMetadata("无标题"));

        public readonly static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(NaviWin),
            new PropertyMetadata(Icons.文件));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu",
            typeof(Menu),
            typeof(NaviWin),
            new PropertyMetadata(null));

        public readonly static DependencyProperty HideTitleBarProperty = DependencyProperty.Register(
            "HideTitleBar",
            typeof(bool),
            typeof(NaviWin),
            new PropertyMetadata(false));

        public readonly static DependencyProperty NaviWidthProperty = DependencyProperty.Register(
            "NaviWidth",
            typeof(GridLength),
            typeof(NaviWin),
            new PropertyMetadata(new GridLength(400)));

        public readonly static DependencyProperty NaviContentProperty = DependencyProperty.Register(
            "NaviContent",
            typeof(object),
            typeof(NaviWin),
            new PropertyMetadata(null));

        public readonly static DependencyProperty NaviDataProperty = DependencyProperty.Register(
            "NaviData",
            typeof(IList<NaviRow>),
            typeof(NaviWin),
            new PropertyMetadata(null, OnNaviDataChanged));

        public readonly static DependencyProperty MainWinProperty = DependencyProperty.Register(
            "MainWin",
            typeof(IWin),
            typeof(NaviWin),
            new PropertyMetadata(null));

        static void OnNaviDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NaviWin)d).OnNaviDataChanged();
        }
        #endregion

        #region 构造方法
        public NaviWin()
        {
            if (AtSys.IsPhoneUI)
                Style = (Style)Application.Current.Resources["PhoneNaviWin"];
            else
                DefaultStyleKey = typeof(NaviWin);
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
        /// 获取设置左侧导航栏宽度，默认400
        /// </summary>
        public GridLength NaviWidth
        {
            get { return (GridLength)GetValue(NaviWidthProperty); }
            set { SetValue(NaviWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置左侧导航栏内容
        /// </summary>
        public object NaviContent
        {
            get { return GetValue(NaviContentProperty); }
            set { SetValue(NaviContentProperty, value); }
        }

        /// <summary>
        /// 获取设置左侧导航栏的导航数据，自动生成导航栏内容，方便常用
        /// </summary>
        public IList<NaviRow> NaviData
        {
            get { return (IList<NaviRow>)GetValue(NaviDataProperty); }
            set { SetValue(NaviDataProperty, value); }
        }

        /// <summary>
        /// 获取设置右侧窗口
        /// </summary>
        public IWin MainWin
        {
            get { return (IWin)GetValue(MainWinProperty); }
            private set { SetValue(MainWinProperty, value); }
        }

        /// <summary>
        /// 获取设置初始参数
        /// </summary>
        public virtual string Params
        {
            get { return null; }
        }
        #endregion

        /// <summary>
        /// 加载窗口，win模式显示在右侧，phone模式导航到窗口首页
        /// </summary>
        /// <param name="p_win"></param>
        public void LoadWin(IWin p_win)
        {
            if (AtSys.IsPhoneUI)
                p_win?.NaviToHome();
            else
                MainWin = p_win;
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (AtSys.IsPhoneUI)
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

        #region 导航栏
        void OnNaviDataChanged()
        {
            var data = NaviData;
            if (data != null)
            {
                Lv lv = new Lv();
                lv.View = Application.Current.Resources["NaviRowView"];
                lv.ItemClick += OnCmdRowClick;
                lv.Data = (IList)NaviData;
                NaviContent = lv;
            }
            else
            {
                ClearValue(NaviContentProperty);
            }
        }

        void OnCmdRowClick(object sender, ItemClickArgs e)
        {
            IWin win = ((NaviRow)e.Data).GetWinObj() as IWin;
            if (win != null)
                LoadWin(win);
        }
        #endregion
    }

    /// <summary>
    /// 导航项数据
    /// </summary>
    public class NaviRow
    {
        object _obj;

        public NaviRow()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_icon">图标</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_winType">窗口类型</param>
        /// <param name="p_desc">描述信息</param>
        public NaviRow(Icons p_icon, string p_title, Type p_winType, string p_desc = null)
        {
            Icon = p_icon;
            Title = p_title;
            WinType = p_winType;
            Desc = p_desc;
        }

        /// <summary>
        /// 获取设置图标
        /// </summary>
        public Icons Icon { get; set; }

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置描述信息
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 获取设置窗口类型
        /// </summary>
        public Type WinType { get; set; }

        internal object GetWinObj()
        {
            if (_obj != null)
                return _obj;

            if (WinType != null)
                _obj = Activator.CreateInstance(WinType);
            return _obj;
        }
    }
}
