#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    public partial class RptList : List
    {
        public RptList()
        {
            InitializeComponent();
        }

        protected override async Task OnQuery()
        {
            var query = _clause.FuzzyOrWhere;
            if (string.IsNullOrEmpty(query) || query == "#全部")
            {
                _lv.Data = await RptX.Query("select id,name,note,ctime,mtime from cm_rpt order by name");
            }
            else
            {
                _lv.Data = await RptX.Query($"select id,name,note,ctime,mtime from cm_rpt where name like '%{query}%' order by name");
            }
        }

        protected override async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除吗？\r\n做个报表不容易，请慎重删除！"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<RptX>().ToList();
                if (await ls.Delete())
                {
                    await Refresh();
                }
            }
            else
            {
                var d = e.Data.To<RptX>();
                if (await d.Delete())
                {
                    await Refresh();
                }
            }
        }

        async void OnEditTemp(Mi e)
        {
            await Rpt.ShowDesign(new AppRptDesignInfo(e.Data as RptX));
        }
        
        void OnRefresh()
        {
            RefreshSqliteWin.UpdateSqliteFile("report");
        }
    }
}