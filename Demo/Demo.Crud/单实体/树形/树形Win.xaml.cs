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
    [View("树形")]
    public partial class 树形Win : Win
    {
        readonly 树形Form _form;

        public 树形Win()
        {
            InitializeComponent();
            _form = new 树形Form { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _tree.Msg += e => _list.Query(e.ID);
            _tree.Navi += () => NaviTo(_list.Title);
            _query.Query += e =>
            {
                _list.Query(e);
                NaviTo(_list.Title);
            };
            _list.Msg += e => _ = _form.Query(e);
            _form.UpdateList += e => _ = _list.Refresh(e.ID);
        }
    }
}