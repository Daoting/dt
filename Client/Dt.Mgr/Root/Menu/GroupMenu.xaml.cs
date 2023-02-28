#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Mgr.Home
{
    /// <summary>
    /// 分组菜单项页面
    /// </summary>
    public sealed partial class GroupMenu : Tab
    {
        OmMenu _parent;

        public GroupMenu(OmMenu p_parent)
        {
            InitializeComponent();
            _parent = p_parent;
            _tb.Text = MenuDs.GetMenuPath(p_parent);
            _lv.Data = MenuDs.LoadGroupMenus(p_parent);
            Title = _parent.Name;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Kit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    Forward(new GroupMenu(menu));
                else
                    MenuDs.OpenMenu(menu);
            });
        }

        void OnSearch(object sender, Mi e)
        {
            Forward(new SearchMenu());
        }
    }
}
