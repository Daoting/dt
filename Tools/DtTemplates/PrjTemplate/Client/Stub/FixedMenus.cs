#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

namespace $ext_safeprojectname$
{
    public class FixedMenus : IFixedMenus
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

        };
    }
}
