#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    [View("供应商管理")]
    public partial class 供应商Win : Win
    {
        readonly 供应商Form _form;

        public 供应商Win()
        {
            InitializeComponent();
            _form = new 供应商Form { OwnWin = this };
            Attach();
        }

        void Attach()
        {
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