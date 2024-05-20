#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public partial class 角色权限List : LvTab
    {
        #region 变量
        long _releatedID;
        #endregion
        
        #region 构造
        public 角色权限List()
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
                _lv.Data = await 权限X.Query($"where exists ( select prv_id from demo_角色权限 b where a.ID = b.prv_id and role_id={_releatedID} )");
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
            var dlg = new 权限4角色();
            if (await dlg.Show(_releatedID, e))
            {
                var ls = new List<角色权限X>();
                foreach (var row in dlg.SelectedRows)
                {
                    var x = new 角色权限X(PrvID: row.ID, RoleID: _releatedID);
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
                var ls = new List<角色权限X>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new 角色权限X(PrvID: row.ID, RoleID: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Delete())
                    await Refresh();
            }
            else
            {
                var x = new 角色权限X(PrvID: e.Row.ID, RoleID: _releatedID);
                if (await x.Delete())
                    await Refresh();
            }
        }
        #endregion
    }
}