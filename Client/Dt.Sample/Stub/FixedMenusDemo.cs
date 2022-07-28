#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using Dt.Mgr;
#endregion

namespace Dt.Sample
{
    public class FixedMenusDemo : IFixedMenus
    {
        /// <summary>
        /// 获取默认主页(DefaultHome)的固定菜单项
        /// </summary>
        public IList<OmMenu> GetFixedMenus() => new List<OmMenu>
        {
            new OmMenu(
                ID: 1110,
                Name: "通讯录",
                Icon: "留言",
                ViewName: "通讯录"),

            new OmMenu(
                ID: 3000,
                Name: "任务",
                Icon: "双绞线",
                ViewName: "任务",
                SvcName: "cm:UserRelated.GetMenuTip"),

            new OmMenu(
                ID: 4000,
                Name: "文件",
                Icon: "文件夹",
                ViewName: "文件"),

            new OmMenu(
                ID: 5000,
                Name: "发布",
                Icon: "公告",
                ViewName: "发布"),

            new OmMenu(
                ID: 1,
                Name: "样例",
                Icon: "词典",
                ViewName: "样例"),
        };
    }
}
