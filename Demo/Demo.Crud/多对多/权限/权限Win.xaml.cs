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
    [View("权限")]
    public partial class 权限Win : Win
    {
        readonly 权限Form _mainForm;

        public 权限Win()
        {
            InitializeComponent();
            _mainForm = new 权限Form { OwnWin = this };
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
            _mainList.Navi += () => NaviTo(_角色List.Title);

            _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
            _mainForm.UpdateRelated += e => _角色List.Query(e.ID);
        }
    }
}