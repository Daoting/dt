#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    [View(LobViews.参数定义)]
    public partial class ParamsWin : Win
    {
        readonly ParamsForm _form;

        public ParamsWin()
        {
            InitializeComponent();
            _form = new ParamsForm { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _query.Search += e =>
            {
                _list.Query(new QueryClause(e));
                NaviTo(_list.Title);
            };

            _list.Msg += e => _ = _form.Query(e);

            _form.UpdateList += e => _ = _list.Refresh(e.ID);
        }
    }
}