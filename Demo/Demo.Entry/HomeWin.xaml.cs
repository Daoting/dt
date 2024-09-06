#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024/5/20 10:39:08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Home;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

#endregion

namespace Demo.Entry
{
    /// <summary>
    /// 覆盖默认主页
    /// </summary>
    [View(LobViews.主页)]
    public partial class HomeWin : Win
    {
        public HomeWin()
        {
            InitializeComponent();

            // 设置固定菜单项
            _menu.InitMenu(new List<OmMenu>
            {
                new OmMenu(
                    ID: 1000,
                    Name: "控件样例",
                    Icon: "词典",
                    ViewName: "控件样例"),

                new OmMenu(
                    ID: 1001,
                    Name: "管理样例",
                    Icon: "数据库",
                    ViewName: "管理样例"),

                new OmMenu(
                    ID: 1002,
                    Name: "业务样例",
                    Icon: "审核",
                    ViewName: "业务样例"),

                new OmMenu(
                    ID: 1003,
                    Name: "通讯录",
                    Icon: "留言",
                    ViewName: "通讯录"),

                new OmMenu(
                    ID: 1004,
                    Name: "文件",
                    Icon: "文件夹",
                    ViewName: "文件"),

            });
        }
    }
}