#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
            if (string.IsNullOrEmpty(e))
            {
                _list.OnSearch(null);
            }
            else
            {
                var clause = new QueryClause();
                clause.Params = new Dict { { "input", $"%{e}%" } };
                clause.Where = @"主表名称 LIKE @input OR 限长4 LIKE @input OR 不重复 LIKE @input OR 扩展1名称 LIKE @input OR 扩展2名称 LIKE @input OR 值变事件 LIKE @input";
                _list.OnSearch(clause);
            }
        }

        void OnQuery(object sender, QueryClause e)
        {
            _list.OnSearch(e);
        }
    }
}