#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public partial class 虚拟List : LvTab
    {
        #region 构造方法
        public 虚拟List()
        {
            InitializeComponent();
            _lv.AddMultiSelMenu(Menu);
        }
        #endregion

        #region 公开
        public void OnSearch(QueryClause p_clause)
        {
            _clause = p_clause;
            NaviTo(this);
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_clause == null)
            {
                _lv.Data = await VirX<主表X, 扩展1X, 扩展2X>.Query(null);
            }
            else
            {
                var par = await _clause.Build<VirX<主表X, 扩展1X, 扩展2X>>();
                _lv.Data = await VirX<主表X, 扩展1X, 扩展2X>.Query(par.Sql, par.Params);
            }
        }
        #endregion

        #region 交互
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                await _win.Form.Update(_lv.SelectedRow?.ID);
            }
        }

        async void OnItemDbClick(object e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                await _win.Form.Open(_lv.SelectedRow?.ID);
            }
        }

        async void OnAdd(Mi e)
        {
            await _win.Form.Open(-1);
        }

        async void OnEdit(Mi e)
        {
            await _win.Form.Open(e.Row?.ID);
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<VirX<主表X, 扩展1X, 扩展2X>>().ToList();
                if (await ls.Delete())
                {
                    await Refresh();
                }
            }
            else
            {
                var d = e.Data.To<VirX<主表X, 扩展1X, 扩展2X>>();
                if (await d.Delete())
                {
                    await Refresh();
                }
            }
        }
        #endregion

        #region 内部
        虚拟Win _win => OwnWin as 虚拟Win;
        QueryClause _clause;
        #endregion
    }
}