#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.单实体
{
    public partial class 虚拟List : Tab
    {
        public 虚拟List()
        {
            InitializeComponent();
        }

        public void Update()
        {
            Query();
        }

        protected override void OnInit(object p_params)
        {
            Query();
        }

        void OnAdd(object sender, Mi e)
        {
            _win.Form.Update(-1);
            NaviTo(_win.Form);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Form.Update(e.Row.ID);
            NaviTo(_win.Form);
        }

        #region 搜索
        /// <summary>
        /// 获取设置查询内容
        /// </summary>
        public QueryClause Clause { get; set; }

        public void OnSearch(QueryClause p_clause)
        {
            Clause = p_clause;
            Query();
            NaviTo(this);
        }

        void OnToSearch(object sender, Mi e)
        {
            NaviTo(new List<Tab> { _win.Search, _win.Query });
        }

        async void Query()
        {
            if (Clause == null)
            {
                _lv.Data = await VirX<主表X, 扩展1X, 扩展2X>.Query(null);
            }
            else
            {
                _lv.Data = await VirX<主表X, 扩展1X, 扩展2X>.Query(Clause.Where, Clause.Params);
            }
        }
        #endregion

        #region 删除
        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<VirX<主表X, 扩展1X, 扩展2X>> ().ToList();
                if (await ls.Delete())
                    Query();
            }
            else if (await e.Data.To<VirX<主表X, 扩展1X, 扩展2X>> ().Delete())
            {
                Query();
            }
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 视图
        private void OnListSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["ListView"], ViewMode.List);
        }

        private void OnTableSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        private void OnTileSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["TileView"], ViewMode.Tile);
        }
        #endregion

        虚拟Win _win => (虚拟Win)OwnWin;
    }
}