#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Tools;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.UIDemo
{
    class Taskbar : ITaskbar
    {
        /// <summary>
        /// 获取任务栏左侧的开始界面
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetStartUI()
        {
            var btn = new Button
            {
                Content = "\uE08E",
                Style = Res.字符按钮,
            };
            ToolTipService.SetToolTip(btn, "开始");
            return btn;
        }

        /// <summary>
        /// 获取任务栏右侧的托盘界面
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetTrayUI()
        {
            var btn = new Button
            {
                Content = "\uE004",
                Style = Res.字符按钮,
            };
            btn.Click += Btn_Click;
            return btn;
        }

        void Btn_Click(object sender, RoutedEventArgs e)
        {
            var nav = new NavList { ViewMode = NavViewMode.Tile, Title = "系统", To = NavTarget.NewWin };
            nav.Data = new Nl<Nav>
            {
                new Nav("本地库", typeof(LocalDbWin), Icons.数据库) { Desc = "管理 LocalState\\.data 目录下的 sqlite 库" },
                new Nav("本地文件", typeof(LocalFileWin), Icons.文件) { Desc = "管理 LocalState 的所有文件" },
                new Nav("更新缓存文件", typeof(RefreshSqliteWin), Icons.刷新) { Desc = "刷新服务端指定的 sqlite 缓存文件" },

                new Nav("数据库工具", typeof(RemoteDbWin), Icons.数据库) { Desc = "数据库初始化、备份等功能" },

                new Nav("类型别名", typeof(TypeAliasWin), Icons.划卡) { Desc = "所有为类型命名别名的名称与类型的列表" },

                new Nav("切换服务", null, Icons.服务器) { Desc = "切换服务地址", Callback = (s, n) =>
                {
                    SysTrace.ToggleSvcUrl();
                    if (s is Dlg dlg)
                        dlg.Close();
                } },
            };

            var dlg = new Dlg
            {
                HideTitleBar = true,
                WinPlacement = DlgPlacement.FromTopRight,
                Width = 500,
            };
            dlg.LoadTab(nav);
            dlg.Show();
        }
    }
}
