﻿#region 文件描述
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
    public partial class 用户角色List : LvTab
    {
        #region 构造
        public 用户角色List()
        {
            InitializeComponent();
            _lv.AddMultiSelMenu(Menu);
        }
        #endregion

        #region 公开
        public void Update(long p_releatedID)
        {
            if (_releatedID == p_releatedID)
                return;
            
            _releatedID = p_releatedID;
            Menu["添加"].IsEnabled = _releatedID > 0;
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_releatedID > 0)
            {
                _lv.Data = await 角色X.Query($"where exists ( select role_id from demo_用户角色 b where a.ID = b.role_id and user_id={_releatedID} )");
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
            var dlg = new 角色4用户();
            if (await dlg.Show(_releatedID, e))
            {
                var ls = new List<用户角色X>();
                foreach (var row in dlg.SelectedRows)
                {
                    var x = new 用户角色X(RoleID: row.ID, UserID: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Save())
                    await Refresh();
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
                var ls = new List<用户角色X>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new 用户角色X(RoleID: row.ID, UserID: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Delete())
                    await Refresh();
            }
            else
            {
                var x = new 用户角色X(RoleID: e.Row.ID, UserID: _releatedID);
                if (await x.Delete())
                    await Refresh();
            }
        }
        #endregion

        #region 内部
        用户Win _win => OwnWin as 用户Win;
        long _releatedID;
        #endregion
    }
}