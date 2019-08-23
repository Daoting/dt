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

namespace Bs.Mgr.Home
{
    /// <summary>
    /// 分组菜单项页面
    /// </summary>
    public sealed partial class GroupMenu : UserControl, ITabContent
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
            AtKit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    Tab.NaviTo(new GroupMenu(menu));
                else
                    AtUI.OpenMenu(menu);
            });
        }

        void OnSearch(object sender, Mi e)
        {
            Tab.NaviTo(new SearchMenu());
        }

        #region ITabContent
        public Tab Tab { get; set; }

        Menu _menu;
        public Menu Menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new Menu();
                    Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
                    mi.Click += OnSearch;
                    _menu.Items.Add(mi);
                }
                return _menu;
            }
        }

        public string Title
        {
            get { return _parent.Name; }
        }
        #endregion
    }
}
