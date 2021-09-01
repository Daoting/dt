#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-01 创建
******************************************************************************/
#endregion

#region 引用命名
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
        public void NaviTo(Nav p_content)
        {
            _host?.NaviTo(p_content);
        }

        /// <summary>
        /// 返回上一内容
        /// </summary>
        public void GoBack()
        {
            _host?.GoBack();
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
