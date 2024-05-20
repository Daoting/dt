#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    public partial class OptionGroupList : LvTab
    {
        #region 变量
        QueryClause _clause;
        #endregion

        #region 构造
        public OptionGroupList()
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
                _lv.Data = await OptionGroupX.Query(null);
            }
            else
            {
                var par = await _clause.Build<OptionGroupX>();
                _lv.Data = await OptionGroupX.Query(par.Sql, par.Params);
            }
        }
        #endregion

        #region 交互
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple
                && OwnWin is OptionGroupWin win)
            {
                await win.ParentForm.Update(_lv.SelectedRow?.ID);
            }
        }

        async void OnItemDbClick(object e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple
                && OwnWin is OptionGroupWin win)
            {
                await win.ParentForm.Open(_lv.SelectedRow?.ID);
            }
        }

        async void OnAdd(Mi e)
        {
            if (OwnWin is OptionGroupWin win)
            { 
                await win.ParentForm.Open(-1);
            }
        }

        async void OnEdit(Mi e)
        {
            if (OwnWin is OptionGroupWin win)
            { 
                await win.ParentForm.Open(e.Row?.ID);
            }
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple
                && OwnWin is OptionGroupWin win)
            {
                NaviTo(new List<Tab> { win.OptionList });
            }
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }
            
            var d = e.Data.To<OptionGroupX>();
            if (await d.Delete())
            {
                await Refresh();
            }
        }
        #endregion
    }
}