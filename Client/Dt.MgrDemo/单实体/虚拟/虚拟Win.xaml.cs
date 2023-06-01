#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.单实体
{
    [View("虚拟Win")]
    public partial class 虚拟Win : Win
    {
        public 虚拟Win()
        {
            InitializeComponent();
        }

        public 虚拟List List => _list;

        public 虚拟Form Form => _form;

        public FuzzySearch Search => _search;

        public 虚拟Query Query => _query;

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