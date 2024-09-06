#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    [View("部门管理")]
    public partial class 部门Win : Win
    {
        readonly 部门Form _mainForm;

        public 部门Win()
        {
            InitializeComponent();
            _mainForm = new 部门Form { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _tree.Msg += e => _mainList.Query(e.ID);
            _tree.Navi += () => NaviTo(_mainList.Title);

            _mainList.Msg += e => _ = _mainForm.Query(e);

            _mainForm.UpdateList += e =>
            {
                _ = _mainList.Refresh(e.ID);
                _ = _tree.Refresh();
            };
        }
    }
}