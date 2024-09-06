#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    [View("角色")]
    public partial class 角色Win : Win
    {
        readonly 角色Form _mainForm;

        public 角色Win()
        {
            InitializeComponent();
            _mainForm = new 角色Form { OwnWin = this };
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
            _mainList.Navi += () => NaviTo(_权限List.Title + "," + _用户List.Title);

            _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
            _mainForm.UpdateRelated += e => 
            {
                _权限List.Query(e.ID);
                _用户List.Query(e.ID);
            };
        }
    }
}