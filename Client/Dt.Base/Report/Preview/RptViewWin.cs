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
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认报表预览窗口
    /// </summary>
    internal partial class RptViewWin : Win
    {
        readonly RptView _view = new RptView();
        Menu _menu;

        public RptViewWin(RptInfo p_info)
        {
            // 确保RptInfo已初始化，因含异步
            if (p_info.ScriptObj != null)
                p_info.ScriptObj.View = _view;

            WinCenter wc = new WinCenter();
            var tabs = new Tabs();
            var tab = new Tab { Title = "内容" };
            tab.Content = _view;
            tabs.Items.Add(tab);
            wc.Items.Add(tabs);
            Items.Add(wc);

            var setting = p_info.Root.ViewSetting;
            if (setting.ShowMenu)
            {
                _menu = new Menu();
                if (setting.ShowSearchMi)
                    _menu.Items.Add(new Mi { ID = "查询", Icon = Icons.搜索, Cmd = _view.CmdSearch });
                if (setting.ShowExportMi)
                    _menu.Items.Add(new Mi { ID = "导出", Icon = Icons.导出, Cmd = _view.CmdExport });
                if (setting.ShowPrintMi)
                    _menu.Items.Add(new Mi { ID = "打印", Icon = Icons.打印, Cmd = _view.CmdPrint });
                p_info.ScriptObj?.InitMenu(_menu);
                tab.Menu = _menu;
            }

            if (setting.ShowSearchForm && p_info.Root.Params.Data.Count > 0)
            {
                WinItem wi = new WinItem { DockState = WinItemState.DockedLeft };
                tabs = new Tabs();
                tab = new Tab { Title = "查询" };
                // 加载查询面板内容
                var fm = new RptSearchForm(p_info, tab);
                fm.Query += (s, e) => _view.LoadReport(e);
                tab.Content = fm;
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
                AtKit.Warn("浏览报表组时描述信息不完整！");
                return;
            }

            LoadRptView();
            WinItem wi = new WinItem { DockState = WinItemState.DockedLeft };
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

            // 加载查询面板内容
            var fm = new RptSearchForm(info, tab);
            tab.Content = fm;
            fm.Query += OnQuery;

            // 初次加载自动执行查询
            if (info.Root.ViewSetting.AutoQuery)
                fm.DoQuery();
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
                var setting = e.Root.ViewSetting;
                if (setting.ShowMenu)
                {
                    if (_menu.Visibility == Visibility.Collapsed)
                        _menu.Visibility = Visibility.Visible;
                    _menu["查询"].Visibility = setting.ShowSearchMi ? Visibility.Visible : Visibility.Collapsed;
                    _menu["导出"].Visibility = setting.ShowExportMi ? Visibility.Visible : Visibility.Collapsed;
                    _menu["打印"].Visibility = setting.ShowPrintMi ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    if (_menu.Visibility == Visibility.Visible)
                        _menu.Visibility = Visibility.Collapsed;
                }
            }
            _view.LoadReport(e);
        }

        void LoadRptView()
        {
            WinCenter wc = new WinCenter();
            var tabs = new Tabs();
            var tab = new Tab { Title = "内容" };
            tab.Content = _view;
            tabs.Items.Add(tab);
            wc.Items.Add(tabs);
            Items.Add(wc);

            _menu = new Menu
            {
                Items =
                {
                    new Mi { ID = "查询", Icon = Icons.搜索, Cmd = _view.CmdSearch },
                    new Mi { ID = "导出", Icon = Icons.导出, Cmd = _view.CmdExport },
                    new Mi { ID = "打印", Icon = Icons.打印, Cmd = _view.CmdPrint },
                }
            };
            tab.Menu = _menu;
        }
        #endregion
    }
}
