#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    public partial class $parentroot$List : Tab
    {
        public $parentroot$List()
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

        async void OnAdd(object sender, Mi e)
        {
            NaviToChild();
            await _win.ParentForm.Update(-1);
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            NaviToChild();
            if (e.IsChanged)
                await _win.ParentForm.Update(e.Row.ID);
        }

        void NaviToChild()
        {
            if (Kit.IsPhoneUI)
                NaviTo(new List<Tab> { _win.ParentForm$childtabs$ });
        }

        #region 搜索
        /// <summary>
        /// 查询参数
        /// </summary>
        public QueryClause Clause { get; set; }

        void OnToSearch(object sender, Mi e)
        {
            if (_dlgQuery == null)
                CreateQueryDlg();
            _dlgQuery.Show();
        }

        async void Query()
        {
            if (Clause == null)
            {
                _lv.Data = await $entity$.Query();
            }
            else
            {
                _lv.Data = await $entity$.Query(Clause.Where, Clause.Params);
            }
        }

        void CreateQueryDlg()
        {
            Tabs tbs = new Tabs();
            var fs = new FuzzySearch();
            tbs.Items.Add(fs);
            fs.Search += (s, e) =>
            {
                if (string.IsNullOrEmpty(e))
                {
                    Clause = null;
                }
                else
                {
                    var clause = new QueryClause();
                    clause.Params = new Dict { { "input", $"%{e}%" } };
                    clause.Where = @"$blurclause$";
                    Clause = clause;
                }
                Query();
                _dlgQuery.Close();
            };

            var qs = new $parentroot$Query();
            qs.Query += (s, e) =>
            {
                Clause = e;
                Query();
                _dlgQuery.Close();
            };
            tbs.Items.Add(qs);

            _dlgQuery = new Dlg
            {
                Title = "搜索",
                Content = tbs,
                IsPinned = true
            };

            if (!Kit.IsPhoneUI)
            {
                _dlgQuery.WinPlacement = DlgPlacement.CenterScreen;
                _dlgQuery.MinWidth = 300;
                _dlgQuery.MaxWidth = Kit.ViewWidth / 4;
                _dlgQuery.MinHeight = 500;
                _dlgQuery.ShowVeil = true;
            }
        }

        Dlg _dlgQuery;
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
                var ls = _lv.SelectedItems.Cast<$entity$> ().ToList();
                if (await ls.Delete())
                    Query();
            }
            else if (await e.Data.To<$entity$> ().Delete())
            {
                Query();
            }
            _win.ChildForm.BackToHome();
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

        $parentroot$Win _win => ($parentroot$Win)OwnWin;
    }
}