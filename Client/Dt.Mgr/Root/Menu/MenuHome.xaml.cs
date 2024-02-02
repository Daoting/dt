#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Home
{
    /// <summary>
    /// 主页菜单的占位Tab
    /// </summary>
    public partial class MenuHome : Tab
    {
        Dlg _dlg;
        Tab _tab;

        public MenuHome()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取设置固定菜单项，通常在加载菜单前由外部设置
        /// </summary>
        public IList<OmMenu> FixedMenus
        {
            get { return MenuDs.FixedMenus; }
            set { MenuDs.FixedMenus = value; }
        }

        /// <summary>
        /// 显示开始菜单对话框
        /// </summary>
        public void ShowDlg()
        {
            if (_dlg == null)
            {
                _dlg = new Dlg
                {
                    HideTitleBar = true,
                    WinPlacement = DlgPlacement.FromTopLeft,
                    Width = 400,
                    Height = Kit.ViewHeight / 2,
                    BorderThickness = new Thickness(4),
                    BorderBrush = Res.主蓝,
                };
                _dlg.LoadTab(this);
            }
            _dlg.Show();
        }

        protected override void OnFirstLoaded()
        {
            if (!Kit.IsLogon)
                LoginDs.LoginSuc += LoadMenus;
            else
                LoadMenus();
        }
        
        async void LoadMenus()
        {
            await MenuDs.InitMenus();
            if (MenuDs.FavMenus.Count == 0)
            {
                FillTab(new RootMenu());
            }
            else
            {
                FillTab(new FavMenu());
            }
        }

        void FillTab(Tab p_tab)
        {
            Title = p_tab.Title;
            Icon = p_tab.Icon;
            Menu = p_tab.Menu;
            Content = p_tab.Content;
            p_tab.Tag = this;

            // 在android上，若不设为成员变量，Tab内的所有事件不触发，像被回收了！
            _tab = p_tab;
        }
    }
}