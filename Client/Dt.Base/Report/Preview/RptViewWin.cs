#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base.Docking;
using Dt.Core;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 默认报表预览窗口
    /// </summary>
    internal partial class RptViewWin : Win
    {
        readonly RptView _view = new RptView();
        Tab _tabCenter;

        public RptViewWin(RptInfo p_info)
        {
            // 确保RptInfo已初始化，因含异步！

            IRptSearchForm searchForm = null;
            if (p_info.ScriptObj != null)
            {
                p_info.ScriptObj.View = _view;
                searchForm = p_info.ScriptObj.GetSearchForm(p_info);
            }

            Main wc = new Main();
            var tabs = new Tabs();
            var tab = new Tab { Title = "内容" };
            tab.Content = _view;
            tabs.Items.Add(tab);
            wc.Items.Add(tabs);
            Items.Add(wc);

            var setting = p_info.Root.ViewSetting;
            if (setting.ShowMenu)
            {
                var menu = new Menu();
                if (!setting.ShowSearchForm
                    && (searchForm != null || p_info.Root.Params.ExistXaml))
                {
                    menu.Items.Add(new Mi { ID = "查询", Icon = Icons.搜索, Cmd = _view.CmdSearch });
                }
                menu.Items.Add(new Mi { ID = "导出", Icon = Icons.导出, Cmd = _view.CmdExport });
                menu.Items.Add(new Mi { ID = "打印", Icon = Icons.打印, Cmd = _view.CmdPrint });
                p_info.ScriptObj?.InitMenu(menu);
                tab.Menu = menu;
            }

            if (setting.ShowSearchForm
                && (searchForm != null || p_info.Root.Params.ExistXaml))
            {
                Pane wi = new Pane();
                tabs = new Tabs();
                tab = new Tab { Title = "查询" };

                // 加载查询面板内容
                if (searchForm == null)
                    searchForm = new RptSearchForm(p_info);
                tab.Menu = searchForm.Menu;
                searchForm.Query += (s, e) => _view.LoadReport(e);
                tab.Content = searchForm;

                tabs.Items.Add(tab);
                wi.Items.Add(tabs);
                Items.Add(wi);
            }

            // 初次加载自动执行查询
            if (setting.AutoQuery || p_info.Root.Params.Data.Count == 0)
                _view.LoadReport(p_info);
        }

        #region 报表组
        public RptViewWin(RptInfoList p_infos)
        {
            if (p_infos == null || p_infos.Count < 2)
            {
                Kit.Warn("浏览报表组时描述信息不完整！");
                return;
            }

            LoadRptView();
            Pane wi = new Pane();
            var tabs = new Tabs();
            tabs.SelectedChanged += OnSelectedTabChanged;
            foreach (var info in p_infos)
            {
                tabs.Items.Add(new Tab { Title = info.Name, Tag = info });
            }
            wi.Items.Add(tabs);
            Items.Add(wi);
        }

        async void OnSelectedTabChanged(object sender, SelectedChangedEventArgs e)
        {
            Tab tab = e.SelectItem as Tab;
            if (tab.Content != null)
                return;

            RptInfo info = tab.Tag as RptInfo;
            if (!await info.Init())
                return;

            // 加载查询面板内容，脚本优先
            IRptSearchForm searchForm = null;
            if (info.ScriptObj != null)
                searchForm = info.ScriptObj.GetSearchForm(info);
            if (searchForm == null && info.Root.Params.ExistXaml)
                searchForm = new RptSearchForm(info);
            if (searchForm != null)
            {
                tab.Content = searchForm;
                tab.Menu = searchForm.Menu;
                searchForm.Query += OnQuery;
            }

            // 初次加载自动执行查询
            if (info.Root.ViewSetting.AutoQuery || searchForm == null)
                _view.LoadReport(info);
        }

        /// <summary>
        /// 已提供完整查询参数值，加载报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnQuery(object sender, RptInfo e)
        {
            if (_view.Info != e)
            {
                if (e.Root.ViewSetting.ShowMenu)
                {
                    if (e.ViewMenu == null)
                    {
                        var menu = new Menu
                        {
                            Items =
                            {
                                new Mi { ID = "导出", Icon = Icons.导出, Cmd = _view.CmdExport },
                                new Mi { ID = "打印", Icon = Icons.打印, Cmd = _view.CmdPrint },
                            }
                        };
                        e.ViewMenu = menu;
                        e.ScriptObj?.InitMenu(menu);
                    }
                    _tabCenter.Menu = e.ViewMenu;
                }
                else if (_tabCenter.Menu != null)
                {
                    _tabCenter.ClearValue(Tab.MenuProperty);
                }
            }
            _view.LoadReport(e);
        }

        void LoadRptView()
        {
            Main wc = new Main();
            var tabs = new Tabs();
            _tabCenter = new Tab { Title = "内容" };
            _tabCenter.Content = _view;
            tabs.Items.Add(_tabCenter);
            wc.Items.Add(tabs);
            Items.Add(wc);
        }
        #endregion
    }
}
