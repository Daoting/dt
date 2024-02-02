#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 默认报表预览窗口
    /// </summary>
    internal partial class RptViewWin : Win
    {
        public RptViewWin(RptInfo p_info)
        {
            // 虽然未调用InitializeComponent()，xaml文件不能删除，否则win上不可见！

            // 确保RptInfo已初始化，因含异步！
            LoadRptView(p_info);
        }

        void LoadRptView(RptInfo p_info)
        {
            RptView view = new RptView { Title = p_info.Name };

            RptSearchTab mvSearch = null;
            if (p_info.ScriptObj != null)
            {
                p_info.ScriptObj.View = view;
                mvSearch = p_info.ScriptObj.GetSearchForm(p_info);
            }

            // 主区RptView
            Main wc = new Main();
            var tabs = new Tabs();
            tabs.Items.Add(view);
            wc.Items.Add(tabs);
            Items.Add(wc);

            // 左侧查询面板
            var setting = p_info.Root.ViewSetting;
            if (setting.ShowSearchForm
                && (mvSearch != null || p_info.Root.Params.ExistXaml))
            {
                Pane wi = new Pane();
                tabs = new Tabs();

                // 加载查询面板内容
                if (mvSearch == null)
                    mvSearch = new DefaultRptSearch(p_info);
                mvSearch.Query += (s, e) => view.LoadReport(e);

                tabs.Items.Add(mvSearch);
                wi.Items.Add(tabs);
                Items.Add(wi);
            }

            // 初次加载，参数不完备时不绘制内容
            view.LoadReport(p_info);
        }
    }
}
