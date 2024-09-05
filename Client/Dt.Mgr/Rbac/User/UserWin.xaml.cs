#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    [View(LobViews.用户账号)]
    public partial class UserWin : Win
    {
        readonly UserForm _mainForm;
        
        public UserWin()
        {
            InitializeComponent();
            _mainForm = new UserForm { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _query.Query += e =>
            {
                _mainList.Query(e);
                NaviTo(_mainList.Title);
            };

            _mainList.Msg += e => _ = _mainForm.Query(e);
            _mainList.Navi += () => NaviTo(_groupList.Title + "," + _roleList.Title + "," + _menuList.Title + "," + _perList.Title);

            _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
            _mainForm.UpdateRelated += e =>
            {
                _groupList.Query(e.ID);
                _roleList.Query(e.ID);
                _menuList.Query(e.ID);
                _perList.Query(e.ID);
            };
        }
    }
}