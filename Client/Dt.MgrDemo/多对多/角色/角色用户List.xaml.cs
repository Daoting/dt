﻿#region 文件描述
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

namespace Dt.MgrDemo.多对多
{
    public partial class 角色用户List : Tab
    {
        long _releatedID;

        public 角色用户List()
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
                _lv.Data = await 用户X.Query("where exists ( select UserID from demo_用户角色 b where a.ID = b.UserID and RoleID=@ReleatedID )", new Dict { { "ReleatedID", _releatedID.ToString() } });
            }
            else
            {
                _lv.Data = null;
            }
        }

        async void OnAdd(object sender, Mi e)
        {
            var dlg = new 用户4角色Dlg();
            if (await dlg.Show(_releatedID, e))
            {
                var ls = new List<用户角色X>();
                foreach (var row in dlg.SelectedRows)
                {
                    var x = new 用户角色X(UserID: row.ID, RoleID: _releatedID);
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
                var ls = new List<用户角色X>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new 用户角色X(UserID: row.ID, RoleID: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Delete())
                    Refresh();
            }
            else
            {
                var x = new 用户角色X(UserID: e.Row.ID, RoleID: _releatedID);
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

        角色Win _win => (角色Win)OwnWin;
    }
}