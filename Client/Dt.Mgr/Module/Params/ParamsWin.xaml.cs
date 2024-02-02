#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
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
        public ParamsWin()
        {
            InitializeComponent();
        }

        public ParamsList List => _list;

        public ParamsForm Form => _form;

        public FuzzySearch Search => _search;

        void OnSearch(string e)
        {
            _list.OnSearch(new QueryClause(e));
        }
    }
}