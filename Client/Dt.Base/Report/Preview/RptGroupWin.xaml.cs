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
        public RptGroupWin(RptInfoList p_infos)
        {
            InitializeComponent();

            if (p_infos == null || p_infos.Count < 2)
            {
                Kit.Warn("浏览报表组时描述信息不完整！");
                return;
            }

            var ls = new Nl<Nav>();
            foreach (var info in p_infos)
            {
                ls.Add(new Nav(info.Name, typeof(RptViewWin)) { Callback = OpenWin, Params = info });
            }
            _nav.Data = ls;
            _nav.Select(0);
        }

        async void OpenWin(Win p_win, Nav p_nav)
        {
            var info = p_nav.Params as RptInfo;
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
}
