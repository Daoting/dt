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
    public partial class 权限List : LvTab
    {
        #region 构造
        public 权限List()
        {
            InitializeComponent();
            if (Kit.IsPhoneUI)
                _lv.ItemClick += OnItemClick;
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
                _lv.Data = await 权限X.Query(null);
            }
            else
            {
                var par = await _clause.Build<权限X>();
                _lv.Data = await 权限X.Query(par.Sql, par.Params);
            }
        }
        #endregion

        #region 交互
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                await _win.MainForm.Update(_lv.SelectedRow?.ID);
            }
        }

        async void OnItemDbClick(object e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                await _win.MainForm.Open(_lv.SelectedRow?.ID);
            }
        }

        async void OnAdd(Mi e)
        {
            await _win.MainForm.Open(-1);
        }

        async void OnEdit(Mi e)
        {
            await _win.MainForm.Open(e.Row?.ID);
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                NaviTo(new List<Tab> { _win.角色List });
            }
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            var d = e.Data.To<权限X>();
            if (await d.Delete())
            {
                await Refresh();
            }
        }
        #endregion

        #region 内部
        权限Win _win => OwnWin as 权限Win;
        QueryClause _clause;
        #endregion
    }
}