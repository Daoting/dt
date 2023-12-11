﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public partial class 权限角色List : Tab
    {
        #region 构造方法
        public 权限角色List()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
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
                _lv.Data = await 角色X.Query($"where exists ( select role_id from demo_角色权限 b where a.ID = b.role_id and prv_id={_releatedID} )");
            }
            else
            {
                _lv.Data = null;
            }
        }
        #endregion

        #region 交互
        async void OnAdd(Mi e)
        {
            var dlg = new 角色4权限Dlg();
            if (await dlg.Show(_releatedID, e))
            {
                var ls = new List<角色权限X>();
                foreach (var row in dlg.SelectedRows)
                {
                    var x = new 角色权限X(RoleID: row.ID, PrvID: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Save())
                    Refresh();
            }
        }
        
        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除关联吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }
            
            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                var ls = new List<角色权限X>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new 角色权限X(RoleID: row.ID, PrvID: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Delete())
                    Refresh();
            }
            else
            {
                var x = new 角色权限X(RoleID: e.Row.ID, PrvID: _releatedID);
                if (await x.Delete())
                    Refresh();
            }
        }
        #endregion

        #region 选择
        void OnSelectAll(Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(Mi e)
        {
            _lv.SelectionMode = SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(Mi e)
        {
            _lv.SelectionMode = SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 内部
        权限Win _win => OwnWin as 权限Win;
        long _releatedID;
        #endregion
    }
}