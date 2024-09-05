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
    [View(LobViews.报表设计)]
    public partial class RptWin : Win
    {
        readonly RptForm _form;

        public RptWin()
        {
            InitializeComponent();
            _form = new RptForm { OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _query.Search += e =>
            {
                _list.Query(new QueryClause(e));
                NaviTo(_list.Title);
            };

            _list.Msg += async e =>
            {
                if (e.Event == LvEventType.DbClick)
                {
                    await Rpt.ShowDesign(new AppRptDesignInfo(e.Data as RptX));
                }
                else
                {
                    await _form.Query(e);
                }
            };

            _form.UpdateList += e => _ = _list.Refresh(e.ID);
        }
    }
}