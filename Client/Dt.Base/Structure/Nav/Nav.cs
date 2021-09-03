#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 能够放在Tab Dlg中支持内部导航的用户控件
    /// </summary>
    public partial class Nav : UserControl
    {
        #region 静态内容
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(Nav),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu",
            typeof(Menu),
            typeof(Nav),
            new PropertyMetadata(null));

        public readonly static DependencyProperty HideTitleBarProperty = DependencyProperty.Register(
            "HideTitleBar",
            typeof(bool),
            typeof(Nav),
            new PropertyMetadata(false));
        #endregion

        INavHost _host;

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
        /// 获取设置是否隐藏宿主标题栏，Dlg 或 PhoneUI模式时有效
        /// </summary>
        public bool HideTitleBar
        {
            get { return (bool)GetValue(HideTitleBarProperty); }
            set { SetValue(HideTitleBarProperty, value); }
        }

        /// <summary>
        /// 向前导航到新内容
        /// </summary>
        /// <param name="p_content"></param>
        public void Forward(Nav p_content)
        {
            _host?.Forward(p_content);
        }

        /// <summary>
        /// 向后导航到上一内容
        /// </summary>
        public void Backward()
        {
            _host?.Backward();
        }

        /// <summary>
        /// 导航到指定页，支持多页Tab形式
        /// </summary>
        /// <param name="p_tabTitle">多个页面时用逗号隔开(自动以Tab形式显示)，null时自动导航到第一个Tab</param>
        public void NaviTo(string p_tabTitle)
        {
            if (Kit.IsPhoneUI && _host is Tab tab && tab.OwnWin != null)
                tab.OwnWin.NaviTo(p_tabTitle);
        }

        /// <summary>
        /// 导航到当前页
        /// </summary>
        public void NaviToSelf()
        {
            NaviTo(Title);
        }

        /// <summary>
        /// 将内容添加到宿主容器
        /// </summary>
        /// <param name="p_host">宿主容器</param>
        internal void AddToHost(INavHost p_host)
        {
            _host = p_host;
        }
    }
}
