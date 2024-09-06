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
    [View("物资入出类别")]
    public partial class 入出类别Win : Win
    {
        readonly 入出类别Form _form;

        public 入出类别Win()
        {
            InitializeComponent();
            _form = new 入出类别Form { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _query.Query += e =>
            {
                _list.Query(e);
                NaviTo(_list.Title);
            };
            //_list.Msg += e => _ = _form.Query(e);
            //_form.UpdateList += e => _ = _list.Refresh(e.ID);
        }
    }
}