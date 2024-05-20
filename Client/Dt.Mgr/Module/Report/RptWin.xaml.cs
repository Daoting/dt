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
        public RptWin()
        {
            InitializeComponent();
            Form = new RptForm { OwnWin = this };
        }

        public RptList List => _list;

        public RptForm Form { get; }

        public FuzzySearch Query => _query;

        void OnSearch(string e)
        {
            _list.OnSearch(e);
        }
    }
}