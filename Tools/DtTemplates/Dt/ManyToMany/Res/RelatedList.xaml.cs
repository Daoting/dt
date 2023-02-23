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
    public partial class $mainroot$$childroot$List : Tab
    {
        long _releatedID;

        public $mainroot$$childroot$List()
        {
            InitializeComponent();
        }

        public void Update(long p_releatedID)
        {
            _releatedID = p_releatedID;
            Menu["添加"].IsEnabled = _releatedID > 0;
            Refresh();
        }

        public async void Refresh()
        {
            if (_releatedID > 0)
            {
                _lv.Data = await $entity$.Query("$whereclause$", new Dict { { "ReleatedID", _releatedID.ToString() } });
            }
            else
            {
                _lv.Data = null;
            }
        }

        async void OnAdd(object sender, Mi e)
        {
            var dlg = new $selectdlg$();
            if (await dlg.Show(_releatedID, e))
            {
                var ls = new List<$relatedentity$>();
                foreach (var row in dlg.SelectedRows)
                {
                    var x = new $relatedentity$($relatedid$: row.ID, $mainrelatedid$: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Save())
                    Refresh();
            }
        }

        #region 删除
        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除关联吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }
            
            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                var ls = new List<$relatedentity$>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new $relatedentity$($relatedid$: row.ID, $mainrelatedid$: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Delete())
                    Refresh();
            }
            else
            {
                var x = new $relatedentity$($relatedid$: e.Row.ID, $mainrelatedid$: _releatedID);
                if (await x.Delete())
                    Refresh();
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

        $mainroot$Win _win => ($mainroot$Win)OwnWin;
    }
}