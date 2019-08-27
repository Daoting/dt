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
    /// 搜索菜单项
    /// </summary>
    public sealed partial class SearchMenu : UserControl, ITabContent
    {
        public SearchMenu()
        {
            InitializeComponent();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            AtKit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    Tab.NaviTo(new GroupMenu(menu));
                else
                    MenuKit.OpenMenu(menu);
            });
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="p_filter"></param>
        void OnSearch(object sender, string p_filter)
        {
            if (string.IsNullOrEmpty(p_filter))
                _lv.Data = null;
            else
                _lv.Data = MenuKit.LoadMenusByName(p_filter.ToLower());
        }

        #region ITabContent
        public Tab Tab { get; set; }

        public Menu Menu
        {
            get { return null; }
        }

        public string Title
        {
            get { return "搜索"; }
        }
        #endregion
    }
}
