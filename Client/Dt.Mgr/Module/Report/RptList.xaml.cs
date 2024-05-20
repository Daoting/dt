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
    public partial class RptList : LvTab
    {
        #region 变量
        string _query;
        #endregion

        #region 构造
        public RptList()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public void OnSearch(string p_txt)
        {
            _query = p_txt;
            Title = string.IsNullOrEmpty(p_txt) ? "所有报表" : "报表列表 - " + p_txt;
            NaviTo(this);
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await RptX.Query("select id,name,note,ctime,mtime from cm_rpt order by name");
            }
            else
            {
                _lv.Data = await RptX.Query($"select id,name,note,ctime,mtime from cm_rpt where name like '%{_query}%' order by name");
            }
        }
        #endregion

        #region 交互
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple
                && OwnWin is RptWin win)
            {
                await win.Form.Update(_lv.SelectedRow?.ID);
            }
        }

        async void OnItemDbClick(object e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple
                && e is RptX rpt)
            {
                await Rpt.ShowDesign(new AppRptDesignInfo(rpt));
            }
        }

        async void OnAdd(Mi e)
        {
            if (OwnWin is RptWin win)
            {
                await win.Form.Open(-1);
            }
        }

        async void OnEdit(Mi e)
        {
            if (OwnWin is RptWin win)
            {
                await win.Form.Open(e.Row?.ID);
            }
        }

        async void OnDel(Mi e)
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
            await Rpt.ShowDesign(new AppRptDesignInfo(e.Data.To<RptX>()));
        }
        
        void OnRefresh(Mi e)
        {
            RefreshSqliteWin.UpdateSqliteFile("report");
        }
        #endregion
    }
}