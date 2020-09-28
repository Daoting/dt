#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base.Docking;
using Dt.Base.Report;
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认报表预览窗口
    /// </summary>
    internal partial class RptViewWin : Win
    {
        RptView _view;
        Tab _centerTab;

        public RptViewWin(RptInfoList p_infos)
        {
            if (p_infos == null || p_infos.Count < 2)
            {
                AtKit.Warn("浏览报表组时描述信息不完整！");
                return;
            }

            LoadRptView();
            WinItem wi = new WinItem { DockState = WinItemState.DockedLeft };
            var tabs = new Tabs();
            tabs.SelectedChanged += (s, e) => SelectTab(e.SelectItem as Tab);
            foreach (var info in p_infos)
            {
                tabs.Items.Add(new Tab { Title = info.Name, Tag = info });
            }
            wi.Items.Add(tabs);
            Items.Add(wi);
        }

        public RptViewWin(RptInfo p_info)
        {
            LoadRptView();
            LoadSearchForm(p_info);
        }

        async void LoadSearchForm(RptInfo p_info)
        {
            if (!await p_info.InitTemplate())
                return;

            if (!p_info.Root.ViewSetting.HideSearchForm)
            {
                WinItem wi = new WinItem { DockState = WinItemState.DockedLeft };
                var tabs = new Tabs();
                var tab = new Tab { Title = "查询", Tag = p_info };
                tabs.Items.Add(tab);
                wi.Items.Add(tabs);
                Items.Add(wi);
                SelectTab(tab);
            }
            else
            {
                CreateMenu(p_info);

                // 初次加载自动执行查询
                //if (p_info.Root.ViewSetting.AutoQuery)
                //    fm.DoQuery();
            }
        }

        async void SelectTab(Tab p_tab)
        {
            if (p_tab.Content != null)
                return;

            RptInfo info = p_tab.Tag as RptInfo;
            // 加载报表模板
            if (!await info.InitTemplate())
                return;

            // 加载查询面板内容
            var fm = new RptSearchForm(info);
            p_tab.Content = fm;
            fm.Query += OnQuery;

            CreateMenu(info);
            _view.LoadReport(info);
            // 初次加载自动执行查询
            //if (info.Root.ViewSetting.AutoQuery)
            //    fm.DoQuery();
        }

        void CreateMenu(RptInfo p_info)
        {
            Menu menu = new Menu();
            Mi mi = new Mi { ID = "导出" };
            menu.Items.Add(mi);
            p_info.ViewMenu = menu;
        }

        void LoadRptView()
        {
            WinCenter wc = new WinCenter();
            var tabs = new Tabs();
            _centerTab = new Tab { Title = "内容" };
            _view = new RptView();
            _centerTab.Content = _view;
            tabs.Items.Add(_centerTab);
            wc.Items.Add(tabs);
            Items.Add(wc);
        }

        /// <summary>
        /// 已提供完整查询参数值，加载报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnQuery(object sender, RptInfo e)
        {
            if (_centerTab.Menu != e.ViewMenu)
                _centerTab.Menu = e.ViewMenu;
            //_view.LoadReport(e);
        }
    }
}
