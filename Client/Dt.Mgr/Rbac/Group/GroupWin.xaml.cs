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
    [View(LobViews.分组管理)]
    public partial class GroupWin : Win
    {
        readonly GroupForm _mainForm;
        
        public GroupWin()
        {
            InitializeComponent();
            _mainForm = new GroupForm { OwnWin = this };
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
            _mainList.Navi += () => NaviTo(_userList.Title + "," + _roleList.Title);

            _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
            _mainForm.UpdateRelated += e =>
            {
                _userList.Query(e.ID);
                _roleList.Query(e.ID);
            };
        }
    }
}