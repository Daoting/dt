#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    [View("视图Win")]
    public partial class 视图Win : Win
    {
        public 视图Win()
        {
            InitializeComponent();
        }

        public 视图List List => _list;

        public 视图Form Form => _form;

        public FuzzySearch Search => _search;

        public 视图Query Query => _query;

        void OnSearch(object sender, string e)
        {
            _list.OnSearch(new QueryClause(e));
        }

        void OnQuery(object sender, QueryClause e)
        {
            _list.OnSearch(e);
        }
    }
}