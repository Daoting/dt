#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Dt.Base.Tools;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class MenuTree : Tree
    {
        public MenuTree()
        {
            InitializeComponent();
            CreateMenu(Menu);
            _tv.FilterCfg = new FilterCfg
            {
                FilterCols = "name",
                EnablePinYin = true,
                Placeholder = "文字或拼音简码",
                IsRealtime = true,
            };
        }

        public MenuX SelectedMenu => _tv.Selected<MenuX>();

        public void SelectByID(long p_id)
        {
            _tv.SelectByID(p_id);
        }
        
        protected override void OnFirstLoaded()
        {
            _tv.FixedRoot = new MenuX(ID: 0, Name: "菜单", IsGroup: true);
            _ = Refresh();
        }

        protected override async Task OnQuery()
        {
            _tv.Data = await MenuX.Query("select id,parent_id,name from cm_menu where is_group='1' order by dispidx");
        }
        
        void OnRefresh(Mi e)
        {
            RefreshSqliteWin.UpdateSqliteFile("menu");
        }
    }
}