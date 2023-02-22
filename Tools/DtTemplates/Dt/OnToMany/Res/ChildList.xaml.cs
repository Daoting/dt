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
    public partial class $parentroot$$childroot$List : Tab
    {
        long _parentID;
        $parentroot$$childroot$Form _form;

        public $parentroot$$childroot$List()
        {
            InitializeComponent();
        }

        public void Update(long p_parentID)
        {
            _parentID = p_parentID;
            Menu["增加"].IsEnabled = _parentID > 0;
            Refresh();
            _win.ChildForm.BackToHome();
        }

        public async void Refresh()
        {
            if (_parentID > 0)
            {
                _lv.Data = await $entity$.Query("$parentidname$=@ParentID", new Dict { { "ParentID", _parentID.ToString() } });
            }
            else
            {
                _lv.Data = null;
            }
        }

        void OnAdd(object sender, Mi e)
        {
            ShowForm(-1);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (_lv.SelectionMode != Base.SelectionMode.Multiple)
                ShowForm(e.Row.ID);
        }

        async void ShowForm(long p_id)
        {
            if (_form == null)
                _form = new $parentroot$$childroot$Form();
            _win.ChildForm.Toggle(_form);
            await _form.Update(p_id, _parentID);
        }

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
                    Refresh();
            }
            else if (await e.Data.To<$entity$> ().Delete())
            {
                Refresh();
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