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

namespace Dt.Mgr.Rbac
{
    [View(LobViews.系统角色)]
    public partial class RoleWin : Win
    {
        readonly RoleForm _mainForm;
        
        public RoleWin()
        {
            InitializeComponent();
            _mainForm = new RoleForm { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _query.Search += e =>
            {
                _mainList.Query(new QueryClause(e));
                NaviTo(_mainList.Title);
            };

            _mainList.Msg += e => _ = _mainForm.Query(e);
            _mainList.Navi += () => NaviTo(_userList.Title + "," + _menuList.Title + "," + _permissionList.Title + "," + _groupList.Title);

            _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
            _mainForm.UpdateRelated += e =>
            {
                _userList.Query(e.ID);
                _menuList.Query(e.ID);
                _permissionList.Query(e.ID);
                _groupList.Query(e.ID);
            };
        }
    }
}