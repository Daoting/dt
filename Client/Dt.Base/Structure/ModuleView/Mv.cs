#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 业务模块视图(ModuleView)控件，放在Tab中支持内部导航
    /// </summary>
    public partial class Mv : UserControl
    {
        #region 静态内容
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(Mv),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu",
            typeof(Menu),
            typeof(Mv),
            new PropertyMetadata(null));

        public readonly static DependencyProperty HideTitleBarProperty = DependencyProperty.Register(
            "HideTitleBar",
            typeof(bool),
            typeof(Mv),
            new PropertyMetadata(false));

        public static readonly DependencyProperty OwnDlgProperty = DependencyProperty.Register(
            "OwnDlg",
            typeof(Dlg),
            typeof(Mv),
            new PropertyMetadata(null));
        #endregion

        #region 成员变量
        Tab _tab;
        TaskCompletionSource<bool> _taskSrc;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置宿主标题文字
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置宿主菜单
        /// </summary>
        public Menu Menu
        {
            get { return (Menu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// 获取设置是否隐藏宿主标题栏，PhoneUI模式时有效
        /// </summary>
        public bool HideTitleBar
        {
            get { return (bool)GetValue(HideTitleBarProperty); }
            set { SetValue(HideTitleBarProperty, value); }
        }

        /// <summary>
        /// 带遮罩时的Dlg容器
        /// </summary>
        internal Dlg OwnDlg
        {
            get { return (Dlg)GetValue(OwnDlgProperty); }
            private set { SetValue(OwnDlgProperty, value); }
        }

        /// <summary>
        /// 返回值
        /// </summary>
        protected object Result { get; set; }
        #endregion

        /// <summary>
        /// 向前导航到新内容，可异步等待返回值
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="p_content">导航到Mv</param>
        /// <param name="p_params">Mv输入参数</param>
        /// <param name="p_isModal">WinUI模式是否带遮罩，遮罩为了禁止对其他位置编辑(用Dlg实现)</param>
        /// <returns>返回时的输出参数</returns>
        public Task<T> Forward<T>(Mv p_content, object p_params = null, bool p_isModal = false)
        {
            Forward(p_content, p_params, p_isModal);
            return p_content.StartWait<T>();
        }

        /// <summary>
        /// 向前导航到新内容
        /// </summary>
        /// <param name="p_content">导航到Mv</param>
        /// <param name="p_params">Mv输入参数</param>
        /// <param name="p_isModal">WinUI模式是否带遮罩，遮罩为了禁止对其他位置编辑(用Dlg实现)</param>
        public void Forward(Mv p_content, object p_params = null, bool p_isModal = false)
        {
            p_content.Initialize(p_params);
            if (p_isModal)
                ShowDlg(p_content);
            else
                _tab.Forward(p_content);
        }

        /// <summary>
        /// 向后返回到上一内容
        /// </summary>
        public void Backward()
        {
            _tab.Backward();
        }

        /// <summary>
        /// 导航到指定页，支持多页Tab形式
        /// </summary>
        /// <param name="p_tabTitle">多个页面时用逗号隔开(自动以Tab形式显示)，null时自动导航到第一个Tab</param>
        public void NaviTo(string p_tabTitle)
        {
            if (Kit.IsPhoneUI && _tab is Tab tab && tab.OwnWin != null)
                tab.OwnWin.NaviTo(p_tabTitle);
        }

        /// <summary>
        /// 导航到当前页
        /// </summary>
        public void NaviToSelf()
        {
            NaviTo(Title);
        }

        #region 虚方法
        protected virtual void OnInit(object p_params)
        {
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 将内容添加到宿主Tab
        /// </summary>
        /// <param name="p_tab">宿主容器</param>
        internal void AddToHost(Tab p_tab)
        {
            _tab = p_tab;
        }

        /// <summary>
        /// 等待当前Mv返回
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <returns>返回值</returns>
        async Task<T> StartWait<T>()
        {
            _taskSrc = new TaskCompletionSource<bool>();
            await _taskSrc.Task;
            return Result.To<T>();
        }

        /// <summary>
        /// 结束等待当前Mv
        /// </summary>
        internal void StopWait()
        {
            // Forward<T>的情况
            if (_taskSrc != null && !_taskSrc.Task.IsCompleted)
            {
                _taskSrc.SetResult(true);
                _taskSrc = null;
            }
        }

        /// <summary>
        /// 导航前的初始化
        /// </summary>
        /// <param name="p_params"></param>
        void Initialize(object p_params)
        {
            Result = null;
            OnInit(p_params);
        }

        void ShowDlg(Mv p_content)
        {
            if (Kit.IsPhoneUI)
            {
                Tab tab = new Tab { OwnWin = _tab.OwnWin, Content = p_content };
                PhonePage.Show(tab);
                return;
            }

            var dlg = new Dlg()
            {
                PlacementTarget = _tab.OwnTabs,
                WinPlacement = DlgPlacement.TargetOverlap,
                Resizeable = false,
                HideTitleBar = true,
                ShowVeil = true,
                IsPinned = true,
                BorderThickness = new Thickness(0),
            };
            p_content.OwnDlg = dlg;

            Tabs tabs = new Tabs();
            tabs.Items.Add(new Tab { Content = p_content });
            dlg.Content = tabs;
            dlg.Show();
        }
        #endregion
    }
}
