#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-30 创建
**************************************************************************/
#endregion

#region 命名空间
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 默认报表组窗口
    /// </summary>
    internal partial class RptGroupWin : Win
    {
        public RptGroupWin(RptGroupWinParams p_params)
        {
            InitializeComponent();

            if (p_params.Infos == null || p_params.Infos.Count < 2)
            {
                Kit.Warn("浏览报表组时描述信息不完整！");
                return;
            }

            var ls = new Nl<Nav>();
            foreach (var info in p_params.Infos)
            {
                ls.Add(new Nav(info.Name, typeof(RptWin)) { Callback = OpenWin, Params = new RptWinParams { Info = info, IsPdf = p_params.IsPdf } });
            }
            _nav.Data = ls;
            _nav.Select(0);
        }

        async void OpenWin(object p_owner, Nav p_nav)
        {
            var info = ((RptWinParams)p_nav.Params).Info;
            if (await info.Init())
            {
                var win = p_nav.GetCenter();
                LoadMain(win);
            }
            else
            {
                Kit.Warn($"初始化报表模板[{info.Name}]出错！");
            }
        }
    }

    internal class RptGroupWinParams
    {
        public RptInfoList Infos { get; set; }
        public bool IsPdf { get; set; }
    }
}
