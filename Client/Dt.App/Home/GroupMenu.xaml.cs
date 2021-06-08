#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Home
{
    /// <summary>
    /// 分组菜单项页面
    /// </summary>
    public sealed partial class GroupMenu : UserControl, INaviContent
    {
        OmMenu _parent;

        public GroupMenu(OmMenu p_parent)
        {
            InitializeComponent();
            _parent = p_parent;
            _tb.Text = MenuKit.GetMenuPath(p_parent);
            _lv.Data = MenuKit.LoadGroupMenus(p_parent);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Kit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    _host.NaviTo(new GroupMenu(menu));
                else
                    MenuKit.OpenMenu(menu);
            });
        }

        void OnSearch(object sender, Mi e)
        {
            _host.NaviTo(new SearchMenu());
        }

        #region INaviContent
        INaviHost _host;

        void INaviContent.AddToHost(INaviHost p_host)
        {
            _host = p_host;
            _host.Title = _parent.Name;

            var menu = new Menu();
            Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
            mi.Click += OnSearch;
            menu.Items.Add(mi);
            _host.Menu = menu;
        }
        #endregion
    }
}
