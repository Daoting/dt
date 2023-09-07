#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $ext_safeprojectname$
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
            InitFixedMenus();
        }

        /// <summary>
        /// 设置固定菜单项
        /// </summary>
        void InitFixedMenus()
        {
            _menu.FixedMenus = new List<OmMenu>
            {
                new OmMenu(
                    ID: 1001,
                    Name: "通讯录",
                    Icon: "留言",
                    ViewName: "通讯录"),

                new OmMenu(
                    ID: 1002,
                    Name: "任务",
                    Icon: "双绞线",
                    ViewName: "任务"),

                new OmMenu(
                    ID: 1003,
                    Name: "文件",
                    Icon: "文件夹",
                    ViewName: "文件"),

            };
        }
    }
}