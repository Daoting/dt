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
    internal partial class RptWin : Win
    {
        RptTab _rpt;
        RptInfo _info;
        
        public RptWin(RptWinParams p_params)
        {
            // 虽然未调用InitializeComponent()，xaml文件不能删除，否则win上不可见！

            // 确保RptInfo已初始化，因含异步！
            LoadRptView(p_params);
        }

        void LoadRptView(RptWinParams p_params)
        {
            _info = p_params.Info;
            _rpt = new RptTab { Title = _info.Name };
            if (p_params.IsPdf)
                _rpt.IsPdf = true;

            // 主区RptTab
            Main wc = new Main();
            var tabs = new Tabs();
            tabs.Items.Add(_rpt);
            wc.Items.Add(tabs);
            Items.Add(wc);

            if (_info.ScriptObj != null)
                _info.ScriptObj.View = _rpt;

            // 左侧查询面板
            var pars = _info.Root.Params;
            if (pars.ExistQueryForm)
            {
                Pane wi = new Pane();
                tabs = new Tabs();
                Tab tab = new Tab { Title = "查询" };
                tabs.Items.Add(tab);
                wi.Items.Add(tabs);
                Items.Add(wi);

                // 加载查询面板内容
                LoadQueryForm(tab);
            }

            // 初次加载，参数不完备时不绘制内容
            // 参数完备 且 自动查询或无参数 时，加载报表
            var error = _info.IsParamsValid();
            if (error == null
                && (_info.Root.ViewSetting.AutoQuery || _info.Root.Params.Data.Count == 0))
            {
                _rpt.LoadReport(_info);
            }
        }

        async void LoadQueryForm(Tab p_tab)
        {
            var con = await _info.Root.Params.CreateQueryForm(_info.Params);
            p_tab.Content = con;
            con.Query += row =>
            {
                _info.UpdateParams(row);
                _rpt.LoadReport(_info);
            };
        }

        protected override void OnClosed()
        {
            // 必须关闭
            _rpt.Close();
        }
    }

    internal class RptWinParams
    {
        public RptInfo Info { get; set; }
        public bool IsPdf { get; set; }

        #region 比较
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RptWinParams))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            // 只比较标识，识别窗口用
            if (Info != null)
                return Info.Equals(((RptWinParams)obj).Info);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
