#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    [View("$clsroot$Win")]
    public partial class $clsroot$Win : Win
    {
        public $clsroot$Win()
        {
            InitializeComponent();
        }

        public $clsroot$List List => _list;

        public $clsroot$Form Form => _form;

        public FuzzySearch Search => _search;

        public $clsroot$Query Query => _query;

        void OnSearch(object sender, string e)
        {
            if (string.IsNullOrEmpty(e))
            {
                _list.OnSearch(null);
            }
            else
            {
                var clause = new QueryClause();
                clause.Params = new Dict { { "input", $"%{e}%" } };
                clause.Where = @"$blurclause$";
                _list.OnSearch(clause);
            }
        }

        void OnQuery(object sender, QueryClause e)
        {
            _list.OnSearch(e);
        }
    }
}